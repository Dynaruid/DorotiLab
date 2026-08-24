using System.Runtime.InteropServices;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DirectComposition;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Host.WindowsAppSdk;

internal sealed class ArmNDualFrontPresenter : IDisposable
{
    private readonly nint _window;
    private readonly int _capacityWidth;
    private readonly int _capacityHeight;
    private readonly object _presentGate = new();
    private readonly object _compositionGate = new();
    private IDXGIFactory2? _factory;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private readonly IDXGISwapChain3?[] _swapChains = new IDXGISwapChain3?[2];
    private readonly nint[] _frameLatencyHandles = new nint[2];
    private ID3D12CommandAllocator? _allocator;
    private ID3D12GraphicsCommandList? _commands;
    private ID3D12Fence? _fence;
    private EventWaitHandle? _fenceEvent;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private ArmNBackingStore? _backing;
    private ArmNCompositionBridge? _composition;
    private ulong _nextFence;
    private ulong _submittedFence;
    private int _visibleSlot;
    private long _pendingEpoch;
    private int _pendingOffsetX;
    private int _pendingOffsetY;
    private int _pendingWidth;
    private int _pendingHeight;
    private bool _disposed;

    internal ArmNDualFrontPresenter(nint window, int capacityWidth, int capacityHeight)
    {
        if (window == 0) throw new ArgumentOutOfRangeException(nameof(window));
        if (capacityWidth <= 0) throw new ArgumentOutOfRangeException(nameof(capacityWidth));
        if (capacityHeight <= 0) throw new ArgumentOutOfRangeException(nameof(capacityHeight));
        _window = window;
        _capacityWidth = capacityWidth;
        _capacityHeight = capacityHeight;
    }

    internal event Action<long, int, int, int, int>? Committed;

    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal long LatestGeometryEpoch => _composition?.LatestEpoch ?? 0;

    internal void StageGeometry(long epoch, int offsetX, int offsetY, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ValidateGeometry(offsetX, offsetY, width, height);
        lock (_compositionGate)
        {
            _pendingEpoch = epoch;
            _pendingOffsetX = offsetX;
            _pendingOffsetY = offsetY;
            _pendingWidth = width;
            _pendingHeight = height;
            _composition?.Stage(epoch, offsetX, offsetY, width, height);
        }
    }

    internal bool RenderAndPresent(
        long epoch,
        int offsetX,
        int offsetY,
        int width,
        int height,
        Func<SKSurface, bool> paint)
    {
        ArgumentNullException.ThrowIfNull(paint);
        ObjectDisposedException.ThrowIf(_disposed, this);
        ValidateGeometry(offsetX, offsetY, width, height);
        lock (_presentGate)
        {
            EnsureInitialized();
            lock (_compositionGate)
            {
                if (_pendingEpoch != epoch) return false;
                _composition!.Stage(epoch, offsetX, offsetY, width, height);
            }
            WaitForGpu();
            _backing!.Surface.Canvas.Clear(SKColors.Transparent);
            if (!paint(_backing.Surface)) return false;
            _backing.Surface.Canvas.Flush();
            _context!.Flush(_backing.Surface);
            _context.Submit(false);

            lock (_compositionGate)
            {
                if (!_composition.IsLatest(epoch)) return false;
            }
            var stagingSlot = 1 - _visibleSlot;
            var swapChain = _swapChains[stagingSlot]!;
            WaitForGpu();
            _allocator!.Reset();
            _commands!.Reset(_allocator);
            using (var buffer = swapChain.GetBuffer<ID3D12Resource>(swapChain.CurrentBackBufferIndex))
            {
                _commands.ResourceBarrier(
                [
                    ResourceBarrier.BarrierTransition(
                        _backing.Resource, ResourceStates.RenderTarget, ResourceStates.CopySource),
                    ResourceBarrier.BarrierTransition(
                        buffer, ResourceStates.Present, ResourceStates.CopyDest),
                ]);
                _commands.CopyResource(buffer, _backing.Resource);
                _commands.ResourceBarrier(
                [
                    ResourceBarrier.BarrierTransition(
                        _backing.Resource, ResourceStates.CopySource, ResourceStates.RenderTarget),
                    ResourceBarrier.BarrierTransition(
                        buffer, ResourceStates.CopyDest, ResourceStates.Present),
                ]);
                _commands.Close();
                _queue!.ExecuteCommandList(_commands);
            }
            _submittedFence = checked(++_nextFence);
            _queue!.Signal(_fence!, _submittedFence).CheckError();

            lock (_compositionGate)
            {
                if (!_composition.IsLatest(epoch)) return false;
            }
            swapChain.Present(0, PresentFlags.None).CheckError();
            WaitForFrontReady(stagingSlot);
            lock (_compositionGate)
            {
                if (!_composition.IsLatest(epoch)) return false;
                _composition.Commit(epoch, stagingSlot, width, height);
            }
            _composition.WaitForCommit(epoch);
            _visibleSlot = stagingSlot;
            Committed?.Invoke(epoch, offsetX, offsetY, width, height);
            return true;
        }
    }

    private void EnsureInitialized()
    {
        if (_device is not null) return;
        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);
        _adapter = FindAdapterForWindow(_factory, _window);
        if (_adapter is null)
        {
            using var factory6 = _factory.QueryInterface<IDXGIFactory6>();
            _adapter = factory6.EnumAdapterByGpuPreference<IDXGIAdapter1>(
                0, GpuPreference.MinimumPower);
        }
        AdapterDescription = _adapter.Description1.Description;
        _device = D3D12CreateDevice<ID3D12Device2>(_adapter, FeatureLevel.Level_11_0);
        _queue = _device.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        _allocator = _device.CreateCommandAllocator(CommandListType.Direct);
        _commands = _device.CreateCommandList<ID3D12GraphicsCommandList>(
            CommandListType.Direct, _allocator, null);
        _commands.Close();
        _fence = _device.CreateFence(0, FenceFlags.None);
        _fenceEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        _backend = new GRVorticeD3DBackendContext
        {
            Adapter = _adapter,
            Device = _device,
            Queue = _queue,
        };
        _context = GRContext.CreateDirect3D(_backend) ??
            throw new InvalidOperationException("Skia could not create the Arm N D3D12 context.");
        _backing = new ArmNBackingStore(_device, _context);
        _backing.EnsureSize(_capacityWidth, _capacityHeight);

        var description = new SwapChainDescription1(
            checked((uint)_capacityWidth), checked((uint)_capacityHeight),
            Format.R8G8B8A8_UNorm, false, Usage.RenderTargetOutput, 2,
            Scaling.Stretch, SwapEffect.FlipSequential, AlphaMode.Premultiplied,
            SwapChainFlags.FrameLatencyWaitableObject);
        for (var slot = 0; slot < _swapChains.Length; slot++)
        {
            using var created = _factory.CreateSwapChainForComposition(_queue, description, null);
            created.BackgroundColor = new Vortice.Mathematics.Color4(0, 0, 0, 0);
            _swapChains[slot] = created.QueryInterface<IDXGISwapChain3>();
            using var swapChain2 = _swapChains[slot]!.QueryInterface<IDXGISwapChain2>();
            swapChain2.MaximumFrameLatency = 1;
            _frameLatencyHandles[slot] = swapChain2.FrameLatencyWaitableObject;
            if (_frameLatencyHandles[slot] == 0)
                throw new InvalidOperationException($"Arm N slot {slot} has no frame-latency handle.");
            ConsumeInitialFrameLatencySignal(_frameLatencyHandles[slot]);
        }
        _composition = new ArmNCompositionBridge(
            _window, _swapChains[0]!, _swapChains[1]!, _capacityWidth, _capacityHeight);
        lock (_compositionGate)
        {
            if (_pendingEpoch != 0)
                _composition.Stage(
                    _pendingEpoch,
                    _pendingOffsetX,
                    _pendingOffsetY,
                    _pendingWidth,
                    _pendingHeight);
        }
    }

    private void ValidateGeometry(int offsetX, int offsetY, int width, int height)
    {
        if (offsetX < 0 || offsetY < 0 || width <= 0 || height <= 0 ||
            offsetX + width > _capacityWidth || offsetY + height > _capacityHeight)
            throw new ArgumentOutOfRangeException(nameof(width),
                $"Geometry {offsetX},{offsetY} {width}x{height} exceeds Arm N capacity {_capacityWidth}x{_capacityHeight}.");
    }

    private void WaitForGpu()
    {
        if (_submittedFence == 0 || _fence!.CompletedValue >= _submittedFence) return;
        _fence.SetEventOnCompletion(_submittedFence, _fenceEvent!).CheckError();
        if (!_fenceEvent!.WaitOne(TimeSpan.FromSeconds(5)))
            throw new TimeoutException($"Arm N D3D12 fence {_submittedFence} timed out.");
    }

    private void WaitForFrontReady(int slot)
    {
        const uint waitObject0 = 0;
        var result = WaitForSingleObject(_frameLatencyHandles[slot], 5000);
        if (result != waitObject0)
            throw new TimeoutException(
                $"Arm N prepared front slot {slot} did not latch (0x{result:X8}).");
    }

    private static void ConsumeInitialFrameLatencySignal(nint handle)
    {
        const uint waitObject0 = 0;
        const uint waitTimeout = 0x00000102;
        var result = WaitForSingleObject(handle, 0);
        if (result is not (waitObject0 or waitTimeout))
            throw new InvalidOperationException(
                $"Initial Arm N frame-latency wait failed (0x{result:X8}).");
    }

    private static IDXGIAdapter1? FindAdapterForWindow(IDXGIFactory2 factory, nint window)
    {
        var monitor = MonitorFromWindow(window, 2);
        if (monitor == 0) return null;
        for (uint adapterIndex = 0; ; adapterIndex++)
        {
            if (factory.EnumAdapters1(adapterIndex, out var adapter).Failure) break;
            var matched = false;
            for (uint outputIndex = 0; ; outputIndex++)
            {
                if (adapter.EnumOutputs(outputIndex, out var output).Failure) break;
                using (output)
                {
                    if (output.Description.Monitor == monitor)
                    {
                        matched = true;
                        break;
                    }
                }
            }
            if (matched) return adapter;
            adapter.Dispose();
        }
        return null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (_presentGate)
        {
            WaitForGpu();
            _backing?.Dispose();
            _composition?.Dispose();
            foreach (var swapChain in _swapChains) swapChain?.Dispose();
            _fenceEvent?.Dispose();
            _context?.Dispose();
            _backend?.Dispose();
            _fence?.Dispose();
            _commands?.Dispose();
            _allocator?.Dispose();
            _queue?.Dispose();
            _device?.Dispose();
            _adapter?.Dispose();
            _factory?.Dispose();
        }
    }

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint window, uint flags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint handle, uint milliseconds);
}

internal sealed class ArmNCompositionBridge : IDisposable
{
    private IDCompositionDevice? _device;
    private IDCompositionTarget? _target;
    private IDCompositionVisual? _root;
    private readonly IDCompositionVisual?[] _visuals = new IDCompositionVisual?[2];
    private long _latestEpoch;
    private long _submittedEpoch;
    private int _offsetX;
    private int _offsetY;
    private int _width;
    private int _height;
    private bool _disposed;

    internal ArmNCompositionBridge(
        nint window,
        IDXGISwapChain3 primary,
        IDXGISwapChain3 alternate,
        int capacityWidth,
        int capacityHeight)
    {
        try
        {
            _device = DComp.DCompositionCreateDevice<IDCompositionDevice>(null!);
            _device.CreateTargetForHwnd(window, true, out _target).CheckError();
            _device.CreateVisual(out _root).CheckError();
            var swapChains = new[] { primary, alternate };
            for (var slot = 0; slot < _visuals.Length; slot++)
            {
                _device.CreateVisual(out var visual).CheckError();
                visual.SetContent(swapChains[slot]).CheckError();
                SetVisualState(visual, 0, 0, 0, 0, capacityWidth, capacityHeight);
                _visuals[slot] = visual;
            }
            _target.SetRoot(_root).CheckError();
            _device.Commit().CheckError();
            _device.WaitForCommitCompletion().CheckError();
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    internal long LatestEpoch => Volatile.Read(ref _latestEpoch);

    internal bool IsLatest(long epoch) => epoch == Volatile.Read(ref _latestEpoch);

    internal void Stage(long epoch, int offsetX, int offsetY, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _offsetX = offsetX;
        _offsetY = offsetY;
        _width = width;
        _height = height;
        Volatile.Write(ref _latestEpoch, epoch);
    }

    internal void Commit(long epoch, int slot, int preparedWidth, int preparedHeight)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsLatest(epoch)) return;
        if ((uint)slot >= _visuals.Length || _visuals[slot] is null)
            throw new ArgumentOutOfRangeException(nameof(slot));
        if (_width != preparedWidth || _height != preparedHeight)
            throw new InvalidOperationException(
                $"Arm N geometry {_width}x{_height} does not match front {preparedWidth}x{preparedHeight}.");
        SetVisualState(_visuals[slot]!, _offsetX, _offsetY, 0, 0, _width, _height);
        _root!.RemoveAllVisuals().CheckError();
        _root.AddVisual(_visuals[slot]!, false, null!).CheckError();
        _device!.Commit().CheckError();
        Volatile.Write(ref _submittedEpoch, epoch);
    }

    internal void WaitForCommit(long epoch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _device!.WaitForCommitCompletion().CheckError();
        if (Volatile.Read(ref _submittedEpoch) != epoch)
            throw new InvalidOperationException(
                $"Arm N completed epoch does not match submitted epoch {epoch}.");
    }

    private static void SetVisualState(
        IDCompositionVisual visual,
        float offsetX,
        float offsetY,
        float clipLeft,
        float clipTop,
        float clipRight,
        float clipBottom)
    {
        visual.SetOffsetX(offsetX).CheckError();
        visual.SetOffsetY(offsetY).CheckError();
        visual.SetClip(new RawRectF(clipLeft, clipTop, clipRight, clipBottom)).CheckError();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_target is not null) _target.SetRoot(null).CheckError();
        if (_device is not null) _device.Commit().CheckError();
        foreach (var visual in _visuals) visual?.Dispose();
        _root?.Dispose();
        _target?.Dispose();
        _device?.Dispose();
        _root = null;
        _target = null;
        _device = null;
    }
}

internal sealed class ArmNBackingStore(ID3D12Device device, GRContext context) : IDisposable
{
    private ID3D12Resource? _resource;
    private GRVorticeD3DTextureResourceInfo? _resourceInfo;
    private GRBackendRenderTarget? _target;
    private SKSurface? _surface;
    private int _width;
    private int _height;

    internal ID3D12Resource Resource => _resource!;
    internal SKSurface Surface => _surface!;

    internal void EnsureSize(int width, int height)
    {
        if (_surface is not null && width == _width && height == _height) return;
        DisposeResources();
        _resource = device.CreateCommittedResource(
            HeapType.Default,
            HeapFlags.None,
            ResourceDescription.Texture2D(
                Format.R8G8B8A8_UNorm,
                checked((uint)width),
                checked((uint)height),
                1, 1, 1, 0,
                ResourceFlags.AllowRenderTarget),
            ResourceStates.RenderTarget,
            null);
        _resourceInfo = new GRVorticeD3DTextureResourceInfo
        {
            Resource = _resource,
            ResourceState = ResourceStates.RenderTarget,
            Format = Format.R8G8B8A8_UNorm,
            SampleCount = 1,
            LevelCount = 1,
        };
        _target = new GRBackendRenderTarget(width, height, _resourceInfo);
        _surface = SKSurface.Create(context, _target, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888) ??
            throw new InvalidOperationException("Skia could not wrap the Arm N backing store.");
        _width = width;
        _height = height;
    }

    private void DisposeResources()
    {
        _surface?.Dispose();
        _surface = null;
        _target?.Dispose();
        _target = null;
        _resourceInfo?.Dispose();
        _resourceInfo = null;
        _resource?.Dispose();
        _resource = null;
        _width = _height = 0;
    }

    public void Dispose() => DisposeResources();
}

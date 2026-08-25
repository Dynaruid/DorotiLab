using System.Runtime.InteropServices;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using Vortice.DXGI;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Host.WindowsAppSdk;

internal sealed class WindowsManagedHwndPresenter : IDisposable
{
    private IDXGIFactory6? _factory;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12InfoQueue? _infoQueue;
    private ID3D12CommandQueue? _queue;
    private ID3D12CommandAllocator? _copyAllocator;
    private ID3D12GraphicsCommandList? _copyCommandList;
    private ID3D12Fence? _fence;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private IDXGISwapChain3? _swapChain;
    private WindowsManagedD3D12BackingStore? _backing;
    private nint _window;
    private ulong _nextFence;
    private ulong _confirmedFence;
    private bool _debugBaselineSealed;
    private bool _disposed;

    internal WindowsManagedHwndPresenter(bool enableDebugLayer)
    {
        DebugLayerEnabled = enableDebugLayer;
    }

    internal bool DebugLayerEnabled { get; }
    internal int Width { get; private set; }
    internal int Height { get; private set; }
    internal ulong DeviceGeneration { get; private set; }
    internal ulong ResizeBuffersCount { get; private set; }
    internal ulong ResizeInvalidCallCount { get; private set; }
    internal ulong PresentCount { get; private set; }
    internal ulong ManagedSubmitFenceCount { get; private set; }
    internal ulong CopyFenceCount { get; private set; }
    internal ulong InitializationDebugMessageCount { get; private set; }
    internal ulong InitializationDebugErrorCount { get; private set; }
    internal ulong OperationalDebugMessageCount { get; private set; }
    internal ulong OperationalDebugErrorCount { get; private set; }
    internal ulong OperationalDebugWarningCount { get; private set; }
    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal void EnsureTarget(nint childWindow, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (childWindow == 0) throw new ArgumentOutOfRangeException(nameof(childWindow));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        EnsureDevice();
        if (_window != 0 && _window != childWindow)
            ReleaseSwapChain();
        _window = childWindow;
        if (_swapChain is null)
        {
            var description = new SwapChainDescription1(
                checked((uint)width), checked((uint)height),
                Format.B8G8R8A8_UNorm, false,
                Usage.RenderTargetOutput, 2, Scaling.None,
                SwapEffect.FlipDiscard, AlphaMode.Ignore, SwapChainFlags.None);
            using var created = _factory!.CreateSwapChainForHwnd(
                _queue!, childWindow, description, null, null);
            _swapChain = created.QueryInterface<IDXGISwapChain3>();
        }
        else if (Width != width || Height != height)
        {
            WaitIdle();
            _backing?.Dispose();
            _backing = null;
            try
            {
                _swapChain.ResizeBuffers(
                    2, checked((uint)width), checked((uint)height),
                    Format.B8G8R8A8_UNorm, SwapChainFlags.None).CheckError();
            }
            catch (SharpGenException exception) when (exception.ResultCode.Code == unchecked((int)0x887A0001))
            {
                ResizeInvalidCallCount++;
                throw;
            }
            ResizeBuffersCount++;
        }
        _backing ??= new WindowsManagedD3D12BackingStore(_device!, _context!);
        _backing.EnsureSize(width, height);
        Width = width;
        Height = height;
    }

    internal void SealInitializationDebugBaseline()
    {
        if (!DebugLayerEnabled || _infoQueue is null || _debugBaselineSealed)
            return;
        var snapshot = CaptureAndClearDebugMessages("initialization");
        InitializationDebugMessageCount += snapshot.Total;
        InitializationDebugErrorCount += snapshot.Errors;
        _debugBaselineSealed = true;
    }

    internal void CaptureOperationalDebugMessages()
    {
        if (!DebugLayerEnabled || _infoQueue is null || !_debugBaselineSealed) return;
        var snapshot = CaptureAndClearDebugMessages("operation");
        OperationalDebugMessageCount += snapshot.Total;
        OperationalDebugErrorCount += snapshot.Errors;
        OperationalDebugWarningCount += snapshot.Warnings;
    }

    internal void RenderAndPresent(Action<SKCanvas> paint)
    {
        ArgumentNullException.ThrowIfNull(paint);
        _ = RenderAndPresent(
            surface =>
            {
                paint(surface.Canvas);
                return true;
            },
            static value => value);
    }

    internal T RenderAndPresent<T>(Func<SKSurface, T> paint, Predicate<T> shouldPresent)
    {
        ArgumentNullException.ThrowIfNull(paint);
        ArgumentNullException.ThrowIfNull(shouldPresent);
        ObjectDisposedException.ThrowIf(_disposed, this);
        var context = _context ?? throw new InvalidOperationException("The managed D3D12 context is unavailable.");
        var backing = _backing ?? throw new InvalidOperationException("The exact backing store is unavailable.");
        var swapChain = _swapChain ?? throw new InvalidOperationException("The HWND swap chain is unavailable.");
        var result = paint(backing.Surface);
        if (!shouldPresent(result)) return result;
        backing.Surface.Canvas.Flush();
        context.Flush(backing.Surface);
        context.Submit(true);
        SignalAndWait();
        ManagedSubmitFenceCount++;

        _copyAllocator!.Reset();
        _copyCommandList!.Reset(_copyAllocator);
        using (var buffer = swapChain.GetBuffer<ID3D12Resource>(swapChain.CurrentBackBufferIndex))
        {
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(backing.Resource, ResourceStates.RenderTarget, ResourceStates.CopySource),
                ResourceBarrier.BarrierTransition(buffer, ResourceStates.Present, ResourceStates.CopyDest),
            ]);
            _copyCommandList.CopyResource(buffer, backing.Resource);
            _copyCommandList.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(backing.Resource, ResourceStates.CopySource, ResourceStates.RenderTarget),
                ResourceBarrier.BarrierTransition(buffer, ResourceStates.CopyDest, ResourceStates.Present),
            ]);
            _copyCommandList.Close();
            _queue!.ExecuteCommandList(_copyCommandList);
        }
        SignalAndWait();
        CopyFenceCount++;
        swapChain.Present(0, PresentFlags.None).CheckError();
        PresentCount++;
        Marshal.ThrowExceptionForHR(DwmFlush());
        return result;
    }

    internal void ResetDevice()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ReleaseDevice();
    }

    private void EnsureDevice()
    {
        if (_device is not null) return;
        if (DebugLayerEnabled)
        {
            using var debug = D3D12GetDebugInterface<ID3D12Debug>();
            debug.EnableDebugLayer();
        }
        _factory = CreateDXGIFactory2<IDXGIFactory6>(DebugLayerEnabled);
        for (uint index = 0; ; index++)
        {
            var result = _factory.EnumAdapterByGpuPreference(
                index, GpuPreference.HighPerformance, out IDXGIAdapter1? candidate);
            if (result == Vortice.DXGI.ResultCode.NotFound) break;
            result.CheckError();
            if ((candidate!.Description1.Flags & AdapterFlags.Software) != 0)
            {
                candidate.Dispose();
                continue;
            }
            try
            {
                _device = D3D12CreateDevice<ID3D12Device2>(candidate, FeatureLevel.Level_11_0);
                _adapter = candidate;
                break;
            }
            catch
            {
                candidate.Dispose();
            }
        }
        if (_device is null || _adapter is null)
            throw new InvalidOperationException("No hardware D3D12 adapter is available.");
        AdapterDescription = _adapter.Description1.Description;
        if (DebugLayerEnabled)
            _infoQueue = _device.QueryInterface<ID3D12InfoQueue>();
        _queue = _device.CreateCommandQueue(CommandListType.Direct, 0, CommandQueueFlags.None, 0);
        _copyAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
        _copyCommandList = _device.CreateCommandList<ID3D12GraphicsCommandList>(
            CommandListType.Direct, _copyAllocator, null);
        _copyCommandList.Close();
        _fence = _device.CreateFence(0, FenceFlags.None);
        _backend = new GRVorticeD3DBackendContext
        {
            Adapter = _adapter,
            Device = _device,
            Queue = _queue,
        };
        _context = GRContext.CreateDirect3D(_backend) ??
            throw new InvalidOperationException("Skia could not create the managed-owner D3D12 context.");
        DeviceGeneration++;
        _debugBaselineSealed = false;
    }

    private void SignalAndWait()
    {
        var target = checked(++_nextFence);
        _queue!.Signal(_fence!, target).CheckError();
        if (_fence!.CompletedValue < target)
        {
            using var completion = new EventWaitHandle(false, EventResetMode.AutoReset);
            _fence.SetEventOnCompletion(target, completion).CheckError();
            if (!completion.WaitOne(TimeSpan.FromSeconds(5)))
                throw new TimeoutException($"Managed D3D12 fence {target} timed out.");
        }
        _confirmedFence = target;
    }

    private void WaitIdle()
    {
        if (_queue is null || _fence is null) return;
        SignalAndWait();
    }

    private void ReleaseSwapChain()
    {
        WaitIdle();
        _backing?.Dispose();
        _backing = null;
        _swapChain?.Dispose();
        _swapChain = null;
        _window = 0;
        Width = Height = 0;
    }

    private void ReleaseDevice()
    {
        if (_device is null) return;
        ReleaseSwapChain();
        _context?.AbandonContext(false);
        _context?.Dispose();
        _context = null;
        _backend?.Dispose();
        _backend = null;
        CaptureOperationalDebugMessages();
        _copyCommandList?.Dispose();
        _copyCommandList = null;
        _copyAllocator?.Dispose();
        _copyAllocator = null;
        _fence?.Dispose();
        _fence = null;
        _nextFence = _confirmedFence = 0;
        _queue?.Dispose();
        _queue = null;
        _infoQueue?.Dispose();
        _infoQueue = null;
        _device?.Dispose();
        _device = null;
        _adapter?.Dispose();
        _adapter = null;
        _factory?.Dispose();
        _factory = null;
        AdapterDescription = "uninitialized";
    }

    private (ulong Total, ulong Errors, ulong Warnings) CaptureAndClearDebugMessages(string stage)
    {
        if (_infoQueue is null) return default;
        var total = _infoQueue.NumStoredMessages;
        ulong errors = 0;
        ulong warnings = 0;
        for (ulong index = 0; index < total; index++)
        {
            var message = _infoQueue.GetMessage(index);
            if (message.Severity is MessageSeverity.Error or MessageSeverity.Corruption)
                errors++;
            else if (message.Severity == MessageSeverity.Warning)
                warnings++;
            Console.Error.WriteLine(
                $"D3D12 {stage} severity={message.Severity} id={(int)message.Id}: {message.Description}");
        }
        _infoQueue.ClearStoredMessages();
        return (total, errors, warnings);
    }

    public void Dispose()
    {
        if (_disposed) return;
        ReleaseDevice();
        _disposed = true;
    }

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();
}

internal sealed class WindowsManagedD3D12BackingStore : IDisposable
{
    private readonly ID3D12Device _device;
    private readonly GRContext _context;
    private ID3D12Resource? _resource;
    private GRVorticeD3DTextureResourceInfo? _resourceInfo;
    private GRBackendRenderTarget? _target;
    private SKSurface? _surface;

    internal WindowsManagedD3D12BackingStore(ID3D12Device device, GRContext context)
    {
        _device = device;
        _context = context;
    }

    internal ID3D12Resource Resource => _resource ??
        throw new InvalidOperationException("The managed exact backing resource is unavailable.");
    internal SKSurface Surface => _surface ??
        throw new InvalidOperationException("The managed exact Skia surface is unavailable.");
    internal int Width { get; private set; }
    internal int Height { get; private set; }

    internal void EnsureSize(int width, int height)
    {
        if (_surface is not null && Width == width && Height == height) return;
        Release();
        var description = ResourceDescription.Texture2D(
            Format.B8G8R8A8_UNorm, checked((uint)width), checked((uint)height),
            1, 1, 1, 0, ResourceFlags.AllowRenderTarget);
        _resource = _device.CreateCommittedResource(
            HeapType.Default, HeapFlags.None, description,
            ResourceStates.RenderTarget, null);
        _resourceInfo = new GRVorticeD3DTextureResourceInfo
        {
            Resource = _resource,
            ResourceState = ResourceStates.RenderTarget,
            Format = Format.B8G8R8A8_UNorm,
            SampleCount = 1,
            LevelCount = 1,
        };
        _target = new GRBackendRenderTarget(width, height, _resourceInfo);
        _surface = SKSurface.Create(
            _context, _target, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888) ??
            throw new InvalidOperationException("Skia could not wrap the managed exact D3D12 backing.");
        Width = width;
        Height = height;
    }

    private void Release()
    {
        _surface?.Dispose();
        _surface = null;
        _target?.Dispose();
        _target = null;
        _resourceInfo?.Dispose();
        _resourceInfo = null;
        _resource?.Dispose();
        _resource = null;
        Width = Height = 0;
    }

    public void Dispose() => Release();
}

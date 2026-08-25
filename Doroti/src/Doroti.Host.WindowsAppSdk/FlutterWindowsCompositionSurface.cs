using System.ComponentModel;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using SkiaSharp;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DirectComposition;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Vortice.Direct3D12.D3D12;
using static Vortice.DXGI.DXGI;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Product presentation surface for the Flutter-style host. A single
/// premultiplied-alpha DirectComposition front is attached to the child HWND,
/// which is also the input and exact-metrics authority. The backing/front
/// capacity covers the virtual desktop, so WM_SIZE never destroys the visible
/// native surface.
/// </summary>
internal sealed class FlutterWindowsCompositionSurface : IFlutterWindowsScheduledSurface
{
    private const int SmCxVirtualScreen = 78;
    private const int SmCyVirtualScreen = 79;
    private const uint WaitObject0 = 0;
    private static readonly TimeSpan ProvisionalAdmissionTimeout = TimeSpan.FromMilliseconds(100);
    private readonly nint _compositionHwnd;
    private readonly nint _childHwnd;
    private readonly object _provisionalGate = new();
    private readonly int _rasterManagedThreadId;
    private readonly uint _rasterNativeThreadId;
    private IDXGIFactory2? _factory;
    private IDXGIAdapter1? _adapter;
    private ID3D12Device2? _device;
    private ID3D12CommandQueue? _queue;
    private ID3D12CommandAllocator? _allocator;
    private ID3D12GraphicsCommandList? _commands;
    private ID3D12Fence? _fence;
    private EventWaitHandle? _fenceEvent;
    private GRVorticeD3DBackendContext? _backend;
    private GRContext? _context;
    private ArmNBackingStore? _backing;
    private IDXGISwapChain3? _swapChain;
    private IDCompositionDevice? _compositionDevice;
    private IDCompositionTarget? _compositionTarget;
    private IDCompositionVisual? _rootVisual;
    private IDCompositionVisual? _contentVisual;
    private WindowsViewMetrics? _targetMetrics;
    private ulong _nextFence;
    private ulong _submittedFence;
    private int _capacityWidth;
    private int _capacityHeight;
    private long _surfaceGeneration;
    private long _presentAttemptCount;
    private long _successfulPresentCount;
    private long _capacityGrowthCount;
    private long _exactExtentMismatchCount;
    private bool _firstCommitCompleted;
    private bool _recoveryPending;
    private bool _disposed;
    private FlutterWindowsProvisionalResize? _provisionalResize;

    private FlutterWindowsCompositionSurface(nint compositionHwnd, nint childHwnd)
    {
        _compositionHwnd = compositionHwnd;
        _childHwnd = childHwnd;
        _rasterManagedThreadId = Environment.CurrentManagedThreadId;
        _rasterNativeThreadId = GetCurrentThreadId();
    }

    internal static FlutterWindowsCompositionSurface CreateOnCurrentRasterThread(
        nint compositionHwnd,
        nint childHwnd,
        WindowsViewMetrics initialMetrics)
    {
        ArgumentNullException.ThrowIfNull(initialMetrics);
        if (compositionHwnd == 0 || !IsWindow(compositionHwnd))
            throw new ArgumentOutOfRangeException(nameof(compositionHwnd));
        if (childHwnd == 0 || !IsWindow(childHwnd))
            throw new ArgumentOutOfRangeException(nameof(childHwnd));
        var result = new FlutterWindowsCompositionSurface(compositionHwnd, childHwnd);
        try
        {
            result.Initialize(initialMetrics.PhysicalWidth, initialMetrics.PhysicalHeight);
            result.UpdateForMetrics(initialMetrics);
            return result;
        }
        catch
        {
            result.DisposeAfterFailedCreate();
            throw;
        }
    }

    internal FlutterWindowsCompositionSurfaceSnapshot Snapshot => new(
        _compositionHwnd,
        _childHwnd,
        _targetMetrics,
        _surfaceGeneration,
        _capacityWidth,
        _capacityHeight,
        Interlocked.Read(ref _presentAttemptCount),
        Interlocked.Read(ref _successfulPresentCount),
        Interlocked.Read(ref _capacityGrowthCount),
        Interlocked.Read(ref _exactExtentMismatchCount),
        AdapterDescription,
        SoftwareFallback,
        _rasterManagedThreadId,
        _rasterNativeThreadId,
        _disposed);

    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal bool SoftwareFallback =>
        AdapterDescription.Contains("software", StringComparison.OrdinalIgnoreCase) ||
        AdapterDescription.Contains("basic render driver", StringComparison.OrdinalIgnoreCase);

    internal FlutterWindowsProvisionalResize BeginProvisionalResize(WindowsViewMetrics targetMetrics)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        ThrowIfDisposed();
        lock (_provisionalGate)
        {
            _provisionalResize?.Cancel();
            var state = new FlutterWindowsProvisionalResize(targetMetrics);
            _provisionalResize = state;
            return state;
        }
    }

    internal bool AdmitProvisionalResize(
        WindowsViewMetrics admittedMetrics,
        out bool preparedBeforeAdmission)
    {
        ArgumentNullException.ThrowIfNull(admittedMetrics);
        lock (_provisionalGate)
        {
            if (_provisionalResize is not { } state ||
                !ReferenceEquals(state.TargetMetrics, admittedMetrics))
            {
                preparedBeforeAdmission = false;
                return false;
            }
            preparedBeforeAdmission = state.IsPrepared;
            state.Admit();
            return true;
        }
    }

    internal void CancelProvisionalResize(WindowsViewMetrics targetMetrics)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        lock (_provisionalGate)
        {
            if (_provisionalResize is not { } state ||
                !ReferenceEquals(state.TargetMetrics, targetMetrics))
                return;
            state.Cancel();
            _provisionalResize = null;
        }
    }

    public FlutterWindowsAngleEglSurfaceUpdateResult UpdateForMetrics(
        WindowsViewMetrics targetMetrics)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        EnsureRasterThread();
        ThrowIfDisposed();
        if (!targetMetrics.HasDrawableSize)
        {
            _targetMetrics = targetMetrics;
            return new(false, false, false, _surfaceGeneration, 0, 0);
        }

        var provisional = GetProvisionalResize(targetMetrics);
        if (provisional is null) ValidateExactChildTarget(targetMetrics);
        var grew = EnsureCapacity(targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
        _targetMetrics = targetMetrics;
        return new(false, grew, false, _surfaceGeneration,
            targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
    }

    public FlutterWindowsAngleEglPresentResult RenderAndSwap(
        WindowsViewMetrics targetMetrics,
        Action<SKSurface> paint)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        ArgumentNullException.ThrowIfNull(paint);
        EnsureRasterThread();
        ThrowIfDisposed();
        if (_targetMetrics != targetMetrics || !targetMetrics.HasDrawableSize)
            throw new InvalidOperationException(
                "The composition front can present only its exact admitted WindowsViewMetrics target.");
        var provisional = GetProvisionalResize(targetMetrics);
        if (provisional is null) ValidateExactChildTarget(targetMetrics);
        if (_recoveryPending)
        {
            _recoveryPending = false;
            RecreateGraphics(targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
            _targetMetrics = targetMetrics;
        }

        WaitForGpu();
        var surface = _backing?.Surface ?? throw new ObjectDisposedException(nameof(FlutterWindowsCompositionSurface));
        var canvas = surface.Canvas;
        // The backing store covers the virtual desktop so the native front is
        // never recreated in the resize hot path. Clear only the exact client
        // viewport, however. Clearing the full capacity made a normal-size
        // window pay the fill cost of every monitor on every resize frame.
        // A later expansion has a larger exact clip and therefore clears every
        // newly exposed pixel before the scene is painted.
        var saveCount = canvas.Save();
        try
        {
            canvas.ClipRect(
                new SKRect(0, 0, targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight),
                SKClipOperation.Intersect,
                antialias: false);
            canvas.Clear(SKColors.Transparent);
            paint(surface);
        }
        finally
        {
            canvas.RestoreToCount(saveCount);
        }
        surface.Canvas.Flush();
        _context!.Flush(surface);
        _context.Submit(false);

        if (provisional is not null)
        {
            provisional.MarkPrepared();
            if (!provisional.WaitForAdmission(ProvisionalAdmissionTimeout))
                throw new InvalidOperationException(
                    "The provisional composition frame was not admitted by matching child HWND geometry.");
            ValidateExactChildTarget(targetMetrics);
        }

        _allocator!.Reset();
        _commands!.Reset(_allocator);
        using (var buffer = _swapChain!.GetBuffer<ID3D12Resource>(_swapChain.CurrentBackBufferIndex))
        {
            _commands.ResourceBarrier(
            [
                ResourceBarrier.BarrierTransition(
                    _backing!.Resource, ResourceStates.RenderTarget, ResourceStates.CopySource),
                ResourceBarrier.BarrierTransition(
                    buffer, ResourceStates.Present, ResourceStates.CopyDest),
            ]);
            // Copy only pixels that the child HWND can expose. CopyResource
            // moved the entire virtual-desktop capacity for every frame and
            // needlessly reduced resize cadence, especially while DWM also
            // had to move the window origin for a left/top drag.
            _commands.CopyTextureRegion(
                new TextureCopyLocation(buffer, 0),
                0,
                0,
                0,
                new TextureCopyLocation(_backing.Resource, 0),
                new Box(
                    0,
                    0,
                    0,
                    targetMetrics.PhysicalWidth,
                    targetMetrics.PhysicalHeight,
                    1));
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
        Interlocked.Increment(ref _presentAttemptCount);
        _swapChain.Present(0, PresentFlags.None).CheckError();
        _compositionDevice!.Commit().CheckError();
        if (!_firstCommitCompleted)
        {
            _compositionDevice.WaitForCommitCompletion().CheckError();
            _firstCommitCompleted = true;
        }
        Interlocked.Increment(ref _successfulPresentCount);
        if (provisional is not null) CompleteProvisionalResize(provisional);
        return new(
            targetMetrics.PhysicalWidth,
            targetMetrics.PhysicalHeight,
            _surfaceGeneration,
            RecoveredFromContextLoss: false,
            SuccessfulSwap: true);
    }

    internal void RequestLifecycleRecovery()
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        _recoveryPending = true;
    }

    private void Initialize(int initialWidth, int initialHeight)
    {
        EnsureRasterThread();
        _factory = CreateDXGIFactory2<IDXGIFactory2>(false);
        _adapter = FindAdapterForWindow(_factory, _compositionHwnd);
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
            throw new InvalidOperationException("Skia could not create the Windows composition D3D12 context.");

        var virtualWidth = Math.Max(1, GetSystemMetrics(SmCxVirtualScreen));
        var virtualHeight = Math.Max(1, GetSystemMetrics(SmCyVirtualScreen));
        _capacityWidth = Math.Max(initialWidth, virtualWidth);
        _capacityHeight = Math.Max(initialHeight, virtualHeight);
        CreateCapacityResources();
    }

    private void CreateCapacityResources()
    {
        _backing = new ArmNBackingStore(_device!, _context!);
        _backing.EnsureSize(_capacityWidth, _capacityHeight);
        _backing.Surface.Canvas.Clear(SKColors.Transparent);
        _backing.Surface.Canvas.Flush();
        _context!.Flush(_backing.Surface);
        _context.Submit(false);
        var description = new SwapChainDescription1(
            checked((uint)_capacityWidth),
            checked((uint)_capacityHeight),
            Format.R8G8B8A8_UNorm,
            false,
            Usage.RenderTargetOutput,
            2,
            Scaling.Stretch,
            SwapEffect.FlipSequential,
            AlphaMode.Premultiplied,
            SwapChainFlags.FrameLatencyWaitableObject);
        using var created = _factory!.CreateSwapChainForComposition(_queue!, description, null);
        created.BackgroundColor = new Vortice.Mathematics.Color4(0, 0, 0, 0);
        _swapChain = created.QueryInterface<IDXGISwapChain3>();
        using (var swapChain2 = _swapChain.QueryInterface<IDXGISwapChain2>())
            swapChain2.MaximumFrameLatency = 1;

        _compositionDevice = DComp.DCompositionCreateDevice<IDCompositionDevice>(null!);
        // Keep the composition target on the same child HWND that owns exact
        // metrics. Its parent moves the child's screen origin during a left or
        // top resize, avoiding an independent top-level visual-origin update.
        _compositionDevice.CreateTargetForHwnd(
            _childHwnd, topmost: true, out _compositionTarget).CheckError();
        _compositionDevice.CreateVisual(out _rootVisual).CheckError();
        _compositionDevice.CreateVisual(out _contentVisual).CheckError();
        _contentVisual.SetContent(_swapChain).CheckError();
        // The child HWND clips the full-capacity DirectComposition target. The
        // unused portion is cleared transparent on every frame; it must never
        // retain a scene from an older layout generation.
        _rootVisual.AddVisual(_contentVisual, false, null!).CheckError();
        _compositionTarget.SetRoot(_rootVisual).CheckError();
        _compositionDevice.Commit().CheckError();
        checked { _surfaceGeneration++; }
    }

    private bool EnsureCapacity(int width, int height)
    {
        if (width <= _capacityWidth && height <= _capacityHeight) return false;
        _capacityWidth = Math.Max(width, _capacityWidth);
        _capacityHeight = Math.Max(height, _capacityHeight);
        RecreateGraphics(width, height);
        Interlocked.Increment(ref _capacityGrowthCount);
        return true;
    }

    private void RecreateGraphics(int minimumWidth, int minimumHeight)
    {
        WaitForGpu();
        _capacityWidth = Math.Max(_capacityWidth, minimumWidth);
        _capacityHeight = Math.Max(_capacityHeight, minimumHeight);
        DisposeCapacityResources();
        CreateCapacityResources();
        _firstCommitCompleted = false;
    }

    private void ValidateExactChildTarget(WindowsViewMetrics targetMetrics)
    {
        if (!GetClientRect(_childHwnd, out var clientRect))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "GetClientRect(child composition target) failed.");
        if (clientRect.Width == targetMetrics.PhysicalWidth &&
            clientRect.Height == targetMetrics.PhysicalHeight)
            return;
        Interlocked.Increment(ref _exactExtentMismatchCount);
        throw new InvalidOperationException(
            $"Child HWND {clientRect.Width}x{clientRect.Height} no longer matches admitted metrics " +
            $"{targetMetrics.PhysicalWidth}x{targetMetrics.PhysicalHeight}.");
    }

    private FlutterWindowsProvisionalResize? GetProvisionalResize(WindowsViewMetrics targetMetrics)
    {
        lock (_provisionalGate)
        {
            return _provisionalResize is { } state &&
                ReferenceEquals(state.TargetMetrics, targetMetrics)
                ? state
                : null;
        }
    }

    private void CompleteProvisionalResize(FlutterWindowsProvisionalResize state)
    {
        lock (_provisionalGate)
        {
            if (ReferenceEquals(_provisionalResize, state)) _provisionalResize = null;
        }
    }

    private void WaitForGpu()
    {
        if (_submittedFence == 0 || _fence!.CompletedValue >= _submittedFence) return;
        _fence.SetEventOnCompletion(_submittedFence, _fenceEvent!).CheckError();
        var result = WaitForSingleObject(_fenceEvent!.SafeWaitHandle.DangerousGetHandle(), 5000);
        if (result != WaitObject0)
            throw new TimeoutException($"Windows composition D3D12 fence {_submittedFence} timed out.");
    }

    private void EnsureRasterThread()
    {
        if (Environment.CurrentManagedThreadId == _rasterManagedThreadId &&
            GetCurrentThreadId() == _rasterNativeThreadId)
            return;
        throw new InvalidOperationException(
            "The Windows composition surface is owned by its dedicated raster thread.");
    }

    private void DisposeCapacityResources()
    {
        if (_compositionTarget is not null) _compositionTarget.SetRoot(null).CheckError();
        if (_compositionDevice is not null)
        {
            _compositionDevice.Commit().CheckError();
            _compositionDevice.WaitForCommitCompletion().CheckError();
        }
        _contentVisual?.Dispose();
        _contentVisual = null;
        _rootVisual?.Dispose();
        _rootVisual = null;
        _compositionTarget?.Dispose();
        _compositionTarget = null;
        _compositionDevice?.Dispose();
        _compositionDevice = null;
        _swapChain?.Dispose();
        _swapChain = null;
        _backing?.Dispose();
        _backing = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        EnsureRasterThread();
        _disposed = true;
        lock (_provisionalGate)
        {
            _provisionalResize?.Cancel();
            _provisionalResize = null;
        }
        WaitForGpu();
        DisposeCapacityResources();
        _context?.Dispose();
        _context = null;
        _backend?.Dispose();
        _backend = null;
        _fenceEvent?.Dispose();
        _fenceEvent = null;
        _fence?.Dispose();
        _fence = null;
        _commands?.Dispose();
        _commands = null;
        _allocator?.Dispose();
        _allocator = null;
        _queue?.Dispose();
        _queue = null;
        _device?.Dispose();
        _device = null;
        _adapter?.Dispose();
        _adapter = null;
        _factory?.Dispose();
        _factory = null;
    }

    private void DisposeAfterFailedCreate()
    {
        try
        {
            if (_compositionTarget is not null) _compositionTarget.SetRoot(null).CheckError();
            _compositionDevice?.Commit().CheckError();
        }
        catch { }
        try { _contentVisual?.Dispose(); } catch { }
        try { _rootVisual?.Dispose(); } catch { }
        try { _compositionTarget?.Dispose(); } catch { }
        try { _compositionDevice?.Dispose(); } catch { }
        try { _swapChain?.Dispose(); } catch { }
        try { _backing?.Dispose(); } catch { }
        try { _context?.Dispose(); } catch { }
        try { _backend?.Dispose(); } catch { }
        try { _fenceEvent?.Dispose(); } catch { }
        try { _fence?.Dispose(); } catch { }
        try { _commands?.Dispose(); } catch { }
        try { _allocator?.Dispose(); } catch { }
        try { _queue?.Dispose(); } catch { }
        try { _device?.Dispose(); } catch { }
        try { _adapter?.Dispose(); } catch { }
        try { _factory?.Dispose(); } catch { }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

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

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
        internal readonly int Width => Right - Left;
        internal readonly int Height => Bottom - Top;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint hwnd, out NativeRect rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(nint hwnd);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromWindow(nint window, uint flags);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint handle, uint milliseconds);
}

internal sealed class FlutterWindowsProvisionalResize
{
    private readonly TaskCompletionSource<bool> _prepared = new(
        TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _admitted = new(
        TaskCreationOptions.RunContinuationsAsynchronously);

    internal FlutterWindowsProvisionalResize(WindowsViewMetrics targetMetrics) =>
        TargetMetrics = targetMetrics ?? throw new ArgumentNullException(nameof(targetMetrics));

    internal WindowsViewMetrics TargetMetrics { get; }

    internal bool IsPrepared =>
        _prepared.Task.IsCompletedSuccessfully && _prepared.Task.Result;

    internal bool WaitForPreparation(TimeSpan timeout) =>
        _prepared.Task.Wait(timeout) && _prepared.Task.GetAwaiter().GetResult();

    internal bool WaitForAdmission(TimeSpan timeout) =>
        _admitted.Task.Wait(timeout) && _admitted.Task.GetAwaiter().GetResult();

    internal void MarkPrepared() => _prepared.TrySetResult(true);

    internal void Admit() => _admitted.TrySetResult(true);

    internal void Cancel()
    {
        _prepared.TrySetResult(false);
        _admitted.TrySetResult(false);
    }
}

internal sealed record FlutterWindowsCompositionSurfaceSnapshot(
    nint CompositionHwnd,
    nint ChildHwnd,
    WindowsViewMetrics? TargetMetrics,
    long SurfaceGeneration,
    int CapacityWidth,
    int CapacityHeight,
    long PresentAttemptCount,
    long SuccessfulPresentCount,
    long CapacityGrowthCount,
    long ExactExtentMismatchCount,
    string AdapterDescription,
    bool SoftwareFallback,
    int RasterManagedThreadId,
    uint RasterNativeThreadId,
    bool Disposed);

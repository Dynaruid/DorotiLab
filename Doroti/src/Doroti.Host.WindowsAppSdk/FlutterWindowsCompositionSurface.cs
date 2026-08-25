using System.ComponentModel;
using System.Diagnostics;
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
/// starts at the exact client extent plus a bounded transient reservoir; it is
/// not sized from the monitor or virtual desktop.
/// </summary>
internal sealed class FlutterWindowsCompositionSurface : IFlutterWindowsScheduledSurface
{
    private const int TransientReservePixels = 768;
    private const int CapacityQuantumPixels = 256;
    private const uint WaitObject0 = 0;
    private readonly nint _compositionHwnd;
    private readonly nint _childHwnd;
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
    private long _transientPresentCount;
    private long _capacityGrowthCount;
    private long _exactExtentMismatchCount;
    private long _gpuFenceWaitCount;
    private long _gpuFenceWaitTotalMicroseconds;
    private long _presentCommitCount;
    private long _presentCommitTotalMicroseconds;
    private bool _firstCommitCompleted;
    private int _committedWidth;
    private int _committedHeight;
    private int _transientWidth;
    private int _transientHeight;
    private bool _recoveryPending;
    private bool _disposed;

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
        Interlocked.Read(ref _transientPresentCount),
        Interlocked.Read(ref _capacityGrowthCount),
        Interlocked.Read(ref _exactExtentMismatchCount),
        Interlocked.Read(ref _gpuFenceWaitCount),
        Interlocked.Read(ref _gpuFenceWaitTotalMicroseconds),
        Interlocked.Read(ref _presentCommitCount),
        Interlocked.Read(ref _presentCommitTotalMicroseconds),
        AdapterDescription,
        SoftwareFallback,
        _rasterManagedThreadId,
        _rasterNativeThreadId,
        _disposed);

    internal string AdapterDescription { get; private set; } = "uninitialized";

    internal bool SoftwareFallback =>
        AdapterDescription.Contains("software", StringComparison.OrdinalIgnoreCase) ||
        AdapterDescription.Contains("basic render driver", StringComparison.OrdinalIgnoreCase);

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

        ValidateExactChildTarget(targetMetrics);
        var grew = EnsureCapacity(targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
        _targetMetrics = targetMetrics;
        return new(false, grew, false, _surfaceGeneration,
            targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
    }

    public FlutterWindowsAngleEglPresentResult RenderAndSwap(
        WindowsViewMetrics targetMetrics,
        Action<SKSurface> paint,
        Action? beforeSwap = null)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        ArgumentNullException.ThrowIfNull(paint);
        EnsureRasterThread();
        ThrowIfDisposed();
        if (_targetMetrics != targetMetrics || !targetMetrics.HasDrawableSize)
            throw new InvalidOperationException(
                "The composition front can present only its exact admitted WindowsViewMetrics target.");
        ValidateExactChildTarget(targetMetrics);
        if (_recoveryPending)
        {
            _recoveryPending = false;
            RecreateGraphics(targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
            _targetMetrics = targetMetrics;
        }

        WaitForGpu();
        var surface = _backing?.Surface ?? throw new ObjectDisposedException(nameof(FlutterWindowsCompositionSurface));
        var canvas = surface.Canvas;
        // Clear only the exact client viewport. A later expansion has a larger
        // exact clip and therefore clears every newly exposed pixel before the
        // scene is painted.
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

        beforeSwap?.Invoke();
        CopyAndPresent(targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
        _committedWidth = targetMetrics.PhysicalWidth;
        _committedHeight = targetMetrics.PhysicalHeight;
        _transientWidth = _committedWidth;
        _transientHeight = _committedHeight;
        Interlocked.Increment(ref _successfulPresentCount);
        return new(
            targetMetrics.PhysicalWidth,
            targetMetrics.PhysicalHeight,
            _surfaceGeneration,
            RecoveredFromContextLoss: false,
            SuccessfulSwap: true);
    }

    /// <summary>
    /// Presents the last exact scene at 1:1 scale while native geometry is
    /// ahead of framework layout. Newly exposed pixels repeat a known-safe
    /// background row/column; the scene body is never scaled in X or Y.
    /// This is a transient compositor front and does not change exact metrics.
    /// </summary>
    internal bool PresentTransient(WindowsViewMetrics targetMetrics)
    {
        ArgumentNullException.ThrowIfNull(targetMetrics);
        if (!targetMetrics.HasDrawableSize) return false;
        ValidateExactChildTarget(targetMetrics);
        return PresentTransient(targetMetrics.PhysicalWidth, targetMetrics.PhysicalHeight);
    }

    /// <summary>
    /// R1 native-control variant only: remove the compositor root before the
    /// measured sizing episode so USER32/DWM geometry can be timed without a
    /// visible product surface. Resources remain owned by the raster thread
    /// and are released normally at shutdown.
    /// </summary>
    internal void DetachVisibleRootForDiagnostic()
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        _compositionTarget!.SetRoot(null).CheckError();
        _compositionDevice!.Commit().CheckError();
        _compositionDevice.WaitForCommitCompletion().CheckError();
    }

    /// <summary>
    /// Prepares a proposed native extent before Windows exposes it. The child
    /// HWND still clips the larger front until geometry advances, so this does
    /// not admit, rewrite, or wait on native geometry.
    /// </summary>
    internal bool PresentTransient(int physicalWidth, int physicalHeight)
    {
        EnsureRasterThread();
        ThrowIfDisposed();
        if (physicalWidth <= 0 || physicalHeight <= 0 ||
            _committedWidth <= 0 || _committedHeight <= 0)
            return false;
        const int transientRefillThresholdPixels = 128;
        var expandsWidth = physicalWidth > _committedWidth;
        var expandsHeight = physicalHeight > _committedHeight;
        if (!expandsWidth && !expandsHeight)
            return false;
        if ((!expandsWidth || _transientWidth - physicalWidth >= transientRefillThresholdPixels) &&
            (!expandsHeight || _transientHeight - physicalHeight >= transientRefillThresholdPixels))
            return false;
        if (expandsWidth) physicalWidth = checked(physicalWidth + TransientReservePixels);
        if (expandsHeight) physicalHeight = checked(physicalHeight + TransientReservePixels);
        _ = EnsureCapacity(physicalWidth, physicalHeight);
        if (expandsWidth) physicalWidth = Math.Min(_capacityWidth, RoundUp(physicalWidth, CapacityQuantumPixels));
        if (expandsHeight) physicalHeight = Math.Min(_capacityHeight, RoundUp(physicalHeight, CapacityQuantumPixels));
        if (_transientWidth >= physicalWidth && _transientHeight >= physicalHeight)
            return false;

        WaitForGpu();
        var surface = _backing?.Surface ?? throw new ObjectDisposedException(nameof(FlutterWindowsCompositionSurface));
        using var committed = surface.Snapshot(new SKRectI(0, 0, _committedWidth, _committedHeight));
        var canvas = surface.Canvas;
        var oldWidth = Math.Min(_committedWidth, physicalWidth);
        var oldHeight = Math.Min(_committedHeight, physicalHeight);
        var safeBackgroundX = Math.Min(8, _committedWidth - 1);
        var safeBackgroundY = Math.Max(0, _committedHeight - 9);
        if (physicalWidth > _committedWidth && oldHeight > 0)
        {
            canvas.DrawImage(
                committed,
                new SKRect(safeBackgroundX, 0, safeBackgroundX + 1, oldHeight),
                new SKRect(_committedWidth, 0, physicalWidth, oldHeight),
                new SKSamplingOptions(SKFilterMode.Nearest));
        }
        if (physicalHeight > _committedHeight && oldWidth > 0)
        {
            canvas.DrawImage(
                committed,
                new SKRect(0, safeBackgroundY, oldWidth, safeBackgroundY + 1),
                new SKRect(0, _committedHeight, oldWidth, physicalHeight),
                new SKSamplingOptions(SKFilterMode.Nearest));
        }
        if (physicalWidth > _committedWidth && physicalHeight > _committedHeight)
        {
            canvas.DrawImage(
                committed,
                new SKRect(safeBackgroundX, safeBackgroundY,
                    safeBackgroundX + 1, safeBackgroundY + 1),
                new SKRect(_committedWidth, _committedHeight,
                    physicalWidth, physicalHeight),
                new SKSamplingOptions(SKFilterMode.Nearest));
        }
        canvas.Flush();
        _context!.Flush(surface);
        _context.Submit(false);
        CopyAndPresent(physicalWidth, physicalHeight);
        _transientWidth = physicalWidth;
        _transientHeight = physicalHeight;
        Interlocked.Increment(ref _transientPresentCount);
        return true;
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

        _capacityWidth = RoundUp(
            checked(Math.Max(1, initialWidth) + TransientReservePixels),
            CapacityQuantumPixels);
        _capacityHeight = RoundUp(
            checked(Math.Max(1, initialHeight) + TransientReservePixels),
            CapacityQuantumPixels);
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
        // The child HWND clips the bounded-reservoir DirectComposition target. The
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

    private void WaitForGpu()
    {
        if (_submittedFence == 0 || _fence!.CompletedValue >= _submittedFence) return;
        var started = Stopwatch.GetTimestamp();
        _fence.SetEventOnCompletion(_submittedFence, _fenceEvent!).CheckError();
        var result = WaitForSingleObject(_fenceEvent!.SafeWaitHandle.DangerousGetHandle(), 5000);
        var elapsed = Stopwatch.GetElapsedTime(started);
        Interlocked.Increment(ref _gpuFenceWaitCount);
        Interlocked.Add(ref _gpuFenceWaitTotalMicroseconds, Math.Max(0L, elapsed.Ticks / 10L));
        if (result != WaitObject0)
            throw new TimeoutException($"Windows composition D3D12 fence {_submittedFence} timed out.");
    }

    private void CopyAndPresent(int width, int height)
    {
        var started = Stopwatch.GetTimestamp();
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
            _commands.CopyTextureRegion(
                new TextureCopyLocation(buffer, 0),
                0,
                0,
                0,
                new TextureCopyLocation(_backing.Resource, 0),
                new Box(0, 0, 0, width, height, 1));
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
        var elapsed = Stopwatch.GetElapsedTime(started);
        Interlocked.Increment(ref _presentCommitCount);
        Interlocked.Add(ref _presentCommitTotalMicroseconds, Math.Max(0L, elapsed.Ticks / 10L));
        if (!_firstCommitCompleted)
        {
            _compositionDevice.WaitForCommitCompletion().CheckError();
            _firstCommitCompleted = true;
        }
    }

    private static int RoundUp(int value, int quantum) =>
        checked(((value + quantum - 1) / quantum) * quantum);

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

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(nint handle, uint milliseconds);
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
    long TransientPresentCount,
    long CapacityGrowthCount,
    long ExactExtentMismatchCount,
    long GpuFenceWaitCount,
    long GpuFenceWaitTotalMicroseconds,
    long PresentCommitCount,
    long PresentCommitTotalMicroseconds,
    string AdapterDescription,
    bool SoftwareFallback,
    int RasterManagedThreadId,
    uint RasterNativeThreadId,
    bool Disposed);

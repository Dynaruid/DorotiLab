using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Rect = Doroti.Ui.Rect;
using Size = Doroti.Ui.Size;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Product composition root for the Flutter-style Windows App SDK adapter.
/// The component classes remain independently testable, while this adapter is
/// the only place where their platform, framework, raster, input, UIA, and
/// lifecycle ownership is joined for a real Doroti view.
/// </summary>
internal sealed class FlutterWindowsHostAdapter :
    IWindowsAppSdkProductHost,
    IViewHostCapability,
    IFrameHostCapability,
    IExactFrameHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    ITextInputHostCapability,
    IPlatformEnvironmentHostCapability,
    IPlatformServicesHostCapability,
    ISkiaSceneRendererHost,
    IFlutterWindowsEngineTaskRunner
{
    private const uint WmClose = 0x0010;
    private const uint WmAppFrame = 0x8001;
    private const uint WmAppFirstFrame = 0x8002;
    private const uint WmAppEngineTask = 0x8003;
    private const uint WmAppSmokeResize = 0x8004;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpNoMove = 0x0002;
    private const uint WsOverlappedWindow = 0x00cf0000;
    private const uint WsClipChildren = 0x02000000;
    private const uint WsExAppWindow = 0x00040000;

    private readonly ulong _viewId;
    private readonly DorotiViewConfiguration _configuration;
    private readonly FlutterWindowsAngleNativeProvenance _nativeProvenance;
    private readonly FlutterWindowsAppSdkBootstrap _bootstrap = new();
    private readonly FlutterWindowsHostWindow _host;
    private readonly FlutterWindowsViewMetricsCoordinator _metrics;
    private readonly FlutterWindowsDwmVsyncSource _vsync;
    private readonly FlutterWindowsFrameScheduler _scheduler;
    private readonly FlutterWindowsDedicatedRasterTaskRunner _rasterRunner;
    private readonly FlutterWindowsAngleEglSharedContext _sharedContext;
    private readonly FlutterWindowsAngleEglWindowSurface _surface;
    private readonly FlutterWindowsInputHost _input;
    private readonly FlutterWindowsUiaBridge _uia;
    private readonly FlutterWindowsLifecycleManager _lifecycle;
    private readonly ConcurrentQueue<Action> _engineTasks = new();
    private readonly ConcurrentDictionary<long, ProductResizeWait> _resizeWaitsByGeneration = new();
    private readonly ConcurrentDictionary<long, ProductResizeWait> _resizeWaitsByCausalFrame = new();
    private readonly AutoResetEvent _frameSignal = new(false);
    private readonly Thread _frameThread;
    private readonly object _gate = new();
    private SkiaSceneRenderer? _renderer;
    private FlutterWindowsScheduledRaster? _scheduledRaster;
    private long _inputSequence;
    private long _surfaceGeneration;
    private long _pendingResizeGeneration;
    private long _frameRequests;
    private long _frameDispatches;
    private long _firstFramePosts;
    private long _engineTasksPosted;
    private long _engineTasksRun;
    private long _resizeWaitDone;
    private long _resizeWaitTimedOut;
    private long _resizeWaitSuperseded;
    private long _resizeDwmFlush;
    private int _firstFrameNotified;
    private int _smokeResizePosted;
    private int _smokeWidth;
    private int _smokeHeight;
    private int _smokeDpi;
    private long _smokeVersion;
    private bool _shown;
    private bool _closing;
    private bool _disposed;
    private Action<int, SemanticsAction, object?>? _semanticsAction;

    internal FlutterWindowsHostAdapter(ulong viewId, DorotiViewConfiguration configuration)
    {
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        _viewId = viewId;
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _nativeProvenance = FlutterWindowsAngleEglContext.EnsureNativeArtifactsForWindowSurface();

        var initialWidth = Math.Max(320, checked((int)Math.Round(configuration.logicalSize.width)));
        var initialHeight = Math.Max(240, checked((int)Math.Round(configuration.logicalSize.height)));
        _rasterRunner = new("Doroti WindowsAppSdk FlutterEmbedder raster");
        _host = FlutterWindowsHostWindow.CreateOnCurrentThread(
            _bootstrap,
            new FlutterWindowsHostWindowOptions(
                configuration.title,
                initialWidth,
                initialHeight,
                MinimumClientWidth: 320,
                MinimumClientHeight: 240,
                MaximumClientWidth: 16_384,
                MaximumClientHeight: 16_384,
                InitialX: 100,
                InitialY: 100),
            new FlutterWindowsHostWindowTeardown
            {
                DisposeViewSurface = DisposeRasterResources,
            });
        _metrics = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
            _host,
            viewId,
            new FlutterWindowsPhysicalConstraints(1, 1, 16_384, 16_384));
        _vsync = new FlutterWindowsDwmVsyncSource(_host.ViewHwnd);
        _scheduler = new FlutterWindowsFrameScheduler(_metrics, _vsync);
        _sharedContext = RunRaster(FlutterWindowsAngleEglSharedContext.CreateOnCurrentRasterThread);
        _surface = RunRaster(() => FlutterWindowsAngleEglWindowSurface.CreateOnCurrentRasterThread(
            _sharedContext,
            _host.ViewHwnd,
            _metrics.Current));
        Volatile.Write(ref _surfaceGeneration, _surface.Snapshot.SurfaceGeneration);

        _input = new FlutterWindowsInputHost(_host, viewId, () => _metrics.Current);
        _uia = FlutterWindowsUiaBridge.AttachToHostWindow(
            _host,
            _metrics,
            this,
            (nodeId, action, arguments) => _semanticsAction?.Invoke(nodeId, action, arguments),
            () => _input.RequestFocus(ViewFocusState.focused, ViewFocusDirection.undefined));
        _lifecycle = new FlutterWindowsLifecycleManager(
            _host,
            RequestInvalidate,
            RequestGraphicsRecovery,
            TerminalizePendingProductWork,
            _scheduler);

        _host.TopLevelMessageReceived += HandleTopLevelMessage;
        _metrics.MetricsPublished += HandleMetricsPublished;
        _scheduler.LatestMetricsFrameRequested += HandleLatestMetricsFrameRequested;
        _input.PointerData += HandlePointerData;
        _input.KeyData += HandleKeyData;
        _input.FocusData += HandleFocusData;
        _input.EditingStateChanged += HandleEditingStateChanged;
        _input.ActionPerformed += HandleActionPerformed;

        _frameThread = new(FrameLoop)
        {
            IsBackground = true,
            Name = "Doroti WindowsAppSdk FlutterEmbedder DWM frame clock",
        };
        _frameThread.SetApartmentState(ApartmentState.MTA);
        _frameThread.Start();
        _lifecycle.EnsureVisibleInCurrentWorkArea();
    }

    public ViewMetrics Metrics => _metrics.Current.ToViewMetrics(SurfaceGeneration);
    public DorotiViewEpoch ViewEpoch => _metrics.Current.ToViewEpoch();
    public long InputSequence => Volatile.Read(ref _inputSequence);
    public long SurfaceGeneration => Math.Max(1, Volatile.Read(ref _surfaceGeneration));
    public DorotiResizeEpoch ResizeTarget => _metrics.Current.ToResizeEpoch();
    public PlatformConfiguration Configuration => new(
        [ToLocale(CultureInfo.CurrentUICulture)],
        Brightness.light,
        false,
        false,
        HostOperatingSystem.windows);

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;
    public event Action<DorotiTextEditingState>? EditingStateChanged;
    public event Action<DorotiTextInputAction>? ActionPerformed;
    public event Action<PlatformConfiguration>? ConfigurationChanged;
    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add => _semanticsAction += value;
        remove => _semanticsAction -= value;
    }
    public event Action<long, TimeSpan>? InputReceived;

    internal nint TopLevelHwnd => _host.TopLevelHwnd;
    internal nint ViewHwnd => _host.ViewHwnd;

    public void AttachRenderer(SkiaSceneRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate)
        {
            if (_renderer is not null)
                throw new InvalidOperationException("The FlutterEmbedder renderer is already attached.");
            _renderer = renderer;
            _scheduledRaster = new FlutterWindowsScheduledRaster(
                _scheduler,
                _surface,
                renderer,
                _rasterRunner);
            _scheduledRaster.CausalTraceCompleted += HandleCausalTraceCompleted;
        }
        renderer.AttachSurface(RequestInvalidate);
        RequestInvalidate();
    }

    public int RunMessageLoop()
    {
        NativeMessage message;
        while (NativeMethods.GetMessageW(out message, 0, 0, 0) > 0)
        {
            NativeMethods.TranslateMessage(in message);
            NativeMethods.DispatchMessageW(in message);
        }
        return checked((int)message.WParam);
    }

    public void WriteDiagnostics(SkiaFrameDiagnostics diagnostics)
    {
        var scheduler = _scheduler.Snapshot;
        var surface = _surface.Snapshot;
        var lifecycle = _lifecycle.Snapshot;
        Console.Error.WriteLine(
            "doroti.windowsappsdk.summary=" +
            $"adapter=FlutterEmbedder;topLevel=0x{TopLevelHwnd:x};child=0x{ViewHwnd:x};" +
            $"surfaceGeneration={surface.SurfaceGeneration};swaps={surface.SuccessfulSwapCount};" +
            $"renderer={surface.Renderer};softwareFallback={surface.SoftwareFallback};" +
            $"queueDepth={scheduler.QueueDepth};queueMax={scheduler.MaxObservedQueueDepth};" +
            $"stalePresent={scheduler.StaleOrWrongSizePresentCount};presented={scheduler.PresentedFrameCount};" +
            $"frameRequests={Volatile.Read(ref _frameRequests)};frameDispatches={Volatile.Read(ref _frameDispatches)};" +
            $"resizeDone={Volatile.Read(ref _resizeWaitDone)};resizeTimedOut={Volatile.Read(ref _resizeWaitTimedOut)};" +
            $"resizeSuperseded={Volatile.Read(ref _resizeWaitSuperseded)};" +
            $"resizeDwmFlush={Volatile.Read(ref _resizeDwmFlush)};" +
            $"engineTasks={Volatile.Read(ref _engineTasksRun)}/{Volatile.Read(ref _engineTasksPosted)};" +
            $"dpiChanges={lifecycle.DpiChangedCount};displayChanges={lifecycle.DisplayChangedCount};" +
            $"angle={_nativeProvenance.AnglePackageVersion}:{_nativeProvenance.AngleSha256};" +
            $"skia={_nativeProvenance.SkiaPackageVersion}:{_nativeProvenance.SkiaSha256};" +
            $"pathFallback={_nativeProvenance.PathFallbackUsed};submitted={diagnostics.Submitted};" +
            $"scenePresented={diagnostics.Presented};failed={diagnostics.Failed};superseded={diagnostics.Superseded}");
    }

    public void ApplyLeftResizeSmokeStep(int step)
    {
        if (_disposed) return;
        var cycle = Math.Abs(step % 48);
        var delta = (cycle <= 24 ? cycle : 48 - cycle) * 4;
        var current = _metrics.Current;
        var width = Math.Max(320, current.PhysicalWidth - delta);
        Volatile.Write(ref _smokeWidth, width);
        Volatile.Write(ref _smokeHeight, Math.Max(240, current.PhysicalHeight));
        Volatile.Write(ref _smokeDpi, checked((int)Math.Round(current.DevicePixelRatio * 96.0)));
        Interlocked.Increment(ref _smokeVersion);
        if (Interlocked.Exchange(ref _smokeResizePosted, 1) == 0)
            _ = NativeMethods.PostMessageW(TopLevelHwnd, WmAppSmokeResize, 0, 0);
    }

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_shown) return;
        _scheduler.SetHidden(false);
        LifecycleChanged?.Invoke(AppLifecycleState.resumed);
        ConfigurationChanged?.Invoke(Configuration);
        RequestInvalidate();
        PumpUntilFirstFrame();
        _shown = true;
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        var dpi = NativeMethods.GetDpiForWindow(TopLevelHwnd);
        if (dpi == 0) dpi = 96;
        var scale = dpi / 96.0;
        SetClientSize(
            checked((int)Math.Round(logicalSize.width * scale)),
            checked((int)Math.Round(logicalSize.height * scale)),
            dpi);
    }

    public void Close()
    {
        if (!_disposed) _ = NativeMethods.PostMessageW(TopLevelHwnd, WmClose, 0, 0);
    }

    public void ScheduleFrame(Action<TimeSpan> callback) => ScheduleFrame(ViewEpoch, callback);

    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(expectedEpoch);
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        Interlocked.Increment(ref _frameRequests);
        var metrics = _metrics.Current;
        FlutterWindowsFrameScheduleResult result;
        FlutterWindowsScheduledFrameCallback dispatch = (ticket, vsync) =>
        {
            try
            {
                callback(vsync.Timestamp);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"doroti.windowsappsdk.flutter.framework-frame.fail={exception}");
                throw;
            }
            QueueRaster(ticket, vsync);
        };
        if (Volatile.Read(ref _pendingResizeGeneration) == metrics.ResizeGeneration &&
            expectedEpoch.ResizeTargetGeneration == metrics.ResizeGeneration)
        {
            result = _scheduler.ScheduleResize(metrics, dispatch);
            if (result.Accepted)
            {
                Interlocked.CompareExchange(ref _pendingResizeGeneration, 0, metrics.ResizeGeneration);
                BindResizeWait(result.Ticket, metrics.ResizeGeneration);
            }
        }
        else
        {
            result = _scheduler.ScheduleOrdinary(expectedEpoch, dispatch);
        }
        if (result.Accepted) _frameSignal.Set();
    }

    public void RequestInvalidate()
    {
        if (_disposed || _renderer is null || _scheduledRaster is null) return;
        var scheduler = _scheduler.Snapshot;
        if (scheduler.HasPendingResize || scheduler.HasPendingOrdinary ||
            scheduler.HasResizeInFlight || scheduler.HasOrdinaryInFlight)
        {
            _frameSignal.Set();
            return;
        }
        var metrics = _metrics.Current;
        var pendingResize = Volatile.Read(ref _pendingResizeGeneration) == metrics.ResizeGeneration;
        var result = pendingResize
            ? _scheduler.ScheduleResize(metrics, QueueRaster)
            : _scheduler.ScheduleOrdinary(metrics.ToViewEpoch(), QueueRaster);
        if (result.Accepted)
        {
            if (pendingResize)
            {
                Interlocked.CompareExchange(ref _pendingResizeGeneration, 0, metrics.ResizeGeneration);
                BindResizeWait(result.Ticket, metrics.ResizeGeneration);
            }
            _frameSignal.Set();
        }
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction) =>
        _input.RequestFocus(state, direction);
    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState) =>
        _input.SetClient(configuration, initialState);
    public void UpdateState(DorotiTextEditingState state) => _input.UpdateState(state);
    public void SetCaretRect(Rect logicalRect) => _input.SetCaretRect(logicalRect);
    public void ClearClient() => _input.ClearClient();
    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default) =>
        _input.GetClipboardTextAsync(cancellationToken);
    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default) =>
        _input.SetClipboardTextAsync(text, cancellationToken);
    public void SetCursor(DorotiMouseCursorKind cursor) => _input.SetCursor(cursor);
    public void UpdateSemantics(SemanticsUpdate update) => _ = _uia.UpdateSemantics(update);
    public void ClearSemantics() => _uia.ClearSemantics();

    public void PostEngineTask(Action task)
    {
        ArgumentNullException.ThrowIfNull(task);
        ObjectDisposedException.ThrowIf(_disposed, this);
        _engineTasks.Enqueue(task);
        Interlocked.Increment(ref _engineTasksPosted);
        _ = NativeMethods.PostMessageW(TopLevelHwnd, WmAppEngineTask, 0, 0);
    }

    public bool TryRunOneTask()
    {
        if (!_engineTasks.TryDequeue(out var task)) return false;
        task();
        Interlocked.Increment(ref _engineTasksRun);
        return true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _closing = true;
        _frameSignal.Set();
        if (Thread.CurrentThread != _frameThread && !_frameThread.Join(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("FlutterEmbedder frame clock did not stop.");
        _host.TopLevelMessageReceived -= HandleTopLevelMessage;
        _metrics.MetricsPublished -= HandleMetricsPublished;
        _scheduler.LatestMetricsFrameRequested -= HandleLatestMetricsFrameRequested;
        _input.PointerData -= HandlePointerData;
        _input.KeyData -= HandleKeyData;
        _input.FocusData -= HandleFocusData;
        _input.EditingStateChanged -= HandleEditingStateChanged;
        _input.ActionPerformed -= HandleActionPerformed;
        _lifecycle.BeginShutdown();
        _uia.Dispose();
        _input.Dispose();
        _lifecycle.Dispose();
        _scheduler.Dispose();
        _metrics.Dispose();
        _host.Dispose();
        _rasterRunner.Dispose();
        _frameSignal.Dispose();
        Closed?.Invoke();
        Closed = null;
    }

    private void QueueRaster(FlutterWindowsFrameTicket ticket, FlutterWindowsVsyncSample vsync)
    {
        FlutterWindowsScheduledRaster? raster;
        lock (_gate) raster = _scheduledRaster;
        if (raster is null)
        {
            _scheduler.ReportFrameFailure(ticket, "FlutterEmbedder renderer was not attached.");
            return;
        }
        Interlocked.Increment(ref _frameDispatches);
        raster.QueueRender(ticket, vsync);
    }

    private void HandleCausalTraceCompleted(FlutterWindowsScheduledRasterCausalTrace trace)
    {
        ProductResizeWait? resizeWait = null;
        if (_resizeWaitsByCausalFrame.TryRemove(trace.CausalFrameId, out var matchedResize))
        {
            resizeWait = matchedResize;
            resizeWait.PresentedSuccessfully = trace.Presented;
            resizeWait.Presented.Set();
        }
        if (trace.Presented)
        {
            // This callback is raised by the dedicated raster owner. Reading
            // its immutable snapshot directly avoids posting back to and
            // synchronously waiting on the same single-thread task runner.
            Volatile.Write(ref _surfaceGeneration, _surface.Snapshot.SurfaceGeneration);
            if (Interlocked.CompareExchange(ref _firstFrameNotified, 1, 0) == 0)
            {
                Interlocked.Increment(ref _firstFramePosts);
                _ = NativeMethods.PostMessageW(TopLevelHwnd, WmAppFirstFrame, 0, 0);
            }
        }
        else if (!_host.Snapshot.FirstFrameSwapped)
        {
            var raster = _scheduledRaster?.Snapshot;
            var renderer = _renderer?.Diagnostics;
            var recentTrace = renderer is null
                ? string.Empty
                : string.Join(",", renderer.Trace.TakeLast(8).Select(entry => $"{entry.Phase}:{entry.Reason}"));
            Console.Error.WriteLine(
                $"doroti.windowsappsdk.flutter.first-frame.pending=" +
                $"causal={trace.CausalFrameId};exact={trace.ExactMetrics};" +
                $"rasterFailures={raster?.FailureCount};rejected={raster?.RejectedRasterCount};" +
                $"rendererPending={renderer?.PendingScene};trace={recentTrace}");
        }
        if (resizeWait is not null)
        {
            if (resizeWait.PlatformUnblocked.Wait(TimeSpan.FromMilliseconds(100)) && trace.Presented)
            {
                var result = NativeMethods.DwmFlush();
                if (result >= 0) Interlocked.Increment(ref _resizeDwmFlush);
            }
            _resizeWaitsByGeneration.TryRemove(resizeWait.ResizeGeneration, out _);
            resizeWait.Dispose();
        }
        _frameSignal.Set();
    }

    private FlutterWindowsChildMessageResult HandleTopLevelMessage(FlutterWindowsTopLevelMessage message)
    {
        switch (message.Message)
        {
            case WmAppFrame:
                DrainEngineTasks();
                var frame = _scheduler.TryRunOneFrame();
                if (frame.Disposition == FlutterWindowsFrameRunDisposition.CallbackFailed)
                    Console.Error.WriteLine("doroti.windowsappsdk.flutter.frame-callback=failed");
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmAppFirstFrame:
                if (!_host.Snapshot.FirstFrameSwapped) _host.NotifyFirstFrameSwapped();
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmAppEngineTask:
                DrainEngineTasks();
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmAppSmokeResize:
                HandleSmokeResize();
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmClose:
                if (!_closing)
                {
                    _closing = true;
                    CloseRequested?.Invoke();
                    _lifecycle.BeginShutdown();
                    NativeMethods.PostQuitMessage(0);
                }
                return FlutterWindowsChildMessageResult.HandledResult();
            default:
                return FlutterWindowsChildMessageResult.Unhandled;
        }
    }

    private void HandleMetricsPublished(WindowsViewMetrics metrics)
    {
        Volatile.Write(ref _pendingResizeGeneration, metrics.ResizeGeneration);
        if (_renderer is null || _scheduledRaster is null || !metrics.HasDrawableSize)
        {
            MetricsChanged?.Invoke(metrics.ToViewMetrics(SurfaceGeneration));
            return;
        }

        var resizeWait = new ProductResizeWait(metrics.ResizeGeneration);
        if (!_resizeWaitsByGeneration.TryAdd(metrics.ResizeGeneration, resizeWait))
            throw new InvalidOperationException("A product resize generation was registered more than once.");
        try
        {
            MetricsChanged?.Invoke(metrics.ToViewMetrics(SurfaceGeneration));
            if (!resizeWait.CausalBound)
            {
                Interlocked.Increment(ref _resizeWaitSuperseded);
                _resizeWaitsByGeneration.TryRemove(metrics.ResizeGeneration, out _);
            }
            else
            {
                _ = _scheduler.TryRunOneFrame();
                var started = Stopwatch.GetTimestamp();
                while (!resizeWait.Presented.IsSet && Stopwatch.GetElapsedTime(started) < TimeSpan.FromMilliseconds(100))
                {
                    if (!TryRunOneTask()) Thread.Yield();
                }
                if (resizeWait.PresentedSuccessfully)
                    Interlocked.Increment(ref _resizeWaitDone);
                else
                    Interlocked.Increment(ref _resizeWaitTimedOut);
            }
        }
        finally
        {
            resizeWait.PlatformUnblocked.Set();
        }
        if (!resizeWait.CausalBound) resizeWait.Dispose();
    }

    private void HandleLatestMetricsFrameRequested(WindowsViewMetrics metrics)
    {
        Volatile.Write(ref _pendingResizeGeneration, metrics.ResizeGeneration);
        MetricsChanged?.Invoke(metrics.ToViewMetrics(SurfaceGeneration));
        RequestInvalidate();
    }

    private void HandlePointerData(PointerDataPacket packet)
    {
        RaiseInputReceipt();
        PointerData?.Invoke(packet);
    }

    private void HandleKeyData(KeyData data)
    {
        RaiseInputReceipt();
        KeyData?.Invoke(data);
    }

    private void HandleFocusData(RawFocusData data)
    {
        RaiseInputReceipt();
        FocusData?.Invoke(data);
    }

    private void HandleEditingStateChanged(DorotiTextEditingState state)
    {
        RaiseInputReceipt();
        EditingStateChanged?.Invoke(state);
    }

    private void HandleActionPerformed(DorotiTextInputAction action)
    {
        RaiseInputReceipt();
        ActionPerformed?.Invoke(action);
    }

    private void RaiseInputReceipt()
    {
        var sequence = Interlocked.Increment(ref _inputSequence);
        InputReceived?.Invoke(sequence, DorotiFrameClock.Now);
    }

    private void RequestGraphicsRecovery()
    {
        if (_disposed) return;
        _rasterRunner.Post(() =>
        {
            _surface.RequestLifecycleRecovery();
            RequestInvalidate();
        });
    }

    private void FrameLoop()
    {
        try
        {
            while (true)
            {
                _frameSignal.WaitOne();
                if (_disposed) return;
                // A hidden first-frame-gated top-level may not yet participate
                // in DWM composition. Do not wait on DwmFlush until one exact
                // swap has made the window eligible for presentation.
                if (_host.Snapshot.FirstFrameSwapped)
                {
                    var result = NativeMethods.DwmFlush();
                    if (result < 0) Marshal.ThrowExceptionForHR(result);
                }
                _ = NativeMethods.PostMessageW(TopLevelHwnd, WmAppFrame, 0, 0);
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"doroti.windowsappsdk.flutter.frame.fatal={exception}");
            _ = NativeMethods.PostMessageW(TopLevelHwnd, WmClose, 0, 0);
        }
    }

    private void PumpUntilFirstFrame()
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        while (!_host.Snapshot.FirstFrameSwapped)
        {
            // The first frame is intentionally produced before the top-level
            // joins visible DWM cadence. Drive one bounded scheduler turn from
            // the owning platform loop instead of depending on a hidden-window
            // composition wake-up.
            _ = _scheduler.TryRunOneFrame();
            while (NativeMethods.PeekMessageW(out var message, 0, 0, 0, 1))
            {
                if (message.Message == 0x0012)
                    throw new InvalidOperationException("FlutterEmbedder quit before its first exact swap.");
                NativeMethods.TranslateMessage(in message);
                NativeMethods.DispatchMessageW(in message);
            }
            DrainEngineTasks();
            if (DateTime.UtcNow >= deadline)
            {
                var scheduler = _scheduler.Snapshot;
                var raster = _scheduledRaster?.Snapshot;
                throw new TimeoutException(
                    "FlutterEmbedder did not present its first exact frame within 10 seconds. " +
                    $"queue={scheduler.QueueDepth};pending={scheduler.HasPendingResize}/{scheduler.HasPendingOrdinary};" +
                    $"active={scheduler.HasResizeInFlight}/{scheduler.HasOrdinaryInFlight};" +
                    $"callbacks={scheduler.CallbackCount};raster={raster?.RasterCount};" +
                    $"rejected={raster?.RejectedRasterCount};failures={raster?.FailureCount}.");
            }
            Thread.Sleep(1);
        }
    }

    private void DrainEngineTasks()
    {
        while (TryRunOneTask()) { }
    }

    private void HandleSmokeResize()
    {
        var version = Volatile.Read(ref _smokeVersion);
        SetClientSize(
            Volatile.Read(ref _smokeWidth),
            Volatile.Read(ref _smokeHeight),
            checked((uint)Volatile.Read(ref _smokeDpi)));
        Interlocked.Exchange(ref _smokeResizePosted, 0);
        if (version != Volatile.Read(ref _smokeVersion) &&
            Interlocked.Exchange(ref _smokeResizePosted, 1) == 0)
            _ = NativeMethods.PostMessageW(TopLevelHwnd, WmAppSmokeResize, 0, 0);
    }

    private void BindResizeWait(FlutterWindowsFrameTicket ticket, long resizeGeneration)
    {
        if (!_resizeWaitsByGeneration.TryGetValue(resizeGeneration, out var resizeWait)) return;
        if (!_resizeWaitsByCausalFrame.TryAdd(ticket.CausalFrameId, resizeWait))
            throw new InvalidOperationException("A causal frame was bound to more than one product resize wait.");
        resizeWait.CausalBound = true;
    }

    private void TerminalizePendingProductWork()
    {
        _scheduler.SetSuspended(true);
        foreach (var resizeWait in _resizeWaitsByGeneration.Values.Distinct())
        {
            resizeWait.PresentedSuccessfully = false;
            resizeWait.Presented.Set();
            resizeWait.PlatformUnblocked.Set();
        }
    }

    private void SetClientSize(int width, int height, uint dpi)
    {
        width = Math.Clamp(width, 320, 16_384);
        height = Math.Clamp(height, 240, 16_384);
        var rect = new NativeRect(0, 0, width, height);
        if (!NativeMethods.AdjustWindowRectExForDpi(
                ref rect,
                WsOverlappedWindow | WsClipChildren,
                false,
                WsExAppWindow,
                dpi))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "AdjustWindowRectExForDpi failed.");
        if (!NativeMethods.SetWindowPos(
                TopLevelHwnd,
                0,
                0,
                0,
                rect.Width,
                rect.Height,
                SwpNoActivate | SwpNoMove))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "SetWindowPos for client resize failed.");
    }

    private T RunRaster<T>(Func<T> operation) =>
        _rasterRunner.RunAsync(operation).AsTask().GetAwaiter().GetResult();

    private void DisposeRasterResources()
    {
        RunRaster(() =>
        {
            if (_scheduledRaster is not null)
            {
                _scheduledRaster.CausalTraceCompleted -= HandleCausalTraceCompleted;
                _scheduledRaster.Dispose();
                _scheduledRaster = null;
            }
            _renderer?.Dispose();
            _surface.Dispose();
            _sharedContext.Dispose();
            return true;
        });
    }

    private static Locale ToLocale(CultureInfo culture)
    {
        var name = culture.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return name.Length switch
        {
            0 => new Locale("en"),
            1 => new Locale(name[0]),
            _ => new Locale(name[0], name[^1]),
        };
    }

    private sealed class ProductResizeWait(long resizeGeneration) : IDisposable
    {
        internal long ResizeGeneration { get; } = resizeGeneration;
        internal ManualResetEventSlim Presented { get; } = new(false);
        internal ManualResetEventSlim PlatformUnblocked { get; } = new(false);
        internal volatile bool PresentedSuccessfully;
        internal volatile bool CausalBound;

        public void Dispose()
        {
            Presented.Dispose();
            PlatformUnblocked.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect(int left, int top, int right, int bottom)
    {
        internal int Left = left;
        internal int Top = top;
        internal int Right = right;
        internal int Bottom = bottom;
        internal readonly int Width => Right - Left;
        internal readonly int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        internal nint Hwnd;
        internal uint Message;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal int PointX;
        internal int PointY;
        internal uint Private;
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetMessageW(out NativeMessage message, nint hwnd, uint minimum, uint maximum);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessageW(out NativeMessage message, nint hwnd, uint minimum, uint maximum, uint remove);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TranslateMessage(in NativeMessage message);
        [DllImport("user32.dll")]
        internal static extern nint DispatchMessageW(in NativeMessage message);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessageW(nint hwnd, uint message, nuint wParam, nint lParam);
        [DllImport("user32.dll")]
        internal static extern void PostQuitMessage(int exitCode);
        [DllImport("user32.dll")]
        internal static extern uint GetDpiForWindow(nint hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustWindowRectExForDpi(
            ref NativeRect rect, uint style, [MarshalAs(UnmanagedType.Bool)] bool hasMenu, uint exStyle, uint dpi);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(
            nint hwnd, nint insertAfter, int x, int y, int width, int height, uint flags);
        [DllImport("dwmapi.dll")]
        internal static extern int DwmFlush();
    }
}

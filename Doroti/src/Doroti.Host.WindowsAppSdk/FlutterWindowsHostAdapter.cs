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
    ILatestMetricsFrameHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    ITextInputHostCapability,
    IPlatformEnvironmentHostCapability,
    IPlatformServicesHostCapability,
    ISkiaSceneRendererHost,
    IFlutterWindowsEngineTaskRunner
{
    private const uint WmClose = 0x0010;
    private const uint WmWindowPosChanging = 0x0046;
    private const uint WmEnterSizeMove = 0x0231;
    private const uint WmExitSizeMove = 0x0232;
    private const uint WmAppFrame = 0x8001;
    private const uint WmAppFirstFrame = 0x8002;
    private const uint WmAppSmokeResize = 0x8004;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoMove = 0x0002;
    private const uint WsOverlappedWindow = 0x00cf0000;
    private const uint WsClipChildren = 0x02000000;
    private const uint WsExAppWindow = 0x00040000;

    private readonly ulong _viewId;
    private readonly DorotiViewConfiguration _configuration;
    private readonly FlutterWindowsAppSdkBootstrap _bootstrap = new();
    private readonly FlutterWindowsHostWindow _host;
    private readonly FlutterWindowsViewMetricsCoordinator _metrics;
    private readonly FlutterWindowsDwmVsyncSource _vsync;
    private readonly FlutterWindowsFrameScheduler _scheduler;
    private readonly FlutterWindowsDedicatedRasterTaskRunner _rasterRunner;
    private readonly FlutterWindowsCompositionSurface _surface;
    private readonly FlutterWindowsInputHost _input;
    private readonly FlutterWindowsUiaBridge _uia;
    private readonly FlutterWindowsLifecycleManager _lifecycle;
    private readonly FlutterWindowsResizeTrace _resizeTrace;
    private readonly ConcurrentQueue<Action> _engineTasks = new();
    private readonly AutoResetEvent _engineTaskSignal = new(false);
    private readonly AutoResetEvent _frameSignal = new(false);
    private readonly int _rendererDelayMilliseconds = ResolveRendererDelayMilliseconds();
    private readonly Thread _engineThread;
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
    private long _frameworkMetricsGeneration;
    private WindowsViewMetrics? _latestFrameworkMetricsRequest;
    private int _frameworkMetricsDrainPosted;
    private long _latestTransientExtentRequest;
    private int _transientMetricsDrainPosted;
    private long _repaintDeferredForMetrics;
    private long _resizePlatformDispatchCount;
    private long _resizePlatformDispatchTotalMicroseconds;
    private long _resizePlatformDispatchMaxMicroseconds;
    private int _engineManagedThreadId;
    private int _firstFrameNotified;
    private int _smokeResizePosted;
    private int _smokeBaseWidth;
    private int _smokeBaseHeight;
    private int _smokeAnchorRight;
    private int _smokeAnchorBottom;
    private int _smokeAnchorInitialized;
    private int _smokeMoveLeft;
    private int _smokeMoveTop;
    private int _smokeAppliedStep;
    private int _interactiveSizeMove;
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
                InitialY: 100,
                UseNoRedirectionBitmap: true),
            new FlutterWindowsHostWindowTeardown
            {
                DisposeViewSurface = DisposeRasterResources,
            });
        _metrics = FlutterWindowsViewMetricsCoordinator.AttachToHostWindow(
            _host,
            viewId,
            new FlutterWindowsPhysicalConstraints(1, 1, 16_384, 16_384));
        Volatile.Write(ref _frameworkMetricsGeneration, _metrics.Current.ResizeGeneration);
        _vsync = new FlutterWindowsDwmVsyncSource(_host.ViewHwnd);
        _scheduler = new FlutterWindowsFrameScheduler(_metrics, _vsync);
        _surface = RunRaster(() => FlutterWindowsCompositionSurface.CreateOnCurrentRasterThread(
            _host.TopLevelHwnd,
            _host.ViewHwnd,
            _metrics.Current));
        Volatile.Write(ref _surfaceGeneration, _surface.Snapshot.SurfaceGeneration);
        _resizeTrace = new FlutterWindowsResizeTrace(viewId, _host.TopLevelHwnd, _host.ViewHwnd);

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
        _host.ChildRepaintRequested += RequestInvalidate;
        _metrics.MetricsPublished += HandleMetricsPublished;
        _scheduler.LatestMetricsFrameRequested += HandleLatestMetricsFrameRequested;
        _input.PointerData += HandlePointerData;
        _input.KeyData += HandleKeyData;
        _input.FocusData += HandleFocusData;
        _input.EditingStateChanged += HandleEditingStateChanged;
        _input.ActionPerformed += HandleActionPerformed;

        _engineThread = new(EngineLoop)
        {
            IsBackground = true,
            Name = "Doroti WindowsAppSdk FlutterEmbedder framework callbacks",
        };
        _engineThread.SetApartmentState(ApartmentState.MTA);
        _engineThread.Start();
        _frameThread = new(FrameLoop)
        {
            IsBackground = true,
            Name = "Doroti WindowsAppSdk FlutterEmbedder DWM frame clock",
        };
        _frameThread.SetApartmentState(ApartmentState.MTA);
        _frameThread.Start();
        _lifecycle.EnsureVisibleInCurrentWorkArea();
    }

    public ViewMetrics Metrics => CurrentFrameworkMetrics.ToViewMetrics(SurfaceGeneration);
    public DorotiViewEpoch ViewEpoch => CurrentFrameworkMetrics.ToViewEpoch();
    public long InputSequence => Volatile.Read(ref _inputSequence);
    public long SurfaceGeneration => Math.Max(1, Volatile.Read(ref _surfaceGeneration));
    public DorotiResizeEpoch ResizeTarget => CurrentFrameworkMetrics.ToResizeEpoch();
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
                _rasterRunner,
                _resizeTrace);
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
        var nativeMetrics = _metrics.Current;
        var surfaceMetrics = surface.TargetMetrics;
        Console.Error.WriteLine(
            "doroti.windowsappsdk.summary=" +
            $"adapter=FlutterEmbedder;topLevel=0x{TopLevelHwnd:x};child=0x{ViewHwnd:x};" +
            $"surfaceGeneration={surface.SurfaceGeneration};presents={surface.SuccessfulPresentCount};" +
            $"presenter=single-dcomp-safe-background-reservoir;capacity={surface.CapacityWidth}x{surface.CapacityHeight};" +
            $"capacityGrowth={surface.CapacityGrowthCount};transientPresents={surface.TransientPresentCount};" +
            $"preparedGeometryGate=false;" +
            $"renderer={surface.AdapterDescription};softwareFallback={surface.SoftwareFallback};" +
            $"queueDepth={scheduler.QueueDepth};queueMax={scheduler.MaxObservedQueueDepth};" +
            $"stalePresent={scheduler.StaleOrWrongSizePresentCount};presented={scheduler.PresentedFrameCount};" +
            $"resizeCallbacks={scheduler.ResizeCallbackCount};resizeMerged={scheduler.ResizeMergedCount};" +
            $"staleCallbacks={scheduler.DroppedStaleCallbackCount};" +
            $"frameRequests={Volatile.Read(ref _frameRequests)};frameDispatches={Volatile.Read(ref _frameDispatches)};" +
            $"resizeDone=0;resizeTimedOut=0;resizeSuperseded=0;" +
            $"resizeDwmFlush=0;resizePlatformWaits=0;" +
            $"resizePlatformDispatchUs={Volatile.Read(ref _resizePlatformDispatchTotalMicroseconds)}/" +
            $"{Volatile.Read(ref _resizePlatformDispatchCount)}/" +
            $"{Volatile.Read(ref _resizePlatformDispatchMaxMicroseconds)};" +
            $"frameworkThread={Volatile.Read(ref _engineManagedThreadId)};" +
            $"engineTasks={Volatile.Read(ref _engineTasksRun)}/{Volatile.Read(ref _engineTasksPosted)};" +
            $"frameworkMetrics={Volatile.Read(ref _frameworkMetricsGeneration)};" +
            $"repaintDeferredForMetrics={Volatile.Read(ref _repaintDeferredForMetrics)};" +
            $"nativeMetrics={nativeMetrics.ResizeGeneration}:{nativeMetrics.PhysicalWidth}x{nativeMetrics.PhysicalHeight};" +
            $"surfaceMetrics={surfaceMetrics?.ResizeGeneration}:{surfaceMetrics?.PhysicalWidth}x{surfaceMetrics?.PhysicalHeight};" +
            $"dpiChanges={lifecycle.DpiChangedCount};displayChanges={lifecycle.DisplayChangedCount};" +
            $"alpha=premultiplied;surfaceRecreateOnResize=false;submitted={diagnostics.Submitted};" +
            $"scenePresented={diagnostics.Presented};failed={diagnostics.Failed};superseded={diagnostics.Superseded}");
    }

    public void ApplyLeftResizeSmokeStep(int step)
        => QueueAnchoredResizeSmokeStep(step, moveLeft: true, moveTop: false);

    public void ApplyTopLeftResizeSmokeStep(int step)
        => QueueAnchoredResizeSmokeStep(step, moveLeft: true, moveTop: true);

    private void QueueAnchoredResizeSmokeStep(int step, bool moveLeft, bool moveTop)
    {
        if (_disposed) return;
        _ = step;
        Volatile.Write(ref _smokeMoveLeft, moveLeft ? 1 : 0);
        Volatile.Write(ref _smokeMoveTop, moveTop ? 1 : 0);
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
        ArgumentNullException.ThrowIfNull(callback);
        ScheduleFrameCore(
            expectedEpoch,
            (timestamp, _) => callback(timestamp),
            canReplaceBeforeDispatch: false);
    }

    public void ScheduleFrame(
        DorotiViewEpoch expectedEpoch,
        Action<TimeSpan, DorotiViewEpoch> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ScheduleFrameCore(expectedEpoch, callback, canReplaceBeforeDispatch: true);
    }

    private void ScheduleFrameCore(
        DorotiViewEpoch expectedEpoch,
        Action<TimeSpan, DorotiViewEpoch> callback,
        bool canReplaceBeforeDispatch)
    {
        ArgumentNullException.ThrowIfNull(expectedEpoch);
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        Interlocked.Increment(ref _frameRequests);
        var metrics = CurrentFrameworkMetrics;
        FlutterWindowsFrameScheduleResult result;
        FlutterWindowsScheduledFrameCallback dispatch = (ticket, vsync) =>
        {
            try
            {
                PostEngineTask(() => DispatchFrameworkFrame(ticket, vsync, callback));
            }
            catch (Exception exception)
            {
                _scheduler.ReportFrameFailure(ticket,
                    $"framework callback post failed: {exception.GetType().Name}");
                throw;
            }
        };
        if (Volatile.Read(ref _pendingResizeGeneration) == metrics.ResizeGeneration &&
            expectedEpoch.ResizeTargetGeneration == metrics.ResizeGeneration)
        {
            result = _scheduler.ScheduleResize(metrics, dispatch, canReplaceBeforeDispatch);
            if (result.Accepted)
            {
                Interlocked.CompareExchange(ref _pendingResizeGeneration, 0, metrics.ResizeGeneration);
            }
        }
        else
        {
            result = _scheduler.ScheduleOrdinary(expectedEpoch, dispatch, canReplaceBeforeDispatch);
        }
        if (!result.Accepted) return;
        _frameSignal.Set();
    }

    public void RequestInvalidate()
    {
        if (_disposed || _renderer is null || _scheduledRaster is null) return;
        var metrics = _metrics.Current;
        // WM_PAINT for the resized child can arrive before the independent
        // framework thread has applied that metrics generation to RenderView.
        // Replaying here would tag an old layout with the new native extent and
        // leave a transparent strip even though the scheduler/surface sizes are
        // exact. The queued MetricsChanged callback owns the next frame.
        if (Volatile.Read(ref _frameworkMetricsGeneration) < metrics.ResizeGeneration)
        {
            Interlocked.Increment(ref _repaintDeferredForMetrics);
            return;
        }
        var scheduler = _scheduler.Snapshot;
        if (scheduler.HasPendingResize || scheduler.HasPendingOrdinary ||
            scheduler.HasResizeInFlight || scheduler.HasOrdinaryInFlight)
        {
            _frameSignal.Set();
            return;
        }
        var pendingResize = Volatile.Read(ref _pendingResizeGeneration) == metrics.ResizeGeneration;
        var result = pendingResize
            ? _scheduler.ScheduleResize(metrics, QueueRaster)
            : _scheduler.ScheduleOrdinary(metrics.ToViewEpoch(), QueueRaster);
        if (result.Accepted)
        {
            if (pendingResize)
            {
                Interlocked.CompareExchange(ref _pendingResizeGeneration, 0, metrics.ResizeGeneration);
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
        _engineTaskSignal.Set();
    }

    public bool TryRunOneTask()
    {
        if (Environment.CurrentManagedThreadId != Volatile.Read(ref _engineManagedThreadId))
            return false;
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
        TerminalizePendingProductWork();
        _engineTaskSignal.Set();
        _frameSignal.Set();
        if (Thread.CurrentThread != _engineThread && !_engineThread.Join(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("FlutterEmbedder framework callback runner did not stop.");
        if (Thread.CurrentThread != _frameThread && !_frameThread.Join(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("FlutterEmbedder frame clock did not stop.");
        _host.TopLevelMessageReceived -= HandleTopLevelMessage;
        _host.ChildRepaintRequested -= RequestInvalidate;
        _metrics.MetricsPublished -= HandleMetricsPublished;
        _scheduler.LatestMetricsFrameRequested -= HandleLatestMetricsFrameRequested;
        _input.PointerData -= HandlePointerData;
        _input.KeyData -= HandleKeyData;
        _input.FocusData -= HandleFocusData;
        _input.EditingStateChanged -= HandleEditingStateChanged;
        _input.ActionPerformed -= HandleActionPerformed;
        _lifecycle.BeginShutdown();
        var schedulerSummary = _scheduler.Snapshot;
        var rasterSummary = _scheduledRaster?.Snapshot;
        _resizeTrace.Record(
            "shutdown",
            _metrics.Current,
            detail: $"traceDropped={_resizeTrace.DroppedEventCount};" +
                    $"queueMax={schedulerSummary.MaxObservedQueueDepth};" +
                    $"stalePresent={schedulerSummary.StaleOrWrongSizePresentCount};" +
                    $"failed={rasterSummary?.FailureCount ?? 0};" +
                    $"causalGap={schedulerSummary.CausalGapCount};" +
                    $"receiptMismatch={rasterSummary?.CausalReceiptMismatchCount ?? 0}");
        _uia.Dispose();
        _input.Dispose();
        _lifecycle.Dispose();
        _scheduler.Dispose();
        _metrics.Dispose();
        _host.Dispose();
        _resizeTrace.Dispose();
        _rasterRunner.Dispose();
        _engineTaskSignal.Dispose();
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
        _resizeTrace.Record(
            "frameworkFrameTerminal",
            causalFrameId: trace.CausalFrameId,
            detail: $"viewId={trace.ViewId};generation={trace.ResizeGeneration};" +
                    $"size={trace.PhysicalWidth}x{trace.PhysicalHeight};presented={trace.Presented}");
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
        _frameSignal.Set();
    }

    private FlutterWindowsChildMessageResult HandleTopLevelMessage(FlutterWindowsTopLevelMessage message)
    {
        switch (message.Message)
        {
            case WmAppFrame:
                var frame = _scheduler.TryRunOneFrame();
                if (frame.Disposition == FlutterWindowsFrameRunDisposition.CallbackFailed)
                    Console.Error.WriteLine("doroti.windowsappsdk.flutter.frame-callback=failed");
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmAppFirstFrame:
                if (!_host.Snapshot.FirstFrameSwapped) _host.NotifyFirstFrameSwapped();
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmAppSmokeResize:
                HandleSmokeResize();
                return FlutterWindowsChildMessageResult.HandledResult();
            case WmWindowPosChanging:
                ObserveProposedWindowPos(message.LParam);
                return FlutterWindowsChildMessageResult.Unhandled;
            case WmEnterSizeMove:
                Volatile.Write(ref _interactiveSizeMove, 1);
                _resizeTrace.Record("resizeStarted", _metrics.Current);
                return FlutterWindowsChildMessageResult.Unhandled;
            case WmExitSizeMove:
                Volatile.Write(ref _interactiveSizeMove, 0);
                PostFinalResizeRecovery();
                _resizeTrace.Record("resizeDone", _metrics.Current);
                return FlutterWindowsChildMessageResult.Unhandled;
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
        var started = Stopwatch.GetTimestamp();
        _resizeTrace.Record("windowSizeObserved", metrics);
        Volatile.Write(ref _pendingResizeGeneration, metrics.ResizeGeneration);
        if (_renderer is null || _scheduledRaster is null || !metrics.HasDrawableSize)
        {
            PostFrameworkMetricsChanged(metrics);
            return;
        }

        // Child WM_SIZE runs on the platform thread inside Windows' modal
        // sizing loop. Waiting here for framework/raster/present serializes
        // every mouse move and can also defer WM_EXITSIZEMOVE and WM_CLOSE,
        // which presents as progressive lag or a frozen window after mouse-up.
        // Publish the exact immutable metrics to the independent framework
        // thread and return immediately. The scheduler still admits only the
        // latest exact generation, and the raster receipt remains the terminal
        // signal; it simply no longer blocks native message dispatch.
        PostFrameworkMetricsChanged(metrics);
        var dispatchElapsed = Stopwatch.GetElapsedTime(started);
        RecordResizePlatformDispatch(dispatchElapsed);
        _resizeTrace.Record("windowSizeHandled", metrics, detail:
            $"dispatchMicroseconds={Math.Max(0L, dispatchElapsed.Ticks / 10L)}");
    }

    private void HandleLatestMetricsFrameRequested(WindowsViewMetrics metrics)
    {
        Volatile.Write(ref _pendingResizeGeneration, metrics.ResizeGeneration);
        PostFrameworkMetricsChanged(metrics);
        _frameSignal.Set();
    }

    private WindowsViewMetrics CurrentFrameworkMetrics => _metrics.Current;

    private void PostTransientExtent(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        var packed = ((long)width << 32) | (uint)height;
        Interlocked.Exchange(ref _latestTransientExtentRequest, packed);
        if (Interlocked.Exchange(ref _transientMetricsDrainPosted, 1) != 0) return;
        _rasterRunner.Post(DrainTransientMetrics);
    }

    private void DrainTransientMetrics()
    {
        try
        {
            while (!_disposed)
            {
                var requested = Interlocked.Exchange(ref _latestTransientExtentRequest, 0);
                if (requested == 0) return;
                var width = checked((int)(requested >> 32));
                var height = checked((int)(requested & uint.MaxValue));
                if (_surface.PresentTransient(width, height))
                {
                    _resizeTrace.Record("transientFrontUpdated", _metrics.Current,
                        detail: $"mode=safe-background-reservoir;exact=false;target={width}x{height}");
                }
            }
        }
        finally
        {
            Volatile.Write(ref _transientMetricsDrainPosted, 0);
            if (!_disposed && Volatile.Read(ref _latestTransientExtentRequest) != 0 &&
                Interlocked.Exchange(ref _transientMetricsDrainPosted, 1) == 0)
            {
                _rasterRunner.Post(DrainTransientMetrics);
            }
        }
    }

    private void ObserveProposedWindowPos(nint lParam)
    {
        if (_disposed || lParam == 0) return;
        var windowPos = Marshal.PtrToStructure<NativeWindowPos>(lParam);
        if ((windowPos.Flags & SwpNoSize) != 0 || windowPos.Width <= 0 || windowPos.Height <= 0)
            return;
        _resizeTrace.Record("windowPosProposed", _metrics.Current, detail:
            $"x={windowPos.X};y={windowPos.Y};width={windowPos.Width};height={windowPos.Height};flags={windowPos.Flags}");
        if (Volatile.Read(ref _interactiveSizeMove) != 0 &&
            NativeMethods.GetWindowRect(TopLevelHwnd, out var outer) &&
            NativeMethods.GetClientRect(TopLevelHwnd, out var client))
        {
            var proposedClientWidth = windowPos.Width - Math.Max(0, outer.Width - client.Width);
            var proposedClientHeight = windowPos.Height - Math.Max(0, outer.Height - client.Height);
            PostTransientExtent(proposedClientWidth, proposedClientHeight);
        }
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

    private void PostFrameworkMetricsChanged(WindowsViewMetrics requestedMetrics)
    {
        Volatile.Write(ref _latestFrameworkMetricsRequest, requestedMetrics);
        TryPostFrameworkMetricsDrain();
    }

    private void TryPostFrameworkMetricsDrain()
    {
        if (_disposed) return;
        if (Interlocked.Exchange(ref _frameworkMetricsDrainPosted, 1) != 0) return;
        PostEngineTask(DrainLatestFrameworkMetrics);
    }

    private void DrainLatestFrameworkMetrics()
    {
        var requested = Interlocked.Exchange(ref _latestFrameworkMetricsRequest, null);
        if (requested is not null)
        {
            var appliedMetrics = CurrentFrameworkMetrics;
            if (appliedMetrics.ResizeGeneration < requested.ResizeGeneration)
                appliedMetrics = requested;
            ApplyFrameworkMetrics(appliedMetrics);
        }
        Interlocked.Exchange(ref _frameworkMetricsDrainPosted, 0);
        if (Volatile.Read(ref _latestFrameworkMetricsRequest) is not null)
            TryPostFrameworkMetricsDrain();
    }

    private void PostFinalResizeRecovery()
    {
        var requestedMetrics = _metrics.Current;
        PostEngineTask(() =>
        {
            ApplyFrameworkMetrics(requestedMetrics);
            // This is ordered after all metrics callbacks already queued by
            // the platform thread. It guarantees one final scheduler wake-up
            // for the actual mouse-up extent without blocking WM_EXITSIZEMOVE.
            RequestInvalidate();
        });
    }

    private void ApplyFrameworkMetrics(WindowsViewMetrics requestedMetrics)
    {
        // Coalesce native publications at the framework-thread boundary.
        // DorotiView reads the host metrics while handling this callback, so
        // publish and record the same latest immutable observation.
        var appliedMetrics = CurrentFrameworkMetrics;
        if (appliedMetrics.ResizeGeneration < requestedMetrics.ResizeGeneration)
            appliedMetrics = requestedMetrics;
        if (Volatile.Read(ref _frameworkMetricsGeneration) >= appliedMetrics.ResizeGeneration)
            return;
        MetricsChanged?.Invoke(appliedMetrics.ToViewMetrics(SurfaceGeneration));
        Volatile.Write(ref _frameworkMetricsGeneration, appliedMetrics.ResizeGeneration);
        _resizeTrace.Record("metricsDelivered", appliedMetrics);
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
                    // Pace both ordinary and interactive work on this
                    // independent frame-clock thread. Skipping DwmFlush while
                    // sizing lets framework/raster work run flat out and
                    // measurably stalls the native sizing loop.
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

    private void EngineLoop()
    {
        Volatile.Write(ref _engineManagedThreadId, Environment.CurrentManagedThreadId);
        try
        {
            while (true)
            {
                _engineTaskSignal.WaitOne();
                if (_disposed) return;
                while (!_disposed && TryRunOneTask()) { }
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"doroti.windowsappsdk.flutter.framework.fatal={exception}");
            _ = NativeMethods.PostMessageW(TopLevelHwnd, WmClose, 0, 0);
        }
    }

    private void HandleSmokeResize()
    {
        var version = Volatile.Read(ref _smokeVersion);
        var current = _metrics.Current;
        var baseWidth = Volatile.Read(ref _smokeBaseWidth);
        if (baseWidth == 0)
        {
            Interlocked.CompareExchange(ref _smokeBaseWidth, current.PhysicalWidth, 0);
            Interlocked.CompareExchange(ref _smokeBaseHeight, current.PhysicalHeight, 0);
            baseWidth = Volatile.Read(ref _smokeBaseWidth);
        }
        if (Interlocked.CompareExchange(ref _smokeAnchorInitialized, 1, 0) == 0)
        {
            if (!NativeMethods.GetWindowRect(TopLevelHwnd, out var windowRect))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GetWindowRect for resize smoke failed.");
            Volatile.Write(ref _smokeAnchorRight, windowRect.Right);
            Volatile.Write(ref _smokeAnchorBottom, windowRect.Bottom);
        }
        var appliedStep = Interlocked.Increment(ref _smokeAppliedStep);
        var cycle = Math.Abs(appliedStep % 48);
        var delta = (cycle <= 24 ? cycle : 48 - cycle) * 4;
        var baseHeight = Math.Max(240, Volatile.Read(ref _smokeBaseHeight));
        var moveTop = Volatile.Read(ref _smokeMoveTop) != 0;
        var dpi = NativeMethods.GetDpiForWindow(TopLevelHwnd);
        if (dpi == 0) dpi = 96;
        SetClientSize(
            Math.Max(320, baseWidth - delta),
            moveTop ? Math.Max(240, baseHeight - delta) : baseHeight,
            dpi,
            Volatile.Read(ref _smokeMoveLeft) != 0,
            moveTop);
        Interlocked.Exchange(ref _smokeResizePosted, 0);
        if (version != Volatile.Read(ref _smokeVersion) &&
            Interlocked.Exchange(ref _smokeResizePosted, 1) == 0)
            _ = NativeMethods.PostMessageW(TopLevelHwnd, WmAppSmokeResize, 0, 0);
    }

    private void TerminalizePendingProductWork()
    {
        _scheduler.SetSuspended(true);
    }

    private void DispatchFrameworkFrame(
        FlutterWindowsFrameTicket ticket,
        FlutterWindowsVsyncSample vsync,
        Action<TimeSpan, DorotiViewEpoch> callback)
    {
        if (_disposed)
        {
            _scheduler.ReportFrameFailure(ticket, "framework callback runner stopped");
            return;
        }
        try
        {
            if (_rendererDelayMilliseconds > 0 && Volatile.Read(ref _interactiveSizeMove) != 0)
            {
                _resizeTrace.Record("rendererDelayInjected", ticket.Metrics, ticket.CausalFrameId,
                    $"milliseconds={_rendererDelayMilliseconds}");
                Thread.Sleep(_rendererDelayMilliseconds);
            }
            callback(vsync.Timestamp, ticket.ExpectedEpoch);
            _resizeTrace.Record("sceneBuilt", ticket.Metrics, ticket.CausalFrameId,
                $"kind={ticket.Kind}");
            QueueRaster(ticket, vsync);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"doroti.windowsappsdk.flutter.framework-frame.fail={exception}");
            _scheduler.ReportFrameFailure(ticket,
                $"framework callback failed: {exception.GetType().Name}");
        }
    }

    private void RecordResizePlatformDispatch(TimeSpan elapsed)
    {
        var microseconds = Math.Max(0L, elapsed.Ticks / 10L);
        Interlocked.Increment(ref _resizePlatformDispatchCount);
        Interlocked.Add(ref _resizePlatformDispatchTotalMicroseconds, microseconds);
        var observed = Volatile.Read(ref _resizePlatformDispatchMaxMicroseconds);
        while (microseconds > observed)
        {
            var previous = Interlocked.CompareExchange(
                ref _resizePlatformDispatchMaxMicroseconds,
                microseconds,
                observed);
            if (previous == observed) break;
            observed = previous;
        }
    }

    private void SetClientSize(
        int width,
        int height,
        uint dpi,
        bool moveLeft = false,
        bool moveTop = false)
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
        var x = moveLeft ? Volatile.Read(ref _smokeAnchorRight) - rect.Width : 0;
        var y = moveTop ? Volatile.Read(ref _smokeAnchorBottom) - rect.Height : 0;
        var flags = SwpNoActivate;
        if (!moveLeft && !moveTop) flags |= SwpNoMove;
        if (!NativeMethods.SetWindowPos(
                TopLevelHwnd,
                0,
                x,
                y,
                rect.Width,
                rect.Height,
                flags))
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
              return true;
        });
    }

    private static int ResolveRendererDelayMilliseconds()
    {
        var value = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_RESIZE_RENDERER_DELAY_MS");
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var milliseconds) &&
               milliseconds is >= 0 and <= 1_000
            ? milliseconds
            : 0;
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
    private struct NativeWindowPos
    {
        internal nint Hwnd;
        internal nint InsertAfter;
        internal int X;
        internal int Y;
        internal int Width;
        internal int Height;
        internal uint Flags;
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
        internal static extern bool GetWindowRect(nint hwnd, out NativeRect rect);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(nint hwnd, out NativeRect rect);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(
            nint hwnd, nint insertAfter, int x, int y, int width, int height, uint flags);
        [DllImport("dwmapi.dll")]
        internal static extern int DwmFlush();
    }
}

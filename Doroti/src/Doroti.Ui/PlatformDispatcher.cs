using Doroti.Runtime;

namespace Doroti.Ui;

/// <summary>Managed dart:ui dispatcher. Every callback carries its originating view.</summary>
public sealed class PlatformDispatcher : IDisposable
{
    private static readonly AsyncLocal<PlatformDispatcher?> ActiveDispatcher = new();
    private readonly object _gate = new();
    private readonly object _dispatchGate = new();
    private readonly Dictionary<ulong, DorotiView> _views = [];
    private readonly DartMicrotaskQueue _microtasks = new();
    private readonly DorotiFrameTrace _frameTrace = new();
    private readonly IDartPerformanceModeCapability? _performanceModeCapability;
    private readonly HashSet<Guid> _backgroundIsolates = [];
    private ChannelBuffers? _channelBuffers;
    private AccessibilityFeatures _accessibilityFeatures =
        new(false, false, false, false, false, false, false);
    private long _frameNumber;
    private long _frameTransactionNumber;
    private int _dispatchDepth;
    private bool _disposed;

    public PlatformDispatcher(IDartPerformanceModeCapability? performanceModeCapability = null)
    {
        _performanceModeCapability = performanceModeCapability;
    }

    /// <summary>
    /// The dispatcher active for the current managed execution context. Hosts and
    /// framework bindings should enter a scope instead of replacing process-global state.
    /// </summary>
    public static PlatformDispatcher instance => ActiveDispatcher.Value ??
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.DartPerformanceMode,
            null,
            DartUiInvocation.Managed("dart:ui#PlatformDispatcher.instance"),
            "no dispatcher is active in the current execution context");

    internal static PlatformDispatcher? current => ActiveDispatcher.Value;

    public DorotiFrameTrace frameTrace => _frameTrace;

    public IDisposable EnterScope()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var previous = ActiveDispatcher.Value;
        ActiveDispatcher.Value = this;
        return new DispatcherScope(previous, DartAsyncRuntime.enterMicrotaskScheduler(EnqueueMicrotask));
    }

    private void EnqueueMicrotask(Action callback)
    {
        DorotiView[] registered;
        lock (_gate)
        {
            // A Dart timer can race the final framework unmount. Its captured
            // scheduler belongs to this dispatcher, so callbacks arriving after
            // disposal/detach must not escape on a ThreadPool thread or revive a
            // closed view.
            if (_disposed || _views.Count == 0) return;
            _microtasks.enqueue(callback);
            if (Volatile.Read(ref _dispatchDepth) != 0) return;
            registered = _views.Values.ToArray();
        }
        foreach (var view in registered)
        {
            view.ScheduleFrame(DartUiInvocation.Managed("dart:ui#PlatformDispatcher.microtask"));
        }
    }

    public void requestDartPerformanceMode(DartPerformanceMode mode)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_performanceModeCapability is null)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.DartPerformanceMode,
                null,
                DartUiInvocation.Managed("dart:ui#PlatformDispatcher.requestDartPerformanceMode"),
                "the active host did not register it");
        }
        _performanceModeCapability.Request(mode);
    }

    public void scheduleFrame()
    {
        DorotiView[] registered;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            registered = _views.Values.ToArray();
        }
        if (registered.Length == 0)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.ViewFrameDispatch,
                null,
                DartUiInvocation.Managed("dart:ui#PlatformDispatcher.scheduleFrame"),
                "no Flutter view is registered");
        }
        foreach (var view in registered)
        {
            view.ScheduleFrame(DartUiInvocation.Managed("dart:ui#PlatformDispatcher.scheduleFrame"));
        }
    }

    public void scheduleWarmUpFrame(Action beginFrame, Action drawFrame)
    {
        ArgumentNullException.ThrowIfNull(beginFrame);
        ArgumentNullException.ThrowIfNull(drawFrame);
        ObjectDisposedException.ThrowIf(_disposed, this);
        var registeredViews = views;
        if (registeredViews.Count == 0)
        {
            beginFrame();
            drawFrame();
            return;
        }
        var frameNumber = Interlocked.Increment(ref _frameNumber);
        var buildScopes = registeredViews
            .Select(view => view.EnterSceneBuildScope(view.CaptureViewEpoch(), frameNumber))
            .ToArray();
        try
        {
            beginFrame();
            drawFrame();
        }
        finally
        {
            foreach (var scope in buildScopes.Reverse()) scope.Dispose();
        }
    }

    public IReadOnlyList<DorotiView> views
    {
        get
        {
            lock (_gate)
            {
                return _views.Values.OrderBy(view => view.viewId).ToArray();
            }
        }
    }

    public AccessibilityFeatures accessibilityFeatures
    {
        get => _accessibilityFeatures;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (_accessibilityFeatures == value) return;
            _accessibilityFeatures = value;
            onAccessibilityFeaturesChanged?.Invoke();
        }
    }

    public DorotiView? implicitView => views.Count == 1 ? views[0] : null;
    public string defaultRouteName { get; set; } = "/";
    public bool brieflyShowPassword { get; set; } = true;
    public IReadOnlyList<Locale> locales => implicitView?.environmentConfiguration?.locales ?? [];
    public Brightness platformBrightness => implicitView?.environmentConfiguration?.platformBrightness ?? Brightness.light;
    public bool alwaysUse24HourFormat => implicitView?.environmentConfiguration?.alwaysUse24HourFormat ?? false;
    public bool nativeSpellCheckServiceDefined => implicitView?.environmentConfiguration?.nativeSpellCheckServiceDefined ?? false;
    public double textScaleFactor => implicitView?.environmentConfiguration?.textScaleFactor ?? 1.0;
    public bool supportsShowingSystemContextMenu => implicitView?.environmentConfiguration?.supportsShowingSystemContextMenu ?? false;
    public double? lineHeightScaleFactorOverride => implicitView?.environmentConfiguration?.lineHeightScaleFactorOverride;
    public double? letterSpacingOverride => implicitView?.environmentConfiguration?.letterSpacingOverride;
    public double? wordSpacingOverride => implicitView?.environmentConfiguration?.wordSpacingOverride;
    public double? paragraphSpacingOverride => implicitView?.environmentConfiguration?.paragraphSpacingOverride;
    public FrameData frameData => new(Volatile.Read(ref _frameNumber));

    public void setApplicationLocale(Locale locale) => applicationLocale = locale;

    public Locale? applicationLocale { get; private set; }

    public Locale? computePlatformResolvedLocale(List<Locale> supportedLocales)
    {
        ArgumentNullException.ThrowIfNull(supportedLocales);
        foreach (var preferred in locales)
        {
            var exact = supportedLocales.FirstOrDefault(candidate => candidate == preferred);
            if (exact != default) return exact;
            var language = supportedLocales.FirstOrDefault(candidate =>
                string.Equals(candidate.languageCode, preferred.languageCode, StringComparison.OrdinalIgnoreCase));
            if (language != default) return language;
        }
        return supportedLocales.FirstOrDefault();
    }

    public double scaleFontSize(double unscaledFontSize)
    {
        if (!double.IsFinite(unscaledFontSize) || unscaledFontSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unscaledFontSize));
        }
        return unscaledFontSize * textScaleFactor;
    }

    public DorotiView? view(long id)
    {
        lock (_gate)
        {
            return id >= 0 && _views.TryGetValue(checked((ulong)id), out var result) ? result : null;
        }
    }

    public void requestViewFocusChange(long viewId, ViewFocusState state, ViewFocusDirection direction)
    {
        var target = view(viewId) ?? throw new ArgumentOutOfRangeException(nameof(viewId), viewId, "Unknown Flutter view.");
        target.requestFocusChange(state, direction);
    }

    public Action<DorotiView>? onMetricsChanged { get; set; }

    public Action? onTextScaleFactorChanged { get; set; }

    public Action? onPlatformBrightnessChanged { get; set; }

    public bool semanticsEnabled => views.Any(view => view.semanticsEnabled);

    public Action? semanticsEnabledChanged { get; set; }

    public Action? onSemanticsEnabledChanged
    {
        get => semanticsEnabledChanged;
        set => semanticsEnabledChanged = value;
    }

    public Action? onAccessibilityFeaturesChanged { get; set; }

    public void addSemanticsEnabledListener(Action listener) => semanticsEnabledChanged += listener;

    public void removeSemanticsEnabledListener(Action listener) => semanticsEnabledChanged -= listener;

    public Action<DorotiView, AppLifecycleState>? onAppLifecycleStateChanged { get; set; }

    public event Action<DorotiView, AppLifecycleState>? lifecycleChanged;

    public Action<Doroti.Runtime.Duration>? onBeginFrame { get; set; }

    public Action? onDrawFrame { get; set; }

    public Action<List<FrameTiming>>? onReportTimings { get; set; }

    /// <summary>Framework-side frame listeners. Concrete hosts only raise the dart:ui callbacks.</summary>
    public event Action<DorotiView, TimeSpan>? beginFrame;

    public event Action<DorotiView>? drawFrame;

    public Action<DorotiView, PointerDataPacket>? onPointerDataPacket { get; set; }

    /// <summary>dart:ui PlatformDispatcher.onHitTest — framework hit-test callback for platform views.</summary>
    public Func<HitTestRequest, HitTestResponse>? onHitTest { get; set; }

    public Func<KeyData, bool>? onKeyData { get; set; }

    public event Action<DorotiView, KeyData>? keyData;

    public Action<DorotiView, RawFocusData>? onFocusData { get; set; }

    public event Action<DorotiView, RawFocusData>? focusData;

    public Action<DorotiView>? onViewFocusChanged { get; set; }
    public Action<ViewFocusEvent>? onViewFocusChange { get; set; }

    public string? initialLifecycleState
    {
        get
        {
            lock (_gate)
            {
                return _views.Values.OrderBy(view => view.viewId).FirstOrDefault()?.metrics.lifecycleState.ToString();
            }
        }
    }

    public ChannelBuffers channelBuffers => _channelBuffers ??= new(RequireMessagingView());

    public void registerBackgroundIsolate(RootIsolateToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        lock (_gate) _backgroundIsolates.Add(token.Value);
    }

    public async void sendPlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback)
    {
        var response = await RequireMessagingView().SendPlatformMessageAsync(
            channel, data?.asMemory(), DartUiInvocation.Managed("dart:ui#PlatformDispatcher.sendPlatformMessage"));
        callback?.Invoke(response is null ? null : (ByteData)response.Value);
    }

    public async void sendPortPlatformMessage(string channel, ByteData? data, long identifier, SendPort sendPort)
    {
        ArgumentNullException.ThrowIfNull(sendPort);
        var response = await RequireMessagingView().SendPlatformMessageAsync(
            channel, data?.asMemory(), DartUiInvocation.Managed("dart:ui#PlatformDispatcher.sendPortPlatformMessage"));
        sendPort.send(new List<object?> { identifier, response is null ? null : new Uint8List(response.Value.ToArray()) });
    }

    public Action<DorotiView, PlatformConfiguration>? onPlatformConfigurationChanged { get; set; }

    public Action<SemanticsActionEvent>? onSemanticsActionEvent { get; set; }

    public void setSemanticsTreeEnabled(bool enabled)
    {
        DorotiView[] registered;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            registered = _views.Values.ToArray();
        }
        foreach (var view in registered)
        {
            view.SetSemanticsTreeEnabled(
                enabled,
                DartUiInvocation.Managed("dart:ui#PlatformDispatcher.setSemanticsTreeEnabled"));
        }
    }

    public DorotiView RegisterView(ulong viewId, DorotiViewCapabilities capabilities)
    {
        if (viewId == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewId), "A Flutter view id must be non-zero.");
        }
        ArgumentNullException.ThrowIfNull(capabilities);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_views.ContainsKey(viewId))
            {
                throw new InvalidOperationException($"Flutter view {viewId} is already registered.");
            }
            capabilities.Seal();
            DorotiView view;
            try
            {
                view = new DorotiView(this, viewId, capabilities);
            }
            catch
            {
                capabilities.Dispose();
                throw;
            }
            _views.Add(viewId, view);
            return view;
        }
    }

    public DorotiView GetView(ulong viewId, DartUiInvocation invocation)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_views.TryGetValue(viewId, out var view))
            {
                return view;
            }
        }
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.ViewLifecycleMetrics,
            viewId,
            invocation,
            "the view is not registered or has already shut down");
    }

    internal void Remove(DorotiView view)
    {
        lock (_gate)
        {
            if (_views.TryGetValue(view.viewId, out var current) && ReferenceEquals(current, view))
            {
                _views.Remove(view.viewId);
            }
        }
    }

    internal void DispatchMetrics(DorotiView view) => DispatchWithEnvironment(view, () => onMetricsChanged?.Invoke(view));

    internal void DispatchLifecycle(DorotiView view, AppLifecycleState state) =>
        DispatchWithEnvironment(view, () =>
        {
            onAppLifecycleStateChanged?.Invoke(view, state);
            lifecycleChanged?.Invoke(view, state);
        });

    internal void DispatchPointer(DorotiView view, PointerDataPacket packet) =>
        DispatchWithEnvironment(view, () => onPointerDataPacket?.Invoke(view, packet));

    internal void DispatchKey(DorotiView view, KeyData data) =>
        DispatchWithEnvironment(view, () =>
        {
            onKeyData?.Invoke(data);
            keyData?.Invoke(view, data);
        });

    internal void DispatchFocus(DorotiView view, RawFocusData data)
    {
        DispatchWithEnvironment(view, () =>
        {
            onFocusData?.Invoke(view, data);
            focusData?.Invoke(view, data);
            onViewFocusChanged?.Invoke(view);
            onViewFocusChange?.Invoke(new ViewFocusEvent(view.viewId, data.isFocused));
        });
    }

    internal void DispatchPlatformConfiguration(DorotiView view, PlatformConfiguration configuration)
    {
        DispatchWithEnvironment(view, () =>
        {
            onPlatformConfigurationChanged?.Invoke(view, configuration);
            onTextScaleFactorChanged?.Invoke();
            onPlatformBrightnessChanged?.Invoke();
        });
    }

    internal void DispatchSemanticsAction(DorotiView view, SemanticsActionEvent action) =>
        DispatchWithEnvironment(view, () => onSemanticsActionEvent?.Invoke(action));

    internal void DispatchFrame(
        DorotiView view,
        TimeSpan timestamp,
        DorotiViewEpoch requestedEpoch,
        DorotiFrameTransaction? transaction = null)
    {
        DispatchWithEnvironment(view, () =>
        {
            transaction?.DeliverMetrics(requestedEpoch);
            var frameNumber = Interlocked.Increment(ref _frameNumber);
            using var buildScope = view.EnterSceneBuildScope(requestedEpoch, frameNumber, transaction);
            _frameTrace.Record(DorotiFramePhase.beginFrame, view.viewId, timestamp);
            onBeginFrame?.Invoke(timestamp);
            beginFrame?.Invoke(view, timestamp);
            onDrawFrame?.Invoke();
            drawFrame?.Invoke(view);
            _frameTrace.Record(DorotiFramePhase.drawFrame, view.viewId, DorotiFrameClock.Now);
        });
    }

    internal long NextFrameTransactionId() =>
        Interlocked.Increment(ref _frameTransactionNumber);

    private void DispatchWithEnvironment(DorotiView view, Action callback)
    {
        // A Flutter isolate has one event loop. Android delivers TextureView
        // paints on its GL thread while touch, semantics, and lifecycle events
        // arrive on the UI thread, so serialize their framework callbacks here.
        // The monitor is reentrant for nested framework dispatch on one thread.
        lock (_dispatchGate)
        {
            using var dispatcherScope = EnterScope();
            if (view.environmentConfiguration is { } configuration)
            {
                using var environmentScope = PlatformEnvironmentContext.Enter(configuration);
                DispatchAndDrainMicrotasks(callback);
                return;
            }
            DispatchAndDrainMicrotasks(callback);
        }
    }

    private void DispatchAndDrainMicrotasks(Action callback)
    {
        _dispatchDepth++;
        try
        {
            callback();
        }
        finally
        {
            _dispatchDepth--;
            if (_dispatchDepth == 0)
            {
                _microtasks.drain();
            }
        }
    }

    private DorotiView RequireMessagingView()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_views.Count == 1) return _views.Values.Single();
        }
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.PlatformMessaging, null,
            DartUiInvocation.Managed("dart:ui#PlatformDispatcher.platformMessaging"),
            "platform messaging requires exactly one active view in this host-neutral dispatcher scope");
    }

    internal ValueTask<ReadOnlyMemory<byte>> LoadApplicationResourceAsync(
        string key,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var view = RequireMessagingView();
        return view.RequireCapability<IApplicationResourceHostCapability>(
            DorotiCapabilityIds.ApplicationResources, invocation).LoadAsync(key, cancellationToken);
    }

    public void Dispose()
    {
        DorotiView[] viewsToDispose;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            viewsToDispose = _views.Values.ToArray();
            _views.Clear();
        }
        foreach (var view in viewsToDispose)
        {
            view.DisposeFromDispatcher();
        }
    }

    private sealed class DispatcherScope(
        PlatformDispatcher? previous,
        IDisposable microtaskSchedulerScope) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            try
            {
                microtaskSchedulerScope.Dispose();
            }
            finally
            {
                ActiveDispatcher.Value = previous;
            }
        }
    }
}

public readonly record struct FrameData(long frameNumber);

public enum DartPerformanceMode
{
    balanced,
    latency,
    throughput,
    memory
}

public sealed class DorotiView : IDisposable
{
    private readonly PlatformDispatcher _dispatcher;
    private readonly DorotiViewCapabilities _capabilities;
    private readonly IViewHostCapability _viewHost;
    private readonly IInputHostCapability? _inputHost;
    private readonly IPlatformEnvironmentHostCapability? _environmentHost;
    private readonly ISemanticsHostCapability? _semanticsHost;
    private readonly AsyncLocal<DorotiSceneBuildToken?> _activeBuildToken = new();
    private readonly AsyncLocal<DorotiFrameTransaction?> _activeFrameTransaction = new();
    private bool _disposed;

    internal DorotiView(PlatformDispatcher dispatcher, ulong viewId, DorotiViewCapabilities capabilities)
    {
        _dispatcher = dispatcher;
        this.viewId = viewId;
        _capabilities = capabilities;
        _viewHost = capabilities.Require<IViewHostCapability>(
            viewId,
            DorotiCapabilityIds.ViewLifecycleMetrics,
            DartUiInvocation.Managed("dart:ui#DorotiView"));
        _viewHost.MetricsChanged += HandleMetricsChanged;
        _viewHost.LifecycleChanged += HandleLifecycleChanged;
        _viewHost.CloseRequested += HandleCloseRequested;
        _viewHost.Closed += HandleClosed;
        if (capabilities.RegisteredIds.Contains(DorotiCapabilityIds.InputEvents, StringComparer.Ordinal))
        {
            _inputHost = capabilities.Require<IInputHostCapability>(
                viewId,
                DorotiCapabilityIds.InputEvents,
                DartUiInvocation.Managed("dart:ui#PointerDataPacket"));
            _inputHost.PointerData += HandlePointerData;
            _inputHost.KeyData += HandleKeyData;
            _inputHost.FocusData += HandleFocusData;
        }
        if (capabilities.RegisteredIds.Contains(DorotiCapabilityIds.PlatformEnvironment, StringComparer.Ordinal))
        {
            _environmentHost = capabilities.Require<IPlatformEnvironmentHostCapability>(
                viewId,
                DorotiCapabilityIds.PlatformEnvironment,
                DartUiInvocation.Managed("dart:ui#PlatformConfiguration"));
            _environmentHost.ConfigurationChanged += HandlePlatformConfigurationChanged;
        }
        if (capabilities.RegisteredIds.Contains(DorotiCapabilityIds.AccessibilitySemantics, StringComparer.Ordinal))
        {
            _semanticsHost = capabilities.Require<ISemanticsHostCapability>(
                viewId,
                DorotiCapabilityIds.AccessibilitySemantics,
                DartUiInvocation.Managed("dart:ui#SemanticsUpdate"));
            _semanticsHost.Action += HandleSemanticsAction;
        }
    }

    public ulong viewId { get; }
    internal DorotiFrameTrace FrameTrace => _dispatcher.frameTrace;

    public string targetIdentity => _capabilities.TargetIdentity;

    public IReadOnlyCollection<string> registeredCapabilityIds => _capabilities.RegisteredIds;

    /// <summary>Enters this view's platform environment for an externally initiated framework callback.</summary>
    public IDisposable EnterPlatformEnvironmentScope()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_environmentHost is null)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformEnvironment,
                viewId,
                DartUiInvocation.Managed("dart:ui#DorotiView.EnterPlatformEnvironmentScope"),
                "the active host did not register it",
                targetIdentity);
        }
        return PlatformEnvironmentContext.Enter(_environmentHost.Configuration);
    }

    internal void requestFocusChange(ViewFocusState state, ViewFocusDirection direction)
    {
        if (_inputHost is IViewFocusRequestCapability capability)
        {
            capability.RequestFocus(state, direction);
            return;
        }
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.InputEvents,
            viewId,
            DartUiInvocation.Managed("dart:ui#PlatformDispatcher.requestViewFocusChange"),
            "the active input host does not support outbound view-focus requests");
    }

    public double devicePixelRatio => metrics.devicePixelRatio;

    public Size physicalSize => metrics.physicalSize;

    public ViewConstraints physicalConstraints => ViewConstraints.tight(metrics.physicalSize);

    public ViewPadding padding => metrics.viewPadding;
    public ViewPadding viewPadding => metrics.viewPadding;
    public ViewPadding viewInsets => metrics.viewInsets;
    public ViewPadding systemGestureInsets => metrics.systemGestureInsets;

    public IReadOnlyList<DisplayFeature> displayFeatures { get; set; } = Array.Empty<DisplayFeature>();

    public DisplayCornerRadii? displayCornerRadii { get; set; }

    internal bool semanticsEnabled => _semanticsHost is not null;

    public PlatformDispatcher platformDispatcher => _dispatcher;

    public GestureSettings gestureSettings { get; set; } = new();

    public ViewMetrics metrics
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _viewHost.Metrics;
        }
    }

    internal DorotiViewEpoch CaptureViewEpoch()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _viewHost.ViewEpoch;
    }

    internal IDisposable EnterSceneBuildScope(
        DorotiViewEpoch epoch,
        long frameNumber,
        DorotiFrameTransaction? transaction = null)
    {
        ArgumentNullException.ThrowIfNull(epoch);
        if (epoch.ViewId != viewId)
            throw new InvalidOperationException(
                $"View epoch {epoch.ViewId} cannot build a scene for view {viewId}.");
        var previous = _activeBuildToken.Value;
        var previousTransaction = _activeFrameTransaction.Value;
        _activeBuildToken.Value = new(epoch, frameNumber, 0, 0);
        _activeFrameTransaction.Value = transaction;
        return new SceneBuildScope(this, previous, previousTransaction);
    }

    public PlatformConfiguration platformConfiguration => _environmentHost?.Configuration ??
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.PlatformEnvironment,
            viewId,
            DartUiInvocation.Managed("dart:ui#PlatformConfiguration"),
            "the active host did not register it");

    internal PlatformConfiguration? environmentConfiguration => _environmentHost?.Configuration;

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _viewHost.Show();
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        }
        _viewHost.Resize(logicalSize);
    }

    public void ScheduleFrame(DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var frameHost = _capabilities.Require<IFrameHostCapability>(
            viewId,
            DorotiCapabilityIds.ViewFrameDispatch,
            invocation);
        _dispatcher.frameTrace.Record(DorotiFramePhase.scheduleFrame, viewId, DorotiFrameClock.Now,
            reason: invocation.ElementId);
        var requestedEpoch = CaptureViewEpoch();
        if (frameHost is ILatestMetricsFrameHostCapability latestMetricsFrameHost)
        {
            latestMetricsFrameHost.ScheduleFrame(
                requestedEpoch,
                (timestamp, admittedEpoch) =>
                    _dispatcher.DispatchFrame(this, timestamp, admittedEpoch));
        }
        else if (frameHost is IExactFrameHostCapability exactFrameHost)
        {
            exactFrameHost.ScheduleFrame(
                requestedEpoch,
                timestamp => _dispatcher.DispatchFrame(this, timestamp, requestedEpoch));
        }
        else
        {
            frameHost.ScheduleFrame(
                timestamp => _dispatcher.DispatchFrame(this, timestamp, requestedEpoch));
        }
    }

    /// <summary>
    /// Dispatches one opt-in exact frame request using the supplied immutable
    /// epoch. The caller may wait on the returned transaction while the
    /// renderer and presenter advance it to a terminal state.
    /// </summary>
    public DorotiFrameTransaction RequestExactFrame(
        DorotiViewEpoch requestedEpoch,
        string visibleTargetIdentity,
        TimeSpan? timestamp = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(requestedEpoch);
        if (requestedEpoch.ViewId != viewId)
            throw new InvalidOperationException(
                $"View epoch {requestedEpoch.ViewId} cannot request a frame for view {viewId}.");
        var target = new DorotiResizeEpoch(
            requestedEpoch.ResizeTargetGeneration,
            requestedEpoch.LogicalWidth,
            requestedEpoch.LogicalHeight,
            requestedEpoch.PhysicalWidth,
            requestedEpoch.PhysicalHeight,
            requestedEpoch.DeviceScaleX,
            requestedEpoch.DeviceScaleY,
            requestedEpoch.TimestampMicroseconds);
        var transaction = new DorotiFrameTransaction(
            _dispatcher.NextFrameTransactionId(), target, visibleTargetIdentity);
        try
        {
            _dispatcher.DispatchFrame(this, timestamp ?? DorotiFrameClock.Now, requestedEpoch, transaction);
        }
        catch (Exception exception)
        {
            transaction.TryComplete(DorotiFrameTerminal.failed,
                $"exact frame dispatch failed: {exception.Message}");
            throw;
        }
        return transaction;
    }

    public static Task<DorotiFrameTransactionSnapshot> WaitForExactFrameAsync(
        DorotiFrameTransaction transaction,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        return transaction.Completion.WaitAsync(timeout, cancellationToken);
    }

    public async ValueTask<ReadOnlyMemory<byte>?> SendPlatformMessageAsync(
        string channel,
        ReadOnlyMemory<byte>? data,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return await _capabilities.Require<IPlatformMessageHostCapability>(
                viewId,
                DorotiCapabilityIds.PlatformMessaging,
                invocation)
            .SendAsync(channel, data, cancellationToken)
            .ConfigureAwait(false);
    }

    public TCapability RequireCapability<TCapability>(string id, DartUiInvocation invocation)
        where TCapability : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _capabilities.Require<TCapability>(viewId, id, invocation);
    }

    public void SubmitScene(Scene scene, DartUiInvocation invocation)
        => SubmitScene(scene, null, invocation);

    private void SubmitScene(Scene scene, Size? rootPhysicalSize, DartUiInvocation invocation)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (scene.debugDisposed || scene.viewId != viewId)
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.GraphicsScene,
                viewId,
                invocation,
                scene.debugDisposed ? "the scene is disposed" : $"scene belongs to view {scene.viewId}");
        }
        var token = _activeBuildToken.Value;
        if (token is not null)
        {
            var width = rootPhysicalSize is null
                ? token.ViewEpoch.PhysicalWidth
                : ToPhysicalDimension(rootPhysicalSize.width, nameof(rootPhysicalSize));
            var height = rootPhysicalSize is null
                ? token.ViewEpoch.PhysicalHeight
                : ToPhysicalDimension(rootPhysicalSize.height, nameof(rootPhysicalSize));
            token = token.WithRootPhysicalSize(width, height);
        }
        _capabilities.Require<ISceneHostCapability>(viewId, DorotiCapabilityIds.GraphicsScene, invocation)
            .Submit(viewId, new(scene, token, _activeFrameTransaction.Value), invocation);
    }

    public void render(Scene scene) => SubmitScene(scene, DartUiInvocation.Managed("dart:ui#DorotiView.render"));

    public void render(Scene scene, Size size)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(size);
        SubmitScene(scene, size, DartUiInvocation.Managed("dart:ui#DorotiView.render"));
    }

    private static int ToPhysicalDimension(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value < 0)
            throw new ArgumentOutOfRangeException(parameterName);
        return checked((int)Math.Round(value));
    }

    public Paragraph LayoutParagraph(ParagraphRequest request, DartUiInvocation invocation) =>
        _capabilities.Require<IParagraphHostCapability>(viewId, DorotiCapabilityIds.GraphicsText, invocation)
            .Layout(request, invocation);

    public ValueTask<Image> DecodeImageAsync(
        ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default) =>
        _capabilities.Require<IImageHostCapability>(viewId, DorotiCapabilityIds.GraphicsImage, invocation)
            .DecodeAsync(bytes, invocation, cancellationToken);

    public void UpdateSemantics(SemanticsUpdate update, DartUiInvocation invocation) =>
        _capabilities.Require<ISemanticsHostCapability>(viewId, DorotiCapabilityIds.AccessibilitySemantics, invocation)
            .Update(update, invocation);

    internal void SetSemanticsTreeEnabled(bool enabled, DartUiInvocation invocation) =>
        _capabilities.Require<ISemanticsHostCapability>(viewId, DorotiCapabilityIds.AccessibilitySemantics, invocation)
            .SetEnabled(enabled, invocation);

    public void updateSemantics(SemanticsUpdate update) =>
        UpdateSemantics(update, DartUiInvocation.Managed("dart:ui#DorotiView.updateSemantics"));

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _dispatcher.Remove(this);
        DisposeCore(closeHost: true);
    }

    internal void DisposeFromDispatcher() => DisposeCore(closeHost: true);

    private void DisposeCore(bool closeHost)
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _viewHost.MetricsChanged -= HandleMetricsChanged;
        _viewHost.LifecycleChanged -= HandleLifecycleChanged;
        _viewHost.CloseRequested -= HandleCloseRequested;
        _viewHost.Closed -= HandleClosed;
        if (_inputHost is not null)
        {
            _inputHost.PointerData -= HandlePointerData;
            _inputHost.KeyData -= HandleKeyData;
            _inputHost.FocusData -= HandleFocusData;
        }
        if (_environmentHost is not null)
        {
            _environmentHost.ConfigurationChanged -= HandlePlatformConfigurationChanged;
        }
        if (_semanticsHost is not null)
        {
            _semanticsHost.Action -= HandleSemanticsAction;
        }
        if (closeHost)
        {
            _viewHost.Close();
        }
        _capabilities.Dispose();
    }

    private void HandleMetricsChanged(ViewMetrics metrics)
    {
        _ = metrics;
        if (!_disposed)
        {
            _dispatcher.DispatchMetrics(this);
        }
    }

    private void HandleLifecycleChanged(AppLifecycleState state)
    {
        if (!_disposed)
        {
            _dispatcher.DispatchLifecycle(this, state);
        }
    }

    private void HandleCloseRequested()
    {
        if (!_disposed)
        {
            _dispatcher.DispatchLifecycle(this, AppLifecycleState.detached);
        }
    }

    private void HandleClosed()
    {
        _dispatcher.Remove(this);
        DisposeCore(closeHost: false);
    }

    private void HandlePointerData(PointerDataPacket packet) => _dispatcher.DispatchPointer(this, packet);

    private void HandleKeyData(KeyData data) => _dispatcher.DispatchKey(this, data);

    private void HandleFocusData(RawFocusData data) => _dispatcher.DispatchFocus(this, data);

    private void HandlePlatformConfigurationChanged(PlatformConfiguration configuration) =>
        _dispatcher.DispatchPlatformConfiguration(this, configuration);

    private void HandleSemanticsAction(SemanticsActionEvent action) => _dispatcher.DispatchSemanticsAction(this, action);

    private sealed class SceneBuildScope(
        DorotiView owner,
        DorotiSceneBuildToken? previous,
        DorotiFrameTransaction? previousTransaction) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            owner._activeBuildToken.Value = previous;
            owner._activeFrameTransaction.Value = previousTransaction;
        }
    }
}

public sealed class SemanticsBinding
{
    public static SemanticsBinding instance { get; } = new();

    public bool disableAnimations => PlatformDispatcher.instance.accessibilityFeatures.disableAnimations;
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/binding.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public interface WidgetsBindingObserver
{
    public Future<bool> didPopRoute() => Future<bool>.value(false);
    public bool handleStartBackGesture(PredictiveBackEvent backEvent) => false;
    public void handleUpdateBackGestureProgress(PredictiveBackEvent backEvent) { }
    public void handleCommitBackGesture() { }
    public void handleCancelBackGesture() { }
    public void handleStatusBarTap() { }
    public Future<bool> didPushRoute(string route) => Future<bool>.value(false);
    public Future<bool> didPushRouteInformation(RouteInformation routeInformation)
    {
        DartUri uriLocal = routeInformation.uri;
        return didPushRoute(
            Dart_coreLibrary.decodeComponent(
                new DartUri(
                    path: (uriLocal.path.Length == 0) ? "/" : uriLocal.path,
                    queryParameters: !Enumerable.Any(uriLocal.queryParametersAll)
                        ? null
                        : uriLocal.queryParametersAll,
                    fragment: (uriLocal.fragment.Length == 0) ? null : uriLocal.fragment
                ).ToString()
            )
        );
    }
    public void didChangeMetrics() { }
    public void didChangeTextScaleFactor() { }
    public void didChangePlatformBrightness() { }
    public void didChangeLocales(List<Locale>? locales) { }
    public void didChangeAppLifecycleState(AppLifecycleState state) { }
    public void didChangeViewFocus(ViewFocusEvent @event) { }
    public async Future<AppExitResponse> didRequestAppExit()
    {
        return AppExitResponse.exit;
    }
    public void didHaveMemoryPressure() { }
    public void didChangeAccessibilityFeatures() { }
}

public interface WidgetsBinding
{
    public static WidgetsBinding? _instance = default;
    ValueNotifier<bool>? _debugShowWidgetInspectorOverrideNotifierObject { get; set; }
    ValueNotifier<bool>? _debugWidgetInspectorSelectionOnTapEnabledNotifierObject { get; set; }
    bool _debugExcludeRootWidgetInspector { get; set; }
    BuildOwner? _buildOwner { get; set; }
    PlatformMenuDelegate platformMenuDelegate { get; set; }
    List<WidgetsBindingObserver> _observers { get; }
    List<WidgetsBindingObserver> _backGestureObservers { get; }
    bool _needToReportFirstFrame { get; set; }
    Completer<object?> _firstFrameCompleter { get; }
    bool debugBuildingDirtyElements { get; set; }
    Element? _rootElement { get; set; }
    bool _readyToProduceFrames { get; set; }
    WindowingOwnerIo _windowingOwner { get; set; }

    public static WidgetsBinding instance
    {
        get => BindingBase.checkInstance(_instance);
    }
    public bool debugShowWidgetInspectorOverride { get; set; }
    public ValueNotifier<bool> debugShowWidgetInspectorOverrideNotifier { get; }
    public ValueNotifier<bool> debugWidgetInspectorSelectionOnTapEnabled { get; }
    public bool debugExcludeRootWidgetInspector { get; set; }
    public void resetInternalState();
    public void _debugAddStackFilters();
    public DartMap<string, List<DartMap<string, string>>> _formatEvaluationResult(
        List<ViolationIo> violations
    );
    public Future _forceRebuild();
    public BuildOwner? buildOwner { get; }
    public FocusManager focusManager { get; }
    public void addObserver(WidgetsBindingObserver observer);
    public bool removeObserver(WidgetsBindingObserver observer);
    public Future<AppExitResponse> handleRequestAppExit();
    public void handleMetricsChanged();
    public void handleTextScaleFactorChanged();
    public void handlePlatformBrightnessChanged();
    public void handleAccessibilityFeaturesChanged();
    public void handleLocaleChanged();
    public void dispatchLocalesChanged(List<Locale>? locales);
    public void dispatchAccessibilityFeaturesChanged();
    public Future _handleStatusBarActions(MethodCall call);
    public Future<bool> handlePopRoute();
    public bool _handleStartBackGesture(DartMap<string, object?> arguments);
    public void _handleUpdateBackGestureProgress(DartMap<string, object?> arguments);
    public Future _handleCommitBackGesture();
    public void _handleCancelBackGesture();
    public Future<bool> handlePushRoute(string route);
    public Future<bool> _handlePushRouteInformation(DartMap<object, object?> routeArguments);
    public Future<bool> _handleNavigationInvocation(MethodCall methodCall);
    public Future<object?> _handleBackGestureInvocation(MethodCall methodCall);
    public void handleAppLifecycleStateChanged(AppLifecycleState state);
    public void handleViewFocusChanged(ViewFocusEvent @event);
    public void handleMemoryPressure();
    public bool firstFrameRasterized { get; }
    public Future waitUntilFirstFrameRasterized { get; }
    public bool debugDidSendFirstFrameEvent { get; }
    public void _handleBuildScheduled();
    public void drawFrame();
    public Element? rootElement { get; }
    public Element? renderViewElement { get; }
    public bool framesEnabled { get; }
    public Widget wrapWithDefaultView(Widget rootWidget);
    public void scheduleAttachRootWidget(Widget rootWidget);
    public void attachRootWidget(Widget rootWidget);
    public void attachToBuildOwner(RootWidget widget);
    public bool isRootWidgetAttached { get; }
    public Locale? computePlatformResolvedLocale(List<Locale> supportedLocales);
    public WindowingOwnerIo windowingOwner { get; set; }
    PlatformDispatcher platformDispatcher { get; }
    AppLifecycleState? lifecycleState { get; }
    bool debugCheckZone(string entryPoint);
    void addPostFrameCallback(Action<Duration> callback, string debugLabel = "callback");
    void scheduleWarmUpFrame();
    IEnumerable<RenderView> renderViews { get; }
    void hitTestInView(HitTestResult result, Offset position, long viewId);
    DorotiView window =>
        platformDispatcher.implicitView
        ?? throw new InvalidOperationException(
            "WidgetsBinding.window requires exactly one Flutter view."
        );
    Future endOfFrame => Scheduler.SchedulerBinding.instance.endOfFrame;
    void cancelPointer(long pointer) => GestureBinding.instance.cancelPointer(pointer);
}

public static partial class BindingLibrary
{
    public static void runApp(Widget app)
    {
        WidgetsBinding binding = WidgetsFlutterBinding.ensureInitialized();
        _runWidget(binding.wrapWithDefaultView(app), binding, "runApp");
    }
}

public static partial class BindingLibrary
{
    public static void runWidget(Widget app)
    {
        WidgetsBinding binding = WidgetsFlutterBinding.ensureInitialized();
        _runWidget(app, binding, "runWidget");
    }
}

public static partial class BindingLibrary
{
    internal static void _runWidget(Widget app, WidgetsBinding binding, string debugEntryPoint)
    {
        DartRuntimePrimitives.Assert(() => binding.debugCheckZone(debugEntryPoint));
        DartRuntimePrimitives.Ignore(
            (
                (Func<WidgetsBinding>)(
                    () =>
                    {
                        var __cascade = binding;
                        __cascade.scheduleAttachRootWidget(app);
                        __cascade.scheduleWarmUpFrame();
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public static partial class BindingLibrary
{
    internal static string _debugDumpAppString()
    {
        var mode = Foundation.ConstantsLibrary.kDebugMode
            ? "DEBUG MODE"
            : (Foundation.ConstantsLibrary.kReleaseMode ? "RELEASE MODE" : "PROFILE MODE");
        var buffer = new StringBuffer();
        buffer.writeln($"{DartRuntimePrimitives.RuntimeType(WidgetsBinding.instance)} - {mode}");
        if (WidgetsBinding.instance.rootElement is not null)
        {
            buffer.writeln(WidgetsBinding.instance.rootElement!.toStringDeep());
        }
        else
        {
            buffer.writeln("<no tree currently mounted>");
        }
        return buffer.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpApp()
    {
        PrintLibrary.debugPrint(_debugDumpAppString());
    }
}

public class RootWidget : Widget
{
    public virtual Widget? child { get; private set; }
    public virtual string? debugShortDescription { get; private set; }

    public RootWidget(Key? key = null, Widget? child = null, string? debugShortDescription = null)
        : base(key: key)
    {
        this.child = child;
        this.debugShortDescription = debugShortDescription;
    }

    public override RootElement createElement() => new RootElement(this);

    public virtual RootElement attach(BuildOwner owner, RootElement? element = null)
    {
        if (element is null)
        {
            owner.lockState(() =>
            {
                element = createElement();
                DartRuntimePrimitives.Assert(() => element is not null);
                element!.assignOwner(owner);
            });
            owner.buildScope(
                element!,
                () =>
                {
                    element!.mount(null, null);
                }
            );
        }
        else
        {
            element._newWidget = this;
            element.markNeedsBuild();
        }
        return element!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() =>
        DartRuntimePrimitives.ConvertValue<string>(debugShortDescription ?? base.toStringShort());
}

public class RootElement : Element, RootElementMixin
{
    internal virtual Element? _child { get; set; } = default;
    internal virtual RootWidget? _newWidget { get; set; } = default;

    public RootElement(RootWidget widget)
        : base(widget) { }

    public override void visitChildren(Action<Element> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, _child));
        _child = null;
        base.forgetChild(child);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => newSlot is null);
        base.mount(parent, newSlot);
        _rebuild();
        DartRuntimePrimitives.Assert(() => _child is not null);
        base.performRebuild();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (RootWidget)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        _rebuild();
    }

    public override void performRebuild()
    {
        if (_newWidget is not null)
        {
            RootWidget newWidget = _newWidget!;
            _newWidget = null;
            update(newWidget);
        }
        base.performRebuild();
        DartRuntimePrimitives.Assert(() => _newWidget is null);
    }

    internal virtual void _rebuild()
    {
        try
        {
            _child = updateChild(_child, ((RootWidget)widget).child, null);
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            var details = new FlutterErrorDetails(
                exception: exceptionLocal,
                stack: stackLocal,
                library: "widgets library",
                context: new ErrorDescription("attaching to the render tree")
            );
            FlutterError.reportError(details);
            _child = null;
        }
    }

    public override bool debugDoingBuild => false;

    public override bool debugExpectsRenderObjectForSlot(object? slot) => false;

    public virtual void assignOwner(BuildOwner owner)
    {
        _owner = owner;
        _parentBuildScope = new BuildScope();
    }
}

public class WidgetsFlutterBinding
    : GestureBinding,
        PaintingBinding,
        Framework.Semantics.SemanticsBinding,
        RendererBinding,
        WidgetsBinding
{
    private static readonly TimeSpan ActiveScrollSemanticsPollInterval = TimeSpan.FromMilliseconds(
        50
    );
    private readonly object _semanticsFlushGate = new();
    private Timer? _deferredSemanticsFlush;
    private bool _metricsSemanticsFramePumpArmed;

    public WidgetsFlutterBinding(PlatformDispatcher? platformDispatcher = null)
        : base(platformDispatcher) { }

    public virtual ImageCache _imageCache { get; set; } = default!;
    public virtual _SystemFontsNotifier__binding _systemFonts { get; set; } =
        new _SystemFontsNotifier__binding();
    private bool __late__semanticsEnabled_initialized;
    private ValueNotifier<bool> __late__semanticsEnabled = default!;
    public virtual ValueNotifier<bool> _semanticsEnabled
    {
        get
        {
            if (!__late__semanticsEnabled_initialized)
            {
                __late__semanticsEnabled = new ValueNotifier<bool>(
                    platformDispatcher.semanticsEnabled
                );
                __late__semanticsEnabled_initialized = true;
            }
            return __late__semanticsEnabled;
        }
    }
    public virtual ObserverList<
        Action<SemanticsActionEvent>
    > _semanticsActionListeners { get; set; } = new ObserverList<Action<SemanticsActionEvent>>();
    public virtual long _outstandingHandles { get; set; } = 0L;
    public virtual SemanticsHandle? _semanticsHandle { get; set; } = default;
    public virtual AccessibilityFeatures _accessibilityFeatures { get; set; } = default!;
    private bool __late__manifold_initialized;
    private PipelineManifold __late__manifold = default!;
    public virtual PipelineManifold _manifold
    {
        get
        {
            if (!__late__manifold_initialized)
            {
                __late__manifold = new _BindingPipelineManifold__binding(this);
                __late__manifold_initialized = true;
            }
            return __late__manifold;
        }
    }
    public virtual MouseTracker? _mouseTracker { get; set; } = default;
    private bool __late_pipelineOwner_initialized;
    private PipelineOwner __late_pipelineOwner = default!;
    public virtual PipelineOwner pipelineOwner
    {
        get
        {
            if (!__late_pipelineOwner_initialized)
            {
                __late_pipelineOwner = new PipelineOwner(
                    onSemanticsOwnerCreated: () =>
                    {
                        ((RenderView?)pipelineOwner.rootNode)!?.scheduleInitialSemantics();
                    },
                    onSemanticsUpdate: (update) =>
                    {
                        ((RenderView?)pipelineOwner.rootNode)!?.updateSemantics(update);
                    },
                    onSemanticsOwnerDisposed: () =>
                    {
                        ((RenderView?)pipelineOwner.rootNode)!?.clearSemantics();
                    }
                );
                __late_pipelineOwner_initialized = true;
            }
            return __late_pipelineOwner;
        }
    }
    private bool __late_renderView_initialized;
    private RenderView __late_renderView = default!;
    public virtual RenderView renderView
    {
        get
        {
            if (!__late_renderView_initialized)
            {
                __late_renderView = new _ReusableRenderView__binding(
                    view: platformDispatcher.implicitView!
                );
                __late_renderView_initialized = true;
            }
            return __late_renderView;
        }
    }
    public virtual PipelineOwner _rootPipelineOwner { get; set; } = default!;
    public virtual DartMap<object, RenderView> _viewIdToRenderView { get; set; } =
        new DartMap<object, RenderView>();
    public virtual bool _debugMouseTrackerUpdateScheduled { get; set; } = false;
    public virtual long _firstFrameDeferredCount { get; set; } = 0L;
    public virtual bool _firstFrameSent { get; set; } = false;
    public virtual ValueNotifier<bool>? _debugShowWidgetInspectorOverrideNotifierObject { get; set; } =
        default;
    public virtual ValueNotifier<bool>? _debugWidgetInspectorSelectionOnTapEnabledNotifierObject { get; set; } =
        default;
    public virtual bool _debugExcludeRootWidgetInspector { get; set; } = false;
    public virtual BuildOwner? _buildOwner { get; set; } = default;
    public virtual PlatformMenuDelegate platformMenuDelegate { get; set; } = default!;
    public virtual List<WidgetsBindingObserver> _observers { get; set; } =
        new List<WidgetsBindingObserver>();
    public virtual List<WidgetsBindingObserver> _backGestureObservers { get; set; } =
        new List<WidgetsBindingObserver>();
    public virtual bool _needToReportFirstFrame { get; set; } = true;
    public virtual Completer<object?> _firstFrameCompleter { get; set; } = new Completer<object?>();
    public virtual bool debugBuildingDirtyElements { get; set; } = false;
    public virtual Element? _rootElement { get; set; } = default;
    public virtual bool _readyToProduceFrames { get; set; } = false;
    public virtual WindowingOwnerIo _windowingOwner { get; set; } = default!;

    public static WidgetsBinding ensureInitialized()
    {
        if (WidgetsBinding._instance is null)
        {
            new WidgetsFlutterBinding();
        }
        return WidgetsBinding.instance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    protected override void initInstances()
    {
        base.initInstances();
        PaintingBinding._instance = this;
        Framework.Semantics.SemanticsBinding._instance = this;
        RendererBinding._instance = this;
        WidgetsBinding._instance = this;
        _accessibilityFeatures = platformDispatcher.accessibilityFeatures;
        platformDispatcher.onSemanticsEnabledChanged = _handleSemanticsEnabledChanged;
        platformDispatcher.onSemanticsActionEvent = _handleSemanticsActionEvent;
        platformDispatcher.onAccessibilityFeaturesChanged = () =>
        {
            if (
                Scheduler.SchedulerBinding.instance.schedulerPhase
                == Scheduler.SchedulerPhase.persistentCallbacks
            )
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    _ => handleAccessibilityFeaturesChanged(),
                    debugLabel: "SemanticsBinding.handleAccessibilityFeaturesChanged"
                );
            }
            else
            {
                handleAccessibilityFeaturesChanged();
            }
        };
        _handleSemanticsEnabledChanged();
        addSemanticsEnabledListener(_handleFrameworkSemanticsEnabledChanged);
        if (semanticsEnabled)
        {
            _handleFrameworkSemanticsEnabledChanged();
        }
        _buildOwner = new BuildOwner();
        _buildOwner.onBuildScheduled = _handleBuildScheduled;
        _imageCache = createImageCache();
        DartRuntimePrimitives.Ignore(PaintingBinding.shaderWarmUp?.execute());
        _rootPipelineOwner = createRootPipelineOwner();
        platformDispatcher.onMetricsChanged = _ => handleMetricsChanged();
        platformDispatcher.onLocaleChanged = handleLocaleChanged;
        platformDispatcher.onTextScaleFactorChanged = handleTextScaleFactorChanged;
        platformDispatcher.onPlatformBrightnessChanged = handlePlatformBrightnessChanged;
        addPersistentFrameCallback(_handlePersistentFrameCallback);
        initMouseTracker();
        rootPipelineOwner.attach(_manifold);
    }

    public virtual ImageCache imageCache => _imageCache;

    public virtual ImageCache createImageCache() => new ImageCache();

    public virtual Future<Codec> instantiateImageCodecFromBuffer(
        ImmutableBuffer buffer,
        long? cacheWidth = null,
        long? cacheHeight = null,
        bool allowUpscaling = false
    )
    {
        DartRuntimePrimitives.Assert(() => (cacheWidth is null) || (cacheWidth > 0L));
        DartRuntimePrimitives.Assert(() => (cacheHeight is null) || (cacheHeight > 0L));
        return Dart_uiLibrary.instantiateImageCodecFromBuffer(
            buffer,
            targetWidth: cacheWidth,
            targetHeight: cacheHeight,
            allowUpscaling: allowUpscaling
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<Codec> instantiateImageCodecWithSize(
        ImmutableBuffer buffer,
        Func<long, long, TargetImageSize>? getTargetSize = null
    )
    {
        return Dart_uiLibrary.instantiateImageCodecWithSize(buffer, getTargetSize: getTargetSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void evict(string asset)
    {
        base.evict(asset);
        imageCache.clear();
        imageCache.clearLiveImages();
    }

    public override void handleMemoryPressure()
    {
        base.handleMemoryPressure();
        imageCache.clear();
    }

    public virtual Listenable systemFonts =>
        DartRuntimePrimitives.ConvertValue<Listenable>(_systemFonts);

    public override async Future handleSystemMessage(object systemMessage)
    {
        await base.handleSystemMessage(systemMessage);
        var message = DartRuntimePrimitives.ConvertMap<string, object>(
            (System.Collections.IDictionary)systemMessage
        );
        var @type = ((string?)message.GetValueOrDefault("type"))!;
        switch (@type)
        {
            case "fontsChange":
            {
                _systemFonts.notifyListeners();
                break;
            }
        }
        return;
    }

    public virtual bool semanticsEnabled
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                _semanticsEnabled.value == (_outstandingHandles > 0L)
            );
            return _semanticsEnabled.value;
        }
    }

    public virtual void addSemanticsEnabledListener(Action listener)
    {
        _semanticsEnabled.addListener(listener);
    }

    public virtual void removeSemanticsEnabledListener(Action listener)
    {
        _semanticsEnabled.removeListener(listener);
    }

    public virtual void addSemanticsActionListener(Action<SemanticsActionEvent> listener)
    {
        _semanticsActionListeners.add(listener);
    }

    public virtual void removeSemanticsActionListener(Action<SemanticsActionEvent> listener)
    {
        _semanticsActionListeners.remove(listener);
    }

    public virtual Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId) =>
        DartRuntimePrimitives.ConvertValue<Rect>(null);

    public virtual long debugOutstandingSemanticsHandles => _outstandingHandles;

    public virtual SemanticsHandle ensureSemantics()
    {
        DartRuntimePrimitives.Assert(() => _outstandingHandles >= 0L);
        _outstandingHandles++;
        DartRuntimePrimitives.Assert(() => _outstandingHandles > 0L);
        _semanticsEnabled.value = true;
        return new SemanticsHandle(() => _didDisposeSemanticsHandle());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _didDisposeSemanticsHandle()
    {
        DartRuntimePrimitives.Assert(() => _outstandingHandles > 0L);
        _outstandingHandles--;
        DartRuntimePrimitives.Assert(() => _outstandingHandles >= 0L);
        _semanticsEnabled.value = _outstandingHandles > 0L;
    }

    public virtual void _handleSemanticsEnabledChanged()
    {
        if (platformDispatcher.semanticsEnabled)
        {
            _semanticsHandle ??= ensureSemantics();
        }
        else
        {
            _semanticsHandle?.dispose();
            _semanticsHandle = null;
        }
    }

    public virtual void _handleSemanticsActionEvent(SemanticsActionEvent action)
    {
        object? argumentsLocal = action.arguments;
        SemanticsActionEvent decodedAction =
            (argumentsLocal is ByteData)
                ? action.copyWith(
                    arguments: new StandardMessageCodec().decodeMessage((ByteData)argumentsLocal)
                )
                : action;
        List<Action<SemanticsActionEvent>> localListeners = _semanticsActionListeners.toList();
        foreach (var listener in localListeners)
        {
            if (_semanticsActionListeners.contains(listener))
            {
                listener(decodedAction);
            }
        }
        performSemanticsAction(decodedAction);
    }

    public virtual void _handleFrameworkSemanticsEnabledChanged()
    {
        platformDispatcher.setSemanticsTreeEnabled(semanticsEnabled);
    }

    public virtual AccessibilityFeatures accessibilityFeatures =>
        DartRuntimePrimitives.ConvertValue<AccessibilityFeatures>(_accessibilityFeatures);

    public virtual void handleAccessibilityFeaturesChanged()
    {
        _accessibilityFeatures = platformDispatcher.accessibilityFeatures;
        foreach (var observer in new List<WidgetsBindingObserver>(_observers))
        {
            try
            {
                observer.didChangeAccessibilityFeatures();
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exception,
                        stack: stack,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangeAccessibilityFeatures"
                        )
                    )
                );
            }
        }
    }

    public virtual SemanticsUpdateBuilder createSemanticsUpdateBuilder()
    {
        return new SemanticsUpdateBuilder();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool disableAnimations
    {
        get
        {
            bool value = _accessibilityFeatures.disableAnimations;
            DartRuntimePrimitives.Assert(() =>
            {
                if (Framework.Semantics.DebugLibrary.debugSemanticsDisableAnimations is not null)
                {
                    value = (
                        Framework.Semantics.DebugLibrary.debugSemanticsDisableAnimations
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    );
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            return value;
        }
    }

    protected override void initServiceExtensions()
    {
        base.initServiceExtensions();
        DartRuntimePrimitives.Assert(() =>
        {
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.invertOversizedImages.ToString(),
                getter: async () => Painting.DebugLibrary.debugInvertOversizedImages,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Painting.DebugLibrary.debugInvertOversizedImages != value)
                        {
                            Painting.DebugLibrary.debugInvertOversizedImages = value;
                            DartAsyncRuntime.unawaited(_forceRepaint());
                        }
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.debugPaint.ToString(),
                getter: async () => Rendering.DebugLibrary.debugPaintSizeEnabled,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugPaintSizeEnabled == value)
                        {
                            return;
                        }
                        Rendering.DebugLibrary.debugPaintSizeEnabled = value;
                        DartAsyncRuntime.unawaited(_forceRepaint());
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.debugPaintBaselinesEnabled.ToString(),
                getter: async () => Rendering.DebugLibrary.debugPaintBaselinesEnabled,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugPaintBaselinesEnabled == value)
                        {
                            return;
                        }
                        Rendering.DebugLibrary.debugPaintBaselinesEnabled = value;
                        DartAsyncRuntime.unawaited(_forceRepaint());
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.repaintRainbow.ToString(),
                getter: async () => Rendering.DebugLibrary.debugRepaintRainbowEnabled,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        bool repaint = Rendering.DebugLibrary.debugRepaintRainbowEnabled && !value;
                        Rendering.DebugLibrary.debugRepaintRainbowEnabled = value;
                        if (repaint)
                        {
                            DartAsyncRuntime.unawaited(_forceRepaint());
                        }
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerServiceExtension(
                name: RenderingServiceExtensions.debugDumpLayerTree.ToString(),
                callback: async (parameters) =>
                {
                    return new DartMap<string, object>
                    {
                        ["data"] = Rendering.BindingLibrary._debugCollectLayerTrees(),
                    };
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.debugDisableClipLayers.ToString(),
                getter: async () => Rendering.DebugLibrary.debugDisableClipLayers,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugDisableClipLayers == value)
                        {
                            return;
                        }
                        Rendering.DebugLibrary.debugDisableClipLayers = value;
                        DartAsyncRuntime.unawaited(_forceRepaint());
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.debugDisablePhysicalShapeLayers.ToString(),
                getter: async () => Rendering.DebugLibrary.debugDisablePhysicalShapeLayers,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugDisablePhysicalShapeLayers == value)
                        {
                            return;
                        }
                        Rendering.DebugLibrary.debugDisablePhysicalShapeLayers = value;
                        DartAsyncRuntime.unawaited(_forceRepaint());
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.debugDisableOpacityLayers.ToString(),
                getter: async () => Rendering.DebugLibrary.debugDisableOpacityLayers,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugDisableOpacityLayers == value)
                        {
                            return;
                        }
                        Rendering.DebugLibrary.debugDisableOpacityLayers = value;
                        DartAsyncRuntime.unawaited(_forceRepaint());
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            registerServiceExtension(
                name: RenderingServiceExtensions.debugDumpRenderTree.ToString(),
                callback: async (parameters) =>
                {
                    return new DartMap<string, object>
                    {
                        ["data"] = Rendering.BindingLibrary._debugCollectRenderTrees(),
                    };
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            registerServiceExtension(
                name: RenderingServiceExtensions.debugDumpSemanticsTreeInTraversalOrder.ToString(),
                callback: async (parameters) =>
                {
                    return new DartMap<string, object>
                    {
                        ["data"] = Rendering.BindingLibrary._debugCollectSemanticsTrees(
                            DebugSemanticsDumpOrder.traversalOrder
                        ),
                    };
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            registerServiceExtension(
                name: RenderingServiceExtensions.debugDumpSemanticsTreeInInverseHitTestOrder.ToString(),
                callback: async (parameters) =>
                {
                    return new DartMap<string, object>
                    {
                        ["data"] = Rendering.BindingLibrary._debugCollectSemanticsTrees(
                            DebugSemanticsDumpOrder.inverseHitTest
                        ),
                    };
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.profileRenderObjectPaints.ToString(),
                getter: async () => Rendering.DebugLibrary.debugProfilePaintsEnabled,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugProfilePaintsEnabled != value)
                        {
                            Rendering.DebugLibrary.debugProfilePaintsEnabled = value;
                        }
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
            registerBoolServiceExtension(
                name: RenderingServiceExtensions.profileRenderObjectLayouts.ToString(),
                getter: async () => Rendering.DebugLibrary.debugProfileLayoutsEnabled,
                setter: (Func<bool, Future>)(
                    async (value) =>
                    {
                        if (Rendering.DebugLibrary.debugProfileLayoutsEnabled != value)
                        {
                            Rendering.DebugLibrary.debugProfileLayoutsEnabled = value;
                        }
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
        }
    }

    public virtual MouseTracker mouseTracker =>
        DartRuntimePrimitives.ConvertValue<MouseTracker>(_mouseTracker!);

    public virtual PipelineOwner createRootPipelineOwner()
    {
        return new _DefaultRootPipelineOwner__binding();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PipelineOwner rootPipelineOwner => _rootPipelineOwner;
    public virtual IEnumerable<RenderView> renderViews => _viewIdToRenderView.Values;

    public virtual void addRenderView(RenderView view)
    {
        object viewIdLocal = checked((long)view.flutterView.viewId);
        DartRuntimePrimitives.Assert(() => !_viewIdToRenderView.containsValue(view));
        DartRuntimePrimitives.Assert(() => !_viewIdToRenderView.ContainsKey(viewIdLocal));
        _viewIdToRenderView[viewIdLocal] = view;
        view.configuration = createViewConfigurationFor(view);
    }

    public virtual void removeRenderView(RenderView view)
    {
        object viewIdLocal = checked((long)view.flutterView.viewId);
        DartRuntimePrimitives.Assert(() =>
            Equals(_viewIdToRenderView.GetValueOrDefault(viewIdLocal), view)
        );
        _viewIdToRenderView.remove(viewIdLocal);
    }

    public virtual Rendering.ViewConfiguration createViewConfigurationFor(RenderView renderView)
    {
        return Rendering.ViewConfiguration.CreateFromView(renderView.flutterView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SceneBuilder createSceneBuilder() =>
        new SceneBuilder(
            platformDispatcher.implicitView?.viewId
                ?? throw new InvalidOperationException(
                    "Rendering a scene requires an implicit Flutter view."
                )
        );

    public virtual PictureRecorder createPictureRecorder() =>
        DartRuntimePrimitives.ConvertValue<PictureRecorder>(new PictureRecorder());

    public virtual Canvas createCanvas(PictureRecorder recorder) =>
        DartRuntimePrimitives.ConvertValue<Canvas>(new Canvas(recorder));

    public virtual void handleMetricsChanged()
    {
        var forceFrame = false;
        foreach (RenderView view in renderViews)
        {
            forceFrame = forceFrame || view.child is not null;
            view.configuration = createViewConfigurationFor(view);
        }
        if (forceFrame)
        {
            scheduleForcedFrame();
        }
        foreach (var observer in new List<WidgetsBindingObserver>(_observers))
        {
            try
            {
                observer.didChangeMetrics();
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exception,
                        stack: stack,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangeMetrics"
                        )
                    )
                );
            }
        }
    }

    public virtual void handleTextScaleFactorChanged()
    {
        foreach (var observer in new List<WidgetsBindingObserver>(_observers))
        {
            try
            {
                observer.didChangeTextScaleFactor();
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exception,
                        stack: stack,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangeTextScaleFactor"
                        )
                    )
                );
            }
        }
    }

    public virtual void handlePlatformBrightnessChanged()
    {
        foreach (var observer in new List<WidgetsBindingObserver>(_observers))
        {
            try
            {
                observer.didChangePlatformBrightness();
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exception,
                        stack: stack,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangePlatformBrightness"
                        )
                    )
                );
            }
        }
    }

    public virtual void initMouseTracker(MouseTracker? tracker = null)
    {
        _mouseTracker?.dispose();
        _mouseTracker =
            tracker
            ?? new MouseTracker(
                (position, viewId) =>
                {
                    var result = new HitTestResult();
                    hitTestInView(result, position, viewId);
                    return result;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
    }

    public override void dispatchEvent(PointerEvent @event, HitTestResult? result)
    {
        _mouseTracker!.updateWithEvent(
            @event,
            (@event is Gestures.PointerMoveEvent) ? null : result
        );
        base.dispatchEvent(@event, result);
    }

    public virtual void performSemanticsAction(SemanticsActionEvent action)
    {
        _viewIdToRenderView
            .GetValueOrDefault(checked((long)action.viewId))
            ?.owner?.semanticsOwner?.performAction(action.nodeId, action.type, action.arguments);
    }

    public virtual void _handleWebFirstFrame(Duration __unused0)
    {
        DartRuntimePrimitives.Assert(() => Foundation.ConstantsLibrary.kIsWeb);
        var methodChannel = new MethodChannel("flutter/service_worker");
        DartRuntimePrimitives.Ignore(
            methodChannel
                .invokeMethod<object?>("first-frame")
                .then(
                    (_) => { },
                    onError: (error, stack) =>
                    {
                        FlutterError.reportError(
                            new FlutterErrorDetails(
                                exception: error,
                                stack: stack,
                                library: "rendering library",
                                context: new ErrorDescription("while sending the first-frame event")
                            )
                        );
                    }
                )
        );
    }

    public virtual void _handlePersistentFrameCallback(Duration timeStamp)
    {
        drawFrame();
        _scheduleMouseTrackerUpdate();
    }

    public virtual void _scheduleMouseTrackerUpdate()
    {
        DartRuntimePrimitives.Assert(() => !_debugMouseTrackerUpdateScheduled);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugMouseTrackerUpdateScheduled = true;
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (duration) =>
            {
                DartRuntimePrimitives.Assert(() => _debugMouseTrackerUpdateScheduled);
                DartRuntimePrimitives.Assert(() =>
                {
                    _debugMouseTrackerUpdateScheduled = false;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
                _mouseTracker!.updateAllDevices();
            },
            debugLabel: "RendererBinding.mouseTrackerUpdate"
        );
    }

    public virtual bool sendFramesToEngine =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _firstFrameSent || (_firstFrameDeferredCount == 0L)
        );

    public virtual void deferFirstFrame()
    {
        DartRuntimePrimitives.Assert(() => _firstFrameDeferredCount >= 0L);
        _firstFrameDeferredCount += 1L;
    }

    public virtual void allowFirstFrame()
    {
        DartRuntimePrimitives.Assert(() => _firstFrameDeferredCount > 0L);
        _firstFrameDeferredCount -= 1L;
        if (!_firstFrameSent)
        {
            scheduleWarmUpFrame();
        }
    }

    public virtual void resetFirstFrameSent()
    {
        _firstFrameSent = false;
    }

    public virtual void drawFrame()
    {
        var frameViewId = platformDispatcher.implicitView?.viewId ?? 0;
        // A long lazy layout cannot make an ongoing wheel stream idle merely
        // because the UI thread has not had a chance to receive its next event.
        var scrollWasActive = platformDispatcher.frameTrace.HasActiveScrollActivity;
        debugBuildingDirtyElements = true;
        try
        {
            platformDispatcher.frameTrace.Record(
                DorotiFramePhase.build,
                frameViewId,
                DorotiFrameClock.Now
            );
            if (rootElement is not null)
            {
                buildOwner!.buildScope(rootElement);
            }
            platformDispatcher.frameTrace.Record(
                DorotiFramePhase.layout,
                frameViewId,
                DorotiFrameClock.Now
            );
            rootPipelineOwner.flushLayout();
            rootPipelineOwner.flushCompositingBits();
            platformDispatcher.frameTrace.Record(
                DorotiFramePhase.paint,
                frameViewId,
                DorotiFrameClock.Now
            );
            rootPipelineOwner.flushPaint();
            if (sendFramesToEngine)
            {
                platformDispatcher.frameTrace.Record(
                    DorotiFramePhase.sceneBuild,
                    frameViewId,
                    DorotiFrameClock.Now
                );
                foreach (RenderView renderView in renderViews)
                {
                    renderView.compositeFrame();
                }
                if (shouldFlushSemantics(frameViewId, scrollWasActive))
                {
                    platformDispatcher.frameTrace.Record(
                        DorotiFramePhase.semanticsBuild,
                        frameViewId,
                        DorotiFrameClock.Now
                    );
                    rootPipelineOwner.flushSemantics();
                    platformDispatcher.frameTrace.Record(
                        DorotiFramePhase.semanticsBuildEnd,
                        frameViewId,
                        DorotiFrameClock.Now
                    );
                }
                _firstFrameSent = true;
            }
            platformDispatcher.frameTrace.Record(
                DorotiFramePhase.finalizeTree,
                frameViewId,
                DorotiFrameClock.Now
            );
            buildOwner!.finalizeTree();
            platformDispatcher.frameTrace.Record(
                DorotiFramePhase.finalizeTreeEnd,
                frameViewId,
                DorotiFrameClock.Now
            );
        }
        finally
        {
            debugBuildingDirtyElements = false;
        }
        _needToReportFirstFrame = false;
    }

    private bool shouldFlushSemantics(ulong frameViewId, bool scrollWasActive)
    {
        if (!rootPipelineOwner.hasPendingSemanticsUpdate)
        {
            return false;
        }

        lock (_semanticsFlushGate)
        {
            var activeScroll =
                scrollWasActive || platformDispatcher.frameTrace.HasActiveScrollActivity;
            var activeMetrics =
                platformDispatcher.implicitView?.coalesceSemanticsGeometryDuringActiveMetrics
                    == true
                && platformDispatcher.frameTrace.HasActiveMetricsActivity;
            if (!activeScroll && !activeMetrics)
            {
                _deferredSemanticsFlush?.cancel();
                _deferredSemanticsFlush = null;
                return true;
            }

            if (activeScroll && _deferredSemanticsFlush is null)
            {
                platformDispatcher.frameTrace.Record(
                    DorotiFramePhase.semanticsDeferred,
                    frameViewId,
                    DorotiFrameClock.Now,
                    reason: "active scroll accessibility flush deferred until rest"
                );
                armDeferredSemanticsFlush();
            }
            if (activeMetrics)
            {
                platformDispatcher.frameTrace.Record(
                    DorotiFramePhase.semanticsDeferred,
                    frameViewId,
                    DorotiFrameClock.Now,
                    reason: "active metrics accessibility geometry flush deferred until rest"
                );
                armMetricsSemanticsFramePump();
            }
            return false;
        }
    }

    private void armMetricsSemanticsFramePump()
    {
        if (_metricsSemanticsFramePumpArmed)
        {
            return;
        }

        _metricsSemanticsFramePumpArmed = true;
        addPostFrameCallback(
            _ =>
            {
                var schedule = false;
                lock (_semanticsFlushGate)
                {
                    _metricsSemanticsFramePumpArmed = false;
                    schedule =
                        rootPipelineOwner.hasPendingSemanticsUpdate
                        && platformDispatcher.frameTrace.HasActiveMetricsActivity;
                    if (
                        !schedule
                        && rootPipelineOwner.hasPendingSemanticsUpdate
                        && !platformDispatcher.frameTrace.HasActiveScrollActivity
                    )
                    {
                        // One final frame crosses the quiet boundary and flushes
                        // the retained geometry. The browser/worker rAF owner, not
                        // a managed ThreadPool timer, drives this path.
                        schedule = true;
                    }
                }
                if (schedule)
                {
                    scheduleFrame();
                }
            },
            debugLabel: "RendererBinding.metricsSemanticsFramePump"
        );
    }

    private void armDeferredSemanticsFlush()
    {
        _deferredSemanticsFlush = new Timer(
            (Duration)ActiveScrollSemanticsPollInterval,
            () =>
            {
                var schedule = false;
                lock (_semanticsFlushGate)
                {
                    _deferredSemanticsFlush = null;
                    if (platformDispatcher.frameTrace.HasActiveScrollActivity)
                    {
                        // Polling only observes retained activity state. It does not
                        // request a visual frame or rebuild semantics while active.
                        armDeferredSemanticsFlush();
                        return;
                    }
                    schedule = rootPipelineOwner.hasPendingSemanticsUpdate;
                }
                if (schedule)
                {
                    scheduleFrame();
                }
            }
        );
    }

    protected override async Task performReassemble()
    {
        await base.performReassemble();
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("Preparing Hot Reload (layout)");
        }
        try
        {
            foreach (RenderView renderView in renderViews)
            {
                renderView.reassemble();
            }
        }
        finally
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
        scheduleWarmUpFrame();
        await endOfFrame;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void hitTestInView(HitTestResult result, Offset position, long viewId)
    {
        _viewIdToRenderView.GetValueOrDefault(viewId)?.hitTest(result, position: position);
        base.hitTestInView(result, position, viewId);
    }

    public virtual Future _forceRepaint()
    {
        Action<RenderObject> visitor = default!;
        visitor = (child) =>
        {
            child.markNeedsPaint();
            child.visitChildren(visitor);
        };
        foreach (RenderView renderView in renderViews)
        {
            renderView.visitChildren(visitor);
        }
        return endOfFrame;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugShowWidgetInspectorOverride
    {
        get { return debugShowWidgetInspectorOverrideNotifier.value; }
        set
        {
            var __value = value;
            debugShowWidgetInspectorOverrideNotifier.value = __value;
        }
    }
    public virtual ValueNotifier<bool> debugShowWidgetInspectorOverrideNotifier =>
        _debugShowWidgetInspectorOverrideNotifierObject ??= new ValueNotifier<bool>(false);
    public virtual ValueNotifier<bool> debugWidgetInspectorSelectionOnTapEnabled =>
        _debugWidgetInspectorSelectionOnTapEnabledNotifierObject ??= new ValueNotifier<bool>(true);
    public virtual bool debugExcludeRootWidgetInspector
    {
        get => _debugExcludeRootWidgetInspector;
        set
        {
            var __value = value;
            _debugExcludeRootWidgetInspector = __value;
        }
    }

    public override void resetInternalState()
    {
        base.resetInternalState();
        _debugShowWidgetInspectorOverrideNotifierObject?.dispose();
        _debugShowWidgetInspectorOverrideNotifierObject = null;
        _debugWidgetInspectorSelectionOnTapEnabledNotifierObject?.dispose();
        _debugWidgetInspectorSelectionOnTapEnabledNotifierObject = null;
    }

    public virtual void _debugAddStackFilters()
    {
        var elementInflateWidget = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "Element",
            method: "inflateWidget"
        );
        var elementUpdateChild = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "Element",
            method: "updateChild"
        );
        var elementRebuild = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "Element",
            method: "rebuild"
        );
        var componentElementPerformRebuild = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "ComponentElement",
            method: "performRebuild"
        );
        var componentElementFirstBuild = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "ComponentElement",
            method: "_firstBuild"
        );
        var componentElementMount = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "ComponentElement",
            method: "mount"
        );
        var statefulElementFirstBuild = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "StatefulElement",
            method: "_firstBuild"
        );
        var singleChildMount = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "SingleChildRenderObjectElement",
            method: "mount"
        );
        var statefulElementRebuild = new PartialStackFrame(
            package: "package:flutter/src/widgets/framework.dart",
            className: "StatefulElement",
            method: "performRebuild"
        );
        var replacementString = "...     Normal element mounting";
        FlutterError.addDefaultStackFilter(
            new RepetitiveStackFrameFilter(
                frames: new List<PartialStackFrame>
                {
                    elementInflateWidget,
                    elementUpdateChild,
                    componentElementPerformRebuild,
                    elementRebuild,
                    componentElementFirstBuild,
                    componentElementMount,
                },
                replacement: replacementString
            )
        );
        FlutterError.addDefaultStackFilter(
            new RepetitiveStackFrameFilter(
                frames: new List<PartialStackFrame>
                {
                    elementUpdateChild,
                    componentElementPerformRebuild,
                    elementRebuild,
                    componentElementFirstBuild,
                    componentElementMount,
                },
                replacement: replacementString
            )
        );
        FlutterError.addDefaultStackFilter(
            new RepetitiveStackFrameFilter(
                frames: new List<PartialStackFrame>
                {
                    elementInflateWidget,
                    elementUpdateChild,
                    componentElementPerformRebuild,
                    statefulElementRebuild,
                    elementRebuild,
                    componentElementFirstBuild,
                    statefulElementFirstBuild,
                    componentElementMount,
                },
                replacement: replacementString
            )
        );
        FlutterError.addDefaultStackFilter(
            new RepetitiveStackFrameFilter(
                frames: new List<PartialStackFrame>
                {
                    elementUpdateChild,
                    componentElementPerformRebuild,
                    statefulElementRebuild,
                    elementRebuild,
                    componentElementFirstBuild,
                    statefulElementFirstBuild,
                    componentElementMount,
                },
                replacement: replacementString
            )
        );
        FlutterError.addDefaultStackFilter(
            new RepetitiveStackFrameFilter(
                frames: new List<PartialStackFrame>
                {
                    elementInflateWidget,
                    elementUpdateChild,
                    singleChildMount,
                },
                replacement: replacementString
            )
        );
        FlutterError.addDefaultStackFilter(
            new RepetitiveStackFrameFilter(
                frames: new List<PartialStackFrame> { elementUpdateChild, singleChildMount },
                replacement: replacementString
            )
        );
    }

    public virtual DartMap<string, List<DartMap<string, string>>> _formatEvaluationResult(
        List<ViolationIo> violations
    )
    {
        return new DartMap<string, List<DartMap<string, string>>>
        {
            ["result"] = violations
                .map(
                    (violation) =>
                    {
                        return new DartMap<string, string>
                        {
                            ["nodeId"] = violation.node.id.ToString(),
                            ["message"] = violation.reason,
                        };
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
                .ToList(),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future _forceRebuild()
    {
        if (rootElement is not null)
        {
            buildOwner!.reassemble(rootElement!);
            return endOfFrame;
        }
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BuildOwner? buildOwner => _buildOwner;
    public virtual FocusManager focusManager => _buildOwner!.focusManager;

    public virtual void addObserver(WidgetsBindingObserver observer) => _observers.Add(observer);

    public virtual bool removeObserver(WidgetsBindingObserver observer)
    {
        _backGestureObservers.Remove(observer);
        return _observers.Remove(observer);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override async Future<AppExitResponse> handleRequestAppExit()
    {
        var didCancel = false;
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                if (Equals(await observer.didRequestAppExit(), AppExitResponse.cancel))
                {
                    didCancel = true;
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didRequestAppExit"
                        )
                    )
                );
            }
        }
        return didCancel ? AppExitResponse.cancel : AppExitResponse.exit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleLocaleChanged()
    {
        dispatchLocalesChanged(platformDispatcher.locales.ToList());
    }

    public virtual void dispatchLocalesChanged(List<Locale>? locales)
    {
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                observer.didChangeLocales(locales);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangeLocales"
                        )
                    )
                );
            }
        }
    }

    public virtual void dispatchAccessibilityFeaturesChanged()
    {
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            observer.didChangeAccessibilityFeatures();
        }
    }

    public virtual async Future _handleStatusBarActions(MethodCall call)
    {
        DartRuntimePrimitives.Assert(() => call.method == "handleScrollToTop");
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                observer.handleStatusBarTap();
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                var details = new FlutterErrorDetails(
                    exception: exceptionLocal,
                    stack: stackLocal,
                    library: "widgets library",
                    context: new ErrorDescription("handling status bar action")
                );
                FlutterError.reportError(details);
            }
        }
    }

    public virtual async Future<bool> handlePopRoute()
    {
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                if (await observer.didPopRoute())
                {
                    return true;
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didPopRoute"
                        )
                    )
                );
            }
        }
        DartRuntimePrimitives.Ignore(
            SystemNavigator
                .pop()
                .catchError(
                    (exception, stack) =>
                    {
                        FlutterError.reportError(
                            new FlutterErrorDetails(
                                exception: exception,
                                stack: stack,
                                library: "widgets library",
                                context: new ErrorDescription("while popping route")
                            )
                        );
                    }
                )
        );
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _handleStartBackGesture(DartMap<string, object?> arguments)
    {
        _backGestureObservers.Clear();
        var backEvent = PredictiveBackEvent.CreateFromMap(arguments);
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                if (observer.handleStartBackGesture(backEvent))
                {
                    _backGestureObservers.Add(observer);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.handleStartBackGesture"
                        )
                    )
                );
            }
        }
        return Enumerable.Any(_backGestureObservers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _handleUpdateBackGestureProgress(DartMap<string, object?> arguments)
    {
        if (!Enumerable.Any(_backGestureObservers))
        {
            return;
        }
        var backEvent = PredictiveBackEvent.CreateFromMap(arguments);
        foreach (WidgetsBindingObserver observer in _backGestureObservers)
        {
            try
            {
                observer.handleUpdateBackGestureProgress(backEvent);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.handleUpdateBackGestureProgress"
                        )
                    )
                );
            }
        }
    }

    public virtual async Future _handleCommitBackGesture()
    {
        if (!Enumerable.Any(_backGestureObservers))
        {
            await handlePopRoute();
            return;
        }
        foreach (WidgetsBindingObserver observer in _backGestureObservers)
        {
            try
            {
                observer.handleCommitBackGesture();
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.handleCommitBackGesture"
                        )
                    )
                );
            }
        }
    }

    public virtual void _handleCancelBackGesture()
    {
        foreach (WidgetsBindingObserver observer in _backGestureObservers)
        {
            try
            {
                observer.handleCancelBackGesture();
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.handleCancelBackGesture"
                        )
                    )
                );
            }
        }
    }

    public virtual async Future<bool> handlePushRoute(string route)
    {
        var routeInformation = new RouteInformation(uri: DartUri.parse(route));
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                if (await observer.didPushRouteInformation(routeInformation))
                {
                    return true;
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didPushRouteInformation"
                        )
                    )
                );
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual async Future<bool> _handlePushRouteInformation(
        DartMap<object, object?> routeArguments
    )
    {
        var routeInformation = new RouteInformation(
            uri: DartUri.parse(((string?)routeArguments.GetValueOrDefault("location"))!),
            state: routeArguments.GetValueOrDefault("state")!
        );
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                if (await observer.didPushRouteInformation(routeInformation))
                {
                    return true;
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didPushRouteInformation"
                        )
                    )
                );
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<bool> _handleNavigationInvocation(MethodCall methodCall)
    {
        return methodCall.method switch
        {
            "popRoute" => handlePopRoute(),
            "pushRoute" => handlePushRoute(
                methodCall.arguments as string
                    ?? throw new FormatException("pushRoute requires a route name.")
            ),
            "pushRouteInformation" => _handlePushRouteInformation(
                DartRuntimePrimitives.ConvertMap<object, object?>(
                    methodCall.arguments as System.Collections.IDictionary
                        ?? throw new FormatException("Route information requires a map.")
                )
            ),
            _ => Future<bool>.value(false),
        };
    }

    public virtual async Future<object?> _handleBackGestureInvocation(MethodCall methodCall)
    {
        var arguments = methodCall.arguments is null
            ? null
            : DartRuntimePrimitives.ConvertMap<string, object?>(
                methodCall.arguments as System.Collections.IDictionary
                    ?? throw new FormatException("Back gesture arguments require a map.")
            );
        switch (methodCall.method)
        {
            case "startBackGesture":
                return _handleStartBackGesture(
                    arguments ?? throw new FormatException("Start back gesture requires arguments.")
                );
            case "updateBackGestureProgress":
                _handleUpdateBackGestureProgress(
                    arguments
                        ?? throw new FormatException("Back gesture progress requires arguments.")
                );
                return null;
            case "commitBackGesture":
                await _handleCommitBackGesture();
                return null;
            case "cancelBackGesture":
                _handleCancelBackGesture();
                return null;
            default:
                throw new MissingPluginException();
        }
    }

    public override void handleAppLifecycleStateChanged(AppLifecycleState state)
    {
        base.handleAppLifecycleStateChanged(state);
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                observer.didChangeAppLifecycleState(state);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangeAppLifecycleState"
                        )
                    )
                );
            }
        }
    }

    public override void handleViewFocusChanged(ViewFocusEvent @event)
    {
        DartRuntimePrimitives.Noop();
        foreach (
            var observer in new List<WidgetsBindingObserver>(
                DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(_observers)
            )
        )
        {
            try
            {
                observer.didChangeViewFocus(@event);
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widgets library",
                        context: new ErrorDescription(
                            "while dispatching notifications for WidgetsBindingObserver.didChangeViewFocus"
                        )
                    )
                );
            }
        }
    }

    public virtual bool firstFrameRasterized => _firstFrameCompleter.isCompleted;
    public virtual Future waitUntilFirstFrameRasterized => _firstFrameCompleter.future;
    public virtual bool debugDidSendFirstFrameEvent => !_needToReportFirstFrame;

    public virtual void _handleBuildScheduled()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (debugBuildingDirtyElements)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary("Build scheduled during frame."),
                            new ErrorDescription(
                                "While the widget tree was being built, laid out, and painted, "
                                    + "a new frame was scheduled to rebuild the widget tree."
                            ),
                            new ErrorHint(
                                "This might be because setState() was called from a layout or "
                                    + "paint callback. "
                                    + "If a change is needed to the widget tree, it should be applied "
                                    + "as the tree is being built. Scheduling a change for the subsequent "
                                    + "frame instead results in an interface that lags behind by one frame. "
                                    + "If this was done to make your build dependent on a size measured at "
                                    + "layout time, consider using a LayoutBuilder, CustomSingleChildLayout, "
                                    + "or CustomMultiChildLayout. If, on the other hand, the one frame delay "
                                    + "is the desired effect, for example because this is an "
                                    + "animation, consider scheduling the frame in a post-frame callback "
                                    + "using SchedulerBinding.addPostFrameCallback or "
                                    + "using an AnimationController to trigger the animation."
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        ensureVisualUpdate();
    }

    public virtual Element? rootElement => _rootElement;
    public virtual Element? renderViewElement => rootElement;
    public override bool framesEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(base.framesEnabled && _readyToProduceFrames);

    public virtual Widget wrapWithDefaultView(Widget rootWidget)
    {
        if (platformDispatcher.implicitView is null)
        {
            throw new InvalidOperationException(
                "The app requested a view, but the platform did not provide one.\n"
                    + "This is likely because the app called `runApp` to render its root "
                    + "widget, which expects the platform to provide a default view to "
                    + "render into (the \"implicit\" view).\n"
                    + "However, the platform likely has multi-view mode enabled, which does "
                    + "not create this default \"implicit\" view.\n"
                    + "Try using `runWidget` instead of `runApp` to start your app.\n"
                    + "`runWidget` allows you to provide a `View` widget, without requiring "
                    + "a default view."
                    + $"{(Foundation.ConstantsLibrary.kIsWeb ? "\nSee: https://flutter.dev/to/web-multiview-runwidget" : "")}"
            );
        }
        return new View(
            view: platformDispatcher.implicitView!,
            deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner: pipelineOwner,
            deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView: renderView,
            child: rootWidget
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void scheduleAttachRootWidget(Widget rootWidget)
    {
        Timer.run(() =>
        {
            attachRootWidget(rootWidget);
        });
    }

    public virtual void attachRootWidget(Widget rootWidget)
    {
        attachToBuildOwner(new RootWidget(debugShortDescription: "[root]", child: rootWidget));
    }

    public virtual void attachToBuildOwner(RootWidget widget)
    {
        var isBootstrapFrame = rootElement is null;
        _readyToProduceFrames = true;
        _rootElement = DartRuntimePrimitives.ConvertValue<Element>(
            widget.attach(buildOwner!, ((RootElement?)rootElement)!)
        );
        if (isBootstrapFrame)
        {
            Scheduler.SchedulerBinding.instance.ensureVisualUpdate();
        }
    }

    public virtual bool isRootWidgetAttached =>
        DartRuntimePrimitives.ConvertValue<bool>(_rootElement is not null);

    public virtual Locale? computePlatformResolvedLocale(List<Locale> supportedLocales)
    {
        return platformDispatcher.computePlatformResolvedLocale(supportedLocales);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual WindowingOwnerIo windowingOwner
    {
        get
        {
            if (!_featuresLibrary.isWindowingEnabled)
            {
                throw new NotSupportedException(
                    "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n"
                );
            }
            return _windowingOwner;
        }
        set
        {
            var owner = value;
            if (!_featuresLibrary.isWindowingEnabled)
            {
                throw new NotSupportedException(
                    "Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n"
                );
            }
            _windowingOwner = owner;
        }
    }
}

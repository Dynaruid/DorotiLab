// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/binding.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public interface WidgetsBindingObserver
{
    public Future<bool> didPopRoute() => Future<bool>.value(false);
    public bool handleStartBackGesture(global::Doroti.Framework.Services.PredictiveBackEvent backEvent) => false;
    public void handleUpdateBackGestureProgress(global::Doroti.Framework.Services.PredictiveBackEvent backEvent)
    {
    }
    public void handleCommitBackGesture()
    {
    }
    public void handleCancelBackGesture()
    {
    }
    public void handleStatusBarTap()
    {
    }
    public Future<bool> didPushRoute(string route) => Future<bool>.value(false);
    public Future<bool> didPushRouteInformation(RouteInformation routeInformation)
    {
        DartUri uri__8315 = ((RouteInformation)routeInformation).uri;
        return ((Future<bool>)(object?)didPushRoute(Dart_coreLibrary.decodeComponent(new DartUri(path: ((uri__8315.path.Length == 0) ? "/" : uri__8315.path), queryParameters: (!System.Linq.Enumerable.Any(uri__8315.queryParametersAll) ? null : uri__8315.queryParametersAll), fragment: ((uri__8315.fragment.Length == 0) ? null : uri__8315.fragment)).ToString())));
    }
    public void didChangeMetrics()
    {
    }
    public void didChangeTextScaleFactor()
    {
    }
    public void didChangePlatformBrightness()
    {
    }
    public void didChangeLocales(List<Locale>? locales)
    {
    }
    public void didChangeAppLifecycleState(AppLifecycleState state)
    {
    }
    public void didChangeViewFocus(ViewFocusEvent @event)
    {
    }
    public async Future<global::Doroti.Ui.AppExitResponse> didRequestAppExit()
    {
        return AppExitResponse.exit;
    }
    public void didHaveMemoryPressure()
    {
    }
    public void didChangeAccessibilityFeatures()
    {
    }
}

public interface WidgetsBinding
{
    public static WidgetsBinding? _instance = default;
    global::Doroti.Framework.Foundation.ValueNotifier<bool>? _debugShowWidgetInspectorOverrideNotifierObject { get; set; }
    global::Doroti.Framework.Foundation.ValueNotifier<bool>? _debugWidgetInspectorSelectionOnTapEnabledNotifierObject { get; set; }
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
    public global::Doroti.Framework.Foundation.ValueNotifier<bool> debugShowWidgetInspectorOverrideNotifier { get; }
    public global::Doroti.Framework.Foundation.ValueNotifier<bool> debugWidgetInspectorSelectionOnTapEnabled { get; }
    public bool debugExcludeRootWidgetInspector { get; set; }
    public void resetInternalState();
    public void _debugAddStackFilters();
    public DartMap<string, List<DartMap<string, string>>> _formatEvaluationResult(List<ViolationIo> violations);
    public Future _forceRebuild();
    public BuildOwner? buildOwner { get; }
    public FocusManager focusManager { get; }
    public void addObserver(WidgetsBindingObserver observer);
    public bool removeObserver(WidgetsBindingObserver observer);
    public Future<global::Doroti.Ui.AppExitResponse> handleRequestAppExit();
    public void handleMetricsChanged();
    public void handleTextScaleFactorChanged();
    public void handlePlatformBrightnessChanged();
    public void handleAccessibilityFeaturesChanged();
    public void handleLocaleChanged();
    public void dispatchLocalesChanged(List<Locale>? locales);
    public void dispatchAccessibilityFeaturesChanged();
    public Future _handleStatusBarActions(global::Doroti.Framework.Services.MethodCall call);
    public Future<bool> handlePopRoute();
    public bool _handleStartBackGesture(DartMap<string?, object> arguments);
    public void _handleUpdateBackGestureProgress(DartMap<string?, object> arguments);
    public Future _handleCommitBackGesture();
    public void _handleCancelBackGesture();
    public Future<bool> handlePushRoute(string route);
    public Future<bool> _handlePushRouteInformation(DartMap<object, object> routeArguments);
    public Future<bool> _handleNavigationInvocation(global::Doroti.Framework.Services.MethodCall methodCall);
    public Future<object> _handleBackGestureInvocation(global::Doroti.Framework.Services.MethodCall methodCall);
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
    public global::Doroti.Ui.Locale? computePlatformResolvedLocale(List<Locale> supportedLocales);
    public WindowingOwnerIo windowingOwner { get; set; }
    PlatformDispatcher platformDispatcher { get; }
    AppLifecycleState? lifecycleState { get; }
    bool debugCheckZone(string entryPoint);
    void addPostFrameCallback(global::System.Action<Duration> callback, string debugLabel = "callback");
    void scheduleWarmUpFrame();
    IEnumerable<global::Doroti.Framework.Rendering.RenderView> renderViews { get; }
    void hitTestInView(global::Doroti.Framework.Gestures.HitTestResult result, Offset position, long viewId);
    DorotiView window => platformDispatcher.implicitView ?? throw new InvalidOperationException("WidgetsBinding.window requires exactly one Flutter view.");
    Future endOfFrame => global::Doroti.Framework.Scheduler.SchedulerBinding.instance.endOfFrame;
    void cancelPointer(long pointer) => global::Doroti.Framework.Gestures.GestureBinding.instance.cancelPointer(pointer);
}

public static partial class BindingLibrary
{
    public static void runApp(Widget app)
    {
        WidgetsBinding binding__71383 = ((WidgetsBinding)(object?)WidgetsFlutterBinding.ensureInitialized());
        BindingLibrary._runWidget(binding__71383.wrapWithDefaultView(app), binding__71383, "runApp");
    }
}

public static partial class BindingLibrary
{
    public static void runWidget(Widget app)
    {
        WidgetsBinding binding__73993 = ((WidgetsBinding)(object?)WidgetsFlutterBinding.ensureInitialized());
        BindingLibrary._runWidget(app, binding__73993, "runWidget");
    }
}

public static partial class BindingLibrary
{
    internal static void _runWidget(Widget app, WidgetsBinding binding, string debugEntryPoint)
    {
        DartRuntimePrimitives.Assert(() => binding.debugCheckZone(debugEntryPoint));
        DartRuntimePrimitives.Ignore(((Func<WidgetsBinding>)(() =>
{
    var __cascade = binding;
    __cascade.scheduleAttachRootWidget(app);
    __cascade.scheduleWarmUpFrame();
    return __cascade;
}))());
    }
}

public static partial class BindingLibrary
{
    internal static string _debugDumpAppString()
    {
        var mode__74336 = (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? "DEBUG MODE" : (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode ? "RELEASE MODE" : "PROFILE MODE"));
        var buffer__74451 = new StringBuffer();
        buffer__74451.writeln($"{DartRuntimePrimitives.RuntimeType(WidgetsBinding.instance)} - {mode__74336}");
        if ((WidgetsBinding.instance.rootElement is not null))
        {
            buffer__74451.writeln(((string)((dynamic)WidgetsBinding.instance.rootElement!).toStringDeep()));
        }
        else
        {
            buffer__74451.writeln("<no tree currently mounted>");
        }
        return buffer__74451.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpApp()
    {
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(BindingLibrary._debugDumpAppString());
    }
}

public class RootWidget : Widget
{
    public virtual Widget? child { get; private set; }
    public virtual string? debugShortDescription { get; private set; }

    public RootWidget(global::Doroti.Framework.Foundation.Key? key = null, Widget? child = null, string? debugShortDescription = null) : base(key: key)
    {
        this.child = child;
        this.debugShortDescription = debugShortDescription;
    }

    public override RootElement createElement() => new RootElement(this);
    public virtual RootElement attach(BuildOwner owner, RootElement? element = null)
    {
        if ((element is null))
        {
            owner.lockState(((global::System.Action)(() =>
            {
                element = createElement();
                DartRuntimePrimitives.Assert(() => (element is not null));
                element!.assignOwner(owner);
            })));
            owner.buildScope(element!, ((global::System.Action)(() =>
            {
                element!.mount(((Element)(object)null), null);
            })));
        }
        else
        {
            element._newWidget = this;
            element.markNeedsBuild();
        }
        return element!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => DartRuntimePrimitives.ConvertValue<string>(((this.debugShortDescription ?? (string)base.toStringShort())));
}

public class RootElement : Element, RootElementMixin
{
    internal virtual Element? _child { get; set; } = default;
    internal virtual RootWidget? _newWidget { get; set; } = default;

    public RootElement(RootWidget widget) : base(widget)
    {
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override void forgetChild(Element child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this._child)));
        _child = null;
        base.forgetChild(child);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => (parent is null));
        DartRuntimePrimitives.Assert(() => (parent is null));
        DartRuntimePrimitives.Assert(() => (newSlot is null));
        base.mount(parent, newSlot);
        _rebuild();
        DartRuntimePrimitives.Assert(() => (this._child is not null));
        base.performRebuild();
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (RootWidget)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        _rebuild();
    }

    public override void performRebuild()
    {
        if ((this._newWidget is not null))
        {
            RootWidget newWidget__78139 = this._newWidget!;
            _newWidget = null;
            update(newWidget__78139);
        }
        base.performRebuild();
        DartRuntimePrimitives.Assert(() => (this._newWidget is null));
    }

    internal virtual void _rebuild()
    {
        try
        {
            _child = updateChild(this._child, ((Widget?)((dynamic)(((RootWidget?)(object?)this.widget)!)).child), null);
        }
        catch (Exception exception__78411)
        {
            var stack__78422 = new System.Diagnostics.StackTrace();
            var details__78443 = new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__78411, stack: stack__78422, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("attaching to the render tree"));
            FlutterError.reportError(details__78443);
            _child = null;
        }
    }

    public virtual bool debugDoingBuild => false;
    public override bool debugExpectsRenderObjectForSlot(object? slot) => false;
    public virtual void assignOwner(BuildOwner owner)
    {
        _owner = owner;
        _parentBuildScope = new BuildScope();
    }

}

public class WidgetsFlutterBinding : global::Doroti.Framework.Gestures.GestureBinding, global::Doroti.Framework.Painting.PaintingBinding, global::Doroti.Framework.Semantics.SemanticsBinding, global::Doroti.Framework.Rendering.RendererBinding, WidgetsBinding
{
    public WidgetsFlutterBinding(PlatformDispatcher? platformDispatcher = null) : base(platformDispatcher) { }

    public virtual ImageCache _imageCache { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting._SystemFontsNotifier__binding _systemFonts { get; set; } = new _SystemFontsNotifier__binding();
    private bool __late__semanticsEnabled_initialized;
    private global::Doroti.Framework.Foundation.ValueNotifier<bool> __late__semanticsEnabled = default!;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _semanticsEnabled
    {
        get
        {
            if (!__late__semanticsEnabled_initialized)
            {
                __late__semanticsEnabled = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(this.platformDispatcher.semanticsEnabled);
                __late__semanticsEnabled_initialized = true;
            }
            return __late__semanticsEnabled;
        }
    }
    public virtual global::Doroti.Framework.Foundation.ObserverList<global::System.Action<SemanticsActionEvent>> _semanticsActionListeners { get; set; } = new global::Doroti.Framework.Foundation.ObserverList<global::System.Action<SemanticsActionEvent>>();
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
                __late__manifold = ((PipelineManifold)(object?)new _BindingPipelineManifold__binding(this));
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
                __late_pipelineOwner = new PipelineOwner(onSemanticsOwnerCreated: ((global::System.Action)(() =>
                {
                    (((RenderView?)(object?)((PipelineOwner)this.pipelineOwner).rootNode)!)?.scheduleInitialSemantics();
                })), onSemanticsUpdate: ((global::System.Action<SemanticsUpdate>)((update) =>
                {
                    (((RenderView?)(object?)((PipelineOwner)this.pipelineOwner).rootNode)!)?.updateSemantics(update);
                })), onSemanticsOwnerDisposed: ((global::System.Action)(() =>
                {
                    (((RenderView?)(object?)((PipelineOwner)this.pipelineOwner).rootNode)!)?.clearSemantics();
                })));
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
                __late_renderView = ((RenderView)(object?)new _ReusableRenderView__binding(view: this.platformDispatcher.implicitView!));
                __late_renderView_initialized = true;
            }
            return __late_renderView;
        }
    }
    public virtual PipelineOwner _rootPipelineOwner { get; set; } = default!;
    public virtual DartMap<object, RenderView> _viewIdToRenderView { get; set; } = new DartMap<object, RenderView>();
    public virtual bool _debugMouseTrackerUpdateScheduled { get; set; } = false;
    public virtual long _firstFrameDeferredCount { get; set; } = 0L;
    public virtual bool _firstFrameSent { get; set; } = false;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool>? _debugShowWidgetInspectorOverrideNotifierObject { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool>? _debugWidgetInspectorSelectionOnTapEnabledNotifierObject { get; set; } = default;
    public virtual bool _debugExcludeRootWidgetInspector { get; set; } = false;
    public virtual BuildOwner? _buildOwner { get; set; } = default;
    public virtual PlatformMenuDelegate platformMenuDelegate { get; set; } = default!;
    public virtual List<WidgetsBindingObserver> _observers { get; set; } = new List<WidgetsBindingObserver>();
    public virtual List<WidgetsBindingObserver> _backGestureObservers { get; set; } = new List<WidgetsBindingObserver>();
    public virtual bool _needToReportFirstFrame { get; set; } = true;
    public virtual Completer<object?> _firstFrameCompleter { get; set; } = new Completer<object?>();
    public virtual bool debugBuildingDirtyElements { get; set; } = false;
    public virtual Element? _rootElement { get; set; } = default;
    public virtual bool _readyToProduceFrames { get; set; } = false;
    public virtual WindowingOwnerIo _windowingOwner { get; set; } = default!;

    public static WidgetsBinding ensureInitialized()
    {
        if ((WidgetsBinding._instance is null))
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
        global::Doroti.Framework.Semantics.SemanticsBinding._instance = this;
        global::Doroti.Framework.Rendering.RendererBinding._instance = this;
        WidgetsBinding._instance = this;
        this._accessibilityFeatures = this.platformDispatcher.accessibilityFeatures;
        this.platformDispatcher.onSemanticsEnabledChanged = this._handleSemanticsEnabledChanged;
        this.platformDispatcher.onSemanticsActionEvent = this._handleSemanticsActionEvent;
        this.platformDispatcher.onAccessibilityFeaturesChanged = () =>
        {
            if (global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase ==
                global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    _ => handleAccessibilityFeaturesChanged(),
                    debugLabel: "SemanticsBinding.handleAccessibilityFeaturesChanged");
            }
            else
            {
                handleAccessibilityFeaturesChanged();
            }
        };
        _handleSemanticsEnabledChanged();
        addSemanticsEnabledListener(this._handleFrameworkSemanticsEnabledChanged);
        if (this.semanticsEnabled)
        {
            _handleFrameworkSemanticsEnabledChanged();
        }
        this._buildOwner = new BuildOwner();
        this._buildOwner.onBuildScheduled = _handleBuildScheduled;
        this._imageCache = createImageCache();
        DartRuntimePrimitives.Ignore(PaintingBinding.shaderWarmUp?.execute());
        this._rootPipelineOwner = createRootPipelineOwner();
        this.platformDispatcher.onMetricsChanged = _ => handleMetricsChanged();
        this.platformDispatcher.onTextScaleFactorChanged = handleTextScaleFactorChanged;
        this.platformDispatcher.onPlatformBrightnessChanged = handlePlatformBrightnessChanged;
        addPersistentFrameCallback(this._handlePersistentFrameCallback);
        initMouseTracker();
        this.rootPipelineOwner.attach(this._manifold);
    }

    public virtual ImageCache imageCache => this._imageCache;
    public virtual ImageCache createImageCache() => new ImageCache();
    public virtual Future<global::Doroti.Ui.Codec> instantiateImageCodecFromBuffer(ImmutableBuffer buffer, long? cacheWidth = null, long? cacheHeight = null, bool allowUpscaling = false)
    {
        DartRuntimePrimitives.Assert(() => ((cacheWidth is null) || (cacheWidth > 0L)));
        DartRuntimePrimitives.Assert(() => ((cacheHeight is null) || (cacheHeight > 0L)));
        return ((Future<global::Doroti.Ui.Codec>)(object?)Dart_uiLibrary.instantiateImageCodecFromBuffer(buffer, targetWidth: cacheWidth, targetHeight: cacheHeight, allowUpscaling: allowUpscaling));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<global::Doroti.Ui.Codec> instantiateImageCodecWithSize(ImmutableBuffer buffer, global::System.Func<long, long, TargetImageSize>? getTargetSize = null)
    {
        return ((Future<global::Doroti.Ui.Codec>)(object?)Dart_uiLibrary.instantiateImageCodecWithSize(buffer, getTargetSize: getTargetSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void evict(string asset)
    {
        base.evict(asset);
        this.imageCache.clear();
        this.imageCache.clearLiveImages();
    }

    public virtual void handleMemoryPressure()
    {
        base.handleMemoryPressure();
        this.imageCache.clear();
    }

    public virtual global::Doroti.Framework.Foundation.Listenable systemFonts => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.Listenable>(this._systemFonts);
    public async virtual Future handleSystemMessage(object systemMessage)
    {
        await base.handleSystemMessage(systemMessage);
        var message__7319 = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)systemMessage);
        var type__7378 = ((string?)(object?)message__7319.GetValueOrDefault("type"))!;
        switch (type__7378)
        {
            case "fontsChange":
                {
                    this._systemFonts.notifyListeners();
                    break;
                }
        }
        return;
    }

    public virtual bool semanticsEnabled
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._semanticsEnabled).value == ((this._outstandingHandles > 0L))));
            return ((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._semanticsEnabled).value;
            return default!;
        }
    }
    public virtual void addSemanticsEnabledListener(global::System.Action listener)
    {
        this._semanticsEnabled.addListener(() => listener());
    }

    public virtual void removeSemanticsEnabledListener(global::System.Action listener)
    {
        this._semanticsEnabled.removeListener(() => listener());
    }

    public virtual void addSemanticsActionListener(global::System.Action<SemanticsActionEvent> listener)
    {
        this._semanticsActionListeners.add((global::System.Action<SemanticsActionEvent>)listener);
    }

    public virtual void removeSemanticsActionListener(global::System.Action<SemanticsActionEvent> listener)
    {
        this._semanticsActionListeners.remove((global::System.Action<SemanticsActionEvent>)listener);
    }

    public virtual global::Doroti.Ui.Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>(null);
    public virtual long debugOutstandingSemanticsHandles => this._outstandingHandles;
    public virtual SemanticsHandle ensureSemantics()
    {
        DartRuntimePrimitives.Assert(() => (this._outstandingHandles >= 0L));
        this._outstandingHandles++;
        DartRuntimePrimitives.Assert(() => (this._outstandingHandles > 0L));
        this._semanticsEnabled.value = true;
        return new SemanticsHandle(() => this._didDisposeSemanticsHandle());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _didDisposeSemanticsHandle()
    {
        DartRuntimePrimitives.Assert(() => (this._outstandingHandles > 0L));
        this._outstandingHandles--;
        DartRuntimePrimitives.Assert(() => (this._outstandingHandles >= 0L));
        this._semanticsEnabled.value = (this._outstandingHandles > 0L);
    }

    public virtual void _handleSemanticsEnabledChanged()
    {
        if (this.platformDispatcher.semanticsEnabled)
        {
            this._semanticsHandle ??= ensureSemantics();
        }
        else
        {
            this._semanticsHandle?.dispose();
            this._semanticsHandle = null;
        }
    }

    public virtual void _handleSemanticsActionEvent(SemanticsActionEvent action)
    {
        object? arguments__6642 = action.arguments;
        global::Doroti.Ui.SemanticsActionEvent decodedAction__6706 = ((global::Doroti.Ui.SemanticsActionEvent)(object?)((arguments__6642 is ByteData) ? action.copyWith(arguments: new global::Doroti.Framework.Services.StandardMessageCodec().decodeMessage(((ByteData)arguments__6642))) : action));
        List<global::System.Action<global::Doroti.Ui.SemanticsActionEvent>> localListeners__7088 = ((List<global::System.Action<global::Doroti.Ui.SemanticsActionEvent>>)(object?)this._semanticsActionListeners.toList());
        foreach (var listener__7180 in localListeners__7088)
        {
            if (this._semanticsActionListeners.contains((global::System.Action<SemanticsActionEvent>)listener__7180))
            {
                listener__7180(decodedAction__6706);
            }
        }
        performSemanticsAction(decodedAction__6706);
    }

    public virtual void _handleFrameworkSemanticsEnabledChanged()
    {
        this.platformDispatcher.setSemanticsTreeEnabled(this.semanticsEnabled);
    }

    public virtual global::Doroti.Ui.AccessibilityFeatures accessibilityFeatures => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.AccessibilityFeatures>(this._accessibilityFeatures);
    public virtual void handleAccessibilityFeaturesChanged()
    {
        this._accessibilityFeatures = this.platformDispatcher.accessibilityFeatures;
    }

    public virtual global::Doroti.Ui.SemanticsUpdateBuilder createSemanticsUpdateBuilder()
    {
        return ((global::Doroti.Ui.SemanticsUpdateBuilder)(object?)new global::Doroti.Ui.SemanticsUpdateBuilder());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool disableAnimations
    {
        get
        {
            bool value__9471 = this._accessibilityFeatures.disableAnimations;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Framework.Semantics.DebugLibrary.debugSemanticsDisableAnimations is not null))
                    {
                        value__9471 = DartRuntimePrimitives.RequireValue(global::Doroti.Framework.Semantics.DebugLibrary.debugSemanticsDisableAnimations);
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return value__9471;
            return default!;
        }
    }
    protected override void initServiceExtensions()
    {
        base.initServiceExtensions();
        DartRuntimePrimitives.Assert(() =>
            {
                registerBoolServiceExtension(name: RenderingServiceExtensions.invertOversizedImages.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Painting.DebugLibrary.debugInvertOversizedImages)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Painting.DebugLibrary.debugInvertOversizedImages != value))
                    {
                        global::Doroti.Framework.Painting.DebugLibrary.debugInvertOversizedImages = value;
                        global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    }
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugPaint.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugPaintBaselinesEnabled.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.repaintRainbow.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    bool repaint__4659 = (global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled && !value);
                    global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled = value;
                    if (repaint__4659)
                    {
                        global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    }
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerServiceExtension(name: RenderingServiceExtensions.debugDumpLayerTree.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
                {
                    return new DartMap<string, object> { ["data"] = global::Doroti.Framework.Rendering.BindingLibrary._debugCollectLayerTrees() };
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisableClipLayers.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisablePhysicalShapeLayers.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugDisablePhysicalShapeLayers)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugDisablePhysicalShapeLayers == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugDisablePhysicalShapeLayers = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisableOpacityLayers.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpRenderTree.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = global::Doroti.Framework.Rendering.BindingLibrary._debugCollectRenderTrees() };
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpSemanticsTreeInTraversalOrder.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = global::Doroti.Framework.Rendering.BindingLibrary._debugCollectSemanticsTrees(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder.traversalOrder) };
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpSemanticsTreeInInverseHitTestOrder.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = global::Doroti.Framework.Rendering.BindingLibrary._debugCollectSemanticsTrees(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder.inverseHitTest) };
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            registerBoolServiceExtension(name: RenderingServiceExtensions.profileRenderObjectPaints.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled)), setter: ((Func<bool, Future>)(async (value) =>
            {
                if ((global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled != value))
                {
                    global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled = value;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            registerBoolServiceExtension(name: RenderingServiceExtensions.profileRenderObjectLayouts.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled)), setter: ((Func<bool, Future>)(async (value) =>
            {
                if ((global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled != value))
                {
                    global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled = value;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
        }
    }

    public virtual MouseTracker mouseTracker => DartRuntimePrimitives.ConvertValue<MouseTracker>(this._mouseTracker!);
    public virtual PipelineOwner createRootPipelineOwner()
    {
        return ((PipelineOwner)(object?)new _DefaultRootPipelineOwner__binding());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PipelineOwner rootPipelineOwner => this._rootPipelineOwner;
    public virtual IEnumerable<RenderView> renderViews => this._viewIdToRenderView.Values;
    public virtual void addRenderView(RenderView view)
    {
        object viewId__14388 = checked((long)((RenderView)view).flutterView.viewId);
        DartRuntimePrimitives.Assert(() => !this._viewIdToRenderView.containsValue(view));
        DartRuntimePrimitives.Assert(() => !this._viewIdToRenderView.ContainsKey(viewId__14388));
        this._viewIdToRenderView[viewId__14388] = view;
        view.configuration = createViewConfigurationFor(view);
    }

    public virtual void removeRenderView(RenderView view)
    {
        object viewId__14785 = checked((long)((RenderView)view).flutterView.viewId);
        DartRuntimePrimitives.Assert(() => (object.Equals(this._viewIdToRenderView.GetValueOrDefault(viewId__14785), view)));
        this._viewIdToRenderView.remove(viewId__14785);
    }

    public virtual global::Doroti.Framework.Rendering.ViewConfiguration createViewConfigurationFor(RenderView renderView)
    {
        return global::Doroti.Framework.Rendering.ViewConfiguration.CreateFromView(((RenderView)renderView).flutterView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.SceneBuilder createSceneBuilder() =>
        new global::Doroti.Ui.SceneBuilder(
            this.platformDispatcher.implicitView?.viewId ??
            throw new InvalidOperationException("Rendering a scene requires an implicit Flutter view."));
    public virtual global::Doroti.Ui.PictureRecorder createPictureRecorder() => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.PictureRecorder>(new global::Doroti.Ui.PictureRecorder());
    public virtual global::Doroti.Ui.Canvas createCanvas(PictureRecorder recorder) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Canvas>(new global::Doroti.Ui.Canvas(recorder));
    public virtual void handleMetricsChanged()
    {
        var forceFrame__16782 = false;
        foreach (RenderView view__16828 in this.renderViews)
        {
            forceFrame__16782 = (forceFrame__16782 || (((RenderBox?)((dynamic)view__16828).child) is not null));
            view__16828.configuration = createViewConfigurationFor(view__16828);
        }
        if (forceFrame__16782)
        {
            scheduleForcedFrame();
        }
    }

    public virtual void handleTextScaleFactorChanged()
    {
    }

    public virtual void handlePlatformBrightnessChanged()
    {
    }

    public virtual void initMouseTracker(MouseTracker? tracker = null)
    {
        this._mouseTracker?.dispose();
        this._mouseTracker = (tracker ?? new MouseTracker(((global::System.Func<Offset, long, global::Doroti.Framework.Gestures.HitTestResult>)((position, viewId) =>
        {
            var result__18519 = new global::Doroti.Framework.Gestures.HitTestResult();
            hitTestInView(result__18519, position, viewId);
            return result__18519;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
    }

    public override void dispatchEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestResult result)
    {
        this._mouseTracker!.updateWithEvent(@event, ((@event is global::Doroti.Framework.Gestures.PointerMoveEvent) ? null : result));
        base.dispatchEvent(@event, result);
    }

    public virtual void performSemanticsAction(SemanticsActionEvent action)
    {
        this._viewIdToRenderView.GetValueOrDefault(checked((long)action.viewId))?.owner?.semanticsOwner?.performAction(action.nodeId, action.type, action.arguments);
    }

    public virtual void _handleWebFirstFrame(Duration __unused0)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb);
        var methodChannel__21314 = new global::Doroti.Framework.Services.MethodChannel("flutter/service_worker");
        DartRuntimePrimitives.Ignore(methodChannel__21314.invokeMethod<object?>("first-frame").then(((_) =>
        {
        }), onError: ((error, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "rendering library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending the first-frame event")));
        })));
    }

    public virtual void _handlePersistentFrameCallback(Duration timeStamp)
    {
        drawFrame();
        _scheduleMouseTrackerUpdate();
    }

    public virtual void _scheduleMouseTrackerUpdate()
    {
        DartRuntimePrimitives.Assert(() => !this._debugMouseTrackerUpdateScheduled);
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugMouseTrackerUpdateScheduled = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
        {
            DartRuntimePrimitives.Assert(() => this._debugMouseTrackerUpdateScheduled);
            DartRuntimePrimitives.Assert(() =>
                {
                    this._debugMouseTrackerUpdateScheduled = false;
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            this._mouseTracker!.updateAllDevices();
        })), debugLabel: "RendererBinding.mouseTrackerUpdate");
    }

    public virtual bool sendFramesToEngine => DartRuntimePrimitives.ConvertValue<bool>((this._firstFrameSent || (this._firstFrameDeferredCount == 0L)));
    public virtual void deferFirstFrame()
    {
        DartRuntimePrimitives.Assert(() => (this._firstFrameDeferredCount >= 0L));
        this._firstFrameDeferredCount += 1L;
    }

    public virtual void allowFirstFrame()
    {
        DartRuntimePrimitives.Assert(() => (this._firstFrameDeferredCount > 0L));
        this._firstFrameDeferredCount -= 1L;
        if (!this._firstFrameSent)
        {
            scheduleWarmUpFrame();
        }
    }

    public virtual void resetFirstFrameSent()
    {
        this._firstFrameSent = false;
    }

    public virtual void drawFrame()
    {
        this.debugBuildingDirtyElements = true;
        try
        {
            if (this.rootElement is not null)
            {
                this.buildOwner!.buildScope(this.rootElement);
            }
            this.rootPipelineOwner.flushLayout();
            this.rootPipelineOwner.flushCompositingBits();
            this.rootPipelineOwner.flushPaint();
            if (this.sendFramesToEngine)
            {
                foreach (RenderView renderView__27663 in this.renderViews)
                {
                    renderView__27663.compositeFrame();
                }
                this.rootPipelineOwner.flushSemantics();
                this._firstFrameSent = true;
            }
            this.buildOwner!.finalizeTree();
        }
        finally
        {
            this.debugBuildingDirtyElements = false;
        }
        this._needToReportFirstFrame = false;
    }

    protected async override Task performReassemble()
    {
        await base.performReassemble();
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync("Preparing Hot Reload (layout)");
        }
        try
        {
            foreach (RenderView renderView__28120 in this.renderViews)
            {
                renderView__28120.reassemble();
            }
        }
        finally
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
        scheduleWarmUpFrame();
        await this.endOfFrame;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void hitTestInView(global::Doroti.Framework.Gestures.HitTestResult result, Offset position, long viewId)
    {
        this._viewIdToRenderView.GetValueOrDefault(viewId)?.hitTest(result, position: position);
        base.hitTestInView(result, position, viewId);
    }

    public virtual Future _forceRepaint()
    {
        global::System.Action<RenderObject> visitor__28613 = default!;
        visitor__28613 = (global::System.Action<RenderObject>)((child) =>
        {
            ((dynamic)child).markNeedsPaint();
            ((dynamic)child).visitChildren((global::System.Action<RenderObject>)visitor__28613);
        });
        foreach (RenderView renderView__28758 in this.renderViews)
        {
            renderView__28758.visitChildren((global::System.Action<RenderObject>)visitor__28613);
        }
        return this.endOfFrame;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugShowWidgetInspectorOverride
    {
        get
        {
            return ((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this.debugShowWidgetInspectorOverrideNotifier).value;
            return default!;
        }
        set
        {
            var __value = value;
            this.debugShowWidgetInspectorOverrideNotifier.value = __value;
        }
    }
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> debugShowWidgetInspectorOverrideNotifier => this._debugShowWidgetInspectorOverrideNotifierObject ??= new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> debugWidgetInspectorSelectionOnTapEnabled => this._debugWidgetInspectorSelectionOnTapEnabledNotifierObject ??= new global::Doroti.Framework.Foundation.ValueNotifier<bool>(true);
    public virtual bool debugExcludeRootWidgetInspector
    {
        get => this._debugExcludeRootWidgetInspector;
        set
        {
            var __value = value;
            this._debugExcludeRootWidgetInspector = __value;
        }
    }
    public virtual void resetInternalState()
    {
        base.resetInternalState();
        this._debugShowWidgetInspectorOverrideNotifierObject?.dispose();
        this._debugShowWidgetInspectorOverrideNotifierObject = null;
        this._debugWidgetInspectorSelectionOnTapEnabledNotifierObject?.dispose();
        this._debugWidgetInspectorSelectionOnTapEnabledNotifierObject = null;
    }

    public virtual void _debugAddStackFilters()
    {
        var elementInflateWidget__21891 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "Element", method: "inflateWidget");
        var elementUpdateChild__22070 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "Element", method: "updateChild");
        var elementRebuild__22245 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "Element", method: "rebuild");
        var componentElementPerformRebuild__22412 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "ComponentElement", method: "performRebuild");
        var componentElementFirstBuild__22611 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "ComponentElement", method: "_firstBuild");
        var componentElementMount__22803 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "ComponentElement", method: "mount");
        var statefulElementFirstBuild__22984 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "StatefulElement", method: "_firstBuild");
        var singleChildMount__23174 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "SingleChildRenderObjectElement", method: "mount");
        var statefulElementRebuild__23364 = new global::Doroti.Framework.Foundation.PartialStackFrame(package: "package:flutter/src/widgets/framework.dart", className: "StatefulElement", method: "performRebuild");
        var replacementString__23555 = "...     Normal element mounting";
        FlutterError.addDefaultStackFilter(new global::Doroti.Framework.Foundation.RepetitiveStackFrameFilter(frames: new List<global::Doroti.Framework.Foundation.PartialStackFrame> { elementInflateWidget__21891, elementUpdateChild__22070, componentElementPerformRebuild__22412, elementRebuild__22245, componentElementFirstBuild__22611, componentElementMount__22803 }, replacement: replacementString__23555));
        FlutterError.addDefaultStackFilter(new global::Doroti.Framework.Foundation.RepetitiveStackFrameFilter(frames: new List<global::Doroti.Framework.Foundation.PartialStackFrame> { elementUpdateChild__22070, componentElementPerformRebuild__22412, elementRebuild__22245, componentElementFirstBuild__22611, componentElementMount__22803 }, replacement: replacementString__23555));
        FlutterError.addDefaultStackFilter(new global::Doroti.Framework.Foundation.RepetitiveStackFrameFilter(frames: new List<global::Doroti.Framework.Foundation.PartialStackFrame> { elementInflateWidget__21891, elementUpdateChild__22070, componentElementPerformRebuild__22412, statefulElementRebuild__23364, elementRebuild__22245, componentElementFirstBuild__22611, statefulElementFirstBuild__22984, componentElementMount__22803 }, replacement: replacementString__23555));
        FlutterError.addDefaultStackFilter(new global::Doroti.Framework.Foundation.RepetitiveStackFrameFilter(frames: new List<global::Doroti.Framework.Foundation.PartialStackFrame> { elementUpdateChild__22070, componentElementPerformRebuild__22412, statefulElementRebuild__23364, elementRebuild__22245, componentElementFirstBuild__22611, statefulElementFirstBuild__22984, componentElementMount__22803 }, replacement: replacementString__23555));
        FlutterError.addDefaultStackFilter(new global::Doroti.Framework.Foundation.RepetitiveStackFrameFilter(frames: new List<global::Doroti.Framework.Foundation.PartialStackFrame> { elementInflateWidget__21891, elementUpdateChild__22070, singleChildMount__23174 }, replacement: replacementString__23555));
        FlutterError.addDefaultStackFilter(new global::Doroti.Framework.Foundation.RepetitiveStackFrameFilter(frames: new List<global::Doroti.Framework.Foundation.PartialStackFrame> { elementUpdateChild__22070, singleChildMount__23174 }, replacement: replacementString__23555));
    }

    public virtual DartMap<string, List<DartMap<string, string>>> _formatEvaluationResult(List<ViolationIo> violations)
    {
        return new DartMap<string, List<DartMap<string, string>>>
        {
            ["result"] = violations.map<ViolationIo, DartMap<string, string>>(((violation) =>
            {
                return new DartMap<string, string> { ["nodeId"] = ((ViolationIo)violation).node.id.ToString(), ["message"] = ((ViolationIo)violation).reason };
                throw new InvalidOperationException("Dart closure completed without a value.");
            })).ToList()
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future _forceRebuild()
    {
        if ((this.rootElement is not null))
        {
            this.buildOwner!.reassemble(this.rootElement!);
            return this.endOfFrame;
        }
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BuildOwner? buildOwner => this._buildOwner;
    public virtual FocusManager focusManager => this._buildOwner!.focusManager;
    public virtual void addObserver(WidgetsBindingObserver observer) => this._observers.Add(observer);
    public virtual bool removeObserver(WidgetsBindingObserver observer)
    {
        this._backGestureObservers.Remove(observer);
        return this._observers.Remove(observer);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<global::Doroti.Ui.AppExitResponse> handleRequestAppExit()
    {
        var didCancel__34096 = false;
        foreach (var observer__34130 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                if ((object.Equals((await observer__34130.didRequestAppExit()), AppExitResponse.cancel)))
                {
                    didCancel__34096 = true;
                }
            }
            catch (Exception exception__34554)
            {
                var stack__34565 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__34554, stack: stack__34565, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didRequestAppExit")));
            }
        }
        return ((global::Doroti.Ui.AppExitResponse)(object?)(didCancel__34096 ? AppExitResponse.cancel : AppExitResponse.exit));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleLocaleChanged()
    {
        dispatchLocalesChanged(this.platformDispatcher.locales.ToList());
    }

    public virtual void dispatchLocalesChanged(List<Locale>? locales)
    {
        foreach (var observer__38210 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                observer__38210.didChangeLocales(locales);
            }
            catch (Exception exception__38340)
            {
                var stack__38351 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__38340, stack: stack__38351, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangeLocales")));
            }
        }
    }

    public virtual void dispatchAccessibilityFeaturesChanged()
    {
        foreach (var observer__39172 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            observer__39172.didChangeAccessibilityFeatures();
        }
    }

    public async virtual Future _handleStatusBarActions(global::Doroti.Framework.Services.MethodCall call)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Services.MethodCall)call).method == "handleScrollToTop"));
        foreach (var observer__39418 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                observer__39418.handleStatusBarTap();
            }
            catch (Exception exception__39543)
            {
                var stack__39554 = new System.Diagnostics.StackTrace();
                var details__39577 = new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__39543, stack: stack__39554, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("handling status bar action"));
                FlutterError.reportError(details__39577);
            }
        }
    }

    public async virtual Future<bool> handlePopRoute()
    {
        foreach (var observer__41831 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                if (await observer__41831.didPopRoute())
                {
                    return true;
                }
            }
            catch (Exception exception__41994)
            {
                var stack__42005 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__41994, stack: stack__42005, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didPopRoute")));
            }
        }
        DartRuntimePrimitives.Ignore(SystemNavigator.pop().catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((exception, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while popping route")));
        }))));
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _handleStartBackGesture(DartMap<string?, object> arguments)
    {
        this._backGestureObservers.Clear();
        var backEvent__42984 = global::Doroti.Framework.Services.PredictiveBackEvent.CreateFromMap(arguments);
        foreach (var observer__43051 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                if (observer__43051.handleStartBackGesture(backEvent__42984))
                {
                    this._backGestureObservers.Add(observer__43051);
                }
            }
            catch (Exception exception__43252)
            {
                var stack__43263 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__43252, stack: stack__43263, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.handleStartBackGesture")));
            }
        }
        return System.Linq.Enumerable.Any(this._backGestureObservers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _handleUpdateBackGestureProgress(DartMap<string?, object> arguments)
    {
        if (!System.Linq.Enumerable.Any(this._backGestureObservers))
        {
            return;
        }
        var backEvent__43825 = global::Doroti.Framework.Services.PredictiveBackEvent.CreateFromMap(arguments);
        foreach (WidgetsBindingObserver observer__43915 in this._backGestureObservers)
        {
            try
            {
                observer__43915.handleUpdateBackGestureProgress(backEvent__43825);
            }
            catch (Exception exception__44040)
            {
                var stack__44051 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__44040, stack: stack__44051, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.handleUpdateBackGestureProgress")));
            }
        }
    }

    public async virtual Future _handleCommitBackGesture()
    {
        if (!System.Linq.Enumerable.Any(this._backGestureObservers))
        {
            await handlePopRoute();
            return;
        }
        foreach (WidgetsBindingObserver observer__44975 in this._backGestureObservers)
        {
            try
            {
                observer__44975.handleCommitBackGesture();
            }
            catch (Exception exception__45083)
            {
                var stack__45094 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__45083, stack: stack__45094, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.handleCommitBackGesture")));
            }
        }
    }

    public virtual void _handleCancelBackGesture()
    {
        foreach (WidgetsBindingObserver observer__45539 in this._backGestureObservers)
        {
            try
            {
                observer__45539.handleCancelBackGesture();
            }
            catch (Exception exception__45647)
            {
                var stack__45658 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__45647, stack: stack__45658, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.handleCancelBackGesture")));
            }
        }
    }

    public async virtual Future<bool> handlePushRoute(string route)
    {
        var routeInformation__46613 = new RouteInformation(uri: DartUri.parse(route));
        foreach (var observer__46688 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                if (await observer__46688.didPushRouteInformation(routeInformation__46613))
                {
                    return true;
                }
            }
            catch (Exception exception__46879)
            {
                var stack__46890 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__46879, stack: stack__46890, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didPushRouteInformation")));
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> _handlePushRouteInformation(DartMap<object, object> routeArguments)
    {
        var routeInformation__47378 = new RouteInformation(uri: DartUri.parse(((string?)(object?)routeArguments.GetValueOrDefault("location"))!), state: ((object?)(object?)routeArguments.GetValueOrDefault("state"))!);
        foreach (var observer__47546 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                if (await observer__47546.didPushRouteInformation(routeInformation__47378))
                {
                    return true;
                }
            }
            catch (Exception exception__47737)
            {
                var stack__47748 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__47737, stack: stack__47748, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didPushRouteInformation")));
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<bool> _handleNavigationInvocation(global::Doroti.Framework.Services.MethodCall methodCall)
    {
        return (((global::Doroti.Framework.Services.MethodCall)methodCall).method switch { "popRoute" => handlePopRoute(), "pushRoute" => handlePushRoute(((string?)(object?)((global::Doroti.Framework.Services.MethodCall)methodCall).arguments)!), "pushRouteInformation" => _handlePushRouteInformation(DartRuntimePrimitives.ConvertMap<object, object>((System.Collections.IDictionary)((global::Doroti.Framework.Services.MethodCall)methodCall).arguments)), _ => Future<bool>.value(false) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<object> _handleBackGestureInvocation(global::Doroti.Framework.Services.MethodCall methodCall)
    {
        DartMap<string?, object?>? arguments__48683 = (DartRuntimePrimitives.ConvertMap<object?, object?>((System.Collections.IDictionary)((global::Doroti.Framework.Services.MethodCall)methodCall).arguments))?.cast<string?, object?>().cast<string?, object?>();
        return (((global::Doroti.Framework.Services.MethodCall)methodCall).method switch { "startBackGesture" => _handleStartBackGesture(arguments__48683!), "updateBackGestureProgress" => DartRuntimePrimitives.CaptureVoid(() => _handleUpdateBackGestureProgress(arguments__48683!)), "commitBackGesture" => _handleCommitBackGesture(), "cancelBackGesture" => DartRuntimePrimitives.CaptureVoid(() => _handleCancelBackGesture()), _ => throw new global::Doroti.Framework.Services.MissingPluginException() });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleAppLifecycleStateChanged(AppLifecycleState state)
    {
        base.handleAppLifecycleStateChanged(state);
        foreach (var observer__49278 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                observer__49278.didChangeAppLifecycleState(state);
            }
            catch (Exception exception__49416)
            {
                var stack__49427 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__49416, stack: stack__49427, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangeAppLifecycleState")));
            }
        }
    }

    public virtual void handleViewFocusChanged(ViewFocusEvent @event)
    {
        DartRuntimePrimitives.Noop();
        foreach (var observer__49923 in new List<WidgetsBindingObserver>(DartRuntimePrimitives.ConvertEnumerable<WidgetsBindingObserver>(this._observers)))
        {
            try
            {
                observer__49923.didChangeViewFocus(@event);
            }
            catch (Exception exception__50053)
            {
                var stack__50064 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception__50053, stack: stack__50064, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while dispatching notifications for WidgetsBindingObserver.didChangeViewFocus")));
            }
        }
    }

    public virtual bool firstFrameRasterized => this._firstFrameCompleter.isCompleted;
    public virtual Future waitUntilFirstFrameRasterized => this._firstFrameCompleter.future;
    public virtual bool debugDidSendFirstFrameEvent => !this._needToReportFirstFrame;
    public virtual void _handleBuildScheduled()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (this.debugBuildingDirtyElements)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Build scheduled during frame."), new global::Doroti.Framework.Foundation.ErrorDescription("While the widget tree was being built, laid out, and painted, " + "a new frame was scheduled to rebuild the widget tree."), new global::Doroti.Framework.Foundation.ErrorHint("This might be because setState() was called from a layout or " + "paint callback. " + "If a change is needed to the widget tree, it should be applied " + "as the tree is being built. Scheduling a change for the subsequent " + "frame instead results in an interface that lags behind by one frame. " + "If this was done to make your build dependent on a size measured at " + "layout time, consider using a LayoutBuilder, CustomSingleChildLayout, " + "or CustomMultiChildLayout. If, on the other hand, the one frame delay " + "is the desired effect, for example because this is an " + "animation, consider scheduling the frame in a post-frame callback " + "using SchedulerBinding.addPostFrameCallback or " + "using an AnimationController to trigger the animation.") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        ensureVisualUpdate();
    }

    public virtual Element? rootElement => this._rootElement;
    public virtual Element? renderViewElement => this.rootElement;
    public virtual bool framesEnabled => DartRuntimePrimitives.ConvertValue<bool>((base.framesEnabled && this._readyToProduceFrames));
    public virtual Widget wrapWithDefaultView(Widget rootWidget)
    {
        if ((this.platformDispatcher.implicitView is null))
        {
            throw new InvalidOperationException("The app requested a view, but the platform did not provide one.\n" + "This is likely because the app called `runApp` to render its root " + "widget, which expects the platform to provide a default view to " + "render into (the \"implicit\" view).\n" + "However, the platform likely has multi-view mode enabled, which does " + "not create this default \"implicit\" view.\n" + "Try using `runWidget` instead of `runApp` to start your app.\n" + "`runWidget` allows you to provide a `View` widget, without requiring " + "a default view." + $"{((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? "\nSee: https://flutter.dev/to/web-multiview-runwidget" : ""))}");
        }
        return ((Widget)(object?)new View(view: this.platformDispatcher.implicitView!, deprecatedDoNotUseWillBeRemovedWithoutNoticePipelineOwner: this.pipelineOwner, deprecatedDoNotUseWillBeRemovedWithoutNoticeRenderView: this.renderView, child: rootWidget));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void scheduleAttachRootWidget(Widget rootWidget)
    {
        global::Doroti.Runtime.Timer.run((() =>
        {
            attachRootWidget(rootWidget);
        }));
    }

    public virtual void attachRootWidget(Widget rootWidget)
    {
        attachToBuildOwner(new RootWidget(debugShortDescription: "[root]", child: rootWidget));
    }

    public virtual void attachToBuildOwner(RootWidget widget)
    {
        var isBootstrapFrame__63249 = (this.rootElement is null);
        this._readyToProduceFrames = true;
        this._rootElement = DartRuntimePrimitives.ConvertValue<Element>(widget.attach(this.buildOwner!, ((RootElement?)(object?)this.rootElement)!));
        if (isBootstrapFrame__63249)
        {
            global::Doroti.Framework.Scheduler.SchedulerBinding.instance.ensureVisualUpdate();
        }
    }

    public virtual bool isRootWidgetAttached => DartRuntimePrimitives.ConvertValue<bool>((this._rootElement is not null));
    public virtual global::Doroti.Ui.Locale? computePlatformResolvedLocale(List<Locale> supportedLocales)
    {
        return this.platformDispatcher.computePlatformResolvedLocale(supportedLocales);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual WindowingOwnerIo windowingOwner
    {
        get
        {
            if (!global::Doroti.Framework.Foundation._featuresLibrary.isWindowingEnabled)
            {
                throw new NotSupportedException("Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n");
            }
            return this._windowingOwner;
            return default!;
        }
        set
        {
            var owner = value;
            if (!global::Doroti.Framework.Foundation._featuresLibrary.isWindowingEnabled)
            {
                throw new NotSupportedException("Windowing APIs are not enabled.\n\nWindowing APIs are currently experimental. Do not use windowing APIs in\nproduction applications or plugins published to pub.dev.\n\nTo try experimental windowing APIs:\n1. Switch to Flutter's main release channel.\n2. Turn on the windowing feature flag.\n\nSee: https://github.com/flutter/flutter/issues/30701.\n");
            }
            this._windowingOwner = owner;
        }
    }
}

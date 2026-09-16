// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/binding.dart
using Doroti.Runtime;
using Doroti.Ui;
using SemanticsBinding = global::Doroti.Framework.Semantics.SemanticsBinding;

namespace Doroti.Framework.Rendering;

public interface RendererBinding
{
    public static RendererBinding? _instance = default;
    PipelineManifold _manifold { get; }
    MouseTracker? _mouseTracker { get; set; }
    PipelineOwner pipelineOwner { get; }
    RenderView renderView { get; }
    PipelineOwner _rootPipelineOwner { get; set; }
    DartMap<object, RenderView> _viewIdToRenderView { get; }
    bool _debugMouseTrackerUpdateScheduled { get; set; }
    long _firstFrameDeferredCount { get; set; }
    bool _firstFrameSent { get; set; }

    public static RendererBinding instance
    {
        get => BindingBase.checkInstance(_instance);
    }
    public MouseTracker mouseTracker { get; }
    public PipelineOwner createRootPipelineOwner();
    public PipelineOwner rootPipelineOwner { get; }
    public IEnumerable<RenderView> renderViews { get; }
    public void addRenderView(RenderView view);
    public void removeRenderView(RenderView view);
    public ViewConfiguration createViewConfigurationFor(RenderView renderView);
    public global::Doroti.Ui.SceneBuilder createSceneBuilder();
    public global::Doroti.Ui.PictureRecorder createPictureRecorder();
    public global::Doroti.Ui.Canvas createCanvas(PictureRecorder recorder);
    public void handleMetricsChanged();
    public void handleTextScaleFactorChanged();
    public void handlePlatformBrightnessChanged();
    public void initMouseTracker(MouseTracker? tracker = null);
    public void dispatchEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestResult? hitTestResult);
    public void performSemanticsAction(SemanticsActionEvent action);
    public Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId);
    public void _handleWebFirstFrame(Duration __unused0);
    public void _handlePersistentFrameCallback(Duration timeStamp);
    public void _scheduleMouseTrackerUpdate();
    public bool sendFramesToEngine { get; }
    public void deferFirstFrame();
    public void allowFirstFrame();
    public void resetFirstFrameSent();
    public void drawFrame();
    public void hitTestInView(HitTestResult result, Offset position, long viewId);
    public Future _forceRepaint();
    void ensureVisualUpdate();
    bool semanticsEnabled { get; }
    void removeSemanticsEnabledListener(Action listener);
}

public static partial class BindingLibrary
{
    internal static string _debugCollectRenderTrees()
    {
        if (RendererBinding.instance.renderViews.Count() == 0)
        {
            return "No render tree root was added to the binding.";
        }
        return string.Join("\n\n", new List<string>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpRenderTree()
    {
        PrintLibrary.debugPrint(_debugCollectRenderTrees());
    }
}

public static partial class BindingLibrary
{
    internal static string _debugCollectLayerTrees()
    {
        if (RendererBinding.instance.renderViews.Count() == 0)
        {
            return "No render tree root was added to the binding.";
        }
        return string.Join("\n\n", new List<string>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpLayerTree()
    {
        PrintLibrary.debugPrint(_debugCollectLayerTrees());
    }
}

public static partial class BindingLibrary
{
    internal static string _debugCollectSemanticsTrees(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder childOrder)
    {
        if (RendererBinding.instance.renderViews.Count() == 0)
        {
            return "No render tree root was added to the binding.";
        }
        var explanation = "For performance reasons, the framework only generates semantics when asked to do so by the platform.\n" + "Usually, platforms only ask for semantics when assistive technologies (like screen readers) are running.\n" + "To generate semantics, try turning on an assistive technology (like VoiceOver or TalkBack) on your device.";
        var trees = new List<string>();
        var printedExplanation = false;
        foreach (RenderView renderView in RendererBinding.instance.renderViews)
        {
            string? tree = renderView.debugSemantics?.toStringDeep(childOrder: childOrder);
            if (tree is not null)
            {
                trees.Add(tree);
            }
            else
            {
                var message = $"Semantics not generated for {renderView}.";
                if (!printedExplanation)
                {
                    printedExplanation = true;
                    message = $"{message}\n{explanation}";
                }
                trees.Add(message);
            }
        }
        return string.Join("\n\n", trees);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpSemanticsTree(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder childOrder = DebugSemanticsDumpOrder.traversalOrder)
    {
        PrintLibrary.debugPrint(_debugCollectSemanticsTrees(childOrder));
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpPipelineOwnerTree()
    {
        PrintLibrary.debugPrint(RendererBinding.instance.rootPipelineOwner.toStringDeep());
    }
}

public class RenderingFlutterBinding : global::Doroti.Framework.Gestures.GestureBinding, global::Doroti.Framework.Semantics.SemanticsBinding, global::Doroti.Framework.Painting.PaintingBinding, RendererBinding
{
    private bool __late__semanticsEnabled_initialized;
    private ValueNotifier<bool> __late__semanticsEnabled = default!;
    public virtual ValueNotifier<bool> _semanticsEnabled
    {
        get
        {
            if (!__late__semanticsEnabled_initialized)
            {
                __late__semanticsEnabled = new ValueNotifier<bool>(platformDispatcher.semanticsEnabled);
                __late__semanticsEnabled_initialized = true;
            }
            return __late__semanticsEnabled;
        }
    }
    public virtual ObserverList<Action<SemanticsActionEvent>> _semanticsActionListeners { get; set; } = new ObserverList<Action<global::Doroti.Ui.SemanticsActionEvent>>();
    public virtual long _outstandingHandles { get; set; } = 0L;
    public virtual SemanticsHandle? _semanticsHandle { get; set; } = default;
    public virtual AccessibilityFeatures _accessibilityFeatures { get; set; } = default!;
    public virtual ImageCache _imageCache { get; set; } = default!;
    public virtual _SystemFontsNotifier__binding _systemFonts { get; set; } = new _SystemFontsNotifier__binding();
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
                __late_pipelineOwner = new PipelineOwner(onSemanticsOwnerCreated: () =>
                {
                    ((RenderView?)(object?)pipelineOwner.rootNode)!?.scheduleInitialSemantics();
                }, onSemanticsUpdate: (update) =>
                {
                    ((RenderView?)(object?)pipelineOwner.rootNode)!?.updateSemantics(update);
                }, onSemanticsOwnerDisposed: () =>
                {
                    ((RenderView?)(object?)pipelineOwner.rootNode)!?.clearSemantics();
                });
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
                __late_renderView = new _ReusableRenderView__binding(view: platformDispatcher.implicitView!);
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

    public static RendererBinding ensureInitialized()
    {
        if (RendererBinding._instance is null)
        {
            new RenderingFlutterBinding();
        }
        return RendererBinding.instance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    protected override void initInstances()
    {
        base.initInstances();
        SemanticsBinding._instance = this;
        _accessibilityFeatures = platformDispatcher.accessibilityFeatures;
        ((Func<PlatformDispatcher>)(() =>
{
    var __cascade = platformDispatcher;
    __cascade.onSemanticsEnabledChanged = _handleSemanticsEnabledChanged;
    __cascade.onSemanticsActionEvent = _handleSemanticsActionEvent;
    __cascade.onAccessibilityFeaturesChanged = () =>
    {
        if (Equals(SchedulerBinding.instance.schedulerPhase, SchedulerPhase.persistentCallbacks))
        {
            SchedulerBinding.instance.addPostFrameCallback((duration) =>
            {
                handleAccessibilityFeaturesChanged();
            }, debugLabel: "SemanticsBinding.handleAccessibilityFeaturesChanged");
        }
        else
        {
            handleAccessibilityFeaturesChanged();
        }
    };
    return __cascade;
}))();
        _handleSemanticsEnabledChanged();
        addSemanticsEnabledListener(_handleFrameworkSemanticsEnabledChanged);
        if (semanticsEnabled)
        {
            _handleFrameworkSemanticsEnabledChanged();
        }
    }

    public virtual bool semanticsEnabled
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _semanticsEnabled.value == _outstandingHandles > 0L);
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

    public virtual Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId) => null;
    public virtual long debugOutstandingSemanticsHandles => _outstandingHandles;
    public virtual SemanticsHandle ensureSemantics()
    {
        DartRuntimePrimitives.Assert(() => _outstandingHandles >= 0L);
        _outstandingHandles++;
        DartRuntimePrimitives.Assert(() => _outstandingHandles > 0L);
        _semanticsEnabled.value = true;
        return new SemanticsHandle(_didDisposeSemanticsHandle);
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
        global::Doroti.Ui.SemanticsActionEvent decodedAction = (argumentsLocal is ByteData) ? action.copyWith(arguments: new StandardMessageCodec().decodeMessage((ByteData)argumentsLocal)) : action;
        List<Action<global::Doroti.Ui.SemanticsActionEvent>> localListeners = _semanticsActionListeners.ToList();
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

    public virtual AccessibilityFeatures accessibilityFeatures => _accessibilityFeatures;
    public virtual void handleAccessibilityFeaturesChanged()
    {
        _accessibilityFeatures = platformDispatcher.accessibilityFeatures;
    }

    public virtual SemanticsUpdateBuilder createSemanticsUpdateBuilder()
    {
        return new global::Doroti.Ui.SemanticsUpdateBuilder();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool disableAnimations
    {
        get
        {
            bool value = _accessibilityFeatures.disableAnimations;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (Semantics.DebugLibrary.debugSemanticsDisableAnimations is not null)
                    {
                        value = DartRuntimePrimitives.RequireValue(Semantics.DebugLibrary.debugSemanticsDisableAnimations);
                    }
                    return true;
                });
            return value;
        }
    }
    public virtual ImageCache imageCache => _imageCache;
    public virtual ImageCache createImageCache() => new ImageCache();
    public virtual Future<Codec> instantiateImageCodecFromBuffer(ImmutableBuffer buffer, long? cacheWidth = null, long? cacheHeight = null, bool allowUpscaling = false)
    {
        DartRuntimePrimitives.Assert(() => (cacheWidth is null) || (cacheWidth > 0L));
        DartRuntimePrimitives.Assert(() => (cacheHeight is null) || (cacheHeight > 0L));
        return Dart_uiLibrary.instantiateImageCodecFromBuffer(buffer, targetWidth: cacheWidth, targetHeight: cacheHeight, allowUpscaling: allowUpscaling);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<Codec> instantiateImageCodecWithSize(ImmutableBuffer buffer, Func<long, long, TargetImageSize>? getTargetSize = null)
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

    public virtual Listenable systemFonts => _systemFonts;
    public async override Future handleSystemMessage(object systemMessage)
    {
        await base.handleSystemMessage(systemMessage);
        var message = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)systemMessage);
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

    protected override void initServiceExtensions()
    {
        base.initServiceExtensions();
        DartRuntimePrimitives.Assert(() =>
            {
                registerBoolServiceExtension(name: RenderingServiceExtensions.invertOversizedImages.ToString(), getter: async () => Painting.DebugLibrary.debugInvertOversizedImages, setter: (Func<bool, Future>)(async (value) =>
                {
                    if (Painting.DebugLibrary.debugInvertOversizedImages != value)
                    {
                        Painting.DebugLibrary.debugInvertOversizedImages = value;
                        DartAsyncRuntime.unawaited(_forceRepaint());
                    }
                }));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugPaint.ToString(), getter: async () => DebugLibrary.debugPaintSizeEnabled, setter: (Func<bool, Future>)(async (value) =>
                {
                    if (DebugLibrary.debugPaintSizeEnabled == value)
                    {
                        return;
                    }
                    DebugLibrary.debugPaintSizeEnabled = value;
                    DartAsyncRuntime.unawaited(_forceRepaint());
                }));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugPaintBaselinesEnabled.ToString(), getter: async () => DebugLibrary.debugPaintBaselinesEnabled, setter: (Func<bool, Future>)(async (value) =>
                {
                    if (DebugLibrary.debugPaintBaselinesEnabled == value)
                    {
                        return;
                    }
                    DebugLibrary.debugPaintBaselinesEnabled = value;
                    DartAsyncRuntime.unawaited(_forceRepaint());
                }));
                registerBoolServiceExtension(name: RenderingServiceExtensions.repaintRainbow.ToString(), getter: async () => DebugLibrary.debugRepaintRainbowEnabled, setter: (Func<bool, Future>)(async (value) =>
                {
                    bool repaint = DebugLibrary.debugRepaintRainbowEnabled && !value;
                    DebugLibrary.debugRepaintRainbowEnabled = value;
                    if (repaint)
                    {
                        DartAsyncRuntime.unawaited(_forceRepaint());
                    }
                }));
                registerServiceExtension(name: RenderingServiceExtensions.debugDumpLayerTree.ToString(), callback: async (parameters) =>
                {
                    return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectLayerTrees() };
                });
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisableClipLayers.ToString(), getter: async () => DebugLibrary.debugDisableClipLayers, setter: (Func<bool, Future>)(async (value) =>
                {
                    if (DebugLibrary.debugDisableClipLayers == value)
                    {
                        return;
                    }
                    DebugLibrary.debugDisableClipLayers = value;
                    DartAsyncRuntime.unawaited(_forceRepaint());
                }));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisablePhysicalShapeLayers.ToString(), getter: async () => DebugLibrary.debugDisablePhysicalShapeLayers, setter: (Func<bool, Future>)(async (value) =>
                {
                    if (DebugLibrary.debugDisablePhysicalShapeLayers == value)
                    {
                        return;
                    }
                    DebugLibrary.debugDisablePhysicalShapeLayers = value;
                    DartAsyncRuntime.unawaited(_forceRepaint());
                }));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisableOpacityLayers.ToString(), getter: async () => DebugLibrary.debugDisableOpacityLayers, setter: (Func<bool, Future>)(async (value) =>
                {
                    if (DebugLibrary.debugDisableOpacityLayers == value)
                    {
                        return;
                    }
                    DebugLibrary.debugDisableOpacityLayers = value;
                    DartAsyncRuntime.unawaited(_forceRepaint());
                }));
                return true;
            });
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpRenderTree.ToString(), callback: async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectRenderTrees() };
            });
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpSemanticsTreeInTraversalOrder.ToString(), callback: async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectSemanticsTrees(DebugSemanticsDumpOrder.traversalOrder) };
            });
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpSemanticsTreeInInverseHitTestOrder.ToString(), callback: async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectSemanticsTrees(DebugSemanticsDumpOrder.inverseHitTest) };
            });
            registerBoolServiceExtension(name: RenderingServiceExtensions.profileRenderObjectPaints.ToString(), getter: async () => DebugLibrary.debugProfilePaintsEnabled, setter: (Func<bool, Future>)(async (value) =>
            {
                if (DebugLibrary.debugProfilePaintsEnabled != value)
                {
                    DebugLibrary.debugProfilePaintsEnabled = value;
                }
            }));
            registerBoolServiceExtension(name: RenderingServiceExtensions.profileRenderObjectLayouts.ToString(), getter: async () => DebugLibrary.debugProfileLayoutsEnabled, setter: (Func<bool, Future>)(async (value) =>
            {
                if (DebugLibrary.debugProfileLayoutsEnabled != value)
                {
                    DebugLibrary.debugProfileLayoutsEnabled = value;
                }
            }));
        }
    }

    public virtual MouseTracker mouseTracker => _mouseTracker!;
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
        DartRuntimePrimitives.Assert(() => Equals(_viewIdToRenderView.GetValueOrDefault(viewIdLocal), view));
        _viewIdToRenderView.remove(viewIdLocal);
    }

    public virtual ViewConfiguration createViewConfigurationFor(RenderView renderView)
    {
        return ViewConfiguration.CreateFromView(renderView.flutterView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SceneBuilder createSceneBuilder() => new global::Doroti.Ui.SceneBuilder();
    public virtual PictureRecorder createPictureRecorder() => new global::Doroti.Ui.PictureRecorder();
    public virtual Canvas createCanvas(PictureRecorder recorder) => new global::Doroti.Ui.Canvas(recorder);
    public virtual void handleMetricsChanged()
    {
        var forceFrame = false;
        foreach (RenderView view in renderViews)
        {
            forceFrame = forceFrame || (view.child is not null);
            view.configuration = createViewConfigurationFor(view);
        }
        if (forceFrame)
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
        _mouseTracker?.dispose();
        _mouseTracker = tracker ?? new MouseTracker((position, viewId) =>
        {
            var result = new HitTestResult();
            hitTestInView(result, position, viewId);
            return result;
        });
    }

    public override void dispatchEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestResult? hitTestResult)
    {
        _mouseTracker!.updateWithEvent(@event, (@event is global::Doroti.Framework.Gestures.PointerMoveEvent) ? null : hitTestResult);
        base.dispatchEvent(@event, hitTestResult);
    }

    public virtual void performSemanticsAction(SemanticsActionEvent action)
    {
        _viewIdToRenderView.GetValueOrDefault(action.viewId)?.owner?.semanticsOwner?.performAction(action.nodeId, action.type, action.arguments);
    }

    public virtual void _handleWebFirstFrame(Duration __unused0)
    {
        DartRuntimePrimitives.Assert(() => Foundation.ConstantsLibrary.kIsWeb);
        var methodChannel = new MethodChannel("flutter/service_worker");
        _ = methodChannel.invokeMethod<object?>("first-frame").then((_) =>
        {
        }, onError: (error, stack) =>
        {
            FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stack, library: "rendering library", context: new ErrorDescription("while sending the first-frame event")));
        });
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
            });
        SchedulerBinding.instance.addPostFrameCallback((duration) =>
        {
            DartRuntimePrimitives.Assert(() => _debugMouseTrackerUpdateScheduled);
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugMouseTrackerUpdateScheduled = false;
                    return true;
                });
            _mouseTracker!.updateAllDevices();
        }, debugLabel: "RendererBinding.mouseTrackerUpdate");
    }

    public virtual bool sendFramesToEngine => _firstFrameSent || (_firstFrameDeferredCount == 0L);
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
        rootPipelineOwner.flushLayout();
        rootPipelineOwner.flushCompositingBits();
        rootPipelineOwner.flushPaint();
        if (sendFramesToEngine)
        {
            foreach (RenderView renderView in renderViews)
            {
                renderView.compositeFrame();
            }
            rootPipelineOwner.flushSemantics();
            _firstFrameSent = true;
        }
    }

    protected async override Task performReassemble()
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

    public void handleMetricsChanged(DorotiView _) => handleMetricsChanged();
}

internal class _BindingPipelineManifold__binding : ChangeNotifier, PipelineManifold
{
    internal virtual RendererBinding _binding { get; private set; } = default!;

    internal _BindingPipelineManifold__binding(RendererBinding _binding)
    {
        this._binding = _binding;
    }

    public virtual void requestVisualUpdate()
    {
        _binding.ensureVisualUpdate();
    }

    public virtual bool semanticsEnabled => _binding.semanticsEnabled;
    public override void dispose()
    {
        _binding.removeSemanticsEnabledListener(notifyListeners);
        base.dispose();
    }

}

internal class _DefaultRootPipelineOwner__binding : PipelineOwner
{
    internal _DefaultRootPipelineOwner__binding() : base(onSemanticsUpdate: _onSemanticsUpdate)
    {
    }

    public override RenderObject? rootNode
    {
        set
        {
            var _ = value;
            DartRuntimePrimitives.Assert(() =>
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Cannot set a rootNode on the default root pipeline owner."), new ErrorDescription("By default, the RendererBinding.rootPipelineOwner is not configured " + "to manage a root node because this pipeline owner does not define a " + "proper onSemanticsUpdate callback to handle semantics for that node."), new ErrorHint("Typically, the root pipeline owner does not manage a root node. " + "Instead, properly configured child pipeline owners (which do manage " + "root nodes) are added to it. Alternatively, if you do want to set a " + "root node for the root pipeline owner, override " + "RendererBinding.createRootPipelineOwner to create a " + "pipeline owner that is configured to properly handle semantics for " + "the provided root node.") });
                });
        }
    }
    internal static void _onSemanticsUpdate(SemanticsUpdate __unused0)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

}

internal class _ReusableRenderView__binding : RenderView
{
    internal virtual bool _initialFramePrepared { get; set; } = false;

    internal _ReusableRenderView__binding(DorotiView view) : base(view: view)
    {
    }

    public override void prepareInitialFrame()
    {
        if (_initialFramePrepared)
        {
            return;
        }
        base.prepareInitialFrame();
        _initialFramePrepared = true;
    }

    public override void scheduleInitialSemantics()
    {
        clearSemantics();
        base.scheduleInitialSemantics();
    }

    public override void dispose()
    {
        child = null;
    }

}


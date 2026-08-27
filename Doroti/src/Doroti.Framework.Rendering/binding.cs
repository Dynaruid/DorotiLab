// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/binding.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
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
        if ((RendererBinding.instance.renderViews.Count() == 0))
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
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(BindingLibrary._debugCollectRenderTrees());
    }
}

public static partial class BindingLibrary
{
    internal static string _debugCollectLayerTrees()
    {
        if ((RendererBinding.instance.renderViews.Count() == 0))
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
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(BindingLibrary._debugCollectLayerTrees());
    }
}

public static partial class BindingLibrary
{
    internal static string _debugCollectSemanticsTrees(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder childOrder)
    {
        if ((RendererBinding.instance.renderViews.Count() == 0))
        {
            return "No render tree root was added to the binding.";
        }
        var explanation = "For performance reasons, the framework only generates semantics when asked to do so by the platform.\n" + "Usually, platforms only ask for semantics when assistive technologies (like screen readers) are running.\n" + "To generate semantics, try turning on an assistive technology (like VoiceOver or TalkBack) on your device.";
        var trees = new List<string>();
        var printedExplanation = false;
        foreach (RenderView renderView in RendererBinding.instance.renderViews)
        {
            string? tree = renderView.debugSemantics?.toStringDeep(childOrder: childOrder);
            if ((tree is not null))
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
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(BindingLibrary._debugCollectSemanticsTrees(childOrder));
    }
}

public static partial class BindingLibrary
{
    public static void debugDumpPipelineOwnerTree()
    {
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(RendererBinding.instance.rootPipelineOwner.toStringDeep());
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
                __late_pipelineOwner = new PipelineOwner(onSemanticsOwnerCreated: (() =>
                {
                    (((RenderView?)(object?)((PipelineOwner)this.pipelineOwner).rootNode)!)?.scheduleInitialSemantics();
                }), onSemanticsUpdate: ((update) =>
                {
                    (((RenderView?)(object?)((PipelineOwner)this.pipelineOwner).rootNode)!)?.updateSemantics(update);
                }), onSemanticsOwnerDisposed: (() =>
                {
                    (((RenderView?)(object?)((PipelineOwner)this.pipelineOwner).rootNode)!)?.clearSemantics();
                }));
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
        if ((RendererBinding._instance is null))
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
        this._accessibilityFeatures = platformDispatcher.accessibilityFeatures;
        ((Func<PlatformDispatcher>)(() =>
{
    var __cascade = platformDispatcher;
    __cascade.onSemanticsEnabledChanged = this._handleSemanticsEnabledChanged;
    __cascade.onSemanticsActionEvent = this._handleSemanticsActionEvent;
    __cascade.onAccessibilityFeaturesChanged = (() =>
    {
        if ((object.Equals(SchedulerBinding.instance.schedulerPhase, SchedulerPhase.persistentCallbacks)))
        {
            SchedulerBinding.instance.addPostFrameCallback(((duration) =>
            {
                handleAccessibilityFeaturesChanged();
            }), debugLabel: "SemanticsBinding.handleAccessibilityFeaturesChanged");
        }
        else
        {
            handleAccessibilityFeaturesChanged();
        }
    });
    return __cascade;
}))();
        _handleSemanticsEnabledChanged();
        addSemanticsEnabledListener((Action)this._handleFrameworkSemanticsEnabledChanged);
        if (this.semanticsEnabled)
        {
            _handleFrameworkSemanticsEnabledChanged();
        }
    }

    public virtual bool semanticsEnabled
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this._semanticsEnabled.value == ((this._outstandingHandles > 0L))));
            return this._semanticsEnabled.value;
            return default!;
        }
    }
    public virtual void addSemanticsEnabledListener(Action listener)
    {
        this._semanticsEnabled.addListener(listener);
    }

    public virtual void removeSemanticsEnabledListener(Action listener)
    {
        this._semanticsEnabled.removeListener(listener);
    }

    public virtual void addSemanticsActionListener(Action<SemanticsActionEvent> listener)
    {
        this._semanticsActionListeners.add(listener);
    }

    public virtual void removeSemanticsActionListener(Action<SemanticsActionEvent> listener)
    {
        this._semanticsActionListeners.remove(listener);
    }

    public virtual Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId) => null;
    public virtual long debugOutstandingSemanticsHandles => this._outstandingHandles;
    public virtual SemanticsHandle ensureSemantics()
    {
        DartRuntimePrimitives.Assert(() => (this._outstandingHandles >= 0L));
        this._outstandingHandles++;
        DartRuntimePrimitives.Assert(() => (this._outstandingHandles > 0L));
        this._semanticsEnabled.value = true;
        return new SemanticsHandle(this._didDisposeSemanticsHandle);
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
        if (platformDispatcher.semanticsEnabled)
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
        object? argumentsLocal = action.arguments;
        global::Doroti.Ui.SemanticsActionEvent decodedAction = ((argumentsLocal is ByteData) ? action.copyWith(arguments: new StandardMessageCodec().decodeMessage(((ByteData)argumentsLocal))) : action);
        List<Action<global::Doroti.Ui.SemanticsActionEvent>> localListeners = this._semanticsActionListeners.ToList();
        foreach (var listener in localListeners)
        {
            if (this._semanticsActionListeners.contains(listener))
            {
                listener(decodedAction);
            }
        }
        performSemanticsAction(decodedAction);
    }

    public virtual void _handleFrameworkSemanticsEnabledChanged()
    {
        platformDispatcher.setSemanticsTreeEnabled(this.semanticsEnabled);
    }

    public virtual AccessibilityFeatures accessibilityFeatures => this._accessibilityFeatures;
    public virtual void handleAccessibilityFeaturesChanged()
    {
        this._accessibilityFeatures = platformDispatcher.accessibilityFeatures;
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
            bool value = this._accessibilityFeatures.disableAnimations;
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((global::Doroti.Framework.Semantics.DebugLibrary.debugSemanticsDisableAnimations is not null))
                    {
                        value = DartRuntimePrimitives.RequireValue(global::Doroti.Framework.Semantics.DebugLibrary.debugSemanticsDisableAnimations);
                    }
                    return true;
                });
            return value;
            return default!;
        }
    }
    public virtual ImageCache imageCache => this._imageCache;
    public virtual ImageCache createImageCache() => new ImageCache();
    public virtual Future<Codec> instantiateImageCodecFromBuffer(ImmutableBuffer buffer, long? cacheWidth = null, long? cacheHeight = null, bool allowUpscaling = false)
    {
        DartRuntimePrimitives.Assert(() => ((cacheWidth is null) || (cacheWidth > 0L)));
        DartRuntimePrimitives.Assert(() => ((cacheHeight is null) || (cacheHeight > 0L)));
        return Dart_uiLibrary.instantiateImageCodecFromBuffer(buffer, targetWidth: cacheWidth, targetHeight: cacheHeight, allowUpscaling: allowUpscaling);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<Codec> instantiateImageCodecWithSize(ImmutableBuffer buffer, Func<long, long, TargetImageSize>? getTargetSize = null)
    {
        return Dart_uiLibrary.instantiateImageCodecWithSize(buffer, getTargetSize: getTargetSize);
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

    public virtual Listenable systemFonts => this._systemFonts;
    public async virtual Future handleSystemMessage(object systemMessage)
    {
        await base.handleSystemMessage(systemMessage);
        var message = DartRuntimePrimitives.ConvertMap<string, object>((System.Collections.IDictionary)systemMessage);
        var @type = ((string?)(object?)message.GetValueOrDefault("type"))!;
        switch (@type)
        {
            case "fontsChange":
                {
                    this._systemFonts.notifyListeners();
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
                registerBoolServiceExtension(name: RenderingServiceExtensions.invertOversizedImages.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Painting.DebugLibrary.debugInvertOversizedImages)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Painting.DebugLibrary.debugInvertOversizedImages != value))
                    {
                        global::Doroti.Framework.Painting.DebugLibrary.debugInvertOversizedImages = value;
                        global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    }
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugPaint.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugPaintBaselinesEnabled.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugPaintBaselinesEnabled = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.repaintRainbow.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    bool repaint = (global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled && !value);
                    global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled = value;
                    if (repaint)
                    {
                        global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                    }
                })));
                registerServiceExtension(name: RenderingServiceExtensions.debugDumpLayerTree.ToString(), callback: (async (parameters) =>
                {
                    return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectLayerTrees() };
                }));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisableClipLayers.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugDisableClipLayers = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisablePhysicalShapeLayers.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugDisablePhysicalShapeLayers)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugDisablePhysicalShapeLayers == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugDisablePhysicalShapeLayers = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                })));
                registerBoolServiceExtension(name: RenderingServiceExtensions.debugDisableOpacityLayers.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers)), setter: ((Func<bool, Future>)(async (value) =>
                {
                    if ((global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers == value))
                    {
                        return;
                    }
                    global::Doroti.Framework.Rendering.DebugLibrary.debugDisableOpacityLayers = value;
                    global::Doroti.Runtime.DartAsyncRuntime.unawaited(_forceRepaint());
                })));
                return true;
            });
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpRenderTree.ToString(), callback: (async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectRenderTrees() };
            }));
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpSemanticsTreeInTraversalOrder.ToString(), callback: (async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectSemanticsTrees(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder.traversalOrder) };
            }));
            registerServiceExtension(name: RenderingServiceExtensions.debugDumpSemanticsTreeInInverseHitTestOrder.ToString(), callback: (async (parameters) =>
            {
                return new DartMap<string, object> { ["data"] = BindingLibrary._debugCollectSemanticsTrees(global::Doroti.Framework.Semantics.DebugSemanticsDumpOrder.inverseHitTest) };
            }));
            registerBoolServiceExtension(name: RenderingServiceExtensions.profileRenderObjectPaints.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled)), setter: ((Func<bool, Future>)(async (value) =>
            {
                if ((global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled != value))
                {
                    global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled = value;
                }
            })));
            registerBoolServiceExtension(name: RenderingServiceExtensions.profileRenderObjectLayouts.ToString(), getter: ((Func<Future<bool>>)(async () => global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled)), setter: ((Func<bool, Future>)(async (value) =>
            {
                if ((global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled != value))
                {
                    global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled = value;
                }
            })));
        }
    }

    public virtual MouseTracker mouseTracker => this._mouseTracker!;
    public virtual PipelineOwner createRootPipelineOwner()
    {
        return new _DefaultRootPipelineOwner__binding();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PipelineOwner rootPipelineOwner => this._rootPipelineOwner;
    public virtual IEnumerable<RenderView> renderViews => this._viewIdToRenderView.Values;
    public virtual void addRenderView(RenderView view)
    {
        object viewIdLocal = checked((long)((RenderView)view).flutterView.viewId);
        DartRuntimePrimitives.Assert(() => !this._viewIdToRenderView.containsValue(view));
        DartRuntimePrimitives.Assert(() => !this._viewIdToRenderView.ContainsKey(viewIdLocal));
        this._viewIdToRenderView[viewIdLocal] = view;
        view.configuration = createViewConfigurationFor(view);
    }

    public virtual void removeRenderView(RenderView view)
    {
        object viewIdLocal = checked((long)((RenderView)view).flutterView.viewId);
        DartRuntimePrimitives.Assert(() => (object.Equals(this._viewIdToRenderView.GetValueOrDefault(viewIdLocal), view)));
        this._viewIdToRenderView.remove(viewIdLocal);
    }

    public virtual ViewConfiguration createViewConfigurationFor(RenderView renderView)
    {
        return ViewConfiguration.CreateFromView(((RenderView)renderView).flutterView);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SceneBuilder createSceneBuilder() => new global::Doroti.Ui.SceneBuilder();
    public virtual PictureRecorder createPictureRecorder() => new global::Doroti.Ui.PictureRecorder();
    public virtual Canvas createCanvas(PictureRecorder recorder) => new global::Doroti.Ui.Canvas(recorder);
    public virtual void handleMetricsChanged()
    {
        var forceFrame = false;
        foreach (RenderView view in this.renderViews)
        {
            forceFrame = (forceFrame || (view.child is not null));
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
        this._mouseTracker?.dispose();
        this._mouseTracker = (tracker ?? new MouseTracker(((position, viewId) =>
        {
            var result = new HitTestResult();
            hitTestInView(result, position, viewId);
            return result;
            return default;
        })));
    }

    public virtual void dispatchEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestResult? hitTestResult)
    {
        this._mouseTracker!.updateWithEvent(@event, ((@event is global::Doroti.Framework.Gestures.PointerMoveEvent) ? null : hitTestResult));
        base.dispatchEvent(@event, hitTestResult);
    }

    public virtual void performSemanticsAction(SemanticsActionEvent action)
    {
        this._viewIdToRenderView.GetValueOrDefault(action.viewId)?.owner?.semanticsOwner?.performAction(action.nodeId, action.type, action.arguments);
    }

    public virtual void _handleWebFirstFrame(Duration __unused0)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb);
        var methodChannel = new MethodChannel("flutter/service_worker");
        _ = methodChannel.invokeMethod<object?>("first-frame").then(((_) =>
        {
        }), onError: ((error, stack) =>
        {
            FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stack, library: "rendering library", context: new ErrorDescription("while sending the first-frame event")));
        }));
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
            });
        SchedulerBinding.instance.addPostFrameCallback(((duration) =>
        {
            DartRuntimePrimitives.Assert(() => this._debugMouseTrackerUpdateScheduled);
            DartRuntimePrimitives.Assert(() =>
                {
                    this._debugMouseTrackerUpdateScheduled = false;
                    return true;
                });
            this._mouseTracker!.updateAllDevices();
        }), debugLabel: "RendererBinding.mouseTrackerUpdate");
    }

    public virtual bool sendFramesToEngine => (this._firstFrameSent || (this._firstFrameDeferredCount == 0L));
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
        this.rootPipelineOwner.flushLayout();
        this.rootPipelineOwner.flushCompositingBits();
        this.rootPipelineOwner.flushPaint();
        if (this.sendFramesToEngine)
        {
            foreach (RenderView renderView in this.renderViews)
            {
                renderView.compositeFrame();
            }
            this.rootPipelineOwner.flushSemantics();
            this._firstFrameSent = true;
        }
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
            foreach (RenderView renderView in this.renderViews)
            {
                renderView.reassemble();
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
        await endOfFrame;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void hitTestInView(HitTestResult result, Offset position, long viewId)
    {
        this._viewIdToRenderView.GetValueOrDefault(viewId)?.hitTest(result, position: position);
        base.hitTestInView(result, position, viewId);
    }

    public virtual Future _forceRepaint()
    {
        Action<RenderObject> visitor = default!;
        visitor = ((child) =>
        {
            child.markNeedsPaint();
            child.visitChildren((Action<RenderObject>)visitor);
        });
        foreach (RenderView renderView in this.renderViews)
        {
            renderView.visitChildren((Action<RenderObject>)visitor);
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
        this._binding.ensureVisualUpdate();
    }

    public virtual bool semanticsEnabled => this._binding.semanticsEnabled;
    public override void dispose()
    {
        this._binding.removeSemanticsEnabledListener((Action)notifyListeners);
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
        if (this._initialFramePrepared)
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


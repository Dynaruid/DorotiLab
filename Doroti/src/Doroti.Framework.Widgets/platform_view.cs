// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/platform_view.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class AndroidView : StatefulWidget
{
    public virtual string viewType { get; private set; } = default!;
    public virtual global::System.Action<long>? onPlatformViewCreated { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual TextDirection? layoutDirection { get; private set; }
    public virtual HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers { get; private set; }
    public virtual dynamic creationParams { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MessageCodec<object>? creationParamsCodec { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public AndroidView(global::Doroti.Generated.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null, dynamic creationParams = default!, global::Doroti.Generated.Framework.Services.MessageCodec<object>? creationParamsCodec = null, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.hitTestBehavior = hitTestBehavior;
        this.layoutDirection = layoutDirection;
        this.gestureRecognizers = gestureRecognizers;
        this.creationParams = creationParams;
        this.creationParamsCodec = creationParamsCodec;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(((creationParams is null) || (creationParamsCodec is not null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AndroidViewState__platform_view());
}

public abstract class _DarwinView__platform_view : StatefulWidget
{
    public virtual string viewType { get; private set; } = default!;
    public virtual global::System.Action<long>? onPlatformViewCreated { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual TextDirection? layoutDirection { get; private set; }
    public virtual dynamic creationParams { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MessageCodec<object>? creationParamsCodec { get; private set; }
    public virtual HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers { get; private set; }

    internal _DarwinView__platform_view(global::Doroti.Generated.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, dynamic creationParams = default!, global::Doroti.Generated.Framework.Services.MessageCodec<object>? creationParamsCodec = null, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null) : base(key: key)
    {
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.hitTestBehavior = hitTestBehavior;
        this.layoutDirection = layoutDirection;
        this.creationParams = creationParams;
        this.creationParamsCodec = creationParamsCodec;
        this.gestureRecognizers = gestureRecognizers;
        System.Diagnostics.Debug.Assert(((creationParams is null) || (creationParamsCodec is not null)));
    }

}

public class UiKitView : _DarwinView__platform_view
{
    public virtual global::Doroti.Generated.Framework.Services.UiKitViewGestureBlockingPolicy gestureBlockingPolicy { get; private set; } = default!;

    public UiKitView(global::Doroti.Generated.Framework.Foundation.Key? key = null, string viewType = default!, global::Doroti.Generated.Framework.Services.UiKitViewGestureBlockingPolicy gestureBlockingPolicy = global::Doroti.Generated.Framework.Services.UiKitViewGestureBlockingPolicy.fallbackToPluginDefault, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, dynamic creationParams = default!, global::Doroti.Generated.Framework.Services.MessageCodec<object>? creationParamsCodec = null, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null) : base(key: key, viewType: viewType, onPlatformViewCreated: onPlatformViewCreated, hitTestBehavior: hitTestBehavior, layoutDirection: DartRuntimePrimitives.RequireValue(layoutDirection), creationParams: (object?)creationParams, creationParamsCodec: creationParamsCodec, gestureRecognizers: gestureRecognizers)
    {
        this.gestureBlockingPolicy = gestureBlockingPolicy;
        System.Diagnostics.Debug.Assert(((creationParams is null) || (creationParamsCodec is not null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _UiKitViewState__platform_view());
}

public class AppKitView : _DarwinView__platform_view
{
    public AppKitView(global::Doroti.Generated.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, dynamic creationParams = default!, global::Doroti.Generated.Framework.Services.MessageCodec<object>? creationParamsCodec = null, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null) : base(key: key, viewType: viewType, onPlatformViewCreated: onPlatformViewCreated, hitTestBehavior: hitTestBehavior, layoutDirection: DartRuntimePrimitives.RequireValue(layoutDirection), creationParams: (object?)creationParams, creationParamsCodec: creationParamsCodec, gestureRecognizers: gestureRecognizers)
    {
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AppKitViewState__platform_view());
}

public delegate void ElementCreatedCallback(object element);

public class HtmlElementView : StatelessWidget
{
    public virtual string viewType { get; private set; } = default!;
    public virtual global::System.Action<long>? onPlatformViewCreated { get; private set; }
    public virtual object? creationParams { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;

    public HtmlElementView(global::Doroti.Generated.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, object? creationParams = null, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.opaque) : base(key: key)
    {
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.creationParams = creationParams;
        this.hitTestBehavior = hitTestBehavior;
    }

    public static HtmlElementView CreateFromTagName(global::Doroti.Generated.Framework.Foundation.Key? key = null, string tagName = default!, bool isVisible = true, global::System.Action<object>? onElementCreated = null, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior.opaque) => throw new NotSupportedException("HtmlElementView is only available on Flutter Web");

    public override Widget build(BuildContext context) => throw new NotSupportedException("HtmlElementView is only available on Flutter Web");
}

internal class _AndroidViewState__platform_view : State<AndroidView>
{
    internal virtual long? _id { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Services.AndroidViewController _controller { get; set; } = default!;
    internal virtual TextDirection? _layoutDirection { get; set; } = default;
    internal virtual bool _initialized { get; set; } = false;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal static HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> _emptyRecognizersSet = new HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>();

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Focus(focusNode: this._focusNode, onFocusChange: this._onFocusChange, child: new _AndroidPlatformView__platform_view(controller: this._controller, hitTestBehavior: ((AndroidView)this.widget).hitTestBehavior, gestureRecognizers: (((AndroidView)this.widget).gestureRecognizers ?? _emptyRecognizersSet), clipBehavior: ((AndroidView)this.widget).clipBehavior)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initializeOnce()
    {
        if (this._initialized)
        {
            return;
        }
        _initialized = true;
        _createNewAndroidView();
        _focusNode = new FocusNode(debugLabel: $"AndroidView(id: {this._id})");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.TextDirection newLayoutDirection__30813 = _findLayoutDirection();
        var didChangeLayoutDirection__30868 = (!object.Equals(this._layoutDirection, newLayoutDirection__30813));
        _layoutDirection = newLayoutDirection__30813;
        _initializeOnce();
        if (didChangeLayoutDirection__30868)
        {
            DartRuntimePrimitives.Ignore(this._controller.setLayoutDirection(DartRuntimePrimitives.RequireValue(this._layoutDirection)));
        }
    }

    public override void didUpdateWidget(AndroidView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        global::Doroti.Ui.TextDirection newLayoutDirection__31386 = _findLayoutDirection();
        var didChangeLayoutDirection__31441 = (!object.Equals(this._layoutDirection, newLayoutDirection__31386));
        _layoutDirection = newLayoutDirection__31386;
        if ((((AndroidView)this.widget).viewType != ((AndroidView)oldWidget).viewType))
        {
            ((dynamic)this._controller).disposePostFrame();
            _createNewAndroidView();
            return;
        }
        if (didChangeLayoutDirection__31441)
        {
            DartRuntimePrimitives.Ignore(this._controller.setLayoutDirection(DartRuntimePrimitives.RequireValue(this._layoutDirection)));
        }
    }

    internal virtual global::Doroti.Ui.TextDirection _findLayoutDirection()
    {
        DartRuntimePrimitives.Assert(() => ((((AndroidView)this.widget).layoutDirection is not null) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(this.context)));
        return ((((AndroidView)this.widget).layoutDirection ?? (TextDirection)Directionality.of(this.context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(this._controller.dispose());
        this._focusNode?.dispose();
        _focusNode = null;
        base.dispose();
    }

    internal virtual void _createNewAndroidView()
    {
        _id = global::Doroti.Generated.Framework.Services.Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
        _controller = PlatformViewsService.initAndroidView(id: DartRuntimePrimitives.RequireValue(this._id), viewType: ((AndroidView)this.widget).viewType, layoutDirection: DartRuntimePrimitives.RequireValue(this._layoutDirection), creationParams: ((AndroidView)this.widget).creationParams, creationParamsCodec: ((AndroidView)this.widget).creationParamsCodec, onFocus: ((global::System.Action)(() => {
this._focusNode!.requestFocus();
})));
        if ((((AndroidView)this.widget).onPlatformViewCreated is not null))
        {
            this._controller.addOnPlatformViewCreatedListener(((AndroidView)this.widget).onPlatformViewCreated!);
        }
    }

    internal virtual void _onFocusChange(bool isFocused)
    {
        if (!((global::Doroti.Generated.Framework.Services.AndroidViewController)this._controller).isCreated)
        {
            return;
        }
        if (!isFocused)
        {
            DartRuntimePrimitives.Ignore(this._controller.clearFocus().catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((e, stack) => {
if ((e is global::Doroti.Generated.Framework.Services.MissingPluginException))
{
    return;
}
else
{
    FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: e, stack: stack, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while clearing the platform view focus")));
}
}))));
            return;
        }
        DartRuntimePrimitives.Ignore(global::Doroti.Generated.Framework.Services.SystemChannels.textInput.invokeMethod<object?>("TextInput.setPlatformViewClient", new DartMap<string, object> { ["platformViewId"] = this._id }).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((e, stack) => {
if ((e is global::Doroti.Generated.Framework.Services.MissingPluginException))
{
    return;
}
else
{
    FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: e, stack: stack, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while setting the platform view client")));
}
}))));
    }

}

internal abstract class _DarwinViewState__platform_view<PlatformViewT, ControllerT, RenderT, ViewT> : State<PlatformViewT> where PlatformViewT : _DarwinView__platform_view where ControllerT : global::Doroti.Generated.Framework.Services.DarwinPlatformViewController where RenderT : global::Doroti.Generated.Framework.Rendering.RenderDarwinPlatformView<ControllerT> where ViewT : _DarwinPlatformView__platform_view<ControllerT, RenderT>
{
    internal virtual ControllerT? _controller { get; set; } = default;
    internal virtual TextDirection? _layoutDirection { get; set; } = default;
    internal virtual bool _initialized { get; set; } = false;
    public virtual FocusNode? focusNode { get; set; } = default;
    internal static HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> _emptyRecognizersSet = new HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>>();

    public override Widget build(BuildContext context)
    {
        ControllerT? controller__35380 = this._controller;
        if ((controller__35380 is null))
        {
            return ((Widget)(object?)SizedBox.CreateExpand());
        }
        return ((Widget)(object?)new Focus(focusNode: this.focusNode, onFocusChange: ((isFocused) => { _onFocusChange(isFocused, controller__35380); }), child: childPlatformView()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract ViewT childPlatformView();
    internal virtual void _initializeOnce()
    {
        if (this._initialized)
        {
            return;
        }
        _initialized = true;
        DartRuntimePrimitives.Ignore(_createNewUiKitView());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.TextDirection newLayoutDirection__35914 = _findLayoutDirection();
        var didChangeLayoutDirection__35969 = (!object.Equals(this._layoutDirection, newLayoutDirection__35914));
        _layoutDirection = newLayoutDirection__35914;
        _initializeOnce();
        if (didChangeLayoutDirection__35969)
        {
            DartRuntimePrimitives.Ignore(this._controller?.setLayoutDirection(DartRuntimePrimitives.RequireValue(this._layoutDirection)));
        }
    }

    public override void didUpdateWidget(PlatformViewT oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        global::Doroti.Ui.TextDirection newLayoutDirection__36490 = _findLayoutDirection();
        var didChangeLayoutDirection__36545 = (!object.Equals(this._layoutDirection, newLayoutDirection__36490));
        _layoutDirection = newLayoutDirection__36490;
        if ((this.widget.viewType != ((_DarwinView__platform_view)(object)oldWidget).viewType))
        {
            DartRuntimePrimitives.Ignore(this._controller?.dispose());
            _controller = null;
            this.focusNode?.dispose();
            focusNode = null;
            DartRuntimePrimitives.Ignore(_createNewUiKitView());
            return;
        }
        if (didChangeLayoutDirection__36545)
        {
            DartRuntimePrimitives.Ignore(this._controller?.setLayoutDirection(DartRuntimePrimitives.RequireValue(this._layoutDirection)));
        }
    }

    internal virtual global::Doroti.Ui.TextDirection _findLayoutDirection()
    {
        DartRuntimePrimitives.Assert(() => ((this.widget.layoutDirection is not null) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(this.context)));
        return ((this.widget.layoutDirection ?? (TextDirection)Directionality.of(this.context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(this._controller?.dispose());
        _controller = null;
        this.focusNode?.dispose();
        focusNode = null;
        base.dispose();
    }

    internal async virtual Future _createNewUiKitView()
    {
        try
        {
            long id__37391 = global::Doroti.Generated.Framework.Services.Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
            ControllerT controller__37467 = await createNewViewController(id__37391);
            if (!this.mounted)
            {
                DartRuntimePrimitives.Ignore(controller__37467.dispose());
                return;
            }
            this.widget.onPlatformViewCreated?.Invoke(id__37391);
            setState(((global::System.Action)(() => {
_controller = controller__37467;
focusNode = new FocusNode(debugLabel: $"UiKitView(id: {id__37391})");
})));
        }
        catch (Exception error__37779)
        {
            var stack__37786 = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: error__37779, stack: stack__37786, library: "widgets", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while creating a Darwin platform view")));
        }
    }

    public abstract Future<ControllerT> createNewViewController(long id);
    internal virtual void _onFocusChange(bool isFocused, ControllerT controller)
    {
        if (!isFocused)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(global::Doroti.Generated.Framework.Services.SystemChannels.textInput.invokeMethod<object?>("TextInput.setPlatformViewClient", new DartMap<string, object> { ["platformViewId"] = ((DarwinPlatformViewController)(object)controller).id }).then(((_) => {
}), onError: ((error, stack) => {
FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while setting the platform view client")));
})));
    }

}

internal class _UiKitViewState__platform_view : _DarwinViewState__platform_view<UiKitView, global::Doroti.Generated.Framework.Services.UiKitViewController, global::Doroti.Generated.Framework.Rendering.RenderUiKitView, _UiKitPlatformView__platform_view>
{
    public async override Future<global::Doroti.Generated.Framework.Services.UiKitViewController> createNewViewController(long id)
    {
        return await PlatformViewsService.initUiKitView(id: id, viewType: this.widget.viewType, gestureBlockingPolicy: this.widget.gestureBlockingPolicy, layoutDirection: DartRuntimePrimitives.RequireValue(this._layoutDirection), creationParams: this.widget.creationParams, creationParamsCodec: this.widget.creationParamsCodec, onFocus: ((global::System.Action)(() => {
this.focusNode?.requestFocus();
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _UiKitPlatformView__platform_view childPlatformView()
    {
        return new _UiKitPlatformView__platform_view(controller: this._controller!, hitTestBehavior: this.widget.hitTestBehavior, gestureRecognizers: (this.widget.gestureRecognizers ?? _emptyRecognizersSet));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AppKitViewState__platform_view : _DarwinViewState__platform_view<AppKitView, global::Doroti.Generated.Framework.Services.AppKitViewController, global::Doroti.Generated.Framework.Rendering.RenderAppKitView, _AppKitPlatformView__platform_view>
{
    public async override Future<global::Doroti.Generated.Framework.Services.AppKitViewController> createNewViewController(long id)
    {
        return await PlatformViewsService.initAppKitView(id: id, viewType: this.widget.viewType, layoutDirection: DartRuntimePrimitives.RequireValue(this._layoutDirection), creationParams: this.widget.creationParams, creationParamsCodec: this.widget.creationParamsCodec, onFocus: ((global::System.Action)(() => {
this.focusNode?.requestFocus();
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _AppKitPlatformView__platform_view childPlatformView()
    {
        return new _AppKitPlatformView__platform_view(controller: this._controller!, hitTestBehavior: this.widget.hitTestBehavior, gestureRecognizers: (this.widget.gestureRecognizers ?? _emptyRecognizersSet));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AndroidPlatformView__platform_view : LeafRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Services.AndroidViewController controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _AndroidPlatformView__platform_view(global::Doroti.Generated.Framework.Services.AndroidViewController controller, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers, Clip clipBehavior = Clip.hardEdge)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(new global::Doroti.Generated.Framework.Rendering.RenderAndroidView(viewController: this.controller, hitTestBehavior: this.hitTestBehavior, gestureRecognizers: this.gestureRecognizers, clipBehavior: this.clipBehavior));
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.RenderAndroidView)(object)renderObject;
        __renderObject.controller = this.controller;
        __renderObject.hitTestBehavior = this.hitTestBehavior;
        __renderObject.updateGestureRecognizers(this.gestureRecognizers);
        __renderObject.clipBehavior = this.clipBehavior;
    }

}

internal abstract class _DarwinPlatformView__platform_view<TController, TRender> : LeafRenderObjectWidget where TController : global::Doroti.Generated.Framework.Services.DarwinPlatformViewController where TRender : global::Doroti.Generated.Framework.Rendering.RenderDarwinPlatformView<TController>
{
    public virtual TController controller { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;

    internal _DarwinPlatformView__platform_view(TController controller, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (TRender)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<TRender>)(() =>
{            var __cascade = __renderObject;
            __cascade.viewController = this.controller;
            __cascade.hitTestBehavior = this.hitTestBehavior;
            __cascade.updateGestureRecognizers(this.gestureRecognizers);
            return __cascade;        }))());
    }

}

internal class _UiKitPlatformView__platform_view : _DarwinPlatformView__platform_view<global::Doroti.Generated.Framework.Services.UiKitViewController, global::Doroti.Generated.Framework.Rendering.RenderUiKitView>
{
    internal _UiKitPlatformView__platform_view(global::Doroti.Generated.Framework.Services.UiKitViewController controller, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderUiKitView(viewController: this.controller, hitTestBehavior: this.hitTestBehavior, gestureRecognizers: this.gestureRecognizers));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AppKitPlatformView__platform_view : _DarwinPlatformView__platform_view<global::Doroti.Generated.Framework.Services.AppKitViewController, global::Doroti.Generated.Framework.Rendering.RenderAppKitView>
{
    internal _AppKitPlatformView__platform_view(global::Doroti.Generated.Framework.Services.AppKitViewController controller, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.RenderAppKitView(viewController: this.controller, hitTestBehavior: this.hitTestBehavior, gestureRecognizers: this.gestureRecognizers));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PlatformViewCreationParams
{
    public virtual long id { get; private set; } = default!;
    public virtual string viewType { get; private set; } = default!;
    public virtual global::System.Action<long> onPlatformViewCreated { get; private set; } = default!;
    public virtual global::System.Action<bool> onFocusChanged { get; private set; } = default!;

    public PlatformViewCreationParams(long id, string viewType, global::System.Action<long> onPlatformViewCreated, global::System.Action<bool> onFocusChanged)
    {
        this.id = id;
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.onFocusChanged = onFocusChanged;
    }

}

public delegate Widget PlatformViewSurfaceFactory(BuildContext context, global::Doroti.Generated.Framework.Services.PlatformViewController controller);

public delegate global::Doroti.Generated.Framework.Services.PlatformViewController CreatePlatformViewCallback(PlatformViewCreationParams @params);

public class PlatformViewLink : StatefulWidget
{
    internal virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Services.PlatformViewController, Widget> _surfaceFactory { get; private set; } = default!;
    internal virtual global::System.Func<PlatformViewCreationParams, global::Doroti.Generated.Framework.Services.PlatformViewController> _onCreatePlatformView { get; private set; } = default!;
    public virtual string viewType { get; private set; } = default!;

    public PlatformViewLink(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Services.PlatformViewController, Widget> surfaceFactory = default!, global::System.Func<PlatformViewCreationParams, global::Doroti.Generated.Framework.Services.PlatformViewController> onCreatePlatformView = default!, string viewType = default!) : base(key: key)
    {
        this.viewType = viewType;
        this._surfaceFactory = surfaceFactory;
        this._onCreatePlatformView = onCreatePlatformView;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PlatformViewLinkState__platform_view());
}

internal class _PlatformViewLinkState__platform_view : State<PlatformViewLink>
{
    internal virtual long? _id { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Services.PlatformViewController? _controller { get; set; } = default;
    internal virtual bool _platformViewCreated { get; set; } = false;
    internal virtual Widget? _surface { get; set; } = default;
    internal virtual FocusNode? _focusNode { get; set; } = default;

    public override Widget build(BuildContext context)
    {
        global::Doroti.Generated.Framework.Services.PlatformViewController? controller__47607 = this._controller;
        if ((controller__47607 is null))
        {
            return ((Widget)(object?)SizedBox.CreateExpand());
        }
        if (!this._platformViewCreated)
        {
            return ((Widget)(object?)new _PlatformViewPlaceHolder__platform_view(onLayout: ((global::System.Action<Size, Offset>)((size, position) => {
if ((((global::Doroti.Generated.Framework.Services.PlatformViewController)controller__47607).awaitingCreation && !size.isEmpty))
{
    DartRuntimePrimitives.Ignore(controller__47607.create(size: size, position: position));
}
}))));
        }
        _surface ??= this.widget._surfaceFactory(context, controller__47607);
        return ((Widget)(object?)new Focus(focusNode: this._focusNode, onFocusChange: this._handleFrameworkFocusChanged, child: this._surface!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        _focusNode = new FocusNode(debugLabel: $"PlatformView(id: {this._id})");
        _initialize();
        base.initState();
    }

    public override void didUpdateWidget(PlatformViewLink oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((PlatformViewLink)this.widget).viewType != ((PlatformViewLink)oldWidget).viewType))
        {
            ((dynamic)this._controller)?.disposePostFrame();
            _surface = null;
            _initialize();
        }
    }

    internal virtual void _initialize()
    {
        _id = global::Doroti.Generated.Framework.Services.Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
        _controller = this.widget._onCreatePlatformView(new PlatformViewCreationParams(id: DartRuntimePrimitives.RequireValue(this._id), viewType: ((PlatformViewLink)this.widget).viewType, onPlatformViewCreated: (global::System.Action<long>)this._onPlatformViewCreated, onFocusChanged: (global::System.Action<bool>)this._handlePlatformFocusChanged));
    }

    internal virtual void _onPlatformViewCreated(long id)
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
_platformViewCreated = true;
})));
        }
    }

    internal virtual void _handleFrameworkFocusChanged(bool isFocused)
    {
        if (!isFocused)
        {
            DartRuntimePrimitives.Ignore(this._controller?.clearFocus());
        }
        DartRuntimePrimitives.Ignore(global::Doroti.Generated.Framework.Services.SystemChannels.textInput.invokeMethod<object?>("TextInput.setPlatformViewClient", new DartMap<string, object> { ["platformViewId"] = this._id }).catchError(((global::System.Action<object, global::System.Diagnostics.StackTrace>)((error, stack) => {
FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widget library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while handling framework focus changed on platform view")));
}))));
    }

    internal virtual void _handlePlatformFocusChanged(bool isFocused)
    {
        if (isFocused)
        {
            this._focusNode!.requestFocus();
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(this._controller?.dispose());
        _controller = null;
        this._focusNode?.dispose();
        _focusNode = null;
        base.dispose();
    }

}

public class PlatformViewSurface : LeafRenderObjectWidget
{
    public virtual global::Doroti.Generated.Framework.Services.PlatformViewController controller { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;

    public PlatformViewSurface(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Services.PlatformViewController controller = default!, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = default!, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers = default!) : base(key: key)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new global::Doroti.Generated.Framework.Rendering.PlatformViewRenderBox(controller: this.controller, gestureRecognizers: this.gestureRecognizers, hitTestBehavior: this.hitTestBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Generated.Framework.Rendering.PlatformViewRenderBox)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.PlatformViewRenderBox>)(() =>
{            var __cascade = __renderObject;
            __cascade.controller = this.controller;
            __cascade.hitTestBehavior = this.hitTestBehavior;
            __cascade.updateGestureRecognizers(this.gestureRecognizers);
            return __cascade;        }))());
    }

}

public class AndroidViewSurface : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Services.AndroidViewController controller { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;

    public AndroidViewSurface(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Services.AndroidViewController controller = default!, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = default!, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers = default!) : base(key: key)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
    }

    public override IState createState()
    {
        return ((IState)(object?)new _AndroidViewSurfaceState__platform_view());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AndroidViewSurfaceState__platform_view : State<AndroidViewSurface>
{
    public override void initState()
    {
        base.initState();
        if (!((AndroidViewSurface)this.widget).controller.isCreated)
        {
            ((AndroidViewSurface)this.widget).controller.addOnPlatformViewCreatedListener((global::System.Action<long>)this._onPlatformViewCreated);
        }
    }

    public override void dispose()
    {
        ((AndroidViewSurface)this.widget).controller.removeOnPlatformViewCreatedListener((global::System.Action<long>)this._onPlatformViewCreated);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (((AndroidViewSurface)this.widget).controller.requiresViewComposition)
        {
            return ((Widget)(object?)new _PlatformLayerBasedAndroidViewSurface__platform_view(controller: ((AndroidViewSurface)this.widget).controller, hitTestBehavior: ((AndroidViewSurface)this.widget).hitTestBehavior, gestureRecognizers: ((AndroidViewSurface)this.widget).gestureRecognizers));
        }
        else
        {
            return ((Widget)(object?)new _TextureBasedAndroidViewSurface__platform_view(controller: ((AndroidViewSurface)this.widget).controller, hitTestBehavior: ((AndroidViewSurface)this.widget).hitTestBehavior, gestureRecognizers: ((AndroidViewSurface)this.widget).gestureRecognizers));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onPlatformViewCreated(long __unused0)
    {
        setState(((global::System.Action)(() => {
})));
    }

}

internal class _TextureBasedAndroidViewSurface__platform_view : PlatformViewSurface
{
    internal _TextureBasedAndroidViewSurface__platform_view(global::Doroti.Generated.Framework.Services.AndroidViewController controller, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var viewController__57289 = ((global::Doroti.Generated.Framework.Services.AndroidViewController?)(object?)this.controller)!;
        var renderBox__57471 = new global::Doroti.Generated.Framework.Rendering.RenderAndroidView(viewController: viewController__57289, gestureRecognizers: this.gestureRecognizers, hitTestBehavior: this.hitTestBehavior);
        viewController__57289.pointTransformer = (global::System.Func<Offset, Offset>)((position) => ((Offset)((dynamic)renderBox__57471).globalToLocal(position)));
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)renderBox__57471);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PlatformLayerBasedAndroidViewSurface__platform_view : PlatformViewSurface
{
    internal _PlatformLayerBasedAndroidViewSurface__platform_view(global::Doroti.Generated.Framework.Services.AndroidViewController controller, global::Doroti.Generated.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Generated.Framework.Foundation.Factory<global::Doroti.Generated.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var viewController__58093 = ((global::Doroti.Generated.Framework.Services.AndroidViewController?)(object?)this.controller)!;
        var renderBox__58157 = ((global::Doroti.Generated.Framework.Rendering.PlatformViewRenderBox?)(object?)base.createRenderObject(context))!;
        viewController__58093.pointTransformer = (global::System.Func<Offset, Offset>)((position) => ((Offset)((dynamic)renderBox__58157).globalToLocal(position)));
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)renderBox__58157);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _OnLayoutCallback__platform_view(Size size, Offset position);

public class _PlatformViewPlaceholderBox__platform_view : global::Doroti.Generated.Framework.Rendering.RenderConstrainedBox
{
    public virtual global::System.Action<Size, Offset> onLayout { get; set; } = default!;

    internal _PlatformViewPlaceholderBox__platform_view(global::System.Action<Size, Offset> onLayout) : base(additionalConstraints: global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: double.PositiveInfinity, height: double.PositiveInfinity))
    {
        this.onLayout = onLayout;
    }

    public override void performLayout()
    {
        base.performLayout();
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((_) => {
if (!this.attached)
{
    return;
}
this.onLayout(this.size, localToGlobal(Offset.zero));
})), debugLabel: "PlatformViewPlaceholderBox.onLayout");
    }

}

internal class _PlatformViewPlaceHolder__platform_view : SingleChildRenderObjectWidget
{
    public virtual global::System.Action<Size, Offset> onLayout { get; private set; } = default!;

    internal _PlatformViewPlaceHolder__platform_view(global::System.Action<Size, Offset> onLayout)
    {
        this.onLayout = onLayout;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _PlatformViewPlaceholderBox__platform_view(onLayout: (global::System.Action<Size, Offset>)this.onLayout));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_PlatformViewPlaceholderBox__platform_view)(object)renderObject;
        __renderObject.onLayout = (global::System.Action<Size, Offset>)this.onLayout;
    }

}


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/platform_view.dart
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Framework.Widgets.PlatformViewDisposal;

namespace Doroti.Framework.Widgets;

public class AndroidView : StatefulWidget
{
    public virtual string viewType { get; private set; } = default!;
    public virtual global::System.Action<long>? onPlatformViewCreated { get; private set; }
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual TextDirection? layoutDirection { get; private set; }
    public virtual HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers { get; private set; }
    public virtual object? creationParams { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MessageCodec<object>? creationParamsCodec { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public AndroidView(global::Doroti.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null, object? creationParams = default!, global::Doroti.Framework.Services.MessageCodec<object>? creationParamsCodec = null, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.hitTestBehavior = hitTestBehavior;
        this.layoutDirection = layoutDirection;
        this.gestureRecognizers = gestureRecognizers;
        this.creationParams = creationParams;
        this.creationParamsCodec = creationParamsCodec;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((creationParams is null) || (creationParamsCodec is not null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AndroidViewState__platform_view());
}

public abstract class _DarwinView__platform_view : StatefulWidget
{
    public virtual string viewType { get; private set; } = default!;
    public virtual global::System.Action<long>? onPlatformViewCreated { get; private set; }
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual TextDirection? layoutDirection { get; private set; }
    public virtual object? creationParams { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MessageCodec<object>? creationParamsCodec { get; private set; }
    public virtual HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers { get; private set; }

    internal _DarwinView__platform_view(global::Doroti.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, object? creationParams = default!, global::Doroti.Framework.Services.MessageCodec<object>? creationParamsCodec = null, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null) : base(key: key)
    {
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.hitTestBehavior = hitTestBehavior;
        this.layoutDirection = layoutDirection;
        this.creationParams = creationParams;
        this.creationParamsCodec = creationParamsCodec;
        this.gestureRecognizers = gestureRecognizers;
        System.Diagnostics.Debug.Assert((creationParams is null) || (creationParamsCodec is not null));
    }

}

public class UiKitView : _DarwinView__platform_view
{
    public virtual global::Doroti.Framework.Services.UiKitViewGestureBlockingPolicy gestureBlockingPolicy { get; private set; } = default!;

    public UiKitView(global::Doroti.Framework.Foundation.Key? key = null, string viewType = default!, global::Doroti.Framework.Services.UiKitViewGestureBlockingPolicy gestureBlockingPolicy = UiKitViewGestureBlockingPolicy.fallbackToPluginDefault, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, object? creationParams = default!, global::Doroti.Framework.Services.MessageCodec<object>? creationParamsCodec = null, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null) : base(key: key, viewType: viewType, onPlatformViewCreated: onPlatformViewCreated, hitTestBehavior: hitTestBehavior, layoutDirection: DartRuntimePrimitives.RequireValue(layoutDirection), creationParams: creationParams, creationParamsCodec: creationParamsCodec, gestureRecognizers: gestureRecognizers)
    {
        this.gestureBlockingPolicy = gestureBlockingPolicy;
        System.Diagnostics.Debug.Assert((creationParams is null) || (creationParamsCodec is not null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _UiKitViewState__platform_view());
}

public class AppKitView : _DarwinView__platform_view
{
    public AppKitView(global::Doroti.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = PlatformViewHitTestBehavior.opaque, TextDirection? layoutDirection = null, object? creationParams = default!, global::Doroti.Framework.Services.MessageCodec<object>? creationParamsCodec = null, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>? gestureRecognizers = null) : base(key: key, viewType: viewType, onPlatformViewCreated: onPlatformViewCreated, hitTestBehavior: hitTestBehavior, layoutDirection: DartRuntimePrimitives.RequireValue(layoutDirection), creationParams: creationParams, creationParamsCodec: creationParamsCodec, gestureRecognizers: gestureRecognizers)
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
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;

    public HtmlElementView(global::Doroti.Framework.Foundation.Key? key = null, string viewType = default!, global::System.Action<long>? onPlatformViewCreated = null, object? creationParams = null, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = PlatformViewHitTestBehavior.opaque) : base(key: key)
    {
        this.viewType = viewType;
        this.onPlatformViewCreated = onPlatformViewCreated;
        this.creationParams = creationParams;
        this.hitTestBehavior = hitTestBehavior;
    }

    public static HtmlElementView CreateFromTagName(global::Doroti.Framework.Foundation.Key? key = null, string tagName = default!, bool isVisible = true, global::System.Action<object>? onElementCreated = null, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = PlatformViewHitTestBehavior.opaque) => throw new NotSupportedException("HtmlElementView is only available on Flutter Web");

    public override Widget build(BuildContext context) => new RegisteredHtmlElementView(this);
}

internal class _AndroidViewState__platform_view : State<AndroidView>
{
    internal virtual long? _id { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.AndroidViewController _controller { get; set; } = default!;
    internal virtual TextDirection? _layoutDirection { get; set; } = default;
    internal virtual bool _initialized { get; set; } = false;
    internal virtual FocusNode? _focusNode { get; set; } = default;
    internal static HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> _emptyRecognizersSet = new HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>();

    public override Widget build(BuildContext context)
    {
        return new Focus(focusNode: _focusNode, onFocusChange: _onFocusChange, child: new _AndroidPlatformView__platform_view(controller: _controller, hitTestBehavior: widget.hitTestBehavior, gestureRecognizers: widget.gestureRecognizers ?? _emptyRecognizersSet, clipBehavior: widget.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initializeOnce()
    {
        if (_initialized)
        {
            return;
        }
        _initialized = true;
        _createNewAndroidView();
        _focusNode = new FocusNode(debugLabel: $"AndroidView(id: {_id})");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.TextDirection newLayoutDirection = _findLayoutDirection();
        var didChangeLayoutDirection = !Equals(_layoutDirection, newLayoutDirection);
        _layoutDirection = newLayoutDirection;
        _initializeOnce();
        if (didChangeLayoutDirection)
        {
            DartRuntimePrimitives.Ignore(_controller.setLayoutDirection(DartRuntimePrimitives.RequireValue(_layoutDirection)));
        }
    }

    public override void didUpdateWidget(AndroidView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        global::Doroti.Ui.TextDirection newLayoutDirection = _findLayoutDirection();
        var didChangeLayoutDirection = !Equals(_layoutDirection, newLayoutDirection);
        _layoutDirection = newLayoutDirection;
        if (widget.viewType != oldWidget.viewType)
        {
            _disposeControllerPostFrame(_controller);
            _createNewAndroidView();
            return;
        }
        if (didChangeLayoutDirection)
        {
            DartRuntimePrimitives.Ignore(_controller.setLayoutDirection(DartRuntimePrimitives.RequireValue(_layoutDirection)));
        }
    }

    internal virtual global::Doroti.Ui.TextDirection _findLayoutDirection()
    {
        DartRuntimePrimitives.Assert(() => (widget.layoutDirection is not null) || DebugLibrary.debugCheckHasDirectionality(context));
        return widget.layoutDirection ?? Directionality.of(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(_controller.dispose());
        _focusNode?.dispose();
        _focusNode = null;
        base.dispose();
    }

    internal virtual void _createNewAndroidView()
    {
        _id = Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
        _controller = PlatformViewsService.initAndroidView(id: DartRuntimePrimitives.RequireValue(_id), viewType: widget.viewType, layoutDirection: DartRuntimePrimitives.RequireValue(_layoutDirection), creationParams: widget.creationParams, creationParamsCodec: widget.creationParamsCodec, onFocus: () =>
        {
            _focusNode!.requestFocus();
        });
        if (widget.onPlatformViewCreated is not null)
        {
            _controller.addOnPlatformViewCreatedListener(widget.onPlatformViewCreated!);
        }
    }

    internal virtual void _onFocusChange(bool isFocused)
    {
        if (!_controller.isCreated)
        {
            return;
        }
        if (!isFocused)
        {
            DartRuntimePrimitives.Ignore(_controller.clearFocus().catchError((e, stack) =>
            {
                if (e is global::Doroti.Framework.Services.MissingPluginException)
                {
                    return;
                }
                else
                {
                    FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: e, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while clearing the platform view focus")));
                }
            }));
            return;
        }
        DartRuntimePrimitives.Ignore(SystemChannels.textInput.invokeMethod<object?>("TextInput.setPlatformViewClient", new DartMap<string, object?> { ["platformViewId"] = _id }).catchError((e, stack) =>
        {
            if (e is global::Doroti.Framework.Services.MissingPluginException)
            {
                return;
            }
            else
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: e, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while setting the platform view client")));
            }
        }));
    }

}

internal abstract class _DarwinViewState__platform_view<PlatformViewT, ControllerT, RenderT, ViewT> : State<PlatformViewT> where PlatformViewT : _DarwinView__platform_view where ControllerT : global::Doroti.Framework.Services.DarwinPlatformViewController where RenderT : global::Doroti.Framework.Rendering.RenderDarwinPlatformView<ControllerT> where ViewT : _DarwinPlatformView__platform_view<ControllerT, RenderT>
{
    internal virtual ControllerT? _controller { get; set; } = default;
    internal virtual TextDirection? _layoutDirection { get; set; } = default;
    internal virtual bool _initialized { get; set; } = false;
    public virtual FocusNode? focusNode { get; set; } = default;
    internal static HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> _emptyRecognizersSet = new HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>>();

    public override Widget build(BuildContext context)
    {
        ControllerT? controller = _controller;
        if (controller is null)
        {
            return SizedBox.CreateExpand();
        }
        return new Focus(focusNode: focusNode, onFocusChange: (isFocused) => { _onFocusChange(isFocused, controller); }, child: childPlatformView());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract ViewT childPlatformView();
    internal virtual void _initializeOnce()
    {
        if (_initialized)
        {
            return;
        }
        _initialized = true;
        DartRuntimePrimitives.Ignore(_createNewUiKitView());
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Ui.TextDirection newLayoutDirection = _findLayoutDirection();
        var didChangeLayoutDirection = !Equals(_layoutDirection, newLayoutDirection);
        _layoutDirection = newLayoutDirection;
        _initializeOnce();
        if (didChangeLayoutDirection)
        {
            DartRuntimePrimitives.Ignore(_controller?.setLayoutDirection(DartRuntimePrimitives.RequireValue(_layoutDirection)));
        }
    }

    public override void didUpdateWidget(PlatformViewT oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        global::Doroti.Ui.TextDirection newLayoutDirection = _findLayoutDirection();
        var didChangeLayoutDirection = !Equals(_layoutDirection, newLayoutDirection);
        _layoutDirection = newLayoutDirection;
        if (widget.viewType != oldWidget.viewType)
        {
            DartRuntimePrimitives.Ignore(_controller?.dispose());
            _controller = null;
            focusNode?.dispose();
            focusNode = null;
            DartRuntimePrimitives.Ignore(_createNewUiKitView());
            return;
        }
        if (didChangeLayoutDirection)
        {
            DartRuntimePrimitives.Ignore(_controller?.setLayoutDirection(DartRuntimePrimitives.RequireValue(_layoutDirection)));
        }
    }

    internal virtual global::Doroti.Ui.TextDirection _findLayoutDirection()
    {
        DartRuntimePrimitives.Assert(() => (widget.layoutDirection is not null) || DebugLibrary.debugCheckHasDirectionality(context));
        return widget.layoutDirection ?? Directionality.of(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(_controller?.dispose());
        _controller = null;
        focusNode?.dispose();
        focusNode = null;
        base.dispose();
    }

    internal async virtual Future _createNewUiKitView()
    {
        try
        {
            long id = Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
            ControllerT controller = await createNewViewController(id);
            if (!mounted)
            {
                DartRuntimePrimitives.Ignore(controller.dispose());
                return;
            }
            widget.onPlatformViewCreated?.Invoke(id);
            setState(() =>
            {
                _controller = controller;
                focusNode = new FocusNode(debugLabel: $"UiKitView(id: {id})");
            });
        }
        catch (Exception error)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stackLocal, library: "widgets", context: new global::Doroti.Framework.Foundation.ErrorDescription("while creating a Darwin platform view")));
        }
    }

    public abstract Future<ControllerT> createNewViewController(long id);
    internal virtual void _onFocusChange(bool isFocused, ControllerT controller)
    {
        if (!isFocused)
        {
            return;
        }
        DartRuntimePrimitives.Ignore(SystemChannels.textInput.invokeMethod<object?>("TextInput.setPlatformViewClient", new DartMap<string, object?> { ["platformViewId"] = controller.id }).then((_) =>
        {
        }, onError: (error, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widgets library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while setting the platform view client")));
        }));
    }

}

internal class _UiKitViewState__platform_view : _DarwinViewState__platform_view<UiKitView, global::Doroti.Framework.Services.UiKitViewController, global::Doroti.Framework.Rendering.RenderUiKitView, _UiKitPlatformView__platform_view>
{
    public async override Future<global::Doroti.Framework.Services.UiKitViewController> createNewViewController(long id)
    {
        return await PlatformViewsService.initUiKitView(id: id, viewType: widget.viewType, gestureBlockingPolicy: widget.gestureBlockingPolicy, layoutDirection: DartRuntimePrimitives.RequireValue(_layoutDirection), creationParams: widget.creationParams, creationParamsCodec: widget.creationParamsCodec, onFocus: () =>
        {
            focusNode?.requestFocus();
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _UiKitPlatformView__platform_view childPlatformView()
    {
        return new _UiKitPlatformView__platform_view(controller: _controller!, hitTestBehavior: widget.hitTestBehavior, gestureRecognizers: widget.gestureRecognizers ?? _emptyRecognizersSet);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AppKitViewState__platform_view : _DarwinViewState__platform_view<AppKitView, global::Doroti.Framework.Services.AppKitViewController, global::Doroti.Framework.Rendering.RenderAppKitView, _AppKitPlatformView__platform_view>
{
    public async override Future<global::Doroti.Framework.Services.AppKitViewController> createNewViewController(long id)
    {
        return await PlatformViewsService.initAppKitView(id: id, viewType: widget.viewType, layoutDirection: DartRuntimePrimitives.RequireValue(_layoutDirection), creationParams: widget.creationParams, creationParamsCodec: widget.creationParamsCodec, onFocus: () =>
        {
            focusNode?.requestFocus();
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _AppKitPlatformView__platform_view childPlatformView()
    {
        return new _AppKitPlatformView__platform_view(controller: _controller!, hitTestBehavior: widget.hitTestBehavior, gestureRecognizers: widget.gestureRecognizers ?? _emptyRecognizersSet);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AndroidPlatformView__platform_view : LeafRenderObjectWidget
{
    public virtual global::Doroti.Framework.Services.AndroidViewController controller { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _AndroidPlatformView__platform_view(global::Doroti.Framework.Services.AndroidViewController controller, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers, Clip clipBehavior = Clip.hardEdge)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(new global::Doroti.Framework.Rendering.RenderAndroidView(viewController: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers, clipBehavior: clipBehavior));
    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderAndroidView)renderObject;
        __renderObject.controller = controller;
        __renderObject.hitTestBehavior = hitTestBehavior;
        __renderObject.updateGestureRecognizers(gestureRecognizers);
        __renderObject.clipBehavior = clipBehavior;
    }

}

internal abstract class _DarwinPlatformView__platform_view<TController, TRender> : LeafRenderObjectWidget where TController : global::Doroti.Framework.Services.DarwinPlatformViewController where TRender : global::Doroti.Framework.Rendering.RenderDarwinPlatformView<TController>
{
    public virtual TController controller { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;

    internal _DarwinPlatformView__platform_view(TController controller, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (TRender)renderObject;
        DartRuntimePrimitives.Ignore(((Func<TRender>)(() =>
{
    var __cascade = __renderObject;
    __cascade.viewController = controller;
    __cascade.hitTestBehavior = hitTestBehavior;
    __cascade.updateGestureRecognizers(gestureRecognizers);
    return __cascade;
}))());
    }

}

internal class _UiKitPlatformView__platform_view : _DarwinPlatformView__platform_view<global::Doroti.Framework.Services.UiKitViewController, global::Doroti.Framework.Rendering.RenderUiKitView>
{
    internal _UiKitPlatformView__platform_view(global::Doroti.Framework.Services.UiKitViewController controller, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.RenderUiKitView(viewController: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AppKitPlatformView__platform_view : _DarwinPlatformView__platform_view<global::Doroti.Framework.Services.AppKitViewController, global::Doroti.Framework.Rendering.RenderAppKitView>
{
    internal _AppKitPlatformView__platform_view(global::Doroti.Framework.Services.AppKitViewController controller, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.RenderAppKitView(viewController: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers);
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

public delegate Widget PlatformViewSurfaceFactory(BuildContext context, global::Doroti.Framework.Services.PlatformViewController controller);

public delegate global::Doroti.Framework.Services.PlatformViewController CreatePlatformViewCallback(PlatformViewCreationParams @params);

public class PlatformViewLink : StatefulWidget
{
    internal virtual global::System.Func<BuildContext, global::Doroti.Framework.Services.PlatformViewController, Widget> _surfaceFactory { get; private set; } = default!;
    internal virtual global::System.Func<PlatformViewCreationParams, global::Doroti.Framework.Services.PlatformViewController> _onCreatePlatformView { get; private set; } = default!;
    public virtual string viewType { get; private set; } = default!;

    public PlatformViewLink(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, global::Doroti.Framework.Services.PlatformViewController, Widget> surfaceFactory = default!, global::System.Func<PlatformViewCreationParams, global::Doroti.Framework.Services.PlatformViewController> onCreatePlatformView = default!, string viewType = default!) : base(key: key)
    {
        this.viewType = viewType;
        _surfaceFactory = surfaceFactory;
        _onCreatePlatformView = onCreatePlatformView;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PlatformViewLinkState__platform_view());
}

internal class _PlatformViewLinkState__platform_view : State<PlatformViewLink>
{
    internal virtual long? _id { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.PlatformViewController? _controller { get; set; } = default;
    internal virtual bool _platformViewCreated { get; set; } = false;
    internal virtual Widget? _surface { get; set; } = default;
    internal virtual FocusNode? _focusNode { get; set; } = default;

    public override Widget build(BuildContext context)
    {
        global::Doroti.Framework.Services.PlatformViewController? controller = _controller;
        if (controller is null)
        {
            return SizedBox.CreateExpand();
        }
        if (!_platformViewCreated)
        {
            return new _PlatformViewPlaceHolder__platform_view(onLayout: (size, position) =>
            {
                if (controller.awaitingCreation && !size.isEmpty)
                {
                    DartRuntimePrimitives.Ignore(controller.create(size: size, position: position));
                }
            });
        }
        _surface ??= widget._surfaceFactory(context, controller);
        return new Focus(focusNode: _focusNode, onFocusChange: _handleFrameworkFocusChanged, child: _surface!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void initState()
    {
        _focusNode = new FocusNode(debugLabel: $"PlatformView(id: {_id})");
        _initialize();
        base.initState();
    }

    public override void didUpdateWidget(PlatformViewLink oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.viewType != oldWidget.viewType)
        {
            if (_controller is not null) _disposeControllerPostFrame(_controller);
            _surface = null;
            _initialize();
        }
    }

    internal virtual void _initialize()
    {
        _id = Platform_viewsLibrary.platformViewsRegistry.getNextPlatformViewId();
        _controller = widget._onCreatePlatformView(new PlatformViewCreationParams(id: DartRuntimePrimitives.RequireValue(_id), viewType: widget.viewType, onPlatformViewCreated: _onPlatformViewCreated, onFocusChanged: _handlePlatformFocusChanged));
    }

    internal virtual void _onPlatformViewCreated(long id)
    {
        if (mounted)
        {
            setState(() =>
            {
                _platformViewCreated = true;
            });
        }
    }

    internal virtual void _handleFrameworkFocusChanged(bool isFocused)
    {
        if (!isFocused)
        {
            DartRuntimePrimitives.Ignore(_controller?.clearFocus());
        }
        DartRuntimePrimitives.Ignore(SystemChannels.textInput.invokeMethod<object?>("TextInput.setPlatformViewClient", new DartMap<string, object?> { ["platformViewId"] = _id }).catchError((error, stack) =>
        {
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stack, library: "widget library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while handling framework focus changed on platform view")));
        }));
    }

    internal virtual void _handlePlatformFocusChanged(bool isFocused)
    {
        if (isFocused)
        {
            _focusNode!.requestFocus();
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Ignore(_controller?.dispose());
        _controller = null;
        _focusNode?.dispose();
        _focusNode = null;
        base.dispose();
    }

}

public class PlatformViewSurface : LeafRenderObjectWidget
{
    public virtual global::Doroti.Framework.Services.PlatformViewController controller { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;

    public PlatformViewSurface(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Services.PlatformViewController controller = default!, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = default!, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers = default!) : base(key: key)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new global::Doroti.Framework.Rendering.PlatformViewRenderBox(controller: controller, gestureRecognizers: gestureRecognizers, hitTestBehavior: hitTestBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.PlatformViewRenderBox)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.PlatformViewRenderBox>)(() =>
{
    var __cascade = __renderObject;
    __cascade.controller = controller;
    __cascade.hitTestBehavior = hitTestBehavior;
    __cascade.updateGestureRecognizers(gestureRecognizers);
    return __cascade;
}))());
    }

}

public class AndroidViewSurface : StatefulWidget
{
    public virtual global::Doroti.Framework.Services.AndroidViewController controller { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior { get; private set; } = default!;

    public AndroidViewSurface(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Services.AndroidViewController controller = default!, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior = default!, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers = default!) : base(key: key)
    {
        this.controller = controller;
        this.hitTestBehavior = hitTestBehavior;
        this.gestureRecognizers = gestureRecognizers;
    }

    public override IState createState()
    {
        return new _AndroidViewSurfaceState__platform_view();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AndroidViewSurfaceState__platform_view : State<AndroidViewSurface>
{
    public override void initState()
    {
        base.initState();
        if (!widget.controller.isCreated)
        {
            widget.controller.addOnPlatformViewCreatedListener(_onPlatformViewCreated);
        }
    }

    public override void dispose()
    {
        widget.controller.removeOnPlatformViewCreatedListener(_onPlatformViewCreated);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        if (widget.controller.requiresViewComposition)
        {
            return new _PlatformLayerBasedAndroidViewSurface__platform_view(controller: widget.controller, hitTestBehavior: widget.hitTestBehavior, gestureRecognizers: widget.gestureRecognizers);
        }
        else
        {
            return new _TextureBasedAndroidViewSurface__platform_view(controller: widget.controller, hitTestBehavior: widget.hitTestBehavior, gestureRecognizers: widget.gestureRecognizers);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _onPlatformViewCreated(long __unused0)
    {
        setState(() =>
        {
        });
    }

}

internal class _TextureBasedAndroidViewSurface__platform_view : PlatformViewSurface
{
    internal _TextureBasedAndroidViewSurface__platform_view(global::Doroti.Framework.Services.AndroidViewController controller, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var viewControllerLocal = ((global::Doroti.Framework.Services.AndroidViewController?)controller)!;
        var renderBox = new global::Doroti.Framework.Rendering.RenderAndroidView(viewController: viewControllerLocal, gestureRecognizers: gestureRecognizers, hitTestBehavior: hitTestBehavior);
        viewControllerLocal.pointTransformer = (position) => renderBox.globalToLocal(position);
        return renderBox;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PlatformLayerBasedAndroidViewSurface__platform_view : PlatformViewSurface
{
    internal _PlatformLayerBasedAndroidViewSurface__platform_view(global::Doroti.Framework.Services.AndroidViewController controller, global::Doroti.Framework.Rendering.PlatformViewHitTestBehavior hitTestBehavior, HashSet<global::Doroti.Framework.Foundation.Factory<global::Doroti.Framework.Gestures.OneSequenceGestureRecognizer>> gestureRecognizers) : base(controller: controller, hitTestBehavior: hitTestBehavior, gestureRecognizers: gestureRecognizers)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var viewController = ((global::Doroti.Framework.Services.AndroidViewController?)controller)!;
        var renderBox = ((global::Doroti.Framework.Rendering.PlatformViewRenderBox?)base.createRenderObject(context))!;
        viewController.pointTransformer = (position) => renderBox.globalToLocal(position);
        return renderBox;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _OnLayoutCallback__platform_view(Size size, Offset position);

public class _PlatformViewPlaceholderBox__platform_view : global::Doroti.Framework.Rendering.RenderConstrainedBox
{
    public virtual global::System.Action<Size, Offset> onLayout { get; set; } = default!;

    internal _PlatformViewPlaceholderBox__platform_view(global::System.Action<Size, Offset> onLayout) : base(additionalConstraints: BoxConstraints.CreateTightFor(width: double.PositiveInfinity, height: double.PositiveInfinity))
    {
        this.onLayout = onLayout;
    }

    public override void performLayout()
    {
        base.performLayout();
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((_) =>
        {
            if (!attached)
            {
                return;
            }
            onLayout(size, localToGlobal(Offset.zero));
        }, debugLabel: "PlatformViewPlaceholderBox.onLayout");
    }

}

internal class _PlatformViewPlaceHolder__platform_view : SingleChildRenderObjectWidget
{
    public virtual global::System.Action<Size, Offset> onLayout { get; private set; } = default!;

    internal _PlatformViewPlaceHolder__platform_view(global::System.Action<Size, Offset> onLayout)
    {
        this.onLayout = onLayout;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _PlatformViewPlaceholderBox__platform_view(onLayout: onLayout);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_PlatformViewPlaceholderBox__platform_view)renderObject;
        __renderObject.onLayout = onLayout;
    }

}


internal static class PlatformViewDisposal
{
    internal static void _disposeControllerPostFrame(global::Doroti.Framework.Services.PlatformViewController controller)
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(_ => DartRuntimePrimitives.Ignore(controller.dispose()), debugLabel: "PlatformViewController.dispose");
    }
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/adapter.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class RenderObjectToWidgetAdapter<T> : RenderObjectWidget
    where T : RenderObject
{
    public virtual Widget? child { get; private set; }
    public virtual RenderObjectWithChildMixin<T> container { get; private set; } = default!;
    public virtual string? debugShortDescription { get; private set; }

    public RenderObjectToWidgetAdapter(
        Widget? child = null,
        RenderObjectWithChildMixin<T> container = default!,
        string? debugShortDescription = null
    )
        : base(key: new GlobalObjectKey<IState>(container))
    {
        this.child = child;
        this.container = container;
        this.debugShortDescription = debugShortDescription;
    }

    public override RenderObjectToWidgetElement<T> createElement() =>
        new RenderObjectToWidgetElement<T>(this);

    public override RenderObject createRenderObject(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<RenderObject>(container);

    public override void updateRenderObject(BuildContext context, RenderObject renderObject) { }

    public virtual RenderObjectToWidgetElement<T> attachToRenderTree(
        BuildOwner owner,
        RenderObjectToWidgetElement<T>? element = null
    )
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
            element._newWidget = DartRuntimePrimitives.ConvertValue<Widget>(this);
            element.markNeedsBuild();
        }
        return element!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() =>
        DartRuntimePrimitives.ConvertValue<string>(debugShortDescription ?? base.toStringShort());
}

public class RenderObjectToWidgetElement<T> : RenderTreeRootElement, RootElementMixin
    where T : RenderObject
{
    internal virtual Element? _child { get; set; } = default;
    internal static object _rootChildSlot = new object();
    internal virtual Widget? _newWidget { get; set; } = default;

    public RenderObjectToWidgetElement(RenderObjectToWidgetAdapter<T> widget)
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
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (RenderObjectToWidgetAdapter<T>)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        _rebuild();
    }

    public override void performRebuild()
    {
        if (_newWidget is not null)
        {
            Widget newWidget = _newWidget!;
            _newWidget = null;
            update(((RenderObjectToWidgetAdapter<T>?)newWidget)!);
        }
        base.performRebuild();
        DartRuntimePrimitives.Assert(() => _newWidget is null);
    }

    internal virtual void _rebuild()
    {
        try
        {
            _child = updateChild(
                _child,
                ((RenderObjectToWidgetAdapter<T>?)widget)!.child,
                _rootChildSlot
            );
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
            Widget error = ErrorWidget.builder(details);
            _child = updateChild(null, error, _rootChildSlot);
        }
    }

    public override RenderObject renderObject =>
        DartRuntimePrimitives.ConvertValue<RenderObject>(
            ((RenderObjectWithChildMixin<T>?)base.renderObject)!
        );

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => Equals(slot, _rootChildSlot));
        DartRuntimePrimitives.Assert(() =>
            ((RenderObjectWithChildMixin<T>)renderObject).debugValidateChild(child)
        );
        ((RenderObjectWithChildMixin<T>)renderObject).child = ((T?)child)!;
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() =>
            Equals(((RenderObjectWithChildMixin<T>)renderObject).child, child)
        );
        ((RenderObjectWithChildMixin<T>)renderObject).child = default(T);
    }

    public virtual void assignOwner(BuildOwner owner)
    {
        _owner = owner;
        _parentBuildScope = new BuildScope();
    }
}

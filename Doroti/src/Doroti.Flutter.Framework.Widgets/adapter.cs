// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/adapter.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class RenderObjectToWidgetAdapter<T> : RenderObjectWidget where T : global::Doroti.Generated.Framework.Rendering.RenderObject
{
    public virtual Widget? child { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<T> container { get; private set; } = default!;
    public virtual string? debugShortDescription { get; private set; }

    public RenderObjectToWidgetAdapter(Widget? child = null, global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<T> container = default!, string? debugShortDescription = null) : base(key: new GlobalObjectKey<IState>(container))
    {
        this.child = child;
        this.container = container;
        this.debugShortDescription = debugShortDescription;
    }

    public override RenderObjectToWidgetElement<T> createElement() => new RenderObjectToWidgetElement<T>(this);
    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(this.container);
    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
    }

    public virtual RenderObjectToWidgetElement<T> attachToRenderTree(BuildOwner owner, RenderObjectToWidgetElement<T>? element = null)
    {
        if ((element is null))
        {
            owner.lockState(((global::System.Action)(() => {
element = createElement();
DartRuntimePrimitives.Assert(() => (element is not null));
element!.assignOwner(owner);
})));
            owner.buildScope(element!, ((global::System.Action)(() => {
element!.mount(((Element)(object)null), null);
})));
        }
        else
        {
            element._newWidget = DartRuntimePrimitives.ConvertValue<Widget>(this);
            element.markNeedsBuild();
        }
        return element!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort() => DartRuntimePrimitives.ConvertValue<string>(((this.debugShortDescription ?? (string)base.toStringShort())));
}

public class RenderObjectToWidgetElement<T> : RenderTreeRootElement, RootElementMixin where T : global::Doroti.Generated.Framework.Rendering.RenderObject
{
    internal virtual Element? _child { get; set; } = default;
    internal static object _rootChildSlot = new object();
    internal virtual Widget? _newWidget { get; set; } = default;

    public RenderObjectToWidgetElement(RenderObjectToWidgetAdapter<T> widget) : base(widget)
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
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (RenderObjectToWidgetAdapter<T>)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        _rebuild();
    }

    public override void performRebuild()
    {
        if ((this._newWidget is not null))
        {
            Widget newWidget__4575 = this._newWidget!;
            _newWidget = null;
            update(((RenderObjectToWidgetAdapter<T>?)(object?)newWidget__4575)!);
        }
        base.performRebuild();
        DartRuntimePrimitives.Assert(() => (this._newWidget is null));
    }

    internal virtual void _rebuild()
    {
        try
        {
            _child = updateChild(this._child, ((Widget?)((dynamic)(((RenderObjectToWidgetAdapter<T>?)(object?)this.widget)!)).child), _rootChildSlot);
        }
        catch (Exception exception__4978)
        {
            var stack__4989 = new System.Diagnostics.StackTrace();
            var details__5010 = new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception__4978, stack: stack__4989, library: "widgets library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("attaching to the render tree"));
            FlutterError.reportError(details__5010);
            Widget error__5265 = ErrorWidget.builder(details__5010);
            _child = updateChild(((Element)(object)null), error__5265, _rootChildSlot);
        }
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<T>?)(object?)base.renderObject)!);
    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(slot, _rootChildSlot)));
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)this.renderObject).debugValidateChild(child)));
        ((dynamic)this.renderObject).child = ((T?)(object?)child)!;
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        DartRuntimePrimitives.Assert(() => object.Equals(((global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<T>)this.renderObject).child, child));
        ((dynamic)this.renderObject).child = default(T);
    }

    public virtual void assignOwner(BuildOwner owner)
    {
        _owner = owner;
        _parentBuildScope = new BuildScope();
    }

}


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/sliver_prototype_extent_list.dart
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

public class SliverPrototypeExtentList : SliverMultiBoxAdaptorWidget
{
    public virtual Widget prototypeItem { get; private set; } = default!;

    public SliverPrototypeExtentList(global::Doroti.Generated.Framework.Foundation.Key? key = null, SliverChildDelegate @delegate = default!, Widget prototypeItem = default!) : base(key: key, @delegate: @delegate)
    {
        this.prototypeItem = prototypeItem;
    }

    public static SliverPrototypeExtentList CreateBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, Widget prototypeItem = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        var __instance = new SliverPrototypeExtentList(default!, default!, default!);
        __instance.prototypeItem = prototypeItem;
        return __instance;
    }

    public static SliverPrototypeExtentList CreateList(global::Doroti.Generated.Framework.Foundation.Key? key = null, List<Widget> children = default!, Widget prototypeItem = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true)
    {
        var __instance = new SliverPrototypeExtentList(default!, default!, default!);
        __instance.prototypeItem = prototypeItem;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var element__6516 = ((_SliverPrototypeExtentListElement__sliver_prototype_extent_list?)(object?)context)!;
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderSliverPrototypeExtentList__sliver_prototype_extent_list(childManager: element__6516));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SliverMultiBoxAdaptorElement createElement() => DartRuntimePrimitives.ConvertValue<SliverMultiBoxAdaptorElement>(new _SliverPrototypeExtentListElement__sliver_prototype_extent_list(this));
}

public class _SliverPrototypeExtentListElement__sliver_prototype_extent_list : SliverMultiBoxAdaptorElement
{
    internal virtual Element? _prototype { get; set; } = default;
    internal static object _prototypeSlot = new object();

    internal _SliverPrototypeExtentListElement__sliver_prototype_extent_list(SliverPrototypeExtentList widget) : base(widget)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(((_RenderSliverPrototypeExtentList__sliver_prototype_extent_list?)(object?)base.renderObject)!);
    public override void insertRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        if ((object.Equals(slot, _prototypeSlot)))
        {
            DartRuntimePrimitives.Assert(() => (child is global::Doroti.Generated.Framework.Rendering.RenderBox));
            ((dynamic)this.renderObject).child = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)child)!;
        }
        else
        {
            base.insertRenderObjectChild(child, ((long)slot));
        }
    }

    public override void didAdoptChild(global::Doroti.Generated.Framework.Rendering.RenderBox child)
    {
        if ((!object.Equals(child, ((_RenderSliverPrototypeExtentList__sliver_prototype_extent_list)this.renderObject).child)))
        {
            base.didAdoptChild(child);
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((object.Equals(newSlot, _prototypeSlot)))
        {
            DartRuntimePrimitives.Assert(() => false);
        }
        else
        {
            base.moveRenderObjectChild(__child, ((long)oldSlot), ((long)newSlot));
        }
    }

    public override void removeRenderObjectChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, object? slot)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        if ((object.Equals(((_RenderSliverPrototypeExtentList__sliver_prototype_extent_list)this.renderObject).child, __child)))
        {
            ((dynamic)this.renderObject).child = null;
        }
        else
        {
            base.removeRenderObjectChild(__child, ((long)slot));
        }
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        if ((this._prototype is not null))
        {
            visitor(this._prototype!);
        }
        base.visitChildren((global::System.Action<Element>)visitor);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        _prototype = updateChild(this._prototype, (((SliverPrototypeExtentList?)(object?)this.widget)!).prototypeItem, _prototypeSlot);
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SliverPrototypeExtentList)(object)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(this.widget, __newWidget)));
        _prototype = updateChild(this._prototype, (((SliverPrototypeExtentList?)(object?)this.widget)!).prototypeItem, _prototypeSlot);
    }

}

public class _RenderSliverPrototypeExtentList__sliver_prototype_extent_list : global::Doroti.Generated.Framework.Rendering.RenderSliverFixedExtentBoxAdaptor
{
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderBox? _child { get; set; } = default;

    internal _RenderSliverPrototypeExtentList__sliver_prototype_extent_list(_SliverPrototypeExtentListElement__sliver_prototype_extent_list childManager) : base(childManager: childManager)
    {
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            _child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
            markNeedsLayout();
        }
    }
    public override void performLayout()
    {
        this.child!.layout(this.constraints.asBoxConstraints(), parentUsesSize: true);
        base.performLayout();
    }

    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        this._child?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
        base.redepthChildren();
    }

    public override void visitChildren(global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
        base.visitChildren((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)visitor);
    }

    public override double? itemExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => ((this.child is not null) && this.child!.hasSize));
            return ((object.Equals(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).axis, global::Doroti.Generated.Framework.Painting.Axis.vertical)) ? this.child!.size.height : this.child!.size.width);
            return default!;
        }
    }
}


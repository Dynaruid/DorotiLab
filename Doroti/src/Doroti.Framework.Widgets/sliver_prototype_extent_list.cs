// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_prototype_extent_list.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class SliverPrototypeExtentList : SliverMultiBoxAdaptorWidget
{
    public virtual Widget prototypeItem { get; private set; } = default!;

    public SliverPrototypeExtentList(
        Key? key = null,
        SliverChildDelegate @delegate = default!,
        Widget prototypeItem = default!
    )
        : base(key: key, @delegate: @delegate)
    {
        this.prototypeItem = prototypeItem;
    }

    public static SliverPrototypeExtentList CreateBuilder(
        Key? key = null,
        Func<BuildContext, long, Widget?> itemBuilder = default!,
        Widget prototypeItem = default!,
        Func<Key, long?>? findChildIndexCallback = null,
        long? itemCount = null,
        bool addAutomaticKeepAlives = true,
        bool addRepaintBoundaries = true,
        bool addSemanticIndexes = true
    )
    {
        return new SliverPrototypeExtentList(
            key,
            new SliverChildBuilderDelegate(
                itemBuilder,
                findChildIndexCallback,
                itemCount,
                addAutomaticKeepAlives,
                addRepaintBoundaries,
                addSemanticIndexes
            ),
            prototypeItem
        );
    }

    public static SliverPrototypeExtentList CreateList(
        Key? key = null,
        List<Widget> children = default!,
        Widget prototypeItem = default!,
        bool addAutomaticKeepAlives = true,
        bool addRepaintBoundaries = true,
        bool addSemanticIndexes = true
    )
    {
        return new SliverPrototypeExtentList(
            key,
            new SliverChildListDelegate(
                children ?? [],
                addAutomaticKeepAlives: addAutomaticKeepAlives,
                addRepaintBoundaries: addRepaintBoundaries,
                addSemanticIndexes: addSemanticIndexes
            ),
            prototypeItem
        );
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        var element = ((_SliverPrototypeExtentListElement__sliver_prototype_extent_list?)context)!;
        return new _RenderSliverPrototypeExtentList__sliver_prototype_extent_list(
            childManager: element
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SliverMultiBoxAdaptorElement createElement() =>
        DartRuntimePrimitives.ConvertValue<SliverMultiBoxAdaptorElement>(
            new _SliverPrototypeExtentListElement__sliver_prototype_extent_list(this)
        );
}

public class _SliverPrototypeExtentListElement__sliver_prototype_extent_list
    : SliverMultiBoxAdaptorElement
{
    internal virtual Element? _prototype { get; set; } = default;
    internal static object _prototypeSlot = new object();

    internal _SliverPrototypeExtentListElement__sliver_prototype_extent_list(
        SliverPrototypeExtentList widget
    )
        : base(widget) { }

    public override _RenderSliverPrototypeExtentList__sliver_prototype_extent_list renderObject =>
        (_RenderSliverPrototypeExtentList__sliver_prototype_extent_list)base.renderObject;

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        if (Equals(slot, _prototypeSlot))
        {
            DartRuntimePrimitives.Assert(() => child is RenderBox);
            renderObject.child = ((RenderBox?)child)!;
        }
        else
        {
            base.insertRenderObjectChild(
                child,
                slot is long indexslot
                    ? indexslot
                    : throw new ArgumentException("A sliver child requires an index.", nameof(slot))
            );
        }
    }

    public override void didAdoptChild(RenderBox child)
    {
        if (!Equals(child, renderObject.child))
        {
            base.didAdoptChild(child);
        }
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __child = (RenderBox)child;
        if (Equals(newSlot, _prototypeSlot))
        {
            DartRuntimePrimitives.Assert(() => false);
        }
        else
        {
            base.moveRenderObjectChild(
                __child,
                oldSlot is long indexoldSlot
                    ? indexoldSlot
                    : throw new ArgumentException(
                        "A sliver child requires an index.",
                        nameof(oldSlot)
                    ),
                newSlot is long indexnewSlot
                    ? indexnewSlot
                    : throw new ArgumentException(
                        "A sliver child requires an index.",
                        nameof(newSlot)
                    )
            );
        }
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        var __child = (RenderBox)child;
        if (Equals(renderObject.child, __child))
        {
            renderObject.child = null;
        }
        else
        {
            base.removeRenderObjectChild(
                __child,
                slot is long indexslot
                    ? indexslot
                    : throw new ArgumentException("A sliver child requires an index.", nameof(slot))
            );
        }
    }

    public override void visitChildren(Action<Element> visitor)
    {
        if (_prototype is not null)
        {
            visitor(_prototype!);
        }
        base.visitChildren(visitor);
    }

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        _prototype = updateChild(
            _prototype,
            ((SliverPrototypeExtentList?)widget)!.prototypeItem,
            _prototypeSlot
        );
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (SliverPrototypeExtentList)newWidget;
        base.update(__newWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget, __newWidget));
        _prototype = updateChild(
            _prototype,
            ((SliverPrototypeExtentList?)widget)!.prototypeItem,
            _prototypeSlot
        );
    }
}

public class _RenderSliverPrototypeExtentList__sliver_prototype_extent_list
    : RenderSliverFixedExtentBoxAdaptor
{
    internal virtual RenderBox? _child { get; set; } = default;

    internal _RenderSliverPrototypeExtentList__sliver_prototype_extent_list(
        _SliverPrototypeExtentListElement__sliver_prototype_extent_list childManager
    )
        : base(childManager: childManager) { }

    public virtual RenderBox? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
            markNeedsLayout();
        }
    }

    public override void performLayout()
    {
        child!.layout(constraints.asBoxConstraints(), parentUsesSize: true);
        base.performLayout();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        _child?.detach();
    }

    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
        base.redepthChildren();
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
        base.visitChildren(visitor);
    }

    public override double? itemExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (child is not null) && child!.hasSize);
            return Equals(constraints.axis, Axis.vertical) ? child!.size.height : child!.size.width;
        }
    }
}

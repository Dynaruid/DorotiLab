// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/viewport.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class Viewport : MultiChildRenderObjectWidget
{
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public virtual AxisDirection? crossAxisDirection { get; private set; }
    public virtual double anchor { get; private set; } = default!;
    public virtual ViewportOffset offset { get; private set; } = default!;
    public virtual Key? center { get; private set; }
    public virtual double? cacheExtent { get; private set; }
    public virtual CacheExtentStyle cacheExtentStyle { get; private set; } = default!;
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual SliverPaintOrder paintOrder { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Viewport(
        Key? key = null,
        AxisDirection axisDirection = AxisDirection.down,
        AxisDirection? crossAxisDirection = null,
        double anchor = 0.0,
        ViewportOffset offset = default!,
        Key? center = null,
        double? cacheExtent = null,
        CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel,
        ScrollCacheExtent? scrollCacheExtent = null,
        SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop,
        Clip clipBehavior = Clip.hardEdge,
        List<Widget> slivers = default!
    )
        : base(key: key, children: slivers)
    {
        List<Widget> __slivers = slivers ?? new List<Widget>();
        this.axisDirection = axisDirection;
        this.crossAxisDirection = crossAxisDirection;
        this.anchor = anchor;
        this.offset = offset;
        this.center = center;
        this.cacheExtent = cacheExtent;
        this.cacheExtentStyle = cacheExtentStyle;
        this.scrollCacheExtent = scrollCacheExtent;
        this.paintOrder = paintOrder;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(
            (center is null)
                || (__slivers.where((child) => Equals(child.key, center)).Count() == 1L)
        );
        System.Diagnostics.Debug.Assert(
            (!Equals(cacheExtentStyle, CacheExtentStyle.viewport)) || (cacheExtent is not null)
        );
    }

    internal virtual ScrollCacheExtent? _effectiveScrollCacheExtent
    {
        get
        {
            if (scrollCacheExtent is not null)
            {
                return scrollCacheExtent;
            }
            if (cacheExtent is not null)
            {
                double cacheExtent__value6836 = (
                    cacheExtent
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                switch (cacheExtentStyle)
                {
                    case CacheExtentStyle.pixel:
                    {
                        return ScrollCacheExtent.CreatePixels(
                            (
                                cacheExtent
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        );
                    }
                    case CacheExtentStyle.viewport:
                    {
                        return ScrollCacheExtent.CreateViewport(
                            (
                                cacheExtent
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        );
                    }
                    default:
                        throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        );
                }
            }
            return null;
        }
    }

    public static AxisDirection getDefaultCrossAxisDirection(
        BuildContext context,
        AxisDirection axisDirection
    )
    {
        switch (axisDirection)
        {
            case AxisDirection.up:
            {
                DartRuntimePrimitives.Assert(() =>
                    DebugLibrary.debugCheckHasDirectionality(
                        context,
                        why: "to determine the cross-axis direction when the viewport has an 'up' axisDirection",
                        alternative: "Alternatively, consider specifying the 'crossAxisDirection' argument on the Viewport."
                    )
                );
                return Basic_typesLibrary.textDirectionToAxisDirection(Directionality.of(context));
            }
            case AxisDirection.right:
            {
                return AxisDirection.down;
            }
            case AxisDirection.down:
            {
                DartRuntimePrimitives.Assert(() =>
                    DebugLibrary.debugCheckHasDirectionality(
                        context,
                        why: "to determine the cross-axis direction when the viewport has a 'down' axisDirection",
                        alternative: "Alternatively, consider specifying the 'crossAxisDirection' argument on the Viewport."
                    )
                );
                return Basic_typesLibrary.textDirectionToAxisDirection(Directionality.of(context));
            }
            case AxisDirection.left:
            {
                return AxisDirection.down;
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderViewport(
            axisDirection: axisDirection,
            crossAxisDirection: crossAxisDirection
                ?? getDefaultCrossAxisDirection(context, axisDirection),
            anchor: anchor,
            offset: offset,
            scrollCacheExtent: _effectiveScrollCacheExtent,
            paintOrder: paintOrder,
            clipBehavior: clipBehavior
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderViewport)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderViewport>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.axisDirection = axisDirection;
                        __cascade.crossAxisDirection =
                            crossAxisDirection
                            ?? getDefaultCrossAxisDirection(context, axisDirection);
                        __cascade.anchor = anchor;
                        __cascade.offset = offset;
                        __cascade.scrollCacheExtent = _effectiveScrollCacheExtent;
                        __cascade.paintOrder = paintOrder;
                        __cascade.clipBehavior = clipBehavior;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override MultiChildRenderObjectElement createElement() =>
        DartRuntimePrimitives.ConvertValue<MultiChildRenderObjectElement>(
            new _ViewportElement__viewport(this)
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<AxisDirection>("axisDirection", axisDirection));
        properties.add(
            new EnumProperty<AxisDirection>(
                "crossAxisDirection",
                crossAxisDirection,
                defaultValue: null
            )
        );
        properties.add(new DoubleProperty("anchor", anchor));
        properties.add(new DiagnosticsProperty<ViewportOffset>("offset", offset));
        if (center is not null)
        {
            properties.add(new DiagnosticsProperty<Key>("center", center));
        }
        else
        {
            if (Enumerable.Any(children) && (children.First().key is not null))
            {
                properties.add(
                    new DiagnosticsProperty<Key>(
                        "center",
                        children.First().key,
                        tooltip: "implicit"
                    )
                );
            }
        }
        properties.add(
            new DiagnosticsProperty<ScrollCacheExtent>("scrollCacheExtent", scrollCacheExtent)
        );
    }
}

internal class _ViewportElement__viewport
    : MultiChildRenderObjectElement,
        NotifiableElementMixin,
        ViewportElementMixin
{
    internal virtual bool _doingMountOrUpdate { get; set; } = false;
    internal virtual long? _centerSlotIndex { get; set; } = default;

    internal _ViewportElement__viewport(Viewport widget)
        : base(widget) { }

    public override RenderViewport renderObject => (RenderViewport)base.renderObject;

    public override void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => !_doingMountOrUpdate);
        _doingMountOrUpdate = true;
        base.mount(parent, newSlot);
        _updateCenter();
        DartRuntimePrimitives.Assert(() => _doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (MultiChildRenderObjectWidget)newWidget;
        DartRuntimePrimitives.Assert(() => !_doingMountOrUpdate);
        _doingMountOrUpdate = true;
        base.update(__newWidget);
        _updateCenter();
        DartRuntimePrimitives.Assert(() => _doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    internal virtual void _updateCenter()
    {
        var viewport = ((Viewport?)widget)!;
        if (viewport.center is not null)
        {
            var elementIndex = 0L;
            foreach (Element e in children)
            {
                if (Equals(e.widget.key, viewport.center))
                {
                    renderObject.center = ((RenderSliver?)e.renderObject)!;
                    break;
                }
                elementIndex++;
            }
            DartRuntimePrimitives.Assert(() => elementIndex < children.Count());
            _centerSlotIndex = elementIndex;
        }
        else
        {
            if (Enumerable.Any(children))
            {
                renderObject.center = ((RenderSliver?)children.First().renderObject)!;
                _centerSlotIndex = 0L;
            }
            else
            {
                renderObject.center = null;
                _centerSlotIndex = null;
            }
        }
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        var __slot = (IndexedSlot<Element?>?)slot;
        base.insertRenderObjectChild(child, __slot);
        if (
            !_doingMountOrUpdate
            && (
                (
                    __slot
                    ?? throw new ArgumentException(
                        "A viewport child requires an indexed slot.",
                        nameof(slot)
                    )
                ).index == _centerSlotIndex
            )
        )
        {
            renderObject.center = ((RenderSliver?)child)!;
        }
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (IndexedSlot<Element?>?)oldSlot;
        var __newSlot = (IndexedSlot<Element?>?)newSlot;
        base.moveRenderObjectChild(child, __oldSlot, __newSlot);
        DartRuntimePrimitives.Assert(() => _doingMountOrUpdate);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        base.removeRenderObjectChild(child, slot);
        if (!_doingMountOrUpdate && Equals(renderObject.center, child))
        {
            renderObject.center = null;
        }
    }

    public override void debugVisitOnstageChildren(Action<Element> visitor)
    {
        children
            .where(
                (e) =>
                {
                    var renderSliver = ((RenderSliver?)e.renderObject!)!;
                    return renderSliver.geometry!.visible;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
            .forEach((__arg0) => visitor(__arg0));
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(_parent?._notificationTree, this);
    }

    public virtual bool onNotification(Notification notification)
    {
        if (notification is ViewportNotificationMixin)
        {
            ((ViewportNotificationMixin)notification)._depth += 1L;
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ShrinkWrappingViewport : MultiChildRenderObjectWidget
{
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public virtual AxisDirection? crossAxisDirection { get; private set; }
    public virtual ViewportOffset offset { get; private set; } = default!;
    public virtual SliverPaintOrder paintOrder { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual CacheExtentStyle cacheExtentStyle { get; private set; } = default!;
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }

    public ShrinkWrappingViewport(
        Key? key = null,
        AxisDirection axisDirection = AxisDirection.down,
        AxisDirection? crossAxisDirection = null,
        ViewportOffset offset = default!,
        SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop,
        Clip clipBehavior = Clip.hardEdge,
        double? cacheExtent = null,
        CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel,
        ScrollCacheExtent? scrollCacheExtent = null,
        List<Widget> slivers = default!
    )
        : base(key: key, children: slivers)
    {
        List<Widget> __slivers = slivers ?? new List<Widget>();
        this.axisDirection = axisDirection;
        this.crossAxisDirection = crossAxisDirection;
        this.offset = offset;
        this.paintOrder = paintOrder;
        this.clipBehavior = clipBehavior;
        this.cacheExtent = cacheExtent;
        this.cacheExtentStyle = cacheExtentStyle;
        this.scrollCacheExtent = scrollCacheExtent;
    }

    internal virtual ScrollCacheExtent? _effectiveScrollCacheExtent
    {
        get
        {
            if (scrollCacheExtent is not null)
            {
                return scrollCacheExtent;
            }
            if (cacheExtent is not null)
            {
                double cacheExtent__value17671 = (
                    cacheExtent
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                switch (cacheExtentStyle)
                {
                    case CacheExtentStyle.pixel:
                    {
                        return ScrollCacheExtent.CreatePixels(
                            (
                                cacheExtent
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        );
                    }
                    case CacheExtentStyle.viewport:
                    {
                        return ScrollCacheExtent.CreateViewport(
                            (
                                cacheExtent
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        );
                    }
                    default:
                        throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        );
                }
            }
            return null;
        }
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new RenderShrinkWrappingViewport(
            axisDirection: axisDirection,
            crossAxisDirection: crossAxisDirection
                ?? Viewport.getDefaultCrossAxisDirection(context, axisDirection),
            offset: offset,
            paintOrder: paintOrder,
            clipBehavior: clipBehavior,
            scrollCacheExtent: _effectiveScrollCacheExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderShrinkWrappingViewport)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<RenderShrinkWrappingViewport>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.axisDirection = axisDirection;
                        __cascade.crossAxisDirection =
                            crossAxisDirection
                            ?? Viewport.getDefaultCrossAxisDirection(context, axisDirection);
                        __cascade.offset = offset;
                        __cascade.paintOrder = paintOrder;
                        __cascade.clipBehavior = clipBehavior;
                        __cascade.scrollCacheExtent = _effectiveScrollCacheExtent;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<AxisDirection>("axisDirection", axisDirection));
        properties.add(
            new EnumProperty<AxisDirection>(
                "crossAxisDirection",
                crossAxisDirection,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<ViewportOffset>("offset", offset));
        properties.add(
            new DiagnosticsProperty<ScrollCacheExtent>(
                "scrollCacheExtent",
                scrollCacheExtent,
                defaultValue: null
            )
        );
    }
}

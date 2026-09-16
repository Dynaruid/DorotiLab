// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/viewport.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class Viewport : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection { get; private set; }
    public virtual double anchor { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.Key? center { get; private set; }
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Viewport(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection = null, double anchor = 0.0, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Foundation.Key? center = null, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge, List<Widget> slivers = default!) : base(key: key, children: slivers)
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
        System.Diagnostics.Debug.Assert(((center is null) || (__slivers.where(((child) => (Equals(((Widget)child).key, center)))).Count() == 1L)));
        System.Diagnostics.Debug.Assert(((!Equals(cacheExtentStyle, CacheExtentStyle.viewport)) || (cacheExtent is not null)));
    }

    internal virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? _effectiveScrollCacheExtent
    {
        get
        {
            if ((this.scrollCacheExtent is not null))
            {
                return this.scrollCacheExtent;
            }
            if ((this.cacheExtent is not null))
            {
                double cacheExtent__value6836 = DartRuntimePrimitives.RequireValue(cacheExtent);
                switch (this.cacheExtentStyle)
                {
                    case CacheExtentStyle.pixel:
                        {
                            return ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    case CacheExtentStyle.viewport:
                        {
                            return ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            return ((global::Doroti.Framework.Rendering.ScrollCacheExtent?)null);
        }
    }
    public static global::Doroti.Framework.Painting.AxisDirection getDefaultCrossAxisDirection(BuildContext context, global::Doroti.Framework.Painting.AxisDirection axisDirection)
    {
        switch (axisDirection)
        {
            case AxisDirection.up:
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context, why: "to determine the cross-axis direction when the viewport has an 'up' axisDirection", alternative: "Alternatively, consider specifying the 'crossAxisDirection' argument on the Viewport."));
                    return Basic_typesLibrary.textDirectionToAxisDirection(Directionality.of(context));
                }
            case AxisDirection.right:
                {
                    return AxisDirection.down;
                }
            case AxisDirection.down:
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context, why: "to determine the cross-axis direction when the viewport has a 'down' axisDirection", alternative: "Alternatively, consider specifying the 'crossAxisDirection' argument on the Viewport."));
                    return Basic_typesLibrary.textDirectionToAxisDirection(Directionality.of(context));
                }
            case AxisDirection.left:
                {
                    return AxisDirection.down;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)new global::Doroti.Framework.Rendering.RenderViewport(axisDirection: this.axisDirection, crossAxisDirection: ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)getDefaultCrossAxisDirection(context, this.axisDirection))), anchor: this.anchor, offset: this.offset, scrollCacheExtent: this._effectiveScrollCacheExtent, paintOrder: this.paintOrder, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderViewport)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderViewport>)(() =>
{
    var __cascade = __renderObject;
    __cascade.axisDirection = this.axisDirection;
    __cascade.crossAxisDirection = ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)getDefaultCrossAxisDirection(context, this.axisDirection)));
    __cascade.anchor = this.anchor;
    __cascade.offset = this.offset;
    __cascade.scrollCacheExtent = this._effectiveScrollCacheExtent;
    __cascade.paintOrder = this.paintOrder;
    __cascade.clipBehavior = this.clipBehavior;
    return __cascade;
}))());
    }

    public override MultiChildRenderObjectElement createElement() => DartRuntimePrimitives.ConvertValue<MultiChildRenderObjectElement>(new _ViewportElement__viewport(this));
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("crossAxisDirection", this.crossAxisDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("anchor", this.anchor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.ViewportOffset>("offset", this.offset));
        if ((this.center is not null))
        {
            properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Foundation.Key>("center", this.center));
        }
        else
        {
            if ((Enumerable.Any(this.children) && (this.children.First().key is not null)))
            {
                properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Foundation.Key>("center", this.children.First().key, tooltip: "implicit"));
            }
        }
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.ScrollCacheExtent>("scrollCacheExtent", this.scrollCacheExtent));
    }

}

internal class _ViewportElement__viewport : MultiChildRenderObjectElement, NotifiableElementMixin, ViewportElementMixin
{
    internal virtual bool _doingMountOrUpdate { get; set; } = false;
    internal virtual long? _centerSlotIndex { get; set; } = default;

    internal _ViewportElement__viewport(Viewport widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderViewport renderObject => (global::Doroti.Framework.Rendering.RenderViewport)base.renderObject;
    public override void mount(Element? parent, object? newSlot)
    {
        DartRuntimePrimitives.Assert(() => !this._doingMountOrUpdate);
        _doingMountOrUpdate = true;
        base.mount(parent, newSlot);
        _updateCenter();
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (MultiChildRenderObjectWidget)newWidget;
        DartRuntimePrimitives.Assert(() => !this._doingMountOrUpdate);
        _doingMountOrUpdate = true;
        base.update(__newWidget);
        _updateCenter();
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    internal virtual void _updateCenter()
    {
        var viewport = ((Viewport?)this.widget)!;
        if ((((Viewport)viewport).center is not null))
        {
            var elementIndex = 0L;
            foreach (Element e in this.children)
            {
                if ((Equals(((Element)e).widget.key, ((Viewport)viewport).center)))
                {
                    this.renderObject.center = ((global::Doroti.Framework.Rendering.RenderSliver?)((Element)e).renderObject)!;
                    break;
                }
                elementIndex++;
            }
            DartRuntimePrimitives.Assert(() => (elementIndex < this.children.Count()));
            _centerSlotIndex = elementIndex;
        }
        else
        {
            if (Enumerable.Any(this.children))
            {
                this.renderObject.center = ((global::Doroti.Framework.Rendering.RenderSliver?)this.children.First().renderObject)!;
                _centerSlotIndex = 0L;
            }
            else
            {
                this.renderObject.center = null;
                _centerSlotIndex = null;
            }
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __slot = (IndexedSlot<Element?>?)slot;
        base.insertRenderObjectChild(child, __slot);
        if ((!this._doingMountOrUpdate && (( __slot as IndexedSlot<Element?> ?? throw new ArgumentException("A viewport child requires an indexed slot.", nameof(slot))).index == this._centerSlotIndex)))
        {
            this.renderObject.center = ((global::Doroti.Framework.Rendering.RenderSliver?)child)!;
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (IndexedSlot<Element?>?)oldSlot;
        var __newSlot = (IndexedSlot<Element?>?)newSlot;
        base.moveRenderObjectChild(child, __oldSlot, __newSlot);
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        base.removeRenderObjectChild(child, slot);
        if ((!this._doingMountOrUpdate && (Equals(((global::Doroti.Framework.Rendering.RenderViewport)this.renderObject).center, child))))
        {
            this.renderObject.center = null;
        }
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        this.children.where(((e) =>
        {
            var renderSliver = ((global::Doroti.Framework.Rendering.RenderSliver?)((Element)e).renderObject!)!;
            return ((global::Doroti.Framework.Rendering.RenderSliver)renderSliver).geometry!.visible;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).forEach((__arg0) => ((global::System.Action<Element>)visitor)(__arg0));
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(this._parent?._notificationTree, this);
    }

    public virtual bool onNotification(Notification notification)
    {
        if ((notification is ViewportNotificationMixin))
        {
            ((ViewportNotificationMixin)notification)._depth += 1L;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ShrinkWrappingViewport : MultiChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection { get; private set; }
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }

    public ShrinkWrappingViewport(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection = null, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> slivers = default!) : base(key: key, children: slivers)
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

    internal virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? _effectiveScrollCacheExtent
    {
        get
        {
            if ((this.scrollCacheExtent is not null))
            {
                return this.scrollCacheExtent;
            }
            if ((this.cacheExtent is not null))
            {
                double cacheExtent__value17671 = DartRuntimePrimitives.RequireValue(cacheExtent);
                switch (this.cacheExtentStyle)
                {
                    case CacheExtentStyle.pixel:
                        {
                            return ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    case CacheExtentStyle.viewport:
                        {
                            return ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            return ((global::Doroti.Framework.Rendering.ScrollCacheExtent?)null);
        }
    }
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)new global::Doroti.Framework.Rendering.RenderShrinkWrappingViewport(axisDirection: this.axisDirection, crossAxisDirection: ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection))), offset: this.offset, paintOrder: this.paintOrder, clipBehavior: this.clipBehavior, scrollCacheExtent: this._effectiveScrollCacheExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderShrinkWrappingViewport)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderShrinkWrappingViewport>)(() =>
{
    var __cascade = __renderObject;
    __cascade.axisDirection = this.axisDirection;
    __cascade.crossAxisDirection = ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection)));
    __cascade.offset = this.offset;
    __cascade.paintOrder = this.paintOrder;
    __cascade.clipBehavior = this.clipBehavior;
    __cascade.scrollCacheExtent = this._effectiveScrollCacheExtent;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("crossAxisDirection", this.crossAxisDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.ViewportOffset>("offset", this.offset));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.ScrollCacheExtent>("scrollCacheExtent", this.scrollCacheExtent, defaultValue: null));
    }

}


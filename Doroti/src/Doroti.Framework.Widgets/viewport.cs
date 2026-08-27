// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/viewport.dart
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

    public Viewport(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = global::Doroti.Framework.Painting.AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection = null, double anchor = 0.0, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Foundation.Key? center = null, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle = global::Doroti.Framework.Rendering.CacheExtentStyle.pixel, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder = global::Doroti.Framework.Rendering.SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge, List<Widget> slivers = default!) : base(key: key, children: slivers)
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
        System.Diagnostics.Debug.Assert(((center is null) || (__slivers.where(((child) => (object.Equals(((Widget)child).key, center)))).Count() == 1L)));
        System.Diagnostics.Debug.Assert(((!object.Equals(cacheExtentStyle, global::Doroti.Framework.Rendering.CacheExtentStyle.viewport)) || (cacheExtent is not null)));
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
                    case global::Doroti.Framework.Rendering.CacheExtentStyle.pixel:
                        {
                            return global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    case global::Doroti.Framework.Rendering.CacheExtentStyle.viewport:
                        {
                            return global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            return ((global::Doroti.Framework.Rendering.ScrollCacheExtent)(object)null);
            return default!;
        }
    }
    public static global::Doroti.Framework.Painting.AxisDirection getDefaultCrossAxisDirection(BuildContext context, global::Doroti.Framework.Painting.AxisDirection axisDirection)
    {
        switch (axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to determine the cross-axis direction when the viewport has an 'up' axisDirection", alternative: "Alternatively, consider specifying the 'crossAxisDirection' argument on the Viewport."));
                    return global::Doroti.Framework.Painting.Basic_typesLibrary.textDirectionToAxisDirection(Directionality.of(context));
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    return global::Doroti.Framework.Painting.AxisDirection.down;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to determine the cross-axis direction when the viewport has a 'down' axisDirection", alternative: "Alternatively, consider specifying the 'crossAxisDirection' argument on the Viewport."));
                    return global::Doroti.Framework.Painting.Basic_typesLibrary.textDirectionToAxisDirection(Directionality.of(context));
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    return global::Doroti.Framework.Painting.AxisDirection.down;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderViewport(axisDirection: this.axisDirection, crossAxisDirection: ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection))), anchor: this.anchor, offset: this.offset, scrollCacheExtent: this._effectiveScrollCacheExtent, paintOrder: this.paintOrder, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderViewport)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderViewport>)(() =>
{
    var __cascade = __renderObject;
    __cascade.axisDirection = this.axisDirection;
    __cascade.crossAxisDirection = ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection)));
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
            if ((System.Linq.Enumerable.Any(this.children) && (this.children.First().key is not null)))
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

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((global::Doroti.Framework.Rendering.RenderViewport?)(object?)base.renderObject)!);
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
        var __newWidget = (MultiChildRenderObjectWidget)(object)newWidget;
        DartRuntimePrimitives.Assert(() => !this._doingMountOrUpdate);
        _doingMountOrUpdate = true;
        base.update(__newWidget);
        _updateCenter();
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
        _doingMountOrUpdate = false;
    }

    internal virtual void _updateCenter()
    {
        var viewport = ((Viewport?)(object?)this.widget)!;
        if ((((Viewport)viewport).center is not null))
        {
            var elementIndex = 0L;
            foreach (Element e in this.children)
            {
                if ((object.Equals(((Element)e).widget.key, ((Viewport)viewport).center)))
                {
                    ((dynamic)this.renderObject).center = ((global::Doroti.Framework.Rendering.RenderSliver?)(object?)((Element)e).renderObject)!;
                    break;
                }
                elementIndex++;
            }
            DartRuntimePrimitives.Assert(() => (elementIndex < this.children.Count()));
            _centerSlotIndex = elementIndex;
        }
        else
        {
            if (System.Linq.Enumerable.Any(this.children))
            {
                ((dynamic)this.renderObject).center = ((global::Doroti.Framework.Rendering.RenderSliver?)(object?)this.children.First().renderObject)!;
                _centerSlotIndex = 0L;
            }
            else
            {
                ((dynamic)this.renderObject).center = null;
                _centerSlotIndex = null;
            }
        }
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        var __slot = (IndexedSlot<Element?>)(object)slot;
        base.insertRenderObjectChild(child, __slot);
        if ((!this._doingMountOrUpdate && (((IndexedSlot<Element?>)__slot).index == this._centerSlotIndex)))
        {
            ((dynamic)this.renderObject).center = ((global::Doroti.Framework.Rendering.RenderSliver?)(object?)child)!;
        }
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        var __oldSlot = (IndexedSlot<Element?>)(object)oldSlot;
        var __newSlot = (IndexedSlot<Element?>)(object)newSlot;
        base.moveRenderObjectChild(child, __oldSlot, __newSlot);
        DartRuntimePrimitives.Assert(() => this._doingMountOrUpdate);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        base.removeRenderObjectChild(child, slot);
        if ((!this._doingMountOrUpdate && (object.Equals(((global::Doroti.Framework.Rendering.RenderViewport)this.renderObject).center, child))))
        {
            ((dynamic)this.renderObject).center = null;
        }
    }

    public override void debugVisitOnstageChildren(global::System.Action<Element> visitor)
    {
        this.children.where(((e) =>
        {
            var renderSliver = ((global::Doroti.Framework.Rendering.RenderSliver?)(object?)((Element)e).renderObject!)!;
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

    public ShrinkWrappingViewport(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = global::Doroti.Framework.Painting.AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection = null, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder = global::Doroti.Framework.Rendering.SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge, double? cacheExtent = null, global::Doroti.Framework.Rendering.CacheExtentStyle cacheExtentStyle = global::Doroti.Framework.Rendering.CacheExtentStyle.pixel, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> slivers = default!) : base(key: key, children: slivers)
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
                    case global::Doroti.Framework.Rendering.CacheExtentStyle.pixel:
                        {
                            return global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    case global::Doroti.Framework.Rendering.CacheExtentStyle.viewport:
                        {
                            return global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(this.cacheExtent));
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
            }
            return ((global::Doroti.Framework.Rendering.ScrollCacheExtent)(object)null);
            return default!;
        }
    }
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderShrinkWrappingViewport(axisDirection: this.axisDirection, crossAxisDirection: ((this.crossAxisDirection ?? (global::Doroti.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection))), offset: this.offset, paintOrder: this.paintOrder, clipBehavior: this.clipBehavior, scrollCacheExtent: this._effectiveScrollCacheExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderShrinkWrappingViewport)(object)renderObject;
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


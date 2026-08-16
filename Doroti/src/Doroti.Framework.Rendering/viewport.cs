// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/viewport.dart
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

namespace Doroti.Framework.Rendering;

public interface ScrollCacheExtent
{
    public static ScrollCacheExtent CreatePixels(double pixels)
        => new _PixelScrollCacheExtent__viewport(pixels);

    public static ScrollCacheExtent CreateViewport(double value)
        => new _ViewportScrollCacheExtent__viewport(value);

    public double _calculateCacheOffset(double mainAxisExtent);
    public CacheExtentStyle style { get; }
    public double value { get; }
}

internal class _PixelScrollCacheExtent__viewport : ScrollCacheExtent
{
    public virtual double pixels { get; private set; } = default!;

    internal _PixelScrollCacheExtent__viewport(double pixels)
    {
        this.pixels = pixels;
    }

    public virtual double _calculateCacheOffset(double mainAxisExtent) => this.pixels;
    public virtual CacheExtentStyle style => CacheExtentStyle.pixel;
    public virtual double value => this.pixels;
    public override string ToString()
    {
        return $"ScrollCacheExtent.pixels({this.pixels})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _PixelScrollCacheExtent__viewport;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return ((__other is _PixelScrollCacheExtent__viewport) && (((_PixelScrollCacheExtent__viewport)((_PixelScrollCacheExtent__viewport)__other)).pixels == this.pixels));
    }

    public override int GetHashCode() => this.pixels.GetHashCode();
}

internal class _ViewportScrollCacheExtent__viewport : ScrollCacheExtent
{
    public virtual double value { get; private set; } = default!;

    internal _ViewportScrollCacheExtent__viewport(double value)
    {
        this.value = value;
    }

    public virtual double _calculateCacheOffset(double mainAxisExtent) => (this.value * mainAxisExtent);
    public virtual CacheExtentStyle style => CacheExtentStyle.viewport;
    public override string ToString()
    {
        return $"ScrollCacheExtent.viewport({this.value})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ViewportScrollCacheExtent__viewport;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return ((__other is _ViewportScrollCacheExtent__viewport) && (((_ViewportScrollCacheExtent__viewport)((_ViewportScrollCacheExtent__viewport)__other)).value == this.value));
    }

    public override int GetHashCode() => this.value.GetHashCode();
}

public enum CacheExtentStyle
{
    pixel,
    viewport
}

public enum SliverPaintOrder
{
    firstIsTop,
    lastIsTop
}

public abstract class RenderAbstractViewport : RenderObject
{
    public const double defaultCacheExtent = 250.0;

    public static RenderAbstractViewport? maybeOf(RenderObject? @object)
    {
        while ((@object is not null))
        {
            if ((@object is RenderAbstractViewport))
            {
                RenderAbstractViewport @object__as6686 = (RenderAbstractViewport)@object;
                return ((RenderAbstractViewport)@object__as6686);
            }
            @object = ((RenderObject)@object).parent;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static RenderAbstractViewport of(RenderObject? @object)
    {
        RenderAbstractViewport? viewport__7359 = maybeOf(@object);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((viewport__7359 is null))
                {
                    throw new FlutterError("RenderAbstractViewport.of() was called with a render object that was " + "not a descendant of a RenderAbstractViewport.\n" + "No RenderAbstractViewport render object ancestor could be found starting " + "from the object that was passed to RenderAbstractViewport.of().\n" + "The render object where the viewport search started was:\n" + $"  {@object}");
                }
                return true;
            });
        return viewport__7359!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract RevealedOffset getOffsetToReveal(RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null);
}

public class RevealedOffset
{
    public virtual double offset { get; private set; } = default!;
    public virtual Rect rect { get; private set; } = default!;

    public RevealedOffset(double offset, Rect rect)
    {
        this.offset = offset;
        this.rect = rect;
    }

    public static RevealedOffset? clampOffset(RevealedOffset leadingEdgeOffset, RevealedOffset trailingEdgeOffset, double currentOffset)
    {
        bool inverted__14162 = (((RevealedOffset)leadingEdgeOffset).offset < ((RevealedOffset)trailingEdgeOffset).offset);
        RevealedOffset smaller__14252 = default!;
        RevealedOffset larger__14286 = default!;
        (smaller__14252, larger__14286) = (inverted__14162 ? (((RevealedOffset, RevealedOffset))(leadingEdgeOffset, trailingEdgeOffset)) : (((RevealedOffset, RevealedOffset))(trailingEdgeOffset, leadingEdgeOffset)));
        if ((currentOffset > ((RevealedOffset)larger__14286).offset))
        {
            return larger__14286;
        }
        else
        {
            if ((currentOffset < ((RevealedOffset)smaller__14252).offset))
            {
                return smaller__14252;
            }
            else
            {
                return null;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RevealedOffset"))}(offset: {this.offset}, rect: {this.rect})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RenderViewportBase<ParentDataClass> : RenderBox, ContainerRenderObjectMixin<RenderSliver, ParentDataClass> where ParentDataClass : ContainerParentDataMixin<RenderSliver>
{
    internal virtual global::Doroti.Framework.Painting.AxisDirection _axisDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.AxisDirection _crossAxisDirection { get; set; } = default!;
    internal virtual ViewportOffset _offset { get; set; } = default!;
    internal virtual ScrollCacheExtent _scrollCacheExtent { get; set; } = default!;
    internal virtual double? _calculatedCacheExtent { get; set; } = default;
    internal virtual SliverPaintOrder _paintOrder { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderSliver? _firstChild { get; set; } = default;
    public virtual RenderSliver? _lastChild { get; set; } = default;

    protected RenderViewportBase(global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection crossAxisDirection = default!, ViewportOffset offset = default!, double? cacheExtent = null, CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel, ScrollCacheExtent? scrollCacheExtent = null, SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge)
    {
        this._axisDirection = axisDirection;
        this._crossAxisDirection = crossAxisDirection;
        this._offset = offset;
        this._scrollCacheExtent = (scrollCacheExtent ?? (cacheExtentStyle switch { CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels((cacheExtent ?? RenderAbstractViewport.defaultCacheExtent)), CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(cacheExtent)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        this._paintOrder = paintOrder;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((!object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(axisDirection), global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(crossAxisDirection))));
        System.Diagnostics.Debug.Assert(((cacheExtent is not null) || (object.Equals(cacheExtentStyle, CacheExtentStyle.pixel))));
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.addTagForChildren(RenderViewport.useTwoPaneSemantics);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        this.childrenInPaintOrder.where(sliver =>
        {
            var geometry = ((RenderSliver)sliver).geometry;
            return geometry is not null && (geometry.visible || geometry.cacheExtent > 0.0 || ((RenderSliver)sliver).ensureSemantics);
        }).forEach(visitor);
    }

    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection
    {
        get => this._axisDirection;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._axisDirection)))
            {
                return;
            }
            _axisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.AxisDirection crossAxisDirection
    {
        get => this._crossAxisDirection;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._crossAxisDirection)))
            {
                return;
            }
            _crossAxisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.Axis axis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public virtual ViewportOffset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._offset)))
            {
                return;
            }
            if (attached)
            {
                this._offset.removeListener(markNeedsLayout);
            }
            _offset = __value;
            if (attached)
            {
                this._offset.addListener(markNeedsLayout);
            }
            markNeedsLayout();
        }
    }
    public virtual double? cacheExtent
    {
        get => ((ScrollCacheExtent)this._scrollCacheExtent).value;
        set
        {
            var __value = value;
            if ((__value == this.cacheExtent))
            {
                return;
            }
            if ((__value is null))
            {
                _scrollCacheExtent = ScrollCacheExtent.CreatePixels(RenderAbstractViewport.defaultCacheExtent);
            }
            else
            {
                _scrollCacheExtent = (this._scrollCacheExtent switch { _PixelScrollCacheExtent__viewport __object21598 => ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(__value)), _ViewportScrollCacheExtent__viewport __object21668 => ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(__value)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
            markNeedsLayout();
        }
    }
    public virtual ScrollCacheExtent scrollCacheExtent
    {
        get => this._scrollCacheExtent;
        set
        {
            var __value = value;
            ScrollCacheExtent effectiveValue__23373 = (__value ?? ScrollCacheExtent.CreatePixels(RenderAbstractViewport.defaultCacheExtent));
            if ((object.Equals(effectiveValue__23373, this._scrollCacheExtent)))
            {
                return;
            }
            _scrollCacheExtent = effectiveValue__23373;
            markNeedsLayout();
        }
    }
    public virtual CacheExtentStyle cacheExtentStyle
    {
        get => ((ScrollCacheExtent)this._scrollCacheExtent).style;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this.cacheExtentStyle)))
            {
                return;
            }
            _scrollCacheExtent = (DartRuntimePrimitives.RequireValue(__value) switch { CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(this.cacheExtent)), CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(this.cacheExtent)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            markNeedsLayout();
        }
    }
    public virtual SliverPaintOrder paintOrder
    {
        get => this._paintOrder;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._paintOrder)))
            {
                _paintOrder = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderSliver? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((ParentDataClass?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        this._offset.addListener(markNeedsLayout);
    }

    public override void detach()
    {
        this._offset.removeListener(markNeedsLayout);
        base.detach();
        RenderSliver? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((ParentDataClass?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public virtual bool debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!RenderObject.debugCheckingIntrinsics)
                {
                    DartRuntimePrimitives.Assert(() => (this is not RenderShrinkWrappingViewport));
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} does not support returning intrinsic dimensions."), new ErrorDescription("Calculating the intrinsic dimensions would require instantiating every child of " + "the viewport, which defeats the point of viewports being lazy."), new ErrorHint("If you are merely trying to shrink-wrap the viewport in the main axis direction, " + "consider a RenderShrinkWrappingViewport render object (ShrinkWrappingViewport widget), " + "which achieves that effect without implementing the intrinsic dimension API.") });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        DartRuntimePrimitives.Assert(() => debugThrowIfNotCheckingIntrinsics());
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isRepaintBoundary => true;
    public virtual double layoutChildSequence(RenderSliver? child, double scrollOffset, double overlap, double layoutOffset, double remainingPaintExtent, double mainAxisExtent, double crossAxisExtent, GrowthDirection growthDirection, Func<RenderSliver, RenderSliver?> advance, double remainingCacheExtent, double cacheOrigin)
    {
        DartRuntimePrimitives.Assert(() => double.IsFinite(scrollOffset));
        DartRuntimePrimitives.Assert(() => (scrollOffset >= 0.0));
        var initialLayoutOffset__30333 = layoutOffset;
        ScrollDirection adjustedUserScrollDirection__30395 = global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToScrollDirection(((ViewportOffset)this.offset).userScrollDirection, growthDirection);
        double maxPaintOffset__30539 = (layoutOffset + overlap);
        var precedingScrollExtent__30588 = 0.0;
        while ((child is not null))
        {
            var sliverScrollOffset__30658 = ((scrollOffset <= 0.0) ? 0.0 : scrollOffset);
            double correctedCacheOrigin__30911 = Math.Max(cacheOrigin, -sliverScrollOffset__30658);
            double cacheExtentCorrection__30997 = (cacheOrigin - correctedCacheOrigin__30911);
            DartRuntimePrimitives.Assert(() => (sliverScrollOffset__30658 >= correctedCacheOrigin__30911.abs()));
            DartRuntimePrimitives.Assert(() => (correctedCacheOrigin__30911 <= 0.0));
            DartRuntimePrimitives.Assert(() => (sliverScrollOffset__30658 >= 0.0));
            DartRuntimePrimitives.Assert(() => (cacheExtentCorrection__30997 <= 0.0));
            child.layout(new SliverConstraints(axisDirection: this.axisDirection, growthDirection: growthDirection, userScrollDirection: adjustedUserScrollDirection__30395, scrollOffset: sliverScrollOffset__30658, precedingScrollExtent: precedingScrollExtent__30588, overlap: (maxPaintOffset__30539 - layoutOffset), remainingPaintExtent: Math.Max(0.0, ((remainingPaintExtent - layoutOffset) + initialLayoutOffset__30333)), crossAxisExtent: crossAxisExtent, crossAxisDirection: this.crossAxisDirection, viewportMainAxisExtent: mainAxisExtent, remainingCacheExtent: Math.Max(0.0, (remainingCacheExtent + cacheExtentCorrection__30997)), cacheOrigin: correctedCacheOrigin__30911), parentUsesSize: true);
            SliverGeometry childLayoutGeometry__32095 = ((RenderSliver)child).geometry!;
            DartRuntimePrimitives.Assert(() => childLayoutGeometry__32095.debugAssertIsValid());
            if ((((SliverGeometry)childLayoutGeometry__32095).scrollOffsetCorrection is not null))
            {
                return DartRuntimePrimitives.RequireValue(((SliverGeometry)childLayoutGeometry__32095).scrollOffsetCorrection);
            }
            double effectiveLayoutOffset__32545 = (layoutOffset + ((SliverGeometry)childLayoutGeometry__32095).paintOrigin);
            if ((((SliverGeometry)childLayoutGeometry__32095).visible || (scrollOffset > 0L)))
            {
                updateChildLayoutOffset(child, effectiveLayoutOffset__32545, growthDirection);
            }
            else
            {
                updateChildLayoutOffset(child, (-scrollOffset + initialLayoutOffset__30333), growthDirection);
            }
            maxPaintOffset__30539 = Math.Max((effectiveLayoutOffset__32545 + ((SliverGeometry)childLayoutGeometry__32095).paintExtent), maxPaintOffset__30539);
            scrollOffset -= ((SliverGeometry)childLayoutGeometry__32095).scrollExtent;
            precedingScrollExtent__30588 += ((SliverGeometry)childLayoutGeometry__32095).scrollExtent;
            layoutOffset += ((SliverGeometry)childLayoutGeometry__32095).layoutExtent;
            if ((((SliverGeometry)childLayoutGeometry__32095).cacheExtent != 0.0))
            {
                remainingCacheExtent -= (((SliverGeometry)childLayoutGeometry__32095).cacheExtent - cacheExtentCorrection__30997);
                cacheOrigin = Math.Min((correctedCacheOrigin__30911 + ((SliverGeometry)childLayoutGeometry__32095).cacheExtent), 0.0);
            }
            updateOutOfBandData(growthDirection, childLayoutGeometry__32095);
            child = advance(child);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        if ((((RenderSliver)__child).ensureSemantics && !((((RenderSliver)__child).geometry!.visible || (((RenderSliver)__child).geometry!.cacheExtent > 0.0)))))
        {
            return null;
        }
        switch (this.clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    break;
                }
        }
        global::Doroti.Ui.Rect viewportClip__34470 = (Offset.zero & size);
        if (((((RenderSliver)__child).constraints.overlap == 0L) || !double.IsFinite(((RenderSliver)__child).constraints.viewportMainAxisExtent)))
        {
            return viewportClip__34470;
        }
        double left__35313 = viewportClip__34470.left;
        double right__35350 = viewportClip__34470.right;
        double top__35389 = viewportClip__34470.top;
        double bottom__35424 = viewportClip__34470.bottom;
        double startOfOverlap__35471 = (((RenderSliver)__child).constraints.viewportMainAxisExtent - ((RenderSliver)__child).constraints.remainingPaintExtent);
        double overlapCorrection__35596 = (startOfOverlap__35471 + ((RenderSliver)__child).constraints.overlap);
        switch (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(this.axisDirection, ((RenderSliver)__child).constraints.growthDirection))
        {
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    top__35389 += overlapCorrection__35596;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    bottom__35424 -= overlapCorrection__35596;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    left__35313 += overlapCorrection__35596;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    right__35350 -= overlapCorrection__35596;
                    break;
                }
        }
        return global::Doroti.Ui.Rect.fromLTRB(left__35313, top__35389, right__35350, bottom__35424);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect? describeSemanticsClip(RenderObject? child)
    {
        var __child = child is null ? null : (RenderSliver)(object)child;
        if ((((__child is not null) && ((RenderSliver)__child).ensureSemantics) && !((((RenderSliver)__child).geometry!.visible || (((RenderSliver)__child).geometry!.cacheExtent > 0.0)))))
        {
            return null;
        }
        if ((this._calculatedCacheExtent is null))
        {
            return semanticBounds;
        }
        switch (this.axis)
        {
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    return global::Doroti.Ui.Rect.fromLTRB(semanticBounds.left, (semanticBounds.top - DartRuntimePrimitives.RequireValue(this._calculatedCacheExtent)), semanticBounds.right, (semanticBounds.bottom + DartRuntimePrimitives.RequireValue(this._calculatedCacheExtent)));
                }
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    return global::Doroti.Ui.Rect.fromLTRB((semanticBounds.left - DartRuntimePrimitives.RequireValue(this._calculatedCacheExtent)), semanticBounds.top, (semanticBounds.right + DartRuntimePrimitives.RequireValue(this._calculatedCacheExtent)), semanticBounds.bottom);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((firstChild is null))
        {
            return;
        }
        if ((this.hasVisualOverflow && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)this._paintContents, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            _paintContents(context, offset);
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    internal virtual void _paintContents(PaintingContext context, Offset offset)
    {
        foreach (RenderSliver child__37854 in this.childrenInPaintOrder)
        {
            if (((RenderSliver)child__37854).geometry!.visible)
            {
                context.paintChild(child__37854, (offset + paintOffsetOf(child__37854)));
            }
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                base.debugPaintSize(context, offset);
                var paint__38158 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Ui.Color(4278255360L);
    return __cascade;
}))();
                global::Doroti.Ui.Canvas canvas__38303 = ((PaintingContext)context).canvas;
                RenderSliver? child__38348 = firstChild;
                while ((child__38348 is not null))
                {
                    global::Doroti.Ui.Size size__38417 = (this.axis switch { global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Size(((RenderSliver)child__38348).constraints.crossAxisExtent, ((RenderSliver)child__38348).geometry!.layoutExtent), global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Size(((RenderSliver)child__38348).geometry!.layoutExtent, ((RenderSliver)child__38348).constraints.crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                    canvas__38303.drawRect(((((offset + paintOffsetOf(child__38348))) & size__38417)).deflate(0.5), paint__38158);
                    child__38348 = childAfter(child__38348);
                }
                return true;
            });
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        var (mainAxisPosition__38921, crossAxisPosition__38946) = (this.axis switch { global::Doroti.Framework.Painting.Axis.vertical => (((double, double))((position.dy, position.dx))), global::Doroti.Framework.Painting.Axis.horizontal => (((double, double))((position.dx, position.dy))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var sliverResult__39104 = SliverHitTestResult.CreateWrap(result);
        foreach (RenderSliver child__39181 in this.childrenInHitTestOrder)
        {
            if (!((RenderSliver)child__39181).geometry!.visible)
            {
                continue;
            }
            var transform__39292 = Matrix4.identity();
            applyPaintTransform(child__39181, transform__39292);
            bool isHit__39408 = result.addWithOutOfBandPosition(paintTransform: transform__39292, hitTest: ((Func<BoxHitTestResult, bool>)((result) =>
            {
                return child__39181.hitTest(sliverResult__39104, mainAxisPosition: computeChildMainAxisPosition(child__39181, mainAxisPosition__38921), crossAxisPosition: crossAxisPosition__38946);
                return default;
            })));
            if (isHit__39408)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RevealedOffset getOffsetToReveal(RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        axis = this.axis;
        var leadingScrollOffset__40949 = 0.0;
        var child__41195 = target;
        RenderBox? pivot__41226 = default!;
        var onlySlivers__41241 = (target is RenderSliver);
        while ((!object.Equals(((RenderObject)child__41195).parent, this)))
        {
            RenderObject parent__41405 = ((RenderObject)child__41195).parent!;
            if ((child__41195 is RenderBox))
            {
                RenderBox child__41195__as41439 = (RenderBox)child__41195;
                pivot__41226 = ((RenderBox)child__41195__as41439);
            }
            if ((parent__41405 is RenderSliver))
            {
                RenderSliver parent__41405__as41502 = (RenderSliver)parent__41405;
                leadingScrollOffset__40949 += DartRuntimePrimitives.RequireValue(((RenderSliver)parent__41405__as41502).childScrollOffset(child__41195));
            }
            else
            {
                onlySlivers__41241 = false;
                leadingScrollOffset__40949 = 0.0;
            }
            child__41195 = parent__41405;
        }
        global::Doroti.Ui.Rect rectLocal__41781 = default!;
        double pivotExtent__41874 = default!;
        GrowthDirection growthDirection__41913 = default!;
        if ((pivot__41226 is not null))
        {
            DartRuntimePrimitives.Assert(() => (pivot__41226.parent is not null));
            DartRuntimePrimitives.Assert(() => (!object.Equals(pivot__41226.parent, this)));
            DartRuntimePrimitives.Assert(() => (!object.Equals(pivot__41226, this)));
            DartRuntimePrimitives.Assert(() => (pivot__41226.parent is RenderSliver));
            var pivotParent__42334 = ((RenderSliver?)(object?)pivot__41226.parent!)!;
            growthDirection__41913 = ((RenderSliver)pivotParent__42334).constraints.growthDirection;
            pivotExtent__41874 = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.horizontal => ((RenderBox)pivot__41226).size.width, global::Doroti.Framework.Painting.Axis.vertical => ((RenderBox)pivot__41226).size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            rect ??= ((RenderObject)target).paintBounds;
            rectLocal__41781 = MatrixUtils.transformRect(target.getTransformTo(pivot__41226), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        }
        else
        {
            if (onlySlivers__41241)
            {
                var targetSliver__42841 = ((RenderSliver?)(object?)target)!;
                growthDirection__41913 = ((RenderSliver)targetSliver__42841).constraints.growthDirection;
                pivotExtent__41874 = ((RenderSliver)targetSliver__42841).geometry!.scrollExtent;
                if ((rect is null))
                {
                    switch (DartRuntimePrimitives.RequireValue(axis))
                    {
                        case global::Doroti.Framework.Painting.Axis.horizontal:
                            {
                                rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, ((RenderSliver)targetSliver__42841).geometry!.scrollExtent, ((RenderSliver)targetSliver__42841).constraints.crossAxisExtent);
                                break;
                            }
                        case global::Doroti.Framework.Painting.Axis.vertical:
                            {
                                rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, ((RenderSliver)targetSliver__42841).constraints.crossAxisExtent, ((RenderSliver)targetSliver__42841).geometry!.scrollExtent);
                                break;
                            }
                    }
                }
                rectLocal__41781 = DartRuntimePrimitives.RequireValue(rect);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (rect is not null));
                return new RevealedOffset(offset: ((ViewportOffset)this.offset).pixels, rect: DartRuntimePrimitives.RequireValue(rect));
            }
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child__41195).parent, this)));
        DartRuntimePrimitives.Assert(() => (child__41195 is RenderSliver));
        var sliver__43854 = ((RenderSliver?)(object?)child__41195)!;
        leadingScrollOffset__40949 += (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(this.axisDirection, growthDirection__41913) switch { global::Doroti.Framework.Painting.AxisDirection.up => (pivotExtent__41874 - rectLocal__41781.bottom), global::Doroti.Framework.Painting.AxisDirection.left => (pivotExtent__41874 - rectLocal__41781.right), global::Doroti.Framework.Painting.AxisDirection.right => rectLocal__41781.left, global::Doroti.Framework.Painting.AxisDirection.down => rectLocal__41781.top, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        bool isPinned__44749 = ((((RenderSliver)sliver__43854).geometry!.maxScrollObstructionExtent > 0L) && (leadingScrollOffset__40949 >= 0L));
        leadingScrollOffset__40949 = scrollOffsetOf(sliver__43854, leadingScrollOffset__40949);
        Matrix4 transform__45179 = target.getTransformTo(this);
        global::Doroti.Ui.Rect targetRect__45229 = MatrixUtils.transformRect(transform__45179, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        double extentOfPinnedSlivers__45303 = maxScrollObstructionExtentBefore(sliver__43854);
        switch (((RenderSliver)sliver__43854).constraints.growthDirection)
        {
            case GrowthDirection.forward:
                {
                    if ((isPinned__44749 && (alignment <= 0L)))
                    {
                        return new RevealedOffset(offset: double.PositiveInfinity, rect: targetRect__45229);
                    }
                    leadingScrollOffset__40949 -= extentOfPinnedSlivers__45303;
                    break;
                }
            case GrowthDirection.reverse:
                {
                    if ((isPinned__44749 && (alignment >= 1L)))
                    {
                        return new RevealedOffset(offset: double.NegativeInfinity, rect: targetRect__45229);
                    }
                    leadingScrollOffset__40949 -= (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.vertical => targetRect__45229.height, global::Doroti.Framework.Painting.Axis.horizontal => targetRect__45229.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                    break;
                }
        }
        double mainAxisExtentDifference__46172 = (DartRuntimePrimitives.RequireValue(axis) switch { global::Doroti.Framework.Painting.Axis.horizontal => ((size.width - extentOfPinnedSlivers__45303) - rectLocal__41781.width), global::Doroti.Framework.Painting.Axis.vertical => ((size.height - extentOfPinnedSlivers__45303) - rectLocal__41781.height), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double targetOffset__46398 = (leadingScrollOffset__40949 - (mainAxisExtentDifference__46172 * alignment));
        double offsetDifference__46490 = (((ViewportOffset)this.offset).pixels - targetOffset__46398);
        targetRect__45229 = (this.axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => targetRect__45229.translate(0.0, -offsetDifference__46490), global::Doroti.Framework.Painting.AxisDirection.down => targetRect__45229.translate(0.0, offsetDifference__46490), global::Doroti.Framework.Painting.AxisDirection.left => targetRect__45229.translate(-offsetDifference__46490, 0.0), global::Doroti.Framework.Painting.AxisDirection.right => targetRect__45229.translate(offsetDifference__46490, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return new RevealedOffset(offset: targetOffset__46398, rect: targetRect__45229);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset computeAbsolutePaintOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        DartRuntimePrimitives.Assert(() => (((RenderSliver)child).geometry is not null));
        return (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(this.axisDirection, growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0.0, ((size.height - layoutOffset) - ((RenderSliver)child).geometry!.paintExtent)), global::Doroti.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(((size.width - layoutOffset) - ((RenderSliver)child).geometry!.paintExtent), 0.0), global::Doroti.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(layoutOffset, 0.0), global::Doroti.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0.0, layoutOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("crossAxisDirection", this.crossAxisDirection));
        properties.add(new DiagnosticsProperty<ViewportOffset>("offset", this.offset));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__48439 = new List<DiagnosticsNode>();
        RenderSliver? child__48489 = firstChild;
        if ((child__48489 is null))
        {
            return children__48439;
        }
        long count__48572 = this.indexOfFirstChild;
        while (true)
        {
            children__48439.Add(((Diagnosticable)child__48489!).toDiagnosticsNode(name: labelForChild(count__48572)));
            if ((object.Equals(child__48489, lastChild)))
            {
                break;
            }
            count__48572 += 1L;
            child__48489 = childAfter(child__48489);
        }
        return children__48439;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract bool hasVisualOverflow { get; }
    public abstract void updateOutOfBandData(GrowthDirection growthDirection, SliverGeometry childLayoutGeometry);
    public abstract void updateChildLayoutOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection);
    public abstract global::Doroti.Ui.Offset paintOffsetOf(RenderSliver child);
    public abstract double scrollOffsetOf(RenderSliver child, double scrollOffsetWithinChild);
    public abstract double maxScrollObstructionExtentBefore(RenderSliver child);
    public abstract double computeChildMainAxisPosition(RenderSliver child, double parentMainAxisPosition);
    public abstract long indexOfFirstChild { get; }
    public abstract string labelForChild(long index);
    public virtual IEnumerable<RenderSliver> childrenInPaintOrder
    {
        get
        {
            return (this.paintOrder switch { SliverPaintOrder.firstIsTop => this._childrenLastToFirst, SliverPaintOrder.lastIsTop => this._childrenFirstToLast, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public virtual IEnumerable<RenderSliver> childrenInHitTestOrder
    {
        get
        {
            return (this.paintOrder switch { SliverPaintOrder.firstIsTop => this._childrenFirstToLast, SliverPaintOrder.lastIsTop => this._childrenLastToFirst, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual IEnumerable<RenderSliver> _childrenLastToFirst
    {
        get
        {
            var children__53750 = new List<RenderSliver>();
            RenderSliver? child__53797 = lastChild;
            while ((child__53797 is not null))
            {
                children__53750.Add(child__53797);
                child__53797 = childBefore(child__53797);
            }
            return children__53750;
            return default!;
        }
    }
    internal virtual IEnumerable<RenderSliver> _childrenFirstToLast
    {
        get
        {
            var children__53999 = new List<RenderSliver>();
            RenderSliver? child__54046 = firstChild;
            while ((child__54046 is not null))
            {
                children__53999.Add(child__54046);
                child__54046 = childAfter(child__54046);
            }
            return children__53999;
            return default!;
        }
    }
    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        if (!((ViewportOffset)this.offset).allowImplicitScrolling)
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        global::Doroti.Ui.Rect? newRect__54552 = global::Doroti.Framework.Rendering.RenderViewportBase<ParentDataClass>.showInViewport(descendant: descendant, viewport: this, offset: this.offset, rect: rect, duration: duration, curve: curve);
        base.showOnScreen(rect: newRect__54552, duration: duration, curve: curve);
    }

    public static global::Doroti.Ui.Rect? showInViewport(RenderObject? descendant = null, Rect? rect = null, RenderViewportBase<ParentDataClass> viewport = default!, ViewportOffset offset = default!, Duration duration = default, Curve curve = default!)
    {
        if ((descendant is null))
        {
            return rect;
        }
        RevealedOffset leadingEdgeOffset__56345 = viewport.getOffsetToReveal(descendant, 0.0, rect: rect);
        RevealedOffset trailingEdgeOffset__56472 = viewport.getOffsetToReveal(descendant, 1.0, rect: rect);
        double currentOffset__56592 = ((ViewportOffset)offset).pixels;
        RevealedOffset? targetOffset__56649 = RevealedOffset.clampOffset(leadingEdgeOffset: leadingEdgeOffset__56345, trailingEdgeOffset: trailingEdgeOffset__56472, currentOffset: currentOffset__56592);
        if ((targetOffset__56649 is null))
        {
            DartRuntimePrimitives.Assert(() => (viewport.parent is not null));
            Matrix4 transform__57047 = descendant.getTransformTo(viewport.parent);
            return MatrixUtils.transformRect(transform__57047, (rect ?? ((RenderObject)descendant).paintBounds));
        }
        _ = offset.moveTo(((RevealedOffset)targetOffset__56649).offset, duration: duration, curve: curve);
        return ((RevealedOffset)targetOffset__56649).rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData__173585 = ((ParentDataClass?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((ParentDataClass?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData__173981 = ((ParentDataClass?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((ParentDataClass?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderSliver child, RenderSliver? after = null)
    {
        var childParentData__175971 = ((ParentDataClass?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((ParentDataClass?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData__176766 = ((ParentDataClass?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((ParentDataClass?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((ParentDataClass?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is ParentDataClass));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderSliver child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderSliver>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderSliver child)
    {
        var childParentData__179226 = ((ParentDataClass?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((ParentDataClass?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((ParentDataClass?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderSliver child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderSliver? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((ParentDataClass?)(object?)child__180623.parentData!)!;
            RenderSliver? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__181479 = ((ParentDataClass?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderSliver? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((ParentDataClass?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderSliver? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((ParentDataClass?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderSliver? firstChild => this._firstChild;
    public virtual RenderSliver? lastChild => this._lastChild;
    public virtual RenderSliver? childBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((ParentDataClass?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? childAfter(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((ParentDataClass?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderViewport : RenderViewportBase<SliverPhysicalContainerParentData>
{
    public static global::Doroti.Framework.Semantics.SemanticsTag useTwoPaneSemantics = new global::Doroti.Framework.Semantics.SemanticsTag("RenderViewport.twoPane");
    public static global::Doroti.Framework.Semantics.SemanticsTag excludeFromScrolling = new global::Doroti.Framework.Semantics.SemanticsTag("RenderViewport.excludeFromScrolling");
    internal virtual double _anchor { get; set; } = default!;
    internal virtual RenderSliver? _center { get; set; } = default;
    internal const long _maxLayoutCyclesPerChild = 10L;
    internal virtual double _minScrollExtent { get; set; } = default!;
    internal virtual double _maxScrollExtent { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;

    public RenderViewport(global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection crossAxisDirection = default!, ViewportOffset offset = default!, double anchor = 0.0, List<RenderSliver>? children = null, RenderSliver? center = null, double? cacheExtent = null, CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel, ScrollCacheExtent? scrollCacheExtent = null, SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge) : base(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection, offset: offset, cacheExtent: cacheExtent, cacheExtentStyle: cacheExtentStyle, scrollCacheExtent: scrollCacheExtent, paintOrder: paintOrder, clipBehavior: clipBehavior)
    {
        this._anchor = anchor;
        this._center = center;
        System.Diagnostics.Debug.Assert(((anchor >= 0.0) && (anchor <= 1.0)));
        System.Diagnostics.Debug.Assert(((!object.Equals(cacheExtentStyle, CacheExtentStyle.viewport)) || (cacheExtent is not null)));
    }

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverPhysicalContainerParentData))
        {
            child.parentData = new SliverPhysicalContainerParentData();
        }
    }

    public virtual double anchor
    {
        get => this._anchor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((__value >= 0.0) && (__value <= 1.0)));
            if ((DartRuntimePrimitives.RequireValue(__value) == this._anchor))
            {
                return;
            }
            _anchor = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual RenderSliver? center
    {
        get => this._center;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._center)))
            {
                return;
            }
            _center = __value;
            markNeedsLayout();
        }
    }
    public override bool sizedByParent => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Rendering.DebugLibrary.debugCheckHasBoundedAxis(axis, constraints));
        return ((BoxConstraints)constraints).biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        switch (axis)
        {
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    offset.applyViewportDimension(size.height);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    offset.applyViewportDimension(size.width);
                    break;
                }
        }
        if ((this.center is null))
        {
            DartRuntimePrimitives.Assert(() => (firstChild is null));
            _minScrollExtent = 0.0;
            _maxScrollExtent = 0.0;
            _hasVisualOverflow = false;
            offset.applyContentDimensions(0.0, 0.0);
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this.center!.parent, this)));
        var (mainAxisExtent__64728, crossAxisExtent__64751) = (axis switch { global::Doroti.Framework.Painting.Axis.vertical => (((double, double))((size.height, size.width))), global::Doroti.Framework.Painting.Axis.horizontal => (((double, double))((size.width, size.height))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double centerOffsetAdjustment__64913 = this.center!.centerOffsetAdjustment;
        long maxLayoutCycles__64984 = (_maxLayoutCyclesPerChild * childCount);
        double correction__65053 = default!;
        var count__65073 = 0L;
        do
        {
            correction__65053 = _attemptLayout(mainAxisExtent__64728, crossAxisExtent__64751, (((ViewportOffset)offset).pixels + centerOffsetAdjustment__64913));
            if ((correction__65053 != 0.0))
            {
                offset.correctBy(correction__65053);
            }
            else
            {
                if (offset.applyContentDimensions(Math.Min(0.0, (this._minScrollExtent + (mainAxisExtent__64728 * this.anchor))), Math.Max(0.0, (this._maxScrollExtent - (mainAxisExtent__64728 * ((1.0 - this.anchor)))))))
                {
                    break;
                }
            }
            count__65073 += 1L;
        }
        while ((count__65073 < maxLayoutCycles__64984));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((count__65073 >= maxLayoutCycles__64984))
                {
                    DartRuntimePrimitives.Assert(() => (count__65073 != 1L));
                    throw new FlutterError("A RenderViewport exceeded its maximum number of layout cycles.\n" + "RenderViewport render objects, during layout, can retry if either their " + "slivers or their ViewportOffset decide that the offset should be corrected " + "to take into account information collected during that layout.\n" + $"In the case of this RenderViewport object, however, this happened {count__65073} " + "times and still there was no consensus on the scroll offset. This usually " + "indicates a bug. Specifically, it means that one of the following three " + "problems is being experienced by the RenderViewport object:\n" + " * One of the RenderSliver children or the ViewportOffset have a bug such" + " that they always think that they need to correct the offset regardless.\n" + " * Some combination of the RenderSliver children and the ViewportOffset" + " have a bad interaction such that one applies a correction then another" + " applies a reverse correction, leading to an infinite loop of corrections.\n" + " * There is a pathological case that would eventually resolve, but it is" + " so complicated that it cannot be resolved in any reasonable number of" + " layout passes.");
                }
                return true;
            });
    }

    internal virtual double _attemptLayout(double mainAxisExtent, double crossAxisExtent, double correctedOffset)
    {
        DartRuntimePrimitives.Assert(() => !double.IsNaN(mainAxisExtent));
        DartRuntimePrimitives.Assert(() => (mainAxisExtent >= 0.0));
        DartRuntimePrimitives.Assert(() => double.IsFinite(mainAxisExtent));
        DartRuntimePrimitives.Assert(() => double.IsFinite(crossAxisExtent));
        DartRuntimePrimitives.Assert(() => (crossAxisExtent >= 0.0));
        DartRuntimePrimitives.Assert(() => double.IsFinite(correctedOffset));
        _minScrollExtent = 0.0;
        _maxScrollExtent = 0.0;
        _hasVisualOverflow = false;
        double centerOffset__67665 = ((mainAxisExtent * this.anchor) - correctedOffset);
        double reverseDirectionRemainingPaintExtent__67740 = Dart_uiLibrary.clampDouble(centerOffset__67665, 0.0, mainAxisExtent);
        double forwardDirectionRemainingPaintExtent__67869 = Dart_uiLibrary.clampDouble((mainAxisExtent - centerOffset__67665), 0.0, mainAxisExtent);
        _calculatedCacheExtent = _scrollCacheExtent._calculateCacheOffset(mainAxisExtent);
        double fullCacheExtent__68104 = (mainAxisExtent + (2L * DartRuntimePrimitives.RequireValue(_calculatedCacheExtent)));
        double centerCacheOffset__68185 = (centerOffset__67665 + DartRuntimePrimitives.RequireValue(_calculatedCacheExtent));
        double reverseDirectionRemainingCacheExtent__68262 = Dart_uiLibrary.clampDouble(centerCacheOffset__68185, 0.0, fullCacheExtent__68104);
        double forwardDirectionRemainingCacheExtent__68397 = Dart_uiLibrary.clampDouble((fullCacheExtent__68104 - centerCacheOffset__68185), 0.0, fullCacheExtent__68104);
        RenderSliver? leadingNegativeChild__68558 = childBefore(this.center!);
        if ((leadingNegativeChild__68558 is not null))
        {
            double result__68696 = layoutChildSequence(child: leadingNegativeChild__68558, scrollOffset: (Math.Max(mainAxisExtent, centerOffset__67665) - mainAxisExtent), overlap: 0.0, layoutOffset: forwardDirectionRemainingPaintExtent__67869, remainingPaintExtent: reverseDirectionRemainingPaintExtent__67740, mainAxisExtent: mainAxisExtent, crossAxisExtent: crossAxisExtent, growthDirection: GrowthDirection.reverse, advance: (Func<RenderSliver, RenderSliver?>)childBefore, remainingCacheExtent: reverseDirectionRemainingCacheExtent__68262, cacheOrigin: Dart_uiLibrary.clampDouble((mainAxisExtent - centerOffset__67665), -DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), 0.0));
            if ((result__68696 != 0.0))
            {
                return -result__68696;
            }
        }
        return layoutChildSequence(child: this.center, scrollOffset: Math.Max(0.0, -centerOffset__67665), overlap: ((leadingNegativeChild__68558 is null) ? Math.Min(0.0, -centerOffset__67665) : 0.0), layoutOffset: ((centerOffset__67665 >= mainAxisExtent) ? centerOffset__67665 : reverseDirectionRemainingPaintExtent__67740), remainingPaintExtent: forwardDirectionRemainingPaintExtent__67869, mainAxisExtent: mainAxisExtent, crossAxisExtent: crossAxisExtent, growthDirection: GrowthDirection.forward, advance: (Func<RenderSliver, RenderSliver?>)childAfter, remainingCacheExtent: forwardDirectionRemainingCacheExtent__68397, cacheOrigin: Dart_uiLibrary.clampDouble(centerOffset__67665, -DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), 0.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasVisualOverflow => this._hasVisualOverflow;
    public override void updateOutOfBandData(GrowthDirection growthDirection, SliverGeometry childLayoutGeometry)
    {
        switch (growthDirection)
        {
            case GrowthDirection.forward:
                {
                    _maxScrollExtent += ((SliverGeometry)childLayoutGeometry).scrollExtent;
                    break;
                }
            case GrowthDirection.reverse:
                {
                    _minScrollExtent -= ((SliverGeometry)childLayoutGeometry).scrollExtent;
                    break;
                }
        }
        if (((SliverGeometry)childLayoutGeometry).hasVisualOverflow)
        {
            _hasVisualOverflow = true;
        }
    }

    public override void updateChildLayoutOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection)
    {
        var childParentData__70756 = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        childParentData__70756.paintOffset = computeAbsolutePaintOffset(child, layoutOffset, growthDirection);
    }

    public override Offset paintOffsetOf(RenderSliver child)
    {
        var childParentData__70993 = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        return ((SliverPhysicalParentData)childParentData__70993).paintOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double scrollOffsetOf(RenderSliver child, double scrollOffsetWithinChild)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        GrowthDirection growthDirection__71253 = ((RenderSliver)child).constraints.growthDirection;
        switch (growthDirection__71253)
        {
            case GrowthDirection.forward:
                {
                    var scrollOffsetToChild__71385 = 0.0;
                    RenderSliver? current__71434 = this.center;
                    while ((!object.Equals(current__71434, child)))
                    {
                        scrollOffsetToChild__71385 += current__71434!.geometry!.scrollExtent;
                        current__71434 = childAfter(current__71434);
                    }
                    return (scrollOffsetToChild__71385 + scrollOffsetWithinChild);
                }
            case GrowthDirection.reverse:
                {
                    var scrollOffsetToChild__71714 = 0.0;
                    RenderSliver? current__71763 = childBefore(this.center!);
                    while ((!object.Equals(current__71763, child)))
                    {
                        scrollOffsetToChild__71714 -= current__71763!.geometry!.scrollExtent;
                        current__71763 = childBefore(current__71763);
                    }
                    return (scrollOffsetToChild__71714 - scrollOffsetWithinChild);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxScrollObstructionExtentBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        GrowthDirection growthDirection__72157 = ((RenderSliver)child).constraints.growthDirection;
        switch (growthDirection__72157)
        {
            case GrowthDirection.forward:
                {
                    var pinnedExtent__72289 = 0.0;
                    RenderSliver? current__72331 = this.center;
                    while ((!object.Equals(current__72331, child)))
                    {
                        pinnedExtent__72289 += current__72331!.geometry!.maxScrollObstructionExtent;
                        current__72331 = childAfter(current__72331);
                    }
                    return pinnedExtent__72289;
                }
            case GrowthDirection.reverse:
                {
                    var pinnedExtent__72585 = 0.0;
                    RenderSliver? current__72627 = childBefore(this.center!);
                    while ((!object.Equals(current__72627, child)))
                    {
                        pinnedExtent__72585 += current__72627!.geometry!.maxScrollObstructionExtent;
                        current__72627 = childBefore(current__72627);
                    }
                    return pinnedExtent__72585;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var childParentData__73025 = ((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!;
        childParentData__73025.applyPaintTransform(transform);
    }

    public override double computeChildMainAxisPosition(RenderSliver child, double parentMainAxisPosition)
    {
        global::Doroti.Ui.Offset paintOffset__73267 = (((SliverPhysicalParentData?)(object?)child.parentData!)!).paintOffset;
        return (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((RenderSliver)child).constraints.axisDirection, ((RenderSliver)child).constraints.growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.down => (parentMainAxisPosition - paintOffset__73267.dy), global::Doroti.Framework.Painting.AxisDirection.right => (parentMainAxisPosition - paintOffset__73267.dx), global::Doroti.Framework.Painting.AxisDirection.up => (((RenderSliver)child).geometry!.paintExtent - ((parentMainAxisPosition - paintOffset__73267.dy))), global::Doroti.Framework.Painting.AxisDirection.left => (((RenderSliver)child).geometry!.paintExtent - ((parentMainAxisPosition - paintOffset__73267.dx))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long indexOfFirstChild
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.center is not null));
            DartRuntimePrimitives.Assert(() => (object.Equals(this.center!.parent, this)));
            DartRuntimePrimitives.Assert(() => (firstChild is not null));
            var count__73984 = 0L;
            RenderSliver? child__74013 = this.center;
            while ((!object.Equals(child__74013, firstChild)))
            {
                count__73984 -= 1L;
                child__74013 = childBefore(child__74013!);
            }
            return count__73984;
            return default!;
        }
    }
    public override string labelForChild(long index)
    {
        if ((index == 0L))
        {
            return "center child";
        }
        return $"child {index}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("anchor", this.anchor));
    }

}

public class RenderShrinkWrappingViewport : RenderViewportBase<SliverLogicalContainerParentData>
{
    internal virtual double _maxScrollExtent { get; set; } = default!;
    internal virtual double _shrinkWrapExtent { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;

    public RenderShrinkWrappingViewport(global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection crossAxisDirection = default!, ViewportOffset offset = default!, SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge, ScrollCacheExtent? scrollCacheExtent = null, List<RenderSliver>? children = null) : base(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection, offset: offset, paintOrder: paintOrder, clipBehavior: clipBehavior, scrollCacheExtent: scrollCacheExtent)
    {
    }

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverLogicalContainerParentData))
        {
            child.parentData = new SliverLogicalContainerParentData();
        }
    }

    public override bool debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!RenderObject.debugCheckingIntrinsics)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this.GetType()} does not support returning intrinsic dimensions."), new ErrorDescription("Calculating the intrinsic dimensions would require instantiating every child of " + "the viewport, which defeats the point of viewports being lazy."), new ErrorHint("If you are merely trying to shrink-wrap the viewport in the main axis direction, " + "you should be able to achieve that effect by just giving the viewport loose " + "constraints, without needing to measure its intrinsic dimensions.") });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckHasBoundedCrossAxis()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                switch (axis)
                {
                    case global::Doroti.Framework.Painting.Axis.vertical:
                        {
                            if (!((BoxConstraints)constraints).hasBoundedWidth)
                            {
                                throw new FlutterError("Vertical viewport was given unbounded width.\n" + "Viewports expand in the cross axis to fill their container and " + "constrain their children to match their extent in the cross axis. " + "In this case, a vertical shrinkwrapping viewport was given an " + "unlimited amount of horizontal space in which to expand.");
                            }
                            break;
                        }
                    case global::Doroti.Framework.Painting.Axis.horizontal:
                        {
                            if (!((BoxConstraints)constraints).hasBoundedHeight)
                            {
                                throw new FlutterError("Horizontal viewport was given unbounded height.\n" + "Viewports expand in the cross axis to fill their container and " + "constrain their children to match their extent in the cross axis. " + "In this case, a horizontal shrinkwrapping viewport was given an " + "unlimited amount of vertical space in which to expand.");
                            }
                            break;
                        }
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__78830 = this.constraints;
        if ((firstChild is null))
        {
            DartRuntimePrimitives.Assert(() => _debugCheckHasBoundedCrossAxis());
            size = (axis switch { global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Size(((BoxConstraints)constraints__78830).maxWidth, ((BoxConstraints)constraints__78830).minHeight), global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Size(((BoxConstraints)constraints__78830).minWidth, ((BoxConstraints)constraints__78830).maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            offset.applyViewportDimension(0.0);
            _maxScrollExtent = 0.0;
            _shrinkWrapExtent = 0.0;
            _hasVisualOverflow = false;
            offset.applyContentDimensions(0.0, 0.0);
            return;
        }
        DartRuntimePrimitives.Assert(() => _debugCheckHasBoundedCrossAxis());
        var (mainAxisExtent__79553, crossAxisExtent__79576) = (axis switch { global::Doroti.Framework.Painting.Axis.vertical => (((double, double))((((BoxConstraints)constraints__78830).maxHeight, ((BoxConstraints)constraints__78830).maxWidth))), global::Doroti.Framework.Painting.Axis.horizontal => (((double, double))((((BoxConstraints)constraints__78830).maxWidth, ((BoxConstraints)constraints__78830).maxHeight))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double correction__79772 = default!;
        double effectiveExtent__79795 = default!;
        while (true)
        {
            correction__79772 = _attemptLayout(mainAxisExtent__79553, crossAxisExtent__79576, ((ViewportOffset)offset).pixels);
            if ((correction__79772 != 0.0))
            {
                offset.correctBy(correction__79772);
            }
            else
            {
                effectiveExtent__79795 = (axis switch { global::Doroti.Framework.Painting.Axis.vertical => constraints__78830.constrainHeight(this._shrinkWrapExtent), global::Doroti.Framework.Painting.Axis.horizontal => constraints__78830.constrainWidth(this._shrinkWrapExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                bool didAcceptViewportDimension__80221 = offset.applyViewportDimension(effectiveExtent__79795);
                bool didAcceptContentDimension__80317 = offset.applyContentDimensions(0.0, Math.Max(0.0, (this._maxScrollExtent - effectiveExtent__79795)));
                if ((didAcceptViewportDimension__80221 && didAcceptContentDimension__80317))
                {
                    break;
                }
            }
        }
        size = (axis switch { global::Doroti.Framework.Painting.Axis.vertical => constraints__78830.constrainDimensions(crossAxisExtent__79576, effectiveExtent__79795), global::Doroti.Framework.Painting.Axis.horizontal => constraints__78830.constrainDimensions(effectiveExtent__79795, crossAxisExtent__79576), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    }

    internal virtual double _attemptLayout(double mainAxisExtent, double crossAxisExtent, double correctedOffset)
    {
        DartRuntimePrimitives.Assert(() => !double.IsNaN(mainAxisExtent));
        DartRuntimePrimitives.Assert(() => (mainAxisExtent >= 0.0));
        DartRuntimePrimitives.Assert(() => double.IsFinite(crossAxisExtent));
        DartRuntimePrimitives.Assert(() => (crossAxisExtent >= 0.0));
        DartRuntimePrimitives.Assert(() => double.IsFinite(correctedOffset));
        _maxScrollExtent = 0.0;
        _shrinkWrapExtent = 0.0;
        _hasVisualOverflow = (correctedOffset < 0.0);
        if (double.IsFinite(mainAxisExtent))
        {
            _calculatedCacheExtent = _scrollCacheExtent._calculateCacheOffset(mainAxisExtent);
        }
        else
        {
            _calculatedCacheExtent = 0.0;
        }
        return layoutChildSequence(child: firstChild, scrollOffset: Math.Max(0.0, correctedOffset), overlap: Math.Min(0.0, correctedOffset), layoutOffset: Math.Max(0.0, -correctedOffset), remainingPaintExtent: (mainAxisExtent + Math.Min(0.0, correctedOffset)), mainAxisExtent: mainAxisExtent, crossAxisExtent: crossAxisExtent, growthDirection: GrowthDirection.forward, advance: (Func<RenderSliver, RenderSliver?>)childAfter, remainingCacheExtent: (mainAxisExtent + (2L * DartRuntimePrimitives.RequireValue(_calculatedCacheExtent))), cacheOrigin: -DartRuntimePrimitives.RequireValue(_calculatedCacheExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasVisualOverflow => this._hasVisualOverflow;
    public override void updateOutOfBandData(GrowthDirection growthDirection, SliverGeometry childLayoutGeometry)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(growthDirection, GrowthDirection.forward)));
        _maxScrollExtent += ((SliverGeometry)childLayoutGeometry).scrollExtent;
        if (((SliverGeometry)childLayoutGeometry).hasVisualOverflow)
        {
            _hasVisualOverflow = true;
        }
        _shrinkWrapExtent += ((SliverGeometry)childLayoutGeometry).maxPaintExtent;
    }

    public override void updateChildLayoutOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(growthDirection, GrowthDirection.forward)));
        var childParentData__83180 = ((SliverLogicalParentData?)(object?)child.parentData!)!;
        childParentData__83180.layoutOffset = layoutOffset;
    }

    public override Offset paintOffsetOf(RenderSliver child)
    {
        var childParentData__83365 = ((SliverLogicalParentData?)(object?)child.parentData!)!;
        return computeAbsolutePaintOffset(child, DartRuntimePrimitives.RequireValue(((SliverLogicalParentData)childParentData__83365).layoutOffset), GrowthDirection.forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double scrollOffsetOf(RenderSliver child, double scrollOffsetWithinChild)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderSliver)child).constraints.growthDirection, GrowthDirection.forward)));
        var scrollOffsetToChild__83767 = 0.0;
        RenderSliver? current__83812 = firstChild;
        while ((!object.Equals(current__83812, child)))
        {
            scrollOffsetToChild__83767 += current__83812!.geometry!.scrollExtent;
            current__83812 = childAfter(current__83812);
        }
        return (scrollOffsetToChild__83767 + scrollOffsetWithinChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxScrollObstructionExtentBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderSliver)child).constraints.growthDirection, GrowthDirection.forward)));
        var pinnedExtent__84225 = 0.0;
        RenderSliver? current__84263 = firstChild;
        while ((!object.Equals(current__84263, child)))
        {
            pinnedExtent__84225 += current__84263!.geometry!.maxScrollObstructionExtent;
            current__84263 = childAfter(current__84263);
        }
        return pinnedExtent__84225;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        global::Doroti.Ui.Offset offset__84631 = paintOffsetOf(((RenderSliver?)(object?)child)!);
        transform.translateByDouble(offset__84631.dx, offset__84631.dy, 0, 1);
    }

    public override double computeChildMainAxisPosition(RenderSliver child, double parentMainAxisPosition)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        double layoutOffset__84885 = DartRuntimePrimitives.RequireValue((((SliverLogicalParentData?)(object?)child.parentData!)!).layoutOffset);
        return (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((RenderSliver)child).constraints.axisDirection, ((RenderSliver)child).constraints.growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.down => (parentMainAxisPosition - layoutOffset__84885), global::Doroti.Framework.Painting.AxisDirection.right => (parentMainAxisPosition - layoutOffset__84885), global::Doroti.Framework.Painting.AxisDirection.up => ((size.height - parentMainAxisPosition) - layoutOffset__84885), global::Doroti.Framework.Painting.AxisDirection.left => ((size.width - parentMainAxisPosition) - layoutOffset__84885), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long indexOfFirstChild => 0L;
    public override string labelForChild(long index) => $"child {index}";
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/viewport.dart
using Doroti.Runtime;
using Doroti.Ui;

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

    public virtual double _calculateCacheOffset(double mainAxisExtent) => pixels;
    public virtual CacheExtentStyle style => CacheExtentStyle.pixel;
    public virtual double value => pixels;
    public override string ToString()
    {
        return $"ScrollCacheExtent.pixels({pixels})";
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
        return (__other is _PixelScrollCacheExtent__viewport) && (__other.pixels == pixels);
    }

    public override int GetHashCode() => pixels.GetHashCode();
}

internal class _ViewportScrollCacheExtent__viewport : ScrollCacheExtent
{
    public virtual double value { get; private set; } = default!;

    internal _ViewportScrollCacheExtent__viewport(double value)
    {
        this.value = value;
    }

    public virtual double _calculateCacheOffset(double mainAxisExtent) => value * mainAxisExtent;
    public virtual CacheExtentStyle style => CacheExtentStyle.viewport;
    public override string ToString()
    {
        return $"ScrollCacheExtent.viewport({value})";
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
        return (__other is _ViewportScrollCacheExtent__viewport) && (__other.value == value);
    }

    public override int GetHashCode() => value.GetHashCode();
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
        while (@object is not null)
        {
            if (@object is RenderAbstractViewport)
            {
                RenderAbstractViewport @object__as6686 = (RenderAbstractViewport)@object;
                return @object__as6686;
            }
            @object = @object.parent;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static RenderAbstractViewport of(RenderObject? @object)
    {
        RenderAbstractViewport? viewport = maybeOf(@object);
        DartRuntimePrimitives.Assert(() =>
            {
                if (viewport is null)
                {
                    throw new FlutterError("RenderAbstractViewport.of() was called with a render object that was " + "not a descendant of a RenderAbstractViewport.\n" + "No RenderAbstractViewport render object ancestor could be found starting " + "from the object that was passed to RenderAbstractViewport.of().\n" + "The render object where the viewport search started was:\n" + $"  {@object}");
                }
                return true;
            });
        return viewport!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract RevealedOffset getOffsetToReveal(RenderObject target, double alignment, Rect? rect = null, Axis? axis = null);
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
        bool inverted = leadingEdgeOffset.offset < trailingEdgeOffset.offset;
        RevealedOffset smaller = default!;
        RevealedOffset larger = default!;
        (smaller, larger) = inverted ? (((RevealedOffset, RevealedOffset))(leadingEdgeOffset, trailingEdgeOffset)) : (((RevealedOffset, RevealedOffset))(trailingEdgeOffset, leadingEdgeOffset));
        if (currentOffset > larger.offset)
        {
            return larger;
        }
        else
        {
            if (currentOffset < smaller.offset)
            {
                return smaller;
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
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RevealedOffset")}(offset: {offset}, rect: {rect})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RenderViewportBase<ParentDataClass> : RenderBox, ContainerRenderObjectMixin<RenderSliver, ParentDataClass> where ParentDataClass : ContainerParentDataMixin<RenderSliver>
{
    internal virtual AxisDirection _axisDirection { get; set; } = default!;
    internal virtual AxisDirection _crossAxisDirection { get; set; } = default!;
    internal virtual ViewportOffset _offset { get; set; } = default!;
    internal virtual ScrollCacheExtent _scrollCacheExtent { get; set; } = default!;
    internal virtual double? _calculatedCacheExtent { get; set; } = default;
    internal virtual SliverPaintOrder _paintOrder { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderSliver? _firstChild { get; set; } = default;
    public virtual RenderSliver? _lastChild { get; set; } = default;

    protected RenderViewportBase(AxisDirection axisDirection = AxisDirection.down, AxisDirection crossAxisDirection = default!, ViewportOffset offset = default!, double? cacheExtent = null, CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel, ScrollCacheExtent? scrollCacheExtent = null, SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge)
    {
        _axisDirection = axisDirection;
        _crossAxisDirection = crossAxisDirection;
        _offset = offset;
        _scrollCacheExtent = scrollCacheExtent ?? (cacheExtentStyle switch { CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels(cacheExtent ?? RenderAbstractViewport.defaultCacheExtent), CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(cacheExtent)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        _paintOrder = paintOrder;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(!Equals(Basic_typesLibrary.axisDirectionToAxis(axisDirection), Basic_typesLibrary.axisDirectionToAxis(crossAxisDirection)));
        System.Diagnostics.Debug.Assert((cacheExtent is not null) || Equals(cacheExtentStyle, CacheExtentStyle.pixel));
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.addTagForChildren(RenderViewport.useTwoPaneSemantics);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        childrenInPaintOrder.where(sliver =>
        {
            var geometry = sliver.geometry;
            return geometry is not null && (geometry.visible || geometry.cacheExtent > 0.0 || sliver.ensureSemantics);
        }).forEach(visitor);
    }

    public virtual AxisDirection axisDirection
    {
        get => _axisDirection;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _axisDirection))
            {
                return;
            }
            _axisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual AxisDirection crossAxisDirection
    {
        get => _crossAxisDirection;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _crossAxisDirection))
            {
                return;
            }
            _crossAxisDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual Axis axis => Basic_typesLibrary.axisDirectionToAxis(axisDirection);
    public virtual ViewportOffset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (Equals(__value, _offset))
            {
                return;
            }
            if (attached)
            {
                _offset.removeListener(markNeedsLayout);
            }
            _offset = __value;
            if (attached)
            {
                _offset.addListener(markNeedsLayout);
            }
            markNeedsLayout();
        }
    }
    public virtual double? cacheExtent
    {
        get => _scrollCacheExtent.value;
        set
        {
            var __value = value;
            if (__value == cacheExtent)
            {
                return;
            }
            if (__value is null)
            {
                _scrollCacheExtent = ScrollCacheExtent.CreatePixels(RenderAbstractViewport.defaultCacheExtent);
            }
            else
            {
                _scrollCacheExtent = _scrollCacheExtent switch { _PixelScrollCacheExtent__viewport __object21598 => ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(__value)), _ViewportScrollCacheExtent__viewport __object21668 => ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(__value)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            }
            markNeedsLayout();
        }
    }
    [System.Diagnostics.CodeAnalysis.AllowNull]
    public virtual ScrollCacheExtent scrollCacheExtent
    {
        get => _scrollCacheExtent;
        set
        {
            var __value = value;
            ScrollCacheExtent effectiveValue = __value ?? ScrollCacheExtent.CreatePixels(RenderAbstractViewport.defaultCacheExtent);
            if (Equals(effectiveValue, _scrollCacheExtent))
            {
                return;
            }
            _scrollCacheExtent = effectiveValue;
            markNeedsLayout();
        }
    }
    public virtual CacheExtentStyle cacheExtentStyle
    {
        get => _scrollCacheExtent.style;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), cacheExtentStyle))
            {
                return;
            }
            _scrollCacheExtent = DartRuntimePrimitives.RequireValue(__value) switch { CacheExtentStyle.pixel => ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(cacheExtent)), CacheExtentStyle.viewport => ScrollCacheExtent.CreateViewport(DartRuntimePrimitives.RequireValue(cacheExtent)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            markNeedsLayout();
        }
    }
    public virtual SliverPaintOrder paintOrder
    {
        get => _paintOrder;
        set
        {
            var __value = value;
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _paintOrder))
            {
                _paintOrder = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _clipBehavior))
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
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        _offset.addListener(markNeedsLayout);
    }

    public override void detach()
    {
        _offset.removeListener(markNeedsLayout);
        base.detach();
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual bool debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!debugCheckingIntrinsics)
                {
                    DartRuntimePrimitives.Assert(() => this is not RenderShrinkWrappingViewport);
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} does not support returning intrinsic dimensions."), new ErrorDescription("Calculating the intrinsic dimensions would require instantiating every child of " + "the viewport, which defeats the point of viewports being lazy."), new ErrorHint("If you are merely trying to shrink-wrap the viewport in the main axis direction, " + "consider a RenderShrinkWrappingViewport render object (ShrinkWrappingViewport widget), " + "which achieves that effect without implementing the intrinsic dimension API.") });
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
        DartRuntimePrimitives.Assert(() => scrollOffset >= 0.0);
        var initialLayoutOffset = layoutOffset;
        ScrollDirection adjustedUserScrollDirection = SliverLibrary.applyGrowthDirectionToScrollDirection(offset.userScrollDirection, growthDirection);
        double maxPaintOffset = layoutOffset + overlap;
        var precedingScrollExtentLocal = 0.0;
        while (child is not null)
        {
            var sliverScrollOffset = (scrollOffset <= 0.0) ? 0.0 : scrollOffset;
            double correctedCacheOrigin = Math.Max(cacheOrigin, -sliverScrollOffset);
            double cacheExtentCorrection = cacheOrigin - correctedCacheOrigin;
            DartRuntimePrimitives.Assert(() => sliverScrollOffset >= correctedCacheOrigin.abs());
            DartRuntimePrimitives.Assert(() => correctedCacheOrigin <= 0.0);
            DartRuntimePrimitives.Assert(() => sliverScrollOffset >= 0.0);
            DartRuntimePrimitives.Assert(() => cacheExtentCorrection <= 0.0);
            child.layout(new SliverConstraints(axisDirection: axisDirection, growthDirection: growthDirection, userScrollDirection: adjustedUserScrollDirection, scrollOffset: sliverScrollOffset, precedingScrollExtent: precedingScrollExtentLocal, overlap: maxPaintOffset - layoutOffset, remainingPaintExtent: Math.Max(0.0, remainingPaintExtent - layoutOffset + initialLayoutOffset), crossAxisExtent: crossAxisExtent, crossAxisDirection: crossAxisDirection, viewportMainAxisExtent: mainAxisExtent, remainingCacheExtent: Math.Max(0.0, remainingCacheExtent + cacheExtentCorrection), cacheOrigin: correctedCacheOrigin), parentUsesSize: true);
            SliverGeometry childLayoutGeometry = child.geometry!;
            DartRuntimePrimitives.Assert(() => childLayoutGeometry.debugAssertIsValid());
            if (childLayoutGeometry.scrollOffsetCorrection is not null)
            {
                return DartRuntimePrimitives.RequireValue(childLayoutGeometry.scrollOffsetCorrection);
            }
            double effectiveLayoutOffset = layoutOffset + childLayoutGeometry.paintOrigin;
            if (childLayoutGeometry.visible || (scrollOffset > 0L))
            {
                updateChildLayoutOffset(child, effectiveLayoutOffset, growthDirection);
            }
            else
            {
                updateChildLayoutOffset(child, -scrollOffset + initialLayoutOffset, growthDirection);
            }
            maxPaintOffset = Math.Max(effectiveLayoutOffset + childLayoutGeometry.paintExtent, maxPaintOffset);
            scrollOffset -= childLayoutGeometry.scrollExtent;
            precedingScrollExtentLocal += childLayoutGeometry.scrollExtent;
            layoutOffset += childLayoutGeometry.layoutExtent;
            if (childLayoutGeometry.cacheExtent != 0.0)
            {
                remainingCacheExtent -= childLayoutGeometry.cacheExtent - cacheExtentCorrection;
                cacheOrigin = Math.Min(correctedCacheOrigin + childLayoutGeometry.cacheExtent, 0.0);
            }
            updateOutOfBandData(growthDirection, childLayoutGeometry);
            child = advance(child);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        if (__child.ensureSemantics && !(__child.geometry!.visible || (__child.geometry!.cacheExtent > 0.0)))
        {
            return null;
        }
        switch (clipBehavior)
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
        Rect viewportClip = Offset.zero & size;
        if ((__child.constraints.overlap == 0L) || !double.IsFinite(__child.constraints.viewportMainAxisExtent))
        {
            return viewportClip;
        }
        double leftLocal = viewportClip.left;
        double rightLocal = viewportClip.right;
        double topLocal = viewportClip.top;
        double bottomLocal = viewportClip.bottom;
        double startOfOverlap = __child.constraints.viewportMainAxisExtent - __child.constraints.remainingPaintExtent;
        double overlapCorrection = startOfOverlap + __child.constraints.overlap;
        switch (SliverLibrary.applyGrowthDirectionToAxisDirection(axisDirection, __child.constraints.growthDirection))
        {
            case AxisDirection.down:
                {
                    topLocal += overlapCorrection;
                    break;
                }
            case AxisDirection.up:
                {
                    bottomLocal -= overlapCorrection;
                    break;
                }
            case AxisDirection.right:
                {
                    leftLocal += overlapCorrection;
                    break;
                }
            case AxisDirection.left:
                {
                    rightLocal -= overlapCorrection;
                    break;
                }
        }
        return Rect.fromLTRB(leftLocal, topLocal, rightLocal, bottomLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect? describeSemanticsClip(RenderObject? child)
    {
        var __child = child is null ? null : (RenderSliver)(object)child;
        if ((__child is not null) && __child.ensureSemantics && !(__child.geometry!.visible || (__child.geometry!.cacheExtent > 0.0)))
        {
            return null;
        }
        if (_calculatedCacheExtent is null)
        {
            return semanticBounds;
        }
        switch (axis)
        {
            case Axis.vertical:
                {
                    return Rect.fromLTRB(semanticBounds.left, semanticBounds.top - DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), semanticBounds.right, semanticBounds.bottom + DartRuntimePrimitives.RequireValue(_calculatedCacheExtent));
                }
            case Axis.horizontal:
                {
                    return Rect.fromLTRB(semanticBounds.left - DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), semanticBounds.top, semanticBounds.right + DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), semanticBounds.bottom);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (firstChild is null)
        {
            return;
        }
        if (hasVisualOverflow && (!Equals(clipBehavior, Clip.none)))
        {
            _clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, Offset.zero & size, _paintContents, clipBehavior: clipBehavior, oldLayer: _clipRectLayer.layer);
        }
        else
        {
            _clipRectLayer.layer = null;
            _paintContents(context, offset);
        }
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        base.dispose();
    }

    internal virtual void _paintContents(PaintingContext context, Offset offset)
    {
        foreach (RenderSliver child in childrenInPaintOrder)
        {
            if (child.geometry!.visible)
            {
                context.paintChild(child, offset + paintOffsetOf(child));
            }
        }
    }

    public override void debugPaintSize(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                base.debugPaintSize(context, offset);
                var paint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new Color(4278255360L);
    return __cascade;
}))();
                Canvas canvasLocal = context.canvas;
                RenderSliver? child = firstChild;
                while (child is not null)
                {
                    Size size = axis switch { Axis.vertical => new Size(child.constraints.crossAxisExtent, child.geometry!.layoutExtent), Axis.horizontal => new Size(child.geometry!.layoutExtent, child.constraints.crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                    canvasLocal.drawRect((offset + paintOffsetOf(child) & size).deflate(0.5), paint);
                    child = childAfter(child);
                }
                return true;
            });
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        var (mainAxisPositionLocal, crossAxisPositionLocal) = axis switch { Axis.vertical => (position.dy, position.dx), Axis.horizontal => (position.dx, position.dy), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var sliverResult = SliverHitTestResult.CreateWrap(result);
        foreach (RenderSliver child in childrenInHitTestOrder)
        {
            if (!child.geometry!.visible)
            {
                continue;
            }
            var transform = Matrix4.identity();
            applyPaintTransform(child, transform);
            bool isHit = result.addWithOutOfBandPosition(paintTransform: transform, hitTest: (result) =>
            {
                return child.hitTest(sliverResult, mainAxisPosition: computeChildMainAxisPosition(child, mainAxisPositionLocal), crossAxisPosition: crossAxisPositionLocal);
            });
            if (isHit)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RevealedOffset getOffsetToReveal(RenderObject target, double alignment, Rect? rect = null, Axis? axis = null)
    {
        axis = this.axis;
        var leadingScrollOffset = 0.0;
        var child = target;
        RenderBox? pivot = default!;
        var onlySlivers = target is RenderSliver;
        while (!Equals(child.parent, this))
        {
            RenderObject parentLocal = child.parent!;
            if (child is RenderBox)
            {
                RenderBox child__41195__as41439 = (RenderBox)child;
                pivot = child__41195__as41439;
            }
            if (parentLocal is RenderSliver)
            {
                RenderSliver parent__41405__as41502 = (RenderSliver)parentLocal;
                leadingScrollOffset += DartRuntimePrimitives.RequireValue(parent__41405__as41502.childScrollOffset(child));
            }
            else
            {
                onlySlivers = false;
                leadingScrollOffset = 0.0;
            }
            child = parentLocal;
        }
        Rect rectLocal = default!;
        double pivotExtent = default!;
        GrowthDirection growthDirectionLocal = default!;
        if (pivot is not null)
        {
            DartRuntimePrimitives.Assert(() => pivot.parent is not null);
            DartRuntimePrimitives.Assert(() => !Equals(pivot.parent, this));
            DartRuntimePrimitives.Assert(() => !Equals(pivot, this));
            DartRuntimePrimitives.Assert(() => pivot.parent is RenderSliver);
            var pivotParent = ((RenderSliver?)(object?)pivot.parent!)!;
            growthDirectionLocal = pivotParent.constraints.growthDirection;
            pivotExtent = DartRuntimePrimitives.RequireValue(axis) switch { Axis.horizontal => pivot.size.width, Axis.vertical => pivot.size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            rect ??= target.paintBounds;
            rectLocal = MatrixUtils.transformRect(target.getTransformTo(pivot), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        }
        else
        {
            if (onlySlivers)
            {
                var targetSliver = ((RenderSliver?)(object?)target)!;
                growthDirectionLocal = targetSliver.constraints.growthDirection;
                pivotExtent = targetSliver.geometry!.scrollExtent;
                if (rect is null)
                {
                    switch (DartRuntimePrimitives.RequireValue(axis))
                    {
                        case Axis.horizontal:
                            {
                                rect = Rect.fromLTWH(0, 0, targetSliver.geometry!.scrollExtent, targetSliver.constraints.crossAxisExtent);
                                break;
                            }
                        case Axis.vertical:
                            {
                                rect = Rect.fromLTWH(0, 0, targetSliver.constraints.crossAxisExtent, targetSliver.geometry!.scrollExtent);
                                break;
                            }
                    }
                }
                rectLocal = DartRuntimePrimitives.RequireValue(rect);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => rect is not null);
                return new RevealedOffset(offset: offset.pixels, rect: DartRuntimePrimitives.RequireValue(rect));
            }
        }
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        DartRuntimePrimitives.Assert(() => child is RenderSliver);
        var sliver = ((RenderSliver?)(object?)child)!;
        leadingScrollOffset += SliverLibrary.applyGrowthDirectionToAxisDirection(axisDirection, growthDirectionLocal) switch { AxisDirection.up => pivotExtent - rectLocal.bottom, AxisDirection.left => pivotExtent - rectLocal.right, AxisDirection.right => rectLocal.left, AxisDirection.down => rectLocal.top, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        bool isPinned = (sliver.geometry!.maxScrollObstructionExtent > 0L) && (leadingScrollOffset >= 0L);
        leadingScrollOffset = scrollOffsetOf(sliver, leadingScrollOffset);
        Matrix4 transform = target.getTransformTo(this);
        Rect targetRect = MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        double extentOfPinnedSlivers = maxScrollObstructionExtentBefore(sliver);
        switch (sliver.constraints.growthDirection)
        {
            case GrowthDirection.forward:
                {
                    if (isPinned && (alignment <= 0L))
                    {
                        return new RevealedOffset(offset: double.PositiveInfinity, rect: targetRect);
                    }
                    leadingScrollOffset -= extentOfPinnedSlivers;
                    break;
                }
            case GrowthDirection.reverse:
                {
                    if (isPinned && (alignment >= 1L))
                    {
                        return new RevealedOffset(offset: double.NegativeInfinity, rect: targetRect);
                    }
                    leadingScrollOffset -= DartRuntimePrimitives.RequireValue(axis) switch { Axis.vertical => targetRect.height, Axis.horizontal => targetRect.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                    break;
                }
        }
        double mainAxisExtentDifference = DartRuntimePrimitives.RequireValue(axis) switch { Axis.horizontal => size.width - extentOfPinnedSlivers - rectLocal.width, Axis.vertical => size.height - extentOfPinnedSlivers - rectLocal.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double targetOffset = leadingScrollOffset - (mainAxisExtentDifference * alignment);
        double offsetDifference = offset.pixels - targetOffset;
        targetRect = axisDirection switch { AxisDirection.up => targetRect.translate(0.0, -offsetDifference), AxisDirection.down => targetRect.translate(0.0, offsetDifference), AxisDirection.left => targetRect.translate(-offsetDifference, 0.0), AxisDirection.right => targetRect.translate(offsetDifference, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new RevealedOffset(offset: targetOffset, rect: targetRect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Offset computeAbsolutePaintOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        DartRuntimePrimitives.Assert(() => child.geometry is not null);
        return SliverLibrary.applyGrowthDirectionToAxisDirection(axisDirection, growthDirection) switch { AxisDirection.up => new Offset(0.0, size.height - layoutOffset - child.geometry!.paintExtent), AxisDirection.left => new Offset(size.width - layoutOffset - child.geometry!.paintExtent, 0.0), AxisDirection.right => new Offset(layoutOffset, 0.0), AxisDirection.down => new Offset(0.0, layoutOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<AxisDirection>("axisDirection", axisDirection));
        properties.add(new EnumProperty<AxisDirection>("crossAxisDirection", crossAxisDirection));
        properties.add(new DiagnosticsProperty<ViewportOffset>("offset", offset));
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        RenderSliver? child = firstChild;
        if (child is null)
        {
            return children;
        }
        long count = indexOfFirstChild;
        while (true)
        {
            children.Add(((Diagnosticable)child!).toDiagnosticsNode(name: labelForChild(count)));
            if (Equals(child, lastChild))
            {
                break;
            }
            count += 1L;
            child = childAfter(child);
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract bool hasVisualOverflow { get; }
    public abstract void updateOutOfBandData(GrowthDirection growthDirection, SliverGeometry childLayoutGeometry);
    public abstract void updateChildLayoutOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection);
    public abstract Offset paintOffsetOf(RenderSliver child);
    public abstract double scrollOffsetOf(RenderSliver child, double scrollOffsetWithinChild);
    public abstract double maxScrollObstructionExtentBefore(RenderSliver child);
    public abstract double computeChildMainAxisPosition(RenderSliver child, double parentMainAxisPosition);
    public abstract long indexOfFirstChild { get; }
    public abstract string labelForChild(long index);
    public virtual IEnumerable<RenderSliver> childrenInPaintOrder
    {
        get
        {
            return paintOrder switch { SliverPaintOrder.firstIsTop => _childrenLastToFirst, SliverPaintOrder.lastIsTop => _childrenFirstToLast, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public virtual IEnumerable<RenderSliver> childrenInHitTestOrder
    {
        get
        {
            return paintOrder switch { SliverPaintOrder.firstIsTop => _childrenFirstToLast, SliverPaintOrder.lastIsTop => _childrenLastToFirst, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    internal virtual IEnumerable<RenderSliver> _childrenLastToFirst
    {
        get
        {
            var children = new List<RenderSliver>();
            RenderSliver? child = lastChild;
            while (child is not null)
            {
                children.Add(child);
                child = childBefore(child);
            }
            return children;
        }
    }
    internal virtual IEnumerable<RenderSliver> _childrenFirstToLast
    {
        get
        {
            var children = new List<RenderSliver>();
            RenderSliver? child = firstChild;
            while (child is not null)
            {
                children.Add(child);
                child = childAfter(child);
            }
            return children;
        }
    }
    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        if (!offset.allowImplicitScrolling)
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        Rect? newRect = RenderViewportBase<ParentDataClass>.showInViewport(descendant: descendant, viewport: this, offset: offset, rect: rect, duration: duration, curve: curve);
        base.showOnScreen(rect: newRect, duration: duration, curve: curve);
    }

    public static Rect? showInViewport(RenderObject? descendant = null, Rect? rect = null, RenderViewportBase<ParentDataClass> viewport = default!, ViewportOffset offset = default!, Duration duration = default, Curve curve = default!)
    {
        if (descendant is null)
        {
            return rect;
        }
        RevealedOffset leadingEdgeOffsetLocal = viewport.getOffsetToReveal(descendant, 0.0, rect: rect);
        RevealedOffset trailingEdgeOffsetLocal = viewport.getOffsetToReveal(descendant, 1.0, rect: rect);
        double currentOffsetLocal = offset.pixels;
        RevealedOffset? targetOffset = RevealedOffset.clampOffset(leadingEdgeOffset: leadingEdgeOffsetLocal, trailingEdgeOffset: trailingEdgeOffsetLocal, currentOffset: currentOffsetLocal);
        if (targetOffset is null)
        {
            DartRuntimePrimitives.Assert(() => viewport.parent is not null);
            Matrix4 transform = descendant.getTransformTo(viewport.parent);
            return MatrixUtils.transformRect(transform, rect ?? descendant.paintBounds);
        }
        _ = offset.moveTo(targetOffset.offset, duration: duration, curve: curve);
        return targetOffset.rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderSliver)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderSliver child, RenderSliver? after = null)
    {
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((ParentDataClass?)(object?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((ParentDataClass?)(object?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((ParentDataClass?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ParentDataClass?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is ParentDataClass);
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderSliver child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderSliver>? children)
    {
        children?.forEach(add);
    }

    public virtual void _removeFromChildList(RenderSliver child)
    {
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((ParentDataClass?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ParentDataClass?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderSliver child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
            RenderSliver? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderSliver? firstChild => _firstChild;
    public virtual RenderSliver? lastChild => _lastChild;
    public virtual RenderSliver? childBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? childAfter(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ParentDataClass?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RenderViewport : RenderViewportBase<SliverPhysicalContainerParentData>
{
    public static SemanticsTag useTwoPaneSemantics = new SemanticsTag("RenderViewport.twoPane");
    public static SemanticsTag excludeFromScrolling = new SemanticsTag("RenderViewport.excludeFromScrolling");
    internal virtual double _anchor { get; set; } = default!;
    internal virtual RenderSliver? _center { get; set; } = default;
    internal const long _maxLayoutCyclesPerChild = 10L;
    internal virtual double _minScrollExtent { get; set; } = default!;
    internal virtual double _maxScrollExtent { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;

    public RenderViewport(AxisDirection axisDirection = AxisDirection.down, AxisDirection crossAxisDirection = default!, ViewportOffset offset = default!, double anchor = 0.0, List<RenderSliver>? children = null, RenderSliver? center = null, double? cacheExtent = null, CacheExtentStyle cacheExtentStyle = CacheExtentStyle.pixel, ScrollCacheExtent? scrollCacheExtent = null, SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge) : base(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection, offset: offset, cacheExtent: cacheExtent, cacheExtentStyle: cacheExtentStyle, scrollCacheExtent: scrollCacheExtent, paintOrder: paintOrder, clipBehavior: clipBehavior)
    {
        _anchor = anchor;
        _center = center;
        System.Diagnostics.Debug.Assert((anchor >= 0.0) && (anchor <= 1.0));
        System.Diagnostics.Debug.Assert((!Equals(cacheExtentStyle, CacheExtentStyle.viewport)) || (cacheExtent is not null));
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverPhysicalContainerParentData)
        {
            child.parentData = new SliverPhysicalContainerParentData();
        }
    }

    public virtual double anchor
    {
        get => _anchor;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0) && (__value <= 1.0));
            if (DartRuntimePrimitives.RequireValue(__value) == _anchor)
            {
                return;
            }
            _anchor = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual RenderSliver? center
    {
        get => _center;
        set
        {
            var __value = value;
            if (Equals(__value, _center))
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
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasBoundedAxis(axis, constraints));
        return constraints.biggest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        switch (axis)
        {
            case Axis.vertical:
                {
                    offset.applyViewportDimension(size.height);
                    break;
                }
            case Axis.horizontal:
                {
                    offset.applyViewportDimension(size.width);
                    break;
                }
        }
        if (center is null)
        {
            DartRuntimePrimitives.Assert(() => firstChild is null);
            _minScrollExtent = 0.0;
            _maxScrollExtent = 0.0;
            _hasVisualOverflow = false;
            offset.applyContentDimensions(0.0, 0.0);
            return;
        }
        DartRuntimePrimitives.Assert(() => Equals(center!.parent, this));
        var (mainAxisExtent, crossAxisExtent) = axis switch { Axis.vertical => (size.height, size.width), Axis.horizontal => (size.width, size.height), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double centerOffsetAdjustmentLocal = center!.centerOffsetAdjustment;
        long maxLayoutCycles = _maxLayoutCyclesPerChild * childCount;
        double correction = default!;
        var count = 0L;
        do
        {
            correction = _attemptLayout(mainAxisExtent, crossAxisExtent, offset.pixels + centerOffsetAdjustmentLocal);
            if (correction != 0.0)
            {
                offset.correctBy(correction);
            }
            else
            {
                if (offset.applyContentDimensions(Math.Min(0.0, _minScrollExtent + (mainAxisExtent * anchor)), Math.Max(0.0, _maxScrollExtent - (mainAxisExtent * (1.0 - anchor)))))
                {
                    break;
                }
            }
            count += 1L;
        }
        while (count < maxLayoutCycles);
        DartRuntimePrimitives.Assert(() =>
            {
                if (count >= maxLayoutCycles)
                {
                    DartRuntimePrimitives.Assert(() => count != 1L);
                    throw new FlutterError("A RenderViewport exceeded its maximum number of layout cycles.\n" + "RenderViewport render objects, during layout, can retry if either their " + "slivers or their ViewportOffset decide that the offset should be corrected " + "to take into account information collected during that layout.\n" + $"In the case of this RenderViewport object, however, this happened {count} " + "times and still there was no consensus on the scroll offset. This usually " + "indicates a bug. Specifically, it means that one of the following three " + "problems is being experienced by the RenderViewport object:\n" + " * One of the RenderSliver children or the ViewportOffset have a bug such" + " that they always think that they need to correct the offset regardless.\n" + " * Some combination of the RenderSliver children and the ViewportOffset" + " have a bad interaction such that one applies a correction then another" + " applies a reverse correction, leading to an infinite loop of corrections.\n" + " * There is a pathological case that would eventually resolve, but it is" + " so complicated that it cannot be resolved in any reasonable number of" + " layout passes.");
                }
                return true;
            });
    }

    internal virtual double _attemptLayout(double mainAxisExtent, double crossAxisExtent, double correctedOffset)
    {
        DartRuntimePrimitives.Assert(() => !double.IsNaN(mainAxisExtent));
        DartRuntimePrimitives.Assert(() => mainAxisExtent >= 0.0);
        DartRuntimePrimitives.Assert(() => double.IsFinite(mainAxisExtent));
        DartRuntimePrimitives.Assert(() => double.IsFinite(crossAxisExtent));
        DartRuntimePrimitives.Assert(() => crossAxisExtent >= 0.0);
        DartRuntimePrimitives.Assert(() => double.IsFinite(correctedOffset));
        _minScrollExtent = 0.0;
        _maxScrollExtent = 0.0;
        _hasVisualOverflow = false;
        double centerOffset = (mainAxisExtent * anchor) - correctedOffset;
        double reverseDirectionRemainingPaintExtent = Dart_uiLibrary.clampDouble(centerOffset, 0.0, mainAxisExtent);
        double forwardDirectionRemainingPaintExtent = Dart_uiLibrary.clampDouble(mainAxisExtent - centerOffset, 0.0, mainAxisExtent);
        _calculatedCacheExtent = _scrollCacheExtent._calculateCacheOffset(mainAxisExtent);
        double fullCacheExtent = mainAxisExtent + (2L * DartRuntimePrimitives.RequireValue(_calculatedCacheExtent));
        double centerCacheOffset = centerOffset + DartRuntimePrimitives.RequireValue(_calculatedCacheExtent);
        double reverseDirectionRemainingCacheExtent = Dart_uiLibrary.clampDouble(centerCacheOffset, 0.0, fullCacheExtent);
        double forwardDirectionRemainingCacheExtent = Dart_uiLibrary.clampDouble(fullCacheExtent - centerCacheOffset, 0.0, fullCacheExtent);
        RenderSliver? leadingNegativeChild = childBefore(center!);
        if (leadingNegativeChild is not null)
        {
            double result = layoutChildSequence(child: leadingNegativeChild, scrollOffset: Math.Max(mainAxisExtent, centerOffset) - mainAxisExtent, overlap: 0.0, layoutOffset: forwardDirectionRemainingPaintExtent, remainingPaintExtent: reverseDirectionRemainingPaintExtent, mainAxisExtent: mainAxisExtent, crossAxisExtent: crossAxisExtent, growthDirection: GrowthDirection.reverse, advance: childBefore, remainingCacheExtent: reverseDirectionRemainingCacheExtent, cacheOrigin: Dart_uiLibrary.clampDouble(mainAxisExtent - centerOffset, -DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), 0.0));
            if (result != 0.0)
            {
                return -result;
            }
        }
        return layoutChildSequence(child: center, scrollOffset: Math.Max(0.0, -centerOffset), overlap: (leadingNegativeChild is null) ? Math.Min(0.0, -centerOffset) : 0.0, layoutOffset: (centerOffset >= mainAxisExtent) ? centerOffset : reverseDirectionRemainingPaintExtent, remainingPaintExtent: forwardDirectionRemainingPaintExtent, mainAxisExtent: mainAxisExtent, crossAxisExtent: crossAxisExtent, growthDirection: GrowthDirection.forward, advance: childAfter, remainingCacheExtent: forwardDirectionRemainingCacheExtent, cacheOrigin: Dart_uiLibrary.clampDouble(centerOffset, -DartRuntimePrimitives.RequireValue(_calculatedCacheExtent), 0.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasVisualOverflow => _hasVisualOverflow;
    public override void updateOutOfBandData(GrowthDirection growthDirection, SliverGeometry childLayoutGeometry)
    {
        switch (growthDirection)
        {
            case GrowthDirection.forward:
                {
                    _maxScrollExtent += childLayoutGeometry.scrollExtent;
                    break;
                }
            case GrowthDirection.reverse:
                {
                    _minScrollExtent -= childLayoutGeometry.scrollExtent;
                    break;
                }
        }
        if (childLayoutGeometry.hasVisualOverflow)
        {
            _hasVisualOverflow = true;
        }
    }

    public override void updateChildLayoutOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection)
    {
        var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        childParentData.paintOffset = computeAbsolutePaintOffset(child, layoutOffset, growthDirection);
    }

    public override Offset paintOffsetOf(RenderSliver child)
    {
        var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        return childParentData.paintOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double scrollOffsetOf(RenderSliver child, double scrollOffsetWithinChild)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        GrowthDirection growthDirectionLocal = child.constraints.growthDirection;
        switch (growthDirectionLocal)
        {
            case GrowthDirection.forward:
                {
                    var scrollOffsetToChild = 0.0;
                    RenderSliver? current = center;
                    while (!Equals(current, child))
                    {
                        scrollOffsetToChild += current!.geometry!.scrollExtent;
                        current = childAfter(current);
                    }
                    return scrollOffsetToChild + scrollOffsetWithinChild;
                }
            case GrowthDirection.reverse:
                {
                    var scrollOffsetToChildLocal = 0.0;
                    RenderSliver? currentLocal = childBefore(center!);
                    while (!Equals(currentLocal, child))
                    {
                        scrollOffsetToChildLocal -= currentLocal!.geometry!.scrollExtent;
                        currentLocal = childBefore(currentLocal);
                    }
                    return scrollOffsetToChildLocal - scrollOffsetWithinChild;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxScrollObstructionExtentBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        GrowthDirection growthDirectionLocal = child.constraints.growthDirection;
        switch (growthDirectionLocal)
        {
            case GrowthDirection.forward:
                {
                    var pinnedExtent = 0.0;
                    RenderSliver? current = center;
                    while (!Equals(current, child))
                    {
                        pinnedExtent += current!.geometry!.maxScrollObstructionExtent;
                        current = childAfter(current);
                    }
                    return pinnedExtent;
                }
            case GrowthDirection.reverse:
                {
                    var pinnedExtentLocal = 0.0;
                    RenderSliver? currentLocal = childBefore(center!);
                    while (!Equals(currentLocal, child))
                    {
                        pinnedExtentLocal += currentLocal!.geometry!.maxScrollObstructionExtent;
                        currentLocal = childBefore(currentLocal);
                    }
                    return pinnedExtentLocal;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override double computeChildMainAxisPosition(RenderSliver child, double parentMainAxisPosition)
    {
        Offset paintOffsetLocal = ((SliverPhysicalParentData?)(object?)child.parentData!)!.paintOffset;
        return SliverLibrary.applyGrowthDirectionToAxisDirection(child.constraints.axisDirection, child.constraints.growthDirection) switch { AxisDirection.down => parentMainAxisPosition - paintOffsetLocal.dy, AxisDirection.right => parentMainAxisPosition - paintOffsetLocal.dx, AxisDirection.up => child.geometry!.paintExtent - (parentMainAxisPosition - paintOffsetLocal.dy), AxisDirection.left => child.geometry!.paintExtent - (parentMainAxisPosition - paintOffsetLocal.dx), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long indexOfFirstChild
    {
        get
        {
            DartRuntimePrimitives.Assert(() => center is not null);
            DartRuntimePrimitives.Assert(() => Equals(center!.parent, this));
            DartRuntimePrimitives.Assert(() => firstChild is not null);
            var count = 0L;
            RenderSliver? child = center;
            while (!Equals(child, firstChild))
            {
                count -= 1L;
                child = childBefore(child!);
            }
            return count;
        }
    }
    public override string labelForChild(long index)
    {
        if (index == 0L)
        {
            return "center child";
        }
        return $"child {index}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("anchor", anchor));
    }

}

public class RenderShrinkWrappingViewport : RenderViewportBase<SliverLogicalContainerParentData>
{
    internal virtual double _maxScrollExtent { get; set; } = default!;
    internal virtual double _shrinkWrapExtent { get; set; } = default!;
    internal virtual bool _hasVisualOverflow { get; set; } = false;

    public RenderShrinkWrappingViewport(AxisDirection axisDirection = AxisDirection.down, AxisDirection crossAxisDirection = default!, ViewportOffset offset = default!, SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, Clip clipBehavior = Clip.hardEdge, ScrollCacheExtent? scrollCacheExtent = null, List<RenderSliver>? children = null) : base(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection, offset: offset, paintOrder: paintOrder, clipBehavior: clipBehavior, scrollCacheExtent: scrollCacheExtent)
    {
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverLogicalContainerParentData)
        {
            child.parentData = new SliverLogicalContainerParentData();
        }
    }

    public override bool debugThrowIfNotCheckingIntrinsics()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!debugCheckingIntrinsics)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} does not support returning intrinsic dimensions."), new ErrorDescription("Calculating the intrinsic dimensions would require instantiating every child of " + "the viewport, which defeats the point of viewports being lazy."), new ErrorHint("If you are merely trying to shrink-wrap the viewport in the main axis direction, " + "you should be able to achieve that effect by just giving the viewport loose " + "constraints, without needing to measure its intrinsic dimensions.") });
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
                    case Axis.vertical:
                        {
                            if (!constraints.hasBoundedWidth)
                            {
                                throw new FlutterError("Vertical viewport was given unbounded width.\n" + "Viewports expand in the cross axis to fill their container and " + "constrain their children to match their extent in the cross axis. " + "In this case, a vertical shrinkwrapping viewport was given an " + "unlimited amount of horizontal space in which to expand.");
                            }
                            break;
                        }
                    case Axis.horizontal:
                        {
                            if (!constraints.hasBoundedHeight)
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
        BoxConstraints constraintsLocal = constraints;
        if (firstChild is null)
        {
            DartRuntimePrimitives.Assert(() => _debugCheckHasBoundedCrossAxis());
            size = axis switch { Axis.vertical => new Size(constraintsLocal.maxWidth, constraintsLocal.minHeight), Axis.horizontal => new Size(constraintsLocal.minWidth, constraintsLocal.maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            offset.applyViewportDimension(0.0);
            _maxScrollExtent = 0.0;
            _shrinkWrapExtent = 0.0;
            _hasVisualOverflow = false;
            offset.applyContentDimensions(0.0, 0.0);
            return;
        }
        DartRuntimePrimitives.Assert(() => _debugCheckHasBoundedCrossAxis());
        var (mainAxisExtent, crossAxisExtent) = axis switch { Axis.vertical => (constraintsLocal.maxHeight, constraintsLocal.maxWidth), Axis.horizontal => (constraintsLocal.maxWidth, constraintsLocal.maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double correction = default!;
        double effectiveExtent = default!;
        while (true)
        {
            correction = _attemptLayout(mainAxisExtent, crossAxisExtent, offset.pixels);
            if (correction != 0.0)
            {
                offset.correctBy(correction);
            }
            else
            {
                effectiveExtent = axis switch { Axis.vertical => constraintsLocal.constrainHeight(_shrinkWrapExtent), Axis.horizontal => constraintsLocal.constrainWidth(_shrinkWrapExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                bool didAcceptViewportDimension = offset.applyViewportDimension(effectiveExtent);
                bool didAcceptContentDimension = offset.applyContentDimensions(0.0, Math.Max(0.0, _maxScrollExtent - effectiveExtent));
                if (didAcceptViewportDimension && didAcceptContentDimension)
                {
                    break;
                }
            }
        }
        size = axis switch { Axis.vertical => constraintsLocal.constrainDimensions(crossAxisExtent, effectiveExtent), Axis.horizontal => constraintsLocal.constrainDimensions(effectiveExtent, crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    internal virtual double _attemptLayout(double mainAxisExtent, double crossAxisExtent, double correctedOffset)
    {
        DartRuntimePrimitives.Assert(() => !double.IsNaN(mainAxisExtent));
        DartRuntimePrimitives.Assert(() => mainAxisExtent >= 0.0);
        DartRuntimePrimitives.Assert(() => double.IsFinite(crossAxisExtent));
        DartRuntimePrimitives.Assert(() => crossAxisExtent >= 0.0);
        DartRuntimePrimitives.Assert(() => double.IsFinite(correctedOffset));
        _maxScrollExtent = 0.0;
        _shrinkWrapExtent = 0.0;
        _hasVisualOverflow = correctedOffset < 0.0;
        if (double.IsFinite(mainAxisExtent))
        {
            _calculatedCacheExtent = _scrollCacheExtent._calculateCacheOffset(mainAxisExtent);
        }
        else
        {
            _calculatedCacheExtent = 0.0;
        }
        return layoutChildSequence(child: firstChild, scrollOffset: Math.Max(0.0, correctedOffset), overlap: Math.Min(0.0, correctedOffset), layoutOffset: Math.Max(0.0, -correctedOffset), remainingPaintExtent: mainAxisExtent + Math.Min(0.0, correctedOffset), mainAxisExtent: mainAxisExtent, crossAxisExtent: crossAxisExtent, growthDirection: GrowthDirection.forward, advance: childAfter, remainingCacheExtent: mainAxisExtent + (2L * DartRuntimePrimitives.RequireValue(_calculatedCacheExtent)), cacheOrigin: -DartRuntimePrimitives.RequireValue(_calculatedCacheExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasVisualOverflow => _hasVisualOverflow;
    public override void updateOutOfBandData(GrowthDirection growthDirection, SliverGeometry childLayoutGeometry)
    {
        DartRuntimePrimitives.Assert(() => Equals(growthDirection, GrowthDirection.forward));
        _maxScrollExtent += childLayoutGeometry.scrollExtent;
        if (childLayoutGeometry.hasVisualOverflow)
        {
            _hasVisualOverflow = true;
        }
        _shrinkWrapExtent += childLayoutGeometry.maxPaintExtent;
    }

    public override void updateChildLayoutOffset(RenderSliver child, double layoutOffset, GrowthDirection growthDirection)
    {
        DartRuntimePrimitives.Assert(() => Equals(growthDirection, GrowthDirection.forward));
        var childParentData = ((SliverLogicalParentData?)(object?)child.parentData!)!;
        childParentData.layoutOffset = layoutOffset;
    }

    public override Offset paintOffsetOf(RenderSliver child)
    {
        var childParentData = ((SliverLogicalParentData?)(object?)child.parentData!)!;
        return computeAbsolutePaintOffset(child, DartRuntimePrimitives.RequireValue(childParentData.layoutOffset), GrowthDirection.forward);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double scrollOffsetOf(RenderSliver child, double scrollOffsetWithinChild)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        DartRuntimePrimitives.Assert(() => Equals(child.constraints.growthDirection, GrowthDirection.forward));
        var scrollOffsetToChild = 0.0;
        RenderSliver? current = firstChild;
        while (!Equals(current, child))
        {
            scrollOffsetToChild += current!.geometry!.scrollExtent;
            current = childAfter(current);
        }
        return scrollOffsetToChild + scrollOffsetWithinChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double maxScrollObstructionExtentBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        DartRuntimePrimitives.Assert(() => Equals(child.constraints.growthDirection, GrowthDirection.forward));
        var pinnedExtent = 0.0;
        RenderSliver? current = firstChild;
        while (!Equals(current, child))
        {
            pinnedExtent += current!.geometry!.maxScrollObstructionExtent;
            current = childAfter(current);
        }
        return pinnedExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        Offset offset = paintOffsetOf(((RenderSliver?)(object?)child)!);
        transform.translateByDouble(offset.dx, offset.dy, 0, 1);
    }

    public override double computeChildMainAxisPosition(RenderSliver child, double parentMainAxisPosition)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        double layoutOffsetLocal = DartRuntimePrimitives.RequireValue(((SliverLogicalParentData?)(object?)child.parentData!)!.layoutOffset);
        return SliverLibrary.applyGrowthDirectionToAxisDirection(child.constraints.axisDirection, child.constraints.growthDirection) switch { AxisDirection.down => parentMainAxisPosition - layoutOffsetLocal, AxisDirection.right => parentMainAxisPosition - layoutOffsetLocal, AxisDirection.up => size.height - parentMainAxisPosition - layoutOffsetLocal, AxisDirection.left => size.width - parentMainAxisPosition - layoutOffsetLocal, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long indexOfFirstChild => 0L;
    public override string labelForChild(long index) => $"child {index}";
}

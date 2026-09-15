// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_fill.dart
using Doroti.Runtime;

namespace Doroti.Framework.Rendering;

public class RenderSliverFillViewport : RenderSliverFixedExtentBoxAdaptor
{
    internal virtual double _viewportFraction { get; set; } = default!;
    internal virtual bool _allowImplicitScrolling { get; set; } = default!;

    public RenderSliverFillViewport(RenderSliverBoxChildManager childManager, double viewportFraction = 1.0, bool allowImplicitScrolling = true) : base(childManager: childManager)
    {
        this._viewportFraction = viewportFraction;
        this._allowImplicitScrolling = allowImplicitScrolling;
        System.Diagnostics.Debug.Assert((viewportFraction > 0.0));
    }

    public override double? itemExtent => (((SliverConstraints)constraints).viewportMainAxisExtent * this.viewportFraction);
    public virtual double viewportFraction
    {
        get => this._viewportFraction;
        set
        {
            var __value = value;
            if ((this._viewportFraction == __value))
            {
                return;
            }
            _viewportFraction = __value;
            markNeedsLayout();
        }
    }
    public virtual bool allowImplicitScrolling
    {
        get => this._allowImplicitScrolling;
        set
        {
            var __value = value;
            if ((this._allowImplicitScrolling == __value))
            {
                return;
            }
            _allowImplicitScrolling = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        if (this.allowImplicitScrolling)
        {
            base.visitChildrenForSemantics((Action<RenderObject>)visitor);
            return;
        }
        double visibleStart = ((SliverConstraints)constraints).scrollOffset;
        double visibleEnd = (visibleStart + ((SliverConstraints)constraints).viewportMainAxisExtent);
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            double childStart = DartRuntimePrimitives.RequireValue((((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!).layoutOffset);
            if ((childStart >= visibleEnd))
            {
                break;
            }
            if (((childStart + this.itemExtent) > visibleStart))
            {
                visitor(child);
            }
            child = childAfter(child);
        }
    }

}

public class RenderSliverFillRemainingWithScrollable : RenderSliverSingleBoxAdapter
{
    public RenderSliverFillRemainingWithScrollable(RenderBox? child = null) : base(child: child)
    {
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        double extent = (((SliverConstraints)constraintsLocal).remainingPaintExtent - Math.Min(((SliverConstraints)constraintsLocal).overlap, 0.0));
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: 0.0, to: ((SliverConstraints)constraintsLocal).viewportMainAxisExtent);
        if ((child is not null))
        {
            var maxExtentLocal = extent;
            if (((extent == 0L) && (cacheExtentLocal > 0L)))
            {
                maxExtentLocal = cacheExtentLocal;
            }
            child!.layout(constraintsLocal.asBoxConstraints(minExtent: extent, maxExtent: maxExtentLocal));
        }
        double paintedChildSize = calculatePaintOffset(constraintsLocal, from: 0.0, to: extent);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize));
        DartRuntimePrimitives.Assert(() => (paintedChildSize >= 0.0));
        geometry = new SliverGeometry(scrollExtent: ((SliverConstraints)constraintsLocal).viewportMainAxisExtent, paintExtent: paintedChildSize, maxPaintExtent: paintedChildSize, hasVisualOverflow: ((extent > ((SliverConstraints)constraintsLocal).remainingPaintExtent) || (((SliverConstraints)constraintsLocal).scrollOffset > 0.0)), cacheExtent: cacheExtentLocal);
        if ((child is not null))
        {
            setChildParentData(child!, constraintsLocal, geometry!);
        }
    }

}

public class RenderSliverFillRemaining : RenderSliverSingleBoxAdapter
{
    public RenderSliverFillRemaining(RenderBox? child = null) : base(child: child)
    {
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        double extent = (((SliverConstraints)constraintsLocal).viewportMainAxisExtent - ((SliverConstraints)constraintsLocal).precedingScrollExtent);
        if ((child is not null))
        {
            double childExtent = (((SliverConstraints)constraintsLocal).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => child!.getMaxIntrinsicWidth(((SliverConstraints)constraintsLocal).crossAxisExtent), global::Doroti.Framework.Painting.Axis.vertical => child!.getMaxIntrinsicHeight(((SliverConstraints)constraintsLocal).crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            extent = Math.Max(extent, childExtent);
            child!.layout(constraintsLocal.asBoxConstraints(minExtent: extent, maxExtent: extent));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(extent));
        double paintedChildSize = calculatePaintOffset(constraintsLocal, from: 0.0, to: extent);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize));
        DartRuntimePrimitives.Assert(() => (paintedChildSize >= 0.0));
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: 0.0, to: extent);
        geometry = new SliverGeometry(scrollExtent: extent, paintExtent: paintedChildSize, maxPaintExtent: paintedChildSize, hasVisualOverflow: ((extent > ((SliverConstraints)constraintsLocal).remainingPaintExtent) || (((SliverConstraints)constraintsLocal).scrollOffset > 0.0)), cacheExtent: cacheExtentLocal);
        if ((child is not null))
        {
            setChildParentData(child!, constraintsLocal, geometry!);
        }
    }

}

public class RenderSliverFillRemainingAndOverscroll : RenderSliverSingleBoxAdapter
{
    public RenderSliverFillRemainingAndOverscroll(RenderBox? child = null) : base(child: child)
    {
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        double extent = (((SliverConstraints)constraintsLocal).viewportMainAxisExtent - ((SliverConstraints)constraintsLocal).precedingScrollExtent);
        double maxExtentLocal = (((SliverConstraints)constraintsLocal).remainingPaintExtent - Math.Min(((SliverConstraints)constraintsLocal).overlap, 0.0));
        if ((child is not null))
        {
            double childExtent = (((SliverConstraints)constraintsLocal).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => child!.getMaxIntrinsicWidth(((SliverConstraints)constraintsLocal).crossAxisExtent), global::Doroti.Framework.Painting.Axis.vertical => child!.getMaxIntrinsicHeight(((SliverConstraints)constraintsLocal).crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            extent = Math.Max(extent, childExtent);
            maxExtentLocal = Math.Max(extent, maxExtentLocal);
            child!.layout(constraintsLocal.asBoxConstraints(minExtent: extent, maxExtent: maxExtentLocal));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(extent));
        double paintedChildSize = calculatePaintOffset(constraintsLocal, from: 0.0, to: extent);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize));
        DartRuntimePrimitives.Assert(() => (paintedChildSize >= 0.0));
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: 0.0, to: extent);
        geometry = new SliverGeometry(scrollExtent: extent, paintExtent: Math.Min(maxExtentLocal, ((SliverConstraints)constraintsLocal).remainingPaintExtent), maxPaintExtent: maxExtentLocal, hasVisualOverflow: ((extent > ((SliverConstraints)constraintsLocal).remainingPaintExtent) || (((SliverConstraints)constraintsLocal).scrollOffset > 0.0)), cacheExtent: cacheExtentLocal);
        if ((child is not null))
        {
            setChildParentData(child!, constraintsLocal, geometry!);
        }
    }

}


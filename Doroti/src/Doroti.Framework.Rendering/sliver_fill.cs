// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_fill.dart
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

namespace Doroti.Generated.Framework.Rendering;

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
        double visibleStart__2877 = ((SliverConstraints)constraints).scrollOffset;
        double visibleEnd__2935 = (visibleStart__2877 + ((SliverConstraints)constraints).viewportMainAxisExtent);
        RenderBox? child__3015 = firstChild;
        while ((child__3015 is not null))
        {
            double childStart__3082 = DartRuntimePrimitives.RequireValue((((SliverMultiBoxAdaptorParentData?)(object?)child__3015.parentData!)!).layoutOffset);
            if ((childStart__3082 >= visibleEnd__2935))
            {
                break;
            }
            if (((childStart__3082 + this.itemExtent) > visibleStart__2877))
            {
                visitor(child__3015);
            }
            child__3015 = childAfter(child__3015);
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
        SliverConstraints constraints__4718 = this.constraints;
        double extent__4767 = (((SliverConstraints)constraints__4718).remainingPaintExtent - Math.Min(((SliverConstraints)constraints__4718).overlap, 0.0));
        double cacheExtent__4865 = calculateCacheOffset(constraints__4718, from: 0.0, to: ((SliverConstraints)constraints__4718).viewportMainAxisExtent);
        if ((child is not null))
        {
            var maxExtent__5025 = extent__4767;
            if (((extent__4767 == 0L) && (cacheExtent__4865 > 0L)))
            {
                maxExtent__5025 = cacheExtent__4865;
            }
            child!.layout(constraints__4718.asBoxConstraints(minExtent: extent__4767, maxExtent: maxExtent__5025));
        }
        double paintedChildSize__5439 = calculatePaintOffset(constraints__4718, from: 0.0, to: extent__4767);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize__5439));
        DartRuntimePrimitives.Assert(() => (paintedChildSize__5439 >= 0.0));
        geometry = new SliverGeometry(scrollExtent: ((SliverConstraints)constraints__4718).viewportMainAxisExtent, paintExtent: paintedChildSize__5439, maxPaintExtent: paintedChildSize__5439, hasVisualOverflow: ((extent__4767 > ((SliverConstraints)constraints__4718).remainingPaintExtent) || (((SliverConstraints)constraints__4718).scrollOffset > 0.0)), cacheExtent: cacheExtent__4865);
        if ((child is not null))
        {
            setChildParentData(child!, constraints__4718, geometry!);
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
        SliverConstraints constraints__7271 = this.constraints;
        double extent__7444 = (((SliverConstraints)constraints__7271).viewportMainAxisExtent - ((SliverConstraints)constraints__7271).precedingScrollExtent);
        if ((child is not null))
        {
            double childExtent__7570 = (((SliverConstraints)constraints__7271).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => child!.getMaxIntrinsicWidth(((SliverConstraints)constraints__7271).crossAxisExtent), global::Doroti.Generated.Framework.Painting.Axis.vertical => child!.getMaxIntrinsicHeight(((SliverConstraints)constraints__7271).crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            extent__7444 = Math.Max(extent__7444, childExtent__7570);
            child!.layout(constraints__7271.asBoxConstraints(minExtent: extent__7444, maxExtent: extent__7444));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(extent__7444));
        double paintedChildSize__8436 = calculatePaintOffset(constraints__7271, from: 0.0, to: extent__7444);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize__8436));
        DartRuntimePrimitives.Assert(() => (paintedChildSize__8436 >= 0.0));
        double cacheExtent__8607 = calculateCacheOffset(constraints__7271, from: 0.0, to: extent__7444);
        geometry = new SliverGeometry(scrollExtent: extent__7444, paintExtent: paintedChildSize__8436, maxPaintExtent: paintedChildSize__8436, hasVisualOverflow: ((extent__7444 > ((SliverConstraints)constraints__7271).remainingPaintExtent) || (((SliverConstraints)constraints__7271).scrollOffset > 0.0)), cacheExtent: cacheExtent__8607);
        if ((child is not null))
        {
            setChildParentData(child!, constraints__7271, geometry!);
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
        SliverConstraints constraints__10416 = this.constraints;
        double extent__10589 = (((SliverConstraints)constraints__10416).viewportMainAxisExtent - ((SliverConstraints)constraints__10416).precedingScrollExtent);
        double maxExtent__10832 = (((SliverConstraints)constraints__10416).remainingPaintExtent - Math.Min(((SliverConstraints)constraints__10416).overlap, 0.0));
        if ((child is not null))
        {
            double childExtent__10960 = (((SliverConstraints)constraints__10416).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => child!.getMaxIntrinsicWidth(((SliverConstraints)constraints__10416).crossAxisExtent), global::Doroti.Generated.Framework.Painting.Axis.vertical => child!.getMaxIntrinsicHeight(((SliverConstraints)constraints__10416).crossAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            extent__10589 = Math.Max(extent__10589, childExtent__10960);
            maxExtent__10832 = Math.Max(extent__10589, maxExtent__10832);
            child!.layout(constraints__10416.asBoxConstraints(minExtent: extent__10589, maxExtent: maxExtent__10832));
        }
        DartRuntimePrimitives.Assert(() => double.IsFinite(extent__10589));
        double paintedChildSize__12069 = calculatePaintOffset(constraints__10416, from: 0.0, to: extent__10589);
        DartRuntimePrimitives.Assert(() => double.IsFinite(paintedChildSize__12069));
        DartRuntimePrimitives.Assert(() => (paintedChildSize__12069 >= 0.0));
        double cacheExtent__12240 = calculateCacheOffset(constraints__10416, from: 0.0, to: extent__10589);
        geometry = new SliverGeometry(scrollExtent: extent__10589, paintExtent: Math.Min(maxExtent__10832, ((SliverConstraints)constraints__10416).remainingPaintExtent), maxPaintExtent: maxExtent__10832, hasVisualOverflow: ((extent__10589 > ((SliverConstraints)constraints__10416).remainingPaintExtent) || (((SliverConstraints)constraints__10416).scrollOffset > 0.0)), cacheExtent: cacheExtent__12240);
        if ((child is not null))
        {
            setChildParentData(child!, constraints__10416, geometry!);
        }
    }

}


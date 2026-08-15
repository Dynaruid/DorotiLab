// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/pinned_header_sliver.dart
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

namespace Doroti.Generated.Framework.Widgets;

public class PinnedHeaderSliver : StatelessWidget
{
    public virtual Widget? child { get; private set; }

    public PinnedHeaderSliver(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget? child = null) : base(key: key)
    {
        this.child = child;
    }

    public override Widget build(BuildContext context) => DartRuntimePrimitives.ConvertValue<Widget>(new _PinnedHeaderSliver__pinned_header_sliver(child: new Semantics(container: true, explicitChildNodes: true, child: this.child)));
}

internal class _PinnedHeaderSliver__pinned_header_sliver : SingleChildRenderObjectWidget
{
    internal _PinnedHeaderSliver__pinned_header_sliver(Widget? child = null) : base(child: child)
    {
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderPinnedHeaderSliver__pinned_header_sliver());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RenderPinnedHeaderSliver__pinned_header_sliver : global::Doroti.Generated.Framework.Rendering.RenderSliverSingleBoxAdapter
{
    internal _RenderPinnedHeaderSliver__pinned_header_sliver()
    {
    }

    public virtual double childExtent
    {
        get
        {
            if ((this.child is null))
            {
                return 0.0;
            }
            DartRuntimePrimitives.Assert(() => this.child!.hasSize);
            return (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => this.child!.size.height, global::Doroti.Generated.Framework.Painting.Axis.horizontal => this.child!.size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override double childMainAxisPosition(global::Doroti.Generated.Framework.Rendering.RenderObject child) => 0;
    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.SliverConstraints constraints__3834 = this.constraints;
        this.child?.layout(constraints__3834.asBoxConstraints(), parentUsesSize: true);
        double layoutExtent__3957 = Dart_uiLibrary.clampDouble((this.childExtent - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3834).scrollOffset), 0, ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3834).remainingPaintExtent);
        double paintExtent__4104 = Math.Min(this.childExtent, (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3834).remainingPaintExtent - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3834).overlap));
        geometry = new global::Doroti.Generated.Framework.Rendering.SliverGeometry(scrollExtent: this.childExtent, paintOrigin: ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3834).overlap, paintExtent: paintExtent__4104, layoutExtent: layoutExtent__3957, maxPaintExtent: this.childExtent, maxScrollObstructionExtent: this.childExtent, cacheExtent: calculateCacheOffset(constraints__3834, from: 0.0, to: this.childExtent), hasVisualOverflow: true);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if (((this.geometry is not null) && (this.geometry!.layoutExtent < this.childExtent)))
        {
            config.addTagForChildren(global::Doroti.Generated.Framework.Rendering.RenderViewport.excludeFromScrolling);
        }
    }

}


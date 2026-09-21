// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/pinned_header_sliver.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class PinnedHeaderSliver : StatelessWidget
{
    public virtual Widget? child { get; private set; }

    public PinnedHeaderSliver(Key? key = null, Widget? child = null)
        : base(key: key)
    {
        this.child = child;
    }

    public override Widget build(BuildContext context) =>
        DartRuntimePrimitives.ConvertValue<Widget>(
            new _PinnedHeaderSliver__pinned_header_sliver(
                child: new Semantics(container: true, explicitChildNodes: true, child: child)
            )
        );
}

internal class _PinnedHeaderSliver__pinned_header_sliver : SingleChildRenderObjectWidget
{
    internal _PinnedHeaderSliver__pinned_header_sliver(Widget? child = null)
        : base(child: child) { }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderPinnedHeaderSliver__pinned_header_sliver();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _RenderPinnedHeaderSliver__pinned_header_sliver : RenderSliverSingleBoxAdapter
{
    internal _RenderPinnedHeaderSliver__pinned_header_sliver() { }

    public virtual double childExtent
    {
        get
        {
            if (child is null)
            {
                return 0.0;
            }
            DartRuntimePrimitives.Assert(() => child!.hasSize);
            return constraints.axis switch
            {
                Axis.vertical => child!.size.height,
                Axis.horizontal => child!.size.width,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
    }

    public override double childMainAxisPosition(RenderObject child) => 0;

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
        child?.layout(constraintsLocal.asBoxConstraints(), parentUsesSize: true);
        double layoutExtentLocal = Dart_uiLibrary.clampDouble(
            childExtent - constraintsLocal.scrollOffset,
            0,
            constraintsLocal.remainingPaintExtent
        );
        double paintExtentLocal = Math.Min(
            childExtent,
            constraintsLocal.remainingPaintExtent - constraintsLocal.overlap
        );
        geometry = new SliverGeometry(
            scrollExtent: childExtent,
            paintOrigin: constraintsLocal.overlap,
            paintExtent: paintExtentLocal,
            layoutExtent: layoutExtentLocal,
            maxPaintExtent: childExtent,
            maxScrollObstructionExtent: childExtent,
            cacheExtent: calculateCacheOffset(constraintsLocal, from: 0.0, to: childExtent),
            hasVisualOverflow: true
        );
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if ((geometry is not null) && (geometry!.layoutExtent < childExtent))
        {
            config.addTagForChildren(RenderViewport.excludeFromScrolling);
        }
    }
}

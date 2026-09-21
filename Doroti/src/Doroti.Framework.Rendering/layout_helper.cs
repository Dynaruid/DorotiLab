// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/layout_helper.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public delegate Size ChildLayouter(RenderBox child, BoxConstraints constraints);

public delegate double? ChildBaselineGetter(
    RenderBox child,
    BoxConstraints constraints,
    TextBaseline baseline
);

public abstract class ChildLayoutHelper
{
    public static Size dryLayoutChild(RenderBox child, BoxConstraints constraints)
    {
        return child.getDryLayout(constraints);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Size layoutChild(RenderBox child, BoxConstraints constraints)
    {
        child.layout(constraints, parentUsesSize: true);
        return child.size;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static double? getDryBaseline(
        RenderBox child,
        BoxConstraints constraints,
        TextBaseline baseline
    )
    {
        return child.getDryBaseline(constraints, baseline);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static double? getBaseline(
        RenderBox child,
        BoxConstraints constraints,
        TextBaseline baseline
    )
    {
        DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => Equals(child.constraints, constraints));
        return child.getDistanceToBaseline(baseline, onlyReal: true);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

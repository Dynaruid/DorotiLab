// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/layout_helper.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public delegate Size ChildLayouter(RenderBox child, BoxConstraints constraints);

public delegate double? ChildBaselineGetter(RenderBox child, BoxConstraints constraints, TextBaseline baseline);

public abstract class ChildLayoutHelper
{
    public static global::Doroti.Ui.Size dryLayoutChild(RenderBox child, BoxConstraints constraints)
    {
        return child.getDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Size layoutChild(RenderBox child, BoxConstraints constraints)
    {
        child.layout(constraints, parentUsesSize: true);
        return ((RenderBox)child).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double? getDryBaseline(RenderBox child, BoxConstraints constraints, TextBaseline baseline)
    {
        return child.getDryBaseline(constraints, baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double? getBaseline(RenderBox child, BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !child.debugNeedsLayout);
        DartRuntimePrimitives.Assert(() => (Equals(((RenderBox)child).constraints, constraints)));
        return child.getDistanceToBaseline(baseline, onlyReal: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


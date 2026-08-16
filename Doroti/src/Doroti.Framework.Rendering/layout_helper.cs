// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/layout_helper.dart
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
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderBox)child).constraints, constraints)));
        return child.getDistanceToBaseline(baseline, onlyReal: true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


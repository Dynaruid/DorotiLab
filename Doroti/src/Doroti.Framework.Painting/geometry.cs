// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/geometry.dart
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

namespace Doroti.Framework.Painting;

public static partial class GeometryLibrary
{
    public static Offset positionDependentBox(Size size, Size childSize, Offset target, bool preferBelow, double verticalOffset = 0.0, double margin = 10.0)
    {
        bool fitsBelow = (((target.dy + verticalOffset) + childSize.height) <= (size.height - margin));
        bool fitsAbove = (((target.dy - verticalOffset) - childSize.height) >= margin);
        var tooltipBelow = ((fitsAbove == fitsBelow) ? preferBelow : fitsBelow);
        double y = default!;
        if (tooltipBelow)
        {
            y = Math.Min((target.dy + verticalOffset), (size.height - margin));
        }
        else
        {
            y = Math.Max(((target.dy - verticalOffset) - childSize.height), margin);
        }
        double flexibleSpace = (size.width - childSize.width);
        double x = ((flexibleSpace <= (2L * margin)) ? (flexibleSpace / 2.0) : Dart_uiLibrary.clampDouble((target.dx - (childSize.width / 2L)), margin, (flexibleSpace - margin)));
        return new global::Doroti.Ui.Offset(x, y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}


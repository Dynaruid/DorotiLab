// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/geometry.dart
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class GeometryLibrary
{
    public static Offset positionDependentBox(Size size, Size childSize, Offset target, bool preferBelow, double verticalOffset = 0.0, double margin = 10.0)
    {
        bool fitsBelow = (target.dy + verticalOffset + childSize.height) <= (size.height - margin);
        bool fitsAbove = (target.dy - verticalOffset - childSize.height) >= margin;
        var tooltipBelow = (fitsAbove == fitsBelow) ? preferBelow : fitsBelow;
        double y = default!;
        if (tooltipBelow)
        {
            y = Math.Min(target.dy + verticalOffset, size.height - margin);
        }
        else
        {
            y = Math.Max(target.dy - verticalOffset - childSize.height, margin);
        }
        double flexibleSpace = size.width - childSize.width;
        double x = (flexibleSpace <= (2L * margin)) ? (flexibleSpace / 2.0) : Dart_uiLibrary.clampDouble(target.dx - (childSize.width / 2L), margin, flexibleSpace - margin);
        return new global::Doroti.Ui.Offset(x, y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}


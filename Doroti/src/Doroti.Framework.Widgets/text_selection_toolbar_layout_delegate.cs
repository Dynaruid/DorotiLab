// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection_toolbar_layout_delegate.dart
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

namespace Doroti.Framework.Widgets;

public class TextSelectionToolbarLayoutDelegate : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Offset anchorAbove { get; private set; } = default!;
    public virtual Offset anchorBelow { get; private set; } = default!;
    public virtual bool? fitsAbove { get; private set; }

    public TextSelectionToolbarLayoutDelegate(Offset anchorAbove, Offset anchorBelow, bool? fitsAbove = null)
    {
        this.anchorAbove = anchorAbove;
        this.anchorBelow = anchorBelow;
        this.fitsAbove = fitsAbove;
    }

    public static double centerOn(double position, double width, double max)
    {
        if (((position - (width / 2.0)) < 0.0))
        {
            return 0.0;
        }
        if (((position + (width / 2.0)) > max))
        {
            return (max - width);
        }
        return (position - (width / 2.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        bool fitsAbove__2580 = (this.fitsAbove ?? (this.anchorAbove.dy >= childSize.height));
        global::Doroti.Ui.Offset anchor__2663 = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(fitsAbove__2580) ? this.anchorAbove : this.anchorBelow));
        return new global::Doroti.Ui.Offset(TextSelectionToolbarLayoutDelegate.centerOn(anchor__2663.dx, childSize.width, size.width), (DartRuntimePrimitives.RequireValue(fitsAbove__2580) ? Math.Max(0.0, (anchor__2663.dy - childSize.height)) : anchor__2663.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (TextSelectionToolbarLayoutDelegate)(object)oldDelegate;
        return (((!object.Equals(this.anchorAbove, ((TextSelectionToolbarLayoutDelegate)__oldDelegate).anchorAbove)) || (!object.Equals(this.anchorBelow, ((TextSelectionToolbarLayoutDelegate)__oldDelegate).anchorBelow))) || (this.fitsAbove != ((TextSelectionToolbarLayoutDelegate)__oldDelegate).fitsAbove));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


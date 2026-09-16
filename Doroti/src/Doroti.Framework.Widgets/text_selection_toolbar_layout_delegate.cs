// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection_toolbar_layout_delegate.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class TextSelectionToolbarLayoutDelegate : SingleChildLayoutDelegate
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
        if ((position - (width / 2.0)) < 0.0)
        {
            return 0.0;
        }
        if ((position + (width / 2.0)) > max)
        {
            return max - width;
        }
        return position - (width / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints)
    {
        return constraints.loosen();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        bool fitsAboveLocal = fitsAbove ?? (anchorAbove.dy >= childSize.height);
        Offset anchor = DartRuntimePrimitives.RequireValue(fitsAboveLocal) ? anchorAbove : anchorBelow;
        return new Offset(centerOn(anchor.dx, childSize.width, size.width), DartRuntimePrimitives.RequireValue(fitsAboveLocal) ? Math.Max(0.0, anchor.dy - childSize.height) : anchor.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (TextSelectionToolbarLayoutDelegate)oldDelegate;
        return (!Equals(anchorAbove, __oldDelegate.anchorAbove)) || (!Equals(anchorBelow, __oldDelegate.anchorBelow)) || (fitsAbove != __oldDelegate.fitsAbove);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


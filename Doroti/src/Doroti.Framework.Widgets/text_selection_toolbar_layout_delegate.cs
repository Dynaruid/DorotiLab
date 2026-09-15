// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/text_selection_toolbar_layout_delegate.dart
#pragma warning disable CS8600, CS8603, CS8605
using Doroti.Runtime;
using Doroti.Ui;

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
        bool fitsAboveLocal = (this.fitsAbove ?? (this.anchorAbove.dy >= childSize.height));
        global::Doroti.Ui.Offset anchor = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(fitsAboveLocal) ? this.anchorAbove : this.anchorBelow));
        return new global::Doroti.Ui.Offset(TextSelectionToolbarLayoutDelegate.centerOn(anchor.dx, childSize.width, size.width), (DartRuntimePrimitives.RequireValue(fitsAboveLocal) ? Math.Max(0.0, (anchor.dy - childSize.height)) : anchor.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (TextSelectionToolbarLayoutDelegate)(object)oldDelegate;
        return (((!object.Equals(this.anchorAbove, ((TextSelectionToolbarLayoutDelegate)__oldDelegate).anchorAbove)) || (!object.Equals(this.anchorBelow, ((TextSelectionToolbarLayoutDelegate)__oldDelegate).anchorBelow))) || (this.fitsAbove != ((TextSelectionToolbarLayoutDelegate)__oldDelegate).fitsAbove));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


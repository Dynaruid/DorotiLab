// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/desktop_text_selection_toolbar_layout_delegate.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class DesktopTextSelectionToolbarLayoutDelegate : SingleChildLayoutDelegate
{
    public virtual Offset anchor { get; private set; } = default!;

    public DesktopTextSelectionToolbarLayoutDelegate(Offset anchor)
    {
        this.anchor = anchor;
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints)
    {
        return constraints.loosen();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        var overhang = new Offset(
            anchor.dx + childSize.width - size.width,
            anchor.dy + childSize.height - size.height
        );
        return new Offset(
            (overhang.dx > 0.0) ? (anchor.dx - overhang.dx) : anchor.dx,
            (overhang.dy > 0.0) ? (anchor.dy - overhang.dy) : anchor.dy
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (DesktopTextSelectionToolbarLayoutDelegate)oldDelegate;
        return !Equals(anchor, __oldDelegate.anchor);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/desktop_text_selection_toolbar_layout_delegate.dart
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class DesktopTextSelectionToolbarLayoutDelegate : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Offset anchor { get; private set; } = default!;

    public DesktopTextSelectionToolbarLayoutDelegate(Offset anchor)
    {
        this.anchor = anchor;
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)constraints.loosen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        var overhang = new global::Doroti.Ui.Offset(((this.anchor.dx + childSize.width) - size.width), ((this.anchor.dy + childSize.height) - size.height));
        return new global::Doroti.Ui.Offset(((overhang.dx > 0.0) ? (this.anchor.dx - overhang.dx) : this.anchor.dx), ((overhang.dy > 0.0) ? (this.anchor.dy - overhang.dy) : this.anchor.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (DesktopTextSelectionToolbarLayoutDelegate)oldDelegate;
        return (!Equals(this.anchor, ((DesktopTextSelectionToolbarLayoutDelegate)__oldDelegate).anchor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


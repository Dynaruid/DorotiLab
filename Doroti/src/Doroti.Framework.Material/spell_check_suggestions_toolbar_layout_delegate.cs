// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/spell_check_suggestions_toolbar_layout_delegate.dart

using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SpellCheckSuggestionsToolbarLayoutDelegate : SingleChildLayoutDelegate
{
    public virtual Offset anchor { get; private set; } = default!;

    public SpellCheckSuggestionsToolbarLayoutDelegate(Offset anchor)
    {
        this.anchor = anchor;
    }

    public override BoxConstraints getConstraintsForChild(BoxConstraints constraints)
    {
        return constraints.loosen();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        return new Offset(
            TextSelectionToolbarLayoutDelegate.centerOn(anchor.dx, childSize.width, size.width),
            ((anchor.dy + childSize.height) > size.height)
                ? (size.height - childSize.height)
                : anchor.dy
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (SpellCheckSuggestionsToolbarLayoutDelegate)oldDelegate;
        return !Equals(anchor, __oldDelegate.anchor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

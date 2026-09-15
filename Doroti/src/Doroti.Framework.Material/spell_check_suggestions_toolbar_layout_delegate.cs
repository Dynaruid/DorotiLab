// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/spell_check_suggestions_toolbar_layout_delegate.dart
#pragma warning disable CS8600, CS8603
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SpellCheckSuggestionsToolbarLayoutDelegate : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Offset anchor { get; private set; } = default!;

    public SpellCheckSuggestionsToolbarLayoutDelegate(Offset anchor)
    {
        this.anchor = anchor;
    }

    public override global::Doroti.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        return new global::Doroti.Ui.Offset(TextSelectionToolbarLayoutDelegate.centerOn(this.anchor.dx, childSize.width, size.width), (((this.anchor.dy + childSize.height) > size.height) ? (size.height - childSize.height) : this.anchor.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (SpellCheckSuggestionsToolbarLayoutDelegate)(object)oldDelegate;
        return (!object.Equals(this.anchor, ((SpellCheckSuggestionsToolbarLayoutDelegate)__oldDelegate).anchor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

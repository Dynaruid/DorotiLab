// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/spell_check_suggestions_toolbar_layout_delegate.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class SpellCheckSuggestionsToolbarLayoutDelegate : global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Offset anchor { get; private set; } = default!;

    public SpellCheckSuggestionsToolbarLayoutDelegate(Offset anchor)
    {
        this.anchor = anchor;
    }

    public override global::Doroti.Generated.Framework.Rendering.BoxConstraints getConstraintsForChild(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Offset getPositionForChild(Size size, Size childSize)
    {
        return new global::Doroti.Ui.Offset(TextSelectionToolbarLayoutDelegate.centerOn(this.anchor.dx, childSize.width, size.width), (((this.anchor.dy + childSize.height) > size.height) ? (size.height - childSize.height) : this.anchor.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (SpellCheckSuggestionsToolbarLayoutDelegate)(object)oldDelegate;
        return (!object.Equals(this.anchor, ((SpellCheckSuggestionsToolbarLayoutDelegate)__oldDelegate).anchor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

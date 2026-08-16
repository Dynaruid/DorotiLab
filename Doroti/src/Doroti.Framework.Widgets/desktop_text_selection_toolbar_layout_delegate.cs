// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/desktop_text_selection_toolbar_layout_delegate.dart
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

public class DesktopTextSelectionToolbarLayoutDelegate : global::Doroti.Framework.Rendering.SingleChildLayoutDelegate
{
    public virtual Offset anchor { get; private set; } = default!;

    public DesktopTextSelectionToolbarLayoutDelegate(Offset anchor)
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
        var overhang__1366 = new global::Doroti.Ui.Offset(((this.anchor.dx + childSize.width) - size.width), ((this.anchor.dy + childSize.height) - size.height));
        return new global::Doroti.Ui.Offset(((overhang__1366.dx > 0.0) ? (this.anchor.dx - overhang__1366.dx) : this.anchor.dx), ((overhang__1366.dy > 0.0) ? (this.anchor.dy - overhang__1366.dy) : this.anchor.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.SingleChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (DesktopTextSelectionToolbarLayoutDelegate)(object)oldDelegate;
        return (!object.Equals(this.anchor, ((DesktopTextSelectionToolbarLayoutDelegate)__oldDelegate).anchor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


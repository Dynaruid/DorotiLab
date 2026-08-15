// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/tooltip_visibility.dart
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

internal class _TooltipVisibilityScope__tooltip_visibility : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual bool visible { get; private set; } = default!;

    internal _TooltipVisibilityScope__tooltip_visibility(global::Doroti.Generated.Framework.Widgets.Widget child, bool visible) : base(child: child)
    {
        this.visible = visible;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __old = (_TooltipVisibilityScope__tooltip_visibility)(object)oldWidget;
        return (((_TooltipVisibilityScope__tooltip_visibility)__old).visible != this.visible);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TooltipVisibility : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool visible { get; private set; } = default!;

    public TooltipVisibility(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool visible = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.visible = visible;
        this.child = child;
    }

    public static bool of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _TooltipVisibilityScope__tooltip_visibility? visibility__1620 = ((_TooltipVisibilityScope__tooltip_visibility?)(object?)context.dependOnInheritedWidgetOfExactType<_TooltipVisibilityScope__tooltip_visibility>());
        return (visibility__1620?.visible ?? true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _TooltipVisibilityScope__tooltip_visibility(visible: this.visible, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

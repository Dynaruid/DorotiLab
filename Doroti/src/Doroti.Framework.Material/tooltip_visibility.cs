// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/tooltip_visibility.dart
#pragma warning disable CS8600, CS8603
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

internal class _TooltipVisibilityScope__tooltip_visibility : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual bool visible { get; private set; } = default!;

    internal _TooltipVisibilityScope__tooltip_visibility(global::Doroti.Framework.Widgets.Widget child, bool visible) : base(child: child)
    {
        this.visible = visible;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __old = (_TooltipVisibilityScope__tooltip_visibility)(object)oldWidget;
        return (((_TooltipVisibilityScope__tooltip_visibility)__old).visible != this.visible);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TooltipVisibility : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual bool visible { get; private set; } = default!;

    public TooltipVisibility(global::Doroti.Framework.Foundation.Key? key = null, bool visible = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        this.visible = visible;
        this.child = child;
    }

    public static bool of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _TooltipVisibilityScope__tooltip_visibility? visibility = ((_TooltipVisibilityScope__tooltip_visibility?)(object?)context.dependOnInheritedWidgetOfExactType<_TooltipVisibilityScope__tooltip_visibility>());
        return (visibility?.visible ?? true);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _TooltipVisibilityScope__tooltip_visibility(visible: this.visible, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

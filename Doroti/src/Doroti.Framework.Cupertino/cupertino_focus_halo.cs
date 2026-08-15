// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/cupertino_focus_halo.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public class CupertinoFocusHalo : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry _borderRadius { get; private set; } = default!;
    internal virtual global::System.Func<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry, global::Doroti.Generated.Framework.Painting.BorderSide, global::Doroti.Generated.Framework.Painting.ShapeBorder> _shapeBuilder { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoFocusHalo(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
        this.child = child;
        this._borderRadius = global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        this._shapeBuilder = ((global::System.Func<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry, global::Doroti.Generated.Framework.Painting.BorderSide, global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder>)((borderRadius, side) => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(side: side, borderRadius: borderRadius)));
    }

    public static CupertinoFocusHalo CreateWithRRect(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry borderRadius, global::Doroti.Generated.Framework.Foundation.Key? key = null)
    {
        var __instance = new CupertinoFocusHalo(child: child, key: key);
        __instance.child = child;
        __instance._borderRadius = borderRadius;
        __instance._shapeBuilder = ((global::System.Func<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry, global::Doroti.Generated.Framework.Painting.BorderSide, global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder>)((borderRadius, side) => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(side: side, borderRadius: borderRadius)));
        return __instance;
    }

    public static CupertinoFocusHalo CreateWithRoundedSuperellipse(global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry borderRadius, global::Doroti.Generated.Framework.Foundation.Key? key = null)
    {
        var __instance = new CupertinoFocusHalo(child: child, key: key);
        __instance.child = child;
        __instance._borderRadius = borderRadius;
        __instance._shapeBuilder = ((global::System.Func<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry?, global::Doroti.Generated.Framework.Painting.BorderSide, global::Doroti.Generated.Framework.Painting.RoundedSuperellipseBorder>)((borderRadius, side) => new global::Doroti.Generated.Framework.Painting.RoundedSuperellipseBorder(side: side, borderRadius: borderRadius)));
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoFocusHaloState__cupertino_focus_halo());
}

internal class _CupertinoFocusHaloState__cupertino_focus_halo : global::Doroti.Generated.Framework.Widgets.State<CupertinoFocusHalo>
{
    internal virtual bool _childHasFocus { get; set; } = false;

    internal virtual global::Doroti.Ui.Color _effectiveFocusOutlineColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(global::Doroti.Generated.Framework.Painting.HSLColor.CreateFromColor(CupertinoColors.activeBlue.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor());
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Focus(canRequestFocus: false, skipTraversal: true, includeSemantics: false, onFocusChange: ((global::System.Action<bool>)((hasFocus) => {
setState(((global::System.Action)(() => {
_childHasFocus = hasFocus;
})));
})), child: new global::Doroti.Generated.Framework.Widgets.DecoratedBox(position: global::Doroti.Generated.Framework.Rendering.DecorationPosition.foreground, decoration: new global::Doroti.Generated.Framework.Painting.ShapeDecoration(shape: this.widget._shapeBuilder(((CupertinoFocusHalo)this.widget)._borderRadius, (this._childHasFocus ? new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._effectiveFocusOutlineColor, width: 3.5) : global::Doroti.Generated.Framework.Painting.BorderSide.none))), child: ((CupertinoFocusHalo)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

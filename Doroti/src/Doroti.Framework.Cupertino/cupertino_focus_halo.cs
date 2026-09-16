// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/cupertino_focus_halo.dart

using Doroti.Runtime;

namespace Doroti.Framework.Cupertino;

public class CupertinoFocusHalo : global::Doroti.Framework.Widgets.StatefulWidget
{
    internal virtual global::Doroti.Framework.Painting.BorderRadiusGeometry _borderRadius { get; private set; } = default!;
    internal virtual global::System.Func<global::Doroti.Framework.Painting.BorderRadiusGeometry, global::Doroti.Framework.Painting.BorderSide, global::Doroti.Framework.Painting.ShapeBorder> _shapeBuilder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    public CupertinoFocusHalo(global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Foundation.Key? key = null) : base(key: key)
    {
        this.child = child;
        _borderRadius = BorderRadius.zero;
        _shapeBuilder = (global::System.Func<global::Doroti.Framework.Painting.BorderRadiusGeometry, global::Doroti.Framework.Painting.BorderSide, global::Doroti.Framework.Painting.RoundedRectangleBorder>)((borderRadius, side) => new global::Doroti.Framework.Painting.RoundedRectangleBorder(side: side, borderRadius: borderRadius));
    }

    public static CupertinoFocusHalo CreateWithRRect(global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Painting.BorderRadiusGeometry borderRadius, global::Doroti.Framework.Foundation.Key? key = null)
    {
        var __instance = new CupertinoFocusHalo(child: child, key: key);
        __instance.child = child;
        __instance._borderRadius = borderRadius;
        __instance._shapeBuilder = (global::System.Func<global::Doroti.Framework.Painting.BorderRadiusGeometry, global::Doroti.Framework.Painting.BorderSide, global::Doroti.Framework.Painting.RoundedRectangleBorder>)((borderRadius, side) => new global::Doroti.Framework.Painting.RoundedRectangleBorder(side: side, borderRadius: borderRadius));
        return __instance;
    }

    public static CupertinoFocusHalo CreateWithRoundedSuperellipse(global::Doroti.Framework.Widgets.Widget child, global::Doroti.Framework.Painting.BorderRadiusGeometry borderRadius, global::Doroti.Framework.Foundation.Key? key = null)
    {
        var __instance = new CupertinoFocusHalo(child: child, key: key);
        __instance.child = child;
        __instance._borderRadius = borderRadius;
        __instance._shapeBuilder = (global::System.Func<global::Doroti.Framework.Painting.BorderRadiusGeometry?, global::Doroti.Framework.Painting.BorderSide, global::Doroti.Framework.Painting.RoundedSuperellipseBorder>)((borderRadius, side) => new global::Doroti.Framework.Painting.RoundedSuperellipseBorder(side: side, borderRadius: borderRadius));
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoFocusHaloState__cupertino_focus_halo());
}

internal class _CupertinoFocusHaloState__cupertino_focus_halo : global::Doroti.Framework.Widgets.State<CupertinoFocusHalo>
{
    internal virtual bool _childHasFocus { get; set; } = false;

    internal virtual global::Doroti.Ui.Color _effectiveFocusOutlineColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(HSLColor.CreateFromColor(CupertinoColors.activeBlue.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor());
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Focus(canRequestFocus: false, skipTraversal: true, includeSemantics: false, onFocusChange: (hasFocus) =>
        {
            setState(() =>
            {
                _childHasFocus = hasFocus;
            });
        }, child: new global::Doroti.Framework.Widgets.DecoratedBox(position: DecorationPosition.foreground, decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: widget._shapeBuilder(widget._borderRadius, _childHasFocus ? new global::Doroti.Framework.Painting.BorderSide(color: _effectiveFocusOutlineColor, width: 3.5) : BorderSide.none)), child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

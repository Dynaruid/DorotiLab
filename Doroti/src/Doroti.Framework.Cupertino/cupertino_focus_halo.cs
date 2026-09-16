// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/cupertino_focus_halo.dart

using Doroti.Runtime;

namespace Doroti.Framework.Cupertino;

public class CupertinoFocusHalo : StatefulWidget
{
    internal virtual BorderRadiusGeometry _borderRadius { get; private set; } = default!;
    internal virtual Func<BorderRadiusGeometry, BorderSide, ShapeBorder> _shapeBuilder { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    public CupertinoFocusHalo(Widget child, Key? key = null) : base(key: key)
    {
        this.child = child;
        _borderRadius = BorderRadius.zero;
        _shapeBuilder = (Func<BorderRadiusGeometry, BorderSide, RoundedRectangleBorder>)((borderRadius, side) => new RoundedRectangleBorder(side: side, borderRadius: borderRadius));
    }

    public static CupertinoFocusHalo CreateWithRRect(Widget child, BorderRadiusGeometry borderRadius, Key? key = null)
    {
        var __instance = new CupertinoFocusHalo(child: child, key: key);
        __instance.child = child;
        __instance._borderRadius = borderRadius;
        __instance._shapeBuilder = (Func<BorderRadiusGeometry, BorderSide, RoundedRectangleBorder>)((borderRadius, side) => new RoundedRectangleBorder(side: side, borderRadius: borderRadius));
        return __instance;
    }

    public static CupertinoFocusHalo CreateWithRoundedSuperellipse(Widget child, BorderRadiusGeometry borderRadius, Key? key = null)
    {
        var __instance = new CupertinoFocusHalo(child: child, key: key);
        __instance.child = child;
        __instance._borderRadius = borderRadius;
        __instance._shapeBuilder = (Func<BorderRadiusGeometry?, BorderSide, RoundedSuperellipseBorder>)((borderRadius, side) => new RoundedSuperellipseBorder(side: side, borderRadius: borderRadius));
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoFocusHaloState__cupertino_focus_halo());
}

internal class _CupertinoFocusHaloState__cupertino_focus_halo : State<CupertinoFocusHalo>
{
    internal virtual bool _childHasFocus { get; set; } = false;

    internal virtual Ui.Color _effectiveFocusOutlineColor => DartRuntimePrimitives.ConvertValue<Ui.Color>(HSLColor.CreateFromColor(CupertinoColors.activeBlue.withOpacity(ConstantsLibrary.kCupertinoFocusColorOpacity)).withLightness(ConstantsLibrary.kCupertinoFocusColorBrightness).withSaturation(ConstantsLibrary.kCupertinoFocusColorSaturation).toColor());
    public override Widget build(BuildContext context)
    {
        return new Focus(canRequestFocus: false, skipTraversal: true, includeSemantics: false, onFocusChange: (hasFocus) =>
        {
            setState(() =>
            {
                _childHasFocus = hasFocus;
            });
        }, child: new DecoratedBox(position: DecorationPosition.foreground, decoration: new ShapeDecoration(shape: widget._shapeBuilder(widget._borderRadius, _childHasFocus ? new BorderSide(color: _effectiveFocusOutlineColor, width: 3.5) : BorderSide.none)), child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

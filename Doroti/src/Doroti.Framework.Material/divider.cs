// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/divider.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class Divider : StatelessWidget
{
    public virtual double? height { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual BorderRadiusGeometry? radius { get; private set; }
    public virtual Color? color { get; private set; }

    public Divider(Key? key = null, double? height = null, double? thickness = null, double? indent = null, double? endIndent = null, Color? color = null, BorderRadiusGeometry? radius = null) : base(key: key)
    {
        this.height = height;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.color = color;
        this.radius = radius;
        System.Diagnostics.Debug.Assert((height is null) || (height >= 0.0));
        System.Diagnostics.Debug.Assert((thickness is null) || (thickness >= 0.0));
        System.Diagnostics.Debug.Assert((indent is null) || (indent >= 0.0));
        System.Diagnostics.Debug.Assert((endIndent is null) || (endIndent >= 0.0));
    }

    public static BorderSide createBorderSide(BuildContext? context, Color? color = null, double? width = null)
    {
        DividerThemeData? dividerTheme = (context is not null) ? DividerTheme.of(context) : null;
        DividerThemeData? defaults = (context is not null) ? new _DividerDefaultsM3__divider(context) : null;
        Color? effectiveColor = (color ?? dividerTheme?.color) ?? defaults?.color;
        double effectiveWidth = ((width ?? dividerTheme?.thickness) ?? defaults?.thickness) ?? 0.0;
        if (effectiveColor is null)
        {
            return new BorderSide(width: effectiveWidth);
        }
        return new BorderSide(color: effectiveColor, width: effectiveWidth);
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        DividerThemeData defaults = new _DividerDefaultsM3__divider(context);
        double heightLocal = (height ?? dividerTheme.space) ?? DartRuntimePrimitives.RequireValue(defaults.space);
        double thicknessLocal = (thickness ?? dividerTheme.thickness) ?? DartRuntimePrimitives.RequireValue(defaults.thickness);
        double indentLocal = (indent ?? dividerTheme.indent) ?? DartRuntimePrimitives.RequireValue(defaults.indent);
        double endIndentLocal = (endIndent ?? dividerTheme.endIndent) ?? DartRuntimePrimitives.RequireValue(defaults.endIndent);
        return new SizedBox(height: DartRuntimePrimitives.RequireValue(heightLocal), child: new Center(child: new Container(height: DartRuntimePrimitives.RequireValue(thicknessLocal), margin: EdgeInsetsDirectional.CreateOnly(start: DartRuntimePrimitives.RequireValue(indentLocal), end: DartRuntimePrimitives.RequireValue(endIndentLocal)), decoration: new BoxDecoration(borderRadius: (radius ?? dividerTheme.radius) ?? defaults.radius, border: new Border(bottom: createBorderSide(context, color: color, width: DartRuntimePrimitives.RequireValue(thicknessLocal)))))));
    }

}

public class VerticalDivider : StatelessWidget
{
    public virtual double? width { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual BorderRadiusGeometry? radius { get; private set; }

    public VerticalDivider(Key? key = null, double? width = null, double? thickness = null, double? indent = null, double? endIndent = null, Color? color = null, BorderRadiusGeometry? radius = null) : base(key: key)
    {
        this.width = width;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.color = color;
        this.radius = radius;
        System.Diagnostics.Debug.Assert((width is null) || (width >= 0.0));
        System.Diagnostics.Debug.Assert((thickness is null) || (thickness >= 0.0));
        System.Diagnostics.Debug.Assert((indent is null) || (indent >= 0.0));
        System.Diagnostics.Debug.Assert((endIndent is null) || (endIndent >= 0.0));
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        DividerThemeData defaults = new _DividerDefaultsM3__divider(context);
        double widthLocal = (width ?? dividerTheme.space) ?? DartRuntimePrimitives.RequireValue(defaults.space);
        double thicknessLocal = (thickness ?? dividerTheme.thickness) ?? DartRuntimePrimitives.RequireValue(defaults.thickness);
        double indentLocal = (indent ?? dividerTheme.indent) ?? DartRuntimePrimitives.RequireValue(defaults.indent);
        double endIndentLocal = (endIndent ?? dividerTheme.endIndent) ?? DartRuntimePrimitives.RequireValue(defaults.endIndent);
        return new SizedBox(width: DartRuntimePrimitives.RequireValue(widthLocal), child: new Center(child: new Container(width: DartRuntimePrimitives.RequireValue(thicknessLocal), margin: EdgeInsetsDirectional.CreateOnly(top: DartRuntimePrimitives.RequireValue(indentLocal), bottom: DartRuntimePrimitives.RequireValue(endIndentLocal)), decoration: new BoxDecoration(borderRadius: (radius ?? dividerTheme.radius) ?? defaults.radius, border: new Border(left: Divider.createBorderSide(context, color: color, width: DartRuntimePrimitives.RequireValue(thicknessLocal)))))));
    }

}

internal class _DividerDefaultsM3__divider : DividerThemeData
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _DividerDefaultsM3__divider(BuildContext context) : base(space: 16, thickness: 1.0, indent: 0, endIndent: 0)
    {
        this.context = context;
    }

    public override Color? color => DartRuntimePrimitives.ConvertValue<Color>(Theme.of(context).colorScheme.outlineVariant);
}

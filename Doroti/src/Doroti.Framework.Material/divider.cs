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

    public Divider(
        Key? key = null,
        double? height = null,
        double? thickness = null,
        double? indent = null,
        double? endIndent = null,
        Color? color = null,
        BorderRadiusGeometry? radius = null
    )
        : base(key: key)
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

    public static BorderSide createBorderSide(
        BuildContext? context,
        Color? color = null,
        double? width = null
    )
    {
        DividerThemeData? dividerTheme = (context is not null) ? DividerTheme.of(context) : null;
        DividerThemeData? defaults =
            (context is not null) ? new _DividerDefaultsM3__divider(context) : null;
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
        double heightLocal =
            (height ?? dividerTheme.space)
            ?? (
                defaults.space
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double thicknessLocal =
            (thickness ?? dividerTheme.thickness)
            ?? (
                defaults.thickness
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double indentLocal =
            (indent ?? dividerTheme.indent)
            ?? (
                defaults.indent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double endIndentLocal =
            (endIndent ?? dividerTheme.endIndent)
            ?? (
                defaults.endIndent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        return new SizedBox(
            height: (heightLocal),
            child: new Center(
                child: new Container(
                    height: (thicknessLocal),
                    margin: EdgeInsetsDirectional.CreateOnly(
                        start: (indentLocal),
                        end: (endIndentLocal)
                    ),
                    decoration: new BoxDecoration(
                        borderRadius: (radius ?? dividerTheme.radius) ?? defaults.radius,
                        border: new Border(
                            bottom: createBorderSide(context, color: color, width: (thicknessLocal))
                        )
                    )
                )
            )
        );
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

    public VerticalDivider(
        Key? key = null,
        double? width = null,
        double? thickness = null,
        double? indent = null,
        double? endIndent = null,
        Color? color = null,
        BorderRadiusGeometry? radius = null
    )
        : base(key: key)
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
        double widthLocal =
            (width ?? dividerTheme.space)
            ?? (
                defaults.space
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double thicknessLocal =
            (thickness ?? dividerTheme.thickness)
            ?? (
                defaults.thickness
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double indentLocal =
            (indent ?? dividerTheme.indent)
            ?? (
                defaults.indent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        double endIndentLocal =
            (endIndent ?? dividerTheme.endIndent)
            ?? (
                defaults.endIndent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        return new SizedBox(
            width: (widthLocal),
            child: new Center(
                child: new Container(
                    width: (thicknessLocal),
                    margin: EdgeInsetsDirectional.CreateOnly(
                        top: (indentLocal),
                        bottom: (endIndentLocal)
                    ),
                    decoration: new BoxDecoration(
                        borderRadius: (radius ?? dividerTheme.radius) ?? defaults.radius,
                        border: new Border(
                            left: Divider.createBorderSide(
                                context,
                                color: color,
                                width: (thicknessLocal)
                            )
                        )
                    )
                )
            )
        );
    }
}

internal class _DividerDefaultsM3__divider : DividerThemeData
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _DividerDefaultsM3__divider(BuildContext context)
        : base(space: 16, thickness: 1.0, indent: 0, endIndent: 0)
    {
        this.context = context;
    }

    public override Color? color =>
        DartRuntimePrimitives.ConvertValue<Color>(Theme.of(context).colorScheme.outlineVariant);
}

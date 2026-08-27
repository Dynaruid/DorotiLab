// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/divider.dart
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

public class Divider : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual double? height { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadiusGeometry? radius { get; private set; }
    public virtual Color? color { get; private set; }

    public Divider(global::Doroti.Framework.Foundation.Key? key = null, double? height = null, double? thickness = null, double? indent = null, double? endIndent = null, Color? color = null, global::Doroti.Framework.Painting.BorderRadiusGeometry? radius = null) : base(key: key)
    {
        this.height = height;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.color = color;
        this.radius = radius;
        System.Diagnostics.Debug.Assert(((height is null) || (height >= 0.0)));
        System.Diagnostics.Debug.Assert(((thickness is null) || (thickness >= 0.0)));
        System.Diagnostics.Debug.Assert(((indent is null) || (indent >= 0.0)));
        System.Diagnostics.Debug.Assert(((endIndent is null) || (endIndent >= 0.0)));
    }

    public static global::Doroti.Framework.Painting.BorderSide createBorderSide(global::Doroti.Framework.Widgets.BuildContext? context, Color? color = null, double? width = null)
    {
        DividerThemeData? dividerTheme = ((context is not null) ? DividerTheme.of(context) : null);
        DividerThemeData? defaults = ((context is not null) ? (Theme.of(context).useMaterial3 ? new _DividerDefaultsM3__divider(context) : new _DividerDefaultsM2__divider(context)) : null);
        global::Doroti.Ui.Color? effectiveColor = ((global::Doroti.Ui.Color?)(object?)((color ?? dividerTheme?.color) ?? defaults?.color));
        double effectiveWidth = (((width ?? dividerTheme?.thickness) ?? defaults?.thickness) ?? 0.0);
        if ((effectiveColor is null))
        {
            return new global::Doroti.Framework.Painting.BorderSide(width: effectiveWidth);
        }
        return new global::Doroti.Framework.Painting.BorderSide(color: effectiveColor, width: effectiveWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        DividerThemeData defaults = (theme.useMaterial3 ? new _DividerDefaultsM3__divider(context) : new _DividerDefaultsM2__divider(context));
        double heightLocal = ((this.height ?? dividerTheme.space) ?? DartRuntimePrimitives.RequireValue(defaults.space));
        double thicknessLocal = ((this.thickness ?? dividerTheme.thickness) ?? DartRuntimePrimitives.RequireValue(defaults.thickness));
        double indentLocal = ((this.indent ?? dividerTheme.indent) ?? DartRuntimePrimitives.RequireValue(defaults.indent));
        double endIndentLocal = ((this.endIndent ?? dividerTheme.endIndent) ?? DartRuntimePrimitives.RequireValue(defaults.endIndent));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: DartRuntimePrimitives.RequireValue(heightLocal), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Container(height: DartRuntimePrimitives.RequireValue(thicknessLocal), margin: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: DartRuntimePrimitives.RequireValue(indentLocal), end: DartRuntimePrimitives.RequireValue(endIndentLocal)), decoration: new global::Doroti.Framework.Painting.BoxDecoration(borderRadius: ((this.radius ?? dividerTheme.radius) ?? defaults.radius), border: new global::Doroti.Framework.Painting.Border(bottom: Divider.createBorderSide(context, color: this.color, width: DartRuntimePrimitives.RequireValue(thicknessLocal))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class VerticalDivider : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual double? width { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadiusGeometry? radius { get; private set; }

    public VerticalDivider(global::Doroti.Framework.Foundation.Key? key = null, double? width = null, double? thickness = null, double? indent = null, double? endIndent = null, Color? color = null, global::Doroti.Framework.Painting.BorderRadiusGeometry? radius = null) : base(key: key)
    {
        this.width = width;
        this.thickness = thickness;
        this.indent = indent;
        this.endIndent = endIndent;
        this.color = color;
        this.radius = radius;
        System.Diagnostics.Debug.Assert(((width is null) || (width >= 0.0)));
        System.Diagnostics.Debug.Assert(((thickness is null) || (thickness >= 0.0)));
        System.Diagnostics.Debug.Assert(((indent is null) || (indent >= 0.0)));
        System.Diagnostics.Debug.Assert(((endIndent is null) || (endIndent >= 0.0)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        DividerThemeData dividerTheme = DividerTheme.of(context);
        DividerThemeData defaults = (theme.useMaterial3 ? new _DividerDefaultsM3__divider(context) : new _DividerDefaultsM2__divider(context));
        double widthLocal = ((this.width ?? dividerTheme.space) ?? DartRuntimePrimitives.RequireValue(defaults.space));
        double thicknessLocal = ((this.thickness ?? dividerTheme.thickness) ?? DartRuntimePrimitives.RequireValue(defaults.thickness));
        double indentLocal = ((this.indent ?? dividerTheme.indent) ?? DartRuntimePrimitives.RequireValue(defaults.indent));
        double endIndentLocal = ((this.endIndent ?? dividerTheme.endIndent) ?? DartRuntimePrimitives.RequireValue(defaults.endIndent));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: DartRuntimePrimitives.RequireValue(widthLocal), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Container(width: DartRuntimePrimitives.RequireValue(thicknessLocal), margin: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(top: DartRuntimePrimitives.RequireValue(indentLocal), bottom: DartRuntimePrimitives.RequireValue(endIndentLocal)), decoration: new global::Doroti.Framework.Painting.BoxDecoration(borderRadius: ((this.radius ?? dividerTheme.radius) ?? defaults.radius), border: new global::Doroti.Framework.Painting.Border(left: Divider.createBorderSide(context, color: this.color, width: DartRuntimePrimitives.RequireValue(thicknessLocal))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DividerDefaultsM2__divider : DividerThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DividerDefaultsM2__divider(global::Doroti.Framework.Widgets.BuildContext context) : base(space: 16, thickness: 0, indent: 0, endIndent: 0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).dividerColor);
}

internal class _DividerDefaultsM3__divider : DividerThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DividerDefaultsM3__divider(global::Doroti.Framework.Widgets.BuildContext context) : base(space: 16, thickness: 1.0, indent: 0, endIndent: 0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Theme.of(this.context).colorScheme.outlineVariant);
}

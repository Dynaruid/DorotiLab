// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/divider.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class Divider : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual double? height { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius { get; private set; }
    public virtual Color? color { get; private set; }

    public Divider(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? height = null, double? thickness = null, double? indent = null, double? endIndent = null, Color? color = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius = null) : base(key: key)
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

    public static global::Doroti.Generated.Framework.Painting.BorderSide createBorderSide(global::Doroti.Generated.Framework.Widgets.BuildContext? context, Color? color = null, double? width = null)
    {
        DividerThemeData? dividerTheme__5875 = ((context is not null) ? DividerTheme.of(context) : null);
        DividerThemeData? defaults__5969 = ((context is not null) ? (Theme.of(context).useMaterial3 ? new _DividerDefaultsM3__divider(context) : new _DividerDefaultsM2__divider(context)) : null);
        global::Doroti.Flutter.Ui.Color? effectiveColor__6158 = ((global::Doroti.Flutter.Ui.Color?)(object?)((color ?? dividerTheme__5875?.color) ?? defaults__5969?.color));
        double effectiveWidth__6241 = (((width ?? dividerTheme__5875?.thickness) ?? defaults__5969?.thickness) ?? 0.0);
        if ((effectiveColor__6158 is null))
        {
            return new global::Doroti.Generated.Framework.Painting.BorderSide(width: effectiveWidth__6241);
        }
        return new global::Doroti.Generated.Framework.Painting.BorderSide(color: effectiveColor__6158, width: effectiveWidth__6241);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__6657 = Theme.of(context);
        DividerThemeData dividerTheme__6711 = DividerTheme.of(context);
        DividerThemeData defaults__6779 = (theme__6657.useMaterial3 ? new _DividerDefaultsM3__divider(context) : new _DividerDefaultsM2__divider(context));
        double height__6903 = ((this.height ?? dividerTheme__6711.space) ?? DartRuntimePrimitives.RequireValue(defaults__6779.space));
        double thickness__6983 = ((this.thickness ?? dividerTheme__6711.thickness) ?? DartRuntimePrimitives.RequireValue(defaults__6779.thickness));
        double indent__7077 = ((this.indent ?? dividerTheme__6711.indent) ?? DartRuntimePrimitives.RequireValue(defaults__6779.indent));
        double endIndent__7159 = ((this.endIndent ?? dividerTheme__6711.endIndent) ?? DartRuntimePrimitives.RequireValue(defaults__6779.endIndent));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(height: DartRuntimePrimitives.RequireValue(height__6903), child: new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Container(height: DartRuntimePrimitives.RequireValue(thickness__6983), margin: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: DartRuntimePrimitives.RequireValue(indent__7077), end: DartRuntimePrimitives.RequireValue(endIndent__7159)), decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(borderRadius: ((this.radius ?? dividerTheme__6711.radius) ?? defaults__6779.radius), border: new global::Doroti.Generated.Framework.Painting.Border(bottom: Divider.createBorderSide(context, color: this.color, width: DartRuntimePrimitives.RequireValue(thickness__6983))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class VerticalDivider : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual double? width { get; private set; }
    public virtual double? thickness { get; private set; }
    public virtual double? indent { get; private set; }
    public virtual double? endIndent { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius { get; private set; }

    public VerticalDivider(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? width = null, double? thickness = null, double? indent = null, double? endIndent = null, Color? color = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? radius = null) : base(key: key)
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__11213 = Theme.of(context);
        DividerThemeData dividerTheme__11267 = DividerTheme.of(context);
        DividerThemeData defaults__11335 = (theme__11213.useMaterial3 ? new _DividerDefaultsM3__divider(context) : new _DividerDefaultsM2__divider(context));
        double width__11459 = ((this.width ?? dividerTheme__11267.space) ?? DartRuntimePrimitives.RequireValue(defaults__11335.space));
        double thickness__11537 = ((this.thickness ?? dividerTheme__11267.thickness) ?? DartRuntimePrimitives.RequireValue(defaults__11335.thickness));
        double indent__11631 = ((this.indent ?? dividerTheme__11267.indent) ?? DartRuntimePrimitives.RequireValue(defaults__11335.indent));
        double endIndent__11713 = ((this.endIndent ?? dividerTheme__11267.endIndent) ?? DartRuntimePrimitives.RequireValue(defaults__11335.endIndent));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: DartRuntimePrimitives.RequireValue(width__11459), child: new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Container(width: DartRuntimePrimitives.RequireValue(thickness__11537), margin: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(top: DartRuntimePrimitives.RequireValue(indent__11631), bottom: DartRuntimePrimitives.RequireValue(endIndent__11713)), decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(borderRadius: ((this.radius ?? dividerTheme__11267.radius) ?? defaults__11335.radius), border: new global::Doroti.Generated.Framework.Painting.Border(left: Divider.createBorderSide(context, color: this.color, width: DartRuntimePrimitives.RequireValue(thickness__11537))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DividerDefaultsM2__divider : DividerThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DividerDefaultsM2__divider(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(space: 16, thickness: 0, indent: 0, endIndent: 0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Flutter.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Theme.of(this.context).dividerColor);
}

internal class _DividerDefaultsM3__divider : DividerThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DividerDefaultsM3__divider(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(space: 16, thickness: 1.0, indent: 0, endIndent: 0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Flutter.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Color>(Theme.of(this.context).colorScheme.outlineVariant);
}

// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/banner_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class MaterialBannerThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? leadingPadding { get; private set; }

    public MaterialBannerThemeData(Color? backgroundColor = null, Color? surfaceTintColor = null, Color? shadowColor = null, Color? dividerColor = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? leadingPadding = null)
    {
        this.backgroundColor = backgroundColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        this.dividerColor = dividerColor;
        this.contentTextStyle = contentTextStyle;
        this.elevation = elevation;
        this.padding = padding;
        this.leadingPadding = leadingPadding;
    }

    public virtual MaterialBannerThemeData copyWith(Color? backgroundColor = null, Color? surfaceTintColor = null, Color? shadowColor = null, Color? dividerColor = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? leadingPadding = null)
    {
        return new MaterialBannerThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shadowColor: (shadowColor ?? this.shadowColor), dividerColor: (dividerColor ?? this.dividerColor), contentTextStyle: (contentTextStyle ?? this.contentTextStyle), elevation: (elevation ?? this.elevation), padding: (padding ?? this.padding), leadingPadding: (leadingPadding ?? this.leadingPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MaterialBannerThemeData lerp(MaterialBannerThemeData? a, MaterialBannerThemeData? b, double t)
    {
        return new MaterialBannerThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), dividerColor: Dart_uiLibrary.Color.lerp(a?.dividerColor, b?.dividerColor, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), leadingPadding: EdgeInsetsGeometry.lerp(a?.leadingPadding, b?.leadingPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.surfaceTintColor, this.shadowColor, this.dividerColor, this.contentTextStyle, this.elevation, this.padding, this.leadingPadding));
    public override bool Equals(object? other)
    {
        var __other = other as MaterialBannerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is MaterialBannerThemeData) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).dividerColor, this.dividerColor))) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).contentTextStyle, this.contentTextStyle))) && (((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).elevation == this.elevation)) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).padding, this.padding))) && (object.Equals(((MaterialBannerThemeData)((MaterialBannerThemeData)__other)).leadingPadding, this.leadingPadding)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dividerColor", this.dividerColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("contentTextStyle", this.contentTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("leadingPadding", this.leadingPadding, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialBannerTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual MaterialBannerThemeData? data { get; private set; }

    public MaterialBannerTheme(global::Doroti.Framework.Foundation.Key? key = null, MaterialBannerThemeData? data = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MaterialBannerThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MaterialBannerTheme? bannerTheme__6819 = ((MaterialBannerTheme?)(object?)context.dependOnInheritedWidgetOfExactType<MaterialBannerTheme>());
        return (bannerTheme__6819?.data ?? Theme.of(context).bannerTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new MaterialBannerTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((MaterialBannerTheme)oldWidget).data)));
}

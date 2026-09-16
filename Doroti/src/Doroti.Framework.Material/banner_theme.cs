// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/banner_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class MaterialBannerThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    public virtual TextStyle? contentTextStyle { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual EdgeInsetsGeometry? leadingPadding { get; private set; }

    public MaterialBannerThemeData(Color? backgroundColor = null, Color? surfaceTintColor = null, Color? shadowColor = null, Color? dividerColor = null, TextStyle? contentTextStyle = null, double? elevation = null, EdgeInsetsGeometry? padding = null, EdgeInsetsGeometry? leadingPadding = null)
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

    public virtual MaterialBannerThemeData copyWith(Color? backgroundColor = null, Color? surfaceTintColor = null, Color? shadowColor = null, Color? dividerColor = null, TextStyle? contentTextStyle = null, double? elevation = null, EdgeInsetsGeometry? padding = null, EdgeInsetsGeometry? leadingPadding = null)
    {
        return new MaterialBannerThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, shadowColor: shadowColor ?? this.shadowColor, dividerColor: dividerColor ?? this.dividerColor, contentTextStyle: contentTextStyle ?? this.contentTextStyle, elevation: elevation ?? this.elevation, padding: padding ?? this.padding, leadingPadding: leadingPadding ?? this.leadingPadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static MaterialBannerThemeData lerp(MaterialBannerThemeData? a, MaterialBannerThemeData? b, double t)
    {
        return new MaterialBannerThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), dividerColor: Dart_uiLibrary.Color.lerp(a?.dividerColor, b?.dividerColor, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), leadingPadding: EdgeInsetsGeometry.lerp(a?.leadingPadding, b?.leadingPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, surfaceTintColor, shadowColor, dividerColor, contentTextStyle, elevation, padding, leadingPadding));
    public override bool Equals(object? other)
    {
        var __other = other as MaterialBannerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is MaterialBannerThemeData) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.shadowColor, shadowColor) && Equals(__other.dividerColor, dividerColor) && Equals(__other.contentTextStyle, contentTextStyle) && (__other.elevation == elevation) && Equals(__other.padding, padding) && Equals(__other.leadingPadding, leadingPadding);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("dividerColor", dividerColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle>("contentTextStyle", contentTextStyle, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("leadingPadding", leadingPadding, defaultValue: null));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialBannerTheme : InheritedTheme
{
    public virtual MaterialBannerThemeData? data { get; private set; }

    public MaterialBannerTheme(Key? key = null, MaterialBannerThemeData? data = null, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static MaterialBannerThemeData of(BuildContext context)
    {
        MaterialBannerTheme? bannerThemeLocal = context.dependOnInheritedWidgetOfExactType<MaterialBannerTheme>();
        return bannerThemeLocal?.data ?? Theme.of(context).bannerTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new MaterialBannerTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((MaterialBannerTheme)oldWidget).data));
}

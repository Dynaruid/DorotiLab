// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SearchBarThemeData : Diagnosticable
{
    public virtual WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual WidgetStateProperty<BorderSide?>? side { get; private set; }
    public virtual WidgetStateProperty<OutlinedBorder?>? shape { get; private set; }
    public virtual WidgetStateProperty<EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? textStyle { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? hintStyle { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual TextCapitalization? textCapitalization { get; private set; }

    public SearchBarThemeData(WidgetStateProperty<double?>? elevation = null, WidgetStateProperty<Color?>? backgroundColor = null, WidgetStateProperty<Color?>? shadowColor = null, WidgetStateProperty<Color?>? surfaceTintColor = null, WidgetStateProperty<Color?>? overlayColor = null, WidgetStateProperty<BorderSide?>? side = null, WidgetStateProperty<OutlinedBorder?>? shape = null, WidgetStateProperty<EdgeInsetsGeometry?>? padding = null, WidgetStateProperty<TextStyle?>? textStyle = null, WidgetStateProperty<TextStyle?>? hintStyle = null, BoxConstraints? constraints = null, TextCapitalization? textCapitalization = null)
    {
        this.elevation = elevation;
        this.backgroundColor = backgroundColor;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.overlayColor = overlayColor;
        this.side = side;
        this.shape = shape;
        this.padding = padding;
        this.textStyle = textStyle;
        this.hintStyle = hintStyle;
        this.constraints = constraints;
        this.textCapitalization = textCapitalization;
    }

    public virtual SearchBarThemeData copyWith(WidgetStateProperty<double?>? elevation = null, WidgetStateProperty<Color?>? backgroundColor = null, WidgetStateProperty<Color?>? shadowColor = null, WidgetStateProperty<Color?>? surfaceTintColor = null, WidgetStateProperty<Color?>? overlayColor = null, WidgetStateProperty<BorderSide?>? side = null, WidgetStateProperty<OutlinedBorder?>? shape = null, WidgetStateProperty<EdgeInsetsGeometry?>? padding = null, WidgetStateProperty<TextStyle?>? textStyle = null, WidgetStateProperty<TextStyle?>? hintStyle = null, BoxConstraints? constraints = null, TextCapitalization? textCapitalization = null)
    {
        return new SearchBarThemeData(elevation: elevation ?? this.elevation, backgroundColor: backgroundColor ?? this.backgroundColor, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, overlayColor: overlayColor ?? this.overlayColor, side: side ?? this.side, shape: shape ?? this.shape, padding: padding ?? this.padding, textStyle: textStyle ?? this.textStyle, hintStyle: hintStyle ?? this.hintStyle, constraints: constraints ?? this.constraints, textCapitalization: textCapitalization ?? this.textCapitalization);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SearchBarThemeData? lerp(SearchBarThemeData? a, SearchBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new SearchBarThemeData(elevation: WidgetStateProperty.lerp(a?.elevation, b?.elevation, t, Dart_uiLibrary.lerpDouble), backgroundColor: WidgetStateProperty.lerp(a?.backgroundColor, b?.backgroundColor, t, Color.lerp), shadowColor: WidgetStateProperty.lerp(a?.shadowColor, b?.shadowColor, t, Color.lerp), surfaceTintColor: WidgetStateProperty.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t, Color.lerp), overlayColor: WidgetStateProperty.lerp(a?.overlayColor, b?.overlayColor, t, Color.lerp), side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp(a?.shape, b?.shape, t, OutlinedBorder.lerp), padding: WidgetStateProperty.lerp(a?.padding, b?.padding, t, EdgeInsetsGeometry.lerp), textStyle: WidgetStateProperty.lerp(a?.textStyle, b?.textStyle, t, TextStyle.lerp), hintStyle: WidgetStateProperty.lerp(a?.hintStyle, b?.hintStyle, t, TextStyle.lerp), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), textCapitalization: (t < 0.5) ? a?.textCapitalization : b?.textCapitalization);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(elevation, backgroundColor, shadowColor, surfaceTintColor, overlayColor, side, shape, padding, textStyle, hintStyle, constraints, textCapitalization));
    public override bool Equals(object? other)
    {
        var __other = other as SearchBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SearchBarThemeData) && Equals(__other.elevation, elevation) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.shadowColor, shadowColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.overlayColor, overlayColor) && Equals(__other.side, side) && Equals(__other.shape, shape) && Equals(__other.padding, padding) && Equals(__other.textStyle, textStyle) && Equals(__other.hintStyle, hintStyle) && Equals(__other.constraints, constraints) && Equals(__other.textCapitalization, textCapitalization);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<WidgetStateProperty<double?>>("elevation", elevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("shadowColor", shadowColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<Color?>>("overlayColor", overlayColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<BorderSide?>>("side", side, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<OutlinedBorder?>>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<EdgeInsetsGeometry?>>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<TextStyle?>>("textStyle", textStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<WidgetStateProperty<TextStyle?>>("hintStyle", hintStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextCapitalization>("textCapitalization", textCapitalization, defaultValue: null));
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

public class SearchBarTheme : InheritedWidget
{
    public virtual SearchBarThemeData data { get; private set; } = default!;

    public SearchBarTheme(Key? key = null, SearchBarThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SearchBarThemeData of(BuildContext context)
    {
        SearchBarTheme? searchBarThemeLocal = context.dependOnInheritedWidgetOfExactType<SearchBarTheme>();
        return searchBarThemeLocal?.data ?? Theme.of(context).searchBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SearchBarTheme)oldWidget).data));
}

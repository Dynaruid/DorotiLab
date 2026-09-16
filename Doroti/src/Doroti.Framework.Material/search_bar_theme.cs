// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SearchBarThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? hintStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Framework.Services.TextCapitalization? textCapitalization { get; private set; }

    public SearchBarThemeData(global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? hintStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null)
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

    public virtual SearchBarThemeData copyWith(global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? hintStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Services.TextCapitalization? textCapitalization = null)
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
        return new SearchBarThemeData(elevation: WidgetStateProperty.lerp<double?>(a?.elevation, b?.elevation, t, Dart_uiLibrary.lerpDouble), backgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.backgroundColor, b?.backgroundColor, t, Color.lerp), shadowColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.shadowColor, b?.shadowColor, t, Color.lerp), surfaceTintColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.surfaceTintColor, b?.surfaceTintColor, t, Color.lerp), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, Color.lerp), side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.OutlinedBorder?>(a?.shape, b?.shape, t, OutlinedBorder.lerp), padding: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>(a?.padding, b?.padding, t, EdgeInsetsGeometry.lerp), textStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.textStyle, b?.textStyle, t, TextStyle.lerp), hintStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.hintStyle, b?.hintStyle, t, TextStyle.lerp), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), textCapitalization: (t < 0.5) ? a?.textCapitalization : b?.textCapitalization);
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

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("shadowColor", shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>>("side", side, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("textStyle", textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("hintStyle", hintStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Services.TextCapitalization>("textCapitalization", textCapitalization, defaultValue: null));
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

public class SearchBarTheme : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual SearchBarThemeData data { get; private set; } = default!;

    public SearchBarTheme(global::Doroti.Framework.Foundation.Key? key = null, SearchBarThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SearchBarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SearchBarTheme? searchBarThemeLocal = context.dependOnInheritedWidgetOfExactType<SearchBarTheme>();
        return searchBarThemeLocal?.data ?? Theme.of(context).searchBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SearchBarTheme)oldWidget).data));
}

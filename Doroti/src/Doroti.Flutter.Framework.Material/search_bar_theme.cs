// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/search_bar_theme.dart
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

public class SearchBarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? textStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? hintStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.TextCapitalization? textCapitalization { get; private set; }

    public SearchBarThemeData(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? hintStyle = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Generated.Framework.Services.TextCapitalization? textCapitalization = null)
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

    public virtual SearchBarThemeData copyWith(global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>? elevation = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? shadowColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? surfaceTintColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? shape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>? padding = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? textStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? hintStyle = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Generated.Framework.Services.TextCapitalization? textCapitalization = null)
    {
        return new SearchBarThemeData(elevation: (elevation ?? this.elevation), backgroundColor: (backgroundColor ?? this.backgroundColor), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), overlayColor: (overlayColor ?? this.overlayColor), side: (side ?? this.side), shape: (shape ?? this.shape), padding: (padding ?? this.padding), textStyle: (textStyle ?? this.textStyle), hintStyle: (hintStyle ?? this.hintStyle), constraints: (constraints ?? this.constraints), textCapitalization: (textCapitalization ?? this.textCapitalization));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SearchBarThemeData? lerp(SearchBarThemeData? a, SearchBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new SearchBarThemeData(elevation: WidgetStateProperty.lerp<double?>(a?.elevation, b?.elevation, t, (global::System.Func<double?, double?, double, double?>)Dart_uiLibrary.lerpDouble), backgroundColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.backgroundColor, b?.backgroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), shadowColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.shadowColor, b?.shadowColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), surfaceTintColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.surfaceTintColor, b?.surfaceTintColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), overlayColor: WidgetStateProperty.lerp<global::Doroti.Flutter.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), side: WidgetStateBorderSide.lerp(a?.side, b?.side, t), shape: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(a?.shape, b?.shape, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.OutlinedBorder?, global::Doroti.Generated.Framework.Painting.OutlinedBorder?, double, global::Doroti.Generated.Framework.Painting.OutlinedBorder?>)global::Doroti.Generated.Framework.Painting.OutlinedBorder.lerp), padding: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>(a?.padding, b?.padding, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?, double, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>)global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry.lerp), textStyle: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.TextStyle?>(a?.textStyle, b?.textStyle, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.TextStyle?, global::Doroti.Generated.Framework.Painting.TextStyle?, double, global::Doroti.Generated.Framework.Painting.TextStyle?>)global::Doroti.Generated.Framework.Painting.TextStyle.lerp), hintStyle: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.TextStyle?>(a?.hintStyle, b?.hintStyle, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.TextStyle?, global::Doroti.Generated.Framework.Painting.TextStyle?, double, global::Doroti.Generated.Framework.Painting.TextStyle?>)global::Doroti.Generated.Framework.Painting.TextStyle.lerp), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), textCapitalization: ((t < 0.5) ? a?.textCapitalization : b?.textCapitalization));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.elevation, this.backgroundColor, this.shadowColor, this.surfaceTintColor, this.overlayColor, this.side, this.shape, this.padding, this.textStyle, this.hintStyle, this.constraints, this.textCapitalization));
    public override bool Equals(object? other)
    {
        var __other = other as SearchBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((__other is SearchBarThemeData) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).elevation, this.elevation))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).overlayColor, this.overlayColor))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).side, this.side))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).shape, this.shape))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).padding, this.padding))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).textStyle, this.textStyle))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).hintStyle, this.hintStyle))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).constraints, this.constraints))) && (object.Equals(((SearchBarThemeData)((SearchBarThemeData)__other)).textCapitalization, this.textCapitalization)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double?>>("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>>("side", this.side, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?>>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>("textStyle", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>("hintStyle", this.hintStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Services.TextCapitalization>("textCapitalization", this.textCapitalization, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class SearchBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual SearchBarThemeData data { get; private set; } = default!;

    public SearchBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, SearchBarThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SearchBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        SearchBarTheme? searchBarTheme__10295 = ((SearchBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<SearchBarTheme>());
        return (searchBarTheme__10295?.data ?? Theme.of(context).searchBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((SearchBarTheme)oldWidget).data)));
}

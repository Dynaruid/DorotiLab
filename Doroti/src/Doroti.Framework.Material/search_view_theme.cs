// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search_view_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SearchViewThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual double? headerHeight { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? headerTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? headerHintStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? barPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual Color? dividerColor { get; private set; }

    public SearchViewThemeData(Color? backgroundColor = null, double? elevation = null, Color? surfaceTintColor = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? barPadding = null, bool? shrinkWrap = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, double? headerHeight = null, global::Doroti.Framework.Painting.TextStyle? headerTextStyle = null, global::Doroti.Framework.Painting.TextStyle? headerHintStyle = null, Color? dividerColor = null)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.surfaceTintColor = surfaceTintColor;
        this.constraints = constraints;
        this.padding = padding;
        this.barPadding = barPadding;
        this.shrinkWrap = shrinkWrap;
        this.side = side;
        this.shape = shape;
        this.headerHeight = headerHeight;
        this.headerTextStyle = headerTextStyle;
        this.headerHintStyle = headerHintStyle;
        this.dividerColor = dividerColor;
    }

    public virtual SearchViewThemeData copyWith(Color? backgroundColor = null, double? elevation = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, double? headerHeight = null, global::Doroti.Framework.Painting.TextStyle? headerTextStyle = null, global::Doroti.Framework.Painting.TextStyle? headerHintStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? barPadding = null, bool? shrinkWrap = null, Color? dividerColor = null)
    {
        return new SearchViewThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, elevation: elevation ?? this.elevation, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, side: side ?? this.side, shape: shape ?? this.shape, headerHeight: headerHeight ?? this.headerHeight, headerTextStyle: headerTextStyle ?? this.headerTextStyle, headerHintStyle: headerHintStyle ?? this.headerHintStyle, constraints: constraints ?? this.constraints, padding: padding ?? this.padding, barPadding: barPadding ?? this.barPadding, shrinkWrap: shrinkWrap ?? this.shrinkWrap, dividerColor: dividerColor ?? this.dividerColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SearchViewThemeData? lerp(SearchViewThemeData? a, SearchViewThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new SearchViewThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), side: _lerpSides(a?.side, b?.side, t), shape: OutlinedBorder.lerp(a?.shape, b?.shape, t), headerHeight: Dart_uiLibrary.lerpDouble(a?.headerHeight, b?.headerHeight, t), headerTextStyle: TextStyle.lerp(a?.headerTextStyle, b?.headerTextStyle, t), headerHintStyle: TextStyle.lerp(a?.headerTextStyle, b?.headerTextStyle, t), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), barPadding: EdgeInsetsGeometry.lerp(a?.barPadding, b?.barPadding, t), shrinkWrap: (t < 0.5) ? a?.shrinkWrap : b?.shrinkWrap, dividerColor: Dart_uiLibrary.Color.lerp(a?.dividerColor, b?.dividerColor, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, elevation, surfaceTintColor, side, shape, headerHeight, headerTextStyle, headerHintStyle, constraints, padding, barPadding, shrinkWrap, dividerColor));
    public override bool Equals(object? other)
    {
        var __other = other as SearchViewThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SearchViewThemeData) && Equals(__other.backgroundColor, backgroundColor) && (__other.elevation == elevation) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.side, side) && Equals(__other.shape, shape) && (__other.headerHeight == headerHeight) && Equals(__other.headerTextStyle, headerTextStyle) && Equals(__other.headerHintStyle, headerHintStyle) && Equals(__other.constraints, constraints) && Equals(__other.padding, padding) && Equals(__other.barPadding, barPadding) && (__other.shrinkWrap == shrinkWrap) && Equals(__other.dividerColor, dividerColor);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double?>("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide?>("side", side, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.OutlinedBorder?>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double?>("headerHeight", headerHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle?>("headerTextStyle", headerTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle?>("headerHintStyle", headerHintStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>("barPadding", barPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool?>("shrinkWrap", shrinkWrap, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Color?>("dividerColor", dividerColor, defaultValue: null));
    }

    internal static global::Doroti.Framework.Painting.BorderSide? _lerpSides(global::Doroti.Framework.Painting.BorderSide? a, global::Doroti.Framework.Painting.BorderSide? b, double t)
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        if (a is global::Doroti.Framework.Widgets.WidgetStateBorderSide)
        {
            a = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)a).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        if (b is global::Doroti.Framework.Widgets.WidgetStateBorderSide)
        {
            b = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)b).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        a ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: a.color.withAlpha(0L));
        return (global::Doroti.Framework.Painting.BorderSide?)BorderSide.lerp(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

public class SearchViewTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual SearchViewThemeData data { get; private set; } = default!;

    public SearchViewTheme(global::Doroti.Framework.Foundation.Key? key = null, SearchViewThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SearchViewThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SearchViewTheme? searchViewThemeLocal = context.dependOnInheritedWidgetOfExactType<SearchViewTheme>();
        return searchViewThemeLocal?.data ?? Theme.of(context).searchViewTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new SearchViewTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SearchViewTheme)oldWidget).data));
}

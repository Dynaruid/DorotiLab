// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/search_view_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SearchViewThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual double? headerHeight { get; private set; }
    public virtual TextStyle? headerTextStyle { get; private set; }
    public virtual TextStyle? headerHintStyle { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual EdgeInsetsGeometry? barPadding { get; private set; }
    public virtual bool? shrinkWrap { get; private set; }
    public virtual Color? dividerColor { get; private set; }

    public SearchViewThemeData(Color? backgroundColor = null, double? elevation = null, Color? surfaceTintColor = null, BoxConstraints? constraints = null, EdgeInsetsGeometry? padding = null, EdgeInsetsGeometry? barPadding = null, bool? shrinkWrap = null, BorderSide? side = null, OutlinedBorder? shape = null, double? headerHeight = null, TextStyle? headerTextStyle = null, TextStyle? headerHintStyle = null, Color? dividerColor = null)
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

    public virtual SearchViewThemeData copyWith(Color? backgroundColor = null, double? elevation = null, Color? surfaceTintColor = null, BorderSide? side = null, OutlinedBorder? shape = null, double? headerHeight = null, TextStyle? headerTextStyle = null, TextStyle? headerHintStyle = null, BoxConstraints? constraints = null, EdgeInsetsGeometry? padding = null, EdgeInsetsGeometry? barPadding = null, bool? shrinkWrap = null, Color? dividerColor = null)
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

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DiagnosticsProperty<Color?>("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<double?>("elevation", elevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<Color?>("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<BorderSide?>("side", side, defaultValue: null));
        properties.add(new DiagnosticsProperty<OutlinedBorder?>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<double?>("headerHeight", headerHeight, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle?>("headerTextStyle", headerTextStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle?>("headerHintStyle", headerHintStyle, defaultValue: null));
        properties.add(new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry?>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry?>("barPadding", barPadding, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool?>("shrinkWrap", shrinkWrap, defaultValue: null));
        properties.add(new DiagnosticsProperty<Color?>("dividerColor", dividerColor, defaultValue: null));
    }

    internal static BorderSide? _lerpSides(BorderSide? a, BorderSide? b, double t)
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        if (a is WidgetStateBorderSide)
        {
            a = ((WidgetStateBorderSide)a).resolve(new HashSet<WidgetState>());
        }
        if (b is WidgetStateBorderSide)
        {
            b = ((WidgetStateBorderSide)b).resolve(new HashSet<WidgetState>());
        }
        a ??= new BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new BorderSide(width: 0, color: a.color.withAlpha(0L));
        return (BorderSide?)BorderSide.lerp(a, b, t);
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

public class SearchViewTheme : InheritedTheme
{
    public virtual SearchViewThemeData data { get; private set; } = default!;

    public SearchViewTheme(Key? key = null, SearchViewThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SearchViewThemeData of(BuildContext context)
    {
        SearchViewTheme? searchViewThemeLocal = context.dependOnInheritedWidgetOfExactType<SearchViewTheme>();
        return searchViewThemeLocal?.data ?? Theme.of(context).searchViewTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new SearchViewTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SearchViewTheme)oldWidget).data));
}

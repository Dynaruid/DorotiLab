// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_rail_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class NavigationRailThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual double? groupAlignment { get; private set; }
    public virtual NavigationRailLabelType? labelType { get; private set; }
    public virtual bool? useIndicator { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual double? minWidth { get; private set; }
    public virtual double? minExtendedWidth { get; private set; }

    public NavigationRailThemeData(Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle = null, global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle = null, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = null, double? groupAlignment = null, NavigationRailLabelType? labelType = null, bool? useIndicator = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, double? minWidth = null, double? minExtendedWidth = null)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.unselectedLabelTextStyle = unselectedLabelTextStyle;
        this.selectedLabelTextStyle = selectedLabelTextStyle;
        this.unselectedIconTheme = unselectedIconTheme;
        this.selectedIconTheme = selectedIconTheme;
        this.groupAlignment = groupAlignment;
        this.labelType = labelType;
        this.useIndicator = useIndicator;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.minWidth = minWidth;
        this.minExtendedWidth = minExtendedWidth;
    }

    public virtual NavigationRailThemeData copyWith(Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.TextStyle? unselectedLabelTextStyle = null, global::Doroti.Framework.Painting.TextStyle? selectedLabelTextStyle = null, global::Doroti.Framework.Widgets.IconThemeData? unselectedIconTheme = null, global::Doroti.Framework.Widgets.IconThemeData? selectedIconTheme = null, double? groupAlignment = null, NavigationRailLabelType? labelType = null, bool? useIndicator = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, double? minWidth = null, double? minExtendedWidth = null)
    {
        return new NavigationRailThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, elevation: elevation ?? this.elevation, unselectedLabelTextStyle: unselectedLabelTextStyle ?? this.unselectedLabelTextStyle, selectedLabelTextStyle: selectedLabelTextStyle ?? this.selectedLabelTextStyle, unselectedIconTheme: unselectedIconTheme ?? this.unselectedIconTheme, selectedIconTheme: selectedIconTheme ?? this.selectedIconTheme, groupAlignment: groupAlignment ?? this.groupAlignment, labelType: labelType ?? this.labelType, useIndicator: useIndicator ?? this.useIndicator, indicatorColor: indicatorColor ?? this.indicatorColor, indicatorShape: indicatorShape ?? this.indicatorShape, minWidth: minWidth ?? this.minWidth, minExtendedWidth: minExtendedWidth ?? this.minExtendedWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static NavigationRailThemeData? lerp(NavigationRailThemeData? a, NavigationRailThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new NavigationRailThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), unselectedLabelTextStyle: TextStyle.lerp(a?.unselectedLabelTextStyle, b?.unselectedLabelTextStyle, t), selectedLabelTextStyle: TextStyle.lerp(a?.selectedLabelTextStyle, b?.selectedLabelTextStyle, t), unselectedIconTheme: ((a?.unselectedIconTheme is null) && (b?.unselectedIconTheme is null)) ? null : IconThemeData.lerp(a?.unselectedIconTheme, b?.unselectedIconTheme, t), selectedIconTheme: ((a?.selectedIconTheme is null) && (b?.selectedIconTheme is null)) ? null : IconThemeData.lerp(a?.selectedIconTheme, b?.selectedIconTheme, t), groupAlignment: Dart_uiLibrary.lerpDouble(a?.groupAlignment, b?.groupAlignment, t), labelType: (t < 0.5) ? a?.labelType : b?.labelType, useIndicator: (t < 0.5) ? a?.useIndicator : b?.useIndicator, indicatorColor: Dart_uiLibrary.Color.lerp(a?.indicatorColor, b?.indicatorColor, t), indicatorShape: ShapeBorder.lerp(a?.indicatorShape, b?.indicatorShape, t), minWidth: Dart_uiLibrary.lerpDouble(a?.minWidth, b?.minWidth, t), minExtendedWidth: Dart_uiLibrary.lerpDouble(a?.minExtendedWidth, b?.minExtendedWidth, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, elevation, unselectedLabelTextStyle, selectedLabelTextStyle, unselectedIconTheme, selectedIconTheme, groupAlignment, labelType, useIndicator, indicatorColor, indicatorShape, minWidth, minExtendedWidth));
    public override bool Equals(object? other)
    {
        var __other = other as NavigationRailThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is NavigationRailThemeData) && Equals(__other.backgroundColor, backgroundColor) && (__other.elevation == elevation) && Equals(__other.unselectedLabelTextStyle, unselectedLabelTextStyle) && Equals(__other.selectedLabelTextStyle, selectedLabelTextStyle) && Equals(__other.unselectedIconTheme, unselectedIconTheme) && Equals(__other.selectedIconTheme, selectedIconTheme) && (__other.groupAlignment == groupAlignment) && Equals(__other.labelType, labelType) && (__other.useIndicator == useIndicator) && Equals(__other.indicatorColor, indicatorColor) && Equals(__other.indicatorShape, indicatorShape) && (__other.minWidth == minWidth) && (__other.minExtendedWidth == minExtendedWidth);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData = new NavigationRailThemeData();
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: defaultData.backgroundColor));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: defaultData.elevation));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("unselectedLabelTextStyle", unselectedLabelTextStyle, defaultValue: defaultData.unselectedLabelTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("selectedLabelTextStyle", selectedLabelTextStyle, defaultValue: defaultData.selectedLabelTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("unselectedIconTheme", unselectedIconTheme, defaultValue: defaultData.unselectedIconTheme));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("selectedIconTheme", selectedIconTheme, defaultValue: defaultData.selectedIconTheme));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("groupAlignment", groupAlignment, defaultValue: defaultData.groupAlignment));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<NavigationRailLabelType>("labelType", labelType, defaultValue: defaultData.labelType));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("useIndicator", useIndicator, defaultValue: defaultData.useIndicator));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("indicatorColor", indicatorColor, defaultValue: defaultData.indicatorColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("indicatorShape", indicatorShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minWidth", minWidth, defaultValue: defaultData.minWidth));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minExtendedWidth", minExtendedWidth, defaultValue: defaultData.minExtendedWidth));
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

public class NavigationRailTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual NavigationRailThemeData data { get; private set; } = default!;

    public NavigationRailTheme(global::Doroti.Framework.Foundation.Key? key = null, NavigationRailThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static NavigationRailThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NavigationRailTheme? navigationRailThemeLocal = context.dependOnInheritedWidgetOfExactType<NavigationRailTheme>();
        return navigationRailThemeLocal?.data ?? Theme.of(context).navigationRailTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new NavigationRailTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((NavigationRailTheme)oldWidget).data));
}

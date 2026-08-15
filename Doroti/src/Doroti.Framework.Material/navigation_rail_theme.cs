// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_rail_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class NavigationRailThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? selectedLabelTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? unselectedIconTheme { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? selectedIconTheme { get; private set; }
    public virtual double? groupAlignment { get; private set; }
    public virtual NavigationRailLabelType? labelType { get; private set; }
    public virtual bool? useIndicator { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual double? minWidth { get; private set; }
    public virtual double? minExtendedWidth { get; private set; }

    public NavigationRailThemeData(Color? backgroundColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? selectedLabelTextStyle = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? unselectedIconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? selectedIconTheme = null, double? groupAlignment = null, NavigationRailLabelType? labelType = null, bool? useIndicator = null, Color? indicatorColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? indicatorShape = null, double? minWidth = null, double? minExtendedWidth = null)
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

    public virtual NavigationRailThemeData copyWith(Color? backgroundColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.TextStyle? unselectedLabelTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? selectedLabelTextStyle = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? unselectedIconTheme = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? selectedIconTheme = null, double? groupAlignment = null, NavigationRailLabelType? labelType = null, bool? useIndicator = null, Color? indicatorColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? indicatorShape = null, double? minWidth = null, double? minExtendedWidth = null)
    {
        return new NavigationRailThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), unselectedLabelTextStyle: (unselectedLabelTextStyle ?? this.unselectedLabelTextStyle), selectedLabelTextStyle: (selectedLabelTextStyle ?? this.selectedLabelTextStyle), unselectedIconTheme: (unselectedIconTheme ?? this.unselectedIconTheme), selectedIconTheme: (selectedIconTheme ?? this.selectedIconTheme), groupAlignment: (groupAlignment ?? this.groupAlignment), labelType: (labelType ?? this.labelType), useIndicator: (useIndicator ?? this.useIndicator), indicatorColor: (indicatorColor ?? this.indicatorColor), indicatorShape: (indicatorShape ?? this.indicatorShape), minWidth: (minWidth ?? this.minWidth), minExtendedWidth: (minExtendedWidth ?? this.minExtendedWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static NavigationRailThemeData? lerp(NavigationRailThemeData? a, NavigationRailThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new NavigationRailThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), unselectedLabelTextStyle: TextStyle.lerp(a?.unselectedLabelTextStyle, b?.unselectedLabelTextStyle, t), selectedLabelTextStyle: TextStyle.lerp(a?.selectedLabelTextStyle, b?.selectedLabelTextStyle, t), unselectedIconTheme: (((a?.unselectedIconTheme is null) && (b?.unselectedIconTheme is null)) ? null : IconThemeData.lerp(a?.unselectedIconTheme, b?.unselectedIconTheme, t)), selectedIconTheme: (((a?.selectedIconTheme is null) && (b?.selectedIconTheme is null)) ? null : IconThemeData.lerp(a?.selectedIconTheme, b?.selectedIconTheme, t)), groupAlignment: Dart_uiLibrary.lerpDouble(a?.groupAlignment, b?.groupAlignment, t), labelType: ((t < 0.5) ? a?.labelType : b?.labelType), useIndicator: ((t < 0.5) ? a?.useIndicator : b?.useIndicator), indicatorColor: Dart_uiLibrary.Color.lerp(a?.indicatorColor, b?.indicatorColor, t), indicatorShape: ShapeBorder.lerp(a?.indicatorShape, b?.indicatorShape, t), minWidth: Dart_uiLibrary.lerpDouble(a?.minWidth, b?.minWidth, t), minExtendedWidth: Dart_uiLibrary.lerpDouble(a?.minExtendedWidth, b?.minExtendedWidth, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.elevation, this.unselectedLabelTextStyle, this.selectedLabelTextStyle, this.unselectedIconTheme, this.selectedIconTheme, this.groupAlignment, this.labelType, this.useIndicator, this.indicatorColor, this.indicatorShape, this.minWidth, this.minExtendedWidth));
    public override bool Equals(object? other)
    {
        var __other = other as NavigationRailThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((__other is NavigationRailThemeData) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).backgroundColor, this.backgroundColor))) && (((NavigationRailThemeData)((NavigationRailThemeData)__other)).elevation == this.elevation)) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).unselectedLabelTextStyle, this.unselectedLabelTextStyle))) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).selectedLabelTextStyle, this.selectedLabelTextStyle))) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).unselectedIconTheme, this.unselectedIconTheme))) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).selectedIconTheme, this.selectedIconTheme))) && (((NavigationRailThemeData)((NavigationRailThemeData)__other)).groupAlignment == this.groupAlignment)) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).labelType, this.labelType))) && (((NavigationRailThemeData)((NavigationRailThemeData)__other)).useIndicator == this.useIndicator)) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).indicatorColor, this.indicatorColor))) && (object.Equals(((NavigationRailThemeData)((NavigationRailThemeData)__other)).indicatorShape, this.indicatorShape))) && (((NavigationRailThemeData)((NavigationRailThemeData)__other)).minWidth == this.minWidth)) && (((NavigationRailThemeData)((NavigationRailThemeData)__other)).minExtendedWidth == this.minExtendedWidth));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData__8189 = new NavigationRailThemeData();
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: ((NavigationRailThemeData)defaultData__8189).backgroundColor));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: ((NavigationRailThemeData)defaultData__8189).elevation));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("unselectedLabelTextStyle", this.unselectedLabelTextStyle, defaultValue: ((NavigationRailThemeData)defaultData__8189).unselectedLabelTextStyle));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("selectedLabelTextStyle", this.selectedLabelTextStyle, defaultValue: ((NavigationRailThemeData)defaultData__8189).selectedLabelTextStyle));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData>("unselectedIconTheme", this.unselectedIconTheme, defaultValue: ((NavigationRailThemeData)defaultData__8189).unselectedIconTheme));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData>("selectedIconTheme", this.selectedIconTheme, defaultValue: ((NavigationRailThemeData)defaultData__8189).selectedIconTheme));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("groupAlignment", this.groupAlignment, defaultValue: ((NavigationRailThemeData)defaultData__8189).groupAlignment));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<NavigationRailLabelType>("labelType", this.labelType, defaultValue: ((NavigationRailThemeData)defaultData__8189).labelType));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("useIndicator", this.useIndicator, defaultValue: ((NavigationRailThemeData)defaultData__8189).useIndicator));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("indicatorColor", this.indicatorColor, defaultValue: ((NavigationRailThemeData)defaultData__8189).indicatorColor));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("indicatorShape", this.indicatorShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minWidth", this.minWidth, defaultValue: ((NavigationRailThemeData)defaultData__8189).minWidth));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minExtendedWidth", this.minExtendedWidth, defaultValue: ((NavigationRailThemeData)defaultData__8189).minExtendedWidth));
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

public class NavigationRailTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual NavigationRailThemeData data { get; private set; } = default!;

    public NavigationRailTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, NavigationRailThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static NavigationRailThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        NavigationRailTheme? navigationRailTheme__11397 = ((NavigationRailTheme?)(object?)context.dependOnInheritedWidgetOfExactType<NavigationRailTheme>());
        return (navigationRailTheme__11397?.data ?? Theme.of(context).navigationRailTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new NavigationRailTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((NavigationRailTheme)oldWidget).data)));
}

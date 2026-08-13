// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/popup_menu_theme.dart
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

public enum PopupMenuPosition
{
    over,
    under
}

public class PopupMenuThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual PopupMenuPosition? position { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual double? iconSize { get; private set; }

    public PopupMenuThemeData(Color? color = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, PopupMenuPosition? position = null, Color? iconColor = null, double? iconSize = null)
    {
        this.color = color;
        this.shape = shape;
        this.menuPadding = menuPadding;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.textStyle = textStyle;
        this.labelTextStyle = labelTextStyle;
        this.enableFeedback = enableFeedback;
        this.mouseCursor = mouseCursor;
        this.position = position;
        this.iconColor = iconColor;
        this.iconSize = iconSize;
    }

    public virtual PopupMenuThemeData copyWith(Color? color = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor = null, PopupMenuPosition? position = null, Color? iconColor = null, double? iconSize = null)
    {
        return new PopupMenuThemeData(color: (color ?? this.color), shape: (shape ?? this.shape), menuPadding: (menuPadding ?? this.menuPadding), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), textStyle: (textStyle ?? this.textStyle), labelTextStyle: (labelTextStyle ?? this.labelTextStyle), enableFeedback: (enableFeedback ?? this.enableFeedback), mouseCursor: (mouseCursor ?? this.mouseCursor), position: (position ?? this.position), iconColor: (iconColor ?? this.iconColor), iconSize: (iconSize ?? this.iconSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static PopupMenuThemeData? lerp(PopupMenuThemeData? a, PopupMenuThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new PopupMenuThemeData(color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), menuPadding: EdgeInsetsGeometry.lerp(a?.menuPadding, b?.menuPadding, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), labelTextStyle: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.TextStyle?>(a?.labelTextStyle, b?.labelTextStyle, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.TextStyle?, global::Doroti.Generated.Framework.Painting.TextStyle?, double, global::Doroti.Generated.Framework.Painting.TextStyle?>)global::Doroti.Generated.Framework.Painting.TextStyle.lerp), enableFeedback: ((t < 0.5) ? a?.enableFeedback : b?.enableFeedback), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), position: ((t < 0.5) ? a?.position : b?.position), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), iconSize: Dart_uiLibrary.lerpDouble(a?.iconSize, b?.iconSize, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.color, this.shape, this.menuPadding, this.elevation, this.shadowColor, this.surfaceTintColor, this.textStyle, this.labelTextStyle, this.enableFeedback, this.mouseCursor, this.position, this.iconColor, this.iconSize));
    public override bool Equals(object? other)
    {
        var __other = other as PopupMenuThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((__other is PopupMenuThemeData) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).color, this.color))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).shape, this.shape))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).menuPadding, this.menuPadding))) && (((PopupMenuThemeData)((PopupMenuThemeData)__other)).elevation == this.elevation)) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).textStyle, this.textStyle))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).labelTextStyle, this.labelTextStyle))) && (((PopupMenuThemeData)((PopupMenuThemeData)__other)).enableFeedback == this.enableFeedback)) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).position, this.position))) && (object.Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).iconColor, this.iconColor))) && (((PopupMenuThemeData)((PopupMenuThemeData)__other)).iconSize == this.iconSize));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("menuPadding", this.menuPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("text style", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>("labelTextStyle", this.labelTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", this.enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<PopupMenuPosition>("position", this.position, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("iconSize", this.iconSize, defaultValue: null));
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

public class PopupMenuTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual PopupMenuThemeData data { get; private set; } = default!;

    public PopupMenuTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, PopupMenuThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static PopupMenuThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        PopupMenuTheme? popupMenuTheme__9465 = ((PopupMenuTheme?)(object?)context.dependOnInheritedWidgetOfExactType<PopupMenuTheme>());
        return (popupMenuTheme__9465?.data ?? Theme.of(context).popupMenuTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new PopupMenuTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((PopupMenuTheme)oldWidget).data)));
}

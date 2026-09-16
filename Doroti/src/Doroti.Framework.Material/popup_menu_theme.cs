// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/popup_menu_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum PopupMenuPosition
{
    over,
    under
}

public class PopupMenuThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? menuPadding { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual PopupMenuPosition? position { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual double? iconSize { get; private set; }

    public PopupMenuThemeData(Color? color = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle = null, bool? enableFeedback = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, PopupMenuPosition? position = null, Color? iconColor = null, double? iconSize = null)
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

    public virtual PopupMenuThemeData copyWith(Color? color = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? menuPadding = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle = null, bool? enableFeedback = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, PopupMenuPosition? position = null, Color? iconColor = null, double? iconSize = null)
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
        return new PopupMenuThemeData(color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), menuPadding: EdgeInsetsGeometry.lerp(a?.menuPadding, b?.menuPadding, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), labelTextStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.labelTextStyle, b?.labelTextStyle, t, (global::System.Func<global::Doroti.Framework.Painting.TextStyle?, global::Doroti.Framework.Painting.TextStyle?, double, global::Doroti.Framework.Painting.TextStyle?>)TextStyle.lerp), enableFeedback: ((t < 0.5) ? a?.enableFeedback : b?.enableFeedback), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor), position: ((t < 0.5) ? a?.position : b?.position), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), iconSize: Dart_uiLibrary.lerpDouble(a?.iconSize, b?.iconSize, t));
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
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((__other is PopupMenuThemeData) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).color, this.color))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).shape, this.shape))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).menuPadding, this.menuPadding))) && (((PopupMenuThemeData)((PopupMenuThemeData)__other)).elevation == this.elevation)) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).shadowColor, this.shadowColor))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).textStyle, this.textStyle))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).labelTextStyle, this.labelTextStyle))) && (((PopupMenuThemeData)((PopupMenuThemeData)__other)).enableFeedback == this.enableFeedback)) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).mouseCursor, this.mouseCursor))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).position, this.position))) && (Equals(((PopupMenuThemeData)((PopupMenuThemeData)__other)).iconColor, this.iconColor))) && (((PopupMenuThemeData)((PopupMenuThemeData)__other)).iconSize == this.iconSize));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("menuPadding", this.menuPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("text style", this.textStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("labelTextStyle", this.labelTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", this.enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<PopupMenuPosition>("position", this.position, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("iconSize", this.iconSize, defaultValue: null));
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
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PopupMenuTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual PopupMenuThemeData data { get; private set; } = default!;

    public PopupMenuTheme(global::Doroti.Framework.Foundation.Key? key = null, PopupMenuThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static PopupMenuThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        PopupMenuTheme? popupMenuThemeLocal = ((PopupMenuTheme?)context.dependOnInheritedWidgetOfExactType<PopupMenuTheme>());
        return (popupMenuThemeLocal?.data ?? Theme.of(context).popupMenuTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)new PopupMenuTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(this.data, ((PopupMenuTheme)oldWidget).data)));
}

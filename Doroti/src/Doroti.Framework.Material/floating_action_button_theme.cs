// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/floating_action_button_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class FloatingActionButtonThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? foregroundColor { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? focusElevation { get; private set; }
    public virtual double? hoverElevation { get; private set; }
    public virtual double? disabledElevation { get; private set; }
    public virtual double? highlightElevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? iconSize { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? sizeConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? smallSizeConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? largeSizeConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? extendedSizeConstraints { get; private set; }
    public virtual double? extendedIconLabelSpacing { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? extendedPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? extendedTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }

    public FloatingActionButtonThemeData(Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? disabledElevation = null, double? highlightElevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool? enableFeedback = null, double? iconSize = null, global::Doroti.Framework.Rendering.BoxConstraints? sizeConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? smallSizeConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? largeSizeConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? extendedSizeConstraints = null, double? extendedIconLabelSpacing = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? extendedPadding = null, global::Doroti.Framework.Painting.TextStyle? extendedTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null)
    {
        this.foregroundColor = foregroundColor;
        this.backgroundColor = backgroundColor;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.splashColor = splashColor;
        this.elevation = elevation;
        this.focusElevation = focusElevation;
        this.hoverElevation = hoverElevation;
        this.disabledElevation = disabledElevation;
        this.highlightElevation = highlightElevation;
        this.shape = shape;
        this.enableFeedback = enableFeedback;
        this.iconSize = iconSize;
        this.sizeConstraints = sizeConstraints;
        this.smallSizeConstraints = smallSizeConstraints;
        this.largeSizeConstraints = largeSizeConstraints;
        this.extendedSizeConstraints = extendedSizeConstraints;
        this.extendedIconLabelSpacing = extendedIconLabelSpacing;
        this.extendedPadding = extendedPadding;
        this.extendedTextStyle = extendedTextStyle;
        this.mouseCursor = mouseCursor;
    }

    public virtual FloatingActionButtonThemeData copyWith(Color? foregroundColor = null, Color? backgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? splashColor = null, double? elevation = null, double? focusElevation = null, double? hoverElevation = null, double? disabledElevation = null, double? highlightElevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool? enableFeedback = null, double? iconSize = null, global::Doroti.Framework.Rendering.BoxConstraints? sizeConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? smallSizeConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? largeSizeConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? extendedSizeConstraints = null, double? extendedIconLabelSpacing = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? extendedPadding = null, global::Doroti.Framework.Painting.TextStyle? extendedTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null)
    {
        return new FloatingActionButtonThemeData(foregroundColor: foregroundColor ?? this.foregroundColor, backgroundColor: backgroundColor ?? this.backgroundColor, focusColor: focusColor ?? this.focusColor, hoverColor: hoverColor ?? this.hoverColor, splashColor: splashColor ?? this.splashColor, elevation: elevation ?? this.elevation, focusElevation: focusElevation ?? this.focusElevation, hoverElevation: hoverElevation ?? this.hoverElevation, disabledElevation: disabledElevation ?? this.disabledElevation, highlightElevation: highlightElevation ?? this.highlightElevation, shape: shape ?? this.shape, enableFeedback: enableFeedback ?? this.enableFeedback, iconSize: iconSize ?? this.iconSize, sizeConstraints: sizeConstraints ?? this.sizeConstraints, smallSizeConstraints: smallSizeConstraints ?? this.smallSizeConstraints, largeSizeConstraints: largeSizeConstraints ?? this.largeSizeConstraints, extendedSizeConstraints: extendedSizeConstraints ?? this.extendedSizeConstraints, extendedIconLabelSpacing: extendedIconLabelSpacing ?? this.extendedIconLabelSpacing, extendedPadding: extendedPadding ?? this.extendedPadding, extendedTextStyle: extendedTextStyle ?? this.extendedTextStyle, mouseCursor: mouseCursor ?? this.mouseCursor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FloatingActionButtonThemeData? lerp(FloatingActionButtonThemeData? a, FloatingActionButtonThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new FloatingActionButtonThemeData(foregroundColor: Dart_uiLibrary.Color.lerp(a?.foregroundColor, b?.foregroundColor, t), backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), focusColor: Dart_uiLibrary.Color.lerp(a?.focusColor, b?.focusColor, t), hoverColor: Dart_uiLibrary.Color.lerp(a?.hoverColor, b?.hoverColor, t), splashColor: Dart_uiLibrary.Color.lerp(a?.splashColor, b?.splashColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), focusElevation: Dart_uiLibrary.lerpDouble(a?.focusElevation, b?.focusElevation, t), hoverElevation: Dart_uiLibrary.lerpDouble(a?.hoverElevation, b?.hoverElevation, t), disabledElevation: Dart_uiLibrary.lerpDouble(a?.disabledElevation, b?.disabledElevation, t), highlightElevation: Dart_uiLibrary.lerpDouble(a?.highlightElevation, b?.highlightElevation, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), enableFeedback: (t < 0.5) ? a?.enableFeedback : b?.enableFeedback, iconSize: Dart_uiLibrary.lerpDouble(a?.iconSize, b?.iconSize, t), sizeConstraints: BoxConstraints.lerp(a?.sizeConstraints, b?.sizeConstraints, t), smallSizeConstraints: BoxConstraints.lerp(a?.smallSizeConstraints, b?.smallSizeConstraints, t), largeSizeConstraints: BoxConstraints.lerp(a?.largeSizeConstraints, b?.largeSizeConstraints, t), extendedSizeConstraints: BoxConstraints.lerp(a?.extendedSizeConstraints, b?.extendedSizeConstraints, t), extendedIconLabelSpacing: Dart_uiLibrary.lerpDouble(a?.extendedIconLabelSpacing, b?.extendedIconLabelSpacing, t), extendedPadding: EdgeInsetsGeometry.lerp(a?.extendedPadding, b?.extendedPadding, t), extendedTextStyle: TextStyle.lerp(a?.extendedTextStyle, b?.extendedTextStyle, t), mouseCursor: (t < 0.5) ? a?.mouseCursor : b?.mouseCursor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(foregroundColor, backgroundColor, focusColor, hoverColor, splashColor, elevation, focusElevation, hoverElevation, disabledElevation, highlightElevation, shape, enableFeedback, iconSize, sizeConstraints, smallSizeConstraints, largeSizeConstraints, extendedSizeConstraints, extendedIconLabelSpacing, extendedPadding, FoundationRuntimePorts.ObjectHash(extendedTextStyle, mouseCursor)));
    public override bool Equals(object? other)
    {
        var __other = other as FloatingActionButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is FloatingActionButtonThemeData) && Equals(__other.foregroundColor, foregroundColor) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.focusColor, focusColor) && Equals(__other.hoverColor, hoverColor) && Equals(__other.splashColor, splashColor) && (__other.elevation == elevation) && (__other.focusElevation == focusElevation) && (__other.hoverElevation == hoverElevation) && (__other.disabledElevation == disabledElevation) && (__other.highlightElevation == highlightElevation) && Equals(__other.shape, shape) && (__other.enableFeedback == enableFeedback) && (__other.iconSize == iconSize) && Equals(__other.sizeConstraints, sizeConstraints) && Equals(__other.smallSizeConstraints, smallSizeConstraints) && Equals(__other.largeSizeConstraints, largeSizeConstraints) && Equals(__other.extendedSizeConstraints, extendedSizeConstraints) && (__other.extendedIconLabelSpacing == extendedIconLabelSpacing) && Equals(__other.extendedPadding, extendedPadding) && Equals(__other.extendedTextStyle, extendedTextStyle) && Equals(__other.mouseCursor, mouseCursor);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("foregroundColor", foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("focusElevation", focusElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("hoverElevation", hoverElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("disabledElevation", disabledElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("highlightElevation", highlightElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("iconSize", iconSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("sizeConstraints", sizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("smallSizeConstraints", smallSizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("largeSizeConstraints", largeSizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("extendedSizeConstraints", extendedSizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("extendedIconLabelSpacing", extendedIconLabelSpacing, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("extendedPadding", extendedPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("extendedTextStyle", extendedTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", mouseCursor, defaultValue: null));
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

public class FloatingActionButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual FloatingActionButtonThemeData data { get; private set; } = default!;

    public FloatingActionButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, FloatingActionButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static FloatingActionButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        FloatingActionButtonTheme? fabTheme = context.dependOnInheritedWidgetOfExactType<FloatingActionButtonTheme>();
        return fabTheme?.data ?? Theme.of(context).floatingActionButtonTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new FloatingActionButtonTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((FloatingActionButtonTheme)oldWidget).data));
}

// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/floating_action_button_theme.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

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
        return new FloatingActionButtonThemeData(foregroundColor: (foregroundColor ?? this.foregroundColor), backgroundColor: (backgroundColor ?? this.backgroundColor), focusColor: (focusColor ?? this.focusColor), hoverColor: (hoverColor ?? this.hoverColor), splashColor: (splashColor ?? this.splashColor), elevation: (elevation ?? this.elevation), focusElevation: (focusElevation ?? this.focusElevation), hoverElevation: (hoverElevation ?? this.hoverElevation), disabledElevation: (disabledElevation ?? this.disabledElevation), highlightElevation: (highlightElevation ?? this.highlightElevation), shape: (shape ?? this.shape), enableFeedback: (enableFeedback ?? this.enableFeedback), iconSize: (iconSize ?? this.iconSize), sizeConstraints: (sizeConstraints ?? this.sizeConstraints), smallSizeConstraints: (smallSizeConstraints ?? this.smallSizeConstraints), largeSizeConstraints: (largeSizeConstraints ?? this.largeSizeConstraints), extendedSizeConstraints: (extendedSizeConstraints ?? this.extendedSizeConstraints), extendedIconLabelSpacing: (extendedIconLabelSpacing ?? this.extendedIconLabelSpacing), extendedPadding: (extendedPadding ?? this.extendedPadding), extendedTextStyle: (extendedTextStyle ?? this.extendedTextStyle), mouseCursor: (mouseCursor ?? this.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FloatingActionButtonThemeData? lerp(FloatingActionButtonThemeData? a, FloatingActionButtonThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new FloatingActionButtonThemeData(foregroundColor: Dart_uiLibrary.Color.lerp(a?.foregroundColor, b?.foregroundColor, t), backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), focusColor: Dart_uiLibrary.Color.lerp(a?.focusColor, b?.focusColor, t), hoverColor: Dart_uiLibrary.Color.lerp(a?.hoverColor, b?.hoverColor, t), splashColor: Dart_uiLibrary.Color.lerp(a?.splashColor, b?.splashColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), focusElevation: Dart_uiLibrary.lerpDouble(a?.focusElevation, b?.focusElevation, t), hoverElevation: Dart_uiLibrary.lerpDouble(a?.hoverElevation, b?.hoverElevation, t), disabledElevation: Dart_uiLibrary.lerpDouble(a?.disabledElevation, b?.disabledElevation, t), highlightElevation: Dart_uiLibrary.lerpDouble(a?.highlightElevation, b?.highlightElevation, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), enableFeedback: ((t < 0.5) ? a?.enableFeedback : b?.enableFeedback), iconSize: Dart_uiLibrary.lerpDouble(a?.iconSize, b?.iconSize, t), sizeConstraints: BoxConstraints.lerp(a?.sizeConstraints, b?.sizeConstraints, t), smallSizeConstraints: BoxConstraints.lerp(a?.smallSizeConstraints, b?.smallSizeConstraints, t), largeSizeConstraints: BoxConstraints.lerp(a?.largeSizeConstraints, b?.largeSizeConstraints, t), extendedSizeConstraints: BoxConstraints.lerp(a?.extendedSizeConstraints, b?.extendedSizeConstraints, t), extendedIconLabelSpacing: Dart_uiLibrary.lerpDouble(a?.extendedIconLabelSpacing, b?.extendedIconLabelSpacing, t), extendedPadding: EdgeInsetsGeometry.lerp(a?.extendedPadding, b?.extendedPadding, t), extendedTextStyle: TextStyle.lerp(a?.extendedTextStyle, b?.extendedTextStyle, t), mouseCursor: ((t < 0.5) ? a?.mouseCursor : b?.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.foregroundColor, this.backgroundColor, this.focusColor, this.hoverColor, this.splashColor, this.elevation, this.focusElevation, this.hoverElevation, this.disabledElevation, this.highlightElevation, this.shape, this.enableFeedback, this.iconSize, this.sizeConstraints, this.smallSizeConstraints, this.largeSizeConstraints, this.extendedSizeConstraints, this.extendedIconLabelSpacing, this.extendedPadding, FoundationRuntimePorts.ObjectHash(this.extendedTextStyle, this.mouseCursor)));
    public override bool Equals(object? other)
    {
        var __other = other as FloatingActionButtonThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((((__other is FloatingActionButtonThemeData) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).foregroundColor, this.foregroundColor))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).focusColor, this.focusColor))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).hoverColor, this.hoverColor))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).splashColor, this.splashColor))) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).elevation == this.elevation)) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).focusElevation == this.focusElevation)) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).hoverElevation == this.hoverElevation)) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).disabledElevation == this.disabledElevation)) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).highlightElevation == this.highlightElevation)) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).shape, this.shape))) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).enableFeedback == this.enableFeedback)) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).iconSize == this.iconSize)) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).sizeConstraints, this.sizeConstraints))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).smallSizeConstraints, this.smallSizeConstraints))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).largeSizeConstraints, this.largeSizeConstraints))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).extendedSizeConstraints, this.extendedSizeConstraints))) && (((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).extendedIconLabelSpacing == this.extendedIconLabelSpacing)) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).extendedPadding, this.extendedPadding))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).extendedTextStyle, this.extendedTextStyle))) && (object.Equals(((FloatingActionButtonThemeData)((FloatingActionButtonThemeData)__other)).mouseCursor, this.mouseCursor)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("foregroundColor", this.foregroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("focusElevation", this.focusElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("hoverElevation", this.hoverElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("disabledElevation", this.disabledElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("highlightElevation", this.highlightElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("enableFeedback", this.enableFeedback, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("iconSize", this.iconSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("sizeConstraints", this.sizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("smallSizeConstraints", this.smallSizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("largeSizeConstraints", this.largeSizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("extendedSizeConstraints", this.extendedSizeConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("extendedIconLabelSpacing", this.extendedIconLabelSpacing, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("extendedPadding", this.extendedPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("extendedTextStyle", this.extendedTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class FloatingActionButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual FloatingActionButtonThemeData data { get; private set; } = default!;

    public FloatingActionButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, FloatingActionButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static FloatingActionButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        FloatingActionButtonTheme? fabTheme__14931 = ((FloatingActionButtonTheme?)(object?)context.dependOnInheritedWidgetOfExactType<FloatingActionButtonTheme>());
        return (fabTheme__14931?.data ?? Theme.of(context).floatingActionButtonTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new FloatingActionButtonTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((FloatingActionButtonTheme)oldWidget).data)));
}

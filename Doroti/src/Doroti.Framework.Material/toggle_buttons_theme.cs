// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/toggle_buttons_theme.dart
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

public class ToggleButtonsThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? fillColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? borderColor { get; private set; }
    public virtual Color? selectedBorderColor { get; private set; }
    public virtual Color? disabledBorderColor { get; private set; }
    public virtual double? borderWidth { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderRadius? borderRadius { get; private set; }

    public ToggleButtonsThemeData(global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? borderWidth = null)
    {
        this.textStyle = textStyle;
        this.constraints = constraints;
        this.color = color;
        this.selectedColor = selectedColor;
        this.disabledColor = disabledColor;
        this.fillColor = fillColor;
        this.focusColor = focusColor;
        this.highlightColor = highlightColor;
        this.hoverColor = hoverColor;
        this.splashColor = splashColor;
        this.borderColor = borderColor;
        this.selectedBorderColor = selectedBorderColor;
        this.disabledBorderColor = disabledBorderColor;
        this.borderRadius = borderRadius;
        this.borderWidth = borderWidth;
    }

    public virtual ToggleButtonsThemeData copyWith(global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? borderWidth = null)
    {
        return new ToggleButtonsThemeData(textStyle: (textStyle ?? this.textStyle), constraints: (constraints ?? this.constraints), color: (color ?? this.color), selectedColor: (selectedColor ?? this.selectedColor), disabledColor: (disabledColor ?? this.disabledColor), fillColor: (fillColor ?? this.fillColor), focusColor: (focusColor ?? this.focusColor), highlightColor: (highlightColor ?? this.highlightColor), hoverColor: (hoverColor ?? this.hoverColor), splashColor: (splashColor ?? this.splashColor), borderColor: (borderColor ?? this.borderColor), selectedBorderColor: (selectedBorderColor ?? this.selectedBorderColor), disabledBorderColor: (disabledBorderColor ?? this.disabledBorderColor), borderRadius: (borderRadius ?? this.borderRadius), borderWidth: (borderWidth ?? this.borderWidth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ToggleButtonsThemeData? lerp(ToggleButtonsThemeData? a, ToggleButtonsThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ToggleButtonsThemeData(textStyle: TextStyle.lerp(a?.textStyle, b?.textStyle, t), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), selectedColor: Dart_uiLibrary.Color.lerp(a?.selectedColor, b?.selectedColor, t), disabledColor: Dart_uiLibrary.Color.lerp(a?.disabledColor, b?.disabledColor, t), fillColor: Dart_uiLibrary.Color.lerp(a?.fillColor, b?.fillColor, t), focusColor: Dart_uiLibrary.Color.lerp(a?.focusColor, b?.focusColor, t), highlightColor: Dart_uiLibrary.Color.lerp(a?.highlightColor, b?.highlightColor, t), hoverColor: Dart_uiLibrary.Color.lerp(a?.hoverColor, b?.hoverColor, t), splashColor: Dart_uiLibrary.Color.lerp(a?.splashColor, b?.splashColor, t), borderColor: Dart_uiLibrary.Color.lerp(a?.borderColor, b?.borderColor, t), selectedBorderColor: Dart_uiLibrary.Color.lerp(a?.selectedBorderColor, b?.selectedBorderColor, t), disabledBorderColor: Dart_uiLibrary.Color.lerp(a?.disabledBorderColor, b?.disabledBorderColor, t), borderRadius: BorderRadius.lerp(a?.borderRadius, b?.borderRadius, t), borderWidth: Dart_uiLibrary.lerpDouble(a?.borderWidth, b?.borderWidth, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.textStyle, this.constraints, this.color, this.selectedColor, this.disabledColor, this.fillColor, this.focusColor, this.highlightColor, this.hoverColor, this.splashColor, this.borderColor, this.selectedBorderColor, this.disabledBorderColor, this.borderRadius, this.borderWidth));
    public override bool Equals(object? other)
    {
        var __other = other as ToggleButtonsThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((__other is ToggleButtonsThemeData) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).textStyle, this.textStyle))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).constraints, this.constraints))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).color, this.color))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).selectedColor, this.selectedColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).disabledColor, this.disabledColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).fillColor, this.fillColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).focusColor, this.focusColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).highlightColor, this.highlightColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).hoverColor, this.hoverColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).splashColor, this.splashColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).borderColor, this.borderColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).selectedBorderColor, this.selectedBorderColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).disabledBorderColor, this.disabledBorderColor))) && (object.Equals(((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).borderRadius, this.borderRadius))) && (((ToggleButtonsThemeData)((ToggleButtonsThemeData)__other)).borderWidth == this.borderWidth));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        this.textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedColor", this.selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("fillColor", this.fillColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", this.highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("borderColor", this.borderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedBorderColor", this.selectedBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledBorderColor", this.disabledBorderColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderRadius>("borderRadius", this.borderRadius, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("borderWidth", this.borderWidth, defaultValue: null));
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

public class ToggleButtonsTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual ToggleButtonsThemeData data { get; private set; } = default!;

    public ToggleButtonsTheme(global::Doroti.Framework.Foundation.Key? key = null, ToggleButtonsThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ToggleButtonsThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ToggleButtonsTheme? toggleButtonsTheme__10064 = ((ToggleButtonsTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ToggleButtonsTheme>());
        return (toggleButtonsTheme__10064?.data ?? Theme.of(context).toggleButtonsTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new ToggleButtonsTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ToggleButtonsTheme)oldWidget).data)));
}

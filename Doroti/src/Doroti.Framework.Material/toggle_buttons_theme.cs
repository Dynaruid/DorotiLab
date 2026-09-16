// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/toggle_buttons_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ToggleButtonsThemeData : Diagnosticable
{
    public virtual TextStyle? textStyle { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
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
    public virtual BorderRadius? borderRadius { get; private set; }

    public ToggleButtonsThemeData(TextStyle? textStyle = null, BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, BorderRadius? borderRadius = null, double? borderWidth = null)
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

    public virtual ToggleButtonsThemeData copyWith(TextStyle? textStyle = null, BoxConstraints? constraints = null, Color? color = null, Color? selectedColor = null, Color? disabledColor = null, Color? fillColor = null, Color? focusColor = null, Color? highlightColor = null, Color? hoverColor = null, Color? splashColor = null, Color? borderColor = null, Color? selectedBorderColor = null, Color? disabledBorderColor = null, BorderRadius? borderRadius = null, double? borderWidth = null)
    {
        return new ToggleButtonsThemeData(textStyle: textStyle ?? this.textStyle, constraints: constraints ?? this.constraints, color: color ?? this.color, selectedColor: selectedColor ?? this.selectedColor, disabledColor: disabledColor ?? this.disabledColor, fillColor: fillColor ?? this.fillColor, focusColor: focusColor ?? this.focusColor, highlightColor: highlightColor ?? this.highlightColor, hoverColor: hoverColor ?? this.hoverColor, splashColor: splashColor ?? this.splashColor, borderColor: borderColor ?? this.borderColor, selectedBorderColor: selectedBorderColor ?? this.selectedBorderColor, disabledBorderColor: disabledBorderColor ?? this.disabledBorderColor, borderRadius: borderRadius ?? this.borderRadius, borderWidth: borderWidth ?? this.borderWidth);
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

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(textStyle, constraints, color, selectedColor, disabledColor, fillColor, focusColor, highlightColor, hoverColor, splashColor, borderColor, selectedBorderColor, disabledBorderColor, borderRadius, borderWidth));
    public override bool Equals(object? other)
    {
        var __other = other as ToggleButtonsThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ToggleButtonsThemeData) && Equals(__other.textStyle, textStyle) && Equals(__other.constraints, constraints) && Equals(__other.color, color) && Equals(__other.selectedColor, selectedColor) && Equals(__other.disabledColor, disabledColor) && Equals(__other.fillColor, fillColor) && Equals(__other.focusColor, focusColor) && Equals(__other.highlightColor, highlightColor) && Equals(__other.hoverColor, hoverColor) && Equals(__other.splashColor, splashColor) && Equals(__other.borderColor, borderColor) && Equals(__other.selectedBorderColor, selectedBorderColor) && Equals(__other.disabledBorderColor, disabledBorderColor) && Equals(__other.borderRadius, borderRadius) && (__other.borderWidth == borderWidth);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        textStyle?.debugFillProperties(properties, prefix: "textStyle.");
        properties.add(new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new ColorProperty("selectedColor", selectedColor, defaultValue: null));
        properties.add(new ColorProperty("disabledColor", disabledColor, defaultValue: null));
        properties.add(new ColorProperty("fillColor", fillColor, defaultValue: null));
        properties.add(new ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new ColorProperty("highlightColor", highlightColor, defaultValue: null));
        properties.add(new ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new ColorProperty("borderColor", borderColor, defaultValue: null));
        properties.add(new ColorProperty("selectedBorderColor", selectedBorderColor, defaultValue: null));
        properties.add(new ColorProperty("disabledBorderColor", disabledBorderColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<BorderRadius>("borderRadius", borderRadius, defaultValue: null));
        properties.add(new DoubleProperty("borderWidth", borderWidth, defaultValue: null));
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

public class ToggleButtonsTheme : InheritedTheme
{
    public virtual ToggleButtonsThemeData data { get; private set; } = default!;

    public ToggleButtonsTheme(Key? key = null, ToggleButtonsThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ToggleButtonsThemeData of(BuildContext context)
    {
        ToggleButtonsTheme? toggleButtonsThemeLocal = context.dependOnInheritedWidgetOfExactType<ToggleButtonsTheme>();
        return toggleButtonsThemeLocal?.data ?? Theme.of(context).toggleButtonsTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new ToggleButtonsTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ToggleButtonsTheme)oldWidget).data));
}

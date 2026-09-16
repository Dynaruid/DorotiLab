// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum ButtonTextTheme
{
    normal,
    accent,
    primary
}

public enum ButtonBarLayoutBehavior
{
    constrained,
    padded
}

public class ButtonTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual ButtonThemeData data { get; private set; } = default!;

    public ButtonTheme(global::Doroti.Framework.Foundation.Key? key = null, ButtonTextTheme textTheme = ButtonTextTheme.normal, ButtonBarLayoutBehavior layoutBehavior = ButtonBarLayoutBehavior.padded, double minWidth = 88.0, double height = 36.0, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool alignedDropdown = false, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        data = new ButtonThemeData(textTheme: textTheme, minWidth: minWidth, height: height, padding: padding, shape: shape, alignedDropdown: alignedDropdown, layoutBehavior: layoutBehavior, buttonColor: buttonColor, disabledColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, colorScheme: colorScheme, materialTapTargetSize: materialTapTargetSize);
        System.Diagnostics.Debug.Assert(minWidth >= 0.0);
        System.Diagnostics.Debug.Assert(height >= 0.0);
    }

    public static ButtonTheme CreateFromButtonThemeData(global::Doroti.Framework.Foundation.Key? key = null, ButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        var __instance = new ButtonTheme(key: key, child: child);
        __instance.data = data;
        return __instance;
    }

    public static ButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ButtonTheme? inheritedButtonTheme = context.dependOnInheritedWidgetOfExactType<ButtonTheme>();
        ButtonThemeData? buttonThemeLocal = inheritedButtonTheme?.data;
        if (buttonThemeLocal?.colorScheme is null)
        {
            ThemeData theme = Theme.of(context);
            buttonThemeLocal ??= theme.buttonTheme;
            if (buttonThemeLocal.colorScheme is null)
            {
                buttonThemeLocal = buttonThemeLocal.copyWith(colorScheme: theme.buttonTheme.colorScheme ?? theme.colorScheme);
                DartRuntimePrimitives.Assert(() => buttonThemeLocal.colorScheme is not null);
            }
        }
        return buttonThemeLocal!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return CreateFromButtonThemeData(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ButtonTheme)oldWidget).data));
}

public class ButtonThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual double minWidth { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual ButtonTextTheme textTheme { get; private set; } = default!;
    public virtual ButtonBarLayoutBehavior layoutBehavior { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _padding { get; private set; }
    internal virtual global::Doroti.Framework.Painting.ShapeBorder? _shape { get; private set; }
    public virtual bool alignedDropdown { get; private set; } = default!;
    internal virtual Color? _buttonColor { get; private set; }
    internal virtual Color? _disabledColor { get; private set; }
    internal virtual Color? _focusColor { get; private set; }
    internal virtual Color? _hoverColor { get; private set; }
    internal virtual Color? _highlightColor { get; private set; }
    internal virtual Color? _splashColor { get; private set; }
    public virtual ColorScheme? colorScheme { get; private set; }
    internal virtual MaterialTapTargetSize? _materialTapTargetSize { get; private set; }

    public ButtonThemeData(ButtonTextTheme textTheme = ButtonTextTheme.normal, double minWidth = 88.0, double height = 36.0, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, ButtonBarLayoutBehavior layoutBehavior = ButtonBarLayoutBehavior.padded, bool alignedDropdown = false, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null)
    {
        this.textTheme = textTheme;
        this.minWidth = minWidth;
        this.height = height;
        this.layoutBehavior = layoutBehavior;
        this.alignedDropdown = alignedDropdown;
        this.colorScheme = colorScheme;
        _buttonColor = buttonColor;
        _disabledColor = disabledColor;
        _focusColor = focusColor;
        _hoverColor = hoverColor;
        _highlightColor = highlightColor;
        _splashColor = splashColor;
        _padding = padding;
        _shape = shape;
        _materialTapTargetSize = materialTapTargetSize;
        System.Diagnostics.Debug.Assert(minWidth >= 0.0);
        System.Diagnostics.Debug.Assert(height >= 0.0);
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints
    {
        get
        {
            return new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(minWidth), minHeight: DartRuntimePrimitives.RequireValue(height));
        }
    }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(_padding ?? (textTheme switch { ButtonTextTheme.normal => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.accent => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.primary => EdgeInsets.CreateSymmetric(horizontal: 24.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(_shape ?? (textTheme switch { ButtonTextTheme.normal => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))), ButtonTextTheme.accent => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))), ButtonTextTheme.primary => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    public virtual global::Doroti.Ui.Brightness getBrightness(MaterialButton button)
    {
        return button.colorBrightness ?? colorScheme!.brightness;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonTextTheme getTextTheme(MaterialButton button) => DartRuntimePrimitives.ConvertValue<ButtonTextTheme>(button.textTheme ?? textTheme);
    public virtual global::Doroti.Ui.Color getDisabledTextColor(MaterialButton button)
    {
        return (button.textColor ?? button.disabledTextColor) ?? colorScheme!.onSurface.withOpacity(0.38);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getDisabledFillColor(MaterialButton button)
    {
        return (button.disabledColor ?? _disabledColor) ?? colorScheme!.onSurface.withOpacity(0.38);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color? getFillColor(MaterialButton button)
    {
        global::Doroti.Ui.Color? fillColor = button.enabled ? button.color : button.disabledColor;
        if (fillColor is not null)
        {
            return (global::Doroti.Ui.Color?)fillColor;
        }
        if (Equals(DartRuntimePrimitives.RuntimeType(button), typeof(MaterialButton)))
        {
            return null;
        }
        if (button.enabled && (_buttonColor is not null))
        {
            return (global::Doroti.Ui.Color?)_buttonColor;
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
            case ButtonTextTheme.accent:
                {
                    return (global::Doroti.Ui.Color?)(button.enabled ? colorScheme!.primary : getDisabledFillColor(button));
                }
            case ButtonTextTheme.primary:
                {
                    return (global::Doroti.Ui.Color?)(button.enabled ? (_buttonColor ?? colorScheme!.primary) : colorScheme!.onSurface.withOpacity(0.12));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getTextColor(MaterialButton button)
    {
        if (!button.enabled)
        {
            return getDisabledTextColor(button);
        }
        if (button.textColor is not null)
        {
            return button.textColor!;
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
                {
                    return Equals(getBrightness(button), Brightness.dark) ? Colors.white : Colors.black87;
                }
            case ButtonTextTheme.accent:
                {
                    return colorScheme!.secondary;
                }
            case ButtonTextTheme.primary:
                {
                    global::Doroti.Ui.Color? fillColor = getFillColor(button);
                    var fillIsDark = (fillColor is not null) ? Equals(ThemeData.estimateBrightnessForColor(fillColor), Brightness.dark) : Equals(getBrightness(button), Brightness.dark);
                    return fillIsDark ? Colors.white : Colors.black;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getSplashColor(MaterialButton button)
    {
        if (button.splashColor is not null)
        {
            return button.splashColor!;
        }
        if (_splashColor is not null)
        {
            switch (getTextTheme(button))
            {
                case ButtonTextTheme.normal:
                case ButtonTextTheme.accent:
                    {
                        return _splashColor;
                    }
                case ButtonTextTheme.primary:
                    {
                        break;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        return getTextColor(button).withOpacity(0.12);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getFocusColor(MaterialButton button)
    {
        return (button.focusColor ?? _focusColor) ?? getTextColor(button).withOpacity(0.12);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getHoverColor(MaterialButton button)
    {
        return (button.hoverColor ?? _hoverColor) ?? getTextColor(button).withOpacity(0.04);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getHighlightColor(MaterialButton button)
    {
        if (button.highlightColor is not null)
        {
            return button.highlightColor!;
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
            case ButtonTextTheme.accent:
                {
                    return _highlightColor ?? getTextColor(button).withOpacity(0.16);
                }
            case ButtonTextTheme.primary:
                {
                    return Colors.transparent;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>(button.elevation ?? 2.0);
    public virtual double getFocusElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>(button.focusElevation ?? 4.0);
    public virtual double getHoverElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>(button.hoverElevation ?? 4.0);
    public virtual double getHighlightElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>(button.highlightElevation ?? 8.0);
    public virtual double getDisabledElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>(button.disabledElevation ?? 0.0);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry getPadding(MaterialButton button)
    {
        return (button.padding ?? _padding) ?? (getTextTheme(button) switch { ButtonTextTheme.normal => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.accent => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.primary => EdgeInsets.CreateSymmetric(horizontal: 24.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.ShapeBorder getShape(MaterialButton button) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(button.shape ?? shape);
    public virtual Duration getAnimationDuration(MaterialButton button)
    {
        return button.animationDuration ?? ConstantsLibrary.kThemeChangeDuration;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints getConstraints(MaterialButton button) => constraints;
    public virtual MaterialTapTargetSize getMaterialTapTargetSize(MaterialButton button)
    {
        return (button.materialTapTargetSize ?? _materialTapTargetSize) ?? MaterialTapTargetSize.padded;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonThemeData copyWith(ButtonTextTheme? textTheme = null, ButtonBarLayoutBehavior? layoutBehavior = null, double? minWidth = null, double? height = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool? alignedDropdown = null, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null)
    {
        return new ButtonThemeData(textTheme: textTheme ?? this.textTheme, layoutBehavior: layoutBehavior ?? this.layoutBehavior, minWidth: minWidth ?? this.minWidth, height: height ?? this.height, padding: padding ?? this.padding, shape: shape ?? this.shape, alignedDropdown: alignedDropdown ?? this.alignedDropdown, buttonColor: buttonColor ?? _buttonColor, disabledColor: disabledColor ?? _disabledColor, focusColor: focusColor ?? _focusColor, hoverColor: hoverColor ?? _hoverColor, highlightColor: highlightColor ?? _highlightColor, splashColor: splashColor ?? _splashColor, colorScheme: colorScheme ?? this.colorScheme, materialTapTargetSize: materialTapTargetSize ?? _materialTapTargetSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ButtonThemeData;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ButtonThemeData) && Equals(__other.textTheme, textTheme) && (__other.minWidth == minWidth) && (__other.height == height) && Equals(__other.padding, padding) && Equals(__other.shape, shape) && (__other.alignedDropdown == alignedDropdown) && Equals(__other._buttonColor, _buttonColor) && Equals(__other._disabledColor, _disabledColor) && Equals(__other._focusColor, _focusColor) && Equals(__other._hoverColor, _hoverColor) && Equals(__other._highlightColor, _highlightColor) && Equals(__other._splashColor, _splashColor) && Equals(__other.colorScheme, colorScheme) && Equals(__other._materialTapTargetSize, _materialTapTargetSize);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(textTheme, minWidth, height, padding, shape, alignedDropdown, _buttonColor, _disabledColor, _focusColor, _hoverColor, _highlightColor, _splashColor, colorScheme, _materialTapTargetSize));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultTheme = new ButtonThemeData();
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<ButtonTextTheme>("textTheme", textTheme, defaultValue: defaultTheme.textTheme));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minWidth", minWidth, defaultValue: defaultTheme.minWidth));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", height, defaultValue: defaultTheme.height));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding, defaultValue: defaultTheme.padding));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: defaultTheme.shape));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alignedDropdown", value: alignedDropdown, defaultValue: defaultTheme.alignedDropdown, ifTrue: "dropdown width matches button"));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("buttonColor", _buttonColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", _disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", _focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", _hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", _highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", _splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ColorScheme>("colorScheme", colorScheme, defaultValue: defaultTheme.colorScheme));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", _materialTapTargetSize, defaultValue: null));
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

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
        this.data = new ButtonThemeData(textTheme: textTheme, minWidth: minWidth, height: height, padding: padding, shape: shape, alignedDropdown: alignedDropdown, layoutBehavior: layoutBehavior, buttonColor: buttonColor, disabledColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, colorScheme: colorScheme, materialTapTargetSize: materialTapTargetSize);
        System.Diagnostics.Debug.Assert((minWidth >= 0.0));
        System.Diagnostics.Debug.Assert((height >= 0.0));
    }

    public static ButtonTheme CreateFromButtonThemeData(global::Doroti.Framework.Foundation.Key? key = null, ButtonThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        var __instance = new ButtonTheme(key: key, child: child);
        __instance.data = data;
        return __instance;
    }

    public static ButtonThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ButtonTheme? inheritedButtonTheme = ((ButtonTheme?)context.dependOnInheritedWidgetOfExactType<ButtonTheme>());
        ButtonThemeData? buttonThemeLocal = inheritedButtonTheme?.data;
        if ((buttonThemeLocal?.colorScheme is null))
        {
            ThemeData theme = ((ThemeData)Theme.of(context));
            buttonThemeLocal ??= ((ThemeData)theme).buttonTheme;
            if ((((ButtonThemeData)buttonThemeLocal).colorScheme is null))
            {
                buttonThemeLocal = buttonThemeLocal.copyWith(colorScheme: (((ThemeData)theme).buttonTheme.colorScheme ?? ((ThemeData)theme).colorScheme));
                DartRuntimePrimitives.Assert(() => (((ButtonThemeData)buttonThemeLocal).colorScheme is not null));
            }
        }
        return buttonThemeLocal!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)CreateFromButtonThemeData(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!Equals(this.data, ((ButtonTheme)oldWidget).data)));
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
        this._buttonColor = buttonColor;
        this._disabledColor = disabledColor;
        this._focusColor = focusColor;
        this._hoverColor = hoverColor;
        this._highlightColor = highlightColor;
        this._splashColor = splashColor;
        this._padding = padding;
        this._shape = shape;
        this._materialTapTargetSize = materialTapTargetSize;
        System.Diagnostics.Debug.Assert((minWidth >= 0.0));
        System.Diagnostics.Debug.Assert((height >= 0.0));
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints
    {
        get
        {
            return new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(this.minWidth), minHeight: DartRuntimePrimitives.RequireValue(this.height));
        }
    }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>((this._padding ?? (this.textTheme switch { ButtonTextTheme.normal => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.accent => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.primary => EdgeInsets.CreateSymmetric(horizontal: 24.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>((this._shape ?? (this.textTheme switch { ButtonTextTheme.normal => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))), ButtonTextTheme.accent => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(2.0))), ButtonTextTheme.primary => new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(4.0))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    public virtual global::Doroti.Ui.Brightness getBrightness(MaterialButton button)
    {
        return (button.colorBrightness ?? this.colorScheme!.brightness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonTextTheme getTextTheme(MaterialButton button) => DartRuntimePrimitives.ConvertValue<ButtonTextTheme>((button.textTheme ?? this.textTheme));
    public virtual global::Doroti.Ui.Color getDisabledTextColor(MaterialButton button)
    {
        return ((global::Doroti.Ui.Color)((button.textColor ?? button.disabledTextColor) ?? this.colorScheme!.onSurface.withOpacity(0.38)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getDisabledFillColor(MaterialButton button)
    {
        return ((global::Doroti.Ui.Color)((button.disabledColor ?? this._disabledColor) ?? this.colorScheme!.onSurface.withOpacity(0.38)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color? getFillColor(MaterialButton button)
    {
        global::Doroti.Ui.Color? fillColor = ((global::Doroti.Ui.Color?)(button.enabled ? button.color : button.disabledColor));
        if ((fillColor is not null))
        {
            return ((global::Doroti.Ui.Color?)fillColor);
        }
        if ((Equals(DartRuntimePrimitives.RuntimeType(button), typeof(MaterialButton))))
        {
            return null;
        }
        if ((button.enabled && (this._buttonColor is not null)))
        {
            return ((global::Doroti.Ui.Color?)this._buttonColor);
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
            case ButtonTextTheme.accent:
                {
                    return ((global::Doroti.Ui.Color?)(button.enabled ? this.colorScheme!.primary : getDisabledFillColor(button)));
                }
            case ButtonTextTheme.primary:
                {
                    return ((global::Doroti.Ui.Color?)(button.enabled ? (this._buttonColor ?? this.colorScheme!.primary) : this.colorScheme!.onSurface.withOpacity(0.12)));
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
            return ((global::Doroti.Ui.Color)getDisabledTextColor(button));
        }
        if ((button.textColor is not null))
        {
            return ((global::Doroti.Ui.Color)button.textColor!);
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
                {
                    return ((global::Doroti.Ui.Color)((Equals(getBrightness(button), Brightness.dark)) ? Colors.white : Colors.black87));
                }
            case ButtonTextTheme.accent:
                {
                    return ((global::Doroti.Ui.Color)this.colorScheme!.secondary);
                }
            case ButtonTextTheme.primary:
                {
                    global::Doroti.Ui.Color? fillColor = ((global::Doroti.Ui.Color?)getFillColor(button));
                    var fillIsDark = ((fillColor is not null) ? (Equals(ThemeData.estimateBrightnessForColor(fillColor), Brightness.dark)) : (Equals(getBrightness(button), Brightness.dark)));
                    return ((global::Doroti.Ui.Color)(fillIsDark ? Colors.white : Colors.black));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getSplashColor(MaterialButton button)
    {
        if ((button.splashColor is not null))
        {
            return ((global::Doroti.Ui.Color)button.splashColor!);
        }
        if ((this._splashColor is not null))
        {
            switch (getTextTheme(button))
            {
                case ButtonTextTheme.normal:
                case ButtonTextTheme.accent:
                    {
                        return ((global::Doroti.Ui.Color)this._splashColor);
                    }
                case ButtonTextTheme.primary:
                    {
                        break;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        return ((global::Doroti.Ui.Color)getTextColor(button).withOpacity(0.12));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getFocusColor(MaterialButton button)
    {
        return ((global::Doroti.Ui.Color)((button.focusColor ?? this._focusColor) ?? getTextColor(button).withOpacity(0.12)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getHoverColor(MaterialButton button)
    {
        return ((global::Doroti.Ui.Color)((button.hoverColor ?? this._hoverColor) ?? getTextColor(button).withOpacity(0.04)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color getHighlightColor(MaterialButton button)
    {
        if ((button.highlightColor is not null))
        {
            return ((global::Doroti.Ui.Color)button.highlightColor!);
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
            case ButtonTextTheme.accent:
                {
                    return ((global::Doroti.Ui.Color)(this._highlightColor ?? getTextColor(button).withOpacity(0.16)));
                }
            case ButtonTextTheme.primary:
                {
                    return ((global::Doroti.Ui.Color)Colors.transparent);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>((button.elevation ?? 2.0));
    public virtual double getFocusElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>((button.focusElevation ?? 4.0));
    public virtual double getHoverElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>((button.hoverElevation ?? 4.0));
    public virtual double getHighlightElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>((button.highlightElevation ?? 8.0));
    public virtual double getDisabledElevation(MaterialButton button) => DartRuntimePrimitives.ConvertValue<double>((button.disabledElevation ?? 0.0));
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry getPadding(MaterialButton button)
    {
        return ((button.padding ?? this._padding) ?? (getTextTheme(button) switch { ButtonTextTheme.normal => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.accent => EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.primary => EdgeInsets.CreateSymmetric(horizontal: 24.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Painting.ShapeBorder getShape(MaterialButton button) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.ShapeBorder>(((button.shape ?? (global::Doroti.Framework.Painting.ShapeBorder)this.shape)));
    public virtual Duration getAnimationDuration(MaterialButton button)
    {
        return (button.animationDuration ?? ConstantsLibrary.kThemeChangeDuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.BoxConstraints getConstraints(MaterialButton button) => this.constraints;
    public virtual MaterialTapTargetSize getMaterialTapTargetSize(MaterialButton button)
    {
        return ((button.materialTapTargetSize ?? this._materialTapTargetSize) ?? MaterialTapTargetSize.padded);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonThemeData copyWith(ButtonTextTheme? textTheme = null, ButtonBarLayoutBehavior? layoutBehavior = null, double? minWidth = null, double? height = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, bool? alignedDropdown = null, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null)
    {
        return new ButtonThemeData(textTheme: (textTheme ?? this.textTheme), layoutBehavior: (layoutBehavior ?? this.layoutBehavior), minWidth: (minWidth ?? this.minWidth), height: (height ?? this.height), padding: (padding ?? this.padding), shape: (shape ?? this.shape), alignedDropdown: (alignedDropdown ?? this.alignedDropdown), buttonColor: (buttonColor ?? this._buttonColor), disabledColor: (disabledColor ?? this._disabledColor), focusColor: (focusColor ?? this._focusColor), hoverColor: (hoverColor ?? this._hoverColor), highlightColor: (highlightColor ?? this._highlightColor), splashColor: (splashColor ?? this._splashColor), colorScheme: (colorScheme ?? this.colorScheme), materialTapTargetSize: (materialTapTargetSize ?? this._materialTapTargetSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ButtonThemeData;
        if (__other is null) return false;
        if ((!Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((__other is ButtonThemeData) && (Equals(((ButtonThemeData)((ButtonThemeData)__other)).textTheme, this.textTheme))) && (((ButtonThemeData)((ButtonThemeData)__other)).minWidth == this.minWidth)) && (((ButtonThemeData)((ButtonThemeData)__other)).height == this.height)) && (Equals(((ButtonThemeData)((ButtonThemeData)__other)).padding, this.padding))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other)).shape, this.shape))) && (((ButtonThemeData)((ButtonThemeData)__other)).alignedDropdown == this.alignedDropdown)) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._buttonColor, this._buttonColor))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._disabledColor, this._disabledColor))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._focusColor, this._focusColor))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._hoverColor, this._hoverColor))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._highlightColor, this._highlightColor))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._splashColor, this._splashColor))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other)).colorScheme, this.colorScheme))) && (Equals(((ButtonThemeData)((ButtonThemeData)__other))._materialTapTargetSize, this._materialTapTargetSize)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.textTheme, this.minWidth, this.height, this.padding, this.shape, this.alignedDropdown, this._buttonColor, this._disabledColor, this._focusColor, this._hoverColor, this._highlightColor, this._splashColor, this.colorScheme, this._materialTapTargetSize));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultTheme = new ButtonThemeData();
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<ButtonTextTheme>("textTheme", this.textTheme, defaultValue: ((ButtonThemeData)defaultTheme).textTheme));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minWidth", this.minWidth, defaultValue: ((ButtonThemeData)defaultTheme).minWidth));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: ((ButtonThemeData)defaultTheme).height));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: ((ButtonThemeData)defaultTheme).padding));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: ((ButtonThemeData)defaultTheme).shape));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("alignedDropdown", value: this.alignedDropdown, defaultValue: ((ButtonThemeData)defaultTheme).alignedDropdown, ifTrue: "dropdown width matches button"));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("buttonColor", this._buttonColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", this._disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", this._focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", this._hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", this._highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", this._splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ColorScheme>("colorScheme", this.colorScheme, defaultValue: ((ButtonThemeData)defaultTheme).colorScheme));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this._materialTapTargetSize, defaultValue: null));
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

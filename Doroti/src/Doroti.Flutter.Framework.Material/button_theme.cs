// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/button_theme.dart
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

public class ButtonTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual ButtonThemeData data { get; private set; } = default!;

    public ButtonTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, ButtonTextTheme textTheme = ButtonTextTheme.normal, ButtonBarLayoutBehavior layoutBehavior = ButtonBarLayoutBehavior.padded, double minWidth = 88.0, double height = 36.0, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, bool alignedDropdown = false, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = new ButtonThemeData(textTheme: textTheme, minWidth: minWidth, height: height, padding: padding, shape: shape, alignedDropdown: alignedDropdown, layoutBehavior: layoutBehavior, buttonColor: buttonColor, disabledColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, colorScheme: colorScheme, materialTapTargetSize: materialTapTargetSize);
        System.Diagnostics.Debug.Assert((minWidth >= 0.0));
        System.Diagnostics.Debug.Assert((height >= 0.0));
    }

    public static ButtonTheme CreateFromButtonThemeData(global::Doroti.Generated.Framework.Foundation.Key? key = null, ButtonThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        var __instance = new ButtonTheme(key: key, child: child);
        __instance.data = data;
        return __instance;
    }

    public static ButtonThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ButtonTheme? inheritedButtonTheme__4568 = ((ButtonTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ButtonTheme>());
        ButtonThemeData? buttonTheme__4680 = inheritedButtonTheme__4568?.data;
        if ((buttonTheme__4680?.colorScheme is null))
        {
            ThemeData theme__4847 = ((ThemeData)(object?)Theme.of(context));
            buttonTheme__4680 ??= ((ThemeData)theme__4847).buttonTheme;
            if ((((ButtonThemeData)buttonTheme__4680).colorScheme is null))
            {
                buttonTheme__4680 = buttonTheme__4680.copyWith(colorScheme: (((ThemeData)theme__4847).buttonTheme.colorScheme ?? ((ThemeData)theme__4847).colorScheme));
                DartRuntimePrimitives.Assert(() => (((ButtonThemeData)buttonTheme__4680).colorScheme is not null));
            }
        }
        return buttonTheme__4680!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)ButtonTheme.CreateFromButtonThemeData(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ButtonTheme)oldWidget).data)));
}

public class ButtonThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual double minWidth { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual ButtonTextTheme textTheme { get; private set; } = default!;
    public virtual ButtonBarLayoutBehavior layoutBehavior { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? _padding { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? _shape { get; private set; }
    public virtual bool alignedDropdown { get; private set; } = default!;
    internal virtual Color? _buttonColor { get; private set; }
    internal virtual Color? _disabledColor { get; private set; }
    internal virtual Color? _focusColor { get; private set; }
    internal virtual Color? _hoverColor { get; private set; }
    internal virtual Color? _highlightColor { get; private set; }
    internal virtual Color? _splashColor { get; private set; }
    public virtual ColorScheme? colorScheme { get; private set; }
    internal virtual MaterialTapTargetSize? _materialTapTargetSize { get; private set; }

    public ButtonThemeData(ButtonTextTheme textTheme = ButtonTextTheme.normal, double minWidth = 88.0, double height = 36.0, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, ButtonBarLayoutBehavior layoutBehavior = ButtonBarLayoutBehavior.padded, bool alignedDropdown = false, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null)
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

    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints
    {
        get
        {
            return new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: DartRuntimePrimitives.RequireValue(this.minWidth), minHeight: DartRuntimePrimitives.RequireValue(this.height));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>((this._padding ?? (this.textTheme switch { ButtonTextTheme.normal => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.accent => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.primary => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>((this._shape ?? (this.textTheme switch { ButtonTextTheme.normal => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0))), ButtonTextTheme.accent => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(2.0))), ButtonTextTheme.primary => new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(4.0))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
    public virtual global::Doroti.Flutter.Ui.Brightness getBrightness(MaterialButton button)
    {
        return (button.colorBrightness ?? this.colorScheme!.brightness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonTextTheme getTextTheme(MaterialButton button) => DartRuntimePrimitives.ConvertValue<ButtonTextTheme>((button.textTheme ?? this.textTheme));
    public virtual global::Doroti.Flutter.Ui.Color getDisabledTextColor(MaterialButton button)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)((button.textColor ?? button.disabledTextColor) ?? this.colorScheme!.onSurface.withOpacity(0.38)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color getDisabledFillColor(MaterialButton button)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)((button.disabledColor ?? this._disabledColor) ?? this.colorScheme!.onSurface.withOpacity(0.38)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color? getFillColor(MaterialButton button)
    {
        global::Doroti.Flutter.Ui.Color? fillColor__16244 = ((global::Doroti.Flutter.Ui.Color?)(object?)(button.enabled ? button.color : button.disabledColor));
        if ((fillColor__16244 is not null))
        {
            return ((global::Doroti.Flutter.Ui.Color?)(object?)fillColor__16244);
        }
        if ((object.Equals(DartRuntimePrimitives.RuntimeType(button), typeof(MaterialButton))))
        {
            return null;
        }
        if ((button.enabled && (this._buttonColor is not null)))
        {
            return ((global::Doroti.Flutter.Ui.Color?)(object?)this._buttonColor);
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
            case ButtonTextTheme.accent:
                {
                    return ((global::Doroti.Flutter.Ui.Color?)(object?)(button.enabled ? this.colorScheme!.primary : getDisabledFillColor(button)));
                }
            case ButtonTextTheme.primary:
                {
                    return ((global::Doroti.Flutter.Ui.Color?)(object?)(button.enabled ? (this._buttonColor ?? this.colorScheme!.primary) : this.colorScheme!.onSurface.withOpacity(0.12)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color getTextColor(MaterialButton button)
    {
        if (!button.enabled)
        {
            return ((global::Doroti.Flutter.Ui.Color)(object?)getDisabledTextColor(button));
        }
        if ((button.textColor is not null))
        {
            return ((global::Doroti.Flutter.Ui.Color)(object?)button.textColor!);
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
                {
                    return ((global::Doroti.Flutter.Ui.Color)(object?)((object.Equals(getBrightness(button), Brightness.dark)) ? Colors.white : Colors.black87));
                }
            case ButtonTextTheme.accent:
                {
                    return ((global::Doroti.Flutter.Ui.Color)(object?)this.colorScheme!.secondary);
                }
            case ButtonTextTheme.primary:
                {
                    global::Doroti.Flutter.Ui.Color? fillColor__18198 = ((global::Doroti.Flutter.Ui.Color?)(object?)getFillColor(button));
                    var fillIsDark__18246 = ((fillColor__18198 is not null) ? (object.Equals(ThemeData.estimateBrightnessForColor(fillColor__18198), Brightness.dark)) : (object.Equals(getBrightness(button), Brightness.dark)));
                    return ((global::Doroti.Flutter.Ui.Color)(object?)(fillIsDark__18246 ? Colors.white : Colors.black));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color getSplashColor(MaterialButton button)
    {
        if ((button.splashColor is not null))
        {
            return ((global::Doroti.Flutter.Ui.Color)(object?)button.splashColor!);
        }
        if ((this._splashColor is not null))
        {
            switch (getTextTheme(button))
            {
                case ButtonTextTheme.normal:
                case ButtonTextTheme.accent:
                    {
                        return ((global::Doroti.Flutter.Ui.Color)(object?)this._splashColor);
                    }
                case ButtonTextTheme.primary:
                    {
                        break;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
        return ((global::Doroti.Flutter.Ui.Color)(object?)getTextColor(button).withOpacity(0.12));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color getFocusColor(MaterialButton button)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)((button.focusColor ?? this._focusColor) ?? getTextColor(button).withOpacity(0.12)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color getHoverColor(MaterialButton button)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)((button.hoverColor ?? this._hoverColor) ?? getTextColor(button).withOpacity(0.04)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.Color getHighlightColor(MaterialButton button)
    {
        if ((button.highlightColor is not null))
        {
            return ((global::Doroti.Flutter.Ui.Color)(object?)button.highlightColor!);
        }
        switch (getTextTheme(button))
        {
            case ButtonTextTheme.normal:
            case ButtonTextTheme.accent:
                {
                    return ((global::Doroti.Flutter.Ui.Color)(object?)(this._highlightColor ?? getTextColor(button).withOpacity(0.16)));
                }
            case ButtonTextTheme.primary:
                {
                    return ((global::Doroti.Flutter.Ui.Color)(object?)Colors.transparent);
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
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry getPadding(MaterialButton button)
    {
        return ((button.padding ?? this._padding) ?? (getTextTheme(button) switch { ButtonTextTheme.normal => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.accent => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16.0), ButtonTextTheme.primary => global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 24.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder getShape(MaterialButton button) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.ShapeBorder>(((button.shape ?? (global::Doroti.Generated.Framework.Painting.ShapeBorder)this.shape)));
    public virtual Duration getAnimationDuration(MaterialButton button)
    {
        return (button.animationDuration ?? ConstantsLibrary.kThemeChangeDuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints getConstraints(MaterialButton button) => this.constraints;
    public virtual MaterialTapTargetSize getMaterialTapTargetSize(MaterialButton button)
    {
        return ((button.materialTapTargetSize ?? this._materialTapTargetSize) ?? MaterialTapTargetSize.padded);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonThemeData copyWith(ButtonTextTheme? textTheme = null, ButtonBarLayoutBehavior? layoutBehavior = null, double? minWidth = null, double? height = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, bool? alignedDropdown = null, Color? buttonColor = null, Color? disabledColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, ColorScheme? colorScheme = null, MaterialTapTargetSize? materialTapTargetSize = null)
    {
        return new ButtonThemeData(textTheme: (textTheme ?? this.textTheme), layoutBehavior: (layoutBehavior ?? this.layoutBehavior), minWidth: (minWidth ?? this.minWidth), height: (height ?? this.height), padding: (padding ?? this.padding), shape: (shape ?? this.shape), alignedDropdown: (alignedDropdown ?? this.alignedDropdown), buttonColor: (buttonColor ?? this._buttonColor), disabledColor: (disabledColor ?? this._disabledColor), focusColor: (focusColor ?? this._focusColor), hoverColor: (hoverColor ?? this._hoverColor), highlightColor: (highlightColor ?? this._highlightColor), splashColor: (splashColor ?? this._splashColor), colorScheme: (colorScheme ?? this.colorScheme), materialTapTargetSize: (materialTapTargetSize ?? this._materialTapTargetSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ButtonThemeData;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((__other is ButtonThemeData) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other)).textTheme, this.textTheme))) && (((ButtonThemeData)((ButtonThemeData)__other)).minWidth == this.minWidth)) && (((ButtonThemeData)((ButtonThemeData)__other)).height == this.height)) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other)).padding, this.padding))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other)).shape, this.shape))) && (((ButtonThemeData)((ButtonThemeData)__other)).alignedDropdown == this.alignedDropdown)) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._buttonColor, this._buttonColor))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._disabledColor, this._disabledColor))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._focusColor, this._focusColor))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._hoverColor, this._hoverColor))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._highlightColor, this._highlightColor))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._splashColor, this._splashColor))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other)).colorScheme, this.colorScheme))) && (object.Equals(((ButtonThemeData)((ButtonThemeData)__other))._materialTapTargetSize, this._materialTapTargetSize)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.textTheme, this.minWidth, this.height, this.padding, this.shape, this.alignedDropdown, this._buttonColor, this._disabledColor, this._focusColor, this._hoverColor, this._highlightColor, this._splashColor, this.colorScheme, this._materialTapTargetSize));
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultTheme__27797 = new ButtonThemeData();
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<ButtonTextTheme>("textTheme", this.textTheme, defaultValue: ((ButtonThemeData)defaultTheme__27797).textTheme));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("minWidth", this.minWidth, defaultValue: ((ButtonThemeData)defaultTheme__27797).minWidth));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: ((ButtonThemeData)defaultTheme__27797).height));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: ((ButtonThemeData)defaultTheme__27797).padding));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: ((ButtonThemeData)defaultTheme__27797).shape));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("alignedDropdown", value: this.alignedDropdown, defaultValue: ((ButtonThemeData)defaultTheme__27797).alignedDropdown, ifTrue: "dropdown width matches button"));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("buttonColor", this._buttonColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledColor", this._disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("focusColor", this._focusColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("hoverColor", this._hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("highlightColor", this._highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("splashColor", this._splashColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ColorScheme>("colorScheme", this.colorScheme, defaultValue: ((ButtonThemeData)defaultTheme__27797).colorScheme));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<MaterialTapTargetSize>("materialTapTargetSize", this._materialTapTargetSize, defaultValue: null));
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

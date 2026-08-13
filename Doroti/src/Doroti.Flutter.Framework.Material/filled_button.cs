// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/filled_button.dart
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

internal enum _FilledButtonVariant__filled_button
{
    filled,
    tonal
}

public class FilledButton : ButtonStyleButton
{
    internal virtual _FilledButtonVariant__filled_button _variant { get; private set; } = default!;
    internal virtual bool _addPadding { get; private set; } = default!;

    public FilledButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? child = default!) : base(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: child)
    {
        this._variant = _FilledButtonVariant__filled_button.filled;
        this._addPadding = false;
    }

    public static FilledButton CreateIcon(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new FilledButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: default!);
        __instance._variant = _FilledButtonVariant__filled_button.filled;
        __instance._addPadding = (icon is not null);
        return __instance;
    }

    public static FilledButton CreateTonal(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? child = default!)
    {
        var __instance = new FilledButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: child);
        __instance._variant = _FilledButtonVariant__filled_button.tonal;
        __instance._addPadding = false;
        return __instance;
    }

    public static FilledButton CreateTonalIcon(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new FilledButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: default!);
        __instance._variant = _FilledButtonVariant__filled_button.tonal;
        __instance._addPadding = (icon is not null);
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, IconAlignment? iconAlignment = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? overlayColorProp__9474 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)((foregroundColor, overlayColor) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(null), (_, global::Doroti.Flutter.Ui.Color { a: 0.0 } __object9574) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(overlayColor)), (_, global::Doroti.Flutter.Ui.Color color__9660) => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = color__9660.withOpacity(0.1), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = color__9660.withOpacity(0.08), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = color__9660.withOpacity(0.1) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Flutter.Ui.Color?>()), (global::Doroti.Flutter.Ui.Color color__9689, _) => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = color__9689.withOpacity(0.1), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = color__9689.withOpacity(0.08), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = color__9689.withOpacity(0.1) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Flutter.Ui.Color?>()) }));
        return new ButtonStyle(textStyle: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle>(textStyle), backgroundColor: ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), overlayColor: overlayColorProp__9474, shadowColor: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Color>(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Color>(surfaceTintColor), iconColor: ButtonStyleButton.defaultColor(iconColor, disabledIconColor), iconSize: ButtonStyleButton.allOrNull<double?>(iconSize), iconAlignment: iconAlignment, elevation: ButtonStyleButton.allOrNull(elevation), padding: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(padding), minimumSize: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Size>(minimumSize), fixedSize: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Size>(fixedSize), maximumSize: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Size>(maximumSize), side: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.BorderSide>(side), shape: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(shape), mouseCursor: global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Generated.Framework.Services.MouseCursor?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabledMouseCursor, [global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory, backgroundBuilder: backgroundBuilder, foregroundBuilder: foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ButtonStyle buttonStyle__16652 = (this._variant switch { _FilledButtonVariant__filled_button.filled => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledButtonDefaultsM3__filled_button(context)), _FilledButtonVariant__filled_button.tonal => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledTonalButtonDefaultsM3__filled_button(context)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if (this._addPadding)
        {
            bool useMaterial3__16880 = Theme.of(context).useMaterial3;
            double defaultFontSize__16946 = (buttonStyle__16652.textStyle?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
            double effectiveTextScale__17066 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__16946) / 14.0);
            global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry scaledPadding__17193 = (useMaterial3__16880 ? ButtonStyleButton.scaledPadding(new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(16, 0, 24, 0), new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(8, 0, 12, 0), new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(4, 0, 6, 0), effectiveTextScale__17066) : ButtonStyleButton.scaledPadding(new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(12, 0, 16, 0), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(8, 0, 4, 0), effectiveTextScale__17066));
            return buttonStyle__16652.copyWith(padding: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(scaledPadding__17193));
        }
        return buttonStyle__16652;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return FilledButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Filled_buttonLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__18243 = Theme.of(context);
        double defaultFontSize__18285 = (theme__18243.textTheme.labelLarge?.fontSize ?? 14.0);
        double effectiveTextScale__18364 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__18285) / 14.0);
        var padding1x__18457 = (theme__18243.useMaterial3 ? 24.0 : 16.0);
        return ButtonStyleButton.scaledPadding(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: padding1x__18457), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: (padding1x__18457 / 2L)), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: ((padding1x__18457 / 2L) / 2L)), effectiveTextScale__18364);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FilledButtonWithIconChild__filled_button : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual ButtonStyle? buttonStyle { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }

    internal _FilledButtonWithIconChild__filled_button(global::Doroti.Generated.Framework.Widgets.Widget label, global::Doroti.Generated.Framework.Widgets.Widget icon, ButtonStyle? buttonStyle, IconAlignment? iconAlignment)
    {
        this.label = label;
        this.icon = icon;
        this.buttonStyle = buttonStyle;
        this.iconAlignment = iconAlignment;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        double defaultFontSize__19136 = (this.buttonStyle?.textStyle?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
        double scale__19253 = (Dart_uiLibrary.clampDouble((MediaQuery.textScalerOf(context).scale(defaultFontSize__19136) / 14.0), 1.0, 2.0) - 1.0);
        FilledButtonThemeData filledButtonTheme__19395 = FilledButtonTheme.of(context);
        IconAlignment effectiveIconAlignment__19470 = (((this.iconAlignment ?? filledButtonTheme__19395.style?.iconAlignment) ?? this.buttonStyle?.iconAlignment) ?? IconAlignment.start);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scale__19253)), children: ((object.Equals(effectiveIconAlignment__19470, IconAlignment.start)) ? new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: this.label)) } : new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: this.label)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.icon) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FilledButtonDefaultsM3__filled_button : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _FilledButtonDefaultsM3__filled_button(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?> textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle>(Theme.of(this.context).textTheme.labelLarge));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.12));
}
return (this._colors.primary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
return (this._colors.onPrimary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onPrimary.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onPrimary.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onPrimary.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return 0.0;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return 0.0;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return 1.0;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return 0.0;
}
return 0.0;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(Filled_buttonLibrary._scaledPadding(this.context)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Flutter.Ui.Size(64.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(18.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? iconColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>?)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onPrimary);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onPrimary);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onPrimary);
}
return (this._colors.onPrimary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

internal class _FilledTonalButtonDefaultsM3__filled_button : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _FilledTonalButtonDefaultsM3__filled_button(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?> textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle>(Theme.of(this.context).textTheme.labelLarge));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.12));
}
return (this._colors.secondaryContainer);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
return (this._colors.onSecondaryContainer);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSecondaryContainer.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSecondaryContainer.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSecondaryContainer.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return 0.0;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return 0.0;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return 1.0;
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return 0.0;
}
return 0.0;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(Filled_buttonLibrary._scaledPadding(this.context)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Flutter.Ui.Size(64.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(18.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? iconColor
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color>?)(object?)WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSecondaryContainer);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSecondaryContainer);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSecondaryContainer);
}
return (this._colors.onSecondaryContainer);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

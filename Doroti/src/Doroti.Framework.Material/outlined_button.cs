// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/outlined_button.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class OutlinedButton : ButtonStyleButton
{
    internal virtual bool _addPadding { get; private set; } = default!;

    public OutlinedButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? child = default!) : base(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: child)
    {
        this._addPadding = false;
    }

    public static OutlinedButton CreateIcon(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new OutlinedButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: default!);
        __instance._addPadding = (icon is not null);
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, IconAlignment? iconAlignment = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColorProp__7371 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)((backgroundColor, disabledBackgroundColor) switch { (_, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(backgroundColor)), (_, _) => ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor) }));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp__7671 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)((foregroundColor, overlayColor) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(null), (_, global::Doroti.Ui.Color { a: 0.0 } __object7771) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(overlayColor)), (_, global::Doroti.Ui.Color color__7857) => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = color__7857.withOpacity(0.1), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = color__7857.withOpacity(0.08), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = color__7857.withOpacity(0.1) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()), (global::Doroti.Ui.Color color__7886, _) => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = color__7886.withOpacity(0.1), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = color__7886.withOpacity(0.08), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = color__7886.withOpacity(0.1) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()) }));
        return new ButtonStyle(textStyle: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.TextStyle>(textStyle), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), backgroundColor: backgroundColorProp__7371, overlayColor: overlayColorProp__7671, shadowColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(surfaceTintColor), iconColor: ButtonStyleButton.defaultColor(iconColor, disabledIconColor), iconSize: ButtonStyleButton.allOrNull<double?>(iconSize), iconAlignment: iconAlignment, elevation: ButtonStyleButton.allOrNull<double?>(elevation), padding: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(padding), minimumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(minimumSize), fixedSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(fixedSize), maximumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(maximumSize), side: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.BorderSide>(side), shape: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(shape), mouseCursor: global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Generated.Framework.Services.MouseCursor?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabledMouseCursor, [global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory, backgroundBuilder: backgroundBuilder, foregroundBuilder: foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__13929 = Theme.of(context);
        ColorScheme colorScheme__13978 = theme__13929.colorScheme;
        ButtonStyle buttonStyle__14033 = (theme__13929.useMaterial3 ? new _OutlinedButtonDefaultsM3__outlined_button(context) : OutlinedButton.styleFrom(foregroundColor: colorScheme__13978.primary, disabledForegroundColor: colorScheme__13978.onSurface.withOpacity(0.38), backgroundColor: Colors.transparent, disabledBackgroundColor: Colors.transparent, shadowColor: theme__13929.shadowColor, elevation: 0, textStyle: theme__13929.textTheme.labelLarge, padding: Outlined_buttonLibrary._scaledPadding(context), minimumSize: new global::Doroti.Ui.Size(64, 36), maximumSize: Size.infinite, side: new global::Doroti.Generated.Framework.Painting.BorderSide(color: colorScheme__13978.onSurface.withOpacity(0.12)), shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4))), enabledMouseCursor: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic), disabledMouseCursor: global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic, visualDensity: theme__13929.visualDensity, tapTargetSize: theme__13929.materialTapTargetSize, animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, splashFactory: InkRipple.splashFactory));
        if ((this._addPadding && theme__13929.useMaterial3))
        {
            double defaultFontSize__15371 = (buttonStyle__14033.textStyle?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
            double effectiveTextScale__15491 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__15371) / 14.0);
            global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry scaledPadding__15617 = ButtonStyleButton.scaledPadding(new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(16, 0, 24, 0), new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(8, 0, 12, 0), new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(4, 0, 6, 0), effectiveTextScale__15491);
            return buttonStyle__14033.copyWith(padding: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(scaledPadding__15617));
        }
        return buttonStyle__14033;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return OutlinedButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Outlined_buttonLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__16235 = Theme.of(context);
        var padding1x__16270 = (theme__16235.useMaterial3 ? 24.0 : 16.0);
        double defaultFontSize__16331 = (theme__16235.textTheme.labelLarge?.fontSize ?? 14.0);
        double effectiveTextScale__16410 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__16331) / 14.0);
        return ButtonStyleButton.scaledPadding(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: padding1x__16270), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: (padding1x__16270 / 2L)), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: ((padding1x__16270 / 2L) / 2L)), effectiveTextScale__16410);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _OutlinedButtonWithIconChild__outlined_button : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual ButtonStyle? buttonStyle { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }

    internal _OutlinedButtonWithIconChild__outlined_button(global::Doroti.Generated.Framework.Widgets.Widget label, global::Doroti.Generated.Framework.Widgets.Widget icon, ButtonStyle? buttonStyle, IconAlignment? iconAlignment)
    {
        this.label = label;
        this.icon = icon;
        this.buttonStyle = buttonStyle;
        this.iconAlignment = iconAlignment;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        double defaultFontSize__17132 = (this.buttonStyle?.textStyle?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
        double scale__17249 = (Dart_uiLibrary.clampDouble((MediaQuery.textScalerOf(context).scale(defaultFontSize__17132) / 14.0), 1.0, 2.0) - 1.0);
        OutlinedButtonThemeData outlinedButtonTheme__17393 = OutlinedButtonTheme.of(context);
        IconAlignment effectiveIconAlignment__17472 = (((this.iconAlignment ?? outlinedButtonTheme__17393.style?.iconAlignment) ?? this.buttonStyle?.iconAlignment) ?? IconAlignment.start);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scale__17249)), children: ((object.Equals(effectiveIconAlignment__17472, IconAlignment.start)) ? new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: this.label)) } : new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: this.label)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.icon) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _OutlinedButtonDefaultsM3__outlined_button : ButtonStyle
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

    internal _OutlinedButtonDefaultsM3__outlined_button(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?> textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle?>(Theme.of(this.context).textTheme.labelLarge));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
return (this._colors.primary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.primary.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.primary.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.primary.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(0.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(Outlined_buttonLibrary._scaledPadding(this.context)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(64.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(18.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? iconColor
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
    return (this._colors.primary);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.primary);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.primary);
}
return (this._colors.primary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide>? side => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12)));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.primary));
}
return (new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.outline));
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

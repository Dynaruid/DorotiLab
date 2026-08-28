// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/elevated_button.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class ElevatedButton : ButtonStyleButton
{
    internal virtual bool _addPadding { get; private set; } = default!;

    public ElevatedButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: child)
    {
        this._addPadding = false;
    }

    public static ElevatedButton CreateIcon(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = Clip.none, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Widgets.Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new ElevatedButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: default!);
        __instance._addPadding = (icon is not null);
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, IconAlignment? iconAlignment = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)(object?)((foregroundColor, overlayColor) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(null), (_, global::Doroti.Ui.Color { a: 0.0 } __object7732) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(overlayColor)), (_, global::Doroti.Ui.Color color) => global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = color.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = color.withOpacity(0.08), [global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = color.withOpacity(0.1) }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()), (global::Doroti.Ui.Color colorLocal, _) => global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = colorLocal.withOpacity(0.1), [global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = colorLocal.withOpacity(0.08), [global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = colorLocal.withOpacity(0.1) }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()) }));
        global::Doroti.Framework.Widgets.WidgetStateProperty<double>? elevationValue = default!;
        if ((elevation is not null))
        {
            double elevation__value8154 = DartRuntimePrimitives.RequireValue(elevation);
            elevationValue = global::Doroti.Framework.Widgets.WidgetStateProperty<double>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, double> { [global::Doroti.Framework.Widgets.WidgetState.disabled.asConstraint()] = 0, [global::Doroti.Framework.Widgets.WidgetState.pressed.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation__value8154) + 6L), [global::Doroti.Framework.Widgets.WidgetState.hovered.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation__value8154) + 2L), [global::Doroti.Framework.Widgets.WidgetState.focused.asConstraint()] = (DartRuntimePrimitives.RequireValue(elevation__value8154) + 2L), [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = DartRuntimePrimitives.RequireValue(elevation__value8154) });
        }
        return new ButtonStyle(textStyle: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle>(textStyle), backgroundColor: ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), overlayColor: overlayColorProp, shadowColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(surfaceTintColor), iconColor: ButtonStyleButton.defaultColor(iconColor, disabledIconColor), iconSize: ButtonStyleButton.allOrNull<double?>(iconSize), iconAlignment: iconAlignment, elevation: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(elevationValue), padding: ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(padding), minimumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(minimumSize), fixedSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(fixedSize), maximumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(maximumSize), side: ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.BorderSide>(side), shape: ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.OutlinedBorder>(shape), mouseCursor: global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Framework.Services.MouseCursor?> { [global::Doroti.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabledMouseCursor, [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory, backgroundBuilder: backgroundBuilder, foregroundBuilder: foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        ButtonStyle buttonStyle = (theme.useMaterial3 ? new _ElevatedButtonDefaultsM3__elevated_button(context) : ElevatedButton.styleFrom(backgroundColor: colorSchemeLocal.primary, foregroundColor: colorSchemeLocal.onPrimary, disabledBackgroundColor: colorSchemeLocal.onSurface.withOpacity(0.12), disabledForegroundColor: colorSchemeLocal.onSurface.withOpacity(0.38), shadowColor: theme.shadowColor, elevation: 2, textStyle: theme.textTheme.labelLarge, padding: Elevated_buttonLibrary._scaledPadding(context), minimumSize: new global::Doroti.Ui.Size(64, 36), maximumSize: Size.infinite, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4))), enabledMouseCursor: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Framework.Services.SystemMouseCursors.click : global::Doroti.Framework.Services.SystemMouseCursors.basic), disabledMouseCursor: global::Doroti.Framework.Services.SystemMouseCursors.basic, visualDensity: theme.visualDensity, tapTargetSize: theme.materialTapTargetSize, animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Framework.Painting.Alignment.center, splashFactory: InkRipple.splashFactory));
        if (this._addPadding)
        {
            double defaultFontSize = (buttonStyle.textStyle?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
            double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0);
            global::Doroti.Framework.Painting.EdgeInsetsGeometry scaledPaddingLocal = (theme.useMaterial3 ? ButtonStyleButton.scaledPadding(new global::Doroti.Framework.Painting.EdgeInsetsDirectional(16, 0, 24, 0), new global::Doroti.Framework.Painting.EdgeInsetsDirectional(8, 0, 12, 0), new global::Doroti.Framework.Painting.EdgeInsetsDirectional(4, 0, 6, 0), effectiveTextScale) : ButtonStyleButton.scaledPadding(new global::Doroti.Framework.Painting.EdgeInsetsDirectional(12, 0, 16, 0), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), new global::Doroti.Framework.Painting.EdgeInsetsDirectional(8, 0, 4, 0), effectiveTextScale));
            return buttonStyle.copyWith(padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(scaledPaddingLocal));
        }
        return buttonStyle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ElevatedButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Elevated_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        var padding1x = (theme.useMaterial3 ? 24.0 : 16.0);
        double defaultFontSize = (theme.textTheme.labelLarge?.fontSize ?? 14.0);
        double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0);
        return ButtonStyleButton.scaledPadding(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: padding1x), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: (padding1x / 2L)), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: ((padding1x / 2L) / 2L)), effectiveTextScale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ElevatedButtonWithIconChild__elevated_button : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual ButtonStyle? buttonStyle { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }

    internal _ElevatedButtonWithIconChild__elevated_button(global::Doroti.Framework.Widgets.Widget label, global::Doroti.Framework.Widgets.Widget icon, ButtonStyle? buttonStyle, IconAlignment? iconAlignment)
    {
        this.label = label;
        this.icon = icon;
        this.buttonStyle = buttonStyle;
        this.iconAlignment = iconAlignment;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        double defaultFontSize = (this.buttonStyle?.textStyle?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
        double scaleLocal = (Dart_uiLibrary.clampDouble((MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0), 1.0, 2.0) - 1.0);
        ElevatedButtonThemeData elevatedButtonTheme = ElevatedButtonTheme.of(context);
        IconAlignment effectiveIconAlignment = (((this.iconAlignment ?? elevatedButtonTheme.style?.iconAlignment) ?? this.buttonStyle?.iconAlignment) ?? IconAlignment.start);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scaleLocal)), children: ((object.Equals(effectiveIconAlignment, IconAlignment.start)) ? new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: this.label)) } : new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: this.label)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(this.icon) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ElevatedButtonDefaultsM3__elevated_button : ButtonStyle
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _ElevatedButtonDefaultsM3__elevated_button(global::Doroti.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Framework.Painting.Alignment.center)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle>(Theme.of(this.context).textTheme.labelLarge));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return (this._colors.onSurface.withOpacity(0.12));
        }
        return (this._colors.surfaceContainerLow);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return (this._colors.onSurface.withOpacity(0.38));
        }
        return (this._colors.primary);
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
        {
            return (this._colors.primary.withOpacity(0.1));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return (this._colors.primary.withOpacity(0.08));
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return (this._colors.primary.withOpacity(0.1));
        }
        return null;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(this._colors.shadow));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => WidgetStateProperty.resolveWith<double?>((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return 0.0;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
        {
            return 1.0;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return 3.0;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return 1.0;
        }
        return 1.0;
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(Elevated_buttonLibrary._scaledPadding(this.context)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(64.0, 40.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(18.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? iconColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return (this._colors.onSurface.withOpacity(0.38));
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return (this._colors.primary);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return (this._colors.primary);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return (this._colors.primary);
                }
                return (this._colors.primary);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(global::Doroti.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

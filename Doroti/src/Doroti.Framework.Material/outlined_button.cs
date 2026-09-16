// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/outlined_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class OutlinedButton : ButtonStyleButton
{
    internal virtual bool _addPadding { get; private set; } = default!;

    public OutlinedButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget? child = default!) : base(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: child)
    {
        _addPadding = false;
    }

    public static OutlinedButton CreateIcon(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget? icon = null, global::Doroti.Framework.Widgets.Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new OutlinedButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: icon is null ? label : new _OutlinedButtonWithIconChild__outlined_button(label: label, icon: icon, buttonStyle: style, iconAlignment: iconAlignment));
        __instance._addPadding = icon is not null;
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, IconAlignment? iconAlignment = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColorProp = (backgroundColor, disabledBackgroundColor) switch { (_, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(backgroundColor)), (_, _) => defaultColor(backgroundColor, disabledBackgroundColor) };
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp = (global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>?)((foregroundColor, overlayColor) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(null), (_, global::Doroti.Ui.Color { a: 0.0 } __object7771) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color?>(overlayColor)), (_, global::Doroti.Ui.Color color) => WidgetStateProperty<Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [WidgetState.pressed.asConstraint()] = color.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = color.withOpacity(0.08), [WidgetState.focused.asConstraint()] = color.withOpacity(0.1) }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()), (global::Doroti.Ui.Color colorLocal, _) => WidgetStateProperty<Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [WidgetState.pressed.asConstraint()] = colorLocal.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = colorLocal.withOpacity(0.08), [WidgetState.focused.asConstraint()] = colorLocal.withOpacity(0.1) }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()) });
        return new ButtonStyle(textStyle: allOrNull(textStyle), foregroundColor: defaultColor(foregroundColor, disabledForegroundColor), backgroundColor: backgroundColorProp, overlayColor: overlayColorProp, shadowColor: allOrNull(shadowColor), surfaceTintColor: allOrNull(surfaceTintColor), iconColor: defaultColor(iconColor, disabledIconColor), iconSize: allOrNull(iconSize), iconAlignment: iconAlignment, elevation: allOrNull(elevation), padding: allOrNull(padding), minimumSize: allOrNull(minimumSize), fixedSize: allOrNull(fixedSize), maximumSize: allOrNull(maximumSize), side: allOrNull(side), shape: allOrNull(shape), mouseCursor: WidgetStateProperty<MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Framework.Services.MouseCursor?> { [WidgetState.disabled.asConstraint()] = disabledMouseCursor, [WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory, backgroundBuilder: backgroundBuilder, foregroundBuilder: foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        ButtonStyle buttonStyle = new _OutlinedButtonDefaultsM3__outlined_button(context);
        if (_addPadding)
        {
            double defaultFontSize = buttonStyle.textStyle?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0;
            double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
            global::Doroti.Framework.Painting.EdgeInsetsGeometry scaledPaddingLocal = scaledPadding(new global::Doroti.Framework.Painting.EdgeInsetsDirectional(16, 0, 24, 0), new global::Doroti.Framework.Painting.EdgeInsetsDirectional(8, 0, 12, 0), new global::Doroti.Framework.Painting.EdgeInsetsDirectional(4, 0, 6, 0), effectiveTextScale);
            return buttonStyle.copyWith(padding: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(scaledPaddingLocal));
        }
        return buttonStyle;
    }

    public override ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return OutlinedButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Outlined_buttonLibrary
{
    internal static global::Doroti.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        var padding1x = 24.0;
        double defaultFontSize = theme.textTheme.labelLarge?.fontSize ?? 14.0;
        double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
        return ButtonStyleButton.scaledPadding(EdgeInsets.CreateSymmetric(horizontal: padding1x), EdgeInsets.CreateSymmetric(horizontal: padding1x / 2L), EdgeInsets.CreateSymmetric(horizontal: padding1x / 2L / 2L), effectiveTextScale);
    }
}

internal class _OutlinedButtonWithIconChild__outlined_button : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual ButtonStyle? buttonStyle { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }

    internal _OutlinedButtonWithIconChild__outlined_button(global::Doroti.Framework.Widgets.Widget label, global::Doroti.Framework.Widgets.Widget icon, ButtonStyle? buttonStyle, IconAlignment? iconAlignment)
    {
        this.label = label;
        this.icon = icon;
        this.buttonStyle = buttonStyle;
        this.iconAlignment = iconAlignment;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        double defaultFontSize = buttonStyle?.textStyle?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0;
        double scaleLocal = Dart_uiLibrary.clampDouble(MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0, 1.0, 2.0) - 1.0;
        OutlinedButtonThemeData outlinedButtonTheme = OutlinedButtonTheme.of(context);
        IconAlignment effectiveIconAlignment = ((iconAlignment ?? outlinedButtonTheme.style?.iconAlignment) ?? buttonStyle?.iconAlignment) ?? IconAlignment.start;
        return new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scaleLocal)), children: Equals(effectiveIconAlignment, IconAlignment.start) ? new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: label)) } : new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: label)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(icon) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _OutlinedButtonDefaultsM3__outlined_button : ButtonStyle
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
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _OutlinedButtonDefaultsM3__outlined_button(global::Doroti.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(Theme.of(context).textTheme.labelLarge));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        return _colors.primary;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.primary.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.primary.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.primary.withOpacity(0.1);
        }
        return null;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(Outlined_buttonLibrary._scaledPadding(context)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size>(new global::Doroti.Ui.Size(64.0, 40.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(18.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? iconColor
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return _colors.onSurface.withOpacity(0.38);
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.primary;
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.primary;
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.primary;
                }
                return _colors.primary;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size>(Size.infinite));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side => WidgetStateProperty.resolveWith<global::Doroti.Framework.Painting.BorderSide?>((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface.withOpacity(0.12));
        }
        if (states.Contains(WidgetState.focused))
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.primary);
        }
        return new global::Doroti.Framework.Painting.BorderSide(color: _colors.outline);
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/outlined_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class OutlinedButton : ButtonStyleButton
{
    internal virtual bool _addPadding { get; private set; } = default!;

    public OutlinedButton(Key? key = null, Action? onPressed = default!, Action? onLongPress = null, System.Action<bool>? onHover = null, System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, WidgetStatesController? statesController = null, Widget? child = default!) : base(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: child)
    {
        _addPadding = false;
    }

    public static OutlinedButton CreateIcon(Key? key = null, Action? onPressed = default!, Action? onLongPress = null, System.Action<bool>? onHover = null, System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, WidgetStatesController? statesController = null, Widget? icon = null, Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new OutlinedButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: icon is null ? label : new _OutlinedButtonWithIconChild__outlined_button(label: label, icon: icon, buttonStyle: style, iconAlignment: iconAlignment));
        __instance._addPadding = icon is not null;
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, IconAlignment? iconAlignment = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, TextStyle? textStyle = null, EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, BorderSide? side = null, OutlinedBorder? shape = null, MouseCursor? enabledMouseCursor = null, MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? backgroundBuilder = null, Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? foregroundBuilder = null)
    {
        WidgetStateProperty<Color?>? backgroundColorProp = (backgroundColor, disabledBackgroundColor) switch { (_, null) => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color?>(backgroundColor)), (_, _) => defaultColor(backgroundColor, disabledBackgroundColor) };
        WidgetStateProperty<Color?>? overlayColorProp = (WidgetStateProperty<Color?>?)((foregroundColor, overlayColor) switch { (null, null) => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(null), (_, Color { a: 0.0 } __object7771) => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color?>(overlayColor)), (_, Color color) => WidgetStateProperty<Color?>.CreateFromMap(new DartMap<WidgetStatesConstraint, Color?> { [WidgetState.pressed.asConstraint()] = color.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = color.withOpacity(0.08), [WidgetState.focused.asConstraint()] = color.withOpacity(0.1) }.cast<WidgetStatesConstraint, Color?>()), (Color colorLocal, _) => WidgetStateProperty<Color?>.CreateFromMap(new DartMap<WidgetStatesConstraint, Color?> { [WidgetState.pressed.asConstraint()] = colorLocal.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = colorLocal.withOpacity(0.08), [WidgetState.focused.asConstraint()] = colorLocal.withOpacity(0.1) }.cast<WidgetStatesConstraint, Color?>()) });
        return new ButtonStyle(textStyle: allOrNull(textStyle), foregroundColor: defaultColor(foregroundColor, disabledForegroundColor), backgroundColor: backgroundColorProp, overlayColor: overlayColorProp, shadowColor: allOrNull(shadowColor), surfaceTintColor: allOrNull(surfaceTintColor), iconColor: defaultColor(iconColor, disabledIconColor), iconSize: allOrNull(iconSize), iconAlignment: iconAlignment, elevation: allOrNull(elevation), padding: allOrNull(padding), minimumSize: allOrNull(minimumSize), fixedSize: allOrNull(fixedSize), maximumSize: allOrNull(maximumSize), side: allOrNull(side), shape: allOrNull(shape), mouseCursor: WidgetStateProperty<MouseCursor?>.CreateFromMap(new DartMap<WidgetStatesConstraint, MouseCursor?> { [WidgetState.disabled.asConstraint()] = disabledMouseCursor, [WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory, backgroundBuilder: backgroundBuilder, foregroundBuilder: foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ButtonStyle defaultStyleOf(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        ColorScheme colorSchemeLocal = theme.colorScheme;
        ButtonStyle buttonStyle = new _OutlinedButtonDefaultsM3__outlined_button(context);
        if (_addPadding)
        {
            double defaultFontSize = buttonStyle.textStyle?.resolve(new HashSet<WidgetState>())?.fontSize ?? 14.0;
            double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
            EdgeInsetsGeometry scaledPaddingLocal = scaledPadding(new EdgeInsetsDirectional(16, 0, 24, 0), new EdgeInsetsDirectional(8, 0, 12, 0), new EdgeInsetsDirectional(4, 0, 6, 0), effectiveTextScale);
            return buttonStyle.copyWith(padding: new WidgetStatePropertyAll<EdgeInsetsGeometry>(scaledPaddingLocal));
        }
        return buttonStyle;
    }

    public override ButtonStyle? themeStyleOf(BuildContext context)
    {
        return OutlinedButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Outlined_buttonLibrary
{
    internal static EdgeInsetsGeometry _scaledPadding(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        var padding1x = 24.0;
        double defaultFontSize = theme.textTheme.labelLarge?.fontSize ?? 14.0;
        double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
        return ButtonStyleButton.scaledPadding(EdgeInsets.CreateSymmetric(horizontal: padding1x), EdgeInsets.CreateSymmetric(horizontal: padding1x / 2L), EdgeInsets.CreateSymmetric(horizontal: padding1x / 2L / 2L), effectiveTextScale);
    }
}

internal class _OutlinedButtonWithIconChild__outlined_button : StatelessWidget
{
    public virtual Widget label { get; private set; } = default!;
    public virtual Widget icon { get; private set; } = default!;
    public virtual ButtonStyle? buttonStyle { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }

    internal _OutlinedButtonWithIconChild__outlined_button(Widget label, Widget icon, ButtonStyle? buttonStyle, IconAlignment? iconAlignment)
    {
        this.label = label;
        this.icon = icon;
        this.buttonStyle = buttonStyle;
        this.iconAlignment = iconAlignment;
    }

    public override Widget build(BuildContext context)
    {
        double defaultFontSize = buttonStyle?.textStyle?.resolve(new HashSet<WidgetState>())?.fontSize ?? 14.0;
        double scaleLocal = Dart_uiLibrary.clampDouble(MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0, 1.0, 2.0) - 1.0;
        OutlinedButtonThemeData outlinedButtonTheme = OutlinedButtonTheme.of(context);
        IconAlignment effectiveIconAlignment = ((iconAlignment ?? outlinedButtonTheme.style?.iconAlignment) ?? buttonStyle?.iconAlignment) ?? IconAlignment.start;
        return new Row(mainAxisSize: MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scaleLocal)), children: Equals(effectiveIconAlignment, IconAlignment.start) ? new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(icon), DartRuntimePrimitives.ConvertValue<Widget>(new Flexible(child: label)) } : new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Flexible(child: label)), DartRuntimePrimitives.ConvertValue<Widget>(icon) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _OutlinedButtonDefaultsM3__outlined_button : ButtonStyle
{
    public virtual BuildContext context { get; private set; } = default!;
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

    internal _OutlinedButtonDefaultsM3__outlined_button(BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
    }

    public override WidgetStateProperty<TextStyle?>? textStyle => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<TextStyle?>>(new WidgetStatePropertyAll<TextStyle?>(Theme.of(context).textTheme.labelLarge));
    public override WidgetStateProperty<Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        return _colors.primary;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override WidgetStateProperty<Color?>? shadowColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color?>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(0.0));
    public override WidgetStateProperty<EdgeInsetsGeometry?>? padding => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<EdgeInsetsGeometry?>>(new WidgetStatePropertyAll<EdgeInsetsGeometry>(Outlined_buttonLibrary._scaledPadding(context)));
    public override WidgetStateProperty<Size?>? minimumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size?>>(new WidgetStatePropertyAll<Size>(new Size(64.0, 40.0)));
    public override WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(18.0));
    public override WidgetStateProperty<Color?>? iconColor
    {
        get
        {
            return (WidgetStateProperty<Color?>?)WidgetStateProperty.resolveWith((states) =>
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
    public override WidgetStateProperty<Size?>? maximumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size?>>(new WidgetStatePropertyAll<Size>(Size.infinite));
    public override WidgetStateProperty<BorderSide?>? side => WidgetStateProperty.resolveWith<BorderSide?>((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return new BorderSide(color: _colors.onSurface.withOpacity(0.12));
        }
        if (states.Contains(WidgetState.focused))
        {
            return new BorderSide(color: _colors.primary);
        }
        return new BorderSide(color: _colors.outline);
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    public override WidgetStateProperty<OutlinedBorder?>? shape => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<OutlinedBorder?>>(new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()));
    public override WidgetStateProperty<MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

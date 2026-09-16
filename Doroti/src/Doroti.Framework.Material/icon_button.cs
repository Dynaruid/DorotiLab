// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/icon_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Icon_buttonLibrary
{
    internal static double _kMinButtonSize = ConstantsLibrary.kMinInteractiveDimension;
}

public enum _IconButtonVariant__icon_button
{
    standard,
    filled,
    filledTonal,
    outlined
}

public class IconButton : StatelessWidget
{
    public virtual double? iconSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual AlignmentGeometry? alignment { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual Widget icon { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Action? onPressed { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool? isSelected { get; private set; }
    public virtual Widget? selectedIcon { get; private set; }
    public virtual WidgetStatesController? statesController { get; private set; }
    internal virtual _IconButtonVariant__icon_button _variant { get; private set; } = default!;

    public IconButton(Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, Action? onPressed = default!, Action<bool>? onHover = null, Action? onLongPress = null, MouseCursor? mouseCursor = null, FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, Widget? selectedIcon = null, WidgetStatesController? statesController = null, Widget icon = default!) : base(key: key)
    {
        this.iconSize = iconSize;
        this.visualDensity = visualDensity;
        this.padding = padding;
        this.alignment = alignment;
        this.splashRadius = splashRadius;
        this.color = color;
        this.focusColor = focusColor;
        this.hoverColor = hoverColor;
        this.highlightColor = highlightColor;
        this.splashColor = splashColor;
        this.disabledColor = disabledColor;
        this.onPressed = onPressed;
        this.onHover = onHover;
        this.onLongPress = onLongPress;
        this.mouseCursor = mouseCursor;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.tooltip = tooltip;
        this.enableFeedback = enableFeedback;
        this.constraints = constraints;
        this.style = style;
        this.isSelected = isSelected;
        this.selectedIcon = selectedIcon;
        this.statesController = statesController;
        this.icon = icon;
        _variant = _IconButtonVariant__icon_button.standard;
        System.Diagnostics.Debug.Assert((splashRadius is null) || (DartRuntimePrimitives.RequireValue(splashRadius) > 0L));
    }

    public static IconButton CreateFilled(Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, Action? onPressed = default!, Action<bool>? onHover = null, Action? onLongPress = null, MouseCursor? mouseCursor = null, FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, Widget? selectedIcon = null, WidgetStatesController? statesController = null, Widget icon = default!)
    {
        var __instance = new IconButton(key: key, iconSize: iconSize, visualDensity: visualDensity, padding: padding, alignment: alignment, splashRadius: splashRadius, color: color, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, disabledColor: disabledColor, onPressed: onPressed, onHover: onHover, onLongPress: onLongPress, mouseCursor: mouseCursor, focusNode: focusNode, autofocus: autofocus, tooltip: tooltip, enableFeedback: enableFeedback, constraints: constraints, style: style, isSelected: isSelected, selectedIcon: selectedIcon, statesController: statesController, icon: icon);
        __instance.iconSize = iconSize;
        __instance.visualDensity = visualDensity;
        __instance.padding = padding;
        __instance.alignment = alignment;
        __instance.splashRadius = splashRadius;
        __instance.color = color;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.highlightColor = highlightColor;
        __instance.splashColor = splashColor;
        __instance.disabledColor = disabledColor;
        __instance.onPressed = onPressed;
        __instance.onHover = onHover;
        __instance.onLongPress = onLongPress;
        __instance.mouseCursor = mouseCursor;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.tooltip = tooltip;
        __instance.enableFeedback = enableFeedback;
        __instance.constraints = constraints;
        __instance.style = style;
        __instance.isSelected = isSelected;
        __instance.selectedIcon = selectedIcon;
        __instance.statesController = statesController;
        __instance.icon = icon;
        __instance._variant = _IconButtonVariant__icon_button.filled;
        return __instance;
    }

    public static IconButton CreateFilledTonal(Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, Action? onPressed = default!, Action<bool>? onHover = null, Action? onLongPress = null, MouseCursor? mouseCursor = null, FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, Widget? selectedIcon = null, WidgetStatesController? statesController = null, Widget icon = default!)
    {
        var __instance = new IconButton(key: key, iconSize: iconSize, visualDensity: visualDensity, padding: padding, alignment: alignment, splashRadius: splashRadius, color: color, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, disabledColor: disabledColor, onPressed: onPressed, onHover: onHover, onLongPress: onLongPress, mouseCursor: mouseCursor, focusNode: focusNode, autofocus: autofocus, tooltip: tooltip, enableFeedback: enableFeedback, constraints: constraints, style: style, isSelected: isSelected, selectedIcon: selectedIcon, statesController: statesController, icon: icon);
        __instance.iconSize = iconSize;
        __instance.visualDensity = visualDensity;
        __instance.padding = padding;
        __instance.alignment = alignment;
        __instance.splashRadius = splashRadius;
        __instance.color = color;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.highlightColor = highlightColor;
        __instance.splashColor = splashColor;
        __instance.disabledColor = disabledColor;
        __instance.onPressed = onPressed;
        __instance.onHover = onHover;
        __instance.onLongPress = onLongPress;
        __instance.mouseCursor = mouseCursor;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.tooltip = tooltip;
        __instance.enableFeedback = enableFeedback;
        __instance.constraints = constraints;
        __instance.style = style;
        __instance.isSelected = isSelected;
        __instance.selectedIcon = selectedIcon;
        __instance.statesController = statesController;
        __instance.icon = icon;
        __instance._variant = _IconButtonVariant__icon_button.filledTonal;
        return __instance;
    }

    public static IconButton CreateOutlined(Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, EdgeInsetsGeometry? padding = null, AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, Action? onPressed = default!, Action<bool>? onHover = null, Action? onLongPress = null, MouseCursor? mouseCursor = null, FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, Widget? selectedIcon = null, WidgetStatesController? statesController = null, Widget icon = default!)
    {
        var __instance = new IconButton(key: key, iconSize: iconSize, visualDensity: visualDensity, padding: padding, alignment: alignment, splashRadius: splashRadius, color: color, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, splashColor: splashColor, disabledColor: disabledColor, onPressed: onPressed, onHover: onHover, onLongPress: onLongPress, mouseCursor: mouseCursor, focusNode: focusNode, autofocus: autofocus, tooltip: tooltip, enableFeedback: enableFeedback, constraints: constraints, style: style, isSelected: isSelected, selectedIcon: selectedIcon, statesController: statesController, icon: icon);
        __instance.iconSize = iconSize;
        __instance.visualDensity = visualDensity;
        __instance.padding = padding;
        __instance.alignment = alignment;
        __instance.splashRadius = splashRadius;
        __instance.color = color;
        __instance.focusColor = focusColor;
        __instance.hoverColor = hoverColor;
        __instance.highlightColor = highlightColor;
        __instance.splashColor = splashColor;
        __instance.disabledColor = disabledColor;
        __instance.onPressed = onPressed;
        __instance.onHover = onHover;
        __instance.onLongPress = onLongPress;
        __instance.mouseCursor = mouseCursor;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.tooltip = tooltip;
        __instance.enableFeedback = enableFeedback;
        __instance.constraints = constraints;
        __instance.style = style;
        __instance.isSelected = isSelected;
        __instance.selectedIcon = selectedIcon;
        __instance.statesController = statesController;
        __instance.icon = icon;
        __instance._variant = _IconButtonVariant__icon_button.outlined;
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? overlayColor = null, double? elevation = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, double? iconSize = null, BorderSide? side = null, OutlinedBorder? shape = null, EdgeInsetsGeometry? padding = null, MouseCursor? enabledMouseCursor = null, MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        Color? overlayFallback = overlayColor ?? foregroundColor;
        WidgetStateProperty<Color?>? overlayColorProp = default!;
        if ((((hoverColor ?? focusColor) ?? highlightColor) ?? overlayFallback) is not null)
        {
            overlayColorProp = overlayColor switch { Color { a: 0.0 } __object25318 => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color>(__object25318)), _ => WidgetStateProperty<Color?>.CreateFromMap(new DartMap<WidgetStatesConstraint, Color?> { [WidgetState.pressed.asConstraint()] = highlightColor ?? overlayFallback?.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = hoverColor ?? overlayFallback?.withOpacity(0.08), [WidgetState.focused.asConstraint()] = focusColor ?? overlayFallback?.withOpacity(0.1) }.cast<WidgetStatesConstraint, Color?>()) };
        }
        return new ButtonStyle(backgroundColor: ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), overlayColor: overlayColorProp, shadowColor: ButtonStyleButton.allOrNull(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull(surfaceTintColor), elevation: ButtonStyleButton.allOrNull(elevation), padding: ButtonStyleButton.allOrNull(padding), minimumSize: ButtonStyleButton.allOrNull(minimumSize), fixedSize: ButtonStyleButton.allOrNull(fixedSize), maximumSize: ButtonStyleButton.allOrNull(maximumSize), iconSize: ButtonStyleButton.allOrNull(iconSize), side: ButtonStyleButton.allOrNull(side), shape: ButtonStyleButton.allOrNull(shape), mouseCursor: ((disabledMouseCursor is null) && (enabledMouseCursor is null)) ? null : WidgetStateProperty<MouseCursor?>.CreateFromMap(new DartMap<WidgetStatesConstraint, MouseCursor?> { [WidgetState.disabled.asConstraint()] = disabledMouseCursor, [WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        {
            Size? minSize = (constraints is null) ? null : new Size(constraints!.minWidth, constraints!.minHeight);
            Size? maxSize = (constraints is null) ? null : new Size(constraints!.maxWidth, constraints!.maxHeight);
            ButtonStyle adjustedStyle = styleFrom(visualDensity: visualDensity, foregroundColor: color, disabledForegroundColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, padding: padding, minimumSize: minSize, maximumSize: maxSize, iconSize: iconSize, alignment: alignment, enabledMouseCursor: mouseCursor, disabledMouseCursor: mouseCursor, enableFeedback: enableFeedback);
            if (style is not null)
            {
                adjustedStyle = style!.merge(adjustedStyle);
            }
            if (adjustedStyle.iconColor is null)
            {
                adjustedStyle = adjustedStyle.copyWith(iconColor: adjustedStyle.foregroundColor);
            }
            Widget effectiveIcon = icon;
            if ((isSelected ?? false) && (selectedIcon is not null))
            {
                effectiveIcon = selectedIcon!;
            }
            return new _SelectableIconButton__icon_button(style: adjustedStyle, onPressed: onPressed, onHover: onHover, onLongPress: (onPressed is not null) ? onLongPress : null, autofocus: autofocus, focusNode: focusNode, isSelected: isSelected, variant: _variant, tooltip: tooltip, statesController: statesController, child: effectiveIcon);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("tooltip", tooltip, defaultValue: null, quoted: false));
        properties.add(new ObjectFlagProperty<Action>("onPressed", onPressed, ifNull: "disabled"));
        properties.add(new ObjectFlagProperty<Action<bool>>("onHover", onHover, ifNull: "disabled"));
        properties.add(new ObjectFlagProperty<Action>("onLongPress", onLongPress, ifNull: "disabled"));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new ColorProperty("disabledColor", disabledColor, defaultValue: null));
        properties.add(new ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new ColorProperty("highlightColor", highlightColor, defaultValue: null));
        properties.add(new ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new DiagnosticsProperty<FocusNode>("focusNode", focusNode, defaultValue: null));
    }

}

public class _SelectableIconButton__icon_button : StatefulWidget
{
    public virtual bool? isSelected { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual _IconButtonVariant__icon_button variant { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Action? onPressed { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual Action? onLongPress { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual WidgetStatesController? statesController { get; private set; }

    internal _SelectableIconButton__icon_button(bool? isSelected = null, ButtonStyle? style = null, FocusNode? focusNode = null, Action? onLongPress = null, Action<bool>? onHover = null, WidgetStatesController? statesController = null, _IconButtonVariant__icon_button variant = default!, bool autofocus = default!, Action? onPressed = default!, string? tooltip = null, Widget child = default!)
    {
        this.isSelected = isSelected;
        this.style = style;
        this.focusNode = focusNode;
        this.onLongPress = onLongPress;
        this.onHover = onHover;
        this.statesController = statesController;
        this.variant = variant;
        this.autofocus = autofocus;
        this.onPressed = onPressed;
        this.tooltip = tooltip;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SelectableIconButtonState__icon_button());
}

internal class _SelectableIconButtonState__icon_button : State<_SelectableIconButton__icon_button>
{
    internal virtual WidgetStatesController? _internalStatesController { get; set; } = default;

    public virtual WidgetStatesController statesController => DartRuntimePrimitives.ConvertValue<WidgetStatesController>(widget.statesController ?? _internalStatesController!);
    internal virtual bool _isSelected => DartRuntimePrimitives.ConvertValue<bool>(widget.isSelected ?? false);
    public override void initState()
    {
        base.initState();
        if (widget.statesController is null)
        {
            _internalStatesController = new WidgetStatesController();
        }
        statesController.update(WidgetState.selected, _isSelected);
    }

    public override void didUpdateWidget(_SelectableIconButton__icon_button oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.statesController, oldWidget.statesController))
        {
            if (widget.statesController is not null)
            {
                _internalStatesController?.dispose();
                _internalStatesController = null;
            }
            _initStatesController();
        }
        if (widget.isSelected != oldWidget.isSelected)
        {
            statesController.update(WidgetState.selected, _isSelected);
        }
    }

    internal virtual void _initStatesController()
    {
        if (widget.statesController is null)
        {
            _internalStatesController = new WidgetStatesController();
        }
        statesController.update(WidgetState.selected, _isSelected);
    }

    public override Widget build(BuildContext context)
    {
        var toggleableLocal = widget.isSelected is not null;
        return new _IconButtonM3__icon_button(statesController: statesController, style: widget.style, autofocus: widget.autofocus, focusNode: widget.focusNode, onPressed: widget.onPressed, onHover: widget.onHover, onLongPress: (widget.onPressed is not null) ? widget.onLongPress : null, variant: widget.variant, toggleable: toggleableLocal, tooltip: widget.tooltip, child: new Widgets.Semantics(selected: widget.isSelected, child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _internalStatesController?.dispose();
        base.dispose();
    }

}

internal class _IconButtonM3__icon_button : ButtonStyleButton
{
    public virtual _IconButtonVariant__icon_button variant { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;

    internal _IconButtonM3__icon_button(Action? onPressed, ButtonStyle? style = null, FocusNode? focusNode = null, Action<bool>? onHover = null, Action? onLongPress = null, bool autofocus = false, WidgetStatesController? statesController = null, _IconButtonVariant__icon_button variant = default!, bool toggleable = default!, string? tooltip = null, Widget child = default!) : base(onPressed: onPressed, style: style, focusNode: focusNode, onHover: onHover, onLongPress: onLongPress, autofocus: autofocus, statesController: statesController, tooltip: tooltip, child: child, onFocusChange: null, clipBehavior: Clip.none)
    {
        this.variant = variant;
        this.toggleable = toggleable;
    }

    public override ButtonStyle defaultStyleOf(BuildContext context)
    {
        return variant switch { _IconButtonVariant__icon_button.filled => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledIconButtonDefaultsM3__icon_button(context, toggleable)), _IconButtonVariant__icon_button.filledTonal => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledTonalIconButtonDefaultsM3__icon_button(context, toggleable)), _IconButtonVariant__icon_button.outlined => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _OutlinedIconButtonDefaultsM3__icon_button(context, toggleable)), _IconButtonVariant__icon_button.standard => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _IconButtonDefaultsM3__icon_button(context, toggleable)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ButtonStyle? themeStyleOf(BuildContext context)
    {
        IconThemeData iconTheme = IconTheme.of(context);
        var isDefaultSize = iconTheme.size == IconThemeData.CreateFallback().size;
        bool isDefaultColor = DartRuntimePrimitives.Identical(iconTheme.color, Theme.brightnessOf(context) switch { Brightness.light => ConstantsLibrary.kDefaultIconDarkColor, Brightness.dark => ConstantsLibrary.kDefaultIconLightColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        ButtonStyle iconThemeStyle = IconButton.styleFrom(foregroundColor: isDefaultColor ? null : iconTheme.color, iconSize: isDefaultSize ? null : iconTheme.size);
        return IconButtonTheme.of(context).style?.merge(iconThemeStyle) ?? iconThemeStyle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _IconButtonDefaultsM3__icon_button(BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override WidgetStateProperty<Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.primary;
        }
        return _colors.onSurfaceVariant;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected))
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
        }
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.onSurfaceVariant.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.onSurfaceVariant.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.onSurfaceVariant.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(0.0));
    public override WidgetStateProperty<Color>? shadowColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<EdgeInsetsGeometry>>(new WidgetStatePropertyAll<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override WidgetStateProperty<Size>? minimumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(new Size(40.0, 40.0)));
    public override WidgetStateProperty<Size>? maximumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(Size.infinite));
    public override WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(24.0));
    public override WidgetStateProperty<BorderSide?>? side => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<BorderSide?>>(null);
    public override WidgetStateProperty<OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<OutlinedBorder>>(new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()));
    public override WidgetStateProperty<MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

internal class _FilledIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _FilledIconButtonDefaultsM3__icon_button(BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override WidgetStateProperty<Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.12);
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.primary;
        }
        if (toggleable)
        {
            return _colors.surfaceContainerHighest;
        }
        return _colors.primary;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.onPrimary;
        }
        if (toggleable)
        {
            return _colors.primary;
        }
        return _colors.onPrimary;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected))
        {
            if (states.Contains(WidgetState.pressed))
            {
                return _colors.onPrimary.withOpacity(0.1);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return _colors.onPrimary.withOpacity(0.08);
            }
            if (states.Contains(WidgetState.focused))
            {
                return _colors.onPrimary.withOpacity(0.1);
            }
        }
        if (toggleable)
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
        }
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.onPrimary.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.onPrimary.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.onPrimary.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(0.0));
    public override WidgetStateProperty<Color>? shadowColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<EdgeInsetsGeometry>>(new WidgetStatePropertyAll<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override WidgetStateProperty<Size>? minimumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(new Size(40.0, 40.0)));
    public override WidgetStateProperty<Size>? maximumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(Size.infinite));
    public override WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(24.0));
    public override WidgetStateProperty<BorderSide?>? side => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<BorderSide?>>(null);
    public override WidgetStateProperty<OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<OutlinedBorder>>(new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()));
    public override WidgetStateProperty<MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

internal class _FilledTonalIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _FilledTonalIconButtonDefaultsM3__icon_button(BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override WidgetStateProperty<Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.12);
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.secondaryContainer;
        }
        if (toggleable)
        {
            return _colors.surfaceContainerHighest;
        }
        return _colors.secondaryContainer;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.onSecondaryContainer;
        }
        if (toggleable)
        {
            return _colors.onSurfaceVariant;
        }
        return _colors.onSecondaryContainer;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected))
        {
            if (states.Contains(WidgetState.pressed))
            {
                return _colors.onSecondaryContainer.withOpacity(0.1);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return _colors.onSecondaryContainer.withOpacity(0.08);
            }
            if (states.Contains(WidgetState.focused))
            {
                return _colors.onSecondaryContainer.withOpacity(0.1);
            }
        }
        if (toggleable)
        {
            if (states.Contains(WidgetState.pressed))
            {
                return _colors.onSurfaceVariant.withOpacity(0.1);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return _colors.onSurfaceVariant.withOpacity(0.08);
            }
            if (states.Contains(WidgetState.focused))
            {
                return _colors.onSurfaceVariant.withOpacity(0.1);
            }
        }
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.onSecondaryContainer.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.onSecondaryContainer.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.onSecondaryContainer.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(0.0));
    public override WidgetStateProperty<Color>? shadowColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<EdgeInsetsGeometry>>(new WidgetStatePropertyAll<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override WidgetStateProperty<Size>? minimumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(new Size(40.0, 40.0)));
    public override WidgetStateProperty<Size>? maximumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(Size.infinite));
    public override WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(24.0));
    public override WidgetStateProperty<BorderSide?>? side => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<BorderSide?>>(null);
    public override WidgetStateProperty<OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<OutlinedBorder>>(new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()));
    public override WidgetStateProperty<MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

internal class _OutlinedIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _OutlinedIconButtonDefaultsM3__icon_button(BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override WidgetStateProperty<Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            if (states.Contains(WidgetState.selected))
            {
                return _colors.onSurface.withOpacity(0.12);
            }
            return Colors.transparent;
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.inverseSurface;
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.38);
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.onInverseSurface;
        }
        return _colors.onSurfaceVariant;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected))
        {
            if (states.Contains(WidgetState.pressed))
            {
                return _colors.onInverseSurface.withOpacity(0.1);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return _colors.onInverseSurface.withOpacity(0.08);
            }
            if (states.Contains(WidgetState.focused))
            {
                return _colors.onInverseSurface.withOpacity(0.08);
            }
        }
        if (states.Contains(WidgetState.pressed))
        {
            return _colors.onSurface.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.onSurfaceVariant.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.onSurfaceVariant.withOpacity(0.08);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(0.0));
    public override WidgetStateProperty<Color>? shadowColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color>>(new WidgetStatePropertyAll<Color>(Colors.transparent));
    public override WidgetStateProperty<EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<EdgeInsetsGeometry>>(new WidgetStatePropertyAll<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override WidgetStateProperty<Size>? minimumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(new Size(40.0, 40.0)));
    public override WidgetStateProperty<Size>? maximumSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Size>>(new WidgetStatePropertyAll<Size>(Size.infinite));
    public override WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<double?>>(new WidgetStatePropertyAll<double?>(24.0));
    public override WidgetStateProperty<BorderSide?>? side => WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected))
        {
            return null;
        }
        else
        {
            if (states.Contains(WidgetState.disabled))
            {
                return new BorderSide(color: _colors.onSurface.withOpacity(0.12));
            }
            return new BorderSide(color: _colors.outline);
        }
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    public override WidgetStateProperty<OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<OutlinedBorder>>(new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()));
    public override WidgetStateProperty<MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

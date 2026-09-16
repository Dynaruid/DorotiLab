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

public class IconButton : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual double? iconSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool? isSelected { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? selectedIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    internal virtual _IconButtonVariant__icon_button _variant { get; private set; } = default!;

    public IconButton(global::Doroti.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget icon = default!) : base(key: key)
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

    public static IconButton CreateFilled(global::Doroti.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget icon = default!)
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

    public static IconButton CreateFilledTonal(global::Doroti.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget icon = default!)
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

    public static IconButton CreateOutlined(global::Doroti.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Framework.Widgets.Widget icon = default!)
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

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? overlayColor = null, double? elevation = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, double? iconSize = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        global::Doroti.Ui.Color? overlayFallback = overlayColor ?? foregroundColor;
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp = default!;
        if ((((hoverColor ?? focusColor) ?? highlightColor) ?? overlayFallback) is not null)
        {
            overlayColorProp = overlayColor switch { global::Doroti.Ui.Color { a: 0.0 } __object25318 => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(__object25318)), _ => WidgetStateProperty<Color?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, Color?> { [WidgetState.pressed.asConstraint()] = highlightColor ?? overlayFallback?.withOpacity(0.1), [WidgetState.hovered.asConstraint()] = hoverColor ?? overlayFallback?.withOpacity(0.08), [WidgetState.focused.asConstraint()] = focusColor ?? overlayFallback?.withOpacity(0.1) }.cast<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()) };
        }
        return new ButtonStyle(backgroundColor: ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), overlayColor: overlayColorProp, shadowColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(surfaceTintColor), elevation: ButtonStyleButton.allOrNull<double?>(elevation), padding: ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(padding), minimumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(minimumSize), fixedSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(fixedSize), maximumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(maximumSize), iconSize: ButtonStyleButton.allOrNull<double?>(iconSize), side: ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.BorderSide>(side), shape: ButtonStyleButton.allOrNull<global::Doroti.Framework.Painting.OutlinedBorder>(shape), mouseCursor: ((disabledMouseCursor is null) && (enabledMouseCursor is null)) ? null : WidgetStateProperty<MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Framework.Services.MouseCursor?> { [WidgetState.disabled.asConstraint()] = disabledMouseCursor, [WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        {
            global::Doroti.Ui.Size? minSize = (constraints is null) ? null : new global::Doroti.Ui.Size(constraints!.minWidth, constraints!.minHeight);
            global::Doroti.Ui.Size? maxSize = (constraints is null) ? null : new global::Doroti.Ui.Size(constraints!.maxWidth, constraints!.maxHeight);
            ButtonStyle adjustedStyle = styleFrom(visualDensity: visualDensity, foregroundColor: color, disabledForegroundColor: disabledColor, focusColor: focusColor, hoverColor: hoverColor, highlightColor: highlightColor, padding: padding, minimumSize: minSize, maximumSize: maxSize, iconSize: iconSize, alignment: alignment, enabledMouseCursor: mouseCursor, disabledMouseCursor: mouseCursor, enableFeedback: enableFeedback);
            if (style is not null)
            {
                adjustedStyle = style!.merge(adjustedStyle);
            }
            if (adjustedStyle.iconColor is null)
            {
                adjustedStyle = adjustedStyle.copyWith(iconColor: adjustedStyle.foregroundColor);
            }
            global::Doroti.Framework.Widgets.Widget effectiveIcon = icon;
            if ((isSelected ?? false) && (selectedIcon is not null))
            {
                effectiveIcon = selectedIcon!;
            }
            return new _SelectableIconButton__icon_button(style: adjustedStyle, onPressed: onPressed, onHover: onHover, onLongPress: (onPressed is not null) ? onLongPress : null, autofocus: autofocus, focusNode: focusNode, isSelected: isSelected, variant: _variant, tooltip: tooltip, statesController: statesController, child: effectiveIcon);
        }
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("tooltip", tooltip, defaultValue: null, quoted: false));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action>("onPressed", onPressed, ifNull: "disabled"));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<bool>>("onHover", onHover, ifNull: "disabled"));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action>("onLongPress", onLongPress, ifNull: "disabled"));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("focusColor", focusColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hoverColor", hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("highlightColor", highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("splashColor", splashColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.FocusNode>("focusNode", focusNode, defaultValue: null));
    }

}

public class _SelectableIconButton__icon_button : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool? isSelected { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual _IconButtonVariant__icon_button variant { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }

    internal _SelectableIconButton__icon_button(bool? isSelected = null, ButtonStyle? style = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, _IconButtonVariant__icon_button variant = default!, bool autofocus = default!, global::System.Action? onPressed = default!, string? tooltip = null, global::Doroti.Framework.Widgets.Widget child = default!)
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

internal class _SelectableIconButtonState__icon_button : global::Doroti.Framework.Widgets.State<_SelectableIconButton__icon_button>
{
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController? _internalStatesController { get; set; } = default;

    public virtual global::Doroti.Framework.Widgets.WidgetStatesController statesController => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStatesController>(widget.statesController ?? _internalStatesController!);
    internal virtual bool _isSelected => DartRuntimePrimitives.ConvertValue<bool>(widget.isSelected ?? false);
    public override void initState()
    {
        base.initState();
        if (widget.statesController is null)
        {
            _internalStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController();
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
            _internalStatesController = new global::Doroti.Framework.Widgets.WidgetStatesController();
        }
        statesController.update(WidgetState.selected, _isSelected);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var toggleableLocal = widget.isSelected is not null;
        return new _IconButtonM3__icon_button(statesController: statesController, style: widget.style, autofocus: widget.autofocus, focusNode: widget.focusNode, onPressed: widget.onPressed, onHover: widget.onHover, onLongPress: (widget.onPressed is not null) ? widget.onLongPress : null, variant: widget.variant, toggleable: toggleableLocal, tooltip: widget.tooltip, child: new global::Doroti.Framework.Widgets.Semantics(selected: widget.isSelected, child: widget.child));
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

    internal _IconButtonM3__icon_button(global::System.Action? onPressed, ButtonStyle? style = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, _IconButtonVariant__icon_button variant = default!, bool toggleable = default!, string? tooltip = null, global::Doroti.Framework.Widgets.Widget child = default!) : base(onPressed: onPressed, style: style, focusNode: focusNode, onHover: onHover, onLongPress: onLongPress, autofocus: autofocus, statesController: statesController, tooltip: tooltip, child: child, onFocusChange: null, clipBehavior: Clip.none)
    {
        this.variant = variant;
        this.toggleable = toggleable;
    }

    public override ButtonStyle defaultStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return variant switch { _IconButtonVariant__icon_button.filled => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledIconButtonDefaultsM3__icon_button(context, toggleable)), _IconButtonVariant__icon_button.filledTonal => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledTonalIconButtonDefaultsM3__icon_button(context, toggleable)), _IconButtonVariant__icon_button.outlined => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _OutlinedIconButtonDefaultsM3__icon_button(context, toggleable)), _IconButtonVariant__icon_button.standard => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _IconButtonDefaultsM3__icon_button(context, toggleable)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ButtonStyle? themeStyleOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.IconThemeData iconTheme = IconTheme.of(context);
        var isDefaultSize = iconTheme.size == IconThemeData.CreateFallback().size;
        bool isDefaultColor = DartRuntimePrimitives.Identical(iconTheme.color, Theme.brightnessOf(context) switch { Brightness.light => ConstantsLibrary.kDefaultIconDarkColor, Brightness.dark => ConstantsLibrary.kDefaultIconLightColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        ButtonStyle iconThemeStyle = IconButton.styleFrom(foregroundColor: isDefaultColor ? null : iconTheme.color, iconSize: isDefaultSize ? null : iconTheme.size);
        return IconButtonTheme.of(context).style?.merge(iconThemeStyle) ?? iconThemeStyle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _IconButtonDefaultsM3__icon_button(global::Doroti.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>>(null);
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

internal class _FilledIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _FilledIconButtonDefaultsM3__icon_button(global::Doroti.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>>(null);
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

internal class _FilledTonalIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _FilledTonalIconButtonDefaultsM3__icon_button(global::Doroti.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>>(null);
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

internal class _OutlinedIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _OutlinedIconButtonDefaultsM3__icon_button(global::Doroti.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
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
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(0.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<double?>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<double?>(24.0));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.BorderSide?>? side => WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected))
        {
            return null;
        }
        else
        {
            if (states.Contains(WidgetState.disabled))
            {
                return new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface.withOpacity(0.12));
            }
            return new global::Doroti.Framework.Painting.BorderSide(color: _colors.outline);
        }
        throw new InvalidOperationException("Dart closure completed without a value.");
    });
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.OutlinedBorder>>(new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.OutlinedBorder>(new global::Doroti.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>(WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(context).splashFactory;
}

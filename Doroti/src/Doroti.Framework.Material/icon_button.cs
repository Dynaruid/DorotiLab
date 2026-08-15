// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/icon_button.dart
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

public class IconButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual double? iconSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual Color? focusColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? splashColor { get; private set; }
    public virtual Color? highlightColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual string? tooltip { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual bool? isSelected { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    internal virtual _IconButtonVariant__icon_button _variant { get; private set; } = default!;

    public IconButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget icon = default!) : base(key: key)
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
        this._variant = _IconButtonVariant__icon_button.standard;
        System.Diagnostics.Debug.Assert(((splashRadius is null) || (DartRuntimePrimitives.RequireValue(splashRadius) > 0L)));
    }

    public static IconButton CreateFilled(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget icon = default!)
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

    public static IconButton CreateFilledTonal(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget icon = default!)
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

    public static IconButton CreateOutlined(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? iconSize = null, VisualDensity? visualDensity = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, double? splashRadius = null, Color? color = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? splashColor = null, Color? disabledColor = null, global::System.Action? onPressed = default!, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, string? tooltip = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, ButtonStyle? style = null, bool? isSelected = null, global::Doroti.Generated.Framework.Widgets.Widget? selectedIcon = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget icon = default!)
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

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? focusColor = null, Color? hoverColor = null, Color? highlightColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? overlayColor = null, double? elevation = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, double? iconSize = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null)
    {
        global::Doroti.Ui.Color? overlayFallback__25076 = ((global::Doroti.Ui.Color?)(object?)(overlayColor ?? foregroundColor));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColorProp__25160 = default!;
        if ((((((hoverColor ?? focusColor) ?? highlightColor) ?? overlayFallback__25076)) is not null))
        {
            overlayColorProp__25160 = (overlayColor switch { global::Doroti.Ui.Color { a: 0.0 } __object25318 => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(__object25318)), _ => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = (highlightColor ?? overlayFallback__25076?.withOpacity(0.1)), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = (hoverColor ?? overlayFallback__25076?.withOpacity(0.08)), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = (focusColor ?? overlayFallback__25076?.withOpacity(0.1)) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Ui.Color?>()) });
        }
        return new ButtonStyle(backgroundColor: ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), overlayColor: overlayColorProp__25160, shadowColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull<global::Doroti.Ui.Color>(surfaceTintColor), elevation: ButtonStyleButton.allOrNull<double?>(elevation), padding: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(padding), minimumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(minimumSize), fixedSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(fixedSize), maximumSize: ButtonStyleButton.allOrNull<global::Doroti.Ui.Size>(maximumSize), iconSize: ButtonStyleButton.allOrNull<double?>(iconSize), side: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.BorderSide>(side), shape: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(shape), mouseCursor: (((disabledMouseCursor is null) && (enabledMouseCursor is null)) ? null : global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Generated.Framework.Services.MouseCursor?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabledMouseCursor, [global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any] = enabledMouseCursor })), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__27262 = Theme.of(context);
        if (theme__27262.useMaterial3)
        {
            global::Doroti.Ui.Size? minSize__27338 = ((global::Doroti.Ui.Size?)(object?)((this.constraints is null) ? null : new global::Doroti.Ui.Size(this.constraints!.minWidth, this.constraints!.minHeight)));
            global::Doroti.Ui.Size? maxSize__27468 = ((global::Doroti.Ui.Size?)(object?)((this.constraints is null) ? null : new global::Doroti.Ui.Size(this.constraints!.maxWidth, this.constraints!.maxHeight)));
            ButtonStyle adjustedStyle__27599 = ((ButtonStyle)(object?)IconButton.styleFrom(visualDensity: this.visualDensity, foregroundColor: this.color, disabledForegroundColor: this.disabledColor, focusColor: this.focusColor, hoverColor: this.hoverColor, highlightColor: this.highlightColor, padding: this.padding, minimumSize: minSize__27338, maximumSize: maxSize__27468, iconSize: this.iconSize, alignment: this.alignment, enabledMouseCursor: this.mouseCursor, disabledMouseCursor: this.mouseCursor, enableFeedback: this.enableFeedback));
            if ((this.style is not null))
            {
                adjustedStyle__27599 = this.style!.merge(adjustedStyle__27599);
            }
            if ((adjustedStyle__27599.iconColor is null))
            {
                adjustedStyle__27599 = adjustedStyle__27599.copyWith(iconColor: adjustedStyle__27599.foregroundColor);
            }
            global::Doroti.Generated.Framework.Widgets.Widget effectiveIcon__28368 = this.icon;
            if ((((this.isSelected ?? false)) && (this.selectedIcon is not null)))
            {
                effectiveIcon__28368 = this.selectedIcon!;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _SelectableIconButton__icon_button(style: adjustedStyle__27599, onPressed: () => this.onPressed(), onHover: (global::System.Action<bool>?)this.onHover, onLongPress: ((global::System.Action)((this.onPressed is not null) ? this.onLongPress : null)), autofocus: this.autofocus, focusNode: this.focusNode, isSelected: this.isSelected, variant: this._variant, tooltip: this.tooltip, statesController: this.statesController, child: effectiveIcon__28368));
        }
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        global::Doroti.Ui.Color? currentColor__28971 = default!;
        if ((this.onPressed is not null))
        {
            currentColor__28971 = this.color;
        }
        else
        {
            currentColor__28971 = (this.disabledColor ?? theme__27262.disabledColor);
        }
        VisualDensity effectiveVisualDensity__29145 = (this.visualDensity ?? theme__27262.visualDensity);
        global::Doroti.Generated.Framework.Rendering.BoxConstraints unadjustedConstraints__29234 = (this.constraints ?? new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: Icon_buttonLibrary._kMinButtonSize, minHeight: Icon_buttonLibrary._kMinButtonSize));
        global::Doroti.Generated.Framework.Rendering.BoxConstraints adjustedConstraints__29383 = effectiveVisualDensity__29145.effectiveConstraints(unadjustedConstraints__29234);
        double effectiveIconSize__29503 = ((this.iconSize ?? IconTheme.of(context).size) ?? 24.0);
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry effectivePadding__29600 = (this.padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0));
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry effectiveAlignment__29685 = (this.alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center);
        bool effectiveEnableFeedback__29752 = (this.enableFeedback ?? true);
        global::Doroti.Generated.Framework.Widgets.Widget result__29814 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: adjustedConstraints__29383, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: effectivePadding__29600, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateSquare(dimension: effectiveIconSize__29503, child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: effectiveAlignment__29685, child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: effectiveIconSize__29503, color: currentColor__28971), child: this.icon))))));
        result__29814 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkResponse(focusNode: this.focusNode, autofocus: this.autofocus, canRequestFocus: (this.onPressed is not null), onTap: this.onPressed, onHover: this.onHover, onLongPress: ((global::System.Action)((this.onPressed is not null) ? this.onLongPress : null)), mouseCursor: (this.mouseCursor ?? (((this.onPressed is not null) ? global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable : global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic))), enableFeedback: effectiveEnableFeedback__29752, focusColor: (this.focusColor ?? theme__27262.focusColor), hoverColor: (this.hoverColor ?? theme__27262.hoverColor), highlightColor: (this.highlightColor ?? theme__27262.highlightColor), splashColor: (this.splashColor ?? theme__27262.splashColor), radius: (this.splashRadius ?? Math.Max(Material.defaultSplashRadius, (((effectiveIconSize__29503 + Math.Min(((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)effectivePadding__29600).horizontal, ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)effectivePadding__29600).vertical))) * 0.7))), child: result__29814));
        if ((this.tooltip is not null))
        {
            result__29814 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Tooltip(message: this.tooltip, child: result__29814));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(button: true, enabled: (this.onPressed is not null), child: result__29814));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("tooltip", this.tooltip, defaultValue: null, quoted: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::System.Action>("onPressed", () => this.onPressed(), ifNull: "disabled"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::System.Action<bool>>("onHover", (global::System.Action<bool>?)this.onHover, ifNull: "disabled"));
        properties.add(new global::Doroti.Generated.Framework.Foundation.ObjectFlagProperty<global::System.Action>("onLongPress", () => this.onLongPress(), ifNull: "disabled"));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("focusColor", this.focusColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("hoverColor", this.hoverColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("highlightColor", this.highlightColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("splashColor", this.splashColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.FocusNode>("focusNode", this.focusNode, defaultValue: null));
    }

}

public class _SelectableIconButton__icon_button : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual bool? isSelected { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual _IconButtonVariant__icon_button variant { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual global::System.Action? onLongPress { get; private set; }
    public virtual global::System.Action<bool>? onHover { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }

    internal _SelectableIconButton__icon_button(bool? isSelected = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, _IconButtonVariant__icon_button variant = default!, bool autofocus = default!, global::System.Action? onPressed = default!, string? tooltip = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
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

internal class _SelectableIconButtonState__icon_button : global::Doroti.Generated.Framework.Widgets.State<_SelectableIconButton__icon_button>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? _internalStatesController { get; set; } = default;

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController statesController => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStatesController>((((_SelectableIconButton__icon_button)this.widget).statesController ?? this._internalStatesController!));
    internal virtual bool _isSelected => DartRuntimePrimitives.ConvertValue<bool>((((_SelectableIconButton__icon_button)this.widget).isSelected ?? false));
    public override void initState()
    {
        base.initState();
        if ((((_SelectableIconButton__icon_button)this.widget).statesController is null))
        {
            _internalStatesController = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();
        }
        this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.selected, this._isSelected);
    }

    public override void didUpdateWidget(_SelectableIconButton__icon_button oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_SelectableIconButton__icon_button)this.widget).statesController, ((_SelectableIconButton__icon_button)oldWidget).statesController)))
        {
            if ((((_SelectableIconButton__icon_button)this.widget).statesController is not null))
            {
                this._internalStatesController?.dispose();
                _internalStatesController = null;
            }
            _initStatesController();
        }
        if ((((_SelectableIconButton__icon_button)this.widget).isSelected != ((_SelectableIconButton__icon_button)oldWidget).isSelected))
        {
            this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.selected, this._isSelected);
        }
    }

    internal virtual void _initStatesController()
    {
        if ((((_SelectableIconButton__icon_button)this.widget).statesController is null))
        {
            _internalStatesController = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();
        }
        this.statesController.update(global::Doroti.Generated.Framework.Widgets.WidgetState.selected, this._isSelected);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var toggleable__34753 = (((_SelectableIconButton__icon_button)this.widget).isSelected is not null);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _IconButtonM3__icon_button(statesController: this.statesController, style: ((_SelectableIconButton__icon_button)this.widget).style, autofocus: ((_SelectableIconButton__icon_button)this.widget).autofocus, focusNode: ((_SelectableIconButton__icon_button)this.widget).focusNode, onPressed: () => ((_SelectableIconButton__icon_button)this.widget).onPressed(), onHover: (global::System.Action<bool>?)((_SelectableIconButton__icon_button)this.widget).onHover, onLongPress: ((global::System.Action)((((_SelectableIconButton__icon_button)this.widget).onPressed is not null) ? ((_SelectableIconButton__icon_button)this.widget).onLongPress : null)), variant: ((_SelectableIconButton__icon_button)this.widget).variant, toggleable: toggleable__34753, tooltip: ((_SelectableIconButton__icon_button)this.widget).tooltip, child: new global::Doroti.Generated.Framework.Widgets.Semantics(selected: ((_SelectableIconButton__icon_button)this.widget).isSelected, child: ((_SelectableIconButton__icon_button)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._internalStatesController?.dispose();
        base.dispose();
    }

}

internal class _IconButtonM3__icon_button : ButtonStyleButton
{
    public virtual _IconButtonVariant__icon_button variant { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;

    internal _IconButtonM3__icon_button(global::System.Action? onPressed, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::System.Action<bool>? onHover = null, global::System.Action? onLongPress = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, _IconButtonVariant__icon_button variant = default!, bool toggleable = default!, string? tooltip = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(onPressed: onPressed, style: style, focusNode: focusNode, onHover: onHover, onLongPress: onLongPress, autofocus: autofocus, statesController: statesController, tooltip: tooltip, child: child, onFocusChange: null, clipBehavior: Clip.none)
    {
        this.variant = variant;
        this.toggleable = toggleable;
    }

    public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return (this.variant switch { _IconButtonVariant__icon_button.filled => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledIconButtonDefaultsM3__icon_button(context, this.toggleable)), _IconButtonVariant__icon_button.filledTonal => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _FilledTonalIconButtonDefaultsM3__icon_button(context, this.toggleable)), _IconButtonVariant__icon_button.outlined => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _OutlinedIconButtonDefaultsM3__icon_button(context, this.toggleable)), _IconButtonVariant__icon_button.standard => DartRuntimePrimitives.ConvertValue<ButtonStyle>(new _IconButtonDefaultsM3__icon_button(context, this.toggleable)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.IconThemeData iconTheme__38073 = ((global::Doroti.Generated.Framework.Widgets.IconThemeData)(object?)IconTheme.of(context));
        var isDefaultSize__38118 = (((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__38073).size == global::Doroti.Generated.Framework.Widgets.IconThemeData.CreateFallback().size);
        bool isDefaultColor__38204 = DartRuntimePrimitives.Identical(((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__38073).color, (Theme.brightnessOf(context) switch { Brightness.light => ConstantsLibrary.kDefaultIconDarkColor, Brightness.dark => ConstantsLibrary.kDefaultIconLightColor, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        ButtonStyle iconThemeStyle__38416 = ((ButtonStyle)(object?)IconButton.styleFrom(foregroundColor: (isDefaultColor__38204 ? null : ((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__38073).color), iconSize: (isDefaultSize__38118 ? null : ((global::Doroti.Generated.Framework.Widgets.IconThemeData)iconTheme__38073).size)));
        return (IconButtonTheme.of(context).style?.merge(iconThemeStyle__38416) ?? iconThemeStyle__38416);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _IconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _IconButtonDefaultsM3__icon_button(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.primary);
}
return (this._colors.onSurfaceVariant);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
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
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurfaceVariant.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurfaceVariant.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurfaceVariant.withOpacity(0.1));
}
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(0.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(24.0));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>>(null);
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

internal class _FilledIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _FilledIconButtonDefaultsM3__icon_button(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.12));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.primary);
}
if (this.toggleable)
{
    return (this._colors.surfaceContainerHighest);
}
return (this._colors.primary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.onPrimary);
}
if (this.toggleable)
{
    return (this._colors.primary);
}
return (this._colors.onPrimary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
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
}
if (this.toggleable)
{
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
}
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
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(0.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(24.0));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>>(null);
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

internal class _FilledTonalIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _FilledTonalIconButtonDefaultsM3__icon_button(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.12));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.secondaryContainer);
}
if (this.toggleable)
{
    return (this._colors.surfaceContainerHighest);
}
return (this._colors.secondaryContainer);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.onSecondaryContainer);
}
if (this.toggleable)
{
    return (this._colors.onSurfaceVariant);
}
return (this._colors.onSecondaryContainer);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
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
}
if (this.toggleable)
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.onSurfaceVariant.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.onSurfaceVariant.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.onSurfaceVariant.withOpacity(0.1));
    }
}
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
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(0.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(24.0));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>>(null);
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

internal class _OutlinedIconButtonDefaultsM3__icon_button : ButtonStyle
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
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

    internal _OutlinedIconButtonDefaultsM3__icon_button(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool toggleable) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
        this.toggleable = toggleable;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
    {
        return (this._colors.onSurface.withOpacity(0.12));
    }
    return (Colors.transparent);
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.inverseSurface);
}
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.onInverseSurface);
}
return (this._colors.onSurfaceVariant);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (this._colors.onInverseSurface.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (this._colors.onInverseSurface.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (this._colors.onInverseSurface.withOpacity(0.08));
    }
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (this._colors.onSurface.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (this._colors.onSurfaceVariant.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (this._colors.onSurfaceVariant.withOpacity(0.08));
}
return (Colors.transparent);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(0.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(new global::Doroti.Ui.Size(40.0, 40.0)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? iconSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(24.0));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>? side => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return null;
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
    {
        return (new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12)));
    }
    return (new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.outline));
}
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => VisualDensity.standard;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

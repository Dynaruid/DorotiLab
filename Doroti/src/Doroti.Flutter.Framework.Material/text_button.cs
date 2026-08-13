// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/text_button.dart
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

public class TextButton : ButtonStyleButton
{
    internal virtual bool _addPadding { get; private set; } = default!;

    public TextButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, bool? isSemanticButton = true, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, isSemanticButton: isSemanticButton, child: child)
    {
        this._addPadding = false;
    }

    public static TextButton CreateIcon(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action? onPressed = default!, global::System.Action? onLongPress = null, global::System.Action<bool>? onHover = null, global::System.Action<bool>? onFocusChange = null, ButtonStyle? style = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, Clip? clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::Doroti.Generated.Framework.Widgets.Widget? icon = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, IconAlignment? iconAlignment = null)
    {
        var __instance = new TextButton(key: key, onPressed: onPressed, onLongPress: onLongPress, onHover: onHover, onFocusChange: onFocusChange, style: style, focusNode: focusNode, autofocus: autofocus, clipBehavior: clipBehavior, statesController: statesController, child: default!);
        __instance._addPadding = (icon is not null);
        return __instance;
    }

    public static ButtonStyle styleFrom(Color? foregroundColor = null, Color? backgroundColor = null, Color? disabledForegroundColor = null, Color? disabledBackgroundColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? iconColor = null, double? iconSize = null, IconAlignment? iconAlignment = null, Color? disabledIconColor = null, Color? overlayColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, Size? minimumSize = null, Size? fixedSize = null, Size? maximumSize = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Services.MouseCursor? enabledMouseCursor = null, global::Doroti.Generated.Framework.Services.MouseCursor? disabledMouseCursor = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? tapTargetSize = null, Duration? animationDuration = null, bool? enableFeedback = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, InteractiveInkFeatureFactory? splashFactory = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? backgroundBuilder = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? foregroundBuilder = null)
    {
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? backgroundColorProp__7671 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)((backgroundColor, disabledBackgroundColor) switch { (_, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(backgroundColor)), (_, _) => ButtonStyleButton.defaultColor(backgroundColor, disabledBackgroundColor) }));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? iconColorProp__7973 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)((iconColor, disabledIconColor) switch { (_, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(iconColor)), (_, _) => ButtonStyleButton.defaultColor(iconColor, disabledIconColor) }));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? overlayColorProp__8220 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>?)(object?)((foregroundColor, overlayColor) switch { (null, null) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(null), (_, global::Doroti.Flutter.Ui.Color { a: 0.0 } __object8320) => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color?>(overlayColor)), (_, global::Doroti.Flutter.Ui.Color color__8406) => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = color__8406.withOpacity(0.1), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = color__8406.withOpacity(0.08), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = color__8406.withOpacity(0.1) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Flutter.Ui.Color?>()), (global::Doroti.Flutter.Ui.Color color__8435, _) => global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, Color?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.pressed.asConstraint()] = color__8435.withOpacity(0.1), [global::Doroti.Generated.Framework.Widgets.WidgetState.hovered.asConstraint()] = color__8435.withOpacity(0.08), [global::Doroti.Generated.Framework.Widgets.WidgetState.focused.asConstraint()] = color__8435.withOpacity(0.1) }.cast<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Flutter.Ui.Color?>()) }));
        return new ButtonStyle(textStyle: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.TextStyle>(textStyle), foregroundColor: ButtonStyleButton.defaultColor(foregroundColor, disabledForegroundColor), backgroundColor: backgroundColorProp__7671, overlayColor: overlayColorProp__8220, shadowColor: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Color>(shadowColor), surfaceTintColor: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Color>(surfaceTintColor), iconColor: iconColorProp__7973, iconSize: ButtonStyleButton.allOrNull<double?>(iconSize), iconAlignment: iconAlignment, elevation: ButtonStyleButton.allOrNull<double?>(elevation), padding: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(padding), minimumSize: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Size>(minimumSize), fixedSize: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Size>(fixedSize), maximumSize: ButtonStyleButton.allOrNull<global::Doroti.Flutter.Ui.Size>(maximumSize), side: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.BorderSide>(side), shape: ButtonStyleButton.allOrNull<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(shape), mouseCursor: global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>.CreateFromMap(new DartMap<global::Doroti.Generated.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Generated.Framework.Services.MouseCursor?> { [global::Doroti.Generated.Framework.Widgets.WidgetState.disabled.asConstraint()] = disabledMouseCursor, [global::Doroti.Generated.Framework.Widgets.WidgetStateMembers.any] = enabledMouseCursor }), visualDensity: visualDensity, tapTargetSize: tapTargetSize, animationDuration: animationDuration, enableFeedback: enableFeedback, alignment: alignment, splashFactory: splashFactory, backgroundBuilder: backgroundBuilder, foregroundBuilder: foregroundBuilder);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle defaultStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__15202 = Theme.of(context);
        ColorScheme colorScheme__15251 = theme__15202.colorScheme;
        ButtonStyle buttonStyle__15306 = (theme__15202.useMaterial3 ? new _TextButtonDefaultsM3__text_button(context) : TextButton.styleFrom(foregroundColor: colorScheme__15251.primary, disabledForegroundColor: colorScheme__15251.onSurface.withOpacity(0.38), backgroundColor: Colors.transparent, disabledBackgroundColor: Colors.transparent, shadowColor: theme__15202.shadowColor, elevation: 0, textStyle: theme__15202.textTheme.labelLarge, padding: Text_buttonLibrary._scaledPadding(context), minimumSize: new global::Doroti.Flutter.Ui.Size(64, 36), maximumSize: Size.infinite, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(4))), enabledMouseCursor: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb ? global::Doroti.Generated.Framework.Services.SystemMouseCursors.click : global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic), disabledMouseCursor: global::Doroti.Generated.Framework.Services.SystemMouseCursors.basic, visualDensity: theme__15202.visualDensity, tapTargetSize: theme__15202.materialTapTargetSize, animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, splashFactory: InkRipple.splashFactory));
        if (this._addPadding)
        {
            double defaultFontSize__16536 = (buttonStyle__15306.textStyle?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
            double effectiveTextScale__16656 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__16536) / 14.0);
            global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry scaledPadding__16782 = ButtonStyleButton.scaledPadding((theme__15202.useMaterial3 ? new global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional(12, 8, 16, 8) : global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8)), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), effectiveTextScale__16656);
            return buttonStyle__15306.copyWith(padding: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(scaledPadding__16782));
        }
        return buttonStyle__15306;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ButtonStyle? themeStyleOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return TextButtonTheme.of(context).style;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Text_buttonLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry _scaledPadding(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__17547 = Theme.of(context);
        double defaultFontSize__17589 = (theme__17547.textTheme.labelLarge?.fontSize ?? 14.0);
        double effectiveTextScale__17668 = (MediaQuery.textScalerOf(context).scale(defaultFontSize__17589) / 14.0);
        return ButtonStyleButton.scaledPadding((theme__17547.useMaterial3 ? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 12, vertical: 8) : global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8)), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4), effectiveTextScale__17668);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _TextButtonWithIconChild__text_button : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget icon { get; private set; } = default!;
    public virtual ButtonStyle? buttonStyle { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }

    internal _TextButtonWithIconChild__text_button(global::Doroti.Generated.Framework.Widgets.Widget label, global::Doroti.Generated.Framework.Widgets.Widget icon, ButtonStyle? buttonStyle, IconAlignment? iconAlignment)
    {
        this.label = label;
        this.icon = icon;
        this.buttonStyle = buttonStyle;
        this.iconAlignment = iconAlignment;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        double defaultFontSize__18441 = (this.buttonStyle?.textStyle?.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())?.fontSize ?? 14.0);
        double scale__18558 = (Dart_uiLibrary.clampDouble((MediaQuery.textScalerOf(context).scale(defaultFontSize__18441) / 14.0), 1.0, 2.0) - 1.0);
        TextButtonThemeData textButtonTheme__18697 = TextButtonTheme.of(context);
        IconAlignment effectiveIconAlignment__18768 = (((this.iconAlignment ?? textButtonTheme__18697.style?.iconAlignment) ?? this.buttonStyle?.iconAlignment) ?? IconAlignment.start);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, spacing: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(8L, 4L, scale__18558)), children: ((object.Equals(effectiveIconAlignment__18768, IconAlignment.start)) ? new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.icon), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: this.label)) } : new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: this.label)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(this.icon) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TextButtonDefaultsM3__text_button : ButtonStyle
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

    internal _TextButtonDefaultsM3__text_button(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(animationDuration: ConstantsLibrary.kThemeChangeDuration, enableFeedback: true, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?> textStyle => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.TextStyle?>(Theme.of(this.context).textTheme.labelLarge));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? foregroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (this._colors.onSurface.withOpacity(0.38));
}
return (this._colors.primary);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
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
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Color>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Color>(Colors.transparent));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>? elevation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<double>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<double>(0.0));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(Text_buttonLibrary._scaledPadding(this.context)));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>? minimumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Size>(new global::Doroti.Flutter.Ui.Size(64.0, 40.0)));
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
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>? maximumSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Flutter.Ui.Size>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Flutter.Ui.Size>(Size.infinite));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>? shape => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>>(new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()));
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>? mouseCursor => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Services.MouseCursor?>>(global::Doroti.Generated.Framework.Widgets.WidgetStateMouseCursor.adaptiveClickable);
    public override VisualDensity? visualDensity => Theme.of(this.context).visualDensity;
    public override MaterialTapTargetSize? tapTargetSize => Theme.of(this.context).materialTapTargetSize;
    public override InteractiveInkFeatureFactory? splashFactory => Theme.of(this.context).splashFactory;
}

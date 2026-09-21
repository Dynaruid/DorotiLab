// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/action_chip.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _ChipVariant__action_chip
{
    flat,
    elevated,
}

public class ActionChip
    : StatelessWidget,
        ChipAttributes,
        TappableChipAttributes,
        DisabledChipAttributes
{
    public virtual Widget? avatar { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual Action? onPressed { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    internal virtual _ChipVariant__action_chip _chipVariant { get; private set; } = default!;

    public ActionChip(
        Key? key = null,
        Widget? avatar = null,
        Widget label = default!,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        Action? onPressed = null,
        double? pressElevation = null,
        string? tooltip = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        Clip clipBehavior = Clip.none,
        FocusNode? focusNode = null,
        bool autofocus = false,
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        Color? disabledColor = null,
        EdgeInsetsGeometry? padding = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        IconThemeData? iconTheme = null,
        BoxConstraints? avatarBoxConstraints = null,
        ChipAnimationStyle? chipAnimationStyle = null,
        MouseCursor? mouseCursor = null
    )
        : base(key: key)
    {
        this.avatar = avatar;
        this.label = label;
        this.labelStyle = labelStyle;
        this.labelPadding = labelPadding;
        this.onPressed = onPressed;
        this.pressElevation = pressElevation;
        this.tooltip = tooltip;
        this.side = side;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.disabledColor = disabledColor;
        this.padding = padding;
        this.visualDensity = visualDensity;
        this.materialTapTargetSize = materialTapTargetSize;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.iconTheme = iconTheme;
        this.avatarBoxConstraints = avatarBoxConstraints;
        this.chipAnimationStyle = chipAnimationStyle;
        this.mouseCursor = mouseCursor;
        _chipVariant = _ChipVariant__action_chip.flat;
        System.Diagnostics.Debug.Assert((pressElevation is null) || (pressElevation >= 0.0));
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public static ActionChip CreateElevated(
        Key? key = null,
        Widget? avatar = null,
        Widget label = default!,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        Action? onPressed = null,
        double? pressElevation = null,
        string? tooltip = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        Clip clipBehavior = Clip.none,
        FocusNode? focusNode = null,
        bool autofocus = false,
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        Color? disabledColor = null,
        EdgeInsetsGeometry? padding = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        IconThemeData? iconTheme = null,
        BoxConstraints? avatarBoxConstraints = null,
        ChipAnimationStyle? chipAnimationStyle = null,
        MouseCursor? mouseCursor = null
    )
    {
        var __instance = new ActionChip(
            key: key,
            avatar: avatar,
            label: label,
            labelStyle: labelStyle,
            labelPadding: labelPadding,
            onPressed: onPressed,
            pressElevation: pressElevation,
            tooltip: tooltip,
            side: side,
            shape: shape,
            clipBehavior: clipBehavior,
            focusNode: focusNode,
            autofocus: autofocus,
            color: color,
            backgroundColor: backgroundColor,
            disabledColor: disabledColor,
            padding: padding,
            visualDensity: visualDensity,
            materialTapTargetSize: materialTapTargetSize,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            iconTheme: iconTheme,
            avatarBoxConstraints: avatarBoxConstraints,
            chipAnimationStyle: chipAnimationStyle,
            mouseCursor: mouseCursor
        );
        __instance.avatar = avatar;
        __instance.label = label;
        __instance.labelStyle = labelStyle;
        __instance.labelPadding = labelPadding;
        __instance.onPressed = onPressed;
        __instance.pressElevation = pressElevation;
        __instance.tooltip = tooltip;
        __instance.side = side;
        __instance.shape = shape;
        __instance.clipBehavior = clipBehavior;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.color = color;
        __instance.backgroundColor = backgroundColor;
        __instance.disabledColor = disabledColor;
        __instance.padding = padding;
        __instance.visualDensity = visualDensity;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.elevation = elevation;
        __instance.shadowColor = shadowColor;
        __instance.surfaceTintColor = surfaceTintColor;
        __instance.iconTheme = iconTheme;
        __instance.avatarBoxConstraints = avatarBoxConstraints;
        __instance.chipAnimationStyle = chipAnimationStyle;
        __instance.mouseCursor = mouseCursor;
        __instance._chipVariant = _ChipVariant__action_chip.elevated;
        return __instance;
    }

    public virtual bool isEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(onPressed is not null);

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ChipThemeData? defaults = (ChipThemeData?)
            new _ActionChipDefaultsM3__action_chip(context, isEnabled, _chipVariant);
        return new RawChip(
            defaultProperties: defaults,
            avatar: avatar,
            label: label,
            onPressed: onPressed,
            pressElevation: pressElevation,
            tooltip: tooltip,
            labelStyle: labelStyle,
            color: color,
            backgroundColor: backgroundColor,
            side: side,
            shape: shape,
            clipBehavior: clipBehavior,
            focusNode: focusNode,
            autofocus: autofocus,
            disabledColor: disabledColor,
            padding: padding,
            visualDensity: visualDensity,
            isEnabled: isEnabled,
            labelPadding: labelPadding,
            materialTapTargetSize: materialTapTargetSize,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            iconTheme: iconTheme,
            avatarBoxConstraints: avatarBoxConstraints,
            chipAnimationStyle: chipAnimationStyle,
            mouseCursor: mouseCursor
        );
    }
}

internal class _ActionChipDefaultsM3__action_chip : ChipThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    internal virtual _ChipVariant__action_chip _chipVariant { get; private set; } = default!;
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
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _ActionChipDefaultsM3__action_chip(
        BuildContext context,
        bool isEnabled,
        _ChipVariant__action_chip _chipVariant
    )
        : base(
            shape: new RoundedRectangleBorder(
                borderRadius: BorderRadius.CreateAll(Radius.circular(8.0))
            ),
            showCheckmark: true
        )
    {
        this.context = context;
        this.isEnabled = isEnabled;
        this._chipVariant = _chipVariant;
    }

    public override double? elevation =>
        Equals(_chipVariant, _ChipVariant__action_chip.flat) ? 0.0 : (isEnabled ? 1.0 : 0.0);
    public override double? pressElevation => 1.0;
    public override TextStyle? labelStyle =>
        _textTheme.labelLarge?.copyWith(color: isEnabled ? _colors.onSurface : _colors.onSurface);
    public override WidgetStateProperty<Color?>? color =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(
            WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.disabled))
                    {
                        return Equals(_chipVariant, _ChipVariant__action_chip.flat)
                            ? null
                            : _colors.onSurface.withOpacity(0.12);
                    }
                    return Equals(_chipVariant, _ChipVariant__action_chip.flat)
                        ? null
                        : _colors.surfaceContainerLow;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            Equals(_chipVariant, _ChipVariant__action_chip.flat)
                ? Colors.transparent
                : _colors.shadow
        );
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override BorderSide? side =>
        Equals(_chipVariant, _ChipVariant__action_chip.flat)
            ? (
                isEnabled
                    ? new BorderSide(color: _colors.outlineVariant)
                    : new BorderSide(color: _colors.onSurface.withOpacity(0.12))
            )
            : new BorderSide(color: Colors.transparent);
    public override IconThemeData? iconTheme =>
        new IconThemeData(color: isEnabled ? _colors.primary : _colors.onSurface, size: 18.0);
    public override EdgeInsetsGeometry? padding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0));
    public override EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSizeLocal = labelStyle?.fontSize ?? 14.0;
            double fontSizeRatio = MediaQuery.textScalerOf(context).scale(fontSizeLocal) / 14.0;
            return EdgeInsets.lerp(
                EdgeInsets.CreateSymmetric(horizontal: 8.0),
                EdgeInsets.CreateSymmetric(horizontal: 4.0),
                DorotiUiLibrary.clampDouble(fontSizeRatio - 1.0, 0.0, 1.0)
            )!;
        }
    }
}

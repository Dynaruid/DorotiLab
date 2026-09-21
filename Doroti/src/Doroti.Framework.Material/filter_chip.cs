// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/filter_chip.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _ChipVariant__filter_chip
{
    flat,
    elevated,
}

public class FilterChip
    : StatelessWidget,
        ChipAttributes,
        DeletableChipAttributes,
        SelectableChipAttributes,
        CheckmarkableChipAttributes,
        DisabledChipAttributes
{
    public virtual Widget? avatar { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual Action<bool>? onSelected { get; private set; }
    public virtual Widget? deleteIcon { get; private set; }
    public virtual Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? selectedShadowColor { get; private set; }
    public virtual bool? showCheckmark { get; private set; }
    public virtual Color? checkmarkColor { get; private set; }
    public virtual ShapeBorder avatarBorder { get; private set; } = default!;
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    internal virtual _ChipVariant__filter_chip _chipVariant { get; private set; } = default!;

    public FilterChip(
        Key? key = null,
        Widget? avatar = null,
        Widget label = default!,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        bool selected = false,
        Action<bool>? onSelected = default!,
        Widget? deleteIcon = null,
        Action? onDeleted = null,
        Color? deleteIconColor = null,
        string? deleteButtonTooltipMessage = null,
        double? pressElevation = null,
        Color? disabledColor = null,
        Color? selectedColor = null,
        string? tooltip = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        Clip clipBehavior = Clip.none,
        FocusNode? focusNode = null,
        bool autofocus = false,
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        EdgeInsetsGeometry? padding = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        IconThemeData? iconTheme = null,
        Color? selectedShadowColor = null,
        bool? showCheckmark = null,
        Color? checkmarkColor = null,
        ShapeBorder avatarBorder = default!,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null,
        ChipAnimationStyle? chipAnimationStyle = null,
        MouseCursor? mouseCursor = null
    )
        : base(key: key)
    {
        ShapeBorder __avatarBorder = avatarBorder ?? new CircleBorder();
        this.avatar = avatar;
        this.label = label;
        this.labelStyle = labelStyle;
        this.labelPadding = labelPadding;
        this.selected = selected;
        this.onSelected = onSelected;
        this.deleteIcon = deleteIcon;
        this.onDeleted = onDeleted;
        this.deleteIconColor = deleteIconColor;
        this.deleteButtonTooltipMessage = deleteButtonTooltipMessage;
        this.pressElevation = pressElevation;
        this.disabledColor = disabledColor;
        this.selectedColor = selectedColor;
        this.tooltip = tooltip;
        this.side = side;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.padding = padding;
        this.visualDensity = visualDensity;
        this.materialTapTargetSize = materialTapTargetSize;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.iconTheme = iconTheme;
        this.selectedShadowColor = selectedShadowColor;
        this.showCheckmark = showCheckmark;
        this.checkmarkColor = checkmarkColor;
        this.avatarBorder = __avatarBorder;
        this.avatarBoxConstraints = avatarBoxConstraints;
        this.deleteIconBoxConstraints = deleteIconBoxConstraints;
        this.chipAnimationStyle = chipAnimationStyle;
        this.mouseCursor = mouseCursor;
        _chipVariant = _ChipVariant__filter_chip.flat;
        System.Diagnostics.Debug.Assert((pressElevation is null) || (pressElevation >= 0.0));
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public static FilterChip CreateElevated(
        Key? key = null,
        Widget? avatar = null,
        Widget label = default!,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        bool selected = false,
        Action<bool>? onSelected = default!,
        Widget? deleteIcon = null,
        Action? onDeleted = null,
        Color? deleteIconColor = null,
        string? deleteButtonTooltipMessage = null,
        double? pressElevation = null,
        Color? disabledColor = null,
        Color? selectedColor = null,
        string? tooltip = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        Clip clipBehavior = Clip.none,
        FocusNode? focusNode = null,
        bool autofocus = false,
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        EdgeInsetsGeometry? padding = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        IconThemeData? iconTheme = null,
        Color? selectedShadowColor = null,
        bool? showCheckmark = null,
        Color? checkmarkColor = null,
        ShapeBorder avatarBorder = default!,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null,
        ChipAnimationStyle? chipAnimationStyle = null,
        MouseCursor? mouseCursor = null
    )
    {
        var __instance = new FilterChip(
            key: key,
            avatar: avatar,
            label: label,
            labelStyle: labelStyle,
            labelPadding: labelPadding,
            selected: selected,
            onSelected: onSelected,
            deleteIcon: deleteIcon,
            onDeleted: onDeleted,
            deleteIconColor: deleteIconColor,
            deleteButtonTooltipMessage: deleteButtonTooltipMessage,
            pressElevation: pressElevation,
            disabledColor: disabledColor,
            selectedColor: selectedColor,
            tooltip: tooltip,
            side: side,
            shape: shape,
            clipBehavior: clipBehavior,
            focusNode: focusNode,
            autofocus: autofocus,
            color: color,
            backgroundColor: backgroundColor,
            padding: padding,
            visualDensity: visualDensity,
            materialTapTargetSize: materialTapTargetSize,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            iconTheme: iconTheme,
            selectedShadowColor: selectedShadowColor,
            showCheckmark: showCheckmark,
            checkmarkColor: checkmarkColor,
            avatarBorder: avatarBorder,
            avatarBoxConstraints: avatarBoxConstraints,
            deleteIconBoxConstraints: deleteIconBoxConstraints,
            chipAnimationStyle: chipAnimationStyle,
            mouseCursor: mouseCursor
        );
        ShapeBorder __avatarBorder = avatarBorder ?? new CircleBorder();
        __instance.avatar = avatar;
        __instance.label = label;
        __instance.labelStyle = labelStyle;
        __instance.labelPadding = labelPadding;
        __instance.selected = selected;
        __instance.onSelected = onSelected;
        __instance.deleteIcon = deleteIcon;
        __instance.onDeleted = onDeleted;
        __instance.deleteIconColor = deleteIconColor;
        __instance.deleteButtonTooltipMessage = deleteButtonTooltipMessage;
        __instance.pressElevation = pressElevation;
        __instance.disabledColor = disabledColor;
        __instance.selectedColor = selectedColor;
        __instance.tooltip = tooltip;
        __instance.side = side;
        __instance.shape = shape;
        __instance.clipBehavior = clipBehavior;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.color = color;
        __instance.backgroundColor = backgroundColor;
        __instance.padding = padding;
        __instance.visualDensity = visualDensity;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.elevation = elevation;
        __instance.shadowColor = shadowColor;
        __instance.surfaceTintColor = surfaceTintColor;
        __instance.iconTheme = iconTheme;
        __instance.selectedShadowColor = selectedShadowColor;
        __instance.showCheckmark = showCheckmark;
        __instance.checkmarkColor = checkmarkColor;
        __instance.avatarBorder = __avatarBorder;
        __instance.avatarBoxConstraints = avatarBoxConstraints;
        __instance.deleteIconBoxConstraints = deleteIconBoxConstraints;
        __instance.chipAnimationStyle = chipAnimationStyle;
        __instance.mouseCursor = mouseCursor;
        __instance._chipVariant = _ChipVariant__filter_chip.elevated;
        return __instance;
    }

    public virtual bool isEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(onSelected is not null);

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ChipThemeData? defaults = (ChipThemeData?)
            new _FilterChipDefaultsM3__filter_chip(context, isEnabled, selected, _chipVariant);
        Widget? resolvedDeleteIcon = deleteIcon ?? new Icon(Icons.clear, size: 18);
        return new RawChip(
            defaultProperties: defaults,
            avatar: avatar,
            label: label,
            labelStyle: labelStyle,
            labelPadding: labelPadding,
            onSelected: onSelected,
            deleteIcon: resolvedDeleteIcon,
            onDeleted: onDeleted,
            deleteIconColor: deleteIconColor,
            deleteButtonTooltipMessage: deleteButtonTooltipMessage,
            pressElevation: pressElevation,
            selected: selected,
            tooltip: tooltip,
            side: side,
            shape: shape,
            clipBehavior: clipBehavior,
            focusNode: focusNode,
            autofocus: autofocus,
            color: color,
            backgroundColor: backgroundColor,
            disabledColor: disabledColor,
            selectedColor: selectedColor,
            padding: padding,
            visualDensity: visualDensity,
            isEnabled: isEnabled,
            materialTapTargetSize: materialTapTargetSize,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            selectedShadowColor: selectedShadowColor,
            showCheckmark: showCheckmark,
            checkmarkColor: checkmarkColor,
            avatarBorder: avatarBorder,
            iconTheme: iconTheme,
            avatarBoxConstraints: avatarBoxConstraints,
            deleteIconBoxConstraints: deleteIconBoxConstraints,
            chipAnimationStyle: chipAnimationStyle,
            mouseCursor: mouseCursor
        );
    }
}

internal class _FilterChipDefaultsM3__filter_chip : ChipThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    public virtual bool isSelected { get; private set; } = default!;
    internal virtual _ChipVariant__filter_chip _chipVariant { get; private set; } = default!;
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

    internal _FilterChipDefaultsM3__filter_chip(
        BuildContext context,
        bool isEnabled,
        bool isSelected,
        _ChipVariant__filter_chip _chipVariant
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
        this.isSelected = isSelected;
        this._chipVariant = _chipVariant;
    }

    public override double? elevation =>
        Equals(_chipVariant, _ChipVariant__filter_chip.flat) ? 0.0 : (isEnabled ? 1.0 : 0.0);
    public override double? pressElevation => 1.0;
    public override TextStyle? labelStyle =>
        _textTheme.labelLarge?.copyWith(
            color: isEnabled
                ? (isSelected ? _colors.onSecondaryContainer : _colors.onSurfaceVariant)
                : _colors.onSurface
        );
    public override WidgetStateProperty<Color?>? color =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(
            WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (
                        states.Contains(WidgetState.selected)
                        && states.Contains(WidgetState.disabled)
                    )
                    {
                        return Equals(_chipVariant, _ChipVariant__filter_chip.flat)
                            ? _colors.onSurface.withOpacity(0.12)
                            : _colors.onSurface.withOpacity(0.12);
                    }
                    if (states.Contains(WidgetState.disabled))
                    {
                        return Equals(_chipVariant, _ChipVariant__filter_chip.flat)
                            ? null
                            : _colors.onSurface.withOpacity(0.12);
                    }
                    if (states.Contains(WidgetState.selected))
                    {
                        return Equals(_chipVariant, _ChipVariant__filter_chip.flat)
                            ? _colors.secondaryContainer
                            : _colors.secondaryContainer;
                    }
                    return Equals(_chipVariant, _ChipVariant__filter_chip.flat)
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
            Equals(_chipVariant, _ChipVariant__filter_chip.flat)
                ? Colors.transparent
                : _colors.shadow
        );
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? checkmarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            isEnabled
                ? (isSelected ? _colors.onSecondaryContainer : _colors.primary)
                : _colors.onSurface
        );
    public override Color? deleteIconColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            isEnabled
                ? (isSelected ? _colors.onSecondaryContainer : _colors.onSurfaceVariant)
                : _colors.onSurface
        );
    public override BorderSide? side =>
        (Equals(_chipVariant, _ChipVariant__filter_chip.flat) && !isSelected)
            ? (
                isEnabled
                    ? new BorderSide(color: _colors.outlineVariant)
                    : new BorderSide(color: _colors.onSurface.withOpacity(0.12))
            )
            : new BorderSide(color: Colors.transparent);
    public override IconThemeData? iconTheme =>
        new IconThemeData(
            color: isEnabled
                ? (isSelected ? _colors.onSecondaryContainer : _colors.primary)
                : _colors.onSurface,
            size: 18.0
        );
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

// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_chip.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class InputChip : StatelessWidget, ChipAttributes, DeletableChipAttributes, SelectableChipAttributes, CheckmarkableChipAttributes, DisabledChipAttributes, TappableChipAttributes
{
    public virtual Widget? avatar { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    public virtual System.Action<bool>? onSelected { get; private set; }
    public virtual Widget? deleteIcon { get; private set; }
    public virtual Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual Action? onPressed { get; private set; }
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

    public InputChip(Key? key = null, Widget? avatar = null, Widget label = default!, TextStyle? labelStyle = null, EdgeInsetsGeometry? labelPadding = null, bool selected = false, bool isEnabled = true, System.Action<bool>? onSelected = null, Widget? deleteIcon = null, Action? onDeleted = null, Color? deleteIconColor = null, string? deleteButtonTooltipMessage = null, Action? onPressed = null, double? pressElevation = null, Color? disabledColor = null, Color? selectedColor = null, string? tooltip = null, BorderSide? side = null, OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, FocusNode? focusNode = null, bool autofocus = false, WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, IconThemeData? iconTheme = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, ShapeBorder avatarBorder = default!, BoxConstraints? avatarBoxConstraints = null, BoxConstraints? deleteIconBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, MouseCursor? mouseCursor = null) : base(key: key)
    {
        ShapeBorder __avatarBorder = avatarBorder ?? new CircleBorder();
        this.avatar = avatar;
        this.label = label;
        this.labelStyle = labelStyle;
        this.labelPadding = labelPadding;
        this.selected = selected;
        this.isEnabled = isEnabled;
        this.onSelected = onSelected;
        this.deleteIcon = deleteIcon;
        this.onDeleted = onDeleted;
        this.deleteIconColor = deleteIconColor;
        this.deleteButtonTooltipMessage = deleteButtonTooltipMessage;
        this.onPressed = onPressed;
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
        System.Diagnostics.Debug.Assert((pressElevation is null) || (pressElevation >= 0.0));
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ChipThemeData? defaults = (ChipThemeData?)new _InputChipDefaultsM3__input_chip(context, isEnabled, selected);
        Widget? resolvedDeleteIcon = deleteIcon ?? new Icon(Icons.clear, size: 18);
        return new RawChip(defaultProperties: defaults, avatar: avatar, label: label, labelStyle: labelStyle, labelPadding: labelPadding, deleteIcon: resolvedDeleteIcon, onDeleted: onDeleted, deleteIconColor: deleteIconColor, deleteButtonTooltipMessage: deleteButtonTooltipMessage, onSelected: onSelected, onPressed: onPressed, pressElevation: pressElevation, selected: selected, disabledColor: disabledColor, selectedColor: selectedColor, tooltip: tooltip, side: side, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, color: color, backgroundColor: backgroundColor, padding: padding, visualDensity: visualDensity, materialTapTargetSize: materialTapTargetSize, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, selectedShadowColor: selectedShadowColor, showCheckmark: showCheckmark, checkmarkColor: checkmarkColor, isEnabled: isEnabled && ((onSelected is not null) || (onDeleted is not null) || (onPressed is not null)), avatarBorder: avatarBorder, iconTheme: iconTheme, avatarBoxConstraints: avatarBoxConstraints, deleteIconBoxConstraints: deleteIconBoxConstraints, chipAnimationStyle: chipAnimationStyle, mouseCursor: mouseCursor);
    }

}

internal class _InputChipDefaultsM3__input_chip : ChipThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    public virtual bool isSelected { get; private set; } = default!;
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

    internal _InputChipDefaultsM3__input_chip(BuildContext context, bool isEnabled, bool isSelected) : base(elevation: 0.0, shape: new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(8.0))), showCheckmark: true)
    {
        this.context = context;
        this.isEnabled = isEnabled;
        this.isSelected = isSelected;
    }

    public override TextStyle? labelStyle => _textTheme.labelLarge?.copyWith(color: isEnabled ? (isSelected ? _colors.onSecondaryContainer : _colors.onSurfaceVariant) : _colors.onSurface);
    public override WidgetStateProperty<Color?>? color => DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(WidgetState.selected) && states.Contains(WidgetState.disabled))
        {
            return _colors.onSurface.withOpacity(0.12);
        }
        if (states.Contains(WidgetState.disabled))
        {
            return null;
        }
        if (states.Contains(WidgetState.selected))
        {
            return _colors.secondaryContainer;
        }
        return null;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override Color? shadowColor => DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<Color>(isEnabled ? (isSelected ? _colors.primary : _colors.onSurfaceVariant) : _colors.onSurface);
    public override Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<Color>(isEnabled ? (isSelected ? _colors.onSecondaryContainer : _colors.onSurfaceVariant) : _colors.onSurface);
    public override BorderSide? side => !isSelected ? (isEnabled ? new BorderSide(color: _colors.outlineVariant) : new BorderSide(color: _colors.onSurface.withOpacity(0.12))) : new BorderSide(color: Colors.transparent);
    public override IconThemeData? iconTheme => new IconThemeData(color: isEnabled ? (isSelected ? _colors.primary : _colors.onSurfaceVariant) : _colors.onSurface, size: 18.0);
    public override EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0));
    public override EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSizeLocal = labelStyle?.fontSize ?? 14.0;
            double fontSizeRatio = MediaQuery.textScalerOf(context).scale(fontSizeLocal) / 14.0;
            return EdgeInsets.lerp(EdgeInsets.CreateSymmetric(horizontal: 8.0), EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble(fontSizeRatio - 1.0, 0.0, 1.0))!;
        }
    }
}

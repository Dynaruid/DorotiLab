// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/filter_chip.dart
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

internal enum _ChipVariant__filter_chip
{
    flat,
    elevated
}

public class FilterChip : global::Doroti.Generated.Framework.Widgets.StatelessWidget, ChipAttributes, DeletableChipAttributes, SelectableChipAttributes, CheckmarkableChipAttributes, DisabledChipAttributes
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? avatar { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual global::System.Action<bool>? onSelected { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? deleteIcon { get; private set; }
    public virtual global::System.Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? selectedShadowColor { get; private set; }
    public virtual bool? showCheckmark { get; private set; }
    public virtual Color? checkmarkColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder avatarBorder { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    internal virtual _ChipVariant__filter_chip _chipVariant { get; private set; } = default!;

    public FilterChip(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? avatar = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, bool selected = false, global::System.Action<bool>? onSelected = default!, global::Doroti.Generated.Framework.Widgets.Widget? deleteIcon = null, global::System.Action? onDeleted = null, Color? deleteIconColor = null, string? deleteButtonTooltipMessage = null, double? pressElevation = null, Color? disabledColor = null, Color? selectedColor = null, string? tooltip = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder avatarBorder = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.ShapeBorder __avatarBorder = avatarBorder ?? new global::Doroti.Generated.Framework.Painting.CircleBorder();
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
        this._chipVariant = _ChipVariant__filter_chip.flat;
        System.Diagnostics.Debug.Assert(((pressElevation is null) || (pressElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public static FilterChip CreateElevated(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? avatar = null, global::Doroti.Generated.Framework.Widgets.Widget label = default!, global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, bool selected = false, global::System.Action<bool>? onSelected = default!, global::Doroti.Generated.Framework.Widgets.Widget? deleteIcon = null, global::System.Action? onDeleted = null, Color? deleteIconColor = null, string? deleteButtonTooltipMessage = null, double? pressElevation = null, Color? disabledColor = null, Color? selectedColor = null, string? tooltip = null, global::Doroti.Generated.Framework.Painting.BorderSide? side = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder avatarBorder = default!, global::Doroti.Generated.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null)
    {
        var __instance = new FilterChip(key: key, avatar: avatar, label: label, labelStyle: labelStyle, labelPadding: labelPadding, selected: selected, onSelected: onSelected, deleteIcon: deleteIcon, onDeleted: onDeleted, deleteIconColor: deleteIconColor, deleteButtonTooltipMessage: deleteButtonTooltipMessage, pressElevation: pressElevation, disabledColor: disabledColor, selectedColor: selectedColor, tooltip: tooltip, side: side, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, color: color, backgroundColor: backgroundColor, padding: padding, visualDensity: visualDensity, materialTapTargetSize: materialTapTargetSize, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconTheme: iconTheme, selectedShadowColor: selectedShadowColor, showCheckmark: showCheckmark, checkmarkColor: checkmarkColor, avatarBorder: avatarBorder, avatarBoxConstraints: avatarBoxConstraints, deleteIconBoxConstraints: deleteIconBoxConstraints, chipAnimationStyle: chipAnimationStyle, mouseCursor: mouseCursor);
        global::Doroti.Generated.Framework.Painting.ShapeBorder __avatarBorder = avatarBorder ?? new global::Doroti.Generated.Framework.Painting.CircleBorder();
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

    public virtual bool isEnabled => DartRuntimePrimitives.ConvertValue<bool>((this.onSelected is not null));
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ChipThemeData? defaults__7322 = ((ChipThemeData?)(object?)(Theme.of(context).useMaterial3 ? new _FilterChipDefaultsM3__filter_chip(context, this.isEnabled, this.selected, this._chipVariant) : null));
        global::Doroti.Generated.Framework.Widgets.Widget? resolvedDeleteIcon__7474 = (this.deleteIcon ?? ((Theme.of(context).useMaterial3 ? new global::Doroti.Generated.Framework.Widgets.Icon(Icons.clear, size: 18) : null)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new RawChip(defaultProperties: defaults__7322, avatar: this.avatar, label: this.label, labelStyle: this.labelStyle, labelPadding: this.labelPadding, onSelected: (global::System.Action<bool>?)this.onSelected, deleteIcon: resolvedDeleteIcon__7474, onDeleted: () => this.onDeleted(), deleteIconColor: this.deleteIconColor, deleteButtonTooltipMessage: this.deleteButtonTooltipMessage, pressElevation: this.pressElevation, selected: this.selected, tooltip: this.tooltip, side: this.side, shape: this.shape, clipBehavior: this.clipBehavior, focusNode: this.focusNode, autofocus: this.autofocus, color: this.color, backgroundColor: this.backgroundColor, disabledColor: this.disabledColor, selectedColor: this.selectedColor, padding: this.padding, visualDensity: this.visualDensity, isEnabled: this.isEnabled, materialTapTargetSize: this.materialTapTargetSize, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, selectedShadowColor: this.selectedShadowColor, showCheckmark: this.showCheckmark, checkmarkColor: this.checkmarkColor, avatarBorder: this.avatarBorder, iconTheme: this.iconTheme, avatarBoxConstraints: this.avatarBoxConstraints, deleteIconBoxConstraints: this.deleteIconBoxConstraints, chipAnimationStyle: this.chipAnimationStyle, mouseCursor: this.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FilterChipDefaultsM3__filter_chip : ChipThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
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
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _FilterChipDefaultsM3__filter_chip(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isEnabled, bool isSelected, _ChipVariant__filter_chip _chipVariant) : base(shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))), showCheckmark: true)
    {
        this.context = context;
        this.isEnabled = isEnabled;
        this.isSelected = isSelected;
        this._chipVariant = _chipVariant;
    }

    public override double? elevation => ((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) ? 0.0 : (this.isEnabled ? 1.0 : 0.0));
    public override double? pressElevation => 1.0;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? labelStyle => this._textTheme.labelLarge?.copyWith(color: (this.isEnabled ? (this.isSelected ? this._colors.onSecondaryContainer : this._colors.onSurfaceVariant) : this._colors.onSurface));
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if ((states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected) && states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled)))
{
    return (((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) ? this._colors.onSurface.withOpacity(0.12) : this._colors.onSurface.withOpacity(0.12)));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
{
    return (((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) ? null : this._colors.onSurface.withOpacity(0.12)));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) ? this._colors.secondaryContainer : this._colors.secondaryContainer));
}
return (((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) ? null : this._colors.surfaceContainerLow));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) ? Colors.transparent : this._colors.shadow));
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this.isEnabled ? (this.isSelected ? this._colors.onSecondaryContainer : this._colors.primary) : this._colors.onSurface));
    public virtual global::Doroti.Ui.Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this.isEnabled ? (this.isSelected ? this._colors.onSecondaryContainer : this._colors.onSurfaceVariant) : this._colors.onSurface));
    public override global::Doroti.Generated.Framework.Painting.BorderSide? side => (((object.Equals(this._chipVariant, _ChipVariant__filter_chip.flat)) && !this.isSelected) ? (this.isEnabled ? new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.outlineVariant) : new global::Doroti.Generated.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12))) : new global::Doroti.Generated.Framework.Painting.BorderSide(color: Colors.transparent));
    public override global::Doroti.Generated.Framework.Widgets.IconThemeData? iconTheme => new global::Doroti.Generated.Framework.Widgets.IconThemeData(color: (this.isEnabled ? (this.isSelected ? this._colors.onSecondaryContainer : this._colors.primary) : this._colors.onSurface), size: 18.0);
    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(8.0));
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSize__12595 = (this.labelStyle?.fontSize ?? 14.0);
            double fontSizeRatio__12653 = (MediaQuery.textScalerOf(this.context).scale(fontSize__12595) / 14.0);
            return ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry?)(object?)EdgeInsets.lerp(global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble((fontSizeRatio__12653 - 1.0), 0.0, 1.0))!);
            return default!;
        }
    }
}

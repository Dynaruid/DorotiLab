// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_chip.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class InputChip : global::Doroti.Framework.Widgets.StatelessWidget, ChipAttributes, DeletableChipAttributes, SelectableChipAttributes, CheckmarkableChipAttributes, DisabledChipAttributes, TappableChipAttributes
{
    public virtual global::Doroti.Framework.Widgets.Widget? avatar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    public virtual global::System.Action<bool>? onSelected { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? deleteIcon { get; private set; }
    public virtual global::System.Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? selectedShadowColor { get; private set; }
    public virtual bool? showCheckmark { get; private set; }
    public virtual Color? checkmarkColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder avatarBorder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    public InputChip(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? avatar = null, global::Doroti.Framework.Widgets.Widget label = default!, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, bool selected = false, bool isEnabled = true, global::System.Action<bool>? onSelected = null, global::Doroti.Framework.Widgets.Widget? deleteIcon = null, global::System.Action? onDeleted = null, Color? deleteIconColor = null, string? deleteButtonTooltipMessage = null, global::System.Action? onPressed = null, double? pressElevation = null, Color? disabledColor = null, Color? selectedColor = null, string? tooltip = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, global::Doroti.Framework.Painting.ShapeBorder avatarBorder = default!, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.ShapeBorder __avatarBorder = avatarBorder ?? new global::Doroti.Framework.Painting.CircleBorder();
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
        System.Diagnostics.Debug.Assert(((pressElevation is null) || (pressElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ChipThemeData? defaults__6612 = ((ChipThemeData?)(object?)(Theme.of(context).useMaterial3 ? new _InputChipDefaultsM3__input_chip(context, this.isEnabled, this.selected) : null));
        global::Doroti.Framework.Widgets.Widget? resolvedDeleteIcon__6749 = (this.deleteIcon ?? ((Theme.of(context).useMaterial3 ? new global::Doroti.Framework.Widgets.Icon(Icons.clear, size: 18) : null)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new RawChip(defaultProperties: defaults__6612, avatar: this.avatar, label: this.label, labelStyle: this.labelStyle, labelPadding: this.labelPadding, deleteIcon: resolvedDeleteIcon__6749, onDeleted: () => this.onDeleted(), deleteIconColor: this.deleteIconColor, deleteButtonTooltipMessage: this.deleteButtonTooltipMessage, onSelected: (global::System.Action<bool>?)this.onSelected, onPressed: () => this.onPressed(), pressElevation: this.pressElevation, selected: this.selected, disabledColor: this.disabledColor, selectedColor: this.selectedColor, tooltip: this.tooltip, side: this.side, shape: this.shape, clipBehavior: this.clipBehavior, focusNode: this.focusNode, autofocus: this.autofocus, color: this.color, backgroundColor: this.backgroundColor, padding: this.padding, visualDensity: this.visualDensity, materialTapTargetSize: this.materialTapTargetSize, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, selectedShadowColor: this.selectedShadowColor, showCheckmark: this.showCheckmark, checkmarkColor: this.checkmarkColor, isEnabled: (this.isEnabled && ((((this.onSelected is not null) || (this.onDeleted is not null)) || (this.onPressed is not null)))), avatarBorder: this.avatarBorder, iconTheme: this.iconTheme, avatarBoxConstraints: this.avatarBoxConstraints, deleteIconBoxConstraints: this.deleteIconBoxConstraints, chipAnimationStyle: this.chipAnimationStyle, mouseCursor: this.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InputChipDefaultsM3__input_chip : ChipThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _InputChipDefaultsM3__input_chip(global::Doroti.Framework.Widgets.BuildContext context, bool isEnabled, bool isSelected) : base(elevation: 0.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))), showCheckmark: true)
    {
        this.context = context;
        this.isEnabled = isEnabled;
        this.isSelected = isSelected;
    }

    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => this._textTheme.labelLarge?.copyWith(color: (this.isEnabled ? (this.isSelected ? this._colors.onSecondaryContainer : this._colors.onSurfaceVariant) : this._colors.onSurface));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) => {
if ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) && states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled)))
{
    return (this._colors.onSurface.withOpacity(0.12));
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
{
    return null;
}
if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
{
    return (this._colors.secondaryContainer);
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this.isEnabled ? (this.isSelected ? this._colors.primary : this._colors.onSurfaceVariant) : this._colors.onSurface));
    public virtual global::Doroti.Ui.Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this.isEnabled ? (this.isSelected ? this._colors.onSecondaryContainer : this._colors.onSurfaceVariant) : this._colors.onSurface));
    public override global::Doroti.Framework.Painting.BorderSide? side => (!this.isSelected ? (this.isEnabled ? new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outlineVariant) : new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12))) : new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent));
    public override global::Doroti.Framework.Widgets.IconThemeData? iconTheme => new global::Doroti.Framework.Widgets.IconThemeData(color: (this.isEnabled ? (this.isSelected ? this._colors.primary : this._colors.onSurfaceVariant) : this._colors.onSurface), size: 18.0);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8.0));
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSize__11290 = (this.labelStyle?.fontSize ?? 14.0);
            double fontSizeRatio__11348 = (MediaQuery.textScalerOf(this.context).scale(fontSize__11290) / 14.0);
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry?)(object?)EdgeInsets.lerp(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble((fontSizeRatio__11348 - 1.0), 0.0, 1.0))!);
            return default!;
        }
    }
}

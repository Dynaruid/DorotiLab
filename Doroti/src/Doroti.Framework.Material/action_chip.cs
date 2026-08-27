// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/action_chip.dart
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

internal enum _ChipVariant__action_chip
{
    flat,
    elevated
}

public class ActionChip : global::Doroti.Framework.Widgets.StatelessWidget, ChipAttributes, TappableChipAttributes, DisabledChipAttributes
{
    public virtual global::Doroti.Framework.Widgets.Widget? avatar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    internal virtual _ChipVariant__action_chip _chipVariant { get; private set; } = default!;

    public ActionChip(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? avatar = null, global::Doroti.Framework.Widgets.Widget label = default!, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::System.Action? onPressed = null, double? pressElevation = null, string? tooltip = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, Color? disabledColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
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
        this._chipVariant = _ChipVariant__action_chip.flat;
        System.Diagnostics.Debug.Assert(((pressElevation is null) || (pressElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public static ActionChip CreateElevated(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? avatar = null, global::Doroti.Framework.Widgets.Widget label = default!, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::System.Action? onPressed = null, double? pressElevation = null, string? tooltip = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, Color? disabledColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null)
    {
        var __instance = new ActionChip(key: key, avatar: avatar, label: label, labelStyle: labelStyle, labelPadding: labelPadding, onPressed: onPressed, pressElevation: pressElevation, tooltip: tooltip, side: side, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, color: color, backgroundColor: backgroundColor, disabledColor: disabledColor, padding: padding, visualDensity: visualDensity, materialTapTargetSize: materialTapTargetSize, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconTheme: iconTheme, avatarBoxConstraints: avatarBoxConstraints, chipAnimationStyle: chipAnimationStyle, mouseCursor: mouseCursor);
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

    public virtual bool isEnabled => DartRuntimePrimitives.ConvertValue<bool>((this.onPressed is not null));
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        ChipThemeData? defaults = ((ChipThemeData?)(object?)(Theme.of(context).useMaterial3 ? new _ActionChipDefaultsM3__action_chip(context, this.isEnabled, this._chipVariant) : null));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new RawChip(defaultProperties: defaults, avatar: this.avatar, label: this.label, onPressed: () => this.onPressed(), pressElevation: this.pressElevation, tooltip: this.tooltip, labelStyle: this.labelStyle, color: this.color, backgroundColor: this.backgroundColor, side: this.side, shape: this.shape, clipBehavior: this.clipBehavior, focusNode: this.focusNode, autofocus: this.autofocus, disabledColor: this.disabledColor, padding: this.padding, visualDensity: this.visualDensity, isEnabled: this.isEnabled, labelPadding: this.labelPadding, materialTapTargetSize: this.materialTapTargetSize, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, iconTheme: this.iconTheme, avatarBoxConstraints: this.avatarBoxConstraints, chipAnimationStyle: this.chipAnimationStyle, mouseCursor: this.mouseCursor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActionChipDefaultsM3__action_chip : ChipThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _ActionChipDefaultsM3__action_chip(global::Doroti.Framework.Widgets.BuildContext context, bool isEnabled, _ChipVariant__action_chip _chipVariant) : base(shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))), showCheckmark: true)
    {
        this.context = context;
        this.isEnabled = isEnabled;
        this._chipVariant = _chipVariant;
    }

    public override double? elevation => ((object.Equals(this._chipVariant, _ChipVariant__action_chip.flat)) ? 0.0 : (this.isEnabled ? 1.0 : 0.0));
    public override double? pressElevation => 1.0;
    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => this._textTheme.labelLarge?.copyWith(color: (this.isEnabled ? this._colors.onSurface : this._colors.onSurface));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(WidgetStateProperty.resolveWith((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return (((object.Equals(this._chipVariant, _ChipVariant__action_chip.flat)) ? null : this._colors.onSurface.withOpacity(0.12)));
        }
        return (((object.Equals(this._chipVariant, _ChipVariant__action_chip.flat)) ? null : this._colors.surfaceContainerLow));
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((object.Equals(this._chipVariant, _ChipVariant__action_chip.flat)) ? Colors.transparent : this._colors.shadow));
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(null);
    public virtual global::Doroti.Ui.Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(null);
    public override global::Doroti.Framework.Painting.BorderSide? side => ((object.Equals(this._chipVariant, _ChipVariant__action_chip.flat)) ? (this.isEnabled ? new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outlineVariant) : new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12))) : new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent));
    public override global::Doroti.Framework.Widgets.IconThemeData? iconTheme => new global::Doroti.Framework.Widgets.IconThemeData(color: (this.isEnabled ? this._colors.primary : this._colors.onSurface), size: 18.0);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8.0));
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSizeLocal = (this.labelStyle?.fontSize ?? 14.0);
            double fontSizeRatio = (MediaQuery.textScalerOf(this.context).scale(fontSizeLocal) / 14.0);
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry?)(object?)EdgeInsets.lerp(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble((fontSizeRatio - 1.0), 0.0, 1.0))!);
            return default!;
        }
    }
}

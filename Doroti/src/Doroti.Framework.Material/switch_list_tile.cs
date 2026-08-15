// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/switch_list_tile.dart
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

internal enum _SwitchListTileType__switch_list_tile
{
    material,
    adaptive
}

public class SwitchListTile : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual bool value { get; private set; } = default!;
    public virtual global::System.Action<bool>? onChanged { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color? activeThumbColor { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveThumbColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual dynamic activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual dynamic inactiveThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>? thumbIcon { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Color? tileColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? secondary { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    internal virtual _SwitchListTileType__switch_list_tile _switchListTileType { get; private set; } = default!;
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual bool? applyCupertinoTheme { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;

    public SwitchListTile(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, dynamic activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, dynamic inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, Color? tileColor = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Generated.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Color? selectedTileColor = null, VisualDensity? visualDensity = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, Color? hoverColor = null, bool internalAddSemanticForOnTap = false) : base(key: key)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.activeColor = activeColor;
        this.activeThumbColor = activeThumbColor;
        this.activeTrackColor = activeTrackColor;
        this.inactiveThumbColor = inactiveThumbColor;
        this.inactiveTrackColor = inactiveTrackColor;
        this.activeThumbImage = activeThumbImage;
        this.onActiveThumbImageError = onActiveThumbImageError;
        this.inactiveThumbImage = inactiveThumbImage;
        this.onInactiveThumbImageError = onInactiveThumbImageError;
        this.thumbColor = thumbColor;
        this.trackColor = trackColor;
        this.trackOutlineColor = trackOutlineColor;
        this.thumbIcon = thumbIcon;
        this.materialTapTargetSize = materialTapTargetSize;
        this.dragStartBehavior = dragStartBehavior;
        this.mouseCursor = mouseCursor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.focusNode = focusNode;
        this.statesController = statesController;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.tileColor = tileColor;
        this.title = title;
        this.subtitle = subtitle;
        this.isThreeLine = isThreeLine;
        this.dense = dense;
        this.contentPadding = contentPadding;
        this.secondary = secondary;
        this.selected = selected;
        this.controlAffinity = controlAffinity;
        this.shape = shape;
        this.selectedTileColor = selectedTileColor;
        this.visualDensity = visualDensity;
        this.enableFeedback = enableFeedback;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.minTileHeight = minTileHeight;
        this.hoverColor = hoverColor;
        this.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        this._switchListTileType = _SwitchListTileType__switch_list_tile.material;
        this.applyCupertinoTheme = false;
        System.Diagnostics.Debug.Assert(((activeThumbImage is not null) || (onActiveThumbImageError is null)));
        System.Diagnostics.Debug.Assert(((inactiveThumbImage is not null) || (onInactiveThumbImageError is null)));
        System.Diagnostics.Debug.Assert(((isThreeLine != true) || (subtitle is not null)));
    }

    public static SwitchListTile CreateAdaptive(global::Doroti.Generated.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, dynamic activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, dynamic inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, global::Doroti.Generated.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Generated.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Generated.Framework.Widgets.WidgetStatesController? statesController = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, bool? applyCupertinoTheme = null, Color? tileColor = null, global::Doroti.Generated.Framework.Widgets.Widget? title = null, global::Doroti.Generated.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Generated.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Color? selectedTileColor = null, VisualDensity? visualDensity = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, Color? hoverColor = null, bool internalAddSemanticForOnTap = false)
    {
        var __instance = new SwitchListTile(key: key, value: value, onChanged: onChanged, activeColor: activeColor, activeThumbColor: activeThumbColor, activeTrackColor: activeTrackColor, inactiveThumbColor: inactiveThumbColor, inactiveTrackColor: inactiveTrackColor, activeThumbImage: activeThumbImage, onActiveThumbImageError: onActiveThumbImageError, inactiveThumbImage: inactiveThumbImage, onInactiveThumbImageError: onInactiveThumbImageError, thumbColor: thumbColor, trackColor: trackColor, trackOutlineColor: trackOutlineColor, thumbIcon: thumbIcon, materialTapTargetSize: materialTapTargetSize, dragStartBehavior: dragStartBehavior, mouseCursor: mouseCursor, overlayColor: overlayColor, splashRadius: splashRadius, focusNode: focusNode, statesController: statesController, onFocusChange: onFocusChange, autofocus: autofocus, tileColor: tileColor, title: title, subtitle: subtitle, isThreeLine: isThreeLine, dense: dense, contentPadding: contentPadding, secondary: secondary, selected: selected, controlAffinity: controlAffinity, shape: shape, selectedTileColor: selectedTileColor, visualDensity: visualDensity, enableFeedback: enableFeedback, horizontalTitleGap: horizontalTitleGap, minVerticalPadding: minVerticalPadding, minLeadingWidth: minLeadingWidth, minTileHeight: minTileHeight, hoverColor: hoverColor, internalAddSemanticForOnTap: internalAddSemanticForOnTap);
        __instance.value = value;
        __instance.onChanged = onChanged;
        __instance.activeColor = activeColor;
        __instance.activeThumbColor = activeThumbColor;
        __instance.activeTrackColor = activeTrackColor;
        __instance.inactiveThumbColor = inactiveThumbColor;
        __instance.inactiveTrackColor = inactiveTrackColor;
        __instance.activeThumbImage = activeThumbImage;
        __instance.onActiveThumbImageError = onActiveThumbImageError;
        __instance.inactiveThumbImage = inactiveThumbImage;
        __instance.onInactiveThumbImageError = onInactiveThumbImageError;
        __instance.thumbColor = thumbColor;
        __instance.trackColor = trackColor;
        __instance.trackOutlineColor = trackOutlineColor;
        __instance.thumbIcon = thumbIcon;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.mouseCursor = mouseCursor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.focusNode = focusNode;
        __instance.statesController = statesController;
        __instance.onFocusChange = onFocusChange;
        __instance.autofocus = autofocus;
        __instance.applyCupertinoTheme = applyCupertinoTheme;
        __instance.tileColor = tileColor;
        __instance.title = title;
        __instance.subtitle = subtitle;
        __instance.isThreeLine = isThreeLine;
        __instance.dense = dense;
        __instance.contentPadding = contentPadding;
        __instance.secondary = secondary;
        __instance.selected = selected;
        __instance.controlAffinity = controlAffinity;
        __instance.shape = shape;
        __instance.selectedTileColor = selectedTileColor;
        __instance.visualDensity = visualDensity;
        __instance.enableFeedback = enableFeedback;
        __instance.horizontalTitleGap = horizontalTitleGap;
        __instance.minVerticalPadding = minVerticalPadding;
        __instance.minLeadingWidth = minLeadingWidth;
        __instance.minTileHeight = minTileHeight;
        __instance.hoverColor = hoverColor;
        __instance.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        __instance._switchListTileType = _SwitchListTileType__switch_list_tile.adaptive;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget control__20854 = default!;
        switch (this._switchListTileType)
        {
            case _SwitchListTileType__switch_list_tile.adaptive:
                {
                    control__20854 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(child: Switch.CreateAdaptive(value: this.value, onChanged: (global::System.Action<bool>?)this.onChanged, activeColor: this.activeColor, activeThumbColor: this.activeThumbColor, activeThumbImage: this.activeThumbImage, inactiveThumbImage: this.inactiveThumbImage, materialTapTargetSize: (this.materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap), activeTrackColor: this.activeTrackColor, inactiveTrackColor: this.inactiveTrackColor, inactiveThumbColor: this.inactiveThumbColor, autofocus: this.autofocus, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, onActiveThumbImageError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this.onActiveThumbImageError, onInactiveThumbImageError: (global::System.Action<object, global::System.Diagnostics.StackTrace?>?)this.onInactiveThumbImageError, thumbColor: this.thumbColor, trackColor: this.trackColor, trackOutlineColor: this.trackOutlineColor, thumbIcon: this.thumbIcon, applyCupertinoTheme: this.applyCupertinoTheme, dragStartBehavior: this.dragStartBehavior, mouseCursor: this.mouseCursor, splashRadius: this.splashRadius, overlayColor: this.overlayColor)));
                    break;
                }
            case _SwitchListTileType__switch_list_tile.material:
                {
                    control__20854 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ExcludeFocus(child: new Switch(value: this.value, onChanged: this.onChanged, activeColor: this.activeColor, activeThumbColor: this.activeThumbColor, activeThumbImage: this.activeThumbImage, inactiveThumbImage: this.inactiveThumbImage, materialTapTargetSize: (this.materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap), activeTrackColor: this.activeTrackColor, inactiveTrackColor: this.inactiveTrackColor, inactiveThumbColor: this.inactiveThumbColor, autofocus: this.autofocus, onFocusChange: this.onFocusChange, onActiveThumbImageError: this.onActiveThumbImageError, onInactiveThumbImageError: this.onInactiveThumbImageError, thumbColor: this.thumbColor, trackColor: this.trackColor, trackOutlineColor: this.trackOutlineColor, thumbIcon: this.thumbIcon, dragStartBehavior: this.dragStartBehavior, mouseCursor: this.mouseCursor, splashRadius: this.splashRadius, overlayColor: this.overlayColor)));
                    break;
                }
        }
        ListTileThemeData listTileTheme__23281 = ListTileTheme.of(context);
        ListTileControlAffinity effectiveControlAffinity__23358 = ((this.controlAffinity ?? listTileTheme__23281.controlAffinity) ?? ListTileControlAffinity.platform);
        global::Doroti.Generated.Framework.Widgets.Widget? leading__23491 = default!;
        global::Doroti.Generated.Framework.Widgets.Widget? trailing__23500 = default!;
        DartRuntimePrimitives.Ignore((leading__23491, trailing__23500) = (effectiveControlAffinity__23358 switch { var __constant23578 when (object.Equals(__constant23578, ListTileControlAffinity.leading)) => (((global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?)>((control__20854, this.secondary)))), var __constant23641 when (object.Equals(__constant23641, ListTileControlAffinity.trailing)) => (((global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?)>((this.secondary, control__20854)))), var __constant23677 when (object.Equals(__constant23677, ListTileControlAffinity.platform)) => (((global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget?)>((this.secondary, control__20854)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        ThemeData theme__23763 = Theme.of(context);
        SwitchThemeData switchTheme__23816 = SwitchTheme.of(context);
        var states__23865 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection23874 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (this.selected) { __collection23874.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection23874; }))();
        global::Doroti.Ui.Color effectiveActiveColor__23941 = ((global::Doroti.Ui.Color)(object?)((((this.activeThumbColor ?? this.activeColor) ?? (Color)switchTheme__23816.thumbColor?.resolve(states__23865))) ?? theme__23763.colorScheme.secondary));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.MergeSemantics(child: new ListTile(selectedColor: effectiveActiveColor__23941, leading: leading__23491, title: this.title, subtitle: this.subtitle, trailing: trailing__23500, isThreeLine: this.isThreeLine, dense: this.dense, contentPadding: this.contentPadding, enabled: (this.onChanged is not null), onTap: ((global::System.Action)((this.onChanged is not null) ? (() => {
this.onChanged!(!this.value);
}) : null)), selected: this.selected, selectedTileColor: this.selectedTileColor, autofocus: this.autofocus, shape: this.shape, tileColor: this.tileColor, visualDensity: this.visualDensity, focusNode: this.focusNode, statesController: this.statesController, onFocusChange: this.onFocusChange, enableFeedback: this.enableFeedback, horizontalTitleGap: this.horizontalTitleGap, minVerticalPadding: this.minVerticalPadding, minLeadingWidth: this.minLeadingWidth, minTileHeight: this.minTileHeight, hoverColor: this.hoverColor, internalAddSemanticForOnTap: this.internalAddSemanticForOnTap)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/switch_list_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _SwitchListTileType__switch_list_tile
{
    material,
    adaptive
}

public class SwitchListTile : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool value { get; private set; } = default!;
    public virtual global::System.Action<bool>? onChanged { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color? activeThumbColor { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveThumbColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.IImageProvider? activeThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage { get; private set; } = default!;
    public virtual global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Color? tileColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? secondary { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    internal virtual _SwitchListTileType__switch_list_tile _switchListTileType { get; private set; } = default!;
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
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

    public SwitchListTile(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, global::Doroti.Framework.Painting.IImageProvider? activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, Color? tileColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Color? selectedTileColor = null, VisualDensity? visualDensity = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, Color? hoverColor = null, bool internalAddSemanticForOnTap = false) : base(key: key)
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
        _switchListTileType = _SwitchListTileType__switch_list_tile.material;
        applyCupertinoTheme = false;
        System.Diagnostics.Debug.Assert((activeThumbImage is not null) || (onActiveThumbImageError is null));
        System.Diagnostics.Debug.Assert((inactiveThumbImage is not null) || (onInactiveThumbImageError is null));
        System.Diagnostics.Debug.Assert((isThreeLine != true) || (subtitle is not null));
    }

    public static SwitchListTile CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, bool value = default!, global::System.Action<bool>? onChanged = default!, Color? activeColor = null, Color? activeThumbColor = null, Color? activeTrackColor = null, Color? inactiveThumbColor = null, Color? inactiveTrackColor = null, global::Doroti.Framework.Painting.IImageProvider? activeThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onActiveThumbImageError = null, global::Doroti.Framework.Painting.IImageProvider? inactiveThumbImage = null, global::System.Action<object, global::System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? trackOutlineColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.Icon?>? thumbIcon = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::System.Action<bool>? onFocusChange = null, bool autofocus = false, bool? applyCupertinoTheme = null, Color? tileColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Color? selectedTileColor = null, VisualDensity? visualDensity = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, Color? hoverColor = null, bool internalAddSemanticForOnTap = false)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget control = default!;
        switch (_switchListTileType)
        {
            case _SwitchListTileType__switch_list_tile.adaptive:
                {
                    control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeFocus(child: Switch.CreateAdaptive(value: value, onChanged: onChanged, activeColor: activeColor, activeThumbColor: activeThumbColor, activeThumbImage: activeThumbImage, inactiveThumbImage: inactiveThumbImage, materialTapTargetSize: materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap, activeTrackColor: activeTrackColor, inactiveTrackColor: inactiveTrackColor, inactiveThumbColor: inactiveThumbColor, autofocus: autofocus, onFocusChange: onFocusChange, onActiveThumbImageError: onActiveThumbImageError, onInactiveThumbImageError: onInactiveThumbImageError, thumbColor: thumbColor, trackColor: trackColor, trackOutlineColor: trackOutlineColor, thumbIcon: thumbIcon, applyCupertinoTheme: applyCupertinoTheme, dragStartBehavior: dragStartBehavior, mouseCursor: mouseCursor, splashRadius: splashRadius, overlayColor: overlayColor)));
                    break;
                }
            case _SwitchListTileType__switch_list_tile.material:
                {
                    control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeFocus(child: new Switch(value: value, onChanged: onChanged, activeColor: activeColor, activeThumbColor: activeThumbColor, activeThumbImage: activeThumbImage, inactiveThumbImage: inactiveThumbImage, materialTapTargetSize: materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap, activeTrackColor: activeTrackColor, inactiveTrackColor: inactiveTrackColor, inactiveThumbColor: inactiveThumbColor, autofocus: autofocus, onFocusChange: onFocusChange, onActiveThumbImageError: onActiveThumbImageError, onInactiveThumbImageError: onInactiveThumbImageError, thumbColor: thumbColor, trackColor: trackColor, trackOutlineColor: trackOutlineColor, thumbIcon: thumbIcon, dragStartBehavior: dragStartBehavior, mouseCursor: mouseCursor, splashRadius: splashRadius, overlayColor: overlayColor)));
                    break;
                }
        }
        ListTileThemeData listTileTheme = ListTileTheme.of(context);
        ListTileControlAffinity effectiveControlAffinity = (controlAffinity ?? listTileTheme.controlAffinity) ?? ListTileControlAffinity.platform;
        global::Doroti.Framework.Widgets.Widget? leadingLocal = default!;
        global::Doroti.Framework.Widgets.Widget? trailingLocal = default!;
        DartRuntimePrimitives.Ignore((leadingLocal, trailingLocal) = effectiveControlAffinity switch { var __constant23578 when Equals(__constant23578, ListTileControlAffinity.leading) => DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((control, secondary)), var __constant23641 when Equals(__constant23641, ListTileControlAffinity.trailing) => DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((secondary, control)), var __constant23677 when Equals(__constant23677, ListTileControlAffinity.platform) => DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((secondary, control)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        ThemeData theme = Theme.of(context);
        SwitchThemeData switchTheme = SwitchTheme.of(context);
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection23874 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (selected) { __collection23874.Add(WidgetState.selected); } return __collection23874; }))();
        global::Doroti.Ui.Color effectiveActiveColor = ((activeThumbColor ?? activeColor) ?? (switchTheme.thumbColor?.resolve(states))) ?? theme.colorScheme.secondary;
        return new global::Doroti.Framework.Widgets.MergeSemantics(child: new ListTile(selectedColor: effectiveActiveColor, leading: leadingLocal, title: title, subtitle: subtitle, trailing: trailingLocal, isThreeLine: isThreeLine, dense: dense, contentPadding: contentPadding, enabled: onChanged is not null, onTap: (onChanged is not null) ? (() =>
        {
            onChanged!(!value);
        }) : null, selected: selected, selectedTileColor: selectedTileColor, autofocus: autofocus, shape: shape, tileColor: tileColor, visualDensity: visualDensity, focusNode: focusNode, statesController: statesController, onFocusChange: onFocusChange, enableFeedback: enableFeedback, horizontalTitleGap: horizontalTitleGap, minVerticalPadding: minVerticalPadding, minLeadingWidth: minLeadingWidth, minTileHeight: minTileHeight, hoverColor: hoverColor, internalAddSemanticForOnTap: internalAddSemanticForOnTap));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

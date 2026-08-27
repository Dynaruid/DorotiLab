// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/checkbox_list_tile.dart
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

internal enum _CheckboxType__checkbox_list_tile
{
    material,
    adaptive
}

public class CheckboxListTile : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool? value { get; private set; }
    public virtual global::System.Action<bool?>? onChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual Color? checkColor { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual bool isError { get; private set; } = default!;
    public virtual Color? tileColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? secondary { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual bool tristate { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? checkboxShape { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    public virtual bool? enabled { get; private set; }
    public virtual ListTileTitleAlignment? titleAlignment { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;
    public virtual double checkboxScaleFactor { get; private set; } = default!;
    public virtual string? checkboxSemanticLabel { get; private set; }
    internal virtual _CheckboxType__checkbox_list_tile _checkboxType { get; private set; } = default!;

    public CheckboxListTile(global::Doroti.Framework.Foundation.Key? key = null, bool? value = default!, global::System.Action<bool?>? onChanged = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, bool autofocus = false, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.BorderSide? side = null, bool isError = false, bool? enabled = null, Color? tileColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool tristate = false, global::Doroti.Framework.Painting.OutlinedBorder? checkboxShape = null, Color? selectedTileColor = null, global::System.Action<bool>? onFocusChange = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, string? checkboxSemanticLabel = null, double checkboxScaleFactor = 1.0, ListTileTitleAlignment? titleAlignment = null, bool internalAddSemanticForOnTap = false) : base(key: key)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.activeColor = activeColor;
        this.fillColor = fillColor;
        this.checkColor = checkColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.visualDensity = visualDensity;
        this.focusNode = focusNode;
        this.statesController = statesController;
        this.autofocus = autofocus;
        this.shape = shape;
        this.side = side;
        this.isError = isError;
        this.enabled = enabled;
        this.tileColor = tileColor;
        this.title = title;
        this.subtitle = subtitle;
        this.isThreeLine = isThreeLine;
        this.dense = dense;
        this.secondary = secondary;
        this.selected = selected;
        this.controlAffinity = controlAffinity;
        this.contentPadding = contentPadding;
        this.tristate = tristate;
        this.checkboxShape = checkboxShape;
        this.selectedTileColor = selectedTileColor;
        this.onFocusChange = onFocusChange;
        this.enableFeedback = enableFeedback;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.minTileHeight = minTileHeight;
        this.checkboxSemanticLabel = checkboxSemanticLabel;
        this.checkboxScaleFactor = checkboxScaleFactor;
        this.titleAlignment = titleAlignment;
        this.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        this._checkboxType = _CheckboxType__checkbox_list_tile.material;
        System.Diagnostics.Debug.Assert((tristate || (value is not null)));
        System.Diagnostics.Debug.Assert(((isThreeLine != true) || (subtitle is not null)));
    }

    public static CheckboxListTile CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, bool? value = default!, global::System.Action<bool?>? onChanged = default!, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? checkColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, bool autofocus = false, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.BorderSide? side = null, bool isError = false, bool? enabled = null, Color? tileColor = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, bool tristate = false, global::Doroti.Framework.Painting.OutlinedBorder? checkboxShape = null, Color? selectedTileColor = null, global::System.Action<bool>? onFocusChange = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, string? checkboxSemanticLabel = null, double checkboxScaleFactor = 1.0, ListTileTitleAlignment? titleAlignment = null, bool internalAddSemanticForOnTap = false)
    {
        var __instance = new CheckboxListTile(key: key, value: value, onChanged: onChanged, mouseCursor: mouseCursor, activeColor: activeColor, fillColor: fillColor, checkColor: checkColor, hoverColor: hoverColor, overlayColor: overlayColor, splashRadius: splashRadius, materialTapTargetSize: materialTapTargetSize, visualDensity: visualDensity, focusNode: focusNode, statesController: statesController, autofocus: autofocus, shape: shape, side: side, isError: isError, enabled: enabled, tileColor: tileColor, title: title, subtitle: subtitle, isThreeLine: isThreeLine, dense: dense, secondary: secondary, selected: selected, controlAffinity: controlAffinity, contentPadding: contentPadding, tristate: tristate, checkboxShape: checkboxShape, selectedTileColor: selectedTileColor, onFocusChange: onFocusChange, enableFeedback: enableFeedback, horizontalTitleGap: horizontalTitleGap, minVerticalPadding: minVerticalPadding, minLeadingWidth: minLeadingWidth, minTileHeight: minTileHeight, checkboxSemanticLabel: checkboxSemanticLabel, checkboxScaleFactor: checkboxScaleFactor, titleAlignment: titleAlignment, internalAddSemanticForOnTap: internalAddSemanticForOnTap);
        __instance.value = value;
        __instance.onChanged = onChanged;
        __instance.mouseCursor = mouseCursor;
        __instance.activeColor = activeColor;
        __instance.fillColor = fillColor;
        __instance.checkColor = checkColor;
        __instance.hoverColor = hoverColor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.visualDensity = visualDensity;
        __instance.focusNode = focusNode;
        __instance.statesController = statesController;
        __instance.autofocus = autofocus;
        __instance.shape = shape;
        __instance.side = side;
        __instance.isError = isError;
        __instance.enabled = enabled;
        __instance.tileColor = tileColor;
        __instance.title = title;
        __instance.subtitle = subtitle;
        __instance.isThreeLine = isThreeLine;
        __instance.dense = dense;
        __instance.secondary = secondary;
        __instance.selected = selected;
        __instance.controlAffinity = controlAffinity;
        __instance.contentPadding = contentPadding;
        __instance.tristate = tristate;
        __instance.checkboxShape = checkboxShape;
        __instance.selectedTileColor = selectedTileColor;
        __instance.onFocusChange = onFocusChange;
        __instance.enableFeedback = enableFeedback;
        __instance.horizontalTitleGap = horizontalTitleGap;
        __instance.minVerticalPadding = minVerticalPadding;
        __instance.minLeadingWidth = minLeadingWidth;
        __instance.minTileHeight = minTileHeight;
        __instance.checkboxSemanticLabel = checkboxSemanticLabel;
        __instance.checkboxScaleFactor = checkboxScaleFactor;
        __instance.titleAlignment = titleAlignment;
        __instance.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        __instance._checkboxType = _CheckboxType__checkbox_list_tile.adaptive;
        return __instance;
    }

    internal virtual void _handleValueChange()
    {
        DartRuntimePrimitives.Assert(() => (this.onChanged is not null));
        switch (this.value)
        {
            case false:
                {
                    this.onChanged!(true);
                    break;
                }
            case true:
                {
                    this.onChanged!((this.tristate ? null : false));
                    break;
                }
            case null:
                {
                    this.onChanged!(false);
                    break;
                }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget control = default!;
        switch (this._checkboxType)
        {
            case _CheckboxType__checkbox_list_tile.material:
                {
                    control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeFocus(child: new Checkbox(value: this.value, onChanged: ((global::System.Action<bool?>)((this.enabled ?? true) ? this.onChanged : null)), mouseCursor: this.mouseCursor, activeColor: this.activeColor, fillColor: this.fillColor, checkColor: this.checkColor, hoverColor: this.hoverColor, overlayColor: this.overlayColor, splashRadius: this.splashRadius, materialTapTargetSize: (this.materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap), autofocus: this.autofocus, tristate: this.tristate, shape: this.checkboxShape, side: this.side, isError: this.isError, semanticLabel: this.checkboxSemanticLabel)));
                    break;
                }
            case _CheckboxType__checkbox_list_tile.adaptive:
                {
                    control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeFocus(child: Checkbox.CreateAdaptive(value: this.value, onChanged: ((global::System.Action<bool?>)((this.enabled ?? true) ? this.onChanged : null)), mouseCursor: this.mouseCursor, activeColor: this.activeColor, fillColor: this.fillColor, checkColor: this.checkColor, hoverColor: this.hoverColor, overlayColor: this.overlayColor, splashRadius: this.splashRadius, materialTapTargetSize: (this.materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap), autofocus: this.autofocus, tristate: this.tristate, shape: this.checkboxShape, side: this.side, isError: this.isError, semanticLabel: this.checkboxSemanticLabel)));
                    break;
                }
        }
        if ((this.checkboxScaleFactor != 1.0))
        {
            control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Transform.CreateScale(scale: this.checkboxScaleFactor, child: control));
        }
        ListTileThemeData listTileTheme = ListTileTheme.of(context);
        ListTileControlAffinity effectiveControlAffinity = ((this.controlAffinity ?? listTileTheme.controlAffinity) ?? ListTileControlAffinity.platform);
        var (leadingLocal, trailingLocal) = (effectiveControlAffinity switch { ListTileControlAffinity.leading => (((global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((control, this.secondary)))), ListTileControlAffinity.trailing => (((global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((this.secondary, control)))), ListTileControlAffinity.platform => (((global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((this.secondary, control)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        ThemeData theme = Theme.of(context);
        CheckboxThemeData checkboxTheme = CheckboxTheme.of(context);
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection21344 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (this.selected) { __collection21344.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection21344; }))();
        global::Doroti.Ui.Color effectiveActiveColor = ((global::Doroti.Ui.Color)(object?)(((this.activeColor ?? (Color)checkboxTheme.fillColor?.resolve(states))) ?? theme.colorScheme.secondary));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MergeSemantics(child: new ListTile(selectedColor: effectiveActiveColor, leading: leadingLocal, title: this.title, subtitle: this.subtitle, trailing: trailingLocal, isThreeLine: this.isThreeLine, dense: this.dense, enabled: (this.enabled ?? (this.onChanged is not null)), onTap: ((global::System.Action)((this.onChanged is not null) ? this._handleValueChange : null)), selected: this.selected, autofocus: this.autofocus, contentPadding: this.contentPadding, shape: this.shape, selectedTileColor: this.selectedTileColor, tileColor: this.tileColor, visualDensity: this.visualDensity, focusNode: this.focusNode, statesController: this.statesController, onFocusChange: (global::System.Action<bool>?)this.onFocusChange, enableFeedback: this.enableFeedback, horizontalTitleGap: this.horizontalTitleGap, minVerticalPadding: this.minVerticalPadding, minLeadingWidth: this.minLeadingWidth, minTileHeight: this.minTileHeight, titleAlignment: this.titleAlignment, internalAddSemanticForOnTap: this.internalAddSemanticForOnTap)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

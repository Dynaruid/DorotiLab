// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/radio_list_tile.dart
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

internal enum _RadioType__radio_list_tile
{
    material,
    adaptive
}

public class RadioListTile<T> : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual global::System.Action<T?>? onChanged { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? title { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? subtitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? secondary { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? tileColor { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController? statesController { get; private set; }
    public virtual global::System.Action<bool>? onFocusChange { get; private set; }
    public virtual bool? enableFeedback { get; private set; }
    public virtual double? horizontalTitleGap { get; private set; }
    public virtual double? minVerticalPadding { get; private set; }
    public virtual double? minLeadingWidth { get; private set; }
    public virtual double? minTileHeight { get; private set; }
    internal virtual _RadioType__radio_list_tile _radioType { get; private set; } = default!;
    public virtual ListTileTitleAlignment? titleAlignment { get; private set; }
    public virtual bool internalAddSemanticForOnTap { get; private set; } = default!;
    public virtual bool useCupertinoCheckmarkStyle { get; private set; } = default!;
    public virtual double radioScaleFactor { get; private set; } = default!;
    public virtual bool? enabled { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? radioBackgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? radioSide { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? radioInnerRadius { get; private set; }

    public RadioListTile(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default, global::System.Action<T?>? onChanged = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool toggleable = false, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, bool autofocus = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Color? tileColor = null, Color? selectedTileColor = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::System.Action<bool>? onFocusChange = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, double radioScaleFactor = 1.0, ListTileTitleAlignment? titleAlignment = null, bool? enabled = null, bool internalAddSemanticForOnTap = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? radioBackgroundColor = null, global::Doroti.Framework.Painting.BorderSide? radioSide = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? radioInnerRadius = null) : base(key: key)
    {
        this.value = value;
        this.groupValue = groupValue;
        this.onChanged = onChanged;
        this.mouseCursor = mouseCursor;
        this.toggleable = toggleable;
        this.activeColor = activeColor;
        this.fillColor = fillColor;
        this.hoverColor = hoverColor;
        this.overlayColor = overlayColor;
        this.splashRadius = splashRadius;
        this.materialTapTargetSize = materialTapTargetSize;
        this.title = title;
        this.subtitle = subtitle;
        this.isThreeLine = isThreeLine;
        this.dense = dense;
        this.secondary = secondary;
        this.selected = selected;
        this.controlAffinity = controlAffinity;
        this.autofocus = autofocus;
        this.contentPadding = contentPadding;
        this.shape = shape;
        this.tileColor = tileColor;
        this.selectedTileColor = selectedTileColor;
        this.visualDensity = visualDensity;
        this.focusNode = focusNode;
        this.statesController = statesController;
        this.onFocusChange = onFocusChange;
        this.enableFeedback = enableFeedback;
        this.horizontalTitleGap = horizontalTitleGap;
        this.minVerticalPadding = minVerticalPadding;
        this.minLeadingWidth = minLeadingWidth;
        this.minTileHeight = minTileHeight;
        this.radioScaleFactor = radioScaleFactor;
        this.titleAlignment = titleAlignment;
        this.enabled = enabled;
        this.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        this.radioBackgroundColor = radioBackgroundColor;
        this.radioSide = radioSide;
        this.radioInnerRadius = radioInnerRadius;
        this._radioType = _RadioType__radio_list_tile.material;
        this.useCupertinoCheckmarkStyle = false;
        System.Diagnostics.Debug.Assert(((isThreeLine != true) || (subtitle is not null)));
    }

    public static RadioListTile<T> CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, T value = default!, T? groupValue = default, global::System.Action<T?>? onChanged = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, bool toggleable = false, Color? activeColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? fillColor = null, Color? hoverColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, double? splashRadius = null, MaterialTapTargetSize? materialTapTargetSize = null, global::Doroti.Framework.Widgets.Widget? title = null, global::Doroti.Framework.Widgets.Widget? subtitle = null, bool? isThreeLine = null, bool? dense = null, global::Doroti.Framework.Widgets.Widget? secondary = null, bool selected = false, ListTileControlAffinity? controlAffinity = null, bool autofocus = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? contentPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Color? tileColor = null, Color? selectedTileColor = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, global::Doroti.Framework.Widgets.WidgetStatesController? statesController = null, global::System.Action<bool>? onFocusChange = null, bool? enableFeedback = null, double? horizontalTitleGap = null, double? minVerticalPadding = null, double? minLeadingWidth = null, double? minTileHeight = null, double radioScaleFactor = 1.0, bool? enabled = null, bool useCupertinoCheckmarkStyle = false, ListTileTitleAlignment? titleAlignment = null, bool internalAddSemanticForOnTap = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? radioBackgroundColor = null, global::Doroti.Framework.Painting.BorderSide? radioSide = null, global::Doroti.Framework.Widgets.WidgetStateProperty<double?>? radioInnerRadius = null)
    {
        var __instance = new RadioListTile<T>(key: key, value: value, groupValue: groupValue, onChanged: onChanged, mouseCursor: mouseCursor, toggleable: toggleable, activeColor: activeColor, fillColor: fillColor, hoverColor: hoverColor, overlayColor: overlayColor, splashRadius: splashRadius, materialTapTargetSize: materialTapTargetSize, title: title, subtitle: subtitle, isThreeLine: isThreeLine, dense: dense, secondary: secondary, selected: selected, controlAffinity: controlAffinity, autofocus: autofocus, contentPadding: contentPadding, shape: shape, tileColor: tileColor, selectedTileColor: selectedTileColor, visualDensity: visualDensity, focusNode: focusNode, statesController: statesController, onFocusChange: onFocusChange, enableFeedback: enableFeedback, horizontalTitleGap: horizontalTitleGap, minVerticalPadding: minVerticalPadding, minLeadingWidth: minLeadingWidth, minTileHeight: minTileHeight, radioScaleFactor: radioScaleFactor, titleAlignment: titleAlignment, enabled: enabled, internalAddSemanticForOnTap: internalAddSemanticForOnTap, radioBackgroundColor: radioBackgroundColor, radioSide: radioSide, radioInnerRadius: radioInnerRadius);
        __instance.value = value;
        __instance.groupValue = groupValue;
        __instance.onChanged = onChanged;
        __instance.mouseCursor = mouseCursor;
        __instance.toggleable = toggleable;
        __instance.activeColor = activeColor;
        __instance.fillColor = fillColor;
        __instance.hoverColor = hoverColor;
        __instance.overlayColor = overlayColor;
        __instance.splashRadius = splashRadius;
        __instance.materialTapTargetSize = materialTapTargetSize;
        __instance.title = title;
        __instance.subtitle = subtitle;
        __instance.isThreeLine = isThreeLine;
        __instance.dense = dense;
        __instance.secondary = secondary;
        __instance.selected = selected;
        __instance.controlAffinity = controlAffinity;
        __instance.autofocus = autofocus;
        __instance.contentPadding = contentPadding;
        __instance.shape = shape;
        __instance.tileColor = tileColor;
        __instance.selectedTileColor = selectedTileColor;
        __instance.visualDensity = visualDensity;
        __instance.focusNode = focusNode;
        __instance.statesController = statesController;
        __instance.onFocusChange = onFocusChange;
        __instance.enableFeedback = enableFeedback;
        __instance.horizontalTitleGap = horizontalTitleGap;
        __instance.minVerticalPadding = minVerticalPadding;
        __instance.minLeadingWidth = minLeadingWidth;
        __instance.minTileHeight = minTileHeight;
        __instance.radioScaleFactor = radioScaleFactor;
        __instance.enabled = enabled;
        __instance.useCupertinoCheckmarkStyle = useCupertinoCheckmarkStyle;
        __instance.titleAlignment = titleAlignment;
        __instance.internalAddSemanticForOnTap = internalAddSemanticForOnTap;
        __instance.radioBackgroundColor = radioBackgroundColor;
        __instance.radioSide = radioSide;
        __instance.radioInnerRadius = radioInnerRadius;
        __instance._radioType = _RadioType__radio_list_tile.adaptive;
        return __instance;
    }

    public virtual bool @checked => DartRuntimePrimitives.ConvertValue<bool>(EqualityComparer<T>.Default.Equals(this.value, this.groupValue));
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RadioListTileState__radio_list_tile<T>());
}

internal class _RadioListTileState__radio_list_tile<T> : global::Doroti.Framework.Widgets.State<RadioListTile<T>>, global::Doroti.Framework.Widgets.RadioClient<T>
{
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _internalFocusNode { get; set; } = default;
    private bool __late__radioRegistry_initialized;
    private _RadioRegistry__radio_list_tile<T> __late__radioRegistry = default!;
    internal virtual _RadioRegistry__radio_list_tile<T> _radioRegistry
    {
        get
        {
            if (!__late__radioRegistry_initialized)
            {
                __late__radioRegistry = new _RadioRegistry__radio_list_tile<T>(this);
                __late__radioRegistry_initialized = true;
            }
            return __late__radioRegistry;
        }
    }
    public virtual RadioGroupRegistry<T>? _registry { get; set; } = default;

    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((RadioListTile<T>)(object)this.widget).focusNode ?? (_internalFocusNode ??= new global::Doroti.Framework.Widgets.FocusNode())));
    public virtual T radioValue => ((RadioListTile<T>)(object)this.widget).value;
    public virtual bool tristate => ((RadioListTile<T>)(object)this.widget).toggleable;
    public virtual bool enabled => this._enabled;
    public virtual bool @checked => DartRuntimePrimitives.ConvertValue<bool>(EqualityComparer<T>.Default.Equals(this.radioValue, this.effectiveGroupValue));
    public virtual T? effectiveGroupValue => DartRuntimePrimitives.ConvertValue<T>((DartRuntimePrimitives.NullAware(this.registry, __target => __target.groupValue) ?? ((RadioListTile<T>)(object)this.widget).groupValue));
    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((RadioListTile<T>)(object)this.widget).enabled ?? (((((RadioListTile<T>)(object)this.widget).onChanged is not null) || (this.registry is not null)))));
    internal virtual void _handleListTileTap()
    {
        if ((!((RadioListTile<T>)(object)this.widget).toggleable && this.@checked))
        {
            return;
        }
        T? newValue = default!;
        if (this.@checked)
        {
            newValue = default(T);
        }
        else
        {
            newValue = this.radioValue;
        }
        handleChange(newValue);
    }

    public virtual void handleChange(T? value)
    {
        if ((this.registry is not null))
        {
            this.registry!.onChanged(value);
        }
        if ((((RadioListTile<T>)(object)this.widget).onChanged is not null))
        {
            ((RadioListTile<T>)(object)this.widget).onChanged!(value);
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        registry = RadioGroup.maybeOf<T>(this.context);
    }

    public override void dispose()
    {
        registry = null;
        this._internalFocusNode?.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((!((((RadioListTile<T>)(object)this.widget).enabled ?? false)) || (((RadioListTile<T>)(object)this.widget).onChanged is not null)) || (RadioGroup.maybeOf<T>(context) is not null)), () => (object?)"Radio is enabled but has no RadioListTile.onChange or registry above");
        global::Doroti.Framework.Widgets.Widget control = default!;
        switch (((RadioListTile<T>)(object)this.widget)._radioType)
        {
            case _RadioType__radio_list_tile.material:
                {
                    control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeFocus(child: new Radio<T>(value: this.radioValue, groupValue: ((_RadioRegistry__radio_list_tile<T>)this._radioRegistry).groupValue, toggleable: ((RadioListTile<T>)(object)this.widget).toggleable, activeColor: ((RadioListTile<T>)(object)this.widget).activeColor, materialTapTargetSize: (((RadioListTile<T>)(object)this.widget).materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap), autofocus: ((RadioListTile<T>)(object)this.widget).autofocus, fillColor: ((RadioListTile<T>)(object)this.widget).fillColor, mouseCursor: ((RadioListTile<T>)(object)this.widget).mouseCursor, hoverColor: ((RadioListTile<T>)(object)this.widget).hoverColor, overlayColor: ((RadioListTile<T>)(object)this.widget).overlayColor, splashRadius: ((RadioListTile<T>)(object)this.widget).splashRadius, enabled: this._enabled, groupRegistry: this._radioRegistry, backgroundColor: ((RadioListTile<T>)(object)this.widget).radioBackgroundColor, side: ((RadioListTile<T>)(object)this.widget).radioSide, innerRadius: ((RadioListTile<T>)(object)this.widget).radioInnerRadius)));
                    break;
                }
            case _RadioType__radio_list_tile.adaptive:
                {
                    control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeFocus(child: Radio<T>.CreateAdaptive(value: this.radioValue, groupValue: ((_RadioRegistry__radio_list_tile<T>)this._radioRegistry).groupValue, toggleable: ((RadioListTile<T>)(object)this.widget).toggleable, activeColor: ((RadioListTile<T>)(object)this.widget).activeColor, materialTapTargetSize: (((RadioListTile<T>)(object)this.widget).materialTapTargetSize ?? MaterialTapTargetSize.shrinkWrap), autofocus: ((RadioListTile<T>)(object)this.widget).autofocus, fillColor: ((RadioListTile<T>)(object)this.widget).fillColor, mouseCursor: ((RadioListTile<T>)(object)this.widget).mouseCursor, hoverColor: ((RadioListTile<T>)(object)this.widget).hoverColor, overlayColor: ((RadioListTile<T>)(object)this.widget).overlayColor, splashRadius: ((RadioListTile<T>)(object)this.widget).splashRadius, useCupertinoCheckmarkStyle: ((RadioListTile<T>)(object)this.widget).useCupertinoCheckmarkStyle, enabled: this._enabled, groupRegistry: this._radioRegistry, backgroundColor: ((RadioListTile<T>)(object)this.widget).radioBackgroundColor, side: ((RadioListTile<T>)(object)this.widget).radioSide, innerRadius: ((RadioListTile<T>)(object)this.widget).radioInnerRadius)));
                    break;
                }
        }
        if ((((RadioListTile<T>)(object)this.widget).radioScaleFactor != 1.0))
        {
            control = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Transform.CreateScale(scale: ((RadioListTile<T>)(object)this.widget).radioScaleFactor, child: control));
        }
        ListTileThemeData listTileTheme = ListTileTheme.of(context);
        ListTileControlAffinity effectiveControlAffinity = ((((RadioListTile<T>)(object)this.widget).controlAffinity ?? listTileTheme.controlAffinity) ?? ListTileControlAffinity.platform);
        global::Doroti.Framework.Widgets.Widget? leadingLocal = default!;
        global::Doroti.Framework.Widgets.Widget? trailingLocal = default!;
        DartRuntimePrimitives.Ignore((leadingLocal, trailingLocal) = (effectiveControlAffinity switch { var __constant24953 when (object.Equals(__constant24953, ListTileControlAffinity.leading)) => (((global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((control, ((RadioListTile<T>)(object)this.widget).secondary)))), var __constant24994 when (object.Equals(__constant24994, ListTileControlAffinity.platform)) => (((global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((control, ((RadioListTile<T>)(object)this.widget).secondary)))), var __constant25065 when (object.Equals(__constant25065, ListTileControlAffinity.trailing)) => (((global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?))(DartRuntimePrimitives.ConvertValue<(global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget?)>((((RadioListTile<T>)(object)this.widget).secondary, control)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        ThemeData theme = Theme.of(context);
        RadioThemeData radioThemeData = RadioTheme.of(context);
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection25269 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((RadioListTile<T>)(object)this.widget).selected) { __collection25269.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection25269; }))();
        global::Doroti.Ui.Color effectiveActiveColor = ((global::Doroti.Ui.Color)(object?)(((((RadioListTile<T>)(object)this.widget).activeColor ?? (Color)radioThemeData.fillColor?.resolve(states))) ?? theme.colorScheme.secondary));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MergeSemantics(child: new ListTile(selectedColor: effectiveActiveColor, leading: leadingLocal, title: ((RadioListTile<T>)(object)this.widget).title, subtitle: ((RadioListTile<T>)(object)this.widget).subtitle, trailing: trailingLocal, isThreeLine: ((RadioListTile<T>)(object)this.widget).isThreeLine, dense: ((RadioListTile<T>)(object)this.widget).dense, enabled: this._enabled, shape: ((RadioListTile<T>)(object)this.widget).shape, tileColor: ((RadioListTile<T>)(object)this.widget).tileColor, selectedTileColor: ((RadioListTile<T>)(object)this.widget).selectedTileColor, onTap: ((global::System.Action)(this._enabled ? this._handleListTileTap : null)), selected: ((RadioListTile<T>)(object)this.widget).selected, autofocus: ((RadioListTile<T>)(object)this.widget).autofocus, contentPadding: ((RadioListTile<T>)(object)this.widget).contentPadding, visualDensity: ((RadioListTile<T>)(object)this.widget).visualDensity, focusNode: this.focusNode, statesController: ((RadioListTile<T>)(object)this.widget).statesController, onFocusChange: ((RadioListTile<T>)(object)this.widget).onFocusChange, enableFeedback: ((RadioListTile<T>)(object)this.widget).enableFeedback, horizontalTitleGap: ((RadioListTile<T>)(object)this.widget).horizontalTitleGap, minVerticalPadding: ((RadioListTile<T>)(object)this.widget).minVerticalPadding, minLeadingWidth: ((RadioListTile<T>)(object)this.widget).minLeadingWidth, minTileHeight: ((RadioListTile<T>)(object)this.widget).minTileHeight, titleAlignment: ((RadioListTile<T>)(object)this.widget).titleAlignment, internalAddSemanticForOnTap: ((RadioListTile<T>)(object)this.widget).internalAddSemanticForOnTap)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => this._registry;
        set
        {
            var newRegistry = value;
            if ((!object.Equals(this._registry, newRegistry)))
            {
                this._registry?.unregisterClient(this);
            }
            this._registry = newRegistry;
            this._registry?.registerClient(this);
        }
    }
}

internal class _RadioRegistry__radio_list_tile<T> : global::Doroti.Framework.Widgets.RadioGroupRegistry<T>
{
    public virtual _RadioListTileState__radio_list_tile<T> state { get; private set; } = default!;

    internal _RadioRegistry__radio_list_tile(_RadioListTileState__radio_list_tile<T> state)
    {
        this.state = state;
    }

    public virtual T? groupValue => ((_RadioListTileState__radio_list_tile<T>)this.state).effectiveGroupValue;
    public virtual global::System.Action<T?> onChanged => ((_RadioListTileState__radio_list_tile<T>)this.state).handleChange;
    public virtual void registerClient(global::Doroti.Framework.Widgets.RadioClient<T> radio)
    {
    }

    public virtual void unregisterClient(global::Doroti.Framework.Widgets.RadioClient<T> radio)
    {
    }

}

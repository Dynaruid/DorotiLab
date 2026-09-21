// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/radio_list_tile.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal enum _RadioType__radio_list_tile
{
    material,
    adaptive,
}

public class RadioListTile<T> : StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual Action<T?>? onChanged { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool toggleable { get; private set; } = default!;
    public virtual Color? activeColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? fillColor { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual Color? hoverColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double? splashRadius { get; private set; }
    public virtual Widget? title { get; private set; }
    public virtual Widget? subtitle { get; private set; }
    public virtual Widget? secondary { get; private set; }
    public virtual bool? isThreeLine { get; private set; }
    public virtual bool? dense { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual ListTileControlAffinity? controlAffinity { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? contentPadding { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Color? tileColor { get; private set; }
    public virtual Color? selectedTileColor { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual WidgetStatesController? statesController { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
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
    public virtual WidgetStateProperty<Color?>? radioBackgroundColor { get; private set; }
    public virtual BorderSide? radioSide { get; private set; }
    public virtual WidgetStateProperty<double?>? radioInnerRadius { get; private set; }

    public RadioListTile(
        Key? key = null,
        T value = default!,
        T? groupValue = default,
        Action<T?>? onChanged = null,
        MouseCursor? mouseCursor = null,
        bool toggleable = false,
        Color? activeColor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        Color? hoverColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        Widget? title = null,
        Widget? subtitle = null,
        bool? isThreeLine = null,
        bool? dense = null,
        Widget? secondary = null,
        bool selected = false,
        ListTileControlAffinity? controlAffinity = null,
        bool autofocus = false,
        EdgeInsetsGeometry? contentPadding = null,
        ShapeBorder? shape = null,
        Color? tileColor = null,
        Color? selectedTileColor = null,
        VisualDensity? visualDensity = null,
        FocusNode? focusNode = null,
        WidgetStatesController? statesController = null,
        Action<bool>? onFocusChange = null,
        bool? enableFeedback = null,
        double? horizontalTitleGap = null,
        double? minVerticalPadding = null,
        double? minLeadingWidth = null,
        double? minTileHeight = null,
        double radioScaleFactor = 1.0,
        ListTileTitleAlignment? titleAlignment = null,
        bool? enabled = null,
        bool internalAddSemanticForOnTap = false,
        WidgetStateProperty<Color?>? radioBackgroundColor = null,
        BorderSide? radioSide = null,
        WidgetStateProperty<double?>? radioInnerRadius = null
    )
        : base(key: key)
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
        _radioType = _RadioType__radio_list_tile.material;
        useCupertinoCheckmarkStyle = false;
        System.Diagnostics.Debug.Assert((isThreeLine != true) || (subtitle is not null));
    }

    public static RadioListTile<T> CreateAdaptive(
        Key? key = null,
        T value = default!,
        T? groupValue = default,
        Action<T?>? onChanged = null,
        MouseCursor? mouseCursor = null,
        bool toggleable = false,
        Color? activeColor = null,
        WidgetStateProperty<Color?>? fillColor = null,
        Color? hoverColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        double? splashRadius = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        Widget? title = null,
        Widget? subtitle = null,
        bool? isThreeLine = null,
        bool? dense = null,
        Widget? secondary = null,
        bool selected = false,
        ListTileControlAffinity? controlAffinity = null,
        bool autofocus = false,
        EdgeInsetsGeometry? contentPadding = null,
        ShapeBorder? shape = null,
        Color? tileColor = null,
        Color? selectedTileColor = null,
        VisualDensity? visualDensity = null,
        FocusNode? focusNode = null,
        WidgetStatesController? statesController = null,
        Action<bool>? onFocusChange = null,
        bool? enableFeedback = null,
        double? horizontalTitleGap = null,
        double? minVerticalPadding = null,
        double? minLeadingWidth = null,
        double? minTileHeight = null,
        double radioScaleFactor = 1.0,
        bool? enabled = null,
        bool useCupertinoCheckmarkStyle = false,
        ListTileTitleAlignment? titleAlignment = null,
        bool internalAddSemanticForOnTap = false,
        WidgetStateProperty<Color?>? radioBackgroundColor = null,
        BorderSide? radioSide = null,
        WidgetStateProperty<double?>? radioInnerRadius = null
    )
    {
        var __instance = new RadioListTile<T>(
            key: key,
            value: value,
            groupValue: groupValue,
            onChanged: onChanged,
            mouseCursor: mouseCursor,
            toggleable: toggleable,
            activeColor: activeColor,
            fillColor: fillColor,
            hoverColor: hoverColor,
            overlayColor: overlayColor,
            splashRadius: splashRadius,
            materialTapTargetSize: materialTapTargetSize,
            title: title,
            subtitle: subtitle,
            isThreeLine: isThreeLine,
            dense: dense,
            secondary: secondary,
            selected: selected,
            controlAffinity: controlAffinity,
            autofocus: autofocus,
            contentPadding: contentPadding,
            shape: shape,
            tileColor: tileColor,
            selectedTileColor: selectedTileColor,
            visualDensity: visualDensity,
            focusNode: focusNode,
            statesController: statesController,
            onFocusChange: onFocusChange,
            enableFeedback: enableFeedback,
            horizontalTitleGap: horizontalTitleGap,
            minVerticalPadding: minVerticalPadding,
            minLeadingWidth: minLeadingWidth,
            minTileHeight: minTileHeight,
            radioScaleFactor: radioScaleFactor,
            titleAlignment: titleAlignment,
            enabled: enabled,
            internalAddSemanticForOnTap: internalAddSemanticForOnTap,
            radioBackgroundColor: radioBackgroundColor,
            radioSide: radioSide,
            radioInnerRadius: radioInnerRadius
        );
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

    public virtual bool @checked =>
        DartRuntimePrimitives.ConvertValue<bool>(
            EqualityComparer<T>.Default.Equals(value, groupValue)
        );

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RadioListTileState__radio_list_tile<T>());
}

internal class _RadioListTileState__radio_list_tile<T> : State<RadioListTile<T>>, RadioClient<T>
{
    internal virtual FocusNode? _internalFocusNode { get; set; } = default;
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

    public virtual FocusNode focusNode =>
        DartRuntimePrimitives.ConvertValue<FocusNode>(
            widget.focusNode ?? (_internalFocusNode ??= new FocusNode())
        );
    public virtual T radioValue => widget.value;
    public virtual bool tristate => widget.toggleable;
    public virtual bool enabled => _enabled;
    public virtual bool @checked =>
        DartRuntimePrimitives.ConvertValue<bool>(
            EqualityComparer<T>.Default.Equals(radioValue, effectiveGroupValue)
        );
    public virtual T? effectiveGroupValue =>
        DartRuntimePrimitives.ConvertValue<T>(
            DartRuntimePrimitives.NullAware(registry, __target => __target.groupValue)
                ?? widget.groupValue
        );
    internal virtual bool _enabled =>
        DartRuntimePrimitives.ConvertValue<bool>(
            widget.enabled ?? ((widget.onChanged is not null) || (registry is not null))
        );

    internal virtual void _handleListTileTap()
    {
        if (!widget.toggleable && @checked)
        {
            return;
        }
        T? newValue = default!;
        if (@checked)
        {
            newValue = default(T);
        }
        else
        {
            newValue = radioValue;
        }
        handleChange(newValue);
    }

    public virtual void handleChange(T? value)
    {
        if (registry is not null)
        {
            registry!.onChanged(value);
        }
        if (widget.onChanged is not null)
        {
            widget.onChanged!(value);
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        registry = RadioGroup.maybeOf<T>(context);
    }

    public override void dispose()
    {
        registry = null;
        _internalFocusNode?.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(
            () =>
                !(widget.enabled ?? false)
                || (widget.onChanged is not null)
                || (RadioGroup.maybeOf<T>(context) is not null),
            () => (object?)"Radio is enabled but has no RadioListTile.onChange or registry above"
        );
        Widget control = default!;
        switch (widget._radioType)
        {
            case _RadioType__radio_list_tile.material:
            {
                control = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ExcludeFocus(
                        child: new Radio<T>(
                            value: radioValue,
                            groupValue: _radioRegistry.groupValue,
                            toggleable: widget.toggleable,
                            activeColor: widget.activeColor,
                            materialTapTargetSize: widget.materialTapTargetSize
                                ?? MaterialTapTargetSize.shrinkWrap,
                            autofocus: widget.autofocus,
                            fillColor: widget.fillColor,
                            mouseCursor: widget.mouseCursor,
                            hoverColor: widget.hoverColor,
                            overlayColor: widget.overlayColor,
                            splashRadius: widget.splashRadius,
                            enabled: _enabled,
                            groupRegistry: _radioRegistry,
                            backgroundColor: widget.radioBackgroundColor,
                            side: widget.radioSide,
                            innerRadius: widget.radioInnerRadius
                        )
                    )
                );
                break;
            }
            case _RadioType__radio_list_tile.adaptive:
            {
                control = DartRuntimePrimitives.ConvertValue<Widget>(
                    new ExcludeFocus(
                        child: Radio<T>.CreateAdaptive(
                            value: radioValue,
                            groupValue: _radioRegistry.groupValue,
                            toggleable: widget.toggleable,
                            activeColor: widget.activeColor,
                            materialTapTargetSize: widget.materialTapTargetSize
                                ?? MaterialTapTargetSize.shrinkWrap,
                            autofocus: widget.autofocus,
                            fillColor: widget.fillColor,
                            mouseCursor: widget.mouseCursor,
                            hoverColor: widget.hoverColor,
                            overlayColor: widget.overlayColor,
                            splashRadius: widget.splashRadius,
                            useCupertinoCheckmarkStyle: widget.useCupertinoCheckmarkStyle,
                            enabled: _enabled,
                            groupRegistry: _radioRegistry,
                            backgroundColor: widget.radioBackgroundColor,
                            side: widget.radioSide,
                            innerRadius: widget.radioInnerRadius
                        )
                    )
                );
                break;
            }
        }
        if (widget.radioScaleFactor != 1.0)
        {
            control = DartRuntimePrimitives.ConvertValue<Widget>(
                Transform.CreateScale(scale: widget.radioScaleFactor, child: control)
            );
        }
        ListTileThemeData listTileTheme = ListTileTheme.of(context);
        ListTileControlAffinity effectiveControlAffinity =
            (widget.controlAffinity ?? listTileTheme.controlAffinity)
            ?? ListTileControlAffinity.platform;
        Widget? leadingLocal = default!;
        Widget? trailingLocal = default!;
        DartRuntimePrimitives.Ignore(
            (leadingLocal, trailingLocal) = effectiveControlAffinity switch
            {
                var __constant24953 when Equals(__constant24953, ListTileControlAffinity.leading) =>
                    DartRuntimePrimitives.ConvertValue<(Widget?, Widget?)>(
                        (control, widget.secondary)
                    ),
                var __constant24994
                    when Equals(__constant24994, ListTileControlAffinity.platform) =>
                    DartRuntimePrimitives.ConvertValue<(Widget?, Widget?)>(
                        (control, widget.secondary)
                    ),
                var __constant25065
                    when Equals(__constant25065, ListTileControlAffinity.trailing) =>
                    DartRuntimePrimitives.ConvertValue<(Widget?, Widget?)>(
                        (widget.secondary, control)
                    ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            }
        );
        ThemeData theme = Theme.of(context);
        RadioThemeData radioThemeData = RadioTheme.of(context);
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection25269 = new HashSet<WidgetState>();
                    if (widget.selected)
                    {
                        __collection25269.Add(WidgetState.selected);
                    }
                    return __collection25269;
                }
            )
        )();
        Color effectiveActiveColor =
            (widget.activeColor ?? (radioThemeData.fillColor?.resolve(states)))
            ?? theme.colorScheme.secondary;
        return new MergeSemantics(
            child: new ListTile(
                selectedColor: effectiveActiveColor,
                leading: leadingLocal,
                title: widget.title,
                subtitle: widget.subtitle,
                trailing: trailingLocal,
                isThreeLine: widget.isThreeLine,
                dense: widget.dense,
                enabled: _enabled,
                shape: widget.shape,
                tileColor: widget.tileColor,
                selectedTileColor: widget.selectedTileColor,
                onTap: _enabled ? _handleListTileTap : null,
                selected: widget.selected,
                autofocus: widget.autofocus,
                contentPadding: widget.contentPadding,
                visualDensity: widget.visualDensity,
                focusNode: focusNode,
                statesController: widget.statesController,
                onFocusChange: widget.onFocusChange,
                enableFeedback: widget.enableFeedback,
                horizontalTitleGap: widget.horizontalTitleGap,
                minVerticalPadding: widget.minVerticalPadding,
                minLeadingWidth: widget.minLeadingWidth,
                minTileHeight: widget.minTileHeight,
                titleAlignment: widget.titleAlignment,
                internalAddSemanticForOnTap: widget.internalAddSemanticForOnTap
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => _registry;
        set
        {
            var newRegistry = value;
            if (!Equals(_registry, newRegistry))
            {
                _registry?.unregisterClient(this);
            }
            _registry = newRegistry;
            _registry?.registerClient(this);
        }
    }
}

internal class _RadioRegistry__radio_list_tile<T> : RadioGroupRegistry<T>
{
    public virtual _RadioListTileState__radio_list_tile<T> state { get; private set; } = default!;

    internal _RadioRegistry__radio_list_tile(_RadioListTileState__radio_list_tile<T> state)
    {
        this.state = state;
    }

    public virtual T? groupValue => state.effectiveGroupValue;
    public virtual Action<T?> onChanged => state.handleChange;

    public virtual void registerClient(RadioClient<T> radio) { }

    public virtual void unregisterClient(RadioClient<T> radio) { }
}

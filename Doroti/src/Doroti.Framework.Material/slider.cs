// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate void PaintValueIndicator(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset);

internal enum _SliderType__slider
{
    material,
    adaptive
}

public enum SliderInteraction
{
    tapAndSlide,
    tapOnly,
    slideOnly,
    slideThumb
}

public class Slider : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual double value { get; private set; } = default!;
    public virtual double? secondaryTrackValue { get; private set; }
    public virtual global::System.Action<double>? onChanged { get; private set; }
    public virtual global::System.Action<double>? onChangeStart { get; private set; }
    public virtual global::System.Action<double>? onChangeEnd { get; private set; }
    public virtual double min { get; private set; } = default!;
    public virtual double max { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual string? label { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual Color? secondaryActiveColor { get; private set; }
    public virtual Color? thumbColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual SemanticFormatterCallback? semanticFormatterCallback { get; private set; }
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual SliderInteraction? allowedInteraction { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual ShowValueIndicator? showValueIndicator { get; private set; }
    public virtual bool? year2023 { get; private set; }
    internal virtual _SliderType__slider _sliderType { get; private set; } = default!;

    public Slider(global::Doroti.Framework.Foundation.Key? key = null, double value = default!, double? secondaryTrackValue = null, global::System.Action<double>? onChanged = default!, global::System.Action<double>? onChangeStart = null, global::System.Action<double>? onChangeEnd = null, double min = 0.0, double max = 1.0, long? divisions = null, string? label = null, Color? activeColor = null, Color? inactiveColor = null, Color? secondaryActiveColor = null, Color? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, SemanticFormatterCallback? semanticFormatterCallback = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, SliderInteraction? allowedInteraction = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, ShowValueIndicator? showValueIndicator = null, bool? year2023 = null) : base(key: key)
    {
        this.value = value;
        this.secondaryTrackValue = secondaryTrackValue;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.min = min;
        this.max = max;
        this.divisions = divisions;
        this.label = label;
        this.activeColor = activeColor;
        this.inactiveColor = inactiveColor;
        this.secondaryActiveColor = secondaryActiveColor;
        this.thumbColor = thumbColor;
        this.overlayColor = overlayColor;
        this.mouseCursor = mouseCursor;
        this.semanticFormatterCallback = semanticFormatterCallback;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.allowedInteraction = allowedInteraction;
        this.padding = padding;
        this.showValueIndicator = showValueIndicator;
        this.year2023 = year2023;
        _sliderType = _SliderType__slider.material;
        System.Diagnostics.Debug.Assert(min <= max);
        System.Diagnostics.Debug.Assert((value >= min) && (value <= max));
        System.Diagnostics.Debug.Assert((secondaryTrackValue is null) || (secondaryTrackValue >= min) && (secondaryTrackValue <= max));
        System.Diagnostics.Debug.Assert((divisions is null) || (DartRuntimePrimitives.RequireValue(divisions) > 0L));
    }

    public static Slider CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, double value = default!, double? secondaryTrackValue = null, global::System.Action<double>? onChanged = default!, global::System.Action<double>? onChangeStart = null, global::System.Action<double>? onChangeEnd = null, double min = 0.0, double max = 1.0, long? divisions = null, string? label = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null, Color? activeColor = null, Color? inactiveColor = null, Color? secondaryActiveColor = null, Color? thumbColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, SemanticFormatterCallback? semanticFormatterCallback = null, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, SliderInteraction? allowedInteraction = null, ShowValueIndicator? showValueIndicator = null, bool? year2023 = null)
    {
        var __instance = new Slider(key: key, value: value, secondaryTrackValue: secondaryTrackValue, onChanged: onChanged, onChangeStart: onChangeStart, onChangeEnd: onChangeEnd, min: min, max: max, divisions: divisions, label: label, activeColor: activeColor, inactiveColor: inactiveColor, secondaryActiveColor: secondaryActiveColor, thumbColor: thumbColor, overlayColor: overlayColor, mouseCursor: mouseCursor, semanticFormatterCallback: semanticFormatterCallback, focusNode: focusNode, autofocus: autofocus, allowedInteraction: allowedInteraction, showValueIndicator: showValueIndicator, year2023: year2023);
        __instance.value = value;
        __instance.secondaryTrackValue = secondaryTrackValue;
        __instance.onChanged = onChanged;
        __instance.onChangeStart = onChangeStart;
        __instance.onChangeEnd = onChangeEnd;
        __instance.min = min;
        __instance.max = max;
        __instance.divisions = divisions;
        __instance.label = label;
        __instance.mouseCursor = mouseCursor;
        __instance.activeColor = activeColor;
        __instance.inactiveColor = inactiveColor;
        __instance.secondaryActiveColor = secondaryActiveColor;
        __instance.thumbColor = thumbColor;
        __instance.overlayColor = overlayColor;
        __instance.semanticFormatterCallback = semanticFormatterCallback;
        __instance.focusNode = focusNode;
        __instance.autofocus = autofocus;
        __instance.allowedInteraction = allowedInteraction;
        __instance.showValueIndicator = showValueIndicator;
        __instance.year2023 = year2023;
        __instance._sliderType = _SliderType__slider.adaptive;
        __instance.padding = null;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _SliderState__slider());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("value", value));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("secondaryTrackValue", secondaryTrackValue));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<double>>("onChanged", onChanged, ifNull: "disabled"));
        properties.add(ObjectFlagProperty<System.Action<double>>.CreateHas("onChangeStart", onChangeStart));
        properties.add(ObjectFlagProperty<System.Action<double>>.CreateHas("onChangeEnd", onChangeEnd));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("min", min));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("max", max));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("divisions", divisions));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("label", label));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("activeColor", activeColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inactiveColor", inactiveColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryActiveColor", secondaryActiveColor));
        properties.add(ObjectFlagProperty<SemanticFormatterCallback>.CreateHas("semanticFormatterCallback", semanticFormatterCallback));
        properties.add(ObjectFlagProperty<FocusNode>.CreateHas("focusNode", focusNode));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("autofocus", value: autofocus, ifTrue: "autofocus"));
    }

}

public class _SliderState__slider : global::Doroti.Framework.Widgets.State<Slider>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<Slider>
{
    public static Duration enableAnimationDuration = Duration.Create(milliseconds: 75L);
    public static Duration valueIndicatorAnimationDuration = Duration.Create(milliseconds: 100L);
    public virtual global::Doroti.Framework.Animation.AnimationController overlayController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController valueIndicatorController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController enableController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController positionController { get; set; } = default!;
    public virtual Timer? interactionTimer { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _renderObjectKey { get; private set; } = GlobalKey<IState>.Create();
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _traditionalNavShortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowUp)] = _AdjustSliderIntent__slider.CreateUp(), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowDown)] = _AdjustSliderIntent__slider.CreateDown(), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowLeft)] = _AdjustSliderIntent__slider.CreateLeft(), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowRight)] = _AdjustSliderIntent__slider.CreateRight() };
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _directionalNavShortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowLeft)] = _AdjustSliderIntent__slider.CreateLeft(), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowRight)] = _AdjustSliderIntent__slider.CreateRight() };
    internal virtual DartMap<Type, dynamic> _actionMap { get; set; } = default!;
    public virtual global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>? paintValueIndicator { get; set; } = default;
    internal virtual bool _dragging { get; set; } = false;
    internal virtual double? _currentChangedValue { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode? _focusNode { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.OverlayPortalController _valueIndicatorOverlayPortalController { get; private set; } = ((Func<global::Doroti.Framework.Widgets.OverlayPortalController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Widgets.OverlayPortalController(debugLabel: "Slider ValueIndicator");
    __cascade.show();
    return __cascade;
}))();
    internal virtual bool _focused { get; set; } = false;
    internal virtual bool _hovering { get; set; } = false;
    internal virtual global::Doroti.Framework.Rendering.LayerLink _layerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>(widget.onChanged is not null);
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>(widget.focusNode ?? _focusNode!);
    public override void initState()
    {
        base.initState();
        overlayController = new global::Doroti.Framework.Animation.AnimationController(duration: ConstantsLibrary.kRadialReactionDuration, vsync: this);
        valueIndicatorController = new global::Doroti.Framework.Animation.AnimationController(duration: valueIndicatorAnimationDuration, vsync: this);
        enableController = new global::Doroti.Framework.Animation.AnimationController(duration: enableAnimationDuration, vsync: this);
        positionController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.zero, vsync: this);
        enableController.value = (widget.onChanged is not null) ? 1.0 : 0.0;
        positionController.value = _convert(widget.value);
        _actionMap = new DartMap<Type, dynamic> { [typeof(_AdjustSliderIntent__slider)] = new global::Doroti.Framework.Widgets.CallbackAction<_AdjustSliderIntent__slider>(onInvoke: (__arg0) => { ((global::System.Action<_AdjustSliderIntent__slider>)_actionHandler)(__arg0); return default!; }) };
        if (widget.focusNode is null)
        {
            _focusNode ??= new global::Doroti.Framework.Widgets.FocusNode();
        }
    }

    public override void dispose()
    {
        interactionTimer?.cancel();
        overlayController.dispose();
        valueIndicatorController.dispose();
        enableController.dispose();
        positionController.dispose();
        _focusNode?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleChanged(double value)
    {
        DartRuntimePrimitives.Assert(() => widget.onChanged is not null);
        double lerpValue = _lerp(value);
        if (_currentChangedValue != lerpValue)
        {
            _currentChangedValue = lerpValue;
            if (_currentChangedValue != widget.value)
            {
                widget.onChanged!(DartRuntimePrimitives.RequireValue(_currentChangedValue));
            }
        }
    }

    internal virtual void _handleDragStart(double value)
    {
        setState(() =>
        {
            _dragging = true;
        });
        widget.onChangeStart?.Invoke(_lerp(value));
    }

    internal virtual void _handleDragEnd(double value)
    {
        setState(() =>
        {
            _dragging = false;
        });
        _currentChangedValue = null;
        widget.onChangeEnd?.Invoke(_lerp(value));
    }

    internal virtual void _actionHandler(_AdjustSliderIntent__slider intent)
    {
        global::Doroti.Ui.TextDirection directionality = Directionality.of(_renderObjectKey.currentContext!);
        bool shouldIncrease = intent.type switch { _SliderAdjustmentType__slider.up => true,_SliderAdjustmentType__slider.down => false,_SliderAdjustmentType__slider.left => Equals(directionality, TextDirection.rtl),_SliderAdjustmentType__slider.right => Equals(directionality, TextDirection.ltr),_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var slider = ((_RenderSlider__slider?)_renderObjectKey.currentContext!.findRenderObject()!)!;
        if (shouldIncrease) { slider.increaseAction(); } else { slider.decreaseAction(); }
        return;
    }

    internal virtual void _handleFocusHighlightChanged(bool focused)
    {
        if (focused != _focused)
        {
            setState(() =>
            {
                _focused = focused;
            });
        }
    }

    internal virtual void _handleHoverChanged(bool hovering)
    {
        if (hovering != _hovering)
        {
            setState(() =>
            {
                _hovering = hovering;
            });
        }
    }

    internal virtual double _lerp(double value)
    {
        DartRuntimePrimitives.Assert(() => value >= 0.0);
        DartRuntimePrimitives.Assert(() => value <= 1.0);
        return (value * (widget.max - widget.min)) + widget.min;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _discretize(double value)
    {
        DartRuntimePrimitives.Assert(() => widget.divisions is not null);
        DartRuntimePrimitives.Assert(() => (value >= 0.0) && (value <= 1.0));
        long divisionsLocal = DartRuntimePrimitives.RequireValue(widget.divisions);
        return (value * divisionsLocal).round() / (double)divisionsLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _convert(double value)
    {
        double ret = _unlerp(value);
        if (widget.divisions is not null)
        {
            ret = _discretize(ret);
        }
        return ret;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _unlerp(double value)
    {
        DartRuntimePrimitives.Assert(() => value <= widget.max);
        DartRuntimePrimitives.Assert(() => value >= widget.min);
        return (widget.max > widget.min) ? ((value - widget.min) / (widget.max - widget.min)) : 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        switch (widget._sliderType)
        {
            case _SliderType__slider.material:
                {
                    return _buildMaterialSlider(context);
                }
            case _SliderType__slider.adaptive:
                {
                    ThemeData theme = Theme.of(context);
                    switch (theme.platform)
                    {
                        case TargetPlatform.android:
                        case TargetPlatform.fuchsia:
                        case TargetPlatform.linux:
                        case TargetPlatform.windows:
                            {
                                return _buildMaterialSlider(context);
                            }
                        case TargetPlatform.iOS:
                        case TargetPlatform.macOS:
                            {
                                return _buildCupertinoSlider(context);
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMaterialSlider(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        SliderThemeData sliderThemeLocal = SliderTheme.of(context);
        bool year2023Local = (widget.year2023 ?? sliderThemeLocal.year2023) ?? true;
        SliderThemeData defaults = year2023Local ? new _SliderDefaultsM3Year2023__slider(context) : new _SliderDefaultsM3__slider(context);
        ShowValueIndicator defaultShowValueIndicator = ShowValueIndicator.onlyForDiscrete;
        SliderInteraction defaultAllowedInteraction = SliderInteraction.tapAndSlide;
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection31364 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!_enabled) { __collection31364.Add(WidgetState.disabled); } if (_hovering) { __collection31364.Add(WidgetState.hovered); } if (_focused) { __collection31364.Add(WidgetState.focused); } if (_dragging) { __collection31364.Add(WidgetState.dragged); } return __collection31364; }))();
        SliderComponentShape valueIndicatorShapeLocal = sliderThemeLocal.valueIndicatorShape ?? defaults.valueIndicatorShape!;
        global::Doroti.Ui.Color valueIndicatorColorLocal = default!;
        if (valueIndicatorShapeLocal is RectangularSliderValueIndicatorShape)
        {
            RectangularSliderValueIndicatorShape valueIndicatorShape__31866__as32007 = (RectangularSliderValueIndicatorShape)valueIndicatorShapeLocal;
            valueIndicatorColorLocal = sliderThemeLocal.valueIndicatorColor ?? Dart_uiLibrary.Color.alphaBlend(theme.colorScheme.onSurface.withOpacity(0.6), theme.colorScheme.surface.withOpacity(0.9));
        }
        else
        {
            valueIndicatorColorLocal = (widget.activeColor ?? sliderThemeLocal.valueIndicatorColor) ?? defaults.valueIndicatorColor!;
        }
        Color? effectiveOverlayColor()
        {
            return ((widget.overlayColor?.resolve(states) ?? widget.activeColor?.withOpacity(0.12)) ?? WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(sliderThemeLocal.overlayColor, states)) ?? WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(defaults.overlayColor, states);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.TextStyle valueIndicatorTextStyleLocal = sliderThemeLocal.valueIndicatorTextStyle ?? defaults.valueIndicatorTextStyle!;
        if (MediaQuery.boldTextOf(context))
        {
            valueIndicatorTextStyleLocal = valueIndicatorTextStyleLocal.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold));
        }
        sliderThemeLocal = sliderThemeLocal.copyWith(trackHeight: sliderThemeLocal.trackHeight ?? defaults.trackHeight, activeTrackColor: (widget.activeColor ?? sliderThemeLocal.activeTrackColor) ?? defaults.activeTrackColor, inactiveTrackColor: (widget.inactiveColor ?? sliderThemeLocal.inactiveTrackColor) ?? defaults.inactiveTrackColor, secondaryActiveTrackColor: (widget.secondaryActiveColor ?? sliderThemeLocal.secondaryActiveTrackColor) ?? defaults.secondaryActiveTrackColor, disabledActiveTrackColor: sliderThemeLocal.disabledActiveTrackColor ?? defaults.disabledActiveTrackColor, disabledInactiveTrackColor: sliderThemeLocal.disabledInactiveTrackColor ?? defaults.disabledInactiveTrackColor, disabledSecondaryActiveTrackColor: sliderThemeLocal.disabledSecondaryActiveTrackColor ?? defaults.disabledSecondaryActiveTrackColor, activeTickMarkColor: (widget.inactiveColor ?? sliderThemeLocal.activeTickMarkColor) ?? defaults.activeTickMarkColor, inactiveTickMarkColor: (widget.activeColor ?? sliderThemeLocal.inactiveTickMarkColor) ?? defaults.inactiveTickMarkColor, disabledActiveTickMarkColor: sliderThemeLocal.disabledActiveTickMarkColor ?? defaults.disabledActiveTickMarkColor, disabledInactiveTickMarkColor: sliderThemeLocal.disabledInactiveTickMarkColor ?? defaults.disabledInactiveTickMarkColor, thumbColor: ((widget.thumbColor ?? widget.activeColor) ?? sliderThemeLocal.thumbColor) ?? defaults.thumbColor, disabledThumbColor: sliderThemeLocal.disabledThumbColor ?? defaults.disabledThumbColor, overlayColor: effectiveOverlayColor(), valueIndicatorColor: valueIndicatorColorLocal, trackShape: sliderThemeLocal.trackShape ?? defaults.trackShape, tickMarkShape: sliderThemeLocal.tickMarkShape ?? defaults.tickMarkShape, thumbShape: sliderThemeLocal.thumbShape ?? defaults.thumbShape, overlayShape: sliderThemeLocal.overlayShape ?? defaults.overlayShape, valueIndicatorShape: valueIndicatorShapeLocal, showValueIndicator: (widget.showValueIndicator ?? sliderThemeLocal.showValueIndicator) ?? defaultShowValueIndicator, valueIndicatorTextStyle: valueIndicatorTextStyleLocal, padding: widget.padding ?? sliderThemeLocal.padding, thumbSize: sliderThemeLocal.thumbSize ?? defaults.thumbSize, trackGap: sliderThemeLocal.trackGap ?? defaults.trackGap);
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = (WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(widget.mouseCursor, states) ?? (sliderThemeLocal.mouseCursor?.resolve(states))) ?? WidgetStateMouseCursor.clickable.resolve(states);
        SliderInteraction effectiveAllowedInteraction = (widget.allowedInteraction ?? sliderThemeLocal.allowedInteraction) ?? defaultAllowedInteraction;
        Size screenSize()
        {
            return MediaQuery.sizeOf(context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::System.Action? handleDidGainAccessibilityFocus = default!;
        switch (theme.platform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
                {
                    break;
                }
            case TargetPlatform.windows:
                {
                    handleDidGainAccessibilityFocus = () =>
                    {
                        if (!focusNode.hasFocus && focusNode.canRequestFocus)
                        {
                            focusNode.requestFocus();
                        }
                    };
                    break;
                }
        }
        DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> shortcutMap = MediaQuery.navigationModeOf(context) switch { NavigationMode.directional => _directionalNavShortcutMap, NavigationMode.traditional => _traditionalNavShortcutMap,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double fontSizeLocal = sliderThemeLocal.valueIndicatorTextStyle?.fontSize ?? Text_painterLibrary.kDefaultFontSize;
        double fontSizeToScale = (fontSizeLocal == 0.0) ? Text_painterLibrary.kDefaultFontSize : fontSizeLocal;
        global::Doroti.Framework.Painting.TextScaler textScaler = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 1.3);
        double effectiveTextScale = textScaler.scale(fontSizeToScale) / fontSizeToScale;
        global::Doroti.Framework.Widgets.Widget result = new global::Doroti.Framework.Widgets.CompositedTransformTarget(link: _layerLink, child: new _SliderRenderObjectWidget__slider(key: _renderObjectKey, value: _convert(widget.value), secondaryTrackValue: (widget.secondaryTrackValue is not null) ? _convert(DartRuntimePrimitives.RequireValue(widget.secondaryTrackValue)) : null, divisions: widget.divisions, label: widget.label, sliderTheme: sliderThemeLocal, textScaleFactor: effectiveTextScale, screenSize: screenSize(), onChanged: (widget.onChanged is not null && widget.max > widget.min) ? _handleChanged : null, onChangeStart: _handleDragStart, onChangeEnd: _handleDragEnd, state: this, semanticFormatterCallback: widget.semanticFormatterCallback, onDidGainAccessibilityFocus: () => handleDidGainAccessibilityFocus(), hasFocus: _focused, hovering: _hovering, allowedInteraction: effectiveAllowedInteraction));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? paddingLocal = widget.padding ?? sliderThemeLocal.padding;
        if (paddingLocal is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: result));
        }
        result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.OverlayPortal(controller: _valueIndicatorOverlayPortalController, overlayChildBuilder: (context) =>
        {
            return _buildValueIndicator(DartRuntimePrimitives.RequireValue(sliderThemeLocal.showValueIndicator));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: result));
        return new global::Doroti.Framework.Widgets.FocusableActionDetector(actions: _actionMap, shortcuts: shortcutMap, focusNode: focusNode, autofocus: widget.autofocus, enabled: _enabled, onShowFocusHighlight: _handleFocusHighlightChanged, onShowHoverHighlight: _handleHoverChanged, mouseCursor: effectiveMouseCursor, includeFocusSemantics: false, child: result);
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCupertinoSlider(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new CupertinoSlider(value: widget.value, onChanged: widget.onChanged, onChangeStart: widget.onChangeStart, onChangeEnd: widget.onChangeEnd, min: widget.min, max: widget.max, divisions: widget.divisions, activeColor: widget.activeColor, thumbColor: widget.thumbColor ?? CupertinoColors.white));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildValueIndicator(ShowValueIndicator showValueIndicator)
    {
        global::Doroti.Framework.Widgets.Widget valueIndicator = new global::Doroti.Framework.Widgets.CompositedTransformFollower(link: _layerLink, child: new _ValueIndicatorRenderObjectWidget__slider(state: this));
        return showValueIndicator switch { var __constant40364 when Equals(__constant40364, ShowValueIndicator.never) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(SizedBox.CreateShrink()),var __constant40423 when Equals(__constant40423, ShowValueIndicator.onlyForDiscrete) => (widget.divisions is not null) ? valueIndicator : SizedBox.CreateShrink(),var __constant40544 when Equals(__constant40544, ShowValueIndicator.onlyForContinuous) => (widget.divisions is null) ? valueIndicator : SizedBox.CreateShrink(),var __logical40667 when Equals(__logical40667, ShowValueIndicator.alwaysVisible) || Equals(__logical40667, ShowValueIndicator.always) => valueIndicator,var __constant40744 when Equals(__constant40744, ShowValueIndicator.onDrag) => valueIndicator,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _SliderRenderObjectWidget__slider : global::Doroti.Framework.Widgets.LeafRenderObjectWidget
{
    public virtual double value { get; private set; } = default!;
    public virtual double? secondaryTrackValue { get; private set; }
    public virtual long? divisions { get; private set; }
    public virtual string? label { get; private set; }
    public virtual SliderThemeData sliderTheme { get; private set; } = default!;
    public virtual double textScaleFactor { get; private set; } = default!;
    public virtual Size screenSize { get; private set; } = default!;
    public virtual global::System.Action<double>? onChanged { get; private set; }
    public virtual global::System.Action<double>? onChangeStart { get; private set; }
    public virtual global::System.Action<double>? onChangeEnd { get; private set; }
    public virtual SemanticFormatterCallback? semanticFormatterCallback { get; private set; }
    public virtual global::System.Action? onDidGainAccessibilityFocus { get; private set; }
    public virtual _SliderState__slider state { get; private set; } = default!;
    public virtual bool hasFocus { get; private set; } = default!;
    public virtual bool hovering { get; private set; } = default!;
    public virtual SliderInteraction allowedInteraction { get; private set; } = default!;

    internal _SliderRenderObjectWidget__slider(global::Doroti.Framework.Foundation.Key? key = null, double value = default!, double? secondaryTrackValue = default!, long? divisions = default!, string? label = default!, SliderThemeData sliderTheme = default!, double textScaleFactor = default!, Size screenSize = default!, global::System.Action<double>? onChanged = default!, global::System.Action<double>? onChangeStart = default!, global::System.Action<double>? onChangeEnd = default!, _SliderState__slider state = default!, SemanticFormatterCallback? semanticFormatterCallback = default!, global::System.Action? onDidGainAccessibilityFocus = default!, bool hasFocus = default!, bool hovering = default!, SliderInteraction allowedInteraction = default!) : base(key: key)
    {
        this.value = value;
        this.secondaryTrackValue = secondaryTrackValue;
        this.divisions = divisions;
        this.label = label;
        this.sliderTheme = sliderTheme;
        this.textScaleFactor = textScaleFactor;
        this.screenSize = screenSize;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.state = state;
        this.semanticFormatterCallback = semanticFormatterCallback;
        this.onDidGainAccessibilityFocus = onDidGainAccessibilityFocus;
        this.hasFocus = hasFocus;
        this.hovering = hovering;
        this.allowedInteraction = allowedInteraction;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderSlider__slider(value: value, secondaryTrackValue: secondaryTrackValue, divisions: divisions, label: label, sliderTheme: sliderTheme, textScaleFactor: textScaleFactor, screenSize: screenSize, onChanged: onChanged, onChangeStart: onChangeStart, onChangeEnd: onChangeEnd, state: state, textDirection: Directionality.of(context), semanticFormatterCallback: semanticFormatterCallback, onDidGainAccessibilityFocus: onDidGainAccessibilityFocus, platform: Theme.of(context).platform, hasFocus: hasFocus, hovering: hovering, gestureSettings: MediaQuery.gestureSettingsOf(context), allowedInteraction: allowedInteraction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSlider__slider)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSlider__slider>)(() =>
{
    var __cascade = __renderObject;
    __cascade.divisions = divisions;
    __cascade.value = value;
    __cascade.secondaryTrackValue = secondaryTrackValue;
    __cascade.label = label;
    __cascade.sliderTheme = sliderTheme;
    __cascade.textScaleFactor = textScaleFactor;
    __cascade.screenSize = screenSize;
    __cascade.onChanged = onChanged;
    __cascade.onChangeStart = onChangeStart;
    __cascade.onChangeEnd = onChangeEnd;
    __cascade.textDirection = Directionality.of(context);
    __cascade.semanticFormatterCallback = semanticFormatterCallback;
    __cascade.onDidGainAccessibilityFocus = onDidGainAccessibilityFocus;
    __cascade.platform = Theme.of(context).platform;
    __cascade.hasFocus = hasFocus;
    __cascade.hovering = hovering;
    __cascade.gestureSettings = MediaQuery.gestureSettingsOf(context);
    __cascade.allowedInteraction = allowedInteraction;
    return __cascade;
}))());
    }

}

public class _RenderSlider__slider : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RelayoutWhenSystemFontsChangeMixin
{
    internal static Duration _positionAnimationDuration = Duration.Create(milliseconds: 75L);
    internal static Duration _minimumInteractionTime = Duration.Create(milliseconds: 500L);
    internal const double _minPreferredTrackWidth = 144.0;
    internal virtual _SliderState__slider _state { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _overlayAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _enableAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.TextPainter _labelPainter { get; private set; } = new global::Doroti.Framework.Painting.TextPainter();
    internal virtual global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer _drag { get; set; } = default!;
    internal virtual global::Doroti.Framework.Gestures.TapGestureRecognizer _tap { get; set; } = default!;
    internal virtual bool _active { get; set; } = false;
    public virtual global::System.Action? onDidGainAccessibilityFocus { get; set; } = default;
    internal virtual double _currentDragValue { get; set; } = 0.0;
    public virtual Rect? overlayRect { get; set; } = default;
    internal virtual double _value { get; set; } = default!;
    internal virtual double? _secondaryTrackValue { get; set; } = default;
    internal virtual global::Doroti.Framework.Foundation.TargetPlatform _platform { get; set; } = default!;
    internal virtual SemanticFormatterCallback? _semanticFormatterCallback { get; set; } = default;
    internal virtual long? _divisions { get; set; } = default;
    internal virtual string? _label { get; set; } = default;
    internal virtual SliderThemeData _sliderTheme { get; set; } = default!;
    internal virtual double _textScaleFactor { get; set; } = default!;
    internal virtual Size _screenSize { get; set; } = default!;
    internal virtual global::System.Action<double>? _onChanged { get; set; } = default;
    public virtual global::System.Action<double>? onChangeStart { get; set; } = default;
    public virtual global::System.Action<double>? onChangeEnd { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual bool _hasFocus { get; set; } = default!;
    internal virtual bool _hovering { get; set; } = default!;
    internal virtual bool _hoveringThumb { get; set; } = false;
    internal virtual SliderInteraction _allowedInteraction { get; set; } = default!;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderSlider__slider(double value, double? secondaryTrackValue, long? divisions, string? label, SliderThemeData sliderTheme, double textScaleFactor, Size screenSize, global::Doroti.Framework.Foundation.TargetPlatform platform, global::System.Action<double>? onChanged, SemanticFormatterCallback? semanticFormatterCallback, global::System.Action? onDidGainAccessibilityFocus, global::System.Action<double>? onChangeStart, global::System.Action<double>? onChangeEnd, _SliderState__slider state, TextDirection textDirection, bool hasFocus, bool hovering, global::Doroti.Framework.Gestures.DeviceGestureSettings gestureSettings, SliderInteraction allowedInteraction)
    {
        this.onDidGainAccessibilityFocus = onDidGainAccessibilityFocus;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        _platform = platform;
        _semanticFormatterCallback = semanticFormatterCallback;
        _label = label;
        _value = DartRuntimePrimitives.RequireValue(value);
        _secondaryTrackValue = secondaryTrackValue;
        _divisions = divisions;
        _sliderTheme = sliderTheme;
        _textScaleFactor = textScaleFactor;
        _screenSize = screenSize;
        _onChanged = onChanged;
        _state = state;
        _textDirection = textDirection;
        _hasFocus = hasFocus;
        _hovering = hovering;
        _allowedInteraction = allowedInteraction;
        System.Diagnostics.Debug.Assert((value >= 0.0) && (value <= 1.0));
        System.Diagnostics.Debug.Assert((secondaryTrackValue is null) || (secondaryTrackValue >= 0.0) && (secondaryTrackValue <= 1.0));
        _updateLabelPainter();
        var teamLocal = new global::Doroti.Framework.Gestures.GestureArenaTeam();
        _drag = ((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer();
    __cascade.team = teamLocal;
    __cascade.onStart = _handleDragStart;
    __cascade.onUpdate = _handleDragUpdate;
    __cascade.onEnd = _handleDragEnd;
    __cascade.onCancel = _endInteraction;
    __cascade.gestureSettings = gestureSettings;
    return __cascade;
}))();
        _tap = ((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.TapGestureRecognizer();
    __cascade.team = teamLocal;
    __cascade.onTapDown = _handleTapDown;
    __cascade.onTapUp = _handleTapUp;
    __cascade.gestureSettings = gestureSettings;
    return __cascade;
}))();
        _overlayAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _state.overlayController, curve: Curves.fastOutSlowIn);
        _valueIndicatorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _state.valueIndicatorController, curve: Curves.fastOutSlowIn);
        _enableAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _state.enableController, curve: Curves.easeInOut);
    }

    internal virtual double _maxSliderPartWidth => _sliderPartSizes.map<Size, double>((size) => size.width).reduce(Dart_mathLibrary.max);
    internal virtual double _maxSliderPartHeight => _sliderPartSizes.map<Size, double>((size) => size.height).reduce(Dart_mathLibrary.max);
    internal virtual double _thumbSizeHeight => _sliderTheme.thumbShape!.getPreferredSize(isInteractive, isDiscrete).height;
    internal virtual double _overlayHeight => _sliderTheme.overlayShape!.getPreferredSize(isInteractive, isDiscrete).height;
    internal virtual List<global::Doroti.Ui.Size> _sliderPartSizes => new List<global::Doroti.Ui.Size> { new global::Doroti.Ui.Size(_sliderTheme.overlayShape!.getPreferredSize(isInteractive, isDiscrete).width, (_sliderTheme.padding is not null) ? _thumbSizeHeight : _overlayHeight), _sliderTheme.thumbShape!.getPreferredSize(isInteractive, isDiscrete), _sliderTheme.tickMarkShape!.getPreferredSize(isEnabled: isInteractive, sliderTheme: sliderTheme) }.Cast<global::Doroti.Ui.Size>().ToList();
    internal virtual double _minPreferredTrackHeight => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(_sliderTheme.trackHeight));
    internal virtual global::Doroti.Ui.Rect _trackRect => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>(_sliderTheme.trackShape!.getPreferredRect(parentBox: this, sliderTheme: _sliderTheme, isDiscrete: false));
    public virtual bool isInteractive => DartRuntimePrimitives.ConvertValue<bool>(onChanged is not null);
    public virtual bool isDiscrete => DartRuntimePrimitives.ConvertValue<bool>((divisions is not null) && (DartRuntimePrimitives.RequireValue(divisions) > 0L));
    public virtual double value
    {
        get => _value;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => (newValue >= 0.0) && (newValue <= 1.0));
            double convertedValue = isDiscrete ? _discretize(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newValue))) : DartRuntimePrimitives.RequireValue(newValue);
            if (convertedValue == _value)
            {
                return;
            }
            _value = convertedValue;
            if (isDiscrete)
            {
                double distance = (_value - _state.positionController.value).abs();
                _state.positionController.duration = (distance != 0.0) ? (_positionAnimationDuration * (1.0 / distance)) : Duration.zero;
                _state.positionController.animateTo(convertedValue, curve: Curves.easeInOut);
            }
            else
            {
                _state.positionController.value = convertedValue;
            }
            markNeedsSemanticsUpdate();
        }
    }
    public virtual double? secondaryTrackValue
    {
        get => _secondaryTrackValue;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => (newValue is null) || (newValue >= 0.0) && (newValue <= 1.0));
            if (newValue == _secondaryTrackValue)
            {
                return;
            }
            _secondaryTrackValue = newValue;
            markNeedsPaint();
            markNeedsSemanticsUpdate();
        }
    }
    public virtual global::Doroti.Framework.Gestures.DeviceGestureSettings? gestureSettings
    {
        get => _drag.gestureSettings;
        set
        {
            var gestureSettings = value;
            _drag.gestureSettings = gestureSettings;
            _tap.gestureSettings = gestureSettings;
        }
    }
    public virtual global::Doroti.Framework.Foundation.TargetPlatform platform
    {
        get => _platform;
        set
        {
            var __value = value;
            if (Equals(_platform, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _platform = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual SemanticFormatterCallback? semanticFormatterCallback
    {
        get => _semanticFormatterCallback;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(_semanticFormatterCallback, __value))
            {
                return;
            }
            _semanticFormatterCallback = __value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? divisions
    {
        get => _divisions;
        set
        {
            var __value = value;
            if (__value == _divisions)
            {
                return;
            }
            _divisions = __value;
            markNeedsPaint();
        }
    }
    public virtual string? label
    {
        get => _label;
        set
        {
            var __value = value;
            if (__value == _label)
            {
                return;
            }
            _label = __value;
            _updateLabelPainter();
        }
    }
    public virtual SliderThemeData sliderTheme
    {
        get => _sliderTheme;
        set
        {
            var __value = value;
            if (Equals(__value, _sliderTheme))
            {
                return;
            }
            _sliderTheme = __value;
            _updateLabelPainter();
        }
    }
    public virtual double textScaleFactor
    {
        get => _textScaleFactor;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _textScaleFactor)
            {
                return;
            }
            _textScaleFactor = DartRuntimePrimitives.RequireValue(__value);
            _updateLabelPainter();
        }
    }
    public virtual global::Doroti.Ui.Size screenSize
    {
        get => _screenSize;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _screenSize))
            {
                return;
            }
            _screenSize = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::System.Action<double>? onChanged
    {
        get => _onChanged;
        set
        {
            var __value = value;
            if (Equals(__value, _onChanged))
            {
                return;
            }
            bool wasInteractive = isInteractive;
            _onChanged = __value;
            if (wasInteractive != isInteractive)
            {
                if (isInteractive)
                {
                    _state.enableController.forward();
                }
                else
                {
                    _state.enableController.reverse();
                }
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _textDirection))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            _updateLabelPainter();
        }
    }
    public virtual bool hasFocus
    {
        get => _hasFocus;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _hasFocus)
            {
                return;
            }
            _hasFocus = DartRuntimePrimitives.RequireValue(__value);
            _updateForFocus(_hasFocus);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool hovering
    {
        get => _hovering;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _hovering)
            {
                return;
            }
            _hovering = DartRuntimePrimitives.RequireValue(__value);
            _updateForHover(_hovering);
        }
    }
    public virtual bool hoveringThumb
    {
        get => _hoveringThumb;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _hoveringThumb)
            {
                return;
            }
            _hoveringThumb = DartRuntimePrimitives.RequireValue(__value);
            _updateForHover(_hovering);
        }
    }
    public virtual SliderInteraction allowedInteraction
    {
        get => _allowedInteraction;
        set
        {
            var __value = value;
            if (Equals(DartRuntimePrimitives.RequireValue(__value), _allowedInteraction))
            {
                return;
            }
            _allowedInteraction = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual void _updateForFocus(bool focused)
    {
        if (focused)
        {
            _state.overlayController.forward();
            if (shouldShowValueIndicatorWhenDragged)
            {
                _state.valueIndicatorController.forward();
            }
        }
        else
        {
            _state.overlayController.reverse();
            if (shouldShowValueIndicatorWhenDragged)
            {
                _state.valueIndicatorController.reverse();
            }
        }
    }

    internal virtual void _updateForHover(bool hovered)
    {
        if (hovered && hoveringThumb)
        {
            _state.overlayController.forward();
        }
        else
        {
            if (!_active && !hasFocus)
            {
                _state.overlayController.reverse();
            }
        }
    }

    public virtual bool shouldAlwaysShowValueIndicator => DartRuntimePrimitives.ConvertValue<bool>(Equals(_sliderTheme.showValueIndicator, ShowValueIndicator.alwaysVisible));
    public virtual bool shouldShowValueIndicatorWhenDragged => _sliderTheme.showValueIndicator! switch { var __constant54689 when Equals(__constant54689, ShowValueIndicator.onlyForDiscrete) => isDiscrete,var __constant54743 when Equals(__constant54743, ShowValueIndicator.onlyForContinuous) => !isDiscrete,var __constant54800 when Equals(__constant54800, ShowValueIndicator.always) => true,var __constant54829 when Equals(__constant54829, ShowValueIndicator.onDrag) => true,var __constant54868 when Equals(__constant54868, ShowValueIndicator.never) => false,var __constant54896 when Equals(__constant54896, ShowValueIndicator.alwaysVisible) => false,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    internal virtual double _adjustmentUnit
    {
        get
        {
            switch (_platform)
            {
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return 0.1;
                    }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return 0.05;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
    internal virtual void _updateLabelPainter()
    {
        if (label is not null)
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = _labelPainter;
    __cascade.text = new global::Doroti.Framework.Painting.TextSpan(style: _sliderTheme.valueIndicatorTextStyle, text: label);
    __cascade.textDirection = textDirection;
    __cascade.textScaleFactor = textScaleFactor;
    __cascade.layout();
    return __cascade;
}))());
        }
        else
        {
            _labelPainter.text = null;
        }
        markNeedsLayout();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        _labelPainter.markNeedsLayout();
        _updateLabelPainter();
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.addListener(_scheduleSystemFontsUpdate);
        _overlayAnimation.addListener(markNeedsPaint);
        _valueIndicatorAnimation.addListener(markNeedsPaint);
        _enableAnimation.addListener(markNeedsPaint);
        _state.positionController.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _overlayAnimation.removeListener(markNeedsPaint);
        _valueIndicatorAnimation.removeListener(markNeedsPaint);
        _enableAnimation.removeListener(markNeedsPaint);
        _state.positionController.removeListener(markNeedsPaint);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.removeListener(_scheduleSystemFontsUpdate);
        base.detach();
    }

    public override void dispose()
    {
        _drag.dispose();
        _tap.dispose();
        _labelPainter.dispose();
        _enableAnimation.dispose();
        _valueIndicatorAnimation.dispose();
        _overlayAnimation.dispose();
        base.dispose();
    }

    internal virtual double _getValueFromVisualPosition(double visualPosition)
    {
        return textDirection switch { TextDirection.rtl => 1.0 - visualPosition,TextDirection.ltr => visualPosition,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    internal virtual double _getValueFromGlobalPosition(Offset globalPosition)
    {
        double visualPosition = (globalToLocal(globalPosition).dx - _trackRect.left) / _trackRect.width;
        return _getValueFromVisualPosition(visualPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _discretize(double value)
    {
        double result = Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0);
        if (isDiscrete)
        {
            result = (result * DartRuntimePrimitives.RequireValue(divisions)).round() / (double)DartRuntimePrimitives.RequireValue(divisions);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _startInteraction(Offset globalPosition)
    {
        if (!_state.mounted)
        {
            return;
        }
        if (!_active && isInteractive)
        {
            switch (allowedInteraction)
            {
                case SliderInteraction.tapAndSlide:
                case SliderInteraction.tapOnly:
                    {
                        _active = true;
                        _currentDragValue = _getValueFromGlobalPosition(globalPosition);
                        break;
                    }
                case SliderInteraction.slideThumb:
                    {
                        if (_isPointerOnOverlay(globalPosition))
                        {
                            _active = true;
                            _currentDragValue = value;
                        }
                        break;
                    }
                case SliderInteraction.slideOnly:
                    {
                        _active = true;
                        _currentDragValue = value;
                        break;
                    }
            }
            if (_active)
            {
                onChangeStart?.Invoke(_discretize(DartRuntimePrimitives.RequireValue(value)));
                onChanged!(_discretize(_currentDragValue));
                _state.overlayController.forward();
                if (shouldShowValueIndicatorWhenDragged)
                {
                    _state.valueIndicatorController.forward();
                    _state.interactionTimer?.cancel();
                    _state.interactionTimer = new Timer(_minimumInteractionTime * Scheduler.BindingLibrary.timeDilation, () =>
                    {
                        _state.interactionTimer = null;
                        if (!_active && _state.valueIndicatorController.isCompleted)
                        {
                            _state.valueIndicatorController.reverse();
                        }
                    });
                }
            }
        }
    }

    internal virtual void _endInteraction()
    {
        if (!_state.mounted)
        {
            return;
        }
        if (_active && _state.mounted)
        {
            onChangeEnd?.Invoke(_discretize(_currentDragValue));
            _active = false;
            _currentDragValue = 0.0;
            _state.overlayController.reverse();
            if (shouldShowValueIndicatorWhenDragged && (_state.interactionTimer is null))
            {
                _state.valueIndicatorController.reverse();
            }
        }
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        _startInteraction(details.globalPosition);
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!_state.mounted)
        {
            return;
        }
        switch (allowedInteraction)
        {
            case SliderInteraction.tapAndSlide:
            case SliderInteraction.slideOnly:
            case SliderInteraction.slideThumb:
                {
                    if (_active && isInteractive)
                    {
                        double valueDelta = DartRuntimePrimitives.RequireValue(details.primaryDelta) / _trackRect.width;
                        _currentDragValue += textDirection switch { TextDirection.rtl => -valueDelta,TextDirection.ltr => valueDelta,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                        onChanged!(_discretize(_currentDragValue));
                    }
                    break;
                }
            case SliderInteraction.tapOnly:
                {
                    break;
                }
        }
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        _endInteraction();
    }

    internal virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        _startInteraction(details.globalPosition);
    }

    internal virtual void _handleTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
    {
        _endInteraction();
    }

    internal virtual bool _isPointerOnOverlay(Offset globalPosition)
    {
        return DartRuntimePrimitives.RequireValue(overlayRect).contains(globalToLocal(globalPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry)
    {
        if (!_state.mounted)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is global::Doroti.Framework.Gestures.PointerDownEvent) && isInteractive)
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as60897 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            _drag.addPointer(@event__as60897);
            _tap.addPointer(@event__as60897);
        }
        if (isInteractive && (overlayRect is not null))
        {
            hoveringThumb = DartRuntimePrimitives.RequireValue(overlayRect).contains(@event.localPosition);
        }
    }

    public override double computeMinIntrinsicWidth(double height) => DartRuntimePrimitives.ConvertValue<double>(_minPreferredTrackWidth + _maxSliderPartWidth);
    public override double computeMaxIntrinsicWidth(double height) => DartRuntimePrimitives.ConvertValue<double>(_minPreferredTrackWidth + _maxSliderPartWidth);
    public override double computeMinIntrinsicHeight(double width) => Math.Max(_minPreferredTrackHeight, _maxSliderPartHeight);
    public override double computeMaxIntrinsicHeight(double width) => Math.Max(_minPreferredTrackHeight, _maxSliderPartHeight);
    public override bool sizedByParent => true;
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Ui.Size(constraints.hasBoundedWidth ? constraints.maxWidth : (_minPreferredTrackWidth + _maxSliderPartWidth), constraints.hasBoundedHeight ? constraints.maxHeight : Math.Max(_minPreferredTrackHeight, _maxSliderPartHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        double controllerValue = _state.positionController.value;
        var (visualPositionLocal, secondaryVisualPosition) = textDirection switch { TextDirection.rtl when _secondaryTrackValue is null => (1.0 - controllerValue, null), TextDirection.rtl => DartRuntimePrimitives.ConvertValue<(double, double?)>((1.0 - controllerValue, 1.0 - DartRuntimePrimitives.RequireValue(_secondaryTrackValue))), TextDirection.ltr => (controllerValue, _secondaryTrackValue), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        global::Doroti.Ui.Rect trackRectLocal = _sliderTheme.trackShape!.getPreferredRect(parentBox: this, offset: offset, sliderTheme: _sliderTheme, isDiscrete: isDiscrete);
        global::Doroti.Ui.Offset thumbCenterLocal = _calcThumbCenter(trackRect: trackRectLocal, visualPosition: visualPositionLocal);
        if (isInteractive)
        {
            global::Doroti.Ui.Size overlaySize = sliderTheme.overlayShape!.getPreferredSize(isInteractive, false);
            overlayRect = Rect.fromCircle(center: thumbCenterLocal, radius: overlaySize.width / 2.0);
        }
        global::Doroti.Ui.Offset? secondaryOffsetLocal = (global::Doroti.Ui.Offset?)(object?)((secondaryVisualPosition is not null) ? new global::Doroti.Ui.Offset(trackRectLocal.left + (DartRuntimePrimitives.RequireValue(secondaryVisualPosition) * trackRectLocal.width), trackRectLocal.center.dy) : null);
        double? thumbWidth = _sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.width;
        double? thumbHeight = _sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.height;
        double? trackGapLocal = _sliderTheme.trackGap;
        double? pressedThumbWidth = _sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState> { WidgetState.pressed })?.width;
        double delta = default!;
        if (_active && (thumbWidth is not null) && (pressedThumbWidth is not null) && (trackGapLocal is not null))
        {
            double thumbWidth__63660__value64036 = DartRuntimePrimitives.RequireValue(thumbWidth);
            double pressedThumbWidth__63884__value64058 = DartRuntimePrimitives.RequireValue(pressedThumbWidth);
            double trackGap__63832__value64087 = DartRuntimePrimitives.RequireValue(trackGapLocal);
            delta = DartRuntimePrimitives.RequireValue(thumbWidth__63660__value64036) - DartRuntimePrimitives.RequireValue(pressedThumbWidth__63884__value64058);
            if (DartRuntimePrimitives.RequireValue(thumbWidth__63660__value64036) > 0.0)
            {
                thumbWidth = DartRuntimePrimitives.RequireValue(pressedThumbWidth__63884__value64058);
            }
            if (DartRuntimePrimitives.RequireValue(trackGap__63832__value64087) > 0.0)
            {
                trackGapLocal = DartRuntimePrimitives.RequireValue(trackGap__63832__value64087) - (delta / 2L);
            }
        }
        _sliderTheme.trackShape!.paint(context, offset, parentBox: this, sliderTheme: _sliderTheme.copyWith(trackGap: trackGapLocal), enableAnimation: _enableAnimation, textDirection: _textDirection, thumbCenter: thumbCenterLocal, secondaryOffset: secondaryOffsetLocal, isDiscrete: isDiscrete, isEnabled: isInteractive);
        if (!_overlayAnimation.isDismissed)
        {
            _sliderTheme.overlayShape!.paint(context, thumbCenterLocal, activationAnimation: _overlayAnimation, enableAnimation: _enableAnimation, isDiscrete: isDiscrete, labelPainter: _labelPainter, parentBox: this, sliderTheme: _sliderTheme, textDirection: _textDirection, value: _value, textScaleFactor: _textScaleFactor, sizeWithOverflow: screenSize.isEmpty ? size : screenSize);
        }
        if (isDiscrete)
        {
            double tickMarkWidth = _sliderTheme.tickMarkShape!.getPreferredSize(isEnabled: isInteractive, sliderTheme: _sliderTheme).width;
            double discreteTrackPadding = trackRectLocal.height;
            double adjustedTrackWidth = trackRectLocal.width - discreteTrackPadding;
            if ((adjustedTrackWidth / DartRuntimePrimitives.RequireValue(divisions)) >= (3.0 * tickMarkWidth))
            {
                double dyLocal = trackRectLocal.center.dy;
                for (var i = 0L; i <= DartRuntimePrimitives.RequireValue(divisions); i++)
                {
                    double valueLocal = i / (double)DartRuntimePrimitives.RequireValue(divisions);
                    double dx = trackRectLocal.left + (DartRuntimePrimitives.RequireValue(valueLocal) * adjustedTrackWidth) + (discreteTrackPadding / 2L);
                    var tickMarkOffset = new global::Doroti.Ui.Offset(dx, dyLocal);
                    _sliderTheme.tickMarkShape!.paint(context, tickMarkOffset, parentBox: this, sliderTheme: _sliderTheme, enableAnimation: _enableAnimation, textDirection: _textDirection, thumbCenter: thumbCenterLocal, isEnabled: isInteractive);
                }
            }
        }
        if (isInteractive && (label is not null) && (shouldShowValueIndicatorWhenDragged && !_valueIndicatorAnimation.isDismissed || shouldAlwaysShowValueIndicator))
        {
            _state.paintValueIndicator = (context, offset) =>
            {
                if (attached && (_labelPainter.text is not null))
                {
                    _sliderTheme.valueIndicatorShape?.paint(context, offset + thumbCenterLocal, activationAnimation: shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : _valueIndicatorAnimation, enableAnimation: shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : _enableAnimation, isDiscrete: isDiscrete, labelPainter: _labelPainter, parentBox: this, sliderTheme: _sliderTheme, textDirection: _textDirection, value: _value, textScaleFactor: textScaleFactor, sizeWithOverflow: screenSize.isEmpty ? size : screenSize);
                }
            };
        }
        else
        {
            _state.paintValueIndicator = null;
        }
        _sliderTheme.thumbShape!.paint(context, thumbCenterLocal, activationAnimation: _overlayAnimation, enableAnimation: _enableAnimation, isDiscrete: isDiscrete, labelPainter: _labelPainter, parentBox: this, sliderTheme: ((thumbWidth is not null) && (thumbHeight is not null)) ? _sliderTheme.copyWith(thumbSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbWidth), DartRuntimePrimitives.RequireValue(thumbHeight)))) : _sliderTheme, textDirection: _textDirection, value: _value, textScaleFactor: textScaleFactor, sizeWithOverflow: screenSize.isEmpty ? size : screenSize);
    }

    internal virtual global::Doroti.Ui.Offset _calcThumbCenter(Rect trackRect, double visualPosition)
    {
        double padding = _sliderTheme.trackShape!.isRounded ? trackRect.height : 0.0;
        double thumbPosition = isDiscrete ? (trackRect.left + (visualPosition * (trackRect.width - padding)) + (padding / 2L)) : (trackRect.left + (visualPosition * trackRect.width));
        global::Doroti.Ui.Size thumbPreferredSize = _sliderTheme.thumbShape!.getPreferredSize(isInteractive, isDiscrete);
        double thumbPadding = (padding > (thumbPreferredSize.width / 2L)) ? (padding / 2L) : 0;
        return new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(thumbPosition, trackRect.left + thumbPadding, trackRect.right - thumbPadding), trackRect.center.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _semanticThumbCenter
    {
        get
        {
            double visualPositionLocal = textDirection switch { TextDirection.rtl => 1.0 - _value,TextDirection.ltr => _value,_ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            return _calcThumbCenter(trackRect: _trackRect, visualPosition: visualPositionLocal);
        }
    }
    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        node.rect = Rect.fromCenter(center: _semanticThumbCenter, width: Widgets.ConstantsLibrary.kMinInteractiveDimension, height: Widgets.ConstantsLibrary.kMinInteractiveDimension);
        node.updateWith(config: config);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.isEnabled = isInteractive;
        if (label is not null)
        {
            config.label = label!;
        }
        config.isSlider = true;
        config.isFocusable = isInteractive;
        config.isFocused = hasFocus;
        if (onDidGainAccessibilityFocus is not null)
        {
            config.onDidGainAccessibilityFocus = onDidGainAccessibilityFocus;
        }
        config.textDirection = textDirection;
        if (isInteractive)
        {
            config.onIncrease = increaseAction;
            config.onDecrease = decreaseAction;
            config.onFocus = onFocusAction;
        }
        if (semanticFormatterCallback is not null)
        {
            config.value = semanticFormatterCallback!(_state._lerp(DartRuntimePrimitives.RequireValue(value)));
            config.increasedValue = semanticFormatterCallback!(_state._lerp(Dart_uiLibrary.clampDouble(value + _semanticActionUnit, 0.0, 1.0)));
            config.decreasedValue = semanticFormatterCallback!(_state._lerp(Dart_uiLibrary.clampDouble(value - _semanticActionUnit, 0.0, 1.0)));
        }
        else
        {
            config.value = $"{(value * 100L).round()}%";
            config.increasedValue = $"{(Dart_uiLibrary.clampDouble(value + _semanticActionUnit, 0.0, 1.0) * 100L).round()}%";
            config.decreasedValue = $"{(Dart_uiLibrary.clampDouble(value - _semanticActionUnit, 0.0, 1.0) * 100L).round()}%";
        }
    }

    internal virtual double _semanticActionUnit => (divisions is not null) ? (1.0 / DartRuntimePrimitives.RequireValue(divisions)) : _adjustmentUnit;
    public virtual void onFocusAction()
    {
        if (isInteractive)
        {
            if (!_state.mounted)
            {
                return;
            }
            if (!hasFocus)
            {
                _state.focusNode.requestFocus();
            }
        }
    }

    public virtual void increaseAction()
    {
        if (isInteractive)
        {
            onChangeStart!(currentValue);
            double increase = increaseValue();
            onChanged!(increase);
            onChangeEnd!(increase);
            if (!_state.mounted)
            {
                return;
            }
        }
    }

    public virtual void decreaseAction()
    {
        if (isInteractive)
        {
            onChangeStart!(currentValue);
            double decrease = decreaseValue();
            onChanged!(decrease);
            onChangeEnd!(decrease);
            if (!_state.mounted)
            {
                return;
            }
        }
    }

    public virtual double currentValue
    {
        get
        {
            return Dart_uiLibrary.clampDouble(value, 0.0, 1.0);
        }
    }
    public virtual double increaseValue()
    {
        return Dart_uiLibrary.clampDouble(value + _semanticActionUnit, 0.0, 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double decreaseValue()
    {
        return Dart_uiLibrary.clampDouble(value - _semanticActionUnit, 0.0, 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (_hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        _hasPendingSystemFontsDidChangeCallBack = true;
        Scheduler.SchedulerBinding.instance.scheduleFrameCallback((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => _hasPendingSystemFontsDidChangeCallBack);
            _hasPendingSystemFontsDidChangeCallBack = false;
            DartRuntimePrimitives.Assert(() => attached || (debugDisposed ?? true), () => (object?)$"{this} is detached during {Scheduler.SchedulerBinding.instance.schedulerPhase} but is not disposed.");
            if (attached)
            {
                systemFontsDidChange();
            }
        });
    }

}

internal class _AdjustSliderIntent__slider : global::Doroti.Framework.Widgets.Intent
{
    public virtual _SliderAdjustmentType__slider type { get; private set; } = default!;

    internal _AdjustSliderIntent__slider(_SliderAdjustmentType__slider type)
    {
        this.type = type;
    }

    internal static _AdjustSliderIntent__slider CreateRight()
    {
        var __instance = new _AdjustSliderIntent__slider(type: default!);
        __instance.type = _SliderAdjustmentType__slider.right;
        return __instance;
    }

    internal static _AdjustSliderIntent__slider CreateLeft()
    {
        var __instance = new _AdjustSliderIntent__slider(type: default!);
        __instance.type = _SliderAdjustmentType__slider.left;
        return __instance;
    }

    internal static _AdjustSliderIntent__slider CreateUp()
    {
        var __instance = new _AdjustSliderIntent__slider(type: default!);
        __instance.type = _SliderAdjustmentType__slider.up;
        return __instance;
    }

    internal static _AdjustSliderIntent__slider CreateDown()
    {
        var __instance = new _AdjustSliderIntent__slider(type: default!);
        __instance.type = _SliderAdjustmentType__slider.down;
        return __instance;
    }

}

internal enum _SliderAdjustmentType__slider
{
    right,
    left,
    up,
    down
}

internal class _ValueIndicatorRenderObjectWidget__slider : global::Doroti.Framework.Widgets.LeafRenderObjectWidget
{
    public virtual _SliderState__slider state { get; private set; } = default!;

    internal _ValueIndicatorRenderObjectWidget__slider(_SliderState__slider state)
    {
        this.state = state;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderValueIndicator__slider(state: state);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderValueIndicator__slider)renderObject;
        __renderObject._state = DartRuntimePrimitives.ConvertValue<_SliderState__slider>(state);
    }

}

public class _RenderValueIndicator__slider : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RelayoutWhenSystemFontsChangeMixin
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual _SliderState__slider _state { get; set; } = default!;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderValueIndicator__slider(_SliderState__slider state)
    {
        _state = state;
        _valueIndicatorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _state.valueIndicatorController, curve: Curves.fastOutSlowIn);
    }

    public override bool sizedByParent => true;
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.addListener(_scheduleSystemFontsUpdate);
        _valueIndicatorAnimation.addListener(markNeedsPaint);
        _state.positionController.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _valueIndicatorAnimation.removeListener(markNeedsPaint);
        _state.positionController.removeListener(markNeedsPaint);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.removeListener(_scheduleSystemFontsUpdate);
        base.detach();
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        _state.paintValueIndicator?.Invoke(context, offset);
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return constraints.smallest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _valueIndicatorAnimation.dispose();
        base.dispose();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (_hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        _hasPendingSystemFontsDidChangeCallBack = true;
        Scheduler.SchedulerBinding.instance.scheduleFrameCallback((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => _hasPendingSystemFontsDidChangeCallBack);
            _hasPendingSystemFontsDidChangeCallBack = false;
            DartRuntimePrimitives.Assert(() => attached || (debugDisposed ?? true), () => (object?)$"{this} is detached during {Scheduler.SchedulerBinding.instance.schedulerPhase} but is not disposed.");
            if (attached)
            {
                systemFontsDidChange();
            }
        });
    }

}

internal class _SliderDefaultsM3Year2023__slider : SliderThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _SliderDefaultsM3Year2023__slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 4.0)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.surfaceContainerHighest);
    public override global::Doroti.Ui.Color? secondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary.withOpacity(0.54));
    public override global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.38));
    public override global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.12));
    public override global::Doroti.Ui.Color? disabledSecondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.12));
    public override global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onPrimary.withOpacity(0.38));
    public override global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurfaceVariant.withOpacity(0.38));
    public override global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.38));
    public override global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.38));
    public override global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Dart_uiLibrary.Color.alphaBlend(_colors.onSurface.withOpacity(0.38), _colors.surface));
    public override global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(WidgetStateColor.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.dragged))
        {
            return _colors.primary.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.primary.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.primary.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(context).textTheme.labelMedium!.copyWith(color: _colors.onPrimary);
    public override global::Doroti.Ui.Color? valueIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override SliderComponentShape? valueIndicatorShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new DropSliderValueIndicatorShape());
    public override SliderComponentShape? thumbShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderThumbShape());
    public override SliderTrackShape? trackShape => DartRuntimePrimitives.ConvertValue<SliderTrackShape>(new RoundedRectSliderTrackShape());
    public override SliderComponentShape? overlayShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override SliderTickMarkShape? tickMarkShape => DartRuntimePrimitives.ConvertValue<SliderTickMarkShape>(new RoundSliderTickMarkShape());
}

internal class _SliderDefaultsM3__slider : SliderThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _SliderDefaultsM3__slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 16.0)
    {
        this.context = context;
    }

    public override global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.secondaryContainer);
    public override global::Doroti.Ui.Color? secondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary.withOpacity(0.54));
    public override global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.38));
    public override global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.12));
    public override global::Doroti.Ui.Color? disabledSecondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.38));
    public override global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onPrimary.withOpacity(1.0));
    public override global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSecondaryContainer.withOpacity(1.0));
    public override global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onInverseSurface);
    public override global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface);
    public override global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.primary);
    public override global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.onSurface.withOpacity(0.38));
    public override global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(WidgetStateColor.CreateResolveWith((states) =>
    {
        if (states.Contains(WidgetState.dragged))
        {
            return _colors.primary.withOpacity(0.1);
        }
        if (states.Contains(WidgetState.hovered))
        {
            return _colors.primary.withOpacity(0.08);
        }
        if (states.Contains(WidgetState.focused))
        {
            return _colors.primary.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    }));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(context).textTheme.labelLarge!.copyWith(color: _colors.onInverseSurface);
    public override global::Doroti.Ui.Color? valueIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_colors.inverseSurface);
    public override SliderComponentShape? valueIndicatorShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundedRectSliderValueIndicatorShape());
    public override SliderComponentShape? thumbShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new HandleThumbShape());
    public override SliderTrackShape? trackShape => DartRuntimePrimitives.ConvertValue<SliderTrackShape>(new GappedSliderTrackShape());
    public override SliderComponentShape? overlayShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override SliderTickMarkShape? tickMarkShape => DartRuntimePrimitives.ConvertValue<SliderTickMarkShape>(new RoundSliderTickMarkShape(tickMarkRadius: 4.0 / 2L));
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? thumbSize
    {
        get
        {
            return (global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return new global::Doroti.Ui.Size(4.0, 44.0);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return new global::Doroti.Ui.Size(4.0, 44.0);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return new global::Doroti.Ui.Size(2.0, 44.0);
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return new global::Doroti.Ui.Size(2.0, 44.0);
                }
                return new global::Doroti.Ui.Size(4.0, 44.0);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override double? trackGap => 6.0;
}

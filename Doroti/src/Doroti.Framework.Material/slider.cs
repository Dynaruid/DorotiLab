// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider.dart
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
        this._sliderType = _SliderType__slider.material;
        System.Diagnostics.Debug.Assert((min <= max));
        System.Diagnostics.Debug.Assert(((value >= min) && (value <= max)));
        System.Diagnostics.Debug.Assert(((secondaryTrackValue is null) || (((secondaryTrackValue >= min) && (secondaryTrackValue <= max)))));
        System.Diagnostics.Debug.Assert(((divisions is null) || (DartRuntimePrimitives.RequireValue(divisions) > 0L)));
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
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("value", this.value));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("secondaryTrackValue", this.secondaryTrackValue));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<double>>("onChanged", (global::System.Action<double>?)this.onChanged, ifNull: "disabled"));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<double>>.CreateHas("onChangeStart", this.onChangeStart));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<double>>.CreateHas("onChangeEnd", this.onChangeEnd));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("min", this.min));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("max", this.max));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("divisions", this.divisions));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("label", this.label));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("activeColor", this.activeColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inactiveColor", this.inactiveColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryActiveColor", this.secondaryActiveColor));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<SemanticFormatterCallback>.CreateHas("semanticFormatterCallback", this.semanticFormatterCallback));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<global::Doroti.Framework.Widgets.FocusNode>.CreateHas("focusNode", this.focusNode));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("autofocus", value: this.autofocus, ifTrue: "autofocus"));
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
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _renderObjectKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _traditionalNavShortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)(object?)_AdjustSliderIntent__slider.CreateUp()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)(object?)_AdjustSliderIntent__slider.CreateDown()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Framework.Widgets.Intent)(object?)_AdjustSliderIntent__slider.CreateLeft()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Framework.Widgets.Intent)(object?)_AdjustSliderIntent__slider.CreateRight()) };
    internal static DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> _directionalNavShortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Framework.Widgets.Intent)(object?)_AdjustSliderIntent__slider.CreateLeft()), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Framework.Widgets.Intent)(object?)_AdjustSliderIntent__slider.CreateRight()) };
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

    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((Slider)this.widget).onChanged is not null));
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.FocusNode>((((Slider)this.widget).focusNode ?? this._focusNode!));
    public override void initState()
    {
        base.initState();
        overlayController = new global::Doroti.Framework.Animation.AnimationController(duration: ConstantsLibrary.kRadialReactionDuration, vsync: this);
        valueIndicatorController = new global::Doroti.Framework.Animation.AnimationController(duration: valueIndicatorAnimationDuration, vsync: this);
        enableController = new global::Doroti.Framework.Animation.AnimationController(duration: enableAnimationDuration, vsync: this);
        positionController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.zero, vsync: this);
        this.enableController.value = ((((Slider)this.widget).onChanged is not null) ? 1.0 : 0.0);
        this.positionController.value = _convert(((Slider)this.widget).value);
        _actionMap = new DartMap<Type, dynamic> { [typeof(_AdjustSliderIntent__slider)] = new global::Doroti.Framework.Widgets.CallbackAction<_AdjustSliderIntent__slider>(onInvoke: (__arg0) => { ((global::System.Action<_AdjustSliderIntent__slider>)this._actionHandler)(__arg0); return default!; }) };
        if ((((Slider)this.widget).focusNode is null))
        {
            _focusNode ??= new global::Doroti.Framework.Widgets.FocusNode();
        }
    }

    public override void dispose()
    {
        this.interactionTimer?.cancel();
        this.overlayController.dispose();
        this.valueIndicatorController.dispose();
        this.enableController.dispose();
        this.positionController.dispose();
        this._focusNode?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleChanged(double value)
    {
        DartRuntimePrimitives.Assert(() => (((Slider)this.widget).onChanged is not null));
        double lerpValue__27148 = _lerp(value);
        if ((this._currentChangedValue != lerpValue__27148))
        {
            _currentChangedValue = lerpValue__27148;
            if ((this._currentChangedValue != ((Slider)this.widget).value))
            {
                ((Slider)this.widget).onChanged!(DartRuntimePrimitives.RequireValue(this._currentChangedValue));
            }
        }
    }

    internal virtual void _handleDragStart(double value)
    {
        setState(((global::System.Action)(() =>
        {
            _dragging = true;
        })));
        ((Slider)this.widget).onChangeStart?.Invoke(_lerp(value));
    }

    internal virtual void _handleDragEnd(double value)
    {
        setState(((global::System.Action)(() =>
        {
            _dragging = false;
        })));
        _currentChangedValue = null;
        ((Slider)this.widget).onChangeEnd?.Invoke(_lerp(value));
    }

    internal virtual void _actionHandler(_AdjustSliderIntent__slider intent)
    {
        global::Doroti.Ui.TextDirection directionality__27766 = Directionality.of(((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._renderObjectKey).currentContext!);
        bool shouldIncrease__27851 = (((_AdjustSliderIntent__slider)intent).type switch { _SliderAdjustmentType__slider.up => true, _SliderAdjustmentType__slider.down => false, _SliderAdjustmentType__slider.left => (object.Equals(directionality__27766, TextDirection.rtl)), _SliderAdjustmentType__slider.right => (object.Equals(directionality__27766, TextDirection.ltr)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var slider__28139 = ((_RenderSlider__slider?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)this._renderObjectKey).currentContext!.findRenderObject()!)!;
        if (shouldIncrease__27851) { slider__28139.increaseAction(); } else { slider__28139.decreaseAction(); }
        return;
    }

    internal virtual void _handleFocusHighlightChanged(bool focused)
    {
        if ((focused != this._focused))
        {
            setState(((global::System.Action)(() =>
            {
                _focused = focused;
            })));
        }
    }

    internal virtual void _handleHoverChanged(bool hovering)
    {
        if ((hovering != this._hovering))
        {
            setState(((global::System.Action)(() =>
            {
                _hovering = hovering;
            })));
        }
    }

    internal virtual double _lerp(double value)
    {
        DartRuntimePrimitives.Assert(() => (value >= 0.0));
        DartRuntimePrimitives.Assert(() => (value <= 1.0));
        return ((value * ((((Slider)this.widget).max - ((Slider)this.widget).min))) + ((Slider)this.widget).min);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _discretize(double value)
    {
        DartRuntimePrimitives.Assert(() => (((Slider)this.widget).divisions is not null));
        DartRuntimePrimitives.Assert(() => ((value >= 0.0) && (value <= 1.0)));
        long divisions__29039 = DartRuntimePrimitives.RequireValue(((Slider)this.widget).divisions);
        return (((value * divisions__29039)).round() / divisions__29039);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _convert(double value)
    {
        double ret__29172 = _unlerp(value);
        if ((((Slider)this.widget).divisions is not null))
        {
            ret__29172 = _discretize(ret__29172);
        }
        return ret__29172;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _unlerp(double value)
    {
        DartRuntimePrimitives.Assert(() => (value <= ((Slider)this.widget).max));
        DartRuntimePrimitives.Assert(() => (value >= ((Slider)this.widget).min));
        return ((((Slider)this.widget).max > ((Slider)this.widget).min) ? (((value - ((Slider)this.widget).min)) / ((((Slider)this.widget).max - ((Slider)this.widget).min))) : 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        switch (((Slider)this.widget)._sliderType)
        {
            case _SliderType__slider.material:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildMaterialSlider(context));
                }
            case _SliderType__slider.adaptive:
                {
                    ThemeData theme__29887 = Theme.of(context);
                    switch (theme__29887.platform)
                    {
                        case global::Doroti.Framework.Foundation.TargetPlatform.android:
                        case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                        case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                            {
                                return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildMaterialSlider(context));
                            }
                        case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                        case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                            {
                                return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildCupertinoSlider(context));
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMaterialSlider(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__30400 = Theme.of(context);
        SliderThemeData sliderTheme__30447 = SliderTheme.of(context);
        bool year2023__30501 = ((((Slider)this.widget).year2023 ?? sliderTheme__30447.year2023) ?? true);
        dynamic defaults__30587 = (((object)theme__30400.useMaterial3) switch { true => (year2023__30501 ? new _SliderDefaultsM3Year2023__slider(context) : new _SliderDefaultsM3__slider(context)), false => new _SliderDefaultsM2__slider(context), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        ShowValueIndicator defaultShowValueIndicator__31193 = ShowValueIndicator.onlyForDiscrete;
        SliderInteraction defaultAllowedInteraction__31285 = SliderInteraction.tapAndSlide;
        var states__31355 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection31364 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection31364.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (this._hovering) { __collection31364.Add(global::Doroti.Framework.Widgets.WidgetState.hovered); } if (this._focused) { __collection31364.Add(global::Doroti.Framework.Widgets.WidgetState.focused); } if (this._dragging) { __collection31364.Add(global::Doroti.Framework.Widgets.WidgetState.dragged); } return __collection31364; }))();
        SliderComponentShape valueIndicatorShape__31866 = (sliderTheme__30447.valueIndicatorShape ?? defaults__30587.valueIndicatorShape!);
        global::Doroti.Ui.Color valueIndicatorColor__31978 = default!;
        if ((valueIndicatorShape__31866 is RectangularSliderValueIndicatorShape))
        {
            RectangularSliderValueIndicatorShape valueIndicatorShape__31866__as32007 = (RectangularSliderValueIndicatorShape)valueIndicatorShape__31866;
            valueIndicatorColor__31978 = (sliderTheme__30447.valueIndicatorColor ?? Dart_uiLibrary.Color.alphaBlend(theme__30400.colorScheme.onSurface.withOpacity(0.6), theme__30400.colorScheme.surface.withOpacity(0.9)));
        }
        else
        {
            valueIndicatorColor__31978 = ((((Slider)this.widget).activeColor ?? sliderTheme__30447.valueIndicatorColor) ?? defaults__30587.valueIndicatorColor!);
        }
        Color? effectiveOverlayColor()
        {
            return (((((((Slider)this.widget).overlayColor?.resolve(states__31355) ?? ((Slider)this.widget).activeColor?.withOpacity(0.12)) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(sliderTheme__30447.overlayColor, states__31355))) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(defaults__30587.overlayColor, states__31355)));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Framework.Painting.TextStyle valueIndicatorTextStyle__32773 = (sliderTheme__30447.valueIndicatorTextStyle ?? defaults__30587.valueIndicatorTextStyle!);
        if (MediaQuery.boldTextOf(context))
        {
            valueIndicatorTextStyle__32773 = valueIndicatorTextStyle__32773.merge(new global::Doroti.Framework.Painting.TextStyle(fontWeight: FontWeight.bold));
        }
        sliderTheme__30447 = sliderTheme__30447.copyWith(trackHeight: (sliderTheme__30447.trackHeight ?? defaults__30587.trackHeight), activeTrackColor: ((((Slider)this.widget).activeColor ?? sliderTheme__30447.activeTrackColor) ?? defaults__30587.activeTrackColor), inactiveTrackColor: ((((Slider)this.widget).inactiveColor ?? sliderTheme__30447.inactiveTrackColor) ?? defaults__30587.inactiveTrackColor), secondaryActiveTrackColor: ((((Slider)this.widget).secondaryActiveColor ?? sliderTheme__30447.secondaryActiveTrackColor) ?? defaults__30587.secondaryActiveTrackColor), disabledActiveTrackColor: (sliderTheme__30447.disabledActiveTrackColor ?? defaults__30587.disabledActiveTrackColor), disabledInactiveTrackColor: (sliderTheme__30447.disabledInactiveTrackColor ?? defaults__30587.disabledInactiveTrackColor), disabledSecondaryActiveTrackColor: (sliderTheme__30447.disabledSecondaryActiveTrackColor ?? defaults__30587.disabledSecondaryActiveTrackColor), activeTickMarkColor: ((((Slider)this.widget).inactiveColor ?? sliderTheme__30447.activeTickMarkColor) ?? defaults__30587.activeTickMarkColor), inactiveTickMarkColor: ((((Slider)this.widget).activeColor ?? sliderTheme__30447.inactiveTickMarkColor) ?? defaults__30587.inactiveTickMarkColor), disabledActiveTickMarkColor: (sliderTheme__30447.disabledActiveTickMarkColor ?? defaults__30587.disabledActiveTickMarkColor), disabledInactiveTickMarkColor: (sliderTheme__30447.disabledInactiveTickMarkColor ?? defaults__30587.disabledInactiveTickMarkColor), thumbColor: (((((Slider)this.widget).thumbColor ?? ((Slider)this.widget).activeColor) ?? sliderTheme__30447.thumbColor) ?? defaults__30587.thumbColor), disabledThumbColor: (sliderTheme__30447.disabledThumbColor ?? defaults__30587.disabledThumbColor), overlayColor: effectiveOverlayColor(), valueIndicatorColor: valueIndicatorColor__31978, trackShape: (sliderTheme__30447.trackShape ?? defaults__30587.trackShape), tickMarkShape: (sliderTheme__30447.tickMarkShape ?? defaults__30587.tickMarkShape), thumbShape: (sliderTheme__30447.thumbShape ?? defaults__30587.thumbShape), overlayShape: (sliderTheme__30447.overlayShape ?? defaults__30587.overlayShape), valueIndicatorShape: valueIndicatorShape__31866, showValueIndicator: ((((Slider)this.widget).showValueIndicator ?? sliderTheme__30447.showValueIndicator) ?? defaultShowValueIndicator__31193), valueIndicatorTextStyle: valueIndicatorTextStyle__32773, padding: (((Slider)this.widget).padding ?? sliderTheme__30447.padding), thumbSize: (sliderTheme__30447.thumbSize ?? defaults__30587.thumbSize), trackGap: (sliderTheme__30447.trackGap ?? defaults__30587.trackGap));
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor__35490 = ((((WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor?>(((Slider)this.widget).mouseCursor, states__31355) ?? (global::Doroti.Framework.Services.MouseCursor)sliderTheme__30447.mouseCursor?.resolve(states__31355))) ?? (global::Doroti.Framework.Services.MouseCursor)global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states__31355)));
        SliderInteraction effectiveAllowedInteraction__35734 = ((((Slider)this.widget).allowedInteraction ?? sliderTheme__30447.allowedInteraction) ?? defaultAllowedInteraction__31285);
        Size screenSize()
        {
            return MediaQuery.sizeOf(context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::System.Action? handleDidGainAccessibilityFocus__36116 = default!;
        switch (theme__30400.platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    handleDidGainAccessibilityFocus__36116 = (global::System.Action)(() =>
                    {
                        if ((!((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus && ((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).canRequestFocus))
                        {
                            this.focusNode.requestFocus();
                        }
                    });
                    break;
                }
        }
        DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> shortcutMap__36694 = (MediaQuery.navigationModeOf(context) switch { global::Doroti.Framework.Widgets.NavigationMode.directional => _directionalNavShortcutMap, global::Doroti.Framework.Widgets.NavigationMode.traditional => _traditionalNavShortcutMap, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double fontSize__36922 = (sliderTheme__30447.valueIndicatorTextStyle?.fontSize ?? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double fontSizeToScale__37017 = ((fontSize__36922 == 0.0) ? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize : fontSize__36922);
        global::Doroti.Framework.Painting.TextScaler textScaler__37103 = (theme__30400.useMaterial3 ? MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 1.3) : MediaQuery.textScalerOf(context));
        double effectiveTextScale__37510 = (textScaler__37103.scale(fontSizeToScale__37017) / fontSizeToScale__37017);
        global::Doroti.Framework.Widgets.Widget result__37596 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CompositedTransformTarget(link: this._layerLink, child: new _SliderRenderObjectWidget__slider(key: this._renderObjectKey, value: _convert(((Slider)this.widget).value), secondaryTrackValue: (((((Slider)this.widget).secondaryTrackValue is not null)) ? _convert(DartRuntimePrimitives.RequireValue(((Slider)this.widget).secondaryTrackValue)) : null), divisions: ((Slider)this.widget).divisions, label: ((Slider)this.widget).label, sliderTheme: sliderTheme__30447, textScaleFactor: effectiveTextScale__37510, screenSize: screenSize(), onChanged: ((global::System.Action<double>)((((((Slider)this.widget).onChanged is not null)) && ((((Slider)this.widget).max > ((Slider)this.widget).min))) ? this._handleChanged : null)), onChangeStart: (global::System.Action<double>)this._handleDragStart, onChangeEnd: (global::System.Action<double>)this._handleDragEnd, state: this, semanticFormatterCallback: (SemanticFormatterCallback?)((Slider)this.widget).semanticFormatterCallback, onDidGainAccessibilityFocus: () => handleDidGainAccessibilityFocus__36116(), hasFocus: this._focused, hovering: this._hovering, allowedInteraction: effectiveAllowedInteraction__35734)));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding__38582 = (((Slider)this.widget).padding ?? sliderTheme__30447.padding);
        if ((padding__38582 is not null))
        {
            result__37596 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: padding__38582, child: result__37596));
        }
        result__37596 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.OverlayPortal(controller: this._valueIndicatorOverlayPortalController, overlayChildBuilder: ((context) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildValueIndicator(DartRuntimePrimitives.RequireValue(sliderTheme__30447.showValueIndicator)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), child: result__37596));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.FocusableActionDetector(actions: this._actionMap, shortcuts: shortcutMap__36694, focusNode: this.focusNode, autofocus: ((Slider)this.widget).autofocus, enabled: this._enabled, onShowFocusHighlight: (global::System.Action<bool>)this._handleFocusHighlightChanged, onShowHoverHighlight: (global::System.Action<bool>)this._handleHoverChanged, mouseCursor: effectiveMouseCursor__35490, includeFocusSemantics: false, child: result__37596));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCupertinoSlider(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: double.PositiveInfinity, child: new CupertinoSlider(value: ((Slider)this.widget).value, onChanged: ((Slider)this.widget).onChanged, onChangeStart: ((Slider)this.widget).onChangeStart, onChangeEnd: ((Slider)this.widget).onChangeEnd, min: ((Slider)this.widget).min, max: ((Slider)this.widget).max, divisions: ((Slider)this.widget).divisions, activeColor: ((Slider)this.widget).activeColor, thumbColor: (((Slider)this.widget).thumbColor ?? CupertinoColors.white))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildValueIndicator(ShowValueIndicator showValueIndicator)
    {
        global::Doroti.Framework.Widgets.Widget valueIndicator__40179 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CompositedTransformFollower(link: this._layerLink, child: new _ValueIndicatorRenderObjectWidget__slider(state: this)));
        return (showValueIndicator switch { var __constant40364 when (object.Equals(__constant40364, ShowValueIndicator.never)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __constant40423 when (object.Equals(__constant40423, ShowValueIndicator.onlyForDiscrete)) => ((((Slider)this.widget).divisions is not null) ? valueIndicator__40179 : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __constant40544 when (object.Equals(__constant40544, ShowValueIndicator.onlyForContinuous)) => ((((Slider)this.widget).divisions is null) ? valueIndicator__40179 : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __logical40667 when ((object.Equals(__logical40667, ShowValueIndicator.alwaysVisible) || object.Equals(__logical40667, ShowValueIndicator.always))) => valueIndicator__40179, var __constant40744 when (object.Equals(__constant40744, ShowValueIndicator.onDrag)) => valueIndicator__40179, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSlider__slider(value: this.value, secondaryTrackValue: this.secondaryTrackValue, divisions: this.divisions, label: this.label, sliderTheme: this.sliderTheme, textScaleFactor: this.textScaleFactor, screenSize: this.screenSize, onChanged: (global::System.Action<double>?)this.onChanged, onChangeStart: (global::System.Action<double>?)this.onChangeStart, onChangeEnd: (global::System.Action<double>?)this.onChangeEnd, state: this.state, textDirection: Directionality.of(context), semanticFormatterCallback: (SemanticFormatterCallback?)this.semanticFormatterCallback, onDidGainAccessibilityFocus: () => this.onDidGainAccessibilityFocus(), platform: Theme.of(context).platform, hasFocus: this.hasFocus, hovering: this.hovering, gestureSettings: MediaQuery.gestureSettingsOf(context), allowedInteraction: this.allowedInteraction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSlider__slider)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSlider__slider>)(() =>
{
    var __cascade = __renderObject;
    __cascade.divisions = this.divisions;
    __cascade.value = this.value;
    __cascade.secondaryTrackValue = this.secondaryTrackValue;
    __cascade.label = this.label;
    __cascade.sliderTheme = this.sliderTheme;
    __cascade.textScaleFactor = this.textScaleFactor;
    __cascade.screenSize = this.screenSize;
    __cascade.onChanged = this.onChanged;
    __cascade.onChangeStart = this.onChangeStart;
    __cascade.onChangeEnd = this.onChangeEnd;
    __cascade.textDirection = Directionality.of(context);
    __cascade.semanticFormatterCallback = this.semanticFormatterCallback;
    __cascade.onDidGainAccessibilityFocus = this.onDidGainAccessibilityFocus;
    __cascade.platform = Theme.of(context).platform;
    __cascade.hasFocus = this.hasFocus;
    __cascade.hovering = this.hovering;
    __cascade.gestureSettings = MediaQuery.gestureSettingsOf(context);
    __cascade.allowedInteraction = this.allowedInteraction;
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
        this._platform = platform;
        this._semanticFormatterCallback = semanticFormatterCallback;
        this._label = label;
        this._value = DartRuntimePrimitives.RequireValue(value);
        this._secondaryTrackValue = secondaryTrackValue;
        this._divisions = divisions;
        this._sliderTheme = sliderTheme;
        this._textScaleFactor = textScaleFactor;
        this._screenSize = screenSize;
        this._onChanged = onChanged;
        this._state = state;
        this._textDirection = textDirection;
        this._hasFocus = hasFocus;
        this._hovering = hovering;
        this._allowedInteraction = allowedInteraction;
        System.Diagnostics.Debug.Assert(((value >= 0.0) && (value <= 1.0)));
        System.Diagnostics.Debug.Assert(((secondaryTrackValue is null) || (((secondaryTrackValue >= 0.0) && (secondaryTrackValue <= 1.0)))));
        _updateLabelPainter();
        var team__45514 = new global::Doroti.Framework.Gestures.GestureArenaTeam();
        _drag = ((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer();
    __cascade.team = team__45514;
    __cascade.onStart = this._handleDragStart;
    __cascade.onUpdate = this._handleDragUpdate;
    __cascade.onEnd = this._handleDragEnd;
    __cascade.onCancel = this._endInteraction;
    __cascade.gestureSettings = gestureSettings;
    return __cascade;
}))();
        _tap = ((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.TapGestureRecognizer();
    __cascade.team = team__45514;
    __cascade.onTapDown = this._handleTapDown;
    __cascade.onTapUp = this._handleTapUp;
    __cascade.gestureSettings = gestureSettings;
    return __cascade;
}))();
        _overlayAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_SliderState__slider)this._state).overlayController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _valueIndicatorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_SliderState__slider)this._state).valueIndicatorController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _enableAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_SliderState__slider)this._state).enableController, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
    }

    internal virtual double _maxSliderPartWidth => this._sliderPartSizes.map<Size, double>(((size) => size.width)).reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
    internal virtual double _maxSliderPartHeight => this._sliderPartSizes.map<Size, double>(((size) => size.height)).reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
    internal virtual double _thumbSizeHeight => this._sliderTheme.thumbShape!.getPreferredSize(this.isInteractive, this.isDiscrete).height;
    internal virtual double _overlayHeight => this._sliderTheme.overlayShape!.getPreferredSize(this.isInteractive, this.isDiscrete).height;
    internal virtual List<global::Doroti.Ui.Size> _sliderPartSizes => new List<global::Doroti.Ui.Size> { new global::Doroti.Ui.Size(this._sliderTheme.overlayShape!.getPreferredSize(this.isInteractive, this.isDiscrete).width, ((this._sliderTheme.padding is not null) ? this._thumbSizeHeight : this._overlayHeight)), this._sliderTheme.thumbShape!.getPreferredSize(this.isInteractive, this.isDiscrete), this._sliderTheme.tickMarkShape!.getPreferredSize(isEnabled: this.isInteractive, sliderTheme: this.sliderTheme) }.Cast<global::Doroti.Ui.Size>().ToList();
    internal virtual double _minPreferredTrackHeight => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(this._sliderTheme.trackHeight));
    internal virtual global::Doroti.Ui.Rect _trackRect => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>(this._sliderTheme.trackShape!.getPreferredRect(parentBox: this, sliderTheme: this._sliderTheme, isDiscrete: false));
    public virtual bool isInteractive => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
    public virtual bool isDiscrete => DartRuntimePrimitives.ConvertValue<bool>(((this.divisions is not null) && (DartRuntimePrimitives.RequireValue(this.divisions) > 0L)));
    public virtual double value
    {
        get => this._value;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => ((newValue >= 0.0) && (newValue <= 1.0)));
            double convertedValue__48696 = (this.isDiscrete ? _discretize(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newValue))) : DartRuntimePrimitives.RequireValue(newValue));
            if ((convertedValue__48696 == this._value))
            {
                return;
            }
            _value = convertedValue__48696;
            if (this.isDiscrete)
            {
                double distance__49156 = ((this._value - ((_SliderState__slider)this._state).positionController.value)).abs();
                ((_SliderState__slider)this._state).positionController.duration = ((distance__49156 != 0.0) ? (_positionAnimationDuration * ((1.0 / distance__49156))) : Duration.zero);
                ((_SliderState__slider)this._state).positionController.animateTo(convertedValue__48696, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
            }
            else
            {
                ((_SliderState__slider)this._state).positionController.value = convertedValue__48696;
            }
            markNeedsSemanticsUpdate();
        }
    }
    public virtual double? secondaryTrackValue
    {
        get => this._secondaryTrackValue;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => ((newValue is null) || (((newValue >= 0.0) && (newValue <= 1.0)))));
            if ((newValue == this._secondaryTrackValue))
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
        get => this._drag.gestureSettings;
        set
        {
            var gestureSettings = value;
            this._drag.gestureSettings = gestureSettings;
            this._tap.gestureSettings = gestureSettings;
        }
    }
    public virtual global::Doroti.Framework.Foundation.TargetPlatform platform
    {
        get => this._platform;
        set
        {
            var __value = value;
            if ((object.Equals(this._platform, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _platform = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual SemanticFormatterCallback? semanticFormatterCallback
    {
        get => this._semanticFormatterCallback;
        set
        {
            var __value = value is null ? null : (SemanticFormatterCallback)(object)value;
            if ((object.Equals((SemanticFormatterCallback?)this._semanticFormatterCallback, (SemanticFormatterCallback?)__value)))
            {
                return;
            }
            _semanticFormatterCallback = (SemanticFormatterCallback)__value;
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? divisions
    {
        get => this._divisions;
        set
        {
            var __value = value;
            if ((__value == this._divisions))
            {
                return;
            }
            _divisions = __value;
            markNeedsPaint();
        }
    }
    public virtual string? label
    {
        get => this._label;
        set
        {
            var __value = value;
            if ((__value == this._label))
            {
                return;
            }
            _label = __value;
            _updateLabelPainter();
        }
    }
    public virtual SliderThemeData sliderTheme
    {
        get => this._sliderTheme;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._sliderTheme)))
            {
                return;
            }
            _sliderTheme = __value;
            _updateLabelPainter();
        }
    }
    public virtual double textScaleFactor
    {
        get => this._textScaleFactor;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._textScaleFactor))
            {
                return;
            }
            _textScaleFactor = DartRuntimePrimitives.RequireValue(__value);
            _updateLabelPainter();
        }
    }
    public virtual global::Doroti.Ui.Size screenSize
    {
        get => this._screenSize;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._screenSize)))
            {
                return;
            }
            _screenSize = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::System.Action<double>? onChanged
    {
        get => this._onChanged;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<double>?)__value, (global::System.Action<double>?)this._onChanged)))
            {
                return;
            }
            bool wasInteractive__51963 = this.isInteractive;
            _onChanged = (global::System.Action<double>)__value;
            if ((wasInteractive__51963 != this.isInteractive))
            {
                if (this.isInteractive)
                {
                    ((_SliderState__slider)this._state).enableController.forward();
                }
                else
                {
                    ((_SliderState__slider)this._state).enableController.reverse();
                }
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._textDirection)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            _updateLabelPainter();
        }
    }
    public virtual bool hasFocus
    {
        get => this._hasFocus;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._hasFocus))
            {
                return;
            }
            _hasFocus = DartRuntimePrimitives.RequireValue(__value);
            _updateForFocus(this._hasFocus);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool hovering
    {
        get => this._hovering;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._hovering))
            {
                return;
            }
            _hovering = DartRuntimePrimitives.RequireValue(__value);
            _updateForHover(this._hovering);
        }
    }
    public virtual bool hoveringThumb
    {
        get => this._hoveringThumb;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._hoveringThumb))
            {
                return;
            }
            _hoveringThumb = DartRuntimePrimitives.RequireValue(__value);
            _updateForHover(this._hovering);
        }
    }
    public virtual SliderInteraction allowedInteraction
    {
        get => this._allowedInteraction;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this._allowedInteraction)))
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
            ((_SliderState__slider)this._state).overlayController.forward();
            if (this.shouldShowValueIndicatorWhenDragged)
            {
                ((_SliderState__slider)this._state).valueIndicatorController.forward();
            }
        }
        else
        {
            ((_SliderState__slider)this._state).overlayController.reverse();
            if (this.shouldShowValueIndicatorWhenDragged)
            {
                ((_SliderState__slider)this._state).valueIndicatorController.reverse();
            }
        }
    }

    internal virtual void _updateForHover(bool hovered)
    {
        if ((hovered && this.hoveringThumb))
        {
            ((_SliderState__slider)this._state).overlayController.forward();
        }
        else
        {
            if ((!this._active && !this.hasFocus))
            {
                ((_SliderState__slider)this._state).overlayController.reverse();
            }
        }
    }

    public virtual bool shouldAlwaysShowValueIndicator => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this._sliderTheme.showValueIndicator, ShowValueIndicator.alwaysVisible)));
    public virtual bool shouldShowValueIndicatorWhenDragged => (this._sliderTheme.showValueIndicator! switch { var __constant54689 when (object.Equals(__constant54689, ShowValueIndicator.onlyForDiscrete)) => this.isDiscrete, var __constant54743 when (object.Equals(__constant54743, ShowValueIndicator.onlyForContinuous)) => !this.isDiscrete, var __constant54800 when (object.Equals(__constant54800, ShowValueIndicator.always)) => true, var __constant54829 when (object.Equals(__constant54829, ShowValueIndicator.onDrag)) => true, var __constant54868 when (object.Equals(__constant54868, ShowValueIndicator.never)) => false, var __constant54896 when (object.Equals(__constant54896, ShowValueIndicator.alwaysVisible)) => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual double _adjustmentUnit
    {
        get
        {
            switch (this._platform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return 0.1;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        return 0.05;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
        }
    }
    internal virtual void _updateLabelPainter()
    {
        if ((this.label is not null))
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = this._labelPainter;
    __cascade.text = new global::Doroti.Framework.Painting.TextSpan(style: this._sliderTheme.valueIndicatorTextStyle, text: this.label);
    __cascade.textDirection = this.textDirection;
    __cascade.textScaleFactor = this.textScaleFactor;
    __cascade.layout();
    return __cascade;
}))());
        }
        else
        {
            this._labelPainter.text = null;
        }
        markNeedsLayout();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        this._labelPainter.markNeedsLayout();
        _updateLabelPainter();
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._scheduleSystemFontsUpdate());
        this._overlayAnimation.addListener(() => this.markNeedsPaint());
        this._valueIndicatorAnimation.addListener(() => this.markNeedsPaint());
        this._enableAnimation.addListener(() => this.markNeedsPaint());
        ((_SliderState__slider)this._state).positionController.addListener(() => this.markNeedsPaint());
    }

    public override void detach()
    {
        this._overlayAnimation.removeListener(() => this.markNeedsPaint());
        this._valueIndicatorAnimation.removeListener(() => this.markNeedsPaint());
        this._enableAnimation.removeListener(() => this.markNeedsPaint());
        ((_SliderState__slider)this._state).positionController.removeListener(() => this.markNeedsPaint());
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._scheduleSystemFontsUpdate());
        base.detach();
    }

    public override void dispose()
    {
        this._drag.dispose();
        this._tap.dispose();
        this._labelPainter.dispose();
        this._enableAnimation.dispose();
        this._valueIndicatorAnimation.dispose();
        this._overlayAnimation.dispose();
        base.dispose();
    }

    internal virtual double _getValueFromVisualPosition(double visualPosition)
    {
        return (this.textDirection switch { TextDirection.rtl => (1.0 - visualPosition), TextDirection.ltr => visualPosition, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getValueFromGlobalPosition(Offset globalPosition)
    {
        double visualPosition__57202 = (((globalToLocal(globalPosition).dx - this._trackRect.left)) / this._trackRect.width);
        return _getValueFromVisualPosition(visualPosition__57202);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _discretize(double value)
    {
        double result__57409 = Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0);
        if (this.isDiscrete)
        {
            result__57409 = (((result__57409 * DartRuntimePrimitives.RequireValue(this.divisions))).round() / DartRuntimePrimitives.RequireValue(this.divisions));
        }
        return result__57409;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _startInteraction(Offset globalPosition)
    {
        if (!this._state.mounted)
        {
            return;
        }
        if ((!this._active && this.isInteractive))
        {
            switch (this.allowedInteraction)
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
                            _currentDragValue = this.value;
                        }
                        break;
                    }
                case SliderInteraction.slideOnly:
                    {
                        _active = true;
                        _currentDragValue = this.value;
                        break;
                    }
            }
            if (this._active)
            {
                this.onChangeStart?.Invoke(_discretize(DartRuntimePrimitives.RequireValue(this.value)));
                this.onChanged!(_discretize(this._currentDragValue));
                ((_SliderState__slider)this._state).overlayController.forward();
                if (this.shouldShowValueIndicatorWhenDragged)
                {
                    ((_SliderState__slider)this._state).valueIndicatorController.forward();
                    ((_SliderState__slider)this._state).interactionTimer?.cancel();
                    this._state.interactionTimer = new Timer((_minimumInteractionTime * global::Doroti.Framework.Scheduler.BindingLibrary.timeDilation), (() =>
                    {
                        this._state.interactionTimer = null;
                        if ((!this._active && ((_SliderState__slider)this._state).valueIndicatorController.isCompleted))
                        {
                            ((_SliderState__slider)this._state).valueIndicatorController.reverse();
                        }
                    }));
                }
            }
        }
    }

    internal virtual void _endInteraction()
    {
        if (!this._state.mounted)
        {
            return;
        }
        if ((this._active && this._state.mounted))
        {
            this.onChangeEnd?.Invoke(_discretize(this._currentDragValue));
            _active = false;
            _currentDragValue = 0.0;
            ((_SliderState__slider)this._state).overlayController.reverse();
            if ((this.shouldShowValueIndicatorWhenDragged && (((_SliderState__slider)this._state).interactionTimer is null)))
            {
                ((_SliderState__slider)this._state).valueIndicatorController.reverse();
            }
        }
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        _startInteraction(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition);
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!this._state.mounted)
        {
            return;
        }
        switch (this.allowedInteraction)
        {
            case SliderInteraction.tapAndSlide:
            case SliderInteraction.slideOnly:
            case SliderInteraction.slideThumb:
                {
                    if ((this._active && this.isInteractive))
                    {
                        double valueDelta__59904 = (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta) / this._trackRect.width);
                        _currentDragValue += (this.textDirection switch { TextDirection.rtl => -valueDelta__59904, TextDirection.ltr => valueDelta__59904, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                        this.onChanged!(_discretize(this._currentDragValue));
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
        _startInteraction(((global::Doroti.Framework.Gestures.TapDownDetails)details).globalPosition);
    }

    internal virtual void _handleTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
    {
        _endInteraction();
    }

    internal virtual bool _isPointerOnOverlay(Offset globalPosition)
    {
        return DartRuntimePrimitives.RequireValue(this.overlayRect).contains(globalToLocal(globalPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestSelf(Offset position) => true;
    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry)
    {
        if (!this._state.mounted)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (((@event is global::Doroti.Framework.Gestures.PointerDownEvent) && this.isInteractive))
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as60897 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            this._drag.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as60897));
            this._tap.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as60897));
        }
        if ((this.isInteractive && (this.overlayRect is not null)))
        {
            hoveringThumb = DartRuntimePrimitives.RequireValue(this.overlayRect).contains(((global::Doroti.Framework.Gestures.PointerEvent)@event).localPosition);
        }
    }

    public override double computeMinIntrinsicWidth(double height) => DartRuntimePrimitives.ConvertValue<double>((_minPreferredTrackWidth + this._maxSliderPartWidth));
    public override double computeMaxIntrinsicWidth(double height) => DartRuntimePrimitives.ConvertValue<double>((_minPreferredTrackWidth + this._maxSliderPartWidth));
    public override double computeMinIntrinsicHeight(double width) => Math.Max(this._minPreferredTrackHeight, this._maxSliderPartHeight);
    public override double computeMaxIntrinsicHeight(double width) => Math.Max(this._minPreferredTrackHeight, this._maxSliderPartHeight);
    public override bool sizedByParent => true;
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Ui.Size((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).hasBoundedWidth ? ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth : (_minPreferredTrackWidth + this._maxSliderPartWidth)), (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).hasBoundedHeight ? ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight : Math.Max(this._minPreferredTrackHeight, this._maxSliderPartHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        double controllerValue__62172 = ((_SliderState__slider)this._state).positionController.value;
        var (visualPosition__62436, secondaryVisualPosition__62460) = (this.textDirection switch { TextDirection.rtl when ((this._secondaryTrackValue is null)) => (((double, double?))(((1.0 - controllerValue__62172), (double?)null))), TextDirection.rtl => (((double, double?))(DartRuntimePrimitives.ConvertValue<(double, double?)>(((1.0 - controllerValue__62172), (1.0 - DartRuntimePrimitives.RequireValue(this._secondaryTrackValue)))))), TextDirection.ltr => (((double, double?))((controllerValue__62172, this._secondaryTrackValue))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect__62776 = ((global::Doroti.Ui.Rect)(object?)this._sliderTheme.trackShape!.getPreferredRect(parentBox: this, offset: offset, sliderTheme: this._sliderTheme, isDiscrete: this.isDiscrete));
        global::Doroti.Ui.Offset thumbCenter__62964 = ((global::Doroti.Ui.Offset)(object?)_calcThumbCenter(trackRect: trackRect__62776, visualPosition: visualPosition__62436));
        if (this.isInteractive)
        {
            global::Doroti.Ui.Size overlaySize__63112 = ((global::Doroti.Ui.Size)(object?)this.sliderTheme.overlayShape!.getPreferredSize(this.isInteractive, false));
            overlayRect = global::Doroti.Ui.Rect.fromCircle(center: thumbCenter__62964, radius: (overlaySize__63112.width / 2.0));
        }
        global::Doroti.Ui.Offset? secondaryOffset__63307 = ((global::Doroti.Ui.Offset?)(object?)(((secondaryVisualPosition__62460 is not null)) ? new global::Doroti.Ui.Offset((trackRect__62776.left + (DartRuntimePrimitives.RequireValue(secondaryVisualPosition__62460) * trackRect__62776.width)), ((Offset)((dynamic)trackRect__62776).center).dy) : null));
        double? thumbWidth__63660 = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.width;
        double? thumbHeight__63748 = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.height;
        double? trackGap__63832 = this._sliderTheme.trackGap;
        double? pressedThumbWidth__63884 = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.pressed })?.width;
        double delta__64010 = default!;
        if ((((this._active && (thumbWidth__63660 is not null)) && (pressedThumbWidth__63884 is not null)) && (trackGap__63832 is not null)))
        {
            double thumbWidth__63660__value64036 = DartRuntimePrimitives.RequireValue(thumbWidth__63660);
            double pressedThumbWidth__63884__value64058 = DartRuntimePrimitives.RequireValue(pressedThumbWidth__63884);
            double trackGap__63832__value64087 = DartRuntimePrimitives.RequireValue(trackGap__63832);
            delta__64010 = (DartRuntimePrimitives.RequireValue(thumbWidth__63660__value64036) - DartRuntimePrimitives.RequireValue(pressedThumbWidth__63884__value64058));
            if ((DartRuntimePrimitives.RequireValue(thumbWidth__63660__value64036) > 0.0))
            {
                thumbWidth__63660 = DartRuntimePrimitives.RequireValue(pressedThumbWidth__63884__value64058);
            }
            if ((DartRuntimePrimitives.RequireValue(trackGap__63832__value64087) > 0.0))
            {
                trackGap__63832 = (DartRuntimePrimitives.RequireValue(trackGap__63832__value64087) - (delta__64010 / 2L));
            }
        }
        this._sliderTheme.trackShape!.paint(context, offset, parentBox: this, sliderTheme: this._sliderTheme.copyWith(trackGap: trackGap__63832), enableAnimation: this._enableAnimation, textDirection: this._textDirection, thumbCenter: thumbCenter__62964, secondaryOffset: secondaryOffset__63307, isDiscrete: this.isDiscrete, isEnabled: this.isInteractive);
        if (!this._overlayAnimation.isDismissed)
        {
            this._sliderTheme.overlayShape!.paint(context, thumbCenter__62964, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._labelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: this._value, textScaleFactor: this._textScaleFactor, sizeWithOverflow: (this.screenSize.isEmpty ? this.size : this.screenSize));
        }
        if (this.isDiscrete)
        {
            double tickMarkWidth__65253 = this._sliderTheme.tickMarkShape!.getPreferredSize(isEnabled: this.isInteractive, sliderTheme: this._sliderTheme).width;
            double discreteTrackPadding__65415 = trackRect__62776.height;
            double adjustedTrackWidth__65475 = (trackRect__62776.width - discreteTrackPadding__65415);
            if (((adjustedTrackWidth__65475 / DartRuntimePrimitives.RequireValue(this.divisions)) >= (3.0 * tickMarkWidth__65253)))
            {
                double dy__65700 = ((Offset)((dynamic)trackRect__62776).center).dy;
                for (var i__65743 = 0L; (i__65743 <= DartRuntimePrimitives.RequireValue(this.divisions)); i__65743++)
                {
                    double value__65797 = (i__65743 / DartRuntimePrimitives.RequireValue(this.divisions));
                    double dx__65979 = ((trackRect__62776.left + (DartRuntimePrimitives.RequireValue(value__65797) * adjustedTrackWidth__65475)) + (discreteTrackPadding__65415 / 2L));
                    var tickMarkOffset__66072 = new global::Doroti.Ui.Offset(dx__65979, dy__65700);
                    this._sliderTheme.tickMarkShape!.paint(context, tickMarkOffset__66072, parentBox: this, sliderTheme: this._sliderTheme, enableAnimation: this._enableAnimation, textDirection: this._textDirection, thumbCenter: thumbCenter__62964, isEnabled: this.isInteractive);
                }
            }
        }
        if (((this.isInteractive && (this.label is not null)) && ((((this.shouldShowValueIndicatorWhenDragged && !this._valueIndicatorAnimation.isDismissed)) || this.shouldAlwaysShowValueIndicator))))
        {
            this._state.paintValueIndicator = (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) =>
            {
                if ((this.attached && (((global::Doroti.Framework.Painting.TextPainter)this._labelPainter).text is not null)))
                {
                    this._sliderTheme.valueIndicatorShape?.paint(context, (offset + thumbCenter__62964), activationAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._valueIndicatorAnimation), enableAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._enableAnimation), isDiscrete: this.isDiscrete, labelPainter: this._labelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: this._value, textScaleFactor: this.textScaleFactor, sizeWithOverflow: (this.screenSize.isEmpty ? this.size : this.screenSize));
                }
            });
        }
        else
        {
            this._state.paintValueIndicator = null;
        }
        this._sliderTheme.thumbShape!.paint(context, thumbCenter__62964, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._labelPainter, parentBox: this, sliderTheme: (((thumbWidth__63660 is not null) && (thumbHeight__63748 is not null)) ? this._sliderTheme.copyWith(thumbSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbWidth__63660), DartRuntimePrimitives.RequireValue(thumbHeight__63748)))) : this._sliderTheme), textDirection: this._textDirection, value: this._value, textScaleFactor: this.textScaleFactor, sizeWithOverflow: (this.screenSize.isEmpty ? this.size : this.screenSize));
    }

    internal virtual global::Doroti.Ui.Offset _calcThumbCenter(Rect trackRect, double visualPosition)
    {
        double padding__68691 = (this._sliderTheme.trackShape!.isRounded ? trackRect.height : 0.0);
        double thumbPosition__68779 = (this.isDiscrete ? ((trackRect.left + (visualPosition * ((trackRect.width - padding__68691)))) + (padding__68691 / 2L)) : (trackRect.left + (visualPosition * trackRect.width)));
        global::Doroti.Ui.Size thumbPreferredSize__69134 = ((global::Doroti.Ui.Size)(object?)this._sliderTheme.thumbShape!.getPreferredSize(this.isInteractive, this.isDiscrete));
        double thumbPadding__69261 = ((padding__68691 > (thumbPreferredSize__69134.width / 2L)) ? (padding__68691 / 2L) : 0);
        return new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(thumbPosition__68779, (trackRect.left + thumbPadding__69261), (trackRect.right - thumbPadding__69261)), ((Offset)((dynamic)trackRect).center).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _semanticThumbCenter
    {
        get
        {
            double visualPosition__69542 = (this.textDirection switch { TextDirection.rtl => (1.0 - this._value), TextDirection.ltr => this._value, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return _calcThumbCenter(trackRect: this._trackRect, visualPosition: visualPosition__69542);
            return default!;
        }
    }
    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        node.rect = global::Doroti.Ui.Rect.fromCenter(center: this._semanticThumbCenter, width: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, height: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension);
        node.updateWith(config: config);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.isEnabled = this.isInteractive;
        if ((this.label is not null))
        {
            config.label = this.label!;
        }
        config.isSlider = true;
        config.isFocusable = this.isInteractive;
        config.isFocused = this.hasFocus;
        if ((this.onDidGainAccessibilityFocus is not null))
        {
            config.onDidGainAccessibilityFocus = (global::System.Action)this.onDidGainAccessibilityFocus;
        }
        ((dynamic)config).textDirection = this.textDirection;
        if (this.isInteractive)
        {
            config.onIncrease = (global::System.Action)this.increaseAction;
            config.onDecrease = (global::System.Action)this.decreaseAction;
            config.onFocus = (global::System.Action)this.onFocusAction;
        }
        if ((this.semanticFormatterCallback is not null))
        {
            config.value = this.semanticFormatterCallback!(this._state._lerp(DartRuntimePrimitives.RequireValue(this.value)));
            config.increasedValue = this.semanticFormatterCallback!(this._state._lerp(Dart_uiLibrary.clampDouble((this.value + this._semanticActionUnit), 0.0, 1.0)));
            config.decreasedValue = this.semanticFormatterCallback!(this._state._lerp(Dart_uiLibrary.clampDouble((this.value - this._semanticActionUnit), 0.0, 1.0)));
        }
        else
        {
            config.value = $"{((this.value * 100L)).round()}%";
            config.increasedValue = $"{((Dart_uiLibrary.clampDouble((this.value + this._semanticActionUnit), 0.0, 1.0) * 100L)).round()}%";
            config.decreasedValue = $"{((Dart_uiLibrary.clampDouble((this.value - this._semanticActionUnit), 0.0, 1.0) * 100L)).round()}%";
        }
    }

    internal virtual double _semanticActionUnit => ((this.divisions is not null) ? (1.0 / DartRuntimePrimitives.RequireValue(this.divisions)) : this._adjustmentUnit);
    public virtual void onFocusAction()
    {
        if (this.isInteractive)
        {
            if (!this._state.mounted)
            {
                return;
            }
            if (!this.hasFocus)
            {
                ((_SliderState__slider)this._state).focusNode.requestFocus();
            }
        }
    }

    public virtual void increaseAction()
    {
        if (this.isInteractive)
        {
            this.onChangeStart!(this.currentValue);
            double increase__72119 = increaseValue();
            this.onChanged!(increase__72119);
            this.onChangeEnd!(increase__72119);
            if (!this._state.mounted)
            {
                return;
            }
        }
    }

    public virtual void decreaseAction()
    {
        if (this.isInteractive)
        {
            this.onChangeStart!(this.currentValue);
            double decrease__72375 = decreaseValue();
            this.onChanged!(decrease__72375);
            this.onChangeEnd!(decrease__72375);
            if (!this._state.mounted)
            {
                return;
            }
        }
    }

    public virtual double currentValue
    {
        get
        {
            return Dart_uiLibrary.clampDouble(this.value, 0.0, 1.0);
            return default!;
        }
    }
    public virtual double increaseValue()
    {
        return Dart_uiLibrary.clampDouble((this.value + this._semanticActionUnit), 0.0, 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double decreaseValue()
    {
        return Dart_uiLibrary.clampDouble((this.value - this._semanticActionUnit), 0.0, 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (this._hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        this._hasPendingSystemFontsDidChangeCallBack = true;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => this._hasPendingSystemFontsDidChangeCallBack);
            this._hasPendingSystemFontsDidChangeCallBack = false;
            DartRuntimePrimitives.Assert(() => (this.attached || ((this.debugDisposed ?? true))), () => (object?)$"{this} is detached during {(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase)} but is not disposed.");
            if (this.attached)
            {
                systemFontsDidChange();
            }
        })));
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderValueIndicator__slider(state: this.state));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderValueIndicator__slider)(object)renderObject;
        __renderObject._state = DartRuntimePrimitives.ConvertValue<_SliderState__slider>(this.state);
    }

}

public class _RenderValueIndicator__slider : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RelayoutWhenSystemFontsChangeMixin
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual _SliderState__slider _state { get; set; } = default!;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderValueIndicator__slider(_SliderState__slider state)
    {
        this._state = state;
        _valueIndicatorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_SliderState__slider)this._state).valueIndicatorController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
    }

    public override bool sizedByParent => true;
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._scheduleSystemFontsUpdate());
        this._valueIndicatorAnimation.addListener(() => this.markNeedsPaint());
        ((_SliderState__slider)this._state).positionController.addListener(() => this.markNeedsPaint());
    }

    public override void detach()
    {
        this._valueIndicatorAnimation.removeListener(() => this.markNeedsPaint());
        ((_SliderState__slider)this._state).positionController.removeListener(() => this.markNeedsPaint());
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._scheduleSystemFontsUpdate());
        base.detach();
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        ((_SliderState__slider)this._state).paintValueIndicator?.Invoke(context, offset);
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).smallest;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._valueIndicatorAnimation.dispose();
        base.dispose();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (this._hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        this._hasPendingSystemFontsDidChangeCallBack = true;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
        {
            DartRuntimePrimitives.Assert(() => this._hasPendingSystemFontsDidChangeCallBack);
            this._hasPendingSystemFontsDidChangeCallBack = false;
            DartRuntimePrimitives.Assert(() => (this.attached || ((this.debugDisposed ?? true))), () => (object?)$"{this} is detached during {(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase)} but is not disposed.");
            if (this.attached)
            {
                systemFontsDidChange();
            }
        })));
    }

}

internal class _SliderDefaultsM2__slider : SliderThemeData
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late_sliderTheme_initialized;
    private SliderThemeData __late_sliderTheme = default!;
    public virtual SliderThemeData sliderTheme
    {
        get
        {
            if (!__late_sliderTheme_initialized)
            {
                __late_sliderTheme = SliderTheme.of(this.context);
                __late_sliderTheme_initialized = true;
            }
            return __late_sliderTheme;
        }
    }

    internal _SliderDefaultsM2__slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 4.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.24));
    public virtual global::Doroti.Ui.Color? secondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.32));
    public virtual global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? disabledSecondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Dart_uiLibrary.Color.alphaBlend(this._colors.onSurface.withOpacity(0.38), this._colors.surface));
    public virtual global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.12));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(this.context).textTheme.bodyLarge!.copyWith(color: this._colors.onPrimary);
    public virtual global::Doroti.Ui.Color? valueIndicatorColor
    {
        get
        {
            if ((this.sliderTheme.valueIndicatorShape is RoundedRectSliderValueIndicatorShape))
            {
                return this._colors.inverseSurface;
            }
            return this._colors.primary;
            return default!;
        }
    }
    public override SliderComponentShape? valueIndicatorShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RectangularSliderValueIndicatorShape());
    public override SliderComponentShape? thumbShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderThumbShape());
    public override SliderTrackShape? trackShape => DartRuntimePrimitives.ConvertValue<SliderTrackShape>(new RoundedRectSliderTrackShape());
    public override SliderComponentShape? overlayShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override SliderTickMarkShape? tickMarkShape => DartRuntimePrimitives.ConvertValue<SliderTickMarkShape>(new RoundSliderTickMarkShape());
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _SliderDefaultsM3Year2023__slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 4.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surfaceContainerHighest);
    public virtual global::Doroti.Ui.Color? secondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? disabledSecondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurfaceVariant.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Dart_uiLibrary.Color.alphaBlend(this._colors.onSurface.withOpacity(0.38), this._colors.surface));
    public virtual global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.dragged))
        {
            return this._colors.primary.withOpacity(0.1);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return this._colors.primary.withOpacity(0.08);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return this._colors.primary.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(this.context).textTheme.labelMedium!.copyWith(color: this._colors.onPrimary);
    public virtual global::Doroti.Ui.Color? valueIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
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
                __late__colors = Theme.of(this.context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }

    internal _SliderDefaultsM3__slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 16.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public virtual global::Doroti.Ui.Color? secondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? disabledSecondaryActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(1.0));
    public virtual global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSecondaryContainer.withOpacity(1.0));
    public virtual global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onInverseSurface);
    public virtual global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
    {
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.dragged))
        {
            return this._colors.primary.withOpacity(0.1);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
        {
            return this._colors.primary.withOpacity(0.08);
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
        {
            return this._colors.primary.withOpacity(0.1);
        }
        return Colors.transparent;
        throw new InvalidOperationException("Dart closure completed without a value.");
    })));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(this.context).textTheme.labelLarge!.copyWith(color: this._colors.onInverseSurface);
    public virtual global::Doroti.Ui.Color? valueIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.inverseSurface);
    public override SliderComponentShape? valueIndicatorShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundedRectSliderValueIndicatorShape());
    public override SliderComponentShape? thumbShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new HandleThumbShape());
    public override SliderTrackShape? trackShape => DartRuntimePrimitives.ConvertValue<SliderTrackShape>(new GappedSliderTrackShape());
    public override SliderComponentShape? overlayShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override SliderTickMarkShape? tickMarkShape => DartRuntimePrimitives.ConvertValue<SliderTickMarkShape>(new RoundSliderTickMarkShape(tickMarkRadius: (4.0 / 2L)));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? thumbSize
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>?)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
                {
                    return new global::Doroti.Ui.Size(4.0, 44.0);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return new global::Doroti.Ui.Size(4.0, 44.0);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return new global::Doroti.Ui.Size(2.0, 44.0);
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return new global::Doroti.Ui.Size(2.0, 44.0);
                }
                return new global::Doroti.Ui.Size(4.0, 44.0);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public override double? trackGap => 6.0;
}

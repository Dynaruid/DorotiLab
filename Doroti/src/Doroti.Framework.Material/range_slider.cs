// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/range_slider.dart
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

public delegate void PaintRangeValueIndicator(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset);

public class RangeSlider : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual RangeValues values { get; private set; } = default!;
    public virtual global::System.Action<RangeValues>? onChanged { get; private set; }
    public virtual global::System.Action<RangeValues>? onChangeStart { get; private set; }
    public virtual global::System.Action<RangeValues>? onChangeEnd { get; private set; }
    public virtual double min { get; private set; } = default!;
    public virtual double max { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual RangeLabels? labels { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual SemanticFormatterCallback? semanticFormatterCallback { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual bool? year2023 { get; private set; }
    internal static double _minTouchTargetWidth = global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension;

    public RangeSlider(global::Doroti.Framework.Foundation.Key? key = null, RangeValues values = default!, global::System.Action<RangeValues>? onChanged = default!, global::System.Action<RangeValues>? onChangeStart = null, global::System.Action<RangeValues>? onChangeEnd = null, double min = 0.0, double max = 1.0, long? divisions = null, RangeLabels? labels = null, Color? activeColor = null, Color? inactiveColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, SemanticFormatterCallback? semanticFormatterCallback = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool? year2023 = null) : base(key: key)
    {
        this.values = values;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.min = min;
        this.max = max;
        this.divisions = divisions;
        this.labels = labels;
        this.activeColor = activeColor;
        this.inactiveColor = inactiveColor;
        this.overlayColor = overlayColor;
        this.mouseCursor = mouseCursor;
        this.semanticFormatterCallback = semanticFormatterCallback;
        this.padding = padding;
        this.year2023 = year2023;
        System.Diagnostics.Debug.Assert((min <= max));
        System.Diagnostics.Debug.Assert((((RangeValues)values).start <= ((RangeValues)values).end));
        System.Diagnostics.Debug.Assert(((((RangeValues)values).start >= min) && (((RangeValues)values).start <= max)));
        System.Diagnostics.Debug.Assert(((((RangeValues)values).end >= min) && (((RangeValues)values).end <= max)));
        System.Diagnostics.Debug.Assert(((divisions is null) || (DartRuntimePrimitives.RequireValue(divisions) > 0L)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RangeSliderState__range_slider());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("valueStart", ((RangeValues)this.values).start));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("valueEnd", ((RangeValues)this.values).end));
        properties.add(new global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<RangeValues>>("onChanged", (global::System.Action<RangeValues>?)this.onChanged, ifNull: "disabled"));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<RangeValues>>.CreateHas("onChangeStart", this.onChangeStart));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<global::System.Action<RangeValues>>.CreateHas("onChangeEnd", this.onChangeEnd));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("min", this.min));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("max", this.max));
        properties.add(new global::Doroti.Framework.Foundation.IntProperty("divisions", this.divisions));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("labelStart", this.labels?.start));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("labelEnd", this.labels?.end));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("activeColor", this.activeColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inactiveColor", this.inactiveColor));
        properties.add(global::Doroti.Framework.Foundation.ObjectFlagProperty<SemanticFormatterCallback>.CreateHas("semanticFormatterCallback", this.semanticFormatterCallback));
    }

}

public class _RangeSliderState__range_slider : global::Doroti.Framework.Widgets.State<RangeSlider>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<RangeSlider>
{
    public static Duration enableAnimationDuration = Duration.Create(milliseconds: 75L);
    public static Duration valueIndicatorAnimationDuration = Duration.Create(milliseconds: 100L);
    public virtual global::Doroti.Framework.Widgets.FocusNode startFocusNode { get; private set; } = new global::Doroti.Framework.Widgets.FocusNode();
    public virtual global::Doroti.Framework.Widgets.FocusNode endFocusNode { get; private set; } = new global::Doroti.Framework.Widgets.FocusNode();
    public virtual global::Doroti.Framework.Animation.AnimationController overlayController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController valueIndicatorController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController enableController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController startPositionController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController endPositionController { get; set; } = default!;
    public virtual Timer? interactionTimer { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>? paintTopValueIndicator { get; set; } = default;
    public virtual global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>? paintBottomValueIndicator { get; set; } = default;
    internal virtual bool _dragging { get; set; } = false;
    internal virtual bool _hovering { get; set; } = false;
    internal virtual bool _showHoverHighlight { get; set; } = false;
    internal virtual global::Doroti.Framework.Widgets.OverlayPortalController _valueIndicatorOverlayPortalController { get; private set; } = ((Func<global::Doroti.Framework.Widgets.OverlayPortalController>)(() =>
{            var __cascade = new global::Doroti.Framework.Widgets.OverlayPortalController(debugLabel: "RangeSlider ValueIndicator");
            __cascade.show();
            return __cascade;        }))();
    internal virtual global::Doroti.Framework.Rendering.LayerLink _layerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((RangeSlider)this.widget).onChanged is not null));
    internal virtual void _handleHoverChanged(bool hovering)
    {
        if ((hovering != this._hovering))
        {
            setState(((global::System.Action)(() => {
_hovering = hovering;
_showHoverHighlight = (hovering && this._enabled);
})));
        }
    }

    public override void initState()
    {
        base.initState();
        overlayController = new global::Doroti.Framework.Animation.AnimationController(duration: ConstantsLibrary.kRadialReactionDuration, vsync: this);
        valueIndicatorController = new global::Doroti.Framework.Animation.AnimationController(duration: valueIndicatorAnimationDuration, vsync: this);
        enableController = new global::Doroti.Framework.Animation.AnimationController(duration: enableAnimationDuration, vsync: this, value: (this._enabled ? 1.0 : 0.0));
        startPositionController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.zero, vsync: this, value: _unlerp(((RangeSlider)this.widget).values.start));
        endPositionController = new global::Doroti.Framework.Animation.AnimationController(duration: Duration.zero, vsync: this, value: _unlerp(((RangeSlider)this.widget).values.end));
    }

    public override void didUpdateWidget(RangeSlider oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((object.Equals((global::System.Action<RangeValues>?)((RangeSlider)oldWidget).onChanged, (global::System.Action<RangeValues>?)((RangeSlider)this.widget).onChanged)))
        {
            return;
        }
        var wasEnabled__20623 = (((RangeSlider)oldWidget).onChanged is not null);
        bool isEnabled__20680 = this._enabled;
        if ((wasEnabled__20623 != isEnabled__20680))
        {
            if (isEnabled__20680)
            {
                this.enableController.forward();
            }
            else
            {
                this.enableController.reverse();
            }
            _showHoverHighlight = (this._hovering && isEnabled__20680);
        }
    }

    public override void dispose()
    {
        this.interactionTimer?.cancel();
        this.overlayController.dispose();
        this.valueIndicatorController.dispose();
        this.enableController.dispose();
        this.startPositionController.dispose();
        this.endPositionController.dispose();
        this.startFocusNode.dispose();
        this.endFocusNode.dispose();
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

    internal virtual void _handleChanged(RangeValues values)
    {
        DartRuntimePrimitives.Assert(() => this._enabled);
        RangeValues lerpValues__21334 = ((RangeValues)(object?)_lerpRangeValues(values));
        if ((!object.Equals(lerpValues__21334, ((RangeSlider)this.widget).values)))
        {
            ((RangeSlider)this.widget).onChanged!(lerpValues__21334);
        }
    }

    internal virtual void _handleDragStart(RangeValues values)
    {
        setState(((global::System.Action)(() => {
_dragging = true;
})));
        ((RangeSlider)this.widget).onChangeStart?.Invoke(_lerpRangeValues(values));
    }

    internal virtual void _handleDragEnd(RangeValues values)
    {
        setState(((global::System.Action)(() => {
_dragging = false;
})));
        ((RangeSlider)this.widget).onChangeEnd?.Invoke(_lerpRangeValues(values));
    }

    internal virtual double _lerp(double value) => DartRuntimePrimitives.ConvertValue<double>(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RangeSlider)this.widget).min, ((RangeSlider)this.widget).max, value)));
    internal virtual RangeValues _lerpRangeValues(RangeValues values)
    {
        return new RangeValues(_lerp(((RangeValues)values).start), _lerp(((RangeValues)values).end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _unlerp(double value)
    {
        DartRuntimePrimitives.Assert(() => (value <= ((RangeSlider)this.widget).max));
        DartRuntimePrimitives.Assert(() => (value >= ((RangeSlider)this.widget).min));
        return ((((RangeSlider)this.widget).max > ((RangeSlider)this.widget).min) ? (((value - ((RangeSlider)this.widget).min)) / ((((RangeSlider)this.widget).max - ((RangeSlider)this.widget).min))) : 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual RangeValues _unlerpRangeValues(RangeValues values)
    {
        return new RangeValues(_unlerp(((RangeValues)values).start), _unlerp(((RangeValues)values).end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Thumb? _defaultRangeThumbSelector(TextDirection textDirection, RangeValues values, double tapValue, Size thumbSize, Size trackSize, double dx)
    {
        double touchRadius__23303 = (Math.Max(thumbSize.width, RangeSlider._minTouchTargetWidth) / 2L);
        bool inStartTouchTarget__23397 = ((((tapValue - ((RangeValues)values).start)).abs() * trackSize.width) < touchRadius__23303);
        bool inEndTouchTarget__23498 = ((((tapValue - ((RangeValues)values).end)).abs() * trackSize.width) < touchRadius__23303);
        if ((inStartTouchTarget__23397 && inEndTouchTarget__23498))
        {
            var (towardsStart__24040, towardsEnd__24059) = (textDirection switch { TextDirection.ltr => (((bool, bool))(((dx < 0L), (dx > 0L)))), TextDirection.rtl => (((bool, bool))(((dx > 0L), (dx < 0L)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            if (towardsStart__24040)
            {
                return Thumb.start;
            }
            if (towardsEnd__24059)
            {
                return Thumb.end;
            }
        }
        else
        {
            if (((tapValue * 2L) < (((RangeValues)values).start + ((RangeValues)values).end)))
            {
                return Thumb.start;
            }
            else
            {
                return Thumb.end;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme__24708 = Theme.of(context);
        SliderThemeData sliderTheme__24755 = SliderTheme.of(context);
        bool year2023__24809 = ((((RangeSlider)this.widget).year2023 ?? sliderTheme__24755.year2023) ?? true);
        SliderThemeData defaults__24895 = ((theme__24708.useMaterial3 && !year2023__24809) ? new _RangeSliderDefaultsM3__range_slider(context) : new _RangeSliderDefaultsM2__range_slider(context));
        var states__25429 = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection25438 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection25438.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (this._hovering) { __collection25438.Add(global::Doroti.Framework.Widgets.WidgetState.hovered); } if (this._dragging) { __collection25438.Add(global::Doroti.Framework.Widgets.WidgetState.dragged); } return __collection25438; }))();
        RangeSliderValueIndicatorShape valueIndicatorShape__25909 = (sliderTheme__24755.rangeValueIndicatorShape ?? defaults__24895.rangeValueIndicatorShape!);
        global::Doroti.Ui.Color valueIndicatorColor__26031 = default!;
        if ((valueIndicatorShape__25909 is RectangularRangeSliderValueIndicatorShape))
        {
            RectangularRangeSliderValueIndicatorShape valueIndicatorShape__25909__as26060 = (RectangularRangeSliderValueIndicatorShape)valueIndicatorShape__25909;
            valueIndicatorColor__26031 = (sliderTheme__24755.valueIndicatorColor ?? Dart_uiLibrary.Color.alphaBlend(theme__24708.colorScheme.onSurface.withOpacity(0.6), theme__24708.colorScheme.surface.withOpacity(0.9)));
        }
        else
        {
            valueIndicatorColor__26031 = ((((RangeSlider)this.widget).activeColor ?? sliderTheme__24755.valueIndicatorColor) ?? defaults__24895.valueIndicatorColor!);
        }
        Color? effectiveOverlayColor()
        {
            return ((((((RangeSlider)this.widget).overlayColor?.resolve(states__25429) ?? ((RangeSlider)this.widget).activeColor?.withOpacity(0.12)) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(sliderTheme__24755.overlayColor, states__25429))) ?? defaults__24895.overlayColor);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        sliderTheme__24755 = sliderTheme__24755.copyWith(trackHeight: (sliderTheme__24755.trackHeight ?? defaults__24895.trackHeight), activeTrackColor: ((((RangeSlider)this.widget).activeColor ?? sliderTheme__24755.activeTrackColor) ?? defaults__24895.activeTrackColor), inactiveTrackColor: ((((RangeSlider)this.widget).inactiveColor ?? sliderTheme__24755.inactiveTrackColor) ?? defaults__24895.inactiveTrackColor), disabledActiveTrackColor: (sliderTheme__24755.disabledActiveTrackColor ?? defaults__24895.disabledActiveTrackColor), disabledInactiveTrackColor: (sliderTheme__24755.disabledInactiveTrackColor ?? defaults__24895.disabledInactiveTrackColor), activeTickMarkColor: ((((RangeSlider)this.widget).inactiveColor ?? sliderTheme__24755.activeTickMarkColor) ?? defaults__24895.activeTickMarkColor), inactiveTickMarkColor: ((((RangeSlider)this.widget).activeColor ?? sliderTheme__24755.inactiveTickMarkColor) ?? defaults__24895.inactiveTickMarkColor), disabledActiveTickMarkColor: (sliderTheme__24755.disabledActiveTickMarkColor ?? defaults__24895.disabledActiveTickMarkColor), disabledInactiveTickMarkColor: (sliderTheme__24755.disabledInactiveTickMarkColor ?? defaults__24895.disabledInactiveTickMarkColor), thumbColor: ((((RangeSlider)this.widget).activeColor ?? sliderTheme__24755.thumbColor) ?? defaults__24895.thumbColor), overlappingShapeStrokeColor: (sliderTheme__24755.overlappingShapeStrokeColor ?? defaults__24895.overlappingShapeStrokeColor), disabledThumbColor: (sliderTheme__24755.disabledThumbColor ?? defaults__24895.disabledThumbColor), overlayColor: effectiveOverlayColor(), valueIndicatorColor: valueIndicatorColor__26031, rangeTrackShape: (sliderTheme__24755.rangeTrackShape ?? defaults__24895.rangeTrackShape), rangeTickMarkShape: (sliderTheme__24755.rangeTickMarkShape ?? defaults__24895.rangeTickMarkShape), rangeThumbShape: (sliderTheme__24755.rangeThumbShape ?? defaults__24895.rangeThumbShape), overlayShape: (sliderTheme__24755.overlayShape ?? defaults__24895.overlayShape), rangeValueIndicatorShape: valueIndicatorShape__25909, showValueIndicator: (sliderTheme__24755.showValueIndicator ?? defaults__24895.showValueIndicator), valueIndicatorTextStyle: (sliderTheme__24755.valueIndicatorTextStyle ?? defaults__24895.valueIndicatorTextStyle), minThumbSeparation: (sliderTheme__24755.minThumbSeparation ?? defaults__24895.minThumbSeparation), thumbSelector: ((sliderTheme__24755.thumbSelector ?? (global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>)this._defaultRangeThumbSelector)), padding: (((RangeSlider)this.widget).padding ?? sliderTheme__24755.padding), thumbSize: (sliderTheme__24755.thumbSize ?? defaults__24895.thumbSize), trackGap: (sliderTheme__24755.trackGap ?? defaults__24895.trackGap));
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor__29212 = ((((((RangeSlider)this.widget).mouseCursor?.resolve(states__25429) ?? (global::Doroti.Framework.Services.MouseCursor)sliderTheme__24755.mouseCursor?.resolve(states__25429))) ?? (global::Doroti.Framework.Services.MouseCursor)global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states__25429)));
        Size screenSize()
        {
            return MediaQuery.sizeOf(context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double fontSize__29640 = (sliderTheme__24755.valueIndicatorTextStyle?.fontSize ?? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double fontSizeToScale__29735 = ((fontSize__29640 == 0.0) ? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize : fontSize__29640);
        double effectiveTextScale__29817 = (MediaQuery.textScalerOf(context).scale(fontSizeToScale__29735) / fontSizeToScale__29735);
        global::Doroti.Framework.Widgets.Widget result__29933 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CompositedTransformTarget(link: this._layerLink, child: new global::Doroti.Framework.Widgets.OverlayPortal(controller: this._valueIndicatorOverlayPortalController, overlayChildBuilder: ((context) => {
return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildValueIndicator(DartRuntimePrimitives.RequireValue(sliderTheme__24755.showValueIndicator)));
throw new InvalidOperationException("Dart closure completed without a value.");
}), child: new _RangeSliderRenderObjectWidget__range_slider(values: _unlerpRangeValues(((RangeSlider)this.widget).values), divisions: ((RangeSlider)this.widget).divisions, labels: ((RangeSlider)this.widget).labels, sliderTheme: sliderTheme__24755, textScaleFactor: effectiveTextScale__29817, screenSize: screenSize(), onChanged: ((global::System.Action<RangeValues>)((this._enabled && ((((RangeSlider)this.widget).max > ((RangeSlider)this.widget).min))) ? this._handleChanged : null)), onChangeStart: (global::System.Action<RangeValues>)this._handleDragStart, onChangeEnd: (global::System.Action<RangeValues>)this._handleDragEnd, state: this, semanticFormatterCallback: (SemanticFormatterCallback?)((RangeSlider)this.widget).semanticFormatterCallback, hovering: this._showHoverHighlight))));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding__30868 = (((RangeSlider)this.widget).padding ?? sliderTheme__24755.padding);
        if ((padding__30868 is not null))
        {
            result__29933 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: padding__30868, child: result__29933));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Focus(focusNode: this.startFocusNode, includeSemantics: false, child: global::Doroti.Framework.Widgets.SizedBox.CreateShrink())), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Focus(focusNode: this.endFocusNode, includeSemantics: false, child: global::Doroti.Framework.Widgets.SizedBox.CreateShrink())) })), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MouseRegion(onEnter: ((global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)((_) => { _handleHoverChanged(true); })), onExit: ((global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)((_) => { _handleHoverChanged(false); })), cursor: effectiveMouseCursor__29212, child: result__29933)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildValueIndicator(ShowValueIndicator showValueIndicator)
    {
        global::Doroti.Framework.Widgets.Widget valueIndicator__31818 = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CompositedTransformFollower(link: this._layerLink, child: new _ValueIndicatorRenderObjectWidget__range_slider(state: this)));
        return (showValueIndicator switch { var __constant32003 when (object.Equals(__constant32003, ShowValueIndicator.never)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __constant32062 when (object.Equals(__constant32062, ShowValueIndicator.onlyForDiscrete)) => ((((RangeSlider)this.widget).divisions is not null) ? valueIndicator__31818 : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __constant32183 when (object.Equals(__constant32183, ShowValueIndicator.onlyForContinuous)) => ((((RangeSlider)this.widget).divisions is null) ? valueIndicator__31818 : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __logical32306 when ((object.Equals(__logical32306, ShowValueIndicator.alwaysVisible) || object.Equals(__logical32306, ShowValueIndicator.always))) => valueIndicator__31818, var __constant32383 when (object.Equals(__constant32383, ShowValueIndicator.onDrag)) => valueIndicator__31818, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
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

internal class _RangeSliderRenderObjectWidget__range_slider : global::Doroti.Framework.Widgets.LeafRenderObjectWidget
{
    public virtual RangeValues values { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual RangeLabels? labels { get; private set; }
    public virtual SliderThemeData sliderTheme { get; private set; } = default!;
    public virtual double textScaleFactor { get; private set; } = default!;
    public virtual Size screenSize { get; private set; } = default!;
    public virtual global::System.Action<RangeValues>? onChanged { get; private set; }
    public virtual global::System.Action<RangeValues>? onChangeStart { get; private set; }
    public virtual global::System.Action<RangeValues>? onChangeEnd { get; private set; }
    public virtual SemanticFormatterCallback? semanticFormatterCallback { get; private set; }
    public virtual _RangeSliderState__range_slider state { get; private set; } = default!;
    public virtual bool hovering { get; private set; } = default!;

    internal _RangeSliderRenderObjectWidget__range_slider(RangeValues values, long? divisions, RangeLabels? labels, SliderThemeData sliderTheme, double textScaleFactor, Size screenSize, global::System.Action<RangeValues>? onChanged, global::System.Action<RangeValues>? onChangeStart, global::System.Action<RangeValues>? onChangeEnd, _RangeSliderState__range_slider state, SemanticFormatterCallback? semanticFormatterCallback, bool hovering)
    {
        this.values = values;
        this.divisions = divisions;
        this.labels = labels;
        this.sliderTheme = sliderTheme;
        this.textScaleFactor = textScaleFactor;
        this.screenSize = screenSize;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.state = state;
        this.semanticFormatterCallback = semanticFormatterCallback;
        this.hovering = hovering;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderRangeSlider__range_slider(values: this.values, divisions: this.divisions, labels: this.labels, sliderTheme: this.sliderTheme, theme: Theme.of(context), textScaleFactor: this.textScaleFactor, screenSize: this.screenSize, onChanged: (global::System.Action<RangeValues>?)this.onChanged, onChangeStart: (global::System.Action<RangeValues>?)this.onChangeStart, onChangeEnd: (global::System.Action<RangeValues>?)this.onChangeEnd, state: this.state, textDirection: Directionality.of(context), semanticFormatterCallback: (SemanticFormatterCallback?)this.semanticFormatterCallback, platform: Theme.of(context).platform, hovering: this.hovering, gestureSettings: MediaQuery.gestureSettingsOf(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderRangeSlider__range_slider)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderRangeSlider__range_slider>)(() =>
{            var __cascade = __renderObject;
            __cascade.divisions = this.divisions;
            __cascade.values = this.values;
            __cascade.labels = this.labels;
            __cascade.sliderTheme = this.sliderTheme;
            __cascade.theme = Theme.of(context);
            __cascade.textScaleFactor = this.textScaleFactor;
            __cascade.screenSize = this.screenSize;
            __cascade.onChanged = this.onChanged;
            __cascade.onChangeStart = this.onChangeStart;
            __cascade.onChangeEnd = this.onChangeEnd;
            __cascade.textDirection = Directionality.of(context);
            __cascade.semanticFormatterCallback = this.semanticFormatterCallback;
            __cascade.platform = Theme.of(context).platform;
            __cascade.hovering = this.hovering;
            __cascade.gestureSettings = MediaQuery.gestureSettingsOf(context);
            return __cascade;        }))());
    }

}

public class _RenderRangeSlider__range_slider : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RelayoutWhenSystemFontsChangeMixin
{
    internal virtual Thumb? _lastThumbSelection { get; set; } = default;
    internal static Duration _positionAnimationDuration = Duration.Create(milliseconds: 75L);
    internal const double _minPreferredTrackWidth = 144.0;
    internal static Duration _minimumInteractionTime = Duration.Create(milliseconds: 500L);
    internal virtual _RangeSliderState__range_slider _state { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _overlayAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _enableAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.TextPainter _startLabelPainter { get; private set; } = new global::Doroti.Framework.Painting.TextPainter();
    internal virtual global::Doroti.Framework.Painting.TextPainter _endLabelPainter { get; private set; } = new global::Doroti.Framework.Painting.TextPainter();
    internal virtual global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer _drag { get; set; } = default!;
    internal virtual global::Doroti.Framework.Gestures.TapGestureRecognizer _tap { get; set; } = default!;
    internal virtual bool _active { get; set; } = false;
    internal virtual RangeValues _newValues { get; set; } = default!;
    internal virtual Offset _startThumbCenter { get; set; } = Offset.zero;
    internal virtual Offset _endThumbCenter { get; set; } = Offset.zero;
    public virtual Rect? overlayStartRect { get; set; } = default;
    public virtual Rect? overlayEndRect { get; set; } = default;
    internal virtual RangeValues _values { get; set; } = default!;
    internal virtual global::Doroti.Framework.Foundation.TargetPlatform _platform { get; set; } = default!;
    internal virtual SemanticFormatterCallback? _semanticFormatterCallback { get; set; } = default;
    internal virtual long? _divisions { get; set; } = default;
    internal virtual RangeLabels? _labels { get; set; } = default;
    internal virtual SliderThemeData _sliderTheme { get; set; } = default!;
    internal virtual ThemeData? _theme { get; set; } = default;
    internal virtual double _textScaleFactor { get; set; } = default!;
    internal virtual Size _screenSize { get; set; } = default!;
    internal virtual global::System.Action<RangeValues>? _onChanged { get; set; } = default;
    public virtual global::System.Action<RangeValues>? onChangeStart { get; set; } = default;
    public virtual global::System.Action<RangeValues>? onChangeEnd { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual bool _hovering { get; set; } = default!;
    internal virtual bool _hoveringStartThumb { get; set; } = false;
    internal virtual bool _hoveringEndThumb { get; set; } = false;
    internal virtual global::Doroti.Framework.Semantics.SemanticsNode? _startSemanticsNode { get; set; } = default;
    internal virtual global::Doroti.Framework.Semantics.SemanticsNode? _endSemanticsNode { get; set; } = default;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderRangeSlider__range_slider(RangeValues values, long? divisions, RangeLabels? labels, SliderThemeData sliderTheme, ThemeData? theme, double textScaleFactor, Size screenSize, global::Doroti.Framework.Foundation.TargetPlatform platform, global::System.Action<RangeValues>? onChanged, SemanticFormatterCallback? semanticFormatterCallback, global::System.Action<RangeValues>? onChangeStart, global::System.Action<RangeValues>? onChangeEnd, _RangeSliderState__range_slider state, TextDirection textDirection, bool hovering, global::Doroti.Framework.Gestures.DeviceGestureSettings gestureSettings)
    {
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this._platform = platform;
        this._semanticFormatterCallback = semanticFormatterCallback;
        this._labels = labels;
        this._values = values;
        this._divisions = divisions;
        this._sliderTheme = sliderTheme;
        this._theme = theme;
        this._textScaleFactor = textScaleFactor;
        this._screenSize = screenSize;
        this._onChanged = onChanged;
        this._state = state;
        this._textDirection = textDirection;
        this._hovering = hovering;
        System.Diagnostics.Debug.Assert(((((RangeValues)values).start >= 0.0) && (((RangeValues)values).start <= 1.0)));
        System.Diagnostics.Debug.Assert(((((RangeValues)values).end >= 0.0) && (((RangeValues)values).end <= 1.0)));
        _updateLabelPainters();
        var team__36175 = new global::Doroti.Framework.Gestures.GestureArenaTeam();
        _drag = ((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{            var __cascade = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer();
            __cascade.team = team__36175;
            __cascade.onStart = this._handleDragStart;
            __cascade.onUpdate = this._handleDragUpdate;
            __cascade.onEnd = this._handleDragEnd;
            __cascade.onCancel = this._handleDragCancel;
            __cascade.gestureSettings = gestureSettings;
            return __cascade;        }))();
        _tap = ((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
{            var __cascade = new global::Doroti.Framework.Gestures.TapGestureRecognizer();
            __cascade.team = team__36175;
            __cascade.onTapDown = this._handleTapDown;
            __cascade.onTapUp = this._handleTapUp;
            __cascade.gestureSettings = gestureSettings;
            return __cascade;        }))();
        _overlayAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_RangeSliderState__range_slider)this._state).overlayController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _valueIndicatorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_RangeSliderState__range_slider)this._state).valueIndicatorController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        _enableAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_RangeSliderState__range_slider)this._state).enableController, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
    }

    internal virtual double _maxSliderPartWidth => this._sliderPartSizes.map<Size, double>(((size) => size.width)).reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
    internal virtual double _maxSliderPartHeight => this._sliderPartSizes.map<Size, double>(((size) => size.height)).reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
    internal virtual double _thumbSizeHeight => this._sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete).height;
    internal virtual double _overlayHeight => this._sliderTheme.overlayShape!.getPreferredSize(this.isEnabled, this.isDiscrete).height;
    internal virtual List<global::Doroti.Ui.Size> _sliderPartSizes => new List<global::Doroti.Ui.Size> { new global::Doroti.Ui.Size(this._sliderTheme.overlayShape!.getPreferredSize(this.isEnabled, this.isDiscrete).width, ((this._sliderTheme.padding is not null) ? this._thumbSizeHeight : this._overlayHeight)), this._sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete), this._sliderTheme.rangeTickMarkShape!.getPreferredSize(isEnabled: this.isEnabled, sliderTheme: this.sliderTheme) }.Cast<global::Doroti.Ui.Size>().ToList();
    internal virtual double? _minPreferredTrackHeight => this._sliderTheme.trackHeight;
    internal virtual global::Doroti.Ui.Rect _trackRect => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>(this._sliderTheme.rangeTrackShape!.getPreferredRect(parentBox: this, sliderTheme: this._sliderTheme, isDiscrete: false));
    public virtual bool isEnabled => DartRuntimePrimitives.ConvertValue<bool>((this.onChanged is not null));
    public virtual bool isDiscrete => DartRuntimePrimitives.ConvertValue<bool>(((this.divisions is not null) && (DartRuntimePrimitives.RequireValue(this.divisions) > 0L)));
    internal virtual double _minThumbSeparationValue => (this.isDiscrete ? 0 : (DartRuntimePrimitives.RequireValue(this.sliderTheme.minThumbSeparation) / this._trackRect.width));
    public virtual RangeValues values
    {
        get => this._values;
        set
        {
            var newValues = value;
            DartRuntimePrimitives.Assert(() => ((((RangeValues)newValues).start >= 0.0) && (((RangeValues)newValues).start <= 1.0)));
            DartRuntimePrimitives.Assert(() => ((((RangeValues)newValues).end >= 0.0) && (((RangeValues)newValues).end <= 1.0)));
            DartRuntimePrimitives.Assert(() => (((RangeValues)newValues).start <= ((RangeValues)newValues).end));
            RangeValues convertedValues__39864 = (this.isDiscrete ? _discretizeRangeValues(newValues) : newValues);
            if ((object.Equals(convertedValues__39864, this._values)))
            {
                return;
            }
            _values = convertedValues__39864;
            if (this.isDiscrete)
            {
                double startDistance__40342 = ((((RangeValues)this._values).start - ((_RangeSliderState__range_slider)this._state).startPositionController.value)).abs();
                ((_RangeSliderState__range_slider)this._state).startPositionController.duration = ((startDistance__40342 != 0.0) ? (_positionAnimationDuration * ((1.0 / startDistance__40342))) : Duration.zero);
                ((_RangeSliderState__range_slider)this._state).startPositionController.animateTo(((RangeValues)this._values).start, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
                double endDistance__40686 = ((((RangeValues)this._values).end - ((_RangeSliderState__range_slider)this._state).endPositionController.value)).abs();
                ((_RangeSliderState__range_slider)this._state).endPositionController.duration = ((endDistance__40686 != 0.0) ? (_positionAnimationDuration * ((1.0 / endDistance__40686))) : Duration.zero);
                ((_RangeSliderState__range_slider)this._state).endPositionController.animateTo(((RangeValues)this._values).end, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
            }
            else
            {
                ((_RangeSliderState__range_slider)this._state).startPositionController.value = ((RangeValues)convertedValues__39864).start;
                ((_RangeSliderState__range_slider)this._state).endPositionController.value = ((RangeValues)convertedValues__39864).end;
            }
            markNeedsSemanticsUpdate();
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
    public virtual RangeLabels? labels
    {
        get => this._labels;
        set
        {
            var labels = value;
            if ((object.Equals(labels, this._labels)))
            {
                return;
            }
            _labels = labels;
            _updateLabelPainters();
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
            markNeedsPaint();
        }
    }
    public virtual ThemeData? theme
    {
        get => this._theme;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._theme)))
            {
                return;
            }
            _theme = __value;
            markNeedsPaint();
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
            _updateLabelPainters();
        }
    }
    public virtual global::Doroti.Ui.Size screenSize
    {
        get => this._screenSize;
        set
        {
            var __value = value;
            if ((object.Equals(DartRuntimePrimitives.RequireValue(__value), this.screenSize)))
            {
                return;
            }
            _screenSize = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    public virtual global::System.Action<RangeValues>? onChanged
    {
        get => this._onChanged;
        set
        {
            var __value = value;
            if ((object.Equals((global::System.Action<RangeValues>?)__value, (global::System.Action<RangeValues>?)this._onChanged)))
            {
                return;
            }
            bool wasEnabled__43438 = this.isEnabled;
            _onChanged = (global::System.Action<RangeValues>)__value;
            if ((wasEnabled__43438 != this.isEnabled))
            {
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
            _updateLabelPainters();
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
    public virtual bool hoveringStartThumb
    {
        get => this._hoveringStartThumb;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._hoveringStartThumb))
            {
                return;
            }
            _hoveringStartThumb = DartRuntimePrimitives.RequireValue(__value);
            _updateForHover(this._hovering);
        }
    }
    public virtual bool hoveringEndThumb
    {
        get => this._hoveringEndThumb;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._hoveringEndThumb))
            {
                return;
            }
            _hoveringEndThumb = DartRuntimePrimitives.RequireValue(__value);
            _updateForHover(this._hovering);
        }
    }
    internal virtual void _updateForHover(bool hovered)
    {
        if ((hovered && ((this.hoveringStartThumb || this.hoveringEndThumb))))
        {
            ((_RangeSliderState__range_slider)this._state).overlayController.forward();
        }
        else
        {
            ((_RangeSliderState__range_slider)this._state).overlayController.reverse();
        }
    }

    public virtual bool shouldAlwaysShowValueIndicator => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(this._sliderTheme.showValueIndicator, ShowValueIndicator.alwaysVisible)));
    public virtual bool shouldShowValueIndicatorWhenDragged => (this._sliderTheme.showValueIndicator! switch { var __constant45371 when (object.Equals(__constant45371, ShowValueIndicator.onlyForDiscrete)) => this.isDiscrete, var __constant45425 when (object.Equals(__constant45425, ShowValueIndicator.onlyForContinuous)) => !this.isDiscrete, var __logical45482 when ((object.Equals(__logical45482, ShowValueIndicator.alwaysVisible) || object.Equals(__logical45482, ShowValueIndicator.always))) => true, var __constant45555 when (object.Equals(__constant45555, ShowValueIndicator.onDrag)) => true, var __constant45594 when (object.Equals(__constant45594, ShowValueIndicator.never)) => false, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual global::Doroti.Ui.Size _thumbSize => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Size>(this._sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete));
    internal virtual double _adjustmentUnit
    {
        get
        {
            switch (this._platform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                    {
                        return 0.1;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
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
    internal virtual void _updateLabelPainters()
    {
        _updateLabelPainter(Thumb.start);
        _updateLabelPainter(Thumb.end);
    }

    internal virtual void _updateLabelPainter(Thumb thumb)
    {
        RangeLabels? labels__46338 = this.labels;
        if ((labels__46338 is null))
        {
            return;
        }
        var (text__46425, labelPainter__46443) = (thumb switch { var __constant46482 when (object.Equals(__constant46482, Thumb.start)) => (((string, global::Doroti.Framework.Painting.TextPainter))((((RangeLabels)labels__46338).start, this._startLabelPainter))), var __constant46539 when (object.Equals(__constant46539, Thumb.end)) => (((string, global::Doroti.Framework.Painting.TextPainter))((((RangeLabels)labels__46338).end, this._endLabelPainter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{            var __cascade = labelPainter__46443;
            __cascade.text = new global::Doroti.Framework.Painting.TextSpan(style: this._sliderTheme.valueIndicatorTextStyle, text: text__46425);
            __cascade.textDirection = this.textDirection;
            __cascade.textScaleFactor = this.textScaleFactor;
            __cascade.layout();
            return __cascade;        }))());
        markNeedsLayout();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        this._startLabelPainter.markNeedsLayout();
        this._endLabelPainter.markNeedsLayout();
        _updateLabelPainters();
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._scheduleSystemFontsUpdate());
        this._overlayAnimation.addListener(() => this.markNeedsPaint());
        this._valueIndicatorAnimation.addListener(() => this.markNeedsPaint());
        this._enableAnimation.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startPositionController.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).endPositionController.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startFocusNode.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startFocusNode.addListener(() => this.markNeedsSemanticsUpdate());
        ((_RangeSliderState__range_slider)this._state).endFocusNode.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).endFocusNode.addListener(() => this.markNeedsSemanticsUpdate());
    }

    public override void detach()
    {
        this._overlayAnimation.removeListener(() => this.markNeedsPaint());
        this._valueIndicatorAnimation.removeListener(() => this.markNeedsPaint());
        this._enableAnimation.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startPositionController.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).endPositionController.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startFocusNode.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startFocusNode.removeListener(() => this.markNeedsSemanticsUpdate());
        ((_RangeSliderState__range_slider)this._state).endFocusNode.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).endFocusNode.removeListener(() => this.markNeedsSemanticsUpdate());
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._scheduleSystemFontsUpdate());
        base.detach();
    }

    public override void dispose()
    {
        this._drag.dispose();
        this._tap.dispose();
        this._startLabelPainter.dispose();
        this._endLabelPainter.dispose();
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
        double visualPosition__49003 = (((globalToLocal(globalPosition).dx - this._trackRect.left)) / this._trackRect.width);
        return _getValueFromVisualPosition(visualPosition__49003);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _discretize(double value)
    {
        double result__49210 = Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0);
        if (this.isDiscrete)
        {
            result__49210 = (((result__49210 * DartRuntimePrimitives.RequireValue(this.divisions))).round() / DartRuntimePrimitives.RequireValue(this.divisions));
        }
        return result__49210;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual RangeValues _discretizeRangeValues(RangeValues values)
    {
        return new RangeValues(_discretize(((RangeValues)values).start), _discretize(((RangeValues)values).end));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _startInteraction(Offset globalPosition)
    {
        if (this._active)
        {
            return;
        }
        double tapValue__49607 = Dart_uiLibrary.clampDouble(_getValueFromGlobalPosition(globalPosition), 0.0, 1.0);
        _lastThumbSelection = this.sliderTheme.thumbSelector!(this.textDirection, this.values, tapValue__49607, this._thumbSize, this.size, 0);
        if ((this._lastThumbSelection is not null))
        {
            switch (this._lastThumbSelection!)
            {
                case var __constant49928 when (object.Equals(__constant49928, Thumb.start)):
                    {
                        ((_RangeSliderState__range_slider)this._state).startFocusNode.requestFocus();
                        break;
                    }
                case var __constant50002 when (object.Equals(__constant50002, Thumb.end)):
                    {
                        ((_RangeSliderState__range_slider)this._state).endFocusNode.requestFocus();
                        break;
                    }
            }
            _active = true;
            RangeValues currentValues__50331 = ((RangeValues)(object?)_discretizeRangeValues(this.values));
            _newValues = (this._lastThumbSelection! switch { var __constant50438 when (object.Equals(__constant50438, Thumb.start)) => new RangeValues(tapValue__49607, ((RangeValues)currentValues__50331).end), var __constant50503 when (object.Equals(__constant50503, Thumb.end)) => new RangeValues(((RangeValues)currentValues__50331).start, tapValue__49607), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            _updateLabelPainter(DartRuntimePrimitives.RequireValue(this._lastThumbSelection));
            this.onChangeStart?.Invoke(currentValues__50331);
            this.onChanged!(_discretizeRangeValues(this._newValues));
            ((_RangeSliderState__range_slider)this._state).overlayController.forward();
            if (this.shouldShowValueIndicatorWhenDragged)
            {
                ((_RangeSliderState__range_slider)this._state).valueIndicatorController.forward();
                ((_RangeSliderState__range_slider)this._state).interactionTimer?.cancel();
                this._state.interactionTimer = new Timer((_minimumInteractionTime * global::Doroti.Framework.Scheduler.BindingLibrary.timeDilation), (() => {
this._state.interactionTimer = null;
if ((!this._active && ((_RangeSliderState__range_slider)this._state).valueIndicatorController.isCompleted))
{
    ((_RangeSliderState__range_slider)this._state).valueIndicatorController.reverse();
}
}));
            }
        }
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if (!this._state.mounted)
        {
            return;
        }
        double dragValue__51319 = _getValueFromGlobalPosition(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition);
        var shouldCallOnChangeStart__51608 = false;
        if ((this._lastThumbSelection is null))
        {
            _lastThumbSelection = this.sliderTheme.thumbSelector!(this.textDirection, this.values, dragValue__51319, this._thumbSize, this.size, ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta.dx);
            if ((this._lastThumbSelection is not null))
            {
                shouldCallOnChangeStart__51608 = true;
                _active = true;
                ((_RangeSliderState__range_slider)this._state).overlayController.forward();
                if (this.shouldShowValueIndicatorWhenDragged)
                {
                    ((_RangeSliderState__range_slider)this._state).valueIndicatorController.forward();
                }
            }
        }
        if ((this.isEnabled && (this._lastThumbSelection is not null)))
        {
            RangeValues currentValues__52217 = ((RangeValues)(object?)_discretizeRangeValues(this.values));
            if (((this.onChangeStart is not null) && shouldCallOnChangeStart__51608))
            {
                this.onChangeStart!(currentValues__52217);
            }
            double currentDragValue__52393 = _discretize(dragValue__51319);
            _newValues = (this._lastThumbSelection! switch { var __constant52496 when (object.Equals(__constant52496, Thumb.start)) => new RangeValues(Math.Min(currentDragValue__52393, (((RangeValues)currentValues__52217).end - this._minThumbSeparationValue)), ((RangeValues)currentValues__52217).end), var __constant52656 when (object.Equals(__constant52656, Thumb.end)) => new RangeValues(((RangeValues)currentValues__52217).start, Math.Max(currentDragValue__52393, (((RangeValues)currentValues__52217).start + this._minThumbSeparationValue))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            this.onChanged!(_discretizeRangeValues(this._newValues));
        }
    }

    internal virtual void _endInteraction()
    {
        if (!this._state.mounted)
        {
            return;
        }
        if ((this.shouldShowValueIndicatorWhenDragged && (((_RangeSliderState__range_slider)this._state).interactionTimer is null)))
        {
            ((_RangeSliderState__range_slider)this._state).valueIndicatorController.reverse();
        }
        if (((this._active && this._state.mounted) && (this._lastThumbSelection is not null)))
        {
            RangeValues discreteValues__53189 = ((RangeValues)(object?)_discretizeRangeValues(this._newValues));
            this.onChangeEnd?.Invoke(discreteValues__53189);
            _active = false;
        }
        ((_RangeSliderState__range_slider)this._state).overlayController.reverse();
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        _startInteraction(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        _endInteraction();
    }

    internal virtual void _handleDragCancel()
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

    public override bool hitTestSelf(Offset position) => true;
    public override void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, global::Doroti.Framework.Gestures.HitTestEntry<global::Doroti.Framework.Gestures.HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if (((@event is global::Doroti.Framework.Gestures.PointerDownEvent) && this.isEnabled))
        {
            global::Doroti.Framework.Gestures.PointerDownEvent @event__as53949 = (global::Doroti.Framework.Gestures.PointerDownEvent)@event;
            this._drag.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as53949));
            this._tap.addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)((global::Doroti.Framework.Gestures.PointerDownEvent)@event__as53949));
        }
        if (this.isEnabled)
        {
            if ((this.overlayStartRect is not null))
            {
                hoveringStartThumb = DartRuntimePrimitives.RequireValue(this.overlayStartRect).contains(((global::Doroti.Framework.Gestures.PointerEvent)@event).localPosition);
            }
            if ((this.overlayEndRect is not null))
            {
                hoveringEndThumb = DartRuntimePrimitives.RequireValue(this.overlayEndRect).contains(((global::Doroti.Framework.Gestures.PointerEvent)@event).localPosition);
            }
        }
    }

    public override double computeMinIntrinsicWidth(double height) => DartRuntimePrimitives.ConvertValue<double>((_minPreferredTrackWidth + this._maxSliderPartWidth));
    public override double computeMaxIntrinsicWidth(double height) => DartRuntimePrimitives.ConvertValue<double>((_minPreferredTrackWidth + this._maxSliderPartWidth));
    public override double computeMinIntrinsicHeight(double width) => Math.Max(DartRuntimePrimitives.RequireValue(this._minPreferredTrackHeight), this._maxSliderPartHeight);
    public override double computeMaxIntrinsicHeight(double width) => Math.Max(DartRuntimePrimitives.RequireValue(this._minPreferredTrackHeight), this._maxSliderPartHeight);
    public override bool sizedByParent => true;
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return new global::Doroti.Ui.Size((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).hasBoundedWidth ? ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth : (_minPreferredTrackWidth + this._maxSliderPartWidth)), (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).hasBoundedHeight ? ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight : Math.Max(DartRuntimePrimitives.RequireValue(this._minPreferredTrackHeight), this._maxSliderPartHeight)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        double startValue__55372 = ((_RangeSliderState__range_slider)this._state).startPositionController.value;
        double endValue__55440 = ((_RangeSliderState__range_slider)this._state).endPositionController.value;
        var (startVisualPosition__55700, endVisualPosition__55728) = (this.textDirection switch { TextDirection.rtl => (((double, double))(((1.0 - startValue__55372), (1.0 - endValue__55440)))), TextDirection.ltr => (((double, double))((startValue__55372, endValue__55440))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect__55911 = ((global::Doroti.Ui.Rect)(object?)this._sliderTheme.rangeTrackShape!.getPreferredRect(parentBox: this, offset: offset, sliderTheme: this._sliderTheme, isDiscrete: this.isDiscrete));
        double padding__56103 = (this._sliderTheme.rangeTrackShape!.isRounded ? trackRect__55911.height : 0.0);
        double thumbYOffset__56196 = ((Offset)((dynamic)trackRect__55911).center).dy;
        double startThumbPosition__56249 = (this.isDiscrete ? ((trackRect__55911.left + (startVisualPosition__55700 * ((trackRect__55911.width - padding__56103)))) + (padding__56103 / 2L)) : (trackRect__55911.left + (startVisualPosition__55700 * trackRect__55911.width)));
        double endThumbPosition__56455 = (this.isDiscrete ? ((trackRect__55911.left + (endVisualPosition__55728 * ((trackRect__55911.width - padding__56103)))) + (padding__56103 / 2L)) : (trackRect__55911.left + (endVisualPosition__55728 * trackRect__55911.width)));
        global::Doroti.Ui.Size thumbPreferredSize__56653 = ((global::Doroti.Ui.Size)(object?)this._sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete));
        double thumbPadding__56781 = (((padding__56103 > (thumbPreferredSize__56653.width / 2L)) ? (padding__56103 / 2L) : 0));
        _startThumbCenter = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(startThumbPosition__56249, (trackRect__55911.left + thumbPadding__56781), (trackRect__55911.right - thumbPadding__56781)), thumbYOffset__56196);
        _endThumbCenter = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(endThumbPosition__56455, (trackRect__55911.left + thumbPadding__56781), (trackRect__55911.right - thumbPadding__56781)), thumbYOffset__56196);
        if (this.isEnabled)
        {
            global::Doroti.Ui.Size overlaySize__57245 = ((global::Doroti.Ui.Size)(object?)this.sliderTheme.overlayShape!.getPreferredSize(this.isEnabled, false));
            overlayStartRect = global::Doroti.Ui.Rect.fromCircle(center: this._startThumbCenter, radius: (overlaySize__57245.width / 2.0));
            overlayEndRect = global::Doroti.Ui.Rect.fromCircle(center: this._endThumbCenter, radius: (overlaySize__57245.width / 2.0));
        }
        double? thumbWidth__57746 = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.width;
        double? thumbHeight__57834 = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.height;
        double? trackGap__57918 = this._sliderTheme.trackGap;
        double? pressedThumbWidth__57970 = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.pressed })?.width;
        double delta__58096 = default!;
        if ((((this._active && (thumbWidth__57746 is not null)) && (pressedThumbWidth__57970 is not null)) && (trackGap__57918 is not null)))
        {
            double thumbWidth__57746__value58122 = DartRuntimePrimitives.RequireValue(thumbWidth__57746);
            double pressedThumbWidth__57970__value58144 = DartRuntimePrimitives.RequireValue(pressedThumbWidth__57970);
            double trackGap__57918__value58173 = DartRuntimePrimitives.RequireValue(trackGap__57918);
            delta__58096 = (DartRuntimePrimitives.RequireValue(thumbWidth__57746__value58122) - DartRuntimePrimitives.RequireValue(pressedThumbWidth__57970__value58144));
            thumbWidth__57746 = DartRuntimePrimitives.RequireValue(pressedThumbWidth__57970__value58144);
            if ((DartRuntimePrimitives.RequireValue(trackGap__57918__value58173) > 0.0))
            {
                trackGap__57918 = (DartRuntimePrimitives.RequireValue(trackGap__57918__value58173) - (delta__58096 / 2L));
            }
        }
        this._sliderTheme.rangeTrackShape!.paint(context, offset, parentBox: this, sliderTheme: this._sliderTheme.copyWith(trackGap: trackGap__57918), enableAnimation: this._enableAnimation, textDirection: this._textDirection, startThumbCenter: this._startThumbCenter, endThumbCenter: this._endThumbCenter, isDiscrete: this.isDiscrete, isEnabled: this.isEnabled);
        bool startThumbSelected__58757 = ((object.Equals(this._lastThumbSelection, Thumb.start)) && !this.hoveringEndThumb);
        bool endThumbSelected__58850 = ((object.Equals(this._lastThumbSelection, Thumb.end)) && !this.hoveringStartThumb);
        global::Doroti.Ui.Size resolvedscreenSize__58941 = ((global::Doroti.Ui.Size)(object?)(this.screenSize.isEmpty ? this.size : this.screenSize));
        if (((_RangeSliderState__range_slider)this._state).startFocusNode.hasFocus)
        {
            this._sliderTheme.overlayShape!.paint(context, this._startThumbCenter, activationAnimation: new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0), enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._startLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: startValue__55372, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
        }
        if (((_RangeSliderState__range_slider)this._state).endFocusNode.hasFocus)
        {
            this._sliderTheme.overlayShape!.paint(context, this._endThumbCenter, activationAnimation: new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0), enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._endLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: endValue__55440, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
        }
        if (!this._overlayAnimation.isDismissed)
        {
            if ((startThumbSelected__58757 || this.hoveringStartThumb))
            {
                this._sliderTheme.overlayShape!.paint(context, this._startThumbCenter, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._startLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: startValue__55372, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
            }
            if ((endThumbSelected__58850 || this.hoveringEndThumb))
            {
                this._sliderTheme.overlayShape!.paint(context, this._endThumbCenter, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._endLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: endValue__55440, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
            }
        }
        if (this.isDiscrete)
        {
            double tickMarkWidth__61293 = this._sliderTheme.rangeTickMarkShape!.getPreferredSize(isEnabled: this.isEnabled, sliderTheme: this._sliderTheme).width;
            double discreteTrackPadding__61456 = trackRect__55911.height;
            double adjustedTrackWidth__61516 = (trackRect__55911.width - discreteTrackPadding__61456);
            if (((adjustedTrackWidth__61516 / DartRuntimePrimitives.RequireValue(this.divisions)) >= (3.0 * tickMarkWidth__61293)))
            {
                double dy__61741 = ((Offset)((dynamic)trackRect__55911).center).dy;
                for (var i__61784 = 0L; (i__61784 <= DartRuntimePrimitives.RequireValue(this.divisions)); i__61784++)
                {
                    double value__61838 = (i__61784 / DartRuntimePrimitives.RequireValue(this.divisions));
                    double dx__62020 = ((trackRect__55911.left + (DartRuntimePrimitives.RequireValue(value__61838) * adjustedTrackWidth__61516)) + (discreteTrackPadding__61456 / 2L));
                    var tickMarkOffset__62113 = new global::Doroti.Ui.Offset(dx__62020, dy__61741);
                    this._sliderTheme.rangeTickMarkShape!.paint(context, tickMarkOffset__62113, parentBox: this, sliderTheme: this._sliderTheme, enableAnimation: this._enableAnimation, textDirection: this._textDirection, startThumbCenter: this._startThumbCenter, endThumbCenter: this._endThumbCenter, isEnabled: this.isEnabled);
                }
            }
        }
        double thumbDelta__62586 = ((this._endThumbCenter.dx - this._startThumbCenter.dx)).abs();
        var isLastThumbStart__62661 = (object.Equals(this._lastThumbSelection, Thumb.start));
        Thumb bottomThumb__62732 = (isLastThumbStart__62661 ? Thumb.end : Thumb.start);
        Thumb topThumb__62806 = (isLastThumbStart__62661 ? Thumb.start : Thumb.end);
        global::Doroti.Ui.Offset bottomThumbCenter__62878 = ((global::Doroti.Ui.Offset)(object?)(isLastThumbStart__62661 ? this._endThumbCenter : this._startThumbCenter));
        global::Doroti.Ui.Offset topThumbCenter__62971 = ((global::Doroti.Ui.Offset)(object?)(isLastThumbStart__62661 ? this._startThumbCenter : this._endThumbCenter));
        global::Doroti.Framework.Painting.TextPainter bottomLabelPainter__63066 = (isLastThumbStart__62661 ? this._endLabelPainter : this._startLabelPainter);
        global::Doroti.Framework.Painting.TextPainter topLabelPainter__63167 = (isLastThumbStart__62661 ? this._startLabelPainter : this._endLabelPainter);
        var bottomValue__63253 = (isLastThumbStart__62661 ? endValue__55440 : startValue__55372);
        var topValue__63319 = (isLastThumbStart__62661 ? startValue__55372 : endValue__55440);
        bool shouldPaintValueIndicators__63387 = ((this.isEnabled && (this.labels is not null)) && ((((this.shouldShowValueIndicatorWhenDragged && !this._valueIndicatorAnimation.isDismissed)) || this.shouldAlwaysShowValueIndicator)));
        if (shouldPaintValueIndicators__63387)
        {
            this._state.paintBottomValueIndicator = (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) => {
if (this.attached)
{
    this._sliderTheme.rangeValueIndicatorShape!.paint(context, bottomThumbCenter__62878, activationAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._valueIndicatorAnimation), enableAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._enableAnimation), isDiscrete: this.isDiscrete, isOnTop: false, labelPainter: bottomLabelPainter__63066, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, thumb: bottomThumb__62732, value: bottomValue__63253, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
}
});
        }
        this._sliderTheme.rangeThumbShape!.paint(context, bottomThumbCenter__62878, activationAnimation: this._valueIndicatorAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, isOnTop: false, textDirection: this.textDirection, sliderTheme: (((thumbWidth__57746 is not null) && (thumbHeight__57834 is not null)) ? this._sliderTheme.copyWith(thumbSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbWidth__57746), DartRuntimePrimitives.RequireValue(thumbHeight__57834)))) : this._sliderTheme), thumb: bottomThumb__62732, isPressed: ((object.Equals(bottomThumb__62732, Thumb.start)) ? startThumbSelected__58757 : endThumbSelected__58850));
        if (shouldPaintValueIndicators__63387)
        {
            double startOffset__65256 = this.sliderTheme.rangeValueIndicatorShape!.getHorizontalShift(parentBox: this, center: this._startThumbCenter, labelPainter: this._startLabelPainter, activationAnimation: this._valueIndicatorAnimation, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
            double endOffset__65601 = this.sliderTheme.rangeValueIndicatorShape!.getHorizontalShift(parentBox: this, center: this._endThumbCenter, labelPainter: this._endLabelPainter, activationAnimation: this._valueIndicatorAnimation, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
            double startHalfWidth__65940 = (this.sliderTheme.rangeValueIndicatorShape!.getPreferredSize(this.isEnabled, this.isDiscrete, labelPainter: this._startLabelPainter, textScaleFactor: this.textScaleFactor).width / 2L);
            double endHalfWidth__66264 = (this.sliderTheme.rangeValueIndicatorShape!.getPreferredSize(this.isEnabled, this.isDiscrete, labelPainter: this._endLabelPainter, textScaleFactor: this.textScaleFactor).width / 2L);
            double innerOverflow__66584 = ((startHalfWidth__65940 + endHalfWidth__66264) + (this.textDirection switch { TextDirection.ltr => (startOffset__65256 - endOffset__65601), TextDirection.rtl => (endOffset__65601 - startOffset__65256), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            this._state.paintTopValueIndicator = (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) => {
if (this.attached)
{
    this._sliderTheme.rangeValueIndicatorShape!.paint(context, topThumbCenter__62971, activationAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._valueIndicatorAnimation), enableAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._enableAnimation), isDiscrete: this.isDiscrete, isOnTop: (thumbDelta__62586 < innerOverflow__66584), labelPainter: topLabelPainter__63167, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, thumb: topThumb__62806, value: topValue__63319, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize__58941);
}
});
        }
        this._sliderTheme.rangeThumbShape!.paint(context, topThumbCenter__62971, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, isOnTop: (thumbDelta__62586 < this.sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete).width), textDirection: this.textDirection, sliderTheme: (((thumbWidth__57746 is not null) && (thumbHeight__57834 is not null)) ? this._sliderTheme.copyWith(thumbSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbWidth__57746), DartRuntimePrimitives.RequireValue(thumbHeight__57834)))) : this._sliderTheme), thumb: topThumb__62806, isPressed: ((object.Equals(topThumb__62806, Thumb.start)) ? startThumbSelected__58757 : endThumbSelected__58850));
    }

    internal virtual global::Doroti.Framework.Semantics.SemanticsConfiguration _createSemanticsConfiguration(double value, double increasedValue, double decreasedValue, global::System.Action increaseAction, global::System.Action decreaseAction, bool focused)
    {
        var config__68936 = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
        config__68936.isEnabled = this.isEnabled;
        ((dynamic)config__68936).textDirection = this.textDirection;
        config__68936.isSlider = true;
        config__68936.isFocusable = true;
        config__68936.isFocused = focused;
        if (this.isEnabled)
        {
            config__68936.onIncrease = (global::System.Action)increaseAction;
            config__68936.onDecrease = (global::System.Action)decreaseAction;
        }
        if ((this.semanticFormatterCallback is not null))
        {
            config__68936.value = this.semanticFormatterCallback!(this._state._lerp(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value))));
            config__68936.increasedValue = this.semanticFormatterCallback!(this._state._lerp(increasedValue));
            config__68936.decreasedValue = this.semanticFormatterCallback!(this._state._lerp(decreasedValue));
        }
        else
        {
            config__68936.value = $"{((DartRuntimePrimitives.RequireValue(value) * 100L)).round()}%";
            config__68936.increasedValue = $"{((increasedValue * 100L)).round()}%";
            config__68936.decreasedValue = $"{((decreasedValue * 100L)).round()}%";
        }
        return config__68936;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(children));
        global::Doroti.Framework.Semantics.SemanticsConfiguration startSemanticsConfiguration__69980 = ((global::Doroti.Framework.Semantics.SemanticsConfiguration)(object?)_createSemanticsConfiguration(((RangeValues)this.values).start, this._increasedStartValue, this._decreasedStartValue, () => this._increaseStartAction(), () => this._decreaseStartAction(), focused: ((_RangeSliderState__range_slider)this._state).startFocusNode.hasFocus));
        global::Doroti.Framework.Semantics.SemanticsConfiguration endSemanticsConfiguration__70260 = ((global::Doroti.Framework.Semantics.SemanticsConfiguration)(object?)_createSemanticsConfiguration(((RangeValues)this.values).end, this._increasedEndValue, this._decreasedEndValue, () => this._increaseEndAction(), () => this._decreaseEndAction(), focused: ((_RangeSliderState__range_slider)this._state).endFocusNode.hasFocus));
        var leftRect__70574 = global::Doroti.Ui.Rect.fromCenter(center: this._startThumbCenter, width: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, height: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension);
        var rightRect__70731 = global::Doroti.Ui.Rect.fromCenter(center: this._endThumbCenter, width: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, height: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension);
        _startSemanticsNode ??= new global::Doroti.Framework.Semantics.SemanticsNode();
        _endSemanticsNode ??= new global::Doroti.Framework.Semantics.SemanticsNode();
        switch (this.textDirection)
        {
            case TextDirection.ltr:
                {
                    this._startSemanticsNode!.rect = leftRect__70574;
                    this._endSemanticsNode!.rect = rightRect__70731;
                    break;
                }
            case TextDirection.rtl:
                {
                    this._startSemanticsNode!.rect = rightRect__70731;
                    this._endSemanticsNode!.rect = leftRect__70574;
                    break;
                }
        }
        this._startSemanticsNode!.updateWith(config: startSemanticsConfiguration__69980);
        this._endSemanticsNode!.updateWith(config: endSemanticsConfiguration__70260);
        var finalChildren__71400 = new List<global::Doroti.Framework.Semantics.SemanticsNode> { this._startSemanticsNode!, this._endSemanticsNode! };
        node.updateWith(config: config, childrenInInversePaintOrder: finalChildren__71400);
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _startSemanticsNode = null;
        _endSemanticsNode = null;
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
    }

    internal virtual double _semanticActionUnit => ((this.divisions is not null) ? (1.0 / DartRuntimePrimitives.RequireValue(this.divisions)) : this._adjustmentUnit);
    internal virtual void _increaseStartAction()
    {
        if (this.isEnabled)
        {
            this.onChanged!(new RangeValues(this._increasedStartValue, ((RangeValues)this.values).end));
        }
    }

    internal virtual void _decreaseStartAction()
    {
        if (this.isEnabled)
        {
            this.onChanged!(new RangeValues(this._decreasedStartValue, ((RangeValues)this.values).end));
        }
    }

    internal virtual void _increaseEndAction()
    {
        if (this.isEnabled)
        {
            this.onChanged!(new RangeValues(((RangeValues)this.values).start, this._increasedEndValue));
        }
    }

    internal virtual void _decreaseEndAction()
    {
        if (this.isEnabled)
        {
            this.onChanged!(new RangeValues(((RangeValues)this.values).start, this._decreasedEndValue));
        }
    }

    internal virtual double _increasedStartValue
    {
        get
        {
            double increasedStartValue__72694 = Dart_coreLibrary.parse(((((RangeValues)this.values).start + this._semanticActionUnit)).toStringAsFixed(2L));
            return ((increasedStartValue__72694 <= (((RangeValues)this.values).end - this._minThumbSeparationValue)) ? increasedStartValue__72694 : ((RangeValues)this.values).start);
            return default!;
        }
    }
    internal virtual double _decreasedStartValue
    {
        get
        {
            return Dart_uiLibrary.clampDouble((((RangeValues)this.values).start - this._semanticActionUnit), 0.0, 1.0);
            return default!;
        }
    }
    internal virtual double _increasedEndValue
    {
        get
        {
            return Dart_uiLibrary.clampDouble((((RangeValues)this.values).end + this._semanticActionUnit), 0.0, 1.0);
            return default!;
        }
    }
    internal virtual double _decreasedEndValue
    {
        get
        {
            double decreasedEndValue__73200 = (((RangeValues)this.values).end - this._semanticActionUnit);
            return ((decreasedEndValue__73200 >= (((RangeValues)this.values).start + this._minThumbSeparationValue)) ? decreasedEndValue__73200 : ((RangeValues)this.values).end);
            return default!;
        }
    }
    public virtual void _scheduleSystemFontsUpdate()
    {
        if (this._hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        this._hasPendingSystemFontsDidChangeCallBack = true;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
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

internal class _ValueIndicatorRenderObjectWidget__range_slider : global::Doroti.Framework.Widgets.LeafRenderObjectWidget
{
    public virtual _RangeSliderState__range_slider state { get; private set; } = default!;

    internal _ValueIndicatorRenderObjectWidget__range_slider(_RangeSliderState__range_slider state)
    {
        this.state = state;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderValueIndicator__range_slider(state: this.state));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderValueIndicator__range_slider)(object)renderObject;
        __renderObject._state = this.state;
    }

}

public class _RenderValueIndicator__range_slider : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RelayoutWhenSystemFontsChangeMixin
{
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual _RangeSliderState__range_slider _state { get; set; } = default!;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderValueIndicator__range_slider(_RangeSliderState__range_slider state)
    {
        this._state = state;
        _valueIndicatorAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: ((_RangeSliderState__range_slider)this._state).valueIndicatorController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
    }

    public override bool sizedByParent => true;
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._scheduleSystemFontsUpdate());
        this._valueIndicatorAnimation.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startPositionController.addListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).endPositionController.addListener(() => this.markNeedsPaint());
    }

    public override void detach()
    {
        this._valueIndicatorAnimation.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).startPositionController.removeListener(() => this.markNeedsPaint());
        ((_RangeSliderState__range_slider)this._state).endPositionController.removeListener(() => this.markNeedsPaint());
        DartRuntimePrimitives.Assert(() => !this._hasPendingSystemFontsDidChangeCallBack);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._scheduleSystemFontsUpdate());
        base.detach();
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        ((_RangeSliderState__range_slider)this._state).paintBottomValueIndicator?.Invoke(context, offset);
        ((_RangeSliderState__range_slider)this._state).paintTopValueIndicator?.Invoke(context, offset);
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
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.scheduleFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
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

internal class _RangeSliderDefaultsM2__range_slider : SliderThemeData
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

    internal _RangeSliderDefaultsM2__range_slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 4)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.24));
    public virtual global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.32));
    public virtual global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.54));
    public virtual global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? overlappingShapeStrokeColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surface);
    public virtual global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Dart_uiLibrary.Color.alphaBlend(this._colors.onSurface.withOpacity(0.38), this._colors.surface));
    public virtual global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.12));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(this.context).textTheme.bodyLarge!.copyWith(color: this._colors.onPrimary);
    public virtual global::Doroti.Ui.Color? valueIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public override RangeSliderTrackShape? rangeTrackShape => DartRuntimePrimitives.ConvertValue<RangeSliderTrackShape>(new RoundedRectRangeSliderTrackShape());
    public override RangeSliderTickMarkShape? rangeTickMarkShape => DartRuntimePrimitives.ConvertValue<RangeSliderTickMarkShape>(new RoundRangeSliderTickMarkShape());
    public override RangeSliderThumbShape? rangeThumbShape => DartRuntimePrimitives.ConvertValue<RangeSliderThumbShape>(new RoundRangeSliderThumbShape());
    public override SliderComponentShape? overlayShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override RangeSliderValueIndicatorShape? rangeValueIndicatorShape => DartRuntimePrimitives.ConvertValue<RangeSliderValueIndicatorShape>(new RectangularRangeSliderValueIndicatorShape());
    public override ShowValueIndicator? showValueIndicator => ShowValueIndicator.onlyForDiscrete;
    public override double? minThumbSeparation => 8;
}

internal class _RangeSliderDefaultsM3__range_slider : SliderThemeData
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

    internal _RangeSliderDefaultsM3__range_slider(global::Doroti.Framework.Widgets.BuildContext context) : base(trackHeight: 16.0)
    {
        this.context = context;
    }

    public virtual global::Doroti.Ui.Color? activeTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? inactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.secondaryContainer);
    public virtual global::Doroti.Ui.Color? disabledActiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? disabledInactiveTrackColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.12));
    public virtual global::Doroti.Ui.Color? activeTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onPrimary.withOpacity(1.0));
    public virtual global::Doroti.Ui.Color? inactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSecondaryContainer.withOpacity(1.0));
    public virtual global::Doroti.Ui.Color? disabledActiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onInverseSurface);
    public virtual global::Doroti.Ui.Color? disabledInactiveTickMarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface);
    public virtual global::Doroti.Ui.Color? thumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary);
    public virtual global::Doroti.Ui.Color? overlappingShapeStrokeColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.surface);
    public virtual global::Doroti.Ui.Color? disabledThumbColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.onSurface.withOpacity(0.38));
    public virtual global::Doroti.Ui.Color? overlayColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.primary.withOpacity(0.12));
    public override global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle => Theme.of(this.context).textTheme.labelLarge!.copyWith(color: this._colors.onInverseSurface);
    public virtual global::Doroti.Ui.Color? valueIndicatorColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(this._colors.inverseSurface);
    public override RangeSliderTrackShape? rangeTrackShape => DartRuntimePrimitives.ConvertValue<RangeSliderTrackShape>(new GappedRangeSliderTrackShape());
    public override RangeSliderTickMarkShape? rangeTickMarkShape => DartRuntimePrimitives.ConvertValue<RangeSliderTickMarkShape>(new RoundRangeSliderTickMarkShape(tickMarkRadius: (4.0 / 2L)));
    public override RangeSliderThumbShape? rangeThumbShape => DartRuntimePrimitives.ConvertValue<RangeSliderThumbShape>(new HandleRangeSliderThumbShape());
    public override SliderComponentShape? overlayShape => DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override RangeSliderValueIndicatorShape? rangeValueIndicatorShape => DartRuntimePrimitives.ConvertValue<RangeSliderValueIndicatorShape>(new RoundedRectRangeSliderValueIndicatorShape());
    public override ShowValueIndicator? showValueIndicator => ShowValueIndicator.onlyForDiscrete;
    public override double? minThumbSeparation => 0;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>? thumbSize
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>?)(object?)WidgetStateProperty.resolveWith((states) => {
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

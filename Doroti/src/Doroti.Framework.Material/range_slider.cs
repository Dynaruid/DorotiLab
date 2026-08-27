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
{
    var __cascade = new global::Doroti.Framework.Widgets.OverlayPortalController(debugLabel: "RangeSlider ValueIndicator");
    __cascade.show();
    return __cascade;
}))();
    internal virtual global::Doroti.Framework.Rendering.LayerLink _layerLink { get; private set; } = new global::Doroti.Framework.Rendering.LayerLink();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _enabled => DartRuntimePrimitives.ConvertValue<bool>((((RangeSlider)this.widget).onChanged is not null));
    internal virtual void _handleHoverChanged(bool hovering)
    {
        if ((hovering != this._hovering))
        {
            setState(((global::System.Action)(() =>
            {
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
        var wasEnabled = (((RangeSlider)oldWidget).onChanged is not null);
        bool isEnabled = this._enabled;
        if ((wasEnabled != isEnabled))
        {
            if (isEnabled)
            {
                this.enableController.forward();
            }
            else
            {
                this.enableController.reverse();
            }
            _showHoverHighlight = (this._hovering && isEnabled);
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
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
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
        RangeValues lerpValues = ((RangeValues)(object?)_lerpRangeValues(values));
        if ((!object.Equals(lerpValues, ((RangeSlider)this.widget).values)))
        {
            ((RangeSlider)this.widget).onChanged!(lerpValues);
        }
    }

    internal virtual void _handleDragStart(RangeValues values)
    {
        setState(((global::System.Action)(() =>
        {
            _dragging = true;
        })));
        ((RangeSlider)this.widget).onChangeStart?.Invoke(_lerpRangeValues(values));
    }

    internal virtual void _handleDragEnd(RangeValues values)
    {
        setState(((global::System.Action)(() =>
        {
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
        double touchRadius = (Math.Max(thumbSize.width, RangeSlider._minTouchTargetWidth) / 2L);
        bool inStartTouchTarget = ((((tapValue - ((RangeValues)values).start)).abs() * trackSize.width) < touchRadius);
        bool inEndTouchTarget = ((((tapValue - ((RangeValues)values).end)).abs() * trackSize.width) < touchRadius);
        if ((inStartTouchTarget && inEndTouchTarget))
        {
            var (towardsStart, towardsEnd) = (textDirection switch { TextDirection.ltr => (((bool, bool))(((dx < 0L), (dx > 0L)))), TextDirection.rtl => (((bool, bool))(((dx > 0L), (dx < 0L)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            if (towardsStart)
            {
                return Thumb.start;
            }
            if (towardsEnd)
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
        ThemeData theme = Theme.of(context);
        SliderThemeData sliderThemeLocal = SliderTheme.of(context);
        bool year2023Local = ((((RangeSlider)this.widget).year2023 ?? sliderThemeLocal.year2023) ?? true);
        SliderThemeData defaults = ((theme.useMaterial3 && !year2023Local) ? new _RangeSliderDefaultsM3__range_slider(context) : new _RangeSliderDefaultsM2__range_slider(context));
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection25438 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (!this._enabled) { __collection25438.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (this._hovering) { __collection25438.Add(global::Doroti.Framework.Widgets.WidgetState.hovered); } if (this._dragging) { __collection25438.Add(global::Doroti.Framework.Widgets.WidgetState.dragged); } return __collection25438; }))();
        RangeSliderValueIndicatorShape valueIndicatorShape = (sliderThemeLocal.rangeValueIndicatorShape ?? defaults.rangeValueIndicatorShape!);
        global::Doroti.Ui.Color valueIndicatorColorLocal = default!;
        if ((valueIndicatorShape is RectangularRangeSliderValueIndicatorShape))
        {
            RectangularRangeSliderValueIndicatorShape valueIndicatorShape__25909__as26060 = (RectangularRangeSliderValueIndicatorShape)valueIndicatorShape;
            valueIndicatorColorLocal = (sliderThemeLocal.valueIndicatorColor ?? Dart_uiLibrary.Color.alphaBlend(theme.colorScheme.onSurface.withOpacity(0.6), theme.colorScheme.surface.withOpacity(0.9)));
        }
        else
        {
            valueIndicatorColorLocal = ((((RangeSlider)this.widget).activeColor ?? sliderThemeLocal.valueIndicatorColor) ?? defaults.valueIndicatorColor!);
        }
        Color? effectiveOverlayColor()
        {
            return ((((((RangeSlider)this.widget).overlayColor?.resolve(states) ?? ((RangeSlider)this.widget).activeColor?.withOpacity(0.12)) ?? (Color)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(sliderThemeLocal.overlayColor, states))) ?? defaults.overlayColor);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        sliderThemeLocal = sliderThemeLocal.copyWith(trackHeight: (sliderThemeLocal.trackHeight ?? defaults.trackHeight), activeTrackColor: ((((RangeSlider)this.widget).activeColor ?? sliderThemeLocal.activeTrackColor) ?? defaults.activeTrackColor), inactiveTrackColor: ((((RangeSlider)this.widget).inactiveColor ?? sliderThemeLocal.inactiveTrackColor) ?? defaults.inactiveTrackColor), disabledActiveTrackColor: (sliderThemeLocal.disabledActiveTrackColor ?? defaults.disabledActiveTrackColor), disabledInactiveTrackColor: (sliderThemeLocal.disabledInactiveTrackColor ?? defaults.disabledInactiveTrackColor), activeTickMarkColor: ((((RangeSlider)this.widget).inactiveColor ?? sliderThemeLocal.activeTickMarkColor) ?? defaults.activeTickMarkColor), inactiveTickMarkColor: ((((RangeSlider)this.widget).activeColor ?? sliderThemeLocal.inactiveTickMarkColor) ?? defaults.inactiveTickMarkColor), disabledActiveTickMarkColor: (sliderThemeLocal.disabledActiveTickMarkColor ?? defaults.disabledActiveTickMarkColor), disabledInactiveTickMarkColor: (sliderThemeLocal.disabledInactiveTickMarkColor ?? defaults.disabledInactiveTickMarkColor), thumbColor: ((((RangeSlider)this.widget).activeColor ?? sliderThemeLocal.thumbColor) ?? defaults.thumbColor), overlappingShapeStrokeColor: (sliderThemeLocal.overlappingShapeStrokeColor ?? defaults.overlappingShapeStrokeColor), disabledThumbColor: (sliderThemeLocal.disabledThumbColor ?? defaults.disabledThumbColor), overlayColor: effectiveOverlayColor(), valueIndicatorColor: valueIndicatorColorLocal, rangeTrackShape: (sliderThemeLocal.rangeTrackShape ?? defaults.rangeTrackShape), rangeTickMarkShape: (sliderThemeLocal.rangeTickMarkShape ?? defaults.rangeTickMarkShape), rangeThumbShape: (sliderThemeLocal.rangeThumbShape ?? defaults.rangeThumbShape), overlayShape: (sliderThemeLocal.overlayShape ?? defaults.overlayShape), rangeValueIndicatorShape: valueIndicatorShape, showValueIndicator: (sliderThemeLocal.showValueIndicator ?? defaults.showValueIndicator), valueIndicatorTextStyle: (sliderThemeLocal.valueIndicatorTextStyle ?? defaults.valueIndicatorTextStyle), minThumbSeparation: (sliderThemeLocal.minThumbSeparation ?? defaults.minThumbSeparation), thumbSelector: ((sliderThemeLocal.thumbSelector ?? (global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>)this._defaultRangeThumbSelector)), padding: (((RangeSlider)this.widget).padding ?? sliderThemeLocal.padding), thumbSize: (sliderThemeLocal.thumbSize ?? defaults.thumbSize), trackGap: (sliderThemeLocal.trackGap ?? defaults.trackGap));
        global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = ((((((RangeSlider)this.widget).mouseCursor?.resolve(states) ?? (global::Doroti.Framework.Services.MouseCursor)sliderThemeLocal.mouseCursor?.resolve(states))) ?? (global::Doroti.Framework.Services.MouseCursor)global::Doroti.Framework.Widgets.WidgetStateMouseCursor.clickable.resolve(states)));
        Size screenSize()
        {
            return MediaQuery.sizeOf(context);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double fontSizeLocal = (sliderThemeLocal.valueIndicatorTextStyle?.fontSize ?? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double fontSizeToScale = ((fontSizeLocal == 0.0) ? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize : fontSizeLocal);
        double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale);
        global::Doroti.Framework.Widgets.Widget result = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CompositedTransformTarget(link: this._layerLink, child: new global::Doroti.Framework.Widgets.OverlayPortal(controller: this._valueIndicatorOverlayPortalController, overlayChildBuilder: ((context) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildValueIndicator(DartRuntimePrimitives.RequireValue(sliderThemeLocal.showValueIndicator)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), child: new _RangeSliderRenderObjectWidget__range_slider(values: _unlerpRangeValues(((RangeSlider)this.widget).values), divisions: ((RangeSlider)this.widget).divisions, labels: ((RangeSlider)this.widget).labels, sliderTheme: sliderThemeLocal, textScaleFactor: effectiveTextScale, screenSize: screenSize(), onChanged: ((global::System.Action<RangeValues>)((this._enabled && ((((RangeSlider)this.widget).max > ((RangeSlider)this.widget).min))) ? this._handleChanged : null)), onChangeStart: (global::System.Action<RangeValues>)this._handleDragStart, onChangeEnd: (global::System.Action<RangeValues>)this._handleDragEnd, state: this, semanticFormatterCallback: (SemanticFormatterCallback?)((RangeSlider)this.widget).semanticFormatterCallback, hovering: this._showHoverHighlight))));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? paddingLocal = (((RangeSlider)this.widget).padding ?? sliderThemeLocal.padding);
        if ((paddingLocal is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: result));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Focus(focusNode: this.startFocusNode, includeSemantics: false, child: global::Doroti.Framework.Widgets.SizedBox.CreateShrink())), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Focus(focusNode: this.endFocusNode, includeSemantics: false, child: global::Doroti.Framework.Widgets.SizedBox.CreateShrink())) })), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.MouseRegion(onEnter: ((global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)((_) => { _handleHoverChanged(true); })), onExit: ((global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)((_) => { _handleHoverChanged(false); })), cursor: effectiveMouseCursor, child: result)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildValueIndicator(ShowValueIndicator showValueIndicator)
    {
        global::Doroti.Framework.Widgets.Widget valueIndicator = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CompositedTransformFollower(link: this._layerLink, child: new _ValueIndicatorRenderObjectWidget__range_slider(state: this)));
        return (showValueIndicator switch { var __constant32003 when (object.Equals(__constant32003, ShowValueIndicator.never)) => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __constant32062 when (object.Equals(__constant32062, ShowValueIndicator.onlyForDiscrete)) => ((((RangeSlider)this.widget).divisions is not null) ? valueIndicator : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __constant32183 when (object.Equals(__constant32183, ShowValueIndicator.onlyForContinuous)) => ((((RangeSlider)this.widget).divisions is null) ? valueIndicator : global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), var __logical32306 when ((object.Equals(__logical32306, ShowValueIndicator.alwaysVisible) || object.Equals(__logical32306, ShowValueIndicator.always))) => valueIndicator, var __constant32383 when (object.Equals(__constant32383, ShowValueIndicator.onDrag)) => valueIndicator, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
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
{
    var __cascade = __renderObject;
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
    return __cascade;
}))());
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
        var teamLocal = new global::Doroti.Framework.Gestures.GestureArenaTeam();
        _drag = ((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer();
    __cascade.team = teamLocal;
    __cascade.onStart = this._handleDragStart;
    __cascade.onUpdate = this._handleDragUpdate;
    __cascade.onEnd = this._handleDragEnd;
    __cascade.onCancel = this._handleDragCancel;
    __cascade.gestureSettings = gestureSettings;
    return __cascade;
}))();
        _tap = ((Func<global::Doroti.Framework.Gestures.TapGestureRecognizer>)(() =>
{
    var __cascade = new global::Doroti.Framework.Gestures.TapGestureRecognizer();
    __cascade.team = teamLocal;
    __cascade.onTapDown = this._handleTapDown;
    __cascade.onTapUp = this._handleTapUp;
    __cascade.gestureSettings = gestureSettings;
    return __cascade;
}))();
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
            RangeValues convertedValues = (this.isDiscrete ? _discretizeRangeValues(newValues) : newValues);
            if ((object.Equals(convertedValues, this._values)))
            {
                return;
            }
            _values = convertedValues;
            if (this.isDiscrete)
            {
                double startDistance = ((((RangeValues)this._values).start - ((_RangeSliderState__range_slider)this._state).startPositionController.value)).abs();
                ((_RangeSliderState__range_slider)this._state).startPositionController.duration = ((startDistance != 0.0) ? (_positionAnimationDuration * ((1.0 / startDistance))) : Duration.zero);
                ((_RangeSliderState__range_slider)this._state).startPositionController.animateTo(((RangeValues)this._values).start, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
                double endDistance = ((((RangeValues)this._values).end - ((_RangeSliderState__range_slider)this._state).endPositionController.value)).abs();
                ((_RangeSliderState__range_slider)this._state).endPositionController.duration = ((endDistance != 0.0) ? (_positionAnimationDuration * ((1.0 / endDistance))) : Duration.zero);
                ((_RangeSliderState__range_slider)this._state).endPositionController.animateTo(((RangeValues)this._values).end, curve: global::Doroti.Framework.Animation.Curves.easeInOut);
            }
            else
            {
                ((_RangeSliderState__range_slider)this._state).startPositionController.value = ((RangeValues)convertedValues).start;
                ((_RangeSliderState__range_slider)this._state).endPositionController.value = ((RangeValues)convertedValues).end;
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
            bool wasEnabled = this.isEnabled;
            _onChanged = (global::System.Action<RangeValues>)__value;
            if ((wasEnabled != this.isEnabled))
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
        RangeLabels? labelsLocal = this.labels;
        if ((labelsLocal is null))
        {
            return;
        }
        var (textLocal, labelPainter) = (thumb switch { var __constant46482 when (object.Equals(__constant46482, Thumb.start)) => (((string, global::Doroti.Framework.Painting.TextPainter))((((RangeLabels)labelsLocal).start, this._startLabelPainter))), var __constant46539 when (object.Equals(__constant46539, Thumb.end)) => (((string, global::Doroti.Framework.Painting.TextPainter))((((RangeLabels)labelsLocal).end, this._endLabelPainter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = labelPainter;
    __cascade.text = new global::Doroti.Framework.Painting.TextSpan(style: this._sliderTheme.valueIndicatorTextStyle, text: textLocal);
    __cascade.textDirection = this.textDirection;
    __cascade.textScaleFactor = this.textScaleFactor;
    __cascade.layout();
    return __cascade;
}))());
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
        double visualPosition = (((globalToLocal(globalPosition).dx - this._trackRect.left)) / this._trackRect.width);
        return _getValueFromVisualPosition(visualPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _discretize(double value)
    {
        double result = Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), 0.0, 1.0);
        if (this.isDiscrete)
        {
            result = (((result * DartRuntimePrimitives.RequireValue(this.divisions))).round() / DartRuntimePrimitives.RequireValue(this.divisions));
        }
        return result;
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
        double tapValue = Dart_uiLibrary.clampDouble(_getValueFromGlobalPosition(globalPosition), 0.0, 1.0);
        _lastThumbSelection = this.sliderTheme.thumbSelector!(this.textDirection, this.values, tapValue, this._thumbSize, this.size, 0);
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
            RangeValues currentValues = ((RangeValues)(object?)_discretizeRangeValues(this.values));
            _newValues = (this._lastThumbSelection! switch { var __constant50438 when (object.Equals(__constant50438, Thumb.start)) => new RangeValues(tapValue, ((RangeValues)currentValues).end), var __constant50503 when (object.Equals(__constant50503, Thumb.end)) => new RangeValues(((RangeValues)currentValues).start, tapValue), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            _updateLabelPainter(DartRuntimePrimitives.RequireValue(this._lastThumbSelection));
            this.onChangeStart?.Invoke(currentValues);
            this.onChanged!(_discretizeRangeValues(this._newValues));
            ((_RangeSliderState__range_slider)this._state).overlayController.forward();
            if (this.shouldShowValueIndicatorWhenDragged)
            {
                ((_RangeSliderState__range_slider)this._state).valueIndicatorController.forward();
                ((_RangeSliderState__range_slider)this._state).interactionTimer?.cancel();
                this._state.interactionTimer = new Timer((_minimumInteractionTime * global::Doroti.Framework.Scheduler.BindingLibrary.timeDilation), (() =>
                {
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
        double dragValue = _getValueFromGlobalPosition(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition);
        var shouldCallOnChangeStart = false;
        if ((this._lastThumbSelection is null))
        {
            _lastThumbSelection = this.sliderTheme.thumbSelector!(this.textDirection, this.values, dragValue, this._thumbSize, this.size, ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta.dx);
            if ((this._lastThumbSelection is not null))
            {
                shouldCallOnChangeStart = true;
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
            RangeValues currentValues = ((RangeValues)(object?)_discretizeRangeValues(this.values));
            if (((this.onChangeStart is not null) && shouldCallOnChangeStart))
            {
                this.onChangeStart!(currentValues);
            }
            double currentDragValue = _discretize(dragValue);
            _newValues = (this._lastThumbSelection! switch { var __constant52496 when (object.Equals(__constant52496, Thumb.start)) => new RangeValues(Math.Min(currentDragValue, (((RangeValues)currentValues).end - this._minThumbSeparationValue)), ((RangeValues)currentValues).end), var __constant52656 when (object.Equals(__constant52656, Thumb.end)) => new RangeValues(((RangeValues)currentValues).start, Math.Max(currentDragValue, (((RangeValues)currentValues).start + this._minThumbSeparationValue))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
            RangeValues discreteValues = ((RangeValues)(object?)_discretizeRangeValues(this._newValues));
            this.onChangeEnd?.Invoke(discreteValues);
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
        double startValue = ((_RangeSliderState__range_slider)this._state).startPositionController.value;
        double endValue = ((_RangeSliderState__range_slider)this._state).endPositionController.value;
        var (startVisualPosition, endVisualPosition) = (this.textDirection switch { TextDirection.rtl => (((double, double))(((1.0 - startValue), (1.0 - endValue)))), TextDirection.ltr => (((double, double))((startValue, endValue))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)this._sliderTheme.rangeTrackShape!.getPreferredRect(parentBox: this, offset: offset, sliderTheme: this._sliderTheme, isDiscrete: this.isDiscrete));
        double padding = (this._sliderTheme.rangeTrackShape!.isRounded ? trackRect.height : 0.0);
        double thumbYOffset = ((Offset)((dynamic)trackRect).center).dy;
        double startThumbPosition = (this.isDiscrete ? ((trackRect.left + (startVisualPosition * ((trackRect.width - padding)))) + (padding / 2L)) : (trackRect.left + (startVisualPosition * trackRect.width)));
        double endThumbPosition = (this.isDiscrete ? ((trackRect.left + (endVisualPosition * ((trackRect.width - padding)))) + (padding / 2L)) : (trackRect.left + (endVisualPosition * trackRect.width)));
        global::Doroti.Ui.Size thumbPreferredSize = ((global::Doroti.Ui.Size)(object?)this._sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete));
        double thumbPadding = (((padding > (thumbPreferredSize.width / 2L)) ? (padding / 2L) : 0));
        _startThumbCenter = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(startThumbPosition, (trackRect.left + thumbPadding), (trackRect.right - thumbPadding)), thumbYOffset);
        _endThumbCenter = new global::Doroti.Ui.Offset(Dart_uiLibrary.clampDouble(endThumbPosition, (trackRect.left + thumbPadding), (trackRect.right - thumbPadding)), thumbYOffset);
        if (this.isEnabled)
        {
            global::Doroti.Ui.Size overlaySize = ((global::Doroti.Ui.Size)(object?)this.sliderTheme.overlayShape!.getPreferredSize(this.isEnabled, false));
            overlayStartRect = global::Doroti.Ui.Rect.fromCircle(center: this._startThumbCenter, radius: (overlaySize.width / 2.0));
            overlayEndRect = global::Doroti.Ui.Rect.fromCircle(center: this._endThumbCenter, radius: (overlaySize.width / 2.0));
        }
        double? thumbWidth = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.width;
        double? thumbHeight = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())?.height;
        double? trackGapLocal = this._sliderTheme.trackGap;
        double? pressedThumbWidth = this._sliderTheme.thumbSize?.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.pressed })?.width;
        double delta = default!;
        if ((((this._active && (thumbWidth is not null)) && (pressedThumbWidth is not null)) && (trackGapLocal is not null)))
        {
            double thumbWidth__57746__value58122 = DartRuntimePrimitives.RequireValue(thumbWidth);
            double pressedThumbWidth__57970__value58144 = DartRuntimePrimitives.RequireValue(pressedThumbWidth);
            double trackGap__57918__value58173 = DartRuntimePrimitives.RequireValue(trackGapLocal);
            delta = (DartRuntimePrimitives.RequireValue(thumbWidth__57746__value58122) - DartRuntimePrimitives.RequireValue(pressedThumbWidth__57970__value58144));
            thumbWidth = DartRuntimePrimitives.RequireValue(pressedThumbWidth__57970__value58144);
            if ((DartRuntimePrimitives.RequireValue(trackGap__57918__value58173) > 0.0))
            {
                trackGapLocal = (DartRuntimePrimitives.RequireValue(trackGap__57918__value58173) - (delta / 2L));
            }
        }
        this._sliderTheme.rangeTrackShape!.paint(context, offset, parentBox: this, sliderTheme: this._sliderTheme.copyWith(trackGap: trackGapLocal), enableAnimation: this._enableAnimation, textDirection: this._textDirection, startThumbCenter: this._startThumbCenter, endThumbCenter: this._endThumbCenter, isDiscrete: this.isDiscrete, isEnabled: this.isEnabled);
        bool startThumbSelected = ((object.Equals(this._lastThumbSelection, Thumb.start)) && !this.hoveringEndThumb);
        bool endThumbSelected = ((object.Equals(this._lastThumbSelection, Thumb.end)) && !this.hoveringStartThumb);
        global::Doroti.Ui.Size resolvedscreenSize = ((global::Doroti.Ui.Size)(object?)(this.screenSize.isEmpty ? this.size : this.screenSize));
        if (((_RangeSliderState__range_slider)this._state).startFocusNode.hasFocus)
        {
            this._sliderTheme.overlayShape!.paint(context, this._startThumbCenter, activationAnimation: new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0), enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._startLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: startValue, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize);
        }
        if (((_RangeSliderState__range_slider)this._state).endFocusNode.hasFocus)
        {
            this._sliderTheme.overlayShape!.paint(context, this._endThumbCenter, activationAnimation: new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1.0), enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._endLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: endValue, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize);
        }
        if (!this._overlayAnimation.isDismissed)
        {
            if ((startThumbSelected || this.hoveringStartThumb))
            {
                this._sliderTheme.overlayShape!.paint(context, this._startThumbCenter, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._startLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: startValue, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize);
            }
            if ((endThumbSelected || this.hoveringEndThumb))
            {
                this._sliderTheme.overlayShape!.paint(context, this._endThumbCenter, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, labelPainter: this._endLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, value: endValue, textScaleFactor: this._textScaleFactor, sizeWithOverflow: resolvedscreenSize);
            }
        }
        if (this.isDiscrete)
        {
            double tickMarkWidth = this._sliderTheme.rangeTickMarkShape!.getPreferredSize(isEnabled: this.isEnabled, sliderTheme: this._sliderTheme).width;
            double discreteTrackPadding = trackRect.height;
            double adjustedTrackWidth = (trackRect.width - discreteTrackPadding);
            if (((adjustedTrackWidth / DartRuntimePrimitives.RequireValue(this.divisions)) >= (3.0 * tickMarkWidth)))
            {
                double dyLocal = ((Offset)((dynamic)trackRect).center).dy;
                for (var i = 0L; (i <= DartRuntimePrimitives.RequireValue(this.divisions)); i++)
                {
                    double valueLocal = (i / DartRuntimePrimitives.RequireValue(this.divisions));
                    double dxLocal = ((trackRect.left + (DartRuntimePrimitives.RequireValue(valueLocal) * adjustedTrackWidth)) + (discreteTrackPadding / 2L));
                    var tickMarkOffset = new global::Doroti.Ui.Offset(dxLocal, dyLocal);
                    this._sliderTheme.rangeTickMarkShape!.paint(context, tickMarkOffset, parentBox: this, sliderTheme: this._sliderTheme, enableAnimation: this._enableAnimation, textDirection: this._textDirection, startThumbCenter: this._startThumbCenter, endThumbCenter: this._endThumbCenter, isEnabled: this.isEnabled);
                }
            }
        }
        double thumbDelta = ((this._endThumbCenter.dx - this._startThumbCenter.dx)).abs();
        var isLastThumbStart = (object.Equals(this._lastThumbSelection, Thumb.start));
        Thumb bottomThumb = (isLastThumbStart ? Thumb.end : Thumb.start);
        Thumb topThumb = (isLastThumbStart ? Thumb.start : Thumb.end);
        global::Doroti.Ui.Offset bottomThumbCenter = ((global::Doroti.Ui.Offset)(object?)(isLastThumbStart ? this._endThumbCenter : this._startThumbCenter));
        global::Doroti.Ui.Offset topThumbCenter = ((global::Doroti.Ui.Offset)(object?)(isLastThumbStart ? this._startThumbCenter : this._endThumbCenter));
        global::Doroti.Framework.Painting.TextPainter bottomLabelPainter = (isLastThumbStart ? this._endLabelPainter : this._startLabelPainter);
        global::Doroti.Framework.Painting.TextPainter topLabelPainter = (isLastThumbStart ? this._startLabelPainter : this._endLabelPainter);
        var bottomValue = (isLastThumbStart ? endValue : startValue);
        var topValue = (isLastThumbStart ? startValue : endValue);
        bool shouldPaintValueIndicators = ((this.isEnabled && (this.labels is not null)) && ((((this.shouldShowValueIndicatorWhenDragged && !this._valueIndicatorAnimation.isDismissed)) || this.shouldAlwaysShowValueIndicator)));
        if (shouldPaintValueIndicators)
        {
            this._state.paintBottomValueIndicator = (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) =>
            {
                if (this.attached)
                {
                    this._sliderTheme.rangeValueIndicatorShape!.paint(context, bottomThumbCenter, activationAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._valueIndicatorAnimation), enableAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._enableAnimation), isDiscrete: this.isDiscrete, isOnTop: false, labelPainter: bottomLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, thumb: bottomThumb, value: bottomValue, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize);
                }
            });
        }
        this._sliderTheme.rangeThumbShape!.paint(context, bottomThumbCenter, activationAnimation: this._valueIndicatorAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, isOnTop: false, textDirection: this.textDirection, sliderTheme: (((thumbWidth is not null) && (thumbHeight is not null)) ? this._sliderTheme.copyWith(thumbSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbWidth), DartRuntimePrimitives.RequireValue(thumbHeight)))) : this._sliderTheme), thumb: bottomThumb, isPressed: ((object.Equals(bottomThumb, Thumb.start)) ? startThumbSelected : endThumbSelected));
        if (shouldPaintValueIndicators)
        {
            double startOffset = this.sliderTheme.rangeValueIndicatorShape!.getHorizontalShift(parentBox: this, center: this._startThumbCenter, labelPainter: this._startLabelPainter, activationAnimation: this._valueIndicatorAnimation, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize);
            double endOffset = this.sliderTheme.rangeValueIndicatorShape!.getHorizontalShift(parentBox: this, center: this._endThumbCenter, labelPainter: this._endLabelPainter, activationAnimation: this._valueIndicatorAnimation, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize);
            double startHalfWidth = (this.sliderTheme.rangeValueIndicatorShape!.getPreferredSize(this.isEnabled, this.isDiscrete, labelPainter: this._startLabelPainter, textScaleFactor: this.textScaleFactor).width / 2L);
            double endHalfWidth = (this.sliderTheme.rangeValueIndicatorShape!.getPreferredSize(this.isEnabled, this.isDiscrete, labelPainter: this._endLabelPainter, textScaleFactor: this.textScaleFactor).width / 2L);
            double innerOverflow = ((startHalfWidth + endHalfWidth) + (this.textDirection switch { TextDirection.ltr => (startOffset - endOffset), TextDirection.rtl => (endOffset - startOffset), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            this._state.paintTopValueIndicator = (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) =>
            {
                if (this.attached)
                {
                    this._sliderTheme.rangeValueIndicatorShape!.paint(context, topThumbCenter, activationAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._valueIndicatorAnimation), enableAnimation: (this.shouldAlwaysShowValueIndicator ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(1) : this._enableAnimation), isDiscrete: this.isDiscrete, isOnTop: (thumbDelta < innerOverflow), labelPainter: topLabelPainter, parentBox: this, sliderTheme: this._sliderTheme, textDirection: this._textDirection, thumb: topThumb, value: topValue, textScaleFactor: this.textScaleFactor, sizeWithOverflow: resolvedscreenSize);
                }
            });
        }
        this._sliderTheme.rangeThumbShape!.paint(context, topThumbCenter, activationAnimation: this._overlayAnimation, enableAnimation: this._enableAnimation, isDiscrete: this.isDiscrete, isOnTop: (thumbDelta < this.sliderTheme.rangeThumbShape!.getPreferredSize(this.isEnabled, this.isDiscrete).width), textDirection: this.textDirection, sliderTheme: (((thumbWidth is not null) && (thumbHeight is not null)) ? this._sliderTheme.copyWith(thumbSize: new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Size?>(new global::Doroti.Ui.Size(DartRuntimePrimitives.RequireValue(thumbWidth), DartRuntimePrimitives.RequireValue(thumbHeight)))) : this._sliderTheme), thumb: topThumb, isPressed: ((object.Equals(topThumb, Thumb.start)) ? startThumbSelected : endThumbSelected));
    }

    internal virtual global::Doroti.Framework.Semantics.SemanticsConfiguration _createSemanticsConfiguration(double value, double increasedValue, double decreasedValue, global::System.Action increaseAction, global::System.Action decreaseAction, bool focused)
    {
        var config = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
        config.isEnabled = this.isEnabled;
        ((dynamic)config).textDirection = this.textDirection;
        config.isSlider = true;
        config.isFocusable = true;
        config.isFocused = focused;
        if (this.isEnabled)
        {
            config.onIncrease = (global::System.Action)increaseAction;
            config.onDecrease = (global::System.Action)decreaseAction;
        }
        if ((this.semanticFormatterCallback is not null))
        {
            config.value = this.semanticFormatterCallback!(this._state._lerp(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value))));
            config.increasedValue = this.semanticFormatterCallback!(this._state._lerp(increasedValue));
            config.decreasedValue = this.semanticFormatterCallback!(this._state._lerp(decreasedValue));
        }
        else
        {
            config.value = $"{((DartRuntimePrimitives.RequireValue(value) * 100L)).round()}%";
            config.increasedValue = $"{((increasedValue * 100L)).round()}%";
            config.decreasedValue = $"{((decreasedValue * 100L)).round()}%";
        }
        return config;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(children));
        global::Doroti.Framework.Semantics.SemanticsConfiguration startSemanticsConfiguration = ((global::Doroti.Framework.Semantics.SemanticsConfiguration)(object?)_createSemanticsConfiguration(((RangeValues)this.values).start, this._increasedStartValue, this._decreasedStartValue, () => this._increaseStartAction(), () => this._decreaseStartAction(), focused: ((_RangeSliderState__range_slider)this._state).startFocusNode.hasFocus));
        global::Doroti.Framework.Semantics.SemanticsConfiguration endSemanticsConfiguration = ((global::Doroti.Framework.Semantics.SemanticsConfiguration)(object?)_createSemanticsConfiguration(((RangeValues)this.values).end, this._increasedEndValue, this._decreasedEndValue, () => this._increaseEndAction(), () => this._decreaseEndAction(), focused: ((_RangeSliderState__range_slider)this._state).endFocusNode.hasFocus));
        var leftRect = global::Doroti.Ui.Rect.fromCenter(center: this._startThumbCenter, width: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, height: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension);
        var rightRect = global::Doroti.Ui.Rect.fromCenter(center: this._endThumbCenter, width: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension, height: global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension);
        _startSemanticsNode ??= new global::Doroti.Framework.Semantics.SemanticsNode();
        _endSemanticsNode ??= new global::Doroti.Framework.Semantics.SemanticsNode();
        switch (this.textDirection)
        {
            case TextDirection.ltr:
                {
                    this._startSemanticsNode!.rect = leftRect;
                    this._endSemanticsNode!.rect = rightRect;
                    break;
                }
            case TextDirection.rtl:
                {
                    this._startSemanticsNode!.rect = rightRect;
                    this._endSemanticsNode!.rect = leftRect;
                    break;
                }
        }
        this._startSemanticsNode!.updateWith(config: startSemanticsConfiguration);
        this._endSemanticsNode!.updateWith(config: endSemanticsConfiguration);
        var finalChildren = new List<global::Doroti.Framework.Semantics.SemanticsNode> { this._startSemanticsNode!, this._endSemanticsNode! };
        node.updateWith(config: config, childrenInInversePaintOrder: finalChildren);
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
            double increasedStartValue = Dart_coreLibrary.parse(((((RangeValues)this.values).start + this._semanticActionUnit)).toStringAsFixed(2L));
            return ((increasedStartValue <= (((RangeValues)this.values).end - this._minThumbSeparationValue)) ? increasedStartValue : ((RangeValues)this.values).start);
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
            double decreasedEndValue = (((RangeValues)this.values).end - this._semanticActionUnit);
            return ((decreasedEndValue >= (((RangeValues)this.values).start + this._minThumbSeparationValue)) ? decreasedEndValue : ((RangeValues)this.values).end);
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

// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/range_slider.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public delegate void PaintRangeValueIndicator(PaintingContext context, Offset offset);

public class RangeSlider : StatefulWidget
{
    public virtual RangeValues values { get; private set; } = default!;
    public virtual Action<RangeValues>? onChanged { get; private set; }
    public virtual Action<RangeValues>? onChangeStart { get; private set; }
    public virtual Action<RangeValues>? onChangeEnd { get; private set; }
    public virtual double min { get; private set; } = default!;
    public virtual double max { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual RangeLabels? labels { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color? inactiveColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual SemanticFormatterCallback? semanticFormatterCallback { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual bool? year2023 { get; private set; }
    internal static double _minTouchTargetWidth = Widgets.ConstantsLibrary.kMinInteractiveDimension;

    public RangeSlider(
        Key? key = null,
        RangeValues values = default!,
        Action<RangeValues>? onChanged = default!,
        Action<RangeValues>? onChangeStart = null,
        Action<RangeValues>? onChangeEnd = null,
        double min = 0.0,
        double max = 1.0,
        long? divisions = null,
        RangeLabels? labels = null,
        Color? activeColor = null,
        Color? inactiveColor = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        SemanticFormatterCallback? semanticFormatterCallback = null,
        EdgeInsetsGeometry? padding = null,
        bool? year2023 = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(min <= max);
        System.Diagnostics.Debug.Assert(values.start <= values.end);
        System.Diagnostics.Debug.Assert((values.start >= min) && (values.start <= max));
        System.Diagnostics.Debug.Assert((values.end >= min) && (values.end <= max));
        System.Diagnostics.Debug.Assert(
            (divisions is null)
                || (
                    (
                        divisions
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RangeSliderState__range_slider());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("valueStart", values.start));
        properties.add(new DoubleProperty("valueEnd", values.end));
        properties.add(
            new ObjectFlagProperty<Action<RangeValues>>("onChanged", onChanged, ifNull: "disabled")
        );
        properties.add(
            ObjectFlagProperty<Action<RangeValues>>.CreateHas("onChangeStart", onChangeStart)
        );
        properties.add(
            ObjectFlagProperty<Action<RangeValues>>.CreateHas("onChangeEnd", onChangeEnd)
        );
        properties.add(new DoubleProperty("min", min));
        properties.add(new DoubleProperty("max", max));
        properties.add(new IntProperty("divisions", divisions));
        properties.add(new StringProperty("labelStart", labels?.start));
        properties.add(new StringProperty("labelEnd", labels?.end));
        properties.add(new ColorProperty("activeColor", activeColor));
        properties.add(new ColorProperty("inactiveColor", inactiveColor));
        properties.add(
            ObjectFlagProperty<SemanticFormatterCallback>.CreateHas(
                "semanticFormatterCallback",
                semanticFormatterCallback
            )
        );
    }
}

public class _RangeSliderState__range_slider
    : State<RangeSlider>,
        TickerProviderStateMixin<RangeSlider>
{
    public static Duration enableAnimationDuration = Duration.Create(milliseconds: 75L);
    public static Duration valueIndicatorAnimationDuration = Duration.Create(milliseconds: 100L);
    public virtual FocusNode startFocusNode { get; private set; } = new FocusNode();
    public virtual FocusNode endFocusNode { get; private set; } = new FocusNode();
    public virtual AnimationController overlayController { get; set; } = default!;
    public virtual AnimationController valueIndicatorController { get; set; } = default!;
    public virtual AnimationController enableController { get; set; } = default!;
    public virtual AnimationController startPositionController { get; set; } = default!;
    public virtual AnimationController endPositionController { get; set; } = default!;
    public virtual Timer? interactionTimer { get; set; } = default;
    public virtual Action<PaintingContext, Offset>? paintTopValueIndicator { get; set; } = default;
    public virtual Action<PaintingContext, Offset>? paintBottomValueIndicator { get; set; } =
        default;
    internal virtual bool _dragging { get; set; } = false;
    internal virtual bool _hovering { get; set; } = false;
    internal virtual bool _showHoverHighlight { get; set; } = false;
    internal virtual OverlayPortalController _valueIndicatorOverlayPortalController
    {
        get;
        private set;
    } =
        (
            (Func<OverlayPortalController>)(
                () =>
                {
                    var __cascade = new OverlayPortalController(
                        debugLabel: "RangeSlider ValueIndicator"
                    );
                    __cascade.show();
                    return __cascade;
                }
            )
        )();
    internal virtual LayerLink _layerLink { get; private set; } = new LayerLink();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _enabled =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.onChanged is not null);

    internal virtual void _handleHoverChanged(bool hovering)
    {
        if (hovering != _hovering)
        {
            setState(() =>
            {
                _hovering = hovering;
                _showHoverHighlight = hovering && _enabled;
            });
        }
    }

    public override void initState()
    {
        base.initState();
        overlayController = new AnimationController(
            duration: ConstantsLibrary.kRadialReactionDuration,
            vsync: this
        );
        valueIndicatorController = new AnimationController(
            duration: valueIndicatorAnimationDuration,
            vsync: this
        );
        enableController = new AnimationController(
            duration: enableAnimationDuration,
            vsync: this,
            value: _enabled ? 1.0 : 0.0
        );
        startPositionController = new AnimationController(
            duration: Duration.zero,
            vsync: this,
            value: _unlerp(widget.values.start)
        );
        endPositionController = new AnimationController(
            duration: Duration.zero,
            vsync: this,
            value: _unlerp(widget.values.end)
        );
    }

    public override void didUpdateWidget(RangeSlider oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (Equals(oldWidget.onChanged, widget.onChanged))
        {
            return;
        }
        var wasEnabled = oldWidget.onChanged is not null;
        bool isEnabled = _enabled;
        if (wasEnabled != isEnabled)
        {
            if (isEnabled)
            {
                enableController.forward();
            }
            else
            {
                enableController.reverse();
            }
            _showHoverHighlight = _hovering && isEnabled;
        }
    }

    public override void dispose()
    {
        interactionTimer?.cancel();
        overlayController.dispose();
        valueIndicatorController.dispose();
        enableController.dispose();
        startPositionController.dispose();
        endPositionController.dispose();
        startFocusNode.dispose();
        endFocusNode.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleChanged(RangeValues values)
    {
        DartRuntimePrimitives.Assert(() => _enabled);
        RangeValues lerpValues = _lerpRangeValues(values);
        if (!Equals(lerpValues, widget.values))
        {
            widget.onChanged!(lerpValues);
        }
    }

    internal virtual void _handleDragStart(RangeValues values)
    {
        setState(() =>
        {
            _dragging = true;
        });
        widget.onChangeStart?.Invoke(_lerpRangeValues(values));
    }

    internal virtual void _handleDragEnd(RangeValues values)
    {
        setState(() =>
        {
            _dragging = false;
        });
        widget.onChangeEnd?.Invoke(_lerpRangeValues(values));
    }

    internal virtual double _lerp(double value) =>
        DartRuntimePrimitives.ConvertValue<double>(
            (
                DorotiUiLibrary.lerpDouble(widget.min, widget.max, value)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );

    internal virtual RangeValues _lerpRangeValues(RangeValues values)
    {
        return new RangeValues(_lerp(values.start), _lerp(values.end));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _unlerp(double value)
    {
        DartRuntimePrimitives.Assert(() => value <= widget.max);
        DartRuntimePrimitives.Assert(() => value >= widget.min);
        return (widget.max > widget.min) ? ((value - widget.min) / (widget.max - widget.min)) : 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual RangeValues _unlerpRangeValues(RangeValues values)
    {
        return new RangeValues(_unlerp(values.start), _unlerp(values.end));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Thumb? _defaultRangeThumbSelector(
        TextDirection textDirection,
        RangeValues values,
        double tapValue,
        Size thumbSize,
        Size trackSize,
        double dx
    )
    {
        double touchRadius = Math.Max(thumbSize.width, RangeSlider._minTouchTargetWidth) / 2L;
        bool inStartTouchTarget = ((tapValue - values.start).abs() * trackSize.width) < touchRadius;
        bool inEndTouchTarget = ((tapValue - values.end).abs() * trackSize.width) < touchRadius;
        if (inStartTouchTarget && inEndTouchTarget)
        {
            var (towardsStart, towardsEnd) = textDirection switch
            {
                TextDirection.ltr => (dx < 0L, (dx > 0L)),
                TextDirection.rtl => (dx > 0L, (dx < 0L)),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
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
            if ((tapValue * 2L) < (values.start + values.end))
            {
                return Thumb.start;
            }
            else
            {
                return Thumb.end;
            }
        }
        return null;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme = Theme.of(context);
        SliderThemeData sliderThemeLocal = SliderTheme.of(context);
        bool year2023Local = (widget.year2023 ?? sliderThemeLocal.year2023) ?? true;
        SliderThemeData defaults =
            (!year2023Local)
                ? new _RangeSliderDefaultsM3__range_slider(context)
                : new _RangeSliderDefaultsM3Year2023__range_slider(context);
        var states = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection25438 = new HashSet<WidgetState>();
                    if (!_enabled)
                    {
                        __collection25438.Add(WidgetState.disabled);
                    }
                    if (_hovering)
                    {
                        __collection25438.Add(WidgetState.hovered);
                    }
                    if (_dragging)
                    {
                        __collection25438.Add(WidgetState.dragged);
                    }
                    return __collection25438;
                }
            )
        )();
        RangeSliderValueIndicatorShape valueIndicatorShape =
            sliderThemeLocal.rangeValueIndicatorShape ?? defaults.rangeValueIndicatorShape!;
        Color valueIndicatorColorLocal = default!;
        if (valueIndicatorShape is RectangularRangeSliderValueIndicatorShape)
        {
            RectangularRangeSliderValueIndicatorShape valueIndicatorShape__25909__as26060 =
                (RectangularRangeSliderValueIndicatorShape)valueIndicatorShape;
            valueIndicatorColorLocal =
                sliderThemeLocal.valueIndicatorColor
                ?? DorotiUiLibrary.Color.alphaBlend(
                    theme.colorScheme.onSurface.withOpacity(0.6),
                    theme.colorScheme.surface.withOpacity(0.9)
                );
        }
        else
        {
            valueIndicatorColorLocal =
                (widget.activeColor ?? sliderThemeLocal.valueIndicatorColor)
                ?? defaults.valueIndicatorColor!;
        }
        Color? effectiveOverlayColor()
        {
            return (
                    (widget.overlayColor?.resolve(states) ?? widget.activeColor?.withOpacity(0.12))
                    ?? WidgetStateProperty.resolveAs(sliderThemeLocal.overlayColor, states)
                ) ?? defaults.overlayColor;
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        sliderThemeLocal = sliderThemeLocal.copyWith(
            trackHeight: sliderThemeLocal.trackHeight ?? defaults.trackHeight,
            activeTrackColor: (widget.activeColor ?? sliderThemeLocal.activeTrackColor)
                ?? defaults.activeTrackColor,
            inactiveTrackColor: (widget.inactiveColor ?? sliderThemeLocal.inactiveTrackColor)
                ?? defaults.inactiveTrackColor,
            disabledActiveTrackColor: sliderThemeLocal.disabledActiveTrackColor
                ?? defaults.disabledActiveTrackColor,
            disabledInactiveTrackColor: sliderThemeLocal.disabledInactiveTrackColor
                ?? defaults.disabledInactiveTrackColor,
            activeTickMarkColor: (widget.inactiveColor ?? sliderThemeLocal.activeTickMarkColor)
                ?? defaults.activeTickMarkColor,
            inactiveTickMarkColor: (widget.activeColor ?? sliderThemeLocal.inactiveTickMarkColor)
                ?? defaults.inactiveTickMarkColor,
            disabledActiveTickMarkColor: sliderThemeLocal.disabledActiveTickMarkColor
                ?? defaults.disabledActiveTickMarkColor,
            disabledInactiveTickMarkColor: sliderThemeLocal.disabledInactiveTickMarkColor
                ?? defaults.disabledInactiveTickMarkColor,
            thumbColor: (widget.activeColor ?? sliderThemeLocal.thumbColor) ?? defaults.thumbColor,
            overlappingShapeStrokeColor: sliderThemeLocal.overlappingShapeStrokeColor
                ?? defaults.overlappingShapeStrokeColor,
            disabledThumbColor: sliderThemeLocal.disabledThumbColor ?? defaults.disabledThumbColor,
            overlayColor: effectiveOverlayColor(),
            valueIndicatorColor: valueIndicatorColorLocal,
            rangeTrackShape: sliderThemeLocal.rangeTrackShape ?? defaults.rangeTrackShape,
            rangeTickMarkShape: sliderThemeLocal.rangeTickMarkShape ?? defaults.rangeTickMarkShape,
            rangeThumbShape: sliderThemeLocal.rangeThumbShape ?? defaults.rangeThumbShape,
            overlayShape: sliderThemeLocal.overlayShape ?? defaults.overlayShape,
            rangeValueIndicatorShape: valueIndicatorShape,
            showValueIndicator: sliderThemeLocal.showValueIndicator ?? defaults.showValueIndicator,
            valueIndicatorTextStyle: sliderThemeLocal.valueIndicatorTextStyle
                ?? defaults.valueIndicatorTextStyle,
            minThumbSeparation: sliderThemeLocal.minThumbSeparation ?? defaults.minThumbSeparation,
            thumbSelector: sliderThemeLocal.thumbSelector ?? _defaultRangeThumbSelector,
            padding: widget.padding ?? sliderThemeLocal.padding,
            thumbSize: sliderThemeLocal.thumbSize ?? defaults.thumbSize,
            trackGap: sliderThemeLocal.trackGap ?? defaults.trackGap
        );
        MouseCursor effectiveMouseCursor =
            (widget.mouseCursor?.resolve(states) ?? (sliderThemeLocal.mouseCursor?.resolve(states)))
            ?? WidgetStateMouseCursor.clickable.resolve(states);
        Size screenSize()
        {
            return MediaQuery.sizeOf(context);
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        double fontSizeLocal =
            sliderThemeLocal.valueIndicatorTextStyle?.fontSize
            ?? Text_painterLibrary.kDefaultFontSize;
        double fontSizeToScale =
            (fontSizeLocal == 0.0) ? Text_painterLibrary.kDefaultFontSize : fontSizeLocal;
        double effectiveTextScale =
            MediaQuery.textScalerOf(context).scale(fontSizeToScale) / fontSizeToScale;
        Widget result = new CompositedTransformTarget(
            link: _layerLink,
            child: new OverlayPortal(
                controller: _valueIndicatorOverlayPortalController,
                overlayChildBuilder: (context) =>
                {
                    return _buildValueIndicator(
                        (
                            sliderThemeLocal.showValueIndicator
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                },
                child: new _RangeSliderRenderObjectWidget__range_slider(
                    values: _unlerpRangeValues(widget.values),
                    divisions: widget.divisions,
                    labels: widget.labels,
                    sliderTheme: sliderThemeLocal,
                    textScaleFactor: effectiveTextScale,
                    screenSize: screenSize(),
                    onChanged: (_enabled && widget.max > widget.min) ? _handleChanged : null,
                    onChangeStart: _handleDragStart,
                    onChangeEnd: _handleDragEnd,
                    state: this,
                    semanticFormatterCallback: widget.semanticFormatterCallback,
                    hovering: _showHoverHighlight
                )
            )
        );
        EdgeInsetsGeometry? paddingLocal = widget.padding ?? sliderThemeLocal.padding;
        if (paddingLocal is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(padding: paddingLocal, child: result)
            );
        }
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Row(
                        children: new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Focus(
                                    focusNode: startFocusNode,
                                    includeSemantics: false,
                                    child: SizedBox.CreateShrink()
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new Focus(
                                    focusNode: endFocusNode,
                                    includeSemantics: false,
                                    child: SizedBox.CreateShrink()
                                )
                            ),
                        }
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new MouseRegion(
                        onEnter: (_) =>
                        {
                            _handleHoverChanged(true);
                        },
                        onExit: (_) =>
                        {
                            _handleHoverChanged(false);
                        },
                        cursor: effectiveMouseCursor,
                        child: result
                    )
                ),
            }
        );
    }

    internal virtual Widget _buildValueIndicator(ShowValueIndicator showValueIndicator)
    {
        Widget valueIndicator = new CompositedTransformFollower(
            link: _layerLink,
            child: new _ValueIndicatorRenderObjectWidget__range_slider(state: this)
        );
        return showValueIndicator switch
        {
            var __constant32003 when Equals(__constant32003, ShowValueIndicator.never) =>
                DartRuntimePrimitives.ConvertValue<Widget>(SizedBox.CreateShrink()),
            var __constant32062 when Equals(__constant32062, ShowValueIndicator.onlyForDiscrete) =>
                (widget.divisions is not null) ? valueIndicator : SizedBox.CreateShrink(),
            var __constant32183
                when Equals(__constant32183, ShowValueIndicator.onlyForContinuous) => (
                widget.divisions is null
            )
                ? valueIndicator
                : SizedBox.CreateShrink(),
            var __logical32306
                when Equals(__logical32306, ShowValueIndicator.alwaysVisible)
                    || Equals(__logical32306, ShowValueIndicator.always) => valueIndicator,
            var __constant32383 when Equals(__constant32383, ShowValueIndicator.onDrag) =>
                valueIndicator,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

internal class _RangeSliderRenderObjectWidget__range_slider : LeafRenderObjectWidget
{
    public virtual RangeValues values { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual RangeLabels? labels { get; private set; }
    public virtual SliderThemeData sliderTheme { get; private set; } = default!;
    public virtual double textScaleFactor { get; private set; } = default!;
    public virtual Size screenSize { get; private set; } = default!;
    public virtual Action<RangeValues>? onChanged { get; private set; }
    public virtual Action<RangeValues>? onChangeStart { get; private set; }
    public virtual Action<RangeValues>? onChangeEnd { get; private set; }
    public virtual SemanticFormatterCallback? semanticFormatterCallback { get; private set; }
    public virtual _RangeSliderState__range_slider state { get; private set; } = default!;
    public virtual bool hovering { get; private set; } = default!;

    internal _RangeSliderRenderObjectWidget__range_slider(
        RangeValues values,
        long? divisions,
        RangeLabels? labels,
        SliderThemeData sliderTheme,
        double textScaleFactor,
        Size screenSize,
        Action<RangeValues>? onChanged,
        Action<RangeValues>? onChangeStart,
        Action<RangeValues>? onChangeEnd,
        _RangeSliderState__range_slider state,
        SemanticFormatterCallback? semanticFormatterCallback,
        bool hovering
    )
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

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderRangeSlider__range_slider(
            values: values,
            divisions: divisions,
            labels: labels,
            sliderTheme: sliderTheme,
            theme: Theme.of(context),
            textScaleFactor: textScaleFactor,
            screenSize: screenSize,
            onChanged: onChanged,
            onChangeStart: onChangeStart,
            onChangeEnd: onChangeEnd,
            state: state,
            textDirection: Directionality.of(context),
            semanticFormatterCallback: semanticFormatterCallback,
            platform: Theme.of(context).platform,
            hovering: hovering,
            gestureSettings: MediaQuery.gestureSettingsOf(context)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderRangeSlider__range_slider)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderRangeSlider__range_slider>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.divisions = divisions;
                        __cascade.values = values;
                        __cascade.labels = labels;
                        __cascade.sliderTheme = sliderTheme;
                        __cascade.theme = Theme.of(context);
                        __cascade.textScaleFactor = textScaleFactor;
                        __cascade.screenSize = screenSize;
                        __cascade.onChanged = onChanged;
                        __cascade.onChangeStart = onChangeStart;
                        __cascade.onChangeEnd = onChangeEnd;
                        __cascade.textDirection = Directionality.of(context);
                        __cascade.semanticFormatterCallback = semanticFormatterCallback;
                        __cascade.platform = Theme.of(context).platform;
                        __cascade.hovering = hovering;
                        __cascade.gestureSettings = MediaQuery.gestureSettingsOf(context);
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderRangeSlider__range_slider : RenderBox, RelayoutWhenSystemFontsChangeMixin
{
    internal virtual Thumb? _lastThumbSelection { get; set; } = default;
    internal static Duration _positionAnimationDuration = Duration.Create(milliseconds: 75L);
    internal const double _minPreferredTrackWidth = 144.0;
    internal static Duration _minimumInteractionTime = Duration.Create(milliseconds: 500L);
    internal virtual _RangeSliderState__range_slider _state { get; private set; } = default!;
    internal virtual CurvedAnimation _overlayAnimation { get; set; } = default!;
    internal virtual CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual CurvedAnimation _enableAnimation { get; set; } = default!;
    internal virtual TextPainter _startLabelPainter { get; private set; } = new TextPainter();
    internal virtual TextPainter _endLabelPainter { get; private set; } = new TextPainter();
    internal virtual Gestures.HorizontalDragGestureRecognizer _drag { get; set; } = default!;
    internal virtual Gestures.TapGestureRecognizer _tap { get; set; } = default!;
    internal virtual bool _active { get; set; } = false;
    internal virtual RangeValues _newValues { get; set; } = default!;
    internal virtual Offset _startThumbCenter { get; set; } = Offset.zero;
    internal virtual Offset _endThumbCenter { get; set; } = Offset.zero;
    public virtual Rect? overlayStartRect { get; set; } = default;
    public virtual Rect? overlayEndRect { get; set; } = default;
    internal virtual RangeValues _values { get; set; } = default!;
    internal virtual TargetPlatform _platform { get; set; } = default!;
    internal virtual SemanticFormatterCallback? _semanticFormatterCallback { get; set; } = default;
    internal virtual long? _divisions { get; set; } = default;
    internal virtual RangeLabels? _labels { get; set; } = default;
    internal virtual SliderThemeData _sliderTheme { get; set; } = default!;
    internal virtual ThemeData? _theme { get; set; } = default;
    internal virtual double _textScaleFactor { get; set; } = default!;
    internal virtual Size _screenSize { get; set; } = default!;
    internal virtual Action<RangeValues>? _onChanged { get; set; } = default;
    public virtual Action<RangeValues>? onChangeStart { get; set; } = default;
    public virtual Action<RangeValues>? onChangeEnd { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual bool _hovering { get; set; } = default!;
    internal virtual bool _hoveringStartThumb { get; set; } = false;
    internal virtual bool _hoveringEndThumb { get; set; } = false;
    internal virtual SemanticsNode? _startSemanticsNode { get; set; } = default;
    internal virtual SemanticsNode? _endSemanticsNode { get; set; } = default;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderRangeSlider__range_slider(
        RangeValues values,
        long? divisions,
        RangeLabels? labels,
        SliderThemeData sliderTheme,
        ThemeData? theme,
        double textScaleFactor,
        Size screenSize,
        TargetPlatform platform,
        Action<RangeValues>? onChanged,
        SemanticFormatterCallback? semanticFormatterCallback,
        Action<RangeValues>? onChangeStart,
        Action<RangeValues>? onChangeEnd,
        _RangeSliderState__range_slider state,
        TextDirection textDirection,
        bool hovering,
        Gestures.DeviceGestureSettings gestureSettings
    )
    {
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        _platform = platform;
        _semanticFormatterCallback = semanticFormatterCallback;
        _labels = labels;
        _values = values;
        _divisions = divisions;
        _sliderTheme = sliderTheme;
        _theme = theme;
        _textScaleFactor = textScaleFactor;
        _screenSize = screenSize;
        _onChanged = onChanged;
        _state = state;
        _textDirection = textDirection;
        _hovering = hovering;
        System.Diagnostics.Debug.Assert((values.start >= 0.0) && (values.start <= 1.0));
        System.Diagnostics.Debug.Assert((values.end >= 0.0) && (values.end <= 1.0));
        _updateLabelPainters();
        var teamLocal = new Gestures.GestureArenaTeam();
        _drag = (
            (Func<Gestures.HorizontalDragGestureRecognizer>)(
                () =>
                {
                    var __cascade = new Gestures.HorizontalDragGestureRecognizer();
                    __cascade.team = teamLocal;
                    __cascade.onStart = _handleDragStart;
                    __cascade.onUpdate = _handleDragUpdate;
                    __cascade.onEnd = _handleDragEnd;
                    __cascade.onCancel = _handleDragCancel;
                    __cascade.gestureSettings = gestureSettings;
                    return __cascade;
                }
            )
        )();
        _tap = (
            (Func<Gestures.TapGestureRecognizer>)(
                () =>
                {
                    var __cascade = new Gestures.TapGestureRecognizer();
                    __cascade.team = teamLocal;
                    __cascade.onTapDown = _handleTapDown;
                    __cascade.onTapUp = _handleTapUp;
                    __cascade.gestureSettings = gestureSettings;
                    return __cascade;
                }
            )
        )();
        _overlayAnimation = new CurvedAnimation(
            parent: _state.overlayController,
            curve: Curves.fastOutSlowIn
        );
        _valueIndicatorAnimation = new CurvedAnimation(
            parent: _state.valueIndicatorController,
            curve: Curves.fastOutSlowIn
        );
        _enableAnimation = new CurvedAnimation(
            parent: _state.enableController,
            curve: Curves.easeInOut
        );
    }

    internal virtual double _maxSliderPartWidth =>
        _sliderPartSizes.map((size) => size.width).reduce(Math.Max);
    internal virtual double _maxSliderPartHeight =>
        _sliderPartSizes.map((size) => size.height).reduce(Math.Max);
    internal virtual double _thumbSizeHeight =>
        _sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete).height;
    internal virtual double _overlayHeight =>
        _sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).height;
    internal virtual List<Size> _sliderPartSizes =>
        new List<Size>
        {
            new Size(
                _sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width,
                (_sliderTheme.padding is not null) ? _thumbSizeHeight : _overlayHeight
            ),
            _sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete),
            _sliderTheme.rangeTickMarkShape!.getPreferredSize(
                isEnabled: isEnabled,
                sliderTheme: sliderTheme
            ),
        }
            .Cast<Size>()
            .ToList();
    internal virtual double? _minPreferredTrackHeight => _sliderTheme.trackHeight;
    internal virtual Rect _trackRect =>
        DartRuntimePrimitives.ConvertValue<Rect>(
            _sliderTheme.rangeTrackShape!.getPreferredRect(
                parentBox: this,
                sliderTheme: _sliderTheme,
                isDiscrete: false
            )
        );
    public virtual bool isEnabled =>
        DartRuntimePrimitives.ConvertValue<bool>(onChanged is not null);
    public virtual bool isDiscrete =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (divisions is not null)
                && (
                    (
                        divisions
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) > 0L
                )
        );
    internal virtual double _minThumbSeparationValue =>
        isDiscrete
            ? 0
            : (
                (
                    sliderTheme.minThumbSeparation
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) / _trackRect.width
            );
    public virtual RangeValues values
    {
        get => _values;
        set
        {
            var newValues = value;
            DartRuntimePrimitives.Assert(() =>
                (newValues.start >= 0.0) && (newValues.start <= 1.0)
            );
            DartRuntimePrimitives.Assert(() => (newValues.end >= 0.0) && (newValues.end <= 1.0));
            DartRuntimePrimitives.Assert(() => newValues.start <= newValues.end);
            RangeValues convertedValues = isDiscrete
                ? _discretizeRangeValues(newValues)
                : newValues;
            if (Equals(convertedValues, _values))
            {
                return;
            }
            _values = convertedValues;
            if (isDiscrete)
            {
                double startDistance = (_values.start - _state.startPositionController.value).abs();
                _state.startPositionController.duration =
                    (startDistance != 0.0)
                        ? (_positionAnimationDuration * (1.0 / startDistance))
                        : Duration.zero;
                _state.startPositionController.animateTo(_values.start, curve: Curves.easeInOut);
                double endDistance = (_values.end - _state.endPositionController.value).abs();
                _state.endPositionController.duration =
                    (endDistance != 0.0)
                        ? (_positionAnimationDuration * (1.0 / endDistance))
                        : Duration.zero;
                _state.endPositionController.animateTo(_values.end, curve: Curves.easeInOut);
            }
            else
            {
                _state.startPositionController.value = convertedValues.start;
                _state.endPositionController.value = convertedValues.end;
            }
            markNeedsSemanticsUpdate();
        }
    }
    public virtual TargetPlatform platform
    {
        get => _platform;
        set
        {
            var __value = value;
            if (Equals(_platform, (__value)))
            {
                return;
            }
            _platform = (__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual Gestures.DeviceGestureSettings? gestureSettings
    {
        get => _drag.gestureSettings;
        set
        {
            var gestureSettings = value;
            _drag.gestureSettings = gestureSettings;
            _tap.gestureSettings = gestureSettings;
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
    public virtual RangeLabels? labels
    {
        get => _labels;
        set
        {
            var labels = value;
            if (Equals(labels, _labels))
            {
                return;
            }
            _labels = labels;
            _updateLabelPainters();
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
            markNeedsPaint();
        }
    }
    public virtual ThemeData? theme
    {
        get => _theme;
        set
        {
            var __value = value;
            if (Equals(__value, _theme))
            {
                return;
            }
            _theme = __value;
            markNeedsPaint();
        }
    }
    public virtual double textScaleFactor
    {
        get => _textScaleFactor;
        set
        {
            var __value = value;
            if ((__value) == _textScaleFactor)
            {
                return;
            }
            _textScaleFactor = (__value);
            _updateLabelPainters();
        }
    }
    public virtual Size screenSize
    {
        get => _screenSize;
        set
        {
            var __value = value;
            if (
                Equals(
                    (
                        __value
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    screenSize
                )
            )
            {
                return;
            }
            _screenSize = (
                __value
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            markNeedsPaint();
        }
    }
    public virtual Action<RangeValues>? onChanged
    {
        get => _onChanged;
        set
        {
            var __value = value;
            if (Equals(__value, _onChanged))
            {
                return;
            }
            bool wasEnabled = isEnabled;
            _onChanged = __value;
            if (wasEnabled != isEnabled)
            {
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals((__value), _textDirection))
            {
                return;
            }
            _textDirection = (__value);
            _updateLabelPainters();
        }
    }
    public virtual bool hovering
    {
        get => _hovering;
        set
        {
            var __value = value;
            if ((__value) == _hovering)
            {
                return;
            }
            _hovering = (__value);
            _updateForHover(_hovering);
        }
    }
    public virtual bool hoveringStartThumb
    {
        get => _hoveringStartThumb;
        set
        {
            var __value = value;
            if ((__value) == _hoveringStartThumb)
            {
                return;
            }
            _hoveringStartThumb = (__value);
            _updateForHover(_hovering);
        }
    }
    public virtual bool hoveringEndThumb
    {
        get => _hoveringEndThumb;
        set
        {
            var __value = value;
            if ((__value) == _hoveringEndThumb)
            {
                return;
            }
            _hoveringEndThumb = (__value);
            _updateForHover(_hovering);
        }
    }

    internal virtual void _updateForHover(bool hovered)
    {
        if (hovered && (hoveringStartThumb || hoveringEndThumb))
        {
            _state.overlayController.forward();
        }
        else
        {
            _state.overlayController.reverse();
        }
    }

    public virtual bool shouldAlwaysShowValueIndicator =>
        DartRuntimePrimitives.ConvertValue<bool>(
            Equals(_sliderTheme.showValueIndicator, ShowValueIndicator.alwaysVisible)
        );
    public virtual bool shouldShowValueIndicatorWhenDragged =>
        _sliderTheme.showValueIndicator! switch
        {
            var __constant45371 when Equals(__constant45371, ShowValueIndicator.onlyForDiscrete) =>
                isDiscrete,
            var __constant45425
                when Equals(__constant45425, ShowValueIndicator.onlyForContinuous) => !isDiscrete,
            var __logical45482
                when Equals(__logical45482, ShowValueIndicator.alwaysVisible)
                    || Equals(__logical45482, ShowValueIndicator.always) => true,
            var __constant45555 when Equals(__constant45555, ShowValueIndicator.onDrag) => true,
            var __constant45594 when Equals(__constant45594, ShowValueIndicator.never) => false,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
    internal virtual Size _thumbSize =>
        DartRuntimePrimitives.ConvertValue<Size>(
            _sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete)
        );
    internal virtual double _adjustmentUnit
    {
        get
        {
            switch (_platform)
            {
                case TargetPlatform.iOS:
                {
                    return 0.1;
                }
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                {
                    return 0.05;
                }
                default:
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    );
            }
        }
    }

    internal virtual void _updateLabelPainters()
    {
        _updateLabelPainter(Thumb.start);
        _updateLabelPainter(Thumb.end);
    }

    internal virtual void _updateLabelPainter(Thumb thumb)
    {
        RangeLabels? labelsLocal = labels;
        if (labelsLocal is null)
        {
            return;
        }
        var (textLocal, labelPainter) = thumb switch
        {
            var __constant46482 when Equals(__constant46482, Thumb.start) => ((string, TextPainter))
                (labelsLocal.start, _startLabelPainter),
            var __constant46539 when Equals(__constant46539, Thumb.end) => ((string, TextPainter))
                (labelsLocal.end, _endLabelPainter),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        DartRuntimePrimitives.Ignore(
            (
                (Func<TextPainter>)(
                    () =>
                    {
                        var __cascade = labelPainter;
                        __cascade.text = new TextSpan(
                            style: _sliderTheme.valueIndicatorTextStyle,
                            text: textLocal
                        );
                        __cascade.textDirection = textDirection;
                        __cascade.textScaleFactor = textScaleFactor;
                        __cascade.layout();
                        return __cascade;
                    }
                )
            )()
        );
        markNeedsLayout();
    }

    public virtual void systemFontsDidChange()
    {
        markNeedsLayout();
        _startLabelPainter.markNeedsLayout();
        _endLabelPainter.markNeedsLayout();
        _updateLabelPainters();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.addListener(_scheduleSystemFontsUpdate);
        _overlayAnimation.addListener(markNeedsPaint);
        _valueIndicatorAnimation.addListener(markNeedsPaint);
        _enableAnimation.addListener(markNeedsPaint);
        _state.startPositionController.addListener(markNeedsPaint);
        _state.endPositionController.addListener(markNeedsPaint);
        _state.startFocusNode.addListener(markNeedsPaint);
        _state.startFocusNode.addListener(markNeedsSemanticsUpdate);
        _state.endFocusNode.addListener(markNeedsPaint);
        _state.endFocusNode.addListener(markNeedsSemanticsUpdate);
    }

    public override void detach()
    {
        _overlayAnimation.removeListener(markNeedsPaint);
        _valueIndicatorAnimation.removeListener(markNeedsPaint);
        _enableAnimation.removeListener(markNeedsPaint);
        _state.startPositionController.removeListener(markNeedsPaint);
        _state.endPositionController.removeListener(markNeedsPaint);
        _state.startFocusNode.removeListener(markNeedsPaint);
        _state.startFocusNode.removeListener(markNeedsSemanticsUpdate);
        _state.endFocusNode.removeListener(markNeedsPaint);
        _state.endFocusNode.removeListener(markNeedsSemanticsUpdate);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.removeListener(_scheduleSystemFontsUpdate);
        base.detach();
    }

    public override void dispose()
    {
        _drag.dispose();
        _tap.dispose();
        _startLabelPainter.dispose();
        _endLabelPainter.dispose();
        _enableAnimation.dispose();
        _valueIndicatorAnimation.dispose();
        _overlayAnimation.dispose();
        base.dispose();
    }

    internal virtual double _getValueFromVisualPosition(double visualPosition)
    {
        return textDirection switch
        {
            TextDirection.rtl => 1.0 - visualPosition,
            TextDirection.ltr => visualPosition,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
    }

    internal virtual double _getValueFromGlobalPosition(Offset globalPosition)
    {
        double visualPosition =
            (globalToLocal(globalPosition).dx - _trackRect.left) / _trackRect.width;
        return _getValueFromVisualPosition(visualPosition);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _discretize(double value)
    {
        double result = DorotiUiLibrary.clampDouble((value), 0.0, 1.0);
        if (isDiscrete)
        {
            result =
                (
                    result
                    * (
                        divisions
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ).round()
                / (double)(
                    divisions
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual RangeValues _discretizeRangeValues(RangeValues values)
    {
        return new RangeValues(_discretize(values.start), _discretize(values.end));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _startInteraction(Offset globalPosition)
    {
        if (_active)
        {
            return;
        }
        double tapValue = DorotiUiLibrary.clampDouble(
            _getValueFromGlobalPosition(globalPosition),
            0.0,
            1.0
        );
        _lastThumbSelection = sliderTheme.thumbSelector!(
            textDirection,
            values,
            tapValue,
            _thumbSize,
            size,
            0
        );
        if (_lastThumbSelection is not null)
        {
            switch (_lastThumbSelection!)
            {
                case var __constant49928 when Equals(__constant49928, Thumb.start):
                {
                    _state.startFocusNode.requestFocus();
                    break;
                }
                case var __constant50002 when Equals(__constant50002, Thumb.end):
                {
                    _state.endFocusNode.requestFocus();
                    break;
                }
            }
            _active = true;
            RangeValues currentValues = _discretizeRangeValues(values);
            _newValues = _lastThumbSelection! switch
            {
                var __constant50438 when Equals(__constant50438, Thumb.start) => new RangeValues(
                    tapValue,
                    currentValues.end
                ),
                var __constant50503 when Equals(__constant50503, Thumb.end) => new RangeValues(
                    currentValues.start,
                    tapValue
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
            _updateLabelPainter(
                (
                    _lastThumbSelection
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
            onChangeStart?.Invoke(currentValues);
            onChanged!(_discretizeRangeValues(_newValues));
            _state.overlayController.forward();
            if (shouldShowValueIndicatorWhenDragged)
            {
                _state.valueIndicatorController.forward();
                _state.interactionTimer?.cancel();
                _state.interactionTimer = new Timer(
                    _minimumInteractionTime * Scheduler.BindingLibrary.timeDilation,
                    () =>
                    {
                        _state.interactionTimer = null;
                        if (!_active && _state.valueIndicatorController.isCompleted)
                        {
                            _state.valueIndicatorController.reverse();
                        }
                    }
                );
            }
        }
    }

    internal virtual void _handleDragUpdate(Gestures.DragUpdateDetails details)
    {
        if (!_state.mounted)
        {
            return;
        }
        double dragValue = _getValueFromGlobalPosition(details.globalPosition);
        var shouldCallOnChangeStart = false;
        if (_lastThumbSelection is null)
        {
            _lastThumbSelection = sliderTheme.thumbSelector!(
                textDirection,
                values,
                dragValue,
                _thumbSize,
                size,
                details.delta.dx
            );
            if (_lastThumbSelection is not null)
            {
                shouldCallOnChangeStart = true;
                _active = true;
                _state.overlayController.forward();
                if (shouldShowValueIndicatorWhenDragged)
                {
                    _state.valueIndicatorController.forward();
                }
            }
        }
        if (isEnabled && (_lastThumbSelection is not null))
        {
            RangeValues currentValues = _discretizeRangeValues(values);
            if ((onChangeStart is not null) && shouldCallOnChangeStart)
            {
                onChangeStart!(currentValues);
            }
            double currentDragValue = _discretize(dragValue);
            _newValues = _lastThumbSelection! switch
            {
                var __constant52496 when Equals(__constant52496, Thumb.start) => new RangeValues(
                    Math.Min(currentDragValue, currentValues.end - _minThumbSeparationValue),
                    currentValues.end
                ),
                var __constant52656 when Equals(__constant52656, Thumb.end) => new RangeValues(
                    currentValues.start,
                    Math.Max(currentDragValue, currentValues.start + _minThumbSeparationValue)
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
            onChanged!(_discretizeRangeValues(_newValues));
        }
    }

    internal virtual void _endInteraction()
    {
        if (!_state.mounted)
        {
            return;
        }
        if (shouldShowValueIndicatorWhenDragged && (_state.interactionTimer is null))
        {
            _state.valueIndicatorController.reverse();
        }
        if (_active && _state.mounted && (_lastThumbSelection is not null))
        {
            RangeValues discreteValues = _discretizeRangeValues(_newValues);
            onChangeEnd?.Invoke(discreteValues);
            _active = false;
        }
        _state.overlayController.reverse();
    }

    internal virtual void _handleDragStart(Gestures.DragStartDetails details)
    {
        _startInteraction(details.globalPosition);
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details)
    {
        _endInteraction();
    }

    internal virtual void _handleDragCancel()
    {
        _endInteraction();
    }

    internal virtual void _handleTapDown(Gestures.TapDownDetails details)
    {
        _startInteraction(details.globalPosition);
    }

    internal virtual void _handleTapUp(Gestures.TapUpDetails details)
    {
        _endInteraction();
    }

    public override bool hitTestSelf(Offset position) => true;

    public override void handleEvent(
        Gestures.PointerEvent @event,
        Gestures.HitTestEntry<Gestures.HitTestTarget> entry
    )
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is Gestures.PointerDownEvent) && isEnabled)
        {
            Gestures.PointerDownEvent @event__as53949 = (Gestures.PointerDownEvent)@event;
            _drag.addPointer(@event__as53949);
            _tap.addPointer(@event__as53949);
        }
        if (isEnabled)
        {
            if (overlayStartRect is not null)
            {
                hoveringStartThumb = (
                    overlayStartRect
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).contains(@event.localPosition);
            }
            if (overlayEndRect is not null)
            {
                hoveringEndThumb = (
                    overlayEndRect
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).contains(@event.localPosition);
            }
        }
    }

    public override double computeMinIntrinsicWidth(double height) =>
        DartRuntimePrimitives.ConvertValue<double>(_minPreferredTrackWidth + _maxSliderPartWidth);

    public override double computeMaxIntrinsicWidth(double height) =>
        DartRuntimePrimitives.ConvertValue<double>(_minPreferredTrackWidth + _maxSliderPartWidth);

    public override double computeMinIntrinsicHeight(double width) =>
        Math.Max(
            (
                _minPreferredTrackHeight
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            _maxSliderPartHeight
        );

    public override double computeMaxIntrinsicHeight(double width) =>
        Math.Max(
            (
                _minPreferredTrackHeight
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            _maxSliderPartHeight
        );

    public override bool sizedByParent => true;

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return new Size(
            constraints.hasBoundedWidth
                ? constraints.maxWidth
                : (_minPreferredTrackWidth + _maxSliderPartWidth),
            constraints.hasBoundedHeight
                ? constraints.maxHeight
                : Math.Max(
                    (
                        _minPreferredTrackHeight
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    _maxSliderPartHeight
                )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        double startValue = _state.startPositionController.value;
        double endValue = _state.endPositionController.value;
        var (startVisualPosition, endVisualPosition) = textDirection switch
        {
            TextDirection.rtl => (1.0 - startValue, 1.0 - endValue),
            TextDirection.ltr => (startValue, endValue),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        Rect trackRect = _sliderTheme.rangeTrackShape!.getPreferredRect(
            parentBox: this,
            offset: offset,
            sliderTheme: _sliderTheme,
            isDiscrete: isDiscrete
        );
        double padding = _sliderTheme.rangeTrackShape!.isRounded ? trackRect.height : 0.0;
        double thumbYOffset = trackRect.center.dy;
        double startThumbPosition = isDiscrete
            ? (
                trackRect.left
                + (startVisualPosition * (trackRect.width - padding))
                + (padding / 2L)
            )
            : (trackRect.left + (startVisualPosition * trackRect.width));
        double endThumbPosition = isDiscrete
            ? (trackRect.left + (endVisualPosition * (trackRect.width - padding)) + (padding / 2L))
            : (trackRect.left + (endVisualPosition * trackRect.width));
        Size thumbPreferredSize = _sliderTheme.rangeThumbShape!.getPreferredSize(
            isEnabled,
            isDiscrete
        );
        double thumbPadding = (padding > (thumbPreferredSize.width / 2L)) ? (padding / 2L) : 0;
        _startThumbCenter = new Offset(
            DorotiUiLibrary.clampDouble(
                startThumbPosition,
                trackRect.left + thumbPadding,
                trackRect.right - thumbPadding
            ),
            thumbYOffset
        );
        _endThumbCenter = new Offset(
            DorotiUiLibrary.clampDouble(
                endThumbPosition,
                trackRect.left + thumbPadding,
                trackRect.right - thumbPadding
            ),
            thumbYOffset
        );
        if (isEnabled)
        {
            Size overlaySize = sliderTheme.overlayShape!.getPreferredSize(isEnabled, false);
            overlayStartRect = Rect.fromCircle(
                center: _startThumbCenter,
                radius: overlaySize.width / 2.0
            );
            overlayEndRect = Rect.fromCircle(
                center: _endThumbCenter,
                radius: overlaySize.width / 2.0
            );
        }
        double? thumbWidth = _sliderTheme.thumbSize?.resolve(new HashSet<WidgetState>())?.width;
        double? thumbHeight = _sliderTheme.thumbSize?.resolve(new HashSet<WidgetState>())?.height;
        double? trackGapLocal = _sliderTheme.trackGap;
        double? pressedThumbWidth = _sliderTheme
            .thumbSize?.resolve(new HashSet<WidgetState> { WidgetState.pressed })
            ?.width;
        double delta = default!;
        if (
            _active
            && (thumbWidth is not null)
            && (pressedThumbWidth is not null)
            && (trackGapLocal is not null)
        )
        {
            double thumbWidth__57746__value58122 = (
                thumbWidth
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            double pressedThumbWidth__57970__value58144 = (
                pressedThumbWidth
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            double trackGap__57918__value58173 = (
                trackGapLocal
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            delta = (thumbWidth__57746__value58122) - (pressedThumbWidth__57970__value58144);
            thumbWidth = (pressedThumbWidth__57970__value58144);
            if ((trackGap__57918__value58173) > 0.0)
            {
                trackGapLocal = (trackGap__57918__value58173) - (delta / 2L);
            }
        }
        _sliderTheme.rangeTrackShape!.paint(
            context,
            offset,
            parentBox: this,
            sliderTheme: _sliderTheme.copyWith(trackGap: trackGapLocal),
            enableAnimation: _enableAnimation,
            textDirection: _textDirection,
            startThumbCenter: _startThumbCenter,
            endThumbCenter: _endThumbCenter,
            isDiscrete: isDiscrete,
            isEnabled: isEnabled
        );
        bool startThumbSelected = Equals(_lastThumbSelection, Thumb.start) && !hoveringEndThumb;
        bool endThumbSelected = Equals(_lastThumbSelection, Thumb.end) && !hoveringStartThumb;
        Size resolvedscreenSize = screenSize.isEmpty ? size : screenSize;
        if (_state.startFocusNode.hasFocus)
        {
            _sliderTheme.overlayShape!.paint(
                context,
                _startThumbCenter,
                activationAnimation: new AlwaysStoppedAnimation<double>(1.0),
                enableAnimation: _enableAnimation,
                isDiscrete: isDiscrete,
                labelPainter: _startLabelPainter,
                parentBox: this,
                sliderTheme: _sliderTheme,
                textDirection: _textDirection,
                value: startValue,
                textScaleFactor: _textScaleFactor,
                sizeWithOverflow: resolvedscreenSize
            );
        }
        if (_state.endFocusNode.hasFocus)
        {
            _sliderTheme.overlayShape!.paint(
                context,
                _endThumbCenter,
                activationAnimation: new AlwaysStoppedAnimation<double>(1.0),
                enableAnimation: _enableAnimation,
                isDiscrete: isDiscrete,
                labelPainter: _endLabelPainter,
                parentBox: this,
                sliderTheme: _sliderTheme,
                textDirection: _textDirection,
                value: endValue,
                textScaleFactor: _textScaleFactor,
                sizeWithOverflow: resolvedscreenSize
            );
        }
        if (!_overlayAnimation.isDismissed)
        {
            if (startThumbSelected || hoveringStartThumb)
            {
                _sliderTheme.overlayShape!.paint(
                    context,
                    _startThumbCenter,
                    activationAnimation: _overlayAnimation,
                    enableAnimation: _enableAnimation,
                    isDiscrete: isDiscrete,
                    labelPainter: _startLabelPainter,
                    parentBox: this,
                    sliderTheme: _sliderTheme,
                    textDirection: _textDirection,
                    value: startValue,
                    textScaleFactor: _textScaleFactor,
                    sizeWithOverflow: resolvedscreenSize
                );
            }
            if (endThumbSelected || hoveringEndThumb)
            {
                _sliderTheme.overlayShape!.paint(
                    context,
                    _endThumbCenter,
                    activationAnimation: _overlayAnimation,
                    enableAnimation: _enableAnimation,
                    isDiscrete: isDiscrete,
                    labelPainter: _endLabelPainter,
                    parentBox: this,
                    sliderTheme: _sliderTheme,
                    textDirection: _textDirection,
                    value: endValue,
                    textScaleFactor: _textScaleFactor,
                    sizeWithOverflow: resolvedscreenSize
                );
            }
        }
        if (isDiscrete)
        {
            double tickMarkWidth = _sliderTheme
                .rangeTickMarkShape!.getPreferredSize(
                    isEnabled: isEnabled,
                    sliderTheme: _sliderTheme
                )
                .width;
            double discreteTrackPadding = trackRect.height;
            double adjustedTrackWidth = trackRect.width - discreteTrackPadding;
            if (
                (
                    adjustedTrackWidth
                    / (
                        divisions
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ) >= (3.0 * tickMarkWidth)
            )
            {
                double dyLocal = trackRect.center.dy;
                for (
                    var i = 0L;
                    i
                        <= (
                            divisions
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        );
                    i++
                )
                {
                    double valueLocal =
                        i
                        / (double)(
                            divisions
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        );
                    double dxLocal =
                        trackRect.left
                        + ((valueLocal) * adjustedTrackWidth)
                        + (discreteTrackPadding / 2L);
                    var tickMarkOffset = new Offset(dxLocal, dyLocal);
                    _sliderTheme.rangeTickMarkShape!.paint(
                        context,
                        tickMarkOffset,
                        parentBox: this,
                        sliderTheme: _sliderTheme,
                        enableAnimation: _enableAnimation,
                        textDirection: _textDirection,
                        startThumbCenter: _startThumbCenter,
                        endThumbCenter: _endThumbCenter,
                        isEnabled: isEnabled
                    );
                }
            }
        }
        double thumbDelta = (_endThumbCenter.dx - _startThumbCenter.dx).abs();
        var isLastThumbStart = Equals(_lastThumbSelection, Thumb.start);
        Thumb bottomThumb = isLastThumbStart ? Thumb.end : Thumb.start;
        Thumb topThumb = isLastThumbStart ? Thumb.start : Thumb.end;
        Offset bottomThumbCenter = isLastThumbStart ? _endThumbCenter : _startThumbCenter;
        Offset topThumbCenter = isLastThumbStart ? _startThumbCenter : _endThumbCenter;
        TextPainter bottomLabelPainter = isLastThumbStart ? _endLabelPainter : _startLabelPainter;
        TextPainter topLabelPainter = isLastThumbStart ? _startLabelPainter : _endLabelPainter;
        var bottomValue = isLastThumbStart ? endValue : startValue;
        var topValue = isLastThumbStart ? startValue : endValue;
        bool shouldPaintValueIndicators =
            isEnabled
            && (labels is not null)
            && (
                (shouldShowValueIndicatorWhenDragged && !_valueIndicatorAnimation.isDismissed)
                || shouldAlwaysShowValueIndicator
            );
        if (shouldPaintValueIndicators)
        {
            _state.paintBottomValueIndicator = (context, offset) =>
            {
                if (attached)
                {
                    _sliderTheme.rangeValueIndicatorShape!.paint(
                        context,
                        bottomThumbCenter,
                        activationAnimation: shouldAlwaysShowValueIndicator
                            ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(
                                1
                            )
                            : _valueIndicatorAnimation,
                        enableAnimation: shouldAlwaysShowValueIndicator
                            ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(
                                1
                            )
                            : _enableAnimation,
                        isDiscrete: isDiscrete,
                        isOnTop: false,
                        labelPainter: bottomLabelPainter,
                        parentBox: this,
                        sliderTheme: _sliderTheme,
                        textDirection: _textDirection,
                        thumb: bottomThumb,
                        value: bottomValue,
                        textScaleFactor: textScaleFactor,
                        sizeWithOverflow: resolvedscreenSize
                    );
                }
            };
        }
        _sliderTheme.rangeThumbShape!.paint(
            context,
            bottomThumbCenter,
            activationAnimation: _valueIndicatorAnimation,
            enableAnimation: _enableAnimation,
            isDiscrete: isDiscrete,
            isOnTop: false,
            textDirection: textDirection,
            sliderTheme: ((thumbWidth is not null) && (thumbHeight is not null))
                ? _sliderTheme.copyWith(
                    thumbSize: new WidgetStatePropertyAll<Size?>(
                        new Size(
                            (
                                thumbWidth
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ),
                            (
                                thumbHeight
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    )
                )
                : _sliderTheme,
            thumb: bottomThumb,
            isPressed: Equals(bottomThumb, Thumb.start) ? startThumbSelected : endThumbSelected
        );
        if (shouldPaintValueIndicators)
        {
            double startOffset = sliderTheme.rangeValueIndicatorShape!.getHorizontalShift(
                parentBox: this,
                center: _startThumbCenter,
                labelPainter: _startLabelPainter,
                activationAnimation: _valueIndicatorAnimation,
                textScaleFactor: textScaleFactor,
                sizeWithOverflow: resolvedscreenSize
            );
            double endOffset = sliderTheme.rangeValueIndicatorShape!.getHorizontalShift(
                parentBox: this,
                center: _endThumbCenter,
                labelPainter: _endLabelPainter,
                activationAnimation: _valueIndicatorAnimation,
                textScaleFactor: textScaleFactor,
                sizeWithOverflow: resolvedscreenSize
            );
            double startHalfWidth =
                sliderTheme
                    .rangeValueIndicatorShape!.getPreferredSize(
                        isEnabled,
                        isDiscrete,
                        labelPainter: _startLabelPainter,
                        textScaleFactor: textScaleFactor
                    )
                    .width / 2L;
            double endHalfWidth =
                sliderTheme
                    .rangeValueIndicatorShape!.getPreferredSize(
                        isEnabled,
                        isDiscrete,
                        labelPainter: _endLabelPainter,
                        textScaleFactor: textScaleFactor
                    )
                    .width / 2L;
            double innerOverflow =
                startHalfWidth
                + endHalfWidth
                + (
                    textDirection switch
                    {
                        TextDirection.ltr => startOffset - endOffset,
                        TextDirection.rtl => endOffset - startOffset,
                        _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                            throw new InvalidOperationException(
                                "Switch expression did not handle the supplied value."
                            ),
                    }
                );
            _state.paintTopValueIndicator = (context, offset) =>
            {
                if (attached)
                {
                    _sliderTheme.rangeValueIndicatorShape!.paint(
                        context,
                        topThumbCenter,
                        activationAnimation: shouldAlwaysShowValueIndicator
                            ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(
                                1
                            )
                            : _valueIndicatorAnimation,
                        enableAnimation: shouldAlwaysShowValueIndicator
                            ? new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<double>(
                                1
                            )
                            : _enableAnimation,
                        isDiscrete: isDiscrete,
                        isOnTop: thumbDelta < innerOverflow,
                        labelPainter: topLabelPainter,
                        parentBox: this,
                        sliderTheme: _sliderTheme,
                        textDirection: _textDirection,
                        thumb: topThumb,
                        value: topValue,
                        textScaleFactor: textScaleFactor,
                        sizeWithOverflow: resolvedscreenSize
                    );
                }
            };
        }
        _sliderTheme.rangeThumbShape!.paint(
            context,
            topThumbCenter,
            activationAnimation: _overlayAnimation,
            enableAnimation: _enableAnimation,
            isDiscrete: isDiscrete,
            isOnTop: thumbDelta
                < sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete).width,
            textDirection: textDirection,
            sliderTheme: ((thumbWidth is not null) && (thumbHeight is not null))
                ? _sliderTheme.copyWith(
                    thumbSize: new WidgetStatePropertyAll<Size?>(
                        new Size(
                            (
                                thumbWidth
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ),
                            (
                                thumbHeight
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    )
                )
                : _sliderTheme,
            thumb: topThumb,
            isPressed: Equals(topThumb, Thumb.start) ? startThumbSelected : endThumbSelected
        );
    }

    internal virtual SemanticsConfiguration _createSemanticsConfiguration(
        double value,
        double increasedValue,
        double decreasedValue,
        Action increaseAction,
        Action decreaseAction,
        bool focused
    )
    {
        var config = new SemanticsConfiguration();
        config.isEnabled = isEnabled;
        config.textDirection = textDirection;
        config.isSlider = true;
        config.isFocusable = true;
        config.isFocused = focused;
        if (isEnabled)
        {
            config.onIncrease = increaseAction;
            config.onDecrease = decreaseAction;
        }
        if (semanticFormatterCallback is not null)
        {
            config.value = semanticFormatterCallback!(_state._lerp(((value))));
            config.increasedValue = semanticFormatterCallback!(_state._lerp(increasedValue));
            config.decreasedValue = semanticFormatterCallback!(_state._lerp(decreasedValue));
        }
        else
        {
            config.value = $"{((value) * 100L).round()}%";
            config.increasedValue = $"{(increasedValue * 100L).round()}%";
            config.decreasedValue = $"{(decreasedValue * 100L).round()}%";
        }
        return config;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void assembleSemanticsNode(
        SemanticsNode node,
        SemanticsConfiguration config,
        IEnumerable<SemanticsNode> children
    )
    {
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(children));
        SemanticsConfiguration startSemanticsConfiguration = _createSemanticsConfiguration(
            values.start,
            _increasedStartValue,
            _decreasedStartValue,
            () => _increaseStartAction(),
            () => _decreaseStartAction(),
            focused: _state.startFocusNode.hasFocus
        );
        SemanticsConfiguration endSemanticsConfiguration = _createSemanticsConfiguration(
            values.end,
            _increasedEndValue,
            _decreasedEndValue,
            () => _increaseEndAction(),
            () => _decreaseEndAction(),
            focused: _state.endFocusNode.hasFocus
        );
        var leftRect = Rect.fromCenter(
            center: _startThumbCenter,
            width: Widgets.ConstantsLibrary.kMinInteractiveDimension,
            height: Widgets.ConstantsLibrary.kMinInteractiveDimension
        );
        var rightRect = Rect.fromCenter(
            center: _endThumbCenter,
            width: Widgets.ConstantsLibrary.kMinInteractiveDimension,
            height: Widgets.ConstantsLibrary.kMinInteractiveDimension
        );
        _startSemanticsNode ??= new SemanticsNode();
        _endSemanticsNode ??= new SemanticsNode();
        switch (textDirection)
        {
            case TextDirection.ltr:
            {
                _startSemanticsNode!.rect = leftRect;
                _endSemanticsNode!.rect = rightRect;
                break;
            }
            case TextDirection.rtl:
            {
                _startSemanticsNode!.rect = rightRect;
                _endSemanticsNode!.rect = leftRect;
                break;
            }
        }
        _startSemanticsNode!.updateWith(config: startSemanticsConfiguration);
        _endSemanticsNode!.updateWith(config: endSemanticsConfiguration);
        var finalChildren = new List<SemanticsNode> { _startSemanticsNode!, _endSemanticsNode! };
        node.updateWith(config: config, childrenInInversePaintOrder: finalChildren);
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _startSemanticsNode = null;
        _endSemanticsNode = null;
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
    }

    internal virtual double _semanticActionUnit =>
        (divisions is not null)
            ? (
                1.0
                / (
                    divisions
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
            : _adjustmentUnit;

    internal virtual void _increaseStartAction()
    {
        if (isEnabled)
        {
            onChanged!(new RangeValues(_increasedStartValue, values.end));
        }
    }

    internal virtual void _decreaseStartAction()
    {
        if (isEnabled)
        {
            onChanged!(new RangeValues(_decreasedStartValue, values.end));
        }
    }

    internal virtual void _increaseEndAction()
    {
        if (isEnabled)
        {
            onChanged!(new RangeValues(values.start, _increasedEndValue));
        }
    }

    internal virtual void _decreaseEndAction()
    {
        if (isEnabled)
        {
            onChanged!(new RangeValues(values.start, _decreasedEndValue));
        }
    }

    internal virtual double _increasedStartValue
    {
        get
        {
            double increasedStartValue = double.Parse(
                (values.start + _semanticActionUnit).toStringAsFixed(2L),
                System.Globalization.CultureInfo.InvariantCulture
            );
            return (increasedStartValue <= (values.end - _minThumbSeparationValue))
                ? increasedStartValue
                : values.start;
        }
    }
    internal virtual double _decreasedStartValue
    {
        get { return DorotiUiLibrary.clampDouble(values.start - _semanticActionUnit, 0.0, 1.0); }
    }
    internal virtual double _increasedEndValue
    {
        get { return DorotiUiLibrary.clampDouble(values.end + _semanticActionUnit, 0.0, 1.0); }
    }
    internal virtual double _decreasedEndValue
    {
        get
        {
            double decreasedEndValue = values.end - _semanticActionUnit;
            return (decreasedEndValue >= (values.start + _minThumbSeparationValue))
                ? decreasedEndValue
                : values.end;
        }
    }

    public virtual void _scheduleSystemFontsUpdate()
    {
        if (_hasPendingSystemFontsDidChangeCallBack)
        {
            return;
        }
        _hasPendingSystemFontsDidChangeCallBack = true;
        Scheduler.SchedulerBinding.instance.scheduleFrameCallback(
            (timeStamp) =>
            {
                DartRuntimePrimitives.Assert(() => _hasPendingSystemFontsDidChangeCallBack);
                _hasPendingSystemFontsDidChangeCallBack = false;
                DartRuntimePrimitives.Assert(
                    () => attached || (debugDisposed ?? true),
                    () =>
                        (object?)
                            $"{this} is detached during {Scheduler.SchedulerBinding.instance.schedulerPhase} but is not disposed."
                );
                if (attached)
                {
                    systemFontsDidChange();
                }
            }
        );
    }
}

internal class _ValueIndicatorRenderObjectWidget__range_slider : LeafRenderObjectWidget
{
    public virtual _RangeSliderState__range_slider state { get; private set; } = default!;

    internal _ValueIndicatorRenderObjectWidget__range_slider(_RangeSliderState__range_slider state)
    {
        this.state = state;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderValueIndicator__range_slider(state: state);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderValueIndicator__range_slider)renderObject;
        __renderObject._state = state;
    }
}

public class _RenderValueIndicator__range_slider : RenderBox, RelayoutWhenSystemFontsChangeMixin
{
    internal virtual CurvedAnimation _valueIndicatorAnimation { get; set; } = default!;
    internal virtual _RangeSliderState__range_slider _state { get; set; } = default!;
    public virtual bool _hasPendingSystemFontsDidChangeCallBack { get; set; } = false;

    internal _RenderValueIndicator__range_slider(_RangeSliderState__range_slider state)
    {
        _state = state;
        _valueIndicatorAnimation = new CurvedAnimation(
            parent: _state.valueIndicatorController,
            curve: Curves.fastOutSlowIn
        );
    }

    public override bool sizedByParent => true;

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.addListener(_scheduleSystemFontsUpdate);
        _valueIndicatorAnimation.addListener(markNeedsPaint);
        _state.startPositionController.addListener(markNeedsPaint);
        _state.endPositionController.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _valueIndicatorAnimation.removeListener(markNeedsPaint);
        _state.startPositionController.removeListener(markNeedsPaint);
        _state.endPositionController.removeListener(markNeedsPaint);
        DartRuntimePrimitives.Assert(() => !_hasPendingSystemFontsDidChangeCallBack);
        PaintingBinding.instance.systemFonts.removeListener(_scheduleSystemFontsUpdate);
        base.detach();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _state.paintBottomValueIndicator?.Invoke(context, offset);
        _state.paintTopValueIndicator?.Invoke(context, offset);
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.smallest;
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        Scheduler.SchedulerBinding.instance.scheduleFrameCallback(
            (timeStamp) =>
            {
                DartRuntimePrimitives.Assert(() => _hasPendingSystemFontsDidChangeCallBack);
                _hasPendingSystemFontsDidChangeCallBack = false;
                DartRuntimePrimitives.Assert(
                    () => attached || (debugDisposed ?? true),
                    () =>
                        (object?)
                            $"{this} is detached during {Scheduler.SchedulerBinding.instance.schedulerPhase} but is not disposed."
                );
                if (attached)
                {
                    systemFontsDidChange();
                }
            }
        );
    }
}

// The 2023 Material 3 range slider retains its original shape and color defaults.

internal class _RangeSliderDefaultsM3Year2023__range_slider : SliderThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
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
    private bool __late_sliderTheme_initialized;
    private SliderThemeData __late_sliderTheme = default!;
    public virtual SliderThemeData sliderTheme
    {
        get
        {
            if (!__late_sliderTheme_initialized)
            {
                __late_sliderTheme = SliderTheme.of(context);
                __late_sliderTheme_initialized = true;
            }
            return __late_sliderTheme;
        }
    }

    internal _RangeSliderDefaultsM3Year2023__range_slider(BuildContext context)
        : base(trackHeight: 4)
    {
        this.context = context;
    }

    public override Color? activeTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? inactiveTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary.withOpacity(0.24));
    public override Color? disabledActiveTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface.withOpacity(0.32));
    public override Color? disabledInactiveTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface.withOpacity(0.12));
    public override Color? activeTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onPrimary.withOpacity(0.54));
    public override Color? inactiveTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary.withOpacity(0.54));
    public override Color? disabledActiveTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onPrimary.withOpacity(0.12));
    public override Color? disabledInactiveTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface.withOpacity(0.12));
    public override Color? thumbColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? overlappingShapeStrokeColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surface);
    public override Color? disabledThumbColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            DorotiUiLibrary.Color.alphaBlend(_colors.onSurface.withOpacity(0.38), _colors.surface)
        );
    public override Color? overlayColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary.withOpacity(0.12));
    public override TextStyle? valueIndicatorTextStyle =>
        Theme.of(context).textTheme.bodyLarge!.copyWith(color: _colors.onPrimary);
    public override Color? valueIndicatorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override RangeSliderTrackShape? rangeTrackShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderTrackShape>(
            new RoundedRectRangeSliderTrackShape()
        );
    public override RangeSliderTickMarkShape? rangeTickMarkShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderTickMarkShape>(
            new RoundRangeSliderTickMarkShape()
        );
    public override RangeSliderThumbShape? rangeThumbShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderThumbShape>(new RoundRangeSliderThumbShape());
    public override SliderComponentShape? overlayShape =>
        DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override RangeSliderValueIndicatorShape? rangeValueIndicatorShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderValueIndicatorShape>(
            new RectangularRangeSliderValueIndicatorShape()
        );
    public override ShowValueIndicator? showValueIndicator => ShowValueIndicator.onlyForDiscrete;
    public override double? minThumbSeparation => 8;
}

internal class _RangeSliderDefaultsM3__range_slider : SliderThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
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

    internal _RangeSliderDefaultsM3__range_slider(BuildContext context)
        : base(trackHeight: 16.0)
    {
        this.context = context;
    }

    public override Color? activeTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? inactiveTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.secondaryContainer);
    public override Color? disabledActiveTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface.withOpacity(0.38));
    public override Color? disabledInactiveTrackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface.withOpacity(0.12));
    public override Color? activeTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onPrimary.withOpacity(1.0));
    public override Color? inactiveTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSecondaryContainer.withOpacity(1.0));
    public override Color? disabledActiveTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onInverseSurface);
    public override Color? disabledInactiveTickMarkColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface);
    public override Color? thumbColor => DartRuntimePrimitives.ConvertValue<Color>(_colors.primary);
    public override Color? overlappingShapeStrokeColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.surface);
    public override Color? disabledThumbColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.onSurface.withOpacity(0.38));
    public override Color? overlayColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.primary.withOpacity(0.12));
    public override TextStyle? valueIndicatorTextStyle =>
        Theme.of(context).textTheme.labelLarge!.copyWith(color: _colors.onInverseSurface);
    public override Color? valueIndicatorColor =>
        DartRuntimePrimitives.ConvertValue<Color>(_colors.inverseSurface);
    public override RangeSliderTrackShape? rangeTrackShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderTrackShape>(
            new GappedRangeSliderTrackShape()
        );
    public override RangeSliderTickMarkShape? rangeTickMarkShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderTickMarkShape>(
            new RoundRangeSliderTickMarkShape(tickMarkRadius: 4.0 / 2L)
        );
    public override RangeSliderThumbShape? rangeThumbShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderThumbShape>(
            new HandleRangeSliderThumbShape()
        );
    public override SliderComponentShape? overlayShape =>
        DartRuntimePrimitives.ConvertValue<SliderComponentShape>(new RoundSliderOverlayShape());
    public override RangeSliderValueIndicatorShape? rangeValueIndicatorShape =>
        DartRuntimePrimitives.ConvertValue<RangeSliderValueIndicatorShape>(
            new RoundedRectRangeSliderValueIndicatorShape()
        );
    public override ShowValueIndicator? showValueIndicator => ShowValueIndicator.onlyForDiscrete;
    public override double? minThumbSeparation => 0;
    public override WidgetStateProperty<Size?>? thumbSize
    {
        get
        {
            return (WidgetStateProperty<Size?>?)
                WidgetStateProperty.resolveWith(
                    (states) =>
                    {
                        if (states.Contains(WidgetState.disabled))
                        {
                            return new Size(4.0, 44.0);
                        }
                        if (states.Contains(WidgetState.hovered))
                        {
                            return new Size(4.0, 44.0);
                        }
                        if (states.Contains(WidgetState.focused))
                        {
                            return new Size(2.0, 44.0);
                        }
                        if (states.Contains(WidgetState.pressed))
                        {
                            return new Size(2.0, 44.0);
                        }
                        return new Size(4.0, 44.0);
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                );
        }
    }
    public override double? trackGap => 6.0;
}

// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/slider.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

internal delegate void _SliderValueChanged__slider(double value, bool isFastDrag);

public static partial class SliderLibrary
{
    internal static double _kVelocityThreshold = 1.0;
}

public class CupertinoSlider : StatefulWidget
{
    public virtual double value { get; private set; } = default!;
    public virtual Action<double>? onChanged { get; private set; }
    public virtual Action<double>? onChangeStart { get; private set; }
    public virtual Action<double>? onChangeEnd { get; private set; }
    public virtual double min { get; private set; } = default!;
    public virtual double max { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual Color? activeColor { get; private set; }
    public virtual Color thumbColor { get; private set; } = default!;

    public CupertinoSlider(
        Key? key = null,
        double value = default!,
        Action<double>? onChanged = default!,
        Action<double>? onChangeStart = null,
        Action<double>? onChangeEnd = null,
        double min = 0.0,
        double max = 1.0,
        long? divisions = null,
        Color? activeColor = null,
        Color thumbColor = default!
    )
        : base(key: key)
    {
        Color __thumbColor = thumbColor ?? CupertinoColors.white;
        this.value = value;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.min = min;
        this.max = max;
        this.divisions = divisions;
        this.activeColor = activeColor;
        this.thumbColor = __thumbColor;
        System.Diagnostics.Debug.Assert((value >= min) && (value <= max));
        System.Diagnostics.Debug.Assert(
            (divisions is null) || (DartRuntimePrimitives.RequireValue(divisions) > 0L)
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSliderState__slider());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("value", value));
        properties.add(new DoubleProperty("min", min));
        properties.add(new DoubleProperty("max", max));
    }
}

internal class _CupertinoSliderState__slider
    : State<CupertinoSlider>,
        TickerProviderStateMixin<CupertinoSlider>
{
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual void _handleChanged(double value, bool isFastDrag)
    {
        DartRuntimePrimitives.Assert(() => widget.onChanged is not null);
        double lerpValue = DartRuntimePrimitives.RequireValue(
            Dart_uiLibrary.lerpDouble(widget.min, widget.max, value)
        );
        bool isAtEdge = (lerpValue == widget.max) || (lerpValue == widget.min);
        if (lerpValue != widget.value)
        {
            if (isAtEdge)
            {
                _emitHapticFeedback(isFastDrag);
            }
            widget.onChanged!(lerpValue);
        }
    }

    internal virtual void _handleDragStart(double value)
    {
        DartRuntimePrimitives.Assert(() => widget.onChangeStart is not null);
        widget.onChangeStart!(
            DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(widget.min, widget.max, value)
            )
        );
    }

    internal virtual void _handleDragEnd(double value)
    {
        DartRuntimePrimitives.Assert(() => widget.onChangeEnd is not null);
        widget.onChangeEnd!(
            DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(widget.min, widget.max, value)
            )
        );
    }

    internal virtual void _emitHapticFeedback(bool isFastDrag)
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            {
                if (isFastDrag)
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.mediumImpact());
                }
                else
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
                }
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                break;
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        return new _CupertinoSliderRenderObjectWidget__slider(
            value: (widget.value - widget.min) / (widget.max - widget.min),
            divisions: widget.divisions,
            activeColor: CupertinoDynamicColor.resolve(
                widget.activeColor ?? CupertinoTheme.of(context).primaryColor,
                context
            ),
            thumbColor: widget.thumbColor,
            onChanged: (widget.onChanged is not null) ? _handleChanged : null,
            onChangeStart: (widget.onChangeStart is not null) ? _handleDragStart : null,
            onChangeEnd: (widget.onChangeEnd is not null) ? _handleDragEnd : null,
            vsync: this
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public override void dispose()
    {
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

internal class _CupertinoSliderRenderObjectWidget__slider : LeafRenderObjectWidget
{
    public virtual double value { get; private set; } = default!;
    public virtual long? divisions { get; private set; }
    public virtual Color activeColor { get; private set; } = default!;
    public virtual Color thumbColor { get; private set; } = default!;
    public virtual Action<double, bool>? onChanged { get; private set; }
    public virtual Action<double>? onChangeStart { get; private set; }
    public virtual Action<double>? onChangeEnd { get; private set; }
    public virtual Scheduler.TickerProvider vsync { get; private set; } = default!;

    internal _CupertinoSliderRenderObjectWidget__slider(
        double value,
        long? divisions = null,
        Color activeColor = default!,
        Color thumbColor = default!,
        Action<double, bool>? onChanged = null,
        Action<double>? onChangeStart = null,
        Action<double>? onChangeEnd = null,
        Scheduler.TickerProvider vsync = default!
    )
    {
        this.value = value;
        this.divisions = divisions;
        this.activeColor = activeColor;
        this.thumbColor = thumbColor;
        this.onChanged = onChanged;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        this.vsync = vsync;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        return new _RenderCupertinoSlider__slider(
            value: value,
            divisions: divisions,
            activeColor: activeColor,
            thumbColor: CupertinoDynamicColor.resolve(thumbColor, context),
            trackColor: CupertinoDynamicColor.resolve(CupertinoColors.systemFill, context),
            onChanged: onChanged,
            onChangeStart: onChangeStart,
            onChangeEnd: onChangeEnd,
            vsync: vsync,
            textDirection: Directionality.of(context),
            cursor: Foundation.ConstantsLibrary.kIsWeb
                ? SystemMouseCursors.click
                : MouseCursor.defer
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoSlider__slider)renderObject;
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderCupertinoSlider__slider>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.value = value;
                        __cascade.divisions = divisions;
                        __cascade.activeColor = activeColor;
                        __cascade.thumbColor = CupertinoDynamicColor.resolve(thumbColor, context);
                        __cascade.trackColor = CupertinoDynamicColor.resolve(
                            CupertinoColors.systemFill,
                            context
                        );
                        __cascade.onChanged = onChanged;
                        __cascade.onChangeStart = onChangeStart;
                        __cascade.onChangeEnd = onChangeEnd;
                        __cascade.textDirection = Directionality.of(context);
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public static partial class SliderLibrary
{
    internal static double _kPadding = 8.0;
}

public static partial class SliderLibrary
{
    internal static double _kSliderHeight = 2.0 * (CupertinoThumbPainter.radius + _kPadding);
}

public static partial class SliderLibrary
{
    internal static double _kSliderWidth = 176.0;
}

public static partial class SliderLibrary
{
    internal static Duration _kDiscreteTransitionDuration = Duration.Create(milliseconds: 500L);
}

public static partial class SliderLibrary
{
    internal static double _kAdjustmentUnit = 0.1;
}

public class _RenderCupertinoSlider__slider : RenderConstrainedBox
{
    internal virtual double _value { get; set; } = default!;
    internal virtual long? _divisions { get; set; } = default;
    internal virtual Color _activeColor { get; set; } = default!;
    internal virtual Color _thumbColor { get; set; } = default!;
    internal virtual Color _trackColor { get; set; } = default!;
    internal virtual Action<double, bool>? _onChanged { get; set; } = default;
    public virtual Action<double>? onChangeStart { get; set; } = default;
    public virtual Action<double>? onChangeEnd { get; set; } = default;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual AnimationController _position { get; set; } = default!;
    internal virtual Gestures.HorizontalDragGestureRecognizer _drag { get; set; } = default!;
    internal virtual double _currentDragValue { get; set; } = 0.0;
    internal virtual Duration? _lastUpdateTimestamp { get; set; } = default;
    internal virtual MouseCursor _cursor { get; set; } = default!;
    public virtual Action<Gestures.PointerEnterEvent>? onEnter { get; set; } = default;
    public virtual Action<Gestures.PointerHoverEvent>? onHover { get; set; } = default;
    public virtual Action<Gestures.PointerExitEvent>? onExit { get; set; } = default;

    internal _RenderCupertinoSlider__slider(
        double value,
        long? divisions = null,
        Color activeColor = default!,
        Color thumbColor = default!,
        Color trackColor = default!,
        Action<double, bool>? onChanged = null,
        Action<double>? onChangeStart = null,
        Action<double>? onChangeEnd = null,
        Scheduler.TickerProvider vsync = default!,
        TextDirection textDirection = default!,
        MouseCursor cursor = default!
    )
        : base(
            additionalConstraints: BoxConstraints.CreateTightFor(
                width: SliderLibrary._kSliderWidth,
                height: SliderLibrary._kSliderHeight
            )
        )
    {
        MouseCursor __cursor = cursor ?? MouseCursor.defer;
        this.onChangeStart = onChangeStart;
        this.onChangeEnd = onChangeEnd;
        _cursor = __cursor;
        _value = DartRuntimePrimitives.RequireValue(value);
        _divisions = divisions;
        _activeColor = activeColor;
        _thumbColor = thumbColor;
        _trackColor = trackColor;
        _onChanged = onChanged;
        _textDirection = textDirection;
        System.Diagnostics.Debug.Assert((value >= 0.0) && (value <= 1.0));
        _drag = (
            (Func<Gestures.HorizontalDragGestureRecognizer>)(
                () =>
                {
                    var __cascade = new Gestures.HorizontalDragGestureRecognizer();
                    __cascade.onStart = _handleDragStart;
                    __cascade.onUpdate = _handleDragUpdate;
                    __cascade.onEnd = _handleDragEnd;
                    return __cascade;
                }
            )
        )();
        _position = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        value: DartRuntimePrimitives.RequireValue(value),
                        duration: SliderLibrary._kDiscreteTransitionDuration,
                        vsync: vsync
                    );
                    __cascade.addListener(markNeedsPaint);
                    return __cascade;
                }
            )
        )();
    }

    public virtual double value
    {
        get => _value;
        set
        {
            var newValue = value;
            DartRuntimePrimitives.Assert(() => (newValue >= 0.0) && (newValue <= 1.0));
            if (newValue == _value)
            {
                return;
            }
            _value = newValue;
            if (divisions is not null)
            {
                long divisions__value13358 = DartRuntimePrimitives.RequireValue(divisions);
                _position.animateTo(newValue, curve: Curves.fastOutSlowIn);
            }
            else
            {
                _position.value = newValue;
            }
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
    public virtual Color activeColor
    {
        get => _activeColor;
        set
        {
            var __value = value;
            if (Equals(__value, _activeColor))
            {
                return;
            }
            _activeColor = __value;
            markNeedsPaint();
        }
    }
    public virtual Color thumbColor
    {
        get => _thumbColor;
        set
        {
            var __value = value;
            if (Equals(__value, _thumbColor))
            {
                return;
            }
            _thumbColor = __value;
            markNeedsPaint();
        }
    }
    public virtual Color trackColor
    {
        get => _trackColor;
        set
        {
            var __value = value;
            if (Equals(__value, _trackColor))
            {
                return;
            }
            _trackColor = __value;
            markNeedsPaint();
        }
    }
    public virtual Action<double, bool>? onChanged
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
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsPaint();
        }
    }
    internal virtual double _discretizedCurrentDragValue
    {
        get
        {
            double dragValue = Dart_uiLibrary.clampDouble(_currentDragValue, 0.0, 1.0);
            if (divisions is not null)
            {
                long divisions__value15208 = DartRuntimePrimitives.RequireValue(divisions);
                dragValue =
                    (dragValue * DartRuntimePrimitives.RequireValue(divisions)).round()
                    / DartRuntimePrimitives.RequireValue(divisions);
            }
            return dragValue;
        }
    }
    internal virtual double _trackLeft => SliderLibrary._kPadding;
    internal virtual double _trackRight =>
        DartRuntimePrimitives.ConvertValue<double>(size.width - SliderLibrary._kPadding);
    internal virtual double _thumbCenter
    {
        get
        {
            double visualPosition = textDirection switch
            {
                TextDirection.rtl => 1.0 - _value,
                TextDirection.ltr => _value,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            return DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(
                    _trackLeft + CupertinoThumbPainter.radius,
                    _trackRight - CupertinoThumbPainter.radius,
                    visualPosition
                )
            );
        }
    }
    public virtual bool isInteractive =>
        DartRuntimePrimitives.ConvertValue<bool>(onChanged is not null);

    internal virtual void _handleDragStart(Gestures.DragStartDetails details) =>
        _startInteraction(details);

    internal virtual void _handleDragUpdate(Gestures.DragUpdateDetails details)
    {
        if (!isInteractive)
        {
            return;
        }
        double extent = Math.Max(
            SliderLibrary._kPadding,
            size.width - (2.0 * (SliderLibrary._kPadding + CupertinoThumbPainter.radius))
        );
        double valueDelta = DartRuntimePrimitives.RequireValue(details.primaryDelta) / extent;
        _currentDragValue += textDirection switch
        {
            TextDirection.rtl => -valueDelta,
            TextDirection.ltr => valueDelta,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        var isFast = false;
        Duration? currentTimestamp = details.sourceTimeStamp;
        if ((currentTimestamp is not null) && (_lastUpdateTimestamp is not null))
        {
            Duration currentTimestamp__16442__value16494 = DartRuntimePrimitives.RequireValue(
                currentTimestamp
            );
            long timeDelta = (
                DartRuntimePrimitives.RequireValue(currentTimestamp__16442__value16494)
                - DartRuntimePrimitives.RequireValue(_lastUpdateTimestamp)
            ).inMilliseconds;
            double velocity = valueDelta.abs() * 1000.0 / timeDelta;
            isFast = velocity > SliderLibrary._kVelocityThreshold;
        }
        _lastUpdateTimestamp = currentTimestamp;
        onChanged!(_discretizedCurrentDragValue, isFast);
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details) => _endInteraction();

    internal virtual void _startInteraction(Gestures.DragStartDetails details)
    {
        if (isInteractive)
        {
            onChangeStart?.Invoke(_discretizedCurrentDragValue);
            _currentDragValue = _value;
            _lastUpdateTimestamp = details.sourceTimeStamp;
            onChanged!(_discretizedCurrentDragValue, false);
        }
    }

    internal virtual void _endInteraction()
    {
        onChangeEnd?.Invoke(_discretizedCurrentDragValue);
        _currentDragValue = 0.0;
        _lastUpdateTimestamp = null;
    }

    public override bool hitTestSelf(Offset position)
    {
        return (position.dx - _thumbCenter).abs()
            < (CupertinoThumbPainter.radius + SliderLibrary._kPadding);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(
        Gestures.PointerEvent @event,
        Gestures.HitTestEntry<Gestures.HitTestTarget> entry
    )
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is Gestures.PointerDownEvent) && isInteractive)
        {
            Gestures.PointerDownEvent @event__as17793 = (Gestures.PointerDownEvent)@event;
            _drag.addPointer(@event__as17793);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        var (visualPosition, leftColor, rightColor) = textDirection switch
        {
            TextDirection.rtl => ((double, Color, Color))
                (1.0 - _position.value, _activeColor, trackColor),
            TextDirection.ltr => ((double, Color, Color))
                (_position.value, trackColor, _activeColor),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        double trackCenter = offset.dy + (size.height / 2.0);
        double trackLeft = offset.dx + _trackLeft;
        double trackTop = trackCenter - 1.0;
        double trackBottom = trackCenter + 1.0;
        double trackRight = offset.dx + _trackRight;
        double trackActive = offset.dx + _thumbCenter;
        Canvas canvasLocal = context.canvas;
        if (visualPosition > 0.0)
        {
            var paintLocal = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = rightColor;
                        return __cascade;
                    }
                )
            )();
            canvasLocal.drawRRect(
                RRect.fromLTRBXY(trackLeft, trackTop, trackActive, trackBottom, 1.0, 1.0),
                paintLocal
            );
        }
        if (visualPosition < 1.0)
        {
            var paintAlternate = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = leftColor;
                        return __cascade;
                    }
                )
            )();
            canvasLocal.drawRRect(
                RRect.fromLTRBXY(trackActive, trackTop, trackRight, trackBottom, 1.0, 1.0),
                paintAlternate
            );
        }
        var thumbCenter = new Offset(trackActive, trackCenter);
        new CupertinoThumbPainter(color: thumbColor).paint(
            canvasLocal,
            Rect.fromCircle(center: thumbCenter, radius: CupertinoThumbPainter.radius)
        );
    }

    public override void describeSemanticsConfiguration(Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = isInteractive;
        config.isSlider = true;
        if (isInteractive)
        {
            config.textDirection = textDirection;
            config.onIncrease = _increaseAction;
            config.onDecrease = _decreaseAction;
            config.value = $"{(value * 100L).round()}%";
            config.increasedValue =
                $"{(Dart_uiLibrary.clampDouble(value + _semanticActionUnit, 0.0, 1.0) * 100L).round()}%";
            config.decreasedValue =
                $"{(Dart_uiLibrary.clampDouble(value - _semanticActionUnit, 0.0, 1.0) * 100L).round()}%";
        }
    }

    internal virtual double _semanticActionUnit =>
        (divisions is not null)
            ? (1.0 / DartRuntimePrimitives.RequireValue(divisions))
            : SliderLibrary._kAdjustmentUnit;

    internal virtual void _increaseAction()
    {
        if (isInteractive)
        {
            onChanged!(Dart_uiLibrary.clampDouble(value + _semanticActionUnit, 0.0, 1.0), false);
        }
    }

    internal virtual void _decreaseAction()
    {
        if (isInteractive)
        {
            onChanged!(Dart_uiLibrary.clampDouble(value - _semanticActionUnit, 0.0, 1.0), false);
        }
    }

    public virtual MouseCursor cursor
    {
        get => _cursor;
        set
        {
            var __value = value;
            if (!Equals(_cursor, __value))
            {
                _cursor = __value;
                markNeedsPaint();
            }
        }
    }
    public virtual bool validForMouseTracker => false;

    public override void dispose()
    {
        _drag.dispose();
        _position.dispose();
        base.dispose();
    }
}

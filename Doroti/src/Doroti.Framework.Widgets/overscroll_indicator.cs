// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/overscroll_indicator.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class GlowingOverscrollIndicator : StatefulWidget
{
    public virtual bool showLeading { get; private set; } = default!;
    public virtual bool showTrailing { get; private set; } = default!;
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual Func<ScrollNotification, bool> notificationPredicate { get; private set; } =
        default!;
    public virtual Widget? child { get; private set; }

    public GlowingOverscrollIndicator(
        Key? key = null,
        bool showLeading = true,
        bool showTrailing = true,
        AxisDirection axisDirection = default!,
        Color color = default!,
        Func<ScrollNotification, bool> notificationPredicate = default!,
        Widget? child = null
    )
        : base(key: key)
    {
        Func<ScrollNotification, bool> __notificationPredicate =
            notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        this.showLeading = showLeading;
        this.showTrailing = showTrailing;
        this.axisDirection = axisDirection;
        this.color = color;
        this.notificationPredicate = __notificationPredicate;
        this.child = child;
    }

    public virtual Axis axis => Basic_typesLibrary.axisDirectionToAxis(axisDirection);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _GlowingOverscrollIndicatorState__overscroll_indicator()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<AxisDirection>("axisDirection", axisDirection));
        string showDescription = (showLeading, showTrailing) switch
        {
            (true, true) => "both sides",
            (true, false) => "leading side only",
            (false, true) => "trailing side only",
            (false, false) => "neither side (!)",
        };
        properties.add(new MessageProperty("show", showDescription));
        properties.add(new ColorProperty("color", color, showName: false));
    }
}

internal class _GlowingOverscrollIndicatorState__overscroll_indicator
    : State<GlowingOverscrollIndicator>,
        TickerProviderStateMixin<GlowingOverscrollIndicator>
{
    internal virtual _GlowController__overscroll_indicator? _leadingController { get; set; } =
        default;
    internal virtual _GlowController__overscroll_indicator? _trailingController { get; set; } =
        default;
    internal virtual Listenable? _leadingAndTrailingListener { get; set; } = default;
    internal virtual Type? _lastNotificationType { get; set; } = default;
    internal virtual DartMap<bool, bool> _accepted { get; private set; } =
        new DartMap<bool, bool> { [false] = true, [true] = true };
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _leadingController = new _GlowController__overscroll_indicator(
            vsync: this,
            color: widget.color,
            axis: widget.axis
        );
        _trailingController = new _GlowController__overscroll_indicator(
            vsync: this,
            color: widget.color,
            axis: widget.axis
        );
        _leadingAndTrailingListener = Listenable.CreateMerge(
            new List<Listenable> { _leadingController!, _trailingController! }.Cast<Listenable?>()
        );
    }

    public override void didUpdateWidget(GlowingOverscrollIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(oldWidget.color, widget.color)) || (!Equals(oldWidget.axis, widget.axis)))
        {
            _leadingController!.color = widget.color;
            _leadingController!.axis = widget.axis;
            _trailingController!.color = widget.color;
            _trailingController!.axis = widget.axis;
        }
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if (!widget.notificationPredicate(notification))
        {
            return false;
        }
        if (!Equals(notification.metrics.axis, widget.axis))
        {
            return false;
        }
        _leadingController!._paintOffsetScrollPixels = -Math.Min(
            notification.metrics.pixels - notification.metrics.minScrollExtent,
            _leadingController!._paintOffset
        );
        _trailingController!._paintOffsetScrollPixels = -Math.Min(
            notification.metrics.maxScrollExtent - notification.metrics.pixels,
            _trailingController!._paintOffset
        );
        if (notification is OverscrollNotification)
        {
            OverscrollNotification notification__as9386 = (OverscrollNotification)notification;
            _GlowController__overscroll_indicator? controller = default!;
            if (notification__as9386.overscroll < 0.0)
            {
                controller = _leadingController;
            }
            else
            {
                if (notification__as9386.overscroll > 0.0)
                {
                    controller = _trailingController;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => false);
                }
            }
            var isLeading = Equals(controller, _leadingController);
            if (!Equals(_lastNotificationType, typeof(OverscrollNotification)))
            {
                var confirmationNotification = new OverscrollIndicatorNotification(
                    leading: isLeading
                );
                confirmationNotification.dispatch(context);
                _accepted[isLeading] = confirmationNotification.accepted;
                if (
                    (
                        DartCollectionRuntime.NullableMapValue<bool>(_accepted, isLeading)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
                {
                    controller!._paintOffset = confirmationNotification.paintOffset;
                }
            }
            DartRuntimePrimitives.Assert(() => controller is not null);
            if (
                (
                    DartCollectionRuntime.NullableMapValue<bool>(_accepted, isLeading)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            )
            {
                if (notification__as9386.velocity != 0.0)
                {
                    DartRuntimePrimitives.Assert(() => notification__as9386.dragDetails is null);
                    controller!.absorbImpact(notification__as9386.velocity.abs());
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => notification__as9386.overscroll != 0.0);
                    if (notification__as9386.dragDetails is not null)
                    {
                        var renderer = (
                            (RenderBox?)notification__as9386.context!.findRenderObject()!
                        )!;
                        DartRuntimePrimitives.Assert(() => renderer.hasSize);
                        Size sizeLocal = renderer.size;
                        Offset position = renderer.globalToLocal(
                            notification__as9386.dragDetails!.globalPosition
                        );
                        switch (notification__as9386.metrics.axis)
                        {
                            case Axis.horizontal:
                            {
                                controller!.pull(
                                    notification__as9386.overscroll.abs(),
                                    sizeLocal.width,
                                    Dart_uiLibrary.clampDouble(position.dy, 0.0, sizeLocal.height),
                                    sizeLocal.height
                                );
                                break;
                            }
                            case Axis.vertical:
                            {
                                controller!.pull(
                                    notification__as9386.overscroll.abs(),
                                    sizeLocal.height,
                                    Dart_uiLibrary.clampDouble(position.dx, 0.0, sizeLocal.width),
                                    sizeLocal.width
                                );
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (
                (
                    (notification is ScrollEndNotification)
                    && (((ScrollEndNotification)notification).dragDetails is not null)
                )
                || (
                    (notification is ScrollUpdateNotification)
                    && (((ScrollUpdateNotification)notification).dragDetails is not null)
                )
            )
            {
                _leadingController!.scrollEnd();
                _trailingController!.scrollEnd();
            }
        }
        _lastNotificationType = DartRuntimePrimitives.RuntimeType(notification);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _leadingController!.dispose();
        _trailingController!.dispose();
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
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: _handleScrollNotification,
            child: new RepaintBoundary(
                child: new CustomPaint(
                    foregroundPainter: new _GlowingOverscrollIndicatorPainter__overscroll_indicator(
                        leadingController: widget.showLeading ? _leadingController : null,
                        trailingController: widget.showTrailing ? _trailingController : null,
                        axisDirection: widget.axisDirection,
                        repaint: _leadingAndTrailingListener
                    ),
                    child: new RepaintBoundary(child: widget.child)
                )
            )
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

internal enum _GlowState__overscroll_indicator
{
    idle,
    absorb,
    pull,
    recede,
}

public class _GlowController__overscroll_indicator : ChangeNotifier
{
    internal virtual _GlowState__overscroll_indicator _state { get; set; } =
        _GlowState__overscroll_indicator.idle;
    internal virtual AnimationController _glowController { get; private set; } = default!;
    internal virtual Timer? _pullRecedeTimer { get; set; } = default;
    internal virtual double _paintOffset { get; set; } = 0.0;
    internal virtual double _paintOffsetScrollPixels { get; set; } = 0.0;
    internal virtual CurvedAnimation _decelerator { get; private set; } = default!;
    internal virtual Tween<double> _glowOpacityTween { get; private set; } =
        new Tween<double>(begin: 0.0, end: 0.0);
    internal virtual Animation<double> _glowOpacity { get; private set; } = default!;
    internal virtual Tween<double> _glowSizeTween { get; private set; } =
        new Tween<double>(begin: 0.0, end: 0.0);
    internal virtual Animation<double> _glowSize { get; private set; } = default!;
    internal virtual Scheduler.Ticker _displacementTicker { get; private set; } = default!;
    internal virtual Duration? _displacementTickerLastElapsed { get; set; } = default;
    internal virtual double _displacementTarget { get; set; } = 0.5;
    internal virtual double _displacement { get; set; } = 0.5;
    internal virtual double _pullDistance { get; set; } = 0.0;
    internal virtual Color _color { get; set; } = default!;
    internal virtual Axis _axis { get; set; } = default!;
    internal static Duration _recedeTime = Duration.Create(milliseconds: 600L);
    internal static Duration _pullTime = Duration.Create(milliseconds: 167L);
    internal static Duration _pullHoldTime = Duration.Create(milliseconds: 167L);
    internal static Duration _pullDecayTime = Duration.Create(milliseconds: 2000L);
    internal static Duration _crossAxisHalfTime = Duration.Create(
        microseconds: (Duration.microsecondsPerSecond / 60.0).round()
    );
    internal const double _maxOpacity = 0.5;
    internal const double _pullOpacityGlowFactor = 0.8;
    internal const double _velocityGlowFactor = 0.00006;
    internal const double _sqrt3 = 1.73205080757;
    internal static double _widthToHeightFactor = 3.0 / 4.0 * (2.0 - _sqrt3);
    internal const double _minVelocity = 100.0;
    internal const double _maxVelocity = 10000.0;

    internal _GlowController__overscroll_indicator(
        Scheduler.TickerProvider vsync,
        Color color,
        Axis axis
    )
    {
        _color = color;
        _axis = axis;
    }

    public virtual Color color
    {
        get => _color;
        set
        {
            var __value = value;
            if (Equals(color, __value))
            {
                return;
            }
            _color = __value;
            notifyListeners();
        }
    }
    public virtual Axis axis
    {
        get => _axis;
        set
        {
            var __value = value;
            if (Equals(axis, __value))
            {
                return;
            }
            _axis = __value;
            notifyListeners();
        }
    }

    public override void dispose()
    {
        _glowController.dispose();
        _decelerator.dispose();
        _displacementTicker.dispose();
        _pullRecedeTimer?.cancel();
        base.dispose();
    }

    public virtual void absorbImpact(double velocity)
    {
        DartRuntimePrimitives.Assert(() => velocity >= 0.0);
        _pullRecedeTimer?.cancel();
        _pullRecedeTimer = null;
        velocity = Dart_uiLibrary.clampDouble(velocity, _minVelocity, _maxVelocity);
        _glowOpacityTween.begin = Equals(_state, _GlowState__overscroll_indicator.idle)
            ? 0.3
            : _glowOpacity.value;
        _glowOpacityTween.end = Dart_uiLibrary.clampDouble(
            velocity * _velocityGlowFactor,
            (_glowOpacityTween.begin),
            _maxOpacity
        );
        _glowSizeTween.begin = _glowSize.value;
        _glowSizeTween.end = Math.Min(0.025 + (7.5e-7 * velocity * velocity), 1.0);
        _glowController.duration = Duration.Create(
            milliseconds: (0.15 + (velocity * 0.02)).round()
        );
        _glowController.forward(from: 0.0);
        _displacement = 0.5;
        _state = _GlowState__overscroll_indicator.absorb;
    }

    public virtual void pull(
        double overscroll,
        double extent,
        double crossAxisOffset,
        double crossExtent
    )
    {
        _pullRecedeTimer?.cancel();
        _pullDistance += overscroll / 200.0;
        _glowOpacityTween.begin = _glowOpacity.value;
        _glowOpacityTween.end = Math.Min(
            _glowOpacity.value + (overscroll / extent * _pullOpacityGlowFactor),
            _maxOpacity
        );
        double height = Math.Min(extent, crossExtent * _widthToHeightFactor);
        _glowSizeTween.begin = _glowSize.value;
        _glowSizeTween.end = Math.Max(
            1.0 - (1.0 / (0.7 * Dart_mathLibrary.sqrt(_pullDistance * height))),
            _glowSize.value
        );
        _displacementTarget = crossAxisOffset / crossExtent;
        if (_displacementTarget != _displacement)
        {
            if (!_displacementTicker.isTicking)
            {
                DartRuntimePrimitives.Assert(() => _displacementTickerLastElapsed is null);
                _displacementTicker.start();
            }
        }
        else
        {
            _displacementTicker.stop();
            _displacementTickerLastElapsed = null;
        }
        _glowController.duration = _pullTime;
        if (!Equals(_state, _GlowState__overscroll_indicator.pull))
        {
            _glowController.forward(from: 0.0);
            _state = _GlowState__overscroll_indicator.pull;
        }
        else
        {
            if (!_glowController.isAnimating)
            {
                DartRuntimePrimitives.Assert(() => _glowController.value == 1.0);
                notifyListeners();
            }
        }
        _pullRecedeTimer = new Timer(
            _pullHoldTime,
            () =>
            {
                _recede(_pullDecayTime);
            }
        );
    }

    public virtual void scrollEnd()
    {
        if (Equals(_state, _GlowState__overscroll_indicator.pull))
        {
            _recede(_recedeTime);
        }
    }

    internal virtual void _changePhase(AnimationStatus status)
    {
        if (!AnimationStatusMembers.isCompleted(status))
        {
            return;
        }
        switch (_state)
        {
            case _GlowState__overscroll_indicator.absorb:
            {
                _recede(_recedeTime);
                break;
            }
            case _GlowState__overscroll_indicator.recede:
            {
                _state = _GlowState__overscroll_indicator.idle;
                _pullDistance = 0.0;
                break;
            }
            case _GlowState__overscroll_indicator.pull:
            case _GlowState__overscroll_indicator.idle:
            {
                break;
            }
        }
    }

    internal virtual void _recede(Duration duration)
    {
        if (
            Equals(_state, _GlowState__overscroll_indicator.recede)
            || Equals(_state, _GlowState__overscroll_indicator.idle)
        )
        {
            return;
        }
        _pullRecedeTimer?.cancel();
        _pullRecedeTimer = null;
        _glowOpacityTween.begin = _glowOpacity.value;
        _glowOpacityTween.end = 0.0;
        _glowSizeTween.begin = _glowSize.value;
        _glowSizeTween.end = 0.0;
        _glowController.duration = duration;
        _glowController.forward(from: 0.0);
        _state = _GlowState__overscroll_indicator.recede;
    }

    internal virtual void _tickDisplacement(Duration elapsed)
    {
        if (_displacementTickerLastElapsed is not null)
        {
            double t = (
                elapsed.inMicroseconds
                - (
                    _displacementTickerLastElapsed
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).inMicroseconds
            ).toDouble();
            _displacement =
                _displacementTarget
                - (
                    (_displacementTarget - _displacement)
                    * Dart_mathLibrary.pow(2.0, -t / _crossAxisHalfTime.inMicroseconds)
                );
            notifyListeners();
        }
        if (
            Physics.UtilsLibrary.nearEqual(
                _displacementTarget,
                _displacement,
                Physics.Tolerance.defaultTolerance.distance
            )
        )
        {
            _displacementTicker.stop();
            _displacementTickerLastElapsed = null;
        }
        else
        {
            _displacementTickerLastElapsed = elapsed;
        }
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        if (_glowOpacity.value == 0.0)
        {
            return;
        }
        double baseGlowScale = (size.width > size.height) ? (size.height / size.width) : 1.0;
        double radius = size.width * 3.0 / 2.0;
        double heightLocal = Math.Min(size.height, size.width * _widthToHeightFactor);
        double scaleY = _glowSize.value * baseGlowScale;
        var rect = Rect.fromLTWH(0.0, 0.0, size.width, heightLocal);
        var center = new Offset(size.width / 2.0 * (0.5 + _displacement), heightLocal - radius);
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color.withOpacity(_glowOpacity.value);
                    return __cascade;
                }
            )
        )();
        canvas.save();
        canvas.translate(0.0, _paintOffset + _paintOffsetScrollPixels);
        canvas.scale(1.0, scaleY);
        canvas.clipRect(rect);
        canvas.drawCircle(center, radius, paintLocal);
        canvas.restore();
    }

    public override string ToString()
    {
        return $"_GlowController(color: {color}, axis: {axis.ToString()})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _GlowingOverscrollIndicatorPainter__overscroll_indicator : CustomPainter
{
    public virtual _GlowController__overscroll_indicator? leadingController { get; private set; }
    public virtual _GlowController__overscroll_indicator? trailingController { get; private set; }
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public static double piOver2 = Dart_mathLibrary.pi / 2.0;

    internal _GlowingOverscrollIndicatorPainter__overscroll_indicator(
        _GlowController__overscroll_indicator? leadingController = null,
        _GlowController__overscroll_indicator? trailingController = null,
        AxisDirection axisDirection = default!,
        Listenable? repaint = null
    )
        : base(repaint: repaint)
    {
        this.leadingController = leadingController;
        this.trailingController = trailingController;
        this.axisDirection = axisDirection;
    }

    internal virtual void _paintSide(
        Canvas canvas,
        Size size,
        _GlowController__overscroll_indicator? controller,
        AxisDirection axisDirection,
        GrowthDirection growthDirection
    )
    {
        if (controller is null)
        {
            return;
        }
        switch (SliverLibrary.applyGrowthDirectionToAxisDirection(axisDirection, growthDirection))
        {
            case AxisDirection.up:
            {
                controller.paint(canvas, size);
                break;
            }
            case AxisDirection.down:
            {
                canvas.save();
                canvas.translate(0.0, size.height);
                canvas.scale(1.0, -1.0);
                controller.paint(canvas, size);
                canvas.restore();
                break;
            }
            case AxisDirection.left:
            {
                canvas.save();
                canvas.rotate(piOver2);
                canvas.scale(1.0, -1.0);
                controller.paint(canvas, new Size(size.height, size.width));
                canvas.restore();
                break;
            }
            case AxisDirection.right:
            {
                canvas.save();
                canvas.translate(size.width, 0.0);
                canvas.rotate(piOver2);
                controller.paint(canvas, new Size(size.height, size.width));
                canvas.restore();
                break;
            }
        }
    }

    public override void paint(Canvas canvas, Size size)
    {
        _paintSide(canvas, size, leadingController, axisDirection, GrowthDirection.reverse);
        _paintSide(canvas, size, trailingController, axisDirection, GrowthDirection.forward);
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_GlowingOverscrollIndicatorPainter__overscroll_indicator)oldDelegate;
        return (!Equals(__oldDelegate.leadingController, leadingController))
            || (!Equals(__oldDelegate.trailingController, trailingController));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"_GlowingOverscrollIndicatorPainter({leadingController}, {trailingController})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class StretchingOverscrollIndicator : StatefulWidget
{
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public virtual Func<ScrollNotification, bool> notificationPredicate { get; private set; } =
        default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public StretchingOverscrollIndicator(
        Key? key = null,
        AxisDirection axisDirection = default!,
        Func<ScrollNotification, bool> notificationPredicate = default!,
        Clip clipBehavior = Clip.hardEdge,
        Widget? child = null
    )
        : base(key: key)
    {
        Func<ScrollNotification, bool> __notificationPredicate =
            notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        this.axisDirection = axisDirection;
        this.notificationPredicate = __notificationPredicate;
        this.clipBehavior = clipBehavior;
        this.child = child;
    }

    public virtual Axis axis => Basic_typesLibrary.axisDirectionToAxis(axisDirection);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _StretchingOverscrollIndicatorState__overscroll_indicator()
        );

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<AxisDirection>("axisDirection", axisDirection));
    }
}

internal class _StretchingOverscrollIndicatorState__overscroll_indicator
    : State<StretchingOverscrollIndicator>,
        TickerProviderStateMixin<StretchingOverscrollIndicator>
{
    private bool __late__stretchController_initialized;
    private _StretchController__overscroll_indicator __late__stretchController = default!;
    internal virtual _StretchController__overscroll_indicator _stretchController
    {
        get
        {
            if (!__late__stretchController_initialized)
            {
                __late__stretchController = new _StretchController__overscroll_indicator(
                    vsync: this
                );
                __late__stretchController_initialized = true;
            }
            return __late__stretchController;
        }
    }
    internal virtual ScrollNotification? _lastNotification { get; set; } = default;
    internal virtual OverscrollNotification? _lastOverscrollNotification { get; set; } = default;
    internal virtual double _totalOverscroll { get; set; } = 0.0;
    internal virtual bool _accepted { get; set; } = true;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if (!widget.notificationPredicate(notification))
        {
            return false;
        }
        if (!Equals(notification.metrics.axis, widget.axis))
        {
            return false;
        }
        if (notification is ScrollStartNotification)
        {
            ScrollStartNotification notification__as27070 = (ScrollStartNotification)notification;
            _accepted = true;
            _totalOverscroll = 0.0;
        }
        else
        {
            if (notification is OverscrollNotification)
            {
                OverscrollNotification notification__as27182 = (OverscrollNotification)notification;
                _lastOverscrollNotification = notification__as27182;
                if (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(_lastNotification),
                        typeof(OverscrollNotification)
                    )
                )
                {
                    var confirmationNotification = new OverscrollIndicatorNotification(
                        leading: notification__as27182.overscroll < 0.0
                    );
                    confirmationNotification.dispatch(context);
                    _accepted = confirmationNotification.accepted;
                }
                if (_accepted)
                {
                    _totalOverscroll += notification__as27182.overscroll;
                    if (notification__as27182.velocity != 0.0)
                    {
                        DartRuntimePrimitives.Assert(() =>
                            notification__as27182.dragDetails is null
                        );
                        _stretchController.absorbImpact(notification__as27182.velocity);
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => notification__as27182.overscroll != 0.0);
                        if (notification__as27182.dragDetails is not null)
                        {
                            double viewportDimensionLocal = notification__as27182
                                .metrics
                                .viewportDimension;
                            double distanceForPull = _totalOverscroll / viewportDimensionLocal;
                            double clampedOverscroll = Dart_uiLibrary.clampDouble(
                                distanceForPull,
                                -1.0,
                                1.0
                            );
                            _stretchController.pull(clampedOverscroll);
                        }
                    }
                }
            }
            else
            {
                if (notification is ScrollEndNotification)
                {
                    ScrollEndNotification notification__as28637 =
                        (ScrollEndNotification)notification;
                    double velocityLocal = widget.axis switch
                    {
                        Axis.vertical => notification__as28637
                            .dragDetails
                            ?.velocity
                            .pixelsPerSecond
                            .dy
                            ?? 0.0,
                        Axis.horizontal => notification__as28637
                            .dragDetails
                            ?.velocity
                            .pixelsPerSecond
                            .dx
                            ?? 0.0,
                        _ => throw new InvalidOperationException(
                            "Non-exhaustive Dart switch value."
                        ),
                    };
                    if (
                        Equals(notification__as28637.metrics.axisDirection, AxisDirection.left)
                        || Equals(notification__as28637.metrics.axisDirection, AxisDirection.up)
                    )
                    {
                        velocityLocal = -velocityLocal;
                    }
                    _totalOverscroll = 0.0;
                    if (_accepted)
                    {
                        _stretchController.scrollEnd(velocityLocal);
                    }
                }
                else
                {
                    if (notification is ScrollUpdateNotification)
                    {
                        ScrollUpdateNotification notification__as29426 =
                            (ScrollUpdateNotification)notification;
                        _totalOverscroll = 0.0;
                        _stretchController.scrollEnd(0.0);
                    }
                }
            }
        }
        _lastNotification = notification;
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _stretchController.dispose();
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
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: _handleScrollNotification,
            child: new AnimatedBuilder(
                animation: _stretchController,
                builder: (context, child) =>
                {
                    double stretch = _stretchController.overscroll;
                    double mainAxisSize = default!;
                    switch (widget.axis)
                    {
                        case Axis.horizontal:
                        {
                            mainAxisSize = MediaQuery.widthOf(context);
                            break;
                        }
                        case Axis.vertical:
                        {
                            mainAxisSize = MediaQuery.heightOf(context);
                            break;
                        }
                    }
                    double viewportDimensionLocal =
                        _lastOverscrollNotification?.metrics.viewportDimension ?? mainAxisSize;
                    double overscrollLocal = -stretch;
                    if (
                        Equals(widget.axisDirection, AxisDirection.up)
                        || Equals(widget.axisDirection, AxisDirection.left)
                    )
                    {
                        overscrollLocal = -overscrollLocal;
                    }
                    Widget transform = new StretchEffect(
                        stretchStrength: overscrollLocal,
                        axis: widget.axis,
                        child: widget.child ?? SizedBox.CreateShrink()
                    );
                    return new ClipRect(
                        clipBehavior: ((stretch != 0.0) && (viewportDimensionLocal != mainAxisSize))
                            ? widget.clipBehavior
                            : Clip.none,
                        child: transform
                    );
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            )
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

internal class _StretchController__overscroll_indicator : Listenable
{
    public virtual Scheduler.TickerProvider vsync { get; private set; } = default!;
    internal virtual AnimationController? _controller { get; set; } = default;
    internal virtual ValueNotifier<double> _overscrollNotifier { get; private set; } =
        new ValueNotifier<double>(0.0);
    internal virtual double _interruptedOverscroll { get; set; } = 0.0;
    internal static double _exponentialScalar = Dart_mathLibrary.e / 0.33;
    internal const double _stretchIntensity = 0.016;
    public static double minOverscroll = -1.0;
    public const double maxOverscroll = 1.0;
    internal static double _flingVelocityFriction = 1L / 6000L;
    internal static double _absorbImpactVelocityFriction = 1L / 3000L;
    internal const double _maxFlingVelocity = 0.5;
    internal const double _maxAbsorbImpactVelocity = 1.25;
    public const double kNaturalFrequency = 24.657;
    public const double kDampingRatio = 0.98;
    public const double kTimeCorrectionFactor = 0.8;
    public static double kStiffness = kNaturalFrequency * kNaturalFrequency;
    internal static Physics.SpringDescription _kStretchSpringDescription =
        Physics.SpringDescription.CreateWithDampingRatio(
            mass: 1,
            stiffness: kStiffness * kTimeCorrectionFactor * kTimeCorrectionFactor,
            ratio: kDampingRatio
        );

    internal _StretchController__overscroll_indicator(Scheduler.TickerProvider vsync)
    {
        this.vsync = vsync;
    }

    public virtual double overscroll
    {
        get => _overscrollNotifier.value;
        set
        {
            var newValue = value;
            _overscrollNotifier.value = Dart_uiLibrary.clampDouble(
                newValue,
                minOverscroll,
                maxOverscroll
            );
        }
    }

    public virtual void addListener(Action listener)
    {
        _overscrollNotifier.addListener(listener);
    }

    public virtual void removeListener(Action listener)
    {
        _overscrollNotifier.removeListener(listener);
    }

    internal virtual Physics.SpringSimulation _createStretchSimulation(double velocity)
    {
        return new Physics.SpringSimulation(
            _kStretchSpringDescription,
            overscroll,
            0.0,
            velocity * kTimeCorrectionFactor
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void absorbImpact(double velocity)
    {
        if (velocity == 0.0)
        {
            return;
        }
        double scaledVelocity = Dart_uiLibrary.clampDouble(
            velocity * _absorbImpactVelocityFriction,
            -_maxAbsorbImpactVelocity,
            _maxAbsorbImpactVelocity
        );
        animate(_createStretchSimulation(scaledVelocity));
    }

    public virtual void scrollEnd(double velocity)
    {
        if ((velocity == 0.0) && (overscroll == 0.0))
        {
            return;
        }
        double scaledVelocity = Dart_uiLibrary.clampDouble(
            -(velocity * _flingVelocityFriction),
            -_maxFlingVelocity,
            _maxFlingVelocity
        );
        if (_controller is null)
        {
            animate(_createStretchSimulation(scaledVelocity));
        }
    }

    public virtual void animate(Physics.Simulation simulation)
    {
        var controller = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = AnimationController.CreateUnbounded(vsync: vsync);
                    __cascade.addListener(() =>
                    {
                        double newOverscroll = _controller?.value ?? 0.0;
                        overscroll = newOverscroll;
                    });
                    return __cascade;
                }
            )
        )();
        DartRuntimePrimitives.Ignore(
            controller
                .animateWith(simulation)
                .whenComplete(() =>
                {
                    if (Equals(_controller, controller))
                    {
                        overscroll = 0.0;
                        _interruptedOverscroll = 0.0;
                        controller.dispose();
                        _controller = null;
                    }
                    return default!;
                })
        );
        _controller?.dispose();
        _controller = controller;
    }

    public virtual void pull(double normalizedOverscroll)
    {
        if (_controller is not null)
        {
            _interruptedOverscroll = _controller!.value;
            _controller!.dispose();
            _controller = null;
        }
        var pullDistance = normalizedOverscroll;
        double absDistance = pullDistance.abs();
        double linearIntensity = _stretchIntensity * absDistance;
        double exponentialIntensity =
            _stretchIntensity * (1L - Dart_mathLibrary.exp(-absDistance * _exponentialScalar));
        double directionSign = Math.Sign(pullDistance);
        double newOverscroll = directionSign * (linearIntensity + exponentialIntensity);
        overscroll = newOverscroll + _interruptedOverscroll;
    }

    public virtual void dispose()
    {
        _controller?.dispose();
        _controller = null;
        _overscrollNotifier.dispose();
    }

    public override string ToString() => "_StretchController()";
}

public class OverscrollIndicatorNotification : Notification, ViewportNotificationMixin
{
    public virtual bool leading { get; private set; } = default!;
    public virtual double paintOffset { get; set; } = 0.0;
    public virtual bool accepted { get; set; } = true;
    public virtual long _depth { get; set; } = 0L;

    public OverscrollIndicatorNotification(bool leading)
    {
        this.leading = leading;
    }

    public virtual void disallowIndicator()
    {
        accepted = false;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {depth} ({((depth == 0L) ? "local" : "remote")})");
        description.Add($"side: {(leading ? "leading edge" : "trailing edge")}");
    }

    public virtual long depth => _depth;
}

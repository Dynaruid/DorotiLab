// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/dismissible.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public static partial class DismissibleLibrary
{
    internal static Curve _kResizeTimeCurve = new Interval(0.4, 1.0, curve: Curves.ease);
}

public static partial class DismissibleLibrary
{
    internal static double _kMinFlingVelocity = 700.0;
}

public static partial class DismissibleLibrary
{
    internal static double _kMinFlingVelocityDelta = 400.0;
}

public static partial class DismissibleLibrary
{
    internal static double _kFlingVelocityScale = 1.0 / 300.0;
}

public static partial class DismissibleLibrary
{
    internal static double _kDismissThreshold = 0.4;
}

public delegate void DismissDirectionCallback(DismissDirection direction);

public delegate Future<bool?> ConfirmDismissCallback(DismissDirection direction);

public delegate void DismissUpdateCallback(DismissUpdateDetails details);

public enum DismissDirection
{
    vertical,
    horizontal,
    endToStart,
    startToEnd,
    up,
    down,
    none,
}

public class Dismissible : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget? background { get; private set; }
    public virtual Widget? secondaryBackground { get; private set; }
    public virtual Func<DismissDirection, Future<bool?>>? confirmDismiss { get; private set; }
    public virtual Action? onResize { get; private set; }
    public virtual Action<DismissDirection>? onDismissed { get; private set; }
    public virtual DismissDirection direction { get; private set; } = default!;
    public virtual Duration? resizeDuration { get; private set; }
    public virtual DartMap<DismissDirection, double> dismissThresholds { get; private set; } =
        default!;
    public virtual Duration movementDuration { get; private set; } = default!;
    public virtual double crossAxisEndOffset { get; private set; } = default!;
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual HitTestBehavior behavior { get; private set; } = default!;
    public virtual Action<DismissUpdateDetails>? onUpdate { get; private set; }

    public Dismissible(
        Key key,
        Widget child,
        Widget? background = null,
        Widget? secondaryBackground = null,
        Func<DismissDirection, Future<bool?>>? confirmDismiss = null,
        Action? onResize = null,
        Action<DismissUpdateDetails>? onUpdate = null,
        Action<DismissDirection>? onDismissed = null,
        DismissDirection direction = DismissDirection.horizontal,
        Duration? resizeDuration = null,
        DartMap<DismissDirection, double> dismissThresholds = default!,
        Duration? movementDuration = null,
        double crossAxisEndOffset = 0.0,
        DragStartBehavior dragStartBehavior = DragStartBehavior.start,
        HitTestBehavior behavior = HitTestBehavior.opaque
    )
        : base(key: key)
    {
        Duration? __resizeDuration = resizeDuration ?? Duration.Create(milliseconds: 300);
        DartMap<DismissDirection, double> __dismissThresholds =
            dismissThresholds ?? new DartMap<DismissDirection, double>();
        Duration __movementDuration = movementDuration ?? Duration.Create(milliseconds: 200);
        this.child = child;
        this.background = background;
        this.secondaryBackground = secondaryBackground;
        this.confirmDismiss = confirmDismiss;
        this.onResize = onResize;
        this.onUpdate = onUpdate;
        this.onDismissed = onDismissed;
        this.direction = direction;
        this.resizeDuration = __resizeDuration;
        this.dismissThresholds = __dismissThresholds;
        this.movementDuration = __movementDuration;
        this.crossAxisEndOffset = crossAxisEndOffset;
        this.dragStartBehavior = dragStartBehavior;
        this.behavior = behavior;
        System.Diagnostics.Debug.Assert((secondaryBackground is null) || (background is not null));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DismissibleState__dismissible());
}

public class DismissUpdateDetails
{
    public virtual DismissDirection direction { get; private set; } = default!;
    public virtual bool reached { get; private set; } = default!;
    public virtual bool previousReached { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;

    public DismissUpdateDetails(
        DismissDirection direction = DismissDirection.horizontal,
        bool reached = false,
        bool previousReached = false,
        double progress = 0.0
    )
    {
        this.direction = direction;
        this.reached = reached;
        this.previousReached = previousReached;
        this.progress = progress;
    }
}

internal class _DismissibleClipper__dismissible : CustomClipper<Rect>
{
    public virtual Axis axis { get; private set; } = default!;
    public virtual Animation<Offset> moveAnimation { get; private set; } = default!;

    internal _DismissibleClipper__dismissible(Axis axis, Animation<Offset> moveAnimation)
        : base(reclip: moveAnimation)
    {
        this.axis = axis;
        this.moveAnimation = moveAnimation;
    }

    public override Rect getClip(Size size)
    {
        switch (axis)
        {
            case Axis.horizontal:
            {
                double offset = moveAnimation.value.dx * size.width;
                if (offset < 0L)
                {
                    return Rect.fromLTRB(size.width + offset, 0.0, size.width, size.height);
                }
                return Rect.fromLTRB(0.0, 0.0, offset, size.height);
            }
            case Axis.vertical:
            {
                double offsetLocal = moveAnimation.value.dy * size.height;
                if (offsetLocal < 0L)
                {
                    return Rect.fromLTRB(0.0, size.height + offsetLocal, size.width, size.height);
                }
                return Rect.fromLTRB(0.0, 0.0, size.width, offsetLocal);
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Rect getApproximateClipRect(Size size) => getClip(size);

    public override bool shouldReclip(CustomClipper<Rect> oldClipper)
    {
        var __oldClipper = (_DismissibleClipper__dismissible)oldClipper;
        return (!Equals(__oldClipper.axis, axis))
            || (!Equals(__oldClipper.moveAnimation.value, moveAnimation.value));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal enum _FlingGestureKind__dismissible
{
    none,
    forward,
    reverse,
}

internal class _DismissibleState__dismissible
    : State<Dismissible>,
        TickerProviderStateMixin<Dismissible>,
        AutomaticKeepAliveClientMixin<Dismissible>
{
    private bool __late__moveController_initialized;
    private AnimationController __late__moveController = default!;
    internal virtual AnimationController _moveController
    {
        get
        {
            if (!__late__moveController_initialized)
            {
                __late__moveController = new AnimationController(
                    duration: widget.movementDuration,
                    vsync: this
                );
                __late__moveController_initialized = true;
            }
            return __late__moveController;
        }
    }
    internal virtual Animation<Offset> _moveAnimation { get; set; } = default!;
    internal virtual AnimationController? _resizeController { get; set; } = default;
    internal virtual Animation<double>? _resizeAnimation { get; set; } = default;
    internal virtual double _dragExtent { get; set; } = 0.0;
    internal virtual bool _confirming { get; set; } = false;
    internal virtual bool _dragUnderway { get; set; } = false;
    internal virtual Size? _sizePriorToCollapse { get; set; } = default;
    internal virtual bool _dismissThresholdReached { get; set; } = false;
    internal virtual GlobalKey<IState> _contentKey { get; private set; } =
        GlobalKey<IState>.Create();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    public override void initState()
    {
        base.initState();
        if (wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        DartRuntimePrimitives.Ignore(
            (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = _moveController;
                        __cascade.addStatusListener(
                            (AnimationStatus __status) =>
                            {
                                _ = _handleDismissStatusChanged(__status);
                            }
                        );
                        __cascade.addListener(_handleDismissUpdateValueChanged);
                        return __cascade;
                    }
                )
            )()
        );
        _updateMoveAnimation();
    }

    public virtual bool wantKeepAlive =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _moveController.isAnimating || (_resizeController?.isAnimating ?? false)
        );

    public override void dispose()
    {
        _moveController.dispose();
        _resizeController?.dispose();
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
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual bool _directionIsXAxis
    {
        get
        {
            return Equals(widget.direction, DismissDirection.horizontal)
                || Equals(widget.direction, DismissDirection.endToStart)
                || Equals(widget.direction, DismissDirection.startToEnd);
        }
    }

    internal virtual DismissDirection _extentToDirection(double extent)
    {
        if (extent == 0.0)
        {
            return DismissDirection.none;
        }
        if (_directionIsXAxis)
        {
            return Directionality.of(context) switch
            {
                TextDirection.rtl when extent < 0L => DismissDirection.startToEnd,
                TextDirection.ltr when extent > 0L => DismissDirection.startToEnd,
                TextDirection.rtl => DismissDirection.endToStart,
                TextDirection.ltr => DismissDirection.endToStart,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
        }
        return (extent > 0L) ? DismissDirection.down : DismissDirection.up;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual DismissDirection _dismissDirection => _extentToDirection(_dragExtent);
    internal virtual double _dismissThreshold =>
        DartRuntimePrimitives.ConvertValue<double>(
            DartCollectionRuntime.NullableMapValue<double>(
                widget.dismissThresholds,
                _dismissDirection
            ) ?? DismissibleLibrary._kDismissThreshold
        );
    internal virtual double _overallDragAxisExtent
    {
        get
        {
            Size sizeLocal = (
                context.size
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            return _directionIsXAxis ? sizeLocal.width : sizeLocal.height;
        }
    }

    internal virtual void _handleDragStart(DragStartDetails details)
    {
        if (_confirming)
        {
            return;
        }
        _dragUnderway = true;
        if (_moveController.isAnimating)
        {
            _dragExtent = _moveController.value * _overallDragAxisExtent * Math.Sign(_dragExtent);
            _moveController.stop();
        }
        else
        {
            _dragExtent = 0.0;
            _moveController.value = 0.0;
        }
        setState(() =>
        {
            _updateMoveAnimation();
        });
    }

    internal virtual void _handleDragUpdate(DragUpdateDetails details)
    {
        if (!_dragUnderway || _moveController.isAnimating)
        {
            return;
        }
        double delta = (
            details.primaryDelta
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        double oldDragExtent = _dragExtent;
        switch (widget.direction)
        {
            case DismissDirection.horizontal:
            case DismissDirection.vertical:
            {
                _dragExtent += delta;
                break;
            }
            case DismissDirection.up:
            {
                if ((_dragExtent + delta) < 0L)
                {
                    _dragExtent += delta;
                }
                break;
            }
            case DismissDirection.down:
            {
                if ((_dragExtent + delta) > 0L)
                {
                    _dragExtent += delta;
                }
                break;
            }
            case DismissDirection.endToStart:
            {
                switch (Directionality.of(context))
                {
                    case TextDirection.rtl:
                    {
                        if ((_dragExtent + delta) > 0L)
                        {
                            _dragExtent += delta;
                        }
                        break;
                    }
                    case TextDirection.ltr:
                    {
                        if ((_dragExtent + delta) < 0L)
                        {
                            _dragExtent += delta;
                        }
                        break;
                    }
                }
                break;
            }
            case DismissDirection.startToEnd:
            {
                switch (Directionality.of(context))
                {
                    case TextDirection.rtl:
                    {
                        if ((_dragExtent + delta) < 0L)
                        {
                            _dragExtent += delta;
                        }
                        break;
                    }
                    case TextDirection.ltr:
                    {
                        if ((_dragExtent + delta) > 0L)
                        {
                            _dragExtent += delta;
                        }
                        break;
                    }
                }
                break;
            }
            case DismissDirection.none:
            {
                _dragExtent = 0;
                break;
            }
        }
        if (Math.Sign(oldDragExtent) != Math.Sign(_dragExtent))
        {
            setState(() =>
            {
                _updateMoveAnimation();
            });
        }
        if (!_moveController.isAnimating)
        {
            _moveController.value = _dragExtent.abs() / _overallDragAxisExtent;
        }
    }

    internal virtual void _handleDismissUpdateValueChanged()
    {
        if (widget.onUpdate is not null)
        {
            bool oldDismissThresholdReached = _dismissThresholdReached;
            _dismissThresholdReached = _moveController.value > _dismissThreshold;
            var details = new DismissUpdateDetails(
                direction: _dismissDirection,
                reached: _dismissThresholdReached,
                previousReached: oldDismissThresholdReached,
                progress: _moveController.value
            );
            widget.onUpdate!(details);
        }
    }

    internal virtual void _updateMoveAnimation()
    {
        double endLocal = Math.Sign(_dragExtent);
        _moveAnimation = _moveController.drive(
            new Tween<Offset>(
                begin: Offset.zero,
                end: _directionIsXAxis
                    ? new Offset(endLocal, widget.crossAxisEndOffset)
                    : new Offset(widget.crossAxisEndOffset, endLocal)
            )
        );
    }

    internal virtual _FlingGestureKind__dismissible _describeFlingGesture(Velocity velocity)
    {
        if (_dragExtent == 0.0)
        {
            return _FlingGestureKind__dismissible.none;
        }
        double vx = velocity.pixelsPerSecond.dx;
        double vy = velocity.pixelsPerSecond.dy;
        DismissDirection flingDirection = default!;
        if (_directionIsXAxis)
        {
            if (
                ((vx.abs() - vy.abs()) < DismissibleLibrary._kMinFlingVelocityDelta)
                || (vx.abs() < DismissibleLibrary._kMinFlingVelocity)
            )
            {
                return _FlingGestureKind__dismissible.none;
            }
            DartRuntimePrimitives.Assert(() => vx != 0.0);
            flingDirection = _extentToDirection(vx);
        }
        else
        {
            if (
                ((vy.abs() - vx.abs()) < DismissibleLibrary._kMinFlingVelocityDelta)
                || (vy.abs() < DismissibleLibrary._kMinFlingVelocity)
            )
            {
                return _FlingGestureKind__dismissible.none;
            }
            DartRuntimePrimitives.Assert(() => vy != 0.0);
            flingDirection = _extentToDirection(vy);
        }
        if (Equals(flingDirection, _dismissDirection))
        {
            return _FlingGestureKind__dismissible.forward;
        }
        return _FlingGestureKind__dismissible.reverse;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleDragEnd(DragEndDetails details)
    {
        if (!_dragUnderway || _moveController.isAnimating)
        {
            return;
        }
        _dragUnderway = false;
        if (_moveController.isCompleted)
        {
            DartRuntimePrimitives.Ignore(_handleMoveCompleted());
            return;
        }
        double flingVelocity = _directionIsXAxis
            ? details.velocity.pixelsPerSecond.dx
            : details.velocity.pixelsPerSecond.dy;
        switch (_describeFlingGesture(details.velocity))
        {
            case _FlingGestureKind__dismissible.forward:
            {
                DartRuntimePrimitives.Assert(() => _dragExtent != 0.0);
                DartRuntimePrimitives.Assert(() => !_moveController.isDismissed);
                if (_dismissThreshold >= 1.0)
                {
                    _moveController.reverse();
                    break;
                }
                _dragExtent = Math.Sign(flingVelocity);
                _moveController.fling(
                    velocity: flingVelocity.abs() * DismissibleLibrary._kFlingVelocityScale
                );
                break;
            }
            case _FlingGestureKind__dismissible.reverse:
            {
                DartRuntimePrimitives.Assert(() => _dragExtent != 0.0);
                DartRuntimePrimitives.Assert(() => !_moveController.isDismissed);
                _dragExtent = Math.Sign(flingVelocity);
                _moveController.fling(
                    velocity: -flingVelocity.abs() * DismissibleLibrary._kFlingVelocityScale
                );
                break;
            }
            case _FlingGestureKind__dismissible.none:
            {
                if (!_moveController.isDismissed)
                {
                    if (_moveController.value > _dismissThreshold)
                    {
                        _moveController.forward();
                    }
                    else
                    {
                        _moveController.reverse();
                    }
                }
                break;
            }
        }
    }

    internal virtual async Future _handleDismissStatusChanged(AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status) && !_dragUnderway)
        {
            await _handleMoveCompleted();
        }
        if (mounted)
        {
            updateKeepAlive();
        }
    }

    internal virtual async Future _handleMoveCompleted()
    {
        if (_dismissThreshold >= 1.0)
        {
            DartRuntimePrimitives.Observe(_moveController.reverse(), "Dismissible.reverse");
            return;
        }
        bool result = await _confirmStartResizeAnimation();
        if (mounted)
        {
            if (result)
            {
                _startResizeAnimation();
            }
            else
            {
                DartRuntimePrimitives.Observe(_moveController.reverse(), "Dismissible.reverse");
            }
        }
    }

    internal virtual async Future<bool> _confirmStartResizeAnimation()
    {
        if (widget.confirmDismiss is not null)
        {
            _confirming = true;
            DismissDirection direction = _dismissDirection;
            try
            {
                return await widget.confirmDismiss!(direction) ?? false;
            }
            finally
            {
                _confirming = false;
            }
        }
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _startResizeAnimation()
    {
        DartRuntimePrimitives.Assert(() => _moveController.isCompleted);
        DartRuntimePrimitives.Assert(() => _resizeController is null);
        DartRuntimePrimitives.Assert(() => _sizePriorToCollapse is null);
        if (widget.resizeDuration is null)
        {
            if (widget.onDismissed is not null)
            {
                DismissDirection direction = _dismissDirection;
                widget.onDismissed!(direction);
            }
        }
        else
        {
            _resizeController = (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = new AnimationController(
                            duration: widget.resizeDuration,
                            vsync: this
                        );
                        __cascade.addListener(_handleResizeProgressChanged);
                        __cascade.addStatusListener((status) => updateKeepAlive());
                        return __cascade;
                    }
                )
            )();
            _resizeController!.forward();
            setState(() =>
            {
                _sizePriorToCollapse = context.size;
                _resizeAnimation = _resizeController!
                    .drive(new CurveTween(curve: DismissibleLibrary._kResizeTimeCurve))
                    .drive(new Tween<double>(begin: 1.0, end: 0.0));
            });
        }
    }

    internal virtual void _handleResizeProgressChanged()
    {
        if (_resizeController!.isCompleted)
        {
            widget.onDismissed?.Invoke(_dismissDirection);
        }
        else
        {
            widget.onResize?.Invoke();
        }
    }

    public override Widget build(BuildContext context)
    {
        if (wantKeepAlive && (_keepAliveHandle is null))
        {
            _ensureKeepAlive();
        }
        DartRuntimePrimitives.Assert(() =>
            !_directionIsXAxis || DebugLibrary.debugCheckHasDirectionality(context)
        );
        Widget? backgroundLocal = widget.background;
        if (widget.secondaryBackground is not null)
        {
            DismissDirection directionLocal = _dismissDirection;
            if (
                Equals(directionLocal, DismissDirection.endToStart)
                || Equals(directionLocal, DismissDirection.up)
            )
            {
                backgroundLocal = widget.secondaryBackground;
            }
        }
        if (_resizeAnimation is not null)
        {
            DartRuntimePrimitives.Assert(() =>
            {
                if (!Equals(_resizeAnimation!.status, AnimationStatus.forward))
                {
                    DartRuntimePrimitives.Assert(() => _resizeAnimation!.isCompleted);
                    throw DartRuntimePrimitives.AsException(
                        new FlutterError(
                            new List<DiagnosticsNode>
                            {
                                new ErrorSummary(
                                    "A dismissed Dismissible widget is still part of the tree."
                                ),
                                new ErrorHint(
                                    "Make sure to implement the onDismissed handler and to immediately remove the Dismissible "
                                        + "widget from the application once that handler has fired."
                                ),
                            }
                        )
                    );
                }
                return true;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            });
            return new SizeTransition(
                sizeFactor: _resizeAnimation!,
                axis: _directionIsXAxis ? Axis.vertical : Axis.horizontal,
                child: new SizedBox(
                    width: (
                        _sizePriorToCollapse
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).width,
                    height: (
                        _sizePriorToCollapse
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).height,
                    child: backgroundLocal
                )
            );
        }
        Widget content = new SlideTransition(
            position: _moveAnimation,
            child: new KeyedSubtree(key: _contentKey, child: widget.child)
        );
        if (backgroundLocal is not null)
        {
            content = DartRuntimePrimitives.ConvertValue<Widget>(
                new Stack(children: new List<Widget> { content })
            );
        }
        if (Equals(widget.direction, DismissDirection.none))
        {
            return content;
        }
        return new GestureDetector(
            onHorizontalDragStart: _directionIsXAxis ? _handleDragStart : null,
            onHorizontalDragUpdate: _directionIsXAxis ? _handleDragUpdate : null,
            onHorizontalDragEnd: _directionIsXAxis ? _handleDragEnd : null,
            onVerticalDragStart: _directionIsXAxis ? null : _handleDragStart,
            onVerticalDragUpdate: _directionIsXAxis ? null : _handleDragUpdate,
            onVerticalDragEnd: _directionIsXAxis ? null : _handleDragEnd,
            behavior: widget.behavior,
            dragStartBehavior: widget.dragStartBehavior,
            child: content
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => _keepAliveHandle is null);
        _keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(_keepAliveHandle!).dispatch(context);
    }

    public virtual void _releaseKeepAlive()
    {
        _keepAliveHandle!.dispose();
        _keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (wantKeepAlive)
        {
            if (_keepAliveHandle is null)
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if (_keepAliveHandle is not null)
            {
                _releaseKeepAlive();
            }
        }
    }

    public override void deactivate()
    {
        if (_keepAliveHandle is not null)
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }
}

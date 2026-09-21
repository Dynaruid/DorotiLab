// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/tap_and_drag.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public static partial class Tap_and_dragLibrary
{
    internal static double _getGlobalDistance(PointerEvent @event, OffsetPair? originPosition)
    {
        DartRuntimePrimitives.Assert(() => originPosition is not null);
        Offset offset = @event.position - originPosition!.global;
        return offset.distance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _DragState__tap_and_drag
{
    ready,
    possible,
    accepted,
}

public delegate void GestureTapDragDownCallback(TapDragDownDetails details);

public class TapDragDownDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition
    {
        get => __field_globalPosition;
    }
    private Offset __field_localPosition = default!;
    public override Offset localPosition
    {
        get => __field_localPosition;
    }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragDownDetails(
        Offset globalPosition,
        Offset localPosition,
        PointerDeviceKind? kind = null,
        long consecutiveTapCount = default!
    )
    {
        __field_globalPosition = globalPosition;
        __field_localPosition = localPosition;
        this.kind = kind;
        this.consecutiveTapCount = consecutiveTapCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
        properties.add(new IntProperty("consecutiveTapCount", consecutiveTapCount));
    }
}

public delegate void GestureTapDragUpCallback(TapDragUpDetails details);

public class TapDragUpDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition
    {
        get => __field_globalPosition;
    }
    private Offset __field_localPosition = default!;
    public override Offset localPosition
    {
        get => __field_localPosition;
    }
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragUpDetails(
        Offset globalPosition,
        Offset localPosition,
        PointerDeviceKind kind,
        long consecutiveTapCount
    )
    {
        __field_globalPosition = globalPosition;
        __field_localPosition = localPosition;
        this.kind = kind;
        this.consecutiveTapCount = consecutiveTapCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
        properties.add(new IntProperty("consecutiveTapCount", consecutiveTapCount));
    }
}

public delegate void GestureTapDragStartCallback(TapDragStartDetails details);

public class TapDragStartDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition
    {
        get => __field_globalPosition;
    }
    private Offset __field_localPosition = default!;
    public override Offset localPosition
    {
        get => __field_localPosition;
    }
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragStartDetails(
        Offset globalPosition,
        Offset localPosition,
        Duration? sourceTimeStamp = null,
        PointerDeviceKind? kind = null,
        long consecutiveTapCount = default!
    )
    {
        __field_globalPosition = globalPosition;
        __field_localPosition = localPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.kind = kind;
        this.consecutiveTapCount = consecutiveTapCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", sourceTimeStamp));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
        properties.add(new IntProperty("consecutiveTapCount", consecutiveTapCount));
    }
}

public delegate void GestureTapDragUpdateCallback(TapDragUpdateDetails details);

public class TapDragUpdateDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition
    {
        get => __field_globalPosition;
    }
    private Offset __field_localPosition = default!;
    public override Offset localPosition
    {
        get => __field_localPosition;
    }
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual Offset delta { get; private set; } = default!;
    public virtual double? primaryDelta { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual Offset offsetFromOrigin { get; private set; } = default!;
    public virtual Offset localOffsetFromOrigin { get; private set; } = default!;
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragUpdateDetails(
        Offset globalPosition,
        Offset localPosition,
        Duration? sourceTimeStamp = null,
        Offset delta = default,
        double? primaryDelta = null,
        PointerDeviceKind? kind = null,
        Offset offsetFromOrigin = default!,
        Offset localOffsetFromOrigin = default!,
        long consecutiveTapCount = default!
    )
    {
        __field_globalPosition = globalPosition;
        __field_localPosition = localPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.delta = delta;
        this.primaryDelta = primaryDelta;
        this.kind = kind;
        this.offsetFromOrigin = offsetFromOrigin;
        this.localOffsetFromOrigin = localOffsetFromOrigin;
        this.consecutiveTapCount = consecutiveTapCount;
        System.Diagnostics.Debug.Assert(
            (primaryDelta is null)
                || (
                    (
                        (
                            primaryDelta
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ) == delta.dx
                    ) && (delta.dy == 0.0)
                )
                || (
                    (
                        (
                            primaryDelta
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ) == delta.dy
                    ) && (delta.dx == 0.0)
                )
        );
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", sourceTimeStamp));
        properties.add(new DiagnosticsProperty<Offset>("delta", delta));
        properties.add(new DoubleProperty("primaryDelta", primaryDelta));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
        properties.add(new DiagnosticsProperty<Offset>("offsetFromOrigin", offsetFromOrigin));
        properties.add(
            new DiagnosticsProperty<Offset>("localOffsetFromOrigin", localOffsetFromOrigin)
        );
        properties.add(new IntProperty("consecutiveTapCount", consecutiveTapCount));
    }
}

public delegate void GestureTapDragEndCallback(TapDragEndDetails endDetails);

public class TapDragEndDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition
    {
        get => __field_globalPosition;
    }
    private Offset __field_localPosition = default!;
    public override Offset localPosition
    {
        get => __field_localPosition;
    }
    public virtual Velocity velocity { get; private set; } = default!;
    public virtual double? primaryVelocity { get; private set; }
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragEndDetails(
        Offset globalPosition = default,
        Offset? localPosition = null,
        Velocity velocity = default!,
        double? primaryVelocity = null,
        long consecutiveTapCount = default!
    )
    {
        Velocity __velocity = velocity ?? Velocity.zero;
        __field_globalPosition = globalPosition;
        this.velocity = __velocity;
        this.primaryVelocity = primaryVelocity;
        this.consecutiveTapCount = consecutiveTapCount;
        __field_localPosition = localPosition ?? globalPosition;
        System.Diagnostics.Debug.Assert(
            (primaryVelocity is null)
                || (
                    (
                        primaryVelocity
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) == __velocity.pixelsPerSecond.dx
                )
                || (
                    (
                        primaryVelocity
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) == __velocity.pixelsPerSecond.dy
                )
        );
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Velocity>("velocity", velocity));
        properties.add(new DoubleProperty("primaryVelocity", primaryVelocity));
        properties.add(new IntProperty("consecutiveTapCount", consecutiveTapCount));
    }
}

public delegate void GestureCancelCallback();

public interface _TapStatusTrackerMixin__tap_and_drag
{
    PointerDownEvent? _down { get; set; }
    PointerUpEvent? _up { get; set; }
    long _consecutiveTapCount { get; set; }
    OffsetPair? _originPosition { get; set; }
    long? _previousButtons { get; set; }
    Timer? _consecutiveTapTimer { get; set; }
    Offset? _lastTapOffset { get; set; }
    Action? onTapTrackStart { get; set; }
    Action? onTapTrackReset { get; set; }

    public PointerDownEvent? currentDown { get; }
    public PointerUpEvent? currentUp { get; }
    public long consecutiveTapCount { get; }
    public long? maxConsecutiveTap { get; }
    public void addAllowedPointer(PointerDownEvent @event);
    public void handleEvent(PointerEvent @event);
    public void rejectGesture(long pointer);
    public void dispose();
    public void _trackTap(PointerDownEvent @event);
    public bool _hasSameButton(long buttons);
    public bool _isWithinConsecutiveTapTolerance(Offset secondTapOffset);
    public bool _representsSameSeries(PointerDownEvent @event);
    public void _consecutiveTapTimerStart();
    public void _consecutiveTapTimerStop();
    public void _consecutiveTapTimerTimeout();
    public void _tapTrackerReset();
}

public abstract class BaseTapAndDragGestureRecognizer
    : OneSequenceGestureRecognizer,
        _TapStatusTrackerMixin__tap_and_drag
{
    public virtual DragStartBehavior dragStartBehavior { get; set; } = default!;
    public virtual Duration? dragUpdateThrottleFrequency { get; set; } = default;
    public virtual long? maxConsecutiveTap { get; set; } = default;
    public virtual bool eagerVictoryOnDrag { get; set; } = default!;
    public virtual Action<TapDragDownDetails>? onTapDown { get; set; } = default;
    public virtual Action<TapDragUpDetails>? onTapUp { get; set; } = default;
    public virtual Action<TapDragStartDetails>? onDragStart { get; set; } = default;
    public virtual Action<TapDragUpdateDetails>? onDragUpdate { get; set; } = default;
    public virtual Action<TapDragEndDetails>? onDragEnd { get; set; } = default;
    public virtual Action? onCancel { get; set; } = default;
    internal virtual bool _pastSlopTolerance { get; set; } = false;
    internal virtual bool _sentTapDown { get; set; } = false;
    internal virtual bool _wonArenaForPrimaryPointer { get; set; } = false;
    internal virtual long? _primaryPointer { get; set; } = default;
    internal virtual Timer? _deadlineTimer { get; set; } = default;
    internal virtual Duration _deadline { get; private set; } = default!;
    internal virtual _DragState__tap_and_drag _dragState { get; set; } =
        _DragState__tap_and_drag.ready;
    internal virtual PointerEvent? _start { get; set; } = default;
    internal virtual OffsetPair _initialPosition { get; set; } = default!;
    internal virtual OffsetPair _currentPosition { get; set; } = default!;
    internal virtual double _globalDistanceMoved { get; set; } = default!;
    internal virtual double _globalDistanceMovedAllAxes { get; set; } = default!;
    internal virtual TapDragUpdateDetails? _lastDragUpdateDetails { get; set; } = default;
    internal virtual Timer? _dragUpdateThrottleTimer { get; set; } = default;
    internal virtual HashSet<long> _acceptedActivePointers { get; private set; } =
        new HashSet<long>();
    public virtual PointerDownEvent? _down { get; set; } = default;
    public virtual PointerUpEvent? _up { get; set; } = default;
    public virtual long _consecutiveTapCount { get; set; } = 0L;
    public virtual OffsetPair? _originPosition { get; set; } = default;
    public virtual long? _previousButtons { get; set; } = default;
    public virtual Timer? _consecutiveTapTimer { get; set; } = default;
    public virtual Offset? _lastTapOffset { get; set; } = default;
    public virtual Action? onTapTrackStart { get; set; } = default;
    public virtual Action? onTapTrackReset { get; set; } = default;

    protected BaseTapAndDragGestureRecognizer(
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null,
        Func<long, bool> allowedButtonsFilter = default!,
        bool eagerVictoryOnDrag = true
    )
        : base(
            debugOwner: debugOwner,
            supportedDevices: supportedDevices,
            allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior
        )
    {
        this.eagerVictoryOnDrag = eagerVictoryOnDrag;
        _deadline = ConstantsLibrary.kPressTimeout;
        dragStartBehavior = DragStartBehavior.start;
    }

    internal abstract Offset _getDeltaForDetails(Offset delta);
    internal abstract double? _getPrimaryValueFromOffset(Offset value);
    internal abstract bool _hasSufficientGlobalDistanceToAccept(
        PointerDeviceKind pointerDeviceKind
    );

    internal virtual void _handleDragUpdateThrottled()
    {
        DartRuntimePrimitives.Assert(() => _lastDragUpdateDetails is not null);
        if (onDragUpdate is not null)
        {
            invokeCallback<object?>(
                "onDragUpdate",
                () =>
                {
                    ((Action)(() => onDragUpdate!(_lastDragUpdateDetails!)))();
                    return null;
                }
            );
        }
        _dragUpdateThrottleTimer = null;
        _lastDragUpdateDetails = null;
    }

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if (_primaryPointer is null)
        {
            switch (@event.buttons)
            {
                case var __constant38875 when Equals(__constant38875, EventsLibrary.kPrimaryButton):
                {
                    if (
                        (onTapDown is null)
                        && (onDragStart is null)
                        && (onDragUpdate is null)
                        && (onDragEnd is null)
                        && (onTapUp is null)
                        && (onCancel is null)
                    )
                    {
                        return false;
                    }
                    break;
                }
                default:
                {
                    return false;
                }
            }
        }
        else
        {
            if (@event.pointer != _primaryPointer)
            {
                return false;
            }
        }
        return base.isPointerAllowed(((PointerDownEvent?)(object?)@event)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if (Equals(_dragState, _DragState__tap_and_drag.ready))
        {
            base.addAllowedPointer(@event);
            if ((_consecutiveTapTimer is not null) && !_consecutiveTapTimer!.isActive)
            {
                _tapTrackerReset();
            }
            if (maxConsecutiveTap == _consecutiveTapCount)
            {
                _tapTrackerReset();
            }
            _up = null;
            if ((_down is not null) && !_representsSameSeries(@event))
            {
                _consecutiveTapCount = 1L;
            }
            else
            {
                _consecutiveTapCount += 1L;
            }
            _consecutiveTapTimerStop();
            _trackTap(@event);
            _primaryPointer = @event.pointer;
            _globalDistanceMoved = 0.0;
            _globalDistanceMovedAllAxes = 0.0;
            _dragState = _DragState__tap_and_drag.possible;
            _initialPosition = new OffsetPair(global: @event.position, local: @event.localPosition);
            _currentPosition = _initialPosition;
            _deadlineTimer = new Timer(_deadline, () => _didExceedDeadlineWithEvent(@event));
        }
    }

    public override void handleNonAllowedPointer(PointerDownEvent @event)
    {
        if (@event.buttons != EventsLibrary.kPrimaryButton)
        {
            if (!_wonArenaForPrimaryPointer)
            {
                base.handleNonAllowedPointer(@event);
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
        if (pointer != _primaryPointer)
        {
            return;
        }
        _stopDeadlineTimer();
        DartRuntimePrimitives.Assert(() => !_acceptedActivePointers.Contains(pointer));
        _acceptedActivePointers.Add(pointer);
        if (currentDown is not null)
        {
            _checkTapDown(currentDown!);
        }
        _wonArenaForPrimaryPointer = true;
        if ((_start is not null) && eagerVictoryOnDrag)
        {
            DartRuntimePrimitives.Assert(() =>
                Equals(_dragState, _DragState__tap_and_drag.accepted)
            );
            DartRuntimePrimitives.Assert(() => currentUp is null);
            _acceptDrag(_start!);
        }
        if ((_start is not null) && !eagerVictoryOnDrag)
        {
            DartRuntimePrimitives.Assert(() =>
                Equals(_dragState, _DragState__tap_and_drag.possible)
            );
            DartRuntimePrimitives.Assert(() => currentUp is null);
            _dragState = _DragState__tap_and_drag.accepted;
            _acceptDrag(_start!);
        }
        if (currentUp is not null)
        {
            _checkTapUp(currentUp!);
        }
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        switch (_dragState)
        {
            case _DragState__tap_and_drag.ready:
            {
                _checkCancel();
                resolve(GestureDisposition.rejected);
                break;
            }
            case _DragState__tap_and_drag.possible:
            {
                if (_pastSlopTolerance)
                {
                    if (_wonArenaForPrimaryPointer)
                    {
                        if (currentDown is not null)
                        {
                            if (!_acceptedActivePointers.Remove(pointer))
                            {
                                resolvePointer(pointer, GestureDisposition.rejected);
                            }
                            _dragState = _DragState__tap_and_drag.accepted;
                            _acceptDrag(currentDown!);
                            _checkDragEnd();
                        }
                    }
                    else
                    {
                        _checkCancel();
                        resolve(GestureDisposition.rejected);
                    }
                }
                else
                {
                    if (currentUp is not null)
                    {
                        _checkTapUp(currentUp!);
                    }
                }
                break;
            }
            case _DragState__tap_and_drag.accepted:
            {
                _checkDragEnd();
                break;
            }
        }
        _stopDeadlineTimer();
        _start = null;
        _dragState = _DragState__tap_and_drag.ready;
        _pastSlopTolerance = false;
    }

    public override void handleEvent(PointerEvent @event)
    {
        if (@event.pointer != _primaryPointer)
        {
            return;
        }
        if (@event is PointerMoveEvent)
        {
            PointerMoveEvent @event__as22088 = (PointerMoveEvent)@event;
            double computedSlop = EventsLibrary.computeHitSlop(
                @event__as22088.kind,
                gestureSettings
            );
            bool isSlopPastTolerance =
                Tap_and_dragLibrary._getGlobalDistance(@event__as22088, _originPosition)
                > computedSlop;
            if (isSlopPastTolerance)
            {
                _consecutiveTapTimerStop();
                _previousButtons = null;
                _lastTapOffset = null;
            }
        }
        else
        {
            if (@event is PointerUpEvent)
            {
                PointerUpEvent @event__as22451 = (PointerUpEvent)@event;
                _up = @event__as22451;
                if (_down is not null)
                {
                    _consecutiveTapTimerStop();
                    _consecutiveTapTimerStart();
                }
            }
            else
            {
                if (@event is PointerCancelEvent)
                {
                    PointerCancelEvent @event__as22620 = (PointerCancelEvent)@event;
                    _tapTrackerReset();
                }
            }
        }
        if (@event is PointerMoveEvent)
        {
            PointerMoveEvent @event__as43046 = (PointerMoveEvent)@event;
            double computedSlopLocal = EventsLibrary.computeHitSlop(
                @event__as43046.kind,
                gestureSettings
            );
            _pastSlopTolerance =
                _pastSlopTolerance
                || (
                    Tap_and_dragLibrary._getGlobalDistance(@event__as43046, _initialPosition)
                    > computedSlopLocal
                );
            if (Equals(_dragState, _DragState__tap_and_drag.accepted))
            {
                _currentPosition = OffsetPair.CreateFromEventPosition(@event__as43046);
                _checkDragUpdate(@event__as43046);
            }
            else
            {
                if (Equals(_dragState, _DragState__tap_and_drag.possible))
                {
                    if (_start is null)
                    {
                        _checkDrag(@event__as43046);
                    }
                    if ((_start is not null) && _wonArenaForPrimaryPointer)
                    {
                        _dragState = _DragState__tap_and_drag.accepted;
                        _acceptDrag(_start!);
                    }
                }
            }
        }
        else
        {
            if (@event is PointerUpEvent)
            {
                PointerUpEvent @event__as45053 = (PointerUpEvent)@event;
                if (Equals(_dragState, _DragState__tap_and_drag.possible))
                {
                    stopTrackingIfPointerNoLongerDown(@event__as45053);
                }
                else
                {
                    if (Equals(_dragState, _DragState__tap_and_drag.accepted))
                    {
                        _giveUpPointer(@event__as45053.pointer);
                    }
                }
            }
            else
            {
                if (@event is PointerCancelEvent)
                {
                    PointerCancelEvent @event__as45427 = (PointerCancelEvent)@event;
                    _dragState = _DragState__tap_and_drag.ready;
                    _giveUpPointer(@event__as45427.pointer);
                }
            }
        }
    }

    public override void rejectGesture(long pointer)
    {
        if (pointer != _primaryPointer)
        {
            return;
        }
        _tapTrackerReset();
        _stopDeadlineTimer();
        _giveUpPointer(pointer);
        _resetTaps();
        _resetDragUpdateThrottle();
    }

    public override void dispose()
    {
        _stopDeadlineTimer();
        _resetDragUpdateThrottle();
        _tapTrackerReset();
        base.dispose();
    }

    public override string debugDescription => "tap_and_drag";

    internal virtual void _acceptDrag(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => Equals(_dragState, _DragState__tap_and_drag.accepted));
        if (!_wonArenaForPrimaryPointer)
        {
            return;
        }
        if (Equals(dragStartBehavior, DragStartBehavior.start))
        {
            _initialPosition = _initialPosition.op_Add(
                new OffsetPair(global: @event.delta, local: @event.localDelta)
            );
            _currentPosition = _initialPosition;
        }
        _checkDragStart(@event);
        Offset localDeltaLocal = @event.localDelta;
        if (!Equals(localDeltaLocal, Offset.zero))
        {
            _currentPosition = OffsetPair.CreateFromEventPosition(@event);
            Offset correctedLocalPosition = _initialPosition.local + localDeltaLocal;
            Matrix4? localToGlobalTransform =
                (@event.transform is null) ? null : Matrix4.tryInvert(@event.transform!);
            Offset globalUpdateDelta = PointerEvent.transformDeltaViaPositions(
                transform: localToGlobalTransform,
                untransformedDelta: localDeltaLocal,
                untransformedEndPosition: correctedLocalPosition
            );
            var updateDelta = new OffsetPair(local: localDeltaLocal, global: globalUpdateDelta);
            _checkDragUpdate(@event, corrected: _initialPosition.op_Add(updateDelta));
        }
    }

    internal virtual void _checkDrag(PointerMoveEvent @event)
    {
        Matrix4? localToGlobalTransform =
            (@event.transform is null) ? null : Matrix4.tryInvert(@event.transform!);
        Offset movedLocally = _getDeltaForDetails(@event.localDelta);
        _globalDistanceMoved +=
            PointerEvent
                .transformDeltaViaPositions(
                    transform: localToGlobalTransform,
                    untransformedDelta: movedLocally,
                    untransformedEndPosition: @event.localPosition
                )
                .distance * Math.Sign(_getPrimaryValueFromOffset(movedLocally) ?? 1);
        _globalDistanceMovedAllAxes +=
            PointerEvent
                .transformDeltaViaPositions(
                    transform: localToGlobalTransform,
                    untransformedDelta: @event.localDelta,
                    untransformedEndPosition: @event.localPosition
                )
                .distance * Math.Sign(1L);
        if (
            _hasSufficientGlobalDistanceToAccept(@event.kind)
            || (
                _wonArenaForPrimaryPointer
                && (
                    _globalDistanceMovedAllAxes.abs()
                    > EventsLibrary.computePanSlop(@event.kind, gestureSettings)
                )
            )
        )
        {
            _start = @event;
            if (eagerVictoryOnDrag)
            {
                _dragState = _DragState__tap_and_drag.accepted;
                if (!_wonArenaForPrimaryPointer)
                {
                    resolve(GestureDisposition.accepted);
                }
            }
        }
    }

    internal virtual void _checkTapDown(PointerDownEvent @event)
    {
        if (_sentTapDown)
        {
            return;
        }
        var details = new TapDragDownDetails(
            globalPosition: @event.position,
            localPosition: @event.localPosition,
            kind: getKindForPointer(@event.pointer),
            consecutiveTapCount: consecutiveTapCount
        );
        if (onTapDown is not null)
        {
            invokeCallback<object?>(
                "onTapDown",
                () =>
                {
                    ((Action)(() => onTapDown!(details)))();
                    return null;
                }
            );
        }
        _sentTapDown = true;
    }

    internal virtual void _checkTapUp(PointerUpEvent @event)
    {
        if (!_wonArenaForPrimaryPointer)
        {
            return;
        }
        var upDetails = new TapDragUpDetails(
            kind: @event.kind,
            globalPosition: @event.position,
            localPosition: @event.localPosition,
            consecutiveTapCount: consecutiveTapCount
        );
        if (onTapUp is not null)
        {
            invokeCallback<object?>(
                "onTapUp",
                () =>
                {
                    ((Action)(() => onTapUp!(upDetails)))();
                    return null;
                }
            );
        }
        _resetTaps();
        if (!_acceptedActivePointers.Remove(@event.pointer))
        {
            resolvePointer(@event.pointer, GestureDisposition.rejected);
        }
    }

    internal virtual void _checkDragStart(PointerEvent @event)
    {
        if (onDragStart is not null)
        {
            var details = new TapDragStartDetails(
                sourceTimeStamp: @event.timeStamp,
                globalPosition: _initialPosition.global,
                localPosition: _initialPosition.local,
                kind: getKindForPointer(@event.pointer),
                consecutiveTapCount: consecutiveTapCount
            );
            invokeCallback<object?>(
                "onDragStart",
                () =>
                {
                    ((Action)(() => onDragStart!(details)))();
                    return null;
                }
            );
        }
        _start = null;
    }

    internal virtual void _checkDragUpdate(PointerEvent @event, OffsetPair? corrected = null)
    {
        Offset globalPositionLocal = corrected?.global ?? @event.position;
        Offset localPositionLocal = corrected?.local ?? @event.localPosition;
        var details = new TapDragUpdateDetails(
            sourceTimeStamp: @event.timeStamp,
            delta: @event.localDelta,
            globalPosition: globalPositionLocal,
            kind: getKindForPointer(@event.pointer),
            localPosition: localPositionLocal,
            offsetFromOrigin: globalPositionLocal - _initialPosition.global,
            localOffsetFromOrigin: localPositionLocal - _initialPosition.local,
            consecutiveTapCount: consecutiveTapCount
        );
        if (dragUpdateThrottleFrequency is not null)
        {
            _lastDragUpdateDetails = details;
            _dragUpdateThrottleTimer ??= new Timer(
                (
                    dragUpdateThrottleFrequency
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                _handleDragUpdateThrottled
            );
        }
        else
        {
            if (onDragUpdate is not null)
            {
                invokeCallback<object?>(
                    "onDragUpdate",
                    () =>
                    {
                        ((Action)(() => onDragUpdate!(details)))();
                        return null;
                    }
                );
            }
        }
    }

    internal virtual void _checkDragEnd()
    {
        Offset globalPositionLocal = _currentPosition.global;
        Offset localPositionLocal = _currentPosition.local;
        if (_dragUpdateThrottleTimer is not null)
        {
            _dragUpdateThrottleTimer!.cancel();
            _handleDragUpdateThrottled();
        }
        var endDetails = new TapDragEndDetails(
            globalPosition: globalPositionLocal,
            localPosition: localPositionLocal,
            primaryVelocity: 0.0,
            consecutiveTapCount: consecutiveTapCount
        );
        if (onDragEnd is not null)
        {
            invokeCallback<object?>(
                "onDragEnd",
                () =>
                {
                    ((Action)(() => onDragEnd!(endDetails)))();
                    return null;
                }
            );
        }
        _resetTaps();
        _resetDragUpdateThrottle();
    }

    internal virtual void _checkCancel()
    {
        if (!_sentTapDown)
        {
            return;
        }
        if (onCancel is not null)
        {
            invokeCallback<object?>(
                "onCancel",
                () =>
                {
                    onCancel!();
                    return null;
                }
            );
        }
        _resetDragUpdateThrottle();
        _resetTaps();
    }

    internal virtual void _didExceedDeadlineWithEvent(PointerDownEvent @event)
    {
        _didExceedDeadline();
    }

    internal virtual void _didExceedDeadline()
    {
        if (currentDown is not null)
        {
            _checkTapDown(currentDown!);
            if (consecutiveTapCount > 1L)
            {
                resolve(GestureDisposition.accepted);
            }
        }
    }

    internal virtual void _giveUpPointer(long pointer)
    {
        stopTrackingPointer(pointer);
        if (!_acceptedActivePointers.Remove(pointer))
        {
            resolvePointer(pointer, GestureDisposition.rejected);
        }
    }

    internal virtual void _resetTaps()
    {
        _sentTapDown = false;
        _wonArenaForPrimaryPointer = false;
        _primaryPointer = null;
    }

    internal virtual void _resetDragUpdateThrottle()
    {
        if (dragUpdateThrottleFrequency is null)
        {
            return;
        }
        _lastDragUpdateDetails = null;
        if (_dragUpdateThrottleTimer is not null)
        {
            _dragUpdateThrottleTimer!.cancel();
            _dragUpdateThrottleTimer = null;
        }
    }

    internal virtual void _stopDeadlineTimer()
    {
        if (_deadlineTimer is not null)
        {
            _deadlineTimer!.cancel();
            _deadlineTimer = null;
        }
    }

    public virtual PointerDownEvent? currentDown => _down;
    public virtual PointerUpEvent? currentUp => _up;
    public virtual long consecutiveTapCount => _consecutiveTapCount;

    public virtual void _trackTap(PointerDownEvent @event)
    {
        _down = @event;
        _previousButtons = @event.buttons;
        _lastTapOffset = @event.position;
        _originPosition = new OffsetPair(local: @event.localPosition, global: @event.position);
        onTapTrackStart?.Invoke();
    }

    public virtual bool _hasSameButton(long buttons)
    {
        DartRuntimePrimitives.Assert(() => _previousButtons is not null);
        if (
            buttons
            == (
                _previousButtons
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        )
        {
            return true;
        }
        else
        {
            return false;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isWithinConsecutiveTapTolerance(Offset secondTapOffset)
    {
        if (_lastTapOffset is null)
        {
            return false;
        }
        Offset difference =
            secondTapOffset
            - (
                _lastTapOffset
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        return difference.distance <= ConstantsLibrary.kDoubleTapSlop;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _representsSameSeries(PointerDownEvent @event)
    {
        return (_consecutiveTapTimer is not null)
            && _isWithinConsecutiveTapTolerance(@event.position)
            && _hasSameButton(@event.buttons);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _consecutiveTapTimerStart()
    {
        _consecutiveTapTimer ??= new Timer(
            ConstantsLibrary.kDoubleTapTimeout,
            _consecutiveTapTimerTimeout
        );
    }

    public virtual void _consecutiveTapTimerStop()
    {
        if (_consecutiveTapTimer is not null)
        {
            _consecutiveTapTimer!.cancel();
            _consecutiveTapTimer = null;
        }
    }

    public virtual void _consecutiveTapTimerTimeout() { }

    public virtual void _tapTrackerReset()
    {
        _consecutiveTapTimerStop();
        _previousButtons = null;
        _originPosition = null;
        _lastTapOffset = null;
        _consecutiveTapCount = 0L;
        _down = null;
        _up = null;
        onTapTrackReset?.Invoke();
    }
}

public class TapAndHorizontalDragGestureRecognizer : BaseTapAndDragGestureRecognizer
{
    public TapAndHorizontalDragGestureRecognizer(
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null
    )
        : base(debugOwner: debugOwner, supportedDevices: supportedDevices) { }

    internal override bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind)
    {
        return _globalDistanceMoved.abs()
            > EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => new Offset(delta.dx, 0.0);

    internal override double? _getPrimaryValueFromOffset(Offset value) => value.dx;

    public override string debugDescription => "tap and horizontal drag";
}

public class TapAndPanGestureRecognizer : BaseTapAndDragGestureRecognizer
{
    public TapAndPanGestureRecognizer(
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null
    )
        : base(debugOwner: debugOwner, supportedDevices: supportedDevices) { }

    internal override bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind)
    {
        return _globalDistanceMoved.abs()
            > EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => delta;

    internal override double? _getPrimaryValueFromOffset(Offset value) => null;

    public override string debugDescription => "tap and pan";
}

public class TapAndDragGestureRecognizer : BaseTapAndDragGestureRecognizer
{
    public TapAndDragGestureRecognizer(
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null
    )
        : base(debugOwner: debugOwner, supportedDevices: supportedDevices) { }

    internal override bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind)
    {
        return _globalDistanceMoved.abs()
            > EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => delta;

    internal override double? _getPrimaryValueFromOffset(Offset value) => null;

    public override string debugDescription => "tap and pan";
}

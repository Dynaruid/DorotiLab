// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/tap_and_drag.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Gestures;

public static partial class Tap_and_dragLibrary
{
    internal static double _getGlobalDistance(PointerEvent @event, OffsetPair? originPosition)
    {
        DartRuntimePrimitives.Assert(() => (originPosition is not null));
        global::Doroti.Ui.Offset offset__699 = (((PointerEvent)@event).position - originPosition!.global);
        return offset__699.distance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal enum _DragState__tap_and_drag
{
    ready,
    possible,
    accepted
}

public delegate void GestureTapDragDownCallback(TapDragDownDetails details);

public class TapDragDownDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragDownDetails(Offset globalPosition, Offset localPosition, PointerDeviceKind? kind = null, long consecutiveTapCount = default!)
    {
        this.__field_globalPosition = globalPosition;
        this.__field_localPosition = localPosition;
        this.kind = kind;
        this.consecutiveTapCount = consecutiveTapCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
        properties.add(new IntProperty("consecutiveTapCount", this.consecutiveTapCount));
    }

}

public delegate void GestureTapDragUpCallback(TapDragUpDetails details);

public class TapDragUpDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragUpDetails(Offset globalPosition, Offset localPosition, PointerDeviceKind kind, long consecutiveTapCount)
    {
        this.__field_globalPosition = globalPosition;
        this.__field_localPosition = localPosition;
        this.kind = kind;
        this.consecutiveTapCount = consecutiveTapCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
        properties.add(new IntProperty("consecutiveTapCount", this.consecutiveTapCount));
    }

}

public delegate void GestureTapDragStartCallback(TapDragStartDetails details);

public class TapDragStartDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragStartDetails(Offset globalPosition, Offset localPosition, Duration? sourceTimeStamp = null, PointerDeviceKind? kind = null, long consecutiveTapCount = default!)
    {
        this.__field_globalPosition = globalPosition;
        this.__field_localPosition = localPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.kind = kind;
        this.consecutiveTapCount = consecutiveTapCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", this.sourceTimeStamp));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
        properties.add(new IntProperty("consecutiveTapCount", this.consecutiveTapCount));
    }

}

public delegate void GestureTapDragUpdateCallback(TapDragUpdateDetails details);

public class TapDragUpdateDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual Offset delta { get; private set; } = default!;
    public virtual double? primaryDelta { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual Offset offsetFromOrigin { get; private set; } = default!;
    public virtual Offset localOffsetFromOrigin { get; private set; } = default!;
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragUpdateDetails(Offset globalPosition, Offset localPosition, Duration? sourceTimeStamp = null, Offset delta = default, double? primaryDelta = null, PointerDeviceKind? kind = null, Offset offsetFromOrigin = default!, Offset localOffsetFromOrigin = default!, long consecutiveTapCount = default!)
    {
        this.__field_globalPosition = globalPosition;
        this.__field_localPosition = localPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.delta = delta;
        this.primaryDelta = primaryDelta;
        this.kind = kind;
        this.offsetFromOrigin = offsetFromOrigin;
        this.localOffsetFromOrigin = localOffsetFromOrigin;
        this.consecutiveTapCount = consecutiveTapCount;
        System.Diagnostics.Debug.Assert((((primaryDelta is null) || (((DartRuntimePrimitives.RequireValue(primaryDelta) == delta.dx) && (delta.dy == 0.0)))) || (((DartRuntimePrimitives.RequireValue(primaryDelta) == delta.dy) && (delta.dx == 0.0)))));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", this.sourceTimeStamp));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", this.delta));
        properties.add(new DoubleProperty("primaryDelta", this.primaryDelta));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("offsetFromOrigin", this.offsetFromOrigin));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localOffsetFromOrigin", this.localOffsetFromOrigin));
        properties.add(new IntProperty("consecutiveTapCount", this.consecutiveTapCount));
    }

}

public delegate void GestureTapDragEndCallback(TapDragEndDetails endDetails);

public class TapDragEndDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Velocity velocity { get; private set; } = default!;
    public virtual double? primaryVelocity { get; private set; }
    public virtual long consecutiveTapCount { get; private set; } = default!;

    public TapDragEndDetails(Offset globalPosition = default, Offset? localPosition = null, Velocity velocity = default!, double? primaryVelocity = null, long consecutiveTapCount = default!)
    {
        Velocity __velocity = velocity ?? Velocity.zero;
        this.__field_globalPosition = globalPosition;
        this.velocity = __velocity;
        this.primaryVelocity = primaryVelocity;
        this.consecutiveTapCount = consecutiveTapCount;
        this.__field_localPosition = (localPosition ?? globalPosition);
        System.Diagnostics.Debug.Assert((((primaryVelocity is null) || (DartRuntimePrimitives.RequireValue(primaryVelocity) == ((Velocity)__velocity).pixelsPerSecond.dx)) || (DartRuntimePrimitives.RequireValue(primaryVelocity) == ((Velocity)__velocity).pixelsPerSecond.dy)));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Velocity>("velocity", this.velocity));
        properties.add(new DoubleProperty("primaryVelocity", this.primaryVelocity));
        properties.add(new IntProperty("consecutiveTapCount", this.consecutiveTapCount));
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

public abstract class BaseTapAndDragGestureRecognizer : OneSequenceGestureRecognizer, _TapStatusTrackerMixin__tap_and_drag
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
    internal virtual _DragState__tap_and_drag _dragState { get; set; } = _DragState__tap_and_drag.ready;
    internal virtual PointerEvent? _start { get; set; } = default;
    internal virtual OffsetPair _initialPosition { get; set; } = default!;
    internal virtual OffsetPair _currentPosition { get; set; } = default!;
    internal virtual double _globalDistanceMoved { get; set; } = default!;
    internal virtual double _globalDistanceMovedAllAxes { get; set; } = default!;
    internal virtual TapDragUpdateDetails? _lastDragUpdateDetails { get; set; } = default;
    internal virtual Timer? _dragUpdateThrottleTimer { get; set; } = default;
    internal virtual HashSet<long> _acceptedActivePointers { get; private set; } = new HashSet<long>();
    public virtual PointerDownEvent? _down { get; set; } = default;
    public virtual PointerUpEvent? _up { get; set; } = default;
    public virtual long _consecutiveTapCount { get; set; } = 0L;
    public virtual OffsetPair? _originPosition { get; set; } = default;
    public virtual long? _previousButtons { get; set; } = default;
    public virtual Timer? _consecutiveTapTimer { get; set; } = default;
    public virtual Offset? _lastTapOffset { get; set; } = default;
    public virtual Action? onTapTrackStart { get; set; } = default;
    public virtual Action? onTapTrackReset { get; set; } = default;

    protected BaseTapAndDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, bool eagerVictoryOnDrag = true) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
        this.eagerVictoryOnDrag = eagerVictoryOnDrag;
        this._deadline = global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kPressTimeout;
        this.dragStartBehavior = DragStartBehavior.start;
    }

    internal abstract global::Doroti.Ui.Offset _getDeltaForDetails(Offset delta);
    internal abstract double? _getPrimaryValueFromOffset(Offset value);
    internal abstract bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind);
    internal virtual void _handleDragUpdateThrottled()
    {
        DartRuntimePrimitives.Assert(() => (this._lastDragUpdateDetails is not null));
        if ((this.onDragUpdate is not null))
        {
            invokeCallback<object?>("onDragUpdate", () => { ((Action)((() => this.onDragUpdate!(this._lastDragUpdateDetails!))))(); return null; });
        }
        _dragUpdateThrottleTimer = null;
        _lastDragUpdateDetails = null;
    }

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if ((this._primaryPointer is null))
        {
            switch (((PointerEvent)@event).buttons)
            {
                case var __constant38875 when object.Equals(__constant38875, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                    {
                        if (((((((this.onTapDown is null) && (this.onDragStart is null)) && (this.onDragUpdate is null)) && (this.onDragEnd is null)) && (this.onTapUp is null)) && (this.onCancel is null)))
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
            if ((((PointerEvent)@event).pointer != this._primaryPointer))
            {
                return false;
            }
        }
        return base.isPointerAllowed(((PointerDownEvent?)(object?)@event)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if ((object.Equals(this._dragState, _DragState__tap_and_drag.ready)))
        {
            base.addAllowedPointer(@event);
            if (((this._consecutiveTapTimer is not null) && !this._consecutiveTapTimer!.isActive))
            {
                _tapTrackerReset();
            }
            if ((this.maxConsecutiveTap == this._consecutiveTapCount))
            {
                _tapTrackerReset();
            }
            _up = null;
            if (((this._down is not null) && !_representsSameSeries(@event)))
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
            _currentPosition = this._initialPosition;
            _deadlineTimer = new Timer(this._deadline, (() => _didExceedDeadlineWithEvent(@event)));
        }
    }

    public override void handleNonAllowedPointer(PointerDownEvent @event)
    {
        if ((@event.buttons != global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton))
        {
            if (!this._wonArenaForPrimaryPointer)
            {
                base.handleNonAllowedPointer(@event);
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
        if ((pointer != this._primaryPointer))
        {
            return;
        }
        _stopDeadlineTimer();
        DartRuntimePrimitives.Assert(() => !this._acceptedActivePointers.Contains(pointer));
        this._acceptedActivePointers.Add(pointer);
        if ((currentDown is not null))
        {
            _checkTapDown(currentDown!);
        }
        _wonArenaForPrimaryPointer = true;
        if (((this._start is not null) && this.eagerVictoryOnDrag))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._dragState, _DragState__tap_and_drag.accepted)));
            DartRuntimePrimitives.Assert(() => (currentUp is null));
            _acceptDrag(this._start!);
        }
        if (((this._start is not null) && !this.eagerVictoryOnDrag))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._dragState, _DragState__tap_and_drag.possible)));
            DartRuntimePrimitives.Assert(() => (currentUp is null));
            _dragState = _DragState__tap_and_drag.accepted;
            _acceptDrag(this._start!);
        }
        if ((currentUp is not null))
        {
            _checkTapUp(currentUp!);
        }
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        switch (this._dragState)
        {
            case _DragState__tap_and_drag.ready:
                {
                    _checkCancel();
                    resolve(GestureDisposition.rejected);
                    break;
                }
            case _DragState__tap_and_drag.possible:
                {
                    if (this._pastSlopTolerance)
                    {
                        if (this._wonArenaForPrimaryPointer)
                        {
                            if ((currentDown is not null))
                            {
                                if (!this._acceptedActivePointers.Remove(pointer))
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
                        if ((currentUp is not null))
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
        if ((((PointerEvent)@event).pointer != this._primaryPointer))
        {
            return;
        }
        if ((@event is PointerMoveEvent))
        {
            PointerMoveEvent @event__as22088 = (PointerMoveEvent)@event;
            double computedSlop__22136 = global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(((PointerMoveEvent)@event__as22088).kind, gestureSettings);
            bool isSlopPastTolerance__22213 = (Tap_and_dragLibrary._getGlobalDistance(((PointerMoveEvent)@event__as22088), this._originPosition) > computedSlop__22136);
            if (isSlopPastTolerance__22213)
            {
                _consecutiveTapTimerStop();
                _previousButtons = null;
                _lastTapOffset = null;
            }
        }
        else
        {
            if ((@event is PointerUpEvent))
            {
                PointerUpEvent @event__as22451 = (PointerUpEvent)@event;
                _up = ((PointerUpEvent)@event__as22451);
                if ((this._down is not null))
                {
                    _consecutiveTapTimerStop();
                    _consecutiveTapTimerStart();
                }
            }
            else
            {
                if ((@event is PointerCancelEvent))
                {
                    PointerCancelEvent @event__as22620 = (PointerCancelEvent)@event;
                    _tapTrackerReset();
                }
            }
        }
        if ((@event is PointerMoveEvent))
        {
            PointerMoveEvent @event__as43046 = (PointerMoveEvent)@event;
            double computedSlop__44158 = global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(((PointerMoveEvent)@event__as43046).kind, gestureSettings);
            _pastSlopTolerance = (this._pastSlopTolerance || (Tap_and_dragLibrary._getGlobalDistance(((PointerMoveEvent)@event__as43046), this._initialPosition) > computedSlop__44158));
            if ((object.Equals(this._dragState, _DragState__tap_and_drag.accepted)))
            {
                _currentPosition = OffsetPair.CreateFromEventPosition(((PointerMoveEvent)@event__as43046));
                _checkDragUpdate(((PointerMoveEvent)@event__as43046));
            }
            else
            {
                if ((object.Equals(this._dragState, _DragState__tap_and_drag.possible)))
                {
                    if ((this._start is null))
                    {
                        _checkDrag(((PointerMoveEvent)@event__as43046));
                    }
                    if (((this._start is not null) && this._wonArenaForPrimaryPointer))
                    {
                        _dragState = _DragState__tap_and_drag.accepted;
                        _acceptDrag(this._start!);
                    }
                }
            }
        }
        else
        {
            if ((@event is PointerUpEvent))
            {
                PointerUpEvent @event__as45053 = (PointerUpEvent)@event;
                if ((object.Equals(this._dragState, _DragState__tap_and_drag.possible)))
                {
                    stopTrackingIfPointerNoLongerDown(((PointerUpEvent)@event__as45053));
                }
                else
                {
                    if ((object.Equals(this._dragState, _DragState__tap_and_drag.accepted)))
                    {
                        _giveUpPointer(((PointerUpEvent)@event__as45053).pointer);
                    }
                }
            }
            else
            {
                if ((@event is PointerCancelEvent))
                {
                    PointerCancelEvent @event__as45427 = (PointerCancelEvent)@event;
                    _dragState = _DragState__tap_and_drag.ready;
                    _giveUpPointer(((PointerCancelEvent)@event__as45427).pointer);
                }
            }
        }
    }

    public override void rejectGesture(long pointer)
    {
        if ((pointer != this._primaryPointer))
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
        DartRuntimePrimitives.Assert(() => (object.Equals(this._dragState, _DragState__tap_and_drag.accepted)));
        if (!this._wonArenaForPrimaryPointer)
        {
            return;
        }
        if ((object.Equals(this.dragStartBehavior, DragStartBehavior.start)))
        {
            _initialPosition = _initialPosition.op_Add(new OffsetPair(global: ((PointerEvent)@event).delta, local: ((PointerEvent)@event).localDelta));
            _currentPosition = this._initialPosition;
        }
        _checkDragStart(@event);
        global::Doroti.Ui.Offset localDelta__46354 = ((PointerEvent)@event).localDelta;
        if ((!object.Equals(localDelta__46354, Offset.zero)))
        {
            _currentPosition = OffsetPair.CreateFromEventPosition(@event);
            global::Doroti.Ui.Offset correctedLocalPosition__46503 = (((OffsetPair)this._initialPosition).local + localDelta__46354);
            Matrix4? localToGlobalTransform__46586 = ((((PointerEvent)@event).transform is null) ? null : Matrix4.tryInvert(((PointerEvent)@event).transform!));
            global::Doroti.Ui.Offset globalUpdateDelta__46720 = PointerEvent.transformDeltaViaPositions(transform: localToGlobalTransform__46586, untransformedDelta: localDelta__46354, untransformedEndPosition: correctedLocalPosition__46503);
            var updateDelta__46943 = new OffsetPair(local: localDelta__46354, global: globalUpdateDelta__46720);
            _checkDragUpdate(@event, corrected: (this._initialPosition.op_Add(updateDelta__46943)));
        }
    }

    internal virtual void _checkDrag(PointerMoveEvent @event)
    {
        Matrix4? localToGlobalTransform__47207 = ((@event.transform is null) ? null : Matrix4.tryInvert(@event.transform!));
        global::Doroti.Ui.Offset movedLocally__47335 = _getDeltaForDetails(@event.localDelta);
        _globalDistanceMoved += (PointerEvent.transformDeltaViaPositions(transform: localToGlobalTransform__47207, untransformedDelta: movedLocally__47335, untransformedEndPosition: @event.localPosition).distance * Math.Sign(((_getPrimaryValueFromOffset(movedLocally__47335) ?? 1))));
        _globalDistanceMovedAllAxes += (PointerEvent.transformDeltaViaPositions(transform: localToGlobalTransform__47207, untransformedDelta: @event.localDelta, untransformedEndPosition: @event.localPosition).distance * Math.Sign(1L));
        if ((_hasSufficientGlobalDistanceToAccept(@event.kind) || ((this._wonArenaForPrimaryPointer && (this._globalDistanceMovedAllAxes.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computePanSlop(@event.kind, gestureSettings))))))
        {
            _start = @event;
            if (this.eagerVictoryOnDrag)
            {
                _dragState = _DragState__tap_and_drag.accepted;
                if (!this._wonArenaForPrimaryPointer)
                {
                    resolve(GestureDisposition.accepted);
                }
            }
        }
    }

    internal virtual void _checkTapDown(PointerDownEvent @event)
    {
        if (this._sentTapDown)
        {
            return;
        }
        var details__48479 = new TapDragDownDetails(globalPosition: @event.position, localPosition: @event.localPosition, kind: getKindForPointer(@event.pointer), consecutiveTapCount: consecutiveTapCount);
        if ((this.onTapDown is not null))
        {
            invokeCallback<object?>("onTapDown", () => { ((Action)((() => this.onTapDown!(details__48479))))(); return null; });
        }
        _sentTapDown = true;
    }

    internal virtual void _checkTapUp(PointerUpEvent @event)
    {
        if (!this._wonArenaForPrimaryPointer)
        {
            return;
        }
        var upDetails__48932 = new TapDragUpDetails(kind: @event.kind, globalPosition: @event.position, localPosition: @event.localPosition, consecutiveTapCount: consecutiveTapCount);
        if ((this.onTapUp is not null))
        {
            invokeCallback<object?>("onTapUp", () => { ((Action)((() => this.onTapUp!(upDetails__48932))))(); return null; });
        }
        _resetTaps();
        if (!this._acceptedActivePointers.Remove(@event.pointer))
        {
            resolvePointer(@event.pointer, GestureDisposition.rejected);
        }
    }

    internal virtual void _checkDragStart(PointerEvent @event)
    {
        if ((this.onDragStart is not null))
        {
            var details__49457 = new TapDragStartDetails(sourceTimeStamp: ((PointerEvent)@event).timeStamp, globalPosition: ((OffsetPair)this._initialPosition).global, localPosition: ((OffsetPair)this._initialPosition).local, kind: getKindForPointer(((PointerEvent)@event).pointer), consecutiveTapCount: consecutiveTapCount);
            invokeCallback<object?>("onDragStart", () => { ((Action)((() => this.onDragStart!(details__49457))))(); return null; });
        }
        _start = null;
    }

    internal virtual void _checkDragUpdate(PointerEvent @event, OffsetPair? corrected = null)
    {
        global::Doroti.Ui.Offset globalPosition__49925 = (corrected?.global ?? ((PointerEvent)@event).position);
        global::Doroti.Ui.Offset localPosition__49996 = (corrected?.local ?? ((PointerEvent)@event).localPosition);
        var details__50064 = new TapDragUpdateDetails(sourceTimeStamp: ((PointerEvent)@event).timeStamp, delta: ((PointerEvent)@event).localDelta, globalPosition: globalPosition__49925, kind: getKindForPointer(((PointerEvent)@event).pointer), localPosition: localPosition__49996, offsetFromOrigin: (globalPosition__49925 - ((OffsetPair)this._initialPosition).global), localOffsetFromOrigin: (localPosition__49996 - ((OffsetPair)this._initialPosition).local), consecutiveTapCount: consecutiveTapCount);
        if ((this.dragUpdateThrottleFrequency is not null))
        {
            _lastDragUpdateDetails = details__50064;
            _dragUpdateThrottleTimer ??= new Timer(DartRuntimePrimitives.RequireValue(this.dragUpdateThrottleFrequency), this._handleDragUpdateThrottled);
        }
        else
        {
            if ((this.onDragUpdate is not null))
            {
                invokeCallback<object?>("onDragUpdate", () => { ((Action)((() => this.onDragUpdate!(details__50064))))(); return null; });
            }
        }
    }

    internal virtual void _checkDragEnd()
    {
        global::Doroti.Ui.Offset globalPosition__50912 = ((OffsetPair)this._currentPosition).global;
        global::Doroti.Ui.Offset localPosition__50971 = ((OffsetPair)this._currentPosition).local;
        if ((this._dragUpdateThrottleTimer is not null))
        {
            this._dragUpdateThrottleTimer!.cancel();
            _handleDragUpdateThrottled();
        }
        var endDetails__51254 = new TapDragEndDetails(globalPosition: globalPosition__50912, localPosition: localPosition__50971, primaryVelocity: 0.0, consecutiveTapCount: consecutiveTapCount);
        if ((this.onDragEnd is not null))
        {
            invokeCallback<object?>("onDragEnd", () => { ((Action)((() => this.onDragEnd!(endDetails__51254))))(); return null; });
        }
        _resetTaps();
        _resetDragUpdateThrottle();
    }

    internal virtual void _checkCancel()
    {
        if (!this._sentTapDown)
        {
            return;
        }
        if ((this.onCancel is not null))
        {
            invokeCallback<object?>("onCancel", () => { ((Action)(this.onCancel!))(); return null; });
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
        if ((currentDown is not null))
        {
            _checkTapDown(currentDown!);
            if ((consecutiveTapCount > 1L))
            {
                resolve(GestureDisposition.accepted);
            }
        }
    }

    internal virtual void _giveUpPointer(long pointer)
    {
        stopTrackingPointer(pointer);
        if (!this._acceptedActivePointers.Remove(pointer))
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
        if ((this.dragUpdateThrottleFrequency is null))
        {
            return;
        }
        _lastDragUpdateDetails = null;
        if ((this._dragUpdateThrottleTimer is not null))
        {
            this._dragUpdateThrottleTimer!.cancel();
            _dragUpdateThrottleTimer = null;
        }
    }

    internal virtual void _stopDeadlineTimer()
    {
        if ((this._deadlineTimer is not null))
        {
            this._deadlineTimer!.cancel();
            _deadlineTimer = null;
        }
    }

    public virtual PointerDownEvent? currentDown => this._down;
    public virtual PointerUpEvent? currentUp => this._up;
    public virtual long consecutiveTapCount => this._consecutiveTapCount;
    public virtual void _trackTap(PointerDownEvent @event)
    {
        this._down = @event;
        this._previousButtons = @event.buttons;
        this._lastTapOffset = @event.position;
        this._originPosition = new OffsetPair(local: @event.localPosition, global: @event.position);
        this.onTapTrackStart?.Invoke();
    }

    public virtual bool _hasSameButton(long buttons)
    {
        DartRuntimePrimitives.Assert(() => (this._previousButtons is not null));
        if ((buttons == DartRuntimePrimitives.RequireValue(this._previousButtons)))
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
        if ((this._lastTapOffset is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset difference__23425 = (secondTapOffset - DartRuntimePrimitives.RequireValue(this._lastTapOffset));
        return (difference__23425.distance <= global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapSlop);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _representsSameSeries(PointerDownEvent @event)
    {
        return (((this._consecutiveTapTimer is not null) && _isWithinConsecutiveTapTolerance(@event.position)) && _hasSameButton(@event.buttons));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _consecutiveTapTimerStart()
    {
        this._consecutiveTapTimer ??= new Timer(global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapTimeout, this._consecutiveTapTimerTimeout);
    }

    public virtual void _consecutiveTapTimerStop()
    {
        if ((this._consecutiveTapTimer is not null))
        {
            this._consecutiveTapTimer!.cancel();
            this._consecutiveTapTimer = null;
        }
    }

    public virtual void _consecutiveTapTimerTimeout()
    {
    }

    public virtual void _tapTrackerReset()
    {
        _consecutiveTapTimerStop();
        this._previousButtons = null;
        this._originPosition = null;
        this._lastTapOffset = null;
        this._consecutiveTapCount = 0L;
        this._down = null;
        this._up = null;
        this.onTapTrackReset?.Invoke();
    }

}

public class TapAndHorizontalDragGestureRecognizer : BaseTapAndDragGestureRecognizer
{
    public TapAndHorizontalDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices)
    {
    }

    internal override bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind)
    {
        return (_globalDistanceMoved.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => new global::Doroti.Ui.Offset(delta.dx, 0.0);
    internal override double? _getPrimaryValueFromOffset(Offset value) => value.dx;
    public override string debugDescription => "tap and horizontal drag";
}

public class TapAndPanGestureRecognizer : BaseTapAndDragGestureRecognizer
{
    public TapAndPanGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices)
    {
    }

    internal override bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind)
    {
        return (_globalDistanceMoved.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => delta;
    internal override double? _getPrimaryValueFromOffset(Offset value) => null;
    public override string debugDescription => "tap and pan";
}

public class TapAndDragGestureRecognizer : BaseTapAndDragGestureRecognizer
{
    public TapAndDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices)
    {
    }

    internal override bool _hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind)
    {
        return (_globalDistanceMoved.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => delta;
    internal override double? _getPrimaryValueFromOffset(Offset value) => null;
    public override string debugDescription => "tap and pan";
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/monodrag.dart
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

namespace Doroti.Framework.Gestures;

internal enum _DragState__monodrag
{
    ready,
    possible,
    accepted
}

public delegate void GestureDragEndCallback(DragEndDetails details);

public delegate void GestureDragCancelCallback();

public delegate VelocityTracker GestureVelocityTrackerBuilder(PointerEvent @event);

public abstract class DragGestureRecognizer : OneSequenceGestureRecognizer
{
    public virtual DragStartBehavior dragStartBehavior { get; set; } = default!;
    public virtual MultitouchDragStrategy multitouchDragStrategy { get; set; } = default!;
    public virtual Action<DragDownDetails>? onDown { get; set; } = default;
    public virtual Action<DragStartDetails>? onStart { get; set; } = default;
    public virtual Action<DragUpdateDetails>? onUpdate { get; set; } = default;
    public virtual Action<DragEndDetails>? onEnd { get; set; } = default;
    public virtual Action? onCancel { get; set; } = default;
    public virtual double? minFlingDistance { get; set; } = default;
    public virtual double? minFlingVelocity { get; set; } = default;
    public virtual double? maxFlingVelocity { get; set; } = default;
    public virtual bool onlyAcceptDragOnThreshold { get; set; } = default!;
    public virtual Func<PointerEvent, VelocityTracker> velocityTrackerBuilder { get; set; } = default!;
    internal virtual _DragState__monodrag _state { get; set; } = _DragState__monodrag.ready;
    internal virtual OffsetPair _initialPosition { get; set; } = default!;
    internal virtual OffsetPair _pendingDragOffset { get; set; } = default!;
    internal virtual OffsetPair _lastPosition { get; set; } = default!;
    internal virtual Duration? _lastPendingEventTimestamp { get; set; } = default;
    internal virtual long? _initialButtons { get; set; } = default;
    internal virtual Matrix4? _lastTransform { get; set; } = default;
    internal virtual double _globalDistanceMoved { get; set; } = default!;
    internal virtual bool _hasDragThresholdBeenMet { get; set; } = false;
    internal virtual DartMap<long, VelocityTracker> _velocityTrackers { get; private set; } = new DartMap<long, VelocityTracker>();
    internal virtual DartMap<long, Offset> _moveDeltaBeforeFrame { get; private set; } = new DartMap<long, Offset>();
    internal virtual Duration? _frameTimeStamp { get; set; } = default;
    internal virtual Offset _lastUpdatedDeltaForPan { get; set; } = Offset.zero;
    internal virtual List<long> _acceptedActivePointers { get; private set; } = new List<long>();
    internal virtual long? _activePointer { get; set; } = default;

    protected DragGestureRecognizer(object? debugOwner = null, DragStartBehavior dragStartBehavior = DragStartBehavior.start, MultitouchDragStrategy multitouchDragStrategy = MultitouchDragStrategy.latestPointer, Func<PointerEvent, VelocityTracker> velocityTrackerBuilder = default!, bool onlyAcceptDragOnThreshold = false, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
        Func<PointerEvent, VelocityTracker> __velocityTrackerBuilder = velocityTrackerBuilder ?? _defaultBuilder;
        this.dragStartBehavior = dragStartBehavior;
        this.multitouchDragStrategy = multitouchDragStrategy;
        this.velocityTrackerBuilder = __velocityTrackerBuilder;
        this.onlyAcceptDragOnThreshold = onlyAcceptDragOnThreshold;
    }

    internal static VelocityTracker _defaultBuilder(PointerEvent @event) => new VelocityTracker(((PointerEvent)@event).kind);
    internal static bool _defaultButtonAcceptBehavior(long buttons) => (buttons == global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton);
    public virtual OffsetPair lastPosition => this._lastPosition;
    public virtual Duration? debugLastPendingEventTimestamp
    {
        get
        {
            Duration? lastPendingEventTimestamp = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    lastPendingEventTimestamp = this._lastPendingEventTimestamp;
                    return true;
                });
            return lastPendingEventTimestamp;
            return default!;
        }
    }
    public virtual double globalDistanceMoved => this._globalDistanceMoved;
    public abstract bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind);
    public abstract DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind);
    internal abstract global::Doroti.Ui.Offset _getDeltaForDetails(Offset delta);
    internal abstract double? _getPrimaryValueFromOffset(Offset value);
    internal virtual _DragDirection__monodrag? _getPrimaryDragAxis() => null;
    public abstract bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop);
    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if ((this._initialButtons is null))
        {
            if ((((((this.onDown is null) && (this.onStart is null)) && (this.onUpdate is null)) && (this.onEnd is null)) && (this.onCancel is null)))
            {
                return false;
            }
        }
        else
        {
            if ((((PointerEvent)@event).buttons != this._initialButtons))
            {
                return false;
            }
        }
        return base.isPointerAllowed(((PointerDownEvent?)(object?)@event)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _addPointer(PointerEvent @event)
    {
        this._velocityTrackers[((PointerEvent)@event).pointer] = this.velocityTrackerBuilder(@event);
        switch (this._state)
        {
            case _DragState__monodrag.ready:
                {
                    _state = _DragState__monodrag.possible;
                    _initialPosition = new OffsetPair(global: ((PointerEvent)@event).position, local: ((PointerEvent)@event).localPosition);
                    _lastPosition = this._initialPosition;
                    _pendingDragOffset = OffsetPair.zero;
                    _globalDistanceMoved = 0.0;
                    _lastPendingEventTimestamp = ((PointerEvent)@event).timeStamp;
                    _lastTransform = ((PointerEvent)@event).transform;
                    _checkDown();
                    break;
                }
            case _DragState__monodrag.possible:
                {
                    break;
                }
            case _DragState__monodrag.accepted:
                {
                    resolve(GestureDisposition.accepted);
                    break;
                }
        }
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        if ((object.Equals(this._state, _DragState__monodrag.ready)))
        {
            _initialButtons = @event.buttons;
        }
        _addPointer(@event);
    }

    public override void addAllowedPointerPanZoom(PointerPanZoomStartEvent @event)
    {
        base.addAllowedPointerPanZoom(@event);
        startTrackingPointer(@event.pointer, @event.transform);
        if ((object.Equals(this._state, _DragState__monodrag.ready)))
        {
            _initialButtons = global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton;
        }
        _addPointer(@event);
    }

    internal virtual bool _shouldTrackMoveEvent(long pointer)
    {
        bool result = default!;
        switch (this.multitouchDragStrategy)
        {
            case MultitouchDragStrategy.sumAllPointers:
            case MultitouchDragStrategy.averageBoundaryPointers:
                {
                    result = true;
                    break;
                }
            case MultitouchDragStrategy.latestPointer:
                {
                    result = ((this._activePointer is null) || (pointer == this._activePointer));
                    break;
                }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _recordMoveDeltaForMultitouch(long pointer, Offset localDelta)
    {
        if ((!object.Equals(this.multitouchDragStrategy, MultitouchDragStrategy.averageBoundaryPointers)))
        {
            DartRuntimePrimitives.Assert(() => (this._frameTimeStamp is null));
            DartRuntimePrimitives.Assert(() => (checked((long)(this._moveDeltaBeforeFrame.Count)) == 0));
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._frameTimeStamp, SchedulerBinding.instance.currentSystemFrameTimeStamp)));
        if (((!object.Equals(this._state, _DragState__monodrag.accepted)) || (object.Equals(localDelta, Offset.zero))))
        {
            return;
        }
        if (this._moveDeltaBeforeFrame.ContainsKey(pointer))
        {
            global::Doroti.Ui.Offset offset = DartRuntimePrimitives.RequireValue(this._moveDeltaBeforeFrame.GetValueOrDefault(pointer));
            this._moveDeltaBeforeFrame[pointer] = (offset + localDelta);
        }
        else
        {
            this._moveDeltaBeforeFrame[pointer] = localDelta;
        }
    }

    internal virtual double _getSumDelta(long pointer, bool positive, _DragDirection__monodrag axis)
    {
        var sum = 0.0;
        if (!this._moveDeltaBeforeFrame.ContainsKey(pointer))
        {
            return sum;
        }
        global::Doroti.Ui.Offset offset = DartRuntimePrimitives.RequireValue(this._moveDeltaBeforeFrame.GetValueOrDefault(pointer));
        if (positive)
        {
            if ((object.Equals(axis, _DragDirection__monodrag.vertical)))
            {
                sum = Math.Max(offset.dy, 0.0);
            }
            else
            {
                sum = Math.Max(offset.dx, 0.0);
            }
        }
        else
        {
            if ((object.Equals(axis, _DragDirection__monodrag.vertical)))
            {
                sum = Math.Min(offset.dy, 0.0);
            }
            else
            {
                sum = Math.Min(offset.dx, 0.0);
            }
        }
        return sum;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _getMaxSumDeltaPointer(bool positive, _DragDirection__monodrag axis)
    {
        if ((checked((long)(this._moveDeltaBeforeFrame.Count)) == 0))
        {
            return null;
        }
        long? ret = default!;
        double? max = default!;
        double sum = default!;
        foreach (long pointerLocal in this._moveDeltaBeforeFrame.Keys)
        {
            sum = _getSumDelta(pointer: pointerLocal, positive: positive, axis: axis);
            if ((ret is null))
            {
                ret = pointerLocal;
                max = sum;
            }
            else
            {
                if (positive)
                {
                    if ((sum > DartRuntimePrimitives.RequireValue(max)))
                    {
                        ret = pointerLocal;
                        max = sum;
                    }
                }
                else
                {
                    if ((sum < DartRuntimePrimitives.RequireValue(max)))
                    {
                        ret = pointerLocal;
                        max = sum;
                    }
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (ret is not null));
        return ret;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _resolveLocalDeltaForMultitouch(long pointer, Offset localDelta)
    {
        if ((!object.Equals(this.multitouchDragStrategy, MultitouchDragStrategy.averageBoundaryPointers)))
        {
            if ((this._frameTimeStamp is not null))
            {
                this._moveDeltaBeforeFrame.Clear();
                _frameTimeStamp = null;
                _lastUpdatedDeltaForPan = Offset.zero;
            }
            return localDelta;
        }
        Duration currentSystemFrameTimeStampLocal = SchedulerBinding.instance.currentSystemFrameTimeStamp;
        if ((!object.Equals(this._frameTimeStamp, currentSystemFrameTimeStampLocal)))
        {
            this._moveDeltaBeforeFrame.Clear();
            _lastUpdatedDeltaForPan = Offset.zero;
            _frameTimeStamp = currentSystemFrameTimeStampLocal;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._frameTimeStamp, SchedulerBinding.instance.currentSystemFrameTimeStamp)));
        _DragDirection__monodrag? axisLocal = _getPrimaryDragAxis();
        if ((((!object.Equals(this._state, _DragState__monodrag.accepted)) || (object.Equals(localDelta, Offset.zero))) || (((checked((long)(this._moveDeltaBeforeFrame.Count)) == 0) && (axisLocal is not null)))))
        {
            return localDelta;
        }
        double dxLocal = default!;
        double dyLocal = default!;
        if ((object.Equals(axisLocal, _DragDirection__monodrag.horizontal)))
        {
            dxLocal = _resolveDelta(pointer: pointer, axis: _DragDirection__monodrag.horizontal, localDelta: localDelta);
            DartRuntimePrimitives.Assert(() => (dxLocal.abs() <= localDelta.dx.abs()));
            dyLocal = 0.0;
        }
        else
        {
            if ((object.Equals(axisLocal, _DragDirection__monodrag.vertical)))
            {
                dxLocal = 0.0;
                dyLocal = _resolveDelta(pointer: pointer, axis: _DragDirection__monodrag.vertical, localDelta: localDelta);
                DartRuntimePrimitives.Assert(() => (dyLocal.abs() <= localDelta.dy.abs()));
            }
            else
            {
                double averageX = _resolveDeltaForPanGesture(axis: _DragDirection__monodrag.horizontal, localDelta: localDelta);
                double averageY = _resolveDeltaForPanGesture(axis: _DragDirection__monodrag.vertical, localDelta: localDelta);
                global::Doroti.Ui.Offset updatedDelta = (new global::Doroti.Ui.Offset(averageX, averageY) - this._lastUpdatedDeltaForPan);
                _lastUpdatedDeltaForPan = new global::Doroti.Ui.Offset(averageX, averageY);
                dxLocal = updatedDelta.dx;
                dyLocal = updatedDelta.dy;
            }
        }
        return new global::Doroti.Ui.Offset(dxLocal, dyLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveDelta(long pointer, _DragDirection__monodrag axis, Offset localDelta)
    {
        bool positiveLocal = ((object.Equals(axis, _DragDirection__monodrag.horizontal)) ? (localDelta.dx > 0L) : (localDelta.dy > 0L));
        double delta = ((object.Equals(axis, _DragDirection__monodrag.horizontal)) ? localDelta.dx : localDelta.dy);
        long? maxSumDeltaPointer = _getMaxSumDeltaPointer(positive: positiveLocal, axis: axis);
        DartRuntimePrimitives.Assert(() => (maxSumDeltaPointer is not null));
        if ((maxSumDeltaPointer == pointer))
        {
            return delta;
        }
        else
        {
            double maxSumDelta = _getSumDelta(pointer: DartRuntimePrimitives.RequireValue(maxSumDeltaPointer), positive: positiveLocal, axis: axis);
            double curPointerSumDelta = _getSumDelta(pointer: pointer, positive: positiveLocal, axis: axis);
            if (positiveLocal)
            {
                if (((curPointerSumDelta + delta) > maxSumDelta))
                {
                    return ((curPointerSumDelta + delta) - maxSumDelta);
                }
                else
                {
                    return 0.0;
                }
            }
            else
            {
                if (((curPointerSumDelta + delta) < maxSumDelta))
                {
                    return ((curPointerSumDelta + delta) - maxSumDelta);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveDeltaForPanGesture(_DragDirection__monodrag axis, Offset localDelta)
    {
        double delta = ((object.Equals(axis, _DragDirection__monodrag.horizontal)) ? localDelta.dx : localDelta.dy);
        long pointerCount = checked((long)(this._acceptedActivePointers.Count));
        DartRuntimePrimitives.Assert(() => (pointerCount >= 1L));
        var sum = delta;
        foreach (global::Doroti.Ui.Offset offset in this._moveDeltaBeforeFrame.Values)
        {
            if ((object.Equals(axis, _DragDirection__monodrag.horizontal)))
            {
                sum += offset.dx;
            }
            else
            {
                sum += offset.dy;
            }
        }
        return (sum / pointerCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._state, _DragState__monodrag.ready)));
        if ((!((PointerEvent)@event).synthesized && (((((@event is PointerDownEvent) || (@event is PointerMoveEvent)) || (@event is PointerPanZoomStartEvent)) || (@event is PointerPanZoomUpdateEvent)))))
        {
            global::Doroti.Ui.Offset positionLocal = (@event switch { PointerPanZoomStartEvent __object24693 => Offset.zero, PointerPanZoomUpdateEvent __object24744 => ((PointerPanZoomUpdateEvent)((PointerPanZoomUpdateEvent)__object24744)).pan, _ => ((PointerEvent)@event).localPosition });
            this._velocityTrackers.GetValueOrDefault(((PointerEvent)@event).pointer)!.addPosition(((PointerEvent)@event).timeStamp, positionLocal);
        }
        if (((@event is PointerMoveEvent) && (((PointerMoveEvent)@event).buttons != this._initialButtons)))
        {
            PointerMoveEvent @event__as24923 = (PointerMoveEvent)@event;
            _giveUpPointer(((PointerMoveEvent)@event__as24923).pointer);
            return;
        }
        if (((((@event is PointerMoveEvent) || (@event is PointerPanZoomUpdateEvent))) && _shouldTrackMoveEvent(((PointerEvent)@event).pointer)))
        {
            global::Doroti.Ui.Offset deltaLocal = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).delta : (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).panDelta);
            global::Doroti.Ui.Offset localDeltaLocal = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).localDelta : (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).localPanDelta);
            global::Doroti.Ui.Offset positionAlternate = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).position : ((((PointerEvent)@event).position + (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).pan)));
            global::Doroti.Ui.Offset localPositionLocal = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).localPosition : ((((PointerEvent)@event).localPosition + (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).localPan)));
            _lastPosition = new OffsetPair(local: DartRuntimePrimitives.RequireValue(localPositionLocal), global: positionAlternate);
            global::Doroti.Ui.Offset resolvedDelta = _resolveLocalDeltaForMultitouch(((PointerEvent)@event).pointer, localDeltaLocal);
            switch (this._state)
            {
                case _DragState__monodrag.ready or _DragState__monodrag.possible:
                    {
                        _pendingDragOffset = _pendingDragOffset.op_Add(new OffsetPair(local: localDeltaLocal, global: deltaLocal));
                        _lastPendingEventTimestamp = ((PointerEvent)@event).timeStamp;
                        _lastTransform = ((PointerEvent)@event).transform;
                        global::Doroti.Ui.Offset movedLocally = _getDeltaForDetails(localDeltaLocal);
                        Matrix4? localToGlobalTransform = ((((PointerEvent)@event).transform is null) ? null : Matrix4.tryInvert(((PointerEvent)@event).transform!));
                        _globalDistanceMoved += (PointerEvent.transformDeltaViaPositions(transform: localToGlobalTransform, untransformedDelta: movedLocally, untransformedEndPosition: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(localPositionLocal))).distance * Math.Sign(((_getPrimaryValueFromOffset(movedLocally) ?? 1))));
                        if (hasSufficientGlobalDistanceToAccept(((PointerEvent)@event).kind, gestureSettings?.touchSlop))
                        {
                            _hasDragThresholdBeenMet = true;
                            if (this._acceptedActivePointers.Contains(((PointerEvent)@event).pointer))
                            {
                                _checkDrag(((PointerEvent)@event).pointer);
                            }
                            else
                            {
                                resolve(GestureDisposition.accepted);
                            }
                        }
                        break;
                    }
                case _DragState__monodrag.accepted:
                    {
                        _checkUpdate(sourceTimeStamp: ((PointerEvent)@event).timeStamp, delta: _getDeltaForDetails(resolvedDelta), primaryDelta: _getPrimaryValueFromOffset(resolvedDelta), globalPosition: positionAlternate, localPosition: DartRuntimePrimitives.RequireValue(localPositionLocal), pointer: ((PointerEvent)@event).pointer);
                        break;
                    }
            }
            _recordMoveDeltaForMultitouch(((PointerEvent)@event).pointer, localDeltaLocal);
        }
        if (@event is PointerUpEvent or PointerCancelEvent or PointerPanZoomEndEvent)
        {
            _giveUpPointer(((PointerEvent)@event).pointer);
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => !this._acceptedActivePointers.Contains(pointer));
        this._acceptedActivePointers.Add(pointer);
        _activePointer = pointer;
        if ((!this.onlyAcceptDragOnThreshold || this._hasDragThresholdBeenMet))
        {
            _checkDrag(pointer);
        }
    }

    public override void rejectGesture(long pointer)
    {
        _giveUpPointer(pointer);
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._state, _DragState__monodrag.ready)));
        switch (this._state)
        {
            case _DragState__monodrag.ready:
                {
                    break;
                }
            case _DragState__monodrag.possible:
                {
                    resolve(GestureDisposition.rejected);
                    _checkCancel();
                    break;
                }
            case _DragState__monodrag.accepted:
                {
                    _checkEnd(pointer);
                    break;
                }
        }
        _hasDragThresholdBeenMet = false;
        this._velocityTrackers.Clear();
        _initialButtons = null;
        _state = _DragState__monodrag.ready;
    }

    internal virtual void _giveUpPointer(long pointer)
    {
        stopTrackingPointer(pointer);
        if (!this._acceptedActivePointers.Remove(pointer))
        {
            resolvePointer(pointer, GestureDisposition.rejected);
        }
        this._moveDeltaBeforeFrame.remove(pointer);
        if ((this._activePointer == pointer))
        {
            _activePointer = ((checked((long)(this._acceptedActivePointers.Count)) != 0) ? this._acceptedActivePointers.First() : null);
        }
    }

    internal virtual void _checkDown()
    {
        if ((this.onDown is not null))
        {
            var details = new DragDownDetails(globalPosition: ((OffsetPair)this._initialPosition).global, localPosition: ((OffsetPair)this._initialPosition).local);
            invokeCallback<object?>("onDown", () => { ((Action)((() => this.onDown!(details))))(); return null; });
        }
    }

    internal virtual void _checkDrag(long pointer)
    {
        if ((object.Equals(this._state, _DragState__monodrag.accepted)))
        {
            return;
        }
        _state = _DragState__monodrag.accepted;
        OffsetPair deltaLocal = this._pendingDragOffset;
        Duration? timestamp = this._lastPendingEventTimestamp;
        Matrix4? transformLocal = this._lastTransform;
        global::Doroti.Ui.Offset localUpdateDelta = default!;
        switch (this.dragStartBehavior)
        {
            case DragStartBehavior.start:
                {
                    _initialPosition = (this._initialPosition.op_Add(deltaLocal));
                    localUpdateDelta = Offset.zero;
                    break;
                }
            case DragStartBehavior.down:
                {
                    localUpdateDelta = _getDeltaForDetails(((OffsetPair)deltaLocal).local);
                    break;
                }
        }
        _pendingDragOffset = OffsetPair.zero;
        _lastPendingEventTimestamp = null;
        _lastTransform = null;
        _checkStart(timestamp, pointer);
        if (((!object.Equals(localUpdateDelta, Offset.zero)) && (this.onUpdate is not null)))
        {
            Matrix4? localToGlobal = ((transformLocal is not null) ? Matrix4.tryInvert(transformLocal) : null);
            global::Doroti.Ui.Offset correctedLocalPosition = (((OffsetPair)this._initialPosition).local + localUpdateDelta);
            global::Doroti.Ui.Offset globalUpdateDelta = PointerEvent.transformDeltaViaPositions(untransformedEndPosition: correctedLocalPosition, untransformedDelta: localUpdateDelta, transform: localToGlobal);
            var updateDelta = new OffsetPair(local: localUpdateDelta, global: globalUpdateDelta);
            OffsetPair correctedPosition = (this._initialPosition.op_Add(updateDelta));
            _checkUpdate(sourceTimeStamp: timestamp, delta: localUpdateDelta, primaryDelta: _getPrimaryValueFromOffset(localUpdateDelta), globalPosition: ((OffsetPair)correctedPosition).global, localPosition: ((OffsetPair)correctedPosition).local, pointer: pointer);
        }
        resolve(GestureDisposition.accepted);
    }

    internal virtual void _checkStart(Duration? timestamp, long pointer)
    {
        if ((this.onStart is not null))
        {
            var details = new DragStartDetails(sourceTimeStamp: timestamp, globalPosition: ((OffsetPair)this._initialPosition).global, localPosition: ((OffsetPair)this._initialPosition).local, kind: getKindForPointer(pointer));
            invokeCallback<object?>("onStart", () => { ((Action)((() => this.onStart!(details))))(); return null; });
        }
    }

    internal virtual void _checkUpdate(Duration? sourceTimeStamp = null, Offset delta = default!, double? primaryDelta = null, Offset globalPosition = default!, Offset? localPosition = null, long pointer = default!)
    {
        if ((this.onUpdate is not null))
        {
            var details = new DragUpdateDetails(sourceTimeStamp: sourceTimeStamp, delta: delta, primaryDelta: primaryDelta, globalPosition: globalPosition, localPosition: localPosition, kind: getKindForPointer(pointer));
            invokeCallback<object?>("onUpdate", () => { ((Action)((() => this.onUpdate!(details))))(); return null; });
        }
    }

    internal virtual void _checkEnd(long pointer)
    {
        if ((this.onEnd is null))
        {
            return;
        }
        VelocityTracker tracker = this._velocityTrackers.GetValueOrDefault(pointer)!;
        VelocityEstimate? estimate = tracker.getVelocityEstimate();
        DragEndDetails? details = default!;
        Func<string> debugReport = default!;
        if ((estimate is null))
        {
            debugReport = (() => "Could not estimate velocity.");
        }
        else
        {
            details = considerFling(estimate, ((VelocityTracker)tracker).kind);
            debugReport = (((details is not null)) ? (() => $"{estimate}; fling at {details!.velocity}.") : (() => $"{estimate}; judged to not be a fling."));
        }
        details ??= new DragEndDetails(primaryVelocity: 0.0, globalPosition: ((OffsetPair)this._lastPosition).global, localPosition: ((OffsetPair)this._lastPosition).local);
        invokeCallback<object?>("onEnd", () => { ((Action)((() => this.onEnd!(details!))))(); return null; }, debugReport);
    }

    internal virtual void _checkCancel()
    {
        if ((this.onCancel is not null))
        {
            invokeCallback<object?>("onCancel", () => { ((Action)(this.onCancel!))(); return null; });
        }
    }

    public override void dispose()
    {
        this._velocityTrackers.Clear();
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<DragStartBehavior>("start behavior", this.dragStartBehavior));
    }

}

public class VerticalDragGestureRecognizer : DragGestureRecognizer
{
    public VerticalDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? DragGestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    public override bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        double minVelocity = (minFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity);
        double minDistance = (minFlingDistance ?? global::Doroti.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings));
        return ((((VelocityEstimate)estimate).pixelsPerSecond.dy.abs() > minVelocity) && (((VelocityEstimate)estimate).offset.dy.abs() > minDistance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        double maxVelocity = (maxFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity);
        double dyLocal = Dart_uiLibrary.clampDouble(((VelocityEstimate)estimate).pixelsPerSecond.dy, -maxVelocity, maxVelocity);
        return new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new global::Doroti.Ui.Offset(0, dyLocal)), primaryVelocity: dyLocal, globalPosition: ((OffsetPair)lastPosition).global, localPosition: ((OffsetPair)lastPosition).local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return (globalDistanceMoved.abs() > global::Doroti.Framework.Gestures.EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => new global::Doroti.Ui.Offset(0.0, delta.dy);
    internal override double? _getPrimaryValueFromOffset(Offset value) => value.dy;
    internal override _DragDirection__monodrag? _getPrimaryDragAxis() => _DragDirection__monodrag.vertical;
    public override string debugDescription => "vertical drag";
}

public class HorizontalDragGestureRecognizer : DragGestureRecognizer
{
    public HorizontalDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? DragGestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    public override bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        double minVelocity = (minFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity);
        double minDistance = (minFlingDistance ?? global::Doroti.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings));
        return ((((VelocityEstimate)estimate).pixelsPerSecond.dx.abs() > minVelocity) && (((VelocityEstimate)estimate).offset.dx.abs() > minDistance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        double maxVelocity = (maxFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity);
        double dxLocal = Dart_uiLibrary.clampDouble(((VelocityEstimate)estimate).pixelsPerSecond.dx, -maxVelocity, maxVelocity);
        return new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new global::Doroti.Ui.Offset(dxLocal, 0)), primaryVelocity: dxLocal, globalPosition: ((OffsetPair)_lastPosition).global, localPosition: ((OffsetPair)_lastPosition).local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return (globalDistanceMoved.abs() > global::Doroti.Framework.Gestures.EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => new global::Doroti.Ui.Offset(delta.dx, 0.0);
    internal override double? _getPrimaryValueFromOffset(Offset value) => value.dx;
    internal override _DragDirection__monodrag? _getPrimaryDragAxis() => _DragDirection__monodrag.horizontal;
    public override string debugDescription => "horizontal drag";
}

public class PanGestureRecognizer : DragGestureRecognizer
{
    public PanGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? DragGestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    public override bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        double minVelocity = (minFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity);
        double minDistance = (minFlingDistance ?? global::Doroti.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings));
        return ((((VelocityEstimate)estimate).pixelsPerSecond.distanceSquared > (minVelocity * minVelocity)) && (((VelocityEstimate)estimate).offset.distanceSquared > (minDistance * minDistance)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        Velocity velocityLocal = new Velocity(pixelsPerSecond: ((VelocityEstimate)estimate).pixelsPerSecond).clampMagnitude((minFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity), (maxFlingVelocity ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity));
        return new DragEndDetails(velocity: velocityLocal, globalPosition: ((OffsetPair)lastPosition).global, localPosition: ((OffsetPair)lastPosition).local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return (globalDistanceMoved.abs() > global::Doroti.Framework.Gestures.EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => delta;
    internal override double? _getPrimaryValueFromOffset(Offset value) => null;
    public override string debugDescription => "pan";
}

internal enum _DragDirection__monodrag
{
    horizontal,
    vertical
}


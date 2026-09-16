// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/monodrag.dart
using Doroti.Runtime;
using Doroti.Ui;

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

    internal static VelocityTracker _defaultBuilder(PointerEvent @event) => new VelocityTracker(@event.kind);
    internal new static bool _defaultButtonAcceptBehavior(long buttons) => buttons == EventsLibrary.kPrimaryButton;
    public virtual OffsetPair lastPosition => _lastPosition;
    public virtual Duration? debugLastPendingEventTimestamp
    {
        get
        {
            Duration? lastPendingEventTimestamp = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    lastPendingEventTimestamp = _lastPendingEventTimestamp;
                    return true;
                });
            return lastPendingEventTimestamp;
        }
    }
    public virtual double globalDistanceMoved => _globalDistanceMoved;
    public abstract bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind);
    public abstract DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind);
    internal abstract Offset _getDeltaForDetails(Offset delta);
    internal abstract double? _getPrimaryValueFromOffset(Offset value);
    internal virtual _DragDirection__monodrag? _getPrimaryDragAxis() => null;
    public abstract bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop);
    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if (_initialButtons is null)
        {
            if ((onDown is null) && (onStart is null) && (onUpdate is null) && (onEnd is null) && (onCancel is null))
            {
                return false;
            }
        }
        else
        {
            if (@event.buttons != _initialButtons)
            {
                return false;
            }
        }
        return base.isPointerAllowed(((PointerDownEvent?)(object?)@event)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _addPointer(PointerEvent @event)
    {
        _velocityTrackers[@event.pointer] = velocityTrackerBuilder(@event);
        switch (_state)
        {
            case _DragState__monodrag.ready:
                {
                    _state = _DragState__monodrag.possible;
                    _initialPosition = new OffsetPair(global: @event.position, local: @event.localPosition);
                    _lastPosition = _initialPosition;
                    _pendingDragOffset = OffsetPair.zero;
                    _globalDistanceMoved = 0.0;
                    _lastPendingEventTimestamp = @event.timeStamp;
                    _lastTransform = @event.transform;
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
        if (Equals(_state, _DragState__monodrag.ready))
        {
            _initialButtons = @event.buttons;
        }
        _addPointer(@event);
    }

    public override void addAllowedPointerPanZoom(PointerPanZoomStartEvent @event)
    {
        base.addAllowedPointerPanZoom(@event);
        startTrackingPointer(@event.pointer, @event.transform);
        if (Equals(_state, _DragState__monodrag.ready))
        {
            _initialButtons = EventsLibrary.kPrimaryButton;
        }
        _addPointer(@event);
    }

    internal virtual bool _shouldTrackMoveEvent(long pointer)
    {
        bool result = default!;
        switch (multitouchDragStrategy)
        {
            case MultitouchDragStrategy.sumAllPointers:
            case MultitouchDragStrategy.averageBoundaryPointers:
                {
                    result = true;
                    break;
                }
            case MultitouchDragStrategy.latestPointer:
                {
                    result = (_activePointer is null) || (pointer == _activePointer);
                    break;
                }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _recordMoveDeltaForMultitouch(long pointer, Offset localDelta)
    {
        if (!Equals(multitouchDragStrategy, MultitouchDragStrategy.averageBoundaryPointers))
        {
            DartRuntimePrimitives.Assert(() => _frameTimeStamp is null);
            DartRuntimePrimitives.Assert(() => checked((long)_moveDeltaBeforeFrame.Count) == 0);
            return;
        }
        DartRuntimePrimitives.Assert(() => Equals(_frameTimeStamp, SchedulerBinding.instance.currentSystemFrameTimeStamp));
        if ((!Equals(_state, _DragState__monodrag.accepted)) || Equals(localDelta, Offset.zero))
        {
            return;
        }
        if (_moveDeltaBeforeFrame.ContainsKey(pointer))
        {
            Offset offset = DartRuntimePrimitives.RequireValue(_moveDeltaBeforeFrame.GetValueOrDefault(pointer));
            _moveDeltaBeforeFrame[pointer] = offset + localDelta;
        }
        else
        {
            _moveDeltaBeforeFrame[pointer] = localDelta;
        }
    }

    internal virtual double _getSumDelta(long pointer, bool positive, _DragDirection__monodrag axis)
    {
        var sum = 0.0;
        if (!_moveDeltaBeforeFrame.ContainsKey(pointer))
        {
            return sum;
        }
        Offset offset = DartRuntimePrimitives.RequireValue(_moveDeltaBeforeFrame.GetValueOrDefault(pointer));
        if (positive)
        {
            if (Equals(axis, _DragDirection__monodrag.vertical))
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
            if (Equals(axis, _DragDirection__monodrag.vertical))
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
        if (checked((long)_moveDeltaBeforeFrame.Count) == 0)
        {
            return null;
        }
        long? ret = default!;
        double? max = default!;
        double sum = default!;
        foreach (long pointerLocal in _moveDeltaBeforeFrame.Keys)
        {
            sum = _getSumDelta(pointer: pointerLocal, positive: positive, axis: axis);
            if (ret is null)
            {
                ret = pointerLocal;
                max = sum;
            }
            else
            {
                if (positive)
                {
                    if (sum > DartRuntimePrimitives.RequireValue(max))
                    {
                        ret = pointerLocal;
                        max = sum;
                    }
                }
                else
                {
                    if (sum < DartRuntimePrimitives.RequireValue(max))
                    {
                        ret = pointerLocal;
                        max = sum;
                    }
                }
            }
        }
        DartRuntimePrimitives.Assert(() => ret is not null);
        return ret;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _resolveLocalDeltaForMultitouch(long pointer, Offset localDelta)
    {
        if (!Equals(multitouchDragStrategy, MultitouchDragStrategy.averageBoundaryPointers))
        {
            if (_frameTimeStamp is not null)
            {
                _moveDeltaBeforeFrame.Clear();
                _frameTimeStamp = null;
                _lastUpdatedDeltaForPan = Offset.zero;
            }
            return localDelta;
        }
        Duration currentSystemFrameTimeStampLocal = SchedulerBinding.instance.currentSystemFrameTimeStamp;
        if (!Equals(_frameTimeStamp, currentSystemFrameTimeStampLocal))
        {
            _moveDeltaBeforeFrame.Clear();
            _lastUpdatedDeltaForPan = Offset.zero;
            _frameTimeStamp = currentSystemFrameTimeStampLocal;
        }
        DartRuntimePrimitives.Assert(() => Equals(_frameTimeStamp, SchedulerBinding.instance.currentSystemFrameTimeStamp));
        _DragDirection__monodrag? axisLocal = _getPrimaryDragAxis();
        if ((!Equals(_state, _DragState__monodrag.accepted)) || Equals(localDelta, Offset.zero) || (checked((long)_moveDeltaBeforeFrame.Count) == 0) && (axisLocal is not null))
        {
            return localDelta;
        }
        double dxLocal = default!;
        double dyLocal = default!;
        if (Equals(axisLocal, _DragDirection__monodrag.horizontal))
        {
            dxLocal = _resolveDelta(pointer: pointer, axis: _DragDirection__monodrag.horizontal, localDelta: localDelta);
            DartRuntimePrimitives.Assert(() => dxLocal.abs() <= localDelta.dx.abs());
            dyLocal = 0.0;
        }
        else
        {
            if (Equals(axisLocal, _DragDirection__monodrag.vertical))
            {
                dxLocal = 0.0;
                dyLocal = _resolveDelta(pointer: pointer, axis: _DragDirection__monodrag.vertical, localDelta: localDelta);
                DartRuntimePrimitives.Assert(() => dyLocal.abs() <= localDelta.dy.abs());
            }
            else
            {
                double averageX = _resolveDeltaForPanGesture(axis: _DragDirection__monodrag.horizontal, localDelta: localDelta);
                double averageY = _resolveDeltaForPanGesture(axis: _DragDirection__monodrag.vertical, localDelta: localDelta);
                Offset updatedDelta = new Offset(averageX, averageY) - _lastUpdatedDeltaForPan;
                _lastUpdatedDeltaForPan = new Offset(averageX, averageY);
                dxLocal = updatedDelta.dx;
                dyLocal = updatedDelta.dy;
            }
        }
        return new Offset(dxLocal, dyLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveDelta(long pointer, _DragDirection__monodrag axis, Offset localDelta)
    {
        bool positiveLocal = Equals(axis, _DragDirection__monodrag.horizontal) ? (localDelta.dx > 0L) : (localDelta.dy > 0L);
        double delta = Equals(axis, _DragDirection__monodrag.horizontal) ? localDelta.dx : localDelta.dy;
        long? maxSumDeltaPointer = _getMaxSumDeltaPointer(positive: positiveLocal, axis: axis);
        DartRuntimePrimitives.Assert(() => maxSumDeltaPointer is not null);
        if (maxSumDeltaPointer == pointer)
        {
            return delta;
        }
        else
        {
            double maxSumDelta = _getSumDelta(pointer: DartRuntimePrimitives.RequireValue(maxSumDeltaPointer), positive: positiveLocal, axis: axis);
            double curPointerSumDelta = _getSumDelta(pointer: pointer, positive: positiveLocal, axis: axis);
            if (positiveLocal)
            {
                if ((curPointerSumDelta + delta) > maxSumDelta)
                {
                    return curPointerSumDelta + delta - maxSumDelta;
                }
                else
                {
                    return 0.0;
                }
            }
            else
            {
                if ((curPointerSumDelta + delta) < maxSumDelta)
                {
                    return curPointerSumDelta + delta - maxSumDelta;
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
        double delta = Equals(axis, _DragDirection__monodrag.horizontal) ? localDelta.dx : localDelta.dy;
        long pointerCount = checked(_acceptedActivePointers.Count);
        DartRuntimePrimitives.Assert(() => pointerCount >= 1L);
        var sum = delta;
        foreach (Offset offset in _moveDeltaBeforeFrame.Values)
        {
            if (Equals(axis, _DragDirection__monodrag.horizontal))
            {
                sum += offset.dx;
            }
            else
            {
                sum += offset.dy;
            }
        }
        return sum / pointerCount;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !Equals(_state, _DragState__monodrag.ready));
        if (!@event.synthesized && ((@event is PointerDownEvent) || (@event is PointerMoveEvent) || (@event is PointerPanZoomStartEvent) || (@event is PointerPanZoomUpdateEvent)))
        {
            Offset positionLocal = @event switch { PointerPanZoomStartEvent __object24693 => Offset.zero, PointerPanZoomUpdateEvent __object24744 => __object24744.pan, _ => @event.localPosition };
            _velocityTrackers.GetValueOrDefault(@event.pointer)!.addPosition(@event.timeStamp, positionLocal);
        }
        if ((@event is PointerMoveEvent) && (((PointerMoveEvent)@event).buttons != _initialButtons))
        {
            PointerMoveEvent @event__as24923 = (PointerMoveEvent)@event;
            _giveUpPointer(@event__as24923.pointer);
            return;
        }
        if (((@event is PointerMoveEvent) || (@event is PointerPanZoomUpdateEvent)) && _shouldTrackMoveEvent(@event.pointer))
        {
            Offset deltaLocal = (@event is PointerMoveEvent) ? ((PointerMoveEvent)@event).delta : ((PointerPanZoomUpdateEvent?)(object?)((PointerPanZoomUpdateEvent?)(object?)@event)!)!.panDelta;
            Offset localDeltaLocal = (@event is PointerMoveEvent) ? ((PointerMoveEvent)@event).localDelta : ((PointerPanZoomUpdateEvent?)(object?)((PointerPanZoomUpdateEvent?)(object?)@event)!)!.localPanDelta;
            Offset positionAlternate = (@event is PointerMoveEvent) ? ((PointerMoveEvent)@event).position : (@event.position + ((PointerPanZoomUpdateEvent?)(object?)((PointerPanZoomUpdateEvent?)(object?)@event)!)!.pan);
            Offset localPositionLocal = (@event is PointerMoveEvent) ? ((PointerMoveEvent)@event).localPosition : (@event.localPosition + ((PointerPanZoomUpdateEvent?)(object?)((PointerPanZoomUpdateEvent?)(object?)@event)!)!.localPan);
            _lastPosition = new OffsetPair(local: DartRuntimePrimitives.RequireValue(localPositionLocal), global: positionAlternate);
            Offset resolvedDelta = _resolveLocalDeltaForMultitouch(@event.pointer, localDeltaLocal);
            switch (_state)
            {
                case _DragState__monodrag.ready or _DragState__monodrag.possible:
                    {
                        _pendingDragOffset = _pendingDragOffset.op_Add(new OffsetPair(local: localDeltaLocal, global: deltaLocal));
                        _lastPendingEventTimestamp = @event.timeStamp;
                        _lastTransform = @event.transform;
                        Offset movedLocally = _getDeltaForDetails(localDeltaLocal);
                        Matrix4? localToGlobalTransform = (@event.transform is null) ? null : Matrix4.tryInvert(@event.transform!);
                        _globalDistanceMoved += PointerEvent.transformDeltaViaPositions(transform: localToGlobalTransform, untransformedDelta: movedLocally, untransformedEndPosition: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(localPositionLocal))).distance * Math.Sign(_getPrimaryValueFromOffset(movedLocally) ?? 1);
                        if (hasSufficientGlobalDistanceToAccept(@event.kind, gestureSettings?.touchSlop))
                        {
                            _hasDragThresholdBeenMet = true;
                            if (_acceptedActivePointers.Contains(@event.pointer))
                            {
                                _checkDrag(@event.pointer);
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
                        _checkUpdate(sourceTimeStamp: @event.timeStamp, delta: _getDeltaForDetails(resolvedDelta), primaryDelta: _getPrimaryValueFromOffset(resolvedDelta), globalPosition: positionAlternate, localPosition: DartRuntimePrimitives.RequireValue(localPositionLocal), pointer: @event.pointer);
                        break;
                    }
            }
            _recordMoveDeltaForMultitouch(@event.pointer, localDeltaLocal);
        }
        if (@event is PointerUpEvent or PointerCancelEvent or PointerPanZoomEndEvent)
        {
            _giveUpPointer(@event.pointer);
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => !_acceptedActivePointers.Contains(pointer));
        _acceptedActivePointers.Add(pointer);
        _activePointer = pointer;
        if (!onlyAcceptDragOnThreshold || _hasDragThresholdBeenMet)
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
        DartRuntimePrimitives.Assert(() => !Equals(_state, _DragState__monodrag.ready));
        switch (_state)
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
        _velocityTrackers.Clear();
        _initialButtons = null;
        _state = _DragState__monodrag.ready;
    }

    internal virtual void _giveUpPointer(long pointer)
    {
        stopTrackingPointer(pointer);
        if (!_acceptedActivePointers.Remove(pointer))
        {
            resolvePointer(pointer, GestureDisposition.rejected);
        }
        _moveDeltaBeforeFrame.remove(pointer);
        if (_activePointer == pointer)
        {
            _activePointer = (checked((long)_acceptedActivePointers.Count) != 0) ? _acceptedActivePointers.First() : null;
        }
    }

    internal virtual void _checkDown()
    {
        if (onDown is not null)
        {
            var details = new DragDownDetails(globalPosition: _initialPosition.global, localPosition: _initialPosition.local);
            invokeCallback<object?>("onDown", () => { ((Action)(() => onDown!(details)))(); return null; });
        }
    }

    internal virtual void _checkDrag(long pointer)
    {
        if (Equals(_state, _DragState__monodrag.accepted))
        {
            return;
        }
        _state = _DragState__monodrag.accepted;
        OffsetPair deltaLocal = _pendingDragOffset;
        Duration? timestamp = _lastPendingEventTimestamp;
        Matrix4? transformLocal = _lastTransform;
        Offset localUpdateDelta = default!;
        switch (dragStartBehavior)
        {
            case DragStartBehavior.start:
                {
                    _initialPosition = _initialPosition.op_Add(deltaLocal);
                    localUpdateDelta = Offset.zero;
                    break;
                }
            case DragStartBehavior.down:
                {
                    localUpdateDelta = _getDeltaForDetails(deltaLocal.local);
                    break;
                }
        }
        _pendingDragOffset = OffsetPair.zero;
        _lastPendingEventTimestamp = null;
        _lastTransform = null;
        _checkStart(timestamp, pointer);
        if ((!Equals(localUpdateDelta, Offset.zero)) && (onUpdate is not null))
        {
            Matrix4? localToGlobal = (transformLocal is not null) ? Matrix4.tryInvert(transformLocal) : null;
            Offset correctedLocalPosition = _initialPosition.local + localUpdateDelta;
            Offset globalUpdateDelta = PointerEvent.transformDeltaViaPositions(untransformedEndPosition: correctedLocalPosition, untransformedDelta: localUpdateDelta, transform: localToGlobal);
            var updateDelta = new OffsetPair(local: localUpdateDelta, global: globalUpdateDelta);
            OffsetPair correctedPosition = _initialPosition.op_Add(updateDelta);
            _checkUpdate(sourceTimeStamp: timestamp, delta: localUpdateDelta, primaryDelta: _getPrimaryValueFromOffset(localUpdateDelta), globalPosition: correctedPosition.global, localPosition: correctedPosition.local, pointer: pointer);
        }
        resolve(GestureDisposition.accepted);
    }

    internal virtual void _checkStart(Duration? timestamp, long pointer)
    {
        if (onStart is not null)
        {
            var details = new DragStartDetails(sourceTimeStamp: timestamp, globalPosition: _initialPosition.global, localPosition: _initialPosition.local, kind: getKindForPointer(pointer));
            invokeCallback<object?>("onStart", () => { ((Action)(() => onStart!(details)))(); return null; });
        }
    }

    internal virtual void _checkUpdate(Duration? sourceTimeStamp = null, Offset delta = default!, double? primaryDelta = null, Offset globalPosition = default!, Offset? localPosition = null, long pointer = default!)
    {
        if (onUpdate is not null)
        {
            var details = new DragUpdateDetails(sourceTimeStamp: sourceTimeStamp, delta: delta, primaryDelta: primaryDelta, globalPosition: globalPosition, localPosition: localPosition, kind: getKindForPointer(pointer));
            invokeCallback<object?>("onUpdate", () => { ((Action)(() => onUpdate!(details)))(); return null; });
        }
    }

    internal virtual void _checkEnd(long pointer)
    {
        if (onEnd is null)
        {
            return;
        }
        VelocityTracker tracker = _velocityTrackers.GetValueOrDefault(pointer)!;
        VelocityEstimate? estimate = tracker.getVelocityEstimate();
        DragEndDetails? details = default!;
        Func<string> debugReport = default!;
        if (estimate is null)
        {
            debugReport = () => "Could not estimate velocity.";
        }
        else
        {
            details = considerFling(estimate, tracker.kind);
            debugReport = (details is not null) ? (() => $"{estimate}; fling at {details!.velocity}.") : (() => $"{estimate}; judged to not be a fling.");
        }
        details ??= new DragEndDetails(primaryVelocity: 0.0, globalPosition: _lastPosition.global, localPosition: _lastPosition.local);
        invokeCallback<object?>("onEnd", () => { ((Action)(() => onEnd!(details!)))(); return null; }, debugReport);
    }

    internal virtual void _checkCancel()
    {
        if (onCancel is not null)
        {
            invokeCallback<object?>("onCancel", () => { onCancel!(); return null; });
        }
    }

    public override void dispose()
    {
        _velocityTrackers.Clear();
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<DragStartBehavior>("start behavior", dragStartBehavior));
    }

}

public class VerticalDragGestureRecognizer : DragGestureRecognizer
{
    public VerticalDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior)
    {
    }

    public override bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        double minVelocity = minFlingVelocity ?? ConstantsLibrary.kMinFlingVelocity;
        double minDistance = minFlingDistance ?? EventsLibrary.computeHitSlop(kind, gestureSettings);
        return (estimate.pixelsPerSecond.dy.abs() > minVelocity) && (estimate.offset.dy.abs() > minDistance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        double maxVelocity = maxFlingVelocity ?? ConstantsLibrary.kMaxFlingVelocity;
        double dyLocal = Dart_uiLibrary.clampDouble(estimate.pixelsPerSecond.dy, -maxVelocity, maxVelocity);
        return new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new Offset(0, dyLocal)), primaryVelocity: dyLocal, globalPosition: lastPosition.global, localPosition: lastPosition.local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return globalDistanceMoved.abs() > EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => new Offset(0.0, delta.dy);
    internal override double? _getPrimaryValueFromOffset(Offset value) => value.dy;
    internal override _DragDirection__monodrag? _getPrimaryDragAxis() => _DragDirection__monodrag.vertical;
    public override string debugDescription => "vertical drag";
}

public class HorizontalDragGestureRecognizer : DragGestureRecognizer
{
    public HorizontalDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior)
    {
    }

    public override bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        double minVelocity = minFlingVelocity ?? ConstantsLibrary.kMinFlingVelocity;
        double minDistance = minFlingDistance ?? EventsLibrary.computeHitSlop(kind, gestureSettings);
        return (estimate.pixelsPerSecond.dx.abs() > minVelocity) && (estimate.offset.dx.abs() > minDistance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        double maxVelocity = maxFlingVelocity ?? ConstantsLibrary.kMaxFlingVelocity;
        double dxLocal = Dart_uiLibrary.clampDouble(estimate.pixelsPerSecond.dx, -maxVelocity, maxVelocity);
        return new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new Offset(dxLocal, 0)), primaryVelocity: dxLocal, globalPosition: _lastPosition.global, localPosition: _lastPosition.local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return globalDistanceMoved.abs() > EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal override Offset _getDeltaForDetails(Offset delta) => new Offset(delta.dx, 0.0);
    internal override double? _getPrimaryValueFromOffset(Offset value) => value.dx;
    internal override _DragDirection__monodrag? _getPrimaryDragAxis() => _DragDirection__monodrag.horizontal;
    public override string debugDescription => "horizontal drag";
}

public class PanGestureRecognizer : DragGestureRecognizer
{
    public PanGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior)
    {
    }

    public override bool isFlingGesture(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        double minVelocity = minFlingVelocity ?? ConstantsLibrary.kMinFlingVelocity;
        double minDistance = minFlingDistance ?? EventsLibrary.computeHitSlop(kind, gestureSettings);
        return (estimate.pixelsPerSecond.distanceSquared > (minVelocity * minVelocity)) && (estimate.offset.distanceSquared > (minDistance * minDistance));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        Velocity velocityLocal = new Velocity(pixelsPerSecond: estimate.pixelsPerSecond).clampMagnitude(minFlingVelocity ?? ConstantsLibrary.kMinFlingVelocity, maxFlingVelocity ?? ConstantsLibrary.kMaxFlingVelocity);
        return new DragEndDetails(velocity: velocityLocal, globalPosition: lastPosition.global, localPosition: lastPosition.local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return globalDistanceMoved.abs() > EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings);
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


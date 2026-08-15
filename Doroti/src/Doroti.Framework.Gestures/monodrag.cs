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

namespace Doroti.Generated.Framework.Gestures;

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
    internal static bool _defaultButtonAcceptBehavior(long buttons) => (buttons == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton);
    public virtual OffsetPair lastPosition => this._lastPosition;
    public virtual Duration? debugLastPendingEventTimestamp
    {
        get
        {
            Duration? lastPendingEventTimestamp__13296 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    lastPendingEventTimestamp__13296 = this._lastPendingEventTimestamp;
                    return true;
                });
            return lastPendingEventTimestamp__13296;
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
            _initialButtons = global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton;
        }
        _addPointer(@event);
    }

    internal virtual bool _shouldTrackMoveEvent(long pointer)
    {
        bool result__18317 = default!;
        switch (this.multitouchDragStrategy)
        {
            case MultitouchDragStrategy.sumAllPointers:
            case MultitouchDragStrategy.averageBoundaryPointers:
                {
                    result__18317 = true;
                    break;
                }
            case MultitouchDragStrategy.latestPointer:
                {
                    result__18317 = ((this._activePointer is null) || (pointer == this._activePointer));
                    break;
                }
        }
        return result__18317;
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
            global::Doroti.Ui.Offset offset__19155 = DartRuntimePrimitives.RequireValue(this._moveDeltaBeforeFrame.GetValueOrDefault(pointer));
            this._moveDeltaBeforeFrame[pointer] = (offset__19155 + localDelta);
        }
        else
        {
            this._moveDeltaBeforeFrame[pointer] = localDelta;
        }
    }

    internal virtual double _getSumDelta(long pointer, bool positive, _DragDirection__monodrag axis)
    {
        var sum__19459 = 0.0;
        if (!this._moveDeltaBeforeFrame.ContainsKey(pointer))
        {
            return sum__19459;
        }
        global::Doroti.Ui.Offset offset__19568 = DartRuntimePrimitives.RequireValue(this._moveDeltaBeforeFrame.GetValueOrDefault(pointer));
        if (positive)
        {
            if ((object.Equals(axis, _DragDirection__monodrag.vertical)))
            {
                sum__19459 = Math.Max(offset__19568.dy, 0.0);
            }
            else
            {
                sum__19459 = Math.Max(offset__19568.dx, 0.0);
            }
        }
        else
        {
            if ((object.Equals(axis, _DragDirection__monodrag.vertical)))
            {
                sum__19459 = Math.Min(offset__19568.dy, 0.0);
            }
            else
            {
                sum__19459 = Math.Min(offset__19568.dx, 0.0);
            }
        }
        return sum__19459;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _getMaxSumDeltaPointer(bool positive, _DragDirection__monodrag axis)
    {
        if ((checked((long)(this._moveDeltaBeforeFrame.Count)) == 0))
        {
            return null;
        }
        long? ret__20111 = default!;
        double? max__20128 = default!;
        double sum__20144 = default!;
        foreach (long pointer__20168 in this._moveDeltaBeforeFrame.Keys)
        {
            sum__20144 = _getSumDelta(pointer: pointer__20168, positive: positive, axis: axis);
            if ((ret__20111 is null))
            {
                ret__20111 = pointer__20168;
                max__20128 = sum__20144;
            }
            else
            {
                if (positive)
                {
                    if ((sum__20144 > DartRuntimePrimitives.RequireValue(max__20128)))
                    {
                        ret__20111 = pointer__20168;
                        max__20128 = sum__20144;
                    }
                }
                else
                {
                    if ((sum__20144 < DartRuntimePrimitives.RequireValue(max__20128)))
                    {
                        ret__20111 = pointer__20168;
                        max__20128 = sum__20144;
                    }
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (ret__20111 is not null));
        return ret__20111;
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
        Duration currentSystemFrameTimeStamp__21031 = SchedulerBinding.instance.currentSystemFrameTimeStamp;
        if ((!object.Equals(this._frameTimeStamp, currentSystemFrameTimeStamp__21031)))
        {
            this._moveDeltaBeforeFrame.Clear();
            _lastUpdatedDeltaForPan = Offset.zero;
            _frameTimeStamp = currentSystemFrameTimeStamp__21031;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._frameTimeStamp, SchedulerBinding.instance.currentSystemFrameTimeStamp)));
        _DragDirection__monodrag? axis__21437 = _getPrimaryDragAxis();
        if ((((!object.Equals(this._state, _DragState__monodrag.accepted)) || (object.Equals(localDelta, Offset.zero))) || (((checked((long)(this._moveDeltaBeforeFrame.Count)) == 0) && (axis__21437 is not null)))))
        {
            return localDelta;
        }
        double dx__21654 = default!;
        double dy__21658 = default!;
        if ((object.Equals(axis__21437, _DragDirection__monodrag.horizontal)))
        {
            dx__21654 = _resolveDelta(pointer: pointer, axis: _DragDirection__monodrag.horizontal, localDelta: localDelta);
            DartRuntimePrimitives.Assert(() => (dx__21654.abs() <= localDelta.dx.abs()));
            dy__21658 = 0.0;
        }
        else
        {
            if ((object.Equals(axis__21437, _DragDirection__monodrag.vertical)))
            {
                dx__21654 = 0.0;
                dy__21658 = _resolveDelta(pointer: pointer, axis: _DragDirection__monodrag.vertical, localDelta: localDelta);
                DartRuntimePrimitives.Assert(() => (dy__21658.abs() <= localDelta.dy.abs()));
            }
            else
            {
                double averageX__22115 = _resolveDeltaForPanGesture(axis: _DragDirection__monodrag.horizontal, localDelta: localDelta);
                double averageY__22255 = _resolveDeltaForPanGesture(axis: _DragDirection__monodrag.vertical, localDelta: localDelta);
                global::Doroti.Ui.Offset updatedDelta__22393 = (new global::Doroti.Ui.Offset(averageX__22115, averageY__22255) - this._lastUpdatedDeltaForPan);
                _lastUpdatedDeltaForPan = new global::Doroti.Ui.Offset(averageX__22115, averageY__22255);
                dx__21654 = updatedDelta__22393.dx;
                dy__21658 = updatedDelta__22393.dy;
            }
        }
        return new global::Doroti.Ui.Offset(dx__21654, dy__21658);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _resolveDelta(long pointer, _DragDirection__monodrag axis, Offset localDelta)
    {
        bool positive__22756 = ((object.Equals(axis, _DragDirection__monodrag.horizontal)) ? (localDelta.dx > 0L) : (localDelta.dy > 0L));
        double delta__22859 = ((object.Equals(axis, _DragDirection__monodrag.horizontal)) ? localDelta.dx : localDelta.dy);
        long? maxSumDeltaPointer__22949 = _getMaxSumDeltaPointer(positive: positive__22756, axis: axis);
        DartRuntimePrimitives.Assert(() => (maxSumDeltaPointer__22949 is not null));
        if ((maxSumDeltaPointer__22949 == pointer))
        {
            return delta__22859;
        }
        else
        {
            double maxSumDelta__23160 = _getSumDelta(pointer: DartRuntimePrimitives.RequireValue(maxSumDeltaPointer__22949), positive: positive__22756, axis: axis);
            double curPointerSumDelta__23302 = _getSumDelta(pointer: pointer, positive: positive__22756, axis: axis);
            if (positive__22756)
            {
                if (((curPointerSumDelta__23302 + delta__22859) > maxSumDelta__23160))
                {
                    return ((curPointerSumDelta__23302 + delta__22859) - maxSumDelta__23160);
                }
                else
                {
                    return 0.0;
                }
            }
            else
            {
                if (((curPointerSumDelta__23302 + delta__22859) < maxSumDelta__23160))
                {
                    return ((curPointerSumDelta__23302 + delta__22859) - maxSumDelta__23160);
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
        double delta__23919 = ((object.Equals(axis, _DragDirection__monodrag.horizontal)) ? localDelta.dx : localDelta.dy);
        long pointerCount__24008 = checked((long)(this._acceptedActivePointers.Count));
        DartRuntimePrimitives.Assert(() => (pointerCount__24008 >= 1L));
        var sum__24095 = delta__23919;
        foreach (global::Doroti.Ui.Offset offset__24130 in this._moveDeltaBeforeFrame.Values)
        {
            if ((object.Equals(axis, _DragDirection__monodrag.horizontal)))
            {
                sum__24095 += offset__24130.dx;
            }
            else
            {
                sum__24095 += offset__24130.dy;
            }
        }
        return (sum__24095 / pointerCount__24008);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._state, _DragState__monodrag.ready)));
        if ((!((PointerEvent)@event).synthesized && (((((@event is PointerDownEvent) || (@event is PointerMoveEvent)) || (@event is PointerPanZoomStartEvent)) || (@event is PointerPanZoomUpdateEvent)))))
        {
            global::Doroti.Ui.Offset position__24657 = (@event switch { PointerPanZoomStartEvent __object24693 => Offset.zero, PointerPanZoomUpdateEvent __object24744 => ((PointerPanZoomUpdateEvent)((PointerPanZoomUpdateEvent)__object24744)).pan, _ => ((PointerEvent)@event).localPosition });
            this._velocityTrackers.GetValueOrDefault(((PointerEvent)@event).pointer)!.addPosition(((PointerEvent)@event).timeStamp, position__24657);
        }
        if (((@event is PointerMoveEvent) && (((PointerMoveEvent)@event).buttons != this._initialButtons)))
        {
            PointerMoveEvent @event__as24923 = (PointerMoveEvent)@event;
            _giveUpPointer(((PointerMoveEvent)@event__as24923).pointer);
            return;
        }
        if (((((@event is PointerMoveEvent) || (@event is PointerPanZoomUpdateEvent))) && _shouldTrackMoveEvent(((PointerEvent)@event).pointer)))
        {
            global::Doroti.Ui.Offset delta__25189 = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).delta : (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).panDelta);
            global::Doroti.Ui.Offset localDelta__25327 = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).localDelta : (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).localPanDelta);
            global::Doroti.Ui.Offset position__25480 = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).position : ((((PointerEvent)@event).position + (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).pan)));
            global::Doroti.Ui.Offset localPosition__25638 = (((@event is PointerMoveEvent)) ? ((PointerMoveEvent)@event).localPosition : ((((PointerEvent)@event).localPosition + (((PointerPanZoomUpdateEvent?)(object?)(((PointerPanZoomUpdateEvent?)(object?)@event)!))!).localPan)));
            _lastPosition = new OffsetPair(local: DartRuntimePrimitives.RequireValue(localPosition__25638), global: position__25480);
            global::Doroti.Ui.Offset resolvedDelta__25890 = _resolveLocalDeltaForMultitouch(((PointerEvent)@event).pointer, localDelta__25327);
            switch (this._state)
            {
                case _DragState__monodrag.ready or _DragState__monodrag.possible:
                    {
                        _pendingDragOffset = _pendingDragOffset.op_Add(new OffsetPair(local: localDelta__25327, global: delta__25189));
                        _lastPendingEventTimestamp = ((PointerEvent)@event).timeStamp;
                        _lastTransform = ((PointerEvent)@event).transform;
                        global::Doroti.Ui.Offset movedLocally__26245 = _getDeltaForDetails(localDelta__25327);
                        Matrix4? localToGlobalTransform__26318 = ((((PointerEvent)@event).transform is null) ? null : Matrix4.tryInvert(((PointerEvent)@event).transform!));
                        _globalDistanceMoved += (PointerEvent.transformDeltaViaPositions(transform: localToGlobalTransform__26318, untransformedDelta: movedLocally__26245, untransformedEndPosition: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(localPosition__25638))).distance * Math.Sign(((_getPrimaryValueFromOffset(movedLocally__26245) ?? 1))));
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
                        _checkUpdate(sourceTimeStamp: ((PointerEvent)@event).timeStamp, delta: _getDeltaForDetails(resolvedDelta__25890), primaryDelta: _getPrimaryValueFromOffset(resolvedDelta__25890), globalPosition: position__25480, localPosition: DartRuntimePrimitives.RequireValue(localPosition__25638), pointer: ((PointerEvent)@event).pointer);
                        break;
                    }
            }
            _recordMoveDeltaForMultitouch(((PointerEvent)@event).pointer, localDelta__25327);
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
            var details__29640 = new DragDownDetails(globalPosition: ((OffsetPair)this._initialPosition).global, localPosition: ((OffsetPair)this._initialPosition).local);
            invokeCallback<object?>("onDown", () => { ((Action)((() => this.onDown!(details__29640))))(); return null; });
        }
    }

    internal virtual void _checkDrag(long pointer)
    {
        if ((object.Equals(this._state, _DragState__monodrag.accepted)))
        {
            return;
        }
        _state = _DragState__monodrag.accepted;
        OffsetPair delta__29994 = this._pendingDragOffset;
        Duration? timestamp__30042 = this._lastPendingEventTimestamp;
        Matrix4? transform__30101 = this._lastTransform;
        global::Doroti.Ui.Offset localUpdateDelta__30146 = default!;
        switch (this.dragStartBehavior)
        {
            case DragStartBehavior.start:
                {
                    _initialPosition = (this._initialPosition.op_Add(delta__29994));
                    localUpdateDelta__30146 = Offset.zero;
                    break;
                }
            case DragStartBehavior.down:
                {
                    localUpdateDelta__30146 = _getDeltaForDetails(((OffsetPair)delta__29994).local);
                    break;
                }
        }
        _pendingDragOffset = OffsetPair.zero;
        _lastPendingEventTimestamp = null;
        _lastTransform = null;
        _checkStart(timestamp__30042, pointer);
        if (((!object.Equals(localUpdateDelta__30146, Offset.zero)) && (this.onUpdate is not null)))
        {
            Matrix4? localToGlobal__30657 = ((transform__30101 is not null) ? Matrix4.tryInvert(transform__30101) : null);
            global::Doroti.Ui.Offset correctedLocalPosition__30749 = (((OffsetPair)this._initialPosition).local + localUpdateDelta__30146);
            global::Doroti.Ui.Offset globalUpdateDelta__30836 = PointerEvent.transformDeltaViaPositions(untransformedEndPosition: correctedLocalPosition__30749, untransformedDelta: localUpdateDelta__30146, transform: localToGlobal__30657);
            var updateDelta__31056 = new OffsetPair(local: localUpdateDelta__30146, global: globalUpdateDelta__30836);
            OffsetPair correctedPosition__31157 = (this._initialPosition.op_Add(updateDelta__31056));
            _checkUpdate(sourceTimeStamp: timestamp__30042, delta: localUpdateDelta__30146, primaryDelta: _getPrimaryValueFromOffset(localUpdateDelta__30146), globalPosition: ((OffsetPair)correctedPosition__31157).global, localPosition: ((OffsetPair)correctedPosition__31157).local, pointer: pointer);
        }
        resolve(GestureDisposition.accepted);
    }

    internal virtual void _checkStart(Duration? timestamp, long pointer)
    {
        if ((this.onStart is not null))
        {
            var details__31929 = new DragStartDetails(sourceTimeStamp: timestamp, globalPosition: ((OffsetPair)this._initialPosition).global, localPosition: ((OffsetPair)this._initialPosition).local, kind: getKindForPointer(pointer));
            invokeCallback<object?>("onStart", () => { ((Action)((() => this.onStart!(details__31929))))(); return null; });
        }
    }

    internal virtual void _checkUpdate(Duration? sourceTimeStamp = null, Offset delta = default!, double? primaryDelta = null, Offset globalPosition = default!, Offset? localPosition = null, long pointer = default!)
    {
        if ((this.onUpdate is not null))
        {
            var details__32457 = new DragUpdateDetails(sourceTimeStamp: sourceTimeStamp, delta: delta, primaryDelta: primaryDelta, globalPosition: globalPosition, localPosition: localPosition, kind: getKindForPointer(pointer));
            invokeCallback<object?>("onUpdate", () => { ((Action)((() => this.onUpdate!(details__32457))))(); return null; });
        }
    }

    internal virtual void _checkEnd(long pointer)
    {
        if ((this.onEnd is null))
        {
            return;
        }
        VelocityTracker tracker__32896 = this._velocityTrackers.GetValueOrDefault(pointer)!;
        VelocityEstimate? estimate__32963 = tracker__32896.getVelocityEstimate();
        DragEndDetails? details__33026 = default!;
        Func<string> debugReport__33063 = default!;
        if ((estimate__32963 is null))
        {
            debugReport__33063 = (() => "Could not estimate velocity.");
        }
        else
        {
            details__33026 = considerFling(estimate__32963, ((VelocityTracker)tracker__32896).kind);
            debugReport__33063 = (((details__33026 is not null)) ? (() => $"{estimate__32963}; fling at {details__33026!.velocity}.") : (() => $"{estimate__32963}; judged to not be a fling."));
        }
        details__33026 ??= new DragEndDetails(primaryVelocity: 0.0, globalPosition: ((OffsetPair)this._lastPosition).global, localPosition: ((OffsetPair)this._lastPosition).local);
        invokeCallback<object?>("onEnd", () => { ((Action)((() => this.onEnd!(details__33026!))))(); return null; }, debugReport__33063);
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
        double minVelocity__34838 = (minFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity);
        double minDistance__34908 = (minFlingDistance ?? global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings));
        return ((((VelocityEstimate)estimate).pixelsPerSecond.dy.abs() > minVelocity__34838) && (((VelocityEstimate)estimate).offset.dy.abs() > minDistance__34908));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        double maxVelocity__35278 = (maxFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity);
        double dy__35348 = Dart_uiLibrary.clampDouble(((VelocityEstimate)estimate).pixelsPerSecond.dy, -maxVelocity__35278, maxVelocity__35278);
        return new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new global::Doroti.Ui.Offset(0, dy__35348)), primaryVelocity: dy__35348, globalPosition: ((OffsetPair)lastPosition).global, localPosition: ((OffsetPair)lastPosition).local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return (globalDistanceMoved.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings));
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
        double minVelocity__36945 = (minFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity);
        double minDistance__37015 = (minFlingDistance ?? global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings));
        return ((((VelocityEstimate)estimate).pixelsPerSecond.dx.abs() > minVelocity__36945) && (((VelocityEstimate)estimate).offset.dx.abs() > minDistance__37015));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        double maxVelocity__37385 = (maxFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity);
        double dx__37455 = Dart_uiLibrary.clampDouble(((VelocityEstimate)estimate).pixelsPerSecond.dx, -maxVelocity__37385, maxVelocity__37385);
        return new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new global::Doroti.Ui.Offset(dx__37455, 0)), primaryVelocity: dx__37455, globalPosition: ((OffsetPair)_lastPosition).global, localPosition: ((OffsetPair)_lastPosition).local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return (globalDistanceMoved.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(pointerDeviceKind, gestureSettings));
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
        double minVelocity__38983 = (minFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity);
        double minDistance__39053 = (minFlingDistance ?? global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings));
        return ((((VelocityEstimate)estimate).pixelsPerSecond.distanceSquared > (minVelocity__38983 * minVelocity__38983)) && (((VelocityEstimate)estimate).offset.distanceSquared > (minDistance__39053 * minDistance__39053)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DragEndDetails? considerFling(VelocityEstimate estimate, PointerDeviceKind kind)
    {
        if (!isFlingGesture(estimate, kind))
        {
            return null;
        }
        Velocity velocity__39467 = new Velocity(pixelsPerSecond: ((VelocityEstimate)estimate).pixelsPerSecond).clampMagnitude((minFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity), (maxFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity));
        return new DragEndDetails(velocity: velocity__39467, globalPosition: ((OffsetPair)lastPosition).global, localPosition: ((OffsetPair)lastPosition).local);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hasSufficientGlobalDistanceToAccept(PointerDeviceKind pointerDeviceKind, double? deviceTouchSlop)
    {
        return (globalDistanceMoved.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computePanSlop(pointerDeviceKind, gestureSettings));
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


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/multidrag.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public delegate Drag? GestureMultiDragStartCallback(Offset position);

public abstract class MultiDragPointerState
{
    public virtual DeviceGestureSettings? gestureSettings { get; private set; }
    public virtual Offset initialPosition { get; private set; } = default!;
    internal virtual VelocityTracker _velocityTracker { get; private set; } = default!;
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    internal virtual Drag? _client { get; set; } = default;
    internal virtual Offset? _pendingDelta { get; set; } = Offset.zero;
    internal virtual Duration? _lastPendingEventTimestamp { get; set; } = default;
    internal virtual GestureArenaEntry? _arenaEntry { get; set; } = default;

    protected MultiDragPointerState(Offset initialPosition, PointerDeviceKind kind, DeviceGestureSettings? gestureSettings)
    {
        this.initialPosition = initialPosition;
        this.kind = kind;
        this.gestureSettings = gestureSettings;
        _velocityTracker = new VelocityTracker(kind);
    }

    public virtual global::Doroti.Ui.Offset? pendingDelta => _pendingDelta;
    internal virtual void _setArenaEntry(GestureArenaEntry entry)
    {
        DartRuntimePrimitives.Assert(() => _arenaEntry is null);
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        DartRuntimePrimitives.Assert(() => _client is null);
        _arenaEntry = entry;
    }

    public virtual void resolve(GestureDisposition disposition)
    {
        _arenaEntry!.resolve(disposition);
    }

    internal virtual void _move(PointerMoveEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _arenaEntry is not null);
        if (!@event.synthesized)
        {
            _velocityTracker.addPosition(@event.timeStamp, @event.position);
        }
        if (_client is not null)
        {
            DartRuntimePrimitives.Assert(() => pendingDelta is null);
            _client!.update(new DragUpdateDetails(sourceTimeStamp: @event.timeStamp, delta: @event.delta, globalPosition: @event.position));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => pendingDelta is not null);
            _pendingDelta = DartRuntimePrimitives.RequireValue(_pendingDelta) + @event.delta;
            _lastPendingEventTimestamp = @event.timeStamp;
            checkForResolutionAfterMove();
        }
    }

    public virtual void checkForResolutionAfterMove()
    {
    }

    public abstract void accepted(Func<Offset, Drag?> starter);
    public virtual void rejected()
    {
        DartRuntimePrimitives.Assert(() => _arenaEntry is not null);
        DartRuntimePrimitives.Assert(() => _client is null);
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        _pendingDelta = null;
        _lastPendingEventTimestamp = null;
        _arenaEntry = null;
    }

    internal virtual void _startDrag(Drag client)
    {
        DartRuntimePrimitives.Assert(() => _arenaEntry is not null);
        DartRuntimePrimitives.Assert(() => _client is null);
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        _client = client;
        var details = new DragUpdateDetails(sourceTimeStamp: _lastPendingEventTimestamp, delta: DartRuntimePrimitives.RequireValue(pendingDelta), globalPosition: initialPosition);
        _pendingDelta = null;
        _lastPendingEventTimestamp = null;
        _client!.update(details);
    }

    internal virtual void _up()
    {
        DartRuntimePrimitives.Assert(() => _arenaEntry is not null);
        if (_client is not null)
        {
            DartRuntimePrimitives.Assert(() => pendingDelta is null);
            var details = new DragEndDetails(velocity: _velocityTracker.getVelocity());
            Drag client = _client!;
            _client = null;
            client.end(details);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => pendingDelta is not null);
            _pendingDelta = null;
            _lastPendingEventTimestamp = null;
        }
    }

    internal virtual void _cancel()
    {
        DartRuntimePrimitives.Assert(() => _arenaEntry is not null);
        if (_client is not null)
        {
            DartRuntimePrimitives.Assert(() => pendingDelta is null);
            Drag client = _client!;
            _client = null;
            client.cancel();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => pendingDelta is not null);
            _pendingDelta = null;
            _lastPendingEventTimestamp = null;
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _arenaEntry?.resolve(GestureDisposition.rejected);
        _arenaEntry = null;
        DartRuntimePrimitives.Assert(() =>
            {
                _pendingDelta = null;
                return true;
            });
    }

}

public abstract class MultiDragGestureRecognizer : GestureRecognizer
{
    public virtual Func<Offset, Drag?>? onStart { get; set; } = default;
    internal virtual DartMap<long, MultiDragPointerState>? _pointers { get; set; } = new DartMap<long, MultiDragPointerState>();

    protected MultiDragGestureRecognizer(object? debugOwner, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool>? allowedButtonsFilter = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior)
    {
    }

    internal new static bool _defaultButtonAcceptBehavior(long buttons) => buttons == EventsLibrary.kPrimaryButton;
    public override void addAllowedPointer(PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _pointers is not null);
        DartRuntimePrimitives.Assert(() => !_pointers!.ContainsKey(@event.pointer));
        MultiDragPointerState state = createNewPointerState(@event);
        _pointers![@event.pointer] = state;
        GestureBinding.instance.pointerRouter.addRoute(@event.pointer, _handleEvent);
        state._setArenaEntry(GestureBinding.instance.gestureArena.add(@event.pointer, this));
    }

    public abstract MultiDragPointerState createNewPointerState(PointerDownEvent @event);
    internal virtual void _handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _pointers is not null);
        DartRuntimePrimitives.Assert(() => _pointers!.ContainsKey(@event.pointer));
        MultiDragPointerState state = _pointers!.GetValueOrDefault(@event.pointer)!;
        if (@event is PointerMoveEvent)
        {
            PointerMoveEvent @event__as8770 = (PointerMoveEvent)@event;
            state._move(@event__as8770);
        }
        else
        {
            if (@event is PointerUpEvent)
            {
                PointerUpEvent @event__as8876 = (PointerUpEvent)@event;
                DartRuntimePrimitives.Assert(() => Equals(@event__as8876.delta, Offset.zero));
                state._up();
                _removeState(@event__as8876.pointer);
            }
            else
            {
                if (@event is PointerCancelEvent)
                {
                    PointerCancelEvent @event__as9050 = (PointerCancelEvent)@event;
                    DartRuntimePrimitives.Assert(() => Equals(@event__as9050.delta, Offset.zero));
                    state._cancel();
                    _removeState(@event__as9050.pointer);
                }
                else
                {
                    if (@event is not PointerDownEvent)
                    {
                        DartRuntimePrimitives.Assert(() => false);
                    }
                }
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => _pointers is not null);
        MultiDragPointerState? state = _pointers!.GetValueOrDefault(pointer);
        if (state is null)
        {
            return;
        }
        state.accepted((initialPosition) => _startDrag(initialPosition, pointer));
    }

    internal virtual Drag? _startDrag(Offset initialPosition, long pointer)
    {
        DartRuntimePrimitives.Assert(() => _pointers is not null);
        MultiDragPointerState state = _pointers!.GetValueOrDefault(pointer)!;
        DartRuntimePrimitives.Assert(() => state._pendingDelta is not null);
        Drag? drag = default!;
        if (onStart is not null)
        {
            drag = invokeCallback<Drag?>("onStart", () => onStart!(initialPosition));
        }
        if (drag is not null)
        {
            state._startDrag(drag);
        }
        else
        {
            _removeState(pointer);
        }
        return drag;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void rejectGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => _pointers is not null);
        if (_pointers!.ContainsKey(pointer))
        {
            MultiDragPointerState state = _pointers!.GetValueOrDefault(pointer)!;
            state.rejected();
            _removeState(pointer);
        }
    }

    internal virtual void _removeState(long pointer)
    {
        if (_pointers is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _pointers!.ContainsKey(pointer));
        GestureBinding.instance.pointerRouter.removeRoute(pointer, _handleEvent);
        _pointers!.remove(pointer)!.dispose();
    }

    public override void dispose()
    {
        _pointers!.Keys.ToList().forEach(_removeState);
        DartRuntimePrimitives.Assert(() => checked((long)_pointers!.Count) == 0);
        _pointers = null;
        base.dispose();
    }

}

internal class _ImmediatePointerState__multidrag : MultiDragPointerState
{
    internal _ImmediatePointerState__multidrag(Offset initialPosition, PointerDeviceKind kind, DeviceGestureSettings? gestureSettings) : base(initialPosition, kind, gestureSettings)
    {
    }

    public override void checkForResolutionAfterMove()
    {
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        if (DartRuntimePrimitives.RequireValue(pendingDelta).distance > EventsLibrary.computeHitSlop(kind, gestureSettings))
        {
            resolve(GestureDisposition.accepted);
        }
    }

    public override void accepted(Func<Offset, Drag?> starter)
    {
        starter(initialPosition);
    }

}

public class ImmediateMultiDragGestureRecognizer : MultiDragGestureRecognizer
{
    public ImmediateMultiDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool>? allowedButtonsFilter = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter)
    {
    }

    public override MultiDragPointerState createNewPointerState(PointerDownEvent @event)
    {
        return new _ImmediatePointerState__multidrag(@event.position, @event.kind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "multidrag";
}

internal class _HorizontalPointerState__multidrag : MultiDragPointerState
{
    internal _HorizontalPointerState__multidrag(Offset initialPosition, PointerDeviceKind kind, DeviceGestureSettings? gestureSettings) : base(initialPosition, kind, gestureSettings)
    {
    }

    public override void checkForResolutionAfterMove()
    {
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        if (DartRuntimePrimitives.RequireValue(pendingDelta).dx.abs() > EventsLibrary.computeHitSlop(kind, gestureSettings))
        {
            resolve(GestureDisposition.accepted);
        }
    }

    public override void accepted(Func<Offset, Drag?> starter)
    {
        starter(initialPosition);
    }

}

public class HorizontalMultiDragGestureRecognizer : MultiDragGestureRecognizer
{
    public HorizontalMultiDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool>? allowedButtonsFilter = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter)
    {
    }

    public override MultiDragPointerState createNewPointerState(PointerDownEvent @event)
    {
        return new _HorizontalPointerState__multidrag(@event.position, @event.kind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "horizontal multidrag";
}

internal class _VerticalPointerState__multidrag : MultiDragPointerState
{
    internal _VerticalPointerState__multidrag(Offset initialPosition, PointerDeviceKind kind, DeviceGestureSettings? gestureSettings) : base(initialPosition, kind, gestureSettings)
    {
    }

    public override void checkForResolutionAfterMove()
    {
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        if (DartRuntimePrimitives.RequireValue(pendingDelta).dy.abs() > EventsLibrary.computeHitSlop(kind, gestureSettings))
        {
            resolve(GestureDisposition.accepted);
        }
    }

    public override void accepted(Func<Offset, Drag?> starter)
    {
        starter(initialPosition);
    }

}

public class VerticalMultiDragGestureRecognizer : MultiDragGestureRecognizer
{
    public VerticalMultiDragGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool>? allowedButtonsFilter = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter)
    {
    }

    public override MultiDragPointerState createNewPointerState(PointerDownEvent @event)
    {
        return new _VerticalPointerState__multidrag(@event.position, @event.kind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "vertical multidrag";
}

internal class _DelayedPointerState__multidrag : MultiDragPointerState
{
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual Func<Offset, Drag?>? _starter { get; set; } = default;

    internal _DelayedPointerState__multidrag(Offset initialPosition, Duration delay, PointerDeviceKind kind, DeviceGestureSettings? gestureSettings) : base(initialPosition, kind, gestureSettings)
    {
    }

    internal virtual void _delayPassed()
    {
        DartRuntimePrimitives.Assert(() => _timer is not null);
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(pendingDelta).distance <= EventsLibrary.computeHitSlop(kind, gestureSettings));
        _timer = null;
        if (_starter is not null)
        {
            _starter!(initialPosition);
            _starter = null;
        }
        else
        {
            resolve(GestureDisposition.accepted);
        }
        DartRuntimePrimitives.Assert(() => _starter is null);
    }

    internal virtual void _ensureTimerStopped()
    {
        _timer?.cancel();
        _timer = null;
    }

    public override void accepted(Func<Offset, Drag?> starter)
    {
        DartRuntimePrimitives.Assert(() => _starter is null);
        if (_timer is null)
        {
            starter(initialPosition);
        }
        else
        {
            _starter = starter;
        }
    }

    public override void checkForResolutionAfterMove()
    {
        if (_timer is null)
        {
            DartRuntimePrimitives.Assert(() => _starter is not null);
            return;
        }
        DartRuntimePrimitives.Assert(() => pendingDelta is not null);
        if (DartRuntimePrimitives.RequireValue(pendingDelta).distance > EventsLibrary.computeHitSlop(kind, gestureSettings))
        {
            resolve(GestureDisposition.rejected);
            _ensureTimerStopped();
        }
    }

    public override void dispose()
    {
        _ensureTimerStopped();
        base.dispose();
    }

}

public class DelayedMultiDragGestureRecognizer : MultiDragGestureRecognizer
{
    public virtual Duration delay { get; private set; } = default!;

    public DelayedMultiDragGestureRecognizer(Duration? delay = null, object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool>? allowedButtonsFilter = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter)
    {
        Duration __delay = delay ?? ConstantsLibrary.kLongPressTimeout;
        this.delay = __delay;
    }

    public override MultiDragPointerState createNewPointerState(PointerDownEvent @event)
    {
        return new _DelayedPointerState__multidrag(@event.position, DartRuntimePrimitives.RequireValue(delay), @event.kind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "long multidrag";
}


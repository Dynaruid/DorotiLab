// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/multidrag.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Gestures;

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
        this._velocityTracker = new VelocityTracker(kind);
    }

    public virtual global::Doroti.Flutter.Ui.Offset? pendingDelta => this._pendingDelta;
    internal virtual void _setArenaEntry(GestureArenaEntry entry)
    {
        DartRuntimePrimitives.Assert(() => (this._arenaEntry is null));
        DartRuntimePrimitives.Assert(() => (this.pendingDelta is not null));
        DartRuntimePrimitives.Assert(() => (this._client is null));
        _arenaEntry = entry;
    }

    public virtual void resolve(GestureDisposition disposition)
    {
        this._arenaEntry!.resolve(disposition);
    }

    internal virtual void _move(PointerMoveEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._arenaEntry is not null));
        if (!@event.synthesized)
        {
            this._velocityTracker.addPosition(@event.timeStamp, @event.position);
        }
        if ((this._client is not null))
        {
            DartRuntimePrimitives.Assert(() => (this.pendingDelta is null));
            this._client!.update(new DragUpdateDetails(sourceTimeStamp: @event.timeStamp, delta: @event.delta, globalPosition: @event.position));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this.pendingDelta is not null));
            _pendingDelta = (DartRuntimePrimitives.RequireValue(this._pendingDelta) + @event.delta);
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
        DartRuntimePrimitives.Assert(() => (this._arenaEntry is not null));
        DartRuntimePrimitives.Assert(() => (this._client is null));
        DartRuntimePrimitives.Assert(() => (this.pendingDelta is not null));
        _pendingDelta = null;
        _lastPendingEventTimestamp = null;
        _arenaEntry = null;
    }

    internal virtual void _startDrag(Drag client)
    {
        DartRuntimePrimitives.Assert(() => (this._arenaEntry is not null));
        DartRuntimePrimitives.Assert(() => (this._client is null));
        DartRuntimePrimitives.Assert(() => (this.pendingDelta is not null));
        _client = client;
        var details__4668 = new DragUpdateDetails(sourceTimeStamp: this._lastPendingEventTimestamp, delta: DartRuntimePrimitives.RequireValue(this.pendingDelta), globalPosition: this.initialPosition);
        _pendingDelta = null;
        _lastPendingEventTimestamp = null;
        this._client!.update(details__4668);
    }

    internal virtual void _up()
    {
        DartRuntimePrimitives.Assert(() => (this._arenaEntry is not null));
        if ((this._client is not null))
        {
            DartRuntimePrimitives.Assert(() => (this.pendingDelta is null));
            var details__5090 = new DragEndDetails(velocity: this._velocityTracker.getVelocity());
            Drag client__5175 = this._client!;
            _client = null;
            client__5175.end(details__5090);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this.pendingDelta is not null));
            _pendingDelta = null;
            _lastPendingEventTimestamp = null;
        }
    }

    internal virtual void _cancel()
    {
        DartRuntimePrimitives.Assert(() => (this._arenaEntry is not null));
        if ((this._client is not null))
        {
            DartRuntimePrimitives.Assert(() => (this.pendingDelta is null));
            Drag client__5551 = this._client!;
            _client = null;
            client__5551.cancel();
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this.pendingDelta is not null));
            _pendingDelta = null;
            _lastPendingEventTimestamp = null;
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._arenaEntry?.resolve(GestureDisposition.rejected);
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

    protected MultiDragGestureRecognizer(object? debugOwner, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool>? allowedButtonsFilter = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: (allowedButtonsFilter ?? _defaultButtonAcceptBehavior))
    {
    }

    internal static bool _defaultButtonAcceptBehavior(long buttons) => (buttons == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton);
    public override void addAllowedPointer(PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._pointers is not null));
        DartRuntimePrimitives.Assert(() => !this._pointers!.ContainsKey(@event.pointer));
        MultiDragPointerState state__8081 = createNewPointerState(@event);
        this._pointers![@event.pointer] = state__8081;
        GestureBinding.instance.pointerRouter.addRoute(@event.pointer, (Action<PointerEvent>)this._handleEvent);
        state__8081._setArenaEntry(GestureBinding.instance.gestureArena.add(@event.pointer, this));
    }

    public abstract MultiDragPointerState createNewPointerState(PointerDownEvent @event);
    internal virtual void _handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._pointers is not null));
        DartRuntimePrimitives.Assert(() => this._pointers!.ContainsKey(((PointerEvent)@event).pointer));
        MultiDragPointerState state__8726 = this._pointers!.GetValueOrDefault(((PointerEvent)@event).pointer)!;
        if ((@event is PointerMoveEvent))
        {
            PointerMoveEvent @event__as8770 = (PointerMoveEvent)@event;
            state__8726._move(((PointerMoveEvent)@event__as8770));
        }
        else
        {
            if ((@event is PointerUpEvent))
            {
                PointerUpEvent @event__as8876 = (PointerUpEvent)@event;
                DartRuntimePrimitives.Assert(() => (object.Equals(((PointerUpEvent)@event__as8876).delta, Offset.zero)));
                state__8726._up();
                _removeState(((PointerUpEvent)@event__as8876).pointer);
            }
            else
            {
                if ((@event is PointerCancelEvent))
                {
                    PointerCancelEvent @event__as9050 = (PointerCancelEvent)@event;
                    DartRuntimePrimitives.Assert(() => (object.Equals(((PointerCancelEvent)@event__as9050).delta, Offset.zero)));
                    state__8726._cancel();
                    _removeState(((PointerCancelEvent)@event__as9050).pointer);
                }
                else
                {
                    if ((@event is not PointerDownEvent))
                    {
                        DartRuntimePrimitives.Assert(() => false);
                    }
                }
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (this._pointers is not null));
        MultiDragPointerState? state__9610 = this._pointers!.GetValueOrDefault(pointer);
        if ((state__9610 is null))
        {
            return;
        }
        state__9610.accepted(((Func<Offset, Drag?>)((initialPosition) => _startDrag(initialPosition, pointer))));
    }

    internal virtual Drag? _startDrag(Offset initialPosition, long pointer)
    {
        DartRuntimePrimitives.Assert(() => (this._pointers is not null));
        MultiDragPointerState state__9975 = this._pointers!.GetValueOrDefault(pointer)!;
        DartRuntimePrimitives.Assert(() => (((MultiDragPointerState)state__9975)._pendingDelta is not null));
        Drag? drag__10056 = default!;
        if ((this.onStart is not null))
        {
            drag__10056 = invokeCallback<Drag?>("onStart", ((Func<Drag?>)(() => this.onStart!(initialPosition))));
        }
        if ((drag__10056 is not null))
        {
            state__9975._startDrag(drag__10056);
        }
        else
        {
            _removeState(pointer);
        }
        return drag__10056;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void rejectGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (this._pointers is not null));
        if (this._pointers!.ContainsKey(pointer))
        {
            MultiDragPointerState state__10455 = this._pointers!.GetValueOrDefault(pointer)!;
            state__10455.rejected();
            _removeState(pointer);
        }
    }

    internal virtual void _removeState(long pointer)
    {
        if ((this._pointers is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => this._pointers!.ContainsKey(pointer));
        GestureBinding.instance.pointerRouter.removeRoute(pointer, (Action<PointerEvent>)this._handleEvent);
        this._pointers!.remove(pointer)!.dispose();
    }

    public override void dispose()
    {
        this._pointers!.Keys.ToList().forEach(this._removeState);
        DartRuntimePrimitives.Assert(() => (checked((long)(this._pointers!.Count)) == 0));
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
        DartRuntimePrimitives.Assert(() => (pendingDelta is not null));
        if ((DartRuntimePrimitives.RequireValue(pendingDelta).distance > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings)))
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
        DartRuntimePrimitives.Assert(() => (pendingDelta is not null));
        if ((DartRuntimePrimitives.RequireValue(pendingDelta).dx.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings)))
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
        DartRuntimePrimitives.Assert(() => (pendingDelta is not null));
        if ((DartRuntimePrimitives.RequireValue(pendingDelta).dy.abs() > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings)))
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
        DartRuntimePrimitives.Assert(() => (this._timer is not null));
        DartRuntimePrimitives.Assert(() => (pendingDelta is not null));
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(pendingDelta).distance <= global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings)));
        _timer = null;
        if ((this._starter is not null))
        {
            this._starter!(initialPosition);
            _starter = null;
        }
        else
        {
            resolve(GestureDisposition.accepted);
        }
        DartRuntimePrimitives.Assert(() => (this._starter is null));
    }

    internal virtual void _ensureTimerStopped()
    {
        this._timer?.cancel();
        _timer = null;
    }

    public override void accepted(Func<Offset, Drag?> starter)
    {
        DartRuntimePrimitives.Assert(() => (this._starter is null));
        if ((this._timer is null))
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
        if ((this._timer is null))
        {
            DartRuntimePrimitives.Assert(() => (this._starter is not null));
            return;
        }
        DartRuntimePrimitives.Assert(() => (pendingDelta is not null));
        if ((DartRuntimePrimitives.RequireValue(pendingDelta).distance > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(kind, gestureSettings)))
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
        return new _DelayedPointerState__multidrag(@event.position, DartRuntimePrimitives.RequireValue(this.delay), @event.kind, gestureSettings);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "long multidrag";
}


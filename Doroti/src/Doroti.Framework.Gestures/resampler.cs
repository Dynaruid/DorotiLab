// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/resampler.dart
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

public delegate void HandleEventCallback(PointerEvent @event);

public class PointerEventResampler
{
    internal virtual Queue<PointerEvent> _queuedEvents { get; private set; } = new Queue<PointerEvent>();
    internal virtual PointerEvent? _last { get; set; } = default;
    internal virtual PointerEvent? _next { get; set; } = default;
    internal virtual Offset _position { get; set; } = Offset.zero;
    internal virtual bool _isTracked { get; set; } = false;
    internal virtual bool _isDown { get; set; } = false;
    internal virtual long _pointerIdentifier { get; set; } = 0L;
    internal virtual long _hasButtons { get; set; } = 0L;

    internal virtual PointerEvent _toHoverEvent(PointerEvent @event, Offset position, Offset delta, Duration timeStamp, long buttons)
    {
        return new PointerHoverEvent(viewId: ((PointerEvent)@event).viewId, timeStamp: timeStamp, kind: ((PointerEvent)@event).kind, device: ((PointerEvent)@event).device, position: position, delta: delta, buttons: ((PointerEvent)@event).buttons, obscured: ((PointerEvent)@event).obscured, pressureMin: ((PointerEvent)@event).pressureMin, pressureMax: ((PointerEvent)@event).pressureMax, distance: ((PointerEvent)@event).distance, distanceMax: ((PointerEvent)@event).distanceMax, size: ((PointerEvent)@event).size, radiusMajor: ((PointerEvent)@event).radiusMajor, radiusMinor: ((PointerEvent)@event).radiusMinor, radiusMin: ((PointerEvent)@event).radiusMin, radiusMax: ((PointerEvent)@event).radiusMax, orientation: ((PointerEvent)@event).orientation, tilt: ((PointerEvent)@event).tilt, synthesized: ((PointerEvent)@event).synthesized, embedderId: ((PointerEvent)@event).embedderId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual PointerEvent _toMoveEvent(PointerEvent @event, Offset position, Offset delta, long pointerIdentifier, Duration timeStamp, long buttons)
    {
        return new PointerMoveEvent(viewId: ((PointerEvent)@event).viewId, timeStamp: timeStamp, pointer: pointerIdentifier, kind: ((PointerEvent)@event).kind, device: ((PointerEvent)@event).device, position: position, delta: delta, buttons: buttons, obscured: ((PointerEvent)@event).obscured, pressure: ((PointerEvent)@event).pressure, pressureMin: ((PointerEvent)@event).pressureMin, pressureMax: ((PointerEvent)@event).pressureMax, distanceMax: ((PointerEvent)@event).distanceMax, size: ((PointerEvent)@event).size, radiusMajor: ((PointerEvent)@event).radiusMajor, radiusMinor: ((PointerEvent)@event).radiusMinor, radiusMin: ((PointerEvent)@event).radiusMin, radiusMax: ((PointerEvent)@event).radiusMax, orientation: ((PointerEvent)@event).orientation, tilt: ((PointerEvent)@event).tilt, platformData: ((PointerEvent)@event).platformData, synthesized: ((PointerEvent)@event).synthesized, embedderId: ((PointerEvent)@event).embedderId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual PointerEvent _toMoveOrHoverEvent(PointerEvent @event, Offset position, Offset delta, long pointerIdentifier, Duration timeStamp, bool isDown, long buttons)
    {
        return (isDown ? _toMoveEvent(@event, position, delta, pointerIdentifier, timeStamp, buttons) : _toHoverEvent(@event, position, delta, timeStamp, buttons));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _positionAt(Duration sampleTime)
    {
        double x = (this._next?.position.dx ?? 0.0);
        double y = (this._next?.position.dy ?? 0.0);
        Duration nextTimeStamp = (this._next?.timeStamp ?? Duration.zero);
        Duration lastTimeStamp = (this._last?.timeStamp ?? Duration.zero);
        if (((nextTimeStamp > sampleTime) && (nextTimeStamp > lastTimeStamp)))
        {
            double interval = ((nextTimeStamp - lastTimeStamp)).inMicroseconds.toDouble();
            double scalar = (((sampleTime - lastTimeStamp)).inMicroseconds.toDouble() / interval);
            double lastX = (this._last?.position.dx ?? 0.0);
            double lastY = (this._last?.position.dy ?? 0.0);
            x = (lastX + (((x - lastX)) * scalar));
            y = (lastY + (((y - lastY)) * scalar));
        }
        return new global::Doroti.Ui.Offset(x, y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _processPointerEvents(Duration sampleTime)
    {
        IEnumerator<PointerEvent> it = this._queuedEvents.GetEnumerator();
        while (it.MoveNext())
        {
            PointerEvent @event = it.Current;
            if (((((PointerEvent)@event).timeStamp <= sampleTime) || (this._last is null)))
            {
                _last = @event;
                _next = @event;
                continue;
            }
            Duration nextTimeStamp = (this._next?.timeStamp ?? Duration.zero);
            if ((nextTimeStamp < sampleTime))
            {
                _next = @event;
                break;
            }
        }
    }

    internal virtual void _dequeueAndSampleNonHoverOrMovePointerEventsUntil(Duration sampleTime, Duration nextSampleTime, Action<PointerEvent> callback)
    {
        var endTime = sampleTime;
        IEnumerator<PointerEvent> it = this._queuedEvents.GetEnumerator();
        while (it.MoveNext())
        {
            PointerEvent @event = it.Current;
            if ((((PointerEvent)@event).timeStamp > sampleTime))
            {
                if ((((PointerEvent)@event).timeStamp >= nextSampleTime))
                {
                    break;
                }
                if (((@event is PointerUpEvent) || (@event is PointerRemovedEvent)))
                {
                    endTime = ((PointerEvent)@event).timeStamp;
                    continue;
                }
                if (((@event is not PointerMoveEvent) && (@event is not PointerHoverEvent)))
                {
                    break;
                }
            }
        }
        while ((this._queuedEvents.Count != 0))
        {
            PointerEvent eventLocal = this._queuedEvents.Peek();
            if ((((PointerEvent)eventLocal).timeStamp > endTime))
            {
                break;
            }
            bool wasTracked = this._isTracked;
            bool wasDown = this._isDown;
            long hadButtons = this._hasButtons;
            _isTracked = (eventLocal is not PointerRemovedEvent);
            _isDown = ((PointerEvent)eventLocal).down;
            _hasButtons = ((PointerEvent)eventLocal).buttons;
            global::Doroti.Ui.Offset positionLocal = _positionAt(sampleTime);
            if ((this._isTracked && !wasTracked))
            {
                _position = positionLocal;
            }
            long pointerIdentifier = ((PointerEvent)eventLocal).pointer;
            DartRuntimePrimitives.Assert(() => (!wasDown || (this._pointerIdentifier == pointerIdentifier)));
            _pointerIdentifier = pointerIdentifier;
            if (((eventLocal is not PointerMoveEvent) && (eventLocal is not PointerHoverEvent)))
            {
                if ((!object.Equals(positionLocal, this._position)))
                {
                    global::Doroti.Ui.Offset deltaLocal = (positionLocal - this._position);
                    callback(_toMoveOrHoverEvent(eventLocal, positionLocal, deltaLocal, this._pointerIdentifier, sampleTime, wasDown, hadButtons));
                    _position = positionLocal;
                }
                callback(eventLocal.copyWith(position: positionLocal, delta: Offset.zero, pointer: pointerIdentifier, timeStamp: sampleTime));
            }
            this._queuedEvents.Dequeue();
        }
    }

    internal virtual void _samplePointerPosition(Duration sampleTime, Action<PointerEvent> callback)
    {
        global::Doroti.Ui.Offset position = _positionAt(sampleTime);
        PointerEvent? next = this._next;
        if (((!object.Equals(position, this._position)) && (next is not null)))
        {
            global::Doroti.Ui.Offset delta = (position - this._position);
            callback(_toMoveOrHoverEvent(next, position, delta, this._pointerIdentifier, sampleTime, this._isDown, this._hasButtons));
            _position = position;
        }
    }

    public virtual void addEvent(PointerEvent @event)
    {
        this._queuedEvents.Enqueue(@event);
    }

    public virtual void sample(Duration sampleTime, Duration nextSampleTime, Action<PointerEvent> callback)
    {
        _processPointerEvents(sampleTime);
        _dequeueAndSampleNonHoverOrMovePointerEventsUntil(sampleTime, nextSampleTime, (Action<PointerEvent>)callback);
        if (this._isTracked)
        {
            _samplePointerPosition(sampleTime, (Action<PointerEvent>)callback);
        }
    }

    public virtual void stop(Action<PointerEvent> callback)
    {
        while ((this._queuedEvents.Count != 0))
        {
            callback(this._queuedEvents.Dequeue());
        }
        _pointerIdentifier = 0L;
        _isDown = false;
        _isTracked = false;
        _position = Offset.zero;
        _next = null;
        _last = null;
    }

    public virtual bool hasPendingEvents => (this._queuedEvents.Count != 0);
    public virtual bool isTracked => this._isTracked;
    public virtual bool isDown => this._isDown;
}


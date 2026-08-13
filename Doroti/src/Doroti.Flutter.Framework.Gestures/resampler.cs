// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/resampler.dart
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

    internal virtual global::Doroti.Flutter.Ui.Offset _positionAt(Duration sampleTime)
    {
        double x__4072 = (this._next?.position.dx ?? 0.0);
        double y__4114 = (this._next?.position.dy ?? 0.0);
        Duration nextTimeStamp__4165 = (this._next?.timeStamp ?? Duration.zero);
        Duration lastTimeStamp__4235 = (this._last?.timeStamp ?? Duration.zero);
        if (((nextTimeStamp__4165 > sampleTime) && (nextTimeStamp__4165 > lastTimeStamp__4235)))
        {
            double interval__4436 = ((nextTimeStamp__4165 - lastTimeStamp__4235)).inMicroseconds.toDouble();
            double scalar__4525 = (((sampleTime - lastTimeStamp__4235)).inMicroseconds.toDouble() / interval__4436);
            double lastX__4620 = (this._last?.position.dx ?? 0.0);
            double lastY__4674 = (this._last?.position.dy ?? 0.0);
            x__4072 = (lastX__4620 + (((x__4072 - lastX__4620)) * scalar__4525));
            y__4114 = (lastY__4674 + (((y__4114 - lastY__4674)) * scalar__4525));
        }
        return new global::Doroti.Flutter.Ui.Offset(x__4072, y__4114);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _processPointerEvents(Duration sampleTime)
    {
        IEnumerator<PointerEvent> it__4911 = this._queuedEvents.GetEnumerator();
        while (it__4911.MoveNext())
        {
            PointerEvent @event__4993 = it__4911.Current;
            if (((((PointerEvent)@event__4993).timeStamp <= sampleTime) || (this._last is null)))
            {
                _last = @event__4993;
                _next = @event__4993;
                continue;
            }
            Duration nextTimeStamp__5420 = (this._next?.timeStamp ?? Duration.zero);
            if ((nextTimeStamp__5420 < sampleTime))
            {
                _next = @event__4993;
                break;
            }
        }
    }

    internal virtual void _dequeueAndSampleNonHoverOrMovePointerEventsUntil(Duration sampleTime, Duration nextSampleTime, Action<PointerEvent> callback)
    {
        var endTime__5728 = sampleTime;
        IEnumerator<PointerEvent> it__5832 = this._queuedEvents.GetEnumerator();
        while (it__5832.MoveNext())
        {
            PointerEvent @event__5914 = it__5832.Current;
            if ((((PointerEvent)@event__5914).timeStamp > sampleTime))
            {
                if ((((PointerEvent)@event__5914).timeStamp >= nextSampleTime))
                {
                    break;
                }
                if (((@event__5914 is PointerUpEvent) || (@event__5914 is PointerRemovedEvent)))
                {
                    endTime__5728 = ((PointerEvent)@event__5914).timeStamp;
                    continue;
                }
                if (((@event__5914 is not PointerMoveEvent) && (@event__5914 is not PointerHoverEvent)))
                {
                    break;
                }
            }
        }
        while ((this._queuedEvents.Count != 0))
        {
            PointerEvent @event__6750 = this._queuedEvents.Peek();
            if ((((PointerEvent)@event__6750).timeStamp > endTime__5728))
            {
                break;
            }
            bool wasTracked__6924 = this._isTracked;
            bool wasDown__6966 = this._isDown;
            long hadButtons__7001 = this._hasButtons;
            _isTracked = (@event__6750 is not PointerRemovedEvent);
            _isDown = ((PointerEvent)@event__6750).down;
            _hasButtons = ((PointerEvent)@event__6750).buttons;
            global::Doroti.Flutter.Ui.Offset position__7227 = _positionAt(sampleTime);
            if ((this._isTracked && !wasTracked__6924))
            {
                _position = position__7227;
            }
            long pointerIdentifier__7466 = ((PointerEvent)@event__6750).pointer;
            DartRuntimePrimitives.Assert(() => (!wasDown__6966 || (this._pointerIdentifier == pointerIdentifier__7466)));
            _pointerIdentifier = pointerIdentifier__7466;
            if (((@event__6750 is not PointerMoveEvent) && (@event__6750 is not PointerHoverEvent)))
            {
                if ((!object.Equals(position__7227, this._position)))
                {
                    global::Doroti.Flutter.Ui.Offset delta__8267 = (position__7227 - this._position);
                    callback(_toMoveOrHoverEvent(@event__6750, position__7227, delta__8267, this._pointerIdentifier, sampleTime, wasDown__6966, hadButtons__7001));
                    _position = position__7227;
                }
                callback(@event__6750.copyWith(position: position__7227, delta: Offset.zero, pointer: pointerIdentifier__7466, timeStamp: sampleTime));
            }
            this._queuedEvents.Dequeue();
        }
    }

    internal virtual void _samplePointerPosition(Duration sampleTime, Action<PointerEvent> callback)
    {
        global::Doroti.Flutter.Ui.Offset position__8990 = _positionAt(sampleTime);
        PointerEvent? next__9112 = this._next;
        if (((!object.Equals(position__8990, this._position)) && (next__9112 is not null)))
        {
            global::Doroti.Flutter.Ui.Offset delta__9194 = (position__8990 - this._position);
            callback(_toMoveOrHoverEvent(next__9112, position__8990, delta__9194, this._pointerIdentifier, sampleTime, this._isDown, this._hasButtons));
            _position = position__8990;
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


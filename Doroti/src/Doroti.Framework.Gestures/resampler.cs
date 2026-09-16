// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/resampler.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        return new PointerHoverEvent(viewId: @event.viewId, timeStamp: timeStamp, kind: @event.kind, device: @event.device, position: position, delta: delta, buttons: @event.buttons, obscured: @event.obscured, pressureMin: @event.pressureMin, pressureMax: @event.pressureMax, distance: @event.distance, distanceMax: @event.distanceMax, size: @event.size, radiusMajor: @event.radiusMajor, radiusMinor: @event.radiusMinor, radiusMin: @event.radiusMin, radiusMax: @event.radiusMax, orientation: @event.orientation, tilt: @event.tilt, synthesized: @event.synthesized, embedderId: @event.embedderId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual PointerEvent _toMoveEvent(PointerEvent @event, Offset position, Offset delta, long pointerIdentifier, Duration timeStamp, long buttons)
    {
        return new PointerMoveEvent(viewId: @event.viewId, timeStamp: timeStamp, pointer: pointerIdentifier, kind: @event.kind, device: @event.device, position: position, delta: delta, buttons: buttons, obscured: @event.obscured, pressure: @event.pressure, pressureMin: @event.pressureMin, pressureMax: @event.pressureMax, distanceMax: @event.distanceMax, size: @event.size, radiusMajor: @event.radiusMajor, radiusMinor: @event.radiusMinor, radiusMin: @event.radiusMin, radiusMax: @event.radiusMax, orientation: @event.orientation, tilt: @event.tilt, platformData: @event.platformData, synthesized: @event.synthesized, embedderId: @event.embedderId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual PointerEvent _toMoveOrHoverEvent(PointerEvent @event, Offset position, Offset delta, long pointerIdentifier, Duration timeStamp, bool isDown, long buttons)
    {
        return isDown ? _toMoveEvent(@event, position, delta, pointerIdentifier, timeStamp, buttons) : _toHoverEvent(@event, position, delta, timeStamp, buttons);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _positionAt(Duration sampleTime)
    {
        double x = _next?.position.dx ?? 0.0;
        double y = _next?.position.dy ?? 0.0;
        Duration nextTimeStamp = _next?.timeStamp ?? Duration.zero;
        Duration lastTimeStamp = _last?.timeStamp ?? Duration.zero;
        if ((nextTimeStamp > sampleTime) && (nextTimeStamp > lastTimeStamp))
        {
            double interval = (nextTimeStamp - lastTimeStamp).inMicroseconds.toDouble();
            double scalar = (sampleTime - lastTimeStamp).inMicroseconds.toDouble() / interval;
            double lastX = _last?.position.dx ?? 0.0;
            double lastY = _last?.position.dy ?? 0.0;
            x = lastX + ((x - lastX) * scalar);
            y = lastY + ((y - lastY) * scalar);
        }
        return new Offset(x, y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _processPointerEvents(Duration sampleTime)
    {
        IEnumerator<PointerEvent> it = _queuedEvents.GetEnumerator();
        while (it.MoveNext())
        {
            PointerEvent @event = it.Current;
            if ((@event.timeStamp <= sampleTime) || (_last is null))
            {
                _last = @event;
                _next = @event;
                continue;
            }
            Duration nextTimeStamp = _next?.timeStamp ?? Duration.zero;
            if (nextTimeStamp < sampleTime)
            {
                _next = @event;
                break;
            }
        }
    }

    internal virtual void _dequeueAndSampleNonHoverOrMovePointerEventsUntil(Duration sampleTime, Duration nextSampleTime, Action<PointerEvent> callback)
    {
        var endTime = sampleTime;
        IEnumerator<PointerEvent> it = _queuedEvents.GetEnumerator();
        while (it.MoveNext())
        {
            PointerEvent @event = it.Current;
            if (@event.timeStamp > sampleTime)
            {
                if (@event.timeStamp >= nextSampleTime)
                {
                    break;
                }
                if ((@event is PointerUpEvent) || (@event is PointerRemovedEvent))
                {
                    endTime = @event.timeStamp;
                    continue;
                }
                if ((@event is not PointerMoveEvent) && (@event is not PointerHoverEvent))
                {
                    break;
                }
            }
        }
        while (_queuedEvents.Count != 0)
        {
            PointerEvent eventLocal = _queuedEvents.Peek();
            if (eventLocal.timeStamp > endTime)
            {
                break;
            }
            bool wasTracked = _isTracked;
            bool wasDown = _isDown;
            long hadButtons = _hasButtons;
            _isTracked = eventLocal is not PointerRemovedEvent;
            _isDown = eventLocal.down;
            _hasButtons = eventLocal.buttons;
            Offset positionLocal = _positionAt(sampleTime);
            if (_isTracked && !wasTracked)
            {
                _position = positionLocal;
            }
            long pointerIdentifier = eventLocal.pointer;
            DartRuntimePrimitives.Assert(() => !wasDown || (_pointerIdentifier == pointerIdentifier));
            _pointerIdentifier = pointerIdentifier;
            if ((eventLocal is not PointerMoveEvent) && (eventLocal is not PointerHoverEvent))
            {
                if (!Equals(positionLocal, _position))
                {
                    Offset deltaLocal = positionLocal - _position;
                    callback(_toMoveOrHoverEvent(eventLocal, positionLocal, deltaLocal, _pointerIdentifier, sampleTime, wasDown, hadButtons));
                    _position = positionLocal;
                }
                callback(eventLocal.copyWith(position: positionLocal, delta: Offset.zero, pointer: pointerIdentifier, timeStamp: sampleTime));
            }
            _queuedEvents.Dequeue();
        }
    }

    internal virtual void _samplePointerPosition(Duration sampleTime, Action<PointerEvent> callback)
    {
        Offset position = _positionAt(sampleTime);
        PointerEvent? next = _next;
        if ((!Equals(position, _position)) && (next is not null))
        {
            Offset delta = position - _position;
            callback(_toMoveOrHoverEvent(next, position, delta, _pointerIdentifier, sampleTime, _isDown, _hasButtons));
            _position = position;
        }
    }

    public virtual void addEvent(PointerEvent @event)
    {
        _queuedEvents.Enqueue(@event);
    }

    public virtual void sample(Duration sampleTime, Duration nextSampleTime, Action<PointerEvent> callback)
    {
        _processPointerEvents(sampleTime);
        _dequeueAndSampleNonHoverOrMovePointerEventsUntil(sampleTime, nextSampleTime, callback);
        if (_isTracked)
        {
            _samplePointerPosition(sampleTime, callback);
        }
    }

    public virtual void stop(Action<PointerEvent> callback)
    {
        while (_queuedEvents.Count != 0)
        {
            callback(_queuedEvents.Dequeue());
        }
        _pointerIdentifier = 0L;
        _isDown = false;
        _isTracked = false;
        _position = Offset.zero;
        _next = null;
        _last = null;
    }

    public virtual bool hasPendingEvents => _queuedEvents.Count != 0;
    public virtual bool isTracked => _isTracked;
    public virtual bool isDown => _isDown;
}


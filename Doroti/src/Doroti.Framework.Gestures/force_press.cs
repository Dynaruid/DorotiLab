// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/force_press.dart
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

internal enum _ForceState__force_press
{
    ready,
    possible,
    accepted,
    started,
    peaked
}

public class ForcePressDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual double pressure { get; private set; } = default!;

    public ForcePressDetails(Offset globalPosition, Offset? localPosition = null, double pressure = default!)
    {
        this.__field_globalPosition = globalPosition;
        this.pressure = pressure;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DoubleProperty("pressure", this.pressure));
    }

}

public delegate void GestureForcePressStartCallback(ForcePressDetails details);

public delegate void GestureForcePressPeakCallback(ForcePressDetails details);

public delegate void GestureForcePressUpdateCallback(ForcePressDetails details);

public delegate void GestureForcePressEndCallback(ForcePressDetails details);

public delegate double GestureForceInterpolation(double pressureMin, double pressureMax, double pressure);

public class ForcePressGestureRecognizer : OneSequenceGestureRecognizer
{
    public virtual Action<ForcePressDetails>? onStart { get; set; } = default;
    public virtual Action<ForcePressDetails>? onUpdate { get; set; } = default;
    public virtual Action<ForcePressDetails>? onPeak { get; set; } = default;
    public virtual Action<ForcePressDetails>? onEnd { get; set; } = default;
    public virtual double startPressure { get; private set; } = default!;
    public virtual double peakPressure { get; private set; } = default!;
    public virtual Func<double, double, double, double> interpolation { get; private set; } = default!;
    internal virtual OffsetPair _lastPosition { get; set; } = default!;
    internal virtual double _lastPressure { get; set; } = default!;
    internal virtual _ForceState__force_press _state { get; set; } = _ForceState__force_press.ready;

    public ForcePressGestureRecognizer(double startPressure = 0.4, double peakPressure = 0.85, Func<double, double, double, double> interpolation = default!, object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
        Func<double, double, double, double> __interpolation = interpolation ?? _inverseLerp;
        this.startPressure = startPressure;
        this.peakPressure = peakPressure;
        this.interpolation = __interpolation;
        System.Diagnostics.Debug.Assert((peakPressure > startPressure));
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if ((@event.pressureMax <= 1.0))
        {
            resolve(GestureDisposition.rejected);
        }
        else
        {
            base.addAllowedPointer(@event);
            if ((object.Equals(this._state, _ForceState__force_press.ready)))
            {
                _state = _ForceState__force_press.possible;
                _lastPosition = OffsetPair.CreateFromEventPosition(@event);
            }
        }
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._state, _ForceState__force_press.ready)));
        if (((@event is PointerMoveEvent) || (@event is PointerDownEvent)))
        {
            double pressureLocal = this.interpolation(((PointerEvent)@event).pressureMin, ((PointerEvent)@event).pressureMax, ((PointerEvent)@event).pressure);
            DartRuntimePrimitives.Assert(() => ((((pressureLocal >= 0.0) && (pressureLocal <= 1.0))) || double.IsNaN(pressureLocal)));
            _lastPosition = OffsetPair.CreateFromEventPosition(@event);
            _lastPressure = pressureLocal;
            if ((object.Equals(this._state, _ForceState__force_press.possible)))
            {
                if ((pressureLocal > this.startPressure))
                {
                    _state = _ForceState__force_press.started;
                    resolve(GestureDisposition.accepted);
                }
                else
                {
                    if ((((PointerEvent)@event).delta.distanceSquared > global::Doroti.Framework.Gestures.EventsLibrary.computeHitSlop(((PointerEvent)@event).kind, gestureSettings)))
                    {
                        resolve(GestureDisposition.rejected);
                    }
                }
            }
            if (((pressureLocal > this.startPressure) && (object.Equals(this._state, _ForceState__force_press.accepted))))
            {
                _state = _ForceState__force_press.started;
                if ((this.onStart is not null))
                {
                    invokeCallback<object?>("onStart", () => { ((Action)((() => this.onStart!(new ForcePressDetails(pressure: pressureLocal, globalPosition: ((OffsetPair)this._lastPosition).global, localPosition: ((OffsetPair)this._lastPosition).local)))))(); return null; });
                }
            }
            if ((((this.onPeak is not null) && (pressureLocal > this.peakPressure)) && ((object.Equals(this._state, _ForceState__force_press.started)))))
            {
                _state = _ForceState__force_press.peaked;
                if ((this.onPeak is not null))
                {
                    invokeCallback<object?>("onPeak", () => { ((Action)((() => this.onPeak!(new ForcePressDetails(pressure: pressureLocal, globalPosition: ((PointerEvent)@event).position, localPosition: ((PointerEvent)@event).localPosition)))))(); return null; });
                }
            }
            if ((((this.onUpdate is not null) && !double.IsNaN(pressureLocal)) && (((object.Equals(this._state, _ForceState__force_press.started)) || (object.Equals(this._state, _ForceState__force_press.peaked))))))
            {
                if ((this.onUpdate is not null))
                {
                    invokeCallback<object?>("onUpdate", () => { ((Action)((() => this.onUpdate!(new ForcePressDetails(pressure: pressureLocal, globalPosition: ((PointerEvent)@event).position, localPosition: ((PointerEvent)@event).localPosition)))))(); return null; });
                }
            }
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public override void acceptGesture(long pointer)
    {
        if ((object.Equals(this._state, _ForceState__force_press.possible)))
        {
            _state = _ForceState__force_press.accepted;
        }
        if (((this.onStart is not null) && (object.Equals(this._state, _ForceState__force_press.started))))
        {
            invokeCallback<object?>("onStart", () => { ((Action)((() => this.onStart!(new ForcePressDetails(pressure: this._lastPressure, globalPosition: ((OffsetPair)this._lastPosition).global, localPosition: ((OffsetPair)this._lastPosition).local)))))(); return null; });
        }
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        bool wasAccepted = ((object.Equals(this._state, _ForceState__force_press.started)) || (object.Equals(this._state, _ForceState__force_press.peaked)));
        if ((object.Equals(this._state, _ForceState__force_press.possible)))
        {
            resolve(GestureDisposition.rejected);
            return;
        }
        if ((wasAccepted && (this.onEnd is not null)))
        {
            if ((this.onEnd is not null))
            {
                invokeCallback<object?>("onEnd", () => { ((Action)((() => this.onEnd!(new ForcePressDetails(pressure: 0.0, globalPosition: ((OffsetPair)this._lastPosition).global, localPosition: ((OffsetPair)this._lastPosition).local)))))(); return null; });
            }
        }
        _state = _ForceState__force_press.ready;
    }

    public override void rejectGesture(long pointer)
    {
        stopTrackingPointer(pointer);
        didStopTrackingLastPointer(pointer);
    }

    internal static double _inverseLerp(double min, double max, double t)
    {
        DartRuntimePrimitives.Assert(() => (min <= max));
        double value = (((t - min)) / ((max - min)));
        if (!double.IsNaN(value))
        {
            value = Dart_uiLibrary.clampDouble(value, 0.0, 1.0);
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string debugDescription => "force press";
}


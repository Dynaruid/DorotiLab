// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/force_press.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

internal enum _ForceState__force_press
{
    ready,
    possible,
    accepted,
    started,
    peaked,
}

public class ForcePressDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition
    {
        get => __field_globalPosition;
    }
    private Offset __field_localPosition = default!;
    public override Offset localPosition
    {
        get => __field_localPosition;
    }
    public virtual double pressure { get; private set; } = default!;

    public ForcePressDetails(
        Offset globalPosition,
        Offset? localPosition = null,
        double pressure = default!
    )
    {
        __field_globalPosition = globalPosition;
        this.pressure = pressure;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DoubleProperty("pressure", pressure));
    }
}

public delegate void GestureForcePressStartCallback(ForcePressDetails details);

public delegate void GestureForcePressPeakCallback(ForcePressDetails details);

public delegate void GestureForcePressUpdateCallback(ForcePressDetails details);

public delegate void GestureForcePressEndCallback(ForcePressDetails details);

public delegate double GestureForceInterpolation(
    double pressureMin,
    double pressureMax,
    double pressure
);

public class ForcePressGestureRecognizer : OneSequenceGestureRecognizer
{
    public virtual Action<ForcePressDetails>? onStart { get; set; } = default;
    public virtual Action<ForcePressDetails>? onUpdate { get; set; } = default;
    public virtual Action<ForcePressDetails>? onPeak { get; set; } = default;
    public virtual Action<ForcePressDetails>? onEnd { get; set; } = default;
    public virtual double startPressure { get; private set; } = default!;
    public virtual double peakPressure { get; private set; } = default!;
    public virtual Func<double, double, double, double> interpolation { get; private set; } =
        default!;
    internal virtual OffsetPair _lastPosition { get; set; } = default!;
    internal virtual double _lastPressure { get; set; } = default!;
    internal virtual _ForceState__force_press _state { get; set; } = _ForceState__force_press.ready;

    public ForcePressGestureRecognizer(
        double startPressure = 0.4,
        double peakPressure = 0.85,
        Func<double, double, double, double> interpolation = default!,
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null,
        Func<long, bool> allowedButtonsFilter = default!
    )
        : base(
            debugOwner: debugOwner,
            supportedDevices: supportedDevices,
            allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior
        )
    {
        Func<double, double, double, double> __interpolation = interpolation ?? _inverseLerp;
        this.startPressure = startPressure;
        this.peakPressure = peakPressure;
        this.interpolation = __interpolation;
        System.Diagnostics.Debug.Assert(peakPressure > startPressure);
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if (@event.pressureMax <= 1.0)
        {
            resolve(GestureDisposition.rejected);
        }
        else
        {
            base.addAllowedPointer(@event);
            if (Equals(_state, _ForceState__force_press.ready))
            {
                _state = _ForceState__force_press.possible;
                _lastPosition = OffsetPair.CreateFromEventPosition(@event);
            }
        }
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !Equals(_state, _ForceState__force_press.ready));
        if ((@event is PointerMoveEvent) || (@event is PointerDownEvent))
        {
            double pressureLocal = interpolation(
                @event.pressureMin,
                @event.pressureMax,
                @event.pressure
            );
            DartRuntimePrimitives.Assert(() =>
                ((pressureLocal >= 0.0) && (pressureLocal <= 1.0)) || double.IsNaN(pressureLocal)
            );
            _lastPosition = OffsetPair.CreateFromEventPosition(@event);
            _lastPressure = pressureLocal;
            if (Equals(_state, _ForceState__force_press.possible))
            {
                if (pressureLocal > startPressure)
                {
                    _state = _ForceState__force_press.started;
                    resolve(GestureDisposition.accepted);
                }
                else
                {
                    if (
                        @event.delta.distanceSquared
                        > EventsLibrary.computeHitSlop(@event.kind, gestureSettings)
                    )
                    {
                        resolve(GestureDisposition.rejected);
                    }
                }
            }
            if (
                (pressureLocal > startPressure) && Equals(_state, _ForceState__force_press.accepted)
            )
            {
                _state = _ForceState__force_press.started;
                if (onStart is not null)
                {
                    invokeCallback<object?>(
                        "onStart",
                        () =>
                        {
                            (
                                (Action)(
                                    () =>
                                        onStart!(
                                            new ForcePressDetails(
                                                pressure: pressureLocal,
                                                globalPosition: _lastPosition.global,
                                                localPosition: _lastPosition.local
                                            )
                                        )
                                )
                            )();
                            return null;
                        }
                    );
                }
            }
            if (
                (onPeak is not null)
                && (pressureLocal > peakPressure)
                && Equals(_state, _ForceState__force_press.started)
            )
            {
                _state = _ForceState__force_press.peaked;
                if (onPeak is not null)
                {
                    invokeCallback<object?>(
                        "onPeak",
                        () =>
                        {
                            (
                                (Action)(
                                    () =>
                                        onPeak!(
                                            new ForcePressDetails(
                                                pressure: pressureLocal,
                                                globalPosition: @event.position,
                                                localPosition: @event.localPosition
                                            )
                                        )
                                )
                            )();
                            return null;
                        }
                    );
                }
            }
            if (
                (onUpdate is not null)
                && !double.IsNaN(pressureLocal)
                && (
                    Equals(_state, _ForceState__force_press.started)
                    || Equals(_state, _ForceState__force_press.peaked)
                )
            )
            {
                if (onUpdate is not null)
                {
                    invokeCallback<object?>(
                        "onUpdate",
                        () =>
                        {
                            (
                                (Action)(
                                    () =>
                                        onUpdate!(
                                            new ForcePressDetails(
                                                pressure: pressureLocal,
                                                globalPosition: @event.position,
                                                localPosition: @event.localPosition
                                            )
                                        )
                                )
                            )();
                            return null;
                        }
                    );
                }
            }
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public override void acceptGesture(long pointer)
    {
        if (Equals(_state, _ForceState__force_press.possible))
        {
            _state = _ForceState__force_press.accepted;
        }
        if ((onStart is not null) && Equals(_state, _ForceState__force_press.started))
        {
            invokeCallback<object?>(
                "onStart",
                () =>
                {
                    (
                        (Action)(
                            () =>
                                onStart!(
                                    new ForcePressDetails(
                                        pressure: _lastPressure,
                                        globalPosition: _lastPosition.global,
                                        localPosition: _lastPosition.local
                                    )
                                )
                        )
                    )();
                    return null;
                }
            );
        }
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        bool wasAccepted =
            Equals(_state, _ForceState__force_press.started)
            || Equals(_state, _ForceState__force_press.peaked);
        if (Equals(_state, _ForceState__force_press.possible))
        {
            resolve(GestureDisposition.rejected);
            return;
        }
        if (wasAccepted && (onEnd is not null))
        {
            if (onEnd is not null)
            {
                invokeCallback<object?>(
                    "onEnd",
                    () =>
                    {
                        (
                            (Action)(
                                () =>
                                    onEnd!(
                                        new ForcePressDetails(
                                            pressure: 0.0,
                                            globalPosition: _lastPosition.global,
                                            localPosition: _lastPosition.local
                                        )
                                    )
                            )
                        )();
                        return null;
                    }
                );
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
        DartRuntimePrimitives.Assert(() => min <= max);
        double value = (t - min) / (max - min);
        if (!double.IsNaN(value))
        {
            value = DorotiUiLibrary.clampDouble(value, 0.0, 1.0);
        }
        return value;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string debugDescription => "force press";
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/long_press.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public delegate void GestureLongPressDownCallback(LongPressDownDetails details);

public delegate void GestureLongPressCancelCallback();

public delegate void GestureLongPressCallback();

public delegate void GestureLongPressUpCallback();

public delegate void GestureLongPressStartCallback(LongPressStartDetails details);

public delegate void GestureLongPressMoveUpdateCallback(LongPressMoveUpdateDetails details);

public delegate void GestureLongPressEndCallback(LongPressEndDetails details);

public class LongPressDownDetails : PositionedGestureDetails, Diagnosticable
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
    public virtual PointerDeviceKind? kind { get; private set; }

    public LongPressDownDetails(
        Offset globalPosition = default,
        Offset? localPosition = null,
        PointerDeviceKind? kind = null
    )
    {
        __field_globalPosition = globalPosition;
        this.kind = kind;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
    }
}

public class LongPressStartDetails : PositionedGestureDetails, Diagnosticable
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

    public LongPressStartDetails(Offset globalPosition = default, Offset? localPosition = null)
    {
        __field_globalPosition = globalPosition;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
    }
}

public class LongPressMoveUpdateDetails : PositionedGestureDetails, Diagnosticable
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
    public virtual Offset offsetFromOrigin { get; private set; } = default!;
    public virtual Offset localOffsetFromOrigin { get; private set; } = default!;

    public LongPressMoveUpdateDetails(
        Offset globalPosition = default,
        Offset? localPosition = null,
        Offset offsetFromOrigin = default,
        Offset? localOffsetFromOrigin = null
    )
    {
        __field_globalPosition = globalPosition;
        this.offsetFromOrigin = offsetFromOrigin;
        __field_localPosition = localPosition ?? globalPosition;
        this.localOffsetFromOrigin = localOffsetFromOrigin ?? offsetFromOrigin;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Offset>("offsetFromOrigin", offsetFromOrigin));
        properties.add(
            new DiagnosticsProperty<Offset>("localOffsetFromOrigin", localOffsetFromOrigin)
        );
    }
}

public class LongPressEndDetails : PositionedGestureDetails, Diagnosticable
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
    public virtual Velocity velocity { get; private set; } = default!;

    public LongPressEndDetails(
        Offset globalPosition = default,
        Offset? localPosition = null,
        Velocity velocity = default!
    )
    {
        Velocity __velocity = velocity ?? Velocity.zero;
        __field_globalPosition = globalPosition;
        this.velocity = __velocity;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Velocity>("velocity", velocity));
    }
}

public class LongPressGestureRecognizer : PrimaryPointerGestureRecognizer
{
    internal virtual bool _longPressAccepted { get; set; } = false;
    internal virtual OffsetPair? _longPressOrigin { get; set; } = default;
    internal virtual long? _initialButtons { get; set; } = default;
    public virtual Action<LongPressDownDetails>? onLongPressDown { get; set; } = default;
    public virtual Action? onLongPressCancel { get; set; } = default;
    public virtual Action? onLongPress { get; set; } = default;
    public virtual Action<LongPressStartDetails>? onLongPressStart { get; set; } = default;
    public virtual Action<LongPressMoveUpdateDetails>? onLongPressMoveUpdate { get; set; } =
        default;
    public virtual Action? onLongPressUp { get; set; } = default;
    public virtual Action<LongPressEndDetails>? onLongPressEnd { get; set; } = default;
    public virtual Action<LongPressDownDetails>? onSecondaryLongPressDown { get; set; } = default;
    public virtual Action? onSecondaryLongPressCancel { get; set; } = default;
    public virtual Action? onSecondaryLongPress { get; set; } = default;
    public virtual Action<LongPressStartDetails>? onSecondaryLongPressStart { get; set; } = default;
    public virtual Action<LongPressMoveUpdateDetails>? onSecondaryLongPressMoveUpdate { get; set; } =
        default;
    public virtual Action? onSecondaryLongPressUp { get; set; } = default;
    public virtual Action<LongPressEndDetails>? onSecondaryLongPressEnd { get; set; } = default;
    public virtual Action<LongPressDownDetails>? onTertiaryLongPressDown { get; set; } = default;
    public virtual Action? onTertiaryLongPressCancel { get; set; } = default;
    public virtual Action? onTertiaryLongPress { get; set; } = default;
    public virtual Action<LongPressStartDetails>? onTertiaryLongPressStart { get; set; } = default;
    public virtual Action<LongPressMoveUpdateDetails>? onTertiaryLongPressMoveUpdate { get; set; } =
        default;
    public virtual Action? onTertiaryLongPressUp { get; set; } = default;
    public virtual Action<LongPressEndDetails>? onTertiaryLongPressEnd { get; set; } = default;
    internal virtual VelocityTracker? _velocityTracker { get; set; } = default;

    public LongPressGestureRecognizer(
        Duration? duration = null,
        double? postAcceptSlopTolerance = null,
        HashSet<PointerDeviceKind>? supportedDevices = null,
        object? debugOwner = null,
        Func<long, bool>? allowedButtonsFilter = null
    )
        : base(
            postAcceptSlopTolerance: postAcceptSlopTolerance,
            supportedDevices: supportedDevices,
            debugOwner: debugOwner,
            deadline: duration ?? ConstantsLibrary.kLongPressTimeout,
            allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior
        ) { }

    internal static new bool _defaultButtonAcceptBehavior(long buttons) =>
        (buttons == EventsLibrary.kPrimaryButton)
        || (buttons == EventsLibrary.kSecondaryButton)
        || (buttons == EventsLibrary.kTertiaryButton);

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        switch (@event.buttons)
        {
            case var __constant25085 when Equals(__constant25085, EventsLibrary.kPrimaryButton):
            {
                if (
                    (onLongPressDown is null)
                    && (onLongPressCancel is null)
                    && (onLongPressStart is null)
                    && (onLongPress is null)
                    && (onLongPressMoveUpdate is null)
                    && (onLongPressEnd is null)
                    && (onLongPressUp is null)
                )
                {
                    return false;
                }
                break;
            }
            case var __constant25421 when Equals(__constant25421, EventsLibrary.kSecondaryButton):
            {
                if (
                    (onSecondaryLongPressDown is null)
                    && (onSecondaryLongPressCancel is null)
                    && (onSecondaryLongPressStart is null)
                    && (onSecondaryLongPress is null)
                    && (onSecondaryLongPressMoveUpdate is null)
                    && (onSecondaryLongPressEnd is null)
                    && (onSecondaryLongPressUp is null)
                )
                {
                    return false;
                }
                break;
            }
            case var __constant25822 when Equals(__constant25822, EventsLibrary.kTertiaryButton):
            {
                if (
                    (onTertiaryLongPressDown is null)
                    && (onTertiaryLongPressCancel is null)
                    && (onTertiaryLongPressStart is null)
                    && (onTertiaryLongPress is null)
                    && (onTertiaryLongPressMoveUpdate is null)
                    && (onTertiaryLongPressEnd is null)
                    && (onTertiaryLongPressUp is null)
                )
                {
                    return false;
                }
                break;
            }
            default:
            {
                return false;
            }
        }
        return base.isPointerAllowed(@event);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void didExceedDeadline()
    {
        resolve(GestureDisposition.accepted);
        _longPressAccepted = true;
        base.acceptGesture(
            (
                primaryPointer
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        _checkLongPressStart();
    }

    public override void handlePrimaryPointer(PointerEvent @event)
    {
        if (!@event.synthesized)
        {
            if (@event is PointerDownEvent)
            {
                PointerDownEvent @event__as26655 = (PointerDownEvent)@event;
                _velocityTracker = new VelocityTracker(@event__as26655.kind);
                _velocityTracker!.addPosition(
                    @event__as26655.timeStamp,
                    @event__as26655.localPosition
                );
            }
            if (@event is PointerMoveEvent)
            {
                PointerMoveEvent @event__as26844 = (PointerMoveEvent)@event;
                DartRuntimePrimitives.Assert(() => _velocityTracker is not null);
                _velocityTracker!.addPosition(
                    @event__as26844.timeStamp,
                    @event__as26844.localPosition
                );
            }
        }
        if (@event is PointerUpEvent)
        {
            PointerUpEvent @event__as27015 = (PointerUpEvent)@event;
            if (_longPressAccepted)
            {
                _checkLongPressEnd(@event__as27015);
            }
            else
            {
                resolve(GestureDisposition.rejected);
            }
            _reset();
        }
        else
        {
            if (@event is PointerCancelEvent)
            {
                PointerCancelEvent @event__as27254 = (PointerCancelEvent)@event;
                _checkLongPressCancel();
                _reset();
            }
            else
            {
                if (@event is PointerDownEvent)
                {
                    PointerDownEvent @event__as27347 = (PointerDownEvent)@event;
                    _longPressOrigin = OffsetPair.CreateFromEventPosition(@event__as27347);
                    _initialButtons = @event__as27347.buttons;
                    _checkLongPressDown(@event__as27347);
                }
                else
                {
                    if (@event is PointerMoveEvent)
                    {
                        PointerMoveEvent @event__as27552 = (PointerMoveEvent)@event;
                        if ((@event__as27552.buttons != _initialButtons) && !_longPressAccepted)
                        {
                            resolve(GestureDisposition.rejected);
                            stopTrackingPointer(
                                (
                                    primaryPointer
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                )
                            );
                        }
                        else
                        {
                            if (_longPressAccepted)
                            {
                                _checkLongPressMoveUpdate(@event__as27552);
                            }
                        }
                    }
                }
            }
        }
    }

    internal virtual void _checkLongPressDown(PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _longPressOrigin is not null);
        var details = new LongPressDownDetails(
            globalPosition: _longPressOrigin!.global,
            localPosition: _longPressOrigin!.local,
            kind: getKindForPointer(@event.pointer)
        );
        switch (_initialButtons)
        {
            case var __constant28164 when Equals(__constant28164, EventsLibrary.kPrimaryButton):
            {
                if (onLongPressDown is not null)
                {
                    invokeCallback<object?>(
                        "onLongPressDown",
                        () =>
                        {
                            ((Action)(() => onLongPressDown!(details)))();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant28324 when Equals(__constant28324, EventsLibrary.kSecondaryButton):
            {
                if (onSecondaryLongPressDown is not null)
                {
                    invokeCallback<object?>(
                        "onSecondaryLongPressDown",
                        () =>
                        {
                            ((Action)(() => onSecondaryLongPressDown!(details)))();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant28550 when Equals(__constant28550, EventsLibrary.kTertiaryButton):
            {
                if (onTertiaryLongPressDown is not null)
                {
                    invokeCallback<object?>(
                        "onTertiaryLongPressDown",
                        () =>
                        {
                            ((Action)(() => onTertiaryLongPressDown!(details)))();
                            return null;
                        }
                    );
                }
                break;
            }
            default:
            {
                DartRuntimePrimitives.Assert(() => false);
                break;
            }
        }
    }

    internal virtual void _checkLongPressCancel()
    {
        if (Equals(state, GestureRecognizerState.possible))
        {
            switch (_initialButtons)
            {
                case var __constant28941 when Equals(__constant28941, EventsLibrary.kPrimaryButton):
                {
                    if (onLongPressCancel is not null)
                    {
                        invokeCallback<object?>(
                            "onLongPressCancel",
                            () =>
                            {
                                onLongPressCancel!();
                                return null;
                            }
                        );
                    }
                    break;
                }
                case var __constant29100
                    when Equals(__constant29100, EventsLibrary.kSecondaryButton):
                {
                    if (onSecondaryLongPressCancel is not null)
                    {
                        invokeCallback<object?>(
                            "onSecondaryLongPressCancel",
                            () =>
                            {
                                onSecondaryLongPressCancel!();
                                return null;
                            }
                        );
                    }
                    break;
                }
                case var __constant29288
                    when Equals(__constant29288, EventsLibrary.kTertiaryButton):
                {
                    if (onTertiaryLongPressCancel is not null)
                    {
                        invokeCallback<object?>(
                            "onTertiaryLongPressCancel",
                            () =>
                            {
                                onTertiaryLongPressCancel!();
                                return null;
                            }
                        );
                    }
                    break;
                }
                default:
                {
                    DartRuntimePrimitives.Assert(() => false);
                    break;
                }
            }
        }
    }

    internal virtual void _checkLongPressStart()
    {
        switch (_initialButtons)
        {
            case var __constant29631 when Equals(__constant29631, EventsLibrary.kPrimaryButton):
            {
                if (onLongPressStart is not null)
                {
                    var details = new LongPressStartDetails(
                        globalPosition: _longPressOrigin!.global,
                        localPosition: _longPressOrigin!.local
                    );
                    invokeCallback<object?>(
                        "onLongPressStart",
                        () =>
                        {
                            ((Action)(() => onLongPressStart!(details)))();
                            return null;
                        }
                    );
                }
                if (onLongPress is not null)
                {
                    invokeCallback<object?>(
                        "onLongPress",
                        () =>
                        {
                            onLongPress!();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant30068 when Equals(__constant30068, EventsLibrary.kSecondaryButton):
            {
                if (onSecondaryLongPressStart is not null)
                {
                    var detailsLocal = new LongPressStartDetails(
                        globalPosition: _longPressOrigin!.global,
                        localPosition: _longPressOrigin!.local
                    );
                    invokeCallback<object?>(
                        "onSecondaryLongPressStart",
                        () =>
                        {
                            ((Action)(() => onSecondaryLongPressStart!(detailsLocal)))();
                            return null;
                        }
                    );
                }
                if (onSecondaryLongPress is not null)
                {
                    invokeCallback<object?>(
                        "onSecondaryLongPress",
                        () =>
                        {
                            onSecondaryLongPress!();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant30598 when Equals(__constant30598, EventsLibrary.kTertiaryButton):
            {
                if (onTertiaryLongPressStart is not null)
                {
                    var detailsAlternate = new LongPressStartDetails(
                        globalPosition: _longPressOrigin!.global,
                        localPosition: _longPressOrigin!.local
                    );
                    invokeCallback<object?>(
                        "onTertiaryLongPressStart",
                        () =>
                        {
                            ((Action)(() => onTertiaryLongPressStart!(detailsAlternate)))();
                            return null;
                        }
                    );
                }
                if (onTertiaryLongPress is not null)
                {
                    invokeCallback<object?>(
                        "onTertiaryLongPress",
                        () =>
                        {
                            onTertiaryLongPress!();
                            return null;
                        }
                    );
                }
                break;
            }
            default:
            {
                DartRuntimePrimitives.Assert(() => false);
                break;
            }
        }
    }

    internal virtual void _checkLongPressMoveUpdate(PointerEvent @event)
    {
        var details = new LongPressMoveUpdateDetails(
            globalPosition: @event.position,
            localPosition: @event.localPosition,
            offsetFromOrigin: @event.position - _longPressOrigin!.global,
            localOffsetFromOrigin: @event.localPosition - _longPressOrigin!.local
        );
        switch (_initialButtons)
        {
            case var __constant31571 when Equals(__constant31571, EventsLibrary.kPrimaryButton):
            {
                if (onLongPressMoveUpdate is not null)
                {
                    invokeCallback<object?>(
                        "onLongPressMoveUpdate",
                        () =>
                        {
                            ((Action)(() => onLongPressMoveUpdate!(details)))();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant31749 when Equals(__constant31749, EventsLibrary.kSecondaryButton):
            {
                if (onSecondaryLongPressMoveUpdate is not null)
                {
                    invokeCallback<object?>(
                        "onSecondaryLongPressMoveUpdate",
                        () =>
                        {
                            ((Action)(() => onSecondaryLongPressMoveUpdate!(details)))();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant31993 when Equals(__constant31993, EventsLibrary.kTertiaryButton):
            {
                if (onTertiaryLongPressMoveUpdate is not null)
                {
                    invokeCallback<object?>(
                        "onTertiaryLongPressMoveUpdate",
                        () =>
                        {
                            ((Action)(() => onTertiaryLongPressMoveUpdate!(details)))();
                            return null;
                        }
                    );
                }
                break;
            }
            default:
            {
                DartRuntimePrimitives.Assert(() => false);
                break;
            }
        }
    }

    internal virtual void _checkLongPressEnd(PointerEvent @event)
    {
        VelocityEstimate? estimate = _velocityTracker!.getVelocityEstimate();
        Velocity velocityLocal =
            (estimate is null)
                ? Velocity.zero
                : new Velocity(pixelsPerSecond: estimate.pixelsPerSecond);
        var details = new LongPressEndDetails(
            globalPosition: @event.position,
            localPosition: @event.localPosition,
            velocity: velocityLocal
        );
        _velocityTracker = null;
        switch (_initialButtons)
        {
            case var __constant32796 when Equals(__constant32796, EventsLibrary.kPrimaryButton):
            {
                if (onLongPressEnd is not null)
                {
                    invokeCallback<object?>(
                        "onLongPressEnd",
                        () =>
                        {
                            ((Action)(() => onLongPressEnd!(details)))();
                            return null;
                        }
                    );
                }
                if (onLongPressUp is not null)
                {
                    invokeCallback<object?>(
                        "onLongPressUp",
                        () =>
                        {
                            onLongPressUp!();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant33065 when Equals(__constant33065, EventsLibrary.kSecondaryButton):
            {
                if (onSecondaryLongPressEnd is not null)
                {
                    invokeCallback<object?>(
                        "onSecondaryLongPressEnd",
                        () =>
                        {
                            ((Action)(() => onSecondaryLongPressEnd!(details)))();
                            return null;
                        }
                    );
                }
                if (onSecondaryLongPressUp is not null)
                {
                    invokeCallback<object?>(
                        "onSecondaryLongPressUp",
                        () =>
                        {
                            onSecondaryLongPressUp!();
                            return null;
                        }
                    );
                }
                break;
            }
            case var __constant33390 when Equals(__constant33390, EventsLibrary.kTertiaryButton):
            {
                if (onTertiaryLongPressEnd is not null)
                {
                    invokeCallback<object?>(
                        "onTertiaryLongPressEnd",
                        () =>
                        {
                            ((Action)(() => onTertiaryLongPressEnd!(details)))();
                            return null;
                        }
                    );
                }
                if (onTertiaryLongPressUp is not null)
                {
                    invokeCallback<object?>(
                        "onTertiaryLongPressUp",
                        () =>
                        {
                            onTertiaryLongPressUp!();
                            return null;
                        }
                    );
                }
                break;
            }
            default:
            {
                DartRuntimePrimitives.Assert(() => false);
                break;
            }
        }
    }

    internal virtual void _reset()
    {
        _longPressAccepted = false;
        _longPressOrigin = null;
        _initialButtons = null;
        _velocityTracker = null;
    }

    public override void resolve(GestureDisposition disposition)
    {
        if (Equals(disposition, GestureDisposition.rejected))
        {
            if (_longPressAccepted)
            {
                _reset();
            }
            else
            {
                _checkLongPressCancel();
            }
        }
        base.resolve(disposition);
    }

    public override void acceptGesture(long pointer) { }

    public override string debugDescription => "long press";
}

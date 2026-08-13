// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/long_press.dart
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
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind? kind { get; private set; }

    public LongPressDownDetails(Offset globalPosition = default, Offset? localPosition = null, PointerDeviceKind? kind = null)
    {
        this.__field_globalPosition = globalPosition;
        this.kind = kind;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Flutter.Ui.PointerDeviceKind>("kind", this.kind));
    }

}

public class LongPressStartDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }

    public LongPressStartDetails(Offset globalPosition = default, Offset? localPosition = null)
    {
        this.__field_globalPosition = globalPosition;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localPosition", this.localPosition));
    }

}

public class LongPressMoveUpdateDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Offset offsetFromOrigin { get; private set; } = default!;
    public virtual Offset localOffsetFromOrigin { get; private set; } = default!;

    public LongPressMoveUpdateDetails(Offset globalPosition = default, Offset? localPosition = null, Offset offsetFromOrigin = default, Offset? localOffsetFromOrigin = null)
    {
        this.__field_globalPosition = globalPosition;
        this.offsetFromOrigin = offsetFromOrigin;
        this.__field_localPosition = (localPosition ?? globalPosition);
        this.localOffsetFromOrigin = (localOffsetFromOrigin ?? offsetFromOrigin);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("offsetFromOrigin", this.offsetFromOrigin));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localOffsetFromOrigin", this.localOffsetFromOrigin));
    }

}

public class LongPressEndDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Velocity velocity { get; private set; } = default!;

    public LongPressEndDetails(Offset globalPosition = default, Offset? localPosition = null, Velocity velocity = default!)
    {
        Velocity __velocity = velocity ?? Velocity.zero;
        this.__field_globalPosition = globalPosition;
        this.velocity = __velocity;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Velocity>("velocity", this.velocity));
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
    public virtual Action<LongPressMoveUpdateDetails>? onLongPressMoveUpdate { get; set; } = default;
    public virtual Action? onLongPressUp { get; set; } = default;
    public virtual Action<LongPressEndDetails>? onLongPressEnd { get; set; } = default;
    public virtual Action<LongPressDownDetails>? onSecondaryLongPressDown { get; set; } = default;
    public virtual Action? onSecondaryLongPressCancel { get; set; } = default;
    public virtual Action? onSecondaryLongPress { get; set; } = default;
    public virtual Action<LongPressStartDetails>? onSecondaryLongPressStart { get; set; } = default;
    public virtual Action<LongPressMoveUpdateDetails>? onSecondaryLongPressMoveUpdate { get; set; } = default;
    public virtual Action? onSecondaryLongPressUp { get; set; } = default;
    public virtual Action<LongPressEndDetails>? onSecondaryLongPressEnd { get; set; } = default;
    public virtual Action<LongPressDownDetails>? onTertiaryLongPressDown { get; set; } = default;
    public virtual Action? onTertiaryLongPressCancel { get; set; } = default;
    public virtual Action? onTertiaryLongPress { get; set; } = default;
    public virtual Action<LongPressStartDetails>? onTertiaryLongPressStart { get; set; } = default;
    public virtual Action<LongPressMoveUpdateDetails>? onTertiaryLongPressMoveUpdate { get; set; } = default;
    public virtual Action? onTertiaryLongPressUp { get; set; } = default;
    public virtual Action<LongPressEndDetails>? onTertiaryLongPressEnd { get; set; } = default;
    internal virtual VelocityTracker? _velocityTracker { get; set; } = default;

    public LongPressGestureRecognizer(Duration? duration = null, double? postAcceptSlopTolerance = null, HashSet<PointerDeviceKind>? supportedDevices = null, object? debugOwner = null, Func<long, bool>? allowedButtonsFilter = null) : base(postAcceptSlopTolerance: postAcceptSlopTolerance, supportedDevices: supportedDevices, debugOwner: debugOwner, deadline: (duration ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kLongPressTimeout), allowedButtonsFilter: (allowedButtonsFilter ?? _defaultButtonAcceptBehavior))
    {
    }

    internal static bool _defaultButtonAcceptBehavior(long buttons) => (((buttons == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton) || (buttons == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton)) || (buttons == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton));
    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        switch (@event.buttons)
        {
            case var __constant25085 when object.Equals(__constant25085, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((((((((this.onLongPressDown is null) && (this.onLongPressCancel is null)) && (this.onLongPressStart is null)) && (this.onLongPress is null)) && (this.onLongPressMoveUpdate is null)) && (this.onLongPressEnd is null)) && (this.onLongPressUp is null)))
                    {
                        return false;
                    }
                    break;
                }
            case var __constant25421 when object.Equals(__constant25421, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((((((((this.onSecondaryLongPressDown is null) && (this.onSecondaryLongPressCancel is null)) && (this.onSecondaryLongPressStart is null)) && (this.onSecondaryLongPress is null)) && (this.onSecondaryLongPressMoveUpdate is null)) && (this.onSecondaryLongPressEnd is null)) && (this.onSecondaryLongPressUp is null)))
                    {
                        return false;
                    }
                    break;
                }
            case var __constant25822 when object.Equals(__constant25822, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((((((((this.onTertiaryLongPressDown is null) && (this.onTertiaryLongPressCancel is null)) && (this.onTertiaryLongPressStart is null)) && (this.onTertiaryLongPress is null)) && (this.onTertiaryLongPressMoveUpdate is null)) && (this.onTertiaryLongPressEnd is null)) && (this.onTertiaryLongPressUp is null)))
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didExceedDeadline()
    {
        resolve(GestureDisposition.accepted);
        _longPressAccepted = true;
        base.acceptGesture(DartRuntimePrimitives.RequireValue(primaryPointer));
        _checkLongPressStart();
    }

    public override void handlePrimaryPointer(PointerEvent @event)
    {
        if (!((PointerEvent)@event).synthesized)
        {
            if ((@event is PointerDownEvent))
            {
                PointerDownEvent @event__as26655 = (PointerDownEvent)@event;
                _velocityTracker = new VelocityTracker(((PointerDownEvent)@event__as26655).kind);
                this._velocityTracker!.addPosition(((PointerDownEvent)@event__as26655).timeStamp, ((PointerDownEvent)@event__as26655).localPosition);
            }
            if ((@event is PointerMoveEvent))
            {
                PointerMoveEvent @event__as26844 = (PointerMoveEvent)@event;
                DartRuntimePrimitives.Assert(() => (this._velocityTracker is not null));
                this._velocityTracker!.addPosition(((PointerMoveEvent)@event__as26844).timeStamp, ((PointerMoveEvent)@event__as26844).localPosition);
            }
        }
        if ((@event is PointerUpEvent))
        {
            PointerUpEvent @event__as27015 = (PointerUpEvent)@event;
            if (this._longPressAccepted)
            {
                _checkLongPressEnd(((PointerUpEvent)@event__as27015));
            }
            else
            {
                resolve(GestureDisposition.rejected);
            }
            _reset();
        }
        else
        {
            if ((@event is PointerCancelEvent))
            {
                PointerCancelEvent @event__as27254 = (PointerCancelEvent)@event;
                _checkLongPressCancel();
                _reset();
            }
            else
            {
                if ((@event is PointerDownEvent))
                {
                    PointerDownEvent @event__as27347 = (PointerDownEvent)@event;
                    _longPressOrigin = OffsetPair.CreateFromEventPosition(((PointerDownEvent)@event__as27347));
                    _initialButtons = ((PointerDownEvent)@event__as27347).buttons;
                    _checkLongPressDown(((PointerDownEvent)@event__as27347));
                }
                else
                {
                    if ((@event is PointerMoveEvent))
                    {
                        PointerMoveEvent @event__as27552 = (PointerMoveEvent)@event;
                        if (((((PointerMoveEvent)@event__as27552).buttons != this._initialButtons) && !this._longPressAccepted))
                        {
                            resolve(GestureDisposition.rejected);
                            stopTrackingPointer(DartRuntimePrimitives.RequireValue(primaryPointer));
                        }
                        else
                        {
                            if (this._longPressAccepted)
                            {
                                _checkLongPressMoveUpdate(((PointerMoveEvent)@event__as27552));
                            }
                        }
                    }
                }
            }
        }
    }

    internal virtual void _checkLongPressDown(PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._longPressOrigin is not null));
        var details__27943 = new LongPressDownDetails(globalPosition: this._longPressOrigin!.global, localPosition: this._longPressOrigin!.local, kind: getKindForPointer(@event.pointer));
        switch (this._initialButtons)
        {
            case var __constant28164 when object.Equals(__constant28164, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onLongPressDown is not null))
                    {
                        invokeCallback<object?>("onLongPressDown", () => { ((Action)((() => this.onLongPressDown!(details__27943))))(); return null; });
                    }
                    break;
                }
            case var __constant28324 when object.Equals(__constant28324, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryLongPressDown is not null))
                    {
                        invokeCallback<object?>("onSecondaryLongPressDown", () => { ((Action)((() => this.onSecondaryLongPressDown!(details__27943))))(); return null; });
                    }
                    break;
                }
            case var __constant28550 when object.Equals(__constant28550, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryLongPressDown is not null))
                    {
                        invokeCallback<object?>("onTertiaryLongPressDown", () => { ((Action)((() => this.onTertiaryLongPressDown!(details__27943))))(); return null; });
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
        if ((object.Equals(state, GestureRecognizerState.possible)))
        {
            switch (this._initialButtons)
            {
                case var __constant28941 when object.Equals(__constant28941, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                    {
                        if ((this.onLongPressCancel is not null))
                        {
                            invokeCallback<object?>("onLongPressCancel", () => { ((Action)(this.onLongPressCancel!))(); return null; });
                        }
                        break;
                    }
                case var __constant29100 when object.Equals(__constant29100, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton):
                    {
                        if ((this.onSecondaryLongPressCancel is not null))
                        {
                            invokeCallback<object?>("onSecondaryLongPressCancel", () => { ((Action)(this.onSecondaryLongPressCancel!))(); return null; });
                        }
                        break;
                    }
                case var __constant29288 when object.Equals(__constant29288, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton):
                    {
                        if ((this.onTertiaryLongPressCancel is not null))
                        {
                            invokeCallback<object?>("onTertiaryLongPressCancel", () => { ((Action)(this.onTertiaryLongPressCancel!))(); return null; });
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
        switch (this._initialButtons)
        {
            case var __constant29631 when object.Equals(__constant29631, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onLongPressStart is not null))
                    {
                        var details__29703 = new LongPressStartDetails(globalPosition: this._longPressOrigin!.global, localPosition: this._longPressOrigin!.local);
                        invokeCallback<object?>("onLongPressStart", () => { ((Action)((() => this.onLongPressStart!(details__29703))))(); return null; });
                    }
                    if ((this.onLongPress is not null))
                    {
                        invokeCallback<object?>("onLongPress", () => { ((Action)(this.onLongPress!))(); return null; });
                    }
                    break;
                }
            case var __constant30068 when object.Equals(__constant30068, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryLongPressStart is not null))
                    {
                        var details__30151 = new LongPressStartDetails(globalPosition: this._longPressOrigin!.global, localPosition: this._longPressOrigin!.local);
                        invokeCallback<object?>("onSecondaryLongPressStart", () => { ((Action)((() => this.onSecondaryLongPressStart!(details__30151))))(); return null; });
                    }
                    if ((this.onSecondaryLongPress is not null))
                    {
                        invokeCallback<object?>("onSecondaryLongPress", () => { ((Action)(this.onSecondaryLongPress!))(); return null; });
                    }
                    break;
                }
            case var __constant30598 when object.Equals(__constant30598, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryLongPressStart is not null))
                    {
                        var details__30679 = new LongPressStartDetails(globalPosition: this._longPressOrigin!.global, localPosition: this._longPressOrigin!.local);
                        invokeCallback<object?>("onTertiaryLongPressStart", () => { ((Action)((() => this.onTertiaryLongPressStart!(details__30679))))(); return null; });
                    }
                    if ((this.onTertiaryLongPress is not null))
                    {
                        invokeCallback<object?>("onTertiaryLongPress", () => { ((Action)(this.onTertiaryLongPress!))(); return null; });
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
        var details__31261 = new LongPressMoveUpdateDetails(globalPosition: ((PointerEvent)@event).position, localPosition: ((PointerEvent)@event).localPosition, offsetFromOrigin: (((PointerEvent)@event).position - this._longPressOrigin!.global), localOffsetFromOrigin: (((PointerEvent)@event).localPosition - this._longPressOrigin!.local));
        switch (this._initialButtons)
        {
            case var __constant31571 when object.Equals(__constant31571, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onLongPressMoveUpdate is not null))
                    {
                        invokeCallback<object?>("onLongPressMoveUpdate", () => { ((Action)((() => this.onLongPressMoveUpdate!(details__31261))))(); return null; });
                    }
                    break;
                }
            case var __constant31749 when object.Equals(__constant31749, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryLongPressMoveUpdate is not null))
                    {
                        invokeCallback<object?>("onSecondaryLongPressMoveUpdate", () => { ((Action)((() => this.onSecondaryLongPressMoveUpdate!(details__31261))))(); return null; });
                    }
                    break;
                }
            case var __constant31993 when object.Equals(__constant31993, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryLongPressMoveUpdate is not null))
                    {
                        invokeCallback<object?>("onTertiaryLongPressMoveUpdate", () => { ((Action)((() => this.onTertiaryLongPressMoveUpdate!(details__31261))))(); return null; });
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
        VelocityEstimate? estimate__32384 = this._velocityTracker!.getVelocityEstimate();
        Velocity velocity__32455 = ((estimate__32384 is null) ? Velocity.zero : new Velocity(pixelsPerSecond: ((VelocityEstimate)estimate__32384).pixelsPerSecond));
        var details__32580 = new LongPressEndDetails(globalPosition: ((PointerEvent)@event).position, localPosition: ((PointerEvent)@event).localPosition, velocity: velocity__32455);
        _velocityTracker = null;
        switch (this._initialButtons)
        {
            case var __constant32796 when object.Equals(__constant32796, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onLongPressEnd is not null))
                    {
                        invokeCallback<object?>("onLongPressEnd", () => { ((Action)((() => this.onLongPressEnd!(details__32580))))(); return null; });
                    }
                    if ((this.onLongPressUp is not null))
                    {
                        invokeCallback<object?>("onLongPressUp", () => { ((Action)(this.onLongPressUp!))(); return null; });
                    }
                    break;
                }
            case var __constant33065 when object.Equals(__constant33065, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryLongPressEnd is not null))
                    {
                        invokeCallback<object?>("onSecondaryLongPressEnd", () => { ((Action)((() => this.onSecondaryLongPressEnd!(details__32580))))(); return null; });
                    }
                    if ((this.onSecondaryLongPressUp is not null))
                    {
                        invokeCallback<object?>("onSecondaryLongPressUp", () => { ((Action)(this.onSecondaryLongPressUp!))(); return null; });
                    }
                    break;
                }
            case var __constant33390 when object.Equals(__constant33390, global::Doroti.Generated.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryLongPressEnd is not null))
                    {
                        invokeCallback<object?>("onTertiaryLongPressEnd", () => { ((Action)((() => this.onTertiaryLongPressEnd!(details__32580))))(); return null; });
                    }
                    if ((this.onTertiaryLongPressUp is not null))
                    {
                        invokeCallback<object?>("onTertiaryLongPressUp", () => { ((Action)(this.onTertiaryLongPressUp!))(); return null; });
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
        if ((object.Equals(disposition, GestureDisposition.rejected)))
        {
            if (this._longPressAccepted)
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

    public override void acceptGesture(long pointer)
    {
    }

    public override string debugDescription => "long press";
}

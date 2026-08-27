// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/tap.dart
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

public class TapDownDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind? kind { get; private set; }

    public TapDownDetails(Offset globalPosition = default, Offset? localPosition = null, PointerDeviceKind? kind = null)
    {
        this.__field_globalPosition = globalPosition;
        this.kind = kind;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
    }

}

public delegate void GestureTapDownCallback(TapDownDetails details);

public class TapUpDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind kind { get; private set; } = default!;

    public TapUpDetails(Offset globalPosition = default, Offset? localPosition = null, PointerDeviceKind kind = default!)
    {
        this.__field_globalPosition = globalPosition;
        this.kind = kind;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
    }

}

public class TapMoveDetails
{
    public virtual Offset globalPosition { get; private set; } = default!;
    public virtual Offset localPosition { get; private set; } = default!;
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual Offset delta { get; private set; } = default!;

    public TapMoveDetails(PointerDeviceKind kind, Offset globalPosition = default, Offset delta = default, Offset? localPosition = null)
    {
        this.kind = kind;
        this.globalPosition = globalPosition;
        this.delta = delta;
        this.localPosition = (localPosition ?? globalPosition);
    }

}

public delegate void GestureTapUpCallback(TapUpDetails details);

public delegate void GestureTapCallback();

public delegate void GestureTapMoveCallback(TapMoveDetails details);

public delegate void GestureTapCancelCallback();

public abstract class BaseTapGestureRecognizer : PrimaryPointerGestureRecognizer
{
    internal virtual bool _sentTapDown { get; set; } = false;
    internal virtual bool _wonArenaForPrimaryPointer { get; set; } = false;
    internal virtual PointerDownEvent? _down { get; set; } = default;
    internal virtual PointerUpEvent? _up { get; set; } = default;

    protected BaseTapGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, double? preAcceptSlopTolerance = null, double? postAcceptSlopTolerance = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior, preAcceptSlopTolerance: preAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop, postAcceptSlopTolerance: postAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop, deadline: global::Doroti.Framework.Gestures.ConstantsLibrary.kPressTimeout)
    {
    }

    public abstract void handleTapDown(PointerDownEvent down);
    public abstract void handleTapUp(PointerDownEvent down, PointerUpEvent up);
    public virtual void handleTapMove(PointerMoveEvent move)
    {
    }

    public abstract void handleTapCancel(PointerDownEvent down, PointerCancelEvent? cancel = null, string reason = default!);
    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if ((object.Equals(state, GestureRecognizerState.ready)))
        {
            if (((this._down is not null) && (this._up is not null)))
            {
                DartRuntimePrimitives.Assert(() => (this._down!.pointer == this._up!.pointer));
                _reset();
            }
            DartRuntimePrimitives.Assert(() => ((this._down is null) && (this._up is null)));
            _down = @event;
        }
        if ((this._down is not null))
        {
            base.addAllowedPointer(@event);
        }
    }

    public override void startTrackingPointer(long pointer, Matrix4? transform = null)
    {
        DartRuntimePrimitives.Assert(() => (this._down is not null));
        base.startTrackingPointer(pointer, transform);
    }

    public override void handlePrimaryPointer(PointerEvent @event)
    {
        if ((@event is PointerUpEvent))
        {
            PointerUpEvent @event__as11925 = (PointerUpEvent)@event;
            _up = ((PointerUpEvent)@event__as11925);
            _checkUp();
        }
        else
        {
            if ((@event is PointerCancelEvent))
            {
                PointerCancelEvent @event__as12004 = (PointerCancelEvent)@event;
                resolve(GestureDisposition.rejected);
                if (this._sentTapDown)
                {
                    _checkCancel(((PointerCancelEvent)@event__as12004), "");
                }
                _reset();
            }
            else
            {
                if ((((PointerEvent)@event).buttons != this._down!.buttons))
                {
                    resolve(GestureDisposition.rejected);
                    stopTrackingPointer(DartRuntimePrimitives.RequireValue(primaryPointer));
                }
                else
                {
                    if ((@event is PointerMoveEvent))
                    {
                        PointerMoveEvent @event__as12315 = (PointerMoveEvent)@event;
                        _checkMove(((PointerMoveEvent)@event__as12315));
                    }
                }
            }
        }
    }

    public override void resolve(GestureDisposition disposition)
    {
        if ((this._wonArenaForPrimaryPointer && (object.Equals(disposition, GestureDisposition.rejected))))
        {
            DartRuntimePrimitives.Assert(() => this._sentTapDown);
            _checkCancel(null, "spontaneous");
            _reset();
        }
        base.resolve(disposition);
    }

    public override void didExceedDeadline()
    {
        _checkDown();
    }

    public override void acceptGesture(long pointer)
    {
        base.acceptGesture(pointer);
        if ((pointer == primaryPointer))
        {
            _checkDown();
            _wonArenaForPrimaryPointer = true;
            _checkUp();
        }
    }

    public override void rejectGesture(long pointer)
    {
        base.rejectGesture(pointer);
        if ((pointer == primaryPointer))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(state, GestureRecognizerState.possible)));
            if (this._sentTapDown)
            {
                _checkCancel(null, "forced");
            }
            _reset();
        }
    }

    internal virtual void _checkDown()
    {
        if (this._sentTapDown)
        {
            return;
        }
        handleTapDown(down: this._down!);
        _sentTapDown = true;
    }

    internal virtual void _checkUp()
    {
        if ((!this._wonArenaForPrimaryPointer || (this._up is null)))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (this._up!.pointer == this._down!.pointer));
        handleTapUp(down: this._down!, up: this._up!);
        _reset();
    }

    internal virtual void _checkCancel(PointerCancelEvent? @event, string note)
    {
        handleTapCancel(down: this._down!, cancel: @event, reason: note);
    }

    internal virtual void _checkMove(PointerMoveEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (@event.pointer == this._down!.pointer));
        handleTapMove(move: @event);
    }

    internal virtual void _reset()
    {
        _sentTapDown = false;
        _wonArenaForPrimaryPointer = false;
        _up = null;
        _down = null;
    }

    public override string debugDescription => "base tap";
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("wonArenaForPrimaryPointer", value: this._wonArenaForPrimaryPointer, ifTrue: "won arena"));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("finalPosition", this._up?.position, defaultValue: null));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("finalLocalPosition", this._up?.localPosition, defaultValue: this._up?.position));
        properties.add(new IntProperty("button", this._down?.buttons, defaultValue: null));
        properties.add(new FlagProperty("sentTapDown", value: this._sentTapDown, ifTrue: "sent tap down"));
    }

}

public class TapGestureRecognizer : BaseTapGestureRecognizer
{
    public virtual Action<TapDownDetails>? onTapDown { get; set; } = default;
    public virtual Action<TapUpDetails>? onTapUp { get; set; } = default;
    public virtual Action? onTap { get; set; } = default;
    public virtual Action<TapMoveDetails>? onTapMove { get; set; } = default;
    public virtual Action? onTapCancel { get; set; } = default;
    public virtual Action? onSecondaryTap { get; set; } = default;
    public virtual Action<TapDownDetails>? onSecondaryTapDown { get; set; } = default;
    public virtual Action<TapUpDetails>? onSecondaryTapUp { get; set; } = default;
    public virtual Action? onSecondaryTapCancel { get; set; } = default;
    public virtual Action<TapDownDetails>? onTertiaryTapDown { get; set; } = default;
    public virtual Action<TapUpDetails>? onTertiaryTapUp { get; set; } = default;
    public virtual Action? onTertiaryTapCancel { get; set; } = default;

    public TapGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, double? preAcceptSlopTolerance = null, double? postAcceptSlopTolerance = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior, preAcceptSlopTolerance: preAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop, postAcceptSlopTolerance: postAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop)
    {
    }

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        switch (@event.buttons)
        {
            case var __constant26676 when object.Equals(__constant26676, global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((((((this.onTapDown is null) && (this.onTap is null)) && (this.onTapUp is null)) && (this.onTapCancel is null)) && (this.onTapMove is null)))
                    {
                        return false;
                    }
                    break;
                }
            case var __constant26898 when object.Equals(__constant26898, global::Doroti.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if (((((this.onSecondaryTap is null) && (this.onSecondaryTapDown is null)) && (this.onSecondaryTapUp is null)) && (this.onSecondaryTapCancel is null)))
                    {
                        return false;
                    }
                    break;
                }
            case var __constant27125 when object.Equals(__constant27125, global::Doroti.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((((this.onTertiaryTapDown is null) && (this.onTertiaryTapUp is null)) && (this.onTertiaryTapCancel is null)))
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

    public override void handleTapDown(PointerDownEvent down)
    {
        var details = new TapDownDetails(globalPosition: down.position, localPosition: down.localPosition, kind: getKindForPointer(down.pointer));
        switch (down.buttons)
        {
            case var __constant27652 when object.Equals(__constant27652, global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onTapDown is not null))
                    {
                        invokeCallback<object?>("onTapDown", () => { ((Action)((() => this.onTapDown!(details))))(); return null; });
                    }
                    break;
                }
            case var __constant27794 when object.Equals(__constant27794, global::Doroti.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryTapDown is not null))
                    {
                        invokeCallback<object?>("onSecondaryTapDown", () => { ((Action)((() => this.onSecondaryTapDown!(details))))(); return null; });
                    }
                    break;
                }
            case var __constant27965 when object.Equals(__constant27965, global::Doroti.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryTapDown is not null))
                    {
                        invokeCallback<object?>("onTertiaryTapDown", () => { ((Action)((() => this.onTertiaryTapDown!(details))))(); return null; });
                    }
                    break;
                }
            default:
                break;
        }
    }

    public override void handleTapUp(PointerDownEvent down, PointerUpEvent up)
    {
        var details = new TapUpDetails(kind: up.kind, globalPosition: up.position, localPosition: up.localPosition);
        switch (down.buttons)
        {
            case var __constant28430 when object.Equals(__constant28430, global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onTapUp is not null))
                    {
                        invokeCallback<object?>("onTapUp", () => { ((Action)((() => this.onTapUp!(details))))(); return null; });
                    }
                    if ((this.onTap is not null))
                    {
                        invokeCallback<object?>("onTap", () => { ((Action)(this.onTap!))(); return null; });
                    }
                    break;
                }
            case var __constant28654 when object.Equals(__constant28654, global::Doroti.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryTapUp is not null))
                    {
                        invokeCallback<object?>("onSecondaryTapUp", () => { ((Action)((() => this.onSecondaryTapUp!(details))))(); return null; });
                    }
                    if ((this.onSecondaryTap is not null))
                    {
                        invokeCallback<object?>("onSecondaryTap", () => { ((Action)((() => this.onSecondaryTap!())))(); return null; });
                    }
                    break;
                }
            case var __constant28942 when object.Equals(__constant28942, global::Doroti.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryTapUp is not null))
                    {
                        invokeCallback<object?>("onTertiaryTapUp", () => { ((Action)((() => this.onTertiaryTapUp!(details))))(); return null; });
                    }
                    break;
                }
            default:
                break;
        }
    }

    public override void handleTapMove(PointerMoveEvent move)
    {
        if (((this.onTapMove is not null) && (move.buttons == global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton)))
        {
            var details = new TapMoveDetails(globalPosition: move.position, localPosition: move.localPosition, kind: getKindForPointer(move.pointer), delta: move.delta);
            invokeCallback<object?>("onTapMove", () => { ((Action)((() => this.onTapMove!(details))))(); return null; });
        }
    }

    public override void handleTapCancel(PointerDownEvent down, PointerCancelEvent? cancel = null, string reason = default!)
    {
        var note = ((reason == "") ? reason : $"{reason} ");
        switch (down.buttons)
        {
            case var __constant29790 when object.Equals(__constant29790, global::Doroti.Framework.Gestures.EventsLibrary.kPrimaryButton):
                {
                    if ((this.onTapCancel is not null))
                    {
                        invokeCallback<object?>($"{note}onTapCancel", () => { ((Action)(this.onTapCancel!))(); return null; });
                    }
                    break;
                }
            case var __constant29930 when object.Equals(__constant29930, global::Doroti.Framework.Gestures.EventsLibrary.kSecondaryButton):
                {
                    if ((this.onSecondaryTapCancel is not null))
                    {
                        invokeCallback<object?>($"{note}onSecondaryTapCancel", () => { ((Action)(this.onSecondaryTapCancel!))(); return null; });
                    }
                    break;
                }
            case var __constant30099 when object.Equals(__constant30099, global::Doroti.Framework.Gestures.EventsLibrary.kTertiaryButton):
                {
                    if ((this.onTertiaryTapCancel is not null))
                    {
                        invokeCallback<object?>($"{note}onTertiaryTapCancel", () => { ((Action)(this.onTertiaryTapCancel!))(); return null; });
                    }
                    break;
                }
            default:
                break;
        }
    }

    public override string debugDescription => "tap";
}


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/tap.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        __field_globalPosition = globalPosition;
        this.kind = kind;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind));
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
        __field_globalPosition = globalPosition;
        this.kind = kind;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind));
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
        this.localPosition = localPosition ?? globalPosition;
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

    protected BaseTapGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, double? preAcceptSlopTolerance = null, double? postAcceptSlopTolerance = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior, preAcceptSlopTolerance: preAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop, postAcceptSlopTolerance: postAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop, deadline: ConstantsLibrary.kPressTimeout)
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
        if (Equals(state, GestureRecognizerState.ready))
        {
            if ((_down is not null) && (_up is not null))
            {
                DartRuntimePrimitives.Assert(() => _down!.pointer == _up!.pointer);
                _reset();
            }
            DartRuntimePrimitives.Assert(() => (_down is null) && (_up is null));
            _down = @event;
        }
        if (_down is not null)
        {
            base.addAllowedPointer(@event);
        }
    }

    public override void startTrackingPointer(long pointer, Matrix4? transform = null)
    {
        DartRuntimePrimitives.Assert(() => _down is not null);
        base.startTrackingPointer(pointer, transform);
    }

    public override void handlePrimaryPointer(PointerEvent @event)
    {
        if (@event is PointerUpEvent)
        {
            PointerUpEvent @event__as11925 = (PointerUpEvent)@event;
            _up = @event__as11925;
            _checkUp();
        }
        else
        {
            if (@event is PointerCancelEvent)
            {
                PointerCancelEvent @event__as12004 = (PointerCancelEvent)@event;
                resolve(GestureDisposition.rejected);
                if (_sentTapDown)
                {
                    _checkCancel(@event__as12004, "");
                }
                _reset();
            }
            else
            {
                if (@event.buttons != _down!.buttons)
                {
                    resolve(GestureDisposition.rejected);
                    stopTrackingPointer(DartRuntimePrimitives.RequireValue(primaryPointer));
                }
                else
                {
                    if (@event is PointerMoveEvent)
                    {
                        PointerMoveEvent @event__as12315 = (PointerMoveEvent)@event;
                        _checkMove(@event__as12315);
                    }
                }
            }
        }
    }

    public override void resolve(GestureDisposition disposition)
    {
        if (_wonArenaForPrimaryPointer && Equals(disposition, GestureDisposition.rejected))
        {
            DartRuntimePrimitives.Assert(() => _sentTapDown);
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
        if (pointer == primaryPointer)
        {
            _checkDown();
            _wonArenaForPrimaryPointer = true;
            _checkUp();
        }
    }

    public override void rejectGesture(long pointer)
    {
        base.rejectGesture(pointer);
        if (pointer == primaryPointer)
        {
            DartRuntimePrimitives.Assert(() => !Equals(state, GestureRecognizerState.possible));
            if (_sentTapDown)
            {
                _checkCancel(null, "forced");
            }
            _reset();
        }
    }

    internal virtual void _checkDown()
    {
        if (_sentTapDown)
        {
            return;
        }
        handleTapDown(down: _down!);
        _sentTapDown = true;
    }

    internal virtual void _checkUp()
    {
        if (!_wonArenaForPrimaryPointer || (_up is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => _up!.pointer == _down!.pointer);
        handleTapUp(down: _down!, up: _up!);
        _reset();
    }

    internal virtual void _checkCancel(PointerCancelEvent? @event, string note)
    {
        handleTapCancel(down: _down!, cancel: @event, reason: note);
    }

    internal virtual void _checkMove(PointerMoveEvent @event)
    {
        DartRuntimePrimitives.Assert(() => @event.pointer == _down!.pointer);
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
        properties.add(new FlagProperty("wonArenaForPrimaryPointer", value: _wonArenaForPrimaryPointer, ifTrue: "won arena"));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("finalPosition", _up?.position, defaultValue: null));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("finalLocalPosition", _up?.localPosition, defaultValue: _up?.position));
        properties.add(new IntProperty("button", _down?.buttons, defaultValue: null));
        properties.add(new FlagProperty("sentTapDown", value: _sentTapDown, ifTrue: "sent tap down"));
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

    public TapGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, double? preAcceptSlopTolerance = null, double? postAcceptSlopTolerance = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior, preAcceptSlopTolerance: preAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop, postAcceptSlopTolerance: postAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop)
    {
    }

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        switch (@event.buttons)
        {
            case var __constant26676 when Equals(__constant26676, EventsLibrary.kPrimaryButton):
                {
                    if ((onTapDown is null) && (onTap is null) && (onTapUp is null) && (onTapCancel is null) && (onTapMove is null))
                    {
                        return false;
                    }
                    break;
                }
            case var __constant26898 when Equals(__constant26898, EventsLibrary.kSecondaryButton):
                {
                    if ((onSecondaryTap is null) && (onSecondaryTapDown is null) && (onSecondaryTapUp is null) && (onSecondaryTapCancel is null))
                    {
                        return false;
                    }
                    break;
                }
            case var __constant27125 when Equals(__constant27125, EventsLibrary.kTertiaryButton):
                {
                    if ((onTertiaryTapDown is null) && (onTertiaryTapUp is null) && (onTertiaryTapCancel is null))
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
            case var __constant27652 when Equals(__constant27652, EventsLibrary.kPrimaryButton):
                {
                    if (onTapDown is not null)
                    {
                        invokeCallback<object?>("onTapDown", () => { ((Action)(() => onTapDown!(details)))(); return null; });
                    }
                    break;
                }
            case var __constant27794 when Equals(__constant27794, EventsLibrary.kSecondaryButton):
                {
                    if (onSecondaryTapDown is not null)
                    {
                        invokeCallback<object?>("onSecondaryTapDown", () => { ((Action)(() => onSecondaryTapDown!(details)))(); return null; });
                    }
                    break;
                }
            case var __constant27965 when Equals(__constant27965, EventsLibrary.kTertiaryButton):
                {
                    if (onTertiaryTapDown is not null)
                    {
                        invokeCallback<object?>("onTertiaryTapDown", () => { ((Action)(() => onTertiaryTapDown!(details)))(); return null; });
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
            case var __constant28430 when Equals(__constant28430, EventsLibrary.kPrimaryButton):
                {
                    if (onTapUp is not null)
                    {
                        invokeCallback<object?>("onTapUp", () => { ((Action)(() => onTapUp!(details)))(); return null; });
                    }
                    if (onTap is not null)
                    {
                        invokeCallback<object?>("onTap", () => { onTap!(); return null; });
                    }
                    break;
                }
            case var __constant28654 when Equals(__constant28654, EventsLibrary.kSecondaryButton):
                {
                    if (onSecondaryTapUp is not null)
                    {
                        invokeCallback<object?>("onSecondaryTapUp", () => { ((Action)(() => onSecondaryTapUp!(details)))(); return null; });
                    }
                    if (onSecondaryTap is not null)
                    {
                        invokeCallback<object?>("onSecondaryTap", () => { ((Action)(() => onSecondaryTap!()))(); return null; });
                    }
                    break;
                }
            case var __constant28942 when Equals(__constant28942, EventsLibrary.kTertiaryButton):
                {
                    if (onTertiaryTapUp is not null)
                    {
                        invokeCallback<object?>("onTertiaryTapUp", () => { ((Action)(() => onTertiaryTapUp!(details)))(); return null; });
                    }
                    break;
                }
            default:
                break;
        }
    }

    public override void handleTapMove(PointerMoveEvent move)
    {
        if ((onTapMove is not null) && (move.buttons == EventsLibrary.kPrimaryButton))
        {
            var details = new TapMoveDetails(globalPosition: move.position, localPosition: move.localPosition, kind: getKindForPointer(move.pointer), delta: move.delta);
            invokeCallback<object?>("onTapMove", () => { ((Action)(() => onTapMove!(details)))(); return null; });
        }
    }

    public override void handleTapCancel(PointerDownEvent down, PointerCancelEvent? cancel = null, string reason = default!)
    {
        var note = (reason == "") ? reason : $"{reason} ";
        switch (down.buttons)
        {
            case var __constant29790 when Equals(__constant29790, EventsLibrary.kPrimaryButton):
                {
                    if (onTapCancel is not null)
                    {
                        invokeCallback<object?>($"{note}onTapCancel", () => { onTapCancel!(); return null; });
                    }
                    break;
                }
            case var __constant29930 when Equals(__constant29930, EventsLibrary.kSecondaryButton):
                {
                    if (onSecondaryTapCancel is not null)
                    {
                        invokeCallback<object?>($"{note}onSecondaryTapCancel", () => { onSecondaryTapCancel!(); return null; });
                    }
                    break;
                }
            case var __constant30099 when Equals(__constant30099, EventsLibrary.kTertiaryButton):
                {
                    if (onTertiaryTapCancel is not null)
                    {
                        invokeCallback<object?>($"{note}onTertiaryTapCancel", () => { onTertiaryTapCancel!(); return null; });
                    }
                    break;
                }
            default:
                break;
        }
    }

    public override string debugDescription => "tap";
}


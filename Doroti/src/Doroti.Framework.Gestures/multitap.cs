// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/multitap.dart
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

namespace Doroti.Generated.Framework.Gestures;

public delegate void GestureDoubleTapCallback();

public delegate void GestureMultiTapDownCallback(long pointer, TapDownDetails details);

public delegate void GestureMultiTapUpCallback(long pointer, TapUpDetails details);

public delegate void GestureMultiTapCallback(long pointer);

public delegate void GestureMultiTapCancelCallback(long pointer);

internal class _CountdownZoned__multitap
{
    internal virtual bool _timeout { get; set; } = false;

    internal _CountdownZoned__multitap(Duration duration)
    {
    }

    public virtual bool timeout => this._timeout;
    internal virtual void _onTimeout()
    {
        _timeout = true;
    }

}

internal class _TapTracker__multitap
{
    public virtual DeviceGestureSettings? gestureSettings { get; private set; }
    public virtual long pointer { get; private set; } = default!;
    public virtual GestureArenaEntry entry { get; private set; } = default!;
    internal virtual Offset _initialGlobalPosition { get; private set; } = default!;
    public virtual long initialButtons { get; private set; } = default!;
    internal virtual _CountdownZoned__multitap _doubleTapMinTimeCountdown { get; private set; } = default!;
    internal virtual bool _isTrackingPointer { get; set; } = false;

    internal _TapTracker__multitap(PointerDownEvent @event, GestureArenaEntry entry, Duration doubleTapMinTime, DeviceGestureSettings? gestureSettings)
    {
        this.entry = entry;
        this.gestureSettings = gestureSettings;
        this.pointer = @event.pointer;
        this._initialGlobalPosition = @event.position;
        this.initialButtons = @event.buttons;
        this._doubleTapMinTimeCountdown = new _CountdownZoned__multitap(duration: doubleTapMinTime);
    }

    public virtual void startTrackingPointer(Action<PointerEvent> route, Matrix4? transform)
    {
        if (!this._isTrackingPointer)
        {
            _isTrackingPointer = true;
            GestureBinding.instance.pointerRouter.addRoute(this.pointer, (Action<PointerEvent>)route, transform);
        }
    }

    public virtual void stopTrackingPointer(Action<PointerEvent> route)
    {
        if (this._isTrackingPointer)
        {
            _isTrackingPointer = false;
            GestureBinding.instance.pointerRouter.removeRoute(this.pointer, (Action<PointerEvent>)route);
        }
    }

    public virtual bool isWithinGlobalTolerance(PointerEvent @event, double tolerance)
    {
        global::Doroti.Ui.Offset offset__3321 = (((PointerEvent)@event).position - this._initialGlobalPosition);
        return (offset__3321.distance <= tolerance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hasElapsedMinTime()
    {
        return ((_CountdownZoned__multitap)this._doubleTapMinTimeCountdown).timeout;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hasSameButton(PointerDownEvent @event)
    {
        return (@event.buttons == this.initialButtons);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DoubleTapGestureRecognizer : GestureRecognizer
{
    public virtual Action<TapDownDetails>? onDoubleTapDown { get; set; } = default;
    public virtual Action? onDoubleTap { get; set; } = default;
    public virtual Action? onDoubleTapCancel { get; set; } = default;
    internal virtual Timer? _doubleTapTimer { get; set; } = default;
    internal virtual _TapTracker__multitap? _firstTap { get; set; } = default;
    internal virtual DartMap<long, _TapTracker__multitap> _trackers { get; private set; } = new DartMap<long, _TapTracker__multitap>();

    public DoubleTapGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    internal static bool _defaultButtonAcceptBehavior(long buttons) => (buttons == global::Doroti.Generated.Framework.Gestures.EventsLibrary.kPrimaryButton);
    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if ((this._firstTap is null))
        {
            if ((((this.onDoubleTapDown is null) && (this.onDoubleTap is null)) && (this.onDoubleTapCancel is null)))
            {
                return false;
            }
        }
        bool isPointerAllowed__7673 = base.isPointerAllowed(@event);
        if (!isPointerAllowed__7673)
        {
            _reset();
        }
        return isPointerAllowed__7673;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if ((this._firstTap is not null))
        {
            if (!this._firstTap!.isWithinGlobalTolerance(@event, global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapSlop))
            {
                return;
            }
            else
            {
                if ((!this._firstTap!.hasElapsedMinTime() || !this._firstTap!.hasSameButton(@event)))
                {
                    _reset();
                    _trackTap(@event);
                    return;
                }
                else
                {
                    if ((this.onDoubleTapDown is not null))
                    {
                        var details__8385 = new TapDownDetails(globalPosition: @event.position, localPosition: @event.localPosition, kind: getKindForPointer(@event.pointer));
                        invokeCallback<object?>("onDoubleTapDown", () => { ((Action)((() => this.onDoubleTapDown!(details__8385))))(); return null; });
                    }
                }
            }
        }
        _trackTap(@event);
    }

    internal virtual void _trackTap(PointerDownEvent @event)
    {
        _stopDoubleTapTimer();
        var tracker__8763 = new _TapTracker__multitap(@event: @event, entry: GestureBinding.instance.gestureArena.add(@event.pointer, this), doubleTapMinTime: global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapMinTime, gestureSettings: gestureSettings);
        this._trackers[@event.pointer] = tracker__8763;
        tracker__8763.startTrackingPointer((Action<PointerEvent>)this._handleEvent, @event.transform);
    }

    internal virtual void _handleEvent(PointerEvent @event)
    {
        _TapTracker__multitap tracker__9146 = this._trackers.GetValueOrDefault(((PointerEvent)@event).pointer)!;
        if ((@event is PointerUpEvent))
        {
            PointerUpEvent @event__as9191 = (PointerUpEvent)@event;
            if ((this._firstTap is null))
            {
                _registerFirstTap(tracker__9146);
            }
            else
            {
                _registerSecondTap(tracker__9146);
            }
        }
        else
        {
            if ((@event is PointerMoveEvent))
            {
                PointerMoveEvent @event__as9360 = (PointerMoveEvent)@event;
                if (!tracker__9146.isWithinGlobalTolerance(((PointerMoveEvent)@event__as9360), global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapTouchSlop))
                {
                    _reject(tracker__9146);
                }
            }
            else
            {
                if ((@event is PointerCancelEvent))
                {
                    PointerCancelEvent @event__as9512 = (PointerCancelEvent)@event;
                    _reject(tracker__9146);
                }
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
    }

    public override void rejectGesture(long pointer)
    {
        _TapTracker__multitap? tracker__9693 = this._trackers.GetValueOrDefault(pointer);
        if ((((tracker__9693 is null) && (this._firstTap is not null)) && (this._firstTap!.pointer == pointer)))
        {
            tracker__9693 = this._firstTap;
        }
        if ((tracker__9693 is not null))
        {
            _reject(tracker__9693);
        }
    }

    internal virtual void _reject(_TapTracker__multitap tracker)
    {
        this._trackers.remove(((_TapTracker__multitap)tracker).pointer);
        ((_TapTracker__multitap)tracker).entry.resolve(GestureDisposition.rejected);
        _freezeTracker(tracker);
        if ((this._firstTap is not null))
        {
            if ((object.Equals(tracker, this._firstTap)))
            {
                _reset();
            }
            else
            {
                _checkCancel();
                if ((checked((long)(this._trackers.Count)) == 0))
                {
                    _reset();
                }
            }
        }
    }

    public override void dispose()
    {
        _reset();
        base.dispose();
    }

    internal virtual void _reset()
    {
        _stopDoubleTapTimer();
        if ((this._firstTap is not null))
        {
            if ((checked((long)(this._trackers.Count)) != 0))
            {
                _checkCancel();
            }
            _TapTracker__multitap tracker__10745 = this._firstTap!;
            _firstTap = null;
            _reject(tracker__10745);
            GestureBinding.instance.gestureArena.release(((_TapTracker__multitap)tracker__10745).pointer);
        }
        _clearTrackers();
    }

    internal virtual void _registerFirstTap(_TapTracker__multitap tracker)
    {
        _startDoubleTapTimer();
        GestureBinding.instance.gestureArena.hold(((_TapTracker__multitap)tracker).pointer);
        _freezeTracker(tracker);
        this._trackers.remove(((_TapTracker__multitap)tracker).pointer);
        _clearTrackers();
        _firstTap = tracker;
    }

    internal virtual void _registerSecondTap(_TapTracker__multitap tracker)
    {
        this._firstTap!.entry.resolve(GestureDisposition.accepted);
        ((_TapTracker__multitap)tracker).entry.resolve(GestureDisposition.accepted);
        _freezeTracker(tracker);
        this._trackers.remove(((_TapTracker__multitap)tracker).pointer);
        _checkUp(((_TapTracker__multitap)tracker).initialButtons);
        _reset();
    }

    internal virtual void _clearTrackers()
    {
        this._trackers.Values.ToList().forEach(this._reject);
        DartRuntimePrimitives.Assert(() => (checked((long)(this._trackers.Count)) == 0));
    }

    internal virtual void _freezeTracker(_TapTracker__multitap tracker)
    {
        tracker.stopTrackingPointer((Action<PointerEvent>)this._handleEvent);
    }

    internal virtual void _startDoubleTapTimer()
    {
        _doubleTapTimer ??= new Timer(global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapTimeout, this._reset);
    }

    internal virtual void _stopDoubleTapTimer()
    {
        if ((this._doubleTapTimer is not null))
        {
            this._doubleTapTimer!.cancel();
            _doubleTapTimer = null;
        }
    }

    internal virtual void _checkUp(long buttons)
    {
        if ((this.onDoubleTap is not null))
        {
            invokeCallback<object?>("onDoubleTap", () => { ((Action)(this.onDoubleTap!))(); return null; });
        }
    }

    internal virtual void _checkCancel()
    {
        if ((this.onDoubleTapCancel is not null))
        {
            invokeCallback<object?>("onDoubleTapCancel", () => { ((Action)(this.onDoubleTapCancel!))(); return null; });
        }
    }

    public override string debugDescription => "double tap";
}

internal class _TapGesture__multitap : _TapTracker__multitap
{
    public virtual MultiTapGestureRecognizer gestureRecognizer { get; private set; } = default!;
    internal virtual bool _wonArena { get; set; } = false;
    internal virtual Timer? _timer { get; set; } = default;
    internal virtual OffsetPair _lastPosition { get; set; } = default!;
    internal virtual OffsetPair? _finalPosition { get; set; } = default;

    internal _TapGesture__multitap(MultiTapGestureRecognizer gestureRecognizer, PointerEvent @event, Duration longTapDelay, DeviceGestureSettings? gestureSettings) : base(gestureSettings: gestureSettings, @event: ((PointerDownEvent?)(object?)@event)!, entry: GestureBinding.instance.gestureArena.add(((PointerDownEvent)@event).pointer, gestureRecognizer), doubleTapMinTime: global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapMinTime)
    {
        this.gestureRecognizer = gestureRecognizer;
        this._lastPosition = OffsetPair.CreateFromEventPosition(@event);
    }

    public virtual void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (((PointerEvent)@event).pointer == pointer));
        if ((@event is PointerMoveEvent))
        {
            PointerMoveEvent @event__as13527 = (PointerMoveEvent)@event;
            if (!isWithinGlobalTolerance(((PointerMoveEvent)@event__as13527), global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeHitSlop(((PointerMoveEvent)@event__as13527).kind, gestureSettings)))
            {
                cancel();
            }
            else
            {
                _lastPosition = OffsetPair.CreateFromEventPosition(((PointerMoveEvent)@event__as13527));
            }
        }
        else
        {
            if ((@event is PointerCancelEvent))
            {
                PointerCancelEvent @event__as13763 = (PointerCancelEvent)@event;
                cancel();
            }
            else
            {
                if ((@event is PointerUpEvent))
                {
                    PointerUpEvent @event__as13825 = (PointerUpEvent)@event;
                    stopTrackingPointer((Action<PointerEvent>)this.handleEvent);
                    _finalPosition = OffsetPair.CreateFromEventPosition(((PointerUpEvent)@event__as13825));
                    _check();
                }
            }
        }
    }

    public override void stopTrackingPointer(Action<PointerEvent> route)
    {
        this._timer?.cancel();
        _timer = null;
        base.stopTrackingPointer((Action<PointerEvent>)route);
    }

    public virtual void accept()
    {
        _wonArena = true;
        _check();
    }

    public virtual void reject()
    {
        stopTrackingPointer((Action<PointerEvent>)this.handleEvent);
        this.gestureRecognizer._dispatchCancel(pointer);
    }

    public virtual void cancel()
    {
        if (this._wonArena)
        {
            reject();
        }
        else
        {
            entry.resolve(GestureDisposition.rejected);
        }
    }

    internal virtual void _check()
    {
        if ((this._wonArena && (this._finalPosition is not null)))
        {
            this.gestureRecognizer._dispatchTap(pointer, this._finalPosition!);
        }
    }

}

public class MultiTapGestureRecognizer : GestureRecognizer
{
    public virtual Action<long, TapDownDetails>? onTapDown { get; set; } = default;
    public virtual Action<long, TapUpDetails>? onTapUp { get; set; } = default;
    public virtual Action<long>? onTap { get; set; } = default;
    public virtual Action<long>? onTapCancel { get; set; } = default;
    public virtual Duration longTapDelay { get; set; } = default!;
    public virtual Action<long, TapDownDetails>? onLongTapDown { get; set; } = default;
    internal virtual DartMap<long, _TapGesture__multitap> _gestureMap { get; private set; } = new DartMap<long, _TapGesture__multitap>();

    public MultiTapGestureRecognizer(Duration longTapDelay = default, object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
        this.longTapDelay = longTapDelay;
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !this._gestureMap.ContainsKey(@event.pointer));
        this._gestureMap[@event.pointer] = new _TapGesture__multitap(gestureRecognizer: this, @event: @event, longTapDelay: this.longTapDelay, gestureSettings: gestureSettings);
        if ((this.onTapDown is not null))
        {
            invokeCallback<object?>("onTapDown", () =>
            {
                ((Action)((() =>
                {
                    this.onTapDown!(@event.pointer, new TapDownDetails(globalPosition: @event.position, localPosition: @event.localPosition, kind: @event.kind));
                })))(); return null;
            });
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => this._gestureMap.ContainsKey(pointer));
        this._gestureMap.GetValueOrDefault(pointer)!.accept();
    }

    public override void rejectGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => this._gestureMap.ContainsKey(pointer));
        this._gestureMap.GetValueOrDefault(pointer)!.reject();
        DartRuntimePrimitives.Assert(() => !this._gestureMap.ContainsKey(pointer));
    }

    internal virtual void _dispatchCancel(long pointer)
    {
        DartRuntimePrimitives.Assert(() => this._gestureMap.ContainsKey(pointer));
        this._gestureMap.remove(pointer);
        if ((this.onTapCancel is not null))
        {
            invokeCallback<object?>("onTapCancel", () => { ((Action)((() => this.onTapCancel!(pointer))))(); return null; });
        }
    }

    internal virtual void _dispatchTap(long pointer, OffsetPair position)
    {
        DartRuntimePrimitives.Assert(() => this._gestureMap.ContainsKey(pointer));
        this._gestureMap.remove(pointer);
        if ((this.onTapUp is not null))
        {
            invokeCallback<object?>("onTapUp", () =>
            {
                ((Action)((() =>
                {
                    this.onTapUp!(pointer, new TapUpDetails(kind: getKindForPointer(pointer), localPosition: ((OffsetPair)position).local, globalPosition: ((OffsetPair)position).global));
                })))(); return null;
            });
        }
        if ((this.onTap is not null))
        {
            invokeCallback<object?>("onTap", () => { ((Action)((() => this.onTap!(pointer))))(); return null; });
        }
    }

    internal virtual void _dispatchLongTap(long pointer, OffsetPair lastPosition)
    {
        DartRuntimePrimitives.Assert(() => this._gestureMap.ContainsKey(pointer));
        if ((this.onLongTapDown is not null))
        {
            invokeCallback<object?>("onLongTapDown", () =>
            {
                ((Action)((() =>
                {
                    this.onLongTapDown!(pointer, new TapDownDetails(globalPosition: ((OffsetPair)lastPosition).global, localPosition: ((OffsetPair)lastPosition).local, kind: getKindForPointer(pointer)));
                })))(); return null;
            });
        }
    }

    public override void dispose()
    {
        var localGestures__18525 = new List<_TapGesture__multitap>(DartRuntimePrimitives.ConvertEnumerable<_TapGesture__multitap>(this._gestureMap.Values));
        foreach (var gesture__18598 in localGestures__18525)
        {
            gesture__18598.cancel();
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this._gestureMap.Count)) == 0));
        base.dispose();
    }

    public override string debugDescription => "multitap";
}

public delegate void GestureSerialTapDownCallback(SerialTapDownDetails details);

public class SerialTapDownDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual long buttons { get; private set; } = default!;
    public virtual long count { get; private set; } = default!;

    public SerialTapDownDetails(Offset globalPosition = default, Offset? localPosition = null, PointerDeviceKind kind = default!, long buttons = 0, long count = 1)
    {
        this.__field_globalPosition = globalPosition;
        this.kind = kind;
        this.buttons = buttons;
        this.count = count;
        this.__field_localPosition = (localPosition ?? globalPosition);
        System.Diagnostics.Debug.Assert((count > 0L));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
        properties.add(new IntProperty("buttons", this.buttons));
        properties.add(new IntProperty("count", this.count));
    }

}

public delegate void GestureSerialTapCancelCallback(SerialTapCancelDetails details);

public class SerialTapCancelDetails : Diagnosticable
{
    public virtual long count { get; private set; } = default!;

    public SerialTapCancelDetails(long count = 1)
    {
        this.count = count;
        System.Diagnostics.Debug.Assert((count > 0L));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("count", this.count));
    }

}

public delegate void GestureSerialTapUpCallback(SerialTapUpDetails details);

public class SerialTapUpDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual PointerDeviceKind? kind { get; private set; }
    public virtual long count { get; private set; } = default!;

    public SerialTapUpDetails(Offset globalPosition = default, Offset? localPosition = null, PointerDeviceKind? kind = null, long count = 1)
    {
        this.__field_globalPosition = globalPosition;
        this.kind = kind;
        this.count = count;
        this.__field_localPosition = (localPosition ?? globalPosition);
        System.Diagnostics.Debug.Assert((count > 0L));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
        properties.add(new IntProperty("count", this.count));
    }

}

public class SerialTapGestureRecognizer : GestureRecognizer
{
    public virtual Action<SerialTapDownDetails>? onSerialTapDown { get; set; } = default;
    public virtual Action<SerialTapCancelDetails>? onSerialTapCancel { get; set; } = default;
    public virtual Action<SerialTapUpDetails>? onSerialTapUp { get; set; } = default;
    internal virtual Timer? _serialTapTimer { get; set; } = default;
    internal virtual List<_TapTracker__multitap> _completedTaps { get; private set; } = new List<_TapTracker__multitap>();
    internal virtual DartMap<long, GestureDisposition> _gestureResolutions { get; private set; } = new DartMap<long, GestureDisposition>();
    internal virtual _TapTracker__multitap? _pendingTap { get; set; } = default;

    public SerialTapGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    public virtual bool isTrackingPointer => (this._pendingTap is not null);
    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if ((((this.onSerialTapDown is null) && (this.onSerialTapCancel is null)) && (this.onSerialTapUp is null)))
        {
            return false;
        }
        return base.isPointerAllowed(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if (((((checked((long)(this._completedTaps.Count)) != 0) && !_representsSameSeries(this._completedTaps.Last(), @event))) || (this._pendingTap is not null)))
        {
            _reset();
        }
        _trackTap(@event);
    }

    internal virtual bool _representsSameSeries(_TapTracker__multitap tap, PointerDownEvent @event)
    {
        return ((tap.hasElapsedMinTime() && tap.hasSameButton(@event)) && tap.isWithinGlobalTolerance(@event, global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapSlop));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _trackTap(PointerDownEvent @event)
    {
        _stopSerialTapTimer();
        if ((this.onSerialTapDown is not null))
        {
            var details__31711 = new SerialTapDownDetails(globalPosition: @event.position, localPosition: @event.localPosition, kind: getKindForPointer(@event.pointer), buttons: @event.buttons, count: (checked((long)(this._completedTaps.Count)) + 1L));
            invokeCallback<object?>("onSerialTapDown", () => { ((Action)((() => this.onSerialTapDown!(details__31711))))(); return null; });
        }
        var tracker__32054 = new _TapTracker__multitap(gestureSettings: gestureSettings, @event: @event, entry: GestureBinding.instance.gestureArena.add(@event.pointer, this), doubleTapMinTime: global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapMinTime);
        DartRuntimePrimitives.Assert(() => (this._pendingTap is null));
        _pendingTap = tracker__32054;
        tracker__32054.startTrackingPointer((Action<PointerEvent>)this._handleEvent, @event.transform);
    }

    internal virtual void _handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (this._pendingTap is not null));
        DartRuntimePrimitives.Assert(() => (this._pendingTap!.pointer == ((PointerEvent)@event).pointer));
        _TapTracker__multitap tracker__32541 = this._pendingTap!;
        if ((@event is PointerUpEvent))
        {
            PointerUpEvent @event__as32573 = (PointerUpEvent)@event;
            _registerTap(((PointerUpEvent)@event__as32573), tracker__32541);
        }
        else
        {
            if ((@event is PointerMoveEvent))
            {
                PointerMoveEvent @event__as32651 = (PointerMoveEvent)@event;
                if (!tracker__32541.isWithinGlobalTolerance(((PointerMoveEvent)@event__as32651), global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapTouchSlop))
                {
                    _reset();
                }
            }
            else
            {
                if ((@event is PointerCancelEvent))
                {
                    PointerCancelEvent @event__as32795 = (PointerCancelEvent)@event;
                    _reset();
                }
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (this._pendingTap is not null));
        DartRuntimePrimitives.Assert(() => (this._pendingTap!.pointer == pointer));
        this._gestureResolutions[pointer] = GestureDisposition.accepted;
    }

    public override void rejectGesture(long pointer)
    {
        this._gestureResolutions[pointer] = GestureDisposition.rejected;
        _reset();
    }

    internal virtual void _rejectPendingTap()
    {
        DartRuntimePrimitives.Assert(() => (this._pendingTap is not null));
        _TapTracker__multitap tracker__33263 = this._pendingTap!;
        _pendingTap = null;
        _checkCancel((checked((long)(this._completedTaps.Count)) + 1L));
        if (!this._gestureResolutions.ContainsKey(((_TapTracker__multitap)tracker__33263).pointer))
        {
            ((_TapTracker__multitap)tracker__33263).entry.resolve(GestureDisposition.rejected);
        }
        _stopTrackingPointer(tracker__33263);
    }

    public override void dispose()
    {
        _reset();
        base.dispose();
    }

    internal virtual void _reset()
    {
        if ((this._pendingTap is not null))
        {
            _rejectPendingTap();
        }
        _pendingTap = null;
        this._completedTaps.Clear();
        this._gestureResolutions.Clear();
        _stopSerialTapTimer();
    }

    internal virtual void _registerTap(PointerUpEvent @event, _TapTracker__multitap tracker)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(tracker, this._pendingTap)));
        DartRuntimePrimitives.Assert(() => (((_TapTracker__multitap)tracker).pointer == @event.pointer));
        _startSerialTapTimer();
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._gestureResolutions.GetValueOrDefault(@event.pointer), GestureDisposition.rejected)));
        if (!this._gestureResolutions.ContainsKey(@event.pointer))
        {
            ((_TapTracker__multitap)tracker).entry.resolve(GestureDisposition.accepted);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._gestureResolutions.GetValueOrDefault(@event.pointer), GestureDisposition.accepted)));
        _stopTrackingPointer(tracker);
        _pendingTap = null;
        _checkUp(@event, tracker);
        this._completedTaps.Add(tracker);
    }

    internal virtual void _stopTrackingPointer(_TapTracker__multitap tracker)
    {
        tracker.stopTrackingPointer((Action<PointerEvent>)this._handleEvent);
    }

    internal virtual void _startSerialTapTimer()
    {
        _serialTapTimer ??= new Timer(global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kDoubleTapTimeout, this._reset);
    }

    internal virtual void _stopSerialTapTimer()
    {
        if ((this._serialTapTimer is not null))
        {
            this._serialTapTimer!.cancel();
            _serialTapTimer = null;
        }
    }

    internal virtual void _checkUp(PointerUpEvent @event, _TapTracker__multitap tracker)
    {
        if ((this.onSerialTapUp is not null))
        {
            var details__35109 = new SerialTapUpDetails(globalPosition: @event.position, localPosition: @event.localPosition, kind: getKindForPointer(((_TapTracker__multitap)tracker).pointer), count: (checked((long)(this._completedTaps.Count)) + 1L));
            invokeCallback<object?>("onSerialTapUp", () => { ((Action)((() => this.onSerialTapUp!(details__35109))))(); return null; });
        }
    }

    internal virtual void _checkCancel(long count)
    {
        if ((this.onSerialTapCancel is not null))
        {
            var details__35493 = new SerialTapCancelDetails(count: count);
            invokeCallback<object?>("onSerialTapCancel", () => { ((Action)((() => this.onSerialTapCancel!(details__35493))))(); return null; });
        }
    }

    public override string debugDescription => "serial tap";
}


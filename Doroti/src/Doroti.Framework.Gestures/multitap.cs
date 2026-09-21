// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/multitap.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public delegate void GestureDoubleTapCallback();

public delegate void GestureMultiTapDownCallback(long pointer, TapDownDetails details);

public delegate void GestureMultiTapUpCallback(long pointer, TapUpDetails details);

public delegate void GestureMultiTapCallback(long pointer);

public delegate void GestureMultiTapCancelCallback(long pointer);

internal class _CountdownZoned__multitap
{
    internal virtual bool _timeout { get; set; } = false;

    internal _CountdownZoned__multitap(Duration duration) { }

    public virtual bool timeout => _timeout;

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
    internal virtual _CountdownZoned__multitap _doubleTapMinTimeCountdown { get; private set; } =
        default!;
    internal virtual bool _isTrackingPointer { get; set; } = false;

    internal _TapTracker__multitap(
        PointerDownEvent @event,
        GestureArenaEntry entry,
        Duration doubleTapMinTime,
        DeviceGestureSettings? gestureSettings
    )
    {
        this.entry = entry;
        this.gestureSettings = gestureSettings;
        pointer = @event.pointer;
        _initialGlobalPosition = @event.position;
        initialButtons = @event.buttons;
        _doubleTapMinTimeCountdown = new _CountdownZoned__multitap(duration: doubleTapMinTime);
    }

    public virtual void startTrackingPointer(Action<PointerEvent> route, Matrix4? transform)
    {
        if (!_isTrackingPointer)
        {
            _isTrackingPointer = true;
            GestureBinding.instance.pointerRouter.addRoute(pointer, route, transform);
        }
    }

    public virtual void stopTrackingPointer(Action<PointerEvent> route)
    {
        if (_isTrackingPointer)
        {
            _isTrackingPointer = false;
            GestureBinding.instance.pointerRouter.removeRoute(pointer, route);
        }
    }

    public virtual bool isWithinGlobalTolerance(PointerEvent @event, double tolerance)
    {
        Offset offset = @event.position - _initialGlobalPosition;
        return offset.distance <= tolerance;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hasElapsedMinTime()
    {
        return _doubleTapMinTimeCountdown.timeout;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hasSameButton(PointerDownEvent @event)
    {
        return @event.buttons == initialButtons;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class DoubleTapGestureRecognizer : GestureRecognizer
{
    public virtual Action<TapDownDetails>? onDoubleTapDown { get; set; } = default;
    public virtual Action? onDoubleTap { get; set; } = default;
    public virtual Action? onDoubleTapCancel { get; set; } = default;
    internal virtual Timer? _doubleTapTimer { get; set; } = default;
    internal virtual _TapTracker__multitap? _firstTap { get; set; } = default;
    internal virtual DartMap<long, _TapTracker__multitap> _trackers { get; private set; } =
        new DartMap<long, _TapTracker__multitap>();

    public DoubleTapGestureRecognizer(
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null,
        Func<long, bool> allowedButtonsFilter = default!
    )
        : base(
            debugOwner: debugOwner,
            supportedDevices: supportedDevices,
            allowedButtonsFilter: allowedButtonsFilter
                ?? GestureRecognizer._defaultButtonAcceptBehavior
        ) { }

    internal static new bool _defaultButtonAcceptBehavior(long buttons) =>
        buttons == EventsLibrary.kPrimaryButton;

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if (_firstTap is null)
        {
            if ((onDoubleTapDown is null) && (onDoubleTap is null) && (onDoubleTapCancel is null))
            {
                return false;
            }
        }
        bool isPointerAllowedLocal = base.isPointerAllowed(@event);
        if (!isPointerAllowedLocal)
        {
            _reset();
        }
        return isPointerAllowedLocal;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if (_firstTap is not null)
        {
            if (!_firstTap!.isWithinGlobalTolerance(@event, ConstantsLibrary.kDoubleTapSlop))
            {
                return;
            }
            else
            {
                if (!_firstTap!.hasElapsedMinTime() || !_firstTap!.hasSameButton(@event))
                {
                    _reset();
                    _trackTap(@event);
                    return;
                }
                else
                {
                    if (onDoubleTapDown is not null)
                    {
                        var details = new TapDownDetails(
                            globalPosition: @event.position,
                            localPosition: @event.localPosition,
                            kind: getKindForPointer(@event.pointer)
                        );
                        invokeCallback<object?>(
                            "onDoubleTapDown",
                            () =>
                            {
                                ((Action)(() => onDoubleTapDown!(details)))();
                                return null;
                            }
                        );
                    }
                }
            }
        }
        _trackTap(@event);
    }

    internal virtual void _trackTap(PointerDownEvent @event)
    {
        _stopDoubleTapTimer();
        var tracker = new _TapTracker__multitap(
            @event: @event,
            entry: GestureBinding.instance.gestureArena.add(@event.pointer, this),
            doubleTapMinTime: ConstantsLibrary.kDoubleTapMinTime,
            gestureSettings: gestureSettings
        );
        _trackers[@event.pointer] = tracker;
        tracker.startTrackingPointer(_handleEvent, @event.transform);
    }

    internal virtual void _handleEvent(PointerEvent @event)
    {
        _TapTracker__multitap tracker = _trackers.GetValueOrDefault(@event.pointer)!;
        if (@event is PointerUpEvent)
        {
            PointerUpEvent @event__as9191 = (PointerUpEvent)@event;
            if (_firstTap is null)
            {
                _registerFirstTap(tracker);
            }
            else
            {
                _registerSecondTap(tracker);
            }
        }
        else
        {
            if (@event is PointerMoveEvent)
            {
                PointerMoveEvent @event__as9360 = (PointerMoveEvent)@event;
                if (
                    !tracker.isWithinGlobalTolerance(
                        @event__as9360,
                        ConstantsLibrary.kDoubleTapTouchSlop
                    )
                )
                {
                    _reject(tracker);
                }
            }
            else
            {
                if (@event is PointerCancelEvent)
                {
                    PointerCancelEvent @event__as9512 = (PointerCancelEvent)@event;
                    _reject(tracker);
                }
            }
        }
    }

    public override void acceptGesture(long pointer) { }

    public override void rejectGesture(long pointer)
    {
        _TapTracker__multitap? tracker = _trackers.GetValueOrDefault(pointer);
        if ((tracker is null) && (_firstTap is not null) && (_firstTap!.pointer == pointer))
        {
            tracker = _firstTap;
        }
        if (tracker is not null)
        {
            _reject(tracker);
        }
    }

    internal virtual void _reject(_TapTracker__multitap tracker)
    {
        _trackers.remove(tracker.pointer);
        tracker.entry.resolve(GestureDisposition.rejected);
        _freezeTracker(tracker);
        if (_firstTap is not null)
        {
            if (Equals(tracker, _firstTap))
            {
                _reset();
            }
            else
            {
                _checkCancel();
                if (checked((long)_trackers.Count) == 0)
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
        if (_firstTap is not null)
        {
            if (checked((long)_trackers.Count) != 0)
            {
                _checkCancel();
            }
            _TapTracker__multitap tracker = _firstTap!;
            _firstTap = null;
            _reject(tracker);
            GestureBinding.instance.gestureArena.release(tracker.pointer);
        }
        _clearTrackers();
    }

    internal virtual void _registerFirstTap(_TapTracker__multitap tracker)
    {
        _startDoubleTapTimer();
        GestureBinding.instance.gestureArena.hold(tracker.pointer);
        _freezeTracker(tracker);
        _trackers.remove(tracker.pointer);
        _clearTrackers();
        _firstTap = tracker;
    }

    internal virtual void _registerSecondTap(_TapTracker__multitap tracker)
    {
        _firstTap!.entry.resolve(GestureDisposition.accepted);
        tracker.entry.resolve(GestureDisposition.accepted);
        _freezeTracker(tracker);
        _trackers.remove(tracker.pointer);
        _checkUp(tracker.initialButtons);
        _reset();
    }

    internal virtual void _clearTrackers()
    {
        _trackers.Values.ToList().forEach(_reject);
        DartRuntimePrimitives.Assert(() => checked((long)_trackers.Count) == 0);
    }

    internal virtual void _freezeTracker(_TapTracker__multitap tracker)
    {
        tracker.stopTrackingPointer(_handleEvent);
    }

    internal virtual void _startDoubleTapTimer()
    {
        _doubleTapTimer ??= new Timer(ConstantsLibrary.kDoubleTapTimeout, _reset);
    }

    internal virtual void _stopDoubleTapTimer()
    {
        if (_doubleTapTimer is not null)
        {
            _doubleTapTimer!.cancel();
            _doubleTapTimer = null;
        }
    }

    internal virtual void _checkUp(long buttons)
    {
        if (onDoubleTap is not null)
        {
            invokeCallback<object?>(
                "onDoubleTap",
                () =>
                {
                    onDoubleTap!();
                    return null;
                }
            );
        }
    }

    internal virtual void _checkCancel()
    {
        if (onDoubleTapCancel is not null)
        {
            invokeCallback<object?>(
                "onDoubleTapCancel",
                () =>
                {
                    onDoubleTapCancel!();
                    return null;
                }
            );
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

    internal _TapGesture__multitap(
        MultiTapGestureRecognizer gestureRecognizer,
        PointerEvent @event,
        Duration longTapDelay,
        DeviceGestureSettings? gestureSettings
    )
        : base(
            gestureSettings: gestureSettings,
            @event: ((PointerDownEvent?)(object?)@event)!,
            entry: GestureBinding.instance.gestureArena.add(
                ((PointerDownEvent)@event).pointer,
                gestureRecognizer
            ),
            doubleTapMinTime: ConstantsLibrary.kDoubleTapMinTime
        )
    {
        this.gestureRecognizer = gestureRecognizer;
        _lastPosition = OffsetPair.CreateFromEventPosition(@event);
    }

    public virtual void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => @event.pointer == pointer);
        if (@event is PointerMoveEvent)
        {
            PointerMoveEvent @event__as13527 = (PointerMoveEvent)@event;
            if (
                !isWithinGlobalTolerance(
                    @event__as13527,
                    EventsLibrary.computeHitSlop(@event__as13527.kind, gestureSettings)
                )
            )
            {
                cancel();
            }
            else
            {
                _lastPosition = OffsetPair.CreateFromEventPosition(@event__as13527);
            }
        }
        else
        {
            if (@event is PointerCancelEvent)
            {
                PointerCancelEvent @event__as13763 = (PointerCancelEvent)@event;
                cancel();
            }
            else
            {
                if (@event is PointerUpEvent)
                {
                    PointerUpEvent @event__as13825 = (PointerUpEvent)@event;
                    stopTrackingPointer(handleEvent);
                    _finalPosition = OffsetPair.CreateFromEventPosition(@event__as13825);
                    _check();
                }
            }
        }
    }

    public override void stopTrackingPointer(Action<PointerEvent> route)
    {
        _timer?.cancel();
        _timer = null;
        base.stopTrackingPointer(route);
    }

    public virtual void accept()
    {
        _wonArena = true;
        _check();
    }

    public virtual void reject()
    {
        stopTrackingPointer(handleEvent);
        gestureRecognizer._dispatchCancel(pointer);
    }

    public virtual void cancel()
    {
        if (_wonArena)
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
        if (_wonArena && (_finalPosition is not null))
        {
            gestureRecognizer._dispatchTap(pointer, _finalPosition!);
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
    internal virtual DartMap<long, _TapGesture__multitap> _gestureMap { get; private set; } =
        new DartMap<long, _TapGesture__multitap>();

    public MultiTapGestureRecognizer(
        Duration longTapDelay = default,
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
        this.longTapDelay = longTapDelay;
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !_gestureMap.ContainsKey(@event.pointer));
        _gestureMap[@event.pointer] = new _TapGesture__multitap(
            gestureRecognizer: this,
            @event: @event,
            longTapDelay: longTapDelay,
            gestureSettings: gestureSettings
        );
        if (onTapDown is not null)
        {
            invokeCallback<object?>(
                "onTapDown",
                () =>
                {
                    (
                        (Action)(
                            () =>
                            {
                                onTapDown!(
                                    @event.pointer,
                                    new TapDownDetails(
                                        globalPosition: @event.position,
                                        localPosition: @event.localPosition,
                                        kind: @event.kind
                                    )
                                );
                            }
                        )
                    )();
                    return null;
                }
            );
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => _gestureMap.ContainsKey(pointer));
        _gestureMap.GetValueOrDefault(pointer)!.accept();
    }

    public override void rejectGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => _gestureMap.ContainsKey(pointer));
        _gestureMap.GetValueOrDefault(pointer)!.reject();
        DartRuntimePrimitives.Assert(() => !_gestureMap.ContainsKey(pointer));
    }

    internal virtual void _dispatchCancel(long pointer)
    {
        DartRuntimePrimitives.Assert(() => _gestureMap.ContainsKey(pointer));
        _gestureMap.remove(pointer);
        if (onTapCancel is not null)
        {
            invokeCallback<object?>(
                "onTapCancel",
                () =>
                {
                    ((Action)(() => onTapCancel!(pointer)))();
                    return null;
                }
            );
        }
    }

    internal virtual void _dispatchTap(long pointer, OffsetPair position)
    {
        DartRuntimePrimitives.Assert(() => _gestureMap.ContainsKey(pointer));
        _gestureMap.remove(pointer);
        if (onTapUp is not null)
        {
            invokeCallback<object?>(
                "onTapUp",
                () =>
                {
                    (
                        (Action)(
                            () =>
                            {
                                onTapUp!(
                                    pointer,
                                    new TapUpDetails(
                                        kind: getKindForPointer(pointer),
                                        localPosition: position.local,
                                        globalPosition: position.global
                                    )
                                );
                            }
                        )
                    )();
                    return null;
                }
            );
        }
        if (onTap is not null)
        {
            invokeCallback<object?>(
                "onTap",
                () =>
                {
                    ((Action)(() => onTap!(pointer)))();
                    return null;
                }
            );
        }
    }

    internal virtual void _dispatchLongTap(long pointer, OffsetPair lastPosition)
    {
        DartRuntimePrimitives.Assert(() => _gestureMap.ContainsKey(pointer));
        if (onLongTapDown is not null)
        {
            invokeCallback<object?>(
                "onLongTapDown",
                () =>
                {
                    (
                        (Action)(
                            () =>
                            {
                                onLongTapDown!(
                                    pointer,
                                    new TapDownDetails(
                                        globalPosition: lastPosition.global,
                                        localPosition: lastPosition.local,
                                        kind: getKindForPointer(pointer)
                                    )
                                );
                            }
                        )
                    )();
                    return null;
                }
            );
        }
    }

    public override void dispose()
    {
        var localGestures = new List<_TapGesture__multitap>(
            DartRuntimePrimitives.ConvertEnumerable<_TapGesture__multitap>(_gestureMap.Values)
        );
        foreach (var gesture in localGestures)
        {
            gesture.cancel();
        }
        DartRuntimePrimitives.Assert(() => checked((long)_gestureMap.Count) == 0);
        base.dispose();
    }

    public override string debugDescription => "multitap";
}

public delegate void GestureSerialTapDownCallback(SerialTapDownDetails details);

public class SerialTapDownDetails : PositionedGestureDetails, Diagnosticable
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
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual long buttons { get; private set; } = default!;
    public virtual long count { get; private set; } = default!;

    public SerialTapDownDetails(
        Offset globalPosition = default,
        Offset? localPosition = null,
        PointerDeviceKind kind = default!,
        long buttons = 0,
        long count = 1
    )
    {
        __field_globalPosition = globalPosition;
        this.kind = kind;
        this.buttons = buttons;
        this.count = count;
        __field_localPosition = localPosition ?? globalPosition;
        System.Diagnostics.Debug.Assert(count > 0L);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
        properties.add(new IntProperty("buttons", buttons));
        properties.add(new IntProperty("count", count));
    }
}

public delegate void GestureSerialTapCancelCallback(SerialTapCancelDetails details);

public class SerialTapCancelDetails : Diagnosticable
{
    public virtual long count { get; private set; } = default!;

    public SerialTapCancelDetails(long count = 1)
    {
        this.count = count;
        System.Diagnostics.Debug.Assert(count > 0L);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("count", count));
    }
}

public delegate void GestureSerialTapUpCallback(SerialTapUpDetails details);

public class SerialTapUpDetails : PositionedGestureDetails, Diagnosticable
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
    public virtual long count { get; private set; } = default!;

    public SerialTapUpDetails(
        Offset globalPosition = default,
        Offset? localPosition = null,
        PointerDeviceKind? kind = null,
        long count = 1
    )
    {
        __field_globalPosition = globalPosition;
        this.kind = kind;
        this.count = count;
        __field_localPosition = localPosition ?? globalPosition;
        System.Diagnostics.Debug.Assert(count > 0L);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
        properties.add(new IntProperty("count", count));
    }
}

public class SerialTapGestureRecognizer : GestureRecognizer
{
    public virtual Action<SerialTapDownDetails>? onSerialTapDown { get; set; } = default;
    public virtual Action<SerialTapCancelDetails>? onSerialTapCancel { get; set; } = default;
    public virtual Action<SerialTapUpDetails>? onSerialTapUp { get; set; } = default;
    internal virtual Timer? _serialTapTimer { get; set; } = default;
    internal virtual List<_TapTracker__multitap> _completedTaps { get; private set; } =
        new List<_TapTracker__multitap>();
    internal virtual DartMap<long, GestureDisposition> _gestureResolutions { get; private set; } =
        new DartMap<long, GestureDisposition>();
    internal virtual _TapTracker__multitap? _pendingTap { get; set; } = default;

    public SerialTapGestureRecognizer(
        object? debugOwner = null,
        HashSet<PointerDeviceKind>? supportedDevices = null,
        Func<long, bool> allowedButtonsFilter = default!
    )
        : base(
            debugOwner: debugOwner,
            supportedDevices: supportedDevices,
            allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior
        ) { }

    public virtual bool isTrackingPointer => _pendingTap is not null;

    public override bool isPointerAllowed(PointerDownEvent @event)
    {
        if ((onSerialTapDown is null) && (onSerialTapCancel is null) && (onSerialTapUp is null))
        {
            return false;
        }
        return base.isPointerAllowed(@event);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        if (
            (
                (checked((long)_completedTaps.Count) != 0)
                && !_representsSameSeries(_completedTaps.Last(), @event)
            ) || (_pendingTap is not null)
        )
        {
            _reset();
        }
        _trackTap(@event);
    }

    internal virtual bool _representsSameSeries(_TapTracker__multitap tap, PointerDownEvent @event)
    {
        return tap.hasElapsedMinTime()
            && tap.hasSameButton(@event)
            && tap.isWithinGlobalTolerance(@event, ConstantsLibrary.kDoubleTapSlop);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _trackTap(PointerDownEvent @event)
    {
        _stopSerialTapTimer();
        if (onSerialTapDown is not null)
        {
            var details = new SerialTapDownDetails(
                globalPosition: @event.position,
                localPosition: @event.localPosition,
                kind: getKindForPointer(@event.pointer),
                buttons: @event.buttons,
                count: checked(_completedTaps.Count) + 1L
            );
            invokeCallback<object?>(
                "onSerialTapDown",
                () =>
                {
                    ((Action)(() => onSerialTapDown!(details)))();
                    return null;
                }
            );
        }
        var tracker = new _TapTracker__multitap(
            gestureSettings: gestureSettings,
            @event: @event,
            entry: GestureBinding.instance.gestureArena.add(@event.pointer, this),
            doubleTapMinTime: ConstantsLibrary.kDoubleTapMinTime
        );
        DartRuntimePrimitives.Assert(() => _pendingTap is null);
        _pendingTap = tracker;
        tracker.startTrackingPointer(_handleEvent, @event.transform);
    }

    internal virtual void _handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => _pendingTap is not null);
        DartRuntimePrimitives.Assert(() => _pendingTap!.pointer == @event.pointer);
        _TapTracker__multitap tracker = _pendingTap!;
        if (@event is PointerUpEvent)
        {
            PointerUpEvent @event__as32573 = (PointerUpEvent)@event;
            _registerTap(@event__as32573, tracker);
        }
        else
        {
            if (@event is PointerMoveEvent)
            {
                PointerMoveEvent @event__as32651 = (PointerMoveEvent)@event;
                if (
                    !tracker.isWithinGlobalTolerance(
                        @event__as32651,
                        ConstantsLibrary.kDoubleTapTouchSlop
                    )
                )
                {
                    _reset();
                }
            }
            else
            {
                if (@event is PointerCancelEvent)
                {
                    PointerCancelEvent @event__as32795 = (PointerCancelEvent)@event;
                    _reset();
                }
            }
        }
    }

    public override void acceptGesture(long pointer)
    {
        DartRuntimePrimitives.Assert(() => _pendingTap is not null);
        DartRuntimePrimitives.Assert(() => _pendingTap!.pointer == pointer);
        _gestureResolutions[pointer] = GestureDisposition.accepted;
    }

    public override void rejectGesture(long pointer)
    {
        _gestureResolutions[pointer] = GestureDisposition.rejected;
        _reset();
    }

    internal virtual void _rejectPendingTap()
    {
        DartRuntimePrimitives.Assert(() => _pendingTap is not null);
        _TapTracker__multitap tracker = _pendingTap!;
        _pendingTap = null;
        _checkCancel(checked(_completedTaps.Count) + 1L);
        if (!_gestureResolutions.ContainsKey(tracker.pointer))
        {
            tracker.entry.resolve(GestureDisposition.rejected);
        }
        _stopTrackingPointer(tracker);
    }

    public override void dispose()
    {
        _reset();
        base.dispose();
    }

    internal virtual void _reset()
    {
        if (_pendingTap is not null)
        {
            _rejectPendingTap();
        }
        _pendingTap = null;
        _completedTaps.Clear();
        _gestureResolutions.Clear();
        _stopSerialTapTimer();
    }

    internal virtual void _registerTap(PointerUpEvent @event, _TapTracker__multitap tracker)
    {
        DartRuntimePrimitives.Assert(() => Equals(tracker, _pendingTap));
        DartRuntimePrimitives.Assert(() => tracker.pointer == @event.pointer);
        _startSerialTapTimer();
        DartRuntimePrimitives.Assert(() =>
            !Equals(
                _gestureResolutions.GetValueOrDefault(@event.pointer),
                GestureDisposition.rejected
            )
        );
        if (!_gestureResolutions.ContainsKey(@event.pointer))
        {
            tracker.entry.resolve(GestureDisposition.accepted);
        }
        DartRuntimePrimitives.Assert(() =>
            Equals(
                _gestureResolutions.GetValueOrDefault(@event.pointer),
                GestureDisposition.accepted
            )
        );
        _stopTrackingPointer(tracker);
        _pendingTap = null;
        _checkUp(@event, tracker);
        _completedTaps.Add(tracker);
    }

    internal virtual void _stopTrackingPointer(_TapTracker__multitap tracker)
    {
        tracker.stopTrackingPointer(_handleEvent);
    }

    internal virtual void _startSerialTapTimer()
    {
        _serialTapTimer ??= new Timer(ConstantsLibrary.kDoubleTapTimeout, _reset);
    }

    internal virtual void _stopSerialTapTimer()
    {
        if (_serialTapTimer is not null)
        {
            _serialTapTimer!.cancel();
            _serialTapTimer = null;
        }
    }

    internal virtual void _checkUp(PointerUpEvent @event, _TapTracker__multitap tracker)
    {
        if (onSerialTapUp is not null)
        {
            var details = new SerialTapUpDetails(
                globalPosition: @event.position,
                localPosition: @event.localPosition,
                kind: getKindForPointer(tracker.pointer),
                count: checked(_completedTaps.Count) + 1L
            );
            invokeCallback<object?>(
                "onSerialTapUp",
                () =>
                {
                    ((Action)(() => onSerialTapUp!(details)))();
                    return null;
                }
            );
        }
    }

    internal virtual void _checkCancel(long count)
    {
        if (onSerialTapCancel is not null)
        {
            var details = new SerialTapCancelDetails(count: count);
            invokeCallback<object?>(
                "onSerialTapCancel",
                () =>
                {
                    ((Action)(() => onSerialTapCancel!(details)))();
                    return null;
                }
            );
        }
    }

    public override string debugDescription => "serial tap";
}

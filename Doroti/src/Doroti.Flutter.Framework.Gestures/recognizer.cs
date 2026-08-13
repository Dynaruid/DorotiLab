// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/recognizer.dart
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

public delegate T RecognizerCallback<T>();

public enum DragStartBehavior
{
    down,
    start
}

public enum MultitouchDragStrategy
{
    latestPointer,
    averageBoundaryPointers,
    sumAllPointers
}

public delegate bool AllowedButtonsFilter(long buttons);

public abstract class GestureRecognizer : GestureArenaMember, DiagnosticableTree
{
    public virtual object? debugOwner { get; private set; }
    public virtual DeviceGestureSettings? gestureSettings { get; set; } = default;
    public virtual HashSet<PointerDeviceKind>? supportedDevices { get; set; } = default;
    public virtual Func<long, bool> allowedButtonsFilter { get; private set; } = default!;
    internal virtual DartMap<long, _RecognizerEventData__recognizer> _pointerToEventData { get; private set; } = new DartMap<long, _RecognizerEventData__recognizer>();

    protected GestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!)
    {
        Func<long, bool> __allowedButtonsFilter = allowedButtonsFilter ?? _defaultButtonAcceptBehavior;
        this.debugOwner = debugOwner;
        this.supportedDevices = supportedDevices;
        this.allowedButtonsFilter = __allowedButtonsFilter;
    }

    public virtual void acceptGesture(long pointer) => throw new NotSupportedException();
    public virtual void rejectGesture(long pointer) => throw new NotSupportedException();
    internal static bool _defaultButtonAcceptBehavior(long buttons) => true;
    public virtual void addPointerPanZoom(PointerPanZoomStartEvent @event)
    {
        this._pointerToEventData[@event.pointer] = new _RecognizerEventData__recognizer(kind: @event.kind, buttons: @event.buttons);
        if (isPointerPanZoomAllowed(@event))
        {
            addAllowedPointerPanZoom(@event);
        }
        else
        {
            handleNonAllowedPointerPanZoom(@event);
        }
    }

    public virtual void addAllowedPointerPanZoom(PointerPanZoomStartEvent @event)
    {
    }

    public virtual void addPointer(PointerDownEvent @event)
    {
        this._pointerToEventData[@event.pointer] = new _RecognizerEventData__recognizer(kind: @event.kind, buttons: @event.buttons);
        if (isPointerAllowed(@event))
        {
            addAllowedPointer(@event);
        }
        else
        {
            handleNonAllowedPointer(@event);
        }
    }

    public virtual void addAllowedPointer(PointerDownEvent @event)
    {
    }

    public virtual void handleNonAllowedPointer(PointerDownEvent @event)
    {
    }

    public virtual bool isPointerAllowed(PointerDownEvent @event)
    {
        return ((((this.supportedDevices is null) || this.supportedDevices!.Contains(@event.kind))) && this.allowedButtonsFilter(@event.buttons));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleNonAllowedPointerPanZoom(PointerPanZoomStartEvent @event)
    {
    }

    public virtual bool isPointerPanZoomAllowed(PointerPanZoomStartEvent @event)
    {
        return ((this.supportedDevices is null) || this.supportedDevices!.Contains(@event.kind));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Ui.PointerDeviceKind getKindForPointer(long pointer)
    {
        DartRuntimePrimitives.Assert(() => this._pointerToEventData.ContainsKey(pointer));
        return this._pointerToEventData.GetValueOrDefault(pointer)!.kind;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getButtonsForPointer(long pointer)
    {
        DartRuntimePrimitives.Assert(() => this._pointerToEventData.ContainsKey(pointer));
        return this._pointerToEventData.GetValueOrDefault(pointer)!.buttons;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
    }

    public abstract string debugDescription { get; }
    public virtual T? invokeCallback<T>(string name, Func<T> callback, Func<string>? debugReport = null)
    {
        T? result__13841 = default!;
        try
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Generated.Framework.Gestures.DebugLibrary.debugPrintRecognizerCallbacksTrace)
                    {
                        string? report__13951 = ((debugReport is not null) ? debugReport() : null);
                        var prefix__14141 = (global::Doroti.Generated.Framework.Gestures.DebugLibrary.debugPrintGestureArenaDiagnostics ? $"{DartCoreExtensions.repeat(" ", 19L)}❙ " : "");
                        global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint($"{prefix__14141}{this} calling {name} callback.{((((report__13951 is null ? (bool?)null : report__13951.Length != 0) ?? false)) ? $" {report__13951}" : "")}");
                    }
                    return true;
                });
            result__13841 = callback();
        }
        catch (Exception exception__14428)
        {
            var stack__14439 = new System.Diagnostics.StackTrace();
            InformationCollector? collector__14476 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__14476 = (() => new List<DiagnosticsNode> { new StringProperty("Handler", name), new DiagnosticsProperty<GestureRecognizer>("Recognizer", this, style: DiagnosticsTreeStyle.errorProperty) });
                    return true;
                });
            FlutterError.reportError(new FlutterErrorDetails(exception: exception__14428, stack: stack__14439, library: "gesture", context: new ErrorDescription("while handling a gesture"), informationCollector: collector__14476));
        }
        return result__13841;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<object>("debugOwner", this.debugOwner, defaultValue: null));
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

public abstract class OneSequenceGestureRecognizer : GestureRecognizer
{
    public virtual DartMap<long, GestureArenaEntry> _entries { get; private set; } = new DartMap<long, GestureArenaEntry>();
    public virtual HashSet<long> _trackedPointers { get; private set; } = new HashSet<long>();
    internal virtual GestureArenaTeam? _team { get; set; } = default;

    protected OneSequenceGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        startTrackingPointer(@event.pointer, @event.transform);
    }

    public override void handleNonAllowedPointer(PointerDownEvent @event)
    {
        resolve(GestureDisposition.rejected);
    }

    public abstract void handleEvent(PointerEvent @event);
    public override void acceptGesture(long pointer)
    {
    }

    public override void rejectGesture(long pointer)
    {
    }

    public abstract void didStopTrackingLastPointer(long pointer);
    public virtual void resolve(GestureDisposition disposition)
    {
        var localEntries__17819 = new List<GestureArenaEntry>(DartRuntimePrimitives.ConvertEnumerable<GestureArenaEntry>(this._entries.Values));
        this._entries.Clear();
        foreach (var entry__17916 in localEntries__17819)
        {
            entry__17916.resolve(disposition);
        }
    }

    public virtual void resolvePointer(long pointer, GestureDisposition disposition)
    {
        GestureArenaEntry? entry__18222 = this._entries.GetValueOrDefault(pointer);
        if ((entry__18222 is not null))
        {
            this._entries.remove(pointer);
            entry__18222.resolve(disposition);
        }
    }

    public override void dispose()
    {
        resolve(GestureDisposition.rejected);
        foreach (long pointer__18443 in this._trackedPointers)
        {
            GestureBinding.instance.pointerRouter.removeRoute(pointer__18443, (Action<PointerEvent>)this.handleEvent);
        }
        this._trackedPointers.Clear();
        DartRuntimePrimitives.Assert(() => (checked((long)(this._entries.Count)) == 0));
        base.dispose();
    }

    public virtual GestureArenaTeam? team
    {
        get => this._team;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value is not null));
            DartRuntimePrimitives.Assert(() => (checked((long)(this._entries.Count)) == 0));
            DartRuntimePrimitives.Assert(() => (checked((long)(this._trackedPointers.Count)) == 0));
            DartRuntimePrimitives.Assert(() => (this._team is null));
            _team = __value;
        }
    }
    public virtual GestureArenaEntry _addPointerToArena(long pointer)
    {
        return (this._team?.add(pointer, this) ?? GestureBinding.instance.gestureArena.add(pointer, this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void startTrackingPointer(long pointer, Matrix4? transform = null)
    {
        GestureBinding.instance.pointerRouter.addRoute(pointer, (Action<PointerEvent>)this.handleEvent, transform);
        this._trackedPointers.Add(pointer);
        this._entries[pointer] = _addPointerToArena(pointer);
    }

    public virtual void stopTrackingPointer(long pointer)
    {
        if (this._trackedPointers.Contains(pointer))
        {
            GestureBinding.instance.pointerRouter.removeRoute(pointer, (Action<PointerEvent>)this.handleEvent);
            this._trackedPointers.Remove(pointer);
            if ((checked((long)(this._trackedPointers.Count)) == 0))
            {
                didStopTrackingLastPointer(pointer);
            }
        }
    }

    public virtual void stopTrackingIfPointerNoLongerDown(PointerEvent @event)
    {
        if ((((@event is PointerUpEvent) || (@event is PointerCancelEvent)) || (@event is PointerPanZoomEndEvent)))
        {
            stopTrackingPointer(((PointerEvent)@event).pointer);
        }
    }

}

public enum GestureRecognizerState
{
    ready,
    possible,
    defunct
}

public static partial class RecognizerLibrary
{
    internal static double _unsetTouchSlop = -1.0;
}

public abstract class PrimaryPointerGestureRecognizer : OneSequenceGestureRecognizer
{
    public virtual Duration? deadline { get; private set; }
    internal virtual double? _preAcceptSlopTolerance { get; private set; }
    internal virtual double? _postAcceptSlopTolerance { get; private set; }
    internal virtual GestureRecognizerState _state { get; set; } = GestureRecognizerState.ready;
    internal virtual long? _primaryPointer { get; set; } = default;
    internal virtual OffsetPair? _initialPosition { get; set; } = default;
    internal virtual bool _gestureAccepted { get; set; } = false;
    internal virtual Timer? _timer { get; set; } = default;

    protected PrimaryPointerGestureRecognizer(Duration? deadline = null, double? preAcceptSlopTolerance = null, double? postAcceptSlopTolerance = null, object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
        double? __preAcceptSlopTolerance = preAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop;
        double? __postAcceptSlopTolerance = postAcceptSlopTolerance ?? RecognizerLibrary._unsetTouchSlop;
        this.deadline = deadline;
        this._preAcceptSlopTolerance = __preAcceptSlopTolerance;
        this._postAcceptSlopTolerance = __postAcceptSlopTolerance;
        System.Diagnostics.Debug.Assert((((__preAcceptSlopTolerance == RecognizerLibrary._unsetTouchSlop) || (__preAcceptSlopTolerance is null)) || (__preAcceptSlopTolerance >= 0L)));
        System.Diagnostics.Debug.Assert((((__postAcceptSlopTolerance == RecognizerLibrary._unsetTouchSlop) || (__postAcceptSlopTolerance is null)) || (__postAcceptSlopTolerance >= 0L)));
    }

    public virtual double? preAcceptSlopTolerance => ((this._preAcceptSlopTolerance == RecognizerLibrary._unsetTouchSlop) ? this._defaultTouchSlop : this._preAcceptSlopTolerance);
    public virtual double? postAcceptSlopTolerance => ((this._postAcceptSlopTolerance == RecognizerLibrary._unsetTouchSlop) ? this._defaultTouchSlop : this._postAcceptSlopTolerance);
    internal virtual double _defaultTouchSlop => (gestureSettings?.touchSlop ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop);
    public virtual GestureRecognizerState state => this._state;
    public virtual long? primaryPointer => this._primaryPointer;
    public virtual OffsetPair? initialPosition => this._initialPosition;
    public override void addAllowedPointer(PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        if ((object.Equals(this.state, GestureRecognizerState.ready)))
        {
            _state = GestureRecognizerState.possible;
            _primaryPointer = @event.pointer;
            _initialPosition = new OffsetPair(local: @event.localPosition, global: @event.position);
            if ((this.deadline is not null))
            {
                Duration deadline__value27990 = DartRuntimePrimitives.RequireValue(deadline);
                _timer = new Timer(DartRuntimePrimitives.RequireValue(this.deadline), (() => didExceedDeadlineWithEvent(@event)));
            }
        }
    }

    public override void handleNonAllowedPointer(PointerDownEvent @event)
    {
        if (!this._gestureAccepted)
        {
            base.handleNonAllowedPointer(@event);
        }
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this.state, GestureRecognizerState.ready)));
        if (((object.Equals(this.state, GestureRecognizerState.possible)) && (((PointerEvent)@event).pointer == this.primaryPointer)))
        {
            bool isPreAcceptSlopPastTolerance__28466 = ((!this._gestureAccepted && (this.preAcceptSlopTolerance is not null)) && (_getGlobalDistance(@event) > DartRuntimePrimitives.RequireValue(this.preAcceptSlopTolerance)));
            bool isPostAcceptSlopPastTolerance__28652 = ((this._gestureAccepted && (this.postAcceptSlopTolerance is not null)) && (_getGlobalDistance(@event) > DartRuntimePrimitives.RequireValue(this.postAcceptSlopTolerance)));
            if (((@event is PointerMoveEvent) && ((isPreAcceptSlopPastTolerance__28466 || isPostAcceptSlopPastTolerance__28652))))
            {
                PointerMoveEvent @event__as28834 = (PointerMoveEvent)@event;
                resolve(GestureDisposition.rejected);
                stopTrackingPointer(DartRuntimePrimitives.RequireValue(this.primaryPointer));
            }
            else
            {
                handlePrimaryPointer(@event);
            }
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    public abstract void handlePrimaryPointer(PointerEvent @event);
    public virtual void didExceedDeadline()
    {
        DartRuntimePrimitives.Assert(() => (this.deadline is null));
    }

    public virtual void didExceedDeadlineWithEvent(PointerDownEvent @event)
    {
        didExceedDeadline();
    }

    public override void acceptGesture(long pointer)
    {
        if ((pointer == this.primaryPointer))
        {
            _stopTimer();
            _gestureAccepted = true;
        }
    }

    public override void rejectGesture(long pointer)
    {
        if (((pointer == this.primaryPointer) && (object.Equals(this.state, GestureRecognizerState.possible))))
        {
            _stopTimer();
            _state = GestureRecognizerState.defunct;
        }
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this.state, GestureRecognizerState.ready)));
        _stopTimer();
        _state = GestureRecognizerState.ready;
        _initialPosition = null;
        _gestureAccepted = false;
    }

    public override void dispose()
    {
        _stopTimer();
        base.dispose();
    }

    internal virtual void _stopTimer()
    {
        if ((this._timer is not null))
        {
            this._timer!.cancel();
            _timer = null;
        }
    }

    internal virtual double _getGlobalDistance(PointerEvent @event)
    {
        global::Doroti.Flutter.Ui.Offset offset__30873 = (((PointerEvent)@event).position - this.initialPosition!.global);
        return offset__30873.distance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<GestureRecognizerState>("state", this.state));
    }

}

public class OffsetPair
{
    public static OffsetPair zero = new OffsetPair(local: Offset.zero, global: Offset.zero);
    public virtual Offset local { get; private set; } = default!;
    public virtual Offset global { get; private set; } = default!;

    public OffsetPair(Offset local, Offset global)
    {
        this.local = local;
        this.global = global;
    }

    public static OffsetPair CreateFromEventPosition(PointerEvent @event)
    {
        var __instance = new OffsetPair(default!, default!);
        __instance.local = ((PointerEvent)@event).localPosition;
        __instance.global = ((PointerEvent)@event).position;
        return __instance;
    }

    public static OffsetPair CreateFromEventDelta(PointerEvent @event)
    {
        var __instance = new OffsetPair(default!, default!);
        __instance.local = ((PointerEvent)@event).localDelta;
        __instance.global = ((PointerEvent)@event).delta;
        return __instance;
    }

    public virtual OffsetPair op_Add(OffsetPair other)
    {
        return new OffsetPair(local: (this.local + ((OffsetPair)other).local), global: (this.global + ((OffsetPair)other).global));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual OffsetPair op_Subtract(OffsetPair other)
    {
        return new OffsetPair(local: (this.local - ((OffsetPair)other).local), global: (this.global - ((OffsetPair)other).global));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "OffsetPair"))}(local: {this.local}, global: {this.global})";
}

internal class _RecognizerEventData__recognizer
{
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual long buttons { get; private set; } = default!;

    internal _RecognizerEventData__recognizer(PointerDeviceKind kind, long buttons)
    {
        this.kind = kind;
        this.buttons = buttons;
    }

}

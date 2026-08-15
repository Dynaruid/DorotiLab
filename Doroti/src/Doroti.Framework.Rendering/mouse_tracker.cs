// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/mouse_tracker.dart
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

namespace Doroti.Generated.Framework.Rendering;

public delegate HitTestResult MouseTrackerHitTest(Offset offset, long viewId);

internal class _MouseState__mouse_tracker
{
    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _annotations { get; set; } = new DartMap<IMouseTrackerAnnotation, Matrix4>();
    internal virtual global::Doroti.Generated.Framework.Gestures.PointerEvent _latestEvent { get; set; } = default!;

    internal _MouseState__mouse_tracker(global::Doroti.Generated.Framework.Gestures.PointerEvent initialEvent)
    {
        this._latestEvent = initialEvent;
    }

    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> annotations => this._annotations;
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> replaceAnnotations(DartMap<IMouseTrackerAnnotation, Matrix4> value)
    {
        DartMap<IMouseTrackerAnnotation, Matrix4> previous__1249 = this._annotations;
        _annotations = value;
        return previous__1249;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Gestures.PointerEvent latestEvent => this._latestEvent;
    public virtual global::Doroti.Generated.Framework.Gestures.PointerEvent replaceLatestEvent(global::Doroti.Generated.Framework.Gestures.PointerEvent value)
    {
        DartRuntimePrimitives.Assert(() => (value.device == this._latestEvent.device));
        global::Doroti.Generated.Framework.Gestures.PointerEvent previous__1604 = this._latestEvent;
        _latestEvent = value;
        return previous__1604;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long device => this.latestEvent.device;
    public override string ToString()
    {
        var describeLatestEvent__1766 = $"latestEvent: {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.latestEvent))}";
        var describeAnnotations__1847 = $"annotations: [list of {checked((long)(this.annotations.Count))}]";
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({describeLatestEvent__1766}, {describeAnnotations__1847})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MouseTrackerUpdateDetails__mouse_tracker : Diagnosticable
{
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations { get; private set; } = default!;
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.PointerEvent? previousEvent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.PointerEvent? triggeringEvent { get; private set; }

    internal _MouseTrackerUpdateDetails__mouse_tracker(DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations, DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations, global::Doroti.Generated.Framework.Gestures.PointerEvent previousEvent)
    {
        this.lastAnnotations = lastAnnotations;
        this.nextAnnotations = nextAnnotations;
        this.previousEvent = previousEvent;
        this.triggeringEvent = null;
    }

    internal static _MouseTrackerUpdateDetails__mouse_tracker CreateByPointerEvent(DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations, DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations, global::Doroti.Generated.Framework.Gestures.PointerEvent? previousEvent = null, global::Doroti.Generated.Framework.Gestures.PointerEvent triggeringEvent = default!)
    {
        var __instance = new _MouseTrackerUpdateDetails__mouse_tracker(default!, default!, default!);
        __instance.lastAnnotations = lastAnnotations;
        __instance.nextAnnotations = nextAnnotations;
        __instance.previousEvent = previousEvent;
        __instance.triggeringEvent = triggeringEvent;
        return __instance;
    }

    public virtual long device
    {
        get
        {
            long result__4065 = ((this.previousEvent ?? this.triggeringEvent))!.device;
            return result__4065;
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Gestures.PointerEvent latestEvent
    {
        get
        {
            global::Doroti.Generated.Framework.Gestures.PointerEvent result__4307 = (this.triggeringEvent ?? this.previousEvent!);
            return result__4307;
            return default!;
        }
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("device", this.device));
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Gestures.PointerEvent>("previousEvent", this.previousEvent));
        properties.add(new DiagnosticsProperty<global::Doroti.Generated.Framework.Gestures.PointerEvent>("triggeringEvent", this.triggeringEvent));
        properties.add(new DiagnosticsProperty<DartMap<IMouseTrackerAnnotation, Matrix4>>("lastAnnotations", this.lastAnnotations));
        properties.add(new DiagnosticsProperty<DartMap<IMouseTrackerAnnotation, Matrix4>>("nextAnnotations", this.nextAnnotations));
    }

}

public class MouseTracker : ChangeNotifier
{
    internal virtual Func<Offset, long, HitTestResult> _hitTestInView { get; private set; } = default!;
    internal virtual MouseCursorManager _mouseCursorMixin { get; private set; } = new MouseCursorManager(SystemMouseCursors.basic);
    internal virtual DartMap<long, _MouseState__mouse_tracker> _mouseStates { get; private set; } = new DartMap<long, _MouseState__mouse_tracker>();
    internal virtual bool _debugDuringDeviceUpdate { get; set; } = false;

    public MouseTracker(Func<Offset, long, HitTestResult> hitTestInView)
    {
        this._hitTestInView = hitTestInView;
    }

    internal virtual void _monitorMouseConnection(Action task)
    {
        bool mouseWasConnected__6828 = this.mouseIsConnected;
        task();
        if ((mouseWasConnected__6828 != this.mouseIsConnected))
        {
            notifyListeners();
        }
    }

    internal virtual void _deviceUpdatePhase(Action task)
    {
        DartRuntimePrimitives.Assert(() => !this._debugDuringDeviceUpdate);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDuringDeviceUpdate = true;
                return true;
            });
        task();
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDuringDeviceUpdate = false;
                return true;
            });
    }

    internal static bool _shouldMarkStateDirty(_MouseState__mouse_tracker? state, global::Doroti.Generated.Framework.Gestures.PointerEvent @event)
    {
        if ((state is null))
        {
            return true;
        }
        global::Doroti.Generated.Framework.Gestures.PointerEvent lastEvent__7684 = ((_MouseState__mouse_tracker)state).latestEvent;
        DartRuntimePrimitives.Assert(() => (@event.device == lastEvent__7684.device));
        DartRuntimePrimitives.Assert(() => (((@event is global::Doroti.Generated.Framework.Gestures.PointerAddedEvent)) == ((lastEvent__7684 is global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent))));
        if ((@event is global::Doroti.Generated.Framework.Gestures.PointerSignalEvent))
        {
            global::Doroti.Generated.Framework.Gestures.PointerSignalEvent @event__as8007 = (global::Doroti.Generated.Framework.Gestures.PointerSignalEvent)@event;
            return false;
        }
        return (((lastEvent__7684 is global::Doroti.Generated.Framework.Gestures.PointerAddedEvent) || (@event is global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent)) || (!object.Equals(lastEvent__7684.position, @event.position)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _hitTestInViewResultToAnnotations(HitTestResult result)
    {
        var annotations__8307 = new DartMap<IMouseTrackerAnnotation, Matrix4>();
        foreach (HitTestEntry<HitTestTarget> entry__8386 in result.path)
        {
            object target__8429 = entry__8386.target;
            if ((target__8429 is IMouseTrackerAnnotation))
            {
                IMouseTrackerAnnotation target__8429__as8462 = (IMouseTrackerAnnotation)target__8429;
                annotations__8307[target__8429__as8462] = entry__8386.transform!;
            }
        }
        return annotations__8307;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _findAnnotations(_MouseState__mouse_tracker state)
    {
        global::Doroti.Ui.Offset globalPosition__8922 = ((_MouseState__mouse_tracker)state).latestEvent.position;
        long device__8981 = ((_MouseState__mouse_tracker)state).device;
        long viewId__9018 = ((_MouseState__mouse_tracker)state).latestEvent.viewId;
        if (!this._mouseStates.ContainsKey(device__8981))
        {
            return new DartMap<IMouseTrackerAnnotation, Matrix4>();
        }
        return _hitTestInViewResultToAnnotations(this._hitTestInView(globalPosition__8922, viewId__9018));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDeviceUpdate(_MouseTrackerUpdateDetails__mouse_tracker details)
    {
        DartRuntimePrimitives.Assert(() => this._debugDuringDeviceUpdate);
        _handleDeviceUpdateMouseEvents(details);
        this._mouseCursorMixin.handleDeviceCursorUpdate(((_MouseTrackerUpdateDetails__mouse_tracker)details).device, ((_MouseTrackerUpdateDetails__mouse_tracker)details).triggeringEvent, ((_MouseTrackerUpdateDetails__mouse_tracker)details).nextAnnotations.Keys.map<IMouseTrackerAnnotation, MouseCursor>(((annotation) => annotation.cursor)));
    }

    public virtual bool mouseIsConnected => (checked((long)(this._mouseStates.Count)) != 0);
    public virtual void updateWithEvent(global::Doroti.Generated.Framework.Gestures.PointerEvent @event, HitTestResult? hitTestResult)
    {
        if (((!object.Equals(@event.kind, PointerDeviceKind.mouse)) && (!object.Equals(@event.kind, PointerDeviceKind.stylus))))
        {
            return;
        }
        if ((@event is global::Doroti.Generated.Framework.Gestures.PointerSignalEvent))
        {
            global::Doroti.Generated.Framework.Gestures.PointerSignalEvent @event__as11595 = (global::Doroti.Generated.Framework.Gestures.PointerSignalEvent)@event;
            return;
        }
        HitTestResult result__11670 = (@event switch { global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent __object11702 => new HitTestResult(), _ => (hitTestResult ?? this._hitTestInView(@event.position, @event.viewId)) });
        long device__11839 = @event.device;
        _MouseState__mouse_tracker? existingState__11885 = this._mouseStates.GetValueOrDefault(device__11839);
        if (!_shouldMarkStateDirty(existingState__11885, @event))
        {
            return;
        }
        _monitorMouseConnection(((Action)(() =>
        {
            _deviceUpdatePhase(((Action)(() =>
            {
                if ((existingState__11885 is null))
                {
                    if ((@event is global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent))
                    {
                        global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent @event__as12312 = (global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent)@event;
                        return;
                    }
                    this._mouseStates[device__11839] = new _MouseState__mouse_tracker(initialEvent: @event);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (@event is not global::Doroti.Generated.Framework.Gestures.PointerAddedEvent));
                    if ((@event is global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent))
                    {
                        global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent @event__as12521 = (global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent)@event;
                        this._mouseStates.remove(@event__as12521.device);
                    }
                }
                _MouseState__mouse_tracker targetState__12648 = (this._mouseStates.GetValueOrDefault(device__11839) ?? existingState__11885!);
                global::Doroti.Generated.Framework.Gestures.PointerEvent lastEvent__12730 = targetState__12648.replaceLatestEvent(@event);
                DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations__12832 = ((@event is global::Doroti.Generated.Framework.Gestures.PointerRemovedEvent) ? new DartMap<IMouseTrackerAnnotation, Matrix4>() : _hitTestInViewResultToAnnotations(result__11670));
                DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations__13037 = targetState__12648.replaceAnnotations(nextAnnotations__12832);
                _handleDeviceUpdate(_MouseTrackerUpdateDetails__mouse_tracker.CreateByPointerEvent(lastAnnotations: lastAnnotations__13037, nextAnnotations: nextAnnotations__12832, previousEvent: lastEvent__12730, triggeringEvent: @event));
            })));
        })));
    }

    public virtual void updateAllDevices()
    {
        _deviceUpdatePhase(((Action)(() =>
        {
            foreach (_MouseState__mouse_tracker dirtyState__14024 in this._mouseStates.Values)
            {
                global::Doroti.Generated.Framework.Gestures.PointerEvent lastEvent__14088 = ((_MouseState__mouse_tracker)dirtyState__14024).latestEvent;
                DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations__14175 = _findAnnotations(dirtyState__14024);
                DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations__14274 = dirtyState__14024.replaceAnnotations(nextAnnotations__14175);
                _handleDeviceUpdate(new _MouseTrackerUpdateDetails__mouse_tracker(lastAnnotations: lastAnnotations__14274, nextAnnotations: nextAnnotations__14175, previousEvent: lastEvent__14088));
            }
        })));
    }

    public virtual MouseCursor? debugDeviceActiveCursor(long device)
    {
        return this._mouseCursorMixin.debugDeviceActiveCursor(device);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _handleDeviceUpdateMouseEvents(_MouseTrackerUpdateDetails__mouse_tracker details)
    {
        global::Doroti.Generated.Framework.Gestures.PointerEvent latestEvent__15215 = ((_MouseTrackerUpdateDetails__mouse_tracker)details).latestEvent;
        DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations__15298 = ((_MouseTrackerUpdateDetails__mouse_tracker)details).lastAnnotations;
        DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations__15388 = ((_MouseTrackerUpdateDetails__mouse_tracker)details).nextAnnotations;
        var baseExitEvent__15828 = global::Doroti.Generated.Framework.Gestures.PointerExitEvent.CreateFromMouseEvent(latestEvent__15215);
        lastAnnotations__15298.forEach(((annotation, transform) =>
        {
            if ((annotation.validForMouseTracker && !nextAnnotations__15388.ContainsKey(annotation)))
            {
                annotation.onExit?.Invoke(baseExitEvent__15828.transformed(lastAnnotations__15298.GetValueOrDefault(annotation)));
            }
        }));
        List<IMouseTrackerAnnotation> enteringAnnotations__16317 = nextAnnotations__15388.Keys.where(((annotation) => !lastAnnotations__15298.ContainsKey(annotation))).ToList();
        var baseEnterEvent__16485 = global::Doroti.Generated.Framework.Gestures.PointerEnterEvent.CreateFromMouseEvent(latestEvent__15215);
        foreach (IMouseTrackerAnnotation annotation__16587 in System.Linq.Enumerable.Reverse(enteringAnnotations__16317))
        {
            if (annotation__16587.validForMouseTracker)
            {
                annotation__16587.onEnter?.Invoke(baseEnterEvent__16485.transformed(nextAnnotations__15388.GetValueOrDefault(annotation__16587)));
            }
        }
    }

}

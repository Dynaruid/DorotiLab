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

namespace Doroti.Framework.Rendering;

public delegate HitTestResult MouseTrackerHitTest(Offset offset, long viewId);

internal class _MouseState__mouse_tracker
{
    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _annotations { get; set; } = new DartMap<IMouseTrackerAnnotation, Matrix4>();
    internal virtual global::Doroti.Framework.Gestures.PointerEvent _latestEvent { get; set; } = default!;

    internal _MouseState__mouse_tracker(global::Doroti.Framework.Gestures.PointerEvent initialEvent)
    {
        this._latestEvent = initialEvent;
    }

    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> annotations => this._annotations;
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> replaceAnnotations(DartMap<IMouseTrackerAnnotation, Matrix4> value)
    {
        DartMap<IMouseTrackerAnnotation, Matrix4> previous = this._annotations;
        _annotations = value;
        return previous;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Gestures.PointerEvent latestEvent => this._latestEvent;
    public virtual global::Doroti.Framework.Gestures.PointerEvent replaceLatestEvent(global::Doroti.Framework.Gestures.PointerEvent value)
    {
        DartRuntimePrimitives.Assert(() => (value.device == this._latestEvent.device));
        global::Doroti.Framework.Gestures.PointerEvent previous = this._latestEvent;
        _latestEvent = value;
        return previous;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long device => this.latestEvent.device;
    public override string ToString()
    {
        var describeLatestEvent = $"latestEvent: {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.latestEvent))}";
        var describeAnnotations = $"annotations: [list of {checked((long)(this.annotations.Count))}]";
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({describeLatestEvent}, {describeAnnotations})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MouseTrackerUpdateDetails__mouse_tracker : Diagnosticable
{
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations { get; private set; } = default!;
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.PointerEvent? previousEvent { get; private set; }
    public virtual global::Doroti.Framework.Gestures.PointerEvent? triggeringEvent { get; private set; }

    internal _MouseTrackerUpdateDetails__mouse_tracker(DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations, DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations, global::Doroti.Framework.Gestures.PointerEvent previousEvent)
    {
        this.lastAnnotations = lastAnnotations;
        this.nextAnnotations = nextAnnotations;
        this.previousEvent = previousEvent;
        this.triggeringEvent = null;
    }

    internal static _MouseTrackerUpdateDetails__mouse_tracker CreateByPointerEvent(DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations, DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations, global::Doroti.Framework.Gestures.PointerEvent? previousEvent = null, global::Doroti.Framework.Gestures.PointerEvent triggeringEvent = default!)
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
            long result = ((this.previousEvent ?? this.triggeringEvent))!.device;
            return result;
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Gestures.PointerEvent latestEvent
    {
        get
        {
            global::Doroti.Framework.Gestures.PointerEvent result = (this.triggeringEvent ?? this.previousEvent!);
            return result;
            return default!;
        }
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("device", this.device));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Gestures.PointerEvent>("previousEvent", this.previousEvent));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Gestures.PointerEvent>("triggeringEvent", this.triggeringEvent));
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
        bool mouseWasConnected = this.mouseIsConnected;
        task();
        if ((mouseWasConnected != this.mouseIsConnected))
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

    internal static bool _shouldMarkStateDirty(_MouseState__mouse_tracker? state, global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        if ((state is null))
        {
            return true;
        }
        global::Doroti.Framework.Gestures.PointerEvent lastEvent = ((_MouseState__mouse_tracker)state).latestEvent;
        DartRuntimePrimitives.Assert(() => (@event.device == lastEvent.device));
        DartRuntimePrimitives.Assert(() => (((@event is global::Doroti.Framework.Gestures.PointerAddedEvent)) == ((lastEvent is global::Doroti.Framework.Gestures.PointerRemovedEvent))));
        if ((@event is global::Doroti.Framework.Gestures.PointerSignalEvent))
        {
            global::Doroti.Framework.Gestures.PointerSignalEvent @event__as8007 = (global::Doroti.Framework.Gestures.PointerSignalEvent)@event;
            return false;
        }
        return (((lastEvent is global::Doroti.Framework.Gestures.PointerAddedEvent) || (@event is global::Doroti.Framework.Gestures.PointerRemovedEvent)) || (!object.Equals(lastEvent.position, @event.position)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _hitTestInViewResultToAnnotations(HitTestResult result)
    {
        var annotations = new DartMap<IMouseTrackerAnnotation, Matrix4>();
        foreach (HitTestEntry<HitTestTarget> entry in result.path)
        {
            object targetLocal = entry.target;
            if ((targetLocal is IMouseTrackerAnnotation))
            {
                IMouseTrackerAnnotation target__8429__as8462 = (IMouseTrackerAnnotation)targetLocal;
                annotations[target__8429__as8462] = entry.transform!;
            }
        }
        return annotations;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _findAnnotations(_MouseState__mouse_tracker state)
    {
        global::Doroti.Ui.Offset globalPosition = ((_MouseState__mouse_tracker)state).latestEvent.position;
        long deviceLocal = ((_MouseState__mouse_tracker)state).device;
        long viewIdLocal = ((_MouseState__mouse_tracker)state).latestEvent.viewId;
        if (!this._mouseStates.ContainsKey(deviceLocal))
        {
            return new DartMap<IMouseTrackerAnnotation, Matrix4>();
        }
        return _hitTestInViewResultToAnnotations(this._hitTestInView(globalPosition, viewIdLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDeviceUpdate(_MouseTrackerUpdateDetails__mouse_tracker details)
    {
        DartRuntimePrimitives.Assert(() => this._debugDuringDeviceUpdate);
        _handleDeviceUpdateMouseEvents(details);
        this._mouseCursorMixin.handleDeviceCursorUpdate(((_MouseTrackerUpdateDetails__mouse_tracker)details).device, ((_MouseTrackerUpdateDetails__mouse_tracker)details).triggeringEvent, ((_MouseTrackerUpdateDetails__mouse_tracker)details).nextAnnotations.Keys.map<IMouseTrackerAnnotation, MouseCursor>(((annotation) => annotation.cursor)));
    }

    public virtual bool mouseIsConnected => (checked((long)(this._mouseStates.Count)) != 0);
    public virtual void updateWithEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestResult? hitTestResult)
    {
        if (((!object.Equals(@event.kind, PointerDeviceKind.mouse)) && (!object.Equals(@event.kind, PointerDeviceKind.stylus))))
        {
            return;
        }
        if ((@event is global::Doroti.Framework.Gestures.PointerSignalEvent))
        {
            global::Doroti.Framework.Gestures.PointerSignalEvent @event__as11595 = (global::Doroti.Framework.Gestures.PointerSignalEvent)@event;
            return;
        }
        HitTestResult result = (@event switch { global::Doroti.Framework.Gestures.PointerRemovedEvent __object11702 => new HitTestResult(), _ => (hitTestResult ?? this._hitTestInView(@event.position, @event.viewId)) });
        long deviceLocal = @event.device;
        _MouseState__mouse_tracker? existingState = this._mouseStates.GetValueOrDefault(deviceLocal);
        if (!_shouldMarkStateDirty(existingState, @event))
        {
            return;
        }
        _monitorMouseConnection(((Action)(() =>
        {
            _deviceUpdatePhase(((Action)(() =>
            {
                if ((existingState is null))
                {
                    if ((@event is global::Doroti.Framework.Gestures.PointerRemovedEvent))
                    {
                        global::Doroti.Framework.Gestures.PointerRemovedEvent @event__as12312 = (global::Doroti.Framework.Gestures.PointerRemovedEvent)@event;
                        return;
                    }
                    this._mouseStates[deviceLocal] = new _MouseState__mouse_tracker(initialEvent: @event);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (@event is not global::Doroti.Framework.Gestures.PointerAddedEvent));
                    if ((@event is global::Doroti.Framework.Gestures.PointerRemovedEvent))
                    {
                        global::Doroti.Framework.Gestures.PointerRemovedEvent @event__as12521 = (global::Doroti.Framework.Gestures.PointerRemovedEvent)@event;
                        this._mouseStates.remove(@event__as12521.device);
                    }
                }
                _MouseState__mouse_tracker targetState = (this._mouseStates.GetValueOrDefault(deviceLocal) ?? existingState!);
                global::Doroti.Framework.Gestures.PointerEvent lastEvent = targetState.replaceLatestEvent(@event);
                DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotationsLocal = ((@event is global::Doroti.Framework.Gestures.PointerRemovedEvent) ? new DartMap<IMouseTrackerAnnotation, Matrix4>() : _hitTestInViewResultToAnnotations(result));
                DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotationsLocal = targetState.replaceAnnotations(nextAnnotationsLocal);
                _handleDeviceUpdate(_MouseTrackerUpdateDetails__mouse_tracker.CreateByPointerEvent(lastAnnotations: lastAnnotationsLocal, nextAnnotations: nextAnnotationsLocal, previousEvent: lastEvent, triggeringEvent: @event));
            })));
        })));
    }

    public virtual void updateAllDevices()
    {
        _deviceUpdatePhase(((Action)(() =>
        {
            foreach (_MouseState__mouse_tracker dirtyState in this._mouseStates.Values)
            {
                global::Doroti.Framework.Gestures.PointerEvent lastEvent = ((_MouseState__mouse_tracker)dirtyState).latestEvent;
                DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotationsLocal = _findAnnotations(dirtyState);
                DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotationsLocal = dirtyState.replaceAnnotations(nextAnnotationsLocal);
                _handleDeviceUpdate(new _MouseTrackerUpdateDetails__mouse_tracker(lastAnnotations: lastAnnotationsLocal, nextAnnotations: nextAnnotationsLocal, previousEvent: lastEvent));
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
        global::Doroti.Framework.Gestures.PointerEvent latestEventLocal = ((_MouseTrackerUpdateDetails__mouse_tracker)details).latestEvent;
        DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotationsLocal = ((_MouseTrackerUpdateDetails__mouse_tracker)details).lastAnnotations;
        DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotationsLocal = ((_MouseTrackerUpdateDetails__mouse_tracker)details).nextAnnotations;
        var baseExitEvent = global::Doroti.Framework.Gestures.PointerExitEvent.CreateFromMouseEvent(latestEventLocal);
        lastAnnotationsLocal.forEach(((annotation, transform) =>
        {
            if ((annotation.validForMouseTracker && !nextAnnotationsLocal.ContainsKey(annotation)))
            {
                annotation.onExit?.Invoke(baseExitEvent.transformed(lastAnnotationsLocal.GetValueOrDefault(annotation)));
            }
        }));
        List<IMouseTrackerAnnotation> enteringAnnotations = nextAnnotationsLocal.Keys.where(((annotation) => !lastAnnotationsLocal.ContainsKey(annotation))).ToList();
        var baseEnterEvent = global::Doroti.Framework.Gestures.PointerEnterEvent.CreateFromMouseEvent(latestEventLocal);
        foreach (IMouseTrackerAnnotation annotationLocal in System.Linq.Enumerable.Reverse(enteringAnnotations))
        {
            if (annotationLocal.validForMouseTracker)
            {
                annotationLocal.onEnter?.Invoke(baseEnterEvent.transformed(nextAnnotationsLocal.GetValueOrDefault(annotationLocal)));
            }
        }
    }

}

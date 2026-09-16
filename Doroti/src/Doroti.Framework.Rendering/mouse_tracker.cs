// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/mouse_tracker.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public delegate HitTestResult MouseTrackerHitTest(Offset offset, long viewId);

internal class _MouseState__mouse_tracker
{
    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _annotations { get; set; } = new DartMap<IMouseTrackerAnnotation, Matrix4>();
    internal virtual global::Doroti.Framework.Gestures.PointerEvent _latestEvent { get; set; } = default!;

    internal _MouseState__mouse_tracker(global::Doroti.Framework.Gestures.PointerEvent initialEvent)
    {
        _latestEvent = initialEvent;
    }

    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> annotations => _annotations;
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> replaceAnnotations(DartMap<IMouseTrackerAnnotation, Matrix4> value)
    {
        DartMap<IMouseTrackerAnnotation, Matrix4> previous = _annotations;
        _annotations = value;
        return previous;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Gestures.PointerEvent latestEvent => _latestEvent;
    public virtual global::Doroti.Framework.Gestures.PointerEvent replaceLatestEvent(global::Doroti.Framework.Gestures.PointerEvent value)
    {
        DartRuntimePrimitives.Assert(() => value.device == _latestEvent.device);
        global::Doroti.Framework.Gestures.PointerEvent previous = _latestEvent;
        _latestEvent = value;
        return previous;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long device => latestEvent.device;
    public override string ToString()
    {
        var describeLatestEvent = $"latestEvent: {DiagnosticsLibrary.describeIdentity(latestEvent)}";
        var describeAnnotations = $"annotations: [list of {checked((long)annotations.Count)}]";
        return $"{DiagnosticsLibrary.describeIdentity(this)}({describeLatestEvent}, {describeAnnotations})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MouseTrackerUpdateDetails__mouse_tracker : Diagnosticable
{
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations { get; private set; } = default!;
    public virtual DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.PointerEvent? previousEvent { get; private set; }
    public virtual global::Doroti.Framework.Gestures.PointerEvent? triggeringEvent { get; private set; }

    internal _MouseTrackerUpdateDetails__mouse_tracker(DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations, DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations, global::Doroti.Framework.Gestures.PointerEvent? previousEvent)
    {
        this.lastAnnotations = lastAnnotations;
        this.nextAnnotations = nextAnnotations;
        this.previousEvent = previousEvent;
        triggeringEvent = null;
    }

    internal static _MouseTrackerUpdateDetails__mouse_tracker CreateByPointerEvent(DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotations, DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotations, global::Doroti.Framework.Gestures.PointerEvent? previousEvent = null, global::Doroti.Framework.Gestures.PointerEvent triggeringEvent = default!)
    {
        var __instance = new _MouseTrackerUpdateDetails__mouse_tracker(lastAnnotations, nextAnnotations, previousEvent);
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
            long result = (previousEvent ?? triggeringEvent)!.device;
            return result;
        }
    }
    public virtual global::Doroti.Framework.Gestures.PointerEvent latestEvent
    {
        get
        {
            global::Doroti.Framework.Gestures.PointerEvent result = triggeringEvent ?? previousEvent!;
            return result;
        }
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IntProperty("device", device));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Gestures.PointerEvent>("previousEvent", previousEvent));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Gestures.PointerEvent>("triggeringEvent", triggeringEvent));
        properties.add(new DiagnosticsProperty<DartMap<IMouseTrackerAnnotation, Matrix4>>("lastAnnotations", lastAnnotations));
        properties.add(new DiagnosticsProperty<DartMap<IMouseTrackerAnnotation, Matrix4>>("nextAnnotations", nextAnnotations));
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
        _hitTestInView = hitTestInView;
    }

    internal virtual void _monitorMouseConnection(Action task)
    {
        bool mouseWasConnected = mouseIsConnected;
        task();
        if (mouseWasConnected != mouseIsConnected)
        {
            notifyListeners();
        }
    }

    internal virtual void _deviceUpdatePhase(Action task)
    {
        DartRuntimePrimitives.Assert(() => !_debugDuringDeviceUpdate);
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
        if (state is null)
        {
            return true;
        }
        global::Doroti.Framework.Gestures.PointerEvent lastEvent = state.latestEvent;
        DartRuntimePrimitives.Assert(() => @event.device == lastEvent.device);
        DartRuntimePrimitives.Assert(() => @event is global::Doroti.Framework.Gestures.PointerAddedEvent == lastEvent is global::Doroti.Framework.Gestures.PointerRemovedEvent);
        if (@event is global::Doroti.Framework.Gestures.PointerSignalEvent)
        {
            global::Doroti.Framework.Gestures.PointerSignalEvent @event__as8007 = (global::Doroti.Framework.Gestures.PointerSignalEvent)@event;
            return false;
        }
        return (lastEvent is global::Doroti.Framework.Gestures.PointerAddedEvent) || (@event is global::Doroti.Framework.Gestures.PointerRemovedEvent) || (!Equals(lastEvent.position, @event.position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DartMap<IMouseTrackerAnnotation, Matrix4> _hitTestInViewResultToAnnotations(HitTestResult result)
    {
        var annotations = new DartMap<IMouseTrackerAnnotation, Matrix4>();
        foreach (HitTestEntry<HitTestTarget> entry in result.path)
        {
            object targetLocal = entry.target;
            if (targetLocal is IMouseTrackerAnnotation)
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
        global::Doroti.Ui.Offset globalPosition = state.latestEvent.position;
        long deviceLocal = state.device;
        long viewIdLocal = state.latestEvent.viewId;
        if (!_mouseStates.ContainsKey(deviceLocal))
        {
            return new DartMap<IMouseTrackerAnnotation, Matrix4>();
        }
        return _hitTestInViewResultToAnnotations(_hitTestInView(globalPosition, viewIdLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDeviceUpdate(_MouseTrackerUpdateDetails__mouse_tracker details)
    {
        DartRuntimePrimitives.Assert(() => _debugDuringDeviceUpdate);
        _handleDeviceUpdateMouseEvents(details);
        _mouseCursorMixin.handleDeviceCursorUpdate(details.device, details.triggeringEvent, details.nextAnnotations.Keys.map<IMouseTrackerAnnotation, MouseCursor>((annotation) => annotation.cursor));
    }

    public virtual bool mouseIsConnected => checked((long)_mouseStates.Count) != 0;
    public virtual void updateWithEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestResult? hitTestResult)
    {
        if ((!Equals(@event.kind, PointerDeviceKind.mouse)) && (!Equals(@event.kind, PointerDeviceKind.stylus)))
        {
            return;
        }
        if (@event is global::Doroti.Framework.Gestures.PointerSignalEvent)
        {
            global::Doroti.Framework.Gestures.PointerSignalEvent @event__as11595 = (global::Doroti.Framework.Gestures.PointerSignalEvent)@event;
            return;
        }
        HitTestResult result = @event switch { global::Doroti.Framework.Gestures.PointerRemovedEvent __object11702 => new HitTestResult(), _ => hitTestResult ?? _hitTestInView(@event.position, @event.viewId) };
        long deviceLocal = @event.device;
        _MouseState__mouse_tracker? existingState = _mouseStates.GetValueOrDefault(deviceLocal);
        if (!_shouldMarkStateDirty(existingState, @event))
        {
            return;
        }
        _monitorMouseConnection(() =>
        {
            _deviceUpdatePhase(() =>
            {
                if (existingState is null)
                {
                    if (@event is global::Doroti.Framework.Gestures.PointerRemovedEvent)
                    {
                        global::Doroti.Framework.Gestures.PointerRemovedEvent @event__as12312 = (global::Doroti.Framework.Gestures.PointerRemovedEvent)@event;
                        return;
                    }
                    _mouseStates[deviceLocal] = new _MouseState__mouse_tracker(initialEvent: @event);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => @event is not PointerAddedEvent);
                    if (@event is global::Doroti.Framework.Gestures.PointerRemovedEvent)
                    {
                        global::Doroti.Framework.Gestures.PointerRemovedEvent @event__as12521 = (global::Doroti.Framework.Gestures.PointerRemovedEvent)@event;
                        _mouseStates.remove(@event__as12521.device);
                    }
                }
                _MouseState__mouse_tracker targetState = _mouseStates.GetValueOrDefault(deviceLocal) ?? existingState!;
                global::Doroti.Framework.Gestures.PointerEvent lastEvent = targetState.replaceLatestEvent(@event);
                DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotationsLocal = (@event is global::Doroti.Framework.Gestures.PointerRemovedEvent) ? new DartMap<IMouseTrackerAnnotation, Matrix4>() : _hitTestInViewResultToAnnotations(result);
                DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotationsLocal = targetState.replaceAnnotations(nextAnnotationsLocal);
                _handleDeviceUpdate(_MouseTrackerUpdateDetails__mouse_tracker.CreateByPointerEvent(lastAnnotations: lastAnnotationsLocal, nextAnnotations: nextAnnotationsLocal, previousEvent: lastEvent, triggeringEvent: @event));
            });
        });
    }

    public virtual void updateAllDevices()
    {
        _deviceUpdatePhase(() =>
        {
            foreach (_MouseState__mouse_tracker dirtyState in _mouseStates.Values)
            {
                global::Doroti.Framework.Gestures.PointerEvent lastEvent = dirtyState.latestEvent;
                DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotationsLocal = _findAnnotations(dirtyState);
                DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotationsLocal = dirtyState.replaceAnnotations(nextAnnotationsLocal);
                _handleDeviceUpdate(new _MouseTrackerUpdateDetails__mouse_tracker(lastAnnotations: lastAnnotationsLocal, nextAnnotations: nextAnnotationsLocal, previousEvent: lastEvent));
            }
        });
    }

    public virtual MouseCursor? debugDeviceActiveCursor(long device)
    {
        return _mouseCursorMixin.debugDeviceActiveCursor(device);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _handleDeviceUpdateMouseEvents(_MouseTrackerUpdateDetails__mouse_tracker details)
    {
        global::Doroti.Framework.Gestures.PointerEvent latestEventLocal = details.latestEvent;
        DartMap<IMouseTrackerAnnotation, Matrix4> lastAnnotationsLocal = details.lastAnnotations;
        DartMap<IMouseTrackerAnnotation, Matrix4> nextAnnotationsLocal = details.nextAnnotations;
        var baseExitEvent = Gestures.PointerExitEvent.CreateFromMouseEvent(latestEventLocal);
        lastAnnotationsLocal.forEach((annotation, transform) =>
        {
            if (annotation.validForMouseTracker && !nextAnnotationsLocal.ContainsKey(annotation))
            {
                annotation.onExit?.Invoke(baseExitEvent.transformed(lastAnnotationsLocal.GetValueOrDefault(annotation)));
            }
        });
        List<IMouseTrackerAnnotation> enteringAnnotations = nextAnnotationsLocal.Keys.where((annotation) => !lastAnnotationsLocal.ContainsKey(annotation)).ToList();
        var baseEnterEvent = Gestures.PointerEnterEvent.CreateFromMouseEvent(latestEventLocal);
        foreach (IMouseTrackerAnnotation annotationLocal in Enumerable.Reverse(enteringAnnotations))
        {
            if (annotationLocal.validForMouseTracker)
            {
                annotationLocal.onEnter?.Invoke(baseEnterEvent.transformed(nextAnnotationsLocal.GetValueOrDefault(annotationLocal)));
            }
        }
    }

}

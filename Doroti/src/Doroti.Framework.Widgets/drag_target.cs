// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/drag_target.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate bool DragTargetWillAccept<T>(T? data);

public delegate bool DragTargetWillAcceptWithDetails<T>(DragTargetDetails<T> details);

public delegate void DragTargetAccept<T>(T data);

public delegate void DragTargetAcceptWithDetails<T>(DragTargetDetails<T> details);

public delegate Widget DragTargetBuilder<T>(BuildContext context, List<T?> candidateData, List<object> rejectedData);

public delegate void DragUpdateCallback(DragUpdateDetails details);

public delegate void DraggableCanceledCallback(Velocity velocity, Offset offset);

public delegate void DragEndCallback(DraggableDetails details);

public delegate void DragTargetLeave<T>(T? data);

public delegate void DragTargetMove<T>(DragTargetDetails<T> details);

public delegate Offset DragAnchorStrategy(Draggable<object> draggable, BuildContext context, Offset position);

public static partial class Drag_targetLibrary
{
    public static Offset childDragAnchorStrategy<T>(Draggable<T> draggable, BuildContext context, Offset position)
    {
        var renderObject = ((RenderBox?)context.findRenderObject()!)!;
        return renderObject.globalToLocal(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Drag_targetLibrary
{
    public static Offset pointerDragAnchorStrategy<T>(Draggable<T> draggable, BuildContext context, Offset position)
    {
        return Offset.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class Draggable<T> : StatefulWidget
{
    public virtual T? data { get; private set; }
    public virtual Axis? axis { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget? childWhenDragging { get; private set; }
    public virtual Widget feedback { get; private set; } = default!;
    public virtual Offset feedbackOffset { get; private set; } = default!;
    public virtual Func<Draggable<T>, BuildContext, Offset, Offset> dragAnchorStrategy { get; private set; } = default!;
    public virtual bool ignoringFeedbackSemantics { get; private set; } = default!;
    public virtual bool ignoringFeedbackPointer { get; private set; } = default!;
    public virtual Axis? affinity { get; private set; }
    public virtual long? maxSimultaneousDrags { get; private set; }
    public virtual Action? onDragStarted { get; private set; }
    public virtual System.Action<DragUpdateDetails>? onDragUpdate { get; private set; }
    public virtual Action<Velocity, Offset>? onDraggableCanceled { get; private set; }
    public virtual Action? onDragCompleted { get; private set; }
    public virtual System.Action<DraggableDetails>? onDragEnd { get; private set; }
    public virtual bool rootOverlay { get; private set; } = default!;
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual Func<long, bool>? allowedButtonsFilter { get; private set; }

    public Draggable(Key? key = null, Widget child = default!, Widget feedback = default!, T? data = default, Axis? axis = null, Widget? childWhenDragging = null, Offset feedbackOffset = default, Func<Draggable<T>, BuildContext, Offset, Offset> dragAnchorStrategy = default!, Axis? affinity = null, long? maxSimultaneousDrags = null, Action? onDragStarted = null, System.Action<DragUpdateDetails>? onDragUpdate = null, Action<Velocity, Offset>? onDraggableCanceled = null, System.Action<DraggableDetails>? onDragEnd = null, Action? onDragCompleted = null, bool ignoringFeedbackSemantics = true, bool ignoringFeedbackPointer = true, bool rootOverlay = false, HitTestBehavior hitTestBehavior = HitTestBehavior.deferToChild, Func<long, bool>? allowedButtonsFilter = null) : base(key: key)
    {
        Func<Draggable<T>, BuildContext, Offset, Offset> __dragAnchorStrategy = dragAnchorStrategy ?? Drag_targetLibrary.childDragAnchorStrategy;
        this.child = child;
        this.feedback = feedback;
        this.data = data;
        this.axis = axis;
        this.childWhenDragging = childWhenDragging;
        this.feedbackOffset = feedbackOffset;
        this.dragAnchorStrategy = __dragAnchorStrategy;
        this.affinity = affinity;
        this.maxSimultaneousDrags = maxSimultaneousDrags;
        this.onDragStarted = onDragStarted;
        this.onDragUpdate = onDragUpdate;
        this.onDraggableCanceled = onDraggableCanceled;
        this.onDragEnd = onDragEnd;
        this.onDragCompleted = onDragCompleted;
        this.ignoringFeedbackSemantics = ignoringFeedbackSemantics;
        this.ignoringFeedbackPointer = ignoringFeedbackPointer;
        this.rootOverlay = rootOverlay;
        this.hitTestBehavior = hitTestBehavior;
        this.allowedButtonsFilter = allowedButtonsFilter;
        System.Diagnostics.Debug.Assert((maxSimultaneousDrags is null) || (maxSimultaneousDrags >= 0L));
    }

    public virtual MultiDragGestureRecognizer createRecognizer(Func<Offset, Drag?> onStart)
    {
        return ((Func<MultiDragGestureRecognizer>)(() =>
{
    var __cascade = affinity switch { Axis.horizontal => DartRuntimePrimitives.ConvertValue<MultiDragGestureRecognizer>(new HorizontalMultiDragGestureRecognizer(allowedButtonsFilter: allowedButtonsFilter)), Axis.vertical => DartRuntimePrimitives.ConvertValue<MultiDragGestureRecognizer>(new VerticalMultiDragGestureRecognizer(allowedButtonsFilter: allowedButtonsFilter)), null => DartRuntimePrimitives.ConvertValue<MultiDragGestureRecognizer>(new ImmediateMultiDragGestureRecognizer(allowedButtonsFilter: allowedButtonsFilter)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    __cascade.onStart = onStart;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DraggableState__drag_target<T>());
}

public class LongPressDraggable<T> : Draggable<T>
{
    public virtual bool hapticFeedbackOnStart { get; private set; } = default!;
    public virtual Duration delay { get; private set; } = default!;

    public LongPressDraggable(Key? key = null, Widget child = default!, Widget feedback = default!, T? data = default, Axis? axis = null, Widget? childWhenDragging = null, Offset feedbackOffset = default, Func<Draggable<T>, BuildContext, Offset, Offset> dragAnchorStrategy = default!, long? maxSimultaneousDrags = null, Action? onDragStarted = null, System.Action<DragUpdateDetails>? onDragUpdate = null, Action<Velocity, Offset>? onDraggableCanceled = null, System.Action<DraggableDetails>? onDragEnd = null, Action? onDragCompleted = null, bool hapticFeedbackOnStart = true, bool ignoringFeedbackSemantics = true, bool ignoringFeedbackPointer = true, Duration? delay = null, Func<long, bool>? allowedButtonsFilter = null, HitTestBehavior hitTestBehavior = HitTestBehavior.deferToChild, bool rootOverlay = false) : base(key: key, child: child, feedback: feedback, data: data, axis: DartRuntimePrimitives.RequireValue(axis), childWhenDragging: childWhenDragging, feedbackOffset: feedbackOffset, dragAnchorStrategy: dragAnchorStrategy ?? Drag_targetLibrary.childDragAnchorStrategy, maxSimultaneousDrags: DartRuntimePrimitives.RequireValue(maxSimultaneousDrags), onDragStarted: onDragStarted, onDragUpdate: onDragUpdate, onDraggableCanceled: onDraggableCanceled, onDragEnd: onDragEnd, onDragCompleted: onDragCompleted, ignoringFeedbackSemantics: ignoringFeedbackSemantics, ignoringFeedbackPointer: ignoringFeedbackPointer, allowedButtonsFilter: allowedButtonsFilter, hitTestBehavior: hitTestBehavior, rootOverlay: rootOverlay)
    {
        Duration __delay = delay ?? Gestures.ConstantsLibrary.kLongPressTimeout;
        this.hapticFeedbackOnStart = hapticFeedbackOnStart;
        this.delay = __delay;
    }

    public override DelayedMultiDragGestureRecognizer createRecognizer(Func<Offset, Drag?> onStart)
    {
        return ((Func<DelayedMultiDragGestureRecognizer>)(() =>
{
    var __cascade = new DelayedMultiDragGestureRecognizer(delay: DartRuntimePrimitives.RequireValue(delay), allowedButtonsFilter: allowedButtonsFilter);
    __cascade.onStart = (position) =>
    {
        Drag? result = onStart(position);
        if ((result is not null) && hapticFeedbackOnStart)
        {
            DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
        }
        return result;
        throw new InvalidOperationException("Dart closure completed without a value.");
    };
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DraggableState__drag_target<T> : State<Draggable<T>>
{
    internal virtual GestureRecognizer? _recognizer { get; set; } = default;
    internal virtual long _activeCount { get; set; } = 0L;

    public override void initState()
    {
        base.initState();
        _recognizer = DartRuntimePrimitives.ConvertValue<GestureRecognizer>(widget.createRecognizer((Func<Offset, _DragAvatar__drag_target<T>?>)_startDrag));
    }

    public override void dispose()
    {
        _disposeRecognizerIfInactive();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        _recognizer!.gestureSettings = MediaQuery.maybeGestureSettingsOf(context);
        base.didChangeDependencies();
    }

    internal virtual void _disposeRecognizerIfInactive()
    {
        if (_activeCount > 0L)
        {
            return;
        }
        _recognizer!.dispose();
        _recognizer = null;
    }

    internal virtual void _routePointer(Gestures.PointerDownEvent @event)
    {
        if ((widget.maxSimultaneousDrags is not null) && (_activeCount >= DartRuntimePrimitives.RequireValue(widget.maxSimultaneousDrags)))
        {
            return;
        }
        _recognizer!.addPointer(@event);
    }

    internal virtual _DragAvatar__drag_target<T>? _startDrag(Offset position)
    {
        if ((widget.maxSimultaneousDrags is not null) && (_activeCount >= DartRuntimePrimitives.RequireValue(widget.maxSimultaneousDrags)))
        {
            return default;
        }
        Offset dragStartPointLocal = default!;
        dragStartPointLocal = widget.dragAnchorStrategy(widget, context, position);
        setState(() =>
        {
            _activeCount += 1L;
        });
        var avatar = new _DragAvatar__drag_target<T>(overlayState: Overlay.of(context, debugRequiredFor: widget, rootOverlay: widget.rootOverlay), data: widget.data, axis: widget.axis, initialPosition: position, dragStartPoint: dragStartPointLocal, feedback: widget.feedback, feedbackOffset: widget.feedbackOffset, ignoringFeedbackSemantics: widget.ignoringFeedbackSemantics, ignoringFeedbackPointer: widget.ignoringFeedbackPointer, viewId: checked((long)View.of(context).viewId), onDragUpdate: (details) =>
        {
            if (mounted && (widget.onDragUpdate is not null))
            {
                widget.onDragUpdate!(details);
            }
        }, onDragEnd: (velocity, offset, wasAccepted) =>
        {
            if (mounted)
            {
                setState(() =>
                {
                    _activeCount -= 1L;
                });
            }
            else
            {
                _activeCount -= 1L;
                _disposeRecognizerIfInactive();
            }
            if (mounted && (widget.onDragEnd is not null))
            {
                widget.onDragEnd!(new DraggableDetails(wasAccepted: wasAccepted, velocity: velocity, offset: offset));
            }
            if (wasAccepted && (widget.onDragCompleted is not null))
            {
                widget.onDragCompleted!();
            }
            if (!wasAccepted && (widget.onDraggableCanceled is not null))
            {
                widget.onDraggableCanceled!(velocity, offset);
            }
        });
        widget.onDragStarted?.Invoke();
        return avatar;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasOverlay(context));
        bool canDrag = (widget.maxSimultaneousDrags is null) || (_activeCount < DartRuntimePrimitives.RequireValue(widget.maxSimultaneousDrags));
        bool showChild = (_activeCount == 0L) || (widget.childWhenDragging is null);
        return new Listener(behavior: widget.hitTestBehavior, onPointerDown: canDrag ? _routePointer : null, child: showChild ? widget.child : widget.childWhenDragging);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DraggableDetails
{
    public virtual bool wasAccepted { get; private set; } = default!;
    public virtual Velocity velocity { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;

    public DraggableDetails(bool wasAccepted = false, Velocity velocity = default!, Offset offset = default!)
    {
        this.wasAccepted = wasAccepted;
        this.velocity = velocity;
        this.offset = offset;
    }

}

public class DragTargetDetails<T>
{
    public virtual T data { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;

    public DragTargetDetails(T data, Offset offset)
    {
        this.data = data;
        this.offset = offset;
    }

}

public class DragTarget<T> : StatefulWidget
{
    public virtual Func<BuildContext, List<T?>, List<object?>, Widget> builder { get; private set; } = default!;
    public virtual Func<T?, bool>? onWillAccept { get; private set; }
    public virtual Func<DragTargetDetails<T>, bool>? onWillAcceptWithDetails { get; private set; }
    public virtual System.Action<T>? onAccept { get; private set; }
    public virtual System.Action<DragTargetDetails<T>>? onAcceptWithDetails { get; private set; }
    public virtual System.Action<T?>? onLeave { get; private set; }
    public virtual System.Action<DragTargetDetails<T>>? onMove { get; private set; }
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;

    public DragTarget(Key? key = null, Func<BuildContext, List<T?>, List<object?>, Widget> builder = default!, Func<T?, bool>? onWillAccept = null, Func<DragTargetDetails<T>, bool>? onWillAcceptWithDetails = null, System.Action<T>? onAccept = null, System.Action<DragTargetDetails<T>>? onAcceptWithDetails = null, System.Action<T?>? onLeave = null, System.Action<DragTargetDetails<T>>? onMove = null, HitTestBehavior hitTestBehavior = HitTestBehavior.translucent) : base(key: key)
    {
        this.builder = builder;
        this.onWillAccept = onWillAccept;
        this.onWillAcceptWithDetails = onWillAcceptWithDetails;
        this.onAccept = onAccept;
        this.onAcceptWithDetails = onAcceptWithDetails;
        this.onLeave = onLeave;
        this.onMove = onMove;
        this.hitTestBehavior = hitTestBehavior;
        System.Diagnostics.Debug.Assert((onWillAccept is null) || (onWillAcceptWithDetails is null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DragTargetState__drag_target<T>());
}

public static partial class Drag_targetLibrary
{
    internal static List<T?> _mapAvatarsToData<T>(List<IDragAvatar> avatars)
    {
        return avatars.map((avatar) => (T?)avatar.data).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal interface IDragAvatar
{
    object? data { get; }
    Offset? lastOffset { get; }
}

internal interface IDragTargetState
{
    bool isExpectedDataType(object? data, Type type);
    bool didEnter(IDragAvatar avatar);
    void didLeave(IDragAvatar avatar);
    void didDrop(IDragAvatar avatar);
    void didMove(IDragAvatar avatar);
}

internal class _DragTargetState__drag_target<T> : State<DragTarget<T>>, IDragTargetState
{
    internal virtual List<IDragAvatar> _candidateAvatars { get; private set; } = new List<IDragAvatar>();
    internal virtual List<IDragAvatar> _rejectedAvatars { get; private set; } = new List<IDragAvatar>();

    public virtual bool isExpectedDataType(object? data, Type type)
    {
        if (Foundation.ConstantsLibrary.kIsWeb && (Equals(type, typeof(long)) && Equals(typeof(T), typeof(double)) || Equals(type, typeof(double)) && Equals(typeof(T), typeof(long))))
        {
            return false;
        }
        return data is T || (data is null && default(T) is null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didEnter(IDragAvatar avatar)
    {
        DartRuntimePrimitives.Assert(() => !_candidateAvatars.Contains(avatar));
        DartRuntimePrimitives.Assert(() => !_rejectedAvatars.Contains(avatar));
        bool resolvedWillAccept = (widget.onWillAccept is null) && (widget.onWillAcceptWithDetails is null) || (widget.onWillAccept is not null) && widget.onWillAccept!(((T?)avatar.data)!) || (widget.onWillAcceptWithDetails is not null) && (avatar.data is not null) && widget.onWillAcceptWithDetails!(new DragTargetDetails<T>(data: ((T?)avatar.data!)!, offset: DartRuntimePrimitives.RequireValue(avatar.lastOffset)));
        if (resolvedWillAccept)
        {
            setState(() =>
            {
                _candidateAvatars.Add(avatar);
            });
            return true;
        }
        else
        {
            setState(() =>
            {
                _rejectedAvatars.Add(avatar);
            });
            return false;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave(IDragAvatar avatar)
    {
        DartRuntimePrimitives.Assert(() => _candidateAvatars.Contains(avatar) || _rejectedAvatars.Contains(avatar));
        if (!mounted)
        {
            return;
        }
        setState(() =>
        {
            _candidateAvatars.Remove(avatar);
            _rejectedAvatars.Remove(avatar);
        });
        widget.onLeave?.Invoke(((T?)avatar.data)!);
    }

    public virtual void didDrop(IDragAvatar avatar)
    {
        DartRuntimePrimitives.Assert(() => _candidateAvatars.Contains(avatar));
        if (!mounted)
        {
            return;
        }
        setState(() =>
        {
            _candidateAvatars.Remove(avatar);
        });
        if (avatar.data is not null)
        {
            widget.onAccept?.Invoke(((T?)avatar.data!)!);
            widget.onAcceptWithDetails?.Invoke(new DragTargetDetails<T>(data: ((T?)avatar.data!)!, offset: DartRuntimePrimitives.RequireValue(avatar.lastOffset)));
        }
    }

    public virtual void didMove(IDragAvatar avatar)
    {
        if (!mounted || (avatar.data is null))
        {
            return;
        }
        widget.onMove?.Invoke(new DragTargetDetails<T>(data: ((T?)avatar.data!)!, offset: DartRuntimePrimitives.RequireValue(avatar.lastOffset)));
    }

    public override Widget build(BuildContext context)
    {
        return new MetaData(metaData: this, behavior: widget.hitTestBehavior, child: widget.builder(context, Drag_targetLibrary._mapAvatarsToData<T>(_candidateAvatars), Drag_targetLibrary._mapAvatarsToData<object>(_rejectedAvatars)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _DragEndKind__drag_target
{
    dropped,
    canceled
}

internal delegate void _OnDragEnd__drag_target(Velocity velocity, Offset offset, bool wasAccepted);

public class _DragAvatar__drag_target<T> : Drag, IDragAvatar
{
    object? IDragAvatar.data => data;
    Offset? IDragAvatar.lastOffset => _lastOffset;

    public virtual T? data { get; private set; }
    public virtual Axis? axis { get; private set; }
    public virtual Offset dragStartPoint { get; private set; } = default!;
    public virtual Widget? feedback { get; private set; }
    public virtual Offset feedbackOffset { get; private set; } = default!;
    public virtual System.Action<DragUpdateDetails>? onDragUpdate { get; private set; }
    public virtual Action<Velocity, Offset, bool>? onDragEnd { get; private set; }
    public virtual OverlayState overlayState { get; private set; } = default!;
    public virtual bool ignoringFeedbackSemantics { get; private set; } = default!;
    public virtual bool ignoringFeedbackPointer { get; private set; } = default!;
    public virtual long viewId { get; private set; } = default!;
    internal virtual IDragTargetState? _activeTarget { get; set; } = default;
    internal virtual List<IDragTargetState> _enteredTargets { get; private set; } = new List<IDragTargetState>();
    internal virtual Offset _position { get; set; } = default!;
    internal virtual Offset? _lastOffset { get; set; } = default;
    internal virtual Offset _overlayOffset { get; set; } = default!;
    internal virtual OverlayEntry? _entry { get; set; } = default;

    internal _DragAvatar__drag_target(OverlayState overlayState, T? data = default, Axis? axis = null, Offset initialPosition = default!, Offset dragStartPoint = default, Widget? feedback = null, Offset feedbackOffset = default, System.Action<DragUpdateDetails>? onDragUpdate = null, Action<Velocity, Offset, bool>? onDragEnd = null, bool ignoringFeedbackSemantics = default!, bool ignoringFeedbackPointer = default!, long viewId = default!)
    {
        this.overlayState = overlayState;
        this.data = data;
        this.axis = axis;
        this.dragStartPoint = dragStartPoint;
        this.feedback = feedback;
        this.feedbackOffset = feedbackOffset;
        this.onDragUpdate = onDragUpdate;
        this.onDragEnd = onDragEnd;
        this.ignoringFeedbackSemantics = ignoringFeedbackSemantics;
        this.ignoringFeedbackPointer = ignoringFeedbackPointer;
        this.viewId = viewId;
        _position = initialPosition;
        _entry = new OverlayEntry(builder: _build);
        overlayState.insert(_entry);
        updateDrag(initialPosition);
    }

    public override void update(DragUpdateDetails details)
    {
        Offset oldPosition = _position;
        _position += _restrictAxis(details.delta);
        updateDrag(_position);
        if ((onDragUpdate is not null) && (!Equals(_position, oldPosition)))
        {
            onDragUpdate!(details);
        }
    }

    public override void end(DragEndDetails details)
    {
        finishDrag(_DragEndKind__drag_target.dropped, _restrictVelocityAxis(details.velocity));
    }

    public override void cancel()
    {
        finishDrag(_DragEndKind__drag_target.canceled);
    }

    public virtual void updateDrag(Offset globalPosition)
    {
        _lastOffset = globalPosition - dragStartPoint;
        if (overlayState.mounted)
        {
            var box = ((RenderBox?)overlayState.context.findRenderObject()!)!;
            Offset overlaySpaceOffset = box.globalToLocal(globalPosition);
            _overlayOffset = overlaySpaceOffset - dragStartPoint;
            _entry!.markNeedsBuild();
        }
        var result = new HitTestResult();
        WidgetsBinding.instance.hitTestInView(result, globalPosition + feedbackOffset, viewId);
        List<IDragTargetState> targets = _getDragTargets(result.path.Cast<HitTestEntry<HitTestTarget>>()).ToList().ToList();
        var listsMatch = false;
        if ((checked(targets.Count) >= checked((long)_enteredTargets.Count)) && Enumerable.Any(_enteredTargets))
        {
            listsMatch = true;
            IEnumerator<IDragTargetState> iterator = targets.GetEnumerator();
            for (var i = 0L; i < checked(_enteredTargets.Count); i += 1L)
            {
                iterator.MoveNext();
                if (!Equals(iterator.Current, _enteredTargets[(int)i]))
                {
                    listsMatch = false;
                    break;
                }
            }
        }
        if (listsMatch && ((_activeTarget is not null) || (checked(targets.Count) == checked((long)_enteredTargets.Count))))
        {
            foreach (IDragTargetState targetLocal in _enteredTargets)
            {
                targetLocal.didMove(this);
            }
            return;
        }
        _leaveAllEntered();
        IDragTargetState? newTarget = targets.cast<IDragTargetState?>().firstWhere((target) =>
        {
            if (target is null)
            {
                return false;
            }
            _enteredTargets.Add(target);
            return target.didEnter(this);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, orElse: () => default!);
        foreach (IDragTargetState targetAlternate in _enteredTargets)
        {
            targetAlternate.didMove(this);
        }
        _activeTarget = newTarget;
    }

    internal virtual IEnumerable<IDragTargetState> _getDragTargets(IEnumerable<HitTestEntry<HitTestTarget>> path)
    {
        foreach (var entry in path)
            if (entry.target is RenderMetaData metadata &&
                metadata.metaData is IDragTargetState target && target.isExpectedDataType(data, typeof(T)))
                yield return target;
    }

    internal virtual void _leaveAllEntered()
    {
        for (var i = 0L; i < checked(_enteredTargets.Count); i += 1L)
        {
            _enteredTargets[(int)i].didLeave(this);
        }
        _enteredTargets.Clear();
    }

    public virtual void finishDrag(_DragEndKind__drag_target endKind, Velocity? velocity = null)
    {
        var wasAccepted = false;
        if (Equals(endKind, _DragEndKind__drag_target.dropped) && (_activeTarget is not null))
        {
            _activeTarget!.didDrop(this);
            wasAccepted = true;
            _enteredTargets.Remove(_activeTarget);
        }
        _leaveAllEntered();
        _activeTarget = null;
        _entry!.remove();
        _entry!.dispose();
        _entry = null;
        onDragEnd?.Invoke(velocity ?? Velocity.zero, DartRuntimePrimitives.RequireValue(_lastOffset), wasAccepted);
    }

    internal virtual Widget _build(BuildContext context)
    {
        return new Positioned(left: _overlayOffset.dx, top: _overlayOffset.dy, child: new ExcludeSemantics(excluding: ignoringFeedbackSemantics, child: new IgnorePointer(ignoring: ignoringFeedbackPointer, child: feedback)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Velocity _restrictVelocityAxis(Velocity velocity)
    {
        if (axis is null)
        {
            return velocity;
        }
        return new Velocity(pixelsPerSecond: _restrictAxis(velocity.pixelsPerSecond));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _restrictAxis(Offset offset)
    {
        return axis switch { Axis.horizontal => new Offset(offset.dx, 0.0), Axis.vertical => new Offset(0.0, offset.dy), null => offset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/drag_target.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public delegate bool DragTargetWillAccept<T>(T? data);

public delegate bool DragTargetWillAcceptWithDetails<T>(DragTargetDetails<T> details);

public delegate void DragTargetAccept<T>(T data);

public delegate void DragTargetAcceptWithDetails<T>(DragTargetDetails<T> details);

public delegate Widget DragTargetBuilder<T>(BuildContext context, List<T?> candidateData, List<object> rejectedData);

public delegate void DragUpdateCallback(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details);

public delegate void DraggableCanceledCallback(global::Doroti.Generated.Framework.Gestures.Velocity velocity, Offset offset);

public delegate void DragEndCallback(DraggableDetails details);

public delegate void DragTargetLeave<T>(T? data);

public delegate void DragTargetMove<T>(DragTargetDetails<T> details);

public delegate Offset DragAnchorStrategy(Draggable<object> draggable, BuildContext context, Offset position);

public static partial class Drag_targetLibrary
{
    public static Offset childDragAnchorStrategy(Draggable<object> draggable, BuildContext context, Offset position)
    {
        var renderObject__4151 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)context.findRenderObject()!)!;
        return ((Offset)((dynamic)renderObject__4151).globalToLocal(position));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Drag_targetLibrary
{
    public static Offset pointerDragAnchorStrategy(Draggable<object> draggable, BuildContext context, Offset position)
    {
        return Offset.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class Draggable<T> : StatefulWidget
{
    public virtual T? data { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis? axis { get; private set; }
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget? childWhenDragging { get; private set; }
    public virtual Widget feedback { get; private set; } = default!;
    public virtual Offset feedbackOffset { get; private set; } = default!;
    public virtual global::System.Func<Draggable<object>, BuildContext, Offset, Offset> dragAnchorStrategy { get; private set; } = default!;
    public virtual bool ignoringFeedbackSemantics { get; private set; } = default!;
    public virtual bool ignoringFeedbackPointer { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis? affinity { get; private set; }
    public virtual long? maxSimultaneousDrags { get; private set; }
    public virtual global::System.Action? onDragStarted { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>? onDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.Velocity, Offset>? onDraggableCanceled { get; private set; }
    public virtual global::System.Action? onDragCompleted { get; private set; }
    public virtual global::System.Action<DraggableDetails>? onDragEnd { get; private set; }
    public virtual bool rootOverlay { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual global::System.Func<long, bool>? allowedButtonsFilter { get; private set; }

    public Draggable(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, Widget feedback = default!, T? data = default, global::Doroti.Generated.Framework.Painting.Axis? axis = null, Widget? childWhenDragging = null, Offset feedbackOffset = default, global::System.Func<Draggable<object>, BuildContext, Offset, Offset> dragAnchorStrategy = default!, global::Doroti.Generated.Framework.Painting.Axis? affinity = null, long? maxSimultaneousDrags = null, global::System.Action? onDragStarted = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>? onDragUpdate = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.Velocity, Offset>? onDraggableCanceled = null, global::System.Action<DraggableDetails>? onDragEnd = null, global::System.Action? onDragCompleted = null, bool ignoringFeedbackSemantics = true, bool ignoringFeedbackPointer = true, bool rootOverlay = false, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild, global::System.Func<long, bool>? allowedButtonsFilter = null) : base(key: key)
    {
        global::System.Func<Draggable<object>, BuildContext, Offset, Offset> __dragAnchorStrategy = dragAnchorStrategy ?? Drag_targetLibrary.childDragAnchorStrategy;
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
        System.Diagnostics.Debug.Assert(((maxSimultaneousDrags is null) || (maxSimultaneousDrags >= 0L)));
    }

    public virtual global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer createRecognizer(global::System.Func<Offset, global::Doroti.Generated.Framework.Gestures.Drag?> onStart)
    {
        return ((Func<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>)(() =>
{            var __cascade = (this.affinity switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>(new global::Doroti.Generated.Framework.Gestures.HorizontalMultiDragGestureRecognizer(allowedButtonsFilter: (global::System.Func<long, bool>?)this.allowedButtonsFilter)), global::Doroti.Generated.Framework.Painting.Axis.vertical => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>(new global::Doroti.Generated.Framework.Gestures.VerticalMultiDragGestureRecognizer(allowedButtonsFilter: (global::System.Func<long, bool>?)this.allowedButtonsFilter)), null => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>(new global::Doroti.Generated.Framework.Gestures.ImmediateMultiDragGestureRecognizer(allowedButtonsFilter: (global::System.Func<long, bool>?)this.allowedButtonsFilter)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            __cascade.onStart = onStart;
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DraggableState__drag_target<T>());
}

public class LongPressDraggable<T> : Draggable<T>
{
    public virtual bool hapticFeedbackOnStart { get; private set; } = default!;
    public virtual Duration delay { get; private set; } = default!;

    public LongPressDraggable(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, Widget feedback = default!, T? data = default, global::Doroti.Generated.Framework.Painting.Axis? axis = null, Widget? childWhenDragging = null, Offset feedbackOffset = default, global::System.Func<Draggable<object>, BuildContext, Offset, Offset> dragAnchorStrategy = default!, long? maxSimultaneousDrags = null, global::System.Action? onDragStarted = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>? onDragUpdate = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.Velocity, Offset>? onDraggableCanceled = null, global::System.Action<DraggableDetails>? onDragEnd = null, global::System.Action? onDragCompleted = null, bool hapticFeedbackOnStart = true, bool ignoringFeedbackSemantics = true, bool ignoringFeedbackPointer = true, Duration? delay = null, global::System.Func<long, bool>? allowedButtonsFilter = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.deferToChild, bool rootOverlay = false) : base(key: key, child: child, feedback: feedback, data: data, axis: DartRuntimePrimitives.RequireValue(axis), childWhenDragging: childWhenDragging, feedbackOffset: feedbackOffset, dragAnchorStrategy: dragAnchorStrategy ?? Drag_targetLibrary.childDragAnchorStrategy, maxSimultaneousDrags: DartRuntimePrimitives.RequireValue(maxSimultaneousDrags), onDragStarted: onDragStarted, onDragUpdate: onDragUpdate, onDraggableCanceled: onDraggableCanceled, onDragEnd: onDragEnd, onDragCompleted: onDragCompleted, ignoringFeedbackSemantics: ignoringFeedbackSemantics, ignoringFeedbackPointer: ignoringFeedbackPointer, allowedButtonsFilter: allowedButtonsFilter, hitTestBehavior: hitTestBehavior, rootOverlay: rootOverlay)
    {
        Duration __delay = delay ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kLongPressTimeout;
        this.hapticFeedbackOnStart = hapticFeedbackOnStart;
        this.delay = __delay;
    }

    public override global::Doroti.Generated.Framework.Gestures.DelayedMultiDragGestureRecognizer createRecognizer(global::System.Func<Offset, global::Doroti.Generated.Framework.Gestures.Drag?> onStart)
    {
        return ((Func<global::Doroti.Generated.Framework.Gestures.DelayedMultiDragGestureRecognizer>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Gestures.DelayedMultiDragGestureRecognizer(delay: DartRuntimePrimitives.RequireValue(this.delay), allowedButtonsFilter: (global::System.Func<long, bool>?)this.allowedButtonsFilter);
            __cascade.onStart = ((position) => {
global::Doroti.Generated.Framework.Gestures.Drag? result__17898 = onStart(position);
if (((result__17898 is not null) && this.hapticFeedbackOnStart))
{
    DartRuntimePrimitives.Ignore(HapticFeedback.selectionClick());
}
return result__17898;
throw new InvalidOperationException("Dart closure completed without a value.");
});
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DraggableState__drag_target<T> : State<Draggable<T>>
{
    internal virtual global::Doroti.Generated.Framework.Gestures.GestureRecognizer? _recognizer { get; set; } = default;
    internal virtual long _activeCount { get; set; } = 0L;

    public override void initState()
    {
        base.initState();
        _recognizer = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.GestureRecognizer>(this.widget.createRecognizer((global::System.Func<Offset, _DragAvatar__drag_target<T>?>)this._startDrag));
    }

    public override void dispose()
    {
        _disposeRecognizerIfInactive();
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        this._recognizer!.gestureSettings = MediaQuery.maybeGestureSettingsOf(this.context);
        base.didChangeDependencies();
    }

    internal virtual void _disposeRecognizerIfInactive()
    {
        if ((this._activeCount > 0L))
        {
            return;
        }
        this._recognizer!.dispose();
        _recognizer = null;
    }

    internal virtual void _routePointer(global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event)
    {
        if (((((Draggable<T>)(object)this.widget).maxSimultaneousDrags is not null) && (this._activeCount >= DartRuntimePrimitives.RequireValue(((Draggable<T>)(object)this.widget).maxSimultaneousDrags))))
        {
            return;
        }
        this._recognizer!.addPointer((global::Doroti.Generated.Framework.Gestures.PointerDownEvent)(object)@event);
    }

    internal virtual _DragAvatar__drag_target<T>? _startDrag(Offset position)
    {
        if (((((Draggable<T>)(object)this.widget).maxSimultaneousDrags is not null) && (this._activeCount >= DartRuntimePrimitives.RequireValue(((Draggable<T>)(object)this.widget).maxSimultaneousDrags))))
        {
            return default;
        }
        global::Doroti.Ui.Offset dragStartPoint__19661 = default!;
        dragStartPoint__19661 = ((dynamic)this.widget.dragAnchorStrategy)(this.widget, this.context, position);
        setState(((global::System.Action)(() => {
_activeCount += 1L;
})));
        var avatar__19813 = new _DragAvatar__drag_target<T>(overlayState: Overlay.of(this.context, debugRequiredFor: this.widget, rootOverlay: ((Draggable<T>)(object)this.widget).rootOverlay), data: ((Draggable<T>)(object)this.widget).data, axis: ((Draggable<T>)(object)this.widget).axis, initialPosition: position, dragStartPoint: dragStartPoint__19661, feedback: ((Draggable<T>)(object)this.widget).feedback, feedbackOffset: ((Draggable<T>)(object)this.widget).feedbackOffset, ignoringFeedbackSemantics: ((Draggable<T>)(object)this.widget).ignoringFeedbackSemantics, ignoringFeedbackPointer: ((Draggable<T>)(object)this.widget).ignoringFeedbackPointer, viewId: checked((long)View.of(this.context).viewId), onDragUpdate: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>)((details) => {
if ((this.mounted && (((Draggable<T>)(object)this.widget).onDragUpdate is not null)))
{
    ((Draggable<T>)(object)this.widget).onDragUpdate!(details);
}
})), onDragEnd: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.Velocity, Offset, bool>)((velocity, offset, wasAccepted) => {
if (this.mounted)
{
    setState(((global::System.Action)(() => {
_activeCount -= 1L;
})));
}
else
{
    _activeCount -= 1L;
    _disposeRecognizerIfInactive();
}
if ((this.mounted && (((Draggable<T>)(object)this.widget).onDragEnd is not null)))
{
    ((Draggable<T>)(object)this.widget).onDragEnd!(new DraggableDetails(wasAccepted: wasAccepted, velocity: velocity, offset: offset));
}
if ((wasAccepted && (((Draggable<T>)(object)this.widget).onDragCompleted is not null)))
{
    ((Draggable<T>)(object)this.widget).onDragCompleted!();
}
if ((!wasAccepted && (((Draggable<T>)(object)this.widget).onDraggableCanceled is not null)))
{
    ((Draggable<T>)(object)this.widget).onDraggableCanceled!(velocity, offset);
}
})));
        ((Draggable<T>)(object)this.widget).onDragStarted?.Invoke();
        return avatar__19813;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        bool canDrag__21351 = ((((Draggable<T>)(object)this.widget).maxSimultaneousDrags is null) || (this._activeCount < DartRuntimePrimitives.RequireValue(((Draggable<T>)(object)this.widget).maxSimultaneousDrags)));
        bool showChild__21468 = ((this._activeCount == 0L) || (((Draggable<T>)(object)this.widget).childWhenDragging is null));
        return ((Widget)(object?)new Listener(behavior: ((Draggable<T>)(object)this.widget).hitTestBehavior, onPointerDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>)(canDrag__21351 ? this._routePointer : null)), child: (showChild__21468 ? ((Draggable<T>)(object)this.widget).child : ((Draggable<T>)(object)this.widget).childWhenDragging)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DraggableDetails
{
    public virtual bool wasAccepted { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.Velocity velocity { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;

    public DraggableDetails(bool wasAccepted = false, global::Doroti.Generated.Framework.Gestures.Velocity velocity = default!, Offset offset = default!)
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
    public virtual global::System.Func<BuildContext, List<T?>, List<object>, Widget> builder { get; private set; } = default!;
    public virtual global::System.Func<T?, bool>? onWillAccept { get; private set; }
    public virtual global::System.Func<DragTargetDetails<T>, bool>? onWillAcceptWithDetails { get; private set; }
    public virtual global::System.Action<T>? onAccept { get; private set; }
    public virtual global::System.Action<DragTargetDetails<T>>? onAcceptWithDetails { get; private set; }
    public virtual global::System.Action<T?>? onLeave { get; private set; }
    public virtual global::System.Action<DragTargetDetails<T>>? onMove { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;

    public DragTarget(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, List<T?>, List<object>, Widget> builder = default!, global::System.Func<T?, bool>? onWillAccept = null, global::System.Func<DragTargetDetails<T>, bool>? onWillAcceptWithDetails = null, global::System.Action<T>? onAccept = null, global::System.Action<DragTargetDetails<T>>? onAcceptWithDetails = null, global::System.Action<T?>? onLeave = null, global::System.Action<DragTargetDetails<T>>? onMove = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.translucent) : base(key: key)
    {
        this.builder = builder;
        this.onWillAccept = onWillAccept;
        this.onWillAcceptWithDetails = onWillAcceptWithDetails;
        this.onAccept = onAccept;
        this.onAcceptWithDetails = onAcceptWithDetails;
        this.onLeave = onLeave;
        this.onMove = onMove;
        this.hitTestBehavior = hitTestBehavior;
        System.Diagnostics.Debug.Assert(((onWillAccept is null) || (onWillAcceptWithDetails is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DragTargetState__drag_target<T>());
}

public static partial class Drag_targetLibrary
{
    internal static List<T?> _mapAvatarsToData<T>(List<dynamic> avatars)
    {
        return avatars.map<dynamic, T?>(((avatar) => ((T?)(object?)((dynamic)avatar).data)!)).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DragTargetState__drag_target<T> : State<DragTarget<T>>
{
    internal virtual List<dynamic> _candidateAvatars { get; private set; } = new List<_DragAvatar__drag_target<object>>().Cast<dynamic>().ToList();
    internal virtual List<dynamic> _rejectedAvatars { get; private set; } = new List<_DragAvatar__drag_target<object>>().Cast<dynamic>().ToList();

    public virtual bool isExpectedDataType(object? data, Type type)
    {
        if ((global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb && (((((object.Equals(type, typeof(long))) && (object.Equals(typeof(T), typeof(double))))) || (((object.Equals(type, typeof(double))) && (object.Equals(typeof(T), typeof(long)))))))))
        {
            return false;
        }
        return (data is T);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool didEnter(dynamic avatar)
    {
        DartRuntimePrimitives.Assert(() => !this._candidateAvatars.Contains(avatar));
        DartRuntimePrimitives.Assert(() => !this._rejectedAvatars.Contains(avatar));
        bool resolvedWillAccept__29181 = (((((((DragTarget<T>)(object)this.widget).onWillAccept is null) && (((DragTarget<T>)(object)this.widget).onWillAcceptWithDetails is null))) || (((((DragTarget<T>)(object)this.widget).onWillAccept is not null) && ((DragTarget<T>)(object)this.widget).onWillAccept!(((T?)(object?)((dynamic)avatar).data)!)))) || ((((((DragTarget<T>)(object)this.widget).onWillAcceptWithDetails is not null) && (((dynamic)avatar).data is not null)) && ((DragTarget<T>)(object)this.widget).onWillAcceptWithDetails!(new DragTargetDetails<T>(data: ((T?)(object?)((dynamic)avatar).data!)!, offset: DartRuntimePrimitives.RequireValue(((Offset?)((dynamic)avatar)._lastOffset)))))));
        if (resolvedWillAccept__29181)
        {
            setState(((global::System.Action)(() => {
this._candidateAvatars.Add(avatar);
})));
            return true;
        }
        else
        {
            setState(((global::System.Action)(() => {
this._rejectedAvatars.Add(avatar);
})));
            return false;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didLeave(dynamic avatar)
    {
        DartRuntimePrimitives.Assert(() => (this._candidateAvatars.Contains(avatar) || this._rejectedAvatars.Contains(avatar)));
        if (!this.mounted)
        {
            return;
        }
        setState(((global::System.Action)(() => {
this._candidateAvatars.Remove(avatar);
this._rejectedAvatars.Remove(avatar);
})));
        ((DragTarget<T>)(object)this.widget).onLeave?.Invoke(((T?)(object?)((dynamic)avatar).data)!);
    }

    public virtual void didDrop(dynamic avatar)
    {
        DartRuntimePrimitives.Assert(() => this._candidateAvatars.Contains(avatar));
        if (!this.mounted)
        {
            return;
        }
        setState(((global::System.Action)(() => {
this._candidateAvatars.Remove(avatar);
})));
        if ((((dynamic)avatar).data is not null))
        {
            ((DragTarget<T>)(object)this.widget).onAccept?.Invoke(((T?)(object?)((dynamic)avatar).data!)!);
            ((DragTarget<T>)(object)this.widget).onAcceptWithDetails?.Invoke(new DragTargetDetails<T>(data: ((T?)(object?)((dynamic)avatar).data!)!, offset: DartRuntimePrimitives.RequireValue(((Offset?)((dynamic)avatar)._lastOffset))));
        }
    }

    public virtual void didMove(dynamic avatar)
    {
        if ((!this.mounted || (((dynamic)avatar).data is null)))
        {
            return;
        }
        ((DragTarget<T>)(object)this.widget).onMove?.Invoke(new DragTargetDetails<T>(data: ((T?)(object?)((dynamic)avatar).data!)!, offset: DartRuntimePrimitives.RequireValue(((Offset?)((dynamic)avatar)._lastOffset))));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new MetaData(metaData: this, behavior: ((DragTarget<T>)(object)this.widget).hitTestBehavior, child: this.widget.builder(context, Drag_targetLibrary._mapAvatarsToData<T>(this._candidateAvatars), Drag_targetLibrary._mapAvatarsToData<object>(this._rejectedAvatars))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _DragEndKind__drag_target
{
    dropped,
    canceled
}

internal delegate void _OnDragEnd__drag_target(global::Doroti.Generated.Framework.Gestures.Velocity velocity, Offset offset, bool wasAccepted);

public class _DragAvatar__drag_target<T> : global::Doroti.Generated.Framework.Gestures.Drag
{
    public virtual T? data { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis? axis { get; private set; }
    public virtual Offset dragStartPoint { get; private set; } = default!;
    public virtual Widget? feedback { get; private set; }
    public virtual Offset feedbackOffset { get; private set; } = default!;
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>? onDragUpdate { get; private set; }
    public virtual global::System.Action<global::Doroti.Generated.Framework.Gestures.Velocity, Offset, bool>? onDragEnd { get; private set; }
    public virtual OverlayState overlayState { get; private set; } = default!;
    public virtual bool ignoringFeedbackSemantics { get; private set; } = default!;
    public virtual bool ignoringFeedbackPointer { get; private set; } = default!;
    public virtual long viewId { get; private set; } = default!;
    internal virtual _DragTargetState__drag_target<object>? _activeTarget { get; set; } = default;
    internal virtual List<_DragTargetState__drag_target<object>> _enteredTargets { get; private set; } = new List<_DragTargetState__drag_target<object>>();
    internal virtual Offset _position { get; set; } = default!;
    internal virtual Offset? _lastOffset { get; set; } = default;
    internal virtual Offset _overlayOffset { get; set; } = default!;
    internal virtual OverlayEntry? _entry { get; set; } = default;

    internal _DragAvatar__drag_target(OverlayState overlayState, T? data = default, global::Doroti.Generated.Framework.Painting.Axis? axis = null, Offset initialPosition = default!, Offset dragStartPoint = default, Widget? feedback = null, Offset feedbackOffset = default, global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>? onDragUpdate = null, global::System.Action<global::Doroti.Generated.Framework.Gestures.Velocity, Offset, bool>? onDragEnd = null, bool ignoringFeedbackSemantics = default!, bool ignoringFeedbackPointer = default!, long viewId = default!)
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
        this._position = initialPosition;
    }

    public override void update(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        global::Doroti.Ui.Offset oldPosition__32743 = ((global::Doroti.Ui.Offset)(object?)this._position);
        _position += _restrictAxis(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta);
        updateDrag(this._position);
        if (((this.onDragUpdate is not null) && (!object.Equals(this._position, oldPosition__32743))))
        {
            this.onDragUpdate!(details);
        }
    }

    public override void end(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        finishDrag(_DragEndKind__drag_target.dropped, _restrictVelocityAxis(((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity));
    }

    public override void cancel()
    {
        finishDrag(_DragEndKind__drag_target.canceled);
    }

    public virtual void updateDrag(Offset globalPosition)
    {
        _lastOffset = (globalPosition - this.dragStartPoint);
        if (this.overlayState.mounted)
        {
            var box__33288 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.overlayState.context.findRenderObject()!)!;
            global::Doroti.Ui.Offset overlaySpaceOffset__33368 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)box__33288).globalToLocal(globalPosition)));
            _overlayOffset = (overlaySpaceOffset__33368 - this.dragStartPoint);
            this._entry!.markNeedsBuild();
        }
        var result__33534 = new global::Doroti.Generated.Framework.Gestures.HitTestResult();
        WidgetsBinding.instance.hitTestInView(result__33534, (globalPosition + this.feedbackOffset), this.viewId);
        List<_DragTargetState__drag_target<object>> targets__33694 = _getDragTargets(((global::Doroti.Generated.Framework.Gestures.HitTestResult)result__33534).path.Cast<global::Doroti.Generated.Framework.Gestures.HitTestEntry<global::Doroti.Generated.Framework.Gestures.HitTestTarget>>()).ToList().ToList();
        var listsMatch__33752 = false;
        if (((checked((long)(targets__33694.Count)) >= checked((long)(this._enteredTargets.Count))) && System.Linq.Enumerable.Any(this._enteredTargets)))
        {
            listsMatch__33752 = true;
            IEnumerator<_DragTargetState__drag_target<object>> iterator__33926 = targets__33694.GetEnumerator();
            for (var i__33970 = 0L; (i__33970 < checked((long)(this._enteredTargets.Count))); i__33970 += 1L)
            {
                iterator__33926.MoveNext();
                if ((!object.Equals(iterator__33926.Current, this._enteredTargets[(int)(i__33970)])))
                {
                    listsMatch__33752 = false;
                    break;
                }
            }
        }
        if ((listsMatch__33752 && (((this._activeTarget is not null) || (checked((long)(targets__33694.Count)) == checked((long)(this._enteredTargets.Count)))))))
        {
            foreach (_DragTargetState__drag_target<object> target__34886 in this._enteredTargets)
            {
                target__34886.didMove(this);
            }
            return;
        }
        _leaveAllEntered();
        _DragTargetState__drag_target<object>? newTarget__35087 = targets__33694.cast<_DragTargetState__drag_target<object>?>().firstWhere(((target) => {
if ((target is null))
{
    return false;
}
this._enteredTargets.Add(target);
return target.didEnter(this);
throw new InvalidOperationException("Dart closure completed without a value.");
}), orElse: (() => default!));
        foreach (_DragTargetState__drag_target<object> target__35465 in this._enteredTargets)
        {
            target__35465.didMove(this);
        }
        _activeTarget = newTarget__35087;
    }

    internal virtual IEnumerable<_DragTargetState__drag_target<object>> _getDragTargets(IEnumerable<global::Doroti.Generated.Framework.Gestures.HitTestEntry<global::Doroti.Generated.Framework.Gestures.HitTestTarget>> path)
    {
        return ((IEnumerable<_DragTargetState__drag_target<object>>)(object?)new List<_DragTargetState__drag_target<object>>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _leaveAllEntered()
    {
        for (var i__36136 = 0L; (i__36136 < checked((long)(this._enteredTargets.Count))); i__36136 += 1L)
        {
            this._enteredTargets[(int)(i__36136)].didLeave(this);
        }
        this._enteredTargets.Clear();
    }

    public virtual void finishDrag(_DragEndKind__drag_target endKind, global::Doroti.Generated.Framework.Gestures.Velocity? velocity = null)
    {
        var wasAccepted__36334 = false;
        if (((object.Equals(endKind, _DragEndKind__drag_target.dropped)) && (this._activeTarget is not null)))
        {
            this._activeTarget!.didDrop(this);
            wasAccepted__36334 = true;
            this._enteredTargets.Remove(this._activeTarget);
        }
        _leaveAllEntered();
        _activeTarget = null;
        this._entry!.remove();
        this._entry!.dispose();
        _entry = null;
        this.onDragEnd?.Invoke((velocity ?? global::Doroti.Generated.Framework.Gestures.Velocity.zero), DartRuntimePrimitives.RequireValue(this._lastOffset), wasAccepted__36334);
    }

    internal virtual Widget _build(BuildContext context)
    {
        return ((Widget)(object?)new Positioned(left: this._overlayOffset.dx, top: this._overlayOffset.dy, child: new ExcludeSemantics(excluding: this.ignoringFeedbackSemantics, child: new IgnorePointer(ignoring: this.ignoringFeedbackPointer, child: this.feedback))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Gestures.Velocity _restrictVelocityAxis(global::Doroti.Generated.Framework.Gestures.Velocity velocity)
    {
        if ((this.axis is null))
        {
            return velocity;
        }
        return new global::Doroti.Generated.Framework.Gestures.Velocity(pixelsPerSecond: _restrictAxis(((global::Doroti.Generated.Framework.Gestures.Velocity)velocity).pixelsPerSecond));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _restrictAxis(Offset offset)
    {
        return (this.axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(offset.dx, 0.0), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, offset.dy), null => offset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/undo_history.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class UndoHistory<T> : StatefulWidget
{
    public virtual global::Doroti.Framework.Foundation.ValueNotifier<T> value { get; private set; } = default!;
    public virtual global::System.Func<T?, T, bool>? shouldChangeUndoStack { get; private set; }
    public virtual global::System.Func<T, T>? undoStackModifier { get; private set; }
    public virtual global::System.Action<T> onTriggered { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual UndoHistoryController? controller { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public UndoHistory(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<T?, T, bool>? shouldChangeUndoStack = null, global::Doroti.Framework.Foundation.ValueNotifier<T> value = default!, global::System.Action<T> onTriggered = default!, FocusNode focusNode = default!, global::System.Func<T, T>? undoStackModifier = null, UndoHistoryController? controller = null, Widget child = default!) : base(key: key)
    {
        this.shouldChangeUndoStack = shouldChangeUndoStack;
        this.value = value;
        this.onTriggered = onTriggered;
        this.focusNode = focusNode;
        this.undoStackModifier = undoStackModifier;
        this.controller = controller;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new UndoHistoryState<T>());
}

public class UndoHistoryState<T> : State<UndoHistory<T>>, global::Doroti.Framework.Services.UndoManagerClient
{
    internal virtual _UndoStack__undo_history<T> _stack { get; private set; } = new _UndoStack__undo_history<T>();
    internal virtual global::System.Func<T, Timer> _throttledPush { get; private set; } = default!;
    internal virtual Timer? _throttleTimer { get; set; } = default;
    internal virtual bool _duringTrigger { get; set; } = false;
    internal static Duration _kThrottleDuration = Duration.Create(milliseconds: 500L);
    internal virtual T? _lastValue { get; set; } = default;
    internal virtual UndoHistoryController? _controller { get; set; } = default;

    internal virtual UndoHistoryController _effectiveController => DartRuntimePrimitives.ConvertValue<UndoHistoryController>(widget.controller ?? (_controller ??= new UndoHistoryController()));
    public virtual void undo()
    {
        if (_stack.currentValue is null)
        {
            return;
        }
        if (_throttleTimer?.isActive ?? false)
        {
            _throttleTimer?.cancel();
            _update(_stack.currentValue);
        }
        else
        {
            _update(_stack.undo());
        }
        _updateState();
    }

    public virtual void redo()
    {
        _update(_stack.redo());
        _updateState();
    }

    public virtual bool canUndo => _stack.canUndo;
    public virtual bool canRedo => _stack.canRedo;
    internal virtual void _updateState()
    {
        _effectiveController.value = new UndoHistoryValue(canUndo: canUndo, canRedo: canRedo);
        if (!Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS))
        {
            return;
        }
        if (Equals(UndoManager.client, this))
        {
            UndoManager.setUndoState(canUndo: canUndo, canRedo: canRedo);
        }
    }

    internal virtual void _undoFromIntent(UndoTextIntent intent)
    {
        undo();
    }

    internal virtual void _redoFromIntent(RedoTextIntent intent)
    {
        redo();
    }

    internal virtual void _update(T? nextValue)
    {
        if (nextValue is null)
        {
            return;
        }
        if (Equals(nextValue, _lastValue))
        {
            return;
        }
        _lastValue = DartRuntimePrimitives.ConvertValue<T>(nextValue);
        _duringTrigger = true;
        try
        {
            widget.onTriggered(nextValue);
            DartRuntimePrimitives.Assert(() => Equals(widget.value.value, nextValue));
        }
        finally
        {
            _duringTrigger = false;
        }
    }

    internal virtual void _push()
    {
        if (EqualityComparer<T>.Default.Equals(widget.value.value, _lastValue))
        {
            return;
        }
        if (_duringTrigger)
        {
            return;
        }
        if (!(widget.shouldChangeUndoStack is null ? true : widget.shouldChangeUndoStack.Invoke(_lastValue, widget.value.value)))
        {
            return;
        }
        T nextValue = widget.undoStackModifier is null ? widget.value.value : widget.undoStackModifier.Invoke(widget.value.value);
        if (EqualityComparer<T>.Default.Equals(nextValue, _lastValue))
        {
            return;
        }
        _lastValue = nextValue;
        _throttleTimer = _throttledPush(nextValue);
    }

    internal virtual void _handleFocus()
    {
        if (!widget.focusNode.hasFocus)
        {
            if (Equals(UndoManager.client, this))
            {
                UndoManager.client = null;
            }
            return;
        }
        UndoManager.client = this;
        _updateState();
    }

    public virtual void handlePlatformUndo(global::Doroti.Framework.Services.UndoDirection direction)
    {
        switch (direction)
        {
            case UndoDirection.undo:
                {
                    undo();
                    break;
                }
            case UndoDirection.redo:
                {
                    redo();
                    break;
                }
        }
    }

    public override void initState()
    {
        base.initState();
        _throttledPush = Undo_historyLibrary._throttle<T>(duration: _kThrottleDuration, function: (currentValue) =>
        {
            _stack.push(currentValue);
            _updateState();
        });
        _push();
        widget.value.addListener(_push);
        _handleFocus();
        widget.focusNode.addListener(_handleFocus);
        _effectiveController.onUndo.addListener(undo);
        _effectiveController.onRedo.addListener(redo);
    }

    public override void didUpdateWidget(UndoHistory<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.value, oldWidget.value))
        {
            _stack.clear();
            oldWidget.value.removeListener(_push);
            widget.value.addListener(_push);
        }
        if (!Equals(widget.focusNode, oldWidget.focusNode))
        {
            oldWidget.focusNode.removeListener(_handleFocus);
            widget.focusNode.addListener(_handleFocus);
        }
        if (!Equals(widget.controller, oldWidget.controller))
        {
            _effectiveController.onUndo.removeListener(undo);
            _effectiveController.onRedo.removeListener(redo);
            _controller?.dispose();
            _controller = null;
            _effectiveController.onUndo.addListener(undo);
            _effectiveController.onRedo.addListener(redo);
        }
    }

    public override void dispose()
    {
        if (Equals(UndoManager.client, this))
        {
            UndoManager.client = null;
        }
        widget.value.removeListener(_push);
        widget.focusNode.removeListener(_handleFocus);
        _effectiveController.onUndo.removeListener(undo);
        _effectiveController.onRedo.removeListener(redo);
        _controller?.dispose();
        _throttleTimer?.cancel();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new Actions(actions: new DartMap<Type, dynamic> { [typeof(UndoTextIntent)] = Action<UndoTextIntent>.CreateOverridable(context: context, defaultAction: new CallbackAction<UndoTextIntent>(onInvoke: (__arg0) => { ((global::System.Action<UndoTextIntent>)_undoFromIntent)(__arg0); return default!; })), [typeof(RedoTextIntent)] = Action<RedoTextIntent>.CreateOverridable(context: context, defaultAction: new CallbackAction<RedoTextIntent>(onInvoke: (__arg0) => { ((global::System.Action<RedoTextIntent>)_redoFromIntent)(__arg0); return default!; })) }, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class UndoHistoryValue
{
    public static UndoHistoryValue empty = new UndoHistoryValue();
    public virtual bool canUndo { get; private set; } = default!;
    public virtual bool canRedo { get; private set; } = default!;

    public UndoHistoryValue(bool canUndo = false, bool canRedo = false)
    {
        this.canUndo = canUndo;
        this.canRedo = canRedo;
    }

    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "UndoHistoryValue")}(canUndo: {canUndo}, canRedo: {canRedo})";
    public override bool Equals(object? other)
    {
        var __other = other as UndoHistoryValue;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is UndoHistoryValue) && (__other.canUndo == canUndo) && (__other.canRedo == canRedo);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(canUndo.GetHashCode(), canRedo.GetHashCode()));
}

public class UndoHistoryController : global::Doroti.Framework.Foundation.ValueNotifier<UndoHistoryValue>
{
    public virtual global::Doroti.Framework.Foundation.ChangeNotifier onUndo { get; private set; } = new global::Doroti.Framework.Foundation.ChangeNotifier();
    public virtual global::Doroti.Framework.Foundation.ChangeNotifier onRedo { get; private set; } = new global::Doroti.Framework.Foundation.ChangeNotifier();

    public UndoHistoryController(UndoHistoryValue? value = null) : base(value ?? UndoHistoryValue.empty)
    {
    }

    public virtual void undo()
    {
        if (!value.canUndo)
        {
            return;
        }
        onUndo.notifyListeners();
    }

    public virtual void redo()
    {
        if (!value.canRedo)
        {
            return;
        }
        onRedo.notifyListeners();
    }

    public override void dispose()
    {
        onUndo.dispose();
        onRedo.dispose();
        base.dispose();
    }

}

internal class _UndoStack__undo_history<T>
{
    internal virtual List<T> _list { get; private set; } = new List<T>();
    internal virtual long _index { get; set; } = -1L;

    internal _UndoStack__undo_history()
    {
    }

    public virtual T? currentValue => !Enumerable.Any(_list) ? default(T) : _list[(int)_index];
    public virtual bool canUndo => DartRuntimePrimitives.ConvertValue<bool>(Enumerable.Any(_list) && (_index > 0L));
    public virtual bool canRedo => DartRuntimePrimitives.ConvertValue<bool>(Enumerable.Any(_list) && (_index < (checked(_list.Count) - 1L)));
    public virtual void push(T value)
    {
        if (!Enumerable.Any(_list))
        {
            _index = 0L;
            _list.Add(value);
            return;
        }
        DartRuntimePrimitives.Assert(() => (_index < checked(_list.Count)) && (_index >= 0L));
        if (EqualityComparer<T>.Default.Equals(value, currentValue))
        {
            return;
        }
        if (_index != (checked(_list.Count) - 1L))
        {
            _list.RemoveRange(checked((int)(_index + 1L)), checked((int)checked((long)_list.Count)));
        }
        _list.Add(value);
        _index = checked(_list.Count) - 1L;
    }

    public virtual T? undo()
    {
        if (!Enumerable.Any(_list))
        {
            return default;
        }
        DartRuntimePrimitives.Assert(() => (_index < checked(_list.Count)) && (_index >= 0L));
        if (_index != 0L)
        {
            _index = _index - 1L;
        }
        return currentValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? redo()
    {
        if (!Enumerable.Any(_list))
        {
            return default;
        }
        DartRuntimePrimitives.Assert(() => (_index < checked(_list.Count)) && (_index >= 0L));
        if (_index < (checked(_list.Count) - 1L))
        {
            _index = _index + 1L;
        }
        return currentValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        _list.Clear();
        _index = -1L;
    }

    public override string ToString()
    {
        return $"_UndoStack {_list}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _Throttleable__undo_history<T>(T currentArg);

internal delegate Timer _Throttled__undo_history<T>(T currentArg);

public static partial class Undo_historyLibrary
{
    internal static global::System.Func<T, Timer> _throttle<T>(Duration duration, global::System.Action<T> function)
    {
        Timer? timer = default!;
        T arg = default!;
        return (currentArg) =>
        {
            arg = currentArg;
            if ((timer is not null) && timer!.isActive)
            {
                return timer!;
            }
            timer = new Timer(duration, () =>
            {
                function(arg);
                timer = null;
            });
            return timer!;
            throw new InvalidOperationException("Dart closure completed without a value.");
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}


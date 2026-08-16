// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/undo_history.dart
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

    internal virtual UndoHistoryController _effectiveController => DartRuntimePrimitives.ConvertValue<UndoHistoryController>((((UndoHistory<T>)(object)this.widget).controller ?? (_controller ??= new UndoHistoryController())));
    public virtual void undo()
    {
        if ((((_UndoStack__undo_history<T>)this._stack).currentValue is null))
        {
            return;
        }
        if ((this._throttleTimer?.isActive ?? false))
        {
            this._throttleTimer?.cancel();
            _update(((_UndoStack__undo_history<T>)this._stack).currentValue);
        }
        else
        {
            _update(this._stack.undo());
        }
        _updateState();
    }

    public virtual void redo()
    {
        _update(this._stack.redo());
        _updateState();
    }

    public virtual bool canUndo => ((_UndoStack__undo_history<T>)this._stack).canUndo;
    public virtual bool canRedo => ((_UndoStack__undo_history<T>)this._stack).canRedo;
    internal virtual void _updateState()
    {
        this._effectiveController.value = new UndoHistoryValue(canUndo: this.canUndo, canRedo: this.canRedo);
        if ((!object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)))
        {
            return;
        }
        if ((object.Equals(global::Doroti.Framework.Services.UndoManager.client, this)))
        {
            UndoManager.setUndoState(canUndo: this.canUndo, canRedo: this.canRedo);
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
        if ((nextValue is null))
        {
            return;
        }
        if (object.Equals(nextValue, this._lastValue))
        {
            return;
        }
        _lastValue = DartRuntimePrimitives.ConvertValue<T>(nextValue);
        _duringTrigger = true;
        try
        {
            this.widget.onTriggered(nextValue);
            DartRuntimePrimitives.Assert(() => object.Equals(((UndoHistory<T>)(object)this.widget).value.value, nextValue));
        }
        finally
        {
            _duringTrigger = false;
        }
    }

    internal virtual void _push()
    {
        if (EqualityComparer<T>.Default.Equals(((UndoHistory<T>)(object)this.widget).value.value, this._lastValue))
        {
            return;
        }
        if (this._duringTrigger)
        {
            return;
        }
        if (!(((((UndoHistory<T>)(object)this.widget).shouldChangeUndoStack is null ? true : ((UndoHistory<T>)(object)this.widget).shouldChangeUndoStack.Invoke(this._lastValue, ((UndoHistory<T>)(object)this.widget).value.value)))))
        {
            return;
        }
        T nextValue__6012 = ((((UndoHistory<T>)(object)this.widget).undoStackModifier is null ? ((UndoHistory<T>)(object)this.widget).value.value : ((UndoHistory<T>)(object)this.widget).undoStackModifier.Invoke(((UndoHistory<T>)(object)this.widget).value.value)));
        if (EqualityComparer<T>.Default.Equals(nextValue__6012, this._lastValue))
        {
            return;
        }
        _lastValue = nextValue__6012;
        _throttleTimer = this._throttledPush(nextValue__6012);
    }

    internal virtual void _handleFocus()
    {
        if (!((UndoHistory<T>)(object)this.widget).focusNode.hasFocus)
        {
            if ((object.Equals(global::Doroti.Framework.Services.UndoManager.client, this)))
            {
                global::Doroti.Framework.Services.UndoManager.client = null;
            }
            return;
        }
        global::Doroti.Framework.Services.UndoManager.client = this;
        _updateState();
    }

    public virtual void handlePlatformUndo(global::Doroti.Framework.Services.UndoDirection direction)
    {
        switch (direction)
        {
            case global::Doroti.Framework.Services.UndoDirection.undo:
                {
                    undo();
                    break;
                }
            case global::Doroti.Framework.Services.UndoDirection.redo:
                {
                    redo();
                    break;
                }
        }
    }

    public override void initState()
    {
        base.initState();
        _throttledPush = (global::System.Func<T, Timer>)Undo_historyLibrary._throttle<T>(duration: _kThrottleDuration, function: ((global::System.Action<T>)((currentValue) => {
this._stack.push(currentValue);
_updateState();
})));
        _push();
        ((UndoHistory<T>)(object)this.widget).value.addListener(() => this._push());
        _handleFocus();
        ((UndoHistory<T>)(object)this.widget).focusNode.addListener(() => this._handleFocus());
        ((UndoHistoryController)this._effectiveController).onUndo.addListener(() => this.undo());
        ((UndoHistoryController)this._effectiveController).onRedo.addListener(() => this.redo());
    }

    public override void didUpdateWidget(UndoHistory<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((UndoHistory<T>)(object)this.widget).value, ((UndoHistory<T>)oldWidget).value)))
        {
            this._stack.clear();
            ((UndoHistory<T>)oldWidget).value.removeListener(() => this._push());
            ((UndoHistory<T>)(object)this.widget).value.addListener(() => this._push());
        }
        if ((!object.Equals(((UndoHistory<T>)(object)this.widget).focusNode, ((UndoHistory<T>)oldWidget).focusNode)))
        {
            ((UndoHistory<T>)oldWidget).focusNode.removeListener(() => this._handleFocus());
            ((UndoHistory<T>)(object)this.widget).focusNode.addListener(() => this._handleFocus());
        }
        if ((!object.Equals(((UndoHistory<T>)(object)this.widget).controller, ((UndoHistory<T>)oldWidget).controller)))
        {
            ((UndoHistoryController)this._effectiveController).onUndo.removeListener(() => this.undo());
            ((UndoHistoryController)this._effectiveController).onRedo.removeListener(() => this.redo());
            this._controller?.dispose();
            _controller = null;
            ((UndoHistoryController)this._effectiveController).onUndo.addListener(() => this.undo());
            ((UndoHistoryController)this._effectiveController).onRedo.addListener(() => this.redo());
        }
    }

    public override void dispose()
    {
        if ((object.Equals(global::Doroti.Framework.Services.UndoManager.client, this)))
        {
            global::Doroti.Framework.Services.UndoManager.client = null;
        }
        ((UndoHistory<T>)(object)this.widget).value.removeListener(() => this._push());
        ((UndoHistory<T>)(object)this.widget).focusNode.removeListener(() => this._handleFocus());
        ((UndoHistoryController)this._effectiveController).onUndo.removeListener(() => this.undo());
        ((UndoHistoryController)this._effectiveController).onRedo.removeListener(() => this.redo());
        this._controller?.dispose();
        this._throttleTimer?.cancel();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Actions(actions: new DartMap<Type, dynamic> { [typeof(UndoTextIntent)] = Action<UndoTextIntent>.CreateOverridable(context: context, defaultAction: new CallbackAction<UndoTextIntent>(onInvoke: (__arg0) => { ((global::System.Action<UndoTextIntent>)this._undoFromIntent)(__arg0); return default!; })), [typeof(RedoTextIntent)] = Action<RedoTextIntent>.CreateOverridable(context: context, defaultAction: new CallbackAction<RedoTextIntent>(onInvoke: (__arg0) => { ((global::System.Action<RedoTextIntent>)this._redoFromIntent)(__arg0); return default!; })) }, child: ((UndoHistory<T>)(object)this.widget).child));
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

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "UndoHistoryValue"))}(canUndo: {this.canUndo}, canRedo: {this.canRedo})";
    public override bool Equals(object? other)
    {
        var __other = other as UndoHistoryValue;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((__other is UndoHistoryValue) && (((UndoHistoryValue)((UndoHistoryValue)__other)).canUndo == this.canUndo)) && (((UndoHistoryValue)((UndoHistoryValue)__other)).canRedo == this.canRedo));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.canUndo.GetHashCode(), this.canRedo.GetHashCode()));
}

public class UndoHistoryController : global::Doroti.Framework.Foundation.ValueNotifier<UndoHistoryValue>
{
    public virtual global::Doroti.Framework.Foundation.ChangeNotifier onUndo { get; private set; } = new global::Doroti.Framework.Foundation.ChangeNotifier();
    public virtual global::Doroti.Framework.Foundation.ChangeNotifier onRedo { get; private set; } = new global::Doroti.Framework.Foundation.ChangeNotifier();

    public UndoHistoryController(UndoHistoryValue? value = null) : base((value ?? UndoHistoryValue.empty))
    {
    }

    public virtual void undo()
    {
        if (!((UndoHistoryValue)(object)this.value).canUndo)
        {
            return;
        }
        this.onUndo.notifyListeners();
    }

    public virtual void redo()
    {
        if (!((UndoHistoryValue)(object)this.value).canRedo)
        {
            return;
        }
        this.onRedo.notifyListeners();
    }

    public virtual void dispose()
    {
        this.onUndo.dispose();
        this.onRedo.dispose();
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

    public virtual T? currentValue => (!System.Linq.Enumerable.Any(this._list) ? default(T) : this._list[(int)(this._index)]);
    public virtual bool canUndo => DartRuntimePrimitives.ConvertValue<bool>((System.Linq.Enumerable.Any(this._list) && (this._index > 0L)));
    public virtual bool canRedo => DartRuntimePrimitives.ConvertValue<bool>((System.Linq.Enumerable.Any(this._list) && (this._index < (checked((long)(this._list.Count)) - 1L))));
    public virtual void push(T value)
    {
        if (!System.Linq.Enumerable.Any(this._list))
        {
            _index = 0L;
            this._list.Add(value);
            return;
        }
        DartRuntimePrimitives.Assert(() => ((this._index < checked((long)(this._list.Count))) && (this._index >= 0L)));
        if (EqualityComparer<T>.Default.Equals(value, this.currentValue))
        {
            return;
        }
        if ((this._index != (checked((long)(this._list.Count)) - 1L)))
        {
            this._list.RemoveRange(checked((int)(this._index + 1L)), checked((int)checked((long)(this._list.Count))));
        }
        this._list.Add(value);
        _index = (checked((long)(this._list.Count)) - 1L);
    }

    public virtual T? undo()
    {
        if (!System.Linq.Enumerable.Any(this._list))
        {
            return default;
        }
        DartRuntimePrimitives.Assert(() => ((this._index < checked((long)(this._list.Count))) && (this._index >= 0L)));
        if ((this._index != 0L))
        {
            _index = (this._index - 1L);
        }
        return this.currentValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T? redo()
    {
        if (!System.Linq.Enumerable.Any(this._list))
        {
            return default;
        }
        DartRuntimePrimitives.Assert(() => ((this._index < checked((long)(this._list.Count))) && (this._index >= 0L)));
        if ((this._index < (checked((long)(this._list.Count)) - 1L)))
        {
            _index = (this._index + 1L);
        }
        return this.currentValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        this._list.Clear();
        _index = -1L;
    }

    public override string ToString()
    {
        return $"_UndoStack {this._list}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _Throttleable__undo_history<T>(T currentArg);

internal delegate Timer _Throttled__undo_history<T>(T currentArg);

public static partial class Undo_historyLibrary
{
    internal static global::System.Func<T, Timer> _throttle<T>(Duration duration, global::System.Action<T> function)
    {
        Timer? timer__14360 = default!;
        T arg__14376 = default!;
        return ((global::System.Func<T, Timer>)((currentArg) => {
arg__14376 = currentArg;
if (((timer__14360 is not null) && timer__14360!.isActive))
{
    return timer__14360!;
}
timer__14360 = new Timer(duration, (() => {
function(arg__14376);
timer__14360 = null;
}));
return timer__14360!;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}


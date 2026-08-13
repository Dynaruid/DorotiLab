// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/foundation/change_notifier.dart
using System;
using System.Collections.Generic;
using System.Linq;
using Doroti.Flutter.Runtime;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;

namespace Doroti.Generated.Framework.Foundation;

public interface Listenable
{
    public static Listenable merge(IEnumerable<Listenable?> listenables)
        => new _MergingListenable(listenables);
    public static Listenable CreateMerge(IEnumerable<Listenable?> listenables) => merge(listenables);

    public void addListener(Action listener);
    public void removeListener(Action listener);
    public void AddListener(Action listener) => addListener(listener);
    public void RemoveListener(Action listener) => removeListener(listener);
}

public interface ValueListenable<T> : Listenable
{
    public T value { get; }
}

public class ChangeNotifier : Listenable, IDisposable
{
    private int _count = 0;
    private static readonly List<Action?> _emptyListeners = new List<Action?>(System.Linq.Enumerable.Repeat<Action?>(null, 0));
    private List<Action?> _listeners = _emptyListeners;
    private int _notificationCallStackDepth = 0;
    private int _reentrantlyRemovedListeners = 0;
    private bool _debugDisposed = false;
    private bool _debugCreationDispatched = false;

    public static bool debugAssertNotDisposed(ChangeNotifier notifier)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (notifier._debugDisposed)
                {
                    throw new FlutterError($"A {notifier.GetType()} was used after being disposed.\n" + $"Once you have called dispose() on a {notifier.GetType()}, it " + "can no longer be used.");
                }
                return true;
            });
        return true;
    }

    public bool hasListeners => (_count > 0);
    public int debugListenerCount => _count;
    public static void maybeDispatchObjectCreation(ChangeNotifier @object)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (!@object._debugCreationDispatched)
                {
                    DebugLibrary.debugMaybeDispatchCreated("foundation", "ChangeNotifier", @object);
                    @object._debugCreationDispatched = true;
                }
                return true;
            });
    }

    public void addListener(Action listener)
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        if (MemoryAllocationsLibrary.kFlutterMemoryAllocationsEnabled)
        {
            maybeDispatchObjectCreation(this);
        }
        if ((_count == _listeners.Count))
        {
            if ((_count == 0))
            {
                _listeners = new List<Action?>(System.Linq.Enumerable.Repeat<Action?>(null, 1));
            }
            else
            {
                var newListeners = new List<Action?>(System.Linq.Enumerable.Repeat<Action?>(null, (_listeners.Count * 2)));
                for (var i = 0; (i < _count); i++)
                {
                    newListeners[i] = _listeners[i];
                }
                _listeners = newListeners;
            }
        }
        _listeners[_count++] = listener;
    }

    public void AddListener(Action listener) => addListener(listener);

    private void _removeAt(int index)
    {
        _count -= 1;
        if (((_count * 2) <= _listeners.Count))
        {
            var newListeners = new List<Action?>(System.Linq.Enumerable.Repeat<Action?>(null, _count));
            for (var i = 0; (i < index); i++)
            {
                newListeners[i] = _listeners[i];
            }
            for (var i = index; (i < _count); i++)
            {
                newListeners[i] = _listeners[(i + 1)];
            }
            _listeners = newListeners;
        }
        else
        {
            for (var i = index; (i < _count); i++)
            {
                _listeners[i] = _listeners[(i + 1)];
            }
            _listeners[_count] = null;
        }
    }

    public void removeListener(Action listener)
    {
        for (var i = 0; (i < _count); i++)
        {
            Action? listenerAtIndex = _listeners[i];
            if ((listenerAtIndex == listener))
            {
                if ((_notificationCallStackDepth > 0))
                {
                    _listeners[i] = null;
                    _reentrantlyRemovedListeners++;
                }
                else
                {
                    _removeAt(i);
                }
                break;
            }
        }
    }

    public void RemoveListener(Action listener) => removeListener(listener);

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        DartRuntimePrimitives.Assert(() => (_notificationCallStackDepth == 0));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDisposed = true;
                if (_debugCreationDispatched)
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugMaybeDispatchDisposed(this));
                }
                return true;
            });
        _listeners = _emptyListeners;
        _count = 0;
    }

    public virtual void Dispose()
    {
        dispose();
        GC.SuppressFinalize(this);
    }

    public void notifyListeners()
    {
        DartRuntimePrimitives.Assert(() => ChangeNotifier.debugAssertNotDisposed(this));
        if ((_count == 0))
        {
            return;
        }
        _notificationCallStackDepth++;
        int end = _count;
        for (var i = 0; (i < end); i++)
        {
            try
            {
                _listeners[i]?.Invoke();
            }
            catch (Exception exception)
            {
                var stack = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception, stack, "foundation library", new ErrorDescription($"while dispatching notifications for {this.GetType()}"), () => new List<DiagnosticsNode> { new DiagnosticsProperty<ChangeNotifier>($"The {this.GetType()} sending notification was", this, DiagnosticsTreeStyle.errorProperty) }));
            }
        }
        _notificationCallStackDepth--;
        if (((_notificationCallStackDepth == 0) && (_reentrantlyRemovedListeners > 0)))
        {
            int newLength = (_count - _reentrantlyRemovedListeners);
            if (((newLength * 2) <= _listeners.Count))
            {
                var newListeners = new List<Action?>(System.Linq.Enumerable.Repeat<Action?>(null, newLength));
                var newIndex = 0;
                for (var i = 0; (i < _count); i++)
                {
                    Action? listener = _listeners[i];
                    if ((listener is not null))
                    {
                        newListeners[newIndex++] = listener;
                    }
                }
                _listeners = newListeners;
            }
            else
            {
                for (var i = 0; (i < newLength); i += 1)
                {
                    if ((_listeners[i] is null))
                    {
                        int swapIndex = (i + 1);
                        while ((_listeners[swapIndex] is null))
                        {
                            swapIndex += 1;
                        }
                        _listeners[i] = _listeners[swapIndex];
                        _listeners[swapIndex] = null;
                    }
                }
            }
            _reentrantlyRemovedListeners = 0;
            _count = newLength;
        }
    }

    protected void NotifyListeners() => notifyListeners();

}

internal class _MergingListenable : Listenable
{
    private IEnumerable<Listenable?> _children { get; }

    internal _MergingListenable(IEnumerable<Listenable?> _children)
    {
        this._children = _children;
    }

    public void addListener(Action listener)
    {
        foreach (Listenable? child in _children)
        {
            child?.addListener(listener);
        }
    }

    public void removeListener(Action listener)
    {
        foreach (Listenable? child in _children)
        {
            child?.removeListener(listener);
        }
    }

    public override string ToString()
    {
        return $"Listenable.merge([{string.Join(", ", _children)}])";
    }

}

public class ValueNotifier<T> : ChangeNotifier, ValueListenable<T>
{
    private T _value;

    public ValueNotifier(T _value)
    {
        this._value = _value;
    }

    public virtual T value
    {
        get => _value;
        set
        {
            var newValue = value;
            if (EqualityComparer<T>.Default.Equals(_value, newValue))
            {
                return;
            }
            _value = newValue;
            notifyListeners();
        }
    }
    public virtual T Value
    {
        get => value;
        set => this.value = value;
    }
    public override string ToString() => $"{DiagnosticsLibrary.describeIdentity(this)}({value})";
}

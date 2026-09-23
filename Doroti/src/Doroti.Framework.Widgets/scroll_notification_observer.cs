// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_notification_observer.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate void ScrollNotificationCallback(ScrollNotification notification);

internal class _ScrollNotificationObserverScope__scroll_notification_observer : InheritedWidget
{
    internal virtual ScrollNotificationObserverState _scrollNotificationObserverState
    {
        get;
        private set;
    } = default!;

    internal _ScrollNotificationObserverScope__scroll_notification_observer(
        Widget child,
        ScrollNotificationObserverState scrollNotificationObserverState
    )
        : base(child: child)
    {
        _scrollNotificationObserverState = scrollNotificationObserverState;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        !Equals(
            _scrollNotificationObserverState,
            (
                (_ScrollNotificationObserverScope__scroll_notification_observer)oldWidget
            )._scrollNotificationObserverState
        );
}

internal class _ListenerEntry__scroll_notification_observer
{
    internal LinkedListNode<_ListenerEntry__scroll_notification_observer>? node { get; set; }
    public virtual Action<ScrollNotification> listener { get; private set; } = default!;

    internal _ListenerEntry__scroll_notification_observer(Action<ScrollNotification> listener)
    {
        this.listener = listener;
    }
}

public class ScrollNotificationObserver : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public ScrollNotificationObserver(Key? key = null, Widget child = default!)
        : base(key: key)
    {
        this.child = child;
    }

    public static ScrollNotificationObserverState? maybeOf(BuildContext context)
    {
        return context
            .dependOnInheritedWidgetOfExactType<_ScrollNotificationObserverScope__scroll_notification_observer>()
            ?._scrollNotificationObserverState;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ScrollNotificationObserverState of(BuildContext context)
    {
        ScrollNotificationObserverState? observerState = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (observerState is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "ScrollNotificationObserver.of() was called with a context that does not contain a "
                            + "ScrollNotificationObserver widget.\n"
                            + "No ScrollNotificationObserver widget ancestor could be found starting from the "
                            + "context that was passed to ScrollNotificationObserver.of(). This can happen "
                            + "because you are using a widget that looks for a ScrollNotificationObserver "
                            + "ancestor, but no such ancestor exists.\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return observerState!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new ScrollNotificationObserverState());
}

public class ScrollNotificationObserverState : State<ScrollNotificationObserver>
{
    internal virtual LinkedList<_ListenerEntry__scroll_notification_observer>? _listeners { get; set; } =
        new LinkedList<_ListenerEntry__scroll_notification_observer>();

    internal virtual bool _debugAssertNotDisposed()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_listeners is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        $"A {GetType()} was used after being disposed.\n"
                            + $"Once you have called dispose() on a {GetType()}, it can no longer be used."
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void addListener(Action<ScrollNotification> listener)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        var entry = new _ListenerEntry__scroll_notification_observer(listener);
        entry.node = _listeners!.AddLast(entry);
    }

    public virtual void removeListener(Action<ScrollNotification> listener)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        foreach (_ListenerEntry__scroll_notification_observer entry in _listeners!)
        {
            if (Equals(entry.listener, listener))
            {
                _listeners.Remove(entry.node!);
                entry.node = null;
                return;
            }
        }
    }

    internal virtual void _notifyListeners(ScrollNotification notification)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        if (_listeners!.Count == 0)
        {
            return;
        }
        var localListeners = new List<_ListenerEntry__scroll_notification_observer>(_listeners!);
        foreach (var entry in localListeners)
        {
            try
            {
                if (entry.node?.List is not null)
                {
                    entry.listener(notification);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "widget library",
                        context: new ErrorDescription(
                            $"while dispatching notifications for {GetType()}"
                        ),
                        informationCollector: () =>
                            new List<DiagnosticsNode>
                            {
                                new DiagnosticsProperty<ScrollNotificationObserverState>(
                                    $"The {GetType()} sending notification was",
                                    this,
                                    style: DiagnosticsTreeStyle.errorProperty
                                ),
                            }
                    )
                );
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        return new NotificationListener<ScrollMetricsNotification>(
            onNotification: (notification) =>
            {
                _notifyListeners(notification.asScrollUpdate());
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new NotificationListener<ScrollNotification>(
                onNotification: (notification) =>
                {
                    _notifyListeners(notification);
                    return false;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                },
                child: new _ScrollNotificationObserverScope__scroll_notification_observer(
                    scrollNotificationObserverState: this,
                    child: widget.child
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        _listeners = null;
        base.dispose();
    }
}

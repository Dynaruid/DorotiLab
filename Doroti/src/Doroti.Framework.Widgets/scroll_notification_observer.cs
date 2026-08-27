// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_notification_observer.dart
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

public delegate void ScrollNotificationCallback(ScrollNotification notification);

internal class _ScrollNotificationObserverScope__scroll_notification_observer : InheritedWidget
{
    internal virtual ScrollNotificationObserverState _scrollNotificationObserverState { get; private set; } = default!;

    internal _ScrollNotificationObserverScope__scroll_notification_observer(Widget child, ScrollNotificationObserverState scrollNotificationObserverState) : base(child: child)
    {
        this._scrollNotificationObserverState = scrollNotificationObserverState;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => (!object.Equals(this._scrollNotificationObserverState, ((_ScrollNotificationObserverScope__scroll_notification_observer)oldWidget)._scrollNotificationObserverState));
}

internal class _ListenerEntry__scroll_notification_observer : DartLinkedListEntry<_ListenerEntry__scroll_notification_observer>
{
    public virtual global::System.Action<ScrollNotification> listener { get; private set; } = default!;

    internal _ListenerEntry__scroll_notification_observer(global::System.Action<ScrollNotification> listener)
    {
        this.listener = listener;
    }

}

public class ScrollNotificationObserver : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public ScrollNotificationObserver(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public static ScrollNotificationObserverState? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<_ScrollNotificationObserverScope__scroll_notification_observer>()?._scrollNotificationObserverState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollNotificationObserverState of(BuildContext context)
    {
        ScrollNotificationObserverState? observerState = ((ScrollNotificationObserverState?)(object?)ScrollNotificationObserver.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((observerState is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("ScrollNotificationObserver.of() was called with a context that does not contain a " + "ScrollNotificationObserver widget.\n" + "No ScrollNotificationObserver widget ancestor could be found starting from the " + "context that was passed to ScrollNotificationObserver.of(). This can happen " + "because you are using a widget that looks for a ScrollNotificationObserver " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return observerState!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ScrollNotificationObserverState());
}

public class ScrollNotificationObserverState : State<ScrollNotificationObserver>
{
    internal virtual DartLinkedList<_ListenerEntry__scroll_notification_observer>? _listeners { get; set; } = new DartLinkedList<_ListenerEntry__scroll_notification_observer>();

    internal virtual bool _debugAssertNotDisposed()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._listeners is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create($"A {this.GetType()} was used after being disposed.\n" + $"Once you have called dispose() on a {this.GetType()}, it can no longer be used."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void addListener(global::System.Action<ScrollNotification> listener)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        this._listeners!.add(new _ListenerEntry__scroll_notification_observer((global::System.Action<ScrollNotification>)listener));
    }

    public virtual void removeListener(global::System.Action<ScrollNotification> listener)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        foreach (_ListenerEntry__scroll_notification_observer entry in this._listeners!)
        {
            if ((object.Equals((global::System.Action<ScrollNotification>)((_ListenerEntry__scroll_notification_observer)entry).listener, (global::System.Action<ScrollNotification>)listener)))
            {
                entry.unlink();
                return;
            }
        }
    }

    internal virtual void _notifyListeners(ScrollNotification notification)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        if (this._listeners!.isEmpty)
        {
            return;
        }
        var localListeners = new List<_ListenerEntry__scroll_notification_observer>(this._listeners!);
        foreach (var entry in localListeners)
        {
            try
            {
                if ((((DartLinkedList<_ListenerEntry__scroll_notification_observer>?)((dynamic)entry).list) is not null))
                {
                    entry.listener(notification);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widget library", context: new global::Doroti.Framework.Foundation.ErrorDescription($"while dispatching notifications for {this.GetType()}"), informationCollector: ((InformationCollector)(() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollNotificationObserverState>($"The {this.GetType()} sending notification was", this, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }))));
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new NotificationListener<ScrollMetricsNotification>(onNotification: ((global::System.Func<ScrollMetricsNotification, bool>?)((notification) =>
        {
            _notifyListeners(notification.asScrollUpdate());
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new NotificationListener<ScrollNotification>(onNotification: ((global::System.Func<ScrollNotification, bool>?)((notification) =>
        {
            _notifyListeners(notification);
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new _ScrollNotificationObserverScope__scroll_notification_observer(scrollNotificationObserverState: this, child: ((ScrollNotificationObserver)this.widget).child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => _debugAssertNotDisposed());
        _listeners = null;
        base.dispose();
    }

}


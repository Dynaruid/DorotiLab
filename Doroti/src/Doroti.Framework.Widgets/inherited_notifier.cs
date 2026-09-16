// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_notifier.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class InheritedNotifier<T> : InheritedWidget where T : global::Doroti.Framework.Foundation.Listenable
{
    public virtual T? notifier { get; private set; }

    protected InheritedNotifier(global::Doroti.Framework.Foundation.Key? key = null, T? notifier = default, Widget child = default!) : base(key: key, child: child)
    {
        this.notifier = notifier;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (InheritedNotifier<T>)oldWidget;
        return !EqualityComparer<T>.Default.Equals(__oldWidget.notifier, notifier);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override InheritedElement createElement() => DartRuntimePrimitives.ConvertValue<InheritedElement>(new _InheritedNotifierElement__inherited_notifier<T>(this));
}

internal class _InheritedNotifierElement__inherited_notifier<T> : InheritedElement where T : global::Doroti.Framework.Foundation.Listenable
{
    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual bool _dirty { get; set; } = false;

    internal _InheritedNotifierElement__inherited_notifier(InheritedNotifier<T> widget) : base(widget)
    {
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (InheritedNotifier<T>)newWidget;
        T? oldNotifier = ((InheritedNotifier<T>?)widget)!.notifier;
        T? newNotifier = __newWidget.notifier;
        if (!EqualityComparer<T>.Default.Equals(oldNotifier, newNotifier))
        {
            oldNotifier?.removeListener(_handleUpdate);
            newNotifier?.addListener(_handleUpdate);
        }
        base.update(__newWidget);
    }

    public override Widget build()
    {
        if (_dirty)
        {
            notifyClients(((InheritedNotifier<T>?)widget)!);
        }
        return base.build();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleUpdate()
    {
        _dirty = true;
        markNeedsBuild();
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedNotifier<T>)oldWidget;
        base.notifyClients(__oldWidget);
        _dirty = false;
    }

    public override void unmount()
    {
        ((InheritedNotifier<T>?)widget)!.notifier?.removeListener(_handleUpdate);
        base.unmount();
    }

}


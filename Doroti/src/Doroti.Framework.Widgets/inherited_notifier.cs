// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_notifier.dart
#pragma warning disable CS8600, CS8603
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

public abstract class InheritedNotifier<T> : InheritedWidget where T : global::Doroti.Framework.Foundation.Listenable
{
    public virtual T? notifier { get; private set; }

    protected InheritedNotifier(global::Doroti.Framework.Foundation.Key? key = null, T? notifier = default, Widget child = default!) : base(key: key, child: child)
    {
        this.notifier = notifier;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (InheritedNotifier<T>)(object)oldWidget;
        return !EqualityComparer<T>.Default.Equals(((InheritedNotifier<T>)__oldWidget).notifier, this.notifier);
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
        var __newWidget = (InheritedNotifier<T>)(object)newWidget;
        T? oldNotifier = (((InheritedNotifier<T>?)(object?)this.widget)!).notifier;
        T? newNotifier = ((InheritedNotifier<T>)__newWidget).notifier;
        if (!EqualityComparer<T>.Default.Equals(oldNotifier, newNotifier))
        {
            oldNotifier?.removeListener(this._handleUpdate);
            newNotifier?.addListener(this._handleUpdate);
        }
        base.update(__newWidget);
    }

    public override Widget build()
    {
        if (this._dirty)
        {
            notifyClients(((InheritedNotifier<T>?)(object?)this.widget)!);
        }
        return ((Widget)(object?)base.build());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleUpdate()
    {
        _dirty = true;
        markNeedsBuild();
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
        var __oldWidget = (InheritedNotifier<T>)(object)oldWidget;
        base.notifyClients(__oldWidget);
        _dirty = false;
    }

    public override void unmount()
    {
        (((InheritedNotifier<T>?)(object?)this.widget)!).notifier?.removeListener(this._handleUpdate);
        base.unmount();
    }

}


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_notifier.dart
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
    internal virtual bool _dirty { get; set; } = false;

    internal _InheritedNotifierElement__inherited_notifier(InheritedNotifier<T> widget) : base(widget)
    {
    }

    public override void update(Widget newWidget)
    {
        var __newWidget = (InheritedNotifier<T>)(object)newWidget;
        T? oldNotifier__4353 = (((InheritedNotifier<T>?)(object?)this.widget)!).notifier;
        T? newNotifier__4423 = ((InheritedNotifier<T>)__newWidget).notifier;
        if (!EqualityComparer<T>.Default.Equals(oldNotifier__4353, newNotifier__4423))
        {
            oldNotifier__4353?.removeListener(() => this._handleUpdate());
            newNotifier__4423?.addListener(() => this._handleUpdate());
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
        (((InheritedNotifier<T>?)(object?)this.widget)!).notifier?.removeListener(() => this._handleUpdate());
        base.unmount();
    }

}


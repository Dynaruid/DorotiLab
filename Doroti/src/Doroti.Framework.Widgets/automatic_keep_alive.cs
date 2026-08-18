// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/automatic_keep_alive.dart
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

public class AutomaticKeepAlive : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;

    public AutomaticKeepAlive(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key)
    {
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AutomaticKeepAliveState__automatic_keep_alive());
}

internal class _AutomaticKeepAliveState__automatic_keep_alive : State<AutomaticKeepAlive>
{
    internal virtual DartMap<global::Doroti.Framework.Foundation.Listenable, global::System.Action>? _handles { get; set; } = default;
    internal virtual Widget _child { get; set; } = default!;
    internal virtual bool _keepingAlive { get; set; } = false;

    public override void initState()
    {
        base.initState();
        _updateChild();
    }

    public override void didUpdateWidget(AutomaticKeepAlive oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateChild();
    }

    internal virtual void _updateChild()
    {
        _child = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<KeepAliveNotification>(onNotification: (global::System.Func<KeepAliveNotification, bool>)this._addClient, child: ((AutomaticKeepAlive)(object)this.widget).child));
    }

    public override void dispose()
    {
        if ((this._handles is not null))
        {
            foreach (global::Doroti.Framework.Foundation.Listenable handle__4211 in this._handles!.Keys)
            {
                handle__4211.removeListener(this._handles!.GetValueOrDefault(handle__4211)!);
            }
        }
        base.dispose();
    }

    internal virtual bool _addClient(KeepAliveNotification notification)
    {
        global::Doroti.Framework.Foundation.Listenable handle__4407 = ((KeepAliveNotification)notification).handle;
        _handles ??= new DartMap<global::Doroti.Framework.Foundation.Listenable, global::System.Action>();
        DartRuntimePrimitives.Assert(() => !this._handles!.ContainsKey(handle__4407));
        this._handles![handle__4407] = (global::System.Action)_createCallback(handle__4407);
        handle__4407.addListener(this._handles!.GetValueOrDefault(handle__4407)!);
        if (!this._keepingAlive)
        {
            _keepingAlive = true;
            ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>? childElement__4732 = ((ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>?)(object?)_getChildElement());
            if ((childElement__4732 is not null))
            {
                _updateParentDataOfChild(childElement__4732);
            }
            else
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
                {
                    if (!this.mounted)
                    {
                        return;
                    }
                    ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>? childElement__5346 = ((ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>?)(object?)_getChildElement());
                    DartRuntimePrimitives.Assert(() => (childElement__5346 is not null));
                    _updateParentDataOfChild(childElement__5346!);
                })), debugLabel: "AutomaticKeepAlive.updateParentData");
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>? _getChildElement()
    {
        DartRuntimePrimitives.Assert(() => this.mounted);
        var element__5871 = ((Element?)(object?)this.context)!;
        Element? childElement__5914 = default!;
        element__5871.visitChildren(((global::System.Action<Element>)((child) =>
        {
            childElement__5914 = child;
        })));
        DartRuntimePrimitives.Assert(() => ((childElement__5914 is null) || (childElement__5914 is ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>)));
        return ((ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>?)(object?)childElement__5914)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateParentDataOfChild(ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin> childElement)
    {
        childElement.applyWidgetOutOfTurn(((ParentDataWidget<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>?)(object?)build(this.context))!);
    }

    internal virtual global::System.Action _createCallback(global::Doroti.Framework.Foundation.Listenable handle)
    {
        global::System.Action callback__7626 = default!;
        return callback__7626 = (global::System.Action)(() =>
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!this.mounted)
                    {
                        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("AutomaticKeepAlive handle triggered after AutomaticKeepAlive was disposed.\n" + "Widgets should always trigger their KeepAliveNotification handle when they are " + "deactivated, so that they (or their handle) do not send spurious events later " + "when they are no longer in the tree."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            this._handles!.remove(handle);
            handle.removeListener(() => callback__7626());
            if (!System.Linq.Enumerable.Any(this._handles!))
            {
                if ((FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase) < FoundationRuntimePorts.EnumIndex(global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
                {
                    setState(((global::System.Action)(() =>
                    {
                        _keepingAlive = false;
                    })));
                }
                else
                {
                    _keepingAlive = false;
                    DartAsyncRuntime.scheduleMicrotask((() =>
                    {
                        if ((this.mounted && !System.Linq.Enumerable.Any(this._handles!)))
                        {
                            setState(((global::System.Action)(() =>
                            {
                                DartRuntimePrimitives.Assert(() => !this._keepingAlive);
                            })));
                        }
                    }));
                }
            }
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new KeepAlive(keepAlive: this._keepingAlive, child: this._child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.FlagProperty("_keepingAlive", value: this._keepingAlive, ifTrue: "keeping subtree alive"));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<global::Doroti.Framework.Foundation.Listenable, global::System.Action>>("handles", this._handles, description: ((this._handles is not null) ? $"{checked((long)(this._handles!.Count))} active client{((checked((long)(this._handles!.Count)) == 1L) ? "" : "s")}" : null), ifNull: "no notifications ever received"));
    }

}

public class KeepAliveNotification : Notification
{
    public virtual global::Doroti.Framework.Foundation.Listenable handle { get; private set; } = default!;

    public KeepAliveNotification(global::Doroti.Framework.Foundation.Listenable handle)
    {
        this.handle = handle;
    }

}

public class KeepAliveHandle : global::Doroti.Framework.Foundation.ChangeNotifier
{
    public virtual void dispose()
    {
        notifyListeners();
        base.dispose();
    }

}

public interface AutomaticKeepAliveClientMixin<T> where T : StatefulWidget
{
    KeepAliveHandle? _keepAliveHandle { get; set; }

    public void _ensureKeepAlive();
    public void _releaseKeepAlive();
    public bool wantKeepAlive { get; }
    public void updateKeepAlive();
    public void initState();
    public void deactivate();
    public Widget build(BuildContext context);
}

internal class _NullWidget__automatic_keep_alive : StatelessWidget
{
    internal _NullWidget__automatic_keep_alive()
    {
    }

    public override Widget build(BuildContext context)
    {
        throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Widgets that mix AutomaticKeepAliveClientMixin into their State must " + "call super.build() but must ignore the return value of the superclass."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


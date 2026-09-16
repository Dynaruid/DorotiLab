// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/automatic_keep_alive.dart
using Doroti.Runtime;

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
        _child = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<KeepAliveNotification>(onNotification: _addClient, child: widget.child));
    }

    public override void dispose()
    {
        if (_handles is not null)
        {
            foreach (global::Doroti.Framework.Foundation.Listenable handle in _handles!.Keys)
            {
                handle.removeListener(_handles!.GetValueOrDefault(handle)!);
            }
        }
        base.dispose();
    }

    internal virtual bool _addClient(KeepAliveNotification notification)
    {
        global::Doroti.Framework.Foundation.Listenable handleLocal = notification.handle;
        _handles ??= new DartMap<global::Doroti.Framework.Foundation.Listenable, global::System.Action>();
        DartRuntimePrimitives.Assert(() => !_handles!.ContainsKey(handleLocal));
        _handles![handleLocal] = _createCallback(handleLocal);
        handleLocal.addListener(_handles!.GetValueOrDefault(handleLocal)!);
        if (!_keepingAlive)
        {
            _keepingAlive = true;
            ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>? childElement = _getChildElement();
            if (childElement is not null)
            {
                _updateParentDataOfChild(childElement);
            }
            else
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
                {
                    if (!mounted)
                    {
                        return;
                    }
                    ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>? childElementLocal = _getChildElement();
                    DartRuntimePrimitives.Assert(() => childElementLocal is not null);
                    _updateParentDataOfChild(childElementLocal!);
                }, debugLabel: "AutomaticKeepAlive.updateParentData");
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>? _getChildElement()
    {
        DartRuntimePrimitives.Assert(() => mounted);
        var element = ((Element?)context)!;
        Element? childElement = default!;
        element.visitChildren((child) =>
        {
            childElement = child;
        });
        DartRuntimePrimitives.Assert(() => (childElement is null) || (childElement is ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>));
        return ((ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>?)childElement)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateParentDataOfChild(ParentDataElement<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin> childElement)
    {
        childElement.applyWidgetOutOfTurn(((ParentDataWidget<global::Doroti.Framework.Rendering.KeepAliveParentDataMixin>?)build(context))!);
    }

    internal virtual global::System.Action _createCallback(global::Doroti.Framework.Foundation.Listenable handle)
    {
        global::System.Action callback = default!;
        return callback = () =>
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if (!mounted)
                    {
                        throw DartRuntimePrimitives.AsException(FlutterError.Create("AutomaticKeepAlive handle triggered after AutomaticKeepAlive was disposed.\n" + "Widgets should always trigger their KeepAliveNotification handle when they are " + "deactivated, so that they (or their handle) do not send spurious events later " + "when they are no longer in the tree."));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            _handles!.remove(handle);
            handle.removeListener(callback);
            if (!Enumerable.Any(_handles!))
            {
                if (FoundationRuntimePorts.EnumIndex(Scheduler.SchedulerBinding.instance.schedulerPhase) < FoundationRuntimePorts.EnumIndex(Scheduler.SchedulerPhase.persistentCallbacks))
                {
                    setState(() =>
                    {
                        _keepingAlive = false;
                    });
                }
                else
                {
                    _keepingAlive = false;
                    DartAsyncRuntime.scheduleMicrotask(() =>
                    {
                        if (mounted && !Enumerable.Any(_handles!))
                        {
                            setState(() =>
                            {
                                DartRuntimePrimitives.Assert(() => !_keepingAlive);
                            });
                        }
                    });
                }
            }
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new KeepAlive(keepAlive: _keepingAlive, child: _child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.FlagProperty("_keepingAlive", value: _keepingAlive, ifTrue: "keeping subtree alive"));
        description.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<DartMap<global::Doroti.Framework.Foundation.Listenable, global::System.Action>>("handles", _handles, description: (_handles is not null) ? $"{checked((long)_handles!.Count)} active client{((checked(_handles!.Count) == 1L) ? "" : "s")}" : null, ifNull: "no notifications ever received"));
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
    public override void dispose()
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
        throw DartRuntimePrimitives.AsException(FlutterError.Create("Widgets that mix AutomaticKeepAliveClientMixin into their State must " + "call super.build() but must ignore the return value of the superclass."));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


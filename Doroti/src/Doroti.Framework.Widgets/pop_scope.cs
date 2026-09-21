// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/pop_scope.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate void PopInvokedCallback(bool didPop);

public class PopScope<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Action<bool, T?>? onPopInvokedWithResult { get; private set; }
    public virtual Action<bool>? onPopInvoked { get; private set; }
    public virtual bool canPop { get; private set; } = default!;

    public PopScope(
        Key? key = null,
        Widget child = default!,
        bool canPop = true,
        Action<bool, T?>? onPopInvokedWithResult = null,
        Action<bool>? onPopInvoked = null
    )
        : base(key: key)
    {
        this.child = child;
        this.canPop = canPop;
        this.onPopInvokedWithResult = onPopInvokedWithResult;
        this.onPopInvoked = onPopInvoked;
        System.Diagnostics.Debug.Assert((onPopInvokedWithResult is null) || (onPopInvoked is null));
    }

    internal virtual void _callPopInvoked(bool didPop, T? result)
    {
        if (onPopInvokedWithResult is not null)
        {
            onPopInvokedWithResult!(didPop, result);
            return;
        }
        onPopInvoked?.Invoke(didPop);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _PopScopeState__pop_scope<T>());
}

internal class _PopScopeState__pop_scope<T> : State<PopScope<T>>, IPopEntry
{
    ValueListenable<bool> IPopEntry.canPopNotifier => canPopNotifier;

    void IPopEntry.onPopInvokedWithResultObject(bool didPop, object? result) =>
        onPopInvokedWithResult(didPop, result is null ? default : (T)result);

    internal virtual IModalRoute? _route { get; set; } = default!;
    public virtual ValueNotifier<bool> canPopNotifier { get; private set; } = default!;

    public virtual void onPopInvoked(bool didPop)
    {
        throw new NotImplementedException();
    }

    public virtual void onPopInvokedWithResult(bool didPop, T? result)
    {
        widget._callPopInvoked(didPop, result);
    }

    public override void initState()
    {
        base.initState();
        canPopNotifier = new ValueNotifier<bool>(widget.canPop);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        IModalRoute? nextRoute = ModalRoute<object>.untypedOf(context);
        if (!Equals(nextRoute, _route))
        {
            _route?.unregisterPopEntry(this);
            _route = nextRoute;
            _route?.registerPopEntry(this);
        }
    }

    public override void didUpdateWidget(PopScope<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        canPopNotifier.value = widget.canPop;
    }

    public override void dispose()
    {
        _route?.unregisterPopEntry(this);
        canPopNotifier.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context) => widget.child;
}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/pop_scope.dart
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

namespace Doroti.Generated.Framework.Widgets;

public delegate void PopInvokedCallback(bool didPop);

public class PopScope<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Action<bool, T?>? onPopInvokedWithResult { get; private set; }
    public virtual global::System.Action<bool>? onPopInvoked { get; private set; }
    public virtual bool canPop { get; private set; } = default!;

    public PopScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, bool canPop = true, global::System.Action<bool, T?>? onPopInvokedWithResult = null, global::System.Action<bool>? onPopInvoked = null) : base(key: key)
    {
        this.child = child;
        this.canPop = canPop;
        this.onPopInvokedWithResult = onPopInvokedWithResult;
        this.onPopInvoked = onPopInvoked;
        System.Diagnostics.Debug.Assert(((onPopInvokedWithResult is null) || (onPopInvoked is null)));
    }

    internal virtual void _callPopInvoked(bool didPop, T? result)
    {
        if ((this.onPopInvokedWithResult is not null))
        {
            this.onPopInvokedWithResult!(didPop, result);
            return;
        }
        this.onPopInvoked?.Invoke(didPop);
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PopScopeState__pop_scope<T>());
}

internal class _PopScopeState__pop_scope<T> : State<PopScope<T>>
{
    internal virtual dynamic _route { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool> canPopNotifier { get; private set; } = default!;

    public virtual void onPopInvoked(bool didPop)
    {
        throw new NotImplementedException();
    }

    public virtual void onPopInvokedWithResult(bool didPop, T? result)
    {
        this.widget._callPopInvoked(didPop, result);
    }

    public override void initState()
    {
        base.initState();
        canPopNotifier = new global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool>(((PopScope<T>)(object)this.widget).canPop);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        dynamic nextRoute__7173 = ModalRoute<object>.of<object>(this.context);
        if ((!object.Equals(nextRoute__7173, this._route)))
        {
            ((dynamic)this._route)?.unregisterPopEntry(this);
            _route = nextRoute__7173;
            ((dynamic)this._route)?.registerPopEntry(this);
        }
    }

    public override void didUpdateWidget(PopScope<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        this.canPopNotifier.value = ((PopScope<T>)(object)this.widget).canPop;
    }

    public override void dispose()
    {
        ((dynamic)this._route)?.unregisterPopEntry(this);
        this.canPopNotifier.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context) => ((PopScope<T>)(object)this.widget).child;
}


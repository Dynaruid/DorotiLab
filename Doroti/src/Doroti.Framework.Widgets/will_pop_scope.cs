// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/will_pop_scope.dart
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

public class WillPopScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual global::System.Func<Future<bool>>? onWillPop { get; private set; }

    public WillPopScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, global::System.Func<Future<bool>>? onWillPop = default!) : base(key: key)
    {
        this.child = child;
        this.onWillPop = onWillPop;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WillPopScopeState__will_pop_scope());
}

internal class _WillPopScopeState__will_pop_scope : State<WillPopScope>
{
    internal virtual dynamic _route { get; set; } = default!;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if ((((WillPopScope)this.widget).onWillPop is not null))
        {
            ((dynamic)this._route)?.removeScopedWillPopCallback(((WillPopScope)this.widget).onWillPop!);
        }
        _route = ModalRoute<object>.of<object>(this.context);
        if ((((WillPopScope)this.widget).onWillPop is not null))
        {
            ((dynamic)this._route)?.addScopedWillPopCallback(((WillPopScope)this.widget).onWillPop!);
        }
    }

    public override void didUpdateWidget(WillPopScope oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals((global::System.Func<Future<bool>>?)((WillPopScope)this.widget).onWillPop, (global::System.Func<Future<bool>>?)((WillPopScope)oldWidget).onWillPop)) && (this._route is not null)))
        {
            if ((((WillPopScope)oldWidget).onWillPop is not null))
            {
                ((dynamic)this._route!).removeScopedWillPopCallback(((WillPopScope)oldWidget).onWillPop!);
            }
            if ((((WillPopScope)this.widget).onWillPop is not null))
            {
                ((dynamic)this._route!).addScopedWillPopCallback(((WillPopScope)this.widget).onWillPop!);
            }
        }
    }

    public override void dispose()
    {
        if ((((WillPopScope)this.widget).onWillPop is not null))
        {
            ((dynamic)this._route)?.removeScopedWillPopCallback(((WillPopScope)this.widget).onWillPop!);
        }
        base.dispose();
    }

    public override Widget build(BuildContext context) => ((WillPopScope)this.widget).child;
}


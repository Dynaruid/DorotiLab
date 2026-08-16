// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/navigator_pop_handler.dart
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

public class NavigatorPopHandler<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual global::System.Action? onPop { get; private set; }
    public virtual global::System.Action<T?>? onPopWithResult { get; private set; }

    public NavigatorPopHandler(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action? onPop = null, global::System.Action<T?>? onPopWithResult = null, bool enabled = true, Widget child = default!) : base(key: key)
    {
        this.onPop = onPop;
        this.onPopWithResult = onPopWithResult;
        this.enabled = enabled;
        this.child = child;
        System.Diagnostics.Debug.Assert(((onPop is null) || (onPopWithResult is null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _NavigatorPopHandlerState__navigator_pop_handler<T>());
}

internal class _NavigatorPopHandlerState__navigator_pop_handler<T> : State<NavigatorPopHandler<T>>
{
    internal virtual bool _canPop { get; set; } = true;

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new PopScope<T>(canPop: (!((NavigatorPopHandler<T>)(object)this.widget).enabled || this._canPop), onPopInvokedWithResult: ((global::System.Action<bool, T?>)((didPop, result) => {
if (didPop)
{
    return;
}
((NavigatorPopHandler<T>)(object)this.widget).onPop?.Invoke();
((NavigatorPopHandler<T>)(object)this.widget).onPopWithResult?.Invoke(result);
})), child: new NotificationListener<NavigationNotification>(onNotification: ((global::System.Func<NavigationNotification, bool>?)((notification) => {
bool nextCanPop__4598 = !((NavigationNotification)notification).canHandlePop;
if ((nextCanPop__4598 != this._canPop))
{
    setState(((global::System.Action)(() => {
_canPop = nextCanPop__4598;
})));
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: ((NavigatorPopHandler<T>)(object)this.widget).child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void PopResultCallback<T>(T? result);


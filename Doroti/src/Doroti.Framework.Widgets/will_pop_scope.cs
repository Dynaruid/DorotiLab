// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/will_pop_scope.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class WillPopScope : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Func<Future<bool>>? onWillPop { get; private set; }

    public WillPopScope(Key? key = null, Widget child = default!, Func<Future<bool>>? onWillPop = default!) : base(key: key)
    {
        this.child = child;
        this.onWillPop = onWillPop;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WillPopScopeState__will_pop_scope());
}

internal class _WillPopScopeState__will_pop_scope : State<WillPopScope>
{
    internal virtual IModalRoute? _route { get; set; } = default!;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (widget.onWillPop is not null)
        {
            _route?.removeScopedWillPopCallback(widget.onWillPop!);
        }
        _route = ModalRoute<object>.untypedOf(context);
        if (widget.onWillPop is not null)
        {
            _route?.addScopedWillPopCallback(widget.onWillPop!);
        }
    }

    public override void didUpdateWidget(WillPopScope oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(widget.onWillPop, oldWidget.onWillPop)) && (_route is not null))
        {
            if (oldWidget.onWillPop is not null)
            {
                _route!.removeScopedWillPopCallback(oldWidget.onWillPop!);
            }
            if (widget.onWillPop is not null)
            {
                _route!.addScopedWillPopCallback(widget.onWillPop!);
            }
        }
    }

    public override void dispose()
    {
        if (widget.onWillPop is not null)
        {
            _route?.removeScopedWillPopCallback(widget.onWillPop!);
        }
        base.dispose();
    }

    public override Widget build(BuildContext context) => widget.child;
}

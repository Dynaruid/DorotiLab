// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/navigator_pop_handler.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class NavigatorPopHandler<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual Action? onPop { get; private set; }
    public virtual System.Action<T?>? onPopWithResult { get; private set; }

    public NavigatorPopHandler(Key? key = null, Action? onPop = null, System.Action<T?>? onPopWithResult = null, bool enabled = true, Widget child = default!) : base(key: key)
    {
        this.onPop = onPop;
        this.onPopWithResult = onPopWithResult;
        this.enabled = enabled;
        this.child = child;
        System.Diagnostics.Debug.Assert((onPop is null) || (onPopWithResult is null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _NavigatorPopHandlerState__navigator_pop_handler<T>());
}

internal class _NavigatorPopHandlerState__navigator_pop_handler<T> : State<NavigatorPopHandler<T>>
{
    internal virtual bool _canPop { get; set; } = true;

    public override Widget build(BuildContext context)
    {
        return new PopScope<T>(canPop: !widget.enabled || _canPop, onPopInvokedWithResult: (didPop, result) =>
        {
            if (didPop)
            {
                return;
            }
            widget.onPop?.Invoke();
            widget.onPopWithResult?.Invoke(result);
        }, child: new NotificationListener<NavigationNotification>(onNotification: (notification) =>
        {
            bool nextCanPop = !notification.canHandlePop;
            if (nextCanPop != _canPop)
            {
                setState(() =>
                {
                    _canPop = nextCanPop;
                });
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void PopResultCallback<T>(T? result);


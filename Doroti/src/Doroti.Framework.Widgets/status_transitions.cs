// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/status_transitions.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class StatusTransitionWidget : StatefulWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;

    protected StatusTransitionWidget(Key? key = null, Animation<double> animation = default!)
        : base(key: key)
    {
        this.animation = animation;
    }

    public abstract Widget build(BuildContext context);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _StatusTransitionState__status_transitions()
        );
}

internal class _StatusTransitionState__status_transitions : State<StatusTransitionWidget>
{
    public override void initState()
    {
        base.initState();
        widget.animation.addStatusListener(_animationStatusChanged);
    }

    public override void didUpdateWidget(StatusTransitionWidget oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.animation, oldWidget.animation))
        {
            oldWidget.animation.removeStatusListener(_animationStatusChanged);
            widget.animation.addStatusListener(_animationStatusChanged);
        }
    }

    public override void dispose()
    {
        widget.animation.removeStatusListener(_animationStatusChanged);
        base.dispose();
    }

    internal virtual void _animationStatusChanged(AnimationStatus status)
    {
        setState(() => { });
    }

    public override Widget build(BuildContext context)
    {
        return widget.build(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

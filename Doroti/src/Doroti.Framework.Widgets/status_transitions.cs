// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/status_transitions.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class StatusTransitionWidget : StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;

    protected StatusTransitionWidget(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> animation = default!) : base(key: key)
    {
        this.animation = animation;
    }

    public abstract Widget build(BuildContext context);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StatusTransitionState__status_transitions());
}

internal class _StatusTransitionState__status_transitions : State<StatusTransitionWidget>
{
    public override void initState()
    {
        base.initState();
        ((StatusTransitionWidget)this.widget).animation.addStatusListener((AnimationStatusListener)this._animationStatusChanged);
    }

    public override void didUpdateWidget(StatusTransitionWidget oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((StatusTransitionWidget)this.widget).animation, ((StatusTransitionWidget)oldWidget).animation)))
        {
            ((StatusTransitionWidget)oldWidget).animation.removeStatusListener((AnimationStatusListener)this._animationStatusChanged);
            ((StatusTransitionWidget)this.widget).animation.addStatusListener((AnimationStatusListener)this._animationStatusChanged);
        }
    }

    public override void dispose()
    {
        ((StatusTransitionWidget)this.widget).animation.removeStatusListener((AnimationStatusListener)this._animationStatusChanged);
        base.dispose();
    }

    internal virtual void _animationStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() =>
        {
        })));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)this.widget.build(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


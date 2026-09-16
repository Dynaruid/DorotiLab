// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/dual_transition_builder.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate Widget AnimatedTransitionBuilder(BuildContext context, Animation<double> animation, Widget? child);

public class DualTransitionBuilder : StatefulWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget?, Widget> forwardBuilder { get; private set; } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget?, Widget> reverseBuilder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public DualTransitionBuilder(Key? key = null, Animation<double> animation = default!, Func<BuildContext, Animation<double>, Widget?, Widget> forwardBuilder = default!, Func<BuildContext, Animation<double>, Widget?, Widget> reverseBuilder = default!, Widget? child = null) : base(key: key)
    {
        this.animation = animation;
        this.forwardBuilder = forwardBuilder;
        this.reverseBuilder = reverseBuilder;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DualTransitionBuilderState__dual_transition_builder());
}

internal class _DualTransitionBuilderState__dual_transition_builder : State<DualTransitionBuilder>
{
    internal virtual AnimationStatus _effectiveAnimationStatus { get; set; } = default!;
    internal virtual ProxyAnimation _forwardAnimation { get; private set; } = new ProxyAnimation();
    internal virtual ProxyAnimation _reverseAnimation { get; private set; } = new ProxyAnimation();

    public override void initState()
    {
        base.initState();
        _effectiveAnimationStatus = widget.animation.status;
        widget.animation.addStatusListener(_animationListener);
        _updateAnimations();
    }

    internal virtual void _animationListener(AnimationStatus animationStatus)
    {
        AnimationStatus oldEffective = _effectiveAnimationStatus;
        _effectiveAnimationStatus = _calculateEffectiveAnimationStatus(lastEffective: _effectiveAnimationStatus, current: animationStatus);
        if (!Equals(oldEffective, _effectiveAnimationStatus))
        {
            _updateAnimations();
        }
    }

    public override void didUpdateWidget(DualTransitionBuilder oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.animation, widget.animation))
        {
            oldWidget.animation.removeStatusListener(_animationListener);
            widget.animation.addStatusListener(_animationListener);
            _animationListener(widget.animation.status);
        }
    }

    internal virtual AnimationStatus _calculateEffectiveAnimationStatus(AnimationStatus lastEffective, AnimationStatus current)
    {
        switch (current)
        {
            case AnimationStatus.dismissed:
            case AnimationStatus.completed:
                {
                    return current;
                }
            case AnimationStatus.forward:
                {
                    switch (lastEffective)
                    {
                        case AnimationStatus.dismissed:
                        case AnimationStatus.completed:
                        case AnimationStatus.forward:
                            {
                                return current;
                            }
                        case AnimationStatus.reverse:
                            {
                                return lastEffective;
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                }
            case AnimationStatus.reverse:
                {
                    switch (lastEffective)
                    {
                        case AnimationStatus.dismissed:
                        case AnimationStatus.completed:
                        case AnimationStatus.reverse:
                            {
                                return current;
                            }
                        case AnimationStatus.forward:
                            {
                                return lastEffective;
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimations()
    {
        switch (_effectiveAnimationStatus)
        {
            case AnimationStatus.dismissed:
            case AnimationStatus.forward:
                {
                    _forwardAnimation.parent = widget.animation;
                    _reverseAnimation.parent = AnimationsLibrary.kAlwaysDismissedAnimation;
                    break;
                }
            case AnimationStatus.reverse:
            case AnimationStatus.completed:
                {
                    _forwardAnimation.parent = AnimationsLibrary.kAlwaysCompleteAnimation;
                    _reverseAnimation.parent = DartRuntimePrimitives.ConvertValue<Animation<double>>(new ReverseAnimation(widget.animation));
                    break;
                }
        }
    }

    public override void dispose()
    {
        widget.animation.removeStatusListener(_animationListener);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return widget.forwardBuilder(context, _forwardAnimation, widget.reverseBuilder(context, _reverseAnimation, widget.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


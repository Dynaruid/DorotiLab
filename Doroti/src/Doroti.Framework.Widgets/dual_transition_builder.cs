// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/dual_transition_builder.dart
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

public delegate Widget AnimatedTransitionBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, Widget? child);

public class DualTransitionBuilder : StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget?, Widget> forwardBuilder { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget?, Widget> reverseBuilder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public DualTransitionBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Animation<double> animation = default!, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget?, Widget> forwardBuilder = default!, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget?, Widget> reverseBuilder = default!, Widget? child = null) : base(key: key)
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
    internal virtual global::Doroti.Framework.Animation.AnimationStatus _effectiveAnimationStatus { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _forwardAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _reverseAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation();

    public override void initState()
    {
        base.initState();
        _effectiveAnimationStatus = ((DualTransitionBuilder)this.widget).animation.status;
        ((DualTransitionBuilder)this.widget).animation.addStatusListener((AnimationStatusListener)this._animationListener);
        _updateAnimations();
    }

    internal virtual void _animationListener(global::Doroti.Framework.Animation.AnimationStatus animationStatus)
    {
        global::Doroti.Framework.Animation.AnimationStatus oldEffective__4606 = this._effectiveAnimationStatus;
        _effectiveAnimationStatus = _calculateEffectiveAnimationStatus(lastEffective: this._effectiveAnimationStatus, current: animationStatus);
        if ((!object.Equals(oldEffective__4606, this._effectiveAnimationStatus)))
        {
            _updateAnimations();
        }
    }

    public override void didUpdateWidget(DualTransitionBuilder oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((DualTransitionBuilder)oldWidget).animation, ((DualTransitionBuilder)this.widget).animation)))
        {
            ((DualTransitionBuilder)oldWidget).animation.removeStatusListener((AnimationStatusListener)this._animationListener);
            ((DualTransitionBuilder)this.widget).animation.addStatusListener((AnimationStatusListener)this._animationListener);
            _animationListener(((DualTransitionBuilder)this.widget).animation.status);
        }
    }

    internal virtual global::Doroti.Framework.Animation.AnimationStatus _calculateEffectiveAnimationStatus(global::Doroti.Framework.Animation.AnimationStatus lastEffective, global::Doroti.Framework.Animation.AnimationStatus current)
    {
        switch (current)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
            case global::Doroti.Framework.Animation.AnimationStatus.completed:
                {
                    return current;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.forward:
                {
                    switch (lastEffective)
                    {
                        case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
                        case global::Doroti.Framework.Animation.AnimationStatus.completed:
                        case global::Doroti.Framework.Animation.AnimationStatus.forward:
                            {
                                return current;
                            }
                        case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                            {
                                return lastEffective;
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                {
                    switch (lastEffective)
                    {
                        case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
                        case global::Doroti.Framework.Animation.AnimationStatus.completed:
                        case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                            {
                                return current;
                            }
                        case global::Doroti.Framework.Animation.AnimationStatus.forward:
                            {
                                return lastEffective;
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimations()
    {
        switch (this._effectiveAnimationStatus)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
            case global::Doroti.Framework.Animation.AnimationStatus.forward:
                {
                    this._forwardAnimation.parent = ((DualTransitionBuilder)this.widget).animation;
                    this._reverseAnimation.parent = global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation;
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.reverse:
            case global::Doroti.Framework.Animation.AnimationStatus.completed:
                {
                    this._forwardAnimation.parent = global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation;
                    this._reverseAnimation.parent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(((DualTransitionBuilder)this.widget).animation));
                    break;
                }
        }
    }

    public override void dispose()
    {
        ((DualTransitionBuilder)this.widget).animation.removeStatusListener((AnimationStatusListener)this._animationListener);
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return this.widget.forwardBuilder(context, this._forwardAnimation, this.widget.reverseBuilder(context, this._reverseAnimation, ((DualTransitionBuilder)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


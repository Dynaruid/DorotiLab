// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/status_transitions.dart
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
        setState(((global::System.Action)(() => {
})));
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)this.widget.build(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


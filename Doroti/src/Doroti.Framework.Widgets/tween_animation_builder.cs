// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/tween_animation_builder.dart
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

public class TweenAnimationBuilder<T> : ImplicitlyAnimatedWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Tween<T> tween { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public TweenAnimationBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Animation.Tween<T> tween = default!, Duration duration = default!, global::Doroti.Generated.Framework.Animation.Curve curve = default!, global::System.Func<BuildContext, T, Widget?, Widget> builder = default!, global::System.Action? onEnd = null, Widget? child = null) : base(key: key, duration: duration, curve: curve ?? global::Doroti.Generated.Framework.Animation.Curves.linear, onEnd: onEnd)
    {
        this.tween = tween;
        this.builder = builder;
        this.child = child;
    }

    public override ImplicitlyAnimatedWidgetState<ImplicitlyAnimatedWidget> createState()
    {
        return ((ImplicitlyAnimatedWidgetState<ImplicitlyAnimatedWidget>)(object?)new _TweenAnimationBuilderState__tween_animation_builder<T>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TweenAnimationBuilderState__tween_animation_builder<T> : AnimatedWidgetBaseState<TweenAnimationBuilder<T>>
{
    internal virtual global::Doroti.Generated.Framework.Animation.Tween<T>? _currentTween { get; set; } = default;

    public override void initState()
    {
        _currentTween = ((TweenAnimationBuilder<T>)(object)this.widget).tween;
        this._currentTween!.begin ??= this._currentTween!.end;
        base.initState();
        if (!EqualityComparer<T>.Default.Equals(this._currentTween!.begin, this._currentTween!.end))
        {
            this.controller.forward();
        }
    }

    public override void forEachTween(global::System.Func<global::Doroti.Generated.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Generated.Framework.Animation.IDartTween>, global::Doroti.Generated.Framework.Animation.IDartTween?> visitor)
    {
        DartRuntimePrimitives.Assert(() => (((TweenAnimationBuilder<T>)(object)this.widget).tween.end is not null), () => (object?)"Tween provided to TweenAnimationBuilder must have non-null Tween.end value.");
        _currentTween = ((global::Doroti.Generated.Framework.Animation.Tween<T>?)(object?)visitor(this._currentTween, ((TweenAnimationBuilder<T>)(object)this.widget).tween.end, ((value) => {
DartRuntimePrimitives.Assert(() => false);
throw new InvalidOperationException("Constructor will never be called because null is never provided as current tween.");
throw new InvalidOperationException("Dart closure completed without a value.");
})))!;
    }

    public override Widget build(BuildContext context)
    {
        return this.widget.builder(context, this._currentTween!.evaluate(this.animation), ((TweenAnimationBuilder<T>)(object)this.widget).child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


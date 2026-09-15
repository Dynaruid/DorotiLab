// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/tween_animation_builder.dart
#pragma warning disable CS8600, CS8603, CS8604
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class TweenAnimationBuilder<T> : ImplicitlyAnimatedWidget
{
    public virtual global::Doroti.Framework.Animation.Tween<T> tween { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public TweenAnimationBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Animation.Tween<T> tween = default!, Duration duration = default!, global::Doroti.Framework.Animation.Curve curve = default!, global::System.Func<BuildContext, T, Widget?, Widget> builder = default!, global::System.Action? onEnd = null, Widget? child = null) : base(key: key, duration: duration, curve: curve ?? global::Doroti.Framework.Animation.Curves.linear, onEnd: onEnd)
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
    internal virtual global::Doroti.Framework.Animation.Tween<T>? _currentTween { get; set; } = default;

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

    public override void forEachTween(global::System.Func<global::Doroti.Framework.Animation.IDartTween?, object, global::System.Func<object, global::Doroti.Framework.Animation.IDartTween>, global::Doroti.Framework.Animation.IDartTween?> visitor)
    {
        DartRuntimePrimitives.Assert(() => (((TweenAnimationBuilder<T>)(object)this.widget).tween.end is not null), () => (object?)"Tween provided to TweenAnimationBuilder must have non-null Tween.end value.");
        _currentTween = ((global::Doroti.Framework.Animation.Tween<T>?)(object?)visitor(this._currentTween, ((TweenAnimationBuilder<T>)(object)this.widget).tween.end, ((value) =>
        {
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


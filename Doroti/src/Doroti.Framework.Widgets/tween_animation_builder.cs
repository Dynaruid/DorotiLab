// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/tween_animation_builder.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class TweenAnimationBuilder<T> : ImplicitlyAnimatedWidget
{
    public virtual Tween<T> tween { get; private set; } = default!;
    public virtual Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public TweenAnimationBuilder(
        Key? key = null,
        Tween<T> tween = default!,
        Duration duration = default!,
        Curve curve = default!,
        Func<BuildContext, T, Widget?, Widget> builder = default!,
        Action? onEnd = null,
        Widget? child = null
    )
        : base(key: key, duration: duration, curve: curve ?? Curves.linear, onEnd: onEnd)
    {
        this.tween = tween;
        this.builder = builder;
        this.child = child;
    }

    public override ImplicitlyAnimatedWidgetState<TweenAnimationBuilder<T>> createState()
    {
        return new _TweenAnimationBuilderState__tween_animation_builder<T>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _TweenAnimationBuilderState__tween_animation_builder<T>
    : AnimatedWidgetBaseState<TweenAnimationBuilder<T>>
{
    internal virtual Tween<T>? _currentTween { get; set; } = default;

    public override void initState()
    {
        _currentTween = widget.tween;
        _currentTween!.begin ??= _currentTween!.end;
        base.initState();
        if (!EqualityComparer<T>.Default.Equals(_currentTween!.begin, _currentTween!.end))
        {
            controller.forward();
        }
    }

    public override void forEachTween(
        Func<IDartTween?, object?, Func<object, IDartTween>, IDartTween?> visitor
    )
    {
        DartRuntimePrimitives.Assert(
            () => widget.tween.end is not null,
            () =>
                (object?)
                    "Tween provided to TweenAnimationBuilder must have non-null Tween.end value."
        );
        _currentTween = (
            (Tween<T>?)visitor(
                _currentTween,
                widget.tween.end,
                (value) =>
                {
                    DartRuntimePrimitives.Assert(() => false);
                    throw new InvalidOperationException(
                        "Constructor will never be called because null is never provided as current tween."
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        )!;
    }

    public override Widget build(BuildContext context)
    {
        return widget.builder(context, _currentTween!.evaluate(animation), widget.child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

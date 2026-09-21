// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animation.dart
using Doroti.Runtime;

namespace Doroti.Framework.Animation;

public enum AnimationStatus
{
    dismissed,
    forward,
    reverse,
    completed,
}

public static class AnimationStatusMembers
{
    public static bool isDismissed(this AnimationStatus value) =>
        Equals(value, AnimationStatus.dismissed);

    public static bool isCompleted(this AnimationStatus value) =>
        Equals(value, AnimationStatus.completed);

    public static bool isAnimating(this AnimationStatus value) =>
        value switch
        {
            AnimationStatus.forward => true,
            AnimationStatus.reverse => true,
            AnimationStatus.completed => false,
            AnimationStatus.dismissed => false,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };

    public static bool isForwardOrCompleted(this AnimationStatus value) =>
        value switch
        {
            AnimationStatus.forward => true,
            AnimationStatus.completed => true,
            AnimationStatus.reverse => false,
            AnimationStatus.dismissed => false,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
}

public delegate void AnimationStatusListener(AnimationStatus status);

public delegate T ValueListenableTransformer<T>(T __unnamed_);

public abstract class Animation<T> : Listenable, ValueListenable<T>
{
    protected Animation() { }

    public static Animation<T> CreateFromValueListenable(
        ValueListenable<T> listenable,
        Func<T, T>? transformer = null
    ) => new _ValueListenableDelegateAnimation__animation<T>(listenable, transformer);

    public abstract void addListener(Action listener);
    public abstract void removeListener(Action listener);
    public abstract void addStatusListener(AnimationStatusListener listener);
    public abstract void removeStatusListener(AnimationStatusListener listener);
    public abstract AnimationStatus status { get; }
    public virtual T value
    {
        get =>
            throw new NotSupportedException(
                "The generated getter contract has no base implementation."
            );
        set =>
            throw new NotSupportedException(
                "The generated setter contract has no base implementation."
            );
    }
    public virtual bool isDismissed => AnimationStatusMembers.isDismissed(status);
    public virtual bool isCompleted => AnimationStatusMembers.isCompleted(status);
    public virtual bool isAnimating => AnimationStatusMembers.isAnimating(status);
    public virtual bool isForwardOrCompleted => AnimationStatusMembers.isForwardOrCompleted(status);

    public virtual Animation<U> drive<U>(Animatable<U> child)
    {
        DartRuntimePrimitives.Assert(() => this is Animation<double>);
        return child.animate(((Animation<double>?)(object?)this)!);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}({toStringDetails()})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual string toStringDetails()
    {
        return status switch
        {
            AnimationStatus.forward => "▶",
            AnimationStatus.reverse => "◀",
            AnimationStatus.completed => "⏭",
            AnimationStatus.dismissed => "⏮",
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ValueListenableDelegateAnimation__animation<T> : Animation<T>
{
    internal virtual ValueListenable<T> _listenable { get; private set; } = default!;
    internal virtual Func<T, T>? _transformer { get; private set; }

    internal _ValueListenableDelegateAnimation__animation(
        ValueListenable<T> _listenable,
        Func<T, T>? transformer = null
    )
    {
        this._listenable = _listenable;
        _transformer = transformer;
    }

    public override void addListener(Action listener)
    {
        _listenable.addListener(listener);
    }

    public override void addStatusListener(AnimationStatusListener listener) { }

    public override void removeListener(Action listener)
    {
        _listenable.removeListener(listener);
    }

    public override void removeStatusListener(AnimationStatusListener listener) { }

    public override AnimationStatus status => AnimationStatus.forward;
    public override T value =>
        _transformer is null ? _listenable.value : _transformer.Invoke(_listenable.value);
}

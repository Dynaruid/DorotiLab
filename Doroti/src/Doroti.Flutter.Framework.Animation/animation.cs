// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animation.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Animation;

public enum AnimationStatus
{
    dismissed,
    forward,
    reverse,
    completed
}

public static class AnimationStatusMembers
{
    public static bool isDismissed(this AnimationStatus value) => (object.Equals(value, AnimationStatus.dismissed));
    public static bool isCompleted(this AnimationStatus value) => (object.Equals(value, AnimationStatus.completed));
    public static bool isAnimating(this AnimationStatus value) => (value switch { AnimationStatus.forward => true, AnimationStatus.reverse => true, AnimationStatus.completed => false, AnimationStatus.dismissed => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public static bool isForwardOrCompleted(this AnimationStatus value) => (value switch { AnimationStatus.forward => true, AnimationStatus.completed => true, AnimationStatus.reverse => false, AnimationStatus.dismissed => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
}

public delegate void AnimationStatusListener(AnimationStatus status);

public delegate T ValueListenableTransformer<T>(T __unnamed_);

public abstract class Animation<T> : Listenable, ValueListenable<T>
{
    protected Animation()
    {
    }

    public static Animation<T> CreateFromValueListenable(ValueListenable<T> listenable, Func<T, T>? transformer = null)
        => new _ValueListenableDelegateAnimation__animation<T>(listenable, transformer);

    public abstract void addListener(Action listener);
    public abstract void removeListener(Action listener);
    public abstract void addStatusListener(AnimationStatusListener listener);
    public abstract void removeStatusListener(AnimationStatusListener listener);
    public abstract AnimationStatus status { get; }
    public virtual T value
    {
        get => throw new NotSupportedException("Dart getter contract has no base implementation.");
        set => throw new NotSupportedException("Dart setter contract has no base implementation.");
    }
    public virtual bool isDismissed => global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isDismissed(this.status);
    public virtual bool isCompleted => global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(this.status);
    public virtual bool isAnimating => global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isAnimating(this.status);
    public virtual bool isForwardOrCompleted => global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isForwardOrCompleted(this.status);
    public virtual Animation<U> drive<U>(Animatable<U> child)
    {
        DartRuntimePrimitives.Assert(() => (this is Animation<double>));
        return child.animate(((Animation<double>?)(object?)this)!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({toStringDetails()})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringDetails()
    {
        return (this.status switch { AnimationStatus.forward => "▶", AnimationStatus.reverse => "◀", AnimationStatus.completed => "⏭", AnimationStatus.dismissed => "⏮", _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ValueListenableDelegateAnimation__animation<T> : Animation<T>
{
    internal virtual ValueListenable<T> _listenable { get; private set; } = default!;
    internal virtual Func<T, T>? _transformer { get; private set; }

    internal _ValueListenableDelegateAnimation__animation(ValueListenable<T> _listenable, Func<T, T>? transformer = null)
    {
        this._listenable = _listenable;
        this._transformer = transformer;
    }

    public override void addListener(Action listener)
    {
        this._listenable.addListener(listener);
    }

    public override void addStatusListener(AnimationStatusListener listener)
    {
    }

    public override void removeListener(Action listener)
    {
        this._listenable.removeListener(listener);
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
    }

    public override AnimationStatus status => AnimationStatus.forward;
    public override T value => ((this._transformer is null ? this._listenable.value : this._transformer.Invoke(this._listenable.value)));
}


// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/tween.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Animation;

public delegate T AnimatableCallback<T>(double value);

public abstract class Animatable<T>
{
    protected Animatable()
    {
    }

    public static Animatable<T> CreateFromCallback(Func<double, T> callback)
        => new _CallbackAnimatable__tween<T>(callback);

    public abstract T transform(double t);
    public virtual T evaluate(Animation<double> animation) => transform(animation.value);
    public virtual Animation<T> animate(Animation<double> parent)
    {
        return new _AnimatedEvaluation__tween<T>(parent, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animatable<T> chain(Animatable<double> parent)
    {
        return new _ChainedEvaluation__tween<T>(parent, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
internal class _CallbackAnimatable__tween<T> : Animatable<T>
{
    internal virtual Func<double, T> _callback { get; private set; } = default!;

    internal _CallbackAnimatable__tween(Func<double, T> _callback)
    {
        this._callback = _callback;
    }

    public override T transform(double t)
    {
        return _callback(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AnimatedEvaluation__tween<T> : Animation<T>, AnimationWithParentMixin<double>
{
    public virtual Animation<double> parent { get; private set; } = default!;
    internal virtual Animatable<T> _evaluatable { get; private set; } = default!;

    internal _AnimatedEvaluation__tween(Animation<double> parent, Animatable<T> _evaluatable)
    {
        this.parent = parent;
        this._evaluatable = _evaluatable;
    }

    public override T value => _evaluatable.evaluate(parent);
    public override string ToString()
    {
        return $"{parent}➩{_evaluatable}➩{value}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringDetails()
    {
        return $"{base.toStringDetails()} {_evaluatable}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addListener(Action listener) => parent.addListener(listener);
    public override void removeListener(Action listener) => parent.removeListener(listener);
    public override void addStatusListener(AnimationStatusListener listener) => parent.addStatusListener(listener);
    public override void removeStatusListener(AnimationStatusListener listener) => parent.removeStatusListener(listener);
    public override AnimationStatus status => parent.status;
}

internal class _ChainedEvaluation__tween<T> : Animatable<T>
{
    internal virtual Animatable<double> _parent { get; private set; } = default!;
    internal virtual Animatable<T> _evaluatable { get; private set; } = default!;

    internal _ChainedEvaluation__tween(Animatable<double> _parent, Animatable<T> _evaluatable)
    {
        this._parent = _parent;
        this._evaluatable = _evaluatable;
    }

    public override T transform(double t)
    {
        return _evaluatable.transform(_parent.transform(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{_parent}➩{_evaluatable}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface IDartTween
{
    object? begin { get; set; }
    object? end { get; set; }
    object? evaluate(Animation<double> animation);
    object? transform(double t);
}

public class Tween<T> : Animatable<T>, IDartTween
{
    public virtual T? begin { get; set; } = default;
    public virtual T? end { get; set; } = default;

    public Tween(T? begin = default, T? end = default)
    {
        this.begin = begin;
        this.end = end;
    }

    public virtual T lerp(double t)
    {
        DartRuntimePrimitives.Assert(() => begin is not null);
        DartRuntimePrimitives.Assert(() => end is not null);
        return DartRuntimePrimitives.LerpTweenValue(
            DartRuntimePrimitives.RequireNonNull(begin),
            DartRuntimePrimitives.RequireNonNull(end),
            t);
    }

    public override T transform(double t)
    {
        if (t == 0.0)
        {
            return ((T?)(object?)begin)!;
        }
        if (t == 1.0)
        {
            return ((T?)(object?)end)!;
        }
        return lerp(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "Animatable")}({begin} → {end})";

    object? IDartTween.begin { get => begin; set => begin = value is null ? default : DartRuntimePrimitives.ConvertValue<T>(value); }
    object? IDartTween.end { get => end; set => end = value is null ? default : DartRuntimePrimitives.ConvertValue<T>(value); }
    object? IDartTween.evaluate(Animation<double> animation) => evaluate(animation);
    object? IDartTween.transform(double t) => transform(t);
}

public class ReverseTween<T> : Tween<T>
{
    public virtual Tween<T> parent { get; private set; } = default!;

    public ReverseTween(Tween<T> parent) : base(begin: parent.end, end: parent.begin)
    {
        this.parent = parent;
    }

    public override T lerp(double t) => parent.lerp(1.0 - t);
}

public class ColorTween : Tween<Color?>
{
    public ColorTween(Color? begin = null, Color? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Ui.Color? lerp(double t) => Dart_uiLibrary.Color.lerp(begin, end, t);
}

public class SizeTween : Tween<Size?>
{
    public SizeTween(Size? begin = null, Size? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Ui.Size? lerp(double t) => Dart_uiLibrary.Size.lerp(begin, end, t);
}

public class RectTween : Tween<Rect?>
{
    public RectTween(Rect? begin = null, Rect? end = null) : base(begin: begin, end: end)
    {
    }

    public override global::Doroti.Ui.Rect? lerp(double t) => Dart_uiLibrary.Rect.lerp(begin, end, t);
}

public class IntTween : Tween<long>
{
    public IntTween(long? begin = null, long? end = null) : base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))
    {
    }

    public override long lerp(double t) => (DartRuntimePrimitives.RequireValue(begin) + ((DartRuntimePrimitives.RequireValue(end) - DartRuntimePrimitives.RequireValue(begin)) * t)).round();
}

public class StepTween : Tween<long>
{
    public StepTween(long? begin = null, long? end = null) : base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))
    {
    }

    public override long lerp(double t) => (DartRuntimePrimitives.RequireValue(begin) + ((DartRuntimePrimitives.RequireValue(end) - DartRuntimePrimitives.RequireValue(begin)) * t)).floor();
}

public class ConstantTween<T> : Tween<T>
{
    public ConstantTween(T value) : base(begin: value, end: value)
    {
    }

    public override T lerp(double t) => ((T?)(object?)begin)!;
    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ConstantTween")}(value: {begin})";
}

public class CurveTween : Animatable<double>
{
    public virtual Curve curve { get; set; } = default!;

    public CurveTween(Curve curve)
    {
        this.curve = curve;
    }

    public override double transform(double t)
    {
        if ((t == 0.0) || (t == 1.0))
        {
            DartRuntimePrimitives.Assert(() => curve.transform(t).round() == t);
            return t;
        }
        return curve.transform(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "CurveTween")}(curve: {curve})";
}

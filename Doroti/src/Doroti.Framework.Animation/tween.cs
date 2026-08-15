// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/tween.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Animation;

public delegate T AnimatableCallback<T>(double value);

public abstract class Animatable<T>
{
    protected Animatable()
    {
    }

    public static Animatable<T> CreateFromCallback(Func<double, T> callback)
        => new _CallbackAnimatable__tween<T>(callback);

    public abstract T transform(double t);
    public virtual T evaluate(Animation<double> animation) => transform(((Animation<double>)animation).value);
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
        return this._callback(t);
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

    public override T value => this._evaluatable.evaluate(this.parent);
    public override string ToString()
    {
        return $"{this.parent}➩{this._evaluatable}➩{this.value}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringDetails()
    {
        return $"{base.toStringDetails()} {this._evaluatable}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addListener(Action listener) => this.parent.addListener((Action)listener);
    public override void removeListener(Action listener) => this.parent.removeListener((Action)listener);
    public override void addStatusListener(AnimationStatusListener listener) => this.parent.addStatusListener((AnimationStatusListener)listener);
    public override void removeStatusListener(AnimationStatusListener listener) => this.parent.removeStatusListener((AnimationStatusListener)listener);
    public override AnimationStatus status => ((Animation<double>)this.parent).status;
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
        return this._evaluatable.transform(this._parent.transform(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{this._parent}➩{this._evaluatable}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface IDartTween
{
    dynamic? begin { get; set; }
    dynamic? end { get; set; }
    dynamic evaluate(Animation<double> animation);
    dynamic transform(double t);
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
        DartRuntimePrimitives.Assert(() => (this.begin is not null));
        DartRuntimePrimitives.Assert(() => (this.end is not null));
        DartRuntimePrimitives.Assert(() =>
            {
                object result__11290 = default!;
                try
                {
                    result__11290 = (((dynamic)(((object?)(object?)this.begin)!)) + ((dynamic)(((dynamic)((((dynamic)(((object?)(object?)this.end)!)) - ((dynamic)(((object?)(object?)this.begin)!))))) * ((dynamic)t))));
                    _ = ((T?)(object?)result__11290)!;
                    return true;
                }
                catch (NoSuchMethodError)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Cannot lerp between \"{this.begin}\" and \"{this.end}\"."), new ErrorDescription($"The type {DartRuntimePrimitives.RuntimeType(this.begin)} might not fully implement `+`, `-`, and/or `*`. " + "See \"Types with special considerations\" at https://api.flutter.dev/flutter/animation/Tween-class.html " + "for more information.") });
                }
                catch (TypeError)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Cannot lerp between \"{this.begin}\" and \"{this.end}\"."), new ErrorDescription($"The type {DartRuntimePrimitives.RuntimeType(this.begin)} returned a {DartRuntimePrimitives.RuntimeType(result__11290)} after " + "multiplication with a double value. " + "See \"Types with special considerations\" at https://api.flutter.dev/flutter/animation/Tween-class.html " + "for more information.") });
                }
            });
        return ((T?)(object?)(((dynamic)(((object?)(object?)this.begin)!)) + ((dynamic)(((dynamic)((((dynamic)(((object?)(object?)this.end)!)) - ((dynamic)(((object?)(object?)this.begin)!))))) * ((dynamic)t)))))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override T transform(double t)
    {
        if ((t == 0.0))
        {
            return ((T?)(object?)this.begin)!;
        }
        if ((t == 1.0))
        {
            return ((T?)(object?)this.end)!;
        }
        return lerp(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Animatable"))}({this.begin} → {this.end})";

    dynamic? IDartTween.begin { get => begin; set => begin = (T?)value; }
    dynamic? IDartTween.end { get => end; set => end = (T?)value; }
    dynamic IDartTween.evaluate(Animation<double> animation) => evaluate(animation)!;
    dynamic IDartTween.transform(double t) => transform(t)!;
}

public class ReverseTween<T> : Tween<T>
{
    public virtual Tween<T> parent { get; private set; } = default!;

    public ReverseTween(Tween<T> parent) : base(begin: ((Tween<T>)parent).end, end: ((Tween<T>)parent).begin)
    {
        this.parent = parent;
    }

    public override T lerp(double t) => this.parent.lerp((1.0 - t));
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

    public override long lerp(double t) => ((DartRuntimePrimitives.RequireValue(begin) + (((DartRuntimePrimitives.RequireValue(end) - DartRuntimePrimitives.RequireValue(begin))) * t))).round();
}

public class StepTween : Tween<long>
{
    public StepTween(long? begin = null, long? end = null) : base(begin: DartRuntimePrimitives.RequireValue(begin), end: DartRuntimePrimitives.RequireValue(end))
    {
    }

    public override long lerp(double t) => ((DartRuntimePrimitives.RequireValue(begin) + (((DartRuntimePrimitives.RequireValue(end) - DartRuntimePrimitives.RequireValue(begin))) * t))).floor();
}

public class ConstantTween<T> : Tween<T>
{
    public ConstantTween(T value) : base(begin: value, end: value)
    {
    }

    public override T lerp(double t) => ((T?)(object?)begin)!;
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ConstantTween"))}(value: {begin})";
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
        if (((t == 0.0) || (t == 1.0)))
        {
            DartRuntimePrimitives.Assert(() => (this.curve.transform(t).round() == t));
            return t;
        }
        return this.curve.transform(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "CurveTween"))}(curve: {this.curve})";
}

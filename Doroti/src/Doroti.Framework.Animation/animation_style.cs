// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animation_style.dart
using Doroti.Runtime;

namespace Doroti.Framework.Animation;

public class AnimationStyle : Diagnosticable
{
    public static AnimationStyle noAnimation = new AnimationStyle(duration: Duration.zero, reverseDuration: Duration.zero);
    public virtual Curve? curve { get; private set; }
    public virtual Duration? duration { get; private set; }
    public virtual Curve? reverseCurve { get; private set; }
    public virtual Duration? reverseDuration { get; private set; }

    public AnimationStyle(Curve? curve = null, Duration? duration = null, Curve? reverseCurve = null, Duration? reverseDuration = null)
    {
        this.curve = curve;
        this.duration = duration;
        this.reverseCurve = reverseCurve;
        this.reverseDuration = reverseDuration;
    }

    public virtual AnimationStyle copyWith(Curve? curve = null, Duration? duration = null, Curve? reverseCurve = null, Duration? reverseDuration = null)
    {
        return new AnimationStyle(curve: curve ?? this.curve, duration: duration ?? this.duration, reverseCurve: reverseCurve ?? this.reverseCurve, reverseDuration: reverseDuration ?? this.reverseDuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AnimationStyle merge(AnimationStyle? other)
    {
        if (other is null)
        {
            return this;
        }
        return copyWith(curve: other.curve, duration: other.duration, reverseCurve: other.reverseCurve, reverseDuration: other.reverseDuration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AnimationStyle? lerp(AnimationStyle? a, AnimationStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new AnimationStyle(curve: _lerp(a?.curve, b?.curve, t, (arg0, arg1, arg2) => new _LerpedCurve__animation_style(arg0, arg1, arg2)), duration: DartRuntimePrimitives.LerpNullable(a?.duration, b?.duration, t, _lerpDuration), reverseCurve: _lerp(a?.reverseCurve, b?.reverseCurve, t, (arg0, arg1, arg2) => new _LerpedCurve__animation_style(arg0, arg1, arg2)), reverseDuration: DartRuntimePrimitives.LerpNullable(a?.reverseDuration, b?.reverseDuration, t, _lerpDuration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static T? _lerp<T>(T? a, T? b, double t, Func<T?, T?, double, T> lerp)
    {
        if (Equals(a, b) || (t == 0.0))
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        return lerp(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Duration _lerpDuration(Duration? a, Duration? b, double t)
    {
        return Duration.Create(microseconds: (((a?.inMicroseconds ?? 0L) * (1.0 - t)) + ((b?.inMicroseconds ?? 0L) * t)).round());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AnimationStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is AnimationStyle) && Equals(__other.curve, curve) && Equals(__other.duration, duration) && Equals(__other.reverseCurve, reverseCurve) && Equals(__other.reverseDuration, reverseDuration);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(curve, duration, reverseCurve, reverseDuration);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Curve>("curve", curve, defaultValue: null));
        properties.add(new DiagnosticsProperty<Duration>("duration", duration, defaultValue: null));
        properties.add(new DiagnosticsProperty<Curve>("reverseCurve", reverseCurve, defaultValue: null));
        properties.add(new DiagnosticsProperty<Duration>("reverseDuration", reverseDuration, defaultValue: null));
    }

}

internal class _LerpedCurve__animation_style : Curve
{
    public virtual Curve first { get; private set; } = default!;
    public virtual Curve second { get; private set; } = default!;
    internal virtual double _t { get; private set; } = default!;

    internal _LerpedCurve__animation_style(Curve? a, Curve? b, double _t)
    {
        this._t = _t;
        first = a ?? Curves.linear;
        second = b ?? Curves.linear;
    }

    public override double transform(double t)
    {
        double a = first.transform(t);
        double b = second.transform(t);
        return (DartRuntimePrimitives.RequireValue(a) * (1.0 - _t)) + (DartRuntimePrimitives.RequireValue(b) * _t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _LerpedCurve__animation_style;
        if (__other is null) return false;
        return (__other is _LerpedCurve__animation_style) && Equals(__other.first, first) && Equals(__other.second, second) && (__other._t == _t);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(first, second, _t);
    public override string ToString() => $"_LerpedCurve({first}, {second}, t: {_t})";
}


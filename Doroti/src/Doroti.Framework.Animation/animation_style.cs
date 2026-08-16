// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animation_style.dart
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
        return new AnimationStyle(curve: (curve ?? this.curve), duration: (duration ?? this.duration), reverseCurve: (reverseCurve ?? this.reverseCurve), reverseDuration: (reverseDuration ?? this.reverseDuration));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AnimationStyle merge(AnimationStyle? other)
    {
        if ((other is null))
        {
            return this;
        }
        return copyWith(curve: ((AnimationStyle)other).curve, duration: ((AnimationStyle)other).duration, reverseCurve: ((AnimationStyle)other).reverseCurve, reverseDuration: ((AnimationStyle)other).reverseDuration);
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
        if (((object.Equals(a, b)) || (t == 0.0)))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        return lerp(a, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Duration _lerpDuration(Duration? a, Duration? b, double t)
    {
        return Duration.Create(microseconds: (((((a?.inMicroseconds ?? 0L)) * ((1.0 - t))) + (((b?.inMicroseconds ?? 0L)) * t))).round());
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is AnimationStyle) && (object.Equals(((AnimationStyle)((AnimationStyle)__other)).curve, this.curve))) && (object.Equals(((AnimationStyle)((AnimationStyle)__other)).duration, this.duration))) && (object.Equals(((AnimationStyle)((AnimationStyle)__other)).reverseCurve, this.reverseCurve))) && (object.Equals(((AnimationStyle)((AnimationStyle)__other)).reverseDuration, this.reverseDuration)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.curve, this.duration, this.reverseCurve, this.reverseDuration);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Curve>("curve", this.curve, defaultValue: null));
        properties.add(new DiagnosticsProperty<Duration>("duration", this.duration, defaultValue: null));
        properties.add(new DiagnosticsProperty<Curve>("reverseCurve", this.reverseCurve, defaultValue: null));
        properties.add(new DiagnosticsProperty<Duration>("reverseDuration", this.reverseDuration, defaultValue: null));
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
        this.first = (a ?? Curves.linear);
        this.second = (b ?? Curves.linear);
    }

    public override double transform(double t)
    {
        double a__4937 = this.first.transform(t);
        double b__4978 = this.second.transform(t);
        return ((DartRuntimePrimitives.RequireValue(a__4937) * ((1.0 - this._t))) + (DartRuntimePrimitives.RequireValue(b__4978) * this._t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _LerpedCurve__animation_style;
        if (__other is null) return false;
        return ((((__other is _LerpedCurve__animation_style) && (object.Equals(((_LerpedCurve__animation_style)((_LerpedCurve__animation_style)__other)).first, this.first))) && (object.Equals(((_LerpedCurve__animation_style)((_LerpedCurve__animation_style)__other)).second, this.second))) && (((_LerpedCurve__animation_style)((_LerpedCurve__animation_style)__other))._t == this._t));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.first, this.second, this._t);
    public override string ToString() => $"_LerpedCurve({this.first}, {this.second}, t: {this._t})";
}


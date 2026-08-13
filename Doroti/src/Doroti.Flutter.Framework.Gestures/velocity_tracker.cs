// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/velocity_tracker.dart
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

namespace Doroti.Generated.Framework.Gestures;

public class Velocity
{
    public static Velocity zero = new Velocity(pixelsPerSecond: Offset.zero);
    public virtual Offset pixelsPerSecond { get; private set; } = default!;

    public Velocity(Offset pixelsPerSecond)
    {
        this.pixelsPerSecond = pixelsPerSecond;
    }

    public virtual Velocity op_Subtract() => new Velocity(pixelsPerSecond: -this.pixelsPerSecond);
    public static Velocity operator -(Velocity value) => value.op_Subtract();
    public virtual Velocity op_Subtract(Velocity other)
    {
        return new Velocity(pixelsPerSecond: (this.pixelsPerSecond - ((Velocity)other).pixelsPerSecond));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity op_Add(Velocity other)
    {
        return new Velocity(pixelsPerSecond: (this.pixelsPerSecond + ((Velocity)other).pixelsPerSecond));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity clampMagnitude(double minValue, double maxValue)
    {
        DartRuntimePrimitives.Assert(() => (minValue >= 0.0));
        DartRuntimePrimitives.Assert(() => ((maxValue >= 0.0) && (maxValue >= minValue)));
        double valueSquared__1821 = this.pixelsPerSecond.distanceSquared;
        if ((valueSquared__1821 > (maxValue * maxValue)))
        {
            return new Velocity(pixelsPerSecond: (((this.pixelsPerSecond / this.pixelsPerSecond.distance)) * maxValue));
        }
        if ((valueSquared__1821 < (minValue * minValue)))
        {
            return new Velocity(pixelsPerSecond: (((this.pixelsPerSecond / this.pixelsPerSecond.distance)) * minValue));
        }
        return this;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as Velocity;
        if (__other is null) return false;
        return ((__other is Velocity) && (object.Equals(((Velocity)((Velocity)__other)).pixelsPerSecond, this.pixelsPerSecond)));
    }

    public override int GetHashCode() => this.pixelsPerSecond.GetHashCode();
    public override string ToString() => $"Velocity({this.pixelsPerSecond.dx.toStringAsFixed(1L)}, {this.pixelsPerSecond.dy.toStringAsFixed(1L)})";
}
public class VelocityEstimate
{
    public virtual Offset pixelsPerSecond { get; private set; } = default!;
    public virtual double confidence { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Offset offset { get; private set; } = default!;

    public VelocityEstimate(Offset pixelsPerSecond, double confidence, Duration duration, Offset offset)
    {
        this.pixelsPerSecond = pixelsPerSecond;
        this.confidence = confidence;
        this.duration = duration;
        this.offset = offset;
    }

    public override string ToString() => $"VelocityEstimate({this.pixelsPerSecond.dx.toStringAsFixed(1L)}, {this.pixelsPerSecond.dy.toStringAsFixed(1L)}; offset: {this.offset}, duration: {this.duration}, confidence: {this.confidence.toStringAsFixed(1L)})";
}

internal class _PointAtTime__velocity_tracker
{
    public virtual Duration time { get; private set; } = default!;
    public virtual Offset point { get; private set; } = default!;

    internal _PointAtTime__velocity_tracker(Offset point, Duration time)
    {
        this.point = point;
        this.time = time;
    }

    public override string ToString() => $"_PointAtTime({this.point} at {this.time})";
}

public class VelocityTracker
{
    internal const long _assumePointerMoveStoppedMilliseconds = 40L;
    internal const long _historySize = 20L;
    internal const long _horizonMilliseconds = 100L;
    internal const long _minSampleSize = 3L;
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    internal virtual Stopwatch? _stopwatch { get; set; } = default;
    internal virtual List<_PointAtTime__velocity_tracker?> _samples { get; private set; } = new List<_PointAtTime__velocity_tracker?>(System.Linq.Enumerable.Repeat<_PointAtTime__velocity_tracker?>(null, checked((int)_historySize)));
    internal virtual long _index { get; set; } = 0L;

    public VelocityTracker(PointerDeviceKind kind)
    {
        this.kind = kind;
    }

    internal virtual Stopwatch _sinceLastSample
    {
        get
        {
            _stopwatch ??= GestureBinding._instance?.samplingClock.stopwatch() ?? new Stopwatch();
            return this._stopwatch!;
            return default!;
        }
    }
    public virtual void addPosition(Duration time, Offset position)
    {
        this._sinceLastSample.Start();
        this._sinceLastSample.Reset();
        _index += 1L;
        if ((this._index == _historySize))
        {
            _index = 0L;
        }
        this._samples[(int)(this._index)] = new _PointAtTime__velocity_tracker(position, time);
    }

    public virtual VelocityEstimate? getVelocityEstimate()
    {
        if ((this._sinceLastSample.ElapsedMilliseconds > _assumePointerMoveStoppedMilliseconds))
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        var x__6616 = new List<double>();
        var y__6642 = new List<double>();
        var w__6668 = new List<double>();
        var time__6694 = new List<double>();
        var sampleCount__6721 = 0L;
        long index__6746 = this._index;
        _PointAtTime__velocity_tracker? newestSample__6787 = this._samples[(int)(index__6746)];
        if ((newestSample__6787 is null))
        {
            return null;
        }
        _PointAtTime__velocity_tracker previousSample__6894 = newestSample__6787;
        _PointAtTime__velocity_tracker oldestSample__6942 = newestSample__6787;
        do
        {
            _PointAtTime__velocity_tracker? sample__7136 = this._samples[(int)(index__6746)];
            if ((sample__7136 is null))
            {
                break;
            }
            double age__7233 = (((((_PointAtTime__velocity_tracker)newestSample__6787).time - ((_PointAtTime__velocity_tracker)sample__7136).time)).inMicroseconds.toDouble() / 1000L);
            double delta__7326 = (((((_PointAtTime__velocity_tracker)sample__7136).time - ((_PointAtTime__velocity_tracker)previousSample__6894).time)).inMicroseconds.abs().toDouble() / 1000L);
            previousSample__6894 = sample__7136;
            if (((age__7233 > _horizonMilliseconds) || (delta__7326 > _assumePointerMoveStoppedMilliseconds)))
            {
                break;
            }
            oldestSample__6942 = sample__7136;
            global::Doroti.Flutter.Ui.Offset position__7612 = ((_PointAtTime__velocity_tracker)sample__7136).point;
            x__6616.Add(position__7612.dx);
            y__6642.Add(position__7612.dy);
            w__6668.Add(1.0);
            time__6694.Add(-age__7233);
            index__6746 = ((((index__6746 == 0L) ? _historySize : index__6746)) - 1L);
            sampleCount__6721 += 1L;
        }
        while ((sampleCount__6721 < _historySize));
        if ((sampleCount__6721 >= _minSampleSize))
        {
            PolynomialFit? xFit__8006 = new LeastSquaresSolver(time__6694, x__6616, w__6668).solve(2L);
            PolynomialFit? yFit__8086 = new LeastSquaresSolver(time__6694, y__6642, w__6668).solve(2L);
            if (((xFit__8006 is not null) && (yFit__8086 is not null)))
            {
                return new VelocityEstimate(pixelsPerSecond: new global::Doroti.Flutter.Ui.Offset((((PolynomialFit)xFit__8006).coefficients[(int)(1L)] * 1000L), (((PolynomialFit)yFit__8086).coefficients[(int)(1L)] * 1000L)), confidence: (((PolynomialFit)xFit__8006).confidence * ((PolynomialFit)yFit__8086).confidence), duration: (((_PointAtTime__velocity_tracker)newestSample__6787).time - ((_PointAtTime__velocity_tracker)oldestSample__6942).time), offset: (((_PointAtTime__velocity_tracker)newestSample__6787).point - ((_PointAtTime__velocity_tracker)oldestSample__6942).point));
            }
        }
        return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: (((_PointAtTime__velocity_tracker)newestSample__6787).time - ((_PointAtTime__velocity_tracker)oldestSample__6942).time), offset: (((_PointAtTime__velocity_tracker)newestSample__6787).point - ((_PointAtTime__velocity_tracker)oldestSample__6942).point));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity getVelocity()
    {
        VelocityEstimate? estimate__9231 = getVelocityEstimate();
        if (((estimate__9231 is null) || (object.Equals(((VelocityEstimate)estimate__9231).pixelsPerSecond, Offset.zero))))
        {
            return Velocity.zero;
        }
        return new Velocity(pixelsPerSecond: ((VelocityEstimate)estimate__9231).pixelsPerSecond);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSScrollViewFlingVelocityTracker : VelocityTracker
{
    internal const long _sampleSize = 20L;
    internal virtual List<_PointAtTime__velocity_tracker?> _touchSamples { get; private set; } = new List<_PointAtTime__velocity_tracker?>(System.Linq.Enumerable.Repeat<_PointAtTime__velocity_tracker?>(null, checked((int)_sampleSize)));

    public IOSScrollViewFlingVelocityTracker(PointerDeviceKind kind) : base(kind)
    {
    }

    public override void addPosition(Duration time, Offset position)
    {
        _sinceLastSample.Start();
        _sinceLastSample.Reset();
        DartRuntimePrimitives.Assert(() =>
            {
                _PointAtTime__velocity_tracker? previousPoint__11370 = this._touchSamples[(int)(_index)];
                if (((previousPoint__11370 is null) || (((_PointAtTime__velocity_tracker)previousPoint__11370).time <= time)))
                {
                    return true;
                }
                throw new FlutterError($"The position being added ({position}) has a smaller timestamp ({time}) " + $"than its predecessor: {previousPoint__11370}.");
            });
        _index = (((_index + 1L)) % _sampleSize);
        this._touchSamples[(int)(_index)] = new _PointAtTime__velocity_tracker(position, time);
    }

    internal virtual global::Doroti.Flutter.Ui.Offset _previousVelocityAt(long index)
    {
        long endIndex__12072 = (((_index + index)) % _sampleSize);
        long startIndex__12129 = ((((_index + index) - 1L)) % _sampleSize);
        _PointAtTime__velocity_tracker? end__12202 = this._touchSamples[(int)(endIndex__12072)];
        _PointAtTime__velocity_tracker? start__12257 = this._touchSamples[(int)(startIndex__12129)];
        if (((end__12202 is null) || (start__12257 is null)))
        {
            return Offset.zero;
        }
        long dt__12380 = ((((_PointAtTime__velocity_tracker)end__12202).time - ((_PointAtTime__velocity_tracker)start__12257).time)).inMicroseconds;
        DartRuntimePrimitives.Assert(() => (dt__12380 >= 0L));
        return ((dt__12380 > 0L) ? ((((((_PointAtTime__velocity_tracker)end__12202).point - ((_PointAtTime__velocity_tracker)start__12257).point)) * 1000) / ((dt__12380.toDouble() / 1000L))) : Offset.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override VelocityEstimate? getVelocityEstimate()
    {
        if ((_sinceLastSample.ElapsedMilliseconds > VelocityTracker._assumePointerMoveStoppedMilliseconds))
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        global::Doroti.Flutter.Ui.Offset estimatedVelocity__13498 = (((_previousVelocityAt(-2L) * 0.6) + (_previousVelocityAt(-1L) * 0.35)) + (_previousVelocityAt(0L) * 0.05));
        _PointAtTime__velocity_tracker? newestSample__13663 = this._touchSamples[(int)(_index)];
        _PointAtTime__velocity_tracker? oldestNonNullSample__13719 = default!;
        for (var i__13754 = 1L; (i__13754 <= _sampleSize); i__13754 += 1L)
        {
            oldestNonNullSample__13719 = this._touchSamples[(int)((((_index + i__13754)) % _sampleSize))];
            if ((oldestNonNullSample__13719 is not null))
            {
                break;
            }
        }
        if (((oldestNonNullSample__13719 is null) || (newestSample__13663 is null)))
        {
            DartRuntimePrimitives.Assert(() => false);
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 0.0, duration: Duration.zero, offset: Offset.zero);
        }
        else
        {
            return new VelocityEstimate(pixelsPerSecond: estimatedVelocity__13498, confidence: 1.0, duration: (((_PointAtTime__velocity_tracker)newestSample__13663).time - ((_PointAtTime__velocity_tracker)oldestNonNullSample__13719).time), offset: (((_PointAtTime__velocity_tracker)newestSample__13663).point - ((_PointAtTime__velocity_tracker)oldestNonNullSample__13719).point));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MacOSScrollViewFlingVelocityTracker : IOSScrollViewFlingVelocityTracker
{
    public MacOSScrollViewFlingVelocityTracker(PointerDeviceKind kind) : base(kind)
    {
    }

    public override VelocityEstimate getVelocityEstimate()
    {
        if ((_sinceLastSample.ElapsedMilliseconds > VelocityTracker._assumePointerMoveStoppedMilliseconds))
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        global::Doroti.Flutter.Ui.Offset estimatedVelocity__16211 = (((_previousVelocityAt(-2L) * 0.15) + (_previousVelocityAt(-1L) * 0.65)) + (_previousVelocityAt(0L) * 0.2));
        _PointAtTime__velocity_tracker? newestSample__16376 = _touchSamples[(int)(_index)];
        _PointAtTime__velocity_tracker? oldestNonNullSample__16432 = default!;
        for (var i__16467 = 1L; (i__16467 <= IOSScrollViewFlingVelocityTracker._sampleSize); i__16467 += 1L)
        {
            oldestNonNullSample__16432 = _touchSamples[(int)((((_index + i__16467)) % IOSScrollViewFlingVelocityTracker._sampleSize))];
            if ((oldestNonNullSample__16432 is not null))
            {
                break;
            }
        }
        if (((oldestNonNullSample__16432 is null) || (newestSample__16376 is null)))
        {
            DartRuntimePrimitives.Assert(() => false);
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 0.0, duration: Duration.zero, offset: Offset.zero);
        }
        else
        {
            return new VelocityEstimate(pixelsPerSecond: estimatedVelocity__16211, confidence: 1.0, duration: (((_PointAtTime__velocity_tracker)newestSample__16376).time - ((_PointAtTime__velocity_tracker)oldestNonNullSample__16432).time), offset: (((_PointAtTime__velocity_tracker)newestSample__16376).point - ((_PointAtTime__velocity_tracker)oldestNonNullSample__16432).point));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

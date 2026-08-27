// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/velocity_tracker.dart
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

namespace Doroti.Framework.Gestures;

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
        double valueSquared = this.pixelsPerSecond.distanceSquared;
        if ((valueSquared > (maxValue * maxValue)))
        {
            return new Velocity(pixelsPerSecond: (((this.pixelsPerSecond / this.pixelsPerSecond.distance)) * maxValue));
        }
        if ((valueSquared < (minValue * minValue)))
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
        var x = new List<double>();
        var y = new List<double>();
        var w = new List<double>();
        var timeLocal = new List<double>();
        var sampleCount = 0L;
        long index = this._index;
        _PointAtTime__velocity_tracker? newestSample = this._samples[(int)(index)];
        if ((newestSample is null))
        {
            return null;
        }
        _PointAtTime__velocity_tracker previousSample = newestSample;
        _PointAtTime__velocity_tracker oldestSample = newestSample;
        do
        {
            _PointAtTime__velocity_tracker? sample = this._samples[(int)(index)];
            if ((sample is null))
            {
                break;
            }
            double age = (((((_PointAtTime__velocity_tracker)newestSample).time - ((_PointAtTime__velocity_tracker)sample).time)).inMicroseconds.toDouble() / 1000L);
            double delta = (((((_PointAtTime__velocity_tracker)sample).time - ((_PointAtTime__velocity_tracker)previousSample).time)).inMicroseconds.abs().toDouble() / 1000L);
            previousSample = sample;
            if (((age > _horizonMilliseconds) || (delta > _assumePointerMoveStoppedMilliseconds)))
            {
                break;
            }
            oldestSample = sample;
            global::Doroti.Ui.Offset position = ((_PointAtTime__velocity_tracker)sample).point;
            x.Add(position.dx);
            y.Add(position.dy);
            w.Add(1.0);
            timeLocal.Add(-age);
            index = ((((index == 0L) ? _historySize : index)) - 1L);
            sampleCount += 1L;
        }
        while ((sampleCount < _historySize));
        if ((sampleCount >= _minSampleSize))
        {
            PolynomialFit? xFit = new LeastSquaresSolver(timeLocal, x, w).solve(2L);
            PolynomialFit? yFit = new LeastSquaresSolver(timeLocal, y, w).solve(2L);
            if (((xFit is not null) && (yFit is not null)))
            {
                return new VelocityEstimate(pixelsPerSecond: new global::Doroti.Ui.Offset((((PolynomialFit)xFit).coefficients[(int)(1L)] * 1000L), (((PolynomialFit)yFit).coefficients[(int)(1L)] * 1000L)), confidence: (((PolynomialFit)xFit).confidence * ((PolynomialFit)yFit).confidence), duration: (((_PointAtTime__velocity_tracker)newestSample).time - ((_PointAtTime__velocity_tracker)oldestSample).time), offset: (((_PointAtTime__velocity_tracker)newestSample).point - ((_PointAtTime__velocity_tracker)oldestSample).point));
            }
        }
        return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: (((_PointAtTime__velocity_tracker)newestSample).time - ((_PointAtTime__velocity_tracker)oldestSample).time), offset: (((_PointAtTime__velocity_tracker)newestSample).point - ((_PointAtTime__velocity_tracker)oldestSample).point));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity getVelocity()
    {
        VelocityEstimate? estimate = getVelocityEstimate();
        if (((estimate is null) || (object.Equals(((VelocityEstimate)estimate).pixelsPerSecond, Offset.zero))))
        {
            return Velocity.zero;
        }
        return new Velocity(pixelsPerSecond: ((VelocityEstimate)estimate).pixelsPerSecond);
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
                _PointAtTime__velocity_tracker? previousPoint = this._touchSamples[(int)(_index)];
                if (((previousPoint is null) || (((_PointAtTime__velocity_tracker)previousPoint).time <= time)))
                {
                    return true;
                }
                throw new FlutterError($"The position being added ({position}) has a smaller timestamp ({time}) " + $"than its predecessor: {previousPoint}.");
            });
        _index = (((_index + 1L)) % _sampleSize);
        this._touchSamples[(int)(_index)] = new _PointAtTime__velocity_tracker(position, time);
    }

    internal virtual global::Doroti.Ui.Offset _previousVelocityAt(long index)
    {
        long endIndex = (((_index + index)) % _sampleSize);
        long startIndex = ((((_index + index) - 1L)) % _sampleSize);
        _PointAtTime__velocity_tracker? end = this._touchSamples[(int)(endIndex)];
        _PointAtTime__velocity_tracker? start = this._touchSamples[(int)(startIndex)];
        if (((end is null) || (start is null)))
        {
            return Offset.zero;
        }
        long dt = ((((_PointAtTime__velocity_tracker)end).time - ((_PointAtTime__velocity_tracker)start).time)).inMicroseconds;
        DartRuntimePrimitives.Assert(() => (dt >= 0L));
        return ((dt > 0L) ? ((((((_PointAtTime__velocity_tracker)end).point - ((_PointAtTime__velocity_tracker)start).point)) * 1000) / ((dt.toDouble() / 1000L))) : Offset.zero);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override VelocityEstimate? getVelocityEstimate()
    {
        if ((_sinceLastSample.ElapsedMilliseconds > VelocityTracker._assumePointerMoveStoppedMilliseconds))
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        global::Doroti.Ui.Offset estimatedVelocity = (((_previousVelocityAt(-2L) * 0.6) + (_previousVelocityAt(-1L) * 0.35)) + (_previousVelocityAt(0L) * 0.05));
        _PointAtTime__velocity_tracker? newestSample = this._touchSamples[(int)(_index)];
        _PointAtTime__velocity_tracker? oldestNonNullSample = default!;
        for (var i = 1L; (i <= _sampleSize); i += 1L)
        {
            oldestNonNullSample = this._touchSamples[(int)((((_index + i)) % _sampleSize))];
            if ((oldestNonNullSample is not null))
            {
                break;
            }
        }
        if (((oldestNonNullSample is null) || (newestSample is null)))
        {
            DartRuntimePrimitives.Assert(() => false);
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 0.0, duration: Duration.zero, offset: Offset.zero);
        }
        else
        {
            return new VelocityEstimate(pixelsPerSecond: estimatedVelocity, confidence: 1.0, duration: (((_PointAtTime__velocity_tracker)newestSample).time - ((_PointAtTime__velocity_tracker)oldestNonNullSample).time), offset: (((_PointAtTime__velocity_tracker)newestSample).point - ((_PointAtTime__velocity_tracker)oldestNonNullSample).point));
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
        global::Doroti.Ui.Offset estimatedVelocity = (((_previousVelocityAt(-2L) * 0.15) + (_previousVelocityAt(-1L) * 0.65)) + (_previousVelocityAt(0L) * 0.2));
        _PointAtTime__velocity_tracker? newestSample = _touchSamples[(int)(_index)];
        _PointAtTime__velocity_tracker? oldestNonNullSample = default!;
        for (var i = 1L; (i <= IOSScrollViewFlingVelocityTracker._sampleSize); i += 1L)
        {
            oldestNonNullSample = _touchSamples[(int)((((_index + i)) % IOSScrollViewFlingVelocityTracker._sampleSize))];
            if ((oldestNonNullSample is not null))
            {
                break;
            }
        }
        if (((oldestNonNullSample is null) || (newestSample is null)))
        {
            DartRuntimePrimitives.Assert(() => false);
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 0.0, duration: Duration.zero, offset: Offset.zero);
        }
        else
        {
            return new VelocityEstimate(pixelsPerSecond: estimatedVelocity, confidence: 1.0, duration: (((_PointAtTime__velocity_tracker)newestSample).time - ((_PointAtTime__velocity_tracker)oldestNonNullSample).time), offset: (((_PointAtTime__velocity_tracker)newestSample).point - ((_PointAtTime__velocity_tracker)oldestNonNullSample).point));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/velocity_tracker.dart
using System.Diagnostics;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public class Velocity
{
    public static Velocity zero = new Velocity(pixelsPerSecond: Offset.zero);
    public virtual Offset pixelsPerSecond { get; private set; } = default!;

    public Velocity(Offset pixelsPerSecond)
    {
        this.pixelsPerSecond = pixelsPerSecond;
    }

    public virtual Velocity op_Subtract() => new Velocity(pixelsPerSecond: -pixelsPerSecond);
    public static Velocity operator -(Velocity value) => value.op_Subtract();
    public virtual Velocity op_Subtract(Velocity other)
    {
        return new Velocity(pixelsPerSecond: pixelsPerSecond - other.pixelsPerSecond);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity op_Add(Velocity other)
    {
        return new Velocity(pixelsPerSecond: pixelsPerSecond + other.pixelsPerSecond);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity clampMagnitude(double minValue, double maxValue)
    {
        DartRuntimePrimitives.Assert(() => minValue >= 0.0);
        DartRuntimePrimitives.Assert(() => (maxValue >= 0.0) && (maxValue >= minValue));
        double valueSquared = pixelsPerSecond.distanceSquared;
        if (valueSquared > (maxValue * maxValue))
        {
            return new Velocity(pixelsPerSecond: pixelsPerSecond / pixelsPerSecond.distance * maxValue);
        }
        if (valueSquared < (minValue * minValue))
        {
            return new Velocity(pixelsPerSecond: pixelsPerSecond / pixelsPerSecond.distance * minValue);
        }
        return this;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as Velocity;
        if (__other is null) return false;
        return (__other is Velocity) && Equals(__other.pixelsPerSecond, pixelsPerSecond);
    }

    public override int GetHashCode() => pixelsPerSecond.GetHashCode();
    public override string ToString() => $"Velocity({pixelsPerSecond.dx.toStringAsFixed(1L)}, {pixelsPerSecond.dy.toStringAsFixed(1L)})";
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

    public override string ToString() => $"VelocityEstimate({pixelsPerSecond.dx.toStringAsFixed(1L)}, {pixelsPerSecond.dy.toStringAsFixed(1L)}; offset: {offset}, duration: {duration}, confidence: {confidence.toStringAsFixed(1L)})";
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

    public override string ToString() => $"_PointAtTime({point} at {time})";
}

public class VelocityTracker
{
    internal const long _assumePointerMoveStoppedMilliseconds = 40L;
    internal const long _historySize = 20L;
    internal const long _horizonMilliseconds = 100L;
    internal const long _minSampleSize = 3L;
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    internal virtual Stopwatch? _stopwatch { get; set; } = default;
    internal virtual List<_PointAtTime__velocity_tracker?> _samples { get; private set; } = new List<_PointAtTime__velocity_tracker?>(Enumerable.Repeat<_PointAtTime__velocity_tracker?>(null, checked((int)_historySize)));
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
            return _stopwatch!;
        }
    }
    public virtual void addPosition(Duration time, Offset position)
    {
        // Dart reset() preserves the running state; Stopwatch.Reset() stops it.
        _sinceLastSample.Restart();
        _index += 1L;
        if (_index == _historySize)
        {
            _index = 0L;
        }
        _samples[(int)_index] = new _PointAtTime__velocity_tracker(position, time);
    }

    public virtual VelocityEstimate? getVelocityEstimate()
    {
        if (_sinceLastSample.ElapsedMilliseconds > _assumePointerMoveStoppedMilliseconds)
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        var x = new List<double>();
        var y = new List<double>();
        var w = new List<double>();
        var timeLocal = new List<double>();
        var sampleCount = 0L;
        long index = _index;
        _PointAtTime__velocity_tracker? newestSample = _samples[(int)index];
        if (newestSample is null)
        {
            return null;
        }
        _PointAtTime__velocity_tracker previousSample = newestSample;
        _PointAtTime__velocity_tracker oldestSample = newestSample;
        do
        {
            _PointAtTime__velocity_tracker? sample = _samples[(int)index];
            if (sample is null)
            {
                break;
            }
            double age = (newestSample.time - sample.time).inMicroseconds.toDouble() / 1000L;
            double delta = (sample.time - previousSample.time).inMicroseconds.abs().toDouble() / 1000L;
            previousSample = sample;
            if ((age > _horizonMilliseconds) || (delta > _assumePointerMoveStoppedMilliseconds))
            {
                break;
            }
            oldestSample = sample;
            Offset position = sample.point;
            x.Add(position.dx);
            y.Add(position.dy);
            w.Add(1.0);
            timeLocal.Add(-age);
            index = ((index == 0L) ? _historySize : index) - 1L;
            sampleCount += 1L;
        }
        while (sampleCount < _historySize);
        if (sampleCount >= _minSampleSize)
        {
            PolynomialFit? xFit = new LeastSquaresSolver(timeLocal, x, w).solve(2L);
            PolynomialFit? yFit = new LeastSquaresSolver(timeLocal, y, w).solve(2L);
            if ((xFit is not null) && (yFit is not null))
            {
                return new VelocityEstimate(pixelsPerSecond: new Offset(xFit.coefficients[(int)1L] * 1000L, yFit.coefficients[(int)1L] * 1000L), confidence: xFit.confidence * yFit.confidence, duration: newestSample.time - oldestSample.time, offset: newestSample.point - oldestSample.point);
            }
        }
        return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: newestSample.time - oldestSample.time, offset: newestSample.point - oldestSample.point);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Velocity getVelocity()
    {
        VelocityEstimate? estimate = getVelocityEstimate();
        if ((estimate is null) || Equals(estimate.pixelsPerSecond, Offset.zero))
        {
            return Velocity.zero;
        }
        return new Velocity(pixelsPerSecond: estimate.pixelsPerSecond);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class IOSScrollViewFlingVelocityTracker : VelocityTracker
{
    internal const long _sampleSize = 20L;
    internal virtual List<_PointAtTime__velocity_tracker?> _touchSamples { get; private set; } = new List<_PointAtTime__velocity_tracker?>(Enumerable.Repeat<_PointAtTime__velocity_tracker?>(null, checked((int)_sampleSize)));

    public IOSScrollViewFlingVelocityTracker(PointerDeviceKind kind) : base(kind)
    {
    }

    public override void addPosition(Duration time, Offset position)
    {
        _sinceLastSample.Restart();
        DartRuntimePrimitives.Assert(() =>
            {
                _PointAtTime__velocity_tracker? previousPoint = _touchSamples[(int)_index];
                if ((previousPoint is null) || (previousPoint.time <= time))
                {
                    return true;
                }
                throw new FlutterError($"The position being added ({position}) has a smaller timestamp ({time}) " + $"than its predecessor: {previousPoint}.");
            });
        _index = (_index + 1L) % _sampleSize;
        _touchSamples[(int)_index] = new _PointAtTime__velocity_tracker(position, time);
    }

    internal virtual Offset _previousVelocityAt(long index)
    {
        // Dart % wraps negative offsets into the ring; C# % keeps their sign.
        long endIndex = (((_index + index) % _sampleSize) + _sampleSize) % _sampleSize;
        long startIndex = (((_index + index - 1L) % _sampleSize) + _sampleSize) % _sampleSize;
        _PointAtTime__velocity_tracker? end = _touchSamples[(int)endIndex];
        _PointAtTime__velocity_tracker? start = _touchSamples[(int)startIndex];
        if ((end is null) || (start is null))
        {
            return Offset.zero;
        }
        long dt = (end.time - start.time).inMicroseconds;
        DartRuntimePrimitives.Assert(() => dt >= 0L);
        return (dt > 0L) ? ((end.point - start.point) * 1000 / (dt.toDouble() / 1000L)) : Offset.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override VelocityEstimate? getVelocityEstimate()
    {
        if (_sinceLastSample.ElapsedMilliseconds > _assumePointerMoveStoppedMilliseconds)
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        Offset estimatedVelocity = (_previousVelocityAt(-2L) * 0.6) + (_previousVelocityAt(-1L) * 0.35) + (_previousVelocityAt(0L) * 0.05);
        _PointAtTime__velocity_tracker? newestSample = _touchSamples[(int)_index];
        _PointAtTime__velocity_tracker? oldestNonNullSample = default!;
        for (var i = 1L; i <= _sampleSize; i += 1L)
        {
            oldestNonNullSample = _touchSamples[(int)((_index + i) % _sampleSize)];
            if (oldestNonNullSample is not null)
            {
                break;
            }
        }
        if ((oldestNonNullSample is null) || (newestSample is null))
        {
            DartRuntimePrimitives.Assert(() => false);
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 0.0, duration: Duration.zero, offset: Offset.zero);
        }
        else
        {
            return new VelocityEstimate(pixelsPerSecond: estimatedVelocity, confidence: 1.0, duration: newestSample.time - oldestNonNullSample.time, offset: newestSample.point - oldestNonNullSample.point);
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
        if (_sinceLastSample.ElapsedMilliseconds > _assumePointerMoveStoppedMilliseconds)
        {
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 1.0, duration: Duration.zero, offset: Offset.zero);
        }
        Offset estimatedVelocity = (_previousVelocityAt(-2L) * 0.15) + (_previousVelocityAt(-1L) * 0.65) + (_previousVelocityAt(0L) * 0.2);
        _PointAtTime__velocity_tracker? newestSample = _touchSamples[(int)_index];
        _PointAtTime__velocity_tracker? oldestNonNullSample = default!;
        for (var i = 1L; i <= _sampleSize; i += 1L)
        {
            oldestNonNullSample = _touchSamples[(int)((_index + i) % _sampleSize)];
            if (oldestNonNullSample is not null)
            {
                break;
            }
        }
        if ((oldestNonNullSample is null) || (newestSample is null))
        {
            DartRuntimePrimitives.Assert(() => false);
            return new VelocityEstimate(pixelsPerSecond: Offset.zero, confidence: 0.0, duration: Duration.zero, offset: Offset.zero);
        }
        else
        {
            return new VelocityEstimate(pixelsPerSecond: estimatedVelocity, confidence: 1.0, duration: newestSample.time - oldestNonNullSample.time, offset: newestSample.point - oldestNonNullSample.point);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

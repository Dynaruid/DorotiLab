// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/curves.dart
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

public abstract class ParametricCurve<T>
{
    protected ParametricCurve()
    {
    }

    public virtual T transform(double t)
    {
        DartRuntimePrimitives.Assert(() => ((t >= 0.0) && (t <= 1.0)));
        return transformInternal(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T transformInternal(double t)
    {
        throw new NotImplementedException();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ParametricCurve");
}

public abstract class Curve : ParametricCurve<double>
{
    protected Curve()
    {
    }

    public override double transform(double t)
    {
        if (((t == 0.0) || (t == 1.0)))
        {
            return t;
        }
        return base.transform(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Curve flipped => new FlippedCurve(this);
}

internal class _Linear__curves : Curve
{
    internal _Linear__curves()
    {
    }

    public override double transformInternal(double t) => t;
}

public class SawTooth : Curve
{
    public virtual long count { get; private set; } = default!;

    public SawTooth(long count)
    {
        this.count = count;
    }

    public override double transformInternal(double t)
    {
        t *= this.count;
        return (t - t.truncateToDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SawTooth"))}({this.count})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Interval : Curve
{
    public virtual double begin { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;
    public virtual Curve curve { get; private set; } = default!;

    public Interval(double begin, double end, Curve curve = default!)
    {
        Curve __curve = curve ?? Curves.linear;
        this.begin = begin;
        this.end = end;
        this.curve = __curve;
    }

    public override double transformInternal(double t)
    {
        DartRuntimePrimitives.Assert(() => (this.begin >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.begin <= 1.0));
        DartRuntimePrimitives.Assert(() => (this.end >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.end <= 1.0));
        DartRuntimePrimitives.Assert(() => (this.end >= this.begin));
        t = Dart_uiLibrary.clampDouble((((t - this.begin)) / ((this.end - this.begin))), 0.0, 1.0);
        if (((t == 0.0) || (t == 1.0)))
        {
            return t;
        }
        return this.curve.transform(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        if ((this.curve is not _Linear__curves))
        {
            return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Interval"))}({this.begin}⋯{this.end})➩{this.curve}";
        }
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Interval"))}({this.begin}⋯{this.end})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Split : Curve
{
    public virtual double split { get; private set; } = default!;
    public virtual Curve beginCurve { get; private set; } = default!;
    public virtual Curve endCurve { get; private set; } = default!;

    public Split(double split, Curve beginCurve = default!, Curve endCurve = default!)
    {
        Curve __beginCurve = beginCurve ?? Curves.linear;
        Curve __endCurve = endCurve ?? Curves.easeOutCubic;
        this.split = split;
        this.beginCurve = __beginCurve;
        this.endCurve = __endCurve;
    }

    public override double transform(double t)
    {
        DartRuntimePrimitives.Assert(() => ((t >= 0.0) && (t <= 1.0)));
        DartRuntimePrimitives.Assert(() => ((this.split >= 0.0) && (this.split <= 1.0)));
        if (((t == 0.0) || (t == 1.0)))
        {
            return t;
        }
        if ((t == this.split))
        {
            return this.split;
        }
        if ((t < this.split))
        {
            double curveProgress = (t / this.split);
            double transformed = this.beginCurve.transform(curveProgress);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, this.split, transformed));
        }
        else
        {
            double curveProgressLocal = (((t - this.split)) / ((1L - this.split)));
            double transformedLocal = this.endCurve.transform(curveProgressLocal);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.split, 1L, transformedLocal));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({this.split}, {this.beginCurve}, {this.endCurve})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Threshold : Curve
{
    public virtual double threshold { get; private set; } = default!;

    public Threshold(double threshold)
    {
        this.threshold = threshold;
    }

    public override double transformInternal(double t)
    {
        DartRuntimePrimitives.Assert(() => (this.threshold >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.threshold <= 1.0));
        return ((t < this.threshold) ? 0.0 : 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Cubic : Curve
{
    public virtual double a { get; private set; } = default!;
    public virtual double b { get; private set; } = default!;
    public virtual double c { get; private set; } = default!;
    public virtual double d { get; private set; } = default!;
    internal const double _cubicErrorBound = 0.001;

    public Cubic(double a, double b, double c, double d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }

    internal virtual double _evaluateCubic(double a, double b, double m)
    {
        return ((((((3L * a) * ((1L - m))) * ((1L - m))) * m) + ((((3L * b) * ((1L - m))) * m) * m)) + ((m * m) * m));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double transformInternal(double t)
    {
        if (double.IsNaN(t))
        {
            throw new DartArgumentError(t, "t", "must not be NaN");
        }
        if ((t <= 0.0))
        {
            return 0.0;
        }
        if ((t >= 1.0))
        {
            return 1.0;
        }
        var start = 0.0;
        var end = 1.0;
        while (true)
        {
            double midpoint = (((start + end)) / 2L);
            double estimate = _evaluateCubic(this.a, this.c, midpoint);
            if ((((t - estimate)).abs() < _cubicErrorBound))
            {
                return _evaluateCubic(this.b, this.d, midpoint);
            }
            if ((estimate < t))
            {
                start = midpoint;
            }
            else
            {
                end = midpoint;
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Cubic"))}({this.a.toStringAsFixed(2L)}, {this.b.toStringAsFixed(2L)}, {this.c.toStringAsFixed(2L)}, {this.d.toStringAsFixed(2L)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ThreePointCubic : Curve
{
    public virtual Offset a1 { get; private set; } = default!;
    public virtual Offset b1 { get; private set; } = default!;
    public virtual Offset midpoint { get; private set; } = default!;
    public virtual Offset a2 { get; private set; } = default!;
    public virtual Offset b2 { get; private set; } = default!;

    public ThreePointCubic(Offset a1, Offset b1, Offset midpoint, Offset a2, Offset b2)
    {
        this.a1 = a1;
        this.b1 = b1;
        this.midpoint = midpoint;
        this.a2 = a2;
        this.b2 = b2;
    }

    public override double transformInternal(double t)
    {
        bool firstCurve = (t < this.midpoint.dx);
        double scaleX = (firstCurve ? this.midpoint.dx : (1.0 - this.midpoint.dx));
        double scaleY = (firstCurve ? this.midpoint.dy : (1.0 - this.midpoint.dy));
        double scaledT = (((t - ((firstCurve ? 0.0 : this.midpoint.dx)))) / scaleX);
        if (firstCurve)
        {
            return (new Cubic((this.a1.dx / scaleX), (this.a1.dy / scaleY), (this.b1.dx / scaleX), (this.b1.dy / scaleY)).transform(scaledT) * scaleY);
        }
        else
        {
            return ((new Cubic((((this.a2.dx - this.midpoint.dx)) / scaleX), (((this.a2.dy - this.midpoint.dy)) / scaleY), (((this.b2.dx - this.midpoint.dx)) / scaleX), (((this.b2.dy - this.midpoint.dy)) / scaleY)).transform(scaledT) * scaleY) + this.midpoint.dy);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, $"ThreePointCubic({this.a1}, {this.b1}, {this.midpoint}, {this.a2}, {this.b2})"))} ";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class Curve2D : ParametricCurve<Offset>
{
    protected Curve2D()
    {
    }

    public virtual IEnumerable<Curve2DSample> generateSamples(double start = 0.0, double end = 1.0, double tolerance = 1e-10)
    {
        DartRuntimePrimitives.Assert(() => (end > start));
        var rand = new DartRandom(this.samplingSeed);
        bool isFlat(Offset p, Offset q, Offset r)
        {
            global::Doroti.Ui.Offset pr = (p - r);
            global::Doroti.Ui.Offset qr = (q - r);
            double z = ((pr.dx * qr.dy) - (qr.dx * pr.dy));
            return (((z * z)) < tolerance);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var first = new Curve2DSample(start, transform(start));
        var last = new Curve2DSample(end, transform(end));
        var samples = new List<Curve2DSample> { first };
        void sample(Curve2DSample p, Curve2DSample q, bool forceSubdivide = false)
        {
            double tLocal = (((Curve2DSample)p).t + (((0.45 + (0.1 * rand.nextDouble()))) * ((((Curve2DSample)q).t - ((Curve2DSample)p).t))));
            var rLocal = new Curve2DSample(tLocal, transform(tLocal));
            if ((!forceSubdivide && isFlat(((Curve2DSample)p).value, ((Curve2DSample)q).value, ((Curve2DSample)rLocal).value)))
            {
                samples.Add(q);
            }
            else
            {
                sample(p, rLocal);
                sample(rLocal, q);
            }
        }
        sample(first, last, forceSubdivide: ((((((Curve2DSample)first).value.dx - ((Curve2DSample)last).value.dx)).abs() < tolerance) && (((((Curve2DSample)first).value.dy - ((Curve2DSample)last).value.dy)).abs() < tolerance)));
        return samples;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long samplingSeed => 0L;
    public virtual double findInverse(double x)
    {
        var start = 0.0;
        var end = 1.0;
        double mid = default!;
        double offsetToOrigin(double pos)
        {
            return (x - transform(pos).dx);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var errorLimit = 0.000001;
        var count = 100L;
        double startValue = offsetToOrigin(start);
        while ((((((end - start)) / 2.0) > errorLimit) && (count > 0L)))
        {
            mid = (((end + start)) / 2.0);
            double value = offsetToOrigin(mid);
            if ((Math.Sign(value) == Math.Sign(startValue)))
            {
                start = mid;
            }
            else
            {
                end = mid;
            }
            count--;
        }
        return mid;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class Curve2DSample
{
    public virtual double t { get; private set; } = default!;
    public virtual Offset value { get; private set; } = default!;

    public Curve2DSample(double t, Offset value)
    {
        this.t = t;
        this.value = value;
    }

    public override string ToString()
    {
        return $"[({this.value.dx.toStringAsFixed(2L)}, {this.value.dy.toStringAsFixed(2L)}), {this.t.toStringAsFixed(2L)}]";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CatmullRomSpline : Curve2D
{
    internal virtual List<List<Offset>> _cubicSegments { get; private set; } = default!;
    internal virtual List<Offset>? _controlPoints { get; private set; }
    internal virtual Offset? _startHandle { get; private set; }
    internal virtual Offset? _endHandle { get; private set; }
    internal virtual double? _tension { get; private set; }

    public CatmullRomSpline(List<Offset> controlPoints, double tension = 0.0, Offset? startHandle = null, Offset? endHandle = null)
    {
        this._controlPoints = controlPoints;
        this._startHandle = startHandle;
        this._endHandle = endHandle;
        this._tension = tension;
        this._cubicSegments = new List<List<global::Doroti.Ui.Offset>>();
        System.Diagnostics.Debug.Assert((tension <= 1.0));
        System.Diagnostics.Debug.Assert((tension >= 0.0));
        System.Diagnostics.Debug.Assert((checked((long)(controlPoints.Count)) > 3L));
    }

    public static CatmullRomSpline CreatePrecompute(List<Offset> controlPoints, double tension = 0.0, Offset? startHandle = null, Offset? endHandle = null)
    {
        var __instance = new CatmullRomSpline(controlPoints, tension, startHandle, endHandle);
        __instance._controlPoints = null;
        __instance._startHandle = null;
        __instance._endHandle = null;
        __instance._tension = null;
        __instance._cubicSegments = _computeSegments(controlPoints, tension, startHandle: startHandle, endHandle: endHandle);
        return __instance;
    }

    internal static List<List<global::Doroti.Ui.Offset>> _computeSegments(List<Offset> controlPoints, double tension, Offset? startHandle = null, Offset? endHandle = null)
    {
        DartRuntimePrimitives.Assert(() => ((startHandle is null) || DartRuntimePrimitives.RequireValue(startHandle).isFinite));
        DartRuntimePrimitives.Assert(() => ((endHandle is null) || DartRuntimePrimitives.RequireValue(endHandle).isFinite));
        DartRuntimePrimitives.Assert(() =>
            {
                for (var index = 0L; (index < checked((long)(controlPoints.Count))); index++)
                {
                    if (!controlPoints[(int)(index)].isFinite)
                    {
                        throw new FlutterError($"The provided CatmullRomSpline control point at index {index} is not " + $"finite. The control point given was {controlPoints[(int)(index)]}.");
                    }
                }
                return true;
            });
        startHandle ??= ((controlPoints[(int)(0L)] * 2.0) - controlPoints[(int)(1L)]);
        endHandle ??= ((controlPoints.Last() * 2.0) - controlPoints[(int)((checked((long)(controlPoints.Count)) - 2L))]);
        var allPoints = new List<global::Doroti.Ui.Offset> { DartRuntimePrimitives.RequireValue(startHandle), DartRuntimePrimitives.RequireValue(endHandle) };
        var alpha = 0.5;
        double reverseTension = (1.0 - tension);
        var result = new List<List<global::Doroti.Ui.Offset>>();
        for (var i = 0L; (i < (checked((long)(allPoints.Count)) - 3L)); ++i)
        {
            var curve = new List<global::Doroti.Ui.Offset> { allPoints[(int)(i)], allPoints[(int)((i + 1L))], allPoints[(int)((i + 2L))], allPoints[(int)((i + 3L))] };
            global::Doroti.Ui.Offset diffCurve10 = (curve[(int)(1L)] - curve[(int)(0L)]);
            global::Doroti.Ui.Offset diffCurve21 = (curve[(int)(2L)] - curve[(int)(1L)]);
            global::Doroti.Ui.Offset diffCurve32 = (curve[(int)(3L)] - curve[(int)(2L)]);
            double t01 = global::Doroti.Runtime.Dart_mathLibrary.pow(diffCurve10.distance, alpha).toDouble();
            double t12 = global::Doroti.Runtime.Dart_mathLibrary.pow(diffCurve21.distance, alpha).toDouble();
            double t23 = global::Doroti.Runtime.Dart_mathLibrary.pow(diffCurve32.distance, alpha).toDouble();
            global::Doroti.Ui.Offset m1 = (((diffCurve21 + ((((diffCurve10 / t01) - (((curve[(int)(2L)] - curve[(int)(0L)])) / ((t01 + t12))))) * t12))) * reverseTension);
            global::Doroti.Ui.Offset m2 = (((diffCurve21 + ((((diffCurve32 / t23) - (((curve[(int)(3L)] - curve[(int)(1L)])) / ((t12 + t23))))) * t12))) * reverseTension);
            global::Doroti.Ui.Offset sumM12 = (m1 + m2);
            var segment = new List<global::Doroti.Ui.Offset> { ((diffCurve21 * -2.0) + sumM12), (((diffCurve21 * 3.0) - m1) - sumM12), m1, curve[(int)(1L)] };
            result.Add(segment);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _initializeIfNeeded()
    {
        if ((checked((long)(this._cubicSegments.Count)) != 0))
        {
            return;
        }
        this._cubicSegments.AddRange(_computeSegments(this._controlPoints!, DartRuntimePrimitives.RequireValue(this._tension), startHandle: this._startHandle, endHandle: this._endHandle));
    }

    public override long samplingSeed
    {
        get
        {
            _initializeIfNeeded();
            global::Doroti.Ui.Offset seedPoint = this._cubicSegments[(int)(0L)][(int)(1L)];
            return ((((seedPoint.dx + seedPoint.dy)) * 10000L)).round();
            return default!;
        }
    }
    public override global::Doroti.Ui.Offset transformInternal(double t)
    {
        _initializeIfNeeded();
        double length = checked((long)(this._cubicSegments.Count)).toDouble();
        double position = default!;
        double localT = default!;
        long index = default!;
        if ((t < 1.0))
        {
            position = (t * length);
            localT = (position % 1.0);
            index = position.floor();
        }
        else
        {
            position = length;
            localT = 1.0;
            index = (checked((long)(this._cubicSegments.Count)) - 1L);
        }
        List<global::Doroti.Ui.Offset> cubicControlPoints = this._cubicSegments[(int)(index)];
        double localT2 = (localT * localT);
        return (((((cubicControlPoints[(int)(0L)] * localT2) * localT) + (cubicControlPoints[(int)(1L)] * localT2)) + (cubicControlPoints[(int)(2L)] * localT)) + cubicControlPoints[(int)(3L)]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CatmullRomCurve : Curve
{
    internal static List<string> _debugAssertReasons = new List<string>();
    internal virtual List<Curve2DSample> _precomputedSamples { get; private set; } = default!;
    public virtual List<Offset> controlPoints { get; private set; } = default!;
    public virtual double tension { get; private set; } = default!;

    public CatmullRomCurve(List<Offset> controlPoints, double tension = 0.0)
    {
        this.controlPoints = controlPoints;
        this.tension = tension;
        this._precomputedSamples = new List<Curve2DSample>();
        System.Diagnostics.Debug.Assert(((Func<bool>)(() =>
        {
            return validateControlPoints(controlPoints, tension: tension, reasons: ((Func<List<string>>)(() =>
            {
                var __cascade = _debugAssertReasons;
                __cascade.Clear();
                return __cascade;
            }))());
            return default;
        }))());
    }

    public static CatmullRomCurve CreatePrecompute(List<Offset> controlPoints, double tension = 0.0)
    {
        var __instance = new CatmullRomCurve(controlPoints, tension);
        __instance.controlPoints = controlPoints;
        __instance.tension = tension;
        __instance._precomputedSamples = _computeSamples(controlPoints, tension);
        return __instance;
    }

    internal static List<Curve2DSample> _computeSamples(List<Offset> controlPoints, double tension)
    {
        return CatmullRomSpline.CreatePrecompute(new List<global::Doroti.Ui.Offset> { Offset.zero, new global::Doroti.Ui.Offset(1.0, 1.0) }, tension: tension).generateSamples(tolerance: 1e-12).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool validateControlPoints(List<Offset>? controlPoints, double tension = 0.0, List<string>? reasons = null)
    {
        if ((controlPoints is null))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    reasons?.Add("Supplied control points cannot be null");
                    return true;
                });
            return false;
        }
        if ((checked((long)(controlPoints.Count)) < 2L))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    reasons?.Add("There must be at least two points supplied to create a valid curve.");
                    return true;
                });
            return false;
        }
        controlPoints = new List<global::Doroti.Ui.Offset> { Offset.zero, new global::Doroti.Ui.Offset(1.0, 1.0) };
        global::Doroti.Ui.Offset startHandle = ((controlPoints[(int)(0L)] * 2.0) - controlPoints[(int)(1L)]);
        global::Doroti.Ui.Offset endHandle = ((controlPoints.Last() * 2.0) - controlPoints[(int)((checked((long)(controlPoints.Count)) - 2L))]);
        controlPoints = new List<global::Doroti.Ui.Offset> { startHandle, endHandle };
        double lastX = -double.PositiveInfinity;
        for (var i = 0L; (i < checked((long)(controlPoints.Count))); ++i)
        {
            if ((((i > 1L) && (i < (checked((long)(controlPoints.Count)) - 2L))) && (((controlPoints[(int)(i)].dx <= 0.0) || (controlPoints[(int)(i)].dx >= 1.0)))))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add("Control points must have X values between 0.0 and 1.0, exclusive. " + $"Point {i} has an x value ({controlPoints![(int)(i)].dx}) which is outside the range.");
                        return true;
                    });
                return false;
            }
            if ((controlPoints[(int)(i)].dx <= lastX))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add("Each X coordinate must be greater than the preceding X coordinate " + $"(i.e. must be monotonically increasing in X). Point {i} has an x value of " + $"{controlPoints![(int)(i)].dx}, which is not greater than {lastX}");
                        return true;
                    });
                return false;
            }
            lastX = controlPoints[(int)(i)].dx;
        }
        var success = true;
        lastX = -double.PositiveInfinity;
        var tolerance = 0.001;
        var testSpline = new CatmullRomSpline(controlPoints, tension: tension);
        double startLocal = testSpline.findInverse(0.0);
        double endLocal = testSpline.findInverse(1.0);
        IEnumerable<Curve2DSample> samplePoints = testSpline.generateSamples(start: startLocal, end: endLocal);
        if (((samplePoints.First().value.dy.abs() > tolerance) || (((1.0 - samplePoints.Last().value.dy)).abs() > tolerance)))
        {
            var bail = true;
            success = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    reasons?.Add($"The curve has more than one Y value at X = {samplePoints.First().value.dx}. " + "Try moving some control points further away from this value of X, or increasing " + "the tension.");
                    bail = (reasons is null);
                    return true;
                });
            if (bail)
            {
                return false;
            }
        }
        foreach (var sample in samplePoints)
        {
            global::Doroti.Ui.Offset point = ((Curve2DSample)sample).value;
            double tLocal = ((Curve2DSample)sample).t;
            double x = point.dx;
            if ((((tLocal >= startLocal) && (tLocal <= endLocal)) && (((x < -0.001) || (x > (1.0 + 0.001))))))
            {
                var bailLocal = true;
                success = false;
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add($"The resulting curve has an X value ({x}) which is outside " + "the range [0.0, 1.0], inclusive.");
                        bailLocal = (reasons is null);
                        return true;
                    });
                if (bailLocal)
                {
                    return false;
                }
            }
            if ((x < lastX))
            {
                var bailAlternate = true;
                success = false;
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add($"The curve has more than one Y value at x = {x}. Try moving " + "some control points further apart in X, or increasing the tension.");
                        bailAlternate = (reasons is null);
                        return true;
                    });
                if (bailAlternate)
                {
                    return false;
                }
            }
            lastX = x;
        }
        return success;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double transformInternal(double t)
    {
        if ((checked((long)(this._precomputedSamples.Count)) == 0))
        {
            this._precomputedSamples.AddRange(_computeSamples(this.controlPoints, this.tension));
        }
        var start = 0L;
        long end = (checked((long)(this._precomputedSamples.Count)) - 1L);
        long mid = default!;
        global::Doroti.Ui.Offset valueLocal = default!;
        global::Doroti.Ui.Offset startValue = this._precomputedSamples[(int)(start)].value;
        global::Doroti.Ui.Offset endValue = this._precomputedSamples[(int)(end)].value;
        while (((end - start) > 1L))
        {
            mid = (checked((long)(((end + start)) / 2L)));
            valueLocal = this._precomputedSamples[(int)(mid)].value;
            if ((t >= valueLocal.dx))
            {
                start = mid;
                startValue = valueLocal;
            }
            else
            {
                end = mid;
                endValue = valueLocal;
            }
        }
        double t2 = (((t - startValue.dx)) / ((endValue.dx - startValue.dx)));
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(startValue.dy, endValue.dy, t2));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FlippedCurve : Curve
{
    public virtual Curve curve { get; private set; } = default!;

    public FlippedCurve(Curve curve)
    {
        this.curve = curve;
    }

    public override double transformInternal(double t) => (1.0 - this.curve.transform((1.0 - t)));
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FlippedCurve"))}({this.curve})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DecelerateCurve__curves : Curve
{
    internal _DecelerateCurve__curves()
    {
    }

    public override double transformInternal(double t)
    {
        t = (1.0 - t);
        return (1.0 - (t * t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class CurvesLibrary
{
    internal static double _bounce(double t)
    {
        if ((t < (1.0 / 2.75)))
        {
            return ((7.5625 * t) * t);
        }
        else
        {
            if ((t < (2L / 2.75)))
            {
                t -= (1.5 / 2.75);
                return (((7.5625 * t) * t) + 0.75);
            }
            else
            {
                if ((t < (2.5 / 2.75)))
                {
                    t -= (2.25 / 2.75);
                    return (((7.5625 * t) * t) + 0.9375);
                }
            }
        }
        t -= (2.625 / 2.75);
        return (((7.5625 * t) * t) + 0.984375);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _BounceInCurve__curves : Curve
{
    internal _BounceInCurve__curves()
    {
    }

    public override double transformInternal(double t)
    {
        return (1.0 - CurvesLibrary._bounce((1.0 - t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BounceOutCurve__curves : Curve
{
    internal _BounceOutCurve__curves()
    {
    }

    public override double transformInternal(double t)
    {
        return CurvesLibrary._bounce(t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BounceInOutCurve__curves : Curve
{
    internal _BounceInOutCurve__curves()
    {
    }

    public override double transformInternal(double t)
    {
        if ((t < 0.5))
        {
            return (((1.0 - CurvesLibrary._bounce((1.0 - (t * 2.0))))) * 0.5);
        }
        else
        {
            return ((CurvesLibrary._bounce(((t * 2.0) - 1.0)) * 0.5) + 0.5);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ElasticInCurve : Curve
{
    public virtual double period { get; private set; } = default!;

    public ElasticInCurve(double period = 0.4)
    {
        this.period = period;
    }

    public override double transformInternal(double t)
    {
        double s = (this.period / 4.0);
        t = (t - 1.0);
        return (-global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (10.0 * t)) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s)) * ((Dart_mathLibrary.pi * 2.0))) / this.period)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ElasticInCurve"))}({this.period})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ElasticOutCurve : Curve
{
    public virtual double period { get; private set; } = default!;

    public ElasticOutCurve(double period = 0.4)
    {
        this.period = period;
    }

    public override double transformInternal(double t)
    {
        double s = (this.period / 4.0);
        return ((global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (-10L * t)) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s)) * ((Dart_mathLibrary.pi * 2.0))) / this.period))) + 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ElasticOutCurve"))}({this.period})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ElasticInOutCurve : Curve
{
    public virtual double period { get; private set; } = default!;

    public ElasticInOutCurve(double period = 0.4)
    {
        this.period = period;
    }

    public override double transformInternal(double t)
    {
        double s = (this.period / 4.0);
        t = ((2.0 * t) - 1.0);
        if ((t < 0.0))
        {
            return ((-0.5 * global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (10.0 * t))) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s)) * ((Dart_mathLibrary.pi * 2.0))) / this.period)));
        }
        else
        {
            return (((global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (-10.0 * t)) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s)) * ((Dart_mathLibrary.pi * 2.0))) / this.period))) * 0.5) + 1.0);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ElasticInOutCurve"))}({this.period})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class Curves
{
    public static Curve linear = new _Linear__curves();
    public static Curve decelerate = new _DecelerateCurve__curves();
    public static Cubic fastLinearToSlowEaseIn = new Cubic(0.18, 1.0, 0.04, 1.0);
    public static ThreePointCubic fastEaseInToSlowEaseOut = new ThreePointCubic(new global::Doroti.Ui.Offset(0.056, 0.024), new global::Doroti.Ui.Offset(0.108, 0.3085), new global::Doroti.Ui.Offset(0.198, 0.541), new global::Doroti.Ui.Offset(0.3655, 1.0), new global::Doroti.Ui.Offset(0.5465, 0.989));
    public static Cubic ease = new Cubic(0.25, 0.1, 0.25, 1.0);
    public static Cubic easeIn = new Cubic(0.42, 0.0, 1.0, 1.0);
    public static Cubic easeInToLinear = new Cubic(0.67, 0.03, 0.65, 0.09);
    public static Cubic easeInSine = new Cubic(0.47, 0.0, 0.745, 0.715);
    public static Cubic easeInQuad = new Cubic(0.55, 0.085, 0.68, 0.53);
    public static Cubic easeInCubic = new Cubic(0.55, 0.055, 0.675, 0.19);
    public static Cubic easeInQuart = new Cubic(0.895, 0.03, 0.685, 0.22);
    public static Cubic easeInQuint = new Cubic(0.755, 0.05, 0.855, 0.06);
    public static Cubic easeInExpo = new Cubic(0.95, 0.05, 0.795, 0.035);
    public static Cubic easeInCirc = new Cubic(0.6, 0.04, 0.98, 0.335);
    public static Cubic easeInBack = new Cubic(0.6, -0.28, 0.735, 0.045);
    public static Cubic easeOut = new Cubic(0.0, 0.0, 0.58, 1.0);
    public static Cubic linearToEaseOut = new Cubic(0.35, 0.91, 0.33, 0.97);
    public static Cubic easeOutSine = new Cubic(0.39, 0.575, 0.565, 1.0);
    public static Cubic easeOutQuad = new Cubic(0.25, 0.46, 0.45, 0.94);
    public static Cubic easeOutCubic = new Cubic(0.215, 0.61, 0.355, 1.0);
    public static Cubic easeOutQuart = new Cubic(0.165, 0.84, 0.44, 1.0);
    public static Cubic easeOutQuint = new Cubic(0.23, 1.0, 0.32, 1.0);
    public static Cubic easeOutExpo = new Cubic(0.19, 1.0, 0.22, 1.0);
    public static Cubic easeOutCirc = new Cubic(0.075, 0.82, 0.165, 1.0);
    public static Cubic easeOutBack = new Cubic(0.175, 0.885, 0.32, 1.275);
    public static Cubic easeInOut = new Cubic(0.42, 0.0, 0.58, 1.0);
    public static Cubic easeInOutSine = new Cubic(0.445, 0.05, 0.55, 0.95);
    public static Cubic easeInOutQuad = new Cubic(0.455, 0.03, 0.515, 0.955);
    public static Cubic easeInOutCubic = new Cubic(0.645, 0.045, 0.355, 1.0);
    public static ThreePointCubic easeInOutCubicEmphasized = new ThreePointCubic(new global::Doroti.Ui.Offset(0.05, 0), new global::Doroti.Ui.Offset(0.133333, 0.06), new global::Doroti.Ui.Offset(0.166666, 0.4), new global::Doroti.Ui.Offset(0.208333, 0.82), new global::Doroti.Ui.Offset(0.25, 1));
    public static Cubic easeInOutQuart = new Cubic(0.77, 0.0, 0.175, 1.0);
    public static Cubic easeInOutQuint = new Cubic(0.86, 0.0, 0.07, 1.0);
    public static Cubic easeInOutExpo = new Cubic(1.0, 0.0, 0.0, 1.0);
    public static Cubic easeInOutCirc = new Cubic(0.785, 0.135, 0.15, 0.86);
    public static Cubic easeInOutBack = new Cubic(0.68, -0.55, 0.265, 1.55);
    public static Cubic fastOutSlowIn = new Cubic(0.4, 0.0, 0.2, 1.0);
    public static Cubic slowMiddle = new Cubic(0.15, 0.85, 0.85, 0.15);
    public static Curve bounceIn = new _BounceInCurve__curves();
    public static Curve bounceOut = new _BounceOutCurve__curves();
    public static Curve bounceInOut = new _BounceInOutCurve__curves();
    public static ElasticInCurve elasticIn = new ElasticInCurve();
    public static ElasticOutCurve elasticOut = new ElasticOutCurve();
    public static ElasticInOutCurve elasticInOut = new ElasticInOutCurve();

}

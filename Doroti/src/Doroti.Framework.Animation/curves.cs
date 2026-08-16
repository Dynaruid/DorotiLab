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
            double curveProgress__8547 = (t / this.split);
            double transformed__8593 = this.beginCurve.transform(curveProgress__8547);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, this.split, transformed__8593));
        }
        else
        {
            double curveProgress__8725 = (((t - this.split)) / ((1L - this.split)));
            double transformed__8787 = this.endCurve.transform(curveProgress__8725);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.split, 1L, transformed__8787));
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
        var start__16076 = 0.0;
        var end__16097 = 1.0;
        while (true)
        {
            double midpoint__16146 = (((start__16076 + end__16097)) / 2L);
            double estimate__16195 = _evaluateCubic(this.a, this.c, midpoint__16146);
            if ((((t - estimate__16195)).abs() < _cubicErrorBound))
            {
                return _evaluateCubic(this.b, this.d, midpoint__16146);
            }
            if ((estimate__16195 < t))
            {
                start__16076 = midpoint__16146;
            }
            else
            {
                end__16097 = midpoint__16146;
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
        bool firstCurve__19230 = (t < this.midpoint.dx);
        double scaleX__19277 = (firstCurve__19230 ? this.midpoint.dx : (1.0 - this.midpoint.dx));
        double scaleY__19349 = (firstCurve__19230 ? this.midpoint.dy : (1.0 - this.midpoint.dy));
        double scaledT__19421 = (((t - ((firstCurve__19230 ? 0.0 : this.midpoint.dx)))) / scaleX__19277);
        if (firstCurve__19230)
        {
            return (new Cubic((this.a1.dx / scaleX__19277), (this.a1.dy / scaleY__19349), (this.b1.dx / scaleX__19277), (this.b1.dy / scaleY__19349)).transform(scaledT__19421) * scaleY__19349);
        }
        else
        {
            return ((new Cubic((((this.a2.dx - this.midpoint.dx)) / scaleX__19277), (((this.a2.dy - this.midpoint.dy)) / scaleY__19349), (((this.b2.dx - this.midpoint.dx)) / scaleX__19277), (((this.b2.dy - this.midpoint.dy)) / scaleY__19349)).transform(scaledT__19421) * scaleY__19349) + this.midpoint.dy);
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
        var rand__23209 = new DartRandom(this.samplingSeed);
        bool isFlat(Offset p, Offset q, Offset r)
        {
            global::Doroti.Ui.Offset pr__23382 = (p - r);
            global::Doroti.Ui.Offset qr__23413 = (q - r);
            double z__23444 = ((pr__23382.dx * qr__23413.dy) - (qr__23413.dx * pr__23382.dy));
            return (((z__23444 * z__23444)) < tolerance);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var first__23530 = new Curve2DSample(start, transform(start));
        var last__23588 = new Curve2DSample(end, transform(end));
        var samples__23641 = new List<Curve2DSample> { first__23530 };
        void sample(Curve2DSample p, Curve2DSample q, bool forceSubdivide = false)
        {
            double t__23894 = (((Curve2DSample)p).t + (((0.45 + (0.1 * rand__23209.nextDouble()))) * ((((Curve2DSample)q).t - ((Curve2DSample)p).t))));
            var r__23964 = new Curve2DSample(t__23894, transform(t__23894));
            if ((!forceSubdivide && isFlat(((Curve2DSample)p).value, ((Curve2DSample)q).value, ((Curve2DSample)r__23964).value)))
            {
                samples__23641.Add(q);
            }
            else
            {
                sample(p, r__23964);
                sample(r__23964, q);
            }
        }
        sample(first__23530, last__23588, forceSubdivide: ((((((Curve2DSample)first__23530).value.dx - ((Curve2DSample)last__23588).value.dx)).abs() < tolerance) && (((((Curve2DSample)first__23530).value.dy - ((Curve2DSample)last__23588).value.dy)).abs() < tolerance)));
        return samples__23641;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long samplingSeed => 0L;
    public virtual double findInverse(double x)
    {
        var start__25355 = 0.0;
        var end__25376 = 1.0;
        double mid__25403 = default!;
        double offsetToOrigin(double pos)
        {
            return (x - transform(pos).dx);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var errorLimit__25599 = 0.000001;
        var count__25626 = 100L;
        double startValue__25656 = offsetToOrigin(start__25355);
        while ((((((end__25376 - start__25355)) / 2.0) > errorLimit__25599) && (count__25626 > 0L)))
        {
            mid__25403 = (((end__25376 + start__25355)) / 2.0);
            double value__25804 = offsetToOrigin(mid__25403);
            if ((Math.Sign(value__25804) == Math.Sign(startValue__25656)))
            {
                start__25355 = mid__25403;
            }
            else
            {
                end__25376 = mid__25403;
            }
            count__25626--;
        }
        return mid__25403;
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
        var __instance = new CatmullRomSpline(default!, default!, default!, default!);
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
                for (var index__31649 = 0L; (index__31649 < checked((long)(controlPoints.Count))); index__31649++)
                {
                    if (!controlPoints[(int)(index__31649)].isFinite)
                    {
                        throw new FlutterError($"The provided CatmullRomSpline control point at index {index__31649} is not " + $"finite. The control point given was {controlPoints[(int)(index__31649)]}.");
                    }
                }
                return true;
            });
        startHandle ??= ((controlPoints[(int)(0L)] * 2.0) - controlPoints[(int)(1L)]);
        endHandle ??= ((controlPoints.Last() * 2.0) - controlPoints[(int)((checked((long)(controlPoints.Count)) - 2L))]);
        var allPoints__32366 = new List<global::Doroti.Ui.Offset> { DartRuntimePrimitives.RequireValue(startHandle), DartRuntimePrimitives.RequireValue(endHandle) };
        var alpha__32763 = 0.5;
        double reverseTension__32793 = (1.0 - tension);
        var result__32835 = new List<List<global::Doroti.Ui.Offset>>();
        for (var i__32875 = 0L; (i__32875 < (checked((long)(allPoints__32366.Count)) - 3L)); ++i__32875)
        {
            var curve__32927 = new List<global::Doroti.Ui.Offset> { allPoints__32366[(int)(i__32875)], allPoints__32366[(int)((i__32875 + 1L))], allPoints__32366[(int)((i__32875 + 2L))], allPoints__32366[(int)((i__32875 + 3L))] };
            global::Doroti.Ui.Offset diffCurve10__33032 = (curve__32927[(int)(1L)] - curve__32927[(int)(0L)]);
            global::Doroti.Ui.Offset diffCurve21__33086 = (curve__32927[(int)(2L)] - curve__32927[(int)(1L)]);
            global::Doroti.Ui.Offset diffCurve32__33140 = (curve__32927[(int)(3L)] - curve__32927[(int)(2L)]);
            double t01__33194 = global::Doroti.Runtime.Dart_mathLibrary.pow(diffCurve10__33032.distance, alpha__32763).toDouble();
            double t12__33269 = global::Doroti.Runtime.Dart_mathLibrary.pow(diffCurve21__33086.distance, alpha__32763).toDouble();
            double t23__33344 = global::Doroti.Runtime.Dart_mathLibrary.pow(diffCurve32__33140.distance, alpha__32763).toDouble();
            global::Doroti.Ui.Offset m1__33420 = (((diffCurve21__33086 + ((((diffCurve10__33032 / t01__33194) - (((curve__32927[(int)(2L)] - curve__32927[(int)(0L)])) / ((t01__33194 + t12__33269))))) * t12__33269))) * reverseTension__32793);
            global::Doroti.Ui.Offset m2__33562 = (((diffCurve21__33086 + ((((diffCurve32__33140 / t23__33344) - (((curve__32927[(int)(3L)] - curve__32927[(int)(1L)])) / ((t12__33269 + t23__33344))))) * t12__33269))) * reverseTension__32793);
            global::Doroti.Ui.Offset sumM12__33704 = (m1__33420 + m2__33562);
            var segment__33735 = new List<global::Doroti.Ui.Offset> { ((diffCurve21__33086 * -2.0) + sumM12__33704), (((diffCurve21__33086 * 3.0) - m1__33420) - sumM12__33704), m1__33420, curve__32927[(int)(1L)] };
            result__32835.Add(segment__33735);
        }
        return result__32835;
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
            global::Doroti.Ui.Offset seedPoint__34612 = this._cubicSegments[(int)(0L)][(int)(1L)];
            return ((((seedPoint__34612.dx + seedPoint__34612.dy)) * 10000L)).round();
            return default!;
        }
    }
    public override global::Doroti.Ui.Offset transformInternal(double t)
    {
        _initializeIfNeeded();
        double length__34806 = checked((long)(this._cubicSegments.Count)).toDouble();
        double position__34866 = default!;
        double localT__34893 = default!;
        long index__34915 = default!;
        if ((t < 1.0))
        {
            position__34866 = (t * length__34806);
            localT__34893 = (position__34866 % 1.0);
            index__34915 = position__34866.floor();
        }
        else
        {
            position__34866 = length__34806;
            localT__34893 = 1.0;
            index__34915 = (checked((long)(this._cubicSegments.Count)) - 1L);
        }
        List<global::Doroti.Ui.Offset> cubicControlPoints__35161 = this._cubicSegments[(int)(index__34915)];
        double localT2__35222 = (localT__34893 * localT__34893);
        return (((((cubicControlPoints__35161[(int)(0L)] * localT2__35222) * localT__34893) + (cubicControlPoints__35161[(int)(1L)] * localT2__35222)) + (cubicControlPoints__35161[(int)(2L)] * localT__34893)) + cubicControlPoints__35161[(int)(3L)]);
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
        var __instance = new CatmullRomCurve(default!, default!);
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
        global::Doroti.Ui.Offset startHandle__42976 = ((controlPoints[(int)(0L)] * 2.0) - controlPoints[(int)(1L)]);
        global::Doroti.Ui.Offset endHandle__43050 = ((controlPoints.Last() * 2.0) - controlPoints[(int)((checked((long)(controlPoints.Count)) - 2L))]);
        controlPoints = new List<global::Doroti.Ui.Offset> { startHandle__42976, endHandle__43050 };
        double lastX__43213 = -double.PositiveInfinity;
        for (var i__43252 = 0L; (i__43252 < checked((long)(controlPoints.Count))); ++i__43252)
        {
            if ((((i__43252 > 1L) && (i__43252 < (checked((long)(controlPoints.Count)) - 2L))) && (((controlPoints[(int)(i__43252)].dx <= 0.0) || (controlPoints[(int)(i__43252)].dx >= 1.0)))))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add("Control points must have X values between 0.0 and 1.0, exclusive. " + $"Point {i__43252} has an x value ({controlPoints![(int)(i__43252)].dx}) which is outside the range.");
                        return true;
                    });
                return false;
            }
            if ((controlPoints[(int)(i__43252)].dx <= lastX__43213))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add("Each X coordinate must be greater than the preceding X coordinate " + $"(i.e. must be monotonically increasing in X). Point {i__43252} has an x value of " + $"{controlPoints![(int)(i__43252)].dx}, which is not greater than {lastX__43213}");
                        return true;
                    });
                return false;
            }
            lastX__43213 = controlPoints[(int)(i__43252)].dx;
        }
        var success__44181 = true;
        lastX__43213 = -double.PositiveInfinity;
        var tolerance__44307 = 0.001;
        var testSpline__44335 = new CatmullRomSpline(controlPoints, tension: tension);
        double start__44416 = testSpline__44335.findInverse(0.0);
        double end__44470 = testSpline__44335.findInverse(1.0);
        IEnumerable<Curve2DSample> samplePoints__44539 = testSpline__44335.generateSamples(start: start__44416, end: end__44470);
        if (((samplePoints__44539.First().value.dy.abs() > tolerance__44307) || (((1.0 - samplePoints__44539.Last().value.dy)).abs() > tolerance__44307)))
        {
            var bail__44881 = true;
            success__44181 = false;
            DartRuntimePrimitives.Assert(() =>
                {
                    reasons?.Add($"The curve has more than one Y value at X = {samplePoints__44539.First().value.dx}. " + "Try moving some control points further away from this value of X, or increasing " + "the tension.");
                    bail__44881 = (reasons is null);
                    return true;
                });
            if (bail__44881)
            {
                return false;
            }
        }
        foreach (var sample__45491 in samplePoints__44539)
        {
            global::Doroti.Ui.Offset point__45536 = ((Curve2DSample)sample__45491).value;
            double t__45577 = ((Curve2DSample)sample__45491).t;
            double x__45610 = point__45536.dx;
            if ((((t__45577 >= start__44416) && (t__45577 <= end__44470)) && (((x__45610 < -0.001) || (x__45610 > (1.0 + 0.001))))))
            {
                var bail__45705 = true;
                success__44181 = false;
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add($"The resulting curve has an X value ({x__45610}) which is outside " + "the range [0.0, 1.0], inclusive.");
                        bail__45705 = (reasons is null);
                        return true;
                    });
                if (bail__45705)
                {
                    return false;
                }
            }
            if ((x__45610 < lastX__43213))
            {
                var bail__46277 = true;
                success__44181 = false;
                DartRuntimePrimitives.Assert(() =>
                    {
                        reasons?.Add($"The curve has more than one Y value at x = {x__45610}. Try moving " + "some control points further apart in X, or increasing the tension.");
                        bail__46277 = (reasons is null);
                        return true;
                    });
                if (bail__46277)
                {
                    return false;
                }
            }
            lastX__43213 = x__45610;
        }
        return success__44181;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double transformInternal(double t)
    {
        if ((checked((long)(this._precomputedSamples.Count)) == 0))
        {
            this._precomputedSamples.AddRange(_computeSamples(this.controlPoints, this.tension));
        }
        var start__47245 = 0L;
        long end__47264 = (checked((long)(this._precomputedSamples.Count)) - 1L);
        long mid__47310 = default!;
        global::Doroti.Ui.Offset value__47326 = default!;
        global::Doroti.Ui.Offset startValue__47344 = this._precomputedSamples[(int)(start__47245)].value;
        global::Doroti.Ui.Offset endValue__47402 = this._precomputedSamples[(int)(end__47264)].value;
        while (((end__47264 - start__47245) > 1L))
        {
            mid__47310 = (checked((long)(((end__47264 + start__47245)) / 2L)));
            value__47326 = this._precomputedSamples[(int)(mid__47310)].value;
            if ((t >= value__47326.dx))
            {
                start__47245 = mid__47310;
                startValue__47344 = value__47326;
            }
            else
            {
                end__47264 = mid__47310;
                endValue__47402 = value__47326;
            }
        }
        double t2__47882 = (((t - startValue__47344.dx)) / ((endValue__47402.dx - startValue__47344.dx)));
        return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(startValue__47344.dy, endValue__47402.dy, t2__47882));
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
        double s__51838 = (this.period / 4.0);
        t = (t - 1.0);
        return (-global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (10.0 * t)) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s__51838)) * ((Dart_mathLibrary.pi * 2.0))) / this.period)));
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
        double s__52711 = (this.period / 4.0);
        return ((global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (-10L * t)) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s__52711)) * ((Dart_mathLibrary.pi * 2.0))) / this.period))) + 1.0);
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
        double s__53605 = (this.period / 4.0);
        t = ((2.0 * t) - 1.0);
        if ((t < 0.0))
        {
            return ((-0.5 * global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (10.0 * t))) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s__53605)) * ((Dart_mathLibrary.pi * 2.0))) / this.period)));
        }
        else
        {
            return (((global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (-10.0 * t)) * global::Doroti.Runtime.Dart_mathLibrary.sin(((((t - s__53605)) * ((Dart_mathLibrary.pi * 2.0))) / this.period))) * 0.5) + 1.0);
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

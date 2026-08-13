// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/lsq_solver.dart
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

public class _Vector__lsq_solver
{
    internal virtual long _offset { get; private set; } = default!;
    internal virtual long _length { get; private set; } = default!;
    internal virtual List<double> _elements { get; private set; } = default!;

    internal _Vector__lsq_solver(long size)
    {
        this._offset = 0L;
        this._length = size;
        this._elements = new Float64List(size).ToList();
    }

    internal static _Vector__lsq_solver CreateFromVOL(List<double> values, long offset, long length)
    {
        var __instance = new _Vector__lsq_solver(default!);
        __instance._offset = offset;
        __instance._length = length;
        __instance._elements = values;
        return __instance;
    }

    public double this[long i]
    {
        get
        {
            return this._elements[(int)((i + this._offset))];
        }
        set
        {
            this._elements[(int)((i + this._offset))] = value;
        }
    }

    public virtual double op_Multiply(_Vector__lsq_solver a)
    {
        var result__776 = 0.0;
        for (var i__803 = 0L; (i__803 < this._length); i__803 += 1L)
        {
            result__776 += (this[i__803] * a[i__803]);
        }
        return result__776;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double norm() => global::Doroti.Flutter.Runtime.Dart_mathLibrary.sqrt((this.op_Multiply(this)));
}

internal class _Matrix__lsq_solver
{
    internal virtual long _columns { get; private set; } = default!;
    internal virtual List<double> _elements { get; private set; } = default!;

    internal _Matrix__lsq_solver(long rows, long cols)
    {
        this._columns = cols;
        this._elements = new Float64List((rows * cols)).ToList();
    }

    public virtual double get(long row, long col) => this._elements[(int)(((row * this._columns) + col))];
    public virtual void set(long row, long col, double value)
    {
        this._elements[(int)(((row * this._columns) + col))] = value;
    }

    public virtual _Vector__lsq_solver getRow(long row) => _Vector__lsq_solver.CreateFromVOL(this._elements, (row * this._columns), this._columns);
}

public class PolynomialFit
{
    public virtual List<double> coefficients { get; private set; } = default!;
    public virtual double confidence { get; set; } = default!;

    public PolynomialFit(long degree)
    {
        this.coefficients = new Float64List((degree + 1L)).ToList();
    }

    public override string ToString()
    {
        var coefficientString__2255 = this.coefficients.map<double, string>(((c) => c.toStringAsPrecision(3L))).ToList().ToString();
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "PolynomialFit"))}({coefficientString__2255}, confidence: {this.confidence.toStringAsFixed(3L)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LeastSquaresSolver
{
    public virtual List<double> x { get; private set; } = default!;
    public virtual List<double> y { get; private set; } = default!;
    public virtual List<double> w { get; private set; } = default!;

    public LeastSquaresSolver(List<double> x, List<double> y, List<double> w)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        System.Diagnostics.Debug.Assert((checked((long)(x.Count)) == checked((long)(y.Count))));
        System.Diagnostics.Debug.Assert((checked((long)(y.Count)) == checked((long)(w.Count))));
    }

    public virtual PolynomialFit? solve(long degree)
    {
        if ((degree > checked((long)(this.x.Count))))
        {
            return null;
        }
        var result__3259 = new PolynomialFit(degree);
        long m__3386 = checked((long)(this.x.Count));
        long n__3414 = (degree + 1L);
        var a__3514 = new _Matrix__lsq_solver(n__3414, m__3386);
        for (var h__3546 = 0L; (h__3546 < m__3386); h__3546 += 1L)
        {
            a__3514.set(0L, h__3546, this.w[(int)(h__3546)]);
            for (var i__3610 = 1L; (i__3610 < n__3414); i__3610 += 1L)
            {
                a__3514.set(i__3610, h__3546, (a__3514.get((i__3610 - 1L), h__3546) * this.x[(int)(h__3546)]));
            }
        }
        var q__3832 = new _Matrix__lsq_solver(n__3414, m__3386);
        var r__3910 = new _Matrix__lsq_solver(n__3414, n__3414);
        for (var j__3942 = 0L; (j__3942 < n__3414); j__3942 += 1L)
        {
            for (var h__3981 = 0L; (h__3981 < m__3386); h__3981 += 1L)
            {
                q__3832.set(j__3942, h__3981, a__3514.get(j__3942, h__3981));
            }
            for (var i__4062 = 0L; (i__4062 < j__3942); i__4062 += 1L)
            {
                double dot__4107 = (q__3832.getRow(j__3942).op_Multiply(q__3832.getRow(i__4062)));
                for (var h__4157 = 0L; (h__4157 < m__3386); h__4157 += 1L)
                {
                    q__3832.set(j__3942, h__4157, (q__3832.get(j__3942, h__4157) - (dot__4107 * q__3832.get(i__4062, h__4157))));
                }
            }
            double norm__4275 = q__3832.getRow(j__3942).norm();
            if ((norm__4275 < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                return null;
            }
            double inverseNorm__4461 = (1.0 / norm__4275);
            for (var h__4502 = 0L; (h__4502 < m__3386); h__4502 += 1L)
            {
                q__3832.set(j__3942, h__4502, (q__3832.get(j__3942, h__4502) * inverseNorm__4461));
            }
            for (var i__4597 = 0L; (i__4597 < n__3414); i__4597 += 1L)
            {
                r__3910.set(j__3942, i__4597, ((i__4597 < j__3942) ? 0.0 : (q__3832.getRow(j__3942).op_Multiply(a__3514.getRow(i__4597)))));
            }
        }
        var wy__4869 = new _Vector__lsq_solver(m__3386);
        for (var h__4899 = 0L; (h__4899 < m__3386); h__4899 += 1L)
        {
            wy__4869[h__4899] = (this.y[(int)(h__4899)] * this.w[(int)(h__4899)]);
        }
        for (long i__4969 = (n__3414 - 1L); (i__4969 >= 0L); i__4969 -= 1L)
        {
            ((PolynomialFit)result__3259).coefficients[(int)(i__4969)] = (q__3832.getRow(i__4969).op_Multiply(wy__4869));
            for (long j__5062 = (n__3414 - 1L); (j__5062 > i__4969); j__5062 -= 1L)
            {
                ((PolynomialFit)result__3259).coefficients[(int)(i__4969)] -= (r__3910.get(i__4969, j__5062) * ((PolynomialFit)result__3259).coefficients[(int)(j__5062)]);
            }
            ((PolynomialFit)result__3259).coefficients[(int)(i__4969)] /= r__3910.get(i__4969, i__4969);
        }
        var yMean__5549 = 0.0;
        for (var h__5575 = 0L; (h__5575 < m__3386); h__5575 += 1L)
        {
            yMean__5549 += this.y[(int)(h__5575)];
        }
        yMean__5549 /= m__3386;
        var sumSquaredError__5651 = 0.0;
        var sumSquaredTotal__5682 = 0.0;
        for (var h__5718 = 0L; (h__5718 < m__3386); h__5718 += 1L)
        {
            var term__5752 = 1.0;
            double err__5777 = (this.y[(int)(h__5718)] - ((PolynomialFit)result__3259).coefficients[(int)(0L)]);
            for (var i__5829 = 1L; (i__5829 < n__3414); i__5829 += 1L)
            {
                term__5752 *= this.x[(int)(h__5718)];
                err__5777 -= (term__5752 * ((PolynomialFit)result__3259).coefficients[(int)(i__5829)]);
            }
            sumSquaredError__5651 += (((this.w[(int)(h__5718)] * this.w[(int)(h__5718)]) * err__5777) * err__5777);
            double v__5998 = (this.y[(int)(h__5718)] - yMean__5549);
            sumSquaredTotal__5682 += (((this.w[(int)(h__5718)] * this.w[(int)(h__5718)]) * v__5998) * v__5998);
        }
        result__3259.confidence = ((sumSquaredTotal__5682 <= global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) ? 1.0 : (1.0 - ((sumSquaredError__5651 / sumSquaredTotal__5682))));
        return result__3259;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


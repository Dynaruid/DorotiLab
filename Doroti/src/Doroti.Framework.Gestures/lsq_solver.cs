// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/lsq_solver.dart
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
        var result = 0.0;
        for (var i = 0L; (i < this._length); i += 1L)
        {
            result += (this[i] * a[i]);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double norm() => global::Doroti.Runtime.Dart_mathLibrary.sqrt((this.op_Multiply(this)));
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
        var coefficientString = this.coefficients.map<double, string>(((c) => c.toStringAsPrecision(3L))).ToList().ToString();
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "PolynomialFit"))}({coefficientString}, confidence: {this.confidence.toStringAsFixed(3L)})";
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
        var result = new PolynomialFit(degree);
        long m = checked((long)(this.x.Count));
        long n = (degree + 1L);
        var a = new _Matrix__lsq_solver(n, m);
        for (var h = 0L; (h < m); h += 1L)
        {
            a.set(0L, h, this.w[(int)(h)]);
            for (var i = 1L; (i < n); i += 1L)
            {
                a.set(i, h, (a.get((i - 1L), h) * this.x[(int)(h)]));
            }
        }
        var q = new _Matrix__lsq_solver(n, m);
        var r = new _Matrix__lsq_solver(n, n);
        for (var j = 0L; (j < n); j += 1L)
        {
            for (var hLocal = 0L; (hLocal < m); hLocal += 1L)
            {
                q.set(j, hLocal, a.get(j, hLocal));
            }
            for (var iLocal = 0L; (iLocal < j); iLocal += 1L)
            {
                double dot = (q.getRow(j).op_Multiply(q.getRow(iLocal)));
                for (var hAlternate = 0L; (hAlternate < m); hAlternate += 1L)
                {
                    q.set(j, hAlternate, (q.get(j, hAlternate) - (dot * q.get(iLocal, hAlternate))));
                }
            }
            double normLocal = q.getRow(j).norm();
            if ((normLocal < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                return null;
            }
            double inverseNorm = (1.0 / normLocal);
            for (var hNested = 0L; (hNested < m); hNested += 1L)
            {
                q.set(j, hNested, (q.get(j, hNested) * inverseNorm));
            }
            for (var iAlternate = 0L; (iAlternate < n); iAlternate += 1L)
            {
                r.set(j, iAlternate, ((iAlternate < j) ? 0.0 : (q.getRow(j).op_Multiply(a.getRow(iAlternate)))));
            }
        }
        var wy = new _Vector__lsq_solver(m);
        for (var hCurrent = 0L; (hCurrent < m); hCurrent += 1L)
        {
            wy[hCurrent] = (this.y[(int)(hCurrent)] * this.w[(int)(hCurrent)]);
        }
        for (long iNested = (n - 1L); (iNested >= 0L); iNested -= 1L)
        {
            ((PolynomialFit)result).coefficients[(int)(iNested)] = (q.getRow(iNested).op_Multiply(wy));
            for (long jLocal = (n - 1L); (jLocal > iNested); jLocal -= 1L)
            {
                ((PolynomialFit)result).coefficients[(int)(iNested)] -= (r.get(iNested, jLocal) * ((PolynomialFit)result).coefficients[(int)(jLocal)]);
            }
            ((PolynomialFit)result).coefficients[(int)(iNested)] /= r.get(iNested, iNested);
        }
        var yMean = 0.0;
        for (var hNext = 0L; (hNext < m); hNext += 1L)
        {
            yMean += this.y[(int)(hNext)];
        }
        yMean /= m;
        var sumSquaredError = 0.0;
        var sumSquaredTotal = 0.0;
        for (var hCandidate = 0L; (hCandidate < m); hCandidate += 1L)
        {
            var term = 1.0;
            double err = (this.y[(int)(hCandidate)] - ((PolynomialFit)result).coefficients[(int)(0L)]);
            for (var iCurrent = 1L; (iCurrent < n); iCurrent += 1L)
            {
                term *= this.x[(int)(hCandidate)];
                err -= (term * ((PolynomialFit)result).coefficients[(int)(iCurrent)]);
            }
            sumSquaredError += (((this.w[(int)(hCandidate)] * this.w[(int)(hCandidate)]) * err) * err);
            double v = (this.y[(int)(hCandidate)] - yMean);
            sumSquaredTotal += (((this.w[(int)(hCandidate)] * this.w[(int)(hCandidate)]) * v) * v);
        }
        result.confidence = ((sumSquaredTotal <= global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) ? 1.0 : (1.0 - ((sumSquaredError / sumSquaredTotal))));
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


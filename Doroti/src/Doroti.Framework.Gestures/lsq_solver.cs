// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/lsq_solver.dart
using Doroti.Runtime;

namespace Doroti.Framework.Gestures;

public class _Vector__lsq_solver
{
    internal virtual long _offset { get; private set; } = default!;
    internal virtual long _length { get; private set; } = default!;
    internal virtual List<double> _elements { get; private set; } = default!;

    internal _Vector__lsq_solver(long size)
    {
        _offset = 0L;
        _length = size;
        _elements = Enumerable.Repeat(0.0, checked((int)size)).ToList();
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
        get { return _elements[(int)(i + _offset)]; }
        set { _elements[(int)(i + _offset)] = value; }
    }

    public virtual double op_Multiply(_Vector__lsq_solver a)
    {
        var result = 0.0;
        for (var i = 0L; i < _length; i += 1L)
        {
            result += this[i] * a[i];
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double norm() => Math.Sqrt(op_Multiply(this));
}

internal class _Matrix__lsq_solver
{
    internal virtual long _columns { get; private set; } = default!;
    internal virtual List<double> _elements { get; private set; } = default!;

    internal _Matrix__lsq_solver(long rows, long cols)
    {
        _columns = cols;
        _elements = Enumerable.Repeat(0.0, checked((int)(rows * cols))).ToList();
    }

    public virtual double get(long row, long col) => _elements[(int)((row * _columns) + col)];

    public virtual void set(long row, long col, double value)
    {
        _elements[(int)((row * _columns) + col)] = value;
    }

    public virtual _Vector__lsq_solver getRow(long row) =>
        _Vector__lsq_solver.CreateFromVOL(_elements, row * _columns, _columns);
}

public class PolynomialFit
{
    public virtual List<double> coefficients { get; private set; } = default!;
    public virtual double confidence { get; set; } = default!;

    public PolynomialFit(long degree)
    {
        coefficients = Enumerable.Repeat(0.0, checked((int)(degree + 1L))).ToList();
    }

    public override string ToString()
    {
        var coefficientString = coefficients
            .map((c) => c.toStringAsPrecision(3L))
            .ToList()
            .ToString();
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "PolynomialFit")}({coefficientString}, confidence: {confidence.toStringAsFixed(3L)})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        System.Diagnostics.Debug.Assert(checked(x.Count) == checked((long)y.Count));
        System.Diagnostics.Debug.Assert(checked(y.Count) == checked((long)w.Count));
    }

    public virtual PolynomialFit? solve(long degree)
    {
        if (degree > checked(x.Count))
        {
            return null;
        }
        var result = new PolynomialFit(degree);
        long m = checked(x.Count);
        long n = degree + 1L;
        var a = new _Matrix__lsq_solver(n, m);
        for (var h = 0L; h < m; h += 1L)
        {
            a.set(0L, h, w[(int)h]);
            for (var i = 1L; i < n; i += 1L)
            {
                a.set(i, h, a.get(i - 1L, h) * x[(int)h]);
            }
        }
        var q = new _Matrix__lsq_solver(n, m);
        var r = new _Matrix__lsq_solver(n, n);
        for (var j = 0L; j < n; j += 1L)
        {
            for (var hLocal = 0L; hLocal < m; hLocal += 1L)
            {
                q.set(j, hLocal, a.get(j, hLocal));
            }
            for (var iLocal = 0L; iLocal < j; iLocal += 1L)
            {
                double dot = q.getRow(j).op_Multiply(q.getRow(iLocal));
                for (var hAlternate = 0L; hAlternate < m; hAlternate += 1L)
                {
                    q.set(j, hAlternate, q.get(j, hAlternate) - (dot * q.get(iLocal, hAlternate)));
                }
            }
            double normLocal = q.getRow(j).norm();
            if (normLocal < Foundation.ConstantsLibrary.precisionErrorTolerance)
            {
                return null;
            }
            double inverseNorm = 1.0 / normLocal;
            for (var hNested = 0L; hNested < m; hNested += 1L)
            {
                q.set(j, hNested, q.get(j, hNested) * inverseNorm);
            }
            for (var iAlternate = 0L; iAlternate < n; iAlternate += 1L)
            {
                r.set(
                    j,
                    iAlternate,
                    (iAlternate < j) ? 0.0 : q.getRow(j).op_Multiply(a.getRow(iAlternate))
                );
            }
        }
        var wy = new _Vector__lsq_solver(m);
        for (var hCurrent = 0L; hCurrent < m; hCurrent += 1L)
        {
            wy[hCurrent] = y[(int)hCurrent] * w[(int)hCurrent];
        }
        for (long iNested = n - 1L; iNested >= 0L; iNested -= 1L)
        {
            result.coefficients[(int)iNested] = q.getRow(iNested).op_Multiply(wy);
            for (long jLocal = n - 1L; jLocal > iNested; jLocal -= 1L)
            {
                result.coefficients[(int)iNested] -=
                    r.get(iNested, jLocal) * result.coefficients[(int)jLocal];
            }
            result.coefficients[(int)iNested] /= r.get(iNested, iNested);
        }
        var yMean = 0.0;
        for (var hNext = 0L; hNext < m; hNext += 1L)
        {
            yMean += y[(int)hNext];
        }
        yMean /= m;
        var sumSquaredError = 0.0;
        var sumSquaredTotal = 0.0;
        for (var hCandidate = 0L; hCandidate < m; hCandidate += 1L)
        {
            var term = 1.0;
            double err = y[(int)hCandidate] - result.coefficients[(int)0L];
            for (var iCurrent = 1L; iCurrent < n; iCurrent += 1L)
            {
                term *= x[(int)hCandidate];
                err -= term * result.coefficients[(int)iCurrent];
            }
            sumSquaredError += w[(int)hCandidate] * w[(int)hCandidate] * err * err;
            double v = y[(int)hCandidate] - yMean;
            sumSquaredTotal += w[(int)hCandidate] * w[(int)hCandidate] * v * v;
        }
        result.confidence =
            (sumSquaredTotal <= Foundation.ConstantsLibrary.precisionErrorTolerance)
                ? 1.0
                : (1.0 - (sumSquaredError / sumSquaredTotal));
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

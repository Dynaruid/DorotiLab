// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/matrix_utils.dart
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

namespace Doroti.Framework.Painting;

public abstract class MatrixUtils
{
    internal static Float64List _minMax = new Float64List(4L);

    public static global::Doroti.Ui.Offset? getAsTranslation(Matrix4 transform)
    {
        if (transform.storage is [1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, double dx, double dy, 0.0, 1.0])
        {
            return new global::Doroti.Ui.Offset(dx, dy);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double? getAsScale(Matrix4 transform)
    {
        if (transform.storage is [double diagonal1, 0.0, 0.0, 0.0, 0.0, double diagonal2, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0] && ((diagonal1 == diagonal2)))
        {
            return diagonal1;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void multiplyInPlace(Matrix4 a, Matrix4 b)
    {
        Float64List aStorage = a.storage;
        double m00 = aStorage[0L];
        double m01 = aStorage[4L];
        double m02 = aStorage[8L];
        double m03 = aStorage[12L];
        double m10 = aStorage[1L];
        double m11 = aStorage[5L];
        double m12 = aStorage[9L];
        double m13 = aStorage[13L];
        double m20 = aStorage[2L];
        double m21 = aStorage[6L];
        double m22 = aStorage[10L];
        double m23 = aStorage[14L];
        double m30 = aStorage[3L];
        double m31 = aStorage[7L];
        double m32 = aStorage[11L];
        double m33 = aStorage[15L];
        Float64List bStorage = b.storage;
        double n00 = bStorage[0L];
        double n01 = bStorage[4L];
        double n02 = bStorage[8L];
        double n03 = bStorage[12L];
        double n10 = bStorage[1L];
        double n11 = bStorage[5L];
        double n12 = bStorage[9L];
        double n13 = bStorage[13L];
        double n20 = bStorage[2L];
        double n21 = bStorage[6L];
        double n22 = bStorage[10L];
        double n23 = bStorage[14L];
        double n30 = bStorage[3L];
        double n31 = bStorage[7L];
        double n32 = bStorage[11L];
        double n33 = bStorage[15L];
        bStorage[0L] = (((((m00 * n00)) + ((m01 * n10))) + ((m02 * n20))) + ((m03 * n30)));
        bStorage[4L] = (((((m00 * n01)) + ((m01 * n11))) + ((m02 * n21))) + ((m03 * n31)));
        bStorage[8L] = (((((m00 * n02)) + ((m01 * n12))) + ((m02 * n22))) + ((m03 * n32)));
        bStorage[12L] = (((((m00 * n03)) + ((m01 * n13))) + ((m02 * n23))) + ((m03 * n33)));
        bStorage[1L] = (((((m10 * n00)) + ((m11 * n10))) + ((m12 * n20))) + ((m13 * n30)));
        bStorage[5L] = (((((m10 * n01)) + ((m11 * n11))) + ((m12 * n21))) + ((m13 * n31)));
        bStorage[9L] = (((((m10 * n02)) + ((m11 * n12))) + ((m12 * n22))) + ((m13 * n32)));
        bStorage[13L] = (((((m10 * n03)) + ((m11 * n13))) + ((m12 * n23))) + ((m13 * n33)));
        bStorage[2L] = (((((m20 * n00)) + ((m21 * n10))) + ((m22 * n20))) + ((m23 * n30)));
        bStorage[6L] = (((((m20 * n01)) + ((m21 * n11))) + ((m22 * n21))) + ((m23 * n31)));
        bStorage[10L] = (((((m20 * n02)) + ((m21 * n12))) + ((m22 * n22))) + ((m23 * n32)));
        bStorage[14L] = (((((m20 * n03)) + ((m21 * n13))) + ((m22 * n23))) + ((m23 * n33)));
        bStorage[3L] = (((((m30 * n00)) + ((m31 * n10))) + ((m32 * n20))) + ((m33 * n30)));
        bStorage[7L] = (((((m30 * n01)) + ((m31 * n11))) + ((m32 * n21))) + ((m33 * n31)));
        bStorage[11L] = (((((m30 * n02)) + ((m31 * n12))) + ((m32 * n22))) + ((m33 * n32)));
        bStorage[15L] = (((((m30 * n03)) + ((m31 * n13))) + ((m32 * n23))) + ((m33 * n33)));
    }

    public static bool matrixEquals(Matrix4? a, Matrix4? b)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return true;
        }
        DartRuntimePrimitives.Assert(() => ((a is not null) || (b is not null)));
        if ((a is null))
        {
            return isIdentity(b!);
        }
        if ((b is null))
        {
            return isIdentity(a);
        }
        return ((((((((((((((((a.storage[0L] == b.storage[0L]) && (a.storage[1L] == b.storage[1L])) && (a.storage[2L] == b.storage[2L])) && (a.storage[3L] == b.storage[3L])) && (a.storage[4L] == b.storage[4L])) && (a.storage[5L] == b.storage[5L])) && (a.storage[6L] == b.storage[6L])) && (a.storage[7L] == b.storage[7L])) && (a.storage[8L] == b.storage[8L])) && (a.storage[9L] == b.storage[9L])) && (a.storage[10L] == b.storage[10L])) && (a.storage[11L] == b.storage[11L])) && (a.storage[12L] == b.storage[12L])) && (a.storage[13L] == b.storage[13L])) && (a.storage[14L] == b.storage[14L])) && (a.storage[15L] == b.storage[15L]));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool isIdentity(Matrix4 a)
    {
        return ((((((((((((((((a.storage[0L] == 1.0) && (a.storage[1L] == 0.0)) && (a.storage[2L] == 0.0)) && (a.storage[3L] == 0.0)) && (a.storage[4L] == 0.0)) && (a.storage[5L] == 1.0)) && (a.storage[6L] == 0.0)) && (a.storage[7L] == 0.0)) && (a.storage[8L] == 0.0)) && (a.storage[9L] == 0.0)) && (a.storage[10L] == 1.0)) && (a.storage[11L] == 0.0)) && (a.storage[12L] == 0.0)) && (a.storage[13L] == 0.0)) && (a.storage[14L] == 0.0)) && (a.storage[15L] == 1.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Offset transformPoint(Matrix4 transform, Offset point)
    {
        Float64List storageLocal = transform.storage;
        double x = point.dx;
        double y = point.dy;
        double rx = (((storageLocal[0L] * x) + (storageLocal[4L] * y)) + storageLocal[12L]);
        double ry = (((storageLocal[1L] * x) + (storageLocal[5L] * y)) + storageLocal[13L]);
        double rw = (((storageLocal[3L] * x) + (storageLocal[7L] * y)) + storageLocal[15L]);
        if ((rw == 1.0))
        {
            return new global::Doroti.Ui.Offset(rx, ry);
        }
        else
        {
            return new global::Doroti.Ui.Offset((rx / rw), (ry / rw));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _safeTransformRect(Matrix4 transform, Rect rect)
    {
        Float64List storageLocal = transform.storage;
        bool isAffine = (((storageLocal[3L] == 0.0) && (storageLocal[7L] == 0.0)) && (storageLocal[15L] == 1.0));
        _accumulate(storageLocal, rect.left, rect.top, true, isAffine);
        _accumulate(storageLocal, rect.right, rect.top, false, isAffine);
        _accumulate(storageLocal, rect.left, rect.bottom, false, isAffine);
        _accumulate(storageLocal, rect.right, rect.bottom, false, isAffine);
        return global::Doroti.Ui.Rect.fromLTRB(_minMax[0L], _minMax[1L], _minMax[2L], _minMax[3L]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _accumulate(Float64List m, double x, double y, bool first, bool isAffine)
    {
        double w = (isAffine ? 1.0 : (1.0 / ((((m[3L] * x) + (m[7L] * y)) + m[15L]))));
        double tx = (((((m[0L] * x) + (m[4L] * y)) + m[12L])) * w);
        double ty = (((((m[1L] * x) + (m[5L] * y)) + m[13L])) * w);
        if (first)
        {
            _minMax[0L] = _minMax[2L] = tx;
            _minMax[1L] = _minMax[3L] = ty;
        }
        else
        {
            if ((tx < _minMax[0L]))
            {
                _minMax[0L] = tx;
            }
            if ((ty < _minMax[1L]))
            {
                _minMax[1L] = ty;
            }
            if ((tx > _minMax[2L]))
            {
                _minMax[2L] = tx;
            }
            if ((ty > _minMax[3L]))
            {
                _minMax[3L] = ty;
            }
        }
    }

    public static global::Doroti.Ui.Rect transformRect(Matrix4 transform, Rect rect)
    {
        Float64List storageLocal = transform.storage;
        double x = rect.left;
        double y = rect.top;
        double w = (rect.right - x);
        double h = (rect.bottom - y);
        if ((!double.IsFinite(w) || !double.IsFinite(h)))
        {
            return _safeTransformRect(transform, rect);
        }
        double wx = (storageLocal[0L] * w);
        double hx = (storageLocal[4L] * h);
        double rx = (((storageLocal[0L] * x) + (storageLocal[4L] * y)) + storageLocal[12L]);
        double wy = (storageLocal[1L] * w);
        double hy = (storageLocal[5L] * h);
        double ry = (((storageLocal[1L] * x) + (storageLocal[5L] * y)) + storageLocal[13L]);
        if ((((storageLocal[3L] == 0.0) && (storageLocal[7L] == 0.0)) && (storageLocal[15L] == 1.0)))
        {
            var leftLocal = rx;
            var rightLocal = rx;
            if ((wx < 0L))
            {
                leftLocal += wx;
            }
            else
            {
                rightLocal += wx;
            }
            if ((hx < 0L))
            {
                leftLocal += hx;
            }
            else
            {
                rightLocal += hx;
            }
            var topLocal = ry;
            var bottomLocal = ry;
            if ((wy < 0L))
            {
                topLocal += wy;
            }
            else
            {
                bottomLocal += wy;
            }
            if ((hy < 0L))
            {
                topLocal += hy;
            }
            else
            {
                bottomLocal += hy;
            }
            return global::Doroti.Ui.Rect.fromLTRB(leftLocal, topLocal, rightLocal, bottomLocal);
        }
        else
        {
            double ww = (storageLocal[3L] * w);
            double hw = (storageLocal[7L] * h);
            double rw = (((storageLocal[3L] * x) + (storageLocal[7L] * y)) + storageLocal[15L]);
            double ulx = (rx / rw);
            double uly = (ry / rw);
            double urx = (((rx + wx)) / ((rw + ww)));
            double ury = (((ry + wy)) / ((rw + ww)));
            double llx = (((rx + hx)) / ((rw + hw)));
            double lly = (((ry + hy)) / ((rw + hw)));
            double lrx = ((((rx + wx) + hx)) / (((rw + ww) + hw)));
            double lry = ((((ry + wy) + hy)) / (((rw + ww) + hw)));
            return global::Doroti.Ui.Rect.fromLTRB(_min4(ulx, urx, llx, lrx), _min4(uly, ury, lly, lry), _max4(ulx, urx, llx, lrx), _max4(uly, ury, lly, lry));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _min4(double a, double b, double c, double d)
    {
        var e = (((DartRuntimePrimitives.RequireValue(a) < DartRuntimePrimitives.RequireValue(b))) ? DartRuntimePrimitives.RequireValue(a) : DartRuntimePrimitives.RequireValue(b));
        var f = (((c < d)) ? c : d);
        return (((e < f)) ? e : f);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _max4(double a, double b, double c, double d)
    {
        var e = (((DartRuntimePrimitives.RequireValue(a) > DartRuntimePrimitives.RequireValue(b))) ? DartRuntimePrimitives.RequireValue(a) : DartRuntimePrimitives.RequireValue(b));
        var f = (((c > d)) ? c : d);
        return (((e > f)) ? e : f);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Rect inverseTransformRect(Matrix4 transform, Rect rect)
    {
        if (isIdentity(transform))
        {
            return rect;
        }
        transform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.copy(transform);
    __cascade.invert();
    return __cascade;
}))();
        return transformRect(transform, rect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Matrix4 createCylindricalProjectionTransform(double radius, double angle, double perspective = 0.001, Axis orientation = Axis.vertical)
    {
        DartRuntimePrimitives.Assert(() => ((perspective >= 0L) && (perspective <= 1.0)));
        var result = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.setEntry(3L, 2L, -perspective);
    __cascade.setEntry(2L, 3L, -radius);
    __cascade.setEntry(3L, 3L, ((perspective * radius) + 1.0));
    return __cascade;
}))();
        result = ((Matrix4?)(object?)(result * (((orientation switch { Axis.horizontal => Matrix4.rotationY(angle), Axis.vertical => Matrix4.rotationX(angle), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }) * Matrix4.translationValues(0.0, 0.0, radius)))))!;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Matrix4 forceToPoint(Offset offset)
    {
        var result = Matrix4.zero();
        Float64List storageLocal = result.storage;
        storageLocal[10L] = 1;
        storageLocal[12L] = offset.dx;
        storageLocal[13L] = offset.dy;
        storageLocal[15L] = 1;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Matrix_utilsLibrary
{
    public static List<string> debugDescribeTransform(Matrix4? transform)
    {
        if ((transform is null))
        {
            return new List<string> { "null" };
        }
        return new List<string> { $"[0] {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 3L)))}", $"[1] {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 3L)))}", $"[2] {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 3L)))}", $"[3] {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 3L)))}" };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class TransformProperty : DiagnosticsProperty<Matrix4>
{
    public TransformProperty(string name, Matrix4? value, bool showName = true, object? defaultValue = default!, DiagnosticLevel level = DiagnosticLevel.info) : base(name, value, showName: showName, defaultValue: defaultValue ?? global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue, level: level)
    {
    }

    public virtual string valueToString(TextTreeConfiguration? parentConfiguration = null)
    {
        if (((parentConfiguration is not null) && !parentConfiguration.lineBreakProperties))
        {
            var values = new List<string> { $"{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 3L)))}", $"{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 3L)))}", $"{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 3L)))}", $"{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 0L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 1L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 2L)))},{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 3L)))}" };
            return $"[{string.Join("; ", values)}]";
        }
        return string.Join("\n", Matrix_utilsLibrary.debugDescribeTransform(value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


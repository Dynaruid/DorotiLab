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

namespace Doroti.Generated.Framework.Painting;

public abstract class MatrixUtils
{
    internal static Float64List _minMax = new Float64List(4L);

    public static global::Doroti.Ui.Offset? getAsTranslation(Matrix4 transform)
    {
        if (transform.storage is [1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, double dx__949, double dy__972, 0.0, 1.0])
        {
            return new global::Doroti.Ui.Offset(dx__949, dy__972);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double? getAsScale(Matrix4 transform)
    {
        if (transform.storage is [double diagonal1__1451, 0.0, 0.0, 0.0, 0.0, double diagonal2__1525, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0] && ((diagonal1__1451 == diagonal2__1525)))
        {
            return diagonal1__1451;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static void multiplyInPlace(Matrix4 a, Matrix4 b)
    {
        Float64List aStorage__2090 = a.storage;
        double m00__2129 = aStorage__2090[0L];
        double m01__2165 = aStorage__2090[4L];
        double m02__2201 = aStorage__2090[8L];
        double m03__2237 = aStorage__2090[12L];
        double m10__2274 = aStorage__2090[1L];
        double m11__2310 = aStorage__2090[5L];
        double m12__2346 = aStorage__2090[9L];
        double m13__2382 = aStorage__2090[13L];
        double m20__2419 = aStorage__2090[2L];
        double m21__2455 = aStorage__2090[6L];
        double m22__2491 = aStorage__2090[10L];
        double m23__2528 = aStorage__2090[14L];
        double m30__2565 = aStorage__2090[3L];
        double m31__2601 = aStorage__2090[7L];
        double m32__2637 = aStorage__2090[11L];
        double m33__2674 = aStorage__2090[15L];
        Float64List bStorage__2716 = b.storage;
        double n00__2755 = bStorage__2716[0L];
        double n01__2791 = bStorage__2716[4L];
        double n02__2827 = bStorage__2716[8L];
        double n03__2863 = bStorage__2716[12L];
        double n10__2900 = bStorage__2716[1L];
        double n11__2936 = bStorage__2716[5L];
        double n12__2972 = bStorage__2716[9L];
        double n13__3008 = bStorage__2716[13L];
        double n20__3045 = bStorage__2716[2L];
        double n21__3081 = bStorage__2716[6L];
        double n22__3117 = bStorage__2716[10L];
        double n23__3154 = bStorage__2716[14L];
        double n30__3191 = bStorage__2716[3L];
        double n31__3227 = bStorage__2716[7L];
        double n32__3263 = bStorage__2716[11L];
        double n33__3300 = bStorage__2716[15L];
        bStorage__2716[0L] = (((((m00__2129 * n00__2755)) + ((m01__2165 * n10__2900))) + ((m02__2201 * n20__3045))) + ((m03__2237 * n30__3191)));
        bStorage__2716[4L] = (((((m00__2129 * n01__2791)) + ((m01__2165 * n11__2936))) + ((m02__2201 * n21__3081))) + ((m03__2237 * n31__3227)));
        bStorage__2716[8L] = (((((m00__2129 * n02__2827)) + ((m01__2165 * n12__2972))) + ((m02__2201 * n22__3117))) + ((m03__2237 * n32__3263)));
        bStorage__2716[12L] = (((((m00__2129 * n03__2863)) + ((m01__2165 * n13__3008))) + ((m02__2201 * n23__3154))) + ((m03__2237 * n33__3300)));
        bStorage__2716[1L] = (((((m10__2274 * n00__2755)) + ((m11__2310 * n10__2900))) + ((m12__2346 * n20__3045))) + ((m13__2382 * n30__3191)));
        bStorage__2716[5L] = (((((m10__2274 * n01__2791)) + ((m11__2310 * n11__2936))) + ((m12__2346 * n21__3081))) + ((m13__2382 * n31__3227)));
        bStorage__2716[9L] = (((((m10__2274 * n02__2827)) + ((m11__2310 * n12__2972))) + ((m12__2346 * n22__3117))) + ((m13__2382 * n32__3263)));
        bStorage__2716[13L] = (((((m10__2274 * n03__2863)) + ((m11__2310 * n13__3008))) + ((m12__2346 * n23__3154))) + ((m13__2382 * n33__3300)));
        bStorage__2716[2L] = (((((m20__2419 * n00__2755)) + ((m21__2455 * n10__2900))) + ((m22__2491 * n20__3045))) + ((m23__2528 * n30__3191)));
        bStorage__2716[6L] = (((((m20__2419 * n01__2791)) + ((m21__2455 * n11__2936))) + ((m22__2491 * n21__3081))) + ((m23__2528 * n31__3227)));
        bStorage__2716[10L] = (((((m20__2419 * n02__2827)) + ((m21__2455 * n12__2972))) + ((m22__2491 * n22__3117))) + ((m23__2528 * n32__3263)));
        bStorage__2716[14L] = (((((m20__2419 * n03__2863)) + ((m21__2455 * n13__3008))) + ((m22__2491 * n23__3154))) + ((m23__2528 * n33__3300)));
        bStorage__2716[3L] = (((((m30__2565 * n00__2755)) + ((m31__2601 * n10__2900))) + ((m32__2637 * n20__3045))) + ((m33__2674 * n30__3191)));
        bStorage__2716[7L] = (((((m30__2565 * n01__2791)) + ((m31__2601 * n11__2936))) + ((m32__2637 * n21__3081))) + ((m33__2674 * n31__3227)));
        bStorage__2716[11L] = (((((m30__2565 * n02__2827)) + ((m31__2601 * n12__2972))) + ((m32__2637 * n22__3117))) + ((m33__2674 * n32__3263)));
        bStorage__2716[15L] = (((((m30__2565 * n03__2863)) + ((m31__2601 * n13__3008))) + ((m32__2637 * n23__3154))) + ((m33__2674 * n33__3300)));
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
        Float64List storage__6986 = transform.storage;
        double x__7032 = point.dx;
        double y__7063 = point.dy;
        double rx__7246 = (((storage__6986[0L] * x__7032) + (storage__6986[4L] * y__7063)) + storage__6986[12L]);
        double ry__7315 = (((storage__6986[1L] * x__7032) + (storage__6986[5L] * y__7063)) + storage__6986[13L]);
        double rw__7384 = (((storage__6986[3L] * x__7032) + (storage__6986[7L] * y__7063)) + storage__6986[15L]);
        if ((rw__7384 == 1.0))
        {
            return new global::Doroti.Ui.Offset(rx__7246, ry__7315);
        }
        else
        {
            return new global::Doroti.Ui.Offset((rx__7246 / rw__7384), (ry__7315 / rw__7384));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect _safeTransformRect(Matrix4 transform, Rect rect)
    {
        Float64List storage__7939 = transform.storage;
        bool isAffine__7983 = (((storage__7939[3L] == 0.0) && (storage__7939[7L] == 0.0)) && (storage__7939[15L] == 1.0));
        _accumulate(storage__7939, rect.left, rect.top, true, isAffine__7983);
        _accumulate(storage__7939, rect.right, rect.top, false, isAffine__7983);
        _accumulate(storage__7939, rect.left, rect.bottom, false, isAffine__7983);
        _accumulate(storage__7939, rect.right, rect.bottom, false, isAffine__7983);
        return global::Doroti.Ui.Rect.fromLTRB(_minMax[0L], _minMax[1L], _minMax[2L], _minMax[3L]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _accumulate(Float64List m, double x, double y, bool first, bool isAffine)
    {
        double w__8560 = (isAffine ? 1.0 : (1.0 / ((((m[3L] * x) + (m[7L] * y)) + m[15L]))));
        double tx__8635 = (((((m[0L] * x) + (m[4L] * y)) + m[12L])) * w__8560);
        double ty__8692 = (((((m[1L] * x) + (m[5L] * y)) + m[13L])) * w__8560);
        if (first)
        {
            _minMax[0L] = _minMax[2L] = tx__8635;
            _minMax[1L] = _minMax[3L] = ty__8692;
        }
        else
        {
            if ((tx__8635 < _minMax[0L]))
            {
                _minMax[0L] = tx__8635;
            }
            if ((ty__8692 < _minMax[1L]))
            {
                _minMax[1L] = ty__8692;
            }
            if ((tx__8635 > _minMax[2L]))
            {
                _minMax[2L] = tx__8635;
            }
            if ((ty__8692 > _minMax[3L]))
            {
                _minMax[3L] = ty__8692;
            }
        }
    }

    public static global::Doroti.Ui.Rect transformRect(Matrix4 transform, Rect rect)
    {
        Float64List storage__9509 = transform.storage;
        double x__9555 = rect.left;
        double y__9587 = rect.top;
        double w__9618 = (rect.right - x__9555);
        double h__9655 = (rect.bottom - y__9587);
        if ((!double.IsFinite(w__9618) || !double.IsFinite(h__9655)))
        {
            return _safeTransformRect(transform, rect);
        }
        double wx__18397 = (storage__9509[0L] * w__9618);
        double hx__18435 = (storage__9509[4L] * h__9655);
        double rx__18473 = (((storage__9509[0L] * x__9555) + (storage__9509[4L] * y__9587)) + storage__9509[12L]);
        double wy__18543 = (storage__9509[1L] * w__9618);
        double hy__18581 = (storage__9509[5L] * h__9655);
        double ry__18619 = (((storage__9509[1L] * x__9555) + (storage__9509[5L] * y__9587)) + storage__9509[13L]);
        if ((((storage__9509[3L] == 0.0) && (storage__9509[7L] == 0.0)) && (storage__9509[15L] == 1.0)))
        {
            var left__18754 = rx__18473;
            var right__18775 = rx__18473;
            if ((wx__18397 < 0L))
            {
                left__18754 += wx__18397;
            }
            else
            {
                right__18775 += wx__18397;
            }
            if ((hx__18435 < 0L))
            {
                left__18754 += hx__18435;
            }
            else
            {
                right__18775 += hx__18435;
            }
            var top__18966 = ry__18619;
            var bottom__18986 = ry__18619;
            if ((wy__18543 < 0L))
            {
                top__18966 += wy__18543;
            }
            else
            {
                bottom__18986 += wy__18543;
            }
            if ((hy__18581 < 0L))
            {
                top__18966 += hy__18581;
            }
            else
            {
                bottom__18986 += hy__18581;
            }
            return global::Doroti.Ui.Rect.fromLTRB(left__18754, top__18966, right__18775, bottom__18986);
        }
        else
        {
            double ww__19254 = (storage__9509[3L] * w__9618);
            double hw__19294 = (storage__9509[7L] * h__9655);
            double rw__19334 = (((storage__9509[3L] * x__9555) + (storage__9509[7L] * y__9587)) + storage__9509[15L]);
            double ulx__19406 = (rx__18473 / rw__19334);
            double uly__19440 = (ry__18619 / rw__19334);
            double urx__19474 = (((rx__18473 + wx__18397)) / ((rw__19334 + ww__19254)));
            double ury__19522 = (((ry__18619 + wy__18543)) / ((rw__19334 + ww__19254)));
            double llx__19570 = (((rx__18473 + hx__18435)) / ((rw__19334 + hw__19294)));
            double lly__19618 = (((ry__18619 + hy__18581)) / ((rw__19334 + hw__19294)));
            double lrx__19666 = ((((rx__18473 + wx__18397) + hx__18435)) / (((rw__19334 + ww__19254) + hw__19294)));
            double lry__19724 = ((((ry__18619 + wy__18543) + hy__18581)) / (((rw__19334 + ww__19254) + hw__19294)));
            return global::Doroti.Ui.Rect.fromLTRB(_min4(ulx__19406, urx__19474, llx__19570, lrx__19666), _min4(uly__19440, ury__19522, lly__19618, lry__19724), _max4(ulx__19406, urx__19474, llx__19570, lrx__19666), _max4(uly__19440, ury__19522, lly__19618, lry__19724));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _min4(double a, double b, double c, double d)
    {
        var e__20026 = (((DartRuntimePrimitives.RequireValue(a) < DartRuntimePrimitives.RequireValue(b))) ? DartRuntimePrimitives.RequireValue(a) : DartRuntimePrimitives.RequireValue(b));
        var f__20057 = (((c < d)) ? c : d);
        return (((e__20026 < f__20057)) ? e__20026 : f__20057);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _max4(double a, double b, double c, double d)
    {
        var e__20185 = (((DartRuntimePrimitives.RequireValue(a) > DartRuntimePrimitives.RequireValue(b))) ? DartRuntimePrimitives.RequireValue(a) : DartRuntimePrimitives.RequireValue(b));
        var f__20216 = (((c > d)) ? c : d);
        return (((e__20185 > f__20216)) ? e__20185 : f__20216);
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
        var result__23848 = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.setEntry(3L, 2L, -perspective);
    __cascade.setEntry(2L, 3L, -radius);
    __cascade.setEntry(3L, 3L, ((perspective * radius) + 1.0));
    return __cascade;
}))();
        result__23848 = ((Matrix4?)(object?)(result__23848 * (((orientation switch { Axis.horizontal => Matrix4.rotationY(angle), Axis.vertical => Matrix4.rotationX(angle), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }) * Matrix4.translationValues(0.0, 0.0, radius)))))!;
        return result__23848;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Matrix4 forceToPoint(Offset offset)
    {
        var result__24655 = Matrix4.zero();
        Float64List storage__24702 = result__24655.storage;
        storage__24702[10L] = 1;
        storage__24702[12L] = offset.dx;
        storage__24702[13L] = offset.dy;
        storage__24702[15L] = 1;
        return result__24655;
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
        return new List<string> { $"[0] {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(0L, 3L)))}", $"[1] {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(1L, 3L)))}", $"[2] {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(2L, 3L)))}", $"[3] {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(transform.entry(3L, 3L)))}" };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class TransformProperty : DiagnosticsProperty<Matrix4>
{
    public TransformProperty(string name, Matrix4? value, bool showName = true, object? defaultValue = default!, DiagnosticLevel level = DiagnosticLevel.info) : base(name, value, showName: showName, defaultValue: defaultValue ?? global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue, level: level)
    {
    }

    public virtual string valueToString(TextTreeConfiguration? parentConfiguration = null)
    {
        if (((parentConfiguration is not null) && !parentConfiguration.lineBreakProperties))
        {
            var values__26772 = new List<string> { $"{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(0L, 3L)))}", $"{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(1L, 3L)))}", $"{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(2L, 3L)))}", $"{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 0L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 1L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 2L)))},{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value!.entry(3L, 3L)))}" };
            return $"[{string.Join("; ", values__26772)}]";
        }
        return string.Join("\n", Matrix_utilsLibrary.debugDescribeTransform(value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


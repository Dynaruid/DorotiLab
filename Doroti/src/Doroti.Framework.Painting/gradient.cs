// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/gradient.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

internal class _ColorsAndStops__gradient
{
    public virtual List<Color> colors { get; private set; } = default!;
    public virtual List<double> stops { get; private set; } = default!;

    internal _ColorsAndStops__gradient(List<Color> colors, List<double> stops)
    {
        this.colors = colors;
        this.stops = stops;
    }
}

public static partial class GradientLibrary
{
    internal static Color _sample(List<Color> colors, List<double> stops, double t)
    {
        DartRuntimePrimitives.Assert(() => checked((long)colors.Count) != 0);
        DartRuntimePrimitives.Assert(() => checked((long)stops.Count) != 0);
        if (t <= stops.First())
        {
            return colors.First();
        }
        if (t >= stops.Last())
        {
            return colors.Last();
        }
        long index = stops.lastIndexWhere((s) => s <= t);
        DartRuntimePrimitives.Assert(() => index != -1L);
        return Dart_uiLibrary.Color.lerp(
            colors[(int)index],
            colors[(int)(index + 1L)],
            (t - stops[(int)index]) / (stops[(int)(index + 1L)] - stops[(int)index])
        )!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class GradientLibrary
{
    internal static _ColorsAndStops__gradient _interpolateColorsAndStops(
        List<Color> aColors,
        List<double> aStops,
        List<Color> bColors,
        List<double> bStops,
        double t
    )
    {
        DartRuntimePrimitives.Assert(() => checked(aColors.Count) >= 2L);
        DartRuntimePrimitives.Assert(() => checked(bColors.Count) >= 2L);
        DartRuntimePrimitives.Assert(() => checked(aStops.Count) == checked((long)aColors.Count));
        DartRuntimePrimitives.Assert(() => checked(bStops.Count) == checked((long)bColors.Count));
        var stops = (
            (Func<SortedSet<double>>)(
                () =>
                {
                    var __cascade = new SortedSet<double>();
                    __cascade.UnionWith(aStops);
                    __cascade.UnionWith(bStops);
                    return __cascade;
                }
            )
        )();
        List<double> interpolatedStops = stops.ToList();
        List<Color> interpolatedColors = interpolatedStops
            .map(
                (stop) =>
                    Dart_uiLibrary.Color.lerp(
                        _sample(aColors, aStops, stop),
                        _sample(bColors, bStops, stop),
                        t
                    )!
            )
            .ToList();
        return new _ColorsAndStops__gradient(interpolatedColors, interpolatedStops);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public interface GradientTransform
{
    public Matrix4? transform(Rect bounds, TextDirection? textDirection = null);
}

public class GradientRotation : GradientTransform
{
    public virtual double radians { get; private set; } = default!;

    public GradientRotation(double radians)
    {
        this.radians = radians;
    }

    public virtual Matrix4? transform(Rect bounds, TextDirection? textDirection = null)
    {
        double sinRadians = Dart_mathLibrary.sin(radians);
        double oneMinusCosRadians = 1L - Dart_mathLibrary.cos(radians);
        Offset centerLocal = bounds.center;
        double originX = (sinRadians * centerLocal.dy) + (oneMinusCosRadians * centerLocal.dx);
        double originY = (-sinRadians * centerLocal.dx) + (oneMinusCosRadians * centerLocal.dy);
        return (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = Matrix4.identity();
                    __cascade.translateByDouble(originX, originY, 0, 1);
                    __cascade.rotateZ(radians);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as GradientRotation;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is GradientRotation) && (__other.radians == radians);
    }

    public override int GetHashCode() => radians.GetHashCode();

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "GradientRotation")}(radians: {Foundation.DebugLibrary.debugFormatDouble(radians)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class Gradient
{
    public virtual List<Color> colors { get; private set; } = default!;
    public virtual List<double>? stops { get; private set; }
    public virtual GradientTransform? transform { get; private set; }

    protected Gradient(
        List<Color> colors,
        List<double>? stops = null,
        GradientTransform? transform = null
    )
    {
        this.colors = colors;
        this.stops = stops;
        this.transform = transform;
    }

    internal virtual List<double> _impliedStops()
    {
        if (stops is not null)
        {
            return stops!;
        }
        DartRuntimePrimitives.Assert(() => checked(colors.Count) >= 2L);
        double separation = 1.0 / (checked(colors.Count) - 1L);
        return new List<double>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)checked((long)colors.Count))),
                (index) => index * separation
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Shader createShader(Rect rect, TextDirection? textDirection = null);
    public abstract Gradient scale(double factor);
    public abstract Gradient withOpacity(double opacity);

    public virtual Gradient fromColor(Color color)
    {
        return new LinearGradient(
            colors: new List<Color>(
                Enumerable.Repeat(color, checked((int)checked((long)colors.Count)))
            ),
            stops: stops,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Gradient? lerpFrom(Gradient? a, double t)
    {
        if (a is null)
        {
            return scale(t);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Gradient? lerpTo(Gradient? b, double t)
    {
        if (b is null)
        {
            return scale(1.0 - t);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Gradient? lerp(Gradient? a, Gradient? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        Gradient? result = default!;
        if (b is not null)
        {
            result = b.lerpFrom(a, t);
        }
        if ((result is null) && (a is not null))
        {
            result = a.lerpTo(b, t);
        }
        if (result is not null)
        {
            return result;
        }
        DartRuntimePrimitives.Assert(() => (a is not null) && (b is not null));
        return (t < 0.5) ? a!.scale(1.0 - (t * 2.0)) : b!.scale((t - 0.5) * 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Float64List? _resolveTransform(Rect bounds, TextDirection? textDirection)
    {
        return transform?.transform(bounds, textDirection: textDirection)?.storage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class LinearGradient : Gradient
{
    public virtual AlignmentGeometry begin { get; private set; } = default!;
    public virtual AlignmentGeometry end { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;

    public LinearGradient(
        AlignmentGeometry begin = default!,
        AlignmentGeometry end = default!,
        List<Color> colors = default!,
        List<double>? stops = null,
        TileMode tileMode = TileMode.clamp,
        GradientTransform? transform = null
    )
        : base(colors: colors, stops: stops, transform: transform)
    {
        AlignmentGeometry __begin = begin ?? Alignment.centerLeft;
        AlignmentGeometry __end = end ?? Alignment.centerRight;
        this.begin = __begin;
        this.end = __end;
        this.tileMode = tileMode;
    }

    public override Shader createShader(Rect rect, TextDirection? textDirection = null)
    {
        return Ui.Gradient.linear(
            begin.resolve(textDirection).withinRect(rect),
            end.resolve(textDirection).withinRect(rect),
            colors,
            _impliedStops(),
            tileMode,
            _resolveTransform(rect, textDirection)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearGradient scale(double factor)
    {
        return new LinearGradient(
            begin: begin,
            end: end,
            colors: colors.map((color) => Dart_uiLibrary.Color.lerp(null, color, factor)!).ToList(),
            stops: stops,
            tileMode: tileMode,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearGradient fromColor(Color color)
    {
        return new LinearGradient(
            begin: begin,
            end: end,
            colors: new List<Color>(
                Enumerable.Repeat(color, checked((int)checked((long)colors.Count)))
            ),
            stops: stops,
            tileMode: tileMode,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpFrom(Gradient? a, double t)
    {
        if (a is LinearGradient)
        {
            LinearGradient a__as18648 = (LinearGradient)a;
            return lerp((LinearGradient?)a__as18648, this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpTo(Gradient? b, double t)
    {
        if (b is LinearGradient)
        {
            LinearGradient b__as18826 = (LinearGradient)b;
            return lerp(this, (LinearGradient?)b__as18826, t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static LinearGradient? lerp(LinearGradient? a, LinearGradient? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.scale(t);
        }
        if (b is null)
        {
            return a.scale(1.0 - t);
        }
        _ColorsAndStops__gradient interpolated = GradientLibrary._interpolateColorsAndStops(
            a.colors,
            a._impliedStops(),
            b.colors,
            b._impliedStops(),
            t
        );
        return new LinearGradient(
            begin: AlignmentGeometry.lerp(a.begin, b.begin, t)!,
            end: AlignmentGeometry.lerp(a.end, b.end, t)!,
            colors: interpolated.colors,
            stops: interpolated.stops,
            tileMode: (t < 0.5) ? a.tileMode : b.tileMode,
            transform: (t < 0.5) ? a.transform : b.transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as LinearGradient;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is LinearGradient)
            && Equals(__other.begin, begin)
            && Equals(__other.end, end)
            && Equals(__other.tileMode, tileMode)
            && Equals(__other.transform, transform)
            && CollectionsLibrary.listEquals(__other.colors, colors)
            && CollectionsLibrary.listEquals(__other.stops, stops);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            begin,
            end,
            tileMode,
            transform,
            FoundationRuntimePorts.ObjectHashAll(colors),
            (stops is null) ? null : FoundationRuntimePorts.ObjectHashAll(stops!)
        );

    public override string ToString()
    {
        var description = new List<string>
        {
            $"begin: {begin}",
            $"end: {end}",
            $"colors: {colors}",
            $"tileMode: {tileMode}",
        };
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearGradient")}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearGradient withOpacity(double opacity)
    {
        return new LinearGradient(
            begin: begin,
            end: end,
            colors: new List<Color>(),
            stops: stops,
            tileMode: tileMode,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RadialGradient : Gradient
{
    public virtual AlignmentGeometry center { get; private set; } = default!;
    public virtual double radius { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;
    public virtual AlignmentGeometry? focal { get; private set; }
    public virtual double focalRadius { get; private set; } = default!;

    public RadialGradient(
        AlignmentGeometry center = default!,
        double radius = 0.5,
        List<Color> colors = default!,
        List<double>? stops = null,
        TileMode tileMode = TileMode.clamp,
        AlignmentGeometry? focal = null,
        double focalRadius = 0.0,
        GradientTransform? transform = null
    )
        : base(colors: colors, stops: stops, transform: transform)
    {
        AlignmentGeometry __center = center ?? Alignment.center;
        this.center = __center;
        this.radius = radius;
        this.tileMode = tileMode;
        this.focal = focal;
        this.focalRadius = focalRadius;
    }

    public override Shader createShader(Rect rect, TextDirection? textDirection = null)
    {
        return Ui.Gradient.radial(
            center.resolve(textDirection).withinRect(rect),
            radius * rect.shortestSide,
            colors,
            _impliedStops(),
            tileMode,
            _resolveTransform(rect, textDirection),
            focal?.resolve(textDirection).withinRect(rect),
            focalRadius * rect.shortestSide
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RadialGradient scale(double factor)
    {
        return new RadialGradient(
            center: center,
            radius: radius,
            colors: colors.map((color) => Dart_uiLibrary.Color.lerp(null, color, factor)!).ToList(),
            stops: stops,
            tileMode: tileMode,
            focal: focal,
            focalRadius: focalRadius,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RadialGradient fromColor(Color color)
    {
        return new RadialGradient(
            center: center,
            radius: radius,
            colors: new List<Color>(
                Enumerable.Repeat(color, checked((int)checked((long)colors.Count)))
            ),
            stops: stops,
            tileMode: tileMode,
            focal: focal,
            focalRadius: focalRadius,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpFrom(Gradient? a, double t)
    {
        if (a is RadialGradient)
        {
            RadialGradient a__as30225 = (RadialGradient)a;
            return lerp((RadialGradient?)a__as30225, this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpTo(Gradient? b, double t)
    {
        if (b is RadialGradient)
        {
            RadialGradient b__as30403 = (RadialGradient)b;
            return lerp(this, (RadialGradient?)b__as30403, t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static RadialGradient? lerp(RadialGradient? a, RadialGradient? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.scale(t);
        }
        if (b is null)
        {
            return a.scale(1.0 - t);
        }
        _ColorsAndStops__gradient interpolated = GradientLibrary._interpolateColorsAndStops(
            a.colors,
            a._impliedStops(),
            b.colors,
            b._impliedStops(),
            t
        );
        return new RadialGradient(
            center: AlignmentGeometry.lerp(a.center, b.center, t)!,
            radius: Math.Max(
                0.0,
                DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.radius, b.radius, t))
            ),
            colors: interpolated.colors,
            stops: interpolated.stops,
            tileMode: (t < 0.5) ? a.tileMode : b.tileMode,
            focal: AlignmentGeometry.lerp(a.focal, b.focal, t),
            focalRadius: Math.Max(
                0.0,
                DartRuntimePrimitives.RequireValue(
                    Dart_uiLibrary.lerpDouble(a.focalRadius, b.focalRadius, t)
                )
            ),
            transform: (t < 0.5) ? a.transform : b.transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as RadialGradient;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is RadialGradient)
            && Equals(__other.center, center)
            && (__other.radius == radius)
            && Equals(__other.tileMode, tileMode)
            && Equals(__other.transform, transform)
            && CollectionsLibrary.listEquals(__other.colors, colors)
            && CollectionsLibrary.listEquals(__other.stops, stops)
            && Equals(__other.focal, focal)
            && (__other.focalRadius == focalRadius);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            center,
            radius,
            tileMode,
            transform,
            FoundationRuntimePorts.ObjectHashAll(colors),
            (stops is null) ? null : FoundationRuntimePorts.ObjectHashAll(stops!),
            focal,
            focalRadius
        );

    public override string ToString()
    {
        var description = new List<string>
        {
            $"center: {center}",
            $"radius: {Foundation.DebugLibrary.debugFormatDouble(radius)}",
            $"colors: {colors}",
            $"tileMode: {tileMode}",
            $"focalRadius: {Foundation.DebugLibrary.debugFormatDouble(focalRadius)}",
        };
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RadialGradient")}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RadialGradient withOpacity(double opacity)
    {
        return new RadialGradient(
            center: center,
            radius: radius,
            colors: new List<Color>(),
            stops: stops,
            tileMode: tileMode,
            focal: focal,
            focalRadius: focalRadius,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SweepGradient : Gradient
{
    public virtual AlignmentGeometry center { get; private set; } = default!;
    public virtual double startAngle { get; private set; } = default!;
    public virtual double endAngle { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;

    public SweepGradient(
        AlignmentGeometry center = default!,
        double startAngle = 0.0,
        double? endAngle = null,
        List<Color> colors = default!,
        List<double>? stops = null,
        TileMode tileMode = TileMode.clamp,
        GradientTransform? transform = null
    )
        : base(colors: colors, stops: stops, transform: transform)
    {
        AlignmentGeometry __center = center ?? Alignment.center;
        double __endAngle = endAngle ?? (Dart_mathLibrary.pi * 2);
        this.center = __center;
        this.startAngle = startAngle;
        this.endAngle = __endAngle;
        this.tileMode = tileMode;
    }

    public override Shader createShader(Rect rect, TextDirection? textDirection = null)
    {
        return Ui.Gradient.sweep(
            center.resolve(textDirection).withinRect(rect),
            colors,
            _impliedStops(),
            tileMode,
            startAngle,
            DartRuntimePrimitives.RequireValue(endAngle),
            _resolveTransform(rect, textDirection)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SweepGradient scale(double factor)
    {
        return new SweepGradient(
            center: center,
            startAngle: startAngle,
            endAngle: DartRuntimePrimitives.RequireValue(endAngle),
            colors: colors.map((color) => Dart_uiLibrary.Color.lerp(null, color, factor)!).ToList(),
            stops: stops,
            tileMode: tileMode,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SweepGradient fromColor(Color color)
    {
        return new SweepGradient(
            center: center,
            startAngle: startAngle,
            endAngle: DartRuntimePrimitives.RequireValue(endAngle),
            colors: new List<Color>(
                Enumerable.Repeat(color, checked((int)checked((long)colors.Count)))
            ),
            stops: stops,
            tileMode: tileMode,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpFrom(Gradient? a, double t)
    {
        if (a is SweepGradient)
        {
            SweepGradient a__as41442 = (SweepGradient)a;
            return lerp((SweepGradient?)a__as41442, this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpTo(Gradient? b, double t)
    {
        if (b is SweepGradient)
        {
            SweepGradient b__as41618 = (SweepGradient)b;
            return lerp(this, (SweepGradient?)b__as41618, t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SweepGradient? lerp(SweepGradient? a, SweepGradient? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.scale(t);
        }
        if (b is null)
        {
            return a.scale(1.0 - t);
        }
        _ColorsAndStops__gradient interpolated = GradientLibrary._interpolateColorsAndStops(
            a.colors,
            a._impliedStops(),
            b.colors,
            b._impliedStops(),
            t
        );
        return new SweepGradient(
            center: AlignmentGeometry.lerp(a.center, b.center, t)!,
            startAngle: Math.Max(
                0.0,
                DartRuntimePrimitives.RequireValue(
                    Dart_uiLibrary.lerpDouble(a.startAngle, b.startAngle, t)
                )
            ),
            endAngle: Math.Max(
                0.0,
                DartRuntimePrimitives.RequireValue(
                    Dart_uiLibrary.lerpDouble(
                        DartRuntimePrimitives.RequireValue(a.endAngle),
                        DartRuntimePrimitives.RequireValue(b.endAngle),
                        t
                    )
                )
            ),
            colors: interpolated.colors,
            stops: interpolated.stops,
            tileMode: (t < 0.5) ? a.tileMode : b.tileMode,
            transform: (t < 0.5) ? a.transform : b.transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SweepGradient;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SweepGradient)
            && Equals(__other.center, center)
            && (__other.startAngle == startAngle)
            && (__other.endAngle == endAngle)
            && Equals(__other.tileMode, tileMode)
            && Equals(__other.transform, transform)
            && CollectionsLibrary.listEquals(__other.colors, colors)
            && CollectionsLibrary.listEquals(__other.stops, stops);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            center,
            startAngle,
            DartRuntimePrimitives.RequireValue(endAngle),
            tileMode,
            transform,
            FoundationRuntimePorts.ObjectHashAll(colors),
            (stops is null) ? null : FoundationRuntimePorts.ObjectHashAll(stops!)
        );

    public override string ToString()
    {
        var description = new List<string>
        {
            $"center: {center}",
            $"startAngle: {Foundation.DebugLibrary.debugFormatDouble(startAngle)}",
            $"endAngle: {Foundation.DebugLibrary.debugFormatDouble(DartRuntimePrimitives.RequireValue(endAngle))}",
            $"colors: {colors}",
            $"tileMode: {tileMode}",
        };
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SweepGradient")}({string.Join(", ", description)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SweepGradient withOpacity(double opacity)
    {
        return new SweepGradient(
            center: center,
            startAngle: startAngle,
            endAngle: DartRuntimePrimitives.RequireValue(endAngle),
            colors: new List<Color>(),
            stops: stops,
            tileMode: tileMode,
            transform: transform
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

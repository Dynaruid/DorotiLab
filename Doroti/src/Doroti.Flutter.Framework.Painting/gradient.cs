// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/gradient.dart
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

namespace Doroti.Generated.Framework.Painting;

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
        DartRuntimePrimitives.Assert(() => (checked((long)(colors.Count)) != 0));
        DartRuntimePrimitives.Assert(() => (checked((long)(stops.Count)) != 0));
        if ((t <= stops.First()))
        {
            return colors.First();
        }
        if ((t >= stops.Last()))
        {
            return colors.Last();
        }
        long index__975 = stops.lastIndexWhere(((s) => (s <= t)));
        DartRuntimePrimitives.Assert(() => (index__975 != -1L));
        return Dart_uiLibrary.Color.lerp(colors[(int)(index__975)], colors[(int)((index__975 + 1L))], (((t - stops[(int)(index__975)])) / ((stops[(int)((index__975 + 1L))] - stops[(int)(index__975)]))))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class GradientLibrary
{
    internal static _ColorsAndStops__gradient _interpolateColorsAndStops(List<Color> aColors, List<double> aStops, List<Color> bColors, List<double> bStops, double t)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(aColors.Count)) >= 2L));
        DartRuntimePrimitives.Assert(() => (checked((long)(bColors.Count)) >= 2L));
        DartRuntimePrimitives.Assert(() => (checked((long)(aStops.Count)) == checked((long)(aColors.Count))));
        DartRuntimePrimitives.Assert(() => (checked((long)(bStops.Count)) == checked((long)(bColors.Count))));
        var stops__1490 = ((Func<SortedSet<double>>)(() =>
{
    var __cascade = new SortedSet<double>();
    __cascade.UnionWith(aStops);
    __cascade.UnionWith(bStops);
    return __cascade;
}))();
        List<double> interpolatedStops__1585 = stops__1490.ToList();
        List<global::Doroti.Flutter.Ui.Color> interpolatedColors__1656 = interpolatedStops__1585.map<double, Color>(((stop) => Dart_uiLibrary.Color.lerp(GradientLibrary._sample(aColors, aStops, stop), GradientLibrary._sample(bColors, bStops, stop), t)!)).ToList();
        return new _ColorsAndStops__gradient(interpolatedColors__1656, interpolatedStops__1585);
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
        double sinRadians__3710 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.sin(this.radians);
        double oneMinusCosRadians__3759 = (1L - global::Doroti.Flutter.Runtime.Dart_mathLibrary.cos(this.radians));
        global::Doroti.Flutter.Ui.Offset center__3820 = bounds.center;
        double originX__3861 = ((sinRadians__3710 * center__3820.dy) + (oneMinusCosRadians__3759 * center__3820.dx));
        double originY__3945 = ((-sinRadians__3710 * center__3820.dx) + (oneMinusCosRadians__3759 * center__3820.dy));
        return ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.translateByDouble(originX__3861, originY__3945, 0, 1);
    __cascade.rotateZ(this.radians);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as GradientRotation;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is GradientRotation) && (((GradientRotation)((GradientRotation)__other)).radians == this.radians));
    }

    public override int GetHashCode() => this.radians.GetHashCode();
    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "GradientRotation"))}(radians: {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.radians))})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class Gradient
{
    public virtual List<Color> colors { get; private set; } = default!;
    public virtual List<double>? stops { get; private set; }
    public virtual GradientTransform? transform { get; private set; }

    protected Gradient(List<Color> colors, List<double>? stops = null, GradientTransform? transform = null)
    {
        this.colors = colors;
        this.stops = stops;
        this.transform = transform;
    }

    internal virtual List<double> _impliedStops()
    {
        if ((this.stops is not null))
        {
            return this.stops!;
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this.colors.Count)) >= 2L));
        double separation__7310 = (1.0 / ((checked((long)(this.colors.Count)) - 1L)));
        return new List<double>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(this.colors.Count)))), ((index) => (index * separation__7310))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract global::Doroti.Flutter.Ui.Shader createShader(Rect rect, TextDirection? textDirection = null);
    public abstract Gradient scale(double factor);
    public abstract Gradient withOpacity(double opacity);
    public virtual Gradient fromColor(Color color)
    {
        return new LinearGradient(colors: new List<global::Doroti.Flutter.Ui.Color>(System.Linq.Enumerable.Repeat<global::Doroti.Flutter.Ui.Color>(color, checked((int)checked((long)(this.colors.Count))))), stops: this.stops, transform: this.transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Gradient? lerpFrom(Gradient? a, double t)
    {
        if ((a is null))
        {
            return scale(t);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Gradient? lerpTo(Gradient? b, double t)
    {
        if ((b is null))
        {
            return scale((1.0 - t));
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
        Gradient? result__12598 = default!;
        if ((b is not null))
        {
            result__12598 = b.lerpFrom(a, t);
        }
        if (((result__12598 is null) && (a is not null)))
        {
            result__12598 = a.lerpTo(b, t);
        }
        if ((result__12598 is not null))
        {
            return result__12598;
        }
        DartRuntimePrimitives.Assert(() => ((a is not null) && (b is not null)));
        return ((t < 0.5) ? a!.scale((1.0 - ((t * 2.0)))) : b!.scale((((t - 0.5)) * 2.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Float64List? _resolveTransform(Rect bounds, TextDirection? textDirection)
    {
        return this.transform?.transform(bounds, textDirection: textDirection)?.storage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class LinearGradient : Gradient
{
    public virtual AlignmentGeometry begin { get; private set; } = default!;
    public virtual AlignmentGeometry end { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;

    public LinearGradient(AlignmentGeometry begin = default!, AlignmentGeometry end = default!, List<Color> colors = default!, List<double>? stops = null, TileMode tileMode = TileMode.clamp, GradientTransform? transform = null) : base(colors: colors, stops: stops, transform: transform)
    {
        AlignmentGeometry __begin = begin ?? Alignment.centerLeft;
        AlignmentGeometry __end = end ?? Alignment.centerRight;
        this.begin = __begin;
        this.end = __end;
        this.tileMode = tileMode;
    }

    public override Shader createShader(Rect rect, TextDirection? textDirection = null)
    {
        return global::Doroti.Flutter.Ui.Gradient.linear(this.begin.resolve(textDirection).withinRect(rect), this.end.resolve(textDirection).withinRect(rect), colors, _impliedStops(), this.tileMode, _resolveTransform(rect, textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearGradient scale(double factor)
    {
        return new LinearGradient(begin: this.begin, end: this.end, colors: colors.map<Color, Color>(((color) => Dart_uiLibrary.Color.lerp(null, color, factor)!)).ToList(), stops: stops, tileMode: this.tileMode, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearGradient fromColor(Color color)
    {
        return new LinearGradient(begin: this.begin, end: this.end, colors: new List<global::Doroti.Flutter.Ui.Color>(System.Linq.Enumerable.Repeat<global::Doroti.Flutter.Ui.Color>(color, checked((int)checked((long)(colors.Count))))), stops: stops, tileMode: this.tileMode, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpFrom(Gradient? a, double t)
    {
        if ((a is LinearGradient))
        {
            LinearGradient a__as18648 = (LinearGradient)a;
            return LinearGradient.lerp(((LinearGradient?)a__as18648), this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpTo(Gradient? b, double t)
    {
        if ((b is LinearGradient))
        {
            LinearGradient b__as18826 = (LinearGradient)b;
            return LinearGradient.lerp(this, ((LinearGradient?)b__as18826), t);
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
        if ((a is null))
        {
            return b!.scale(t);
        }
        if ((b is null))
        {
            return a.scale((1.0 - t));
        }
        _ColorsAndStops__gradient interpolated__20315 = GradientLibrary._interpolateColorsAndStops(a.colors, a._impliedStops(), b.colors, b._impliedStops(), t);
        return new LinearGradient(begin: AlignmentGeometry.lerp(((LinearGradient)a).begin, ((LinearGradient)b).begin, t)!, end: AlignmentGeometry.lerp(((LinearGradient)a).end, ((LinearGradient)b).end, t)!, colors: ((_ColorsAndStops__gradient)interpolated__20315).colors, stops: ((_ColorsAndStops__gradient)interpolated__20315).stops, tileMode: ((t < 0.5) ? ((LinearGradient)a).tileMode : ((LinearGradient)b).tileMode), transform: ((t < 0.5) ? a.transform : b.transform));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as LinearGradient;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((__other is LinearGradient) && (object.Equals(((LinearGradient)((LinearGradient)__other)).begin, this.begin))) && (object.Equals(((LinearGradient)((LinearGradient)__other)).end, this.end))) && (object.Equals(((LinearGradient)((LinearGradient)__other)).tileMode, this.tileMode))) && (object.Equals(((LinearGradient)__other).transform, transform))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Flutter.Ui.Color>(((LinearGradient)__other).colors, colors)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<double>(((LinearGradient)__other).stops, stops));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.begin, this.end, this.tileMode, transform, FoundationRuntimePorts.ObjectHashAll(colors), ((stops is null) ? null : FoundationRuntimePorts.ObjectHashAll(stops!)));
    public override string ToString()
    {
        var description__21498 = new List<string> { $"begin: {this.begin}", $"end: {this.end}", $"colors: {colors}", $"tileMode: {this.tileMode}" };
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearGradient"))}({string.Join(", ", description__21498)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearGradient withOpacity(double opacity)
    {
        return new LinearGradient(begin: this.begin, end: this.end, colors: new List<global::Doroti.Flutter.Ui.Color>(), stops: stops, tileMode: this.tileMode, transform: transform);
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

    public RadialGradient(AlignmentGeometry center = default!, double radius = 0.5, List<Color> colors = default!, List<double>? stops = null, TileMode tileMode = TileMode.clamp, AlignmentGeometry? focal = null, double focalRadius = 0.0, GradientTransform? transform = null) : base(colors: colors, stops: stops, transform: transform)
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
        return global::Doroti.Flutter.Ui.Gradient.radial(this.center.resolve(textDirection).withinRect(rect), (this.radius * rect.shortestSide), colors, _impliedStops(), this.tileMode, _resolveTransform(rect, textDirection), this.focal?.resolve(textDirection).withinRect(rect), (this.focalRadius * rect.shortestSide));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RadialGradient scale(double factor)
    {
        return new RadialGradient(center: this.center, radius: this.radius, colors: colors.map<Color, Color>(((color) => Dart_uiLibrary.Color.lerp(null, color, factor)!)).ToList(), stops: stops, tileMode: this.tileMode, focal: this.focal, focalRadius: this.focalRadius, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RadialGradient fromColor(Color color)
    {
        return new RadialGradient(center: this.center, radius: this.radius, colors: new List<global::Doroti.Flutter.Ui.Color>(System.Linq.Enumerable.Repeat<global::Doroti.Flutter.Ui.Color>(color, checked((int)checked((long)(colors.Count))))), stops: stops, tileMode: this.tileMode, focal: this.focal, focalRadius: this.focalRadius, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpFrom(Gradient? a, double t)
    {
        if ((a is RadialGradient))
        {
            RadialGradient a__as30225 = (RadialGradient)a;
            return RadialGradient.lerp(((RadialGradient?)a__as30225), this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpTo(Gradient? b, double t)
    {
        if ((b is RadialGradient))
        {
            RadialGradient b__as30403 = (RadialGradient)b;
            return RadialGradient.lerp(this, ((RadialGradient?)b__as30403), t);
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
        if ((a is null))
        {
            return b!.scale(t);
        }
        if ((b is null))
        {
            return a.scale((1.0 - t));
        }
        _ColorsAndStops__gradient interpolated__31896 = GradientLibrary._interpolateColorsAndStops(a.colors, a._impliedStops(), b.colors, b._impliedStops(), t);
        return new RadialGradient(center: AlignmentGeometry.lerp(((RadialGradient)a).center, ((RadialGradient)b).center, t)!, radius: Math.Max(0.0, DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RadialGradient)a).radius, ((RadialGradient)b).radius, t))), colors: ((_ColorsAndStops__gradient)interpolated__31896).colors, stops: ((_ColorsAndStops__gradient)interpolated__31896).stops, tileMode: ((t < 0.5) ? ((RadialGradient)a).tileMode : ((RadialGradient)b).tileMode), focal: AlignmentGeometry.lerp(((RadialGradient)a).focal, ((RadialGradient)b).focal, t), focalRadius: Math.Max(0.0, DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((RadialGradient)a).focalRadius, ((RadialGradient)b).focalRadius, t))), transform: ((t < 0.5) ? a.transform : b.transform));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as RadialGradient;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((__other is RadialGradient) && (object.Equals(((RadialGradient)((RadialGradient)__other)).center, this.center))) && (((RadialGradient)((RadialGradient)__other)).radius == this.radius)) && (object.Equals(((RadialGradient)((RadialGradient)__other)).tileMode, this.tileMode))) && (object.Equals(((RadialGradient)__other).transform, transform))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Flutter.Ui.Color>(((RadialGradient)__other).colors, colors)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<double>(((RadialGradient)__other).stops, stops)) && (object.Equals(((RadialGradient)((RadialGradient)__other)).focal, this.focal))) && (((RadialGradient)((RadialGradient)__other)).focalRadius == this.focalRadius));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.center, this.radius, this.tileMode, transform, FoundationRuntimePorts.ObjectHashAll(colors), ((stops is null) ? null : FoundationRuntimePorts.ObjectHashAll(stops!)), this.focal, this.focalRadius);
    public override string ToString()
    {
        var description__33354 = new List<string> { $"center: {this.center}", $"radius: {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.radius))}", $"colors: {colors}", $"tileMode: {this.tileMode}", $"focalRadius: {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.focalRadius))}" };
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RadialGradient"))}({string.Join(", ", description__33354)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RadialGradient withOpacity(double opacity)
    {
        return new RadialGradient(center: this.center, radius: this.radius, colors: new List<global::Doroti.Flutter.Ui.Color>(), stops: stops, tileMode: this.tileMode, focal: this.focal, focalRadius: this.focalRadius, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SweepGradient : Gradient
{
    public virtual AlignmentGeometry center { get; private set; } = default!;
    public virtual double startAngle { get; private set; } = default!;
    public virtual double endAngle { get; private set; } = default!;
    public virtual TileMode tileMode { get; private set; } = default!;

    public SweepGradient(AlignmentGeometry center = default!, double startAngle = 0.0, double? endAngle = null, List<Color> colors = default!, List<double>? stops = null, TileMode tileMode = TileMode.clamp, GradientTransform? transform = null) : base(colors: colors, stops: stops, transform: transform)
    {
        AlignmentGeometry __center = center ?? Alignment.center;
        double __endAngle = endAngle ?? Dart_mathLibrary.pi * 2;
        this.center = __center;
        this.startAngle = startAngle;
        this.endAngle = __endAngle;
        this.tileMode = tileMode;
    }

    public override Shader createShader(Rect rect, TextDirection? textDirection = null)
    {
        return global::Doroti.Flutter.Ui.Gradient.sweep(this.center.resolve(textDirection).withinRect(rect), colors, _impliedStops(), this.tileMode, this.startAngle, DartRuntimePrimitives.RequireValue(this.endAngle), _resolveTransform(rect, textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SweepGradient scale(double factor)
    {
        return new SweepGradient(center: this.center, startAngle: this.startAngle, endAngle: DartRuntimePrimitives.RequireValue(this.endAngle), colors: colors.map<Color, Color>(((color) => Dart_uiLibrary.Color.lerp(null, color, factor)!)).ToList(), stops: stops, tileMode: this.tileMode, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SweepGradient fromColor(Color color)
    {
        return new SweepGradient(center: this.center, startAngle: this.startAngle, endAngle: DartRuntimePrimitives.RequireValue(this.endAngle), colors: new List<global::Doroti.Flutter.Ui.Color>(System.Linq.Enumerable.Repeat<global::Doroti.Flutter.Ui.Color>(color, checked((int)checked((long)(colors.Count))))), stops: stops, tileMode: this.tileMode, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpFrom(Gradient? a, double t)
    {
        if ((a is SweepGradient))
        {
            SweepGradient a__as41442 = (SweepGradient)a;
            return SweepGradient.lerp(((SweepGradient?)a__as41442), this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gradient? lerpTo(Gradient? b, double t)
    {
        if ((b is SweepGradient))
        {
            SweepGradient b__as41618 = (SweepGradient)b;
            return SweepGradient.lerp(this, ((SweepGradient?)b__as41618), t);
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
        if ((a is null))
        {
            return b!.scale(t);
        }
        if ((b is null))
        {
            return a.scale((1.0 - t));
        }
        _ColorsAndStops__gradient interpolated__43011 = GradientLibrary._interpolateColorsAndStops(a.colors, a._impliedStops(), b.colors, b._impliedStops(), t);
        return new SweepGradient(center: AlignmentGeometry.lerp(((SweepGradient)a).center, ((SweepGradient)b).center, t)!, startAngle: Math.Max(0.0, DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((SweepGradient)a).startAngle, ((SweepGradient)b).startAngle, t))), endAngle: Math.Max(0.0, DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(((SweepGradient)a).endAngle), DartRuntimePrimitives.RequireValue(((SweepGradient)b).endAngle), t))), colors: ((_ColorsAndStops__gradient)interpolated__43011).colors, stops: ((_ColorsAndStops__gradient)interpolated__43011).stops, tileMode: ((t < 0.5) ? ((SweepGradient)a).tileMode : ((SweepGradient)b).tileMode), transform: ((t < 0.5) ? a.transform : b.transform));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as SweepGradient;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((__other is SweepGradient) && (object.Equals(((SweepGradient)((SweepGradient)__other)).center, this.center))) && (((SweepGradient)((SweepGradient)__other)).startAngle == this.startAngle)) && (((SweepGradient)((SweepGradient)__other)).endAngle == this.endAngle)) && (object.Equals(((SweepGradient)((SweepGradient)__other)).tileMode, this.tileMode))) && (object.Equals(((SweepGradient)__other).transform, transform))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<global::Doroti.Flutter.Ui.Color>(((SweepGradient)__other).colors, colors)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<double>(((SweepGradient)__other).stops, stops));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.center, this.startAngle, DartRuntimePrimitives.RequireValue(this.endAngle), this.tileMode, transform, FoundationRuntimePorts.ObjectHashAll(colors), ((stops is null) ? null : FoundationRuntimePorts.ObjectHashAll(stops!)));
    public override string ToString()
    {
        var description__44372 = new List<string> { $"center: {this.center}", $"startAngle: {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this.startAngle))}", $"endAngle: {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(DartRuntimePrimitives.RequireValue(this.endAngle)))}", $"colors: {colors}", $"tileMode: {this.tileMode}" };
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SweepGradient"))}({string.Join(", ", description__44372)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SweepGradient withOpacity(double opacity)
    {
        return new SweepGradient(center: this.center, startAngle: this.startAngle, endAngle: DartRuntimePrimitives.RequireValue(this.endAngle), colors: new List<global::Doroti.Flutter.Ui.Color>(), stops: stops, tileMode: this.tileMode, transform: transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


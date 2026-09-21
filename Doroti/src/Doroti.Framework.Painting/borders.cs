// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/borders.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public enum BorderStyle
{
    none,
    solid,
}

public class BorderSide : Diagnosticable
{
    public virtual Color color { get; private set; } = default!;
    public virtual double width { get; private set; } = default!;
    public virtual BorderStyle style { get; private set; } = default!;
    public static BorderSide none = new BorderSide(width: 0.0, style: BorderStyle.none);
    public virtual double strokeAlign { get; private set; } = default!;
    public const double strokeAlignInside = -1.0;
    public const double strokeAlignCenter = 0.0;
    public const double strokeAlignOutside = 1.0;

    public BorderSide(
        Color color = default!,
        double width = 1.0,
        BorderStyle style = BorderStyle.solid,
        double? strokeAlign = null
    )
    {
        Color __color = color ?? new Color(0xFF000000);
        double __strokeAlign = strokeAlign ?? strokeAlignInside;
        this.color = __color;
        this.width = width;
        this.style = style;
        this.strokeAlign = __strokeAlign;
        System.Diagnostics.Debug.Assert(width >= 0.0);
    }

    public static BorderSide merge(BorderSide a, BorderSide b)
    {
        DartRuntimePrimitives.Assert(() => canMerge(a, b));
        bool aIsNone = Equals(a.style, BorderStyle.none) && (a.width == 0.0);
        bool bIsNone = Equals(b.style, BorderStyle.none) && (b.width == 0.0);
        if (aIsNone && bIsNone)
        {
            return none;
        }
        if (aIsNone)
        {
            return b;
        }
        if (bIsNone)
        {
            return a;
        }
        DartRuntimePrimitives.Assert(() => Equals(a.color, b.color));
        DartRuntimePrimitives.Assert(() => Equals(a.style, b.style));
        return new BorderSide(
            color: a.color,
            width: a.width + b.width,
            strokeAlign: Math.Max((a.strokeAlign), (b.strokeAlign)),
            style: a.style
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual BorderSide copyWith(
        Color? color = null,
        double? width = null,
        BorderStyle? style = null,
        double? strokeAlign = null
    )
    {
        return new BorderSide(
            color: color ?? this.color,
            width: width ?? this.width,
            style: style ?? this.style,
            strokeAlign: strokeAlign ?? this.strokeAlign
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual BorderSide scale(double t)
    {
        return new BorderSide(
            color: color,
            width: Math.Max(0.0, width * t),
            style: (t <= 0.0) ? BorderStyle.none : style
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Paint toPaint()
    {
        switch (style)
        {
            case BorderStyle.solid:
            {
                return (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = color;
                            __cascade.strokeWidth = width;
                            __cascade.style = PaintingStyle.stroke;
                            return __cascade;
                        }
                    )
                )();
            }
            case BorderStyle.none:
            {
                return (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = new Color(0L);
                            __cascade.strokeWidth = 0.0;
                            __cascade.style = PaintingStyle.stroke;
                            return __cascade;
                        }
                    )
                )();
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static bool canMerge(BorderSide a, BorderSide b)
    {
        if (
            (Equals(a.style, BorderStyle.none) && (a.width == 0.0))
            || (Equals(b.style, BorderStyle.none) && (b.width == 0.0))
        )
        {
            return true;
        }
        return Equals(a.style, b.style) && Equals(a.color, b.color);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static BorderSide lerp(BorderSide a, BorderSide b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (t == 0.0)
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        double widthLocal = (
            DorotiUiLibrary.lerpDouble(a.width, b.width, t)
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        if ((widthLocal) < 0.0)
        {
            return none;
        }
        if (Equals(a.style, b.style) && (a.strokeAlign == b.strokeAlign))
        {
            return new BorderSide(
                color: DorotiUiLibrary.Color.lerp(a.color, b.color, t)!,
                width: (widthLocal),
                style: a.style,
                strokeAlign: (a.strokeAlign)
            );
        }
        Color colorA = a.style switch
        {
            BorderStyle.solid => a.color,
            BorderStyle.none => a.color.withAlpha(0L),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        Color colorB = b.style switch
        {
            BorderStyle.solid => b.color,
            BorderStyle.none => b.color.withAlpha(0L),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        if (a.strokeAlign != b.strokeAlign)
        {
            return new BorderSide(
                color: DorotiUiLibrary.Color.lerp(colorA, colorB, t)!,
                width: (widthLocal),
                strokeAlign: (
                    DorotiUiLibrary.lerpDouble((a.strokeAlign), (b.strokeAlign), t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        return new BorderSide(
            color: DorotiUiLibrary.Color.lerp(colorA, colorB, t)!,
            width: (widthLocal),
            strokeAlign: (a.strokeAlign)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double strokeInset => width * (1L - ((1L + strokeAlign) / 2L));
    public virtual double strokeOutset => width * (1L + strokeAlign) / 2L;
    public virtual double strokeOffset => width * strokeAlign;

    public override bool Equals(object? other)
    {
        var __other = other as BorderSide;
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
        return (__other is BorderSide)
            && Equals(__other.color, color)
            && (__other.width == width)
            && Equals(__other.style, style)
            && (__other.strokeAlign == strokeAlign);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(color, width, style, (strokeAlign));

    public virtual string toStringShort() => "BorderSide";

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<Color>("color", color, defaultValue: new Color(4278190080L))
        );
        properties.add(new DoubleProperty("width", width, defaultValue: 1.0));
        properties.add(
            new DoubleProperty("strokeAlign", (strokeAlign), defaultValue: strokeAlignInside)
        );
        properties.add(
            new EnumProperty<BorderStyle>("style", style, defaultValue: BorderStyle.solid)
        );
    }
}

public abstract class ShapeBorder
{
    protected ShapeBorder() { }

    public abstract EdgeInsetsGeometry dimensions { get; }

    public virtual ShapeBorder? add(ShapeBorder other, bool reversed = false) => null;

    public virtual ShapeBorder op_Add(ShapeBorder other)
    {
        return (add(other) ?? other.add(this, reversed: true))
            ?? new _CompoundBorder__borders(new List<ShapeBorder> { other, this });
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract ShapeBorder scale(double t);

    public virtual ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is null)
        {
            return scale(t);
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is null)
        {
            return scale(1.0 - t);
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ShapeBorder? lerp(ShapeBorder? a, ShapeBorder? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        ShapeBorder? result =
            ((b?.lerpFrom(a, t) ?? a?.lerpTo(b, t)) ?? b?.lerpTo(a, 1.0 - t))
            ?? a?.lerpFrom(b, 1.0 - t);
        return result ?? ((t < 0.5) ? a : b);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract Path getOuterPath(Rect rect, TextDirection? textDirection = null);
    public abstract Path getInnerPath(Rect rect, TextDirection? textDirection = null);

    public virtual bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return getOuterPath(rect, textDirection: textDirection).contains(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        DartRuntimePrimitives.Assert(() => !preferPaintInterior);
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual bool preferPaintInterior => false;

    public virtual void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    ) { }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ShapeBorder")}()";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public abstract class OutlinedBorder : ShapeBorder
{
    public virtual BorderSide side { get; private set; } = default!;

    protected OutlinedBorder(BorderSide side = default!)
    {
        BorderSide __side = side ?? BorderSide.none;
        this.side = __side;
    }

    public override EdgeInsetsGeometry dimensions =>
        EdgeInsets.CreateAll(Math.Max(side.strokeInset, 0));
    public abstract OutlinedBorder copyWith(
        BorderSide? side = null,
        BorderRadiusGeometry? borderRadius = null,
        double? eccentricity = null,
        LinearBorderEdge? start = null,
        LinearBorderEdge? end = null,
        LinearBorderEdge? top = null,
        LinearBorderEdge? bottom = null,
        double? circularity = null,
        double? rectilinearity = null,
        double? points = null,
        double? innerRadiusRatio = null,
        double? pointRounding = null,
        double? valleyRounding = null,
        double? rotation = null,
        double? squash = null
    );
    public abstract override ShapeBorder scale(double t);

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is null)
        {
            return scale(t);
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is null)
        {
            return scale(1.0 - t);
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static OutlinedBorder? lerp(OutlinedBorder? a, OutlinedBorder? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        ShapeBorder? result =
            ((b?.lerpFrom(a, t) ?? a?.lerpTo(b, t)) ?? b?.lerpTo(a, 1.0 - t))
            ?? a?.lerpFrom(b, 1.0 - t);
        return ((OutlinedBorder?)result)! ?? ((t < 0.5) ? a : b);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CompoundBorder__borders : ShapeBorder
{
    public virtual List<ShapeBorder> borders { get; private set; } = default!;

    internal _CompoundBorder__borders(List<ShapeBorder> borders)
    {
        this.borders = borders;
        System.Diagnostics.Debug.Assert(checked(borders.Count) >= 2L);
        System.Diagnostics.Debug.Assert(
            !borders.any((border) => border is _CompoundBorder__borders)
        );
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            return Enumerable.Aggregate(
                borders,
                (EdgeInsetsGeometry)EdgeInsets.zero,
                (previousValue, border) =>
                {
                    return previousValue.add(border.dimensions);
                }
            );
        }
    }

    public override ShapeBorder? add(ShapeBorder other, bool reversed = false)
    {
        if (other is not _CompoundBorder__borders)
        {
            ShapeBorder ours = reversed ? borders.Last() : borders.First();
            ShapeBorder? merged =
                ours.add(other, reversed: reversed) ?? other.add(ours, reversed: !reversed);
            if (merged is not null)
            {
                var result = new List<ShapeBorder>();
                result[(int)(reversed ? (checked(result.Count) - 1L) : 0L)] = merged;
                return new _CompoundBorder__borders(result);
            }
        }
        var mergedBorders = new List<ShapeBorder>();
        return new _CompoundBorder__borders(mergedBorders);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder scale(double t)
    {
        return new _CompoundBorder__borders(borders.map((border) => border.scale(t)).ToList());
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        return lerp(a, this, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        return lerp(this, b, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static new _CompoundBorder__borders lerp(ShapeBorder? a, ShapeBorder? b, double t)
    {
        DartRuntimePrimitives.Assert(() =>
            (a is _CompoundBorder__borders) || (b is _CompoundBorder__borders)
        );
        IReadOnlyList<ShapeBorder?> aList = a is _CompoundBorder__borders compoundA
            ? (IReadOnlyList<ShapeBorder?>)compoundA.borders
            : [a];
        IReadOnlyList<ShapeBorder?> bList = b is _CompoundBorder__borders compoundB
            ? (IReadOnlyList<ShapeBorder?>)compoundB.borders
            : [b];
        var results = new List<ShapeBorder>();
        long length = Math.Max(checked(aList.Count), checked((long)bList.Count));
        for (var index = 0L; index < length; index += 1L)
        {
            ShapeBorder? localA = (index < checked(aList.Count)) ? aList[(int)index] : null;
            ShapeBorder? localB = (index < checked(bList.Count)) ? bList[(int)index] : null;
            if ((localA is not null) && (localB is not null))
            {
                ShapeBorder? localResult = localA.lerpTo(localB, t) ?? localB.lerpFrom(localA, t);
                if (localResult is not null)
                {
                    results.Add(localResult);
                    continue;
                }
            }
            if (localB is not null)
            {
                results.Add(localB.scale(t));
            }
            if (localA is not null)
            {
                results.Add(localA.scale(1.0 - t));
            }
        }
        return new _CompoundBorder__borders(results);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        for (var index = 0L; index < (checked(borders.Count) - 1L); index += 1L)
        {
            rect = borders[(int)index].dimensions.resolve(textDirection).deflateRect(rect);
        }
        return borders.Last().getInnerPath(rect, textDirection: textDirection);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return borders.First().getOuterPath(rect, textDirection: textDirection);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return borders.First().hitTest(rect, position, textDirection: textDirection);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        borders.First().paintInterior(canvas, rect, paint, textDirection: textDirection);
    }

    public override bool preferPaintInterior => borders.All((border) => border.preferPaintInterior);

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        foreach (ShapeBorder border in borders)
        {
            border.paint(canvas, rect, textDirection: textDirection);
            rect = border.dimensions.resolve(textDirection).deflateRect(rect);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _CompoundBorder__borders;
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
        return (__other is _CompoundBorder__borders)
            && CollectionsLibrary.listEquals(__other.borders, borders);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHashAll(borders);

    public override string ToString()
    {
        return string.Join(" + ", Enumerable.Reverse(borders).map((border) => border.ToString()));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class BordersLibrary
{
    public static void paintBorder(
        Canvas canvas,
        Rect rect,
        BorderSide top = default!,
        BorderSide right = default!,
        BorderSide bottom = default!,
        BorderSide left = default!
    )
    {
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.strokeWidth = 0.0;
                    return __cascade;
                }
            )
        )();
        var path = new Path();
        switch (top.style)
        {
            case BorderStyle.solid:
            {
                paint.color = top.color;
                path.reset();
                path.moveTo(rect.left, rect.top);
                path.lineTo(rect.right, rect.top);
                if (top.width == 0.0)
                {
                    paint.style = PaintingStyle.stroke;
                }
                else
                {
                    paint.style = PaintingStyle.fill;
                    path.lineTo(rect.right - right.width, rect.top + top.width);
                    path.lineTo(rect.left + left.width, rect.top + top.width);
                }
                canvas.drawPath(path, paint);
                break;
            }
            case BorderStyle.none:
            {
                break;
            }
        }
        switch (right.style)
        {
            case BorderStyle.solid:
            {
                paint.color = right.color;
                path.reset();
                path.moveTo(rect.right, rect.top);
                path.lineTo(rect.right, rect.bottom);
                if (right.width == 0.0)
                {
                    paint.style = PaintingStyle.stroke;
                }
                else
                {
                    paint.style = PaintingStyle.fill;
                    path.lineTo(rect.right - right.width, rect.bottom - bottom.width);
                    path.lineTo(rect.right - right.width, rect.top + top.width);
                }
                canvas.drawPath(path, paint);
                break;
            }
            case BorderStyle.none:
            {
                break;
            }
        }
        switch (bottom.style)
        {
            case BorderStyle.solid:
            {
                paint.color = bottom.color;
                path.reset();
                path.moveTo(rect.right, rect.bottom);
                path.lineTo(rect.left, rect.bottom);
                if (bottom.width == 0.0)
                {
                    paint.style = PaintingStyle.stroke;
                }
                else
                {
                    paint.style = PaintingStyle.fill;
                    path.lineTo(rect.left + left.width, rect.bottom - bottom.width);
                    path.lineTo(rect.right - right.width, rect.bottom - bottom.width);
                }
                canvas.drawPath(path, paint);
                break;
            }
            case BorderStyle.none:
            {
                break;
            }
        }
        switch (left.style)
        {
            case BorderStyle.solid:
            {
                paint.color = left.color;
                path.reset();
                path.moveTo(rect.left, rect.bottom);
                path.lineTo(rect.left, rect.top);
                if (left.width == 0.0)
                {
                    paint.style = PaintingStyle.stroke;
                }
                else
                {
                    paint.style = PaintingStyle.fill;
                    path.lineTo(rect.left + left.width, rect.top + top.width);
                    path.lineTo(rect.left + left.width, rect.bottom - bottom.width);
                }
                canvas.drawPath(path, paint);
                break;
            }
            case BorderStyle.none:
            {
                break;
            }
        }
    }
}

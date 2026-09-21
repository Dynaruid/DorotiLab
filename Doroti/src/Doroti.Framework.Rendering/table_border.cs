// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/table_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class TableBorder
{
    public virtual BorderSide top { get; private set; } = default!;
    public virtual BorderSide right { get; private set; } = default!;
    public virtual BorderSide bottom { get; private set; } = default!;
    public virtual BorderSide left { get; private set; } = default!;
    public virtual BorderSide horizontalInside { get; private set; } = default!;
    public virtual BorderSide verticalInside { get; private set; } = default!;
    public virtual BorderRadius borderRadius { get; private set; } = default!;

    public TableBorder(
        BorderSide top = default!,
        BorderSide right = default!,
        BorderSide bottom = default!,
        BorderSide left = default!,
        BorderSide horizontalInside = default!,
        BorderSide verticalInside = default!,
        BorderRadius borderRadius = default!
    )
    {
        BorderSide __top = top ?? BorderSide.none;
        BorderSide __right = right ?? BorderSide.none;
        BorderSide __bottom = bottom ?? BorderSide.none;
        BorderSide __left = left ?? BorderSide.none;
        BorderSide __horizontalInside = horizontalInside ?? BorderSide.none;
        BorderSide __verticalInside = verticalInside ?? BorderSide.none;
        BorderRadius __borderRadius = borderRadius ?? BorderRadius.zero;
        this.top = __top;
        this.right = __right;
        this.bottom = __bottom;
        this.left = __left;
        this.horizontalInside = __horizontalInside;
        this.verticalInside = __verticalInside;
        this.borderRadius = __borderRadius;
    }

    public static TableBorder CreateAll(
        Color color = default!,
        double width = 1.0,
        BorderStyle style = BorderStyle.solid,
        BorderRadius borderRadius = default!
    )
    {
        Color __color = color ?? new Color(0xFF000000);
        BorderRadius __borderRadius = borderRadius ?? BorderRadius.zero;
        var side = new BorderSide(color: __color, width: width, style: style);
        return new TableBorder(
            top: side,
            right: side,
            bottom: side,
            left: side,
            horizontalInside: side,
            verticalInside: side,
            borderRadius: __borderRadius
        );
    }

    public static TableBorder CreateSymmetric(
        BorderSide inside = default!,
        BorderSide outside = default!,
        BorderRadius borderRadius = default!
    )
    {
        var __instance = new TableBorder(
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            borderRadius
        );
        __instance.borderRadius = borderRadius;
        __instance.top = outside;
        __instance.right = outside;
        __instance.bottom = outside;
        __instance.left = outside;
        __instance.horizontalInside = inside;
        __instance.verticalInside = inside;
        return __instance;
    }

    public virtual EdgeInsets dimensions
    {
        get { return new EdgeInsets(left.width, top.width, right.width, bottom.width); }
    }
    public virtual bool isUniform
    {
        get
        {
            return _allSidesMatch((side) => side.color)
                && _allSidesMatch((side) => side.width)
                && _allSidesMatch((side) => side.style);
        }
    }
    internal virtual bool _outerBorderIsUniform
    {
        get
        {
            return _outerSidesMatch((side) => side.color)
                && _outerSidesMatch((side) => side.width)
                && _outerSidesMatch((side) => side.style);
        }
    }

    internal virtual bool _allSidesMatch<T>(Func<BorderSide, T> selector)
    {
        T topValue = selector(top);
        return Equals(selector(right), topValue)
            && Equals(selector(bottom), topValue)
            && Equals(selector(left), topValue)
            && Equals(selector(horizontalInside), topValue)
            && Equals(selector(verticalInside), topValue);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _outerSidesMatch<T>(Func<BorderSide, T> selector)
    {
        T topValue = selector(top);
        return Equals(selector(right), topValue)
            && Equals(selector(bottom), topValue)
            && Equals(selector(left), topValue);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual HashSet<Color> _distinctVisibleOuterColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintTableBorder(Canvas canvas, Rect rect)
    {
        if (_outerBorderIsUniform && (!Equals(borderRadius, BorderRadius.zero)))
        {
            RRect outer = borderRadius.toRRect(rect);
            RRect inner = outer.deflate(top.width);
            var paint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = top.color;
                        return __cascade;
                    }
                )
            )();
            canvas.drawDRRect(outer, inner, paint);
            return;
        }
        HashSet<Color> visibleColors = _distinctVisibleOuterColors();
        if ((checked(visibleColors.Count) == 1L) && (!Equals(borderRadius, BorderRadius.zero)))
        {
            _paintNonUniformBorderWithRadius(
                canvas,
                rect,
                borderRadius: borderRadius,
                top: Equals(top.style, BorderStyle.none) ? BorderSide.none : top,
                right: Equals(right.style, BorderStyle.none) ? BorderSide.none : right,
                bottom: Equals(bottom.style, BorderStyle.none) ? BorderSide.none : bottom,
                left: Equals(left.style, BorderStyle.none) ? BorderSide.none : left,
                color: visibleColors.First()
            );
            return;
        }
        BordersLibrary.paintBorder(
            canvas,
            rect,
            top: top,
            right: right,
            bottom: bottom,
            left: left
        );
    }

    internal static void _paintNonUniformBorderWithRadius(
        Canvas canvas,
        Rect rect,
        BorderRadius borderRadius,
        Color color,
        BorderSide top,
        BorderSide right,
        BorderSide bottom,
        BorderSide left
    )
    {
        RRect borderRect = borderRadius.toRRect(rect);
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color;
                    return __cascade;
                }
            )
        )();
        RRect inner = new EdgeInsets(
            left.strokeInset,
            top.strokeInset,
            right.strokeInset,
            bottom.strokeInset
        ).deflateRRect(borderRect);
        RRect outer = new EdgeInsets(
            left.strokeOutset,
            top.strokeOutset,
            right.strokeOutset,
            bottom.strokeOutset
        ).inflateRRect(borderRect);
        canvas.drawDRRect(outer, inner, paint);
    }

    public virtual TableBorder scale(double t)
    {
        return new TableBorder(
            top: top.scale(t),
            right: right.scale(t),
            bottom: bottom.scale(t),
            left: left.scale(t),
            horizontalInside: horizontalInside.scale(t),
            verticalInside: verticalInside.scale(t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static TableBorder? lerp(TableBorder? a, TableBorder? b, double t)
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
        return new TableBorder(
            top: BorderSide.lerp(a.top, b.top, t),
            right: BorderSide.lerp(a.right, b.right, t),
            bottom: BorderSide.lerp(a.bottom, b.bottom, t),
            left: BorderSide.lerp(a.left, b.left, t),
            horizontalInside: BorderSide.lerp(a.horizontalInside, b.horizontalInside, t),
            verticalInside: BorderSide.lerp(a.verticalInside, b.verticalInside, t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void paint(
        Canvas canvas,
        Rect rect,
        IEnumerable<double> rows,
        IEnumerable<double> columns
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (rows.Count() == 0) || ((rows.First() >= 0.0) && (rows.Last() <= rect.height))
        );
        DartRuntimePrimitives.Assert(() =>
            (columns.Count() == 0) || ((columns.First() >= 0.0) && (columns.Last() <= rect.width))
        );
        if ((columns.Count() != 0) || (rows.Count() != 0))
        {
            var paintLocal = new Paint();
            var path = new Path();
            if (columns.Count() != 0)
            {
                switch (verticalInside.style)
                {
                    case BorderStyle.solid:
                    {
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = paintLocal;
                                    __cascade.color = verticalInside.color;
                                    __cascade.strokeWidth = verticalInside.width;
                                    __cascade.style = PaintingStyle.stroke;
                                    return __cascade;
                                }
                            )
                        )();
                        path.reset();
                        foreach (var x in columns)
                        {
                            path.moveTo(rect.left + x, rect.top);
                            path.lineTo(rect.left + x, rect.bottom);
                        }
                        canvas.drawPath(path, paintLocal);
                        break;
                    }
                    case BorderStyle.none:
                    {
                        break;
                    }
                }
            }
            if (rows.Count() != 0)
            {
                switch (horizontalInside.style)
                {
                    case BorderStyle.solid:
                    {
                        (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = paintLocal;
                                    __cascade.color = horizontalInside.color;
                                    __cascade.strokeWidth = horizontalInside.width;
                                    __cascade.style = PaintingStyle.stroke;
                                    return __cascade;
                                }
                            )
                        )();
                        path.reset();
                        foreach (var y in rows)
                        {
                            path.moveTo(rect.left, rect.top + y);
                            path.lineTo(rect.right, rect.top + y);
                        }
                        canvas.drawPath(path, paintLocal);
                        break;
                    }
                    case BorderStyle.none:
                    {
                        break;
                    }
                }
            }
        }
        _paintTableBorder(canvas, rect);
    }

    public override bool Equals(object? other)
    {
        var __other = other as TableBorder;
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
        return (__other is TableBorder)
            && Equals(__other.top, top)
            && Equals(__other.right, right)
            && Equals(__other.bottom, bottom)
            && Equals(__other.left, left)
            && Equals(__other.horizontalInside, horizontalInside)
            && Equals(__other.verticalInside, verticalInside)
            && Equals(__other.borderRadius, borderRadius);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            top,
            right,
            bottom,
            left,
            horizontalInside,
            verticalInside,
            borderRadius
        );

    public override string ToString() =>
        $"TableBorder({top}, {right}, {bottom}, {left}, {horizontalInside}, {verticalInside}, {borderRadius})";
}

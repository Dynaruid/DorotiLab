// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/table_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class TableBorder
{
    public virtual global::Doroti.Framework.Painting.BorderSide top { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide right { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide bottom { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide left { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide horizontalInside { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderSide verticalInside { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;

    public TableBorder(global::Doroti.Framework.Painting.BorderSide top = default!, global::Doroti.Framework.Painting.BorderSide right = default!, global::Doroti.Framework.Painting.BorderSide bottom = default!, global::Doroti.Framework.Painting.BorderSide left = default!, global::Doroti.Framework.Painting.BorderSide horizontalInside = default!, global::Doroti.Framework.Painting.BorderSide verticalInside = default!, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!)
    {
        global::Doroti.Framework.Painting.BorderSide __top = top ?? BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __right = right ?? BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __bottom = bottom ?? BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __left = left ?? BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __horizontalInside = horizontalInside ?? BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __verticalInside = verticalInside ?? BorderSide.none;
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? BorderRadius.zero;
        this.top = __top;
        this.right = __right;
        this.bottom = __bottom;
        this.left = __left;
        this.horizontalInside = __horizontalInside;
        this.verticalInside = __verticalInside;
        this.borderRadius = __borderRadius;
    }

    public static TableBorder CreateAll(Color color = default!, double width = 1.0, global::Doroti.Framework.Painting.BorderStyle style = BorderStyle.solid, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!)
    {
        Color __color = color ?? new Color(0xFF000000);
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? BorderRadius.zero;
        var side = new global::Doroti.Framework.Painting.BorderSide(color: __color, width: width, style: style);
        return new TableBorder(top: side, right: side, bottom: side, left: side, horizontalInside: side, verticalInside: side, borderRadius: __borderRadius);
    }

    public static TableBorder CreateSymmetric(global::Doroti.Framework.Painting.BorderSide inside = default!, global::Doroti.Framework.Painting.BorderSide outside = default!, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!)
    {
        var __instance = new TableBorder(default!, default!, default!, default!, default!, default!, borderRadius);
        __instance.borderRadius = borderRadius;
        __instance.top = outside;
        __instance.right = outside;
        __instance.bottom = outside;
        __instance.left = outside;
        __instance.horizontalInside = inside;
        __instance.verticalInside = inside;
        return __instance;
    }

    public virtual global::Doroti.Framework.Painting.EdgeInsets dimensions
    {
        get
        {
            return new global::Doroti.Framework.Painting.EdgeInsets(left.width, top.width, right.width, bottom.width);
        }
    }
    public virtual bool isUniform
    {
        get
        {
            return _allSidesMatch<global::Doroti.Ui.Color>((side) => side.color) && _allSidesMatch<double>((side) => side.width) && _allSidesMatch<global::Doroti.Framework.Painting.BorderStyle>((side) => side.style);
        }
    }
    internal virtual bool _outerBorderIsUniform
    {
        get
        {
            return _outerSidesMatch<global::Doroti.Ui.Color>((side) => side.color) && _outerSidesMatch<double>((side) => side.width) && _outerSidesMatch<global::Doroti.Framework.Painting.BorderStyle>((side) => side.style);
        }
    }
    internal virtual bool _allSidesMatch<T>(Func<global::Doroti.Framework.Painting.BorderSide, T> selector)
    {
        T topValue = selector(top);
        return Equals(selector(right), topValue) && Equals(selector(bottom), topValue) && Equals(selector(left), topValue) && Equals(selector(horizontalInside), topValue) && Equals(selector(verticalInside), topValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _outerSidesMatch<T>(Func<global::Doroti.Framework.Painting.BorderSide, T> selector)
    {
        T topValue = selector(top);
        return Equals(selector(right), topValue) && Equals(selector(bottom), topValue) && Equals(selector(left), topValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<global::Doroti.Ui.Color> _distinctVisibleOuterColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintTableBorder(Canvas canvas, Rect rect)
    {
        if (_outerBorderIsUniform && (!Equals(borderRadius, BorderRadius.zero)))
        {
            global::Doroti.Ui.RRect outer = borderRadius.toRRect(rect);
            global::Doroti.Ui.RRect inner = outer.deflate(top.width);
            var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = top.color;
    return __cascade;
}))();
            canvas.drawDRRect(outer, inner, paint);
            return;
        }
        HashSet<global::Doroti.Ui.Color> visibleColors = _distinctVisibleOuterColors();
        if ((checked(visibleColors.Count) == 1L) && (!Equals(borderRadius, BorderRadius.zero)))
        {
            _paintNonUniformBorderWithRadius(canvas, rect, borderRadius: borderRadius, top: Equals(top.style, BorderStyle.none) ? BorderSide.none : top, right: Equals(right.style, BorderStyle.none) ? BorderSide.none : right, bottom: Equals(bottom.style, BorderStyle.none) ? BorderSide.none : bottom, left: Equals(left.style, BorderStyle.none) ? BorderSide.none : left, color: visibleColors.First());
            return;
        }
        BordersLibrary.paintBorder(canvas, rect, top: top, right: right, bottom: bottom, left: left);
    }

    internal static void _paintNonUniformBorderWithRadius(Canvas canvas, Rect rect, global::Doroti.Framework.Painting.BorderRadius borderRadius, Color color, global::Doroti.Framework.Painting.BorderSide top, global::Doroti.Framework.Painting.BorderSide right, global::Doroti.Framework.Painting.BorderSide bottom, global::Doroti.Framework.Painting.BorderSide left)
    {
        global::Doroti.Ui.RRect borderRect = borderRadius.toRRect(rect);
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))();
        global::Doroti.Ui.RRect inner = new global::Doroti.Framework.Painting.EdgeInsets(left.strokeInset, top.strokeInset, right.strokeInset, bottom.strokeInset).deflateRRect(borderRect);
        global::Doroti.Ui.RRect outer = new global::Doroti.Framework.Painting.EdgeInsets(left.strokeOutset, top.strokeOutset, right.strokeOutset, bottom.strokeOutset).inflateRRect(borderRect);
        canvas.drawDRRect(outer, inner, paint);
    }

    public virtual TableBorder scale(double t)
    {
        return new TableBorder(top: top.scale(t), right: right.scale(t), bottom: bottom.scale(t), left: left.scale(t), horizontalInside: horizontalInside.scale(t), verticalInside: verticalInside.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        return new TableBorder(top: BorderSide.lerp(a.top, b.top, t), right: BorderSide.lerp(a.right, b.right, t), bottom: BorderSide.lerp(a.bottom, b.bottom, t), left: BorderSide.lerp(a.left, b.left, t), horizontalInside: BorderSide.lerp(a.horizontalInside, b.horizontalInside, t), verticalInside: BorderSide.lerp(a.verticalInside, b.verticalInside, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(Canvas canvas, Rect rect, IEnumerable<double> rows, IEnumerable<double> columns)
    {
        DartRuntimePrimitives.Assert(() => (rows.Count() == 0) || (rows.First() >= 0.0) && (rows.Last() <= rect.height));
        DartRuntimePrimitives.Assert(() => (columns.Count() == 0) || (columns.First() >= 0.0) && (columns.Last() <= rect.width));
        if ((columns.Count() != 0) || (rows.Count() != 0))
        {
            var paintLocal = new global::Doroti.Ui.Paint();
            var path = new global::Doroti.Ui.Path();
            if (columns.Count() != 0)
            {
                switch (verticalInside.style)
                {
                    case BorderStyle.solid:
                        {
                            ((Func<Paint>)(() =>
{
    var __cascade = paintLocal;
    __cascade.color = verticalInside.color;
    __cascade.strokeWidth = verticalInside.width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
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
                            ((Func<Paint>)(() =>
{
    var __cascade = paintLocal;
    __cascade.color = horizontalInside.color;
    __cascade.strokeWidth = horizontalInside.width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
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
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TableBorder) && Equals(__other.top, top) && Equals(__other.right, right) && Equals(__other.bottom, bottom) && Equals(__other.left, left) && Equals(__other.horizontalInside, horizontalInside) && Equals(__other.verticalInside, verticalInside) && Equals(__other.borderRadius, borderRadius);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(top, right, bottom, left, horizontalInside, verticalInside, borderRadius);
    public override string ToString() => $"TableBorder({top}, {right}, {bottom}, {left}, {horizontalInside}, {verticalInside}, {borderRadius})";
}


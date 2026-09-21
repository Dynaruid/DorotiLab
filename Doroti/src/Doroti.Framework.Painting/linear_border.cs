// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/linear_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class LinearBorderEdge
{
    public virtual double size { get; private set; } = default!;
    public virtual double alignment { get; private set; } = default!;

    public LinearBorderEdge(double size = 1.0, double alignment = 0.0)
    {
        this.size = size;
        this.alignment = alignment;
        System.Diagnostics.Debug.Assert((size >= 0.0) && (size <= 1.0));
    }

    public static LinearBorderEdge? lerp(LinearBorderEdge? a, LinearBorderEdge? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        a ??= new LinearBorderEdge(alignment: b!.alignment, size: 0);
        b ??= new LinearBorderEdge(alignment: a.alignment, size: 0);
        return new LinearBorderEdge(
            size: (
                Dart_uiLibrary.lerpDouble(a.size, b.size, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            alignment: (
                Dart_uiLibrary.lerpDouble(a.alignment, b.alignment, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as LinearBorderEdge;
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
        return (__other is LinearBorderEdge)
            && (__other.size == size)
            && (__other.alignment == alignment);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(size, alignment);

    public override string ToString()
    {
        var s = new StringBuffer(
            $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearBorderEdge")}("
        );
        if (size != 1.0)
        {
            s.write($"size: {size}");
        }
        if (alignment != 0L)
        {
            var comma = (size != 1.0) ? ", " : "";
            s.write($"{comma}alignment: {alignment}");
        }
        s.write(")");
        return s.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class LinearBorder : OutlinedBorder
{
    public static LinearBorder none = new LinearBorder();
    public virtual LinearBorderEdge? start { get; private set; }
    public virtual LinearBorderEdge? end { get; private set; }
    public virtual LinearBorderEdge? top { get; private set; }
    public virtual LinearBorderEdge? bottom { get; private set; }

    public LinearBorder(
        BorderSide side = default!,
        LinearBorderEdge? start = null,
        LinearBorderEdge? end = null,
        LinearBorderEdge? top = null,
        LinearBorderEdge? bottom = null
    )
        : base(side: side ?? BorderSide.none)
    {
        this.start = start;
        this.end = end;
        this.top = top;
        this.bottom = bottom;
    }

    public static LinearBorder CreateStart(
        BorderSide side = default!,
        double alignment = 0.0,
        double size = 1.0
    )
    {
        var __instance = new LinearBorder(side, default!, default!, default!, default!);
        __instance.start = new LinearBorderEdge(alignment: alignment, size: size);
        __instance.end = null;
        __instance.top = null;
        __instance.bottom = null;
        return __instance;
    }

    public static LinearBorder CreateEnd(
        BorderSide side = default!,
        double alignment = 0.0,
        double size = 1.0
    )
    {
        var __instance = new LinearBorder(side, default!, default!, default!, default!);
        __instance.start = null;
        __instance.end = new LinearBorderEdge(alignment: alignment, size: size);
        __instance.top = null;
        __instance.bottom = null;
        return __instance;
    }

    public static LinearBorder CreateTop(
        BorderSide side = default!,
        double alignment = 0.0,
        double size = 1.0
    )
    {
        var __instance = new LinearBorder(side, default!, default!, default!, default!);
        __instance.start = null;
        __instance.end = null;
        __instance.top = new LinearBorderEdge(alignment: alignment, size: size);
        __instance.bottom = null;
        return __instance;
    }

    public static LinearBorder CreateBottom(
        BorderSide side = default!,
        double alignment = 0.0,
        double size = 1.0
    )
    {
        var __instance = new LinearBorder(side, default!, default!, default!, default!);
        __instance.start = null;
        __instance.end = null;
        __instance.top = null;
        __instance.bottom = new LinearBorderEdge(alignment: alignment, size: size);
        return __instance;
    }

    public override LinearBorder scale(double t)
    {
        return new LinearBorder(side: side.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            double widthLocal = side.width;
            return new EdgeInsetsDirectional(
                (start is null) ? 0.0 : widthLocal,
                (top is null) ? 0.0 : widthLocal,
                (end is null) ? 0.0 : widthLocal,
                (bottom is null) ? 0.0 : widthLocal
            );
        }
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is LinearBorder)
        {
            LinearBorder a__as7474 = (LinearBorder)a;
            return new LinearBorder(
                side: BorderSide.lerp(a__as7474.side, side, t),
                start: LinearBorderEdge.lerp(a__as7474.start, start, t),
                end: LinearBorderEdge.lerp(a__as7474.end, end, t),
                top: LinearBorderEdge.lerp(a__as7474.top, top, t),
                bottom: LinearBorderEdge.lerp(a__as7474.bottom, bottom, t)
            );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is LinearBorder)
        {
            LinearBorder b__as7912 = (LinearBorder)b;
            return new LinearBorder(
                side: BorderSide.lerp(side, b__as7912.side, t),
                start: LinearBorderEdge.lerp(start, b__as7912.start, t),
                end: LinearBorderEdge.lerp(end, b__as7912.end, t),
                top: LinearBorderEdge.lerp(top, b__as7912.top, t),
                bottom: LinearBorderEdge.lerp(bottom, b__as7912.bottom, t)
            );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearBorder copyWith(
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
    )
    {
        return new LinearBorder(
            side: side ?? this.side,
            start: start ?? this.start,
            end: end ?? this.end,
            top: top ?? this.top,
            bottom: bottom ?? this.bottom
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        Rect adjustedRect = dimensions.resolve(textDirection).deflateRect(rect);
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(adjustedRect);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(rect);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        EdgeInsets insets = dimensions.resolve(textDirection);
        var rtlLocal = Equals(textDirection, TextDirection.rtl);
        var path = new Path();
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.strokeWidth = 0.0;
                    return __cascade;
                }
            )
        )();
        void drawEdge(Rect rect, Color color)
        {
            paintLocal.color = color;
            path.reset();
            path.moveTo(rect.left, (rect.top));
            if (rect.width == 0.0)
            {
                paintLocal.style = PaintingStyle.stroke;
                path.lineTo(rect.left, (rect.bottom));
            }
            else
            {
                if (rect.height == 0.0)
                {
                    paintLocal.style = PaintingStyle.stroke;
                    path.lineTo(rect.right, (rect.top));
                }
                else
                {
                    paintLocal.style = PaintingStyle.fill;
                    path.lineTo(rect.right, (rect.top));
                    path.lineTo(rect.right, (rect.bottom));
                    path.lineTo(rect.left, (rect.bottom));
                }
            }
            canvas.drawPath(path, paintLocal);
        }
        if ((start is not null) && (start!.size != 0.0) && (!Equals(side.style, BorderStyle.none)))
        {
            var insetRect = Rect.fromLTWH(
                rect.left,
                rect.top + insets.top,
                rect.width,
                rect.height - insets.vertical
            );
            double x = rtlLocal ? (rect.right - insets.right) : rect.left;
            double widthLocal = rtlLocal ? insets.right : insets.left;
            double heightLocal = insetRect.height * start!.size;
            double y = (insetRect.height - heightLocal) * ((start!.alignment + 1.0) / 2.0);
            var r = Rect.fromLTWH(x, y, widthLocal, heightLocal);
            drawEdge(r, side.color);
        }
        if ((end is not null) && (end!.size != 0.0) && (!Equals(side.style, BorderStyle.none)))
        {
            var insetRectLocal = Rect.fromLTWH(
                rect.left,
                rect.top + insets.top,
                rect.width,
                rect.height - insets.vertical
            );
            double xLocal = rtlLocal ? rect.left : (rect.right - insets.right);
            double widthAlternate = rtlLocal ? insets.left : insets.right;
            double heightAlternate = insetRectLocal.height * end!.size;
            double yLocal =
                (insetRectLocal.height - heightAlternate) * ((end!.alignment + 1.0) / 2.0);
            var rLocal = Rect.fromLTWH(xLocal, yLocal, widthAlternate, heightAlternate);
            drawEdge(rLocal, side.color);
        }
        if ((top is not null) && (top!.size != 0.0) && (!Equals(side.style, BorderStyle.none)))
        {
            double widthNested = rect.width * top!.size;
            double startX = (rect.width - widthNested) * ((top!.alignment + 1.0) / 2.0);
            double xAlternate = rtlLocal ? (rect.width - startX - widthNested) : startX;
            var rAlternate = Rect.fromLTWH(xAlternate, rect.top, widthNested, insets.top);
            drawEdge(rAlternate, side.color);
        }
        if (
            (bottom is not null)
            && (bottom!.size != 0.0)
            && (!Equals(side.style, BorderStyle.none))
        )
        {
            double widthCurrent = rect.width * bottom!.size;
            double startXLocal = (rect.width - widthCurrent) * ((bottom!.alignment + 1.0) / 2.0);
            double xNested = rtlLocal ? (rect.width - startXLocal - widthCurrent) : startXLocal;
            var rNested = Rect.fromLTWH(
                xNested,
                rect.bottom - insets.bottom,
                widthCurrent,
                side.width
            );
            drawEdge(rNested, side.color);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as LinearBorder;
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
        return (__other is LinearBorder)
            && Equals(__other.side, side)
            && Equals(__other.start, start)
            && Equals(__other.end, end)
            && Equals(__other.top, top)
            && Equals(__other.bottom, bottom);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(side, start, end, top, bottom);

    public override string ToString()
    {
        if (Equals(this, none))
        {
            return "LinearBorder.none";
        }
        var s = new StringBuffer(
            $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearBorder")}(side: {side}"
        );
        if (start is not null)
        {
            s.write($", start: {start}");
        }
        if (end is not null)
        {
            s.write($", end: {end}");
        }
        if (top is not null)
        {
            s.write($", top: {top}");
        }
        if (bottom is not null)
        {
            s.write($", bottom: {bottom}");
        }
        s.write(")");
        return s.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

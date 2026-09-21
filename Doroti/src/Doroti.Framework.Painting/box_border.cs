// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public enum BoxShape
{
    rectangle,
    circle,
}

public abstract class BoxBorder : ShapeBorder
{
    protected BoxBorder() { }

    public static BoxBorder CreateFromLTRB(
        BorderSide top = default!,
        BorderSide right = default!,
        BorderSide bottom = default!,
        BorderSide left = default!
    ) => new Border(top: top, right: right, bottom: bottom, left: left);

    public static BoxBorder CreateAll(
        Color color = default!,
        double width = default!,
        BorderStyle style = default!,
        double strokeAlign = default!
    ) => Border.CreateAll(color, width, style, strokeAlign);

    public static BoxBorder CreateFromBorderSide(BorderSide side) =>
        Border.CreateFromBorderSide(side);

    public static BoxBorder CreateSymmetric(
        BorderSide vertical = default!,
        BorderSide horizontal = default!
    ) => Border.CreateSymmetric(vertical, horizontal);

    public static BoxBorder CreateFromSTEB(
        BorderSide top = default!,
        BorderSide start = default!,
        BorderSide end = default!,
        BorderSide bottom = default!
    ) => new BorderDirectional(top: top, start: start, end: end, bottom: bottom);

    public abstract BorderSide top { get; }
    public abstract BorderSide bottom { get; }
    public abstract bool isUniform { get; }

    public override BoxBorder? add(ShapeBorder other, bool reversed = false) => null;

    public static BoxBorder? lerp(BoxBorder? a, BoxBorder? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is Border && b is Border)
        {
            Border a__as6271 = (Border)a;
            Border b__as6289 = (Border)b;
            return Border.lerp((Border?)a__as6271, (Border?)b__as6289, t);
        }
        if (a is BorderDirectional && b is BorderDirectional)
        {
            BorderDirectional a__as6356 = (BorderDirectional)a;
            BorderDirectional b__as6385 = (BorderDirectional)b;
            return BorderDirectional.lerp(
                (BorderDirectional?)a__as6356,
                (BorderDirectional?)b__as6385,
                t
            );
        }
        if ((b is Border) && (a is BorderDirectional))
        {
            (a, b) = ((Border)b, (BorderDirectional)a);
            t = 1.0 - t;
        }
        if ((a is Border) && (b is BorderDirectional))
        {
            Border a__as6605 = (Border)a;
            BorderDirectional b__as6620 = (BorderDirectional)b;
            if (Equals(b__as6620.start, BorderSide.none) && Equals(b__as6620.end, BorderSide.none))
            {
                return new Border(
                    top: BorderSide.lerp(a__as6605.top, b__as6620.top, t),
                    right: BorderSide.lerp(a__as6605.right, BorderSide.none, t),
                    bottom: BorderSide.lerp(a__as6605.bottom, b__as6620.bottom, t),
                    left: BorderSide.lerp(a__as6605.left, BorderSide.none, t)
                );
            }
            if (Equals(a__as6605.left, BorderSide.none) && Equals(a__as6605.right, BorderSide.none))
            {
                return new BorderDirectional(
                    top: BorderSide.lerp(a__as6605.top, b__as6620.top, t),
                    start: BorderSide.lerp(BorderSide.none, b__as6620.start, t),
                    end: BorderSide.lerp(BorderSide.none, b__as6620.end, t),
                    bottom: BorderSide.lerp(a__as6605.bottom, b__as6620.bottom, t)
                );
            }
            if (t < 0.5)
            {
                return new Border(
                    top: BorderSide.lerp(a__as6605.top, b__as6620.top, t),
                    right: BorderSide.lerp(a__as6605.right, BorderSide.none, t * 2.0),
                    bottom: BorderSide.lerp(a__as6605.bottom, b__as6620.bottom, t),
                    left: BorderSide.lerp(a__as6605.left, BorderSide.none, t * 2.0)
                );
            }
            return new BorderDirectional(
                top: BorderSide.lerp(a__as6605.top, b__as6620.top, t),
                start: BorderSide.lerp(BorderSide.none, b__as6620.start, (t - 0.5) * 2.0),
                end: BorderSide.lerp(BorderSide.none, b__as6620.end, (t - 0.5) * 2.0),
                bottom: BorderSide.lerp(a__as6605.bottom, b__as6620.bottom, t)
            );
        }
        ShapeBorder? result = b?.lerpFrom(a, t) ?? a?.lerpTo(b, t);
        return ((BoxBorder?)(object?)result)! ?? ((t < 0.5) ? a : b);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => textDirection is not null);
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(dimensions.resolve(textDirection).deflateRect(rect));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => textDirection is not null);
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

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return rect.contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        canvas.drawRect(rect, paint);
    }

    public override bool preferPaintInterior => true;
    public abstract override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    );

    internal static void _paintUniformBorderWithRadius(
        Canvas canvas,
        Rect rect,
        BorderSide side,
        BorderRadius borderRadius
    )
    {
        DartRuntimePrimitives.Assert(() => !Equals(side.style, BorderStyle.none));
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = side.color;
                    return __cascade;
                }
            )
        )();
        double widthLocal = side.width;
        if (widthLocal == 0.0)
        {
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = paint;
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.strokeWidth = 0.0;
                        return __cascade;
                    }
                )
            )();
            canvas.drawRRect(borderRadius.toRRect(rect), paint);
        }
        else
        {
            RRect borderRect = borderRadius.toRRect(rect);
            RRect inner = borderRect.deflate(side.strokeInset);
            RRect outer = borderRect.inflate(side.strokeOutset);
            canvas.drawDRRect(outer, inner, paint);
        }
    }

    public static void paintNonUniformBorder(
        Canvas canvas,
        Rect rect,
        BorderRadius? borderRadius,
        TextDirection? textDirection,
        BoxShape shape = BoxShape.rectangle,
        BorderSide top = default!,
        BorderSide right = default!,
        BorderSide bottom = default!,
        BorderSide left = default!,
        Color color = default!
    )
    {
        RRect borderRect = default!;
        switch (shape)
        {
            case BoxShape.rectangle:
            {
                borderRect = (borderRadius ?? BorderRadius.zero)
                    .resolve(textDirection)
                    .toRRect(rect);
                break;
            }
            case BoxShape.circle:
            {
                DartRuntimePrimitives.Assert(() => borderRadius is null);
                borderRect = RRect.fromRectAndRadius(
                    Rect.fromCircle(center: rect.center, radius: rect.shortestSide / 2.0),
                    Radius.circular(rect.width)
                );
                break;
            }
        }
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

    internal static void _paintUniformBorderWithCircle(Canvas canvas, Rect rect, BorderSide side)
    {
        DartRuntimePrimitives.Assert(() => !Equals(side.style, BorderStyle.none));
        double radius = (rect.shortestSide + side.strokeOffset) / 2L;
        canvas.drawCircle(rect.center, radius, side.toPaint());
    }

    internal static void _paintUniformBorderWithRectangle(Canvas canvas, Rect rect, BorderSide side)
    {
        DartRuntimePrimitives.Assert(() => !Equals(side.style, BorderStyle.none));
        canvas.drawRect(rect.inflate(side.strokeOffset / 2L), side.toPaint());
    }
}

public class Border : BoxBorder
{
    private BorderSide __field_top = default!;
    public override BorderSide top
    {
        get => __field_top;
    }
    public virtual BorderSide right { get; private set; } = default!;
    private BorderSide __field_bottom = default!;
    public override BorderSide bottom
    {
        get => __field_bottom;
    }
    public virtual BorderSide left { get; private set; } = default!;

    public Border(
        BorderSide top = default!,
        BorderSide right = default!,
        BorderSide bottom = default!,
        BorderSide left = default!
    )
    {
        BorderSide __top = top ?? BorderSide.none;
        BorderSide __right = right ?? BorderSide.none;
        BorderSide __bottom = bottom ?? BorderSide.none;
        BorderSide __left = left ?? BorderSide.none;
        __field_top = __top;
        this.right = __right;
        __field_bottom = __bottom;
        this.left = __left;
    }

    public static new Border CreateFromBorderSide(BorderSide side)
    {
        var __instance = new Border(default!, default!, default!, default!);
        __instance.__field_top = side;
        __instance.right = side;
        __instance.__field_bottom = side;
        __instance.left = side;
        return __instance;
    }

    public static new Border CreateSymmetric(
        BorderSide vertical = default!,
        BorderSide horizontal = default!
    )
    {
        var __instance = new Border(default!, default!, default!, default!);
        __instance.left = vertical;
        __instance.__field_top = horizontal;
        __instance.right = vertical;
        __instance.__field_bottom = horizontal;
        return __instance;
    }

    public static Border CreateAll(
        Color color = default!,
        double width = 1.0,
        BorderStyle style = BorderStyle.solid,
        double? strokeAlign = null
    )
    {
        Color __color = color ?? new Color(0xFF000000);
        double __strokeAlign = strokeAlign ?? BorderSide.strokeAlignInside;
        var side = new BorderSide(
            color: __color,
            width: width,
            style: style,
            strokeAlign: DartRuntimePrimitives.RequireValue(__strokeAlign)
        );
        return CreateFromBorderSide(side);
    }

    public static Border merge(Border a, Border b)
    {
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.top, b.top));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.right, b.right));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.bottom, b.bottom));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.left, b.left));
        return new Border(
            top: BorderSide.merge(a.top, b.top),
            right: BorderSide.merge(a.right, b.right),
            bottom: BorderSide.merge(a.bottom, b.bottom),
            left: BorderSide.merge(a.left, b.left)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            return new EdgeInsets(
                left.strokeInset,
                top.strokeInset,
                right.strokeInset,
                bottom.strokeInset
            );
        }
    }
    public override bool isUniform =>
        _colorIsUniform && _widthIsUniform && _styleIsUniform && _strokeAlignIsUniform;
    internal virtual bool _colorIsUniform
    {
        get
        {
            Color topColor = top.color;
            return Equals(left.color, topColor)
                && Equals(bottom.color, topColor)
                && Equals(right.color, topColor);
        }
    }
    internal virtual bool _widthIsUniform
    {
        get
        {
            double topWidth = top.width;
            return (left.width == topWidth)
                && (bottom.width == topWidth)
                && (right.width == topWidth);
        }
    }
    internal virtual bool _styleIsUniform
    {
        get
        {
            BorderStyle topStyle = top.style;
            return Equals(left.style, topStyle)
                && Equals(bottom.style, topStyle)
                && Equals(right.style, topStyle);
        }
    }
    internal virtual bool _strokeAlignIsUniform
    {
        get
        {
            double topStrokeAlign = top.strokeAlign;
            return (left.strokeAlign == topStrokeAlign)
                && (bottom.strokeAlign == topStrokeAlign)
                && (right.strokeAlign == topStrokeAlign);
        }
    }

    internal virtual HashSet<Color> _distinctVisibleColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasHairlineBorder =>
        (Equals(top.style, BorderStyle.solid) && (top.width == 0.0))
        || (Equals(right.style, BorderStyle.solid) && (right.width == 0.0))
        || (Equals(bottom.style, BorderStyle.solid) && (bottom.width == 0.0))
        || (Equals(left.style, BorderStyle.solid) && (left.width == 0.0));

    public override Border? add(ShapeBorder other, bool reversed = false)
    {
        if (
            (other is Border)
            && BorderSide.canMerge(top, ((Border)other).top)
            && BorderSide.canMerge(right, ((Border)other).right)
            && BorderSide.canMerge(bottom, ((Border)other).bottom)
            && BorderSide.canMerge(left, ((Border)other).left)
        )
        {
            Border other__as19873 = (Border)other;
            return merge(this, other__as19873);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Border scale(double t)
    {
        return new Border(
            top: top.scale(t),
            right: right.scale(t),
            bottom: bottom.scale(t),
            left: left.scale(t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is Border)
        {
            Border a__as20414 = (Border)a;
            return lerp(a__as20414, this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is Border)
        {
            Border b__as20581 = (Border)b;
            return lerp(this, b__as20581, t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Border? lerp(Border? a, Border? b, double t)
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
        return new Border(
            top: BorderSide.lerp(a.top, b.top, t),
            right: BorderSide.lerp(a.right, b.right, t),
            bottom: BorderSide.lerp(a.bottom, b.bottom, t),
            left: BorderSide.lerp(a.left, b.left, t)
        );
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
        if (isUniform)
        {
            switch (top.style)
            {
                case BorderStyle.none:
                {
                    return;
                }
                case BorderStyle.solid:
                {
                    switch (shape)
                    {
                        case BoxShape.circle:
                        {
                            DartRuntimePrimitives.Assert(() => borderRadius is null);
                            _paintUniformBorderWithCircle(canvas, rect, top);
                            break;
                        }
                        case BoxShape.rectangle:
                        {
                            if (
                                (borderRadius is not null)
                                && (!Equals(borderRadius, BorderRadius.zero))
                            )
                            {
                                _paintUniformBorderWithRadius(canvas, rect, top, borderRadius);
                                return;
                            }
                            _paintUniformBorderWithRectangle(canvas, rect, top);
                            break;
                        }
                    }
                    return;
                }
            }
        }
        if (_styleIsUniform && Equals(top.style, BorderStyle.none))
        {
            return;
        }
        HashSet<Color> visibleColors = _distinctVisibleColors();
        bool hasHairlineBorder = _hasHairlineBorder;
        if (
            (checked(visibleColors.Count) == 1L)
            && !hasHairlineBorder
            && (
                Equals(shape, BoxShape.circle)
                || ((borderRadius is not null) && (!Equals(borderRadius, BorderRadius.zero)))
            )
        )
        {
            paintNonUniformBorder(
                canvas,
                rect,
                shape: shape,
                borderRadius: borderRadius,
                textDirection: textDirection,
                top: Equals(top.style, BorderStyle.none) ? BorderSide.none : top,
                right: Equals(right.style, BorderStyle.none) ? BorderSide.none : right,
                bottom: Equals(bottom.style, BorderStyle.none) ? BorderSide.none : bottom,
                left: Equals(left.style, BorderStyle.none) ? BorderSide.none : left,
                color: visibleColors.First()
            );
            return;
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (hasHairlineBorder)
            {
                DartRuntimePrimitives.Assert(() =>
                    (borderRadius is null) || Equals(borderRadius, BorderRadius.zero)
                );
            }
            if ((borderRadius is not null) && (!Equals(borderRadius, BorderRadius.zero)))
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            "A borderRadius can only be given on borders with uniform colors."
                        ),
                        new ErrorDescription("The following is not uniform:"),
                    }
                );
            }
            return true;
        });
        DartRuntimePrimitives.Assert(() =>
        {
            if (!Equals(shape, BoxShape.rectangle))
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            "A Border can only be drawn as a circle on borders with uniform colors."
                        ),
                        new ErrorDescription("The following is not uniform:"),
                    }
                );
            }
            return true;
        });
        DartRuntimePrimitives.Assert(() =>
        {
            if (!_strokeAlignIsUniform || (top.strokeAlign != BorderSide.strokeAlignInside))
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            "A Border can only draw strokeAlign different than BorderSide.strokeAlignInside on borders with uniform colors."
                        ),
                    }
                );
            }
            return true;
        });
        BordersLibrary.paintBorder(
            canvas,
            rect,
            top: top,
            right: right,
            bottom: bottom,
            left: left
        );
    }

    public override bool Equals(object? other)
    {
        var __other = other as Border;
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
        return (__other is Border)
            && Equals(__other.top, top)
            && Equals(__other.right, right)
            && Equals(__other.bottom, bottom)
            && Equals(__other.left, left);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(top, right, bottom, left);

    public override string ToString()
    {
        if (isUniform)
        {
            return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "Border")}.all({top})";
        }
        var arguments = new List<string>();
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "Border")}({string.Join(", ", arguments)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class BorderDirectional : BoxBorder
{
    private BorderSide __field_top = default!;
    public override BorderSide top
    {
        get => __field_top;
    }
    public virtual BorderSide start { get; private set; } = default!;
    public virtual BorderSide end { get; private set; } = default!;
    private BorderSide __field_bottom = default!;
    public override BorderSide bottom
    {
        get => __field_bottom;
    }

    public BorderDirectional(
        BorderSide top = default!,
        BorderSide start = default!,
        BorderSide end = default!,
        BorderSide bottom = default!
    )
    {
        BorderSide __top = top ?? BorderSide.none;
        BorderSide __start = start ?? BorderSide.none;
        BorderSide __end = end ?? BorderSide.none;
        BorderSide __bottom = bottom ?? BorderSide.none;
        __field_top = __top;
        this.start = __start;
        this.end = __end;
        __field_bottom = __bottom;
    }

    public static BorderDirectional merge(BorderDirectional a, BorderDirectional b)
    {
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.top, b.top));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.start, b.start));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.end, b.end));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(a.bottom, b.bottom));
        return new BorderDirectional(
            top: BorderSide.merge(a.top, b.top),
            start: BorderSide.merge(a.start, b.start),
            end: BorderSide.merge(a.end, b.end),
            bottom: BorderSide.merge(a.bottom, b.bottom)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            return new EdgeInsetsDirectional(
                start.strokeInset,
                top.strokeInset,
                end.strokeInset,
                bottom.strokeInset
            );
        }
    }
    public override bool isUniform =>
        _colorIsUniform && _widthIsUniform && _styleIsUniform && _strokeAlignIsUniform;
    internal virtual bool _colorIsUniform
    {
        get
        {
            Color topColor = top.color;
            return Equals(start.color, topColor)
                && Equals(bottom.color, topColor)
                && Equals(end.color, topColor);
        }
    }
    internal virtual bool _widthIsUniform
    {
        get
        {
            double topWidth = top.width;
            return (start.width == topWidth)
                && (bottom.width == topWidth)
                && (end.width == topWidth);
        }
    }
    internal virtual bool _styleIsUniform
    {
        get
        {
            BorderStyle topStyle = top.style;
            return Equals(start.style, topStyle)
                && Equals(bottom.style, topStyle)
                && Equals(end.style, topStyle);
        }
    }
    internal virtual bool _strokeAlignIsUniform
    {
        get
        {
            double topStrokeAlign = top.strokeAlign;
            return (start.strokeAlign == topStrokeAlign)
                && (bottom.strokeAlign == topStrokeAlign)
                && (end.strokeAlign == topStrokeAlign);
        }
    }

    internal virtual HashSet<Color> _distinctVisibleColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasHairlineBorder =>
        (Equals(top.style, BorderStyle.solid) && (top.width == 0.0))
        || (Equals(end.style, BorderStyle.solid) && (end.width == 0.0))
        || (Equals(bottom.style, BorderStyle.solid) && (bottom.width == 0.0))
        || (Equals(start.style, BorderStyle.solid) && (start.width == 0.0));

    public override BoxBorder? add(ShapeBorder other, bool reversed = false)
    {
        if (other is BorderDirectional)
        {
            BorderDirectional other__as31816 = (BorderDirectional)other;
            BorderDirectional typedOther = other__as31816;
            if (
                BorderSide.canMerge(top, typedOther.top)
                && BorderSide.canMerge(start, typedOther.start)
                && BorderSide.canMerge(end, typedOther.end)
                && BorderSide.canMerge(bottom, typedOther.bottom)
            )
            {
                return merge(this, typedOther);
            }
            return null;
        }
        if (other is Border)
        {
            Border other__as32221 = (Border)other;
            Border typedOtherLocal = other__as32221;
            if (
                !BorderSide.canMerge(typedOtherLocal.top, top)
                || !BorderSide.canMerge(typedOtherLocal.bottom, bottom)
            )
            {
                return null;
            }
            if ((!Equals(start, BorderSide.none)) || (!Equals(end, BorderSide.none)))
            {
                if (
                    (!Equals(typedOtherLocal.left, BorderSide.none))
                    || (!Equals(typedOtherLocal.right, BorderSide.none))
                )
                {
                    return null;
                }
                DartRuntimePrimitives.Assert(() => Equals(typedOtherLocal.left, BorderSide.none));
                DartRuntimePrimitives.Assert(() => Equals(typedOtherLocal.right, BorderSide.none));
                return new BorderDirectional(
                    top: BorderSide.merge(typedOtherLocal.top, top),
                    start: start,
                    end: end,
                    bottom: BorderSide.merge(typedOtherLocal.bottom, bottom)
                );
            }
            DartRuntimePrimitives.Assert(() => Equals(start, BorderSide.none));
            DartRuntimePrimitives.Assert(() => Equals(end, BorderSide.none));
            return new Border(
                top: BorderSide.merge(typedOtherLocal.top, top),
                right: typedOtherLocal.right,
                bottom: BorderSide.merge(typedOtherLocal.bottom, bottom),
                left: typedOtherLocal.left
            );
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderDirectional scale(double t)
    {
        return new BorderDirectional(
            top: top.scale(t),
            start: start.scale(t),
            end: end.scale(t),
            bottom: bottom.scale(t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is BorderDirectional)
        {
            BorderDirectional a__as33516 = (BorderDirectional)a;
            return lerp(a__as33516, this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is BorderDirectional)
        {
            BorderDirectional b__as33705 = (BorderDirectional)b;
            return lerp(this, b__as33705, t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BorderDirectional? lerp(BorderDirectional? a, BorderDirectional? b, double t)
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
        return new BorderDirectional(
            top: BorderSide.lerp(a.top, b.top, t),
            end: BorderSide.lerp(a.end, b.end, t),
            bottom: BorderSide.lerp(a.bottom, b.bottom, t),
            start: BorderSide.lerp(a.start, b.start, t)
        );
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
        if (isUniform)
        {
            switch (top.style)
            {
                case BorderStyle.none:
                {
                    return;
                }
                case BorderStyle.solid:
                {
                    switch (shape)
                    {
                        case BoxShape.circle:
                        {
                            DartRuntimePrimitives.Assert(() => borderRadius is null);
                            _paintUniformBorderWithCircle(canvas, rect, top);
                            break;
                        }
                        case BoxShape.rectangle:
                        {
                            if (
                                (borderRadius is not null)
                                && (!Equals(borderRadius, BorderRadius.zero))
                            )
                            {
                                _paintUniformBorderWithRadius(canvas, rect, top, borderRadius);
                                return;
                            }
                            _paintUniformBorderWithRectangle(canvas, rect, top);
                            break;
                        }
                    }
                    return;
                }
            }
        }
        if (_styleIsUniform && Equals(top.style, BorderStyle.none))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => textDirection is not null);
        var (leftLocal, rightLocal) = DartRuntimePrimitives.RequireValue(textDirection) switch
        {
            TextDirection.rtl => ((BorderSide, BorderSide))(end, start),
            TextDirection.ltr => ((BorderSide, BorderSide))(start, end),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        HashSet<Color> visibleColors = _distinctVisibleColors();
        bool hasHairlineBorder = _hasHairlineBorder;
        if (
            (checked(visibleColors.Count) == 1L)
            && !hasHairlineBorder
            && (
                Equals(shape, BoxShape.circle)
                || ((borderRadius is not null) && (!Equals(borderRadius, BorderRadius.zero)))
            )
        )
        {
            paintNonUniformBorder(
                canvas,
                rect,
                shape: shape,
                borderRadius: borderRadius,
                textDirection: DartRuntimePrimitives.RequireValue(textDirection),
                top: Equals(top.style, BorderStyle.none) ? BorderSide.none : top,
                right: Equals(rightLocal.style, BorderStyle.none) ? BorderSide.none : rightLocal,
                bottom: Equals(bottom.style, BorderStyle.none) ? BorderSide.none : bottom,
                left: Equals(leftLocal.style, BorderStyle.none) ? BorderSide.none : leftLocal,
                color: visibleColors.First()
            );
            return;
        }
        if (hasHairlineBorder)
        {
            DartRuntimePrimitives.Assert(() =>
                (borderRadius is null) || Equals(borderRadius, BorderRadius.zero)
            );
        }
        DartRuntimePrimitives.Assert(() => borderRadius is null);
        DartRuntimePrimitives.Assert(() => Equals(shape, BoxShape.rectangle));
        DartRuntimePrimitives.Assert(() =>
            _strokeAlignIsUniform && (top.strokeAlign == BorderSide.strokeAlignInside)
        );
        BordersLibrary.paintBorder(
            canvas,
            rect,
            top: top,
            left: leftLocal,
            bottom: bottom,
            right: rightLocal
        );
    }

    public override bool Equals(object? other)
    {
        var __other = other as BorderDirectional;
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
        return (__other is BorderDirectional)
            && Equals(__other.top, top)
            && Equals(__other.start, start)
            && Equals(__other.end, end)
            && Equals(__other.bottom, bottom);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(top, start, end, bottom);

    public override string ToString()
    {
        var arguments = new List<string>();
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "BorderDirectional")}({string.Join(", ", arguments)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

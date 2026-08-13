// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_border.dart
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

public enum BoxShape
{
    rectangle,
    circle
}

public abstract class BoxBorder : ShapeBorder
{
    protected BoxBorder()
    {
    }

    public static BoxBorder CreateFromLTRB(BorderSide top = default!, BorderSide right = default!, BorderSide bottom = default!, BorderSide left = default!) => new Border(top: top, right: right, bottom: bottom, left: left);

    public static BoxBorder CreateAll(Color color = default!, double width = default!, BorderStyle style = default!, double strokeAlign = default!)
        => Border.CreateAll(color, width, style, strokeAlign);

    public static BoxBorder CreateFromBorderSide(BorderSide side)
        => Border.CreateFromBorderSide(side);

    public static BoxBorder CreateSymmetric(BorderSide vertical = default!, BorderSide horizontal = default!)
        => Border.CreateSymmetric(vertical, horizontal);

    public static BoxBorder CreateFromSTEB(BorderSide top = default!, BorderSide start = default!, BorderSide end = default!, BorderSide bottom = default!) => new BorderDirectional(top: top, start: start, end: end, bottom: bottom);

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
        if ((((a is Border)) && ((b is Border))))
        {
            Border a__as6271 = (Border)a;
            Border b__as6289 = (Border)b;
            return Border.lerp(((Border?)a__as6271), ((Border?)b__as6289), t);
        }
        if ((((a is BorderDirectional)) && ((b is BorderDirectional))))
        {
            BorderDirectional a__as6356 = (BorderDirectional)a;
            BorderDirectional b__as6385 = (BorderDirectional)b;
            return BorderDirectional.lerp(((BorderDirectional?)a__as6356), ((BorderDirectional?)b__as6385), t);
        }
        if (((b is Border) && (a is BorderDirectional)))
        {
            (a, b) = (((Border)b), ((BorderDirectional)a));
            t = (1.0 - t);
        }
        if (((a is Border) && (b is BorderDirectional)))
        {
            Border a__as6605 = (Border)a;
            BorderDirectional b__as6620 = (BorderDirectional)b;
            if (((object.Equals(((BorderDirectional)((BorderDirectional)b__as6620)).start, BorderSide.none)) && (object.Equals(((BorderDirectional)((BorderDirectional)b__as6620)).end, BorderSide.none))))
            {
                return new Border(top: BorderSide.lerp(((Border)((Border)a__as6605)).top, ((BorderDirectional)((BorderDirectional)b__as6620)).top, t), right: BorderSide.lerp(((Border)((Border)a__as6605)).right, BorderSide.none, t), bottom: BorderSide.lerp(((Border)((Border)a__as6605)).bottom, ((BorderDirectional)((BorderDirectional)b__as6620)).bottom, t), left: BorderSide.lerp(((Border)((Border)a__as6605)).left, BorderSide.none, t));
            }
            if (((object.Equals(((Border)((Border)a__as6605)).left, BorderSide.none)) && (object.Equals(((Border)((Border)a__as6605)).right, BorderSide.none))))
            {
                return new BorderDirectional(top: BorderSide.lerp(((Border)((Border)a__as6605)).top, ((BorderDirectional)((BorderDirectional)b__as6620)).top, t), start: BorderSide.lerp(BorderSide.none, ((BorderDirectional)((BorderDirectional)b__as6620)).start, t), end: BorderSide.lerp(BorderSide.none, ((BorderDirectional)((BorderDirectional)b__as6620)).end, t), bottom: BorderSide.lerp(((Border)((Border)a__as6605)).bottom, ((BorderDirectional)((BorderDirectional)b__as6620)).bottom, t));
            }
            if ((t < 0.5))
            {
                return new Border(top: BorderSide.lerp(((Border)((Border)a__as6605)).top, ((BorderDirectional)((BorderDirectional)b__as6620)).top, t), right: BorderSide.lerp(((Border)((Border)a__as6605)).right, BorderSide.none, (t * 2.0)), bottom: BorderSide.lerp(((Border)((Border)a__as6605)).bottom, ((BorderDirectional)((BorderDirectional)b__as6620)).bottom, t), left: BorderSide.lerp(((Border)((Border)a__as6605)).left, BorderSide.none, (t * 2.0)));
            }
            return new BorderDirectional(top: BorderSide.lerp(((Border)((Border)a__as6605)).top, ((BorderDirectional)((BorderDirectional)b__as6620)).top, t), start: BorderSide.lerp(BorderSide.none, ((BorderDirectional)((BorderDirectional)b__as6620)).start, (((t - 0.5)) * 2.0)), end: BorderSide.lerp(BorderSide.none, ((BorderDirectional)((BorderDirectional)b__as6620)).end, (((t - 0.5)) * 2.0)), bottom: BorderSide.lerp(((Border)((Border)a__as6605)).bottom, ((BorderDirectional)((BorderDirectional)b__as6620)).bottom, t));
        }
        ShapeBorder? result__8308 = (b?.lerpFrom(a, t) ?? a?.lerpTo(b, t));
        return (((BoxBorder?)(object?)result__8308)! ?? (((t < 0.5) ? a : b)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => (textDirection is not null));
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRect(dimensions.resolve(textDirection).deflateRect(rect));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => (textDirection is not null));
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return rect.contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRect(rect, paint);
    }

    public override bool preferPaintInterior => true;
    public abstract override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null);
    internal static void _paintUniformBorderWithRadius(Canvas canvas, Rect rect, BorderSide side, BorderRadius borderRadius)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(((BorderSide)side).style, BorderStyle.none)));
        var paint__10876 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.color = ((BorderSide)side).color;
    return __cascade;
}))();
        double width__10930 = ((BorderSide)side).width;
        if ((width__10930 == 0.0))
        {
            ((Func<Paint>)(() =>
{
    var __cascade = paint__10876;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 0.0;
    return __cascade;
}))();
            canvas.drawRRect(borderRadius.toRRect(rect), paint__10876);
        }
        else
        {
            global::Doroti.Flutter.Ui.RRect borderRect__11144 = borderRadius.toRRect(rect);
            global::Doroti.Flutter.Ui.RRect inner__11203 = borderRect__11144.deflate(((BorderSide)side).strokeInset);
            global::Doroti.Flutter.Ui.RRect outer__11267 = borderRect__11144.inflate(((BorderSide)side).strokeOutset);
            canvas.drawDRRect(outer__11267, inner__11203, paint__10876);
        }
    }

    public static void paintNonUniformBorder(Canvas canvas, Rect rect, BorderRadius? borderRadius, TextDirection? textDirection, BoxShape shape = BoxShape.rectangle, BorderSide top = default!, BorderSide right = default!, BorderSide bottom = default!, BorderSide left = default!, Color color = default!)
    {
        global::Doroti.Flutter.Ui.RRect borderRect__12048 = default!;
        switch (shape)
        {
            case BoxShape.rectangle:
                {
                    borderRect__12048 = ((borderRadius ?? BorderRadius.zero)).resolve(textDirection).toRRect(rect);
                    break;
                }
            case BoxShape.circle:
                {
                    DartRuntimePrimitives.Assert(() => (borderRadius is null));
                    borderRect__12048 = global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(global::Doroti.Flutter.Ui.Rect.fromCircle(center: rect.center, radius: (rect.shortestSide / 2.0)), global::Doroti.Flutter.Ui.Radius.circular(rect.width));
                    break;
                }
        }
        var paint__12592 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))();
        global::Doroti.Flutter.Ui.RRect inner__12641 = new EdgeInsets(((BorderSide)left).strokeInset, ((BorderSide)top).strokeInset, ((BorderSide)right).strokeInset, ((BorderSide)bottom).strokeInset).deflateRRect(borderRect__12048);
        global::Doroti.Flutter.Ui.RRect outer__12817 = new EdgeInsets(((BorderSide)left).strokeOutset, ((BorderSide)top).strokeOutset, ((BorderSide)right).strokeOutset, ((BorderSide)bottom).strokeOutset).inflateRRect(borderRect__12048);
        canvas.drawDRRect(outer__12817, inner__12641, paint__12592);
    }

    internal static void _paintUniformBorderWithCircle(Canvas canvas, Rect rect, BorderSide side)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(((BorderSide)side).style, BorderStyle.none)));
        double radius__13180 = (((rect.shortestSide + ((BorderSide)side).strokeOffset)) / 2L);
        canvas.drawCircle(rect.center, radius__13180, side.toPaint());
    }

    internal static void _paintUniformBorderWithRectangle(Canvas canvas, Rect rect, BorderSide side)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(((BorderSide)side).style, BorderStyle.none)));
        canvas.drawRect(rect.inflate((((BorderSide)side).strokeOffset / 2L)), side.toPaint());
    }

}

public class Border : BoxBorder
{
    private BorderSide __field_top = default!;
    public override BorderSide top { get => __field_top; }
    public virtual BorderSide right { get; private set; } = default!;
    private BorderSide __field_bottom = default!;
    public override BorderSide bottom { get => __field_bottom; }
    public virtual BorderSide left { get; private set; } = default!;

    public Border(BorderSide top = default!, BorderSide right = default!, BorderSide bottom = default!, BorderSide left = default!)
    {
        BorderSide __top = top ?? BorderSide.none;
        BorderSide __right = right ?? BorderSide.none;
        BorderSide __bottom = bottom ?? BorderSide.none;
        BorderSide __left = left ?? BorderSide.none;
        this.__field_top = __top;
        this.right = __right;
        this.__field_bottom = __bottom;
        this.left = __left;
    }

    public static Border CreateFromBorderSide(BorderSide side)
    {
        var __instance = new Border(default!, default!, default!, default!);
        __instance.__field_top = side;
        __instance.right = side;
        __instance.__field_bottom = side;
        __instance.left = side;
        return __instance;
    }

    public static Border CreateSymmetric(BorderSide vertical = default!, BorderSide horizontal = default!)
    {
        var __instance = new Border(default!, default!, default!, default!);
        __instance.left = vertical;
        __instance.__field_top = horizontal;
        __instance.right = vertical;
        __instance.__field_bottom = horizontal;
        return __instance;
    }

    public static Border CreateAll(Color color = default!, double width = 1.0, BorderStyle style = BorderStyle.solid, double? strokeAlign = null)
    {
        Color __color = color ?? new Color(0xFF000000);
        double __strokeAlign = strokeAlign ?? BorderSide.strokeAlignInside;
        var side__16940 = new BorderSide(color: __color, width: width, style: style, strokeAlign: DartRuntimePrimitives.RequireValue(__strokeAlign));
        return Border.CreateFromBorderSide(side__16940);
    }

    public static Border merge(Border a, Border b)
    {
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((Border)a).top, ((Border)b).top));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((Border)a).right, ((Border)b).right));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((Border)a).bottom, ((Border)b).bottom));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((Border)a).left, ((Border)b).left));
        return new Border(top: BorderSide.merge(((Border)a).top, ((Border)b).top), right: BorderSide.merge(((Border)a).right, ((Border)b).right), bottom: BorderSide.merge(((Border)a).bottom, ((Border)b).bottom), left: BorderSide.merge(((Border)a).left, ((Border)b).left));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            return new EdgeInsets(((BorderSide)this.left).strokeInset, ((BorderSide)this.top).strokeInset, ((BorderSide)this.right).strokeInset, ((BorderSide)this.bottom).strokeInset);
            return default!;
        }
    }
    public override bool isUniform => (((this._colorIsUniform && this._widthIsUniform) && this._styleIsUniform) && this._strokeAlignIsUniform);
    internal virtual bool _colorIsUniform
    {
        get
        {
            global::Doroti.Flutter.Ui.Color topColor__18335 = ((BorderSide)this.top).color;
            return (((object.Equals(((BorderSide)this.left).color, topColor__18335)) && (object.Equals(((BorderSide)this.bottom).color, topColor__18335))) && (object.Equals(((BorderSide)this.right).color, topColor__18335)));
            return default!;
        }
    }
    internal virtual bool _widthIsUniform
    {
        get
        {
            double topWidth__18498 = ((BorderSide)this.top).width;
            return (((((BorderSide)this.left).width == topWidth__18498) && (((BorderSide)this.bottom).width == topWidth__18498)) && (((BorderSide)this.right).width == topWidth__18498));
            return default!;
        }
    }
    internal virtual bool _styleIsUniform
    {
        get
        {
            BorderStyle topStyle__18666 = ((BorderSide)this.top).style;
            return (((object.Equals(((BorderSide)this.left).style, topStyle__18666)) && (object.Equals(((BorderSide)this.bottom).style, topStyle__18666))) && (object.Equals(((BorderSide)this.right).style, topStyle__18666)));
            return default!;
        }
    }
    internal virtual bool _strokeAlignIsUniform
    {
        get
        {
            double topStrokeAlign__18835 = ((BorderSide)this.top).strokeAlign;
            return (((((BorderSide)this.left).strokeAlign == topStrokeAlign__18835) && (((BorderSide)this.bottom).strokeAlign == topStrokeAlign__18835)) && (((BorderSide)this.right).strokeAlign == topStrokeAlign__18835));
            return default!;
        }
    }
    internal virtual HashSet<global::Doroti.Flutter.Ui.Color> _distinctVisibleColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasHairlineBorder => ((((((object.Equals(((BorderSide)this.top).style, BorderStyle.solid)) && (((BorderSide)this.top).width == 0.0))) || (((object.Equals(((BorderSide)this.right).style, BorderStyle.solid)) && (((BorderSide)this.right).width == 0.0)))) || (((object.Equals(((BorderSide)this.bottom).style, BorderStyle.solid)) && (((BorderSide)this.bottom).width == 0.0)))) || (((object.Equals(((BorderSide)this.left).style, BorderStyle.solid)) && (((BorderSide)this.left).width == 0.0))));
    public override Border? add(ShapeBorder other, bool reversed = false)
    {
        if ((((((other is Border) && BorderSide.canMerge(this.top, ((Border)((Border)other)).top)) && BorderSide.canMerge(this.right, ((Border)((Border)other)).right)) && BorderSide.canMerge(this.bottom, ((Border)((Border)other)).bottom)) && BorderSide.canMerge(this.left, ((Border)((Border)other)).left)))
        {
            Border other__as19873 = (Border)other;
            return Border.merge(this, ((Border)other__as19873));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Border scale(double t)
    {
        return new Border(top: this.top.scale(t), right: this.right.scale(t), bottom: this.bottom.scale(t), left: this.left.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is Border))
        {
            Border a__as20414 = (Border)a;
            return Border.lerp(((Border)a__as20414), this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is Border))
        {
            Border b__as20581 = (Border)b;
            return Border.lerp(this, ((Border)b__as20581), t);
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
        if ((a is null))
        {
            return b!.scale(t);
        }
        if ((b is null))
        {
            return a.scale((1.0 - t));
        }
        return new Border(top: BorderSide.lerp(((Border)a).top, ((Border)b).top, t), right: BorderSide.lerp(((Border)a).right, ((Border)b).right, t), bottom: BorderSide.lerp(((Border)a).bottom, ((Border)b).bottom, t), left: BorderSide.lerp(((Border)a).left, ((Border)b).left, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        if (this.isUniform)
        {
            switch (((BorderSide)this.top).style)
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
                                    DartRuntimePrimitives.Assert(() => (borderRadius is null));
                                    BoxBorder._paintUniformBorderWithCircle(canvas, rect, this.top);
                                    break;
                                }
                            case BoxShape.rectangle:
                                {
                                    if (((borderRadius is not null) && (!object.Equals(borderRadius, BorderRadius.zero))))
                                    {
                                        BoxBorder._paintUniformBorderWithRadius(canvas, rect, this.top, borderRadius);
                                        return;
                                    }
                                    BoxBorder._paintUniformBorderWithRectangle(canvas, rect, this.top);
                                    break;
                                }
                        }
                        return;
                    }
            }
        }
        if ((this._styleIsUniform && (object.Equals(((BorderSide)this.top).style, BorderStyle.none))))
        {
            return;
        }
        HashSet<global::Doroti.Flutter.Ui.Color> visibleColors__23663 = _distinctVisibleColors();
        bool hasHairlineBorder__23720 = this._hasHairlineBorder;
        if ((((checked((long)(visibleColors__23663.Count)) == 1L) && !hasHairlineBorder__23720) && (((object.Equals(shape, BoxShape.circle)) || (((borderRadius is not null) && (!object.Equals(borderRadius, BorderRadius.zero))))))))
        {
            BoxBorder.paintNonUniformBorder(canvas, rect, shape: shape, borderRadius: borderRadius, textDirection: textDirection, top: ((object.Equals(((BorderSide)this.top).style, BorderStyle.none)) ? BorderSide.none : this.top), right: ((object.Equals(((BorderSide)this.right).style, BorderStyle.none)) ? BorderSide.none : this.right), bottom: ((object.Equals(((BorderSide)this.bottom).style, BorderStyle.none)) ? BorderSide.none : this.bottom), left: ((object.Equals(((BorderSide)this.left).style, BorderStyle.none)) ? BorderSide.none : this.left), color: visibleColors__23663.First());
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (hasHairlineBorder__23720)
                {
                    DartRuntimePrimitives.Assert(() => ((borderRadius is null) || (object.Equals(borderRadius, BorderRadius.zero))));
                }
                if (((borderRadius is not null) && (!object.Equals(borderRadius, BorderRadius.zero))))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A borderRadius can only be given on borders with uniform colors."), new ErrorDescription("The following is not uniform:") });
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!object.Equals(shape, BoxShape.rectangle)))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A Border can only be drawn as a circle on borders with uniform colors."), new ErrorDescription("The following is not uniform:") });
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() =>
            {
                if ((!this._strokeAlignIsUniform || (((BorderSide)this.top).strokeAlign != BorderSide.strokeAlignInside)))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A Border can only draw strokeAlign different than BorderSide.strokeAlignInside on borders with uniform colors.") });
                }
                return true;
            });
        global::Doroti.Generated.Framework.Painting.BordersLibrary.paintBorder(canvas, rect, top: this.top, right: this.right, bottom: this.bottom, left: this.left);
    }

    public override bool Equals(object? other)
    {
        var __other = other as Border;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is Border) && (object.Equals(((Border)((Border)__other)).top, this.top))) && (object.Equals(((Border)((Border)__other)).right, this.right))) && (object.Equals(((Border)((Border)__other)).bottom, this.bottom))) && (object.Equals(((Border)((Border)__other)).left, this.left)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.top, this.right, this.bottom, this.left);
    public override string ToString()
    {
        if (this.isUniform)
        {
            return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Border"))}.all({this.top})";
        }
        var arguments__26672 = new List<string>();
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Border"))}({string.Join(", ", arguments__26672)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BorderDirectional : BoxBorder
{
    private BorderSide __field_top = default!;
    public override BorderSide top { get => __field_top; }
    public virtual BorderSide start { get; private set; } = default!;
    public virtual BorderSide end { get; private set; } = default!;
    private BorderSide __field_bottom = default!;
    public override BorderSide bottom { get => __field_bottom; }

    public BorderDirectional(BorderSide top = default!, BorderSide start = default!, BorderSide end = default!, BorderSide bottom = default!)
    {
        BorderSide __top = top ?? BorderSide.none;
        BorderSide __start = start ?? BorderSide.none;
        BorderSide __end = end ?? BorderSide.none;
        BorderSide __bottom = bottom ?? BorderSide.none;
        this.__field_top = __top;
        this.start = __start;
        this.end = __end;
        this.__field_bottom = __bottom;
    }

    public static BorderDirectional merge(BorderDirectional a, BorderDirectional b)
    {
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((BorderDirectional)a).top, ((BorderDirectional)b).top));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((BorderDirectional)a).start, ((BorderDirectional)b).start));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((BorderDirectional)a).end, ((BorderDirectional)b).end));
        DartRuntimePrimitives.Assert(() => BorderSide.canMerge(((BorderDirectional)a).bottom, ((BorderDirectional)b).bottom));
        return new BorderDirectional(top: BorderSide.merge(((BorderDirectional)a).top, ((BorderDirectional)b).top), start: BorderSide.merge(((BorderDirectional)a).start, ((BorderDirectional)b).start), end: BorderSide.merge(((BorderDirectional)a).end, ((BorderDirectional)b).end), bottom: BorderSide.merge(((BorderDirectional)a).bottom, ((BorderDirectional)b).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            return new EdgeInsetsDirectional(((BorderSide)this.start).strokeInset, ((BorderSide)this.top).strokeInset, ((BorderSide)this.end).strokeInset, ((BorderSide)this.bottom).strokeInset);
            return default!;
        }
    }
    public override bool isUniform => (((this._colorIsUniform && this._widthIsUniform) && this._styleIsUniform) && this._strokeAlignIsUniform);
    internal virtual bool _colorIsUniform
    {
        get
        {
            global::Doroti.Flutter.Ui.Color topColor__30476 = ((BorderSide)this.top).color;
            return (((object.Equals(((BorderSide)this.start).color, topColor__30476)) && (object.Equals(((BorderSide)this.bottom).color, topColor__30476))) && (object.Equals(((BorderSide)this.end).color, topColor__30476)));
            return default!;
        }
    }
    internal virtual bool _widthIsUniform
    {
        get
        {
            double topWidth__30638 = ((BorderSide)this.top).width;
            return (((((BorderSide)this.start).width == topWidth__30638) && (((BorderSide)this.bottom).width == topWidth__30638)) && (((BorderSide)this.end).width == topWidth__30638));
            return default!;
        }
    }
    internal virtual bool _styleIsUniform
    {
        get
        {
            BorderStyle topStyle__30805 = ((BorderSide)this.top).style;
            return (((object.Equals(((BorderSide)this.start).style, topStyle__30805)) && (object.Equals(((BorderSide)this.bottom).style, topStyle__30805))) && (object.Equals(((BorderSide)this.end).style, topStyle__30805)));
            return default!;
        }
    }
    internal virtual bool _strokeAlignIsUniform
    {
        get
        {
            double topStrokeAlign__30973 = ((BorderSide)this.top).strokeAlign;
            return (((((BorderSide)this.start).strokeAlign == topStrokeAlign__30973) && (((BorderSide)this.bottom).strokeAlign == topStrokeAlign__30973)) && (((BorderSide)this.end).strokeAlign == topStrokeAlign__30973));
            return default!;
        }
    }
    internal virtual HashSet<global::Doroti.Flutter.Ui.Color> _distinctVisibleColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasHairlineBorder => ((((((object.Equals(((BorderSide)this.top).style, BorderStyle.solid)) && (((BorderSide)this.top).width == 0.0))) || (((object.Equals(((BorderSide)this.end).style, BorderStyle.solid)) && (((BorderSide)this.end).width == 0.0)))) || (((object.Equals(((BorderSide)this.bottom).style, BorderStyle.solid)) && (((BorderSide)this.bottom).width == 0.0)))) || (((object.Equals(((BorderSide)this.start).style, BorderStyle.solid)) && (((BorderSide)this.start).width == 0.0))));
    public override BoxBorder? add(ShapeBorder other, bool reversed = false)
    {
        if ((other is BorderDirectional))
        {
            BorderDirectional other__as31816 = (BorderDirectional)other;
            BorderDirectional typedOther__31876 = ((BorderDirectional)other__as31816);
            if ((((BorderSide.canMerge(this.top, ((BorderDirectional)typedOther__31876).top) && BorderSide.canMerge(this.start, ((BorderDirectional)typedOther__31876).start)) && BorderSide.canMerge(this.end, ((BorderDirectional)typedOther__31876).end)) && BorderSide.canMerge(this.bottom, ((BorderDirectional)typedOther__31876).bottom)))
            {
                return BorderDirectional.merge(this, typedOther__31876);
            }
            return null;
        }
        if ((other is Border))
        {
            Border other__as32221 = (Border)other;
            Border typedOther__32259 = ((Border)other__as32221);
            if ((!BorderSide.canMerge(((Border)typedOther__32259).top, this.top) || !BorderSide.canMerge(((Border)typedOther__32259).bottom, this.bottom)))
            {
                return null;
            }
            if (((!object.Equals(this.start, BorderSide.none)) || (!object.Equals(this.end, BorderSide.none))))
            {
                if (((!object.Equals(((Border)typedOther__32259).left, BorderSide.none)) || (!object.Equals(((Border)typedOther__32259).right, BorderSide.none))))
                {
                    return null;
                }
                DartRuntimePrimitives.Assert(() => (object.Equals(((Border)typedOther__32259).left, BorderSide.none)));
                DartRuntimePrimitives.Assert(() => (object.Equals(((Border)typedOther__32259).right, BorderSide.none)));
                return new BorderDirectional(top: BorderSide.merge(((Border)typedOther__32259).top, this.top), start: this.start, end: this.end, bottom: BorderSide.merge(((Border)typedOther__32259).bottom, this.bottom));
            }
            DartRuntimePrimitives.Assert(() => (object.Equals(this.start, BorderSide.none)));
            DartRuntimePrimitives.Assert(() => (object.Equals(this.end, BorderSide.none)));
            return new Border(top: BorderSide.merge(((Border)typedOther__32259).top, this.top), right: ((Border)typedOther__32259).right, bottom: BorderSide.merge(((Border)typedOther__32259).bottom, this.bottom), left: ((Border)typedOther__32259).left);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BorderDirectional scale(double t)
    {
        return new BorderDirectional(top: this.top.scale(t), start: this.start.scale(t), end: this.end.scale(t), bottom: this.bottom.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is BorderDirectional))
        {
            BorderDirectional a__as33516 = (BorderDirectional)a;
            return BorderDirectional.lerp(((BorderDirectional)a__as33516), this, t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is BorderDirectional))
        {
            BorderDirectional b__as33705 = (BorderDirectional)b;
            return BorderDirectional.lerp(this, ((BorderDirectional)b__as33705), t);
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
        if ((a is null))
        {
            return b!.scale(t);
        }
        if ((b is null))
        {
            return a.scale((1.0 - t));
        }
        return new BorderDirectional(top: BorderSide.lerp(((BorderDirectional)a).top, ((BorderDirectional)b).top, t), end: BorderSide.lerp(((BorderDirectional)a).end, ((BorderDirectional)b).end, t), bottom: BorderSide.lerp(((BorderDirectional)a).bottom, ((BorderDirectional)b).bottom, t), start: BorderSide.lerp(((BorderDirectional)a).start, ((BorderDirectional)b).start, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        if (this.isUniform)
        {
            switch (((BorderSide)this.top).style)
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
                                    DartRuntimePrimitives.Assert(() => (borderRadius is null));
                                    BoxBorder._paintUniformBorderWithCircle(canvas, rect, this.top);
                                    break;
                                }
                            case BoxShape.rectangle:
                                {
                                    if (((borderRadius is not null) && (!object.Equals(borderRadius, BorderRadius.zero))))
                                    {
                                        BoxBorder._paintUniformBorderWithRadius(canvas, rect, this.top, borderRadius);
                                        return;
                                    }
                                    BoxBorder._paintUniformBorderWithRectangle(canvas, rect, this.top);
                                    break;
                                }
                        }
                        return;
                    }
            }
        }
        if ((this._styleIsUniform && (object.Equals(((BorderSide)this.top).style, BorderStyle.none))))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (textDirection is not null));
        var (left__36767, right__36784) = (DartRuntimePrimitives.RequireValue(textDirection) switch { TextDirection.rtl => (((BorderSide, BorderSide))((this.end, this.start))), TextDirection.ltr => (((BorderSide, BorderSide))((this.start, this.end))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        HashSet<global::Doroti.Flutter.Ui.Color> visibleColors__37007 = _distinctVisibleColors();
        bool hasHairlineBorder__37064 = this._hasHairlineBorder;
        if ((((checked((long)(visibleColors__37007.Count)) == 1L) && !hasHairlineBorder__37064) && (((object.Equals(shape, BoxShape.circle)) || (((borderRadius is not null) && (!object.Equals(borderRadius, BorderRadius.zero))))))))
        {
            BoxBorder.paintNonUniformBorder(canvas, rect, shape: shape, borderRadius: borderRadius, textDirection: DartRuntimePrimitives.RequireValue(textDirection), top: ((object.Equals(((BorderSide)this.top).style, BorderStyle.none)) ? BorderSide.none : this.top), right: ((object.Equals(((BorderSide)right__36784).style, BorderStyle.none)) ? BorderSide.none : right__36784), bottom: ((object.Equals(((BorderSide)this.bottom).style, BorderStyle.none)) ? BorderSide.none : this.bottom), left: ((object.Equals(((BorderSide)left__36767).style, BorderStyle.none)) ? BorderSide.none : left__36767), color: visibleColors__37007.First());
            return;
        }
        if (hasHairlineBorder__37064)
        {
            DartRuntimePrimitives.Assert(() => ((borderRadius is null) || (object.Equals(borderRadius, BorderRadius.zero))));
        }
        DartRuntimePrimitives.Assert(() => (borderRadius is null));
        DartRuntimePrimitives.Assert(() => (object.Equals(shape, BoxShape.rectangle)));
        DartRuntimePrimitives.Assert(() => (this._strokeAlignIsUniform && (((BorderSide)this.top).strokeAlign == BorderSide.strokeAlignInside)));
        global::Doroti.Generated.Framework.Painting.BordersLibrary.paintBorder(canvas, rect, top: this.top, left: left__36767, bottom: this.bottom, right: right__36784);
    }

    public override bool Equals(object? other)
    {
        var __other = other as BorderDirectional;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is BorderDirectional) && (object.Equals(((BorderDirectional)((BorderDirectional)__other)).top, this.top))) && (object.Equals(((BorderDirectional)((BorderDirectional)__other)).start, this.start))) && (object.Equals(((BorderDirectional)((BorderDirectional)__other)).end, this.end))) && (object.Equals(((BorderDirectional)((BorderDirectional)__other)).bottom, this.bottom)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.top, this.start, this.end, this.bottom);
    public override string ToString()
    {
        var arguments__39057 = new List<string>();
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "BorderDirectional"))}({string.Join(", ", arguments__39057)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/linear_border.dart
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

public class LinearBorderEdge
{
    public virtual double size { get; private set; } = default!;
    public virtual double alignment { get; private set; } = default!;

    public LinearBorderEdge(double size = 1.0, double alignment = 0.0)
    {
        this.size = size;
        this.alignment = alignment;
        System.Diagnostics.Debug.Assert(((size >= 0.0) && (size <= 1.0)));
    }

    public static LinearBorderEdge? lerp(LinearBorderEdge? a, LinearBorderEdge? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        a ??= new LinearBorderEdge(alignment: b!.alignment, size: 0);
        b ??= new LinearBorderEdge(alignment: ((LinearBorderEdge)a).alignment, size: 0);
        return new LinearBorderEdge(size: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((LinearBorderEdge)a).size, ((LinearBorderEdge)b).size, t)), alignment: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((LinearBorderEdge)a).alignment, ((LinearBorderEdge)b).alignment, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as LinearBorderEdge;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is LinearBorderEdge) && (((LinearBorderEdge)((LinearBorderEdge)__other)).size == this.size)) && (((LinearBorderEdge)((LinearBorderEdge)__other)).alignment == this.alignment));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.size, this.alignment);
    public override string ToString()
    {
        var s__3252 = new StringBuffer($"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearBorderEdge"))}(");
        if ((this.size != 1.0))
        {
            s__3252.write($"size: {this.size}");
        }
        if ((this.alignment != 0L))
        {
            var comma__3418 = ((this.size != 1.0) ? ", " : "");
            s__3252.write($"{comma__3418}alignment: {this.alignment}");
        }
        s__3252.write(")");
        return s__3252.ToString();
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

    public LinearBorder(BorderSide side = default!, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null) : base(side: side ?? BorderSide.none)
    {
        this.start = start;
        this.end = end;
        this.top = top;
        this.bottom = bottom;
    }

    public static LinearBorder CreateStart(BorderSide side = default!, double alignment = 0.0, double size = 1.0)
    {
        var __instance = new LinearBorder(default!, default!, default!, default!, default!);
        __instance.start = new LinearBorderEdge(alignment: alignment, size: size);
        __instance.end = null;
        __instance.top = null;
        __instance.bottom = null;
        return __instance;
    }

    public static LinearBorder CreateEnd(BorderSide side = default!, double alignment = 0.0, double size = 1.0)
    {
        var __instance = new LinearBorder(default!, default!, default!, default!, default!);
        __instance.start = null;
        __instance.end = new LinearBorderEdge(alignment: alignment, size: size);
        __instance.top = null;
        __instance.bottom = null;
        return __instance;
    }

    public static LinearBorder CreateTop(BorderSide side = default!, double alignment = 0.0, double size = 1.0)
    {
        var __instance = new LinearBorder(default!, default!, default!, default!, default!);
        __instance.start = null;
        __instance.end = null;
        __instance.top = new LinearBorderEdge(alignment: alignment, size: size);
        __instance.bottom = null;
        return __instance;
    }

    public static LinearBorder CreateBottom(BorderSide side = default!, double alignment = 0.0, double size = 1.0)
    {
        var __instance = new LinearBorder(default!, default!, default!, default!, default!);
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
            double width__7190 = ((BorderSide)side).width;
            return new EdgeInsetsDirectional(((this.start is null) ? 0.0 : width__7190), ((this.top is null) ? 0.0 : width__7190), ((this.end is null) ? 0.0 : width__7190), ((this.bottom is null) ? 0.0 : width__7190));
            return default!;
        }
    }
    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is LinearBorder))
        {
            LinearBorder a__as7474 = (LinearBorder)a;
            return new LinearBorder(side: BorderSide.lerp(((LinearBorder)a__as7474).side, side, t), start: LinearBorderEdge.lerp(((LinearBorder)((LinearBorder)a__as7474)).start, this.start, t), end: LinearBorderEdge.lerp(((LinearBorder)((LinearBorder)a__as7474)).end, this.end, t), top: LinearBorderEdge.lerp(((LinearBorder)((LinearBorder)a__as7474)).top, this.top, t), bottom: LinearBorderEdge.lerp(((LinearBorder)((LinearBorder)a__as7474)).bottom, this.bottom, t));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is LinearBorder))
        {
            LinearBorder b__as7912 = (LinearBorder)b;
            return new LinearBorder(side: BorderSide.lerp(side, ((LinearBorder)b__as7912).side, t), start: LinearBorderEdge.lerp(this.start, ((LinearBorder)((LinearBorder)b__as7912)).start, t), end: LinearBorderEdge.lerp(this.end, ((LinearBorder)((LinearBorder)b__as7912)).end, t), top: LinearBorderEdge.lerp(this.top, ((LinearBorder)((LinearBorder)b__as7912)).top, t), bottom: LinearBorderEdge.lerp(this.bottom, ((LinearBorder)((LinearBorder)b__as7912)).bottom, t));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override LinearBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new LinearBorder(side: (side ?? this.side), start: (start ?? this.start), end: (end ?? this.end), top: (top ?? this.top), bottom: (bottom ?? this.bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        global::Doroti.Ui.Rect adjustedRect__8845 = this.dimensions.resolve(textDirection).deflateRect(rect);
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(adjustedRect__8845);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        EdgeInsets insets__9182 = this.dimensions.resolve(textDirection);
        var rtl__9236 = (object.Equals(textDirection, TextDirection.rtl));
        var path__9289 = new global::Doroti.Ui.Path();
        var paint__9314 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.strokeWidth = 0.0;
    return __cascade;
}))();
        void drawEdge(Rect rect, Color color)
        {
            paint__9314.color = color;
            path__9289.reset();
            path__9289.moveTo(rect.left, DartRuntimePrimitives.RequireValue(rect.top));
            if ((rect.width == 0.0))
            {
                paint__9314.style = PaintingStyle.stroke;
                path__9289.lineTo(rect.left, DartRuntimePrimitives.RequireValue(rect.bottom));
            }
            else
            {
                if ((rect.height == 0.0))
                {
                    paint__9314.style = PaintingStyle.stroke;
                    path__9289.lineTo(rect.right, DartRuntimePrimitives.RequireValue(rect.top));
                }
                else
                {
                    paint__9314.style = PaintingStyle.fill;
                    path__9289.lineTo(rect.right, DartRuntimePrimitives.RequireValue(rect.top));
                    path__9289.lineTo(rect.right, DartRuntimePrimitives.RequireValue(rect.bottom));
                    path__9289.lineTo(rect.left, DartRuntimePrimitives.RequireValue(rect.bottom));
                }
            }
            canvas.drawPath(path__9289, paint__9314);
        }
        if ((((this.start is not null) && (this.start!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            var insetRect__10063 = global::Doroti.Ui.Rect.fromLTWH(rect.left, (rect.top + ((EdgeInsets)insets__9182).top), rect.width, (rect.height - insets__9182.vertical));
            double x__10227 = (rtl__9236 ? (rect.right - ((EdgeInsets)insets__9182).right) : rect.left);
            double width__10295 = (rtl__9236 ? ((EdgeInsets)insets__9182).right : ((EdgeInsets)insets__9182).left);
            double height__10356 = (insetRect__10063.height * this.start!.size);
            double y__10416 = (((insetRect__10063.height - height__10356)) * ((((this.start!.alignment + 1.0)) / 2.0)));
            var r__10496 = global::Doroti.Ui.Rect.fromLTWH(x__10227, y__10416, width__10295, height__10356);
            drawEdge(r__10496, ((BorderSide)side).color);
        }
        if ((((this.end is not null) && (this.end!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            var insetRect__10663 = global::Doroti.Ui.Rect.fromLTWH(rect.left, (rect.top + ((EdgeInsets)insets__9182).top), rect.width, (rect.height - insets__9182.vertical));
            double x__10827 = (rtl__9236 ? rect.left : (rect.right - ((EdgeInsets)insets__9182).right));
            double width__10895 = (rtl__9236 ? ((EdgeInsets)insets__9182).left : ((EdgeInsets)insets__9182).right);
            double height__10956 = (insetRect__10663.height * this.end!.size);
            double y__11014 = (((insetRect__10663.height - height__10956)) * ((((this.end!.alignment + 1.0)) / 2.0)));
            var r__11092 = global::Doroti.Ui.Rect.fromLTWH(x__10827, y__11014, width__10895, height__10956);
            drawEdge(r__11092, ((BorderSide)side).color);
        }
        if ((((this.top is not null) && (this.top!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            double width__11266 = (rect.width * this.top!.size);
            double startX__11317 = (((rect.width - width__11266)) * ((((this.top!.alignment + 1.0)) / 2.0)));
            double x__11400 = (rtl__9236 ? ((rect.width - startX__11317) - width__11266) : startX__11317);
            var r__11460 = global::Doroti.Ui.Rect.fromLTWH(x__11400, rect.top, width__11266, ((EdgeInsets)insets__9182).top);
            drawEdge(r__11460, ((BorderSide)side).color);
        }
        if ((((this.bottom is not null) && (this.bottom!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            double width__11651 = (rect.width * this.bottom!.size);
            double startX__11705 = (((rect.width - width__11651)) * ((((this.bottom!.alignment + 1.0)) / 2.0)));
            double x__11791 = (rtl__9236 ? ((rect.width - startX__11705) - width__11651) : startX__11705);
            var r__11851 = global::Doroti.Ui.Rect.fromLTWH(x__11791, (rect.bottom - ((EdgeInsets)insets__9182).bottom), width__11651, ((BorderSide)side).width);
            drawEdge(r__11851, ((BorderSide)side).color);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as LinearBorder;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((__other is LinearBorder) && (object.Equals(((LinearBorder)__other).side, side))) && (object.Equals(((LinearBorder)((LinearBorder)__other)).start, this.start))) && (object.Equals(((LinearBorder)((LinearBorder)__other)).end, this.end))) && (object.Equals(((LinearBorder)((LinearBorder)__other)).top, this.top))) && (object.Equals(((LinearBorder)((LinearBorder)__other)).bottom, this.bottom)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.start, this.end, this.top, this.bottom);
    public override string ToString()
    {
        if ((object.Equals(this, LinearBorder.none)))
        {
            return "LinearBorder.none";
        }
        var s__12531 = new StringBuffer($"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearBorder"))}(side: {side}");
        if ((this.start is not null))
        {
            s__12531.write($", start: {this.start}");
        }
        if ((this.end is not null))
        {
            s__12531.write($", end: {this.end}");
        }
        if ((this.top is not null))
        {
            s__12531.write($", top: {this.top}");
        }
        if ((this.bottom is not null))
        {
            s__12531.write($", bottom: {this.bottom}");
        }
        s__12531.write(")");
        return s__12531.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


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

namespace Doroti.Framework.Painting;

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
        var s = new StringBuffer($"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearBorderEdge"))}(");
        if ((this.size != 1.0))
        {
            s.write($"size: {this.size}");
        }
        if ((this.alignment != 0L))
        {
            var comma = ((this.size != 1.0) ? ", " : "");
            s.write($"{comma}alignment: {this.alignment}");
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
            double widthLocal = ((BorderSide)side).width;
            return new EdgeInsetsDirectional(((this.start is null) ? 0.0 : widthLocal), ((this.top is null) ? 0.0 : widthLocal), ((this.end is null) ? 0.0 : widthLocal), ((this.bottom is null) ? 0.0 : widthLocal));
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
        global::Doroti.Ui.Rect adjustedRect = this.dimensions.resolve(textDirection).deflateRect(rect);
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(adjustedRect);
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
        EdgeInsets insets = this.dimensions.resolve(textDirection);
        var rtlLocal = (object.Equals(textDirection, TextDirection.rtl));
        var path = new global::Doroti.Ui.Path();
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.strokeWidth = 0.0;
    return __cascade;
}))();
        void drawEdge(Rect rect, Color color)
        {
            paintLocal.color = color;
            path.reset();
            path.moveTo(rect.left, DartRuntimePrimitives.RequireValue(rect.top));
            if ((rect.width == 0.0))
            {
                paintLocal.style = PaintingStyle.stroke;
                path.lineTo(rect.left, DartRuntimePrimitives.RequireValue(rect.bottom));
            }
            else
            {
                if ((rect.height == 0.0))
                {
                    paintLocal.style = PaintingStyle.stroke;
                    path.lineTo(rect.right, DartRuntimePrimitives.RequireValue(rect.top));
                }
                else
                {
                    paintLocal.style = PaintingStyle.fill;
                    path.lineTo(rect.right, DartRuntimePrimitives.RequireValue(rect.top));
                    path.lineTo(rect.right, DartRuntimePrimitives.RequireValue(rect.bottom));
                    path.lineTo(rect.left, DartRuntimePrimitives.RequireValue(rect.bottom));
                }
            }
            canvas.drawPath(path, paintLocal);
        }
        if ((((this.start is not null) && (this.start!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            var insetRect = global::Doroti.Ui.Rect.fromLTWH(rect.left, (rect.top + ((EdgeInsets)insets).top), rect.width, (rect.height - insets.vertical));
            double x = (rtlLocal ? (rect.right - ((EdgeInsets)insets).right) : rect.left);
            double widthLocal = (rtlLocal ? ((EdgeInsets)insets).right : ((EdgeInsets)insets).left);
            double heightLocal = (insetRect.height * this.start!.size);
            double y = (((insetRect.height - heightLocal)) * ((((this.start!.alignment + 1.0)) / 2.0)));
            var r = global::Doroti.Ui.Rect.fromLTWH(x, y, widthLocal, heightLocal);
            drawEdge(r, ((BorderSide)side).color);
        }
        if ((((this.end is not null) && (this.end!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            var insetRectLocal = global::Doroti.Ui.Rect.fromLTWH(rect.left, (rect.top + ((EdgeInsets)insets).top), rect.width, (rect.height - insets.vertical));
            double xLocal = (rtlLocal ? rect.left : (rect.right - ((EdgeInsets)insets).right));
            double widthAlternate = (rtlLocal ? ((EdgeInsets)insets).left : ((EdgeInsets)insets).right);
            double heightAlternate = (insetRectLocal.height * this.end!.size);
            double yLocal = (((insetRectLocal.height - heightAlternate)) * ((((this.end!.alignment + 1.0)) / 2.0)));
            var rLocal = global::Doroti.Ui.Rect.fromLTWH(xLocal, yLocal, widthAlternate, heightAlternate);
            drawEdge(rLocal, ((BorderSide)side).color);
        }
        if ((((this.top is not null) && (this.top!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            double widthNested = (rect.width * this.top!.size);
            double startX = (((rect.width - widthNested)) * ((((this.top!.alignment + 1.0)) / 2.0)));
            double xAlternate = (rtlLocal ? ((rect.width - startX) - widthNested) : startX);
            var rAlternate = global::Doroti.Ui.Rect.fromLTWH(xAlternate, rect.top, widthNested, ((EdgeInsets)insets).top);
            drawEdge(rAlternate, ((BorderSide)side).color);
        }
        if ((((this.bottom is not null) && (this.bottom!.size != 0.0)) && (!object.Equals(((BorderSide)side).style, BorderStyle.none))))
        {
            double widthCurrent = (rect.width * this.bottom!.size);
            double startXLocal = (((rect.width - widthCurrent)) * ((((this.bottom!.alignment + 1.0)) / 2.0)));
            double xNested = (rtlLocal ? ((rect.width - startXLocal) - widthCurrent) : startXLocal);
            var rNested = global::Doroti.Ui.Rect.fromLTWH(xNested, (rect.bottom - ((EdgeInsets)insets).bottom), widthCurrent, ((BorderSide)side).width);
            drawEdge(rNested, ((BorderSide)side).color);
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
        var s = new StringBuffer($"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "LinearBorder"))}(side: {side}");
        if ((this.start is not null))
        {
            s.write($", start: {this.start}");
        }
        if ((this.end is not null))
        {
            s.write($", end: {this.end}");
        }
        if ((this.top is not null))
        {
            s.write($", top: {this.top}");
        }
        if ((this.bottom is not null))
        {
            s.write($", bottom: {this.bottom}");
        }
        s.write(")");
        return s.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}


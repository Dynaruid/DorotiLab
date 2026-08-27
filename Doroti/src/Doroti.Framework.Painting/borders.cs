// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/borders.dart
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

public enum BorderStyle
{
    none,
    solid
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

    public BorderSide(Color color = default!, double width = 1.0, BorderStyle style = BorderStyle.solid, double? strokeAlign = null)
    {
        Color __color = color ?? new Color(0xFF000000);
        double __strokeAlign = strokeAlign ?? strokeAlignInside;
        this.color = __color;
        this.width = width;
        this.style = style;
        this.strokeAlign = __strokeAlign;
        System.Diagnostics.Debug.Assert((width >= 0.0));
    }

    public static BorderSide merge(BorderSide a, BorderSide b)
    {
        DartRuntimePrimitives.Assert(() => canMerge(a, b));
        bool aIsNone = ((object.Equals(((BorderSide)a).style, BorderStyle.none)) && (((BorderSide)a).width == 0.0));
        bool bIsNone = ((object.Equals(((BorderSide)b).style, BorderStyle.none)) && (((BorderSide)b).width == 0.0));
        if ((aIsNone && bIsNone))
        {
            return BorderSide.none;
        }
        if (aIsNone)
        {
            return b;
        }
        if (bIsNone)
        {
            return a;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((BorderSide)a).color, ((BorderSide)b).color)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((BorderSide)a).style, ((BorderSide)b).style)));
        return new BorderSide(color: ((BorderSide)a).color, width: (((BorderSide)a).width + ((BorderSide)b).width), strokeAlign: Math.Max(DartRuntimePrimitives.RequireValue(((BorderSide)a).strokeAlign), DartRuntimePrimitives.RequireValue(((BorderSide)b).strokeAlign)), style: ((BorderSide)a).style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderSide copyWith(Color? color = null, double? width = null, BorderStyle? style = null, double? strokeAlign = null)
    {
        return new BorderSide(color: (color ?? this.color), width: (width ?? this.width), style: (style ?? this.style), strokeAlign: (strokeAlign ?? this.strokeAlign));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual BorderSide scale(double t)
    {
        return new BorderSide(color: this.color, width: Math.Max(0.0, (this.width * t)), style: ((t <= 0.0) ? BorderStyle.none : this.style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Paint toPaint()
    {
        switch (this.style)
        {
            case BorderStyle.solid:
                {
                    return ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color;
    __cascade.strokeWidth = this.width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                }
            case BorderStyle.none:
                {
                    return ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(0L);
    __cascade.strokeWidth = 0.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool canMerge(BorderSide a, BorderSide b)
    {
        if (((((object.Equals(((BorderSide)a).style, BorderStyle.none)) && (((BorderSide)a).width == 0.0))) || (((object.Equals(((BorderSide)b).style, BorderStyle.none)) && (((BorderSide)b).width == 0.0)))))
        {
            return true;
        }
        return ((object.Equals(((BorderSide)a).style, ((BorderSide)b).style)) && (object.Equals(((BorderSide)a).color, ((BorderSide)b).color)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BorderSide lerp(BorderSide a, BorderSide b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((t == 0.0))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        double widthLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BorderSide)a).width, ((BorderSide)b).width, t));
        if ((DartRuntimePrimitives.RequireValue(widthLocal) < 0.0))
        {
            return BorderSide.none;
        }
        if (((object.Equals(((BorderSide)a).style, ((BorderSide)b).style)) && (((BorderSide)a).strokeAlign == ((BorderSide)b).strokeAlign)))
        {
            return new BorderSide(color: Dart_uiLibrary.Color.lerp(((BorderSide)a).color, ((BorderSide)b).color, t)!, width: DartRuntimePrimitives.RequireValue(widthLocal), style: ((BorderSide)a).style, strokeAlign: DartRuntimePrimitives.RequireValue(((BorderSide)a).strokeAlign));
        }
        global::Doroti.Ui.Color colorA = (((BorderSide)a).style switch { BorderStyle.solid => ((BorderSide)a).color, BorderStyle.none => ((BorderSide)a).color.withAlpha(0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Color colorB = (((BorderSide)b).style switch { BorderStyle.solid => ((BorderSide)b).color, BorderStyle.none => ((BorderSide)b).color.withAlpha(0L), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if ((((BorderSide)a).strokeAlign != ((BorderSide)b).strokeAlign))
        {
            return new BorderSide(color: Dart_uiLibrary.Color.lerp(colorA, colorB, t)!, width: DartRuntimePrimitives.RequireValue(widthLocal), strokeAlign: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(DartRuntimePrimitives.RequireValue(((BorderSide)a).strokeAlign), DartRuntimePrimitives.RequireValue(((BorderSide)b).strokeAlign), t)));
        }
        return new BorderSide(color: Dart_uiLibrary.Color.lerp(colorA, colorB, t)!, width: DartRuntimePrimitives.RequireValue(widthLocal), strokeAlign: DartRuntimePrimitives.RequireValue(((BorderSide)a).strokeAlign));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double strokeInset => (this.width * ((1L - (((1L + this.strokeAlign)) / 2L))));
    public virtual double strokeOutset => ((this.width * ((1L + this.strokeAlign))) / 2L);
    public virtual double strokeOffset => (this.width * this.strokeAlign);
    public override bool Equals(object? other)
    {
        var __other = other as BorderSide;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is BorderSide) && (object.Equals(((BorderSide)((BorderSide)__other)).color, this.color))) && (((BorderSide)((BorderSide)__other)).width == this.width)) && (object.Equals(((BorderSide)((BorderSide)__other)).style, this.style))) && (((BorderSide)((BorderSide)__other)).strokeAlign == this.strokeAlign));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.color, this.width, this.style, DartRuntimePrimitives.RequireValue(this.strokeAlign));
    public virtual string toStringShort() => "BorderSide";
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Color>("color", this.color, defaultValue: new global::Doroti.Ui.Color(4278190080L)));
        properties.add(new DoubleProperty("width", this.width, defaultValue: 1.0));
        properties.add(new DoubleProperty("strokeAlign", DartRuntimePrimitives.RequireValue(this.strokeAlign), defaultValue: strokeAlignInside));
        properties.add(new EnumProperty<BorderStyle>("style", this.style, defaultValue: BorderStyle.solid));
    }

}

public abstract class ShapeBorder
{
    protected ShapeBorder()
    {
    }

    public abstract EdgeInsetsGeometry dimensions { get; }
    public virtual ShapeBorder? add(ShapeBorder other, bool reversed = false) => null;
    public virtual ShapeBorder op_Add(ShapeBorder other)
    {
        return ((add(other) ?? other.add(this, reversed: true)) ?? new _CompoundBorder__borders(new List<ShapeBorder> { other, this }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract ShapeBorder scale(double t);
    public virtual ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is null))
        {
            return scale(t);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is null))
        {
            return scale((1.0 - t));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ShapeBorder? lerp(ShapeBorder? a, ShapeBorder? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        ShapeBorder? result = (((b?.lerpFrom(a, t) ?? a?.lerpTo(b, t)) ?? b?.lerpTo(a, (1.0 - t))) ?? a?.lerpFrom(b, (1.0 - t)));
        return (result ?? (((t < 0.5) ? a : b)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract global::Doroti.Ui.Path getOuterPath(Rect rect, TextDirection? textDirection = null);
    public abstract global::Doroti.Ui.Path getInnerPath(Rect rect, TextDirection? textDirection = null);
    public virtual bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return getOuterPath(rect, textDirection: textDirection).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => !this.preferPaintInterior);
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual bool preferPaintInterior => false;
    public virtual void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null) { }
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ShapeBorder"))}()";
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public override EdgeInsetsGeometry dimensions => EdgeInsets.CreateAll(Math.Max(((BorderSide)this.side).strokeInset, 0));
    public abstract OutlinedBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null);
    public abstract override ShapeBorder scale(double t);
    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is null))
        {
            return scale(t);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is null))
        {
            return scale((1.0 - t));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static OutlinedBorder? lerp(OutlinedBorder? a, OutlinedBorder? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        ShapeBorder? result = (((b?.lerpFrom(a, t) ?? a?.lerpTo(b, t)) ?? b?.lerpTo(a, (1.0 - t))) ?? a?.lerpFrom(b, (1.0 - t)));
        return (((OutlinedBorder?)(object?)result)! ?? (((t < 0.5) ? a : b)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CompoundBorder__borders : ShapeBorder
{
    public virtual List<ShapeBorder> borders { get; private set; } = default!;

    internal _CompoundBorder__borders(List<ShapeBorder> borders)
    {
        this.borders = borders;
        System.Diagnostics.Debug.Assert((checked((long)(borders.Count)) >= 2L));
        System.Diagnostics.Debug.Assert(!borders.any(((border) => (border is _CompoundBorder__borders))));
    }

    public override EdgeInsetsGeometry dimensions
    {
        get
        {
            return System.Linq.Enumerable.Aggregate(this.borders, (EdgeInsetsGeometry)EdgeInsets.zero, ((previousValue, border) =>
            {
                return previousValue.add(((ShapeBorder)border).dimensions);
                return default;
            }));
            return default!;
        }
    }
    public override ShapeBorder? add(ShapeBorder other, bool reversed = false)
    {
        if ((other is not _CompoundBorder__borders))
        {
            ShapeBorder ours = (reversed ? this.borders.Last() : this.borders.First());
            ShapeBorder? merged = (ours.add(other, reversed: reversed) ?? other.add(ours, reversed: !reversed));
            if ((merged is not null))
            {
                var result = new List<ShapeBorder>();
                result[(int)((reversed ? (checked((long)(result.Count)) - 1L) : 0L))] = merged;
                return new _CompoundBorder__borders(result);
            }
        }
        var mergedBorders = new List<ShapeBorder>();
        return new _CompoundBorder__borders(mergedBorders);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder scale(double t)
    {
        return new _CompoundBorder__borders(this.borders.map<ShapeBorder, ShapeBorder>(((border) => border.scale(t))).ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        return _CompoundBorder__borders.lerp(a, this, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        return _CompoundBorder__borders.lerp(this, b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static _CompoundBorder__borders lerp(ShapeBorder? a, ShapeBorder? b, double t)
    {
        DartRuntimePrimitives.Assert(() => ((a is _CompoundBorder__borders) || (b is _CompoundBorder__borders)));
        List<ShapeBorder?> aList = ((a is _CompoundBorder__borders) ? ((_CompoundBorder__borders)((_CompoundBorder__borders)a)).borders : new List<ShapeBorder?> { a });
        List<ShapeBorder?> bList = ((b is _CompoundBorder__borders) ? ((_CompoundBorder__borders)((_CompoundBorder__borders)b)).borders : new List<ShapeBorder?> { b });
        var results = new List<ShapeBorder>();
        long length = Math.Max(checked((long)(aList.Count)), checked((long)(bList.Count)));
        for (var index = 0L; (index < length); index += 1L)
        {
            ShapeBorder? localA = ((index < checked((long)(aList.Count))) ? aList[(int)(index)] : null);
            ShapeBorder? localB = ((index < checked((long)(bList.Count))) ? bList[(int)(index)] : null);
            if (((localA is not null) && (localB is not null)))
            {
                ShapeBorder? localResult = (localA.lerpTo(localB, t) ?? localB.lerpFrom(localA, t));
                if ((localResult is not null))
                {
                    results.Add(localResult);
                    continue;
                }
            }
            if ((localB is not null))
            {
                results.Add(localB.scale(t));
            }
            if ((localA is not null))
            {
                results.Add(localA.scale((1.0 - t)));
            }
        }
        return new _CompoundBorder__borders(results);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        for (var index = 0L; (index < (checked((long)(this.borders.Count)) - 1L)); index += 1L)
        {
            rect = this.borders[(int)(index)].dimensions.resolve(textDirection).deflateRect(rect);
        }
        return this.borders.Last().getInnerPath(rect, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return this.borders.First().getOuterPath(rect, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return this.borders.First().hitTest(rect, position, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        this.borders.First().paintInterior(canvas, rect, paint, textDirection: textDirection);
    }

    public override bool preferPaintInterior => this.borders.All(((border) => ((ShapeBorder)border).preferPaintInterior));
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        foreach (ShapeBorder border in this.borders)
        {
            border.paint(canvas, rect, textDirection: textDirection);
            rect = ((ShapeBorder)border).dimensions.resolve(textDirection).deflateRect(rect);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _CompoundBorder__borders;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is _CompoundBorder__borders) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<ShapeBorder>(((_CompoundBorder__borders)((_CompoundBorder__borders)__other)).borders, this.borders));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHashAll(this.borders);
    public override string ToString()
    {
        return string.Join(" + ", System.Linq.Enumerable.Reverse(this.borders).map<ShapeBorder, string>(((border) => border.ToString())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class BordersLibrary
{
    public static void paintBorder(Canvas canvas, Rect rect, BorderSide top = default!, BorderSide right = default!, BorderSide bottom = default!, BorderSide left = default!)
    {
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.strokeWidth = 0.0;
    return __cascade;
}))();
        var path = new global::Doroti.Ui.Path();
        switch (((BorderSide)top).style)
        {
            case BorderStyle.solid:
                {
                    paint.color = ((BorderSide)top).color;
                    path.reset();
                    path.moveTo(rect.left, rect.top);
                    path.lineTo(rect.right, rect.top);
                    if ((((BorderSide)top).width == 0.0))
                    {
                        paint.style = PaintingStyle.stroke;
                    }
                    else
                    {
                        paint.style = PaintingStyle.fill;
                        path.lineTo((rect.right - ((BorderSide)right).width), (rect.top + ((BorderSide)top).width));
                        path.lineTo((rect.left + ((BorderSide)left).width), (rect.top + ((BorderSide)top).width));
                    }
                    canvas.drawPath(path, paint);
                    break;
                }
            case BorderStyle.none:
                {
                    break;
                }
        }
        switch (((BorderSide)right).style)
        {
            case BorderStyle.solid:
                {
                    paint.color = ((BorderSide)right).color;
                    path.reset();
                    path.moveTo(rect.right, rect.top);
                    path.lineTo(rect.right, rect.bottom);
                    if ((((BorderSide)right).width == 0.0))
                    {
                        paint.style = PaintingStyle.stroke;
                    }
                    else
                    {
                        paint.style = PaintingStyle.fill;
                        path.lineTo((rect.right - ((BorderSide)right).width), (rect.bottom - ((BorderSide)bottom).width));
                        path.lineTo((rect.right - ((BorderSide)right).width), (rect.top + ((BorderSide)top).width));
                    }
                    canvas.drawPath(path, paint);
                    break;
                }
            case BorderStyle.none:
                {
                    break;
                }
        }
        switch (((BorderSide)bottom).style)
        {
            case BorderStyle.solid:
                {
                    paint.color = ((BorderSide)bottom).color;
                    path.reset();
                    path.moveTo(rect.right, rect.bottom);
                    path.lineTo(rect.left, rect.bottom);
                    if ((((BorderSide)bottom).width == 0.0))
                    {
                        paint.style = PaintingStyle.stroke;
                    }
                    else
                    {
                        paint.style = PaintingStyle.fill;
                        path.lineTo((rect.left + ((BorderSide)left).width), (rect.bottom - ((BorderSide)bottom).width));
                        path.lineTo((rect.right - ((BorderSide)right).width), (rect.bottom - ((BorderSide)bottom).width));
                    }
                    canvas.drawPath(path, paint);
                    break;
                }
            case BorderStyle.none:
                {
                    break;
                }
        }
        switch (((BorderSide)left).style)
        {
            case BorderStyle.solid:
                {
                    paint.color = ((BorderSide)left).color;
                    path.reset();
                    path.moveTo(rect.left, rect.bottom);
                    path.lineTo(rect.left, rect.top);
                    if ((((BorderSide)left).width == 0.0))
                    {
                        paint.style = PaintingStyle.stroke;
                    }
                    else
                    {
                        paint.style = PaintingStyle.fill;
                        path.lineTo((rect.left + ((BorderSide)left).width), (rect.top + ((BorderSide)top).width));
                        path.lineTo((rect.left + ((BorderSide)left).width), (rect.bottom - ((BorderSide)bottom).width));
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

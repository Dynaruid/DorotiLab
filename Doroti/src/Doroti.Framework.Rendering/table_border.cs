// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/table_border.dart
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
        global::Doroti.Framework.Painting.BorderSide __top = top ?? global::Doroti.Framework.Painting.BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __right = right ?? global::Doroti.Framework.Painting.BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __bottom = bottom ?? global::Doroti.Framework.Painting.BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __left = left ?? global::Doroti.Framework.Painting.BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __horizontalInside = horizontalInside ?? global::Doroti.Framework.Painting.BorderSide.none;
        global::Doroti.Framework.Painting.BorderSide __verticalInside = verticalInside ?? global::Doroti.Framework.Painting.BorderSide.none;
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero;
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
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero;
        var side = new global::Doroti.Framework.Painting.BorderSide(color: __color, width: width, style: style);
        return new TableBorder(top: side, right: side, bottom: side, left: side, horizontalInside: side, verticalInside: side, borderRadius: __borderRadius);
    }

    public static TableBorder CreateSymmetric(global::Doroti.Framework.Painting.BorderSide inside = default!, global::Doroti.Framework.Painting.BorderSide outside = default!, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!)
    {
        var __instance = new TableBorder(default!, default!, default!, default!, default!, default!, default!);
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
            return new global::Doroti.Framework.Painting.EdgeInsets(((global::Doroti.Framework.Painting.BorderSide)this.left).width, ((global::Doroti.Framework.Painting.BorderSide)this.top).width, ((global::Doroti.Framework.Painting.BorderSide)this.right).width, ((global::Doroti.Framework.Painting.BorderSide)this.bottom).width);
            return default!;
        }
    }
    public virtual bool isUniform
    {
        get
        {
            return ((_allSidesMatch<global::Doroti.Ui.Color>(((Func<global::Doroti.Framework.Painting.BorderSide, Color>)((side) => ((global::Doroti.Framework.Painting.BorderSide)side).color))) && _allSidesMatch<double>(((Func<global::Doroti.Framework.Painting.BorderSide, double>)((side) => ((global::Doroti.Framework.Painting.BorderSide)side).width)))) && _allSidesMatch<global::Doroti.Framework.Painting.BorderStyle>(((Func<global::Doroti.Framework.Painting.BorderSide, global::Doroti.Framework.Painting.BorderStyle>)((side) => ((global::Doroti.Framework.Painting.BorderSide)side).style))));
            return default!;
        }
    }
    internal virtual bool _outerBorderIsUniform
    {
        get
        {
            return ((_outerSidesMatch<global::Doroti.Ui.Color>(((Func<global::Doroti.Framework.Painting.BorderSide, Color>)((side) => ((global::Doroti.Framework.Painting.BorderSide)side).color))) && _outerSidesMatch<double>(((Func<global::Doroti.Framework.Painting.BorderSide, double>)((side) => ((global::Doroti.Framework.Painting.BorderSide)side).width)))) && _outerSidesMatch<global::Doroti.Framework.Painting.BorderStyle>(((Func<global::Doroti.Framework.Painting.BorderSide, global::Doroti.Framework.Painting.BorderStyle>)((side) => ((global::Doroti.Framework.Painting.BorderSide)side).style))));
            return default!;
        }
    }
    internal virtual bool _allSidesMatch<T>(Func<global::Doroti.Framework.Painting.BorderSide, T> selector)
    {
        T topValue = selector(this.top);
        return (((((object.Equals(selector(this.right), topValue)) && (object.Equals(selector(this.bottom), topValue))) && (object.Equals(selector(this.left), topValue))) && (object.Equals(selector(this.horizontalInside), topValue))) && (object.Equals(selector(this.verticalInside), topValue)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _outerSidesMatch<T>(Func<global::Doroti.Framework.Painting.BorderSide, T> selector)
    {
        T topValue = selector(this.top);
        return (((object.Equals(selector(this.right), topValue)) && (object.Equals(selector(this.bottom), topValue))) && (object.Equals(selector(this.left), topValue)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<global::Doroti.Ui.Color> _distinctVisibleOuterColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintTableBorder(Canvas canvas, Rect rect)
    {
        if ((this._outerBorderIsUniform && (!object.Equals(this.borderRadius, global::Doroti.Framework.Painting.BorderRadius.zero))))
        {
            global::Doroti.Ui.RRect outer = this.borderRadius.toRRect(rect);
            global::Doroti.Ui.RRect inner = outer.deflate(((global::Doroti.Framework.Painting.BorderSide)this.top).width);
            var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((global::Doroti.Framework.Painting.BorderSide)this.top).color;
    return __cascade;
}))();
            canvas.drawDRRect(outer, inner, paint);
            return;
        }
        HashSet<global::Doroti.Ui.Color> visibleColors = _distinctVisibleOuterColors();
        if (((checked((long)(visibleColors.Count)) == 1L) && (!object.Equals(this.borderRadius, global::Doroti.Framework.Painting.BorderRadius.zero))))
        {
            _paintNonUniformBorderWithRadius(canvas, rect, borderRadius: this.borderRadius, top: ((object.Equals(((global::Doroti.Framework.Painting.BorderSide)this.top).style, global::Doroti.Framework.Painting.BorderStyle.none)) ? global::Doroti.Framework.Painting.BorderSide.none : this.top), right: ((object.Equals(((global::Doroti.Framework.Painting.BorderSide)this.right).style, global::Doroti.Framework.Painting.BorderStyle.none)) ? global::Doroti.Framework.Painting.BorderSide.none : this.right), bottom: ((object.Equals(((global::Doroti.Framework.Painting.BorderSide)this.bottom).style, global::Doroti.Framework.Painting.BorderStyle.none)) ? global::Doroti.Framework.Painting.BorderSide.none : this.bottom), left: ((object.Equals(((global::Doroti.Framework.Painting.BorderSide)this.left).style, global::Doroti.Framework.Painting.BorderStyle.none)) ? global::Doroti.Framework.Painting.BorderSide.none : this.left), color: visibleColors.First());
            return;
        }
        global::Doroti.Framework.Painting.BordersLibrary.paintBorder(canvas, rect, top: this.top, right: this.right, bottom: this.bottom, left: this.left);
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
        global::Doroti.Ui.RRect inner = new global::Doroti.Framework.Painting.EdgeInsets(((global::Doroti.Framework.Painting.BorderSide)left).strokeInset, ((global::Doroti.Framework.Painting.BorderSide)top).strokeInset, ((global::Doroti.Framework.Painting.BorderSide)right).strokeInset, ((global::Doroti.Framework.Painting.BorderSide)bottom).strokeInset).deflateRRect(borderRect);
        global::Doroti.Ui.RRect outer = new global::Doroti.Framework.Painting.EdgeInsets(((global::Doroti.Framework.Painting.BorderSide)left).strokeOutset, ((global::Doroti.Framework.Painting.BorderSide)top).strokeOutset, ((global::Doroti.Framework.Painting.BorderSide)right).strokeOutset, ((global::Doroti.Framework.Painting.BorderSide)bottom).strokeOutset).inflateRRect(borderRect);
        canvas.drawDRRect(outer, inner, paint);
    }

    public virtual TableBorder scale(double t)
    {
        return new TableBorder(top: this.top.scale(t), right: this.right.scale(t), bottom: this.bottom.scale(t), left: this.left.scale(t), horizontalInside: this.horizontalInside.scale(t), verticalInside: this.verticalInside.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TableBorder? lerp(TableBorder? a, TableBorder? b, double t)
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
        return new TableBorder(top: BorderSide.lerp(((TableBorder)a).top, ((TableBorder)b).top, t), right: BorderSide.lerp(((TableBorder)a).right, ((TableBorder)b).right, t), bottom: BorderSide.lerp(((TableBorder)a).bottom, ((TableBorder)b).bottom, t), left: BorderSide.lerp(((TableBorder)a).left, ((TableBorder)b).left, t), horizontalInside: BorderSide.lerp(((TableBorder)a).horizontalInside, ((TableBorder)b).horizontalInside, t), verticalInside: BorderSide.lerp(((TableBorder)a).verticalInside, ((TableBorder)b).verticalInside, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(Canvas canvas, Rect rect, IEnumerable<double> rows, IEnumerable<double> columns)
    {
        DartRuntimePrimitives.Assert(() => ((rows.Count() == 0) || (((rows.First() >= 0.0) && (rows.Last() <= rect.height)))));
        DartRuntimePrimitives.Assert(() => ((columns.Count() == 0) || (((columns.First() >= 0.0) && (columns.Last() <= rect.width)))));
        if (((columns.Count() != 0) || (rows.Count() != 0)))
        {
            var paintLocal = new global::Doroti.Ui.Paint();
            var path = new global::Doroti.Ui.Path();
            if ((columns.Count() != 0))
            {
                switch (((global::Doroti.Framework.Painting.BorderSide)this.verticalInside).style)
                {
                    case global::Doroti.Framework.Painting.BorderStyle.solid:
                        {
                            ((Func<Paint>)(() =>
{
    var __cascade = paintLocal;
    __cascade.color = ((global::Doroti.Framework.Painting.BorderSide)this.verticalInside).color;
    __cascade.strokeWidth = ((global::Doroti.Framework.Painting.BorderSide)this.verticalInside).width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                            path.reset();
                            foreach (var x in columns)
                            {
                                path.moveTo((rect.left + x), rect.top);
                                path.lineTo((rect.left + x), rect.bottom);
                            }
                            canvas.drawPath(path, paintLocal);
                            break;
                        }
                    case global::Doroti.Framework.Painting.BorderStyle.none:
                        {
                            break;
                        }
                }
            }
            if ((rows.Count() != 0))
            {
                switch (((global::Doroti.Framework.Painting.BorderSide)this.horizontalInside).style)
                {
                    case global::Doroti.Framework.Painting.BorderStyle.solid:
                        {
                            ((Func<Paint>)(() =>
{
    var __cascade = paintLocal;
    __cascade.color = ((global::Doroti.Framework.Painting.BorderSide)this.horizontalInside).color;
    __cascade.strokeWidth = ((global::Doroti.Framework.Painting.BorderSide)this.horizontalInside).width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                            path.reset();
                            foreach (var y in rows)
                            {
                                path.moveTo(rect.left, (rect.top + y));
                                path.lineTo(rect.right, (rect.top + y));
                            }
                            canvas.drawPath(path, paintLocal);
                            break;
                        }
                    case global::Doroti.Framework.Painting.BorderStyle.none:
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((__other is TableBorder) && (object.Equals(((TableBorder)((TableBorder)__other)).top, this.top))) && (object.Equals(((TableBorder)((TableBorder)__other)).right, this.right))) && (object.Equals(((TableBorder)((TableBorder)__other)).bottom, this.bottom))) && (object.Equals(((TableBorder)((TableBorder)__other)).left, this.left))) && (object.Equals(((TableBorder)((TableBorder)__other)).horizontalInside, this.horizontalInside))) && (object.Equals(((TableBorder)((TableBorder)__other)).verticalInside, this.verticalInside))) && (object.Equals(((TableBorder)((TableBorder)__other)).borderRadius, this.borderRadius)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.top, this.right, this.bottom, this.left, this.horizontalInside, this.verticalInside, this.borderRadius);
    public override string ToString() => $"TableBorder({this.top}, {this.right}, {this.bottom}, {this.left}, {this.horizontalInside}, {this.verticalInside}, {this.borderRadius})";
}


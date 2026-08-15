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

namespace Doroti.Generated.Framework.Rendering;

public class TableBorder
{
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide top { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide right { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide bottom { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide left { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide horizontalInside { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide verticalInside { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;

    public TableBorder(global::Doroti.Generated.Framework.Painting.BorderSide top = default!, global::Doroti.Generated.Framework.Painting.BorderSide right = default!, global::Doroti.Generated.Framework.Painting.BorderSide bottom = default!, global::Doroti.Generated.Framework.Painting.BorderSide left = default!, global::Doroti.Generated.Framework.Painting.BorderSide horizontalInside = default!, global::Doroti.Generated.Framework.Painting.BorderSide verticalInside = default!, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!)
    {
        global::Doroti.Generated.Framework.Painting.BorderSide __top = top ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        global::Doroti.Generated.Framework.Painting.BorderSide __right = right ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        global::Doroti.Generated.Framework.Painting.BorderSide __bottom = bottom ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        global::Doroti.Generated.Framework.Painting.BorderSide __left = left ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        global::Doroti.Generated.Framework.Painting.BorderSide __horizontalInside = horizontalInside ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        global::Doroti.Generated.Framework.Painting.BorderSide __verticalInside = verticalInside ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        this.top = __top;
        this.right = __right;
        this.bottom = __bottom;
        this.left = __left;
        this.horizontalInside = __horizontalInside;
        this.verticalInside = __verticalInside;
        this.borderRadius = __borderRadius;
    }

    public static TableBorder CreateAll(Color color = default!, double width = 1.0, global::Doroti.Generated.Framework.Painting.BorderStyle style = BorderStyle.solid, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!)
    {
        Color __color = color ?? new Color(0xFF000000);
        global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero;
        var side__1359 = new global::Doroti.Generated.Framework.Painting.BorderSide(color: __color, width: width, style: style);
        return new TableBorder(top: side__1359, right: side__1359, bottom: side__1359, left: side__1359, horizontalInside: side__1359, verticalInside: side__1359, borderRadius: __borderRadius);
    }

    public static TableBorder CreateSymmetric(global::Doroti.Generated.Framework.Painting.BorderSide inside = default!, global::Doroti.Generated.Framework.Painting.BorderSide outside = default!, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!)
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

    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets dimensions
    {
        get
        {
            return new global::Doroti.Generated.Framework.Painting.EdgeInsets(((global::Doroti.Generated.Framework.Painting.BorderSide)this.left).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.top).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.right).width, ((global::Doroti.Generated.Framework.Painting.BorderSide)this.bottom).width);
            return default!;
        }
    }
    public virtual bool isUniform
    {
        get
        {
            return ((_allSidesMatch<global::Doroti.Ui.Color>(((Func<global::Doroti.Generated.Framework.Painting.BorderSide, Color>)((side) => ((global::Doroti.Generated.Framework.Painting.BorderSide)side).color))) && _allSidesMatch<double>(((Func<global::Doroti.Generated.Framework.Painting.BorderSide, double>)((side) => ((global::Doroti.Generated.Framework.Painting.BorderSide)side).width)))) && _allSidesMatch<global::Doroti.Generated.Framework.Painting.BorderStyle>(((Func<global::Doroti.Generated.Framework.Painting.BorderSide, global::Doroti.Generated.Framework.Painting.BorderStyle>)((side) => ((global::Doroti.Generated.Framework.Painting.BorderSide)side).style))));
            return default!;
        }
    }
    internal virtual bool _outerBorderIsUniform
    {
        get
        {
            return ((_outerSidesMatch<global::Doroti.Ui.Color>(((Func<global::Doroti.Generated.Framework.Painting.BorderSide, Color>)((side) => ((global::Doroti.Generated.Framework.Painting.BorderSide)side).color))) && _outerSidesMatch<double>(((Func<global::Doroti.Generated.Framework.Painting.BorderSide, double>)((side) => ((global::Doroti.Generated.Framework.Painting.BorderSide)side).width)))) && _outerSidesMatch<global::Doroti.Generated.Framework.Painting.BorderStyle>(((Func<global::Doroti.Generated.Framework.Painting.BorderSide, global::Doroti.Generated.Framework.Painting.BorderStyle>)((side) => ((global::Doroti.Generated.Framework.Painting.BorderSide)side).style))));
            return default!;
        }
    }
    internal virtual bool _allSidesMatch<T>(Func<global::Doroti.Generated.Framework.Painting.BorderSide, T> selector)
    {
        T topValue__3762 = selector(this.top);
        return (((((object.Equals(selector(this.right), topValue__3762)) && (object.Equals(selector(this.bottom), topValue__3762))) && (object.Equals(selector(this.left), topValue__3762))) && (object.Equals(selector(this.horizontalInside), topValue__3762))) && (object.Equals(selector(this.verticalInside), topValue__3762)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _outerSidesMatch<T>(Func<global::Doroti.Generated.Framework.Painting.BorderSide, T> selector)
    {
        T topValue__4095 = selector(this.top);
        return (((object.Equals(selector(this.right), topValue__4095)) && (object.Equals(selector(this.bottom), topValue__4095))) && (object.Equals(selector(this.left), topValue__4095)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<global::Doroti.Ui.Color> _distinctVisibleOuterColors()
    {
        return new HashSet<Color>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintTableBorder(Canvas canvas, Rect rect)
    {
        if ((this._outerBorderIsUniform && (!object.Equals(this.borderRadius, global::Doroti.Generated.Framework.Painting.BorderRadius.zero))))
        {
            global::Doroti.Ui.RRect outer__4845 = this.borderRadius.toRRect(rect);
            global::Doroti.Ui.RRect inner__4899 = outer__4845.deflate(((global::Doroti.Generated.Framework.Painting.BorderSide)this.top).width);
            var paint__4945 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((global::Doroti.Generated.Framework.Painting.BorderSide)this.top).color;
    return __cascade;
}))();
            canvas.drawDRRect(outer__4845, inner__4899, paint__4945);
            return;
        }
        HashSet<global::Doroti.Ui.Color> visibleColors__5069 = _distinctVisibleOuterColors();
        if (((checked((long)(visibleColors__5069.Count)) == 1L) && (!object.Equals(this.borderRadius, global::Doroti.Generated.Framework.Painting.BorderRadius.zero))))
        {
            _paintNonUniformBorderWithRadius(canvas, rect, borderRadius: this.borderRadius, top: ((object.Equals(((global::Doroti.Generated.Framework.Painting.BorderSide)this.top).style, global::Doroti.Generated.Framework.Painting.BorderStyle.none)) ? global::Doroti.Generated.Framework.Painting.BorderSide.none : this.top), right: ((object.Equals(((global::Doroti.Generated.Framework.Painting.BorderSide)this.right).style, global::Doroti.Generated.Framework.Painting.BorderStyle.none)) ? global::Doroti.Generated.Framework.Painting.BorderSide.none : this.right), bottom: ((object.Equals(((global::Doroti.Generated.Framework.Painting.BorderSide)this.bottom).style, global::Doroti.Generated.Framework.Painting.BorderStyle.none)) ? global::Doroti.Generated.Framework.Painting.BorderSide.none : this.bottom), left: ((object.Equals(((global::Doroti.Generated.Framework.Painting.BorderSide)this.left).style, global::Doroti.Generated.Framework.Painting.BorderStyle.none)) ? global::Doroti.Generated.Framework.Painting.BorderSide.none : this.left), color: visibleColors__5069.First());
            return;
        }
        global::Doroti.Generated.Framework.Painting.BordersLibrary.paintBorder(canvas, rect, top: this.top, right: this.right, bottom: this.bottom, left: this.left);
    }

    internal static void _paintNonUniformBorderWithRadius(Canvas canvas, Rect rect, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius, Color color, global::Doroti.Generated.Framework.Painting.BorderSide top, global::Doroti.Generated.Framework.Painting.BorderSide right, global::Doroti.Generated.Framework.Painting.BorderSide bottom, global::Doroti.Generated.Framework.Painting.BorderSide left)
    {
        global::Doroti.Ui.RRect borderRect__6193 = borderRadius.toRRect(rect);
        var paint__6244 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color;
    return __cascade;
}))();
        global::Doroti.Ui.RRect inner__6293 = new global::Doroti.Generated.Framework.Painting.EdgeInsets(((global::Doroti.Generated.Framework.Painting.BorderSide)left).strokeInset, ((global::Doroti.Generated.Framework.Painting.BorderSide)top).strokeInset, ((global::Doroti.Generated.Framework.Painting.BorderSide)right).strokeInset, ((global::Doroti.Generated.Framework.Painting.BorderSide)bottom).strokeInset).deflateRRect(borderRect__6193);
        global::Doroti.Ui.RRect outer__6469 = new global::Doroti.Generated.Framework.Painting.EdgeInsets(((global::Doroti.Generated.Framework.Painting.BorderSide)left).strokeOutset, ((global::Doroti.Generated.Framework.Painting.BorderSide)top).strokeOutset, ((global::Doroti.Generated.Framework.Painting.BorderSide)right).strokeOutset, ((global::Doroti.Generated.Framework.Painting.BorderSide)bottom).strokeOutset).inflateRRect(borderRect__6193);
        canvas.drawDRRect(outer__6469, inner__6293, paint__6244);
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
            var paint__10347 = new global::Doroti.Ui.Paint();
            var path__10376 = new global::Doroti.Ui.Path();
            if ((columns.Count() != 0))
            {
                switch (((global::Doroti.Generated.Framework.Painting.BorderSide)this.verticalInside).style)
                {
                    case global::Doroti.Generated.Framework.Painting.BorderStyle.solid:
                        {
                            ((Func<Paint>)(() =>
{
    var __cascade = paint__10347;
    __cascade.color = ((global::Doroti.Generated.Framework.Painting.BorderSide)this.verticalInside).color;
    __cascade.strokeWidth = ((global::Doroti.Generated.Framework.Painting.BorderSide)this.verticalInside).width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                            path__10376.reset();
                            foreach (var x__10707 in columns)
                            {
                                path__10376.moveTo((rect.left + x__10707), rect.top);
                                path__10376.lineTo((rect.left + x__10707), rect.bottom);
                            }
                            canvas.drawPath(path__10376, paint__10347);
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.BorderStyle.none:
                        {
                            break;
                        }
                }
            }
            if ((rows.Count() != 0))
            {
                switch (((global::Doroti.Generated.Framework.Painting.BorderSide)this.horizontalInside).style)
                {
                    case global::Doroti.Generated.Framework.Painting.BorderStyle.solid:
                        {
                            ((Func<Paint>)(() =>
{
    var __cascade = paint__10347;
    __cascade.color = ((global::Doroti.Generated.Framework.Painting.BorderSide)this.horizontalInside).color;
    __cascade.strokeWidth = ((global::Doroti.Generated.Framework.Painting.BorderSide)this.horizontalInside).width;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                            path__10376.reset();
                            foreach (var y__11275 in rows)
                            {
                                path__10376.moveTo(rect.left, (rect.top + y__11275));
                                path__10376.lineTo(rect.right, (rect.top + y__11275));
                            }
                            canvas.drawPath(path__10376, paint__10347);
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.BorderStyle.none:
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


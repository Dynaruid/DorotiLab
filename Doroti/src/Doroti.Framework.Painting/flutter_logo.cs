// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/flutter_logo.dart
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

public enum FlutterLogoStyle
{
    markOnly,
    horizontal,
    stacked
}

public class FlutterLogoDecoration : Decoration
{
    public virtual Color textColor { get; private set; } = default!;
    public virtual FlutterLogoStyle style { get; private set; } = default!;
    public virtual EdgeInsets margin { get; private set; } = default!;
    internal virtual double _position { get; private set; } = default!;
    internal virtual double _opacity { get; private set; } = default!;

    public FlutterLogoDecoration(Color textColor = default!, FlutterLogoStyle style = FlutterLogoStyle.markOnly, EdgeInsets margin = default!)
    {
        Color __textColor = textColor ?? new Color(0xFF757575);
        EdgeInsets __margin = margin ?? EdgeInsets.zero;
        this.textColor = __textColor;
        this.style = style;
        this.margin = __margin;
        this._position = (DartRuntimePrimitives.Identical(style, FlutterLogoStyle.markOnly) ? 0.0 : (DartRuntimePrimitives.Identical(style, FlutterLogoStyle.horizontal) ? 1.0 : -1.0));
        this._opacity = 1.0;
    }

    public static FlutterLogoDecoration Create_(Color textColor, FlutterLogoStyle style, EdgeInsets margin, double _position, double _opacity)
    {
        var __instance = new FlutterLogoDecoration(default!, default!, default!);
        __instance.textColor = textColor;
        __instance.style = style;
        __instance.margin = margin;
        __instance._position = _position;
        __instance._opacity = _opacity;
        return __instance;
    }

    internal virtual bool _inTransition => ((this._opacity != 1.0) || ((((this._position != -1.0) && (this._position != 0.0)) && (this._position != 1.0))));
    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() => ((double.IsFinite(this._position) && (this._opacity >= 0.0)) && (this._opacity <= 1.0)));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isComplex => !this._inTransition;
    public static FlutterLogoDecoration? lerp(FlutterLogoDecoration? a, FlutterLogoDecoration? b, double t)
    {
        DartRuntimePrimitives.Assert(() => ((a is null) || a.debugAssertIsValid()));
        DartRuntimePrimitives.Assert(() => ((b is null) || b.debugAssertIsValid()));
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return FlutterLogoDecoration.Create_(b!.textColor, ((FlutterLogoDecoration)b).style, (((FlutterLogoDecoration)b).margin.op_Multiply(t)), ((FlutterLogoDecoration)b)._position, (((FlutterLogoDecoration)b)._opacity * Dart_uiLibrary.clampDouble(t, 0.0, 1.0)));
        }
        if ((b is null))
        {
            return FlutterLogoDecoration.Create_(((FlutterLogoDecoration)a).textColor, ((FlutterLogoDecoration)a).style, (((FlutterLogoDecoration)a).margin.op_Multiply(t)), ((FlutterLogoDecoration)a)._position, (((FlutterLogoDecoration)a)._opacity * Dart_uiLibrary.clampDouble((1.0 - t), 0.0, 1.0)));
        }
        if ((t == 0.0))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        return FlutterLogoDecoration.Create_(Dart_uiLibrary.Color.lerp(((FlutterLogoDecoration)a).textColor, ((FlutterLogoDecoration)b).textColor, t)!, ((t < 0.5) ? ((FlutterLogoDecoration)a).style : ((FlutterLogoDecoration)b).style), EdgeInsets.lerp(((FlutterLogoDecoration)a).margin, ((FlutterLogoDecoration)b).margin, t)!, (((FlutterLogoDecoration)a)._position + (((((FlutterLogoDecoration)b)._position - ((FlutterLogoDecoration)a)._position)) * t)), Dart_uiLibrary.clampDouble((((FlutterLogoDecoration)a)._opacity + (((((FlutterLogoDecoration)b)._opacity - ((FlutterLogoDecoration)a)._opacity)) * t)), 0.0, 1.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FlutterLogoDecoration? lerpFrom(Decoration? a, double t)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if ((a is FlutterLogoDecoration))
        {
            FlutterLogoDecoration a__as4619 = (FlutterLogoDecoration)a;
            DartRuntimePrimitives.Assert(() => (((FlutterLogoDecoration?)a__as4619)?.debugAssertIsValid() ?? true));
            return FlutterLogoDecoration.lerp(((FlutterLogoDecoration?)a__as4619), this, t);
        }
        return ((FlutterLogoDecoration?)(object?)base.lerpFrom(a, t))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FlutterLogoDecoration? lerpTo(Decoration? b, double t)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if ((b is FlutterLogoDecoration))
        {
            FlutterLogoDecoration b__as4933 = (FlutterLogoDecoration)b;
            DartRuntimePrimitives.Assert(() => (((FlutterLogoDecoration?)b__as4933)?.debugAssertIsValid() ?? true));
            return FlutterLogoDecoration.lerp(this, ((FlutterLogoDecoration?)b__as4933), t);
        }
        return ((FlutterLogoDecoration?)(object?)base.lerpTo(b, t))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Size size, Offset position, TextDirection? textDirection = null) => true;
    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return new _FlutterLogoPainter__flutter_logo(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as FlutterLogoDecoration;
        if (__other is null) return false;
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return ((((__other is FlutterLogoDecoration) && (object.Equals(((FlutterLogoDecoration)((FlutterLogoDecoration)__other)).textColor, this.textColor))) && (((FlutterLogoDecoration)((FlutterLogoDecoration)__other))._position == this._position)) && (((FlutterLogoDecoration)((FlutterLogoDecoration)__other))._opacity == this._opacity));
    }

    public override int GetHashCode()
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return FoundationRuntimePorts.ObjectHash(this.textColor, this._position, this._opacity);
        return default!;
    }
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new ColorProperty("textColor", this.textColor));
        properties.add(new EnumProperty<FlutterLogoStyle>("style", this.style));
        if (this._inTransition)
        {
            properties.add(new DiagnosticsNode($"transition {(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this._position))}:{(global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(this._opacity))}"));
        }
    }

}

internal class _FlutterLogoPainter__flutter_logo : BoxPainter
{
    internal virtual FlutterLogoDecoration _config { get; private set; } = default!;
    internal virtual TextPainter _textPainter { get; set; } = default!;
    internal virtual Rect _textBoundingRect { get; set; } = default!;

    internal _FlutterLogoPainter__flutter_logo(FlutterLogoDecoration _config) : base(null)
    {
        this._config = _config;
        System.Diagnostics.Debug.Assert(_config.debugAssertIsValid());
    }

    public override void dispose()
    {
        this._textPainter.dispose();
        base.dispose();
    }

    internal virtual void _prepareText()
    {
        var kLabel__6912 = "Flutter";
        _textPainter = new TextPainter(text: new TextSpan(text: kLabel__6912, style: new TextStyle(color: ((FlutterLogoDecoration)this._config).textColor, fontFamily: "Roboto", fontSize: ((100.0 * 350.0) / 247.0), fontWeight: FontWeight.w300, textBaseline: TextBaseline.alphabetic)), textDirection: TextDirection.ltr);
        this._textPainter.layout();
        global::Doroti.Ui.TextBox textSize__7478 = this._textPainter.getBoxesForSelection(new TextSelection(baseOffset: 0L, extentOffset: kLabel__6912.Length)).Single();
        _textBoundingRect = global::Doroti.Ui.Rect.fromLTRB(textSize__7478.left, textSize__7478.top, textSize__7478.right, textSize__7478.bottom);
    }

    internal virtual void _paintLogo(Canvas canvas, Rect rect)
    {
        canvas.save();
        canvas.translate(rect.left, rect.top);
        canvas.scale((rect.width / 202.0), (rect.height / 202.0));
        canvas.translate((((202.0 - 166.0)) / 2.0), 0.0);
        var lightPaint__8627 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4283745784L);
    return __cascade;
}))();
        var mediumPaint__8692 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4280923894L);
    return __cascade;
}))();
        var darkPaint__8758 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4278278043L);
    return __cascade;
}))();
        var triangleGradient__8823 = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset((87.2623 + 37.9092), (28.8384 + 123.4389)), new global::Doroti.Ui.Offset((42.9205 + 37.9092), (35.0952 + 123.4389)), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(1713022L), new global::Doroti.Ui.Color(1712989054L) });
        var trianglePaint__9062 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = triangleGradient__8823;
    return __cascade;
}))();
        var topBeam__9154 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(37.7, 128.9);
    __cascade.lineTo(9.8, 101.0);
    __cascade.lineTo(100.4, 10.4);
    __cascade.lineTo(156.2, 10.4);
    return __cascade;
}))();
        canvas.drawPath(topBeam__9154, lightPaint__8627);
        var middleBeam__9336 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(156.2, 94.0);
    __cascade.lineTo(100.4, 94.0);
    __cascade.lineTo(78.5, 115.9);
    __cascade.lineTo(106.4, 143.8);
    return __cascade;
}))();
        canvas.drawPath(middleBeam__9336, lightPaint__8627);
        var bottomBeam__9526 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(79.5, 170.7);
    __cascade.lineTo(100.4, 191.6);
    __cascade.lineTo(156.2, 191.6);
    __cascade.lineTo(107.4, 142.8);
    return __cascade;
}))();
        canvas.drawPath(bottomBeam__9526, darkPaint__8758);
        canvas.save();
        canvas.transform(new Float64List(new List<double> { 0.7071, -0.7071, 0.0, 0.0, 0.7071, 0.7071, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, -77.697, 98.057, 0.0, 1.0 }));
        canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(59.8, 123.1, 39.4, 39.4), mediumPaint__8692);
        canvas.restore();
        var triangle__10226 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(79.5, 170.7);
    __cascade.lineTo(120.9, 156.4);
    __cascade.lineTo(107.4, 142.8);
    return __cascade;
}))();
        canvas.drawPath(triangle__10226, trianglePaint__9062);
        canvas.restore();
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        offset += ((FlutterLogoDecoration)this._config).margin.topLeft;
        global::Doroti.Ui.Size canvasSize__10549 = ((FlutterLogoDecoration)this._config).margin.deflateSize(DartRuntimePrimitives.RequireValue(((ImageConfiguration)configuration).size));
        if (canvasSize__10549.isEmpty)
        {
            return;
        }
        global::Doroti.Ui.Size logoSize__10676 = (((FlutterLogoDecoration)this._config)._position switch { > 0.0 => new global::Doroti.Ui.Size(820.0, 232.0), < 0.0 => new global::Doroti.Ui.Size(252.0, 306.0), _ => new global::Doroti.Ui.Size(202.0, 202.0) });
        FittedSizes fittedSize__10918 = global::Doroti.Generated.Framework.Painting.Box_fitLibrary.applyBoxFit(BoxFit.contain, logoSize__10676, canvasSize__10549);
        DartRuntimePrimitives.Assert(() => (object.Equals(((FittedSizes)fittedSize__10918).source, logoSize__10676)));
        global::Doroti.Ui.Rect rect__11040 = Alignment.center.inscribe(((FittedSizes)fittedSize__10918).destination, (offset & canvasSize__10549));
        double centerSquareHeight__11136 = canvasSize__10549.shortestSide;
        var centerSquare__11192 = global::Doroti.Ui.Rect.fromLTWH((offset.dx + (((canvasSize__10549.width - centerSquareHeight__11136)) / 2.0)), (offset.dy + (((canvasSize__10549.height - centerSquareHeight__11136)) / 2.0)), centerSquareHeight__11136, centerSquareHeight__11136);
        global::Doroti.Ui.Rect logoTargetSquare__11428 = default!;
        if ((((FlutterLogoDecoration)this._config)._position > 0.0))
        {
            logoTargetSquare__11428 = global::Doroti.Ui.Rect.fromLTWH(rect__11040.left, rect__11040.top, rect__11040.height, rect__11040.height);
        }
        else
        {
            if ((((FlutterLogoDecoration)this._config)._position < 0.0))
            {
                double logoHeight__11678 = ((rect__11040.height * 191.0) / 306.0);
                logoTargetSquare__11428 = global::Doroti.Ui.Rect.fromLTWH((rect__11040.left + (((rect__11040.width - logoHeight__11678)) / 2.0)), rect__11040.top, logoHeight__11678, logoHeight__11678);
            }
            else
            {
                logoTargetSquare__11428 = centerSquare__11192;
            }
        }
        global::Doroti.Ui.Rect logoSquare__11976 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Rect.lerp(centerSquare__11192, logoTargetSquare__11428, ((FlutterLogoDecoration)this._config)._position.abs()));
        if ((((FlutterLogoDecoration)this._config)._opacity < 1.0))
        {
            canvas.saveLayer((offset & canvasSize__10549), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.colorFilter = global::Doroti.Ui.ColorFilter.mode(new global::Doroti.Ui.Color(4294967295L).withOpacity(((FlutterLogoDecoration)this._config)._opacity), BlendMode.modulate);
    return __cascade;
}))());
        }
        if ((((FlutterLogoDecoration)this._config)._position != 0.0))
        {
            if ((((FlutterLogoDecoration)this._config)._position > 0.0))
            {
                double fontSize__12455 = (((2.0 / 3.0) * logoSquare__11976.height) * ((1L - (((10.4 * 2.0)) / 202.0))));
                double scale__12547 = (fontSize__12455 / 100.0);
                double finalLeftTextPosition__12594 = ((((256.4 / 820.0)) * rect__11040.width) - (((32.0 / 350.0)) * fontSize__12455));
                double initialLeftTextPosition__12995 = ((rect__11040.width / 2.0) - (this._textBoundingRect.width * scale__12547));
                var textOffset__13152 = new global::Doroti.Ui.Offset((rect__11040.left + DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(initialLeftTextPosition__12995, finalLeftTextPosition__12594, ((FlutterLogoDecoration)this._config)._position))), (rect__11040.top + (((rect__11040.height - (this._textBoundingRect.height * scale__12547))) / 2.0)));
                canvas.save();
                if ((((FlutterLogoDecoration)this._config)._position < 1.0))
                {
                    global::Doroti.Ui.Offset center__13465 = logoSquare__11976.center;
                    var path__13509 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(center__13465.dx, center__13465.dy);
    __cascade.lineTo((center__13465.dx + rect__11040.width), (center__13465.dy - rect__11040.width));
    __cascade.lineTo((center__13465.dx + rect__11040.width), (center__13465.dy + rect__11040.width));
    __cascade.close();
    return __cascade;
}))();
                    canvas.clipPath(path__13509);
                }
                canvas.translate(textOffset__13152.dx, textOffset__13152.dy);
                canvas.scale(scale__12547, scale__12547);
                this._textPainter.paint(canvas, Offset.zero);
                canvas.restore();
            }
            else
            {
                if ((((FlutterLogoDecoration)this._config)._position < 0.0))
                {
                    double fontSize__14027 = ((0.35 * logoTargetSquare__11428.height) * ((1L - (((10.4 * 2.0)) / 202.0))));
                    double scale__14120 = (fontSize__14027 / 100.0);
                    if ((((FlutterLogoDecoration)this._config)._position > -1.0))
                    {
                        canvas.saveLayer(this._textBoundingRect, new global::Doroti.Ui.Paint());
                    }
                    else
                    {
                        canvas.save();
                    }
                    canvas.translate((logoTargetSquare__11428.center.dx - (((this._textBoundingRect.width * scale__14120) / 2.0))), logoTargetSquare__11428.bottom);
                    canvas.scale(scale__14120, scale__14120);
                    this._textPainter.paint(canvas, Offset.zero);
                    if ((((FlutterLogoDecoration)this._config)._position > -1.0))
                    {
                        canvas.drawRect(this._textBoundingRect.inflate((this._textBoundingRect.width * 0.5)), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.blendMode = BlendMode.modulate;
    __cascade.shader = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset((this._textBoundingRect.width * -0.5), 0.0), new global::Doroti.Ui.Offset((this._textBoundingRect.width * 1.5), 0.0), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(4294967295L), new global::Doroti.Ui.Color(16777215L), new global::Doroti.Ui.Color(16777215L) }, new List<double> { 0.0, Math.Max(0.0, (((FlutterLogoDecoration)this._config)._position.abs() - 0.1)), Math.Min((((FlutterLogoDecoration)this._config)._position.abs() + 0.1), 1.0), 1.0 });
    return __cascade;
}))());
                    }
                    canvas.restore();
                }
            }
        }
        _paintLogo(canvas, logoSquare__11976);
        if ((((FlutterLogoDecoration)this._config)._opacity < 1.0))
        {
            canvas.restore();
        }
    }

}


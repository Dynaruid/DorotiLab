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

namespace Doroti.Framework.Painting;

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
        var __instance = new FlutterLogoDecoration(textColor, style, margin);
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
            properties.add(new DiagnosticsNode($"transition {(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this._position))}:{(global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(this._opacity))}"));
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
        var kLabel = "Flutter";
        _textPainter = new TextPainter(text: new TextSpan(text: kLabel, style: new TextStyle(color: ((FlutterLogoDecoration)this._config).textColor, fontFamily: "Roboto", fontSize: ((100.0 * 350.0) / 247.0), fontWeight: FontWeight.w300, textBaseline: TextBaseline.alphabetic)), textDirection: TextDirection.ltr);
        this._textPainter.layout();
        global::Doroti.Ui.TextBox textSize = this._textPainter.getBoxesForSelection(new TextSelection(baseOffset: 0L, extentOffset: kLabel.Length)).Single();
        _textBoundingRect = global::Doroti.Ui.Rect.fromLTRB(textSize.left, textSize.top, textSize.right, textSize.bottom);
    }

    internal virtual void _paintLogo(Canvas canvas, Rect rect)
    {
        canvas.save();
        canvas.translate(rect.left, rect.top);
        canvas.scale((rect.width / 202.0), (rect.height / 202.0));
        canvas.translate((((202.0 - 166.0)) / 2.0), 0.0);
        var lightPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4283745784L);
    return __cascade;
}))();
        var mediumPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4280923894L);
    return __cascade;
}))();
        var darkPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4278278043L);
    return __cascade;
}))();
        var triangleGradient = global::Doroti.Ui.Gradient.linear(new global::Doroti.Ui.Offset((87.2623 + 37.9092), (28.8384 + 123.4389)), new global::Doroti.Ui.Offset((42.9205 + 37.9092), (35.0952 + 123.4389)), new List<global::Doroti.Ui.Color> { new global::Doroti.Ui.Color(1713022L), new global::Doroti.Ui.Color(1712989054L) });
        var trianglePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = triangleGradient;
    return __cascade;
}))();
        var topBeam = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(37.7, 128.9);
    __cascade.lineTo(9.8, 101.0);
    __cascade.lineTo(100.4, 10.4);
    __cascade.lineTo(156.2, 10.4);
    return __cascade;
}))();
        canvas.drawPath(topBeam, lightPaint);
        var middleBeam = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(156.2, 94.0);
    __cascade.lineTo(100.4, 94.0);
    __cascade.lineTo(78.5, 115.9);
    __cascade.lineTo(106.4, 143.8);
    return __cascade;
}))();
        canvas.drawPath(middleBeam, lightPaint);
        var bottomBeam = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(79.5, 170.7);
    __cascade.lineTo(100.4, 191.6);
    __cascade.lineTo(156.2, 191.6);
    __cascade.lineTo(107.4, 142.8);
    return __cascade;
}))();
        canvas.drawPath(bottomBeam, darkPaint);
        canvas.save();
        canvas.transform(new Float64List(new List<double> { 0.7071, -0.7071, 0.0, 0.0, 0.7071, 0.7071, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, -77.697, 98.057, 0.0, 1.0 }));
        canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(59.8, 123.1, 39.4, 39.4), mediumPaint);
        canvas.restore();
        var triangle = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(79.5, 170.7);
    __cascade.lineTo(120.9, 156.4);
    __cascade.lineTo(107.4, 142.8);
    return __cascade;
}))();
        canvas.drawPath(triangle, trianglePaint);
        canvas.restore();
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        offset += ((FlutterLogoDecoration)this._config).margin.topLeft;
        global::Doroti.Ui.Size canvasSize = ((FlutterLogoDecoration)this._config).margin.deflateSize(DartRuntimePrimitives.RequireValue(((ImageConfiguration)configuration).size));
        if (canvasSize.isEmpty)
        {
            return;
        }
        global::Doroti.Ui.Size logoSize = (((FlutterLogoDecoration)this._config)._position switch { > 0.0 => new global::Doroti.Ui.Size(820.0, 232.0), < 0.0 => new global::Doroti.Ui.Size(252.0, 306.0), _ => new global::Doroti.Ui.Size(202.0, 202.0) });
        FittedSizes fittedSize = global::Doroti.Framework.Painting.Box_fitLibrary.applyBoxFit(BoxFit.contain, logoSize, canvasSize);
        DartRuntimePrimitives.Assert(() => (object.Equals(((FittedSizes)fittedSize).source, logoSize)));
        global::Doroti.Ui.Rect rect = Alignment.center.inscribe(((FittedSizes)fittedSize).destination, (offset & canvasSize));
        double centerSquareHeight = canvasSize.shortestSide;
        var centerSquare = global::Doroti.Ui.Rect.fromLTWH((offset.dx + (((canvasSize.width - centerSquareHeight)) / 2.0)), (offset.dy + (((canvasSize.height - centerSquareHeight)) / 2.0)), centerSquareHeight, centerSquareHeight);
        global::Doroti.Ui.Rect logoTargetSquare = default!;
        if ((((FlutterLogoDecoration)this._config)._position > 0.0))
        {
            logoTargetSquare = global::Doroti.Ui.Rect.fromLTWH(rect.left, rect.top, rect.height, rect.height);
        }
        else
        {
            if ((((FlutterLogoDecoration)this._config)._position < 0.0))
            {
                double logoHeight = ((rect.height * 191.0) / 306.0);
                logoTargetSquare = global::Doroti.Ui.Rect.fromLTWH((rect.left + (((rect.width - logoHeight)) / 2.0)), rect.top, logoHeight, logoHeight);
            }
            else
            {
                logoTargetSquare = centerSquare;
            }
        }
        global::Doroti.Ui.Rect logoSquare = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Rect.lerp(centerSquare, logoTargetSquare, ((FlutterLogoDecoration)this._config)._position.abs()));
        if ((((FlutterLogoDecoration)this._config)._opacity < 1.0))
        {
            canvas.saveLayer((offset & canvasSize), ((Func<Paint>)(() =>
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
                double fontSize = (((2.0 / 3.0) * logoSquare.height) * ((1L - (((10.4 * 2.0)) / 202.0))));
                double scaleLocal = (fontSize / 100.0);
                double finalLeftTextPosition = ((((256.4 / 820.0)) * rect.width) - (((32.0 / 350.0)) * fontSize));
                double initialLeftTextPosition = ((rect.width / 2.0) - (this._textBoundingRect.width * scaleLocal));
                var textOffset = new global::Doroti.Ui.Offset((rect.left + DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(initialLeftTextPosition, finalLeftTextPosition, ((FlutterLogoDecoration)this._config)._position))), (rect.top + (((rect.height - (this._textBoundingRect.height * scaleLocal))) / 2.0)));
                canvas.save();
                if ((((FlutterLogoDecoration)this._config)._position < 1.0))
                {
                    global::Doroti.Ui.Offset centerLocal = logoSquare.center;
                    var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(centerLocal.dx, centerLocal.dy);
    __cascade.lineTo((centerLocal.dx + rect.width), (centerLocal.dy - rect.width));
    __cascade.lineTo((centerLocal.dx + rect.width), (centerLocal.dy + rect.width));
    __cascade.close();
    return __cascade;
}))();
                    canvas.clipPath(path);
                }
                canvas.translate(textOffset.dx, textOffset.dy);
                canvas.scale(scaleLocal, scaleLocal);
                this._textPainter.paint(canvas, Offset.zero);
                canvas.restore();
            }
            else
            {
                if ((((FlutterLogoDecoration)this._config)._position < 0.0))
                {
                    double fontSizeLocal = ((0.35 * logoTargetSquare.height) * ((1L - (((10.4 * 2.0)) / 202.0))));
                    double scaleAlternate = (fontSizeLocal / 100.0);
                    if ((((FlutterLogoDecoration)this._config)._position > -1.0))
                    {
                        canvas.saveLayer(this._textBoundingRect, new global::Doroti.Ui.Paint());
                    }
                    else
                    {
                        canvas.save();
                    }
                    canvas.translate((logoTargetSquare.center.dx - (((this._textBoundingRect.width * scaleAlternate) / 2.0))), logoTargetSquare.bottom);
                    canvas.scale(scaleAlternate, scaleAlternate);
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
        _paintLogo(canvas, logoSquare);
        if ((((FlutterLogoDecoration)this._config)._opacity < 1.0))
        {
            canvas.restore();
        }
    }

}


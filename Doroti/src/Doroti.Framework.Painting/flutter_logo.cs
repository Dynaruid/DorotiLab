// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/flutter_logo.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public enum FlutterLogoStyle
{
    markOnly,
    horizontal,
    stacked,
}

public class FlutterLogoDecoration : Decoration
{
    public virtual Color textColor { get; private set; } = default!;
    public virtual FlutterLogoStyle style { get; private set; } = default!;
    public virtual EdgeInsets margin { get; private set; } = default!;
    internal virtual double _position { get; private set; } = default!;
    internal virtual double _opacity { get; private set; } = default!;

    public FlutterLogoDecoration(
        Color textColor = default!,
        FlutterLogoStyle style = FlutterLogoStyle.markOnly,
        EdgeInsets margin = default!
    )
    {
        Color __textColor = textColor ?? new Color(0xFF757575);
        EdgeInsets __margin = margin ?? EdgeInsets.zero;
        this.textColor = __textColor;
        this.style = style;
        this.margin = __margin;
        _position = DartRuntimePrimitives.Identical(style, FlutterLogoStyle.markOnly)
            ? 0.0
            : (DartRuntimePrimitives.Identical(style, FlutterLogoStyle.horizontal) ? 1.0 : -1.0);
        _opacity = 1.0;
    }

    public static FlutterLogoDecoration Create_(
        Color textColor,
        FlutterLogoStyle style,
        EdgeInsets margin,
        double _position,
        double _opacity
    )
    {
        var __instance = new FlutterLogoDecoration(textColor, style, margin);
        __instance.textColor = textColor;
        __instance.style = style;
        __instance.margin = margin;
        __instance._position = _position;
        __instance._opacity = _opacity;
        return __instance;
    }

    internal virtual bool _inTransition =>
        (_opacity != 1.0) || ((_position != -1.0) && (_position != 0.0) && (_position != 1.0));

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() =>
            double.IsFinite(_position) && (_opacity >= 0.0) && (_opacity <= 1.0)
        );
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isComplex => !_inTransition;

    public static FlutterLogoDecoration? lerp(
        FlutterLogoDecoration? a,
        FlutterLogoDecoration? b,
        double t
    )
    {
        DartRuntimePrimitives.Assert(() => (a is null) || a.debugAssertIsValid());
        DartRuntimePrimitives.Assert(() => (b is null) || b.debugAssertIsValid());
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return Create_(
                b!.textColor,
                b.style,
                b.margin.op_Multiply(t),
                b._position,
                b._opacity * Dart_uiLibrary.clampDouble(t, 0.0, 1.0)
            );
        }
        if (b is null)
        {
            return Create_(
                a.textColor,
                a.style,
                a.margin.op_Multiply(t),
                a._position,
                a._opacity * Dart_uiLibrary.clampDouble(1.0 - t, 0.0, 1.0)
            );
        }
        if (t == 0.0)
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        return Create_(
            Dart_uiLibrary.Color.lerp(a.textColor, b.textColor, t)!,
            (t < 0.5) ? a.style : b.style,
            EdgeInsets.lerp(a.margin, b.margin, t)!,
            a._position + ((b._position - a._position) * t),
            Dart_uiLibrary.clampDouble(a._opacity + ((b._opacity - a._opacity) * t), 0.0, 1.0)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FlutterLogoDecoration? lerpFrom(Decoration? a, double t)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if (a is FlutterLogoDecoration)
        {
            FlutterLogoDecoration a__as4619 = (FlutterLogoDecoration)a;
            DartRuntimePrimitives.Assert(() =>
                ((FlutterLogoDecoration?)a__as4619)?.debugAssertIsValid() ?? true
            );
            return lerp((FlutterLogoDecoration?)a__as4619, this, t);
        }
        return ((FlutterLogoDecoration?)(object?)base.lerpFrom(a, t))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FlutterLogoDecoration? lerpTo(Decoration? b, double t)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if (b is FlutterLogoDecoration)
        {
            FlutterLogoDecoration b__as4933 = (FlutterLogoDecoration)b;
            DartRuntimePrimitives.Assert(() =>
                ((FlutterLogoDecoration?)b__as4933)?.debugAssertIsValid() ?? true
            );
            return lerp(this, (FlutterLogoDecoration?)b__as4933, t);
        }
        return ((FlutterLogoDecoration?)(object?)base.lerpTo(b, t))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Size size, Offset position, TextDirection? textDirection = null) =>
        true;

    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return new _FlutterLogoPainter__flutter_logo(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
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

    public override bool Equals(object? other)
    {
        var __other = other as FlutterLogoDecoration;
        if (__other is null)
        {
            return false;
        }

        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is FlutterLogoDecoration)
            && Equals(__other.textColor, textColor)
            && (__other._position == _position)
            && (__other._opacity == _opacity);
    }

    public override int GetHashCode()
    {
        DartRuntimePrimitives.Assert(() => debugAssertIsValid());
        return FoundationRuntimePorts.ObjectHash(textColor, _position, _opacity);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new ColorProperty("textColor", textColor));
        properties.add(new EnumProperty<FlutterLogoStyle>("style", style));
        if (_inTransition)
        {
            properties.add(
                new DiagnosticsNode(
                    $"transition {Foundation.DebugLibrary.debugFormatDouble(_position)}:{Foundation.DebugLibrary.debugFormatDouble(_opacity)}"
                )
            );
        }
    }
}

internal class _FlutterLogoPainter__flutter_logo : BoxPainter
{
    internal virtual FlutterLogoDecoration _config { get; private set; } = default!;
    internal virtual TextPainter _textPainter { get; set; } = default!;
    internal virtual Rect _textBoundingRect { get; set; } = default!;

    internal _FlutterLogoPainter__flutter_logo(FlutterLogoDecoration _config)
        : base(null)
    {
        this._config = _config;
        System.Diagnostics.Debug.Assert(_config.debugAssertIsValid());
    }

    public override void dispose()
    {
        _textPainter.dispose();
        base.dispose();
    }

    internal virtual void _prepareText()
    {
        var kLabel = "Flutter";
        _textPainter = new TextPainter(
            text: new TextSpan(
                text: kLabel,
                style: new TextStyle(
                    color: _config.textColor,
                    fontFamily: "Roboto",
                    fontSize: 100.0 * 350.0 / 247.0,
                    fontWeight: FontWeight.w300,
                    textBaseline: TextBaseline.alphabetic
                )
            ),
            textDirection: TextDirection.ltr
        );
        _textPainter.layout();
        TextBox textSize = _textPainter
            .getBoxesForSelection(new TextSelection(baseOffset: 0L, extentOffset: kLabel.Length))
            .Single();
        _textBoundingRect = Rect.fromLTRB(
            textSize.left,
            textSize.top,
            textSize.right,
            textSize.bottom
        );
    }

    internal virtual void _paintLogo(Canvas canvas, Rect rect)
    {
        canvas.save();
        canvas.translate(rect.left, rect.top);
        canvas.scale(rect.width / 202.0, rect.height / 202.0);
        canvas.translate((202.0 - 166.0) / 2.0, 0.0);
        var lightPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = new Color(4283745784L);
                    return __cascade;
                }
            )
        )();
        var mediumPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = new Color(4280923894L);
                    return __cascade;
                }
            )
        )();
        var darkPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = new Color(4278278043L);
                    return __cascade;
                }
            )
        )();
        var triangleGradient = Ui.Gradient.linear(
            new Offset(87.2623 + 37.9092, 28.8384 + 123.4389),
            new Offset(42.9205 + 37.9092, 35.0952 + 123.4389),
            new List<Color> { new Color(1713022L), new Color(1712989054L) }
        );
        var trianglePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.shader = triangleGradient;
                    return __cascade;
                }
            )
        )();
        var topBeam = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.moveTo(37.7, 128.9);
                    __cascade.lineTo(9.8, 101.0);
                    __cascade.lineTo(100.4, 10.4);
                    __cascade.lineTo(156.2, 10.4);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(topBeam, lightPaint);
        var middleBeam = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.moveTo(156.2, 94.0);
                    __cascade.lineTo(100.4, 94.0);
                    __cascade.lineTo(78.5, 115.9);
                    __cascade.lineTo(106.4, 143.8);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(middleBeam, lightPaint);
        var bottomBeam = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.moveTo(79.5, 170.7);
                    __cascade.lineTo(100.4, 191.6);
                    __cascade.lineTo(156.2, 191.6);
                    __cascade.lineTo(107.4, 142.8);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(bottomBeam, darkPaint);
        canvas.save();
        canvas.transform(
            new Float64List(
                new List<double>
                {
                    0.7071,
                    -0.7071,
                    0.0,
                    0.0,
                    0.7071,
                    0.7071,
                    0.0,
                    0.0,
                    0.0,
                    0.0,
                    1.0,
                    0.0,
                    -77.697,
                    98.057,
                    0.0,
                    1.0,
                }
            )
        );
        canvas.drawRect(Rect.fromLTWH(59.8, 123.1, 39.4, 39.4), mediumPaint);
        canvas.restore();
        var triangle = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.moveTo(79.5, 170.7);
                    __cascade.lineTo(120.9, 156.4);
                    __cascade.lineTo(107.4, 142.8);
                    return __cascade;
                }
            )
        )();
        canvas.drawPath(triangle, trianglePaint);
        canvas.restore();
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        offset += _config.margin.topLeft;
        Size canvasSize = _config.margin.deflateSize(
            (
                configuration.size
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        if (canvasSize.isEmpty)
        {
            return;
        }
        Size logoSize = _config._position switch
        {
            > 0.0 => new Size(820.0, 232.0),
            < 0.0 => new Size(252.0, 306.0),
            _ => new Size(202.0, 202.0),
        };
        FittedSizes fittedSize = Box_fitLibrary.applyBoxFit(BoxFit.contain, logoSize, canvasSize);
        DartRuntimePrimitives.Assert(() => Equals(fittedSize.source, logoSize));
        Rect rect = Alignment.center.inscribe(fittedSize.destination, offset & canvasSize);
        double centerSquareHeight = canvasSize.shortestSide;
        var centerSquare = Rect.fromLTWH(
            offset.dx + ((canvasSize.width - centerSquareHeight) / 2.0),
            offset.dy + ((canvasSize.height - centerSquareHeight) / 2.0),
            centerSquareHeight,
            centerSquareHeight
        );
        Rect logoTargetSquare = default!;
        if (_config._position > 0.0)
        {
            logoTargetSquare = Rect.fromLTWH(rect.left, rect.top, rect.height, rect.height);
        }
        else
        {
            if (_config._position < 0.0)
            {
                double logoHeight = rect.height * 191.0 / 306.0;
                logoTargetSquare = Rect.fromLTWH(
                    rect.left + ((rect.width - logoHeight) / 2.0),
                    rect.top,
                    logoHeight,
                    logoHeight
                );
            }
            else
            {
                logoTargetSquare = centerSquare;
            }
        }
        Rect logoSquare = (
            Dart_uiLibrary.Rect.lerp(centerSquare, logoTargetSquare, _config._position.abs())
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        if (_config._opacity < 1.0)
        {
            canvas.saveLayer(
                offset & canvasSize,
                (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.colorFilter = ColorFilter.mode(
                                new Color(4294967295L).withOpacity(_config._opacity),
                                BlendMode.modulate
                            );
                            return __cascade;
                        }
                    )
                )()
            );
        }
        if (_config._position != 0.0)
        {
            if (_config._position > 0.0)
            {
                double fontSize = 2.0 / 3.0 * logoSquare.height * (1L - (10.4 * 2.0 / 202.0));
                double scaleLocal = fontSize / 100.0;
                double finalLeftTextPosition =
                    (256.4 / 820.0 * rect.width) - (32.0 / 350.0 * fontSize);
                double initialLeftTextPosition =
                    (rect.width / 2.0) - (_textBoundingRect.width * scaleLocal);
                var textOffset = new Offset(
                    rect.left
                        + (
                            Dart_uiLibrary.lerpDouble(
                                initialLeftTextPosition,
                                finalLeftTextPosition,
                                _config._position
                            )
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                    rect.top + ((rect.height - (_textBoundingRect.height * scaleLocal)) / 2.0)
                );
                canvas.save();
                if (_config._position < 1.0)
                {
                    Offset centerLocal = logoSquare.center;
                    var path = (
                        (Func<Path>)(
                            () =>
                            {
                                var __cascade = new Path();
                                __cascade.moveTo(centerLocal.dx, centerLocal.dy);
                                __cascade.lineTo(
                                    centerLocal.dx + rect.width,
                                    centerLocal.dy - rect.width
                                );
                                __cascade.lineTo(
                                    centerLocal.dx + rect.width,
                                    centerLocal.dy + rect.width
                                );
                                __cascade.close();
                                return __cascade;
                            }
                        )
                    )();
                    canvas.clipPath(path);
                }
                canvas.translate(textOffset.dx, textOffset.dy);
                canvas.scale(scaleLocal, scaleLocal);
                _textPainter.paint(canvas, Offset.zero);
                canvas.restore();
            }
            else
            {
                if (_config._position < 0.0)
                {
                    double fontSizeLocal =
                        0.35 * logoTargetSquare.height * (1L - (10.4 * 2.0 / 202.0));
                    double scaleAlternate = fontSizeLocal / 100.0;
                    if (_config._position > -1.0)
                    {
                        canvas.saveLayer(_textBoundingRect, new Paint());
                    }
                    else
                    {
                        canvas.save();
                    }
                    canvas.translate(
                        logoTargetSquare.center.dx
                            - (_textBoundingRect.width * scaleAlternate / 2.0),
                        logoTargetSquare.bottom
                    );
                    canvas.scale(scaleAlternate, scaleAlternate);
                    _textPainter.paint(canvas, Offset.zero);
                    if (_config._position > -1.0)
                    {
                        canvas.drawRect(
                            _textBoundingRect.inflate(_textBoundingRect.width * 0.5),
                            (
                                (Func<Paint>)(
                                    () =>
                                    {
                                        var __cascade = new Paint();
                                        __cascade.blendMode = BlendMode.modulate;
                                        __cascade.shader = Ui.Gradient.linear(
                                            new Offset(_textBoundingRect.width * -0.5, 0.0),
                                            new Offset(_textBoundingRect.width * 1.5, 0.0),
                                            new List<Color>
                                            {
                                                new Color(4294967295L),
                                                new Color(4294967295L),
                                                new Color(16777215L),
                                                new Color(16777215L),
                                            },
                                            new List<double>
                                            {
                                                0.0,
                                                Math.Max(0.0, _config._position.abs() - 0.1),
                                                Math.Min(_config._position.abs() + 0.1, 1.0),
                                                1.0,
                                            }
                                        );
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                    }
                    canvas.restore();
                }
            }
        }
        _paintLogo(canvas, logoSquare);
        if (_config._opacity < 1.0)
        {
            canvas.restore();
        }
    }
}

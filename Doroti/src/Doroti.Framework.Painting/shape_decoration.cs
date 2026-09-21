// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/shape_decoration.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class ShapeDecoration : Decoration
{
    public virtual Color? color { get; private set; }
    public virtual Gradient? gradient { get; private set; }
    public virtual DecorationImage? image { get; private set; }
    public virtual List<BoxShadow>? shadows { get; private set; }
    public virtual ShapeBorder shape { get; private set; } = default!;

    public ShapeDecoration(
        Color? color = null,
        DecorationImage? image = null,
        Gradient? gradient = null,
        List<BoxShadow>? shadows = null,
        ShapeBorder shape = default!
    )
    {
        this.color = color;
        this.image = image;
        this.gradient = gradient;
        this.shadows = shadows;
        this.shape = shape;
        System.Diagnostics.Debug.Assert(!((color is not null) && (gradient is not null)));
    }

    public static ShapeDecoration CreateFromBoxDecoration(BoxDecoration source)
    {
        ShapeBorder shapeLocal = default!;
        switch (source.shape)
        {
            case BoxShape.circle:
            {
                if (source.border is not null)
                {
                    DartRuntimePrimitives.Assert(() => source.border!.isUniform);
                    shapeLocal = new CircleBorder(side: source.border!.top);
                }
                else
                {
                    shapeLocal = new CircleBorder();
                }
                break;
            }
            case BoxShape.rectangle:
            {
                if (source.borderRadius is not null)
                {
                    DartRuntimePrimitives.Assert(() =>
                        (source.border is null) || source.border!.isUniform
                    );
                    shapeLocal = new RoundedRectangleBorder(
                        side: source.border?.top ?? BorderSide.none,
                        borderRadius: source.borderRadius!
                    );
                }
                else
                {
                    shapeLocal = source.border ?? new Border();
                }
                break;
            }
        }
        return new ShapeDecoration(
            color: source.color,
            image: source.image,
            gradient: source.gradient,
            shadows: source.boxShadow,
            shape: shapeLocal
        );
    }

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        return shape.getOuterPath(rect, textDirection: (textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry padding => shape.dimensions;
    public override bool isComplex => shadows is not null;

    public override ShapeDecoration? lerpFrom(Decoration? a, double t)
    {
        return a switch
        {
            BoxDecoration __object6540 => lerp(CreateFromBoxDecoration(__object6540), this, t),
            ShapeDecoration __typed6634 => lerp((ShapeDecoration?)__typed6634, this, t),
            _ => ((ShapeDecoration?)base.lerpFrom(a, t))!,
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeDecoration? lerpTo(Decoration? b, double t)
    {
        return b switch
        {
            BoxDecoration __object6850 => lerp(this, CreateFromBoxDecoration(__object6850), t),
            ShapeDecoration __typed6944 => lerp(this, (ShapeDecoration?)__typed6944, t),
            _ => ((ShapeDecoration?)base.lerpTo(b, t))!,
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ShapeDecoration? lerp(ShapeDecoration? a, ShapeDecoration? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is not null) && (b is not null))
        {
            if (t == 0.0)
            {
                return a;
            }
            if (t == 1.0)
            {
                return b;
            }
        }
        Gradient? aGradient = a?.gradient;
        Gradient? bGradient = b?.gradient;
        if ((aGradient is null) && (bGradient is not null) && (a?.color is not null))
        {
            aGradient = bGradient.fromColor(a!.color!);
        }
        else
        {
            if ((bGradient is null) && (aGradient is not null) && (b?.color is not null))
            {
                bGradient = aGradient.fromColor(b!.color!);
            }
        }
        Gradient? gradientLocal = Gradient.lerp(aGradient, bGradient, t);
        return new ShapeDecoration(
            color: (gradientLocal is null)
                ? Dart_uiLibrary.Color.lerp(a?.color, b?.color, t)
                : null,
            gradient: gradientLocal,
            image: DecorationImage.lerp(a?.image, b?.image, t),
            shadows: BoxShadow.lerpList(a?.shadows, b?.shadows, t),
            shape: ShapeBorder.lerp(a?.shape, b?.shape, t)!
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ShapeDecoration;
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
        return (__other is ShapeDecoration)
            && Equals(__other.color, color)
            && Equals(__other.gradient, gradient)
            && Equals(__other.image, image)
            && CollectionsLibrary.listEquals(__other.shadows, shadows)
            && Equals(__other.shape, shape);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            color,
            gradient,
            image,
            shape,
            (shadows is null) ? null : FoundationRuntimePorts.ObjectHashAll(shadows!)
        );

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new DiagnosticsProperty<Gradient>("gradient", gradient, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<DecorationImage>("image", image, defaultValue: null)
        );
        properties.add(
            new IterableProperty<BoxShadow>(
                "shadows",
                shadows,
                defaultValue: null,
                style: DiagnosticsTreeStyle.whitespace
            )
        );
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape));
    }

    public override bool hitTest(Size size, Offset position, TextDirection? textDirection = null)
    {
        return shape.hitTest(Offset.zero & size, position, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        DartRuntimePrimitives.Assert(() => (onChanged is not null) || (image is null));
        return new _ShapeDecorationPainter__shape_decoration(this, onChanged!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ShapeDecorationPainter__shape_decoration : BoxPainter
{
    internal virtual ShapeDecoration _decoration { get; private set; } = default!;
    internal virtual Rect? _lastRect { get; set; } = default;
    internal virtual TextDirection? _lastTextDirection { get; set; } = default;
    internal virtual Path _outerPath { get; set; } = default!;
    internal virtual Path? _innerPath { get; set; } = default;
    internal virtual Paint? _interiorPaint { get; set; } = default;
    internal virtual long? _shadowCount { get; set; } = default;
    internal virtual List<Rect> _shadowBounds { get; set; } = default!;
    internal virtual List<Path> _shadowPaths { get; set; } = default!;
    internal virtual List<Paint> _shadowPaints { get; set; } = default!;
    internal virtual DecorationImagePainter? _imagePainter { get; set; } = default;

    internal _ShapeDecorationPainter__shape_decoration(
        ShapeDecoration _decoration,
        Action onChanged
    )
        : base(onChanged)
    {
        this._decoration = _decoration;
    }

    public override Action onChanged => DartRuntimePrimitives.RequireReference(base.onChanged);

    internal virtual void _precache(Rect rect, TextDirection? textDirection)
    {
        if (Equals(rect, _lastRect) && Equals(textDirection, _lastTextDirection))
        {
            return;
        }
        if (
            (_interiorPaint is null)
            && ((_decoration.color is not null) || (_decoration.gradient is not null))
        )
        {
            _interiorPaint = new Paint();
            if (_decoration.color is not null)
            {
                _interiorPaint!.color = _decoration.color!;
            }
        }
        if (_decoration.gradient is not null)
        {
            _interiorPaint!.shader = _decoration.gradient!.createShader(
                rect,
                textDirection: textDirection
            );
        }
        if (_decoration.shadows is not null)
        {
            if (_shadowCount is null)
            {
                _shadowCount = checked(_decoration.shadows!.Count);
                _shadowPaints = _decoration.shadows!.Select(shadow => shadow.toPaint()).ToList();
            }
            if (_decoration.shape.preferPaintInterior)
            {
                _shadowBounds = _decoration
                    .shadows!.Select(shadow =>
                        rect.shift(shadow.offset).inflate(shadow.spreadRadius)
                    )
                    .ToList();
            }
            else
            {
                _shadowPaths = _decoration
                    .shadows!.Select(shadow =>
                        _decoration.shape.getOuterPath(
                            rect.shift(shadow.offset).inflate(shadow.spreadRadius),
                            textDirection: textDirection
                        )
                    )
                    .ToList();
            }
        }
        if (
            !_decoration.shape.preferPaintInterior
            && ((_interiorPaint is not null) || (_shadowCount is not null))
        )
        {
            _outerPath = _decoration.shape.getOuterPath(rect, textDirection: textDirection);
        }
        if (_decoration.image is not null)
        {
            _innerPath = _decoration.shape.getInnerPath(rect, textDirection: textDirection);
        }
        _lastRect = rect;
        _lastTextDirection = textDirection;
    }

    internal virtual void _paintShadows(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        bool debugHandleDisabledShadowStart(Canvas canvas, BoxShadow boxShadow, Path path)
        {
            if (DebugLibrary.debugDisableShadows && Equals(boxShadow.blurStyle, BlurStyle.outer))
            {
                canvas.save();
                var clipPathLocal = new Path();
                clipPathLocal.fillType = PathFillType.evenOdd;
                clipPathLocal.addRect(Rect.largest);
                clipPathLocal.addPath(path, Offset.zero);
                canvas.clipPath(clipPathLocal);
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool debugHandleDisabledShadowEnd(Canvas canvas, BoxShadow boxShadow)
        {
            if (DebugLibrary.debugDisableShadows && Equals(boxShadow.blurStyle, BlurStyle.outer))
            {
                canvas.restore();
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (_shadowCount is not null)
        {
            if (_decoration.shape.preferPaintInterior)
            {
                for (
                    var index = 0L;
                    index
                        < (
                            _shadowCount
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        );
                    index += 1L
                )
                {
                    DartRuntimePrimitives.Assert(() =>
                        debugHandleDisabledShadowStart(
                            canvas,
                            _decoration.shadows![(int)index],
                            _decoration.shape.getOuterPath(
                                _shadowBounds[(int)index],
                                textDirection: textDirection
                            )
                        )
                    );
                    _decoration.shape.paintInterior(
                        canvas,
                        _shadowBounds[(int)index],
                        _shadowPaints[(int)index],
                        textDirection: textDirection
                    );
                    DartRuntimePrimitives.Assert(() =>
                        debugHandleDisabledShadowEnd(canvas, _decoration.shadows![(int)index])
                    );
                }
            }
            else
            {
                for (
                    var indexLocal = 0L;
                    indexLocal
                        < (
                            _shadowCount
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        );
                    indexLocal += 1L
                )
                {
                    DartRuntimePrimitives.Assert(() =>
                        debugHandleDisabledShadowStart(
                            canvas,
                            _decoration.shadows![(int)indexLocal],
                            _shadowPaths[(int)indexLocal]
                        )
                    );
                    canvas.drawPath(_shadowPaths[(int)indexLocal], _shadowPaints[(int)indexLocal]);
                    DartRuntimePrimitives.Assert(() =>
                        debugHandleDisabledShadowEnd(canvas, _decoration.shadows![(int)indexLocal])
                    );
                }
            }
        }
    }

    internal virtual void _paintInterior(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        if (_interiorPaint is not null)
        {
            if (_decoration.shape.preferPaintInterior)
            {
                Rect adjustedRect = _adjustedRectOnOutlinedBorder(rect);
                _decoration.shape.paintInterior(
                    canvas,
                    adjustedRect,
                    _interiorPaint!,
                    textDirection: textDirection
                );
            }
            else
            {
                canvas.drawPath(_outerPath, _interiorPaint!);
            }
        }
    }

    internal virtual Rect _adjustedRectOnOutlinedBorder(Rect rect)
    {
        if ((_decoration.shape is OutlinedBorder) && (_decoration.color is not null))
        {
            BorderSide sideLocal = ((OutlinedBorder?)_decoration.shape)!.side;
            if ((sideLocal.color.alpha == 255L) && Equals(sideLocal.style, BorderStyle.solid))
            {
                return rect.deflate(sideLocal.strokeInset / 2L);
            }
        }
        return rect;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintImage(Canvas canvas, ImageConfiguration configuration)
    {
        if (_decoration.image is null)
        {
            return;
        }
        _imagePainter ??= _decoration.image!.createPainter(onChanged);
        _imagePainter!.paint(
            canvas,
            (
                _lastRect
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            _innerPath,
            configuration
        );
    }

    public override void dispose()
    {
        _imagePainter?.dispose();
        base.dispose();
    }

    public override void paint(Canvas canvas, Offset offset, ImageConfiguration configuration)
    {
        DartRuntimePrimitives.Assert(() => configuration.size is not null);
        Rect rect =
            offset
            & (
                configuration.size
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        TextDirection? textDirectionLocal = configuration.textDirection;
        _precache(rect, textDirectionLocal);
        _paintShadows(canvas, rect, textDirectionLocal);
        _paintInterior(canvas, rect, textDirectionLocal);
        _paintImage(canvas, configuration);
        _decoration.shape.paint(canvas, rect, textDirection: textDirectionLocal);
    }
}

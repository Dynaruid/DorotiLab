// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_decoration.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class BoxDecoration : Decoration
{
    public virtual Color? color { get; private set; }
    public virtual DecorationImage? image { get; private set; }
    public virtual BoxBorder? border { get; private set; }
    public virtual BorderRadiusGeometry? borderRadius { get; private set; }
    public virtual List<BoxShadow>? boxShadow { get; private set; }
    public virtual Gradient? gradient { get; private set; }
    public virtual BlendMode? backgroundBlendMode { get; private set; }
    public virtual BoxShape shape { get; private set; } = default!;

    public BoxDecoration(
        Color? color = null,
        DecorationImage? image = null,
        BoxBorder? border = null,
        BorderRadiusGeometry? borderRadius = null,
        List<BoxShadow>? boxShadow = null,
        Gradient? gradient = null,
        BlendMode? backgroundBlendMode = null,
        BoxShape shape = BoxShape.rectangle
    )
    {
        this.color = color;
        this.image = image;
        this.border = border;
        this.borderRadius = borderRadius;
        this.boxShadow = boxShadow;
        this.gradient = gradient;
        this.backgroundBlendMode = backgroundBlendMode;
        this.shape = shape;
        System.Diagnostics.Debug.Assert(
            (backgroundBlendMode is null) || (color is not null) || (gradient is not null)
        );
    }

    public virtual BoxDecoration copyWith(
        Color? color = null,
        DecorationImage? image = null,
        BoxBorder? border = null,
        BorderRadiusGeometry? borderRadius = null,
        List<BoxShadow>? boxShadow = null,
        Gradient? gradient = null,
        BlendMode? backgroundBlendMode = null,
        BoxShape? shape = null
    )
    {
        return new BoxDecoration(
            color: color ?? this.color,
            image: image ?? this.image,
            border: border ?? this.border,
            borderRadius: borderRadius ?? this.borderRadius,
            boxShadow: boxShadow ?? this.boxShadow,
            gradient: gradient ?? this.gradient,
            backgroundBlendMode: backgroundBlendMode ?? this.backgroundBlendMode,
            shape: shape ?? this.shape
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() =>
            (!Equals(shape, BoxShape.circle)) || (borderRadius is null)
        );
        return base.debugAssertIsValid();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override EdgeInsetsGeometry padding => border?.dimensions ?? EdgeInsets.zero;

    public override Path getClipPath(Rect rect, TextDirection textDirection)
    {
        switch (shape)
        {
            case BoxShape.circle:
            {
                Offset centerLocal = rect.center;
                double radiusLocal = rect.shortestSide / 2.0;
                var square = Rect.fromCircle(center: centerLocal, radius: radiusLocal);
                return (
                    (Func<Path>)(
                        () =>
                        {
                            var __cascade = new Path();
                            __cascade.addOval(square);
                            return __cascade;
                        }
                    )
                )();
            }
            case BoxShape.rectangle:
            {
                if (borderRadius is not null)
                {
                    return (
                        (Func<Path>)(
                            () =>
                            {
                                var __cascade = new Path();
                                __cascade.addRRect(
                                    borderRadius!.resolve((textDirection)).toRRect(rect)
                                );
                                return __cascade;
                            }
                        )
                    )();
                }
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
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual BoxDecoration scale(double factor)
    {
        return new BoxDecoration(
            color: DorotiUiLibrary.Color.lerp(null, color, factor),
            image: DecorationImage.lerp(null, image, factor),
            border: BoxBorder.lerp(null, border, factor),
            borderRadius: BorderRadiusGeometry.lerp(null, borderRadius, factor),
            boxShadow: BoxShadow.lerpList(null, boxShadow, factor),
            gradient: gradient?.scale(factor),
            shape: shape
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool isComplex => boxShadow is not null;

    public override BoxDecoration? lerpFrom(Decoration? a, double t) =>
        a switch
        {
            null => scale(t),
            BoxDecoration __object9248 => lerp(__object9248, this, t),
            _ => ((BoxDecoration?)(object?)base.lerpFrom(a, t))!,
        };

    public override BoxDecoration? lerpTo(Decoration? b, double t) =>
        b switch
        {
            null => scale(1.0 - t),
            BoxDecoration __object9463 => lerp(this, __object9463, t),
            _ => ((BoxDecoration?)(object?)base.lerpTo(b, t))!,
        };

    public static BoxDecoration? lerp(BoxDecoration? a, BoxDecoration? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!.scale(t);
        }
        if (b is null)
        {
            return a.scale(1.0 - t);
        }
        if (t == 0.0)
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        return new BoxDecoration(
            color: DorotiUiLibrary.Color.lerp(a.color, b.color, t),
            image: DecorationImage.lerp(a.image, b.image, t),
            border: BoxBorder.lerp(a.border, b.border, t),
            borderRadius: BorderRadiusGeometry.lerp(a.borderRadius, b.borderRadius, t),
            boxShadow: BoxShadow.lerpList(a.boxShadow, b.boxShadow, t),
            gradient: Gradient.lerp(a.gradient, b.gradient, t),
            shape: (t < 0.5) ? a.shape : b.shape
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as BoxDecoration;
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
        return (__other is BoxDecoration)
            && Equals(__other.color, color)
            && Equals(__other.image, image)
            && Equals(__other.border, border)
            && Equals(__other.borderRadius, borderRadius)
            && CollectionsLibrary.listEquals(__other.boxShadow, boxShadow)
            && Equals(__other.gradient, gradient)
            && Equals(__other.backgroundBlendMode, backgroundBlendMode)
            && Equals(__other.shape, shape);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            color,
            image,
            border,
            borderRadius,
            (boxShadow is null) ? null : FoundationRuntimePorts.ObjectHashAll(boxShadow!),
            gradient,
            backgroundBlendMode,
            shape
        );

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        (
            (Func<DiagnosticPropertiesBuilder>)(
                () =>
                {
                    var __cascade = properties;
                    __cascade.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
                    __cascade.emptyBodyDescription = "<no decorations specified>";
                    return __cascade;
                }
            )
        )();
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<DecorationImage>("image", image, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<BoxBorder>("border", border, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<BorderRadiusGeometry>(
                "borderRadius",
                borderRadius,
                defaultValue: null
            )
        );
        properties.add(
            new IterableProperty<BoxShadow>(
                "boxShadow",
                boxShadow,
                defaultValue: null,
                style: DiagnosticsTreeStyle.whitespace
            )
        );
        properties.add(new DiagnosticsProperty<Gradient>("gradient", gradient, defaultValue: null));
        properties.add(
            new EnumProperty<BoxShape>("shape", shape, defaultValue: BoxShape.rectangle)
        );
    }

    public override bool hitTest(Size size, Offset position, TextDirection? textDirection = null)
    {
        DartRuntimePrimitives.Assert(() => (Offset.zero & size).contains(position));
        switch (shape)
        {
            case BoxShape.rectangle:
            {
                if (borderRadius is not null)
                {
                    RRect bounds = borderRadius!.resolve(textDirection).toRRect(Offset.zero & size);
                    return bounds.contains(position);
                }
                return true;
            }
            case BoxShape.circle:
            {
                Offset centerLocal = size.center(Offset.zero);
                double radius = Math.Min(size.width, size.height) / 2.0;
                return (position - centerLocal).distanceSquared <= (radius * radius);
            }
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override BoxPainter createBoxPainter(Action onChanged = default!)
    {
        DartRuntimePrimitives.Assert(() => (onChanged is not null) || (image is null));
        return new _BoxDecorationPainter__box_decoration(this, onChanged);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _BoxDecorationPainter__box_decoration : BoxPainter
{
    internal virtual BoxDecoration _decoration { get; private set; } = default!;
    internal virtual Paint? _cachedBackgroundPaint { get; set; } = default;
    internal virtual Rect? _rectForCachedBackgroundPaint { get; set; } = default;
    internal virtual DecorationImagePainter? _imagePainter { get; set; } = default;

    internal _BoxDecorationPainter__box_decoration(BoxDecoration _decoration, Action? onChanged)
        : base(onChanged)
    {
        this._decoration = _decoration;
    }

    internal virtual Paint _getBackgroundPaint(Rect rect, TextDirection? textDirection)
    {
        DartRuntimePrimitives.Assert(() =>
            (_decoration.gradient is not null) || (_rectForCachedBackgroundPaint is null)
        );
        if (
            (_cachedBackgroundPaint is null)
            || (
                (_decoration.gradient is not null) && (!Equals(_rectForCachedBackgroundPaint, rect))
            )
        )
        {
            var paint = new Paint();
            if (_decoration.backgroundBlendMode is not null)
            {
                paint.blendMode = (
                    _decoration.backgroundBlendMode
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            }
            if (_decoration.color is not null)
            {
                paint.color = _decoration.color!;
            }
            if (_decoration.gradient is not null)
            {
                paint.shader = _decoration.gradient!.createShader(
                    rect,
                    textDirection: textDirection
                );
                _rectForCachedBackgroundPaint = rect;
            }
            _cachedBackgroundPaint = paint;
        }
        return _cachedBackgroundPaint!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintBox(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection
    )
    {
        switch (_decoration.shape)
        {
            case BoxShape.circle:
            {
                DartRuntimePrimitives.Assert(() => _decoration.borderRadius is null);
                Offset centerLocal = rect.center;
                double radius = rect.shortestSide / 2.0;
                canvas.drawCircle(centerLocal, radius, paint);
                break;
            }
            case BoxShape.rectangle:
            {
                if (
                    (_decoration.borderRadius is null)
                    || Equals(_decoration.borderRadius, BorderRadius.zero)
                )
                {
                    canvas.drawRect(rect, paint);
                }
                else
                {
                    canvas.drawRRect(
                        _decoration.borderRadius!.resolve(textDirection).toRRect(rect),
                        paint
                    );
                }
                break;
            }
        }
    }

    internal virtual void _paintShadows(Canvas canvas, Rect rect, TextDirection? textDirection)
    {
        if (_decoration.boxShadow is null)
        {
            return;
        }
        foreach (BoxShadow boxShadowLocal in _decoration.boxShadow!)
        {
            Paint paint = boxShadowLocal.toPaint();
            Rect bounds = rect.shift(boxShadowLocal.offset).inflate(boxShadowLocal.spreadRadius);
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    DebugLibrary.debugDisableShadows
                    && Equals(boxShadowLocal.blurStyle, BlurStyle.outer)
                )
                {
                    canvas.save();
                    canvas.clipRect(bounds);
                }
                return true;
            });
            _paintBox(canvas, bounds, paint, textDirection);
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    DebugLibrary.debugDisableShadows
                    && Equals(boxShadowLocal.blurStyle, BlurStyle.outer)
                )
                {
                    canvas.restore();
                }
                return true;
            });
        }
    }

    internal virtual void _paintBackgroundColor(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection
    )
    {
        if ((_decoration.color is not null) || (_decoration.gradient is not null))
        {
            Rect adjustedRect = _adjustedRectOnOutlinedBorder(rect, textDirection);
            _paintBox(
                canvas,
                adjustedRect,
                _getBackgroundPaint(rect, textDirection),
                textDirection
            );
        }
    }

    internal virtual double _calculateAdjustedSide(BorderSide side)
    {
        if ((side.color.alpha == 255L) && Equals(side.style, BorderStyle.solid))
        {
            return side.strokeInset;
        }
        return 0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Rect _adjustedRectOnOutlinedBorder(Rect rect, TextDirection? textDirection)
    {
        if (_decoration.border is null)
        {
            return rect;
        }
        if (_decoration.border is Border)
        {
            var borderLocal = ((Border?)(object?)_decoration.border!)!;
            EdgeInsets insets = new EdgeInsets(
                _calculateAdjustedSide(borderLocal.left),
                _calculateAdjustedSide(borderLocal.top),
                _calculateAdjustedSide(borderLocal.right),
                _calculateAdjustedSide(borderLocal.bottom)
            ).op_Divide(2);
            return Rect.fromLTRB(
                rect.left + insets.left,
                rect.top + insets.top,
                rect.right - insets.right,
                rect.bottom - insets.bottom
            );
        }
        else
        {
            if ((_decoration.border is BorderDirectional) && (textDirection is not null))
            {
                TextDirection textDirection__value18244 = (
                    textDirection
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                var borderAlternate = ((BorderDirectional?)(object?)_decoration.border!)!;
                BorderSide leftSide = Equals((textDirection__value18244), TextDirection.rtl)
                    ? borderAlternate.end
                    : borderAlternate.start;
                BorderSide rightSide = Equals((textDirection__value18244), TextDirection.rtl)
                    ? borderAlternate.start
                    : borderAlternate.end;
                EdgeInsets insetsLocal = new EdgeInsets(
                    _calculateAdjustedSide(leftSide),
                    _calculateAdjustedSide(borderAlternate.top),
                    _calculateAdjustedSide(rightSide),
                    _calculateAdjustedSide(borderAlternate.bottom)
                ).op_Divide(2);
                return Rect.fromLTRB(
                    rect.left + insetsLocal.left,
                    rect.top + insetsLocal.top,
                    rect.right - insetsLocal.right,
                    rect.bottom - insetsLocal.bottom
                );
            }
        }
        return rect;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintBackgroundImage(
        Canvas canvas,
        Rect rect,
        ImageConfiguration configuration
    )
    {
        if (_decoration.image is null)
        {
            return;
        }
        _imagePainter ??= _decoration.image!.createPainter(onChanged!);
        Path? clipPath = default!;
        switch (_decoration.shape)
        {
            case BoxShape.circle:
            {
                DartRuntimePrimitives.Assert(() => _decoration.borderRadius is null);
                Offset centerLocal = rect.center;
                double radiusLocal = rect.shortestSide / 2.0;
                var square = Rect.fromCircle(center: centerLocal, radius: radiusLocal);
                clipPath = (
                    (Func<Path>)(
                        () =>
                        {
                            var __cascade = new Path();
                            __cascade.addOval(square);
                            return __cascade;
                        }
                    )
                )();
                break;
            }
            case BoxShape.rectangle:
            {
                if (_decoration.borderRadius is not null)
                {
                    clipPath = (
                        (Func<Path>)(
                            () =>
                            {
                                var __cascade = new Path();
                                __cascade.addRRect(
                                    _decoration
                                        .borderRadius!.resolve(configuration.textDirection)
                                        .toRRect(rect)
                                );
                                return __cascade;
                            }
                        )
                    )();
                }
                break;
            }
        }
        _imagePainter!.paint(canvas, rect, clipPath, configuration);
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
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        TextDirection? textDirectionLocal = configuration.textDirection;
        _paintShadows(canvas, rect, textDirectionLocal);
        _paintBackgroundColor(canvas, rect, textDirectionLocal);
        _paintBackgroundImage(canvas, rect, configuration);
        _decoration.border?.paint(
            canvas,
            rect,
            shape: _decoration.shape,
            borderRadius: _decoration.borderRadius?.resolve(textDirectionLocal),
            textDirection: configuration.textDirection
        );
    }

    public override string ToString()
    {
        return $"BoxPainter for {_decoration}";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

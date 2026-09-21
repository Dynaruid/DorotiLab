// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_highlight.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Ink_highlightLibrary
{
    internal static Duration _kDefaultHighlightFadeDuration = Duration.Create(milliseconds: 200L);
}

public class InkHighlight : InteractiveInkFeature
{
    internal virtual BoxShape _shape { get; private set; } = default!;
    internal virtual double? _radius { get; private set; }
    internal virtual BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual Func<Rect>? _rectCallback { get; private set; }
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual Animation<long> _alpha { get; set; } = default!;
    internal virtual AnimationController _alphaController { get; set; } = default!;
    internal virtual bool _active { get; set; } = true;

    public InkHighlight(
        MaterialInkController controller,
        RenderBox referenceBox,
        Color color,
        TextDirection textDirection,
        BoxShape shape = BoxShape.rectangle,
        double? radius = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        Func<Rect>? rectCallback = null,
        Action? onRemoved = null,
        Duration? fadeDuration = null
    )
        : base(
            controller: controller,
            referenceBox: referenceBox,
            color: color,
            customBorder: customBorder,
            onRemoved: onRemoved
        )
    {
        Duration __fadeDuration =
            fadeDuration ?? Ink_highlightLibrary._kDefaultHighlightFadeDuration;
        _shape = shape;
        _radius = radius;
        _borderRadius = borderRadius ?? BorderRadius.zero;
        _textDirection = textDirection;
        _rectCallback = rectCallback;
        _alphaController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        duration: DartRuntimePrimitives.RequireValue(__fadeDuration),
                        vsync: this.controller.vsync
                    );
                    __cascade.addListener(this.controller.markNeedsPaint);
                    __cascade.addStatusListener(_handleAlphaStatusChanged);
                    __cascade.forward();
                    return __cascade;
                }
            )
        )();
        _alpha = _alphaController.drive(new IntTween(begin: 0L, end: this.color.alpha));
        this.controller.addInkFeature(this);
    }

    public virtual bool active => _active;

    public virtual void activate()
    {
        _active = true;
        _alphaController.forward();
    }

    public virtual void deactivate()
    {
        _active = false;
        _alphaController.reverse();
    }

    internal virtual void _handleAlphaStatusChanged(AnimationStatus status)
    {
        if (AnimationStatusMembers.isDismissed(status) && !_active)
        {
            dispose();
        }
    }

    public override void dispose()
    {
        _alphaController.dispose();
        base.dispose();
    }

    internal virtual void _paintHighlight(Canvas canvas, Rect rect, Paint paint)
    {
        canvas.save();
        if (customBorder is not null)
        {
            canvas.clipPath(customBorder!.getOuterPath(rect, textDirection: _textDirection));
        }
        switch (_shape)
        {
            case BoxShape.circle:
            {
                canvas.drawCircle(rect.center, _radius ?? Material.defaultSplashRadius, paint);
                break;
            }
            case BoxShape.rectangle:
            {
                if (!Equals(_borderRadius, BorderRadius.zero))
                {
                    var clipRRect = RRect.fromRectAndCorners(
                        rect,
                        topLeft: _borderRadius.topLeft,
                        topRight: _borderRadius.topRight,
                        bottomLeft: _borderRadius.bottomLeft,
                        bottomRight: _borderRadius.bottomRight
                    );
                    canvas.drawRRect(clipRRect, paint);
                }
                else
                {
                    canvas.drawRect(rect, paint);
                }
                break;
            }
        }
        canvas.restore();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color.withAlpha(_alpha.value);
                    return __cascade;
                }
            )
        )();
        Offset? originOffset = MatrixUtils.getAsTranslation(transform);
        Rect rect =
            (_rectCallback is not null) ? _rectCallback() : (Offset.zero & referenceBox.size);
        if (originOffset is null)
        {
            canvas.save();
            canvas.transform(transform.storage);
            _paintHighlight(canvas, rect, paint);
            canvas.restore();
        }
        else
        {
            _paintHighlight(
                canvas,
                rect.shift(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(originOffset)
                    )
                ),
                paint
            );
        }
    }
}

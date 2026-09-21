// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_ripple.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Ink_rippleLibrary
{
    internal static Duration _kUnconfirmedRippleDuration = Duration.Create(seconds: 1L);
}

public static partial class Ink_rippleLibrary
{
    internal static Duration _kFadeInDuration = Duration.Create(milliseconds: 75L);
}

public static partial class Ink_rippleLibrary
{
    internal static Duration _kRadiusDuration = Duration.Create(milliseconds: 225L);
}

public static partial class Ink_rippleLibrary
{
    internal static Duration _kFadeOutDuration = Duration.Create(milliseconds: 375L);
}

public static partial class Ink_rippleLibrary
{
    internal static Duration _kCancelDuration = Duration.Create(milliseconds: 75L);
}

public static partial class Ink_rippleLibrary
{
    internal static double _kFadeOutIntervalStart = 225.0 / 375.0;
}

public static partial class Ink_rippleLibrary
{
    internal static Func<Rect>? _getClipCallback(
        RenderBox referenceBox,
        bool containedInkWell,
        Func<Rect>? rectCallback
    )
    {
        if (rectCallback is not null)
        {
            DartRuntimePrimitives.Assert(() => containedInkWell);
            return rectCallback;
        }
        if (containedInkWell)
        {
            return () => Offset.zero & referenceBox.size;
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Ink_rippleLibrary
{
    internal static double _getTargetRadius(
        RenderBox referenceBox,
        bool containedInkWell,
        Func<Rect>? rectCallback,
        Offset position
    )
    {
        Size sizeLocal = (rectCallback is not null) ? rectCallback().size : referenceBox.size;
        double d1 = sizeLocal.bottomRight(Offset.zero).distance;
        double d2 = (sizeLocal.topRight(Offset.zero) - sizeLocal.bottomLeft(Offset.zero)).distance;
        return Math.Max(d1, d2) / 2.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _InkRippleFactory__ink_ripple : InteractiveInkFeatureFactory
{
    internal _InkRippleFactory__ink_ripple() { }

    public virtual InteractiveInkFeature create(
        MaterialInkController controller,
        RenderBox referenceBox,
        Offset position,
        Color color,
        TextDirection textDirection,
        bool containedInkWell = false,
        Func<Rect>? rectCallback = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        double? radius = null,
        Action? onRemoved = null
    )
    {
        return new InkRipple(
            controller: controller,
            referenceBox: referenceBox,
            position: position,
            color: color,
            containedInkWell: containedInkWell,
            rectCallback: rectCallback,
            borderRadius: borderRadius,
            customBorder: customBorder,
            radius: radius,
            onRemoved: onRemoved,
            textDirection: textDirection
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class InkRipple : InteractiveInkFeature
{
    internal virtual Offset _position { get; private set; } = default!;
    internal virtual BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual double _targetRadius { get; private set; } = default!;
    internal virtual Func<Rect>? _clipCallback { get; private set; }
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual Animation<double> _radius { get; set; } = default!;
    internal virtual AnimationController _radiusController { get; set; } = default!;
    internal virtual Animation<long> _fadeIn { get; set; } = default!;
    internal virtual AnimationController _fadeInController { get; set; } = default!;
    internal virtual Animation<long> _fadeOut { get; set; } = default!;
    internal virtual AnimationController _fadeOutController { get; set; } = default!;
    public static InteractiveInkFeatureFactory splashFactory = new _InkRippleFactory__ink_ripple();
    internal static Animatable<double> _easeCurveTween = new CurveTween(curve: Curves.ease);
    internal static Animatable<double> _fadeOutIntervalTween = new CurveTween(
        curve: new Interval(Ink_rippleLibrary._kFadeOutIntervalStart, 1.0)
    );

    public InkRipple(
        MaterialInkController controller,
        RenderBox referenceBox,
        Offset position,
        Color color,
        TextDirection textDirection,
        bool containedInkWell = false,
        Func<Rect>? rectCallback = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        double? radius = null,
        Action? onRemoved = null
    )
        : base(
            referenceBox: referenceBox,
            customBorder: customBorder,
            onRemoved: onRemoved,
            controller: controller,
            color: color
        )
    {
        _position = position;
        _borderRadius = borderRadius ?? BorderRadius.zero;
        _textDirection = textDirection;
        _targetRadius =
            radius
            ?? Ink_rippleLibrary._getTargetRadius(
                referenceBox,
                containedInkWell,
                rectCallback,
                position
            );
        _clipCallback = Ink_rippleLibrary._getClipCallback(
            referenceBox,
            containedInkWell,
            rectCallback
        );
        _fadeInController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        duration: Ink_rippleLibrary._kFadeInDuration,
                        vsync: controller.vsync
                    );
                    __cascade.addListener(controller.markNeedsPaint);
                    __cascade.forward();
                    return __cascade;
                }
            )
        )();
        _fadeIn = _fadeInController.drive(new IntTween(begin: 0L, end: color.alpha));
        _radiusController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        duration: Ink_rippleLibrary._kUnconfirmedRippleDuration,
                        vsync: controller.vsync
                    );
                    __cascade.addListener(controller.markNeedsPaint);
                    __cascade.forward();
                    return __cascade;
                }
            )
        )();
        _radius = _radiusController.drive(
            new Tween<double>(begin: _targetRadius * 0.3, end: _targetRadius + 5.0).chain(
                _easeCurveTween
            )
        );
        _fadeOutController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        duration: Ink_rippleLibrary._kFadeOutDuration,
                        vsync: controller.vsync
                    );
                    __cascade.addListener(controller.markNeedsPaint);
                    __cascade.addStatusListener(_handleAlphaStatusChanged);
                    return __cascade;
                }
            )
        )();
        _fadeOut = _fadeOutController.drive(
            new IntTween(begin: color.alpha, end: 0L).chain(_fadeOutIntervalTween)
        );
        controller.addInkFeature(this);
    }

    public override void confirm()
    {
        DartRuntimePrimitives.Ignore(
            (
                (Func<AnimationController>)(
                    () =>
                    {
                        var __cascade = _radiusController;
                        __cascade.duration = Ink_rippleLibrary._kRadiusDuration;
                        __cascade.forward();
                        return __cascade;
                    }
                )
            )()
        );
        _fadeInController.forward();
        _fadeOutController.animateTo(1.0, duration: Ink_rippleLibrary._kFadeOutDuration);
    }

    public override void cancel()
    {
        _fadeInController.stop();
        double fadeOutValue = 1.0 - _fadeInController.value;
        _fadeOutController.value = fadeOutValue;
        if (fadeOutValue < 1.0)
        {
            _fadeOutController.animateTo(1.0, duration: Ink_rippleLibrary._kCancelDuration);
        }
    }

    internal virtual void _handleAlphaStatusChanged(AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            dispose();
        }
    }

    public override void dispose()
    {
        _radiusController.dispose();
        _fadeInController.dispose();
        _fadeOutController.dispose();
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        long alpha = _fadeInController.isAnimating ? _fadeIn.value : _fadeOut.value;
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = color.withAlpha(alpha);
                    return __cascade;
                }
            )
        )();
        Rect? rect = _clipCallback?.Invoke();
        Offset centerLocal = (
            Dart_uiLibrary.Offset.lerp(
                _position,
                (rect is not null)
                    ? (
                        rect
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ).center
                    : referenceBox.size.center(Offset.zero),
                Curves.ease.transform(_radiusController.value)
            ) ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        paintInkCircle(
            canvas: canvas,
            transform: transform,
            paint: paintLocal,
            center: centerLocal,
            textDirection: _textDirection,
            radius: _radius.value,
            customBorder: customBorder,
            borderRadius: _borderRadius,
            clipCallback: _clipCallback
        );
    }
}

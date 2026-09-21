// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_sparkle.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class InkSparkle : InteractiveInkFeature
{
    internal static Duration _animationDuration = Duration.Create(milliseconds: 617L);
    internal const double _targetRadiusMultiplier = 2.3;
    internal static double _rotateRight = Dart_mathLibrary.pi * 0.0078125;
    internal static double _rotateLeft = -_rotateRight;
    internal const double _noiseDensity = 2.1;
    internal virtual AnimationController _animationController { get; set; } = default!;
    internal virtual Animation<System.Numerics.Vector2> _center { get; set; } = default!;
    internal virtual Animation<double> _radiusScale { get; set; } = default!;
    internal virtual Animation<double> _alpha { get; set; } = default!;
    internal virtual Animation<double> _sparkleAlpha { get; set; } = default!;
    internal virtual double _turbulenceSeed { get; set; } = default!;

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual Color _color { get; private set; } = default!;
    internal virtual Offset _position { get; private set; } = default!;
    internal virtual BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual double _targetRadius { get; private set; } = default!;
    internal virtual Func<Rect>? _clipCallback { get; private set; }
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual FragmentShader _fragmentShader { get; private set; } = default!;
    internal virtual bool _fragmentShaderInitialized { get; set; } = false;
    public static InteractiveInkFeatureFactory splashFactory =
        new _InkSparkleFactory__ink_sparkle();
    public static InteractiveInkFeatureFactory constantTurbulenceSeedSplashFactory =
        _InkSparkleFactory__ink_sparkle.CreateConstantTurbulenceSeed();

    public InkSparkle(
        MaterialInkController controller,
        RenderBox referenceBox,
        Color color,
        Offset position,
        TextDirection textDirection,
        bool containedInkWell = true,
        Func<Rect>? rectCallback = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        double? radius = null,
        Action? onRemoved = null,
        double? turbulenceSeed = null
    )
        : base(
            controller: controller,
            referenceBox: referenceBox,
            color: color,
            customBorder: customBorder,
            onRemoved: onRemoved
        )
    {
        _color = color;
        _position = position;
        _borderRadius = borderRadius ?? BorderRadius.zero;
        _textDirection = (textDirection);
        _targetRadius =
            (
                radius
                ?? Ink_sparkleLibrary._getTargetRadius(
                    referenceBox,
                    containedInkWell,
                    rectCallback,
                    position
                )
            ) * _targetRadiusMultiplier;
        _clipCallback = Ink_sparkleLibrary._getClipCallback(
            referenceBox,
            containedInkWell,
            rectCallback
        );
        System.Diagnostics.Debug.Assert(containedInkWell || (rectCallback is null));
        _InkSparkleFactory__ink_sparkle.initializeShader(() => this.controller.markNeedsPaint());
        this.controller.addInkFeature(this);
        _animationController = (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(
                        duration: _animationDuration,
                        vsync: this.controller.vsync
                    );
                    __cascade.addListener(this.controller.markNeedsPaint);
                    __cascade.addStatusListener(_handleStatusChanged);
                    __cascade.forward();
                    return __cascade;
                }
            )
        )();
        _radiusScale = new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new CurveTween(curve: Curves.fastOutSlowIn),
                    weight: 75
                ),
                new TweenSequenceItem<double>(tween: new ConstantTween<double>(1.0), weight: 25),
            }
        ).animate(_animationController);
        var centerTween = new Tween<System.Numerics.Vector2>(
            begin: new System.Numerics.Vector2(
                checked((float)_position.dx),
                checked((float)_position.dy)
            ),
            end: new System.Numerics.Vector2(
                checked((float)(this.referenceBox.size.width / 2L)),
                checked((float)(this.referenceBox.size.height / 2L))
            )
        );
        Animation<double> centerProgress = new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: 0.0, end: 1.0),
                    weight: 50
                ),
                new TweenSequenceItem<double>(tween: new ConstantTween<double>(1.0), weight: 50),
            }
        ).animate(_radiusScale);
        _center = centerTween.animate(centerProgress);
        _alpha = new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: 0.0, end: 1.0),
                    weight: 13
                ),
                new TweenSequenceItem<double>(tween: new ConstantTween<double>(1.0), weight: 27),
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: 1.0, end: 0.0),
                    weight: 60
                ),
            }
        ).animate(_animationController);
        _sparkleAlpha = new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: 0.0, end: 1.0),
                    weight: 13
                ),
                new TweenSequenceItem<double>(tween: new ConstantTween<double>(1.0), weight: 27),
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: 1.0, end: 0.0),
                    weight: 50
                ),
            }
        ).animate(_animationController);
        DartRuntimePrimitives.Assert(() =>
        {
            turbulenceSeed ??= _InkSparkleFactory__ink_sparkle.constantSeed;
            return true;
        });
        _turbulenceSeed = turbulenceSeed ?? (new DartRandom().nextDouble() * 1000.0);
    }

    internal virtual void _handleStatusChanged(AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            dispose();
        }
    }

    public override void dispose()
    {
        _animationController.stop();
        _animationController.dispose();
        if (_fragmentShaderInitialized)
        {
            _fragmentShader.dispose();
        }
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => _animationController.isAnimating);
        if (_InkSparkleFactory__ink_sparkle._program is null)
        {
            return;
        }
        if (!_fragmentShaderInitialized)
        {
            _fragmentShader = _InkSparkleFactory__ink_sparkle._program!.fragmentShader();
            _fragmentShaderInitialized = true;
        }
        canvas.save();
        _transformCanvas(canvas: canvas, transform: transform);
        if (_clipCallback is not null)
        {
            _clipCanvas(
                canvas: canvas,
                clipCallback: _clipCallback,
                textDirection: _textDirection,
                customBorder: customBorder,
                borderRadius: _borderRadius
            );
        }
        _updateFragmentShader();
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.shader = _fragmentShader;
                    return __cascade;
                }
            )
        )();
        if (_clipCallback is not null)
        {
            canvas.drawRect(_clipCallback(), paint);
        }
        else
        {
            canvas.drawPaint(paint);
        }
        canvas.restore();
    }

    internal virtual double _width => referenceBox.size.width;
    internal virtual double _height => referenceBox.size.height;

    internal virtual void _updateFragmentShader()
    {
        var turbulenceScale = 1.5;
        double turbulencePhase = _turbulenceSeed + _radiusScale.value;
        var noisePhase = turbulencePhase;
        double rotation1 = (turbulencePhase * _rotateRight) + (1.7 * Dart_mathLibrary.pi);
        double rotation2 = (turbulencePhase * _rotateLeft) + (2.0 * Dart_mathLibrary.pi);
        double rotation3 = (turbulencePhase * _rotateRight) + (2.75 * Dart_mathLibrary.pi);
        DartRuntimePrimitives.Ignore(
            (
                (Func<FragmentShader>)(
                    () =>
                    {
                        var __cascade = _fragmentShader;
                        __cascade.setFloat(0L, _color.red / 255.0);
                        __cascade.setFloat(1L, _color.green / 255.0);
                        __cascade.setFloat(2L, _color.blue / 255.0);
                        __cascade.setFloat(3L, _color.alpha / 255.0);
                        __cascade.setFloat(4L, _alpha.value);
                        __cascade.setFloat(5L, _sparkleAlpha.value);
                        __cascade.setFloat(6L, 1.0);
                        __cascade.setFloat(7L, _radiusScale.value);
                        __cascade.setFloat(8L, _center.value.X);
                        __cascade.setFloat(9L, _center.value.Y);
                        __cascade.setFloat(10L, _targetRadius);
                        __cascade.setFloat(11L, 1.0 / _width);
                        __cascade.setFloat(12L, 1.0 / _height);
                        __cascade.setFloat(13L, _noiseDensity / _width);
                        __cascade.setFloat(14L, _noiseDensity / _height);
                        __cascade.setFloat(15L, noisePhase / 1000.0);
                        __cascade.setFloat(
                            16L,
                            (turbulenceScale * 0.5)
                                + (
                                    turbulencePhase
                                    * 0.01
                                    * Dart_mathLibrary.cos(turbulenceScale * 0.55)
                                )
                        );
                        __cascade.setFloat(
                            17L,
                            (turbulenceScale * 0.5)
                                + (
                                    turbulencePhase
                                    * 0.01
                                    * Dart_mathLibrary.sin(turbulenceScale * 0.55)
                                )
                        );
                        __cascade.setFloat(
                            18L,
                            (turbulenceScale * 0.2)
                                + (
                                    turbulencePhase
                                    * -0.0066
                                    * Dart_mathLibrary.cos(turbulenceScale * 0.45)
                                )
                        );
                        __cascade.setFloat(
                            19L,
                            (turbulenceScale * 0.2)
                                + (
                                    turbulencePhase
                                    * -0.0066
                                    * Dart_mathLibrary.sin(turbulenceScale * 0.45)
                                )
                        );
                        __cascade.setFloat(
                            20L,
                            turbulenceScale
                                + (
                                    turbulencePhase
                                    * -0.0066
                                    * Dart_mathLibrary.cos(turbulenceScale * 0.35)
                                )
                        );
                        __cascade.setFloat(
                            21L,
                            turbulenceScale
                                + (
                                    turbulencePhase
                                    * -0.0066
                                    * Dart_mathLibrary.sin(turbulenceScale * 0.35)
                                )
                        );
                        __cascade.setFloat(22L, Dart_mathLibrary.cos(rotation1));
                        __cascade.setFloat(23L, Dart_mathLibrary.sin(rotation1));
                        __cascade.setFloat(24L, Dart_mathLibrary.cos(rotation2));
                        __cascade.setFloat(25L, Dart_mathLibrary.sin(rotation2));
                        __cascade.setFloat(26L, Dart_mathLibrary.cos(rotation3));
                        __cascade.setFloat(27L, Dart_mathLibrary.sin(rotation3));
                        return __cascade;
                    }
                )
            )()
        );
    }

    internal virtual void _transformCanvas(Canvas canvas, Matrix4 transform)
    {
        Offset? originOffset = MatrixUtils.getAsTranslation(transform);
        if (originOffset is null)
        {
            canvas.transform(transform.storage);
        }
        else
        {
            canvas.translate(
                (
                    originOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).dx,
                (
                    originOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).dy
            );
        }
    }

    internal virtual void _clipCanvas(
        Canvas canvas,
        Func<Rect> clipCallback,
        TextDirection? textDirection = null,
        ShapeBorder? customBorder = null,
        BorderRadius borderRadius = default!
    )
    {
        Rect rect = clipCallback();
        if (customBorder is not null)
        {
            canvas.clipPath(customBorder.getOuterPath(rect, textDirection: textDirection));
        }
        else
        {
            if (!Equals(borderRadius, BorderRadius.zero))
            {
                canvas.clipRRect(
                    RRect.fromRectAndCorners(
                        rect,
                        topLeft: borderRadius.topLeft,
                        topRight: borderRadius.topRight,
                        bottomLeft: borderRadius.bottomLeft,
                        bottomRight: borderRadius.bottomRight
                    )
                );
            }
            else
            {
                canvas.clipRect(rect);
            }
        }
    }
}

internal class _InkSparkleFactory__ink_sparkle : InteractiveInkFeatureFactory
{
    public const double constantSeed = 1337.0;
    internal static bool _initCalled = false;
    internal static FragmentProgram? _program = default;
    public virtual double? turbulenceSeed { get; private set; }

    internal _InkSparkleFactory__ink_sparkle()
    {
        turbulenceSeed = null;
    }

    internal static _InkSparkleFactory__ink_sparkle CreateConstantTurbulenceSeed()
    {
        var __instance = new _InkSparkleFactory__ink_sparkle();
        __instance.turbulenceSeed = constantSeed;
        return __instance;
    }

    public static void initializeShader(Action? onReady = null)
    {
        if (_program is not null)
        {
            onReady?.Invoke();
            return;
        }
        _initCalled = true;
        FrameworkShaderLoader.RegisterResourceOwner(typeof(InkSparkle).Assembly);
        FrameworkShaderLoader.BeginLoad(
            "material.ink-sparkle",
            program =>
            {
                _program = program;
                onReady?.Invoke();
            }
        );
    }

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
        return new InkSparkle(
            controller: controller,
            referenceBox: referenceBox,
            position: position,
            color: color,
            textDirection: textDirection,
            containedInkWell: containedInkWell,
            rectCallback: rectCallback,
            borderRadius: borderRadius,
            customBorder: customBorder,
            radius: radius,
            onRemoved: onRemoved,
            turbulenceSeed: turbulenceSeed
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Ink_sparkleLibrary
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Ink_sparkleLibrary
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

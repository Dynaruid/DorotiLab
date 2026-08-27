// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_sparkle.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public class InkSparkle : InteractiveInkFeature
{
    internal static Duration _animationDuration = Duration.Create(milliseconds: 617L);
    internal const double _targetRadiusMultiplier = 2.3;
    internal static double _rotateRight = (Dart_mathLibrary.pi * 0.0078125);
    internal static double _rotateLeft = -_rotateRight;
    internal const double _noiseDensity = 2.1;
    internal virtual global::Doroti.Framework.Animation.AnimationController _animationController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<global::System.Numerics.Vector2> _center { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _radiusScale { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _alpha { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _sparkleAlpha { get; set; } = default!;
    internal virtual double _turbulenceSeed { get; set; } = default!;
    internal virtual Color _color { get; private set; } = default!;
    internal virtual Offset _position { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Painting.BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual double _targetRadius { get; private set; } = default!;
    internal virtual global::System.Func<Rect>? _clipCallback { get; private set; }
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual global::Doroti.Ui.FragmentShader _fragmentShader { get; private set; } = default!;
    internal virtual bool _fragmentShaderInitialized { get; set; } = false;
    public static InteractiveInkFeatureFactory splashFactory = ((InteractiveInkFeatureFactory)(object?)new _InkSparkleFactory__ink_sparkle());
    public static InteractiveInkFeatureFactory constantTurbulenceSeedSplashFactory = ((InteractiveInkFeatureFactory)(object?)_InkSparkleFactory__ink_sparkle.CreateConstantTurbulenceSeed());

    public InkSparkle(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Color color, Offset position, TextDirection textDirection, bool containedInkWell = true, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null, double? turbulenceSeed = null) : base(controller: controller, referenceBox: referenceBox, color: color, customBorder: customBorder, onRemoved: onRemoved)
    {
        this._color = color;
        this._position = position;
        this._borderRadius = (borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero);
        this._textDirection = DartRuntimePrimitives.RequireValue(textDirection);
        this._targetRadius = (((radius ?? Ink_sparkleLibrary._getTargetRadius(referenceBox, containedInkWell, rectCallback, position))) * _targetRadiusMultiplier);
        this._clipCallback = Ink_sparkleLibrary._getClipCallback(referenceBox, containedInkWell, rectCallback);
        System.Diagnostics.Debug.Assert((containedInkWell || (rectCallback is null)));
        _InkSparkleFactory__ink_sparkle.initializeShader(() => this.controller.markNeedsPaint());
        this.controller.addInkFeature(this);
        _animationController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: _animationDuration, vsync: ((MaterialInkController)this.controller).vsync);
    __cascade.addListener(() => ((MaterialInkController)this.controller).markNeedsPaint());
    __cascade.addStatusListener((AnimationStatusListener)this._handleStatusChanged);
    __cascade.forward();
    return __cascade;
}))();
        _radiusScale = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn), weight: 75), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.ConstantTween<double>(1.0), weight: 25) }).animate(this._animationController);
        var centerTween = new global::Doroti.Framework.Animation.Tween<global::System.Numerics.Vector2>(begin: new global::System.Numerics.Vector2(checked((float)this._position.dx), checked((float)this._position.dy)), end: new global::System.Numerics.Vector2(checked((float)(((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size.width / 2L)), checked((float)(((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size.height / 2L))));
        global::Doroti.Framework.Animation.Animation<double> centerProgress = ((global::Doroti.Framework.Animation.Animation<double>)(object?)new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0), weight: 50), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.ConstantTween<double>(1.0), weight: 50) }).animate(this._radiusScale));
        _center = centerTween.animate(centerProgress);
        _alpha = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0), weight: 13), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.ConstantTween<double>(1.0), weight: 27), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0), weight: 60) }).animate(this._animationController);
        _sparkleAlpha = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0), weight: 13), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.ConstantTween<double>(1.0), weight: 27), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0), weight: 50) }).animate(this._animationController);
        DartRuntimePrimitives.Assert(() =>
            {
                turbulenceSeed ??= _InkSparkleFactory__ink_sparkle.constantSeed;
                return true;
            });
        _turbulenceSeed = (turbulenceSeed ?? (new DartRandom().nextDouble() * 1000.0));
    }

    internal virtual void _handleStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            dispose();
        }
    }

    public override void dispose()
    {
        this._animationController.stop();
        this._animationController.dispose();
        if (this._fragmentShaderInitialized)
        {
            this._fragmentShader.dispose();
        }
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Animation.AnimationController)this._animationController).isAnimating);
        if ((_InkSparkleFactory__ink_sparkle._program is null))
        {
            return;
        }
        if (!this._fragmentShaderInitialized)
        {
            _fragmentShader = _InkSparkleFactory__ink_sparkle._program!.fragmentShader();
            _fragmentShaderInitialized = true;
        }
        canvas.save();
        _transformCanvas(canvas: canvas, transform: transform);
        if ((this._clipCallback is not null))
        {
            _clipCanvas(canvas: canvas, clipCallback: (global::System.Func<Rect>)this._clipCallback, textDirection: this._textDirection, customBorder: this.customBorder, borderRadius: this._borderRadius);
        }
        _updateFragmentShader();
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.shader = this._fragmentShader;
    return __cascade;
}))();
        if ((this._clipCallback is not null))
        {
            canvas.drawRect(this._clipCallback(), paint);
        }
        else
        {
            canvas.drawPaint(paint);
        }
        canvas.restore();
    }

    internal virtual double _width => ((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size.width;
    internal virtual double _height => ((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size.height;
    internal virtual void _updateFragmentShader()
    {
        var turbulenceScale = 1.5;
        double turbulencePhase = (this._turbulenceSeed + ((global::Doroti.Framework.Animation.Animation<double>)this._radiusScale).value);
        var noisePhase = turbulencePhase;
        double rotation1 = ((turbulencePhase * _rotateRight) + (1.7 * Dart_mathLibrary.pi));
        double rotation2 = ((turbulencePhase * _rotateLeft) + (2.0 * Dart_mathLibrary.pi));
        double rotation3 = ((turbulencePhase * _rotateRight) + (2.75 * Dart_mathLibrary.pi));
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Ui.FragmentShader>)(() =>
{
    var __cascade = this._fragmentShader;
    __cascade.setFloat(0L, (this._color.red / 255.0));
    __cascade.setFloat(1L, (this._color.green / 255.0));
    __cascade.setFloat(2L, (this._color.blue / 255.0));
    __cascade.setFloat(3L, (this._color.alpha / 255.0));
    __cascade.setFloat(4L, ((global::Doroti.Framework.Animation.Animation<double>)this._alpha).value);
    __cascade.setFloat(5L, ((global::Doroti.Framework.Animation.Animation<double>)this._sparkleAlpha).value);
    __cascade.setFloat(6L, 1.0);
    __cascade.setFloat(7L, ((global::Doroti.Framework.Animation.Animation<double>)this._radiusScale).value);
    __cascade.setFloat(8L, ((global::Doroti.Framework.Animation.Animation<global::System.Numerics.Vector2>)this._center).value.X);
    __cascade.setFloat(9L, ((global::Doroti.Framework.Animation.Animation<global::System.Numerics.Vector2>)this._center).value.Y);
    __cascade.setFloat(10L, this._targetRadius);
    __cascade.setFloat(11L, (1.0 / this._width));
    __cascade.setFloat(12L, (1.0 / this._height));
    __cascade.setFloat(13L, (_noiseDensity / this._width));
    __cascade.setFloat(14L, (_noiseDensity / this._height));
    __cascade.setFloat(15L, (noisePhase / 1000.0));
    __cascade.setFloat(16L, ((turbulenceScale * 0.5) + (((turbulencePhase * 0.01) * global::Doroti.Runtime.Dart_mathLibrary.cos((turbulenceScale * 0.55))))));
    __cascade.setFloat(17L, ((turbulenceScale * 0.5) + (((turbulencePhase * 0.01) * global::Doroti.Runtime.Dart_mathLibrary.sin((turbulenceScale * 0.55))))));
    __cascade.setFloat(18L, ((turbulenceScale * 0.2) + (((turbulencePhase * -0.0066) * global::Doroti.Runtime.Dart_mathLibrary.cos((turbulenceScale * 0.45))))));
    __cascade.setFloat(19L, ((turbulenceScale * 0.2) + (((turbulencePhase * -0.0066) * global::Doroti.Runtime.Dart_mathLibrary.sin((turbulenceScale * 0.45))))));
    __cascade.setFloat(20L, (turbulenceScale + (((turbulencePhase * -0.0066) * global::Doroti.Runtime.Dart_mathLibrary.cos((turbulenceScale * 0.35))))));
    __cascade.setFloat(21L, (turbulenceScale + (((turbulencePhase * -0.0066) * global::Doroti.Runtime.Dart_mathLibrary.sin((turbulenceScale * 0.35))))));
    __cascade.setFloat(22L, global::Doroti.Runtime.Dart_mathLibrary.cos(rotation1));
    __cascade.setFloat(23L, global::Doroti.Runtime.Dart_mathLibrary.sin(rotation1));
    __cascade.setFloat(24L, global::Doroti.Runtime.Dart_mathLibrary.cos(rotation2));
    __cascade.setFloat(25L, global::Doroti.Runtime.Dart_mathLibrary.sin(rotation2));
    __cascade.setFloat(26L, global::Doroti.Runtime.Dart_mathLibrary.cos(rotation3));
    __cascade.setFloat(27L, global::Doroti.Runtime.Dart_mathLibrary.sin(rotation3));
    return __cascade;
}))());
    }

    internal virtual void _transformCanvas(Canvas canvas, Matrix4 transform)
    {
        global::Doroti.Ui.Offset? originOffset = ((global::Doroti.Ui.Offset?)(object?)MatrixUtils.getAsTranslation(transform));
        if ((originOffset is null))
        {
            canvas.transform(transform.storage);
        }
        else
        {
            canvas.translate(DartRuntimePrimitives.RequireValue(originOffset).dx, DartRuntimePrimitives.RequireValue(originOffset).dy);
        }
    }

    internal virtual void _clipCanvas(Canvas canvas, global::System.Func<Rect> clipCallback, TextDirection? textDirection = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!)
    {
        global::Doroti.Ui.Rect rect = ((global::Doroti.Ui.Rect)(object?)clipCallback());
        if ((customBorder is not null))
        {
            canvas.clipPath(customBorder.getOuterPath(rect, textDirection: textDirection));
        }
        else
        {
            if ((!object.Equals(borderRadius, global::Doroti.Framework.Painting.BorderRadius.zero)))
            {
                canvas.clipRRect(global::Doroti.Ui.RRect.fromRectAndCorners(rect, topLeft: ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).topLeft, topRight: ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).topRight, bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).bottomLeft, bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).bottomRight));
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
    internal static global::Doroti.Ui.FragmentProgram? _program = default;
    public virtual double? turbulenceSeed { get; private set; }

    internal _InkSparkleFactory__ink_sparkle()
    {
        this.turbulenceSeed = null;
    }

    internal static _InkSparkleFactory__ink_sparkle CreateConstantTurbulenceSeed()
    {
        var __instance = new _InkSparkleFactory__ink_sparkle();
        __instance.turbulenceSeed = _InkSparkleFactory__ink_sparkle.constantSeed;
        return __instance;
    }

    public static void initializeShader(global::System.Action? onReady = null)
    {
        if (_program is not null)
        {
            onReady?.Invoke();
            return;
        }
        _initCalled = true;
        global::Doroti.Ui.FrameworkShaderLoader.BeginLoad(
            "material.ink-sparkle",
            program =>
            {
                _program = program;
                onReady?.Invoke();
            });
    }

    public virtual InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null)
    {
        return ((InteractiveInkFeature)(object?)new InkSparkle(controller: controller, referenceBox: referenceBox, position: position, color: color, textDirection: textDirection, containedInkWell: containedInkWell, rectCallback: (global::System.Func<Rect>?)rectCallback, borderRadius: borderRadius, customBorder: customBorder, radius: radius, onRemoved: () => onRemoved(), turbulenceSeed: this.turbulenceSeed));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Ink_sparkleLibrary
{
    internal static global::System.Func<Rect>? _getClipCallback(global::Doroti.Framework.Rendering.RenderBox referenceBox, bool containedInkWell, global::System.Func<Rect>? rectCallback)
    {
        if ((rectCallback is not null))
        {
            DartRuntimePrimitives.Assert(() => containedInkWell);
            return ((global::System.Func<Rect>)rectCallback);
        }
        if (containedInkWell)
        {
            return ((global::System.Func<Rect>)(() => (Offset.zero & ((global::Doroti.Framework.Rendering.RenderBox)referenceBox).size)));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Ink_sparkleLibrary
{
    internal static double _getTargetRadius(global::Doroti.Framework.Rendering.RenderBox referenceBox, bool containedInkWell, global::System.Func<Rect>? rectCallback, Offset position)
    {
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)((rectCallback is not null) ? rectCallback().size : ((global::Doroti.Framework.Rendering.RenderBox)referenceBox).size));
        double d1 = sizeLocal.bottomRight(Offset.zero).distance;
        double d2 = ((sizeLocal.topRight(Offset.zero) - sizeLocal.bottomLeft(Offset.zero))).distance;
        return (Math.Max(d1, d2) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

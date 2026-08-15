// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/ink_ripple.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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
    internal static double _kFadeOutIntervalStart = (225.0 / 375.0);
}

public static partial class Ink_rippleLibrary
{
    internal static global::System.Func<Rect>? _getClipCallback(global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, bool containedInkWell, global::System.Func<Rect>? rectCallback)
    {
        if ((rectCallback is not null))
        {
            DartRuntimePrimitives.Assert(() => containedInkWell);
            return ((global::System.Func<Rect>)rectCallback);
        }
        if (containedInkWell)
        {
            return ((global::System.Func<Rect>)(() => (Offset.zero & ((global::Doroti.Generated.Framework.Rendering.RenderBox)referenceBox).size)));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Ink_rippleLibrary
{
    internal static double _getTargetRadius(global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, bool containedInkWell, global::System.Func<Rect>? rectCallback, Offset position)
    {
        global::Doroti.Flutter.Ui.Size size__1363 = ((global::Doroti.Flutter.Ui.Size)(object?)((rectCallback is not null) ? rectCallback().size : ((global::Doroti.Generated.Framework.Rendering.RenderBox)referenceBox).size));
        double d1__1449 = size__1363.bottomRight(Offset.zero).distance;
        double d2__1509 = ((size__1363.topRight(Offset.zero) - size__1363.bottomLeft(Offset.zero))).distance;
        return (Math.Max(d1__1449, d2__1509) / 2.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _InkRippleFactory__ink_ripple : InteractiveInkFeatureFactory
{
    internal _InkRippleFactory__ink_ripple()
    {
    }

    public virtual InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null)
    {
        return ((InteractiveInkFeature)(object?)new InkRipple(controller: controller, referenceBox: referenceBox, position: position, color: color, containedInkWell: containedInkWell, rectCallback: (global::System.Func<Rect>?)rectCallback, borderRadius: borderRadius, customBorder: customBorder, radius: radius, onRemoved: () => onRemoved(), textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InkRipple : InteractiveInkFeature
{
    internal virtual Offset _position { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual double _targetRadius { get; private set; } = default!;
    internal virtual global::System.Func<Rect>? _clipCallback { get; private set; }
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _radius { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _radiusController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<long> _fadeIn { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _fadeInController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<long> _fadeOut { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _fadeOutController { get; set; } = default!;
    public static InteractiveInkFeatureFactory splashFactory = ((InteractiveInkFeatureFactory)(object?)new _InkRippleFactory__ink_ripple());
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _easeCurveTween = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.ease));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _fadeOutIntervalTween = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(Ink_rippleLibrary._kFadeOutIntervalStart, 1.0)));

    public InkRipple(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null) : base(referenceBox: referenceBox, customBorder: customBorder, onRemoved: onRemoved, controller: controller, color: color)
    {
        this._position = position;
        this._borderRadius = (borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.zero);
        this._textDirection = textDirection;
        this._targetRadius = (radius ?? Ink_rippleLibrary._getTargetRadius(referenceBox, containedInkWell, rectCallback, position));
        this._clipCallback = Ink_rippleLibrary._getClipCallback(referenceBox, containedInkWell, rectCallback);
        _fadeInController = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: Ink_rippleLibrary._kFadeInDuration, vsync: ((MaterialInkController)controller).vsync);
            __cascade.addListener(() => ((MaterialInkController)controller).markNeedsPaint());
            __cascade.forward();
            return __cascade;        }))();
        _fadeIn = this._fadeInController.drive(new global::Doroti.Generated.Framework.Animation.IntTween(begin: 0L, end: color.alpha));
        _radiusController = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: Ink_rippleLibrary._kUnconfirmedRippleDuration, vsync: ((MaterialInkController)controller).vsync);
            __cascade.addListener(() => ((MaterialInkController)controller).markNeedsPaint());
            __cascade.forward();
            return __cascade;        }))();
        _radius = this._radiusController.drive(new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: (this._targetRadius * 0.3), end: (this._targetRadius + 5.0)).chain(_easeCurveTween));
        _fadeOutController = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(duration: Ink_rippleLibrary._kFadeOutDuration, vsync: ((MaterialInkController)controller).vsync);
            __cascade.addListener(() => ((MaterialInkController)controller).markNeedsPaint());
            __cascade.addStatusListener((AnimationStatusListener)this._handleAlphaStatusChanged);
            return __cascade;        }))();
        _fadeOut = this._fadeOutController.drive(new global::Doroti.Generated.Framework.Animation.IntTween(begin: color.alpha, end: 0L).chain(_fadeOutIntervalTween));
        controller.addInkFeature(this);
    }

    public override void confirm()
    {
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._radiusController;
            __cascade.duration = Ink_rippleLibrary._kRadiusDuration;
            __cascade.forward();
            return __cascade;        }))());
        this._fadeInController.forward();
        this._fadeOutController.animateTo(1.0, duration: Ink_rippleLibrary._kFadeOutDuration);
    }

    public override void cancel()
    {
        this._fadeInController.stop();
        double fadeOutValue__7753 = (1.0 - ((global::Doroti.Generated.Framework.Animation.AnimationController)this._fadeInController).value);
        this._fadeOutController.value = fadeOutValue__7753;
        if ((fadeOutValue__7753 < 1.0))
        {
            this._fadeOutController.animateTo(1.0, duration: Ink_rippleLibrary._kCancelDuration);
        }
    }

    internal virtual void _handleAlphaStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            dispose();
        }
    }

    public override void dispose()
    {
        this._radiusController.dispose();
        this._fadeInController.dispose();
        this._fadeOutController.dispose();
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        long alpha__8310 = (((global::Doroti.Generated.Framework.Animation.AnimationController)this._fadeInController).isAnimating ? ((global::Doroti.Generated.Framework.Animation.Animation<long>)this._fadeIn).value : ((global::Doroti.Generated.Framework.Animation.Animation<long>)this._fadeOut).value);
        var paint__8392 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.color = this.color.withAlpha(alpha__8310);
            return __cascade;        }))();
        global::Doroti.Flutter.Ui.Rect? rect__8457 = ((global::Doroti.Flutter.Ui.Rect?)(object?)this._clipCallback?.Invoke());
        global::Doroti.Flutter.Ui.Offset center__8560 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(this._position, ((rect__8457 is not null) ? ((Offset)((dynamic)DartRuntimePrimitives.RequireValue(rect__8457)).center) : ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.referenceBox).size.center(Offset.zero)), global::Doroti.Generated.Framework.Animation.Curves.ease.transform(((global::Doroti.Generated.Framework.Animation.AnimationController)this._radiusController).value))));
        paintInkCircle(canvas: canvas, transform: transform, paint: paint__8392, center: center__8560, textDirection: this._textDirection, radius: ((global::Doroti.Generated.Framework.Animation.Animation<double>)this._radius).value, customBorder: this.customBorder, borderRadius: this._borderRadius, clipCallback: (global::System.Func<Rect>?)this._clipCallback);
    }

}

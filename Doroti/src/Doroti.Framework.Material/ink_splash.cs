// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_splash.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Ink_splashLibrary
{
    internal static Duration _kUnconfirmedSplashDuration = Duration.Create(seconds: 1L);
}

public static partial class Ink_splashLibrary
{
    internal static Duration _kSplashFadeDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Ink_splashLibrary
{
    internal static double _kSplashInitialSize = 0.0;
}

public static partial class Ink_splashLibrary
{
    internal static double _kSplashConfirmedVelocity = 1.0;
}

public static partial class Ink_splashLibrary
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

public static partial class Ink_splashLibrary
{
    internal static double _getTargetRadius(global::Doroti.Framework.Rendering.RenderBox referenceBox, bool containedInkWell, global::System.Func<Rect>? rectCallback, Offset position)
    {
        if (containedInkWell)
        {
            global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)((rectCallback is not null) ? rectCallback().size : ((global::Doroti.Framework.Rendering.RenderBox)referenceBox).size));
            return _getSplashRadiusForPositionInSize(sizeLocal, position);
        }
        return Material.defaultSplashRadius;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Ink_splashLibrary
{
    internal static double _getSplashRadiusForPositionInSize(Size bounds, Offset position)
    {
        double d1 = ((position - bounds.topLeft(Offset.zero))).distance;
        double d2 = ((position - bounds.topRight(Offset.zero))).distance;
        double d3 = ((position - bounds.bottomLeft(Offset.zero))).distance;
        double d4 = ((position - bounds.bottomRight(Offset.zero))).distance;
        return Math.Max(Math.Max(d1, d2), Math.Max(d3, d4)).ceilToDouble();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _InkSplashFactory__ink_splash : InteractiveInkFeatureFactory
{
    internal _InkSplashFactory__ink_splash()
    {
    }

    public virtual InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null)
    {
        return ((InteractiveInkFeature)new InkSplash(controller: controller, referenceBox: referenceBox, position: position, color: color, containedInkWell: containedInkWell, rectCallback: (global::System.Func<Rect>?)rectCallback, borderRadius: borderRadius, customBorder: customBorder, radius: radius, onRemoved: onRemoved, textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class InkSplash : InteractiveInkFeature
{
    internal virtual Offset? _position { get; private set; }
    internal virtual global::Doroti.Framework.Painting.BorderRadius _borderRadius { get; private set; } = default!;
    internal virtual double _targetRadius { get; private set; } = default!;
    internal virtual global::System.Func<Rect>? _clipCallback { get; private set; }
    internal virtual bool _repositionToReferenceBox { get; private set; } = default!;
    internal virtual TextDirection _textDirection { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _radius { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _radiusController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<long> _alpha { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController? _alphaController { get; set; } = default;
    public static InteractiveInkFeatureFactory splashFactory = ((InteractiveInkFeatureFactory)new _InkSplashFactory__ink_splash());

    public InkSplash(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, TextDirection textDirection, Offset? position = null, Color color = default!, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null) : base(referenceBox: referenceBox, customBorder: customBorder, onRemoved: onRemoved, controller: controller, color: color)
    {
        this._position = position;
        this._borderRadius = (borderRadius ?? BorderRadius.zero);
        this._targetRadius = (radius ?? Ink_splashLibrary._getTargetRadius(referenceBox, containedInkWell, rectCallback, DartRuntimePrimitives.RequireValue(position)));
        this._clipCallback = Ink_splashLibrary._getClipCallback(referenceBox, containedInkWell, rectCallback);
        this._repositionToReferenceBox = !containedInkWell;
        this._textDirection = textDirection;
        _radiusController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Ink_splashLibrary._kUnconfirmedSplashDuration, vsync: ((MaterialInkController)controller).vsync);
    __cascade.addListener(((MaterialInkController)controller).markNeedsPaint);
    __cascade.forward();
    return __cascade;
}))();
        _radius = this._radiusController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: Ink_splashLibrary._kSplashInitialSize, end: this._targetRadius));
        _alphaController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Ink_splashLibrary._kSplashFadeDuration, vsync: ((MaterialInkController)controller).vsync);
    __cascade.addListener(((MaterialInkController)controller).markNeedsPaint);
    __cascade.addStatusListener((AnimationStatusListener)this._handleAlphaStatusChanged);
    return __cascade;
}))();
        _alpha = this._alphaController!.drive(new global::Doroti.Framework.Animation.IntTween(begin: color.alpha, end: 0L));
        controller.addInkFeature(this);
    }

    public override void confirm()
    {
        long durationLocal = ((this._targetRadius / Ink_splashLibrary._kSplashConfirmedVelocity)).floor();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._radiusController;
    __cascade.duration = Duration.Create(milliseconds: durationLocal);
    __cascade.forward();
    return __cascade;
}))());
        this._alphaController!.forward();
    }

    public override void cancel()
    {
        this._alphaController?.forward();
    }

    internal virtual void _handleAlphaStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            dispose();
        }
    }

    public override void dispose()
    {
        this._radiusController.dispose();
        this._alphaController!.dispose();
        _alphaController = null;
        base.dispose();
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color.withAlpha(((global::Doroti.Framework.Animation.Animation<long>)this._alpha).value);
    return __cascade;
}))();
        global::Doroti.Ui.Offset? centerLocal = ((global::Doroti.Ui.Offset?)this._position);
        if (this._repositionToReferenceBox)
        {
            centerLocal = Dart_uiLibrary.Offset.lerp(centerLocal, ((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size.center(Offset.zero), ((global::Doroti.Framework.Animation.AnimationController)this._radiusController).value);
        }
        paintInkCircle(canvas: canvas, transform: transform, paint: paintLocal, center: DartRuntimePrimitives.RequireValue(centerLocal), textDirection: this._textDirection, radius: ((global::Doroti.Framework.Animation.Animation<double>)this._radius).value, customBorder: this.customBorder, borderRadius: this._borderRadius, clipCallback: (global::System.Func<Rect>?)this._clipCallback);
    }

}

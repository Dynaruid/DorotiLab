// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/ink_splash.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
            global::Doroti.Ui.Size size__1215 = ((global::Doroti.Ui.Size)(object?)((rectCallback is not null) ? rectCallback().size : ((global::Doroti.Framework.Rendering.RenderBox)referenceBox).size));
            return Ink_splashLibrary._getSplashRadiusForPositionInSize(size__1215, position);
        }
        return Material.defaultSplashRadius;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Ink_splashLibrary
{
    internal static double _getSplashRadiusForPositionInSize(Size bounds, Offset position)
    {
        double d1__1482 = ((position - bounds.topLeft(Offset.zero))).distance;
        double d2__1553 = ((position - bounds.topRight(Offset.zero))).distance;
        double d3__1625 = ((position - bounds.bottomLeft(Offset.zero))).distance;
        double d4__1699 = ((position - bounds.bottomRight(Offset.zero))).distance;
        return Math.Max(Math.Max(d1__1482, d2__1553), Math.Max(d3__1625, d4__1699)).ceilToDouble();
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
        return ((InteractiveInkFeature)(object?)new InkSplash(controller: controller, referenceBox: referenceBox, position: position, color: color, containedInkWell: containedInkWell, rectCallback: (global::System.Func<Rect>?)rectCallback, borderRadius: borderRadius, customBorder: customBorder, radius: radius, onRemoved: () => onRemoved(), textDirection: textDirection));
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
    public static InteractiveInkFeatureFactory splashFactory = ((InteractiveInkFeatureFactory)(object?)new _InkSplashFactory__ink_splash());

    public InkSplash(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, TextDirection textDirection, Offset? position = null, Color color = default!, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null) : base(referenceBox: referenceBox, customBorder: customBorder, onRemoved: onRemoved, controller: controller, color: color)
    {
        this._position = position;
        this._borderRadius = (borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.zero);
        this._targetRadius = (radius ?? Ink_splashLibrary._getTargetRadius(referenceBox, containedInkWell, rectCallback, DartRuntimePrimitives.RequireValue(position)));
        this._clipCallback = Ink_splashLibrary._getClipCallback(referenceBox, containedInkWell, rectCallback);
        this._repositionToReferenceBox = !containedInkWell;
        this._textDirection = textDirection;
        _radiusController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Ink_splashLibrary._kUnconfirmedSplashDuration, vsync: ((MaterialInkController)controller).vsync);
            __cascade.addListener(() => ((MaterialInkController)controller).markNeedsPaint());
            __cascade.forward();
            return __cascade;        }))();
        _radius = this._radiusController.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: Ink_splashLibrary._kSplashInitialSize, end: this._targetRadius));
        _alphaController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: Ink_splashLibrary._kSplashFadeDuration, vsync: ((MaterialInkController)controller).vsync);
            __cascade.addListener(() => ((MaterialInkController)controller).markNeedsPaint());
            __cascade.addStatusListener((AnimationStatusListener)this._handleAlphaStatusChanged);
            return __cascade;        }))();
        _alpha = this._alphaController!.drive(new global::Doroti.Framework.Animation.IntTween(begin: color.alpha, end: 0L));
        controller.addInkFeature(this);
    }

    public override void confirm()
    {
        long duration__6493 = ((this._targetRadius / Ink_splashLibrary._kSplashConfirmedVelocity)).floor();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{            var __cascade = this._radiusController;
            __cascade.duration = Duration.Create(milliseconds: duration__6493);
            __cascade.forward();
            return __cascade;        }))());
        this._alphaController!.forward();
    }

    public override void cancel()
    {
        this._alphaController?.forward();
    }

    internal virtual void _handleAlphaStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
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
        var paint__7103 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = this.color.withAlpha(((global::Doroti.Framework.Animation.Animation<long>)this._alpha).value);
            return __cascade;        }))();
        global::Doroti.Ui.Offset? center__7171 = ((global::Doroti.Ui.Offset?)(object?)this._position);
        if (this._repositionToReferenceBox)
        {
            center__7171 = Dart_uiLibrary.Offset.lerp(center__7171, ((global::Doroti.Framework.Rendering.RenderBox)this.referenceBox).size.center(Offset.zero), ((global::Doroti.Framework.Animation.AnimationController)this._radiusController).value);
        }
        paintInkCircle(canvas: canvas, transform: transform, paint: paint__7103, center: DartRuntimePrimitives.RequireValue(center__7171), textDirection: this._textDirection, radius: ((global::Doroti.Framework.Animation.Animation<double>)this._radius).value, customBorder: this.customBorder, borderRadius: this._borderRadius, clipCallback: (global::System.Func<Rect>?)this._clipCallback);
    }

}

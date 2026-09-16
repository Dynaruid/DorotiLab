// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/no_splash.dart

using Doroti.Ui;

namespace Doroti.Framework.Material;

internal class _NoSplashFactory__no_splash : InteractiveInkFeatureFactory
{
    internal _NoSplashFactory__no_splash()
    {
    }

    public virtual InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null)
    {
        return ((InteractiveInkFeature)new NoSplash(controller: controller, referenceBox: referenceBox, color: color, onRemoved: onRemoved));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NoSplash : InteractiveInkFeature
{
    public static InteractiveInkFeatureFactory splashFactory = ((InteractiveInkFeatureFactory)new _NoSplashFactory__no_splash());

    public NoSplash(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Color color, global::System.Action? onRemoved = null) : base(controller: controller, referenceBox: referenceBox, color: color, onRemoved: onRemoved)
    {
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
    }

    public override void confirm()
    {
        base.confirm();
        dispose();
    }

    public override void cancel()
    {
        base.cancel();
        dispose();
    }

}

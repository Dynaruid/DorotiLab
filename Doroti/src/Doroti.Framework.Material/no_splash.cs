// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/no_splash.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

internal class _NoSplashFactory__no_splash : InteractiveInkFeatureFactory
{
    internal _NoSplashFactory__no_splash()
    {
    }

    public virtual InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null)
    {
        return ((InteractiveInkFeature)(object?)new NoSplash(controller: controller, referenceBox: referenceBox, color: color, onRemoved: () => onRemoved()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NoSplash : InteractiveInkFeature
{
    public static InteractiveInkFeatureFactory splashFactory = ((InteractiveInkFeatureFactory)(object?)new _NoSplashFactory__no_splash());

    public NoSplash(MaterialInkController controller, global::Doroti.Generated.Framework.Rendering.RenderBox referenceBox, Color color, global::System.Action? onRemoved = null) : base(controller: controller, referenceBox: referenceBox, color: color, onRemoved: onRemoved)
    {
    }

    public override void paintFeature(Canvas canvas, Matrix4 transform)
    {
    }

    public virtual void confirm()
    {
        base.confirm();
        dispose();
    }

    public virtual void cancel()
    {
        base.cancel();
        dispose();
    }

}

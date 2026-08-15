// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/elevation_overlay.dart
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

public abstract class ElevationOverlay
{
    public static global::Doroti.Flutter.Ui.Color applySurfaceTint(Color color, Color? surfaceTint, double elevation)
    {
        if (((surfaceTint is not null) && (!object.Equals(surfaceTint, Colors.transparent))))
        {
            return ((global::Doroti.Flutter.Ui.Color)(object?)Dart_uiLibrary.Color.alphaBlend(surfaceTint.withOpacity(ElevationOverlay._surfaceTintOpacityForElevation(elevation)), color));
        }
        return ((global::Doroti.Flutter.Ui.Color)(object?)color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _surfaceTintOpacityForElevation(double elevation)
    {
        if ((elevation < Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(0L)].elevation))
        {
            return Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(0L)].opacity;
        }
        var index__2180 = 0L;
        while ((elevation >= Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(index__2180)].elevation))
        {
            if (((elevation == Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(index__2180)].elevation) || ((index__2180 + 1L) == checked((long)(Elevation_overlayLibrary._surfaceTintElevationOpacities.Count)))))
            {
                return Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(index__2180)].opacity;
            }
            index__2180 += 1L;
        }
        _ElevationOpacity__elevation_overlay lower__2659 = Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)((index__2180 - 1L))];
        _ElevationOpacity__elevation_overlay upper__2738 = Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(index__2180)];
        double t__2802 = (((elevation - ((_ElevationOpacity__elevation_overlay)lower__2659).elevation)) / ((((_ElevationOpacity__elevation_overlay)upper__2738).elevation - ((_ElevationOpacity__elevation_overlay)lower__2659).elevation)));
        return (((_ElevationOpacity__elevation_overlay)lower__2659).opacity + (t__2802 * ((((_ElevationOpacity__elevation_overlay)upper__2738).opacity - ((_ElevationOpacity__elevation_overlay)lower__2659).opacity))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.Color applyOverlay(global::Doroti.Generated.Framework.Widgets.BuildContext context, Color color, double elevation)
    {
        ThemeData theme__4629 = Theme.of(context);
        if (((((elevation > 0.0) && theme__4629.applyElevationOverlayColor) && (object.Equals(theme__4629.brightness, Brightness.dark))) && (object.Equals(color.withOpacity(1.0), theme__4629.colorScheme.surface.withOpacity(1.0)))))
        {
            return ((global::Doroti.Flutter.Ui.Color)(object?)ElevationOverlay.colorWithOverlay(color, theme__4629.colorScheme.onSurface, elevation));
        }
        return ((global::Doroti.Flutter.Ui.Color)(object?)color);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.Color overlayColor(global::Doroti.Generated.Framework.Widgets.BuildContext context, double elevation)
    {
        ThemeData theme__5534 = Theme.of(context);
        return ((global::Doroti.Flutter.Ui.Color)(object?)ElevationOverlay._overlayColor(theme__5534.colorScheme.onSurface, elevation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Flutter.Ui.Color colorWithOverlay(Color surface, Color overlay, double elevation)
    {
        return ((global::Doroti.Flutter.Ui.Color)(object?)Dart_uiLibrary.Color.alphaBlend(ElevationOverlay._overlayColor(overlay, elevation), surface));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Flutter.Ui.Color _overlayColor(Color color, double elevation)
    {
        double opacity__6672 = ((((4.5 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.log((elevation + 1L))) + 2L)) / 100.0);
        return ((global::Doroti.Flutter.Ui.Color)(object?)color.withOpacity(opacity__6672));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ElevationOpacity__elevation_overlay
{
    public virtual double elevation { get; private set; } = default!;
    public virtual double opacity { get; private set; } = default!;

    internal _ElevationOpacity__elevation_overlay(double elevation, double opacity)
    {
        this.elevation = elevation;
        this.opacity = opacity;
    }

}

public static partial class Elevation_overlayLibrary
{
    internal static List<_ElevationOpacity__elevation_overlay> _surfaceTintElevationOpacities = new List<_ElevationOpacity__elevation_overlay> { new _ElevationOpacity__elevation_overlay(0.0, 0.0), new _ElevationOpacity__elevation_overlay(1.0, 0.05), new _ElevationOpacity__elevation_overlay(3.0, 0.08), new _ElevationOpacity__elevation_overlay(6.0, 0.11), new _ElevationOpacity__elevation_overlay(8.0, 0.12), new _ElevationOpacity__elevation_overlay(12.0, 0.14) };
}

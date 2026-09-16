// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/elevation_overlay.dart

using Doroti.Ui;

namespace Doroti.Framework.Material;

public abstract class ElevationOverlay
{
    public static global::Doroti.Ui.Color applySurfaceTint(Color color, Color? surfaceTint, double elevation)
    {
        if ((surfaceTint is not null) && (!Equals(surfaceTint, Colors.transparent)))
        {
            return Dart_uiLibrary.Color.alphaBlend(surfaceTint.withOpacity(_surfaceTintOpacityForElevation(elevation)), color);
        }
        return color;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _surfaceTintOpacityForElevation(double elevation)
    {
        if (elevation < Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)0L].elevation)
        {
            return Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)0L].opacity;
        }
        var index = 0L;
        while (elevation >= Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)index].elevation)
        {
            if ((elevation == Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)index].elevation) || ((index + 1L) == checked(Elevation_overlayLibrary._surfaceTintElevationOpacities.Count)))
            {
                return Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)index].opacity;
            }
            index += 1L;
        }
        _ElevationOpacity__elevation_overlay lower = Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)(index - 1L)];
        _ElevationOpacity__elevation_overlay upper = Elevation_overlayLibrary._surfaceTintElevationOpacities[(int)index];
        double t = (elevation - lower.elevation) / (upper.elevation - lower.elevation);
        return lower.opacity + (t * (upper.opacity - lower.opacity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Color applyOverlay(global::Doroti.Framework.Widgets.BuildContext context, Color color, double elevation)
    {
        ThemeData theme = Theme.of(context);
        if ((elevation > 0.0) && theme.applyElevationOverlayColor && Equals(theme.brightness, Brightness.dark) && Equals(color.withOpacity(1.0), theme.colorScheme.surface.withOpacity(1.0)))
        {
            return colorWithOverlay(color, theme.colorScheme.onSurface, elevation);
        }
        return color;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Color overlayColor(global::Doroti.Framework.Widgets.BuildContext context, double elevation)
    {
        ThemeData theme = Theme.of(context);
        return _overlayColor(theme.colorScheme.onSurface, elevation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Color colorWithOverlay(Color surface, Color overlay, double elevation)
    {
        return Dart_uiLibrary.Color.alphaBlend(_overlayColor(overlay, elevation), surface);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Color _overlayColor(Color color, double elevation)
    {
        double opacity = ((4.5 * Runtime.Dart_mathLibrary.log(elevation + 1L)) + 2L) / 100.0;
        return color.withOpacity(opacity);
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

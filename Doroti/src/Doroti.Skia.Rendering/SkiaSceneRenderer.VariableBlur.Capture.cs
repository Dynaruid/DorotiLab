using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    internal static SKRectI VariableBlurCaptureBounds(
        SKRect visible,
        VariableBlurSettings settings,
        TileMode tile,
        SKMatrix matrix,
        int width,
        int height
    ) => VariableBlurCaptureBounds(visible, settings, tile, matrix, width, height, out _);

    internal static SKRectI VariableBlurCaptureBounds(
        SKRect visible,
        VariableBlurSettings settings,
        TileMode tile,
        SKMatrix matrix,
        int width,
        int height,
        out string reason
    )
    {
        var full = new SKRectI(0, 0, width, height);
        // Wrapped samples need the original domain. Fractional working sizes
        // also keep that domain: changing their rounded scale shifts the grid.
        var grid = 1 / settings.ResolutionScale;
        reason =
            tile is TileMode.repeated or TileMode.mirror ? "wrapped-domain"
            : grid is not (1 or 2 or 4) ? "unsupported-grid"
            : width % (int)grid != 0 || height % (int)grid != 0 ? "rounded-working-grid"
            : matrix.Persp0 != 0 || matrix.Persp1 != 0 || matrix.Persp2 != 1 ? "perspective"
            : "cropped";
        if (reason != "cropped")
            return full;
        var dx = (double)(float)settings.End.dx - (float)settings.Start.dx;
        var dy = (double)(float)settings.End.dy - (float)settings.Start.dy;
        var length = Math.Sqrt(dx * dx + dy * dy);
        var ux = dx / length;
        var uy = dy / length;
        var ax = matrix.ScaleX * ux + matrix.SkewX * uy;
        var ay = matrix.SkewY * ux + matrix.ScaleY * uy;
        var bx = -matrix.ScaleX * uy + matrix.SkewX * ux;
        var by = -matrix.SkewY * uy + matrix.ScaleY * ux;
        var radius = 3 * Math.Max(settings.StartSigma, settings.EndSigma);
        // Both axes plus downsample/reconstruction bilinear footprints. Align
        // origin AND extent to every dyadic working grid, preserving pixel phase.
        var haloX = radius * (Math.Abs(ax) + Math.Abs(bx)) + 2 * grid;
        var haloY = radius * (Math.Abs(ay) + Math.Abs(by)) + 2 * grid;
        if (!double.IsFinite(haloX) || !double.IsFinite(haloY))
        {
            reason = "nonfinite-halo";
            return full;
        }
        var result = new SKRectI(
            (int)Math.Max(0, Math.Floor((visible.Left - haloX) / grid) * grid),
            (int)Math.Max(0, Math.Floor((visible.Top - haloY) / grid) * grid),
            (int)Math.Min(width, Math.Ceiling((visible.Right + haloX) / grid) * grid),
            (int)Math.Min(height, Math.Ceiling((visible.Bottom + haloY) / grid) * grid)
        );
        if (result == full)
            reason = "halo-covers-domain";
        return result;
    }
}

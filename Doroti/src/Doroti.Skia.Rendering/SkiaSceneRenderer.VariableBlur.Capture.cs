using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    // Diagnostic A/B switch: identical binary/kernel, original capture domain.
    private static readonly bool DisableVariableBlurCrop =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_DISABLE_CROP") == "1";

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
        out string reason,
        int allocationAlignment = 1
    )
    {
        var full = new SKRectI(0, 0, width, height);
        if (DisableVariableBlurCrop)
        {
            reason = "forced-full-domain";
            return full;
        }
        if (settings.Kernel == VariableBlurKernel.dualKawase
            && VariableBlurKawaseGeometry(settings, tile, matrix, out _, out var depth, out _))
        {
            // Deepest shared stage determines both pixel phase and support.
            // Padding in the filter retains exact dyadic ratios at domain edges.
            var kawaseGrid = 1 << depth;
            var halo = 5 * kawaseGrid + 2;
            var allocationGrid = Math.Max(kawaseGrid, allocationAlignment);
            var cropped = new SKRectI(
                (int)Math.Max(0, Math.Floor((visible.Left - halo) / allocationGrid) * allocationGrid),
                (int)Math.Max(0, Math.Floor((visible.Top - halo) / allocationGrid) * allocationGrid),
                (int)Math.Min(width, Math.Ceiling((visible.Right + halo) / allocationGrid) * allocationGrid),
                (int)Math.Min(height, Math.Ceiling((visible.Bottom + halo) / allocationGrid) * allocationGrid));
            reason = cropped == full ? "kawase-halo-covers-domain" : "kawase-cropped";
            return cropped;
        }
        // Wrapped samples need the original domain. Preserve a nondivisible
        // axis independently: its ceil(size * scale) / size ratio cannot change,
        // but it does not prevent cropping the other axis on its dyadic grid.
        var grid = 1 / settings.ResolutionScale;
        reason =
            tile is TileMode.repeated or TileMode.mirror ? "wrapped-domain"
            : grid is not (1 or 2 or 4) ? "unsupported-grid"
            : matrix.Persp0 != 0 || matrix.Persp1 != 0 || matrix.Persp2 != 1 ? "perspective"
            : "cropped";
        if (reason != "cropped")
            return full;
        var cropX = width % (int)grid == 0;
        var cropY = height % (int)grid == 0;
        if (!cropX && !cropY)
        {
            reason = "rounded-working-grid";
            return full;
        }
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
        // Enlarge storage on a coarser dyadic grid without changing sigma,
        // sample phase or the required halo. Mobile slider drags then reuse
        // a few capture sizes instead of allocating at every pixel change.
        var storageGrid = Math.Max(grid, allocationAlignment);
        var result = new SKRectI(
            cropX ? (int)Math.Max(0, Math.Floor((visible.Left - haloX) / storageGrid) * storageGrid) : 0,
            cropY ? (int)Math.Max(0, Math.Floor((visible.Top - haloY) / storageGrid) * storageGrid) : 0,
            cropX ? (int)Math.Min(width, Math.Ceiling((visible.Right + haloX) / storageGrid) * storageGrid) : width,
            cropY ? (int)Math.Min(height, Math.Ceiling((visible.Bottom + haloY) / storageGrid) * storageGrid) : height
        );
        if (result == full)
            reason = "halo-covers-domain";
        else if (!cropX || !cropY)
            reason = cropX ? "cropped-x" : "cropped-y";
        return result;
    }
}

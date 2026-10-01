namespace Doroti.Ui;

/// <summary>Sampling kernel for a spatially varying blur.</summary>
public enum VariableBlurKernel
{
    gaussian,

    /// <summary>Pairs neighbouring Gaussian taps with bilinear reads, increasing
    /// the tap count with working sigma. Preserves the Gaussian kernel below two
    /// working pixels of sigma. Approximate when the sampling budget is exhausted.</summary>
    fastGaussian,
}

public sealed partial record ImageFilter
{
    /// <summary>
    /// Gaussian blur whose sigma varies linearly from start to end in local logical
    /// coordinates, clamped beyond the endpoints. Sigma is a standard deviation,
    /// not the sampling radius (approximately 3 * sigma). Requires a Skia GPU target.
    /// </summary>
    /// <remarks>
    /// Use an ancestor clip to bound a BackdropFilter. Supports affine transforms,
    /// ImageFiltered and composition. Gaussian passes take at most 2 * maxSamples + 1
    /// samples; large sigma at a low sample budget can show banding. fastGaussian
    /// pairs adjacent weights using up to min(4 * maxSamples + 1, 129) reads per pass and
    /// blends with the Gaussian at working sigma 2..3 (both kernels run in that
    /// transition). Fixed-cost widely spaced taps are not used. Arbitrary mask
    /// images and native PlatformView backdrop capture are not supported.
    /// resolutionScale (0.125..1) is the minimum working resolution. By default,
    /// adaptiveResolution preserves full resolution at low sigma and blends into
    /// coarser levels as blur increases. Set adaptiveResolution to false to use
    /// a fixed reduced resolution. The default scale 1 preserves full resolution.
    /// </remarks>
    public static ImageFilter variableBlur(
        Offset start,
        Offset end,
        double startSigma = 0,
        double endSigma = 20,
        int maxSamples = 32,
        TileMode tileMode = TileMode.clamp,
        Rect? bounds = null,
        double resolutionScale = 1,
        bool adaptiveResolution = true,
        VariableBlurKernel kernel = VariableBlurKernel.gaussian
    )
    {
        static bool Finite(double value) => double.IsFinite(value) && float.IsFinite((float)value);
        if (!Finite(start.dx) || !Finite(start.dy))
            throw new ArgumentOutOfRangeException(nameof(start));
        if (!Finite(end.dx) || !Finite(end.dy))
            throw new ArgumentOutOfRangeException(nameof(end));
        var dx = (double)(float)end.dx - (float)start.dx;
        var dy = (double)(float)end.dy - (float)start.dy;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length < 1e-6 || !Finite(length))
            throw new ArgumentException(
                "Blur endpoints must be distinct at shader precision.",
                nameof(end)
            );
        if (!Finite(startSigma) || startSigma < 0 || startSigma > float.MaxValue / 6)
            throw new ArgumentOutOfRangeException(nameof(startSigma));
        if (!Finite(endSigma) || endSigma < 0 || endSigma > float.MaxValue / 6)
            throw new ArgumentOutOfRangeException(nameof(endSigma));
        if (maxSamples is < 1 or > 64)
            throw new ArgumentOutOfRangeException(
                nameof(maxSamples),
                "Use 1 through 64 samples per side."
            );
        if (!Enum.IsDefined(tileMode))
            throw new ArgumentOutOfRangeException(nameof(tileMode));
        if (!double.IsFinite(resolutionScale) || resolutionScale is < 0.125 or > 1)
            throw new ArgumentOutOfRangeException(nameof(resolutionScale));
        if (!Enum.IsDefined(kernel))
            throw new ArgumentOutOfRangeException(nameof(kernel));
        return new ImageFilter(tileMode: tileMode, bounds: bounds)
        {
            VariableBlur = new(
                start,
                end,
                startSigma,
                endSigma,
                maxSamples,
                resolutionScale,
                adaptiveResolution,
                kernel
            ),
        };
    }

    internal VariableBlurSettings? VariableBlur { get; private init; }
}

internal sealed record VariableBlurSettings(
    Offset Start,
    Offset End,
    double StartSigma,
    double EndSigma,
    int MaxSamples,
    double ResolutionScale,
    bool AdaptiveResolution,
    VariableBlurKernel Kernel
);

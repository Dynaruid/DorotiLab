using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private readonly bool _croppedAdaptiveBands =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_FULL_BANDS") != "1";

    private SKShader CreateVariableBlurBandShader(SKCanvas target, SKImage input,
        VariableBlurSettings settings, TileMode tileMode, SKMatrix matrix, SKRect region)
    {
        var width = Math.Max(1, (int)Math.Ceiling(input.Width * settings.ResolutionScale));
        var height = Math.Max(1, (int)Math.Ceiling(input.Height * settings.ResolutionScale));
        var sx = (float)width / input.Width;
        var sy = (float)height / input.Height;
        using var reducedSurface = settings.ResolutionScale < 1 ? CreateFilterSurface(target, width, height) : null;
        if (reducedSurface is not null)
        {
            reducedSurface.Canvas.DrawImage(input, SKRect.Create(width, height), new SKSamplingOptions(SKFilterMode.Linear));
            RecordVariableBlurWork("downsample", input.Width, input.Height, width, height, SKRect.Create(width, height));
        }
        using var reduced = reducedSurface?.Snapshot();
        var bounds = new SKRect(region.Left * sx, region.Top * sy, region.Right * sx, region.Bottom * sy);
        bounds.Inflate(1, 1); // final sampling also needs neighbouring working pixels
        using var blurred = ApplyVariableBlurRegion(target, reduced ?? input,
            settings with { ResolutionScale = 1 },
            SKMatrix.Concat(SKMatrix.CreateScale(sx, sy), matrix), bounds, out var origin, tileMode);
        // The image's storage origin differs from its working-domain origin.
        // Map capture p to p * actualRatio - origin, never scale by crop extent.
        return blurred.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp,
            new SKSamplingOptions(SKFilterMode.Linear), SKMatrix.Concat(
                SKMatrix.CreateScale(1 / sx, 1 / sy), SKMatrix.CreateTranslation(origin.X, origin.Y)));
    }
    private IEnumerable<(SKShader Shader, SKRect Region)> VariableBlurBands(
        SKCanvas target,
        SKImage input,
        VariableBlurSettings settings,
        TileMode tileMode,
        int width,
        int height,
        SKMatrix matrix,
        SKMatrix inverse,
        SKRect? outputBounds
    )
    {
        var dx = (double)(float)settings.End.dx - (float)settings.Start.dx;
        var dy = (double)(float)settings.End.dy - (float)settings.Start.dy;
        var lengthSquared = dx * dx + dy * dy;
        var rx = (dx * inverse.ScaleX + dy * inverse.SkewY) / lengthSquared;
        var ry = (dx * inverse.SkewX + dy * inverse.ScaleY) / lengthSquared;
        var rz =
            (
                dx * (inverse.TransX - (float)settings.Start.dx)
                + dy * (inverse.TransY - (float)settings.Start.dy)
            ) / lengthSquared;

        // Use the smallest singular value: stretching one axis must not allow
        // coarse sampling to erase detail along the other (including shear).
        var a = (double)matrix.ScaleX;
        var b = (double)matrix.SkewX;
        var c = (double)matrix.SkewY;
        var d = (double)matrix.ScaleY;
        var trace = a * a + b * b + c * c + d * d;
        var determinant = a * d - b * c;
        var largest = Math.Sqrt(
            (trace + Math.Sqrt(Math.Max(0, trace * trace - 4 * determinant * determinant))) / 2
        );
        var deviceScale = Math.Abs(determinant) / largest;
        var firstSigma = settings.StartSigma * deviceScale;
        var lastSigma = settings.EndSigma * deviceScale;
        var delta = lastSigma - firstSigma;
        var fixedSettings = settings with { AdaptiveResolution = false };
        var levels = new List<double> { 1 };
        for (var scale = settings.ResolutionScale; scale < 1; scale *= 2)
            levels.Add(scale);
        levels.Sort((x, y) => y.CompareTo(x));

        if (
            !double.IsFinite(deviceScale)
            || deviceScale <= 0
            || Math.Max(firstSigma, lastSigma) <= 1 / levels[1]
        )
        {
            using var fallbackImage = ApplyVariableBlur(
                target,
                input,
                fixedSettings with
                {
                    ResolutionScale = 1,
                },
                tileMode,
                width,
                height,
                matrix,
                outputBounds,
                keepWorkingResolution: true
            );
            using var fallbackShader = VariableBlurImageShader(fallbackImage, width, height);
            yield return (fallbackShader, outputBounds ?? SKRect.Create(width, height));
            yield break;
        }
        if (Math.Min(firstSigma, lastSigma) >= 2 / levels[^1])
        {
            using var fallbackImage = ApplyVariableBlur(
                target,
                input,
                fixedSettings,
                tileMode,
                width,
                height,
                matrix,
                outputBounds,
                keepWorkingResolution: true
            );
            using var fallbackShader = VariableBlurImageShader(fallbackImage, width, height);
            yield return (fallbackShader, outputBounds ?? SKRect.Create(width, height));
            yield break;
        }

        var norm = rx * rx + ry * ry;
        var gradientStart = new SKPoint((float)(-rz * rx / norm), (float)(-rz * ry / norm));
        var gradientEnd = new SKPoint((float)((1 - rz) * rx / norm), (float)((1 - rz) * ry / norm));
        // Preserve the existing full-resolution precision checks rather than
        // sending degenerate or nonfinite gradient coordinates to Skia.
        if (
            !float.IsFinite(gradientStart.X)
            || !float.IsFinite(gradientStart.Y)
            || !float.IsFinite(gradientEnd.X)
            || !float.IsFinite(gradientEnd.Y)
            || gradientStart == gradientEnd
        )
        {
            using var fallbackImage = ApplyVariableBlur(
                target,
                input,
                fixedSettings with
                {
                    ResolutionScale = 1,
                },
                tileMode,
                width,
                height,
                matrix,
                outputBounds,
                keepWorkingResolution: true
            );
            using var fallbackShader = VariableBlurImageShader(fallbackImage, width, height);
            yield return (fallbackShader, outputBounds ?? SKRect.Create(width, height));
            yield break;
        }

        var visible = outputBounds ?? SKRect.Create(width, height);
        visible = new SKRect(
            MathF.Floor(visible.Left),
            MathF.Floor(visible.Top),
            MathF.Ceiling(visible.Right),
            MathF.Ceiling(visible.Bottom)
        );
        visible.Intersect(SKRect.Create(width, height));

        for (var level = 0; level < levels.Count; level++)
        {
            // Transition into a coarser level only when its sigma is 1..2 working
            // pixels. Adjacent weights sum to one, including premultiplied alpha.
            var enter = level == 0 ? 0 : 1 / levels[level];
            var leave = level + 1 == levels.Count ? double.PositiveInfinity : 2 / levels[level + 1];
            if (Math.Max(firstSigma, lastSigma) < enter || Math.Min(firstSigma, lastSigma) > leave)
                continue;
            var region = VariableBlurBandBounds(
                visible,
                rx * delta,
                ry * delta,
                rz * delta + firstSigma,
                enter,
                leave,
                Math.Min(firstSigma, lastSigma),
                Math.Max(firstSigma, lastSigma)
            );
            if (region.IsEmpty)
                continue;

            double Weight(double sigma)
            {
                var entering = level == 0 ? 1 : Math.Clamp(sigma / enter - 1, 0, 1);
                var leaving = double.IsPositiveInfinity(leave)
                    ? 0
                    : Math.Clamp(2 * sigma / leave - 1, 0, 1);
                return entering - leaving;
            }

            var stops = new List<double> { 0, 1 };
            if (delta != 0)
                foreach (var sigma in new[] { enter, 2 * enter, leave / 2, leave })
                {
                    var t = (sigma - firstSigma) / delta;
                    if (t > 0 && t < 1)
                        stops.Add(t);
                }
            var positions = stops.Select(t => (float)t).Distinct().Order().ToArray();
            var colors = positions
                .Select(t =>
                    SKColors.White.WithAlpha(
                        (byte)Math.Clamp(Math.Round(255 * Weight(firstSigma + delta * t)), 0, 255)
                    )
                )
                .ToArray();
            if (colors.All(color => color.Alpha == 0))
                continue;

            // All levels share the same capture domain and pixel origin. Cropping
            // the input per band shifts sampling on retained Graphite snapshots.
            using var blurred = _croppedAdaptiveBands && tileMode is TileMode.clamp or TileMode.decal ? null : ApplyVariableBlur(
                target,
                input,
                fixedSettings with
                {
                    ResolutionScale = levels[level],
                },
                tileMode,
                width,
                height,
                matrix,
                region,
                keepWorkingResolution: true
            );
            using var image = blurred is null
                ? CreateVariableBlurBandShader(target, input,
                    fixedSettings with { ResolutionScale = levels[level] }, tileMode, matrix, region)
                : VariableBlurImageShader(blurred, width, height);
            using var mask = SKShader.CreateLinearGradient(
                gradientStart,
                gradientEnd,
                colors,
                positions,
                SKShaderTileMode.Clamp
            );
            using var weighted = SKShader.CreateBlend(SKBlendMode.DstIn, image, mask);
            yield return (weighted, region);
        }
    }

    private static SKShader VariableBlurImageShader(SKImage image, int width, int height) =>
        image.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp,
            new SKSamplingOptions(SKFilterMode.Linear),
            SKMatrix.CreateScale((float)width / image.Width, (float)height / image.Height));

    // Accumulate only over transparent storage; Plus must never be applied to
    // the existing backdrop. The final clip/blend/opacity still applies once.
    private SKImage ApplyAdaptiveVariableBlur(SKCanvas target, SKImage input,
        VariableBlurSettings settings, TileMode tileMode, int width, int height,
        SKMatrix matrix, SKMatrix inverse, SKRect? outputBounds)
    {
        using var output = CreateFilterSurface(target, width, height);
        foreach (var band in VariableBlurBands(target, input, settings, tileMode,
            width, height, matrix, inverse, outputBounds))
        {
            using var paint = new SKPaint { Shader = band.Shader, BlendMode = SKBlendMode.Plus };
            var started = StartVariableBlurStage();
            output.Canvas.DrawRect(band.Region, paint);
            EndVariableBlurStage("band-blend", started);
        }
        return output.Snapshot();
    }

    private SKShader CreateAdaptiveVariableBlurShader(SKCanvas target, SKImage input,
        VariableBlurSettings settings, TileMode tileMode, int width, int height,
        SKMatrix matrix, SKMatrix inverse, SKRect? outputBounds)
    {
        SKShader? combined = null;
        try
        {
            foreach (var band in VariableBlurBands(target, input, settings, tileMode,
                width, height, matrix, inverse, outputBounds))
            {
                // The loop's shaders retain their images. Disposing the managed
                // snapshots does not retire an image still referenced by a shader.
                using var empty = combined is null ? SKShader.CreateColor(SKColors.Transparent) : null;
                var next = SKShader.CreateBlend(SKBlendMode.Plus, combined ?? empty!, band.Shader);
                combined?.Dispose();
                combined = next;
            }
            var result = combined ?? SKShader.CreateColor(SKColors.Transparent);
            combined = null;
            return result;
        }
        finally { combined?.Dispose(); }
    }

    private static SKRect VariableBlurBandBounds(
        SKRect visible,
        double x,
        double y,
        double z,
        double lower,
        double upper,
        double minSigma,
        double maxSigma
    )
    {
        var points = new List<SKPoint>
        {
            new(visible.Left, visible.Top),
            new(visible.Right, visible.Top),
            new(visible.Right, visible.Bottom),
            new(visible.Left, visible.Bottom),
        };
        void Clip(double threshold, double sign)
        {
            var clipped = new List<SKPoint>();
            for (var i = 0; i < points.Count; i++)
            {
                var p = points[i];
                var q = points[(i + 1) % points.Count];
                var fp = sign * (x * p.X + y * p.Y + z - threshold);
                var fq = sign * (x * q.X + y * q.Y + z - threshold);
                if (fp >= 0)
                    clipped.Add(p);
                if ((fp >= 0) != (fq >= 0))
                {
                    var t = fp / (fp - fq);
                    clipped.Add(
                        new((float)(p.X + (q.X - p.X) * t), (float)(p.Y + (q.Y - p.Y) * t))
                    );
                }
            }
            points = clipped;
        }
        if (lower > minSigma)
            Clip(lower, 1);
        if (upper < maxSigma)
            Clip(upper, -1);
        if (points.Count == 0)
            return SKRect.Empty;
        return new SKRect(
            MathF.Floor(points.Min(p => p.X)),
            MathF.Floor(points.Min(p => p.Y)),
            MathF.Ceiling(points.Max(p => p.X)),
            MathF.Ceiling(points.Max(p => p.Y))
        );
    }
}

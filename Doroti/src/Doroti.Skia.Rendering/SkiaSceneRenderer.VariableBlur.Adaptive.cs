using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private SKImage ApplyAdaptiveVariableBlur(
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
            return ApplyVariableBlur(
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
                outputBounds
            );
        if (Math.Min(firstSigma, lastSigma) >= 2 / levels[^1])
            return ApplyVariableBlur(
                target,
                input,
                fixedSettings,
                tileMode,
                width,
                height,
                matrix,
                outputBounds
            );

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
            return ApplyVariableBlur(
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
                outputBounds
            );

        var visible = outputBounds ?? SKRect.Create(width, height);
        visible = new SKRect(
            MathF.Floor(visible.Left),
            MathF.Floor(visible.Top),
            MathF.Ceiling(visible.Right),
            MathF.Ceiling(visible.Bottom)
        );
        visible.Intersect(SKRect.Create(width, height));
        using var output = CreateFilterSurface(target, width, height);
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

            using var blurred = ApplyVariableBlur(
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
                region
            );
            using var image = blurred.ToShader(
                SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp,
                new SKSamplingOptions(SKFilterMode.Nearest)
            );
            using var mask = SKShader.CreateLinearGradient(
                gradientStart,
                gradientEnd,
                colors,
                positions,
                SKShaderTileMode.Clamp
            );
            using var weighted = SKShader.CreateBlend(SKBlendMode.DstIn, image, mask);
            using var paint = new SKPaint { Shader = weighted, BlendMode = SKBlendMode.Plus };
            // Coverage comes from the gradient mask, not an antialiased band edge.
            output.Canvas.DrawRect(region, paint);
        }
        return output.Snapshot();
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

using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private readonly bool _boundedKawaseStages =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_FULL_STAGES") != "1";
    private readonly bool _croppedKawaseDetail =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_FULL_DETAIL") != "1";
    private static readonly Lazy<FragmentProgram> KawaseBlurProgram = new(() =>
    {
        RenderingShaderAssets.Register();
        return FrameworkShaderLoader.LoadEmbeddedProgram("rendering.kawase-blur");
    });
    private static readonly Lazy<FragmentProgram> VariableBlurStageProgram = new(() =>
    {
        RenderingShaderAssets.Register();
        return FrameworkShaderLoader.LoadEmbeddedProgram("rendering.variable-blur-stage");
    });

    // Phase-averaged impulse sigmas, including BOTH reconstruction and final
    // bilinear sampling. Depth 2 reconstructs to 1/2; deeper stages to 1/4.
    // The production-shader impulse regression checks these independently.
    internal static double VariableBlurKawaseSigma(int depth) => depth switch
    {
        2 => 3.5824,
        3 => 7.1995,
        4 => 15.2698,
        5 => 30.9597,
        6 => 62.1276,
        7 => 124.3597,
        _ => throw new ArgumentOutOfRangeException(nameof(depth)),
    };

    internal static bool VariableBlurKawaseGeometry(VariableBlurSettings settings,
        TileMode tile, SKMatrix matrix, out double scale, out int depth, out string reason)
    {
        var a = (double)matrix.ScaleX;
        var b = (double)matrix.SkewX;
        var c = (double)matrix.SkewY;
        var d = (double)matrix.ScaleY;
        var xx = a * a + c * c;
        var yy = b * b + d * d;
        scale = Math.Sqrt(xx);
        depth = 2;
        reason = tile != TileMode.clamp ? "kawase-tile-fallback"
            : matrix.Persp0 != 0 || matrix.Persp1 != 0 || matrix.Persp2 != 1
                || !double.IsFinite(scale) || scale <= 0 || !matrix.TryInvert(out _)
                || Math.Abs(xx - yy) > 1e-5 * xx || Math.Abs(a * b + c * d) > 1e-5 * xx
                ? "kawase-transform-fallback" : "";
        if (reason.Length != 0) return false;
        var sigma = scale * Math.Max(settings.StartSigma, settings.EndSigma);
        if (sigma <= 2) { reason = "kawase-weak-gaussian"; return false; }
        while (depth < 7 && VariableBlurKawaseSigma(depth) < sigma) depth++;
        if (sigma > VariableBlurKawaseSigma(depth))
        { reason = "kawase-radius-fallback"; return false; }
        var cap = 2 / scale;
        if (Math.Min(settings.StartSigma, settings.EndSigma) < cap)
        {
            var t = (cap - settings.StartSigma) / (settings.EndSigma - settings.StartSigma);
            var px = (float)(settings.Start.dx + t * (settings.End.dx - settings.Start.dx));
            var py = (float)(settings.Start.dy + t * (settings.End.dy - settings.Start.dy));
            var end = settings.StartSigma > settings.EndSigma ? settings.End : settings.Start;
            if (px == (float)end.dx && py == (float)end.dy)
            { reason = "kawase-coordinate-fallback"; return false; }
        }
        return true;
    }

    private SKImage KawaseBlurPass(SKCanvas target, SKImage input, int width, int height, bool upsample,
        SKRect? outputBounds = null)
    {
        var fragment = KawaseBlurProgram.Value.fragmentShader();
        fragment.setFloat(2, (double)input.Width / width);
        fragment.setFloat(3, (double)input.Height / height);
        fragment.setFloat(4, upsample ? 1 : 0);
        using var shader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
            (FragmentShaderSnapshot)ShaderSnapshot.Capture(fragment), input,
            new SKSamplingOptions(SKFilterMode.Linear), CreateImageShader,
            RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner, SKShaderTileMode.Clamp);
        using var output = CreateFilterSurface(target, width, height);
        using var paint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Src };
        var draw = outputBounds ?? SKRect.Create(width, height);
        RecordVariableBlurWork(upsample ? "kawase-up" : "kawase-down",
            input.Width, input.Height, width, height, draw);
        var started = StartVariableBlurStage();
        output.Canvas.DrawRect(draw, paint);
        EndVariableBlurStage(upsample ? "kawase-up-draw" : "kawase-down-draw", started);
        Interlocked.Increment(ref _shaderImageFiltersRendered);
        return output.Snapshot();
    }

    private SKShader CreateDualKawaseVariableBlurShader(SKCanvas target, SKImage input,
        VariableBlurSettings settings, int width, int height, SKMatrix matrix, SKRect? outputBounds,
        double deviceScale, int depth)
    {
        var grid = 1 << depth;
        var paddedWidth = checked((width + grid - 1) / grid * grid);
        var paddedHeight = checked((height + grid - 1) / grid * grid);
        matrix.TryInvert(out var inverse);
        var dx = (double)(float)settings.End.dx - (float)settings.Start.dx;
        var dy = (double)(float)settings.End.dy - (float)settings.Start.dy;
        var lengthSquared = dx * dx + dy * dy;
        var rx = (dx * inverse.ScaleX + dy * inverse.SkewY) / lengthSquared;
        var ry = (dx * inverse.SkewX + dy * inverse.ScaleY) / lengthSquared;
        var rz = (dx * (inverse.TransX - (float)settings.Start.dx)
            + dy * (inverse.TransY - (float)settings.Start.dy)) / lengthSquared;
        var firstSigma = settings.StartSigma * deviceScale;
        var lastSigma = settings.EndSigma * deviceScale;
        var visible = outputBounds ?? SKRect.Create(width, height);
        double Sigma(float x, float y) => firstSigma + (lastSigma - firstSigma)
            * Math.Clamp(rx * x + ry * y + rz, 0, 1);
        var visibleSigmas = new[] { Sigma(visible.Left, visible.Top), Sigma(visible.Right, visible.Top),
            Sigma(visible.Right, visible.Bottom), Sigma(visible.Left, visible.Bottom) };
        var min = visibleSigmas.Min();
        var max = visibleSigmas.Max();
        var downs = new List<SKImage>();
        var stages = new List<SKImage>();
        SKShader? combined = null;
        try
        {
            // Pad rather than stretch: every level has a true 2:1 ratio even on
            // odd backing sizes. Clamp padding retains the original edge pixels.
            using (var padded = CreateFilterSurface(target, paddedWidth, paddedHeight))
            {
                using var inputShader = input.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp,
                    new SKSamplingOptions(SKFilterMode.Linear));
                using var paint = new SKPaint { Shader = inputShader, BlendMode = SKBlendMode.Src };
                padded.Canvas.DrawRect(SKRect.Create(paddedWidth, paddedHeight), paint);
                RecordVariableBlurWork("kawase-pad", width, height, paddedWidth, paddedHeight,
                    SKRect.Create(paddedWidth, paddedHeight));
                downs.Add(padded.Snapshot());
            }
            for (var level = 1; level <= depth; level++)
                downs.Add(KawaseBlurPass(target, downs[^1], paddedWidth >> level,
                    paddedHeight >> level, false));
            for (var stageDepth = 2; stageDepth <= depth; stageDepth++)
            {
                var finish = stageDepth == 2 ? 1 : 2;
                var needed = new SKRect?[depth];
                if (_boundedKawaseStages)
                {
                    var band = VariableBlurBandBounds(visible, rx * (lastSigma - firstSigma),
                        ry * (lastSigma - firstSigma), rz * (lastSigma - firstSigma) + firstSigma,
                        stageDepth == 2 ? 0 : VariableBlurKawaseSigma(stageDepth - 1),
                        stageDepth == depth ? double.PositiveInfinity : VariableBlurKawaseSigma(stageDepth + 1),
                        min, max);
                    var factor = 1f / (1 << finish);
                    var region = new SKRect(band.Left * factor, band.Top * factor,
                        band.Right * factor, band.Bottom * factor);
                    region.Inflate(1, 1); // final linear reconstruction footprint
                    for (var level = finish; level < stageDepth; level++)
                    {
                        region = new SKRect(MathF.Floor(region.Left), MathF.Floor(region.Top),
                            MathF.Ceiling(region.Right), MathF.Ceiling(region.Bottom));
                        region.Intersect(SKRect.Create(paddedWidth >> level, paddedHeight >> level));
                        needed[level] = region;
                        region = new SKRect(region.Left * .5f, region.Top * .5f,
                            region.Right * .5f, region.Bottom * .5f);
                        region.Inflate(1.5f, 1.5f); // up kernel plus bilinear input footprint
                    }
                }
                SKImage? reconstructed = null;
                try
                {
                    for (var level = stageDepth - 1; level >= finish; level--)
                    {
                        var next = KawaseBlurPass(target, reconstructed ?? downs[stageDepth],
                            paddedWidth >> level, paddedHeight >> level, true, needed[level]);
                        reconstructed?.Dispose();
                        reconstructed = next;
                    }
                    stages.Add(reconstructed!);
                    reconstructed = null;
                }
                finally { reconstructed?.Dispose(); }
            }

            // Remap the original ramp to clamp at two device pixels. Below that
            // threshold it is the original variable Gaussian, not a sharp/blur
            // crossfade. This also bounds the detail pass's halo and tap count.
            var cap = 2 / deviceScale;
            var detailSettings = settings with { Kernel = VariableBlurKernel.gaussian,
                AdaptiveResolution = false, ResolutionScale = 1 };
            if (Math.Min(settings.StartSigma, settings.EndSigma) >= cap)
                detailSettings = detailSettings with { StartSigma = cap, EndSigma = cap };
            else
            {
                var delta = settings.EndSigma - settings.StartSigma;
                var t = (cap - settings.StartSigma) / delta;
                var point = new Offset(settings.Start.dx + t * (settings.End.dx - settings.Start.dx),
                    settings.Start.dy + t * (settings.End.dy - settings.Start.dy));
                detailSettings = settings.StartSigma > settings.EndSigma
                    ? detailSettings with { Start = point, StartSigma = cap }
                    : detailSettings with { End = point, EndSigma = cap };
            }
            var detailBounds = VariableBlurBandBounds(visible, rx * (lastSigma - firstSigma),
                ry * (lastSigma - firstSigma), rz * (lastSigma - firstSigma) + firstSigma,
                0, VariableBlurKawaseSigma(2), min, max);
            // A stage is consumed through bilinear sampling in the final shader.
            if (!detailBounds.IsEmpty) detailBounds.Inflate(1, 1);
            var detailOrigin = new SKPoint(0, 0);
            using var detail = detailBounds.IsEmpty ? null
                : _croppedKawaseDetail
                    ? ApplyVariableBlurRegion(target, input, detailSettings, matrix, detailBounds, out detailOrigin)
                    : ApplyVariableBlur(target, input, detailSettings, TileMode.clamp, width, height, matrix, detailBounds);
            for (var index = 0; index < stages.Count; index++)
            {
                var lowerSigma = index == 0 ? 2 : VariableBlurKawaseSigma(index + 1);
                var upperSigma = VariableBlurKawaseSigma(index + 2);
                if (min > upperSigma || max < (index == 0 ? 0 : lowerSigma)) continue;
                // A clipped ROI can contain only strong sigma while the global
                // endpoints still include zero. That inactive interval needs a
                // valid child even though it never samples within the final clip.
                var lower = index == 0 ? detail ?? input : stages[index - 1];
                var upper = stages[index];
                var fragment = VariableBlurStageProgram.Value.fragmentShader();
                var values = new double[] { rx, ry, rz, firstSigma, lastSigma, lowerSigma,
                    upperSigma, index == 0 ? 1 : (double)lower.Width / paddedWidth,
                    index == 0 ? 1 : (double)lower.Height / paddedHeight, (double)upper.Width / paddedWidth,
                    (double)upper.Height / paddedHeight, index == stages.Count - 1 ? 1 : 0,
                    index == 0 ? 1 : 0, index == 0 ? -detailOrigin.X : 0,
                    index == 0 ? -detailOrigin.Y : 0 };
                for (var j = 0; j < values.Length; j++) fragment.setFloat(j + 2, values[j]);
                using var band = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                    (FragmentShaderSnapshot)ShaderSnapshot.Capture(fragment), lower,
                    new SKSamplingOptions(SKFilterMode.Linear), CreateImageShader,
                    RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner,
                    SKShaderTileMode.Clamp, [upper]);
                using var empty = combined is null ? SKShader.CreateColor(SKColors.Transparent) : null;
                var next = SKShader.CreateBlend(SKBlendMode.Plus, combined ?? empty!, band);
                combined?.Dispose();
                combined = next;
                RecordVariableBlurWork("kawase-interval", lower.Width, lower.Height,
                    width, height, visible, $"sigma={lowerSigma:F4}..{upperSigma:F4}");
            }
            var result = combined ?? SKShader.CreateColor(SKColors.Transparent);
            combined = null;
            return result;
        }
        finally
        {
            combined?.Dispose();
            foreach (var image in stages) image.Dispose();
            foreach (var image in downs) image.Dispose();
        }
    }

}

using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    // Blur along the gradient FIRST, then along its perpendicular. Sigma is
    // constant along the second axis, so it does not pick up a different kernel
    // from neighbouring rows (the usual variable separable-blur streak artifact).
    // The finite, bilinearly sampled kernel is an approximation to a Gaussian.
    private static readonly Lazy<FragmentProgram> VariableBlurProgram = new(() =>
    {
        FrameworkShaderLoader.RegisterResourceOwner(typeof(SkiaSceneRenderer).Assembly);
        return FrameworkShaderLoader.LoadEmbeddedProgram("rendering.variable-blur");
    });

    private SKImage ApplyVariableBlur(
        SKCanvas target,
        SKImage input,
        VariableBlurSettings settings,
        TileMode tileMode,
        int width,
        int height,
        SKMatrix matrix,
        SKRect? outputBounds = null,
        bool keepWorkingResolution = false
    )
    {
        if (
            matrix.Persp0 != 0
            || matrix.Persp1 != 0
            || matrix.Persp2 != 1
            || !matrix.TryInvert(out var inverse)
        )
            throw new NotSupportedException(
                "ImageFilter.variableBlur requires an invertible affine transform."
            );

        if (settings.ResolutionScale < 1 && settings.AdaptiveResolution)
            return ApplyAdaptiveVariableBlur(
                target,
                input,
                settings,
                tileMode,
                width,
                height,
                matrix,
                inverse,
                outputBounds
            );

        // Keep the capture domain (and therefore tile modes) intact. Only the
        // working resolution changes; use the actual rounded dimensions on each
        // axis so odd sizes and nonuniform transforms retain their coordinates.
        if (settings.ResolutionScale < 1 && Math.Max(settings.StartSigma, settings.EndSigma) > 0)
        {
            var smallWidth = Math.Max(1, (int)Math.Ceiling(width * settings.ResolutionScale));
            var smallHeight = Math.Max(1, (int)Math.Ceiling(height * settings.ResolutionScale));
            var sx = (float)smallWidth / width;
            var sy = (float)smallHeight / height;
            using var small = CreateFilterSurface(target, smallWidth, smallHeight);
            var downsampleStarted = StartVariableBlurStage();
            small.Canvas.DrawImage(
                input,
                SKRect.Create(smallWidth, smallHeight),
                new SKSamplingOptions(SKFilterMode.Linear)
            );
            EndVariableBlurStage("downsample-draw", downsampleStarted);
            downsampleStarted = StartVariableBlurStage();
            using var reduced = small.Snapshot();
            EndVariableBlurStage("downsample-snapshot", downsampleStarted);
            SKRect? smallBounds = outputBounds is { } bounds
                ? new SKRect(
                    bounds.Left * sx,
                    bounds.Top * sy,
                    bounds.Right * sx,
                    bounds.Bottom * sy
                )
                : null;
            // Upsampling reads neighbouring low-resolution pixels as well.
            if (smallBounds is { } expanded)
            {
                expanded.Inflate(1, 1);
                smallBounds = expanded;
            }
            var blurred = ApplyVariableBlur(
                target,
                reduced,
                settings with
                {
                    ResolutionScale = 1,
                },
                tileMode,
                smallWidth,
                smallHeight,
                SKMatrix.Concat(SKMatrix.CreateScale(sx, sy), matrix),
                smallBounds
            );
            if (keepWorkingResolution)
                return blurred;
            using (blurred)
            {
                using var restored = CreateFilterSurface(target, width, height);
                restored.Canvas.DrawImage(
                    blurred,
                    SKRect.Create(width, height),
                    new SKSamplingOptions(SKFilterMode.Linear)
                );
                return restored.Snapshot();
            }
        }

        var dx = (double)(float)settings.End.dx - (float)settings.Start.dx;
        var dy = (double)(float)settings.End.dy - (float)settings.Start.dy;
        var length = Math.Sqrt(dx * dx + dy * dy);
        var ux = dx / length;
        var uy = dy / length;
        var rampX = (ux * inverse.ScaleX + uy * inverse.SkewY) / length;
        var rampY = (ux * inverse.SkewX + uy * inverse.ScaleY) / length;
        var rampZ =
            (
                ux * (inverse.TransX - (float)settings.Start.dx)
                + uy * (inverse.TransY - (float)settings.Start.dy)
            ) / length;
        var mode = tileMode switch
        {
            TileMode.clamp => SKShaderTileMode.Clamp,
            TileMode.decal => SKShaderTileMode.Decal,
            TileMode.repeated => SKShaderTileMode.Repeat,
            TileMode.mirror => SKShaderTileMode.Mirror,
            _ => throw new ArgumentOutOfRangeException(nameof(tileMode)),
        };

        var full = SKRect.Create(width, height);
        var outputRect = outputBounds ?? full;
        // Round outward: partial pixel clips must be applied only once, at final
        // composition. The first pass also needs the second pass's sampling halo.
        outputRect = new SKRect(
            MathF.Floor(outputRect.Left),
            MathF.Floor(outputRect.Top),
            MathF.Ceiling(outputRect.Right),
            MathF.Ceiling(outputRect.Bottom)
        );
        outputRect.Intersect(full);
        var firstRect = full;
        if (tileMode is TileMode.clamp or TileMode.decal)
        {
            var radius = 3 * Math.Max(settings.StartSigma, settings.EndSigma);
            var haloX = radius * Math.Abs(-matrix.ScaleX * uy + matrix.SkewX * ux) + 1;
            var haloY = radius * Math.Abs(-matrix.SkewY * uy + matrix.ScaleY * ux) + 1;
            firstRect = new SKRect(
                (float)Math.Max(0, outputRect.Left - haloX),
                (float)Math.Max(0, outputRect.Top - haloY),
                (float)Math.Min(width, outputRect.Right + haloX),
                (float)Math.Min(height, outputRect.Bottom + haloY)
            );
        }

        SKImage Pass(SKImage source, double x, double y, SKRect drawRect)
        {
            var axisX = matrix.ScaleX * x + matrix.SkewX * y;
            var axisY = matrix.SkewY * x + matrix.ScaleY * y;
            var values = new double[]
            {
                rampX,
                rampY,
                rampZ,
                settings.StartSigma,
                settings.EndSigma,
                axisX,
                axisY,
                settings.MaxSamples,
                settings.Kernel == VariableBlurKernel.fastGaussian ? 1 : 0,
            };
            if (
                values.Any(v => !float.IsFinite((float)v))
                || !float.IsFinite(
                    (float)(
                        3
                        * Math.Max(settings.StartSigma, settings.EndSigma)
                        * Math.Sqrt(axisX * axisX + axisY * axisY)
                    )
                )
            )
                throw new NotSupportedException(
                    "Variable blur exceeds shader coordinate precision."
                );
            var stageStarted = StartVariableBlurStage();
            var fragment = VariableBlurProgram.Value.fragmentShader();
            for (var i = 0; i < values.Length; i++)
                fragment.setFloat(i + 2, values[i]);
            using var shader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                (FragmentShaderSnapshot)ShaderSnapshot.Capture(fragment),
                source,
                new SKSamplingOptions(SKFilterMode.Linear),
                CreateImageShader,
                RuntimeEffectBackend,
                _contextGeneration,
                _runtimeEffectContextOwner,
                mode
            );
            EndVariableBlurStage("gaussian-bind-shader", stageStarted);
            using var output = CreateFilterSurface(target, width, height);
            using var paint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Src };
            stageStarted = StartVariableBlurStage();
            output.Canvas.DrawRect(drawRect, paint);
            EndVariableBlurStage("gaussian-draw", stageStarted);
            Interlocked.Increment(ref _shaderImageFiltersRendered);
            stageStarted = StartVariableBlurStage();
            var snapshot = output.Snapshot();
            EndVariableBlurStage("gaussian-snapshot", stageStarted);
            return snapshot;
        }

        using var first = Pass(input, ux, uy, firstRect);
        return Pass(first, -uy, ux, outputRect);
    }
}

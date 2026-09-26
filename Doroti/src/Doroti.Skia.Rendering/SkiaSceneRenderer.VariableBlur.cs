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
        SKMatrix matrix
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

        SKImage Pass(SKImage source, double x, double y)
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
            using var output = CreateFilterSurface(target, width, height);
            using var paint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Src };
            output.Canvas.DrawRect(SKRect.Create(width, height), paint);
            Interlocked.Increment(ref _shaderImageFiltersRendered);
            return output.Snapshot();
        }

        using var first = Pass(input, ux, uy);
        return Pass(first, -uy, ux);
    }
}

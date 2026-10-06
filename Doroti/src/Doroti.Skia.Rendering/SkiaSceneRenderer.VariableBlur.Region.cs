using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    // Keep Gaussian shader coordinates in the working domain; only storage is local.
    private SKImage ApplyVariableBlurRegion(SKCanvas target, SKImage input,
        VariableBlurSettings settings, SKMatrix matrix, SKRect bounds, out SKPoint origin,
        TileMode tileMode = TileMode.clamp)
    {
        if (matrix.Persp0 != 0 || matrix.Persp1 != 0 || matrix.Persp2 != 1
            || !matrix.TryInvert(out var inverse))
            throw new NotSupportedException("ImageFilter.variableBlur requires an invertible affine transform.");
        var dx = (double)(float)settings.End.dx - (float)settings.Start.dx;
        var dy = (double)(float)settings.End.dy - (float)settings.Start.dy;
        var length = Math.Sqrt(dx * dx + dy * dy);
        var ux = dx / length;
        var uy = dy / length;
        var rampX = (ux * inverse.ScaleX + uy * inverse.SkewY) / length;
        var rampY = (ux * inverse.SkewX + uy * inverse.ScaleY) / length;
        var rampZ = (ux * (inverse.TransX - (float)settings.Start.dx)
            + uy * (inverse.TransY - (float)settings.Start.dy)) / length;
        var alignment = _filterCaptureAlignment;
        SKRectI Round(SKRect rect) => new(
            (int)Math.Max(0, Math.Floor(rect.Left / alignment) * alignment),
            (int)Math.Max(0, Math.Floor(rect.Top / alignment) * alignment),
            (int)Math.Min(input.Width, Math.Ceiling(rect.Right / alignment) * alignment),
            (int)Math.Min(input.Height, Math.Ceiling(rect.Bottom / alignment) * alignment));
        var outputRect = Round(bounds);
        var radius = 3 * Math.Max(settings.StartSigma, settings.EndSigma);
        var haloX = radius * Math.Abs(-matrix.ScaleX * uy + matrix.SkewX * ux) + 1;
        var haloY = radius * Math.Abs(-matrix.SkewY * uy + matrix.ScaleY * ux) + 1;
        var firstRect = Round(new SKRect((float)(outputRect.Left - haloX),
            (float)(outputRect.Top - haloY), (float)(outputRect.Right + haloX),
            (float)(outputRect.Bottom + haloY)));
        SKImage Pass(SKImage source, SKPoint sourceOrigin, SKRectI rect, double ax, double ay)
        {
            var values = new double[] { rampX, rampY, rampZ, settings.StartSigma, settings.EndSigma,
                matrix.ScaleX * ax + matrix.SkewX * ay, matrix.SkewY * ax + matrix.ScaleY * ay,
                settings.MaxSamples, settings.Kernel == VariableBlurKernel.fastGaussian ? 1 : 0 };
            if (values.Any(v => !float.IsFinite((float)v))
                || !float.IsFinite((float)(radius * Math.Sqrt(values[5] * values[5] + values[6] * values[6]))))
                throw new NotSupportedException("Variable blur exceeds shader coordinate precision.");
            var fragment = VariableBlurProgram.Value.fragmentShader();
            for (var j = 0; j < values.Length; j++) fragment.setFloat(j + 2, values[j]);
            using var shader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                (FragmentShaderSnapshot)ShaderSnapshot.Capture(fragment), source,
                new SKSamplingOptions(SKFilterMode.Linear), CreateImageShader,
                RuntimeEffectBackend, _contextGeneration, _runtimeEffectContextOwner,
                tileMode == TileMode.decal ? SKShaderTileMode.Decal : SKShaderTileMode.Clamp,
                inputLocalMatrix: SKMatrix.CreateTranslation(sourceOrigin.X, sourceOrigin.Y));
            using var local = shader.WithLocalMatrix(SKMatrix.CreateTranslation(-rect.Left, -rect.Top));
            using var output = CreateFilterSurface(target, rect.Width, rect.Height);
            using var paint = new SKPaint { Shader = local, BlendMode = SKBlendMode.Src };
            RecordVariableBlurWork("gaussian-pass", source.Width, source.Height, rect.Width, rect.Height,
                SKRect.Create(rect.Width, rect.Height), $"working-origin={rect.Left},{rect.Top}");
            var started = StartVariableBlurStage();
            output.Canvas.DrawRect(SKRect.Create(rect.Width, rect.Height), paint);
            EndVariableBlurStage("gaussian-region-draw", started);
            Interlocked.Increment(ref _shaderImageFiltersRendered);
            return output.Snapshot();
        }
        using var first = Pass(input, new(0, 0), firstRect, ux, uy);
        origin = new(outputRect.Left, outputRect.Top);
        return Pass(first, new(firstRect.Left, firstRect.Top), outputRect, -uy, ux);
    }
}

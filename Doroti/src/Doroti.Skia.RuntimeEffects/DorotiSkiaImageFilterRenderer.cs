using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.RuntimeEffects;

internal static class DorotiSkiaImageFilterRenderer
{
    internal static bool Draw(
        SKCanvas target,
        int pixelWidth,
        int pixelHeight,
        FragmentShaderSnapshot shader,
        SKRect childBounds,
        SKPoint childOffset,
        SKSamplingOptions inputSampling,
        Func<Image, SKShader> imageShaderFactory,
        Action<SKCanvas, int, int> drawChild,
        string backend,
        long contextGeneration)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(shader);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        ArgumentNullException.ThrowIfNull(drawChild);
        if (pixelWidth <= 0 || pixelHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelWidth), "The GPU filter target must have positive dimensions.");
        if (target.Context is not { } context)
            throw new NotSupportedException(
                "Doroti ImageFilter.shader requires the active Skia GPU recording context; software capture is forbidden.");

        var offsetBounds = new SKRect(
            childBounds.Left + childOffset.X,
            childBounds.Top + childOffset.Y,
            childBounds.Right + childOffset.X,
            childBounds.Bottom + childOffset.Y);
        var mappedBounds = target.TotalMatrix.MapRect(offsetBounds);
        var clip = target.DeviceClipBounds;
        var left = Math.Max(0, Math.Max(clip.Left, (int)Math.Floor(mappedBounds.Left)));
        var top = Math.Max(0, Math.Max(clip.Top, (int)Math.Floor(mappedBounds.Top)));
        var right = Math.Min(pixelWidth, Math.Min(clip.Right, (int)Math.Ceiling(mappedBounds.Right)));
        var bottom = Math.Min(pixelHeight, Math.Min(clip.Bottom, (int)Math.Ceiling(mappedBounds.Bottom)));
        if (right <= left || bottom <= top) return false;

        var width = checked(right - left);
        var height = checked(bottom - top);
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var inputSurface = SKSurface.Create(context, true, info)
            ?? throw new InvalidOperationException(
                $"Doroti ImageFilter.shader could not allocate a {width}x{height} GPU input surface.");
        var inputCanvas = inputSurface.Canvas;
        inputCanvas.Clear(SKColors.Transparent);
        inputCanvas.Translate(-left, -top);
        var parentMatrix = target.TotalMatrix;
        inputCanvas.Concat(in parentMatrix);
        inputCanvas.Translate(childOffset.X, childOffset.Y);
        drawChild(inputCanvas, width, height);
        inputCanvas.Flush();

        using var inputImage = inputSurface.Snapshot()
            ?? throw new InvalidOperationException("Doroti ImageFilter.shader could not snapshot its GPU input surface.");
        using var runtimeShader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
            shader,
            inputImage,
            inputSampling,
            imageShaderFactory,
            backend,
            contextGeneration);
        using var paint = new SKPaint { Shader = runtimeShader, BlendMode = SKBlendMode.SrcOver };
        target.Save();
        target.ResetMatrix();
        target.Translate(left, top);
        target.DrawRect(SKRect.Create(width, height), paint);
        target.Restore();
        return true;
    }
}

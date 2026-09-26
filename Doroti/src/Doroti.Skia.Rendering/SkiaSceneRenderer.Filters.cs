using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private sealed record FilterCanvasState(Action<SKCanvas> Apply, bool IsClip = false);

    private static bool ContainsShader(ImageFilterSnapshot filter) =>
        filter.Shader is not null
        || (filter.Inner is not null && ContainsShader(filter.Inner))
        || (filter.Outer is not null && ContainsShader(filter.Outer));

    private static bool RequiresGpuFilterLayers(
        IReadOnlyList<SceneCommand> commands,
        int start,
        int end
    )
    {
        for (var i = start; i < end; i++)
        {
            if (
                commands[i].HostPayload is SceneBackdropFilterPayload backdrop
                    && ContainsShader(backdrop.Filter)
                || commands[i].HostPayload is SceneImageFilterPayload image
                    && image.Filter.Shader is null
                    && ContainsShader(image.Filter)
                || commands[i].HostPayload is SceneRetainedPayload retained
                    && RequiresGpuFilterLayers(retained.Commands, 0, retained.Commands.Count)
            )
                return true;
        }
        return false;
    }

    // Native saveLayer surfaces are private to Skia. For scenes that sample a shader
    // backdrop, own the intermediate GPU surfaces so snapshots always represent the
    // CURRENT layer, including inside opacity/color/shader-mask layers, never the root.
    // Ordinary scenes keep their existing native saveLayer and cache paths.
    private void DrawGpuFilterScene(
        SKCanvas target,
        IReadOnlyList<SceneCommand> commands,
        int start,
        int end,
        int width,
        int height,
        List<FilterCanvasState>? inheritedState = null
    )
    {
        if (!SkiaGpuSurfaces.IsGpu(target))
            throw new NotSupportedException(
                "Shader image and backdrop filters require an owned Skia GPU surface."
            );

        var matrix = target.TotalMatrix;
        var clip = target.DeviceClipBounds;
        var state =
            inheritedState
            ?? new List<FilterCanvasState>
            {
                new(c => c.ClipRect(clip), true),
                new(c => c.SetMatrix(matrix)),
            };
        for (var i = start; i < end; i++)
        {
            var command = commands[i];
            if (command.HostPayload is ScenePicturePayload picture)
            {
                target.Save();
                try
                {
                    target.Translate((float)picture.Offset.dx, (float)picture.Offset.dy);
                    DrawPictureLayer(target, picture);
                }
                finally
                {
                    target.Restore();
                }
                continue;
            }
            if (command.HostPayload is SceneTexturePayload texture)
            {
                DrawTexture(target, texture);
                continue;
            }
            if (command.HostPayload is SceneRetainedPayload retained)
            {
                DrawGpuFilterScene(
                    target,
                    retained.Commands,
                    0,
                    retained.Commands.Count,
                    width,
                    height,
                    state
                );
                continue;
            }
            if (!IsSceneScopeStart(command.Operation))
                throw new NotSupportedException(
                    $"Unsupported GPU filter scene operation '{command.Operation}'."
                );

            var pop = FindMatchingPop(commands, i, end);
            var childStart = i + 1;
            Action<SKCanvas>? change = command.HostPayload switch
            {
                SceneOffsetPayload offset => c => c.Translate((float)offset.Dx, (float)offset.Dy),
                SceneTransformPayload transform => c => Concat(c, transform.Matrix4),
                SceneClipRectPayload rect => c =>
                {
                    if (rect.Behavior != Clip.none)
                        c.ClipRect(
                            ToRect(rect.Rect),
                            SKClipOperation.Intersect,
                            rect.Behavior != Clip.hardEdge
                        );
                },
                SceneClipRRectPayload rounded => c =>
                {
                    using var path = ToPath(rounded.RRect);
                    c.ClipPath(path, SKClipOperation.Intersect, true);
                },
                SceneClipRSuperellipsePayload rounded => c =>
                {
                    using var path = SkiaRSuperellipsePath.Create(rounded.RSuperellipse);
                    c.ClipPath(path, SKClipOperation.Intersect, true);
                },
                SceneClipPathPayload pathClip => c =>
                {
                    using var path = ToPath(pathClip.Path);
                    c.ClipPath(path, SKClipOperation.Intersect, true);
                },
                _ => null,
            };
            if (change is not null)
            {
                target.Save();
                state.Add(
                    new(change, command.Operation.StartsWith("clip", StringComparison.Ordinal))
                );
                try
                {
                    change(target);
                    DrawGpuFilterScene(target, commands, childStart, pop, width, height, state);
                }
                finally
                {
                    state.RemoveAt(state.Count - 1);
                    target.Restore();
                }
            }
            else
            {
                DrawGpuFilterLayer(
                    target,
                    command,
                    commands,
                    childStart,
                    pop,
                    width,
                    height,
                    state
                );
            }
            i = pop;
        }
    }

    private SKSurface CreateFilterSurface(SKCanvas target, int width, int height)
    {
        using var properties = target.Surface?.SurfaceProperties;
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var surface = SkiaGpuSurfaces.CreateCompatible(target, info, properties);
        surface.Canvas.Clear(SKColors.Transparent);
        return surface;
    }

    private void DrawGpuFilterLayer(
        SKCanvas target,
        SceneCommand command,
        IReadOnlyList<SceneCommand> commands,
        int start,
        int end,
        int width,
        int height,
        List<FilterCanvasState> state
    )
    {
        using var layer = CreateFilterSurface(target, width, height);
        var canvas = layer.Canvas;
        // Image filters may read beyond the output clip (blur/morphology halos).
        // Apply ancestor clips when compositing the result, not to the input.
        var layerState =
            command.HostPayload is SceneImageFilterPayload
                ? state.Where(s => !s.IsClip).ToList()
                : new List<FilterCanvasState>(state);
        using var paint = new SKPaint();
        using var color = command.HostPayload is SceneColorFilterPayload cf
            ? ToColorFilter(cf.Filter)
            : null;
        paint.ColorFilter = color;
        if (command.HostPayload is SceneOpacityPayload opacity)
        {
            paint.Color = SKColors.White.WithAlpha(
                (byte)Math.Clamp(Math.Round(opacity.Opacity * 255), 0, 255)
            );
            layerState.Add(
                new(c => c.Translate((float)opacity.Offset.dx, (float)opacity.Offset.dy))
            );
        }
        if (command.HostPayload is SceneImageFilterPayload image)
            layerState.Add(new(c => c.Translate((float)image.Offset.dx, (float)image.Offset.dy)));
        if (command.HostPayload is SceneBackdropFilterPayload backdrop)
        {
            paint.BlendMode = ToBlend(backdrop.BlendMode);
            if (backdrop.Filter.Bounds is { } bounds)
                layerState.Add(
                    new(c => c.ClipRect(ToRect(bounds), SKClipOperation.Intersect, true), true)
                );
            using var input = target.Surface!.Snapshot();
            using var filtered = ApplyGpuImageFilter(
                target,
                input,
                backdrop.Filter,
                width,
                height,
                true
            );
            canvas.DrawImage(filtered, 0, 0, SKSamplingOptions.Default);
        }
        foreach (var apply in layerState)
            apply.Apply(canvas);
        DrawGpuFilterScene(canvas, commands, start, end, width, height, layerState);
        if (command.HostPayload is SceneShaderMaskPayload mask)
        {
            using var shader = ToShader(mask.Shader);
            using var maskPaint = new SKPaint
            {
                Shader = shader,
                BlendMode = ToBlend(mask.BlendMode),
                IsAntialias = true,
            };
            canvas.DrawRect(ToRect(mask.MaskRect), maskPaint);
        }
        using var snapshot = layer.Snapshot();
        using var output = command.HostPayload is SceneImageFilterPayload imageFilter
            ? ApplyGpuImageFilter(target, snapshot, imageFilter.Filter, width, height)
            : null;
        target.Save();
        try
        {
            if (
                command.HostPayload is SceneBackdropFilterPayload b
                && b.Filter.Bounds is { } bounds
            )
                target.ClipRect(ToRect(bounds), SKClipOperation.Intersect, true);
            target.ResetMatrix();
            target.DrawImage(output ?? snapshot, 0, 0, SKSamplingOptions.Default, paint);
        }
        finally
        {
            target.Restore();
        }
    }

    // Each stage keeps its GPU input alive until the output snapshot owns the draw.
    // Compose is deliberately evaluated inner -> outer, including nested shaders.
    private SKImage ApplyGpuImageFilter(
        SKCanvas target,
        SKImage input,
        ImageFilterSnapshot filter,
        int width,
        int height,
        bool isBackdrop = false
    )
    {
        if (filter.Outer is not null && filter.Inner is not null)
        {
            using var inner = ApplyGpuImageFilter(
                target,
                input,
                filter.Inner,
                width,
                height,
                isBackdrop
            );
            return ApplyGpuImageFilter(target, inner, filter.Outer, width, height, isBackdrop);
        }
        if (filter.ColorFilter is not null && filter.Inner is not null)
        {
            using var inner = ApplyGpuImageFilter(
                target,
                input,
                filter.Inner,
                width,
                height,
                isBackdrop
            );
            return ApplyGpuImageFilter(
                target,
                inner,
                filter with
                {
                    Inner = null,
                },
                width,
                height,
                isBackdrop
            );
        }
        using var surface = CreateFilterSurface(target, width, height);
        var canvas = surface.Canvas;
        if (filter.Shader is FragmentShaderSnapshot fragment)
        {
            using var shader = DorotiSkiaRuntimeEffects.CreateImageFilterShader(
                fragment,
                input,
                ToSamplingOptions(filter.FilterQuality),
                CreateImageShader,
                RuntimeEffectBackend,
                _contextGeneration,
                _runtimeEffectContextOwner
            );
            using var paint = new SKPaint { Shader = shader };
            canvas.DrawRect(SKRect.Create(width, height), paint);
            Interlocked.Increment(ref _shaderImageFiltersRendered);
        }
        else
        {
            // Evaluate native filters in the caller's coordinate system, while the
            // captured input remains in device pixels (scale/rotation affect radii).
            var matrix = target.TotalMatrix;
            if (isBackdrop)
            {
                canvas.DrawImage(input, 0, 0, SKSamplingOptions.Default);
                canvas.SetMatrix(matrix);
                using var replace = new SKPaint { BlendMode = SKBlendMode.Src };
                canvas.SaveLayer(
                    new SKCanvasSaveLayerRec { Backdrop = GetImageFilter(filter), Paint = replace }
                );
                canvas.Restore();
                return surface.Snapshot();
            }
            canvas.SetMatrix(matrix);
            using var paint = FilterPaint(filter);
            canvas.SaveLayer(paint);
            canvas.ResetMatrix();
            canvas.DrawImage(input, 0, 0, SKSamplingOptions.Default);
            canvas.Restore();
        }
        return surface.Snapshot();
    }
}

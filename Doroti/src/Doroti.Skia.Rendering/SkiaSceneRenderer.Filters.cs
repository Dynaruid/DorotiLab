using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private readonly bool _nativeVariableBlurSubtrees =
        Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_OWNED_SUBTREES") != "1";
    private sealed record FilterCanvasState(Action<SKCanvas> Apply, bool IsClip = false);

    private static bool ContainsShader(ImageFilterSnapshot filter) =>
        filter.Shader is not null
        || filter.VariableBlur is not null
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
                commands[i].HostPayload is SceneGpuEffectPayload
                || commands[i].HostPayload is SceneBackdropFilterPayload backdrop
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
                    var pictureStarted = StartVariableBlurStage();
                    DrawPictureLayer(target, picture);
                    EndVariableBlurStage("picture-replay", pictureStarted);
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
                if (_nativeVariableBlurSubtrees
                    && !RequiresGpuFilterLayers(retained.Commands, 0, retained.Commands.Count))
                {
                    DrawScene(target, retained.Commands, width, height);
                    continue;
                }
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
            // Ordinary backdrop layers can inherit alpha even in a scene with
            // shader siblings. Keep the same composite in both replay paths.
            if (command.HostPayload is SceneOpacityPayload
                && CanInheritBackdropOpacity(commands, childStart, pop))
            {
                DrawScene(target, commands, i, pop + 1, width, height);
                i = pop;
                continue;
            }
            // Ownership is needed for ancestors of a shader capture. Sibling
            // subtrees without one can keep native saveLayer/filter/cache paths;
            // promoting them too creates unrelated full-frame copies and clears.
            if (_nativeVariableBlurSubtrees && !RequiresGpuFilterLayers(commands, i, pop + 1))
            {
                DrawScene(target, commands, i, pop + 1, width, height);
                i = pop;
                continue;
            }
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

    private DorotiSkiaImageFilterRenderer.SceneSurfaceLease CreateFilterSurface(
        SKCanvas target,
        int width,
        int height
    )
    {
        var stageStarted = StartVariableBlurStage();
        using var properties = target.Surface?.SurfaceProperties;
        var surface = DorotiSkiaImageFilterRenderer.RentSceneSurface(
            target,
            RuntimeEffectBackend,
            _contextGeneration,
            width,
            height,
            _runtimeEffectContextOwner,
            properties
        );
        surface.Canvas.Clear(SKColors.Transparent);
        RecordVariableBlurWork(
            "scene-surface-clear",
            width,
            height,
            width,
            height,
            SKRect.Create(width, height),
            surface.IsTemporary ? "temporary"
                : surface.IsReused ? "pool-hit"
                : "pool-miss"
        );
        EndVariableBlurStage("surface-create-clear", stageStarted);
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
        if (command.HostPayload is SceneGpuEffectPayload gpu)
        {
            var backend =
                SkiaGraphiteSession.CurrentRecording?.GpuEffects
                ?? SkiaGpuEffectScope.Current
                ?? throw new PlatformNotSupportedException(
                    $"GPU effect '{gpu.Program.AssetId}' is unsupported by this host."
                );
            using var backdropInput = gpu.IsBackdrop
                ? target.Surface?.Snapshot()
                    ?? throw new NotSupportedException(
                        "A GPU backdrop requires the current owned Skia layer."
                    )
                : null;
            target.Save();
            try
            {
                target.Translate((float)gpu.Offset.dx, (float)gpu.Offset.dy);
                var matrix = target.TotalMatrix;
                var mapped = matrix.MapRect(ToRect(gpu.Bounds));
                if (
                    !float.IsFinite(mapped.Left)
                    || !float.IsFinite(mapped.Top)
                    || !float.IsFinite(mapped.Right)
                    || !float.IsFinite(mapped.Bottom)
                )
                    throw new InvalidOperationException(
                        "GPU effect capture bounds are not finite."
                    );
                var left = MathF.Floor(mapped.Left);
                var top = MathF.Floor(mapped.Top);
                var captureWidth = checked((int)(MathF.Ceiling(mapped.Right) - left));
                var captureHeight = checked((int)(MathF.Ceiling(mapped.Bottom) - top));
                if (captureWidth <= 0 || captureHeight <= 0)
                    return;
                if (backend.AvailableCaptureBytes > 0)
                {
                    var admission = EffectAllocationPreflight.Evaluate(
                        Rect.fromLTWH(0, 0, captureWidth, captureHeight), PlatformViewTransform.Identity,
                        1, 1, null, 4, 2, backend.AvailableCaptureBytes);
                    if (!admission.Supported) throw new NotSupportedException(admission.Reason);
                }
                var captureMatrix = SKMatrix.Concat(
                    SKMatrix.CreateTranslation(-left, -top),
                    matrix
                );
                var captureState = new List<FilterCanvasState>
                {
                    new(c => c.SetMatrix(captureMatrix)),
                };
                target.ResetMatrix();
                target.Translate(left, top);
                backend.Draw(
                    target,
                    captureWidth,
                    captureHeight,
                    input =>
                    {
                        input.Clear(SKColors.Transparent);
                        if (backdropInput is not null)
                            input.DrawImage(
                                backdropInput,
                                -left,
                                -top,
                                new SKSamplingOptions(SKFilterMode.Nearest)
                            );
                        else
                        {
                            foreach (var apply in captureState)
                                apply.Apply(input);
                            DrawGpuFilterScene(
                                input,
                                commands,
                                start,
                                end,
                                captureWidth,
                                captureHeight,
                                captureState
                            );
                        }
                    },
                    gpu.Program,
                    gpu.Parameters,
                    (float)gpu.Bounds.width,
                    (float)gpu.Bounds.height
                );
            }
            finally
            {
                target.Restore();
            }
            if (gpu.IsBackdrop)
            {
                target.Save();
                try
                {
                    target.Translate((float)gpu.Offset.dx, (float)gpu.Offset.dy);
                    var childState = new List<FilterCanvasState>(state)
                    {
                        new(c => c.Translate((float)gpu.Offset.dx, (float)gpu.Offset.dy)),
                    };
                    DrawGpuFilterScene(target, commands, start, end, width, height, childState);
                }
                finally
                {
                    target.Restore();
                }
            }
            return;
        }
        // An empty backdrop child needs no extra full-frame layer, copy or
        // snapshot. Preserve the final clip/blend directly on the target.
        if (
            start == end
            && command.HostPayload is SceneBackdropFilterPayload direct
            && direct.Filter.VariableBlur is { } variable
        )
        {
            var visible = VariableBlurOutputBounds(target, direct.Filter);
            if (visible.IsEmpty)
                return;
            var capture = VariableBlurCaptureBounds(
                visible,
                variable,
                direct.Filter.TileMode,
                target.TotalMatrix,
                width,
                height,
                out var captureReason,
                _filterCaptureAlignment
            );
            RecordVariableBlurWork(
                "backdrop-capture",
                width,
                height,
                capture.Width,
                capture.Height,
                capture,
                captureReason
            );
            var captureStarted = StartVariableBlurStage();
            using var input = target.Surface!.Snapshot(capture);
            EndVariableBlurStage("backdrop-capture", captureStarted);
            var localVisible = visible;
            localVisible.Offset(-capture.Left, -capture.Top);
            var localMatrix = SKMatrix.Concat(
                SKMatrix.CreateTranslation(-capture.Left, -capture.Top),
                target.TotalMatrix
            );
            var filterStarted = StartVariableBlurStage();
            using var adaptiveShader = variable.Kernel == VariableBlurKernel.dualKawase
                && VariableBlurKawaseGeometry(variable, direct.Filter.TileMode, localMatrix,
                    out var kawaseScale, out var kawaseDepth, out _)
                ? CreateDualKawaseVariableBlurShader(target, input, variable, capture.Width,
                    capture.Height, localMatrix, localVisible, kawaseScale, kawaseDepth)
                : variable.AdaptiveResolution && variable.ResolutionScale < 1
                    && variable.Kernel != VariableBlurKernel.dualKawase
                    && Environment.GetEnvironmentVariable("DOROTI_VARIABLE_BLUR_INTERMEDIATE") != "1"
                    && localMatrix.TryInvert(out var localInverse)
                    ? CreateAdaptiveVariableBlurShader(target, input, variable, direct.Filter.TileMode,
                        capture.Width, capture.Height, localMatrix, localInverse, localVisible)
                    : null;
            using var filtered = adaptiveShader is null ? ApplyVariableBlur(
                target,
                input,
                variable,
                direct.Filter.TileMode,
                capture.Width,
                capture.Height,
                localMatrix,
                localVisible,
                keepWorkingResolution: true
            ) : null;
            EndVariableBlurStage("filter-total", filterStarted);
            using var blend = new SKPaint { BlendMode = ToBlend(direct.BlendMode) };
            target.Save();
            try
            {
                if (direct.Filter.Bounds is { } bounds)
                    target.ClipRect(ToRect(bounds), SKClipOperation.Intersect, true);
                target.ResetMatrix();
                RecordVariableBlurWork("final-composite", filtered?.Width ?? capture.Width,
                    filtered?.Height ?? capture.Height, width, height, visible,
                    adaptiveShader is null ? "direct-backdrop"
                        : variable.Kernel == VariableBlurKernel.dualKawase
                            ? "direct-kawase-shader" : "direct-adaptive-shader");
                if (adaptiveShader is not null)
                {
                    using var translated = adaptiveShader.WithLocalMatrix(
                        SKMatrix.CreateTranslation(capture.Left, capture.Top));
                    blend.Shader = translated;
                    target.DrawRect(capture, blend);
                }
                else
                    target.DrawImage(filtered!, (SKRect)capture,
                        new SKSamplingOptions(SKFilterMode.Linear), blend);

            }
            finally
            {
                target.Restore();
            }
            return;
        }
        using var layer = CreateFilterSurface(target, width, height);
        RecordVariableBlurWork("owned-scene-layer", width, height, width, height,
            target.DeviceClipBounds, command.Operation);
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
            // A retained host surface can be larger than its visible viewport.
            // Exclude that spare capacity before resampling (or wrapping tiles),
            // otherwise fast blur scales the entire backing into the viewport.
            using var input = target.Surface!.Snapshot(new SKRectI(0, 0, width, height));
            SKRect? variableOutputBounds = null;
            if (backdrop.Filter.VariableBlur is not null)
            {
                variableOutputBounds = VariableBlurOutputBounds(target, backdrop.Filter);
                RecordVariableBlurWork(
                    "backdrop-capture",
                    width,
                    height,
                    width,
                    height,
                    SKRect.Create(width, height),
                    "backdrop-with-child"
                );
            }
            using var filtered = ApplyGpuImageFilter(
                target,
                input,
                backdrop.Filter,
                width,
                height,
                true,
                variableOutputBounds: variableOutputBounds
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
            ? ApplyGpuImageFilter(
                target,
                snapshot,
                imageFilter.Filter,
                width,
                height,
                filterMatrix: canvas.TotalMatrix
            )
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

    private static SKRect VariableBlurOutputBounds(SKCanvas target, ImageFilterSnapshot filter)
    {
        SKRect visible = target.DeviceClipBounds;
        if (filter.Bounds is { } bounds)
            visible.Intersect(target.TotalMatrix.MapRect(ToRect(bounds)));
        return visible;
    }

    // Each stage keeps its GPU input alive until the output snapshot owns the draw.
    // Compose is deliberately evaluated inner -> outer, including nested shaders.
    private SKImage ApplyGpuImageFilter(
        SKCanvas target,
        SKImage input,
        ImageFilterSnapshot filter,
        int width,
        int height,
        bool isBackdrop = false,
        SKMatrix? filterMatrix = null,
        SKRect? variableOutputBounds = null
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
                isBackdrop,
                filterMatrix
            );
            return ApplyGpuImageFilter(
                target,
                inner,
                filter.Outer,
                width,
                height,
                isBackdrop,
                filterMatrix
            );
        }
        if (filter.ColorFilter is not null && filter.Inner is not null)
        {
            using var inner = ApplyGpuImageFilter(
                target,
                input,
                filter.Inner,
                width,
                height,
                isBackdrop,
                filterMatrix
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
                isBackdrop,
                filterMatrix
            );
        }
        if (filter.VariableBlur is { } variable)
            return ApplyVariableBlur(
                target,
                input,
                variable,
                filter.TileMode,
                width,
                height,
                filterMatrix ?? target.TotalMatrix,
                variableOutputBounds
            );
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

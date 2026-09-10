using System.Reflection;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

internal static class GpuCompositingContracts
{
    private const string Backend = DorotiSkiaRuntimeEffects.ValidationBackend;
    private const long Generation = 901;
    internal static void Verify(string contract)
    {
        if (contract is not ("blend" or "phase" or "eviction" or "exception" or "owners" or "huge" or "moving"))
            throw new ArgumentException("Unknown GPU compositing contract", nameof(contract));
        using var gpu = new GpuRasterFixture();
        using var target = SKSurface.Create(gpu.Context, true, new SKImageInfo(128, 128));
        using var reference = SKSurface.Create(gpu.Context, true, new SKImageInfo(128, 128));
        try
        {
            if (contract == "blend") VerifyBlends(target, reference);
            else if (contract == "owners") VerifyOwners(target);
            else if (contract == "moving") VerifyMovingFilter(target, reference);
            else VerifyFilters(contract, target, reference);
            Console.WriteLine($"GPU compositing {contract} PASS");
        }
        finally
        {
            DorotiSkiaImageFilterRenderer.ReleaseContext(Backend, Generation);
            DorotiSkiaRuntimeEffects.ReleaseContext(Backend, Generation);
        }
    }

    private static void VerifyMovingFilter(SKSurface target, SKSurface reference)
    {
        var shader = FragmentProgram.fromSource("uniform float2 size;\nuniform shader child;\nhalf4 main(float2 p) { return child.eval(p) * half4(0.5, 0.75, 1, 1); }", "moving-cache").fragmentShader();
        var snapshot = new FragmentShaderSnapshot(shader.CaptureState());
        var movingKey = new object();
        long movingAllocations = 0;
        using var paint = new SKPaint { Color = new SKColor(80, 160, 240, 128), IsAntialias = true };
        void Draw(SKSurface surface, object key, out bool hit) =>
            DorotiSkiaImageFilterRenderer.Draw(surface.Canvas, 128, 128, snapshot,
                SKRect.Create(0, 0, 64, 64), default, SKSamplingOptions.Default,
                _ => throw new Exception("Unexpected sampler"),
                (canvas, _, _) => canvas.DrawCircle(17.25f, 18.375f, 8.25f, paint),
                Backend, Generation, key, 0, out hit);
        for (var frame = 0; frame < 20; frame++)
        {
            DorotiSkiaImageFilterRenderer.BeginFrame(Backend, Generation);
            foreach (var surface in new[] { target, reference })
            {
                surface.Canvas.RestoreToCount(1);
                surface.Canvas.ResetMatrix();
                surface.Canvas.Clear(new SKColor(160, 70, 90, 220));
                surface.Canvas.Save();
                surface.Canvas.ClipRect(SKRect.Create(4, 8, 70, 65));
                surface.Canvas.Translate(-10.375f + Math.Min(frame, 15) * 0.125f, 12.625f);
            }
            var before = DorotiSkiaImageFilterRenderer.Diagnostics.Created;
            Draw(target, movingKey, out var hit);
            if (frame < 16) movingAllocations += DorotiSkiaImageFilterRenderer.Diagnostics.Created - before;
            // A new key takes the full cached-output path for comparison.
            Draw(reference, new object(), out _);
            EqualPixels(target, reference, $"moving/clipped translucent filter frame={frame}");
            if (frame == 19 && !hit) throw new Exception("Settled filter did not resume cache reuse");
        }
        if (movingAllocations >= 16) throw new Exception("Moving filter still allocates an output surface every frame");
        target.Canvas.RestoreToCount(1); reference.Canvas.RestoreToCount(1);
        shader.dispose();
    }

    private static void VerifyBlends(SKSurface target, SKSurface reference)
    {
        using var environment = new ImageFixtureEnvironment();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var begin = typeof(SkiaSceneRenderer).GetMethod("BeginPictureRasterFrame", flags)!;
        var cachedDraw = typeof(SkiaSceneRenderer).GetMethod("DrawPictureLayer", flags)!;
        var directDraw = typeof(SkiaSceneRenderer).GetMethod("DrawPicture", flags)!;
        foreach (var mode in Enum.GetValues<BlendMode>())
        {
            var recorder = new PictureRecorder();
            new Canvas(recorder).drawRect(Rect.fromLTWH(8, 8, 48, 48), new Paint { color = new Color(0x8040a0e0), blendMode = mode });
            using var picture = recorder.endRecording();
            var builder = new SceneBuilder(91);
            builder.addPicture(Offset.zero, picture, Rect.fromLTWH(0, 0, 64, 64), isComplexHint: true);
            using var scene = builder.build();
            var payload = scene.Commands.Single().HostPayload!;
            for (var frame = 0; frame < 3; frame++)
            {
                begin.Invoke(environment.Renderer, null);
                foreach (var surface in new[] { target, reference }) surface.Canvas.Clear(new SKColor(160, 70, 90, 220));
                cachedDraw.Invoke(environment.Renderer, [target.Canvas, payload]);
                directDraw.Invoke(environment.Renderer, [reference.Canvas, picture.Commands]);
                EqualPixels(target, reference, $"blend={mode}, frame={frame}");
            }
        }
    }

    private static void VerifyFilters(string contract, SKSurface target, SKSurface reference)
    {
        var shader = FragmentProgram.fromSource("uniform float2 size;\nuniform shader child;\nhalf4 main(float2 p) { return child.eval(p); }", "cache-identity").fragmentShader();
        var snapshot = new FragmentShaderSnapshot(shader.CaptureState());
        using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        Action<SKCanvas, int, int> child = (canvas, _, _) => canvas.DrawCircle(17.25f, 18.375f, 8.25f, paint);
        var keys = Enumerable.Range(0, 40).Select(_ => new object()).ToArray();
        bool Draw(SKSurface surface, object? key, Action<SKCanvas, int, int>? drawChild = null)
        {
            return DorotiSkiaImageFilterRenderer.Draw(surface.Canvas, 128, 128, snapshot,
                contract == "huge" ? new SKRect(-1e20f, -1e20f, 1e20f, 1e20f) : SKRect.Create(0, 0, 64, 64), default, SKSamplingOptions.Default,
                _ => throw new Exception("unexpected external sampler"), drawChild ?? child,
                Backend, Generation, key, 0, out _);
        }
        if (contract == "exception")
        {
            target.Canvas.Translate(10.375f, 12.625f);
            DorotiSkiaImageFilterRenderer.BeginFrame(Backend, Generation);
            try { Draw(target, null, (canvas, _, _) => { canvas.Save(); canvas.Translate(31, 29); throw new InvalidOperationException("fixture child failure"); }); }
            catch (InvalidOperationException error) when (error.Message == "fixture child failure") { }
        }
        var frames = contract == "eviction" ? 3 : 8;
        for (var frame = 0; frame < frames; frame++)
        {
            DorotiSkiaImageFilterRenderer.BeginFrame(Backend, Generation);
            foreach (var surface in new[] { target, reference })
            {
                surface.Canvas.ResetMatrix();
                surface.Canvas.Clear(SKColors.White);
                surface.Canvas.Translate(contract == "eviction" ? 10 : 10.375f + frame * 0.25f, contract == "eviction" ? 12 : 12.625f);
            }
            if (contract == "eviction")
            {
                foreach (var key in keys)
                {
                    target.Canvas.Clear(SKColors.White);
                    Draw(target, key);
                    reference.Canvas.Clear(SKColors.White);
                    child(reference.Canvas, 128, 128);
                    EqualPixels(target, reference, $"filter cache pressure frame={frame}");
                }
            }
            else
            {
                Draw(target, contract == "exception" ? null : keys[0]);
                child(reference.Canvas, 128, 128);
                EqualPixels(target, reference, $"filter {contract} frame={frame}");
            }
        }
    }

    private static void VerifyOwners(SKSurface firstTarget)
    {
        using var first = new ImageFixtureEnvironment(DorotiSkiaRuntimeEffects.WindowsHwndD3D12Backend);
        using var second = new ImageFixtureEnvironment(DorotiSkiaRuntimeEffects.WindowsHwndD3D12Backend);
        using var secondGpu = new GpuRasterFixture();
        using var secondTarget = SKSurface.Create(secondGpu.Context, true, new SKImageInfo(128, 128));
        var sharedKey = new object();
        var shader = FragmentProgram.fromSource("uniform float2 size;\nuniform shader child;\nhalf4 main(float2 p) { return child.eval(p); }", "owner-identity").fragmentShader();
        Scene Scene(Color color, object? cacheKey = null)
        {
            var recorder = new PictureRecorder();
            new Canvas(recorder).drawRect(Rect.fromLTWH(0, 0, 64, 64), new Paint { color = color });
            using var picture = recorder.endRecording();
            var builder = new SceneBuilder(91);
            builder.pushImageFilter(new ImageFilter(shader, FilterQuality.none), bounds: Rect.fromLTWH(0, 0, 64, 64), cacheKey: cacheKey ?? sharedKey);
            builder.addPicture(Offset.zero, picture);
            builder.pop();
            return builder.build();
        }
        using var redScene = Scene(new Color(0xffff0000));
        using var blueScene = Scene(new Color(0xff0000ff));
        var draw = typeof(SkiaSceneRenderer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(method => method.Name == "DrawScene" && method.GetParameters().Length == 4);
        void Render(ImageFixtureEnvironment environment, SKSurface surface, Scene scene, SKColor expected)
        {
            surface.Canvas.Clear(SKColors.White);
            draw.Invoke(environment.Renderer, [surface.Canvas, scene.Commands, 128, 128]);
            using var image = surface.Snapshot(); using var pixels = SKBitmap.FromImage(image);
            if (pixels.GetPixel(16, 16) != expected) throw new Exception("A renderer reused another GPU owner's filtered image");
        }
        Render(first, firstTarget, redScene, SKColors.Red);
        Render(second, secondTarget, blueScene, SKColors.Blue);
        var misses = DorotiSkiaImageFilterRenderer.Diagnostics.CacheMisses;
        var compilations = DorotiSkiaRuntimeEffects.CompiledEffectCountForValidation;
        second.Renderer.InvalidateGpuContextResources();
        Render(first, firstTarget, redScene, SKColors.Red);
        if (DorotiSkiaImageFilterRenderer.Diagnostics.CacheMisses != misses)
            throw new Exception("Invalidating one view discarded another view's filter cache");
        using var freshRedScene = Scene(new Color(0xffff0000), new object());
        Render(first, firstTarget, freshRedScene, SKColors.Red);
        if (DorotiSkiaRuntimeEffects.CompiledEffectCountForValidation != compilations)
            throw new Exception("Invalidating one view discarded another view's compiled shader");
        Render(second, secondTarget, blueScene, SKColors.Blue);
        misses = DorotiSkiaImageFilterRenderer.Diagnostics.CacheMisses;
        first.Renderer.Dispose();
        Render(second, secondTarget, blueScene, SKColors.Blue);
        if (DorotiSkiaImageFilterRenderer.Diagnostics.CacheMisses != misses)
            throw new Exception("Disposing one view discarded another view's filter cache");
        second.Renderer.Dispose();
        shader.dispose();
    }

    private static void EqualPixels(SKSurface actual, SKSurface expected, string stage)
    {
        actual.Canvas.Flush(); expected.Canvas.Flush();
        using var a = actual.Snapshot(); using var b = expected.Snapshot();
        using var ap = SKBitmap.FromImage(a); using var bp = SKBitmap.FromImage(b);
        if (!ap.Bytes.SequenceEqual(bp.Bytes)) throw new Exception(stage + ": cached/direct GPU pixels differ");
    }
}

using System.Reflection;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class RasterBudgetContracts
{
    internal static void Verify()
    {
        using var environment = new ImageFixtureEnvironment();
        using var gpu = new GpuRasterFixture();
        using var target = SKSurface.Create(gpu.Context, true, new SKImageInfo(64, 64));
        using var reference = SKSurface.Create(new SKImageInfo(64, 64));
        var renderer = environment.Renderer;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var begin = typeof(SkiaSceneRenderer).GetMethod("BeginPictureRasterFrame", flags)!;
        var draw = typeof(SkiaSceneRenderer).GetMethod("DrawPictureLayer", flags)!;
        var payloadProperty = typeof(SceneCommand).GetProperty("HostPayload", flags)!;
        object Payload(int index)
        {
            var recorder = new PictureRecorder();
            var canvas = new Canvas(recorder);
            canvas.drawRect(Rect.fromLTWH(index % 4 * 12, index % 4 * 12, 8, 8),
                new Paint { color = new Color(0xff008800 + index), isAntiAlias = false });
            using var picture = recorder.endRecording();
            var builder = new SceneBuilder(91);
            builder.addPicture(Offset.zero, picture, Rect.fromLTWH(0, 0, 64, 64), isComplexHint: true);
            using var scene = builder.build();
            return payloadProperty.GetValue(scene.Commands.Single())!;
        }
        var payloads = Enumerable.Range(0, 4).Select(Payload).ToArray();
        // Five frames exercise warm-up, deferral, promotion and translation reuse.
        for (var frame = 0; frame < 5; frame++)
        {
            begin.Invoke(renderer, null);
            var before = renderer.Diagnostics.PictureRasterCacheMisses;
            foreach (var surface in new[] { target, reference })
            {
                surface.Canvas.Clear(SKColors.White);
                surface.Canvas.ResetMatrix();
                surface.Canvas.Translate(frame, 0);
                foreach (var payload in payloads) draw.Invoke(renderer, [surface.Canvas, payload]);
                surface.Canvas.Flush();
            }
            gpu.Context.Flush();
            using var actual = target.Snapshot();
            using var expected = reference.Snapshot();
            using var actualPixels = SKBitmap.FromImage(actual);
            using var expectedPixels = SKBitmap.FromImage(expected);
            if (!actualPixels.Bytes.SequenceEqual(expectedPixels.Bytes)) throw new Exception("Deferred/cached picture pixel mismatch");
            if (renderer.Diagnostics.PictureRasterCacheMisses - before > 2) throw new Exception("Promotion frame budget exceeded");
        }
        if (renderer.Diagnostics.PictureRasterCacheHits == 0) throw new Exception("Translation did not reuse raster images");
        begin.Invoke(renderer, null);
        // Distinct one-use keys are workload entries, not repeated test cycles.
        for (var index = 0; index < 140; index++) draw.Invoke(renderer, [target.Canvas, Payload(index)]);
        if (renderer.Diagnostics.Work!.WarmupEntries > 128) throw new Exception("Warm-up metadata is unbounded");
        // Empty frame boundaries age metadata without replaying user actions.
        for (var frame = 0; frame < 121; frame++) begin.Invoke(renderer, null);
        if (renderer.Diagnostics.Work.WarmupEntries != 0) throw new Exception("One-use metadata did not expire");
        renderer.InvalidateGpuContextResources();
        if (renderer.Diagnostics.Work.WarmupEntries != 0 || renderer.Diagnostics.Work.RasterPixels != 0)
            throw new Exception("Context loss retained raster resources");
        Console.WriteLine("Raster budget, replay pixel parity, translation reuse, metadata bound and context invalidation PASS");
    }
}

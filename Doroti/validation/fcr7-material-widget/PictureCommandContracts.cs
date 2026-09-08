using System.Reflection;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class PictureCommandContracts
{
    internal static void Verify()
    {
        using var environment = new ImageFixtureEnvironment();
        using var gpu = new GpuRasterFixture();
        using var target = SKSurface.Create(gpu.Context, true, new SKImageInfo(256, 256));
        using var reference = SKSurface.Create(gpu.Context, true, new SKImageInfo(256, 256));
        var renderer = environment.Renderer;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var retained = typeof(SkiaSceneRenderer).GetMethod("DrawRetainedPicture", flags)!;
        var direct = typeof(SkiaSceneRenderer).GetMethod("DrawPicture", flags)!;
        object Payload(Picture picture, Rect bounds, bool changing = false)
        {
            var builder = new SceneBuilder(91);
            builder.addPicture(Offset.zero, picture, bounds, willChangeHint: changing);
            using var scene = builder.build();
            return scene.Commands.Single().HostPayload!;
        }
        Picture Make(int index, bool shadow = false)
        {
            var recorder = new PictureRecorder();
            var canvas = new Canvas(recorder);
            var path = new Doroti.Ui.Path();
            path.addOval(Rect.fromLTWH(4, 5, 48, 29));
            canvas.save();
            canvas.clipPath(path);
            canvas.drawRect(Rect.fromLTWH(0, 0, 100, 100), new Paint { color = new Color(0x8040a0e0 + index) });
            canvas.drawCircle(new Offset(15.25, 12.375), 10, new Paint { color = new Color(0xc0302040), blendMode = BlendMode.multiply });
            canvas.restore();
            canvas.drawRRect(RRect.fromRectAndRadius(Rect.fromLTWH(7, 42, 35, 20), Radius.circular(7)), new Paint { color = new Color(0xff102090) });
            if (shadow) canvas.drawShadow(path, new Color(0xff000000), 4, true);
            return recorder.endRecording();
        }
        using var picture = Make(0);
        var payload = Payload(picture, Rect.fromLTWH(0, 0, 90, 90));
        foreach (var scale in new[] { 1f, 1.25f, 1.5f, 2f })
        foreach (var translation in new[] { 10.125f, 10.375f, 11.625f })
        {
            foreach (var surface in new[] { target, reference })
            {
                surface.Canvas.ResetMatrix();
                surface.Canvas.Clear(new SKColor(120, 90, 70, 220));
                surface.Canvas.ClipRect(SKRect.Create(256, 256));
                surface.Canvas.Translate(translation, 9.375f);
                surface.Canvas.Scale(scale, scale);
                if (surface == target) retained.Invoke(renderer, [surface.Canvas, payload]);
                else direct.Invoke(renderer, [surface.Canvas, picture.Commands]);
                surface.Canvas.Flush();
            }
            using var actual = target.Snapshot();
            using var expected = reference.Snapshot();
            using var actualPixels = SKBitmap.FromImage(actual);
            using var expectedPixels = SKBitmap.FromImage(expected);
            if (!actualPixels.Bytes.SequenceEqual(expectedPixels.Bytes))
                throw new Exception($"Native commands differ at scale={scale}, phase={translation}");
        }
        if (renderer.Diagnostics.Work!.CommandCacheHits < 10) throw new Exception("Command reuse not exercised");
        var recordings = renderer.Diagnostics.Work.CommandRecordings;
        var newBounds = Payload(picture, Rect.fromLTWH(-10, -10, 110, 110));
        retained.Invoke(renderer, [target.Canvas, newBounds]);
        retained.Invoke(renderer, [target.Canvas, newBounds]);
        if (renderer.Diagnostics.Work.CommandRecordings != recordings + 1) throw new Exception("Changed cull bounds reused recording");
        recordings = renderer.Diagnostics.Work.CommandRecordings;
        using var shadow = Make(1, shadow: true);
        foreach (var excluded in new[] { Payload(shadow, Rect.fromLTWH(0, 0, 90, 90)), Payload(picture, Rect.fromLTWH(0, 0, 90, 90), changing: true), Payload(picture, Rect.fromLTWH(0, 0, double.MaxValue, 90)) })
            for (var use = 0; use < 3; use++) retained.Invoke(renderer, [target.Canvas, excluded]);
        if (renderer.Diagnostics.Work.CommandRecordings != recordings) throw new Exception("Changing/device shadow picture retained");
        // One workload of distinct pictures verifies eviction, not 140 test runs.
        for (var i = 0; i < 140; i++)
        {
            using var item = Make(i);
            var itemPayload = Payload(item, Rect.fromLTWH(0, 0, 90, 90));
            retained.Invoke(renderer, [target.Canvas, itemPayload]);
            retained.Invoke(renderer, [target.Canvas, itemPayload]);
        }
        var work = renderer.Diagnostics.Work;
        if (work.CommandEntries > 128 || work.RetainedCommands > 32768 || work.CommandBytes > 4 * 1024 * 1024)
            throw new Exception("Native command retention exceeds budget");
        renderer.RegisterFontAsync(File.ReadAllBytes("DorotiTestbedApp/assets/fonts/Roboto-medium.ttf"), "Roboto").GetAwaiter().GetResult();
        if (renderer.Diagnostics.Work.CommandEntries != 0) throw new Exception("Font change retained old commands");
        retained.Invoke(renderer, [target.Canvas, payload]);
        retained.Invoke(renderer, [target.Canvas, payload]);
        renderer.InvalidateGpuContextResources();
        if (renderer.Diagnostics.Work.CommandEntries != 0 || renderer.Diagnostics.Work.CommandBytes != 0)
            throw new Exception("Context loss retained commands");
        Console.WriteLine("Native picture commands PASS: DPR/phase/blend/clip pixels, bounds, exclusions, bounded eviction, font/context invalidation");
    }
}

using System.Diagnostics;
using System.Text.Json;
using Doroti.Framework.Cupertino;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

internal static class RenderingRegressions
{
    public static void Run()
    {
        // An independent expected capture rectangle protects halo rounding and pixel phase.
        var settings = new VariableBlurSettings(new(0, 0), new(0, 100), 4, 0, 32, .25, true, VariableBlurKernel.gaussian);
        var capture = SkiaSceneRenderer.VariableBlurCaptureBounds(new SKRect(100, 80, 180, 140), settings,
            TileMode.clamp, SKMatrix.Identity, 400, 300);
        if (capture != new SKRectI(80, 60, 200, 160)) throw new Exception($"Capture origin/halo/grid changed: {capture}");
        foreach (var tile in new[] { TileMode.repeated, TileMode.mirror })
            if (SkiaSceneRenderer.VariableBlurCaptureBounds(new(100, 80, 180, 140), settings, tile,
                SKMatrix.Identity, 400, 300) != new SKRectI(0, 0, 400, 300))
                throw new Exception("Wrapped blur lost its original capture domain.");
        if (SkiaSceneRenderer.VariableBlurCaptureBounds(new(100, 80, 180, 140), settings, TileMode.clamp,
            SKMatrix.Identity, 401, 301) != new SKRectI(0, 0, 401, 301))
            throw new Exception("Odd-size reduced blur changed sampling phase.");

        var baseline = EngineLayer.debugResourceDiagnostics.ActiveEngineLayers;
        var timings = new List<double>();
        var phases = new List<object>();
        object? cache = null;
        using (var tester = new WidgetTester(new Size(320, 240)))
        {
            tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr, child: new Stack(children:
            [new Positioned(left: 20, top: 30, width: 80, height: 60, child: new ColoredBox(color: new Color(0xffff0000)))])));
            foreach (var dpr in new[] { 1d, 1.25, 2 })
            {
                foreach (var size in new[] { new Size(320, 240), new Size(480, 320) })
                {
                    var watch = Stopwatch.StartNew();
                    tester.setViewport(size, dpr);
                    timings.Add(watch.Elapsed.TotalMilliseconds);
                    var trace = tester.FrameTrace;
                    double Between(DorotiFramePhase start, DorotiFramePhase end) =>
                        (trace.Last(entry => entry.Phase == end).RecordedAtMicroseconds -
                         trace.Last(entry => entry.Phase == start).RecordedAtMicroseconds) / 1000d;
                    phases.Add(new { dpr, width = size.width, height = size.height,
                        buildMs = Between(DorotiFramePhase.build, DorotiFramePhase.layout),
                        layoutAndCompositingMs = Between(DorotiFramePhase.layout, DorotiFramePhase.paint),
                        paintMs = Between(DorotiFramePhase.paint, DorotiFramePhase.sceneBuild),
                        cpuRasterMs = Between(DorotiFramePhase.raster, DorotiFramePhase.rasterEnd) });
                    if (tester.pixel((int)(40 * dpr), (int)(50 * dpr)) != SKColors.Red ||
                        tester.pixel((int)(110 * dpr), (int)(50 * dpr)) != SKColors.White)
                        throw new Exception($"Viewport/DPR changed logical geometry: {size}, dpr={dpr}");
                    if (tester.RendererDiagnostics.Failed != 0) throw new Exception("CPU renderer reported a failed frame.");
                }
            }
            var scroll = new ScrollController();
            tester.pumpWidget(new CupertinoApp(home: ListView.CreateBuilder(controller: scroll, itemCount: 1000,
                itemBuilder: (_, index) => new SizedBox(height: 40, child: new Text($"Row {index}")))));
            tester.pumpAndSettle();
            tester.drag(tester.byType<ListView>().Single(), new Offset(0, -160), steps: 5);
            tester.pumpAndSettle();
            if (scroll.offset <= 0 || tester.byType<Text>().Count > 60) throw new Exception("Long list scroll/virtualization regressed.");
            var offset = scroll.offset;
            scroll.jumpTo(offset); // Finish ballistic movement before comparing reload state.
            var reload = tester.reassemble();
            tester.pumpAndSettle();
            reload.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            if (Math.Abs(scroll.offset - offset) > .01) throw new Exception("Reassemble reset scroll state.");
            tester.pumpWidget(new SizedBox());
            scroll.dispose();
            cache = tester.CacheMemory;
        }
        var remaining = EngineLayer.debugResourceDiagnostics.ActiveEngineLayers - baseline;
        if (remaining != 0) throw new Exception($"Renderer teardown retained {remaining} engine layers.");
        var report = new { renderer = "Skia CPU offscreen", phases, samples = timings,
            p95Milliseconds = timings.Order().ElementAt((int)Math.Ceiling(timings.Count * .95) - 1), cache,
            remainingEngineLayers = remaining, gpuMilliseconds = (double?)null, uploadBytes = (long?)null,
            presentInterval = (double?)null, vramBytes = (long?)null };
        Console.WriteLine("PASS: capture domain/grid; viewport/DPR pixels; long-list pointer scroll; reassemble state; engine-layer lifetime.");
        Console.WriteLine(JsonSerializer.Serialize(report));
    }
}

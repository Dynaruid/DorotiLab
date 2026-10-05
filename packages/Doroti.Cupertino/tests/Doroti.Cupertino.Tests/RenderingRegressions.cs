using System.Diagnostics;
using System.Text.Json;
using Doroti.Cupertino;
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
        using (var dialogTester = new WidgetTester())
        {
            var opened = 0;
            dialogTester.pumpWidget(new CupertinoApp(home: new CupertinoPageScaffold(child:
                new Builder(builder: context => new Align(alignment: Alignment.topLeft, child: new CupertinoButton(
                    child: new Text("Open dialog"), onPressed: () =>
                    {
                        opened++;
                        _ = Doroti.Cupertino.RouteLibrary.showCupertinoDialog<object>(context,
                            builder: dialogContext => new CupertinoAlertDialog(title: new Text("Dialog frame"),
                                actions: [new CupertinoDialogAction(child: new Text("Close dialog"),
                                    onPressed: () => Navigator.of(dialogContext).pop<object>())]));
                    }))))));
            dialogTester.pumpAndSettle();
            dialogTester.tap(dialogTester.text("Open dialog").Single());
            dialogTester.pump(TimeSpan.FromMilliseconds(60));
            if (!dialogTester.byType<FadeTransition>().Any(element =>
                ((FadeTransition)element.widget).opacity.value is > 0 and < 1))
                throw new Exception("Dialog did not expose an intermediate animation frame.");
            dialogTester.tap(dialogTester.text("Open dialog").Single());
            if (opened != 1) throw new Exception("Dialog barrier did not block the underlying pointer.");
            dialogTester.pumpAndSettle();
            dialogTester.tap(dialogTester.text("Close dialog").Single());
            dialogTester.pumpAndSettle();
            if (dialogTester.text("Dialog frame").Count != 0) throw new Exception("Closed dialog retained its route.");
        }
        Console.WriteLine("PASS: pointer-opened dialog intermediate fade frame, modal hit testing and route teardown (CPU).");
        // Both public entry points must resolve to the same Fast adaptive filter,
        // including the normalized endpoints and bounds used by BackdropFilter.
        var defaultBounds = new Rect(10, 20, 210, 120);
        var defaultConfig = Doroti.Framework.Rendering.ImageFilterConfig.CreateVariableBlur()
            .resolve(new Doroti.Framework.Rendering.ImageFilterContext(defaultBounds));
        var defaultDirect = ImageFilter.variableBlur(new(10, 20), new(10, 120), bounds: defaultBounds);
        var explicitFast = ImageFilter.variableBlur(new(10, 20), new(10, 120), bounds: defaultBounds,
            resolutionScale: .25, adaptiveResolution: true, kernel: VariableBlurKernel.fastGaussian);
        if (ImageFilterSnapshot.Capture(defaultConfig) != ImageFilterSnapshot.Capture(explicitFast)
            || ImageFilterSnapshot.Capture(defaultDirect) != ImageFilterSnapshot.Capture(explicitFast))
            throw new Exception("VariableBlur defaults diverged between the public filter and normalized config.");
        var fullQuality = ImageFilter.variableBlur(new(10, 20), new(10, 120), bounds: defaultBounds,
            resolutionScale: 1, kernel: VariableBlurKernel.gaussian);
        if (fullQuality.VariableBlur is not { ResolutionScale: 1, Kernel: VariableBlurKernel.gaussian }
            || ImageFilterSnapshot.Capture(fullQuality) == ImageFilterSnapshot.Capture(defaultDirect))
            throw new Exception("Explicit full-resolution Gaussian was lost after default adoption.");
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
        foreach (var (width, height, scale, tile, expectedReason) in new[]
        {
            (401, 301, .25, TileMode.clamp, "rounded-working-grid"),
            (401, 300, .25, TileMode.clamp, "cropped-y"),
            (400, 301, .25, TileMode.clamp, "cropped-x"),
            (400, 300, .25, TileMode.repeated, "wrapped-domain"),
            (400, 300, .125, TileMode.clamp, "unsupported-grid"),
            (400, 300, .25, TileMode.clamp, "cropped"),
        })
        {
            var configuration = settings with { ResolutionScale = scale };
            var diagnosed = SkiaSceneRenderer.VariableBlurCaptureBounds(new(100, 80, 180, 140),
                configuration, tile, SKMatrix.Identity, width, height, out var reason);
            if (reason != expectedReason || diagnosed != SkiaSceneRenderer.VariableBlurCaptureBounds(
                new(100, 80, 180, 140), configuration, tile, SKMatrix.Identity, width, height))
                throw new Exception($"Capture diagnosis changed geometry or reason: {reason}");
        }
        foreach (var (width, height, expected) in new[]
        {
            (401, 300, new SKRectI(0, 60, 401, 160)),
            (400, 301, new SKRectI(80, 0, 200, 301)),
        })
        {
            var partial = SkiaSceneRenderer.VariableBlurCaptureBounds(new(100, 80, 180, 140),
                settings, TileMode.clamp, SKMatrix.Identity, width, height);
            if (partial != expected) throw new Exception($"Per-axis capture changed sampling grid: {partial}");
            foreach (var scale in new[] { 1d, .5, .25 })
            {
                var fullX = Math.Ceiling(width * scale) / width;
                var fullY = Math.Ceiling(height * scale) / height;
                if (Math.Ceiling(partial.Width * scale) / partial.Width != fullX ||
                    Math.Ceiling(partial.Height * scale) / partial.Height != fullY ||
                    partial.Left * fullX != Math.Floor(partial.Left * fullX) ||
                    partial.Top * fullY != Math.Floor(partial.Top * fullY))
                    throw new Exception("Adaptive level changed global scale or texel phase.");
            }
        }
        var phoneCapture = SkiaSceneRenderer.VariableBlurCaptureBounds(new(0, 981, 1170, 1520.7832f),
            settings with { StartSigma = 20 }, TileMode.clamp, SKMatrix.CreateScale(3, 3),
            1170, 2532, out var phoneReason);
        if (phoneCapture != new SKRectI(0, 792, 1170, 1712) || phoneReason != "cropped-y")
            throw new Exception($"iPhone DPR3 capture retained unnecessary full height: {phoneCapture}/{phoneReason}");

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
            tester.pumpUntilComplete(reload);
            tester.pumpAndSettle();
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

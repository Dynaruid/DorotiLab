using System.Reflection;
using System.Runtime.InteropServices;
using Doroti.Cupertino;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Skia.Vulkan;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

internal static class CupertinoDialogRegression
{
    public static void Run()
    {
        using var tester = new WidgetTester(new Size(200, 160));
        var renderer = (SkiaSceneRenderer)typeof(WidgetTester)
            .GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        CheckPixels(renderer, draw =>
        {
            using var surface = SKSurface.Create(new SKImageInfo(200, 160))!;
            draw(surface.Canvas);
            using var image = surface.Snapshot();
            return SKBitmap.FromImage(image);
        }, "CPU");
        CheckRoutes(tester);
    }

    public static void RunGpu()
    {
        GraphiteNativeLibrary.Configure();
        using var gpu = new VulkanDeviceOwner();
        if (gpu.Software) throw new PlatformNotSupportedException("Cupertino GPU validation requires a hardware Vulkan device.");
        using var quick = new GraphiteVulkanQuick(gpu.Instance.Handle, gpu.Physical.Handle,
            gpu.Device.Handle, gpu.Queue.Handle, gpu.Family, (1u << 22) | (2u << 12));
        using var tester = new WidgetTester(new Size(200, 160));
        var renderer = (SkiaSceneRenderer)typeof(WidgetTester)
            .GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        CheckPixels(renderer, draw =>
        {
            var surface = quick.Begin(200, 160);
            draw(surface.Canvas);
            var frame = (SkiaGraphiteSession.Frame)typeof(GraphiteVulkanQuick)
                .GetField("_frame", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(quick)!;
            var info = new SKImageInfo(200, 160, SKColorType.Rgba8888);
            var readback = frame.RequestReadback(surface, info);
            quick.Complete();
            var pixels = readback.GetAwaiter().GetResult();
            var bitmap = new SKBitmap(info);
            Marshal.Copy(pixels.Pixels, 0, bitmap.GetPixels(), pixels.Pixels.Length);
            return bitmap;
        }, $"Graphite/Vulkan GPU: {gpu.Name}", checkOwnedReplay: true);
    }

    private static void CheckPixels(SkiaSceneRenderer renderer, Func<System.Action<SKCanvas>, SKBitmap> rasterize,
        string backend, bool checkOwnedReplay = false)
    {
        var paths = new List<PathCommand>();
        var drawing = new Doroti.Ui.Canvas(paths);
        drawing.drawRect(new Rect(30, 30, 75, 80), new Doroti.Ui.Paint { color = new Color(0x99ffffff) });
        var foreground = new SceneCommand("picture", null)
        {
            HostPayload = new ScenePicturePayload(991, Offset.zero, paths, null, false, true)
        };
        SceneCommand Op(string operation, object payload) => new(operation, null) { HostPayload = payload };
        var filter = ImageFilterSnapshot.Capture(new ImageFilter(30, 30));
        var backdrop = Op("backdropFilter", new SceneBackdropFilterPayload(filter, BlendMode.srcOver, null));
        var clip = Op("clipRect", new SceneClipRectPayload(new Rect(20, 20, 180, 140), Clip.hardEdge));
        var body = new List<SceneCommand> { clip, backdrop, foreground, new("pop", null), new("pop", null) };
        SKBitmap Render(IReadOnlyList<SceneCommand> commands, bool ownedReplay = false) => rasterize(canvas =>
        {
            using var paint = new SKPaint();
            for (var x = 0; x < 200; x += 8)
            {
                paint.Color = (x / 8) % 2 == 0 ? SKColors.Red : SKColors.Blue;
                canvas.DrawRect(x, 0, 8, 160, paint);
            }
            if (ownedReplay)
                typeof(SkiaSceneRenderer).GetMethod("DrawGpuFilterScene", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(renderer, [canvas, commands, 0, commands.Count, 200, 160, null]);
            else
                renderer.DrawPlatformRasterSegment(canvas, commands, 200, 160);
        });
        using var baseline = Render([]);
        using var complete = Render(body);
        // Flutter fades the completed surface (constant sigma), rather than
        // progressively unblurring the background after the foreground fades.
        foreach (var nested in new[] { false, true })
        foreach (var progress in new[] { 0d, .2, .5, .8, 1d })
        {
            var commands = new List<SceneCommand> { Op("opacity", new SceneOpacityPayload(progress, Offset.zero)) };
            if (nested) commands.Add(Op("opacity", new SceneOpacityPayload(.6, Offset.zero)));
            commands.Add(new("retained", null) { HostPayload = new SceneRetainedPayload(body, 1, 0) });
            commands.Add(new("pop", null));
            if (nested) commands.Add(new("pop", null));
            using var actual = Render(commands);
            if (checkOwnedReplay && progress == .5)
            {
                using var owned = Render(commands, ownedReplay: true);
                if (!actual.Pixels.SequenceEqual(owned.Pixels))
                    throw new Exception("Owned GPU replay changed ordinary backdrop opacity inheritance.");
            }
            var alpha = Math.Round(progress * (nested ? .6 : 1) * 255) / 255;
            var maximumError = 0d;
            for (var y = 22; y < 138; y++)
            for (var x = 22; x < 178; x++)
            {
                var b = baseline.GetPixel(x, y);
                var f = complete.GetPixel(x, y);
                var a = actual.GetPixel(x, y);
                maximumError = Math.Max(maximumError, Math.Abs(a.Red - (b.Red + (f.Red - b.Red) * alpha)));
                maximumError = Math.Max(maximumError, Math.Abs(a.Blue - (b.Blue + (f.Blue - b.Blue) * alpha)));
                maximumError = Math.Max(maximumError, Math.Abs(a.Green - (b.Green + (f.Green - b.Green) * alpha)));
            }
            if (maximumError > 2)
                throw new Exception($"Cupertino backdrop/foreground fade diverged at {progress} (nested={nested}): {maximumError:F2}/255.");
        }
        Console.WriteLine($"PASS: constant-sigma backdrop and foreground fade together at five single/nested opacity samples ({backend} pixels).");
    }

    private static void CheckRoutes(WidgetTester tester)
    {
        tester.setViewport(new Size(390, 844), 1);
        var binding = (WidgetsFlutterBinding)typeof(WidgetTester)
            .GetField("_binding", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        foreach (var brightness in new[] { Brightness.light, Brightness.dark })
        {
            tester.pumpWidget(new CupertinoApp(theme: new CupertinoThemeData(brightness: brightness),
                home: new CupertinoPageScaffold(child: new Builder(builder: context =>
                    new Align(alignment: Alignment.topLeft, child: new CupertinoButton(
                        child: new Text("Open dialog"), onPressed: () =>
                            _ = Doroti.Cupertino.RouteLibrary.showCupertinoDialog<object>(context,
                                builder: dialogContext => new Builder(builder: _ => new CupertinoAlertDialog(
                                    title: new Text("Dialog frame"), actions: [new CupertinoDialogAction(
                                        child: new Text("Close dialog"),
                                        onPressed: () => Navigator.of(dialogContext).pop<object>())])))))))));
            tester.pumpAndSettle();
            tester.tap(tester.text("Open dialog").Single());
            tester.pump(TimeSpan.FromMilliseconds(60));
            CheckFrame(false);
            tester.pumpAndSettle();
            tester.tap(tester.text("Close dialog").Single());
            tester.pump(TimeSpan.FromMilliseconds(60));
            CheckFrame(true);
            tester.pump(TimeSpan.FromMilliseconds(60));
            CheckFrame(true);
            tester.pumpAndSettle();
            if (tester.text("Dialog frame").Count != 0)
                throw new Exception("Cupertino dialog retained its route after closing.");
        }
        Console.WriteLine("PASS: pointer-opened wrapped Cupertino dialogs, light/dark opening and closing frames, constant blur and teardown.");

        void CheckFrame(bool closing)
        {
            var surface = tester.byType<CupertinoPopupSurface>().Single();
            var filter = tester.byType<BackdropFilter>().Single();
            var resolved = ((BackdropFilter)filter.widget).filterConfig!.resolve(new ImageFilterContext(new Rect(0, 0, 390, 844)));
            if (ImageFilterSnapshot.Capture(resolved).Outer is not { SigmaX: 30, SigmaY: 30 })
                throw new Exception("Cupertino transition changed the Flutter blur sigma.");
            var fade = surface.findAncestorWidgetOfExactType<FadeTransition>()
                ?? throw new Exception("Cupertino blur and foreground do not share the route fade.");
            if (fade.opacity.value is not (> 0 and < 1))
                throw new Exception("Cupertino frame is not inside the route transition.");
            if (closing && surface.findAncestorWidgetOfExactType<ScaleTransition>() is not null)
                throw new Exception($"Cupertino closing transition scaled the dialog: status={fade.opacity.status}, progress={fade.opacity.value}.");
            using var scene = binding.renderViews.Single().layer!.buildScene(new SceneBuilder(tester.View.viewId));
            if (!HasFadingBackdrop(scene.Commands))
                throw new Exception("Cupertino scene cannot inherit its fade into the backdrop layer.");
        }
    }

    private static bool HasFadingBackdrop(IReadOnlyList<SceneCommand> commands)
    {
        for (var i = 0; i < commands.Count; i++)
        {
            if (commands[i].HostPayload is SceneRetainedPayload retained && HasFadingBackdrop(retained.Commands))
                return true;
            if (commands[i].HostPayload is not SceneOpacityPayload { Opacity: > 0 and < 1 })
                continue;
            var depth = 1;
            var end = i + 1;
            for (; end < commands.Count; end++)
            {
                if (commands[end].Operation == "pop")
                {
                    if (--depth == 0) break;
                }
                else if (commands[end].Operation is not ("picture" or "retained" or "texture")) depth++;
            }
            if (SkiaSceneRenderer.CanInheritBackdropOpacity(commands, i + 1, end))
                return true;
        }
        return false;
    }
}

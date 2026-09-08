using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using C = Doroti.Framework.Cupertino;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifyMagnifierShadows()
    {
        foreach (var mode in new[] { "material", "cupertino", "cupertino-clipped" })
        {
            using var baseline = Capture(false);
            using var shadow = Capture(true);
            var outside = 0;
            for (var y = 65; y < 175; y++)
            for (var x = 65; x < 215; x++)
            {
                var a = baseline.GetPixel(x, y);
                var b = shadow.GetPixel(x, y);
                if (x >= 125 && x < 150 && y >= 115 && y < 125 && a != b)
                    throw new Exception($"{mode}: shadow occludes the lens interior at {x},{y}");
                if ((x < 97 || x > 184 || y < 97 || y > (mode == "material" ? 141 : 151)) && b.Red < a.Red)
                    outside++;
            }
            if (outside < 20) throw new Exception($"{mode}: magnifier shadow missing outside the lens ({outside} pixels)");
            Console.WriteLine($"Magnifier {mode}: outside shadow={outside} pixels, unobscured interior, repeated rendering PASS");

            SKBitmap Capture(bool shadows)
            {
                using var dispatcher = new PlatformDispatcher();
                using var scope = dispatcher.EnterScope();
                using var host = new Host();
                using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
                    "magnifier-fixture", "magnifier-fixture", "magnifier-fixture");
                using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("magnifier-fixture")
                    .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                    .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
                    .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
                    .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
                    .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
                    .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
                    .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
                Widget magnifier = mode == "material"
                    ? new M.Magnifier(shadows: shadows ? null! : new List<BoxShadow>())
                    : new C.CupertinoMagnifier(shadows: shadows ? null! : new List<BoxShadow>(),
                        clipBehavior: mode == "cupertino-clipped" ? Clip.hardEdge : Clip.none);
                var entrypoint = new Doroti.Framework.DorotiWidgetEntrypoint(() =>
                    new Directionality(textDirection: TextDirection.ltr, child: new Stack(children: new List<Widget>
                    {
                        new Positioned(left: 100, top: 100, child: magnifier),
                    })));
                var errors = new List<FlutterErrorDetails>();
                var previousError = FlutterError.onError;
                FlutterError.onError = errors.Add;
                try
                {
                    entrypoint.Bootstrap(dispatcher);
                    entrypoint.AttachView(view);
                    for (var frame = 0; frame < 8 && host.HasPendingFrame; frame++) host.Fire();
                    if (errors.Count > 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
                    using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
                    byte[]? first = null;
                    for (var frame = 0; frame < 4; frame++)
                    {
                        var completion = renderer.Paint(surface, Width, Height)
                            ?? throw new Exception("Magnifier did not produce a scene");
                        renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
                        using var image = surface.Snapshot();
                        using var pixels = SKBitmap.FromImage(image);
                        if (first is not null && !first.SequenceEqual(pixels.Bytes))
                            throw new Exception($"{mode}: repeated rendering changed pixels");
                        first = pixels.Bytes;
                    }
                    using var final = surface.Snapshot();
                    var output = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "doroti-magnifier-shadows");
                    Directory.CreateDirectory(output);
                    using var png = final.Encode(SKEncodedImageFormat.Png, 100);
                    File.WriteAllBytes(System.IO.Path.Combine(output, $"{mode}-{(shadows ? "shadow" : "baseline")}.png"), png.ToArray());
                    return SKBitmap.FromImage(final);
                }
                finally
                {
                    entrypoint.Shutdown();
                    FlutterError.onError = previousError;
                }
            }
        }
        if (Rect.largest != Rect.fromLTRB(-1e9, -1e9, 1e9, 1e9) ||
            !double.IsFinite(Rect.largest.width) || !float.IsFinite((float)Rect.largest.right))
            throw new Exception("Rect.largest must match Flutter's finite Skia-safe bounds");
    }
}

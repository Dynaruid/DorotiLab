using System.Reflection;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class MacMenuRenderingContracts
{
    internal static void Verify()
    {
        using var environment = new ImageFixtureEnvironment();
        var renderer = environment.Renderer;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var draw = typeof(SkiaSceneRenderer).GetMethod("DrawPicture", flags)!;
        var retained = typeof(SkiaSceneRenderer).GetMethod("DrawRetainedPicture", flags)!;
        using var surface = SKSurface.Create(new SKImageInfo(320, 240));
        var bounds = Rect.fromLTWH(40, 40, 240, 160);
        var border = RSuperellipse.fromRectAndRadius(bounds, Radius.circular(8)).deflate(0.5);
        if (border.outerRect != bounds.deflate(0.5) || border.tlRadius != Radius.circular(7.5) ||
            border.deflate(10).tlRadius != Radius.zero)
            throw new Exception("Superellipse border inset must shrink and clamp corner radii");
        foreach (var shape in new[]
        {
            RSuperellipse.fromRectAndRadius(bounds, Radius.circular(24)),
            RSuperellipse.fromRectAndRadius(bounds, Radius.elliptical(48, 24)),
            RSuperellipse.fromRectAndRadius(bounds, Radius.circular(200)),
            new RSuperellipse(bounds, Radius.zero, Radius.circular(12), Radius.elliptical(48, 24), Radius.circular(30)),
        })
        {
            byte[]? expected = null;
            for (var mode = 0; mode < 3; mode++)
            {
                var recorder = new PictureRecorder();
                var canvas = new Canvas(recorder);
                var paint = new Paint { color = new Color(0xff087eff) };
                var path = new Doroti.Ui.Path();
                path.addRSuperellipse(shape);
                if (mode == 0) canvas.drawRSuperellipse(shape, paint);
                else if (mode == 1) canvas.drawPath(path, paint);
                else
                {
                    canvas.save();
                    canvas.clipRSuperellipse(shape);
                    canvas.drawRect(bounds.inflate(10), paint);
                    canvas.restore();
                }
                using var picture = recorder.endRecording();
                surface.Canvas.Clear(SKColors.Transparent);
                draw.Invoke(renderer, [surface.Canvas, picture.Commands]);
                using var image = surface.Snapshot();
                using var pixels = SKBitmap.FromImage(image);
                if (pixels.GetPixel(160, 120).Alpha != 255 || pixels.GetPixel(279, 40).Alpha != 0)
                    throw new Exception("Superellipse lost its interior or rounded corner");
                if (expected is not null && !expected.SequenceEqual(pixels.Bytes))
                {
                    var delta = expected.Zip(pixels.Bytes, (a, b) => Math.Abs(a - b)).Max();
                    // Skia's clip coverage and path rasterizers can round an
                    // antialiased edge differently; the contour must still agree.
                    if (mode != 2 || delta > 8) throw new Exception($"Superellipse draw/path/clip contours disagree: mode={mode}, delta={delta}");
                }
                expected = pixels.Bytes;
            }
        }

        // Flutter's 45-degree corner inset is (1-cos(pi/4))*radius.
        // Check both sides of the contour after translation and elliptical scaling.
        var factory = typeof(SkiaSceneRenderer).Assembly.GetType("Doroti.Skia.Rendering.SkiaRSuperellipsePath")!
            .GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)!;
        using (var path = (SKPath)factory.Invoke(null, [RSuperellipse.fromRectAndRadius(bounds, Radius.elliptical(48, 24))])!)
        {
            const double gap = 0.29289321881;
            if (path.Contains((float)(40 + gap * 48 - 1), (float)(40 + gap * 24 - 1)) ||
                !path.Contains((float)(40 + gap * 48 + 1), (float)(40 + gap * 24 + 1)))
                throw new Exception("Superellipse differs from Flutter's corner inset");
        }
        foreach (var style in new[] { BlurStyle.normal, BlurStyle.solid, BlurStyle.outer, BlurStyle.inner })
        {
            var recorder = new PictureRecorder();
            var canvas = new Canvas(recorder);
            canvas.drawRSuperellipse(RSuperellipse.fromRectAndRadius(bounds, Radius.circular(8)),
                new Paint { color = new Color(0xff000000), maskFilter = MaskFilter.blur(style, 6) });
            using var picture = recorder.endRecording();
            var builder = new SceneBuilder(91);
            builder.addPicture(Offset.zero, picture, bounds);
            using var scene = builder.build();
            for (var frame = 0; frame < 4; frame++)
            {
                surface.Canvas.Clear(SKColors.Transparent);
                retained.Invoke(renderer, [surface.Canvas, scene.Commands.Single().HostPayload]);
                using var image = surface.Snapshot();
                using var pixels = SKBitmap.FromImage(image);
                var near = pixels.GetPixel(160, 203).Alpha;
                var far = pixels.GetPixel(160, 210).Alpha;
                if (style == BlurStyle.inner ? near != 0 : !(near > far && far > 0))
                    throw new Exception($"{style}: blur halo missing, hard-edged or clipped after frame {frame}: {near}/{far}");
                var center = pixels.GetPixel(160, 120).Alpha;
                if (style == BlurStyle.outer ? center != 0 : center != 255)
                    throw new Exception($"{style}: wrong blur interior");
            }
        }
        Console.WriteLine("macOS menu rendering: Flutter corner geometry, draw/path/clip parity, blur styles and retained shadow bounds PASS");
    }
}

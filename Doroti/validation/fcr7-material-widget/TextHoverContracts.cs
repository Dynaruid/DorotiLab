using System.Reflection;
using Doroti.Framework.Painting;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using TextStyle = Doroti.Framework.Painting.TextStyle;

internal static class TextHoverContracts
{
    internal static void Verify(bool platformSurfaces = false)
    {
        using var environment = new ImageFixtureEnvironment();
        environment.Renderer.RegisterFontAsync(File.ReadAllBytes("DorotiTestbedApp/assets/fonts/Roboto-medium.ttf"), "Roboto").GetAwaiter().GetResult();
        foreach (var align in new[] { TextAlign.left, TextAlign.center, TextAlign.right })
        foreach (var width in new[] { double.PositiveInfinity, 200.25 })
        {
            var style = new TextStyle(fontFamily: "Roboto", fontSize: 14, fontWeight: FontWeight.w500, letterSpacing: 0.1, color: new Color(0xff102030));
            var painter = new TextPainter(text: new TextSpan(text: "Create project", style: style), textDirection: TextDirection.ltr, textAlign: align);
            double[]? expected = null;
            for (var frame = 0; frame < 12; frame++)
            {
                painter.text = new TextSpan(text: "Create project", style: style.copyWith(color: new Color(0xff102030 + frame)));
                painter.layout(maxWidth: width);
                var recorder = new PictureRecorder();
                painter.paint(new Canvas(recorder), new Offset(10.25, 9.75));
                using var picture = recorder.endRecording();
                var command = picture.Commands.Single();
                var payload = typeof(PathCommand).GetProperty("HostPayload", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(command)!;
                var painted = (Paragraph)payload.GetType().GetProperty("Paragraph")!.GetValue(payload)!;
                var metrics = painted.computeLineMetrics().Single();
                var actual = new[] { painter.width, painter.height, command.Arguments[0], command.Arguments[1], metrics.left, metrics.width, metrics.baseline };
                expected ??= actual;
                if (!actual.SequenceEqual(expected)) throw new Exception($"Paint-only text geometry changed: {align}/{width}");
            }
            painter.dispose();
        }
        Console.WriteLine("Paint-only text geometry PASS");

        using var gpu = new GpuRasterFixture();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var begin = typeof(SkiaSceneRenderer).GetMethod("BeginPictureRasterFrame", flags)!;
        var draw = typeof(SkiaSceneRenderer).GetMethod("DrawPictureLayer", flags)!;
        var direct = typeof(SkiaSceneRenderer).GetMethod("DrawPicture", flags)!;
        var payloadProperty = typeof(SceneCommand).GetProperty("HostPayload", flags)!;
        var cases = platformSurfaces
            ? new[] {
                (1.75f, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, 0f, false),
                (2.625f, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, 0f, false),
                (3f, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, 0f, false),
                (3.5f, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888, 0f, false),
                (2f, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888, 0f, false),
                (3f, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888, 0f, false),
                (1.25f, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, 90f, false),
                (2.625f, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, 15f, false),
                (1.5f, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888, 0f, true),
            }
            : new[] { 1f, 1.25f, 1.5f, 2f }.Select(scale =>
                (scale, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888, 0f, false)).ToArray();
        var largestCoverageDelta = 0;
        var largestCenterDelta = 0.0;
        foreach (var (scale, origin, colorType, rotation, mirror) in cases)
        foreach (var fontFlags in platformSurfaces
            ? new[] { SKSurfacePropsFlags.None, SKSurfacePropsFlags.UseDeviceIndependentFonts }
            : new[] { SKSurfacePropsFlags.None })
        {
            var info = new SKImageInfo(640, 640, colorType, SKAlphaType.Premul);
            using var properties = new SKSurfaceProperties(fontFlags, SKPixelGeometry.Unknown);
            using var target = SKSurface.Create(gpu.Context, true, info, 0, origin, properties, false);
            using var reference = SKSurface.Create(gpu.Context, true, info, 0, origin, properties, false);
            var alternateFlags = fontFlags == SKSurfacePropsFlags.None ? SKSurfacePropsFlags.UseDeviceIndependentFonts : SKSurfacePropsFlags.None;
            using var alternateProperties = new SKSurfaceProperties(alternateFlags, SKPixelGeometry.Unknown);
            using var alternateTarget = platformSurfaces ? SKSurface.Create(gpu.Context, true, info, 0, origin, alternateProperties, false) : null;
            using var alternateReference = platformSurfaces ? SKSurface.Create(gpu.Context, true, info, 0, origin, alternateProperties, false) : null;
            environment.Renderer.InvalidateGpuContextResources();
            var painter = new TextPainter(text: new TextSpan(text: "Create project", style: new TextStyle(fontFamily: "Roboto", fontSize: 14, fontWeight: FontWeight.w500)), textDirection: TextDirection.ltr);
            painter.layout();
            var recorder = new PictureRecorder();
            painter.paint(new Canvas(recorder), new Offset(3.25, 4.75));
            using var picture = recorder.endRecording();
            var builder = new SceneBuilder(91);
            builder.addPicture(Offset.zero, picture, Rect.fromLTWH(-0.375, -0.625, 150, 40), isComplexHint: true);
            using var scene = builder.build();
            var payload = payloadProperty.GetValue(scene.Commands.Single())!;
            var commands = payload.GetType().GetProperty("Commands")!.GetValue(payload)!;
            var translations = new[] { 10.375f, 10.375f, 10.375f, 11.375f, 11.625f, 11.625f, 11.625f, -0.375f, -0.375f, -0.375f };
            var hitsBefore = environment.Renderer.Diagnostics.PictureRasterCacheHits;
            for (var frame = 0; frame < translations.Length; frame++)
            {
                // Reuse the same picture across surface policies, including an
                // integer translation that would otherwise hit the old raster.
                var alternate = platformSurfaces && frame is >= 3 and <= 6;
                var actualSurface = alternate ? alternateTarget! : target;
                var expectedSurface = alternate ? alternateReference! : reference;
                var activeFlags = alternate ? alternateFlags : fontFlags;
                begin.Invoke(environment.Renderer, null);
                foreach (var surface in new[] { actualSurface, expectedSurface })
                {
                    surface.Canvas.ResetMatrix();
                    surface.Canvas.Clear(SKColors.White);
                    surface.Canvas.Translate(translations[frame] + (mirror ? 550 : rotation != 0 ? 80 : 0), 12.625f);
                    surface.Canvas.RotateDegrees(rotation);
                    surface.Canvas.Scale(mirror ? -scale : scale, scale);
                    if (surface == actualSurface) draw.Invoke(environment.Renderer, [surface.Canvas, payload]);
                    else direct.Invoke(environment.Renderer, [surface.Canvas, commands]);
                    surface.Canvas.Flush();
                }
                gpu.Context.Flush();
                using var image = actualSurface.Snapshot();
                using var pixels = SKBitmap.FromImage(image);
                using var expectedImage = expectedSurface.Snapshot();
                using var expected = SKBitmap.FromImage(expectedImage);
                var actualBytes = pixels.Bytes;
                var expectedBytes = expected.Bytes;
                var maxDelta = actualBytes.Zip(expectedBytes).Max(pair => Math.Abs(pair.First - pair.Second));
                // Device-independent distance-field text can round coverage by
                // two 8-bit levels in an intermediate surface. Keep normal text
                // byte-exact and independently bound glyph displacement below.
                var tolerance = activeFlags == SKSurfacePropsFlags.UseDeviceIndependentFonts ? 2 : 0;
                largestCoverageDelta = Math.Max(largestCoverageDelta, maxDelta);
                if (maxDelta > tolerance) throw new Exception($"Text changed when raster cache promoted: scale={scale}, origin={origin}, color={colorType}, rotation={rotation}, mirror={mirror}, flags={activeFlags}, frame={frame}, maxDelta={maxDelta}");
                var actualCenter = InkCenter(actualBytes, info.Width);
                var expectedCenter = InkCenter(expectedBytes, info.Width);
                largestCenterDelta = Math.Max(largestCenterDelta, Math.Max(Math.Abs(actualCenter.X - expectedCenter.X), Math.Abs(actualCenter.Y - expectedCenter.Y)));
                if (Math.Abs(actualCenter.X - expectedCenter.X) > 0.01 || Math.Abs(actualCenter.Y - expectedCenter.Y) > 0.01)
                    throw new Exception("Cached text ink moved by more than 0.01 device pixels");
            }
            if (environment.Renderer.Diagnostics.PictureRasterCacheHits - hitsBefore < (platformSurfaces ? 3 : 4))
                throw new Exception("Text raster stability must exercise cache hits and integer translation reuse");
            painter.dispose();
        }
        Console.WriteLine($"Text raster promotion pixel stability PASS: {cases.Length * 10 * (platformSurfaces ? 2 : 1)} frames, platform surfaces={platformSurfaces}, max coverage delta={largestCoverageDelta}/255, max center delta={largestCenterDelta} device pixels");
    }

    private static (double X, double Y) InkCenter(byte[] pixels, int width)
    {
        double mass = 0, x = 0, y = 0;
        for (var i = 0; i < pixels.Length; i += 4)
        {
            var ink = 765 - pixels[i] - pixels[i + 1] - pixels[i + 2];
            mass += ink;
            x += (i / 4 % width) * ink;
            y += (i / 4 / width) * ink;
        }
        if (mass <= 0) throw new Exception("Text raster fixture is blank");
        return (x / mass, y / mass);
    }
}

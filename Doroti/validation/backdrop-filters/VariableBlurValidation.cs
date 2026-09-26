using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using BlendMode = Doroti.Ui.BlendMode;

internal static class VariableBlurValidation
{
    private const int Size = 64;

    internal static void Run(SkiaSceneRenderer renderer, VulkanFixture? fixture, bool graphite)
    {
        foreach (var invalid in new[] { -1d, double.NaN, double.PositiveInfinity })
            Reject(() =>
                ImageFilter.variableBlur(Offset.zero, new Offset(0, 32), endSigma: invalid)
            );
        Reject(() => ImageFilter.variableBlur(Offset.zero, Offset.zero));
        Reject(() => ImageFilter.variableBlur(Offset.zero, new Offset(double.NaN, 1)));
        Reject(() => ImageFilter.variableBlur(Offset.zero, new Offset(0, 32), maxSamples: 0));
        Reject(() => ImageFilter.variableBlur(Offset.zero, new Offset(0, 32), maxSamples: 65));
        var filter = ImageFilter.variableBlur(new Offset(0, 12), new Offset(0, 52), endSigma: 4);
        if (
            filter != ImageFilter.variableBlur(new Offset(0, 12), new Offset(0, 52), endSigma: 4)
            || filter == ImageFilter.variableBlur(new Offset(0, 12), new Offset(0, 52), endSigma: 3)
        )
            throw new Exception("Variable blur value equality failed");
        var config = Doroti.Framework.Rendering.ImageFilterConfig.CreateVariableBlur(endSigma: 4);
        var box = new Rect(10, 20, 90, 100);
        if (
            config.resolve(new Doroti.Framework.Rendering.ImageFilterContext(box))
            != ImageFilter.variableBlur(
                new Offset(10, 20),
                new Offset(10, 100),
                endSigma: 4,
                bounds: box
            )
        )
            throw new Exception("Relative variable blur did not follow painted bounds");
        config.resolve(new Doroti.Framework.Rendering.ImageFilterContext(new Rect(0, 0, 0, 0)));
        if (fixture is null)
        {
            using var surface = SKSurface.Create(new SKImageInfo(Size, Size));
            var builder = new SceneBuilder(1);
            builder.pushBackdropFilter(filter);
            builder.pop();
            using var scene = builder.build();
            try
            {
                renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, Size, Size);
                throw new Exception("Variable blur silently accepted a CPU target");
            }
            catch (NotSupportedException) { }
            Console.WriteLine("PASS variable blur: argument/equality contracts and CPU rejection");
            return;
        }

        using var picture = Pattern();
        var source = SourcePixels();
        var cases = new[]
        {
            "vertical",
            "reverse",
            "horizontal",
            "diagonal",
            "zero",
            "constant",
            "translated",
            "scaled",
            "rotated",
            "image-offset",
            "nested",
            "foreground",
            "color-compose",
            "retained",
            "decal",
            "mirror",
            "repeated",
            "clipped",
            "bounded",
        };
        foreach (var name in cases)
        {
            var start = new Offset(0, 12);
            var end = new Offset(0, 52);
            var firstSigma = 0d;
            var lastSigma = 4d;
            var matrix = SKMatrix.Identity;
            var tile = name switch
            {
                "decal" => TileMode.decal,
                "mirror" => TileMode.mirror,
                "repeated" => TileMode.repeated,
                _ => TileMode.clamp,
            };
            if (name == "reverse")
                (firstSigma, lastSigma) = (4, 0);
            if (name == "horizontal")
                (start, end) = (new Offset(12, 0), new Offset(52, 0));
            if (name == "diagonal")
                (start, end) = (new Offset(12, 12), new Offset(52, 52));
            if (name == "zero")
                lastSigma = 0;
            if (name == "constant")
                firstSigma = 4;
            if (name == "translated")
                matrix = SKMatrix.CreateTranslation(3, 8);
            if (name == "scaled")
                matrix = SKMatrix.CreateScale(1.5f, 1.25f);
            if (name == "rotated")
                matrix = new SKMatrix(0, -1, 64, 1, 0, 0, 0, 0, 1);
            if (name == "image-offset")
                matrix = SKMatrix.CreateTranslation(5, 7);
            ImageFilter variable = ImageFilter.variableBlur(
                start,
                end,
                firstSigma,
                lastSigma,
                tileMode: tile,
                bounds: name == "bounded" ? new Rect(8, 9, 53, 54) : null
            );
            if (name == "color-compose")
                variable = new ImageFilter(ColorFilter.saturation(0), variable);
            var builder = new SceneBuilder(1);
            if (name == "nested")
                builder.pushOpacity(153);
            if (name != "image-offset")
                builder.addPicture(Offset.zero, picture);
            builder.pushClipRect(
                name == "clipped" ? new Rect(8, 9, 53, 54) : new Rect(0, 0, 64, 64),
                clipBehavior: Clip.hardEdge
            );
            if (name != "image-offset")
                builder.pushTransform(Matrix4(matrix));
            EngineLayer retained;
            if (name == "image-offset")
            {
                retained = builder.pushImageFilter(
                    variable,
                    offset: new Offset(5, 7),
                    bounds: new Rect(-5, -7, 59, 57)
                );
                builder.addPicture(new Offset(-5, -7), picture);
            }
            else
                retained = builder.pushBackdropFilter(variable, BlendMode.src);
            if (name == "foreground")
            {
                var recorder = new PictureRecorder();
                new Canvas(recorder).drawRect(
                    new Rect(28, 28, 36, 36),
                    new Paint { color = new Color(0xff00ff00) }
                );
                using var foreground = recorder.endRecording();
                builder.addPicture(Offset.zero, foreground);
            }
            builder.pop();
            if (name != "image-offset")
                builder.pop();
            builder.pop();
            if (name == "nested")
                builder.pop();
            using var scene = builder.build();
            using var actual = fixture.CreateSurface(
                new SKImageInfo(Size, Size, SKColorType.Rgba8888, SKAlphaType.Premul)
            );
            actual.Canvas.Clear(SKColors.Transparent);
            renderer.DrawPlatformRasterSegment(actual.Canvas, scene.Commands, Size, Size);
            var expected = Reference(source, start, end, firstSigma, lastSigma, matrix, tile);
            for (var p = 0; p < Size * Size; p++)
            {
                if (
                    name is "clipped" or "bounded"
                    && (p % Size < 8 || p % Size >= 53 || p / Size < 9 || p / Size >= 54)
                )
                    Array.Copy(source, p * 4, expected, p * 4, 4);
                if (name == "color-compose")
                {
                    var gray = (float)(
                        expected[p * 4] * .2126
                        + expected[p * 4 + 1] * .7152
                        + expected[p * 4 + 2] * .0722
                    );
                    for (var c = 0; c < 3; c++)
                        expected[p * 4 + c] = gray;
                }
                if (name == "nested")
                    for (var c = 0; c < 4; c++)
                        expected[p * 4 + c] *= .6f;
                if (
                    name == "foreground"
                    && p % Size is >= 28 and < 36
                    && p / Size is >= 28 and < 36
                )
                {
                    expected[p * 4] = 0;
                    expected[p * 4 + 1] = 255;
                    expected[p * 4 + 2] = 0;
                    expected[p * 4 + 3] = 255;
                }
            }
            Verify(name, expected, actual, fixture, graphite, name == "diagonal" ? 5 : 2);
            if (name == "retained")
            {
                var replay = new SceneBuilder(1);
                replay.addPicture(Offset.zero, picture);
                replay.addRetained(retained);
                using var replayScene = replay.build();
                actual.Canvas.Clear(SKColors.Transparent);
                renderer.DrawPlatformRasterSegment(actual.Canvas, replayScene.Commands, Size, Size);
                Verify("retained-replay", expected, actual, fixture, graphite, 2);
            }
        }
        Console.WriteLine(
            "PASS variable blur: 20 GPU comparisons with independent 2D Gaussian reference"
        );
    }

    private static void Reject(Action action)
    {
        try
        {
            action();
            throw new Exception("Invalid variable blur accepted");
        }
        catch (ArgumentException) { }
    }

    private static double[] Matrix4(SKMatrix m) =>
        [m.ScaleX, m.SkewY, 0, 0, m.SkewX, m.ScaleY, 0, 0, 0, 0, 1, 0, m.TransX, m.TransY, 0, 1];

    private static Picture Pattern()
    {
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        for (var y = 0; y < Size; y += 8)
        for (var x = 0; x < Size; x += 8)
            canvas.drawRect(
                new Rect(x, y, x + 8, y + 8),
                new Paint { color = new Color(Color(x, y)), isAntiAlias = false }
            );
        return recorder.endRecording();
    }

    private static uint Color(int x, int y) =>
        ((x / 8 + y / 8) % 2 == 0) ? 0xffe06020u : 0x804080c0u;

    private static float[] SourcePixels()
    {
        var data = new float[Size * Size * 4];
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            var color = Color(x, y);
            var alpha = color >> 24;
            var p = (y * Size + x) * 4;
            data[p] = MathF.Round(((color >> 16) & 255) * alpha / 255f);
            data[p + 1] = MathF.Round(((color >> 8) & 255) * alpha / 255f);
            data[p + 2] = MathF.Round((color & 255) * alpha / 255f);
            data[p + 3] = alpha;
        }
        return data;
    }

    // Brute-force 2D Gaussian at the OUTPUT pixel's sigma. Does not reuse the
    // production shader or its intermediate axis passes. Readback is test-only.
    private static float[] Reference(
        float[] source,
        Offset start,
        Offset end,
        double firstSigma,
        double lastSigma,
        SKMatrix matrix,
        TileMode tile
    )
    {
        matrix.TryInvert(out var inverse);
        var dx = end.dx - start.dx;
        var dy = end.dy - start.dy;
        var length = Math.Sqrt(dx * dx + dy * dy);
        var ux = dx / length;
        var uy = dy / length;
        var ax = matrix.ScaleX * ux + matrix.SkewX * uy;
        var ay = matrix.SkewY * ux + matrix.ScaleY * uy;
        var bx = -matrix.ScaleX * uy + matrix.SkewX * ux;
        var by = -matrix.SkewY * uy + matrix.ScaleY * ux;
        var output = new float[source.Length];
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            var point = inverse.MapPoint(x + .5f, y + .5f);
            var t = Math.Clamp(
                ((point.X - start.dx) * dx + (point.Y - start.dy) * dy) / (length * length),
                0,
                1
            );
            var sigma = firstSigma + (lastSigma - firstSigma) * t;
            if (sigma < .0001)
            {
                Array.Copy(source, (y * Size + x) * 4, output, (y * Size + x) * 4, 4);
                continue;
            }
            var na = Math.Min(
                32,
                Math.Max(1, (int)Math.Ceiling(3 * sigma * Math.Sqrt(ax * ax + ay * ay)))
            );
            var nb = Math.Min(
                32,
                Math.Max(1, (int)Math.Ceiling(3 * sigma * Math.Sqrt(bx * bx + by * by)))
            );
            var sum = new double[4];
            var total = 0d;
            for (var j = -nb; j <= nb; j++)
            for (var i = -na; i <= na; i++)
            {
                var a = i * 3 * sigma / na;
                var b = j * 3 * sigma / nb;
                var weight = Math.Exp(-(a * a + b * b) / (2 * sigma * sigma));
                for (var c = 0; c < 4; c++)
                    sum[c] +=
                        weight * Sample(source, x + ax * a + bx * b, y + ay * a + by * b, c, tile);
                total += weight;
            }
            for (var c = 0; c < 4; c++)
                output[(y * Size + x) * 4 + c] = (float)(sum[c] / total);
        }
        return output;
    }

    private static double Sample(float[] data, double x, double y, int channel, TileMode tile)
    {
        var ix = (int)Math.Floor(x);
        var iy = (int)Math.Floor(y);
        var fx = x - ix;
        var fy = y - iy;
        double At(int px, int py)
        {
            int Tile(int p) =>
                tile switch
                {
                    TileMode.repeated => ((p % Size) + Size) % Size,
                    TileMode.mirror => Math.Min(
                        ((p % (2 * Size)) + 2 * Size) % (2 * Size),
                        2 * Size - 1 - ((p % (2 * Size)) + 2 * Size) % (2 * Size)
                    ),
                    _ => Math.Clamp(p, 0, Size - 1),
                };
            if (tile == TileMode.decal && (px < 0 || py < 0 || px >= Size || py >= Size))
                return 0;
            return data[(Tile(py) * Size + Tile(px)) * 4 + channel];
        }
        return (1 - fy) * ((1 - fx) * At(ix, iy) + fx * At(ix + 1, iy))
            + fy * ((1 - fx) * At(ix, iy + 1) + fx * At(ix + 1, iy + 1));
    }

    private static void Verify(
        string name,
        float[] expected,
        SKSurface actual,
        VulkanFixture fixture,
        bool graphite,
        double tolerance
    )
    {
        byte[] pixels;
        if (graphite)
            pixels = fixture.ReadGraphite(actual, actual).Actual;
        else
        {
            fixture.Context!.Flush(submit: true, synchronous: true);
            using var bitmap = new SKBitmap(
                new SKImageInfo(Size, Size, SKColorType.Rgba8888, SKAlphaType.Premul)
            );
            if (!actual.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0))
                throw new Exception("Readback failed");
            pixels = bitmap.Bytes;
        }
        // Diagonal two-pass resampling and tiling at a finite capture boundary are
        // approximate; compare the interior against the infinite 2D kernel.
        var inset = name == "diagonal" ? 16 : 0;
        var max = 0d;
        for (var y = inset; y < Size - inset; y++)
        for (var x = inset; x < Size - inset; x++)
        for (var c = 0; c < 4; c++)
            max = Math.Max(
                max,
                Math.Abs(expected[(y * Size + x) * 4 + c] - pixels[(y * Size + x) * 4 + c])
            );
        if (max > tolerance)
            throw new Exception($"variable/{name}: max channel error {max:F3} > {tolerance}");
        if (actual.Canvas.SaveCount != 1)
            throw new Exception("Unbalanced variable blur canvas");
        Console.WriteLine($"PASS variable/{name}: max channel error {max:F3}");
    }
}

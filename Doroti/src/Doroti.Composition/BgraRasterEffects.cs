using Doroti.Graphics;

namespace Doroti.Composition;

/// <summary>Deterministic premultiplied-BGRA layer/filter reference used by managed raster targets.</summary>
public static class BgraRasterEffects
{
    public static byte[] ApplyImageFilter(byte[] source, int width, int height, RasterImageFilter filter)
    {
        ArgumentNullException.ThrowIfNull(source);
        filter.Validate();
        if (source.Length != checked(width * height * 4)) throw new ArgumentException("Pixel buffer size does not match dimensions.", nameof(source));
        return filter.Kind switch
        {
            RasterImageFilterKind.Blur => Blur(source, width, height, filter.SigmaX, filter.SigmaY, filter.TileMode),
            RasterImageFilterKind.Compose => ApplyImageFilter(
                ApplyImageFilter(source, width, height, filter.Inner!), width, height, filter.Outer!),
            RasterImageFilterKind.ColorFilter => ApplyColorFilter(source, filter.ColorFilter!),
            RasterImageFilterKind.Matrix => throw new NotSupportedException("Managed spatial matrix image filters are explicitly unsupported."),
            _ => throw new NotSupportedException($"Managed image-filter kind '{filter.Kind}' is unsupported."),
        };
    }

    public static byte[] ApplyColorFilter(byte[] source, RasterColorFilter filter)
    {
        filter.Validate();
        var result = source.ToArray();
        switch (filter.Kind)
        {
            case RasterColorFilterKind.Matrix:
                var matrix = filter.Matrix!;
                for (var offset = 0; offset < result.Length; offset += 4)
                {
                    var b = source[offset];
                    var g = source[offset + 1];
                    var r = source[offset + 2];
                    var a = source[offset + 3];
                    result[offset + 2] = Channel((matrix[0] * r) + (matrix[1] * g) + (matrix[2] * b) + (matrix[3] * a) + matrix[4]);
                    result[offset + 1] = Channel((matrix[5] * r) + (matrix[6] * g) + (matrix[7] * b) + (matrix[8] * a) + matrix[9]);
                    result[offset] = Channel((matrix[10] * r) + (matrix[11] * g) + (matrix[12] * b) + (matrix[13] * a) + matrix[14]);
                    result[offset + 3] = Channel((matrix[15] * r) + (matrix[16] * g) + (matrix[17] * b) + (matrix[18] * a) + matrix[19]);
                }
                return result;
            case RasterColorFilterKind.Mode:
                var color = filter.Color!.Value;
                var overlay = new byte[result.Length];
                for (var offset = 0; offset < overlay.Length; offset += 4)
                {
                    overlay[offset] = color.Blue;
                    overlay[offset + 1] = color.Green;
                    overlay[offset + 2] = color.Red;
                    overlay[offset + 3] = color.Alpha;
                }
                Composite(result, overlay, result.Length / 4, 1, new Rect(0, 0, result.Length / 4, 1), 1, filter.BlendMode);
                return result;
            case RasterColorFilterKind.LinearToSrgbGamma:
            case RasterColorFilterKind.SrgbToLinearGamma:
                for (var offset = 0; offset < result.Length; offset += 4)
                for (var channel = 0; channel < 3; channel++)
                {
                    var value = source[offset + channel] / 255d;
                    var converted = filter.Kind == RasterColorFilterKind.LinearToSrgbGamma
                        ? value <= 0.0031308 ? value * 12.92 : (1.055 * Math.Pow(value, 1 / 2.4)) - 0.055
                        : value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
                    result[offset + channel] = Channel(converted * 255);
                }
                return result;
            default:
                throw new NotSupportedException($"Managed color-filter kind '{filter.Kind}' is unsupported.");
        }
    }

    public static void Composite(byte[] destination, byte[] source, int width, int height, Rect bounds, double opacity, RasterBlendMode mode)
    {
        if (!bounds.IsFinite || !double.IsFinite(opacity) || opacity is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(bounds));
        var left = Math.Clamp((int)Math.Floor(bounds.Left), 0, width);
        var top = Math.Clamp((int)Math.Floor(bounds.Top), 0, height);
        var right = Math.Clamp((int)Math.Ceiling(bounds.Right), 0, width);
        var bottom = Math.Clamp((int)Math.Ceiling(bounds.Bottom), 0, height);
        for (var y = top; y < bottom; y++)
        for (var x = left; x < right; x++)
        {
            var offset = ((y * width) + x) * 4;
            var sa = source[offset + 3] / 255d * opacity;
            var da = destination[offset + 3] / 255d;
            var sb = source[offset] / 255d * opacity;
            var sg = source[offset + 1] / 255d * opacity;
            var sr = source[offset + 2] / 255d * opacity;
            var db = destination[offset] / 255d;
            var dg = destination[offset + 1] / 255d;
            var dr = destination[offset + 2] / 255d;
            var (sourceFactor, destinationFactor) = mode switch
            {
                RasterBlendMode.Clear => (0d, 0d),
                RasterBlendMode.Source => (1d, 0d),
                RasterBlendMode.Destination => (0d, 1d),
                RasterBlendMode.SourceOver => (1d, 1 - sa),
                RasterBlendMode.DestinationOver => (1 - da, 1d),
                RasterBlendMode.SourceIn => (da, 0d),
                RasterBlendMode.DestinationIn => (0d, sa),
                RasterBlendMode.SourceOut => (1 - da, 0d),
                RasterBlendMode.DestinationOut => (0d, 1 - sa),
                RasterBlendMode.SourceAtop => (da, 1 - sa),
                RasterBlendMode.DestinationAtop => (1 - da, sa),
                RasterBlendMode.Xor => (1 - da, 1 - sa),
                RasterBlendMode.Plus => (1d, 1d),
                _ => throw new NotSupportedException($"Managed blend mode '{mode}' is explicitly unsupported."),
            };
            destination[offset] = Channel(255 * Math.Min(1, (sb * sourceFactor) + (db * destinationFactor)));
            destination[offset + 1] = Channel(255 * Math.Min(1, (sg * sourceFactor) + (dg * destinationFactor)));
            destination[offset + 2] = Channel(255 * Math.Min(1, (sr * sourceFactor) + (dr * destinationFactor)));
            destination[offset + 3] = Channel(255 * Math.Min(1, (sa * sourceFactor) + (da * destinationFactor)));
        }
    }

    private static byte[] Blur(byte[] source, int width, int height, double sigmaX, double sigmaY, RasterTileMode tileMode)
    {
        var horizontal = Convolve(source, width, height, sigmaX, horizontal: true, tileMode);
        return Convolve(horizontal, width, height, sigmaY, horizontal: false, tileMode);
    }

    private static byte[] Convolve(byte[] source, int width, int height, double sigma, bool horizontal, RasterTileMode tileMode)
    {
        if (sigma <= 0) return source.ToArray();
        var radius = Math.Max(1, (int)Math.Ceiling(sigma * 3));
        var weights = Enumerable.Range(-radius, (radius * 2) + 1)
            .Select(offset => Math.Exp(-(offset * offset) / (2 * sigma * sigma))).ToArray();
        var sum = weights.Sum();
        for (var index = 0; index < weights.Length; index++) weights[index] /= sum;
        var result = new byte[source.Length];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        for (var channel = 0; channel < 4; channel++)
        {
            var total = 0d;
            for (var kernel = -radius; kernel <= radius; kernel++)
            {
                var sampleX = horizontal ? Resolve(x + kernel, width, tileMode) : x;
                var sampleY = horizontal ? y : Resolve(y + kernel, height, tileMode);
                if (sampleX < 0 || sampleY < 0) continue;
                total += source[((sampleY * width + sampleX) * 4) + channel] * weights[kernel + radius];
            }
            result[((y * width + x) * 4) + channel] = Channel(total);
        }
        return result;
    }

    private static int Resolve(int value, int extent, RasterTileMode mode) => mode switch
    {
        RasterTileMode.Clamp => Math.Clamp(value, 0, extent - 1),
        RasterTileMode.Decal => value < 0 || value >= extent ? -1 : value,
        RasterTileMode.Repeat => ((value % extent) + extent) % extent,
        RasterTileMode.Mirror => Mirror(value, extent),
        _ => throw new NotSupportedException($"Raster tile mode '{mode}' is unsupported."),
    };

    private static int Mirror(int value, int extent)
    {
        var period = extent * 2;
        var resolved = ((value % period) + period) % period;
        return resolved < extent ? resolved : (period - resolved) - 1;
    }

    private static byte Channel(double value) => (byte)Math.Clamp(Math.Round(value), 0, 255);
}

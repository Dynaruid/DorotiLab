using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace Doroti.SceneLab;

public readonly record struct Rgba(byte R, byte G, byte B, byte A)
{
    public static Rgba Parse(string value)
    {
        if (value.Length != 9 || value[0] != '#')
        {
            throw new FormatException("Color must be #RRGGBBAA.");
        }

        return new Rgba(
            Convert.ToByte(value.Substring(1, 2), 16),
            Convert.ToByte(value.Substring(3, 2), 16),
            Convert.ToByte(value.Substring(5, 2), 16),
            Convert.ToByte(value.Substring(7, 2), 16));
    }
}

public sealed class RgbaImage
{
    private readonly byte[] pixels;

    public RgbaImage(int width, int height, Rgba color)
    {
        if (width <= 0 || height <= 0 || width > 16_384 || height > 16_384)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Image dimensions must be in 1..16384.");
        }

        Width = width;
        Height = height;
        pixels = new byte[checked(width * height * 4)];
        for (var index = 0; index < pixels.Length; index += 4)
        {
            pixels[index] = color.R;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.B;
            pixels[index + 3] = color.A;
        }
    }

    private RgbaImage(int width, int height, byte[] pixels)
    {
        Width = width;
        Height = height;
        this.pixels = pixels;
    }

    public int Width { get; }
    public int Height { get; }

    public static ImageDiff Diff(RgbaImage expected, RgbaImage actual)
    {
        if (expected.Width != actual.Width || expected.Height != actual.Height)
        {
            throw new InvalidOperationException("Expected and actual image dimensions differ.");
        }

        var pixels = new byte[expected.pixels.Length];
        var mismatches = 0;
        var maxDelta = 0;
        for (var index = 0; index < pixels.Length; index += 4)
        {
            var different = false;
            for (var channel = 0; channel < 4; channel++)
            {
                var delta = Math.Abs(expected.pixels[index + channel] - actual.pixels[index + channel]);
                maxDelta = Math.Max(maxDelta, delta);
                different |= delta != 0;
            }

            if (different)
            {
                mismatches++;
                pixels[index] = 255;
                pixels[index + 1] = 0;
                pixels[index + 2] = 255;
                pixels[index + 3] = 255;
            }
            else
            {
                pixels[index] = 0;
                pixels[index + 1] = 0;
                pixels[index + 2] = 0;
                pixels[index + 3] = 0;
            }
        }

        return new ImageDiff(new RgbaImage(expected.Width, expected.Height, pixels), mismatches, maxDelta);
    }

    public void SavePng(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        using var stream = File.Create(path);
        stream.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, Width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], Height);
        header[8] = 8;
        header[9] = 6;
        WriteChunk(stream, "IHDR", header);

        using var raw = new MemoryStream();
        using (var zlib = new ZLibStream(raw, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            for (var row = 0; row < Height; row++)
            {
                zlib.WriteByte(0);
                zlib.Write(pixels, row * Width * 4, Width * 4);
            }
        }
        WriteChunk(stream, "IDAT", raw.ToArray());
        WriteChunk(stream, "IEND", ReadOnlySpan<byte>.Empty);
    }

    private static void WriteChunk(Stream stream, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        stream.Write(length);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        stream.Write(typeBytes);
        stream.Write(data);
        var crcInput = new byte[typeBytes.Length + data.Length];
        typeBytes.CopyTo(crcInput, 0);
        data.CopyTo(crcInput.AsSpan(typeBytes.Length));
        Span<byte> crc = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crc, Crc32.Compute(crcInput));
        stream.Write(crc);
    }
}

public sealed record ImageDiff(RgbaImage Image, int MismatchedPixels, int MaxChannelDelta);

internal static class Crc32
{
    public static uint Compute(ReadOnlySpan<byte> data)
    {
        var crc = 0xffffffffu;
        foreach (var value in data)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc >> 1) ^ (0xedb88320u & (uint)-(int)(crc & 1));
            }
        }
        return ~crc;
    }
}

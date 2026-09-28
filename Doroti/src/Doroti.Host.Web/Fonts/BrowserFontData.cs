using System.Buffers.Binary;
using System.IO.Compression;

namespace Doroti.Host.Web;

/// <summary>Bounded WOFF1 to SFNT normalization, independent of native decoder support.</summary>
public static class BrowserFontData
{
    public const int MaximumBytes = 30 * 1024 * 1024;
    /// <summary>Probe the actual browser Skia build rather than infer support from a native host.</summary>
    public static bool CanUseWoff2Directly(byte[] bytes)
    {
        if (!OperatingSystem.IsBrowser() || !bytes.AsSpan().StartsWith("wOF2"u8)) return false;
        if (bytes.Length < 48 || bytes.Length > MaximumBytes
            || BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(8)) != bytes.Length
            || BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(16)) is < 12 or > MaximumBytes)
            throw new InvalidDataException("Invalid WOFF2 length/output size.");
        using var data = SkiaSharp.SKData.CreateCopy(bytes);
        using var face = SkiaSharp.SKTypeface.FromData(data);
        return face is { GlyphCount: > 0 };
    }

    public static byte[] Normalize(byte[] data)
    {
        if (data.Length < 12 || data.Length > MaximumBytes) throw new InvalidDataException("Font length is invalid (12 bytes to 30 MB required).");
        if (CanUseWoff2Directly(data)) return data;
        if (!data.AsSpan().StartsWith("wOFF"u8))
        {
            if (!(data.AsSpan().StartsWith("OTTO"u8) || data.AsSpan().StartsWith("true"u8)
                || data.AsSpan().StartsWith("ttcf"u8) || BinaryPrimitives.ReadUInt32BigEndian(data) == 0x00010000))
                throw new InvalidDataException("Unrecognized font signature (possibly an HTML fallback response).");
            return data;
        }
        uint U32(int at) => BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(at, 4));
        ushort U16(int at) => BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(at, 2));
        if (data.Length < 44 || U32(8) != data.Length || U16(14) != 0) throw new InvalidDataException("Invalid WOFF header.");
        var count = U16(12); var size = U32(16);
        if (count == 0 || count > 4095 || 44 + count * 20 > data.Length || size > MaximumBytes || size < 12 + count * 16)
            throw new InvalidDataException("Invalid WOFF table count/output size.");
        var result = new byte[size]; data.AsSpan(4, 4).CopyTo(result);
        void W16(int at, int value) => BinaryPrimitives.WriteUInt16BigEndian(result.AsSpan(at), (ushort)value);
        void W32(int at, uint value) => BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(at), value);
        var power = 1; var selector = 0;
        while (power * 2 <= count) { power *= 2; selector++; }
        W16(4, count); W16(6, power * 16); W16(8, selector); W16(10, count * 16 - power * 16);
        var output = 12 + count * 16;
        uint previousTag = 0;
        var regions = new List<(uint Start, uint End)>();
        for (var i = 0; i < count; i++)
        {
            var entry = 44 + i * 20; var offset = U32(entry + 4); var compressed = U32(entry + 8); var original = U32(entry + 12);
            var tag = U32(entry);
            if (tag <= previousTag || offset % 4 != 0 || compressed == 0 || original == 0 || compressed > original || offset < 44 + count * 20 || (ulong)offset + compressed > (ulong)data.Length
                || (ulong)output + original > size) throw new InvalidDataException("Invalid WOFF table bounds.");
            if (regions.Any(r => offset < r.End && offset + compressed > r.Start)) throw new InvalidDataException("Overlapping WOFF tables.");
            regions.Add((offset, offset + compressed)); previousTag = tag;
            W32(12 + i * 16, U32(entry)); W32(16 + i * 16, U32(entry + 16));
            W32(20 + i * 16, (uint)output); W32(24 + i * 16, original);
            var target = result.AsSpan(output, (int)original);
            if (compressed == original) data.AsSpan((int)offset, (int)original).CopyTo(target);
            else
            {
                using var input = new MemoryStream(data, (int)offset, (int)compressed);
                using var zlib = new ZLibStream(input, CompressionMode.Decompress);
                zlib.ReadExactly(target);
                if (zlib.ReadByte() != -1) throw new InvalidDataException("WOFF table exceeds declared output size.");
            }
            uint checksum = 0;
            for (var j = 0; j < target.Length; j += 4)
            {
                if (tag == 0x68656164 && j == 8) continue; // head.checkSumAdjustment is zero for this checksum.
                uint word = 0;
                for (var k = 0; k < 4; k++) word = (word << 8) | (j + k < target.Length ? target[j + k] : 0u);
                checksum = unchecked(checksum + word);
            }
            if (checksum != U32(entry + 16)) throw new InvalidDataException("WOFF table checksum mismatch.");
            output = checked(output + ((int)original + 3 & ~3));
        }
        if (output != result.Length) throw new InvalidDataException("WOFF output size mismatch.");
        return result;
    }
}

using System.Buffers.Binary;
using System.Text;

namespace Doroti.IconTools;

/// <summary>OpenType subsets: compact glyph IDs, preserve metrics, and retain selected outlines.
/// Supports the bundled CFF face and TrueType composite glyphs, without external/native tooling.</summary>
internal static class OpenTypeSubset
{
    private static ushort U16(byte[] data, int offset) => BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));
    private static uint U32(byte[] data, int offset) => BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4));
    private static void W16(byte[] data, int offset, int value) => BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(offset, 2), checked((ushort)value));
    private static void W32(byte[] data, int offset, uint value) => BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(offset, 4), value);
    private static Dictionary<string, byte[]> Tables(byte[] font)
    {
        var tables = new Dictionary<string, byte[]>();
        for (var i = 0; i < U16(font, 4); i++)
        {
            var record = 12 + i * 16;
            tables.Add(Encoding.ASCII.GetString(font, record, 4),
                font.AsSpan(checked((int)U32(font, record + 8)), checked((int)U32(font, record + 12))).ToArray());
        }
        return tables;
    }

    internal static SortedDictionary<int, int> ReadCmap(byte[] font)
    {
        var cmap = Tables(font)["cmap"];
        var result = new SortedDictionary<int, int>();
        for (var table = 0; table < U16(cmap, 2); table++)
        {
            var record = 4 + table * 8;
            var platform = U16(cmap, record);
            var encoding = U16(cmap, record + 2);
            if (platform != 0 && !(platform == 3 && encoding is 1 or 10)) continue;
            var start = checked((int)U32(cmap, record + 4));
            switch (U16(cmap, start))
            {
                case 12:
                    for (var i = 0; i < U32(cmap, start + 12); i++)
                    {
                        var group = start + 16 + i * 12;
                        var first = checked((int)U32(cmap, group));
                        var last = checked((int)U32(cmap, group + 4));
                        var glyph = checked((int)U32(cmap, group + 8));
                        if (first < 0 || last > 0x10ffff || last < first) throw new InvalidDataException("Invalid cmap range.");
                        for (var code = first; code <= last; code++) if (glyph + code - first != 0) result[code] = glyph + code - first;
                    }
                    break;
                case 4:
                    var count = U16(cmap, start + 6) / 2;
                    var ends = start + 14;
                    var starts = ends + count * 2 + 2;
                    var deltas = starts + count * 2;
                    var ranges = deltas + count * 2;
                    for (var i = 0; i < count; i++)
                    {
                        var end = U16(cmap, ends + i * 2);
                        var delta = U16(cmap, deltas + i * 2);
                        var range = U16(cmap, ranges + i * 2);
                        for (var code = (int)U16(cmap, starts + i * 2); code <= end && code != 0xffff; code++)
                        {
                            var glyph = range == 0 ? (code + delta) & 0xffff : U16(cmap,
                                ranges + i * 2 + range + (code - U16(cmap, starts + i * 2)) * 2);
                            if (range != 0 && glyph != 0) glyph = (glyph + delta) & 0xffff;
                            if (glyph != 0) result.TryAdd(code, glyph);
                        }
                    }
                    break;
            }
        }
        return result;
    }

    internal static byte[] Create(byte[] font, IEnumerable<int> codePoints)
    {
        var tables = Tables(font);
        var supported = new HashSet<string> { "head", "hhea", "maxp", "OS/2", "name", "cmap", "post", "CFF ", "hmtx", "glyf", "loca", "cvt ", "fpgm", "prep", "gasp", "FFTM", "GDEF", "GPOS", "GSUB", "DSIG" };
        if (tables.Keys.Any(tag => !supported.Contains(tag)))
            throw new InvalidDataException("Unsupported OpenType table; refusing an unsafe icon subset.");
        var cmap = ReadCmap(font);
        var selected = new SortedDictionary<int, int>();
        foreach (var code in codePoints)
            selected.Add(code, cmap.TryGetValue(code, out var glyph) ? glyph :
                throw new InvalidDataException($"Icon font does not contain U+{code:X}."));
        var glyphs = selected.Values.ToHashSet();
        glyphs.Add(0);
        Dictionary<int, int> remap;
        if (tables.TryGetValue("glyf", out var glyf)) remap = SubsetTrueType(tables, glyf, glyphs);
        else if (tables.TryGetValue("CFF ", out var cff))
        {
            remap = glyphs.Order().Select((glyph, index) => (glyph, index)).ToDictionary(p => p.glyph, p => p.index);
            tables["CFF "] = SubsetCff(cff, glyphs);
        }
        else throw new InvalidDataException("Unsupported icon font outlines.");
        var metrics = new byte[remap.Count * 4];
        var originalMetrics = tables["hmtx"];
        var metricCount = U16(tables["hhea"], 34);
        foreach (var (oldGlyph, newGlyph) in remap)
        {
            W16(metrics, newGlyph * 4, U16(originalMetrics, Math.Min(oldGlyph, metricCount - 1) * 4));
            W16(metrics, newGlyph * 4 + 2, U16(originalMetrics, oldGlyph < metricCount ? oldGlyph * 4 + 2 : metricCount * 4 + (oldGlyph - metricCount) * 2));
        }
        tables["hmtx"] = metrics;
        W16(tables["hhea"], 34, remap.Count);
        W16(tables["maxp"], 4, remap.Count);
        tables["post"] = tables["post"].AsSpan(0, 32).ToArray();
        W32(tables["post"], 0, 0x00030000); // No redundant PostScript glyph-name list.
        // Icons use Unicode codepoints, not text ligatures or contextual substitutions.
        foreach (var tag in new[] { "GSUB", "GPOS", "GDEF", "DSIG", "FFTM" }) tables.Remove(tag);
        tables["cmap"] = MakeCmap(new SortedDictionary<int, int>(selected.ToDictionary(p => p.Key, p => remap[p.Value])));
        W32(tables["head"], 8, 0);
        return WriteFont(U32(font, 0), tables);
    }

    private static byte[] MakeCmap(SortedDictionary<int, int> mapping)
    {
        var groups = new List<(int First, int Last, int Glyph)>();
        foreach (var (code, glyph) in mapping)
        {
            if (groups.Count > 0 && groups[^1].Last + 1 == code && groups[^1].Glyph + code - groups[^1].First == glyph)
                groups[^1] = (groups[^1].First, code, groups[^1].Glyph);
            else groups.Add((code, code, glyph));
        }
        var data = new byte[20 + 16 + groups.Count * 12];
        W16(data, 2, 2);
        W16(data, 4, 0); W16(data, 6, 4); W32(data, 8, 20);
        W16(data, 12, 3); W16(data, 14, 10); W32(data, 16, 20);
        W16(data, 20, 12); W32(data, 24, (uint)(16 + groups.Count * 12)); W32(data, 32, (uint)groups.Count);
        for (var i = 0; i < groups.Count; i++)
        {
            var offset = 36 + i * 12;
            W32(data, offset, (uint)groups[i].First); W32(data, offset + 4, (uint)groups[i].Last); W32(data, offset + 8, (uint)groups[i].Glyph);
        }
        return data;
    }

    private static Dictionary<int, int> SubsetTrueType(Dictionary<string, byte[]> tables, byte[] glyf, HashSet<int> selected)
    {
        var count = U16(tables["maxp"], 4);
        var shortLoca = U16(tables["head"], 50) == 0;
        var loca = tables["loca"];
        int Offset(int glyph) => shortLoca ? U16(loca, glyph * 2) * 2 : checked((int)U32(loca, glyph * 4));
        var pending = new Queue<int>(selected);
        while (pending.TryDequeue(out var glyph))
        {
            if (glyph >= count) throw new InvalidDataException("Glyph index is out of range.");
            var start = Offset(glyph);
            if (start == Offset(glyph + 1) || (short)U16(glyf, start) >= 0) continue;
            var cursor = start + 10;
            int flags;
            do
            {
                flags = U16(glyf, cursor);
                var component = U16(glyf, cursor + 2);
                if (selected.Add(component)) pending.Enqueue(component);
                cursor += 4 + ((flags & 1) != 0 ? 4 : 2);
                if ((flags & 8) != 0) cursor += 2;
                else if ((flags & 64) != 0) cursor += 4;
                else if ((flags & 128) != 0) cursor += 8;
            } while ((flags & 32) != 0);
        }
        using var outlines = new MemoryStream();
        var remap = selected.Order().Select((glyph, index) => (glyph, index)).ToDictionary(p => p.glyph, p => p.index);
        var newLoca = new byte[(remap.Count + 1) * 4];
        foreach (var (glyph, index) in remap)
        {
            W32(newLoca, index * 4, checked((uint)outlines.Position));
            var outline = glyf.AsSpan(Offset(glyph), Offset(glyph + 1) - Offset(glyph)).ToArray();
            if (outline.Length > 0 && (short)U16(outline, 0) < 0)
            {
                var cursor = 10;
                int flags;
                do
                {
                    flags = U16(outline, cursor);
                    W16(outline, cursor + 2, remap[U16(outline, cursor + 2)]);
                    cursor += 4 + ((flags & 1) != 0 ? 4 : 2);
                    if ((flags & 8) != 0) cursor += 2;
                    else if ((flags & 64) != 0) cursor += 4;
                    else if ((flags & 128) != 0) cursor += 8;
                } while ((flags & 32) != 0);
            }
            outlines.Write(outline);
            if ((outlines.Position & 1) != 0) outlines.WriteByte(0);
        }
        W32(newLoca, remap.Count * 4, checked((uint)outlines.Position));
        W16(tables["head"], 50, 1);
        tables["loca"] = newLoca;
        tables["glyf"] = outlines.ToArray();
        return remap;
    }

    private sealed record CffIndex(byte[][] Entries, int End);
    private static CffIndex ReadIndex(byte[] data, int start)
    {
        var count = U16(data, start);
        if (count == 0) return new([], start + 2);
        var size = data[start + 2];
        if (size is < 1 or > 4) throw new InvalidDataException("Invalid CFF INDEX offset size.");
        int Offset(int index)
        {
            uint value = 0;
            for (var i = 0; i < size; i++) value = (value << 8) | data[start + 3 + index * size + i];
            return checked((int)value);
        }
        var first = start + 3 + (count + 1) * size - 1;
        return new(Enumerable.Range(0, count).Select(i => data.AsSpan(first + Offset(i), Offset(i + 1) - Offset(i)).ToArray()).ToArray(),
            first + Offset(count));
    }

    private static byte[] SubsetCff(byte[] data, HashSet<int> selected)
    {
        if (data[0] != 1) throw new InvalidDataException("Only CFF version 1 is supported.");
        var names = ReadIndex(data, data[2]);
        var topStart = names.End;
        var tops = ReadIndex(data, topStart);
        if (tops.Entries.Length != 1) throw new InvalidDataException("Expected one CFF face.");
        var top = tops.Entries[0];
        var operands = new List<(int Value, int Position, int Length)>();
        var charStrings = -1;
        var charset = -1;
        var preserved = new List<byte>();
        var entryStart = 0;
        (int Value, int Position, int Length)? privateEnd = null;
        for (var cursor = 0; cursor < top.Length;)
        {
            var position = cursor;
            var token = top[cursor++];
            if (token >= 32 || token is 28 or 29)
            {
                var value = token switch
                {
                    >= 32 and <= 246 => token - 139,
                    >= 247 and <= 250 => (token - 247) * 256 + top[cursor++] + 108,
                    >= 251 and <= 254 => -(token - 251) * 256 - top[cursor++] - 108,
                    28 => ReadShort(),
                    29 => ReadInt(),
                    _ => throw new InvalidDataException("Unsupported CFF integer."),
                };
                operands.Add((value, position, cursor - position));
            }
            else if (token == 30)
            {
                while (cursor < top.Length) { var number = top[cursor++]; if ((number & 15) == 15 || (number >> 4) == 15) break; }
                operands.Add((0, position, cursor - position));
            }
            else
            {
                var op = token == 12 ? 0x100 + top[cursor++] : token;
                if (op is not (2 or 5 or 15 or 17 or 18 or 0x107)) throw new InvalidDataException("Unsupported CFF Top DICT operator.");
                if (op == 17) charStrings = operands.Single().Value;
                if (op == 15) charset = operands.Single().Value;
                if (op is 5 or 0x107) preserved.AddRange(top.AsSpan(entryStart, cursor - entryStart).ToArray());
                if (op == 18)
                {
                    if (operands.Count != 2 || operands[0].Value != 0 || operands[1].Value != data.Length)
                        throw new InvalidDataException("Unsupported CFF Private dictionary; refusing an unsafe subset.");
                    privateEnd = operands[1];
                }
                if (op is 0x11e or 0x124 or 0x125) throw new InvalidDataException("CID-keyed CFF is not supported.");
                operands.Clear();
                entryStart = cursor;
            }
            short ReadShort() { var value = (short)U16(top, cursor); cursor += 2; return value; }
            int ReadInt() { var value = unchecked((int)U32(top, cursor)); cursor += 4; return value; }
        }
        if (charStrings < 0 || charset < 3 || privateEnd is null) throw new InvalidDataException("Unsupported CFF layout.");
        var strings = ReadIndex(data, charStrings);
        if (strings.End != data.Length) throw new InvalidDataException("CFF CharStrings must be the final nonempty structure.");
        var originalStrings = ReadIndex(data, tops.End);
        var globalSubrs = ReadIndex(data, originalStrings.End);
        if (globalSubrs.Entries.Length != 0) throw new InvalidDataException("CFF subroutines are not supported by this icon subsetter.");
        var sids = new List<int> { 0 };
        var format = data[charset++];
        while (sids.Count < strings.Entries.Length)
        {
            var first = U16(data, charset); charset += 2;
            var extra = format switch { 0 => 0, 1 => data[charset++], 2 => U16(data, charset), _ => throw new InvalidDataException("Unsupported CFF charset.") };
            if (format == 2) charset += 2;
            for (var i = 0; i <= extra; i++) sids.Add(first + i);
        }
        var glyphOrder = selected.Order().ToArray();
        var newStrings = new List<byte[]>();
        var newCharset = new byte[1 + (glyphOrder.Length - 1) * 2];
        for (var i = 1; i < glyphOrder.Length; i++)
        {
            var sid = sids[glyphOrder[i]];
            if (sid >= 391) { newStrings.Add(originalStrings.Entries[sid - 391]); sid = 390 + newStrings.Count; }
            W16(newCharset, 1 + (i - 1) * 2, sid);
        }
        var charData = glyphOrder.Select(glyph => strings.Entries[glyph]).ToArray();
        foreach (var program in charData) ValidateType2(program);
        var nameIndex = WriteIndex(names.Entries);
        var stringIndex = WriteIndex(newStrings.ToArray());
        var charIndex = WriteIndex(charData);
        byte[] Top(int charsetOffset, int charOffset, int endOffset)
        {
            var bytes = new List<byte>(preserved);
            void Offset(int offset) { bytes.Add(29); var number = new byte[4]; W32(number, 0, (uint)offset); bytes.AddRange(number); }
            Offset(charsetOffset); bytes.Add(15);
            Offset(charOffset); bytes.Add(17);
            bytes.Add(139); Offset(endOffset); bytes.Add(18); // Empty Private DICT at end.
            return WriteIndex([bytes.ToArray()]);
        }
        var dummyTop = Top(0, 0, 0);
        var charsetOffset = 4 + nameIndex.Length + dummyTop.Length + stringIndex.Length + 2;
        var charOffset = charsetOffset + newCharset.Length;
        return new byte[] { 1, 0, 4, 4 }.Concat(nameIndex).Concat(Top(charsetOffset, charOffset, charOffset + charIndex.Length))
            .Concat(stringIndex).Concat(new byte[2]).Concat(newCharset).Concat(charIndex).ToArray();
    }

    private static byte[] WriteIndex(byte[][] entries)
    {
        if (entries.Length == 0) return new byte[2];
        var header = 3 + (entries.Length + 1) * 4;
        var data = new byte[header + entries.Sum(e => e.Length)];
        W16(data, 0, entries.Length); data[2] = 4;
        var cursor = header;
        for (var i = 0; i < entries.Length; i++)
        {
            W32(data, 3 + i * 4, (uint)(cursor - header + 1));
            entries[i].CopyTo(data, cursor); cursor += entries[i].Length;
        }
        W32(data, 3 + entries.Length * 4, (uint)(cursor - header + 1));
        return data;
    }

    private static void ValidateType2(byte[] program)
    {
        var stack = 0;
        var stems = 0;
        for (var cursor = 0; cursor < program.Length;)
        {
            var token = program[cursor++];
            if (token >= 32 || token == 28)
            {
                stack++;
                cursor += token == 28 ? 2 : token == 255 ? 4 : token is >= 247 and <= 254 ? 1 : 0;
                continue;
            }
            if (token is 10 or 29 or 11) throw new InvalidDataException("CFF subroutine call is unsupported.");
            if (token == 14 && stack >= 4) throw new InvalidDataException("CFF seac composites are unsupported.");
            if (token is 1 or 3 or 18 or 23 or 19 or 20) stems += stack / 2;
            if (token is 19 or 20) cursor += (stems + 7) / 8;
            if (token == 12 && program[cursor++] is not (34 or 35 or 36 or 37))
                throw new InvalidDataException("CFF arithmetic is unsupported.");
            stack = 0;
        }
    }

    private static uint Checksum(byte[] data)
    {
        uint sum = 0;
        for (var i = 0; i < data.Length; i += 4)
        {
            uint word = 0;
            for (var j = 0; j < 4; j++) word = (word << 8) | (i + j < data.Length ? data[i + j] : 0u);
            sum = unchecked(sum + word);
        }
        return sum;
    }

    private static byte[] WriteFont(uint flavor, Dictionary<string, byte[]> tables)
    {
        var entries = tables.OrderBy(t => t.Key, StringComparer.Ordinal).ToArray();
        var size = 12 + entries.Length * 16 + entries.Sum(t => (t.Value.Length + 3) & ~3);
        var font = new byte[size];
        W32(font, 0, flavor); W16(font, 4, entries.Length);
        var power = (int)Math.Log2(entries.Length);
        W16(font, 6, (1 << power) * 16); W16(font, 8, power); W16(font, 10, entries.Length * 16 - (1 << power) * 16);
        var offset = 12 + entries.Length * 16;
        var headOffset = 0;
        for (var i = 0; i < entries.Length; i++)
        {
            var (tag, data) = entries[i];
            var record = 12 + i * 16;
            Encoding.ASCII.GetBytes(tag).CopyTo(font, record);
            W32(font, record + 4, Checksum(data)); W32(font, record + 8, (uint)offset); W32(font, record + 12, (uint)data.Length);
            data.CopyTo(font, offset);
            if (tag == "head") headOffset = offset;
            offset += (data.Length + 3) & ~3;
        }
        W32(font, headOffset + 8, unchecked(0xb1b0afba - Checksum(font)));
        return font;
    }
}

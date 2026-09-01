using System.Buffers.Binary;
using System.Text;

namespace Doroti.Graphics.DisplayList;

public static class DisplayListChecksum
{
    private const uint Polynomial = 0xEDB88320u;
    private static readonly uint[][] Tables = CreateTables();

    public static uint Compute(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < DisplayListFormat.HeaderSize)
        {
            throw new ArgumentException("A DisplayList checksum requires a complete header.", nameof(buffer));
        }

        var crc = uint.MaxValue;
        Append(ref crc, buffer[..DisplayListFormat.ChecksumOffset]);
        for (var index = 0; index < DisplayListFormat.ChecksumSize; index++)
            crc = Tables[0][(byte)crc] ^ (crc >> 8);
        Append(ref crc, buffer[(DisplayListFormat.ChecksumOffset + DisplayListFormat.ChecksumSize)..]);

        return ~crc;
    }

    private static void Append(ref uint crc, ReadOnlySpan<byte> buffer)
    {
        var table0 = Tables[0];
        while (buffer.Length >= 8)
        {
            crc ^= BinaryPrimitives.ReadUInt32LittleEndian(buffer);
            crc =
                Tables[7][(byte)crc] ^
                Tables[6][(byte)(crc >> 8)] ^
                Tables[5][(byte)(crc >> 16)] ^
                Tables[4][(byte)(crc >> 24)] ^
                Tables[3][buffer[4]] ^
                Tables[2][buffer[5]] ^
                Tables[1][buffer[6]] ^
                table0[buffer[7]];
            buffer = buffer[8..];
        }
        foreach (var value in buffer)
            crc = table0[(byte)(crc ^ value)] ^ (crc >> 8);
    }

    private static uint[][] CreateTables()
    {
        var tables = new uint[8][];
        tables[0] = new uint[256];
        for (uint index = 0; index < tables[0].Length; index++)
        {
            var value = index;
            for (var bit = 0; bit < 8; bit++)
                value = (value & 1) == 0 ? value >> 1 : (value >> 1) ^ Polynomial;
            tables[0][index] = value;
        }
        for (var slice = 1; slice < tables.Length; slice++)
        {
            tables[slice] = new uint[256];
            for (var index = 0; index < tables[slice].Length; index++)
            {
                var prior = tables[slice - 1][index];
                tables[slice][index] = (prior >> 8) ^ tables[0][(byte)prior];
            }
        }
        return tables;
    }
}

internal sealed class DisplayListBinaryWriter
{
    private byte[] _buffer;
    private int _length;

    internal DisplayListBinaryWriter(int initialCapacity = 256)
    {
        if (initialCapacity < 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));
        _buffer = new byte[initialCapacity];
    }

    internal int Length => _length;

    internal ReadOnlySpan<byte> WrittenSpan => _buffer.AsSpan(0, _length);

    internal byte[] ToArray() => WrittenSpan.ToArray();

    internal void WriteByte(byte value)
    {
        EnsureCapacity(1);
        _buffer[_length++] = value;
    }

    internal void WriteBoolean(bool value) => WriteByte(value ? (byte)1 : (byte)0);

    internal void WriteUInt16(ushort value)
    {
        var destination = GetDestination(sizeof(ushort));
        BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
    }

    internal void WriteUInt32(uint value)
    {
        var destination = GetDestination(sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
    }

    internal void PatchUInt32(int offset, uint value)
    {
        if (offset < 0 || offset > _length - sizeof(uint))
            throw new ArgumentOutOfRangeException(nameof(offset));
        BinaryPrimitives.WriteUInt32LittleEndian(_buffer.AsSpan(offset, sizeof(uint)), value);
    }

    internal void WriteInt32(int value)
    {
        var destination = GetDestination(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(destination, value);
    }

    internal void WriteUInt64(ulong value)
    {
        var destination = GetDestination(sizeof(ulong));
        BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
    }

    internal void WriteSingle(float value)
    {
        var canonical = value == 0 ? 0 : value;
        WriteInt32(BitConverter.SingleToInt32Bits(canonical));
    }

    internal void WriteBytes(ReadOnlySpan<byte> values)
    {
        values.CopyTo(GetDestination(values.Length));
    }

    private Span<byte> GetDestination(int length)
    {
        EnsureCapacity(length);
        var destination = _buffer.AsSpan(_length, length);
        _length += length;
        return destination;
    }

    private void EnsureCapacity(int additionalLength)
    {
        if (additionalLength < 0) throw new ArgumentOutOfRangeException(nameof(additionalLength));
        var required = checked(_length + additionalLength);
        if (required <= _buffer.Length) return;
        var doubled = _buffer.Length == 0 ? 256 : checked(_buffer.Length * 2);
        Array.Resize(ref _buffer, Math.Max(required, doubled));
    }
}

internal ref struct DisplayListBinaryReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private readonly int _baseOffset;
    private int _offset;

    internal DisplayListBinaryReader(ReadOnlySpan<byte> buffer, int baseOffset = 0)
    {
        _buffer = buffer;
        _baseOffset = baseOffset;
        _offset = 0;
    }

    internal readonly int AbsoluteOffset => _baseOffset + _offset;

    internal readonly int Remaining => _buffer.Length - _offset;

    internal byte ReadByte()
    {
        Ensure(sizeof(byte));
        return _buffer[_offset++];
    }

    internal bool ReadBoolean()
    {
        var offset = AbsoluteOffset;
        return ReadByte() switch
        {
            0 => false,
            1 => true,
            _ => throw Error(DisplayListFailureCode.InvalidValue, offset, "Boolean values must be encoded as 0 or 1."),
        };
    }

    internal ushort ReadUInt16()
    {
        Ensure(sizeof(ushort));
        var value = BinaryPrimitives.ReadUInt16LittleEndian(_buffer[_offset..]);
        _offset += sizeof(ushort);
        return value;
    }

    internal uint ReadUInt32()
    {
        Ensure(sizeof(uint));
        var value = BinaryPrimitives.ReadUInt32LittleEndian(_buffer[_offset..]);
        _offset += sizeof(uint);
        return value;
    }

    internal int ReadInt32()
    {
        Ensure(sizeof(int));
        var value = BinaryPrimitives.ReadInt32LittleEndian(_buffer[_offset..]);
        _offset += sizeof(int);
        return value;
    }

    internal ulong ReadUInt64()
    {
        Ensure(sizeof(ulong));
        var value = BinaryPrimitives.ReadUInt64LittleEndian(_buffer[_offset..]);
        _offset += sizeof(ulong);
        return value;
    }

    internal float ReadSingle()
    {
        var offset = AbsoluteOffset;
        var bits = ReadInt32();
        if (bits == int.MinValue)
        {
            throw Error(
                DisplayListFailureCode.NonCanonicalEncoding,
                offset,
                "Negative zero is not a canonical DisplayList float encoding.");
        }

        var value = BitConverter.Int32BitsToSingle(bits);
        if (!float.IsFinite(value))
        {
            throw Error(DisplayListFailureCode.InvalidValue, offset, "DisplayList floats must be finite.");
        }

        return value;
    }

    internal ReadOnlySpan<byte> ReadBytes(int length)
    {
        Ensure(length);
        var value = _buffer.Slice(_offset, length);
        _offset += length;
        return value;
    }

    internal DisplayListBinaryReader ReadSubReader(int length)
    {
        var offset = AbsoluteOffset;
        var bytes = ReadBytes(length);
        return new DisplayListBinaryReader(bytes, offset);
    }

    internal readonly void RequireFinished(DisplayListFailureCode code, string message)
    {
        if (Remaining != 0)
        {
            throw Error(code, AbsoluteOffset, message);
        }
    }

    internal readonly DisplayListFormatException Error(
        DisplayListFailureCode code,
        int offset,
        string message) =>
        new(code, offset, message);

    private readonly void Ensure(int length)
    {
        if (length < 0 || length > Remaining)
        {
            throw Error(
                DisplayListFailureCode.BoundsExceeded,
                AbsoluteOffset,
                $"The DisplayList read of {length} bytes exceeds the current section bounds.");
        }
    }
}

internal sealed class DisplayListFormatException(
    DisplayListFailureCode code,
    int offset,
    string message) : Exception(message)
{
    internal DisplayListFailureCode Code { get; } = code;

    internal int Offset { get; } = offset;
}

internal static class DisplayListUtf8
{
    internal static Encoding StrictEncoding { get; } = new UTF8Encoding(false, true);

    internal static int Compare(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        var commonLength = Math.Min(left.Length, right.Length);
        for (var index = 0; index < commonLength; index++)
        {
            var comparison = left[index].CompareTo(right[index]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        return left.Length.CompareTo(right.Length);
    }
}

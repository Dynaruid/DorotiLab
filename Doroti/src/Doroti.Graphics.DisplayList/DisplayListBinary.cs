using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Doroti.Graphics.DisplayList;

public static class DisplayListChecksum
{
    public static uint Compute(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < DisplayListFormat.HeaderSize)
        {
            throw new ArgumentException("A DisplayList checksum requires a complete header.", nameof(buffer));
        }

        var crc = uint.MaxValue;
        for (var index = 0; index < buffer.Length; index++)
        {
            var value = index >= DisplayListFormat.ChecksumOffset &&
                index < DisplayListFormat.ChecksumOffset + DisplayListFormat.ChecksumSize
                ? (byte)0
                : buffer[index];
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) == 0 ? crc >> 1 : (crc >> 1) ^ 0xEDB88320u;
            }
        }

        return ~crc;
    }
}

internal sealed class DisplayListBinaryWriter
{
    private readonly ArrayBufferWriter<byte> _buffer = new();

    internal int Length => _buffer.WrittenCount;

    internal ReadOnlySpan<byte> WrittenSpan => _buffer.WrittenSpan;

    internal byte[] ToArray() => _buffer.WrittenSpan.ToArray();

    internal void WriteByte(byte value)
    {
        var destination = _buffer.GetSpan(1);
        destination[0] = value;
        _buffer.Advance(1);
    }

    internal void WriteBoolean(bool value) => WriteByte(value ? (byte)1 : (byte)0);

    internal void WriteUInt16(ushort value)
    {
        var destination = _buffer.GetSpan(sizeof(ushort));
        BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
        _buffer.Advance(sizeof(ushort));
    }

    internal void WriteUInt32(uint value)
    {
        var destination = _buffer.GetSpan(sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
        _buffer.Advance(sizeof(uint));
    }

    internal void WriteInt32(int value)
    {
        var destination = _buffer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(destination, value);
        _buffer.Advance(sizeof(int));
    }

    internal void WriteUInt64(ulong value)
    {
        var destination = _buffer.GetSpan(sizeof(ulong));
        BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
        _buffer.Advance(sizeof(ulong));
    }

    internal void WriteSingle(float value)
    {
        var canonical = value == 0 ? 0 : value;
        WriteInt32(BitConverter.SingleToInt32Bits(canonical));
    }

    internal void WriteBytes(ReadOnlySpan<byte> values)
    {
        values.CopyTo(_buffer.GetSpan(values.Length));
        _buffer.Advance(values.Length);
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

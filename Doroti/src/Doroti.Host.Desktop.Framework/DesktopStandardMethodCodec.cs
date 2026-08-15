using System.Buffers.Binary;
using System.Text;

namespace Doroti.Host.Desktop.Framework;

/// <summary>
/// Host-side subset of Flutter's StandardMethodCodec. The desktop embedder owns
/// framework channels and therefore cannot depend on the generated Services assembly.
/// </summary>
internal static class DesktopStandardMethodCodec
{
    internal readonly record struct MethodCall(string Method, object? Arguments);

    internal static MethodCall DecodeMethodCall(ReadOnlyMemory<byte>? message)
    {
        if (message is null)
        {
            throw new FormatException("Expected a StandardMethodCodec method call.");
        }
        var reader = new Reader(message.Value.Span);
        var method = reader.ReadValue() as string
            ?? throw new FormatException("StandardMethodCodec method name is not a string.");
        var arguments = reader.ReadValue();
        if (!reader.AtEnd)
        {
            throw new FormatException("StandardMethodCodec method call contains trailing bytes.");
        }
        return new(method, arguments);
    }

    internal static ReadOnlyMemory<byte> EncodeSuccessEnvelope() => new byte[] { 0, 0 };

    internal static ReadOnlyMemory<byte> EncodeErrorEnvelope(string code, string message)
    {
        using var stream = new MemoryStream();
        stream.WriteByte(1);
        WriteString(stream, code);
        WriteString(stream, message);
        stream.WriteByte(0);
        return stream.ToArray();
    }

    private static void WriteString(Stream stream, string value)
    {
        stream.WriteByte(7);
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteSize(stream, bytes.Length);
        stream.Write(bytes);
    }

    private static void WriteSize(Stream stream, int value)
    {
        if (value < 254)
        {
            stream.WriteByte((byte)value);
            return;
        }
        Span<byte> buffer = stackalloc byte[4];
        if (value <= ushort.MaxValue)
        {
            stream.WriteByte(254);
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)value);
            stream.Write(buffer[..2]);
            return;
        }
        stream.WriteByte(255);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)value);
        stream.Write(buffer);
    }

    private ref struct Reader(ReadOnlySpan<byte> bytes)
    {
        private readonly ReadOnlySpan<byte> _bytes = bytes;
        private int _offset;

        internal bool AtEnd => _offset == _bytes.Length;

        internal object? ReadValue()
        {
            var type = ReadByte();
            return type switch
            {
                0 => null,
                1 => true,
                2 => false,
                3 => ReadInt32(),
                4 => ReadInt64(),
                6 => ReadDouble(),
                7 => ReadString(),
                8 => ReadBytes(),
                12 => ReadList(),
                13 => ReadMap(),
                _ => throw new FormatException($"Unsupported StandardMessageCodec value type {type}."),
            };
        }

        private byte ReadByte()
        {
            Ensure(1);
            return _bytes[_offset++];
        }

        private int ReadInt32()
        {
            Align(4);
            Ensure(4);
            var value = BinaryPrimitives.ReadInt32LittleEndian(_bytes[_offset..]);
            _offset += 4;
            return value;
        }

        private long ReadInt64()
        {
            Align(8);
            Ensure(8);
            var value = BinaryPrimitives.ReadInt64LittleEndian(_bytes[_offset..]);
            _offset += 8;
            return value;
        }

        private double ReadDouble() => BitConverter.Int64BitsToDouble(ReadInt64());

        private string ReadString()
        {
            var length = ReadSize();
            Ensure(length);
            var value = Encoding.UTF8.GetString(_bytes.Slice(_offset, length));
            _offset += length;
            return value;
        }

        private byte[] ReadBytes()
        {
            var length = ReadSize();
            Ensure(length);
            var value = _bytes.Slice(_offset, length).ToArray();
            _offset += length;
            return value;
        }

        private object?[] ReadList()
        {
            var length = ReadSize();
            var result = new object?[length];
            for (var index = 0; index < length; index++) result[index] = ReadValue();
            return result;
        }

        private IReadOnlyDictionary<object, object?> ReadMap()
        {
            var length = ReadSize();
            var result = new Dictionary<object, object?>();
            for (var index = 0; index < length; index++)
            {
                var key = ReadValue() ?? throw new FormatException("StandardMessageCodec map keys cannot be null in host channels.");
                result.Add(key, ReadValue());
            }
            return result;
        }

        private int ReadSize()
        {
            var value = ReadByte();
            if (value < 254) return value;
            if (value == 254)
            {
                Ensure(2);
                var size = BinaryPrimitives.ReadUInt16LittleEndian(_bytes[_offset..]);
                _offset += 2;
                return size;
            }
            Ensure(4);
            var wide = BinaryPrimitives.ReadUInt32LittleEndian(_bytes[_offset..]);
            _offset += 4;
            if (wide > int.MaxValue) throw new FormatException("StandardMessageCodec value is too large.");
            return (int)wide;
        }

        private void Align(int alignment)
        {
            var remainder = _offset % alignment;
            if (remainder != 0) _offset += alignment - remainder;
        }

        private void Ensure(int length)
        {
            if (length < 0 || _offset > _bytes.Length - length)
                throw new FormatException("StandardMethodCodec message is truncated.");
        }
    }
}

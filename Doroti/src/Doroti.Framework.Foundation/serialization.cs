// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/serialization.dart
using System.Buffers.Binary;
using Doroti.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public enum Endian
{
    little,
    big,
    host,
}

public sealed class WriteBuffer
{
    private delegate void SpanWriter(Span<byte> span);

    private byte[] _buffer;
    private int _size;
    private bool _done;

    public WriteBuffer(int startCapacity = 8)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startCapacity, 1);
        _buffer = new byte[startCapacity];
    }

    public WriteBuffer(long startCapacity) : this(checked((int)startCapacity)) { }

    public void putUint8(int value) => Write(1, span => span[0] = checked((byte)value));
    public void putUint8(long value) => putUint8(checked((int)value));
    public void putUint16(int value, Endian endian = Endian.host)
    {
        Align(2);
        Write(2, span => WriteUInt16(span, checked((ushort)value), endian));
    }
    public void putUint16(long value, Endian endian = Endian.host) => putUint16(checked((int)value), endian);
    public void putUint32(uint value, Endian endian = Endian.host)
    {
        Align(4);
        Write(4, span => WriteUInt32(span, value, endian));
    }
    public void putUint32(long value, Endian endian = Endian.host) => putUint32(checked((uint)value), endian);
    public void putInt32(int value, Endian endian = Endian.host)
    {
        Align(4);
        Write(4, span => WriteInt32(span, value, endian));
    }
    public void putInt32(long value, Endian endian = Endian.host) => putInt32(checked((int)value), endian);
    public void putInt64(long value, Endian endian = Endian.host)
    {
        Align(8);
        Write(8, span => WriteInt64(span, value, endian));
    }

    public void putFloat64(double value, Endian endian = Endian.host)
    {
        Align(8);
        putInt64(BitConverter.DoubleToInt64Bits(value), endian);
    }

    public void putUint8List(ReadOnlySpan<byte> values) => Append(values);
    public void putUint8List(Uint8List values) => Append(values.Select(value => checked((byte)value)).ToArray());
    public void putInt32List(Int32List values, Endian endian = Endian.host) => putInt32List(values.ToArray(), endian);
    public void putInt64List(Int64List values, Endian endian = Endian.host) => putInt64List(values.ToArray(), endian);
    public void putFloat32List(Float32List values, Endian endian = Endian.host) => putFloat32List(values.ToArray(), endian);
    public void putFloat64List(Float64List values, Endian endian = Endian.host) => putFloat64List(values.ToArray(), endian);

    public void putInt32List(ReadOnlySpan<int> values, Endian endian = Endian.host)
    {
        Align(4);
        foreach (var value in values)
        {
            putInt32(value, endian);
        }
    }

    public void putInt64List(ReadOnlySpan<long> values, Endian endian = Endian.host)
    {
        Align(8);
        foreach (var value in values)
        {
            putInt64(value, endian);
        }
    }

    public void putFloat32List(ReadOnlySpan<float> values, Endian endian = Endian.host)
    {
        Align(4);
        foreach (var value in values)
        {
            putInt32(BitConverter.SingleToInt32Bits(value), endian);
        }
    }

    public void putFloat64List(ReadOnlySpan<double> values, Endian endian = Endian.host)
    {
        Align(8);
        foreach (var value in values)
        {
            putInt64(BitConverter.DoubleToInt64Bits(value), endian);
        }
    }

    public ReadOnlyMemory<byte> done()
    {
        if (_done)
        {
            throw new InvalidOperationException($"done() must not be called more than once on the same {GetType().Name}.");
        }
        _done = true;
        var result = _buffer.AsMemory(0, _size).ToArray();
        _buffer = [];
        return result;
    }

    private void Write(int length, SpanWriter write)
    {
        EnsureWritable(length);
        write(_buffer.AsSpan(_size, length));
        _size += length;
    }

    private void Append(ReadOnlySpan<byte> values)
    {
        EnsureWritable(values.Length);
        values.CopyTo(_buffer.AsSpan(_size));
        _size += values.Length;
    }

    private void Align(int alignment)
    {
        var padding = (_size % alignment) is 0 ? 0 : alignment - (_size % alignment);
        EnsureWritable(padding);
        _buffer.AsSpan(_size, padding).Clear();
        _size += padding;
    }

    private void EnsureWritable(int additional)
    {
        if (_done)
        {
            throw new InvalidOperationException("The buffer has already been finalized.");
        }
        var required = checked(_size + additional);
        if (required > _buffer.Length)
        {
            Array.Resize(ref _buffer, Math.Max(required, _buffer.Length * 2));
        }
    }

    internal static bool Little(Endian endian) => endian == Endian.little || endian == Endian.host && BitConverter.IsLittleEndian;
    internal static void WriteUInt16(Span<byte> span, ushort value, Endian endian) { if (Little(endian)) BinaryPrimitives.WriteUInt16LittleEndian(span, value); else BinaryPrimitives.WriteUInt16BigEndian(span, value); }
    internal static void WriteUInt32(Span<byte> span, uint value, Endian endian) { if (Little(endian)) BinaryPrimitives.WriteUInt32LittleEndian(span, value); else BinaryPrimitives.WriteUInt32BigEndian(span, value); }
    internal static void WriteInt32(Span<byte> span, int value, Endian endian) { if (Little(endian)) BinaryPrimitives.WriteInt32LittleEndian(span, value); else BinaryPrimitives.WriteInt32BigEndian(span, value); }
    internal static void WriteInt64(Span<byte> span, long value, Endian endian) { if (Little(endian)) BinaryPrimitives.WriteInt64LittleEndian(span, value); else BinaryPrimitives.WriteInt64BigEndian(span, value); }
}

public sealed class ReadBuffer
{
    private readonly ReadOnlyMemory<byte> _data;
    private int _position;

    public ReadBuffer(ReadOnlyMemory<byte> data) => _data = data;
    public ReadBuffer(ByteData data) : this(data.asMemory()) { }

    public bool hasRemaining => _position < _data.Length;
    public int getUint8() => Read(1)[0];
    public int getUint16(Endian endian = Endian.host) => WriteBuffer.Little(endian) ? BinaryPrimitives.ReadUInt16LittleEndian(Read(2)) : BinaryPrimitives.ReadUInt16BigEndian(Read(2));
    public uint getUint32(Endian endian = Endian.host) => WriteBuffer.Little(endian) ? BinaryPrimitives.ReadUInt32LittleEndian(Read(4)) : BinaryPrimitives.ReadUInt32BigEndian(Read(4));
    public int getInt32(Endian endian = Endian.host) => WriteBuffer.Little(endian) ? BinaryPrimitives.ReadInt32LittleEndian(Read(4)) : BinaryPrimitives.ReadInt32BigEndian(Read(4));
    public long getInt64(Endian endian = Endian.host) => WriteBuffer.Little(endian) ? BinaryPrimitives.ReadInt64LittleEndian(Read(8)) : BinaryPrimitives.ReadInt64BigEndian(Read(8));

    public double getFloat64(Endian endian = Endian.host)
    {
        Align(8);
        return BitConverter.Int64BitsToDouble(getInt64(endian));
    }

    public ReadOnlyMemory<byte> getUint8List(int length) => ReadMemory(length);
    public ReadOnlyMemory<byte> getUint8List(long length) => getUint8List(checked((int)length));

    public int[] getInt32List(int length, Endian endian = Endian.host)
    {
        Align(4);
        return Enumerable.Range(0, length).Select(_ => getInt32(endian)).ToArray();
    }
    public int[] getInt32List(long length, Endian endian = Endian.host) => getInt32List(checked((int)length), endian);

    public long[] getInt64List(int length, Endian endian = Endian.host)
    {
        Align(8);
        return Enumerable.Range(0, length).Select(_ => getInt64(endian)).ToArray();
    }
    public long[] getInt64List(long length, Endian endian = Endian.host) => getInt64List(checked((int)length), endian);

    public float[] getFloat32List(int length, Endian endian = Endian.host)
    {
        Align(4);
        return Enumerable.Range(0, length).Select(_ => BitConverter.Int32BitsToSingle(getInt32(endian))).ToArray();
    }
    public float[] getFloat32List(long length, Endian endian = Endian.host) => getFloat32List(checked((int)length), endian);

    public double[] getFloat64List(int length, Endian endian = Endian.host)
    {
        Align(8);
        return Enumerable.Range(0, length).Select(_ => BitConverter.Int64BitsToDouble(getInt64(endian))).ToArray();
    }
    public double[] getFloat64List(long length, Endian endian = Endian.host) => getFloat64List(checked((int)length), endian);

    private ReadOnlySpan<byte> Read(int length) => ReadMemory(length).Span;

    private ReadOnlyMemory<byte> ReadMemory(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        if (_position + length > _data.Length)
        {
            throw new InvalidOperationException("The read buffer does not contain enough data.");
        }
        var result = _data.Slice(_position, length);
        _position += length;
        return result;
    }

    private void Align(int alignment)
    {
        var padding = (_position % alignment) is 0 ? 0 : alignment - (_position % alignment);
        _ = ReadMemory(padding);
    }
}

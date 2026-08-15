using System.Buffers.Binary;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Doroti.Runtime;

/// <summary>Managed ownership for the dart:typed_data byte buffer contract.</summary>
public sealed class ByteBuffer
{
    private readonly byte[] _bytes;

    public ByteBuffer(byte[] bytes) => _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));

    public long lengthInBytes => _bytes.LongLength;

    public ByteData asByteData(long offsetInBytes = 0, long? length = null) =>
        new(this, offsetInBytes, length ?? (lengthInBytes - offsetInBytes));
    public Uint8List asUint8List(long offsetInBytes = 0, long? length = null) =>
        new(this, offsetInBytes, length ?? (lengthInBytes - offsetInBytes));
    public List<long> asUint32List(long offsetInBytes = 0, long? length = null)
    {
        var count = length ?? ((lengthInBytes - offsetInBytes) / 4);
        var result = new List<long>(checked((int)count));
        for (var index = 0L; index < count; index++)
        {
            result.Add(BinaryPrimitives.ReadUInt32LittleEndian(Slice(offsetInBytes + (index * 4), 4)));
        }
        return result;
    }

    internal ReadOnlySpan<byte> Slice(long offset, long length) =>
        _bytes.AsSpan(checked((int)offset), checked((int)length));

    internal byte Get(long index) => _bytes[checked((int)index)];

    internal void Set(long index, byte value) => _bytes[checked((int)index)] = value;
}

public sealed class ByteData
{
    public ByteData(Uint8List bytes)
        : this(bytes?.buffer ?? throw new ArgumentNullException(nameof(bytes)), bytes.offsetInBytes, bytes.lengthInBytes) { }

    public ByteData(ByteBuffer buffer, long offsetInBytes = 0, long? lengthInBytes = null)
    {
        this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        this.offsetInBytes = offsetInBytes;
        this.lengthInBytes = lengthInBytes ?? (buffer.lengthInBytes - offsetInBytes);
        if (offsetInBytes < 0 || this.lengthInBytes < 0 || offsetInBytes + this.lengthInBytes > buffer.lengthInBytes)
        {
            throw new ArgumentOutOfRangeException(nameof(offsetInBytes));
        }
    }

    public ByteBuffer buffer { get; }
    public long offsetInBytes { get; }
    public long lengthInBytes { get; }

    public ReadOnlyMemory<byte> asMemory() => buffer.Slice(offsetInBytes, lengthInBytes).ToArray();
    public long getUint32(long byteOffset) =>
        BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(offsetInBytes + byteOffset, 4));
    public static implicit operator ReadOnlyMemory<byte>(ByteData value) => value.asMemory();
    public static implicit operator ByteData(ReadOnlyMemory<byte> value) => new(new Uint8List(value.ToArray()));
}

/// <summary>Dart Uint8List with view semantics over a shared ByteBuffer.</summary>
public sealed class Uint8List : IList<long>, IReadOnlyList<long>
{
    public static implicit operator List<long>(Uint8List value) => value.ToList();
    public Uint8List(long length) : this(new ByteBuffer(new byte[checked((int)length)]), 0, length) { }

    public Uint8List(IEnumerable<long> values)
        : this((values ?? throw new ArgumentNullException(nameof(values))).Select(value => checked((byte)value)).ToArray()) { }

    public Uint8List(IEnumerable<object> values)
        : this((values ?? throw new ArgumentNullException(nameof(values))).Select(Convert.ToInt64)) { }

    public Uint8List(byte[] values) : this(new ByteBuffer(values.ToArray()), 0, values.LongLength) { }

    public Uint8List(ByteBuffer buffer)
        : this(buffer, 0, buffer?.lengthInBytes ?? throw new ArgumentNullException(nameof(buffer))) { }

    public Uint8List(ByteData data)
        : this(data?.buffer ?? throw new ArgumentNullException(nameof(data)), data.offsetInBytes, data.lengthInBytes) { }

    public Uint8List(Uint8List data, long offset, long length)
        : this(data?.buffer ?? throw new ArgumentNullException(nameof(data)), data.offsetInBytes + offset, length) { }

    public Uint8List(ByteBuffer buffer, long offsetInBytes, long length)
    {
        this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        this.offsetInBytes = offsetInBytes;
        lengthInBytes = length;
        if (offsetInBytes < 0 || length < 0 || offsetInBytes + length > buffer.lengthInBytes)
        {
            throw new ArgumentOutOfRangeException(nameof(offsetInBytes));
        }
    }

    public ByteBuffer buffer { get; }
    public long offsetInBytes { get; }
    public long lengthInBytes { get; }
    public bool isNotEmpty => Count != 0;
    public int Count => checked((int)lengthInBytes);
    public bool IsReadOnly => false;

    public long this[int index]
    {
        get => buffer.Get(offsetInBytes + index);
        set => buffer.Set(offsetInBytes + index, checked((byte)value));
    }

    public long this[long index]
    {
        get => this[checked((int)index)];
        set => this[checked((int)index)] = value;
    }

    public IEnumerator<long> GetEnumerator()
    {
        for (var index = 0; index < Count; index++) yield return this[index];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int IndexOf(long item) => ((IEnumerable<long>)this).Select((value, index) => (value, index)).FirstOrDefault(pair => pair.value == item, (0, -1)).index;
    public bool Contains(long item) => IndexOf(item) >= 0;
    public void CopyTo(long[] array, int arrayIndex) { for (var i = 0; i < Count; i++) array[arrayIndex + i] = this[i]; }
    public void Clear() { for (var i = 0; i < Count; i++) this[i] = 0; }
    public void Add(long item) => throw new NotSupportedException("A typed-data list has fixed length.");
    public void Insert(int index, long item) => throw new NotSupportedException("A typed-data list has fixed length.");
    public bool Remove(long item) => throw new NotSupportedException("A typed-data list has fixed length.");
    public void RemoveAt(int index) => throw new NotSupportedException("A typed-data list has fixed length.");
}

public abstract class DartTypedList<T> : IList<T>, IReadOnlyList<T>
{
    private readonly T[] _values;
    protected DartTypedList(long length) => _values = new T[checked((int)length)];
    protected DartTypedList(IEnumerable<T> values) => _values = values.ToArray();
    public int Count => _values.Length;
    public bool IsReadOnly => false;
    public T this[int index] { get => _values[index]; set => _values[index] = value; }
    public T this[long index] { get => _values[checked((int)index)]; set => _values[checked((int)index)] = value; }
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int IndexOf(T item) => Array.IndexOf(_values, item);
    public bool Contains(T item) => _values.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);
    public void Clear() => Array.Clear(_values);
    public void Add(T item) => throw new NotSupportedException("A typed-data list has fixed length.");
    public void Insert(int index, T item) => throw new NotSupportedException("A typed-data list has fixed length.");
    public bool Remove(T item) => throw new NotSupportedException("A typed-data list has fixed length.");
    public void RemoveAt(int index) => throw new NotSupportedException("A typed-data list has fixed length.");
    public T[] ToArray() => _values.ToArray();
}

public sealed class Int32List : DartTypedList<int>
{
    public Int32List(long length) : base(length) { }
    public Int32List(IEnumerable<int> values) : base(values) { }
    public Int32List(IEnumerable<long> values) : base(values.Select(value => checked((int)value))) { }
}
public sealed class Int64List : DartTypedList<long> { public Int64List(long length) : base(length) { } public Int64List(IEnumerable<long> values) : base(values) { } }
public sealed class Float32List : DartTypedList<float> { public Float32List(long length) : base(length) { } public Float32List(IEnumerable<float> values) : base(values) { } }
public sealed class Float64List : DartTypedList<double> { public Float64List(long length) : base(length) { } public Float64List(IEnumerable<double> values) : base(values) { } }

public sealed class ReceivePort
{
    public SendPort sendPort { get; } = new();
    public void listen(Action<object?> listener) => sendPort.Listen(listener);
}

public sealed class SendPort
{
    private Action<object?>? _listener;
    internal void Listen(Action<object?> listener) => _listener = listener ?? throw new ArgumentNullException(nameof(listener));
    public void send(object? message) => (_listener ?? throw new InvalidOperationException("No ReceivePort listener is registered."))(message);
}

public sealed class Expando<T> where T : class
{
    private readonly ConditionalWeakTable<object, Holder> _values = new();
    public T? this[object key]
    {
        get => _values.TryGetValue(key, out var holder) ? holder.Value : null;
        set
        {
            ArgumentNullException.ThrowIfNull(key);
            _values.Remove(key);
            if (value is not null) _values.Add(key, new Holder(value));
        }
    }
    private sealed record Holder(T Value);
}

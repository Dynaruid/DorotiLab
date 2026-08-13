using Doroti.Flutter.Runtime;

namespace Doroti.Flutter.Ui;

public enum TextDirection { rtl, ltr }
public enum TextAffinity { upstream, downstream }
public enum TextAlign { left, right, center, justify, start, end }
public enum KeyEventDeviceType { keyboard, directionalPad, gamepad, joystick, hdmi }
public enum AppExitType { cancelable, required }
public enum AppExitResponse { exit, cancel }

public readonly record struct FontWeight(int value)
{
    public static FontWeight w100 { get; } = new(100);
    public static FontWeight w200 { get; } = new(200);
    public static FontWeight w300 { get; } = new(300);
    public static FontWeight w400 { get; } = new(400);
    public static FontWeight w500 { get; } = new(500);
    public static FontWeight w600 { get; } = new(600);
    public static FontWeight w700 { get; } = new(700);
    public static FontWeight w800 { get; } = new(800);
    public static FontWeight w900 { get; } = new(900);
    public static IReadOnlyList<FontWeight> values { get; } = [w100, w200, w300, w400, w500, w600, w700, w800, w900];
    public static FontWeight normal => w400;
    public static FontWeight bold => w700;
}

public class TextRange
{
    public TextRange(long start, long? end = null)
    {
        this.start = start;
        this.end = end ?? start;
    }

    public TextRange(IReadOnlyDictionary<string, object?> encoded)
        : this(Convert.ToInt64(encoded["start"]), Convert.ToInt64(encoded["end"])) { }

    public static TextRange empty { get; } = new(-1, -1);
    public long start { get; }
    public long end { get; }
    public bool isValid => start >= 0 && end >= 0;
    public bool isCollapsed => start == end;
    public bool isNormalized => start <= end;
    public string textBefore(string text) => text[..checked((int)start)];
    public string textInside(string text) => text[checked((int)start)..checked((int)end)];
    public string textAfter(string text) => text[checked((int)end)..];
}

public sealed record TextPosition(long offset, TextAffinity affinity = TextAffinity.downstream);

public sealed class ImmutableBuffer : IDisposable
{
    private readonly byte[] _bytes;
    private bool _disposed;

    private ImmutableBuffer(byte[] bytes) => _bytes = bytes;
    public long length => _bytes.LongLength;
    public static Future<ImmutableBuffer> fromUint8List(Uint8List bytes) =>
        Future<ImmutableBuffer>.value(new ImmutableBuffer(bytes.Select(value => checked((byte)value)).ToArray()));
    public static Future<ImmutableBuffer> fromAsset(string key) =>
        Future<ImmutableBuffer>.error(new FlutterCapabilityException(
            FlutterCapabilityIds.PlatformMessaging, null,
            DartUiInvocation.Managed($"dart:ui#ImmutableBuffer.fromAsset({key})"),
            "asset loading requires a registered platform asset capability"));
    public static Future<ImmutableBuffer> fromFilePath(string path) =>
        Future<ImmutableBuffer>.fromTask(ReadFileAsync(path));
    private static async Task<ImmutableBuffer> ReadFileAsync(string path) => new(await File.ReadAllBytesAsync(path));
    public ReadOnlyMemory<byte> asMemory()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _bytes;
    }
    public void Dispose() => _disposed = true;
}

public enum ViewFocusState { unfocused, focused }

public enum ViewFocusDirection { undefined, forward, backward }

public sealed record ViewFocusEvent(long viewId, ViewFocusState state, ViewFocusDirection direction)
{
    public ViewFocusEvent(ulong viewId, bool hasFocus)
        : this(checked((long)viewId), hasFocus ? ViewFocusState.focused : ViewFocusState.unfocused, ViewFocusDirection.undefined)
    {
    }

    public bool hasFocus => state == ViewFocusState.focused;
}

public class PointerEvent : IPointerEvent
{
    public long pointer { get; init; }
    public long embedderId { get; init; }
    public long platformData { get; init; }
    public Duration timeStamp { get; init; }
    public Offset position { get; init; }
    public PointerDeviceKind kind { get; init; }
    public double orientation { get; init; }
    public double pressure { get; init; }
    public double size { get; init; }
    public double radiusMajor { get; init; }
    public double radiusMinor { get; init; }

    public static PointerEvent FromFrameworkEvent(
        long eventKind,
        long pointer,
        long embedderId,
        long platformData,
        Duration timeStamp,
        Offset position,
        PointerDeviceKind kind,
        double orientation,
        double pressure,
        double size,
        double radiusMajor,
        double radiusMinor)
    {
        PointerEvent result = eventKind switch
        {
            1 => new PointerDownEvent(),
            2 => new PointerUpEvent(),
            3 => new PointerCancelEvent(),
            4 => new PointerHoverEvent(),
            5 => new PointerMoveEvent(),
            _ => new PointerEvent(),
        };
        return withValues();

        PointerEvent withValues() => result switch
        {
            PointerDownEvent => new PointerDownEvent { pointer = pointer, embedderId = embedderId, platformData = platformData, timeStamp = timeStamp, position = position, kind = kind, orientation = orientation, pressure = pressure, size = size, radiusMajor = radiusMajor, radiusMinor = radiusMinor },
            PointerUpEvent => new PointerUpEvent { pointer = pointer, embedderId = embedderId, platformData = platformData, timeStamp = timeStamp, position = position, kind = kind, orientation = orientation, pressure = pressure, size = size, radiusMajor = radiusMajor, radiusMinor = radiusMinor },
            PointerCancelEvent => new PointerCancelEvent { pointer = pointer, embedderId = embedderId, platformData = platformData, timeStamp = timeStamp, position = position, kind = kind, orientation = orientation, pressure = pressure, size = size, radiusMajor = radiusMajor, radiusMinor = radiusMinor },
            PointerHoverEvent => new PointerHoverEvent { pointer = pointer, embedderId = embedderId, platformData = platformData, timeStamp = timeStamp, position = position, kind = kind, orientation = orientation, pressure = pressure, size = size, radiusMajor = radiusMajor, radiusMinor = radiusMinor },
            PointerMoveEvent => new PointerMoveEvent { pointer = pointer, embedderId = embedderId, platformData = platformData, timeStamp = timeStamp, position = position, kind = kind, orientation = orientation, pressure = pressure, size = size, radiusMajor = radiusMajor, radiusMinor = radiusMinor },
            _ => new PointerEvent { pointer = pointer, embedderId = embedderId, platformData = platformData, timeStamp = timeStamp, position = position, kind = kind, orientation = orientation, pressure = pressure, size = size, radiusMajor = radiusMajor, radiusMinor = radiusMinor },
        };
    }
}

public sealed class PointerDownEvent : PointerEvent { }
public sealed class PointerUpEvent : PointerEvent { }
public sealed class PointerCancelEvent : PointerEvent { }
public sealed class PointerHoverEvent : PointerEvent { }
public sealed class PointerMoveEvent : PointerEvent { }
public sealed class PointerRemovedEvent : PointerEvent, IPointerRemovedEvent { }
public sealed class PointerEnterEvent : PointerEvent { }
public sealed class PointerExitEvent : PointerEvent { }

public sealed record Vector3
{
    public Vector3(double x = 0, double y = 0, double z = 0) => (this.x, this.y, this.z) = (x, y, z);

    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }

    public static Vector3 operator -(Vector3 left, Vector3 right) =>
        new(left.x - right.x, left.y - right.y, left.z - right.z);

    public static Vector3 operator +(Vector3 left, Vector3 right) =>
        new(left.x + right.x, left.y + right.y, left.z + right.z);

    public static Vector3 operator *(Vector3 value, double scale) =>
        new(value.x * scale, value.y * scale, value.z * scale);

    public double dot(Vector3 other) => (x * other.x) + (y * other.y) + (z * other.z);
}

public sealed record Quaternion
{
    public Quaternion(double x = 0, double y = 0, double z = 0, double w = 1) =>
        (this.x, this.y, this.z, this.w) = (x, y, z, w);
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
    public double w { get; set; }
    public Quaternion scaled(double scale) => new(x * scale, y * scale, z * scale, w * scale);
    public Quaternion normalized()
    {
        var length = Math.Sqrt((x * x) + (y * y) + (z * z) + (w * w));
        return length == 0 ? new Quaternion() : new(x / length, y / length, z / length, w / length);
    }
    public static Quaternion operator +(Quaternion left, Quaternion right) =>
        new(left.x + right.x, left.y + right.y, left.z + right.z, left.w + right.w);
}

public sealed class Matrix4
{
    private readonly Float64List _storage;
    public Matrix4() : this([
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    ])
    { }

    public Matrix4(IEnumerable<double> values)
    {
        _storage = new Float64List(values);
        if (_storage.Count != 16) throw new ArgumentException("Matrix4 requires sixteen values.", nameof(values));
    }
    public Float64List storage => _storage;
    public static Matrix4 identity() => new();
    public static Matrix4 zero() => new(new double[16]);
    public static Matrix4 copy(Matrix4 other) => other.clone();
    public static Matrix4 inverted(Matrix4 other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return tryInvert(other) ?? throw new ArgumentException("Matrix4 is not invertible.", nameof(other));
    }
    public static Matrix4 translationValues(double x, double y, double z)
    {
        var result = new Matrix4();
        result._storage[12] = x; result._storage[13] = y; result._storage[14] = z;
        return result;
    }
    public static Matrix4 diagonal3Values(double x, double y, double z) => new([x, 0, 0, 0, 0, y, 0, 0, 0, 0, z, 0, 0, 0, 0, 1]);
    public static Matrix4 diagonal3(global::System.Numerics.Vector3 value) => diagonal3Values(value.X, value.Y, value.Z);
    public static Matrix4 diagonal3(Vector3 value) => diagonal3Values(value.x, value.y, value.z);
    public static Matrix4 rotationX(double radians)
    {
        var c = Math.Cos(radians); var s = Math.Sin(radians);
        return new([1, 0, 0, 0, 0, c, s, 0, 0, -s, c, 0, 0, 0, 0, 1]);
    }
    public static Matrix4 rotationY(double radians)
    {
        var c = Math.Cos(radians); var s = Math.Sin(radians);
        return new([c, 0, -s, 0, 0, 1, 0, 0, s, 0, c, 0, 0, 0, 0, 1]);
    }
    public static Matrix4 rotationZ(double radians)
    {
        var result = new Matrix4(); result.rotateZ(radians); return result;
    }
    public static Matrix4 compose(Vector3 translation, Quaternion rotation, Vector3 scale)
    {
        var matrix = global::System.Numerics.Matrix4x4.CreateScale((float)scale.x, (float)scale.y, (float)scale.z) *
            global::System.Numerics.Matrix4x4.CreateFromQuaternion(new global::System.Numerics.Quaternion(
                (float)rotation.x, (float)rotation.y, (float)rotation.z, (float)rotation.w)) *
            global::System.Numerics.Matrix4x4.CreateTranslation((float)translation.x, (float)translation.y, (float)translation.z);
        return FromNumerics(matrix);
    }
    public double entry(long row, long column) => _storage[checked((int)column * 4 + (int)row)];
    public void setEntry(long row, long column, double value) => _storage[checked((int)column * 4 + (int)row)] = value;
    public void setTranslationRaw(double x, double y, double z)
    {
        _storage[12] = x;
        _storage[13] = y;
        _storage[14] = z;
    }

    public Matrix4 clone() => new(_storage);
    public bool isZero() => _storage.All(value => value == 0);

    public void setZero()
    {
        for (var index = 0; index < 16; index++) _storage[index] = 0;
    }

    public void setIdentity()
    {
        setZero();
        _storage[0] = _storage[5] = _storage[10] = _storage[15] = 1;
    }

    public Matrix4 multiplied(Matrix4 rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        var result = new double[16];
        for (var row = 0; row < 4; row++)
            for (var column = 0; column < 4; column++)
                for (var index = 0; index < 4; index++)
                    result[(column * 4) + row] += _storage[(index * 4) + row] * rhs._storage[(column * 4) + index];
        return new(result);
    }
    public static Matrix4 operator *(Matrix4 left, Matrix4 right) => left.multiplied(right);

    public void multiply(Matrix4 rhs) => CopyStorage(multiplied(rhs)._storage, _storage);
    public double invert()
    {
        var inverse = tryInvert(this);
        if (inverse is null) return 0;
        CopyStorage(inverse._storage, _storage);
        return 1;
    }

    public double copyInverse(Matrix4 other)
    {
        ArgumentNullException.ThrowIfNull(other);
        CopyStorage(other._storage, _storage);
        return invert();
    }

    public void translateByDouble(double x, double y, double z, double w) => leftTranslateByDouble(x, y, z, w);

    public void translate(double x, double y, double z = 0) => leftTranslateByDouble(x, y, z, 1);

    public void scaleByDouble(double x, double y, double z, double w) =>
        multiply(new Matrix4([x, 0, 0, 0, 0, y, 0, 0, 0, 0, z, 0, 0, 0, 0, w]));

    public void rotateZ(double radians)
    {
        var cosine = Math.Cos(radians);
        var sine = Math.Sin(radians);
        multiply(new Matrix4([cosine, sine, 0, 0, -sine, cosine, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]));
    }

    public void rotateX(double radians) => multiply(rotationX(radians));

    public void rotateY(double radians) => multiply(rotationY(radians));

    public double determinant
    {
        get
        {
            var matrix = new global::System.Numerics.Matrix4x4(
                (float)_storage[0], (float)_storage[1], (float)_storage[2], (float)_storage[3],
                (float)_storage[4], (float)_storage[5], (float)_storage[6], (float)_storage[7],
                (float)_storage[8], (float)_storage[9], (float)_storage[10], (float)_storage[11],
                (float)_storage[12], (float)_storage[13], (float)_storage[14], (float)_storage[15]);
            return matrix.GetDeterminant();
        }
    }

    public global::System.Numerics.Vector4 transform(global::System.Numerics.Vector4 value) => new(
        (float)((_storage[0] * value.X) + (_storage[4] * value.Y) + (_storage[8] * value.Z) + (_storage[12] * value.W)),
        (float)((_storage[1] * value.X) + (_storage[5] * value.Y) + (_storage[9] * value.Z) + (_storage[13] * value.W)),
        (float)((_storage[2] * value.X) + (_storage[6] * value.Y) + (_storage[10] * value.Z) + (_storage[14] * value.W)),
        (float)((_storage[3] * value.X) + (_storage[7] * value.Y) + (_storage[11] * value.Z) + (_storage[15] * value.W)));

    public void leftTranslateByDouble(double x, double y, double z, double w)
    {
        _ = w;
        var translation = new Matrix4();
        translation._storage[12] = x;
        translation._storage[13] = y;
        translation._storage[14] = z;
        var result = translation.multiplied(this);
        CopyStorage(result._storage, _storage);
    }

    public global::System.Numerics.Vector4 getRow(long row)
    {
        var index = checked((int)row);
        return new((float)_storage[index], (float)_storage[4 + index], (float)_storage[8 + index], (float)_storage[12 + index]);
    }

    public global::System.Numerics.Vector4 getColumn(long column)
    {
        var index = checked((int)column) * 4;
        return new((float)_storage[index], (float)_storage[index + 1], (float)_storage[index + 2], (float)_storage[index + 3]);
    }

    public void setColumn(long column, global::System.Numerics.Vector4 value)
    {
        var index = checked((int)column);
        if ((uint)index >= 4) throw new ArgumentOutOfRangeException(nameof(column));
        _storage[index * 4] = value.X;
        _storage[(index * 4) + 1] = value.Y;
        _storage[(index * 4) + 2] = value.Z;
        _storage[(index * 4) + 3] = value.W;
    }

    public void setRow(long row, global::System.Numerics.Vector4 value)
    {
        var index = checked((int)row);
        if ((uint)index >= 4) throw new ArgumentOutOfRangeException(nameof(row));
        _storage[index] = value.X;
        _storage[4 + index] = value.Y;
        _storage[8 + index] = value.Z;
        _storage[12 + index] = value.W;
    }

    public Vector3 perspectiveTransform(Vector3 value)
    {
        var x = (_storage[0] * value.x) + (_storage[4] * value.y) + (_storage[8] * value.z) + _storage[12];
        var y = (_storage[1] * value.x) + (_storage[5] * value.y) + (_storage[9] * value.z) + _storage[13];
        var z = (_storage[2] * value.x) + (_storage[6] * value.y) + (_storage[10] * value.z) + _storage[14];
        var w = (_storage[3] * value.x) + (_storage[7] * value.y) + (_storage[11] * value.z) + _storage[15];
        return w == 0 || w == 1 ? new(x, y, z) : new(x / w, y / w, z / w);
    }

    public Vector3 transform3(Vector3 value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var transformed = perspectiveTransform(value);
        value.x = transformed.x;
        value.y = transformed.y;
        value.z = transformed.z;
        return value;
    }

    public double getMaxScaleOnAxis()
    {
        var x = (_storage[0] * _storage[0]) + (_storage[1] * _storage[1]) + (_storage[2] * _storage[2]);
        var y = (_storage[4] * _storage[4]) + (_storage[5] * _storage[5]) + (_storage[6] * _storage[6]);
        var z = (_storage[8] * _storage[8]) + (_storage[9] * _storage[9]) + (_storage[10] * _storage[10]);
        return Math.Sqrt(Math.Max(x, Math.Max(y, z)));
    }

    public Vector3 getTranslation() => new(_storage[12], _storage[13], _storage[14]);

    public void setTranslation(Vector3 translation)
    {
        _storage[12] = translation.x;
        _storage[13] = translation.y;
        _storage[14] = translation.z;
    }

    public bool isIdentity() => _storage.SequenceEqual(identity()._storage);

    public bool decompose(Vector3 translation, Quaternion rotation, Vector3 scale)
    {
        var matrix = ToNumerics();
        if (!global::System.Numerics.Matrix4x4.Decompose(matrix, out var numericScale, out var numericRotation, out var numericTranslation))
        {
            return false;
        }
        (translation.x, translation.y, translation.z) = (numericTranslation.X, numericTranslation.Y, numericTranslation.Z);
        (rotation.x, rotation.y, rotation.z, rotation.w) = (numericRotation.X, numericRotation.Y, numericRotation.Z, numericRotation.W);
        (scale.x, scale.y, scale.z) = (numericScale.X, numericScale.Y, numericScale.Z);
        return true;
    }

    public static Matrix4? tryInvert(Matrix4 value)
    {
        var matrix = new global::System.Numerics.Matrix4x4(
            (float)value._storage[0], (float)value._storage[1], (float)value._storage[2], (float)value._storage[3],
            (float)value._storage[4], (float)value._storage[5], (float)value._storage[6], (float)value._storage[7],
            (float)value._storage[8], (float)value._storage[9], (float)value._storage[10], (float)value._storage[11],
            (float)value._storage[12], (float)value._storage[13], (float)value._storage[14], (float)value._storage[15]);
        if (!global::System.Numerics.Matrix4x4.Invert(matrix, out var inverse)) return null;
        return new Matrix4([
            inverse.M11, inverse.M12, inverse.M13, inverse.M14,
            inverse.M21, inverse.M22, inverse.M23, inverse.M24,
            inverse.M31, inverse.M32, inverse.M33, inverse.M34,
            inverse.M41, inverse.M42, inverse.M43, inverse.M44,
        ]);
    }

    private static void CopyStorage(Float64List source, Float64List destination)
    {
        for (var index = 0; index < 16; index++) destination[index] = source[index];
    }

    private global::System.Numerics.Matrix4x4 ToNumerics() => new(
        (float)_storage[0], (float)_storage[1], (float)_storage[2], (float)_storage[3],
        (float)_storage[4], (float)_storage[5], (float)_storage[6], (float)_storage[7],
        (float)_storage[8], (float)_storage[9], (float)_storage[10], (float)_storage[11],
        (float)_storage[12], (float)_storage[13], (float)_storage[14], (float)_storage[15]);

    private static Matrix4 FromNumerics(global::System.Numerics.Matrix4x4 value) => new([
        value.M11, value.M12, value.M13, value.M14,
        value.M21, value.M22, value.M23, value.M24,
        value.M31, value.M32, value.M33, value.M34,
        value.M41, value.M42, value.M43, value.M44,
    ]);
}

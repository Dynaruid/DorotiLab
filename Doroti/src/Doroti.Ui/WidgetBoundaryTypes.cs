using Doroti.Runtime;

namespace Doroti.Ui;

/// <summary>dart:ui and vector-math value types consumed by the Widgets source port.</summary>
public enum ClipOp { difference, intersect }
public enum PointMode { points, lines, polygon }
public enum DisplayFeatureType { unknown, fold, hinge, cutout }
public enum DisplayFeatureState { unknown, postureFlat, postureHalfOpened }

public readonly record struct RSTransform(double scos, double ssin, double tx, double ty)
{
    public static RSTransform fromComponents(
        double rotation,
        double scale,
        double anchorX,
        double anchorY,
        double translateX,
        double translateY)
    {
        var sin = Math.Sin(rotation) * scale;
        var cos = Math.Cos(rotation) * scale;
        return new(cos, sin, translateX - (cos * anchorX) + (sin * anchorY),
            translateY - (sin * anchorX) - (cos * anchorY));
    }
}

public sealed record DisplayFeature(Rect bounds, DisplayFeatureType type, DisplayFeatureState state);

public sealed class Vertices;

public sealed class FragmentProgram
{
    private FragmentProgram(string debugName, string source)
    {
        this.debugName = debugName;
        this.source = source;
    }

    public string debugName { get; }
    internal string source { get; }

    public static Future<FragmentProgram> fromAsset(string assetKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetKey);
        return Future<FragmentProgram>.fromTask(LoadAsync(assetKey));
    }

    public static FragmentProgram fromSource(string source, string debugName = "inline")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(debugName);
        return new(debugName, source);
    }

    private static async Task<FragmentProgram> LoadAsync(string assetKey)
    {
        var bytes = await PlatformDispatcher.instance.LoadApplicationResourceAsync(
            assetKey, DartUiInvocation.Managed($"dart:ui#FragmentProgram.fromAsset({assetKey})"));
        var source = System.Text.Encoding.UTF8.GetString(bytes.Span);
        if (string.IsNullOrWhiteSpace(source))
            throw new InvalidDataException($"Fragment program asset '{assetKey}' is empty.");
        return new(assetKey, source);
    }

    public FragmentShader fragmentShader() => new(this);
}

public sealed class FragmentShader(FragmentProgram program) : Shader
{
    private readonly object _gate = new();
    private readonly List<double> _floats = [];
    private readonly Dictionary<long, Image> _samplers = [];
    private long _revision;
    private bool _disposed;

    public FragmentProgram program { get; } = program;

    public void setFloat(long index, double value)
    {
        if (index < 0 || index > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(index));
        if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var alreadyDefined = _floats.Count > index;
            while (_floats.Count <= index) _floats.Add(0);
            var targetIndex = checked((int)index);
            if (alreadyDefined && _floats[targetIndex].Equals(value)) return;
            _floats[targetIndex] = value;
            _revision++;
        }
    }

    public void setImageSampler(long index, Image image)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        ArgumentNullException.ThrowIfNull(image);
        ObjectDisposedException.ThrowIf(image.debugDisposed, image);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_samplers.TryGetValue(index, out var current) && ReferenceEquals(current, image)) return;
            _samplers[index] = image;
            _revision++;
        }
    }

    internal long revision
    {
        get
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                return _revision;
            }
        }
    }

    internal FragmentShaderState CaptureState()
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return new(program.debugName, program.source, _floats.ToArray(),
                _samplers.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.Value));
        }
    }

    public void dispose()
    {
        lock (_gate)
        {
            _disposed = true;
            _floats.Clear();
            _samplers.Clear();
        }
    }
}

internal sealed record FragmentShaderState(
    string DebugName,
    string Source,
    IReadOnlyList<double> Floats,
    IReadOnlyDictionary<long, Image> Samplers);

public sealed record Quad(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3);

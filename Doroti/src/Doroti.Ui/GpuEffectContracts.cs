using System.Security.Cryptography;

namespace Doroti.Ui;

/// <summary>An immutable, build-validated effect variant. Raw GPU objects are host-owned.</summary>
public sealed class GpuEffectProgram
{
    private readonly Dictionary<string, GpuEffectVariant> _variants;
    public const int AbiVersion = 1;
    public string AssetId { get; }
    public string EntryPoint => _variants.Values.First().EntryPoint;
    public string Profile => _variants.Keys.First();
    public string ContentHash { get; }
    public int ParameterByteCount { get; }
    public ReadOnlySpan<byte> Vertex => _variants.Values.First().Vertex;
    public ReadOnlySpan<byte> Fragment => _variants.Values.First().Fragment;

    public GpuEffectProgram(string assetId, string profile, string entryPoint,
        ReadOnlySpan<byte> vertex, ReadOnlySpan<byte> fragment, int parameterByteCount = 0)
        : this(assetId, parameterByteCount, new GpuEffectVariant(profile, entryPoint, vertex, fragment)) { }

    public GpuEffectProgram(string assetId, int parameterByteCount, params GpuEffectVariant[] variants)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetId);
        if (variants.Length == 0) throw new ArgumentException("A program requires at least one variant.");
        _variants = variants.ToDictionary(v => v.Profile, StringComparer.Ordinal);
        AssetId = assetId;
        ArgumentOutOfRangeException.ThrowIfNegative(parameterByteCount);
        ParameterByteCount = parameterByteCount;
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(System.Text.Encoding.UTF8.GetBytes($"{AbiVersion}\0{parameterByteCount}\0"));
        foreach (var variant in _variants.Values.OrderBy(v => v.Profile, StringComparer.Ordinal))
        {
            hash.AppendData(System.Text.Encoding.UTF8.GetBytes($"{variant.Profile}\0{variant.EntryPoint}\0{variant.BindingMetadata}\0"));
            hash.AppendData(variant.Vertex);
            hash.AppendData(variant.Fragment);
        }
        ContentHash = Convert.ToHexStringLower(hash.GetHashAndReset());
    }

    public GpuEffectVariant GetVariant(string profile) => _variants.TryGetValue(profile, out var variant)
        ? variant : throw new PlatformNotSupportedException($"Effect '{AssetId}' has no '{profile}' variant.");
}

public sealed class GpuEffectVariant
{
    private readonly byte[] _vertex, _fragment;
    public string Profile { get; }
    public string EntryPoint { get; }
    public string BindingMetadata { get; }
    public ReadOnlySpan<byte> Vertex => _vertex;
    public ReadOnlySpan<byte> Fragment => _fragment;
    public GpuEffectVariant(string profile, string entryPoint, ReadOnlySpan<byte> vertex,
        ReadOnlySpan<byte> fragment, string bindingMetadata = "{}")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(profile);
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPoint);
        if (vertex.IsEmpty || fragment.IsEmpty) throw new ArgumentException("Shader variants must not be empty.");
        Profile = profile;
        EntryPoint = entryPoint;
        BindingMetadata = bindingMetadata;
        _vertex = vertex.ToArray();
        _fragment = fragment.ToArray();
    }
}

/// <summary>Generated serializers copy their explicitly laid-out uniform bytes here.</summary>
public sealed class GpuEffectParameters
{
    private readonly byte[] _bytes;
    public static GpuEffectParameters Empty { get; } = new([]);
    public ReadOnlySpan<byte> Bytes => _bytes;
    public float Time { get; }
    public float DeltaTime { get; }
    public GpuEffectParameters(ReadOnlySpan<byte> bytes, float time = 0, float deltaTime = 0)
    {
        if (!float.IsFinite(time) || !float.IsFinite(deltaTime) || time < 0 || deltaTime < 0)
            throw new ArgumentOutOfRangeException(nameof(time), "Effect timing must be finite and nonnegative.");
        _bytes = bytes.ToArray(); Time = time; DeltaTime = deltaTime;
    }
    public GpuEffectParameters WithFrameTiming(float time, float deltaTime) => new(_bytes, time, deltaTime);
}

internal sealed record SceneGpuEffectPayload(GpuEffectProgram Program,
    GpuEffectParameters Parameters, Offset Offset, Rect Bounds, bool IsBackdrop);

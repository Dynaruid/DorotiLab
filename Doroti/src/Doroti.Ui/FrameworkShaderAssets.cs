using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Runtime;

namespace Doroti.Ui;

public sealed record FrameworkShaderUniform(string Name, string Type);

public sealed record FrameworkShaderSampler(string Name, int Index);

public sealed record FrameworkShaderAsset(
    string Id,
    string? FlutterAssetKey,
    string? FlutterSourcePath,
    string? FlutterSourceSha256,
    string AdaptedSourcePath,
    string AdaptedSourceSha256,
    string OwningAssembly,
    string EmbeddedResourceName,
    IReadOnlyList<FrameworkShaderUniform> Uniforms,
    IReadOnlyList<FrameworkShaderSampler> Samplers,
    string License,
    IReadOnlyList<string> TargetSupport
);

/// <summary>
/// Explicit shader descriptor registry shared by package owners and GPU hosts.
/// Optional source pins describe the Flutter reference; Doroti-original shaders have
/// no Flutter reference. The adapted hash protects the packaged
/// Doroti artifact that is actually loaded at runtime.
/// </summary>
public static class FrameworkShaderManifest
{
    public const string SchemaVersion = "doroti.framework-shader-manifest/v1";

    private static readonly object Gate = new();
    private static readonly Dictionary<string, FrameworkShaderAsset> Registered = new(StringComparer.Ordinal);

    public static IReadOnlyList<FrameworkShaderAsset> Assets
    {
        get { lock (Gate) return Array.AsReadOnly(Registered.Values.OrderBy(asset => asset.Id, StringComparer.Ordinal).ToArray()); }
    }

    /// <summary>Atomically register frozen descriptors with their actual embedded-resource assembly.</summary>
    public static void Register(Assembly owner, IEnumerable<FrameworkShaderAsset> descriptors)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(descriptors);
        var ownerName = owner.GetName().Name;
        var frozen = descriptors.Select(asset =>
        {
            ArgumentNullException.ThrowIfNull(asset);
            ArgumentException.ThrowIfNullOrWhiteSpace(asset.Id);
            if (!string.Equals(asset.OwningAssembly, ownerName, StringComparison.Ordinal))
                throw new InvalidOperationException($"Shader '{asset.Id}' has a different resource owner.");
            if (asset.AdaptedSourceSha256.Length != 64 || asset.AdaptedSourceSha256.Any(character => !char.IsAsciiHexDigit(character)))
                throw new ArgumentException($"Shader '{asset.Id}' has an invalid SHA256.");
            ArgumentException.ThrowIfNullOrWhiteSpace(asset.EmbeddedResourceName);
            return asset with
            {
                Uniforms = Array.AsReadOnly(asset.Uniforms.ToArray()),
                Samplers = Array.AsReadOnly(asset.Samplers.ToArray()),
                TargetSupport = Array.AsReadOnly(asset.TargetSupport.ToArray()),
            };
        }).ToArray();
        var batch = new Dictionary<string, FrameworkShaderAsset>(StringComparer.Ordinal);
        foreach (var asset in frozen)
        {
            if (batch.TryGetValue(asset.Id, out var previous) && !Equivalent(previous, asset))
                throw new InvalidOperationException($"Conflicting shader descriptor '{asset.Id}'.");
            batch[asset.Id] = asset;
        }
        lock (Gate)
        {
            foreach (var asset in batch.Values)
                if (Registered.TryGetValue(asset.Id, out var previous) && !Equivalent(previous, asset))
                    throw new InvalidOperationException($"Shader '{asset.Id}' was registered with another owner, hash or ABI.");
            FrameworkShaderLoader.RegisterResourceOwner(owner);
            foreach (var asset in batch.Values) Registered.TryAdd(asset.Id, asset);
        }
    }

    private static bool Equivalent(FrameworkShaderAsset left, FrameworkShaderAsset right) =>
        left with { Uniforms = right.Uniforms, Samplers = right.Samplers, TargetSupport = right.TargetSupport } == right &&
        left.Uniforms.SequenceEqual(right.Uniforms) && left.Samplers.SequenceEqual(right.Samplers) &&
        left.TargetSupport.SequenceEqual(right.TargetSupport);

    public static FrameworkShaderAsset Get(string id)
    {
        lock (Gate) return Registered.TryGetValue(id, out var asset) ? asset :
            throw new KeyNotFoundException($"Shader '{id}' has no explicit owner registration.");
    }
}

public sealed record FrameworkShaderDiagnostic(
    string Code,
    string AssetId,
    string Message,
    Exception? Error = null
);

/// <summary>
/// Shared loader for embedded framework runtime-effect assets, with synchronous
/// renderer access and the existing asynchronous framework API.
/// It verifies the packaged bytes and ABI before exposing a FragmentProgram. A failed
/// load is reported through diagnostics and never converted into a transparent effect.
/// </summary>
public static partial class FrameworkShaderLoader
{
    private static readonly ConcurrentDictionary<string, Assembly> ResourceOwners = new(
        StringComparer.Ordinal
    );

    /// <summary>Register an embedded-resource owner using typeof(Owner).Assembly before loading its shaders.</summary>
    public static void RegisterResourceOwner(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        var name =
            assembly.GetName().Name
            ?? throw new ArgumentException(
                "A resource owner must have an assembly name.",
                nameof(assembly)
            );
        if (!ReferenceEquals(ResourceOwners.GetOrAdd(name, assembly), assembly))
        {
            throw new InvalidOperationException(
                $"Shader resource owner '{name}' is already registered by another assembly."
            );
        }
    }

    private static readonly ConcurrentDictionary<string, Lazy<Task<FragmentProgram>>> ProgramCache =
        new(StringComparer.Ordinal);
    private static readonly ConcurrentQueue<FrameworkShaderDiagnostic> DiagnosticLog = [];

    public static event Action<FrameworkShaderDiagnostic>? Diagnostic;

    public static IReadOnlyList<FrameworkShaderDiagnostic> Diagnostics => DiagnosticLog.ToArray();

    public static Future<FragmentProgram> LoadProgram(string assetId) =>
        Future<FragmentProgram>.fromTask(GetProgramTask(assetId));

    // Embedded assembly bytes are loaded synchronously, so this cached task is
    // already completed (or faulted). Never route external I/O through this path.
    internal static FragmentProgram LoadEmbeddedProgram(string assetId) =>
        GetProgramTask(assetId).GetAwaiter().GetResult();

    private static Task<FragmentProgram> GetProgramTask(string assetId) =>
        ProgramCache
            .GetOrAdd(
                assetId,
                static id => new Lazy<Task<FragmentProgram>>(
                    () => CreateProgramTask(FrameworkShaderManifest.Get(id)),
                    LazyThreadSafetyMode.ExecutionAndPublication
                )
            )
            .Value;

    /// <summary>Starts an asset load and observes both completion and failure.</summary>
    public static void BeginLoad(
        string assetId,
        Action<FragmentProgram> onReady,
        Action<Exception>? onError = null
    )
    {
        ArgumentNullException.ThrowIfNull(onReady);
        _ = ObserveLoadAsync(assetId, onReady, onError);
    }

    internal static void ClearForValidation() => ProgramCache.Clear();

    private static async Task ObserveLoadAsync(
        string assetId,
        Action<FragmentProgram> onReady,
        Action<Exception>? onError
    )
    {
        try
        {
            onReady(await LoadProgram(assetId).asTask().ConfigureAwait(false));
        }
        catch (Exception error)
        {
            Publish(
                new FrameworkShaderDiagnostic(
                    "DOROTI_SHADER_ASSET_LOAD_FAILED",
                    assetId,
                    $"Framework shader asset '{assetId}' could not be loaded or its ABI verified.",
                    error
                )
            );
            if (onError is not null)
            {
                try
                {
                    onError(error);
                }
                catch (Exception callbackError)
                {
                    Publish(
                        new FrameworkShaderDiagnostic(
                            "DOROTI_SHADER_ASSET_ERROR_CALLBACK_FAILED",
                            assetId,
                            "The framework shader error callback failed while reporting the original load error.",
                            callbackError
                        )
                    );
                }
            }
        }
    }

    private static Task<FragmentProgram> CreateProgramTask(FrameworkShaderAsset asset)
    {
        try
        {
            return Task.FromResult(LoadPackagedProgram(asset));
        }
        catch (Exception error)
        {
            // Preserve faulted-task behavior for LoadProgram/BeginLoad callers.
            return Task.FromException<FragmentProgram>(error);
        }
    }

    private static FragmentProgram LoadPackagedProgram(FrameworkShaderAsset asset)
    {
        var assembly = ResolveAssembly(asset.OwningAssembly);
        using var stream =
            assembly.GetManifestResourceStream(asset.EmbeddedResourceName)
            ?? throw new InvalidDataException(
                $"Framework shader '{asset.Id}' is missing embedded resource '{asset.EmbeddedResourceName}'."
            );
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        var bytes = buffer.ToArray();
        var adaptedHash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        if (!string.Equals(adaptedHash, asset.AdaptedSourceSha256, StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"Framework shader '{asset.Id}' packaged hash mismatch: expected {asset.AdaptedSourceSha256}, got {adaptedHash}."
            );
        }

        var source = Encoding.UTF8.GetString(bytes);
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidDataException($"Framework shader '{asset.Id}' is empty.");
        }

        ValidateAbi(asset, source);
        return FragmentProgram.fromSource(source, asset.FlutterAssetKey ?? asset.Id);
    }

    private static Assembly ResolveAssembly(string name) =>
        ResourceOwners.TryGetValue(name, out var assembly)
            ? assembly
            : throw new InvalidOperationException(
                $"Shader resource owner '{name}' is not registered. Call FrameworkShaderLoader.RegisterResourceOwner(typeof(Owner).Assembly) before loading it."
            );

    private static void ValidateAbi(FrameworkShaderAsset asset, string source)
    {
        var uniforms = UniformRegex()
            .Matches(source)
            .Select(match => new FrameworkShaderUniform(
                match.Groups["name"].Value,
                match.Groups["type"].Value.Replace("half", "float", StringComparison.Ordinal)
            ))
            .ToArray();
        var samplers = SamplerRegex()
            .Matches(source)
            .Select((match, index) => new FrameworkShaderSampler(match.Groups["name"].Value, index))
            .ToArray();
        if (!asset.Uniforms.SequenceEqual(uniforms) || !asset.Samplers.SequenceEqual(samplers))
        {
            throw new InvalidDataException(
                $"Framework shader '{asset.Id}' uniform/sampler ABI drifted from the manifest."
            );
        }
    }

    private static void Publish(FrameworkShaderDiagnostic diagnostic)
    {
        DiagnosticLog.Enqueue(diagnostic);
        try
        {
            Diagnostic?.Invoke(diagnostic);
        }
        catch
        {
            // Diagnostics must not hide the original asset failure.
        }
    }

    [GeneratedRegex(
        @"(?m)^\s*uniform\s+(?<type>(?:float|half)(?:[234])?)\s+(?<name>[A-Za-z_]\w*)\s*;"
    )]
    private static partial Regex UniformRegex();

    [GeneratedRegex(@"(?m)^\s*uniform\s+shader\s+(?<name>[A-Za-z_]\w*)\s*;")]
    private static partial Regex SamplerRegex();
}

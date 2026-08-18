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
    string FlutterAssetKey,
    string FlutterSourcePath,
    string FlutterSourceSha256,
    string AdaptedSourcePath,
    string AdaptedSourceSha256,
    string OwningAssembly,
    string EmbeddedResourceName,
    IReadOnlyList<FrameworkShaderUniform> Uniforms,
    IReadOnlyList<FrameworkShaderSampler> Samplers,
    string License,
    IReadOnlyList<string> TargetSupport);

/// <summary>
/// The closed framework shader manifest shared by framework ports and all GPU hosts.
/// Source pins describe the Flutter reference; the adapted hash protects the packaged
/// Doroti artifact that is actually loaded at runtime.
/// </summary>
public static class FrameworkShaderManifest
{
    public const string SchemaVersion = "doroti.framework-shader-manifest/v1";

    private static readonly IReadOnlyList<FrameworkShaderAsset> _assets =
    [
        new FrameworkShaderAsset(
            Id: "material.ink-sparkle",
            FlutterAssetKey: "shaders/ink_sparkle.frag",
            FlutterSourcePath: "packages/flutter/lib/src/material/shaders/ink_sparkle.frag",
            FlutterSourceSha256: "ed126d87b7df031187485bc37345a84aac211ee5efdeeb488ba28f6b0b817592",
            AdaptedSourcePath: "Doroti/src/Doroti.Framework.Material/Shaders/ink_sparkle.sksl",
            AdaptedSourceSha256: "ad41bc223fe55c3d7997a11748e320e891ccbba2395ccdea40d3d09856f96615",
            OwningAssembly: "Doroti.Framework.Material",
            EmbeddedResourceName: "Doroti.Framework.Material.Shaders.ink_sparkle.sksl",
            Uniforms:
            [
                new("u_color", "float4"),
                new("u_composite_1", "float4"),
                new("u_center", "float2"),
                new("u_max_radius", "float"),
                new("u_resolution_scale", "float2"),
                new("u_noise_scale", "float2"),
                new("u_noise_phase", "float"),
                new("u_circle1", "float2"),
                new("u_circle2", "float2"),
                new("u_circle3", "float2"),
                new("u_rotation1", "float2"),
                new("u_rotation2", "float2"),
                new("u_rotation3", "float2"),
            ],
            Samplers: [],
            License: "BSD-3-Clause",
            TargetSupport: ["android", "windows", "maccatalyst", "web"]),
        new FrameworkShaderAsset(
            Id: "widgets.stretch-effect",
            FlutterAssetKey: "shaders/stretch_effect.frag",
            FlutterSourcePath: "packages/flutter/lib/src/widgets/shaders/stretch_effect.frag",
            FlutterSourceSha256: "704a646ce01cebbc525fc4801c7fef3d4fc5c7ed36e3270c73b44ad5d8f0b204",
            AdaptedSourcePath: "Doroti/src/Doroti.Framework.Widgets/Shaders/stretch_effect.sksl",
            AdaptedSourceSha256: "a479b63ed2a17bd2fa8b08cdcc12a5a71f5509c2fdf1dbcf9361456b56d7c910",
            OwningAssembly: "Doroti.Framework.Widgets",
            EmbeddedResourceName: "Doroti.Framework.Widgets.Shaders.stretch_effect.sksl",
            Uniforms:
            [
                new("u_size", "float2"),
                new("u_max_stretch_intensity", "float"),
                new("u_overscroll_x", "float"),
                new("u_overscroll_y", "float"),
                new("u_interpolation_strength", "float"),
            ],
            Samplers: [new("u_texture", 0)],
            License: "BSD-3-Clause",
            TargetSupport: ["android", "windows", "maccatalyst", "web"]),
    ];

    public static IReadOnlyList<FrameworkShaderAsset> Assets => _assets;

    public static FrameworkShaderAsset Get(string id) =>
        _assets.FirstOrDefault(asset => string.Equals(asset.Id, id, StringComparison.Ordinal))
        ?? throw new KeyNotFoundException($"Framework shader '{id}' is not registered in the closed manifest.");
}

public sealed record FrameworkShaderDiagnostic(
    string Code,
    string AssetId,
    string Message,
    Exception? Error = null);

/// <summary>
/// One asynchronous loader for framework and application runtime-effect assets.
/// It verifies the packaged bytes and ABI before exposing a FragmentProgram. A failed
/// load is reported through diagnostics and never converted into a transparent effect.
/// </summary>
public static partial class FrameworkShaderLoader
{
    private static readonly ConcurrentDictionary<string, Lazy<Task<FragmentProgram>>> ProgramCache =
        new(StringComparer.Ordinal);
    private static readonly ConcurrentQueue<FrameworkShaderDiagnostic> DiagnosticLog = [];

    public static event Action<FrameworkShaderDiagnostic>? Diagnostic;

    public static IReadOnlyList<FrameworkShaderDiagnostic> Diagnostics => DiagnosticLog.ToArray();

    public static Future<FragmentProgram> LoadProgram(string assetId)
    {
        var task = ProgramCache.GetOrAdd(
            assetId,
            static id => new Lazy<Task<FragmentProgram>>(
                () => LoadProgramAsync(FrameworkShaderManifest.Get(id)),
                LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        return Future<FragmentProgram>.fromTask(task);
    }

    /// <summary>Starts an asset load and observes both completion and failure.</summary>
    public static void BeginLoad(
        string assetId,
        Action<FragmentProgram> onReady,
        Action<Exception>? onError = null)
    {
        ArgumentNullException.ThrowIfNull(onReady);
        _ = ObserveLoadAsync(assetId, onReady, onError);
    }

    internal static void ClearForValidation() => ProgramCache.Clear();

    private static async Task ObserveLoadAsync(
        string assetId,
        Action<FragmentProgram> onReady,
        Action<Exception>? onError)
    {
        try
        {
            onReady(await LoadProgram(assetId).asTask().ConfigureAwait(false));
        }
        catch (Exception error)
        {
            Publish(new FrameworkShaderDiagnostic(
                "DOROTI_SHADER_ASSET_LOAD_FAILED",
                assetId,
                $"Framework shader asset '{assetId}' could not be loaded or its ABI verified.",
                error));
            if (onError is not null)
            {
                try
                {
                    onError(error);
                }
                catch (Exception callbackError)
                {
                    Publish(new FrameworkShaderDiagnostic(
                        "DOROTI_SHADER_ASSET_ERROR_CALLBACK_FAILED",
                        assetId,
                        "The framework shader error callback failed while reporting the original load error.",
                        callbackError));
                }
            }
        }
    }

    private static async Task<FragmentProgram> LoadProgramAsync(FrameworkShaderAsset asset)
    {
        var assembly = ResolveAssembly(asset.OwningAssembly);
        await using var stream = assembly.GetManifestResourceStream(asset.EmbeddedResourceName)
            ?? throw new InvalidDataException(
                $"Framework shader '{asset.Id}' is missing embedded resource '{asset.EmbeddedResourceName}'.");
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer).ConfigureAwait(false);
        var bytes = buffer.ToArray();
        var adaptedHash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        if (!string.Equals(adaptedHash, asset.AdaptedSourceSha256, StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Framework shader '{asset.Id}' packaged hash mismatch: expected {asset.AdaptedSourceSha256}, got {adaptedHash}.");
        var source = Encoding.UTF8.GetString(bytes);
        if (string.IsNullOrWhiteSpace(source))
            throw new InvalidDataException($"Framework shader '{asset.Id}' is empty.");
        ValidateAbi(asset, source);
        return FragmentProgram.fromSource(source, asset.FlutterAssetKey);
    }

    private static Assembly ResolveAssembly(string name) =>
        AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly =>
            string.Equals(assembly.GetName().Name, name, StringComparison.Ordinal))
        ?? Assembly.Load(new AssemblyName(name));

    private static void ValidateAbi(FrameworkShaderAsset asset, string source)
    {
        var uniforms = UniformRegex().Matches(source)
            .Select(match => new FrameworkShaderUniform(
                match.Groups["name"].Value,
                match.Groups["type"].Value.Replace("half", "float", StringComparison.Ordinal)))
            .ToArray();
        var samplers = SamplerRegex().Matches(source)
            .Select((match, index) => new FrameworkShaderSampler(match.Groups["name"].Value, index))
            .ToArray();
        if (!asset.Uniforms.SequenceEqual(uniforms) || !asset.Samplers.SequenceEqual(samplers))
        {
            throw new InvalidDataException(
                $"Framework shader '{asset.Id}' uniform/sampler ABI drifted from the manifest.");
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

    [GeneratedRegex(@"(?m)^\s*uniform\s+(?<type>(?:float|half)(?:[234])?)\s+(?<name>[A-Za-z_]\w*)\s*;")]
    private static partial Regex UniformRegex();

    [GeneratedRegex(@"(?m)^\s*uniform\s+shader\s+(?<name>[A-Za-z_]\w*)\s*;")]
    private static partial Regex SamplerRegex();
}

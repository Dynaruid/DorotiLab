using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.RuntimeEffects;

public sealed record DorotiShaderCapabilityDiagnostic(
    string Code,
    string Backend,
    string ShaderName,
    string Message);

public static partial class DorotiSkiaRuntimeEffects
{
    public const string ValidationBackend = "runtime-shader-contract";
    public const string MauiGpuBackend = "skiasharp-maui-skglview-gpu";
    public const string WebGpuBackend = "skiasharp-skglview-webgl2-gpu";

    private static readonly IReadOnlySet<string> SupportedBackends =
        new HashSet<string>(StringComparer.Ordinal)
        {
            ValidationBackend,
            MauiGpuBackend,
            WebGpuBackend,
        };
    private static readonly ConcurrentDictionary<RuntimeEffectCacheKey, Lazy<CompiledRuntimeEffect>> EffectCache = [];
    private static long _compiledEffectCount;

    public static event Action<DorotiShaderCapabilityDiagnostic>? CapabilityDiagnostic;

    internal static long CompiledEffectCountForValidation => Interlocked.Read(ref _compiledEffectCount);

    internal static int CompiledCacheEntryCountForValidation => EffectCache.Count;

    internal static SKShader CreateShader(
        FragmentShaderSnapshot snapshot,
        Func<Image, SKShader> imageShaderFactory,
        string backend = ValidationBackend,
        long contextGeneration = 0)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        var state = snapshot.State;
        EnsureBackendSupported(backend, state.DebugName);
        var compiled = GetCompiledEffect(state, backend, contextGeneration);
        var effect = compiled.Effect;
        using var uniforms = new SKRuntimeEffectUniforms(effect);
        BindFloats(compiled, uniforms, state);
        using var children = new SKRuntimeEffectChildren(effect);
        var childShaders = new List<SKShader>();
        try
        {
            for (var index = 0; index < effect.Children.Count; index++)
            {
                if (!state.Samplers.TryGetValue(index, out var image))
                    throw new InvalidDataException(
                        $"Doroti fragment program '{state.DebugName}' requires image sampler {index} ('{effect.Children[index]}').");
                ObjectDisposedException.ThrowIf(image.debugDisposed, image);
                var shader = imageShaderFactory(image);
                childShaders.Add(shader);
                children[effect.Children[index]] = shader;
            }
            if (state.Samplers.Keys.Any(index => index < 0 || index >= effect.Children.Count))
                throw new InvalidDataException(
                    $"Doroti fragment program '{state.DebugName}' received an image sampler outside its declared child range.");
            return effect.ToShader(uniforms, children)
                ?? throw new InvalidOperationException($"Doroti fragment program '{state.DebugName}' did not create a Skia shader.");
        }
        finally
        {
            foreach (var shader in childShaders) shader.Dispose();
        }
    }

    internal static SKShader CreateImageFilterShader(
        FragmentShaderSnapshot snapshot,
        SKImage input,
        SKSamplingOptions inputSampling,
        Func<Image, SKShader> imageShaderFactory,
        string backend = ValidationBackend,
        long contextGeneration = 0)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        var state = snapshot.State;
        EnsureBackendSupported(backend, state.DebugName);
        var compiled = GetCompiledEffect(state, backend, contextGeneration);
        var effect = compiled.Effect;
        if (effect.Children.Count == 0)
            throw new InvalidDataException(
                $"Doroti image-filter program '{state.DebugName}' requires a shader sampler for the filtered child.");

        if (effect.Uniforms.Count == 0 ||
            !compiled.UniformDeclarations.TryGetValue(effect.Uniforms[0], out var firstUniform) ||
            firstUniform.FloatCount != 2 || firstUniform.ArrayLength != 1)
            throw new InvalidDataException(
                $"Doroti image-filter program '{state.DebugName}' requires its first float uniform to be float2.");

        using var uniforms = new SKRuntimeEffectUniforms(effect);
        BindFloats(compiled, uniforms, state, input.Width, input.Height);
        using var children = new SKRuntimeEffectChildren(effect);
        var childShaders = new List<SKShader>();
        try
        {
            var inputShader = input.ToShader(
                SKShaderTileMode.Decal,
                SKShaderTileMode.Decal,
                inputSampling);
            childShaders.Add(inputShader);
            children[effect.Children[0]] = inputShader;

            for (var index = 1; index < effect.Children.Count; index++)
            {
                if (!state.Samplers.TryGetValue(index, out var image))
                    throw new InvalidDataException(
                        $"Doroti image-filter program '{state.DebugName}' requires image sampler {index} ('{effect.Children[index]}').");
                ObjectDisposedException.ThrowIf(image.debugDisposed, image);
                var shader = imageShaderFactory(image);
                childShaders.Add(shader);
                children[effect.Children[index]] = shader;
            }
            if (state.Samplers.Keys.Any(index => index < 0 || index >= effect.Children.Count))
                throw new InvalidDataException(
                    $"Doroti image-filter program '{state.DebugName}' received an image sampler outside its declared child range.");
            return effect.ToShader(uniforms, children)
                ?? throw new InvalidOperationException(
                    $"Doroti image-filter program '{state.DebugName}' did not create a Skia shader.");
        }
        finally
        {
            foreach (var shader in childShaders) shader.Dispose();
        }
    }

    public static void Validate(string source, string debugName = "validation")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        using var effect = SKRuntimeEffect.CreateShader(source, out var errors)
            ?? throw new InvalidDataException($"Doroti fragment program '{debugName}' failed SkSL compilation: {errors}");
    }

    /// <summary>
    /// Removes effects compiled for prior graphics-context generations. Native shaders
    /// created from the old generation must be gone before the host calls this method.
    /// </summary>
    public static void InvalidateContext(string backend, long currentContextGeneration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(backend);
        foreach (var pair in EffectCache.ToArray())
        {
            if (!string.Equals(pair.Key.Backend, backend, StringComparison.Ordinal) ||
                pair.Key.ContextGeneration == currentContextGeneration ||
                !EffectCache.TryRemove(pair.Key, out var removed) ||
                !removed.IsValueCreated)
                continue;
            removed.Value.Dispose();
        }
    }

    private static void BindFloats(
        CompiledRuntimeEffect compiled,
        SKRuntimeEffectUniforms uniforms,
        FragmentShaderState state,
        int? inputWidth = null,
        int? inputHeight = null)
    {
        var effect = compiled.Effect;
        var declarations = compiled.UniformDeclarations;
        var offset = 0;
        foreach (var name in effect.Uniforms)
        {
            if (!declarations.TryGetValue(name, out var declaration))
                throw new InvalidDataException(
                    $"Doroti fragment program '{state.DebugName}' has unsupported uniform declaration '{name}'.");
            var count = checked(declaration.FloatCount * declaration.ArrayLength);
            var values = new float[count];
            for (var index = 0; index < count && offset + index < state.Floats.Count; index++)
                values[index] = checked((float)state.Floats[offset + index]);
            if (offset == 0 && inputWidth.HasValue && inputHeight.HasValue)
            {
                values[0] = inputWidth.Value;
                values[1] = inputHeight.Value;
            }
            if (count == 1 && declaration.ArrayLength == 1)
                uniforms[name] = values[0];
            else
                uniforms[name] = values;
            offset += count;
        }
        if (state.Floats.Count > offset)
            throw new InvalidDataException(
                $"Doroti fragment program '{state.DebugName}' received {state.Floats.Count} floats but declares {offset}.");
    }

    private static IReadOnlyDictionary<string, UniformDeclaration> ReadUniformDeclarations(string source) =>
        UniformDeclarationRegex().Matches(source)
            .Select(match => new UniformDeclaration(
                match.Groups["name"].Value,
                FloatCount(match.Groups["type"].Value),
                match.Groups["array"].Success ? int.Parse(match.Groups["array"].Value) : 1))
            .ToDictionary(item => item.Name, StringComparer.Ordinal);

    private static CompiledRuntimeEffect GetCompiledEffect(
        FragmentShaderState state,
        string backend,
        long contextGeneration)
    {
        var sourceHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(state.Source))).ToLowerInvariant();
        var key = new RuntimeEffectCacheKey(sourceHash, backend, contextGeneration);
        var lazy = EffectCache.GetOrAdd(key, _ =>
            new Lazy<CompiledRuntimeEffect>(
                () => CompileEffect(state.Source, state.DebugName),
                LazyThreadSafetyMode.ExecutionAndPublication));
        try
        {
            return lazy.Value;
        }
        catch
        {
            ((ICollection<KeyValuePair<RuntimeEffectCacheKey, Lazy<CompiledRuntimeEffect>>>)EffectCache)
                .Remove(new(key, lazy));
            throw;
        }
    }

    private static void EnsureBackendSupported(string backend, string shaderName)
    {
        if (SupportedBackends.Contains(backend) ||
            backend.StartsWith(MauiGpuBackend + "/", StringComparison.Ordinal) ||
            backend.StartsWith(WebGpuBackend + "/", StringComparison.Ordinal))
            return;
        var message =
            $"Runtime effect '{shaderName}' is unsupported on backend '{backend}'. " +
            "The shader was not replaced with a transparent or arbitrary fallback.";
        PublishDiagnostic(new DorotiShaderCapabilityDiagnostic(
            "DOROTI_SHADER_BACKEND_UNSUPPORTED", backend, shaderName, message));
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.GraphicsScene,
            null,
            DartUiInvocation.Managed($"runtime-effect:{shaderName}"),
            message,
            backend);
    }

    private static void PublishDiagnostic(DorotiShaderCapabilityDiagnostic diagnostic)
    {
        try
        {
            CapabilityDiagnostic?.Invoke(diagnostic);
        }
        catch
        {
            // A diagnostic observer cannot hide the capability failure itself.
        }
    }

    private static CompiledRuntimeEffect CompileEffect(string source, string debugName)
    {
        var effect = SKRuntimeEffect.CreateShader(source, out var errors)
            ?? throw new InvalidDataException(
                $"Doroti fragment program '{debugName}' failed SkSL compilation: {errors}");
        Interlocked.Increment(ref _compiledEffectCount);
        return new(effect, ReadUniformDeclarations(source));
    }

    private static int FloatCount(string type)
    {
        var normalized = type.Replace("half", "float", StringComparison.Ordinal);
        return normalized switch
        {
            "float" => 1,
            "float2" => 2,
            "float3" => 3,
            "float4" => 4,
            "float2x2" => 4,
            "float3x3" => 9,
            "float4x4" => 16,
            _ => throw new NotSupportedException($"Doroti runtime effects do not support uniform type '{type}'."),
        };
    }

    private sealed record UniformDeclaration(string Name, int FloatCount, int ArrayLength);

    private sealed record CompiledRuntimeEffect(
        SKRuntimeEffect Effect,
        IReadOnlyDictionary<string, UniformDeclaration> UniformDeclarations) : IDisposable
    {
        public void Dispose() => Effect.Dispose();
    }

    private sealed record RuntimeEffectCacheKey(
        string SourceSha256,
        string Backend,
        long ContextGeneration);

    [GeneratedRegex(@"(?m)^\s*(?:layout\s*\([^)]*\)\s*)?uniform\s+(?<type>(?:float|half)(?:[234](?:x[234])?)?)\s+(?<name>[A-Za-z_]\w*)\s*(?:\[\s*(?<array>\d+)\s*\])?\s*;")]
    private static partial Regex UniformDeclarationRegex();
}

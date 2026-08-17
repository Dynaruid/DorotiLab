using System.Text.RegularExpressions;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.RuntimeEffects;

public static partial class DorotiSkiaRuntimeEffects
{
    internal static SKShader CreateShader(
        FragmentShaderSnapshot snapshot,
        Func<Image, SKShader> imageShaderFactory)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        var state = snapshot.State;
        using var effect = SKRuntimeEffect.CreateShader(state.Source, out var errors)
            ?? throw new InvalidDataException(
                $"Doroti fragment program '{state.DebugName}' failed SkSL compilation: {errors}");
        using var uniforms = new SKRuntimeEffectUniforms(effect);
        BindFloats(effect, uniforms, state);
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
        Func<Image, SKShader> imageShaderFactory)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(imageShaderFactory);
        var state = snapshot.State;
        using var effect = SKRuntimeEffect.CreateShader(state.Source, out var errors)
            ?? throw new InvalidDataException(
                $"Doroti fragment program '{state.DebugName}' failed SkSL compilation: {errors}");
        if (effect.Children.Count == 0)
            throw new InvalidDataException(
                $"Doroti image-filter program '{state.DebugName}' requires a shader sampler for the filtered child.");

        var declarations = ReadUniformDeclarations(state.Source);
        if (effect.Uniforms.Count == 0 ||
            !declarations.TryGetValue(effect.Uniforms[0], out var firstUniform) ||
            firstUniform.FloatCount != 2 || firstUniform.ArrayLength != 1)
            throw new InvalidDataException(
                $"Doroti image-filter program '{state.DebugName}' requires its first float uniform to be float2.");

        using var uniforms = new SKRuntimeEffectUniforms(effect);
        BindFloats(effect, uniforms, state, input.Width, input.Height);
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

    private static void BindFloats(
        SKRuntimeEffect effect,
        SKRuntimeEffectUniforms uniforms,
        FragmentShaderState state,
        int? inputWidth = null,
        int? inputHeight = null)
    {
        var declarations = ReadUniformDeclarations(state.Source);
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

    [GeneratedRegex(@"(?m)^\s*(?:layout\s*\([^)]*\)\s*)?uniform\s+(?<type>(?:float|half)(?:[234](?:x[234])?)?)\s+(?<name>[A-Za-z_]\w*)\s*(?:\[\s*(?<array>\d+)\s*\])?\s*;")]
    private static partial Regex UniformDeclarationRegex();
}

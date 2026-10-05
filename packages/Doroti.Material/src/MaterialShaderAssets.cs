using Doroti.Ui;

namespace Doroti.Material;

/// <summary>Explicit resource descriptors owned by this package; no automatic assembly discovery.</summary>
public static class MaterialShaderAssets
{
    public static void Register() => FrameworkShaderManifest.Register(typeof(MaterialShaderAssets).Assembly,
    [
        new FrameworkShaderAsset(
            Id: "material.ink-sparkle",
            FlutterAssetKey: "shaders/ink_sparkle.frag",
            FlutterSourcePath: "packages/flutter/lib/src/material/shaders/ink_sparkle.frag",
            FlutterSourceSha256: "ed126d87b7df031187485bc37345a84aac211ee5efdeeb488ba28f6b0b817592",
            AdaptedSourcePath: "packages/Doroti.Material/assets/shaders/ink_sparkle.sksl",
            AdaptedSourceSha256: "ad41bc223fe55c3d7997a11748e320e891ccbba2395ccdea40d3d09856f96615",
            OwningAssembly: "Doroti.Material",
            EmbeddedResourceName: "Doroti.Material.Shaders.ink_sparkle.sksl",
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
            TargetSupport: ["android", "windows", "maccatalyst", "web"]
        )
    ]);
}

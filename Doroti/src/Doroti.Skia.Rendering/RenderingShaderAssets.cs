using Doroti.Ui;

namespace Doroti.Skia.Rendering;

/// <summary>Explicit resource descriptors owned by this package; no automatic assembly discovery.</summary>
public static class RenderingShaderAssets
{
    public static void Register() => FrameworkShaderManifest.Register(typeof(RenderingShaderAssets).Assembly,
    [
        new FrameworkShaderAsset(
            Id: "rendering.variable-blur",
            FlutterAssetKey: null,
            FlutterSourcePath: null,
            FlutterSourceSha256: null,
            AdaptedSourcePath: "Doroti/src/Doroti.Skia.Rendering/Shaders/variable_blur.sksl",
            AdaptedSourceSha256: "c89e5f6cdf5147011faa05cdb94f9b6eb0dc04748d38c148c2ec5c0fde7fdfeb",
            OwningAssembly: "Doroti.Skia.Rendering",
            EmbeddedResourceName: "Doroti.Skia.Rendering.Shaders.variable_blur.sksl",
            Uniforms:
            [
                new("size", "float2"),
                new("ramp", "float3"),
                new("sigmas", "float2"),
                new("axis", "float2"),
                new("samples", "float"),
                new("fastKernel", "float"),
            ],
            Samplers: [new("inputImage", 0)],
            License: "BSD-3-Clause",
            // Implementation targets, not a claim of device validation.
            TargetSupport: ["android", "windows", "ios", "macos", "maccatalyst", "linux", "web"]
        ),
new FrameworkShaderAsset(
            Id: "rendering.kawase-blur", FlutterAssetKey: null, FlutterSourcePath: null, FlutterSourceSha256: null,
            AdaptedSourcePath: "Doroti/src/Doroti.Skia.Rendering/Shaders/kawase_blur.sksl",
            AdaptedSourceSha256: "8f446677e2283572fefd7f10472fb623877f23b0eaf7386cf6c0ac71dfec58bf",
            OwningAssembly: "Doroti.Skia.Rendering",
            EmbeddedResourceName: "Doroti.Skia.Rendering.Shaders.kawase_blur.sksl",
            Uniforms: [new("size", "float2"), new("ratio", "float2"), new("upsample", "float")],
            Samplers: [new("inputImage", 0)], License: "BSD-3-Clause",
            TargetSupport: ["android", "windows", "ios", "macos", "maccatalyst", "linux", "web"]
        ),
new FrameworkShaderAsset(
            Id: "rendering.variable-blur-stage", FlutterAssetKey: null, FlutterSourcePath: null, FlutterSourceSha256: null,
            AdaptedSourcePath: "Doroti/src/Doroti.Skia.Rendering/Shaders/variable_blur_stage.sksl",
            AdaptedSourceSha256: "fd526bcae79ba633faad919a85a42384b5938c898d379ec980558da2fa2578cd",
            OwningAssembly: "Doroti.Skia.Rendering",
            EmbeddedResourceName: "Doroti.Skia.Rendering.Shaders.variable_blur_stage.sksl",
            Uniforms: [new("size", "float2"), new("ramp", "float3"), new("sigmas", "float2"), new("interval", "float2"), new("lowerRatio", "float2"), new("upperRatio", "float2"), new("lastInterval", "float"), new("firstInterval", "float"), new("lowerOffset", "float2")],
            Samplers: [new("inputImage", 0), new("upperImage", 1)], License: "BSD-3-Clause",
            TargetSupport: ["android", "windows", "ios", "macos", "maccatalyst", "linux", "web"]
        )
    ]);
}

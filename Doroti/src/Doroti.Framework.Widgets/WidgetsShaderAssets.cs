using Doroti.Ui;

namespace Doroti.Framework.Widgets;

/// <summary>Explicit resource descriptors owned by this package; no automatic assembly discovery.</summary>
public static class WidgetsShaderAssets
{
    public static void Register() => FrameworkShaderManifest.Register(typeof(WidgetsShaderAssets).Assembly,
    [
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
            TargetSupport: ["android", "windows", "maccatalyst", "web"]
        )
    ]);
}

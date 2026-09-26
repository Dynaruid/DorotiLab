# Doroti WGSL compiler — initial fragment implementation

Rust 1.95.0, Naga 30.0.1 and Cargo.lock are pinned. This is a build-time tool;
the application does not load Rust or create a second wgpu device.

From the repository root:

```powershell
python Doroti/validation/run-with-timeout.py cargo build --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
python Doroti/validation/run-with-timeout.py cargo test --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
tools/Doroti.Wgsl/target/debug/doroti-wgsl.exe compile DorotiTestbedApp/assets/effects/swap.wgsl DorotiTestbedApp/assets/effects/swap.effect.json Doroti/artifacts/gpu-effects/swap
```

CLI: `validate|reflect|compile|generate source.wgsl definition.effect.json output-directory`.
`validate` checks WGSL, entry point and ABI. The other commands emit reflection
and selected language variants; `compile` and `generate` also write C# serializers.
Errors are returned with a nonzero exit status and `DOROTIWGSL001`; Naga parse and
validation errors include source positions. Definition/ABI errors currently do
not have individual diagnostic codes or source spans.

Definition schema 1 has `assetId`, `entryPoint` and `requiredBackends`. The current
ABI requires a fragment function with one `@location(0) vec2<f32>` UV parameter
and `@location(0) vec4<f32>` output. Engine bindings are sampled texture 0/0,
sampler 0/1 and optional 32-byte frame uniform 0/2. User uniforms use 1/0.
Other bindings and compute stages are rejected explicitly.

Profiles: `vulkan-fragment` (SPIR-V), `webgpu-fragment` (WGSL),
`webgl2-fragment` (GLSL ES 3.00 plus combined-sampler metadata),
`metal-fragment` (MSL with explicit texture/sampler/buffer indices).
Metal output is source, not a metallib. The tool emits engine vertex variants,
translated entry names and WebGL2 block/field mapping. Hosts select the variant
for their existing renderer; translation success is not device execution proof.

Reflection contains source/definition/variant hashes, bindings and scalar field
offsets. Generated `Doroti.Generated.<AssetId>Parameters` properties flatten
vectors/matrices/arrays by index; `Snapshot()` writes each scalar at its Naga
offset with explicit little-endian writes. Padding remains zero. A vec3/mat3x3/
array/i32 golden checks offsets through byte 112 in a 128-byte block.

Generated `Doroti.Generated.Effects.<AssetId>` currently embeds every declared
variant in the application assembly. This is an initial
source-tree integration, **not the final runner-specific resource catalog**.
Compiler RID distribution, NuGet consumption, full graph schema, manifest ABI
validation at load, and atomic output publication remain open in `work.md`.

The SDK uses an existing compiler via `DorotiWgslTool`; its repository default
is this directory's `target/debug/doroti-wgsl[.exe]`. It never invokes Cargo or
downloads compiler dependencies while building an application.

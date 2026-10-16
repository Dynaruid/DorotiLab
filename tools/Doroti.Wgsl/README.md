# Doroti WGSL compiler

Rust 1.95.0, Naga 30.0.1 and Cargo.lock are pinned. This is a build-time tool;
the application does not load Rust or create a second wgpu device.

From the repository root:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 cargo build --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
python Doroti/eng/run-with-timeout.py --timeout 1200 cargo test --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
```

The former `Doroti/validation/gpu-effects` assets are retired. Create a minimal
example under a disposable run directory instead:

```powershell
New-Item -ItemType Directory -Force temp/testing/wgsl-example | Out-Null
'@fragment fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> { return vec4<f32>(uv, 0., 1.); }' | Set-Content -Encoding utf8 temp/testing/wgsl-example/example.wgsl
'{"schema":1,"assetId":"Example","entryPoint":"fs_main","requiredBackends":["vulkan-fragment","webgpu-fragment","webgl2-fragment","metal-fragment"]}' | Set-Content -Encoding utf8 temp/testing/wgsl-example/example.effect.json
python Doroti/eng/run-with-timeout.py --timeout 1200 cargo run --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml -- compile temp/testing/wgsl-example/example.wgsl temp/testing/wgsl-example/example.effect.json temp/testing/wgsl-example/output
```

Inspect the generated manifest, variants and serializers, then remove your run
directory after recording the result. Rust is required for this compiler build;
ordinary apps without `DorotiGpuEffect` inputs do not require it.

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

Generated `Doroti.Generated.Effects.<AssetId>` embeds every declared variant in
the application assembly. Integration uses a source-tree compiler; compiler RID
distribution, NuGet consumption, a full graph schema, manifest ABI validation at
load, and atomic output publication are not available.

The SDK uses an existing compiler via `DorotiWgslTool`; its repository default
is this directory's `target/debug/doroti-wgsl[.exe]`. It never invokes Cargo or
downloads compiler dependencies while building an application.

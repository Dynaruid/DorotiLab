# WGSL GPU effects — 2026-09-26–27

Overall status: **PARTIAL**. WindowsAppSdk and MAUI Windows display a generated
WGSL fragment effect through the real `GpuEffect` widget; WindowsAppSdk also
displays `GpuBackdropEffect`. Metal/WebGPU/WebGL2 fragment host code is included.
This is not completion
of `work.md` G0–G9. Non-Windows execution is `notVerified`; performance is
`notMeasured`. The user's current scope defers non-Windows device/browser runs,
but still requires their implementations.

## Implemented path

`DorotiGpuEffect` item → pinned Naga → generated SPIR-V/C# → `GpuEffect` →
`GpuEffectLayer` → immutable scene payload → host-owned input VkImage → Graphite
segment → native fullscreen fragment pass → wrapped output → Skia composition.
Both Windows hosts keep their existing device, queue and DXGI presentation.
Android/Linux Vulkan owners also register the shared executor; they were not run.

Input allocation uses transformed paint bounds and DPR. Ancestor clips remain
on final composition. Disabled effects use the ordinary child path. Parameters
are immutable copies; the runtime checks their byte count against generated ABI
metadata. User UBOs, frame UBOs, images, descriptors, shader modules, pipelines,
command buffers and snapshots remain owned until the final host fence.
The frame uniform carries capture pixel dimensions, widget logical dimensions,
and the immutable parameter snapshot's time/deltaTime. `GpuEffectController`
accepts explicit updates or an explicitly started ticker; Stop and disposal end
continuous scheduling. Ticker callbacks retain the public Flutter `Duration` contract.

External barriers restore the Skia input layout. Output transitions from color
attachment to shader-read before wrapping. No scheduled state is mislabeled as
GPU completion. `SubmitSegment` keeps one logical frame and one final completion
owner. Cancelling after intermediate submission fences the same queue before
returning resources; timeout retains resources. The effect executors have no
CPU pixel readback/upload round trip or device/queue-idle call (source audit).
Ordinary Skia source-image uploads and uniform writes are separate. Diagnostic
readbacks are separate; no hardware profiler cost claim is made.

## Evidence

| Check | Result |
| --- | --- |
| Rust compiler contracts | 5 PASS: profile output/determinism, vec3/matrix/array layout, binding/entry rejection, compute/profile rejection, frame ABI shape |
| Fixed GLSL native probe | PASS, independent RGB grid versus channel-swap pixels, six logical frames including three cancellations |
| WGSL-generated SPIR-V probe | PASS, eight GPU draws including nested two-pass identity and premultiplied alpha 128/255; exact pixel equality, pipeline creation 1/reuse 7, three cancellations, live bytes return to 0, peak 65,632 bytes |
| User uniform GPU probe | PASS, six draws, red multiplier 0.5, tolerance 1/255; live bytes return to 0, peak 32,816 bytes |
| Engine frame uniform GPU probe | PASS, 32-byte layout, time 0.5/delta 0.25, logical 32×24 versus physical 64×48, six draws, tolerance 1/255 |
| Probe GPU | AMD Radeon 780M Graphics; Vulkan API integer 4206894 |
| WindowsAppSdk Release | Build PASS, zero warnings/errors; displayed RGB→BGR, three synthetic resize cases, toggle off, following text visible |
| MAUI Windows Release | Build PASS, zero warnings/errors; same display/resize/toggle checks; evidence names NVIDIA GeForce RTX 4060 Laptop GPU, Graphite/Vulkan with native composition |
| WindowsAppSdk / MAUI backdrop | Displayed RGB→BGR from preceding layer content, three resize cases per host, toggle off PASS |
| Controller contracts | 8 PASS: copied bytes, idle construction, explicit/idempotent start, time/delta snapshots, stop, manual update, ticker disposal |
| WebGL2 native probe | Windows Edge / ANGLE AMD Radeon 780M D3D11: 3,072 pixels, tolerance 1/255, scalar/vector and complex vec3/matrix/array/i32 UBOs, GL state restore and retirement PASS. This is not a Ganesh product run |
| Web host | C# and TypeScript build PASS; same Worker GL texture table and existing Dawn device/queue used |
| Metal binding compile | AppleGpuEffects compiled against installed iOS 26.5.10315 reference assembly; zero warnings/errors. MSL compilation/device execution notVerified |
| SDK isolated consumer | 8 PASS: clean, no-op (generated mtime unchanged), source edit, rename, removal rejects stale generated API, removal clean build, compiled C# 128-byte serializer golden including padding, empty compiler success rejects stale outputs |
| Existing Graphite backdrop/image filters | 159 independent native pixel comparisons PASS |
| Existing native texture contracts | 25 PASS |
| Physical drag, mixed-monitor DPR, animation cadence, accessibility | notVerified |
| Metal/WebGPU/WebGL2 product execution | Fragment executors and host registrations implemented; full host/device execution notVerified |
| CPU/GPU timing, first-use comparison, SkSL comparison, long-run memory | notMeasured |

Windows checks inspect screen pixels, not just counters. They start their own
process, operate only that window, and close it. Initial validation attempts
found a zero-height sample color box, a stale MAUI binary path, and a startup
capture taken before the first frame. Those fixture issues were corrected.
MAUI input is delivered through synthetic mouse input because posting mouse
messages to the top-level HWND does not target its WinUI input child.

Raw logs/screenshots are disposable under `Doroti/artifacts/gpu-effects/`:
`windows/`, `windows-maui/`, `swap/`, `uniform/`, and `sdk-*/`. In particular,
`enabled-0.png` through `enabled-2.png`, `disabled.png` and `result.json` record
display checks. The tracked summary above survives artifact cleanup.
`windows-backdrop/` records backdrop display and `webgl-*/` records the independent
GL probes. AppleCompile only proves C# binding usage, not native Metal execution.

## Reproduce

All build/test processes use the repository's 1,200-second process-tree deadline.
Run .NET builds serially because project intermediates are shared.

```powershell
python Doroti/validation/run-with-timeout.py cargo build --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
python Doroti/validation/run-with-timeout.py cargo test --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
python Doroti/validation/run-with-timeout.py tools/Doroti.Wgsl/target/debug/doroti-wgsl.exe compile DorotiTestbedApp/assets/effects/swap.wgsl DorotiTestbedApp/assets/effects/swap.effect.json Doroti/artifacts/gpu-effects/swap
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/gpu-effects/GpuEffects.csproj -r win-x64 -- Doroti/artifacts/gpu-effects/swap
python Doroti/validation/run-with-timeout.py tools/Doroti.Wgsl/target/debug/doroti-wgsl.exe compile Doroti/validation/gpu-effects/shaders/uniform.wgsl Doroti/validation/gpu-effects/shaders/uniform.effect.json Doroti/artifacts/gpu-effects/uniform
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/gpu-effects/GpuEffects.csproj -r win-x64 -- Doroti/artifacts/gpu-effects/uniform --uniform
python Doroti/validation/run-with-timeout.py python Doroti/validation/gpu-effects/verify-build.py
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/gpu-effects/Contracts/Contracts.csproj
python Doroti/validation/run-with-timeout.py dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj
python Doroti/validation/run-with-timeout.py python Doroti/validation/gpu-effects/verify-webgl.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/gpu-effects/verify-webgl.py --complex
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/gpu-effects/verify-windows.py
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release
$env:DOROTI_GPU_EFFECT_EXE = (Resolve-Path DorotiTestbedApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe).Path
$env:DOROTI_GPU_EFFECT_OUT = "$PWD/Doroti/artifacts/gpu-effects/windows-maui"
python Doroti/validation/run-with-timeout.py python Doroti/validation/gpu-effects/verify-windows.py
```

Omit the shader directory argument to use the fixed G0 GLSL binaries. Regenerate
those binaries with Vulkan SDK `glslc shaders/fullscreen.vert -o
shaders/fullscreen.vert.spv` and `glslc shaders/swap.frag -o shaders/swap.frag.spv`.
The WGSL product path does not depend on glslc.

## API and remaining requirements

```xml
<DorotiGpuEffect Include="assets/effects/swap.wgsl"
                 Definition="assets/effects/swap.effect.json" />
```

```csharp
new GpuEffect(Doroti.Generated.Effects.SwapChannels, child: panel);
new GpuBackdropEffect(Doroti.Generated.Effects.SwapChannels, child: panel);
```

The sample is selected with `DOROTI_TESTBED_MODE=gpu-effects` or `gpu-backdrop`. Build the Rust
compiler once before building this source-tree testbed. `DorotiWgslTool` can
point to another installed executable. Missing tools fail with DOROTIWGSL100;
no Rust installation/network build is started implicitly.

Still required: backend capabilities and generic texture leases; linear RGB
conversion/HDR/outsets; parameter-only input reuse and texture pooling;
complete compiler/runner/NuGet/publish catalogs; compute/storage/DAG/history;
Metal metallib packaging; advanced WebGPU/WebGL2 graph profiles;
backdrop nesting/native-view capability qualification; async preparation/loader; failure/device-loss qualification and
performance measurements. Current captures retain Skia's existing RGBA8 color
behavior; they do not yet implement the plan's linear-RGB processing contract.
Unsupported profiles fail explicitly and never switch renderers or fall back to CPU/SkSL.

The Metal path currently compiles generated MSL on first use. Vulkan, Metal and
Web pipelines have bounded caches; Vulkan cache eviction retains in-flight
pipeline references until frame completion. WebGL UBO adapters use actual GL
uniform offsets and array/matrix strides instead of assuming WGSL and std140
layouts match. Its program/FBO/texture/VAO/UBO state changes are restored before
Ganesh's cache is invalidated. WebGPU texture/UBO retirement reuses the existing
queue-completion owner; no second device, canvas, or renderer switch is created.

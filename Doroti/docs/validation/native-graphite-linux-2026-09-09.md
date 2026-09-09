# Native Graphite Linux follow-up — 2026-09-09

**Overall migration: PARTIAL.** This checkout is an Ubuntu 26.04 VMware guest, not the Mac in the preceding execution record. The five-OS default transition remains incomplete. The [Windows](native-graphite-2026-09-09.md) and [Apple](native-graphite-apple-2026-09-09.md) reports remain historical evidence, not results rerun here.

## GPU baseline and historical evidence

The user's correction is confirmed by both [ADR-022](../adr/ADR-022-linux-qt-fbo-spike.md), [ADR-023](../adr/ADR-023-linux-qt-qopenglwindow.md), and current execution. `SVGA3D; build: RELEASE; LLVM;` is **not** llvmpipe. The current GL path has `vmwgfx`, DRI3 and `direct rendering: Yes`. The `Accelerated: no` field alone does not invalidate this VMware GPU-backed GL path.

| API | Current device and evidence | Scope |
| --- | --- | --- |
| OpenGL | VMware SVGA II `15ad:0405`, `vmwgfx`, SVGA3D, Mesa 26.0.8, OpenGL 4.3 | Existing Qt Ganesh GPU path is available and executes the Material sample |
| Vulkan | Loader 1.4.341; only llvmpipe, `PhysicalDeviceType.Cpu`, Vulkan 1.4.335 | Explicit software diagnostic execution; not Graphite/Vulkan hardware qualification |

The original Vulkan environment lacked the Khronos layer. Ubuntu `vulkan-validationlayers` 1.4.341.0-1 was subsequently downloaded and extracted under ignored `Doroti/artifacts/native-graphite/local-tools/`, then selected with process-local `VK_ADD_LAYER_PATH` and `LD_LIBRARY_PATH`. Clang 21.1.8 and Ninja 1.13.2 were prepared in the same way. No root installation, NuGet-cache substitution or product native-library replacement was performed. The user-owned .NET SDK received `wasm-tools` for browser validation.

Base commit: `7499cc876eb9db1c074ac0a22313abffa6c555d5`; the initial working tree was clean. SDK 10.0.400, runtime 10.0.11, Qt 6.10.2. SkiaSharp remains `4.154.0-preview.1.26454.9`, managed source `143a933a753dbfeca1909524b2c06c546c5c3e20`, Skia source `cc43af052d3d98e605bee4ddc98671dafded1c57`.

## Implementation

- Extended the existing Vulkan capability project with a Linux Graphite-only target and a bounded runner. Win32/D3D11 modes still require Windows. Reports preserve selected device type, actual loaded native/managed/loader hashes, resolved symlink paths, context/recorder budgets, target manifests, source hashes, GL/Vulkan preflight and the original error. Diagnostic success remains exit 2/PARTIAL and `productQualified=false`.
- Added `build-linux.py` to build the complete pinned Skia library with the bridge included in the same C++ translation unit. The manifest hashes the actual compiled bridge snapshot and captures GN flags, native dependencies, symbol versions and exports. Its host C++ runtime/glibc is a local diagnostic recipe, not a portable RID package.
- Added ABI 2 `doroti_graphite_vk_context_create`. It forwards actual enabled instance/device extension names and either Features or Features2/pNext to both Graphite and its built-in VMA allocator. The extended probe enables supported `robustBufferAccess`, `storageBuffer16BitAccess`, and `VK_KHR_maintenance1` explicitly; it never forwards the complete supported-feature query as enabled features. The original no-feature factory remains the comparison case.
- The diagnostic managed binding owns its dispatch callback until after native context destruction, verifies the pinned x64 init layout, and uses the pinned internal constructor through `UnsafeAccessor`. It does not mutate private handles by reflection or mix separate Skia libraries. Product managed/native packaging, AOT and trimming remain gates.
- Fixed failure-path ownership in the external texture probe: the recording, surface, backend wrapper and recorder now survive until the diagnostic drain. Previously their `using` scopes could release them before the outer `finally` waited. Offscreen callbacks now contain managed exceptions and keep their targets alive through failure drain. Added intentional post-submit failure and simulated pre-submit `VK_ERROR_DEVICE_LOST` cases. Diagnostic device-idle teardown is never a proposed per-frame product solution.
- Added Linux native assets to the runtime-shader and FCR-7 executable validation projects, allowing their normal commands to run without a manual library-path workaround.

## Confirmed regression baseline

All test/build processes use an external 1,200-second timeout. Functional counts below are not performance measurements.

| Check | Result and limitation |
| --- | --- |
| Stock Graphite Linux context/offscreen | PASS on explicitly allowed llvmpipe: three sizes, color/raster upload/offscreen pixel checks and async readback; validation-enabled run has zero warnings/errors |
| Linux ABI 2 build/exports | PASS: complete pinned library, enabled-feature/extension factory plus existing state/semaphore exports; local diagnostic `linux-x64` asset only |
| Enabled features + persistent texture roundtrip | PASS on llvmpipe with synchronization validation: three complete instance/device/context generations, 36 external frames each (108 total), persistent wrappers, external Vulkan copy, queried/updated layouts, binary wait/signal and finite copy fences; zero warnings/errors |
| Stock/rebuilt small-scene pixels | All three async readback hashes match; this is not a complete visual effect corpus or performance comparison |
| Failure after asynchronous submit | Expected raw FAIL/exit 1 with the injected exception; cleanup returned normally with zero validation warnings/errors and no timeout |
| Simulated device loss | PASS: injected queue-submit error becomes failed Submit plus `IsDeviceLost`, and teardown returns with zero validation messages; physical device reset/recovery remains notVerified |
| Software-device refusal | Expected FAIL without `--allow-software`; device identity is saved before rejection |
| Qt Wayland Material | PASS: 20 requested resize cycles, 37 rasterized frames, 34 presented + 3 replayed, zero failed frames, normal exit 0 |
| Qt XWayland/xcb Material | PASS: 20 requested resize cycles, 34 rasterized/presented frames, zero failed frames, normal exit 0 |
| Qt GPU ownership | Both current live runs identify `skiasharp-qt-opengl-gpu` / SVGA3D, `softwareFallback=false`, zero full-frame CPU copies and synchronous GUI waits |
| Qt ABI/keyboard/clipboard | PASS |
| Runtime shader | PASS after fixing the Linux native package reference; the initial missing-library exception is retained |
| Full FCR-7 Material/widget | PASS, including both theme palettes and Material sample contracts; this is not native visual or physical accessibility acceptance |
| Canonical Linux build/run/publish | Final serialized commands exit 0, record the successful artifact and run the SVGA3D path; canonical Wayland run rasterizes 58 frames (41 presented + 17 replayed), zero failures |
| Relocated Linux publish | PASS: copied the 31 MiB framework-dependent publish tree to a separate artifact directory; XWayland run rasterizes 39 frames (27 presented + 12 replayed), zero failures, normal exit 0. Not a clean OS installation or self-contained package |
| Windows managed probe cross-build | PASS with zero warnings/errors. The ABI 2 Windows native rebuild and execution were not performed here |
| Canonical Web build | PASS with zero warnings/errors and a recorded successful artifact; wasm-tools installed under the user-owned SDK |
| Chrome WebGL browser regression | PASS, 6 tests: real managed Skia GPU ownership with no bitmap transfers, Material destinations/theme/responsive navigation, pointer controls/dialogs/search/image scrolling, lazy components/theme images, visited-section state and latest theme choice |
| Chrome WebGPU unavailable handling | PASS, 1 test: explicit unsupported-adapter error, no host admission, main runtime alive, no automatic WebGL fallback. Current Chrome requestAdapter returns null; actual WebGPU rendering remains notVerified |

The first Linux compilation passed, but the canonical command refused to record a reusable artifact because the workload installation changed the toolchain during that build. The ensuing `-NoBuild` run correctly refused the absent success record. Direct launches of the built app supplied the live GL evidence above. An initial concurrent Web rebuild also collided with a Linux rebuild in shared output directories; its missing-DLL errors are orchestration failures and are retained. Subsequent canonical operations are serialized.

The first device-loss injection was armed too early and correctly prevented context creation: the pinned context factory itself submits initialization work. The corrected negative probe forwards those initial submissions, drains them, then arms the simulated error before drawing. Both reports are retained. This explicit setup drain is a diagnostic step, never a per-frame product behavior.

Chrome 153.0.8010.36 identifies ANGLE/OpenGL ES over VMware SVGA3D and reports `hardwareSupportsVulkan=false`; a separate secure-origin WebGPU adapter request returns null. The first browser attempts failed before application execution because Playwright's FFmpeg executable was absent. After installing only that user-local tool, all seven selected tests passed. Browser artifacts use separate labels for the failed attempt and successful WebGL/WebGPU-unavailable runs; no failed report was overwritten.

The first native bootstrap compiled the prior ABI 1 snapshot while the working source was being extended. Its old builder hashed the mutable source path at completion, so that bootstrap's bridge-source hash is not used as provenance. The final ABI 2 build uses the corrected builder and hashes the actual copied translation-unit include; its manifest is the selected build evidence.

## Reproduction

See the [native bridge README](../../native/graphite/README.md) for build/probe commands, explicit software selection, validation-layer setup, negative cases and managed Windows cross-compilation. Restore the existing product path simply by using the unchanged canonical app runner; the probe never installs its native library into product output or NuGet.

Versioned reports, commands, hashes, selected raw logs, initial failures and target manifests are in [native-graphite-linux-execution.json](../../../history/26-09-09/native-graphite-linux-execution.json). The selected ABI 2 build manifest is `Doroti/artifacts/native-graphite/build-runs/20260909T111118136327Z-linux/manifest.json`; the three-generation roundtrip is `Doroti/artifacts/native-graphite/linux-runs/20260909T111417856271Z/probe.json`. Browser artifacts are under `Doroti/validation/web-playwright/artifacts/native-graphite-linux-webgl-final/` and `native-graphite-linux-webgpu-unavailable/`.

After canonical Web build, serve with `dotnet run --project DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --no-build --no-restore` (the existing development profile provides the isolation headers on port 5088). From `Doroti/validation/web-playwright`:

```sh
DOROTI_BROWSER_CHANNEL=chrome DOROTI_WEB_RENDERER_MODE=worker-direct-webgl \
  DOROTI_WEB_ARTIFACT_LABEL=native-graphite-linux-webgl-final \
  timeout 1200 npx playwright test tests/direct-capability.spec.ts tests/material-sample.spec.ts --project=chromium-hardware
DOROTI_BROWSER_CHANNEL=chrome DOROTI_WEB_ARTIFACT_LABEL=native-graphite-linux-webgpu-unavailable \
  timeout 1200 npx playwright test tests/webgpu-unavailable.spec.ts --project=chromium-hardware
```

Current Qt baseline commands use `DOROTI_TESTBED_MODE=sample`, `DOROTI_RESIZE_FIXTURE=none`, `DOROTI_QT_DIAGNOSTICS=1`, `DOROTI_QT_VALIDATION_RESIZE_CYCLES=20`, and separately `QT_QPA_PLATFORM=wayland` / `xcb`. Xcb in this Wayland desktop is XWayland, not a physical native X11 session. No keyboard/IME actions were injected by these resize runs (`inputCount=0`).

## Remaining work and promotion gates

| Stage | Remaining boundary |
| --- | --- |
| NG0 | Complete target/device support policy, identical workload/performance/memory and physical evidence matrix |
| NG1 | Product Windows external-memory/presentation round trip, Linux/Android WSI acquire/present ownership, real device-loss recovery, required RID packages and ABI/dependency/license qualification |
| NG2 | Product Vulkan session, complete image export/cache/effect qualification and platform-terminal integration; the existing shared Metal session remains a candidate |
| NG3 | Windows App SDK and independent MAUI Graphite/Vulkan adapters and their native/device/input/resize checks |
| NG4 | iOS/Catalyst owned Metal handlers and device lifecycle/input/accessibility; remaining macOS product gates |
| NG5 | Qt QWindow Vulkan presenter, new native ABI, swapchain lifecycle, Wayland/native X11 and physical Vulkan device validation |
| NG6 | Android Surface/ANativeWindow bridge, MAUI handler, capability/minimum SDK/packaging and physical device lifecycle/input tests |
| NG7 | Platform-by-platform promotion, previous native-path removal, clean installed packages and full README/manifest/template synchronization |

There is no connected Windows/Apple/Android execution environment in this session. Existing VMware OpenGL GPU acceleration does not provide a hardware Vulkan device. Simulated errors, CPU Vulkan pixels, API-level frame terminals and past hardware results cannot fill the current physical/device/performance gates. All five-OS completion checkboxes remain subject to their full criteria; no default has been promoted.

# Native Graphite Windows and Android follow-up

Status: **PARTIAL, no native default promotion**. This follow-up adds the shared Vulkan session, native ABI 3, an explicit Windows App SDK Graphite candidate, actual Windows Composition output qualification and an Android arm64 native probe on a connected Galaxy S25. Linux and Apple **product validation was skipped at the user's request**. That does not qualify those platforms or remove their remaining implementation work.

The baseline commit was `f259644d42ac6e7a8e54e41356f7e76e4bb6f38f`, initially with a clean worktree. Raw commands, outcomes, selected logs, native module identities and original failures are recorded in [versioned execution evidence](../../../history/26-09-09/native-graphite-windows-android-execution.json). Each test/build child has an external 1,200-second timeout. Android additionally runs the native executable under an on-device timeout.

## Implementation

- `SkiaGraphiteSession` now supports Vulkan with the exact enabled features/extensions and a rooted dispatch callback. It retains one backend wrapper per target, bounds outstanding frames, rejects concurrent/foreign ownership, supports cancellation and recorder-based raster upload/offscreen/runtime shader/capture, and returns resources after GPU completion. Layout and queue ownership come from the native texture state; a completed external transition is explicitly published back to Graphite.
- Native ABI 3 adds externally observed device-loss notification to the pinned `VulkanSharedContext`. An unknown GPU timeout is not treated as device loss. Version 1/2 signatures are retained. The Windows and Android builds use the same pinned Skia revision `cc43af052d3d98e605bee4ddc98671dafded1c57`; ABI 3 Linux rebuild/execution is not claimed here.
- The shared binding uses host-resolved native function pointers from the same module as SkiaSharp. Static imports of the custom native exports initially broke WebAssembly linking; they were removed from the shared assembly, without allowing undefined WASM symbols or adding fake bridge exports.
- `WindowsManagedVulkanPresenter` selects Graphite only with `DOROTI_WINDOWS_GRAPHITE=1` and an absolute `DOROTI_WINDOWS_GRAPHITE_NATIVE` path. A loaded different Skia module or wrong bridge ABI fails explicitly. The candidate preserves retained backing, exact/latest viewport admission, prepared-frame keys, dedicated D3D11 import, synchronous copy fence, Composition receipts/slot availability and Acrylic. It does not present CPU readback pixels. Picture raster caching remains disabled for this candidate pending its separate qualification.
- Native Windows `Run` joins its raster worker before returning. After that join and GPU drain, a terminal-only ownership handoff allows the closing thread to release renderer/session resources. Ordinary frame access still requires its render owner. If GPU ownership cannot be established, the existing whole-state quarantine retains the session and renderer instead of freeing live work.
- `build-android.py` cross-builds the diagnostic asset using NDK 30, API 24 and 16 KiB ELF alignment. `graphite-android.cpp` and its runner use a unique device temporary directory, verify the actually loaded module path and device-side hashes, and leave installed apps unchanged.

## Verified boundaries

| Contract | Evidence |
| --- | --- |
| Windows ABI 2 baseline | RTX 4060 Laptop and Radeon 780M each passed 3 contexts/108 external texture frames with validation warnings/errors 0. The initial Skia driver-properties warning was addressed by enabling and forwarding the extension. |
| Windows ABI 3 / shared session | Persistent targets, cancellation, frame limit, wrong-thread rejection, asynchronous readback, raster upload, recorder offscreen/runtime shader and simulated submission loss pass on both GPUs. |
| Real Windows output | Graphite → Vulkan copy → dedicated D3D11 imported texture → DwmFlush boundary completion → three buffer-availability returns. A three-context run produced 108 boundary completions per GPU, validation warnings/errors 0. Native mode 1 does not query per-present display statistics; the earlier receipt attribution was incorrect. This is not physical scan-out. |
| Failure cleanup | Exceptions immediately after asynchronous submission and after Composition present preserve the original exit 1. GPU drain/platform retirement finish without validation errors. |
| Product Windows host | Both GPUs × existing/Graphite × opaque/Acrylic/simulated loss: 12 runs pass. The fixture checks actual Framework scene output, platform/raster/input thread separation, reset, resize terminal coverage and normal close. |
| Strict prepared frame | Radeon baseline/candidate pass. NVIDIA baseline/candidate both fail the existing 50 ms CompositionFrame receipt after prepared commit. These are historical strict failures. A 1,000 ms rerun confirmed matching receipts at up to 249.355 ms (Ganesh) / 252.540 ms (Graphite), with reuse, duplicate rejection and teardown passing. The user accepted this latency; NVIDIA now has a 1,000 ms product budget and other vendors keep 50 ms. |
| Galaxy S25 native ABI 3 | SM-S931N, API 36, Adreno 830, Vulkan API value 4206876. Actual module path and hashes match. Three contexts, 36 persistent-target frames, all RGBA pixels, async GPU completion and simulated external-loss notification pass, followed by teardown. |
| Android exclusions | No MAUI/managed session, Activity/SurfaceView/ANativeWindow/WSI output, synchronization validation layer, touch/IME/TalkBack, device rotation/surface recreation or physical display qualification. This is a C ABI GPU probe. |
| Shared contracts | Runtime shader and complete FCR-7 Material/widget contracts pass. |
| Canonical Windows/Web build | Both pass after removing the static custom Vulkan imports from the shared assembly; zero build warnings/errors. The original Web linker failure remains recorded. |
| Actual Material app | Canonical `run -NoBuild`, both GPUs × existing/Graphite: four executions exit 0 with actual first-content/present and clean close. This is initial output, not full native interaction/visual/performance acceptance. |
| Web browser | Hardware direct presenter and Material navigation/theme/pointer/dialog/search/image/scroll contracts: six WebGPU + six explicit WebGL tests pass, zero skipped/unexpected tests. The isolated port 5199 server is stopped and its listener released. |
| Linux/Apple product execution | `skippedByUser` for this follow-up; earlier separate-platform evidence remains historical. |

The native library minimum API 24 is a diagnostic build choice. The shipping Android target still declares API 21 and uses its existing GL host. A Graphite product additionally needs supported Vulkan capabilities, app-level deployment settings and the MAUI surface adapter; the probe does not change that policy.

## Failures preserved

1. Initial shared-frame compilation failed on a private nested field; the frame constructor now accepts its persistent target explicitly.
2. The first Android archive step invoked the Windows Store `python3` alias. The builder now provides a process-local shim to the selected Python interpreter, leaving OS aliases unchanged.
3. The first ABI 3 Windows build exposed Windows' `interface` macro in a Skia Vulkan header. A scoped push/undef/pop fixes the translation-unit include.
4. Product Graphite shutdown originally crossed the session owner thread. The joined-worker/GPU-drain terminal handoff fixes it and subsequent product host execution closes normally.
5. The product validator assumed every Vulkan backend string was Ganesh. Its Vulkan contract now follows the requested Vulkan path while still checking the exact expected Graphite/Ganesh identity and all prior hardware/topology assertions.
6. The NVIDIA prepared-frame receipt times out in both baseline and candidate. Its original snapshots remain failures under the old 50 ms criterion. The subsequent measured latency was accepted by the user; see the separate latency acceptance evidence.
7. The first canonical Web build failed on six undefined custom bridge symbols. The shared function-pointer boundary replaces those static imports; the failure remains in the evidence.

The final canonical/product/Material batch is `Doroti/artifacts/native-graphite/runs/20260909T122221564Z/`; browser reports are under `Doroti/artifacts/native-graphite/web-runs/20260909T123450Z/`. Galaxy ABI 3's loaded-path/hash receipt is `Doroti/artifacts/native-graphite/android-runs/20260909T121155121980Z/report.json`. Those diagnostics do not qualify portable packaging or physical display/input.

The final shared binding's three-generation Composition/strict-prepared rerun is `runs/20260909T124247523Z`: both GPU interop probes pass, both NVIDIA prepared fixtures fail the 50 ms receipt, and both AMD fixtures pass. Final injected-failure cleanup runs are `runs/20260909T124450426Z` (after submit) and `runs/20260909T124627154Z` (after present); each preserves the expected raw failure with validation warnings/errors 0. These paths are relative to `Doroti/artifacts/native-graphite/`.

## Remaining work and recovery

Windows MAUI, Linux Vulkan QWindow/ABI, Android MAUI Surface/WSI, and iOS/Catalyst Graphite adapters remain incomplete. Product RID packaging, clean publish/install, AOT/trim, full image export/cache policy, physical device loss, performance/memory comparison and platform input/accessibility acceptance remain open. **The five-OS migration is not complete.** The accepted NVIDIA receipt latency is no longer a promotion blocker; Linux/Apple execution was explicitly waived, not passed. Existing native product paths and historical evidence are retained.

Reproduction commands are in the [native bridge README](../../native/graphite/README.md). For the explicit Windows candidate:

```powershell
$env:DOROTI_WINDOWS_GRAPHITE = '1'
$env:DOROTI_WINDOWS_GRAPHITE_NATIVE = (Resolve-Path Doroti/artifacts/native-graphite/skia-build/out/doroti-win-x64/libSkiaSharp.dll).Path
pwsh -File Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows
```

Remove both variables and start a fresh process to restore the previous renderer. No NuGet or installed application is replaced. Android diagnostics retain their uniquely named `/data/local/tmp/doroti-graphite-*` directories; the exact paths and hashes are in the execution JSON. Normal app recovery uses the existing canonical run command because the probe does not install an APK.

## NVIDIA latency acceptance (user decision)

The user authorized accepting NVIDIA if the failure was only latency. Diagnostic run `runs/20260909T125347518Z` extended the same strict receipt wait to 1,000 ms; both GPUs and both renderers passed. NVIDIA matching present-id/tag/display-instance receipts arrived around 249/253 ms. Product-policy run `runs/20260909T125621676Z` then passed all four prepared fixtures and four product lifecycle runs: NVIDIA used 1,000 ms (about 241/238 ms observed), AMD retained 50 ms (about 8 ms). Receipt timeouts and outstanding reservations were zero. Over-50 ms observations are counted separately. This accepts the measured response latency, not GPU errors, missing receipts, physical scan-out or unrelated performance regressions. Historical raw reports are unchanged. [Decision, commands and measurements](../../../history/26-09-09/native-graphite-nvidia-latency-acceptance.json).

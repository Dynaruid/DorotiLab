# Native Graphite interop qualification

Status: **ABI 3 native build and default-host distribution configuration**. Graphite is the user-selected default for the native hosts. Windows/Android execution evidence and the Linux/Apple user-requested validation skip are recorded in [default adoption](../../docs/validation/native-graphite-defaults-2026-09-09.md). The qualification probes below retain their historical scope and do not establish all product/performance gates.

The central SkiaSharp package is `4.154.0-preview.1.26454.9`, repository revision `143a933a753dbfeca1909524b2c06c546c5c3e20`. Its Skia submodule is `cc43af052d3d98e605bee4ddc98671dafded1c57`. `build-windows.py` checks out that exact Skia revision, syncs its pinned DEPS, includes `doroti_graphite_interop.inc` inside `src/c/sk_graphite_vulkan.cpp`, and builds the complete `libSkiaSharp.dll`. The bridge never links a second Skia instance. Both managed APIs resolve to the selected DLL in an isolated probe process.

From the repository root, with VS C++ x64 tools/Windows SDK, LLVM, Ninja, Git and Python installed:

```powershell
python Doroti/native/graphite/build-windows.py
& Doroti/validation/windows-vulkan-capability/run-graphite.ps1 `
  -Device @('NVIDIA GeForce RTX 4060 Laptop GPU', 'AMD Radeon 780M Graphics') `
  -NativeLibrary (Resolve-Path Doroti/artifacts/native-graphite/skia-build/out/doroti-win-x64/libSkiaSharp.dll).Path
```

Substitute the exact device names on the machine. The build accepts `--vs`, `--llvm`, and `--jobs`; it refuses a different source revision or unrelated edits to the upstream shim. Each child build/test has a 1,200-second timeout. Test timeout kills the probe process tree. Every test run creates a separate timestamped evidence directory, retaining original failures. Native build logs, GN flags, source/bridge/asset hashes and command exit codes are under `Doroti/artifacts/native-graphite/build-runs/`.

Omit `-NativeLibrary` to inspect the original NuGet asset. The runner records a Vulkan/D3D11 capability baseline separately from Graphite. Probe exit code **2 means PARTIAL**, 1 means failure; it never reports product qualification as exit 0. The old capability mode retains its original 0/1 behavior. No NuGet cache, product output, package version, renderer default, or OS installation is replaced by these commands. To resume the original application, use its existing `Doroti/eng/doroti.ps1` command; no rollback installation is needed.

## Linux reproduction

With clang/clang++, Ninja, Git, Python, fontconfig development files and the Vulkan loader available:

```sh
python3 Doroti/native/graphite/build-linux.py --jobs 4
python3 Doroti/validation/windows-vulkan-capability/run-graphite-linux.py \
  --native-library Doroti/artifacts/native-graphite/skia-linux-build/out/doroti-linux-x64/libSkiaSharp.so \
  --extended-context --device 'exact Vulkan device name'
```

The existing Windows validation project also builds its Graphite-only mode for `net10.0/linux-x64`; Win32/D3D11 modes reject Linux explicitly. Omit `--native-library` and `--extended-context` for the stock no-feature baseline. `--allow-software` explicitly permits a CPU Vulkan ICD for diagnostics only. The report still names its device type, has `productQualified=false`, and exits **2/PARTIAL**, even with successful pixel/synchronization checks. GL and Vulkan preflight logs are separate: VMware `SVGA3D; ... LLVM` plus `vmwgfx`/DRI3/direct rendering remains valid GPU-backed OpenGL evidence even when Vulkan exposes llvmpipe only.

`--context-cycles 3` destroys and recreates the instance, device and Graphite context three times in the same process, retaining each generation's completed external-texture results. The Windows equivalent is `-ContextCycles 3`. Each generation renders 36 external-texture frames. This is normal recreation, not real device-loss recovery.

Use the Khronos validation layer, with `VK_ADD_LAYER_PATH` and `LD_LIBRARY_PATH` if its files are extracted into a local directory. A run without validation never marks external synchronization PASS. Every child command has an external 1,200-second process-group timeout. Build manifests retain actual compiled bridge/DEPS/flags/library hashes, exported symbols, ELF dependencies and symbol-version requirements. This host-toolchain build is not a portable RID package: the supported glibc/sysroot, notices, clean installation and other RID gates remain open. `--cc` and `--cxx` select explicit local compiler executables; `ninja` is resolved from PATH.

`--self-test graphite-after-submit` intentionally exits **1/FAIL** immediately after asynchronous submission to exercise the diagnostic failure drain. The original error and validation output remain failures in the raw report. `--extended-context --self-test graphite-device-lost` injects `VK_ERROR_DEVICE_LOST` before queue submission; it exercises error propagation and teardown without resetting hardware. Its result cannot qualify physical device-loss recovery. The Windows runner accepts `-ExtendedContext` for ABI 2; Windows execution still requires Windows hardware.

Compile the Windows managed probe from Linux without treating compilation as execution:

```sh
timeout 1200 dotnet build Doroti/validation/windows-vulkan-capability/Doroti.Validation.WindowsVulkanCapability.csproj \
  -p:DorotiVulkanProbePlatform=Windows -p:EnableWindowsTargeting=true \
  --artifacts-path Doroti/artifacts/native-graphite/windows-cross-build
```

## ABI ownership (version 3, additive to versions 1 and 2)

| Export | Contract |
| --- | --- |
| `doroti_graphite_interop_version` | Returns 3. Prior exports retain their signatures. The shared product session requires ABI 3. |
| `doroti_graphite_vk_context_report_device_lost` | ABI 3: forwards externally observed `VK_ERROR_DEVICE_LOST` to the same Skia shared context. Not an abandon/drain substitute for an unknown error or timeout. |
| `doroti_graphite_vk_context_create` | Takes the pinned init/options, either enabled Features or Features2 (including pNext), and exact enabled instance/device extension names. Features/strings are borrowed during creation; dispatch callback/user data live through destruction. Uses the pinned VMA allocator; custom allocators and protected contexts are unsupported. |
| `doroti_graphite_vk_texture_get_state` | Reads the persistent Graphite backend texture's tracked image layout and queue family. This is scheduled state, not a GPU completion signal. |
| `doroti_graphite_vk_texture_set_state` | Updates tracking after the caller queues/completes the actual barrier. Performs no Vulkan barrier itself. Shares state with the wrapped Graphite surface. |
| `doroti_graphite_vk_insert_recording` | Adds borrowed binary wait/signal semaphores to the recording. Native wrapper arrays survive insertion; the caller retains actual Vulkan semaphore handles until consumption. |
| `doroti_graphite_has_unfinished_gpu_work` | Read after `CheckAsyncWorkCompletion`. Does not acknowledge platform presentation or physical scan-out. |

The probe uses one graphics queue, one owner thread, and at most one outstanding copy. Each size owns one image, allocation, backend wrapper, surface and recorder through twelve frames. It uses the real queried layout for the Graphite → transfer barrier, copies into a mapped verification buffer, transitions back to `General`, waits for the copy fence, and updates Graphite state. The wrapper is reused. This CPU readback is a verification sink, **not a proposed product output route**. The final device-idle is diagnostic teardown only; frames use `Submit(Sync=false)` and finite copy-fence waits.

The pinned `VulkanCaps` requires `INPUT_ATTACHMENT` together with `COLOR_ATTACHMENT` for renderable external textures. The original attempt without it returned a null surface; the failure remains recorded. Usage now also includes sampled, transfer-source and transfer-destination bits on the actual image and its Graphite descriptor.

## Remaining bridge/package gates

- The original context C ABI cannot pass enabled feature/extension chains. Its baseline still enables **no optional device features or extensions**. ABI 2 adds a separate creation export; the explicit extended probe passes the same enabled `robustBufferAccess`, `VkPhysicalDevice16BitStorageFeatures` chain and `VK_KHR_maintenance1` name used at device creation. Supported bits are queried separately and only selected enabled bits are forwarded. The diagnostic managed binding uses `UnsafeAccessor` for the exact pinned internal context constructor, roots its dispatch delegate through native destruction, and resolves both binding assemblies to one native module. A product managed/native package and its AOT/trim/ABI gates remain required.
- No final-target-state API, native completion callback binding, device-loss recovery, outstanding-work shutdown qualification, or presentation-terminal binding is claimed by ABI 1. The polling path qualifies only the tested same-queue copy.
- Windows D3D11 dedicated-image copy, DwmFlush boundary completion and three-slot availability retirement pass in the ABI 3 probe. Native mode 1 does not query per-present display statistics; the earlier receipt attribution was incorrect. Opaque/Acrylic and simulated loss also run in the explicit product host candidate. The strict prepared-frame receipt arrives around 250 ms on NVIDIA. The user accepted this latency; its product budget is now 1,000 ms, other vendors remain at 50 ms, and missing receipts still fail. Other promotion gates remain open. Linux/Android WSI, split queues, acquire/present semaphore consumption and swapchain destruction remain open.
- The Apple follow-up qualifies the tested M1 Metal drawable ordering and completion release with the stock asset. Apple device loss/iOS/Catalyst qualification, Android lifecycle and other Vulkan RID rebuilds remain open. Do not promote this DLL into the product feed or share its C++ pointers with another Skia DLL.
- The build manifest is provenance for a local diagnostic build, not a reproducible RID package or clean-publish PASS. Native dependencies, licenses and managed/native ABI must be included in the eventual package workflow.

Skia source licensing is the pinned checkout's `LICENSE`; upstream native-package notices are in `SkiaSharp.NativeAssets.Win32/4.154.0-preview.1.26454.9/LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`. Preserve them with any future redistributed asset. The local build does not redistribute or relicense upstream source.

## ABI 3 Windows candidate and Android reproduction

```powershell
python Doroti/native/graphite/build-windows.py
& Doroti/validation/windows-vulkan-capability/run-graphite.ps1 `
  -Device @('NVIDIA GeForce RTX 4060 Laptop GPU', 'AMD Radeon 780M Graphics') `
  -NativeLibrary (Resolve-Path Doroti/artifacts/native-graphite/skia-build/out/doroti-win-x64/libSkiaSharp.dll).Path `
  -ExtendedContext -SharedSession -Composition -ContextCycles 3 `
  -ProductHost -HostScenario @('lifecycle', 'acrylic', 'device-lost') -Regressions
```

`-ProductPresenter` runs the prepared fixture using the product hardware policy. `-PreparedReceiptTimeoutMs 50` reproduces the historical strict gate; an explicit value up to 5,000 ms is a diagnostic override only. `-SelfTest graphite-after-submit` and `graphite-after-present` retain raw exit 1 and require the exact injected error with zero validation messages. `-CanonicalBuilds -MaterialSample` builds the Windows/Web targets and runs the actual Material sample through the canonical runner. No asynchronous process is counted as passing before exit and report validation.

For a fresh Windows sample process, set `DOROTI_WINDOWS_GRAPHITE=1` and an absolute `DOROTI_WINDOWS_GRAPHITE_NATIVE` path to that ABI 3 DLL, then use the normal `Doroti/eng/doroti.ps1 run` command. The candidate rejects a different already-loaded Skia module, wrong bridge ABI and missing native asset. It installs neither files nor a fallback. To return to the prior renderer, remove both variables and start a new process. This explicit diagnostic module selection is not RID packaging or clean-install qualification.

With the pinned checkout prepared above and `ANDROID_NDK_HOME` set:

```powershell
python Doroti/native/graphite/build-android.py --jobs 8
python Doroti/validation/windows-vulkan-capability/run-graphite-android.py --serial R3CY30KZA4B
```

Select the actual device serial. The Android recipe uses API 24, 16 KiB ELF alignment and the same pinned bridge; it does not change the MAUI app's minimum API 21. The arm64 probe runs in a unique `/data/local/tmp/doroti-graphite-*` directory and checks the actually loaded module path and on-device hashes. It returns 2/PARTIAL after three contexts, 36 pixel-checked persistent-texture frames and simulated external loss notification. There is no Activity/Surface/WSI presentation, synchronization validation layer, managed Android binding, or product-package approval in that result. On-device files and host evidence are retained, and no application installation is changed.

## Default-host assets

Source builds stage and hash assets before launch-dependency collection and compilation:

```powershell
python Doroti/native/graphite/build-windows.py --ensure-distribution
python Doroti/native/graphite/stage-android.py --cpu arm64
# Android x64 is built separately when that RID is selected.
python Doroti/native/graphite/stage-android.py --cpu x64
```

On Linux use `python3 Doroti/native/graphite/stage-linux.py`. It builds against the local distribution; other glibc/libstdc++ baselines remain unqualified. The scripts validate the pinned source/bridge and staged SHA-256, and retain timestamped build evidence. The Android source preparation uses the existing Windows/Linux bootstrap, with an installed NDK selected by `ANDROID_NDK_HOME` or the local Android SDK.

Windows App SDK ships `graphite/libSkiaSharp.dll`; Windows MAUI ships the same qualified binary as `graphite/libDorotiGraphite.dll`. Linux ships `graphite/libDorotiGraphite.so`. Desktop distributions include `build-provenance.json`, `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`; the runtime validates ABI/RID/hash. Android embeds its single qualified `libSkiaSharp.so` in the host AAR and excludes stock native runtime assets at the runner dependency root because AOT imports may bypass a managed resolver. No file in the user's NuGet cache is modified.

AppKit/iOS/Catalyst retain the pinned package's Graphite/Metal asset and use the public Metal binding; Vulkan ABI 3 imports are not used on those paths. Their product validation in this Windows session is skipped by user request.

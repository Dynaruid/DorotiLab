# Native Graphite interop qualification

Status: **NG1 PARTIAL; bridge builds are diagnostic assets**. Product defaults still use their existing native renderers. The stock macOS asset has a validated Metal probe and an explicit AppKit Graphite candidate (`DOROTI_MACOS_GRAPHITE=1`); it does not use this Vulkan bridge. See [Windows execution evidence](../../docs/validation/native-graphite-2026-09-09.md), [Apple follow-up](../../docs/validation/native-graphite-apple-2026-09-09.md), and [Linux follow-up](../../docs/validation/native-graphite-linux-2026-09-09.md). Historical Windows ABI 1 results do not qualify the newer ABI 2 on Windows.

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

## ABI ownership (version 2, additive to version 1)

| Export | Contract |
| --- | --- |
| `doroti_graphite_interop_version` | Returns 2. Old state/semaphore exports retain their ABI 1 signatures. Check before calling the bridge. |
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
- Windows D3D11 dedicated-image copy and Windows Presentation slot return must be joined to Graphite and validated separately. Linux/Android WSI, split queues, acquire/present semaphore consumption and swapchain destruction remain open.
- The Apple follow-up qualifies the tested M1 Metal drawable ordering and completion release with the stock asset. Apple device loss/iOS/Catalyst qualification, Android lifecycle and other Vulkan RID rebuilds remain open. Do not promote this DLL into the product feed or share its C++ pointers with another Skia DLL.
- The build manifest is provenance for a local diagnostic build, not a reproducible RID package or clean-publish PASS. Native dependencies, licenses and managed/native ABI must be included in the eventual package workflow.

Skia source licensing is the pinned checkout's `LICENSE`; upstream native-package notices are in `SkiaSharp.NativeAssets.Win32/4.154.0-preview.1.26454.9/LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`. Preserve them with any future redistributed asset. The local build does not redistribute or relicense upstream source.

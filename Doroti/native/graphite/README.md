# Native Graphite interop qualification

Status: **NG1 PARTIAL; rebuilt bridge asset remains diagnostic win-x64 only**. Product defaults still use their existing native renderers. The stock macOS asset now has a validated Metal probe and an explicit AppKit Graphite candidate (`DOROTI_MACOS_GRAPHITE=1`); it does not use this Vulkan bridge. See [Windows execution evidence](../../docs/validation/native-graphite-2026-09-09.md) and [Apple follow-up](../../docs/validation/native-graphite-apple-2026-09-09.md).

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

## ABI 1 ownership

| Export | Contract |
| --- | --- |
| `doroti_graphite_interop_version` | Returns 1. Check before calling the bridge. |
| `doroti_graphite_vk_texture_get_state` | Reads the persistent Graphite backend texture's tracked image layout and queue family. This is scheduled state, not a GPU completion signal. |
| `doroti_graphite_vk_texture_set_state` | Updates tracking after the caller queues/completes the actual barrier. Performs no Vulkan barrier itself. Shares state with the wrapped Graphite surface. |
| `doroti_graphite_vk_insert_recording` | Adds borrowed binary wait/signal semaphores to the recording. Native wrapper arrays survive insertion; the caller retains actual Vulkan semaphore handles until consumption. |
| `doroti_graphite_has_unfinished_gpu_work` | Read after `CheckAsyncWorkCompletion`. Does not acknowledge platform presentation or physical scan-out. |

The probe uses one graphics queue, one owner thread, and at most one outstanding copy. Each size owns one image, allocation, backend wrapper, surface and recorder through twelve frames. It uses the real queried layout for the Graphite → transfer barrier, copies into a mapped verification buffer, transitions back to `General`, waits for the copy fence, and updates Graphite state. The wrapper is reused. This CPU readback is a verification sink, **not a proposed product output route**. The final device-idle is diagnostic teardown only; frames use `Submit(Sync=false)` and finite copy-fence waits.

The pinned `VulkanCaps` requires `INPUT_ATTACHMENT` together with `COLOR_ATTACHMENT` for renderable external textures. The original attempt without it returned a null surface; the failure remains recorded. Usage now also includes sampled, transfer-source and transfer-destination bits on the actual image and its Graphite descriptor.

## Remaining bridge/package gates

- The original context C ABI cannot pass enabled feature/extension chains. This probe enables **no optional device features or extensions**. A product bridge must forward the exact enabled chain and extension list, with validated allocator/callback ownership; supported features must not be reported as enabled features.
- No final-target-state API, native completion callback binding, device-loss recovery, outstanding-work shutdown qualification, or presentation-terminal binding is claimed by ABI 1. The polling path qualifies only the tested same-queue copy.
- Windows D3D11 dedicated-image copy and Windows Presentation slot return must be joined to Graphite and validated separately. Linux/Android WSI, split queues, acquire/present semaphore consumption and swapchain destruction remain open.
- The Apple follow-up qualifies the tested M1 Metal drawable ordering and completion release with the stock asset. Apple device loss/iOS/Catalyst qualification, Android lifecycle and other Vulkan RID rebuilds remain open. Do not promote this DLL into the product feed or share its C++ pointers with another Skia DLL.
- The build manifest is provenance for a local diagnostic build, not a reproducible RID package or clean-publish PASS. Native dependencies, licenses and managed/native ABI must be included in the eventual package workflow.

Skia source licensing is the pinned checkout's `LICENSE`; upstream native-package notices are in `SkiaSharp.NativeAssets.Win32/4.154.0-preview.1.26454.9/LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`. Preserve them with any future redistributed asset. The local build does not redistribute or relicense upstream source.

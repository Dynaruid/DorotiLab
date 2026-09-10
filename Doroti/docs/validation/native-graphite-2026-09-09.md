# Native Graphite NG0/NG1 execution — 2026-09-09

This report preserves the earlier Windows execution. The later Mac execution, shared Metal session and explicit AppKit product candidate are recorded in the [Apple follow-up](native-graphite-apple-2026-09-09.md). The matrix below describes the Windows run, not the later cumulative state.

**Overall: PARTIAL. The five-OS product migration is not complete.** The native backend defaults and central NuGet versions have not been promoted. Windows native interop now has executable probes and a locally rebuilt same-Skia bridge; NG1's complete output/lifecycle/package gate is still open. This is the original execution status; the plan and subsequent decisions are preserved in the [archived summary](../../../history/26-09-10/native-graphite-and-android-scroll-summary.md).

## Implemented and executed

- Extended the existing Windows Vulkan capability project with an independent `--graphite` mode. It records the actual loaded managed/native library paths and SHA-256, backend availability, GPU/driver/LUID, budgets, enabled features/extensions, Vulkan validation and operation checkpoints before native calls.
- Added a pinned source build integrating five C ABI exports into **the same** `libSkiaSharp.dll`. It queries/updates persistent Vulkan texture state, inserts wait/signal semaphores, and exposes pending GPU work. No second Skia library owns those C++ pointers. [Build and ABI instructions](../../native/graphite/README.md).
- Tested both the original NuGet DLL and rebuilt DLL on the RTX 4060 Laptop GPU and Radeon 780M. Context/recorder creation, raster image upload, color/text/gradient/offscreen drawing, Snap/Insert/Submit, asynchronous readback and normal teardown passed. Pixel assertions cover color, raster upload and offscreen image markers; text/gradient appearance has no independent visual acceptance.
- Tested the rebuilt bridge with 64, 128 and 96 pixel external textures, twelve alternating frames each: **36 frames per GPU, 72 total**. Each size retains one wrapper across all frames. Graphite renders, external Vulkan commands copy into a verification buffer, the actual barrier returns the image to `General`, and the next Graphite frame reuses it. Every pixel matched. Wait/signal semaphores, copy fences and tracked state updates were exercised.
- Enabled `VK_LAYER_KHRONOS_validation` **and synchronization validation**. Final runs on both GPUs produced zero warnings/errors. Frames use `Submit(Sync=false)`; there is no per-frame device-idle. The maximum outstanding copy is one. The mapped CPU buffer is a verification sink, not a product presenter.
- Original versus rebuilt offscreen RGBA hashes match on both GPUs for all three sizes. This is a small probe-scene parity result, not Material/native-host parity or a performance result.
- Ran the original Vulkan/D3D11 capability mode independently: PASS on both GPUs. FCR-7 Material/widget runtime contract and Linux Qt ABI/key/clipboard contracts: PASS with external 1,200-second timeouts.
- Corrected current README EN/KO and `work3.md` WebGPU/Vulkan descriptions, preserved ADR-027's historical decision while adding the current state, and aligned the Linux manifest to the real `doroti.qt-host/v2` and QOpenGLWindow/Ganesh path.

## Environment and artifacts

Baseline commit: `a92e687e1444258fb265eb92f8aea43de052f238`; working tree was clean at initial inspection. SDK: .NET `10.0.400`. Windows GPU drivers: NVIDIA `32.0.16.1062`, AMD `32.0.13031.3015`. Actual API versions reported by the two devices: Vulkan 1.4.341 and 1.3.302; the probe creates a Vulkan **1.1** instance and enables **no optional device features/extensions**.

| Item | Identity / evidence |
| --- | --- |
| SkiaSharp package | `4.154.0-preview.1.26454.9` |
| SkiaSharp source | `143a933a753dbfeca1909524b2c06c546c5c3e20` |
| Skia submodule | `cc43af052d3d98e605bee4ddc98671dafded1c57` |
| Context / recorder budget | 256 MiB / 64 MiB |
| Local build | `Doroti/artifacts/native-graphite/build-runs/20260909T094446Z/manifest.json` |
| Bridge + synchronization validation | `Doroti/artifacts/native-graphite/runs/20260909T095414367Z/` |
| Original NuGet + synchronization validation | `Doroti/artifacts/native-graphite/runs/20260909T095655847Z/` |
| Contract baselines | `Doroti/artifacts/native-graphite/baseline/` |
| Versioned execution record | [native-graphite-execution.json](../../../history/26-09-09/native-graphite-execution.json) |

The versioned record embeds the final reports, native asset hashes, build manifest, current target manifests, first interop failures and hashes of larger local logs. Timestamped local directories retain build failures and raw output. Probe/runner exit **2** denotes PARTIAL; a successful capability-only run still exits 0. The source build passed all 1,647 compile/link actions. Export/import inspection and live loading verified the added C symbols; this is not clean RID packaging or an installation test.

## Preserved failures and findings

1. The initial external image omitted `VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT`. `SKSurface.Create` returned null on both GPUs with zero Vulkan layer errors. The pinned Graphite `VulkanCaps` requires input-attachment usage for renderability. The actual image and backend descriptor now include it. Failure reports remain under the earlier timestamped runs and in the versioned execution record.
2. Initial probe builds used newly obsolete SkiaSharp image/text overloads. Warnings-as-errors rejected them; updated sampling/text-alignment overloads build with zero warnings/errors.
3. Native bootstrap first hit Windows command quoting and the Store `python3.exe` alias. It now captures the VS x64 environment with correct quoting and passes the actual Python executable to GN. Build logs preserve these attempts.
4. Public package API reflection confirms no texture-state methods and no enabled-feature/extension fields on `SKGraphiteVkBackendContext`; insert/submit structs do not expose native semaphore/finish fields. The local ABI 1 solves only the tested state/semaphore/polling portion.

Pinned primary sources: [SkiaSharp Graphite context](https://github.com/mono/SkiaSharp/blob/143a933a753dbfeca1909524b2c06c546c5c3e20/binding/SkiaSharp/Gpu/Graphite/SKGraphiteContext.cs), [Vulkan C shim](https://github.com/mono/skia/blob/cc43af052d3d98e605bee4ddc98671dafded1c57/src/c/sk_graphite_vulkan.cpp), [VulkanCaps](https://github.com/mono/skia/blob/cc43af052d3d98e605bee4ddc98671dafded1c57/src/gpu/graphite/vk/VulkanCaps.cpp), [native recording and completion contracts](https://github.com/mono/skia/blob/cc43af052d3d98e605bee4ddc98671dafded1c57/include/gpu/graphite/GraphiteTypes.h).

## Support and completion matrix

Installed package presence is an inventory finding, not proof that a target's loader selected it. Only the win-x64 probe's actual loaded asset has been executed here. Android `adb devices` returned no connected devices. This Windows machine has .NET Apple workloads but no executable native macOS/Xcode or Apple device validation environment. WSL installations do not qualify physical Linux Qt/Wayland/X11 output.

| Target | Current product → desired | Native asset inventory | Implementation / build / execution / physical |
| --- | --- | --- | --- |
| Windows App SDK win-x64 | Ganesh/Vulkan → Graphite/Vulkan | Win32 win-x64 DLL present; actual probe path hashed | bridge only / probe+native PASS / offscreen+external copy PASS / notVerified |
| Windows MAUI win-x64 | Ganesh D3D/GL → Graphite/Vulkan | same Win32 package | adapter not implemented / notVerified / notVerified / notVerified |
| Linux x64 | Qt GL Ganesh → Qt Vulkan Graphite | linux-x64 SO present | not implemented / ABI contract only / notVerified / notVerified |
| Android arm64/x64 | MAUI GL Ganesh → Vulkan Graphite | both SOs present | not implemented / notVerified / notVerified / notVerified |
| macOS arm64 | AppKit Metal Ganesh → Metal Graphite | macOS dylib present | not implemented / notVerified / notVerified / notVerified |
| iOS device/simulator | UIKit Metal Ganesh → Metal Graphite | device/simulator frameworks present | not implemented / notVerified / notVerified / notVerified |
| Mac Catalyst arm64 | UIKit Metal Ganesh → Metal Graphite | framework archive present | not implemented / notVerified / notVerified / notVerified |
| Web | Graphite/Dawn WebGPU default; explicit WebGL | WASM assets present | no renderer change / FCR-7 source contracts PASS / browser notVerified / notVerified |

Android currently still declares API 21. API 24 is the plan's preliminary lower bound for Vulkan; final minimum/API-feature/device capability policy remains unresolved pending the actual renderer/loader contract. Apple GPU-family and simulator policy also remains unresolved. This work does not silently raise deployment minimums before implementing the new host.

## Remaining work and gate

| Stage | Status | Remaining completion condition |
| --- | --- | --- |
| NG0 | PARTIAL | Actual host/publish identities, same-work Material/Cupertino platform traces, performance/memory baselines, final support policies |
| NG1 | PARTIAL | Full enabled-feature/extension/allocator contract; actual output resource roundtrip; context/device loss and outstanding-work shutdown; Metal; all required RID packages |
| NG2 | TODO, NG1 prerequisite open | Shared native session, generation/cache/effect ownership, renderer integration, platform terminal ledger |
| NG3 | BLOCKED by NG1/NG2 | App SDK + MAUI Graphite integration, D3D11/Presentation reuse and Windows lifecycle/IME/UIA qualification |
| NG4 | BLOCKED by NG1/NG2 + Apple execution environment | Native Metal view/handlers, drawable completion/lifecycle, Mac/iOS/Catalyst builds and devices |
| NG5 | BLOCKED by NG1/NG2 | Qt Vulkan ABI/presenter/templates and real Linux output/input tests |
| NG6 | BLOCKED by NG1/NG2 + connected device | Android Vulkan view/handler/bridge/lifecycle and device tests |
| NG7 | PARTIAL documentation only | Product promotion, old native-path removal, package/template/publish/doctor synchronization |

`work.md` explicitly makes NG1 a prerequisite to product conversion. Passing a local external-copy probe does not establish Windows Presentation slot ownership, WSI present-semaphore consumption, Metal drawable release, device-loss safety or cross-RID ABI packaging. Product switching/removal is therefore still pending. Native Graphite is **not** enabled in any product host by this change. The next implementation is completion of that bridge/package/lifecycle contract, followed by NG2 and the platform host work. No performance benefit or physical-display PASS is claimed.

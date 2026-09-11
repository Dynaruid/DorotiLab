# Official Graphite default and distribution — 2026-09-12

The active Vulkan hosts use public Graphite and official `SkiaSharp`/`SkiaSharp.NativeAssets.*` **4.154.0-preview.1.26454.9**. Custom Skia source builds, staging, ABI 3 binding and native package content have been removed from the active graph. [Retired source](../../../history/2026-09-12/work0-custom-graphite/README.md) and historical failures are preserved. Code/default selection is distinct from full product qualification, which remains **PARTIAL**.

## Loading and migration

Normal Windows App SDK and MAUI applications use the official `libSkiaSharp.dll` in the app directory; no environment override or external manifest is needed. `GraphiteNativeLibrary.Configure()` selects the pinned official catalog. Desktop selection checks native and managed package hashes, RID, loaded module path and public Graphite backend availability. It rejects another selected Skia module and retains the selected module for the process lifetime. An optional verified official manifest selects another copy of that official asset for diagnostics. Android checks the official ABI entry in the APK and the actual native module path. Qt uses the official Linux package.

The legacy `SkiaGraphiteVulkanOptions`/private `CreateVulkan` binding was removed. Direct session consumers use `CreateOfficialVulkan` with observed-state and completion contracts. Host users keep their entry points. R is a private Graphite image; P is separately owned by D3D/WSI. Copy restores R to its observed layout and ownership before reuse. A single owner serializes each generation; Vulkan submit success, GPU completion and platform retirement are separate boundaries.

The required Vulkan profile is **1.2**, one color subresource and a supported submission journal. Unsupported alias/dynamic-rendering/descriptor paths fail closed. Split graphics/present queues are not qualified. No native asset or rendering backend is silently replaced. Desktop single-file/static managed provenance is not qualified; Android Mono AOT is not NativeAOT. Apple retains its public Metal and native-link profiles.

## Platform scope

| Host / RID | Configuration | Available evidence |
|---|---|---|
| Windows App SDK / win-x64 | Official Win32 default, D3D11 imports and Windows Presentation | Prior AMD/NVIDIA correctness and delayed close; final package-only default consumer restore/publish/resize/Acrylic/run |
| Windows MAUI / win-x64 | Official Win32 default, private R and D3D12/Composition P | Product screen and asynchronous close; prior input/resize; full performance acceptance incomplete |
| Android / arm64, x64 | Official Android NativeAssets, one Skia `.so` per ABI | Prior Galaxy arm64 and API 36 x64 product input/scroll/lifecycle; final package-only APK inspection |
| Linux Qt / linux-x64 | Official Linux NativeAssets, descriptor carries Vulkan 1.2 | Code/package configuration only; build/X11/Wayland/runtime **skippedByUser** |
| AppKit / osx-arm64 | Official macOS NativeAssets, public Metal | Code/package configuration only; build/runtime **skippedByUser** |
| iOS / device and supported simulator RIDs | Official iOS NativeAssets, existing Metal/AOT configuration | Code/package configuration only; build/bundle/runtime/NativeAOT **skippedByUser** |
| Mac Catalyst / maccatalyst-arm64 | Official MacCatalyst NativeAssets, public Metal | Code/package configuration only; build/runtime **skippedByUser** |
| Web / browser-wasm | Official WebAssembly/Dawn assets | Existing Release build and ten product tests; evidence reused |

API 33 emulator without usable `vkCreateRenderPass2` remains a failed observation; API 36 success does not erase it. Actual GPU reset, permanent stalls, physical scan-out, the full DPI/multi-window/feature matrix and repeat-median performance acceptance remain unverified. The prior MAUI comparison exceeded the initial p95 budget. Combining copy barriers and reusing rooted observer delegates reduces avoidable calls; its performance benefit has not been measured. Long performance runs were not restarted after user feedback.

## Close and lifetime

Logical close blocks frame admission and hides the window within five seconds. Pending GPU/native owners remain retained until actual completion or real device loss. At most one retained generation is allowed; timeout never becomes device loss. MAUI's AppWindow close keeps the UI dispatcher alive while raster workers and Composition callbacks drain, then closes the native window. Timeout is logged while awaiting actual retirement; failed completion retains the owner and does not manufacture exit-code-zero success. Retirement of an indefinitely stalled GPU is not guaranteed.

## Reproduction

Use normal application `dotnet run`/publish commands. Custom Skia checkout, GN/Ninja and staging are not prerequisites. These standalone runners apply the repository's 1,200-second timeout to every child. They are targeted checks, not a list to rerun after documentation changes.

```powershell
python Doroti/validation/stock-graphite-vulkan/run-clean-windows-consumer.py
# After packing the Android host into the same local feed:
python Doroti/validation/stock-graphite-vulkan/run-clean-android-package.py <local-package-feed>
```

The Windows consumer lives outside the repository, uses a fresh package cache and PackageReferences only, and executes the actual Windows host. Android's packaging consumer checks archive provenance without repeating device qualification. The local Android-only MAUI package is a validation slice, not an all-platform release package; Apple/Linux release packaging was not built. No package was pushed to a public feed. New-app templates inherit the same target/runner asset graph; separate generated-template runtime qualification was not repeated.

[Current cutover evidence](../../../history/2026-09-12/validation/stock-graphite/2026-09-12-cutover/README.md) extends [earlier evidence](../../../history/2026-09-12/validation/stock-graphite/2026-09-12/README.md). Original command failures and corrected scoped passes are retained separately. Upstream notices remain in [THIRD-PARTY-NOTICES](../../THIRD-PARTY-NOTICES.md) and official NuGet packages.

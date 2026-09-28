# Current support and evidence

M5-A update (2026-09-28, same base revision + 07 working tree): Windows App SDK now
registers a Copy-only OLE drop receiver for files, Unicode text and URI lists.
Common routing/lifetime, real 5GB sparse-file reads at 4GB offsets, 96/192 DPI
coordinate contexts and a NuGet-only Release consumer pass. Actual OLE drag source
fixture delivery passes for two files, Korean text and a URI using computer-use input.
Explorer-to-Testbed drag could not be performed because the UI tool restricts drag
endpoints to the source window; it remains **notVerified**, as do live monitor-DPI
changes and physical input. Other adapters, OS sending, move/link and virtual files
are not implemented. See [OS drop contract](os-drag-drop.md) and [07 results](../../works/common/07-os-drag-drop.md).

M4 update (2026-09-28, `36b566d11741d2f4971366a75169ce9c06044904` + local changes):
`Doroti.Plugins` and the Windows App SDK FilePicker/URL adapters pass a separate
NuGet-only Release consumer. Real OS dialog selection, user/caller cancellation,
owner-close drain, read-grant disposal and default-browser loopback HTTP pass with
synthetic dialog commands. The common plugin consumer also passes Release trimmed
publish/run. ABI/RID/duplicates/missing handler/assets, denied/unsupported, late replies
and large-file offsets have contract regressions. Native event streams and other
platform adapters remain unfinished; physical input, actual ACL denial and full
Windows renderer trimming/AOT are not qualified. See [plugin contract](plugins.md)
and [M4 results](../../works/common/06-plugin-sdk.md).

Updated 2026-09-28 against `a93c047fe2e93d93cff3e0a6bf3c2789862fea81` plus the local 00–05 changes. This is the current cross-host index. Older reports remain historical; a build or synthetic input test does not establish physical input, accessibility, performance or AOT support.

| Platform / host / renderer | Implementation | Build mode verified now | Automated execution | Visible / physical evidence now | Remaining |
| --- | --- | --- | --- | --- | --- |
| Windows App SDK / default Graphite Vulkan + native D3D12 presentation | Main Desktop companion, existing native editor/WebView adapters | Debug JIT, win-x64, .NET SDK 10.0.400, Windows 10.0.26200 | Actual HWND resize 500×650 DIP, title/state, hide/show, minimize/restore/maximize/fullscreen, caption theme/reset; native and API close cancel then allow, registry empty; editor and WebView each created/recreated | Native state and successful presentation receipts; desktop UI automation unavailable (`native pipe is unavailable`), live resize pixels and physical IME **notVerified** | Mixed-monitor DPI, live drag/flicker, physical Korean candidate/caret, Tab/Shift+Tab, UIA, GPU-loss/AOT. Single window and OnLastWindowClosed only. Backdrop/background mutation requires recreation; transparent backdrop is unsupported. HWND editor + WebView in one composition frame unsupported |
| Web / Chromium / requested worker-direct-webgl | Existing browser host and DOM semantics | Debug browser-wasm build | HTTP startup and nonempty runtime JS | Chrome displayed Material Testbed and populated semantics tree at localhost; physical input **notVerified** | Other browsers/devices, release/AOT, performance; current smoke does not requalify DOM PlatformView input |
| Host-neutral / Doroti.Testing / Skia CPU | Serial virtual clock, frame dispatch, pointer packets, finder/drag/key/text/semantics API, resize/pixels/traces/cache | Debug net10.0; separate NuGet-only consumer | Cupertino pointer tabs, bounded settle, Hangul synthetic editing, unmount; DPR pixels, blur capture policy, 1,000-row scroll/reassemble, zero layer delta | CPU offscreen only | Full GPU blur/golden quality, Dialog, GPU/native/physical coverage; static binding parallelism is deliberately rejected |
| Windows MAUI | Existing main-window Desktop adapter | **notVerified** in this change | Historical evidence in [Desktop](desktop-windows.md) | **notVerified** | Separate WebView wiring and host qualification |
| AppKit / Metal | Existing main-window Desktop and native view adapters | **notVerified** | [Historical Desktop summary](../../history/26-09-26/desktop-window-api-summary.md) | **notVerified** | AppKit capability restrictions, VoiceOver, physical input and additional windows |
| Mac Catalyst | Existing restricted scene Desktop adapter | **notVerified** | Historical summary above | **notVerified** | Separate scene and native view qualification |
| Linux / Qt Quick | Existing basic Desktop and native view adapters | **notVerified** | [Historical PlatformView summary](../../history/26-09-21/platformview-webview-summary.md) | **notVerified** | Physical IME/Orca, packaging and additional windows |
| Linux / Qt Widgets | Native child-view path; Desktop companion unsupported | **notVerified** | Historical PlatformView summary above | **notVerified** | No Quick/WebEngine/effects parity claim |
| Android / MAUI / Vulkan | Existing mobile host and native views | **notVerified** | Historical PlatformView summary above | **notVerified** | Device IME/TalkBack/lifecycle and release qualification |
| iOS / UIKit / Graphite Metal | Existing native-view adapter | **notVerified** | [PlatformView matrix](platform-views/support-matrix.md) retains simulator history | **notVerified** | Physical VoiceOver/IME/device/AOT; Ganesh and Catalyst are separate |

The 04–05 increment adds CPU DPR/pixel/list/reassemble and zero-retained-layer regressions, Web resize queue/memory policy tests, CLI development sessions, and a local VSIX. Installed-VSIX Windows metadata reload preserves State/count/Hangul text/scroll; compile-error correction, serialized requests and Stop pass. Web generated-app Chrome display, counter input, Korean glyphs and 450×800 resize pass; Web metadata Hot Reload now passes in desktop Chrome with the main-owned threaded Debug runtime (WebGL and WebGPU), including installed-VSIX request/frame acknowledgment and compiler-error retry. Release/AOT, worker-owned runtime and physical mobile reload are not qualified. GPU phase/present/VRAM budgets, physical mobile/loss/offline-font qualification and full wizard UI exceptions remain unverified. See [rendering results](../../works/common/04-rendering-lifetime.md) and [editor results](../../works/common/05-vscode-hot-reload.md).

Reproduction and suite boundaries: [testing guide](../tests/README.md). Hosted CI is configured but has not been executed remotely in this change. Windows GPU smoke is opt-in on a labelled interactive self-hosted runner; it is not a headless build claim. No full cross-platform `release`/signing/install qualification was performed.

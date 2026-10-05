# Current support and evidence

2026-10-05: [Android/common follow-up](../../history/26-10-05/work3-summary.md#19-2026-10-05-android와-공통-잔여-실행)
qualifies Galaxy S25 arm64 and Pixel 5 emulator/x64 native lifecycle subsets,
logical surface reuse after Activity replacement, joined Vulkan retirement,
synthetic native IME focus/composition and 60-second foreground frame progress.
The provider-packaged ADB adapter has typed planning and nonce-matched graceful
Stop; actual CLI and installed VSIX Run/Reload/Restart/Stop pass. Three independent
prebuilt design consumers publish and show native first frames. Sample2's Release
trim/profiled Mono AOT lifecycle passes separately. A common replay scene resource
race and lost frame callback after a zero-view interval are fixed; fresh common
Build/G0 and WindowsSmoke pass. Physical IME/TalkBack, scanout, production signing,
clean-machine distribution and the full plan remain unqualified. Exact hashes,
profiles and remaining source/runtime gates are in the linked receipt.

2026-10-05: [work3 Apple/common follow-up](../../history/26-10-05/work3-summary.md#14-2026-10-05-실행-결과)
adds provider-specific logical WindowRequest mapping, AppKit owned panels/sheets
and typed NSMenu, shared mobile application sessions with per-view drain, iOS
device discovery/selection, and provider-owned runtime profile imports. CPU
route/context-menu and font lifetime regressions are included. Fresh Apple builds
and native API runs are recorded separately from historical candidates. Mobile
typed iOS development transport and caller-backdrop sampling for native Cupertino
previews remain Unsupported; full staged coordinator, package/profile and physical
acceptance remain partial. See the linked receipt for exact profiles and remaining work.

The [2026-10-05 physical iOS follow-up](migrations/design-platform/work3-ios-device-verification-2026-10-05.json)
passes development-signed Debug Mono `net10.0-ios27.0/ios-arm64` on iPhone 12 /
iOS 26.6.1: native services, synthetic editor/WebView recreation,
WKWebView/Semantics, rotation functional correctness, joined Stop/Metal retirement
and three same-PID background/foreground cycles with progressing GPU completions.
Reachable paired phones with idle CoreDevice tunnels are now discovered after a
bounded connection check. The user reported no observed issue in brief manual use.
Rotation uses UIKit's system cadence without an application FPS cap; timing is
recorded, and the optional strict phase budget is not qualified. Device
Release/NativeAOT, formal physical IME/VoiceOver, scanout FPS and clean distribution
remain separate. App data was preserved and the normal sample was left foreground.

Later on the same day, the user reported severe rotation stutter on the actual
Components screen. The simpler reload-scene functional PASS did not qualify that
screen's performance. The [Release Mono AOT comparison](migrations/design-platform/work3-ios-release-aot-verification-2026-10-05.json)
records full application/entry/dependency LLVM AOT with interpreter disabled,
development-signed installation and the six native/functional cases passing on
iPhone 12. Mean viewport update gaps improved from 107.50/93.97ms to 39.96/37.89ms
on the same Components scene; this is not scanout FPS or strict phase-budget
qualification. The ordinary Release app is left foreground with profiling/evidence
disabled. Those early NativeAOT publish failures were superseded by the later
[iPhone NativeAOT installation](migrations/design-platform/work3-ios-nativeaot-device-verification-2026-10-05.json):
the original .NET 10 SDK and explicit native roots produced a signed installed app.
One initial input run observed Metal `NotEnqueued`; three independent rechecks
passed, with the cause unresolved. This remains a historical observation and is
not qualified by the newer common source changes without another Apple run.

2026-10-04 current-source follow-up: [full review](validation/2026-10-04-full-review.md) corrects Future/frame/semantics/startup/snapshot/tooling contracts and adds [doctor v4](doctor.md). Current automated and native API evidence is distinct from historical rows and from physical/display/deployment acceptance; the final scope is recorded in the dated validation record.

2026-10-03 W0–W11: [current implementation, automated results and candidate identities](validation/2026-10-03-platform-gap-implementation.md). Windows MAUI adds native picker/overlays/WebView, actual GPU import and additional windows; App SDK has opt-in WinUI editor+WebView mixed composition. Windows UIA/Qt now consume actual layout text geometry. Web has explicit loss/restart, offline fonts and permission-free clipboard availability. MAUI Debug Run/Reload/Restart/Stop passes. The initial Android restart-mode candidate is superseded by the [Android metadata Hot Reload follow-up](validation/2026-10-03-android-hot-reload.md), with real Galaxy S25 state-preserving deltas and compile recovery. Apple validation of the new shared WebKit/semantics/pen code was **SKIPPED by user request** in the initial candidate. The subsequent [Apple follow-up](validation/2026-10-03-apple-platform-gap-followup.md) fixes native accessibility/input gaps and records new AppKit/Catalyst/iOS builds and native API checks. Historical rows retain their original candidate scope; product acceptance remains PARTIAL.

2026-09-29 Linux update: [Qt implementation and execution](../../history/26-10-03/works/results/2026-09-29-linux-qt.md).
Ubuntu 26.04/Qt 6.10.2 VM: Quick Wayland/XWayland, real additional windows,
FilePicker/URL, Copy drop, drag-source cancellation, navigation and package-only
Release/portable install pass. Physical IME/Orca, physical GPU and signed clean-OS
release remain unqualified.

2026-09-29 후속 구현·검증: [Web·Windows·Android 실행 결과](../../history/26-10-03/works/results/2026-09-29-web-windows-android.md). 전체 **PARTIAL**, 범위별 PASS와 미완료를 분리한다. CI workflow는 사용자 지시로 삭제 상태를 유지한다.

2026-09-29 follow-up (`c858ca9add07f20634c13d9e6177710811c1fc93` + working tree):
owner-thread/disposed test API guards, single-notification Explicit exit, corrupt
checkpoint and browser history/bfcache handling, and transactional portable install
retry/integrity regressions pass locally. Packages now has a dedicated CI job;
remote CI, physical input, real browser history and signed clean-machine release
remain unverified. [00–10 review](../../history/26-10-03/works/README.md#2026-09-29-0010-보강-검토).

2026-09-29 update (`8834d7596597b3087a0139f527148baee9a46583` + working tree):
Windows App SDK now supports additional native windows with dispatcher-scoped
framework/input state and both OnLastWindowClosed/Explicit lifetime. Real two-window
presentation, independent size, first-window close/survivor resize and final drain
pass, including a NuGet-only Release consumer. Physical IME, mixed-monitor DPI and
full native-content coverage remain unqualified. [Window context](desktop-window-context.md).

Opt-in navigation/restoration connects the existing Router and restoration buckets
to Windows protocol delivery, Web main-owned history and Android intents. Common
queue/dedup/corruption/round-trip tests pass. Windows cold/warm/restart works; Galaxy
S25 Android 16 Release Mono AOT shows the expected cold/warm/restarted routes on
screen. Browser tools were unavailable, so Web UI history is notVerified.
[Navigation contract](application-navigation.md), [release candidates](release-candidates.md).
Selected Windows/Web package-only Release publish now has a private feed/cache and
template workflow. Signing, clean-VM deployment and production updates remain
notVerified. The table below consolidates the dated results; each entry retains
its evidence scope. No runtime checks were rerun for this documentation update.

M5-A update (2026-09-28, same base revision + 07 working tree): Windows App SDK now
registers a Copy-only OLE drop receiver for files, Unicode text and URI lists.
Common routing/lifetime, real 5GB sparse-file reads at 4GB offsets, 96/192 DPI
coordinate contexts and a NuGet-only Release consumer pass. Actual OLE drag source
fixture delivery passes for two files, Korean text and a URI using computer-use input.
Explorer-to-Testbed drag could not be performed because the UI tool restricts drag
endpoints to the source window; it remains **notVerified**, as do live monitor-DPI
changes and physical input. Other adapters, OS sending, move/link and virtual files
are not implemented. See [OS drop contract](os-drag-drop.md) and [07 results](../../history/26-10-03/works/common/07-os-drag-drop.md).

M4 update (2026-09-28, `36b566d11741d2f4971366a75169ce9c06044904` + local changes):
`Doroti.Plugins` and the Windows App SDK FilePicker/URL adapters pass a separate
NuGet-only Release consumer. Real OS dialog selection, user/caller cancellation,
owner-close drain, read-grant disposal and default-browser loopback HTTP pass with
synthetic dialog commands. The common plugin consumer also passes Release trimmed
publish/run. ABI/RID/duplicates/missing handler/assets, denied/unsupported, late replies
and large-file offsets have contract regressions. At that time, native event streams and other
platform adapters were unfinished (superseded by the linked follow-up); physical input, actual ACL denial and full
Windows renderer trimming/AOT are not qualified. See [plugin contract](plugins.md)
and [M4 results](../../history/26-10-03/works/common/06-plugin-sdk.md).

Cross-host index reconciled from the 2026-09-28 results (`a93c047fe2e93d93cff3e0a6bf3c2789862fea81` plus 00–05 changes) and the 2026-09-29 updates above. A build or synthetic input test does not establish physical input, accessibility, performance or AOT support. Tool unavailability below describes the recorded runs, not a fresh check of current tools.

| Platform / host / renderer | Implementation | Recorded build evidence | Automated execution | Recorded visible / physical evidence | Remaining |
| --- | --- | --- | --- | --- | --- |
| Windows App SDK / default Graphite Vulkan + native D3D12 presentation | Desktop companion, additional windows, OnLastWindowClosed/Explicit lifetime, native editor/WebView adapters | Debug JIT and separate NuGet-only Release self-contained JIT consumer; win-x64, .NET SDK 10.0.400, Windows 10.0.26200 | Native size/title/state/caption, API/native close cancellation and drain; editor/WebView separately recreated; two HWNDs/editor islands, survivor resize and both lifetimes; protocol cold/warm/restart (2026-09-29) | Native state/presentation receipts; installed-VSIX metadata reload preserved State/input/scroll (2026-09-28). Live resize pixels and physical IME **notVerified** | Mixed-monitor DPI, physical candidate/caret/Tab/UIA, full two-window native content, GPU loss, full-renderer trimming/AOT, OS protocol registration and clean signed deployment. Metadata flow after the 09-29 changes needs revalidation. Backdrop/background mutation requires recreation; transparent backdrop and default legacy HWND editor + Windows.UI.Composition WebView mixing unsupported; opt-in WinUI mixed subset qualified separately on 2026-10-03 |
| Web / Chromium / main-owned threaded runtime / WebGL and WebGPU | Browser host, DOM semantics, metadata Hot Reload, history/sessionStorage connection | Debug browser-wasm; package-only Release publish with trimming/AOT disabled (2026-09-29) | HTTP/bootstrap smoke, admission/memory policy, bridge and Node history/storage/bfcache contracts | Chrome UI/glyphs/resize and installed-VSIX state-preserving metadata reload (2026-09-28); physical IME and actual history UI **notVerified** | Real back/forward/refresh/bfcache, DOM PlatformView input, other browsers/devices, physical mobile, performance/loss, worker-owned navigation/reload, Release browser runtime and AOT. Publish payload is not rendering evidence; metadata flow after 09-29 needs revalidation |
| Host-neutral / Doroti.Testing / Skia CPU | Virtual clock/frame dispatch, input/finders/semantics, pixel/trace diagnostics, owner-thread/disposed guards | Debug net10.0; separate NuGet-only consumer | Cupertino tabs, bounded settle, synthetic Hangul, DPR/blur geometry, 1,000-row scroll/reassemble, zero layer delta; Dialog mid-frame/modal hit-test, navigation/restoration and independent owner-thread isolation (2026-09-29) | CPU offscreen only | GPU golden/blur/Dialog quality, full GPU/native resource isolation and host physical input/semantics. Same-thread nesting and API calls outside the owning thread remain rejected |
| Windows MAUI | Main-window Desktop adapter, owner-local WinUI NativeOverlay button/editor/WebView2 and HWND file picker | Debug win-x64/.NET 10; Windows-only NuGet dependency/targets check (2026-10-03) | Separate MAUI native owner, embedded and Ganesh Composition, WebView widget/JS/content, picker/read/cancel, Sample2 upload/preview/cleanup; [W4 execution](validation/2026-10-03-windows-maui-basic-connections.md) | Automated native API and WebView screenshot; physical input/live resize quality **notVerified** | PARTIAL product acceptance. Disjoint overlays only; interleaving/native backdrop remain restricted; GPU import and actual multiple windows have separate 2026-10-03 automated evidence. Legacy child-HWND PlatformView unsupported; physical IME/Tab/UIA/DPI/loss/soak/clean signed Release install notVerified; local Release consumer is recorded separately |
| AppKit / Graphite Metal and Ganesh Metal | Metadata Hot Reload; main/additional NSWindow owners; file picker, Copy drop receiver, URL activation/restoration; native editor/WKWebView | Debug and package-only Release/CoreCLR (LinkMode=None), net10.0-macos27.0/osx-arm64 on Xcode 27; [current release evidence](../../history/26-10-03/works/results/2026-09-29-macos-appkit.md) | AppKit Debug real metadata delta/state preservation/compile recovery/Stop and installed VSIX (2026-10-01); both renderer Desktop/lifetimes, survivor resize, native view recreation, picker cancellation/grant lifetime, native pasteboard, warm LaunchServices and route restart | Two-window screen captures on physical M1/macOS 26.6.2/DPR 2; physical input/VoiceOver **notVerified** | Finder cross-window drop, OS drag sending, physical IME/VoiceOver, mixed DPI, full GPU/loss/soak budgets, migration/input restore and clean signed distribution |
| Mac Catalyst / UIKit / Graphite Metal | Metadata Hot Reload; scene Desktop factory, shared application lease, owner-local UIKit editor/WKWebView, picker/drop/navigation | Debug net10.0-maccatalyst27.0 plus isolated NuGet-only Release publish/native launch, maccatalyst-arm64 | Catalyst Debug real metadata delta/state preservation/compile recovery/Stop and installed VSIX (2026-10-01); separate native view recreation, OS warm URL, two scenes/independent resize/survivor and Explicit drain (2026-09-29) | Native API evidence; physical IME/VoiceOver **notVerified** | [Apple follow-up](../../history/26-10-03/works/results/2026-09-29-ios-catalyst.md): physical input/GPU budgets, native scene restoration, file drop/sending, provisioning and clean distribution |
| Linux / Qt Quick / Graphite Vulkan | Main/additional windows, isolated owners, native services/drop/navigation | Debug and isolated NuGet-only Release/JIT, Qt 6.10.2 | Wayland and xcb/XWayland Desktop/lifetimes, native editor/WebView recreation, picker/cancel/5GiB drop, IPC/restore, 20 resize cycles, portable install/update | Two rendered QQuickWindow captures in VM; software Vulkan, physical input/Orca **notVerified** | External drag success, mixed DPI, physical GPU/loss/soak, full input restoration, OS protocol registration and clean signed deployment |
| Linux / Qt Widgets / OpenGL | Disjoint native child-view path; Desktop companion unsupported | Debug native/managed Qt 6.10.2 | Wayland and XWayland QLineEdit parent/text/synthetic key plus 20 resize cycles | VMware SVGA3D; XWayland window capture, Wayland capture unavailable | Quick mixed input scene rejected by NativeOverlay contract; no interleaving/WebEngine/effects/Desktop parity claim |
| Android / MAUI / Vulkan | Existing mobile host/native views plus Activity intent/Router/restoration connection | Source-based Testbed Release Mono AOT, Galaxy S25 / Android 16 (2026-09-29); final revised APK rebuilt but not reinstalled | ADB VIEW intent cold/warm and force-stop/restart | Expected `/first` → `/second` → restored `/second` captured on device; physical touch/IME **notVerified** | Reinstall/retest final revised APK, IME/TalkBack, full PlatformView/lifecycle/input-state restoration and performance, Android package-only, signing and clean release qualification |
| iOS / UIKit / Graphite Metal | Native-view adapter plus UIKit picker, text/URI Copy drop and scene activation callbacks | Debug plus isolated NuGet-only Release simulator build/native launch; net10.0-ios27.0 / iossimulator-arm64 (2026-09-29) | Separate native editor/WKWebView recreation | Simulator input-scene capture; physical IME/VoiceOver **notVerified** | [Apple follow-up](../../history/26-10-03/works/results/2026-09-29-ios-catalyst.md): OS link confirmation/association, source Debug development-signed iPhone 12 install/launch and 1170×2532 screen verified in the follow-up; physical input, device Release/AOT, GPU/lifetime and distribution remain separate. Ganesh/Catalyst remain separate |

The 04–05 increment adds CPU DPR/pixel/list/reassemble and zero-retained-layer regressions, Web resize queue/memory policy tests, CLI development sessions, and a local VSIX. Installed-VSIX Windows metadata reload preserves State/count/Hangul text/scroll; compile-error correction, serialized requests and Stop pass. Web generated-app Chrome display, counter input, Korean glyphs and 450×800 resize pass; Web metadata Hot Reload now passes in desktop Chrome with the main-owned threaded Debug runtime (WebGL and WebGPU), including installed-VSIX request/frame acknowledgment and compiler-error retry. Release/AOT, worker-owned runtime and physical mobile reload are not qualified. GPU phase/present/VRAM budgets, physical mobile/loss/offline-font qualification and full wizard UI exceptions remain unverified. See [rendering results](../../history/26-10-03/works/common/04-rendering-lifetime.md) and [editor results](../../history/26-10-03/works/common/05-vscode-hot-reload.md).

2026-09-30 iOS development adds CLI and VS Code metadata sessions on macOS. Source Testbed on iPhone 18 Pro Simulator (iOS 27.0, SDK 10.0.401, Mono 10.0.12, Graphite-Metal) passed actual updates with PID/State/count/Hangul/scroll preserved, compiler-error recovery, rude-edit retention and Stop. The separately installed VSIX passed Run/Hot Reload/Stop. The 2026-10-01 follow-up passed physical iPhone 12 USB metadata updates, state preservation, compiler-error recovery, installed-VSIX Run/Hot Reload and Stop with the experimental .NET 11 CoreCLR profile and a pinned interpreter fix. .NET 10 Mono device sessions also passed the CLI and installed-VSIX scenarios over a local-network SDK WebSocket relay on the same iPhone; the development profile interprets framework callers to avoid a mixed-AOT metadata assertion. USB-only .NET 10 forwarding remains unsupported; toolchain requirements and scope are recorded in [iOS qualification](../../history/26-10-03/works/platforms/ios.md#2026-09-30-ios-metadata-hot-reload); simulator evidence does not qualify package-only development, Ganesh or Release/NativeAOT reload.

Reproduction and suite boundaries: [testing guide](../tests/README.md). Hosted CI is configured but has not been executed remotely in this change. Windows GPU smoke is opt-in on a labelled interactive self-hosted runner; it is not a headless build claim. No full cross-platform `release`/signing/install qualification was performed.

Release evidence remains tied to candidate `0.3.0-beta.rc.20260929010529` and its
generation-time manifest. The later local installer/shutdown/restoration fixes
were not repackaged into a new candidate during the recorded follow-up. A new
candidate and matching validation are required; see [release scope](release-candidates.md).

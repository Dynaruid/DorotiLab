# Native Graphite Apple follow-up — 2026-09-09

**Overall migration: PARTIAL.** This follow-up implements a shared Metal session and an explicitly selected macOS AppKit product candidate. Windows/Linux/Android conversion and iOS/Catalyst Metal handlers remain unfinished. Product defaults and pinned packages have not been promoted. The prior [Windows report](native-graphite-2026-09-09.md) remains historical evidence.

## Implemented

- `SkiaGraphiteSession` owns one thread/context generation, recorder, bounded image-provider cache and three frame slots. It registers compatible GPU surfaces, submits asynchronously, supports asynchronous readback, rejects duplicate/cross-thread operations and refuses disposal with outstanding frames/readbacks. The platform owns presentation and terminal classification. See the [ownership contract](../architecture/native-graphite-session.md).
- `DorotiMacOSMetalView` selects this session with `DOROTI_MACOS_GRAPHITE=1`, preserves AppKit input/window/transaction presentation, passes the session as `MauiSkiaPaintContext.ContextIdentity`, bounds in-flight work, and returns Graphite resources on the owner thread after same-queue command-buffer completion. Runtime effects use a distinct native Metal backend ID. Handler release invalidates renderer GPU caches before context destruction. Ganesh remains the default and there is no automatic fallback when the candidate fails.
- Repaired the existing AppKit spike's obsolete renderer-host interface and scene admission. It now publishes exact viewport/build tokens, explicitly submits Ganesh work before presentation, compares the same retained scene with Graphite, and records actual loaded package identities. The initial compiler failure and earlier reports are preserved.
- Added contract/window modes to that existing validation project and a runner with external 1,200-second process-group timeouts. The actual app executable runs directly so native Metal validation logs are captured.

## Executed evidence

Environment: Apple M1, `osx-arm64`, macOS 26.6.2, Xcode 26.6 (17F113), .NET SDK 10.0.400, base commit `ad39cf56f43235b16da5bf54f7e62b42f2b4033b`. The working tree was clean before this follow-up. SkiaSharp remains `4.154.0-preview.1.26454.9`, source `143a933a753dbfeca1909524b2c06c546c5c3e20`. The probe actually loaded `libSkiaSharp.dylib` SHA-256 `1707f663fb7f58d1b83e3607f930275438ca01a5bb4eb0c67fcdf817787da1ca` from its app bundle; this stock asset reports Graphite/Metal available and creates a real context. No Apple native bridge rebuild was needed for these tested operations.

| Check | Result and scope |
| --- | --- |
| Metal external texture contract | PASS: 64/128/96 pixel textures, 12 alternating frames each; three recreated session generations; color, raster-upload, GPU offscreen and runtime-shader pixel assertions; async readback hashes retained |
| Text/gradient/blur | Draw/submit exercised in the texture contract; independent appearance approval remains notVerified |
| Ownership contract | PASS: cancellation, duplicate submit/completion rejection, owner-thread enforcement, independent recorders, frame bound and pending-work disposal rejection |
| Ganesh window baseline | PASS: 20 size changes, minimize/restore, hide/unhide; 45 command buffers completed; shutdown began with one in-flight frame; resources released |
| Graphite window candidate | PASS: same lifecycle/retained scene including a Doroti runtime shader; 44 command buffers completed; shutdown began with one in-flight frame; resources released; one intentionally stale completion rejected |
| Metal validation | Logs confirm `Metal API Validation Enabled`; no validation errors/warnings in the final texture/window/product logs |
| Product build / canonical launch | PASS: `doroti.ps1 build/run`, Release, osx-arm64. Launcher exit is recorded separately from scene presentation |
| Actual Material product content | Both Ganesh and explicit Graphite: two new scenes and two replays, five completed command buffers, zero frame/buffer errors, 2560×1376 at DPR 2, no CPU readbacks/full-frame copies |
| Canonical publish | PASS: generated `DorotiTestbedApp.MacOS-1.0.pkg`, 57,093,111 bytes; SHA-256 `aa88c0c2121023a7f239042d65aa548489f56ab547abc727edf2cd1a0acf1367` |
| Relocated package payload | PASS: expanded the pkg into a separate artifact directory and ran its app; Graphite Material content/replay and five completed command buffers confirmed. No installation into `/Applications` was performed |
| Existing regressions | Runtime shader contract PASS; full FCR-7 Material/widget contract PASS; Web host build PASS with zero warnings/errors |
| Web browser / performance / physical display | notVerified; a successful Web build is not WebGPU/WebGL browser validation |

The window probe uses a 256 MiB context budget for both renderers and a 64 MiB Graphite recorder budget. Its maximum in-flight count is three. It does not measure comparative performance, memory peaks or physical scan-out. The product's picture raster cache remains disabled pending separate qualification.

Product content tests deliberately stopped only their own processes after observing first-content and replay (`exitCode=-15`). They are **not** normal product shutdown tests. The separate window probe is the evidence for the tested in-flight completion/release protocol. Early canonical-launch reports could be written before the framework's first scene and were not treated as product-content PASS; persistent direct launches supplied that evidence.

## Reproduction and artifacts

```sh
python3 Doroti/validation/appkit-metal-spike/run-graphite.py
pwsh -NoProfile -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp -Platform macos -Rid osx-arm64
DOROTI_MACOS_GRAPHITE=1 DOROTI_TESTBED_MODE=sample DOROTI_RESIZE_FIXTURE=none \
  pwsh -NoProfile -File Doroti/eng/doroti.ps1 run -App DorotiTestbedApp -Platform macos -Rid osx-arm64 -NoBuild
pwsh -NoProfile -File Doroti/eng/doroti.ps1 publish -App DorotiTestbedApp -Platform macos -Rid osx-arm64
```

Omit `DOROTI_MACOS_GRAPHITE` to restore the existing Ganesh product path. The environment variable is a candidate selector, not a new default or an automatic recovery policy. The runner's `--mode` switch narrows probe execution; each build/test child is externally bounded to 20 minutes. Product first-content observation used a separate 30-second content deadline and bounded process termination.

Versioned reports, identities, source hashes, manifests and raw log references are in [native-graphite-apple-execution.json](../../../history/26-09-09/native-graphite-apple-execution.json). Local evidence roots under `Doroti/artifacts/native-graphite/apple-runs/`:

- `20260909T100529Z-baseline/`: initial missing `ViewEpoch`/`ResizeTarget` compiler failure.
- `20260909T102527379826Z/`: final Metal contract and Ganesh/Graphite window runs.
- `20260909T101357Z-regression/`: runtime shader, FCR-7 and Web build.
- `20260909T101930Z-product/`: canonical Release build/launch and early reports.
- `20260909T102115Z-product-content/`: persistent product first-content/replay observations.
- `20260909T102258Z-publish/`: publish, expanded payload, relocated execution and logs.

## Remaining gates

| Stage | Cumulative status after this follow-up |
| --- | --- |
| NG0 | PARTIAL: Mac environment, package inventory, identities and limited same-scene baseline added; complete platform work/performance/memory/support matrices still required |
| NG1 | PARTIAL: tested Metal output/readback/completion boundary added; Vulkan enabled features/extensions, Windows presentation ownership, device loss and required RID packaging remain open |
| NG2 | PARTIAL: shared Metal session, native backend ID, compatible captures and callback-based ownership implemented; Vulkan session, full image-export integration, all cache/effect cases and browser regression remain open |
| NG3 | BLOCKED by remaining Vulkan interop gates; Windows App SDK/MAUI product conversion not implemented |
| NG4 | PARTIAL: macOS explicit candidate builds, outputs and packages; iOS/Catalyst handlers and their device/lifecycle/input/accessibility checks remain open |
| NG5 / NG6 | Not implemented: Qt Vulkan ABI/swapchain and Android Surface/handler/bridge work still required |
| NG7 | PARTIAL: candidate docs and macOS package evidence added; no default promotion, old-path removal, clean installation or five-OS release qualification |

There is no connected Android device. `devicectl` lists an iPhone 12 as unavailable; no iOS simulator was booted during this run. The current Mac cache lacks the pinned iOS/Android native packages; other package inventory entries are not execution results. The Mac is now an available Apple validation environment, superseding the absence recorded in the earlier Windows report. No Windows or physical Linux execution target was supplied in this follow-up.

Further macOS gates include device loss/missing completion, reattaching a disposed native view, full product live resize and transaction presentation, Korean IME/VoiceOver/input, screenshot export and long-running memory/performance. The probe does not qualify every product lifecycle case. All five-OS completion checkboxes in `work.md` remain unchecked where their full criteria have not been met.

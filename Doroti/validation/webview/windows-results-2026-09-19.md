# Windows execution ledger — 2026-09-19

Status: **PARTIAL**. `sourceReviewed`, `build`, `automated`, `productLive` have
WindowsAppSDK evidence; `physical` and `nativeAot` are `notVerified`. The MAUI
runner has build evidence only and still lacks this composition adapter.

Environment: .NET SDK 10.0.400, win-x64, AMD Radeon 780M, product DPI 192,
WebView2 runtime 153.0.4234.32. The native host DLL hash and final source/package
hashes are recorded in `artifacts/webview/2026-09-19/windows/execution.json`.

| Gate | Result / artifact under Doroti/artifacts |
|---|---|
| WindowsAppSDK Release / final publish | PASS, no compiler warnings/errors; `windows-webview-final-publish.log` |
| Windows MAUI Release | Build PASS, 0 warnings/errors; `windows-maui-build.log`; no native composition/product approval |
| PlatformView common | 12 contract checks PASS; `windows-platform-common.log` |
| WebView common | owner/event lifetime, async command placement/close, stale/cancel/options PASS; `windows-webview-common.log` |
| Public API product | PASS including JSON/errors/cancel/stale, resources/range/origin, private/shared profiles/clear, ten create/dispose races, late JS disposal; `webview/2026-09-19/windows/223331/commands.txt` |
| Fresh published package | PASS commands and clean exit; `webview/2026-09-19/windows/final-lifetime/`; initial fresh-copy run also passed in `fresh-package-test/` |
| Composition + OS input | PASS touch injection, OS mouse, shield, two views/intermediate raster, move/resize, disposal/recreation; `platform-views/2026-09-19/windows/effects-224648/observed.json` |
| Forced GPU reset | Same gate PASS, requested=1/completed=1, failed terminals/debug errors=0; `platform-views/2026-09-19/windows/effects-225126/` |
| Native/raster spatial pixels | PASS 11-stage calibration from published package; `webview/2026-09-19/windows/calibration-225753/result.json` and PNGs |
| Existing WinUI regression | Pixel/30-frame continuity PASS in `platform-views/2026-09-19/windows/winui-regression/`; corrected navigation/scroll/edit/focus/remount sequence PASS in `winui-input-current/result.json` |
| 0/1/4 views × four workloads | All 12 exited successfully; `webview/2026-09-19/windows/workloads-225358/result.json`; measurements only, no performance approval |
| NativeAOT publish | BLOCKED at existing iOS-only DOROTIAOT002, before ILC/link; `windows-webview-nativeaot.log` |

Calibration: sigma 4/16 measured 3.989/15.947 for both native and raster, baseline
edge sigma 0.156 (pixel sampling), zero/reset difference 0. Saturation 0 gave equal
RGB channels; saturation 2 and independent blue tint matched their intended color
changes. These tests do not qualify every radius, multi-effect configuration,
media source, or cross-platform pixel equivalence.

Workload timing is the bounded last-128 raster/readback and UI commit samples,
not scanout, first content display or input latency. Four-view animation p95 was
15.37/5.09 ms respectively. Private memory reached approximately 1.2 GB in some
active workloads. Browser-process/GPU memory is unobserved. No pre-change baseline
with the same source/environment was reconstructed, so no regression or performance
acceptance claim is made.

Preserved failures:

- `221848/` and `222003/`: internally generated NavigateToString data URL rejected;
  fixed by admitting only the exact requested HTML navigation.
- `222121/`: Range test accidentally returned a Promise, which the API correctly
  rejected; the fixture now observes async fetch via a separate state query.
- `222431/`: AllProfile did not clear custom-scheme localStorage; the adapter now
  clears the fixed app origin explicitly and verifies the result after reload.
- Early effect runs: initial state queried before native load; then recreated
  content loaded while frame evidence remained stale after input. The latter was
  a real retained-frame/input-sequence problem, fixed by native revision tracking
  and requesting a framework frame without weakening the stale-input guard.
- Touch setup initially used contact ID 1 with a one-contact injector and failed
  before product dispatch; ID 0 resolved it. Mouse-after-touch now uses OS mouse
  injection instead of synthetic window messages.
- The WinUI wrapper's final navigation used the old five-tab coordinate and
  opened the newly added WebView sample. Only its test coordinate changed; the
  user's sample edits were preserved and the complete input sequence then passed.
- Initial workload UI timing omitted the WebView commit branch. The source now
  records it and reports sample counts; `workloads-224725/` is superseded by
  `workloads-225358/` for UI timing.

Remaining implementation/qualification: Windows MAUI host integration; full Tab,
Korean IME/UIA and physical pen/touch; same-process two product owners; complete
device/process failure matrix; file/download/permission/fullscreen app policy APIs;
media/protected source coverage; template and clean-machine install; GPU transport
comparison, memory/performance acceptance and Windows NativeAOT support.

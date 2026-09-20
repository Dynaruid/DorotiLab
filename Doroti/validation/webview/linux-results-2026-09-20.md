# Linux Qt execution — 2026-09-20

Overall: **PARTIAL**. This execution implements the public Qt WebView adapter and
color effects and exercises the available Linux environment. It does not close
all work1/work2 physical, policy, performance, distribution or NativeAOT gates.

Environment: Ubuntu 26.04.1 x64; .NET SDK 10.0.400; Qt 6.10.2;
WebEngine `6.10.2+dfsg-1`; KDE Wayland/XWayland; Qt Vulkan **llvmpipe**.
Chromium reports its separate VMware/ANGLE integration; this is not physical GPU
qualification. Runs use uid 1000 and the default Chromium sandbox.

Evidence root: [`Doroti/artifacts/webview/2026-09-20/linux-qt/`](../../artifacts/webview/2026-09-20/linux-qt/).
Failed attempts remain in separate directories. API/calibration reports include
product binary hashes, commands, exits and actual mapped native module paths.
Native source/template hashes are in `template-sync.json`; package versions,
apt candidate, dynamic dependencies and component sizes are separate files.

## Implementation

- Existing Quick item, coordinator, frame/session and retirement remain the owner
  of each browser. `QtWebViewSession` implements the public command/event contract.
  Creation asynchronously resolves bounded manifest resources, then returns to
  the Qt GUI dispatcher. The dispatcher now waits for asynchronous factory
  completion without blocking the GUI loop.
- New optional sibling shim, WebView ABI 1/32 bytes/features 0x1f. Host callbacks
  remain ABI 4/192 bytes and Quick parts remain 96 bytes. Version/size/thread/
  owner/stale rejection is explicit. Normal dispose unbinds before GCHandle free;
  owner close destroys native objects before managed cleanup. Late results cannot
  attach to a new owner/document.
- Navigation/HTML/reload/stop/history, state/features and bounded JSON JS are
  connected to public Quick APIs. Undefined, null, exceptions, Promises, cycles,
  oversized results, cancellation, stale document and dispose are distinguished.
  Qt does not expose the commit/first-content signal required to advertise those.
- Ephemeral profiles are isolated. Shared persistent storage is scoped by the
  manifest application ID. Trusted app scheme resources have exact routes and
  integrity checks; Range is blocked in the request interceptor as well as the
  scheme handler. Shared-profile per-view resource maps and full profile clear
  throw Unsupported. Cache/cookie-only clearing is not advertised as full clear.
- Trusted app messages use main-frame-only ApplicationWorld injection and a
  QWebChannel receiver exposing only `receive`. Native code checks current app URL,
  generation, ID/name/size; remote origins have no facade. App frames and remote
  subresources are blocked. This does not authenticate arbitrary Internet frames.
- Gaussian vertical output applies saturation after both blur passes, with common
  luminance coefficients and premultiplied clamping. Sigma zero and saturation
  0–2 work; independent tint and sharp child remain foreground raster. PV bit 4
  negotiates kind 4; legacy kind 3 preserves saturation=1.
- Input testing exposed a real focus handoff bug: clearing only WebEngine's inner
  editor let Qt restore the outer focus scope. The final native host clears both
  scopes and yields focus when a Doroti text client is installed.

## Executed gates

| Gate | Result and evidence |
|---|---|
| Debug/Release build, publish | PASS; `build-10.log`, `publish.log`, final `publish-close.log` |
| Common PlatformView | 12 PASS; `common-platform.log` |
| Common WebView | PASS owner/event/lifetime/stale/typed error; `common-webview.log` |
| Qt ABI/geometry/GPU | PASS; independent WebView layout; ten submit/rejected-commit/canceled-resize/budget cycles, `managed-contract.log` |
| Native Quick contract | PASS owner/prepare/geometry/input/effect, ten lifecycles/queue close; final `release-native-contract.log` |
| Actual native WebEngine | PASS two owners, version/size/thread/stale rejection, JS cancellation, unbind, ten lifecycles, own renderer SIGKILL→terminal failure→fresh recreation; `release-webview-close-order.log` |
| Product API | 31 PASS on relocated Release, both QPAs; `release-api-wayland`, `release-api-xcb`; final 32-check regressions `release-api-final` / `release-api-close` |
| Product composition/input | 19 checks with actual Vulkan validation-layer loading, each QPA; `product-wayland-validation`, `product-xcb-validation`; final Release `release-product-focus` |
| Product controls/lifetime | PASS ten captures and ten disposal/recreation cycles; `controls-wayland` (captures alone are not full C1–C6 approval) |
| Gaussian/color | PASS 11 stages native/raster, sigma 4/16, zero/reset, saturation 0/1/2 and tint; `calibration-wayland-1`, `calibration-xcb`, `release-calibration-wayland-2` |
| Fractional DPR | PASS same calibration at `QT_SCALE_FACTOR=1.5`; `calibration-dpr15` |
| Korean IME/native Tab | Qt synthetic composition yields ‘한글’ with compositionstart/update/end; Tab→button, Shift+Tab→input, focus return to Doroti text; `input-wayland-focus`, `input-xcb-focus`. Not physical IBus/keyboard or full framework traversal approval |
| Resize | ten rapid XWayland cycles, exit 0, no VUID in this attempt; `resize-xcb.*`. Earlier Qt WSI extent failure is retained, not claimed fixed |
| Dependency errors | helper/resources/locales missing → typed Unsupported; `dependency-errors.json` and matching logs |
| Template WebEngine OFF | native build PASS; no WebEngine/WebChannel in `template-off-dependencies.txt`; native template/source copies match |
| NativeAOT | actual publish rejected by existing iOS-only `DOROTIAOT002`; `nativeaot.log`. No bypass or AOT PASS |

The two-owner fixture uses actual Chromium under two native Quick windows; it is
not two complete Doroti application owners. Process loss is Chromium renderer loss,
not Vulkan device loss. Qt injected events are not physical input or Orca approval.
Initial API/product fixtures and performance captures preceded the final focus,
loading-state and callback-close corrections. Final API/native/input/composition
regressions are recorded separately; the last close correction reran API/lifetime/
process contracts. Gaussian code and workload rendering were unchanged.

Native/raster measured sigma at DPR 1 is **3.999 / 15.866** for requests 4 / 16.
Native and color reset mean differences are **0**. RGB source (180,80,60) maps to
(100,100,100) at saturation 0, (255,60,20) at saturation 2, and (90,40,158) with
independent half-blue tint. This is bounded numerical calibration, not complete
cross-platform color-space/driver equivalence.

## Workload observations

Relocated Release, Wayland, ten seconds per workload after warmup. All requested
0/1/4 browser items were visible. Values below are **Qt frameSwapped interval p95
in milliseconds**, not physical scanout/input latency or content-first-display.
No pre-change baseline or physical GPU exists for this execution. No performance
budget has been accepted.

| Views | Idle p95 | Animation p95 | Scroll p95 | Modal p95 | Peak observed process-tree PSS MiB |
|---|---|---|---|---|---|
| 0 | 45.14 | 47.41 | 45.83 | 46.42 | 188.7 |
| 1 | 46.65 | 60.90 | 61.04 | 63.48 | 381.3 |
| 4 | no intervals | 62.64 | 58.64 | 63.15 | 447.2 |

`workloads-wayland/result.json` includes p50/p95/p99, sample counts, visible items,
process counts, memory observations and host R/P diagnostics. Four-view idle had
no sampled swap interval: its percentile is **null**, not zero latency. PSS is the
readable process-tree subset; disappearing/unreadable processes are counted. In
this execution one zero-view/modal sample had an unreadable process during exit.
GPU memory and browser-internal copy cost are not separately observed. The results
do not justify a physical 60 Hz performance claim.

## Deployment and limits

Publish was copied to `/tmp/doroti-qt-release-0pqsj4p_/app`. API/calibration commands
remove development Qt/QML/helper path overrides. Their `/proc/self/maps` evidence
confirms both native shims came from this relocated app, not the Debug driver's
build tree. Host RUNPATH is `$ORIGIN`. `deployment.json` confirms no Qt libraries,
Chromium helper or engine .pak files were copied. A generated runtime manifest
records actual Qt/WebEngine build versions and the required system closure.
`standalone-result.json` also records an exit-0 live native capture with no
preload driver or development Qt paths. This is relocated execution on the same
machine, not an OS-clean/package-only installation.

Observed system WebEngine core/data/helper/Quick and WebChannel/QML packages total
approximately 270 MiB Installed-Size, excluding their general Qt/OS dependency
closure. Sizes are recorded in `runtime-cost-kib.txt`; helper/locales/QML are not
free dependencies. Installed Qt 6.10.2 is the tested distro build, not a guarantee
that all upstream Chromium security fixes are present. Source API floor 6.8.0 is
not a tested minimum-patch/security promise.

Remaining gates: native-origin GestureArena/nested parent scroll; full framework
Tab, physical Korean IME/Orca; two full product owners/full C1–C6/E1–E3, Vulkan
device loss and protected/media subtrees; public permission/file/download/fullscreen
policy controls; full profile storage deletion and general remote messaging;
physical GPU and before/after performance acceptance; NuGet/package-only clean
machine deployment and Linux NativeAOT. These were not relabeled completed,
unsupported-success or skippedByUser.

## Failures retained

- Initial build/QML adapter iterations preserve their logs. Qt 6 uses script
  collection value dictionaries rather than creatable WebEngineScript objects.
- When the command/profile QObject was registered with QWebChannel, unintended
  profile-property traversal and an abort were observed. The final bridge exposes
  only the message receiver; API/lifetime/process gates pass with that boundary.
- Missing Testbed Linux resource entries were added to the runner manifest.
- Range could be satisfied before the scheme job; explicit interceptor rejection
  fixed the reproduced negative test.
- First Release calibration metadata did not record mapped module paths in its
  capture branch. The validation driver was corrected; the retry passed.
- An initial-HTML replacement could finish without another LoadStarted signal,
  leaving the admission loading latch set. Terminal load callbacks now release
  the latch; ten immediate create/replace/dispose cycles cover the regression.
- Encoded command limits now allow JSON escaping overhead while retaining the
  2 MiB decoded UTF-8 bound. A 1.8 MiB Korean comment/script is covered.
- Native removal/owner close disconnects WebView callbacks before canceled GUI
  posts can release managed contexts or QML destruction can finish scripts.
- Synthetic input initially retained native focus after leaving WebView. The
  reproduced focus-scope bug was fixed and the regression is in the final gates.

See [Linux WebView contract](../../docs/platform-views/linux-webview.md) and
[reproduction](../linux-qt-quick/README.md) for public capabilities and commands.

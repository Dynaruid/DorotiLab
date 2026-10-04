# 2026-10-04 full-review follow-up

Execution of root `work.md`, R0–R8 / F01–F10, A01–A08 and D01–D07.
Base checkout: `4bbcd6eb95d32c092054815f78f63dcf19323fcf`; review baseline:
`61f0f23b8f112e8e277dc896cf0b2c6627aca878`. The original review file is absent.
The implementation uses the current source and plan; historical audit text
does not authorize unrelated work.

Status: **PARTIAL product acceptance**. F01–F10, A01–A08 and D01–D07 are
implemented with registered automated regressions. Available-host native API
and browser checks are separate from the remaining physical/device/release
gates. The [machine-readable record](2026-10-04-full-review.json) preserves
commands, current source identity, logs, environment and candidate hashes.

The “before” conditions below were confirmed from the initial source. A
separately built, unmodified before-candidate was not executed. After checks
inject the reported failure conditions into the production contracts.

## Finding coverage

| Finding | Failure trigger / resulting behavior | Executed regression / boundary |
| --- | --- | --- |
| F01 | Rejected, canceled or throwing captured dispatch and scheduled task | Typed/untyped CPU Future terminal checks; Debug/Release scheduler errors report once and the next task runs |
| F02 | Error recovery/filter escapes its owner | CPU real queues and ExecutionContext, false filter/original exception identity, throwing filter and nested Future |
| F03 | Draw runs before begin-frame microtasks | CPU `A M M2 B C`, warm-up, mid-frame phase, begin-observer/microtask errors recover to idle and the next frame runs |
| F04 | Dispose during partial runtime/GPU/surface/app/texture startup | Production raster role VM await barriers for standalone WebGL and main-owned WebGPU; real Chrome profiles boot/recover; pre-init texture teardown never creates GL |
| F05 | Snapshot is a dimension-only Image | Actual Skia RGBA/PNG/decode/repaint, retained clip/transform, RepaintBoundary and SnapshotWidget DPR/resize/cache/layer teardown |
| F06 | Empty origin list permits navigation | Production BrowserWebView Node and Chrome loopback null/empty/match checks; empty denial preserves src/generations and starts zero requests |
| F07 | Applied A → pending B → new A leaves B pending | Actual Windows MAUI bridge with controlled queue, stale generation/action-owner/dispose checks inside the host connection smoke |
| F08 | Owned `current.new` prevents installer retry/remove | Actual Linux ext4 install/retry/remove, replace failures before/after each promotion, previous selection/userdata/tamper/foreign-link checks |
| F09 | macOS Release lacks a current-run receipt | Shared Windows/macOS environment/receipt failure fixtures and actual Windows candidate native receipt; macOS execution SKIPPED |
| F10 | Compile/Clean deletes a sibling or follows a link | Real Windows and Linux MSBuild Build/Clean consumers, strict child/case/prefix/traversal/root/link rejection and unchanged byte sentinels |
| A01 | First detach/Close/Dispose skips remaining owners | Actual HostSession/dispatcher/registry throwing-owner fixture, all remaining owners attempted, shared capability disposed once, pending Future canceled |
| A02 | Null value loses checked/toggled/mixed state | Common Apple projection 0/1/2 and localized value policy; Apple native compilation/provider/VoiceOver SKIPPED |
| A03 | Same dimensions imply clone identity | True shared-storage clone chain, distinct images false, readback lease across original disposal and one final release on success/error |
| A04 | stdout EOF blocks a full stderr pipe | Actual child with 1 MiB per pipe, nonzero exit, timeout, cancellation and owned grandchild cleanup |
| A05 | Timed-out managed role leaves a late port/StartAsync alive | Production supervisor/endpoint with fake clock and accept/reject ACK, token cancellation, duplicate/foreign ports and no runtime pthread termination; actual browser managed boot and restart |
| A06 | One deficient canvas axis grows both axes | DPR/device/byte policy fixture, exact width sequence, actual Chrome height-only growth with unchanged backing width |
| A07 | Release assumes a `python` alias | Shared absolute interpreter selection; real PowerShell python3-only/python-only/missing fixtures, Unicode/space paths and launcher propagation |
| A08 | Stale Stop kills a newer Android app/listener | Kernel device/package ownership, duplicate-before-deploy, stale token/runtime/PID/start/listener tests; actual Android metadata/Restart/Stop SKIPPED because no ADB device |
| D01 | Selected target/backend/context is omitted | Real CLI workspace/MAUI/all/RID failures and structured report/exit checks |
| D02 | Wrong dotnet/CWD/global.json policy | Custom executable, iOS runner CWD, patch/feature/minor/prerelease fixtures and live selected SDK reports |
| D03 | Generic PASS ignores profile requirements | Project/resource/WGSL graph and platform prerequisites, generic Web without Node/Rust, device-independent build, live pinned compiler-development profile |
| D04 | Native Android/Apple only checks file names | Actual native-doctor missing-wrapper failure; OS wrapper/JDK/SDK/Xcode predicates implemented; Apple actual host SKIPPED |
| D05 | A hung/noisy tool hangs doctor | Both pipes drained with bounded retained output, timeout/cancellation tests, owned-process cleanup and continued independent probes |
| D06 | Failures omit context/report or disagree with exit | Actual JSON/Markdown/console/exit parity, malformed workspace/import failures, atomic v4 reports with input hashes |
| D07 | Doctor lacks execution regression | 11 actual CLI/interpreter tests registered in Source/Developer; live Windows and WSL reports kept distinct from fake tools |

## Executed scope

All rows below are scoped to the working-tree implementation recorded in JSON;
earlier rows from the same day have their own input/candidate epoch. Final
Runtime/Ui behavior is additionally checked in Debug/Release and independent
NuGet consumers after the frame-error recovery correction.

| Scope | Result and limits |
| --- | --- |
| Source / Developer / Targets / Packages | PASS; Source is also part of Developer. Final doctor and production-worker additions are in the maintained suites. Targets builds Windows/Web and runs the HTTP startup fixture |
| CPU Debug / Release | PASS, including Future ownership/terminal, scheduler errors, normal/warm-up frames and exception recovery, snapshot pixels/identity and robust cleanup. Existing CPU/input/layer regressions remain enabled |
| Node | 43 PASS: production WebView, policy, texture/lifetime, standalone/main-owned raster startup and managed supervisor/endpoint |
| Windows App SDK Debug | Native API/input/lifetime/OnLastWindowClosed/Explicit automation PASS; HWND/editor islands/survivor resize and cleanup. Synthetic input is separate from physical IME/Narrator |
| Windows MAUI Debug Graphite | Real connection/bridge smoke PASS, including A–B–A. Native control construction/action ownership is not a physical UIA speech test |
| Chrome 154.0.8037.93 | Threaded main-owned WebGL/WebGPU and isolated single-thread standalone WebGL PASS: actual .NET boot, context/worker loss/restart, first frame/focus, origin rejection, independent canvas-axis growth and 60-second stability. Headless captures do not qualify physical display/FPS |
| Linux ext4 | Installer and actual MSBuild guard fixture 3 PASS under WSL task `/tmp`; Windows tools fixture 2 PASS plus explicit Linux skip |
| Linux Qt Quick / Wayland Debug | Actual current-source build and native multiwindow/desktop/services/navigation/input/20 resize cycles PASS on WSLg, Qt 6.10.2, Graphite/Vulkan. Report says softwareVulkan=true, so physical Linux GPU/Orca/IME/scanout remains notVerified |
| Android MAUI Debug / android-arm64 | Final-source build with `DorotiAndroidDevelopment=true` PASS and APKs produced. One Gradle SDK XML v4/v3 metadata warning; zero errors. Device execution, Graphite/fallback runtime and metadata delta/Restart/Stop remain SKIPPED |
| Doctor live | Common, Windows App SDK, Windows MAUI, Web, Android build and compiler-development prerequisites PASS. WSL Linux prerequisites PARTIAL: app graph evaluation hits its 30-second probe limit. The actual Qt build/smoke passes separately; it does not turn that unresolved diagnostic into PASS |
| Fresh private-feed Release | `0.3.0-beta.review.20261004.2`: dotnet-new template, isolated cache, ProjectReference-free Windows/Web publish, native current-run multiwindow receipt and Offline Web browser. Candidate JSON in the tracked result retains package/payload SHA-256 |

The first local candidate `.1` qualified the implementation before the final
begin-observer/microtask exception recovery test and correction. It is retained
as its own run; `.2` is the final package/native consumer candidate. No package
was published externally. Passed validate runs remove their owned raw consumer,
cache and per-check logs; the retained outer logs and this result record are the
evidence, not an assertion that those deleted raw files still exist.

## Remaining acceptance

- Apple native build/property/VoiceOver and macOS package-only Release run:
  SKIPPED, no Apple host in this session. Common policy and receipt fixtures PASS.
- Android physical stale-Stop and metadata delta/error/restart/stop regression:
  SKIPPED, `adb devices -l` has no device. Mock ownership tests PASS.
- WSL doctor app graph: notVerified after a bounded timeout; the report remains
  PARTIAL/nonzero, even with separate successful Qt build/native automation.
- Physical IME/accessibility speech, monitor/DPI movement, scanout/displayed FPS,
  real GPU hardware coverage, signing/notarization and clean-machine deployment:
  notVerified/notMeasured. No deployment target or signing acceptance was chosen.
- Web/native/GPU-only snapshot scenes unsupported by CPU replay are explicit
  capability failures. Async-only snapshot hosts must reject sync capture.

## Contracts

- Captured Future callbacks retain queue acceptance, cancellation lifetime and
  execution context. Success/error/filter paths use the same owner queue.
  Synchronous scheduled-task failures complete the Future and report once.
- Normal/warm-up frames drain microtasks before draw while retaining scene
  transaction/view scope. Snapshot consumers close their transform scope and
  dispose temporary layers on failures.
- Applied-equivalent semantics replace pending snapshots and refresh action
  ownership; stale scheduled generations cannot resurrect B after A–B–A.
  Apple checked/toggled values are independent of selected traits. AppKit uses
  numeric off/on/mixed state; UIKit uses a localized state value and preserves
  textual value. [UIKit value contract](https://developer.apple.com/documentation/uikit/uiaccessibilityelement/accessibilityvalue),
  [AppKit checkbox value](https://developer.apple.com/documentation/appkit/nsaccessibilitycheckbox/accessibilityvalue()),
  [AX checked/mixed mapping](https://www.w3.org/TR/core-aam-1.2/#ariaCheckedMixed).
- Startup closure races each await and retires late resources. Cleanup attempts
  independent owners after failures, ends caller/role lifetimes, and uses a
  fatal report rather than a successful disposal ACK when cleanup fails.
  Unconfirmed GPU completion retains allocations. Managed ports require an
  accept/reject ACK and token cancellation; the runtime owns pthread Workers.
- Desktop canvas capacity grows only deficient axes; initial/growth admission
  validates backing pixels against device dimensions and the 256 MiB color
  policy. Headroom may collapse to the required size. This is separate from
  source-texture/effect admission and is not a GPU-memory/FPS measurement.
- Scene snapshots use the view's `graphics.scene-snapshot` capability. Skia's
  CPU replay creates transparent sRGB RGBA storage, PNG/readback and repainting,
  bounded to 64 MiB. Native views, GPU textures and GPU-filter-only scenes are
  rejected before allocation. Web/GPU-specific capture remains unsupported by
  this CPU capability; async-only hosts must reject sync capture. Clones share
  storage identity and retain readback leases across original disposal.
- Linux portable installation recovers only verified owned temporaries and
  restores the previous selection on publish failure. Userdata survives.
  TypeScript Compile/Clean validate the same strict child path, OS case rules,
  ancestors and reparse/symlink entries before deletion.
- Native release receipts bind first-frame/two-window/resize/close/survivor
  state to the current run and candidate version. Process output pipes drain
  concurrently; timeout/cancellation ends owned process trees. Python is
  resolved once for wrapper and child. Android dev holds a kernel device/package
  lease, checks runtime/PID/start identity before Stop, and preserves a reused
  host port owned by a different listener.
- [Doctor v4](../doctor.md) shares runner/SDK selection, distinguishes profiles,
  records prerequisite failures and preserves JSON/Markdown/console/exit meaning.

## Evidence boundaries

All test invocations use `run-with-timeout.py --timeout 1200` or the equivalent
validate wrapper. Raw logs are under `temp/testing/full-review/`; Linux ext4
fixtures use an explicitly named WSL task directory under `/tmp` with their
console record in that Windows evidence folder. Linux filesystem tests do not
qualify a Qt display session or a physical Linux GPU.

Windows App SDK/MAUI native automation, Chromium rendering/recovery and CPU/Node
tests are separate records. Android has no connected ADB device in this run;
Apple hosts are unavailable. Physical IME/accessibility speech, monitor movement,
displayed FPS, signing/notarization and clean-machine deployment remain
notVerified. Historical qualified rows in support documents are unchanged.

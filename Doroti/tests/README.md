# Maintained regression tests

The [work3 Android follow-up](../../history/26-10-05/work3-summary.md#19-2026-10-05-android와-공통-잔여-실행)
adds `AndroidSmoke` with explicit RID/device selection, installed-launcher discovery,
standalone APKs, two sample lifecycle probes, joined Vulkan retirement, native
synthetic IME and metadata reload. Activity recreation uses the samples' opt-in
native `Recreate` callback and confirms OS events with the same PID. Host-only
cases use `--cases profiles,tools`. Missing requested device cases report PARTIAL
and exit 2, with the receipt retained. Actual arm64 and emulator/x64 results,
Release trim/profiled Mono AOT and three isolated NuGet native consumers remain
separate evidence scopes. `android_package_smoke.py` verifies candidate hashes and
mobile first-frame receipts; it does not require desktop two-window behavior.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py AndroidSmoke --device <serial> --rid android-arm64
```

The [work3 Apple follow-up](../../history/26-10-05/work3-summary.md#14-2026-10-05-실행-결과) adds evaluated
Apple/provider ownership closures, provider-owned iOS profile imports, typed iOS
device discovery fixtures, shared-view shutdown and captured-caller native-route
tests, and pointer-driven Cupertino context-menu selection. `macos_smoke.py`
includes `windowing` and optionally `stability` (60 seconds); Graphite and Ganesh
are separate runs. Apple aggregates evaluate profiles and ownership before native
smoke. iOS multi-scene reports SKIPPED when the manifest disables it. Native smoke
does not establish physical IME, accessibility or signed distribution.
The iOS `shutdown` case verifies joined application Stop and actual GPU drain.
`macos_package_smoke.py` accepts v2 native receipts and SDK `.pkg` payloads;
independent widgets AppKit consumption is qualified separately from source smoke.
All direct test entries use the 1,200-second wrapper.

The [2026-10-04 full-review follow-up](../docs/validation/2026-10-04-full-review.md) adds Future terminal/error owner context, mid-frame microtasks, CPU scene snapshot/PNG/repaint/DPR/lifetime and concurrent process-pipe regressions to Build/Developer. Source/Developer runs actual doctor CLI fake-tool fixtures, MSBuild Build/Clean sentinels, Android lease/Stop identity and release receipt checks. Linux installer failure injection requires a Linux filesystem and reports SKIPPED elsewhere. Node executes production raster startup barriers and managed accept/reject/timeout protocol; these doubles do not establish .NET Worker boot. Windows MAUI smoke includes the actual A-B-A bridge/action-generation regression. Use the 1200-second wrapper for direct entries.

Android metadata Hot Reload uses `android_development_bridge.py` for session/request
isolation and port cleanup, `android_development_profile.py` for the actual SDK
startup config and Debug/Release guards, and `android_hot_reload_smoke.py` for
real device deltas, retained State/text/scroll, compile recovery and Stop. Run each
with the 1,200-second wrapper. The device test temporarily edits the Testbed reload
scene and restores it in `finally`; its state is automatically seeded, so it does
not establish physical touch/IME evidence. The installed VSIX regression uses
`dist/test/runHost.js ... --android` with `DOROTI_TEST_ANDROID_DEVICE=<serial>`.
Use fresh output folders under `Doroti/artifacts/android-hot-reload`. See
[development sessions](../docs/development-hot-reload.md) for prerequisites and commands.

Windows MAUI basic services and native views use `windows_maui_smoke.py`, which
selects the MAUI executable explicitly. Modes `graphite`, `embedded`, `ganesh`,
`webview`, and `upload` check separate native owner/widget/upload paths. Build the
Testbed `windows` runner first; `upload` also needs Sample2 `windows`. Run with
the 1200-second wrapper and a fresh `temp/testing/` output directory. See the
[PlatformView support matrix](../docs/platform-views/support-matrix.md)
for supported overlay topology and host constraints.

Native C-only configuration and policy, GPU-full preparation, coalescing, native insertion,
generation changes and out-of-order retirement are part of the CPU `Build` suite.
For actual Vulkan C sequential/overlapped pixels and two unfinished recordings on the same recorder:

```powershell
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug -r win-x64 -- --native-frame-gpu temp/testing/native-frame-c-only/windows/gpu-pixels.json
```

On Linux, use `-r linux-x64` and a Linux output path for that GPU fixture.
The explicit RID stages the pinned native Skia library beside the executable;
a RID-less CPU test build does not provide that Graphite payload layout.

This opt-in GPU fixture deliberately stalls the same queue with a timeline
semaphore while recording two independent scenes. It checks Off, Gaussian,
Adaptive, Fast, Fixed sigma 0/1/2/4/8/20/32 and Kawase 20/32, a third-frame
rejection and byte-identical sequential/overlapped C pixels. It does not exercise actual Qt sampling,
native composition, scanout, input latency or hardware execution overlap.

The same fixture checks dynamic texture allowance with two multi-layer GPU-owned
banks above 128 MiB, safe resize shrink and final consumer retirement. CPU `Build`
also covers driver pressure, live-allocation floors, capacity bounds and overflow.

On macOS, validate the production Metal session separately:

```sh
python3 Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug -- --native-frame-metal-gpu temp/testing/apple-frame-review/metal-gpu.json
```

This checks the same 31 blur/sigma conditions, byte-identical sequential C pixels
against two retained asynchronous submissions, GPU-full preparation, third-frame
admission rejection, reverse retirement and final drain. Its terminal waits and
readbacks are fixture-only. It retains two logical frames without a hardware
queue blocker, and does not claim scanout, hardware overlap or native composition.
Apple snapshots include `nativeFramePipeline` without enabling the detailed
`DOROTI_VARIABLE_BLUR_PROFILE` event history. CPU tests also cover Apple raster
wakes coalescing with normal framework requests.

`native_frame_pipeline_collect.py` accepts an already-built Sample2 payload.
Each invocation collects one repeat of Off, Fast 20/32 and Adaptive 20/32 in C.
`--baseline-exe <preserved-before-C-exe>` pairs desktop before/after candidates
and alternates their order. Three invocations (`--repeat 1`, `2`, `3`) produce
30 paired runs under the 1,200s wrapper. Without a baseline, results are after-only.
`--conditions fast:20 --setting C --seconds 12` checks explicit C; the default
`--setting unset` checks no-selector execution. Supply `--platform windows
--exe <exe>`, `--platform android --device <owned-device> --adb <adb>` or
`--platform qt --exe <exe> --driver <test-only-libevidence.so>`.
`native_frame_configuration.py --exe <exe> --output <fresh-raw-dir>` separately
checks desktop initialization rejects A/B, unknown modes and removed selectors.
Run it under the same wrapper. CPU tests check configuration without environment
mutation. Old frame-policy CLI options are no longer accepted.
For an Apple desktop app executable, add `--apple-receipt`: the native event
loop can remain alive after the builder throws. The probe requires the explicit
startup `ArgumentException` receipt and no normal render evidence, records the
exit state before cleanup, and terminates only its own rejected process.
`apple_smoke.py --app <built-bundle.app>` also accepts isolated build outputs.
The Qt driver is compiled from `native/qt_evidence.cpp` using Qt6Quick/Qt6Widgets
pkg-config flags, as in `linux_qt_smoke.py`. Only the probe's own windows close.
Every run retains its log, evidence, payload hash and failures; output must be
under `temp/testing`. Presentation receipt counters are never converted to FPS.
Only genuine displayed-frame timestamps in `display-events.json` enable a
5s-warmup/30s display summary; retain original capture-tool output and clock
provenance. Missing display events report `notMeasured`.

`native_frame_android_lifecycle.py --device <owned-device> --output <raw-dir>`
checks three continuing resumes with at least 5s between snapshots, rotation,
process/surface recreation and a screenshot. Rotation settings are restored.
This is synthetic automation. Physical finger, IME, selection and 10-minute use
require their own evidence.

Run from the repository root with Python, PowerShell 7, Node 24 and the pinned .NET SDK:

```powershell
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Developer
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Targets
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite WindowsSmoke
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite Packages
```

| Suite | Scope |
| --- | --- |
| Source / audit | Current documentation and plan links, tracked temp policy, timeout/exit tests, top-level failure propagation, portable install rollback/retry and integrity/userdata contracts |
| Build | CPU widget regressions only; not the multi-platform product solution |
| Developer | Source + Build + plugin lifetime/protocol and OS drop routing/lifetime regressions + Web resize/admission/mobile backing, Worker request/capture lifecycle and high-resolution texture ownership tests |
| Targets | Windows App SDK and Web Debug builds, Web HTTP startup/bootstrap asset smoke |
| LinuxSmoke | Build and execute Qt Quick on the available Wayland/xcb session: Desktop/two-window lifetimes, picker/drop/source cancellation, navigation/restore, editor/WebView recreation and 20 resize cycles; VM/software GPU results are explicit |
| WindowsSmoke | Already-built default Debug Windows runner; native state/close cancellation, editor/WebView recreation; two HWNDs and native editor islands, survivor resize and both lifetime policies. Requires an interactive GPU Windows agent |
| Packages | Package Testing/Cupertino/Desktop dependency graph; run a separate PackageReference-only widget consumer with its own restore cache |
| Release | Developer + target builds/startup. `doroti release` then audits and packs the product solution; all required platform toolchains are still necessary. No retired Fcr suite aliases silently pass |

Each aggregate invocation has a 1,200-second limit via `eng/run-with-timeout.py`. Timeout kills the child process tree and returns 124. Raw logs, ad-hoc consumers and their builds live in `temp/testing/<suite>/<run>/`. Success prints its summary then deletes the owned run. Failure preserves the printed directory for investigation; delete it after recording the result, checking the resolved path stays under `temp/testing/`. Product runner build outputs retain their normal `bin/obj` policy. Tests are not in the default product solution. `Doroti.Testing` is a product package; the regression executable is not.

On the Apple toolchain, `python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_build_profiles.py` checks real MSBuild evaluation of iOS device/simulator Debug profiles, caller overrides, custom app assemblies, Release Mono, NativeAOT, and template parity. The optional Apple smoke `rotation` case checks native timing, intermediate raster sizes, final pixels/safe areas and display-link shutdown. For device performance qualification, set `DOROTI_UIKIT_ROTATION_ASSERT_SYNC=1` when launching the Testbed rotation probe; it bounds mean/peak viewport phase error relative to the rotation's size change. Simulator interpreter runs do not claim the same performance budget.

Physical iOS smoke uses an already built, development-signed Testbed `.app` and
an explicit paired device UDID. It installs over the existing app without
uninstalling or resetting data:

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/ios_device_smoke.py \
  --device <physical-UDID> \
  --app samples/DorotiTestbedApp/ios/bin/ios-arm64/Debug/net10.0-ios27.0/ios-arm64/DorotiTestbedApp.iOS.app \
  --output temp/testing/ios-device/<fresh-run>
```

The cases cover UIKit services, synthetic editor recreation, WKWebView/Semantics,
real scene rotation, joined application Stop/Metal retirement and three Settings
background/foreground cycles with the same PID and progressing GPU completions.
Rotation checks intermediate layouts, final pixels/safe areas, display-link
shutdown and frame errors. The short transition uses UIKit's system cadence
without an application FPS cap or FPS acceptance threshold. Viewport phase error,
requested cadence and native display-link intervals are recorded; these do not
measure screen scanout FPS. Optional `--assert-rotation-sync` enables the existing
mean 5%/peak 10% phase bounds for a separate performance qualification run.
Use `--sample material --cases rotation,lifecycle` to exercise the full Components
screen when investigating interactive stutter; the default reload scene is much
simpler. Detailed native CPU/GPU trace is opt-in with `--profile-frames`. The final
manual-use launch clears probe/evidence/profiling environment variables so it
represents ordinary app use.
Independent cases continue after a probe failure, and any failure returns a
nonzero exit code. A normal sample remains foreground for manual checking.
Native/synthetic results do not establish physical IME or VoiceOver acceptance,
device Release/NativeAOT, or distribution signing. Raw JSON, command logs and the
screen capture stay in the output directory; probe files remain in app Documents.

App icon defaults, custom overrides, opt-out and portable asset formats use `python Doroti/eng/run-with-timeout.py python Doroti/tests/app_icons.py`. An optional path to a built `Doroti.Runner.Sdk` nupkg also verifies its icon payload. See [app icon configuration](../docs/branding/README.md) for defaults and overrides.

Plugin package qualification uses `python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_packages.py`
for the NuGet-only common/trimmed consumer and SDK diagnostics. The interactive Windows suite is
`python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_windows_packages.py`:
it publishes a NuGet-only host/plugin consumer, selects its own temporary file through real
OS dialog commands, verifies caller/owner cancellation and opens a loopback URL in the default
browser. It removes successful raw runs. This is synthetic UI automation, not physical input.
See [plugin contract and boundaries](../docs/plugins.md).

OS drop package qualification uses `python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_windows_packages.py --drop`
for the NuGet-only Windows receiver, real read handles/5GB sparse file, Unicode/URI,
revoke/cleanup and 96/192 DPI contexts. It exercises native callbacks with standard
Windows IDataObject, not Explorer gestures. `Doroti.Drop.Windows.Tests` additionally
has an opt-in `--interactive` OLE source/target fixture. See [OS drop contract](../docs/os-drag-drop.md).

Minimal external API example (also add the Skia native asset package for the test OS):

```csharp
using var tester = new Doroti.Testing.WidgetTester();
tester.pumpWidget(app);
tester.pumpAndSettle(timeout: TimeSpan.FromSeconds(5));
tester.tap(tester.text("Second").Single());
tester.pump();
```

`pump` advances virtual time and one queued framework frame, including Dart timers/microtasks. `pumpAndSettle` also waits for pending virtual timers, so a blinking cursor or periodic timer intentionally times out until canceled/unmounted. Each tester stays on its owner thread and is non-nested on that thread. Two separate owner-thread contexts are covered by pointer, IME and semantics isolation regressions. Framework exceptions fail a pump. `DumpTree`, `Frames`, `Clock.PendingTimers` and `WritePng` support failure diagnosis; PNGs are CPU renders, not native or GPU capture. `find`, `byType`, `byKey`, `text`, `center`, `tap`, `drag`, `sendKey`, `enterText` and semantics access form the initial contract. Finder results include mounted offstage elements; callers must disambiguate them.

The permanent fixtures protect regressions in pointer tab routing, bounded settle, framework teardown and native close/resource ownership. To prove the Cupertino regression detects a broken hit-test path:

```powershell
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Tests -c Debug -- --break-tab
```

That command must fail; without `--break-tab`, it must pass. Rendering fixtures cover fixed DPR pixels, VariableBlur raster kernels/capture coordinates, a 1,000-row list, reassemble offset preservation and zero engine-layer delta after disposal. Physical IME/accessibility and other GPU/platform combinations need separate checks; consult [support status](../docs/support-status.md).

On macOS with a Metal GPU, run the production renderer pixel checks explicitly:

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-gpu
```

This command compares crop/full and direct/intermediate composition at DPR 1/2/3, Gaussian detail at the clear edge, bounded/full Kawase reconstruction, and native/owned sibling scopes with a VariableBlur inside opacity. Fixed Gaussian also exercises the actual `ApplyVariableBlur` reduced output and final linear sampling: 72 conditions cover both gradient directions, fractional interior/edge ROIs, translucent input, Clamp/Decal, and nondivisible sizes including full-domain fallback. Each equivalent comparison has a maximum channel-error limit of 3/255. It uses synchronous readback only in validation and does not measure FPS or qualify iPhone, other GPU backends, or the full transform/tile/child matrix. The default suite stays on raster surfaces; `--variable-blur-kernel`, `--variable-blur-capture`, and `--variable-blur-kawase` isolate the embedded production SkSL and its manifest/ABI.

For separate Full/Fixed still-image quality review, run from the repository root:

```sh
dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --variable-blur-quality temp/testing/variable-blur/quality-review
```

This macOS Metal command saves 14 PNGs at DPR 3 with sigma 0/1/2/4/8/20/32, small text, 1px lines, a high-frequency pattern, and the repository's reference photograph. Sigma zero bypasses the filter as in SampleApp2. These captures document downsampling softness and the clear-edge/disabled-filter transition; Full and Fixed are not required to be pixel-equivalent. Still-image review does not establish motion quality, physical input behavior, or performance. Keep captures separate from device presentation measurements.

For the installed VSIX and native metadata-update tests, use the commands in [development sessions](../docs/development-hot-reload.md). [Rendering baselines](../docs/rendering-baselines.md) specifies measurement boundaries and prospective budgets. Testbed `DOROTI_SAMPLE=reload` provides a counter, input and long list for manual reload testing.

Linux Qt metadata Hot Reload uses an isolated source-template fixture and keeps
user sample sources untouched:

```sh
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_hot_reload_smoke.py --qpa wayland --output temp/testing/linux-hot-reload/manual
# After npm test / npm run package in Doroti/tools/vscode-doroti, use the generated fixture:
python3 Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js 'temp/testing/linux-hot-reload/manual/한글 경로/ReloadApp' temp/testing/linux-hot-reload/manual-vsix --linux
```

The CLI test verifies real metadata updates, same PID/State/count/Hangul text/
scroll, compile-error recovery, rude-edit deferral and process-tree Stop. The
installed VSIX test additionally checks the Hot Reload command, concurrent
clicks, explicit Restart and Stop in an isolated editor profile. Linux build
profiles check the Debug guard, polling default/override and custom .NET path.
The source-template test uses repository ProjectReferences; it does not claim
NuGet-only development or Hot Reload of native C++/QML/assets.

Web Hot Reload qualification uses `runHost.js ... --web`: a clean-profile installed VSIX, actual SDK metadata updates, request/frame acknowledgments, compile-error correction and duplicate-request serialization. Browser file gates allow real counter/input/scroll and pixel inspection. The standalone `web_rendering.mts` tests also cover the browser bridge endpoint parser; extension unit tests cover origin/session checks and prepare acknowledgment. WebGL/WebGPU evidence and unsupported combinations are tracked in the development contract.

For manual Windows Korean IME/focus checks, set `$env:DOROTI_SAMPLE='input'` and launch the default Testbed windows alias. Compose and cancel Korean text in the first multiline field, move through the native editor or WebView with Tab/Shift+Tab, select/copy/paste, recreate the native view, and confirm the final framework field receives focus. The scene shows selection/composing ranges. Choose the native editor or WebView using the switch button: mixing both composition topologies in one frame remains unsupported.

Navigation/restore, concurrent window contexts and the CPU Dialog intermediate frame are in Developer. Selected Release candidates and portable installation are documented in [release candidates](../docs/release-candidates.md).

Browser services (picker/managed grants, Copy drop, real history and text restoration)
are exercised through the threaded runtime with installed Chrome and Python Playwright:

```powershell
# Serve the built Testbed at the selected --url first.
python Doroti/eng/run-with-timeout.py python Doroti/tests/web_services_smoke.py --url http://127.0.0.1:5199/ --renderer webgl --output temp/testing/browser-services/webgl
# Repeat with --renderer webgpu and a separate output directory.
```

The test uses actual browser chooser/keyboard/mouse events. Its DataTransfer drop is
synthetic DOM input, not Explorer-to-browser transfer. It asserts a native textbox
semantics node cannot absorb sibling buttons, and that worker-side navigation and
plugin calls reach DOM services. It preserves failure images; output is disposable.

Linux Qt uses Python 3 and system Qt 6 development/QML/WebEngine dependencies:

```sh
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_build_profiles.py --output temp/testing/linux-qt/manual-profiles
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite LinuxSmoke
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_smoke.py --qpa xcb --output temp/testing/linux-qt/manual-xcb
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_packages.py temp/testing/linux-qt/manual-packages
```

The profile fixture evaluates SDK/sample/template defaults and invalid option
combinations, separates native caches, rejects missing and unbuilt-profile
output, replaces equal-size/equal-mtime libraries and removes disabled optional
outputs. `LinuxSmoke` includes it and checks the two-frame bound and complete Qt
consumer retirement at shutdown. Package qualification uses the template's
default Quick/C configuration without overrides and exercises matching and
wrong-profile `publish --no-build` with an isolated NuGet cache. These checks
do not qualify physical display timing, GPU execution overlap or physical IME.

Choose a fresh `--output` path. Direct scripts retain raw evidence for review;
the aggregate LinuxSmoke removes successful runs. `--cases` selects
`multi,desktop,services,navigation,input,resize`. The native evidence driver is
compiled only for the test and injected with LD_PRELOAD, never shipped in the
host/template. Dialog selection and QDropEvent input are synthetic native tests,
not physical IME/Orca or external file-manager drags. An optional
`DOROTI_QT_URL_PROBE=http://127.0.0.1:PORT/` also opens the real default browser.
Widgets OpenGL uses the separate ClipRect-only `qt-widgets` sample; see the
[Linux Qt guide](../docs/platform-views/linux-qt.md).

macOS AppKit uses the installed Xcode 27/.NET 10 macOS27 profile:

```sh
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite MacOSSmoke
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_smoke.py --renderer ganesh --output temp/testing/macos-appkit/ganesh
```

Use a fresh output directory. Native desktop automation, two-window screenshots,
picker cancellation, pasteboard grants, WKWebView recreation and LaunchServices
route restoration are distinct from physical IME/VoiceOver/Finder input. The
opt-in automation lives only in the testbed runner. `--cases` selects
`multi,desktop,services,input,navigation,rendering,lifecycle`; `--tfm net10.0-macos` selects
the corresponding Xcode 26 toolchain. Direct scripts retain their raw output;
MacOSSmoke cleans successful runs.

The `lifecycle` fixture uses actual Metal submissions and terminal callbacks to
check same-owner replacement gating, independent-window connection while an old
owner retires, retirement roots, background/retired snapshots, hide/resume three
times and native detach/reattach. Native keyboard events and window backing
factors 1/1.5/2 are synthetic. They verify key-up synthesis and exact pixel extents,
and do not qualify physical keyboard/IME input or a real monitor transition.
The fixture never blocks the GPU to claim hardware overlap.

The local macOS payload fixture is `tests/macos_package_smoke.py --candidate PATH
--previous PATH --output temp/testing/macos-appkit/install`. Run it through the
1,200-second timeout wrapper. It verifies recorded payload hashes and signatures,
runs both extracted app versions with an empty NuGet cache, replaces the app,
then removes it while preserving its own Unicode userdata fixture. It does not
run macOS Installer or qualify Gatekeeper/notarization/clean-machine deployment.

iOS Simulator, device signing and Catalyst scenes require separate checks; package-only publish does not imply clean signed distribution.

Apple suites: `IOSSmoke` uses native scene activity callback injection for unattended
navigation; `CatalystSmoke` uses LaunchServices URL delivery. Both use the maintained
`apple_smoke.py`; raw results separate services, native recreation, features, navigation and
restoration. Catalyst additionally creates/closes real scenes. `--activation os`
on iOS may require confirming the OS Open dialog. This is not a physical-input test.

```sh
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite IOSSmoke
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite CatalystSmoke
```

iOS development validation (macOS, booted simulator, matching iOS workload):

```sh
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/ios_development_bridge.py
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/apple_build_profiles.py
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/ios_hot_reload_smoke.py temp/testing/ios-reload-run --device <simulator-UDID>
python3 Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js samples/DorotiTestbedApp temp/testing/ios-editor-run --ios
```

The smoke test temporarily edits the Testbed reload scene and restores it in `finally`; use a fresh evidence directory and do not edit that scene concurrently. It verifies actual deltas, PID/State/count/text/scroll preservation, compiler-error recovery, rude edits and Stop. State is seeded automatically. The editor test uses a separately installed VSIX/profile; run `npm run package` in the extension folder first. See [development sessions](../docs/development-hot-reload.md).

Physical-device Hot Reload uses the same smoke test with `--rid ios-arm64 --framework net11.0-ios --sdk-version 11.0.100-rc.1.26425.128 --dotnet <prepared-dotnet-host>`. It copies the automatically seeded state probe out of Documents and requires matching completed-frame responses for real SDK deltas. Pair the device, enable Developer Mode, and supply signing/toolchain environment variables described in the [development contract](../docs/development-hot-reload.md).

For installed-VSIX device validation, set `DOROTI_TEST_IOS_RID=ios-arm64`, `DOROTI_TEST_IOS_DEVICE=<UDID>`, `DOROTI_TEST_IOS_TFM=net11.0-ios`, `DOROTI_TEST_IOS_SDK=11.0.100-rc.1.26425.128`, and `DOROTI_TEST_DOTNET=<prepared-dotnet-host>`, then run `runHost.js <Testbed> <fresh-evidence> --ios` through the timeout wrapper. Do not run the CLI and editor smoke simultaneously: both temporarily edit the same scene. `apple_build_profiles.py --device-sdk <installed-version>` also checks CoreCLR, registrar, runtime/crossgen2 alignment and unchanged Apple runtime pack selection.

AppKit and Mac Catalyst development validation (macOS, matching Apple workloads):

```sh
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/mac_hot_reload_smoke.py temp/testing/appkit-reload-run --platform macos --framework net10.0-macos27.0
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/mac_hot_reload_smoke.py temp/testing/catalyst-reload-run --platform maccatalyst --framework net10.0-maccatalyst27.0
python3 Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js samples/DorotiTestbedApp temp/testing/appkit-editor-run --mac
DOROTI_TEST_MAC_TARGET=maccatalyst python3 Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js samples/DorotiTestbedApp temp/testing/catalyst-editor-run --mac
```

Run these sequentially: they temporarily edit the same Testbed scene, restoring it on exit. The CLI smoke checks real metadata updates, seeded state preservation, compile recovery, rude edits and Stop/PID exit. The editor smoke installs the packaged VSIX in an isolated profile and invokes its Run/Hot Reload/Stop commands. `DOROTI_TEST_MAC_TFM` overrides the editor test's Xcode 27 TFM. `apple_build_profiles.py` also checks both Mac development profiles, rejects incompatible settings and verifies ordinary Debug/Release interpreter defaults remain unchanged.

Use windows_maui_smoke.py for MAUI owner/services, windows_native_texture_smoke.py for actual MAUI GPU import,
windows_smoke.py for explicit App SDK/MAUI multiwindow and the opt-in mixed scene.
web_runtime_recovery.py requires a completed Testbed Web build; --main-only uses the default threaded build,
--worker-only requires -p:WasmEnableThreads=false. For an isolated --artifacts-path build,
set DOROTI_WEB_TEST_ARTIFACTS to that same absolute directory; retain the default threaded
build for --main-only. Recovery checks also cover loopback origin admission, independent
canvas-axis growth and 60-second stability after restart. Captures are headless browser
evidence, not physical monitor or input qualification.
web_offline_fonts.py requires Sample2 Assets + DorotiWebFontValidation=true; it checks real Korean glyphs,
zero external requests and decoder-denial diagnostics. Do not build shared TS assets while a browser probe runs.
hot_reload_smoke.py accepts backend Maui and --verify-restart to check actual Run/Reload/Stop/Run/Stop.
All commands use the 1200-second wrapper. The default Apple smoke `features` case covers actual Apple WebView commands, accessibility hierarchy/actions/password guards, and input configuration/Return behavior. iOS/macOS testbed probe copies must match; `apple_build_profiles.py` checks this parity.

`web_package_runtime.py <candidate> <fresh-output>` serves the published NuGet-only Offline Web payload with explicit COOP/COEP and JavaScript `.mjs` MIME, then checks first frame/focus and zero external requests. Runtime ownership uses the SDK-generated `doroti.runtime-profile.json`; standalone worker requires a single-thread build. `web_runtime_recovery.py --worker-only --expect-threaded-rejection` checks early rejection of the ordinary threaded profile. Changing threading profiles in a shared SDK output requires clean/rebuild or isolated artifacts; the normal Testbed profile is threaded.

Web semantics snapshot/delta validation uses the actual managed encoder and DOM host:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --test Doroti/tests/web_semantics.mts
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Debug -- --web-semantics temp/testing/web-semantics/run/packets.json
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Debug
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_semantics_browser.py --packets temp/testing/web-semantics/run/packets.json --output temp/testing/web-semantics/run/browser
```

Use a Python environment with Playwright and installed Chrome, and a fresh browser
output directory. The browser test compares managed deltas with full snapshots,
checks native element/focus/selection retention, parent motion, order/reparent,
deletion, relationship changes, clear and real managed snapshot recovery. It also
can run the existing file/navigation/text/drop service regression in both renderers
when `--services` is added; keep that broader startup/service result separate.
WebGL exercises the focus-restoring insertion fallback; WebGPU uses native DOM
moves when available. This is desktop browser automation, not physical IME,
screen-reader, mobile stability or displayed-FPS qualification.

# Maintained regression tests

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
| Developer | Source + Build + plugin lifetime/protocol and OS drop routing/lifetime regressions + Web resize/admission/mobile backing policy tests |
| Targets | Windows App SDK and Web Debug builds, Web HTTP startup/bootstrap asset smoke |
| LinuxSmoke | Build and execute Qt Quick on the available Wayland/xcb session: Desktop/two-window lifetimes, picker/drop/source cancellation, navigation/restore, editor/WebView recreation and 20 resize cycles; VM/software GPU results are explicit |
| WindowsSmoke | Already-built default Debug Windows runner; native state/close cancellation, editor/WebView recreation; two HWNDs and native editor islands, survivor resize and both lifetime policies. Requires an interactive GPU Windows agent |
| Packages | Package Testing/Cupertino/Desktop dependency graph; run a separate PackageReference-only widget consumer with its own restore cache |
| Release | Developer + target builds/startup. `doroti release` then audits and packs the product solution; all required platform toolchains are still necessary. No retired Fcr suite aliases silently pass |

Each aggregate invocation has a 1,200-second limit via `eng/run-with-timeout.py`. Timeout kills the child process tree and returns 124. Raw logs, ad-hoc consumers and their builds live in `temp/testing/<suite>/<run>/`. Success prints its summary then deletes the owned run. Failure preserves the printed directory for investigation; delete it after recording the result, checking the resolved path stays under `temp/testing/`. Product runner build outputs retain their normal `bin/obj` policy. Tests are not in the default product solution. `Doroti.Testing` is a product package; the regression executable is not.

App icon defaults, custom overrides, opt-out and portable asset formats use `python Doroti/eng/run-with-timeout.py python Doroti/tests/app_icons.py`. An optional path to a built `Doroti.Runner.Sdk` nupkg also verifies its icon payload. Platform build/runtime verification is recorded in the [icon validation notes](../docs/branding/validation.md).

M4 package qualification uses `python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_packages.py`
for the NuGet-only common/trimmed consumer and SDK diagnostics. The interactive Windows suite is
`python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_windows_packages.py`:
it publishes a NuGet-only host/plugin consumer, selects its own temporary file through real
OS dialog commands, verifies caller/owner cancellation and opens a loopback URL in the default
browser. It removes successful raw runs. This is synthetic UI automation, not physical input.
See [plugin contract and boundaries](../docs/plugins.md).

M5-A uses `python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_windows_packages.py --drop`
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

That command must fail; without `--break-tab`, it must pass. Rendering fixtures now cover fixed DPR pixels, VariableBlur capture policy (not its GPU kernel), a 1,000-row list, reassemble offset preservation and zero engine-layer delta after disposal. GPU golden/blur and physical IME/accessibility remain tracked in [M1](../../works/common/01-testing.md) and [M2-B](../../works/common/03-input-accessibility-platformview.md).

For the installed VSIX and native metadata-update tests, use the commands in [development sessions](../docs/development-hot-reload.md). [Rendering baselines](../docs/rendering-baselines.md) specifies measurement boundaries and prospective budgets. Testbed `DOROTI_SAMPLE=reload` provides a counter, input and long list for manual reload testing.

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
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite LinuxSmoke
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_smoke.py --qpa xcb --output temp/testing/linux-qt/manual-xcb
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/linux_qt_packages.py temp/testing/linux-qt/manual-packages
```

Choose a fresh `--output` path. Direct scripts retain raw evidence for review;
the aggregate LinuxSmoke removes successful runs. `--cases` selects
`multi,desktop,services,navigation,input,resize`. The native evidence driver is
compiled only for the test and injected with LD_PRELOAD, never shipped in the
host/template. Dialog selection and QDropEvent input are synthetic native tests,
not physical IME/Orca or external file-manager drags. An optional
`DOROTI_QT_URL_PROBE=http://127.0.0.1:PORT/` also opens the real default browser.
Widgets OpenGL uses the separate ClipRect-only `qt-widgets` sample; see
[Linux execution and reproduction](../../works/results/2026-09-29-linux-qt.md).


macOS AppKit uses the installed Xcode 27/.NET 10 macOS27 profile:

```sh
pwsh -NoProfile -File Doroti/eng/doroti.ps1 validate -ValidationSuite MacOSSmoke
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/macos_smoke.py --renderer ganesh --output temp/testing/macos-appkit/ganesh
```

Use a fresh output directory. Native desktop automation, two-window screenshots,
picker cancellation, pasteboard grants, WKWebView recreation and LaunchServices
route restoration are distinct from physical IME/VoiceOver/Finder input. The
opt-in automation lives only in the testbed runner. `--cases` selects
`multi,desktop,services,input,navigation,rendering`; `--tfm net10.0-macos` selects
the corresponding Xcode 26 toolchain. Direct scripts retain their raw output;
MacOSSmoke cleans successful runs. See [AppKit results](../../works/results/2026-09-29-macos-appkit.md).

The local macOS payload fixture is `tests/macos_package_smoke.py --candidate PATH
--previous PATH --output temp/testing/macos-appkit/install`. Run it through the
1,200-second timeout wrapper. It verifies recorded payload hashes and signatures,
runs both extracted app versions with an empty NuGet cache, replaces the app,
then removes it while preserving its own Unicode userdata fixture. It does not
run macOS Installer or qualify Gatekeeper/notarization/clean-machine deployment.

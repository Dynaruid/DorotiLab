# Desktop window implementation evidence — 2026-09-25

Overall **PARTIAL**. This implements the main-window desktop contract and a
Windows MAUI, AppKit macOS and restricted Mac Catalyst adapters, not the entire W0–W5 plan. The authoritative remaining work
is in the root [work.md](../../../work.md) and the
[API/support document](../../docs/desktop-windows.md).

All build/test commands used `run-with-timeout.py` (1,200 seconds). Shared build
outputs and native GUI probes were used serially. No hundreds-of-iterations
stress test or physical-display qualification is claimed.

| Gate | Result | Evidence / limits |
| --- | --- | --- |
| Core/Widgets Release build | PASS | Warnings-as-errors projects, zero warnings/errors |
| Fake-host contracts | PASS, 25/25 | Identity, two independent controllers/content factories, stale handles, hooks/readiness, cancellation, rollback, close coalescing/cancel, subscription removal, serial appearance updates, request supersession, close during creation, last close vs pending creation |
| Windows MAUI Release / Release x64 | PASS | Source Testbed with separately compiled desktop companion |
| Native control probe | PASS | Windows 200% DPI: hidden-ready state; client 450×800; min client 350×500 via WM_GETMINMAXINFO; resize 500×650; title; show/hide/focus; maximize/restore; minimize/fullscreen/windowed; appearance replacement/reset; first native close canceled, second clean exit |
| Caption Backdrop + Acrylic body | PASS, automated pixels | Body response 84/14/102; caption response 193/30/233; DWMWA_SYSTEMBACKDROP_TYPE=3; clean exit |
| Caption Solid + Acrylic body | PASS, automated pixels | Body response 84/14/102; caption response 0/0/0; DWM type=1; clean exit |
| Caption System + Acrylic body | PASS, automated pixels | Body response 84/14/102; caption response 0/0/0; DWM type=1; clean exit |
| Existing resize operations | PASS, 24 resizes | 41 native prepared frames, 0 native resize timeouts, Graphite device creations=1, final physical 934×629, clean exit. This is not visible-continuity proof |
| Displayed-frame flicker gate | NOT PASSED / PARTIAL | Current run failed before capture: after 961px outer-width setup, renderer evidence stayed 934px while native client expected 935px. No new geometry-continuity PASS; earlier PARTIAL stays open |
| External core/widget package consumer | PASS | 15 local nupkgs; fresh temp consumer/cache; no source ProjectReference in assets; same 25 contracts |
| Package negative builds | PASS | At the Windows checkpoint: Web, Android, iOS, MacCatalyst rejected Desktop; Catalyst is enabled by the later checkpoint below. |
| External Windows package consumer | PASS | Local App/Runner SDK + Windows MAUI packages; common app + desktop companion; generated bootstrap; native control probe and clean exit |
| Full Web/mobile runner builds | notRun | Negative package builds do not substitute for full platform application builds |
| First-display frame sequence / physical appearance | notVerified | Readiness and native/pixel automation are not a complete first-visible-frame capture or physical-display acceptance |
| 100/150% / mixed DPI, high contrast, transparency off | notVerified | No OS-wide settings were changed to manufacture results |
| Hidden/custom chrome, chrome widgets, App theme bridge | notImplemented | Requests rejected; native input/Snap/IME/accessibility gate remains open |
| WindowsAppSDK/Qt adapters | notImplemented | New desktop startup rejected; existing host paths remain intact |
| Native multiple windows / owners | notImplemented, W6 | Fake two-window PASS is not native multi-window support |

## Reproduction

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/desktop-window/Contract.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:Platform=x64
python Doroti/validation/run-with-timeout.py python Doroti/validation/desktop-window/verify-windows.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
$env:DOROTI_DESKTOP_CAPTION = 'Solid' # also test 'System'; remove for Backdrop
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
Remove-Item Env:DOROTI_DESKTOP_CAPTION
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-resize.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/desktop-window/verify-package.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/desktop-window/verify-windows-package.py
```

`verify-windows.py` starts only its own process and verifies the sample's observed
state against native client/frame limits. `DOROTI_DESKTOP_SAMPLE=acrylic` selects
the transparent-base native Acrylic options. The existing common Testbed still
owns its Scaffold opacity toggle; this does not test runtime material mode off/on.
Mode/base-transparency changes return RequiresRecreation, not success.

The shared window finder now excludes DWM-cloaked HWNDs. WS_VISIBLE alone is not
the displayed application window during the new hidden first-frame preparation.

## Fixes found by actual execution

- Default native maximum track size clamped the initial 800 DIP height to 779.5
  at 200% DPI. The adapter preserves explicitly requested client size while
  translating native frame/min/max bounds per-window DPI. A window can exceed
  the monitor work area; it is not silently reported as 450×800 after clamping.
- MAUI's title mapping runs after HandlerChanged. Reassert title and caption
  appearance after the content root loads, still under DWM cloak.
- MAUI 10.0.90's caption template part became visible again during Resizable
  changes and attempted PreferredHeightOption on a native caption. The adapter
  preserves the public template part's collapsed state while native chrome owns
  the caption, and unregisters that callback on close.
- Native close now releases window-owned render/material/subclass resources
  before HWND destruction, then removes the controller and evaluates app exit.
- Queued creation prevents premature last-window exit; a native window that
  closes during initialization is never inserted as a live registry entry.

## Evidence retention

The small [results JSON](results-2026-09-25.json) preserves measured summaries and
binary hashes. Raw PNGs/logs/local nupkgs live under `Doroti/artifacts/validation`
and are disposable. Fresh package consumers live in uniquely named OS temporary
directories. These are local execution artifacts, not permanent evidence links.

Captured source build: working tree based on `351d4dd2`. The snapshot includes
uncommitted implementation changes. Never identify that base commit alone as the
tested implementation. Host hashes and execution directories are recorded in
the JSON; later source/binary changes require a fresh run rather than reusing
these PASS results.


## AppKit macOS — 2026-09-25

Environment: Apple M1 / arm64, macOS 26.6.2 (25G83), Xcode 27.0 (27A266a),
.NET SDK 10.0.400, explicit `net10.0-macos27.0` profile, MAUI 10.0.90,
Retina scale 2. Release build completed with zero warnings/errors.
This is a separate execution from the Windows results above.

| Gate | Result | Scope |
| --- | --- | --- |
| Contracts | PASS, 28/28 | 25 shared contracts plus AppKit capability/material precedence/fallback tests |
| Package contracts/boundaries | PASS | External package-only consumer: 25 shared contracts; At the AppKit checkpoint, Web/Android/iOS/MacCatalyst rejected Desktop references; the Catalyst update below supersedes that boundary |
| External AppKit package consumer | PASS | Fresh app/cache, local App/Runner SDK and macOS packages, native binding/bridge, generated bootstrap, control probe and clean exit |
| Graphite, System caption | PASS | Manual hidden readiness, 450×800 client, 350×500 client minimum, 500×650 resize, zoom/restore, minimize completion, native full-screen completion, hide/show, appearance update/reset, close cancel→allow, registry empty and normal process exit |
| Graphite, Acrylic/Backdrop, app quit | PASS | Same control sequence; first application termination canceled, second allowed |
| Ganesh, Acrylic/Solid, Explicit lifetime | PASS | Same controls, close cancel→allow, app remains running with zero registered windows, then explicit app quit |
| Default legacy/WhenReady sample | PASS | Idempotent Show awaited before Focus; automatic startup, native controls and clean exit |
| Liquid Glass startup | PASS, native operations | Legacy companion with DOROTI_MACOS_BACKDROP=liquidGlass on macOS 26; runtime replacement/reset and clean close |
| Runtime materials | PASS, native operations | Solid→Transparent→Acrylic→LiquidGlass→initial appearance; no base-color recreation requested |
| Screenshots | Captured/reviewed | Native traffic lights, title and client layout; no claim of a controlled background blur-strength/pixel regression gate |
| Physical/OS environment matrix | notVerified | Mixed monitors, physical IME/VoiceOver, old-OS Glass fallback, OS-wide transparency/contrast toggles, full first-visible-frame sequence and live-resize continuity |
| Unsupported | Explicit rejection | Position/SetBounds, per-window Dock hiding, hidden/custom/frameless, app theme bridge, programmatic resize, additional native windows/owners |

`WindowState.Bounds` is null on this adapter. Size is the unobscured content
rectangle in AppKit points; the native minimum height includes the measured
caption height (500 + 32 = 532 on the measured configuration). A window larger
than the work area preserves the explicitly requested client size.

The native probe invokes `PerformClose` and `NSApplication.Terminate` directly.
This qualifies their delegate paths; physical Cmd+W/Cmd+Q input delivery is a
separate keyboard test. Assertions require cancellation on the first decision,
acceptance on the second, an empty manager registry, and exit code 0. The
Explicit test verifies application survival before its final app-quit request.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Release -r osx-arm64 -p:DorotiMacOSTargetFramework=net10.0-macos27.0
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-macos.py --app 'DorotiTestbedApp/macos/bin/Release/net10.0-macos27.0/osx-arm64/Doroti Testbed (AppKit).app' --output /tmp/doroti-appkit-normal --capture
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-macos.py --app 'DorotiTestbedApp/macos/bin/Release/net10.0-macos27.0/osx-arm64/Doroti Testbed (AppKit).app' --output /tmp/doroti-appkit-quit --material acrylic --quit
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-macos.py --app 'DorotiTestbedApp/macos/bin/Release/net10.0-macos27.0/osx-arm64/Doroti Testbed (AppKit).app' --output /tmp/doroti-appkit-ganesh --renderer ganesh --material acrylic --caption Solid --explicit
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-package.py
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-macos-package.py
```

Use fresh output directories. `--capture` requires window capture permission;
control/close checks do not depend on screenshots. The macOS package fixture
includes its own copied native binding/Xcode project and bridge because the SDK
requires the complete runner contract. Doroti libraries are consumed as packages.
The runner guard is not bypassed to produce a successful build.

Execution-driven fixes:

- The pinned upstream MAUI preview orders the window inside allocation. The
  desktop-only launch uses its own handler so Manual readiness stays hidden.
- A hidden Metal presentation blocked GPU completion. Hidden preparation now
  completes rendering without presenting, retains its drawable for Show, and
  drains pending GPU work before native close.
- AppKit constrained the requested 800-point client height to 755 at first show
  and later resize. The owned NSWindow preserves explicit frame dimensions.
- Minimize is asynchronous; its returned state now follows the native delegate
  completion, as full screen already does.
- Native close can resign key after MAUI Destroying. The adapter avoids duplicate
  Deactivated delivery during destruction and guards unexpected native closure.
- The legacy WhenReady sample raced its automatic Show against Focus. Its hook
  now awaits the idempotent Show before requesting focus.

The tracked [macOS results JSON](results-macos-2026-09-25.json) records binary/source identities and summaries.
Raw window captures, native state logs and local packages under
`Doroti/artifacts/validation/desktop-macos` are disposable. Their existence is
not a permanent evidence guarantee. Windows native execution was not repeated
on this Mac; the shared contracts and package boundary checks were rerun.


## Mac Catalyst — 2026-09-25

Environment: Apple M1 / arm64, macOS 26.6.2, Xcode 27.0, .NET SDK 10.0.400,
MAUI 10.0.90, `net10.0-maccatalyst27.0`, native scale 2.

| Gate | Result | Scope |
| --- | --- | --- |
| Release source build | PASS | Zero warnings/errors, explicit Xcode 27 profile |
| Shared/platform contracts | PASS, 30/30 | 25 shared contracts, 3 AppKit policy contracts, 2 Catalyst/platform-startup contracts |
| Native API control/close | PASS | 550×475 → 500×450; locked-resizable API resize 520×460 → 500×450; native title/min/max; theme apply/reset; rejected hide/Dock/fullscreen/Acrylic; first API close canceled, second allowed; zero registered windows |
| Native scene disconnect | PASS | Direct RequestSceneSessionDestruction; closing subscription remains installed but is not invoked; GPU retirement and zero registered windows |
| Explicit lifetime | PASS | Process remains running after scene closure; fixture subsequently terminates only its own process, without claiming a graceful application quit |
| External package contracts/boundaries | PASS | Fresh package-only consumer, 25 shared contracts; Web/Android/iOS rejected, Catalyst accepted |
| External Catalyst package execution | PASS | Fresh consumer/cache, App/Runner SDK, target/host/Desktop packages, own native binding; same API/native control assertions |
| AppKit regression after shared changes | PASS | Clean Release rebuild, zero warnings/errors; Graphite normal/System-caption native control probe, close cancel→allow, empty registry and exit code 0 |

Execution-driven fixes include recovering MAUI window creation for an old unnamed
restored scene, enabling UIKit multi-scene adoption for scene destruction, and
waiting for both native bounds and MAUI layout after geometry requests. A Mac
idiom scene frame already describes UIKit content dimensions: subtracting a
stale layout size invents a chrome offset and produces the wrong size. Fixed
resizability temporarily releases scene limits for an explicit API resize, then
locks the newly observed dimensions.

The source probe observed a restored initial size of 500×450; the fresh package
consumer observed the requested 600×500. Neither is labeled hidden readiness.
The tracked [Catalyst results JSON](results-maccatalyst-2026-09-25.json) preserves
binary hashes, observed states and source hashes. Temporary logs/consumers and
`Doroti/artifacts` packages are disposable. Windows native execution and the
old/default Catalyst SDK profile were not rerun on this Xcode 27 machine.

This adapter uses public UIKit scene APIs, Mac idiom (`UIDeviceFamily=6`) and
Graphite. UIKit scene destruction requires `UIApplicationSupportsMultipleScenes=true`;
the delegate rejects additional windows before MAUI allocates them. `PlatformDefault` startup lets UIKit show the scene; readiness waits
for initialized geometry and a completed frame without promising hidden launch.
`Explicit` manager lifetime leaves application termination to UIKit. Restored
scene geometry can replace the initial size; probes record the actual initial
size and verify explicit post-ready resizing against the native render view.

The source and package probes check title, client size, scene limits, resizable
changes, show/key requests, theme update/reset, unsupported operations, and
registry cleanup. API close and direct native scene destruction are separate
runs. `CanCancelNativeClose=false` is intentional: only API CloseAsync invokes
managed closing decisions. Direct scene destruction tests disconnection, not
physical traffic-light clicks or Cmd+W/Cmd+Q delivery.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj -c Release -r maccatalyst-arm64 -p:DorotiMacCatalystTargetFramework=net10.0-maccatalyst27.0
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-maccatalyst.py --app 'DorotiTestbedApp/macos/bin/Release/net10.0-maccatalyst27.0/maccatalyst-arm64/Doroti Testbed.app' --output /tmp/doroti-catalyst-api
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-maccatalyst.py --app 'DorotiTestbedApp/macos/bin/Release/net10.0-maccatalyst27.0/maccatalyst-arm64/Doroti Testbed.app' --output /tmp/doroti-catalyst-native --native-close
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-package.py
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/desktop-window/verify-maccatalyst-package.py
```

Use fresh probe directories. Build and run sequentially. Catalyst AOT output
can retain obsolete dependencies after shared assembly changes; use `-t:Rebuild`
when validating a changed dependency closure. The package probe uses a fresh
consumer/cache and copies only the application/native binding fixture; Doroti
libraries and SDKs come from local packages. The Xcode 27 profile requires
Catalyst 17 as its SDK minimum; the adapter's runtime minimum is Catalyst 16.

AppKit Acrylic/Liquid Glass, hidden first-frame display, native-close veto,
global pixel placement, per-window Dock/topmost policy and additional windows
are unsupported on this UIKit adapter. Full physical display/input/OS-version
qualification and Intel execution are not claimed.

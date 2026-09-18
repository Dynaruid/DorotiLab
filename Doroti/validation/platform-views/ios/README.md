# UIKit PlatformView validation

## iOS 27 profile

Run these commands from the **workspace root** so its .NET 10 SDK is selected.
The `ios` directory still selects .NET 11 for the existing NativeAOT profile.
Install an iOS/MAUI workload containing the Xcode 27 pack (this run used workload
set 10.0.401 and iOS pack 27.0.10539-xcode27.0). The SDK still labels these bindings
preview. The iOS 27 profile suppresses that notice only and does not disable Xcode
version validation or other project warnings.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet build \
  DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r iossimulator-arm64 -m:1 \
  --artifacts-path Doroti/artifacts/platform-views/2026-09-17/ios/ios27/build \
  -p:DorotiIosTargetFramework=net10.0-ios27.0

python3 Doroti/validation/run-with-timeout.py <pillow-python> \
  Doroti/validation/platform-views/ios/capture-blur-appearance.py \
  --simulator <iOS-27-UDID> --app <built-app> --output <capture-folder> --check-resume
```

The profile selects Mono and the matching Host.Maui TFM. Debug defaults to
`MtouchInterpreter=all,-Doroti.Host.Maui`: Mono AOT for the host, interpreter for
the other assemblies. Full host interpretation reproduced invalid `PendingFrame`
references during calibration on Mono 10.0.12 / iOS 27 Simulator. Do not add
`-p:MtouchInterpreter=all` to the command above: an explicit value overrides the
workaround. To reproduce the old failure deliberately, use that value and launch
with `MONO_GC_DEBUG=check-remset-consistency,verify-before-collections` plus the
calibration environment variables below. With `simctl`, prefix those variable
names with `SIMCTL_CHILD_`. Device Release stays Mono unless explicitly overridden. Existing .NET 11 NativeAOT is a separate
profile, not claimed as iOS 27 SDK qualification. Device builds use `-r ios-arm64`,
a separate artifacts root and the installed signing profile. The Scene manifest
is shared by the Testbed and new app template and keeps multiple scenes disabled.

`--check-resume` opens Settings after a static capture, reactivates the same app,
compares the effect ROI, then continues strength changes and all seven functional
scenes. The probe requires a foreground-active MAUI Scene before checking WebKit
animation and polls movement for up to eight seconds. App launch success alone is
insufficient. Evidence is under `ios/ios27/`; iOS 27 Simulator and the connected
iOS 26.6.1 phone are recorded separately.

## Existing iOS / NativeAOT profile

Run from `DorotiTestbedApp/ios` so its SDK selection applies. All children use the
repository's 1200-second process-tree timeout. Do not run .NET builds sharing
output paths concurrently. Use a single MSBuild node and one explicit artifacts
root; RID-less project queries otherwise use a different Host.Maui assets path.

```sh
python3 ../../Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp.iOS.csproj \
  -c Debug -r ios-arm64 -m:1 \
  --artifacts-path ../../Doroti/artifacts/platform-views/2026-09-17/ios/build \
  -p:DorotiHostTargetFrameworks=net10.0-ios
```

Device Release defaults to net11 NativeAOT. Do not combine net11 with Mono.
Simulator uses `iossimulator-arm64`, without the device signing requirement.
The historical simulator run explicitly set `-p:MtouchInterpreter=all` (net10
Mono interpreter); the preceding Mono AOT simulator build also passed. For the
current iOS 27 profile, use the scoped host-AOT workaround above. It is not a
NativeAOT simulator qualification.
Build, simulator, device, NativeAOT, physical input and visual acceptance are
separate results.

After signing/build, install the resulting `DorotiTestbedApp.iOS.app` with
`xcrun devicectl device install app --device <device> <app>`. Launch using
`--terminate-existing --environment-variables` with these values:

```json
{"DOROTI_TESTBED_MODE":"platform-views","DOROTI_UIKIT_EVIDENCE":"1"}
```

The opt-in probe executes ten actual widget scenes, native identity/editor state,
committed hit-test ordering, programmatic UIButton activation, native focus/text
insertion, and 100 create/dispose cycles. `platform-effects` instead checks
WKWebView identity, material insertion/removal, shield/pass-through targets,
two WebViews, effect movement, disposal and recreation. These are UIKit actions
and hierarchy checks, not physical touches, pixel comparison or IME approval.
The probe writes `Documents/platform-views-evidence.txt` in the app container:

```sh
xcrun devicectl device copy from --device <device> \
  --domain-type appDataContainer --domain-identifier dev.doroti.testbed \
  --source Documents/platform-views-evidence.txt --destination <result.txt>
```

Use a fresh launch and verify the result's contents before claiming a pass.
Optional device screenshots can be obtained through Xcode's device tools or
`pymobiledevice3 developer dvt screenshot --native <path.png>` with the app visible.
Screenshots must be inspected; existence alone proves no pixel gate.

Common contract regression (from workspace root):

```sh
python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/platform-views/Common/Common.csproj \
  --artifacts-path Doroti/artifacts/platform-views/2026-09-17/ios/common-build
```

For automated device collection, use a unique result filename per launch:

```sh
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/platform-views/ios/run-probe.py \
  --device <device> --app <built-app> --output <run-artifacts> --mode platform-views
# Repeat with --mode platform-effects and a separate output directory.
```

`DOROTI_UIKIT_EVIDENCE_NAME` selects that filename. The runner uses a UUID so an
old result cannot satisfy a new run. Release NativeAOT validation uses a separate
artifacts root to avoid mixing net10 Mono and net11 NativeAOT assemblies.

Actual device NativeAOT publication (from `DorotiTestbedApp/ios`):

```sh
python3 ../../Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp.iOS.csproj \
  -c Release -r ios-arm64 -m:1 \
  --artifacts-path ../../Doroti/artifacts/platform-views/2026-09-17/ios/release-build \
  -p:DorotiHostTargetFrameworks=net11.0-ios -p:CodesignProvision=<profile-UUID>
```

Inspect `native-link-inputs.txt`: it must say `UseNativeAot=true`. `PublishAot=true`
in a `dotnet build` log alone is insufficient; that command produced CoreCLR in
this toolchain. Install the published application and execute the probes again.

## Public blur calibration and appearance

The current implementation uses public UIKit material interpolation in
`UIKitPlatformBlurView.cs`. A retained, paused property animator maps common
strength directly to `FractionComplete`. The Light preset includes material tint;
measurements against Gaussian references are descriptive, not an exact-radius gate.
Historical private-filter results do not qualify this implementation.

On 2026-09-18, iPhone 18 Pro / iOS 27 Simulator passed the public adapter's
four-strength/two-theme appearance, Settings resume, and seven functional scenes.
The initial full-interpreter sRGB calibration crashed in Mono GC. Compiling the
host with Mono AOT subsequently passed three consecutive complete calibrations,
including two with GC verification enabled. Zero/decreasing intensity pixel
comparisons were exactly equal in both themes. Evidence is under
`artifacts/platform-views/2026-09-18/ios/public-blur/gc-fix/aot-run-{1,2,3}/`.

Launch with `DOROTI_TESTBED_MODE=platform-effects` and
`DOROTI_UIKIT_BLUR_CALIBRATION=1` to collect `Documents/blur-calibration`.
The probe overlays a native sRGB test image, renders Gaussian references through
Skia, and captures the entire UIWindow using the actual production blur adapter.
It measures .15/.25/.375/.75/1 strengths in both appearances, then decreases to
.375 and clears to zero. The old `DOROTI_UIKIT_GAUSSIAN_CALIBRATION` flag and KVC
exception probe have been removed. Use a fresh app data folder or separate captures
from earlier launches; the device folder can retain old files.
Copy that folder from the application container and analyze with Pillow:

```sh
python3 Doroti/validation/run-with-timeout.py <pillow-python> \
  Doroti/validation/platform-views/ios/analyze-blur-calibration.py <calibration-folder>
```

The analyzer fits a normalized black/white edge spread independently of tint,
checks reference sigma, and reports theme difference and colour error. The historical public-preset measurement on iPhone
12 / iOS 26.6.1 showed that the Light preset has approximately `sigma = fraction * 30pt`.
Its .4 fraction matches sigma 12 with RGB MAE 16.26/255 on the test ROI; material
colour bias remains. The old SystemMaterial baseline has Light/Dark MAE 160.83/255
on the full pattern, whereas the fixed Light preset has zero theme difference.
This calibration does not establish other OS/device behaviour or visual parity.

For the actual WKWebView/Metal fixture, use a published app and Pillow plus
`pymobiledevice3` in the host Python environment:

```sh
python3 Doroti/validation/run-with-timeout.py <pillow-python> \
  Doroti/validation/platform-views/ios/capture-blur-appearance.py \
  --device <device> --app <published-app> --output <capture-folder> \
  --screenshot-tool <pymobiledevice3-executable>
```

This opt-in probe freezes the HTML animation, sets four strengths and both parent
appearances, captures the whole UIWindow, and coordinates separate DVT screenshots
with acknowledgements. It restores theme/strength/animation/idle timer and runs the
seven existing effect lifecycle scenes. Analysis compares the visible effect ROI
and requires adjacent strengths to differ, so identical stale captures cannot
pass. Window hierarchy captures and physical display screenshots are distinct
evidence; neither implies physical touch/IME approval. The captures should include
the sharp foreground child and native content outside the effect.

2026-09-17 calibration, build and product evidence is under
`artifacts/platform-views/2026-09-17/ios/blur-match/`. Earlier failed or stale
capture attempts are retained separately.

For the same capture protocol on Simulator use `--simulator <UDID>` instead of
`--device`; Pillow is required but pymobiledevice3 is not. Simulator captures and
physical-device captures are separate evidence. Current C# evidence is collected
in `artifacts/platform-views/2026-09-17/ios/flutter-blur/managed-*`.

The earlier default .NET iOS pack required Xcode 26.6, while Xcode was updated
to 27.0 during this session. Those historical diagnostic builds explicitly passed
`-p:ValidateXcodeVersion=false`; no project-wide override was added. These results
do not qualify that mismatched combination for deployment. Use the iOS 27 profile
above for Xcode 27; it was built and run without the override.

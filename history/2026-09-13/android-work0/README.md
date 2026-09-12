# work0 Android review — 2026-09-13

Reviewed commit: `88cbb69e9774e23ec37a211fc8a0a68015ed8ea8` plus the changes
recorded in this review. Device: Galaxy S25 / SM-S931N, Android API 36,
`android-arm64`, Adreno 830. **Overall qualification remains PARTIAL.**
The [evidence index](index.json) records current source hashes, final run results
and raw log hashes.

## Corrections

- The testbed and generated Android manifests declared Vulkan 1.1 although the
  official renderer and target descriptors require 1.2. Both now declare
  `android.hardware.vulkan.version=0x00402000`.
- The generated Android `Directory.Build.props` omitted the parent workspace
  import. Consequently `--applicationId dev.doroti.work0consumer` still installed
  `AndroidConsumer.Android`, with version 1/1.0.0 and the assembly name as its
  label. It now imports the workspace metadata like the testbed and honors an
  explicit `ArtifactsPath`. The consumer checks evaluated ID/title/version before
  installation. The initial wrong-identity install is retained as failed evidence.
- `SurfaceDestroyed`/handler disconnect previously called `DeviceWaitIdle` and
  native Graphite disposal synchronously on Android's UI thread. The handler now
  removes draw/poll callbacks, closes frame admission and invalidates host caches
  on their original thread. The detached generation drains on a worker; ownership
  transfers only after actual GPU idle/device loss. Its ANativeWindow reference
  remains alive until successful disposal.
- Replacement renderers, including those in a new Activity/View, wait for pending
  retirement. A five-second overrun is logged while resources remain retained;
  it is not converted to device loss. Failed retirement keeps strong process-wide
  references and blocks replacement. Frame/poll failures also close admission.
- Added short reusable installed-APK/lifecycle/input probes and a real generated
  package-app consumer. The older `run-clean-android-package.py` constructs a
  native TextView and remains packaging evidence only.

The implementation follows the requirement to stop touching an Android surface
before its destroy callback returns ([SurfaceHolder.Callback](https://developer.android.com/reference/android/view/SurfaceHolder.Callback)).
It serializes queue access before moving the terminal wait off the UI thread
([vkDeviceWaitIdle](https://docs.vulkan.org/refpages/latest/refpages/source/vkDeviceWaitIdle.html)).
No further acquire/present is performed by the detached owner.

## Observed device checks

All test commands use the repository's 1,200-second outer timeout. Generated
files are under [the evidence directory](../../../Doroti/artifacts/validation/android-work0-2026-09-13/).

| Check | Evidence / boundary |
|---|---|
| Synthetic base/split APK provenance | 13 cases passed; no GPU claim |
| Testbed Release build / bundletool install | Official arm64 ABI split; Mono profiled AOT and trimming enabled, marshal methods disabled; not NativeAOT |
| Actual loaded native asset | `split_config.arm64_v8a.apk!/lib/arm64-v8a/libSkiaSharp.so`, SHA-256 `63af1ec283b86965542bca1400ae446e6a179a7be5b6187e69aa5fa0bd49e180` |
| Rotation | Automated portrait → landscape → reverse landscape → portrait, with screenshots; device rotation policy restored |
| Background/resume | Two HOME/resume cycles without process replacement; UI owner and retirement worker differ; completed/submitted counts match and outstanding frames are zero |
| Input/effects | Button count 0→1, `work0` text with Samsung keyboard/caret, runtime shader and image filter screen, Material navigation, Color/Components tabs and scrolling |

The first lifecycle run recorded `18/18/0` and `1/1/0`
(submitted/completed/outstanding). This proves normal retirement on this device,
not behavior under an intentionally delayed or permanently stalled GPU.
After adding the cross-view retirement gate, the final testbed run recorded
`19/19/0` and `1/1/0`, again with no renderer error or app ANR in the captured logs.

## Package consumer and failures retained

The corrected generated app passed Release Mono AOT/trim build and automatic
bundletool split installation. Its actual Graphite screen, portrait/landscape/
reverse-landscape and two background/resume cycles passed in process 10682.
Retirement was `12/12/0` then `1/1/0`. Installed package identity is
`dev.doroti.work0consumer`, title `AndroidConsumer`, version code 2 / version 0.2.0.
The installed ABI split has the same official Skia hash as the testbed, with no
custom host AAR and no XA4301 duplicate warning.
The generated app's Increment button updated its shader counter from 0 to 1.
Temporary consumer apps were removed after capture; the final testbed Release
remains installed and was brought back to the foreground.

The generated app build/install exited 0 with three warnings: one existing
Gradle SDK-XML version warning and two SDK pre-install uninstall warnings
(`MSB6006`/`DELETE_FAILED_INTERNAL_ERROR`). The Android SDK calls this uninstall
with `ContinueOnError` before installing the split set. Subsequent installation,
installed paths, official runtime provenance and UI execution were checked; the
warnings are preserved, not hidden or called a zero-warning build.

The consumer uses an isolated temporary workspace, `NUGET_PACKAGES` and
`DOTNET_CLI_HOME`, a locally packed feed, and the actual `dotnet new doroti-app`
template. Doroti dependencies must resolve as packages. The app's own C# and
Java binding projects remain normal generated source projects. Skia is not built
from source; the generated Java bridge's Gradle build is separate.

The initial probe had two tooling errors, retained in the evidence directory:

1. `dotnet restore -r` supplies `RuntimeIdentifiers`, while the fixed runner
   requires `RuntimeIdentifier`. The probe now sets that property explicitly.
2. `pack --no-build` selected an older standalone host DLL/AAR from a different
   output path than the just-built testbed's project reference. That stale AAR
   carried custom Skia files and produced Android warning XA4301. A normal pack
   rebuilt the host and removed the obsolete AAR. The probe now builds while
   packing, rejects Skia/Graphite entries nested in any Doroti AAR, and rejects
   XA4301 instead of accepting Android's duplicate-entry suppression. The initial
   install is not accepted as final package or runtime evidence.

## Remaining acceptance boundaries

- This run does not qualify x64 emulators, other Android versions/GPUs, physical
  touch/display latency, Korean IME composition/selection or accessibility.
- No Android Vulkan/synchronization validation layer was installed. A crash-free
  device run is not proof of zero synchronization validation errors. The prior
  Linux depth-attachment synchronization failure is not erased by this run.
- Delayed GPU retirement, permanent stall, actual device loss and fault recovery
  remain `notVerified` on Android. The source still has synchronous waits in
  swapchain resize and failed-submit cleanup; the change specifically moves
  surface terminal retirement off the UI thread. It does not establish a bound
  on every native rendering call or full process shutdown.
- No new long baseline/candidate performance repetition was started. Existing
  performance acceptance remains incomplete. Android uses Mono AOT; the runner's
  experimental NativeAOT mode is iOS-only, not an Android mode to claim as passed.
- Some older `work0.md` links under `Doroti/docs/validation` and the 2026-09-12
  history are absent from this checkout. Historical claims were not treated as
  current evidence; current source, APKs, logs and screens were inspected again.

## Reproduction

Run from `Doroti/` with an explicitly selected device:

```powershell
python validation/run-with-timeout.py dotnet build ../DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -t:Install -p:AdbTarget=-s%20R3CY30KZA4B
python validation/run-with-timeout.py dotnet run --project validation/android-graphite-apk -c Release
python validation/run-with-timeout.py python validation/android-graphite-apk/device-lifecycle.py --serial R3CY30KZA4B --output artifacts/validation/android-work0/lifecycle
python validation/run-with-timeout.py python validation/android-graphite-apk/testbed-input.py --serial R3CY30KZA4B --output artifacts/validation/android-work0/input
python validation/run-with-timeout.py python validation/android-graphite-apk/testbed-input.py --serial R3CY30KZA4B --material-only --output artifacts/validation/android-work0/material
python validation/run-with-timeout.py python validation/stock-graphite-vulkan/run-clean-android-template.py --serial R3CY30KZA4B --output artifacts/validation/android-work0/template
python validation/run-with-timeout.py python validation/android-graphite-apk/device-lifecycle.py --serial R3CY30KZA4B --package dev.doroti.work0consumer --output artifacts/validation/android-work0/template-lifecycle
python validation/run-with-timeout.py python validation/android-graphite-apk/testbed-input.py --serial R3CY30KZA4B --template-only --output artifacts/validation/android-work0/template-input
```

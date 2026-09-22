# Texture validation — 2026-09-22

All commands run from the repository root. Tests use `run-with-timeout.py`, which
enforces the repository's 1,200-second process-tree deadline.

## Common contracts

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/Textures.csproj
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/NativeContracts/NativeContracts.csproj
```

- CPU pixel/scene/lifecycle/owner-context fixture: **29 PASS**.
- Native adapter registration and external Vulkan ownership/frame lifetime fixture: **25 PASS**.
  The ownership fixture compiles the production `SubmissionJournal` source and
  supplies synthetic API results to verify state transitions, failed submission,
  rejected foreign owners and the unchanged single-queue rule for ordinary targets.
  These are contract tests, not simulated GPU acceptance.

## Android installation and acceptance

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-x64 -p:AndroidPackageFormats=apk
python Doroti/validation/run-with-timeout.py adb -s emulator-5554 install -r DorotiTestbedApp/android/bin/android-x64/Release/net10.0-android/android-x64/dev.doroti.testbed-Signed.apk
adb -s emulator-5554 shell pm grant dev.doroti.testbed android.permission.CAMERA
adb -s emulator-5554 logcat -c
adb -s emulator-5554 shell am start -n dev.doroti.testbed/crc64c80c495bd333b69c.MainActivity --es doroti_testbed_mode texture-native --es doroti_texture_source canvas
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/verify-android.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/verify-android-sources.py
```

The scripts require Pillow and NumPy, inspect a bounded number of screenshots,
and stop on failed assertions. `verify-android.py` expects the native canvas
fixture running in the foreground. It waits for actual native frame counters
before inspecting colors, avoiding launch/splash/home-screen false positives.
The source script runs Camera2 followed by MediaPlayer, leaving the video visible.

For an individual source, launch with `doroti_texture_source` set to `canvas`,
`camera` or `video`. The camera fixture requests 640×480 from the emulator camera;
the video fixture decodes the local generated H.264 clip into a native Surface.
The native fixture is opt-in; ordinary Material sample startup is preserved.

Evidence is under `Doroti/artifacts/textures/android/`:

| Final check | Result |
| --- | --- |
| Release APK build/install | PASS, final build zero warnings/errors |
| Installed APK vs built signed APK SHA-256 | MATCH: `6541365bead19879d26bc336c39e77a37b70b6b8d2f8b484fb45719618ff4820` (`provenance.json`) |
| Device | Android 13/API 33 x86_64, `emulator-5554`; Graphite reported NVIDIA GeForce RTX 4060 Laptop GPU |
| Native hardware Canvas | PASS, 9 checks: readiness, quadrant colors/orientation, controls, frozen pixels, resume motion, recreation, background/resume, runtime errors, matching imported/retired counters (`canvas-checks.json`) |
| Camera2 virtual scene | PASS, visible upright image; final bounded sample received 72, drew/imported/retired 71 (`camera-final.log`, `camera-final-b.png`) |
| MediaPlayer H.264 | PASS, visible changing video; final bounded sample received 85, drew/imported/retired 82 (`video-final.log`, `video-final-b.png`) |
| Native source errors | None in final bounded Camera2/MediaPlayer/canvas logs |
| Android arm64 host, Windows App SDK, Web and Qt managed host builds | PASS, zero warnings/errors; historical baseline; arm64 physical execution unverified, cross-platform native follow-up below |

The first native run exposed the old journal's single-queue assumption; the
fix allows only explicitly registered foreign-input images to transfer between
their declared renderer family and `VK_QUEUE_FAMILY_FOREIGN_EXT`. Ordinary
Graphite targets still reject queue migration. A hardware quadrant test also
caught the OES/Vulkan vertical-coordinate mismatch before final acceptance.

The native path executes `SurfaceTexture` OES sampling → RGBA ImageReader →
AHardwareBuffer import → Graphite sampling. It contains no `ReadPixels`, bitmap
readback, CPU `PushFrame` or CPU pixel upload. The diagnostic import/retirement
counters establish actual use of that path; they are not CPU-transfer byte
measurements. ADB screenshots are separate OS display captures. MediaPlayer
acceptance does not establish whether a particular device decoder is hardware
accelerated.

Not verified: physical Android devices, sensor rotation/front-camera mirroring,
HDR/protected content, API 26–32, sustained frame pacing/power/memory stress,
Vulkan validation-layer instrumentation and injected device loss. The importer
currently uses a conservative completion wait and per-frame Vulkan wrappers.
No claim of zero GPU copies, zero CPU synchronization or full Flutter backend
parity is made.

See [API and architecture](../../docs/textures.md). The native ownership rules
follow Android's [Image/reader and fence APIs](https://developer.android.com/ndk/reference/group/media)
and Vulkan's [Android hardware-buffer sharing rules](https://docs.vulkan.org/spec/latest/chapters/memory.html#memory-external-android-hardware-buffer).

## Windows native GPU input — 2026-09-22 follow-up

Only Windows was executed in this follow-up. Apple and Linux platform code was
configured and reviewed statically; no target builds or runs were performed.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/verify-windows.py
# Optional variants; set one before the same verifier, then remove it afterward:
$env:DOROTI_WINDOWS_GPU_PREFERENCE = 'HighPerformancePreference'
$env:DOROTI_WINDOWS_APPSDK_DEVICE_RESET_COUNT = '1'
$env:DOROTI_TEXTURE_CAMERA = '1'
```

The verifier launches and closes its own product window. The default D3D11 pattern
is generated with GPU ClearView calls, then imported by Vulkan and presented by
the actual D3D12 product path. Assertions cover quadrant colors/orientation,
rounded clipping, advancing frames, frozen pixels, resume, recreation, window
minimize/restore, clean shutdown and matching import/retirement counts. Input is
synthetic HWND dispatch; screenshots are local OS display captures. The camera
variant requires changing actual camera frames and nonzero Media Foundation GPU
sample counts, then checks the same native import retirement/validation reports.

| Check | Evidence/result |
| --- | --- |
| Final Windows Release build | PASS, 0 warnings/errors, includes native C++ producer |
| Common CPU contracts | 29 PASS |
| Native contracts | 25 PASS: foreign/external ownership, rejected transfers, frame leases, cross-view rejection, latest pending replacement, raster revision and exactly-once release |
| AMD Radeon 780M pattern lifecycle | PASS, 265 imports / 265 retired / 0 live, `windows/20260922-061400/result.json` |
| NVIDIA RTX 4060 pattern lifecycle | PASS, 387 / 387 / 0, `windows/20260922-061504/result.json` |
| Final AMD pattern + startup context reset | PASS, one reset, retired sessions 0/0 and 281/281, `windows/20260922-061950/result.json` |
| Final AMD real camera preview | PASS, 70 / 70 / 0, `windows/20260922-062025/result.json`; camera closed on exit |
| Vulkan validation / D3D12 debug | Enabled; 0 errors in all successful runs above |
| Apple plist, Qt targets XML and template/testbed source parity | Static checks PASS |
| iOS/macOS/Catalyst, Linux camera/driver/runtime/build | notVerified, source only per requested scope |

Evidence paths are relative to `Doroti/artifacts/textures/`. The startup reset
happens before the first imported camera/pattern frame; it is not a midstream
native-buffer device-loss test. NV12/YUY2 conversion, decoder integration,
physical input, Apple/Linux cameras, hot unplug, sustained latency/power/memory,
and scan-out timing remain unverified.

The Windows check exposed a retained-scene optimization that dropped producer-only
wakeups. The host now tracks the renderer's texture revision independently of
widget scene revisions, preserving idle-frame suppression while presenting fresh
native/CPU texture frames without a framework rebuild.

## Web native browser sources — 2026-09-22

See [Web implementation and final validation](web-results-2026-09-22.md). Both published WebGPU and explicit WebGL passed source/lifetime and 46 pixel checks each. The pre-existing Debug runtime startup failure was [fixed and independently verified](../web-debug-startup-2026-09-22.md). Physical camera qualification remains separate.

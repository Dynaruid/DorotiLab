# Android PlatformView / WebView validation

Run from the repository root. Every build/test/run child uses the 1,200-second
wrapper. Build shared .NET outputs sequentially; target devices explicitly.

```powershell
python Doroti/validation/run-with-timeout.py pwsh -NoProfile -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp -Platform android -Rid android-arm64 -Configuration Release
python Doroti/validation/run-with-timeout.py adb -s R3CY30KZA4B install -r DorotiTestbedApp/android/bin/android-arm64/Release/net10.0-android/android-arm64/dev.doroti.testbed-Signed.apk
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-android.py --serial R3CY30KZA4B --out Doroti/artifacts/webview/2026-09-20/android/commands-new --mode commands
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-android.py --serial R3CY30KZA4B --out Doroti/artifacts/webview/2026-09-20/android/calibration-new --mode calibration
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-android-calibration.py Doroti/artifacts/webview/2026-09-20/android/calibration-new --scale 3 --row 820 --edge 930 --color-x 600 --require-color-edge
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-android.py --serial R3CY30KZA4B --out Doroti/artifacts/webview/2026-09-20/android/workloads-new --mode workloads
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-android-input.py --serial R3CY30KZA4B --out Doroti/artifacts/webview/2026-09-20/android/input-new
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/webview/Common/Common.csproj -c Release
```

The recorded serial and pixel coordinates are for the current Galaxy at 1080×2340,
density 3, portrait, with the system font/display configuration captured in `device.txt`.
Requery `adb devices -l` and inspect the current screenshot before using another
device/rotation or selecting measurement coordinates. For x64, substitute
`android-x64` in the build/APK path and the selected emulator serial. Do not install
the stale `publish/` APK without independently verifying its build/hash.

`commands` uses the public controller in the product effect scene and records actual
provider results. `calibration` captures 13 acknowledged stages; the separate pixel
validator checks sigma against an analytical Gaussian edge, not the implementation's
radius formula. `workloads` uses eight bounded 0/1/4-view cases and captures window frame
statistics and owner-process memory. `capture` captures two live frames for source
freshness/manual pixel inspection. Artifacts include screenshots, device/provider,
and logcat; prior failed outputs are retained under separate run directories.

The command fixture and calibration are independently selected with activity extras
`doroti_webview_evidence=1` and `doroti_effect_calibration=1`. Do not enable them at
the same time. Extras `doroti_webview_count=0|1|4` and `doroti_webview_workload` select
the `webview-workload` scene. `DOROTI_MAUI_EVIDENCE=1` enables composition cost logs;
`DOROTI_INPUT_TIMING=1` enables input dispatch timing. Files are written under the
testbed's external files directory; no server or debug JavaScript endpoint is opened.

For input/lifecycle, use the actual native input/counter plus the Doroti button and
text field. Verify native tap once, foreground/modal shield, scroll, native→Doroti→
native focus, Korean composing/commit/selection, two native views, disposal/recreation,
rotation and Home/resume. Record original Android rotation settings and restore them.
Avoid treating Back (which can close the Activity) as Home/background. UIAutomator
node presence is not TalkBack navigation approval.

For each final APK, record source hashes, SDK, provider, ABI, archive hash and installed
package path. Separate build, Mono AOT, product screenshots, automatic device input,
physical input and NativeAOT. Current limits and failure boundaries are in the
[Android contract](../../../docs/platform-views/android-webview.md).

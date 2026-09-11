# Haptic feedback regression

Run from `Doroti/`. Every validation command uses the required 20-minute timeout.

```powershell
python validation/run-with-timeout.py dotnet run --project validation/haptic-feedback -c Release
python validation/run-with-timeout.py dotnet build src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release
python validation/run-with-timeout.py node validation/haptic-feedback/web-bridge.mjs
python validation/run-with-timeout.py dotnet build src/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Release -r android-arm64
python validation/run-with-timeout.py dotnet build ../DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Debug -r android-arm64
```

The C# fixture invokes all eight public framework feedback methods through the
real JSON codec and production host routing. It checks invalid arguments,
unrelated channels, handler delegation, cancellation, asynchronous completion,
unsupported hardware completion and the production browser duration mapping.
Only the final hardware/JavaScript boundary is replaced by a recorder.

The fixture also exercises `SystemSound.play` through the composed sound/haptic
platform capabilities. It checks Flutter's `SystemSoundType.*` wire names, all
three sound types, rejected arguments, cancellation, awaited completion, fallback
delegation, and that playing sounds never requests haptics.

## Android Material feedback behavior

The pinned Flutter `56b8e1a8` sources use click **sound**, not vibration, for
`Feedback.forTap`. `InkWell` requests long-press haptics only when a long-press
callback is registered and feedback is enabled. The standalone Material Switch
does not request haptics on tap or drag (`widgets/toggleable.dart`,
`material/switch.dart`). Ordinary sample buttons have no long-press callback.

The Android MAUI host now routes `SystemSound.play` to the activity decor view's
`PlaySoundEffect(Click)`, matching Flutter's Android `PlatformPlugin.java`.
Android tick/alert remain no-ops. Other MAUI hosts retain their existing sound
no-op behavior. Touch-sound preferences are respected, and button/switch taps
have not been changed to vibrate.

The Node fixture imports the actual emitted browser module. It verifies
`navigator.vibrate` calls, missing/denied API behavior, and the Worker sender's
awaited control request. It does not exercise the full managed browser runtime
or a physical vibration actuator.

The repository's iOS Release device profile selects .NET 11 NativeAOT. Build
from the workspace root with its installed .NET 11 SDK (the nested `Doroti/`
`global.json` selects .NET 10):

```powershell
python Doroti/validation/run-with-timeout.py dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Release -r ios-arm64
```

2026-09-11: C# and emitted JavaScript regressions passed. Android MAUI,
Web (including TypeScript), and iOS MAUI managed host builds passed with no
warnings or errors. The iOS result is a managed library build on Windows, not
NativeAOT linking, signing, deployment or Apple device execution.

The Android Testbed Debug arm64 application build also passed with no warnings
or errors. Its generated manifests contain `android.permission.VIBRATE`,
inherited from the host assembly.

In that earlier validation pass, physical feedback was `notVerified`: the connected Android device reported ADB
`unauthorized`; Apple device and physical browser feedback were not run.
To finish device validation, authorize USB debugging, rebuild/install the
Android app, call all eight public methods and check system haptics enabled /
disabled. API <30 notification no-ops also need a suitable device or emulator.
No device permission or haptic preference is changed by the validator.

2026-09-11 follow-up (button/switch feedback): the sound/haptic C# regression
passed, and Android arm64 Release JIT builds passed with zero warnings/errors
under the 20-minute timeout. The sample-mode APK was installed on Galaxy S25
`SM_S931N` (`R3CY30KZA4B`), update time `09:25:25` KST. PID `30209` reached
`MainActivity` / `RESUMED`; the sample rendered and no exception/crash was found
in the captured app log. A Filled button tap added no vibrator record. Long
pressing the brightness icon displayed its tooltip and added a completed
`dev.doroti.testbed` vibration at `09:25:53.519`, Android feedback constant `0`
(LONG_PRESS), duration `122ms`. This verifies that existing long-press haptics
reach the physical Android vibration service; it does not qualify all eight
haptic types or subjective sensation. Touch sounds were disabled (`0`) and
haptics enabled (`1`) before and after the run; audible playback remains
`notVerified`. No phone feedback settings were changed.

Local run artifacts are under `Doroti/artifacts/android-feedback-*`: build
logs, installed/tooltip screenshots, app log, activity state, and vibrator
snapshots before input, after tap, and after long press. The `dotnet run`
deployment command stalled before output and was stopped. Direct sample-mode
build/ADB install succeeded; the temporary environment targets used for that
build are in the same ignored artifacts directory.

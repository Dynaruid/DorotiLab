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

Physical feedback is `notVerified`: the connected Android device reported ADB
`unauthorized`; Apple device and physical browser feedback were not run.
To finish device validation, authorize USB debugging, rebuild/install the
Android app, call all eight public methods and check system haptics enabled /
disabled. API <30 notification no-ops also need a suitable device or emulator.
No device permission or haptic preference is changed by the validator.

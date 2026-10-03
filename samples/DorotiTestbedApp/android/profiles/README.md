# Android startup baseline profile

The profile covers the Release startup journey: reset ART state, cold launch the
Material gallery, focus the initial text field, commit `Startup123`, and stop the app.
Binary profiles must match the target APK's DEX checksums. The generator rejects
empty decoded snapshots to protect the existing profile.

Regenerate after Java/Kotlin or DEX changes by installing the exact Release APK,
exercising this startup journey, and running the command below. Replace
`device-serial` with the target serial from `adb devices -l`.

```powershell
pwsh -NoProfile -File .\Doroti\eng\generate-android-baseline-profile.ps1 `
  -Serial device-serial `
  -Apk .\samples\DorotiTestbedApp\android\bin\android-arm64\Release\net10.0-android\android-arm64\dev.doroti.testbed-Signed.apk `
  -OutputDirectory .\samples\DorotiTestbedApp\android\profiles
```

The Runner SDK packages the binary pair as `assets/dexopt/baseline.prof` and
`assets/dexopt/baseline.profm`. This ART profile covers the Android Java/Kotlin
startup path; it does not replace managed AOT or prove a TTID improvement.

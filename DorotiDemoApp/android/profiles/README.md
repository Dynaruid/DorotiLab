# Android startup baseline profile

`baseline-prof.txt`, `baseline.prof`, and `baseline.profm` were captured from the
Release startup CUJ on `R3CY30KZA4B`: reset ART state, cold launch the Material
gallery, focus the initial text field, commit `Startup123`, and stop the app.

Regenerate after Java/Kotlin or DEX changes by installing the exact Release APK,
exercising the same CUJ, and running:

```powershell
pwsh -NoProfile -File .\Doroti\eng\generate-android-baseline-profile.ps1 `
  -Serial R3CY30KZA4B `
  -Apk .\DorotiDemoApp\android\bin\android-arm64\Release\net10.0-android\android-arm64\dev.doroti.demo-Signed.apk `
  -OutputDirectory .\DorotiDemoApp\android\profiles
```

The Runner SDK packages the binary pair as `assets/dexopt/baseline.prof` and
`assets/dexopt/baseline.profm`. This ART profile covers the Android Java/Kotlin
startup path; it does not replace managed AOT or prove a TTID improvement.

# Android startup baseline profile

The original profile rules were captured from the Release startup CUJ on
`R3CY30KZA4B`: reset ART state, cold launch the Material
gallery, focus the initial text field, commit `Startup123`, and stop the app.

On 2026-09-07 the previous binary profile failed strict decode against the current
DEX checksum. Rules for the renamed Testbed `MainActivity`/`MainApplication`
wrappers were rebound from `crc641bb9c91451c1be9f` to
`crc64c80c495bd333b69c`, then the binary pair was encoded against the current APK.
All 3,630 rules survived strict decode without additions or omissions. The current
arm64 and x64 Release APKs have identical DEX bytes and both passed strict decode.
This is a symbolic rebind of the earlier physical CUJ, not a new device CUJ capture.

The x64 emulator accepted ProfileInstaller's explicit install request and ART
`speed-profile` compilation. This forced diagnostic state does not prove automatic
installation behavior or arm64 runtime performance. Its profile snapshot was empty,
so the generator now refuses an empty decoded snapshot instead of replacing the
existing profile. The old profile and failures are preserved in the
[boot results](../../../history/26-09-07/cross-platform-boot-results.md).

Regenerate after Java/Kotlin or DEX changes by installing the exact Release APK,
exercising the same CUJ, and running:

```powershell
pwsh -NoProfile -File .\Doroti\eng\generate-android-baseline-profile.ps1 `
  -Serial R3CY30KZA4B `
  -Apk .\DorotiTestbedApp\android\bin\android-arm64\Release\net10.0-android\android-arm64\dev.doroti.testbed-Signed.apk `
  -OutputDirectory .\DorotiTestbedApp\android\profiles
```

The Runner SDK packages the binary pair as `assets/dexopt/baseline.prof` and
`assets/dexopt/baseline.profm`. This ART profile covers the Android Java/Kotlin
startup path; it does not replace managed AOT or prove a TTID improvement.

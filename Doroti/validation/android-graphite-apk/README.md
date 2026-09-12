# Android Graphite APK provenance

The product validates the complete installed APK set: `ApplicationInfo.SourceDir`
and `ApplicationInfo.SplitSourceDirs`. Android defines `SourceDir` as the base APK;
ABI-specific native libraries can reside in a split instead. See the
[Android ApplicationInfo contract](https://developer.android.com/reference/android/content/pm/ApplicationInfo#splitSourceDirs).

The current process ABI must have exactly one `libSkiaSharp.so` across that set,
with the pinned official package hash. Direct APK loading must point to the archive
that passed verification. Extracted loading still requires a verified file in
`NativeLibraryDir`. Duplicate, missing, changed and unrelated loaded assets fail.
The existing two-argument standalone APK API remains available.

Run from `Doroti/` to select the pinned .NET SDK:

```powershell
python validation/run-with-timeout.py dotnet run --project validation/android-graphite-apk -c Release
```

The 13 cases use synthetic ZIP archives to cover standalone/split selection,
other ABIs and resource splits, repeated paths, duplicate entries within/across
archives, missing entries, hash mismatch and verified/unverified load paths.
These tests link the product verification helper; they do not load native Skia
or claim device/GPU validation.

`device-lifecycle.py --serial SERIAL --output ARTIFACT_DIRECTORY` checks an
already installed app's official split asset, rotation and two background/resume
cycles. It restores the rotation policy and retains screenshots/logs. Use
`--package dev.doroti.work0consumer` for the generated package consumer.
`testbed-input.py` then checks gallery button/text input and Material navigation;
`--material-only` checks tabs/scroll on the Material screen. Run each through
`validation/run-with-timeout.py`. These are automated device checks, not physical
input, synchronization-validation or performance acceptance.
For the generated app, `testbed-input.py --template-only` verifies its Increment
button and shader counter after the lifecycle probe.

See the [2026-09-13 review](../../../history/2026-09-13/android-work0/README.md)
and `../stock-graphite-vulkan/run-clean-android-template.py` for actual generated
package-app deployment rather than the older native TextView packaging fixture.

For device validation, use the normal Release `dotnet run` deployment (AAB and
bundletool splits), inspect `pm path --user 0 dev.doroti.testbed`, and confirm the
`DorotiGraphite official ... apk=... loaded=...` log plus the displayed app screen.

## Galaxy execution, 2026-09-12

- Before the fix, normal Release `dotnet run` installed splits but startup threw
  `APK must contain exactly one official Skia library for this ABI.` A standalone
  APK worked; that workaround did not validate split loading.
- After the fix, the normal Release/arm64 `dotnet run` build and `DeployToDevice`
  completed with zero errors (one existing Gradle SDK XML version warning).
  Bundletool installed base, arm64, Korean-language and xxhdpi APKs on SM-S931N.
- The first automatically launched process (25732) verified
  `split_config.arm64_v8a.apk` and loaded
  `split_config.arm64_v8a.apk!/lib/arm64-v8a/libSkiaSharp.so` directly.
  Official SHA-256: `63af1ec283b86965542bca1400ae446e6a179a7be5b6187e69aa5fa0bd49e180`.
  Graphite reported Adreno 830; the foreground Material 3 screen was captured and
  visually checked. No fatal exception/signal was found in this process log.
- Force-stop followed by a fresh activity launch also succeeded with the same
  split asset. The deployment command exited 0 when the first app process stopped.
- Evidence: [result](../../artifacts/android-split-apk/result.json),
  [build/deploy log](../../artifacts/android-split-apk/build-run.log),
  [installed paths](../../artifacts/android-split-apk/installed-paths.txt),
  [first launch](../../artifacts/android-split-apk/logcat-first-launch.txt),
  [cold relaunch](../../artifacts/android-split-apk/logcat-relaunch.txt),
  [screen](../../artifacts/android-split-apk/screen.png),
  [13-case result](../../artifacts/android-split-apk/provenance-tests.log).

This is scoped Galaxy arm64 product evidence. The fixture tests cover other ABI
selection, but no x64 device run or full feature/performance qualification was repeated.

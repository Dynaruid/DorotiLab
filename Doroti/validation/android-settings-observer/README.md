# Android settings observer lifecycle regression

The Galaxy Release JIT crash on 2026-09-11 reported `System.NotSupportedException:
Unable to activate instance of type ...MauiViewEnvironment+SettingsObserver` from
`ContentObserver.n_OnChange_Z`.

`MauiViewEnvironment` unregisters and disposes its observer when a view detaches
or reconnects. Android can already have posted a notification to its main-thread
handler. The previous observer had no JNI handle constructor, so a callback after
managed disposal crashed while recreating the peer.

The production observer now owns and clears its dedicated handler queue, releases
its owner callback on disposal, and supports an inert JNI peer for a late native
callback. Its handle constructor is retained in trimmed builds. Existing Release,
JIT, trimming, and renderer settings are unchanged.

The test APK links the actual production observer source and checks:

1. A live observer receives a Java-dispatched notification.
2. Disposal cancels a queued notification.
3. A retained Java reference can call back after managed disposal without an
   exception or refreshing the detached owner.
4. A replacement observer still receives notifications.

Run from the repository root with a connected Android arm64 device:

```powershell
python Doroti/validation/run-with-timeout.py dotnet build Doroti/validation/android-settings-observer/Doroti.Validation.AndroidSettingsObserver.csproj -c Release -t:SignAndroidPackage
python Doroti/validation/run-with-timeout.py python Doroti/validation/android-settings-observer/run.py --serial <serial> --apk Doroti/artifacts/validation/build/android-settings-observer/bin/Release/net10.0-android/android-arm64/dev.doroti.validation.settingsobserver-Signed.apk --output artifacts/android-settings-observer
```

The runner installs a separate validation package and returns nonzero on failure.
Remove that package after testing with `adb -s <serial> uninstall
dev.doroti.validation.settingsobserver`. Each external command has a 20-minute
timeout; the outer wrapper also bounds the complete run.

For a negative control, `-p:ObserverSource=<absolute-path>` can select the previous
observer implementation, renamed to `MauiSettingsObserver`. On Galaxy S25
`R3CY30KZA4B`, that implementation reproduced the same native-peer activation
crash; the production implementation passed all four cases in Release JIT with
trimming enabled. Local evidence is under
`artifacts/android-release-jit-crash/observer-before` and `observer-after`.

The fixed `DorotiTestbedApp` was also built with
`RunAOTCompilation=false` and `AndroidEnableProfiledAot=false`, then installed
over the existing app without clearing its data. The build had zero warnings or
errors; the signed APK contained zero `libaot-*` libraries. Three cold launches
(20 seconds each), two background/foreground cycles with the same process, and
opening the Material sample passed with no app crash or ANR observed. Screenshots,
process/activity evidence, build properties, and the APK SHA-256 are retained
under `artifacts/android-release-jit-crash`. This is startup/lifecycle regression
coverage on the connected Galaxy S25, not extended soak or other-device coverage.

References: Android's [ContentObserver dispatch and transport release](https://github.com/aosp-mirror/platform_frameworks_base/blob/master/core/java/android/database/ContentObserver.java)
and .NET Android's [native-peer construction](https://github.com/dotnet/android/blob/main/src/Mono.Android/Java.Interop/TypeManager.cs).

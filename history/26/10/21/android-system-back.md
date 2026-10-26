# Android system Back and modal navigation

Date: 2026-10-21

## Change

Android system Back previously reached MAUI's root ContentPage rather than
Doroti's Navigator. WidgetsFlutterBinding also omitted the navigation and back
gesture channel registrations, and Android keyboard input translated Back into
Escape. Initial lifecycle names also lacked the expected `AppLifecycleState.`
prefix, and typed native lifecycle events were not wired into ServicesBinding.
WidgetsApp therefore skipped navigation notifications while its lifecycle was
unknown. Together these paths could background the Activity with a dialog open.

- Register `flutter/navigation` and `flutter/backgesture` when the widget binding
  starts and clear those listeners when it is disposed.
- Preserve the Flutter lifecycle wire format and route typed host lifecycle
  changes through ServicesBinding's existing transition/observer handler. Clear
  the callback on disposal. WidgetsApp can now report Back ownership from the
  first route and after pause/resume.
- Add a per-view `AndroidBackNavigation` platform-message adapter. Native Back
  dispatches `popRoute` inside the originating view's framework event scope.
  Existing WidgetsApp/Router, Navigator.maybePop, local history and PopScope
  determine whether the request is consumed. Concurrent presses are coalesced
  while an asynchronous Back decision is pending.
- Bind an AndroidX OnBackPressedCallback to the native Activity lifecycle.
  `SystemNavigator.setFrameworkHandlesBack` controls its enabled state so the
  system retains root back-to-home behavior. `SystemNavigator.pop` temporarily
  bypasses this callback to avoid recursively dispatching another framework Back.
  Handler/load changes rebind the callback; unload/close remove it.
- Leave the Android Back key to the system dispatcher/IME. Hardware Escape
  remains a framework keyboard event.

The behavior follows Flutter's [handlePopRoute](https://api.flutter.dev/flutter/widgets/WidgetsBinding/handlePopRoute.html)
and [binding channel setup](https://api.flutter.dev/flutter/widgets/WidgetsBinding/initInstances.html).
AndroidX callback enablement follows the [Android system Back integration](https://developer.android.com/guide/navigation/custom-back/predictive-back-gesture).
This change handles committed Back requests; it does not add an interactive
route transition driven by predictive-gesture progress.

## Verification

All managed test commands used `python Doroti/eng/run-with-timeout.py --timeout 1200`.

- PASS: `dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --android-back`.
  Covers stacked dialogs, a non-dismissible barrier, page and local history,
  PopScope on pushed/root pages, root fallback, explicit SystemNavigator.pop,
  unrelated platform-method forwarding, async decision coalescing, Router and
  disposal, delivery under a different ambient dispatcher, initial lifecycle
  parsing and typed pause/resume observer order.
  Also runs the activation/restoration regression.
- PASS: `dotnet run --project packages/Doroti.Material/tests/Doroti.Material.Tests -c Release`.
- PASS: `dotnet run --project packages/Doroti.Cupertino/tests/Doroti.Cupertino.Tests -c Release`.
  These are CPU/synthetic widget, input and rendering checks, including existing
  pointer-opened Cupertino dialog and context-menu cases.
- PASS: Final Testbed Android x64 Debug build, 0 errors and 0 warnings, with
  `EmbedAssembliesIntoApk=true` and `PublishTrimmed=false`. These flags produce
  the standalone untrimmed APK used below; this is not Release/trimmed acceptance.
- PASS: Pixel 5 Android 13/API 33 x64 emulator, native Graphite/Vulkan rendering,
  final APK installed using `adb install -r`. Real Android Back dispatch via
  `adb shell input keyevent 4` closes the modal bottom sheet, AlertDialog and
  full-screen dialog. Accessibility trees and screenshots show the content
  disappearing; the same PID and resumed Testbed Activity are retained.
- PASS: Recreate the Activity with an AlertDialog open using the Testbed's
  `doroti_recreate_probe` intent. The dialog survives with the same PID; Back
  closes it and retains the resumed app. Back with no modal then resumes the
  Android launcher, confirming the root system fallback.
- Final runtime receipt: `temp/testing/android-back-20261021/final-runtime.json`.
  APK SHA-256: `048464a7f27da102d57bbd09ad984c6939892b9b40b3278ba4a7cd6716931052`.
  Temporary channel tracing was removed before this final build/run. The final
  log contains neither that tracing nor unobserved Android system Back failures.

Local raw logs: `temp/testing/android-back-20261021/`. These files are disposable
local output, not tracked delivery artifacts. Physical Android, real IME,
gesture cancellation/progress and Android 16 predictive animation remain unverified.

## Reproduce

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --android-back
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Debug -r android-x64 -p:AndroidPackageFormats=apk -p:EmbedAssembliesIntoApk=true -p:PublishTrimmed=false
```

On an Android device, open a Testbed dialog/full-screen dialog/modal bottom sheet,
press Back, and check that the modal closes while the app remains foreground.
On the root screen without a modal or PopScope veto, Back should use the system's
normal Activity/task behavior. With an IME visible, first verify its native
dismissal before checking the next Back request.

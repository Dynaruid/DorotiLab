# Haptic feedback

Doroti's existing `Doroti.Framework.Services.HapticFeedback` methods now reach
the MAUI and Web hosts through `flutter/platform` / `HapticFeedback.vibrate`.
Previously the framework and widget calls existed, but the hosts returned a
missing-plugin response that `OptionalMethodChannel` silently ignored.

The comparison baseline is the bundled Flutter revision
`56b8e1a851a594b1a154f8ea93270807dab22b9a`:

- `packages/flutter/lib/src/services/haptic_feedback.dart`
- `engine/src/flutter/shell/platform/android/io/flutter/plugin/platform/PlatformPlugin.java`
- `engine/src/flutter/shell/platform/darwin/ios/framework/Source/FlutterPlatformPlugin.mm`
- `engine/src/flutter/lib/web_ui/lib/src/engine/platform_dispatcher.dart`

| Framework method | Android | iOS / UIKit | Web duration |
| --- | --- | --- | --- |
| `vibrate()` | `LONG_PRESS` | System vibration | 50 ms |
| `lightImpact()` | `VIRTUAL_KEY` | Light impact | 10 ms |
| `mediumImpact()` | `KEYBOARD_TAP` | Medium impact | 20 ms |
| `heavyImpact()` | `CONTEXT_CLICK` | Heavy impact | 30 ms |
| `selectionClick()` | `CLOCK_TICK` | Selection changed | 10 ms |
| `successNotification()` | `CONFIRM`, API 30+ | Success notification | 20 ms |
| `warningNotification()` | `KEYBOARD_TAP`, API 30+ | Warning notification | 20 ms |
| `errorNotification()` | `REJECT`, API 30+ | Error notification | 30 ms |

MAUI 10.0.90's Android `LongPress` and `Click` map to `LONG_PRESS` and
`CONTEXT_CLICK`, respectively. These two use `HapticFeedback.Default.Perform`;
the remaining types use Android's view feedback API. Notification feedback is
a no-op before API 30, as in Flutter. View feedback respects system haptic
settings rather than forcing a timed vibrator pulse. The MAUI host assembly
declares `android.permission.VIBRATE`, required by MAUI's permission check, so
consuming applications receive it through Android manifest merging.

iOS default vibration uses `Vibration.Default.Vibrate()`. The other types use
their distinct UIKit generators because MAUI exposes only `Click` and
`LongPress`. Impact feedback on iOS / Mac Catalyst 17.5+ uses the current API
with the actual Doroti surface view. All MAUI feedback runs on the main thread.
Mac Catalyst uses the UIKit path subject to hardware support; Windows and
AppKit MAUI hosts complete without device feedback. Windows App SDK and Qt
retain their existing optional unsupported behavior.

Web uses `navigator.vibrate` with Flutter's duration mapping. A managed Worker
requests feedback from the main thread and awaits its acknowledgement. An
absent Vibration API or a browser returning `false` completes without feedback.
Actual output depends on browser support, user activation and device hardware.

The public framework calls and existing widget/platform/`enableFeedback`
conditions remain unchanged. Unrelated platform messages and inbound handler
registration retain their existing routes. Feedback completion means the host
has dispatched the request, not that a physical vibration has finished.

MAUI implementation references:
[Android haptics](https://github.com/dotnet/maui/blob/10.0.90/src/Essentials/src/HapticFeedback/HapticFeedback.android.cs),
[iOS vibration](https://github.com/dotnet/maui/blob/10.0.90/src/Essentials/src/Vibration/Vibration.ios.cs),
[available MAUI feedback types](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.devices.hapticfeedbacktype?view=net-maui-10.0).

See [regression commands and verification limits](../validation/haptic-feedback/README.md).

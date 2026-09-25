# MAUI Windows / Android keyboard regression

Run from the repository root with the required process-tree deadline:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/maui-keyboard/Contract.csproj -c Release
```

The executable links the product `MauiKeyMap` and `MauiKeyboardState` and references
the shared Windows mapping in `Doroti.Hosting`. It checks layout-independent
positions, keypad and modifier identities, initial logical identity across
repeat/up, focus-loss release, late up suppression, repeat after reacquisition,
Unicode scalars and invalid/dead-key values. These are synthetic managed contracts,
not physical keyboard or IME acceptance.

WindowsAppSDK and both MAUI Windows input paths use the same
`Doroti.Hosting.WindowsKeyboardMap`. MAUI's DXGI input owner now emits the same
HID/logical IDs as the native adapter. `WasKeyDown` and pressed state identify
repeats; the per-message repeat count is not an accumulated hold count. Native
character inspection uses `ToUnicodeEx` flag 4. Window deactivation, element
focus loss and detach release held keys once.

Android uses known Linux evdev scan positions when present and Android keycode
fallback for scan-code-zero events. Android explicitly warns that hardware scan
codes vary between devices; unknown positions remain distinct in a fallback
namespace. The logical identity follows the Android keycode/text. Native view
and window focus loss, handler replacement and disposal release held keys.
`ACTION_MULTIPLE` is passed through rather than turned into a stuck key-down.
The active native EditText still owns hardware/IME text edits.

References: [Windows PhysicalKeyStatus](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.input.physicalkeystatus),
[ToUnicodeEx](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-tounicodeex),
[Android KeyEvent](https://developer.android.com/reference/android/view/KeyEvent).

[2026-09-25 execution results](results-2026-09-25.md)

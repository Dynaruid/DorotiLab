# UIKit text input regression

On an Apple Silicon Mac with the .NET 10 iOS workload and a booted simulator:

```sh
python3 Doroti/validation/uikit-text-input/run.py
```

Use `--simulator <udid>` when more than one simulator is running. The probe uses
its own application ID and compiles the production `MauiTextInputBridge.cs`
with a real MAUI Entry/UITextField handler. Logs and `result.txt` are saved under
`Doroti/artifacts/uikit-text-input`.

It checks that UIKit focus callbacks cannot reenter an unfinished framework
client attach/detach, then checks native typing, client replacement, Korean
marked-text preservation across framework echoes, committed Korean text, and
Backspace. Native operations use public UITextField APIs; this fixture does
not synthesize physical keyboard touches or mount the entire Testbed.

The previous bridge revision fails with
`UIKit focus reentered an unfinished framework client change.` To run that
negative control, export the old source and pass `--bridge-source <file.cs>`.

The product reproduction is Components → Search: tap the search bar and type
without tapping the expanded field again. Synchronous native focus/keyboard
metrics can reenter framework attachment and leave the expanded EditableText
focused but disconnected. Queueing UIKit input mutations lets attachment finish
before native callbacks arrive. Verify immediate typing again after closing
and reopening Search, including with the iOS Release/NativeAOT product.

# UIKit text input regression

On an Apple Silicon Mac with the .NET 10 iOS workload and a booted simulator:

```sh
python3 Doroti/validation/uikit-text-input/run.py
```

Use `--simulator <udid>` when more than one simulator is running. The probe uses
its own application ID and compiles the production `MauiTextInputBridge.cs`
and its UIKit partials with the production Entry/UITextField and Editor/UITextView handlers. Logs and `result.txt` are saved under
`Doroti/artifacts/uikit-text-input`.

It checks that UIKit focus callbacks cannot reenter an unfinished framework
client attach/detach, then checks native typing, client replacement, Korean
marked-text preservation across framework echoes, committed Korean text, and
Backspace. It also checks floating-cursor start/update/end events with positive
and negative X/Y displacement, multiline input, rejection of events after client
detachment, and iOS 16+ native menu presentation, focus, Copy/Cut/Paste and dismissal.
Native selection rectangles must stay empty so UIKit cannot draw a second set
of range handles; caret geometry and the native selection offsets remain intact.
Smart insert/delete is disabled on the native proxy to preserve the framework's
exact text when pasting into multiline fields. Native operations use public UITextField APIs; this fixture does
not synthesize physical keyboard touches or mount the entire Testbed.

Alternate bridge revisions can be tested with `--bridge-source <file.cs>`; they
must expose the UIKit integration members used by the current probe.

The product reproduction is Components → Search: tap the search bar and type
without tapping the expanded field again. Synchronous native focus/keyboard
metrics can reenter framework attachment and leave the expanded EditableText
focused but disconnected. Queueing UIKit input mutations lets attachment finish
before native callbacks arrive. Verify immediate typing again after closing
and reopening Search, including with the iOS Release/NativeAOT product.

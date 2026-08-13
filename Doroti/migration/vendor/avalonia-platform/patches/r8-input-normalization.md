# R8 Win32 input normalization

- `NativeInterop.cs` adds the exact user32 closure for hover tracking, pointer capture, wheel/key modifiers and target-only message injection.
- `SimpleWindow.cs` owns mouse enter/leave, capture release/cancel, focus-loss cancellation and screen-to-client wheel conversion.
- `WindowEventTranslator.cs` normalizes buttons, wheel delta, physical extended scan codes, logical keys, repeat and modifiers without exposing HWND or Avalonia input types.
- Touch and pen stay explicitly unsupported in `InputCapabilities`; this patch does not import Avalonia input devices, automation, dispatcher or visual-tree code.

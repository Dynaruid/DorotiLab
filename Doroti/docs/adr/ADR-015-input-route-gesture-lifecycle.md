# ADR-015: input route and gesture lifecycle

Status: accepted; G5-1 reviewed recognizer cutover implemented.

Win32 owns message normalization, device capability reporting and native capture lifetime. `Doroti.Engine.InputDispatcher` converts backend-neutral raw input to a hit-test path snapshot; it does not expose HWND, native message ids or backend types to `Rendering` or `Widgets`.

A pointer-down freezes the ordered target route and local coordinates for that device until all buttons are up or the route is cancelled. Tree mutation during dispatch therefore cannot retarget the corresponding up/cancel event. Window focus/capture loss, minimize and close terminate every captured route with cancel before disposal.

Reviewed `Doroti.Flutter.Framework.Gestures` source owns arena winner selection, tap/vertical-drag recognition, cancellation and drag thresholds. `RenderGestureDetector` and `RenderSliverFixedExtentViewport` directly hold those reviewed recognizer types. `FlutterArenaAdapter`, `FlutterInputAdapter` and `FlutterPointerEventAdapter` only hand off arena lifetime and convert host-neutral packets, timestamps, device kinds and global-to-local coordinates. The former public `Doroti.Widgets.TapGestureRecognizer` and `VerticalDragGestureRecognizer` compatibility policy types no longer exist.

The Win32 source-port accepts mouse messages and `WM_POINTER` touch/pen packets. Touch capability is reported from the current digitizer state; pen is supported by the packet converter and neither device is silently reclassified as mouse. Capture loss updates the retained packet before releasing Win32 capture so re-entrant `WM_CAPTURECHANGED` cannot emit a second cancel.

`eng/validate-g5-1.ps1` keeps automated native-window evidence separate from physical-device evidence. Its target controller posts packets through an actual HWND and proves timestamp, logical-coordinate, wheel and cancel exactly-once behavior, but this injected source is not labeled as a physical mouse, trackpad or touch run.

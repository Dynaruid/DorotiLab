# H1 Avalonia host bootstrap

> Historical evidence only. The `Doroti.Host.Avalonia` source, sample and runnable verifier were removed after the source-ported A2 host replaced this path.

`Doroti.Host.Avalonia` now owns the official Avalonia application/window shell while keeping every Avalonia type internal. `AvaloniaWindowBackend` implements Doroti's `IWindowBackend` and maps open, activation, resize, minimize/restore, per-monitor scaling and close notifications to `IWindow`, `WindowMetrics`, `IWindowEventSink` and `doroti.avalonia-host-trace/v1`.

The H1 window contains one internal `Control`; it does not mirror the Doroti Widget tree with an Avalonia Control tree. `IAvaloniaDisplayListPresenter` executes Doroti `DisplayList` color, rectangle, path, text, clip and transform commands in that container. Image commands fail with an explicit H2 diagnostic. `Capture` renders the live container to a PNG and returns top-down premultiplied BGRA8888 pixels for exact scene checks.

`samples/AvaloniaHostCounter` runs the same four-command color/geometry scene under strict Windows ANGLE EGL and strict software modes. It resizes, minimizes/restores, moves across distinct-scale displays, captures the scene, checks three solid-color pixels and closes the real window. `eng/verify-h1-avalonia.ps1` runs both modes in isolated processes because Avalonia application setup is process-global. Machine-readable results are written under `artifacts/h1-avalonia`; the reviewed hashes, environment, 1.0/1.25/2.0 scale trace and limitations are pinned in `migration/host/h1-avalonia-evidence.json`.

H1 does not introduce a second frame clock or surface lifetime. DisplayList image/resources, frame generation/ACK integration, present failure and GPU resource ownership are H2. The H1 input/text/cursor objects report no input capability; pointer, keyboard, focus, IME, clipboard and accessibility are H3 and are not claimed by this milestone.

# R8 interactive Counter

R8 closes the first product vertical slice:

```text
Win32 message
  -> RawPointerEvent / RawKeyEvent
  -> immutable hit-test route
  -> gesture arena / tap or keyboard activation
  -> State.setState
  -> BuildOwner
  -> PipelineOwner layout/paint/commit
  -> RasterCompositor present / ACK
```

`InteractiveApplication` coordinates build and render owners on the UI thread and records a versioned raw/route/gesture/build/commit/ACK trace. It accepts only backend-neutral `IWindow`, `Widget` and frame-sink contracts. Its render root is resynchronized after component rebuild, then wrapped in `RenderView` so resize and DPI use logical layout with a device-pixel transform.

The R8 widget slice is `Text`, `ColoredBox`, `Center`, `Padding`, `Row`, `Column`, non-positioned `Stack` and tap-only `GestureDetector`. The Flutter manifest labels these types `api-bound`; it does not claim the full Flutter constructors, styling, focus, semantics or gesture catalog. `Text` reuses the immutable paragraph snapshot from R6.

`Doroti.R8.Tests` fixes capture/cancel/wheel/repeat behavior, rapid-input coalescing, resized/DPI hit targets, pixel-changing state and trace replay. `Doroti.Samples.Counter --verify-target` posts messages through the real HWND queue and writes separate hand-written/generated runtime and trace artifacts. Runtime report v2 also warms one fixed raster thread and then performs five isolated GPU create/present/dispose cycles. `INativeResourceDiagnostics` accounts for HWND and WGL contexts, while `IGpuResourceDiagnostics` accounts for Skia GPU contexts and frames; all active counters and process handle/thread deltas must return to baseline. Machine-local reports remain `not-verified` unless this target run actually occurs.

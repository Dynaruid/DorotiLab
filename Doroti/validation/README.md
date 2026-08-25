# Validation contracts and evidence

This directory contains the small, active validation inputs and committed summaries for Doroti.

- `contracts/product-naming-map.json` is read by the Web template/package validation.
- `contracts/flutter-windows-host-protocol.json` pins the Flutter Windows host source slices, protocol anchors, and Doroti ownership mappings used by the Flutter-style Windows host migration.
- `evidence/app-targets-evidence.json` is written by `eng/validate-app-targets.ps1 -Shard Evidence`.
- `evidence/web/` contains the current Web aggregate and browser-manual summaries written by `eng/validate-web-product.ps1`.
- `evidence/flutter-conformance/framework-parity-matrix.json` pins the FCR-0 Flutter source slice, product/runtime/host closure, asset contracts, and static-risk ownership.
- `evidence/flutter-conformance/baseline-evidence.json` records the current inventory result and target-specific baseline boundaries. Existing submitted/presented counters are not a timing or performance PASS.

Run the compact FCR-0 gate with:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Fcr0
```

The FCR-0 entry point first runs `eng/validate-flutter-windows-host-protocol.ps1`. The same source-only check runs in the `flutter-windows-host-protocol` GitHub Actions workflow against a clean checkout of the exact Flutter revision named by the contract. It intentionally establishes neither a Doroti build nor runtime or visible acceptance.

The staged Flutter-style Windows host gates are deliberately separate from FCR-0:

```powershell
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-bootstrap.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-bootstrap-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-top-level.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-top-level-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-metrics.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-metrics-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-egl-surface.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-egl-surface-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-resize-handshake.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-resize-handshake-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-frame-scheduler.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-frame-scheduler-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-input.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-input-live.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windowsappsdk-flutter-input-manual.ps1 -LaunchAccessibilityInsights
```

The F1 live gate proves the pinned self-contained Windows App SDK/ANGLE/Skia bootstrap and its same-thread lifecycle. The F2 live gate proves a standard raw top-level HWND with exactly one child view HWND, physical client layout, min/max conversion, first-show ordering, and ordered teardown. F2's synthetic first-show callback is intentionally not evidence of a child-window EGL swap, compositor continuity, or visible blank/white-frame absence; those remain F4 and FG acceptance.

The F3 live gate resizes the top-level only so F2's child-fill invariant remains active, then treats the child client rect as the sole physical-size authority. It checks immutable metrics/frame generations, current child DPI/display re-observation, `0×0` suspension/restore, rejected stale or mismatched frame proposals, and a deterministic 100/125/150/200% matrix. It still does not create an EGL window surface or present a visible frame.

The F4 live gate creates an actual ANGLE EGL window surface for that child HWND on a dedicated raster thread, ties its Skia default-framebuffer target to the exact F3 extent, and proves 1,000 recreate/1,001 swap operations plus injected context-loss recovery. Its hardware/software classification and successful swaps are not a visible/compositor acceptance claim.

The F5 live gate makes the F3 metrics-to-F4 exact swap sequence an immutable `ResizeStarted → FrameGenerated → SurfaceReady → Presented → Done` transaction. It keeps the platform wait to engine tasks and 100ms, proves all four fault terminals exactly once, re-observes/re-requests after timeout, and makes `DwmFlush` run on the MTA raster owner only after platform unblock. Its directional/resourcing evidence is still not scheduler cadence or visible compositor acceptance.

The F6 live gate keeps one latest pending frame per view, gives exact resize work priority, and connects callback, dedicated-raster, F4 swap, and Skia receipt through a causal ID. It verifies native `DwmGetCompositionTimingInfo(NULL, ...)` timing separately from deterministic 60/120/144/165Hz scheduler rules; neither is evidence of physical scan-out or visible compositor continuity.

The F7 live gate exercises the actual raw child HWND and typed WndProc route for pointer/capture/focus/cursor, keyboard/dead-key/surrogate handling, IMM32 text/candidate/caret geometry, clipboard transport, and a child-root UIA provider with engine-queued Invoke/Value/Scroll actions. It is intentionally not proof of real-user Korean IME candidate UI, Alt+Tab/minimize/restore/popup behavior, Narrator/Accessibility Insights inspection, or visible FG product acceptance; those remain `notVerified` and keep F8 blocked.

The separate F7 manual gate opens a visible raw-child-HWND fixture and, when requested, Accessibility Insights. It records `PASS` only after the observer directly checks the six on-window pointer/focus/IME/clipboard/Narrator/Accessibility Insights/resize-DPI items, toggles F1-F6, and explicitly finishes with F8. Escape, close, timeout, an incomplete checklist, or a mismatched source fingerprint records or validates as `notVerified`; it cannot promote the automated live result. The fixture includes a minimal framework-side selection/copy/cut/paste editor so physical clipboard behavior is exercised without claiming F9 product integration. FG visible/compositor acceptance remains separate.

The gate fails on Flutter source hash drift, missing product/runtime/host/evidence ownership, missing shader/font/data contracts, and unclassified static candidates. It records `implemented`, `adapted`, `explicitUnsupported`, and `notVerified` separately; later differential, native live, physical, and performance gates remain explicit.

Machine-local traces and generated build output belong under `.doroti/` or `artifacts/`. Historical migration inputs and milestone evidence were removed from the active tree; older summaries remain under the repository `history/` archive.

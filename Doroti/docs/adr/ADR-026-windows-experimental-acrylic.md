# ADR-026: Opt-in Windows experimental Acrylic Composition Swapchain

- Status: Experimental
- Date: 2026-09-01 (resize-path amendment 2026-09-02)

## Decision

The opaque Windows App SDK `HwndExactCpp`/ANGLE topology in ADR-025 remains the default. A new window may explicitly request `WindowBackdropMode.experimentalAcrylic` on Windows 11 24H2 build 26100 or newer. The stable `acrylic` and `system` modes do not select this path.

Mode selection happens before the native window is created. Capability, hardware-adapter, Presentation API, ContentIsland, Acrylic controller, or initial surface failure falls back to opaque before show and records requested/effective mode plus the reason. Once shown, a window does not switch between the opaque child HWND and ContentIsland topology.

## Ownership and presentation

The experimental visible owner is one `DesktopChildSiteBridge` with one ContentIsland, one content visual, and `DesktopAcrylicController`. `ResizeContentToParentWindow` is the single child-site geometry owner. `OverrideScale=1` keeps the already-physical Presentation surface in a 1:1 Composition coordinate space while framework layout uses the separately published DPI. The native top HWND retains standard shell, non-client, pointer, keyboard, focus, IME, UIA, and physical client-geometry ownership. The hidden child HWND is only the pre-show opaque fallback endpoint and never contributes pixels while experimental Acrylic is effective.

ANGLE exposes its hardware D3D11 device. The native presentation bridge creates `IPresentationFactory`, manager, surface handle, and at most three capacity-bucketed BGRA8 premultiplied Presentation buffers from that same device. Managed EGL imports each texture directly into an inset transparent guard; Skia renders only the 1:1 inner viewport. A successful callback atomically selects the buffer, source rectangle, identity transform, and present under one native lock. Presentation retirement never waits; an exhausted three-slot set may wait up to 17 ms only on Doroti's raster worker. CPU readback/upload, GDI copies, staging round trips, WARP, a fourth slot, and unavailable-buffer reuse are forbidden. Render scheduling remains current one plus latest pending one; every accepted resize generation ends exactly once as presented, superseded, or failed.

Self-contained undocked RegFree WinRT activation and the ANGLE DLL are resolved from the application directory before applying the process-wide restricted default DLL search. Applying `SetDefaultDllDirectories` earlier breaks the ContentIsland class-factory lookup. No machine-wide Windows App Runtime bootstrap is introduced by this mode.

## Resize contract

Idle, initial, and settled programmatic geometry is exact. The native host does not alter or step the shell's `WM_SIZING` rectangle. Top-level `WM_SIZE` is the physical metrics authority for experimental Composition: it publishes the ContentIsland viewport and latest render request, then returns without waiting. `WM_EXITSIZEMOVE` republishes the latest exact request asynchronously; its successful raster terminal may perform one final `DwmFlush`. Same-generation requests without a new framework scene do not replay an old GPU frame. The opaque host cadence is unchanged. Only the active edge may temporarily differ, by at most:

`min(12 physical px, ceil(6 logical px × rasterizationScale))`

Inactive edges may differ by at most one physical pixel. The three-second geometry profile requires the stationary opposite-edge markers. Both geometry and responsiveness profiles retain the 12-physical-pixel active-edge hard gate; responsiveness additionally requires at least 40 fps with a 0.5-fps finite-sample tolerance, at most 26 physical pixels of cursor-to-active-edge lag, and exact settle within both nine refresh intervals and 50 ms. Per-frame interactive `DwmFlush` is forbidden. Buffer-availability and DWM waits never run in WndProc. `Stretch=None`, stationary-edge alignment, and 1:1 transparent-guard crop may clip or reveal Acrylic at the active edge but may not scale the retained raster. Full blank/black/white/raw-desktop frames, previous-generation full frames, stale input coordinates, or a platform-thread wait on fences, buffer events, commits, or `DwmFlush` fail the experimental contract.

## Runtime options and diagnostics

`doroti/windows/experimental-acrylic` accepts Default/Base/Thin, system/light/dark theme, tint color, tint opacity, and luminosity opacity updates. Updates use one current apply plus one latest pending request; each accepted revision receives one applied, superseded, or failed terminal, and the last request wins. They do not create resize generations or rebuild the ContentIsland, controller, visual tree, or HWND topology.

Diagnostics expose requested/effective mode, fallback reason, capability, adapter LUID/vendor/device, current options, explicit geometry budgets, slot/reuse counters, and option terminals. `Doroti/validation/contracts/windows-experimental-acrylic.json` is the machine-readable contract; `Doroti/eng/validate-windows-experimental-acrylic.ps1` records partial or full-current-DPI automation without reclassifying the earlier strict P1-CS failure.

## Evidence boundary and consequences

The P0.5 and strict P1-CS results remain failures under their original contracts. A bounded experimental run can pass only its listed environment and cases. Automated capture does not prove physical scan-out, subjective border-drag quality, monitor/DPI crossing, Korean IME candidate/caret behavior, Narrator, Accessibility Insights, policy/RDP behavior, or unexecuted device/window-management combinations; those remain `notVerified`.

The 2026-09-02 repair capture on build 26200 at 200% DPI/165 Hz is a partial checkpoint, not a full pass. TopLeft three-second reverse reached 9/0 physical pixels on active/inactive edges, decoded approximately all interactive frame IDs, and settled in 29.16 ms. Geometry-aware visual attribution excludes only expected top-edge shrink crops and then reports zero app-bar/title failures, 4/8 px maximum gaps, and 0/0 final gaps. The 600 ms case remains `FAIL`: 37 px maximum active-edge delta, about 74% matched coverage, 38.17 accepted fps, one uncropped app-bar failure, six title-oracle failures, and 23/13 px gaps. Release/ABI, empty-PATH, fallback, option burst, and opaque-before/after gates passed in `.doroti/evidence/experimental-acrylic-wrapup-20260902`; visible cases were deliberately not rerun in that wrap-up manifest.

Physical scan-out of this revision remains `notVerified`. The earlier strict P0.5/P1-CS failures remain failures, the full matrix and three consecutive qualification runs were not executed, and this ADR does not promote the experimental route while the fast case fails.

Promotion to stable `acrylic`, changing the opaque default, or broad public support requires three complete automatic qualification runs plus physical acceptance and is a separate decision.

# ADR-026: Opt-in Windows experimental Acrylic Composition Swapchain

- Status: Experimental
- Date: 2026-09-01

## Decision

The opaque Windows App SDK `HwndExactCpp`/ANGLE topology in ADR-025 remains the default. A new window may explicitly request `WindowBackdropMode.experimentalAcrylic` on Windows 11 24H2 build 26100 or newer. The stable `acrylic` and `system` modes do not select this path.

Mode selection happens before the native window is created. Capability, hardware-adapter, Presentation API, ContentIsland, Acrylic controller, or initial surface failure falls back to opaque before show and records requested/effective mode plus the reason. Once shown, a window does not switch between the opaque child HWND and ContentIsland topology.

## Ownership and presentation

The experimental visible owner is one `DesktopChildSiteBridge` with one ContentIsland, retained-background/content visuals, and `DesktopAcrylicController`. `OverrideScale=1` keeps the already-physical Presentation surface in a 1:1 Composition coordinate space while framework layout uses the separately published DPI. The native top HWND retains standard shell, non-client, pointer, keyboard, focus, IME, UIA, and physical client-geometry ownership. The hidden child HWND is only the pre-show opaque fallback endpoint and never contributes pixels while experimental Acrylic is effective.

ANGLE exposes its hardware D3D11 device. The native presentation bridge creates `IPresentationFactory`, manager, surface handle, and at most three 256 px capacity-bucketed BGRA8 premultiplied Presentation buffers from that same device; a per-present source rectangle selects the current exact extent. Managed EGL imports each D3D11 texture directly, Skia renders into it, and the bridge presents it to a Composition surface brush. The first three size-changing presents may wait up to 100 ms for prior-present retirement on Doroti's raster worker, and an exhausted three-slot set may wait up to 17 ms for a buffer-availability event there. Neither wait runs on the platform/WndProc thread. CPU readback/upload, GDI copies, staging round trips, WARP, a fourth slot, and unavailable-buffer reuse are forbidden. Render scheduling remains current one plus latest pending one; every accepted resize generation ends exactly once as presented, superseded, or failed.

Self-contained undocked RegFree WinRT activation and the ANGLE DLL are resolved from the application directory before applying the process-wide restricted default DLL search. Applying `SetDefaultDllDirectories` earlier breaks the ContentIsland class-factory lookup. No machine-wide Windows App Runtime bootstrap is introduced by this mode.

## Resize contract

Idle, initial, and settled programmatic geometry is exact. The native host does not alter or step the shell's `WM_SIZING` rectangle. Top-level `WM_SIZE` immediately sizes the ContentIsland child and hidden metrics child from the actual physical client extent; the hidden child's `WM_SIZE` remains the sole metrics authority. It publishes a generation and queues its exact frame. During interactive Composition resize the platform wait yields after 16 ms while the raster terminal continues; `WM_EXITSIZEMOVE` queues the latest exact frame and retains the 100 ms terminal bound. Same-generation requests without a new framework scene do not replay an old GPU frame. The opaque host cadence is unchanged. Only the active edge may temporarily differ, by at most:

`min(12 physical px, ceil(6 logical px × rasterizationScale))`

Inactive edges may differ by at most one physical pixel. The three-second geometry profile requires the stationary opposite-edge markers. The 600 ms responsiveness profile requires at least 40 fps with a 0.5-fps finite-sample tolerance, at most 26 physical pixels of cursor-to-active-edge lag, and exact settle within both nine refresh intervals and 50 ms; transient opposite-marker loss is diagnostic rather than a gate there. Per-frame interactive `DwmFlush` is forbidden because it serializes raster throughput; the raster worker flushes only exit/non-interactive exact presents. Buffer-availability and DWM waits never run in WndProc. Direction-dependent brush alignment and content offsets remain forbidden. A 256-physical-pixel retained background plus top-HWND erase fill prevents the parent from exposing white pixels while ContentIsland scan-out catches up during fast growth. Full blank/white/raw-desktop frames, previous-generation full frames, stale input coordinates, or a platform-thread wait on fences, buffer events, commits, or `DwmFlush` fail the experimental contract.

## Runtime options and diagnostics

`doroti/windows/experimental-acrylic` accepts Default/Base/Thin, system/light/dark theme, tint color, tint opacity, and luminosity opacity updates. Updates use one current apply plus one latest pending request; each accepted revision receives one applied, superseded, or failed terminal, and the last request wins. They do not create resize generations or rebuild the ContentIsland, controller, visual tree, or HWND topology.

Diagnostics expose requested/effective mode, fallback reason, capability, adapter LUID/vendor/device, current options, explicit geometry budgets, slot/reuse counters, and option terminals. `Doroti/validation/contracts/windows-experimental-acrylic.json` is the machine-readable contract; `Doroti/eng/validate-windows-experimental-acrylic.ps1` records partial or full-current-DPI automation without reclassifying the earlier strict P1-CS failure.

## Evidence boundary and consequences

The P0.5 and strict P1-CS results remain failures under their original contracts. A bounded experimental run can pass only its listed environment and cases. Automated capture does not prove physical scan-out, subjective border-drag quality, monitor/DPI crossing, Korean IME candidate/caret behavior, Narrator, Accessibility Insights, policy/RDP behavior, or unexecuted device/window-management combinations; those remain `notVerified`.

The current product-path run on build 26200 at 200% DPI/165 Hz reports `PASS-automated-partial`. TopLeft three-second reverse matched 334 frames, sustained 43.99 outer changes/43.66 accepted frames per second, reached 7/0 physical pixels on active/inactive edges, missed the stationary right/bottom marker zero times, and settled in 31.74 ms/5.24 refresh intervals. Its 600 ms responsiveness case matched 21 frames, sustained 44.95/43.28 fps, reached 20 px maximum cursor lag, and settled in 44.09 ms/7.27 intervals. The fast-profile active/inactive diagnostic was 118/0 px; active-edge geometry and one opposite-marker miss are diagnostic rather than hard gates for this finite responsiveness sample. ABI, empty-PATH, fallback, option burst, resource, capture, and opaque-before/after gates passed.

Automated sampled captures contain no exposed white band after the physical-coordinate, retained overscan, and top-HWND erase-fill repairs. Physical scan-out of this revision remains `notVerified`. The earlier strict P0.5/P1-CS failures remain failures, the full matrix and three consecutive qualification runs were not executed, and this ADR records an experimental partial pass rather than stable qualification.

Promotion to stable `acrylic`, changing the opaque default, or broad public support requires three complete automatic qualification runs plus physical acceptance and is a separate decision.

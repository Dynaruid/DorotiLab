# ADR-008: Avalonia platform-engine vendor slice

- Status: Superseded by ADR-016
- Date: 2026-08-01

## Decision

Doroti selects individual Avalonia Win32/Skia files or symbols plus the minimum proven dependency closure. Each entry is classified `copy`, `adapt`, `rewrite` or `exclude`. `Avalonia.Application`, locator, property system, `Control`, `Visual`, layout, styling, XAML, Avalonia render loop and composition are excluded.

Derived code is internal in `Doroti.Vendor.Avalonia.Win32` or `.Skia`. Each vendor assembly grants `InternalsVisibleTo` only to its matching backend adapter. R4 selects the minimal interop, `SimpleWindow`, window-message, DIB framebuffer and Skia framebuffer symbols. Full `WindowImpl` and DXGI are excluded because their closure crosses the forbidden UI/composition or EGL/ANGLE/MicroCom boundaries.

## Consequences

Vendor projects reference no framework layer or Avalonia package. The Skia vendor slice references only the pinned SkiaSharp runtime; Skia types remain internal. Backends translate internal vendor shapes into `Doroti.Platform`, `Doroti.Graphics` and `Doroti.Composition` contracts. Upstream updates produce a review bundle and never overwrite local adaptations.

ADR-016 initially retained this slice as a migration baseline, then the 2026-08-02 host-only follow-up removed `Doroti.Backends.Win32` and `Doroti.Vendor.Avalonia.Win32` from the product, test, sample, template and release graphs. Historical R4–R9 traces remain evidence of the former implementation, not an active fallback. Only the backend-neutral Skia framebuffer adaptation remains for headless and host-adapter raster tests.

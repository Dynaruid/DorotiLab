# Material sample reference parity repair — 2026-09-07

Reference: `reference/flutter_sample_app`, including its current image demo. Scope: sample appearance/configuration, font loading/fallback, wide layout and elevation shadows.

## Changes

- Matched component spacing, labels/tooltips, dialogs, the six-action modal/persistent bottom sheets, navigation drawer, text fields, progress indicators and image-demo arrangement to the pinned Dart source.
- Matched wide navigation settings placement, seed/image grids, thumbnail shape/elevation and suppression of the unintended automatic drawer action. Preserved responsive breakpoints and independently scrolling component columns.
- Fixed native/legacy Skia font resolution: a missing system family now tries explicit registered fallback families before the platform default. Fallback families participate in the text-resource cache key. The sample registers only regular Roboto on Web, matching Flutter Web's fallback registration, and keeps native weight faces.
- Fixed the shared Divider default getter override so the default divider uses `outlineVariant`.
- Fixed native rounded paths to preserve all four corner radii. Replaced arbitrary shadow passes with directional ambient/spot parameters and Skia tonal-color/blur-fallback equations. Source: [SkShadowUtils.cpp](https://github.com/google/skia/blob/main/src/utils/SkShadowUtils.cpp). CanvasKit already uses Skia's directional shadow implementation.
- Fixed shared `AnimatedModalBarrier` forwarding of a nullable `onDismiss`. Wrapping a missing callback in a non-null lambda previously caused a NullReferenceException instead of falling through to Navigator dismissal.
- Image rendering and explicit palette extraction share the original provider, as in the reference. Mounted tests verify one decode and no repeated extraction/readback while scrolling.

## Verification

All test entrypoints retain the repository's 20-minute timeout.

| Check | Result | Evidence |
|---|---|---|
| CanvasKit Web sample screens, responsive breakpoints 390–1600, light/dark, menus, selection, text input, dialogs, both sheets, lazy inventory, retained state, image themes, DPR 2 sheet dismissal | PASS, 8 tests | `Doroti/validation/web-playwright/artifacts/wrapper/sample-reference-parity-v6/` |
| Image network failure/retry, superseded response, brightness | PASS in v5; unchanged by final barrier/capture repair | `Doroti/validation/web-playwright/artifacts/wrapper/sample-reference-parity-v5/` |
| Separately built pinned Flutter reference captures | PASS, 1 test | `Doroti/validation/web-playwright/artifacts/sample-reference-flutter-v1/` |
| Native font fallback, Material defaults, six elevation levels, asymmetric corners, mounted pickers, drawer bounds, image work, app-bar scroll lifecycle | PASS | `.doroti/evidence/sample-reference-native-v5/` |
| Windows Release app build | PASS, 0 warnings/errors | `dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release --no-restore -v quiet` |

Reproduction:

```powershell
& Doroti/eng/run-web-playwright.ps1 -HeadlessOnly -RendererMode worker-canvaskit-webgl -Port 5093 -TestFile @('tests/material-sample.spec.ts','tests/material-sample-selection.spec.ts') -ArtifactLabel sample-reference-parity-v6
pwsh -NoProfile -File Doroti/eng/test-material-sample.ps1 -Suite Windows -OutputDirectory .doroti/evidence/sample-reference-native-v5
```

The wrapper's `auto` choice exercises a different, legacy renderer. CanvasKit is explicitly selected for the reference comparison.

## Screenshot comparison and limits

Both apps were captured in the same Chromium environment at 800×900 and 1600×900, DPR 1. Comparison excludes app branding, navigation chrome and scrollbar: crop `(0,56,788,820)` at width 800, `(256,56,1588,900)` at width 1600. Coordinates use exclusive right/bottom bounds. This measures the visible content, not every scrolled component or physical display output.

| Screen | Mean absolute RGB difference, 800 / 1600 (0–255) | Pixels with any channel difference >16, 800 / 1600 |
|---|---|---|
| Components | 0.4465 / 0.4065 | 0.8237% / 0.7534% |
| Color | 0.0013 / 0.0003 | 0% / 0% |
| Typography | 0 / 0 | 0% / 0% |
| Elevation | 0 / 0 | 0% / 0% |

Raw measurements: `.doroti/evidence/sample-reference-comparison-v6.json`. Raw captures are under the corresponding Playwright artifact directories. Typography and Elevation content are pixel-identical in these captures. Components retain small button/text/edge differences; whole-screen or all-component pixel identity is not claimed.

Native shadow rendering uses a blur fallback because the available SkiaSharp API does not expose SkShadowUtils tessellation. The native regression establishes zero-elevation behavior, visible positive elevations, opaque occlusion and corner geometry, not exact Flutter shadow pixels. Native physical display, all host platforms, and complete scrolled/dark-screen pixel parity remain `notVerified`.

## Preserved failures

- Earlier Web `auto` captures exposed the legacy missing-family fallback problem.
- v3 contained temporary console diagnostics and intentionally failed the runtime-error gate; the probe was removed.
- v4/v5 failed sheet dismissal; v5 captured the actual AnimatedModalBarrier null callback exception. Final v6 passes both DPR 1 and DPR 2 outside-tap dismissal.
- v5's wide Components capture caught the previous Elevation frame. v6 waits for selected semantics and a newly presented frame before capture. The invalid v5 comparison is preserved separately and is not used in the table above.
- Early native fixtures assumed two image decodes and a mounted resize target for offscreen pictures. Updated implementation/expectations use one shared original image decode and a scoped logical-pixel shadow scale for offscreen rendering. Original logs remain in earlier evidence directories.

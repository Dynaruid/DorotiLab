# Material sample integration validation

The testbed sample is an explicit C# application mode. The default remains diagnostics
until the Windows/Web P1-P5 acceptance gate in [work.md](../../../work.md) passes.
The local reference is `reference/flutter_sample_app`, Flutter SDK revision
`6b182d2c7585eba26d4edce0f97630effd256c33`; its immutable P0 and WebP manifests remain
under `validation/fcr7-material-widget/baselines/` and `.doroti/evidence/material-image-repair/`.
No new screenshot is a replacement for a historical failure.

```powershell
./Doroti/eng/test-material-sample.ps1 -Suite Blockers
./Doroti/eng/test-material-sample.ps1 -Suite Regression
./Doroti/eng/run-web-playwright.ps1 -HeadlessOnly -RendererMode worker-canvaskit-webgl `
  -TestFile @('tests/material-sample.spec.ts','tests/material-sample-selection.spec.ts',
    'tests/material-sample-network.spec.ts','tests/material-sample-boundaries.spec.ts') `
  -ArtifactLabel material-sample-current -Port 5096
# HeadlessOnly includes both DPR 1 and the explicitly tagged DPR 2 pointer scenario.
# Separate shared semantics and resize reversal checks:
./Doroti/eng/run-web-playwright.ps1 -HeadlessOnly -RendererMode worker-canvaskit-webgl `
  -TestFile @('tests/semantics-radio-state.spec.ts','tests/material-sample-motion.spec.ts') `
  -ArtifactLabel material-sample-contracts-current -Port 5096
```

The wrappers enforce the repository 20-minute process timeout. Choose a fresh artifact
label/output directory for each run. Browser screenshots use Chromium hardware WebGL2,
DPR 1, logical viewport dimensions in the filenames, initial light system theme,
embedded MaterialIcons and Roboto regular/medium/bold fallback, retaining the platform's
primary family. NanumGothic remains available for Korean fallback. The diagnostics bundle records the
actual renderer and frame generations. Pointer tests click projected semantic bounds;
they also catch coordinate/hit-test mismatch. Semantics-only clicks are identified in
older evidence. A presented frame must have visible content and zero Raster failedScenes;
DOM semantics alone does not prove successful painting. Progress starts stopped at 0.7;
Play enables indeterminate animation. Presented frame generations, rather than global
queue-idle, determine sample frame completion.

## Shared API contracts

- `UrlLauncher.launchUrl`: view-scoped absolute HTTP(S) without credentials, returns
  `opened`, `blocked`, `unsupported`, `invalidUrl`, or `failed`. `opened` means host
  request acceptance. Windows uses shell activation; Web dispatches to main-thread
  `window.open`; MAUI uses Launcher; Linux Qt uses bounded `xdg-open`. Web popup blocking
  is a result surfaced by the sample. Native browser navigation is a separate live check.
- `Dart_uiLibrary.loadFontFromList`: respects byte-view offset/length and optional family;
  completion waits for registration in the host's text/raster backend. CanvasKit waits
  for retained resource acknowledgment. Successful registration emits `fontsChange` to
  the mounted framework. Hosts own font bytes/typefaces until disposal. DrawParagraph
  forwards the same registered font collection used for UI layout; metrics validation
  remains enforced.
- Semantics builder nodes retain local bounds and the parent coordinate transform.
  Projection composes transforms instead of treating clipped bounds as a new coordinate
  origin. Direct legacy nodes without transforms retain their rect-relative convention.
  The view supplies its DPR; projection removes the framework's physical root scale once.
  Direct legacy nodes remain logical and are not divided by DPR. FCR-6 covers clipped
  ancestor origins, scaling, DPR and geometry-only invalidation. The headed TextField
  pixel/caret regression and sample DPR 2 pointer scenario exercise live projection.
- Optional picker/menu callbacks remain null; nullable restoration values remain nullable.
  Typed routes share status queries without casting their result type. Menu overlay builders
  execute during layout, mixed menu anchor types retain parent/child relationships, and
  zero-duration AnimationController operations return an already completed TickerFuture.

## Evidence and limits

Current run results, historical failures, and per-target build/live status are recorded
in [work.md section 9](../../../work.md#9-2026-09-06-전체-작업-후속-구현-진행-중).
Evidence root: `.doroti/evidence/material-sample-implementation/`; browser wrappers:
`Doroti/validation/web-playwright/artifacts/wrapper/material-sample-implementation-v*/`.
Browser screenshots and traces are in the corresponding non-`wrapper` artifact folder.

`material-sample-final-v27` passed all 13 current integration scenarios in 6.2 minutes,
including nested menus, state changes, DPR 2, light/dark captures, image errors/retry,
resize reversal and the independent radio semantics fixture. Earlier suite-v16 passed
nine original scenarios; the later failures and repairs remain in the work table.
`material-sample-diagnostics-v1` passed six legacy input/resize/DisplayList checks. Its image
pipeline test was skipped because that suite was not built with the dedicated image-validation
entrypoint; it does not replace the successful image differential evidence in work section 8.
The headed TextField test was excluded by the headless project, then run independently:
`material-sample-text-regression-v1` failed with DPR-scaled semantics bounds; v2 passed after
the view-coordinate repair. These failures remain available.

The separate Flutter release build uses its frozen SDK and main.dart. Set
`DOROTI_FLUTTER_REFERENCE=1` only while serving that reference and running
`tests/flutter-material-sample.spec.ts`. `flutter-material-sample-v4` captures four light
screens at 800/1600 and four dark screens at 800, all height 900 and DPR 1. These are
comparison inputs, not a pixel parity certificate. Web now uses CanvasKit's actual Skia
drawShadow with the pinned Flutter engine's light/tonal-color/DPR contract. Native
SkiaSharp still uses the two-blur approximation and has a separate parity gap.
`material-sample-capture-v26` confirms all four light/dark destination captures with
selection and presentation waits. `visual-comparison-v1.json` contains a stale Components
frame mislabeled as dark Color; preserve it as invalid comparison evidence, not a product
color mismatch. The corrected measurement is `visual-comparison-v2.json`.
Reproduce the measurement with `python compare_screens.py <sample-test-output-dir>
<reference-test-output-dir> <new-output.json>` (Pillow and numpy required). The script
checks capture dimensions and refuses to overwrite earlier evidence. It reports RGB
differences for the body crop without assigning an acceptance threshold.

Windows startup evidence is an automated Vulkan smoke, not full physical-input or
scan-out acceptance. Linux managed cross-build excludes a native Qt build. Android
Release packaging is not a device test. macOS/AppKit, Mac Catalyst and iOS require an
Apple runner. Korean IME, screen readers, native URL applications, physical resize and
Flutter pixel parity must be recorded separately from automated smoke/contract results.

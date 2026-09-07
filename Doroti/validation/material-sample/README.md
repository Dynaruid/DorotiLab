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
./Doroti/eng/test-material-sample.ps1 -Suite Windows
./Doroti/eng/test-material-sample.ps1 -Suite AppBarRaster
./Doroti/eng/test-material-sample.ps1 -Suite Scroll
./Doroti/eng/test-material-sample.ps1 -Suite Selection
./Doroti/eng/run-web-playwright.ps1 -HeadlessOnly -RendererMode worker-direct-webgl `
  -TestFile @('tests/material-sample.spec.ts','tests/material-sample-selection.spec.ts',
    'tests/material-sample-network.spec.ts','tests/material-sample-boundaries.spec.ts') `
  -ArtifactLabel material-sample-current -Port 5096
# HeadlessOnly includes both DPR 1 and the explicitly tagged DPR 2 pointer scenario.
# Separate shared semantics and resize reversal checks:
./Doroti/eng/run-web-playwright.ps1 -HeadlessOnly -RendererMode worker-direct-webgl `
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

The `Selection` suite mounts Slider, RangeSlider and icon Switch controls. It checks
six distinct, evenly spaced ticks for five divisions, intermediate value snapping,
continuous mode, pointer input and disabled controls in light/dark and LTR/RTL.
It verifies MaterialIcons' natural 16px em box/baseline and explicit/inherited
paragraph heights, and saves native raster PNGs. This is framework input/native
raster evidence; the actual Web sample needs a separate browser check.
The suite also protects synchronous value continuations through typed and untyped
Future references and chained results, while ordinary Future callbacks remain queued.
This covers the default localization path used when the Web sample first opens.

The `Windows` suite exercises native Skia paragraph pixels and ready-to-paint image
decoding, independent rail callbacks, and every hour/minute dial conversion. It also
mounts Material date/time dialogs against an offscreen host, injects framework pointer
down/up events at rendered targets, and verifies date selection/confirmation/cancellation
and 24-hour hour/minute selection/confirmation. Native PNGs are written under the run's
output directory. This covers framework hit testing and native raster, not Win32 input,
GPU presentation, physical display acceptance, or measured scrolling cadence.

The suite also mounts the actual sample sections and SampleHome. It checks drawer
indicator/InkWell geometry and pointer selection, scrolls the inline Top app bars
through the viewport in both directions, and exercises both outer columns. A synthetic
forwarded depth-zero metrics notification from each inner app bar verifies the sample's
scroll-source filter, including the root Material color and a raster background pixel.
This is a source-isolation contract, not a reproduction of the user's original flicker.

The Windows-only `AppBarRaster` suite uses an offscreen D3D12 Skia context to exercise
the shared native picture cache. It mounts the dark sample at 1275x640 and moves both
columns through 70 positions, checking six intermediate frames per position and three
GPU replays per scene. The uncached CPU scene is the reference. The root toolbar check
includes text and actions; inline bars compare solid interiors to exclude fractional
GPU sampling versus CPU glyph antialiasing differences. The fixture requires cache
pressure beyond 24 admissions and cache hits. Before the cache eviction repair, the
root toolbar lost all 71,400 pixels at step 19. This is a raster regression, separate
from Vulkan presentation and physical display acceptance. See work.md section 15.

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

The finite Components gallery lays out all 29 sections before its first scroll.
Each section has a separate box sliver and repaint boundary, so offscreen sections
do not paint and the gallery is not one oversized raster-cache entry. This deliberately
replaces the reference sample's measured-height sum (unvisited sections counted as zero).
The initial frame does more work; scroll extent no longer grows just by visiting content.
Actual content changes, width and text scaling still update the extent through layout.
The `Scroll` suite checks wheel input, stable first-traversal extent, direct access to the
end, section state retention and width/text-scale/column transitions. It does not certify
physical-device frame cadence or startup latency.

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

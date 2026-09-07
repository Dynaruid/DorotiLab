# Material sample first-scroll extent repair

Date: 2026-09-07

## Change

The user reported stutter and an expanding scroll range while first exploring the
Components gallery. `MeasuredSlivers` counted every unmeasured section as zero.
The original Flutter sample uses this policy too; it is not the framework's default
average-height extrapolation. Doroti also deferred construction per component section.

The finite gallery now uses one `SliverToBoxAdapter` and `RepaintBoundary` per section.
All 29 sections are mounted and measured before scrolling, so visiting a section does
not create its component subtree or extend the scroll range. The viewport still culls
offscreen painting. Per-section state/build ownership and independent column controllers
are retained. Width, text scale and actual content changes go through normal layout;
there is no stale height cache or fixed-height clipping. `CachedSlivers.cs` was removed.

This deliberately trades additional initial mount/layout work and resident elements
for predictable scrolling in this small, heterogeneous gallery. It does not change
the shared framework's lazy lists or claim that all lazy lists should become eager.
First-time raster work can still occur when a section becomes visible. Startup latency,
physical scroll cadence and memory impact were not quantitatively qualified.

## Evidence

All test wrappers enforce the repository's 20-minute timeout. Evidence is preserved
under `.doroti/evidence/sample-scroll-20260907/`.

- `before`, `before-v2`, `before-v3`: fixture setup failures (delegate type, ambiguous
  Action type, then missing explicit locale/platform setup); not product regressions.
- `before-v4`: original implementation FAIL. First traversal changed maxScrollExtent
  from **381.88 to 513.88** at width 800, text scale 1.
- `after`: fixture lacked clipboard capability, newly exercised by initially mounted
  text inputs. Registered the same fixture host as the existing Windows suite.
- `after-v2`: stable extent/state, direct end, width/text-scale/column transitions PASS.
- `scroll-final`: added framework wheel input; all checks PASS. All 29 section states
  retain identity during first down/up traversal. Stable extents: 6736.58 (800/1),
  7145.53 (600/1.25), 1819.98 and 4305.74 (two columns), 6736.58 after returning.
  The wide fixture sets responsive MediaQuery width to 1280 within an 800px render
  surface; it validates column behavior, not physical 1280px display geometry.
- `windows`: existing fixture assumed inline sections were absent at first scroll.
  It read `full-sample-scroll-0.png` before creating it once all sections mounted early.
  Moved baseline capture ahead of forwarded-metrics checks; retained the pixel assertion.
- `windows-final`: mounted Windows sample regression suite PASS, including picker,
  drawer, image, navigation and full SampleHome down/up app-bar lifecycle checks.
- `appbar-raster`: 420 intermediate GPU scrolling frames PASS against CPU app-bar
  regions, including two columns and raster cache reuse. This is offscreen GPU evidence.
- Windows App SDK and Web Release builds: PASS, zero warnings/errors. Web was rebuilt
  after the comparison experiment restored the modified sources.

## Web limitation

`Doroti/validation/web-playwright/artifacts/wrapper/sample-scroll-20260907/`:
the five Material sample scenarios failed at initial content/heading checks; the
surface was blank and the semantics tree contained only two grouping nodes.

To check causality, restored only Components.cs and CachedSlivers.cs from HEAD,
rebuilt and ran the first scenario on the same renderer/port. That unmodified sample
also failed to show the heading and produced the empty grouping tree. The comparison
is preserved under `sample-scroll-20260907-baseline`; the modified source was restored
in `finally`. The initial Web failure is therefore also reproducible without this
scroll change. Its underlying cause remains unresolved. Web interaction and physical
UX acceptance remain **notVerified**; build and native/GPU checks do not replace them.

## Reproduce native regression

```powershell
./Doroti/eng/test-material-sample.ps1 -Suite Scroll
./Doroti/eng/test-material-sample.ps1 -Suite Windows
./Doroti/eng/test-material-sample.ps1 -Suite AppBarRaster
```

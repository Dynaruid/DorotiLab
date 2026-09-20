# Android WebView floating panel motion

Device: Galaxy S25, Android 16/API 36, arm64, WebView 151.0.7922.199.
Linux validation was explicitly skipped by the user. Evidence is stored in
`../../artifacts/webview/2026-09-20/panel-motion/` (gitignored).

## Findings and changes

- Panel movement no longer rebuilds the complete WebView sample. A notifier updates
  the transform, with retained foreground pictures. The outer hit-test boundary
  covers the complete travel area, so dragging again works after moving downward.
- Raster transfer copies directly into a new bitmap, retains Android View identity,
  packs readback slices in two dimensions, and reuses compatible translations.
  Cached content is invalidated for changed pictures, clipping, effects, size or
  GPU epoch. No displayed bitmap is modified in place.
- The base raster used to sample the blur is source-only. Displaying it a second
  time had covered the bottom navigation bar. Unchanged sources can retain bounded
  viewport coverage instead of recapturing the moving blur sample every frame.
  Proven opaque native coverage may be excluded; unknown opacity is conservative.
- Blur previously called `Draw()` on the native WebView hierarchy again with a
  moving sampling viewport. Native content is now recorded during its normal
  traversal. While sampled, a shared GPU compositing layer supplies both the
  ordinary view and blur. This preserves the existing WebView/controller and
  RenderEffect, without native content CPU readback. The extra layer is disabled
  when it is no longer sampled.
- Opt-in input timing separates event age at delivery from synchronous gesture
  dispatch cost. Neither automated input nor native-window gfxinfo alone measures
  panel input-to-display latency.
- `foreground-paint-trace.txt` traced repeated foreground invalidation to the
  disabled stretch `ImageFiltered` widget. Inactive stretch now avoids creating a
  shader, and disabled `ImageFiltered` changes retain the new filter without
  invalidating paint. Enabling still invalidates and applies the latest filter.
  Three render-object lifecycle checks cover this transition. `PlatformEffect`
  retains foreground content separately from its moving backdrop filter.

The shared native display-list approach is also used by
[BlurView's BlurTarget](https://github.com/Dimezis/BlurView/blob/master/library/src/main/java/eightbitlab/com/blurview/BlurTarget.java).
The forced intermediate buffer is defined by
[RenderNode.setUseCompositingLayer](https://developer.android.com/reference/android/graphics/RenderNode#setUseCompositingLayer(boolean,%20android.graphics.Paint)).
These sources explain the mechanism; device evidence below establishes the
observed behavior for this implementation.

## Evidence boundaries

- `input-route/result.json`: two automated drags, 147 moves, no cancel; move dispatch
  median 0.136 ms versus event delivery age median 29 ms/max 82 ms. The framework
  gesture forwarding itself was fast in this run. GPU waits and native raster
  commits run on the same UI thread and delayed input delivery.
- `css-before/idle.mp4`: the local CSS pulse disappeared in 48/839 decoded frames.
  `previous.png` and `missing.png` show adjacent frames with/without the pulse.
- `css-after/idle.mp4`: the initial shared-display-list change showed no missing
  pulse in 837 frames, but the user still observed intermittent flicker. This was
  not accepted as a fix; the shared GPU layer was added afterward.
- After installing `build-shared-layer.log`, the user reported a large improvement
  and then that flicker appeared gone, with occasional frame drops remaining.
- `shared-layer/analysis/result.json` is **not a valid disappearance verdict**:
  the WebView was scrolled during recording, moving the pulse out of the fixed ROI.
  The saved lowest frame still visibly contains the pulse elsewhere.
- `shared-layer-motion/result.json`: both automated drags passed; commit median
  6.283 ms, 447,960,064 readback bytes across 190 commits. Full frame owner median
  21.035 ms/max 76.495 ms. This supports improvement, not sustained 60/120 Hz.
- `foreground-motion/result.json` includes concurrent physical input (5 downs,
  2 cancels for a two-swipe script); exclude it from isolated gesture/performance
  acceptance. Its failed second-drag assertion is not an isolated regression proof.
- `inactive-effect-motion/result.json`: isolated two-drag sequence reached the
  bottom and returned to the exact original header bounds, with no cancels.
  Across 511 commits, only 14 slices changed and 1,519 were reused; readback was
  66,695,536 bytes. Commit median/p95 were 0.177/0.253 ms; full frame owner
  median/p95 were 4.578/15.953 ms. An outlier reached 100.773 ms, so this is not
  sustained frame-rate qualification. Drag cursor start/end updates were then
  localized to their own notifier to avoid whole-sample rebuilds at those edges.
- Final `final-motion-up/result.json`: isolated two-drag sequence passed, with
  525 moves/no cancels. Across 531 commits, 4 slices changed and 1,589 were reused;
  readback was 8,373,248 bytes. Commit median/p95/max: 0.186/0.257/6.772 ms.
  Full owner median/p95/max: 4.503/17.943/25.917 ms. Event delivery age
  median/max: 6/26 ms. These are scoped instrumented measurements, not panel
  presentation latency or a guarantee of sustained 60/120 Hz.
- The immediately preceding `final-motion-verified` run started close to the
  bottom, so the requested downward travel exceeded available space. Its movement
  assertions are excluded; the final run reversed direction after checking bounds.
- Final arm64 Release build (`build-cursor-final.log`) passed with zero warnings
  and errors. The 17 raster checks and 3 disabled-filter checks passed. Installed
  base APK SHA-256 matches the built APK:
  `75618eb0031d6192196928f1cccedfb9d27f0e3e26ae8ef4a9932a1e76d3cd73`.
  `source-hashes.json` records the final source inputs. Temporary paint-stack and
  raster-trace diagnostics were removed. The final app is relaunched without
  profiling extras for normal use.
- Source-only navbar fix was confirmed by the user. Earlier 30-frame navigation
  strip comparison found no pixels differing by more than 20 intensity levels in
  the fixed navigation ROI; intermediate routing-only regression is retained in
  `navigation-strip-comparison.json` for comparison.

## Reproduction

Build/install the arm64 Release APK using the repository timeout wrapper, launch
with `doroti_testbed_mode=sample`, select WebView and the local HTML sample.
For timing only, add `DOROTI_MAUI_EVIDENCE=1`, `DOROTI_INPUT_TIMING=1`, and
`DOROTI_PLATFORM_FRAME_PROFILE=1` Activity extras. Disable diagnostic extras for
normal use. Do not interact physically during an isolated automated drag run.

```
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/platform-views/RasterMotion/RasterMotion.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/platform-views/DisabledImageFilter/DisabledImageFilter.csproj -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/measure-android-panel.py --serial DEVICE --out NEW_DIRECTORY --dy 600
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/analyze-android-pulse.py --video VIDEO --out NEW_DIRECTORY --roi X Y WIDTH HEIGHT
```

The pulse detector requires a fixed, unobscured ROI that contains the pulse at
every animation position. Inspect the saved first/lowest frames. It detects
disappearance and does not qualify animation frame pacing.

Overall performance qualification remains **PARTIAL**: the user reported that
flicker appeared gone and movement improved greatly on the shared-layer build;
the subsequent inactive-effect/cursor changes have automated device evidence.
Long-running physical input, sustained refresh-rate performance, other providers,
GPU variants and Android API versions are not established by these scoped checks.

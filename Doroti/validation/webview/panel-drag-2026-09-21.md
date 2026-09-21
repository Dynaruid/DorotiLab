# WebViewSample Web panel drag — 2026-09-21

Implemented in the shared Web host; no sample-specific drag shortcut or loss of backdrop blur. The WebViewSample previously uploaded a full 1000×800 foreground raster every time its 320×180 panel moved, because the same planner segment also contained stationary UI. The existing translation equivalence check could not reuse that combined bitmap.

The host now applies the shared conservative `SkiaPlatformRasterContent.Split` to foreground segments. Each segment reserves eight DOM paint-order slots so slices remain interleaved with native views, effects and input shields. Proven-equivalent slices retain their canvas and move with CSS transforms. Scope changes, changed clipping/content and unsupported groups still redraw; caches advance only on accepted frame receipts. The original 64 MiB frame pixel budget is retained, with a union-raster fallback when splitting would exceed it.

## Measured result

Artifacts: `Doroti/artifacts/webview/2026-09-21/drag/`. Windows Chrome, Release managed Worker, 1000×800 CSS pixels at DPR 1; the real Material WebView sample with its local HTML and nested YouTube embed. Each direction uses 24 CDP pointer moves spaced by a requested 17 ms plus CDP overhead. Measurements begin after pointer-down and gesture recognition settle, and include 350 ms drainage before release. Pointer-down/up and initial loading may still upload changed content.

| Measurement | Before WebGPU | After WebGPU | After WebGL |
|---|---:|---:|---:|
| Outward drag raster p95 | 23.44 ms | 3.32 ms | 3.96 ms |
| Return drag raster p95 | 23.56 ms | 3.13 ms | 3.86 ms |
| Outward / return pixel upload | 60.8 / 64.0 MB | 0 / 0 bytes | 0 / 0 bytes |
| Steady frame pixel backing | 6,400,000 bytes | 6,966,464 bytes | 6,966,464 bytes |
| Retained canvas count | 2 | 4 | 4 |

Sources: `before-gpu-2/result.json`, `final-gpu/result.json`, `final-gl/result.json`. No pre-change WebGL measurement was taken. WebGPU raster p95 fell about 86% for this scenario. More independent slices modestly increase backing memory here while eliminating steady-motion copies. DOM acceptance timing and managed raster time are not physical input-to-display latency, monitor frame pacing or a sustained FPS claim.

## Validation

- Canonical user command built and launched Release before measurement. Final Release build including TypeScript: zero warnings/errors (`build-final.log`).
- Both WebGPU and WebGL: two drags from the panel's current bounds, requested-direction movement, zero steady-motion uploads, stable raster canvases and unchanged iframe identity. Before/after screenshots were inspected.
- WebGL DPR 3 at 1000×900 CSS pixels: union fallback retained two 3000×2700 rasters, 64,800,000 pixel bytes, no renderer error or iframe recreation. Returning to DPR 1 restored independent slices.
- Product integration on WebGPU and WebGL: two WebViews, effect toggling, shield/native input, text focus, DPR 1/1.25/1.5/2 with viewport changes, ten remove/recreate cycles and controller operations passed (`product-gpu/`, `product-gl/`). The WebGPU product run preceded only the additional high-DPR budget fallback; final WebGL covers that revision.
- Common raster motion checks: 34 passed, including moving-panel/stationary-navigation independent reuse and rejection of whole-segment reuse (`raster-motion.log`).
- DOM lifetime checks: 18 passed, including canvas identity, retained RGBA pixels, independent geometry/order, stale frame/size rejection and restoring the primary canvas (`dom-final/`).
- Final WebGPU pixel calibration passed: iframe/raster blur sigma 4/16 measured 3.999/15.996, reset difference 0, tint and saturation values unchanged, and live iframe source updates still visible (`calibration-gpu/pixels.json`).

Physical mouse feel, high-refresh presentation, other browser engines and broad changing-content performance remain unverified. The existing CPU upload path is still used when pixels actually change; this change is retained-content reuse, not GPU texture sharing.

## Reproduce

Start the user's command, then run sequentially from the repository root:

```powershell
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/measure-web-panel-drag.mjs <new-output-directory> http://127.0.0.1:5088 worker-direct-webgpu --expect-retained
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/measure-web-panel-drag.mjs <another-output-directory> http://127.0.0.1:5088 worker-direct-webgl --expect-retained
```

Every validation child uses the repository's 1200-second timeout. Additional controller/pixel fixtures require a build with `-p:DorotiWebPlatformValidation=true`; the panel measurement uses the normal sample and needs no validation export.

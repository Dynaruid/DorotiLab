# Skia color filter regression

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/render-color-filters/RenderColorFilters.csproj -c Release
```

## Source review and fix (2026-09-23)

`ColorFilterLayer.addToScene` calls `SceneBuilder.pushColorFilter`, which captures
the filter in a typed scene payload. `PaintSnapshot.Capture` also captures
`Paint.colorFilter`. The Skia renderer previously ignored both values: the scene
branch used an unfiltered `SaveLayer`, and `ToPaint` never set `SKPaint.ColorFilter`.

The fix applies the scene filter when restoring its child layer and applies the
paint filter to primitive/image drawing and Canvas `saveLayer`. Temporary native
filter wrappers are disposed after the paint/layer retains their native reference.
Unfiltered paints do not allocate a color filter. This is a rendering correctness
fix, separate from the frame-cost and rendering-memory work in the [archived summary](../../../history/26-09-22/web-frame-cost-and-memory-summary.md).

## Validation

The harness compares every pixel with independent, direct Skia drawing on native
CPU surfaces. It covers all four supported filter kinds: blend mode, a color
matrix including alpha reduction, linear-to-sRGB gamma and sRGB-to-linear gamma.

- Per-paint direct replay, SKPicture recording and cached replay (diagnostic
  counters must confirm recording and cache hits).
- Canvas `saveLayer`, nested scene offset/opacity/filter, and retained layer replay.
- Overlapping opaque/translucent geometry, background preservation, an unfiltered
  following sibling and balanced canvas save counts.

Before the change, all 24 comparisons failed, each with 1,644 differing pixels.
After the change, all 24 comparisons matched exactly, including native filter
wrapper disposal before cached replay.

The existing `platform-views/RasterMotion` suite also passed all 34 checks.
`Doroti.Host.Web` and its shared Skia dependencies built in Release with zero
warnings and zero errors. Both suites and the build used the 20-minute timeout
wrapper; `git diff --check` passed.

GPU raster cache, WebGL/WebGPU, physical-device output and frame-rate performance
remain `notVerified`; CPU pixel equivalence does not establish those results.

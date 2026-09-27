# VariableBlur: native performance and adaptive-region follow-up

2026-09-27. The earlier isolated filter benchmark did not measure the sample's
displayed frame rate. The user's report of motion below 30 FPS was consistent
with desktop capture: about 16 visual updates per second before surface pooling.

## Changes

- Adaptive levels share one input capture domain. An attempted per-band subset
  optimization produced misplaced/repeated content in the native window and was
  removed. Only the shading bounds vary per level. The corrected window was
  inspected before and after scrolling.
- Reduced-resolution levels are sampled directly during composition, avoiding
  a full-size upsample intermediate for every level.
- Empty-child variable backdrops avoid a redundant layer/copy/snapshot. Capture
  bounds include both blur axes' halos and align to the working pixel grid.
  Repeat/mirror, nonintegral working dimensions and unsupported grids retain the
  full viewport domain. Retained backing capacity is never the visible viewport.
- Scene filter surfaces reuse the existing frame/context-owned pool. There are
  at most 16 cached slots and 32 million cached RGBA8 pixels (128 MiB). Excess
  surfaces are temporary; this is not a total GPU-memory or peak-memory bound.
- An opt-in `VariableBlurKernel.fastGaussian` approximates a 13-tap kernel using
  7 bilinear reads per pass at working sigma >= 3. Sigma 2..3 mixes kernels; <= 2
  keeps the original. It can sacrifice quality and is not guaranteed faster for
  every scene. The default remains Gaussian.
- SampleApp2 has Full quality, Adaptive (default), Fast adaptive and Fixed 1/4
  radio choices. All choices use the widget pointer-input path in validation.

## Native measurement

Windows App SDK sample, Release, Radeon 780M, Graphite/Vulkan with D3D12/DXGI
presentation, 200% DPI, 2560x1600 monitor. The **client** was
2500x1450; this is not a claim of measuring a 2560x1600 native client. Adaptive
mode, sigma 20, same 120 wheel events at approximately 16 ms spacing in both runs.

| Metric | Before pooling, corrected adaptive geometry | After bounded 16-slot pool |
| --- | ---: | ---: |
| Desktop content-change rate | 16.02 Hz | 32.99 Hz |
| Median change interval | 55.53 ms | 29.21 ms |
| p95 change interval | 98.21 ms | 47.65 ms |
| Median injected wheel interval | 16.01 ms | 16.07 ms |
| CPU surface create/clear mean | 0.935 ms | 0.033 ms |
| CPU variable-filter stage mean | 11.355 ms | 1.357 ms |

After pooling, diagnostics reported 13 surface creations and 1,241 reuses. Stage
timings include startup and recording/synchronization costs and overlap: do not
add them together or interpret them as GPU shader execution time.

DXGI desktop duplication was sampled at a requested 240 Hz. The script detects
pixel changes in a content strip excluding controls/cursor, counts changes only
during the wheel stream with a 100 ms onset allowance, and reports actual input
timing. This is **desktop visual-change rate**, not Present/ETW events, physical
scan-out or a guaranteed application FPS measurement. Sampling can miss updates.
The short paired runs establish an observed improvement, not a 60 FPS guarantee.
Further host/GPU presentation profiling is still needed for consistently smooth
60 Hz motion at this resolution.

A subsequent Fast adaptive run using the checked-in native fixture observed
38.94 visual changes/second, median interval 24.83 ms and p95 44.55 ms. Screenshot
inspection confirmed the radio selection and correct region, but also visible
multiple-image artifacts in strong blur. This option is deliberately not the
default. It is a separate short run, not a controlled statistical comparison.

The final mounted **Ganesh** 2560x1600/200%-DPI benchmark (12 measured frames after
3 warmups) reported complete-frame medians of 4.24 ms off, 11.74 ms full,
10.47 ms adaptive, 10.06 ms fast and 12.64 ms fixed. It renders real widget scenes
and waits for GPU completion, but still excludes the native **Graphite** host's
presentation path. These numbers illustrate why offscreen milliseconds cannot
be converted into the FPS the user sees. Fixed 1/4 was not fastest in this run.

Raw disposable evidence: `Doroti/artifacts/variable-blur-performance/`
`adaptive2-motion.json`, `native-pooled16-motion.json`,
`native-pooled16-report.json`, `profile2-adaptive.png` and
`native-pooled16-after.png`, `final-fast-motion.json`, `final-fast-after.png` and
`final-mounted-benchmark.log`. This document preserves the result after cleanup.

## Reproduction and checks

Run from the repository root; .NET builds share intermediates and run serially.

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --gpu
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --graphite
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/shader-assets/ShaderAssets.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/cupertino-sample -c Release -- --variable-blur --high-dpi --oversized-backing --2560x1600
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/cupertino-sample -c Release --no-build -- --variable-blur --high-dpi --oversized-backing --2560x1600 --frame-benchmark
```

The mounted benchmark includes framework update, scene painting and synchronous
GPU completion, but excludes readback, window presentation and vsync. Its output
explicitly says **NOT FPS**. The separate `--variable-benchmark` isolates filter
cost using a prerecorded background; it does not exercise the complete host
frame lifecycle or its steady-state surface reuse.

Native fixture: use the primary 2560x1600 desktop at 200% DPI with the sample's
controls and measured content strip unoccluded. Coordinates are calibrated for
a 2500x1450 client; the script rejects
other client sizes/DPI. It launches and closes its own process, checks that click
points belong to that process and checks foreground ownership during the wheel
stream, then restores the cursor. Do not interact with the window
during the roughly five-second capture. Inspect the saved mode/after screenshots
to confirm the chosen radio and geometry, since coordinate clicks alone cannot
prove the rendered selection.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build samples/DorotiSampleApp2/windowsappsdk/DorotiSampleApp2.WindowsAppSdk.csproj -c Release
python -m pip install --target Doroti/artifacts/variable-blur-performance/capture-python dxcam==0.3.0 comtypes==1.4.17 numpy
python Doroti/validation/run-with-timeout.py powershell -NoProfile -ExecutionPolicy Bypass -File Doroti/validation/backdrop-filters/measure-variable-blur-window.ps1 -Name adaptive-check -Mode adaptive
```

`-Mode` also accepts `full`, `fast`, `fixed`, `off`. Outputs include screenshots,
measured input timestamps, desktop change samples and native diagnostics.
`DOROTI_VARIABLE_BLUR_PROFILE=1` enables CPU stage counters; the normal path does
not timestamp each stage. The Windows diagnostic document exposes those counters
as `variableBlurProfile`.

Final pixel checks on both Ganesh and Graphite passed 159 existing filter cases,
60 independent Gaussian reference cases, 40 clipped-domain comparisons (odd and
aligned sizes, five scales, four tile modes), and seven adaptive fixtures plus
seven fast-kernel comparisons. Sharp adaptive pixels remain full-resolution;
fast sharp-region error was <= 1/255. Cross-resolution transition error was
<= 2/255. The mounted 2560x1600, 200%-DPI oversized-backing comparison was exact
(maximum channel error 0); scrolling and all four radio selections passed.
Portrait mounted checks also passed. Other-platform execution is not verified.

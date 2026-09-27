# Progressive / Variable backdrop blur

2026-09-27: Doroti now provides a linear spatially varying Gaussian blur through
the existing `BackdropFilter` and `ImageFiltered` pipeline. Blur sigma changes
per output position; this is not a crossfade between a sharp image and a fixed
blurred image. No external shader package or Apple private API is required.

## Usage

For a panel, use the bounds-relative configuration so painting offsets, layout
and resizing automatically update the gradient:

```csharp
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

new ClipRect(
    child: new BackdropFilter(
        filterConfig: ImageFilterConfig.CreateVariableBlur(
            startSigma: 16,
            endSigma: 0), // strong at the top, clear at the bottom
        child: panel));
```

`start` and `end` are optional relative `Offset`s: `(0,0)` is the top-left,
`(1,1)` the bottom-right. They default to `(0,0)` and `(0,1)`. Reverse the sigmas
for a clear top and blurred bottom. Use `(0,0)` → `(1,0)` horizontally or
`(0,0)` → `(1,1)` diagonally. Endpoints may extend beyond the panel. The config
defaults to `bounded: true`, limiting output to its painted bounds; the ancestor
clip also defines rounded or other shapes. Empty layout bounds resolve safely.

For explicit coordinates or composition, use the immutable filter directly:

```csharp
ImageFilter progressive = ImageFilter.variableBlur(
    start: new Offset(0, 0),
    end: new Offset(0, 160),
    startSigma: 16,
    endSigma: 0,
    maxSamples: 32,
    tileMode: TileMode.clamp);

new BackdropFilter(filter: progressive, child: panel);
new ImageFiltered(imageFilter: progressive, child: content);
ImageFilter tinted = new ImageFilter(ColorFilter.saturation(0.8), progressive);
```

Direct endpoints are logical coordinates in the filter's current canvas/layer,
not normalized UVs and not automatically relative to the widget's top-left.
Prefer `CreateVariableBlur` for positioned backdrop widgets. `ImageFiltered`
includes its image-layer offset when resolving the coordinate transform.
Optional direct `bounds` limits backdrop output, as with the existing blur.

Sigma is the Gaussian **standard deviation in logical pixels**, matching the
existing `ImageFilter(sigmaX, sigmaY)` terminology. Sampling extends to roughly
`3 * sigma`; a community shader's `radius: 30` may therefore correspond to
`sigma: 10`, not `sigma: 30`.

## Kernel and limits

### Performance controls and measured results (2026-09-27)

The latest native-window measurements, allocation fixes, adaptive-region repair,
four sample radio modes and reproduction commands are recorded in
[VariableBlur performance follow-up](variable-blur-performance.md).
The older offscreen timings below isolate filter work and are **not FPS**.

Both factories accept `resolutionScale` in `0.125..1` (default `1`) and
`adaptiveResolution` (default `true`). Scale is the **minimum** working resolution
when adaptive mode is enabled. For a fast large panel with sharp low-blur detail:

```csharp
ImageFilterConfig.CreateVariableBlur(
    startSigma: 20,
    endSigma: 0,
    resolutionScale: 0.25,
    adaptiveResolution: true);
```

For minimum scale `0.25`, working resolution follows device-pixel sigma:

| Device sigma | Working resolution |
| --- | --- |
| 0..2 | Full resolution |
| 2..4 | Blend full and half resolution |
| 4..8 | Blend half and quarter resolution |
| 8+ | Quarter resolution |

Each level evaluates the same variable Gaussian, limited to the bounding box of
its contributing band plus the required sampling halo. GPU gradient masks blend
premultiplied RGBA with weights summing to one; bands are not independent hard
clips. Reversed/diagonal gradients, alpha and the existing tile modes are retained.
The smallest singular value of the affine transform converts sigma to device
pixels, conservatively preserving detail under nonuniform scaling/shear. Other
minimum scales use a doubling ladder up to full resolution (at most four levels).
Entirely weak or strong filters skip unused levels and blending.

Set `adaptiveResolution: false` for the previous fixed-resolution path. That
path is cheaper but softens even clear regions and can alias fine detail.
Both modes account for rounded working dimensions on X/Y and keep sigma in
logical coordinates. Both-zero sigma retains full-resolution identity.
`maxSamples` remains a separate per-side tap cap.

The optional `kernel: VariableBlurKernel.fastGaussian` uses seven bilinear reads
per pass at working sigma >= 3, approximating a 13-tap Gaussian. Working sigma
2..3 crossfades with the original kernel; <= 2 preserves the original kernel.
This approximation can lose detail or show sparse-sampling artifacts at large
sigma. `VariableBlurKernel.gaussian` remains the API and sample default.

Earlier Radeon 780M / Windows Vulkan Ganesh measurements, 1080p, sigma 0→20:

| Coverage | Full resolution | Adaptive min 1/2 | Adaptive min 1/4 | Fixed 1/4 |
| --- | ---: | ---: | ---: | ---: |
| 100% | 6.321 ms | 3.122 ms | 2.988 ms | 1.454 ms |
| 25% | 2.458 ms | 2.007 ms | 2.069 ms | 1.145 ms |

Adaptive mode pays for extra passes to preserve sharp detail. It is not always
faster than full resolution for small panels: the 640x360, 25%-coverage fixture
took 0.743 ms full-resolution and 1.071 ms with adaptive min 1/4. The benchmark
prints fixed and adaptive modes separately so callers can choose this tradeoff.

On Ganesh and Graphite, seven adaptive pixel fixtures cover pixel-width strokes,
alpha, vertical/reversed/horizontal/diagonal ramps, 200% DPI and constant sigmas.
The low-sigma region matches full resolution exactly (max channel error 0);
cross-resolution transition error is at most 1 against independently rendered
full/half/quarter levels blended on CPU. On the vertical stroke fixture, the old
fixed 1/4 path had low-sigma mean channel error 50.597/255; adaptive mode has 0.
Twenty odd-size/shear/oversized-backing comparisons also pass at minimum scales
1, 0.5, 0.25, 0.3 and 0.125. The 200%-DPI mounted sample comparison of exact and
oversized buffers still has max channel error 0; its screenshot shows the clear
end without the former pixelated text/rounded corners. The later Windows display
measurement is documented in the follow-up; other-platform execution remains unverified.

Two bottlenecks were addressed:

- Direct variable backdrop filters previously shaded the whole captured surface
  even behind a small clip. They now shade only the visible output and the first
  pass's required second-axis halo. Repeat/mirror retain full first-pass coverage
  so wrapped samples stay valid. Composed filters retain full coverage because
  subsequent stages can read beyond the final clip.
- Gaussian weights now use a recurrence (one exponential per pixel), and the
  bounded loop exits after the active tap count. The largest measured gains come
  from reducing the shaded area/resolution, not from this arithmetic change.

Historical fixed-resolution optimization measurements (before adaptive mode),
Radeon 780M, Windows Vulkan Ganesh, 1920x1080, sigma 0→20, 32 taps per side:

| Visible coverage | Before, full resolution | After, full resolution | After, half resolution | After, quarter resolution |
| --- | ---: | ---: | ---: | ---: |
| 100% | 6.096 ms | 5.845 ms | 2.325 ms | 1.597 ms |
| 25% | 5.934 ms | 2.643 ms | 1.680 ms | 1.360 ms |

Times are medians of 12 frames following 3 warmups, including synchronous GPU
completion and filter capture/composition, excluding pixel readback and window
presentation. Background geometry is rendered once and copied each frame to
isolate filter cost. These are offscreen filter timings, **not displayed FPS**.
An earlier geometry-heavy fixture spent roughly 8–11 ms recording its thousands
of background rectangles; those times cannot be attributed to VariableBlur.
Quarter resolution is not always fastest for a tiny region because extra
downsample/reconstruction passes have overhead. Full-size capture/layer surfaces
still existed in this measurement. The follow-up removes redundant empty-child
layers and pools intermediate surfaces; it does not remove all composition cost.

The historical fixed-resolution Graphite benchmark had 1080p full coverage of
6.470 / 3.688 / 2.733 ms at full / half / quarter resolution; 25% coverage was
3.739 / 2.733 / 2.239 ms. No pre-change Graphite timing was collected.

Validation passed on both Ganesh and Graphite: 60 VariableBlur comparisons
against an independent CPU 2D Gaussian plus bilinear resampling, 12 odd-sized
fractional-clip/shear/tile-mode comparisons against full-frame GPU output, and
the existing 159 filter comparisons. Full-resolution tolerances remain 2 channel
levels (5 for diagonal resampling); reduced-resolution tolerances are 5 (8 for
diagonal), including low-resolution sampling-count rounding. The cropped versus
full-frame comparisons allow at most 1 channel level. Embedded shader hash/ABI
checks passed. SampleApp2 now defaults to Adaptive and exposes Full quality,
Adaptive, Fast adaptive and Fixed 1/4 radio choices. Its mounted Vulkan test
checks active scrolling and all four choices through pointer input. Native
desktop visual-change measurements are in the follow-up; physical scan-out FPS
and other-platform execution remain unverified.

Follow-up viewport regression: Windows can retain a GPU surface larger than the
current viewport. The initial fast path resized the entire backing snapshot,
shrinking content inside the blur when spare capacity existed. Backdrop capture
now snapshots only `(0, 0, viewportWidth, viewportHeight)` before filtering. This
also keeps repeating/mirrored sampling confined to the viewport. The odd-size
region test now uses oversized backing storage. The mounted sample can reproduce
this host condition with `--variable-blur --high-dpi --oversized-backing`; at 200%
DPI it compares oversized and exact-sized surfaces pixel-for-pixel before
scrolling. The corrected mounted Ganesh comparison had maximum channel error 0.

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --gpu --variable-benchmark
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --graphite --variable-benchmark
```

Add `--full-resolution-only` for a single quality setting. Benchmark PNGs and
local logs are disposable under `Doroti/artifacts/variable-blur-performance/`.

### Shader implementation

The shader source lives in
[`Doroti.Skia.Rendering/Shaders/variable_blur.sksl`](../../src/Doroti.Skia.Rendering/Shaders/variable_blur.sksl),
embedded by its owning renderer assembly. `FrameworkShaderManifest` registers
`rendering.variable-blur`; the common loader checks its SHA-256 and uniform/sampler
ABI and caches the program. `SkiaSceneRenderer.VariableBlur.cs` retains coordinate
conversion, parameter binding and GPU pass/surface ownership. Widgets' stretch
and Material's ink sparkle shaders remain with their owning projects. Doroti's
original blur has no Flutter source reference in the manifest.

Embedded loading is synchronous and cached for renderer use; framework callers
retain the `LoadProgram`/`BeginLoad` APIs and share the same cache. No filesystem
or network asset lookup is added to frame rendering. This does not change GPU
shader compilation or context-specific runtime-effect caching.

When editing a packaged `.sksl`, update its `AdaptedSourceSha256` in
`FrameworkShaderAssets.cs` (`Get-FileHash <path> -Algorithm SHA256`, lowercase).
Uniform/sampler changes also require matching manifest and C# binding updates.
Keep LF line endings, as specified by the repository's `.gitattributes`.

After this source extraction, the three embedded shader assets passed hash/ABI,
cache and callback checks, including missing-asset diagnostics. Variable Blur's
20 offscreen pixel comparisons passed again on each Vulkan backend. Displayed
Windows results below are from the earlier implementation run; display testing
was not repeated for this source-only shader relocation.

For a logical position `p`, the filter computes
`t = clamp(dot(p-start, end-start) / dot(end-start, end-start), 0, 1)` and
`sigma = startSigma + (endSigma-startSigma) * t`. Beyond either endpoint the
endpoint sigma stays constant. Zero sigma returns the source pixel unchanged.
Invalid/nonfinite sigmas, endpoints separated by less than 1e-6 logical pixels
at shader precision, invalid tile modes and
sample counts outside `1..64` are rejected. Negative sigmas are rejected.

Two SkSL passes run on GPU-owned intermediate surfaces. The first samples along
the gradient, the second along its perpendicular. Sigma is constant along the
second direction, avoiding the kernel-strength mixing of an arbitrary fixed
horizontal-then-vertical ordering. Each pass normalizes Gaussian weights and
works with premultiplied RGBA, preserving transparency. This uses the existing
RGBA8 working surface and is not a new linear-light/HDR blur implementation.

The kernel is truncated at three sigma and bilinearly sampled; it is an
approximation, not bit-identical to native `SKImageFilter.CreateBlur`.
`maxSamples` defaults to 32 **per side per pass** (up to 65 reads per pass).
Larger sigma/device scale can exceed this sampling budget and show banding;
increasing it up to 64 improves sampling at additional cost. Diagonal directions
also introduce intermediate-image resampling error. Tiling applies at the
captured layer's image boundary on each pass, not at the output clip; diagonal
edge behavior is correspondingly approximate. Ordinary clips can still sample
background pixels outside the output clip, so they do not truncate the halo.

Invertible affine transforms, including translation, scale, rotation and skew,
map the gradient and sampling axes to device pixels. Perspective and singular
transforms are explicitly unsupported. Ordinary/color/shader filter composition,
retained scenes and backdrop contents inside opacity layers use the existing
owned-layer pipeline. Foreground child content is drawn after the backdrop blur.

This requires Doroti's Skia GPU target. A CPU target is rejected; no software
capture or readback fallback is introduced. Native PlatformView/WebView content
outside the Skia layer is not captured; the native composition planner rejects
this filter even when a Gaussian/saturation intent is attached. The API currently
supports a linear gradient, not arbitrary mask images or multi-stop gradients.

GPU intermediates remain part of the existing shader filter path. The follow-up
measures native desktop content changes and CPU stage timings; peak GPU memory,
physical scan-out and cold shader compilation remain **notMeasured**.
No WGSL compiler or change to the active WGSL work plan is needed.

## Research consulted

Reviewed on 2026-09-27; the implementation is written for Doroti rather than
copying a third-party shader.

| Primary source | Finding and Doroti decision |
| --- | --- |
| [Variablur](https://github.com/daprice/Variablur) and its [Metal source](https://github.com/daprice/Variablur/blob/main/Sources/Variablur/Blurs.metal) | Public Metal shader controls radius from a mask and normalizes Gaussian samples. Its two-axis approach documents possible streaks and sampling-budget banding. Doroti uses a linear gradient with axes ordered to keep sigma constant in the second pass. |
| [nikstar/VariableBlur source](https://github.com/nikstar/VariableBlur/blob/main/Sources/VariableBlur/VariableBlur.swift) | The UIKit implementation dynamically looks up `CAFilter` and installs `variableBlur` on a backdrop layer. Doroti does not depend on this platform-private mechanism. |
| [Skia SkSL & Runtime Effects](https://skia.org/docs/user/sksl/) | Runtime shaders evaluate child shaders inside Skia's pipeline. Doroti uses its existing effect cache, image binding and GPU surface ownership rather than adding a platform-specific blur engine. |

## Verification and reproduction

All commands run from the repository root with the required 1,200-second deadline.
Run .NET builds serially because they share project intermediates.

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --gpu
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --graphite
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/platform-views/Common/Common.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/shader-assets/ShaderAssets.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet build samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/backdrop-filters/verify-variable-blur.py
```

Select the interactive sample with `DOROTI_TESTBED_MODE=variable-blur`. It shows
original stripes, progressive backdrop blur, sharp foreground text and a toggle.
The display script accepts `DOROTI_VARIABLE_BLUR_EXE` and
`DOROTI_VARIABLE_BLUR_OUT` for the MAUI Windows executable and separate artifacts.

Recorded results:

- Ganesh/Vulkan and Graphite/Vulkan: **PASS**, 20 comparisons each against an
  independent CPU double-precision 2D Gaussian reference. Covers vertical,
  horizontal, reversed and diagonal gradients; constant/zero sigma; translation,
  nonuniform scaling, rotation and image-layer offset; opacity; foreground;
  color composition; retained replay; all four tile modes; ancestor and explicit
  bounds clipping. Full-image per-channel tolerance is 2/255; diagonal comparison
  uses the interior and a 5/255 tolerance because of two-pass resampling. Observed
  maxima were 1.185/255 and 4.263/255 respectively. Zero sigma was exact.
- Existing native filter pixel regressions: **PASS**, 159 each on both Vulkan
  backends and 90 on CPU. CPU also passes new argument, value equality,
  bounds-relative config and explicit GPU-only rejection checks.
- Native platform composition common contracts: **PASS**, 14 cases, including
  rejection of variable blur carrying a saturation intent.
- Windows App SDK displayed product: **PASS**, three window sizes at 200% DPI,
  clear → middle → blurred stripe contrast approximately 123.51 → 64.44 → 19.15.
  Synthetic toggle restores contrast to 127.5 throughout. Screenshot inspection
  confirms sharp foreground text. This is displayed-window evidence, distinct
  from the offscreen pixel checks and from physical-input testing.
- MAUI Windows displayed product: **PASS**, the same three sizes at 200% DPI,
  contrast approximately 123.51 → 64.49 → 19.15 and 127.5 throughout when disabled.
  Both Windows runners build in Release with zero warnings/errors. The display
  harness waits for MAUI's first presented frame and ignores cloaked startup
  windows; it verifies click ownership before synthetic mouse input.
- Metal, Android, Linux, WebGL2 and WebGPU execution of this new filter:
  **notVerified**. They share the Skia filter implementation, but code sharing
  and build success do not prove device execution. Physical input and performance
  remain **notVerified** and **notMeasured** respectively.

Raw logs/screenshots are disposable under `Doroti/artifacts/variable-blur/` and
`Doroti/artifacts/variable-blur-*.log`. The results above remain the tracked record
after artifact cleanup.

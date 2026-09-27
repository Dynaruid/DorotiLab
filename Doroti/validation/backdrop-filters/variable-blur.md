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

Full-surface GPU intermediates remain part of the existing shader filter path.
No steady frame-time, peak-memory or first-use performance claim is made:
**notMeasured**. No WGSL compiler or change to the active WGSL work plan is needed.

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

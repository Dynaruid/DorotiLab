# Image and backdrop filter coverage

Doroti's shared Skia renderer implements Gaussian blur, matrix transforms,
dilation, erosion, color filters and inner-to-outer composition for both
`BackdropFilter` and `ImageFiltered`. Shader filters and compositions containing
shaders use GPU intermediate images, including the current backdrop inside
opacity, color-filter and shader-mask layers. There is no software capture fallback.

## C# API

```csharp
new BackdropFilter(
    filter: new ImageFilter(
        outer: ColorFilter.saturation(1.4),
        inner: new ImageFilter(sigmaX: 10, sigmaY: 10)),
    child: panel);

ImageFilter expanded = ImageFilter.dilate(radiusX: 2, radiusY: 3);
ImageFilter contracted = ImageFilter.erode(radiusX: 2, radiusY: 3);
ImageFilter shaderFilter = new ImageFilter(fragmentShader);
ImageFilter combined = new ImageFilter(outer: shaderFilter, inner: expanded);
ImageFilter color = ColorFilter.srgbToLinearGamma(); // implicit conversion
```

`ColorFilter.matrix`, `mode`, `linearToSrgbGamma`, `srgbToLinearGamma` and
`saturation` can be passed where an `ImageFilter` is expected. Saturation uses
Flutter's [luminance matrix](https://api.flutter.dev/flutter/dart-ui/ColorFilter/ColorFilter.saturation.html):
zero removes color, one preserves it, values above one increase saturation.
Morphology radii must be finite, nonnegative and representable by Skia.

Shaders use Doroti's existing **SkSL** contract: first floating-point uniform is
`float2`/`half2` input size; first `uniform shader` receives the input image.
The renderer supplies those automatically. Remaining uniforms and image samplers
are provided by the caller. Flutter GLSL files are not directly interchangeable.
`isShaderFilterSupported` describes Doroti's GPU renderer capability; an arbitrary
CPU canvas is not a supported shader-filter target.

The shader backdrop path owns full-surface GPU intermediates so nested filters
sample the active layer rather than accidentally sampling the root. Ordinary
non-shader scenes retain the native Skia filter/cache path. This work does not
extend platform-native WebView/PlatformView compositor capabilities: their
existing negotiated Gaussian/saturation restrictions still apply.

## Reproduction

Run from the repository root (each process has a 1,200-second deadline):

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --gpu
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/backdrop-filters/BackdropFilters.csproj -c Release -- --graphite
```

The fixture compares actual renderer pixels with independent native Skia draws.
It covers background and child filtering, nested opacity/color layers, rounded
clips, child foreground content, retained replay, scaled filters and `src` blending.
Shader oracles use a native color matrix, including shader-before/after-morphology,
shader/blur, shader/color and shader/shader composition. A spatial mirror shader
also checks automatic input dimensions and displaced sampling against a native
matrix filter, including a nested backdrop layer. A transparent background
makes incorrect alpha compositing observable. GPU readback is only for validation.

2026-09-26: Ganesh/Vulkan and Graphite/Vulkan passed 159 comparisons each on an
AMD Radeon 780M. CPU checks cover the 90 non-shader comparisons. These are
offscreen GPU/CPU pixel checks, not physical display presentation evidence.
WebGL2, WebGPU, Metal and mobile device execution are **notVerified** in this run;
frame time and peak GPU memory for the new shader pipeline are **notMeasured**.

Existing color-filter replay tests (24 checks), platform raster motion tests
(34 checks), and Release builds of Widgets, Hosting and Skia.Rendering also pass.

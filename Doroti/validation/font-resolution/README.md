# Font resolution regression

From the repository root:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/font-resolution -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/font-resolution/verify-native-font-cache.py
```

Checks actual Skia measurements and raster output for host defaults, unknown
families, proportional Latin advances, Roboto 400/500/700, Korean 400/700,
explicit per-glyph fallback, registration aliases, synthetic italic, Windows
Segoe UI defaults/Cupertino aliases, and registration cache invalidation.
PNG artifacts are written under `Doroti/artifacts/font-resolution`.
Roboto test inputs use the existing sample assets. Noto/Nanum fixtures are fetched
from pinned URLs into the ignored artifact directory and checksum-verified on the
first run; tests no longer depend on `Doroti.Host.Web/wwwroot/fonts`.

Native bundle checks register the three embedded faces, compare their measurements,
and preserve the Segoe UI default. The cache test deliberately corrupts a generated
cache entry, verifies offline failure, repairs it online, and verifies unchanged
offline reuse. It requires the preceding test/build to populate the native cache.

2026-09-27 validation: this regression and `validation/cupertino-sample` passed;
SampleApp2 Web Release built with zero warnings/errors. The actual Chrome
SampleApp2 page was reloaded and inspected: the previous monospace-looking
default changed to proportional Roboto with distinct heading weights. Profile
input `한글 Doroti` displayed correctly in both the normal input and bold greeting;
the screenshot is a disposable local artifact at
`Doroti/artifacts/font-resolution/browser-profile.png`.

This is not a physical Korean IME composition test. macOS/iOS/Android font
selection has not been device-validated here. Full Flutter shaping, arbitrary
script fallback downloads and color emoji coverage are outside this regression.

# Third-party notices

Doroti is independently implemented. Reference checkouts are not runtime dependencies. Any source that is selected for adaptation must also appear in the source and provenance manifests before it can enter a product project.

## Flutter

- Upstream: https://github.com/flutter/flutter
- Pinned R3 API baseline: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- Use: read-only API and behavior reference
- License: BSD 3-Clause; see `../reference/flutter-master/LICENSE`
- Copyright: Copyright 2014 The Flutter Authors

## SkiaSharp

- Upstream: https://github.com/mono/SkiaSharp
- Package: `SkiaSharp` 4.151.1; platform-native assets are selected transitively by the target RID graph
- Use: GPU surface implementation behind the MAUI and Web hosts
- License: MIT; package license metadata is preserved by NuGet restore and distribution packaging

The license files named above are inputs to `eng/doroti.ps1 audit`. Distribution packaging must reproduce the applicable notice and license text for every promoted third-party source.

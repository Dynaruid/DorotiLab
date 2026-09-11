# Third-party notices

Doroti is independently implemented. Reference checkouts are not runtime dependencies. Any source that is selected for adaptation must also appear in the source and provenance manifests before it can enter a product project.

## Flutter

- Upstream: https://github.com/flutter/flutter
- Pinned R3 API baseline: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- Use: read-only API and behavior reference
- License: BSD 3-Clause; see `../reference/flutter-master/LICENSE`
- Copyright: Copyright 2014 The Flutter Authors

## Flutter rounded superellipse paths

- Upstream: Flutter engine `lib/web_ui/lib/rsuperellipse_param.dart`, revision `35e669cfa38f3f66d1a743486c1ddceed23f0841`
- Adapted file: `src/Doroti.Skia.Rendering/SkiaRSuperellipsePath.cs`
- Copyright 2013 The Flutter Authors; BSD 3-Clause, reproduced in [LICENSES/Flutter-LICENSE.md](LICENSES/Flutter-LICENSE.md).
- Changes: C# Skia path construction, radius normalization, roundoff guard and a shared contour for painting and clipping.
- Selected source, hashes and dependency closure: [mac-menu-source-provenance.json](validation/fcr7-material-widget/mac-menu-source-provenance.json).

## Material color image extraction

- Upstream behavior: https://github.com/material-foundation/material-color-utilities, Dart package `material_color_utilities 0.13.0`
- C# Wu source: https://github.com/albi005/MaterialColorUtilities/blob/v0.3.0/MaterialColorUtilities/Quantize/QuantizerWu.cs
- Adapted files: `src/Doroti.Runtime/MaterialImageQuantizerWu.cs`, `src/Doroti.Runtime/MaterialImageColorRuntime.cs`
- Copyright 2021 Google LLC; Copyright 2021-2022 project contributors
- License: Apache-2.0, reproduced in [LICENSES/material-color-utilities-Apache-2.0.txt](LICENSES/material-color-utilities-Apache-2.0.txt)
- Changes: 64-bit histogram moments, floating-point squared sums, Dart centroid rounding, pinned five-iteration Wsmeans initialization and score filtering/hue separation, explicit RGBA-to-ARGB conversion.
- `PaletteRandom` adapts seeded Dart VM Random from `sdk/lib/_internal/vm/lib/math_patch.dart`; Copyright 2012, the Dart project authors, BSD-3-Clause, reproduced in [LICENSES/dart-BSD-3-Clause.txt](LICENSES/dart-BSD-3-Clause.txt).
- Selected sources, hashes, and dependency closure: [image-pipeline/source-provenance.json](validation/image-pipeline/source-provenance.json). Existing NuGet `MaterialColorUtilities 0.3.0` supplies Lab/HCT color math.

## SkiaSharp

- Upstream: https://github.com/mono/SkiaSharp
- Packages: `SkiaSharp` and platform native assets `4.154.0-preview.1.26454.9`; public Graphite Vulkan, Metal and Dawn APIs are used. Silk.NET supplies standard Vulkan dispatch; no custom Skia binary is redistributed
- Use: GPU surface implementation behind the Windows App SDK, MAUI, AppKit, Web, and Linux/Qt hosts
- License: MIT; [license text](LICENSES/SkiaSharp-LICENSE.txt) and [native third-party notices](LICENSES/SkiaSharp-Native-THIRD-PARTY-NOTICES.txt) are copied verbatim from the pinned official Win32 NuGet archive and included in Doroti packages

## Silk.NET

- Upstream: https://github.com/dotnet/Silk.NET
- Packages: `Silk.NET.Direct3D11`, `Silk.NET.Direct3D12`, `Silk.NET.DXGI`, `Silk.NET.Direct3D11.Extensions.D3D11On12`, and the existing Vulkan packages, version 2.23.0
- Use: generated native graphics bindings; `Doroti.Graphics.DirectX` owns Windows COM references and uses the binding-independent SkiaSharp Direct3D API
- License: MIT; package license metadata is preserved by NuGet restore and distribution packaging

## ANGLE Windows runtime

- Upstream: https://github.com/AvaloniaUI/angle
- Package: `Avalonia.Angle.Windows.Natives` 2.1.27548.20260419
- Upstream package commit: `1c89805903c1482166356d3b950d474973180e61`
- Use: x64 EGL/GLES runtime for the default Windows App SDK hardware-D3D11 presenter
- License: BSD-style ANGLE license; the package `LICENSE` file and required binary-redistribution notice must be preserved

## Microsoft Windows App SDK

- Upstream: https://github.com/microsoft/windowsappsdk
- Package: `Microsoft.WindowsAppSDK` 2.4.0 for the Windows App SDK host (the repository-wide central version is overridden by this host)
- Use: AppWindow, self-contained Windows App Runtime bootstrap/runtime, and native metadata used by the `HwndExactCpp` target
- License: Microsoft Windows App SDK package license terms; the restored `license.txt` and licenses for included Microsoft components govern use and redistribution

## Wayland background-effect protocol

- Upstream: https://gitlab.freedesktop.org/wayland/wayland-protocols
- Use: vendored `ext-background-effect-v1.xml` client protocol description for compositor blur negotiation
- License: MIT; the copyright and permission notice are preserved in each vendored XML file

## KDE Plasma Wayland blur protocol

- Upstream: https://invent.kde.org/libraries/plasma-wayland-protocols
- Use: vendored legacy `blur.xml` client protocol description for older KWin compositors
- License: LGPL-2.1-or-later; SPDX notices are preserved in each vendored XML file

Distribution packaging must reproduce the applicable notice and license text for every promoted third-party source.


## DorotiTestbedApp Material sample adaptation

`DorotiTestbedApp/src/MaterialSample/` adapts the local Flutter Material sample
in `reference/flutter_sample_app` (Flutter team, BSD-3-Clause). The original
copyright headers and license are retained in `LICENSE.flutter` in that directory.
The local Image demo and its WebP resource are the already retained reference extension.

`DorotiTestbedApp/assets/fonts/MaterialIcons-Regular.otf` is copied from the pinned
local Flutter SDK `bin/cache/artifacts/material_fonts/materialicons-regular.otf`.
Its pinned SDK license (CC BY 4.0) is retained alongside it as `LICENSE.materialicons.txt`.
The Roboto regular/medium/bold fonts come from the same SDK directory; their
Apache-2.0 license is retained as `LICENSE.roboto.txt`.

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
- Packages: `SkiaSharp` and platform native assets `4.152.0-rc.1.26426.14`; `SkiaSharp.Vulkan.Silk.NET` provides the typed Silk.NET Vulkan bridge on Windows
- Use: GPU surface implementation behind the Windows App SDK, MAUI, AppKit, Web, and Linux/Qt hosts
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

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

## Wayland background-effect protocol

- Upstream: https://gitlab.freedesktop.org/wayland/wayland-protocols
- Use: vendored `ext-background-effect-v1.xml` client protocol description for compositor blur negotiation
- License: MIT; the copyright and permission notice are preserved in each vendored XML file

## KDE Plasma Wayland blur protocol

- Upstream: https://invent.kde.org/libraries/plasma-wayland-protocols
- Use: vendored legacy `blur.xml` client protocol description for older KWin compositors
- License: LGPL-2.1-or-later; SPDX notices are preserved in each vendored XML file

Distribution packaging must reproduce the applicable notice and license text for every promoted third-party source.

# Third-party notices

Doroti is independently implemented. Reference checkouts are not runtime dependencies. Any source that is selected for adaptation must also appear in the source and provenance manifests before it can enter a product project.

## Flutter

- Upstream: https://github.com/flutter/flutter
- Pinned R3 API baseline: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- Use: read-only API and behavior reference
- License: BSD 3-Clause; see `../flutter-master/LICENSE`
- Copyright: Copyright 2014 The Flutter Authors

## Avalonia

- Upstream: https://github.com/AvaloniaUI/Avalonia
- Source-port pin: upstream `main@2026-07-31`, commit `f159423f691946e713f454447a780d4677d8a0d2`; selected closure hashes are recorded in `migration/avalonia-shell/shell-dependency-graph.json`
- Official runtime package: `Avalonia.Desktop` 12.1.0, isolated behind the comparison-only `Doroti.Host.Avalonia`; it is absent from the A1 product build graph
- Selected-source use: A0 selects shared platform/dispatcher/render contracts, Win32/automation, X11/Wayland/FreeDesktop, macOS managed/native and Skia/OpenGL closure. A1 adapts the pinned Win32 window/dispatcher/input/IME/clipboard/cursor lifecycle into `Doroti.Vendor.Avalonia.Win32`; exact inputs and hashes are recorded in `migration/avalonia-shell/a1-source-port-provenance.json`. G7-3M adapts the managed C ABI and AppKit/libAvalonia source into `Doroti.Vendor.Avalonia.Native`; its source mapping, generated header, local hashes, dependency closure and license identity are recorded in `migration/avalonia-shell/g7-macos-source-port-provenance.json`
- License: MIT; official release text is at https://github.com/AvaloniaUI/Avalonia/blob/12.1.0/licence.md and the selected-source snapshot is at `../Avalonia-main/licence.md`
- Copyright: Copyright AvaloniaUI OÜ

## SkiaSharp

- Upstream: https://github.com/mono/SkiaSharp
- Package: `SkiaSharp` 3.119.4; platform-native assets are selected transitively by the target RID graph
- Use: internal BGRA8888 and GPU surface implementation behind `Doroti.Backends.Skia`, including WGL on Windows and NSOpenGL on `osx-arm64`
- License: MIT; package license metadata is preserved by NuGet restore and distribution packaging

The license files named above are inputs to `eng/doroti.ps1 audit`. Distribution packaging must reproduce the applicable notice and license text for every promoted third-party source.

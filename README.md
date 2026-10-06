# Doroti

Before launching a target, run `pwsh -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform windows -Scope target`. [Provider doctor](Doroti/docs/doctor.md) report prerequisites; device and deployment acceptance are separate.

**English** | [한국어](README.ko.md)

### Cross-platform UIs in C#, without XAML

Doroti is an experimental UI framework built with C# and .NET. Write widgets, layouts, and UI behavior directly in C#, and share application code across desktop, mobile, and the web.

Doroti brings familiar Flutter-inspired Material and Cupertino APIs together with a shared rendering pipeline built on SkiaSharp and native platform integration. Its framework code is developed and maintained directly in C#.

[Get started](#get-started) · [Platforms](#platforms) · [Documentation](#documentation)

> [!WARNING]
> Doroti is under active development. APIs, behavior, and project structure may change without backward-compatibility guarantees. Platform maturity varies.

## Highlights

- **C# throughout your UI** — define widgets, layout, state, and interactions without XAML.
- **Shared application code** — keep your UI in a platform-neutral library with separate runners for each target.
- **Material and Cupertino widgets** — build on Flutter-inspired APIs implemented and maintained in C#.
- **SkiaSharp-based GPU rendering** — access Skia Graphite from C# through SkiaSharp, using Vulkan, Metal, or WebGPU depending on the platform.
- **Image and backdrop filters** — Gaussian and progressive/variable blur, matrix transforms, dilation/erosion, saturation/color filters, and composed GPU shaders. See the [WGSL compiler guide](tools/Doroti.Wgsl/README.md).
- **Native integration** — platform hosts connect the UI to windows, input, text entry, clipboard, and accessibility services.
- **A sample app and templates** — explore `DorotiTestbedApp` and the `doroti-app` project template.

## Platforms

Doroti shares its widget, layout, painting, and semantics layers across platforms. Each host supplies the native integration and GPU surface.

| Platform | Native host | Doroti implementation | Default rendering backend |
| --- | --- | --- | --- |
| Windows (default) | Windows App SDK, C++ child HWND (`HwndExactCpp`) | `Doroti.Host.WindowsAppSdk` + native C++ host | Skia Graphite / Vulkan |
| Windows (optional) | .NET MAUI / WinUI | `Doroti.Host.Maui` | Skia Graphite / Vulkan |
| Android | .NET MAUI / Android native views | `Doroti.Host.Maui` | Skia Graphite / Vulkan |
| iOS | .NET MAUI / UIKit | `Doroti.Host.Maui` | Skia Graphite / Metal |
| macOS | Native AppKit / MetalKit (`MTKView`) | AppKit adapter in `Doroti.Host.Maui` | Skia Graphite / Metal |
| Mac Catalyst | .NET MAUI / UIKit (Mac Catalyst) | `Doroti.Host.Maui` | Skia Graphite / Metal |
| Web | .NET WebAssembly, render Worker / canvas | `Doroti.Host.Web` | Desktop/iOS: Ganesh/WebGL2; Android: Graphite/Dawn/WebGPU preferred |
| Linux | Qt 6 `QWindow`, native C ABI bridge | `Doroti.Host.Qt` | Skia Graphite / Vulkan |

Web auto selection prefers WebGL2 on desktop/iOS and WebGPU on Android, with initial fallback when available. Explicit `dorotiRenderer=worker-direct-webgl` or `worker-direct-webgpu` overrides auto selection. Runtime graphics loss requires a fresh endpoint; see the [Web configuration guide](Doroti/README.md#web).

The [2026-10-05 Apple follow-up](history/26-10-05/work3-summary.md#14-2026-10-05-실행-결과) records current
AppKit auxiliary windows/NSMenu, Catalyst scene policies, shared mobile view
lifetimes and typed iOS device selection. Build, native automation, physical input
and distribution have separate acceptance scopes; the complete plan remains partial.

## Get started

The sample application is the starting point for exploring Doroti. Prepare these dependencies on the build host:

- **Windows, macOS, and Linux:** Install [PowerShell 7](https://learn.microsoft.com/ko-kr/powershell/scripting/install/install-powershell?view=powershell-7.6) to run the repository scripts.
- **.NET SDK:** Install the 10.0.400 feature band for targets other than iOS. For the iOS Testbed, also install 11.0.100-rc.1.26425.128 alongside .NET 10.
- **Platform tools:** The default Windows runner needs the Windows C++ build tools in the [platform prerequisites](Doroti/README.md#platform-prerequisites). See that guide for workloads and native tools for other targets.
- **WGSL sample build:** The source-tree Testbed currently needs the [WGSL compiler](tools/Doroti.Wgsl/README.md). Install the Rust 1.95.0 toolchain and build it once as shown below.

Clone the repository and run the sample from its root:

```powershell
git clone https://github.com/Dynaruid/DorotiLab.git
cd DorotiLab

python Doroti/eng/run-with-timeout.py cargo build --locked --manifest-path tools/Doroti.Wgsl/Cargo.toml
$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiTestbedApp -Platform windows
```

The command builds and launches the default Windows App SDK runner. Follow the [sample app guide](samples/DorotiTestbedApp/README.md) for Android, iOS, macOS, Mac Catalyst, Linux, and Web commands, or the [framework guide](Doroti/README.md) for build and publish options.

## How it works

Doroti owns the widget and render trees. Platform hosts provide native services and a GPU surface, while the shared framework handles layout, painting, and UI state.

```text
C# application
      ↓
Doroti widgets, layout, and state
      ↓
Shared rendering pipeline
      ↓
Platform host + GPU surface
```

Flutter is the behavior reference for the Material and Cupertino APIs. Doroti maintains its own C# implementation in `Doroti.Framework.*`; it does not embed the Flutter runtime in a WebView.

The Dart-to-C# compiler is an optional source import and behavior-comparison tool.

## Documentation

| Guide | Contents |
| --- | --- |
| [Framework guide](Doroti/README.md) | SDKs, workloads, build commands, packaging, and host configuration |
| [Sample app guide](samples/DorotiTestbedApp/README.md) | Platform launch commands, sample screens, renderer options, and troubleshooting |
| [Cupertino sample](samples/DorotiSampleApp2/README.md) | Cupertino components, profile input, and appearance settings on Windows / Web |
| [Custom carousel sample](samples/DorotiCarouselApp/README.md) | Five customizable carousel demos with snapping, looping, transforms, and controllers |
| [iOS / Android deployment helper](helpers/deploy-helper/README.md) | Select a sample and device, build, install, and launch |
| [Dart-to-C# compiler](tools/Doroti.DartToCSharp/README.md) | Optional source import and migration tooling |
| [WGSL compiler](tools/Doroti.Wgsl/README.md) | Shader compilation, backend profiles, and application integration |

## Project status

Current host, renderer, build-mode and execution boundaries: [support status](Doroti/docs/support-status.md).

Doroti is a personal, experimental project. The platform table describes implemented hosts and rendering defaults, not uniform production readiness.

GPU compatibility, input methods, accessibility, performance, signing, and store distribution depend on the target platform and configuration. Consult the [framework guide](Doroti/README.md#platform-support) before choosing a target.

## Repository layout

| Path | Contents |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | Framework, runtime, rendering, hosts, and SDK |
| [`samples/DorotiTestbedApp/`](samples/DorotiTestbedApp/) | Shared sample application and platform runners |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` template |
| [`Doroti/eng/`](Doroti/eng/) | Build, run, packaging, and diagnostic tools |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Optional Dart-to-C# compiler |

## Feedback and contributions

Doroti is a personal hobby project that I build for fun. As a result, I may not be able to actively review or merge pull requests.

Ideas and bug reports are welcome, as are forks that use Doroti to experiment or explore new directions. When reporting a problem, include the platform, .NET SDK version, rendering backend, and steps to reproduce it.

## License

Doroti is licensed under the [BSD 3-Clause License](LICENSE). See [third-party notices](Doroti/THIRD-PARTY-NOTICES.md) for upstream attribution.

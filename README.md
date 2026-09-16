# Doroti

**English** | [한국어](README.ko.md)

### Cross-platform UIs in C#, without XAML

Doroti is an experimental UI framework built with C# and .NET. Write widgets, layouts, and UI behavior directly in C#, and share application code across desktop, mobile, and the web.

The project began by translating Flutter framework source code into C# and is now developed and maintained directly in C#. It brings familiar Material and Cupertino APIs together with a shared rendering pipeline built on SkiaSharp and native platform integration.

[Get started](#get-started) · [Platforms](#platforms) · [Documentation](#documentation)

> [!WARNING]
> Doroti is under active development. APIs, behavior, and project structure may change without backward-compatibility guarantees. Platform maturity varies.

## Highlights

- **C# throughout your UI** — define widgets, layout, state, and interactions without XAML.
- **Shared application code** — keep your UI in a platform-neutral library with separate runners for each target.
- **Material and Cupertino widgets** — build on Flutter-inspired APIs implemented and maintained in C#.
- **SkiaSharp-based GPU rendering** — access Skia Graphite from C# through SkiaSharp, using Vulkan, Metal, or WebGPU depending on the platform.
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
| Web | .NET WebAssembly, render Worker / canvas | `Doroti.Host.Web` | Skia Graphite / Dawn / WebGPU |
| Linux | Qt 6 `QWindow`, native C ABI bridge | `Doroti.Host.Qt` | Skia Graphite / Vulkan |

Windows App SDK also offers an explicitly selected ANGLE/D3D11 path; Web offers Ganesh/WebGL2. These implementations have different levels of validation; see [project status](#project-status).

## Get started

The sample application is the starting point for exploring Doroti. For Windows, install .NET SDK 10.0.400, PowerShell 7, and the Windows C++ build tools listed in the [platform prerequisites](Doroti/README.md#platform-prerequisites). Other targets have their own SDK and workload requirements.

Clone the repository and run the sample from its root:

```powershell
git clone https://github.com/Dynaruid/DorotiLab.git
cd DorotiLab

$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows
```

The command builds and launches the default Windows App SDK runner. Follow the [sample app guide](DorotiTestbedApp/README.md) for Android, iOS, macOS, Mac Catalyst, Linux, and Web commands, or the [framework guide](Doroti/README.md) for build and publish options.

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

The project began with a Dart-to-C# compiler to bootstrap framework code. Today, feature work happens directly in C#. The compiler remains an optional import and comparison tool.

## Documentation

| Guide | Contents |
| --- | --- |
| [Framework guide](Doroti/README.md) | SDKs, workloads, build commands, packaging, and host configuration |
| [Sample app guide](DorotiTestbedApp/README.md) | Platform launch commands, sample screens, renderer options, and troubleshooting |
| [Dart-to-C# compiler](tools/Doroti.DartToCSharp/README.md) | Optional source import and migration tooling |
| [Development history](history/) | Archived plans and validation records |

## Project status

Doroti is a personal, experimental project. The platform table describes implemented hosts and rendering defaults, not uniform production readiness.

Build checks, native or browser execution, and physical-device testing are tracked separately. GPU compatibility, input methods, accessibility, performance, signing, and store distribution still require platform-specific validation. Consult the [framework guide](Doroti/README.md#platform-evidence-boundaries) and recorded results before choosing a target.

Current priorities include native desktop integration, automated Web behavior checks, and representative release and physical-device testing for each target.

## Repository layout

| Path | Contents |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | Framework, runtime, rendering, hosts, and SDK |
| [`DorotiTestbedApp/`](DorotiTestbedApp/) | Shared sample application and platform runners |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` template |
| [`Doroti/eng/`](Doroti/eng/) | Build, run, packaging, and diagnostic tools |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Optional Dart-to-C# compiler |

## Feedback and contributions

Doroti is a personal hobby project that I build for fun. As a result, I may not be able to actively review or merge pull requests.

Ideas and bug reports are welcome, as are forks that use Doroti to experiment or explore new directions. When reporting a problem, include the platform, .NET SDK version, rendering backend, and steps to reproduce it.

## License

Doroti is licensed under the [BSD 3-Clause License](LICENSE). See [third-party notices](Doroti/THIRD-PARTY-NOTICES.md) for upstream attribution.

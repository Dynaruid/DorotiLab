# Doroti

**English** | [한국어](README.ko.md)

### A cross-platform UI framework built with C# and .NET

> [!WARNING]
> Doroti is currently experimental. Its APIs, architecture, behavior, and project structure may change significantly at any time without backward-compatibility guarantees.

Doroti brings a shared C# widget, layout, painting, semantics, and rendering pipeline to Windows, Android, iOS, native AppKit macOS, Mac Catalyst, Web, and an early Linux/Qt host boundary. Flutter remains the behavior reference for familiar Material and Cupertino APIs, while the maintained product implementation lives in `Doroti.Framework.*`.

Doroti does not embed Flutter in a WebView and does not compose its UI from a platform control tree. Platform hosts provide the native window/view, GPU surface, input, text, clipboard, and accessibility capabilities; Doroti owns the widget and render trees.

## Current development model

The project began by translating large Flutter source slices through a semantic compiler. That bootstrap made the current framework possible, but it is no longer the normal feature workflow.

Today `Doroti/src/Doroti.Framework.*` is product-owned C# source with matching `Doroti.Framework.*` namespaces, assemblies, and packages. Features and fixes are developed directly in the owning framework/runtime/renderer/host contract. The Dart-to-C# compiler and pinned Flutter checkout remain optional import and reference-differential tools; they never overwrite product source. `DorotiDemoApp` and generated `doroti-app` projects are C#-only, and active validation never creates a Dart package inside them.

See [ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md), [ADR-022](Doroti/docs/adr/ADR-022-default-native-platform-bridge.md), and the current [Windows host decision](Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md).

## What works today

- Shared Material/Cupertino widget, element, layout, paint, semantics, and state infrastructure
- A platform-neutral C# application library plus fixed-target runners; `macos` selects native AppKit and `maccatalyst` selects UIKit Mac Catalyst
- One public target-neutral `Program` startup, host-owned native initialization, and runner-local generated bootstrap code
- Windows defaults to a self-contained Windows App SDK 2.4 `HwndExactCpp` child-HWND host with managed hardware-D3D11 ANGLE/EGL and Skia; Windows MAUI remains an explicit independent backend
- Strict Skia GPU rendering through WebGL2 on the Web
- Automated fixed-runner builds include native AppKit macOS/osx-arm64 and the independently retained Mac Catalyst product
- Linux x64 uses a Qt 6 `QOpenGLWindow`, a versioned C ABI v2, and direct Skia rendering into the Qt framebuffer
- Package-only template creation includes both Apple desktop runners and their native bindings (twelve projects total)

Current evidence includes AppKit native launch/Metal presentation, Qt live runs under Wayland and XWayland on a Kubuntu VMware guest, and the Windows App SDK default cutover. The tested Windows physical resize and mixed-DPI monitor-boundary behavior received user acceptance, while strict synthetic capture/pixel/cadence failures remain failures. Physical Windows Korean IME/Narrator coverage, the broader Windows DPI/device/window-management matrix, physical Linux and a real X11 session, Linux Korean IME/Orca, context recreation, long-running performance, unrun target-specific native/browser/physical/accessibility/signing/store acceptance, and cross-target parity remain independent `notVerified` gates.

## Architecture

```text
product-owned Doroti.Framework.* source
                 │
                 ▼
       runtime + widget/render pipeline
                 │
                 ▼
        target host + GPU surface
  Windows App SDK/ANGLE · Windows MAUI · AppKit · Mac Catalyst
                 WebGL2 · Linux Qt/Skia GL
```

Flutter source is consulted when fidelity work needs a behavioral reference. Compiler output is an isolated candidate, not the product source of truth.

## Try it

Requires .NET SDK 10.0.400, matching 10.0.11 runtimes/workloads, and PowerShell 7.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -LastSuccessful
```

The Windows command selects Windows App SDK/`HwndExactCpp` by default. Use `-WindowsBackend Maui` only when the independent Windows MAUI runner is intended.

Windows App SDK now defaults to `Vulkan`; select `DOROTI_WINDOWS_PRESENTER=AngleD3D11` to use ANGLE explicitly. GPU selection defaults to `NoPreference` (system default). Set `DOROTI_WINDOWS_GPU_PREFERENCE` to `LowPowerPreference` or `HighPerformancePreference` for a Windows/DXGI preference; this applies to Vulkan and ANGLE. `DOROTI_WINDOWS_VULKAN_DEVICE` optionally overrides the Vulkan choice with an exact or unique device-name fragment. On Windows 11 24H2+, an app can request `new WindowBackdropOptions(WindowBackdropMode.acrylic)` without an experimental flag; omitted backdrop options and `system` remain opaque. The demo already requests Acrylic. Vulkan uses System32 Vulkan 1.1, dedicated D3D11-texture external memory, and Windows Presentation, with no automatic presenter fallback. Web defaults to `worker-canvaskit-webgl`, including `auto`; other renderers remain explicitly selectable. These defaults do not change the recorded validation results or complete the remaining GPU/DPI/refresh/IME/accessibility qualification.
`-LastSuccessful` (or `-NoBuild`) reuses only a prior successful artifact whose runner, configuration, RID, and source/native-input fingerprint still match; stale or missing state fails closed. `-NoRestore` skips restore without skipping the build.

## Repository layout

| Path | Description |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | Product framework, runtime, rendering, hosts, target packages, and SDK |
| [`DorotiDemoApp/`](DorotiDemoApp/) | Platform-workspace Material dogfood application |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` template |
| [`Doroti/eng/`](Doroti/eng/) | Build, SDK preparation, local-state, and optional diagnostic tools |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Optional Dart/Flutter import and migration compiler |
| [`history/`](history/) | Archived milestone plans, commands, and evidence summaries |

For detailed commands and evidence boundaries, see the [runtime README](Doroti/README.md).

## Roadmap

Current priorities are native desktop capability closure, automated Web live parity, and one representative release/physical acceptance flow per target. Build, native live, browser live, physical, and cross-target results are never substituted for one another.

Doroti is a personal project. Ideas, feedback, forks, and independent experiments are welcome.

## License

See [LICENSE](LICENSE) and [third-party notices](Doroti/THIRD-PARTY-NOTICES.md).

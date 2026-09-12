# Doroti

**English** | [한국어](README.ko.md)

### A cross-platform UI framework built with C# and .NET

> [!WARNING]
> Doroti is currently experimental. Its APIs, architecture, behavior, and project structure may change significantly at any time without backward-compatibility guarantees.

Doroti brings a shared C# widget, layout, painting, semantics, and rendering pipeline to Windows, Android, iOS, native AppKit macOS, Mac Catalyst, Web, and an early Linux/Qt host boundary. Flutter remains the behavior reference for familiar Material and Cupertino APIs, while the maintained product implementation lives in `Doroti.Framework.*`.

Doroti does not embed Flutter in a WebView and does not compose its UI from a platform control tree. Platform hosts provide the native window/view, GPU surface, input, text, clipboard, and accessibility capabilities; Doroti owns the widget and render trees.

## Current development model

The project began by translating large Flutter source slices through a semantic compiler. That bootstrap made the current framework possible, but it is no longer the normal feature workflow.

Today `Doroti/src/Doroti.Framework.*` is product-owned C# source with matching `Doroti.Framework.*` namespaces, assemblies, and packages. Features and fixes are developed directly in the owning framework/runtime/renderer/host contract. The Dart-to-C# compiler and pinned Flutter checkout remain optional import and reference-differential tools; they never overwrite product source. `DorotiTestbedApp` and generated `doroti-app` projects are C#-only, and active validation never creates a Dart package inside them.

See [ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md), [ADR-022](Doroti/docs/adr/ADR-022-default-native-platform-bridge.md), and the current [Windows host decision](Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md).

## What works today

- Shared Material/Cupertino widget, element, layout, paint, semantics, and state infrastructure
- A platform-neutral C# application library plus fixed-target runners; `macos` selects native AppKit and `maccatalyst` selects UIKit Mac Catalyst
- One public target-neutral `Program` startup, host-owned native initialization, and runner-local generated bootstrap code
- Windows defaults to a self-contained Windows App SDK 2.4 `HwndExactCpp` child-HWND host with managed Graphite/Vulkan and Windows Presentation; Windows MAUI remains an explicit independent backend
- Web defaults to Graphite/Dawn WebGPU, with explicit WebGL2 support
- Automated fixed-runner builds include native AppKit macOS/osx-arm64 and the independently retained Mac Catalyst product
- Linux x64 uses a Qt 6 `QWindow`, C ABI v2 with Vulkan surface negotiation, and Graphite/Vulkan swapchain output
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
  Windows App SDK/Vulkan · Windows MAUI · AppKit · Mac Catalyst
             WebGPU / WebGL2 · Linux Qt/Graphite Vulkan
```

Flutter source is consulted when fidelity work needs a behavioral reference. Compiler output is an isolated candidate, not the product source of truth.

## Try it

Requires .NET SDK 10.0.400, matching 10.0.11 runtimes/workloads, and PowerShell 7.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows -LastSuccessful
```

The Windows command selects Windows App SDK/`HwndExactCpp` by default. Use `-WindowsBackend Maui` only when the independent Windows MAUI runner is intended.

Windows App SDK now defaults to `Vulkan`; select `DOROTI_WINDOWS_PRESENTER=AngleD3D11` to use ANGLE explicitly. GPU selection defaults to `NoPreference` (system default). Set `DOROTI_WINDOWS_GPU_PREFERENCE` to `LowPowerPreference` or `HighPerformancePreference` for a Windows/DXGI preference; this applies to Vulkan and ANGLE. `DOROTI_WINDOWS_VULKAN_DEVICE` optionally overrides the Vulkan choice with an exact or unique device-name fragment. On Windows 11 24H2+, an app can request `new WindowBackdropOptions(WindowBackdropMode.acrylic)` without an experimental flag; omitted backdrop options and `system` remain opaque. The demo already requests Acrylic. Vulkan uses System32 Vulkan 1.2, dedicated D3D11-texture external memory, and Windows Presentation, with no automatic presenter fallback. Web defaults to `worker-direct-webgpu` (Graphite/Dawn), including `auto`; `worker-direct-webgl` remains explicitly selectable. Graphite is now the user-selected native default: Vulkan on Windows App SDK, Windows MAUI, Android and Linux Qt; Metal on AppKit, iOS and Mac Catalyst. Android requires API 24 and a Vulkan 1.2 device. Native Vulkan assets come from official SkiaSharp NativeAssets 4.154.0-preview.1.26454.9 packages. No Skia source build, custom staging, or private Graphite ABI is used. The deployed native and managed package hashes are checked before use; there is no automatic fallback. NVIDIA's delayed prepared-frame receipt has a user-accepted 1,000 ms budget; other GPUs keep 50 ms and missing receipts still fail. See [default configuration and validation](Doroti/docs/validation/official-graphite-cutover-2026-09-12.md) for per-host results and explicit comparison switches. macOS/Mac Catalyst checks were resumed; see the [Apple review](history/2026-09-13/apple-work0/README.md) for Release builds, product rendering, official assets, lifecycle results and remaining gates. iOS execution remains unverified. Linux build, packaging and Qt ABI checks were resumed; llvmpipe Graphite runs without environment overrides. Renderer admission checks API capabilities, not hardware/software device classification. Headless correctness passes, while Material product scenes report depth synchronization errors; hardware qualification remains unverified. See the [Linux follow-up](Doroti/docs/validation/linux-official-graphite-2026-09-12.md). VMware SVGA3D GPU-backed OpenGL is reconfirmed; the available CPU Vulkan ICD is separate diagnostic evidence. These defaults do not change the recorded validation results or complete the remaining GPU/DPI/refresh/IME/accessibility qualification.
`-LastSuccessful` (or `-NoBuild`) requires a v3 success record matching the runner, configuration, RID, source/native inputs, restored dependency contents, toolchain, and output hashes. Ordinary build/run restores before collecting dependency identities and rebuilds when dependencies/toolchains changed or were not previously tracked. Old records require a normal build/run first. `-NoRestore` skips restore without skipping dependency verification or the build.

## Repository layout

| Path | Description |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | Product framework, runtime, rendering, hosts, target packages, and SDK |
| [`DorotiTestbedApp/`](DorotiTestbedApp/) | Platform-workspace Material dogfood application |
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

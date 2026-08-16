# Doroti

**English** | [한국어](README.ko.md)

### A cross-platform UI framework built with C# and .NET

Doroti brings a shared C# widget, layout, painting, semantics, and rendering pipeline to Windows, Mac Catalyst, and the Web. Flutter remains the behavior reference for familiar Material and Cupertino APIs, while the maintained product implementation lives in `Doroti.Framework.*`.

Doroti does not embed Flutter in a WebView and does not compose its UI from MAUI or Avalonia controls. Platform hosts provide the native window/view, GPU surface, input, text, clipboard, and accessibility capabilities; Doroti owns the widget and render trees.

## Current development model

The project began by translating large Flutter source slices through a semantic compiler. That bootstrap made the current framework possible, but it is no longer the normal feature workflow.

Today `Doroti/src/Doroti.Framework.*` is product-owned C# source with matching `Doroti.Framework.*` namespaces, assemblies, and packages. Features and fixes are developed directly in the owning framework/runtime/renderer/host contract. The Dart-to-C# compiler and pinned Flutter checkout remain optional import and reference-differential tools; they never overwrite product source. `DorotiDemoApp` and generated `doroti-app` projects are C#-only, and active validation never creates a Dart package inside them.

See [ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md) and the active [work list](work.md).

## What works today

- Shared Material/Cupertino widget, element, layout, paint, semantics, and state infrastructure
- A single C# application project targeting MAUI Windows x64, MAUI Mac Catalyst arm64, or Blazor WebAssembly
- Strict Skia GPU rendering through WinUI 3 `MauiSKSwapChainPanel` on Windows and WebGL2 on the Web
- Windows Release build/publish and actual GPU-frame evidence
- Web external template/package compile and publish evidence, plus a previously recorded manual Chromium canvas/basic-pointer smoke

Native hover/wheel/keyboard/IME/UIA, Mac Catalyst native execution, automated Web interaction, physical acceptance, and cross-target parity remain independent `notVerified` gates. Historical Win32/WGL and AppKit/NSOpenGL evidence is predecessor evidence only.

## Architecture

```text
product-owned Doroti.Framework.* source
                 │
                 ▼
       runtime + widget/render pipeline
                 │
                 ▼
        target host + GPU surface
  Windows MAUI · Mac Catalyst · WebGL2
```

Flutter source is consulted when fidelity work needs a behavioral reference. Compiler output is an isolated candidate, not the product source of truth.

## Try it

Requires .NET 10 and PowerShell 7.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
```

Run the integrated Windows GPU and Web package/publish scenario before a release claim:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

## Repository layout

| Path | Description |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | Product framework, runtime, rendering, hosts, target packages, and SDK |
| [`DorotiDemoApp/`](DorotiDemoApp/) | Single-project Material dogfood application |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` template |
| [`Doroti/eng/`](Doroti/eng/) | Compact build, validation, release, and optional reference workflows |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Optional Dart/Flutter import and migration compiler |
| [`history/`](history/) | Archived milestone plans, commands, and evidence summaries |

For detailed commands and evidence boundaries, see the [runtime README](Doroti/README.md).

## Roadmap

Current priorities are native desktop capability closure, automated Web live parity, and one representative release/physical acceptance flow per target. Build, native live, browser live, physical, and cross-target results are never substituted for one another.

Doroti is an experimental personal project. Ideas, feedback, forks, and independent experiments are welcome.

## License

See [LICENSE](LICENSE) and [third-party notices](Doroti/THIRD-PARTY-NOTICES.md).

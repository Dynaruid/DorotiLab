# Doroti runtime and framework

**English** | [한국어](README.ko.md)

This directory contains Doroti's product runtime: the C# framework packages generated and reviewed from pinned Flutter source, the widget/rendering lifecycle, the Skia renderer, and the native platform hosts.

Doroti does not embed Flutter in a WebView and does not build its UI with Avalonia controls or XAML. The Flutter framework defines the behavioral model, the Doroti runtime executes it on .NET, and the platform host connects it to native windows, input, accessibility, and graphics.

## Current status

The current product gate is an automated **Windows x64** native run. A reviewed Material widget tree can construct, mount, lay out, paint, present, and respond to input in an actual HWND using the strict `skia-wgl-opengl-gpu` backend.

The validated slice includes `MaterialApp`, `Theme`, `Navigator`, `Scaffold`, `AppBar`, `Card`, `ListTile`, buttons and selection controls, common layout widgets, scrolling, a lazy list, local state updates, and native accessibility actions. Both `MaterialApp.builder` and `MaterialApp.home` entry paths are covered.

Linux, macOS, Web, Android, iOS, and physical-device validation remain roadmap work. A project or package compiling for a target is not treated as proof that native presentation and interaction work there.

See the repository [Goal 7 roadmap](../goal7.md) for the current milestone, Web build, and evidence requirements.

## Architecture

```text
pinned Flutter source
        │
        ▼
Doroti.DartToCSharp ──► reviewed C# framework packages
                              │
                              ▼
                 Flutter runtime and hosting
                              │
                              ▼
                  widget / render pipeline
                              │
                              ▼
              target package and native host
        (Windows: Avalonia-derived platform source)
```

- `Doroti.Flutter.Framework.*` contains generated and reviewed framework libraries.
- `Doroti.Flutter.Runtime`, `Doroti.Flutter.Ui`, and `Doroti.Flutter.Hosting` provide Dart/Flutter runtime semantics and application bootstrap.
- `Doroti.Engine`, `Doroti.Rendering`, and `Doroti.Graphics` own frame scheduling, display output, and graphics contracts.
- `Doroti.Host.Desktop` and `Doroti.Target.Windows.win-x64` connect the product to Win32 and strict GPU presentation.
- `Doroti.Host.Avalonia` is a comparison host, not the default product composition root.

Selected Avalonia platform code is adapted behind Doroti contracts and tracked by provenance manifests. Applications do not take a runtime dependency on Avalonia controls.

## Prerequisites

- .NET SDK **10.0.300** or a compatible latest patch, as pinned in [`global.json`](global.json)
- PowerShell 7 (`pwsh`) for the repository workflows
- Dart SDK for compiler and source-audit workflows
- The pinned `flutter-master` and `Avalonia-main` reference checkouts at the repository root
- Windows x64 for the native demo and live UI Automation gates

The reference checkouts are source and behavior inputs; they are not product runtime dependencies.

## Quick start

Run these commands from the repository root:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build
dotnet run --project ./DorotiDemoApp
```

For a short deterministic smoke run on Windows x64:

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry builder --frames 3 --duration-ms 15000
```

## Repository commands

`eng/doroti.ps1` is the main development entry point:

| Command | Purpose |
| --- | --- |
| `doctor` | Check SDKs, desktop backend availability, pinned sources, revisions, and licenses |
| `build` | Build the lean product graph in `Doroti.Product.slnx` |
| `validate` | Run the compiler validation suite |
| `audit` | Run storage, source/provenance, and compiler audits |
| `format` | Verify product formatting without modifying files |
| `release` | Build, audit, pack, and test the package-only external consumer |
| `clean` | Remove Doroti build output, artifacts, and temporary local state |

Examples:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 audit
pwsh -File ./Doroti/eng/doroti.ps1 format
```

The complete G6-3 Material gate is Windows-only and includes both app entry paths, external UI Automation, a 30-second/300-frame cadence run, screenshot geometry, compiler regressions, and a clean package consumer:

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

Generated evidence is written under `migration/flutter-framework/`; transient screenshots and run artifacts are written under `artifacts/`.

## Directory guide

| Path | Contents |
| --- | --- |
| [`src/`](src/) | Product packages, runtime, renderer, hosts, target packages, and analyzers |
| [`validation/`](validation/) | Current compiler, managed behavior, and native validation executables |
| [`migration/`](migration/) | Source selections, promotion inputs, provenance, baselines, and committed evidence |
| [`eng/`](eng/) | Build, audit, promotion, validation, and local-storage workflows |
| [`tools/`](tools/) | Source, behavior, scene, provenance, and porting tools |
| [`samples/`](samples/) | Host comparison and diagnostic samples |
| [`templates/`](templates/) | `dotnet new` templates and package metadata |
| [`docs/`](docs/) | Architecture decisions and milestone design records |

`Doroti.Product.slnx` is the lean product build. `Doroti.slnx` additionally includes tools, validations, the demo, samples, and historical projects used for repository work.

## Generated-source policy

Generated output is reviewable evidence, not a place for hidden fixes. Shared semantic problems are fixed in the analyzer, compiler, or runtime and then regenerated.

- Selection manifests live in `migration/selections/`.
- Manual replacements and platform ports live in `migration/ports/`.
- Compiler-owned `generated-base/`, `manual-snapshot/`, and `effective/` trees must not be edited directly.
- Promoted source must retain its source map, provenance, source revision, and license trail.
- All repository JSON uses `System.Text.Json`; do not introduce `Newtonsoft.Json`.

Start with [typed framework compiler](docs/architecture/f0-typed-framework-compiler.md), [multi-library compilation](docs/architecture/g3-1-multi-library-framework-compiler.md), [Windows RID packaging](docs/architecture/g5-6w-windows-rid-package.md), and [port ownership](docs/architecture/p0-port-ownership.md) for deeper design context.

## Related projects and license

Doroti uses [Flutter](https://github.com/flutter/flutter) as its framework behavior reference and selected [Avalonia](https://github.com/AvaloniaUI/Avalonia) platform implementations as the basis of the Windows host.

Doroti is distributed under the repository's [BSD 3-Clause license](../LICENSE). See [third-party notices](THIRD-PARTY-NOTICES.md) for upstream source, package, revision, and license details.

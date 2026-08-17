# Doroti runtime and framework

**English** | [한국어](README.ko.md)

Doroti is a C#/.NET UI framework with a shared widget, layout, painting, semantics, and rendering pipeline for MAUI Windows, MAUI Mac Catalyst, and Blazor WebAssembly.

## Development model

`src/Doroti.Framework.*` is maintained product source. Its public namespaces are `Doroti.Framework.*`, matching the project, assembly, and package names. Add features and fix correctness directly in the owning framework/runtime/host project, then update every consumer of the shared contract.

The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-reference tools. They do not overwrite product source and are not required for ordinary builds. Compiler candidates stay in isolated workspaces or `migration/` until explicitly reviewed and adopted.

See [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md) for the ownership decision and the root [work list](../work.md) for active priorities.

## Current product boundary

- `Doroti.Framework.*`: product-owned Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, and Material libraries
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: runtime semantics plus the target-neutral startup/builder/descriptor contract
- `Doroti.Engine`, `Doroti.Rendering`, `Doroti.Graphics`: frame scheduling, display output, and graphics contracts
- `Doroti.App.Sdk`: one-project target selection plus generated native/Web bootstrap and plugin registration
- `Doroti.Host.Maui`: host-owned MAUI application/page lifecycle and `SKGLView` GPU-surface integration
- `Doroti.Host.Web`: host-owned Blazor composition, WebGL2 canvas, input, accessibility, and resource bridge

Windows Release build/publish and an actual `MauiSKSwapChainPanel` GPU frame are verified. Web package-only compile/publish and a Chromium canvas/basic-FAB-pointer smoke are verified on runtime 10.0.11. Native hover/wheel/keyboard/IME/UIA, Mac Catalyst native execution, Web keyboard/IME/clipboard/resize/interactive ARIA, physical acceptance, and cross-target parity remain separate `notVerified` gates.

## Requirements

- .NET SDK 10.0.400 or a compatible patch, pinned by [global.json](global.json)
- PowerShell 7
- .NET/ASP.NET/WindowsDesktop and browser-wasm runtime packs at 10.0.11, with matching MAUI/WebAssembly workloads

The `reference/flutter-master` and `reference/Avalonia-main` checkouts are needed only for explicit reference comparison or migration work. Prepare Flutter for such work with `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`.

## Commands

Run from the repository root:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
```

The active command surface is intentionally small:

| Command | Purpose |
| --- | --- |
| `doctor` | Check required .NET/PowerShell tools and report optional reference checkouts |
| `build` | Build `Doroti.Product.slnx` |
| `validate` | Run source ownership, Release build, and application target graph/build checks |
| `validate -ValidationSuite Release` | Add Windows GPU live and external Web template/package publish scenarios |
| `audit` | Check repository-local storage and current source ownership |
| `migration-audit` | Explicitly run compiler, upstream selection, and provenance audits |
| `release` | Run the integrated release suite, audit, pack, and package inspection |
| `clean` | Remove Doroti build output, artifacts, and temporary local state |

Direct suite entry points are [validate.ps1](eng/validate.ps1), [validate-app-targets.ps1](eng/validate-app-targets.ps1), and [validate-web-product.ps1](eng/validate-web-product.ps1). Historical G4-G7 validators are no longer active; their results remain under `history/` at the repository root.

## Source and artifact policy

- Product framework changes belong in `src/Doroti.Framework.*`; no compiler-owned `.g.cs` file is compiled there.
- Fix shared behavior at the lowest owning framework/runtime/rendering/host contract.
- Keep reference comparison, build, native live, browser live, physical, and cross-target claims distinct.
- `migration/` stores provenance, historical selections, reviewed import inputs, and committed evidence.
- `.doroti/` and `artifacts/` store transient tool and validation output.
- All repository JSON uses `System.Text.Json`.

## Directory guide

| Path | Contents |
| --- | --- |
| [`src/`](src/) | Product framework, runtime, renderer, hosts, targets, SDK, and analyzers |
| [`templates/`](templates/) | The single-project `doroti-app` template |
| [`eng/`](eng/) | Compact build, validation, release, storage, and optional reference workflows |
| [`tools/`](tools/) | Source/provenance and diagnostic tooling |
| [`migration/`](migration/) | Historical conversion inputs, provenance, and evidence |
| [`docs/`](docs/) | Current ADRs plus historical architecture records |

Doroti is distributed under the repository BSD 3-Clause license. See [third-party notices](THIRD-PARTY-NOTICES.md) for upstream source and package attribution.

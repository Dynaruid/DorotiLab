# Doroti runtime and framework

**English** | [한국어](README.ko.md)

Doroti is a C#/.NET UI framework with a shared widget, layout, painting, semantics, and rendering pipeline for MAUI Windows, MAUI Mac Catalyst, MAUI Android, and Blazor WebAssembly.

## Development model

`src/Doroti.Framework.*` is maintained product source. Its public namespaces are `Doroti.Framework.*`, matching the project, assembly, and package names. Add features and fix correctness directly in the owning framework/runtime/host project, then update every consumer of the shared contract.

The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-reference tools. They do not overwrite product source and are not required for ordinary builds. Compiler candidates stay in isolated workspaces or `migration/` until explicitly reviewed and adopted.

See [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md) for the ownership decision and the root [work list](../work.md) for active priorities.

## Current product boundary

- `Doroti.Framework.*`: product-owned Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, and Material libraries
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: runtime semantics plus the target-neutral startup/builder/descriptor contract
- `Doroti.Engine`, `Doroti.Rendering`, `Doroti.Graphics`: frame scheduling, display output, and graphics contracts
- `Doroti.App.Sdk`: one-project Windows/Mac Catalyst/Android/Web selection plus generated native/Web bootstrap and plugin registration
- `Doroti.Skia.RuntimeEffects`: shared fail-closed SkSL compiler and uniform/image-sampler binder used by native and Web hosts
- `Doroti.Host.Maui`: host-owned MAUI application/page lifecycle and `SKGLView` GPU-surface integration
- `Doroti.Host.Web`: host-owned Blazor composition, WebGL2 canvas, input, accessibility, and resource bridge

Web execution source is TypeScript-owned. Applications edit `Platforms/Web/src/**/*.ts`; Doroti owns `src/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` 7.0.0 compiles both into target/configuration-specific `obj` directories, and publish contains only the resulting JavaScript. Node, npm, Bun, and a bundler are not application requirements. See [ADR-020](docs/adr/ADR-020-web-typescript-bootstrap.md).

Windows Release build/publish and an actual `MauiSKSwapChainPanel` GPU frame are verified. Android arm64 APK/AAB build and an automated physical-device `MauiSKGLTextureView` OpenGL ES custom-SkSL frame/replay are verified. The Android x64 target is also built and repeatedly scrolled on an x86_64 emulator with persistent visible-content screenshot evidence. Web compile/publish and Mac Catalyst cross-build are verified; fresh Web and Mac native custom-shader presentation remain separate `notVerified` gates.

## Requirements

- .NET SDK 10.0.400 or a compatible patch, pinned by [global.json](global.json)
- PowerShell 7
- .NET/ASP.NET/WindowsDesktop and browser-wasm runtime packs at 10.0.11, with matching MAUI/WebAssembly workloads
- `Microsoft.TypeScript.MSBuild` 7.0.0 restored only for Web projects that contain `Platforms/Web/tsconfig.json`

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

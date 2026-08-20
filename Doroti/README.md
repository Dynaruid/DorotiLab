# Doroti runtime and framework

**English** | [한국어](README.ko.md)

Doroti is a C#/.NET UI framework with a shared widget, layout, painting, semantics, and rendering pipeline for MAUI Windows, Mac Catalyst, Android, iOS, Blazor WebAssembly, and an early Linux/Qt host boundary.

## Development model

`src/Doroti.Framework.*` is maintained product source. Its public namespaces are `Doroti.Framework.*`, matching the project, assembly, and package names. Add features and fix correctness directly in the owning framework/runtime/host project, then update every consumer of the shared contract.

The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-reference tools. They do not overwrite product source and are not required for ordinary builds. Compiler output stays in isolated workspaces until explicitly reviewed and adopted.

See [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md) for source ownership and [ADR-022](docs/adr/ADR-022-default-native-platform-bridge.md) for the default native bridge graph.

## Current product boundary

- `Doroti.Framework.*`: product-owned Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, and Material libraries
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: runtime semantics plus the target-neutral startup/builder/descriptor contract
- `Doroti.App.Sdk`: platform-neutral `net10.0` application assembly and shared asset contract
- `Doroti.Runner.Sdk`: fixed-target runner validation plus runner-local native/Web bootstrap and plugin registration
- `Doroti.Skia.RuntimeEffects`: shared fail-closed SkSL compiler and uniform/image-sampler binder used by native and Web hosts
- `Doroti.Host.Maui`: host-owned MAUI application/page lifecycle and `SKGLView` GPU-surface integration
- `Doroti.Host.Web`: host-owned Blazor composition, WebGL2 canvas, input, accessibility, and resource bridge
- `Doroti.Host.Qt`: managed-owned Linux runner with a Qt 6 `QOpenGLWindow`, versioned C ABI, GPU surface, input, IME, and desktop services
- `Doroti.Skia.Rendering`: host-neutral Skia scene, paragraph, image, runtime-effect, semantics, cache, and terminal-ACK renderer shared by native GPU hosts
- `Doroti.Host.Qt`: managed-owned Linux process and Qt 6 C ABI boundary; native rendering and X11/Wayland acceptance remain `notVerified`

Web execution source is TypeScript-owned. Applications edit `web/src/**/*.ts`; Doroti owns `src/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` 7.0.0 compiles both into runner-local `obj` directories, and publish contains only the resulting JavaScript. Node, npm, Bun, and a bundler are not application requirements. See [ADR-020](docs/adr/ADR-020-web-typescript-bootstrap.md).

Material applications follow system dark mode with `MaterialApp(theme:, darkTheme:, themeMode: ThemeMode.system)`. Build both palettes with `ColorScheme.CreateFromSeed`, `Brightness.light`/`Brightness.dark`, and optional role overrides such as `surface`, `primary`, or `outline`; widgets read the active roles from `Theme.of(context).colorScheme`. See the [DorotiDemoApp dark-mode guide](../DorotiDemoApp/README.md#system-dark-mode-and-color-palettes) for the MAUI/Web change flow and a complete example.

Android, iOS, and Mac Catalyst runners each reference a default app-owned native binding. Android uses `AndroidGradleProject` to build an AAR; iOS and Mac Catalyst use separate `XcodeProject` frameworks. The .NET runner still owns the final app. Managed and Windows cross-build results do not prove Android Studio/Xcode execution, native launch, device behavior, signing, or archive; those gates remain `notVerified` until run on the required host.

## Requirements

- .NET SDK 10.0.400 or a compatible patch, pinned by [global.json](global.json)
- PowerShell 7
- .NET/ASP.NET/WindowsDesktop and browser-wasm runtime packs at 10.0.11, with matching MAUI/WebAssembly workloads
- `Microsoft.TypeScript.MSBuild` 7.0.0 restored only by a Web runner that contains `web/tsconfig.json`

The `reference/flutter-master` checkout is needed only for explicit Flutter reference comparison. Prepare Flutter for that work with `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`.

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
| `build/run/publish -App <path> -Platform <alias>` | Resolve and execute one runner from `doroti-workspace.json` |
| `native doctor\|build\|open\|add -App <path> -Platform android\|ios\|macos` | Inspect, build, locate, or extend the default native bridge workspace |
| `validate` | Run source ownership, Release build, and application target graph/build checks |
| `validate -ValidationSuite Release` | Add Windows GPU live and external Web template/package publish scenarios |
| `audit` | Check repository-local storage and current source ownership |
| `release` | Run the integrated release suite, audit, pack, and package inspection |
| `clean` | Remove Doroti build output, artifacts, and temporary local state |

Direct suite entry points are [validate.ps1](eng/validate.ps1), [validate-app-targets.ps1](eng/validate-app-targets.ps1), [validate-web-product.ps1](eng/validate-web-product.ps1), and the Kubuntu-native [validate-linux-qt.sh](eng/validate-linux-qt.sh). Historical G4-G7 validators are no longer active; their results remain under `history/` at the repository root.

## Source and artifact policy

- Product framework changes belong in `src/Doroti.Framework.*`; no compiler-owned `.g.cs` file is compiled there.
- Fix shared behavior at the lowest owning framework/runtime/rendering/host contract.
- Keep reference comparison, build, native live, browser live, physical, and cross-target claims distinct.
- `validation/contracts/` stores small machine-readable contracts consumed by active validators.
- `validation/evidence/` stores committed summaries produced by active target and Web validators.
- `.doroti/` and `artifacts/` store transient tool and validation output.
- All repository JSON uses `System.Text.Json`.

## Directory guide

| Path | Contents |
| --- | --- |
| [`src/`](src/) | Product framework, runtime, renderer, hosts, targets, SDK, and analyzers |
| [`templates/`](templates/) | The six-runner plus three-binding `doroti-app` platform workspace template |
| [`eng/`](eng/) | Compact build, validation, release, storage, and optional reference workflows |
| [`tools/`](tools/) | Optional Dart/Flutter compiler and shared tooling |
| [`validation/`](validation/) | Active validation contracts, fixtures, and committed evidence |
| [`docs/`](docs/) | Current ADRs plus historical architecture records |

Doroti is distributed under the repository BSD 3-Clause license. See [third-party notices](THIRD-PARTY-NOTICES.md) for upstream source and package attribution.

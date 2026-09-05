# Doroti runtime and framework

**English** | [한국어](README.ko.md)

Doroti is a C#/.NET UI framework with a shared widget, layout, painting, semantics, and rendering pipeline for Windows App SDK, optional Windows MAUI, native AppKit macOS, Mac Catalyst, Android, iOS, Blazor WebAssembly, and Linux/Qt.

## Development model

`src/Doroti.Framework.*` is maintained product source. Its public namespaces are `Doroti.Framework.*`, matching the project, assembly, and package names. Add features and fix correctness directly in the owning framework/runtime/host project, then update every consumer of the shared contract.

The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-reference tools. They do not overwrite product source and are not required for ordinary builds. Compiler output stays in isolated workspaces until explicitly reviewed and adopted.

See [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md) for source ownership, [ADR-022](docs/adr/ADR-022-default-native-platform-bridge.md) for the default native bridge graph, and [ADR-025](docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md) for the current Windows host.

## Current product boundary

- `Doroti.Framework.*`: product-owned Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, and Material libraries
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: runtime semantics plus the target-neutral startup/builder/descriptor contract
- `Doroti.App.Sdk`: platform-neutral `net10.0` application assembly and shared asset contract
- `Doroti.Runner.Sdk`: fixed-target runner validation plus runner-local native/Web bootstrap and plugin registration
- `Doroti.Skia.RuntimeEffects`: shared fail-closed SkSL compiler and uniform/image-sampler binder used by native and Web hosts
- `Doroti.Skia.Rendering`: host-neutral scene, paragraph, image, runtime-effect, semantics, cache, and terminal-ACK renderer shared by native GPU hosts
- `Doroti.Host.WindowsAppSdk` + `Doroti.Host.WindowsAppSdk.Native`: default Windows App SDK 2.4 host; native C++ owns the top-level/child/task HWNDs and ingress, while managed code owns the Doroti framework and Vulkan/Skia presentation (ANGLE remains selectable)
- `Doroti.Target.Windows.WindowsAppSdk.win-x64`: self-contained unpackaged Windows target with `HwndExactCpp`, native host/bootstrap, and app-directory ANGLE runtime
- `Doroti.Host.Maui`: MAUI lifecycle and SKGLView/AppKit-owned MTKView/Metal adapters for Android, iOS, Mac Catalyst, AppKit, and the explicit alternative Windows MAUI backend
- `Doroti.Host.Web`: host-owned Blazor composition, WebGL2 canvas, input, accessibility, and resource bridge
- `Doroti.Host.Qt`: managed-owned Linux process with a Qt 6 `QOpenGLWindow`, versioned C ABI v2, GPU surface, input, IME, desktop services, and an accessibility adapter

Web execution source is TypeScript-owned. Applications edit `web/src/**/*.ts`; Doroti owns `src/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` 7.0.0 compiles both into runner-local `obj` directories, and publish contains only the resulting JavaScript. Node, npm, Bun, and a bundler are not application requirements. The opt-in `worker-direct-webgl` qualification backend transfers the visible canvas once and keeps .NET, Skia, WebGL2, and Worker rAF in one persistent Worker; `auto` defaults to the split UI/Raster Worker `worker-canvaskit-webgl` backend. See [ADR-020](docs/adr/ADR-020-web-typescript-bootstrap.md).

Material applications follow system dark mode with `MaterialApp(theme:, darkTheme:, themeMode: ThemeMode.system)`. Build both palettes with `ColorScheme.CreateFromSeed`, `Brightness.light`/`Brightness.dark`, and optional role overrides such as `surface`, `primary`, or `outline`; widgets read the active roles from `Theme.of(context).colorScheme`. See the [DorotiDemoApp dark-mode guide](../DorotiDemoApp/README.md#system-dark-mode-and-color-palettes) for the MAUI/Web change flow and a complete example.

Android, iOS, native AppKit macOS, and Mac Catalyst runners each reference a default app-owned native binding. Android uses `AndroidGradleProject`; each Apple product has an explicit `XcodeProject` binding contract. The .NET runner still owns the final app. Build results do not prove native launch, device behavior, accessibility, signing, or archive; those gates remain `notVerified` until run.

## Requirements

- .NET SDK 10.0.400 or a compatible patch, pinned by [global.json](global.json)
- PowerShell 7
- .NET/ASP.NET/WindowsDesktop and browser-wasm runtime packs at 10.0.11, with matching MAUI/WebAssembly workloads
- `Microsoft.TypeScript.MSBuild` 7.0.0 restored only by a Web runner that contains `web/tsconfig.json`

Building the default Windows target also requires Visual Studio MSBuild with the MSVC v145 C++ toolset and Windows SDK 10.0.26100.0. Windows App SDK 2.4 and the ANGLE runtime are restored and deployed self-contained with the target; no machine-wide Windows App Runtime or presenter fallback is assumed.

The Linux runner uses system dependencies on a Linux x64 host: Qt 6.5 or newer Core/Gui/Widgets/OpenGL, CMake, a C++ compiler, `pkg-config`, Wayland client development files, `wayland-scanner`, and the QPA plugin used at runtime (`wayland` or `xcb`).

The `reference/flutter-master` checkout is needed only for explicit Flutter reference comparison. Prepare Flutter for that work with `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`.

## Commands

Run from the repository root:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
```

The active command surface is intentionally small:

| Command | Purpose |
| --- | --- |
| `doctor` | Check required .NET/PowerShell tools and report optional reference checkouts |
| `build` | Build `Doroti.Product.slnx` |
| `build/run/publish -App <path> -Platform <alias>` | Resolve and execute one runner from `doroti-workspace.json` |
| `native doctor\|build\|open\|add -App <path> -Platform android\|ios\|macos\|maccatalyst` | Inspect, build, locate, or extend the default native bridge workspace |
| `validate -ValidationSuite <suite>` | Run the supported aggregate validation entry point (`Developer` by default) |
| `audit` | Check local-storage policy and source validation |
| `release` | Run Release validation/audit and pack product artifacts |
| `clean` | Remove Doroti build output, artifacts, and temporary local state |

For Windows, `-Platform windows` selects Windows App SDK/`HwndExactCpp`; add `-WindowsBackend Maui` to select the independent MAUI runner. Target-specific scripts under `eng/` are maintainer diagnostics, not interchangeable product commands. Their contracts and evidence boundaries are described under [validation](validation/README.md), while previous run results remain under `history/` at the repository root.

Windows App SDK now defaults to `Vulkan`; select `DOROTI_WINDOWS_PRESENTER=AngleD3D11` to use ANGLE explicitly. Set `DOROTI_WINDOWS_VULKAN_DEVICE` to an exact or unique device-name fragment when multiple capable GPUs are eligible. On Windows 11 24H2+, an app can request `new WindowBackdropOptions(WindowBackdropMode.acrylic)` without an experimental flag; omitted backdrop options and `system` remain opaque. The demo already requests Acrylic. Vulkan uses System32 Vulkan 1.1, dedicated D3D11-texture external memory, and Windows Presentation, with no automatic presenter fallback. Web defaults to `worker-canvaskit-webgl`, including `auto`; other renderers remain explicitly selectable. These defaults do not change the recorded validation results or complete the remaining GPU/DPI/refresh/IME/accessibility qualification.

Vulkan moving-origin resize submits a prepared frame immediately after the HWND geometry change and waits for its CompositionFrame receipt. Implementation details, earlier failures, observed resize improvement, and validation limits are preserved in the [September 5 history](../history/26-09-05/windows-vulkan-acrylic-resize-summary.md). `experimentalAcrylic` remains a compatibility mode using the same Acrylic implementation.

The Material demo requests ordinary `acrylic` on Windows and draws its translucent Material surface once over a transparent renderer background. `DOROTI_DEMO_EXPERIMENTAL_ACRYLIC=1` is needed only to reproduce the legacy mode. Runtime kind/theme/tint/luminosity updates and state queries retain the existing `doroti/windows/experimental-acrylic` platform channel for compatibility. App content must be transparent or translucent for Acrylic to be visible.

## Platform evidence boundaries

The Windows App SDK target, package, default CLI route, hardware-D3D11 ANGLE runtime, first-frame ordering, and tested physical resize/mixed-DPI boundary behavior have current evidence. C10 is a user-acceptance PASS for the observed opaque conditions; strict synthetic resize qualification and pixel/cadence failures remain failures. Experimental Acrylic automation and its physical acceptance are separate evidence classes: any unexecuted DPI, refresh, edge/speed, monitor, scan-out, IME, accessibility, window-management, or device-loss combination remains `notVerified`. The last full `Doroti.Product.slnx` Release run on Windows failed only after the Windows target passed, when a macOS project invoked unavailable `sips`; the Windows PASS and global FAIL remain separate.

The shared renderer, real Material gallery, swap-based terminal ACK, basic input callbacks, semantics tree, and framework-dependent/self-contained publish paths were exercised for Linux Qt under Wayland and XWayland on a Kubuntu 26.04 VMware guest. Physical Linux, a real X11 session, Korean IME/Orca, forced context recreation, long soaks, and performance remain `notVerified`. See the archived [Linux Qt backend summary](../history/26-08-20/linux-qt-backend-summary.md).

AppKit live coverage and its remaining gates are recorded separately in the archived [AppKit dual-backend summary](../history/26-08-20/macos-appkit-dual-backend-summary.md). Build, native-live, browser-live, physical/device, and accessibility evidence do not substitute for one another.

## Source and artifact policy

- Product framework changes belong in `src/Doroti.Framework.*`; no compiler-owned `.g.cs` file is compiled there.
- Fix shared behavior at the lowest owning framework/runtime/rendering/host contract.
- Keep reference comparison, build, native live, browser live, physical, and cross-target claims distinct.
- `validation/contracts/` stores small machine-readable contracts consumed by active validators.
- `validation/evidence/` is reserved for deliberately committed machine-readable summaries; it is currently empty.
- `.doroti/` and `artifacts/` store transient tool and validation output.
- All repository JSON uses `System.Text.Json`.

## Directory guide

| Path | Contents |
| --- | --- |
| [`src/`](src/) | Product framework, runtime, renderer, hosts, targets, SDK, and analyzers |
| [`templates/`](templates/) | The seven-runner plus four-binding `doroti-app` platform workspace template |
| [`eng/`](eng/) | Compact build, validation, release, storage, and optional reference workflows |
| [`tools/`](tools/) | Optional Dart/Flutter compiler and shared tooling |
| [`validation/`](validation/) | Active validation contracts and fixtures; generated evidence goes to `.doroti/` or `artifacts/` unless explicitly promoted |
| [`docs/`](docs/) | Current ADRs, including the Windows host decision, plus historical architecture records |

Doroti is distributed under the repository BSD 3-Clause license. See [third-party notices](THIRD-PARTY-NOTICES.md) for upstream source and package attribution.

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
- `Doroti.Host.WindowsAppSdk` + `Doroti.Host.WindowsAppSdk.Native`: default Windows App SDK 2.4 host; native C++ owns the top-level/child/task HWNDs and ingress, while managed code owns the Doroti framework and hardware-D3D11 ANGLE/EGL/Skia presentation
- `Doroti.Target.Windows.WindowsAppSdk.win-x64`: self-contained unpackaged Windows target with `HwndExactCpp`, native host/bootstrap, and app-directory ANGLE runtime
- `Doroti.Host.Maui`: MAUI lifecycle and SKGLView/AppKit-owned MTKView/Metal adapters for Android, iOS, Mac Catalyst, AppKit, and the explicit alternative Windows MAUI backend
- `Doroti.Host.Web`: host-owned Blazor composition, WebGL2 canvas, input, accessibility, and resource bridge
- `Doroti.Host.Qt`: managed-owned Linux process with a Qt 6 `QOpenGLWindow`, versioned C ABI v2, GPU surface, input, IME, desktop services, and an accessibility adapter

Web execution source is TypeScript-owned. Applications edit `web/src/**/*.ts`; Doroti owns `src/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` 7.0.0 compiles both into runner-local `obj` directories, and publish contains only the resulting JavaScript. Node, npm, Bun, and a bundler are not application requirements. The opt-in `worker-direct-webgl` qualification backend transfers the visible canvas once and keeps .NET, Skia, WebGL2, and Worker rAF in one persistent Worker; `auto` remains `document-webgl` pending physical acceptance. See [ADR-020](docs/adr/ADR-020-web-typescript-bootstrap.md).

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

The Windows default remains the opaque child-HWND ANGLE presenter. Windows 11 24H2 build 26100 or newer can explicitly request `WindowBackdropMode.experimentalAcrylic`, which uses the same hardware D3D11 device for ANGLE/Skia and a three-slot Composition Swapchain behind one ContentIsland. Capability, adapter, or initialization failure selects opaque before show and records the fallback reason; a visible window never changes topology. Interactive resize leaves shell `WM_SIZING` unmodified and uses actual child `WM_SIZE` physical pixels as the sole metrics authority. Interactive terminal waits are capped at 16 ms; the exit exact wait remains capped at 100 ms. Raster-thread `DwmFlush` is skipped per interactive frame and retained for exit/non-interactive exact presents. The already-physical surface uses a 1:1 ContentIsland scale, while a 256 px retained overscan and dark top-HWND erase fill prevent white exposure during fast growth. Three-second geometry retains the active-edge `min(12 physical px, ceil(6 logical px × scale))` and 1-px inactive-edge limits. The 600 ms responsiveness profile requires 40 fps with a 0.5-fps measurement tolerance, at most 26 px cursor lag, and final exact content within nine refresh intervals and 50 ms. The stable `acrylic` name is not mapped to this path.

`DOROTI_WINDOWS_PRESENTER=Vulkan` selects an experimental Vulkan/Skia presenter; ANGLE remains the default and there is no silent fallback. The product requires System32 Vulkan 1.1, a matching adapter LUID, and dedicated importable D3D11-texture external memory. It presents through Windows Presentation with zero Vulkan WSI swapchains. SkiaSharp packages are pinned to `4.152.0-rc.1.26426.14`, and the Windows host uses `SkiaSharp.Vulkan.Silk.NET` with `GRSilkNetBackendContext`. A native topmost DirectComposition target on one visible exact-size child HWND owns the full-capacity identity source. The parent `WM_SIZE` resizes that child and the nested child `WM_SIZE` waits through the matching Presentation submission for every edge, while CompositionFrame/DWM scan-out remains outside the resize-loop wait. On Windows 11 24H2+, Vulkan may be combined with `experimentalAcrylic`; a host-backdrop-enabled non-topmost `DesktopWindowTarget` owns an active `DesktopAcrylicController` on the top-level HWND, and the exact-child Vulkan overlay remains premultiplied. The controller accepts the shared kind/theme/tint/luminosity option channel. Automated WGC/current-monitor qualification is available through `validate-windows-vulkan-live-resize.ps1`, but physical resize/scan-out and Acrylic acceptance are still `notVerified`.

The Material demo opts in only when `DOROTI_DEMO_EXPERIMENTAL_ACRYLIC=1`. Runtime options use the `doroti/windows/experimental-acrylic` platform channel; ANGLE and Vulkan both apply kind/theme/tint/luminosity through their active `DesktopAcrylicController`. Query it with an empty payload to read effective mode, adapter LUID, transport, budgets, state, and option counters. The dedicated validators remain automated partial evidence; the full edge/DPI/refresh matrix and physical validation are incomplete, so this is not stable qualification.

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

# Doroti framework

**English** | [한국어](README.ko.md) · [Project overview](../README.md)

### Build cross-platform UIs in C#, without XAML

Doroti provides a shared widget, layout, painting, semantics, and rendering pipeline for desktop, mobile, and the web. The project began by translating Flutter framework source into C#; today, `Doroti.Framework.*` is developed and maintained directly in C#.

This guide covers the framework, development environment, application startup, and build tools. For runnable examples and platform launch commands, start with the [sample app guide](../DorotiTestbedApp/README.md).

> [!WARNING]
> Doroti is experimental. APIs and project structure may change, and platform implementations have different levels of validation.

[Requirements](#requirements) · [Build and run](#build-and-run) · [Application development](#application-development) · [Platform configuration](#platform-configuration) · [Framework structure](#framework-structure)

## What the framework provides

- Material and Cupertino widgets, layout, state, painting, and semantics in C#.
- A platform-neutral application library with separate native and Web runners.
- Skia Graphite rendering: Vulkan on Windows, Android, and Linux; Metal on Apple platforms; Dawn/WebGPU on Web.
- Native hosts for Windows App SDK, optional Windows MAUI, Android, UIKit, AppKit, Mac Catalyst, WebAssembly, and Qt.
- Shared startup, assets, native bindings, templates, and build tooling.

See the [platform implementation table](../README.md#platforms) for the host and renderer used by each target.

## Requirements

### Shared tools and SDK selection

- PowerShell 7 runs repository scripts such as `eng/doroti.ps1`. Start the commands below in PowerShell at the **repository root, `DorotiLab`**.
- **Platforms other than iOS: .NET SDK 10.0.400**, or a compatible patch in the same feature band, selected by the [root global.json](../global.json) and [Doroti/global.json](global.json).
- **iOS Testbed: .NET SDK 11.0.100-rc.1.26425.128**, or a compatible patch, selected by [iOS global.json](../DorotiTestbedApp/ios/global.json). Install it alongside .NET 10.
- Restore the platform workloads and NuGet packages for the selected SDK. The .NET 10 paths use the project-pinned 10.0.11 runtime packs, restored as needed for each target.

`dotnet` searches upward from the **current working directory** for `global.json`. Pointing `--project` at an iOS project does not change SDK selection. Run direct iOS commands from `DorotiTestbedApp/ios`, and other platform commands from the repository root. The workspace CLI's `build/run/publish -App ./DorotiTestbedApp -Platform ios` uses the iOS directory automatically.

SDK selection is separate from the target framework. iOS device Release defaults to NativeAOT; use `-CompilationMode Mono` for the explicit recovery profile. NativeAOT uses `net11.0-ios` and MAUI `11.0.0-rc.1.26451.6`. Debug, simulators, and the explicit Mono profile currently retain `net10.0-ios`. Use `publish` to produce the signed device app; see [iOS sample instructions](../DorotiTestbedApp/README.md#ios-sample).

### Platform prerequisites

Prepare the tools for the selected platform. Workload names identify .NET installation components; native SDKs and system libraries must also be installed.

| Platform / RID | Build host | .NET SDK / workload | Additional tools and runtime requirements |
| --- | --- | --- | --- |
| Windows App SDK (default) / `win-x64` | Windows x64 | 10 / no separate MAUI workload | Visual Studio MSBuild, MSVC **v145** C++ toolset, Windows SDK **10.0.26100.0**. The default Vulkan presenter requires a Vulkan 1.2 driver, D3D12 external-memory sharing, and DXGI/DirectComposition support. Acrylic requires Windows 11 24H2 or later. |
| Windows MAUI (optional) / `win-x64` | Windows x64 | 10 / `maui-windows` | Windows SDK and MAUI Windows build tools. Select with `-WindowsBackend Maui` in the workspace CLI. |
| macOS AppKit / `osx-arm64` | Apple Silicon Mac | 10 / `macos` | **macOS 14 or later**, a full Xcode installation compatible with the workload, and Metal support. Xcode also builds the app-owned Swift/Objective-C binding. |
| Mac Catalyst / `maccatalyst-arm64` | Apple Silicon Mac | 10 / `maui-maccatalyst` | A full Xcode installation compatible with the workload, the Mac Catalyst SDK, and Metal support. This is a separate runner from AppKit. |
| Android / `android-arm64`, `android-x64` | Windows or macOS | 10 / `maui-android` | Android SDK Platforms, Build Tools, Platform Tools (`adb`), and **OpenJDK 17–21**. Install the SDK required by the .NET workload plus **API 34** for the native bridge. Use an Android 7.0/API 24 or later device or an emulator with the matching ABI and Vulkan 1.2 support. |
| iOS / `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | macOS + Xcode | 11 / `maui-ios` | A full Xcode installation compatible with the workload and the iOS SDK. Simulators need the matching Simulator runtime; devices need **iOS 15 or later**, a signing certificate, and a provisioning profile. Release NativeAOT targets `ios-arm64`. |
| Linux Qt / `linux-x64` | Linux x64 | 10 / no separate MAUI workload | **Qt 6.5 or later** Core/Gui/Widgets/OpenGL/OpenGLWidgets development files, **CMake 3.24 or later**, a C/C++20 compiler, `pkg-config`, Wayland client development files, `wayland-scanner`, Vulkan development headers, and fontconfig. Runtime requires the `wayland` or `xcb` QPA plugin and a Vulkan 1.2 driver. |
| Web / `browser-wasm` | Windows, macOS, or Linux | 10 / `wasm-tools` | The default WebGPU path requires a browser with WebGPU and WASM threads, a hardware WebGPU adapter, and COOP/COEP isolation. Explicit `worker-direct-webgl` uses WebGL2. |

Windows App SDK 2.4 is restored through NuGet and deployed with the target; a separate machine-wide Windows App Runtime installation is not required. The Android native bridge uses the repository's Gradle 8.10.2 wrapper and AGP 8.6.1. Set `JAVA_HOME` to a supported JDK and add `adb` to `PATH`. On Apple hosts, check the selected Xcode with `xcode-select -p` and `xcodebuild -version`.

Linux also accepts software Vulkan devices such as llvmpipe when they satisfy the API requirements. Web runners restore `Microsoft.TypeScript.MSBuild` 7.0.0; application builds do not require Node, npm, or Bun.

### Check installations and restore workloads

```powershell
# Repository root: check SDK 10
dotnet --version
dotnet workload list
pwsh -File ./Doroti/eng/doroti.ps1 doctor

# macOS AppKit example: substitute the runner for the current host
dotnet workload restore ./DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj

# iOS: check and restore from the directory that selects SDK 11
Push-Location ./DorotiTestbedApp/ios
dotnet --version
dotnet workload list
dotnet workload restore ./DorotiTestbedApp.iOS.csproj
Pop-Location
```

`workload restore` prepares .NET workloads for the selected SDK. It does not install external tools such as Xcode, Android SDK/JDK, MSVC, or Qt. `doctor` checks shared tools; it does not replace a complete platform build or device launch check.

See [Testbed run instructions](../DorotiTestbedApp/README.md#material-sample-mode) for platform commands. The `reference/flutter-master` checkout is needed only for explicit Flutter comparisons; prepare it when needed with `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`.

## Build and run

Run the following from the **repository root, `DorotiLab`**, after preparing the tools for your platform:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor

$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows
```

`run` builds before launching. Windows uses Windows App SDK/`HwndExactCpp` by default; add `-WindowsBackend Maui` for the independent MAUI runner. Select `android`, `ios`, `macos`, `maccatalyst`, `linux`, or `web` with `-Platform` for other targets. Their prerequisites and device options are covered in the [sample app guide](../DorotiTestbedApp/README.md#run-by-platform).

### Build once and reuse

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform web -Configuration Release
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release -LastSuccessful
```

`-LastSuccessful` and `-NoBuild` reuse an existing successful build only when its inputs, dependencies, toolchain, and output hashes still match. A missing or stale record requires another run without these flags. `-NoRestore` skips restore while still building and checking dependencies. These flags apply to `run` only.

<details>
<summary>How build reuse is validated</summary>

The CLI records `doroti.launch-state/v4`, including the runner, configuration, RID, compilation mode, source/resource/native inputs, inherited settings, evaluated project items, restored dependency contents, SDK/workload/tool identities, and output hashes. Static Web assets are included. iOS compilation modes use separate output and cache locations.

Normal builds restore before collecting dependency identities. Changed or previously untracked dependencies and toolchains force a rebuild; older records are rejected for reuse. The compiler runs in a fresh process, and dependency changes during a build prevent recording a successful artifact. Custom native targets declare external dependencies through `DorotiLaunchDependency`.

Qt retains CMake dependency checks and copies the selected native library even if size and timestamps match. Windows C++/WinRT generation checks generator, SDK/package, and header identities. These checks establish build consistency; they do not establish device deployment or runtime acceptance.

</details>

### Command reference

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

`build` without an app targets the product solution, which includes projects requiring different host operating systems. Use `-App` and `-Platform` for a single target. The `release` command validates and packs local artifacts; it does not publish a GitHub Release.

## Application development

### Startup

Create the widget entrypoint with your root widget:

```csharp
new Doroti.Framework.DorotiWidgetEntrypoint(() => new MyApp())
```

`MyApp` is your application's root widget. The entrypoint attaches it and requests the first frame automatically; no manual `scheduleFrameCallback` or `scheduleForcedFrame` call is needed. An optional second `Func<Task>` argument prepares resources, such as fonts, before root creation. Completion returns to the owning view's event loop; detaching or shutting down prevents a pending initialization from attaching the root.

### Themes

Material uses Material 3. Omit the removed `useMaterial3` argument from `ThemeData` constructors, factories, and `copyWith`. Use `Typography.Create` or `CreateMaterial2021` for typography.

To follow system dark mode, provide `theme` and `darkTheme` to `MaterialApp` with `themeMode: ThemeMode.system`. Build palettes with `ColorScheme.CreateFromSeed` and `Brightness.light`/`Brightness.dark`; widgets read them through `Theme.of(context).colorScheme`. See the [complete theme example](../DorotiTestbedApp/README.md#system-dark-mode-and-color-palettes).

### Native bindings

Android, iOS, AppKit macOS, and Mac Catalyst runners each reference an app-owned native binding. Android uses `AndroidGradleProject`; Apple uses `XcodeProject`. The .NET runner owns the final application. The `native` commands inspect, build, locate, or extend these workspaces.

## Platform configuration

### Windows

| Setting | Behavior |
| --- | --- |
| `DOROTI_WINDOWS_PRESENTER` | `Vulkan` by default |
| `DOROTI_WINDOWS_GPU_PREFERENCE` | `NoPreference` by default; `LowPowerPreference` or `HighPerformancePreference` applies to Vulkan |
| `DOROTI_WINDOWS_VULKAN_DEVICE` | Select a Vulkan device by its exact name or a unique name fragment |

The default path renders through Graphite/Vulkan and presents through D3D12/DXGI DirectComposition on the same adapter. There is no automatic presenter fallback. Native PlatformView raster slices retain their D3D11 drawing API. Synchronization and resize details are documented in the [D3D12 output report](docs/validation/windows-d3d12-output-2026-09-14.md).

### Web

| Renderer | Requirements |
| --- | --- |
| `worker-direct-webgpu` (default) | Graphite/Dawn, `runtimeLocation: "main"`, WASM threads, COOP/COEP isolation, hardware WebGPU adapter |
| `worker-direct-webgl` (explicit) | Ganesh/WebGL2; also supports an independent Worker runtime |

`auto` and unknown renderer values select WebGPU. GPU initialization failures do not trigger an automatic fallback. Both paths transfer the visible canvas once. Loader `started` signals runtime/GPU readiness, not the first visible content.

Application bootstrap code lives in `web/src/**/*.ts`; framework Web code lives in `src/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` compiles both into runner-local `obj` directories, and publishing includes the resulting JavaScript. Node, npm, Bun, and a bundler are not required. Testbed and templates preload the same-origin fallback font and use the import-mapped `dotnet.js`. See [Web renderer options](../DorotiTestbedApp/README.md#web-renderers-and-measurement-options).

### Window appearance

`DorotiViewConfiguration.appearance` controls the backdrop and titlebar independently. `unified` is the default titlebar style; `solid` selects a separate titlebar. Use Acrylic on Windows App SDK and Qt, with an optional macOS override for Acrylic or Liquid Glass. See the [window appearance API](docs/window-appearance.md).

On Windows 11 24H2 or later, request `new WindowBackdropOptions(WindowBackdropMode.acrylic)` without an experimental flag. The Material sample already requests Acrylic. App content must be transparent or translucent for the effect to be visible; omitted backdrop options and `system` remain opaque.

<details>
<summary>Compatibility with earlier Acrylic settings</summary>

`experimentalAcrylic` uses the same Acrylic implementation. `DOROTI_DEMO_EXPERIMENTAL_ACRYLIC=1` reproduces the legacy sample mode. Runtime kind/theme/tint/luminosity updates and state queries retain the `doroti/windows/experimental-acrylic` platform channel for compatibility.

</details>

## Framework structure

| Module | Responsibility |
| --- | --- |
| `Doroti.Framework.*` | Foundation, scheduling, services, animation, gestures, layout, painting, semantics, widgets, Material, and Cupertino |
| `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting` | Runtime support and shared startup, builder, and view contracts |
| `Doroti.App.Sdk` | Platform-neutral `net10.0` application assembly and shared assets |
| `Doroti.Runner.Sdk` | Target runner validation, native/Web bootstrap, and plugin registration |
| `Doroti.Skia.Rendering`, `Doroti.Skia.RuntimeEffects` | Shared GPU rendering, scenes, text, images, effects, and SkSL compilation |
| `Doroti.Host.WindowsAppSdk` + `.Native` | Managed framework integration and native C++ window/input handling |
| `Doroti.Host.Maui` | Android, UIKit, AppKit, Mac Catalyst, and optional Windows MAUI adapters |
| `Doroti.Host.Web` | Worker/canvas startup, input, accessibility, and resources |
| `Doroti.Host.Qt` | Qt 6 `QWindow`, C ABI bridge, input, IME, desktop services, and accessibility |
| `Doroti.Target.*` | Platform-specific package composition and deployment |

Features and fixes belong directly in the owning framework, runtime, renderer, or host project. Update consumers when shared contracts change. The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-comparison tools; they do not overwrite maintained framework source and are not required for ordinary builds.

## Platform evidence boundaries

Build, native execution, browser execution, physical-device behavior, accessibility, and cross-platform parity are separate checks. A successful build does not establish signing, store readiness, device behavior, or performance. Unrun combinations remain `notVerified`.

| Area | Scope and further reading |
| --- | --- |
| Windows | Some physical resize and mixed-DPI scenarios received user acceptance; synthetic pixel/cadence failures remain separate. See the [D3D12 output report](docs/validation/windows-d3d12-output-2026-09-14.md) for scoped results. |
| Linux | Historical OpenGL runs under VMware Wayland/XWayland do not qualify the current Vulkan path or physical hardware. See the [Linux Qt record](../history/26-08-20/linux-qt-backend-summary.md). IME, Orca, context recovery, and long-running performance need separate coverage. |
| AppKit | Native execution and remaining conditions are recorded in the [AppKit summary](../history/26-08-20/macos-appkit-dual-backend-summary.md). |
| iOS | NativeAOT build, deployment, and device observations are recorded in the [NativeAOT summary](../history/26-09-10/nativeaot-work2-summary.md); they apply to the documented configurations. |
| Native controls and WebView | Consult the [PlatformView support matrix](docs/platform-views/support-matrix.md) for implementation and validation boundaries. |
| Texture | [Frame textures](docs/textures.md): CPU RGBA, Android Surface/AHB, and Windows D3D11 GPU input. Windows camera and AMD/NVIDIA composition tested; iOS/macOS Metal and Linux DMA-BUF camera adapters are source-only, not runtime-qualified. |

The [development history](../history/) contains results from specific runs, not guarantees for every current device or configuration.

## Development and repository layout

| Path | Contents |
| --- | --- |
| [`src/`](src/) | Framework, runtime, rendering, hosts, targets, and SDK |
| [`templates/`](templates/) | `doroti-app` application workspace template |
| [`eng/`](eng/) | Build, run, validation, packaging, and diagnostic scripts |
| [`tools/`](tools/) | Framework tooling |
| [`validation/`](validation/) | Validation source and fixtures |
| [`docs/`](docs/) | API documentation and scoped validation reports |
| [`../tools/Doroti.DartToCSharp/`](../tools/Doroti.DartToCSharp/) | Optional Dart/Flutter import compiler |

Keep generated tool and validation output under `.doroti/` or `artifacts/`, with validation output under `artifacts/validation/`. Retained milestone records belong in `../history/`. Compiler-owned `.g.cs` files are not compiled into `src/Doroti.Framework.*`. Repository JSON uses `System.Text.Json`.

## License

Doroti uses the [BSD 3-Clause License](../LICENSE). See [third-party notices](THIRD-PARTY-NOTICES.md) for source and package attribution.

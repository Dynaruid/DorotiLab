# Doroti framework

**English** | [한국어](README.ko.md) · [Project overview](../README.md)

### Build cross-platform UIs in C#, without XAML

Doroti provides a shared widget, layout, painting, semantics, and rendering pipeline for desktop, mobile, and the web. Its Flutter-inspired APIs are developed and maintained directly in C# in `Doroti.Framework.*`.

This guide covers the framework, development environment, application startup, and build tools. For runnable examples and platform launch commands, start with the [sample app guide](../samples/DorotiTestbedApp/README.md).

> [!WARNING]
> Doroti is experimental. APIs and project structure may change, and platform implementations have different levels of validation.

[Requirements](#requirements) · [Build and run](#build-and-run) · [Application development](#application-development) · [Platform configuration](#platform-configuration) · [Framework structure](#framework-structure)

## What the framework provides

- Material and Cupertino widgets, layout, state, painting, and semantics in C#.
- A platform-neutral application library with separate native and Web runners.
- Skia Graphite rendering: Vulkan on Windows, Android, and Linux; Metal on Apple platforms. Web defaults to Ganesh/WebGL2 outside Android and prefers Graphite/Dawn/WebGPU on Android.
- Native hosts for Windows App SDK, optional Windows MAUI, Android, UIKit, AppKit, Mac Catalyst, WebAssembly, and Qt.
- Shared startup, assets, native bindings, templates, and build tooling.

See the [platform implementation table](../README.md#platforms) for the host and renderer used by each target.

## Requirements

Release app DLL obfuscation with Obfuscar is optional and supports ReadyToRun publishing. NativeAOT and Mono/WebAssembly AOT paths are excluded.
Windows and Linux Qt runners support `PublishSingleFile=true` with untrimmed JIT or individual ReadyToRun output. See the [carousel publish examples](../samples/DorotiCarouselApp/README.md#데스크톱-단일-실행-파일-게시).

### Shared tools and SDK selection

- Install [PowerShell 7](https://learn.microsoft.com/ko-kr/powershell/scripting/install/install-powershell?view=powershell-7.6) on Windows, macOS, or Linux to run repository scripts such as `eng/doroti.ps1`. Start the commands below in PowerShell at the **repository root, `DorotiLab`**.
- **Platforms other than iOS: .NET SDK 10.0.400**, or a compatible patch in the same feature band, selected by the [root global.json](../global.json) and [Doroti/global.json](global.json).
- **iOS Testbed: .NET SDK 11.0.100-rc.2.26504.105**, or a compatible patch, selected by [iOS global.json](../samples/DorotiTestbedApp/ios/global.json). Install it alongside .NET 10.
- Restore the platform workloads and NuGet packages for the selected SDK. The .NET 10 paths use the project-pinned 10.0.11 runtime packs, restored as needed for each target.

`dotnet` searches upward from the **current working directory** for `global.json`. Pointing `--project` at an iOS project does not change SDK selection. Run direct iOS commands from `samples/DorotiTestbedApp/ios`, and other platform commands from the repository root. The workspace CLI's `build/run/publish -App ./samples/DorotiTestbedApp -Platform ios` uses the iOS directory automatically.

SDK selection is separate from the target framework. Direct Testbed iOS device Release defaults to NativeAOT; pass `-p:DorotiCompilationMode=Mono -p:PublishAot=false` to `dotnet build/publish` for the explicit Mono profile. NativeAOT defaults to `net11.0-ios` and MAUI `11.0.0-rc.2.26505.5`; Debug and simulators retain `net10.0-ios`. The workspace CLI uses the TFM/RID declared in its manifest; the current Testbed `ios` alias selects `net10.0-ios` / `iossimulator-x64`. Use the [iOS sample instructions](../samples/DorotiTestbedApp/README.md#ios-sample) for other simulators, signed devices, and SDK selection.

**.NET 11 RC2 + Xcode 27:** RC2 requires Xcode 27.0 and workload set `11.0.100-rc.2.26509.2` (iOS `27.0.12212-net11-rc.2`). Xcode version validation stays enabled. See the [SampleApp2 build example](../samples/DorotiSampleApp2/README.md#net-11-rc2--xcode-27-native-aot) and [official RC2 requirements](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-rc2-12212).

### Platform prerequisites

Prepare the tools for the selected platform. Workload names identify .NET installation components; native SDKs and system libraries must also be installed.

| Platform / RID | Build host | .NET SDK / workload | Additional tools and runtime requirements |
| --- | --- | --- | --- |
| Windows App SDK (default) / `win-x64` | Windows x64 | 10 / no separate MAUI workload | Visual Studio MSBuild, MSVC **v145** C++ toolset, Windows SDK **10.0.26100.0**. The default Vulkan presenter requires a Vulkan 1.2 driver, D3D12 external-memory sharing, and DXGI/DirectComposition support. Acrylic requires Windows 11 24H2 or later. |
| Windows MAUI (optional) / `win-x64` | Windows x64 | 10 / `maui-windows` | Windows SDK and MAUI Windows build tools. Select the declared `-Platform windows-maui` sample alias. |
| macOS AppKit / `osx-arm64` | Apple Silicon Mac | 10 / `macos` | **macOS 14 or later**, a full Xcode installation compatible with the workload, and Metal support. Xcode also builds the app-owned Swift/Objective-C binding. |
| Mac Catalyst / `maccatalyst-arm64` | Apple Silicon Mac | 10 / `maui-maccatalyst` | A full Xcode installation compatible with the workload, the Mac Catalyst SDK, and Metal support. This is a separate runner from AppKit. |
| Android / `android-arm64`, `android-x64` | Windows or macOS | 10 / `maui-android` | Android SDK Platforms, Build Tools, Platform Tools (`adb`), and **OpenJDK 17–21**. Install the SDK required by the .NET workload plus **API 34** for the native bridge. Use an Android 7.0/API 24 or later device or an emulator with the matching ABI and Vulkan 1.2 support. |
| iOS / `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | macOS + Xcode | 11 / `maui-ios` | A full Xcode installation compatible with the workload and the iOS SDK. Simulators need the matching Simulator runtime; devices need **iOS 15 or later**, a signing certificate, and a provisioning profile. Release NativeAOT targets `ios-arm64`. |
| Linux Qt / `linux-x64` | Linux x64 | 10 / no separate MAUI workload | Default Quick: **Qt 6.6 or later** Core/Gui/Widgets/Quick/Qml/QuickControls2 and runtime QML modules. Testbed WebView additionally requires **Qt 6.8 or later** WebEngineQuick/WebChannel. Also **CMake 3.24 or later**, a C/C++20 compiler, `pkg-config`, Wayland client development files, `wayland-scanner`, Vulkan headers, and fontconfig. Runtime requires `wayland` or `xcb` QPA and Vulkan 1.2. See the [native host guide](../packages/platforms/qt/native/README.md) for comparison backends. |
| Web / `browser-wasm` | Windows, macOS, or Linux | 10 / `wasm-tools` | Android prefers WebGPU when available; other platforms use WebGL2. The Testbed's main-owned runtime also requires WASM threads and COOP/COEP isolation. Explicit `worker-direct-webgpu` requires a hardware WebGPU adapter. |

Windows App SDK 2.4 is restored through NuGet and deployed with the target; a separate machine-wide Windows App Runtime installation is not required. The Android native bridge uses the repository's Gradle 8.10.2 wrapper and AGP 8.6.1. Set `JAVA_HOME` to a supported JDK and add `adb` to `PATH`. On Apple hosts, check the selected Xcode with `xcode-select -p` and `xcodebuild -version`.

Linux also accepts software Vulkan devices such as llvmpipe when they satisfy the API requirements. Web runners restore `Microsoft.TypeScript.MSBuild` 7.0.0; application builds do not require Node, npm, or Bun.

### Check installations and restore workloads

```powershell
# Repository root: check SDK 10
dotnet --version
dotnet workload list
pwsh -File ./Doroti/eng/doroti.ps1 doctor -App ./samples/DorotiTestbedApp -Platform windows -Scope target

# macOS AppKit example: substitute the runner for the current host
dotnet workload restore ./samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj

# iOS: check and restore from the directory that selects SDK 11
Push-Location ./samples/DorotiTestbedApp/ios
dotnet --version
dotnet workload list
dotnet workload restore ./DorotiTestbedApp.iOS.csproj
Pop-Location
```

`workload restore` prepares .NET workloads for the selected SDK. External tools are installed separately. Provider doctor requires explicit `-App` and `-Platform`; `-Scope managed|target|native|tools|full` selects its prerequisite checks. FAIL/PARTIAL exits nonzero. A prerequisite PASS does not replace build/device acceptance.

See [Testbed run instructions](../samples/DorotiTestbedApp/README.md#material-sample-mode) for platform commands. The `reference/flutter-master` checkout is needed only for explicit Flutter comparisons; prepare it when needed with `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`.

## Build and run

Run the following from the **repository root, `DorotiLab`**, after preparing the tools for your platform:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor -App ./samples/DorotiTestbedApp -Platform windows -Scope target

$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiTestbedApp -Platform windows
```

`run` builds before launching. Windows uses Windows App SDK/`HwndExactCpp` by default; select `-Platform windows-maui` for the independent MAUI runner. Select `android`, `ios`, `macos`, `maccatalyst`, `linux`, or `web` with `-Platform` for other targets. Their prerequisites and device options are covered in the [sample app guide](../samples/DorotiTestbedApp/README.md#run-by-platform).

### CLI coverage

`describe -App <workspace> -Platform all` reports declared provider aliases, host restrictions, supported operations and development transport. `build`, `run`, `publish` and `dev` require a declared alias and provider support; `dev` requires Debug. Configuration/device/template services are provided by extensions, and unsupported services fail explicitly. The built-in template is installed with `dotnet new` and supports `--design widgets|material|cupertino`.

The former `-WindowsBackend`, `-Rid`, `-LastSuccessful`, `-NoBuild`, `-NoRestore`, `native` and fixed template command switches are not part of the managed provider CLI. TFM/RID/backend/profile selection belongs to `doroti-workspace.json`; use direct runner/MSBuild commands for other supported build profiles. Android Debug Mono development uses the provider's Python/ADB adapter. Apple Debug development uses packaged Python adapters for AppKit, Catalyst, iOS simulators and the declared iPhone Mono LAN/CoreCLR USB profiles; current Apple runtime acceptance remains unverified. See the [VS Code guide](../tools/vscode-doroti/README.md) for available development targets.

Repository maintenance commands are `validate -ValidationSuite <suite>`, `audit`, and `release` (the Release verification suite). Local package candidates use [release-candidate.py](eng/release-candidate.py) with independent version arguments.

### Local release candidates

From a Windows build host, create independent core/provider packages and an isolated NuGet-only template consumer:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/release-candidate.py --targets windows web --design widgets
```

The defaults are core `0.4.1-beta`, Material/Cupertino `1.0.0-alpha.1`, and each selected provider's manifest version. Override them with `--core-version`, `--material-version`, `--cupertino-version`, and repeated `--provider-version PROVIDER=VERSION`. `--workspace` selects a workspace v2 manifest; targets must be declared aliases available on the build host, with one profile per provider per run.

Candidates go to a fresh `Doroti/artifacts/release/<version>-<id>/` directory unless `--output` is supplied. `candidate.json` records evaluated versions, package/payload hashes, publish profiles and runtime results. `--build-root` must stay under `Doroti/artifacts`; investigation logs/consumers live under `temp/testing/release-candidate/`. Existing nonempty candidate directories are rejected. These local outputs are disposable under the [storage policy](eng/LOCAL_STORAGE.md).

Runtime checks are opt-in with `--runtime-targets`; Android additionally requires `--device <serial>`. Without them, runtime remains `notVerified`. Signing, clean-machine installation and physical input remain separate gates. The script does not publish to NuGet or GitHub, and `doroti release` runs verification without creating these candidates.

## Application development

### Startup

Create the widget entrypoint with your root widget:

```csharp
new Doroti.Framework.DorotiWidgetEntrypoint(() => new MyApp())
```

`MyApp` is your application's root widget. The entrypoint attaches it and requests the first frame automatically; no manual `scheduleFrameCallback` or `scheduleForcedFrame` call is needed. An optional second `Func<Task>` argument prepares resources, such as fonts, before root creation. Completion returns to the owning view's event loop; detaching or shutting down prevents a pending initialization from attaching the root.

### Themes

Material uses Material 3. Omit the removed `useMaterial3` argument from `ThemeData` constructors, factories, and `copyWith`. Use `Typography.Create` or `CreateMaterial2021` for typography.

To follow system dark mode, provide `theme` and `darkTheme` to `MaterialApp` with `themeMode: ThemeMode.system`. Build palettes with `ColorScheme.CreateFromSeed` and `Brightness.light`/`Brightness.dark`; widgets read them through `Theme.of(context).colorScheme`. See the [complete theme example](../samples/DorotiTestbedApp/README.md#system-dark-mode-and-color-palettes).

### App icons

Samples and new templates use the Doroti app icon by default. See [app icon defaults and overrides](branding/README.md#앱의-기본-아이콘) to replace or disable it for an application.

### Native bindings

Android, iOS, AppKit macOS, and Mac Catalyst runners each reference an app-owned native binding. Android uses `AndroidGradleProject`; Apple uses `XcodeProject`. The .NET runner owns the final application and builds the binding with its selected profile. Open Android native workspaces in Android Studio and Apple workspaces in Xcode; the provider CLI does not expose `native doctor/build/open/add` commands.

## Platform configuration

### Windows

| Setting | Behavior |
| --- | --- |
| `DOROTI_WINDOWS_PRESENTER` | `Vulkan` by default |
| `DOROTI_WINDOWS_GPU_PREFERENCE` | `NoPreference` by default; `LowPowerPreference` or `HighPerformancePreference` applies to Vulkan |
| `DOROTI_WINDOWS_VULKAN_DEVICE` | Select a Vulkan device by its exact name or a unique name fragment |

The default path renders through Graphite/Vulkan and presents through D3D12/DXGI DirectComposition on the same adapter. There is no automatic presenter fallback. Native PlatformView raster slices retain their D3D11 drawing API.

### Web

| Renderer | Requirements |
| --- | --- |
| `worker-direct-webgl` (default outside Android) | Ganesh/WebGL2; also supports an independent Worker runtime |
| `worker-direct-webgpu` (preferred on Android) | Graphite/Dawn, `runtimeLocation: "main"`, WASM threads, COOP/COEP isolation, hardware WebGPU adapter |

Omitted, `auto`, and unknown renderer values prefer WebGPU on Android and WebGL2 on Windows, iOS/iPadOS, macOS, and other platforms. Before startup, Android auto selects WebGL2 if the runtime/origin is unsuitable or the WebGPU API/adapter is unavailable. Explicit renderer overrides are preserved. Device/context initialization failures after this preflight do not trigger a backend retry. Both paths transfer the visible canvas once. Loader `started` signals runtime/GPU readiness, not the first visible content.

Application bootstrap code lives in `web/src/**/*.ts`; framework Web code lives in `../packages/platforms/web/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` compiles both into runner-local `obj` directories, and publishing includes the resulting JavaScript. Node, npm, Bun, and a bundler are not required. Testbed and templates preload the same-origin fallback font and use the import-mapped `dotnet.js`. See [Web renderer options](../samples/DorotiTestbedApp/README.md#web-renderers-and-measurement-options).

### Window appearance

`DorotiViewConfiguration.appearance` controls the backdrop and titlebar independently. `unified` is the default titlebar style; `solid` selects a separate titlebar. Use Acrylic on Windows App SDK and Qt, with an optional macOS override for Acrylic or Liquid Glass.

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
| `Doroti.Host.Qt` | Qt 6 `QQuickWindow` (default), C ABI bridge, input, IME, desktop services, and accessibility |
| `Doroti.Target.*` | Platform-specific package composition and deployment |

Features and fixes belong directly in the owning framework, runtime, renderer, or host project. Update consumers when shared contracts change. The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-comparison tools; they do not overwrite maintained framework source and are not required for ordinary builds.

## Platform support

Host, renderer, and build-mode support varies by target. Signing, store distribution, input methods, accessibility, and performance require checks on the intended device and configuration.

Native controls and WebView have host-specific composition constraints. Frame textures support CPU RGBA, Android Surface/AHB, and Windows D3D11 inputs. Web textures accept Canvas/video/frame GPU input on WebGPU or explicit WebGL. iOS/macOS Metal and Linux DMA-BUF camera adapters require runtime verification before use.

## Development and repository layout

| Path | Contents |
| --- | --- |
| [`src/`](src/) | Shared framework, runtime, rendering, tooling, and SDK |
| [`../packages/`](../packages/README.md) | Material/Cupertino implementations and platform host/target providers |
| [`templates/`](templates/) | `doroti-app` application workspace template |
| [`eng/`](eng/) | Build, run, validation, packaging, and diagnostic scripts |
| [`../tools/`](../tools/) | Repository developer tools |
| [`tests/`](tests/) | Selected permanent regression sources |
| [`../tools/Doroti.DartToCSharp/`](../tools/Doroti.DartToCSharp/) | Optional Dart/Flutter import compiler |

Keep generated tool output under `.doroti/` or `artifacts/`. New test sources, consumers and raw evidence belong under repository-root `temp/testing/<task>/<run>/`; summarize results and delete the owned run directory. See the [testing guide](tests/README.md) for retained-failure and direct-script policies. Maintained suites use `eng/validate.ps1` with a 20-minute timeout. Retained milestone records belong in `../history/`. Compiler-owned `.g.cs` files are not compiled into `src/Doroti.Framework.*`. Repository JSON uses `System.Text.Json`.

## License

Doroti uses the [BSD 3-Clause License](../LICENSE). See [third-party notices](THIRD-PARTY-NOTICES.md) for source and package attribution.

### Linux Debian packages

On Debian 13 amd64, Linux Qt publish supports `-PackageFormat deb -Output <directory>`.
The profile publishes a self-contained, untrimmed Release JIT application and packages
an application-private Qt 6.12 runtime, launcher, desktop entry, icon and license notices.
WebEngine 6.140 is bundled by default; a separate local engine `.deb` can be installed
alongside the app without operating an APT repository. Explicit SDK prefixes and
reviewed source/notice inventories are required; see the [runtime guide](../packages/platforms/qt/qt-runtime.md). Configure
an explicit `DorotiDebPackageId`, maintainer, description and copyright file in the Linux
runner; see the [Qt packaging guide](../packages/platforms/qt/README.md#debian-packages)
for metadata, version rules and requirements. Omit `-PackageFormat` for existing directory
publish. Install/update with `sudo apt install ./<app>.deb`; remove with `sudo apt remove
<package-id>` or `sudo apt purge <package-id>`. User XDG data is retained. Local `.deb`
distribution does not enable automatic updates. Installation and GUI acceptance are
tracked separately in the [maintained tests](tests/README.md).
The launcher chooses native Wayland or xcb from the session and preserves explicit
settings. Real Qt 6.12 Debian 13 Wayland/xcb acceptance remains an open release gate.

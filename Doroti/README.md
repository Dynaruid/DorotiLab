# Doroti runtime and framework

**English** | [한국어](README.ko.md)

Doroti is a C#/.NET UI framework with a shared widget, layout, painting, semantics, and rendering pipeline for Windows App SDK, optional Windows MAUI, native AppKit macOS, Mac Catalyst, Android, iOS, WebAssembly, and Linux/Qt.

iOS device (`ios-arm64`) Release builds default to NativeAOT. Debug, simulators and other platforms keep their existing defaults; `-CompilationMode Mono` explicitly selects the recovery profile. See [iOS build instructions](validation/native-aot/README.md).

Official Graphite is the default. Windows uses the standard app-directory DLL from pinned SkiaSharp NativeAssets; no manifest override is required. Android verifies the official APK asset, and Qt negotiates Vulkan 1.2 with the official desktop asset. Custom Skia builds and the private ABI binding have been retired. macOS/Mac Catalyst checks were resumed; see the [Apple review](../history/2026-09-13/apple-work0/README.md) for scoped results and remaining gates. iOS execution remains unverified. Linux build/package/Qt ABI checks were resumed; llvmpipe Graphite runs by default; renderer admission uses API capabilities rather than device classification. Headless checks pass; Material product depth synchronization and hardware qualification remain unresolved. See the [Linux follow-up](docs/validation/linux-official-graphite-2026-09-12.md). Performance acceptance remains incomplete. See [cutover and support matrix](docs/validation/official-graphite-cutover-2026-09-12.md) and [work0 status](../work0.md).

## Development model

`src/Doroti.Framework.*` is maintained product source. Its public namespaces are `Doroti.Framework.*`, matching the project, assembly, and package names. Add features and fix correctness directly in the owning framework/runtime/host project, then update every consumer of the shared contract.

The Dart-to-C# compiler and pinned Flutter checkout remain optional import and behavior-reference tools. They do not overwrite product source and are not required for ordinary builds. Compiler output stays in isolated workspaces until explicitly reviewed and adopted.

See [ADR-019](docs/adr/ADR-019-product-framework-source-ownership.md) for source ownership, [ADR-022](docs/adr/ADR-022-default-native-platform-bridge.md) for the default native bridge graph, and [ADR-025](docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md) for the current Windows host.

## Current product boundary

Widget applications use `new Doroti.Framework.DorotiWidgetEntrypoint(() => new MyApp())`.
The entrypoint attaches the root and requests the initial frame automatically; application code does not need `scheduleFrameCallback` or `scheduleForcedFrame` to start.
An optional second `Func<Task>` argument prepares resources (for example, fonts) before root creation. Completion returns to the owning view's event loop, and detaching or shutting down prevents a pending initialization from attaching a root.

Material themes use Material 3 exclusively. `ThemeData` factories, its constructor, and `copyWith` no longer accept `useMaterial3`, and the version property is removed; omit this argument in application code. Use `Typography.Create` or `CreateMaterial2021` for typography; the 2014/2018 presets are removed.

- `Doroti.Framework.*`: product-owned Foundation, Scheduler, Services, Physics, Animation, Gestures, Painting, Semantics, Rendering, Widgets, Cupertino, and Material libraries
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`: runtime semantics plus the target-neutral startup/builder/descriptor contract
- `Doroti.App.Sdk`: platform-neutral `net10.0` application assembly and shared asset contract
- `Doroti.Runner.Sdk`: fixed-target runner validation plus runner-local native/Web bootstrap and plugin registration
- `Doroti.Skia.RuntimeEffects`: shared fail-closed SkSL compiler and uniform/image-sampler binder used by native and Web hosts
- `Doroti.Skia.Rendering`: host-neutral scene, paragraph, image, runtime-effect, semantics, cache, and terminal-ACK renderer shared by native GPU hosts
- `Doroti.Host.WindowsAppSdk` + `Doroti.Host.WindowsAppSdk.Native`: default Windows App SDK 2.4 host; native C++ owns the top-level/child/task HWNDs and ingress, while managed code owns the Doroti framework and Vulkan/Skia presentation (ANGLE remains selectable)
- `Doroti.Target.Windows.WindowsAppSdk.win-x64`: self-contained unpackaged Windows target with `HwndExactCpp`, native host/bootstrap, and app-directory ANGLE runtime
- `Doroti.Host.Maui`: MAUI lifecycle and SKGLView/AppKit-owned MTKView/Metal adapters for Android, iOS, Mac Catalyst, AppKit, and the explicit alternative Windows MAUI backend
- `Doroti.Host.Web`: Worker bootstrap, WebGL2 canvas, input, accessibility, and resource bridge
- `Doroti.Host.Qt`: managed-owned Linux process with a Qt 6 `QWindow`, versioned C ABI v2, GPU surface, input, IME, desktop services, and an accessibility adapter

Web execution source is TypeScript-owned. Applications edit `web/src/**/*.ts`; Doroti owns `src/Doroti.Host.Web/Web/*.ts`. `Microsoft.TypeScript.MSBuild` 7.0.0 compiles both into runner-local `obj` directories, and publish contains only the resulting JavaScript. Node, npm, Bun, and a bundler are not application requirements. The default `worker-direct-webgpu` backend uses SkiaSharp Graphite/Dawn on a render Worker sharing the main-owned .NET runtime. `worker-direct-webgl` explicitly selects Ganesh/WebGL2. Both transfer the visible canvas once; `auto` and unknown values select WebGPU. No automatic backend fallback is performed. CanvasKit and bitmap presentation paths have been removed. See [ADR-020](docs/adr/ADR-020-web-typescript-bootstrap.md).

Material applications follow system dark mode with `MaterialApp(theme:, darkTheme:, themeMode: ThemeMode.system)`. Build both palettes with `ColorScheme.CreateFromSeed`, `Brightness.light`/`Brightness.dark`, and optional role overrides such as `surface`, `primary`, or `outline`; widgets read the active roles from `Theme.of(context).colorScheme`. See the [DorotiTestbedApp dark-mode guide](../DorotiTestbedApp/README.md#system-dark-mode-and-color-palettes) for the MAUI/Web change flow and a complete example.

Android, iOS, native AppKit macOS, and Mac Catalyst runners each reference a default app-owned native binding. Android uses `AndroidGradleProject`; each Apple product has an explicit `XcodeProject` binding contract. The .NET runner still owns the final app. Build results do not prove native launch, device behavior, accessibility, signing, or archive; those gates remain `notVerified` until run.

## Requirements

### Shared tools and SDK selection

- PowerShell 7 runs repository scripts such as `eng/doroti.ps1`. Start the commands below in PowerShell at the **repository root, `DorotiLab`**.
- **Platforms other than iOS: .NET SDK 10.0.400**, or a compatible patch in the same feature band, selected by the [root global.json](../global.json) and [Doroti/global.json](global.json).
- **iOS Testbed: .NET SDK 11.0.100-rc.1.26425.128**, or a compatible patch, selected by [iOS global.json](../DorotiTestbedApp/ios/global.json). Install it alongside .NET 10.
- Restore the platform workloads and NuGet packages for the selected SDK. The .NET 10 paths use the project-pinned 10.0.11 runtime packs, restored as needed for each target.

`dotnet` searches upward from the **current working directory** for `global.json`. Pointing `--project` at an iOS project does not change SDK selection. Run direct iOS commands from `DorotiTestbedApp/ios`, and other platform commands from the repository root. The workspace CLI's `build/run/publish -App ./DorotiTestbedApp -Platform ios` uses the iOS directory automatically.

SDK selection is separate from the target framework. iOS device Release uses `net11.0-ios`, MAUI `11.0.0-rc.1.26451.6`, and NativeAOT. Debug, simulators, and the explicit Mono profile currently retain `net10.0-ios`. Use `publish` to produce the signed device app; see [iOS build instructions](validation/native-aot/README.md).

### Platform prerequisites

Prepare the tools for the selected platform. Workload names identify .NET installation components; native SDKs and system libraries must also be installed.

| Platform / RID | Build host | .NET SDK / workload | Additional tools and runtime requirements |
| --- | --- | --- | --- |
| Windows App SDK (default) / `win-x64` | Windows x64 | 10 / no separate MAUI workload | Visual Studio MSBuild, MSVC **v145** C++ toolset, Windows SDK **10.0.26100.0**. The default Vulkan presenter requires a Vulkan 1.2 driver, D3D11 external-memory sharing, and Windows Presentation support. Acrylic requires Windows 11 24H2 or later. |
| Windows MAUI (optional) / `win-x64` | Windows x64 | 10 / `maui-windows` | Windows SDK and MAUI Windows build tools. Select with `-WindowsBackend Maui` in the workspace CLI. |
| macOS AppKit / `osx-arm64` | Apple Silicon Mac | 10 / `macos` | **macOS 14 or later**, a full Xcode installation compatible with the workload, and Metal support. Xcode also builds the app-owned Swift/Objective-C binding. |
| Mac Catalyst / `maccatalyst-arm64` | Apple Silicon Mac | 10 / `maui-maccatalyst` | A full Xcode installation compatible with the workload, the Mac Catalyst SDK, and Metal support. This is a separate runner from AppKit. |
| Android / `android-arm64`, `android-x64` | Windows or macOS | 10 / `maui-android` | Android SDK Platforms, Build Tools, Platform Tools (`adb`), and **OpenJDK 17–21**. Install the SDK required by the .NET workload plus **API 34** for the native bridge. Use an Android 7.0/API 24 or later device or an emulator with the matching ABI. |
| iOS / `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | macOS + Xcode | 11 / `maui-ios` | A full Xcode installation compatible with the workload and the iOS SDK. Simulators need the matching Simulator runtime; devices need **iOS 15 or later**, a signing certificate, and a provisioning profile. Release NativeAOT targets `ios-arm64`. |
| Linux Qt / `linux-x64` | Linux x64 | 10 / no separate MAUI workload | **Qt 6.5 or later** Core/Gui/Widgets/OpenGL/OpenGLWidgets development files, **CMake 3.24 or later**, a C/C++20 compiler, `pkg-config`, Wayland client development files, `wayland-scanner`, Vulkan development headers, and fontconfig. Runtime requires the `wayland` or `xcb` QPA plugin and a Vulkan 1.2 driver. |
| Web / `browser-wasm` | Windows, macOS, or Linux | 10 / `wasm-tools` | The default WebGPU path requires a browser with WebGPU and WASM threads, a hardware WebGPU adapter, and COOP/COEP isolation. Explicit `worker-direct-webgl` uses WebGL2. |

Windows App SDK 2.4 and the ANGLE runtime are restored through NuGet and deployed with the target; a separate machine-wide Windows App Runtime installation is not required. The Android native bridge uses the repository's Gradle 8.10.2 wrapper and AGP 8.6.1. Set `JAVA_HOME` to a supported JDK and add `adb` to `PATH`. On Apple hosts, check the selected Xcode with `xcode-select -p` and `xcodebuild -version`.

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

## Commands

Web uses SkiaSharp WASM with a single runtime initialized on main or in the render Worker. The supported renderers are `worker-direct-webgpu` (default) and `worker-direct-webgl`; WebGPU requires `runtimeLocation: "main"`, WASM threads, COOP/COEP isolation and a hardware WebGPU adapter. Explicit WebGL also supports an independent Worker runtime. Testbed and template preload the same-origin fallback font and initialize the runtime through the import-mapped `dotnet.js`. Loader `started` is runtime/GPU readiness, not first visible content.

The Runner SDK owns the Qt CMake target with configuration-specific output. CMake retains native dependency checking. TypeScript and platform binding builds retain their existing correctness checks.

For validated build reuse, run from the repository root:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform web -Configuration Release
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release -LastSuccessful
```

`build` records a successful artifact; ordinary `run` records its build before launching. `-LastSuccessful`/`-NoBuild` requires launch-state v3 and matching source/resource/native inputs, inherited settings, evaluated project items, SDK/workload and tool identities, restored dependency contents, and hashed outputs (including evaluated static Web assets). The dependency identity includes transitive managed/native package assets, runtime packs, native file references, and `project.assets.json`. Restore runs before identity collection; the subsequent build uses that restored graph. Changed or previously untracked dependencies/toolchains force a rebuild; unchanged dependencies retain incremental builds. CLI compilations use fresh compiler processes so a previously warmed compiler server cannot reintroduce old package metadata on later source edits. Dependency changes during a build prevent recording or launching its output. Missing/changed artifacts and old v1/v2 records are rejected for reuse; run without reuse flags to create a v3 record. Generated/dependency directories are pruned from source traversal, but explicitly resolved dependency files are hashed separately. Printed fingerprint/toolchain time is separate from runtime TTID. Custom native build targets declare additional external files through `DorotiLaunchDependency`.

Qt copies the selected native library even when source and destination sizes/timestamps match. Windows C++/WinRT generation hashes the generator, pinned SDK/package metadata, and the complete generated header inventory; changed inputs or missing/modified headers regenerate the projection. These checks do not establish Android/Apple device deployment or physical runtime acceptance.

Run from the repository root:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform windows
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

Windows App SDK now defaults to `Vulkan`; select `DOROTI_WINDOWS_PRESENTER=AngleD3D11` to use ANGLE explicitly. GPU selection defaults to `NoPreference` (system default). Set `DOROTI_WINDOWS_GPU_PREFERENCE` to `LowPowerPreference` or `HighPerformancePreference` for a Windows/DXGI preference; this applies to Vulkan and ANGLE. `DOROTI_WINDOWS_VULKAN_DEVICE` optionally overrides the Vulkan choice with an exact or unique device-name fragment. On Windows 11 24H2+, an app can request `new WindowBackdropOptions(WindowBackdropMode.acrylic)` without an experimental flag; omitted backdrop options and `system` remain opaque. The demo already requests Acrylic. Vulkan uses System32 Vulkan 1.2, dedicated D3D11-texture external memory, and Windows Presentation, with no automatic presenter fallback. Web defaults to `worker-direct-webgpu`, including `auto`; other renderers remain explicitly selectable. These defaults do not change the recorded validation results or complete the remaining GPU/DPI/refresh/IME/accessibility qualification.

Vulkan moving-origin resize submits a prepared frame immediately after the HWND geometry change and waits for its CompositionFrame receipt. Implementation details, earlier failures, observed resize improvement, and validation limits are preserved in the [September 5 history](../history/26-09-05/windows-vulkan-acrylic-resize-summary.md). `experimentalAcrylic` remains a compatibility mode using the same Acrylic implementation.

The Material demo requests ordinary `acrylic` on Windows and draws its translucent Material surface once over a transparent renderer background. `DOROTI_DEMO_EXPERIMENTAL_ACRYLIC=1` is needed only to reproduce the legacy mode. Runtime kind/theme/tint/luminosity updates and state queries retain the existing `doroti/windows/experimental-acrylic` platform channel for compatibility. App content must be transparent or translucent for Acrylic to be visible.

Select the material and titlebar independently with `DorotiViewConfiguration.appearance`. The default is `unified`; `solid` selects a separate titlebar. Use Acrylic on Windows App SDK and Qt, with an optional macOS override for Acrylic or Liquid Glass. See the [window appearance API and platform behavior](docs/window-appearance.md).

## Platform evidence boundaries

The Windows App SDK target, package, default CLI route, hardware-D3D11 ANGLE runtime, first-frame ordering, and tested physical resize/mixed-DPI boundary behavior have current evidence. C10 is a user-acceptance PASS for the observed opaque conditions; strict synthetic resize qualification and pixel/cadence failures remain failures. Experimental Acrylic automation and its physical acceptance are separate evidence classes: any unexecuted DPI, refresh, edge/speed, monitor, scan-out, IME, accessibility, window-management, or device-loss combination remains `notVerified`. The last full `Doroti.Product.slnx` Release run on Windows failed only after the Windows target passed, when a macOS project invoked unavailable `sips`; the Windows PASS and global FAIL remain separate.

The historical OpenGL path’s shared renderer, real Material gallery, swap-based terminal ACK, basic input callbacks, semantics tree, and framework-dependent/self-contained publish paths were exercised for Linux Qt under Wayland and XWayland on a Kubuntu 26.04 VMware guest. Physical Linux, a real X11 session, Korean IME/Orca, forced context recreation, long soaks, and performance remain `notVerified`. See the archived [Linux Qt backend summary](../history/26-08-20/linux-qt-backend-summary.md).

AppKit live coverage and its remaining gates are recorded separately in the archived [AppKit dual-backend summary](../history/26-08-20/macos-appkit-dual-backend-summary.md). Build, native-live, browser-live, physical/device, and accessibility evidence do not substitute for one another.

## Source and artifact policy

- Product framework changes belong in `src/Doroti.Framework.*`; no compiler-owned `.g.cs` file is compiled there.
- Fix shared behavior at the lowest owning framework/runtime/rendering/host contract.
- Keep reference comparison, build, native live, browser live, physical, and cross-target claims distinct.
- `validation/contracts/` stores small machine-readable contracts consumed by active validators.
- `validation/` contains source and fixtures only. Generated output goes under `artifacts/validation/`; retained milestone evidence lives under `../history/`.
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

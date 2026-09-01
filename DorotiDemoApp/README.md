# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods the platform-workspace contract. The root project is target-neutral. Seven runner aliases are available; `macos` and `maccatalyst` are separate permanent products.

## Quick start

Install the .NET 10 SDK and PowerShell 7. Building the Web host from this source tree also requires Node.js 20 or newer and npm 10 or newer so the pinned CanvasKit assets can be restored and verified. Then run one of the following commands from the repository root. The first run can take a while because it restores packages and builds the selected runner.

```powershell
# Run the default Windows backend
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# Opt in to experimental Acrylic on Windows 11 24H2+
$env:DOROTI_DEMO_EXPERIMENTAL_ACRYLIC = '1'
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -Configuration Release

# Run the Web app (Release configuration by default)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web
```

After the Web runner starts, open `http://127.0.0.1:5088` in a browser. To compare renderer backends explicitly, use these addresses:

- Automatic selection: `http://127.0.0.1:5088`
- Document WebGL2: `http://127.0.0.1:5088/?dorotiRenderer=document-webgl`
- Split .NET UI Worker + CanvasKit Raster Worker (qualification opt-in): `http://127.0.0.1:5088/?dorotiRenderer=worker-canvaskit-webgl`
- Direct visible canvas in the persistent .NET Worker (qualification candidate): `http://127.0.0.1:5088/?dorotiRenderer=worker-direct-webgl`
- Same-thread OffscreenCanvas: `http://127.0.0.1:5088/?dorotiRenderer=offscreen-bitmap`
- Persistent .NET Worker: `http://127.0.0.1:5088/?dorotiRenderer=offscreen-worker`

`worker-canvaskit-webgl` is a forced opt-in mode. The main thread owns DOM/input/IME/semantics, logical CSS geometry, Worker supervision, restart policy, and canvas-lease replacement; the UI Worker owns the .NET runtime and a CPU text-layout CanvasKit instance; the Raster Worker owns the visible `OffscreenCanvas` and hardware-WebGL2 CanvasKit instance. If the explicitly requested mode cannot initialize its Workers, WebGL2 context, or packaged assets, it surfaces the failure instead of silently selecting an older renderer.

The Web host source build acquires the exact default variant of `canvaskit-wasm@0.42.0` under lockfile integrity. At runtime it uses only the same-origin JS/WASM packaged by `Doroti.Host.Web` under `/_content/Doroti.Host.Web/canvaskit/0.42.0/`; it does not download CanvasKit from a CDN or the npm registry. `canvaskit.manifest.json` in that path records the version, variant, lockfile integrity, byte length, and SHA-256 of every allowed file. The package also carries the type declarations and upstream `LICENSE`, and consuming applications do not need Node/npm during restore, build, or publish.

`auto` remains `document-webgl` until CanvasKit automatic correctness, performance, physical 60/120 Hz, trackpad, Korean IME, and screen-reader qualification all pass. A successful opt-in run does not promote the default.

### CanvasKit qualification evidence (2026-08-31)

The targeted hardware-WebGL2 Chromium suite passed `5/5`. Its automatic evidence covers main/UI/Raster ownership (`.NET` 0/1/0 and CanvasKit 0/1/1), exact terminal/receipt accounting, UI heartbeat and input dispatch during a 100 ms Raster stall, DPR2 CSS 1080×720 with a 2160×1440 backing and no CSS transform, three bounded Raster Worker/canvas-lease replacements with resource replay, and malformed-protocol rejection followed by bounded recovery.

The full headless CanvasKit-mode run passed `16`, skipped `5` tests that target other renderer modes, and failed `0`. A separate package audit packed all seven required Doroti packages and restored, built, and published a clean package consumer with Node/npm poison shims; no Node/npm invocation occurred, and the five CanvasKit asset hashes matched from source through nupkg to publish. Missing-license and one-byte-tamper packages failed closed with `DOROTICK101` and `DOROTICK102` respectively.

```powershell
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 `
  -Configuration Release `
  -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile tests/canvaskit-worker.spec.ts

pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 `
  -Configuration Release `
  -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile tests/canvaskit-display-list.spec.ts
```

This does not prove removal of all legacy `HostPayload` or Web SkiaSharp dependencies, same-worker Skia/pixel-golden parity, composed runtime-effect filters, complete retained-output caching, comparative performance, 30-minute memory churn, compositor scan-out, physical 60/120 Hz resize, precision-trackpad behavior, Korean IME, or screen-reader acceptance. The package still contains the legacy Web Skia dependencies, so that cutover gate is `FAIL`; the other listed gates remain `PARTIAL` or `notVerified`. Firefox and Safari are not promoted to supported product paths.

Press `Ctrl+C` in the terminal that launched the app to stop it. See [Run by platform](#run-by-platform) below for Android, iOS, macOS, Linux, and Windows MAUI commands.

## Layout

- `Program.cs`, `src/`, `assets/`: shared startup, widget tree, and application assets
- `doroti-workspace.json`: includes distinct `macos` (AppKit) and `maccatalyst` (UIKit) aliases
- `windowsappsdk/`: Windows App SDK 2.4 `HwndExactCpp` child-HWND runner with managed ANGLE/EGL-D3D11 Skia presentation.
- `windows/`: first-class MAUI backend runner and package identity
- `web/`: Blazor WebAssembly runner, TypeScript source, and `wwwroot`
- `android/`: .NET Android/MAUI runner plus the default Gradle AAR and .NET binding
- `ios/`: .NET iOS/MAUI runner plus an independent Xcode framework and binding
- `macos/`: native AppKit/osx-arm64 and Mac Catalyst/maccatalyst-arm64 runners, bindings, manifests, and lock files
- `linux/`: managed runner plus the app-owned CMake/Qt 6 C ABI shim

Generated bootstrap and plugin registration stay below each runner's `obj/<rid>/Doroti.Generated`. Platform icons, splash assets, manifests, entitlements, native source, outputs, and lock files remain in their owning platform directory.

## Commands

Run from the repository root. The workspace CLI resolves the runner path from `doroti-workspace.json`:
`build`, `run`, and `publish` use the Release configuration by default. Specify `-Configuration Debug` only when a Debug build is needed.

`-Configuration` accepts `Debug` or `Release` and defaults to `Release`. The selected value is forwarded directly to the Web runner as `dotnet run --configuration <value>`.

```powershell
# Run the Web app with a Release build (the default)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web -Configuration Release

# Run the Web app with a Debug build
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web -Configuration Debug
```

`ASPNETCORE_ENVIRONMENT=Development` in the Web `launchSettings.json` selects the ASP.NET Core hosting environment. It is independent of the `Debug`/`Release` build configuration that controls compilation optimizations and debug symbols. The default invocation therefore runs a **Release build in the Development hosting environment**. Check `Doroti artifact: configuration=...` in the terminal output to confirm the selected build configuration.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 doctor -App ./DorotiDemoApp -Platform all
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 publish -App ./DorotiDemoApp -Platform web
```

### Run by platform

Run every command below from the repository root. Run the Linux command on a Linux x64 host because that runner also builds its native shim.

```powershell
# Windows App SDK HwndExactCpp backend (current default)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# Independent Windows MAUI backend
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -WindowsBackend Maui

# Web
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web

# Android x64 emulator
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform android -Rid android-x64

# iOS arm64 simulator (Apple Silicon macOS + Xcode)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform ios -Rid iossimulator-arm64

# Native AppKit arm64 (experimental backend, macOS 14+, Apple Silicon)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform macos -Rid osx-arm64

# Mac Catalyst arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform maccatalyst -Rid maccatalyst-arm64

# Linux x64 (Qt 6 + CMake)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform linux -Rid linux-x64
```

### Connect an Android device or emulator

Android SDK Platform Tools and `adb` are required. Android Studio normally installs them under `%LOCALAPPDATA%\Android\Sdk\platform-tools`. If `adb` is not on `PATH`, set its path explicitly:

```powershell
$adb = "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe"
& $adb version
& $adb devices -l
```

A target is ready when the second column of the device list is `device`. When multiple devices or emulators are connected, pass the serial from the first column to `-Device`. You can omit `-Device` when exactly one target is connected.

#### Physical device over USB

1. On the Android device, open **About phone > Software information** and tap **Build number** repeatedly to enable Developer options.
2. Enable **Developer options > USB debugging**, then connect the device to the computer with USB.
3. Accept the RSA debugging prompt on the unlocked device and use `adb devices -l` to find its serial.
4. Use the `android-arm64` RID for a typical arm64 device.

```powershell
# Check the device ABI
& $adb -s <device-serial> shell getprop ro.product.cpu.abi

# Build, install, and run with Release + arm64 AOT
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform android `
  -Rid android-arm64 `
  -Configuration Release `
  -Device <device-serial>

# Faster development run with Debug
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform android `
  -Rid android-arm64 `
  -Configuration Debug `
  -Device <device-serial>
```

Replace `<device-serial>` with the value printed by `adb devices -l`, without the angle brackets. For example, if the serial is `R3CY30KZA4B`, pass `-Device R3CY30KZA4B`.

Android 11 and newer also support wireless debugging on the same network. Open **Developer options > Wireless debugging > Pair device with pairing code**, then use the displayed addresses and ports. The pairing port and connection port can be different.

```powershell
& $adb pair <device-ip>:<pairing-port>
& $adb connect <device-ip>:<debug-port>
& $adb devices -l
```

#### Android emulator

Create a virtual device with an x86_64 system image in Android Studio's **Device Manager**, then start it before running DorotiDemoApp. A booted emulator normally appears with a serial such as `emulator-5554`.

```powershell
& $adb devices -l

pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run `
  -App ./DorotiDemoApp `
  -Platform android `
  -Rid android-x64 `
  -Device emulator-5554
```

The current `android-x64` emulator Release uses the JIT/interpreter compatibility path to avoid a Mono AOT startup issue. Validate arm64 Release AOT behavior on a physical device with `android-arm64`.

#### Connection troubleshooting

- `unauthorized`: unlock the device, accept the RSA prompt, and reconnect USB.
- `offline`: run `& $adb kill-server` followed by `& $adb start-server`, then reconnect the device or emulator.
- Device not listed: check the USB mode, USB debugging, whether the cable supports data, and the manufacturer's Windows USB driver.
- More than one target: pass the exact serial from `adb devices -l` to `-Device`.
- Existing installation has a signing conflict: after confirming that its app data is not needed, run `& $adb -s <device-serial> uninstall dev.doroti.demo`, then run the app again.

After starting the Web runner, open `http://127.0.0.1:5088` in a browser. Android and iOS require a running emulator or simulator, respectively. To run on an Android arm64 device or an iOS arm64 device, change the RID to `-Rid android-arm64` or `-Rid ios-arm64`; a physical iOS device also requires code-signing configuration.

In addition to the .NET SDK and PowerShell 7, Linux requires Qt 6.5 or newer Core/Gui/Widgets/OpenGL, CMake, a C++ compiler, `pkg-config`, Wayland client development files, `wayland-scanner`, and the `wayland` or `xcb` QPA plugin.

Runner projects also support direct .NET commands:

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release
dotnet run --project ./DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Release
dotnet run --project ./DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj # MAUI backend
dotnet run --project ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj
dotnet build ./DorotiDemoApp/android/DorotiDemoApp.Android.csproj -c Release -r android-x64
dotnet build ./DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj -c Release -r iossimulator-arm64
dotnet build ./DorotiDemoApp/macos/DorotiDemoApp.MacOS.csproj -c Release -r osx-arm64
dotnet build ./DorotiDemoApp/macos/DorotiDemoApp.MacCatalyst.csproj -c Release -r maccatalyst-arm64
dotnet publish ./DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj -c Release -r linux-x64
```

The old `dotnet run --project DorotiDemoApp.csproj -p:DorotiTarget=...` path fails with `DOROTIAPP100` and points to the platform runner.

## Default native bridge

Every new app contains Android, iOS, native AppKit macOS, and Mac Catalyst native library/binding contracts. The native library does not replace the final application runner. Diagnose, build, or locate one workspace with:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native doctor -App ./DorotiDemoApp -Platform android
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native build -App ./DorotiDemoApp -Platform android -Rid android-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native open -App ./DorotiDemoApp -Platform ios
```

`native open` prints the Android Studio/Xcode project path; add `-Launch` only when the IDE should actually open. The bridge ABI provides `platformInfo`, `echo`, and UI-thread callback operations.

## System dark mode and color palettes

The demo passes light and dark `ThemeData` values plus `ThemeMode.system` to `MaterialApp`. Both palettes come from `ColorScheme.CreateFromSeed`, and widgets consume the active roles through `Theme.of(context).colorScheme`. The window configuration's `backgroundColor` and `darkBackgroundColor` follow the same light/dark transition.

On Linux the demo requests `WindowBackdropMode.acrylic` with `WindowBackdropFallback.transparent`. A Wayland compositor that advertises `ext-background-effect-v1` or the legacy KDE blur protocol receives a full-client-surface native blur request; otherwise, the configured transparent fallback is used. The alpha of the two background colors controls the acrylic tint strength. This protocol path is implemented, but visual acceptance of compositor blur remains `notVerified`.

## Support and evidence status

The default Windows route is the self-contained Windows App SDK 2.4 `HwndExactCpp` child-HWND host. Native C++ owns the top-level/child/task HWNDs and input/lifecycle ingress; managed code owns Doroti scene construction and hardware-D3D11 ANGLE/EGL/Skia raster and presentation. Each exact-size GPU backing is blitted to an `EGL_FIXED_SIZE_ANGLE` window surface, and a newly created surface is `DwmFlush`ed after its first swap before initial show or resize completion. An 8 ms refresh is active only while an interactive move straddles two monitors and no render is pending or in flight.

`DOROTI_DEMO_EXPERIMENTAL_ACRYLIC=1` selects a separate opt-in ContentIsland/Composition Swapchain path without throttling `WM_SIZING`. Actual child `WM_SIZE` waits at most 16 ms for a terminal during interactive resize and up to 100 ms for the latest exact frame at exit. `DwmFlush` is skipped per interactive frame and retained for exit/non-interactive exact presents. The ContentIsland uses a 1:1 scale for the already-physical surface; a dark top-HWND fill and 256 px retained overscan prevent newly exposed pixels from being erased white during fast top-left growth. Current 200%-DPI/165-Hz TopLeft automation passes at 43.66 fps for three seconds and 43.28 fps for 600 ms. Human rechecking of this binary plus IME, accessibility, and the full matrix remain `notVerified`; opaque remains the default.

The tested Windows physical resize and mixed-DPI boundary behavior received user acceptance. Strict synthetic input qualification and pixel/cadence failures remain failures rather than being reclassified by that acceptance. Automated input passed; automated IME/UIA and lifecycle/device coverage is partial. Physical Korean IME candidate/caret behavior, Narrator/Accessibility Insights, untested edge/speed/DPI/monitor combinations, device removal, installer/MSIX, and shutdown at every wait point remain `notVerified`. The explicit MAUI backend has its own evidence boundary and never serves as a silent fallback. The last full product-solution Release run on Windows failed after the Windows target passed because a macOS project invoked unavailable `sips`; use the target-scoped commands above for Windows work.

Automated builds include native AppKit and Mac Catalyst independently. The AppKit backend is experimental and not Microsoft-supported; its minimum OS is macOS 14 and its first RID is `osx-arm64`.

The archived AppKit record proves native launch, the visible Material gallery, Metal completion-based presentation, zero CPU readback/full-frame copies, a clean AppKit-only bundle, and all three native bridge operations. Pointer/keyboard/IME, accessibility, resize/fullscreen/scale migration, Release live behavior, and signing/notarization/store acceptance remain `notVerified`. Mac Catalyst evidence is never reported as true AppKit macOS evidence.

Linux Qt evidence covers the real Material gallery under Wayland, an XWayland/xcb input smoke, swap terminal ACK, 20/30 resize cycles, the semantics tree, and framework-dependent/self-contained publish on a Kubuntu 26.04 VMware guest. Physical Linux, a real X11 session, Korean IME/Orca, forced context recreation, visual acrylic-blur acceptance, and long-running performance remain `notVerified`.

See [ADR-021](../Doroti/docs/adr/ADR-021-platform-runner-workspaces.md), [ADR-025](../Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md), the archived [platform-runner workspace summary](../history/26-08-19/platform-runner-workspace-summary.md), [AppKit dual-backend summary](../history/26-08-20/macos-appkit-dual-backend-summary.md), and [Linux Qt backend summary](../history/26-08-20/linux-qt-backend-summary.md).

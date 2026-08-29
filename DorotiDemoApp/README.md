# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods the platform-workspace contract. The root project is target-neutral. Seven runner aliases are available; `macos` and `maccatalyst` are separate permanent products.

## Quick start

Install the .NET 10 SDK and PowerShell 7, then run one of the following commands from the repository root. The first run can take a while because it restores packages and builds the selected runner.

```powershell
# Run the default Windows backend
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# Run the Web app
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web
```

After the Web runner starts, open `http://127.0.0.1:5088` in a browser. To compare renderer backends explicitly, use these addresses:

- Automatic selection: `http://127.0.0.1:5088`
- Document WebGL2: `http://127.0.0.1:5088/?dorotiRenderer=document-webgl`
- Same-thread OffscreenCanvas: `http://127.0.0.1:5088/?dorotiRenderer=offscreen-bitmap`
- Persistent .NET Worker: `http://127.0.0.1:5088/?dorotiRenderer=offscreen-worker`

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

The tested Windows physical resize and mixed-DPI boundary behavior received user acceptance. Strict synthetic input qualification and pixel/cadence failures remain failures rather than being reclassified by that acceptance. Automated input passed; automated IME/UIA and lifecycle/device coverage is partial. Physical Korean IME candidate/caret behavior, Narrator/Accessibility Insights, untested edge/speed/DPI/monitor combinations, device removal, installer/MSIX, and shutdown at every wait point remain `notVerified`. The explicit MAUI backend has its own evidence boundary and never serves as a silent fallback. The last full product-solution Release run on Windows failed after the Windows target passed because a macOS project invoked unavailable `sips`; use the target-scoped commands above for Windows work.

Automated builds include native AppKit and Mac Catalyst independently. The AppKit backend is experimental and not Microsoft-supported; its minimum OS is macOS 14 and its first RID is `osx-arm64`.

The archived AppKit record proves native launch, the visible Material gallery, Metal completion-based presentation, zero CPU readback/full-frame copies, a clean AppKit-only bundle, and all three native bridge operations. Pointer/keyboard/IME, accessibility, resize/fullscreen/scale migration, Release live behavior, and signing/notarization/store acceptance remain `notVerified`. Mac Catalyst evidence is never reported as true AppKit macOS evidence.

Linux Qt evidence covers the real Material gallery under Wayland, an XWayland/xcb input smoke, swap terminal ACK, 20/30 resize cycles, the semantics tree, and framework-dependent/self-contained publish on a Kubuntu 26.04 VMware guest. Physical Linux, a real X11 session, Korean IME/Orca, forced context recreation, visual acrylic-blur acceptance, and long-running performance remain `notVerified`.

See [ADR-021](../Doroti/docs/adr/ADR-021-platform-runner-workspaces.md), [ADR-025](../Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md), the archived [platform-runner workspace summary](../history/26-08-19/platform-runner-workspace-summary.md), [AppKit dual-backend summary](../history/26-08-20/macos-appkit-dual-backend-summary.md), and [Linux Qt backend summary](../history/26-08-20/linux-qt-backend-summary.md).

# DorotiTestbedApp

**English** | [한국어](README.ko.md)

A sample and diagnostics app for Doroti's Material widgets and platform hosts.
One shared C# application runs through Windows, macOS AppKit, Mac Catalyst, Linux, Android, iOS, and Web runners.

The Material sample includes Components, Color, Typography, Elevation, nine seed colors,
six image themes, and local/URL image demos. **Diagnostics is the default screen**; the commands below explicitly open sample mode.

- [Prerequisites](#prerequisites)
- [Platform sample commands](#material-sample-mode)
- [Screen and renderer settings](#screen-and-renderer-settings)
- [Build and development](#build-and-development)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Run all commands in PowerShell 7 from the **repository root, `DorotiLab`**.
If your macOS/Linux shell is zsh/bash, enter `pwsh -NoProfile` first.
Use the **.NET SDK 10.0.400 feature band** selected by [global.json](../Doroti/global.json).
Platform-specific workloads and tools are listed with each command below.

The first run restores packages, builds, and deploys to a device where needed. All examples use Release.
For Debug, use `-c Debug` with `dotnet run` or `-Configuration Debug` with the workspace CLI.
Close a running native app before relaunching to apply a mode change.

## Material sample mode

[Windows](#windows-sample) · [macOS AppKit](#macos-appkit-sample) · [Mac Catalyst](#mac-catalyst-sample) · [Linux](#linux-sample) · [Android](#android-sample) · [iOS](#ios-sample) · [Web](#web-sample)

Native apps use `dotnet run -e` to pass `DOROTI_TESTBED_MODE=sample` to the app process.
Setting only a shell environment variable does not guarantee propagation to Apple/Android apps.
The accompanying `DOROTI_RESIZE_FIXTURE=none` disables F0/F1/F2 diagnostic fixtures, which take priority over the sample.
Web selects the mode through its URL.

### Windows sample

Run on a Windows host. This selects the default Windows App SDK/Vulkan runner.

```powershell
dotnet run --project ./DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

For the independent MAUI backend, replace the project in this command with
`./DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj`.

### macOS AppKit sample

Requires Apple Silicon, macOS 14 or later, and compatible Xcode/macOS workloads.
The `macos` runner uses native AppKit.

```powershell
dotnet run --project ./DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Release -r osx-arm64 `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

### Mac Catalyst sample

This UIKit runner requires Apple Silicon macOS and Xcode/Mac Catalyst workloads.
It uses the Mac UI idiom without the iPad compatibility mode's 77% downscaling.
Rendering and pointer coordinates use the Metal view's scale factor.
Trackpad scrolling decelerates after release; discrete mouse-wheel input receives no added inertia.
Both Mac runners open the text field editing menu with a secondary click or Control-click.
The menu follows Flutter Cupertino's continuous corners and blurred shadow;
hovered items use the theme accent and contrasting text color.
Run `dotnet run --project ./Doroti/validation/fcr7-material-widget -- --mac-text-menu`
to check light/dark hover and pressed states, shadows, and editing actions.

```powershell
dotnet run --project ./DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj -c Release -r maccatalyst-arm64 `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

### Linux sample

Run on a Linux x64 host. Requires Qt 6.5 or later Core/Gui/Widgets/OpenGL, CMake,
a C++ compiler, `pkg-config`, Wayland client development files, `wayland-scanner`,
and the `wayland` or `xcb` QPA plugin. The command also builds the native shim.

```powershell
dotnet run --project ./DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj -c Release -r linux-x64 `
  -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

### Android sample

Install the Android workload, Android SDK, and OpenJDK 17–21. Start an emulator or
connect a device with USB debugging enabled. See [Android connection instructions](#connect-an-android-device-or-emulator).

```powershell
# List available device IDs
dotnet run --project ./DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj --list-devices

# x64 emulator: replace emulator-5554 with its actual ID
dotnet run --project ./DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-x64 `
  --device emulator-5554 -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

For an arm64 device or emulator, use `-r android-arm64` and its `--device` ID.
Use `-c Debug` for faster development builds. Android environment settings also affect
the build, so do not use `--no-build` when changing modes.

**Galaxy phone over USB** (replace `device-serial` with the first column from `adb devices -l`):

```powershell
adb devices -l
adb -s device-serial shell getprop ro.product.cpu.abi

# Galaxy phone with the arm64-v8a ABI
dotnet run --project ./DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 `
  --device device-serial -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

After installation, reopen **Doroti Material Testbed** from the app drawer.
On 2026-09-09, a Release AOT APK was installed on a Galaxy `SM-S931N` using the ADB method below,
and the `Doroti Material 3` Components screen was visually confirmed. This did not validate all widgets, input, or performance.
If automatic deployment fails, see [Android build and deployment errors](#android-build-and-deployment-errors).

### iOS sample

Use Apple Silicon macOS with Xcode/iOS workloads, and start Simulator first.

```powershell
# List available simulator/device IDs
dotnet run --project ./DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj --list-devices

# Replace simulator-udid with an actual ID from the list
dotnet run --project ./DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Release -r iossimulator-arm64 `
  --device simulator-udid -e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none
```

For an Intel Mac simulator, use `-r iossimulator-x64`. A physical iPhone/iPad requires
`-r ios-arm64`, its device ID, and separate signing/provisioning configuration.
Here, `--device` belongs to `dotnet run`; the `doroti.ps1` wrapper's `-Device` currently supports Android only.

### Web sample

Start the server and leave this terminal open:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release
```

In a browser, [open the Material sample](http://127.0.0.1:5088/?dorotiTestbedMode=sample).
Web selects the screen with **`dorotiTestbedMode=sample` in the URL** instead of an environment variable.
Omitting the renderer option uses the default SkiaSharp direct Worker/WebGPU renderer.
Press `Ctrl+C` in the server terminal to stop it.

## Screen and renderer settings

### Return to diagnostics

Close the native app, replace `-e DOROTI_TESTBED_MODE=sample` with
`-e DOROTI_TESTBED_MODE=diagnostics` in its command, and run it again.
`-e` does not change the current shell environment. Clear any values previously set manually:

```powershell
Remove-Item Env:DOROTI_TESTBED_MODE -ErrorAction SilentlyContinue
Remove-Item Env:DOROTI_RESIZE_FIXTURE -ErrorAction SilentlyContinue
```

On Web, switch screens using the URLs below without restarting the server. Omitting the mode also opens diagnostics.

### Web renderers and measurement options

| Screen / renderer | Open |
| --- | --- |
| Material sample / default WebGPU | [Sample](http://127.0.0.1:5088/?dorotiTestbedMode=sample) |
| Material sample / WebGL2 | [WebGL2 sample](http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiRenderer=worker-direct-webgl) |
| Diagnostics / default WebGPU | [Diagnostics](http://127.0.0.1:5088/?dorotiTestbedMode=diagnostics) |
| Diagnostics / WebGL2 | [WebGL2 diagnostics](http://127.0.0.1:5088/?dorotiTestbedMode=diagnostics&dorotiRenderer=worker-direct-webgl) |

`dorotiRenderer` selects `worker-direct-webgpu` (default and `auto`) or `worker-direct-webgl`.
The Web host uses SkiaSharp WASM and Microsoft.TypeScript.MSBuild; no CanvasKit/npm restore is required.
After rebuilding a renderer, restart the runner and reload the page.
Check animation with the play button under Components → Communication → Progress indicators.

Add these query parameters to the sample URL only when measuring:

| Query | Purpose |
| --- | --- |
| `dorotiProgressScope=broad` | Compare a full-screen rebuild against the default `local` update |
| `dorotiResizeDiagnostics=1` | Enable framework work/type counters |
| `dorotiResizeDiagnostics=0&dorotiInputMarkers=1` | Minimal input-to-new-scene commit measurement; not physical display latency |

### Windows GPU and Acrylic

Windows App SDK defaults to Vulkan; ANGLE is an explicit alternative.
**The Material sample uses an opaque surface.** Use diagnostics to check Acrylic.
On Windows 11 24H2 or later, ordinary `WindowBackdropMode.acrylic` needs no experimental flag.

| Environment variable | Values / behavior |
| --- | --- |
| `DOROTI_WINDOWS_VULKAN_DEVICE` | Exact or unique GPU name fragment (such as `AMD`); overrides GPU preference for Vulkan |
| `DOROTI_WINDOWS_GPU_PREFERENCE` | `NoPreference` (default), `LowPowerPreference`, `HighPerformancePreference`; applies to Vulkan/ANGLE |
| `DOROTI_WINDOWS_PRESENTER` | `Vulkan` (default) or `AngleD3D11` |

For example, append `-e DOROTI_WINDOWS_GPU_PREFERENCE=HighPerformancePreference` to the Windows command above.
If you previously set a variable with `$env:`, clear it with `Remove-Item Env:VARIABLE_NAME` and relaunch.
`experimentalAcrylic` remains a compatibility option for reproducing earlier behavior.

### System dark mode and color palettes

`MaterialApp` follows the system theme through light/dark `ThemeData` and `ThemeMode.system`.
Palettes use `ColorScheme.CreateFromSeed`; widgets read `Theme.of(context).colorScheme`.
The window's `backgroundColor` and `darkBackgroundColor` follow the same transition.

Linux diagnostic windows request Acrylic with a transparent fallback. A Wayland compositor
supporting `ext-background-effect-v1` or the legacy KDE blur protocol receives a native blur request;
otherwise, the transparent fallback applies.

## Build and development

### Workspace CLI

[doroti-workspace.json](doroti-workspace.json) maps platforms to runner projects.
The CLI's `build`, `run`, and `publish` default to Release. To pass sample mode explicitly,
use the platform-specific `dotnet run -e` commands above.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 doctor -App ./DorotiTestbedApp -Platform all
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 build -App ./DorotiTestbedApp -Platform macos -Rid osx-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform macos -Rid osx-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 publish -App ./DorotiTestbedApp -Platform web
```

### Run by platform

Replace `-Platform` and `-Rid` in the CLI examples for your target.

| Target | `-Platform` | `-Rid` / additional options |
| --- | --- | --- |
| Windows App SDK | `windows` | Use the default |
| Windows MAUI | `windows` | `-WindowsBackend Maui` |
| macOS AppKit | `macos` | `osx-arm64` |
| Mac Catalyst | `maccatalyst` | `maccatalyst-arm64` |
| Linux | `linux` | `linux-x64` |
| Android | `android` | `android-x64` or `android-arm64`; select a device with `-Device` |
| iOS | `ios` | `iossimulator-arm64`, `iossimulator-x64`, or `ios-arm64` |
| Web | `web` | Omit |

Web's `ASPNETCORE_ENVIRONMENT=Development` is independent of the build configuration.
The default is a **Release build in the Development hosting environment**. Check
`Doroti artifact: configuration=...` in CLI output to confirm the build configuration.

The root `DorotiTestbedApp.csproj` is the shared app library.
`dotnet run --project DorotiTestbedApp.csproj -p:DorotiTarget=...` fails with `DOROTIAPP100`;
select a platform runner instead. Build the target you need rather than a full solution requiring tools for other operating systems.

### Native bridge

Android, iOS, AppKit, and Mac Catalyst include app-owned native libraries and bindings.
The default ABI provides `platformInfo`, `echo`, and UI-thread callbacks; it is separate from the final app runner.

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native doctor -App ./DorotiTestbedApp -Platform android
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native build -App ./DorotiTestbedApp -Platform android -Rid android-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native open -App ./DorotiTestbedApp -Platform ios
```

`native open` prints the Android Studio/Xcode project path. Add `-Launch` to open the IDE too.

### Project layout and resources

| Path | Purpose |
| --- | --- |
| `Program.cs`, `src/`, `assets/` | Shared startup, widget tree, and app resources |
| `doroti-workspace.json` | Platform aliases and runner paths |
| `windowsappsdk/`, `windows/` | Windows App SDK and independent MAUI runners |
| `web/` | WebAssembly Worker, TypeScript, and `wwwroot` |
| `android/`, `ios/` | Mobile runners, native projects, and bindings |
| `macos/` | Separate AppKit/Mac Catalyst runners, bindings, and manifests |
| `linux/` | Managed runner and CMake/Qt 6 C ABI shim |

Generated bootstrap and plugin registration live in `Doroti.Generated` below each runner's `obj` directory.
Platform icons, splash assets, entitlements, outputs, and lock files stay in their owning platform directory.
The application ID is `dev.doroti.testbed`, or `dev.doroti.testbed.macos` for AppKit.

Local WebP images and MaterialIcons/Roboto regular, medium, and bold fonts are embedded resources.
See [sample source](src/MaterialSample) and [fonts](assets/fonts) for licenses and provenance.
Image themes access `flutter.github.io`; the URL image demo accesses `plus.unsplash.com`.
Failed theme loads preserve the last successful theme and offer Retry.
Display images are decoded at up to `1024 × DPR` pixels wide while preserving aspect ratio;
`Extract colors` uses the original image for palette extraction.
External-link success means the host accepted the request; Web popup blocking is shown in the UI.

## Troubleshooting

### Connect an Android device or emulator

Add `adb` from Android SDK Platform Tools to `PATH`. With Android Studio's default Windows installation,
you can also run `& "$env:LOCALAPPDATA/Android/Sdk/platform-tools/adb.exe" devices -l`.

1. On a physical device, enable **USB debugging** in Developer options, connect USB, and accept the RSA prompt.
2. For an emulator, check the system image ABI in Android Studio's **Device Manager** and start it first.
3. Check for the `device` status in `adb devices -l`. Pass the serial in its first column to the sample command's `--device`.
4. Check the ABI with `adb -s device-serial shell getprop ro.product.cpu.abi` and select `android-arm64` or `android-x64`.

Replace `device-serial`, `device-ip`, and ports with actual values.
For wireless connections on Android 11 or later, use the address shown under
**Wireless debugging → Pair device with pairing code**. Pairing and connection ports can differ.

```powershell
adb pair device-ip:pairing-port
adb connect device-ip:debug-port
adb devices -l
```

| Symptom | Check / fix |
| --- | --- |
| `unauthorized` | Unlock the device, accept the RSA prompt, and reconnect USB |
| `offline` | Run `adb kill-server`, then `adb start-server`, and reconnect |
| Not listed | Check USB debugging, data cable, USB mode, and the manufacturer's Windows driver |
| Multiple targets | Supply the exact serial with `--device`, or `-Device` for the workspace CLI |
| Signing conflict | If you can discard the app's data, run `adb -s device-serial uninstall dev.doroti.testbed` and reinstall |

`android-x64` Release uses a JIT/interpreter compatibility path to avoid a Mono AOT startup issue.
Validate arm64 Release AOT separately on a physical `android-arm64` device.

### Android build and deployment errors

**SkiaSharp version conflict (`MSB3277`, `CS1705`)**: if the Android host references an older
SkiaSharp version than the current package configuration, refresh both restore caches below,
then repeat the sample run command. This resolved a `4.152` / `4.154` conflict on 2026-09-09.

```powershell
dotnet restore ./Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj `
  -p:RuntimeIdentifier=android-arm64 -p:TargetFramework=net10.0-android --force-evaluate -v minimal
dotnet restore ./Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj `
  -p:TargetFramework=net10.0-android -p:DorotiHostTargetFrameworks=net10.0-android --force-evaluate -v minimal
```

**Automatic deployment fails after APK creation (`DOTNET_HOST_PATH`, `MSB4221`, `MSB4027`)**:
on 2026-09-09, these errors stopped the `DeployToDevice` stage of `dotnet run`.
If a signed APK has just been built in sample mode, install and launch it directly with ADB.
The path below is the `android-arm64` Release output; distinguish it from an older APK in `publish/`.
Check that its modification time matches the current build before installing.

```powershell
$sampleApk = './DorotiTestbedApp/android/bin/android-arm64/Release/net10.0-android/android-arm64/dev.doroti.testbed-Signed.apk'
Get-Item $sampleApk | Select-Object FullName, LastWriteTime, Length
adb -s device-serial install -r --user 0 $sampleApk
adb -s device-serial shell monkey -p dev.doroti.testbed -c android.intent.category.LAUNCHER 1
```

Replace `device-serial` with the connected device's serial. `--user 0` installs for the primary user.
If another profile, such as Galaxy Secure Folder, causes a shell permission error, scope package queries too:
`adb -s device-serial shell pm list packages --user 0 dev.doroti.testbed`.

### The app opens without the sample

- Native: close the app and rerun with `-e DOROTI_TESTBED_MODE=sample -e DOROTI_RESIZE_FIXTURE=none`.
- Android: rebuild and reinstall without `--no-build` when changing modes.
- Web: include `dorotiTestbedMode=sample` in the URL. Renderer selection is separate from screen selection.
- iOS: check that Simulator is running and the `--device` ID is correct. Physical devices also need signing/provisioning.

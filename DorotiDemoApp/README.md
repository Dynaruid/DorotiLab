# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods the platform-workspace contract. The root project is target-neutral. Seven runner aliases are available; `macos` and `maccatalyst` are separate permanent products.

## Layout

- `Program.cs`, `src/`, `assets/`: shared startup, widget tree, and application assets
- `doroti-workspace.json`: includes distinct `macos` (AppKit) and `maccatalyst` (UIKit) aliases
- `windows/`: WinUI/MAUI runner and package identity
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
# Windows (Windows host)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows

# Web
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web

# Android x64 emulator
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform android -Rid android-x64

# iOS x64 simulator (macOS + Xcode)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform ios -Rid iossimulator-x64

# Native AppKit arm64 (experimental backend, macOS 14+, Apple Silicon)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform macos -Rid osx-arm64

# Mac Catalyst arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform maccatalyst -Rid maccatalyst-arm64

# Linux x64 (Qt 6 + CMake)
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform linux -Rid linux-x64
```

After starting the Web runner, open `http://127.0.0.1:5088` in a browser. Android and iOS require a running emulator or simulator, respectively. To run on an Android arm64 device or an iOS arm64 device, change the RID to `-Rid android-arm64` or `-Rid ios-arm64`; a physical iOS device also requires code-signing configuration.

In addition to the .NET SDK and PowerShell 7, Linux requires Qt 6.5 or newer Core/Gui/Widgets/OpenGL, CMake, a C++ compiler, `pkg-config`, Wayland client development files, `wayland-scanner`, and the `wayland` or `xcb` QPA plugin. Run the Linux-native contract/build/publish validation with:

```bash
bash ./Doroti/eng/validate-linux-qt.sh Release
```

Runner projects also support direct .NET commands:

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release
dotnet run --project ./DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj
dotnet run --project ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj
dotnet build ./DorotiDemoApp/android/DorotiDemoApp.Android.csproj -c Release -r android-x64
dotnet build ./DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj -c Release -r iossimulator-x64
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

Automated builds include native AppKit and Mac Catalyst independently. The AppKit backend is experimental and not Microsoft-supported; its minimum OS is macOS 14 and its first RID is `osx-arm64`.

The AppKit product live record proves native launch, the visible Material gallery, Metal completion-based presentation, zero CPU readback/full-frame copies, a clean AppKit-only bundle, and all three native bridge operations. Pointer/keyboard/IME, accessibility, resize/fullscreen/scale migration, Release live behavior, and signing/notarization/store acceptance remain `notVerified`. Mac Catalyst evidence is never reported as true AppKit macOS evidence.

Linux Qt evidence covers the real Material gallery under Wayland, an XWayland/xcb input smoke, swap terminal ACK, 20/30 resize cycles, the semantics tree, and framework-dependent/self-contained publish on a Kubuntu 26.04 VMware guest. Physical Linux, a real X11 session, Korean IME/Orca, forced context recreation, visual acrylic-blur acceptance, and long-running performance remain `notVerified`.

See [ADR-021](../Doroti/docs/adr/ADR-021-platform-runner-workspaces.md), the [workspace evidence](../Doroti/validation/evidence/app-targets-evidence.json), the [AppKit product live record](../Doroti/validation/evidence/appkit-macos/product-live.json), the [Linux Qt live evidence](../Doroti/validation/evidence/linux-qt/kubuntu-vmware-spike.json), and the [Linux Qt packaging evidence](../Doroti/validation/evidence/linux-qt/kubuntu-packaging.json).

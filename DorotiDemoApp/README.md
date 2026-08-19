# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods the platform-workspace contract. The root `DorotiDemoApp.csproj` is a platform-neutral `net10.0` library; each lower-case platform directory owns an independently buildable and runnable application project.

## Layout

- `Program.cs`, `src/`, `assets/`: shared startup, widget tree, and application assets
- `doroti-workspace.json`: `android`, `ios`, `linux`, `macos`, `web`, and `windows` aliases to runner projects
- `windows/`: WinUI/MAUI runner and package identity
- `web/`: Blazor WebAssembly runner, TypeScript source, and `wwwroot`
- `android/`: .NET Android/MAUI runner plus the default Gradle AAR and .NET binding
- `ios/`: .NET iOS/MAUI runner plus an independent Xcode framework and binding
- `macos/`: Mac Catalyst runner plus an independent Xcode framework and binding; this is not true AppKit macOS
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

Runner projects also support direct .NET commands:

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release
dotnet run --project ./DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj
dotnet run --project ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj
dotnet build ./DorotiDemoApp/android/DorotiDemoApp.Android.csproj -c Release -r android-x64
dotnet build ./DorotiDemoApp/ios/DorotiDemoApp.iOS.csproj -c Release -r iossimulator-x64
dotnet build ./DorotiDemoApp/macos/DorotiDemoApp.MacCatalyst.csproj -c Release
dotnet publish ./DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj -c Release -r linux-x64
```

The old `dotnet run --project DorotiDemoApp.csproj -p:DorotiTarget=...` path fails with `DOROTIAPP100` and points to the platform runner.

## Default native bridge

Every new app already contains Android, iOS, and Mac Catalyst native library plus binding projects. The native library does not replace the final application runner. Diagnose, build, or locate one workspace with:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native doctor -App ./DorotiDemoApp -Platform android
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native build -App ./DorotiDemoApp -Platform android -Rid android-arm64
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 native open -App ./DorotiDemoApp -Platform ios
```

`native open` prints the Android Studio/Xcode project path; add `-Launch` only when the IDE should actually open. The bridge ABI provides `platformInfo`, `echo`, and UI-thread callback operations.

## Support and evidence status

Automated Release builds are available for the neutral app, Windows, Web, Android arm64/x64, Mac Catalyst cross-build, iOS device/simulator cross-build, and the managed Linux runner. Android Gradle and iOS Xcode binding scaffolds also compile in the available cross-build environment.

Those results do not prove native launch, GPU presentation, browser interaction, physical device behavior, accessibility, signing/store acceptance, or Linux X11/Wayland behavior. The current platform-workspace evidence keeps those unrun gates `notVerified`; Mac Catalyst evidence is never reported as true AppKit macOS evidence.

See [ADR-021](../Doroti/docs/adr/ADR-021-platform-runner-workspaces.md) and the [workspace evidence](../Doroti/validation/evidence/app-targets-evidence.json).

# DorotiDemoApp

[한국어](README.ko.md) | **English**

A standalone Material components demo copied from the **Components** page in
`samples/DorotiTestbedApp`. It opens the component gallery directly, with light/dark
mode, responsive one/two-column layout, dialogs, drawers, input controls, and the
local/URL image demo.

Run these commands in PowerShell from the repository root:

```powershell
# Windows App SDK
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiDemoApp -Platform windows

# Web
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./samples/DorotiDemoApp -Platform web
```

The workspace also contains Android, iOS, Linux Qt, macOS AppKit, and Mac Catalyst
runners. Select `android`, `ios`, `linux`, `macos`, or `maccatalyst` with `-Platform`
on a suitable build host. See the [framework guide](../../Doroti/README.md) for
platform prerequisites.

The [deployment helper](../../helpers/deploy-helper/README.md) accepts `-App Demo`
to build, install, and launch this sample on Android or iOS.

To build only the shared application or the Windows runner:

```powershell
dotnet build ./samples/DorotiDemoApp/DorotiDemoApp.csproj -c Debug
dotnet build ./samples/DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Debug
```

`src/Components/{Components,Selection,Decorations,ImageDemo}.cs` and the bundled
Roboto fonts/local image are independent copies of the Testbed page and its
resources. Platform scaffolding follows `DorotiCarouselApp`. The copied Flutter
license is in `src/Components/LICENSE.flutter`; the Roboto license is in
`assets/fonts/LICENSE.roboto.txt`.

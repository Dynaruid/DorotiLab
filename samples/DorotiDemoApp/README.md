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

# Web: recommended Release AOT publish and local preview
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo
```

Accept the marked defaults with Enter for .NET 10 / Release / Mono WASM AOT / threaded.
The first AOT publish takes longer. The helper opens the browser; `Ctrl+C` stops its
temporary server and exits the helper. Use `-Mode Mono` for an explicit non-AOT comparison.
Ordinary `run -Configuration Release` does not run the AOT publish output.

The workspace also contains Android, iOS, Linux Qt, macOS AppKit, and Mac Catalyst
runners. Select `android`, `ios`, `linux`, `macos`, or `maccatalyst` with `-Platform`
on a suitable build host. See the [framework guide](../../Doroti/README.md) for
platform prerequisites.

Web publishing defaults to net10/threaded and Release Mono WASM AOT. Use `-p:DorotiWebExecutionMode=auto`
with `dotnet publish` to include both complete variants, or `compatible` for a
single non-isolated deployment. Restart run/watch and reload the page after mode
changes; compatible Hot Reload is unqualified. See [Web profiles and migration](../../packages/platforms/web/README.md#execution-profiles-opt-in).

The [deployment helper](../../helpers/deploy-helper/README.md#web) accepts `-App Demo`
for Web publish/preview and Android/iOS build, install, and launch.

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

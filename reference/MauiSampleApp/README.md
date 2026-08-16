# Doroti MAUI C# Markup GPU feasibility probe

This M0 fixture validates the revised single-project MAUI contract from
`work.md`: all application UI is built in C#, while Windows is allowed one
empty `Platforms/Windows/App.xaml` application definition solely for WinUI
resource and generated-entrypoint initialization. No page, shell, style, or
application UI is defined in XAML.

The fixture uses .NET 10, Microsoft.Maui.Controls 10.0.90,
CommunityToolkit.Maui.Markup 8.0.0, and SkiaSharp.Views.Maui.Controls 3.119.4.
`MauiProgram` registers `UseMauiCommunityToolkitMarkup()`, and the shared
application still constructs `Application -> Window -> ContentPage -> SKGLView`
entirely in C#.

The Windows `ApplicationDefinition` is required. Removing it and replacing the
generated entrypoint with `Application.Start` builds, but live startup fails
while WinUI constructs `XamlControlsResources` because application theme
resources have not been initialized. The Markup Toolkit replaces UI XAML; it
does not replace this WinUI bootstrap contract.

Build and publish with:

```powershell
dotnet restore .\reference\MauiSampleApp\MauiSampleApp.csproj --locked-mode
dotnet build .\reference\MauiSampleApp\MauiSampleApp.csproj `
  -f net10.0-windows10.0.19041.0 -c Debug --no-restore
dotnet publish .\reference\MauiSampleApp\MauiSampleApp.csproj `
  -f net10.0-windows10.0.19041.0 -c Release --no-restore
```

Set `DOROTI_MAUI_FEASIBILITY_EVIDENCE` to capture runtime GPU evidence. By
default the app quits after three demand-driven frames. Set
`DOROTI_MAUI_FEASIBILITY_AUTO_QUIT_FRAMES=0` to keep it open for resize and
lifecycle probes.

Verified on Windows on 2026-08-16:

- Debug build and locked restore: PASS, zero warnings/errors.
- Release publish/live: exit 0 after three frames.
- Native view: `SKGLViewHandler+MauiSKSwapChainPanel`.
- GPU context and GPU surface: non-null; no `SKCanvasView` fallback.
- Demand-driven rendering: `HasRenderLoop=false`.
- Live resize: surface changed from 1894x1014 to 1414x1003 pixels at density 2,
  then the window closed with exit 0.
- XAML scope: exactly one Windows bootstrap `ApplicationDefinition`; zero
  shared/page/shell/style XAML files.

Mac Catalyst compiles package-only on the Windows host with zero warnings and
errors. `SKMetalView`/Metal presentation, arm64 publish, resize/scale, and clean
shutdown remain `notVerified` until run on a Mac arm64 host.

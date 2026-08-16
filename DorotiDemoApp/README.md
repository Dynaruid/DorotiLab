# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods Doroti's product-owned framework and single-project application contract. One `DorotiDemoApp.csproj`, root `Program.cs`, and shared `src/App.cs` target MAUI Windows, MAUI Mac Catalyst, or Blazor WebAssembly.

## Layout

- `src/App.cs`: target-neutral Material widget/state tree using `Doroti.Framework.*`
- `Program.cs`: thin Web and Mac Catalyst bootstrap; Windows uses the SDK-generated WinUI entrypoint
- `Platforms/Maui`: shared C# `Application`, `Window`, `ContentPage`, and `SKGLView`
- `Platforms/Windows`: the only bootstrap `App.xaml`, code-behind, and manifests
- `Platforms/MacCatalyst`: UIKit bootstrap, delegate, plist, and entitlements
- `Platforms/Web`: Blazor composition root and static assets

## Build and validation

```powershell
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64

pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

The developer suite checks all three target graphs and builds. The release suite adds an actual Windows `MauiSKSwapChainPanel` GPU frame and an external Web template/package publish scenario.

Mac Catalyst cross-build is verified from Windows; native publish/run still requires Apple Silicon macOS. Native hover/wheel/capture/keyboard/IME/UIA, browser live automation, physical acceptance, and cross-target parity remain separate `notVerified` gates.

Current evidence is written to [app target evidence](../Doroti/migration/maui/app-targets-evidence.json) and [Web product evidence](../Doroti/migration/web/web-product-evidence.json) when the corresponding release suite completes. Historical Win32/AppKit and G4-G7 evidence remains predecessor-only.

See the [Doroti runtime README](../Doroti/README.md) for source ownership and command details.

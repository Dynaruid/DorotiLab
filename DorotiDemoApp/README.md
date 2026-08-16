# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods Doroti's single-project application contract. One `DorotiDemoApp.csproj`, one root `Program.cs`, and one shared `src/App.cs` select MAUI Windows, MAUI Mac Catalyst, or Blazor WebAssembly without a second host project.

## Layout

- `src/App.cs`: target-neutral reviewed Material widget/state tree
- `Program.cs`: thin target bootstrap for Web and Mac Catalyst; Windows uses generated WinUI `Main`
- `Platforms/Maui`: shared C# `Application`, `Window`, `ContentPage`, and `SKGLView` composition
- `Platforms/Windows`: the only bootstrap `App.xaml`, WinUI code-behind, and package manifests
- `Platforms/MacCatalyst`: UIKit entrypoint, delegate, plist, and entitlements
- `Platforms/Web`: Blazor composition root and static assets

There is exactly one application project. Windows compiles one `ApplicationDefinition` and zero `MauiXaml` items; Mac Catalyst and Web compile no XAML.

## Build and run

From the repository root:

```powershell
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64
```

The Mac Catalyst compile graph passes as a Windows-host cross-build. Publish and native execution still require a suitable Apple Silicon macOS runner and remain `notVerified`.

## Validation status

Run the current single-project gates with:

```powershell
pwsh -File ./Doroti/eng/validate-g7-maui-single-project.ps1 -Shard All
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Graph
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Template
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Compile
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Publish
```

Current evidence proves Windows Release build/publish and a live GPU frame through `MauiSKSwapChainPanel` with the `win-x64/winui3/SKSwapChainPanel/ANGLE-DirectX-Skia` identity, plus Web package-only compile/publish. Basic MAUI touch translation and clipboard/focus adapters exist, but native hover/wheel/capture/keyboard/IME/UIA, resize/DPI/context recreation, Mac Catalyst native execution, physical acceptance, and cross-target parity remain `notVerified`.

The manual Chromium smoke from 2026-08-15 remains scoped to the official Web publish artifact: non-empty GPU canvas, DPR sizing, bounded backdrop blur, two-pass shadows, semantics, a basic pointer state change, and zero console errors. It does not prove the remaining Web automation or physical gates.

## Evidence

- [MAUI single-project evidence](../Doroti/migration/maui/g7-maui-single-project-evidence.json)
- [Web build evidence](../Doroti/migration/web/g7-web-build-evidence.json)
- Historical Win32/AppKit evidence remains predecessor-only and is not promoted to MAUI PASS.

For runtime architecture and development commands, see the [Doroti runtime README](../Doroti/README.md).

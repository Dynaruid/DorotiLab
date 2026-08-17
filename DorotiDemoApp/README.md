# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods Doroti's product-owned framework and single-project application contract. One `DorotiDemoApp.csproj`, root `Program.cs`, and shared `src/App.cs` target MAUI Windows, MAUI Mac Catalyst, or Blazor WebAssembly.

The application is C#-only. Its build and validation flows do not generate or consume a `DorotiDemoApp/dart` package.

## Layout

- `Program.cs`: the public, target-neutral `IDorotiApplicationStartup` used by every target
- `src/App.cs`: the target-neutral Material widget/state tree and view configuration selected by `Program`
- `Platforms/Windows`: thin WinUI shell, optional platform hook, and manifests
- `Platforms/MacCatalyst`: thin UIKit delegate, optional platform hook, plist, and entitlements
- `Platforms/Web/src`: user-owned TypeScript bootstrap policy and browser plugins
- `Platforms/Web/wwwroot`: handwritten HTML, CSS-adjacent resources, locale/assets, and manifest files; generated JavaScript is forbidden here
- `obj/web/<configuration>/net10.0/Doroti.Generated/wwwroot`: compiled application bootstrap/plugin JavaScript
- `obj/<target>/Doroti.Generated`: SDK-owned C# bootstrap and plugin registration; never edit these files

`doroti_bootstrap.ts` configures loading/error UI and typed Blazor startup hooks through Doroti's loader. Only `doroti.loader.ts` calls `Blazor.start()`. Release publish exposes `doroti_bootstrap.js`, `plugins/echo.js`, `_content/Doroti.Host.Web/doroti.loader.js`, and `_content/Doroti.Host.Web/doroti.web.js`; it does not expose TypeScript source, `tsconfig.json`, or compiler assets.

## Build and validation

```powershell
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64

pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

The developer suite checks the shared descriptor, generated bootstrap, synthetic fourth-host extension point, and all three target graphs/builds. The release suite adds an actual Windows `MauiSKSwapChainPanel` GPU frame and an external Web template/package publish scenario. The repository pins SDK 10.0.400 and the Web runtime to 10.0.11; a runtime-version stamp invalidates stale WebCIL publish caches after patch changes.

Mac Catalyst cross-build is verified from Windows; native publish/run still requires Apple Silicon macOS. A manual Chromium smoke verified the published WebGL2 canvas, zero page-console errors, and one rendered FAB click changing `fab=0` to `fab=1`. Keyboard/IME/clipboard/resize/interactive ARIA, native hover/wheel/capture/keyboard/IME/UIA, physical acceptance, and cross-target parity remain separate `notVerified` gates.

Current evidence is written to [app target evidence](../Doroti/migration/maui/app-targets-evidence.json), [Web product evidence](../Doroti/migration/web/web-product-evidence.json), and [manual browser evidence](../Doroti/migration/web/web-browser-live-manual.json). Historical Win32/AppKit and G4-G7 evidence remains predecessor-only.

See the [Doroti runtime README](../Doroti/README.md) for source ownership and command details.

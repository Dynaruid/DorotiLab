# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp dogfoods Doroti's product-owned framework and single-project application contract. One `DorotiDemoApp.csproj`, root `Program.cs`, and shared `src/App.cs` target MAUI Windows, MAUI Mac Catalyst, MAUI Android, or Blazor WebAssembly.

The application is C#-only. Its build and validation flows do not generate or consume a `DorotiDemoApp/dart` package.

## Layout

- `Program.cs`: the public, target-neutral `IDorotiApplicationStartup` used by every target
- `src/App.cs`: the target-neutral Material widget/state tree and view configuration selected by `Program`
- `Platforms/Windows`: thin WinUI shell, optional platform hook, and manifests
- `Platforms/MacCatalyst`: thin UIKit delegate, optional platform hook, plist, and entitlements
- `Platforms/Android`: thin `MainApplication`/`MainActivity` shell and Android manifest
- `Platforms/Web/src`: user-owned TypeScript bootstrap policy and browser plugins
- `Platforms/Web/wwwroot`: handwritten HTML, CSS-adjacent resources, locale/assets, and manifest files; generated JavaScript is forbidden here
- `obj/web/<configuration>/net10.0/Doroti.Generated/wwwroot`: compiled application bootstrap/plugin JavaScript
- `obj/<target>/Doroti.Generated`: SDK-owned C# bootstrap and plugin registration; never edit these files

`doroti_bootstrap.ts` configures loading/error UI and typed Blazor startup hooks through Doroti's loader. Only `doroti.loader.ts` calls `Blazor.start()`. Release publish exposes `doroti_bootstrap.js`, `plugins/echo.js`, `_content/Doroti.Host.Web/doroti.loader.js`, and `_content/Doroti.Host.Web/doroti.web.js`; it does not expose TypeScript source, `tsconfig.json`, or compiler assets.

## Run

Commands are from the repository root. Each target restores against its own lock file (`packages.windows.lock.json`, `packages.maccatalyst.lock.json`, `packages.android.lock.json`, `packages.android-x64.lock.json`, `packages.web.lock.json`). SDK 10.0.400 and the `maui` / `wasm-tools` workloads are required.

### Windows

Host: Windows. RID `win-x64`. GPU surface is `MauiSKSwapChainPanel`.

```powershell
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Windows
```

### Mac Catalyst

Host: Apple Silicon macOS. RID `maccatalyst-arm64`. GPU surface is `MauiSKMetalView`.

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=MacCatalyst --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=MacCatalyst
```

### Android

Connect one device or emulator (`adb devices`) before `dotnet run`. Default RID is `android-arm64`.

Physical arm64 device:

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64 --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64
```

x86_64 emulator:

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Android -p:RuntimeIdentifier=android-x64 --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Android -p:RuntimeIdentifier=android-x64
```

If several devices are attached, add `-p:AdbTarget=-s:<serial>`.

### Web

Host: any OS with the WebAssembly workload. RID `browser-wasm`. `dotnet run` starts the Blazor development server.

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Web --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Web
```

Static publish:

```bash
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
```

## Build and validation

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Windows -p:RuntimeIdentifier=win-x64
dotnet build ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64

pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Fcr8
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
pwsh -File ./Doroti/eng/validate-fcr8-stability.ps1 -Shard WindowsLive
pwsh -File ./Doroti/eng/validate-fcr8-stability.ps1 -Shard AndroidPhysical -AndroidSerial <adb-serial>
pwsh -File ./Doroti/eng/validate-fcr8-stability.ps1 -Shard Soak -SoakSeconds 300
```

The developer suite checks the shared descriptor, generated bootstrap, custom shader contract, synthetic fourth-host extension point, all four target graphs/builds, and the FCR-8 `Inventory`/`Contracts`/`Differential`/`Evidence` representative gates. Actual Windows, Android, and soak acceptance remain explicit shards. Android physical acceptance requires the .NET Android workload, Android SDK/JDK, and an arm64 physical device. The release suite adds an actual Windows `MauiSKSwapChainPanel` GPU frame and an external Web template/package publish scenario. The repository pins SDK 10.0.400 and the Web runtime to 10.0.11; a runtime-version stamp invalidates stale WebCIL publish caches after patch changes.

Custom shaders use SkiaSharp 4.151.1 and SkSL on every current target. Use `FragmentProgram.fromSource(...)` or register a `Resources/Shaders/*.sksl` asset in each application manifest and load it with `await FragmentProgram.fromAsset(...)`; set float/image-sampler uniforms on its `FragmentShader`, then use it from `Paint.shader`, `ShaderMask.shaderCallback`, or `ImageFilter.shader`. `ImageFilter.shader` captures its bounded child into an offscreen texture on the same GPU context, writes the texture size into the first `float2` uniform, and binds the filtered child to the first shader sampler as its implicit input. Flutter's Android stretch now uses an embedded SkSL program through that path. GPU-surface creation, compilation, and binding fail closed rather than selecting a software renderer.

Mac Catalyst cross-build is verified from Windows; native publish/run still requires Apple Silicon macOS. The current Chromium live run verifies the visible GPU surface, checkbox/radio/switch/slider/button/FAB pointer transitions, wheel scrolling, and a keyboard semantics action with zero console errors on Web runtime 10.0.11. Windows live verifies eight native wheel/drag events, two resize transitions, swap-chain replay, shader frames, and zero failed or software-fallback frames. Android physical, paired Flutter raster differential, soak/resource plateau, and IME/TalkBack/stylus/mouse acceptance remain `notVerified` until run.

Current evidence is written to [FCR-8 stability evidence](../Doroti/validation/evidence/flutter-conformance/fcr8-stability-evidence.json), [app target evidence](../Doroti/validation/evidence/app-targets-evidence.json), [Web product evidence](../Doroti/validation/evidence/web/web-product-evidence.json), and [manual browser evidence](../Doroti/validation/evidence/web/web-browser-live-manual.json). Historical Win32/AppKit and G4-G7 evidence remains predecessor-only.

See the [Doroti runtime README](../Doroti/README.md) for source ownership and command details.

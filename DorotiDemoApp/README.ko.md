# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 제품이 직접 소유하는 Doroti framework와 single-project application 계약을 사용하는 dogfood 앱입니다. 하나의 `DorotiDemoApp.csproj`, root `Program.cs`, 공용 `src/App.cs`가 MAUI Windows, MAUI Mac Catalyst, MAUI Android 또는 Blazor WebAssembly를 target으로 선택합니다.

이 application은 C# 전용입니다. Build와 validation flow는 `DorotiDemoApp/dart` package를 생성하거나 소비하지 않습니다.

## 구성

- `Program.cs`: 모든 target이 사용하는 public target-neutral `IDorotiApplicationStartup`
- `src/App.cs`: `Program`이 선택하는 target-neutral Material widget/state tree와 view configuration
- `Platforms/Windows`: 얇은 WinUI shell, 선택적 platform hook과 manifest
- `Platforms/MacCatalyst`: 얇은 UIKit delegate, 선택적 platform hook, plist와 entitlement
- `Platforms/Android`: 얇은 `MainApplication`/`MainActivity` shell과 Android manifest
- `Platforms/Web/src`: 사용자 소유 TypeScript bootstrap 정책과 browser plugin
- `Platforms/Web/wwwroot`: 직접 작성한 HTML, CSS 인접 resource, locale/asset, manifest. 생성 JavaScript는 두지 않음
- `obj/web/<configuration>/net10.0/Doroti.Generated/wwwroot`: compile된 앱 bootstrap/plugin JavaScript
- `obj/<target>/Doroti.Generated`: SDK 소유 C# bootstrap과 plugin registration. 직접 수정하지 않음

`doroti_bootstrap.ts`는 Doroti loader를 통해 loading/error UI와 typed Blazor startup hook을 구성합니다. `Blazor.start()`는 `doroti.loader.ts`만 호출합니다. Release publish에는 `doroti_bootstrap.js`, `plugins/echo.js`, `_content/Doroti.Host.Web/doroti.loader.js`, `_content/Doroti.Host.Web/doroti.web.js`가 있으며 TypeScript source, `tsconfig.json`, compiler asset은 포함하지 않습니다.

## 시스템 다크 모드와 색 팔레트

Demo와 새 `doroti-app` template은 Flutter와 같은 `MaterialApp` 계약을 사용합니다. MAUI host는 Windows, Mac Catalyst, Android의 시스템 theme 변경을, Web host는 `prefers-color-scheme` 변경을 `MediaQuery.platformBrightnessOf(context)`에 전달합니다. `ThemeMode.system`은 이 값에 따라 `theme`과 `darkTheme`을 런타임에 선택합니다.

밝은/어두운 팔레트는 같은 seed에서 만들거나 각 색 role을 별도로 덮어쓸 수 있습니다.

```csharp
var lightPalette = Material.ColorScheme.CreateFromSeed(
    seedColor: new UiColor(0xff6750a4L),
    brightness: Brightness.light,
    surface: new UiColor(0xfffffbfeL));
var darkPalette = Material.ColorScheme.CreateFromSeed(
    seedColor: new UiColor(0xff6750a4L),
    brightness: Brightness.dark,
    surface: new UiColor(0xff141218L));

return new Material.MaterialApp(
    theme: Material.ThemeData.Create(colorScheme: lightPalette, useMaterial3: true),
    darkTheme: Material.ThemeData.Create(colorScheme: darkPalette, useMaterial3: true),
    themeMode: Material.ThemeMode.system,
    home: new CounterPage());
```

Widget에서는 고정 색 대신 `Material.Theme.of(context).colorScheme`의 `primary`, `onPrimary`, `surfaceContainer`, `onSurface` 같은 role을 사용합니다. 앱에서 강제로 고정하려면 `themeMode`를 `ThemeMode.light` 또는 `ThemeMode.dark`로 바꿉니다. 실제 구성 예시는 [`src/App.cs`](src/App.cs)의 `DemoTheme`에 있습니다.

## 중첩 스크롤과 Android 스크롤바 정책

Material gallery는 세로 `SingleChildScrollView` 안에 별도 세로 `ListView.builder`를 둔 중첩 회귀 surface입니다. 두 scrollable은 `_outerScrollController`와 `_innerScrollController`를 따로 사용하고, `fcr5-outer-scrollbar`와 `fcr5-inner-scrollbar` key로 식별됩니다. Inner list는 170 logical pixel viewport 안에 24개 × 30 logical pixel 항목을 두므로 outer와 viewport/content/offset 수치가 분명히 다릅니다. Inner 알림은 계속 bubble하지만 기본 `notification.depth == 0` predicate 때문에 각 scrollbar는 가장 가까운 scrollable의 메트릭만 반영합니다.

Demo의 Android scrollbar는 `thumbVisibility`를 지정하지 않는 transient 정책입니다. Pinned Flutter와 같이 idle peak의 resolved thumb alpha는 255이고, 스크롤 종료 뒤 600 ms 동안 유지된 다음 300 ms 동안 alpha 0까지 fade합니다. `thumbVisibility: true`는 fade를 끄고 peak를 계속 표시하므로 이 demo에서는 사용하지 않습니다. Peak 자체를 반투명하게 해야 하는 제품은 공용 `ScrollbarThemeData.thumbColor`에 상태별 alpha를 명시해야 하며, framework의 Android 기본색을 바꾸지 않습니다.

## 실행

명령은 저장소 루트에서 실행합니다. target마다 lock 파일이 다릅니다 (`packages.windows.lock.json`, `packages.maccatalyst.lock.json`, `packages.android.lock.json`, `packages.android-x64.lock.json`, `packages.web.lock.json`). SDK 10.0.400과 `maui` / `wasm-tools` workload가 필요합니다.

### Windows

호스트: Windows. RID `win-x64`. GPU surface는 `MauiSKSwapChainPanel`.

```powershell
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Windows
```

### Mac Catalyst

호스트: Apple Silicon macOS. RID `maccatalyst-arm64`. GPU surface는 `MauiSKMetalView`.

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=MacCatalyst --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=MacCatalyst
```

### Android

`dotnet run` 전에 기기 또는 에뮬레이터 하나를 연결합니다 (`adb devices`). 기본 RID는 `android-arm64`입니다.

실기기 arm64:

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64 --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Android -p:RuntimeIdentifier=android-arm64
```

x86_64 에뮬레이터:

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Android -p:RuntimeIdentifier=android-x64 --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Android -p:RuntimeIdentifier=android-x64
```

기기가 여러 대면 `-p:AdbTarget=-s:<serial>`을 추가합니다.

### Web

호스트: WebAssembly workload가 있는 OS. RID `browser-wasm`. `dotnet run`은 Blazor 개발 서버를 띄웁니다.

```bash
dotnet restore ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Web --locked-mode
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj --no-restore -p:DorotiTarget=Web
```

정적 publish:

```bash
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
```

## Build와 validation

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

Developer suite는 공통 descriptor, generated bootstrap, custom shader 계약, synthetic fourth-host 확장점, 네 target graph/build와 FCR-8 `Inventory`/`Contracts`/`Differential`/`Evidence` representative gate를 확인합니다. 실제 Windows/Android/soak acceptance는 위의 명시 shard로 분리됩니다. Android physical에는 .NET Android workload, Android SDK/JDK와 arm64 실기기가 필요합니다. Release suite는 실제 Windows `MauiSKSwapChainPanel` GPU frame과 저장소 밖 Web template/package publish 시나리오를 추가합니다. Repository는 SDK 10.0.400과 Web runtime 10.0.11을 고정하며, runtime patch 변경 시 version stamp가 stale WebCIL publish cache를 무효화합니다.

현재 모든 target의 custom shader는 SkiaSharp 4.151.1과 SkSL을 사용합니다. `FragmentProgram.fromSource(...)`를 쓰거나 `Resources/Shaders/*.sksl` asset을 target별 application manifest에 등록한 뒤 `await FragmentProgram.fromAsset(...)`으로 읽습니다. `FragmentShader`에 float/image sampler uniform을 설정하고 `Paint.shader`, `ShaderMask.shaderCallback`, `ImageFilter.shader`에서 사용할 수 있습니다. `ImageFilter.shader`는 bounded child를 같은 GPU context의 offscreen texture로 캡처하고, 첫 `float2` uniform에 texture 크기, 첫 shader sampler에 filtered child를 implicit input으로 바인딩합니다. Flutter Android stretch도 이 경로의 내장 SkSL을 사용합니다. GPU surface 생성이나 compile/binding 실패는 software renderer로 우회하지 않고 fail-closed 합니다.

Mac Catalyst Windows-host cross-build는 확인했지만 native publish/run에는 Apple Silicon macOS가 필요합니다. 현재 Chromium live에서는 Web runtime 10.0.11로 visible GPU surface, checkbox/radio/switch/slider/button/FAB 포인터 상태 전이, wheel scroll과 keyboard semantics action을 확인했고 console error는 0입니다. Windows live에서는 native wheel/drag 8건, 두 번의 resize, swap-chain replay와 shader frame을 확인했으며 failed/software-fallback frame은 0입니다. Android physical, paired Flutter raster differential, soak/resource plateau, IME/TalkBack/stylus/mouse acceptance는 현재 실행 전이므로 `notVerified`입니다.

현재 evidence는 [FCR-8 stability evidence](../Doroti/validation/evidence/flutter-conformance/fcr8-stability-evidence.json), [app target evidence](../Doroti/validation/evidence/app-targets-evidence.json), [Web product evidence](../Doroti/validation/evidence/web/web-product-evidence.json), [수동 browser evidence](../Doroti/validation/evidence/web/web-browser-live-manual.json)에 기록합니다. 과거 Win32/AppKit 및 G4-G7 evidence는 predecessor-only로 보존합니다.

Source 소유권과 명령 설명은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요.

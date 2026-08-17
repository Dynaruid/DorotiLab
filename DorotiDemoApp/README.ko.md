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
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
pwsh -File ./Doroti/eng/validate-app-targets.ps1 -Shard AndroidLive -AndroidSerial <adb-serial>
```

Developer suite는 공통 descriptor, generated bootstrap, custom shader 계약, synthetic fourth-host 확장점과 네 target graph/build를 확인합니다. Android live gate에는 .NET Android workload, Android SDK/JDK와 arm64 실기기 또는 x86_64 에뮬레이터가 필요합니다. Release suite는 실제 Windows `MauiSKSwapChainPanel` GPU frame과 저장소 밖 Web template/package publish 시나리오를 추가합니다. Repository는 SDK 10.0.400과 Web runtime 10.0.11을 고정하며, runtime patch 변경 시 version stamp가 stale WebCIL publish cache를 무효화합니다.

현재 모든 target의 custom shader는 SkSL을 사용합니다. `FragmentProgram.fromSource(...)`를 쓰거나 `Resources/Shaders/*.sksl` asset을 target별 application manifest에 등록한 뒤 `await FragmentProgram.fromAsset(...)`으로 읽습니다. `FragmentShader`에 float/image sampler uniform을 설정하고 `Paint.shader` 또는 `ShaderMask.shaderCallback`에서 사용할 수 있습니다. 현재 SkiaSharp host는 필터 대상 child를 implicit texture input으로 바인딩할 수 없으므로 `ImageFilter.shader`는 지원한다고 광고하지 않으며, Flutter Android stretch는 Flutter의 matrix fallback을 사용합니다. Compile/binding 실패는 software renderer로 우회하지 않고 fail-closed 합니다.

Mac Catalyst Windows-host cross-build는 확인했지만 native publish/run에는 Apple Silicon macOS가 필요합니다. Android arm64 실기기에서 custom SkSL GPU 렌더링을 자동 확인했고 x86_64 에뮬레이터에서 반복 스크롤 뒤 렌더링과 visible-content screenshot도 확인했습니다. 실기기의 수동 지속 표시, IME, TalkBack, stylus, mouse acceptance는 별도입니다. 기존 Chromium smoke의 WebGL2/basic pointer 상태 전이는 유지되지만 새 custom shader의 browser-live 확인은 `notVerified`입니다.

현재 evidence는 [app target evidence](../Doroti/migration/maui/app-targets-evidence.json), [Web product evidence](../Doroti/migration/web/web-product-evidence.json), [수동 browser evidence](../Doroti/migration/web/web-browser-live-manual.json)에 기록합니다. 과거 Win32/AppKit 및 G4-G7 evidence는 predecessor-only로 보존합니다.

Source 소유권과 명령 설명은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요.

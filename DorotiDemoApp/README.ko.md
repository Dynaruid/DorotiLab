# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 제품이 직접 소유하는 Doroti framework와 single-project application 계약을 사용하는 dogfood 앱입니다. 하나의 `DorotiDemoApp.csproj`, root `Program.cs`, 공용 `src/App.cs`가 MAUI Windows, MAUI Mac Catalyst 또는 Blazor WebAssembly를 target으로 선택합니다.

이 application은 C# 전용입니다. Build와 validation flow는 `DorotiDemoApp/dart` package를 생성하거나 소비하지 않습니다.

## 구성

- `Program.cs`: 모든 target이 사용하는 public target-neutral `IDorotiApplicationStartup`
- `src/App.cs`: `Program`이 선택하는 target-neutral Material widget/state tree와 view configuration
- `Platforms/Windows`: 얇은 WinUI shell, 선택적 platform hook과 manifest
- `Platforms/MacCatalyst`: 얇은 UIKit delegate, 선택적 platform hook, plist와 entitlement
- `Platforms/Web/src`: 사용자 소유 TypeScript bootstrap 정책과 browser plugin
- `Platforms/Web/wwwroot`: 직접 작성한 HTML, CSS 인접 resource, locale/asset, manifest. 생성 JavaScript는 두지 않음
- `obj/web/<configuration>/net10.0/Doroti.Generated/wwwroot`: compile된 앱 bootstrap/plugin JavaScript
- `obj/<target>/Doroti.Generated`: SDK 소유 C# bootstrap과 plugin registration. 직접 수정하지 않음

`doroti_bootstrap.ts`는 Doroti loader를 통해 loading/error UI와 typed Blazor startup hook을 구성합니다. `Blazor.start()`는 `doroti.loader.ts`만 호출합니다. Release publish에는 `doroti_bootstrap.js`, `plugins/echo.js`, `_content/Doroti.Host.Web/doroti.loader.js`, `_content/Doroti.Host.Web/doroti.web.js`가 있으며 TypeScript source, `tsconfig.json`, compiler asset은 포함하지 않습니다.

## Build와 validation

```powershell
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64

pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

Developer suite는 공통 descriptor, generated bootstrap, synthetic fourth-host 확장점과 세 target graph/build를 확인합니다. Release suite는 실제 Windows `MauiSKSwapChainPanel` GPU frame과 저장소 밖 Web template/package publish 시나리오를 추가합니다. Repository는 SDK 10.0.400과 Web runtime 10.0.11을 고정하며, runtime patch 변경 시 version stamp가 stale WebCIL publish cache를 무효화합니다.

Mac Catalyst Windows-host cross-build는 확인했지만 native publish/run에는 Apple Silicon macOS가 필요합니다. 수동 Chromium smoke에서 publish된 WebGL2 canvas, page console error 0건, 렌더링된 FAB click에 따른 `fab=0`에서 `fab=1` 상태 전이를 확인했습니다. Keyboard/IME/clipboard/resize/interactive ARIA, native hover/wheel/capture/keyboard/IME/UIA, physical acceptance와 cross-target parity는 각각 별도의 `notVerified` gate입니다.

현재 evidence는 [app target evidence](../Doroti/migration/maui/app-targets-evidence.json), [Web product evidence](../Doroti/migration/web/web-product-evidence.json), [수동 browser evidence](../Doroti/migration/web/web-browser-live-manual.json)에 기록합니다. 과거 Win32/AppKit 및 G4-G7 evidence는 predecessor-only로 보존합니다.

Source 소유권과 명령 설명은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요.

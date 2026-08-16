# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 제품이 직접 소유하는 Doroti framework와 single-project application 계약을 사용하는 dogfood 앱입니다. 하나의 `DorotiDemoApp.csproj`, root `Program.cs`, 공용 `src/App.cs`가 MAUI Windows, MAUI Mac Catalyst 또는 Blazor WebAssembly를 target으로 선택합니다.

이 application은 C# 전용입니다. Build와 validation flow는 `DorotiDemoApp/dart` package를 생성하거나 소비하지 않습니다.

## 구성

- `src/App.cs`: `Doroti.Framework.*`를 사용하는 target-neutral Material widget/state tree
- `Program.cs`: Web과 Mac Catalyst용 얇은 bootstrap. Windows는 SDK가 생성한 WinUI entrypoint 사용
- `Platforms/Maui`: 공용 C# `Application`, `Window`, `ContentPage`, `SKGLView`
- `Platforms/Windows`: 유일한 bootstrap `App.xaml`, code-behind와 manifest
- `Platforms/MacCatalyst`: UIKit bootstrap, delegate, plist와 entitlements
- `Platforms/Web`: Blazor composition root와 static asset

## Build와 validation

```powershell
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64

pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

Developer suite는 세 target graph와 build를 확인합니다. Release suite는 실제 Windows `MauiSKSwapChainPanel` GPU frame과 저장소 밖 Web template/package publish 시나리오를 추가합니다.

Mac Catalyst Windows-host cross-build는 확인했지만 native publish/run에는 Apple Silicon macOS가 필요합니다. Native hover/wheel/capture/keyboard/IME/UIA, browser live 자동화, physical acceptance와 cross-target parity는 각각 별도의 `notVerified` gate입니다.

해당 release suite를 완료하면 현재 evidence는 [app target evidence](../Doroti/migration/maui/app-targets-evidence.json)와 [Web product evidence](../Doroti/migration/web/web-product-evidence.json)에 기록됩니다. 과거 Win32/AppKit 및 G4-G7 evidence는 predecessor-only로 보존합니다.

Source 소유권과 명령 설명은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요.

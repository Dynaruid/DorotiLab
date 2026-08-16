# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 Doroti single-project application 계약을 직접 사용하는 dogfood 앱입니다. 하나의 `DorotiDemoApp.csproj`, root `Program.cs`, 공용 `src/App.cs`로 두 번째 host project 없이 MAUI Windows, MAUI Mac Catalyst, Blazor WebAssembly를 선택합니다.

## 구성

- `src/App.cs`: target-neutral 검토 Material widget/state tree
- `Program.cs`: Web과 Mac Catalyst용 얇은 bootstrap. Windows는 WinUI generated `Main` 사용
- `Platforms/Maui`: 공용 C# `Application`, `Window`, `ContentPage`, `SKGLView` 구성
- `Platforms/Windows`: 유일한 bootstrap `App.xaml`, WinUI code-behind와 package manifest
- `Platforms/MacCatalyst`: UIKit entrypoint, delegate, plist와 entitlements
- `Platforms/Web`: Blazor composition root와 static asset

Application project는 정확히 하나입니다. Windows는 `ApplicationDefinition` 하나와 `MauiXaml` 0개를 컴파일하고 Mac Catalyst/Web은 XAML을 컴파일하지 않습니다.

## 빌드와 실행

Repository root에서 실행합니다.

```powershell
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=MacCatalyst -p:RuntimeIdentifier=maccatalyst-arm64
```

Mac Catalyst compile graph는 Windows-host cross-build를 통과했습니다. Publish와 native 실행은 적절한 Apple Silicon macOS runner가 필요하며 계속 `notVerified`입니다.

## 검증 상태

현재 single-project gate는 다음과 같이 실행합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g7-maui-single-project.ps1 -Shard All
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Graph
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Template
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Compile
pwsh -File ./Doroti/eng/validate-g7-web-build.ps1 -Shard Publish
```

현재 evidence는 Windows Release build/publish, `MauiSKSwapChainPanel`과 `win-x64/winui3/SKSwapChainPanel/ANGLE-DirectX-Skia`를 통한 실제 GPU frame, Web package-only compile/publish를 증명합니다. 기본 MAUI touch 변환과 clipboard/focus adapter는 구현했지만 native hover/wheel/capture/keyboard/IME/UIA, resize/DPI/context recreate, Mac Catalyst native 실행, physical acceptance와 cross-target parity는 `notVerified`입니다.

2026-08-15 수동 Chromium smoke는 공식 Web publish artifact의 non-empty GPU canvas, DPR sizing, bounded backdrop blur, 2-pass shadow, semantics, 기본 pointer 상태 변화와 console error 0에만 적용됩니다. 남은 Web 자동화나 physical gate를 증명하지 않습니다.

## Evidence

- [MAUI single-project evidence](../Doroti/migration/maui/g7-maui-single-project-evidence.json)
- [Web build evidence](../Doroti/migration/web/g7-web-build-evidence.json)
- 기존 Win32/AppKit evidence는 predecessor-only로 유지하며 MAUI PASS로 승격하지 않습니다.

Runtime architecture와 개발 명령은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요.

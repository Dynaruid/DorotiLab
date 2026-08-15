# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 Doroti C# framework 제품의 cross-target 검증 앱입니다. 동일한 `Program.cs` Material widget tree를 Win32/WGL, AppKit/NSOpenGL `osx-arm64`, SkiaSharp/Blazor `browser-wasm` host로 컴파일합니다.

직접 실행할 수 있는 gallery인 동시에 Windows와 G7-3M macOS native gate가 공유하는 generated 제품 scenario입니다.

## Demo 구성

- `MaterialApp.builder`와 `MaterialApp.home`/Navigator 시작 경로
- `Theme`, `Scaffold`, `AppBar`, `Card`, `ListTile`
- Elevated button, checkbox, radio, switch, slider와 floating action button
- `Row`, `Column`, `Stack`, `SingleChildScrollView`, `ListView.builder`
- 측정 가능한 raster 변화를 만드는 local `State` update
- Semantics node와 target-native accessibility action
- Native window와 GPU resource lifecycle 확인

이 앱은 `Doroti/src` 아래로 승격된 제품 project를 참조합니다. Migration candidate나 과거 native-list/runtime-v2 demo를 실행하지 않습니다.

## 요구 사항

- Windows x64, Apple Silicon macOS(`osx-arm64`) 또는 static Web build를 위한 `browser-wasm` workload
- .NET SDK 10.0.300 또는 호환되는 최신 patch
- `Doroti` project를 포함한 전체 DorotiLab checkout
- 전체 validation gate를 위한 PowerShell 7

Project는 host OS 또는 명시한 RID에 따라 정확히 하나의 target composition root를 선택합니다. Linux, Intel macOS, mobile과 physical-device acceptance는 별도 target evidence가 없는 한 `notVerified`입니다.

## Web 앱 빌드

`DorotiDemoApp.Web.csproj`는 desktop target dependency 없이 내부 Blazor host와 동일한 `Program.cs` widget/state source를 컴파일합니다.

```powershell
dotnet build ./DorotiDemoApp/DorotiDemoApp.Web.csproj -c Release
dotnet publish ./DorotiDemoApp/DorotiDemoApp.Web.csproj -c Release -r browser-wasm -o ./publish/doroti-demo-web
```

배포 root는 `publish/doroti-demo-web/wwwroot`입니다. 표준 Blazor loader, fingerprint된 Doroti/SkiaSharp WASM assembly, statically linked native runtime, asset, localization과 예제 Web plugin이 포함됩니다. Chromium GPU/input/ARIA runtime acceptance는 G7-4 소유이며 G7-3은 build/static artifact graph까지만 증명합니다.

2026-08-15 수동 Chromium smoke에서 공식 publish artifact의 non-empty GPU canvas, logical/physical DPR 분리, bounded `sigmaX=12`/`sigmaY=6` backdrop blur, desktop과 같은 2-pass shadow, semantics tree와 pointer 상태 변화를 확인했으며 해당 origin의 console error는 0이었습니다. 이는 `presented`와 기본 pointer 확인이며 G7-4의 wheel/keyboard/IME/clipboard/resize/ARIA/reference 자동화 또는 physical acceptance를 대신하지 않습니다.

## 앱 실행

Repository root에서 실행합니다.

```powershell
dotnet run --project ./DorotiDemoApp
```

기본 진입 경로는 `MaterialApp.builder`입니다. 짧은 자동 smoke test는 다음과 같이 실행합니다.

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry builder --frames 3 --duration-ms 15000
```

Navigator를 사용하는 `MaterialApp.home` 경로는 다음 명령으로 확인할 수 있습니다.

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry home --frames 3 --duration-ms 15000
```

`--smoke`는 visible window에 target-native pointer tap을 넣어 framework hit-test/gesture/state 경로를 확인합니다. 이어 모든 action/selection control, state와 pixel 변화, semantics tree, strict GPU backend를 확인한 뒤 native resource가 균형 있게 해제되었는지 검사합니다.

## Target 검증

Repository root에서 전체 G6-3 gate를 실행합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

Windows gate는 다음 항목을 검증합니다.

- `builder`, `home` 두 entry path
- 외부 Windows UI Automation action 6개
- 30초 동안 요청한 300 frame
- Screenshot color, text ink, layout bounds와 interaction delta
- Compiler와 widget regression
- Release 제품 build
- Local package만 restore하는 clean external consumer

더 작은 범위만 실행하려면 `-Shard`에 `LiveWindows`, `ExternalConsumer`, `Compiler`, `Regression`, `Evidence` 중 하나를 지정할 수 있습니다.

Apple Silicon macOS에서는 G7-3M의 네 shard를 실행합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Source
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Build
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Live
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Package
```

macOS gate는 실제 NSWindow, AppKit lifecycle/focus, pointer와 fractional wheel, key/text input, clipboard 원상복구, NSAccessibility action, Apple M1 strict-GPU present, repeat publish identity와 저장소 밖 package-only consumer를 검증합니다. 물리 Korean IME candidate placement, VoiceOver 탐색과 precise trackpad gesture는 명시적으로 `notVerified`입니다.

## Evidence와 artifact

- Commit되는 통합 evidence: [`../Doroti/migration/flutter-framework/g6-material-demo-evidence.json`](../Doroti/migration/flutter-framework/g6-material-demo-evidence.json)
- G7-3M macOS 통합 evidence: [`../Doroti/migration/macos/g7-macos-shell-evidence.json`](../Doroti/migration/macos/g7-macos-shell-evidence.json)
- G7-3 browser build evidence(`doroti.g7-web-build-evidence/v2`): [`../Doroti/migration/web/g7-web-build-evidence.json`](../Doroti/migration/web/g7-web-build-evidence.json)
- Screenshot/layout reference: [`g6-material-reference.json`](g6-material-reference.json)
- 임시 실행 output: `../Doroti/artifacts/g6-material-demo/win-x64/`

각 evidence는 해당 target에만 적용됩니다. Windows 결과를 macOS로 전이하지 않으며 macOS 자동화는 물리 IME/VoiceOver/trackpad acceptance를 주장하지 않습니다. 두 target 모두 미지원 운영체제의 성공 근거가 아닙니다.

## Project 파일

| 파일 | 역할 |
| --- | --- |
| [`Program.cs`](Program.cs) | 공용 Material gallery/state source와 desktop 전용 host loop/evidence writer |
| [`DorotiDemoApp.csproj`](DorotiDemoApp.csproj) | 제품 framework, hosting과 OS/RID 조건부 desktop target reference |
| [`DorotiDemoApp.Web.csproj`](DorotiDemoApp.Web.csproj) | 동일한 `Program.cs`를 `browser-wasm`으로 컴파일하는 Blazor WebAssembly host |
| [`WebHost/`](WebHost/) | 내부 Web composition root와 static deployment asset |
| [`g6-material-reference.json`](g6-material-reference.json) | 예상 logical geometry, color와 pixel tolerance |

Runtime architecture와 개발 명령은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요. Doroti는 repository의 [BSD 3-Clause license](../LICENSE)로 배포됩니다.

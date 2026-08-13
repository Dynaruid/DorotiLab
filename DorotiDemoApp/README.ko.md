# DorotiDemoApp

[English](README.md) | **한국어**

DorotiDemoApp은 C#으로 옮긴 Flutter framework를 실제 native Windows 환경에서 검증하는 앱입니다. 실제 Material widget tree를 구성하고 Doroti의 Win32 host와 strict Skia WGL/OpenGL backend로 표시하며, framework state와 accessibility 동작을 end-to-end로 실행합니다.

직접 실행할 수 있는 gallery인 동시에 현재 Goal 6 milestone의 상시 blocking 제품 gate입니다.

## Demo 구성

- `MaterialApp.builder`와 `MaterialApp.home`/Navigator 시작 경로
- `Theme`, `Scaffold`, `AppBar`, `Card`, `ListTile`
- Elevated button, checkbox, radio, switch, slider와 floating action button
- `Row`, `Column`, `Stack`, `SingleChildScrollView`, `ListView.builder`
- 측정 가능한 raster 변화를 만드는 local `State` update
- Semantics node와 native UI Automation action
- Native window와 WGL resource lifecycle 확인

이 앱은 `Doroti/src` 아래로 승격된 제품 project를 참조합니다. Migration candidate나 과거 native-list/runtime-v2 demo를 실행하지 않습니다.

## 요구 사항

- Windows x64
- .NET SDK 10.0.300 또는 호환되는 최신 patch
- `Doroti` project를 포함한 전체 DorotiLab checkout
- 전체 validation gate를 위한 PowerShell 7

현재 target package는 non-Windows 실행을 거부합니다. 다른 운영체제와 physical device는 아직 `notVerified`입니다.

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

`--smoke`는 모든 action/selection control을 실행하고 state와 pixel 변화, semantics tree, GPU backend를 확인한 뒤 native resource가 균형 있게 해제되었는지 검사합니다.

## 전체 검증

Repository root에서 전체 G6-3 gate를 실행합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

이 gate는 다음 항목을 검증합니다.

- `builder`, `home` 두 entry path
- 외부 Windows UI Automation action 6개
- 30초 동안 요청한 300 frame
- Screenshot color, text ink, layout bounds와 interaction delta
- Compiler와 widget regression
- Release 제품 build
- Local package만 restore하는 clean external consumer

더 작은 범위만 실행하려면 `-Shard`에 `LiveWindows`, `ExternalConsumer`, `Compiler`, `Regression`, `Evidence` 중 하나를 지정할 수 있습니다.

## Evidence와 artifact

- Commit되는 통합 evidence: [`../Doroti/migration/flutter-framework/g6-material-demo-evidence.json`](../Doroti/migration/flutter-framework/g6-material-demo-evidence.json)
- Screenshot/layout reference: [`g6-material-reference.json`](g6-material-reference.json)
- 임시 실행 output: `../Doroti/artifacts/g6-material-demo/win-x64/`

이 evidence의 범위는 자동화된 Windows x64 native 동작입니다. Physical device나 non-Windows target의 성공으로 확대 해석해서는 안 됩니다.

## Project 파일

| 파일 | 역할 |
| --- | --- |
| [`Program.cs`](Program.cs) | Material gallery, native host loop, smoke interaction과 evidence writer |
| [`DorotiDemoApp.csproj`](DorotiDemoApp.csproj) | 제품 framework, hosting과 Windows target reference |
| [`g6-material-reference.json`](g6-material-reference.json) | 예상 logical geometry, color와 pixel tolerance |

Runtime architecture와 개발 명령은 [Doroti runtime README](../Doroti/README.ko.md)를 참고하세요. Doroti는 repository의 [BSD 3-Clause license](../LICENSE)로 배포됩니다.

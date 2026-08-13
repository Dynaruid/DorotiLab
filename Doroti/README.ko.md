# Doroti runtime과 framework

[English](README.md) | **한국어**

이 디렉터리에는 Doroti의 제품 runtime이 들어 있습니다. 고정된 Flutter source에서 생성하고 검토한 C# framework package, widget/rendering lifecycle, Skia renderer와 native platform host를 함께 관리합니다.

Doroti는 Flutter를 WebView에 넣어 실행하지 않으며 Avalonia control이나 XAML로 UI를 구성하지도 않습니다. Flutter framework를 동작의 기준으로 삼고, Doroti runtime이 그 의미를 .NET에서 실행하며, platform host가 native window·input·accessibility·graphics를 연결합니다.

## 현재 상태

현재 제품 gate는 **Windows x64** native 자동 검증입니다. 검토된 Material widget tree가 실제 HWND에서 construct, mount, layout, paint, present되고 입력에 반응하며, rendering은 strict `skia-wgl-opengl-gpu` backend를 사용합니다.

검증된 범위에는 `MaterialApp`, `Theme`, `Navigator`, `Scaffold`, `AppBar`, `Card`, `ListTile`, button과 selection control, 주요 layout widget, scrolling, lazy list, local state update와 native accessibility action이 포함됩니다. `MaterialApp.builder`와 `MaterialApp.home` 두 진입 경로를 모두 검증합니다.

Linux, macOS, Web, Android, iOS와 physical device 검증은 아직 roadmap에 있습니다. 특정 target에서 project나 package가 compile된 사실만으로 native 표시와 상호작용까지 지원한다고 판단하지 않습니다.

현재 milestone과 증거 기준은 repository의 [Goal 6 roadmap](../goal6.md)에서 확인할 수 있습니다.

## Architecture

```text
pinned Flutter source
        │
        ▼
Doroti.DartToCSharp ──► reviewed C# framework packages
                              │
                              ▼
                 Flutter runtime and hosting
                              │
                              ▼
                  widget / render pipeline
                              │
                              ▼
              target package and native host
        (Windows: Avalonia-derived platform source)
```

- `Doroti.Flutter.Framework.*`에는 생성·검토된 framework library가 들어 있습니다.
- `Doroti.Flutter.Runtime`, `Doroti.Flutter.Ui`, `Doroti.Flutter.Hosting`은 Dart/Flutter runtime 의미와 app bootstrap을 제공합니다.
- `Doroti.Engine`, `Doroti.Rendering`, `Doroti.Graphics`는 frame scheduling, display output과 graphics contract를 담당합니다.
- `Doroti.Host.Desktop`과 `Doroti.Target.Windows.win-x64`가 제품을 Win32와 strict GPU presentation에 연결합니다.
- `Doroti.Host.Avalonia`는 비교용 host이며 기본 제품 composition root가 아닙니다.

선별한 Avalonia platform source는 Doroti contract 뒤에 이식하고 provenance manifest로 추적합니다. Doroti app은 Avalonia control에 runtime dependency를 갖지 않습니다.

## 준비 사항

- [`global.json`](global.json)에 고정된 .NET SDK **10.0.300** 또는 호환되는 최신 patch
- repository workflow 실행을 위한 PowerShell 7 (`pwsh`)
- compiler와 source audit workflow를 위한 Dart SDK
- repository root의 `flutter-master`, `Avalonia-main` reference checkout
- native demo와 live UI Automation gate 실행을 위한 Windows x64

두 reference checkout은 source와 동작의 입력이며 제품 runtime dependency가 아닙니다.

## 빠른 시작

Repository root에서 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build
dotnet run --project ./DorotiDemoApp
```

Windows x64에서 짧은 deterministic smoke run을 실행하려면 다음 명령을 사용합니다.

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry builder --frames 3 --duration-ms 15000
```

## Repository 명령

`eng/doroti.ps1`가 기본 개발 entry point입니다.

| 명령 | 역할 |
| --- | --- |
| `doctor` | SDK, desktop backend, pinned source, revision과 license 확인 |
| `build` | `Doroti.Product.slnx`의 간결한 제품 graph build |
| `validate` | compiler validation suite 실행 |
| `audit` | local storage, source/provenance와 compiler audit 실행 |
| `format` | 파일을 수정하지 않고 제품 formatting 확인 |
| `release` | build, audit, package 생성과 package-only 외부 consumer 검증 |
| `clean` | Doroti build output, artifact와 임시 local state 정리 |

예시:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 audit
pwsh -File ./Doroti/eng/doroti.ps1 format
```

전체 G6-3 Material gate는 Windows 전용입니다. 두 app entry path, 외부 UI Automation, 30초/300 frame cadence, screenshot geometry, compiler regression과 clean package consumer를 함께 검증합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

생성된 evidence는 `migration/flutter-framework/`에, 임시 screenshot과 실행 artifact는 `artifacts/`에 기록됩니다.

## 디렉터리 안내

| 경로 | 내용 |
| --- | --- |
| [`src/`](src/) | 제품 package, runtime, renderer, host, target package와 analyzer |
| [`validation/`](validation/) | 현재 compiler, managed behavior와 native validation executable |
| [`migration/`](migration/) | source selection, promotion input, provenance, baseline과 commit되는 evidence |
| [`eng/`](eng/) | build, audit, promotion, validation과 local-storage workflow |
| [`tools/`](tools/) | source, behavior, scene, provenance와 porting 도구 |
| [`samples/`](samples/) | host 비교와 진단 sample |
| [`templates/`](templates/) | `dotnet new` template과 package metadata |
| [`docs/`](docs/) | architecture decision과 milestone 설계 문서 |

`Doroti.Product.slnx`는 제품만 포함하는 간결한 build입니다. `Doroti.slnx`에는 repository 작업에 필요한 tool, validation, demo, sample과 historical project도 포함됩니다.

## 생성 source 원칙

생성 output은 검토 가능한 evidence이며 숨겨진 수정 장소가 아닙니다. 공통 의미 문제는 analyzer, compiler 또는 runtime에서 고친 뒤 다시 생성합니다.

- Selection manifest는 `migration/selections/`에 둡니다.
- Manual replacement와 platform port는 `migration/ports/`에 둡니다.
- Compiler가 소유하는 `generated-base/`, `manual-snapshot/`, `effective/` tree는 직접 수정하지 않습니다.
- 승격한 source에는 source map, provenance, source revision과 license 이력이 남아야 합니다.
- Repository의 모든 JSON에는 `System.Text.Json`을 사용하며 `Newtonsoft.Json`을 추가하지 않습니다.

더 자세한 설계는 [typed framework compiler](docs/architecture/f0-typed-framework-compiler.md), [multi-library compilation](docs/architecture/g3-1-multi-library-framework-compiler.md), [Windows RID packaging](docs/architecture/g5-6w-windows-rid-package.md), [port ownership](docs/architecture/p0-port-ownership.md) 문서부터 참고하세요.

## 관련 프로젝트와 License

Doroti는 [Flutter](https://github.com/flutter/flutter)를 framework 동작의 기준으로 사용하고, 선별한 [Avalonia](https://github.com/AvaloniaUI/Avalonia) platform 구현을 Windows host의 기반으로 활용합니다.

Doroti는 repository의 [BSD 3-Clause license](../LICENSE)로 배포됩니다. Upstream source, package, revision과 license 상세 내용은 [third-party notices](THIRD-PARTY-NOTICES.md)를 확인하세요.

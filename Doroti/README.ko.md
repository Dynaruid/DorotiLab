# Doroti runtime과 framework

[English](README.md) | **한국어**

이 디렉터리에는 Doroti의 제품 runtime이 들어 있습니다. 고정된 Flutter source에서 생성하고 검토한 C# framework package, widget/rendering lifecycle, Skia renderer와 native platform host를 함께 관리합니다.

Doroti는 Flutter를 WebView에 넣어 실행하지 않으며 Avalonia control이나 XAML로 UI를 구성하지도 않습니다. Flutter framework를 동작의 기준으로 삼고, Doroti runtime이 그 의미를 .NET에서 실행하며, platform host가 native window·input·accessibility·graphics를 연결합니다.

## 현재 상태

현재 migration은 하나의 application project로 **MAUI Windows x64**, **MAUI Mac Catalyst arm64**, **Blazor WebAssembly**를 선택합니다. Windows MAUI 제품 host는 Release build/publish와 실제 GPU 실행을 통과했으며 `MauiSKSwapChainPanel`, `win-x64/winui3/SKSwapChainPanel/ANGLE-DirectX-Skia`, submitted/presented 3, failure 0, software fallback 0을 기록했습니다.

공용 `src/App.cs`의 검토된 Material tree를 선택된 모든 target이 함께 컴파일합니다. Windows native hover/wheel/keyboard/IME/UIA, resize/DPI/context recreate, Mac Catalyst native build/publish/live, physical acceptance와 cross-target parity는 별도 `notVerified` gate로 남아 있습니다.

기존 Win32/WGL과 AppKit/NSOpenGL 결과는 predecessor evidence로만 유지하며 MAUI acceptance로 계산하지 않습니다. 특정 target에서 project나 package가 compile된 사실만으로 native 동작까지 증명되었다고 판단하지 않습니다.

Goal7의 완료 milestone, 남은 Web/release gate와 증거 경계는 역사 기록인 [Goal7 요약](../history/26-08-16/goal7-summary.md)에서 확인할 수 있습니다. 이 문서는 후속 active roadmap을 지정하지 않습니다.

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
 (Windows: WinUI 3/SKSwapChainPanel · Mac Catalyst: UIKit/SKMetalView · Web: WebGL2)
```

- `Doroti.Framework.*`에는 생성·검토된 framework library가 들어 있습니다.
- `Doroti.Runtime`, `Doroti.Ui`, `Doroti.Hosting`은 Dart/Flutter runtime 의미와 app bootstrap을 제공합니다.
- `Doroti.Engine`, `Doroti.Rendering`, `Doroti.Graphics`는 frame scheduling, display output과 graphics contract를 담당합니다.
- `Doroti.App.Sdk`가 target, TFM, RID, platform source set, lock/output 경로와 underlying .NET SDK를 선택합니다.
- `Doroti.Host.Maui`가 `SKGLView` 소유 `SKSurface`를 소비하고 CPU fallback 없이 session/view/frame lifecycle을 연결합니다.
- `Doroti.Target.Windows.Maui.win-x64`, `Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64`, `Doroti.Target.Web.browser-wasm`이 target composition root를 제공합니다.

선별한 Avalonia platform source는 Doroti contract 뒤에 이식하고 provenance manifest로 추적합니다. Doroti app은 Avalonia control에 runtime dependency를 갖지 않습니다.

## 준비 사항

- [`global.json`](global.json)에 고정된 .NET SDK **10.0.300** 또는 호환되는 최신 patch
- repository workflow 실행을 위한 PowerShell 7 (`pwsh`)
- repository root의 pinned 전용 `reference/flutter-master` SDK와 `reference/Avalonia-main` reference checkout
- Win32/UI Automation gate를 위한 Windows x64 또는 G7-3M AppKit gate를 위한 Apple Silicon macOS

로컬 Flutter SDK는 최초 한 번 `pwsh -File ./Doroti/eng/prepare-flutter-sdk.ps1`로 준비합니다. Doroti workflow는 host에 맞는 repository-local launcher(`flutter.bat`/`dart.bat` 또는 `flutter`/`dart`)를 직접 사용하며 `PATH` 또는 사용자 profile의 Flutter 설치로 fallback하지 않습니다. Windows에서 만든 checkout을 macOS로 옮긴 경우 pinned Flutter source와 shell script의 CRLF는 SDK resolver가 LF로 정규화합니다. 각 native gate는 자기 target만 증명합니다. AppKit 실행은 Win32 UI Automation evidence를 대신하지 않고 Windows evidence도 macOS로 전이하지 않습니다. 두 reference checkout은 source와 동작의 입력이며 제품 runtime dependency가 아닙니다.

## 빠른 시작

Repository root에서 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
```

Single-project graph/build sequence, Windows live GPU와 evidence gate는 다음 명령으로 실행합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g7-maui-single-project.ps1 -Shard All
```

## Repository 명령

`eng/doroti.ps1`가 기본 개발 entry point입니다.

| 명령 | 역할 |
| --- | --- |
| `doctor` | SDK, desktop backend, pinned source, revision과 license 확인 |
| `build` | `Doroti.Product.slnx`의 간결한 제품 graph build |
| `audit` | local storage, source/provenance와 compiler audit 실행 |
| `format` | 파일을 수정하지 않고 제품 formatting 확인 |
| `release` | 제품 package를 build/audit/검사하고 G7-3C app template acceptance 구현 전에는 fail-closed |
| `clean` | Doroti build output, artifact와 임시 local state 정리 |

예시:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 audit
pwsh -File ./Doroti/eng/doroti.ps1 format
```

전체 G6-3 Material gate는 Windows 전용입니다. 두 app entry path, 외부 UI Automation, 30초/300 frame cadence, screenshot geometry, compiler regression과 clean package consumer를 함께 검증합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

Apple Silicon macOS에서는 G7-3M이 source, reproducible build, 실제 AppKit live 동작과 저장소 밖 package 소비를 분리해 검증합니다.

```powershell
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Source
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Build
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Live
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Package
```

생성된 evidence는 `migration/flutter-framework/`에, 임시 screenshot과 실행 artifact는 `artifacts/`에 기록됩니다.

## 디렉터리 안내

| 경로 | 내용 |
| --- | --- |
| [`src/`](src/) | 제품 package, runtime, renderer, host, target package와 analyzer |
| [`migration/`](migration/) | source selection, promotion input, provenance, baseline과 commit되는 evidence |
| [`eng/`](eng/) | build, audit, promotion, validation과 local-storage workflow |
| [`tools/`](tools/) | source, behavior, scene, provenance와 porting 도구 |
| [`samples/`](samples/) | host 비교와 진단 sample |
| [`templates/`](templates/) | `dotnet new` template과 package metadata |
| [`docs/`](docs/) | architecture decision과 milestone 설계 문서 |

`Doroti.Product.slnx`는 제품만 포함하는 간결한 build입니다. `Doroti.slnx`에는 repository 작업에 필요한 tool, demo, sample과 historical project도 포함됩니다.

## 생성 source 원칙

생성 output은 검토 가능한 evidence이며 숨겨진 수정 장소가 아닙니다. 공통 의미 문제는 analyzer, compiler 또는 runtime에서 고친 뒤 다시 생성합니다.

- Selection manifest는 `migration/selections/`에 둡니다.
- Manual replacement와 platform port는 `migration/ports/`에 둡니다.
- Compiler가 소유하는 `generated-base/`, `manual-snapshot/`, `effective/` tree는 직접 수정하지 않습니다.
- 승격한 source에는 source map, provenance, source revision과 license 이력이 남아야 합니다.
- Repository의 모든 JSON에는 `System.Text.Json`을 사용하며 `Newtonsoft.Json`을 추가하지 않습니다.

더 자세한 설계는 [typed framework compiler](docs/architecture/f0-typed-framework-compiler.md), [multi-library compilation](docs/architecture/g3-1-multi-library-framework-compiler.md), [Windows RID packaging](docs/architecture/g5-6w-windows-rid-package.md), [macOS RID packaging](docs/architecture/g7-3m-macos-rid-package.md), [port ownership](docs/architecture/p0-port-ownership.md) 문서부터 참고하세요.

## 관련 프로젝트와 License

Doroti는 [Flutter](https://github.com/flutter/flutter)를 framework 동작의 기준으로 사용하고, 선별한 [Avalonia](https://github.com/AvaloniaUI/Avalonia) platform 구현을 Windows와 macOS host의 source 기반으로 활용합니다.

Doroti는 repository의 [BSD 3-Clause license](../LICENSE)로 배포됩니다. Upstream source, package, revision과 license 상세 내용은 [third-party notices](THIRD-PARTY-NOTICES.md)를 확인하세요.

# Doroti

[English](README.md) | **한국어**

### C#과 .NET으로 만드는 cross-platform UI framework

Doroti는 공용 C# widget, layout, painting, semantics, rendering pipeline을 Windows, Mac Catalyst와 Web에 제공합니다. 익숙한 Material/Cupertino API의 동작 reference는 Flutter이지만, 유지보수하는 제품 구현은 `Doroti.Framework.*`에 있습니다.

Doroti는 Flutter를 WebView에 넣지 않으며 MAUI 또는 Avalonia control로 UI를 구성하지 않습니다. Platform host는 native window/view, GPU surface, input, text, clipboard, accessibility capability를 제공하고 Doroti가 widget/render tree를 소유합니다.

## 현재 개발 방식

프로젝트 초반에는 semantic compiler로 Flutter source의 큰 범위를 일괄 변환해 기반을 만들었습니다. 이 bootstrap은 현재 framework를 만드는 데 유효했지만 이제 일반 기능 개발 방식은 아닙니다.

현재 `Doroti/src/Doroti.Framework.*`는 제품이 직접 소유하는 C# source이며 namespace, assembly, package는 `Doroti.Framework.*`로 일치합니다. 기능과 수정은 소유 framework/runtime/renderer/host 계약에서 직접 개발합니다. Dart-to-C# compiler와 고정 Flutter checkout은 선택적인 import·reference differential 도구로 남고 제품 source를 덮어쓰지 않습니다. `DorotiDemoApp`과 생성된 `doroti-app` project는 C# 전용이며 활성 validation은 그 내부에 Dart package를 만들지 않습니다.

자세한 결정은 [ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md), 현재 우선순위는 [작업 목록](work.md)을 참고하세요.

## 현재 동작 범위

- 공용 Material/Cupertino widget, element, layout, paint, semantics, state 기반
- MAUI Windows x64, MAUI Mac Catalyst arm64 또는 Blazor WebAssembly를 선택하는 하나의 C# application project
- Windows WinUI 3 `MauiSKSwapChainPanel`과 Web WebGL2를 통한 strict Skia GPU rendering
- Windows Release build/publish와 실제 GPU frame evidence
- 저장소 밖 Web template/package compile/publish evidence와 기존 수동 Chromium canvas/기본 pointer smoke

Native hover/wheel/keyboard/IME/UIA, Mac Catalyst native 실행, Web interaction 자동화, physical acceptance와 cross-target parity는 각각 독립적인 `notVerified` gate입니다. 과거 Win32/WGL과 AppKit/NSOpenGL 결과는 predecessor evidence로만 유지합니다.

## 구조

```text
제품 소유 Doroti.Framework.* source
                 │
                 ▼
        runtime + widget/render pipeline
                 │
                 ▼
         target host + GPU surface
   Windows MAUI · Mac Catalyst · WebGL2
```

Flutter source는 fidelity 작업에서 동작 reference가 필요할 때 사용합니다. Compiler output은 격리된 candidate이며 제품 source of truth가 아닙니다.

## 실행

.NET 10과 PowerShell 7이 필요합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
```

Release 주장 전에는 Windows GPU와 Web package/publish 통합 시나리오를 실행합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

## Repository 구성

| 경로 | 설명 |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | 제품 framework, runtime, rendering, host, target package와 SDK |
| [`DorotiDemoApp/`](DorotiDemoApp/) | Single-project Material dogfood 앱 |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` template |
| [`Doroti/eng/`](Doroti/eng/) | 간소화한 build, validation, release, 선택적 reference workflow |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | 선택적 Dart/Flutter import·migration compiler |
| [`history/`](history/) | Archive한 milestone 계획, 명령과 evidence 요약 |

명령과 evidence 경계는 [runtime README](Doroti/README.ko.md)를 참고하세요.

## Roadmap

현재 우선순위는 native desktop capability closure, Web live parity 자동화, target별 대표 release/physical acceptance flow입니다. Build, native live, browser live, physical, cross-target 결과는 서로 대신하지 않습니다.

Doroti는 실험적인 개인 프로젝트입니다. 아이디어, 피드백, fork와 독립적인 실험을 환영합니다.

## License

[LICENSE](LICENSE)와 [third-party notices](Doroti/THIRD-PARTY-NOTICES.md)를 참고하세요.

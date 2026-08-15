# Doroti

**한국어** | [English](README.md)

### C#과 .NET으로 만드는 멀티플랫폼 UI runtime.

**Doroti**는 하나의 C# UI codebase를 Web·Desktop·Mobile로 확장하기 위한 실험적인 멀티플랫폼 UI 프로젝트입니다. Flutter framework의 구조와 동작을 .NET으로 옮기고, Windows와 macOS desktop에서는 provenance가 고정된 **Avalonia platform source**를 선별 이식해 native window와 운영체제 서비스를 연결합니다. 앱 UI를 Avalonia control로 구성하지는 않습니다.

Dart로 작성된 Flutter framework의 의미를 C#으로 변환해 `MaterialApp`, `Scaffold`, `Text`, `Button` 같은 익숙한 위젯을 여러 실행 환경에서 사용할 수 있도록 만들고 있습니다. 목표 플랫폼은 **Web, Windows, Linux, macOS, Android와 iOS**입니다.

> Doroti는 Flutter 앱을 WebView에서 실행하거나, Flutter와 비슷한 UI component를 별도로 제공하는 프로젝트가 아닙니다.  
> Flutter framework의 구조와 동작을 가능한 한 그대로 C#과 .NET runtime으로 옮기는 프로젝트입니다.

## 지금 Doroti에서는

Native 제품 검증은 이제 Windows x64와 Apple Silicon macOS에서 실행됩니다. Flutter widget tree가 C# 위에서 생성되고 mount·layout·paint를 거쳐 실제 HWND 또는 NSWindow에 표시됩니다.

현재 데모 앱에서는 다음 요소들이 함께 동작합니다.

- `MaterialApp`, `Theme`, `Scaffold`, `AppBar`
- `Card`, `ListTile`, `Text`, `Icon`
- button, FAB, checkbox, radio, switch, slider
- `Row`, `Column`, `Stack`, scroll view, lazy list
- pointer interaction, state update, semantics
- Avalonia에서 선별 이식한 Win32 및 AppKit/libAvalonia window, dispatcher, input, text, clipboard, cursor, accessibility 구현
- Windows의 Skia WGL/OpenGL과 `osx-arm64`의 NSOpenGL 기반 strict GPU rendering

`MaterialApp.builder`뿐 아니라 `MaterialApp.home`과 Navigator를 거치는 앱 시작 경로도 실제 native frame으로 검증하고 있습니다.

## 왜 만들까요?

Flutter에는 잘 다듬어진 위젯, 일관된 렌더링 모델과 다양한 UI package가 있습니다. .NET에는 강력한 언어와 도구, 풍부한 라이브러리, 그리고 Web·Desktop·Mobile로 확장할 수 있는 실행 환경이 있습니다.

Doroti는 두 기술을 연결하는 조금 엉뚱하지만 신나는 질문에서 출발했습니다.

> **Flutter framework가 .NET에서 실제로 동작할 수 있을까?**

그래서 Doroti는 겉모습만 비슷하게 재구현하기보다 Flutter upstream source를 동작의 기준으로 삼습니다. 이를 위해 semantic compiler, runtime, rendering pipeline과 native host를 함께 개발합니다.

## 플랫폼 셸은 어떻게 구성되어 있나요?

플랫폼 셸은 Doroti runtime과 운영체제를 잇는 계층입니다. window lifecycle, dispatcher, pointer와 keyboard input, IME, clipboard, cursor, accessibility와 rendering surface를 담당합니다. 화면의 widget tree, layout, paint와 state lifecycle은 C#으로 옮긴 Flutter framework와 Doroti runtime이 소유합니다.

Desktop 제품 셸은 공통 `Doroti.Shell.Core` capability 위에 구성됩니다. `Doroti.Host.Desktop`은 `IShellWindowingPlatform`을 주입받고, `win-x64`와 `osx-arm64` composition root가 각각 Win32 또는 AppKit 구현을 연결합니다. 선별한 upstream source와 local adaptation은 provenance manifest로 추적합니다. Doroti 앱의 UI를 Avalonia `Control`이나 XAML로 구성하는 것은 아닙니다.

공식 `Avalonia.Desktop` package를 사용하는 host도 별도로 유지합니다. 이 경로는 기본 제품 셸이 아니라, 소스 이식 host와 동작을 비교하고 rendering·input·window lifecycle을 검증하는 A/B reference입니다.

## 작동 방식

```text
Flutter source
      ↓ semantic compilation
C# framework packages
      ↓
Doroti runtime + widget/rendering pipeline
      ↓
platform host + rendering surface
(Windows: Win32/WGL · macOS: AppKit/NSOpenGL)
      ↓
Web / Windows / Linux / macOS / Android / iOS
```

- **Semantic compiler**가 Dart/Flutter의 타입과 언어 의미를 분석해 C#으로 변환합니다.
- **Doroti runtime**이 Flutter의 scheduler, widget, element, rendering lifecycle을 연결합니다.
- **Platform host**가 각 target의 window 또는 view, input, accessibility와 rendering surface를 연결합니다. Windows와 macOS host는 같은 typed shell 경계 뒤에 target별 source port를 둡니다.
- 변환 결과는 검토 가능한 C# source와 .NET package가 됩니다.

생성된 코드를 몰래 손보는 방식 대신, 문제가 생기면 compiler와 runtime의 공통 의미를 고치고 다시 생성하는 것을 원칙으로 합니다.

## 실행해 보기

현재 데모와 자동 native smoke validation은 **Windows x64**와 **Apple Silicon macOS(`osx-arm64`)**에서 실행할 수 있습니다. .NET SDK 10과 PowerShell 7이 필요합니다.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
dotnet run --project ./DorotiDemoApp
```

짧은 자동 smoke run도 준비되어 있습니다.

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry builder --frames 3 --duration-ms 15000
```

> Windows x64와 `osx-arm64`는 구현된 native desktop target입니다. 물리 Korean IME candidate placement, VoiceOver 탐색, precise trackpad gesture, Intel macOS, Web, Linux, Android와 iOS에는 각각 후속 acceptance 또는 구현 gate가 남아 있습니다.

## 프로젝트 구성

| 경로 | 내용 |
| --- | --- |
| [`Doroti/`](Doroti/) | compiler가 만든 framework, runtime, renderer와 platform host |
| [`Doroti/src/Doroti.Host.Desktop/`](Doroti/src/Doroti.Host.Desktop/) | Windows와 macOS composition root가 함께 사용하는 typed desktop host |
| [`Doroti/src/Doroti.Target.macOS.osx-arm64/`](Doroti/src/Doroti.Target.macOS.osx-arm64/) | Apple Silicon AppKit/NSOpenGL target package |
| [`Doroti/src/Doroti.Host.Avalonia/`](Doroti/src/Doroti.Host.Avalonia/) | 공식 `Avalonia.Desktop` package 기반 비교·검증용 host |
| [`DorotiDemoApp/`](DorotiDemoApp/) | 실제 Material widget tree를 띄우는 데모 앱 |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Dart를 C#으로 변환하는 semantic compiler |
| [`goal7.md`](goal7.md) | 현재 개발 상태, Web 빌드와 다음 release gate를 정리한 로드맵 |

더 깊은 빌드·검증 방법과 아키텍처 기록은 [`Doroti/README.md`](Doroti/README.md)와 [`Doroti/docs/`](Doroti/docs/)에 모아두었습니다.

## 앞으로의 계획

지금의 목표는 단순히 “Flutter 파일이 C#으로 빌드된다”가 아닙니다. 일반적인 앱이 실제로 화면을 전환하고, 입력받고, 스크롤하고, 접근성 도구와 연동할 수 있는 멀티플랫폼 UI runtime을 만드는 것입니다.

앞으로는 navigation과 dialog, form과 물리 IME acceptance, 대규모 scrolling, assets와 localization, 더 많은 Material/Cupertino component를 구현합니다. 현재 Windows와 Apple Silicon macOS 검증을 기반으로 **Web, Linux, Intel macOS, Android와 iOS**까지 지원 범위를 확대할 계획입니다.

Doroti는 아직 개발 중인 실험적 프로젝트입니다. compiler, runtime, rendering과 UI framework가 만나는 기술에 관심이 있다면 프로젝트의 다음 업데이트도 함께 지켜봐 주세요!

## 참여와 포크

Doroti는 개인적으로 즐기며 만들고 있는 취미 프로젝트입니다. 그래서 pull request를 적극적으로 검토하거나 병합하기는 어려울 수 있습니다. 다만 프로젝트에 관한 아이디어와 의견은 언제든 반갑게 듣고 싶습니다.

Doroti를 바탕으로 직접 실험하거나 새로운 방향으로 발전시키는 포크도 환영합니다. 각자의 방식으로 자유롭게 탐구하고 재미있는 가능성을 만들어 주세요!

## 관련 프로젝트

Doroti는 다음 프로젝트의 소스와 설계에서 많은 도움을 받고 있습니다.

- [Flutter](https://github.com/flutter/flutter) — framework 구조와 동작의 기준
- [Avalonia](https://github.com/AvaloniaUI/Avalonia) — 선별한 Windows/macOS desktop platform 구현의 source 기반

## License

Doroti의 라이선스는 [`LICENSE`](LICENSE)에서, 사용한 upstream source와 고지는 [`Doroti/THIRD-PARTY-NOTICES.md`](Doroti/THIRD-PARTY-NOTICES.md)에서 확인할 수 있습니다.

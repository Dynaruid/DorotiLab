# Doroti

[English](README.md) | **한국어**

### XAML 없이 C#으로 만드는 크로스 플랫폼 UI

Doroti는 C#과 .NET으로 개발하는 실험적인 UI 프레임워크입니다. 위젯, 레이아웃, UI 동작을 C# 코드로 직접 작성하고, 데스크톱·모바일·웹에서 애플리케이션 코드를 공유할 수 있습니다.

Flutter 프레임워크 소스를 C#으로 변환하는 데서 시작했으며, 현재는 C# 코드를 직접 개발하고 유지보수합니다. 익숙한 Material·Cupertino API에 SkiaSharp 기반의 공통 렌더링 파이프라인과 플랫폼별 네이티브 연동을 제공합니다.

[시작하기](#시작하기) · [플랫폼별 구현](#플랫폼별-구현) · [문서](#문서)

> [!WARNING]
> Doroti는 활발히 개발 중인 실험적 프로젝트입니다. API, 동작, 프로젝트 구조는 하위 호환성 보장 없이 변경될 수 있으며, 플랫폼별 완성도에는 차이가 있습니다.

## 주요 특징

- **C#으로 작성하는 UI** — XAML 없이 위젯, 레이아웃, 상태와 상호작용을 정의합니다.
- **애플리케이션 코드 공유** — 플랫폼 중립 라이브러리에 UI를 작성하고, 각 플랫폼의 실행 프로젝트에서 사용합니다.
- **Material·Cupertino 위젯** — Flutter에서 익숙한 API를 C#으로 구현하고 유지보수합니다.
- **SkiaSharp 기반 GPU 렌더링** — C#에서 SkiaSharp를 통해 Skia Graphite를 사용하며, 플랫폼에 따라 Vulkan, Metal, WebGPU로 렌더링합니다.
- **네이티브 연동** — 플랫폼 호스트가 창, 입력, 텍스트 입력, 클립보드, 접근성 서비스를 연결합니다.
- **샘플 앱과 템플릿** — `DorotiTestbedApp`과 `doroti-app` 프로젝트 템플릿으로 구성을 살펴볼 수 있습니다.

## 플랫폼별 구현

위젯, 레이아웃, 페인팅, 시맨틱스 계층은 플랫폼 간에 공유합니다. 각 호스트는 네이티브 연동과 GPU 렌더링 표면을 제공합니다.

| 플랫폼 | 네이티브 호스트 | Doroti 구현체 | 기본 렌더링 백엔드 |
| --- | --- | --- | --- |
| Windows (기본) | Windows App SDK, C++ 자식 HWND (`HwndExactCpp`) | `Doroti.Host.WindowsAppSdk` + 네이티브 C++ 호스트 | Skia Graphite / Vulkan |
| Windows (선택) | .NET MAUI / WinUI | `Doroti.Host.Maui` | Skia Graphite / Vulkan |
| Android | .NET MAUI / Android 네이티브 뷰 | `Doroti.Host.Maui` | Skia Graphite / Vulkan |
| iOS | .NET MAUI / UIKit | `Doroti.Host.Maui` | Skia Graphite / Metal |
| macOS | 네이티브 AppKit / MetalKit (`MTKView`) | `Doroti.Host.Maui`의 AppKit 어댑터 | Skia Graphite / Metal |
| Mac Catalyst | .NET MAUI / UIKit (Mac Catalyst) | `Doroti.Host.Maui` | Skia Graphite / Metal |
| Web | .NET WebAssembly, 렌더링 Worker / canvas | `Doroti.Host.Web` | Skia Graphite / Dawn / WebGPU |
| Linux | Qt 6 `QWindow`, 네이티브 C ABI 브리지 | `Doroti.Host.Qt` | Skia Graphite / Vulkan |

Web에서는 Ganesh/WebGL2도 명시적으로 선택할 수 있습니다. 구현체별 검증 범위에는 차이가 있으므로 [프로젝트 상태](#프로젝트-상태)를 함께 참고하세요.

## 시작하기

샘플 앱으로 Doroti를 살펴볼 수 있습니다. 빌드 호스트에 다음 의존성을 준비하세요:

- **Windows·macOS·Linux 공통:** 저장소 스크립트 실행용 [PowerShell 7](https://learn.microsoft.com/ko-kr/powershell/scripting/install/install-powershell?view=powershell-7.6)을 설치합니다.
- **.NET SDK:** iOS 외 대상에는 10.0.400 계열을 설치합니다. iOS Testbed에는 .NET 10과 함께 11.0.100-rc.1.26425.128도 설치합니다.
- **플랫폼별 도구:** 기본 Windows runner에는 [플랫폼별 준비 사항](Doroti/README.ko.md)의 Windows C++ 빌드 도구가 필요합니다. 다른 대상의 workload와 네이티브 도구도 해당 안내를 참고하세요.

저장소를 복제한 뒤 루트 디렉터리에서 샘플을 실행합니다.

```powershell
git clone https://github.com/Dynaruid/DorotiLab.git
cd DorotiLab

$env:DOROTI_TESTBED_MODE = 'sample'
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform windows
```

이 명령은 기본 Windows App SDK 실행 프로젝트를 빌드하고 실행합니다. Android, iOS, macOS, Mac Catalyst, Linux, Web 실행 방법은 [샘플 앱 가이드](DorotiTestbedApp/README.ko.md), 빌드·배포 옵션은 [프레임워크 가이드](Doroti/README.ko.md)를 참고하세요.

## 동작 구조

Doroti가 위젯 트리와 렌더 트리를 관리합니다. 플랫폼 호스트는 네이티브 서비스와 GPU 렌더링 표면을 제공하고, 공통 프레임워크는 레이아웃, 페인팅, UI 상태를 처리합니다.

```text
C# 애플리케이션
      ↓
Doroti 위젯, 레이아웃, 상태
      ↓
공통 렌더링 파이프라인
      ↓
플랫폼 호스트 + GPU 렌더링 표면
```

Material·Cupertino API의 동작은 Flutter를 참고합니다. 실제 구현은 `Doroti.Framework.*`의 C# 코드로 유지보수하며, Flutter 런타임을 WebView에 넣어 실행하는 방식은 아닙니다.

프로젝트 초반에는 Dart-to-C# 컴파일러로 프레임워크의 기반을 만들었습니다. 현재는 C# 코드를 직접 개발하며, 컴파일러는 필요한 소스를 가져오거나 동작을 비교하는 선택적 도구로 남아 있습니다.

## 문서

| 가이드 | 내용 |
| --- | --- |
| [프레임워크 가이드](Doroti/README.ko.md) | SDK, 워크로드, 빌드 명령, 패키징, 호스트 설정 |
| [샘플 앱 가이드](DorotiTestbedApp/README.ko.md) | 플랫폼별 실행, 샘플 화면, 렌더러 옵션, 문제 해결 |
| [Dart-to-C# 컴파일러](tools/Doroti.DartToCSharp/README.ko.md) | 선택적 소스 가져오기 및 마이그레이션 도구 |
| [개발 이력](history/) | 지난 작업 계획과 검증 기록 |

## 프로젝트 상태

Doroti는 개인이 개발하는 실험적 프로젝트입니다. 플랫폼 표는 구현된 호스트와 기본 렌더링 구성을 나타내며, 모든 플랫폼이 동일한 수준으로 제품 사용을 준비했다는 의미는 아닙니다.

빌드 검사, 네이티브·브라우저 실행, 실제 기기 테스트는 구분하여 기록합니다. GPU 호환성, 입력기, 접근성, 성능, 서명과 스토어 배포는 플랫폼별 검증이 더 필요합니다. 사용할 플랫폼을 선택하기 전에 [프레임워크 가이드](Doroti/README.ko.md)와 검증 기록을 확인하세요.

현재는 네이티브 데스크톱 연동, 웹 동작 검증 자동화, 각 플랫폼의 대표적인 릴리스·실제 기기 테스트를 우선 진행하고 있습니다.

## 저장소 구성

| 경로 | 내용 |
| --- | --- |
| [`Doroti/src/`](Doroti/src/) | 프레임워크, 런타임, 렌더링, 호스트, SDK |
| [`DorotiTestbedApp/`](DorotiTestbedApp/) | 공통 샘플 앱과 플랫폼별 실행 프로젝트 |
| [`Doroti/templates/`](Doroti/templates/) | `dotnet new doroti-app` 템플릿 |
| [`Doroti/eng/`](Doroti/eng/) | 빌드, 실행, 패키징, 진단 도구 |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | 선택적 Dart-to-C# 컴파일러 |

## 피드백과 기여

Doroti는 개인적으로 즐기며 만들고 있는 취미 프로젝트입니다. 그래서 Pull Request를 적극적으로 검토하거나 병합하기는 어려울 수 있습니다.

아이디어와 버그 제보를 환영하며, Doroti를 바탕으로 직접 실험하거나 새로운 방향으로 발전시키는 포크도 환영합니다. 문제를 제보할 때는 플랫폼, .NET SDK 버전, 렌더링 백엔드와 재현 방법을 함께 알려주세요.

## 라이선스

Doroti는 [BSD 3-Clause 라이선스](LICENSE)를 따릅니다. 외부 소스와 패키지의 저작권 표기는 [서드파티 고지](Doroti/THIRD-PARTY-NOTICES.md)를 참고하세요.

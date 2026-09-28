# Doroti 작업 계획

기준: [plan.md](../plan.md), 검토 기준일 2026-09-28.

이 폴더는 원본 로드맵을 공통 기능 작업과 플랫폼별 연결·검증 작업으로 나눈 실행 계획이다. 문서 작성 시점의 작업 상태는 **TODO**, 새 실행 검증은 **notVerified**다. 기존 구현이 없다는 뜻이 아니며, 착수할 때 현재 branch의 구현과 과거 증거를 확인해 중복 작업을 줄인다. 이번 문서 분리는 제품 구현·빌드·성능·실기기 검증 결과를 추가하지 않는다.

## 문서 사용 방법

- `common/`은 M0~M7의 공개 계약, 공유 구현, 테스트 기반과 도구 작업을 담당한다. 원문의 기능 체크리스트와 완료 기준을 바탕으로 하며, 테스트 파일의 배치·실행·정리는 [M1의 임시 테스트 규칙](common/01-testing.md#임시-테스트의-배치실행정리)을 적용한다.
- `platforms/`는 공통 기능을 해당 OS·호스트에 연결하고 검증하는 작업이다. 공통 체크리스트를 별개의 중복 기능으로 구현하지 않고 공통 작업 ID와 결과를 연결한다.
- `plan.md`는 원본 로드맵으로 보존한다. 상세 진행 상황은 이 폴더에 기록하고, 우선순위·범위가 바뀌면 원본과 관련 문서를 함께 맞춘다.
- 동일 OS의 서로 다른 호스트와 renderer는 지원·검증 결과를 분리한다. 특히 Windows App SDK/MAUI, macOS AppKit/Mac Catalyst, iOS Graphite/기타 renderer, Qt Quick/Widgets 결과를 서로 대체하지 않는다.

## 공통 작업

| 단계 | 우선순위 | 작업 문서 | 선행 조건 |
| --- | --- | --- | --- |
| M0 | P0 | [실행·검증·지원 현황 정리](common/00-foundation.md) | 없음 |
| M1 | P0 → P1 | [테스트 런타임과 핵심 회귀 장면](common/01-testing.md) | M0 |
| M2-A | P1 | [Desktop 계약과 호스트 연결](common/02-desktop-contract.md) | M1 최소 경로 |
| M2-B | P1 | [텍스트·접근성·PlatformView](common/03-input-accessibility-platformview.md) | M1 최소 경로 |
| M2-C | P1 | [렌더링 성능과 리소스 수명](common/04-rendering-lifetime.md) | M1 최소 경로 |
| M3 | P1 | [VS Code 실행 확장·작성 지원·Hot Reload](common/05-vscode-hot-reload.md) | A/B: M0, C: M1 + 개발 호스트 + B, D: C# 프로젝트 로딩 |
| M4 | P2 | [플러그인 SDK와 대표 네이티브 기능](common/06-plugin-sdk.md) | M0/M1, M2/M6 수명 계약 정합 |
| M5-A | P2 | [OS Drag & Drop](common/07-os-drag-drop.md) | M1/M2, M4 파일 계약 |
| M5-B | P2 | [Deep Link·lifecycle·상태 복원](common/08-navigation-restoration.md) | M1/M2 |
| M6 | P2 | [창 문맥과 실제 멀티윈도우](common/09-multiwindow.md) | M2-A, M1 격리·수명 |
| M7 | P3 | [외부 패키지 소비·배포·출시 기준](common/10-release-packaging.md) | 출시 대상 기능의 선행 단계 |

## 플랫폼별 작업

| 플랫폼 | 작업 문서 | 우선 역할·지원 경계 |
| --- | --- | --- |
| Windows | [Windows](platforms/windows.md) | Windows App SDK가 첫 desktop 기준. MAUI 연결·검증은 별도 |
| Web | [Web](platforms/web.md) | 첫 자동화·개발 기준. 브라우저·renderer·모바일 Web을 구분 |
| Android | [Android](platforms/android.md) | 첫 모바일 실기기 기준. 에뮬레이터와 물리 기기 결과를 구분 |
| macOS | [macOS / AppKit](platforms/macos.md) | AppKit 호스트와 Metal·WKWebView 검증 |
| iOS | [iOS](platforms/ios.md) | UIKit·renderer·실기기·시뮬레이터 결과를 구분 |
| Linux | [Linux / Qt](platforms/linux.md) | Qt Quick/Widgets, Wayland/XWayland와 GPU 환경을 구분 |
| Mac Catalyst | [Mac Catalyst](platforms/maccatalyst.md) | 별도 adapter·scene·배포 검증 |

Windows App SDK와 Web을 첫 개발·자동화 기준으로, Android 실기기를 첫 모바일 기준으로 삼는다. 다른 플랫폼의 완료를 기다리며 이들 작업을 묶어 두지 않는다. M6 실제 멀티윈도우의 이번 대상은 Windows/AppKit/Qt/Catalyst이며 Web·Android·iOS의 추가 창 구현을 새 필수 범위로 늘리지 않는다.

## 실행 순서와 첫 작업 묶음

기본 순서는 **M0 → M1 최소 구현 → M2 → M3 → M4/M5 → M6 → M7**이다. M1의 coverage는 이후 단계에서 필요한 만큼 늘린다. M2의 플랫폼 검증은 계속 유지하고, M3은 기본 호스트가 안정되는 대로 시작할 수 있다. M7의 package-only smoke는 M0부터 가볍게 시작해 마지막에 문제를 몰아 발견하지 않게 한다.

첫 작업 묶음은 다음 다섯 개로 제한한다.

1. `doroti.ps1`의 끊어진 validate/audit/release 경로와 실패 전파를 복구한다.
2. 문서 링크와 Desktop/PlatformView 지원표를 현재 코드에 맞춘다.
3. 최소 테스트 host로 `pumpWidget`·pointer tap·bounded settle을 만들고 Cupertino 탭 회귀 하나를 자동화한다.
4. Windows App SDK Desktop adapter를 붙여 Testbed 기본 runner에서 창 크기·제목·close를 시연한다.
5. 같은 runner에서 한글 조합 입력과 native TextBox/WebView focus 이동을 재현하는 장면 및 실측 결과를 남긴다.

M3-A/B의 VS Code 프로젝트 생성·실행 기능은 M0의 CLI 복구 후 독립적으로 진행하고, 앱의 C# 프로젝트 로딩이 준비되면 M3-D의 import·Widget Snippets를 연결할 수 있다. 위 묶음과 M1의 최소 회귀 경로가 준비되면 M3-C의 Hot Reload runtime·전용 버튼을 연결한다. 생성 마법사 → snippet 작성·import → 실행 → Hot Reload까지 통합 검증한 VSIX를 이번 확장 산출물로 삼는다. Preview·Inspector UI·전체 플랫폼 IDE 지원·별도 C# LSP·시각적 drag & drop 디자이너, 모든 플러그인 동시 구현, 전체 Flutter 테스트 일괄 이식, 무차별적인 `NotImplementedException` 제거는 초기 범위에서 제외한다.

첫 작업의 담당 문서: 1·2는 [M0](common/00-foundation.md), 3은 [M1](common/01-testing.md), 4는 [M2-A](common/02-desktop-contract.md)와 [Windows](platforms/windows.md), 5는 [M2-B](common/03-input-accessibility-platformview.md)와 [Windows](platforms/windows.md)다.

## 공통 완료 규칙

- 각 작업은 **공개 계약·호스트 연결·실행 가능한 샘플·필요한 회귀 검증·지원표 갱신**을 함께 완료한다. 문서 작성이나 컴파일 성공만으로 기능 구현을 완료 처리하지 않는다.
- 결과에 commit/revision, OS·기기·renderer, configuration/AOT, 명령, 기대값·실제값을 기록한다. 과거 결과는 날짜와 적용 범위를 표시한다.
- `PASS`는 실행한 범위에만 사용한다. 실행하지 않은 환경은 `notVerified`, 일부 성공은 `PARTIAL`, 의도적으로 제공하지 않는 기능은 `unsupported`로 남긴다.
- 테스트는 저장소 지침대로 **20분 timeout**을 사용한다. 일반 반복 검증은 30회 이내에서 설계하고, 별도 장기 시험이 필요하면 목적·종료 조건을 정한다.
- 일회성 테스트 스크립트·프로젝트와 테스트 원시 로그·캡처·빌드 산출물은 저장소 루트 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고, 결과 요약 후 삭제한다. 최소 상시 회귀 소스·작은 fixture만 `Doroti/tests/`에 선별해 남긴다. 일반 제품 빌드 산출물과 기존 `Doroti/artifacts`는 기존 정리 지침을 따른다.
- 임시 테스트는 기본 빌드·watch·패키징·전체 CI에서 제외한다. 실패 조사로 임시 파일을 남길 때는 보류 사유·정리 시점을 기록하고, 삭제 후에는 원시 파일이 남아 있다고 보고하지 않는다. 검증 파일의 영구 보존 없이도 결과 요약과 재현 절차로 필요한 검증을 기록할 수 있다.
- Flutter는 동작·렌더링·입력 모델의 비교 기준으로 사용한다. C#에서 유지보수하는 제품 소스를 기본으로 삼고, compiler는 필요한 import/의미 비교에 사용한다.

## 결과 기록 형식

작업의 `TODO`/진행/완료와 실행 결과의 `PASS`/`PARTIAL`/`FAILED`/`notVerified`/`unsupported`를 구분한다. 아래 형식은 실행 후 각 작업 문서에 채운다. 미실행은 실패와 구별하고, 과거 결과를 인용하면 날짜와 적용 범위를 명시한다.

```text
작업 ID / 공통·플랫폼 문서:
작업 상태 / 검증 결과:
commit 또는 revision / 실행일:
OS·버전 / 호스트 / 기기 / renderer / configuration·AOT:
재현 명령 / fixture·샘플 / timeout:
기대값 / 실제값:
증거 종류: 정적 검토 / build / 자동 실행 / offscreen / 실제 화면 / 물리 입력·보조기술
보존한 결과 요약 위치:
임시 테스트 경로 / 정리 여부 / 보류 시 사유·정리 시점:
남은 작업·미검증 조합:
```

## 원문과 참고 자료

[원본 로드맵](../plan.md)의 §2는 기존 구현과 선행 결함에 대한 정적 검토, §6은 그 근거 파일 목록이다. 이 문서들은 그 검토를 새 실행 결과로 승격하지 않는다. `ref1.md`는 현재 체크아웃에 없으므로 원문의 재검토 요약을 참고한다.

- [저장소 지침](../.github/copilot-instructions.md)
- [CLI](../Doroti/eng/doroti.ps1), [Testbed runner manifest](../samples/DorotiTestbedApp/doroti-workspace.json)
- [Desktop 현황](../Doroti/docs/desktop-windows.md), [PlatformView 지원표](../Doroti/docs/platform-views/support-matrix.md)

지원 문서도 과거 기록과 현재 상태가 섞여 있을 수 있다. 최신 지원표 정리와 깨진 실행·문서 경로 복구는 M0 작업으로 추적한다.

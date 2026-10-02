# Doroti 작업 계획

보관일: **2026-10-03**. 루트 `works/`의 문서 35개와 결과 JSON 16개를 이 위치로 옮겼다. 아래 계획·체크리스트·실행 판정은 당시 이력이며, 이 폴더를 활성 작업 계획으로 관리하지 않는다. [보관 요약](../works-summary.md)에 이동 범위와 검증 경계를 기록했다. 현재 제품 계약·지원 현황은 [제품 문서](../../../Doroti/docs/)와 [지원표](../../../Doroti/docs/support-status.md), 실행 가능한 검사는 [테스트 안내](../../../Doroti/tests/README.md)를 따른다.

2026-10-02 Linux Qt 핫리로드: [구성·실행 결과](results/2026-10-02-linux-qt-hot-reload.md). CLI·설치한 VSIX에서 실제 metadata update, 상태 유지, 오류 복구, 명시 Restart·Stop PASS. Linux Quick Debug 소스 앱 범위이며 native/assets·다중 창·NuGet-only 개발 인수는 별도다.

2026-10-02 Linux Qt 구성 후속: [검토·보완 결과](results/2026-10-02-linux-qt-configuration-review.md). SDK·샘플·템플릿 Quick/C 기본값, 옵션별 native cache·오류 거절, package-only Release/설치와 consumer 회수 검증 PASS. 물리 GPU·표시/입력 및 전체 플랫폼 인수는 PARTIAL.

2026-09-29 iOS/Mac Catalyst 후속: [구현·검증 결과](results/2026-09-29-ios-catalyst.md). UIKit 서비스·Catalyst PlatformView/추가 scene·activation 연결과 별도 smoke를 보강했다. 개발 provisioning profile을 발급했고, 후속 요청으로 iPhone 12에 source Debug 설치·기동·실제 화면을 확인했다. 물리 입력·VoiceOver·실제 배포 등은 남아 전체 **PARTIAL**이다.

2026-09-29 macOS 후속: [macOS AppKit 구현·실행 결과](results/2026-09-29-macos-appkit.md). 실제 추가 창·양 Metal renderer·native editor/WKWebView 재생성·파일 선택 취소·Copy drop 연결·activation/복원을 구현·검증했다. 전체는 물리 입력·VoiceOver·송신·clean 배포 잔여 때문에 **PARTIAL**이다.

2026-09-29 Linux 후속: [Qt 구현·실행 결과](results/2026-09-29-linux-qt.md). Quick 양 QPA·Widgets 비교·실제 추가 창·플러그인/drop/navigation·Linux package-only와 portable 설치 PASS; 물리 입력·Orca·물리 GPU·clean 배포는 PARTIAL.

2026-09-29 후속 구현·검증: [Web·Windows·Android 실행 결과](results/2026-09-29-web-windows-android.md). 전체 **PARTIAL**, 범위별 PASS와 미완료를 분리한다. CI workflow는 사용자 지시로 삭제 상태를 유지한다.

기준: [plan 요약](../../26-09-28/plan-summary.md), 실행 갱신일 2026-09-29. 00~10 전체 실행의 구현·검증 및 미완료 범위는
[이번 실행 상태](#2026-09-29-전체-실행-상태)에 정리했다. 다음 단계 착수 조건과 전체 완료를 구분한다.

이 폴더는 원본 로드맵을 공통 기능 작업과 플랫폼별 연결·검증 작업으로 나눈 실행 계획이다. 초기 작성 상태는 TODO/notVerified였으며, 2026-09-28~29 결과를 반영한 현재 상태는 **M0 로컬 기반 완료, M1~M7 PARTIAL**이다. Windows/Web/Android/Linux/macOS에는 범위별 실행 결과가 있고, 나머지 플랫폼의 TODO는 이번 계획에서 새로 검증하지 않았다는 뜻이다. 기존 구현의 부재를 뜻하지 않는다. 최신 범위는 아래 실행 기록과 [지원표](../../../Doroti/docs/support-status.md)를 따른다.

2026-09-29 문서 정합 갱신: 기존 실행 근거로 구현 완료와 검증 잔여 체크박스를 분리하고 Android·CPU 테스트·추가 창 상태를 맞췄다. 새 제품 실행은 하지 않았다. 후속 작업은 [미완료 검토 요약](../../26-09-29/work2-summary.md)을 참고한다. 기존 release 후보와 이후 소스 보강의 검증 결과는 합치지 않는다.

## 문서 사용 방법

- `common/`은 M0~M7의 공개 계약, 공유 구현, 테스트 기반과 도구 작업을 담당한다. 원문의 기능 체크리스트와 완료 기준을 바탕으로 하며, 테스트 파일의 배치·실행·정리는 [M1의 임시 테스트 규칙](common/01-testing.md#임시-테스트의-배치실행정리)을 적용한다.
- `platforms/`는 공통 기능을 해당 OS·호스트에 연결하고 검증하는 작업이다. 공통 체크리스트를 별개의 중복 기능으로 구현하지 않고 공통 작업 ID와 결과를 연결한다.
- 루트 `plan.md`와 `work2.md`~`work6.md`는 아래 날짜별 요약으로 보관했다. 이 폴더는 당시 상세 진행 상황을 보존하며, 이후 제품 변경·지원 현황은 제품 문서에 기록한다.
- 동일 OS의 서로 다른 호스트와 renderer는 지원·검증 결과를 분리한다. 특히 Windows App SDK/MAUI, macOS AppKit/Mac Catalyst, iOS Graphite/기타 renderer, Qt Quick/Widgets 결과를 서로 대체하지 않는다.

## 보관된 루트 작업 문서

2026-10-03에 원문 기준일·작성일·수정일에 맞춰 요약을 보관하고 루트 원본 6개를 삭제했다. 후속 실행 이력과 미완료 판정은 각 요약에 보존하며, 보관 작업 자체를 새 제품 검증으로 집계하지 않는다.

| 원문 | 날짜 폴더 | 보관 요약 |
| --- | --- | --- |
| `plan.md` | `26-09-28` | [작업 진행 순서](../../26-09-28/plan-summary.md) |
| `work2.md` | `26-09-29` | [works 미완료 검토](../../26-09-29/work2-summary.md) |
| `work3.md` | `26-10-01` | [Variable Blur Fixed 후보](../../26-10-01/work3-summary.md) |
| `work4.md` | `26-10-02` | [iOS 프레임 연결·C/Fast 채택](../../26-10-02/work4-summary.md) |
| `work5.md` | `26-10-02` | [네이티브 공통 C 경로](../../26-10-02/work5-summary.md) |
| `work6.md` | `26-10-02` | [C 단일화·동적 texture 예산](../../26-10-02/work6-summary.md) |

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

## 초기 실행 순서와 첫 작업 묶음

아래는 최초 착수 순서의 기록이다. 1~4의 로컬 기반과 5의 입력 장면·합성/native 수명 회귀는 구현됐다. 현재 남은 물리 입력·화면·GPU·다른 adapter·출시 검증은 [최신 보강 결과](#2026-09-29-0010-보강-검토)와 [미완료 검토 요약](../../26-09-29/work2-summary.md)을 따른다.

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

[로드맵 요약](../../26-09-28/plan-summary.md)의 §2는 기존 구현과 선행 결함에 대한 정적 검토, §6은 그 근거 파일 목록이다. 이 문서들은 그 검토를 새 실행 결과로 승격하지 않는다. `ref1.md`는 현재 체크아웃에 없으므로 원문의 재검토 요약을 참고한다.

- [저장소 지침](../../../.github/copilot-instructions.md)
- [CLI](../../../Doroti/eng/doroti.ps1), [Testbed runner manifest](../../../samples/DorotiTestbedApp/doroti-workspace.json)
- [Desktop 현황](../../../Doroti/docs/desktop-windows.md), [PlatformView 지원표](../../../Doroti/docs/platform-views/support-matrix.md)

지원 문서도 과거 기록과 현재 상태가 섞여 있을 수 있다. 최신 지원표 정리와 깨진 실행·문서 경로 복구는 M0 작업으로 추적한다.

## 2026-09-29 전체 실행 상태

기준 revision `8834d7596597b3087a0139f527148baee9a46583` + 미커밋 작업 트리.
사용자 요청 범위는 00~10 전부이며, 아래 잔여 범위를 삭제하거나 완료로 바꾸지 않는다.
아래는 앞선 실행 기록이며, 후속 변경은 [보강 검토](#2026-09-29-0010-보강-검토)를 따른다.

| 문서 | 이번 실행 및 현재 상태 | 남은 범위 |
| --- | --- | --- |
| 00 | Source/CLI 실패 전파 및 기존 Developer 경로 재검증, 선택 플랫폼 후보 release 추가 | 원격 CI·clean OS 배포 |
| 01 | 기존 widget/CPU 수명 + Dialog 중간 프레임/모달 hit-test + 탐색/복원 + 동시 dispatcher pointer·IME·semantics 회귀 | GPU golden, 전체 native 자원 격리 |
| 02 | 기존 Desktop에 실제 Windows 추가 창과 Explicit 종료 연결 | 실제 live resize 픽셀·mixed-monitor·다른 호스트 재검증 |
| 03 | 입력/PlatformView 장면 유지, 창별 text/keyboard/context menu 상태 격리 | Windows 화면 도구 pipe unavailable; 물리 IME·보조기술·전체 native content |
| 04 | CPU DPR/blur geometry/list/reassemble/layer 회귀, 중첩 retained scene의 native view 해제 경쟁 수정 | GPU 품질·present/VRAM 예산·device loss·장기 실기기 |
| 05 | 기존 설치 VSIX/Hot Reload 구현 유지; 창별 binding 수정은 별도 회귀 수행 | 이번 세션 editor/browser 도구 연결 미제공; 통합 IDE 재실행 및 기존 미완료 UI 사례 |
| 06 | 기존 plugin 취소·오류·수명 회귀 유지 | Web/Android FilePicker, event stream, 다른 native adapter |
| 07 | 기존 OS drop 계약 회귀 유지 | Explorer 교차 창, 송신·move/link·다른 플랫폼 |
| 08 | 공통·Windows·Web·Android 연결, Windows cold/warm/restart 및 Galaxy 실제 route 화면 PASS | 실제 browser history, iOS Universal Links·다른 desktop |
| 09 | 창 문맥 ADR·Windows 실제 2창, 독립 크기·종료·Explicit 및 NuGet 소비 PASS | 물리 IME·DPI·native content 전체, AppKit/Qt/Catalyst 추가 창 |
| 10 | 새 버전 feed·template·격리 cache 소비·Windows/Web Release publish·portable installer | 서명·clean VM·실제 배포 업데이트·장기 soak·Android package-only |

현재 도구/장비: Windows 10.0.26200, .NET SDK 10.0.400, Galaxy S25 Android 16.
Windows 화면 도구는 `native pipe is unavailable`, 브라우저 도구의 브라우저 목록은 `[]`였다.
이 실패를 회피해 native/API 테스트를 화면·물리 입력 PASS로 승격하지 않았다.
Android는 ADB intent와 실제 화면 캡처를 사용했고 손가락/물리 키보드 입력과 구분했다.

모든 검증은 1,200초 timeout을 사용한다. 원시 로그·소비 앱·스크린샷은
`temp/testing/plan-all/`, `temp/testing/release-candidate/`의 이번 실행 폴더에 생성한다.
최종 후보 패키지는 삭제 가능한 `Doroti/artifacts/release/<version>/`에 보존하며
각 `candidate.json`의 payload/package hash·toolchain·cleanup 상태를 확인한다.
이전 후보의 cache 정리는 MSBuild가 NuGet DLL을 보유해 한 번 실패했다.
수정 실행기는 MSBuild node reuse를 끄고 실패 시 정리 보류 경로를 기록한다.

### 당시 최종 산출물·정리 보류

후보 생성 실행 당시 최종 검증 후보는 `Doroti/artifacts/release/0.3.0-beta.rc.20260929010529/`이다.
아래 보강 검토에서는 새 후보를 생성하지 않았으므로 이후 소스 수정의 로컬 PASS를 이 payload에 적용하지 않는다.
이 후보의 자체 raw 소비 앱/cache/log는 자동 정리됐고, manifest에 native 실행과
portable 설치·업데이트·변조 거절·제거·userdata 보존 결과를 기록했다.

후속 수동 정리 명령은 경로를 workspace 내부로 제한했지만 자동 승인 검토가
`blocked by policy`로 거절했다. 구체적 이유는 제공되지 않았고 다른 도구로 우회하지 않았다.
다음 실패 조사/이전 후보를 정리 보류한다. `plan-all`의 원시 검증 자료도 이번 정리 단계에서 남겨 두었다.

- `temp/testing/plan-all/`: 이번 로그·Android 캡처·portable 설치 fixture/canary.
- `temp/testing/developer/d410872debb0425c81716f57d527bd4c/`: 공유 action listener 경쟁 조사(수정 후 PASS).
- `temp/testing/developer/b46c2df3346342a087eb0bca556a1b85/`: retirement fixture overload 수정 전 build 조사(수정 후 PASS).
- `temp/testing/release-candidate/1c38349e85f7/`: SDK type declaration 누락 조사(수정 후 PASS).
- `temp/testing/release-candidate/db7bd448acbd/`: 소비 검증 PASS 후 readonly/보유 NuGet DLL 정리 실패.
- `temp/navigation-build.log`: 초기 컴파일 로그.
- `Doroti/artifacts/release/`의 이전 버전 `0.3.0-beta.rc.20260929000428`,
  `0.3.0-beta.rc.20260929000744`, `0.3.0-beta.rc.20260929001602`,
  `0.3.0-beta.rc.20260929002556`, `0.3.0-beta.rc.20260929004147`,
  `0.3.0-beta.rc.20260929005641`. 마지막 두 후보는 이후 수정으로 대체되어 남겨 두었다.

사용자 직접 실행용 [cleanup-plan-all.ps1](../../../cleanup-plan-all.ps1)을 저장소 루트에 두었다.
기본 실행은 대상 미리보기이며, `pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Execute`로
위 목록만 삭제한다. 스크립트는 아직 삭제 모드로 실행하지 않았다. 없는 경로는 건너뛰고
링크 경로는 거절하며, 잠긴 파일의 실패를 보고한다. 다른 프로세스를 강제 종료하지 않는다.
최종 후보와 제품 소스는 이 보류 삭제 목록에 포함하지 않는다.

## 2026-09-29 00~10 보강 검토

기준 revision `c858ca9add07f20634c13d9e6177710811c1fc93` + 이번 미커밋 변경.
11개 공통 문서의 완료 기준·기록과 검증기·주요 구현을 대조했다. 기존 Developer suite가
통과하는 상태에서도 아래 종료·복원·설치 경계 조건의 누락이 발견됐다.

| 문서 | 검토 및 이번 보강 | 여전히 남은 범위 |
| --- | --- | --- |
| 00 | Source가 `plan.md` 링크도 검사. CI에 Packages job 및 plan/SDK pin 변경 trigger 추가. 설치 계약을 Source에 연결 | 원격 CI 실행, clean OS smoke |
| 01 | WidgetTester의 key/text/pointer/semantics/reassemble/이미지 API가 작업 전에 owner thread·disposed 상태를 검사하도록 수정 | GPU golden, 전체 native 자원 격리 |
| 02 | 기존 native 창 계약을 유지하면서 Explicit 종료 요청의 중복 알림 수정 | live resize 픽셀·mixed-monitor·다른 adapter |
| 03 | 기존 합성 한글·native view 수명 회귀 범위 확인. 입력 API의 잘못된 스레드 접근도 차단 | 물리 IME·Tab/focus·UIA와 전체 PlatformView 조합 |
| 04 | CPU DPR/blur geometry/list/dialog/layer 및 Web admission 회귀 재실행 | GPU 품질·present/VRAM·device loss·실기기 장기 실행 |
| 05 | 확장 단위 테스트 8개 및 동시 dispatcher reassemble 회귀 재실행 | 설치 VSIX의 실제 editor/metadata 흐름 재검증, 미완료 UI 예외 |
| 06 | plugin 취소·오류·view/app 수명 회귀 재실행 | event stream, Web/Android 및 다른 native adapter |
| 07 | OS drop 협상·queue·payload 수명 회귀 재실행 | Explorer 교차 창·송신·move/link·다른 플랫폼 |
| 08 | 손상 checkpoint의 cleanShutdown flag도 폐기. 잘못된 data 타입 거절. foreign/malformed history state는 무시하고 URL 전달. bfcache 복귀 시 running 표시 복구·listener 해제 | 실제 브라우저 back/forward·새로고침, iOS/다른 desktop 연결 |
| 09 | 직렬화된 Explicit 종료 요청이 완료 결과를 공유하고 ExitRequested를 한 번만 통지 | 물리 IME·DPI·전체 native content, AppKit/Qt/Catalyst 추가 창 |
| 10 | 설치를 staging→검증→version 승격으로 변경. 복사 실패 정리·동일 버전 재시도, 기존 설치의 추가 파일 거절, userdata 경로 검사 | 서명·clean VM·실제 배포 업데이트·장기 soak·Android package-only |

환경: Windows 10.0.26200 / .NET SDK 10.0.400 / Debug JIT / CPU 회귀와 Node 테스트.
검증 명령은 `python Doroti/eng/run-with-timeout.py` 또는 이를 호출하는 `validate.ps1`을
사용해 1,200초로 제한했다. 모두 로컬 검증이며 원격 CI·물리 입력의 통과를 뜻하지 않는다.

- 수정 전 새 회귀에서 Explicit 종료 이벤트가 1회 대신 **3회** 발생해 실패했다.
- 수정 후 `validate.ps1 -Suite Developer` **PASS**: 기존 widget/rendering/navigation/window,
  plugin·drop·Web 회귀와 잘못된 thread/disposed API 접근·손상 checkpoint·중복 종료 검증.
- `validate.ps1 -Suite Source` **PASS**: 링크·실패 전파·timeout 및 installer 계약.
  작은 payload fixture의 두 번째 Copy-Item에서 실패를 주입해 current 유지·staging/실패 version
  부재·재시도 성공을 확인했다. 변조/추가 DLL 거절과 update/remove 후 한글 userdata 보존도 확인했다.
- `validate.ps1 -Suite Packages` **PASS**: 격리 NuGet cache의 Testing/Cupertino/Desktop 소비 앱.
  이 suite는 의도적으로 repo 전용 회귀를 제외하며 전체 회귀는 Developer가 담당한다.
- `npm.cmd test --prefix Doroti/tools/vscode-doroti` **8/8 PASS**. 설치 VSIX 실행 증거로 확대하지 않는다.
- `validate.ps1 -Suite Targets` **PASS**: Windows App SDK/Web Debug build와 Web HTTP bootstrap smoke.
- `validate.ps1 -Suite WindowsSmoke` **PASS**: API/native close 취소·정리, editor/WebView 각
  생성·재생성, 실제 두 HWND/editor island의 독립 크기·survivor resize·종료와
  OnLastWindowClosed/Explicit 수명. 기본 Windows GPU runner의 native 상태/실행 증거이며
  화면 픽셀·물리 입력·mixed-monitor DPI 검증은 아니다.

성공한 aggregate 실행 폴더는 자동 정리했다. 이번에 새 Release 후보는 만들지 않았으므로
위의 기존 후보 버전에 이번 수정이 포함됐다고 해석하지 않는다. 기존 세션의 정리 보류 목록은
이번 보강과 별개이며 그대로 보존한다.

이번 수정 전 실패 재현 폴더 `temp/testing/build/fd3416cf43254cd09d5a3afc72173251/`의
삭제는 절대 경로/소유 범위/link 검사와 함께 요청했지만 자동 승인 검토에서
`blocked by policy`로 거절됐다. 구체적인 이유는 제공되지 않았다. 우회 삭제하지 않았으며
결함은 수정 후 PASS지만 이 원시 자료는 정리 보류다. 삭제가 허용되는 사용자/후속 세션에서
이 경로만 확인·정리한다. 제품 입력이나 CI에 포함되지 않는다.

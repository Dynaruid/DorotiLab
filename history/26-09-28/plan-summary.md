# Doroti 작업 진행 순서 요약 — plan.md

원문 기준일: **2026-09-28** · 실행·문서 갱신: **2026-09-29** · 보관일: **2026-10-03**

루트 `plan.md`의 진행 순서·착수 조건·초기 검토를 요약했다. **M0 로컬 기반 완료, M1~M7 PARTIAL**이라는 원문의 판정을 보존한다. 이 보관 작업에서 제품 실행·물리 입력·성능·배포를 새로 검증하지 않았다. 당시 상세 체크리스트와 실행 상태는 [works 보관 인덱스](../26-10-03/works/README.md)에 남겼으며, 이후 지원 현황은 [지원표](../../Doroti/docs/support-status.md)를 따른다.

## 1. 전체 진행 순서와 실행 상태

기본 흐름은 **공통 기반 → Windows 기본 동작 → Windows/Web 개발 도구 → 앱 기능 → 실제 추가 창 → 출시 검증**이었다. 다음 단계 착수 조건은 기능 전체의 완료 조건과 다르며, 이미 확보한 구현을 반복하지 않고 잔여 항목부터 진행한다.

| 순서 | 단계·문서 | 주요 범위와 다음 작업 착수 조건 |
| --- | --- | --- |
| 1 | [00 기반 / M0](../26-10-03/works/common/00-foundation.md) | CLI·실패 전파·timeout·임시 경로·지원표·smoke. 최소 검증 경로와 의도적 실패 반환 확보 |
| 2 | [01 테스트 / M1](../26-10-03/works/common/01-testing.md) | WidgetTester·clock·pointer·bounded settle·종료 정리. 실제 입력 경로의 Cupertino 탭 회귀 확보 |
| 3 | [02 Desktop / M2-A](../26-10-03/works/common/02-desktop-contract.md) | Windows App SDK 창·companion·크기·제목·close 취소·resize |
| 4 | [03 입력·접근성 / M2-B](../26-10-03/works/common/03-input-accessibility-platformview.md) | 한글 조합·TextField/native focus·PlatformView 수명. 물리 입력·보조기술은 별도 추적 |
| 5 | [04 렌더링 / M2-C](../26-10-03/works/common/04-rendering-lifetime.md) | Windows/Web 대표 장면·geometry·실제 표시·성능/수명 baseline |
| 6 | [05 VS Code / M3](../26-10-03/works/common/05-vscode-hot-reload.md) | A 설계 → B 생성/실행 → D snippet/import → C Hot Reload → 설치 VSIX 통합 검증 |
| 7 | [06 플러그인 / M4](../26-10-03/works/common/06-plugin-sdk.md) | 기존 SDK 확장·FilePicker/URL·패키지 소비·취소·오류·수명 |
| 8 | [07 OS 드롭 / M5-A](../26-10-03/works/common/07-os-drag-drop.md) | Windows 파일/텍스트/URI 수신부터 송신·다른 adapter로 확장 |
| 9 | [08 Navigation / M5-B](../26-10-03/works/common/08-navigation-restoration.md) | activation·Router·lifecycle·cold/warm 링크·상태 복원 |
| 10 | [09 멀티윈도우 / M6](../26-10-03/works/common/09-multiwindow.md) | 창 문맥 ADR → 실제 OS 창 두 개의 독립 입력·렌더링·종료 |
| 11 | [10 출시 / M7](../26-10-03/works/common/10-release-packaging.md) | 선택한 출시 조합의 package-only 소비·서명·clean 설치·업데이트·장기 실행 |

2026-09-29에는 00~10 전체 실행으로 기반 회귀를 재검증하고 Navigation, 실제 Windows 추가 창, 선택 플랫폼 후보 패키지를 구현·범위별 검증했다. 후속 보강에서 테스트 소유권·반복 종료·복원 실패·브라우저 history·설치 재시도·CI 패키지 검증을 보완했다. **전체 완료는 아니며**, 물리 입력·화면·다른 adapter·출시 서명/clean 설치는 남았다. 이후 보강을 기존 release 후보의 검증으로 소급하지 않는다. 당시 잔여 목록은 [work2 요약](../26-09-29/work2-summary.md)에 보관했다.

## 2. 초기 정적 검토와 선행 과제

`ref1.md` 재검토에서는 기존 플러그인 manifest/handler/ABI·SDK 등록, Desktop manager/controller, Router/restoration, Inspector/service extension/reassemble/Preview 타입을 재사용하도록 했다. 별도 Window API나 플러그인 추상화를 중복 신설하기보다 실제 호스트·외부 패키지·개발 도구 연결을 우선했다.

당시 OS 드롭 공통 연결, 제품 WidgetTester, 완성된 metadata Hot Reload/VS Code 경로, activation/history 연결은 소스 검색에서 확인하지 못했다. 이는 완전한 부재의 증명이 아니며, 이후 구현·범위별 실행 결과와 구분한다.

초기 선행 과제는 끊어진 CLI validate/audit/release 호출, 삭제된 validation 문서 링크, Windows App SDK Desktop companion 허용 조건(`DOROTIDESKTOP005`), 서로 맞지 않는 Desktop 지원 설명, CI 부재와 PlatformView 품질 제한이었다. 이 목록은 당시 검토 이력이다. 후속 CI workflow 삭제 지시는 [work2 요약](../26-09-29/work2-summary.md)에 별도로 보존한다.

## 3. 단계별 범위와 의존 조건

- **M0:** 실행기·실패 전파·20분 timeout·`temp/testing/` 제외·지원표·최소 smoke를 먼저 확보한다. 삭제된 `Doroti/validation` 전체 복구는 요구하지 않았다.
- **M1:** 최소 deterministic runtime과 실제 pointer 회귀부터 확보하고, ListView·IME·Dialog·blur·PlatformView·golden·패키지 소비는 관련 단계에서 확장한다.
- **M2-A/B/C:** Windows 기본 창 → 입력·합성 → 렌더링/수명 순서다. Web과 Android는 같은 공통 장면으로 합류하며, 모든 장비·보조기술 검증 완료를 M3의 일괄 선행 조건으로 삼지 않는다.
- **M3:** 생성·target 선택·Debug 실행·로그/Stop, 작성 지원, metadata update → 소유 문맥 reassemble → 상태 보존을 통합한다. Preview/Inspector UI·별도 C# LSP·시각적 디자이너는 제외했다.
- **M4:** 기존 SDK 계약 위에 Windows 패키지 소비부터 검증하고 Web/Android·다른 native 호스트로 확장한다. 창/view 소유권은 M2/M6와 맞추되 실제 멀티윈도우 완료를 기다리지 않는다.
- **M5-A/B:** OS 드롭과 Navigation은 필수 선후 의존성이 없다. 데이터/stream 수명과 activation·복원 실패를 각각 검증한다.
- **M6:** M2-A와 M1 격리·수명 기반 위에 Windows App SDK → AppKit/Qt → Catalyst scene으로 확장한다. fake controller 두 개는 실제 두 창의 증거가 아니다. Web/Android/iOS 추가 창은 필수 범위에서 제외했다.
- **M7:** package-only smoke는 M0부터 유지한다. Windows → Web → Android를 우선하고 선택한 기능·플랫폼의 clean 설치·서명·업데이트·데이터 유지·장기 회귀를 완료한다. 출시하지 않을 선택 기능까지 모두 기다릴 필요는 없다.

## 4. 플랫폼 합류 원칙

Windows App SDK와 Web은 초기 개발·자동화 기준, Android 실기기는 초기 모바일 기준이다. AppKit·Qt·iOS·Catalyst는 공통 Desktop/입력/수명 계약과 장비·toolchain이 준비되는 순서로 연결한다. 공통 11개 문서를 모두 끝낸 뒤 플랫폼 작업을 시작하는 구조가 아니다.

Windows App SDK/MAUI, AppKit/Catalyst, Qt Quick/Widgets, 각 GPU renderer, simulator/실기기의 실행 결과는 서로 대체하지 않는다. 각 단계에서 해당 공통 문서와 플랫폼 문서의 관련 항목만 함께 진행한다.

## 5. 공통 완료·증거·정리 규칙

공개 계약·호스트 연결·실행 가능한 샘플·회귀 검증·지원표를 함께 완료한다. `PASS`는 실행 범위에만, 일부 성공은 `PARTIAL`, 미실행은 `notVerified`, 의도적인 제한은 `unsupported`로 기록한다. revision·toolchain·OS/기기·renderer·runtime·명령·기대/실제 결과와 증거 종류를 남긴다.

테스트는 **1,200초 timeout**, 일반 반복은 **30회 이내**로 설계한다. 일회성 검사와 raw는 `temp/testing/<작업-ID>/<실행-ID>/`에 두고 기본 build/watch/pack/CI에서 제외한다. 최소 상시 회귀만 `Doroti/tests/`에 남긴다. `temp/testing/`과 `Doroti/artifacts/`의 raw 존재는 보장하지 않으며, 삭제 후에도 재현 절차와 추적되는 결과 요약을 근거로 삼는다.

## 6. 주요 근거와 후속 기록

- [전체 실행·보강 기록](../26-10-03/works/README.md#2026-09-29-전체-실행-상태), [미완료 검토 요약](../26-09-29/work2-summary.md)
- [CLI](../../Doroti/eng/doroti.ps1), [제품 솔루션](../../Doroti/Doroti.Product.slnx), [SDK 조건·코드 생성](../../Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)
- [플러그인·앱 경계](../../Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs), [Desktop 현황](../../Doroti/docs/desktop-windows.md), [PlatformView 지원표](../../Doroti/docs/platform-views/support-matrix.md)

`ref1.md`와 AvaloniaVSCode archive는 원문에서 로컬 참고 자료 또는 clean checkout에 없는 경로로 기록했다. 이번 보관으로 해당 자료를 복원하거나 과거 검토를 새 실행 결과로 승격하지 않았다.

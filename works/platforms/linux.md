# Linux / Qt 작업 계획

원문: [plan.md](../../plan.md) M0~M7 · 작업 상태: **TODO** · 새 실행 검증: **notVerified**

[전체 작업 인덱스](../README.md)

Qt Quick와 Qt Widgets, Wayland/XWayland 및 GPU/소프트웨어 렌더링 환경의 결과를 구분한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행·Desktop adapter — M0/M1/M2-A, P0 → P1

- [ ] Linux runner·Qt/native 의존성·display 환경별 실행 경로와 지원표를 정리하고 M1 회귀 fixture를 연결한다.
- [ ] 기존 Qt adapter의 크기/DPI·최소/최대 크기·show/hide/focus·제목/caption/appearance·close 취소/render drain을 Desktop 계약으로 정리한다.
- [ ] 첫 표시·live resize·최소화/복원·디스플레이 전환·pending GPU 작업 중 종료를 실제 표시와 자원 수명으로 검증하고 미지원 동작은 명시한다.

## 2. 입력·Orca·PlatformView·렌더링 — M2-B/C, P1

- [ ] 한글 조합·selection/caret·후보창·multiline·clipboard·단축키·focus loss와 Doroti 입력 ↔ Qt native 편집/WebView의 Tab/IME 소유권·키 중복을 검증한다.
- [ ] Orca의 semantics·action·focus·탐색 순서를 실제 환경에서 검증한다. VM 결과와 물리 환경 결과를 따로 기록한다.
- [ ] Qt Quick/Widgets 각각의 PlatformView 지원 범위를 확인하고 복수 owner·clip/transform/z-order·gesture 경쟁·재생성·지연 응답을 지원 조합에서 검증한다.
- [ ] 목표 GPU·해상도·display 환경별 baseline/예산을 정하고 리스트·VariableBlur·효과·WebView overlay·texture·resize/DPR의 UI/GPU/upload/present/메모리를 분리 측정한다.
- [ ] capture geometry·폰트 CDN/asset/offline·한글 fallback·pause/resume·context/device loss·자원 해제를 검증한다.

## 3. 앱 기능 — M4/M5, P2

- [ ] FilePicker·URL launcher의 Qt/native 구현·패키지 asset·등록·취소·파일 접근 수명·권한/오류를 검증한다.
- [ ] 파일 관리자의 파일/텍스트/URI drop부터 수신·좌표/action/MIME·큰/여러 파일·stream·취소를 연결한 뒤 송신·drag 이미지·창 간 move 책임을 확장한다.
- [ ] desktop activation 지원 범위를 정하고 cold/warm URI → Router, 대기·중복 처리, lifecycle 저장·재시작/강제 종료 후 복원·migration·fallback을 검증한다.

## 4. 추가 창 — M6, P2

- [ ] 공통 창 문맥 ADR에 맞춰 Qt 실제 추가 창을 구현하고 각 창의 입력·scheduler·Navigator·semantics·PlatformView·GPU surface 소유권을 연결한다.
- [ ] 실제 두 창의 독립 스크롤·IME·DPI·native content·렌더링·닫기, 마지막 창 정책과 닫힌 창으로 향하는 작업 종료를 검증한다.
- [ ] `_window_linux.cs` Satellite를 Desktop 소유권에 연결하거나 지원 경계를 문서화한다. 기본 추가 창 이후 owner/modal/satellite·popup/tooltip을 다룬다.

## 5. 배포 — M7, P3

- [ ] package-only 소비 앱의 clean restore/build/run/publish와 Qt/native library/font/plugin 배치를 검증하고 실제 지원 Release·trimming·NativeAOT 조합을 기록한다.
- [ ] 목표 배포 형식의 패키징·서명 적용 범위·설치·업데이트·제거·데이터 유지·crash/로그 및 장기 실행·자원 누수 회귀를 확인한다.

## 완료 기준

Qt Quick/Widgets와 display/GPU 환경별로 실제 창·입력·Orca·합성·배포 결과를 기록한다. 한 환경의 build나 VM 실행으로 전체 Linux 물리 검증을 완료 처리하지 않는다. Linux VS Code 확장 지원 검증은 M3 후속 후보로 둔다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Qt](../../Doroti/src/Doroti.Host.Qt)
- [Doroti/src/Doroti.Target.Linux.Qt.linux-x64](../../Doroti/src/Doroti.Target.Linux.Qt.linux-x64)
- [Doroti/docs/platform-views/linux-qt.md](../../Doroti/docs/platform-views/linux-qt.md)
- [Doroti/docs/platform-views/linux-webview.md](../../Doroti/docs/platform-views/linux-webview.md)
- [Doroti/src/Doroti.Framework.Widgets/_window_linux.cs](../../Doroti/src/Doroti.Framework.Widgets/_window_linux.cs)
- [samples/DorotiTestbedApp/linux](../../samples/DorotiTestbedApp/linux)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

현재 기록: 작업 계획만 작성했다. 플랫폼 구현·build·실기기·성능 검증을 새로 실행하지 않았다. 각 항목의 완료 시 관련 공통 작업 문서와 플랫폼/호스트/renderer/build mode별 지원표를 함께 갱신한다.

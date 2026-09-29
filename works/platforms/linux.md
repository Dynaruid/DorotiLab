# Linux / Qt 작업 계획

원문: [plan.md](../../plan.md) M0~M7 · 작업 상태: **PARTIAL** · 실행 검증: **범위별 PASS / 잔여 notVerified**

[전체 작업 인덱스](../README.md)

Qt Quick와 Qt Widgets, Wayland/XWayland 및 GPU/소프트웨어 렌더링 환경의 결과를 구분한다.

2026-09-29 구현·실행: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).
Ubuntu 26.04/Qt 6.10.2 VM에서 Quick/Graphite software Vulkan 및 Widgets/OpenGL을 실행했다.
물리 IME/Orca·물리 GPU·clean OS/서명 완료로 확대하지 않는다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행·Desktop adapter — M0/M1/M2-A, P0 → P1

- [x] Linux runner·Qt/native 의존성·display 경로, LinuxSmoke와 Linux CPU 회귀 연결. Source·CPU·Plugin·Drop 및 Quick 양 QPA 자동 회귀 PASS.
- [x] Qt Quick Desktop 계약·지원 거절 정합, 실제 크기/제약/show/hide/focus/제목/상태/close 취소·drain 자동 회귀 PASS. appearance 변경·global placement·Wayland minimize 등의 제한 유지.
- [ ] 첫 표시·live resize·최소화/복원·디스플레이 전환·pending GPU 작업 중 종료를 실제 표시와 자원 수명으로 검증하고 미지원 동작은 명시한다.

## 2. 입력·Orca·PlatformView·렌더링 — M2-B/C, P1

- [x] 공통 입력 샘플의 native editor 초기 text 지원 수정, Quick 두 창 editor와 editor/WebView 각각 재생성 PASS.
- [x] Widgets의 `qt-widgets` ClipRect 장면 native QLineEdit·합성 키·20회 resize 양 QPA PASS. 혼합 Quick 장면의 Widgets 거절을 별도 기록.
- [x] Quick 20회 resize의 frame ACK·reserved/retiring 자원 0, software Vulkan 시간 진단을 기록. physical present/VRAM 수치로 사용하지 않음.

- [ ] 한글 조합·selection/caret·후보창·multiline·clipboard·단축키·focus loss와 Doroti 입력 ↔ Qt native 편집/WebView의 Tab/IME 소유권·키 중복을 검증한다.
- [ ] Orca의 semantics·action·focus·탐색 순서를 실제 환경에서 검증한다. VM 결과와 물리 환경 결과를 따로 기록한다.
- [ ] Qt Quick/Widgets 각각의 PlatformView 지원 범위를 확인하고 복수 owner·clip/transform/z-order·gesture 경쟁·재생성·지연 응답을 지원 조합에서 검증한다.
- [ ] 목표 GPU·해상도·display 환경별 baseline/예산을 정하고 리스트·VariableBlur·효과·WebView overlay·texture·resize/DPR의 UI/GPU/upload/present/메모리를 분리 측정한다.
- [ ] capture geometry·폰트 CDN/asset/offline·한글 fallback·pause/resume·context/device loss·자원 해제를 검증한다.

## 3. 앱 기능 — M4/M5, P2

- [x] Qt FilePicker·native handler/package 등록·caller 취소·한글 bounded read·권한/특수파일 거절 구현 및 자동 실행 PASS.
- [x] URL scheme 검사 및 실제 plugin → xdg-open → loopback browser GET PASS.
- [x] Copy drop 수신·복수 파일/5GiB/한글/URI·등록 해제 후 grant 취소 PASS. 송신 API·PNG/Copy/Move/Link 협상 연결과 취소 PASS.
- [x] main-owned cold/warm IPC → Router, 정상 종료 및 committed checkpoint 후 강제 종료 복원·bad version fallback·cold 우선순위 PASS. 새 XDG 저장소 절대 경로 보장.

- [ ] 물리 FilePicker 선택·취소, desktop portal/권한 UI와 여러 창의 실제 권한/지연 응답을 검증한다. 구현·자동 취소·read grant·URL은 위 PASS 범위다.
- [ ] 파일 관리자에서 시작하는 실제 cross-app 수신, 송신 성공·drag 이미지·Move/Link·창 간 원본 처리 책임을 검증한다. virtual-file stream provider는 현재 unsupported이다.
- [ ] 실제 OS protocol association으로 시작한 cold/warm, 앱별 migration과 사용자 입력·selection 복원을 검증한다. IPC/route/lifecycle/정상·강제 종료/fallback 자동 경로는 PASS다.

## 4. 추가 창 — M6, P2

- [x] 한 QApplication의 실제 추가 창과 창별 dispatcher/session·Navigator·semantics·PlatformView factory·GPU surface·plugin scope 연결.
- [ ] 실제 두 창의 독립 스크롤·IME·DPI·native content·렌더링·닫기, 마지막 창 정책과 닫힌 창으로 향하는 작업 종료를 검증한다.
- [x] `_window_linux.cs` Satellite는 Desktop에 미연결임을 명시했다. owner/modal/satellite·popup/tooltip은 unsupported 경계로 남긴다.

- [x] 두 실제 창의 독립 크기·close 취소·survivor resize·main close 이후 추가 창, Explicit의 창 0개 후 reopen·exit 1회 PASS.

## 5. 배포 — M7, P3

- [x] 25개 패키지의 격리 cache template 소비·Release/JIT publish/run·native/font/license 배치 PASS. trimming 거절 검증.
- [x] Linux portable installer의 실행·업데이트·변조 거절·제거 후 userdata 보존 PASS. 후보 `0.3.0-beta.qt.20260929` 보존.
- [ ] clean OS/VM, 실제 서명·배포·전역 protocol association 및 장기 soak는 남는다.

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

현재 기록: [2026-09-29 실행 결과](../results/2026-09-29-linux-qt.md). 위 미완료 항목은 구현된 부분을 재작업하는 뜻이 아니라 물리 입력·외부 source·전체 합성·성능·출시 qualification 잔여다. 성공한 raw 자료는 결과 요약 후 정리하며 최종 후보만 artifacts에 보존한다.

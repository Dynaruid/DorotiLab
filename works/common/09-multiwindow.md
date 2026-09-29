# M6. 창 문맥과 실제 멀티윈도우

원문: [개발 로드맵](../../plan.md) §3 M6 · 우선순위: **P2** · 작업 상태: **PARTIAL — Windows 실제 추가 창 구현** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

binding/view 모델과 창별 소유권·공유 cache 경계를 정한다. Windows App SDK → AppKit/Qt → Catalyst scene 순으로 실제 창 연결을 추적한다.

선행: [M2-A](02-desktop-contract.md)와 [M1](01-testing.md)의 격리·수명 테스트. 구현 전 창 문맥 ADR을 작성한다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [macOS / AppKit](../platforms/macos.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** M2-A, M1의 격리·수명 테스트. 구현 전에 창 문맥 설계를 짧은 ADR로 고정한다.

- [x] `WidgetsBinding`, `SchedulerBinding`, `ServicesBinding`의 static 상태와 root/view 구조를 조사해, 하나의 binding 아래 여러 view를 둘지 독립 실행 문맥이 필요한지 결정한다. 무조건 singleton을 전부 분리하지 않는다.
- [x] 창별 focus, pointer/keyboard, scheduler target, Navigator/restoration, semantics, PlatformView owner, texture/GPU surface 소유권을 정의한다. 공유 가능한 cache와 창 종료 시 해제할 자원을 구분한다.
- [x] 기존 `DorotiWindowManager.CreateWindowAsync`와 fresh `WindowContent` factory를 실제 Windows App SDK 추가 창 생성에 연결한다.
- [ ] 두 창의 독립 갱신·DPI·IME·렌더링·닫기와 `OnLastWindowClosed`/`Explicit` 수명을 검증한다. 닫힌 창으로 향하는 작업은 안전하게 종료한다.
- [ ] AppKit와 Qt에 확장하고 Catalyst는 scene 계약에 맞춰 별도 구현한다. owner/modal/satellite, popup/tooltip과 창 간 이동은 기본 추가 창 이후에 붙인다.
- [ ] `_window_*`의 Satellite API는 Desktop 소유권으로 연결하거나 지원 경계를 문서화한다. 두 구현이 같은 native 창을 중복 소유하지 않게 한다.

**완료 기준:** 실제 OS 창 두 개에서 서로 다른 입력·스크롤·DPI·native content가 동작하고, 한 창을 닫아도 다른 창이 유지된다. fake host 두 controller 생성은 이 단계 완료 근거가 아니다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-29 구현·검증

후속 보강: 반복/동시 `RequestExitAsync`가 Explicit 종료 이벤트를 3회 발생시키던 회귀를
재현하고 완료 결과를 공유하도록 수정했다. 수정 후 1회 통지 회귀와 NuGet 소비 PASS.
실제 창/물리 입력의 증거 경계는 [보강 검토](../README.md#2026-09-29-0010-보강-검토)를 따른다.

Revision: `8834d7596597b3087a0139f527148baee9a46583` + 이번 작업 트리.
[창 문맥 ADR](../../Doroti/docs/desktop-window-context.md)을 작성한 뒤 구현했다.

- native HWND마다 STA UI thread·host/session/dispatcher·render worker를 소유한다.
  기존 manager/controller/content factory를 재사용하며 새 Window API를 추가하지 않았다.
- binding·IME/keyboard·asset cache·context menu·inspector·semantics registry·기본 action의
  mutable 상태를 dispatcher에 귀속했다. 상수/타입 정의는 공유한다. 두 창 동시 종료 회귀에서
  발견한 공유 action listener 목록 경쟁도 수정했다.
- `RequestExitAsync`와 `OnLastWindowClosed`/`Explicit` 종료, 추가 생성 차단,
  native loop·startup callback 종료 대기를 연결했다. 첫 창이 닫혀도 다른 native loop가 유지된다.
- 두 CPU dispatcher에서 동시에 실제 pointer focus·서로 다른 한글 값·semantics action·unmount
  **PASS**. `WidgetTester`는 각 owner thread에서 사용하며 같은 thread의 nested 사용은 계속 거절한다.
- Windows 10.0.26200 / Windows App SDK / 기본 Vulkan / Debug JIT:
  실제 창 2개 ready/present, 470×650 및 580×620 DIP, 첫 창 종료, survivor 600×640 resize,
  마지막 registry 0, 프로세스 exit 0 **PASS**. OnLastWindowClosed와 Explicit 각각 확인했다.
- 별도 NuGet-only Release 소비 앱에서도 실제 추가 창·ready·resize·종료 **PASS**.
- application boundary를 창마다 새 handler owner로 만들지 않고 앱 서비스 lease를 공유한다.
  첫 창 종료 후 다른 창의 handler 호출이 유지되고 마지막 owner 뒤 Dispose 1회인 회귀 **PASS**.
  native view factory/coordinator와 요청 context는 각 창이 따로 소유한다.

재현: 기본 Windows build 후 `DOROTI_SAMPLE=reload`, `DOROTI_DESKTOP_SAMPLE=solid`,
`DOROTI_MULTIWINDOW_PROBE=<temp/testing/실행/결과.json>`으로 runner를 실행한다.
Explicit은 `DOROTI_DESKTOP_LIFETIME=Explicit`을 추가한다. 실행기는 1,200초 timeout을 사용한다.
수동으로 두 창을 유지하려면 probe 대신 `DOROTI_MULTIWINDOW_SAMPLE=1`을 지정한다.
`DOROTI_SAMPLE=input`과 함께 실행하면 두 창의 framework/native 편집기를 비교할 수 있다.

미검증/미완료: 물리 IME·mixed-monitor DPI·실제 화면 품질·창 간 이동,
AppKit/Qt 추가 창·Catalyst scene, owner/modal/satellite. native 화면 도구는 pipe 연결 실패였다.
동시 native editor 장면에서는 서로 다른 STA에 WinUI TextBox island 2개 생성,
각 창 종료 후 `shutdown-islands=0` 2회, framework 오류 없음과 exit 0을 확인했다.
실제 화면·물리 focus/IME·WebView·여러 DPI의 전체 PlatformView 완료로 확대하지 않는다.
이 과정에서 드러난 Navigator route-name 보고 결함은 08과 함께 수정했다.

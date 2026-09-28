# M6. 창 문맥과 실제 멀티윈도우

원문: [개발 로드맵](../../plan.md) §3 M6 · 우선순위: **P2** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

binding/view 모델과 창별 소유권·공유 cache 경계를 정한다. Windows App SDK → AppKit/Qt → Catalyst scene 순으로 실제 창 연결을 추적한다.

선행: [M2-A](02-desktop-contract.md)와 [M1](01-testing.md)의 격리·수명 테스트. 구현 전 창 문맥 ADR을 작성한다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [macOS / AppKit](../platforms/macos.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** M2-A, M1의 격리·수명 테스트. 구현 전에 창 문맥 설계를 짧은 ADR로 고정한다.

- [ ] `WidgetsBinding`, `SchedulerBinding`, `ServicesBinding`의 static 상태와 root/view 구조를 조사해, 하나의 binding 아래 여러 view를 둘지 독립 실행 문맥이 필요한지 결정한다. 무조건 singleton을 전부 분리하지 않는다.
- [ ] 창별 focus, pointer/keyboard, scheduler target, Navigator/restoration, semantics, PlatformView owner, texture/GPU surface 소유권을 정의한다. 공유 가능한 cache와 창 종료 시 해제할 자원을 구분한다.
- [ ] 기존 `DorotiWindowManager.CreateWindowAsync`와 fresh `WindowContent` factory를 실제 Windows App SDK 추가 창 생성에 연결한다.
- [ ] 두 창의 독립 갱신·DPI·IME·렌더링·닫기와 `OnLastWindowClosed`/`Explicit` 수명을 검증한다. 닫힌 창으로 향하는 작업은 안전하게 종료한다.
- [ ] AppKit와 Qt에 확장하고 Catalyst는 scene 계약에 맞춰 별도 구현한다. owner/modal/satellite, popup/tooltip과 창 간 이동은 기본 추가 창 이후에 붙인다.
- [ ] `_window_*`의 Satellite API는 Desktop 소유권으로 연결하거나 지원 경계를 문서화한다. 두 구현이 같은 native 창을 중복 소유하지 않게 한다.

**완료 기준:** 실제 OS 창 두 개에서 서로 다른 입력·스크롤·DPI·native content가 동작하고, 한 창을 닫아도 다른 창이 유지된다. fake host 두 controller 생성은 이 단계 완료 근거가 아니다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.

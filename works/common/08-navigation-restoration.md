# M5-B. Deep Link·lifecycle·상태 복원

원문: [개발 로드맵](../../plan.md) §3 M5-B · 우선순위: **P2** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

activation 대기·중복 처리, Router, 상태 형식·migration·복원 실패 정책을 정의한다. URL·intent·OS activation 연결은 각 플랫폼에서 검증한다.

선행: [M1](01-testing.md)·M2. [M5-A](07-os-drag-drop.md)와 독립적으로 진행할 수 있다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [ ] 공통 activation event에 URI·출처·cold/warm start를 전달하고 Router 준비 전 들어온 요청의 대기·중복 처리 규칙을 정한다.
- [ ] Web URL ↔ Router와 back/forward/새로고침, Android intent, iOS Universal Link, Windows protocol activation을 순차 연결한다. 나머지 desktop activation도 지원표로 추적한다.
- [ ] 기존 lifecycle 이벤트를 활용해 비활성화/복귀/종료 시 상태 저장 시점을 정한다. 강제 종료 후 복원과 정상 재시작을 구분한다.
- [ ] route·사용자 입력·선택 상태의 저장 형식과 version migration을 정하고, 잘못된 링크·오래된 상태·복원 실패의 fallback을 제공한다.

**완료 기준:** 종료된 앱과 실행 중 앱에 같은 링크를 전달해 목적 화면으로 이동한다. Web back/forward와 새로고침, 프로세스 재시작 후 상태 복원이 서로 충돌하지 않는다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.

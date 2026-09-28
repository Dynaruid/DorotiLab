# M2-A. Desktop 계약과 호스트 연결

원문: [개발 로드맵](../../plan.md) §3 M2-A · 우선순위: **P1** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

기존 Desktop API의 계약·capability·종료 수명을 통합한다. 원문의 Windows 우선 연결은 Windows 작업에서 실행하고 다른 adapter는 각 플랫폼에서 검증한다.

선행: [M1](01-testing.md)의 최소 회귀 경로.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [macOS / AppKit](../platforms/macos.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [ ] Windows App SDK용 `IWindowHostFactory`/`IWindowHost`를 구현하고 SDK Desktop companion 허용 조건·템플릿·샘플을 함께 연결한다.
- [ ] 크기/DPI/최소·최대 크기, show/hide/focus, 제목·native caption·appearance, close 취소·render drain을 기존 Desktop 계약으로 통합한다.
- [ ] 첫 표시, 창 resize, 최소화/복원, 디스플레이 전환, 종료 중 pending GPU 작업을 실제 화면과 리소스 수명으로 검증한다.
- [ ] AppKit/Catalyst/Qt/Windows MAUI의 기존 adapter와 지원표를 맞춘다. 플랫폼이 제공하지 않는 동작은 capability와 명확한 거절 결과로 표현한다.

**완료 기준:** 기본 Windows 명령으로 실행한 샘플이 Desktop companion을 사용하며, native 창 닫기와 API 닫기가 동일한 취소·정리 경로를 따른다. live resize는 실제 표시 프레임의 geometry·깜빡임으로 판정한다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.

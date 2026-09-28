# M5-A. OS Drag & Drop

원문: [개발 로드맵](../../plan.md) §3 M5-A · 우선순위: **P2** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

이벤트·좌표·action·stream·접근 수명을 정의한다. Windows 수신부터 시작하고 호스트가 제공하는 범위를 capability로 구분한다.

선행: [M1](01-testing.md)·M2. 파일 데이터 계약은 [M4](06-plugin-sdk.md)와 공유한다. [M5-B](08-navigation-restoration.md)와 독립적으로 진행할 수 있다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [ ] enter/over/leave/drop, 허용 action(copy/move/link), MIME/type, 취소, 논리 좌표 변환을 공통 계약으로 정의한다.
- [ ] Windows 파일/텍스트/URI 수신을 첫 장면으로 구현하고 AppKit/Qt/Web으로 확장한다. Web과 모바일의 제공 범위는 별도 capability로 표현한다.
- [ ] 전체 파일을 즉시 메모리에 올리지 않도록 stream/비동기 읽기와 접근 수명을 정의한다. 드롭된 경로나 데이터만으로 파일 실행을 수행하지 않는다.
- [ ] 이후 Doroti → OS 송신, drag 이미지, 창 밖 취소와 여러 창 사이 이동을 지원한다. move 성공·취소에 따른 원본 처리 책임을 명확히 한다.

**완료 기준:** Explorer/Finder/파일 관리자 또는 브라우저 파일 드롭으로 실제 파일을 전달할 수 있다. 여러 파일·큰 파일·취소·DPI 변경에서 내용과 좌표가 보존된다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.

# M4. 플러그인 SDK와 대표 네이티브 기능

원문: [개발 로드맵](../../plan.md) §3 M4 · 우선순위: **P2** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

기존 manifest/handler/SDK 등록을 확장하고 FilePicker·URL launcher의 공통 패키지·수명·오류 계약을 담당한다. OS/Web 구현은 각 플랫폼 작업이다.

선행: [M0](00-foundation.md)·[M1](01-testing.md). 창·view 수명은 [M2-A](02-desktop-contract.md)·[M6](09-multiwindow.md)와 맞춘다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** M0/M1, 창·view 문맥 계약은 M2/M6와 일치시킨다.

- [ ] 현재 manifest/handler/SDK 등록 구조를 문서화하고 외부 NuGet이 target별 구현·native asset·Web module을 제공하는 최소 패키지 형식을 정한다.
- [ ] 앱 단위와 view/window 단위 플러그인 수명, UI thread dispatch, cancellation, event stream, dispose, 오류·권한 거절·미지원 결과를 정의한다.
- [ ] RID/ABI 불일치, 중복 id/channel, 누락된 handler/asset을 build 또는 초기화 단계에서 구체적으로 진단한다.
- [ ] 등록 생성은 기존 SDK 경로를 확장한다. Source Generator 도입은 reflection 회피·사용성·AOT 이득이 분명할 때 결정한다.
- [ ] 첫 대표 기능은 FilePicker와 URL launcher로 잡는다. 선택 취소와 외부 파일 접근 수명까지 포함한다. 이후 camera/notification/storage는 요구와 플랫폼 검증 장비에 맞춰 추가한다.
- [ ] 네이티브/Web 구현을 같은 C# 앱에서 사용하고, 미지원 플랫폼은 capability 확인과 예측 가능한 오류를 제공한다.
- [ ] 저장소 project reference 없이 패키지만 설치한 소비 앱에서 restore/build/run/publish, trimming과 목표 AOT 모드를 검증한다.

**완료 기준:** 별도 앱과 별도 플러그인 패키지에서 코드 복사 없이 기능을 사용할 수 있다. 권한 거절·취소·창 종료 중 응답·ABI 불일치가 검증된다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.

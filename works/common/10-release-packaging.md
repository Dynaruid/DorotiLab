# M7. 외부 패키지 소비·배포·출시 기준

원문: [개발 로드맵](../../plan.md) §3 M7 · 우선순위: **P3** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

호환 정책·migration·clean 소비 앱·지원표·출시 증거 형식을 담당한다. 서명·설치·업데이트·AOT는 플랫폼별 조합을 검증한다.

선행: 출시 대상 기능의 선행 단계. package-only smoke는 [M0](00-foundation.md)부터 시작하며 모든 선택 기능 완료를 첫 출시의 필수 조건으로 삼지 않는다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** 출시 대상 기능의 앞선 단계 완료. 모든 선택 기능 완료가 첫 배포의 필수 조건은 아니다.

- [ ] `dotnet new doroti-app`에서 package-only 앱을 만들고 Windows/Web/Android부터 clean 환경 설치·실행을 검증한다. 나머지 host를 순차 확대한다.
- [ ] SDK/runner/native library/font/Web asset/플러그인 버전의 호환 정책, API 변경 안내와 migration 문서를 정한다.
- [ ] Release, trimming, Mono AOT/NativeAOT/Web publish 중 실제 지원하는 조합을 명시하고 각 조합을 검증한다.
- [ ] 플랫폼별 서명·패키징·설치·업데이트·제거와 앱 데이터 유지, crash/로그 수집을 검증한다. 개발자 머신 성공을 clean 배포 성공으로 간주하지 않는다.
- [ ] 리소스 누수·장기 스크롤·창/PlatformView 반복 생성·GPU loss/앱 복귀 회귀를 릴리스 후보에서 확인한다.
- [ ] 제한과 미검증 항목이 포함된 출시 지원표, 실행 가능한 샘플, 문제 보고 양식을 함께 제공한다.

**완료 기준:** 선택한 플랫폼에서 저장소 소스와 개발자 로컬 cache 없이 앱을 설치·실행·업데이트할 수 있다. 배포 패키지와 검증 결과가 같은 revision·toolchain을 가리킨다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.

# M5-B. Deep Link·lifecycle·상태 복원

원문: [개발 로드맵](../../plan.md) §3 M5-B · 우선순위: **P2** · 작업 상태: **PARTIAL — 공통·Windows·Web·Android 연결** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

activation 대기·중복 처리, Router, 상태 형식·migration·복원 실패 정책을 정의한다. URL·intent·OS activation 연결은 각 플랫폼에서 검증한다.

선행: [M1](01-testing.md)·M2. [M5-A](07-os-drag-drop.md)와 독립적으로 진행할 수 있다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [x] 공통 activation event에 URI·출처·cold/warm start를 전달하고 Router 준비 전 들어온 요청의 대기·중복 처리 규칙을 정한다.
- [x] Windows opt-in protocol 전달·Web main-owned history/sessionStorage·Android intent를 공통 Router에 연결했다. Windows cold/warm/restart와 Galaxy route 화면, 공통/Node 계약 회귀는 아래 범위에서 PASS다.
- [ ] 실제 브라우저 back/forward·새로고침·bfcache를 검증하고 Windows 설치 protocol 등록, iOS Universal Link·AppKit/Qt/Catalyst·Web worker-owned framework 연결을 마무리한다.
- [x] checkpoint v1·크기 상한·원자적 파일 교체·lifecycle flush·정상 종료 표시·cold 링크 우선순위·손상/미지원 버전 fallback을 구현하고 공통 회귀를 통과했다.
- [ ] 앱별 schema migration과 route·사용자 입력·선택 상태의 정상 재시작/강제 종료 후 복원을 전체 목표 호스트에서 검증한다. 미지원 버전의 빈 상태 fallback은 데이터 migration 구현과 구분한다.
- [ ] 최종 공통 수명 수정 후 Android APK를 재설치해 cold/warm·복원을 재검증한다. 해당 APK는 재빌드만 통과했으며 앞선 기기 실행과 구분한다.

**완료 기준:** 종료된 앱과 실행 중 앱에 같은 링크를 전달해 목적 화면으로 이동한다. Web back/forward와 새로고침, 프로세스 재시작 후 상태 복원이 서로 충돌하지 않는다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-29 구현·검증

후속 보강: 잘못된 restoration data 타입·base64는 cleanShutdown까지 포함해 checkpoint 전체를
폐기한다. 브라우저의 외부/손상 history state는 null로 낮춰 URL 전달을 유지하며, bfcache 복귀 때
running 표시를 복구하고 dispose 때 pageshow listener도 해제한다. 공통/Node 회귀 PASS이며
실제 browser history 검증으로 확대하지 않는다. [보강 검토](../README.md#2026-09-29-0010-보강-검토).

Revision: `8834d7596597b3087a0139f527148baee9a46583` + 이번 작업 트리.
공개 계약과 재현 절차: [application-navigation.md](../../Doroti/docs/application-navigation.md).
모든 테스트 실행은 `Doroti/eng/run-with-timeout.py`의 1,200초 제한을 사용했다.

- `ApplicationNavigationHost`와 opt-in `DorotiViewConfiguration.Navigation`: URI/source/cold-warm,
  32건 준비 대기, 128개 delivery ID 중복 억제, 단일 Router 구독/해제, 종료 후 전달 거절.
- 기존 `PlatformRouteInformationProvider`·`SystemNavigator`·`RestorationManager`에 연결했다.
  동기 restoration 응답에서 completer가 먼저 비워지는 예외와 Navigator가 route name 대신
  `RouteSettings.ToString()`을 보내던 결함을 수정했다. named-route 판정·복원도 `.name`을 사용한다.
- checkpoint v1, 4 MiB 상한, 임시 파일+원자적 교체, 버전/손상 fallback, 명시적 cold 링크 우선,
  정상 종료 표시, lifecycle flush를 구현했다. 미지원 버전은 migration을 추측하지 않고 빈 상태로 복귀한다.
- 공통 회귀 **PASS**: 준비 전 순서/중복/반복 URI/back state, Router→host,
  실제 framework bucket의 한글 값 재생성, 손상·구버전·cold 링크 우선순위.
- Windows App SDK Debug: 실제 runner를 `doroti-testbed:/first`로 cold 실행하고 두 번째
  프로세스의 `/second` 전달 후 같은 Router의 build 결과가 바뀌었다. 강제 종료 후 링크 없는
  재실행에서 `/second` 복원 **PASS**. OS protocol registry 등록은 설치 단계에 남겨 두었다.
- Web: main-owned runtime에 history push/replace/popstate·sessionStorage·해제 연결.
  Node history/storage 계약 회귀와 Debug/패키지 Release publish는 **PASS**. 브라우저 도구의
  사용 가능 목록이 비어 실제 back/forward/새로고침 UI 검증은 **notVerified**다.
- Android: `DorotiMauiActivity`와 Testbed IntentFilter를 연결했다. Galaxy S25(SM-S931N),
  Android 16, Release/Mono AOT에서 ADB VIEW intent cold `/first`, warm `/second`,
  `am force-stop` 후 링크 없는 재시작 `/second`를 실제 화면으로 확인했다(**PASS**).
  OS intent와 실제 표시 증거이며 물리 터치·한글 IME 조합 검증은 아니다.
  후속 최종 공통 수명 수정 뒤 Android Release 재빌드는 통과했으나, 기기 연결이 끊겨
  그 최종 APK의 재설치는 수행하지 못했다. 앞선 실기기 실행과 구분한다.

남음: iOS Universal Link·AppKit/Qt/Catalyst native 연결, Web worker-owned framework,
실제 브라우저 history, 앱별 schema migration 사례, 사용자 입력/selection의 전체 호스트
재시작 실증. Windows 단일 인스턴스는 opt-in protocol 앱에 적용한다.
원시 증거는 `temp/testing/plan-all/`에서 생성했으며 최종 정리 상태는
[이번 실행 인덱스](../README.md#2026-09-29-전체-실행-상태)를 따른다.

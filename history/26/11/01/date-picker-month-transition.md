# Date picker 월 전환 지연: 공통 엔진 수정

2026-11-01. 공통 엔진 수정, 회귀 검사, Windows 실제 Testbed 자동 입력 비교와
Windows/Android Release 빌드를 완료했다. 사용자는 Windows 수정본의 이전·다음 달
전환이 "부드러워졌음"이라고 확인했다. 갤럭시 Date picker 실기기 성능은 아직 미확인이다.
아래 수치는 화면에 표시된 FPS가 아닌 UI owner의 처리 시간이다.

기준 HEAD는 `a3ce8df0f6afcb2f5ebbbcb9515e68dca757e71d`이다. 이전 Android/Windows 스크롤
수정이 남아 있는 작업 트리를 기준으로 비교했고 해당 변경을 유지했다. 루트 `work.md`는
[이전 요약](scroll-caret-resume-summary.md)으로 정리된 삭제 상태를 유지한다.

## Flutter 구성과 조사 결과

[Flutter 원본](https://github.com/flutter/flutter/blob/master/packages/flutter/lib/src/material/calendar_date_picker.dart)은
`CalendarDatePicker` 아래의 월 선택기를 `PageView.builder`로 구성하고, 이전·다음 달로
200ms 동안 이동한다. 모든 달을 미리 생성하는 구조는 아니다. 2026-11-01에 원본을 확인했다.
Doroti의 달별 지연 생성과 전환 시간도 이 구성을 유지한다.

월 전환에서 수십 ms의 UI 처리가 반복됐다. 초기 CPU 표본에서 빌드, 레이아웃,
페인트, 실행 컨텍스트 변경과 의존성 해제 경로가 확인됐다. 표본의 GC safepoint 위치를
정확한 함수별 CPU 비용으로 해석하지 않고, 실제 경로의 대조 검사와 Testbed 재측정으로 좁혔다.

- `ComponentElement.performRebuild`는 각 컴포넌트에서 뷰 호출 범위에 진입한다.
  이때 같은 뷰에서도 애플리케이션 콜백 범위와 뷰 콜백 범위를 차례로 만들면서
  콜백 객체와 `AsyncLocal` 맵을 반복 교체했다.
- `InheritedElement`의 의존성 저장소는 선형 `DartMap`이었다. 새 날짜 셀을 등록하거나
  지나간 달을 해제할 때 목록을 반복 검색하고 나머지 항목을 이동했다.
- 월 페이지 delegate를 보존하는 실험은 큰 지연을 줄이지 못해 제거했다.
  최종 변경에는 달력 위젯 캐시나 Material Date picker 전용 우회가 없다.

## 최종 변경

- [실행 컨텍스트](../../../../Doroti/src/Doroti.Runtime/DorotiExecutionContext.cs)는
  콜백 큐와 취소 토큰이 같은 중첩 호출, 동일 시간 공급자를 재사용한다.
- [뷰 호출 범위](../../../../Doroti/src/Doroti.Ui/PlatformDispatcher.cs)는 최종 뷰의 취소
  범위로 바로 진입한다. 다른 뷰, 애플리케이션 범위, 외부 스케줄러·시간·환경 override의
  전환과 복원은 유지한다. 플랫폼 환경도 같은 인스턴스이면 중복 범위를 만들지 않는다.
- [상속 의존성](../../../../Doroti/src/Doroti.Framework.Widgets/framework.cs)은 해시 검색과
  연결 목록을 사용한다. 등록·검색·해제는 목록 전체를 훑지 않으며, 갱신 시 알림 순서,
  제거 후 재등록 순서와 알림 중 변경에 대한 snapshot 동작을 유지한다.
- Windows 진단 보고서는 `DOROTI_STAGE_TRACE=1`일 때 애플리케이션 owner에서 기존
  `FrameworkWorkCounters`/`FrameworkWorkProfile`을 수집한다. 기본 실행에서는 끈다.
  세부 프로파일의 512-frame ring과 작업 counter의 8192-boundary ring은 유한하며,
  누적 타입별 수치를 개별 프레임 비용으로 확대 해석하지 않는다.

## Windows 비교

기본 Release 실행의 All Components에서 오른쪽 열을 스크롤하고 `Show date picker`를 열었다.
2026년 11월 → 12월 → 2027년 1월 → 12월 → 11월 → 10월 → 11월의 동일 순서를
기준본과 최종 수정본에 각각 입력했다. 2027년 1월의 다음 달 버튼과 범위 밖 날짜가
비활성화되는 것도 확인했다.

두 비교 실행 모두 CPU sampling과 세부 작업 프로파일을 끄고 기존 프레임 timing만 켰다.
각 창은 단일 월 전환과 포인터 이동을 포함하며, 그 창의 전체 timing sample이 recent ring에
남아 있는지 확인했다. 콜드 앱 시작과 목록 최초 로딩은 아래 월 전환 수치에서 제외했다.

| 전환 | 기준 최대 UI 처리 ms | 수정 최대 UI 처리 ms |
| --- | ---: | ---: |
| 11월 → 12월 | 96.055 | 33.173 |
| 12월 → 1월 | 55.247 | 20.447 |
| 1월 → 12월 | 63.527 | 19.130 |
| 12월 → 11월 | 73.772 | 12.502 |
| 11월 → 10월 | 81.834 | 13.684 |
| 10월 → 11월 | 83.605 | 15.116 |

전환별 최대치 평균은 75.673ms → 19.009ms로 **74.88% 감소**했다.
선택한 창에서 UI 처리만 16.667ms 이상인 sample은 19개 → 5개다.
최종 실행의 failed terminal과 operational debug error는 각각 0이었다.

첫 전환의 33ms, 이후 19~20ms 구간 및 일부 GC pause는 남아 있다. 이 비교는 입력·빌드·
레이아웃·페인트 등을 포함하는 UI owner wall time이며 GPU 작업이나 실제 display pacing을
검증한 결과가 아니다. 최종 세 번의 최대치가 16.667ms 아래였다는 사실도 전체 앱의
60/120Hz 성능 보증을 의미하지 않는다.

성능 비교와 별도 실행에서 2025년 연도 선택, 11월 2일 선택·확정, 재열기에서 선택 유지,
11월 3일 임시 선택·취소와 dark/light 테마 갱신을 확인했다. 취소 후 재열기는 샘플의
`_date = result` 동작에 따라 오늘 날짜로 초기화됐다. Material 위젯 정책 변경으로
해석하지 않는다. 수정본 달력은 Windows 사용자 확인용으로 열어 두었다.

후속 사용자 확인(2026-11-01): Windows 수정본에서 이전·다음 달 화살표를 여러 번 누른
체감에 대해 **"부드러워졌음"**이라고 답했다. Windows 월 전환은 사용자 체감 확인까지
완료로 기록한다. 이 확인은 계측한 display frame pacing이나 다른 플랫폼 성능 검증으로
확대하지 않는다.

## 회귀 검사와 빌드

- [호출 범위 검사](../../../../Doroti/tests/Doroti.Scroll.Tests/InvocationScopeRegression.cs):
  같은 뷰 20회 중첩의 할당 27,200 → 5,440바이트. 다른 뷰, 취소 토큰, 다른 콜백 큐,
  시간·환경 override, 예외, 비동기 컨텍스트 복원과 해제된 뷰 콜백 차단을 통과했다.
- [의존성 검사](../../../../Doroti/tests/Doroti.Scroll.Tests/InheritedDependencyRegression.cs):
  512개 의존성에서 20회 검색의 Element 비교 10,240 → 20회. aspect 갱신, 알림 순서,
  알림 중 등록·제거, 실제 `Element.deactivate`와 제거된 참조의 GC 회수를 통과했다.
- Scroll.Tests 전체 PASS 15개. 최종 Windows App SDK 및 Android arm64 Release 빌드 모두
  경고 0, 오류 0. Android Date picker 실행·실기기 체감은 이 작업에서 검증하지 않았다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal
```

[수치·선택 sample·소스/바이너리 식별](date-picker-month-transition-results.json)을 영속 기록으로 남겼다.
원시 보고서, CPU trace, 빌드 로그와 입력 시각은 로컬 `temp/date-picker-month-review/`에 있다.
로컬 산출물은 정리 가능하며, 이후에도 원시 파일이 보존된다고 가정하지 않는다.

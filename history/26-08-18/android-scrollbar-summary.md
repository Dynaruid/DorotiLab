# Doroti Android 스크롤바 투명도 및 중첩 소유권 수정 완료 요약

- 기록일: 2026-08-18
- 상태: **구현 및 자동 회귀 검증 완료, 사용자 수동 동작 확인 완료**
- 범위: `Doroti.Framework.Widgets`, Dart→C# lowerer 회귀 계약, FCR-5/FCR-7/FCR-8 evidence, `DorotiDemoApp`

## 해결한 문제

같은 축의 outer/inner Scrollable에서 inner를 스크롤하면 outer scrollbar가 inner의 viewport/content
메트릭을 받아 thumb 위치와 길이가 바뀌던 문제를 수정했다. 원인은
`ScrollMetricsNotification.asScrollUpdate()`가 전달한 `depth`를 생성된
`ScrollUpdateNotification` 생성자가 `_depth`에 저장하지 않아, 부모 scrollbar의 기본
`notificationPredicate`가 자식 알림을 로컬 depth 0 알림으로 오인한 것이었다.

현재 pinned Flutter 소스에서 `scroll_notification.cs`를 좁게 재생성해 nullable `depth`의 조건부
대입을 복구했다. 알림 bubbling은 유지하면서 각 scrollbar가 가장 가까운 Scrollable의 알림만
반영한다. 수동 산출물 패치에 머물지 않도록 일반 생성자 block, super-formal, mixin private field 대입을
포함한 Dart→C# lowerer fixture와 Debug/Release 재생성 계약도 함께 고정했다.

## Android thumb 투명도 판정

Android Material 기본 thumb의 peak alpha 255는 pinned Flutter 정책과 일치하므로 framework 기본색은
변경하지 않았다. 공통 painter의 600 ms hold와 300 ms fade, retained paint alpha 전달, 명시적인 theme
alpha를 자동 계약으로 확인했다. 데모는 `thumbVisibility: true`로 항상 보이게 하던 구성을 transient
scrollbar로 바꾸고 README의 설명도 동기화했다.

## 회귀 계약과 증거

- depth 0/1/2가 metrics→update 변환 전후에 보존되고 기본 predicate는 depth 0만 허용한다.
- inner start/update/end/metrics 동안 outer metrics, thumb rect, fade ownership은 유지되고 inner만 갱신된다.
- 이어서 outer를 조작할 때 outer만 갱신되고 inner 상태가 보존된다.
- 수정 전 reviewed 산출물의 depth 손실은 `validation/fcr5-scroll/expected-failure.json`에 고정했다.
- Flutter revision, 관련 source hash/anchor 및 재생성 선택 정보를 FCR manifest/evidence에 기록했다.

## 검증 결과

- Dart→C# lowerer focused fixture Debug/Release: `PASS`
- `validate-fcr5-scroll.ps1` Debug/Release: `PASS`
- `validate-fcr7-material-widget.ps1` Debug/Release: `PASS`
- 관련 Widgets/Material build: `PASS`
- `DorotiDemoApp` Android graph 및 `net10.0-android`/`android-arm64` Release AOT build: `PASS`
- SM-S931N/API 36 FCR-8 physical stability: `PASS`
  - 120 Hz, 737 frames / 6.934 s = 106.287 FPS
  - missed-vsync 0
  - submitted/presented 2944/2944, failed/dropped 0
- 사용자 수동 확인: 기존 중첩 scrollbar 동작이 정상적으로 작동함

## 남은 검증 경계

이번 문제의 구현 완료와 사용자 확인은 끝났지만 다음 항목은 별도 전용 시나리오를 실행하지 않았으므로
`notVerified`로 보존한다.

- Android outer/inner 각각 20회, 회전, light/dark peak·fade 픽셀과 pinned Flutter의 paired capture
- Web browser의 notification-depth/fade smoke
- MacCatalyst native 실행

Windows에서는 사용자 육안 smoke가 정상이었지만 nested ownership 전용 live 판정은 미완료다. 기존
`WindowsLive` 자동 측정도 p95는 양호했으나 단발성 max cadence 때문에 엄격한 cadence gate가 실패했으므로
이번 Android scrollbar 완료 근거와 합치지 않는다.

## 주요 변경 영역

- `Doroti/src/Doroti.Framework.Widgets/scroll_notification.cs`
- `Doroti/validation/fcr5-scroll/` 및 `Doroti/eng/validate-fcr5-scroll.ps1`
- `Doroti/validation/fcr7-material-widget/` 및 `Doroti/eng/validate-fcr7-material-widget.ps1`
- `Doroti/validation/evidence/flutter-conformance/`
- `DorotiDemoApp/src/App.cs`
- `DorotiDemoApp/README.md`, `DorotiDemoApp/README.ko.md`


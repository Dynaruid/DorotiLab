# Doroti Android 스크롤바 투명도 및 중첩 소유권 수정 계획

- 작성일: 2026-08-18
- 상태: **정적 원인 검토 완료, 구현 및 검증 미착수**
- 대상: `Doroti.Framework.Widgets`, `Doroti.Framework.Material`, Dart→C# lowerer, FCR-5/FCR-7 검증, `DorotiDemoApp`, Android MAUI/Skia 표시 경계
- 목표: Android 오버레이 스크롤바의 알파 동작을 의도와 Flutter 기준에 맞게 확정하고, 중첩된 자식 스크롤의 알림/메트릭이 부모 스크롤바 상태를 오염시키는 문제를 생성기까지 포함한 근본 계약에서 수정한다.

## 사용자 관찰

1. Android에서 보이는 오버레이 스타일 스크롤바가 반투명하지 않고 불투명해 보인다.
2. 부모와 자식이 같은 축으로 스크롤되는 중첩 구조에서 자식을 스크롤하면 부모 스크롤바도 반응한다.
3. 이때 부모 thumb의 위치와 길이가 자식의 viewport/content 메트릭에 맞춰진다.
4. 이후 부모를 직접 스크롤하면 부모 스크롤바는 다시 부모 메트릭으로 복구된다.

## 정적 검토 결론

### A. 중첩 스크롤바 문제는 `depth` 전달 손실로 설명된다

Flutter의 기본 스크롤 알림 필터는 `notification.depth == 0`인 가장 가까운 Scrollable만 받아들인다. 자식 알림 자체가 부모 방향으로 전파되는 것은 정상이며, 각 viewport를 지날 때 `depth`를 증가시켜 부모 스크롤바가 자식 알림을 거르는 것이 계약이다.

현재 Doroti 경로는 다음과 같다.

1. `ViewportNotificationMixin`과 viewport listener는 알림이 viewport를 지날 때 `_depth`를 증가시킨다.
2. `ScrollMetricsNotification.asScrollUpdate()`는 현재 depth를 `ScrollUpdateNotification(..., depth: this.depth)`로 전달한다.
3. `RawScrollbarState._handleScrollMetricsNotification()`은 변환된 알림에 `notificationPredicate`를 적용하고, 통과하면 painter를 해당 메트릭으로 갱신한다.
4. 그러나 Doroti의 `ScrollUpdateNotification` 생성자는 `depth` 매개변수를 받기만 하고 `_depth`에 저장하지 않는다.
5. 따라서 자식의 `ScrollMetricsNotification`이 부모까지 올라와도 `asScrollUpdate()` 결과의 depth가 다시 0이 되고, 부모 스크롤바가 이를 로컬 메트릭으로 오인한다.
6. 부모를 다시 스크롤하면 진짜 부모 메트릭이 들어오므로 thumb가 부모 상태로 복구된다.

Pinned Flutter 원본은 생성자 본문에서 `depth != null`일 때 `_depth = depth`를 수행한다. 그러므로 알림 전파를 막거나 부모 `NotificationListener`가 자식 알림을 소비하게 만드는 방식이 아니라, 생성자 본문과 생성 파이프라인의 depth 보존을 복구해야 한다.

현재 데모도 외부 `Scrollbar`/`SingleChildScrollView`에 명시적인 `_scrollController`를 사용하고 내부 `ListView`는 `primary: false`이므로, 관찰된 현상은 단순 controller 공유보다 위 depth 손실과 일치한다.

### B. Android thumb의 완전 불투명 상태는 우선 Flutter 기본 정책으로 분류해야 한다

현재 Doroti와 pinned Flutter의 Material `Scrollbar`는 Android idle thumb에 모두 다음 정책을 사용한다.

```text
Theme.of(context).highlightColor.withOpacity(1.0)
```

즉 Android idle thumb가 완전히 보이는 순간의 alpha 255는 현재 Flutter 기준과 일치한다. 또한 `DorotiDemoApp`은 외부 스크롤바에 `thumbVisibility: true`를 지정하므로 자동 fade-out도 의도적으로 비활성화된 상태다.

반면 공통 `ScrollbarPainter`는 thumb color alpha에 `fadeoutOpacityAnimation.value`를 곱하고, `Color`→`PaintSnapshot`→MAUI `SKColor` 경로도 alpha를 운반한다. 따라서 다음 두 경우를 분리해야 한다.

- **정책/구성 문제:** 항상 보이는 Android 기본 thumb가 alpha 255인 경우. Flutter parity는 유지하고, transient overlay 또는 반투명 peak가 제품 요구라면 `thumbVisibility`와 `ScrollbarThemeData.thumbColor`로 명시한다.
- **렌더링 회귀:** `thumbVisibility: false`에서도 600 ms 대기 + 300 ms fade가 진행되지 않거나, framework paint alpha는 변하는데 최종 Android 픽셀만 불투명한 경우. 이때는 ticker/repaint, retained command snapshot, MAUI/Skia blending 중 최초로 alpha가 어긋나는 층을 수정한다.

공통 Material Android 기본색을 바로 반투명으로 바꾸는 것은 pinned Flutter 동작을 변경하므로, paired evidence 없이 수행하지 않는다.

## 완료 계약

다음 조건을 모두 만족해야 완료로 판정한다.

1. 자식 스크롤 시작, 진행, 종료 및 자식 viewport 크기 변경 동안 부모 scrollbar painter의 메트릭, thumb rect, fade 상태가 바뀌지 않는다.
2. 같은 이벤트 동안 자식 스크롤바만 자식 메트릭으로 갱신된다.
3. 부모를 스크롤하면 부모 스크롤바가 별도의 복구 동작 없이 처음부터 부모 메트릭을 유지한다.
4. `ScrollMetricsNotification.depth`가 `asScrollUpdate()` 전후에 0, 1, 2 모두 정확히 보존된다.
5. Android thumb의 peak/fade alpha가 pinned Flutter 또는 문서화한 제품 theme 정책과 일치하고, framework paint와 실기기 합성 픽셀 사이에 설명되지 않은 alpha 손실이 없다.
6. 수정은 새 framework 변환에서 다시 생성되며, `scroll_notification.cs` 한 파일을 수동으로 고친 상태에 머물지 않는다.
7. 기존 스크롤 cadence, retained rendering, semantics 동기화 및 다른 플랫폼의 scrollbar 동작에 회귀가 없다.

## 작업 범위와 원칙

- 자식 알림의 bubbling은 유지한다. 전체 전파를 차단하면 `ScrollNotificationObserver`, AppBar, overscroll, nested-scroll 소비자가 깨질 수 있다.
- 부모와 자식의 controller를 억지로 공유하거나, scrollbar를 숨겨 증상을 제거하지 않는다.
- 중첩 소유권 수정은 Flutter의 depth 계약을 기준으로 Widgets와 lowerer 양쪽에서 닫는다.
- Android 투명도는 기본 Flutter parity와 제품 시각 정책을 구분한다.
- 자동 테스트, native live, Android 실기기 결과를 서로 대체하지 않는다. 실행하지 않은 게이트는 `notVerified`로 남긴다.
- 현재 worktree의 기존 스크롤/렌더/semantics 변경은 사용자 작업으로 보존하고, 계획 구현 시에도 관련 없는 diff를 되돌리지 않는다.

## M0. 재현과 관측 계약 고정

- [ ] FCR-5 또는 별도 widget fixture에 같은 축의 outer/inner Scrollable을 만든다.
  - outer와 inner는 서로 다른 `ScrollController`를 사용한다.
  - viewport dimension, min/max extent, 초기 offset을 서로 명확히 다르게 둔다.
  - outer/inner scrollbar painter를 독립 key로 식별한다.
- [ ] 다음 시퀀스를 자동 재현한다.
  1. outer 초기 메트릭 및 thumb rect 기록
  2. inner drag 또는 metrics change 발생
  3. inner scrollbar 갱신 확인
  4. outer 메트릭, thumb rect, fade controller가 불변인지 확인
  5. outer drag 후에도 각 scrollbar의 소유권이 유지되는지 확인
- [ ] 알림 진단에 event type, source context/position identity, depth, axis, pixels, viewportDimension, min/max extent, 수신 scrollbar identity를 기록한다.
- [ ] Android alpha fixture는 thumb 뒤에 알려진 두 색의 배경을 두고 다음 상태를 분리한다.
  - 기본 Android idle peak
  - `thumbVisibility: true`
  - `thumbVisibility: false`의 fade 시작/중간/종료
  - 명시적인 반투명 `ScrollbarThemeData.thumbColor`
- [ ] pinned Flutter의 동일 nested-scroll 및 Android scrollbar fixture를 같은 크기/DPR/theme/timestamp로 캡처한다.
- [ ] 최초 재현 evidence에는 현재 실패를 그대로 남기고 상태를 `expectedFailure` 또는 `notVerified`로 표기한다.

### M0 완료 게이트

- [ ] inner와 outer 메트릭을 숫자로 구분할 수 있고, 현재 Doroti에서 inner 이벤트 뒤 outer painter가 inner 값으로 바뀌는 실패가 결정적으로 재현된다.
- [ ] Android 불투명 현상이 peak 정책인지 fade/합성 손실인지 시간축 캡처로 분류된다.

## M1. Dart→C# 생성 계약과 `ScrollUpdateNotification.depth` 복구

- [ ] Dart lowerer 최소 fixture를 추가한다.
  - nullable named parameter를 가진 생성자
  - super-formal 전달
  - 생성자 block body 안의 null 검사
  - mixin에서 온 private field에 대한 대입
- [ ] fixture가 `_depth = depth`에 해당하는 조건부 대입을 생성하는지 확인한다.
- [ ] lowerer가 해당 statement 또는 mixin-private receiver를 누락한다면 생성기에서 일반 규칙으로 수정한다.
  - `ScrollUpdateNotification` 이름에만 의존하는 문자열 치환은 사용하지 않는다.
  - 기존 `ViewportNotificationMixin._depth` receiver cast 규칙과 일관되게 처리한다.
- [ ] lowerer fixture는 통과하지만 reviewed framework 산출물만 오래된 경우, 현재 pinned Flutter revision에서 해당 파일을 좁게 재생성하고 regeneration drift를 검증한다.
- [ ] 동일 AST 형태의 다른 생성자 본문이 누락됐는지 source closure를 감사한다.
- [ ] `scroll_notification.dart`, `scroll_position.dart`, 관련 Flutter notification/scrollbar test의 hash와 anchor를 FCR manifest에 추가한다.

### M1 완료 게이트

- [ ] 새 변환 산출물의 `ScrollUpdateNotification`은 nullable depth가 주어진 경우 `_depth`를 보존한다.
- [ ] lowerer 회귀 테스트가 수정 전 실패, 수정 후 통과한다.
- [ ] 동일 입력을 재생성해도 수동 후처리 없이 같은 결과가 나온다.

## M2. 중첩 scrollbar 소유권 회귀 테스트와 framework 수정

- [ ] 직접 계약 테스트를 추가한다.
  - depth 0 metrics notification → update depth 0
  - depth 1 metrics notification → update depth 1
  - depth 2 metrics notification → update depth 2
  - 기본 predicate는 depth 0만 허용
- [ ] widget-level nested fixture에서 inner drag notification과 inner `ScrollMetricsNotification`을 각각 검증한다.
- [ ] outer scrollbar의 painter update 호출 수와 마지막 metrics snapshot이 inner 이벤트 전후 동일한지 검증한다.
- [ ] inner scrollbar는 같은 이벤트를 정상 수신하고 thumb 길이/위치를 갱신하는지 검증한다.
- [ ] 이후 outer drag에서 outer만 갱신되고 inner 상태는 보존되는 역방향도 검증한다.
- [ ] 같은 축 중첩을 필수로 하고, 다른 축 중첩은 axis filter 회귀용 보조 케이스로 둔다.
- [ ] `ScrollNotificationObserver`, AppBar/RefreshIndicator, overscroll 및 nested scroll 소비자가 기존 depth 의미를 유지하는지 focused regression을 실행한다.

### M2 완료 게이트

- [ ] 사용자 관찰의 “자식 조작 → 부모 thumb가 자식 크기로 변함 → 부모 조작 시 복구” 시퀀스가 자동 fixture에서 더 이상 재현되지 않는다.
- [ ] 알림은 계속 bubble하지만 각 scrollbar는 자신의 가장 가까운 Scrollable만 반영한다.

## M3. Android 투명도 정책 확정 및 최초 불일치 층 수정

### M3-A. Flutter parity 판정

- [ ] light/dark theme에서 pinned Flutter와 Doroti의 다음 값을 비교한다.
  - resolved idle/hover/drag thumb color ARGB
  - fade animation value
  - `ScrollbarPainter`가 제출한 Paint alpha
  - 최종 합성 픽셀
- [ ] `thumbVisibility: false`에서 600 ms 대기 뒤 300 ms 동안 alpha가 단조 감소하고 최종 0이 되는지 확인한다.
- [ ] `thumbVisibility: true`에서는 fade하지 않는 것이 의도된 계약임을 테스트와 문서에 명시한다.

### M3-B. 판정별 구현

- [ ] **Doroti가 pinned Flutter와 일치하는 경우:** framework Android 기본색은 유지한다.
  - transient overlay가 요구사항이면 데모의 강제 `thumbVisibility: true`를 제거하거나 제품 설정으로 전환한다.
  - peak 자체의 반투명이 요구사항이면 Android에만 적용되는 `ScrollbarThemeData.thumbColor` 정책을 명시하고 light/dark 및 idle/hover/drag alpha를 정의한다.
  - 이 선택은 app별 우회가 아니라 공개 theme 계약 사용 사례로 검증/문서화한다.
- [ ] **Doroti fade 값부터 어긋나는 경우:** timer, animation controller, ticker scheduling, painter listener/repaint 순서에서 최초 불일치를 수정한다.
- [ ] **PaintSnapshot까지 맞고 host에서 어긋나는 경우:** retained command가 매 frame의 PaintSnapshot을 보존하는지, `Color.alpha`→`SKColor.Alpha` 변환과 `SrcOver` blending이 유지되는지 수정한다.
- [ ] track의 완전 투명과 thumb의 반투명을 별도 assertion으로 다룬다.

### M3 완료 게이트

- [ ] Android peak/fade 픽셀이 Flutter 기본 또는 승인한 제품 theme의 수치적 alpha와 일치한다.
- [ ] 시각적으로 비슷하다는 판정만이 아니라 painter/command/host/pixel 중 최초 불일치 지점이 evidence에 기록된다.
- [ ] Windows/Web 등 비 Android Material scrollbar 기본색은 의도치 않게 바뀌지 않는다.

## M4. 데모 재현면과 문서 동기화

- [ ] `DorotiDemoApp`의 현재 outer Scrollbar + inner lazy list 구성을 명시적인 중첩 회귀 surface로 정리한다.
- [ ] outer/inner controller, viewport/content 크기, scrollbar key를 진단에서 구별 가능하게 한다.
- [ ] 데모의 Android scrollbar가 transient인지 always-visible인지 UI 의도를 코드와 README에 명시한다.
- [ ] 반투명 peak를 선택한다면 light/dark 배경에서 목표 alpha와 상태별 색을 문서화한다.
- [ ] controller/listener/timer가 dispose에서 누수 없이 해제되는지 확인한다.
- [ ] 데모 변경은 재현과 제품 시각 정책을 보여 주는 용도이며, depth 결함 자체의 수정은 framework/lowerer에서 유지한다.

### M4 완료 게이트

- [ ] README.ko.md의 설명과 실제 Android 동작이 일치한다.
- [ ] 데모에서 inner/outer를 번갈아 스크롤해도 thumb 소유권이 안정적이다.

## M5. 검증 및 증거 갱신

### 자동 검증

- [ ] Dart→C# lowerer focused tests Debug/Release
- [ ] `validate-fcr5-scroll.ps1` Debug/Release
- [ ] `validate-fcr7-material-widget.ps1` Debug/Release
- [ ] 관련 Widgets/Material 프로젝트 build
- [ ] `DorotiDemoApp` Android target graph 및 Release build
- [ ] fixture manifest의 Flutter revision/hash/anchor 검증
- [ ] 생성 산출물 재현성 및 예상 밖 broad diff 확인

### Android 실기기 검증

- [ ] 동일 축 outer/inner를 각각 20회 이상 왕복 스크롤한다.
- [ ] inner 조작 중 outer offset, metrics snapshot, thumb rect, fade state가 불변인지 trace와 화면으로 확인한다.
- [ ] outer 조작 중 반대 방향 소유권도 확인한다.
- [ ] light/dark에서 peak, fade 중간, fade 종료 픽셀을 배경색과 함께 캡처해 유효 alpha를 계산한다.
- [ ] 느린 drag, 빠른 fling, 손가락을 댄 상태, viewport 회전/resize 후에도 동일 계약을 확인한다.
- [ ] 이전 Android cadence/흰 점멸/semantics 게이트를 다시 실행해 새 회귀가 없는지 확인한다.

### 교차 대상 검증

- [ ] Windows live에서 nested scrollbar 소유권 smoke test
- [ ] Web browser에서 notification depth와 fade smoke test
- [ ] 실행하지 못한 MacCatalyst/native 대상은 `notVerified`로 유지

### M5 완료 게이트

- [ ] 자동 계약, Android physical, 필요한 live target evidence가 각각 구분되어 저장된다.
- [ ] 실패나 미실행 항목을 PASS로 요약하지 않는다.
- [ ] 최종 evidence에 수정 전 실패와 수정 후 결과가 모두 남는다.

## 예상 수정 지점

| 계층 | 파일/영역 | 책임 |
|---|---|---|
| Flutter 기준 | `reference/flutter-master/.../widgets/scroll_notification.dart` | 생성자 depth 보존 기준 |
| Flutter 기준 | `reference/flutter-master/.../widgets/scroll_position.dart` | metrics→update 변환 기준 |
| Flutter 기준 | `reference/flutter-master/.../material/scrollbar.dart` 및 tests | Android 색/fade/nested 기준 |
| 생성기 | `tools/Doroti.DartToCSharp/.../FrameworkCSharpLowerer*` | 생성자 block과 mixin-private field 대입 보존 |
| Widgets | `Doroti.Framework.Widgets/scroll_notification.cs` | `ScrollUpdateNotification.depth` 보존 |
| Widgets | `Doroti.Framework.Widgets/scroll_position.cs`, `scrollbar.cs`, viewport 계열 | 변환, predicate, painter ownership 회귀 확인 |
| Material | `Doroti.Framework.Material/scrollbar.cs`, `scrollbar_theme.cs` | Android 기본색과 공개 theme 정책 |
| UI/Host | `Doroti.Ui` paint snapshot, `MauiSkiaCapabilities` | 실제 fade가 깨진 경우 alpha/blend 최초 불일치 수정 |
| 검증 | `validation/fcr5-scroll`, `validation/fcr7-material-widget`, `eng/validate-*` | 자동 계약과 evidence |
| 제품 재현 | `DorotiDemoApp/src/App.cs`, `README.ko.md` | nested repro와 승인된 Android 시각 정책 |

## 금지할 임시 해결책

- 부모 `NotificationListener`에서 모든 자식 알림을 무조건 소비하기
- 부모 scrollbar를 숨기거나 inner scrollbar를 제거하기
- 부모/자식 controller를 공유해 메트릭 오염을 가리기
- Android Material 기본색을 evidence 없이 전 플랫폼 공통으로 변경하기
- `scroll_notification.cs`만 손으로 고치고 lowerer/regeneration 계약을 남겨 두기
- 외부 FPS나 단일 스크린샷만으로 소유권 및 alpha 문제를 완료 처리하기

## 현재 검증 상태

| 항목 | 상태 | 근거 |
|---|---|---|
| 중첩 메트릭 오염 원인 | `reviewed` | Doroti가 전달받은 `depth`를 생성자에서 저장하지 않고, Flutter 원본은 저장함 |
| 일반 notification bubbling | `reviewed` | viewport depth 증가와 기본 depth predicate는 존재함 |
| Android idle thumb 기본 alpha | `reviewed` | Doroti와 pinned Flutter 모두 Android에서 `withOpacity(1.0)` 사용 |
| 데모 fade 구성 | `reviewed` | 외부 scrollbar가 `thumbVisibility: true`로 고정됨 |
| lowerer root cause | `notVerified` | 최소 변환 fixture와 재생성을 아직 실행하지 않음 |
| 자동 nested regression | `notVerified` | fixture 미구현 |
| Android fade/최종 pixel alpha | `notVerified` | 시간축 실기기 캡처 미실행 |
| Android physical acceptance | `notVerified` | 수정 미구현 |
| Windows/Web/MacCatalyst 회귀 | `notVerified` | 수정 미구현 |

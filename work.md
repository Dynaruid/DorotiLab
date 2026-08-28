# First-frame `dynamic` 공용 경로 정적화 작업 계획

- 작성일: 2026-08-28
- 기준 commit: `be1a63e9`
- 상위 계획: `plan.md` S2의 **First-frame `dynamic` 제거**
- 참고 자료: `idea.md`는 후보와 타입 설계 아이디어로만 사용한다. 그 문서의 전수 제거 순서나 결론은 실행 지시가 아니다.
- 현재 상태: **계획 완료**. 구현과 기능·실기 검증은 `notStarted` 또는 `notVerified`다. 새 계측과 반복 A/B는 선행 작업이 아니라 필요할 때만 수행하는 보조 작업이다.
- 목표: 기본 first-frame 경로와 Framework 공용 반복 경로에서 구조상 불필요한 DLR 바인딩·동적 호출이 분명한 2~3개 구간을 정적 계약으로 바꾼다.

## 1. 검토 결론

`idea.md`의 핵심 방향인 “행동은 interface/generic으로, 임의 데이터는 `object?`와 명시적 변환으로 표현한다”는 현재 코드와 맞는다. 특히 `actions.cs`에는 이미 `IIntentAction`과 `IActionListenerSource`가 있고, `Doroti.Runtime`은 `object?` + pattern matching/conversion helper를 사용한다.

성능 작업의 우선순위에는 다음 세 가지를 적용한다.

1. **`dynamic` 키워드 수와 DLR call site 수는 다르다.**
   - `List<dynamic>`, `DartMap<Type, dynamic>`, `DiagnosticsProperty<dynamic>`는 CLR metadata에서 사실상 `object` 계열이며, 선언 자체만으로 매 호출 DLR dispatch를 만들지는 않는다.
   - `((dynamic)value).Member`, dynamic invoke/index/convert 같은 연산은 compiler-generated `CallSite<T>`와 runtime binder를 만든다.
2. **source reachability와 호출 반복성으로 우선순위를 판단한다.**
   - 기본 Demo와 Framework lifecycle에서 직접 도달하고 element/render node/route/control 수만큼 반복되는 동적 호출은 별도 profiler 증명 없이 구현 대상으로 채택할 수 있다.
   - dialog, picker, inspector, Cupertino처럼 기본 경로 밖의 call site는 이번 범위에서 제외한다.
3. **구조상 확실한 비용 제거와 정량 성능 주장을 구분한다.**
   - 반복 경로의 DLR binder를 direct/interface/generic 호출로 바꾸면서 reflection, adapter allocation, boxing을 추가하지 않는 변경은 실행 작업과 할당을 줄이는 것이 명백하므로 먼저 구현한다.
   - 정확한 CPU 비율이나 TTID 개선폭을 주장해야 할 때만 profiler나 반복 A/B를 추가한다. 계측 미수행은 구조적으로 타당한 수정의 착수·유지 실패 사유가 아니다.

따라서 완료 기준은 `dynamic` 검색 일치 0개나 통계적 성능 입증이 아니라, **선택한 공용 반복 경로의 동적 dispatch 제거와 기능·계약 회귀 없음**이다.

## 2. 현재 정적 snapshot

현재 checkout의 `Doroti/src` C# source에서 `dynamic` 검색 일치는 175개 파일, 1,747개다.

기존 Release `net10.0` assembly를 reflection으로 조사한 compiler-generated `CallSite` field 수는 다음과 같다. 이 수치는 새 clean build나 runtime 실행 결과가 아니며, 한 dynamic 식이 여러 binder field를 만들 수 있다.

| Assembly | `CallSite` field | 해석 |
| --- | ---: | --- |
| `Doroti.Framework.Widgets` | 1,386 | 가장 큰 후보지만 미실행 기능 포함 |
| `Doroti.Framework.Material` | 743 | 기본 Demo가 일부 control을 사용하지만 picker/dialog 등도 포함 |
| `Doroti.Framework.Cupertino` | 281 | 기본 Demo first frame에서는 우선 제외 |
| `Doroti.Framework.Painting` | 55 | 기본 root의 source 경로에서 직접 도달하는 항목만 후보 |
| `Doroti.Framework.Rendering` | 2 | 수는 작아도 render-tree 반복 호출 여부를 별도 확인 |
| `Doroti.Runtime` | 0 | runtime의 `dynamic` 반환/문구가 DLR member dispatch를 만들지 않는 현재 방향을 유지 |

정적 owner 상위에는 `NavigatorState` 138, `_RouteEntry__navigator` 53, `TransitionRoute<T>` 21이 있다. Material에서는 `_RadioPaintState__radio` 49, `ScaffoldState` 39, `_TextFieldState__text_field` 36, `_MaterialSwitchState__switch` 32, `_CheckboxState__checkbox` 31, `_SliderState__slider` 30이 크다. 반면 `SingleChildRenderObjectElement`처럼 field 수가 적어도 element 수만큼 반복되는 경로는 호출 횟수 때문에 우선순위가 높을 수 있다.

기본 `DorotiDemoApp`은 다음 경로를 실제로 구성한다.

```text
WidgetsFlutterBinding
  -> MaterialApp(home)
  -> WidgetsApp
  -> Navigator initial home route
  -> default Shortcuts / Actions / Focus / Localizations
  -> Scaffold
  -> TextField + Radio + Switch + Checkbox + Slider
  -> nested ScrollView / ListView
  -> build / render-object child attach / layout / paint / semantics
```

따라서 render-tree core와 Navigator/Route는 별도 계측 없이도 우선 구현 대상으로 삼는다. Actions는 이미 있는 정적 계약을 완성하는 방향이라 세 번째 후보로 우선 검토하고, Material control은 여러 control이 공유하는 좁은 계약이 확인될 때만 범위에 넣는다.

## 3. 범위 원칙

### 3.1 반드시 지킬 규칙

- ADR-019에 따라 `Doroti/src/Doroti.Framework.*` 제품 C# source를 owning project에서 직접 수정한다. Dart-to-C# compiler로 제품 source를 재생성하거나 덮어쓰지 않는다.
- 동작이 필요한 값은 non-generic base/interface/generic으로 표현한다. 임의 payload만 `object?`로 두고 `DartRuntimePrimitives`에서 명시적으로 변환한다.
- DLR을 reflection, `MethodInfo.Invoke`, 문자열 member lookup으로 바꾸지 않는다.
- public 의미, Flutter reference 동작, route result generic, action intent type, listener exactly-once, render child ordering을 유지한다.
- public `dynamic` signature가 DLR 비용을 만들지 않으면 호환성 때문에 MVP에 남길 수 있다. 내부 진입 시 한 번 검증하여 typed view로 정규화한다.
- 각 cluster는 독립 patch로 구현하고 정적 dispatch 제거와 focused behavior validation으로 유지 여부를 판단한다.
- 최대 3개 cluster만 구현한다. 다음 후보가 기본 first-frame/공용 반복 경로가 아니거나 좁고 안전한 정적 계약을 만들 수 없으면 멈춘다.

### 3.2 성능 후보 cluster

| 후보 | 주요 파일/owner | 정적화 방향 | MVP 판단 근거 |
| --- | --- | --- | --- |
| C1 render-tree core | `Widgets/framework.cs`, `Widgets/binding.cs`, 필요한 `Rendering` owner | `RenderObject` direct call, single/multi-child type-erased contract, typed widget child access | 모든 app의 build/layout 경로에서 node 수만큼 반복되며 direct call로 대체 가능 |
| C2 initial Navigator/Route | `Widgets/navigator.cs`, `routes.cs`, `overlay.cs`, `app.cs` | non-generic route/transition base + `Route<T>` typed result, typed route entry/observer flow | 기본 initial route가 항상 사용되고 현재 wildcard 표현의 타입 의미도 바로잡음 |
| C3 Actions shell | `Widgets/actions.cs`, `app.cs`, `Material/app.cs` | 기존 `IIntentAction`/listener 계약을 완성하고 public compatibility map을 내부 typed map으로 정규화 | 기본 app shell에 포함되고 기존 interface를 확장하는 좁은 변경으로 반복 binder를 제거 가능 |
| C4 initial Material common contract | `Material/text_field.cs`, `scaffold.cs`, `radio.cs`, `switch.cs`, `checkbox.cs`, `slider.cs` | `IRestorableProperty`, toggleable/default-style 등 여러 control이 공유하는 좁은 typed contract | 여러 initial control이 같은 계약을 공유하는 경우에만 C3 대체 후보로 채택 |

C1과 C2를 먼저 수행하고, 세 번째는 source 검토에서 가장 좁고 공용성이 높은 C3 또는 C4 한 묶음만 선택한다.

## 4. W0 - 정적 경로 검토와 범위 동결

상태: `notStarted`

### 4.1 source와 정적 inventory

- `dynamic` declaration과 실제 dynamic member invoke/get/set/index/convert를 분리한다.
- C1~C4의 호출자와 default Demo/Framework lifecycle 도달 경로를 source에서 확인한다.
- 기존 Release assembly의 `CallSite` holder (`<>o__*`, `<>p__*`) inventory는 변경 전후 정적 차이를 확인하는 데 재사용한다. 이를 위한 새 runtime trace 도구는 만들지 않는다.
- element/render node/route/action/control 수에 비례해 반복되는 호출과 direct/base/interface 호출로 안전하게 바꿀 수 있는 호출을 우선 표시한다.
- 전체 키워드 수와 전체 `CallSite` field 수는 참고 지표일 뿐 작업 gate로 사용하지 않는다.

### 4.2 cluster 선택 기준

다음 조건을 source와 API 계약으로 판단하여 구현한다.

- 기본 first scene 또는 모든 app의 공용 lifecycle에서 직접 도달한다.
- DLR lookup/convert/invoke를 direct, virtual, interface 또는 generic 호출로 바꾸면 런타임 작업이 확실히 줄어든다.
- 새 reflection, 문자열 lookup, adapter/delegate allocation, boxing을 만들지 않는다.
- public 의미와 Flutter reference 동작을 보존하는 좁은 계약으로 닫을 수 있다.

C1 render-tree core와 C2 Navigator/Route는 위 조건을 이미 만족하는 우선 범위로 고정한다. W0에서는 구체 call site와 영향 받는 contract test만 확정한다. 세 번째 묶음은 C3 Actions를 우선하며, 실제 source 구조상 좁은 계약을 만들 수 없을 때 C4의 공통 계약 하나로 대체한다.

### 4.3 선택적 계측

다음 경우에만 `B07/B09/B11`, EventPipe/ETW 또는 짧은 A/B를 사용한다.

- 두 설계안의 비용·복잡도 판단이 엇갈린다.
- 수정 뒤 체감 또는 frame/startup 회귀가 의심된다.
- 정확한 CPU/allocation/TTID 개선 수치를 외부에 보고해야 한다.

계측 환경이 없거나 결과가 불안정하면 정량 성능은 `notVerified`로 기록하되, 구현 순서를 멈추거나 구조적으로 타당한 patch를 자동으로 되돌리지 않는다.

W0 산출물:

- 선택 owner의 source call-site 목록과 호출 경로
- C1/C2 및 세 번째 후보의 정적 계약 초안
- focused behavior test 목록과 제외 범위

## 5. W1 - C1 render-tree core 정적화

상태: `notStarted`, W0 source 검토 후 실행

### 5.1 대상

우선 확인할 호출은 다음과 같다.

- `ProxyElement`, `ParentDataElement`, `RenderObjectElement`의 typed child/member 접근
- `SingleChildRenderObjectElement`의 child validate/set/remove
- `MultiChildRenderObjectElement`의 insert/move/remove와 sibling ordering
- `WidgetsFlutterBinding`의 `RenderView.child`, `markNeedsPaint`, `visitChildren`
- `RenderObject`의 `dispose`, debug creator/attached 상태처럼 이미 base member로 호출 가능한 항목

### 5.2 설계

- base `RenderObject`에 이미 존재하는 virtual member는 direct call로 바꾼다.
- single/multi-child mixin 의미가 base에 없으면 internal non-generic contract를 `Doroti.Framework.Rendering`에 둔다.
- contract는 `ValidateChild`, `SetChild`, `Insert`, `Move`, `Remove`처럼 element layer가 실제 요구하는 최소 member만 노출한다.
- RenderBox/RenderSliver 등 concrete family가 contract를 구현하되 child type mismatch는 기존 assert/exception보다 늦게 실패하지 않게 한다.
- release hot path에 adapter allocation, delegate allocation, boxing이 새로 생기지 않게 한다.

### 5.3 gate

- single child mount/update/unmount와 multi child insert/move/remove ordering test PASS
- parent data, layout, paint, semantics tree 결과 동일
- 선택한 C1 dynamic member operation이 source와 Release `CallSite` 정적 audit에서 제거됨
- reflection/adapter allocation 없이 direct/interface 호출로 닫힘

## 6. W2 - C2 initial Navigator/Route 정적화

상태: `notStarted`, W1 다음에 실행

### 6.1 핵심 문제

C#의 `TransitionRoute<dynamic>`은 Dart의 “어떤 result type의 TransitionRoute” wildcard가 아니다. `TransitionRoute<int>`는 `TransitionRoute<object>`가 아니므로 현재 type test와 dynamic fallback은 타입 의미와 성능을 함께 흐린다.

### 6.2 설계

- 기존 빈 `_RoutePlaceholder__navigator`를 제품 계약으로 남기지 않고, 이름 있는 non-generic route base/interface를 만든다.
- `Route<T>`는 typed `popped/currentResult`를 유지하면서 non-generic base의 lifecycle, settings, navigator, transition notification을 구현한다.
- TransitionRoute에도 animation, secondary animation, debug label, transition-to/from을 노출하는 non-generic base/interface를 둔다.
- `_RouteEntry__navigator.route`, `RouteTransitionRecord.route`, history/observer/overlay의 내부 흐름을 non-generic route type으로 바꾼다.
- `Navigator` public generic push/pop/result API는 유지한다. public compatibility overload가 필요하면 경계에서 한 번 type check 후 typed core로 들어간다.
- `Route<dynamic>`/`TransitionRoute<dynamic>`을 wildcard처럼 사용하는 type test를 모두 제거한다.
- first route push, didAdd/didChangeNext/didChangePrevious, overlay install, initial transition에 필요한 범위만 바꾼다. restoration/declarative pages 전체 재설계는 이번 정적 계약에 필요하지 않으면 건드리지 않는다.

### 6.3 gate

- `Route<int>`, `Route<string>`, `Route<object?>`가 동일 Navigator history에서 push/transition/pop result를 보존
- initial `home` route와 named initial route의 ordering 및 observer callback exactly-once PASS
- overlay entry install/remove, back/pop, predictive-back 기본 계약 회귀 0
- 선택한 C2 owner의 dynamic route dispatch와 Release `CallSite` 정적 audit 제거
- Demo first scene/first present와 FCR-3/4/6/7 회귀 0

## 7. W3 - 정적 근거가 충분한 잔여 1개 cluster만 정적화

상태: `notStarted`, W1/W2 source 검토와 구현 결과를 바탕으로 결정

W1/W2 이후 중복된 contract 형태와 변경 위험을 검토하여 다음 중 **한 묶음만** 선택한다. 기본 선택은 기존 interface를 활용할 수 있는 Actions다.

### 선택지 A: Actions

- 기존 `IIntentAction`에 intent type, enabled/invoke, override calling-action에 필요한 non-generic member를 모은다.
- `IActionListenerSource` listener를 `Action<object>` 또는 명확한 action contract로 정적화한다.
- `DartMap<Type, dynamic>` public compatibility input은 유지 가능하지만, `Actions` 생성 시 각 value를 검증해 typed map/set으로 정규화한다.
- `ActionListener`, `ActionDispatcher`, `_ActionsState`, overridable action의 member dispatch를 interface/direct call로 바꾼다.
- 서로 다른 Intent subtype, override chain, listener add/remove/notify exactly-once를 검증한다.

### 선택지 B: restoration 공통 계약

- `text_field.cs`, `scaffold.cs`, `form.cs`, `restoration.cs`의 기본 화면 경로에 복제된 dynamic property access가 하나의 공통 계약으로 닫히면 `IRestorableProperty`와 공통 coordinator를 만든다.
- restoration id/owner/registered/enabled/disposed, primitive read/write, listener attach를 typed member로 고정한다.
- 초기 TextField controller, bucket restore, unregister/dispose, duplicate-id failure를 검증한다.
- 단순 중복 제거를 이유로 first-frame에 없던 restoration consumer까지 일괄 변환하지 않는다.

### 선택지 C: Material toggleable/control 공통 계약

- Radio/Switch/Checkbox/Slider에 반복되는 toggleable state/default-style member access 중 여러 control이 공유하는 부분만 좁은 interface/base로 올린다.
- 개별 control의 `dynamic defaults`를 각각 수동 치환하지 않고, 여러 control이 공유하며 call site를 함께 제거하는 계약일 때만 채택한다.
- selected/focused/hovered/pressed state, painter animation, semantics tap, theme M2/M3 결과를 검증한다.

좁은 정적 계약을 만들 수 없거나 새 adapter/reflection/boxing이 필요하면 W3 없이 종료한다. 계측 결과가 없다는 이유만으로 완료된 구조 개선을 되돌리지 않으며, 네 번째 후보로 범위를 늘리지 않는다.

## 8. 채택 및 검증 판정

각 cluster는 다음 조건을 만족하면 유지한다.

- 선택한 공용 반복 경로에서 dynamic member invoke/get/set/index/convert가 제거되거나 명시된 public compatibility boundary에만 남는다.
- DLR을 direct, virtual, interface 또는 generic 호출로 바꾸며 reflection, 문자열 lookup, adapter allocation, boxing을 새로 만들지 않는다.
- focused contract test와 관련 FCR validation이 PASS한다.
- 가능한 target의 Release live smoke에서 first frame, input, route, scroll, semantics에 명백한 회귀가 없다.

위 조건을 만족하면 결과를 `expectedImprovement`로 기록할 수 있다. 이는 제거된 런타임 작업에 근거한 공학적 판단이며, 정확한 TTID/CPU/allocation 개선폭을 측정했다는 뜻은 아니다.

반복 A/B와 profiler는 설계 선택이 애매하거나 회귀가 의심되거나 정량 보고가 필요할 때만 추가한다. 수행하지 않은 정량 성능과 실행하지 못한 physical target은 `notVerified`로 남긴다. 일부 call site 제거만으로 `Microsoft.CSharp`/interpreter dependency 전체 제거를 주장하지 않는다.

## 9. 기능 및 target validation

모든 test/build 명령은 repository 지침에 따라 20분 timeout을 사용한다.

### 9.1 focused contract

- 새 dynamic-dispatch validation: selected owner의 static `CallSite` audit와 runtime behavior fixture
- `dotnet run --project Doroti/validation/fcr3-scheduler/Doroti.Validation.Fcr3Scheduler.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr4-retained-rendering/Doroti.Validation.Fcr4RetainedRendering.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr5-scroll/Doroti.Validation.Fcr5Scroll.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr6-semantics/Doroti.Validation.Fcr6Semantics.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -c Release`

현재 `Doroti/eng/doroti.ps1 validate`가 참조하는 `eng/validate.ps1`은 checkout에 없으므로 이 계획의 검증 명령으로 사용하지 않는다. 각 owning validation project를 직접 실행한다.

### 9.2 product/target

- Windows default Release build: `pwsh -NoProfile -File Doroti/eng/doroti.ps1 build -App DorotiDemoApp -Platform windows`
- Windows live: light/dark initial frame, initial TextField, pointer/key/action, scroll, route/back, semantics-active 경로
- Web Release build/publish: trim/AOT warning, `_framework` compressed/uncompressed payload, first canvas present
- Android arm64 Release physical: 가능한 경우 한 번 이상의 first frame screenshot, PID/focus, crash/ANR, input/scroll/IME smoke. 반복 성능 측정은 선택 사항
- Apple/Linux target은 실제 실행 환경이 없으면 build 결과와 physical/runtime 결과를 분리하고 후자는 `notVerified`
- `git diff --check`와 selected owner `CallSite` inventory diff PASS

자동화 PASS가 physical IME, TalkBack/Narrator, 브라우저 live, Apple physical PASS를 대신하지 않는다.

## 10. MVP와 후속 범위

### MVP

- W0 source/static inventory와 구현 범위 동결
- 구조적 근거가 분명한 최대 3개 cluster의 typed contract 구현
- 각 cluster의 정적 dispatch 제거와 behavior validation으로 `keep` 결정
- focused validation, Windows default live, Web build/payload, Android arm64 build 및 가능한 physical evidence
- 결과를 PASS/FAIL/`notVerified`/`expectedImprovement`로 분리 기록

### 이번 작업에서 제외

- `Doroti.Framework.* dynamic = 0`
- 175개 파일 일괄 치환 또는 단순 `dynamic -> object?` mechanical rewrite
- `PageStorage`, Calendar structural generic, platform `creationParams`, picker/dialog/menu/inspector/Cupertino의 타입 정리
- public API 전체의 breaking typed migration
- Dart-to-C# compiler lowering 변경이나 framework regeneration
- 남은 call site가 있는 상태에서 `Microsoft.CSharp` assembly/interpreter 제거
- 이번 source 경로와 무관한 Web lazy assembly 분할이나 플랫폼 host 최적화

후속 작업은 correctness/API cleanup 또는 compiler import 품질 작업으로 별도 계획한다. 이번 구조 개선을 전체 Framework 전수 정리나 정량 성능 수치의 근거로 확대 해석하지 않는다.

## 11. 실행 순서와 상태

| 순서 | 작업 | 시작 조건 | 종료 조건 | 상태 |
| --- | --- | --- | --- | --- |
| W0 | source/static DLR inventory와 계약 검토 | 없음 | C1/C2와 세 번째 후보, 제외 사유 동결 | `notStarted` |
| W1 | C1 render-tree core 구현 | W0 검토 | behavior PASS + 선택 dispatch 정적 제거 | `notStarted` |
| W2 | C2 Navigator/Route 구현 | W1 contract 안정화 | behavior PASS + 선택 dispatch 정적 제거 | `notStarted` |
| W3 | Actions 우선 잔여 1개 cluster 구현 | 좁은 공용 계약 확인 | behavior PASS + 선택 dispatch 정적 제거 | `notStarted` |
| W4 | common/target validation과 결과 정리 | 최종 patch 확정 | PASS/FAIL/`notVerified`/`expectedImprovement` 분리 완료 | `notStarted` |

## 12. 구현과 병행할 operator 결정

다음 결정은 구현 착수 조건이 아니다. 정량 cross-platform 성능 보고나 해당 target 실기 검증을 할 때만 확정한다.

1. 대표 Windows machine과 Android arm64 physical device
2. Web 판정 browser/CPU/network profile
3. iOS/macOS/Linux physical/runtime gate를 이번 rollout에서 수행할 수 있는지 여부

결정 전에도 W0~W4 구현과 가능한 local validation을 진행한다. 실행하지 않은 target과 정량 성능은 `notVerified`로 유지한다.

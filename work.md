# First-frame `dynamic` hot path 정적화 작업 계획

- 작성일: 2026-08-28
- 기준 commit: `be1a63e9`
- 상위 계획: `plan.md` 188행의 **First-frame `dynamic` 제거**
- 참고 자료: `idea.md`는 후보와 타입 설계 아이디어로만 사용한다. 그 문서의 전수 제거 순서나 결론은 실행 지시가 아니다.
- 현재 상태: **계획 완료**. 구현, 새 계측, A/B 측정, 실기 검증은 모두 `notStarted` 또는 `notVerified`다.
- 목표: `B07 frameworkReady -> B11 firstPresented`에서 실제 실행되는 DLR 바인딩과 반복 동적 호출 중 성능 기여가 큰 2~3개 공통 구간만 정적 계약으로 바꾼다.

## 1. 검토 결론

`idea.md`의 핵심 방향인 “행동은 interface/generic으로, 임의 데이터는 `object?`와 명시적 변환으로 표현한다”는 현재 코드와 맞는다. 특히 `actions.cs`에는 이미 `IIntentAction`과 `IActionListenerSource`가 있고, `Doroti.Runtime`은 `object?` + pattern matching/conversion helper를 사용한다.

다만 성능 작업의 우선순위로 그대로 사용하기 전에 다음 세 가지를 구분해야 한다.

1. **`dynamic` 키워드 수와 DLR call site 수는 다르다.**
   - `List<dynamic>`, `DartMap<Type, dynamic>`, `DiagnosticsProperty<dynamic>`는 CLR metadata에서 사실상 `object` 계열이며, 선언 자체만으로 매 호출 DLR dispatch를 만들지는 않는다.
   - `((dynamic)value).Member`, dynamic invoke/index/convert 같은 연산은 compiler-generated `CallSite<T>`와 runtime binder를 만든다.
2. **정적 call site 존재와 first-frame 실행도 다르다.**
   - dialog, picker, inspector, Cupertino, platform view용 call site가 assembly에 있어도 기본 Demo 첫 화면에서 bind되지 않으면 이번 성능 범위가 아니다.
3. **쉬운 치환과 큰 성능 효과도 다르다.**
   - `PageStorage`나 `creationParams`의 `object?` 치환은 타입 정리로는 유효하지만, first-frame DLR CPU가 확인되지 않으면 이번 작업에서 하지 않는다.

따라서 완료 기준은 `dynamic` 검색 일치 0개가 아니라, **측정된 first-frame DLR hot cluster 제거와 profiler-free Release A/B 개선**이다.

## 2. 현재 정적 snapshot

현재 checkout의 `Doroti/src` C# source에서 `dynamic` 검색 일치는 175개 파일, 1,747개다.

기존 Release `net10.0` assembly를 reflection으로 조사한 compiler-generated `CallSite` field 수는 다음과 같다. 이 수치는 새 clean build나 runtime 실행 결과가 아니며, 한 dynamic 식이 여러 binder field를 만들 수 있다.

| Assembly | `CallSite` field | 해석 |
| --- | ---: | --- |
| `Doroti.Framework.Widgets` | 1,386 | 가장 큰 후보지만 미실행 기능 포함 |
| `Doroti.Framework.Material` | 743 | 기본 Demo가 일부 control을 사용하지만 picker/dialog 등도 포함 |
| `Doroti.Framework.Cupertino` | 281 | 기본 Demo first frame에서는 우선 제외 |
| `Doroti.Framework.Painting` | 55 | trace에 잡힌 항목만 후보 |
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

따라서 Navigator/Route와 render-tree core는 우선 계측 후보가 맞다. Actions와 Material control은 기본 화면에 있다는 이유만으로 채택하지 않고 실제 bind/CPU/allocation 결과로 순서를 정한다.

## 3. 범위 원칙

### 3.1 반드시 지킬 규칙

- ADR-019에 따라 `Doroti/src/Doroti.Framework.*` 제품 C# source를 owning project에서 직접 수정한다. Dart-to-C# compiler로 제품 source를 재생성하거나 덮어쓰지 않는다.
- 동작이 필요한 값은 non-generic base/interface/generic으로 표현한다. 임의 payload만 `object?`로 두고 `DartRuntimePrimitives`에서 명시적으로 변환한다.
- DLR을 reflection, `MethodInfo.Invoke`, 문자열 member lookup으로 바꾸지 않는다.
- public 의미, Flutter reference 동작, route result generic, action intent type, listener exactly-once, render child ordering을 유지한다.
- public `dynamic` signature가 DLR 비용을 만들지 않으면 호환성 때문에 MVP에 남길 수 있다. 내부 진입 시 한 번 검증하여 typed view로 정규화한다.
- 각 cluster는 독립 patch와 A/B evidence를 가진다. 효과가 noise 이하이면 `noBenefit`으로 되돌리고 다음 후보로 넘어간다.
- 최대 3개 cluster만 구현한다. 관측된 first-frame DLR CPU의 80%를 덮었거나 다음 후보가 선택 기준 미만이면 멈춘다.

### 3.2 성능 후보 cluster

| 후보 | 주요 파일/owner | 정적화 방향 | MVP 채택 조건 |
| --- | --- | --- | --- |
| C1 render-tree core | `Widgets/framework.cs`, `Widgets/binding.cs`, 필요한 `Rendering` owner | `RenderObject` direct call, single/multi-child type-erased contract, typed widget child access | first frame에서 bind되고 element/render-node 반복 비용이 측정됨 |
| C2 initial Navigator/Route | `Widgets/navigator.cs`, `routes.cs`, `overlay.cs`, `app.cs` | non-generic route/transition base + `Route<T>` typed result, typed route entry/observer flow | initial home route push/transition이 DLR CPU 상위에 있음 |
| C3 Actions shell | `Widgets/actions.cs`, `app.cs`, `Material/app.cs` | 기존 `IIntentAction`/listener 계약을 완성하고 public compatibility map을 내부 typed map으로 정규화 | default Actions build 또는 first input-ready까지의 binder 비용이 상위에 있음 |
| C4 initial Material common contract | `Material/text_field.cs`, `scaffold.cs`, `radio.cs`, `switch.cs`, `checkbox.cs`, `slider.cs` | `IRestorableProperty`, toggleable/default-style 등 여러 control이 공유하는 좁은 typed contract | 개별 control 치환이 아니라 하나의 공통 계약이 상위 비용 여러 개를 동시에 제거함 |

C1~C4를 전부 한다는 뜻이 아니다. W0 측정에서 순위를 확정한 뒤 상위 2개를 먼저 수행하고, 세 번째는 누적 효과와 잔여 DLR 비용을 보고 결정한다.

## 4. W0 - 실제 DLR hot path 계측과 범위 동결

상태: `notStarted`

### 4.1 정적 inventory

diagnostic validation에 Roslyn/IL 기반 inventory를 추가한다.

- `dynamic` declaration, dynamic member invoke/get/set/index/convert를 분리한다.
- Release assembly의 `CallSite` holder (`<>o__*`, `<>p__*`)를 assembly, owner type, method, source sequence point로 매핑한다.
- 전체 키워드 수와 전체 `CallSite` field 수는 참고 지표로만 남긴다.
- C1~C4 owner별 baseline field 목록을 `.doroti/evidence/startup/<commit>/<platform>/dynamic-static.json`에 저장한다.
- inventory 도구는 product assembly를 수정하거나 IL rewrite하지 않는다.

### 4.2 runtime inventory

`plan.md`의 B07/B09/B11 marker와 같은 monotonic time 축을 사용한다.

- profiler-on Release 3회에서 `Microsoft.CSharp.RuntimeBinder`, `System.Dynamic`, `CallSite<T>` 생성/초기화, 관련 JIT method, allocation, sampled stack을 수집한다.
- diagnostic run에서는 B07 직후, B09, B11 이후에 초기화된 call site 차이를 기록한다. reflection/stack 수집은 first present 이후 flush하며 acceptance 숫자에는 사용하지 않는다.
- closed generic holder처럼 reflection snapshot만으로 누락될 수 있는 항목은 EventPipe/ETW method/JIT stack과 static IL inventory를 합쳐 판정한다.
- profiler-free process-cold 10회와 warm/resume 20회에서 `B07->B11`, TTID, TTFD, managed allocation, first interaction frame p95를 기록한다.
- Windows 기본 `WindowsAppSdk/HwndExactCpp`를 1차 원인 분석 target으로 사용하고, 같은 source의 Android arm64/Web 결과를 별도 target evidence로 유지한다.

### 4.3 cluster 선택 gate

다음 중 하나를 만족하고 실제 first-frame stack에 나타난 cluster만 구현한다.

- `B07->B11` managed sampled CPU의 5% 이상을 차지한다.
- first-bound DLR CPU 또는 allocation의 10% 이상을 차지한다.
- element/render node 수에 비례해 반복되며, first frame 또는 첫 interaction frame p95에서 측정 noise를 넘는 비용이 재현된다.

동률이면 Demo 전용 control보다 모든 app에 공통인 낮은 owner (`Framework.Widgets`/`Rendering`)를 먼저 선택한다. Route의 generic 의미 오류처럼 correctness 가치가 있더라도 성능 gate를 통과하지 못하면 별도 follow-up으로 이동한다.

W0 산출물:

- 전체 정적 inventory
- B07/B09/B11에서 새로 bind된 call site 목록
- owner별 CPU/allocation/JIT 표
- 선택된 최대 3개 cluster와 제외 사유
- baseline raw evidence와 `notVerified` target 목록

## 5. W1 - C1 render-tree core 정적화

상태: `notStarted`, W0 선택 시에만 실행

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
- C1 owner의 Release `CallSite` field와 B07~B11 first-bound call site 0
- C1 단독 A/B가 noise 이하이면 patch는 유지하지 않고 `noBenefit`

## 6. W2 - C2 initial Navigator/Route 정적화

상태: `notStarted`, W0 선택 시에만 실행

### 6.1 핵심 문제

C#의 `TransitionRoute<dynamic>`은 Dart의 “어떤 result type의 TransitionRoute” wildcard가 아니다. `TransitionRoute<int>`는 `TransitionRoute<object>`가 아니므로 현재 type test와 dynamic fallback은 타입 의미와 성능을 함께 흐린다.

### 6.2 설계

- 기존 빈 `_RoutePlaceholder__navigator`를 제품 계약으로 남기지 않고, 이름 있는 non-generic route base/interface를 만든다.
- `Route<T>`는 typed `popped/currentResult`를 유지하면서 non-generic base의 lifecycle, settings, navigator, transition notification을 구현한다.
- TransitionRoute에도 animation, secondary animation, debug label, transition-to/from을 노출하는 non-generic base/interface를 둔다.
- `_RouteEntry__navigator.route`, `RouteTransitionRecord.route`, history/observer/overlay의 내부 흐름을 non-generic route type으로 바꾼다.
- `Navigator` public generic push/pop/result API는 유지한다. public compatibility overload가 필요하면 경계에서 한 번 type check 후 typed core로 들어간다.
- `Route<dynamic>`/`TransitionRoute<dynamic>`을 wildcard처럼 사용하는 type test를 모두 제거한다.
- first route push, didAdd/didChangeNext/didChangePrevious, overlay install, initial transition에 필요한 범위만 바꾼다. restoration/declarative pages 전체 재설계는 trace에 없으면 건드리지 않는다.

### 6.3 gate

- `Route<int>`, `Route<string>`, `Route<object?>`가 동일 Navigator history에서 push/transition/pop result를 보존
- initial `home` route와 named initial route의 ordering 및 observer callback exactly-once PASS
- overlay entry install/remove, back/pop, predictive-back 기본 계약 회귀 0
- C2 선택 owner의 Release `CallSite`와 first-bound DLR 제거
- Demo first scene/first present와 FCR-3/4/6/7 회귀 0

## 7. W3 - 잔여 상위 1개 cluster만 정적화

상태: `notStarted`, W1/W2 재측정 후 결정

W1/W2 이후 profiler를 다시 수집하고 다음 중 **한 묶음만** 선택한다.

### 선택지 A: Actions

- 기존 `IIntentAction`에 intent type, enabled/invoke, override calling-action에 필요한 non-generic member를 모은다.
- `IActionListenerSource` listener를 `Action<object>` 또는 명확한 action contract로 정적화한다.
- `DartMap<Type, dynamic>` public compatibility input은 유지 가능하지만, `Actions` 생성 시 각 value를 검증해 typed map/set으로 정규화한다.
- `ActionListener`, `ActionDispatcher`, `_ActionsState`, overridable action의 member dispatch를 interface/direct call로 바꾼다.
- 서로 다른 Intent subtype, override chain, listener add/remove/notify exactly-once를 검증한다.

### 선택지 B: restoration 공통 계약

- `text_field.cs`, `scaffold.cs`, `form.cs`, `restoration.cs`에 복제된 dynamic property access가 실제 상위라면 `IRestorableProperty`와 공통 coordinator를 만든다.
- restoration id/owner/registered/enabled/disposed, primitive read/write, listener attach를 typed member로 고정한다.
- 초기 TextField controller, bucket restore, unregister/dispose, duplicate-id failure를 검증한다.
- 단순 중복 제거를 이유로 first-frame에 없던 restoration consumer까지 일괄 변환하지 않는다.

### 선택지 C: Material toggleable/control 공통 계약

- Radio/Switch/Checkbox/Slider에 반복되는 toggleable state/default-style member access 중 runtime 상위만 좁은 interface/base로 올린다.
- 개별 control의 `dynamic defaults`를 각각 수동 치환하지 않고, 여러 control이 공유하며 call site를 함께 제거하는 계약일 때만 채택한다.
- selected/focused/hovered/pressed state, painter animation, semantics tap, theme M2/M3 결과를 검증한다.

선택한 묶음이 단독 A/B에서 noise를 넘지 못하면 `noBenefit`으로 되돌리고 W3 없이 종료한다. 네 번째 후보로 범위를 늘리지 않는다.

## 8. 성능 판정

각 cluster 전후를 같은 machine/device, 같은 power/thermal 조건, 같은 Release publish option으로 비교한다.

채택 조건:

- 해당 cluster의 B07~B11 first-bound call site가 0이거나, 명시된 compatibility boundary 외에는 0
- 해당 cluster의 DLR sampled CPU/allocation 90% 이상 감소
- `B07->B11`과 process-cold TTID p50 개선의 95% confidence interval이 0을 넘고, 개선폭이 baseline noise band보다 큼
- TTID p95, TTFD p95, warm/resume p95, first interaction frame p95 중 어느 것도 5% 초과 악화하지 않음
- first frame blank/crash/hang/terminal exactly-once 위반 0

전체 `dynamic` 작업의 성공 목표는 선택된 cluster 누적 기준으로 다음과 같다.

- 관측된 B07~B11 DLR CPU 80% 이상 제거, 또는 잔여 각 cluster가 선택 gate 미만
- process-cold TTID/TTFD 개선은 실제 수치로 보고하되 임의의 절대 ms 목표로 PASS를 만들지 않음
- Web payload/AOT warning 변화는 별도 수치로 기록하며, 일부 call site 제거만으로 `Microsoft.CSharp`/interpreter dependency 전체 제거를 주장하지 않음

reference device가 정해지기 전에는 최종 cross-platform 성능 PASS를 선언하지 않는다. 고정된 개발 machine의 A/B는 `localEvidence`, 물리 Android/iOS는 각각 별도 evidence다.

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
- Android arm64 Release physical: process-cold 10회, first frame screenshot, PID/focus, crash/ANR, input/scroll/IME
- Apple/Linux target은 실제 실행 환경이 없으면 build 결과와 physical/runtime 결과를 분리하고 후자는 `notVerified`
- `git diff --check`와 selected owner `CallSite` inventory diff PASS

자동화 PASS가 physical IME, TalkBack/Narrator, 브라우저 live, Apple physical PASS를 대신하지 않는다.

## 10. MVP와 후속 범위

### MVP

- W0 static/runtime inventory와 baseline
- 측정 상위 최대 3개 cluster의 typed contract 구현
- 각 cluster 독립 A/B와 `keep`/`noBenefit` 결정
- focused validation, Windows default live, Web build/payload, Android arm64 build 및 가능한 physical evidence
- 결과를 PASS/FAIL/`notVerified`/`noBenefit`으로 분리 기록

### 이번 작업에서 제외

- `Doroti.Framework.* dynamic = 0`
- 175개 파일 일괄 치환 또는 단순 `dynamic -> object?` mechanical rewrite
- `PageStorage`, Calendar structural generic, platform `creationParams`, picker/dialog/menu/inspector/Cupertino의 타입 정리
- public API 전체의 breaking typed migration
- Dart-to-C# compiler lowering 변경이나 framework regeneration
- 남은 call site가 있는 상태에서 `Microsoft.CSharp` assembly/interpreter 제거
- first-frame trace에 없는 Web lazy assembly 분할이나 플랫폼 host 최적화

후속 작업은 correctness/API cleanup 또는 compiler import 품질 작업으로 별도 계획한다. 이번 performance evidence를 전수 정리의 정당화로 사용하지 않는다.

## 11. 실행 순서와 상태

| 순서 | 작업 | 시작 조건 | 종료 조건 | 상태 |
| --- | --- | --- | --- | --- |
| W0 | static/runtime DLR inventory와 baseline | B07/B09/B11 marker 사용 가능 | 최대 3개 cluster와 제외 사유 동결 | `notStarted` |
| W1 | 1순위 cluster 구현/A/B | W0 선택 | behavior PASS + 성능 `keep` 또는 `noBenefit` | `notStarted` |
| W2 | 2순위 cluster 구현/A/B | W1 재측정 | behavior PASS + 성능 `keep` 또는 `noBenefit` | `notStarted` |
| W3 | 잔여 1개 cluster 결정/구현 | 잔여가 gate 이상 | 누적 80% 제거 또는 다음 후보 gate 미만 | `notStarted` |
| W4 | common/target validation과 evidence 정리 | 최종 kept patch 확정 | PASS/FAIL/`notVerified` 분리 완료 | `notStarted` |

## 12. 구현 전 operator 결정

최종 cross-platform 성능 PASS 전에 다음만 확정한다.

1. 대표 Windows machine과 Android arm64 physical device
2. Web 판정 browser/CPU/network profile
3. iOS/macOS/Linux physical/runtime gate를 이번 rollout에서 수행할 수 있는지 여부

결정 전에도 W0와 local A/B는 진행할 수 있지만, 해당 target의 최종 상태는 `notVerified`로 유지한다.

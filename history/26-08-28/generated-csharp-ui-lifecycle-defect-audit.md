# 제품 C# UI 수명주기·전달 결함 감사 및 수정 결과

- 감사·수정일: 2026-08-28
- 범위: `Doroti/src/Doroti.Framework.*`, `tools/Doroti.DartToCSharp`, FCR-7 Material/Widget validation
- 관련 작업: [Android/iOS 텍스트 선택 오버레이 요약](./android-text-selection-overlay-summary.md)

## 결론

초기 감사에서 확정한 네 결함군은 공유 Framework C#에서 수정했다.

1. named factory가 `key`, `child`, builder, controller 또는 `SemanticsProperties`를 잃는 문제
2. `removeListener`가 등록 때와 다른 lambda delegate를 만들어 구독 해제에 실패하는 문제
3. RenderObject·ChangeNotifier 수명주기 메서드가 CLR override가 아닌 새 virtual slot이 되는 문제
4. nullable toolbar callback을 non-null wrapper로 바꾸는 문제

다만 초기 문서의 소유권 전제는 잘못됐다. [`Doroti/src/Doroti.Framework.*`는 제품 소유 C#](../../README.md#current-development-model)이며 컴파일러가 덮어쓰는 생성 산출물이 아니다. 따라서 수정은 제품 소스에 직접 적용했고, Dart-to-C# compiler는 optional import/reference 도구로만 보강했다. “전체 Framework 재생성”은 이 제품 경로의 합격 조건이 아니다.

## 수정 전·후 판정

아래 수정 전 수량은 2026-08-28 초기 snapshot이다. 수정 후 수량은 현재 source scan과 Release runtime reflection 결과다.

| 결함군 | 수정 전 | 수정 후 | 판정 |
|---|---:|---:|---|
| `default!` 포함 named factory raw 후보 | 128 | 110 | 조사 후보 수이며 결함 수가 아님 |
| 완전히 비어 있거나 핵심 인자를 잃는 대표 factory | 7 | 0 | PASS |
| 새 lambda를 직접 전달하는 `removeListener` call site | 376 | 0 | PASS |
| RenderObject 핵심 hook의 hidden virtual slot | 22 | 0 | PASS |
| `ChangeNotifier.dispose` hidden slot | 22 | 0 | PASS |
| `addListener`/`removeListener` hidden slot | 7/6 | 0/0 | PASS |
| adaptive toolbar의 nullable callback wrapper | 최소 17 | 0 | PASS |

`var __instance = new ... default!` raw 후보 110개는 계속 존재하지만, 이 문자열 자체는 결함 판정식이 아니다. value-object factory처럼 instance를 만든 뒤 필드를 의도대로 설정하는 구현도 포함한다. 후속 감사에서는 public 인자가 최종 객체 상태에 실제로 소비되는지를 형식별로 확인해야 한다.

## 1. Named factory 인자 보존

동일 이름·동일 형식의 factory 인자를 primary constructor로 전달하는 416개 인자 위치를 복원했다. 이름만 같은 다른 형식은 자동 전달하지 않고 별도로 검토했다.

초기 문서에서 “완전히 비어 있음”으로 분류한 7개는 다음과 같이 수정했다.

| factory | 수정 내용 |
|---|---|
| `AnimatedList.CreateSeparated` | item/separator builder를 교차 호출하는 delegate와 분리 child count 구성 |
| `SliverList.CreateSeparated` | item/separator delegate, semantic index, child count 구성 |
| `SliverSemantics.CreateFromProperties` | `key`, `child`, `localeForSubtree`, `SemanticsProperties`를 base properties constructor에 전달 |
| `Semantics.CreateFromProperties` | 같은 base properties constructor 경로로 전달 |
| `RepaintBoundary.CreateWrap` | child와 기존 key 또는 child index 기반 fallback key 보존 |
| `TextEditingController.CreateFromValue` | text, selection, composing을 포함한 전체 value 보존 |
| `LogicalKeySet.CreateFromSet` | 네 개를 넘는 key도 잘리지 않도록 set 전체 보존 |

추가로 다음 고위험 factory를 직접 보정했다.

- `Positioned.CreateFromRect`, `CreateFromRelativeRect`, `CreateFill`: key와 child 보존
- `PrimaryScrollController.CreateNone`: key와 child를 보존하고 controller null/상속 차단 유지
- `SliverFixedExtentList`, `SliverVariedExtentList`, `SliverGrid`, `SliverPrototypeExtentList`: builder/list delegate 구성
- 그 밖의 동일 이름·동일 형식 key, child, controller, restoration 인자 전달

`Semantics.fromProperties` 두 경로는 pinned Flutter source의 `super.fromProperties()` 의미와 맞게 별도 base constructor를 사용한다. 개별 semantics 필드를 다시 풀어 전달하지 않으므로 전달된 `SemanticsProperties`의 identity도 보존한다.

## 2. Listener delegate identity

수정 전에는 다음 두 식이 서로 다른 delegate를 만들었다.

```csharp
notifier.addListener(() => this._handleChange());
notifier.removeListener(() => this._handleChange());
```

직접 `removeListener` lambda 376개를 안정된 method group 또는 저장된 `Action`으로 바꿨다. dynamic notifier처럼 명시적 delegate 형식이 필요한 위치는 동일한 `Action` 변수를 등록과 제거 양쪽에서 사용한다.

`Future`를 반환하는 두 listener는 C# `Action` method group으로 바꿀 수 없으므로 안정된 void adapter를 한 번 선언해 공유한다.

- `RawAutocomplete._onChangedFieldListener`
- search anchor의 `_updateSuggestionsListener`

재발 방지는 `Doroti/eng/validate-framework-lifecycle-source.ps1`가 Framework source의 직접 `removeListener` lambda를 실패 처리한다.

## 3. CLR lifecycle override

동일 signature의 base virtual이 있는 다음 메서드를 새 `virtual` slot이 아니라 `override`로 연결했다.

- RenderObject: `paint`, `attach`, `detach`, `performLayout`, `computeDryLayout`, `setupParentData`, `applyPaintTransform`, `hitTestChildren`
- ChangeNotifier 계열: `dispose`, `addListener`, `removeListener`
- router mixin 계열의 abstract listener contract

`ChangeNotifier.addListener`와 `removeListener`의 제품 base 선언은 수정 전 non-virtual이었다. 초기 감사가 이 사실을 놓친 채 derived 메서드만 override 대상으로 기록한 것은 부정확했다. base 두 메서드를 virtual contract로 만든 뒤 derived 구현을 override로 연결했다.

FCR-7 reflection gate는 Foundation, Rendering, Widgets, Cupertino, Material assembly를 훑고 위 메서드 이름에서 동일 virtual base를 숨기는 `NewSlot`을 발견하면 실패한다. 현재 결과는 0개다.

초기 raw metadata 후보 909개는 결함 수가 아니다. interface 구현, 공변 반환, 독립 contract가 섞인 분류 전 수치였으므로 현재 판정표에서는 확정 lifecycle signature만 gate 대상으로 삼는다.

## 4. Nullable toolbar callback

Material/Cupertino `AdaptiveTextSelectionToolbar`의 nullable callback을 다음처럼 무조건 non-null로 만들던 wrapper를 제거했다.

```csharp
onCopy: () => onCopy()
```

이제 nullable callback을 그대로 전달한다. callback이 없으면 toolbar item도 생기지 않으며 null invocation 경로가 만들어지지 않는다. Material `CreateSelectable`의 nullable `onShare`도 같은 방식으로 고쳤다.

source gate는 두 adaptive toolbar 파일에서 같은 이름의 callback을 `() => callback()`으로 감싸는 패턴을 실패 처리한다.

## 5. Compiler 보강과 경계

optional Dart-to-C# 경로의 G53 compatibility에는 다음 보정을 추가했다.

- 같은 이름 callback의 불필요한 zero-argument forwarding wrapper 제거
- `(Action)(() => listener())` 형태를 동일 listener method group으로 변환

compiler Release build는 통과했다. 그러나 전체 pinned Flutter import를 새 isolated workspace에서 실행해 differential을 비교하지는 않았다. 또한 zero-argument method-group rewrite는 반환 형식에 대한 typed 판단이 필요하므로, async listener처럼 void adapter가 필요한 경우까지 일반 규칙으로 해결됐다고 판정하지 않는다. 이 항목은 `notVerified`다.

제품 Framework는 compiler output이 아니므로 이 미검증이 현재 제품 수정의 PASS를 무효화하지 않는다. 향후 import 결과를 채택할 때는 제품 source를 덮어쓰지 않고 isolated output diff를 검토해야 한다.

## 6. 회귀 시나리오

FCR-7 runtime contract에 다음 검사를 추가했다.

| ID | 시나리오 | 합격 기준 | 상태 |
|---|---|---|---|
| F-1 | `Positioned.CreateFromRect(key, child)` | key와 child reference 보존 | PASS |
| F-2 | `PrimaryScrollController.CreateNone(key, child)` | subtree 보존, controller null, 자동 상속 차단 | PASS |
| F-3 | `SliverList.CreateSeparated` | item/separator 생성과 child count 보존 | PASS |
| F-4 | `TextEditingController.CreateFromValue` | text, selection, composing 보존 | PASS |
| F-5 | `LogicalKeySet.CreateFromSet` | 5개 key set 전체 보존 | PASS |
| F-6 | `Semantics.fromProperties` 두 변형 | key, child, properties reference 보존 | PASS |
| L-1 | add/remove 후 notifier 발행 | callback 0회, listener count 0 | PASS |
| R-1 | lifecycle reflection scan | 확정 signature hidden slot 0개 | PASS |
| C-1 | nullable toolbar action 미제공 | toolbar item 0개 | PASS |
| P-1 | Android 실제 선택 UI | toolbar, 양쪽 handle, magnifier 표시·drag | `notVerified` |
| P-2 | iOS 실제 선택 UI | Cupertino toolbar, 양쪽 handle, magnifier 표시·drag | `notVerified` |

## 7. 실행한 검증

모든 test/build job은 저장소 지침에 따라 20분 timeout을 사용했다.

| 검증 | 결과 | 근거 |
|---|---|---|
| Framework lifecycle source gate | PASS | 703개 Framework C# 파일, 직접 remove lambda 0, adaptive toolbar null wrapper 0 |
| Dart-to-C# compiler Release build | PASS | warning 0, error 0 |
| FCR-7 Material/Widget Release build | PASS | warning 0, error 0 |
| FCR-7 runtime contract | PASS | `FCR-7 material/widget runtime contract: PASS` |
| Android physical UI | `notVerified` | 이번 작업에서 실기 실행 안 함 |
| iOS physical/simulator UI | `notVerified` | 이번 작업에서 UI 실행 안 함 |
| isolated full Flutter import differential | `notVerified` | 제품 수정의 필수 gate가 아니며 이번 작업에서 실행 안 함 |

## 8. 남은 한계

- raw factory 후보 110개는 자동 결함 수로 사용하지 않는다. 새로운 factory를 감사할 때는 인자 소비와 최종 객체 상태를 함께 검사해야 한다.
- source scan은 알려진 위험 패턴을 막고, CLR dispatch는 runtime reflection으로 검증한다. 둘 중 하나만으로 전체 의미 보존을 증명하지 않는다.
- managed runtime PASS는 Android/iOS의 실제 overlay 배치, drag, magnifier와 접근성 동작을 증명하지 않는다. 해당 항목은 실기 확인 전까지 `notVerified`로 유지한다.

# 생성 C# UI 수명주기·전달 결함 감사

- 작성일: 2026-08-28
- 범위: `Doroti/src`의 현재 추적 C# 산출물, `tools/Doroti.DartToCSharp`의 lowering 규칙, 최신 FCR-7 Release 산출물
- 목적: Android 텍스트 선택에서 확인된 Magnifier, 컨텍스트 메뉴, 선택 핸들 결함과 같은 계열의 잠재·확정 결함을 저장소 전체에서 찾는다.
- 관련 작업: [Android/iOS 텍스트 선택 오버레이 요약](./android-text-selection-overlay-summary.md)

## 결론

이번 결함은 텍스트 선택 한 기능만의 문제가 아니다. 현재 생성 C#에는 다음 세 가지 구조적 결함군이 남아 있다.

1. named factory가 전달 인자를 잃고 `default!` 객체를 반환한다.
2. `addListener`와 `removeListener`가 서로 다른 delegate를 만들어 구독 해제에 실패한다.
3. Dart의 override 메서드가 C#의 새 `virtual` 슬롯으로 생성되어 렌더·수명주기 dispatch가 base 구현으로 빠진다.

| 우선순위 | 결함군 | 현재 확인 수 | 판정 |
|---|---|---:|---|
| P0 | `default!` 기반 named factory 후보 | 128개 | 전수 수정 대상 후보. 이 중 직접적인 인자 소실은 아래에서 별도 확정 |
| P0 | 반환 Widget의 `key`를 constructor에 전달하지 않는 factory | 45개, 31개 형식, 22개 파일 | 확정 |
| P0 | 인자를 하나도 적용하지 않고 기본 instance만 반환하는 factory | 7개 | 확정 |
| P0 | 새 lambda로 `removeListener` 호출 | 376개, 111개 파일 | 확정 |
| P0 | RenderObject 핵심 hook이 base virtual을 숨김 | 22개 | 확정 |
| P1 | `ChangeNotifier.dispose`를 새 virtual로 숨김 | 22개 | 확정, 호출 정적 형식에 따라 cleanup 누락 가능 |
| P1 | `addListener`/`removeListener`를 새 virtual로 숨김 | 각각 7개/6개 | 확정, interface/base dispatch 시 다른 listener store 사용 가능 |
| P1 | nullable callback을 무조건 non-null lambda로 포장 | 텍스트 선택 toolbar에서 최소 17개 | 확정 |
| 조사 필요 | 동일 이름의 base virtual과 새 슬롯이 함께 존재하는 raw metadata 후보 | 909개, 350개 형식 | 결함 수가 아님. 공변 반환 override와 interface 구현 등을 제외하는 후속 분류 필요 |

수량은 2026-08-28 현재 산출물의 스냅샷이다. 전체 재생성 뒤에는 반드시 같은 검사를 다시 실행해야 한다.

## 1. Named factory의 인자 소실

### 1.1 완전히 비어 있는 factory 7개

다음 메서드는 모든 인자를 받지만 `default!`로 instance를 만든 뒤 아무 인자도 적용하지 않고 바로 반환한다.

| 위치 | 메서드 | 영향 |
|---|---|---|
| `Widgets/animated_scroll_view.cs:24` | `AnimatedList.CreateSeparated` | builder, separator, controller, 방향, item count 등 전부 소실 |
| `Widgets/basic.cs:1767` | `SliverSemantics.CreateFromProperties` | key, child, semantics properties 전부 소실 |
| `Widgets/basic.cs:2689` | `RepaintBoundary.CreateWrap` | child와 child index 소실 |
| `Widgets/basic.cs:2819` | `Semantics.CreateFromProperties` | key, child, semantics properties 전부 소실 |
| `Widgets/editable_text.cs:117` | `TextEditingController.CreateFromValue` | 초기 `TextEditingValue` 소실 |
| `Widgets/shortcuts.cs:166` | `LogicalKeySet.CreateFromSet` | 전달된 key set 소실 |
| `Widgets/sliver.cs:70` | `SliverList.CreateSeparated` | builder, separator, child count 등 전부 소실 |

이 패턴은 호출 즉시 잘못된 객체를 만들므로 P0이다. 예를 들어 `SliverList.CreateSeparated`는 delegate가 null인 객체를 반환하며, `TextEditingController.CreateFromValue`는 전달한 selection/composing 상태를 보존하지 않는다.

### 1.2 `key` 소실 45개

factory signature에 `Key? key`가 있지만 `new Type(...)` 호출에는 `key:`가 없고 모두 `default!`가 들어간다. `Widget.key`는 base constructor에서 설정되고 setter가 private이므로 뒤의 field assignment로 복구할 수 없다.

영향 형식 31개는 다음과 같다.

- `AnimatedList`, `AnimatedPositioned`, `BackdropFilter`
- `DefaultSelectionStyle`, `DefaultTextStyle`, `FadeInImage`, `Flow`
- `HeroControllerScope`, `Image`, `InteractiveViewer`, `ListView`, `ListWheelScrollView`
- `OverlayPortal`, `PageView`, `PerformanceOverlay`, `Positioned`, `PrimaryScrollController`
- `SelectionContainer`, `Semantics`, `SemanticsNode`, `Shortcuts`
- `SliverFixedExtentList`, `SliverGrid`, `SliverList`, `SliverPrototypeExtentList`, `SliverSemantics`, `SliverVariedExtentList`, `SliverVisibility`
- `Text`, `Visibility`, `WidgetsApp`

주요 메서드는 다음과 같다.

- `Positioned.CreateFromRect`, `CreateFromRelativeRect`, `CreateFill`
- `Image.CreateNetwork`, `CreateFile`, `CreateAsset`, `CreateMemory`
- `PageView.CreateBuilder`, `CreateCustom`
- `ListView.CreateSeparated`, `CreateCustom`
- `SliverGrid.CreateBuilder`, `CreateCount`, `CreateExtent`, `CreateList`
- `WidgetsApp.CreateRouter`

`key` 소실은 단순 metadata 손실이 아니다. Element 재사용, GlobalKey 조회, 상태 보존, overlay target 연결이 달라질 수 있다.

### 1.3 `child`·builder·controller 소실

공통 인자 이름이 signature 외에는 한 번도 사용되지 않는 factory만 세어도 46개 메서드에서 인자 소실이 확인됐다. 이 검사는 최소치이며 임시 지역 변수에 대입만 하고 실제 constructor에 전달하지 않는 경우는 포함하지 않는다.

| 소실 인자 | 직접 확인 수 |
|---|---:|
| `child` | 10 |
| `itemBuilder` | 6 |
| `children` | 4 |
| `controller` | 3 |
| `restorationId` | 2 |

대표 사례:

- `Positioned.CreateFromRect` 계열은 `child`를 잃는다. 현재 프레임워크 안에서 호출이 19개 확인됐고 Cupertino context menu, dialog, Material badge 등에 직접 영향을 준다.
- `PrimaryScrollController.CreateNone`은 `child`를 잃는다. 현재 scroll view 계열에서 호출이 3개 확인됐다.
- `SliverPrototypeExtentList.CreateBuilder`는 `itemBuilder`를, `CreateList`는 `children`을 잃는다.
- `SliverFixedExtentList`와 `SliverVariedExtentList`의 builder/list factory도 child delegate를 만들지 않는다.
- `PageView.CreateBuilder`는 Material calendar date picker에서 사용된다. factory가 base scroll contract와 key를 잃으면 월 전환·복원·focus 동작이 영향을 받을 수 있다.
- `WidgetsApp.CreateRouter`는 MaterialApp과 CupertinoApp 양쪽에서 호출된다. key 전달이 누락된다.

### 1.4 현재 lowering과 추적 산출물의 불일치

`FrameworkCSharpLowerer.Declarations.cs:1094-1115`에는 named generative constructor의 공통 이름 인자를 primary constructor로 전달하려는 일반 규칙이 이미 있다. 그러나 현재 `Doroti/src`에는 128개의 `default!` factory 후보가 남아 있다.

또한 G53 compatibility에는 다음 개별 보정만 추가돼 있다.

- `GridView` base scroll 인자 전달
- `SizedBox`의 `key`, `child` 전달
- `Transform`의 `key`, `child` 전달

따라서 현재 상태는 “lowerer에 일반 로직이 있으므로 해결됨”으로 판정할 수 없다. 분석 IR의 constructor 대응 실패, 이전 산출물 잔존, 부분 재생성 중 하나 이상이 존재하며 전체 재생성 후 결과 기반 검사가 필요하다.

## 2. Listener delegate 정체성 소실

### 2.1 확정 결함 376개

현재 산출물에는 다음 형태의 구독 해제가 376개, 111개 파일에 있다.

```csharp
notifier.addListener(() => this._handleChange());
notifier.removeListener(() => this._handleChange());
```

두 lambda 식은 서로 다른 delegate다. `ChangeNotifier.removeListener`는 저장된 delegate와 `==`인 항목만 제거하므로 두 번째 호출은 첫 번째 구독을 제거하지 못한다.

상호 배타적으로 분류하면 다음과 같다.

| 형태 | 수 |
|---|---:|
| `_tickerModeNotifier`를 새 lambda로 제거 | 135 |
| 그 밖의 `() => this.Method()` 제거 | 199 |
| `(Action)(() => listener())` cast wrapper | 32 |
| 지역 callback을 새 lambda로 제거하는 기타 형태 | 10 |

상위 hotspot은 다음과 같다.

- `Material/range_slider.cs`: 16
- `Material/slider.cs`: 10
- `Material/time_picker.cs`: 10
- `Widgets/interactive_viewer.cs`: 10
- `Cupertino/context_menu.cs`: 9
- `Material/page_transitions_theme.cs`: 9
- `Widgets/navigator.cs`, `selectable_region.cs`, `text_selection.cs`, `toggleable.cs`, `undo_history.cs`: 각 8

예상 증상:

- dispose 뒤에도 callback이 호출됨
- widget 교체 때 listener 중복 등록
- focus/ticker/clipboard 상태가 여러 번 갱신됨
- overlay가 닫힌 뒤에도 repaint 또는 toolbar 갱신 수행
- 장시간 사용 시 객체·controller·state가 notifier에 붙어 남음

### 2.2 텍스트 선택·입력 경로에 남은 51개

이번 작업과 직접 연결되는 핵심 파일 9개만 합쳐도 51개가 남아 있다.

| 파일 | 수 |
|---|---:|
| `Widgets/editable_text.cs` | 7 |
| `Widgets/text_selection.cs` | 8 |
| `Widgets/selectable_region.cs` | 8 |
| `Material/text_field.cs` | 6 |
| `Material/selectable_text.cs` | 3 |
| `Material/text_selection.cs` | 4 |
| `Cupertino/text_field.cs` | 4 |
| `Cupertino/text_selection.cs` | 2 |
| `Cupertino/context_menu.cs` | 9 |

오늘 적용한 selection overlay의 안정 delegate 보정은 해당 경로의 일부를 고쳤지만, focus, ticker, clipboard, selectable geometry, controller listener에는 같은 문제가 남아 있다.

### 2.3 generator 규칙의 현재 한계

`FrameworkCSharpLowerer.G53Compatibility.cs:1403-1414`에는 `() => this.Method()`를 method group으로 바꾸는 일반 정규식이 추가돼 있다. 그러나 현재 추적 산출물에는 해당 패턴이 남아 있으므로 전체 재생성과 재검사가 필요하다.

더 중요한 예외는 `(Action)(() => listener())` 32개다. G53의 `property.addListener/removeListener` 보정이 이 wrapper를 명시적으로 생성하고 있어, 단순 재생성만으로는 해결되지 않는다. 등록 시 생성한 delegate를 필드/지역 변수에 저장하거나 원래 `listener` method group을 그대로 전달해야 한다.

## 3. `virtual`/`override` dispatch 소실

### 3.1 RenderObject 핵심 hook 22개

최신 FCR-7 Release assembly의 method slot을 검사하고 현재 C# 선언도 대조했다. 아래 메서드는 동일 signature의 base virtual이 있는데도 `override`가 아니라 새 `public virtual`로 선언돼 있다.

| hook | 수 | 영향 |
|---|---:|---|
| `paint` | 6 | custom paint가 호출되지 않아 시각 요소 소실 |
| `detach` | 5 | listener/layer/resource cleanup 누락 |
| `attach` | 3 | listener/layer 등록 누락 |
| `performLayout` | 3 | subclass layout 미실행 |
| `computeDryLayout` | 2 | intrinsic/dry layout 오계산 |
| `setupParentData` | 1 | 잘못된 parent data 사용 |
| `applyPaintTransform` | 1 | overlay/child 좌표 변환 오류 |
| `hitTestChildren` | 1 | pointer hit test가 base 경로로 빠짐 |

확인된 형식:

- Cupertino: `_RenderCupertinoSlider.paint`, `_RenderSwipeSurface.detach`
- Material: `_RenderIntrinsicHorizontalStadium.computeDryLayout/performLayout`, `_RenderMenuItem.performLayout`
- Widgets: `_ColorFilterRenderObject.paint`, `_RenderCompositionCallback.paint`, `_RenderVisibility.paint`
- Widgets overlay: `_RenderLayoutBuilder.computeDryLayout/hitTestChildren/setupParentData/applyPaintTransform/paint`
- Widgets lifecycle: `_RenderSemanticsClipper.attach/detach`, `_RenderSizeChangedWithCallback.performLayout`, `_RenderSliverFloatingHeader.detach`
- Snapshot/TapRegion: `_RenderSnapshotWidget.attach/detach/paint`, `RenderTapRegionSurface.attach/detach`

Magnifier와 Material toolbar에서 발견한 결함도 같은 원인이었다. 현재 G53은 해당 두 파일의 알려진 메서드만 `override`로 바꾸며, 위 22개는 보정 범위 밖이다.

### 3.2 notifier 수명주기 dispatch

`ChangeNotifier.dispose`와 동일 signature인데 새 virtual로 생성된 형식이 22개다.

- Cupertino: `CupertinoTabController`
- Material: `TabController`, `_ZoomEnterTransitionPainter`, `_ZoomExitTransitionPainter`
- Widgets: `FocusManager`, `FocusNode`, `ScrollController`, `ScrollPosition`, `ScrollbarPainter`, `ClipboardStatusNotifier`, `LiveTextInputStatusNotifier`, `UndoHistoryController`, `RestorableProperty<T>`, `ShortcutRegistry`, `ToggleablePainter` 등 18개

구체 형식으로 직접 호출하면 subclass 메서드가 실행될 수 있지만, `ChangeNotifier`, `Listenable` 또는 다른 base 정적 형식으로 호출하면 base slot으로 dispatch될 수 있다. cleanup 보장을 위해 CLR override 관계를 복원해야 한다.

같은 방식으로 `addListener` 7개와 `removeListener` 6개도 base slot을 숨긴다. `ClipboardStatusNotifier`, `LiveTextInputStatusNotifier`, `_CompositeRenderEditablePainter`, `_DefaultSnapshotPainter`, `PlatformRouteInformationProvider` 등이 포함된다. 이 경우 등록과 제거가 서로 다른 listener store로 갈 수 있다.

### 3.3 raw 후보 909개의 해석

metadata에서 동일 이름/signature의 base virtual과 새 slot이 함께 보이는 raw 후보는 909개다. 이 숫자를 곧바로 결함 수로 사용하면 안 된다.

- C# 공변 반환 override는 metadata 표현상 새 slot과 method implementation mapping을 함께 사용할 수 있다.
- Dart interface/mixin을 C# interface로 투영한 경우에는 `virtual` 구현이 정상일 수 있다.
- `copyWith`, theme property getter, diagnostics 계열은 의도적으로 독립 슬롯일 수 있다.

따라서 후속 analyzer는 return type, MethodImpl mapping, base class와 interface를 구분해야 한다. 이번 보고서의 22개 RenderObject hook과 notifier의 void 메서드는 현재 소스의 `virtual` 선언까지 교차 확인해 확정했다.

## 4. Nullable callback을 non-null wrapper로 변경

Material/Cupertino의 `AdaptiveTextSelectionToolbar.CreateEditable`은 nullable callback 8개를 각각 다음과 같이 감싼다.

```csharp
onCopy: () => onCopy()
```

이 wrapper 자체는 항상 non-null이다. 따라서 `EditableText.getEditableButtonItems`의 `onCopy is not null` 같은 capability 검사가 항상 참이 되고, 원래 callback이 null이어도 메뉴 항목이 생성된다. 누르면 null invocation이 발생한다.

- Material `CreateEditable`: nullable callback 8개
- Cupertino `CreateEditable`: nullable callback 8개
- Material `CreateSelectable`: nullable `onShare` 1개

최소 17개가 null 의미를 바꾼다. callback을 그대로 전달하거나 nullable 보존 adapter를 사용해야 한다.

## 5. 플랫폼 영향

이 결함들은 Android host만의 문제가 아니라 공유 Framework C#에 있다.

| 경로 | Android | iOS/Cupertino | Windows/Web 등 |
|---|---|---|---|
| named factory 인자 소실 | 영향 | 영향 | 영향 |
| listener 정체성 소실 | 영향 | 영향 | 영향 |
| RenderObject override 소실 | 영향 | 영향 | 영향 |
| adaptive toolbar nullable callback | Material 영향 | Cupertino 포함 영향 | desktop toolbar 경로도 유사 wrapper 존재 |

Android에서 보였던 핸들·toolbar·magnifier 증상은 한 플랫폼의 native selection overlay 문제가 아니라 공유 Widget/RenderObject graph가 잘못 생성된 결과였다. iOS simulator-arm64는 빌드만 통과했으며, 이번에 새로 확인한 결함들의 iOS 실제 UI 동작은 `notVerified`다.

## 6. 권장 수정 순서

1. **회귀 gate를 먼저 추가한다.**
   - named factory의 public parameter가 constructor/initializer/body 어디에도 소비되지 않으면 실패
   - `removeListener` 인자에 lambda가 직접 들어가면 실패
   - RenderObject/ChangeNotifier 핵심 hook이 base class method와 동일 signature인데 CLR override가 아니면 실패
   - nullable callback이 unconditional wrapper로 non-null이 되면 실패
2. **lowerer의 구조적 원인을 수정한다.**
   - named generative constructor의 super/primary initializer argument mapping을 IR에서 보존
   - Dart method tear-off를 동일 delegate identity의 method group 또는 저장된 delegate로 lowering
   - `FindOverriddenBaseMember`가 superclass의 실제 contract를 찾지 못할 때 이름만으로 `virtual` fallback하지 않도록 signature/assembly contract 해석 보강
3. **전체 Framework를 재생성한다.**
   - 개별 G53 문자열 패치만 늘리지 않는다.
   - 기존 수동 수정이 있는 파일은 재생성 diff를 별도로 감사한다.
4. **정적·metadata gate를 다시 실행한다.**
   - 128/376/22 수가 각각 기대치로 감소했는지 확인한다.
   - raw 909 후보는 MethodImpl/interface를 제외한 actionable 목록으로 축소한다.
5. **런타임 회귀를 플랫폼별로 검증한다.**
   - Android: TextField/SelectableText, context menu, 양쪽 handle drag, magnifier, dispose 후 중복 callback
   - iOS/Cupertino: 동일 항목과 adaptive toolbar action 노출 조건
   - 공통: Positioned child, Sliver builder/list, PageView, PrimaryScrollController, router key/state 유지

## 7. 최소 회귀 시나리오

| ID | 시나리오 | 합격 기준 |
|---|---|---|
| F-1 | `Positioned.CreateFromRect(key, child)` | 반환 객체의 key와 child가 동일 reference |
| F-2 | `PrimaryScrollController.CreateNone(child)` | child tree가 유지되고 primary controller 상속만 차단 |
| F-3 | `SliverList.CreateSeparated` | item/separator delegate가 실제 child를 생성 |
| F-4 | `TextEditingController.CreateFromValue` | text, selection, composing이 모두 보존 |
| L-1 | add/remove 후 notifier 발행 | callback 0회 |
| L-2 | widget/controller 교체를 20회 반복 | callback 중복 및 listener 수 증가 없음 |
| R-1 | RenderObject를 base 형식으로 paint/layout 호출 | concrete override가 실행됨 |
| R-2 | attach/detach 반복 | layer/listener/resource 잔존 없음 |
| C-1 | nullable toolbar action 미제공 | 해당 메뉴 항목이 생성되지 않음 |
| P-1 | Android 선택 UI | toolbar, 양쪽 handle, magnifier 표시·드래그 정상 |
| P-2 | iOS 선택 UI | Cupertino toolbar, 양쪽 handle, magnifier 표시·드래그 정상 |

## 8. 이번 감사의 검증 상태

| 항목 | 상태 | 근거 |
|---|---|---|
| 현재 생성 C# 정적 패턴 검사 | PASS | `Doroti/src` 전체 `.cs` 스캔 |
| ChangeNotifier 제거 동작 대조 | PASS | delegate `==` 비교 후 제거하는 구현 확인 |
| 최신 FCR-7 assembly method slot 검사 | PASS | Release assembly와 현재 source 선언 교차 확인 |
| 실제 결함 수정 | 미수행 | 이번 요청은 감사와 문서화 범위 |
| 새로 확인한 factory/listener/render 결함 Android 실기 검증 | `notVerified` | 개별 수정 전 |
| 새로 확인한 결함 iOS simulator UI 검증 | `notVerified` | iOS는 빌드 확인만 완료된 상태 |

## 9. 감사 범위의 한계

- factory 인자 소실 검사는 `key`, `child`, `children`, `itemBuilder`, `controller`, `restorationId` 등 고위험 공통 인자를 우선 검사했다. 다른 이름의 인자 소실이 더 있을 수 있다.
- `var __instance = new ... default!` 128개 모두가 결함은 아니다. value object factory처럼 이후 모든 필드를 올바르게 초기화하는 경우가 있다.
- raw virtual 후보 909개는 분류 전 수치다. 이번 문서에서 확정으로 표시한 lifecycle 항목과 구분해야 한다.
- build 성공은 dispatch, delegate identity, child graph 보존을 증명하지 않는다. 각 gate는 구조 검사와 실제 UI 검증을 함께 가져야 한다.

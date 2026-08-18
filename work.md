# Doroti Flutter Conformance & Smooth Rendering Upgrade

> 상태: **ACTIVE PLAN — FCR-0 inventory/contract gate, FCR-1 framework shader contract, FCR-2 typed semantic/runtime contract와 FCR-3 scheduler/frame-ownership contract 구현 완료; app interaction·differential·live/physical 검증은 후속 gate로 남음**
> 작성일: 2026-08-18
> Doroti 기준 revision: `3fd08b3` + 이 문서 변경
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> 1차 제품 대상: `DorotiDemoApp`, Android MAUI physical, Windows MAUI live
> 원칙: Flutter source가 framework 동작의 owner이며, Doroti의 차이는 host/backend 적응에 필요한 경우에만 명시한다.

## 1. 왜 이 계획이 필요한가

최근 Android 스크롤 장애는 하나의 성능 문제가 아니었다. InkSparkle의 Flutter framework shader asset이 Doroti package에 없어서 Ink가 보이지 않았고, C# 수치 연산 차이가 애니메이션 프레임 예외를 만들었으며, 그 예외와 native back-buffer 교체가 겹치면서 상단 콘텐츠·배경이 간헐적으로 사라졌다. 접근성 native tree를 매 프레임에 가깝게 갱신한 비용도 스크롤 끊김과 GC 압력을 키웠다.

개별 증상은 현재 완화되었지만, 같은 종류의 누락·오역을 자동으로 막는 계약은 아직 부족하다. 앞으로는 “컴파일된다”, “첫 프레임이 제출됐다”, “픽셀이 한 번 바뀌었다”를 Flutter 호환 완료로 취급하지 않는다. Flutter와 동일한 입력에서 상태, layout, paint, semantics, raster와 지속 프레임이 일치하는지를 확인한다.

## 2. 현재 조사 결과

### 2.1 확인된 문제 유형

| 영역            | 현재 확인된 사실                                                                                                                                                        | 구조적 위험                                                                                                                              |
| --------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| framework asset | Flutter `ink_sparkle.dart`는 `shaders/ink_sparkle.frag`를 요구하지만 Doroti에는 처음에 대응 자산/패키징 계약이 없었다. 현재는 Material assembly에 개별 임베드되어 있다. | 다음 shader/font/data asset도 source만 번역되고 package에서 누락될 수 있다.                                                              |
| runtime effect  | Flutter framework에는 InkSparkle과 StretchEffect shader asset이 있다. Doroti는 InkSparkle은 embedded resource, StretchEffect는 C# 문자열로 서로 다르게 소유한다.        | shader 원본 revision, uniform/sampler ABI, 라이선스, backend 변환과 cold-load 실패를 한 곳에서 검증하지 못한다.                          |
| 언어 의미       | InkSparkle의 `Tween<Vector2>` 경로에서 Dart의 Offset 보간이 C# dynamic binary operation으로 내려가 runtime 예외를 냈다.                                                 | generic arithmetic, `double`/`float`, nullable, callback, `Future`, assert 내부 debug-only side effect가 다른 파일에서도 어긋날 수 있다. |
| 프레임 지속성   | MAUI native surface는 새 back buffer를 받을 수 있으므로 마지막 성공 scene replay가 필요하다. 현재 host는 마지막 scene command를 보존해 재생한다.                        | 이것은 화면 소실을 막는 최소 안전망이며, Flutter식 retained layer 재사용·damage 추적·resource lifetime과 동일하다는 증거는 아니다.       |
| scroll 경로     | Demo에서 실제 `ScrollController.offset`, Scrollbar, lazy child lifecycle과 raster를 함께 보지 않으면 pointer packet만으로 성공을 오판할 수 있다.                        | app-level 보정으로 framework의 PrimaryScrollController/Scrollbar ownership 또는 ScrollActivity 결함이 가려질 수 있다.                    |
| semantics       | MAUI bridge는 15 fps 최신값 coalescing과 native element 재사용을 적용했다. 최근 Android evidence는 `576 received / 175 applied / 422 coalesced`였다.                    | 모든 변경을 동일하게 지연하면 focus/action/selection을 늦출 수 있고, 실제 속성 delta가 아닌 전체 node 정렬·갱신 비용도 남는다.           |
| compositing     | 기존 scene-operation matrix 52개는 reference differential이 전부 `notVerified`다. 그중 9개는 `notVerified`, 4개는 `explicitUnsupported`로 기록되어 있다.                | 오래된 matrix와 현재 host 구현이 어긋날 수 있으며, `presented`만으로 effect 의미 보존을 증명할 수 없다.                                  |
| evidence        | Android physical scroll/FAB 자동화 결과는 있으나 evidence의 상위 status/boundary 일부는 여전히 `partial`/`notVerified`다.                                               | 실행 사실과 acceptance 범위를 schema가 일관되게 표현하지 못하면 회귀를 놓치거나 완료를 과장한다.                                         |

### 2.2 바로 전수 조사할 고위험 패턴

- Flutter의 `FragmentProgram.fromAsset`, framework shader 디렉터리, font/icon/data asset과 Doroti package manifest의 폐쇄성
- `dynamic` 산술, `Tween<T>`, Offset/Size/Rect/Matrix와 `double` → `float` 축소
- `DartRuntimePrimitives.Ignore(Future)` 및 초기화 실패가 관찰되지 않는 비동기 경로
- Dart `assert` 안의 상태 변경과 Debug/Release 의미 보존
- 의미 없는 `default!`, 도달 가능한 `NotImplementedException`/`NotSupportedException`, silent no-op/fallback
- RenderObject dirty propagation, repaint boundary, layer reuse와 scene command snapshot의 수명
- scheduler timestamp 단조성, frame callback ordering, 중복 invalidation과 producer/raster backpressure
- Scrollbar/PrimaryScrollController 연결, pointer signal, drag/ballistic activity, cacheExtent, sliver child 생성·폐기
- scroll 중 semantics topology/geometry 변경량과 native element property write 수

## 3. 업그레이드 원칙

1. **Pinned Flutter를 실행 가능한 명세로 사용한다.** 각 port 파일은 Flutter source path/revision, 대응 symbol, asset과 검증 fixture를 추적한다.
2. **공용 원인을 공용 계층에서 고친다.** compiler/lowerer/runtime/framework/scene/host 문제를 DemoApp workaround나 생성된 `.g.cs` 수정으로 숨기지 않는다.
3. **누락은 조용히 성공시키지 않는다.** 지원 항목은 정확히 구현하고, 미지원 항목은 capability와 원인을 진단하며 `notVerified`/`explicitUnsupported`로 남긴다.
4. **정확성을 먼저 고정하고 최적화한다.** Flutter와 다른 상태·paint 결과를 더 빠르게 만드는 변경은 성능 개선으로 인정하지 않는다.
5. **프레임은 최신 하나를 안전하게 전달한다.** UI/framework state mutation, scene snapshot, raster/present ownership을 분리하고 소비 중인 자원을 변경하지 않는다.
6. **검증 층위를 섞지 않는다.** contract, differential, native live, physical, cross-target 결과를 따로 기록한다.
7. **대표 gate를 작게 유지한다.** 오래된 milestone script를 계속 늘리기보다 제품 시나리오와 capability별 validator로 통폐합한다.

## 4. 실행 순서

```text
FCR-0 기준선·누락 인벤토리
  ├─> FCR-1 framework asset/runtime-effect 계약
  └─> FCR-2 Dart→C# 의미 보존
          -> FCR-3 scheduler·frame ownership
              -> FCR-4 retained rendering·compositing
                  -> FCR-5 Flutter식 scroll·viewport 최적화
                      -> FCR-6 semantics 비용·즉시성
                          -> FCR-7 Material/widget 시각·상호작용 parity
                              -> FCR-8 통합 physical·soak·release evidence
```

FCR-1과 FCR-2는 병행 가능하다. FCR-3~FCR-5는 frame ownership을 먼저 고정한 뒤 진행한다. Android에서 얻은 결과를 Windows/Web/macOS 결과로 대체하지 않는다.

## 5. Milestones

### FCR-0 — Flutter conformance 기준선과 자동 누락 탐지

목표는 사람이 우연히 기능을 눌러 보기 전에 source·asset·consumer 누락을 발견하는 것이다.

작업:

- Material/Widgets/Rendering/Scheduler/Gestures/Semantics에서 다음 1차 source slice를 등록한다.
  - `ink_sparkle.dart`, `stretch_effect.dart`, `ink_well.dart`, `theme_data.dart`
  - `binding.dart`, `object.dart`, `layer.dart`, `view.dart`
  - `scrollable.dart`, `scroll_activity.dart`, `scroll_position*.dart`, `scroll_view.dart`, `scrollbar.dart`
  - `viewport.dart`, `sliver*.dart`, gesture resampler와 scheduler binding
- source별로 `source -> generated/product C# -> runtime dependency -> host consumer -> test/evidence` 연결을 기계 판독 가능한 parity matrix로 만든다.
- Flutter import와 `fromAsset` 호출, shader/font/data 파일을 스캔해 package manifest와 대조한다.
- 도달 가능한 stub/no-op, dynamic arithmetic, unobserved Future, debug/release 민감 패턴을 분류한다. 단순 문자열 검색 결과를 바로 결함으로 세지 않고 실제 Flutter 의미와 reachability를 확인한다.
- 현재 Android/Windows에서 동일 Demo 시나리오의 frame time, invalidation, dirty render/layer 수, scene command 수, allocation/GC, semantics 작업량과 input-to-present를 수집한다.
- 이전 scene-operation matrix에 의존하지 않고 현재 코드에서 필요한 항목을 다시 생성해 stale 상태를 제거한다.

산출물:

- `Doroti/validation/evidence/flutter-conformance/framework-parity-matrix.json`
- `Doroti/validation/evidence/flutter-conformance/baseline-evidence.json`
- 대표 `Doroti/eng/validate.ps1` capability gate

완료 gate:

- 선택 source의 모든 asset/runtime dependency에 owner와 disposition이 있다.
- `implemented`, `adapted`, `explicitUnsupported`, `notVerified` 외의 모호한 상태가 없다.
- 누락 asset, source hash drift, owner 없는 reachable stub이 validation을 실패시킨다.
- baseline은 측정값과 미실행 경계를 함께 기록하며 아직 성능 PASS로 승격하지 않는다.

### FCR-1 — framework asset과 runtime-effect 파이프라인 통합

현재 구현 상태 (2026-08-18): `Doroti/validation/evidence/flutter-conformance/framework-shader-manifest.json`을 공용 manifest로 두고, `Doroti.Ui.FrameworkShaderLoader`가 InkSparkle/StretchEffect의 embedded asset을 비동기 로드·hash/ABI 검증한다. StretchEffect의 C# inline shader fork는 `Shaders/stretch_effect.sksl` embedded resource로 이동했다. Skia runtime-effect cache는 adapted source SHA-256, backend, graphics-context generation을 key로 사용하며 context 재생성 때 이전 generation을 폐기한다. 지원하지 않는 backend는 투명/임의 fallback 없이 capability diagnostic과 예외를 낸다. `validate-framework-shaders.ps1`와 runtime contract는 이 경계를 검증한다.

아직 `notVerified`: cold first interaction부터 완료 후 repaint까지의 실제 GPU frame capture, 100회 반복과 context loss/recreate resource plateau, Flutter reference와 Doroti raster differential, Android physical 및 Windows live presentation.

작업:

- InkSparkle/StretchEffect의 개별 로딩을 공용 framework asset manifest와 loader로 통합한다.
- Flutter asset key, 원본 hash, Doroti 변환 산출물, uniform/sampler ABI, owning assembly, 라이선스와 target support를 manifest에 기록한다.
- shader는 build 시 포함 여부와 source pin을 검증한다. C# 문자열 복제는 제거하거나 생성 산출물로 만들고 수동 fork로 남기지 않는다.
- Flutter의 비동기 compile 의미를 보존하되 실패를 삼키지 않는다. cold start, compile 중 paint, 완료 후 repaint, dispose와 context loss를 명시한다.
- `FragmentProgram`/runtime effect cache key를 source hash + backend + graphics-context generation으로 정의하고 context 재생성 시 안전하게 폐기한다.
- unsupported backend에서는 투명하게 사라지거나 임의 효과로 바꾸지 말고 capability diagnostic을 낸다.
- InkSparkle, StretchEffect, app custom shader와 ImageFilter.shader를 같은 pipeline으로 검증한다.

완료 gate:

- 선택된 Flutter framework shader asset closure가 100%다.
- cold first interaction, 100회 반복 interaction, context recreate에서 blank effect·예외·resource 증가가 없다.
- 동일 size/DPR/color/seed 입력의 Flutter reference와 Doroti raster differential이 허용 오차 안에 든다.
- Android physical과 Windows live에서 실제 GPU presentation을 각각 증명한다.

### FCR-2 — Dart→C# 번역 의미와 runtime primitive 강화

현재 구현 상태 (2026-08-18): `Doroti/validation/fcr2-semantics/flutter-animation-fixture.dart`가 pinned Flutter `tween.dart`, `geometry.dart`, `consolidate_response.dart`에서 추출한 최소 의미 fixture와 provenance/hash를 보유한다. `DartRuntimePrimitives.LerpTweenValue`와 `IDartTweenValue<T>`를 통해 Offset/Size/Rect/Vector2/double generic Tween 산술을 typed 경로로 고정했고, `Tween<T>`의 dynamic binder를 제거했다. Future discard는 `Observe`/diagnostic sink으로 관찰하며, Future error handler와 Completer completion은 awaitable continuation으로 처리하고 timer dispose race를 유지한다. Constructor assert는 Dart assert primitive을 사용해 Debug/Release 차이를 보존한다. `validate-fcr2-semantics.ps1`와 Debug/Release runtime contract가 이 경계를 검증한다.

아직 `notVerified`: InkSparkle을 포함한 실제 `DorotiDemoApp` 60초 interaction log, 새 compiler revision으로 선택 framework 전체를 재생성한 source diff, Flutter/Doroti animation raster differential, Android physical 및 Windows live acceptance.

작업:

- 실제 Flutter source에서 추출한 최소 fixture로 다음 의미를 차등 검사한다.
  - Offset/Size/Rect/Matrix 산술과 lerp
  - `Tween<T>`/`Animatable<T>`의 generic 연산
  - `double` 정밀도와 Skia 호출 직전의 명시적 `float` 변환
  - nullable callback/result와 `return null`
  - Future completion/error/cancellation, timer dispose race
  - cascade, collection mutation, pattern/switch, assert의 Debug/Release 차이
- `System.Numerics.Vector2` 같은 host 타입에 Dart 연산을 dynamic으로 위임하지 않고 typed semantic lowering 또는 전용 value type 연산을 사용한다.
- fire-and-forget이 필요한 Future에는 예외 관찰/diagnostic 정책을 부여한다. 초기화 Future를 단순 `Ignore`로 버리지 않는다.
- framework source의 reachable runtime binder/invalid cast/도달 불가능 후행 throw를 analyzer와 runtime fixture로 잡는다.
- 수정은 compiler/lowerer/runtime primitive에 우선 적용하고, 영향을 받는 product C#을 동일 revision에서 재생성·재검토한다.

완료 gate:

- InkSparkle을 포함한 animation fixture가 Debug/Release에서 Flutter와 동일한 상태 전이를 보인다.
- 대표 앱 60초 interaction log에 예상하지 않은 `RuntimeBinderException`, `InvalidCastException`, unobserved task exception이 0건이다.
- 새 번역에서 금지 패턴이 재생성되면 CI가 source symbol과 함께 실패한다.

### FCR-3 — Flutter식 scheduler, frame coalescing과 ownership

현재 구현 상태 (2026-08-18): `DorotiFrameClock`가 native input, vsync, framework dispatch와 terminal raster의 단조 시간 기준을 제공한다. Scheduler는 transient → mid-frame microtask → persistent → post-frame 순서를 bounded trace로 남기며 stale vsync가 시간을 되돌리지 않게 한다. MAUI host는 callback queue 대신 하나의 pending request만 유지하고, raster가 늦을 때 immutable command-array latest scene만 교체한다. input sequence와 scene sequence는 submit/raster/present/replay/superseded/failed trace에 함께 보존된다. `validate-fcr3-scheduler.ps1` 및 Debug/Release fixture가 이 구조적 계약을 검증한다.

아직 `notVerified`: Flutter reference 실행과의 실제 callback trace differential, Windows resize/context recreate/foreground lifecycle stress, Android physical lifecycle stress, 그리고 60초 앱 interaction trace의 deadlock·duplicate callback·use-after-dispose·process survival acceptance.

작업:

- Flutter의 scheduler phase와 Doroti의 `scheduleFrame -> beginFrame -> drawFrame -> compositeFrame -> submit -> present` 순서를 trace로 대조한다.
- frame timestamp를 단조 시계와 native vsync 기준으로 통일하고 transient/persistent/post-frame callback ordering을 검증한다.
- 플랫폼별 동시 콜백을 framework event loop에 직렬화하되 raster/present와 긴 lock을 공유하지 않는다.
- request는 한 번만 pending으로 두고, raster가 늦으면 immutable latest scene 하나로 교체한다. 소비 중 scene/resource는 mutate/dispose하지 않는다.
- invalidation coalescing, paint 중 재요청, surface/context generation, pause/resume와 resize/DPR 전환을 state machine으로 만든다.
- frame diagnostics에 build/layout/paint/scene-build/raster/present/queue latency, dropped/superseded/replayed/failed 사유를 추가한다.

완료 gate:

- callback ordering differential과 lifecycle fixture가 Flutter 기대 순서와 일치한다.
- 강제 resize/context recreate/foreground 복귀 중 deadlock, duplicate callback, use-after-dispose와 자동 종료가 없다.
- submitted/presented/replayed 숫자뿐 아니라 각 입력 sequence가 어떤 frame에 반영됐는지 추적 가능하다.

### FCR-4 — retained rendering, repaint boundary와 compositing parity

현재 구현 상태 (2026-08-18): pinned Flutter `object.dart`/`layer.dart` symbol fixture와 Debug/Release FCR-4 contract를 추가했다. clean `Layer`는 동일 view 소유의 immutable retained command node 하나를 제출하고, dirty boundary만 다시 record하며 clean sibling은 retained node로 남는다. managed `SceneBuilder`가 in-place로 재사용한 engine handle을 다시 대입할 때 dispose하지 않도록 고쳤고, engine-layer create/dispose/active/snapshot/reuse counter가 release 뒤 baseline으로 돌아오는 계약을 추가했다. MAUI host는 retained payload를 재귀 replay하고 새 native back buffer는 app background로 clear한다. C2 texture/platform-view는 host mapping 부재 시 `NotSupportedException`으로 실패하며 silent 축약하지 않는다.

아직 `notVerified`: Flutter reference의 C0/C1 payload·grouping·GPU pixel differential, native resize/context recreate와 foreground lifecycle, Android physical scroll/animation resource soak, image/shader/filter/paragraph/offscreen surface별 balance counter, 그리고 C2 전체 owner/target matrix. 따라서 FCR-4 완료 gate는 아직 닫지 않는다.

작업:

- `RenderObject` dirty propagation, `PaintingContext`, `RepaintBoundary`, `Layer._needsAddToScene`, engine-layer reuse와 scene lifecycle을 pinned Flutter source와 symbol 단위로 대조한다.
- 매 프레임 전체 command list를 재생성·복사·재래스터하는 비용을 측정하고, 변경되지 않은 Picture/Layer를 immutable retained node로 재사용한다.
- native back buffer가 새로 생기면 마지막 완전 scene을 재구성하되, 평상시에는 damage/dirty subtree만 다시 record하도록 한다.
- layer/resource retain/release를 surface/context generation과 연결하고 image, shader, filter, paragraph와 offscreen surface의 balance counter를 둔다.
- scene-operation matrix의 C0/C1을 실제 payload·grouping·GPU raster·retained replay·reference differential까지 닫는다.
- `ColorFilter`, `ImageFilter`, `ShaderMask`, backdrop, blend/compose, saveLayer, clip과 transform 순서가 Flutter와 같은지 검증한다.
- C2의 texture/platform view/image-nine/points/vertices/atlas는 구현하거나 명시적 owner/target milestone을 유지한다. silent 축약은 금지한다.

완료 gate:

- startup, idle native repaint, active scroll, animation 중 repaint, resize와 context recreate에서 black/uncovered frame이 0이다.
- 변경 없는 subtree의 record/raster 수가 증가하지 않고 retained resource counter가 soak 후 기준선으로 돌아온다.
- C0/C1의 reference differential이 더 이상 `notVerified`가 아니며 C2 미지원은 정확한 capability 진단을 낸다.

### FCR-5 — Flutter식 scroll/viewport/sliver 경로와 성능

작업:

- 다음 causal chain을 한 trace로 연결한다.

```text
native pointer/wheel
  -> PointerData(signalKind 포함)
  -> hit test / gesture arena / drag recognizer
  -> ScrollActivity / ScrollPosition / physics
  -> viewport offset / sliver layout
  -> paint boundary / retained layer
  -> GPU present / Scrollbar / semantics
```

- Flutter의 PrimaryScrollController 선택 규칙과 Scrollbar가 같은 ScrollPosition 하나에 붙는 ownership을 복구한다. Demo의 explicit controller는 해당 계약을 시험하는 fixture로만 사용하고 영구 필수 workaround로 만들지 않는다.
- drag/hold/ballistic/driven activity, overscroll, pointer signal, touch slop, velocity와 resampling을 Flutter fixture와 대조한다.
- offset만 바뀐 프레임에서 전체 widget rebuild나 전체 semantics rebuild가 일어나지 않도록 dirty 범위를 layout/paint에 한정한다.
- `SliverChildBuilderDelegate`, cacheExtent, keepAlive, repaint boundary, semantic index와 child 생성/폐기 범위를 계측한다.
- viewport clip/paint order를 검증해 최상단 child, Ink feature와 Scrollbar가 scene에서 누락되거나 잘못 덮이지 않게 한다.
- text layout/paint, shader program, immutable paint/path/image 캐시는 key와 context 수명을 명확히 하고 hit/miss/eviction을 계측한다.
- 60/90/120 Hz에서 동일 workload를 재생할 수 있는 scroll benchmark를 만든다. 우선 Android 60 Hz physical과 Windows wheel/drag를 release gate로 사용한다.

성능 acceptance 초안 — FCR-0 baseline 후 더 엄격한 값으로 확정:

- steady 60초 scroll에서 p95 input-to-present가 2 vsync 이내이고, 2프레임을 넘는 연속 정지가 없다.
- p95 framework+raster 시간이 해당 refresh interval 이내이며 missed-frame 비율은 1% 미만이다.
- warm-up 뒤 live retained object/resource 수가 plateau를 이루고, scroll 길이에 비례하는 메모리 증가가 없다.
- frame exception, failed present, black/uncovered screenshot, 앱 process 종료가 모두 0이다.
- Flutter reference와 같은 최종 offset, visible child 범위, Scrollbar thumb와 ballistic 종료 상태를 얻는다.

완료 gate:

- Android physical 60초 alternating drag/ballistic scroll와 Windows native wheel/drag를 통과한다.
- packet 수뿐 아니라 offset, lazy lifecycle, raster, Scrollbar, top child 복귀와 process survival을 함께 증명한다.
- 최적화 전후 trace를 남기고 정확성 fixture가 동일한 상태에서만 성능 향상으로 기록한다.

### FCR-6 — semantics delta, 15 fps 제한과 상호작용 즉시성

15 fps는 continuous scroll 중 native accessibility tree 비용을 제한하는 host 정책으로 유지할 수 있지만 모든 semantics 이벤트의 고정 지연 규칙으로 사용하지 않는다.

작업:

- Flutter `markNeedsSemanticsUpdate`와 scroll semantics 생성 범위를 대조해 불필요한 framework update부터 줄인다.
- bridge는 node별 content hash/delta를 계산해 bounds/label/value/actions/flags가 바뀐 native property만 갱신한다.
- continuous geometry-only scroll은 최대 15 fps로 최신값을 coalesce한다.
- focus 이동, tap/action 대상 변화, text/selection/value, node 추가·삭제, route/screen 전환, scroll 종료는 즉시 flush한다.
- 예약 delay에 cancellation/dispose/generation fence를 두어 종료된 surface에 callback하지 않는다.
- native overlay가 ordinary touch/FAB hit test를 가로채지 않는지 계속 검증한다.
- TalkBack/UIA에서 focus order, visible bounds, scroll action과 버튼 action 왕복을 physical checklist로 만든다.

완료 gate:

- scroll 중 native semantics UI-thread 시간이 frame budget의 10%를 넘지 않도록 baseline 기반 threshold를 확정한다.
- 15 fps coalescing 중에도 focus/action/text/selection critical update는 다음 UI dispatch에서 반영된다.
- node identity 재사용, stale node 제거, action routing과 overlay pass-through가 physical에서 통과한다.

### FCR-7 — Material/Widget source slice의 시각·동작 parity

작업:

- 1차 대표 slice를 Scaffold/background, AppBar, FAB, Text, InkWell/InkSparkle, Scrollbar, ListView/SliverList, ShaderMask/ImageFilter로 고정한다.
- 동일 viewport, DPR, font, theme, locale, animation time/seed를 사용해 Flutter reference capture와 Doroti capture를 생성한다.
- golden만 보지 않고 좌표 기반 down/hold/move/up, hover, scroll과 semantics action을 재생한다.
- 기본/pressed/hovered/focused/disabled, scroll top/middle/end, animation cold/warm 상태의 pixel diff와 state trace를 함께 비교한다.
- app-owned transparent background와 framework-owned opaque Scaffold 배경을 구분해 shell 색 노출이 의도인지 결함인지 fixture에 명시한다.
- source slice가 요구하는 asset, render operation, hit-test와 semantics consumer가 matrix에서 모두 닫혀야 component를 `PASS`로 승격한다.

완료 gate:

- “표시됨”이나 전체 픽셀 변화가 아니라 pinned Flutter capture 대비 허용 오차와 원인 분류를 기록한다.
- FAB 실제 callback/상태, Ink 애니메이션, Scrollbar thumb, 상단 text와 background 지속성이 같은 입력 sequence에서 통과한다.
- Android와 Windows 결과를 별도 evidence로 보존한다.

### FCR-8 — 통합 stability, physical acceptance와 release evidence

작업:

- compact validator를 `Inventory`, `Contracts`, `Differential`, `WindowsLive`, `AndroidPhysical`, `Soak`, `Evidence` shard로 구성한다.
- `pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 validate -ValidationSuite Developer`에 빠른 representative gate를 연결하고 physical/soak는 별도 명시 실행으로 둔다.
- Android physical에서 launch, idle repaint, 60초 scroll, 100회 Ink/FAB, background/foreground, resize/rotation, context recreate와 memory/resource plateau를 수집한다.
- Windows에서 wheel/drag, resize/DPI, swap-chain replay, shader cold/warm와 process survival을 수집한다.
- Web browser live, Mac Catalyst native, screen reader/IME/stylus는 실제 실행하기 전까지 `notVerified`로 유지한다.
- evidence schema의 상위 status, target boundary와 하위 실제 결과가 모순되지 않도록 validator에서 검증한다.

완료 gate:

- scoped Android physical과 Windows live의 정확성·성능·stability gate가 모두 PASS다.
- crash/ANR/fatal log, unobserved exception, failed frame, software fallback과 resource leak이 0이다.
- Flutter pin·Doroti revision·device/OS/backend·명령·artifact hash·측정값이 evidence에 남는다.
- 실행하지 않은 target/physical 항목은 자동으로 `notVerified`이며 다른 target의 PASS로 승격되지 않는다.

## 6. 우선순위와 첫 구현 묶음

첫 작업은 아래 범위로 제한한다. 이 묶음이 끝나기 전에는 캐시를 더 추가하거나 프레임 수치만 조정하지 않는다.

1. FCR-0 inventory validator와 parity matrix를 만든다.
2. InkSparkle/StretchEffect를 공용 framework shader manifest·loader로 옮긴다.
3. 실제 Flutter animation snippet으로 Tween/Offset/Future/assert Debug·Release differential fixture를 만든다.
4. frame trace에 phase 시간, scene generation, present 결과와 input sequence id를 추가한다.
5. Android Demo scroll baseline을 다시 수집한 뒤 FCR-4/FCR-5의 최적화 지점을 결정한다.

## 7. 완료로 인정하지 않는 변경

- InkSparkle을 NoSplash/InkRipple로 바꾸거나 animation을 꺼서 예외를 피하는 변경
- Scrollbar를 숨기거나 scrolling을 제거해 hit-test 문제를 피하는 변경
- MaterialApp/Scaffold를 항상 불투명하게 만들어 scene 소실을 가리는 변경
- 모든 frame을 무조건 full replay하거나 무제한 cache해 순간 끊김만 줄이는 변경
- semantics를 끄거나 모든 action을 15 fps 뒤로 미루는 변경
- generated product source 직접 수정, broad exception swallow, forced GC, process 자동 종료
- submitted/presented count, direct callback, 단일 changed-pixel만으로 Flutter parity를 주장하는 evidence

## 8. 최종 Definition of Done

- 선택 Flutter source slice의 source/asset/runtime/host/test closure가 100%이며 silent missing dependency가 없다.
- Android physical에서 startup/scroll/Ink/FAB/Scrollbar/top text/background가 지속되고 60초 stress 후 process가 살아 있다.
- Flutter reference와 scroll state, visible child/layer, Material state transition, semantics와 raster가 허용 범위 안에서 일치한다.
- retained rendering과 cache가 correctness fixture를 바꾸지 않으면서 frame/allocation 지표를 개선한다.
- 15 fps semantics 정책은 continuous geometry update에만 적용되고 critical accessibility action은 즉시 반영된다.
- Windows live와 Android physical evidence가 각각 존재하며 Web/Mac/기타 physical 미실행 범위는 `notVerified`다.
- Developer validator와 새 compact conformance validator가 통과하고, evidence schema가 측정 결과와 모순되지 않는다.

이 계획의 종료 기준은 “Doroti가 Flutter의 모든 API를 구현했다”가 아니다. 선택한 제품 경로에서 Flutter source가 요구하는 동작을 누락 없이 보존하고, 다른 경로의 미지원·미검증 상태를 정확히 드러내며, 다음 Flutter pin 갱신 때 같은 감사를 반복할 수 있는 상태를 만드는 것이다.

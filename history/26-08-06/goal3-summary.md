# Doroti Goal3 폐기 요약

> 역사 기준일: 2026-08-06  
> 원문 제목: `Doroti 3차 목표 — Flutter Framework 전체 시스템 Clone과 Semantic Transpile 계획`  
> 적용 기간: 2026-08-04 ~ 2026-08-06  
> 처리: 루트 `goal3.md`는 제거하고, 완료 결과와 미완료 범위를 이 문서에 보존한다.  
> Goal4 역사 기록: [`goal4-summary.md`](../26-08-10/goal4-summary.md)
> 후속 Goal5 기록: [`goal5-summary.md`](../26-08-12/goal5-summary.md). 현재 active 계획은 [`goal6.md`](../../goal6.md)

## 1. 당시 목표

Goal3는 pinned Flutter `packages/flutter/lib` 전체를 resolved analyzer graph, typed Migration IR, semantic lowering, mechanical `.g.cs`, review/fix와 일반 `.cs` promotion을 거쳐 Doroti용 framework package로 만드는 계획이었다.

목표 범위는 다음 13개 public root와 그 internal/conditional/part closure였다.

- `foundation.dart`, `scheduler.dart`, `services.dart`
- `physics.dart`, `animation.dart`, `gestures.dart`
- `painting.dart`, `semantics.dart`, `rendering.dart`, `widgets.dart`
- `material.dart`, `cupertino.dart`, `widget_previews.dart`

당시 pinned Flutter census는 public root 13개, root export directive 640개, Dart file 695개와 `src` Dart file 682개였다. 생성 후보를 곧바로 제품으로 사용하지 않고 symbol 단위 review와 deterministic promotion을 통과한 일반 `.cs`만 제품 owner로 인정하는 것이 핵심 원칙이었다.

## 2. 완료된 결과

### G3-0 — Evidence truth reset

- resolved inventory와 실제 semantic generation/promotion 완료율을 분리했다.
- 과거 broad `manual-adaptation`과 marker 기반 결과를 framework 구현 완료율에서 제외했다.
- mechanical generation, reviewed generated source, reviewed source port, runtime binding과 blocker를 분리하는 evidence 기준을 세웠다.

### G3-1 — Multi-library framework semantic compiler

- 단일 F0 selection을 일반 `flutter-framework` profile로 확장했다.
- multi-library/part/private scope/import prefix/generic/constructor/extension 후보를 analyzer/IR/emitter에 연결했다.
- library dependency graph의 SCC와 deterministic generated project/package graph를 구현했다.
- unsupported framework syntax가 typed diagnostic과 non-zero exit를 내도록 했다.
- 6 resolved libraries, 16 parts, 5 SCC, 2 generated projects의 검증 selection이 compile/external consumer/determinism gate를 통과했다.

근거:

- `Doroti/migration/flutter-framework/g3-1-evidence.json`
- `Doroti/migration/selections/g3-1-framework-multilibrary.json`
- `Doroti/migration/generated-candidates/flutter-framework/56b8e1a851a594b1a154f8ea93270807dab22b9a/g3-1/`

### G3-2 — Candidate review와 `.cs` promotion pipeline

- mechanical candidate root와 product source root를 분리했다.
- review/diff/promote/rebase workflow와 `migration/promotion.json` schema를 구현했다.
- unmanifested source, candidate/product 이중 owner와 upstream conflict의 자동 overwrite를 차단했다.
- Foundation `annotations`, `object`와 Physics `tolerance`를 초기 pipeline 검증 대상으로 일반 `.cs`로 승격했다.
- clean/incremental/mirror determinism, product compile/API/behavior equivalence와 warning/error 0 gate를 통과했다.

근거:

- `Doroti/migration/flutter-framework/g3-2-evidence.json`
- `Doroti/migration/promotion.json`
- `Doroti/tools/Doroti.SourceTools/`
- `Doroti/validation/Doroti.Validation.Compiler/G3PromotionValidation.cs`

### G3-B0 Foundation 파일럿

다음 6개 library의 candidate generation, review/promotion, promoted compile, notifier parity와 external consumer가 완료됐다.

- `annotations`
- `object`
- `key`
- `change_notifier`
- `collections`
- `observer_list`

reviewed source는 `Doroti/src/Doroti.Flutter.Framework.Foundation/`에 남아 있다. 이 결과는 Foundation 전체 완료가 아니라 제한된 파일럿 완료다.

근거:

- `Doroti/migration/flutter-framework/g3-b0-evidence.json`
- `Doroti/migration/flutter-framework/g3-b0-parity.json`
- `Doroti/migration/selections/g3-b0-foundation-pilot.json`
- `Doroti/validation/generated/g3-b0-foundation-consumer/`

### Compiler 고도화

Foundation 후속 batch를 생성하기 위해 다음 지원이 추가됐다.

- enum emission
- `bool.fromEnvironment`와 host constant mapping
- factory redirect와 private companion type mangling
- Dart indexer 병합, top-level getter와 constructor initializer
- import prefix와 receiver property 구분
- `enum.index`, Dart `int`, shift count와 generic constraint 보정
- per-library analyze/lower/emit 병렬 처리
- CLI `--parallelism`/`-j`와 deterministic parallel publish

## 3. 생성됐지만 완료되지 않은 결과

Foundation batch 2의 다음 8개 library는 semantic generation과 candidate compile까지만 완료됐다.

- `unicode`
- `constants`
- `service_extensions`
- `capabilities`
- `_capabilities_io`
- `node`
- `bitfield`
- `_bitfield_io`

이 candidate는 review/promotion, behavior parity와 product consumer 확장이 완료되지 않았다. 후속 계획은 이 결과를 폐기하지 않지만 새 Flutter/Avalonia ownership boundary로 다시 검토한다.

근거:

- `Doroti/migration/selections/g3-b0-foundation-batch2.json`
- `Doroti/migration/generated-candidates/flutter-framework/56b8e1a851a594b1a154f8ea93270807dab22b9a/g3-b0-b2/`

Foundation 전체 selection은 30 libraries/193 declarations 기준으로 작성됐지만 `milestoneComplete`는 `false`였다. `platform`, diagnostics, binding, isolates, timeline, memory allocations 등을 포함한 남은 library와 full API/behavior parity는 완료되지 않았다.

## 4. 시작하지 않았거나 완료되지 않은 범위

다음 subsystem 계획은 제품 완료로 판정되지 않았다.

- Scheduler와 Services 전체 package
- Physics, Animation과 Gestures 전체 package
- Painting, Semantics와 Rendering 전체 package
- Widgets와 Widget Previews
- Material/Cupertino primitive, demo closure와 전체 public export
- 13개 root/695-file full framework completion
- 일반 Dart application compiler의 full framework 합류
- framework NuGet/package-only external consumer와 release
- Flutter upstream rebase
- physical Linux/macOS, real IME/accessibility와 전체 target gate

과거 F1-F4 closure와 handwritten compatibility source는 source inventory 또는 제한된 vertical slice 근거일 뿐 위 항목의 구현 완료 증거가 아니다.

## 5. 폐기 사유

Goal3는 Flutter framework와 저수준 실행 계층 사이의 source ownership을 충분히 분리하지 않았다. Flutter Engine 전체를 범위에 넣지는 않았지만 `dart:ui`, VM/platform binding과 기존 Doroti runtime/backend가 하나의 넓은 `runtime-binding` 영역으로 표현됐다. 그 결과 다음 의도가 명확하지 않았다.

- Flutter에서는 `packages/flutter/lib`의 framework 의미를 가져온다.
- Flutter Engine/embedder/native platform 구현은 가져오지 않는다.
- window, shell, dispatcher, frame clock, input, IME, clipboard, accessibility, DPI와 graphics surface의 concrete 구현은 Avalonia source-port가 소유한다.
- Flutter framework의 저수준 호출은 managed `dart:ui` 호환 계약과 typed bridge를 통해 Avalonia source-port에 연결한다.
- Avalonia Controls/Layout/Styling은 Flutter Widget/Rendering/Material/Cupertino의 대체 UI framework가 아니다.

이 경계 변경은 남은 milestone의 순서, package graph, runtime binding disposition과 validation source of truth에 영향을 주므로 Goal3 원문을 부분 수정하지 않고 폐기했다.

## 6. 후속 계획으로 인계하는 자산

다음 결과는 유효하며 계속 사용한다.

- analyzer protocol/Migration IR v3와 typed semantic lowering
- multi-library framework compiler와 SCC/project graph
- deterministic `.g.cs` candidate generation
- review/diff/promote/rebase tooling과 promotion manifest
- promoted Foundation 파일럿 source와 제한된 Physics validation source
- Foundation batch 2 mechanical candidate
- compiler/API/behavior evidence를 구분하는 validation 원칙
- Flutter source pin, census와 source map/provenance artifact

다음 항목은 새 계획에서 재판정한다.

- `Doroti.Flutter.Runtime`의 platform reference와 clipboard/channel helper
- `Doroti.Platform` 안의 Flutter-derived behavior
- handwritten `Doroti.Rendering`/`Doroti.Widgets` owner
- broad `runtime-binding` disposition
- Flutter Scheduler/Services/Rendering과 Avalonia source-ported shell/GPU의 실제 연결

이 역사 문서는 Goal3의 작업 지시서가 아니다. Goal4까지의 이력은 [`goal4-summary.md`](../26-08-10/goal4-summary.md), Goal5의 종료 기록은 [`goal5-summary.md`](../26-08-12/goal5-summary.md)에 보존하며, 현재 구현 기준선과 다음 단계는 루트 [`goal6.md`](../../goal6.md)를 따른다.

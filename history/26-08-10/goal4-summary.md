# Doroti Goal4 작업 기록

> 문서 상태: 역사 기록 — active 계획이 아님
> 정리일: 2026-08-10
> 후속 계획: [`goal5-summary.md`](../26-08-12/goal5-summary.md). 현재 active 계획은 [`goal6.md`](../../goal6.md)
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> Avalonia source pin: `f159423f691946e713f454447a780d4677d8a0d2`

이 문서는 삭제한 `goal4.md`에 누적되어 있던 결정, 구현 결과, 실패와 미검증 항목을 보존한다. 과거 시점의 PASS와 현재 작업 트리에서 다시 확인한 PASS를 구분한다. 최신 실행 결과가 기존 evidence와 충돌하면 최신 실행 결과를 다음 계획의 진입 기준으로 사용한다.

## 1. Goal4가 확정한 구조

Goal4의 핵심 결정은 Flutter 전체 stack을 복제하지 않고 두 source family의 소유권을 분리하는 것이었다.

```text
Dart application
  -> reviewed Flutter framework C#
  -> managed dart:ui-compatible contract
  -> Flutter-Avalonia host bridge
  -> Avalonia source-ported native/platform implementation
  -> operating system
```

- Flutter `packages/flutter/lib`가 framework API와 Scheduler, Services, Gestures, Painting, Rendering, Semantics, Widgets 동작을 소유한다.
- Avalonia source-port가 window, dispatcher, frame clock, native input, IME, clipboard, accessibility, DPI와 GPU surface를 소유한다.
- `Doroti.Flutter.Runtime`은 Dart 언어/runtime 의미만 소유한다.
- `Doroti.Flutter.Ui`는 managed `dart:ui` API와 host-neutral capability contract를 소유한다.
- bridge/glue는 DTO 변환, marshalling, 좌표 변환과 lifetime handoff만 수행한다. framework 알고리즘이나 OS별 native 정책의 두 번째 owner가 될 수 없다.
- Flutter Engine/embedder/native platform source와 Avalonia Controls/Layout/Styling/XAML은 제품 Flutter UI 경로에 포함하지 않는다.

## 2. 완료한 기반

### G4-0 — Framework/platform 경계 재분류

2026-08-07에 완료로 기록했다.

- Flutter public root 13개, export directive 640개, Dart file 695개의 census를 고정했다.
- external/native boundary 1,608개를 분류하고 unclassified 0을 만들었다.
- 260개 `avalonia-binding` occurrence를 13개 capability에 연결했다.
- source boundary, capability map, current owner audit와 forbidden-edge 검증을 도입했다.

주요 근거:

- `Doroti/migration/flutter-avalonia/source-boundary.json`
- `Doroti/migration/flutter-avalonia/capability-map.json`
- `Doroti/migration/flutter-avalonia/current-owner-audit.json`
- `Doroti/docs/adr/ADR-018-flutter-avalonia-boundary.md`

### G4-1 — Managed dart:ui contract와 Avalonia binding ABI

2026-08-07에 완료로 기록했다.

- `Doroti.Flutter.Runtime`에서 concrete platform owner를 제거했다.
- `Doroti.Flutter.Ui`에 view, dispatcher, metrics, lifecycle, input, message, semantics와 graphics resource contract를 구성했다.
- `Doroti.Flutter.Hosting`과 Windows `Doroti.Host.Desktop.Flutter` adapter를 연결했다.
- host 부재 시 typed capability error로 실패하고, two-view state와 resource exactly-once를 검증했다.
- framework/host/vendor 사이의 금지 의존과 public native type leak을 검사했다.

### G4-2 — Foundation 경계 교정과 promotion

2026-08-08에 완료로 기록했으며 현재 `g4-2-evidence.json`의 모든 gate도 true다.

- Foundation 30개 library, selection target 192개를 reviewed product source 또는 명시적 runtime/UI owner로 분류했다.
- target disposition은 `flutter-framework` 174, `dart-runtime` 13, `dart-ui-contract` 5이며 unowned/unsupported는 0이었다.
- Runtime/Core의 중복 notifier, collection, diagnostics owner를 제거했다.
- behavior differential과 repository 밖 package-only consumer를 검증했다.

### G4-3/G4-3A — Scheduler·Services와 compiler/API closure

2026-08-08 시점에는 완료로 기록했다.

- Scheduler/Services 58개 library, 287개 declaration, 2,426개 member를 생성했다.
- analyzer closure, switch/local function/constructor reference/function type/async/nullability lowering을 보강했다.
- aggregate generated solution과 package-only consumer를 clean build했다.
- 233개 unique public declaration name을 review하고 product Scheduler/Services source로 승격했다.
- frame, metrics/lifecycle, input/text/IME, clipboard/cursor/channel managed trace를 통과했다.

다만 이 완료 표시는 현재 기준선으로 그대로 사용할 수 없다. 2026-08-10 최신 compiler로 candidate와 결정성을 다시 검증한 결과 generated solution은 warning/error 0으로 빌드되었지만, 제품 promotion API 경계가 `missing 205 / extra 24 / product .g.cs 0`으로 실패했다. `g4-3-evidence.json`의 기존 `success: true`는 과거 통과 기록이며, Goal5에서는 API manifest diff 0을 다시 증명하기 전까지 G4-3을 재검증 필요 상태로 다룬다.

## 3. 부분 완료 작업

### G4-4 — Physics·Animation·Gestures

역사적으로 달성한 항목:

- Physics/Animation/Gestures 42개 library와 353개 declaration을 생성했다.
- 한 시점의 aggregate는 1,709개 member, unclassified/silent omission 0, warning/error 0으로 빌드됐다.
- reviewed product source 353/353, public declaration 259개, public member 973개를 승격했다.
- gesture arena, pointer signal, spring/curve, ticker와 package-only consumer managed gate를 통과했다.
- handwritten animation owner를 제거하고 arena/router/signal entrypoint를 reviewed Gestures 구현으로 전환했다.

끝내 닫지 못한 항목:

- `Doroti.Widgets`의 `TapGestureRecognizer`/`VerticalDragGestureRecognizer` compatibility symbol과 lifecycle adapter가 남았다.
- 실제 Avalonia 창의 mouse/touch/trackpad capture loss, native timestamp와 logical coordinate 정밀도 evidence를 수집하지 않았다.
- native input, sustained frame, GPU/DPI 항목은 `notVerified` 상태였다.

현재 재검증 상태:

- `g4-4-compiler-gate.json`은 42 outputs, 353 declarations, 1,709 members, 56,453 AST nodes를 기록한다.
- unclassified AST, silent omission, generated syntax error는 모두 0이다.
- 현재 aggregate는 27개 C# compiler error로 실패한다.
- 따라서 과거 `g4-4.json success: true`는 managed promotion/behavior 통과 기록일 뿐 현재 candidate, handwritten-owner-zero 또는 native evidence의 성공을 의미하지 않는다.

### G4-5 — Painting·Rendering·Semantics

역사적으로 달성한 항목:

- Painting 48, Rendering 48, Semantics 5의 101-library/663-declaration selection을 고정했다.
- 6,206 members와 257,021 typed AST nodes를 생성했다.
- unclassified AST, silent omission과 generated syntax error를 0으로 유지했다.
- 초기 Painting aggregate 111 errors에서 lowering/runtime 계약을 반복 보강했다.
- 중간 `final-30` 기록에서는 Painting/Semantics가 clean이고 Rendering에 57 errors가 남았다.
- promotion, handwritten Rendering owner cutover, Avalonia.Skia/Automation bridge는 시작 전 상태로 유지했다.

현재 재검증 상태:

- 최신 `g4-5-compiler-gate.json`은 101 outputs, 663 declarations, 6,206 members와 typed coverage를 다시 확인했다.
- aggregate error는 4개다: `CS1503` 2, `CS0173` 1, `CS1977` 1.
- Painting/Rendering/Semantics aggregate verified flag는 모두 false이며 promotion/cutover는 `not-verified-goal4-owned`다.
- 기존 `g4-5.json`의 111-error blocker 문구는 stale history다. 새 계획은 최신 4-error gate를 출발점으로 사용한다.

## 4. 후속 compiler 성능·구조 개선

Goal4 후반의 대형 selection을 다시 다룰 수 있도록 2026-08-10에 compiler W0-W6 작업을 수행했다.

- invocation/library phase, cache/process/context, allocation, working set과 partial publish telemetry를 추가했다.
- 10,124-line `FrameworkCSharpLowerer`를 책임별 partial 파일로 분리했다.
- immutable semantic/AST index와 per-library lowering session을 도입하고 반복 전역 scan을 제거했다.
- analyzer를 single-process/single-context batch로 바꾸고 single/batch byte identity를 검증했다.
- analyzer cache를 Brotli v2, dependency hash, corruption quarantine와 atomic publish 구조로 교체했다.
- Migration IR fragment와 final JSON streaming, 생성 C# 즉시 staging으로 대형 객체 lifetime을 줄였다.
- elapsed time을 1차 gate, memory를 2차 관찰 지표로 정했다.

최종 quick 측정:

| 항목 | W0 기준 | 현재 | 결과 |
|---|---:|---:|---:|
| G4-5 warm end-to-end | 846,689 ms | 13,613 ms | 62.2x |
| C# lowering/printing | 831,283 ms | 3,223 ms | 257.92x |
| analyzer cache payload | 389,124,078 B | 약 14.5 MB | 96.3% 감소 |
| peak working set | 4,171,014,144 B | 3,061,055,488 B | 26.6% 감소, 비차단 |

warm G4-5는 cache hit 101/101, Dart process/context 0이었다. 전체 `j1/2/4/8/16 x 3` 장기 matrix는 실행하지 않았고 quick evidence만 존재한다.

주요 근거:

- `Doroti/artifacts/validation/dart-to-csharp-performance.json`
- `Doroti/eng/measure-dart-to-csharp.ps1`
- `tools/Doroti.DartToCSharp/src/Diagnostics/CompilerTelemetry.cs`
- `tools/Doroti.DartToCSharp/src/Frontend/Dart/AnalyzerSession.cs`
- `tools/Doroti.DartToCSharp/src/Frontend/Dart/AnalyzerCacheStore.cs`

## 5. 현재 판정표

| 영역 | 과거 기록 | 2026-08-10 현재 판정 | Goal5 인계 |
|---|---|---|---|
| G4-0 boundary | PASS | 별도 회귀 없음 | 유지·회귀 검증 |
| G4-1 UI/host ABI | PASS | 별도 회귀 없음 | 유지·회귀 검증 |
| G4-2 Foundation | PASS | evidence gates true | 유지·회귀 검증 |
| G4-3 candidate | PASS | 58-library build/determinism PASS | 유지 |
| G4-3 promotion API | PASS | missing 205, extra 24 | 최우선 복구 |
| G4-4 candidate aggregate | PASS 이력 | 27 errors | 0 error로 복구 |
| G4-4 managed promotion/behavior | PASS 이력 | 현재 candidate와 함께 재검증 필요 | 재검증 |
| G4-4 compatibility owner | 미완 | 미완 | 제거 |
| G4-4 native input | notVerified | notVerified | 실기 evidence |
| G4-5 typed generation | PASS | PASS | 유지 |
| G4-5 aggregate | 부분 PASS 이력 | 4 errors, partitions unverified | 0 error |
| G4-5 promotion/Rendering cutover | 미착수 | 미착수 | 구현 |
| Skia/ACK/device-loss/Automation | notVerified | notVerified | 구현·실기 evidence |
| compiler performance | 장시간 blocker | quick elapsed gate PASS | full matrix 고정 |

## 6. 보존할 교훈

- candidate generation, aggregate build, reviewed promotion, product cutover, managed behavior와 native evidence는 서로 대체할 수 없다.
- evidence JSON의 과거 `success: true`보다 현재 validator 실행 결과를 우선한다.
- generated text 후처리, silent body omission, product-only alias로 compiler 오류를 숨기지 않는다.
- 공용 compiler 변경은 focused fixture뿐 아니라 G3-1, G4-3, G4-4, G4-5 전체 selection을 다시 생성해야 한다.
- hardware, GPU, DPI, accessibility와 cross-OS gate는 실제 target 실행 전까지 `notVerified`로 둔다.
- compiler performance는 elapsed time을 차단 기준으로 사용하고 memory는 회귀 관찰 지표로 기록한다.
- 제품 promotion 전에 API manifest, duplicate owner와 product `.g.cs` 유입을 반드시 다시 감사한다.

Goal4의 장기 설명과 세션별 인계는 이 문서로 종료했다. 후속 Goal5도 [`goal5-summary.md`](../26-08-12/goal5-summary.md)로 종료·보존했으며, 현재 active roadmap은 [`goal6.md`](../../goal6.md)다.

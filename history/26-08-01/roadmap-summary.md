# Doroti 로드맵 1–3 통합 요약

> 역사 기준일: 2026-08-01  
> 요약 대상: 당시 루트에 있던 `roadmap1.md`, `roadmap2.md`, `roadmap3.md`의 통합 기록. 세 원본 문서는 이 요약 작성 후 제거됐다.  
> 문서 성격: 당시 독립 플랫폼 엔진 전략으로 진행한 R1–R11의 결정, 구현 결과와 미완료 게이트를 보존하는 역사 기록  
> 현재 방향: 루트 [`goal.md`](../../goal.md)의 Avalonia Host-first 구성이 이후 작업의 기준이다.

## 1. 당시의 목표와 전략

Doroti는 고정 Flutter revision의 공개 API와 관찰 가능한 동작을 C#/.NET에서 재현하고, Dart package를 결정적으로 C# package로 변환하는 독립 UI runtime을 목표로 했다. Flutter는 API·lifecycle·behavior의 읽기 전용 기준으로 사용했다.

플랫폼 계층은 Avalonia 전체를 사용하지 않고 Win32, framebuffer, WGL/OpenGL과 Skia 구현의 일부만 `copy`, `adapt`, `rewrite`, `exclude`로 분류해 내부 vendor assembly에 이관했다. Doroti가 Widget/Element/RenderObject, scheduler, DisplayList, compositor와 공개 API의 수명을 소유하고, Avalonia UI/property/layout/compositor와 Flutter engine/Dart VM은 제품 의존성에서 제외하는 전략이었다.

핵심 실행 경로는 다음과 같았다.

```text
Dart package
  → Doroti.DartToCSharp
  → versioned Migration IR + generated C# package
  → Doroti.FlutterCompat
  → Widget / Element / RenderObject
  → immutable DisplayList / LayerTreeSnapshot
  → bounded frame lifecycle
  → native platform backend + GPU/software present
```

## 2. 로드맵 1 요약 — 도구, 계약과 렌더링 기반

로드맵 1은 기능 구현보다 먼저 반복 가능한 감사·변환·검증 도구와 아키텍처 경계를 세웠다.

| 단계 | 결과 |
|---|---|
| R1 | `eng/doroti.ps1` 공통 진입점, SourceTools, analyzer, Dart→C# 초안 변환기, SceneLab과 versioned artifact 기반 구축 |
| R3 | .NET 10 solution, warnings-as-errors, deterministic build와 Core/Rendering/Widgets/Platform/Backend/Vendor 경계 구축 |
| R4 | Avalonia의 선택된 Win32/Skia 소스를 내부 vendor slice로 개조하고 naked Win32 window, DPI, surface와 GPU/software fallback 구현 |
| R5 | immutable DisplayList/LayerTreeSnapshot, bounded mailbox, raster-thread ownership, surface generation, resource lease와 exactly-once ACK 구현 |

고정한 불변 규칙은 다음과 같다.

- UI thread만 Widget, Element와 RenderObject tree를 변경한다.
- raster thread만 Skia context와 GPU object를 소유한다.
- stale surface generation의 frame은 present하지 않고 terminal ACK를 정확히 한 번 완료한다.
- 상위 계층은 HWND, Skia 또는 vendor 타입을 알지 않는다.
- 선택된 외부 소스는 source/adapted hash, dependency closure, license, provenance와 patch를 기록한다.
- generated source는 hand-written runtime을 덮어쓰지 않고 별도 workspace에서 검증한다.

## 3. 로드맵 2 요약 — UI 코어와 첫 상호작용 앱

로드맵 2는 RenderObject부터 Widget lifecycle과 실제 입력까지 작은 수직 절편으로 확장하고, 각 runtime 단계에 compiler 단계를 함께 붙였다.

| 단계 | Runtime 결과 | Compiler 결과 |
|---|---|---|
| R6 / C1 | constraints 기반 layout/paint, RenderView, basic boxes, paragraph snapshot과 software/GPU golden | constructor, named/optional/required parameter, generic/inheritance와 render fixture |
| R7 / C2 | Widget/Element/State, BuildOwner, reconciliation, Key/GlobalKey와 lifecycle | callback/lambda, Widget/State/Key 상속과 generated lifecycle fixture |
| R8 / C3 | Win32 pointer/keyboard, capture/cancel, gesture arena, 기본 widget과 interactive Counter | app entry, const object graph, callback과 generated Counter package |

2026-08-01 판정에서 R6–R8과 C0–C3는 완료로 기록됐다. 실제 WGL/OpenGL GPU present, managed software 비교, 125%/200% multi-DPI 이동, hand-written/generated Counter의 state·render·frame/ACK trace와 반복 resource balance가 검증됐다. touch/pen, 고급 text/IME, 전체 gesture catalog와 accessibility는 후속 범위로 남았다.

## 4. 로드맵 3 요약 — 앱 기능, compiler/package와 플랫폼 확장

로드맵 3은 실사용 앱 기반, 외부 배포 가능한 compiler SDK와 두 번째 플랫폼 검증을 목표로 했다.

### R9와 C4

- scrolling/virtualization, animation, focus/shortcut, clipboard, text/IME, image, semantics와 navigation을 하나의 hand-written/generated 앱에 합류시키는 단계다.
- C4 async/navigation 수직 절편과 public platform-port diagnostic은 구현됐다.
- lazy fixed-extent viewport, animation/focus 기반, Unicode grapheme editing, Win32 clipboard/IME 연결점, image cache, semantics snapshot과 overlay/route 골격이 추가됐다.
- `DOT-0033`과 `DOT-0040`의 자동 gate는 만족했지만 R9 전체는 완료되지 않았다.

R9에 남은 실제 종료 조건:

- performance p95/p99와 resource target capture
- HarfBuzz급 shaping/font fallback
- 실제 한글 IME·clipboard와 multi-DPI caret/candidate 검증
- `WM_GETOBJECT` UI Automation/Narrator 연결
- image/device-loss 복구
- R9A/R9B 기능과 C4 generated real-app의 전체 conformance

### R10과 C5

- 실제 Dart package graph, lockfile, import/export/part/conditional import와 deterministic NuGet 생성을 구현했다.
- A 2개, B 1개 package 실행과 C 등급 public-port diagnostic, local-feed external consumer와 template/GPU smoke를 구성했다.
- clean Windows VM, injected mapped-crash 추적과 release 성능/resource capture는 `not-verified`로 남아 beta 배포 승격을 보류했다.

### R11

- Windows에서 만든 공통 계약을 두 번째 플랫폼에서 다시 검증하는 단계였다.
- backend가 Widgets를 참조하지 않고, 공통/public API에 vendor 타입을 노출하지 않으며, 실제 장치 결과와 미검증 결과를 분리하는 것을 완료 기준으로 삼았다.
- 플랫폼 선택과 실장비 검증은 완료되지 않은 후속 범위다.

## 5. 공통 품질·검증 원칙

세 로드맵은 다음 원칙을 공통 gate로 사용했다.

- `doctor`, `build`, `test`, `audit`, `format`의 공통 진입점
- warnings-as-errors, nullable, deterministic build와 generated output byte equality
- reference Flutter, hand-written Doroti와 compiler-generated 결과의 API/behavior 비교
- managed/Skia software golden과 실제 GPU readback 비교
- frame ordering, coalescing, cancellation, stale generation과 exactly-once ACK 검증
- 실제 Windows의 input, DPI, resize, minimize/restore, GPU fallback과 반복 create/dispose 검증
- 실행하지 않은 native·시각·패키지·실장비 결과는 `pass`가 아닌 `not-verified`로 기록
- 외부 source selection, dependency closure, hash, license와 provenance 감사

## 6. 당시 전략의 성과와 한계

성과:

- Doroti-owned runtime, rendering, compiler와 FlutterCompat 경계를 실제 코드와 analyzer로 구축했다.
- low-level Win32/OpenGL/Skia부터 generated Counter와 NuGet package까지 end-to-end 수직 절편을 만들었다.
- compile 성공과 실제 runtime/visual/resource 검증을 분리하는 증거 체계를 만들었다.

한계:

- Avalonia의 플랫폼 셸을 사용하지 않고 일부 내부 코드를 이관했기 때문에 Window, dispatcher, IME, accessibility, clipboard, packaging과 OS lifecycle을 Doroti가 계속 직접 확장해야 했다.
- Avalonia `WindowImpl` 수준의 코드는 Controls, Input, Threading, Composition 등의 큰 dependency closure를 가지므로 선택적 source 이관만으로 완전한 플랫폼 셸을 빠르게 확보하기 어렵다.
- 실사용 UI와 Material/Cupertino보다 플랫폼 기반을 직접 구현·검증하는 작업 비중이 커졌다.

## 7. 이후 방향으로의 인계

이번 구성 변경은 기존 runtime과 증거를 폐기하는 작업이 아니다. 다음 자산은 그대로 유지한다.

- Doroti Core/Rendering/Widgets/FlutterCompat/compiler의 공개·내부 계약
- DisplayList, frame/resource ACK, behavior fixture와 deterministic artifact
- 현재 native Win32 backend의 conformance·fallback 가치
- vendor selection/provenance와 실제 runtime 검증 체계

대신 플랫폼 셸의 기본 구현은 Avalonia package를 사용하는 `Doroti.Host.Avalonia`로 전환한다. 기존 로드맵의 “Avalonia package/Base/Controls 의존성 0건” 규칙은 역사적 결정이며, 현재 기준은 [`goal.md`](../../goal.md)의 “Avalonia 의존성은 Host project에만 허용하고 Doroti 공개·공통 계층에는 누출하지 않는다”는 규칙으로 대체한다.

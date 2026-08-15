# Doroti Goal7 종료 요약 — 제품 정확성 closure와 Windows/macOS shell·Web release

> 상태: **활성 roadmap 종료(부분 완료)** — G7-0~G7-3은 보존된 evidence 기준 `PASS`, G7-4~G7-6은 완료로 주장하지 않음
> 작성일: 2026-08-14
> 정리일: 2026-08-16
> 기준 Doroti revision: `5379137447162adb2957212ea2f336894effe05e` + 종료 시점 작업 트리
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> 이전 기록: [`goal6-summary.md`](../26-08-14/goal6-summary.md)

## 1. 문서 성격

이 문서는 삭제한 루트 `goal7.md`의 완료 결과, 부분 결과와 미실행 gate를 압축해 보존하는 역사 기록이다. 새로운 active roadmap이나 실행 지시서가 아니다.

Goal7은 동일한 일반 C# `DorotiDemoApp`을 Windows `win-x64`, Apple Silicon macOS `osx-arm64`와 Blazor WebAssembly `browser-wasm`으로 build/publish/run하는 제품 경계를 목표로 했다. Flutter SDK와 Dart project는 framework 생성 및 reference differential에만 사용하고, 사용자 제품 경로는 표준 .NET SDK와 배포된 Doroti template/package만 사용하도록 고정했다.

## 2. 고정된 제품·검증 원칙

- Flutter source가 widget/build/layout/paint/state/semantics policy의 단일 owner다.
- Windows/macOS/Web host는 window/document, scheduler, GPU surface, input/text/clipboard/accessibility/resource capability만 소유한다.
- Windows와 macOS는 공용 `Doroti.Shell.Core` 계약을 사용하고 target-specific native composition만 분리한다.
- Web은 Blazor WebAssembly host와 SkiaSharp WebGL2 canvas를 사용하되 Razor/DOM/JavaScript에 Doroti widget policy나 별도 visual widget tree를 만들지 않는다.
- generated `.g.cs` 직접 수정, filename/local-number rewrite, widget type 대체와 software fallback은 제품 수정 또는 strict-GPU 성공으로 인정하지 않는다.
- 검증은 `structural`, `live`, `reference`, `acceptance` 네 종류로 핵심화한다. 반복 안정성·resource·성능은 통합 release run 한 번에서 측정한다.
- 자동화, 수동 관찰, physical 확인과 target별 결과를 서로 대신하지 않는다. 실행하지 않은 항목은 `notVerified`로 유지한다.

## 3. 완료로 보존하는 범위

| Milestone | 상태 | 보존 근거 |
| --- | --- | --- |
| G7-0 | `PASS` | carry-over 분류, 금지 패턴 query, Windows strict-GPU product smoke, Release build — `g7-carryover.json`, `validate-g7-baseline.ps1` |
| G7-1V/I/C | `PASS` | Material reference/generation, Windows native input causal trace, compositing/retained/focus closure — `g7-material-reference-evidence.json`, `g7-native-interaction-evidence.json`, `g7-compositing-evidence.json` |
| G7-2 | `PASS` | Cupertino/adaptive, generated Dart product parity와 package-only Windows consumer — `g7-cupertino-adaptive-evidence.json`, `g7-generated-demo-evidence.json` |
| G7-3M | `PASS` | Apple Silicon NSWindow strict-GPU live, input/text/clipboard/NSAccessibility, 반복 `osx-arm64` package publish — `g7-macos-shell-evidence.json` |
| G7-3N | `PASS` | Doroti naming closure, 전 target producer/consumer graph와 Windows representative live — `g7-doroti-naming-evidence.json` |
| G7-3V/A/B/C | `PASS` | Blazor/Skia browser capability와 build graph, 동일 C# app의 desktop/browser build, `doroti-app` package-only acceptance와 반복 publish identity — `g7-web-build-evidence.json` |

G7-3의 Web `PASS`는 build/static artifact graph까지의 결과다. browser first frame, 전체 interaction, ARIA와 lifecycle을 자동으로 증명한 결과로 확장하지 않는다.

## 4. 부분 결과로 보존하는 G7-4

2026-08-15 수동 Chromium smoke에서 공식 `browser-wasm` publish artifact의 다음 결과를 관찰했다.

- non-empty GPU canvas
- logical size와 physical backing-store DPR 분리
- bounded backdrop blur(`sigmaX=12`, `sigmaY=6`)
- desktop과 같은 ambient/spot 2-pass shadow
- semantics tree
- pointer 입력에 따른 FAB 상태 변화(`24 → 27`)
- 해당 origin의 console error 0

이 결과가 증명하는 범위는 `presented`와 기본 pointer 경로의 수동 관찰뿐이다. 다음 항목은 완료로 승격하지 않는다.

- wheel, keyboard, composition과 clipboard causal trace
- drag/capture와 coalesced high-rate move의 공용 C# normalization 합류
- resize/DPR, background/foreground와 reload lifecycle 회복
- DOM/ARIA semantics action 왕복
- pinned Flutter Web behavior/reference differential
- physical IME와 screen-reader

## 5. 미완료·`notVerified`로 보존하는 범위

### G7-4 — Web live product parity

자동화된 canvas attach → framework mount/layout/paint → GPU present → terminal frame ACK trace와 대표 입력·semantics·resource/plugin scenario가 남아 있다. 다음 계획 산출물은 종료 시점에 존재하지 않았다.

- `Doroti/migration/web/g7-web-live-evidence.json`
- `Doroti/migration/web/g7-web-browser-matrix.json`
- `Doroti/eng/validate-g7-web-live.ps1`

### G7-5 — 통합 release integrity, stability와 performance baseline

Windows/macOS/Web의 동일 source/app identity를 하나의 release manifest로 묶고, target별 통합 scenario에서 stability, frame baseline, memory/resource balance, static hosting, hash/license/provenance/SBOM과 independent rebuild를 확인하는 작업은 미완료다. 다음 계획 산출물은 종료 시점에 존재하지 않았다.

- `Doroti/artifacts/g7-release/<version>/`
- `Doroti/migration/releases/g7-release-evidence.json`
- `Doroti/eng/validate-g7-release.ps1`

### G7-6 — 최소 physical release acceptance

다음 physical checklist는 모두 `notVerified`다.

- packaged Windows의 mouse, keyboard, Korean IME와 대표 accessibility action
- packaged macOS의 mouse/trackpad, keyboard, Korean IME와 대표 VoiceOver action
- Chromium Web의 mouse, keyboard, Korean IME/clipboard와 대표 screen-reader action

`Doroti/migration/targets/g7-target-matrix.json`은 존재하지만 physical 결과를 완료로 만들지 않는다. `Doroti/artifacts/g7-physical/<target>/`과 `Doroti/eng/validate-g7-acceptance.ps1`은 종료 시점에 존재하지 않았다.

## 6. target별 종료 상태

| Target | build/package | automated live | manual presented | physical acceptance |
| --- | --- | --- | --- | --- |
| `win-x64` | `PASS` | 대표 strict-GPU/input 범위 `PASS` | predecessor/Goal7 evidence 보존 | `notVerified-G7-6` |
| `osx-arm64` | `PASS` | NSWindow/GPU/input/text/clipboard/accessibility 대표 범위 `PASS` | G7-3M evidence 보존 | `notVerified-G7-6` |
| `browser-wasm` | `PASS` | `notVerified-G7-4` | GPU canvas와 기본 pointer 수동 확인 | `notVerified-G7-6` |

Linux, Intel macOS(`osx-x64`), Firefox/WebKit, touch, multi-monitor, 모든 component별 native 전수 검사, 모든 DPI/effect 곱집합과 장시간 GPU soak는 Goal7 완료 범위 밖의 후속 target matrix로 남긴다.

## 7. 남은 작업을 재개할 때의 순서

```text
G7-4 Chromium live product automation
  -> G7-5 integrated release evidence
  -> G7-6 physical acceptance
```

재개 시 문서에 적혀 있던 명령 자체를 실행 가능하거나 `PASS`인 것으로 간주하지 않는다. 먼저 누락된 validator/harness와 evidence schema를 구현하고, 각 target의 실제 결과를 기록해야 한다. 하위 공용 의미 결함이 발견되면 generated product나 target별 widget patch가 아니라 compiler/runtime/scene contract에서 수정한다.

## 8. 종료 판단

Goal7은 framework/compiler correctness, Windows/macOS native shell, Web build/static product 경계까지 큰 구현 단계를 닫았다. 그러나 Web live 자동화, 통합 release integrity/stability baseline과 Windows/macOS/Web physical acceptance가 남아 있으므로 **Goal7 전체 완료로 기록하지 않는다**.

> 문서 성격: Goal7 활성 roadmap 삭제 시점의 역사 요약과 evidence 경계. 후속 active roadmap은 아직 지정하지 않았다.

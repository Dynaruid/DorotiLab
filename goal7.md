# Doroti 7차 목표 — 제품 정확성 closure와 Windows/Web release

> 상태: G7-0 ✅ 완료 — G7-1 shared correctness와 G7-3 Web build/publish 착수 가능
> 작성일: 2026-08-14
> 측정 핵심화: 2026-08-15
> 선행 기록: [`history/26-08-14/goal6-summary.md`](history/26-08-14/goal6-summary.md)
> 기준 Doroti revision: `5379137447162adb2957212ea2f336894effe05e` + 현재 작업 트리
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> 최우선 제품 gate: 동일한 일반 Dart `DorotiDemoApp`이 promoted package만 사용해 Windows와 Web `browser-wasm`에서 build/publish되고, 실제 GPU frame과 app-essential input·semantics·resource 경계를 통과할 것

## 0. Goal6에서 이어받는 기준선

Goal6에서 이미 검증한 아래 결과는 재측정하지 않고 predecessor evidence로 보존한다.

- Widgets actual HWND strict-GPU first frame과 기본 lifecycle
- 최소 Material DemoApp 및 navigation/overlay/form/scroll/resource/accessibility/plugin slice
- Material 54/60 family와 Cupertino 55/55 family의 Windows `presented` 증거
- 일반 Dart package compiler, promoted package-only Windows app과 release artifact baseline
- typed scene/effect contract의 C0 managed/Windows bounded slice
- Win32 CalendarDatePicker의 실제 hover/click과 cursor/chrome automated slice

다음 상태는 완료로 승계하지 않는다.

- pinned Flutter reference가 필요한 fidelity fixture의 미실행 상태
- direct callback만 실행한 component의 `interactive`
- 자동화되지 않은 physical input/IME/accessibility
- Windows 결과로 추정한 Web 또는 다른 desktop target 결과
- 아직 구현하지 않은 retained rendering과 browser/package lifecycle

Goal6의 54/60, 55/55, 771, 193, 74, 440 같은 수치는 당시 범위를 설명하는 역사 자료다. Goal7은 이 수치를 전부 다시 만드는 대신, 현재 release를 막는 금지 패턴과 제품 scenario가 남아 있는지만 검증한다.

## 1. 목표와 범위

Goal7의 목표는 동일한 generated Dart app을 공용 Flutter framework 의미로 Windows와 Web에 build·publish·실행하는 것이다.

필수 범위:

- shared correctness: compiler/IR/lowerer/runtime, Material 대표 visual fidelity, scene/compositing과 retained rendering
- interaction capability: pointer, capture/drag, wheel, keyboard, text/composition, semantics action의 target 인과 trace
- product app: 일반 Dart source, promoted packages, resource/localization/plugin과 deterministic publish
- Web build/runtime: `browser-wasm`, GPU canvas, browser host, static artifact, app-essential lifecycle
- release acceptance: Windows와 Chromium Web의 자동화, 실제 Windows와 Web의 최소 physical 확인

이번 Goal의 필수 target은 `win-x64`와 `browser-wasm`이다. Linux/macOS, Firefox/WebKit, 장시간 soak와 모든 device 조합은 capability matrix에 남기되, 별도 지원 target으로 결정되기 전에는 Goal7 완료를 막지 않는다.

Web은 Flutter framework policy를 JavaScript/DOM에 다시 구현하는 별도 UI가 아니다. 공용 generated framework가 widget/build/layout/paint/state를 소유하고, browser adapter는 canvas/scheduler/input/text/clipboard/accessibility/resource/GPU capability만 제공한다.

## 2. 검증 핵심화 원칙

Goal7의 blocking evidence는 아래 네 종류만 둔다.

| Gate | 확인하는 것 | 적용 범위 |
| --- | --- | --- |
| `structural` | analyzer/typed contract, 금지 패턴, clean build와 artifact identity | 변경된 공용 producer와 target graph |
| `live` | 실제 GPU first frame과 app-essential input/semantics/resource 왕복 | Windows, Chromium Web 각 1개 제품 scenario |
| `reference` | 자체 구현만으로 옳음을 판정할 수 없는 visual/effect/platform 의미 | CalendarDatePicker, compositing 대표 fixture, adaptive 선택 |
| `acceptance` | 자동화가 대신할 수 없는 실제 IME/accessibility/input | Windows와 Chromium Web의 짧은 수동 checklist |

공통 규칙:

- 같은 live run이 frame, input, semantics와 resource 인과관계를 함께 증명하면 하나의 evidence로 기록한다. 축마다 별도 실행하지 않는다.
- component family 전체를 native로 반복하지 않는다. managed/widget test는 component별 callback 의미를 맡고, native/browser test는 입력 능력군과 대표 제품 경로를 맡는다.
- 대표 fixture와 제품 scenario는 G7-0 ledger에 선택 이유와 담당 capability를 고정해, 쉬운 fixture로 임의 교체하지 못하게 한다.
- reference raster는 모든 component·DPI 조합이 아니라 fidelity 위험이 큰 대표 fixture에만 사용한다.
- 안정성·resource·성능 측정은 각 milestone에서 반복하지 않고 G7-5에서 target별 한 번의 통합 release run으로 수행한다.
- 반복 횟수나 실행 시간은 근거 없이 roadmap에 고정하지 않는다. validator가 warm-up 이후 crash/frame failure와 resource의 단조 증가 여부를 판정하고 실제 duration/sample 수를 evidence에 기록한다.
- 성능은 먼저 baseline을 기록한다. 명시적으로 승인된 budget이나 심각한 무응답·무한 증가가 아닌 한, 새 target의 임의 수치가 correctness release를 막지 않는다.
- 변경하지 않은 Goal6 범위는 predecessor validator를 전부 재실행하지 않는다. 영향받은 공용 contract와 대표 product smoke만 누적 회귀로 실행한다.
- 미실행 target/backend/browser/device는 `notVerified`로 남기되, Goal7 필수 target이 아니면 release blocker로 세지 않는다.

다음 항목은 Goal7의 독립 blocking 측정에서 제외한다.

- 24개 interactive family 각각의 native input 재측정
- milestone마다 별도로 수행하는 100회 toggle/launch/resize와 30초/300-frame smoke
- 모든 effect × path × DPI의 곱집합 differential
- handwritten/generated app 전체 화면의 중복 raster differential
- Firefox/WebKit, Linux/macOS와 30분 GPU soak의 선행 완료 요구

## 3. Roadmap

### G7-0 — carry-over blocker reset과 재현 가능한 baseline

목적: Goal6 자산은 보존하고, 현재 Windows/Web release를 실제로 막는 항목만 machine-readable ledger로 고정한다.

작업:

- carry-over를 `verified-predecessor`, `release-blocker`, `deferred`, `stale`로 재분류한다.
- compatibility 수치의 포함 관계를 다시 세지 않고, generated hotfix/widget 대체/local-number rewrite 등 금지 패턴을 source query로 고정한다.
- predecessor managed focus/frame-dispatch regression을 최소 재현 fixture로 격리한다.
- 필수 target(`win-x64`, `browser-wasm`)과 후속 target을 분리한다.

완료 gate:

- active release blocker마다 owner, 최소 재현 명령과 후속 milestone이 있음
- stale PASS가 active gate에 포함된 항목 0
- 금지 패턴 query와 대표 Windows product smoke가 재현됨
- 모든 validator shard가 20분 이내이거나 더 작은 shard로 분리됨
- `git diff --check`와 현재 제품 Release build PASS

산출물:

- `Doroti/migration/flutter-framework/g7-carryover.json`
- `Doroti/migration/flutter-framework/g7-compatibility-debt.json`
- `Doroti/migration/targets/g7-target-matrix.json`
- `Doroti/eng/validate-g7-baseline.ps1`

실행 결과(2026-08-15):

- carry-over ledger `PASS`: active release blocker 9개, owner/재현 명령/후속 milestone 누락 0, active gate의 stale PASS 0
- compatibility source query `PASS`: worktree `.g.cs` 직접 수정 0, reviewed adaptation 193, 숫자 local 의존 74, widget type 대체 0, namespace 정규화 뒤 promoted product direct diff 8
- managed regression 최소 fixture `PASS`: focus microtask가 기존 managed view의 `view.frame-dispatch` 미등록을 정확히 재현했으며 G7-1C blocker로 고정
- Windows 대표 product smoke `PASS`: actual HWND, `skia-wgl-opengl-gpu`, requested 3 frame 이상 presented, native pointer/state-raster 변화, failed/cancelled/software fallback 0, HWND/WGL resource balance
- target matrix: `win-x64` implemented/current smoke PASS, 필수 `browser-wasm` absent/release blocker, Linux/macOS/Firefox/WebKit은 non-blocking `notVerified`
- `validate-g7-baseline.ps1 -Shard All`, `Doroti.slnx` Release build(경고 0/오류 0)와 `git diff --check` PASS
- full-solution `dotnet format --verify-no-changes`는 기존 committed whitespace 진단을 재현해 별도 release blocker로 기록했으며, G7-0에서 unrelated source를 일괄 수정하지 않음

### G7-1 — shared correctness closure

진입 조건: G7-0 완료.

세 sub-gate는 병렬로 구현할 수 있지만 모두 닫혀야 제품/Web live gate로 합류한다.

#### G7-1V — Material visual/generation

- pinned Flutter CalendarDatePicker reference를 동일 locale/date/theme/viewport에서 수집한다.
- glyph/baseline/grid/selected-today/corner/shadow를 하나의 구조·raster differential로 판정한다.
- generated-file 보정, 숫자 local명 의존과 widget-specific compatibility rule을 analyzer/typed IR/공용 lowerer/runtime owner로 올린다.
- temporary rule은 제거하거나 symbol/owner/fixture/제거 조건이 있는 blocker로 남긴다.

완료 gate:

- 대표 CalendarDatePicker reference differential PASS
- generated `.g.cs` direct hotfix, widget type 대체와 local-number semantic rewrite 0
- clean/incremental candidate와 promoted product identity PASS
- 영향받은 Material wave와 제품 smoke PASS

#### G7-1I — target input/cursor/semantics capability

- native 입력을 component 수가 아니라 `hover/click`, `drag/capture`, `wheel`, `key`, `text/composition`, `semantics action` 능력군으로 검증한다.
- 각 능력군은 대표 Tier A 제품 control에서 target hit → gesture/action → state/semantics/raster 변화의 causal id를 보존한다.
- Win32 client cursor와 non-client resize ownership을 실제 좌표 event로 검증한다.
- callback 직접 호출은 managed contract로만 남기고 native `interactive` 집계에서 제외한다.

완료 gate:

- 필수 입력 능력군의 Windows native causal trace PASS
- direct callback invocation으로 얻은 native PASS 0
- hover/capture/cursor가 scenario 종료 후 정상 상태로 복귀
- physical 확인은 G7-6과 분리됨

#### G7-1C — scene/compositing/retained effects

- C0 group opacity, saveLayer와 foreground/backdrop 구분을 대표 pinned fixture로 닫는다.
- C1 retained replay, superellipse, filter compose/blend와 shader 계열을 typed contract와 consumer fixture에 연결한다.
- first/unchanged/changed/resize frame에서 retained generation과 stale cache를 검증한다.
- path와 DPI는 각각 contract 경계를 대표하는 최소 fixture로 검증하며 곱집합을 만들지 않는다.
- predecessor managed focus/frame-dispatch regression을 복구한다.

완료 gate:

- C0/C1 typed payload → translation → managed/GPU consumer chain PASS
- 대표 reference differential과 retained invalidation fixture PASS
- unknown/silent no-op/downgrade와 CPU full-frame fallback 0
- intermediate surface/cache/frame resource가 scenario 종료 후 기준 상태로 복귀
- C2 deferred item은 owner가 있고 현재 제품 consumer blocker 0

산출물:

- `Doroti/migration/flutter-framework/g7-material-reference-evidence.json`
- `Doroti/migration/flutter-framework/g7-native-interaction-evidence.json`
- `Doroti/migration/flutter-framework/g7-compositing-evidence.json`
- `Doroti/eng/validate-g7-shared-closure.ps1 -Gate <Visual|Input|Compositing>`

### G7-2 — Cupertino/adaptive와 generated Dart product closure

진입 조건: G7-1 완료.

작업:

- Cupertino는 component별 native 전수 측정 대신 Tier A 대표 control로 pointer/key/semantics 능력군을 재사용한다.
- adaptive constructor의 platform 선택과 대표 behavior를 pinned Flutter trace와 비교한다.
- 일반 Dart DemoApp과 handwritten fixture는 source/app identity 및 대표 behavior/semantics를 비교한다. 전체 화면 raster 중복 비교는 하지 않는다.
- promoted framework/hosting/target package만 사용하는 repository 밖 Windows consumer를 launch하고 대표 toggle을 수행한다.

완료 gate:

- Cupertino Tier A 대표 제품 path의 `presented`/`interactive`/필요 `semantic` PASS
- adaptive platform 선택 reference PASS
- generated app의 navigation/state/semantics가 handwritten 대표 scenario와 일치
- repository-private project/candidate fallback 0
- compiler analyzer와 Flutter SDK analyze diagnostics 0

산출물:

- `Doroti/migration/flutter-framework/g7-cupertino-adaptive-evidence.json`
- `Doroti/migration/flutter-framework/g7-generated-demo-evidence.json`
- `Doroti/eng/validate-g7-product.ps1 -Gate <Cupertino|Generated>`

### G7-3 — Web toolchain, browser host와 publish baseline

진입 조건: G7-0 완료. G7-1과 병렬 착수할 수 있으나 shared semantics를 Web workaround로 복제하지 않는다.

작업:

- .NET WebAssembly toolchain과 browser package compatibility를 검사하고 desktop native dependency 유입을 차단한다.
- `Doroti.Target.Web.browser-wasm` target identity와 `Doroti.Host.Web` adapter를 추가한다.
- document/canvas lifecycle, `requestAnimationFrame`, visibility, resize와 `devicePixelRatio`를 공용 view/frame capability로 연결한다.
- 실제 browser GPU backend identity를 기록한다. CPU/2D fallback은 browser GPU PASS로 세지 않는다.
- conditional import/environment, resource URL, font/localization과 JavaScript plugin ABI를 target-aware manifest로 생성한다.
- repository 밖 package-only consumer에서 clean build/publish하고 deployment-neutral static artifact와 hash manifest를 만든다.

완료 gate:

- browser graph의 Win32/Avalonia/native desktop dependency 0
- `browser-wasm` clean output과 repeat build identity PASS
- repository 밖 package-only Web consumer build/publish PASS
- unsupported plugin/capability silent success 0
- static artifact 누락/hash 불일치 0
- 미지원 trimming/AOT mode는 정확한 blocker와 `notVerified` 상태를 가짐

산출물:

- `Doroti/src/Doroti.Host.Web/`
- `Doroti/src/Doroti.Target.Web.browser-wasm/`
- `Doroti/migration/targets/browser-wasm.json`
- `Doroti/migration/web/g7-web-build-evidence.json`
- `Doroti/artifacts/g7-web/<version>/`
- `Doroti/eng/validate-g7-web-build.ps1 -Shard <Toolchain|Graph|Compile|Publish>`

### G7-4 — Web live product parity

진입 조건: G7-1, G7-2와 G7-3 완료.

작업:

- generated `DorotiDemoApp`을 isolated static server의 Chromium에서 로드한다.
- canvas attach → framework mount/layout/paint → GPU present → terminal frame ACK를 trace한다.
- Windows와 같은 대표 product scenario에서 pointer, wheel, keyboard, composition, clipboard, resize/DPR와 semantics action을 실행한다.
- semantics tree는 DOM/ARIA action bridge로 노출하되 별도 visual DOM widget tree를 만들지 않는다.
- pinned Flutter Web 비교는 geometry/behavior가 target 의미에 민감한 대표 scenario에만 적용한다.

완료 gate:

- browser GPU non-empty first frame PASS
- app-essential input/text/semantics/resource/plugin causal trace PASS
- 대표 resize/DPR, background/foreground와 reload lifecycle 회복 PASS
- software fallback, unhandled exception과 failed/cancelled terminal frame 0
- 대표 Flutter Web behavior/reference differential PASS
- physical IME/screen-reader는 G7-6 전까지 `notVerified`

산출물:

- `Doroti/validation/Doroti.Validation.G7Web/`
- `Doroti/migration/web/g7-web-live-evidence.json`
- `Doroti/migration/web/g7-web-browser-matrix.json`
- `Doroti/eng/validate-g7-web-live.ps1 -Shard <Smoke|Interaction|Reference>`

### G7-5 — 통합 release integrity, stability와 performance baseline

진입 조건: G7-2와 G7-4 완료.

이 단계만 반복·시간 기반 자동 측정을 소유한다.

작업:

- 동일 source/app identity의 Windows package와 Web static artifact를 하나의 release manifest에 묶는다.
- target별 한 번의 통합 scenario에서 launch/reload, navigation, toggle, resize/DPR와 종료를 반복한다.
- warm-up 뒤 first-frame/steady-frame, memory/heap과 handle/surface/listener balance를 함께 기록한다.
- crash, failed frame, unbounded resource growth와 기존 승인 budget 위반만 blocking으로 판정한다.
- Web base path/header/cache, license/provenance/SBOM, artifact hash와 independent rebuild를 검증한다.

완료 gate:

- Windows/Web 통합 stability scenario에서 crash, terminal frame failure와 단조 resource 증가 0
- first-frame, steady-frame, artifact size와 memory/resource baseline이 기록됨
- repository 밖 Windows/Web consumer가 동일 release manifest로 실행됨
- static hosting contract와 artifact hash/license/provenance/independent rebuild PASS

산출물:

- `Doroti/artifacts/g7-release/<version>/`
- `Doroti/migration/releases/g7-release-evidence.json`
- `Doroti/eng/validate-g7-release.ps1 -Target <win-x64|browser-wasm>`

### G7-6 — 최소 physical release acceptance

진입 조건: G7-5 완료.

작업:

- packaged Windows DemoApp에서 mouse, keyboard, Korean IME와 대표 accessibility action을 짧은 checklist로 확인한다.
- Chromium Web artifact에서 mouse, keyboard, Korean IME/clipboard와 대표 screen-reader action을 같은 형식으로 확인한다.
- 자동화 evidence와 user-observed evidence를 별도 필드로 기록한다.
- Linux/macOS, Firefox/WebKit, touch, multi-monitor와 장시간 GPU는 후속 target matrix에서 `notVerified`로 유지한다.

완료 gate:

- Windows physical input/IME/accessibility checklist PASS
- Chromium Web physical input/IME/clipboard/accessibility checklist PASS
- 자동화 결과를 physical 결과로 환산한 항목 0
- 필수 release artifact의 provenance/license/hash와 target matrix PASS

산출물:

- `Doroti/migration/targets/g7-target-matrix.json`
- `Doroti/artifacts/g7-physical/<target>/`
- `Doroti/eng/validate-g7-acceptance.ps1 -Target <win-x64|browser-wasm>`

## 4. 단계 의존성과 중단 규칙

```text
G7-0 carry-over blocker reset
  +-> G7-1 shared correctness
  +-> G7-3 Web build/publish

G7-1 -> G7-2 Cupertino/adaptive/generated product
G7-1 + G7-2 + G7-3 -> G7-4 Web live product
G7-2 + G7-4 -> G7-5 integrated release
G7-5 -> G7-6 physical acceptance
```

- lower layer 공용 의미 결함은 해당 compiler/runtime/scene fixture에서 먼저 수정한다.
- generated product, Web bootstrap 또는 target adapter의 widget-specific patch로 framework 결함을 숨기지 않는다.
- Web build PASS는 browser first frame 또는 interaction PASS가 아니다.
- browser automated ARIA는 physical screen-reader PASS가 아니다.
- completed predecessor gate가 영향 범위 회귀에서 깨지면 최초 regression부터 복구한다.
- unsupported operation/plugin/backend를 no-op 또는 software fallback으로 성공 처리하지 않는다.

## 5. 필수 validation 계약

모든 명령은 `.github/copilot-instructions.md`에 따라 20분 timeout 이내의 shard로 구성한다. 전체 이력 validator를 하나의 aggregate로 다시 실행하지 않는다.

```powershell
./Doroti/eng/validate-g7-baseline.ps1

./Doroti/eng/validate-g7-shared-closure.ps1 -Gate Visual
./Doroti/eng/validate-g7-shared-closure.ps1 -Gate Input
./Doroti/eng/validate-g7-shared-closure.ps1 -Gate Compositing

./Doroti/eng/validate-g7-product.ps1 -Gate Cupertino
./Doroti/eng/validate-g7-product.ps1 -Gate Generated

./Doroti/eng/validate-g7-web-build.ps1 -Shard Toolchain
./Doroti/eng/validate-g7-web-build.ps1 -Shard Publish
./Doroti/eng/validate-g7-web-live.ps1 -Shard Smoke
./Doroti/eng/validate-g7-web-live.ps1 -Shard Interaction

./Doroti/eng/validate-g7-release.ps1 -Target win-x64
./Doroti/eng/validate-g7-release.ps1 -Target browser-wasm

dotnet build Doroti/Doroti.slnx --configuration Release --nologo
dotnet format Doroti/Doroti.slnx --verify-no-changes --no-restore --verbosity minimal
git diff --check
```

아직 존재하지 않는 G7 validator는 해당 milestone의 구현 산출물이다. 문서에 명령이 있다는 이유만으로 실행 또는 PASS로 간주하지 않는다.

## 6. 공통 구현 원칙

- Flutter source가 widget/build/layout/paint/state/semantics policy의 단일 owner다.
- target host는 window/document, scheduler, surface/GPU, input/text/clipboard/accessibility/resource capability만 소유한다.
- browser DOM은 canvas host와 accessibility bridge이며 두 번째 visual widget tree가 아니다.
- scene/canvas/paint payload는 typed immutable contract로 backend까지 보존한다.
- generated `.g.cs` 직접 수정, filename/local-number rewrite와 widget type 대체는 제품 수정으로 인정하지 않는다.
- strict GPU gate에서 CPU full-frame readback/upload나 2D canvas fallback을 숨기지 않는다.
- package-only external consumer, source/compiler/product/target/artifact digest를 release evidence에 포함한다.
- local temporary/cache는 repository의 `.doroti` 계약을 따르고 광범위한 system Temp 정리를 하지 않는다.

## 7. Goal7 최종 완료 정의

Goal7은 다음이 모두 사실일 때 완료한다.

- 일반 Dart `DorotiDemoApp`이 promoted package만 사용해 Windows와 Web `browser-wasm` artifact로 reproducible build된다.
- Windows strict-GPU와 browser GPU에서 실제 first frame, app-essential interaction와 semantics가 target별로 PASS한다.
- Material 대표 visual, native input capability와 scene/compositing/retained gate가 필요한 pinned reference와 함께 닫힌다.
- Cupertino/adaptive와 generated product의 대표 behavior/semantics가 PASS한다.
- Windows/Web release artifact에 hash, provenance, license, capability matrix와 단일 통합 stability/performance baseline이 포함된다.
- Windows와 Chromium Web의 physical input/IME/accessibility checklist가 자동화 결과와 분리되어 기록된다.
- 필수 target의 미실행 항목을 성공으로 기록한 경우가 0이다.

Linux/macOS, Firefox/WebKit, 모든 component별 native 전수 검사, 모든 DPI/effect 곱집합과 장시간 soak는 Goal7 완료 정의에 포함하지 않는다. 해당 target을 실제 지원 대상으로 선택하는 후속 Goal에서 그 target의 release gate로 승격한다.

Goal7의 Web 성공 기준은 “WASM 파일이 생성된다”가 아니다. 동일한 generated Flutter framework app이 브라우저의 실제 GPU canvas에서 frame을 내고, Web input과 accessibility action이 같은 widget state까지 왕복하며, 재현 가능한 static release artifact로 배포될 수 있어야 한다.

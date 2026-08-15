# Doroti 7차 목표 — 제품 정확성 closure와 Windows/macOS shell·Web release

> 상태: G7-3V/A/B/C PASS, G7-4 Chromium 수동 제품 smoke PASS / 자동화 live parity gate 진행 예정
> 작성일: 2026-08-14
> 측정 핵심화: 2026-08-15
> 범위 추가: 2026-08-15 — Apple Silicon macOS shell(`osx-arm64`)을 필수 target으로 승격
> 선행 기록: [`history/26-08-14/goal6-summary.md`](history/26-08-14/goal6-summary.md)
> 기준 Doroti revision: `5379137447162adb2957212ea2f336894effe05e` + 현재 작업 트리
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> Flutter reference toolchain: pinned 비교·호환성 검증에서만 repository-local `flutter-master` SDK를 사용하며 global/PATH Flutter·Dart fallback 금지
> Doroti 제품 toolchain: 사용자 앱은 C#으로 작성하고 표준 .NET SDK와 배포된 Doroti template/package만으로 build한다. Flutter SDK, Flutter CLI, Dart app project와 별도 Flutter platform project는 제품 생성·publish 경로에서 사용하지 않는다.
> Web host model: Blazor WebAssembly를 browser host shell로 채택한다. allowlist된 root/surface Razor component가 canvas·host lifecycle·browser event bridge만 소유하고, Doroti widget/build/layout/paint/state/semantics와 visual tree는 공용 C# framework 및 Skia surface가 계속 소유한다.
> 최우선 제품 gate: 동일한 일반 C# `DorotiDemoApp`을 Blazor WebAssembly `net10.0`/`browser-wasm`과 desktop target으로 build/publish하고, Doroti가 source-port한 platform shell 위에서 Skia GPU frame과 app-essential input·semantics·resource 경계를 통과할 것

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

Goal7의 목표는 Flutter source에서 생성·승격한 C# framework API를 사용해 작성한 동일한 C# app을 Windows, macOS와 Web에 build·publish·실행하는 것이다. Dart→C# compiler는 framework/package 생성 도구이며 사용자 app의 제품 언어는 C#이다.

필수 범위:

- shared correctness: compiler/IR/lowerer/runtime, Material 대표 visual fidelity, scene/compositing과 retained rendering
- interaction capability: pointer, capture/drag, wheel, keyboard, text/composition, semantics action의 target 인과 trace
- product app: 일반 C# project/source, promoted Doroti framework/target packages, resource/localization/plugin과 deterministic publish
- macOS shell: source-ported AppKit/libAvalonia window·dispatcher·surface와 input/text/clipboard/accessibility bridge, `osx-arm64` package-only publish
- Web build/runtime: `browser-wasm`, GPU canvas, browser host, static artifact, app-essential lifecycle
- release acceptance: Windows, macOS와 Chromium Web의 자동화 및 최소 physical 확인

이번 Goal의 필수 target은 `win-x64`, `osx-arm64`와 `browser-wasm`이다. Linux, Intel macOS(`osx-x64`), Firefox/WebKit, 장시간 soak와 모든 device 조합은 capability matrix에 남기되, 별도 지원 target으로 결정되기 전에는 Goal7 완료를 막지 않는다. macOS shell은 Flutter framework policy를 복제하지 않고 `Doroti.Shell.Core`와 공용 desktop host 아래에서 AppKit/libAvalonia capability만 소유한다.

Web은 Flutter framework policy를 JavaScript/DOM에 다시 구현하는 별도 UI가 아니다. 공용 generated framework가 widget/build/layout/paint/state를 소유하고, browser adapter는 canvas/scheduler/input/text/clipboard/accessibility/resource/GPU capability만 제공한다.

Doroti Web 제품은 Flutter Web project의 wrapper도, Dart app transpilation 결과도 아니다. 사용자 project는 C# source와 SDK-style `.csproj`를 소유하고 `Doroti.Framework.*`의 생성·승격된 C# API를 직접 사용한다. browser host project는 표준 Blazor WebAssembly TFM `net10.0`과 RID `browser-wasm`을 사용하며 `SkiaSharp.Views.Blazor.SKGLView`, `SkiaSharp.NativeAssets.WebAssembly`와 Emscripten WebGL2를 통해 canvas에 그린다. 별도 CanvasKit runtime/JS API는 포함하지 않는다. Flutter/Dart source와 pinned Flutter SDK는 framework regeneration 및 reference differential에만 사용하며 사용자 app 생성과 제품 artifact build에는 참여하지 않는다.

Web hosting은 Blazor WebAssembly host 계약을 따른다. `Microsoft.NET.Sdk.BlazorWebAssembly`, `Microsoft.AspNetCore.Components.WebAssembly`, `blazor.webassembly.js`, static web assets, Emscripten native linking과 deployment-neutral `wwwroot` publish를 사용한다. `DorotiRoot`/`DorotiSurface` 같은 allowlist된 host component만 root mount, `SKGLView`, canvas focus·pointer event와 host disposal을 담당한다. Blazor router/form/component library를 Doroti app UI로 사용하거나 Doroti widget을 Razor/DOM node로 투영하지 않으며, browser DOM은 canvas, hidden text/IME와 accessibility bridge만 소유한다. 고빈도 pointer/coalescing, pointer capture, IME와 context-loss처럼 DOM callback 안에서 즉시 처리해야 하는 기능은 작은 `[JSImport]` module에 남기고 입력 정규화·routing·gesture/state policy는 C#에 둔다.

## 2. 검증 핵심화 원칙

Goal7의 blocking evidence는 아래 네 종류만 둔다.

| Gate | 확인하는 것 | 적용 범위 |
| --- | --- | --- |
| `structural` | analyzer/typed contract, 금지 패턴, clean build와 artifact identity | 변경된 공용 producer와 target graph |
| `live` | 실제 GPU first frame과 app-essential input/semantics/resource 왕복 | Windows, macOS, Chromium Web 각 1개 제품 scenario |
| `reference` | 자체 구현만으로 옳음을 판정할 수 없는 visual/effect/platform 의미 | CalendarDatePicker, compositing 대표 fixture, adaptive 선택 |
| `acceptance` | 자동화가 대신할 수 없는 실제 IME/accessibility/input | Windows, macOS와 Chromium Web의 짧은 수동 checklist |

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
- Firefox/WebKit, Linux, Intel macOS와 30분 GPU soak의 선행 완료 요구


## 3. 완료 요약과 남은 Roadmap

완료 milestone은 작업 목록에서 제외하고 아래 evidence 요약으로만 유지한다.

| Milestone | 상태 | 보존 근거 |
| --- | --- | --- |
| G7-0 | `PASS` | carry-over 분류, 금지 패턴 query, Windows strict-GPU product smoke, Release build — `g7-carryover.json`, `validate-g7-baseline.ps1` |
| G7-1V/I/C | `PASS` | Material reference/generation, Windows native input causal trace, compositing/retained/focus closure — `g7-material-reference-evidence.json`, `g7-native-interaction-evidence.json`, `g7-compositing-evidence.json` |
| G7-2 | `PASS` | Cupertino/adaptive와 generated Dart product parity, package-only Windows consumer — `g7-cupertino-adaptive-evidence.json`, `g7-generated-demo-evidence.json` |
| G7-3M | `PASS` | Apple Silicon NSWindow strict-GPU live, input/text/clipboard/NSAccessibility, repeat `osx-arm64` package publish — `g7-macos-shell-evidence.json` |
| G7-3N | `PASS` | 17개 project/package 및 27개 owned type naming closure, 전 target producer/consumer graph, Windows representative live — `g7-doroti-naming-evidence.json` |
| G7-3V/A/B/C | `PASS` | Avalonia Browser exact behavior provenance, 독립 Blazor/Skia capability, 동일 `DorotiDemoApp/Program.cs` desktop/browser build, `doroti-app` package-only acceptance와 720-file repeat publish identity — `g7-web-build-evidence.json` |

완료 범위의 상세 이력은 각 machine-readable evidence와 [Goal6 요약](history/26-08-14/goal6-summary.md)에 보존한다. 미실행 browser live·physical 결과는 아래 active milestone에서 계속 `notVerified`다.

2026-08-15 G7-4 선행 수동 smoke에서는 G7-3 공식 `browser-wasm` publish artifact를 Chromium에 로드해 실제 non-empty GPU canvas, Flutter식 logical size/physical backing-store DPR 적용, bounded backdrop blur(`sigmaX=12`, `sigmaY=6`), desktop과 같은 ambient/spot 2-pass shadow, semantics tree와 pointer 상태 변화(FAB `24 → 27`)를 확인했다. 해당 origin의 console error는 0이었다. 이 결과는 `presented`와 기본 pointer 경로의 수동 관찰이며, wheel/keyboard/composition/clipboard, resize·DPR lifecycle 회복, ARIA action, Flutter Web differential과 physical IME/screen-reader를 완료로 승격하지 않는다.

### G7-4 — Web live product parity

진입 조건: G7-3V/G7-3A/G7-3B/G7-3C 완료. infrastructure probe만으로는 진입할 수 없다.

현재 상태: 공식 publish artifact의 수동 Chromium `presented`/기본 pointer smoke는 확인했다. 아래 자동화 causal trace와 reference/acceptance 항목은 아직 `notVerified`다.

작업:

- `dotnet new doroti-app`과 `dotnet publish -r browser-wasm`으로 만든 C# Doroti app을 isolated static server의 Chromium에서 로드한다.
- canvas attach → framework mount/layout/paint → GPU present → terminal frame ACK를 trace한다.
- Windows와 같은 대표 product scenario에서 pointer, wheel, keyboard, composition, clipboard, resize/DPR와 semantics action을 실행한다.
- click/tap은 표준 Blazor pointer event 경로로, drag/capture와 coalesced high-rate move는 최소 JS fast path로 실행하되 두 경로가 같은 C# normalization과 `IInputHostCapability` 계약에 합류하는지 검증한다.
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

- `Doroti/migration/web/g7-web-live-evidence.json`
- `Doroti/migration/web/g7-web-browser-matrix.json`
- `Doroti/eng/validate-g7-web-live.ps1 -Shard <Smoke|Interaction|Reference>`

### G7-5 — 통합 release integrity, stability와 performance baseline

진입 조건: G7-4 완료.

이 단계만 반복·시간 기반 자동 측정을 소유한다.

작업:

- 동일 source/app identity의 Windows/macOS package와 Web static artifact를 하나의 release manifest에 묶는다.
- target별 한 번의 통합 scenario에서 launch/reload, navigation, toggle, resize/DPR와 종료를 반복한다.
- warm-up 뒤 first-frame/steady-frame, memory/heap과 handle/surface/listener balance를 함께 기록한다.
- crash, failed frame, unbounded resource growth와 기존 승인 budget 위반만 blocking으로 판정한다.
- Web base path/header/cache, license/provenance/SBOM, artifact hash와 independent rebuild를 검증한다.

완료 gate:

- Windows/macOS/Web 통합 stability scenario에서 crash, terminal frame failure와 단조 resource 증가 0
- first-frame, steady-frame, artifact size와 memory/resource baseline이 기록됨
- repository 밖 Windows/macOS/Web consumer가 동일 release manifest로 실행됨
- static hosting contract와 artifact hash/license/provenance/independent rebuild PASS

산출물:

- `Doroti/artifacts/g7-release/<version>/`
- `Doroti/migration/releases/g7-release-evidence.json`
- `Doroti/eng/validate-g7-release.ps1 -Target <win-x64|osx-arm64|browser-wasm>`

### G7-6 — 최소 physical release acceptance

진입 조건: G7-5 완료.

작업:

- packaged Windows DemoApp에서 mouse, keyboard, Korean IME와 대표 accessibility action을 짧은 checklist로 확인한다.
- packaged macOS DemoApp에서 mouse/trackpad, keyboard, Korean IME와 대표 VoiceOver action을 짧은 checklist로 확인한다.
- Chromium Web artifact에서 mouse, keyboard, Korean IME/clipboard와 대표 screen-reader action을 같은 형식으로 확인한다.
- 자동화 evidence와 user-observed evidence를 별도 필드로 기록한다.
- Linux, Intel macOS, Firefox/WebKit, touch, multi-monitor와 장시간 GPU는 후속 target matrix에서 `notVerified`로 유지한다.

완료 gate:

- Windows physical input/IME/accessibility checklist PASS
- macOS physical input/trackpad/IME/VoiceOver checklist PASS
- Chromium Web physical input/IME/clipboard/accessibility checklist PASS
- 자동화 결과를 physical 결과로 환산한 항목 0
- 필수 release artifact의 provenance/license/hash와 target matrix PASS

산출물:

- `Doroti/migration/targets/g7-target-matrix.json`
- `Doroti/artifacts/g7-physical/<target>/`
- `Doroti/eng/validate-g7-acceptance.ps1 -Target <win-x64|osx-arm64|browser-wasm>`

## 4. 단계 의존성과 중단 규칙

완료된 선행 조건은 G7-0, G7-1, G7-2, G7-3M, G7-3N, G7-3V/A/B/C다. G7-4는 G7-3 static product artifact를 Chromium live 결과로 확장한다.

```text
G7-4 Chromium live product
  -> G7-5 integrated release
  -> G7-6 physical acceptance
```

- lower layer 공용 의미 결함은 해당 compiler/runtime/scene fixture에서 먼저 수정한다.
- generated product, Web bootstrap 또는 target adapter의 widget-specific patch로 framework 결함을 숨기지 않는다.
- Web build PASS는 browser first frame 또는 interaction PASS가 아니다.
- browser automated ARIA는 physical screen-reader PASS가 아니다.
- completed predecessor gate가 영향 범위 회귀에서 깨지면 최초 regression부터 복구한다.
- unsupported operation/plugin/backend를 no-op 또는 software fallback으로 성공 처리하지 않는다.

## 5. 남은 validation 계약

모든 명령은 `.github/copilot-instructions.md`에 따라 20분 timeout 이내의 shard로 구성한다. 완료 milestone validator는 영향 범위 회귀가 생긴 경우에만 다시 실행한다.

```powershell
./Doroti/eng/validate-g7-web-live.ps1 -Shard Smoke
./Doroti/eng/validate-g7-web-live.ps1 -Shard Interaction
./Doroti/eng/validate-g7-web-live.ps1 -Shard Reference

./Doroti/eng/validate-g7-release.ps1 -Target win-x64
./Doroti/eng/validate-g7-release.ps1 -Target osx-arm64
./Doroti/eng/validate-g7-release.ps1 -Target browser-wasm

./Doroti/eng/validate-g7-acceptance.ps1 -Target win-x64
./Doroti/eng/validate-g7-acceptance.ps1 -Target osx-arm64
./Doroti/eng/validate-g7-acceptance.ps1 -Target browser-wasm

dotnet build Doroti/Doroti.slnx --configuration Release --nologo
git diff --check
```

아직 존재하지 않는 validator와 shard는 해당 active milestone의 구현 산출물이다. 문서에 명령이 있다는 이유만으로 실행 또는 PASS로 간주하지 않는다.

## 6. 공통 구현 원칙

- Flutter source가 widget/build/layout/paint/state/semantics policy의 단일 owner다.
- Windows/macOS/Web의 Doroti-owned product project, assembly, package, namespace, type와 component는 `Doroti` 또는 역할 기반 중립 이름을 사용한다. `Flutter` 명칭은 upstream/reference/provenance와 exact Flutter compatibility API allowlist 밖으로 노출하지 않는다.
- target host는 window/document, scheduler, surface/GPU, input/text/clipboard/accessibility/resource capability만 소유한다.
- Windows와 macOS desktop host는 같은 `Doroti.Shell.Core` 계약을 사용하며 target-specific native handle, event loop와 service 구현만 갈라진다.
- libAvalonia는 macOS shell의 versioned native build input이며 사전 빌드된 출처 불명 binary나 Avalonia UI/Control graph를 제품 의존성으로 허용하지 않는다.
- browser DOM은 canvas host와 accessibility bridge이며 두 번째 visual widget tree가 아니다.
- Web host는 Blazor WebAssembly runtime/bootstrap/static asset 계약을 사용한다. allowlist된 root/surface component와 `SKGLView`는 canvas·host lifecycle·browser event bridge만 소유하며 Blazor render tree/DOM diff는 Doroti 제품 UI owner가 될 수 없다.
- 표준 Blazor event는 pointer/key/focus를 C# host로 전달하고, `[JSImport]`/`[JSExport]` module은 pointer capture/coalescing, IME, context-loss와 browser capability bridge만 소유한다. widget/build/layout/paint/state/gesture policy를 JavaScript나 Razor에 옮기지 않는다.
- scene/canvas/paint payload는 typed immutable contract로 backend까지 보존한다.
- generated `.g.cs` 직접 수정, filename/local-number rewrite와 widget type 대체는 제품 수정으로 인정하지 않는다.
- strict GPU gate에서 CPU full-frame readback/upload나 2D canvas fallback을 숨기지 않는다.
- package-only external consumer, source/compiler/product/target/artifact digest를 release evidence에 포함한다.
- local temporary/cache는 repository의 `.doroti` 계약을 따르고 광범위한 system Temp 정리를 하지 않는다.

## 7. Goal7 최종 완료 정의

Goal7은 다음이 모두 사실일 때 완료한다.

- Flutter/Dart SDK·CLI와 Flutter project 없이 일반 C# Doroti app을 template으로 생성할 수 있고, 표준 .NET SDK와 promoted Doroti package만 사용해 Windows, macOS `osx-arm64`와 Blazor WebAssembly `net10.0`/`browser-wasm` artifact로 reproducible build된다.
- 전 target의 product graph와 사용자-facing API가 G7-3N naming map으로 전환되고, allowlist 밖 `Doroti.Flutter`, `Host.Desktop.Flutter`와 Doroti-owned `Flutter*` identifier 및 이전 identity fallback이 0이다.
- `dotnet publish -r browser-wasm`의 최종 static artifact가 사용자 C# app assembly, generated C# Doroti framework, SkiaSharp WebAssembly backend, Avalonia behavior를 참고해 Blazor/C#으로 독립 구현한 browser input capability, resource와 browser bootstrap을 포함하며 사용자가 내부 Web host나 runtime JavaScript를 직접 작성하지 않는다.
- Web artifact는 pinned Blazor WebAssembly host, `blazor.webassembly.js`, allowlist된 root/surface component, `SkiaSharp.Views.Blazor`와 `SkiaSharp.NativeAssets.WebAssembly` static/native asset을 포함한다. 별도 CanvasKit runtime, component-per-Doroti-widget DOM tree와 Blazor UI policy는 포함하지 않는다.
- Windows/macOS strict-GPU와 browser GPU에서 실제 first frame, app-essential interaction와 semantics가 target별로 PASS한다.
- Material 대표 visual, native input capability와 scene/compositing/retained gate가 필요한 pinned reference와 함께 닫힌다.
- Cupertino/adaptive와 generated product의 대표 behavior/semantics가 PASS한다.
- Windows/macOS/Web release artifact에 hash, provenance, license, capability matrix와 단일 통합 stability/performance baseline이 포함된다.
- Windows, macOS와 Chromium Web의 physical input/IME/accessibility checklist가 자동화 결과와 분리되어 기록된다.
- 필수 target의 미실행 항목을 성공으로 기록한 경우가 0이다.

Linux, Intel macOS(`osx-x64`), Firefox/WebKit, 모든 component별 native 전수 검사, 모든 DPI/effect 곱집합과 장시간 soak는 Goal7 완료 정의에 포함하지 않는다. 해당 target을 실제 지원 대상으로 선택하는 후속 Goal에서 그 target의 release gate로 승격한다.

Goal7의 macOS 성공 기준은 “managed project와 libAvalonia가 컴파일된다”가 아니다. 동일한 generated Doroti framework app이 실제 NSWindow의 GPU surface에서 frame을 내고, AppKit 입력·텍스트·접근성 action이 같은 widget state까지 왕복하며, provenance가 고정된 `osx-arm64` package로 저장소 밖에서 실행되어야 한다.

Goal7의 Web 성공 기준은 “WASM 파일이 생성된다”, “빈 .NET consumer가 Web target package를 참조한다” 또는 “Blazor host component가 canvas를 만들었다”가 아니다. 사용자가 일반 C#으로 Doroti widget tree를 작성하고 별도 Razor/JavaScript 작성 없이 표준 `dotnet` 명령으로 Blazor WebAssembly `net10.0`/`browser-wasm`에 publish할 수 있어야 한다. Blazor WebAssembly는 runtime, root/surface component와 browser event bridge를 담당하고, 동일한 C# app이 SkiaSharp WebAssembly backend를 통해 브라우저의 실제 GPU canvas에서 frame을 내며 DOM pointer → C# normalization → Doroti gesture/state → Skia frame과 accessibility action이 같은 widget state까지 왕복해야 한다.

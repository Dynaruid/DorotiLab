# Doroti 7차 목표 — 제품 정확성 closure, Web build와 multi-target release

> 상태: G7-0 🚧 시작 — Goal6 partial evidence를 재분류하고 Web target baseline을 추가
> 작성일: 2026-08-14
> 선행 기록: [`history/26-08-14/goal6-summary.md`](history/26-08-14/goal6-summary.md)
> 기준 Doroti revision: `21ebdbbe36691d8e30d66114f39e8a00aa339c43` + 현재 작업 트리
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> Avalonia source pin: `f159423f691946e713f454447a780d4677d8a0d2`
> 최우선 제품 gate: 동일한 일반 Dart `DorotiDemoApp`이 package-only Windows와 Web `browser-wasm`에서 실제 frame을 표시하고, 각 target의 실제 input·semantics·resource 경계를 독립 증거로 통과할 것

## 0. Goal6에서 이어받는 기준선

Goal6는 live-first Windows 기반을 만들었지만 최종 완료되지는 않았다. 아래 결과는 다시 구현하지 않고 predecessor evidence로 보존한다.

- Widgets actual HWND strict-GPU first frame과 lifecycle
- 최소 Material DemoApp 및 navigation/overlay/form/scroll/resource/accessibility/plugin slice
- Material 54/60 family와 Cupertino 55/55 family의 Windows `presented` 증거
- 일반 Dart package compiler, promoted package-only Windows app과 release artifact baseline
- typed scene/effect contract의 C0 managed/Windows bounded slice
- Win32 실제 CalendarDatePicker hover/click과 cursor/chrome automated slice

다음 상태는 완료로 승계하지 않는다.

- pinned Flutter visual/reference differential이 없는 화면
- direct callback만 실행한 component의 `interactive`
- 자동화되지 않은 physical input/IME/accessibility
- Windows 결과로 추정한 Avalonia/Linux/macOS/Web 결과
- 아직 실행하지 않은 retained/stress/browser/package lifecycle

Goal7은 Goal6 번호를 억지로 이어 붙이지 않고, 남은 제품 정확성 gate와 Web target을 하나의 release dependency graph로 다시 구성한다.

## 1. 목표와 범위

Goal7의 목표는 Windows에서 일부 frame이 보이는 상태를 넘어, 동일한 generated Dart app이 공용 Flutter framework 의미를 유지한 채 desktop과 browser target으로 빌드·배포·실행되는 상태를 만드는 것이다.

필수 범위:

- shared correctness: compiler/IR/lowerer/runtime, Material visual fidelity, scene/compositing과 retained rendering
- native interaction: pointer, cursor, keyboard, text/IME, scroll와 semantics의 실제 target input 인과 trace
- product app: 일반 Dart source, promoted packages, resource/localization/plugin과 deterministic publish
- Web build: `browser-wasm` target, browser host, GPU canvas, static artifact와 repository 밖 consumer
- Web runtime: resize/DPR, animation frame, pointer/wheel/keyboard/composition/clipboard, accessibility와 browser lifecycle
- release: Windows와 Web automated matrix, Windows physical 및 최소 한 개 non-Windows physical target

Web은 Flutter framework policy를 JavaScript/DOM에 다시 구현하는 별도 UI가 아니다. 공용 generated framework가 widget/build/layout/paint/state를 소유하고, browser adapter는 canvas/scheduler/input/text/clipboard/accessibility/resource/GPU capability만 제공한다.

## 2. 증거 상태와 target 경계

각 scenario는 다음 축을 독립 기록한다.

1. `compiler`: analyzed/generated/compiled/identity
2. `framework`: constructed/mounted/laidOut/painted/semantic
3. `target`: packaged/loaded/presented/interactive/lifecycle
4. `backend`: managed, Win32-WGL-Skia, browser GPU, browser software fallback
5. `reference`: Flutter trace/raster differential
6. `automation`: synthetic, managed, native/browser automated, physical user/device

필수 target identity:

- `win-x64`: Win32 window + WGL/OpenGL/Skia
- `browser-wasm`: browser document + GPU canvas backend identity
- 후속 desktop RID: Linux X11/Wayland 또는 macOS

한 target/backend/browser의 PASS는 다른 항목으로 환산하지 않는다. browser DOM/ARIA 검사도 physical screen reader 검증을 대신하지 않으며, 실행하지 않은 상태는 `notVerified`로 남긴다.

## 3. Roadmap

### G7-0 — carry-over truth reset과 재현 가능한 baseline

목적: Goal6 partial 구현을 잃지 않되 현재 실제로 닫힌 gate와 열린 gate를 machine-readable ledger로 고정한다.

작업:

- Goal6 evidence를 `verified-predecessor`, `partial`, `blocked`, `notVerified`, `stale`로 재분류한다.
- Material 54/60 presented, native input 1/24, Cupertino 55/55 presented와 generated Windows app 수치를 서로 다른 상태 축으로 보존한다.
- G6 compatibility 771개, reviewed 보정 193개, 숫자 local 의존 74개와 temporary source inventory 440개의 포함 관계를 하나의 debt ledger로 정규화한다.
- 실패 중인 predecessor managed focus/frame-dispatch regression을 재현하고 최초 원인 taxonomy를 고정한다.
- Windows, Web, Linux/macOS와 physical evidence를 별도 target matrix로 초기화한다.

완료 gate:

- Goal6 summary/evidence/현재 source 사이 상태 모순 0
- carry-over item의 owner, 재현 명령, 후속 milestone 누락 0
- stale PASS가 active release gate에 포함된 항목 0
- 모든 validator shard가 20분 이내이거나 더 작은 shard로 분리됨
- `git diff --check`와 현재 제품 Release build PASS

산출물:

- `Doroti/migration/flutter-framework/g7-carryover.json`
- `Doroti/migration/flutter-framework/g7-compatibility-debt.json`
- `Doroti/migration/targets/g7-target-matrix.json`
- `Doroti/eng/validate-g7-baseline.ps1`

### G7-1 — shared correctness 재개방 gate closure

진입 조건: G7-0 완료.

세 gate는 병렬로 구현할 수 있지만 모두 닫혀야 G7-2 product closure로 합류한다.

#### G7-1V — Material visual/generation fidelity

- pinned Flutter CalendarDatePicker reference raster를 동일 locale/date/theme/size/DPI로 수집한다.
- glyph/baseline/grid/selected-today/corner/shadow를 구조 mask와 component별 tolerance로 판정한다.
- generated-file 보정, 숫자 local명 의존과 widget-specific compatibility rule을 analyzer/typed IR/공용 lowerer/runtime owner로 올린다.
- temporary rule은 제거하거나 symbol/owner/fixture/제거 조건이 있는 명시적 blocker로 남긴다.

완료 gate:

- pinned reference visual differential PASS
- generated `.g.cs` direct hotfix와 widget type 대체 0
- filename/source-offset/local-number 의존 semantic rewrite 0
- clean/incremental candidate와 promoted product identity PASS
- M0~M6 strict-GPU 누적 regression PASS

#### G7-1I — 실제 input/cursor/semantics closure

- interactive 대상 24개 family를 좌표 기반 hover/down/up/wheel/key/text input으로 재실행한다.
- target hit, gesture/action, callback, state/semantics/raster 변화에 동일 causal id를 보존한다.
- 실제 Win32 8방향 border drag resize와 pinned Flutter desktop state differential을 추가한다.
- callback 직접 호출은 managed contract로만 남기고 `interactive` 집계에서 제외한다.

완료 gate:

- Tier A 및 검증 대상 interactive family native input 100%
- direct callback invocation으로 얻은 native PASS 0
- hover/capture/cursor stuck 0 및 100회 stress PASS
- automated native와 user-observed physical evidence가 분리됨

#### G7-1C — scene/compositing/retained effect closure

- C0 pinned Flutter differential, group opacity, saveLayer와 foreground/backdrop 구분을 닫는다.
- C1 retained replay, superellipse, image/color filter compose, blend, gradient/image shader와 ShaderMask를 consumer fixture까지 연결한다.
- first/unchanged/scroll/filter-toggle/resize frame에서 retained generation과 stale cache를 검증한다.
- rect/RRect/path, nested transform와 DPI 1.0/1.25/2.0 differential을 실행한다.
- predecessor managed focus/frame-dispatch regression을 복구하고 Material/input gate를 누적 실행한다.

완료 gate:

- C0/C1의 payload/translation/group/GPU/managed/reference/retained 상태 PASS
- unknown/silent no-op/downgrade와 CPU full-frame fallback 0
- 100회 toggle/scroll/resize 및 30초/300-frame stress PASS
- intermediate surface/cache/frame/HWND/WGL resource balance PASS
- C2 deferred item은 owner와 milestone이 있고 현재 consumer blocker 0

산출물:

- `Doroti/migration/flutter-framework/g7-material-reference-evidence.json`
- `Doroti/migration/flutter-framework/g7-native-interaction-evidence.json`
- `Doroti/migration/flutter-framework/g7-compositing-evidence.json`
- `Doroti/eng/validate-g7-shared-closure.ps1 -Gate <Visual|Input|Compositing>`

### G7-2 — Cupertino/adaptive와 generated Dart product closure

진입 조건: G7-1V/G7-1I/G7-1C 완료.

작업:

- Cupertino Tier A component를 실제 native input과 semantics로 재검증한다.
- adaptive constructor를 pinned Flutter reference와 platform별로 비교한다.
- 같은 session에서 Material/Cupertino를 100회 전환해 focus/navigation/theme/resource lifetime을 검사한다.
- handwritten fixture와 generated Dart DemoApp의 behavior/visual/semantics differential을 닫는다.
- promoted framework/hosting/target package만 사용하는 repository 밖 Windows consumer를 100회 launch/toggle한다.

완료 gate:

- Cupertino Tier A `presented`/`interactive`/필요 `semantic` 100%
- adaptive reference differential PASS
- Material/Cupertino 100회 전환과 generated app 100회 launch/toggle leak 0
- handwritten/generated product differential PASS
- repository-private project/candidate fallback 0
- compiler analyzer와 Flutter SDK analyze diagnostics 0

산출물:

- `Doroti/migration/flutter-framework/g7-cupertino-adaptive-evidence.json`
- `Doroti/migration/flutter-framework/g7-generated-demo-evidence.json`
- `Doroti/eng/validate-g7-cupertino-adaptive.ps1`
- `Doroti/eng/validate-g7-generated-demo.ps1`

### G7-3 — Web toolchain, browser host와 build/publish baseline

진입 조건: G7-0 완료. G7-1과 병렬 착수할 수 있으나 shared semantics를 Web 전용 workaround로 복제하지 않는다.

목적: 현재 `win-x64` 단일 target graph에 `browser-wasm`을 first-class target으로 추가하고, 일반 Dart app을 정적 Web artifact로 재현 가능하게 publish한다.

작업:

1. target/dependency feasibility를 고정한다.
   - .NET SDK와 `wasm-tools`, package AOT/trimming/browser compatibility를 validator가 검사한다.
   - desktop/Win32/Avalonia/Skia native dependency가 browser graph로 유입되는 지점을 자동 census한다.
   - 기존 backend를 WebAssembly에서 직접 사용할 수 없으면 공용 scene/raster contract 아래 browser GPU backend를 추가한다. Windows host를 conditional no-op으로 넣지 않는다.

2. browser target과 host를 만든다.
   - `Doroti.Target.Web.browser-wasm` target identity/manifest와 `Doroti.Host.Web` adapter를 추가한다.
   - document/canvas lifecycle, `requestAnimationFrame`, visibility, resize와 `devicePixelRatio`를 Flutter view/frame capability로 연결한다.
   - WebGL2 또는 WebGPU 중 실제 선택한 backend identity와 capability를 evidence에 기록한다. CPU/2D fallback은 browser GPU PASS로 세지 않는다.

3. compiler/application boundary를 target-aware로 만든다.
   - conditional import/environment(`dart.library.*`, Web capability) 선택을 typed application graph에 보존한다.
   - browser resource URL, bundled font/localization과 JavaScript plugin ABI를 manifest로 생성한다.
   - unsupported native plugin/channel은 target/library/capability가 포함된 exact diagnostic으로 실패한다.

4. build와 publish 산출물을 만든다.
   - clean/incremental `browser-wasm` build identity를 검증한다.
   - repository 밖 package-only consumer에서 restore/build/publish한다.
   - `index.html`, JavaScript bootstrap, WASM/runtime/framework/app/resource/font 및 hash manifest를 deployment-neutral static directory에 만든다.
   - base path, MIME type, cache policy와 optional COOP/COEP 요구를 manifest에 기록한다.

완료 gate:

- browser graph의 Win32/Avalonia/native desktop dependency 0
- `browser-wasm` clean/incremental output identity PASS
- repository 밖 package-only Web consumer build/publish PASS
- unsupported plugin/capability silent success 0
- static artifact 누락/hash 불일치 0
- Release trimming/AOT의 지원 여부와 blocker가 명시되고 실행하지 않은 mode는 `notVerified`

산출물:

- `Doroti/src/Doroti.Host.Web/`
- `Doroti/src/Doroti.Target.Web.browser-wasm/`
- `Doroti/migration/targets/browser-wasm.json`
- `Doroti/migration/web/g7-web-build-evidence.json`
- `Doroti/artifacts/g7-web/<version>/`
- `Doroti/eng/validate-g7-web-build.ps1 -Shard <Toolchain|Graph|Compile|Publish|Package>`

### G7-4 — Web live first frame와 application parity

진입 조건: G7-1 shared closure, G7-2 generated product와 G7-3 Web build baseline 완료.

작업:

- generated `DorotiDemoApp`을 local isolated static server에서 실제 browser page로 로드한다.
- canvas attach → framework mount/layout/paint → GPU present → terminal frame ACK를 trace한다.
- browser `PointerEvent`, wheel, keyboard, focus, `beforeinput`/composition, clipboard와 resize/DPR를 target capability로 연결한다.
- navigation/overlay/form/scroll/resource/localization/plugin slice를 Windows와 같은 scenario identity로 실행한다.
- semantics tree를 DOM/ARIA action bridge로 노출하되 visual DOM widget tree를 별도 owner로 만들지 않는다.
- Chromium을 blocking baseline으로 두고 Firefox/WebKit은 capability matrix에 따라 실행한다. 미실행 browser는 성공으로 기록하지 않는다.
- pinned Flutter Web reference와 geometry/raster/behavior differential을 backend tolerance로 비교한다.

완료 gate:

- browser GPU non-empty first frame와 최소 300-frame/30초 cadence PASS
- software fallback, unhandled exception, failed/cancelled frame 0
- pointer hover/click, wheel/keyboard scroll, text/composition, clipboard와 state/raster causal trace PASS
- navigation/overlay/resource/localization/JavaScript plugin happy/exact-failure PASS
- automated DOM/ARIA focus/invoke/toggle/setText/scroll action PASS
- resize/DPR 1.0/1.25/2.0, background/foreground와 reload 뒤 frame/lifecycle 회복 PASS
- Flutter Web reference differential PASS
- physical browser IME/screen-reader는 실제 실행 전까지 `notVerified`

산출물:

- `Doroti/validation/Doroti.Validation.G7Web/`
- `Doroti/migration/web/g7-web-live-evidence.json`
- `Doroti/migration/web/g7-web-browser-matrix.json`
- `Doroti/eng/validate-g7-web-live.ps1 -Browser <Chromium|Firefox|WebKit> -Shard <Smoke|Input|Text|Semantics|Differential|Stress>`

### G7-5 — Web/Windows release package와 performance budget

진입 조건: G7-2와 G7-4 완료.

작업:

- 동일 source/app identity의 Windows package와 Web static artifact를 하나의 release manifest에 묶는다.
- Web compressed/uncompressed bundle, startup/download/compile/first-frame, steady frame와 WASM heap을 기록한다.
- Windows first-frame/working-set/handle/ticker/listener와 같은 이름의 target-specific budget을 둔다.
- Web base-path 배포, immutable hashed asset, reload/cache invalidation과 offline/error disposition을 검증한다.
- license/provenance/SBOM, package hash와 clean independent rebuild를 수행한다.

완료 gate:

- target별 측정 baseline과 승인된 budget 존재; 측정하지 않은 성능 PASS 0
- 100회 Web reload/navigation/toggle 및 Windows launch/toggle resource leak 0
- repository 밖 Windows/Web consumer가 동일 release manifest로 실행됨
- static hosting에 필요한 header/base-path/cache contract PASS
- artifact hash, license, provenance와 independent rebuild PASS

산출물:

- `Doroti/artifacts/g7-release/<version>/`
- `Doroti/migration/releases/g7-release-evidence.json`
- `Doroti/eng/validate-g7-release.ps1 -Target <win-x64|browser-wasm>`

### G7-6 — desktop target 확장과 physical final verification

진입 조건: G7-5 완료.

작업:

- Linux X11/Wayland와 macOS target source-port/package를 공용 host capability에 맞춰 구현한다.
- Windows와 최소 한 개 non-Windows physical desktop target에서 packaged Dart DemoApp을 실행한다.
- physical browser에서도 mouse/touch/keyboard/Korean IME/clipboard/scale와 screen reader를 별도 기록한다.
- desktop surface/device loss, 30분 GPU, install/run/uninstall lifecycle을 검증한다.

완료 gate:

- Windows physical input/IME/accessibility/multi-monitor/DPI/sustained GPU PASS
- 최소 한 개 physical non-Windows desktop target의 GPU/input/text/scale/clipboard/accessibility/package lifecycle PASS
- physical Web browser input/IME/accessibility 결과가 automated Web 결과와 분리되어 기록됨
- 실행하지 않은 OS/backend/browser/device를 성공으로 기록한 항목 0
- release provenance/license/hash와 target matrix PASS

산출물:

- `Doroti/migration/targets/g7-target-matrix.json`
- `Doroti/artifacts/g7-physical/<target>/`
- `Doroti/eng/validate-g7-target.ps1 -Target <target>`

## 4. 단계 의존성과 중단 규칙

```text
G7-0 carry-over truth reset
  +-> G7-1V Material visual/generation
  +-> G7-1I native input/cursor/semantics
  +-> G7-1C scene/compositing/retained
  +-> G7-3 Web toolchain/host/build

G7-1V + G7-1I + G7-1C
  -> G7-2 Cupertino/adaptive/generated product

G7-2 + G7-3
  -> G7-4 Web live/application parity
  -> G7-5 Web/Windows release and performance
  -> G7-6 desktop/physical final verification
```

- lower layer 공용 의미 결함은 해당 compiler/runtime/scene fixture에서 먼저 수정한다.
- generated product, Web JavaScript bootstrap 또는 target adapter의 widget-specific patch로 framework 결함을 숨기지 않는다.
- Web build PASS는 browser first frame, interaction 또는 reference parity PASS가 아니다.
- browser automated ARIA는 physical screen reader PASS가 아니다.
- Chromium PASS를 Firefox/WebKit PASS로, Windows PASS를 Linux/macOS PASS로 확대하지 않는다.
- completed predecessor gate가 깨지면 후속 release 작업을 중단하고 최초 regression부터 복구한다.
- unsupported operation/plugin/backend를 no-op 또는 software fallback으로 성공 처리하지 않는다.

## 5. 필수 validation 계약

모든 명령은 `.github/copilot-instructions.md`에 따라 20분 timeout 이내의 shard로 구성한다. aggregate가 20분을 넘으면 timeout을 늘리지 않고 target/scenario/browser별로 나눈다.

```powershell
./Doroti/eng/validate-g7-baseline.ps1

./Doroti/eng/validate-g7-shared-closure.ps1 -Gate Visual
./Doroti/eng/validate-g7-shared-closure.ps1 -Gate Input
./Doroti/eng/validate-g7-shared-closure.ps1 -Gate Compositing

./Doroti/eng/validate-g7-cupertino-adaptive.ps1
./Doroti/eng/validate-g7-generated-demo.ps1

./Doroti/eng/validate-g7-web-build.ps1 -Shard Toolchain
./Doroti/eng/validate-g7-web-build.ps1 -Shard Publish
./Doroti/eng/validate-g7-web-live.ps1 -Browser Chromium -Shard Smoke
./Doroti/eng/validate-g7-web-live.ps1 -Browser Chromium -Shard Input

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
- package-only external consumer, source/compiler/product/target/artifact digest를 모든 release evidence에 포함한다.
- compiler/build/presented/interactive/semantic/physical/reference 상태를 독립 기록한다.
- local temporary/cache는 repository의 `.doroti` 계약을 따르고 광범위한 system Temp 정리를 하지 않는다.
- elapsed/startup/first-frame/steady-frame을 1차 성능 지표로, memory/heap/resource lifetime을 별도 지표로 기록한다.

## 7. Goal7 최종 완료 정의

Goal7은 다음이 모두 사실일 때 완료한다.

- 일반 Dart `DorotiDemoApp`이 promoted package만 사용해 Windows와 Web `browser-wasm` artifact로 clean/incremental reproducible build된다.
- Windows strict-GPU와 browser GPU에서 실제 first frame, app-essential interaction와 semantics가 target별로 PASS한다.
- Material visual/generation, native interaction와 scene/compositing/retained 재개방 gate가 pinned Flutter differential을 포함해 닫힌다.
- Cupertino/adaptive 및 handwritten/generated product differential이 PASS한다.
- Web pointer/wheel/keyboard/composition/clipboard/resize/DPR/ARIA/resource/plugin/browser lifecycle이 automated browser gate를 통과한다.
- Windows/Web release artifact에 hash, provenance, license, performance와 target capability matrix가 포함된다.
- Windows와 최소 한 개 non-Windows physical desktop target, physical Web browser의 입력/IME/accessibility 결과가 자동화 결과와 분리되어 기록된다.
- 실행하지 않은 syntax/component/effect/OS/backend/browser/device/IME/accessibility를 성공으로 기록한 항목이 0이다.

Goal7의 Web 성공 기준은 “WASM 파일이 생성된다”가 아니다. 동일한 generated Flutter framework app이 브라우저의 실제 GPU canvas에서 frame을 내고, Web input과 accessibility action이 같은 widget state까지 왕복하며, 재현 가능한 static release artifact로 배포될 수 있어야 한다.

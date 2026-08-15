# Doroti 7차 목표 — 제품 정확성 closure와 Windows/macOS shell·Web release

> 상태: G7-3V/A/B/C Doroti 자체 Web app 생성 경로 진행 예정
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
| G7-3I | `verified-infrastructure` | standalone WebAssembly toolchain, browser adapter와 static publish 기반만 승계. C# widget/Skia product PASS로 사용하지 않음 — `g7-web-build-evidence.json` |

완료 범위의 상세 이력은 각 machine-readable evidence와 [Goal6 요약](history/26-08-14/goal6-summary.md)에 보존한다. 미실행 physical·browser product 결과는 아래 active milestone에서 계속 `notVerified`다.

### G7-3 — C# + Skia Blazor WebAssembly `browser-wasm` application closure

진입 조건인 G7-3N은 완료됐고 G7-3I infrastructure를 재사용한다. G7-3은 저장소 밖에서 일반 C# Doroti app을 생성하고, 같은 C# widget tree가 Skia GPU browser canvas에 그려지는 전체 제품 경로를 소유한다.

#### G7-3V — pinned Avalonia Browser behavior reference와 Blazor capability 구현

작업:

- Avalonia revision `f159423f691946e713f454447a780d4677d8a0d2`의 `src/Browser/Avalonia.Browser`를 exact detached reference snapshot으로 확보하고 license, selected source hash와 reference provenance를 고정한다. 이 snapshot은 동작 분석·비교·추적 입력이며 product compile graph에 포함하지 않는다.
- `BrowserDispatcherImpl`, canvas resize/DPR, pointer/capture/coalescing/wheel/key, text/composition/caret, clipboard/cursor와 필요한 JavaScript/TypeScript module에서 참고할 동작을 capability 단위로 선택하고, upstream symbol → browser behavior → Doroti owner/contract mapping을 기록한다.
- 선택 source를 product code로 copy/adapt하거나 `Doroti.Vendor.Avalonia.Browser` project를 만들지 않는다. 실제 구현은 `Doroti.Host.Web`의 `DorotiRoot`/`DorotiSurface` Blazor component, C# browser capability와 최소 `[JSImport]` module이 소유하며 Avalonia type/source/binary를 참조하지 않는다.
- Avalonia `BrowserAppBuilder`, `AvaloniaView`, Controls/property/styling/visual tree, Avalonia composition owner와 `Software2D` fallback은 포함하지 않는다.
- 표준 pointer/key/focus는 Blazor event callback으로 받고, DOM event 등록/해제, `preventDefault`, pointer capture, `getCoalescedEvents`, IME와 browser object 분해처럼 동기 DOM 접근이 필요한 경계만 작은 JS module에 남긴다. Avalonia에서 확인한 pointer kind/phase/button/modifier, 좌표·DPR, wheel, timestamp와 batching 동작은 Avalonia 코드를 이식하지 않고 C# `BrowserInputSource`로 새로 구현한다.
- GPU surface는 Avalonia renderer나 CanvasKit이 아니라 pinned `SkiaSharp.Views.Blazor`/`SkiaSharp.NativeAssets.WebAssembly`의 `SKGLView`를 사용한다. Doroti가 frame scheduler, invalidation, context generation/loss와 terminal ACK를 감싸고 scene/widget/state policy는 생성된 C# Doroti framework가 계속 소유한다.
- Doroti browser interop은 별도 JavaScript build toolchain 없이 표준 ES module source로 유지하고 Blazor static web asset으로 패키징한다.

완료 gate:

- selected upstream file/symbol마다 source hash, 참고한 behavior, Blazor/C# local owner, 독립 구현 증거와 reference 제거 조건이 있음
- product source/compile/publish graph의 Avalonia-derived copied source와 `Avalonia`, `Avalonia.Controls`, Avalonia composition binary/package reference 0
- Blazor `SKGLView` WebGL2 context → SkiaSharp surface → Doroti scene draw → invalidate/present/terminal ACK typed chain PASS
- software/Canvas2D fallback과 unclassified browser source dependency 0
- DOM pointer → Blazor/JSImport bridge → C# normalization → `IInputHostCapability.PointerData` → Doroti gesture/state의 typed chain PASS
- dispatcher/input/text/clipboard/cursor capability가 Blazor/C# owner에서 Doroti host-neutral contract에만 연결되고 Avalonia source/runtime dependency 0

#### G7-3A — C# Doroti app/template contract

작업:

- `Doroti.Templates`에 일반 C# app template을 추가하고 `dotnet new doroti-app --name <name>` 또는 동등한 표준 .NET template UX를 제공한다.
- template은 공용 C# Doroti app project와 `Microsoft.NET.Sdk.BlazorWebAssembly` 기반 browser host project를 함께 제공한다. browser host는 `net10.0`/`browser-wasm`을 사용하고 desktop target과 같은 C# app assembly/widget tree를 참조한다.
- G7-3N에서 승격한 `Doroti.Framework.*`, `DorotiHostSession`, `DorotiView`와 `DorotiCapabilityIds`를 사용하고, G7-3에서 새로 만드는 public namespace, type, Razor component, template source와 진단에도 `Doroti` 제품 이름만 사용한다. `DorotiRoot`와 `DorotiSurface`가 Blazor 공개 component 계약을 소유한다.
- template의 `Program.cs`가 `Doroti.Framework.*`의 생성·승격된 C# API로 root widget을 구성한다. 사용자 앱은 Dart source, `pubspec.yaml`, `doroti.yaml`이나 Flutter platform scaffold를 포함하지 않는다.
- target package의 `build`/`buildTransitive` MSBuild props/targets가 Blazor boot resource, static web assets, resource/font/localization과 plugin declaration을 표준 `ItemGroup`/property 계약으로 제공한다.
- template의 allowlist된 `DorotiRoot.razor`/`DorotiSurface.razor`는 canvas host만 구성하며 사용자가 수정할 필요가 없다. `Microsoft.AspNetCore.Components.WebAssembly`는 .NET 10 host와 함께 pin하고, `SkiaSharp.Views.Blazor`/`SkiaSharp.NativeAssets.WebAssembly`는 현재 공용 `SkiaSharp` `3.119.4` baseline과 같은 version set으로 pin한다. version 변경은 WebGL/input/text live evidence를 함께 갱신할 때만 허용한다.
- Blazor router, forms와 component UI library는 기본 graph에 포함하지 않는다. 사용자-visible Doroti widget마다 Razor component/DOM node를 생성하거나 C# widget policy를 Blazor render tree에 복제하지 않는다.
- `dotnet restore`, `dotnet build`와 `dotnet publish` 외 별도 Flutter/Dart command 없이 app을 build할 수 있게 한다.

완료 gate:

- clean 임시 디렉터리에서 배포된 template/package만으로 C# Doroti app 생성 PASS
- generated project의 `.dart`, `pubspec.yaml`, `.metadata`와 Flutter platform directory 0
- product build process tree의 `flutter`, `flutter.bat`, Dart analyzer/compiler 실행 0
- allowlist 밖 `.razor`와 Blazor router/form/component UI dependency 0
- `blazor.webassembly.js`, host component와 SkiaSharp WASM dependency의 version/hash/provenance가 manifest에 고정됨
- Blazor render tree의 user-visible Doroti widget/scene node 0; canvas와 hidden input/accessibility host만 존재
- template source, public Web API, Razor component, manifest와 product diagnostic의 product-facing `Flutter*` identifier 0
- 동일한 C# root widget source가 desktop 및 browser target에 compile됨
- 잘못된 target/resource/plugin 계약은 stable MSBuild 또는 Doroti diagnostic으로 fail-closed

#### G7-3B — C# widget tree to Skia `browser-wasm` product build

작업:

- `Doroti.Target.Web.browser-wasm` composition root가 C# app entrypoint를 `DorotiHostSession`/`DorotiView`에 attach하고 공용 build/layout/paint/state/semantics pipeline을 시작한다.
- `DorotiSurface.razor`가 `SKGLView`를 만들고 `EnableRenderLoop=false`의 명시적 frame invalidation으로 Doroti scene을 `OnPaintSurface`에 제출한다. context generation, resize/DPR, loss/recovery와 terminal frame 상태를 공용 frame contract로 기록한다.
- `DorotiSurface.razor`는 표준 pointer/key/focus event를 C# host로 전달한다. pointer capture, coalesced high-rate move, IME와 context-loss처럼 DOM callback에서 즉시 처리해야 하는 경계만 `[JSImport]`/`JSHost.ImportAsync` module을 사용하고, object-per-event 직렬화 대신 primitive batch/shared buffer 경로를 제공한다.
- `BrowserHostAdapter`가 `IInputHostCapability`를 구현하고 `DorotiCapabilityIds.InputEvents`로 등록된다. Avalonia behavior reference를 기준으로 Blazor용으로 새로 구현한 C# normalization이 `PointerDataPacket`/`KeyData`/`RawFocusData`를 만들어 Doroti gesture binding과 같은 C# widget state에 전달한다.
- Blazor boot loader가 `blazor.webassembly.js`로 managed host를 시작하고 target package가 `index.html`, root/surface component, Skia/input module과 plugin JavaScript module을 static web assets로 제공한다. base path/fingerprinting/hash manifest는 MSBuild publish 단계에서 확정하며 사용자는 runtime bootstrap을 직접 유지하지 않는다.
- `dotnet publish <project> -c Release -r browser-wasm`이 `net10.0` C# app assembly, Doroti framework/target assemblies, Blazor host assemblies, statically linked SkiaSharp WASM native asset, resource와 hash manifest가 있는 deployment-neutral `wwwroot`를 만든다. `canvaskit.js`/`canvaskit.wasm`은 포함하지 않는다.
- app assembly/type, C# source/project identity, target/browser-reference provenance와 static artifact hash를 하나의 release chain으로 기록한다.

완료 gate:

- fixture 문자열이나 target 생성만이 아니라 C# root widget/state/resource가 최종 app assembly와 static artifact에 포함됨
- 실제 Skia GPU draw가 non-empty browser frame을 만들 수 있는 publish graph PASS
- `blazor.webassembly.js` → allowlist된 root/surface component → C# Doroti root mount → SkiaSharp GPU paint의 bootstrap call graph PASS
- Blazor DOM에는 canvas/hidden input/accessibility bridge만 있고 component-per-widget, DOM layout/paint와 CanvasKit runtime 0
- pointer down/move/up/cancel/wheel → C# normalization → `PointerDataPacket` → hit test/gesture/state → 다음 Skia frame의 causal trace가 build fixture에서 연결됨
- `browser-wasm` clean/repeat output identity PASS
- browser graph의 Win32/AppKit/Avalonia UI/native desktop dependency 0
- unsupported plugin/capability silent success, static artifact 누락/hash 불일치와 repository-private fallback 0
- trimming/AOT는 실제 Doroti C# product graph 기준으로 검증하거나 정확한 blocker와 `notVerified`를 유지함

#### G7-3C — external C# product acceptance

작업:

- 저장소 밖 clean root에 `Doroti.Templates`와 promoted NuGet package를 설치하고 template으로 새 C# app을 만든다.
- C# widget source에 사용자-visible state 변화, asset, localization과 Web plugin을 넣고 표준 `dotnet publish -r browser-wasm`을 수행한다.
- 같은 C# app을 두 번 clean publish해 deployment-neutral static artifact와 hash manifest identity를 비교한다.
- output을 isolated static server에 올릴 수 있는 구조인지 검사하되 실제 Chromium GPU/input/ARIA 실행은 G7-4에서 수행한다.

완료 gate:

- repository 밖 C# Doroti project create/restore/build/publish PASS
- Flutter/Dart SDK 또는 CLI 설치 없이 package-only static Web artifact 생성 PASS
- 별도 `dotnet new blazorwasm`, 사용자의 Razor/JavaScript 작성이나 수동 Blazor package 설치 없이 Doroti template 하나로 Blazor WebAssembly host publish PASS
- 사용자가 직접 작성해야 하는 `index.html`, runtime bootstrap JavaScript와 내부 Web host `.csproj` 0
- repository-private/candidate fallback과 source checkout 절대경로 0
- 최종 artifact가 사용자 C# app assembly, Doroti framework, Skia Web backend와 application/resource manifest를 포함함

산출물:

- `Doroti/migration/web/g7-browser-reference-selection.json`
- `Doroti/migration/web/g7-browser-reference-provenance.json`
- `Doroti/templates/Doroti.Templates/content/doroti-app/`
- `Doroti/src/Doroti.Host.Web/`
- `Doroti/src/Doroti.Target.Web.browser-wasm/`
- `Doroti/src/Doroti.Target.Web.browser-wasm/build/` 및 `buildTransitive/` Blazor host/static asset targets
- `Doroti/validation/cases/g7-csharp-web-app/`
- `Doroti/migration/targets/browser-wasm.json`
- `Doroti/migration/web/g7-web-build-evidence.json`
- `Doroti/artifacts/g7-web/<version>/`
- `Doroti/eng/validate-g7-web-build.ps1 -Shard <Toolchain|Reference|Hosting|Graph|Template|Compile|Publish>`

현재 상태(2026-08-15):

- G7-3N naming closure는 `PASS`, G7-3I standalone WebAssembly baseline은 `verified-infrastructure`다.
- 기존 Web evidence는 toolchain/browser adapter/static publish 기반만 증명한다. C# Doroti widget mount, SkiaSharp Blazor GPU frame과 실제 product app은 아직 증명하지 않으므로 G7-3V/A/B/C가 `notVerified`를 소유한다.
- G7-3V/A/B/C가 모두 PASS하기 전에는 G7-4 Chromium live validation에 진입하지 않는다.

구현 순서:

1. G7-3V: Avalonia Browser reference/provenance를 고정하고 Blazor/C# browser capability를 독립 구현한다.
2. G7-3A: 공용 C# app과 내부 Blazor host를 생성하는 `doroti-app` template/package 계약을 닫는다.
3. G7-3B: 같은 C# widget tree를 `SKGLView` WebGL2 surface에 mount/draw/present하고 입력 인과 trace를 연결한다.
4. G7-3C: 저장소 밖 package-only app의 create/restore/build/repeat publish와 artifact identity를 통과한 뒤 G7-4로 넘긴다.

### G7-4 — Web live product parity

진입 조건: G7-3V/G7-3A/G7-3B/G7-3C 완료. infrastructure probe만으로는 진입할 수 없다.

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

- `Doroti/validation/Doroti.Validation.G7Web/`
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

완료된 선행 조건은 G7-0, G7-1, G7-2, G7-3M, G7-3N이며 G7-3I는 Web infrastructure baseline으로만 승계한다.

```text
G7-3V browser reference/capability + G7-3A C# app/template
  -> G7-3B C# widget tree + Skia Web build
  -> G7-3C external C# product publish
  -> G7-4 Chromium live product
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
./Doroti/eng/validate-g7-web-build.ps1 -Shard Reference
./Doroti/eng/validate-g7-web-build.ps1 -Shard Hosting
./Doroti/eng/validate-g7-web-build.ps1 -Shard Graph
./Doroti/eng/validate-g7-web-build.ps1 -Shard Template
./Doroti/eng/validate-g7-web-build.ps1 -Shard Compile
./Doroti/eng/validate-g7-web-build.ps1 -Shard Publish

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

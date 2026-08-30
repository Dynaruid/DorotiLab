# Doroti Web renderer 구조 개편 작업 계획

- 작성일: 2026-08-30
- 상태: `implemented-qualification-failed` — W0, D0, W1~W6 구현과 자동 검증은 완료; W7 current/direct A/B에서 1회 max latency gate가 실패했고 물리 acceptance는 `notVerified`이므로 계획의 중단 조건에 따라 W8 cutover/legacy 삭제는 수행하지 않음
- 현재 제품 기본값: `document-webgl`
- 우선 후보: `worker-direct-webgl`
- 첫 qualification 범위: Playwright bundled Chromium, hardware WebGL2, full-page single view

## 1. 결론

`worker-direct-webgl`은 다음 구조 문제를 없앨 수 있는 유력한 후보이므로 구현 가치가 있다.

- Worker에서 완성한 frame을 `createImageBitmap`으로 capture한다.
- transferable message와 main-thread task queue를 지난다.
- main의 `bitmaprenderer.transferFromImageBitmap`으로 다시 표시한다.
- display receipt가 돌아올 때까지 bitmap과 managed completion을 함께 추적한다.

다만 현재 증거만으로 기존 renderer를 먼저 삭제하면 안 된다. 이전 A/B에서 `document-webgl`의 wheel latency가 세 경로 중 가장 좋았고, direct transferred canvas에 실제 Doroti .NET/Skia runtime을 결합한 결과는 아직 `notVerified`다. 따라서 다음 순서를 고정한다.

1. 최소 fixture로 transferred visible canvas, Worker rAF, Emscripten WebGL2, context recovery와 Worker 재시작을 먼저 증명한다.
2. 기존 shared scheduler를 유지한 채 `worker-direct-webgl`을 opt-in backend로 만든다.
3. correctness, latency, resize continuity, 실제 입력·IME·고주사율 acceptance를 통과한다.
4. `auto` 기본값을 바꾸고 burn-in한다.
5. 지원 브라우저 정책을 결정한 뒤에만 기존 경로를 삭제하거나 하나의 compatibility surface로 축소한다.

새 `WebFrameCoordinator`를 semantic frame owner로 추가하지 않는다. 이미 `SchedulerBinding → PlatformDispatcher → BrowserHostAdapter → SkiaSceneRenderer`가 frame 요청, latest view-epoch admission, scene mailbox와 terminal의 의미를 소유한다. Worker rAF와 direct WebGL surface는 이 흐름 아래의 host transport/backend가 된다.

Doroti 공용 계층도 그대로 둘 수 있는 것은 아니다. scheduler, view epoch와 terminal 종류는 이미 충분하지만, 장기 실행 시 무한히 커지는 terminal ledger, `submitted`를 `presented`로 세는 Skia 진단, replay attempt와 scene terminal을 구별하지 못하는 receipt, Web에서 끊어진 공용 frame trace는 direct backend 전에 바로잡아야 한다.

## 2. 현재 증거와 판정 경계

아래 표는 구현 전 판정 경계를 기록한 baseline이다. 이번 실행에서 얻은 최종 결과와 gate 판정은 [10. 2026-08-30 실행 결과](#10-2026-08-30-실행-결과)가 갱신한다.

| 항목 | 현재 판정 | 근거와 한계 |
|---|---|---|
| same-generation interaction frame 표시 | `PASS` | `requestId` 단조 증가, generation 비감소, exact size admission으로 기능 회복 |
| Release/TypeScript/FCR/resize contract | 기록상 `PASS` | 이번 계획 작성 turn에서는 재실행하지 않음 |
| 기존 `document-webgl` A/B | 기록상 `PASS` | wheel median p95 26.2 ms |
| 기존 `offscreen-bitmap` A/B | 기록상 `FAIL` | wheel median p95 49.1 ms |
| 기존 `offscreen-worker` A/B | 기록상 `FAIL` | wheel median p95 44.1 ms |
| 새 `worker-direct-webgl` | 구현 전 `notVerified` | 이번 실행에서 opt-in으로 구현했으며 최종 automated qualification은 section 10의 `partial/FAIL` |
| 실제 window border drag/trackpad/120 Hz 이상/scan-out | `notVerified` | browser 내부 commit이나 screenshot으로 대체하지 않음 |
| 실제 한글 IME/길게 누른 Backspace/screen reader | `notVerified` | 자동 DOM·semantics 검사와 별도 gate |

기존 수치의 저장소 근거는 [web-offscreen-worker-renderer.md](history/26-08-29/web-offscreen-worker-renderer.md)다. 기능 회복 수치도 W0에서 동일 command와 artifact schema로 다시 고정한다.

## 3. 현재 구조와 남은 문제

### 3.1 현재 renderer별 경계

| 경로 | .NET/Skia 위치 | visible submit | 구조적 경계 |
|---|---|---|---|
| `document-webgl` | main thread | document canvas WebGL2 | DOM/input과 build/layout/raster가 main rAF에서 경쟁 |
| `offscreen-bitmap` | main thread | main `bitmaprenderer` | detached OffscreenCanvas capture와 bitmap commit |
| `offscreen-worker` | persistent Worker | main `bitmaprenderer` | metrics/input 왕복, bitmap transfer, display receipt |

기능 장애의 직접 원인은 고쳤지만, 세 presenter가 frame admission, resize generation, context loss와 terminal을 서로 다르게 구현한다. 특히 Worker 경로는 app/framework/Skia를 이미 Worker에 옮기고도 마지막 visible submit만 bitmap 왕복으로 남겨 둔다.

### 3.2 현재 실제 owner

| 책임 | 현재 owner | 계획에서의 처리 |
|---|---|---|
| renderer 선택과 runtime 시작 | [doroti.loader.ts](Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts) | opt-in direct mode를 먼저 추가하고 cutover 전까지 현 기본값 유지 |
| DOM/input/IME/semantics/metrics/main rAF/presenter | [doroti.web.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts) | 역할별 module로 분리하고 얇은 public facade만 유지 |
| Worker .NET bootstrap/bitmap protocol | [doroti.raster.worker.ts](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts) | direct Worker bootstrap과 versioned router로 재구성 |
| host rAF와 latest view-epoch admission | [BrowserHostContracts.cs](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs)의 `BrowserHostAdapter` | semantic owner를 유지하고 Worker rAF transport에 연결 |
| framework frame scheduling | [PlatformDispatcher.cs](Doroti/src/Doroti.Ui/PlatformDispatcher.cs), [binding.cs](Doroti/src/Doroti.Framework.Scheduler/binding.cs)의 `SchedulerBinding` | Web 전용 coordinator로 복제하지 않음 |
| scene mailbox와 terminal accounting | [BrowserSkiaCapabilities.cs](Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs), [SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) | causal frame ID와 명시적 terminal을 연결 |
| Worker canvas/Emscripten GL context | [doroti.raster.worker.ts](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts)의 `ensurePresenter` | detached canvas 생성 대신 transferred visible canvas를 받아 context/backing/surface generation을 소유 |
| managed Skia framebuffer wrapper | [DorotiWebWorkerSurface.cs](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs) | 전달받은 framebuffer ID를 감싸는 기존 역할을 유지하고 causal/terminal/generation signature만 필요한 만큼 조정 |
| same-thread staging/exact blit | [DorotiWebGlSurface.razor](Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor), [doroti.web.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts) | 검증된 부분만 Worker direct surface로 이동 |

### 3.3 구현 전에 닫아야 할 계약 구멍

1. `BrowserSkiaCapabilities.HostBridge.InputSequence`가 현재 `0`을 반환한다. 모든 pointer/wheel/key/focus/text/semantics action이 main에서 부여한 공통 sequence를 managed dispatch까지 보존해야 한다.
2. `BrowserSkiaCapabilities.Paint`가 이미 존재하는 `SkiaSceneRenderer.Paint(..., causalFrameId)` 경로를 사용하지 않는다. 현재 JS `requestId` 또는 그 replacement가 host/surface submit attempt의 causal ID가 되도록 연결하되, framework frame identity와 혼동하지 않는다.
3. managed `CompleteFrame(..., bool committed, ...)`만으로는 `superseded`와 `failed`를 구별할 수 없다. 명시적 terminal enum과 reason이 필요하다.
4. main metrics snapshot과 Worker raster resize가 `surfaceGeneration`이라는 이름을 나누어 사용한다. 기존 `DorotiViewEpoch.ResizeTargetGeneration`과 `MetricsGeneration`을 그대로 보존하고, 여기에 Worker `surfaceGeneration`과 `contextGeneration`을 서로 다른 owner/단조성으로 연결한다.
5. TypeScript discriminated union은 compile-time 계약일 뿐이다. protocol version, message kind, sequence, transferable lifecycle과 허용 state transition을 runtime validator가 검사해야 한다.
6. transferred canvas는 죽은 Worker에서 회수할 수 없다. canvas node만 교체하면 기존 `BrowserHost`가 가진 참조와 listener가 남으므로 DOM endpoint 재바인딩 또는 host 전체 재생성이 필요하다.
7. [reference Flutter app](reference/flutter_sample_app/lib/main.dart)은 현재 기본 counter fixture다. Doroti와 같은 workload로 바꾸기 전에는 Flutter differential benchmark가 아니다.

### 3.4 Doroti 자체 감사 판정

결론은 “공용 scheduler/capability 재설계는 불필요하지만 shared lifecycle과 diagnostics는 보강 필요”다. Worker 전용 개념을 `Doroti.Ui`에 올리는 대신 다음 경계를 지킨다.

| 영역 | 판정 | 이 계획의 처리 |
|---|---|---|
| `DorotiViewEpoch → DorotiSceneBuildToken → DorotiFrameDescriptor` | 유지 | immutable framework identity와 submit-time relabel 금지가 이미 구현됨; surface/context/runtime/causal identity를 descriptor에 추가하지 않음 |
| `IFrameHostCapability/IExactFrameHostCapability/ILatestMetricsFrameHostCapability`와 `SchedulerBinding` | 유지 | 새 Worker capability나 coordinator를 만들지 않고 실제 latest-epoch admission 통합 fixture만 추가 |
| `DorotiLatestFrameMailbox`와 5개 `DorotiFrameTerminal` | 유지 | current+latest bound와 `presented/submitted/superseded/dropped/failed` 집합을 재사용 |
| `DorotiFrameTerminalLedger` | **공용 수정 필요** | 현재 completed ID를 영구 보관하는 두 collection을 active scene + 누적/per-terminal counter + bounded recent history로 바꾸고 `registered = completed + active`를 증명 |
| `SkiaFrameDiagnostics`와 `SkiaFrameReceipt` | **공용 수정 필요** | scene admission, `submitted`, `presented`를 서로 다른 counter로 만들고 receipt를 causal paint-attempt receipt로 명문화; replay가 scene terminal을 다시 세지 않도록 `IsNewFrame` 또는 disposition과 reason 추가 |
| `DorotiFrameTrace` | **공용 진단 보강 필요** | 기존 bounded metadata ring은 유지하고 resize-target/metrics generation, framework frame number, causal frame ID와 context generation을 optional correlation field로 추가 |
| Web input sequence, trace attach, typed completion과 context-loss wiring | **Web host 수정 필요** | 공용 input event 형식은 바꾸지 않고 `Doroti.Host.Web` protocol/adapter에서 sequence와 clock-domain을 보존 |
| `runtimeSessionId`, external lease, canvas transfer/rebind | **Web 전용** | 죽은 Worker 밖의 supervisor 계약으로 유지하고 Doroti scene descriptor/ledger 안에 넣지 않음 |

`FrameworkFrameNumber`의 실제 owner는 `PlatformDispatcher`다. `SchedulerBinding`의 debug frame number를 제품 identity로 사용하지 않는다. `inputSequence`도 build descriptor에 합치지 않고 host ingress와 Skia receipt/trace에서 join한다. 현재 첫 qualification은 single view이므로 전역 `_hasScheduledFrame`과 multi-view fan-out은 이번 변경 대상이 아니며, embedded/multi-view 지원을 열 때 공용 scheduler를 다시 감사한다.

## 4. 레퍼런스 조사 결과

조사는 blog나 예제 설명보다 실제 활성 source path를 우선했다. 외부 HEAD는 아래 commit으로 고정하고, Flutter와 SkiaSharp는 저장소가 사용하는 revision/package version에 맞췄다.

| 레퍼런스 | 확인한 현재 구조 | 채택할 원칙 | 그대로 채택하지 않을 것 |
|---|---|---|---|
| Avalonia `fc5923acf2...` | document canvas를 `transferControlToOffscreen()`으로 render pthread Worker에 넘기고 Worker rAF에서 direct WebGL/Skia 실행 | transferred visible canvas, Worker rAF, 좁은 render surface, demand-driven loop, pending batch 하나 | threaded + managed-dispatcher 구성의 main DOM + UI/app managed Worker + 별도 render pthread Worker와 cross-origin-isolation 제약을 Doroti 단일 Worker의 근거로 사용하지 않음 |
| Uno `cf65a2828...` | main/UI dispatcher, document canvas, `window.requestAnimationFrame`; `IBrowserRenderer` 뒤에 WebGL/software backend | 작은 backend interface, duplicate invalidation coalescing, latest retained frame, superseded resource 즉시 release | Uno를 OffscreenCanvas Worker 사례로 표현하지 않으며 main-thread topology를 복사하지 않음 |
| Flutter source reference `56b8e1a8...` | main `FrameService`가 browser rAF를 받고 Skwasm raster Worker가 OffscreenCanvas를 raster한 뒤 `ImageBitmap`을 on-screen canvas/`bitmaprenderer`에 전달 | `Rasterizer/ViewRasterizer/Surface/DisplayCanvas` 분리, current+next 교체, per-view lifecycle, 비교 가능한 evidence discipline | Flutter가 direct visible canvas 또는 app/framework/raster 단일 Worker라고 주장하지 않음 |
| SkiaSharp 4.151.1 `279f93f4...` | platform/JS가 rAF·canvas size·GL context를 소유하고 managed view가 `GRContext/GRBackendRenderTarget/SKSurface`를 생성·재생성·flush | context/surface를 host lifetime에 묶는 좁은 GPU wrapper와 size-change recreation | SkiaSharp가 resize epoch, scheduler, terminal, crash recovery나 scan-out을 보장한다고 간주하지 않음 |

### 4.1 Avalonia

- [canvas transfer registry](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Browser/Avalonia.Browser/webapp/modules/avalonia/rendering/webRenderTargetRegistry.ts#L15-L60)는 main에서 canvas control을 한 번 이전한다.
- [WebGL render target](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Browser/Avalonia.Browser/webapp/modules/avalonia/rendering/webGlRenderTarget.ts#L28-L68)은 같은 Worker에서 context를 만든다.
- [Worker timer](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Browser/Avalonia.Browser/webapp/modules/avalonia/timer.ts#L3-L10)는 `self.requestAnimationFrame`을 사용한다.
- [Compositor](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Avalonia.Base/Rendering/Composition/Compositor.cs#L123-L196)는 `Processed` 전 pending batch를 하나로 제한하고, [CompositionBatch](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Avalonia.Base/Rendering/Composition/Transport/Batch.cs#L33-L65)는 `Processed`와 `Rendered`를 구분한다.
- [RenderLoop](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Avalonia.Base/Rendering/RenderLoop.cs#L107-L170)는 작업이나 animation이 있을 때만 loop를 깨우는 demand-driven 구조다.
- [RawEventGrouping](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Shared/RawEventGrouping.cs#L137-L194)은 FIFO를 지키고 인접한 동일 device/type/modifier의 Move/TouchUpdate만 제한적으로 합친다.

이 자료는 direct transferred canvas의 실용성을 뒷받침한다. 그러나 [BrowserAppBuilder](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Browser/Avalonia.Browser/BrowserAppBuilder.cs#L92-L125)와 [RenderWorker](https://github.com/AvaloniaUI/Avalonia/blob/fc5923acf2afcd2be588d5c285eda8a1a14af228/src/Browser/Avalonia.Browser/Rendering/RenderWorker.cs#L22-L50)가 보여 주듯 UI/app Worker와 render Worker가 분리될 수 있다. Doroti의 “runtime 하나와 GPU surface 하나를 같은 Worker에 둔다”는 별도 가설로 검증한다.

### 4.2 Uno

- [BrowserRenderer.cs](https://github.com/unoplatform/uno/blob/cf65a2828cf21f88ca3074d9040cd46d9ebbffaf/src/Uno.UI.Runtime.Skia.WebAssembly.Browser/Rendering/BrowserRenderer.cs#L47-L57)는 pending invalidation을 하나로 합치고, [BrowserRenderer.ts](https://github.com/unoplatform/uno/blob/cf65a2828cf21f88ca3074d9040cd46d9ebbffaf/src/Uno.UI.Runtime.Skia.WebAssembly.Browser/ts/Runtime/BrowserRenderer.ts#L17-L50)는 이를 `window.requestAnimationFrame`에 예약한다.
- [IBrowserRenderer](https://github.com/unoplatform/uno/blob/cf65a2828cf21f88ca3074d9040cd46d9ebbffaf/src/Uno.UI.Runtime.Skia.WebAssembly.Browser/Rendering/IBrowserRenderer.cs#L5-L11)는 `MakeCurrent`, `Resize`, `Flush`, `NeedsForceResize`만 노출하는 작은 backend 경계다.
- [CompositionTarget rendering](https://github.com/unoplatform/uno/blob/cf65a2828cf21f88ca3074d9040cd46d9ebbffaf/src/Uno.UI/UI/Xaml/Media/CompositionTarget.Rendering.skia.cs#L114-L179)은 queued latest `SKPicture` slot 하나를 유지하며, present 중 새 frame이 오면 빌려 간 이전 frame을 반환 단계에서 결정적으로 release한다.
- [WebGlBrowserRenderer.ts](https://github.com/unoplatform/uno/blob/cf65a2828cf21f88ca3074d9040cd46d9ebbffaf/src/Uno.UI.Runtime.Skia.WebAssembly.Browser/ts/Runtime/WebGlBrowserRenderer.ts#L7-L54)는 현재 활성 경로가 main document canvas임을 보여 준다.

Uno에서 가져올 것은 Worker topology가 아니라 작은 renderer interface와 scheduling/resource 규율이다.

### 4.3 Flutter

- `reference/flutter-master`와 [flutter-sdk.ps1](Doroti/eng/flutter-sdk.ps1)의 source pin은 `56b8e1a851a594b1a154f8ea93270807dab22b9a`다. 반면 [sample app metadata](reference/flutter_sample_app/.metadata)는 생성 revision `6b182d2c...`를 기록한다. 둘 다 실제 benchmark SDK 증거로 대신하지 않고 W0에서 `flutter --version --machine` 출력과 실제 renderer를 artifact로 고정한다.
- [FrameService](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/frame_service.dart#L89-L129)는 main `window.requestAnimationFrame`에서 framework begin/draw를 시작한다.
- [Rasterizer와 RenderQueue](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/rasterizer.dart#L182-L190)는 engine frame과 view별 raster lifecycle을 분리하고 current+next replacement를 사용한다.
- [Surface](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/surface.dart)와 [RenderCanvas](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/render_canvas.dart#L101-L151)는 raster target과 visible display canvas를 별도 계약으로 두고 bitmap transfer 전 visible canvas backing을 exact size로 맞춘다.
- [OffscreenCanvasRasterizer](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/offscreen_canvas_rasterizer.dart#L67-L99)는 raster Worker의 bitmap을 visible canvas에 전달한다.
- [Skwasm surface](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/skwasm/skwasm_impl/surface.dart)는 `ImageBitmap` 반환 경계를 가진다.
- [Flutter Web initialization](https://docs.flutter.dev/platform-integration/web/initialization)은 multi-threaded Skwasm과 single-thread fallback 지원 조건을 설명한다.

Flutter의 bitmap 경계는 platform view overlay와 multi-view를 포함한 범용 구조의 선택이다. Doroti 첫 범위는 full-page single view이므로 그 경계를 제거해 이득을 얻을 가능성이 있지만, 이는 Flutter 복제가 아니라 범위를 좁힌 Doroti 전용 최적화다.

### 4.4 SkiaSharp와 Web platform

- Doroti가 pin한 version은 [Directory.Packages.props](Doroti/Directory.Packages.props)의 SkiaSharp 4.151.1이다.
- [SKGLView 4.151.1](https://github.com/mono/SkiaSharp/blob/279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764/source/SkiaSharp.Views/SkiaSharp.Views.Blazor/SKGLView.razor.cs)은 host가 준 GL framebuffer를 `GRBackendRenderTarget`과 `SKSurface`로 감싸고 size/context 변화에 맞춰 재생성한다.
- pinned Skia의 [CanvasKit FBO 0 direct wrap](https://github.com/google/skia/blob/bdd0c3a8eaba1afa7148f02bba3a07f94e682847/modules/canvaskit/canvaskit_bindings.cpp#L295-L327)은 default framebuffer direct surface가 가능한 경로임을 보여 준다. 따라서 staging FBO는 필수 전제가 아니라 W1에서 direct FBO 0과 비교할 후보이다.
- [HTML canvas specification](https://html.spec.whatwg.org/multipage/canvas.html)은 `OffscreenCanvas` transfer와 placeholder canvas 갱신 의미를 정의한다. `transferControlToOffscreen()`은 context 생성 전에 한 번만 호출해야 한다.
- [HTML event loops and animation frames](https://html.spec.whatwg.org/multipage/webappapis.html)는 Worker animation-frame processing의 기준이다.

Web API에는 compositor scan-out ACK가 없다. direct surface의 exact blit은 `submitted`일 뿐 물리 display에 보였다는 증거가 아니므로 `browser-present-unverified`를 유지한다.

## 5. 목표 구조

```text
Browser main thread
  DOM endpoints
  input / IME / semantics / clipboard / plugin
  ResizeObserver / visualViewport / DPR
  canvas creation-transfer-replacement + Worker supervision
                    │
                    │ versioned runtime-validated protocol
                    ▼
Persistent Doroti Worker
  .NET runtime exactly 1
  BrowserHostAdapter: Worker rAF transport + latest view-epoch admission
  SchedulerBinding: framework phase/build-layout scheduling owner
  PlatformDispatcher: admitted epoch + framework frame number owner
  BrowserSkiaCapabilities / SkiaSceneRenderer: scene + terminal owner
                    │
                    ▼
Direct Web surface
  transferred visible OffscreenCanvas
  WebGL2 context + staging/default framebuffer transaction
  exact submit; no ImageBitmap/bitmaprenderer receipt
```

### 5.1 책임 경계

Main thread:

- root/canvas/IME/semantics DOM과 browser event listener
- pointer, wheel, key, focus, composition, text와 semantics action sampling
- clipboard와 DOM plugin request 처리
- `ResizeObserver`, `visualViewport`, DPR 관측과 immutable metrics publish
- canvas 생성, 최초 transfer, fatal 시 canvas 필수 교체와 W1에서 증명한 DOM endpoint rebind 또는 host recreation
- Worker supervision과 최대 1회 bounded restart

Worker JavaScript:

- versioned protocol decode와 runtime transition validation
- .NET runtime bootstrap
- Worker-local rAF를 managed `BrowserHostAdapter.ScheduleFrame`에 전달
- transferred canvas, WebGL2 context, GPU surface와 context generation
- direct submit diagnostics와 context loss/restore orchestration

Managed shared runtime:

- `SchedulerBinding`: framework phase/build-layout scheduling, `PlatformDispatcher`: admitted epoch와 framework frame number
- `BrowserHostAdapter`: pending callback 하나와 latest `DorotiViewEpoch` admission
- `BrowserSkiaCapabilities/SkiaSceneRenderer`: scene mailbox, causal identity, exactly-once terminal
- widget, render tree, text/semantics tree와 Skia draw commands

새 helper가 필요하더라도 이름과 역할은 `DirectSurfaceLifecycle` 또는 `WorkerHostStateMachine`처럼 제한한다. DOM, framework scheduling, scene build, GL submit, terminal, crash recovery를 한 class에 모은 `WebFrameCoordinator`는 만들지 않는다.

### 5.2 TypeScript module 구조

파일명은 구현 중 기존 generated asset 규칙과 bundle 방식을 확인한 뒤 확정하되 책임은 다음처럼 나눈다.

| module | 책임 |
|---|---|
| `doroti.web.protocol.ts` | message union, protocol version, runtime validator, transition table |
| `doroti.web.dom.ts` | DOM endpoints, input/IME/semantics, metrics observer, host snapshot |
| `doroti.web.worker-host.ts` | canvas transfer, Worker lifecycle, restart와 canvas replacement/endpoint rebind 정책 |
| `doroti.web.worker.ts` | Worker .NET bootstrap, protocol router, managed dispatch, Worker rAF transport |
| `doroti.web.surface.ts` | transferred canvas WebGL2, staging/default framebuffer, resize/context lifecycle |
| `doroti.web.diagnostics.ts` | counters, timestamps, capability identity, artifact snapshot |
| `doroti.web.ts` | stable public imports/JSInvokable facade와 re-export만 유지 |

`doroti.raster.worker.ts`는 한 번에 복제하지 않는다. W2에서 protocol/bootstrap/surface를 추출한 뒤 새 Worker entry로 rename하거나 제거한다. 같은 state machine을 두 파일에 유지하는 중간 상태를 release하지 않는다.

### 5.3 identity와 terminal

| identity | owner | 의미 |
|---|---|---|
| `inputSequence` | main DOM host | 모든 ordered input/action의 단조 증가 sequence |
| `resizeTargetGeneration` | main DOM metrics observer | 새 logical/physical size와 DPR target의 단조 generation |
| `metricsGeneration` | host snapshot/framework delivery boundary | 같은 target이 framework metrics로 전달된 generation; `DorotiViewEpoch`에서 target generation과 함께 보존 |
| `frameworkFrameNumber` | `PlatformDispatcher` | `DorotiSceneBuildToken`이 capture한 framework build frame identity; SchedulerBinding의 debug counter와 별개 |
| `sceneSequence` | `SkiaSceneRenderer` | scene mailbox와 resource lifetime |
| `causalFrameId` | Worker host/direct surface | 한 host/GPU submit attempt를 managed paint, raster와 submit timestamp에 join |
| `surfaceGeneration` | Worker direct surface | backing/surface size transaction |
| `contextGeneration` | Worker GL lifecycle | context loss/restore 전후 GPU resource 구분 |
| `runtimeSessionId` | main Worker supervisor | fatal/restart 전후 external lease namespace 구분 |

direct path에서 bitmap display receipt는 사라지지만 host submission causal identity는 남는다. 현재 JS `requestId`가 이 역할을 이미 수행할 수 있는지 확인해 `causalFrameId`로 연결하거나 rename하며, 같은 submit attempt에 두 단조 counter를 만들지 않는다. input→framework build는 `inputSequence + frameworkFrameNumber`로, build→GPU submit은 `sceneSequence + causalFrameId`로 join한다.

각 admitted scene은 기존 `DorotiFrameTerminal` 집합에서 정확히 한 terminal만 가진다.

- `presented`: 실제 present ACK가 있는 host만 사용. Web direct path에서는 browser scan-out으로 오인해 사용하지 않음.
- `submitted`: Worker에서 exact framebuffer/GPU submit까지 완료. compositor나 scan-out을 의미하지 않음.
- `superseded`: raster 시작 전에 newer admissible scene으로 교체.
- `dropped`: 정상 dispose/shutdown처럼 더 이상 submit하지 않기로 종료.
- `failed`: context, resource 또는 protocol 오류로 submit 불가. reason 필수.

살아 있는 Worker 안에서는 managed ledger가 exactly-once terminal을 소유한다. 강제 종료된 Worker는 callback을 실행할 수 없으므로 main supervisor가 `runtimeSessionId + causalFrameId` 외부 lease를 mirror한다. fatal 시 main이 관측한 open lease만 `failed(runtime-lost)`로 한 번 닫고, 죽은 runtime 내부 ledger가 정산됐다고 주장하지 않는다. 정상 dispose는 Worker가 managed scene을 `dropped`로 닫은 ACK 뒤에 종료한다. context restore 중 새 scene을 무한 대기시키지 않고 current+latest bound를 유지한다.

### 5.4 scheduling과 input

- Worker rAF는 기존 `IFrameHostCapability/ILatestMetricsFrameHostCapability` 구현을 깨우는 transport다.
- framework가 frame을 요청하지 않았고 animation도 없으면 rAF loop를 상시 회전시키지 않는다.
- 한 rAF에 pending framework callback은 하나이며, callback 시작 시 `ResizeTargetGeneration + MetricsGeneration`을 보존한 가장 최신 admissible `DorotiViewEpoch` 하나를 선택한다.
- main→Worker transport는 현재 필요한 `one in-flight + one latest` bound를 유지하고 각 slot을 따로 계측한다. Worker에서 framework admission을 기다리는 epoch는 latest 하나뿐이다.
- raster 중 새 view epoch가 오면 current immutable frame은 끝내고 latest 하나만 다음 rAF에 남긴다.
- down/up/cancel/wheel/key/text/focus/semantics action의 FIFO는 보존한다.
- 같은 pointer의 연속 move만 pointer별 latest/coalesced samples로 줄일 수 있다. down/up 경계를 넘겨 합치지 않는다.
- input arrival이 이미 예약된 frame을 중복 예약하지 않되, raster 중 arrival은 다음 frame dirty 상태를 반드시 남긴다.

### 5.5 resize transaction

목표는 “모든 intermediate frame이 항상 pixel-exact”가 아니라 “browser box는 즉시 따라가고 다음 유효 refresh에서 exact frame으로 수렴”이다.

1. main은 root/canvas CSS box를 viewport와 즉시 맞추고 `ResizeTargetGeneration`과 `MetricsGeneration`을 축약하지 않은 immutable view-epoch packet을 publish한다.
2. Worker는 latest view epoch만 framework에 admission하고 하나의 rAF transaction에서 backing/surface recreation, raster와 exact submit을 완료한다.
3. transaction 도중 partial/default framebuffer가 compositor에 관찰되지 않는지 최소 fixture로 검증한다.
4. 이전 front의 compositor scaling은 exact submit 전 최대 1 measured refresh만 잠정 허용한다.
5. final CSS size, backing size, Skia surface와 admitted metrics는 정확히 일치해야 한다.

staging FBO를 resize 전에 미리 raster한 뒤 backing을 바꾸어 blit할 수 있다고 가정하지 않는다. canvas backing resize가 GL resource/state에 미치는 영향을 W1에서 확인하고 다음 둘 중 증명된 transaction만 채택한다.

- 새 크기 staging을 준비한 뒤 backing resize와 exact blit을 같은 Worker task에서 수행
- rAF 안에서 backing/context-dependent resource를 먼저 재생성하고 raster+blit을 끝낸 뒤 task 반환

둘 다 black/root band, partial frame 또는 1 refresh 초과 stale scale을 막지 못하면 resize 정책을 재설계하고 direct cutover를 중단한다. JS transform/bitmap preview를 몰래 추가해 gate를 우회하지 않는다.

### 5.6 context loss와 Worker fatal

- WebGL context loss는 같은 Worker와 transferred canvas 안에서 shared GPU resource를 먼저 invalidate하고 `contextGeneration/surfaceGeneration`을 올린다. 중단된 새 scene은 `failed`로 한 번 닫고 새 framework frame을 요청해 새 `sceneSequence`로 rebuild한다. replay는 마지막으로 성공한 retained scene에만 허용한다.
- restore가 불가능한 fatal이면 기존 canvas를 재사용하지 않는다.
- canvas는 반드시 교체한다. root/IME/semantics endpoint는 W1에서 증명한 방식에 따라 기존 node에 새 canvas/listener를 rebind하거나 host 전체를 재생성한다.
- 자동 restart는 최대 1회이며 restart loop를 만들지 않는다.
- app state 복원은 별도 persistence 계약이 없으면 보장하지 않는다.
- main이 소유한 old Worker handle, listener, callback과 external lease가 0임을 registry counter로 증명한다. 종료된 Worker 내부 GPU/managed object의 GC 완료나 transferable 회수는 관측 가능하다고 주장하지 않는다.

## 6. 실행 단계

각 단계는 독립 gate를 가진다. W0 뒤에는 D0 공용 보강과 W1 feasibility를 병행할 수 있지만 W3 결합은 D0, W1과 W2가 모두 통과한 뒤 시작한다. 앞 gate가 실패하면 의존 단계, 특히 default cutover와 legacy 삭제로 진행하지 않는다.

### W0. baseline, fixture와 지원 결정을 고정

작업:

1. 현재 dirty tree를 기록하고 사용자 변경을 보존한다.
2. [기존 A/B 기록](history/26-08-29/web-offscreen-worker-renderer.md)의 build, correctness, latency를 현재 Release build에서 재수집한다.
3. [Flutter reference app](reference/flutter_sample_app/lib/main.dart)을 Doroti fixture와 같은 grid/scroll/button/text field/effect 부하로 만든다. framework 기능 구현 비교가 아니라 browser frame pipeline 비교만 포함한다.
4. `flutter --version --machine`의 framework/engine/Dart revision과 실제 renderer를 기록한다. sample app `.metadata`나 source checkout pin을 실행 SDK 증거로 사용하지 않는다.
5. Playwright bundled Chromium version, OS, GPU/driver, DPR, display refresh, power mode와 Doroti commit을 artifact에 기록한다.
6. first interaction과 warm interaction을 분리하고 JIT/GC/shader compile event는 관측 가능한 경우 별도 기록한다.
7. 재현 가능한 differential harness `Doroti/eng/run-web-flutter-differential.ps1`과 matching input script/artifact schema를 만든다.
8. 제품 지원 결정을 ADR 초안에 남긴다.
   - A: Playwright bundled Chromium/hardware WebGL2만 qualification target으로 두고 capability 실패 시 fail-closed
   - B: 동일 scheduler/protocol 아래 최소 compatibility surface 유지

Gate:

- current `auto=document-webgl` correctness가 그대로 `PASS`
- 세 기존 mode의 baseline artifact 생성
- Flutter와 Doroti workload/viewport/input script가 동등함
- 실행한 Flutter framework/engine revision과 renderer가 command output으로 고정됨
- 실제 scan-out이 측정되지 않는다는 limitation 기록

### D0. Doroti 공용 lifecycle과 diagnostics 보강

이 단계는 Worker 구현이 아니라 기존 `Doroti.Ui`/`Doroti.Skia.Rendering` 계약의 장기 실행 안전성과 증거 정확성을 고친다. W1과 병행할 수 있지만 W3 전에 반드시 통과한다.

작업:

1. [ResizeLifecycle.cs](Doroti/src/Doroti.Ui/ResizeLifecycle.cs)의 `DorotiFrameTerminalLedger`를 active scene만 identity로 보관하고 completed 상태는 누적/per-terminal counter와 bounded recent ring으로 제한하는 구조로 바꾼다. `Snapshot()` 전체-history 의존 validation은 bounded diagnostics snapshot으로 이관한다.
2. registration identity의 단조성, duplicate register/terminal 거부와 out-of-order completion을 함께 보존한다. bounded history eviction 뒤에도 이미 완료된 scene이 다시 terminal 처리되지 않게 한다.
3. [SkiaRendererContracts.cs](Doroti/src/Doroti.Skia.Rendering/SkiaRendererContracts.cs)의 diagnostics를 `sceneAccepted`, terminal `submitted`, terminal `presented`처럼 의미가 겹치지 않는 counter로 분리한다. 성공 terminal은 상호 배타적이며 전체 성공은 두 terminal counter의 합으로 계산한다.
4. `LastSubmittedInputSequence`는 `submitted` terminal에서만, `LastPresentedInputSequence`는 `presented` terminal에서만 갱신한다. scene admission watermark가 필요하면 별도 이름으로 둔다.
5. `SkiaFrameReceipt`를 scene-terminal event가 아닌 causal paint-attempt receipt로 명문화하고 `IsNewFrame` 또는 disposition, reason을 추가한다. replay receipt는 생성하되 원 scene ledger와 per-terminal scene counter는 다시 닫거나 올리지 않는다.
6. admission 실패, raster 실패, exact submit, supersede와 renderer dispose가 하나의 terminal accounting helper를 통과하게 한다. 새 terminal enum이나 context-loss retry/requeue API는 만들지 않는다.
7. 기존 [FrameLifecycle.cs](Doroti/src/Doroti.Ui/FrameLifecycle.cs)의 bounded metadata ring에 optional resize-target/metrics generation, framework frame number, causal frame ID와 context generation을 추가하고 `PlatformDispatcher`/`SkiaSceneRenderer`에서 소유한 값만 기록한다.
8. [fcr3-scheduler](Doroti/validation/fcr3-scheduler/Program.cs)에 실제 `ILatestMetricsFrameHostCapability` fixture를 추가한다. epoch A로 요청 후 callback 전에 B로 갱신했을 때 pending callback 하나, B build token, framework frame 1회와 submit 후 C로 relabel되지 않음을 검증한다.
9. Web, MAUI, Qt와 Windows App SDK의 diagnostics adapter/schema를 한 변경에서 맞추고 각 host project를 Release build한다. 공용 pointer/key/text/semantics event 형식은 건드리지 않는다.

Gate:

- 100,000 scene lifecycle 뒤 managed terminal history 저장량 bounded, active scene 0
- `registered = completed + active`, generated scene마다 terminal 정확히 1개, duplicate terminal 0
- replay N회 뒤 scene terminal counter 불변, causal attempt receipt만 N 증가
- synthetic Web `submitted > 0`일 때 `presented = 0`; native present-ACK fixture에서는 `presented > 0`
- input→framework frame→scene→causal attempt trace join 가능, trace ring capacity 불변
- existing legacy/exact/latest host fallback과 current+latest mailbox contract `PASS`

### W1. hard-stop direct canvas feasibility spike

제품 framework와 분리한 최소 fixture로 다음을 먼저 증명한다.

1. main document canvas에서 context 생성 전 `transferControlToOffscreen()`을 정확히 한 번 호출한다.
2. transferred canvas를 dedicated Worker로 넘기고 hardware WebGL2 identity를 기록한다.
3. Worker `requestAnimationFrame`에서 색/grid/edge marker를 main `bitmaprenderer` 없이 직접 표시한다.
4. foreground에서 main rAF와 Worker rAF timestamp를 함께 기록해 cadence와 throttling 차이를 paired comparison한다.
5. 연속 backing resize에서 direct FBO 0와 staging FBO + exact blit, 그리고 resize 전/후 resource recreation 순서를 비교한다.
6. synthetic context loss/restore 또는 가능한 동등 fixture로 resource recreation을 검증한다.
7. Worker fatal 후 canvas는 반드시 교체한다. 기존 root/IME/semantics endpoint에 listener를 rebind하는 방식과 host 전체 recreation 중 leak 없이 동작하는 최소 방식을 선택하고 새 Worker로 최대 1회 복구한다.
8. Emscripten GL context를 transferred canvas에 연결할 수 있는지 검증한다. JS WebGL만 성공하고 .NET/Skia binding이 불가능하면 `FAIL`이다.

Gate:

- instrumented code path/registry상 visible canvas `getContext` owner는 Worker 하나이며 main은 context를 요청하지 않음
- `createImageBitmap`, `bitmaprenderer`, bitmap receipt: 0
- foreground/visible 상태에서 main/Worker 각각 warm-up 60 뒤 600 callback 이상을 3회 수집하고, Worker callback count 차이 1% 이하 및 Worker interval p95가 main p95 + 1 measured refresh 이하
- resize 후 final backing/edge marker exact; 모든 captured sample/screenshot proxy에서 black/root band 0 px
- fatal 뒤 main registry의 old listener/Worker handle/callback/external lease 0, bounded restart `PASS`
- Emscripten WebGL2 context와 Skia-compatible framebuffer proof `PASS`

W1 자동화는 code path, internal registry, rAF trace와 captured screenshot proxy만 증명한다. compositor가 중간 partial frame을 실제 scan-out하지 않았는지, 죽은 Worker 내부 GPU/managed object가 GC됐는지는 `notVerified`다.

실패 시:

- 기존 renderer 삭제 금지
- 실패 capability와 browser 범위를 artifact로 남김
- compatibility backend 또는 main-thread direct path를 다음 설계안으로 재검토

### W2. protocol과 host module 추출

renderer 동작을 바꾸기 전에 [doroti.web.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)의 책임을 5.2 module 경계로 옮긴다.

1. protocol v2 union과 runtime decoder/validator를 먼저 만든다.
2. boot, ready, metrics, input, control response, semantics/text/cursor, diagnostics, fatal/dispose message를 versioning한다.
3. bitmap-era message는 기존 mode용 adapter 안에 격리하고 새 direct protocol에는 넣지 않는다.
4. DOM endpoint와 Worker supervision을 분리한다.
5. protocol permutation, duplicate/out-of-order, stale epoch, unknown version, double-terminal, dispose/fatal tests를 추가한다.
6. main supervisor에 `runtimeSessionId + causalFrameId` external lease mirror를 추가하고 graceful ACK/fatal/runtime-lost transition을 검증한다.
7. 기존 [worker-protocol.spec.ts](Doroti/validation/web-playwright/tests/worker-protocol.spec.ts)를 protocol v2 runner로 확장한다.
8. generated Worker bootstrap/import-map/fingerprinted `dotnet.js` 전달 계약을 그대로 보존한다.

Gate:

- 기존 세 mode의 Release/TypeScript/Playwright 결과 불변
- runtime validator가 malformed/illegal transition을 fail-closed
- listener, callback과 outstanding resource counter leak 0

### W3. `worker-direct-webgl` opt-in backend 결합

1. [doroti.loader.ts](Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts)에 test/opt-in `worker-direct-webgl`을 추가한다. `auto`는 아직 바꾸지 않는다.
2. 기존 persistent Worker의 .NET bootstrap을 새 Worker entry에 연결한다.
3. Worker TypeScript surface가 detached `new OffscreenCanvas`를 만들지 않고 transferred visible canvas를 받아 Emscripten WebGL2 context와 framebuffer/backing generation을 소유하게 한다.
4. [DorotiWebWorkerSurface.cs](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs)는 전달받은 framebuffer ID를 `GRBackendRenderTarget/SKSurface`로 감싸는 역할을 유지하고 causal/terminal/generation signature만 조정한다.
5. W1에서 선택한 direct FBO 0 또는 staging/exact blit primitive를 좁은 Worker TypeScript surface에 구현한다.
6. main managed runtime 0, Worker managed runtime 1을 startup diagnostics로 강제한다.
7. startup, first frame, font/text/image/effect shader, clipping과 opacity를 direct canvas에서 검증한다.

Gate:

- 첫 visible frame과 Demo correctness `PASS`
- main runtime 0, Worker runtime 1
- `ImageBitmap`/display receipt counter 0
- GPU/vendor/renderer/context generation 명시
- context loss/restore 때 interrupted scene `failed` 1회, 새 framework frame의 다른 `sceneSequence`가 exact `submitted` 1회; 성공했던 retained scene replay만 별도 허용

### W4. shared scheduler, ordered input와 terminal 통합

1. Worker rAF callback을 `BrowserHostAdapter.ScheduleFrame` transport로 연결하고 warm-up/legacy rAF 중복을 제거한다.
2. `BrowserFrameworkHost.CreateView`가 Web renderer를 `view.FrameTrace`에 연결한다. MAUI/Qt/Windows와 달리 Web만 이 연결이 빠진 현재 상태를 고친다.
3. main에서 pointer/wheel/key/focus/text editing·action·close/semantics action에 하나의 `inputSequence`를 부여하고 protocol과 [BrowserHostContracts.cs](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs) dispatch signature 전반에 전달한다. main `performance.now()`와 Worker `DorotiFrameClock`은 별도 clock-domain으로 기록한다.
4. `BrowserHostAdapter`가 sequence의 단조 증가, duplicate와 gap을 검증하고 managed event dispatch 전에 watermark와 `InputReceived`를 갱신한다. diagnostics reset이나 Worker restart가 lifetime sequence를 0으로 되돌리지 못하게 한다.
5. Worker surface가 submit attempt별 `causalFrameId`를 부여하고 `BrowserSkiaCapabilities.Paint`가 해당 `SkiaSceneRenderer.Paint(..., causalFrameId)` overload를 사용하게 한다. duplicate request ID와 generation mismatch는 fail-closed하고 framework identity는 `FrameworkFrameNumber`로 유지한다.
6. boolean completion을 runtime-validated `presented/submitted/superseded/dropped/failed + reason`으로 바꾼다. Web direct 성공은 `submitted`, stale replacement는 `superseded`, 정상 종료는 `dropped`, context/runtime/submit 오류는 `failed`다.
7. context loss 전에 `InvalidateGpuContextResources()`를 호출하고 중단 scene을 `failed`로 닫은 뒤 generation 갱신과 새 framework build를 요청한다. 같은 failed scene을 replay/재terminal하지 않는다. backing 교체에는 `InvalidateWindowSurfaceResources()`를 연결한다.
8. `ResizeTargetGeneration/MetricsGeneration/surfaceGeneration/contextGeneration` owner와 admission condition을 contract test로 고정한다.
9. scene은 current+latest 최대 2, main→Worker view-epoch transport는 one-in-flight+one-latest 최대 2, Worker framework-admission pending epoch는 latest 1의 bound를 각각 계측한다.
10. graceful dispose는 renderer가 outstanding managed scene을 `dropped`로 닫은 ACK 뒤 terminate한다. fatal에서는 죽은 managed ledger가 정산됐다고 주장하지 않고 main external lease mirror만 `failed(runtime-lost)`로 닫는다.

Gate:

- protocol permutation과 input ordering `PASS`
- 살아 있는/graceful session에서 generated scene 수와 managed terminal 수 일치
- duplicate/missing terminal 0
- queue depth: scene 2 이하, main transport epoch 2 이하, Worker admission epoch 1
- fatal session에서 main external lease missing/duplicate terminal 0
- same-generation click/wheel frame이 새 `sceneSequence`로 submit
- foreground resize 중 non-coalescible input은 ingress 뒤 2 measured refresh 안에 managed dispatch되고, 모든 sequence가 `dispatched/coalesced/dropped` 중 명시적 input disposition을 가짐
- Web direct에서 scene `submitted > 0`, scene `presented = 0`; replay attempt가 scene terminal count를 늘리지 않음
- context loss의 interrupted scene과 restore 뒤 rebuild scene이 서로 다른 `sceneSequence`이며 각각 `failed`/`submitted` 정확히 1회

### W5. resize continuity 전환

1. main CSS box 즉시 update와 Worker latest view-epoch transaction을 활성화한다.
2. refresh rate를 runtime에서 관측하고 고정 16 ms threshold를 제거한다.
3. timestamped edge marker로 CSS/backing mismatch 체류, black band, final exact와 stale scale proxy를 측정한다.
4. 181-step synthetic resize, maximize/restore와 DPR change contract를 direct mode에 추가한다.
5. current exact frame을 유지할 수 없는 error path는 partial display 대신 terminal `failed`와 bounded recovery로 닫는다.

Gate:

- target → exact submit p50 1 refresh 이내, p95 2 refresh 이내
- stale scale proxy 최대 1 measured refresh
- 모든 captured sample/screenshot에서 black/root band 0 px
- final logical/physical/backing/surface size exact
- newer `ResizeTargetGeneration`이 submit된 뒤 older generation submit 0, final target exact submit 뒤 older/intermediate submit 0

이 gate는 compositor scan-out proof가 아니다. 실제 border drag와 고주사율 monitor gate는 W7에서 별도로 닫는다.

### W6. DOM 기능 parity와 recovery

검증 대상:

- pointer capture, hover, drag, click, wheel와 trackpad-style burst
- key down/up/repeat, focus/blur, tab traversal
- native IME composition, selection, candidate window, long Backspace
- semantics delta/action/focus, screen reader-visible DOM
- cursor, clipboard, plugin request/response
- pagehide/dispose, context loss/restore, Worker fatal/restart

구현 원칙:

- IME composition과 native text owner는 main DOM endpoint에 유지한다.
- managed echo가 현재 native composition/selection을 덮지 못하게 한다.
- semantics bulk delta는 paint critical path 밖에서 적용하되 focus/action/text owner update를 임의 debounce하지 않는다.
- canvas 교체 시 중복 event listener, stale focus owner와 orphan semantics node가 없어야 한다.

Gate:

- synthetic DOM composition/selection, input와 semantics regression `PASS`
- native candidate window/selection, 물리 한글 IME, long key-repeat/Backspace와 screen reader는 실행 전 `notVerified`, 실행 후에만 `PASS/FAIL`
- fatal/restart 이후 input, text와 semantics가 새 endpoint에 한 번씩만 전달

### W7. 자동 성능과 실제 acceptance

동일 machine/Playwright bundled Chromium/display에서 current `document-webgl`, candidate `worker-direct-webgl`과 matching Flutter fixture를 번갈아 최소 3회 측정한다.

| 지표 | candidate gate |
|---|---|
| warm input ingress → exact submit p50 | 1 refresh 이내 |
| warm input ingress → exact submit p95 | 2 refresh 이내 |
| resize target → exact submit p50/p95 | 1 / 2 refresh 이내 |
| current `document-webgl` 대비 warm p95 | 20%와 5 ms 중 더 엄격한 bound 이내, 또는 별도 승인 |
| Flutter 대비 p95 | 같은 fixture에서 Flutter + 1 refresh보다 느리지 않음 |
| main long task | `PerformanceObserver`가 지원하는 warm controlled workload에서 50 ms 초과 0 |
| Worker stall proxy | Worker 내부 rAF callback/build/raster/submit duration 50 ms 초과 0 |
| scene/view-epoch queue | scene 2, main transport 2, Worker admission 1 이하 |
| terminal/resource | live managed ledger와 fatal external lease 각각 missing/duplicate 0; observable end-of-test active 0 |
| stale scale/black band | 1 refresh 이하 / 모든 captured sample에서 0 px |

각 performance run은 foreground/visible page에서 warm input sample 120개 이상과 181-step resize trace를 수집한다. `input starvation`은 non-coalescible ingress→managed dispatch가 2 measured refresh를 넘긴 sample이 하나라도 있으면 실패다. `단계적 추격 0`은 newer resize generation submit 뒤 older generation submit이 없다는 terminal trace로 판정한다. cadence mismatch는 W1의 3-run 조건이 연속 3회 실패할 때 구조 중단 조건으로 승격한다.

JIT, GC, font/image decode와 shader compile은 숨겨진 synthetic warm-up으로 제거하지 말고 first/warm sample과 관측 가능한 event를 별도 artifact로 남긴다. main Long Tasks API로 dedicated Worker를 측정했다고 주장하지 않고 Worker 내부 duration instrumentation을 별도로 사용한다. 전체 suite 안의 latency와 단독 표본이 다르면 둘 다 기록한다.

자동화 통과 뒤 headed Playwright bundled Chromium에서 다음을 확인한다.

- 좌/우/상/하/모서리 window border drag와 빠른 왕복
- maximize/restore, 큰 폭 resize와 monitor 간 DPR 이동
- 실제 wheel mouse와 precision trackpad fling
- resize/scroll 중 button, slider, text input
- 한글 IME 조합, candidate window, selection과 길게 누른 Backspace
- 60 Hz display와 120 Hz 이상 display
- keyboard navigation과 실제 screen reader

뒤늦은 단계적 추격, 1 refresh 초과 stretch, black band, input 무반응, IME owner 충돌이 하나라도 보이면 해당 gate는 `FAIL`이다. 자동 submit 수치로 물리 scan-out acceptance를 대신하지 않는다.

W7 qualification 필수 장비는 Windows에서 headed Playwright bundled Chromium을 실행하는 60 Hz display 1대, 120 Hz 이상 display 1대, precision trackpad, Korean IME와 실제 screen reader 1종이다. 하나라도 확보하지 못하면 자동 구현은 완료할 수 있어도 W7은 `partial/notVerified`이고 W8 default cutover와 legacy 삭제를 시작하지 않는다.

### W8. cutover, compatibility 결정과 legacy 정리

D0과 W1~W7이 모두 통과한 뒤 qualification과 default burn-in을 분리해 전환한다.

1. `auto=worker-direct-webgl`로 바꾸되 기존 mode를 명시적 rollback 경로로 유지한다.
2. 새 기본값으로 automated suite를 다시 실행하고, clean Playwright bundled Chromium launch 3회 이상·browser restart 1회 이상·누적 실제 사용 30분 이상에서 W7 resize/input/IME 시나리오를 반복한다.
3. burn-in 동안 fatal/restart, black band, missing terminal, duplicate input, IME owner 충돌 또는 W7 latency gate regression이 하나라도 있으면 즉시 `auto=document-webgl`로 rollback하고 legacy 삭제를 중단한다.
4. burn-in `PASS`와 support ADR 최종 승인 뒤 legacy 범위를 정리한다.

지원 결정별 결과:

- Playwright bundled Chromium/hardware-only fail-closed가 승인되면 기존 세 renderer와 public selection을 제거한다.
- broader browser 지원이 필요하면 같은 scheduler/protocol/terminal을 공유하는 최소 compatibility surface 하나만 남긴다. 세 개의 독립 frame coordinator/presenter 구조로 돌아가지 않는다.

삭제/갱신 audit 범위:

- renderer enum/policy/loader declaration과 URL query
- target manifest와 TypeScript declaration
- `CanvasPresenter/WorkerPresenter/WorkerDisplayPresenter`와 bitmap receipt protocol
- `DorotiWebRunner`, Razor root/surface, main runtime bootstrap의 실제 잔여 사용
- [Doroti.Runner.Sdk targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)의 generated Worker/bootstrap assets
- [run-web-renderer-ab.ps1](Doroti/eng/run-web-renderer-ab.ps1), [run-web-playwright.ps1](Doroti/eng/run-web-playwright.ps1) mode plumbing과 tests
- FCR-7 string/source contract, ADR-020, Demo [README](DorotiDemoApp/README.md)와 [한국어 README](DorotiDemoApp/README.ko.md)

Worker-only가 되어도 Blazor WebAssembly SDK/publish pipeline이 Worker의 `_framework/dotnet.js`를 생성하는 데 필요할 수 있다. main runtime 제거와 build SDK 제거를 같은 작업으로 처리하지 말고 publish output을 근거로 판정한다.

## 7. 검증 명령과 artifact

모든 test subprocess는 repository 규칙대로 20분 timeout을 실제로 enforce한다. W2에서 `Doroti/eng/run-web-direct-validation.ps1` owned-process orchestrator를 추가하고 기존 wrapper와 A/B script도 같은 timeout helper를 사용하게 한다. orchestrator는 다음을 실행한다.

- `dotnet build Doroti/src/Doroti.Skia.Rendering/Doroti.Skia.Rendering.csproj -c Release`
- `dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release`
- `dotnet build Doroti/src/Doroti.Host.Qt/Doroti.Host.Qt.csproj -c Release`
- `dotnet build Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj -c Release`
- `dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Release -p:DorotiHostTargetFrameworks=net10.0-windows10.0.19041.0`
- `dotnet build DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release`
- `npm run check` in `Doroti/validation/web-playwright`
- `dotnet run --project Doroti/validation/fcr3-scheduler/Doroti.Validation.Fcr3Scheduler.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr4-retained-rendering/Doroti.Validation.Fcr4RetainedRendering.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr5-scroll/Doroti.Validation.Fcr5Scroll.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr6-semantics/Doroti.Validation.Fcr6Semantics.csproj -c Release`
- `dotnet run --project Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -c Release`
- `dotnet run --project Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release`

구현 후 canonical 실행 순서:

1. `pwsh -NoProfile -File ./Doroti/eng/run-web-direct-validation.ps1 -Configuration Release`
2. `pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -HeadlessOnly -RendererMode worker-direct-webgl -TestFile 'tests/worker-protocol.spec.ts'`
3. `pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -HeadlessOnly -RendererMode worker-direct-webgl`
4. `run-web-playwright.ps1`에 mutually-exclusive `-HeadedOnly`를 추가한 뒤 headed Playwright bundled Chromium live resize/input/flicker/context-loss/crash-recovery만 별도 실행
5. `pwsh -NoProfile -File ./Doroti/eng/run-web-renderer-ab.ps1 -Configuration Release -Runs 3` — current/direct comparator로 갱신
6. `pwsh -NoProfile -File ./Doroti/eng/run-web-flutter-differential.ps1 -Configuration Release -Runs 3`

각 run은 다음을 artifact로 남긴다.

- commit, build configuration, browser/OS/GPU/driver/display/DPR
- renderer/runtime/context identity와 capability probe
- input sequence, resize-target/metrics generation, framework frame number, causal frame, scene/surface/context generation과 runtime session
- scene admission/terminal counter, causal attempt receipt, active terminal-ledger count와 bounded recent-history high-water
- ingress/frame/raster/submit timestamp raw samples
- p50/p95/max, refresh-normalized latency와 sample 수
- queue high-water mark, terminal/resource counters
- resize edge-marker screenshot/video와 mismatch duration
- first/warm 및 관측 가능한 JIT/GC/shader event 구분
- 자동 `PASS/FAIL`과 물리 `PASS/FAIL/notVerified`를 분리한 summary

## 8. 지원 범위와 제외

첫 qualification에 포함:

- Playwright bundled Chromium
- hardware WebGL2
- `OffscreenCanvas`, `transferControlToOffscreen`, dedicated Worker rAF
- full-page single Doroti view
- current Doroti DOM IME/semantics/clipboard/plugin contract

명시적 후속 결정 전 제외 또는 `notVerified`:

- Firefox, Safari와 Edge별 보장
- software/Canvas2D fallback
- embedded/multi-view와 DOM platform-view overlay
- WebGPU backend
- Web 이외 platform의 renderer/UX 기능 변경. D0 shared-core API compatibility build와 contract regression은 포함
- app state persistence/restart restoration
- browser compositor의 물리 scan-out ACK 추정

## 9. 완료, 중단과 rollback 조건

구조 완료:

- `doroti.web.ts`가 thin facade이고 DOM/protocol/Worker/surface lifecycle이 좁은 경계를 가짐
- semantic frame owner는 기존 shared scheduler 하나
- instrumented direct visible canvas/context code-path owner는 Worker 하나이며 main `getContext` 호출 0
- main .NET runtime 0, Worker .NET runtime 1
- direct path의 `createImageBitmap/bitmaprenderer/display receipt` 0
- input/resize-target/metrics/framework-frame/scene/causal-submit/surface/context/runtime-session identity owner가 각각 하나
- live/graceful session의 every admitted scene exactly one shared terminal
- fatal session에서 main이 관측한 every external lease exactly one `failed(runtime-lost)` terminal
- terminal ledger storage가 장기 실행에서 bounded이고 `registered = completed + active`
- scene terminal과 causal paint-attempt receipt가 분리되며 Web submit이 `presented`로 집계되지 않음

검증 완료:

- Release, TypeScript, FCR-3/4/5/6/7, protocol/resize contract와 browser regression `PASS`
- current/Flutter differential gate `PASS`
- W7 필수 60 Hz/120 Hz 이상 display, border drag, precision trackpad, Korean IME와 screen reader acceptance `PASS`
- 필수 matrix 밖의 browser/display/hardware는 `notVerified`로 명시

즉시 중단/rollback:

- transferred canvas와 Emscripten/Skia framebuffer 결합이 안정적으로 성립하지 않음
- W1의 foreground main/Worker rAF cadence gate가 연속 3회 실패
- captured proxy 또는 physical acceptance에서 backing resize transaction의 blank/partial frame을 bounded하게 막지 못함
- Worker fatal 후 canvas replacement, 선택한 endpoint rebind/recreation과 main external lease 정산을 안전하게 완료하지 못함
- direct mode가 current default 대비 correctness 또는 승인되지 않은 latency regression을 만듦

이 조건에서는 `auto`를 바꾸거나 legacy를 삭제하지 않는다. 기능 자동화가 통과해도 물리 resize/input/scan-out acceptance 전에는 “Flutter처럼 부드러운 Web renderer 완료”라고 판정하지 않는다.

## 10. 2026-08-30 실행 결과

### 구현 판정

| Workstream | 판정 | 결과 |
|---|---|---|
| W0 | `PASS` | 현재 세 renderer Release baseline을 재수집하고 Flutter `3.48.0-0.3.pre`, framework/engine `56b8e1a851a594b1a154f8ea93270807dab22b9a`, Dart `3.14.0-81.0.dev`로 matching fixture와 differential harness를 고정했다. |
| D0 | `PASS` | terminal ledger를 active + cumulative counter + bounded recent history로 바꾸고 100,000-frame out-of-order/duplicate contract를 통과했다. scene admission/terminal/causal receipt와 bounded frame-trace correlation을 분리했다. |
| W1 | `PASS` | transferred visible canvas, Worker WebGL2/framebuffer 0, Worker rAF 60+600 cadence, context recovery와 fatal 후 canvas/host replacement를 실제 browser에서 증명했다. |
| W2 | `PASS` | protocol v2 runtime decoder/state machine, DOM/surface/worker-host/diagnostics module, 1회 supervisor restart와 malformed envelope fail-closed 검증을 추가했다. |
| W3~W6 | `PASS` | `worker-direct-webgl` opt-in backend, ordered lifetime input sequence, typed terminal, Worker-owned surface/context generation, exact resize, DOM/IME/semantics 재바인딩을 구현했다. `auto`는 그대로 `document-webgl`이다. |
| W7 automated | `partial/FAIL` | direct 전체 suite와 Flutter differential은 통과했지만 current/direct 3회 A/B 중 direct run 2의 max 176.5 ms가 `<100 ms` gate를 실패했다. |
| W7 physical | `notVerified` | 실제 60 Hz/120 Hz 이상 display, 손으로 수행한 border drag, precision trackpad, 한글 IME 후보창/긴 Backspace, screen reader와 물리 scan-out은 이번 자동 실행으로 판정하지 않았다. |
| W8 | `notStarted-by-gate` | W7 전체 PASS가 아니므로 default cutover, burn-in, compatibility 축소와 legacy 삭제를 시작하지 않았다. |

### 재현 가능한 evidence

- `run-web-direct-validation.ps1 -Configuration Release`: Skia/Web/Qt/WindowsAppSdk/MAUI/Demo Release build, TypeScript, FCR-3/4/5/6/7, resize contract 모두 `PASS`.
- 최종 tree direct headless 전체: 11 `PASS`, Flutter 조건부 1 `SKIP`; 2분 continuous flicker/resize, context loss, DPR2, input, capability, startup, protocol/crash recovery 포함. wheel p95 31.8 ms, max 62.4 ms.
- direct headed 자동 resize: Desktop Chrome bounds와 Windows native edge resize 2 `PASS`. 실제 사람 입력 acceptance의 대체 증거는 아니다.
- 기존 renderer 회귀: document 8 `PASS`/4 해당없음 `SKIP`, offscreen-bitmap 8 `PASS`/4 `SKIP`, 최종 tree offscreen-worker 10 `PASS`/2 `SKIP`.
- current/direct A/B 3회: document p95 26.6/26.3/26.9 ms, median 26.6 ms; direct p95 29.0/28.1/25.7 ms, median 28.1 ms. direct max 47.2/176.5/49.6 ms로 absolute max gate `FAIL`.
- Flutter differential 3회, warm sample 각 117개: Doroti direct p50 22.8 ms/p95 53.0 ms, Flutter p50 34.9 ms/p95 42.5 ms. `Flutter p95 + 20 ms` proxy gate는 `PASS`; compositor scan-out ACK는 측정하지 않는다.

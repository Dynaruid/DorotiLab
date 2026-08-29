# Doroti Web Flutter-style OffscreenCanvas + Web Worker 렌더링 작업 계획

- 작성일: 2026-08-29
- 상태: **검토 및 계획만 완료**. 제품 구현, Release build, 브라우저 실행은 아직 `notVerified`다.
- 대상: `Doroti.Host.Web` / `browser-wasm` / SkiaSharp WebGL2
- 목표: main browser thread에서 Doroti framework/Skia raster를 제거한다. 하나의 persistent Web Worker가 유일한 managed Doroti runtime과 실제 `OffscreenCanvas` WebGL2/Skia surface를 소유하고, 완성 frame만 `ImageBitmap`으로 main thread의 visible canvas에 전달한다.
- 필수 범위: worker backend 구현은 성능 개선 여부와 관계없이 이번 작업에 포함한다. 같은-thread OffscreenCanvas는 capability fallback과 A/B/rollback 경로로 함께 구성한다.

## 1. 결론

현재 Doroti Web은 이미 visible canvas의 WebGL2 context 안에서 `front`/`staging` FBO를 사용한다. 따라서 scene은 staging FBO에 보이지 않게 그려지고 완성 뒤 default framebuffer로 blit된다. 그러나 WebGL context와 default framebuffer의 소유자는 여전히 DOM의 visible `<canvas>`이며, 이것은 Flutter Web의 현재 Chrome 계열 구조와 같지 않다.

목표 구조는 다음과 같다.

```text
main browser thread                     persistent Doroti Web Worker
  DOM input/metrics/lifecycle  ------->   managed Doroti runtime
  browser rAF timestamp        ------->   framework build/layout/scene
                                           current + latest mailbox
                                           SkiaSharp + OffscreenCanvas WebGL2
                                           createImageBitmap(offscreenCanvas)
  exact ImageBitmap + receipt  <-------   stale request/epoch 재검사
  bitmaprenderer commit
  DOM text/semantics/plugin bridge
```

핵심 결정:

- visible canvas는 입력/focus/cursor 위치와 화면 표시만 소유하고 WebGL context를 소유하지 않는다.
- raster canvas는 DOM에 붙지 않은 실제 `OffscreenCanvas`이며 WebGL2/Skia context를 하나만 소유한다.
- Flutter source와 같이 `transferToImageBitmap()`보다 `createImageBitmap(offscreenCanvas)`를 우선 사용한다. `transferToImageBitmap()`은 source bitmap을 새 blank image로 교체하므로 현재 Skia surface 수명과의 결합을 먼저 만들지 않는다.
- visible canvas는 `bitmaprenderer`를 사용하고 새 exact bitmap이 준비된 같은 commit 구간에 intrinsic/CSS size를 맞춘 뒤 bitmap ownership을 넘긴다.
- `ResizeObserver`는 metrics만 publish한다. resize callback에서 visible backing reset, WebGL clear, retained-front blit을 하지 않는다.
- async `createImageBitmap` 중 새 frame/resize가 오면 완성된 stale bitmap을 visible canvas에 보내지 않고 `close()` 후 `superseded`로 끝낸다.
- Chrome/Edge에서는 offscreen path를 승격 후보로 삼고, Firefox/Safari는 초기에는 현재 document-WebGL path를 명시적 fallback으로 유지한다. Flutter도 이 두 브라우저에서 `createImageBitmap` 성능 때문에 multi-surface path를 선택한다.
- software/Canvas2D fallback, CanvasKit 도입, WebGPU 전환은 하지 않는다. hardware WebGL2 fail-closed 계약을 유지한다.
- worker는 `offscreen-worker`라는 first-class backend로 다룬다. worker mode에서는 main Blazor runtime을 함께 시작하지 않고 worker의 .NET runtime 하나만 app/framework/renderer를 소유한다.
- canvas만 worker에 넘기고 managed build/raster를 main thread에 남기는 이름뿐인 worker 구성이나 main/worker에 managed runtime을 중복 기동하는 구조는 채택하지 않는다.
- worker mode는 별도 .NET browser-wasm runtime을 worker의 단일 thread에서 실행한다. 이번 MVP에서 `WasmEnableThreads`/SharedArrayBuffer 기반 pthread를 추가로 겹치지 않는다.

## 2. 확인한 현재 구현

### 2.1 Doroti current path

주요 source:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- `Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor`
- `Doroti/src/Doroti.Host.Web/DorotiSurface.razor`
- `Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs`
- `Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs`

현재 frame 흐름:

1. `requestFrame`은 host별 단일 rAF에서 latest framework callback만 전달한다.
2. managed renderer가 scene을 만들면 `requestPresent`가 `current + latest` mailbox에 present descriptor를 넣는다.
3. visible `<canvas>`에서 만든 Emscripten WebGL2 context에 exact-size staging texture/FBO와 depth-stencil renderbuffer를 준비한다.
4. `DorotiWebGlSurface.RenderFrame`이 staging FBO를 `GRBackendRenderTarget`/`SKSurface`로 감싸 raster하고 flush한다.
5. generation이 여전히 exact이면 staging FBO를 visible canvas default framebuffer에 blit하고 staging/front를 교환한다.
6. `CompleteFrame`과 presenter terminal이 각각 한 번 끝난다. GPU blit은 `submitted`이며 browser compositor scan-out 증거는 아니다.

보존해야 할 계약:

- framework rAF owner는 하나다.
- present queue depth는 시작된 `current` 하나와 교체 가능한 `latest` 하나, 최대 2다.
- 시작된 frame의 identity/terminal은 immutable하며 정확히 한 번 끝난다.
- stale resize generation은 visible commit 전에 거절한다.
- WebGL context loss/restore 후 GPU resource와 exact front를 재생성한다.
- logical size, physical size, DPR, resize generation을 한 epoch로 취급한다.
- pointer/wheel/key/text/IME/semantics는 DOM main thread에서 기존 경로를 유지한다.

현재 구조의 한계:

- `renderViaOffscreenBackBuffer: 0`이며 실제 `OffscreenCanvas`는 생성하지 않는다.
- visible canvas가 WebGL context와 default framebuffer를 소유하므로 resize/backing reset과 display presentation이 raster state에 결합된다.
- `front`/`staging` color texture 두 장과 각각의 depth-stencil, visible default buffer를 함께 유지한다.
- managed pending paint가 resize `generation`을 key로 사용한다. 동일 resize epoch 안의 연속 animation frame을 async bitmap 단계와 안전하게 구분하려면 별도 present request identity가 필요하다.
- 현재 presenter drain은 동기다. `createImageBitmap`을 넣으면 async 경계가 생기므로 terminal/resource ownership을 다시 명시해야 한다.

### 2.2 Flutter reference

검토 기준은 repository-local Flutter revision `56b8e1a851a594b1a154f8ea93270807dab22b9a`의 다음 source다.

- `engine/src/flutter/lib/web_ui/lib/src/engine/compositing/offscreen_canvas_rasterizer.dart`
- `engine/src/flutter/lib/web_ui/lib/src/engine/compositing/render_canvas.dart`
- `engine/src/flutter/lib/web_ui/lib/src/engine/compositing/rasterizer.dart`
- `engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/surface.dart`
- `engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/renderer.dart`

확인된 Flutter 구조:

- 하나의 DOM-detached `OffscreenCanvas`와 하나의 GL/Skia surface를 여러 visible `RenderCanvas`가 공유한다.
- offscreen surface를 raster한 뒤 `createImageBitmap` 결과를 visible canvas의 `bitmaprenderer.transferFromImageBitmap()`으로 전달한다.
- visible canvas의 intrinsic size는 bitmap을 표시하기 직전에 exact frame size로 맞춘다.
- render queue는 `current + next`만 보유하고 새 요청은 아직 시작하지 않은 next를 latest로 교체한다.
- CanvasKit은 Chrome 계열에서 offscreen rasterizer를 기본 사용하지만 Firefox/Safari에서는 `createImageBitmap` 성능 때문에 onscreen multi-surface rasterizer를 사용한다.
- 이 경로는 `OffscreenCanvas`를 사용하지만 그 자체로 raster를 worker에서 실행한다는 뜻은 아니다.

## 3. 목표 소유권과 상태 계약

### 3.1 Surface ownership

| 자원 | 소유자 | 규칙 |
| --- | --- | --- |
| `.doroti-root` | DOM host | viewport, input 좌표, semantics 기준 |
| main JavaScript realm | DOM host | input/IME/semantics/plugin/rAF/display만 소유하고 managed app을 실행하지 않음 |
| persistent Web Worker | Doroti worker runtime | app/framework/scene/Skia/raster와 GPU cache를 단독 소유 |
| visible `<canvas>` | display presenter | focus/cursor와 마지막 committed bitmap만 유지 |
| `ImageBitmapRenderingContext` | display presenter | 한 번 생성하고 다른 context mode와 혼용하지 않음 |
| `OffscreenCanvas` | raster presenter | DOM에 attach하지 않으며 exact physical raster size 소유 |
| WebGL2/Emscripten context | raster presenter | OffscreenCanvas당 하나, hardware fail-closed |
| `GRContext`/`SKSurface` | `DorotiWebGlSurface` | offscreen WebGL default framebuffer를 감싸고 context generation과 함께 폐기 |
| `ImageBitmap` | 현재 present request | 생성 후 즉시 visible context가 consume하거나 stale/error 시 `close()` |

### 3.2 Frame identity

`resize generation`과 `present request id`를 분리한다.

- resize generation: logical/physical/DPR target의 identity
- request id: 같은 resize generation 안에서도 증가하는 host present transaction identity
- scene sequence: shared `SkiaSceneRenderer`가 소유하는 framework scene identity
- causal frame id: request id를 `SkiaSceneRenderer.Paint(..., causalFrameId)`에 전달

`BrowserSkiaCapabilities._pendingPaints`는 resize generation이 아니라 request id로 completion을 추적한다. JS의 `RenderFrame`/`CompleteFrame` callback에도 request id와 resize generation을 함께 전달한다. async bitmap capture 중 resize가 바뀌거나 latest request가 교체되어도 다른 frame completion을 잘못 끝내면 안 된다.

### 3.3 Terminal 정의

각 request는 다음 중 정확히 하나로 끝난다.

- `submitted`: exact bitmap이 `transferFromImageBitmap`에 전달됨
- `superseded`: raster 전 교체, raster/capture 중 target 변경, stale bitmap 폐기
- `failed`: GL/Skia/bitmap/display API 오류

`submitted`는 browser display API에 ownership을 넘긴 시점이며 실제 scan-out/presented acknowledgement가 아니다. 기존 `browser-present-unverified` 경계를 유지한다.

## 4. 실행 순서

### S0. Baseline과 capability spike 고정

목적: product presenter를 바꾸기 전에 현재 .NET 10 + SkiaSharp WebAssembly 조합이 실제 OffscreenCanvas GL surface를 감쌀 수 있는지 증명한다.

작업:

- 시작 시 `git status --short`, 영역별 diff, `git diff --check`를 기록하고 기존 사용자 변경을 보존한다.
- 현재 direct path에서 동일 machine/Release의 startup, wheel latency, A→B→C resize, idle/wheel/resize blank oracle, context restore 결과를 다시 수집한다. `history/26-08-29`의 p95 21.1 ms는 과거 증거이지 새 baseline으로 재사용하지 않는다.
- disposable canvas만 사용하는 capability probe를 추가한다.
  - `OffscreenCanvas`, `createImageBitmap`, visible `bitmaprenderer` 존재
  - Emscripten `GL.createContext` 또는 최소 product-owned registration adapter가 `OffscreenCanvas.getContext("webgl2")` 결과를 current context로 등록 가능
  - SkiaSharp `GRContext`와 framebuffer 0 `GRBackendRenderTarget` 생성 가능
  - 고유 색/패턴 raster와 flush 뒤 `createImageBitmap`이 non-empty exact-size 결과 생성
  - disposable visible canvas로 bitmap transfer 성공
  - GPU vendor/renderer가 hardware WebGL2이며 software fallback이 아님
  - resource dispose와 context loss가 예측 가능하게 끝남
- real display canvas의 context mode는 한 번 정하면 바꿀 수 없으므로 probe는 반드시 별도 disposable canvas에서 끝낸다.

Hard gate:

- 위 end-to-end probe가 `PASS`하면 S1로 간다.
- raw OffscreenCanvas WebGL2는 되지만 현재 Emscripten registration만 실패하면 작은 product-owned context registration adapter를 spike한다.
- SkiaSharp가 offscreen GL context/default framebuffer를 안정적으로 감싸지 못하면 제품 경로를 우회 구현하지 않고 `blocked`로 기록한다. CPU readback, PNG, Canvas2D 복사는 대안으로 채택하지 않는다.

### S1. Presenter mode와 request identity 분리

목적: current path를 보존한 상태에서 offscreen path를 안전하게 병행할 공통 계약을 만든다.

작업:

- presenter mode를 `auto | offscreen-bitmap | document-webgl`로 정의한다.
  - test/query override는 명시적 진단 모드에서만 허용한다.
  - `auto`는 browser policy와 S0 capability 결과로 결정한다.
  - 선택된 mode와 fallback reason을 diagnostics에 노출한다.
- `CanvasPresenter`의 공통 mailbox/terminal과 raster/display backend state를 분리한다.
- request id를 `RenderFrame`, `CompleteFrame`, `BrowserSkiaCapabilities.Paint/CompletePaint`까지 전달한다.
- `SkiaSceneRenderer.Paint`의 기존 `causalFrameId` overload를 사용하고 pending completion을 request id로 key한다.
- current document-WebGL path가 이 refactor만으로 이전 Playwright 결과와 terminal counter를 그대로 통과하도록 먼저 검증한다.

Gate:

- direct path의 화면, queue depth, exact terminal, context restore, input/semantics 결과에 회귀가 없어야 한다.
- 이 단계에서는 renderer default를 바꾸지 않는다.

### S2. Offscreen raster surface 구현

목적: 실제 OffscreenCanvas 하나가 WebGL2/Skia raster를 독점하게 한다.

`doroti.web.ts`:

- `new OffscreenCanvas(1, 1)`을 만들고 Emscripten WebGL2 context를 등록한다.
- WebGL capability/extension/GPU identity는 visible canvas가 아니라 offscreen context에서 읽는다.
- offscreen canvas physical size가 target과 달라질 때만 exact size로 바꾸고 `surfaceGeneration`/GL dirty state를 증가시킨다.
- current staging/front FBO가 아니라 offscreen default framebuffer 0을 managed Skia에 전달한다.
- raster/flush가 끝나면 `await createImageBitmap(offscreenCanvas)`을 수행한다.
- await 뒤 host 존재, context generation, request id, resize generation, physical size를 다시 확인한다.
- stale bitmap은 `close()`하고 `superseded` terminal을 기록한다.
- exact bitmap만 visible `bitmaprenderer.transferFromImageBitmap()`으로 consume한다.
- async drain은 current 하나만 실행하고 latest 하나만 보유한다. latest replacement, dispose, context loss의 모든 경로에서 terminal과 bitmap 수명을 닫는다.

`DorotiWebGlSurface.razor` 및 managed bridge:

- DOM canvas 자체를 Skia surface identity로 가정하지 않고 presenter가 준 offscreen context/framebuffer/size/context generation을 감싼다.
- request id를 `PaintSurface`와 `CompleteSurfacePaint`까지 보낸다.
- size, framebuffer, context generation이 바뀔 때 `SKSurface`/`GRBackendRenderTarget`을 정확히 재생성한다.
- dispose/context loss 시 `_context.AbandonContext()`와 shared renderer GPU cache invalidation 순서를 명시한다.

제거/유지 경계:

- offscreen mode에서는 `GpuSurface front/staging`, `blitRectToDefault`, `clearDefaultFramebuffer`, `preserveDrawingBuffer` 의존을 사용하지 않는다.
- direct fallback이 남아 있는 동안 관련 코드는 backend 내부에 격리한다.
- offscreen mode가 default로 승격되고 Firefox/Safari 전략이 확정되기 전에는 direct backend를 삭제하지 않는다.

### S3. Exact display commit과 resize/DPR 처리

목적: visible canvas는 완성된 target만 표시하고 resize observer는 raster/display state를 직접 만지지 않게 한다.

작업:

- `ResizeObserver`/DPR watcher는 immutable `DorotiResizeEpoch`를 publish하고 framework invalidate만 요청한다.
- observer callback에서 canvas intrinsic resize, GL clear, provisional front copy를 하지 않는다.
- old committed bitmap은 next exact bitmap이 준비될 때까지 visible canvas에 남긴다.
- exact commit 직전에 visible canvas의 `width`/`height`와 logical CSS size를 target에 맞추고 바로 bitmap을 transfer한다.
- grow 중 새 영역은 host background가 보일 수 있지만 old frame 전체를 새 size로 stretch하지 않는다. shrink 중에는 root overflow clip을 사용한다.
- DPR/zoom 전환은 logical size, DPR, physical size가 coherent한 하나의 epoch일 때만 commit한다. 현재 stale `devicePixelContentBoxSize` fallback 규칙을 유지한다.
- visible size update와 bitmap transfer 사이에는 `await`, managed callback, 다른 task를 넣지 않는다.

Gate:

- intermediate A/B target은 `superseded` 가능하지만 final C는 exact `submitted`여야 한다.
- visible canvas intrinsic size, bitmap size, offscreen raster size, resize epoch physical size가 final commit에서 모두 같아야 한다.
- stale/size-mismatch commit과 old bitmap stretch sample이 0이어야 한다.

### S4. Context loss, lifecycle, resource bound

목적: 비동기 bitmap과 DOM-detached GL 자원이 leak 없이 복구되게 한다.

작업:

- OffscreenCanvas의 `webglcontextlost`/`webglcontextrestored`를 raster presenter가 소유한다.
- loss 시 current request를 `superseded`, 아직 시작하지 않은 latest를 보존 또는 명시적으로 교체하고, managed `GRContext`와 renderer GPU cache를 invalidate한다.
- visible canvas의 마지막 committed bitmap은 loss 동안 그대로 유지한다.
- restore는 새 offscreen canvas/context를 생성하고 context generation을 증가시킨 뒤 latest exact scene을 replay한다. old GL handles를 재사용하지 않는다.
- dispose 시 current/latest terminal, unconsumed bitmap, Skia surface/context, Emscripten context, event listener를 모두 닫는다.
- diagnostics에 `created/consumed/closed/active ImageBitmap`, context generation, raster/display size, mode, queue depth를 추가한다.

Gate:

- active unconsumed bitmap은 정상 상태 0, async capture 중 최대 1이다.
- queue depth는 최대 2다.
- context loss/restore 후 generation이 증가하고 latest exact frame이 복구되며 failed/unpaired terminal이 없다.
- 실제 GPU memory 감소는 구조만으로 단정하지 않고 측정하지 않으면 `notVerified`로 둔다.

### S5. Playwright 자동 회귀와 A/B 판정

기존 `Doroti/validation/web-playwright`를 확장한다.

추가/수정 항목:

- `offscreen-capability.spec.ts`: S0 probe와 hardware WebGL2 identity
- startup: `offscreen-bitmap`과 `offscreen-worker`에서 raster canvas DOM 미부착, visible `bitmaprenderer`, first exact nonblank commit
- wheel: ingress 즉시 전달, current+latest bound, request-id terminal, wheel→bitmap commit latency
- resize: viewport A→B→C, headed window bounds, DPR 2, zoom/coherency, old-bitmap stretch oracle
- flicker: idle 60초, wheel 30초, resize 30초 blank/clear-only 0
- context restore: offscreen GL context generation 증가와 exact replay
- resource: bitmap created 수가 consumed+closed와 일치하고 active upper bound를 넘지 않음
- input regression: pointer, keyboard, hidden native text input, semantics activation이 visible canvas context 변경과 무관하게 유지됨
- worker ownership: main managed runtime 0, worker managed runtime 1, worker/port/runtime leak 0
- worker protocol: input/metrics/frame/control envelope version, request/response correlation, crash 1회 bounded recovery
- `document-webgl`/`offscreen-bitmap`/`offscreen-worker` 세 mode를 같은 build와 browser 조건에서 직렬 실행하고 결과 artifact를 분리한다.

자동 PASS 조건:

| 항목 | 조건 |
| --- | --- |
| build/typecheck | Release build 경고 0/오류 0, TypeScript check PASS |
| capability | actual OffscreenCanvas WebGL2 + Skia + ImageBitmap + bitmaprenderer end-to-end PASS |
| queue/terminal | max depth 2, request별 terminal 정확히 1, failed/stale-visible/unpaired 0 |
| pixels | first/idle/wheel/resize blank 또는 clear-only candidate 0 |
| resize/DPR | final exact size/epoch 일치, old bitmap stretch 및 size mismatch 0 |
| lifecycle | context loss/restore, hidden/visible, dispose resource bound PASS |
| 기능 회귀 | FCR-7, resize contract, pointer/key/text/semantics Playwright PASS |
| latency | 60 Hz 기준 wheel→exact bitmap commit p95 33.4 ms 이내, 100 ms 이상 stall 0 |

Default 승격용 비교 gate:

- 같은 machine/Release/browser에서 direct와 offscreen을 각각 3회 실행하고 run별 p95의 median을 비교한다.
- offscreen이 absolute latency gate를 통과해도 direct 대비 median p95가 20% 초과 또는 5 ms 초과로 나빠지면 `auto` default로 승격하지 않고 opt-in으로 유지한 채 `createImageBitmap`/Skia flush 구간을 분리 측정한다.
- 구조상 buffer 수 감소나 main-thread 응답 개선은 `expectedImprovement`로만 기록한다. CPU, allocation, GPU memory, scan-out 개선은 측정 전 PASS로 쓰지 않는다.

### S6. Same-thread OffscreenCanvas fallback 완성

목적: worker와 같은 raster/display 계약을 사용하면서 worker/runtime bootstrap만 제외한 rollback backend를 완성한다. 이것은 최종 목표를 대신하지 않는다.

Policy:

- `document-webgl`: 현재 path를 보존한 최저 rollback 기준선
- `offscreen-bitmap`: main managed runtime + actual OffscreenCanvas/bitmaprenderer fallback 및 worker A/B 기준선
- `offscreen-worker`: 이번 작업의 primary backend
- mode는 managed runtime을 시작하기 전에 결정한다. 한 page에서 main과 worker managed runtime을 동시에 시작하지 않는다.
- runtime 도중 backend를 바꿔야 하면 context mode가 이미 고정된 canvas를 재사용하지 않고 host/canvas/runtime 재시작 경계를 사용한다.

Gate:

- S0~S5의 exact bitmap, request identity, resize/DPR, context/resource, Playwright 계약을 `offscreen-bitmap`에서 먼저 닫는다.
- worker 구현 중 문제가 생겨도 direct/same-thread path를 기능 우회 코드와 섞지 않고 독립 backend로 유지한다.
- `offscreen-worker`가 구현되지 않은 상태를 이번 작업의 완료로 판정하지 않는다.

### S7. Persistent Doroti worker runtime bootstrap

목적: main thread의 Blazor component 안에서 app을 실행하는 현재 composition root를 worker-owned Doroti runtime으로 분리한다.

선택한 architecture:

- worker mode는 module Web Worker 하나를 process 수명 동안 유지한다.
- worker 내부에서 별도 .NET 10 `browser-wasm` runtime을 한 번 시작하고 app entrypoint, `DorotiHostSession`, `DorotiView`, framework, `SkiaSceneRenderer`, font/image/GPU cache를 모두 소유한다.
- main thread는 worker mode에서 `Blazor.start()`를 호출하지 않는다. DOM host를 만들기 위해 main managed runtime을 중복 기동하지 않는다.
- main은 TypeScript DOM host, input/IME/semantics, browser rAF, clipboard/plugin adapter, visible bitmap commit만 소유한다.
- worker runtime은 single-thread다. `WasmEnableThreads`, pthread, SharedArrayBuffer, COOP/COEP를 이번 구조에 추가하지 않는다.
- framework 전체가 worker에 있으므로 per-frame scene serialization을 하지 않는다. main/worker 경계를 지나는 것은 input/metrics/control과 완성 `ImageBitmap`뿐이다.

Bootstrap 작업:

- `doroti.loader.ts`가 `auto | offscreen-worker | offscreen-bitmap | document-webgl`을 managed runtime 시작 전에 선택하게 한다.
- `doroti.raster.worker.ts` module entry와 worker 전용 .NET startup module을 추가한다.
- worker용 composition root가 generated application descriptor/manifest, app assembly, plugin descriptor를 받아 기존 `DorotiWebRunner`와 같은 fail-closed 검증으로 app을 시작하게 한다.
- worker mode의 root/canvas/textarea/semantics DOM은 main TypeScript가 생성한다. 현재 `DorotiSurface.razor`/`DorotiWebGlSurface.razor`는 same-thread fallback에서만 사용한다.
- worker/runtime/assembly/Skia native/font/image asset을 publish manifest에 포함하고 fingerprinted URL과 MIME/CORS를 검증한다.
- `Doroti.Target.Web.browser-wasm`과 template이 worker asset/entrypoint를 생성하도록 build/package contract를 확장한다.

S7 capability probe:

- module worker가 Release publish의 .NET runtime과 app assembly를 load하고 managed export를 호출한다.
- worker 안에서 actual OffscreenCanvas hardware WebGL2/Emscripten context와 SkiaSharp `GRContext`를 생성한다.
- 실제 DorotiDemoApp root scene을 build/layout/raster하고 nonblank exact `ImageBitmap`을 main으로 transfer한다.
- main에는 managed runtime이 0개, worker에는 managed runtime이 정확히 1개인지 diagnostics로 확인한다.
- worker creation부터 first exact bitmap까지 startup phase와 bytes를 기록한다.

Hard gate:

- checkerboard JS/WebGL만 성공하고 managed Doroti scene이 worker에서 raster되지 않으면 실패다.
- main Blazor runtime과 worker runtime이 동시에 app/framework를 실행하면 실패다.
- per-frame JSON scene, CPU pixel readback, PNG/Blob encode가 필요하면 실패다.
- worker bootstrap이 현재 .NET 10/SkiaSharp product graph에서 불가능하면 같은-thread 완료로 대체하지 않고 blocker와 최소 reproduction을 기록한다.

### S8. Worker/main protocol과 product frame pipeline

Protocol:

```text
main DOM host                         persistent Doroti worker
  input/metrics/lifecycle      --->    framework host adapter
  rAF callback id + timestamp  --->    build/layout/scene
                                         current + latest raster mailbox
                                         Skia + OffscreenCanvas raster
  ImageBitmap + frame receipt  <---    exact/stale decision
  bitmaprenderer commit
  cursor/text/semantics/plugin <---->   versioned control messages
```

Main-thread 작업:

- pointer/coalesced pointer/wheel/key/focus/native text/IME event를 현재 의미와 timestamp를 보존해 즉시 worker에 전달한다.
- root `ResizeObserver`/DPR watcher가 immutable epoch를 만들고 worker에 latest metrics를 전달한다.
- browser rAF 하나가 worker의 framework frame callback cadence를 소유한다.
- worker가 보낸 request id/epoch/context generation/bitmap을 검증하고 exact bitmap만 visible `bitmaprenderer`에 consume한다.
- stale/unknown bitmap은 즉시 `close()`하고 worker에 terminal receipt를 돌려준다.
- clipboard와 JavaScript plugin은 correlation id가 있는 request/response로 main에서 실행한다.
- worker가 보낸 text input state/caret와 semantics update를 기존 invisible DOM owner에 적용한다.

Worker 작업:

- main message를 `BrowserWorkerHostAdapter`가 기존 `IViewHostCapability`/frame/input/text/platform/semantics 계약으로 변환한다.
- input sample 자체는 합치지 않고 즉시 framework에 전달하며 frame request만 latest rAF로 coalesce한다.
- raster mailbox는 시작된 `current` 하나와 교체 가능한 `latest` 하나, 최대 2다.
- request id, resize generation, scene sequence, causal frame id, context generation을 terminal까지 보존한다.
- `createImageBitmap` 뒤 latest epoch를 다시 검사하고 exact bitmap만 transferable list로 main에 보낸다.
- main의 commit receipt가 오기 전 completion을 `presented`로 부르지 않는다. receipt는 여전히 browser scan-out ACK가 아니라 `submitted`다.
- worker crash/context loss 시 in-flight terminal을 닫고 persistent worker/context를 최대 한 번 재생성한 뒤 latest exact scene을 replay한다. 무한 restart loop는 금지한다.

Message ABI:

- protocol version과 host/view id를 모든 envelope에 넣는다.
- frame/input/control mailbox를 분리해 clipboard/plugin 응답이 frame을 막지 않게 한다.
- frame당 scene JSON은 없으며 `ImageBitmap`은 transferable ownership으로 한 번만 이동한다.
- trace timestamp는 main `performance.timeOrigin` 기준으로 정규화해 main/worker 구간을 한 timeline에서 비교한다.
- dispose/navigation/pagehide 시 worker, message port, bitmap, managed session, GL/Skia resource를 모두 닫는다.

### S9. Worker validation, tuning과 default policy

Worker 구현 완료 gate는 성능 개선을 조건으로 하지 않는다. 다음 correctness/product gate가 모두 PASS하면 `offscreen-worker` backend 구현은 완료다.

- main managed runtime 0, persistent worker managed runtime 1
- actual worker OffscreenCanvas hardware WebGL2 + managed Skia raster
- first content, continuous animation, wheel, resize/DPR/zoom, context restore
- request별 exactly-once terminal, queue depth 최대 2, stale visible commit 0
- ImageBitmap created = consumed + closed, active upper bound 준수
- pointer/key/focus/native text/한글 IME/semantics/plugin/clipboard 회귀 0
- worker crash 1회 복구와 bounded failure terminal
- Release/dev/static hosting에서 worker/runtime/assembly/native/font/image/plugin asset load PASS

성능 계측:

- main-thread Event Timing/input dispatch p50/p95/max
- main-thread Long Task count, total duration, max duration
- browser rAF deadline miss와 request→bitmap commit p50/p95/max
- build/layout, raster/flush, bitmap create, worker handoff, display commit 구간별 시간
- process cold TTID/first exact frame
- 전체 downloaded bytes, worker Wasm/JS heap과 가능한 범위의 GPU memory
- submitted/superseded/failed, queue depth, active bitmap/worker/runtime 수

A/B와 tuning 기준:

- 같은 machine/Release/Desktop Chrome에서 `document-webgl`, `offscreen-bitmap`, `offscreen-worker`를 각각 3회 실행한다.
- worker 결과가 느려도 backend 구현을 제거하지 않고 message/startup/raster 구간을 profile해 tuning한다.
- wheel→commit p95 33.4 ms 이내와 100 ms stall 0은 모든 default 후보의 absolute gate다.
- process cold TTID 10% 초과 또는 memory peak 15% 초과 회귀는 원인을 기록하고 `auto` 승격 전 조정한다.
- main-thread input p95/Long Task/frame p95 개선은 측정값으로 보고하며 구조만으로 개선을 주장하지 않는다.

최종 browser policy:

- Chrome/Edge: S9 correctness와 absolute latency gate가 통과하면 `auto -> offscreen-worker`
- worker correctness는 PASS했지만 default performance gate가 실패하면 backend는 명시적 `offscreen-worker`로 유지하고 `auto -> offscreen-bitmap`을 임시 유지한다. 이것은 worker 구현 미완료를 뜻하지 않는다.
- Firefox/Safari: 실제 worker/offscreen A/B 전에는 `auto -> document-webgl`; 강제 worker 결과는 `notVerified`
- capability 미지원/worker bootstrap 실패: 명시적 fallback reason과 backend identity를 기록하고 page 시작 전에 fallback runtime을 선택
- software renderer로 조용히 fallback하지 않는다.

완료 문서:

- diagnostics/backend identity에 `browser-wasm/worker-offscreen-canvas-webgl2-imagebitmap`을 추가한다.
- `ADR-020-web-typescript-bootstrap.md` 또는 새 ADR에 worker-owned runtime, main DOM boundary, mode selection과 fallback을 기록한다.
- 결과를 실제 완료일의 `history/` 문서에 baseline/A/B/worker startup/브라우저/물리 검증 경계와 함께 기록한다.
- direct/same-thread path 삭제는 Firefox/Safari 및 배포 fallback 정책이 별도 승인된 뒤의 후속 작업으로 남긴다.

## 5. 검증 명령

모든 test runner와 외부 command는 `.github/copilot-instructions.md`에 따라 20분 timeout을 사용한다.

```powershell
# Release product build
dotnet build ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release

# focused shared contracts
dotnet run --project ./Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -c Release
dotnet run --project ./Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release

# browser validation dependencies/typecheck
npm ci --prefix ./Doroti/validation/web-playwright
npm run check --prefix ./Doroti/validation/web-playwright

# Release server lifecycle + Chromium/DPR2/Desktop Chrome suite
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release

# 구현 시 wrapper에 mode filter를 추가해 같은 build에서 A/B
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode document-webgl
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode offscreen-bitmap
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode offscreen-worker

git diff --check
```

## 6. 실제 사용자 검증

자동 검증과 별도로 Desktop Chrome/Edge에서 다음을 확인한다.

- 실제 precision trackpad로 30초 이상 왕복 scroll
- 실제 window border drag, maximize/restore, 화면 간 DPR 이동, browser zoom
- continuous animation 중 blank/이전 frame stretch/깜빡임 0
- canvas pointer/focus, 외부 창 이동 후 복귀, 한글 IME 조합/삭제
- semantics tree가 시각적으로 완전히 보이지 않으면서 접근성 도구에 유지되는지 확인

Playwright screenshot은 browser 내부 pixel 후보를 검증하지만 물리 모니터 scan-out acknowledgement가 아니다. 실제 trackpad, border drag, 고주사율 monitor, 한글 IME, screen reader를 실행하지 않으면 각각 `notVerified`로 남긴다.

## 7. MVP와 후속 경계

### 이번 MVP

- persistent Web Worker가 단독 소유하는 .NET Doroti runtime/framework/Skia/actual OffscreenCanvas WebGL2 surface
- main thread의 TypeScript DOM/input/IME/semantics/plugin bridge와 visible bitmaprenderer
- worker/main versioned protocol과 transferable exact ImageBitmap commit
- request-id 기반 current+latest async mailbox와 exactly-once terminal
- resize/DPR/context-loss/resource lifecycle
- same-thread OffscreenCanvas와 current document-WebGL fallback
- 세 backend의 Playwright A/B, worker correctness gate와 Chrome/Edge default 승격 판단

### 후속

- multi-view 및 HTML platform-view 사이 overlay canvas 구성
- Firefox/Safari offscreen bitmap 성능 검증과 별도 backend 승격
- Firefox/Safari worker backend 검증과 승격
- WebGPU/CanvasKit/Canvas2D/software renderer
- 90/120/144/165 Hz display별 latency budget
- mobile browser virtual keyboard/orientation/low-memory lifecycle

Worker renderer는 이번 작업의 필수 산출물이다. 같은-thread offscreen을 먼저 안정화하는 이유는 worker를 생략하기 위해서가 아니라 공통 surface/terminal 계약과 rollback 기준선을 확보하기 위해서다. worker backend의 구현 완료는 correctness/product gate로 판정하고, 성능 결과는 tuning과 `auto` default 선택에 사용한다.

## 8. 판정 규칙

- capability probe가 실패하면 “OffscreenCanvas API가 존재한다”는 사실만으로 구현을 진행하지 않는다.
- compile/test PASS만으로 visible flicker, 실제 input latency, compositor scan-out을 PASS로 쓰지 않는다.
- offscreen 구조가 들어갔다는 이유만으로 CPU/GPU memory/latency 개선을 단정하지 않는다.
- worker가 존재한다는 이유만으로 병렬 성능 개선을 단정하지 않는다. main+worker 전체 startup, memory, message handoff를 함께 판정한다.
- stale bitmap을 visible canvas에 한 번이라도 transfer하거나 request terminal이 누락/중복되면 FAIL이다.
- `ImageBitmap`을 consume/close하지 않은 경로가 있으면 FAIL이다.
- offscreen mode 실패를 software renderer로 조용히 숨기지 않는다.
- direct/same-thread fallback은 S9와 browser/deployment policy가 닫힐 때까지 rollback 기준선으로 유지한다.
- worker backend 미구현을 same-thread offscreen PASS로 대체하지 않는다.
- input/IME/semantics ownership은 renderer thread/surface 변경과 분리하며 기존 DOM owner 계약을 보존한다.

## 9. 참고

- repository-local Flutter source: `reference/flutter-master`, revision `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- HTML Standard OffscreenCanvas: <https://html.spec.whatwg.org/multipage/canvas.html#the-offscreencanvas-interface>
- MDN OffscreenCanvas: <https://developer.mozilla.org/en-US/docs/Web/API/OffscreenCanvas>
- MDN `transferToImageBitmap`: <https://developer.mozilla.org/en-US/docs/Web/API/OffscreenCanvas/transferToImageBitmap>
- .NET on Web Workers: <https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-on-webworkers?view=aspnetcore-10.0>
- Blazor with .NET on Web Workers: <https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-with-dotnet-on-web-workers?view=aspnetcore-10.0>

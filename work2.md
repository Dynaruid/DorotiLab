# Doroti Web UI Worker / Raster Worker 분리 작업 계획

- 작성일: 2026-08-31
- 상태: `planned/notStarted`
- 현재 비교 기준: `worker-direct-webgl`
- 새 실험 mode 제안: `worker-split-webgl`
- 제품 기본값: 이 계획의 qualification 전까지 기존 `auto=document-webgl` 유지
- 첫 지원 범위: Playwright bundled Chromium, hardware WebGL2, full-page single view

## 1. 결론

이 분리는 검토할 가치가 있고, 현재 남은 resize 지연을 줄일 수 있는 가장 큰 구조적 후보다. 현재 `worker-direct-webgl`은 main thread와 bitmap 왕복을 없앴지만, 하나의 Worker 안에서 다음 작업이 같은 event loop를 점유한다.

- input/resize dispatch
- framework build/layout/paint와 semantics 생성
- scene admission
- Skia command 해석과 raster
- WebGL surface/context 관리와 submit

따라서 raster가 길어지는 순간 최신 resize가 도착해도 UI framework가 새 exact scene을 만들 기회를 늦게 얻는다. UI Worker와 Raster Worker를 분리하면 이전 frame을 raster하는 동안 UI Worker가 최신 metrics로 다음 scene을 만들 수 있다. 이것은 Blazor 또는 SkiaSharp의 고정 한계를 우회하는 것이 아니라, 둘을 같은 Worker event loop에 배치한 현재 topology를 바꾸는 작업이다.

다만 “Flutter식”의 핵심은 Worker를 두 개 보이게 만드는 것이 아니다. Flutter Skwasm은 UI 측이 display list를 만들고, 같은 WASM 메모리를 공유하는 raster thread가 Skia/GPU를 소유하며, current+next queue로 오래된 render request를 버린다. 실제 source는 `Surface` 포인터와 picture 포인터를 작은 `postMessage`로 raster thread에 넘긴다. Doroti도 아래 네 원칙을 목표로 한다.

1. UI는 immutable typed display list를 생산하고 GPU 객체를 소유하지 않는다.
2. Raster는 Skia/GPU/context/visible `OffscreenCanvas`를 단독 소유한다.
3. UI→Raster hot path는 bounded current+latest이고 두 번째 rAF를 기다리지 않는다.
4. frame/resource/resize identity와 exactly-once terminal은 Worker 경계에서도 보존한다.

구현 경로는 선행 spike 결과로 선택한다.

| 경로 | 구조 | 장점 | 현재 위험 | 계획상 위치 |
|---|---|---|---|---|
| A. shared-runtime raster thread | .NET multithread runtime 하나, UI managed Worker와 raster managed Worker가 같은 WASM heap 사용 | scene object를 복사하지 않는 가장 Flutter에 가까운 경로, runtime/assembly 중복이 작음 | .NET browser MT는 현재도 experimental이고, Worker 안에서 MT runtime을 시작하는 이슈가 열려 있으며 JS/WebGL 객체는 thread affinity를 가짐 | F0에서 먼저 증명; 모든 gate 통과 시 우선 채택 |
| B. dual-runtime split | 현재 single-thread UI .NET Worker + 별도 lean raster .NET Worker, transferable binary display list | 현재 검증된 Worker runtime을 유지하고 raster crash를 UI state 손실 없이 재시작 가능 | 두 runtime의 startup/heap/native asset 중복과 encode/decode 비용 | A가 막히면 shipping 경로 |
| C. native Skia sidecar | UI .NET Worker + 전용 C++/Skia WASM raster Worker | 장기적으로 가장 작은 raster runtime과 높은 통제력 | SkiaSharp renderer/runtime-effect/text/image 자원 계층을 C ABI로 다시 구현해야 함 | A와 B가 공통 performance/memory gate를 실패할 때만 별도 계획 |

큰 refactor 전에 F0에서 A와 B를 모두 작은 fixture로 비교한다. A가 public/supported API만으로 canvas transfer, Worker rAF, Skia GPU draw, context recovery를 만들 수 없으면 억지로 runtime internals에 의존하지 않고 B로 간다. B도 scene handoff나 추가 runtime memory가 gate를 넘으면 제품 코드를 계속 확장하지 않고 C의 별도 범위/비용을 다시 승인받는다.

## 2. 현재 Doroti에서 실제로 잘라야 하는 경계

### 2.1 현재 owner

| 책임 | 현재 위치 | 분리 뒤 owner |
|---|---|---|
| DOM/input/IME/clipboard/ResizeObserver/semantics DOM | [doroti.web.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts) main thread | main thread 유지 |
| app/framework/scheduler/layout/paint | [DorotiWebWorkerRunner.cs](Doroti/src/Doroti.Target.Web.browser-wasm/DorotiWebWorkerRunner.cs)가 시작한 .NET runtime | UI Worker |
| view capability 조립 | [BrowserFrameworkHost.cs](Doroti/src/Doroti.Host.Web/BrowserFrameworkHost.cs) | UI Worker, 단 graphics capability를 UI/text/scene transport로 축소 |
| scene mailbox, text layout, image decode, semantics, Skia raster | [BrowserSkiaCapabilities.cs](Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs)와 [SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) | UI text/semantics와 Raster scene/GPU로 분리 |
| visible canvas/WebGL2/Skia surface | [doroti.raster.worker.ts](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts)와 [DorotiWebWorkerSurface.cs](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs) | Raster Worker 단독 |
| Worker bootstrap/supervision | 현재 main이 combined Worker 하나를 시작/재시작 | main이 UI/Raster 두 endpoint와 raster session을 감독 |

현재 `doroti.raster.worker.ts`라는 이름과 달리 이 Worker는 app/UI와 raster를 모두 실행한다. 분리 후에는 기존 파일의 app/bootstrap/input/frame 역할을 `doroti.ui.worker.ts`로 이동하고, 새 raster Worker는 app과 `PlatformDispatcher`를 로드하지 않아야 한다.

### 2.2 그대로 전송할 수 없는 현재 객체

[GraphicsAndSemanticsContracts.cs](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs)의 `SceneCommand`/`PathCommand`는 `object? HostPayload`에 renderer 전용 객체를 숨긴다. 다음 항목은 structured clone이나 Worker 간 managed reference로 넘길 수 없다.

- `Path`, `Paragraph`, `Image`와 `SkiaImageHandle`
- `FragmentShaderState`와 image shader의 `Image`
- reference equality에 의존하는 picture cache key, image-filter cache key와 backdrop ID
- `EngineLayer.RetainedCommands`의 managed array identity
- `DorotiFrameTransaction` 객체와 completion source

따라서 Worker만 하나 더 만들고 현재 `Scene`을 JSON으로 보내는 접근은 금지한다. 먼저 renderer 중립의 immutable display-list 계약과 stable resource ID를 만든다.

### 2.3 동기 text layout 문제

현재 `ParagraphBuilder.build()`는 framework layout 중 `IParagraphHostCapability.Layout`을 동기 호출하고, `SkiaSceneRenderer.Layout`은 Skia font로 code-unit advance를 계산한다. 이를 Raster Worker RPC로 바꾸면 UI Worker가 매 paragraph마다 raster queue를 기다리므로 분리 효과가 사라지고 deadlock 위험도 생긴다.

UI Worker는 CPU-only `SkiaTextLayoutService`를 계속 가져야 한다. Raster Worker에는 native paragraph 객체 대신 immutable text run snapshot을 보낸다. 첫 버전은 현재 기능과 동일한 text/fallback run, font resource ID, size/color, baseline, line range와 advances를 담고 pixel parity를 증명한다. 복합 shaping을 확장할 때는 glyph ID/position buffer로 진화시키되, 이번 분리 작업에서 별도 text-engine 재작성까지 섞지 않는다.

### 2.4 현재 performance 판정 경계

현재 [work.md](work.md)의 최신 자동 증거에서 `worker-direct-webgl` fast native resize는 3회 모두 640 px/500 ms 조건을 충족했고 target→front p95가 47.9~49.9 ms였다. combined fast run은 p95/max 59.6 ms, long run은 p95 52.4 ms/max 87.0 ms였다. 이는 direct path가 이미 한계에 가까이 왔다는 근거지만 물리 scan-out 수치는 아니다.

같은 문서의 Flutter differential은 wheel workload이고, 동일 native resize fixture의 Flutter paired 결과는 `notVerified`다. 따라서 split 목표는 기존 absolute `<60 ms`를 다시 통과하는 것만으로 끝내지 않고, current direct 상대 개선과 새 paired Flutter resize를 둘 다 gate로 둔다.

## 3. 참고 구조와 채택 범위

검토 기준 Flutter revision은 저장소의 `reference/flutter-master`와 동일한 `56b8e1a851a594b1a154f8ea93270807dab22b9a`다.

- Flutter [Skwasm `surface.cc`](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/skwasm/surface.cc#L17-L66)는 main-side `Surface`가 callback ID를 만들고 WASM worker에 실제 raster를 위임하며 같은 native object/display-list pointer를 사용한다.
- Flutter [`library_skwasm_support.js`](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/skwasm/library_skwasm_support.js#L38-L75)는 Worker clock domain을 보정하고 작은 control message와 transferable canvas/bitmap을 전달한다.
- Flutter [`RenderQueue`](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/rasterizer.dart#L174-L191)는 current와 next만 보존하고 새 request가 오면 next를 교체한다.
- Flutter의 범용 Web compositor는 [offscreen raster 결과를 `ImageBitmap`으로 display canvas에 넘길 수 있다](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/offscreen_canvas_rasterizer.dart#L9-L12). Doroti의 첫 범위는 full-page single view이므로 이미 검증한 transferred visible canvas direct submit을 유지한다.
- .NET runtime의 [browser threading 문서](https://github.com/dotnet/runtime/blob/main/src/mono/wasm/threads.md)는 `WasmEnableThreads`를 experimental로 설명하고 JS object의 Worker affinity와 tentative dedicated-worker API를 명시한다.
- 현재 열린 [.NET Worker 내부 MT startup 이슈 #114140](https://github.com/dotnet/runtime/issues/114140)은 현재 Doroti처럼 .NET runtime 자체를 Worker에서 시작한 뒤 MT를 켜는 topology를 release 전제로 삼으면 안 되는 근거다.

즉 Flutter에서 가져올 것은 “공유 가능한 display list, raster-owned GPU, current+next, 작은 receipt”이고, `ImageBitmap` display 단계나 현재 .NET에 없는 runtime 내부 API를 그대로 복사하지 않는다.

## 4. 목표 topology

```text
Browser main thread
  DOM / input / IME / clipboard / semantics DOM
  ResizeObserver + DPR watcher
  UI/Raster supervisor + canvas replacement
       │ input, metrics, platform reply
       ▼
UI Worker
  .NET app + PlatformDispatcher + SchedulerBinding
  build / layout / paint / semantics
  CPU text layout
  SceneProducer + current/latest outbound mailbox
       │ immutable DisplayList + resource ops
       │ direct MessagePort or shared-memory mailbox
       ▼
Raster Worker
  DisplayList consumer + resource registry
  Skia raster cache / runtime effects
  OffscreenCanvas + WebGL2 + SKSurface
  immediate drain, exact target admission, direct submit
       │ terminal / timing / resource receipt
       └──────────────────────────────► UI Worker
```

Main은 scene payload를 relay하지 않는다. main이 `MessageChannel`을 만들고 port를 UI/Raster에 각각 전달하거나, A 경로에서는 동일 runtime의 bounded shared mailbox를 연결한다. main은 worker session/fatal/canvas lease만 관찰한다.

한 framework frame에서 pacing point는 UI Worker의 frame callback 하나다. scene이 Raster Worker에 도착한 뒤 다시 raster rAF를 기다리는 `rAF → build → postMessage → rAF → raster` 구조를 만들지 않는다. Raster는 idle이면 즉시 current를 소비하고, raster 중이면 next 하나만 최신 것으로 교체한다. resize fast wake도 UI scheduling을 깨우는 용도이며 raster Worker에 무제한 timer를 만들지 않는다.

## 5. identity와 ownership 계약

| identity/state | 유일 owner | 규칙 |
|---|---|---|
| `runtimeSessionId` | main supervisor | UI runtime restart 때 증가 |
| `rasterSessionId` | main supervisor | Raster Worker/context endpoint 교체 때 증가 |
| `viewId` | shared dispatcher | 첫 범위는 하나지만 wire header에 항상 포함 |
| `resizeTargetGeneration` | main ResizeObserver/DPR publication | 동일 immutable epoch를 UI와 Raster 양쪽에 즉시 보냄 |
| `metricsGeneration`/framework frame | UI Worker | build 시작 시 한 번 capture, submit-time relabel 금지 |
| `sceneSequence` | UI `SceneProducer` | runtime session 내 단조 증가 |
| `resourceId`/generation | UI resource registry | raster session과 함께 해석; 재사용 전 generation 증가 |
| `surfaceGeneration`/`contextGeneration` | Raster Worker | `DorotiFrameDescriptor`에 섞지 않고 receipt correlation만 함 |
| external canvas lease | main supervisor | transfer 성공/worker fatal/canvas replacement를 exactly once 정산 |
| scene terminal | UI terminal ledger | Raster receipt 또는 raster-session loss로 exactly once 완료 |

직접 WebGL submit에는 compositor/scan-out ACK가 없으므로 자동 진단은 `submitted`까지만 기록한다. 이를 `presented`라고 세지 않는다. 물리 display acceptance는 계속 별도 `notVerified` 경계를 가진다.

UI는 scene buffer나 shared display-list lease를 terminal 전까지 보존한다. Raster는 scene admission 시 참조 resource를 retain하고 terminal 직후 release한다. duplicate/stale terminal은 idempotent하게 무시하되 duplicate counter는 증가시켜 protocol regression을 드러낸다.

## 6. DisplayList와 resource 설계

### 6.1 새 공용 계층

새 `Doroti.Graphics.DisplayList` 프로젝트를 만들어 `Doroti.Ui`와 `Doroti.Skia.Rendering` 사이의 renderer 중립 계약으로 둔다. 이 프로젝트는 SkiaSharp, browser JS, framework widget에 의존하지 않는다.

포함할 형식:

- fixed header: magic, protocol version, byte length, runtime/raster session, view, scene sequence
- immutable frame descriptor: resize target/metrics/framework frame/root physical size
- typed scene/layer/canvas opcode와 explicit payload struct
- string, float, color, matrix, path verb/point table
- frame-local resource reference table
- stable picture/layer/cache identity와 generation
- bounds/complexity/will-change metadata
- CRC 또는 debug hash; Release hot path에서는 길이/version/offset bounds를 반드시 검사

`object HostPayload`, anonymous object, reference-equality cache key와 renderer-specific handle은 wire/in-memory display list에 들어갈 수 없다. 알 수 없는 opcode/version/offset/resource는 fail-closed하고 해당 scene을 `failed(protocol)`로 terminal 처리한다.

### 6.2 두 transport의 공통 API

`IDorotiDisplayListTransport`는 다음 의미만 노출한다.

- `PublishScene(lease, displayList)`
- `PublishResource(operation)`
- `PublishResizeTarget(epoch)`
- `TryReadTerminal(out receipt)`
- `ResetRasterSession(sessionId)`

A 경로는 immutable managed display-list object를 shared mailbox로 넘기며 hot path에서 binary encode를 생략할 수 있다. B 경로는 pooled writer가 만든 `ArrayBuffer`를 transferable로 한 번 이동한다. 두 경로 모두 같은 validator/golden serializer를 사용해 의미 차이를 막는다.

### 6.3 resource protocol

resource message는 scene message와 같은 ordered channel을 사용한다. 첫 참조보다 `create`가 먼저, 마지막 scene terminal 뒤 `release`가 오도록 한다.

| resource | UI-side 상태 | Raster-side 상태 |
|---|---|---|
| font | fallback/font bytes와 content hash, text layout cache | 동일 bytes로 typeface/font cache 생성 |
| image | async create request와 encoded bytes 원본, width/height, clone refcount | 한 번 decode한 `SKImage`, GPU upload/cache |
| fragment program | SKSL/source hash와 sampler schema | compiled runtime effect/context generation cache |
| image shader | image resource ID, tile/matrix/filter | raster 시 shader 생성 |
| picture/layer | stable ID, generation, bounds/hints | raster cache key와 retained resource |
| image/backdrop filter cache key | UI가 부여한 숫자 ID/generation | reference equality 대신 stable key 사용 |

`IImageHostCapability.DecodeAsync`는 원래 async이므로 Raster에 encoded bytes를 보내고 dimensions/resource-created receipt를 기다릴 수 있다. UI `Image`는 `SkiaImageHandle` 대신 clone 가능한 logical resource lease를 가진다. Raster restart를 위해 UI는 active resource의 복구 가능한 source를 보존한다. transferable로 원본을 detach하지 않고 복사본을 보내거나, A 경로에서 immutable shared bytes를 사용한다.

font는 UI text measurement와 Raster glyph draw 양쪽에 필요하다. 같은 font bytes/content hash를 사용하고 font fallback 선택 결과를 text-run snapshot에 넣어 두 Worker가 다른 font를 선택하지 못하게 한다.

### 6.4 retained scene 1차 범위

첫 split qualification에서는 retained command를 새 scene 안에 완전히 펼쳐 correctness를 먼저 고정한다. 기존 picture/layer cache hit가 사라지는 회귀를 측정한 뒤, F8에서 `layerId + generation + delta/update/release` protocol을 연다. 처음부터 cross-frame retained lifetime까지 동시에 구현하지 않는다.

## 7. 단계별 작업

### F0. baseline과 topology decision spike

제품 경로를 수정하기 전에 두 spike를 opt-in fixture로 만든다.

공통 baseline:

1. 현재 `worker-direct-webgl`의 640 px/500 ms fast native resize, long resize, wheel/input, startup, steady memory, scene/raster duration을 3회 다시 수집한다.
2. 저장소 Flutter sample을 같은 viewport/content/native resize script로 맞춰 paired resize를 3회 수집한다. 현재 `work.md`의 Flutter 수치는 wheel differential이고 resize differential은 `notVerified`이므로 이를 섞지 않는다.
3. timestamp를 main/UI/Raster clock domain으로 보정하고 raw sample을 남긴다.

A spike:

- 별도 experimental configuration에서 `WasmEnableThreads=true`와 `crossOriginIsolated`를 확인한다.
- .NET runtime 1개에서 UI execution owner와 persistent raster execution owner가 서로 다른 Worker global scope/managed thread인지 진단한다.
- public API만 사용해 Raster owner에 `OffscreenCanvas`, Worker rAF, WebGL2와 최소 Skia surface를 붙인다.
- UI heartbeat/input counter를 돌리는 동안 Raster를 100 ms 의도적으로 stall하고 UI progress가 멈추지 않는지 확인한다.
- context loss/restore와 Raster owner 교체 뒤 canvas replacement가 가능한지 확인한다.
- `SkiaSharp.NativeAssets.WebAssembly`, runtime effects와 Emscripten GL interop가 threaded build에서 링크/실행되는지 확인한다.

B spike:

- 현재 combined Worker와 별도로 lean raster Worker에서 두 번째 single-thread `.NET` runtime을 시작한다.
- raster bundle은 app/framework/widget assembly를 포함하지 않고 display-list fixture + Skia surface만 로드한다.
- 16 KB, 256 KB, 1 MB, 4 MB display-list buffer를 transferable로 current+latest 전송한다.
- UI heartbeat/raster stall/context recovery/raster-only restart를 같은 방식으로 검사한다.
- 두 runtime의 startup download, instantiated WASM memory와 steady/private memory를 기록한다.

F0 선택 gate:

| 항목 | A 통과 조건 | B 통과 조건 |
|---|---|---|
| 실제 분리 | UI와 Raster owner가 별도 Worker이며 raster stall 중 UI heartbeat가 계속 진행 | UI/Raster가 별도 dedicated Worker이고 raster stall 중 UI heartbeat가 계속 진행 |
| API 안정성 | runtime private/internal symbol, reflection, generated worker patch에 의존 0 | 독립 boot manifest와 public `dotnet.create` host API만 사용 |
| visible path | Raster Worker가 transferred visible canvas의 WebGL2 owner, main/ UI `getContext` 0 | 동일 |
| hot handoff | representative 1 MB scene publish→raster dequeue p95 2 ms 이하, allocation/queue가 bounded | encode+transfer+decode p95 4 ms 이하, queue depth 2 이하 |
| memory | current direct 대비 steady total memory +25% 이하 | current direct 대비 +75% 이하이고 lean raster runtime 추가 steady heap 128 MiB 이하 |
| startup | current direct 대비 interactive-ready +15% 이하 | +25% 이하 |
| recovery | context loss 10회와 raster owner restart 3회에서 missing/duplicate lease 0 | raster-only restart 3회에서 UI state/input sequence 유지, missing/duplicate lease 0 |

A가 전부 통과하면 A를 채택한다. 하나라도 실패하고 B가 통과하면 B를 채택한다. 두 경로가 모두 실패하면 F1 이후 제품 refactor를 중단하고 native sidecar C의 별도 plan/승인을 만든다. 평균값으로 max failure를 숨기지 않는다.

### F1. typed DisplayList와 loopback renderer

예상 변경:

- `Doroti.Graphics.DisplayList` 프로젝트와 unit/contract validation 추가
- `SceneBuilder`/`Canvas`가 현재 public API를 유지하면서 typed payload를 기록하도록 변경
- current `SceneCommand`/`PathCommand`를 compatibility/debug view로만 남기고 renderer hot path에서 `HostPayload` 제거
- path, paint, shader/filter, paragraph snapshot과 resource ref encoder 구현
- `SkiaSceneRenderer` 앞에 loopback decode adapter를 두고 아직 같은 Worker에서 raster

검증:

- 모든 지원 opcode encode→decode structural equality
- truncated/overflow/unknown opcode/resource/version fuzz가 crash 없이 fail-closed
- FCR-4/5/7 fixture와 Demo screenshot/pixel diff가 current direct와 허용 오차 내 일치
- scene buffer bytes/op count/allocation/p50/p95/max artifact
- Windows/Qt/MAUI renderer build와 validation을 함께 통과해 Web 전용 format이 shared renderer를 깨지 않음

F1 종료 전에는 실제 second Worker를 product scene에 연결하지 않는다.

### F2. Skia renderer 역할 분리

현재 `SkiaSceneRenderer`의 네 역할을 분리한다.

- `SkiaTextLayoutService`: UI Worker CPU text 측정과 immutable text-run snapshot
- `SkiaDisplayListRasterizer`: Raster Worker에서 display list를 SKCanvas에 재생
- `SkiaRasterResourceRegistry`: image/font/runtime-effect/filter/picture resource와 context generation
- `DorotiSemanticsProjector`: UI Worker에서 semantics geometry/content를 main으로 전달

최종 raster project graph에서는 `Doroti.Skia.Rendering`의 `Doroti.Ui` project reference를 제거하고 `Doroti.Graphics.DisplayList`만 참조한다. desktop host는 `Doroti.Ui.Scene`을 typed display list로 바꾸는 producer adapter를 별도 조합한다.

scene admission/terminal owner는 UI `SceneProducer`로 옮기고 Raster는 raster attempt receipt를 돌려준다. `DorotiFrameTransaction` 객체 자체는 UI Worker를 떠나지 않는다. Raster receipt의 descriptor/session/scene sequence를 대조한 뒤 UI에서 `BackingStoreReady`, `VisibleSurfaceCommitted`, terminal을 진행한다.

F2 완료 조건:

- Raster assembly가 framework/app/scheduler/semantics DOM을 참조하지 않음
- Raster hot path가 `Doroti.Ui.Image`, `Paragraph`, `Path`, anonymous object나 `HostPayload`를 받지 않음
- UI text layout 중 Raster RPC 0
- current direct loopback mode의 rendering/terminal/resize regression 유지

### F3. resource registry와 lifetime

font/image/shader/picture resource create/use/release/reset protocol을 구현한다.

- ordered resource op + scene reference contract
- UI active lease와 Raster refcount ledger
- duplicate release, use-before-create, stale generation, raster-session mismatch fail-closed
- image clone/dispose와 scene in-flight가 겹쳐도 early native dispose 0
- context loss는 GPU handle만 폐기하고 recoverable CPU/source resource는 다시 upload
- raster restart는 new session으로 active resource snapshot을 재생하고 latest full scene을 다시 보냄

100,000 scene/resource churn, out-of-order injected receipt, repeated image clone/dispose, font fallback, fragment/image shader와 context-loss stress에서 active resource가 최종 0이고 missing/duplicate terminal이 0이어야 한다.

### F4. UI Worker endpoint

기존 combined `doroti.raster.worker.ts`를 역할별로 나눈다.

- `doroti.ui.worker.ts`: .NET app bootstrap, input/metrics/frame/platform message, semantics output
- `BrowserUiGraphicsCapabilities`: scene producer, text layout, image resource client, semantics
- UI→Raster direct port/shared mailbox attach와 runtime validator
- input은 main→UI 한 경로만 유지; Raster에는 pointer/key/text를 보내지 않음
- main platform reply와 plugin/clipboard/IME contract 유지

UI outbound는 `current + latest`로 bounded한다. 전송되지 않은 latest가 새 scene으로 교체되면 즉시 `superseded(ui-mailbox)` terminal을 기록하고 연관 resource lease를 해제한다. 이미 Raster가 current로 받은 scene은 Raster terminal을 기다린다.

### F5. Raster Worker endpoint와 direct visible surface

새 `doroti.skia.worker.ts`와 raster managed entry를 추가한다. 선택한 F0 경로에 따라 shared thread entry 또는 lean runtime bootstrap을 사용하되 protocol 의미는 동일하다.

Raster Worker는 다음만 소유한다.

- latest resize target와 stable backing capacity
- current+next scene queue
- display-list validator/interpreter
- Skia CPU/GPU resource cache
- OffscreenCanvas/WebGL2/context/surface generation
- direct submit와 raster/submit receipt

Raster가 idle일 때 새 scene 도착 즉시 raster를 시작한다. current 실행 중 새 scene이 오면 next를 교체하고 교체된 next에 `superseded(raster-mailbox)`를 보낸다. 같은 scene을 두 번 terminal 처리하거나 stale scene을 새 resize generation으로 relabel하지 않는다.

현재 `DorotiWebWorkerSurface`의 grow-only capacity, exact clip, shrink band clear와 context recovery는 보존한다. Raster Worker 밖에서는 `OffscreenCanvas.getContext`, framebuffer, `GRContext`, `SKSurface`를 만들지 않는다. direct path에 `createImageBitmap`, `bitmaprenderer`, main display receipt를 다시 넣지 않는다.

### F6. frame clock과 resize dual lane

main ResizeObserver publication 하나를 UI와 Raster 양쪽에 보낸다.

- UI: logical metrics admission과 framework frame scheduling
- Raster: physical target/canvas capacity와 exact scene admission
- scene: build 때 capture한 동일 generation/size/DPR descriptor 포함

UI Worker의 Worker rAF가 normal animation의 유일 pacing owner다. native resize 중 browser가 Worker rAF를 throttle할 때만 기존 최대 2회 bounded task wake를 유지한다. Raster arrival 뒤 추가 rAF는 없다.

Raster는 다음 조건이 모두 맞는 scene만 exact submit한다.

- runtime/raster session과 view 일치
- scene target generation = Raster latest target generation
- logical/physical/root size와 DPR exact 일치
- referenced resource generation 전부 존재
- surface capacity가 exact physical size 이상

stale/mismatch scene은 화면에 표시하지 않고 명시 terminal을 돌려준다. resize target만 먼저 도착했을 때 backing resize로 visible canvas를 blank로 만들지 않으며, 기존 exact front와 DOM root background/clip을 유지하다 새 exact scene을 원자적으로 submit한다.

### F7. supervisor, crash recovery와 endpoint 재바인딩

main supervisor state를 `uiSession`과 `rasterSession`으로 분리한다.

- Raster fatal: outstanding raster scene을 `failed(runtime-lost)`로 정산, canvas node 교체/transfer, Raster만 재시작, UI input/IME/app state 유지, resource snapshot + latest full scene replay
- UI fatal: app state를 신뢰할 수 없으므로 current와 같이 host/runtime 전체 재시작
- context loss: Raster session은 유지하고 context generation/GPU resource만 교체
- malformed protocol: offending endpoint fail-closed; silent ignore 금지

canvas replacement 뒤 main의 event listener, focus owner, hidden textarea, semantics root와 diagnostics host가 새 surface endpoint를 참조하는지 검사한다. raster restart가 text input composition을 취소하면 recovery gate 실패다.

### F8. split 이후의 retained compositor 최적화

F0~F7 correctness가 닫힌 뒤에만 다음 최적화를 순서대로 A/B 측정한다.

1. persistent picture/layer ID와 delta update로 unchanged display-list bytes 감소
2. size-independent picture의 Raster cache 유지와 new viewport clip 재합성
3. active resize 중 background/clip/offset만 바뀌는 stable layer를 먼저 compose하고 layout-dependent layer는 latest exact scene으로 교체
4. semantics는 geometry-only update만 active resize 동안 latest-only로 coalesce하고 focus/text/action/content는 즉시 전달
5. pooled display-list buffer와 resource batch; `SharedArrayBuffer`는 cross-origin isolation을 이미 요구하는 A 경로에서만 별도 A/B

old full scene을 임의 CSS scale하거나 다른 size의 scene을 exact로 가장하는 preview는 금지한다. retained fast compose는 별도 `responsive-replay` attempt로 진단하고 scene terminal을 소비하지 않는다. pixel/geometry correctness를 증명하지 못하면 해당 최적화만 제거한다.

### F9. qualification, default cutover와 rollback

새 mode는 `worker-split-webgl` opt-in으로 시작한다. F0~F8과 아래 자동/물리 gate가 모두 통과하기 전에는 `auto`, 기존 renderer, README 기본 URL을 바꾸지 않는다.

qualification 뒤 순서:

1. `auto=worker-split-webgl` burn-in 후보로 전환하되 `worker-direct-webgl` rollback 유지
2. clean browser start 3회, browser restart 1회, 30분 이상 continuous resize/input/animation/resource churn
3. raster-only crash/recovery 3회와 UI fatal full restart 1회
4. physical acceptance와 Flutter paired differential 통과
5. support ADR 승인 뒤에만 legacy 축소 여부 결정

## 8. 검증 계획과 gate

### 8.1 자동 correctness

새 validation:

- `Doroti.Validation.DisplayListContract`: opcode/codec/fuzz/resource/session/lifetime
- `tests/worker-split-topology.spec.ts`: Worker identity, direct port, main relay 0, UI progress under raster stall
- `tests/worker-split-resources.spec.ts`: font/image/shader create/use/release/restart
- `tests/worker-split-recovery.spec.ts`: context loss, raster fatal, canvas replacement, UI state 보존
- 기존 `resize-continuity.spec.ts`, input, IME proxy, semantics, DPR2와 context-loss suite를 split mode로 재사용

필수 invariant:

- main-thread app/framework managed execution 0; A 경로의 runtime bootstrap glue가 main에 남으면 Long Task와 idle cost를 별도 측정
- UI app runtime 1; B 경로일 때 Raster lean runtime 1, A 경로일 때 전체 runtime 1
- UI Worker의 WebGL context 0, Raster Worker 이외 `getContext` 0
- direct path `ImageBitmap` created/active/closed 모두 0
- UI/Raster mailbox high-water 각각 2 이하
- every admitted scene exactly one terminal
- every raster attempt exactly one receipt
- end-of-test active scene/resource/canvas external lease 0
- stale/mismatched generation submit 0
- raster restart 뒤 input sequence 단조성과 focus/text state 보존

### 8.2 performance

모든 run은 foreground/visible page, 같은 browser/GPU/display/DPR에서 raw sample을 보존한다. 최소 3회 실행하며 median p95만으로 단일 max failure를 숨기지 않는다.

| 항목 | qualification gate |
|---|---|
| 640 px/500 ms 이하 native fast resize | target→exact front p95 `< 50 ms`, max `< 60 ms`, 3회 연속 PASS |
| current direct 상대 개선 | 같은 session/fixture의 split p95가 direct보다 `>= 8 ms` 또는 `>= 15%` 낮고, max regression 0 |
| Flutter paired resize | Doroti p95가 같은 run의 Flutter p95 이하, Doroti max는 Flutter max + 1 measured refresh 이하 |
| long native resize | final exact `< 60 ms`, target generation lag p95 `<= 2`, final queue 0 |
| scene handoff | A p95 `<= 2 ms`, B encode+transfer+decode p95 `<= 4 ms`; 4 MB max sample도 1 refresh 미만 |
| UI starvation | raster 100 ms stall 중 non-coalescible input ingress→managed dispatch가 2 measured refresh를 넘긴 sample 0 |
| main responsiveness | measured scenario 중 main-thread Long Task `>50 ms` 0; semantics/IME task는 별도 원인 기록 |
| raster cadence | scene ready→raster start p95 1 measured refresh 미만; second-rAF wait signature 0 |
| memory/startup | F0 선택 gate 유지; 30분 run에서 active resource/heap이 단조 증가하지 않음 |
| visual proxy | captured scale/grid distortion과 right/bottom black band 0 px |

Flutter paired resize gate는 Flutter sample이 실제로 동일 fixture/native target을 관찰했을 때만 판정한다. Flutter가 다른 renderer/fallback을 사용하거나 target 폭/시간을 충족하지 못한 run은 `notVerified`로 버리고 PASS로 세지 않는다.

### 8.3 physical acceptance

자동 submit/pixel proxy 뒤 실제로 확인한다.

- 좌/우/상/하/모서리 border drag와 빠른 왕복
- maximize/restore와 monitor 간 DPR 이동
- 60 Hz와 120 Hz 이상 display
- wheel mouse/precision trackpad fling 중 resize
- resize/raster stress 중 button, slider, keyboard, 한글 IME 조합/후보창/긴 Backspace
- 실제 screen reader focus/action/label
- raster-only crash recovery 직후 focus/IME와 화면 복구

단계적 추격, stretch, black band, input 무반응, IME owner 충돌이 보이면 `FAIL`이다. Playwright screenshot이나 Worker `submitted` timestamp는 물리 scan-out PASS를 대신하지 않는다.

## 9. build, command와 artifact

모든 test subprocess는 repository 규칙대로 20분 timeout을 실제 enforce한다. 새 `run-web-split-validation.ps1`을 추가한다면 기존 wrapper의 owned-process/timeout/cleanup 방식을 재사용한다.

계획된 canonical 순서:

1. `dotnet build Doroti/src/Doroti.Graphics.DisplayList/Doroti.Graphics.DisplayList.csproj -c Release`
2. shared renderer와 Web/Qt/WindowsAppSdk/MAUI Release build
3. `dotnet run --project Doroti/validation/display-list-contract/Doroti.Validation.DisplayListContract.csproj -c Release`
4. 기존 FCR-3/4/5/6/7과 resize-contract
5. `npm run check` in `Doroti/validation/web-playwright`
6. `pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -HeadlessOnly -RendererMode worker-split-webgl`
7. split protocol/topology/resource/recovery 전용 headed test
8. current direct/split A/B 3회
9. 동일 native resize fixture의 Doroti split/Flutter differential 3회
10. physical acceptance 기록

F0 A 경로는 `WasmEnableThreads=true`가 dev server의 isolation header를 실제 적용하는지 response header와 `crossOriginIsolated` 양쪽에서 검사한다. production hosting은 `Cross-Origin-Opener-Policy`/`Cross-Origin-Embedder-Policy`와 모든 font/image/plugin asset의 CORP/CORS 호환성을 별도 deploy gate로 둔다. local success만으로 일반 host 호환성을 주장하지 않는다.

artifact 필드:

- commit/configuration/browser/OS/GPU/driver/display Hz/DPR
- selected topology A/B와 runtime/Worker/thread identity
- cross-origin isolation/header/capability probe
- runtime/raster session, resize target/metrics/framework frame/scene/resource/surface/context generation
- input ingress, UI dispatch, build start/end, scene encode/publish/dequeue, raster start/end, submit/terminal timestamp
- raw buffer bytes/op/resource count, queue depth/high-water, replaced current/latest count
- startup/download/instantiated WASM/steady memory와 30분 trend
- terminal/receipt/resource/external lease totals
- screenshot/video/pixel proxy와 automatic/physical 판정 분리

## 10. 예상 파일 범위

새 파일/프로젝트 후보:

- `Doroti/src/Doroti.Graphics.DisplayList/`
- `Doroti/src/Doroti.Host.Web/Web/doroti.ui.worker.ts`
- `Doroti/src/Doroti.Host.Web/Web/doroti.skia.worker.ts`
- `Doroti/src/Doroti.Host.Web/Web/doroti.web.raster-protocol.ts`
- A 선택 시 shared mailbox/thread host, B 선택 시 lean raster runtime entry project
- `Doroti/validation/display-list-contract/`
- split topology/resource/recovery Playwright specs
- `Doroti/eng/run-web-split-validation.ps1`

주요 수정 후보:

- [GraphicsAndSemanticsContracts.cs](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs)
- [SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs)
- [BrowserSkiaCapabilities.cs](Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs)
- [BrowserFrameworkHost.cs](Doroti/src/Doroti.Host.Web/BrowserFrameworkHost.cs)
- [DorotiWebWorkerSurface.cs](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs)
- [doroti.raster.worker.ts](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts)
- [doroti.web.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)
- [doroti.web.protocol.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts)
- [Sdk.targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)
- renderer selector/type declaration, README와 ADR

shared desktop hosts는 같은 typed display list를 in-process로 `SkiaDisplayListRasterizer`에 넘긴다. Web split을 위해 `Doroti.Ui`에 browser/Worker/MessagePort 개념을 올리거나 다른 platform renderer를 두 번 유지하지 않는다.

## 11. 완료/중단/rollback 조건

구조 완료:

- main/ UI/ Raster owner가 진단으로 구별되고 각 책임이 위 표와 일치
- UI framework work와 Raster GPU work가 실제로 concurrent progress
- UI→Raster contract에 managed/native renderer handle 0
- Raster arrival 뒤 second rAF wait 0
- app/framework scheduler owner 1개, Raster는 frame scheduler가 아니라 bounded consumer
- raster-only restart에서 app/UI/input/IME state 유지
- scene/resource/external lease가 bounded하고 exactly once 정산

qualification 완료:

- Release, TypeScript, DisplayList, FCR, resize/input/semantics/resource/recovery 자동 gate `PASS`
- current direct 대비 correctness regression 0
- 동일 fixture Flutter paired resize gate `PASS`
- 실제 border drag/60 Hz/120 Hz+/trackpad/IME/screen reader acceptance `PASS`

즉시 중단 또는 해당 단계 rollback:

- A가 runtime private API나 현재 열린 Worker-inside-worker MT 동작에 의존
- B의 lean raster bundle이 app/framework를 다시 포함하거나 memory/startup gate 실패
- synchronous text/resource RPC가 framework layout/paint hot path에 생김
- scene serialization/transfer가 한 refresh를 반복해서 소비
- Worker 분리 뒤 stale exact admission, black band, resource leak, duplicate/missing terminal 발생
- raster fatal 뒤 canvas replacement와 UI state 보존을 안전하게 완료하지 못함
- split mode가 paired Flutter 또는 current direct보다 input/resize p95를 악화

이 경우 `worker-direct-webgl`을 유지하고 `auto`를 바꾸지 않는다. 자동 gate만 통과하고 physical acceptance가 남으면 상태는 `partial/notVerified`이며 “Flutter와 동등 이상”으로 판정하지 않는다.

## 12. 명시적 제외

첫 qualification에서 제외:

- Firefox/Safari/비-Chromium 보장과 cross-origin-isolation 불가능 host
- embedded/multi-view와 DOM platform-view overlay
- software/Canvas2D fallback과 WebGPU backend
- native Skia sidecar C의 구현
- text shaping 기능 확장 자체; 현재 behavior parity만 포함
- app state persistence를 포함한 UI Worker fatal 복구
- compositor scan-out ACK 추정
- 기존 renderer 삭제와 public API cutover

이 제외 항목은 자동으로 PASS 처리하지 않고 각각 `notVerified` 또는 후속 계획으로 남긴다.

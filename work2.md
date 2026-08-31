# Doroti Web CanvasKit Raster Worker 전환 작업 계획

상태: `planned / notStarted`

작성일: `2026-08-31`

대상: `Doroti.Host.Web`의 Chromium/WebGL2 제품 경로

문서 성격: 구현 전 작업 계획. 이 문서 변경만으로 build, 성능, 시각 품질이 검증된 것은 아니다.

## 1. 결정

기존 `work2.md`의 여러 구현 후보와 선택 matrix는 폐기한다. Doroti Web의 목표 구조를 다음 하나로 고정한다.

- Browser main thread는 DOM, input, IME, semantics, canvas CSS geometry, `ResizeObserver`와 DPR 관찰만 소유한다.
- UI Worker는 단 하나의 .NET runtime, Doroti framework/widget tree, scheduler, scene build를 소유한다.
- Raster Worker는 .NET runtime 없이 npm `canvaskit-wasm`과 visible `OffscreenCanvas`의 hardware WebGL2 context를 소유한다.
- UI Worker와 Raster Worker는 `MessageChannel` 위의 versioned binary DisplayList와 transferable resource buffer만 교환한다.
- `SharedArrayBuffer`, COOP/COEP, `WasmEnableThreads`, pthread를 요구하지 않는다.
- Web renderer에서는 최종적으로 `SkiaSharp`, `SkiaSharp.NativeAssets.WebAssembly`, `Doroti.Skia.Rendering` 의존성을 제거한다. Desktop/MAUI/Qt renderer의 SkiaSharp 사용은 이 계획의 제거 대상이 아니다.
- CanvasKit 객체는 Worker 사이에 공유하지 않는다. 각 owner 안에서만 생성·사용·폐기하고, Worker 경계에는 값, ID, byte buffer만 전달한다.

최종 renderer mode 이름은 `worker-canvaskit-webgl`로 한다. 기존 `document-webgl`, `offscreen-bitmap`, `offscreen-worker`, `worker-direct-webgl`은 전환 기간의 rollback/비교 기준으로만 유지하며, qualification 전에는 `auto` 기본값을 바꾸지 않는다.

## 2. 목표와 비목표

### 2.1 목표

1. raster stall, shader compile, image decode, GPU flush가 UI .NET event loop와 input dispatch를 막지 않게 한다.
2. SAB가 꺼져 있어도 동작하는 명시적 ownership/transfer 구조를 만든다.
3. CanvasKit을 npm에서 정확한 버전으로 가져와 first-party 정적 asset으로 build/publish한다.
4. current Skia renderer와 동일한 scene, text, image, filter, runtime effect 결과를 CanvasKit renderer에서 재현한다.
5. resize 동안 logical CSS geometry와 physical backing geometry를 분리하고, stale/mismatched frame을 표시하지 않는다.
6. resource와 CanvasKit Embind object의 수명을 계측해 context loss와 Raster Worker 재시작 뒤에도 leak 없이 복구한다.
7. 자동 correctness, 정량 성능, 실제 사용자 시각/input/accessibility 판정을 분리해 기록한다.

### 2.2 비목표

- 두 .NET runtime 사이의 객체 공유 또는 runtime 내부 API를 이용한 managed heap 공유
- SAB를 켠 브라우저만 지원하는 fast path
- custom C++/Skia sidecar, 별도 native daemon, 서버 raster
- full-frame `ImageBitmap`, `readPixels`, PNG/Blob encode를 정상 hot path로 사용하는 구조
- CanvasKit의 Canvas2D emulation 또는 software surface로 조용히 fallback하는 구조
- CanvasKit `SkPicture`/Embind handle을 canonical wire format으로 사용하는 구조
- CSS stretch, clip, black fill로 늦은 exact frame을 시각적으로 숨기는 구조
- Firefox/Safari를 이 단계에서 제품 지원으로 승격하는 일
- Desktop/MAUI/Qt renderer를 CanvasKit으로 교체하는 일

## 3. 현재 기준선과 실제 절단 지점

현재 Web 경로는 이름과 달리 `doroti.raster.worker.ts` 하나가 .NET runtime, framework/UI, Skia raster, OffscreenCanvas/WebGL을 함께 소유한다. main은 worker supervision과 DOM bridge를 담당한다. 이 구조에서는 Worker가 존재해도 UI와 raster가 같은 event loop와 managed runtime 안에 있으므로 raster 정지가 UI scheduling에도 영향을 준다.

현재 코드에서 Worker 경계를 바로 넘길 수 없는 항목은 다음과 같다.

- `SceneCommand`, `PathCommand`의 `object? HostPayload`
- `Path`, `Paragraph`, `UiImage`, `SkiaImageHandle`, shader/filter와 runtime-effect host object
- reference equality 기반 picture cache key와 retained command array identity
- `DorotiFrameTransaction`과 host callback
- `SKTypeface`, `SKFont`, `SKImage`, `SKPaint`, `SKSurface` 등 SkiaSharp object

또한 `IParagraphHostCapability.Layout`은 동기 API다. 현재 `SkiaSceneRenderer.Layout`은 Skia font/fallback을 이용해 code-unit advance를 계산하고 즉시 `Paragraph`를 반환한다. 이를 Raster Worker RPC로 바꾸면 UI Worker가 동기 응답을 기다려야 하며, SAB가 없는 환경에서는 올바른 설계가 아니다.

따라서 전환의 실제 절단 지점은 다음 둘이다.

1. framework object graph와 renderer 사이에 renderer-neutral typed DisplayList/resource protocol을 만든다.
2. 동기 text layout을 UI Worker 안의 CPU-only CanvasKit text service로 옮기고, Raster Worker에는 재구성 가능한 immutable paragraph recipe만 보낸다.

## 4. 목표 topology와 ownership

```text
Browser main thread
  DOM / input / IME / semantics
  ResizeObserver + DPR watcher + metrics sampling rAF
  visible canvas CSS logical size
  worker supervisor and fatal/restart policy
  .NET runtime 0 / CanvasKit runtime 0
            |
            | input, metrics, semantics, control
            v
UI Worker
  .NET runtime 1
  Doroti framework / widget tree / scheduler
  CPU-only CanvasKit text-layout instance 1
  typed DisplayList encoder / resource journal
  no visible canvas / no WebGL context
            |
            | dedicated MessagePort
            | transferable ArrayBuffer + resource messages
            v
Raster Worker
  .NET runtime 0
  CanvasKit GPU instance 1
  DisplayList decoder / resource registry
  visible OffscreenCanvas + hardware WebGL2
  flush / receipt / context recovery
```

main은 두 Worker를 만들고 UI↔Raster 전용 `MessageChannel`의 port를 각각 한 번만 넘긴다. steady state scene traffic은 main을 경유하지 않는다. main은 canvas lease와 DOM lifecycle을 감독하지만 transferred canvas의 `getContext()`를 호출하지 않는다.

UI text service와 Raster Worker는 같은 fingerprinted `canvaskit.wasm`, 같은 CanvasKit version, 같은 font bytes를 각각의 독립 WASM heap에 load한다. 객체를 공유하지 않으며 binary download cache 재사용만 기대한다. 이 중복 memory/startup 비용은 숨기지 않고 qualification gate에서 측정한다.

## 5. CanvasKit build와 배포 계약

### 5.1 dependency pin

- 첫 구현 pin은 `canvaskit-wasm@0.42.0`으로 고정한다.
- `Doroti/src/Doroti.Host.Web/Web/package.json`과 `package-lock.json`을 추가하고 `npm ci`만 canonical restore로 사용한다.
- Worker bundle 도구도 `package.json`에서 exact version으로 고정한다. global npm package나 floating `latest`에 의존하지 않는다.
- CanvasKit license와 third-party notice를 publish artifact에 포함한다.
- CDN, runtime npm registry fetch, unversioned public URL은 금지한다.

version 변경은 단순 patch update로 취급하지 않는다. DisplayList golden, text metrics, shader/filter, context recovery, publish hash gate를 다시 통과한 뒤 pin과 provenance를 함께 갱신한다.

### 5.2 asset pipeline

현재 Host.Web은 `Microsoft.TypeScript.MSBuild`의 `tsc` emit만 사용하며 browser bare import를 bundle하지 않는다. CanvasKit 도입 시 다음의 host-specific build 단계를 추가한다.

1. restore 단계에서 lockfile과 일치하는 `npm ci`를 실행한다.
2. `doroti.canvaskit.layout.ts`와 `doroti.canvaskit.worker.ts`를 browser/ES2022 ESM bundle로 만든다.
3. npm package의 `canvaskit.wasm`을 immutable static web asset으로 등록한다.
4. JS bundle과 WASM에 content fingerprint를 부여하고 생성 manifest에 logical name, URL, byte length, SHA-256, CanvasKit version을 기록한다.
5. 두 Worker 모두 manifest의 같은 WASM URL을 `locateFile`에 제공한다.
6. trimmed publish에서도 bundle, WASM, license, manifest가 누락되지 않는지 검사한다.

generic `Doroti.TypeScript.targets`에 CanvasKit 전용 동작을 무조건 넣지 않는다. 먼저 `Doroti.Host.Web.csproj` 또는 별도 `Doroti.CanvasKit.targets`에 한정하고, 여러 package가 실제로 재사용할 때만 SDK 공통 target으로 승격한다.

### 5.3 fail-closed GPU 생성

Raster Worker는 convenience API가 software canvas로 fallback하게 두지 않는다. CK0에서 아래 순서가 Worker + `OffscreenCanvas`에서 실제로 동작함을 고정한다.

1. `CanvasKit.GetWebGLContext(offscreenCanvas, { majorVersion: 2, failIfMajorPerformanceCaveat: 1, ... })`
2. `CanvasKit.MakeWebGLContext(handle)`
3. `CanvasKit.MakeOnScreenGLSurface(grContext, physicalWidth, physicalHeight, CanvasKit.ColorSpace.SRGB)`
4. scene draw 후 `surface.flush()`

WebGL2 context, GPU context, surface 중 하나라도 생성되지 않으면 해당 mode를 `unsupported` 또는 `failed`로 끝낸다. `MakeCanvasSurface`의 software fallback, Canvas2D fallback, hidden DOM canvas 교체를 호출하지 않는다. renderer/vendor와 context attributes를 diagnostics에 남기고 SwiftShader/llvmpipe 등 software renderer는 제품 qualification에서 FAIL로 처리한다.

`surface.flush()`는 GPU submit 완료일 뿐 compositor scan-out ACK가 아니다. 자동 terminal 이름은 `submitted`, `superseded`, `failed`로 유지하며 `presented`라고 과장하지 않는다.

## 6. Worker protocol과 DisplayList

### 6.1 wire 원칙

- binary little-endian protocol, explicit schema version, fixed-width header를 사용한다.
- JSON은 bootstrap/diagnostics처럼 저빈도 control에만 허용한다.
- scene buffer는 transferable `ArrayBuffer`로 넘기고 sender가 계속 참조하지 않는다.
- scene mailbox는 `rendering 1 + latest pending 1`로 bounded한다. FIFO backlog를 만들지 않는다.
- admitted scene마다 정확히 하나의 terminal, raster attempt마다 정확히 하나의 receipt가 있어야 한다.
- unknown opcode/version/resource는 조용히 skip하지 않고 frame을 `failed`로 끝낸다.
- buffer pool은 receipt와 함께 UI Worker에 반환하며 detached buffer 재사용을 계측한다.

DisplayList header의 최소 필드는 다음과 같다.

| 필드 | 의미 |
|---|---|
| `magic`, `schemaVersion`, `byteLength` | decoder compatibility와 bounds check |
| `viewId`, `sceneSequence`, `buildToken` | framework scene identity |
| `resizeEpoch`, `surfaceGeneration`, `contextGeneration` | geometry/context identity |
| logical width/height, physical width/height, DPR | exact-size admission |
| command/resource count, string-table length | bounded decode |
| optional checksum/diagnostic flags | corruption과 capture 재현 |

### 6.2 command model

`Doroti.Graphics.DisplayList` 프로젝트를 renderer-neutral source of truth로 추가한다. public Canvas/SceneBuilder API는 유지하되 hot path 기록을 typed command와 value payload로 바꾼다.

최소 opcode 범위는 현재 `SkiaSceneRenderer`가 실제 처리하는 기능 전체다.

- save/restore, saveLayer, transform, clip rect/rrect/path
- draw color, line, points, rect/rrect/oval/arc/path, shadow
- paint color/style/stroke/cap/join/anti-alias/blend mode
- gradients, color filter, mask filter, image filter, backdrop filter
- image, image-rect, sampling quality, nine-patch가 있으면 그 경로
- paragraph/text, fallback font, max lines, width/height
- runtime effect/fragment shader와 uniform/child resource
- retained picture/scene reference와 cache hint

`object HostPayload`, arbitrary CLR object, JS/CanvasKit object, pointer, delegate, reference equality는 protocol에 들어갈 수 없다. path는 verb + numeric buffer, paint/filter/shader는 tagged union, string은 UTF-8 table, matrix/color/rect는 명시적 value encoding으로 바꾼다.

### 6.3 resource identity와 수명

resource key는 `(viewId, kind, id, version)`으로 고정한다. message는 최소 다음을 포함한다.

- `registerFont`, `registerImage`, `registerRuntimeEffect`
- `retainResource`, `releaseResource`, `dropGeneration`
- `submitScene`, `sceneReceipt`
- `resizeTarget`, `contextLost`, `contextRestored`
- `rasterReady`, `rasterFatal`, `shutdown`

UI Worker는 recoverable resource journal을 소유한다. Raster Worker가 재시작하거나 context generation이 바뀌면 live font/image/effect descriptor를 새 owner에게 replay한 뒤 scene을 허용한다. 오래된 generation의 resource ACK는 무시하되 정확히 하나의 terminal로 닫는다.

CanvasKit의 `Paint`, `Path`, `Image`, `Shader`, `ImageFilter`, `RuntimeEffect`, `Paragraph`, `Surface`, `GrDirectContext` 등 Embind object는 owner가 명시적으로 `.delete()`한다. JS GC에 해제를 맡기지 않는다. diagnostics는 kind별 created/live/deleted count, byte estimate, generation을 제공한다.

### 6.4 text layout 계약

동기 paragraph layout을 Raster Worker RPC로 만들지 않는다. UI Worker 안에서 WebGL surface 없이 CanvasKit text API와 등록 font bytes를 사용하는 `CanvasKitTextLayoutService`를 초기화한다.

흐름은 다음과 같다.

1. application font bytes를 UI text service와 Raster resource registry에 같은 `(fontId, version, sha256)`로 등록한다.
2. UI service가 text/style/locale/direction/maxWidth로 paragraph를 layout한다.
3. framework에는 width, height, baselines, line metrics, code-unit/range geometry, hit-test 자료를 flat immutable snapshot으로 반환한다.
4. DisplayList에는 원본 text, normalized style, font resource ID, layout width, line-break/metrics hash를 포함한 paragraph recipe를 기록한다.
5. Raster Worker가 같은 CanvasKit/version/font bytes로 paragraph를 재구성하고 layout한 뒤 metrics hash를 확인하고 draw한다.
6. hash나 line count가 다르면 잘못 그리지 말고 frame을 `failed(text-layout-mismatch)`로 끝낸다.

system font 이름만으로 두 Worker가 각자 fallback을 고르게 하지 않는다. 기본 Latin과 한글 fallback을 포함한 허용 font set을 asset manifest로 고정하고 family/weight/style/fallback order를 protocol에 기록한다. surrogate pair, combining mark, emoji/color font, RTL, line break, ellipsis, locale별 fixture를 둔다.

CanvasKit text module이 Worker에서 DOM 없이 동작하지 않거나 동일 recipe가 두 instance에서 결정적으로 재현되지 않으면 CK0 hard stop이다. 그 문제를 synchronous cross-worker wait나 SAB로 우회하지 않는다.

### 6.5 image와 runtime effect

image API는 기존처럼 async이므로 encoded bytes와 resource ID를 Raster Worker에 보내 decode할 수 있다. UI Worker는 restart replay가 가능한 원본 또는 재취득 descriptor를 보존한다. Raster는 decode receipt에 width/height/color type을 반환하고, `UiImage`는 host object 대신 logical resource handle만 가진다.

runtime effect는 SkiaSharp compiled object가 아니라 SkSL source hash, uniform layout, child resource ID로 전달한다. CanvasKit compile failure는 resource-level error와 dependent frame failure로 드러내며 silent fallback paint를 금지한다.

## 7. frame, resize, present 계약

resize authority는 root `ResizeObserver`와 DPR watcher다. main은 immutable metrics를 UI와 Raster에 publish하고 다음 identity를 혼합하지 않는다.

- `resizeEpoch`: 관찰된 logical/physical/DPR target
- `sceneSequence`와 `buildToken`: 그 target으로 build한 scene
- `surfaceGeneration`: physical backing/surface generation
- `contextGeneration`: WebGL/CanvasKit context lifetime

main은 visible canvas의 `style.width/style.height`를 logical px로 소유하고 Raster Worker는 physical backing과 CanvasKit surface를 소유한다. DPR 2라면 예를 들어 CSS 1080×720, backing 2160×1440이며 `transform: none`이어야 한다.

Raster Worker는 별도의 raster `requestAnimationFrame`을 기다리지 않는다. message arrival 또는 active raster 종료 시 current+latest mailbox를 즉시 drain한다. main의 rAF는 metrics sampling/DOM work에만 사용한다.

active resize 중에도 completed frame 자체의 immutable logical/physical size가 정확하고 visible front보다 새로우면 monotonic progressive exact commit을 허용한다. 다음은 항상 거부한다.

- target generation label만 새 것으로 바꾼 오래된 scene
- logical/physical/intrinsic dimension이 서로 다른 frame
- stale context/surface generation의 submit
- CSS stretch가 필요한 frame

visible OffscreenCanvas는 Raster Worker에 한 번 transfer되면 되돌릴 수 없다. Raster Worker fatal 시 main은 기존 canvas를 폐기하고 동일 DOM 위치/semantics 연결을 가진 새 canvas를 생성해 새 Worker에 한 번 transfer한다. canvas lease는 `created → transferred → retired` terminal을 정확히 한 번 가진다. UI state, focus, editing state, input sequence는 UI Worker가 유지한다.

## 8. 구현 단계

각 단계는 앞 단계 gate가 PASS일 때만 다음으로 간다. `notVerified`를 PASS로 간주하지 않는다.

### CK0. 기준선 고정과 CanvasKit feasibility hard gate

예상 작업:

- current `auto`, `document-webgl`, `offscreen-worker`, `worker-direct-webgl`의 build/startup/memory/latency/visual baseline을 같은 fixture로 저장
- current scene command와 Skia renderer 기능 inventory 및 representative golden corpus 작성
- exact `canvaskit-wasm@0.42.0`, bundler, lockfile, static asset manifest spike
- Raster Worker에서 transferred visible OffscreenCanvas + explicit hardware WebGL2 + `MakeOnScreenGLSurface` + `flush()` smoke
- UI Worker에서 DOM/WebGL 없이 CanvasKit font/paragraph layout smoke
- 한 CanvasKit package/WASM URL을 두 Worker에서 독립 초기화하고 startup/heap 비용 측정
- context loss/restore 10회, Raster Worker replacement 3회, canvas replacement smoke

CK0 PASS 조건:

- dev build와 trimmed publish 모두 fingerprinted JS/WASM을 same-origin에서 load
- network에 CDN/npm registry request 0
- main/UI의 visible canvas `getContext` call 0, Raster owner만 WebGL2 context 1
- software/Canvas2D fallback 0
- UI text recipe와 Raster 재-layout의 line/metrics hash mismatch 0
- context/restart run의 missing/duplicate canvas/resource terminal 0
- private CanvasKit/Skia symbol patch나 generated bundle 사후 monkey patch 0

하나라도 구조적으로 불가능하면 CK1 이후 제품 refactor를 시작하지 않고 `blocked` 근거를 기록한다.

### CK1. typed DisplayList와 same-worker loopback

예상 작업:

- `Doroti.Graphics.DisplayList`와 encoder/decoder/validator 추가
- `SceneBuilder`/Canvas recording에서 `HostPayload` 제거
- SkiaSceneRenderer 앞에 DisplayList loopback adapter를 두어 아직 같은 Worker에서 raster
- malformed length/opcode/resource/version fuzz와 deterministic encode golden 추가

PASS 조건:

- representative app의 old direct vs loopback pixel/geometry golden이 승인 tolerance 내 일치
- encode→decode→re-encode canonical bytes 일치
- invalid buffer가 out-of-bounds read나 Worker crash 없이 정확히 하나의 failure terminal 생성
- non-Web desktop renderer build/API contract regression 0

### CK2. UI-owned text/resource service

예상 작업:

- `CanvasKitTextLayoutService` JS bridge와 managed `IParagraphHostCapability` adapter 추가
- registered font manifest/fallback order/metrics snapshot 도입
- `UiImage`와 runtime effect를 logical resource handle로 변경
- recoverable resource journal과 retain/release protocol 구현
- current Skia text service와 CanvasKit text service differential fixture 추가

PASS 조건:

- Latin, 한글, surrogate, combining, RTL, wrapping, ellipsis fixture의 layout/hit-test contract PASS
- UI layout 호출 중 Raster Worker round trip 0
- resource dispose/re-register/restart에서 stale handle 사용 0
- font/image/effect bytes와 logical handle 외 host object가 DisplayList에 남지 않음

### CK3. UI Worker와 Raster Worker 분리

예상 작업:

- current combined `doroti.raster.worker.ts`를 `doroti.ui.worker.ts` 역할로 축소
- 새 `doroti.canvaskit.worker.ts`와 direct UI↔Raster `MessageChannel` 추가
- ready/unsupported/fatal/shutdown state machine 구현
- current+latest scene mailbox, buffer return pool, receipt ledger 구현
- diagnostics에 runtime/owner count와 queue/terminal counter 추가

PASS 조건:

- main `.NET=0`, UI Worker `.NET=1`, Raster Worker `.NET=0`
- Raster Worker CanvasKit GPU owner 1, UI Worker CanvasKit text owner 1
- raster 100 ms 인위적 stall 중 UI heartbeat와 non-coalescible input dispatch 지속
- scene queue high-water 2 이하, every admitted scene exactly one terminal
- main을 경유한 steady-state DisplayList traffic 0

### CK4. CanvasKit renderer parity

구현 순서:

1. transform/clip/save/restore와 basic geometry
2. paint/blend/stroke/anti-alias
3. path/shadow/gradient
4. image/sampling
5. paragraph/text/fallback
6. saveLayer/color/mask/image/backdrop filter
7. ShaderMask의 neutral layer + child draw + shader blend + restore 의미
8. runtime effects/uniform/child shader
9. retained picture cache와 cache eviction

각 묶음은 current Skia loopback renderer와 pixel golden을 통과한 뒤 다음으로 간다. unsupported opcode는 magenta placeholder로 숨기지 않고 deterministic failure로 노출한다.

PASS 조건:

- representative demo와 command corpus에 missing/extra draw 0
- tolerance 밖 pixel diff, black band, scale/grid distortion 0
- saveLayer/filter/runtime-effect 중간 surface leak 0
- CanvasKit object live count가 scene churn 뒤 baseline으로 회복

### CK5. context/resource/restart lifecycle

예상 작업:

- context generation별 CanvasKit surface/GrContext/resource registry 구축
- `webglcontextlost/restored`와 fatal worker replacement 처리
- live resource journal replay와 latest exact scene replay
- intentional crash/invalid resource/compile failure diagnostics

PASS 조건:

- context loss/restore 10회와 Raster Worker replacement 3회에서 blank terminal state 0
- old context object submit 0, stale resource ACK 적용 0
- replacement 뒤 UI state/input sequence/focus/editing state 유지
- 종료 시 active scene/resource/canvas external lease 0

### CK6. direct visible resize와 pacing

예상 작업:

- main logical CSS size/Raster physical backing ownership 고정
- immutable resize epoch와 exact-size admission 연결
- no-raster-rAF immediate mailbox drain
- progressive exact commit과 final exact receipt 계측
- DPR 1/1.25/1.5/2, native border resize, maximize/restore fixture 추가

PASS 조건은 9.3절의 정량 gate를 따른다. ResizeObserver callback에서 surface reset, clear, retained-front blit, synchronous Worker wait를 하지 않는다.

### CK7. Web dependency와 publish cutover

예상 작업:

- `Doroti.Host.Web.csproj`에서 `SkiaSharp`, `SkiaSharp.NativeAssets.WebAssembly`, `Doroti.Skia.Rendering`, Web 전용 `Doroti.Skia.RuntimeEffects` 참조 제거
- Web host capability를 CanvasKit text/resource adapter와 protocol client로 교체
- `Doroti.Target.Web.browser-wasm.targets`의 CanvasKit 금지 `DOROTIWEB007`을 제거하고 required/pinned CanvasKit asset 검증으로 대체
- target manifest의 backend/runtime/ownership 정보를 새 topology로 갱신
- publish에 SkiaSharp WebAssembly native asset이 남으면 실패하는 negative gate 추가

PASS 조건:

- Web restore/build/publish graph에 SkiaSharp package/native asset 0
- Desktop/MAUI/Qt Release build는 기존 SkiaSharp renderer를 계속 사용하고 regression 0
- missing/tampered CanvasKit JS/WASM/license/manifest 각각이 명확한 build 또는 startup failure를 냄
- CSP가 same-origin Worker/WASM만으로 동작하고 `unsafe-eval` 추가 요구 0

### CK8. allocation과 성능 최적화

correctness 전에는 시작하지 않는다.

- DisplayList buffer pool과 string/resource table dedup
- unchanged retained subtree/resource version 재사용
- CanvasKit Paint/Path/Paragraph/Image cache의 bounded eviction
- shader compile warmup과 negative cache
- text-layout CanvasKit의 font/paragraph lifetime 축소
- diagnostics off production fast path 검증

최적화 때문에 schema identity, exact-size admission, terminal accounting을 완화하지 않는다.

### CK9. qualification, 기본값 승격, rollback window

모든 자동 correctness와 정량 gate가 PASS하고 physical acceptance가 별도로 기록된 뒤에만 `auto=worker-canvaskit-webgl`로 바꾼다.

- CanvasKit mode가 unsupported면 명시적으로 기존 renderer로 한 번 fallback할 수 있으나, 사용자가 CanvasKit mode를 강제한 경우 silent fallback하지 않는다.
- 기존 renderer는 최소 한 release 동안 rollback flag로 유지한다.
- rollback 기간의 crash/performance telemetry와 issue가 승인 기준 안이면 obsolete Web renderer/combined worker를 후속 cleanup한다.
- qualification 실패는 CanvasKit 구현을 성공으로 포장하지 않는다. `auto`는 기존 mode에 남기고 FAIL 항목과 재개 조건을 기록한다.

## 9. 검증과 판정 기준

### 9.1 automatic correctness

- DisplayList schema/golden/round-trip/malformed/fuzz
- every admitted scene exactly one `submitted|superseded|failed`
- every raster attempt exactly one receipt
- UI/Raster mailbox high-water 각각 2 이하
- stale scene/resource/context/surface generation submit 0
- end-of-test CanvasKit object/resource/buffer/canvas lease 0
- main managed runtime 0, UI managed runtime 1, Raster managed runtime 0
- image/font/effect restart replay와 intentional crash recovery
- DPR2에서 logical CSS 1080×720 / physical backing 2160×1440 / `transform:none`
- CanvasKit/SkiaSharp Web asset manifest negative tests

### 9.2 visual differential

current Skia loopback와 CanvasKit을 동일 scene bytes, viewport, DPR, font asset으로 capture한다.

- basic geometry는 exact 또는 1 px anti-alias tolerance
- text는 glyph bounds/baseline/line break를 별도 비교하고 pixel diff를 함께 보존
- filter/shadow/runtime effect는 승인 mask와 raw diff를 모두 보존
- golden update는 renderer 변경과 같은 commit에서 자동 승인하지 않고 원인/스크린샷/metric을 review한다.
- Flutter sample은 동일 fixture와 target을 실제 관찰한 run만 참고 비교한다. 다른 renderer/fallback/viewport인 run은 `notVerified`다.

### 9.3 performance qualification

모든 run은 foreground/visible page, 같은 browser/GPU/display/DPR에서 raw sample을 보존한다. 최소 3회 실행하며 median p95만으로 단일 max failure를 숨기지 않는다.

| 항목 | qualification gate |
|---|---|
| 640 px/500 ms 이하 native fast resize | target→exact submit p95 `< 50 ms`, max `< 60 ms`, 3회 연속 PASS |
| current direct 상대 개선 | 같은 session/fixture의 CanvasKit split p95가 direct보다 `>= 8 ms` 또는 `>= 15%` 낮고 max regression 0 |
| long native resize | final exact `< 60 ms`, target generation lag p95 `<= 2`, final queue 0 |
| scene handoff | encode+transfer+decode p95 `<= 4 ms`; representative 4 MiB max도 1 measured refresh 미만 |
| UI starvation | raster 100 ms stall 중 non-coalescible input ingress→managed dispatch가 2 measured refresh를 넘긴 sample 0 |
| main responsiveness | measured scenario 중 main-thread Long Task `> 50 ms` 0; semantics/IME task는 원인을 별도 기록 |
| raster cadence | scene ready→raster start p95 1 measured refresh 미만; second-rAF wait signature 0 |
| startup | current direct 대비 interactive-ready regression `<= 25%` |
| memory | current direct 대비 steady total regression `<= 75%`; 30분 churn 뒤 live heap/resource가 단조 증가하지 않음 |
| visual proxy | scale/grid distortion과 right/bottom black band 0 px |

UI text CanvasKit과 Raster CanvasKit의 이중 instance 때문에 memory gate를 넘지 못하면 이를 숨기거나 SAB로 우회하지 않는다. font subset, lazy init, cache budget을 먼저 적용하고 재측정한다. 그래도 실패하면 default 승격을 중단하고 memory budget 변경 여부를 별도 사용자 결정으로 남긴다.

### 9.4 physical acceptance

자동 submit/pixel proxy 뒤 실제 환경에서 확인한다.

- 좌/우/상/하/모서리 border drag와 빠른 왕복
- maximize/restore와 monitor 간 DPR 이동
- 60 Hz와 120 Hz 이상 display
- wheel mouse/precision trackpad fling 중 resize
- resize/raster stress 중 button, slider, keyboard, 한글 IME 조합/후보창/긴 Backspace
- 실제 screen reader focus/action/label
- Raster Worker crash recovery 직후 focus/IME와 화면 복구

단계적 추격, stretch, black band, input 무반응, IME owner 충돌이 보이면 FAIL이다. Playwright screenshot, `surface.flush()`, Worker `submitted` timestamp는 물리 scan-out PASS를 대신하지 않는다.

## 10. build, command, artifact

모든 test subprocess는 repository 규칙대로 20분 timeout을 실제 enforce한다. 기존 `Doroti/eng/run-web-playwright.ps1`의 owned-process, port ownership, readiness, cleanup 방식을 재사용한다.

계획된 canonical 순서:

1. `npm ci` in `Doroti/src/Doroti.Host.Web/Web`
2. CanvasKit bundle typecheck/build/hash validation
3. `dotnet build Doroti/src/Doroti.Graphics.DisplayList/Doroti.Graphics.DisplayList.csproj -c Release`
4. `dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release`
5. shared renderer와 Windows/Qt/MAUI Release build
6. DisplayList contract/fuzz/visual differential validation
7. `npm ci`와 `npm run check` in `Doroti/validation/web-playwright`
8. 기존 FCR-3/4/5/6/7과 resize-contract
9. `pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -HeadlessOnly -RendererMode worker-canvaskit-webgl`
10. CanvasKit topology/resource/context/restart 전용 headed validation
11. trimmed publish asset/provenance/negative gate
12. current direct vs CanvasKit split 3회와 physical acceptance 기록

보존할 artifact:

- exact package/lockfile와 third-party notice
- fingerprinted CanvasKit JS/WASM manifest와 SHA-256
- DisplayList schema/version/golden corpus
- browser/GPU/DPR/refresh가 포함된 raw timing sample
- context/restart/resource terminal trace
- visual baseline, diff, 승인 mask
- automatic PASS/FAIL, performance PASS/FAIL, physical `notVerified|PASS|FAIL` 표

## 11. 예상 변경 파일

| 영역 | 예상 파일/변경 |
|---|---|
| protocol | `Doroti/src/Doroti.Graphics.DisplayList/` 신규 프로젝트와 tests/validator |
| scene recording | `Doroti/src/Doroti.Ui/`의 Canvas/SceneBuilder/Paragraph/Image host payload 제거 |
| Web package | `Doroti/src/Doroti.Host.Web/Web/package.json`, `package-lock.json` |
| UI Worker | `doroti.ui.worker.ts`, CanvasKit layout bridge, managed UI runner/bootstrap |
| Raster Worker | `doroti.canvaskit.worker.ts`, decoder, CanvasKit renderer, resource registry |
| main/supervisor | `doroti.web.ts`, `doroti.web.worker-host.ts`, metrics/input/semantics/canvas lease |
| Web managed host | `BrowserFrameworkHost.cs`, `BrowserSkiaCapabilities.cs` 대체/삭제, host contracts |
| build/publish | `Doroti.Host.Web.csproj`, host-specific CanvasKit target, static web asset manifest |
| target package | `Doroti.Target.Web.browser-wasm.targets`, `doroti-target-manifest.json`, diagnostics contract |
| validation | display-list contract, CanvasKit visual golden, Web Playwright protocol/topology/recovery/resize tests |
| docs | Demo README의 renderer URL, support/fallback/asset provenance 설명 |

실제 파일명은 CK0/CK1 spike에서 확정하되 책임 경계를 합치지 않는다. generated `bin/`, `obj/`, publish output을 직접 편집하지 않는다.

## 12. hard stop, rollback, 완료 정의

### 12.1 hard stop

다음 중 하나면 broad refactor를 중단한다.

- Worker `OffscreenCanvas`에서 explicit CanvasKit WebGL2 on-screen surface를 software fallback 없이 만들 수 없음
- CPU-only CanvasKit paragraph가 UI Worker에서 DOM 없이 동작하지 않음
- 같은 version/font/recipe인데 UI와 Raster paragraph layout이 결정적으로 일치하지 않음
- npm bundle/WASM을 deterministic same-origin static web asset으로 publish할 수 없음
- context loss나 Raster Worker replacement 때 canvas/resource ownership을 정확히 회수할 수 없음
- current 필수 opcode 중 CanvasKit public API로 재현할 수 없는 기능이 있고 승인된 semantic 대체도 없음

### 12.2 rollback

- CK9 전: `auto`는 current default를 유지한다.
- CK9 후 한 release: query/config flag로 직전 renderer를 선택할 수 있게 한다.
- CanvasKit fatal/unsupported 자동 fallback은 최대 한 번이며 reason을 diagnostics에 남긴다.
- forced `worker-canvaskit-webgl`은 fallback하지 않고 실패를 노출한다.
- rollback이 UI state를 잃거나 반복 restart loop를 만들면 자동 재시도를 중지한다.

### 12.3 완료 정의

다음을 모두 만족해야 이 계획을 `complete`로 바꾼다.

- Web 제품 graph에서 SkiaSharp/native WebAssembly dependency 0
- UI .NET Worker와 CanvasKit Raster Worker의 분리 ownership 검증 PASS
- SAB/COOP/COEP 없이 cold start, render, resize, input, context recovery PASS
- current required scene/text/image/filter/effect visual differential PASS
- protocol/resource/terminal/restart automatic gate PASS
- performance qualification PASS
- physical acceptance 결과가 항목별로 기록됨
- build/publish provenance, license, rollback 문서화 완료

physical test가 실행되지 않았다면 그 항목은 `notVerified`이며 전체를 시각/입력 완료로 표현하지 않는다.

## 13. 참고 자료

Local source of truth:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- `Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts`
- `Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj`
- `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- `Doroti/src/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets`
- `Doroti/src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json`
- `Doroti/validation/web-playwright/`

Upstream:

- CanvasKit overview: <https://docs.skia.org/docs/user/modules/canvaskit/>
- CanvasKit npm package: <https://www.npmjs.com/package/canvaskit-wasm>
- CanvasKit WebGL implementation: <https://github.com/google/skia/blob/main/modules/canvaskit/webgl.js>
- `OffscreenCanvas`: <https://developer.mozilla.org/docs/Web/API/OffscreenCanvas>
- Transferable objects: <https://developer.mozilla.org/docs/Web/API/Web_Workers_API/Transferable_objects>

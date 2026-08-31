# Doroti Web CanvasKit Raster Worker 전환 계획 및 실행 결과

상태: `partial / qualificationIncomplete`

작성일: `2026-08-31`

대상: `Doroti.Host.Web`의 Chromium/WebGL2 제품 경로

문서 성격: 계획과 `2026-08-31` 실행 결과를 함께 기록한다. 구현된 experimental 경로와 자동 검증 결과는 qualification 완료를 뜻하지 않는다.

## 1. 결정

기존 `work2.md`의 여러 구현 후보와 선택 matrix는 폐기한다. Doroti Web의 목표 구조를 다음 하나로 고정한다.

- Browser main thread는 DOM, input, IME, semantics, canvas CSS geometry, `ResizeObserver`와 DPR 관찰만 소유한다.
- UI Worker는 단 하나의 .NET runtime, Doroti framework/widget tree, scheduler, scene build를 소유한다.
- Raster Worker는 .NET runtime 없이 `Doroti.Host.Web`이 npm에서 취득해 package에 포함한 CanvasKit과 visible `OffscreenCanvas`의 hardware WebGL2 context를 소유한다.
- UI Worker와 Raster Worker는 `MessageChannel` 위의 versioned binary DisplayList와 transferable resource buffer만 교환한다.
- `SharedArrayBuffer`, COOP/COEP, `WasmEnableThreads`, pthread를 요구하지 않는다.
- Web renderer에서는 최종적으로 `SkiaSharp`, `SkiaSharp.NativeAssets.WebAssembly`, `Doroti.Skia.Rendering` 의존성을 제거한다. Desktop/MAUI/Qt renderer의 SkiaSharp 사용은 이 계획의 제거 대상이 아니다.
- CanvasKit 객체는 Worker 사이에 공유하지 않는다. 각 owner 안에서만 생성·사용·폐기하고, Worker 경계에는 값, ID, byte buffer만 전달한다.

최종 renderer mode 이름은 `worker-canvaskit-webgl`로 한다. 기존 `document-webgl`, `offscreen-bitmap`, `offscreen-worker`, `worker-direct-webgl`은 전환 기간의 rollback/비교 기준으로만 유지하며, qualification 전에는 `auto` 기본값을 바꾸지 않는다.

## 2. 목표와 비목표

### 2.1 목표

1. raster stall, shader compile, image decode, GPU flush가 UI .NET event loop와 input dispatch를 막지 않게 한다.
2. SAB가 꺼져 있어도 동작하는 명시적 ownership/transfer 구조를 만든다.
3. Doroti SDK source의 Web build/pack은 exact-version npm dependency로 CanvasKit을 취득하고, 배포된 `Doroti.Host.Web` package에는 실행 asset을 포함해 앱 소비자의 `dotnet restore/build/publish`에는 Node/npm을 요구하지 않는다.
4. current Skia renderer와 동일한 scene, text, image, filter, runtime effect 결과를 CanvasKit renderer에서 재현한다.
5. resize 동안 logical CSS geometry와 physical backing geometry를 분리하고, stale/mismatched frame을 표시하지 않는다.
6. resource와 CanvasKit Embind object의 수명을 계측해 context loss와 Raster Worker 재시작 뒤에도 leak 없이 복구한다.
7. 자동 correctness, 정량 성능, 실제 사용자 시각/input/accessibility 판정을 분리해 기록한다.

### 2.2 비목표

- 두 .NET runtime 사이의 객체 공유 또는 runtime 내부 API를 이용한 managed heap 공유
- SAB를 켠 브라우저만 지원하는 fast path
- 배포된 Doroti package를 사용하는 앱의 restore/build/publish 중 Node/npm, bundler 또는 원격 CanvasKit download를 요구하는 구조
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

## 5. CanvasKit npm 취득과 `Doroti.Host.Web` 배포 계약

### 5.1 toolchain 경계와 upstream pin

- asset 전용 Razor Class Library/NuGet 프로젝트는 추가하지 않는다. CanvasKit 취득, 검증, Static Web Asset 등록과 pack은 `Doroti.Host.Web`이 소유한다.
- 첫 upstream pin은 `canvaskit-wasm@0.42.0`으로 고정한다. `Doroti/src/Doroti.Host.Web/Web/package.json`에는 caret/tilde 없는 exact `devDependency`를 기록하고 `package-lock.json`을 commit한다.
- Doroti SDK repository에서 `Doroti.Host.Web`을 source/project-reference로 build 또는 pack하려면 지원되는 Node와 npm이 설치되어 있어야 한다. canonical restore는 `npm ci --ignore-scripts` 하나이며 `npm install`, global package, floating `latest`와 다른 package manager는 사용하지 않는다.
- `Doroti.Host.Web` nupkg은 필요한 CanvasKit runtime asset, type declaration, license와 provenance를 포함한다. 이 nupkg을 사용하는 외부 앱의 `dotnet restore/build/publish`는 Node/npm을 실행하거나 npm registry에 접근하지 않는다.
- 기본 variant의 `canvaskit.js`, `canvaskit.wasm`, `types/index.d.ts`, `LICENSE`만 source allowlist로 사용한다. `full`/`profiling` variant와 npm package의 나머지 파일은 publish하지 않는다.
- CK0에서 필수 public API가 기본 variant에 없다고 증명된 경우에만 `full` variant로 명시적으로 전환하고 size/startup/memory gate를 다시 측정한다. 두 variant를 함께 배포하지 않는다.
- SDK source build에서 npm registry access는 exact lockfile restore에만 허용한다. browser runtime, 배포된 앱 build, CDN, unversioned public URL에서 CanvasKit을 받는 것은 금지한다.

version 변경은 단순 patch update로 취급하지 않는다. lockfile integrity, DisplayList golden, text metrics, shader/filter, context recovery, package/publish hash gate를 다시 통과한 뒤 pin과 provenance를 함께 갱신한다.

### 5.2 SDK source restore와 allowlist 추출

CanvasKit build target은 `Doroti.Host.Web`에 한정하며 다음 순서를 소유한다.

1. source/project-reference build와 pack 시작 전에 Node/npm availability와 지원 version을 fail-fast로 확인한다.
2. `package-lock.json`이 바뀌었거나 lock-hash stamp/필수 `node_modules/canvaskit-wasm` 파일이 없을 때만 `npm ci --ignore-scripts`를 실행한다. clean/cold SDK build는 npm registry access를 허용하고 incremental build는 lock-hash stamp를 재사용한다.
3. lockfile의 `canvaskit-wasm` version과 registry SHA-512 integrity가 승인 pin과 일치하는지 검증한다.
4. `node_modules/canvaskit-wasm`에서 allowlist 파일만 target-specific `obj` 아래로 복사한다. `node_modules`, npm cache, upstream JS/WASM을 source `wwwroot`나 nupkg에 통째로 넣지 않는다.
5. copied file마다 byte length와 SHA-256을 계산하고 version, variant, lockfile integrity와 함께 generated provenance manifest에 기록한다.
6. missing license/asset, version 또는 integrity mismatch, 예상 밖 variant, source tree로의 생성물 write에는 fail closed한다.

generic `Doroti.TypeScript.targets`에는 npm restore나 CanvasKit 전용 동작을 넣지 않는다. Host-specific target은 package-consumer의 build graph로 전파하지 않으며, 외부 nupkg 소비 경로가 Node/npm을 호출하면 gate 실패다.

### 5.3 Static Web Asset와 package layout

allowlist asset은 build가 `Doroti.Host.Web`의 target-specific `obj`로 복사한 뒤 Razor SDK Static Web Asset으로 등록한다. upstream JS와 WASM은 수정하거나 rebundle하지 않는다.

정상 endpoint의 logical base는 `_content/Doroti.Host.Web/canvaskit/0.42.0/`으로 고정하고 실제 URL은 Static Web Asset fingerprint manifest로 resolve한다. UI Worker와 Raster Worker는 같은 manifest에서 다음을 받는다.

- `canvasKitJsUrl`
- `canvasKitWasmUrl`
- upstream version/variant
- lockfile integrity와 JS/WASM byte length/SHA-256

`dotnet pack Doroti.Host.Web` 결과는 정확히 한 variant의 runtime asset, type declaration, license, provenance와 consumer-side asset validation을 포함한다. source/project-reference build와 clean external nupkg-consumer build가 같은 endpoint/hash를 내야 한다. trimmed publish에는 CanvasKit JS/WASM이 정확히 한 벌만 있고 SkiaSharp native WebAssembly, `node_modules`, allowlist 밖의 npm package 파일과 CDN reference가 없어야 한다.

### 5.4 bundler 없는 Worker bootstrap

upstream `canvaskit.js`는 classic/UMD Emscripten script이므로 수정하거나 ESM bundle로 다시 만들지 않는다. 기존 `Microsoft.TypeScript.MSBuild`로 별도의 `doroti.canvaskit.bootstrap.ts`를 classic Worker script로 compile한다.

1. main이 classic Worker를 만들고 fingerprinted CanvasKit JS/WASM URL, role별 Doroti ESM module URL, session과 transferable canvas/port lease를 단일 init envelope로 보낸다.
2. bootstrap은 URL이 same-origin이고 build가 승인한 logical manifest endpoint인지 확인한다. 실제 byte hash는 asset build/publish gate가 검증하며 runtime에서 JS를 다시 fetch/eval하지 않는다.
3. `importScripts(canvasKitJsUrl)`로 upstream script를 한 번 load하고 `CanvasKitInit`을 bootstrap-owned binding으로 고정한다.
4. `const role = await import(roleModuleUrl)`로 `doroti.ui.worker.js` 또는 `doroti.canvaskit.worker.js`를 동적 load한다.
5. bootstrap이 `role.startCanvasKitRole({ CanvasKitInit, canvasKitWasmUrl, initEnvelope })`를 정확히 한 번 호출해 보관한 transferable lease를 넘긴다.
6. role module은 동일 `canvasKitWasmUrl`을 `locateFile`에 제공하고 CanvasKit/.NET/resource 준비가 끝난 뒤에만 `ready`를 보낸다.

Blob/data URL, `eval`, `new Function`, vendor source suffix patch를 사용하지 않는다. classic bootstrap + dynamic ESM import가 target Chromium과 .NET Worker bootstrap에서 동작하는지는 CK0 hard gate로 먼저 증명한다. TypeScript typecheck에는 npm restore된 `canvaskit-wasm/types/index.d.ts`를 사용하되 runtime bare import는 만들지 않는다.

### 5.5 fail-closed GPU 생성

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
- exact `canvaskit-wasm@0.42.0` `package.json`/lockfile, SDK source `npm ci --ignore-scripts`, allowlist extraction과 provenance manifest spike
- missing Node/npm, lockfile/version/integrity mismatch, missing/extra variant, source-tree write negative fixture
- `Doroti.Host.Web` pack과 clean external nupkg-consumer의 zero-npm/zero-registry consume spike
- classic Worker bootstrap의 `importScripts(CanvasKit) → import(Doroti ESM role)` smoke
- Raster Worker에서 transferred visible OffscreenCanvas + explicit hardware WebGL2 + `MakeOnScreenGLSurface` + `flush()` smoke
- UI Worker에서 DOM/WebGL 없이 CanvasKit font/paragraph layout smoke
- 한 CanvasKit package/WASM URL을 두 Worker에서 독립 초기화하고 startup/heap 비용 측정
- context loss/restore 10회, Raster Worker replacement 3회, canvas replacement smoke

CK0 PASS 조건:

- SDK source/project-reference build의 CanvasKit 취득은 exact lockfile의 `npm ci --ignore-scripts`만 사용하고 bundler invocation 0
- clean external nupkg-consumer의 `dotnet restore/build/publish` 중 Node/npm invocation과 CanvasKit registry/CDN request 0
- source/project-reference build, packed nupkg consumer, trimmed publish가 모두 같은 fingerprinted JS/WASM을 same-origin에서 load
- SDK source cold restore 외의 npm registry request 0, lockfile integrity/hash mismatch 허용 0
- main/UI의 visible canvas `getContext` call 0, Raster owner만 WebGL2 context 1
- software/Canvas2D fallback 0
- UI text recipe와 Raster 재-layout의 line/metrics hash mismatch 0
- context/restart run의 missing/duplicate canvas/resource terminal 0
- private CanvasKit/Skia symbol patch, vendor JS 수정, generated bundle 사후 monkey patch 0

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
- `Doroti.Host.Web`의 host-specific npm restore/allowlist extraction, Static Web Asset endpoint, pack/provenance gate 연결
- Web host capability를 CanvasKit text/resource adapter와 protocol client로 교체
- `Doroti.Target.Web.browser-wasm.targets`의 CanvasKit 금지 `DOROTIWEB007`을 제거하고 required/pinned CanvasKit asset 검증으로 대체
- target manifest의 backend/runtime/ownership 정보를 새 topology로 갱신
- publish에 SkiaSharp WebAssembly native asset이 남으면 실패하는 negative gate 추가

PASS 조건:

- Web restore/build/publish graph에 SkiaSharp package/native asset 0
- SDK source Web build/pack graph의 Node/npm invocation은 exact CanvasKit restore에만 한정되고 bundler invocation 0
- clean external nupkg-consumer restore/build/publish graph에 Node/npm/bundler invocation 0
- project-reference와 packed nupkg consumer의 CanvasKit version/variant/JS/WASM hash/endpoint 차이 0
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

1. `npm ci --ignore-scripts` in `Doroti/src/Doroti.Host.Web/Web`
2. lockfile version/integrity, CanvasKit allowlist, generated provenance/per-file hash validation
3. `dotnet build Doroti/src/Doroti.Graphics.DisplayList/Doroti.Graphics.DisplayList.csproj -c Release`
4. `dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release`
5. `dotnet pack Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release`
6. clean external nupkg-consumer fixture의 restore/build/publish, zero-npm과 asset endpoint/hash 검사
7. shared renderer와 Windows/Qt/MAUI Release build
8. DisplayList contract/fuzz/visual differential validation
9. `npm ci`와 `npm run check` in `Doroti/validation/web-playwright`—별도 validation toolchain
10. 기존 FCR-3/4/5/6/7과 resize-contract
11. `pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -HeadlessOnly -RendererMode worker-canvaskit-webgl`
12. CanvasKit topology/resource/context/restart 전용 headed validation
13. trimmed publish asset/provenance/negative gate
14. current direct vs CanvasKit split 3회와 physical acceptance 기록

보존할 artifact:

- exact `package.json`/`package-lock.json`과 registry SHA-512 integrity
- `Doroti.Host.Web` nupkg, generated CanvasKit provenance, third-party notice와 package-content manifest
- fingerprinted CanvasKit JS/WASM endpoint manifest와 SHA-256
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
| CanvasKit npm acquisition | `Doroti/src/Doroti.Host.Web/Web/package.json`, `package-lock.json`, exact `devDependency`, source-build lock/integrity validator |
| CanvasKit asset pipeline | `Doroti.Host.Web` host-specific npm/allowlist/provenance/Static Web Asset targets와 package-consumer zero-npm validator |
| Worker bootstrap | `doroti.canvaskit.bootstrap.ts`, classic bootstrap tsconfig/target, asset URL contract |
| UI Worker | `doroti.ui.worker.ts`, CanvasKit layout bridge, managed UI runner |
| Raster Worker | `doroti.canvaskit.worker.ts`, decoder, CanvasKit renderer, resource registry |
| main/supervisor | `doroti.web.ts`, `doroti.web.worker-host.ts`, metrics/input/semantics/canvas lease |
| Web managed host | `BrowserFrameworkHost.cs`, `BrowserSkiaCapabilities.cs` 대체/삭제, host contracts |
| build/publish | `Doroti.Host.Web.csproj`, target-specific `obj` asset copy, nupkg content와 static web asset endpoint manifest |
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
- SDK source에서 exact lockfile의 npm asset을 deterministic하게 검증·pack할 수 없거나 packed nupkg 소비가 다시 Node/npm을 요구함
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
- SDK source/project-reference Web build와 pack은 exact lockfile의 `npm ci --ignore-scripts`만 사용하고 bundler invocation 0
- clean external nupkg-consumer의 `dotnet restore/build/publish`에서 Node/npm/bundler invocation과 CanvasKit registry/CDN request 0
- source/project-reference와 nupkg-consumer의 CanvasKit version/variant/hash/endpoint identity PASS
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
- CanvasKit upstream package/provenance: <https://www.npmjs.com/package/canvaskit-wasm>
- npm clean install: <https://docs.npmjs.com/cli/commands/npm-ci>
- CanvasKit WebGL implementation: <https://github.com/google/skia/blob/main/modules/canvaskit/webgl.js>
- `OffscreenCanvas`: <https://developer.mozilla.org/docs/Web/API/OffscreenCanvas>
- Transferable objects: <https://developer.mozilla.org/docs/Web/API/Web_Workers_API/Transferable_objects>

## 14. 2026-08-31 실행 결과

### 14.1 총평과 단계 진행 예외

이 실행의 전체 판정은 `PARTIAL / notQualified`다. `worker-canvaskit-webgl` experimental opt-in 경로와 자동 검증 기반은 구현했지만 12.3절의 완료 정의를 만족하지 못했다. 따라서 `auto`는 계속 `document-webgl`이며 forced CanvasKit mode의 fail-closed 정책도 유지한다.

8절은 앞 gate가 PASS일 때만 다음 단계로 진행하도록 계획했지만, 이번 실행에서는 CK0/CK1이 `PARTIAL`인 상태에서 topology와 package feasibility를 실제로 확인하기 위해 후속 experimental implementation까지 진행했다. 이는 계획 이탈이며 CK0/CK1을 PASS로 재분류하지 않는다. 미완료 gate와 재개 조건은 아래 표에 그대로 남긴다.

### 14.2 구현된 범위

- `canvaskit-wasm@0.42.0` default variant를 exact lockfile dependency로 고정하고 Host.Web source build의 allowlist, SHA-256 provenance, Static Web Asset, nupkg/buildTransitive 검증 경로를 추가했다.
- `Doroti.Graphics.DisplayList` v2 little-endian typed schema, encoder/decoder/validator, resource/string table, deterministic golden과 browser-side bounds/opcode 검증을 추가했다. v2는 14.6절의 paragraph text-run recipe를 포함한다.
- main/UI/Raster를 분리하고 UI↔Raster 전용 `MessageChannel`, current+latest mailbox, transferable buffer pool, scene receipt/terminal ledger, resource journal/replay와 canvas lease lifecycle을 구현했다.
- UI Worker에는 CPU-only CanvasKit paragraph layout/metrics hash 서비스를, Raster Worker에는 hardware WebGL2 CanvasKit renderer와 명시적 Embind object accounting을 추가했다.
- Raster renderer는 v2 opcode를 fail-closed로 decode하고 basic geometry, paint/path/image/paragraph, layer/filter/shader와 top-level direct two-pass runtime-effect image filter를 replay한다.
- resize는 logical CSS geometry와 physical backing을 분리하고, exact scene을 같은 GrContext의 GPU staging surface에 먼저 완성한 뒤 visible backing을 교체·복사·flush하는 commit transaction으로 바꿨다.
- scene 단위 paragraph font collection 공유, 최대 8-slot GPU image-filter surface pool, 마지막 Raster failure reason 진단을 추가했다.
- stale Raster session의 message/error를 무시하고, Worker replacement마다 새 visible canvas lease를 정확히 한 번 transfer/retire하며 resource journal을 replay한다.
- CanvasKit mode 전용 topology, stall, DPR2, replacement, malformed-protocol, DisplayList와 기존 resize/input 회귀를 canonical Playwright wrapper에 연결했다.

### 14.3 CK0–CK9 판정

| Gate | 판정 | 확인한 범위 | 남은 조건 또는 실패 원인 |
|---|---|---|---|
| CK0 | `PARTIAL` | exact 0.42.0/default pin, same-origin asset, hardware Raster-only WebGL owner, 7/7 pack, clean packed consumer zero-Node/npm, 3회 replacement를 확인했다. | cold source `npm ci` 재실행, current renderer baseline/startup/heap corpus, context-loss 10회, 전체 negative input matrix가 없다. |
| CK1 | `PARTIAL` | typed DisplayList, 6330-byte v2 managed golden, browser 2/2 계약, malformed recovery가 PASS했다. | `SceneCommand`/`PathCommand`의 `object? HostPayload`와 mapper 변환이 남고, same-worker Skia loopback 및 old-direct pixel golden이 없다. |
| CK2 | `PARTIAL` | UI-owned CanvasKit text service, paragraph metrics hash, logical resource registry/journal과 restart replay를 구현했다. | Latin/한글/surrogate/combining/RTL/wrap/ellipsis/hit-test의 current-Skia differential과 stale-handle fixture가 없고 pre-wire HostPayload가 image/paragraph object를 보유한다. |
| CK3 | `PASS` | main/UI/Raster .NET `0/1/0`, UI/Raster CanvasKit `1/1`, Raster WebGL owner `1`, direct MessageChannel, 100 ms Raster stall 중 UI/input 진행, HWM `<=2`, exact terminal/receipt를 자동 검증했다. | CK3 자체 gate의 남은 자동 blocker는 없다. |
| CK4 | `PARTIAL` | v2 opcode 선언/검증과 representative demo render, no-blank 자동 sample, top-level two-pass runtime filter가 동작한다. | current-Skia↔CanvasKit strict pixel differential/all-opcode raster corpus가 없다. composed/nested runtime-effect image-filter tag 4는 계속 `DOROTIWEB032` fail-closed이며 retained filter output의 stable identity/cache가 없다. |
| CK5 | `FAIL` | Raster Worker 3회 replacement, resource replay, canvas lease terminal, malformed recovery와 context loss/restore 1회는 PASS했다. | `rasterRestartBudget=3`을 context recovery도 소비하므로 동일 session 10회 context loss/restore gate를 구조적으로 통과할 수 없다. shutdown zero-lease/resource 및 focus/editing state 보존도 미검증이다. |
| CK6 | `PARTIAL` | continuous resize/wheel no-blank, viewport A-B-C, pinch zoom, startup, DPR2 logical 1080×720/physical 2160×1440/`transform:none`을 자동 검증했다. | DPR 1.25/1.5, native border/maximize/restore, fast/long resize 3회 정량 gate와 direct 대비 비교가 없다. |
| CK7 | `FAIL` | Host-specific asset/provenance, nupkg/buildTransitive, clean consumer, tamper/missing-license negative gate는 PASS했다. | Host.Web/nuspec/publish에 `SkiaSharp`, `SkiaSharp.NativeAssets.WebAssembly`, `Doroti.Skia.Rendering`, `Doroti.Skia.RuntimeEffects`가 남는다. Web dependency 0 조건은 FAIL이다. |
| CK8 | `PARTIAL` | transfer buffer pool, resource journal, scene-scoped font collection 공유, bounded 8-slot GPU filter-surface pool과 object counters를 구현했다. | Paint/Path/Paragraph/Image bounded cache, shader warmup/negative cache, filter `CacheKey/CacheGeneration` wire 전달, 비교 성능, 30분 memory churn이 없다. |
| CK9 | `notVerified` | rollback mode를 유지하고 forced CanvasKit은 silent fallback하지 않는다. | CK5/CK7이 FAIL이고 전체 correctness/performance/physical acceptance가 끝나지 않아 기본값을 승격하지 않는다. |

### 14.4 자동 검증 결과

#### Contract, build, platform regression

| 명령 또는 범위 | 결과 |
|---|---|
| `dotnet run --project Doroti/validation/display-list-contract/Doroti.Validation.DisplayListContract.csproj -c Release --no-launch-profile` | `PASS`, 6330 bytes, SHA-256 `66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42` |
| `npm run check` in `Doroti/validation/web-playwright` | `PASS`, TypeScript error 0 |
| Release Web wrapper build (`DorotiDemoApp.Web`와 Host/Target dependency 포함) | `PASS`, warning 0, error 0 |
| `Doroti.Target.Web.browser-wasm` Release build | `PASS`, warning 0, error 0 |
| Qt, MAUI, Windows host Release regression builds | `PASS`, warning 0, error 0 |
| FCR-3/4/5/6/7와 resize-contract | `PASS` |
| `git diff --check` | `PASS` |

DisplayList browser matrix는 다음 명령에서 `2/2 PASS`했다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 `
  -Configuration Release `
  -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile tests/canvaskit-display-list.spec.ts
```

CanvasKit topology/lifecycle 전용 suite는 다음 명령에서 `5/5 PASS`했다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 `
  -Configuration Release `
  -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile tests/canvaskit-worker.spec.ts
```

동일 mode의 전체 headless suite는 `16 PASS / 다른 renderer 전용 5 SKIP / 0 FAIL`, 총 `3.8분`이었다. 이전 full run의 6개 실패였던 continuous-resize blank, context-loss 관찰, warm input latency, offscreen capability schema, resize trace backing identity, wheel final commit을 각각 runtime 또는 mode-aware contract에서 수정한 뒤 재실행한 결과다.

주요 자동 수치는 다음과 같다.

- hardware WebGL2: Google/AMD ANGLE Radeon 780M D3D11, software fallback `false`
- continuous flicker: canvas sample 221개, resize 81회, target/front generation `82/82`, scene `254/254`, failed `0`, transfer buffer `257/257`, outstanding `0`
- wheel: 60/60 commit, p95 `53.1 ms`, max `66.6 ms`
- input-front: cold `248.4 ms`, warm `234.9 ms`
- context restore 1회: context generation `2`, Raster session `2`, resource replay `3`, exact front 복구
- Raster replacement 3회: restart/budget `3/3`, final session `4`, retired lease 3개 각각 terminal `1`, resource replay `9`, outstanding buffer `0`
- DPR2: CSS `1080×720`, backing `2160×1440`, CSS transform 없음
- steady GPU owners: `GrDirectContext live=1`, visible `Surface live=1`; bounded `ImageFilterSurfacePool live=1`

이 수치는 자동 submit/DOM/screenshot/counter evidence이며 physical scan-out이나 아래 9.3 performance qualification의 3회 기준을 대신하지 않는다.

#### Pack, clean consumer, provenance

다음 7개 `0.2.0-beta` package를 dependency 순서로 Release pack했고 모두 exit 0이었다.

- `Doroti.Graphics.DisplayList`
- `Doroti.Runtime`
- `Doroti.Ui`
- `Doroti.Skia.RuntimeEffects`
- `Doroti.Skia.Rendering`
- `Doroti.Hosting`
- `Doroti.Host.Web`

새 `final-app`과 새 isolated NuGet cache를 사용하고 `PATH` 앞에 실패용 `node.cmd`/`npm.cmd`를 둔 뒤 다음을 실행했다.

```powershell
dotnet restore CanvasKitConsumer.csproj --configfile NuGet.Config --force --no-http-cache --nologo
dotnet build CanvasKitConsumer.csproj -c Release --no-restore --disable-build-servers --nologo
dotnet publish CanvasKitConsumer.csproj -c Release --no-restore --disable-build-servers --nologo `
  -o C:/Users/parti/Labo/DorotiLab/Doroti/src/Doroti.Host.Web/obj/CanvasKitConsumer/final-app/publish
```

결과는 restore `17.0 s`, build `4.7 s`(warning/error 0), publish `19.1 s`였고 7개 Doroti dependency가 모두 `type=package`였다. poison invocation log는 생성되지 않아 consumer의 Node/npm invocation은 0이다. 처음 custom intermediate path를 app 내부에 둔 시도는 기존 generated source가 Compile glob에 포함되어 `CS0579` 8건으로 실패했지만, 이는 package/product 결함이 아닌 fixture 배치 오류였으며 새 sibling `final-app` 재실행으로 판정을 분리했다.

| CanvasKit 0.42.0 default asset | Bytes | SHA-256 |
|---|---:|---|
| `canvaskit.js` | 120,877 | `443777592179808354031cf411d8d43cac9f6b98d1227123c5c22d401b0fbf7f` |
| `canvaskit.wasm` | 7,317,345 | `25ebed8e60158c5854f8dc807b936daca21354f8bfb6a2231266b0a93812f301` |
| `types/index.d.ts` | 174,139 | `a7017a8fd21f27fdd1afe8036841d7ce0979b2410954fcc6d3f80444c171a6c6` |
| `LICENSE` | 1,635 | `d27678cba0d529e77201e2d2a053628143e986aad8f1e77f7039ad4366c8f978` |
| `canvaskit.manifest.json` | 1,460 | `b3d6627d4a08862aacd62155f5d8c87b9e4b75ad2ad09b030e71b2230061813f` |

다섯 asset과 생성된 Doroti JS 13개는 source output, `Doroti.Host.Web.0.2.0-beta.nupkg`, consumer publish에서 byte/SHA가 모두 같았다. nupkg의 CanvasKit 항목은 위 다섯 개뿐이며 `node_modules`는 0개다. Host nupkg SHA-256은 `e64e3abb844faa7825cdaafb87256cd906bf429009db848ed2ba0cdb2dc9d584`다. `buildTransitive/Doroti.Host.Web.targets`는 `<Exec>`, Node, npm 호출 없이 `DOROTICK101/102` 검사를 포함하며 SHA-256은 `9d25beefeda2eaf54404215b70b1af9d2125a326d837cac0abe029233b7bf39e`다.

실제 변조 package negative gate도 실행했다.

- `canvaskit.js` 1-byte 변조: restore PASS 후 build exit 1, `DOROTICK102`
- `LICENSE` 제거: restore PASS 후 build exit 1, `DOROTICK101`
- 두 경우 모두 Node/npm invocation 0

단, 이번 source pack은 warm npm state에서 `CanvasKit npm restore is current`를 확인한 실행이다. source cold `npm ci --ignore-scripts` 재실행과 registry/network capture는 별도 미검증이다. Consumer는 clean Web SDK publish이며 trimmed browser-wasm publish는 아니다.

### 14.5 명시적으로 남은 검증과 재개 조건

- CK5를 재개하려면 context recovery를 fatal/crash 3회 budget과 분리하거나 same-worker GrContext/surface recovery로 바꾸고 동일 session 10회를 자동 검증해야 한다.
- CK7을 재개하려면 CanvasKit qualification과 rollback 결정을 거친 뒤 Web graph/nuspec/publish에서 SkiaSharp/native/Doroti.Skia dependency를 제거하고 trimmed publish를 다시 검사해야 한다. 현재 publish에는 SkiaSharp 관련 artifact 8개가 남는다.
- CK1/2/4를 재개하려면 pre-wire `HostPayload`를 제거하고 same scene bytes를 current Skia와 CanvasKit에 raster한 strict pixel/text/filter differential corpus를 추가해야 한다.
- composed runtime-effect filter와 retained filter identity/cache를 구현하고 filter/shader/paragraph churn 뒤 bounded eviction과 baseline recovery를 검증해야 한다.
- source cold npm acquisition, tampered lock/integrity/extra variant, CSP와 runtime asset/network negative fixture를 추가해야 한다.
- direct 대비 startup/resize/handoff/long-task/memory를 같은 session에서 3회 비교하고 30분 churn을 실행해야 한다.
- 실제 좌우상하/모서리 border drag, maximize/restore, monitor DPR 이동, 60/120 Hz, precision trackpad, 한글 IME/긴 Backspace, screen reader, crash 직후 focus/editing과 compositor scan-out은 모두 `notVerified`다.
- Firefox와 Safari는 제품 지원으로 승격하지 않았다.

위 조건이 끝날 때까지 12.3절 완료 정의는 `FAIL`, 전체 상태는 `PARTIAL / notQualified`, 기본값은 `auto=document-webgl`이다.

### 14.6 2026-08-31 TextField 공백 및 공통 text-run 후속 수정

`worker-canvaskit-webgl` TextField에서 `WW WW`의 공백 뒤 단어가 보이지 않는 결함을 재현했다. editing value와 semantics에는 UTF-16 전체 `[0,5)`가 남았지만, UI CanvasKit service가 unconstrained layout의 `maxIntrinsicWidth=123.0885...`를 f32 `123.0885...` 그대로 finite relayout 폭으로 사용하면서 SkParagraph가 `[0,3)`만 첫 줄로 남기고 `maxLines=1`에 의해 뒤 단어를 잘랐다.

Flutter의 `TextPainter`/CanvasKit 구현처럼 먼저 unconstrained layout을 수행하도록 바꾸고, Doroti의 UI↔Raster finite f32 recipe 제약을 위해 intrinsic width를 logical pixel 위로 올린 뒤 원래 line range/hard-break/`didExceedMaxLines`와 같음을 bounded retry로 검증한다. NanumGothic 28px fixture에서 `WW WW`는 layout width `124`, line `[0,5)`, `didExceedMaxLines=false`로 확인했다.

공통 `dart:ui` text 구성도 마지막 스타일 하나로 평탄화하지 않는다. `TextStyle`의 family fallback, weight/slant, spacing/height/locale, background/decoration, shadow, font feature/variation을 보존하고, `ParagraphBuilder.pushStyle/pop/addText`가 Flutter와 같은 합성 style stack으로 immutable text run을 만든다. UI CanvasKit 측정과 Raster CanvasKit 재구성은 같은 normalized run list를 사용한다. 이 wire 변경 때문에 DisplayList schema는 v2로 올렸고 C#, main-thread TypeScript validator, Raster Worker decoder와 golden을 함께 갱신했다.

후속 검증 결과는 다음과 같다.

- Release `DorotiDemoApp.Web` build: `PASS`, warning/error 0
- DisplayList v2 managed contract: `PASS`, 6330 bytes, SHA-256 `66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42`
- DisplayList v2 browser contract: `2/2 PASS`
- bundled Chromium headed TextField regression: `1/1 PASS`; semantics value `WW WW`, editable-line raster right edge `WW: 97`, `WW WW: 175`로 공백 뒤 단어의 실제 raster를 확인했다.

이 후속 수정은 해당 TextField 결함과 text-run round trip에 대한 자동 판정만 `PASS`로 바꾼다. 14.5절의 실제 한글 IME, 물리 입력, screen reader, 전체 current-Skia differential과 제품 qualification은 계속 `notVerified` 또는 기존 판정을 유지한다.

### 14.7 2026-08-31 CanvasKit TextField 캐럿 Y 좌표 후속 수정

`worker-canvaskit-webgl`만 TextField 캐럿이 기준 renderer보다 약간 위에 표시되는 결함을 확인했다. 공용 `TextPainter`는 캐럿 위치 계산에 `BoxHeightStyle.strut`을 요청하지만, CanvasKit host paragraph adapter는 height style을 무시하고 항상 `getGlyphInfoAt(...).graphemeLayoutBounds`의 tight bounds를 반환했다. NanumGothic 28px fixture에서 tight bounds는 `top=-1.99, bottom=26.01`, CanvasKit `RectHeightStyle.Strut` bounds는 `top=0, bottom=28`이어서 캐럿 시작점이 약 2 logical px 위로 이동했다.

UI CanvasKit text service가 grapheme별 strut top/bottom을 별도로 snapshot에 포함하고, Raster CanvasKit도 동일 값을 metrics hash에 포함해 UI/Raster paragraph identity를 계속 검증하도록 수정했다. 공용 `Paragraph.getBoxesForRange`는 `BoxHeightStyle.strut` 요청에만 strut bounds를 사용하고 ordinary tight selection/hit-test geometry는 기존 tight bounds를 유지한다.

후속 검증 결과는 다음과 같다.

- `npm run check`: `PASS`
- FCR-7 material/widget runtime contract: `PASS`; tight `[-2,26]`과 strut `[0,28]` 선택을 분리 검증
- Release `DorotiDemoApp.Web` build: `PASS`, warning/error 0
- bundled Chromium headed TextField pixel regression: `worker-canvaskit-webgl` `1/1 PASS`, 기준 `document-webgl` `1/1 PASS`; 두 mode 모두 focused caret top이 TextField semantics bounds에서 `12 logical px`
- CanvasKit topology/lifecycle suite: `5/5 PASS`; UI/Raster metrics hash, stall, 3회 replacement, malformed recovery, DPR2 포함
- `git diff --check`: `PASS`

이 판정은 자동 렌더/DOM/픽셀/counter 범위다. 수정 후 실제 한글 IME 입력과 사용자 시각 acceptance는 아직 `notVerified`이며, 14.5절의 전체 qualification 상태를 변경하지 않는다.

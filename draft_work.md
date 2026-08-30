# Doroti Web Engine v2: 단일 Worker-direct WebGL 엔진 개편 계획

- 작성일: 2026-08-30
- 상태: `ready` — 기존 3경로의 상호작용 불능 수정은 마감했고, v2 구조 개편은 아직 시작하지 않음
- 현재 제품 기본값: `document-webgl`
- v2 목표: `document-webgl`, `offscreen-bitmap`, `offscreen-worker`를 폐기하고 하나의 `worker-direct-webgl` 엔진만 유지한다.
- 최종 품질 기준: Flutter Web과 같은 장비·Chrome·창 조절 시나리오에서 resize/input frame latency와 시각적 연속성을 직접 비교한다.

## 1. 이번 작업 마감

### 1.1 확인한 원인

`offscreen-bitmap`과 `offscreen-worker`가 스크롤·클릭 뒤 반응하지 않던 직접 원인은 visible front admission이 resize generation만 비교한 데 있다.

- 기존 조건은 `new generation > front generation`이었다.
- 클릭, wheel, animation은 viewport 크기를 바꾸지 않으므로 새 scene도 같은 generation을 가진다.
- raster와 input dispatch는 성공해도 완성된 `ImageBitmap`이 표시 단계에서 폐기됐다.
- framework는 terminal을 받았지만 사용자는 이전 화면을 계속 보게 됐다.

### 1.2 반영한 수정

- 두 ImageBitmap 경로의 front admission을 `(requestId 단조 증가) + (generation 비감소) + exact size`로 바꿨다.
- presenter diagnostics에 `frontRequestId`를 추가했다.
- persistent Worker의 display receipt를 최대 2개까지 겹칠 수 있게 해 다음 raster가 매번 main-thread ACK 뒤에 직렬화되지 않도록 했다.
- managed `SkiaPaintCompletion`도 bounded receipt가 도착할 때까지 request별로 보존한다.
- context loss 시 모든 outstanding Worker receipt를 한 번씩 닫는다.
- input 회귀는 접근성 label뿐 아니라 같은 resize generation에서 `frontRequestId`가 실제 증가하는지 검사한다.

### 1.3 최종 자동 결과

| 항목 | 결과 |
|---|---|
| Release Web build | PASS, warning 0 / error 0 |
| TypeScript check | PASS |
| FCR-3 scheduler | PASS |
| FCR-7 Material/widget | PASS |
| resize-contract v4 | PASS, generated/terminal 22/22, max depth 2, stale present 0 |
| `offscreen-bitmap` 전체 headless | 7 PASS, Worker 전용 1 SKIP |
| `offscreen-worker` 전체 headless | 8 PASS, crash/restart 포함 |
| bitmap wheel 단독 표본 | PASS, p95 43.9ms |
| Worker wheel 단독 표본 | PASS, p95 34.5ms |
| bitmap input→front 상한 표본 | first 228.2ms, warm 115.3ms |
| Worker input→front 상한 표본 | first 281.8ms, warm 129.0ms |
| Worker 181-step resize | PASS, target→front p50 37.6ms / p95 63.8ms / max 120.6ms |

input 수치는 Playwright의 semantics/presenter polling까지 포함한 상한이며 compositor scan-out 시각은 아니다. 전체 suite 안의 wheel 표본은 bitmap p95 50.1ms, Worker p95 53.2ms로 run-to-run 편차가 있었다.

실제 사용자가 확인한 Flutter 대비 resize 추격감과 첫 상호작용 체감은 여전히 미달이다. 실제 마우스 창 테두리 drag, trackpad, 120Hz 이상 display, browser compositor scan-out은 `notVerified`다. 따라서 이번 수정은 기능 회복으로 마감하되 Web 성능 완료로 판정하지 않는다.

## 2. 왜 부분 최적화를 종료하는가

현재 세 경로는 같은 frame을 서로 다른 소유권과 terminal로 이동한다.

| 경로 | .NET/Skia 위치 | 실제 표시 | 추가 경계 |
|---|---|---|---|
| `document-webgl` | main thread | document canvas WebGL2 | main rAF 안에서 layout+raster가 DOM/input과 경쟁 |
| `offscreen-bitmap` | main thread | `bitmaprenderer` | `createImageBitmap` 비동기 capture |
| `offscreen-worker` | Worker | `bitmaprenderer` | metrics/input 왕복 + bitmap transfer + display receipt |

공통 계약을 고칠 때마다 `CanvasPresenter`, `WorkerPresenter`, `WorkerDisplayPresenter` 세 구현을 같이 바꿔야 한다. 같은 크기의 interaction frame, context loss, resize generation, resource terminal이 각기 다른 조건을 거쳐 회귀가 반복된다.

특히 Worker 경로는 Skia가 이미 Worker에서 완성한 화면을 다시 `ImageBitmap`으로 복사해 main의 `bitmaprenderer`에 넘긴다. 이 경계는 다음 비용을 구조적으로 남긴다.

1. `createImageBitmap` capture
2. transferable message
3. main task queue 대기
4. `transferFromImageBitmap`
5. display receipt 왕복과 resource accounting

이 비용은 mailbox tuning만으로 Flutter 수준까지 제거할 수 없다. exact-only 정책도 새 viewport에서 이전 canvas 크기를 그대로 두므로 1~2 frame의 지연이 빈 band 또는 뒤늦은 추격으로 직접 보인다.

## 3. Flutter에서 가져올 구조적 원칙

Flutter Web도 backend 구현 중복을 줄이기 위해 DOM과 무관한 surface/rasterizer 계약으로 CanvasKit과 Skwasm을 통합하는 작업을 진행하고 있다. 고수준 책임은 공유하고 backend 차이는 마지막 native 호출과 resource 관리에 한정한다.

- Flutter backend 통합 방향: [flutter/flutter #175624](https://github.com/flutter/flutter/issues/175624)
- Flutter Web engine과 renderer test matrix: [web_ui README](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/README.md)
- Flutter는 지원될 때 multi-threaded Skwasm을 사용하고 호환성용 single-thread 옵션만 둔다: [Web initialization](https://docs.flutter.dev/platform-integration/web/initialization)

Doroti가 그대로 복제할 대상은 Flutter의 언어나 API가 아니라 다음 ownership이다.

- DOM/input/IME/semantics는 browser main thread가 소유한다.
- app/framework/Skia/GPU surface는 하나의 persistent Worker가 소유한다.
- document의 실제 canvas를 Worker에 이전하고 Worker가 visible WebGL surface에 직접 raster한다.
- metrics는 presentation ACK가 아니며 latest mailbox로 즉시 전진한다.
- 한 vsync에 build/layout/paint를 한 번만 수행하고 같은 frame 안에서 visible submit까지 끝낸다.

## 4. 목표 아키텍처

```text
Browser main thread
  DOM shell + input + IME + semantics + viewport metrics
             │
             │ typed protocol v2
             ▼
Persistent Doroti Worker
  one .NET runtime + widget/layout + Skia + frame coordinator
             │
             ▼
transferred document OffscreenCanvas
  WebGL2 staging FBO -> exact blit -> browser compositor
```

### 4.1 유일한 renderer

새 이름은 `worker-direct-webgl`로 고정한다.

1. main에서 document `HTMLCanvasElement` 하나를 만든다.
2. WebGL context를 만들기 전에 `transferControlToOffscreen()`으로 canvas control을 Worker에 한 번 이전한다.
3. Worker는 전달받은 실제 canvas에 Skia WebGL2 context와 staging FBO를 만든다.
4. managed scene을 target 크기의 staging FBO에 raster한다.
5. 같은 Worker task에서 backing size와 exact 1:1 blit을 완료한다.
6. 브라우저 compositor가 이 canvas를 직접 합성한다.

제품 경로에는 `new OffscreenCanvas()` raster copy, `createImageBitmap`, `bitmaprenderer`, bitmap receipt가 없어야 한다.

### 4.2 runtime과 DOM 소유권

Worker에는 app을 포함한 .NET runtime을 정확히 하나만 띄운다. main에는 Blazor/.NET runtime을 띄우지 않는다.

main thread 소유:

- root/canvas/IME/semantics DOM
- pointer, wheel, keyboard, focus, composition event sampling
- clipboard와 DOM plugin
- `visualViewport`/`ResizeObserver`/DPR 관측
- Worker crash 감시와 canvas 교체

Worker 소유:

- widget/element/render tree와 state
- scheduler, animation, layout, semantics tree 생성
- Skia resource/cache/font/image decode
- WebGL2 context, staging/front surface, context loss
- frame id, scene id, resize epoch, exactly-once terminal

### 4.3 protocol v2

`Record<string, unknown>` 메시지를 제거하고 shared discriminated union으로 바꾼다.

main → Worker:

- `boot`: transferred canvas, initial immutable host snapshot, runtime URL
- `metrics-latest`: current+latest depth 2가 아니라 단일 latest slot; superseded metrics는 build 전 제거
- `input`: pointer/wheel/key/focus/text/semantics action의 ordered sequence
- `control-response`: clipboard/plugin 응답
- `dispose` / diagnostic crash

Worker → main:

- `ready` / GPU identity / capability failure
- semantics delta, text state, cursor, focus request
- frame/latency/resource diagnostics
- fatal/context recovery status

bitmap/present/receipt 메시지는 protocol에서 삭제한다.

### 4.4 frame coordinator

Worker의 `WebFrameCoordinator`가 유일한 frame owner가 된다.

- transferred document canvas와 연결된 Worker `requestAnimationFrame`을 authoritative vsync로 사용한다.
- Worker rAF를 지원하지 않거나 cadence probe가 display refresh와 동기화되지 않으면 v2를 시작하지 않고 명확히 fail-closed한다. 이 단계에서 두 번째 renderer fallback은 만들지 않는다.
- rAF 시작 시 ordered input을 먼저 drain하고 가장 최신 metrics를 admission한다.
- 같은 vsync의 중복 invalidation은 하나의 build/layout/scene/raster로 합친다.
- raster 중 새 metrics/input이 오면 현재 immutable frame은 끝내되 latest 하나만 다음 rAF에 남긴다.
- terminal은 Worker의 exact visible blit 시 `submitted`, context/resource 실패 시 `failed`, build 전에 교체된 scene은 `superseded`로 정확히 한 번 기록한다.
- Web API로 scan-out ACK를 알 수 없으므로 `browser-present-unverified`는 유지한다.

### 4.5 Flutter식 resize 연속성 계약

기존의 “exact frame 전에는 visible canvas CSS 크기도 바꾸지 않는다”는 정책을 폐기한다. 이 정책이 pixel 왜곡은 막았지만 창과 canvas 경계가 1~3 frame 늦게 맞는 현상을 만들었다.

새 계약:

- root와 canvas CSS box는 main thread에서 viewport와 즉시 함께 움직인다.
- retained front의 browser compositor scaling은 다음 exact frame까지 최대 1 refresh만 허용한다.
- JS `transform` preview나 별도 bitmap preview는 만들지 않는다.
- Worker는 다음 rAF에서 latest logical/physical metrics로 backing과 Skia surface를 만들고 exact frame으로 교체한다.
- pure-black/root-background band는 0이어야 한다.
- 1 refresh를 넘긴 stale scale, resize 종료 뒤 단계적 추격, final target 불일치는 실패다.

즉, “모든 중간 paint가 pixel-exact”보다 “브라우저 경계는 즉시 연속적이고 다음 vsync에 exact”를 우선한다. 이 정책 변경은 기존 exact-only oracle도 함께 바꾸어야 한다.

### 4.6 crash와 context loss

transferred canvas는 죽은 Worker에서 되찾지 않는다.

- 최초 Worker fatal 시 main이 기존 canvas를 제거하고 같은 위치에 새 canvas를 만든다.
- 새 canvas를 새 Worker에 이전하고 latest host snapshot, focus/text state, app restart 정책을 적용한다.
- 자동 restart는 최대 1회다.
- app state 복원은 별도 persistence 계약이 없는 한 보장하지 않으며 README에 명시한다.
- WebGL context loss는 Worker가 같은 transferred canvas에서 resource를 재생성하고 latest scene을 replay한다.

## 5. 실행 순서

### S0. Flutter 대조 fixture와 baseline 고정

1. `reference/flutter_sample_app`을 DorotiDemoApp과 같은 grid, scroll, button, text field, blur/effect 부하로 맞춘다.
2. 현재 Flutter SDK/engine revision과 renderer를 artifact에 기록한다.
3. Chrome Release, 같은 HWND bounds, 같은 DPR, 같은 display refresh에서 Flutter와 현재 Doroti를 연속 실행한다.
4. 다음 timestamp를 양쪽에서 수집한다.
   - browser input/resize ingress
   - framework frame start/end
   - raster start/end
   - visible submit
   - next browser paint 관측 가능 범위
5. 평균이 아니라 sample 수, p50/p95/max, missed-refresh count, long task를 기록한다.

baseline 없이 “Flutter처럼 부드럽다”를 눈대중 완료 조건으로 사용하지 않는다.

### S1. protocol과 상태 기계 분리

대상:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- 새 `doroti.web.protocol.ts`
- `BrowserHostContracts.cs`
- `BrowserSkiaCapabilities.cs`

1. DOM shell/input/semantics code와 presenter/runtime bootstrap을 파일 단위로 분리한다.
2. protocol v2 union과 sequence/generation/frame id 타입을 만든다.
3. host snapshot 전체 JSON 재직렬화 대신 changed fields가 명시된 immutable metrics/control packet을 사용한다.
4. input ordering, latest metrics, terminal state machine을 독립 validator로 먼저 만든다.

Gate: 아직 renderer를 바꾸지 않은 상태에서 type check, protocol permutation, input ordering test가 PASS해야 한다.

### S2. transferred document canvas proof

대상:

- `doroti.loader.ts`
- 새 `doroti.web.worker.ts`
- 기존 `doroti.raster.worker.ts`에서 재사용할 GPU bootstrap

1. document canvas를 `transferControlToOffscreen()`으로 Worker에 전달한다.
2. Worker가 그 canvas에서 hardware WebGL2 identity를 증명한다.
3. main `bitmaprenderer` 없이 color/grid frame을 직접 표시한다.
4. Worker rAF cadence, resize backing 변경, context loss/restore, canvas replacement restart를 최소 fixture로 검증한다.

Gate:

- `bitmapCreated == 0`
- document canvas의 context owner는 Worker 하나
- main managed runtime 0, Worker managed runtime 1
- 5초 rAF cadence가 display refresh 기준 허용 편차 안
- Worker 종료 뒤 새 canvas/Worker로 bounded recovery PASS

S2 gate가 실패하면 전체 구현으로 진행하지 않고 기술 제약과 지원 브라우저 결정을 다시 검토한다.

### S3. managed runtime과 direct Skia surface 결합

1. 현재 persistent Worker의 .NET bootstrap을 새 Worker로 이동한다.
2. `DorotiWebWorkerSurface`가 새로 만든 hidden OffscreenCanvas가 아니라 전달받은 visible canvas/context를 사용하게 한다.
3. document 경로의 staging FBO + exact blit 로직을 Worker 공통 presenter로 옮긴다.
4. `requestPresent`를 `WebFrameCoordinator.SubmitScene`으로 바꾸고 JS/managed request id owner를 하나로 만든다.
5. `BrowserSkiaCapabilities`의 pending completion은 새 Worker terminal state machine에 맞춘다.

Gate: startup, first frame, effect shader, text, images, context loss가 direct canvas에서 PASS한다.

### S4. input/IME/semantics를 protocol v2로 이동

1. wheel은 누적하지 않고 각 sample을 즉시 sequence와 함께 보낸다.
2. pointer move는 pointer별 latest를 허용하되 down/up/cancel/scroll/key/text/semantics action은 순서를 보존한다.
3. input packet 도착 시 이미 예약된 rAF가 있으면 새 rAF를 추가하지 않고 같은 frame dirty state에 합친다.
4. IME composition은 계속 native DOM endpoint가 소유하며 managed ACK가 composition을 덮지 못하게 한다.
5. semantics는 현재 content/geometry delta를 유지하되 Worker→main 적용을 frame critical path 밖의 task로 보낸다.

Gate:

- click/wheel input sequence → corresponding visible submit p95
- long Backspace/composition/native selection
- keyboard/pointer/semantics action ordering
- resize 중 input starvation 0

### S5. resize continuity 정책 전환

1. main CSS box를 viewport와 즉시 동기화한다.
2. Worker latest metrics admission과 backing/surface resize를 한 frame transaction으로 묶는다.
3. exact-only test의 “transform/scale 항상 0” 조건을 “stale scale 최대 1 refresh” 조건으로 바꾼다.
4. screenshot에 timestamped edge marker를 넣어 black band, stale scale 체류, final exact를 측정한다.
5. 60/120/144Hz에서 refresh interval 기반 threshold를 사용하고 고정 16ms 상수를 쓰지 않는다.

### S6. 기존 세 경로 제거

아래를 제품 코드에서 삭제한다.

- `RequestedPresenterMode`, `PresenterPolicy`, `selectRendererMode`
- `CanvasPresenter`, `WorkerDisplayPresenter`, 기존 `WorkerPresenter` 분기
- `document-webgl`, `offscreen-bitmap`, `offscreen-worker` URL 선택
- `createImageBitmap`/`bitmaprenderer` display와 receipt protocol
- main-thread Blazor runtime bootstrap
- renderer별 README 주소와 renderer matrix test

diagnostics의 renderer 이름은 항상 `worker-direct-webgl` 하나다. 임시 A/B 플래그가 필요하면 test build에서만 사용하고 public URL/runtime policy로 남기지 않는다.

### S7. Flutter 대조 성능 gate

같은 machine/Chrome/display에서 Flutter fixture와 Doroti fixture를 번갈아 3회 이상 측정한다.

필수 목표:

| 지표 | v2 목표 |
|---|---|
| warm click/wheel ingress → visible submit p50 | 1 refresh 이내 |
| warm click/wheel ingress → visible submit p95 | 2 refresh 이내 |
| resize target → exact visible submit p50 | 1 refresh 이내 |
| resize target → exact visible submit p95 | 2 refresh 이내 |
| stale compositor scale | 최대 1 refresh |
| resize 중 black/root band | 0px |
| queue depth | frame current+latest 2 이하, metrics latest 1 |
| Worker/main long task | 50ms 초과 0, startup compile은 별도 기록 |
| resource/terminal | failed 0, unpaired 0, active transferable 0 |
| Flutter 대비 p95 | 동일 fixture에서 Flutter + 1 refresh보다 느리지 않음 |

first interaction은 JIT/AOT/startup warmup을 분리 기록한다. release startup 뒤 hidden synthetic input으로 몰래 warmup해 수치를 숨기지 않는다.

### S8. 최종 실사용 acceptance

자동화 통과 뒤 실제 Chrome에서 다음을 사용자가 확인한다.

- 좌/우/상/하/모서리 drag와 빠른 왕복
- maximize/restore와 큰 폭 resize
- 실제 wheel mouse와 trackpad fling
- drag 중 button/slider/text input
- IME 한글 조합과 길게 누른 Backspace
- 60Hz와 가능한 고주사율 monitor

뒤늦은 단계적 추격, 1 refresh 초과 stretch, 검은 band, input 무반응이 하나라도 보이면 `FAIL`이다. 자동 front commit은 실제 scan-out과 같다고 주장하지 않는다.

## 6. 필수 검증 명령

모든 test timeout은 repository 규칙대로 20분을 유지한다.

1. `dotnet build DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release`
2. `npm run check` in `Doroti/validation/web-playwright`
3. FCR-3 scheduler
4. FCR-7 Material/widget
5. resize/protocol v2 contract validator
6. Chromium headless 전체
7. Chrome headed live resize/input/flicker/context-loss/crash-recovery
8. Flutter reference differential benchmark

## 7. 지원 범위와 제외

v2 첫 완료 범위:

- Windows Chrome/Chromium
- hardware WebGL2
- `OffscreenCanvas`, `transferControlToOffscreen`, dedicated Worker rAF
- full-page single view

첫 완료에서 제외:

- Firefox/Edge/Safari 호환성 보장
- software/Canvas2D fallback
- multi-view/embedded host
- WebGPU backend
- Web 이외 platform 변경
- browser compositor의 실제 scan-out ACK 추정

지원 capability가 없을 때 과거 renderer로 자동 fallback하지 않는다. 명확한 capability 오류로 종료한다. 호환성 경로가 필요해지면 v2 엔진 계약을 공유하는 별도 후속 결정으로 다루며 세 presenter 구조를 되살리지 않는다.

## 8. 완료 조건

- public renderer 선택과 기존 세 구현이 제품 코드/문서에서 제거됨
- 실제 document canvas control이 Worker로 한 번만 이전됨
- Worker .NET runtime 1, main .NET runtime 0
- `createImageBitmap`/`bitmaprenderer`/display receipt 제품 사용 0
- input/metrics/frame/context/resource owner가 각각 하나
- protocol/resize/input/terminal validator PASS
- Release build 및 전체 browser regression PASS
- Flutter differential gate PASS
- 실제 Chrome drag/trackpad/IME acceptance PASS
- 미실행 browser/display/hardware는 `notVerified`로 명시

이 조건 전에는 “Flutter처럼 부드러운 Web 구현체 완료”로 판정하지 않는다.

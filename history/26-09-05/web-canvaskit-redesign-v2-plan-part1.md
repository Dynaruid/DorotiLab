# Web CanvasKit 렌더링 경로 재설계 작업계획 v2

작성일: 2026-09-05

대상: `http://127.0.0.1:5088/?dorotiRenderer=worker-canvaskit-webgl`

상태: **W0~W4 실험 구현·검증 진행 / W5 기능 회귀 검증 진행 / 최종 native 추종·캡처·수용 판정 대기**

2026-09-05 전체 작업 진행: [v2 실행 결과](history/26-09-05/web-canvaskit-redesign-v2-results.md). F0~F2·Flutter 동등 fixture, 픽셀 마커, clock/native 매칭, publish 빌드 분리, P0/P1/P2, 소유권 복사·metrics 병합·CPU picture cache 옵션을 구현했다. 전체 AOT 실패와 공용 bin/obj 혼합 실패는 보존하고 빌드 종류별 산출물을 격리했다. 기능 회귀 PASS와 최종 native 성능·사용자 수용은 구분한다.

## 1. 목표와 개편 결정

빠른 창 테두리 드래그 중 우측·하단 콘텐츠, 중앙 정렬, 줄바꿈이 실제 창 크기를 따라오도록 한다. 최종 크기 일치나 GPU 제출 횟수만으로 완료하지 않는다. 사용자의 “아직 부족하다”는 재확인을 개선 필요 판정으로 반영한다. 이번 조사에서 직접 드래그를 재현하거나 증상을 새로 정량화한 것은 아니다.

**권장 방향은 UI/Raster 분리를 유지하면서 매 프레임 전체 장면을 다시 펼쳐 보내는 비용을 줄이고, 완성 프레임의 표시 소유권을 조기에 비교하는 것이다.** 30fps 제한 해제를 다시 첫 해결책으로 삼지 않는다. AOT, 데이터 복사, retained picture, scheduling, presentation을 독립 실험으로 나누고 효과가 확인된 조합만 통합한다.

| 기존 계획에서 바꿀 점 | 새 결정 |
| --- | --- |
| 30fps 해제부터 순차 최적화 | 빠른 A/B에서 일관된 개선이 없었다. 실행 빌드·프레임 비용·브라우저 전달 지연을 먼저 분리한다. |
| mapping/encoding의 작은 최적화 중심 | picture/layer 재사용 단위를 wire와 Raster까지 유지하는 구조를 핵심 후보로 검증한다. |
| submit 목표 달성 후에만 bitmap 실험 | 단순 fixture에서 표시 경로를 앞서 비교한다. 제출 지연과 표시 ownership 문제는 함께 존재할 수 있다. |
| Release build를 공통 성능 기준으로 사용 | 현행 build, Release publish 비AOT, Release publish AOT를 구분한다. |
| observer→front 통지 중심 판정 | native 변화→observer, 첫 화면 갱신 대기, active 전체 오차, 캡처된 콘텐츠 위치를 추가한다. |

이 문서의 최초 작성 요청은 **검토와 작업계획 작성**이었다. 당시 제품·테스트·Flutter 소스를 수정하지 않았다. 이후 `work.md에 대해 작업해줘` 요청에 따라 W0부터 구현을 시작했다. `auto=document-webgl`, CanvasKit experimental opt-in, Windows/Vulkan 기본 경로를 유지한다.

## 2. 기존 결과와 조사 범위

- 이전 `work.md` 원문: [v1 계획 보관본](history/26-09-05/web-canvaskit-resize-plan-v1.md). R0~R6는 역사적 단계이며 새 W0~W5의 완료를 뜻하지 않는다.
- 기존 구현·실험: [2026-09-05 보고서](history/26-09-05/web-canvaskit-resize-experiments.md). enum boxing 제거와 stage trace가 남아 있고, snapshot 단일 apply와 terminal 대기 실험은 기각됐다.
- 빠른 native corpus는 **72회 유효 표본 모두 추종 gate FAIL**이다. 최초 matrix의 native 입력 FAIL 1건과 별도 재실행 성공 이력을 보존한다. 150ms trial별 observer→caught-up-front p95는 58.7~79.0ms였다. 이번에 재측정한 값이 아니다.
- TopLeft/150ms/reverse display 후보 p95 58.7 / 63.9 / 76.2ms는 일관된 개선이 아니다. 기본 30fps는 비교 baseline으로 남기며 v2 목표 정책으로 확정하지 않는다.
- 느린 corpus의 UI frame p95 약 29ms, mapping 약 3~4ms, encoding 약 8ms는 조사 단서다. 빠른 corpus의 병목 비율이나 AOT 성능으로 일반화하지 않는다.
- 기존 `CK5/CK7 FAIL`, `CK9 notVerified`, `PARTIAL / notQualified`는 [qualification 이력](history/26-09-01/web-canvaskit-raster-worker-summary.md)에 유지한다. 과거 사용자 직접 검증 `notVerified`를 소급 수정하지 않는다.
- 이번에 dirty checkout의 main/UI/Raster, managed bridge/mapper/encoder, Picture/SceneBuilder, framework frame 진입부, native harness를 읽었다. 기존 변경 파일은 보존했다.
- 5088 URL HTTP 접속은 **connection refused**였다. 사용자 브라우저가 실행한 binary를 확인하지 못했다. 서버 시작·빌드·테스트·성능 실험은 실행하지 않았다.
- Flutter 로컬 HEAD와 공식 동일 SHA 코드를 대조했다: `56b8e1a851a594b1a154f8ea93270807dab22b9a`. 검토한 frame/surface/compositing/picture recorder 파일은 Git 상태에 변경이 없었다. SDK 전체가 clean이라고 주장하지 않는다.
- CanvasKit pin은 **canvaskit-wasm 0.42.0 / default**다. 해당 패키지 타입 정의를 HTTP로 확인하고 Skia upstream JS도 읽었다. upstream main 전체가 pinned binary와 동일하다는 가정은 하지 않는다.

## 3. 현재 경로에서 확인한 구조적 비용

```text
native 창 변화 → 브라우저 viewport/layout → main observer
  ├─ immutable ResizeEpoch → Raster target fast lane
  └─ UI Worker metrics → Worker rAF / 기본 resize 30fps gate
       → framework build/layout/paint/scene
       → mapper: picture + retained subtree를 펼침
       → encoder: tables + commands + checksum + 최종 배열
       → managed MemoryView를 JS 소유 bytes로 복사
       → UI full validation + bytes.slice + current/latest
       → pool copy → MessagePort
       → Raster full validation → payload 재독해/replay
       → capacity canvas draw/flush → direct-commit 통지
       → 브라우저 OffscreenCanvas placeholder/compositor 갱신
```

| 위치·심볼 | 확인한 사실 | 설계상 의미 / 미확정 사항 |
| --- | --- | --- |
| `Doroti/src/Doroti.Host.Web/BrowserDisplayListMapper.cs`: `AppendScene`, `AppendPicture` | picture 명령을 전개하고 retained도 재귀 inline한다. 주석에 stable resource ID가 생기기 전까지 inline한다고 명시돼 있다. | framework 재사용이 전송·decode·JS→CanvasKit 호출 재사용으로 이어지지 않는다. 실제 unchanged 비율은 측정해야 한다. |
| `Doroti/src/Doroti.Ui/PaintingTypes.cs`, `GraphicsAndSemanticsContracts.cs` | Picture는 command snapshot을 보유한다. scene payload에는 commands/offset/bounds가 들어가며 독립 wire picture ID는 없다. retained payload에는 view/generation이 있다. | CLR 객체/배열 주소 대신 immutable snapshot의 명시적 identity와 수명을 설계한다. |
| `DisplayListEncoder.cs`: `Encode` | 매 장면 resource/string canonicalization, section writer, 최종 배열 복사, checksum 수행 | 반복 작업·할당·바이트량 감소가 다음 후보다. checksum을 끄는 것을 구조 개선으로 삼지 않는다. |
| `BrowserCanvasKitInterop.cs`, `doroti.web.ts`: `Submit`, `copyManagedBytes` | managed span을 JS로 넘기고 bytes를 만든 뒤 UI bridge 호출. MemoryView 경로에는 추가 slice가 있다. | 현재 UI `validateAndCopyMilliseconds`에는 앞단 복사가 빠져 있다. 총 copy 수/bytes/allocation을 먼저 측정한다. |
| `doroti.ui.worker.ts`: `submitDisplayList`, `sendCurrentScene` | full validation, `bytes.slice()`, 나중에 `pool.copy()`. current terminal 이후 latest 전송 | JS 소유 bytes의 중복 복사와 ACK 처리 지연을 분리한다. 소유권 변경 없이 slice만 지우지 않는다. |
| `doroti.web.protocol.ts`, `doroti.canvaskit.worker.ts` | UI/Raster에서 각각 payload까지 validate. Raster replay도 payload를 읽는다. | 검증+decode 중복 순회 후보다. 수신 경계의 크기/참조/stack/NaN/checksum 검증은 보존한다. |
| `doroti.canvaskit.worker.ts`: `scheduleDrain` | message 후 microtask에서 동기 replay | microtask는 다음 resize message를 먼저 처리하도록 양보하는 수단이 아니다. 실행 중 replay는 선점되지 않는다. |
| `doroti.canvaskit.host.ts`, `doroti.canvaskit.worker.ts` | screen/초기 크기 기반 capacity, CSS zoom=1/DPR, root clip, capacity 안에서 surface 재사용 | 매 resize surface 재생성이 주원인이라는 설명은 맞지 않는다. shrink 후 clear 영역 재노출과 DPR/capacity 경계는 별도 조사한다. |
| `Doroti.Framework.Widgets/binding.cs`: `drawFrame` | build/layout/paint/sceneBuild phase 기록과 dirty pipeline이 이미 있다. | frame trace를 stage ring에 연결한다. framework scheduling을 중복 구현하지 않는다. |
| `DorotiDemoApp/web/DorotiDemoApp.Web.csproj`, `run-web-playwright.ps1` | WasmBuildNative=true; wrapper는 dotnet build 후 dotnet run --no-build | 이것만으로 managed AOT 실행을 증명할 수 없다. 평가된 속성과 실제 배포 파일을 확인한다. |

**우선 가설:** 전체 scene 재전개·인코딩·복사와 UI 점유가 최신 metrics 처리 및 frame 생성을 늦춘다. **독립 가설:** Raster 제출과 브라우저 표시 사이의 위상·geometry ownership이 체감 지연을 더한다. 어느 하나도 이번 코드 조사만으로 확정된 주원인은 아니다.

## 4. 브라우저·Flutter·CanvasKit 근거

### 브라우저 API 계약

- ResizeObserver는 layout 이후 paint 전에 통지한다. devicePixelContentBoxSize 지원 여부와 DPR 전환 coherence를 확인한다. Doroti에는 이 경로·DPR watcher·full-page 조기 통지가 이미 있다. observer 추가만으로 해결하지 않는다. [ResizeObserver](https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver), [physical box](https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserverEntry/devicePixelContentBoxSize)
- visualViewport는 pinch zoom·키보드에 의해 layout viewport와 달라진다. 현재처럼 변화 신호와 실제 root metrics를 구분하고 embedded host에는 full-page 가정을 적용하지 않는다. [VisualViewport](https://developer.mozilla.org/en-US/docs/Web/API/VisualViewport)
- Worker rAF는 표시 주기의 예약 수단이며 main observer·다른 Worker·compositor와 같은 refresh 내 완료를 보장하지 않는다. 단일 예약과 프레임 내 immutable metrics를 유지한다. [Worker rAF](https://developer.mozilla.org/en-US/docs/Web/API/DedicatedWorkerGlobalScope/requestAnimationFrame)
- transferred OffscreenCanvas bitmap은 해당 agent의 rendering update에서 placeholder로 전달된다. flush/postMessage/direct-commit은 scan-out ACK가 아니다. microtask 반복도 새 렌더링 기회를 보장하지 않는다. [HTML canvas 표준](https://html.spec.whatwg.org/multipage/canvas.html#the-offscreencanvas-interface)
- transferToImageBitmap은 기존 bitmap을 넘기고 새 backing bitmap을 만든다. 이전 frame 보존 API로 쓰지 않는다. createImageBitmap은 Promise 기반이고 crop을 지정할 수 있다. 실제 allocation·복사·완료 지연을 비교한다. [transferToImageBitmap](https://developer.mozilla.org/en-US/docs/Web/API/OffscreenCanvas/transferToImageBitmap), [createImageBitmap](https://developer.mozilla.org/en-US/docs/Web/API/Window/createImageBitmap)
- bitmaprenderer.transferFromImageBitmap은 bitmap ownership을 소비한다. pending에서 버린 bitmap은 close하고 consumed bitmap과 구분한다. 소비 호출도 모니터 표시 완료가 아니다. [bitmaprenderer](https://developer.mozilla.org/en-US/docs/Web/API/ImageBitmapRenderingContext/transferFromImageBitmap)
- scheduler.postTask/yield는 지원 여부를 확인한 task 경계 실험용이다. managed frame·동기 replay를 자동 선점하지 않으며 vsync/present API도 아니다. main Atomics.wait, busy-wait, GPU finish로 맞추지 않는다. [Worker scheduler](https://developer.mozilla.org/en-US/docs/Web/API/WorkerGlobalScope/scheduler), [WebGL best practices](https://developer.mozilla.org/en-US/docs/Web/API/WebGL_API/WebGL_best_practices)

### Flutter에서 가져올 구조

아래 코드는 조사한 SHA에 고정한다. Flutter CanvasKit과 skwasm, OffscreenCanvas와 별도 Raster Worker는 서로 다른 개념이다. 실행 비교에서는 renderer asset·threading·isolation headers를 직접 기록한다. [Flutter Wasm 문서](https://docs.flutter.dev/platform-integration/web/wasm)

| Flutter 코드 | 확인한 구조 | Doroti 적용 |
| --- | --- | --- |
| [frame_service.dart](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/frame_service.dart) | 중복 schedule 방지, window rAF로 frame 실행 | 고정 resize FPS 대신 최신 상태와 하나의 reservation을 중심으로 재평가한다. main rAF를 Worker에 무조건 한 번 더 전달하지 않는다. |
| [picture_recorder.dart](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/picture_recorder.dart) | CanvasKit recorder로 SkPicture 생성 | 불변 picture를 매번 primitive로 재전송하는 비용을 줄일 근거다. 모든 resize에서 모든 picture가 재사용된다는 뜻은 아니다. |
| [rasterizer.dart](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/rasterizer.dart) | current/next queue, 다음 요청 교체 | FIFO 누적을 막되 latest-only 조건으로 계속 버려 starvation을 만들지 않는다. |
| [offscreen_canvas_rasterizer.dart](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/offscreen_canvas_rasterizer.dart), [surface.dart](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/surface.dart) | 이 CanvasKit 경로는 picture draw/flush 후 createImageBitmap(canvas). size/DPR 변화 시 surface 재생성 | Flutter가 transferToImageBitmap 또는 grow-only surface만 쓴다고 설명하지 않는다. |
| [render_canvas.dart](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/render_canvas.dart) | bitmap에 맞춘 physical canvas와 DPR에 따른 CSS 크기를 적용한 뒤 transfer | 완성 frame identity·pixels·geometry 적용 지점을 main이 소유하는 후보를 비교한다. 큰 canvas에 작은 bitmap을 넣어 늘어나는 구성을 피한다. |

### CanvasKit API와 실행 빌드

0.42.0 타입에는 PictureRecorder.beginRecording/finishRecordingAsPicture, MakeRenderTarget, MakeOnScreenGLSurface, makeImageSnapshot, flush가 있다. Raster에 SkPicture를 보관하는 prototype은 패키지 교체 없이 시작할 수 있다. snapshot Image와 브라우저 ImageBitmap은 다르며 SkPicture/Surface 핸들은 Worker 간 transferable이 아니다. [Skia 공식 개요](https://skia.org/docs/user/modules/canvaskit/), [0.42.0 타입 정의](https://unpkg.com/canvaskit-wasm@0.42.0/types/index.d.ts)

upstream Surface.requestAnimationFrame은 callback 뒤 flush하는 wrapper다. WebGL helper에는 software fallback과 explicitSwapControl 제약도 있다. 현재의 명시적 GPU context 생성·실패 처리를 유지하며 helper 교체나 별도 gl.flush 추가는 pinned runtime 검증 후 판단한다. [interface.js](https://github.com/google/skia/blob/main/modules/canvaskit/interface.js), [webgl.js](https://github.com/google/skia/blob/main/modules/canvaskit/webgl.js)

.NET AOT는 RunAOTCompilation=true와 publish 경로를 검증해야 한다. CPU 집약 코드의 이점뿐 아니라 다운로드·컴파일 시간·메모리도 비교한다. WasmBuildNative=true와 동의어가 아니다. [Microsoft AOT 문서](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0)

## 5. 권장 설계와 독립 비교안

### A. immutable picture를 전송·Raster 재사용 단위로 유지

목표 경로는 **UI의 변경된 picture 정의 + 프레임별 배치 명령 → Raster picture registry → drawPicture → presenter**다. resize epoch가 바뀌었다는 이유만으로 모든 picture content generation을 갱신하지 않는다.

1. W0에서 picture 재생성/재사용 수, command 수, encoded bytes를 frame에 연결한다. layout 자체가 전부 dirty라면 constraints·RepaintBoundary·paint invalidation을 먼저 좁힌다. 폭이 달라진 paragraph layout을 그대로 쓰는 cache는 금지한다.
2. **1차 후보:** 명시적 immutable picture ID/generation을 가진 CPU command block cache. unchanged block의 mapping/encoding을 재사용한다. resource/string table 참조를 유효하게 유지해야 하므로 다른 frame의 table index가 든 bytes를 그대로 붙이지 않는다.
3. **2차 후보:** define-picture / draw-picture / release-picture에 해당하는 versioned 계약. 이 명칭은 설계용이며 현재 protocol에 구현된 메시지가 아니다. Raster는 첫 수신 block을 검증·recording한 뒤 SkPicture를 재사용한다. 명시적으로 negotiate하고 기존 v1 bytes를 다른 의미로 해석하지 않는다.
4. 현재 worker의 MakePicture(bytes)는 serialized Skia picture 경로다. Doroti DisplayList bytes를 넣을 수 없다. renderer-neutral block을 Raster PictureRecorder로 기록하는 경로와 구분한다. UI CanvasKit에서 전체 scene을 다시 기록·직렬화하는 우회는 1차안에서 제외한다.
5. key는 view/producer session/content identity/generation/resource dependencies를 포함한다. frame resize/context identity와 picture identity를 구분한다. offset/clip/transform은 가능하면 composition 명령으로 둔다. font/image/shader generation, width 의존 text와 DPR 의존 raster cache를 정확히 무효화한다.
6. frame 참조 picture와 하위 resource를 terminal까지 pin한다. definition ACK 전 참조 금지, definition 실패 시 의존 frame 실패, superseded frame의 정확히 한 번 해제, producer handle dispose와 snapshot의 독립 수명을 명시한다.
7. Raster restart/context loss 때 CPU journal에서 재구축하고 필요한 resource 재생 후 scene을 허용한다. 옛 session completion은 무시한다. definition 전송도 bytes/개수 상한을 둔다. registry로 queue 밖 메모리 누적을 숨기지 않는다.
8. recording이 안전한 picture부터 지원한다. destination/backdrop 의존 효과·shader·image filter 등 미지원 조합은 기존 inline 경로로 명시적으로 분류한다. 품질·기능을 생략해 cache hit를 높이지 않는다.

채택 조건: 같은 output에서 warm unchanged picture의 재전송/재record가 사라지고 UI 비용·active 추종이 함께 개선되어야 한다. cache hit나 전송 bytes만 좋은 결과는 최종 채택 근거가 아니다. picture를 GPU bitmap으로 캐시하는 raster cache는 별도 후속 최적화다.

### B. build·copy·validation 비용 정리

- 현행 Release build를 기준으로 동일 소스의 Release publish 비AOT와 AOT를 별도 디렉터리·서버로 비교한다. SkipBuild만으로 publish serving이 됐다고 하지 않는다. wrapper의 정적 publish serve 옵션은 새 작업이다.
- BrowserCanvasKitInterop.Submit → copyManagedBytes → UI admission → pool.copy 전체 duration과 allocation bytes를 계측한다. managed view 수명을 넘기거나 WASM memory buffer를 detach하지 않는다.
- 최초 managed→JS 소유권 복사를 보존하면서 JS bytes 직접 transfer 또는 최초 복사를 pool에 수행하는 방식 중 하나를 실험한다. unsent latest 교체·transferred detached·반환 pool lease 상태를 분리한다.
- Raster 검증과 decode 결과 재사용을 우선 검토한다. UI identity 검사와 full validation의 책임을 문서화한다. full validation 횟수를 줄이면 malformed/corrupt/stale/resource-reference 회귀를 먼저 통과해야 한다.
- diagnostics 직렬화, heartbeat, trace export의 비용을 trace off와 비교한다. bounded ring drop, error/terminal counter는 유지한다.
- 기존 paragraph/paint/filter cache의 hit·miss·invalidation을 먼저 확인한다. warm resize에서 paragraph layout 호출이 없었던 느린 corpus를 근거로 text cache를 다시 추가하지 않는다. F2의 폭 변화·폰트 변경은 별도로 측정한다.
- snapshot 단일 apply를 근거 없이 반복하지 않는다. 빠른 corpus에서 해당 비용이 확인되고 일관된 metrics/context 계약을 제시했을 때만 재시도한다.

### C. 표시 경로를 W1에서 작은 prototype으로 비교

| 후보 | 구성 | 검증할 효과 / 비용 |
| --- | --- | --- |
| P0 | 현재 transferred visible OffscreenCanvas + capacity + microtask replay | baseline. capacity·retained pixels의 현재 계약 유지 |
| P1 | 같은 direct 경로에서 Raster drain을 하나의 Worker rAF로 coalesce | rendering phase 근처 제출 효과. 추가 tick 대기로 latency가 늘면 기각. 두 Worker rAF의 자동 동기화 가정 금지 |
| P2 | 독립 OffscreenCanvas raster → 완성 ImageBitmap → main bitmaprenderer | main이 bitmap identity·정확한 physical/CSS 크기를 함께 적용. snapshot/transfer/main 부하·추가 backing 비용 측정 |

P2 계약:

- 작은 fixture부터 UI/encode/scheduling을 고정해 비교한다. 기존 offscreen-bitmap/offscreen-worker lifecycle은 참고하되 CanvasKit 대안으로 이미 검증됐다고 간주하지 않는다.
- bitmap에 session/sequence/resize epoch/logical·physical size/DPR을 붙인다. main은 completed frame 하나의 identity로 크기와 bitmap을 적용한다. 현재 viewport 크기로 오래된 bitmap을 stretch하지 않는다.
- P2a exact-size 출력과 P2b capacity 출력의 exact crop을 구분한다. 전체 capacity bitmap을 target으로 축소해 PASS를 만들지 않는다. crop/Y 방향/alpha/color space를 확인한다.
- createImageBitmap의 비동기 완료 역전, transferToImageBitmap 이후 새 backing과 surface 유효성을 검증한다. Skia snapshot→readPixels→ImageData를 정상 프레임 경로에 넣지 않는다.
- bitmap 생성/전송 credit≤2, main 미소비 pending은 그 안에서 latest≤1. 교체/실패 bitmap을 close하고 소비 ACK로 credit을 반환한다. 소비 ACK와 GPU scene terminal은 다른 ledger다. displayed backing과 다음 raster backing까지 메모리에 포함한다.
- main 수신 task 즉시 적용과 main rAF 적용을 각각 비교한다. DOM update+transfer는 같은 JS turn에 묶을 수 있지만 compositor 원자성·scan-out 보장으로 표현하지 않는다.
- transferred canvas의 context mode를 중간 전환하지 않는다. host 재초기화로 새 canvas를 만들고 input/IME/semantics overlay bounds를 재검증한다. 미지원 browser는 후보 미지원으로 명시한다.
- capacity·staging·bitmap·picture cache의 메모리 예산은 W0에서 GPU 한계와 실제 bytes를 확인해 후보 실행 전에 고정한다. 초기 screen 크기 과할당과 DPR2/다중 모니터 비용을 포함한다. active resize 중 shrink 재할당을 넣지 않으며 정상 경로에서 GPU 동기 대기로 예산을 맞추지 않는다.

P2는 늦은 UI layout을 해결하지 않는다. 그럼에도 submit 기준 미달만으로 실험을 보류하지 않는다. P0/P1/P2의 캡처 경계·첫 front·긴 정지·geometry 오차로 선택한다.

### D. scheduling·transport와 조건부 topology 대안

- single pending frame, frame 시작 전 최신 수신 metrics, frame 동안 고정 epoch, resize 끝 final frame을 유지한다. metrics coalescing은 input/focus/IME/lifecycle 순서를 넘지 않는다. input 좌표의 geometry epoch와 적용 장벽도 확인한다.
- W2/W3에서 frame cost를 줄인 뒤 display scheduling을 다시 A/B한다. overload는 current+latest와 age/작업 예산으로 제어한다. 고정 30/60fps cap이나 resize-end debounce를 최종 해법으로 삼지 않는다.
- ACK 병목은 encoded→send, terminal sent→handled, Raster idle overlap을 빠른 corpus로 판정한다. 병목이면 transferred in-flight≤2 + UI unsent latest≤1의 credit 실험을 한다. 전역 scene 상한 3과 buffer/resource exactly-once를 검증한다. 느린 corpus의 notNeeded를 그대로 가져오지 않는다.
- yield/postTask는 frame 전 또는 검증된 command block 사이에서만 비교한다. 부분 그림을 visible surface에 노출한 채 yield하지 않는다. 필요시 staging에서 완료 후 commit하고 비용에 포함한다.
- 최적화 후에도 직렬화·두 Worker 경계가 주병목이면 **단일 render Worker의 managed UI+CanvasKit recording/raster 소유**를 제한 prototype으로 비교한다. main DOM/input 역할은 유지한다. wire encode를 그대로 둔 채 Worker 수만 줄인 결과를 경계 제거 효과로 부르지 않는다. input/metrics 수신이 raster에 막히는 비용과 startup/WASM 메모리를 함께 판정한다.
- SharedArrayBuffer/Atomics transport, skwasm 이식, WebGPU/커스텀 CanvasKit 빌드는 현재 주계획에서 제외한다. 남은 구체적 비용·호환성 근거가 있을 때 별도 설계한다. renderer 교체만으로 layout/encoding 문제가 사라진다고 가정하지 않는다.

## 6. 계측·비교 corpus 재구성

### W0에서 보강할 지표

현재 canvaskit-native-fast-resize.spec.ts는 observer target 이후 generation≥target인 첫 front를 찾아 caught-up 시간을 계산한다. 이는 **같은 epoch를 그린 시간이나 실제 표시 시간이 아니다**. 빠른 반전에서 큰 generation이 현재 창 geometry에 가까움을 보장하지 않는다.

active front 사이 interval만 계산하면 첫 front 이전과 마지막 front 이후 공백이 빠진다. finite latency만 모으면 미도달 target이 통계에서 사라질 수 있다. 새 schema에는 아래를 넣고 구 schema p95와 직접 합산하지 않는다.

| 지표 | 정의·처리 |
| --- | --- |
| native→observer | QPC rect 변화와 root metrics 통지 차이. outer rect와 content viewport는 chrome frame·DPI 보정. 동치 target 매칭이 모호하면 uncertainty 또는 notComparable |
| observer→submit / observer→main notification | 서로 다른 endpoint로 저장. superseded/미도달 건수와 전체 분모 명시. 동일 epoch latency와 caught-up latency 분리 |
| first-front / boundary-inclusive gap | motion start→첫 새 front, 연속 front interval, 마지막 front→motion end 포함. front 0회 FAIL, 1회는 interval PASS가 아닌 표본 부족 |
| time-weighted geometry error | target(t)와 front(t)의 width/height·면적 오차 적분 및 p95/max. native active와 observer active 각각 집계. Left/TopLeft content origin/edge도 캡처에서 분석 |
| content age | front metrics 관측 시점 기준. 같은 오래된 frame 재제출로 0 초기화 금지. idle baseline age와 active age 분리 |
| settle | 마지막 target과 같은 epoch/size/DPR의 front 도달. native motion 종료 기준도 별도 보고. reverse 100ms 원점 hold와 motion 분리 |
| work / transport | managed build/layout/paint/scene/map/encode/interop-copy, UI validate/copy/send, Raster validate/record/replay/flush, terminal 왕복, Worker idle. 중첩 duration·JS busy proxy를 OS CPU로 합산하지 않음 |
| captured presentation | fixture의 픽셀 frame/epoch marker, edge/center/wrap landmark와 window rect를 캡처 시간에 연결. DOM dataset/GPU counter로 캡처 frame identity를 대체하지 않음 |

시계는 performance.timeOrigin+performance.now로 통일하고 main/UI/Raster 정밀도와 QPC 변환 오차·drift를 저장한다. uncertainty보다 작은 차이를 개선으로 단정하지 않는다. trace/capture off 성능 corpus와 on 진단 corpus를 분리한다. WGC/브라우저 캡처도 물리 scan-out을 증명하지 않는다.

### 공통 조건

- **F0:** 최소 edge/center marker. UI 비용과 presentation 하한 분리.
- **F1:** 고정 picture 재배치. retained 효과 검증.
- **F2:** 폭에 따른 줄바꿈·layout, text/clip/filter. 잘못된 cache 검출.
- **F3:** 사용자가 확인한 현재 demo. 실사용 채택 기준.

Flutter F0~F2는 같은 logical geometry·글꼴·문자·wrap breakpoint·효과로 만든다. CanvasKit renderer 고정을 먼저 비교하고 skwasm은 별도 표로 둔다. Flutter post-frame과 Doroti GPU submit의 상대 p95를 비교하지 않는다. 공통 캡처 marker endpoint를 쓰거나 상대 latency는 notComparable로 둔다.

주 자극은 Windows SendInput **240Hz / 600px / 150·600ms / Right·Bottom·Left·TopLeft / expand·shrink·reverse**다. 조건별 baseline/후보를 교대로 최소 3회 실행한다. 입력 span -10~+25ms, 요청 이동량 80% 이상을 유지한다. step마다 CDP 왕복·screenshot·settle·sleep을 넣지 않는다. 작은 CDP resize는 기능 진단용이다.

manifest에는 HEAD+dirty patch/hash, 실제 served JS/WASM hash, build/publish/AOT 속성, .NET SDK, Flutter SHA/renderer/asset, CanvasKit pin, browser, GPU/backend, DPI/DPR, 주사율, viewport/headers를 기록한다. 기존 165Hz/DPI 200% 결과를 60Hz로 재명명하지 않는다.

DPR 1/1.25/1.5/2, browser zoom, capacity 초과, 즉시 반전, 최대화/복원, embedded host, monitor 이동, background 복귀를 검증한다. UI/Raster/main 100ms stall과 context loss/restart는 정상 성능 표에서 분리한다. 60Hz 환경을 먼저 고정하고 가능한 120/165Hz는 별도 수치로 남긴다. 미실행 환경은 notVerified다.

## 7. 구현 순서와 gate

각 단계는 변경 목록, 환경 manifest, 실험 JSON, 가설 채택/기각 이유, 한계를 기록한다. **앞 단계 성능 FAIL은 원인이 분리된 다음 실험을 막지 않는다. correctness FAIL인 후보를 제품 기본값으로 채택하지 않는다.**

| 단계 | 작업·주요 위치 | 완료 / 다음 단계 조건 |
| --- | --- | --- |
| W0 | native-fast-resize 지표, frame trace 연결, binary/clock manifest, F0~F3 및 Flutter 동등 fixture | 첫/끝 공백·미도달·geometry 오차 포함. 빠른 baseline 최소 3회와 병목 표. 성능이 나빠도 W1 진입 가능 |
| W1 | publish 비AOT/AOT serving; F0의 P0/P1/P2 표시 prototype | 빌드 효과와 표시 효과 분리. correctness·bitmap 수명 확인 후 캡처/지연으로 방향 결정. retained 완료를 기다리지 않음 |
| W2 | BrowserCanvasKitInterop, doroti.web.ts, UI worker, protocol의 copy/validation 소유권; 실측 dirty 비용 수정 | 전체 copy/allocation 및 빠른 latency 개선. golden/malformed/terminal/context PASS. 무효화 회귀 없음 |
| W3a | Picture/Scene payload identity, mapper immutable block cache | F1 unchanged block 재mapping/encoding 감소. F2 줄바꿈/resource 변화 반영. demo 재사용 비율 확인 |
| W3b | versioned picture definition/reference, Raster recorder registry, release/restart journal | F0~F3 inline 기준과 같은 output. 전체 resource 수명·메모리 상한 PASS. 비용·추종 개선 시 채택. 효과 없으면 W3a만 유지 가능 |
| W4 | P 후보 통합, display scheduling 재A/B, 필요시 credit 2, capacity/GPU budget | 동일 binary 옵션별 조합 효과 확인. 원인 없이 상수 조정 금지. 두 Worker 비용이 계속 주병목일 때만 topology 대안 |
| W5 | 전체 native matrix, Flutter 공통 endpoint, input/TextField/IME/zoom/restart, 사용자 직접 드래그 | 자동 성능·correctness·사용자 체감 모두 충족. 자동 PASS만으로 완료 금지 |

W3는 핵심 후보지만 효과를 전제하지 않는다. W1/W2에서 해결되거나 demo 재사용 가능성이 낮으면 확대 기각 근거를 남긴다. 시작한 구조 변경을 끝내기 위해 불필요한 protocol 복잡도를 채택하지 않는다.

### 수치와 채택 규칙

- 기존 **observer→caught-up-front p95≤33.3ms, front interval p95≤33.3ms, settle≤50ms** 목표 유지. 구 지표와 W0 보강 지표를 모두 보고하고 정의 변경으로 과거 FAIL을 PASS로 바꾸지 않는다.
- first-front와 boundary-inclusive gap을 포함해 **정상 부하 active 100ms 초과 정지 0회**. 미도달·표본 부족을 성공 sample에서 조용히 제외하지 않는다.
- 개별 최적화 채택은 대응 baseline 대비 관련 end-to-end p95 20% 이상 개선 또는 절대 목표 달성, p99/max·geometry 적분·첫 front의 반복적 퇴행 없음으로 판단한다. bytes/CPU 감소만 있는 변경과 목표 달성을 구분한다.
- native→observer·캡처 오차 gate는 W0에서 환경/정밀도/동일 Flutter fixture 확인 후 **후보 실행 전에** 고정한다. 사후 완화 금지. 동일 endpoint이면 Flutter보다 display interval 한 번 이상 뒤처지지 않는 것이 상대 목표다.
- 33.3ms는 기존 최소 추종 목표이며 120/165Hz 품질 완료가 아니다. 주사율별 놓친 tick과 실제 움직임을 함께 판정한다.
- resize/context/DPR 혼합, generation 역행, stale session commit, 중복 terminal, use-after-free, 반환 후 누수는 0. cache는 warm 상한에서 안정되고 restart 후 lease 회수.
- non-resize animation/input p95가 baseline 대비 10% 초과 악화되면 재검토한다. variance와 절대 ms를 함께 보고한다. text/clip/filter output, focus/selection/IME geometry 회귀는 허용하지 않는다.
- 사용자가 Right/Bottom/Left/TopLeft·빠른 반전에서 경계 노출·흔들림·줄바꿈 추종이 충분하다고 판단해야 한다. 현재 피드백은 부족 상태이며 새 후보 사용자 검증은 notVerified다.

## 8. 검증 진입점과 산출물

`.github/copilot-instructions.md`에 따라 **모든 테스트 프로세스 timeout은 20분**이다. 긴 matrix는 조건별 프로세스로 나누고 각각 같은 제한을 적용한다. 실제 빌드/서버/테스트 명령을 결과 보고서에 남긴다.

현재 존재하는 진입점:

```powershell
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -FastResize -Port 5188 -ArtifactLabel resize-v2-w0-baseline

pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -FastResize -RequireLatencyGate -Port 5188 -ArtifactLabel resize-v2-final

pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -HeadlessOnly -TestFile tests/canvaskit-worker.spec.ts -Port 5188

pwsh -NoProfile -File ./Doroti/eng/run-web-flutter-differential.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -FlutterRenderer canvaskit -Resize -DorotiPort 5188 -FlutterPort 5189
```

Flutter 명령은 현행 resize harness 진입점이며 동등 fixture/endpoint/native drag가 완료됐다는 뜻은 아니다. AOT publish serving, P1/P2, retained transport 옵션은 **아직 없다**. W1~W4에서 옵션과 diagnostics identity를 함께 추가한다. 기본 URL과 display 후보 등 실제 설정을 구분한다.

DOROTI_FAST_RESIZE_EDGE/MOTION/MS/RUNS로 진단 범위를 좁힐 수 있으나 최종은 전체 matrix다. Playwright video/trace는 주 성능 corpus에서 끄고 캡처 진단은 별도 실행한다. 점유 포트는 다른 포트로 피하며 사용자 프로세스를 종료하지 않는다.

변경 범위에 맞게 TypeScript check, managed/cross-language DisplayList golden, protocol 오류·buffer/lease/terminal, TextField/input/DPR/resize 회귀를 실행한다. 공통 Ui/host 수정은 auto, worker-direct-webgl 및 영향받는 Windows/shared contract를 확인한다. 전체 backend qualification 완료로 확대 해석하지 않는다.

산출물:

- `history/26-09-05/web-canvaskit-redesign-v2-results.md`: W0 부분 구현·실행 결과 보고서. 최초 계획 조사와 이후 실행 증거를 구분한다.
- `Doroti/validation/web-playwright/artifacts/resize-v2-<stage>-<variant>/`: manifest, 원시 stage/native/capture, trial/조건별 결과, source patch/hash.
- 최종 표는 correctness, stimulus, latency, capturedPresentation, flutterComparability, manualAcceptance를 분리한다. 성능 FAIL인데 exit 0인 baseline 수집도 그대로 표시한다.

## 9. 현재 체크리스트

- [x] 이전 계획·실험 FAIL·기각 이력 검토, v1 원문 보관
- [x] retained flattening, interop copy, validation/replay, scheduling/presentation source 검토
- [x] 브라우저 표준/API, Flutter 고정 SHA, CanvasKit 타입/upstream JS, .NET AOT 문서 대조
- [x] W0~W5, 비교 후보, 수명 계약, 채택·기각 및 사용자 판정 기준 작성
- [ ] W0 빠른 baseline 재구성과 보강 지표·동등 fixture
- W0 부분 완료: v2 main notification 지표, synthetic 회귀 3건, trace off/on 분리, source/관측 served asset manifest, F3 TopLeft/150ms/reverse 3회씩. 동등 fixture·captured endpoint·clock 보정·전체 baseline은 남아 있다.
- [ ] W1 AOT와 P0/P1/P2 독립 실험
- [ ] W2 copy/validation·실측 dirty 비용 개선
- [ ] W3 retained 전송·Raster 재사용 검증과 채택 판단
- [ ] W4 scheduling/presentation 통합 및 조건부 대안 판정
- [ ] W5 전체 자동 gate와 사용자 직접 드래그 수용

계획 작성은 완료했고 W0는 부분 구현 상태다. 제품 개선·성능 달성·실제 화면 수용 완료를 의미하지 않는다.

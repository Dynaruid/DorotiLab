# Web CanvasKit 창 크기 실시간 추종 개편 계획

작성일: 2026-09-05

대상: `http://127.0.0.1:5088/?dorotiRenderer=worker-canvaskit-webgl`

상태: **구현·A/B 실험 진행 / encoding 개선 및 계측 추가 / 리사이즈 성능 gate FAIL / 실제 드래그 notVerified**

2026-09-05 실행 기록: [구현·측정 보고서](history/26-09-05/web-canvaskit-resize-experiments.md). **사용자 지적에 따라 작은 폭의 CDP resize를 주 성능 corpus에서 제외하고, Windows와 같은 native SendInput 240Hz / 600px / 150·600ms 조건으로 R0부터 다시 측정한다.** 이전 느린 corpus의 수치와 채택·기각 판단은 빠른 drag의 근거로 재사용하지 않는다. 현재 기본 scheduling은 30fps baseline이며, `&dorotiResizeScheduling=display`로 제한 해제 후보를 선택한다.

## 1. 목표와 권장 방향

창 테두리를 움직이는 동안 콘텐츠의 레이아웃과 우측·하단 경계가 창 크기를 최대한 빠르게 따라오도록 한다. 종료 후 최종 크기가 맞는 것만으로 완료하지 않는다. Flutter도 완벽한 동기화를 보장하지 않지만, 같은 환경에서 비교할 반응성 기준으로 삼는다.

우선순위는 **고정 30fps 제한 해제 → 최신 metrics 반영과 UI 작업 비용 개선 → 측정으로 확인된 전송 대기 제거 → 필요할 때만 표시 방식 비교**다. 기존 Worker 분리나 CanvasKit 자체를 교체하는 것부터 시작하지 않는다.

현재 소스에는 이미 최신 target의 main→Raster 직통 전달, 프레임 시작 시 최신 epoch 사용, current+latest mailbox, grow-only GPU backing이 있다. 이들을 새 기능처럼 다시 구현하기보다 남은 지연의 소유자를 찾아 줄인다. 특히 `liveResizeTargetFramesPerSecond = 30`은 현재 요청인 최대한 빠른 추종과 맞지 않는다.

본 문서의 최초 작성은 `work.md`만 변경하는 계획 작업이었다. 2026-09-05 사용자가 이 계획에 따른 구현·검증을 명시하여 제품 코드와 검증 harness 작업으로 확장했다. `auto=document-webgl`과 experimental opt-in 정책, 기존 Windows/Vulkan 작업 및 과거 FAIL 기록을 유지한다. CanvasKit 전체 qualification과 이 리사이즈 개선의 판정은 별도로 기록한다.

## 2. 계획 작성 당시 조사 범위와 근거의 한계

- 현재 체크아웃의 main/UI/Raster TypeScript, managed host/DisplayList 진입부, resize 테스트 및 Flutter 비교 wrapper를 읽었다.
- 로컬 Flutter reference HEAD는 `56b8e1a851a594b1a154f8ea93270807dab22b9a`다. 웹의 Flutter master 소스도 대조했으나, 사용자가 비교한 Flutter 앱의 버전·renderer·빌드 모드는 알려져 있지 않다.
- 제공 URL에 HTTP 접속을 시도했으나 연결이 거부됐다. 현재 실행 산출물의 소스 일치, 브라우저 진단값, 실제 드래그를 확인한 것으로 취급하지 않는다. 조사 때문에 서버를 새로 빌드하거나 사용자의 프로세스를 종료하지 않았다.
- 계획 작성 단계에서는 빌드·Playwright·성능 실험을 실행하지 않았다. 이 절의 비용 추정은 당시 코드상 대기 가능성이며, 이후 실측은 실행 보고서에 분리했다.
- `history/26-09-01/web-canvaskit-raster-worker-summary.md`의 `PARTIAL / notQualified`, CK5/CK7 FAIL, CK9 notVerified는 과거 판정으로 보존한다. 이번 계획으로 이를 PASS로 바꾸지 않는다.

## 3. 현재 경로와 병목 후보

```text
main: ResizeObserver / full-page visualViewport.resize / DPR watcher
  ├─ immutable ResizeEpoch ───────────────→ Raster target fast lane
  └─ resize-epoch → UI Worker
                     ├─ managed metrics + snapshot 반영
                     ├─ Worker rAF + resize 중 30fps 제한
                     └─ build/layout/paint → DisplayList encode/copy
                           └─ UI current+latest / terminal 대기
                                → MessagePort → Raster decode/replay/flush
                                     ├─ receipt + terminal → UI
                                     └─ direct-commit → main 진단/CSS geometry
브라우저가 transferred canvas 결과를 compositor에 반영
```

`flush`, `submitted`, `direct-commit`은 GPU 제출 또는 JS 통지다. 실제 모니터 표시 완료 ACK가 아니다. main/UI/Raster 메시지 순서도 브라우저 compositor와 하나의 원자적 트랜잭션을 구성하지 않는다.

| 파일·심볼 | 현재 확인한 동작 | 변경 방향·검증 |
| --- | --- | --- |
| `Doroti/src/Doroti.Host.Web/Web/doroti.ui.worker.ts`: `scheduleManagedFrame`, 129행 부근 | 최근 resize ingress 후 100ms 동안 dispatch 간격을 약 33.3ms 이상으로 제한한다. 100ms는 활성 판정 창이며 매번 100ms 기다리는 debounce가 아니다. | 우선 30fps 제한만 제거한 A/B. rAF 예약은 하나로 유지하며 CPU가 가능하면 매 display tick 처리한다. 고정 60fps 타이머로 대체하지 않는다. |
| `doroti.web.ts`: `observeResizeEntry`, `observeFullPageViewport`, `commitObservedResize` | root observer 외에 full-page viewport 조기 통지가 이미 있다. CanvasKit의 CSS는 observer target에 즉시 맞춰 늘리지 않는다. | 같은 크기의 중복 통지·DPR coherence를 검증한다. observer에 무조건 rAF를 덧붙이면 한 번 더 늦어질 수 있다. |
| `doroti.canvaskit.host.ts`: `dispatchResizeEpoch` | UI가 managed WASM에서 바쁠 때도 Raster에 최신 target을 직접 전달한다. | fast lane을 유지한다. target 전달만으로 최신 레이아웃이 만들어지지는 않는다는 한계를 계측한다. |
| `doroti.ui.worker.ts`: `resize-epoch` handler | 각 메시지마다 `dispatchWorkerResizeEpoch` 후 전체 snapshot JSON dispatch를 수행한다. | metrics와 surface/context identity가 한 번에 일관되게 반영되는지 조사하고 중복 parse/apply를 줄인다. input/focus/lifecycle 순서를 보존한다. |
| `Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs`: `DispatchAnimationFrame` | callback 실행 직전에 `host.ViewEpoch`를 읽는다. | 최신 metrics 사용은 이미 구현됐다. 아직 UI event loop가 받지 못한 main 메시지까지 최신이라고 가정하지 않는다. |
| `doroti.ui.worker.ts`: `submitDisplayList`, `sendCurrentScene`, `finishScene` | UI의 current 장면 하나만 전송하고 terminal 처리 후 latest를 승격한다. `bytes.slice()`와 pool copy가 있다. | UI busy에 따른 ACK 처리 지연, 전송 공백, 바이트 복사 비용을 분리 측정한다. 필요하면 제한된 선행 전송으로 바꾼다. |
| `doroti.canvaskit.worker.ts`: `scheduleDrain`, `render` | message 뒤 microtask에서 동기 decode/replay를 처리한다. Raster에도 current+latest가 있으나 UI는 한 장면씩 보낸다. | 두 mailbox가 있다는 사실만으로 병렬 처리가 충분하다고 판단하지 않는다. JS 동기 replay 중에는 새 target 메시지를 처리할 수 없다. |
| `doroti.canvaskit.worker.ts`: latest-target priority | generation gap ≥2, 직전 replay ≥8ms, front age <40ms이면 progressive 장면을 버릴 수 있다. | gap만 줄이려다 front가 멈추지 않게 한다. 실제 화면 나이·크기 오차와 함께 판정한다. 40ms를 무조건 연장하지 않는다. |
| `doroti.canvaskit.host.ts`: `initialCanvasKitCapacity`, `commitCanvasKitFrontGeometry` | 화면 크기 또는 초기 크기×1.5 이상의 backing, `zoom=1/DPR`, root clipping으로 1:1 매핑한다. | 매 resize마다 surface를 재생성하는 구조가 아니다. capacity 증가/DPR 변경 구간과 일반 구간을 분리한다. |
| `doroti.canvaskit.worker.ts`: `replayIntoVisibleCapacity`, `renderThroughResizeStaging` | capacity 안에서는 재사용한다. 초과 시 staging→확장→복사하며, 축소로 벗어난 밴드는 clear한다. | 재확대 때 새 front가 오기 전 투명 영역이 노출될 수 있다. 콘텐츠 추종·노출 면적을 측정하고 배경색 일치만으로 해결했다고 하지 않는다. |
| `BrowserCanvasKitCapabilities.cs`, `BrowserDisplayListMapper.cs`, `doroti.canvaskit.text.ts` | managed scene mapping/encoding과 UI text 측정, Raster text 재구성 경로가 있다. | UI 프레임이 예산을 넘으면 layout/text/encode/copy를 각각 계측하고 상위 비용부터 줄인다. |
| `canvaskit-worker.spec.ts`, `resize-continuity.spec.ts` | 30fps 최소 간격을 검사하는 assertion과 native 연속 resize 회귀가 있다. | 30fps 정책 assertion을 새 scheduling 계약으로 교체한다. 기존 무누락·DPR·buffer·terminal 검증은 유지한다. |

가장 확실한 사실은 30fps 제한의 존재다. 그 제한을 제거하면 얼마나 개선되는지, UI layout과 Raster 중 어느 쪽이 최종 병목인지는 R0/R1 실측으로 결정한다. 단일 managed 프레임이 20~30ms 걸린다면 제한 해제만으로 60Hz 추종을 달성할 수 없다.

## 4. Flutter와 웹 문서에서 가져올 점

| 확인한 자료 | 실제 내용 | Doroti에 적용할 판단 |
| --- | --- | --- |
| Flutter `frame_service.dart`: `scheduleFrame` | 중복 프레임 예약을 막고 window rAF callback에서 프레임을 처리한다. 조사한 경로에는 Doroti와 같은 resize 전용 30fps gate가 없다. | 프레임 예약 한 개와 최신 상태 반영을 유지하면서 불필요한 추가 대기를 줄인다. |
| Flutter `window.dart`, `full_page_dimensions_provider.dart` | full-page resize는 visualViewport/window resize에서 metrics 갱신으로 이어진다. | Doroti에도 조기 경로가 있으므로 이를 유지한다. embedded root와 모바일 keyboard/insets를 full-page와 혼동하지 않는다. |
| Flutter `compositing/rasterizer.dart`: `RenderQueue` | current와 next를 두고 새 요청이 next를 대체한다. | 모든 중간 크기를 FIFO로 그리지 않는다. 동시에 최신 generation만 허용해 연속 resize가 굶는 정책도 피한다. |
| Flutter `compositing/offscreen_canvas_rasterizer.dart`, `render_canvas.dart` | offscreen 결과를 bitmap으로 만들고 visible RenderCanvas가 bitmap 크기에 맞춰 표시한다. CSS logical size와 physical bitmap을 구분한다. | 표시 ownership을 분리하는 대안은 가능하다. 그러나 ImageBitmap 전송이 항상 빠르거나 zero-copy라는 결론은 아니다. |
| Flutter `canvaskit/surface.dart`: `setSize` | size/DPR가 같으면 재사용하고, 달라지면 provider resize 및 SkSurface 재생성을 수행한다. onscreen/offscreen 구현도 구분된다. | Flutter가 빠른 이유를 무조건 grow-only capacity 또는 특정 surface 기법으로 설명하지 않는다. Doroti의 기존 capacity 재사용은 별도로 평가한다. |
| MDN ResizeObserver / Worker rAF | observer는 paint 전 통지하며 callback이 크기를 바꾸면 루프를 만들 수 있다. Worker rAF는 display 주기에 맞춰 호출되고 background에서 제한될 수 있다. | resize 콜백에서는 가벼운 metrics 전달만 한다. 이벤트→Worker rAF→GPU→compositor가 같은 refresh 안에 끝난다고 보장하지 않는다. |

현재 Flutter는 렌더링 경로가 여러 개이므로 **CanvasKit과 skwasm을 구분**해 비교해야 한다. OffscreenCanvas 사용만으로 Raster Worker 사용을 단정하지 않는다. 현재 repository의 `run-web-flutter-differential.ps1`은 Flutter를 `--wasm`으로 빌드하고 Doroti를 `worker-direct-webgl`로 고정한다. `flutter-differential.spec.ts` 역시 버튼 클릭→관측된 프레임을 비교한다. 이를 그대로 실행한 결과는 이번 CanvasKit resize의 비교 근거가 될 수 없다.

### 외부 출처

2026-09-05에 열람했다. master 링크는 변할 수 있으므로 실험 보고서에는 실제 SDK/engine SHA와 renderer를 반드시 고정한다. 로컬 비교 소스는 `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/` 아래에 있다.

- [Flutter frame scheduling](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/frame_service.dart)
- [Flutter current/next RenderQueue](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/rasterizer.dart)
- [Flutter offscreen rasterizer](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/offscreen_canvas_rasterizer.dart)
- [Flutter CanvasKit surface](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/surface.dart)
- [MDN ResizeObserver](https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver)
- [MDN Worker requestAnimationFrame](https://developer.mozilla.org/en-US/docs/Web/API/DedicatedWorkerGlobalScope/requestAnimationFrame)
- [MDN OffscreenCanvas](https://developer.mozilla.org/en-US/docs/Web/API/OffscreenCanvas)
- [MDN transferToImageBitmap와 자원 수명](https://developer.mozilla.org/en-US/docs/Web/API/OffscreenCanvas/transferToImageBitmap)
- [Flutter web performance 측정](https://docs.flutter.dev/perf/web-performance)

## 5. 개편 설계

### A. 화면 주기에 맞춰 최신 크기의 UI 프레임 생성

1. 현재 30fps 설정을 baseline으로 보존하고 실험 설정에서 gate만 제거한다. 일반 animation, callback ID 검증, dispose/restart 취소 동작은 유지한다.
2. main은 유효한 immutable epoch를 즉시 발행한다. UI는 자신이 수신한 최신 metrics를 다음 프레임 직전에 적용하고 프레임 내에서는 그 epoch를 고정한다. 프레임 중간에 width/height/DPR를 섞지 않는다.
3. pending callback은 하나로 coalesce한다. metrics를 합칠 때는 동일 host/session의 이전 metrics만 대체하며 input/focus/lifecycle event를 버리지 않는다. 필요하면 input 직전에 pending metrics를 먼저 반영하는 순서 장벽을 둔다.
4. 최종 resize target은 animation이 없어도 한 번은 처리되어야 한다. resize 종료를 기다리는 debounce나 100ms quiet period를 렌더링 선행 조건으로 만들지 않는다.
5. 과부하에서는 대기열 길이와 오래된 작업을 줄인다. 고정 30fps로 되돌리기 전에 UI 작업시간·Raster 상태·front age를 보고 최신 pending만 유지한다. 새로운 즉시 프레임 경로가 필요하면 rAF와 중복 실행되지 않는 별도 실험으로 측정한다.

### B. 전송 대기가 실측 병목이면 bounded 선행 전송

현재 Raster terminal이 UI에 도착해도 UI가 WASM 작업 중이면 다음 장면 전송이 늦을 수 있다. 먼저 `raster terminal sent → UI terminal handled → next scene sent`와 Raster idle 시간을 측정한다.

병목이 확인되면 **전송 credit 최대 2개 + UI 미전송 latest 최대 1개**인 실험을 한다. UI의 단일 `currentScene` 의존을 sequence별 in-flight registry로 분리하고, Raster의 실행 current + 최신 pending을 유지한다. 추가된 credit은 transport 최적화이며 queue 무제한 확장이 아니다.

- scene의 실제 raster/terminal과 전송 credit 반환을 혼동하지 않는다. 구현이 terminal에서 buffer를 반환한다면 그 계약을 유지한 채 두 번째 장면을 선행 전송한다.
- 두 번째 장면을 보냈으면 추가 장면은 UI latest 하나로 대체한다. terminal이 돌아올 때까지 임의로 credit을 늘리지 않는다.
- 논리적 살아 있는 scene 상한은 전체 3개, transferred outstanding 상한은 2개로 명시한다. 현재 코드의 각 Worker queue≤2와 다른 전역 계약이므로 테스트·메모리 예산도 함께 바꿔야 한다. 단순히 각 queue 검사만 통과시키지 않는다.
- `sceneSequence`, transfer ID, immutable metrics, context/surface generation, resource retain/release 순서를 보존한다. superseded도 terminal과 buffer 반환은 정확히 한 번이다.
- Raster 동기 replay는 중간 취소할 수 없다. 선행 메시지가 queued 상태에서 낡을 수 있으므로 message task와 microtask drain의 순서를 실험한다. 매 장면마다 추가 rAF를 넣어 대기를 늘리지 않는다.
- UI/Raster stall, restart, malformed message에서도 장면·buffer가 유실되지 않아야 한다. 이 조건을 만족하지 못하거나 end-to-end latency가 나빠지면 기존 전송으로 되돌린다.

### C. UI와 Raster의 실제 작업량 감소

- UI: framework build/layout/paint, CanvasKit paragraph 측정, mapper, encoder, JS 복사 시간을 분리한다. width 의존 paragraph layout을 무효화하지 않고 재사용하는 잘못된 cache는 금지한다.
- 데이터: `bytes.slice()` 및 pool copy를 소유권/수명과 함께 계측한다. managed 메모리가 다음 호출에 재사용되는지 확인 없이 view를 그대로 transfer하지 않는다.
- Raster: paragraph/paint/filter cache의 hit·miss, decode와 replay를 분리하고 기존 cache를 먼저 점검한다. 크기 변화가 실제로 영향을 주지 않는 resource의 재생성을 줄인다.
- 진단: 매 이벤트의 전체 diagnostics 객체 구성·postMessage 비용을 비교한다. bounded ring buffer와 명시적 trace mode를 사용하되 correctness counters와 terminal 기록은 생략하지 않는다.
- capacity: 초기 screen 크기와 DPR2/다중 모니터에서 GPU 메모리 비용을 기록한다. GPU 한계와 예산을 고려한 성장 상한을 마련하고 shrink 재할당은 active resize 밖에서만 별도 검증한다.

### D. 표시 방식 변경은 조건부 비교안

A~C 이후 UI/Raster 제출은 충분히 빨라졌는데 실제 표시 추종만 뒤처질 때, worker-owned offscreen raster → ImageBitmap → main visible canvas를 별도 opt-in 실험한다.

main이 완성 bitmap에 맞는 CSS geometry와 표시를 한 callback에서 반영하도록 설계할 수 있지만 compositor의 원자적 표시 보장은 아니다. main thread 부하, snapshot/transfer 비용, 추가 GPU 메모리, 누락 bitmap close를 측정한다. 기존 transferred visible canvas의 context를 즉석에서 바꾸지 말고 host lifecycle에서 새 canvas로 초기화한다. context-loss·restart 계약도 함께 검증한다.

고정 DPR에서 이전 front는 1:1로 유지하고 root가 clip한다. 이미지 전체를 늘리거나 예측 크기로 layout하여 최신 크기 일치로 계산하지 않는다. 늘어난 영역을 채우는 정책은 콘텐츠가 실제로 갱신된 것과 별개로 보고한다. A~C로 목표를 만족하면 이 구조 변경은 `notNeeded`로 종료한다.

## 6. 작업 순서와 단계별 판정

아래 표는 최초 단계별 완료 조건이다. 실제 실행 상태는 9절과 실행 보고서를 따른다. 앞 단계의 근거 없이 여러 정책을 동시에 바꾸지 않는다. 각 단계는 변경 SHA, 설정, baseline, 결과 JSON, 채택/기각 이유를 남긴다.

| 단계 | 작업 | 진행/완료 조건 |
| --- | --- | --- |
| R0 | 실행 산출물과 renderer 확인, stage trace 추가, 동일 resize baseline 3회, Flutter 비교 harness 확장 | size trajectory·시계·renderer·GPU·빌드 정보가 기록되고 누락/중복 terminal 없이 병목 구간을 구분할 수 있다. baseline 성능이 나쁜 것은 R1 진입을 막지 않는다. |
| R1 | 30fps gate만 제거하고 single-rAF/latest metrics 계약 유지, throttle assertion 교체 | correctness PASS, active 구간 latency/화면 나이 개선 및 CPU/input 퇴행 없음. 효과가 작으면 그 결과를 남기고 비용 분해로 진행한다. |
| R2 | metrics+snapshot 중복 처리 및 상위 UI/encode/copy/진단 비용 최적화 | UI p95 비용과 end-to-end 개선을 함께 확인한다. 정확한 scene identity, text wrapping, input/IME geometry 회귀가 없다. |
| R3 | ACK 왕복이 병목일 때만 credit 2 전송 실험, global queue/resource 계약 보강 | stall/restart/순서/정확히 한 번 terminal PASS 후 latency 개선 시 채택. 병목이 아니면 `notNeeded`, 실패 시 rollback하고 이유를 기록한다. |
| R4 | overload/progressive admission과 capacity/DPR 경계 조정 | 최신성·연속성·메모리를 함께 만족한다. latest-only starvation이나 과거 front 재노출로 해결하지 않는다. |
| R5 | 제출 이후 표시 지연이 남을 때만 bitmap presentation A/B | 동일 workload의 실표시·latency·메모리에서 이득이 있을 때만 채택한다. 아니면 direct 경로 유지 및 `notNeeded`/기각 사유 기록. |
| R6 | 최종 후보의 자동 회귀, Flutter 차등 비교, 사용자 직접 resize | 자동 판정과 실제 체감 판정을 분리하여 아래 완료 기준을 모두 충족한다. 실제 검증 전에는 완료로 표시하지 않는다. |

## 7. 측정 방법과 완료 기준

### 계측 정의

main 관측, UI 수신, frame start/end, encode/copy, scene send, Raster 수신/replay/flush, UI terminal 수신, main direct-commit 수신을 같은 scene/epoch ID로 연결한다. Worker마다 `performance.now()` 원점이 다를 수 있으므로 `performance.timeOrigin + performance.now()` 또는 검증된 offset으로 정규화한다. 시계 정밀도 제한도 기록한다.

| 지표 | 정의·주의 |
| --- | --- |
| epoch 생성→해당 epoch GPU submit | 제출된 epoch에 대한 stage latency다. superseded epoch는 제외 건수를 함께 기록하며 성공 sample만으로 전체 추종을 설명하지 않는다. |
| visible-content age proxy | 각 관측 tick의 현재 front가 사용한 metrics 관측 시점으로부터 지난 시간. 오래된 metrics로 재제출해도 나이가 0이 되지 않는다. 실제 scan-out은 별도다. |
| front update interval | 서로 다른 front generation의 submit 간격 p50/p95/max 및 active 구간 긴 정지 횟수. |
| size mismatch | target와 front의 logical width/height 절댓값 차이 및 불일치 면적의 시간 적분. generation 차이는 보조 지표다. |
| settle latency | 마지막 metrics 관측→그 metrics와 정확히 일치하는 최종 front 제출. |
| overhead | UI/Raster busy·idle, transfer bytes, scene/queue high water, live bitmap/buffer/resource, CPU/GPU 메모리와 input latency. |

기존 native 테스트의 `p95 < 200ms`, 일정 수 이상의 generation commit은 연속성 회귀 기준이다. 이를 통과했다는 이유로 실시간 추종 PASS를 주지 않는다. `page.setViewportSize()` 후 매번 settle을 기다리는 테스트도 연속 드래그 지연을 숨기므로 active trajectory 테스트와 분리한다.

### 공통 실험 조건

- Release build, Playwright bundled Chromium의 hardware WebGL, 같은 viewport trajectory/scene/DPR/GPU에서 baseline과 후보를 각각 3회 이상 측정한다. cold/startup과 warm 구간을 나누고 상세 진단 on/off 영향도 비교한다.
- 단순 모서리·중앙 정렬 도형 fixture와 현재 demo의 text/filter가 있는 workload를 모두 사용한다. Flutter fixture도 같은 배치·문자·wrap breakpoint를 사용한다.
- 느린/빠른 연속 확대·축소, 즉시 방향 반전, Right/Bottom/Left/TopLeft, 최대화/복원, initial capacity 초과, DPR 1/1.25/1.5/2, 브라우저 zoom·모니터 이동을 포함한다.
- 60Hz 기준을 먼저 확보하고 가능한 환경에서 120Hz도 측정한다. background/복귀와 UI/Raster 100ms stall은 정상 성능 corpus와 분리해 복구를 검증한다.
- Flutter는 실제 renderer(CanvasKit/skwasm), threading 활성 여부, isolation headers, SDK SHA를 기록한다. 다른 renderer 결과를 하나의 Flutter 수치로 합치지 않는다.
- CDP `Browser.setWindowBounds`와 `Doroti/eng/resize-window-native.ps1` 기반 headed 검증을 재사용한다. 자동 HWND 변경이 실제 mouse border drag와 같다고 선언하지 않는다.
- 기존 5088/5089 사용 중이면 프로세스를 임의 종료하지 않는다. 향후 wrapper에 base URL/port 옵션을 추가해 별도 owned server로 실행할 수 있게 한다.

### 제안하는 수치 gate

다음 값은 **미측정 목표값**이다. R0에서 환경과 corpus를 고정한 후 구현 전에 확정한다. 결과가 나쁘다고 사후에 완화하여 PASS로 바꾸지 않는다.

- 60Hz warm 일반 demo에서 epoch→submit p95 ≤33.3ms를 목표로 한다. 우선 후보 채택은 baseline 대비 p95 20% 이상 개선 또는 이 절대 목표 달성, p99/max와 size mismatch 적분의 퇴행 없음으로 판단한다.
- active resize의 front update interval p95 ≤33.3ms, 정상 부하에서 100ms 초과 정지 0회, 마지막 target settle ≤50ms를 목표로 한다. 과부하 corpus는 별도 표에 기록한다.
- 동일 endpoint로 계측할 수 있는 Flutter 비교에서는 Doroti p95가 Flutter보다 display interval 한 번 이상 뒤처지지 않는 것을 목표로 한다. framework callback과 GPU submit을 직접 비교하지 않는다. endpoint가 다르면 상대 gate는 `notComparable`로 두고 시각 캡처 기준 비교를 보강한다.
- 크기·DPR·surface identity 혼합, generation 역행, stale session commit, 중복 terminal, detached buffer 사용, 복구 후 outstanding 누수는 0이어야 한다.
- non-resize animation 및 입력 p95가 baseline보다 10% 초과 악화되면 재검토한다. trial variance와 절대 ms도 함께 제시한다. geometry/IME/focus regression은 허용하지 않는다.
- 리사이즈 중의 우측·하단 콘텐츠, 중앙 정렬, text wrap 이동을 실제 사용자가 확인해야 한다. 자동 제출/DOM/screenshot PASS와 사용자 드래그 PASS를 별도 칸에 기록한다.

## 8. 구현 시 검증 진입점

repository 지침 `.github/copilot-instructions.md`에 따라 **모든 테스트 실행은 20분 timeout**을 적용한다. 아래는 최초 계획의 검증 진입점이며, 실제 실행한 옵션과 결과는 실행 보고서를 따른다.

```powershell
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -FastResize -Port 5188 -ArtifactLabel resize-fast-native-baseline

pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -HeadlessOnly -TestFile tests/canvaskit-worker.spec.ts
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -RendererMode worker-canvaskit-webgl -HeadedOnly -TestFile tests/resize-continuity.spec.ts
```

`-FastResize`가 새 주 성능 진입점이다. Windows validator의 동일 입력 실행 파일을 빌드하여 headed Chrome 테두리를 실제로 드래그한다. 기본은 Right/Bottom/Left/TopLeft × expand/shrink/reverse × 150/600ms × 3회다. `DOROTI_FAST_RESIZE_EDGE`, `DOROTI_FAST_RESIZE_MOTION`, `DOROTI_FAST_RESIZE_MS`, `DOROTI_FAST_RESIZE_RUNS`로 진단 범위를 좁힐 수 있다. 실제 QPC 입력 시간과 native window excursion을 확인하며, 속도 조건 미달은 실패다. `-RequireLatencyGate`를 더하면 추종 성능 gate도 테스트 실패로 처리한다. 지정하지 않아도 결과 JSON의 `following.status`에 성능 PASS/FAIL을 기록한다.

`Doroti/validation/web-playwright`의 `npm.cmd run check`와 영향 범위에 맞는 DisplayList/worker protocol/input/TextField 테스트도 20분 제한 wrapper로 실행한다. 공통 host를 바꾸면 `auto`와 `worker-direct-webgl`도 회귀 확인한다. renderer 변경이 없는 문서 단계에는 이 실행을 요구하지 않는다.

`run-web-flutter-differential.ps1` 및 `flutter-differential.spec.ts`는 R0에서 renderer 선택, 연속 resize workload, 동등한 계측 endpoint, port 설정을 추가한 뒤 사용한다. 현행 버튼 비교 결과를 resize PASS로 재사용하지 않는다.

## 9. 현재 체크리스트

- [x] 현재 코드의 30fps gate, 최신 metrics 전달, mailbox, surface 재사용 조사
- [x] Flutter 로컬 소스와 공식 upstream/웹 API 자료 검토
- [x] 코드 근거, 가설, 변경 순서, 비교 방법, 자동/실제 판정 경계 작성
- [x] R0 stage trace, 느린 Doroti baseline 3회, 포트 분리, Flutter renderer 선택·연속 resize harness 실행 (역사적 느린 corpus)
- [x] R0 빠른 native 테스트 구현: Windows SendInput 240Hz / 600px / 150·600ms, 4방향·3동작 각 3회 유효 baseline 확보. 최초 matrix 23 PASS / 1 입력 FAIL, 해당 조건 별도 3회 통과. 실패 이력 보존
- [x] R1 빠른 TopLeft/150ms/reverse 교대 A/B 3쌍 재측정: 후보 p95 58.7 / 63.9 / 76.2ms, 일관된 개선 없음·성능 gate FAIL. 기본 정책 유지
- [ ] R0 동등한 Flutter fixture/endpoint, clock 정밀도 및 전체 corpus 확정 (`notComparable`/부분 완료)
- [x] R1 30fps gate 해제 후보와 single-rAF 계약 검증, 교대 A/B 3쌍; 기본 채택 gate FAIL
- [x] R2 mapping/encoding/paragraph/copy 비용 분해; enum boxing 제거; snapshot 단일 적용 실험 기각·rollback
- [x] R3 느린 corpus의 전송 대기 분해: encoded→send p95 약 0.1~4.8ms; 당시 credit 2 `notNeeded`. 빠른 corpus에서는 재판정 필요
- [x] R4 terminal 대기 기반 과부하 실험: UI 점유율 감소에도 latency 퇴행하여 기각·rollback
- [ ] R5 bitmap 표시 비교: 제출 latency 목표 미달로 진입 조건 미충족 (`deferred`)
- [x] R6 선택한 자동 회귀·trace on/off·headed TextField·native HWND resize 실행 (전체 corpus 완료 아님)
- [ ] R6 성능 목표, 동등한 Flutter 비교 및 사용자 직접 드래그 판정

최종 보고에는 개선 전/후 수치, 채택한 변경, 기각한 실험, 재현 가능한 artifact 경로, 실제 체감 판정을 남긴다. 수치 또는 물리 검증이 미완료이면 해당 항목은 `notVerified`로 유지한다.

# Web Worker direct renderer 구조 개편 요약

- 기준일: 2026-08-30
- 후속 최적화: 2026-08-31
- 대상: `Doroti.Host.Web`, `Doroti.Skia.Rendering`, Web Playwright, Flutter differential fixture
- 최종 상태: `implemented-qualification-failed`
- 제품 기본값: `auto=document-webgl`
- 신규 backend: `worker-direct-webgl` opt-in

## 결론

Worker에서 완성한 frame을 `ImageBitmap`으로 main thread에 전달하는 마지막 표시 경계를 제거하고, transferred visible canvas와 WebGL2 surface를 Worker가 직접 소유하는 `worker-direct-webgl` backend를 구현했다.

공용 scheduler와 frame identity는 기존 `SchedulerBinding → PlatformDispatcher → BrowserHostAdapter → SkiaSceneRenderer` 흐름을 유지했다. 별도의 Web 전용 semantic frame coordinator는 만들지 않았으며, Worker rAF와 direct surface는 기존 흐름 아래의 host transport/backend로 결합했다.

구조, correctness, resize, recovery와 대부분의 자동 검증은 통과했다. 그러나 current/direct A/B 3회 중 direct 한 실행의 max latency가 176.5 ms로 `<100 ms` gate를 실패했고 실제 입력·고주사율·scan-out·접근성 acceptance도 완료하지 못했다. 따라서 W8 default cutover와 legacy renderer 삭제는 수행하지 않았다.

## 주요 구현

- transferred visible canvas, Worker rAF, WebGL2 framebuffer 0과 managed Skia surface를 결합한 direct renderer를 추가했다.
- protocol v2 runtime decoder/state machine과 DOM, surface, Worker host, diagnostics module을 분리했다.
- malformed envelope은 fail-closed 처리하고 Worker fatal 시 canvas/host를 교체하는 supervisor 1회 재시작 계약을 구현했다.
- input sequence를 main ingress부터 managed dispatch까지 보존하고, scene terminal과 causal paint receipt를 구분했다.
- terminal ledger를 active frame, 누적 counter와 bounded recent history로 바꿔 장기 실행 시 무한 증가를 제거했다.
- Worker surface generation, context generation, resize target generation과 metrics generation의 owner를 분리했다.
- exact resize, DOM/input/IME/semantics endpoint 재바인딩, context loss/recovery를 direct 경로에 연결했다.
- DPR geometry는 main DOM이 logical CSS 크기를, Worker가 physical backing 크기를 소유하도록 고정했다.

## 단계별 판정

| 범위 | 판정 | 결과 |
| --- | --- | --- |
| W0 | `PASS` | 기존 renderer Release baseline과 Flutter matching fixture/differential harness를 고정했다. |
| D0 | `PASS` | bounded terminal ledger, scene/receipt 진단 분리와 100,000-frame contract를 통과했다. |
| W1 | `PASS` | transferred canvas, Worker WebGL2/rAF, context recovery와 fatal replacement를 실제 browser에서 증명했다. |
| W2 | `PASS` | protocol v2, module 분리, supervisor restart와 runtime validation을 구현했다. |
| W3~W6 | `PASS` | direct backend, ordered input, typed terminal, generation, exact resize와 DOM parity/recovery를 구현했다. |
| W7 자동 | `partial/FAIL` | 전체 direct suite와 Flutter proxy gate는 통과했으나 A/B absolute max gate가 한 번 실패했다. |
| W7 물리 | `notVerified` | 실제 border drag, precision trackpad, 60/120 Hz 이상 scan-out, 한글 IME, 긴 Backspace와 screen reader를 실행하지 않았다. |
| W8 | `notStarted-by-gate` | default 전환, burn-in, compatibility 축소와 legacy 삭제를 시작하지 않았다. |

## 2026-08-30 자동 검증

- `run-web-direct-validation.ps1 -Configuration Release`: Skia/Web/Qt/WindowsAppSdk/MAUI/Demo Release build, TypeScript, FCR-3/4/5/6/7과 resize contract `PASS`.
- direct headless 전체: 11 `PASS`, Flutter 조건부 1 `SKIP`; wheel p95 31.8 ms, max 62.4 ms.
- headed 자동 resize: Desktop Chrome bounds와 Windows native edge resize 2 `PASS`.
- 기존 renderer 회귀: document 8 `PASS`, offscreen-bitmap 8 `PASS`, offscreen-worker 10 `PASS`; 각 경로의 비해당 항목만 `SKIP`.
- current/direct A/B 3회 median p95: document 26.6 ms, direct 28.1 ms.
- direct max: 47.2 / 176.5 / 49.6 ms. 두 번째 실행 때문에 absolute max gate는 `FAIL`.
- Flutter differential proxy: Doroti direct p50 22.8 ms/p95 53.0 ms, Flutter p50 34.9 ms/p95 42.5 ms. `Flutter p95 + 20 ms` gate는 `PASS`지만 compositor scan-out ACK는 측정하지 않았다.

## 2026-08-31 빠른 native resize 후속 최적화

빠른 resize 중 full snapshot 전달, admission 왕복, exact 크기별 `SKSurface` 재생성, staging FBO clear/blit, 동기식 GPU 상태 조회와 Worker rAF 대기가 같은 raster 임계 구간에 겹치는 병목을 줄였다.

- resize epoch를 숫자형 fast path로 전달했다.
- main→Worker admission을 최대 4 in-flight와 replaceable latest slot으로 제한했다.
- 최신 resize가 이미 예약된 Worker rAF 뒤에 막히지 않도록 bounded task wake를 최대 2개 허용했다.
- transferred framebuffer는 grow-only capacity surface로 직접 감싸고 exact 영역만 clip했다.
- 축소 시 새로 숨겨지는 band만 scissor-clear했다.
- direct hot path의 staging FBO, framebuffer blit, 중복 flush와 동기식 `gl.getError()`를 제거했다.

후속 검증 결과:

| 범위 | 판정 | 결과 |
| --- | --- | --- |
| 640 px / 500 ms 이하 fast native resize | `PASS` | 3회 연속 target→caught-up p95/max 60 ms 미만 |
| headed visual proxy | `PASS` | 126 samples에서 scale/grid distortion과 검은 right/bottom band 0 |
| long Windows native resize | `PASS` | 126 targets의 generation/cadence/final-exact gate 통과; max latency 87.0 ms, cadence max 220.3 ms |
| Release headless resize regression | `PASS` | exact A-B-C, stale mailbox skip, pinch zoom, DPR2 포함 4/4 |
| Flutter paired resize 재측정 | `notVerified` | 같은 native resize 조건의 paired comparison을 다시 실행하지 않음 |
| 실제 border drag와 scan-out | `notVerified` | 사람 손의 방향별/모서리 drag와 물리 display 검증 미실행 |

fast 3회 target→caught-up 결과는 각각 p95/max 48.5/48.5 ms, 47.9/47.9 ms, 49.9/49.9 ms였다. combined run의 fast 결과는 p95/max 59.6/59.6 ms였고, long run은 p50 33.9 ms/p95 52.4 ms/max 87.0 ms였다.

이 수치는 application-side exact framebuffer submit/front-commit 기준이다. screenshot과 pixel proxy는 browser 내부 표시 결과이며 물리 compositor/monitor scan-out 증거가 아니다.

## 최종 정책과 남은 작업

- `worker-direct-webgl`은 opt-in으로 유지한다.
- `auto`는 계속 `document-webgl`을 선택한다.
- 실제 precision trackpad, 한글 IME, 긴 Backspace, screen reader, 60/120 Hz 이상 display와 사람 손 border drag acceptance를 완료해야 한다.
- 동일 fixture/native resize 조건의 Flutter paired comparison이 필요하다.
- W7 전체 gate가 통과한 뒤에만 default cutover, burn-in, compatibility 정책 확정과 legacy renderer 정리를 진행한다.

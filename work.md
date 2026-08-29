# Doroti Web 연속 스크롤·창 크기 조절 개선 작업 계획

- 작성일: 2026-08-29
- 실행 대상: `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web`
- 우선 대상: Windows Desktop Chrome / WebGL2 / browser-wasm
- 현재 판정:
  - 한글 font fallback: 사용자 확인 `PASS`
  - 외부 창 이동 시 TextField focus 해제, 복귀 후 재입력: 사용자 확인 `PASS`
  - 실제 trackpad 연속 스크롤: `FAIL`, 미해결
  - 브라우저 창 연속 resize: `FAIL`, 미해결
  - 간헐적 raster 깜빡임: `FAIL`, 재현 빈도가 낮아 원인 확정 전

## 목표와 완료 조건

이 작업은 build 성공이나 synthetic wheel 한 번으로 완료하지 않는다. 실제 trackpad와 창 테두리 drag에서 반응이 즉시 따라오고, 연속 렌더링 중 blank/old raster 깜빡임이 보이지 않는 것을 사용자 확인까지 받아야 완료한다.

| 영역 | 완료 조건 |
| --- | --- |
| wheel ingress | 각 DOM wheel sample을 같은 task에서 framework에 전달한다. 입력 delta를 host rAF에서 합산하거나 한 프레임 늦게 전달하지 않는다. line/page delta와 mouse/trackpad kind는 Flutter Web 기준으로 정규화한다. |
| scroll frame | rendering만 display rAF 기준으로 latest-only coalescing한다. 정상적인 steady scroll에서 frame queue depth는 2 이하, `failed=0`, stale present 0을 유지한다. |
| scroll latency | wheel timestamp부터 exact front commit까지 p95가 60 Hz 기준 2 refresh interval(33.4 ms) 이내이고 100 ms 이상 stall이 없어야 한다. 수치 통과 후에도 실제 trackpad 10초 왕복 scroll의 사용자 체감 `PASS`가 필요하다. |
| resize metrics | root `ResizeObserver`가 관측한 최신 logical size/DPR을 같은 callback에서 managed metrics로 알린다. 중간 generation은 build 전에 교체할 수 있지만, framework가 시작한 epoch는 immutable하게 판정한다. |
| resize present | resize observer가 진행 중인 Skia/WebGL raster의 framebuffer state를 변경하지 않는다. 새 exact staging frame이 준비된 시점에 backing store와 visible front를 함께 교체한다. drag 종료 후 2 refresh interval 이내에 최종 canvas backing size가 최종 physical target과 일치해야 한다. |
| flicker | 60초 continuous rendering, 30초 trackpad scroll, 30초 window resize 표본에서 blank/clear-only frame과 `failed` terminal이 0이어야 한다. 브라우저 내부 screenshot은 scan-out 증거가 아니므로 실제 화면 확인을 별도 수행한다. |
| 회귀 | 한글 glyph, IME 조합, 외부 창 focus 해제/복귀 입력, pointer/key/semantics가 기존 `PASS`를 유지한다. |

## 확인된 구조 차이와 작업 가설

1. Flutter Web은 wheel event를 개별적으로 즉시 framework에 전달하고, `scheduleFrame` 중복만 rAF에서 제거한다. Doroti의 기존 wheel delta 누적은 고해상도 trackpad sample의 cadence와 timestamp를 잃는다.
2. Flutter Web은 host element의 `ResizeObserver`에서 physical size를 다시 계산해 `onMetricsChanged`를 즉시 호출하고, canvas backing resize는 실제 frame render 직전에 수행한다.
3. Doroti의 기존 resize observer 경로는 exact front가 준비되기 전에 default framebuffer를 resize/blit할 수 있다. asynchronous managed raster와 같은 WebGL context state를 건드리면 resize 지연과 간헐적 raster 오염의 공통 원인이 될 수 있다.
4. 5088 runtime baseline 한 표본은 약 69.3초 동안 `present-requested=1039`, `submitted=486`, `superseded/ack=553`이었다. exact front 제출이 약 7 Hz에 머물러 wheel/resize보다 presenter 왕복과 backpressure가 먼저 병목일 가능성이 크다.
5. 현재 worktree에는 focus/font 수정과 함께 wheel, resize, synchronous presenter에 대한 미검증 변경이 포함되어 있다. 이 변경은 첫 검증 전까지 `PASS`가 아니며, 원인별로 나누어 검증하거나 실패 시 해당 실험만 되돌린다.

## 실행 순서

### S0. 변경 범위 고정과 baseline 재수집

- `git status --short`, `git diff --check`, 영역별 diff를 저장해 기존 사용자 변경을 보호한다.
- 현재 worktree를 Release로 build하고 5088 서버를 새 artifact fingerprint로 재시작한다.
- 10초 idle animation, 10초 실제 trackpad scroll, 10초 실제 window border resize를 각각 분리해 trace를 수집한다.
- 각 표본에 `wheel ingress → framework dispatch → schedule/admit → scene → raster → front commit`과 `resize observed → metrics → exact commit` timestamp를 남긴다.
- 현재 미검증 presenter 실험이 build/runtime에서 실패하면 다른 수정과 섞지 않고 presenter 변경만 rollback한 뒤 baseline을 다시 잡는다.

### S1. presenter 왕복과 frame cadence 수정

- `DorotiWebGlSurface`의 `RenderFrame` 안에서 다시 JS로 왕복하는 nested async interop를 제거한다.
- browser-wasm에서 가능한 synchronous managed callback 또는 동등한 한 번의 frame-boundary 호출로 `RenderFrame → exact blit → CompleteFrame`을 직렬화한다.
- framework의 여러 invalidate는 최신 pending scene 하나로 합치되, 이미 raster를 시작한 frame은 completion terminal을 정확히 하나 기록한다.
- steady animation에서 display refresh마다 최대 한 번만 raster하고, renderer가 감당 가능한 동안 불필요한 `superseded`가 누적되지 않는지 확인한다.
- 이 단계가 cadence/latency를 개선하지 못하면 synchronous interop 가설을 기각하고 변경을 되돌린 뒤 managed paint 시간과 Skia flush 비용을 별도로 profile한다.

### S2. Flutter 방식 wheel 처리

- DOM wheel sample을 rAF 누적 없이 즉시 managed `PointerDataPacket`으로 전달한다.
- `DOM_DELTA_PIXEL/LINE/PAGE`를 Flutter Web과 동일한 기준으로 pixel delta로 변환한다.
- mouse wheel과 continuous trackpad를 구분해 `PointerDeviceKind.mouse/trackpad`를 전달하되, 장치 판별이 불명확한 Firefox 결과는 별도 `notVerified`로 둔다.
- 입력 event마다 raster하지 않고 framework `scheduleFrame`만 한 rAF에 하나로 합쳐지는지 counter로 확인한다.
- scroll boundary에서 browser default를 허용할지 여부는 embedded/full-page 정책이 필요하다. 이번 MVP는 현재 full-page Demo 동작을 유지하고 iframe propagation은 후속으로 둔다.

### S3. Flutter 방식 resize 처리

- root `ResizeObserver`와 DPR watcher는 최신 metrics publish만 담당하게 한다.
- observer callback에서 canvas backing reset, framebuffer bind, clear, blit을 수행하지 않는다.
- 이전 preserved front는 새 exact staging frame이 끝날 때까지 유지한다. resize 중 CSS가 이전 front를 잠시 scale할 수는 있지만 blank buffer를 노출하지 않는다.
- 새 frame commit 시점에만 `canvas.width/height`, viewport, retained front generation을 동일 transaction으로 전환한다.
- 빠른 A→B→C resize에서는 A/B가 `superseded`, C만 `submitted`가 되는지, 최종 logical/physical/DPR 조합이 균일한지 확인한다.

### S4. flicker 원인 분리

- `preserveDrawingBuffer`, retained front/staging FBO, context loss 여부를 각각 trace한다.
- resize observer, presenter, Skia raster가 같은 WebGL state를 교차 변경하지 않는지 `glStateDirty` handoff를 한 owner로 제한한다.
- clear-only 화면은 exact frame이 없는 startup/context restore에서만 허용하고, 정상 continuous rendering에서는 금지한다.
- 자동 screenshot sampling과 terminal trace를 사용하되, 최종 판정은 실제 화면 녹화 또는 사용자 육안 확인으로 한다.

### S5. 검증과 문서화

- 모든 test/validation command는 repository 지침에 따라 20분 timeout을 적용한다.
- Release build:
  - `dotnet build ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release`
- focused validation:
  - `dotnet run --project ./Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -c Release`
  - `dotnet run --project ./Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release`
- 5088 Chrome runtime:
  - first content와 한글 glyph
  - ASCII/한글 입력, IME composition, 외부 창 focus 해제 후 복귀 입력
  - 실제 trackpad 연속 scroll과 scroll boundary
  - 창 테두리 연속 drag, maximize/restore, DPR/zoom change
  - 60초 continuous animation과 context loss/restore diagnostic
- Chrome 이외 Edge/Firefox, 60~165 Hz display matrix, 실제 배포 환경은 실행하지 못하면 `notVerified`로 남긴다.
- 완료 후 원인, 폐기한 실험, 최종 counter, 사용자 체감 결과를 `history/26-08-29/` 아래 dated 문서에 기록하고 이 파일의 활성 계획은 결과 요약으로 교체한다.

## MVP와 후속 경계

### 이번 MVP

- Windows Chrome에서 trackpad scroll과 window resize의 체감 지연 제거
- presenter frame cadence 정상화
- resize/raster WebGL ownership 분리
- focus/font/IME 회귀 방지
- 실제 화면 기준 flicker 재검증

### 후속

- iframe에서 scroll boundary의 parent propagation
- Firefox의 mouse/trackpad 판별 한계와 브라우저별 delta matrix
- 90/120/144/165 Hz display별 latency budget
- mobile browser virtual keyboard resize와 orientation change
- OffscreenCanvas/worker renderer 도입 여부

## 작업 중 판정 규칙

- build와 automated counter가 깨끗해도 실제 trackpad/resize가 버벅이면 `FAIL`을 유지한다.
- intermediate frame의 `superseded`는 latest-only 정책상 허용하지만, terminal 누락, queue depth 2 초과, stale/size-mismatch present는 허용하지 않는다.
- GPU blit/rAF 완료를 display scan-out acknowledgement로 부르지 않는다.
- 텍스트 focus 사용자 확인은 독립 `PASS`로 보존하며, scroll/resize `FAIL`을 가리지 않는다.
- 시각 실험이 악화되면 원인과 rollback 경계를 기록하고 마지막 사용자 확인 경로로 되돌린다.

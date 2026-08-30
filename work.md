# Web 실시간 창 resize를 Flutter식 exact-surface 경로로 전환하는 작업 계획

- 작성일: 2026-08-30
- 상태: `partial` — 2026-08-30 사용자 재확인 뒤 latest-metrics/progressive-exact 추격을 추가해 세 경로 Win32 active commit은 PASS. 최종 코드의 전체 renderer 회귀와 수정 후 실제 마우스 drag는 `notVerified`
- 대상: `document-webgl`, `offscreen-bitmap`, `offscreen-worker` 세 Web renderer
- 목표: 창 테두리를 드래그하는 동안 이전 frame을 새 종횡비로 늘려 보이는 preview를 제거하고, 최신 viewport metrics를 presentation 완료와 독립적으로 전달해 exact-size frame이 브라우저 resize를 실시간으로 따라가게 한다.
- 최종 판정: 자동화는 구조·계약·중간-frame 결함을 검사한다. 실제 마우스/트랙패드 창 resize에서 검은 영역, 비균일 stretch, 버벅이는 추격이 사라졌는지는 headed Chrome 실사용 관찰을 최종 gate로 둔다.

## 1. 문제 정의와 현재 HEAD 기준 원인

현재 문제는 단순히 WebGL backing 크기를 늦게 바꾸는 한 지점의 문제가 아니다. browser metrics, framework layout/raster, visible front의 세 timeline이 presentation ACK를 경계로 직렬화되어 생긴다.

1. `ResizeObserver`/DPR watcher가 새 target epoch를 관측한다.
2. exact frame이 아직 없으므로 `applyRetainedFrontPreview`가 이전 exact front의 CSS 크기를 새 viewport에 맞춘다.
3. target의 가로·세로 비율이 이전 front와 다르면 이전 content가 확대/축소되거나 crop되는 임시 화면이 보인다. 이 preview가 오래 남으면 사용자는 stretch 또는 zoom/crop으로 인지한다.
4. `emitResize`는 `managedResizeInFlightGeneration`이 끝날 때까지 다음 metrics 전달을 보류한다.
5. `offscreen-worker`는 main→worker snapshot mailbox까지 bitmap display 완료 뒤에야 해제한다. 따라서 metrics 전달이 layout/raster, `createImageBitmap`, thread transfer, `bitmaprenderer` commit의 전체 지연에 묶인다.
6. resize 중 만들어진 stale GPU front/ImageBitmap도 newer target의 preview로 소비된다. 이 동작은 화면이 최신 창 크기보다 한두 세대 뒤로 점프하는 현상을 만든다.

결과적으로 세 경로 모두 latest target을 즉시 받지 못하고, 특히 persistent worker는 두 개의 직렬화 경계 때문에 이전 frame preview 체류 시간이 가장 길어진다. 최종 exact-size 수렴과 queue depth/terminal 정상만으로는 이 결함을 검출할 수 없다.

## 2. Flutter 최신 Web 엔진에서 가져올 계약

Flutter Web의 full-page dimensions provider는 `visualViewport` 또는 window의 `resize`를 metrics 변경 source로 사용하고, 새 physical size를 DPR과 함께 계산한다. CanvasKit surface의 `setSize`는 canvas provider의 backing size를 갱신한 뒤 같은 physical size의 SkSurface를 다시 만든다.

- metrics 변경은 이전 frame의 presentation 완료를 기다리는 ACK가 아니다.
- canvas backing size와 SkSurface size는 하나의 physical-size 계약으로 갱신한다.
- retained frame을 새 target의 서로 다른 X/Y 비율로 재해석하는 정책을 framework resize 경로로 사용하지 않는다.
- 렌더링 비용은 scheduler가 coalesce하되, viewport state 자체를 오래된 presentation 뒤에 가두지 않는다.

참조 source:

- [Flutter full-page dimensions provider](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/view_embedder/dimensions_provider/full_page_dimensions_provider.dart)
- [Flutter CanvasKit surface size update](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/surface.dart)

Doroti는 Flutter 코드를 그대로 복제하지 않고 아래 기존 계약을 함께 보존한다.

- immutable resize epoch와 exact-size admission
- current + latest, 최대 depth 2 mailbox
- submitted/superseded/failed의 exactly-once terminal
- hardware WebGL2 fail-closed
- staging surface와 visible front의 소유권 분리
- worker 한 개가 소유하는 단일 .NET runtime
- `ImageBitmap.close()`/ownership terminal의 누락 0

## 3. 목표 아키텍처

```text
ResizeObserver + DPR watcher
  -> immutable latest ResizeEpoch를 즉시 publish
  -> managed metrics/layout scheduler는 latest-only로 coalesce
  -> renderer는 latest epoch와 정확히 일치하는 surface에 raster
  -> visible owner는 exact-size 결과만 commit
  -> raster 중 더 새 epoch가 오면 이전 결과는 Superseded
```

resize 관측 callback은 canvas backing/WebGL state/bitmaprenderer를 직접 reset하지 않는다. visible surface 변경은 renderer의 exact commit 지점 한 곳에서만 수행한다.

이 계획의 최종 상태에는 retained-front resize preview가 없다. 이전 front를 CSS width/height 또는 transform으로 새 target에 확대하는 방식은 임시 완화책일 뿐 완료 조건이 아니다. exact frame 전의 root 노출 영역은 현재 light/dark surface background로 채우고, exact frame cadence를 한 paint interval 수준으로 줄여 노출 자체를 보이지 않게 한다.

## 4. 실행 순서

### S0. baseline과 중간-frame oracle 고정

구현 전에 HEAD baseline을 세 renderer에서 같은 조건으로 기록한다.

- Chrome headed, 실제 top-level window, `viewport: null`
- 작은 창→큰 창→가로형→세로형→복원 순서로 40개 이상의 window bounds 변경
- 각 bounds 변경 직후와 다음 2개 animation frame에서 screenshot, DOM geometry, resize trace 채집
- renderer별로 아래 값을 기록한다.
  - target-observed→managed snapshot 전달 지연 p50/p95/max
  - target-observed→첫 exact front commit 지연 p50/p95/max
  - preview 체류 시간 p95/max와 preview frame 수
  - resize 중 exact front commit cadence
  - scaleX/scaleY 차이의 최대값
  - 오른쪽/아래 black 또는 root-background band의 최대 폭과 연속 paint 수
  - present request/terminal 일대일, failed 수, max queue depth
  - worker snapshot queued/sent/applied과 bitmap created/consumed/closed/active

자동 `Browser.setWindowBounds`는 repeatable oracle이며 실제 손으로 창 테두리를 끄는 acceptance를 대신하지 않는다.

### S1. metrics를 presentation ACK에서 분리

대상: `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`

1. `ResizeObserver`와 DPR watcher를 유일한 viewport authority로 유지한다.
2. 새 epoch가 관측되면 browser host snapshot을 즉시 managed 쪽으로 전달한다.
3. `managedResizeInFlightGeneration`을 “다음 metrics를 막는 lock”으로 사용하지 않는다.
4. 같은 browser frame의 중복 observer signal은 identical epoch로 제거하고, 서로 다른 epoch는 framework가 모두 볼 수 있게 한다.
5. layout/raster 요청 수는 기존 framework rAF owner와 latest callback이 coalesce한다. metrics 자체를 drop해서 raster 부하를 줄이지 않는다.
6. lifecycle/focus/configuration snapshot과 resize snapshot의 ordering을 보존하고, 오래된 snapshot이 최신 resize epoch를 되돌리지 못하게 한다.

Gate:

- 빠른 A→B→C resize에서 managed snapshot이 C 이전 frame의 present terminal을 기다리지 않는다.
- framework callback queue와 renderer mailbox depth는 각각 bounded 상태를 유지한다.
- 각 built scene의 epoch label은 생성 뒤 바뀌지 않는다.

### S2. retained-front resize preview 제거

공통 정책:

1. `applyRetainedFrontPreview`와 `resize-preview-commit` 제품 경로를 제거한다.
2. observer callback에서는 이전 front의 CSS width/height/transform을 target size에 맞춰 변경하지 않는다.
3. root는 `--doroti-surface-background`로 viewport 전체를 항상 덮어 브라우저 기본 검정 clear가 노출되지 않게 한다.
4. exact front commit에서만 canvas logical CSS size, physical backing size, front logical metadata를 한 JS turn 안에서 함께 갱신한다.
5. resize 중 생성된 stale surface/bitmap은 visible front를 교체하지 않고 `superseded` terminal로 닫는다.

`document-webgl`:

- 새 epoch 크기의 staging FBO에서 raster한다.
- staging이 exact인지 final admission에서 다시 검사한다.
- exact일 때만 canvas backing을 target physical size로 바꾸고 staging을 default framebuffer에 1:1 blit한다.
- backing reset과 exact blit 사이에는 `await`, managed callback, 별도 rAF를 두지 않는다.
- 이전 front/staging FBO의 GPU resource lifecycle과 context-loss replay를 유지한다.

`offscreen-bitmap`:

- detached OffscreenCanvas를 target physical size로 raster한다.
- `createImageBitmap` 뒤 target epoch를 다시 검사한다.
- exact bitmap만 visible canvas의 intrinsic/CSS size와 같은 commit block에서 `transferFromImageBitmap`한다.
- stale bitmap은 표시하지 않고 즉시 close하며 terminal accounting을 끝낸다.

`offscreen-worker`:

- main-thread visible canvas에는 exact bitmap만 transfer한다.
- main thread에서 더 새 epoch가 관측된 뒤 도착한 bitmap은 stale preview로 소비하지 않는다.
- stale bitmap의 close owner와 worker receipt를 하나로 고정해 created = consumed + closed, active = 0을 유지한다.

Gate:

- 세 renderer 모두 resize trace의 `resize-preview-commit` 수가 0이다.
- canvas computed style에 resize용 비균일 scale 또는 임시 uniform scale이 없다.
- exact commit 외의 코드가 visible canvas backing/CSS geometry를 바꾸지 않는다.

### S3. persistent worker mailbox를 metrics-admission 기준으로 변경

대상:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- `Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts`

1. main→worker snapshot mailbox의 ACK 의미를 “bitmap이 화면에 표시됨”에서 “worker .NET host가 metrics를 수신함”으로 바꾼다.
2. worker는 `dispatchWorkerSnapshot` 직후 해당 generation의 metrics-admitted receipt를 보낸다.
3. main은 receipt 뒤 대기 중인 latest snapshot을 즉시 보낸다. current + latest 외의 snapshot backlog는 만들지 않는다.
4. .NET scheduler callback, worker raster request, ImageBitmap receipt는 독립 terminal chain으로 유지한다.
5. raster 또는 bitmap 생성 중 최신 snapshot이 바뀌면 이전 request를 exact display로 승격하지 않는다.
6. worker crash/restart 때 snapshot mailbox, present request, bitmap resource의 미완 terminal을 모두 한 번만 닫고 최신 epoch를 replay한다.

Gate:

- worker-snapshot-sent N+1은 frame N의 bitmap display terminal 이전에 발생할 수 있다.
- 빠른 resize에서 snapshot delivery cadence가 bitmap transfer cadence에 제한되지 않는다.
- worker runtime count 1, restart count 계약, queue depth 2 이하, unpaired request 0을 유지한다.

### S4. Playwright headed 프로젝트를 live-resize gate로 확장

대상:

- `Doroti/validation/web-playwright/playwright.config.ts`
- `Doroti/validation/web-playwright/tests/resize-continuity.spec.ts`
- 필요한 경우 `tests/helpers/doroti-diagnostics.ts`

프로젝트 구성 변경은 허용된 범위로 보고 다음을 적용한다.

1. `desktop-chrome-headed`를 세 renderer에 대해 명시적으로 실행할 수 있게 artifact label과 renderer matrix를 정리한다.
2. final settled front만 검사하던 headed test를 1~2초 연속 bounds 변경 test로 확장한다.
3. bounds 변경 사이를 8~16 ms로 두고 각 중간 sample에서 다음을 검사한다.
   - canvas/root가 연결되어 있고 화면이 blank가 아님
   - exact commit 외 임시 CSS transform 없음
   - right/bottom edge의 pure-black band 없음
   - 이전 frame의 known grid/corner가 비균일하게 늘어나지 않음
   - runtime error/console error 없음
4. renderer별 screenshot/video/resize trace/diagnostics JSON을 항상 artifact로 남긴다.
5. 기존 final exact front, DPR 2, context loss, worker crash, input/semantics test도 함께 실행한다.

시각 oracle은 단순 색상 수보다 강하게 만든다. Demo grid와 네 모서리 marker를 이용해 수평/수직 cell 크기 비율을 비교하고, black/root-background strip의 폭과 연속 sample 수를 측정한다.

### S5. 계약·회귀 검증과 renderer 정책 판단

모든 test 명령 timeout은 20분으로 유지한다.

필수 자동 gate:

1. `dotnet build DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release`
2. TypeScript/Playwright type check
3. resize contract validation: epoch immutability, exact admission, depth 2, exactly-once terminal
4. FCR-7 Material/widget validation
5. renderer별 Playwright:
   - startup
   - resize continuity/headed live bounds
   - flicker/nonblank
   - DPR 2
   - context loss/restore
   - offscreen capability
   - worker protocol/crash recovery
   - wheel/input/semantics regression

headed/manual gate:

- document-webgl, offscreen-bitmap, offscreen-worker를 각각 실제 Chrome 창으로 띄운다.
- 좌/우/상/하/모서리 drag, 빠른 왕복, 큰 폭 확대/축소, maximize/restore를 수행한다.
- 검은 영역 0, X/Y stretch 0, resize 종료 뒤 뒤늦게 단계적으로 맞춰지는 현상 0을 사용자 관찰로 확인한다.
- 자동화가 모두 PASS해도 실제 drag가 실패하면 결과는 `FAIL`이다.

성능 기록:

- exact-front latency/cadence를 baseline과 동일한 장비·Chrome에서 비교한다.
- 평균만 쓰지 않고 p50/p95/max와 sample 수를 기록한다.
- compositor scan-out ACK는 Web API로 직접 증명할 수 없으므로 `browser-present-unverified` 경계를 유지한다.

renderer `auto` 선택은 이 수정과 분리한다. 세 경로 correctness와 visible gate가 통과한 뒤에도 worker/bitmap이 document 대비 p95를 실제로 개선하지 못하면 `auto=document-webgl`을 유지한다.

## 5. 완료 조건

다음을 모두 만족할 때만 완료로 판정한다.

- 세 renderer에서 retained resize preview 제품 코드 제거
- resize metrics 전달이 present/bitmap ACK에 의해 직렬화되지 않음
- exact-size surface만 visible commit
- stale front/bitmap display 0, failed terminal 0, unpaired terminal 0
- queue depth 2 이하, worker active bitmap 0으로 수렴
- Release build, resize contract, FCR-7, 관련 Playwright 전체 PASS
- headed intermediate-frame oracle에서 black band와 비균일 scale 0
- 실제 Chrome 창 테두리 drag 사용자 확인 PASS

실행하지 못한 실제 drag, Firefox/Edge/Safari, 60/120/144/165 Hz, DPR/monitor 전환은 `notVerified`로 남기며 다른 자동 결과로 대체하지 않는다.

## 6. 범위 제외

- trackpad wheel cadence 자체의 추가 최적화
- Web renderer `auto` 기본값 변경
- software/CPU renderer fallback 추가
- browser compositor scan-out ACK를 추정값으로 승격
- Firefox/Edge/Safari 전 브라우저 최적화
- Web 이외 Windows/MAUI/Android/Apple/Linux resize 경로 변경

## 7. 현재 working tree 경계

계획 작성 전에 원인 가설을 확인하기 위한 uncommitted draft가 Web TypeScript와 Playwright resize test에 존재한다. 이 draft의 uniform compositor preview는 stretch 완화 실험이며, 위 계획의 최종 목표인 preview 제거와 동일하지 않다. 계획 실행 시 HEAD baseline과 draft를 먼저 분리 검토하고, S0 evidence 없이 draft 결과를 완료로 승격하지 않는다.

## 8. 실행 결과 (2026-08-30)

### 8.1 구현

- `applyRetainedFrontPreview`, `resize-preview-commit`, `preview-front-refresh` 제품 경로를 제거했다.
- `managedResizeInFlightGeneration`/`managedResizePending` presentation lock을 제거하고 각 observer epoch를 `managed-snapshot-dispatched`로 즉시 전달한다.
- `document-webgl`과 `offscreen-bitmap`은 raster 전과 commit 직전에 현재 epoch가 정확히 일치할 때만 visible front를 교체한다.
- `offscreen-worker` main display는 현재 browser epoch와 정확히 일치하는 `ImageBitmap`만 소비하고 stale bitmap은 close한다.
- worker는 `dispatchWorkerSnapshot` 직후 metrics-admission receipt를 보내며 bitmap display terminal과 snapshot mailbox를 분리한다.
- headed project는 screenshot/video/trace를 항상 남긴다. 42 bounds × 변경 직후/다음 2 rAF = renderer당 126개 화면 sample에서 transform, grid/marker 비율, black/root band, terminal/resource 통계를 기록한다.
- Windows 보조 gate `Doroti/eng/resize-window-native.ps1`를 추가했다. 고유 Chrome HWND를 찾아 Win32 `SetWindowPos`로 좌/우/상/하/모서리를 12ms 간격, 180 step 연속 조절한다.

### 8.2 baseline

강화한 exact-only oracle을 기존 uniform-cover draft에 실행했을 때 세 renderer 모두 FAIL했다.

- `document-webgl`: 42개 resize epoch 모두 `resize-preview-commit` 발생
- `offscreen-bitmap`: 중간 sample에서 canvas cover/transform 계약 실패
- `offscreen-worker`: 중간 sample에서 canvas transform/preview 계약 실패

baseline screenshot/video/trace는 `Doroti/validation/web-playwright/artifacts/resize-exact/baseline-v1/<renderer>`에 남겼다.

### 8.3 126-sample headed Chrome 결과

| renderer | 결과 | metrics 전달 p95 | 첫 exact front p95 | preview | black band | scale/grid delta | queue | terminal/resource |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| `document-webgl` | PASS | 0.1ms | 77.9ms | 0 | 0px | 0 / 0 | max 1 | 68/68, failed 0 |
| `offscreen-bitmap` | PASS | 0.1ms | 106.5ms | 0 | 0px | 0 / 0 | max 1 | 68/68, bitmap 70=70+0, active 0 |
| `offscreen-worker` | PASS | 0.1ms | 114.6ms | 0 | 0px | 0 / 0 | max 2 | 68/68, bitmap 71=71+0, active 0 |

worker는 pure-black band와 scale 결함은 0이지만 exact frame 전 surface background가 오른쪽 최대 88px, 아래 최대 40px, 126 sample 중 26 paint에서 관측됐다. 이 값은 검정 clear가 아니라 `--doroti-surface-background`이며 최종 수동 체감 gate를 대체하지 않는다.

artifact는 `Doroti/validation/web-playwright/artifacts/resize-exact/final-live/<renderer>`에 renderer별 screenshot/video/trace/`live-resize-report.json`/diagnostics로 남겼다. compositor scan-out은 계속 `browser-present-unverified`다.

### 8.4 180-step Win32 연속 resize 결과

180회 `SetWindowPos` 요청을 성공 횟수로 사용하지 않고 실제 `target-observed`와 exact commit을 판정했다.

| renderer | 실제 browser epoch | active 구간 exact commit 세대 | preview | failed/unpaired | 판정 |
|---|---:|---:|---:|---:|---|
| `document-webgl` | 60 | 59 | 0 | 0/0 | PASS |
| `offscreen-bitmap` | 60 | 59 | 0 | 0/0 | PASS |
| `offscreen-worker` | 180 | 0 | 0 | 0/0 | **partial** — metrics는 계속 전진하지만 active resize 중 exact display는 종료 후까지 superseded |

worker trace는 다음 snapshot이 이전 present terminal보다 먼저 전달될 수 있음을 직접 확인했고, 최종 resource는 `created 7 = consumed 4 + closed 3`, active 0으로 수렴했다. 다만 “60개 이상의 입력/epoch”는 “60fps exact presentation”과 같지 않다. 특히 worker의 active 구간 exact commit 0은 실제 창 drag에서 content가 추격하지 못할 가능성이므로 완료로 판정하지 않는다.

### 8.5 자동 계약·회귀

- Release Web build: PASS, warning 0/error 0
- TypeScript/Playwright type check: PASS
- resize-contract v4: PASS, generated/terminal 22/22, max depth 2, stale present 0
- FCR-7 Material/widget: PASS. validator를 preview 필수 계약에서 exact-only/metrics-admission 계약으로 갱신했다.
- renderer Playwright headless:
  - `document-webgl`: 8 PASS, worker 전용 1 SKIP
  - `offscreen-bitmap`: 8 PASS, worker 전용 1 SKIP
  - `offscreen-worker`: 9 PASS, crash/restart 포함
- wheel 기록: document p95 27.1ms, bitmap 48.7ms, worker 47.0ms. 이 수정만으로 `auto` 선택을 바꾸지 않아 `auto=document-webgl`을 유지한다.

### 8.6 남은 완료 gate

- 실제 Chrome 마우스 창 테두리 drag: `notVerified`
- 실제 drag에서 worker의 종료 전 exact-front 추격: 현재 자동 evidence상 미달
- Firefox/Edge/Safari, 60/120/144/165Hz, DPR/monitor 전환: `notVerified`
- 따라서 5장의 전체 완료 조건은 아직 충족하지 않았으며 상태를 `partial`로 유지한다.

## 9. 사용자 재확인 뒤 후속 수정 (2026-08-30)

### 9.1 재현과 원인 보정

사용자가 세 renderer를 직접 확인한 결과 모두 창 크기를 늦게 따라오는 것으로 보였고, 8장의 exact-only 결과를 완료로 볼 수 없었다. 후속 trace에서 공통 원인과 worker 고유 원인을 분리했다.

- `BrowserHostAdapter`가 Web에서 `ILatestMetricsFrameHostCapability`를 구현하지 않아, 여러 metrics가 한 rAF에 coalesce되어도 framework build는 첫 signal에서 캡처한 오래된 epoch로 시작했다.
- Flutter Web full-page 경로는 `visualViewport.resize`, 구형 fallback은 `window.resize`를 사용한다. Doroti는 root `ResizeObserver`만 사용하고 있어 full-page source도 Flutter와 맞추지 못했다.
- worker는 managed raster가 끝난 뒤 `createImageBitmap` 중 새 metrics가 하나만 도착해도 해당 bitmap을 폐기했다. 180-step baseline에서 이 조건 때문에 active resize exact commit이 0이었다.
- exact-only를 “현재 browser target과 generation까지 같아야 함”으로 해석하면 비동기 pipeline 시간이 resize event 간격보다 긴 동안 영구 starvation이 발생한다. 후속 계약은 frame 자체의 immutable epoch/size에는 exact하고 visible front generation은 단조 증가해야 하며, 더 최신 metrics는 다음 bounded frame으로 추격하도록 보정했다. 비균일/균일 stretch preview는 다시 도입하지 않았다.

### 9.2 구현

- `BrowserHostAdapter`가 `ILatestMetricsFrameHostCapability`를 구현한다. rAF callback 직전에 `host.ViewEpoch`를 읽어 아직 시작하지 않은 framework work를 최신 immutable metrics로 admission한다.
- full-page metrics source에 Flutter와 같은 `visualViewport.resize`/`window.resize`를 추가했다. `ResizeObserver`는 owned root와 embedded-host 검증용으로 유지하며 동일 size signal은 기존 epoch dedupe가 제거한다.
- `offscreen-bitmap`과 `offscreen-worker`는 bitmap이 자기 descriptor 크기에 정확하고 현재 visible front보다 새 generation이면 진행 중 resize에서 progressive epoch-exact front로 commit한다. 최신 target과 같지 않다는 이유만으로 완성 bitmap을 버리지 않으며 최종 front는 최신 target으로 수렴한다.
- worker startup 중 focus/pointer input이 managed callback ABI 설치 전에 도착하는 race를 bounded startup mailbox로 닫았다.
- Win32 headed gate는 active resize 시간 안에 backing/surface가 같은 frame을 검사하고, 서로 다른 committed generation이 10개 미만이면 실패한다. 따라서 종료 뒤 한 번만 맞는 구현은 PASS할 수 없다.
- metrics callback 비용을 `managed-snapshot-completed`로 별도 기록해 browser observation, JS→managed 처리, framework/raster, visible commit을 분리했다.

### 9.3 최종 Win32 180-step 자동 결과

`SetWindowPos` 180회는 실제 browser epoch 수와 같지 않으므로 둘을 분리했다. 세 renderer 모두 active 구간에서 10개 이상 서로 다른 epoch-exact front를 commit했고 최종 `front == target`, failed 0, resource active 0으로 수렴했다.

| renderer | browser epoch | active committed generation | active commit p50/p95 | JS→managed snapshot p50/p95 | submitted/superseded/failed | queue/resource | 판정 |
|---|---:|---:|---:|---:|---:|---:|---|
| `document-webgl` | 64 | 64 | 45.0/84.7ms | 1.5/11.7ms | 104/0/0 | max 1 | PASS |
| `offscreen-bitmap` | 66 | 66 | 70.0/88.0ms | 1.3/10.9ms | 66/42/0 | max 1, bitmap 110=67+43, active 0 | PASS |
| `offscreen-worker` | 180 | 74 | 62.7/82.8ms | main enqueue 0.1/0.2ms | 74/1/0 | max 2, bitmap 78=75+3, active 0 | PASS |

artifact:

- `Doroti/validation/web-playwright/artifacts/final-instrumented/document-webgl`
- `Doroti/validation/web-playwright/artifacts/final-instrumented/offscreen-bitmap`
- `Doroti/validation/web-playwright/artifacts/final-instrumented-v2/offscreen-worker`

이 수치는 TS→managed 전달만이 주 병목이 아님을 보여 준다. document/bitmap의 snapshot callback은 보통 1~2ms이고 p95 약 11ms이며 managed Skia raster p50은 약 7ms였다. 남은 45~70ms p50 visible cadence에는 framework layout/build, rAF 대기, worker 왕복, ImageBitmap capture/transfer가 포함된다. TS에서 SkiaSharp backing size만 직접 바꾸면 framework layout이 이전 metrics에 남아 crop/빈 영역/후행 재레이아웃을 만들므로 적용하지 않았다.

### 9.4 후속 검증 경계

- Release Web build: PASS, warning 0/error 0
- TypeScript check: PASS
- FCR-7 source/runtime contract: PASS. latest-metrics admission, Flutter viewport source, progressive epoch-exact worker 계약을 재발 방지 항목에 추가했다.
- resize-contract v4: PASS, 22/22 terminal, max depth 2, stale present 0
- final `document-webgl` headless: 8 PASS, worker-only 1 SKIP
- final `offscreen-bitmap` 전체 headless: 사용자의 종료 요청으로 실행 중 중단, `notVerified`
- final `offscreen-worker` 전체 headless/context-loss/crash 회귀: 미실행, `notVerified`; Win32 active resize와 resource terminal은 PASS
- 수정 뒤 실제 마우스 창 테두리 drag 사용자 재확인: `notVerified`. 사용자가 “여전히 좀 느리게 맞춰진다”고 본 시점은 progressive-exact와 Flutter viewport source를 모두 넣기 전 중간 구현이었다.
- 따라서 자동 active-follow starvation은 수정했지만 60fps 성능이나 사용자 체감 완료를 주장하지 않는다. 현재 측정 p50 45~70ms는 추가 framework/build pipeline 최적화 여지가 있음을 명시한다.

## 10. 공통 managed-frame 지연 후속 수정 (2026-08-30)

### 10.1 재현과 실제 병목

9장의 progressive exact-front 수정 뒤에도 세 renderer가 함께 늦게 따라오는 이유를 180-step Win32 trace에 managed frame/scene/semantics 구간을 추가해 다시 분해했다.

- 수정 전 `document-webgl` 표본에서 `target-observed -> front-commit`은 p50 55.1ms, p95 112.6ms였고 managed frame은 p50 62.1ms, p95 94.9ms였다.
- managed Skia raster 자체는 p50 8.9ms였지만, geometry-only resize마다 47개 접근성 node 전체를 C# JSON과 DOM에 다시 적용했다.
- 브라우저 DOM 경로는 모든 ARIA attribute를 다시 쓰고 모든 action listener의 `AbortController`를 폐기/재생성했다. 이 구간만 p50 약 21ms였고 다음 resize/animation-frame callback과 경쟁했다.
- metrics activity 판정은 진단 trace의 forward-clamped timestamp를 재사용했다. browser frame clock이 managed stopwatch보다 앞선 경우 quiet 100ms가 끝나지 않아 semantics flush가 영구 보류되고 frame pump가 계속 도는 clock-domain 결함도 확인했다.

### 10.2 구현

- Web semantics 전송은 직전 node content와 비교해 내용이 같으면 `id`, `children`, `rect`, `contentUnchanged`만 보낸다. JSON null field는 생략한다.
- DOM은 content가 같은 node의 native tag, ARIA attribute, listener, semantics identifier를 보존하고 geometry style만 값이 바뀔 때 갱신한다. 축약 node의 생략된 `flags`로 text field를 `input`에서 `div`로 바꾸던 회귀도 차단했다.
- 연속 metrics 중 geometry semantics는 coalesce하고 quiet 뒤 최종 frame에서 한 번 flush한다. isolated resize는 즉시 처리한다.
- metrics activity의 시작/종료 판정은 host trace timestamp가 아니라 `DorotiFrameClock`의 managed 도착 시각을 사용한다. FCR-3에 미래 host timestamp가 있어도 100ms 뒤 activity가 종료되는 회귀 검사를 추가했다.
- Win32 headed gate는 마지막 bounds를 초기값과 다르게 유지하고, 마지막 `target-observed` 뒤 `semantics-dom-applied`가 없으면 실패한다. stale 접근성 geometry나 영구 frame pump를 성공으로 볼 수 없다.

### 10.3 181-step Win32 최종 결과

다음 latency는 실제 browser `target-observed`와 같은 epoch의 첫 `front-commit`을 연결한 값이다. 세 renderer 모두 active 구간 10세대 이상, 최종 `front == target`, preview 0, failed/unpaired 0, final-target 뒤 semantics flush 1회 이상을 만족했다.

| renderer | 실제 browser epoch | active committed generation | target -> exact front sample/p50/p95/max | final semantics | 판정 |
|---|---:|---:|---:|---:|---|
| `document-webgl` | 117 | 117 | 117 / 27.6 / 39.2 / 51.8ms | PASS | PASS |
| `offscreen-bitmap` | 145 | 145 | 145 / 24.9 / 34.1 / 80.5ms | PASS | PASS |
| `offscreen-worker` | 175 | 139 | 135 / 21.0 / 39.7 / 74.4ms | PASS | PASS |

artifact:

- `Doroti/validation/web-playwright/artifacts/semantics-clock-fix-document-webgl`
- `Doroti/validation/web-playwright/artifacts/semantics-clock-fix-offscreen-bitmap`
- `Doroti/validation/web-playwright/artifacts/semantics-clock-fix-offscreen-worker`

### 10.4 자동 검증과 남은 경계

- Release Web build: PASS, warning 0/error 0
- TypeScript/Playwright type check: PASS
- resize-contract v4: PASS, generated/terminal 22/22, max depth 2, stale present 0
- FCR-3 scheduler: PASS. metrics activity clock-domain 회귀 포함
- FCR-7 Material/widget: PASS
- Win32 native resize headed: 세 renderer 각 PASS
- semantics/pointer/keyboard/native text input: 세 renderer 각 PASS
- 전체 headless:
  - `document-webgl`: 7 PASS, worker-only 1 SKIP
  - `offscreen-bitmap`: resize/flicker/context/input/startup/capability 6 PASS, wheel-continuity 1 FAIL, worker-only 1 SKIP
  - `offscreen-worker`: resize/flicker/context/input/startup/capability/worker crash-recovery 7 PASS, wheel-continuity 1 FAIL
- offscreen 두 모드의 wheel FAIL은 각 synthetic wheel sample 뒤 5초 안에 새 front commit을 요구하는 대기에서 재현됐다. 6장의 trackpad/wheel 추가 최적화는 범위 제외이며 resize PASS로 덮지 않는다.
- 실제 Chrome 마우스 창 테두리 drag 체감: `notVerified`. 자동 지표는 개선됐지만 compositor scan-out, 실제 디스플레이 주사율, 사용자의 최종 체감은 자동 evidence로 대체하지 않는다.

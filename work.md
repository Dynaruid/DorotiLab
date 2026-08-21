# Web resize flicker 제거 작업계획

## 0. 문서 상태와 범위

- 상태: **retained front/staging FBO 구현과 contract 검증 완료. Chrome 수동 관찰은 정상이며, 600-sample 자동 live 판정과 최종 사용자 visible acceptance는 `notVerified`.**
- 사용자 관찰: Windows 빠른 resize는 현재 만족스럽지만 Web에서는 창 크기를 바꿀 때 화면이 깜빡인다.
- 목표: Web에서 interactive resize 중 마지막 정상 화면이 끊기지 않고 유지되다가 최신 exact-size frame으로 교체되게 한다.
- 이번 작업은 `Doroti.Host.Web`의 canvas/WebGL presenter와 Web live evidence에 한정한다.
- 현재 정상인 Windows DXGI/D3D12 1:1 buffer, DPI, latest-only 경로는 수정하지 않는다.
- 기존 worktree의 Windows 후속 변경과 기타 사용자 변경을 보존한다.
- 모든 자동 테스트 명령은 20분 timeout을 적용한다.

## 1. 현재 코드에서 확인한 사실

1. Web 크기 authority는 `.doroti-root`의 `ResizeObserver`와 DPR watcher로 정리되어 있다.
   - `observer.observe(root)`만 사용한다.
   - target sampling은 한 개의 `sampleRaf`로 coalesce한다.
   - 동일 physical size에는 `canvas.width/height`를 다시 쓰지 않는다.
2. presenter는 `current + latest`와 한 개의 rAF만 유지한다.
3. 하지만 새 physical size에서는 `runPresenter()`가 managed raster보다 먼저 다음을 실행한다.
   - `canvas.width = descriptor.physicalWidth`
   - `canvas.height = descriptor.physicalHeight`
4. canvas backing 크기 변경은 WebGL drawing buffer를 즉시 초기화한다. 현재 context는 `preserveDrawingBuffer: 0`이고, 이전 정상 frame을 별도 GPU surface에 보존하지 않는다.
5. 그 다음 `await callback.invokeMethodAsync("RenderFrame", ...)`가 끝나기 전까지 browser가 CSS root background 또는 초기화된 drawing buffer를 표시할 가능성이 있다.
6. 더 강한 race가 존재한다.
   - JS가 backing store를 이미 바꾼 뒤 managed `RenderFrame()`이 `_latestTarget?.Generation != generation`이면 아무것도 그리지 않고 반환한다.
   - JS는 이를 `target changed during raster`로 supersede한 뒤 다음 rAF를 예약한다.
   - 이 경우 새 backing store는 다음 exact frame까지 비어 있을 수 있다.
7. 공통 `SkiaSceneRenderer`는 마지막 정상 scene replay를 지원하지만, 현재 Web 경로에는 canvas reset 직후 그 scene을 보여 줄 retained presentation surface가 없다.
8. 기존 Web evidence는 build/publish와 일반 interaction만 증명한다. `resize and DPR transition`과 browser-visible flicker는 명시적으로 `notVerified`다.

## 2. 우선순위 가설

### H1 — backing-store reset 후 exact raster 전 공백

- 우선순위: 가장 높음.
- 예상 증거: `backing-store` trace와 exact `submitted` 사이 구간이 깜빡임 길이와 일치한다.
- 특히 managed generation reject가 발생한 구간은 `backing-store` 뒤 raster가 없고 다음 rAF까지 공백이 남는다.

### H2 — async JS→WASM interop가 reset과 raster를 서로 다른 browser presentation 기회로 분리

- 우선순위: 높음.
- `invokeMethodAsync`가 resolve되기 전에 browser compositor가 초기화된 canvas를 scan-out하면 exact Paint가 빠르더라도 한 frame flash가 생긴다.

### H3 — alpha/premultiplied canvas와 root background 차이가 공백을 더 눈에 띄게 만듦

- 우선순위: 중간.
- 배경색을 맞추는 것은 진단 수단일 뿐 최종 해결책으로 인정하지 않는다.

### H4 — ResizeObserver 또는 CSS layout 자체의 중복 신호

- 우선순위: 낮음.
- 현재는 root-only observer와 single sample rAF가 이미 적용되어 있다. trace가 중복 target authority를 다시 증명할 때만 이 분기로 돌아간다.

## 3. 완료 조건

- interactive resize 동안 마지막 정상 frame 또는 최신 exact frame 중 하나가 항상 보인다.
- backing-store 크기 변경과 같은 browser task 안에서 retained frame이 새 default framebuffer에 복원된다.
- 최신 exact generation만 front로 commit한다. stale generation은 보이지 않고 terminal은 정확히 한 번 기록된다.
- `current + latest` queue depth는 2 이하이고 무제한 frame/backing-store queue를 만들지 않는다.
- resize, DPR 변경, 0-size/restore, WebGL context loss/restore에서 generation이 역행하지 않는다.
- 제품 경로에 CPU `readPixels`, `toDataURL`, `getImageData`, full-frame bitmap snapshot을 넣지 않는다.
- canvas 숨김, opacity 전환, 임의 debounce/timer, `preserveDrawingBuffer: 1`, root 배경색 위장만으로 PASS하지 않는다.
- Windows resize contract와 build를 회귀시키지 않는다.
- 자동 trace와 실제 browser 영상/직접 관찰을 둘 다 통과해야 visible acceptance를 PASS로 바꾼다.

## 4. 실행 단계

### WEB-FLK-0 — 재현과 관측 기준선 고정

- [ ] 현재 Release Web runner를 실행하고 Chrome/Edge에서 빠른 window-border resize를 동일하게 재현한다.
- [ ] DPR, browser zoom, viewport 시작/종료 크기, GPU renderer, refresh rate를 evidence에 기록한다.
- [x] `doroti.web.ts` trace에 아래 phase와 generation/request ID를 추가한다.
  - `target-observed`
  - `backing-reset-start` / `backing-reset-end`
  - `retained-restore-start` / `retained-restore-end`
  - `managed-raster-start` / `managed-raster-end`
  - `front-commit`
  - `browser-present-observed` 또는 정확히 그 수준을 증명할 수 없다는 표식
- [x] `backing reset → exact front commit` 공백 시간, reset 뒤 raster 없이 supersede된 횟수, queue depth, terminal coverage를 집계한다.
- [ ] Chrome DevTools performance trace와 외부 screen recording 또는 CDP screencast로 실제 flash frame을 보존한다.
- [x] 진단 전용으로 reset 뒤 managed call에 인위적 지연을 넣어 flash 길이가 같이 늘어나는지 확인하고 즉시 제거한다.

실패 게이트:

- 깜빡임을 재현하거나 영상/trace로 reset 구간과 대응하지 못하면 제품 코드를 바꾸지 않는다.
- 인위적 지연이 깜빡임을 확대하지 않거나 backing reset 없이도 flash가 발생하면 H1 구현으로 진행하지 않고 CSS/compositor/alpha branch를 먼저 조사한다.

### WEB-FLK-1 — app-owned FBO 공유 spike

- [x] 단일 Emscripten WebGL2 context에서 app-owned framebuffer, color texture, depth/stencil attachment를 생성한다.
- [x] 해당 framebuffer의 native/Emscripten ID를 SkiaSharp `GRBackendRenderTarget`이 안정적으로 wrap할 수 있는지 작은 spike로 증명한다.
- [ ] FBO size 변경, dispose, context loss/restore 후 ID와 resource ownership이 유효한지 확인한다.
- [x] default framebuffer와 app-owned FBO 사이 `blitFramebuffer`가 GPU 경로로 동작하고 CPU readback이 0인지 기록한다.
- [x] spike resource가 현재 `presenterGlInfo()`와 `DorotiWebGlSurface.EnsureSurface()` 경계에서 어느 쪽 소유인지 문서화한다.

실패 게이트:

- native Skia가 app-owned FBO를 안정적으로 wrap하지 못하거나 context restore 뒤 resource ownership이 불명확하면 본 구현을 시작하지 않는다.
- 이 실패를 CPU bitmap snapshot, `preserveDrawingBuffer`, 매 resize 새 WebGL context로 우회하지 않는다.
- 대안은 별도 설계 검토 후 `managed-owned GPU offscreen surface` 또는 `two-canvas/two-context` 중 하나를 선택하고 비용과 cache/context-generation 영향을 다시 계획한다.

### WEB-FLK-2 — retained front + staging surface 구현

- [x] `CanvasPresenter`를 다음 상태로 명시한다.
  - `front`: 마지막으로 commit된 정상 generation과 GPU texture/FBO
  - `staging`: 현재 exact target을 raster하는 GPU texture/FBO
  - `latest`: 다음 target descriptor 하나
  - `current`: staging에서 처리 중인 descriptor 하나
- [x] 새 target rAF에서 canvas backing size가 바뀌면 managed await 전에 `front`를 새 default framebuffer에 즉시 stretch/blit한다.
- [x] retained blit와 backing reset은 같은 JS task 안에서 끝내며, reset만 된 상태로 browser에 제어를 돌려주지 않는다.
- [x] managed Skia는 default framebuffer가 아니라 exact-size `staging` FBO를 raster한다.
- [x] managed callback은 단순 `Task` 완료가 아니라 `exact-rendered`, `superseded`, `failed`를 구분할 수 있는 결과를 반환한다.
- [x] callback 완료 뒤 host의 최신 resize generation과 descriptor가 일치할 때만 staging을 default framebuffer로 blit하고 front/staging을 swap한다.
- [x] raster 중 target이 바뀌면 staging은 commit하지 않고 기존 front를 유지한 채 latest만 다음 rAF로 넘긴다.
- [x] front 교체, staging 재사용/폐기, backing-store mutation마다 `SurfaceGeneration`의 의미를 명확히 유지한다.
- [x] 한 generation의 terminal은 `submitted/superseded/failed` 중 정확히 하나만 기록한다.

불변식:

- `canvas.width/height` 변경 직후 default framebuffer에는 같은 task 안에서 front content가 존재해야 한다.
- `front.generation`은 단조 증가한다.
- `front` resource는 새 exact staging commit이 성공하기 전에 dispose하지 않는다.
- latest replacement는 current GPU resource를 강제 파괴하지 않는다.
- resize 중 보이는 stretched front는 허용하지만 background-only/transparent frame은 허용하지 않는다.

### WEB-FLK-3 — managed surface와 renderer 계약 정리

- [x] `DorotiWebGlSurface.RenderFrame()`의 generation mismatch 조기 반환이 blank backing store를 남길 수 없게 JS commit 순서와 결과 계약을 바꾼다.
- [x] `EnsureSurface()`가 staging FBO/size/context identity별로 wrapper를 재생성하고, front resource ownership과 섞이지 않게 한다.
- [x] `BrowserSkiaCapabilities.Paint()`가 exact/replay/superseded 결과를 JS presenter terminal과 중복 집계하지 않게 한다.
- [x] resize target과 exact scene이 아직 없을 때는 기존 front만 표시하고 빈 scene을 생성하거나 background clear를 commit하지 않는다.
- [x] initial startup에는 front가 없으므로 app-owned opaque background를 한 번 명시적으로 표시하고 첫 exact frame으로 교체한다.
- [ ] context loss 때만 front/staging을 모두 폐기하고, restore 뒤 마지막 immutable scene 또는 최신 exact scene으로 다시 구성한다.
- [ ] DPR-only 변경과 browser zoom에서도 logical/physical size 및 Skia 좌표 배율이 정확한지 확인한다.

### WEB-FLK-4 — 자동 live validator 작성

- [x] `Doroti/eng/validate-web-resize-continuity-live.ps1`를 추가하고 Release publish/serve/browser drive/evidence 수집을 한 명령으로 묶는다.
- [x] 각 subprocess와 browser 자동화는 20분 timeout을 가진다.
- [x] viewport를 일정 속도 triangle wave로 변경하고 최소 600개의 resize sample을 발생시킨다.
- [x] 최소 다음을 summary에 기록한다.
  - target 수와 generation 범위
  - backing reset 수
  - retained restore 수와 restore latency
  - exact front commit 수
  - stale front commit 수
  - blank exposure count/duration
  - current/latest queue high-watermark
  - submitted/superseded/failed/unterminated terminal 수
  - context/surface generation
  - console/page/managed exception
- [x] browser screenshot API를 제품 렌더 경로와 분리된 검증 수단으로만 사용한다.
- [x] current-source fingerprint와 raw trace, summary, recording 경로를 evidence에 남긴다.

자동 PASS 기준:

- `blankExposureCount == 0`
- `staleFrontCommitCount == 0`
- `generationRegressions == 0`
- `queueHighWatermark <= 2`
- `failed == 0`, `unterminated == 0`
- backing reset마다 retained restore 또는 같은 task의 exact commit이 존재
- resize 종료 후 최신 target exact commit 및 5초 내 정상 close
- browser console/page/managed exception 0

### WEB-FLK-5 — 회귀 검증

- [x] `validate-resize-continuity.ps1 -Shard Contract`
- [ ] `validate-web-product.ps1 -Shard Build`
- [ ] `validate-web-product.ps1 -Shard Publish`
- [ ] Chrome과 Edge에서 빠른 실제 window-border drag를 직접 확인한다.
- [ ] Firefox에서 WebGL2/FBO 동작과 visible continuity를 확인한다.
- [ ] DPR 1.0/1.25/1.5/2.0, browser zoom 80/100/125/150%, maximize/restore를 확인한다.
- [ ] DevTools CPU slowdown과 background/foreground 전환 후 resize를 확인한다.
- [ ] WebGL context loss/restore에서 마지막 scene 또는 최신 exact scene이 복구되는지 확인한다.
- [x] Windows Host Release build와 공통 resize contract를 다시 실행하되 Web 수정이 Windows visible acceptance를 대신한다고 주장하지 않는다.

## 5. 수정 예상 파일

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
  - presenter state machine, retained/staging FBO, backing reset/commit trace
- `Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor`
  - staging framebuffer wrapper와 managed render 결과 계약
- `Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs`
  - exact/replay/terminal 연동이 필요할 때만 수정
- `Doroti/src/Doroti.Host.Web/wwwroot/doroti.web.css`
  - 진단 결과 CSS/compositor 원인이 확인될 때만 수정; flicker 은폐용 변경 금지
- `Doroti/eng/validate-resize-continuity.ps1`
  - retained presenter의 구조적 불변식
- `Doroti/eng/validate-web-resize-continuity-live.ps1`
  - 새 browser live resize gate
- `Doroti/validation/evidence/web/`
  - current-source raw trace, summary, 영상/캡처 메타데이터
- `history/26-08-21/windows-web-resize-pipeline-summary.md`
  - 구현과 검증이 실제로 끝난 뒤 결과만 추가

## 6. 제외 사항

- Windows resize latency 추가 튜닝
- DemoApp 전용 loading overlay 또는 resize mask
- 임의 16/30/50/100 ms debounce
- canvas를 잠시 숨기거나 opacity를 바꾸는 방식
- `preserveDrawingBuffer: 1`만 켜는 방식
- CPU full-frame 복사/readback
- source fingerprint와 다른 오래된 browser evidence 재사용
- build/publish 성공을 visible flicker PASS로 간주하는 것

## 7. 종료 판정

### 2026-08-22 작업 중단점

- H1 기준선은 Chrome에서 확인했다. 진단용 2초 지연 시 `backing-reset`부터 `front-commit`까지 약 2,020,500 us의 공백과 blank 화면이 함께 나타났고, 진단 코드는 즉시 제거했다.
- app-owned front/staging FBO 경로를 구현했다. Chrome에서 `blitFramebuffer`의 source/destination status는 `36053`(`FRAMEBUFFER_COMPLETE`), GL error는 `0`이었고 CPU full-frame readback은 넣지 않았다.
- 820x500에서 1260x700으로 바꾼 수동 관찰에서는 backing reset 직후 retained restore가 약 600 us에 완료되고 이후 exact frame으로 교체되었다. 화면은 blank로 보이지 않았다. 다만 이는 Chrome 한 환경의 수동 관찰이며 browser compositor scan-out 자체를 증명하지 못하므로 `browser-present-unverified` 경계는 유지한다.
- `validate-resize-continuity.ps1 -Shard Contract`는 PASS했다. deterministic state machine은 `maxQueueDepth=2`, `stalePresents=0`, `terminalFrames=10/10`이었고 Web/Windows Host Release build도 이 shard 안에서 성공했다.
- `validate-web-resize-continuity-live.ps1`는 600-sample triangle-wave와 sample별 PNG 판정을 수행하도록 작성했지만, 전체 PNG 캡처가 예상보다 오래 걸려 사용자 요청으로 실행을 중단했다. 따라서 `blankExposureCount == 0`을 포함한 자동 PASS 기준 전체는 **`notVerified`**다.
- `web-resize-chrome-20260822-001935.*`와 `web-resize-chrome-20260822-002201.*`는 제품 continuity 결과가 아니라 validator bootstrap 실패를 기록한 진단 artifact다. 자동 PASS evidence로 사용하지 않는다.
- Edge, Firefox, DPR/zoom matrix, maximize/restore, CPU slowdown, background/foreground, WebGL context loss/restore, 별도 Build/Publish shard, 실제 window-border drag 및 최종 사용자 visible acceptance는 모두 **`notVerified`**다.
- 중단된 validator 전용 Chrome/Python 프로세스는 종료했다. `.doroti/tmp/web-flk-*` 임시 디렉터리는 실행 환경의 삭제 정책으로 자동 제거하지 못했으며 제품 산출물이나 evidence로 간주하지 않는다.

- 자동 contract/build/publish만 통과하면 상태는 `implemented, browser-visible notVerified`다.
- 자동 live trace와 browser recording에서 blank/stale commit 0을 확인하면 `automated continuity PASS`다.
- 사용자가 실제 빠른 drag에서 깜빡임이 사라졌다고 확인해야 최종 visible acceptance를 PASS로 기록한다.
- 어느 실패 게이트에서 멈췄다면 이후 항목은 실행하지 않고 `notVerified`로 남기며, 정확한 evidence와 다음 선택지를 문서에 기록한다.

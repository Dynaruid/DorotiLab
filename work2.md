# Doroti Windows F6-R·FG 남은 작업 계획

## 0. 문서 목적과 역사 경계

- 계획 기준일: 2026-08-25
- 대상: `Doroti.Host.WindowsAppSdk` FlutterEmbedder 제품 경로의 high-speed interactive resize와 최종 Windows 제품 acceptance
- Flutter protocol source: `reference/flutter-master` commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- active presenter 후보: single-front D3D12/DirectComposition + bounded client reservoir
- test timeout: 각 build/test command 최대 20분

2026-08-25까지 완료했거나 폐기한 F0~F10 구현, ANGLE/EGL·DComp 실험, four-edge 수치와 사용자 관찰은 [`history/26-08-25/windowsappsdk-flutter-host-resize-summary.md`](history/26-08-25/windowsappsdk-flutter-host-resize-summary.md)에 보존한다.

이 문서에는 앞으로 실행할 작업과 gate만 둔다. 역사 문서의 PASS, FAIL, `notVerified`를 새 작업의 성공으로 재해석하지 않는다.

## 1. 실행 순서와 hard gate

```text
R0 grid oracle correction
→ R1 native cadence factorial A/B
→ R2 single-front transient scene lifecycle
→ R3 content/GPU bottleneck correction
→ R4 paired resize matrix
→ FG physical/visible/deployment acceptance
→ release decision and legacy cleanup
```

- R0이 PASS하기 전에는 색상 fill 기반 `gap=0`을 content evidence로 사용하지 않는다.
- R1이 lag 증가 owner를 특정하기 전에는 thread priority, frame cap, timeout, debounce를 조정하지 않는다.
- R2가 네 edge expand를 통과하기 전에는 corner/shrink/reverse matrix로 확대하지 않는다.
- R4가 전체 PASS하기 전에는 FG 사용자 수용을 요청하지 않는다.
- FG 전체 PASS 전에는 Arm N/MAUI rollback을 삭제하거나 release를 확정하지 않는다.

## 2. 고정 architecture와 불변식

### 2.1 Native geometry

- Windows가 standard top-level caption, border, move, resize, Snap과 system menu를 소유한다.
- top-level client를 하나의 child render HWND가 항상 채운다.
- actual child physical width/height, DPI, display ID가 framework metrics authority다.
- `WM_WINDOWPOSCHANGING`은 관측과 비차단 transient 요청만 수행한다.
- proposed rect를 이전/prepared extent로 rewrite하지 않는다.
- mouse-up geometry replay나 renderer admission wait를 추가하지 않는다.
- WndProc는 metrics를 latest mailbox에 게시하고 즉시 반환한다.

### 2.2 Framework, raster, presenter

- platform STA, framework MTA, raster MTA, frame-clock MTA ownership을 유지한다.
- framework와 raster queue는 각각 `running 1 + latest pending 1`을 넘지 않는다.
- official framework metrics는 actual child generation만 사용한다.
- exact scene은 generation과 physical extent가 actual child와 모두 일치해야 한다.
- stale/wrong-size scene을 exact present나 exact terminal로 기록하지 않는다.
- compositor에 노출되는 root/front는 하나다.
- offscreen preparation resource는 허용하지만 두 visible front의 교대는 금지한다.
- scene body는 X/Y 1:1 픽셀 대응을 유지한다.
- first exact frame과 mouse-up final exact frame은 별도 hard terminal을 가진다.

### 2.3 Resource와 lifecycle

- backing/swap-chain capacity는 actual client + bounded transient reservoir에서 시작한다.
- monitor/work-area/virtual-desktop 크기의 permanent backing을 만들지 않는다.
- minimize `0×0`은 suspended로 처리하고 surface create/resize를 하지 않는다.
- context/device loss는 진행 중 exact/transient 작업을 terminal 처리한 뒤 새 generation으로 복구한다.
- work-area는 initial/restore/popup placement에만 사용한다.
- shutdown은 pending frame과 transient 상태를 terminal 처리하고 thread-affine resource를 해제한다.

## 3. F6-R 판정 모델

다음 축을 서로 독립적으로 판정한다.

| 판정 축 | 질문 | 주 증거 |
|---|---|---|
| Native cadence | 실제 window edge가 cursor를 제때 따르는가 | 1kHz cursor/window sampler, log-only |
| Uncovered coverage | black/transparent/uninitialized strip이 있는가 | 동일 raw capture frame의 native/client edge |
| Transient scene coverage | 마지막 1:1 scene이 client 어디까지 유효한가 | diagnostic grid tail, source extent/generation |
| Scale integrity | scene body가 X/Y로 늘거나 눌렸는가 | grid spacing X/Y, circle/title landmarks |
| Exact content | actual child와 같은 generation/extent가 present됐는가 | causal exact receipt + QPC-joined capture |
| Final convergence | mouse-up 뒤 transient가 제때 끝나는가 | `WM_EXITSIZEMOVE`→final exact latency |

`uncovered gap=0`은 exact scene coverage를 뜻하지 않는다. `safeFill`, `transientScene`, `exactScene`은 trace와 summary에서 별도 상태로 유지한다.

## 4. R0 — Grid oracle correction

### 목표

색상 fill이 client 끝까지 보이는 상태와 actual scene이 client 끝까지 raster된 상태를 자동 capture에서 구분한다.

### 작업

1. `DorotiDemoApp/src/App.cs`의 resize diagnostic overlay를 foreground scene으로 확정한다.
   - DPI-scaled 32 logical-pixel cyan grid
   - X/Y 원점 marker
   - right/bottom edge marker
   - X/Y spacing을 독립 검출할 수 있는 비대칭 corner glyph
   - safe-background sample 위치와 겹치지 않는 고유 색
2. 마지막 grid source 변경을 Windows Release로 build하고 initial/final 정지 capture에서 marker가 scene 전체에 1:1로 보이는지 확인한다.
3. `windows-resize-capture` frame schema에 다음 필드를 추가한다.
   - `detectedUncoveredLeftGap`
   - `detectedUncoveredRightGap`
   - `gridRightTail`
   - `gridBottomTail`
   - `gridSpacingX`
   - `gridSpacingY`
   - `gridNonUniformScaleRatio`
   - `gridOriginOffsetX/Y`
4. grid line 검출은 app-bar, 카드, 텍스트 색과 구분하고 여러 row/column의 periodic consensus를 사용한다.
5. horizontal resize에서는 right grid tail, vertical resize에서는 bottom grid tail을 기본 coverage oracle로 사용한다. left/top은 움직이는 screen origin과 child-local origin을 함께 기록한다.
6. DPI별 expected grid interval과 initial phase를 evidence에 저장한다. 정상 exact frame의 마지막 grid line→edge 잔여가 한 grid interval 미만인 점을 반영한다.
7. analyzer 명칭과 gate를 다음처럼 교체한다.
   - `contentEdgeGapAtMostOnePixel` → `uncoveredEdgeGapAtMostOnePixel`
   - `maximumContentGapDeltaPixels` → `maximumDetectedUncoveredEdgeGapDeltaPixels`
   - 신규 `gridTailP95/Max`, `gridSpacingDeviation`, `gridOriginDrift`
8. 역사 artifact의 옛 필드는 변환하지 않는다. 새 schema가 없는 artifact는 grid gate에서 `notVerified`로 처리한다.
9. log-only는 retained front 존재만 확인하고 exact visual coverage를 주장하지 않는다. grid/scale 판정은 raw capture에서만 수행한다.

### R0 PASS 조건

- initial/final exact 정지 frame에서 grid X/Y spacing 오차가 각각 1 physical px 이하
- foreground grid의 right/bottom edge marker가 client edge phase와 일치
- 의도적으로 safe fill을 노출한 fixture에서 uncovered gap과 grid tail이 서로 다른 값으로 검출됨
- full-frame X/Y stretch fixture에서 non-uniform scale oracle이 FAIL함
- left/top origin 이동 fixture에서 screen origin과 child-local grid origin을 혼동하지 않음
- raw frame 누락, encoder drop, oracle parse failure 0

## 5. R1 — Native cadence factorial A/B

### 목표

full product에서 남은 native cursor-edge lag가 처음 증가하는 owner를 surface, transient commit, framework, trace 중 하나로 좁힌다.

### 공통 조건

- 동일 binary revision, seed, initial rect와 monitor
- `600px/150ms` expand
- Left, Top, Right, Bottom 각각 10회
- capture off인 log-only를 primary timing으로 사용
- variant별 trace off/on paired run
- run 순서를 교차해 thermal/background bias를 줄임

### Variant

1. standard top-level + child, renderer/surface 없음
2. child DComp target + idle exact front, framework frame 없음
3. DComp target + bounded reservoir prepare/commit만 사용
4. framework latest-only + exact scheduler, transient 없음
5. full product DComp path

### 수집 항목

- cursor-edge lag p50/p95/p99/max/final
- native edge update interval p50/p95/max
- reverse/stall count
- platform WndProc dispatch p50/p95/p99/max
- framework/raster task count와 CPU time
- DComp `Present`/`Commit`, GPU fence wait count와 duration
- frame-clock `DwmFlush` count와 duration
- trace queue/drop과 trace on/off delta

### 판정

- 인접 variant가 p95를 4px 이상 또는 native interval을 1ms 이상 악화시키면 그 단계가 focused owner 후보다.
- 단일 run이 아니라 edge별 10회 분포와 paired delta로 판정한다.
- standard-child control이 현재 display-relative gate를 넘으면 같은 장비/입력에서 별도 재현 후 platform floor를 문서화한다. 한두 번의 outlier로 threshold를 완화하지 않는다.
- DComp/framework 단계에서 증가하면 ETW/WPA의 Input, Win32k, DWM, DXGI, CPU scheduling을 해당 variant에만 수집한다.

### R1 PASS 조건

- 모든 variant에 retry로 인한 두 번째 resize episode가 없음
- stationary opposite edge, required distance, reverse excursion contract PASS
- trace on/off observer overhead가 input duration/native p95 기준 10% 이하
- lag가 처음 증가하는 variant와 관련 thread/API wait가 반복 run에서 재현됨
- 원인 owner와 무관한 scheduling/priority 변경 없이 R2 수정 지점을 하나로 특정

## 6. R2 — Single-front transient scene lifecycle

### 목표

native geometry를 멈추지 않으면서 last exact scene 뒤의 large safe-fill tail을 latest useful 1:1 transient scene으로 줄인다.

### 상태 모델

```text
ExactScene(sourceGeneration, sourceExtent)
  → SafeFillPrepared(targetExtent)
  → TransientScenePrepared(sourceGeneration, sourceExtent)
  → TransientSceneVisible
  → ExactScene(finalGeneration, finalExtent)

prepared transient → Superseded | Failed
visible transient  → ReplacedByNewerTransient | ReplacedByExact | Failed
```

### 작업

1. 현재 reservoir fill을 coverage safety net으로 유지하되 `safeFill` 상태로 명시한다.
2. raster가 끝났을 때 actual generation보다 뒤처졌더라도 다음 조건을 만족하면 transient 후보로 보존한다.
   - current visible transient보다 source generation이 새로움
   - scene body와 source extent가 1:1
   - context/surface generation이 current presenter와 호환됨
   - exact ticket이나 semantics/input bounds로 승격하지 않음
3. offscreen에서 검증한 transient scene을 같은 DComp visible front에 atomic copy/commit한다.
4. trace에 source generation, source extent, target native extent, safe-fill tail과 replacement reason을 기록한다.
5. newest useful transient 하나만 유지하고 오래된 preparation은 GPU paint 전에 supersede한다.
6. exact ticket은 transient 작업보다 우선하며 mouse-up actual generation은 queue head로 승격한다.
7. stale-scene reuse만으로 grid tail gate를 통과하는지 먼저 측정한다.
8. 부족할 때만 bounded predictive lookahead를 별도 spike로 추가한다.
   - actual pipeline p95와 proposed velocity로 lookahead extent 계산
   - client + bounded maximum 안에서만 허용
   - official metrics, input, semantics, exact terminal 변경 금지
   - 예측 실패 시 supersede하고 native rect rollback 금지
9. exact scene present 시 transient clip/source metadata와 safe-fill 상태를 한 commit에서 종료한다.

### R2 PASS 조건

- visible DComp root/front 1개
- exact/transient/safe-fill terminal 누락과 중복 0
- stale/wrong-size exact present 0
- X/Y grid spacing 비균일 오차 2% 이하
- uncovered edge gap p95/max 1 physical px 이하
- grid tail p95 2 refresh 이동량 이하, max 3 refresh 이동량 이하
- grid origin drift, reverse content, flicker, black/white band 0
- native cadence가 R1 selected control보다 10% 이상 악화되지 않음
- mouse-up final exact 100ms 이내, transient 잔존 0

## 7. R3 — Content·GPU 병목 수정

### 목표

R2 뒤 남은 top/bottom final exact와 transient cadence 병목을 단계별 timing으로 줄인다.

### 작업

1. 다음 구간의 p50/p95/p99/max를 같은 causal generation으로 기록한다.
   - actual metrics publication→framework callback
   - callback→scene completion
   - scene→raster admission
   - Skia paint/flush/submit
   - GPU fence wait/copy
   - `Present(0)`→DComp `Commit`
   - mouse-up→final exact
2. top/bottom height growth가 left/right보다 느린 원인을 grid paint, backing growth, copy area, scheduler causal gap으로 분리한다.
3. backing 전체 clear/copy 대신 exact dirty viewport와 새 tail 갱신이 가능한지 검증한다.
4. capacity growth는 interactive hot path 밖에서 준비하거나 bounded offscreen replacement로 수행한다. native geometry를 기다리게 하지 않는다.
5. `DwmFlush`는 frame pacing과 post-exact ordering을 분리 계측한다. 대체 pacing이 입증되기 전 active path에서 제거하지 않는다.
6. scheduler의 causal gap, receipt mismatch, late exact race를 각각 terminal ledger로 닫는다.
7. content 최적화가 native cadence를 악화하면 즉시 revert하고 R1 owner 분석으로 돌아간다.

### R3 PASS 조건

- framework/raster queue high-watermark가 owner별 2 이하
- terminal gap, causal gap, receipt mismatch, trace drop 0
- platform resize p99 1ms 이하, max 4ms 이하
- final exact p95 100ms 이하이며 네 edge 간 최악값 차이가 원인 설명 없이 2배를 넘지 않음
- GPU/fence/commit wait가 WndProc에 전파되지 않음
- R2 grid/uncovered/scale gate 유지

## 8. R4 — Paired resize matrix

### 8.1 Four-edge baseline

동일 seed/start rect/input에서 각 edge의 log-only와 capture를 pair로 실행한다.

- Left, Top, Right, Bottom
- expand `600px/150ms`
- 각 3회 이상
- renderer delay 0과 50ms geometry control 분리

Four-edge baseline이 모두 PASS하기 전에는 다음 matrix로 진행하지 않는다.

### 8.2 Motion matrix

- 네 edge와 네 corner
- expand, shrink, immediate reverse
- `600px/150ms` 각 3회
- `600px/300ms`
- slow/fine drag
- minimum track size 근접
- work-area 근접 크기

### 8.3 Size, DPI, refresh

- initial logical size: 420×300, 640×360, 1000×600
- 가능한 DPI: 100%, 125%, 150%, 200%
- mixed-DPI monitor 이동과 resize
- 가능한 refresh: 60, 120, 144, 165Hz
- primary/secondary monitor와 음수 virtual-screen origin

### 8.4 Lifecycle

- minimize/restore
- maximize/restore
- Snap/unsnap
- fullscreen enter/exit
- occlusion/unocclusion
- sleep/resume
- display disconnect/reconnect
- RDP attach/detach
- context/device loss와 recovery
- resize 중 close/shutdown

### R4 PASS 조건

- active cursor-edge lag p95 ≤ 1 display-refresh 이동량
- active lag max ≤ 2 refresh 이동량
- final lag ≤ 1 physical px
- renderer 50ms fault에서 2 refresh 이상 native stall과 mouse-up jump 0
- uncovered gap p95/max ≤ 1 physical px
- grid tail p95 ≤ 2 refresh 이동량, max ≤ 3 refresh 이동량
- grid X/Y non-uniform scale ≤ 2%
- reverse/flicker/blank/black band 0
- queue overflow, stale exact present, terminal gap, causal mismatch 0
- mouse-up final exact ≤ 100ms
- capture/log-only native latency delta ≤ 10%
- raw active frame/encoder drop 0

## 9. FG — Windows 제품 acceptance

R4 PASS binary 하나만 FG에 사용한다.

### 9.1 Focused automated regression

- F2 top-level/child structural gate
- selected DComp presenter surface/lifecycle gate
- F6 scheduler/causal validator
- F7 input/IME/UIA automated regression
- Windows Release build
- clean publish and restricted-PATH launch
- generated template WindowsAppSdk launch

각 command에 20분 timeout을 적용한다.

### 9.2 Physical/visible acceptance

- 사용자가 네 edge/corner fast/slow resize의 border, grid, content continuity를 확인
- Korean IME composition/candidate/caret
- pointer/capture/cursor/focus/popup
- Narrator와 Accessibility Insights
- mixed-DPI monitor 이동
- minimize/maximize/Snap/fullscreen
- sleep/resume와 가능한 RDP

자동 evidence와 사용자 확인은 별도 provenance로 저장한다.

### 9.3 Deployment negative matrix

- Windows App Runtime absent/update
- native DLL missing
- wrong architecture
- corrupt/incomplete publish
- DispatcherQueue shutdown fault
- context/device recovery failure
- MAUI rollback live launch
- ArmNLegacy rollback live launch

### 9.4 Release 결정

다음이 모두 충족된 뒤에만 release cutover와 cleanup을 별도 승인한다.

- R4 전체 PASS
- focused automated regression PASS
- physical/visible user acceptance PASS
- negative deployment 결과 기록
- rollback launch PASS
- open `notVerified`가 release 범위와 충돌하지 않음

cleanup 승인 전에는 Arm N, MAUI, old validation source와 rollback selector를 삭제하지 않는다.

## 10. Evidence 규격

각 run은 다음을 기록한다.

- source revision과 dirty state
- binary/package fingerprint
- adapter/presenter identity
- GPU adapter와 software fallback 여부
- monitor rect, DPI, refresh, resolution
- run ID, seed, initial/final rect, edge, motion, speed
- log-only/capture paired ID
- input sample과 native geometry timeline
- safe fill/transient/exact source generation과 extent
- grid spacing/tail/origin과 uncovered edge
- framework/raster/GPU/DComp timing
- queue high-watermark와 terminal ledger
- raw capture, derived summary/contact sheet, 사용자 판정의 별도 provenance

WGC는 change-driven capture provenance로 사용하고 display-refresh native timeline과 혼합하지 않는다. build/process/contract 성공을 visible/compositor/physical PASS로 승격하지 않는다.

## 11. 금지 조건

- prepared scene 전까지 native `WINDOWPOS` 고정
- mouse-up geometry replay
- WndProc의 framework/GPU/surface/present/commit wait
- native window geometry 60fps 또는 임의 cadence 제한
- final-only debounce나 framework 60Hz 고정
- full-frame `SetSourceSize` X/Y stretch
- safe fill/repeated edge를 exact content로 계상
- transient를 exact present나 exact terminal로 기록
- dual visible front
- monitor/virtual-desktop permanent backing
- edge별 좌표 보정으로 ownership 문제 은폐
- timeout 증가만으로 terminal 지연 숨김
- 자동 capture 한 방향 PASS를 네 방향/제품 acceptance로 확대

## 12. 정확한 재개 지점

다음 작업은 R0 하나로 제한한다.

1. foreground diagnostic grid와 edge marker source를 완성한다.
2. Windows Release build를 실행한다.
3. initial/final 정지 capture로 grid phase와 X/Y spacing을 확인한다.
4. native observer와 analyzer에 grid-tail schema/gate를 구현한다.
5. safe-fill fixture와 full-stretch negative fixture로 R0 oracle을 검증한다.
6. R0 결과와 exact resume point를 이 문서에 반영한 뒤 R1 진입 여부를 판정한다.

R0 중 native presenter 동작은 수정하지 않는다. oracle이 fill과 exact scene을 구분하는 것이 먼저다.

# Doroti Windows resize ownership 재설계 계획

## 0. 문서 목적과 역사 경계

- 계획 기준일: 2026-08-25
- 대상: `Doroti.Host.WindowsAppSdk` Windows 제품 경로의 interactive resize ownership 재설계와 최종 acceptance
- 핵심 사용자 evidence: R1의 다섯 variant 모두 직접 보았을 때 떨림이 있었음
- test timeout: 각 build/test command 최대 20분
- 문서 상태: **계획 수정만 완료**. 이 개정으로 새 architecture 구현, build, runtime, visible PASS를 주장하지 않는다.

2026-08-25까지 완료했거나 폐기한 F0~F10 구현, ANGLE/EGL·DComp 실험과 four-edge 결과는 [`history/26-08-25/windowsappsdk-flutter-host-resize-summary.md`](history/26-08-25/windowsappsdk-flutter-host-resize-summary.md)에 보존한다.

기존 R0/R1 source와 evidence는 삭제하지 않는다. 다만 사용자 visible FAIL로 전제가 무너진 기존 `R2 single-front transient → R3 GPU 최적화` 경로는 **superseded / notStarted**로 종료한다. safe fill, stale-scene reuse, dirty-copy 최적화는 새 ownership 구조가 visible gate를 통과한 뒤에만 재평가한다.

## 1. 현재 판정

### 1.1 보존할 자동 evidence

- **R0 grid oracle: PASS**
  - foreground grid, right/bottom marker, uncovered gap과 grid tail 분리, stretch/origin fixture를 구현했다.
  - 최종 evidence: `Doroti/artifacts/windows-resize-grid-oracle/r0-20260825-191626-0d6eb68daea14097b0e39a1ab12cc176/r0-validation.json`
  - 이 oracle은 새 prototype에서도 재사용하지만, 자동 grid PASS를 visible smoothness로 승격하지 않는다.
- **R1 factorial: FAIL — owner not isolated**
  - evidence: `Doroti/artifacts/windows-resize-r1-factorial/r1-20260825-190055-64b6aa6c53824cdc901d97bf89382a1f/r1-validation-reanalyzed.json`
  - 5 variant × trace off/on × 4 edge × 10회, 총 400 episode의 drive contract failure와 observer overhead failure는 0이었다.
  - trace-off lag p95 median은 모든 variant가 `26~29px`에 모였고, 인접 단계가 `4px` 또는 `1ms` 이상 악화되는 owner를 찾지 못했다.
  - GPU fence wait는 supplemental에서 전 variant 0이었다. renderer/GPU wait만 줄이는 작업은 현재 공통 떨림의 근본 수정 근거가 없다.

### 1.2 사용자 visible 판정

- 사용자가 R1 test window를 직접 보고 **모든 variant에서 떨림을 확인**했다.
- 이 판정은 다음을 의미한다.
  - variant 3 reservoir, variant 4 framework/exact scheduler, variant 5 full product 사이의 차이를 찾는 기존 방향은 visible 문제를 분리하지 못했다.
  - renderer/surface가 사실상 없는 control에서도 떨림이 보였으므로 product raster만 고쳐서는 충분하지 않다.
  - 자동 `26~29px` 분포가 비슷하다는 사실은 smoothness PASS가 아니라 모든 variant가 같은 공통 geometry/cadence 경로에 묶였다는 증거다.
- 현재 Windows resize 상태는 **visible FAIL**이다. R2~R4와 FG의 과거 `notStarted`를 PASS로 재해석하지 않는다.

## 2. 근본 원인 가설과 architecture 결정

### 2.1 폐기할 전제

기존 제품은 다음 두 visible owner를 동시에 사용한다.

```text
USER32/DWM
  └─ standard top-level border와 실제 HWND geometry

Doroti raster/DirectComposition
  └─ child content, safe fill, exact scene과 present cadence
```

두 경로는 동일 output frame에서 atomic하게 바뀐다는 API 계약이 없다. `DwmFlush`, latest-only queue, safe background, faster paint, thread priority는 각 경로의 지연을 줄일 수는 있어도 border와 content를 하나의 visible transaction으로 만들지 못한다.

따라서 다음 전제를 폐기한다.

- standard moving HWND edge를 유지한 채 content만 더 빨리 따라가면 근본 떨림이 사라진다는 전제
- R1 variant 사이의 작은 수치 차이만 찾으면 visible owner가 결정된다는 전제
- safe fill tail 감소가 native border cadence를 고친다는 전제
- 더 많은 factorial run, timeout, debounce, priority 조정이 architecture 결정을 대신할 수 있다는 전제

### 2.2 primary architecture: Owned Geometry Envelope v2

새 primary 후보는 resize 중 USER32 top-level geometry를 움직이지 않고 visible window geometry 전체를 Doroti가 소유한다.

```text
fixed interaction envelope HWND (WS_POPUP, active monitor/work-area scope)
└─ one DirectComposition root visible
   ├─ app-owned border/caption/buttons
   ├─ app-owned client clip and offset
   └─ one visible content front

hidden preparation resources
└─ next exact/procedural scene backing; simultaneously visible하지 않음
```

핵심 ownership은 다음과 같다.

- interactive resize 중 envelope HWND의 screen rect는 고정한다.
- cursor가 결정한 `OwnedWindowRect`가 border, caption, client clip, content offset과 input/semantics bounds의 단일 geometry authority다.
- visible border/chrome/client clip/content front는 한 DirectComposition root transaction으로 commit한다.
- hidden backing/front slot은 허용하지만 compositor root에 연결되는 visible front는 항상 하나다.
- envelope 크기와 render backing 크기를 분리한다. monitor-sized envelope를 사용해도 D3D12/Skia backing은 owned client + bounded reserve만 가진다.
- idle 상태에서는 exact `SetWindowRgn`으로 outside-window click-through를 보장한다.
- active capture 중에는 per-frame region 변경을 금지하고 old/new rect의 swept union 또는 interaction envelope region을 한 번 arm한다.
- mouse-up final visible commit과 `DwmFlush` 뒤 exact idle region으로 축소한다.

이 구조는 과거 Arm N의 scoped left/top visible 성공을 참고하지만 그대로 채택하지 않는다. 새 제품 후보는 four-edge/corner, Windows 기능, DPI, accessibility를 처음부터 hard gate로 가진다.

### 2.3 fallback decision

Owned Geometry Envelope v2가 같은 조건에서 사용자에게 명확한 visible 개선을 보이지 못하면 구현을 확장하지 않는다. 그때만 다음 두 선택을 별도 결정한다.

1. standard Windows shell의 platform cadence를 제품 한계로 수용
2. standard shell은 idle/final ownership만 유지하고 resize 중 별도 app-owned proxy가 visible geometry를 소유하는 새로운 spike

fallback proxy는 이 문서에서 자동 승인하지 않는다. native shell을 투명화/숨김 처리하는 동안 drag capture, taskbar, z-order, accessibility가 깨질 수 있으므로 별도 architecture 승인이 필요하다.

## 3. 실행 순서와 hard gate

```text
P0 physical native-control truth gate
→ O1 independent owned-envelope visual spike
→ O2 atomic transaction and terminal lifecycle
→ O3 product metrics/input/semantics integration
→ O4 Windows behavior compatibility
→ O5 four-edge/corner visible gate
→ O6 performance and lifecycle matrix
→ FG product acceptance
→ release decision and legacy cleanup
```

- P0 전에는 synthetic `SendInput` cadence를 사용자 visible acceptance로 사용하지 않는다.
- O1이 same-condition visible comparison에서 명확히 개선되기 전에는 product host를 전환하지 않는다.
- O2가 one-owner/one-visible-front terminal contract를 통과하기 전에는 framework와 input을 연결하지 않는다.
- O3가 PASS하기 전에는 Snap/system menu 같은 Windows 기능 확장에 들어가지 않는다.
- O4 전체 PASS 전에는 현재 FlutterEmbedder default를 바꾸지 않는다.
- O5 사용자 visible PASS 전에는 GPU/content 최적화나 대규모 matrix로 확대하지 않는다.
- FG 전체 PASS 전에는 Arm N, MAUI, current standard-shell path와 rollback selector를 삭제하지 않는다.

## 4. P0 — Physical native-control truth gate

### 목표

현재 모든 variant에서 보인 떨림을 injected cursor quantization, standard USER32/DWM resize cadence, Doroti visible transaction 문제로 분리한다.

### 비교 대상

1. bare standard Win32 top-level + solid GDI child, renderer/DComp 없음
2. current R1 variant 1 standard shell
3. O1 owned-envelope prototype

### 방법

- 같은 monitor, DPI, refresh, initial rect를 사용한다.
- 사용자가 실제 mouse로 Left/Top/Right/Bottom fast와 slow drag를 각각 3회 수행한다.
- synthetic `600px/150ms`는 반복 가능한 보조 측정으로만 남긴다.
- 1kHz cursor/window sampler, raw capture, 가능하면 240fps 이상 external video를 별도 provenance로 저장한다.
- border 자체 떨림, cursor-edge 추종, opposite edge, content continuity를 각각 판정한다.
- 사용자가 본 결과를 run ID와 edge별 `PASS/FAIL`로 직접 기록한다.

### P0 판정

- bare standard shell도 physical mouse에서 떨리면 같은 장비/refresh의 platform floor로 기록한다.
- platform floor는 Doroti product PASS 면제가 아니다. O1이 동일 조건에서 더 부드러운 visible geometry를 제공해야 다음 단계로 간다.
- physical mouse는 smooth한데 injected run만 떨리면 `SendInput` absolute-coordinate mapping과 integer time distribution을 고친 뒤 자동 threshold를 다시 정의한다.
- 결과가 애매하면 slow-motion frame과 사용자 판정을 우선하며 추가 400-run factorial로 대체하지 않는다.

## 5. O1 — Independent Owned Geometry Envelope visual spike

### 범위

제품 framework를 연결하지 않은 native validation target에서 먼저 구현한다. 기존 `windows-top-level-presentation`의 Arm N 코드를 참고할 수 있지만 새 candidate를 `OwnedEnvelopeV2`로 분리해 과거 evidence와 섞지 않는다.

### 구현

1. active monitor/work-area를 덮는 fixed `WS_POPUP | WS_VISIBLE` interaction envelope를 만든다.
2. `OwnedWindowRect`는 envelope-local physical pixels로 유지한다.
3. app-owned caption, four borders, corner, asymmetric grid를 하나의 DComp root 아래 그린다.
4. `WM_NCHITTEST`에 의존하지 않고 app-owned border hit test + `SetCapture`로 resize lifecycle을 소유한다.
5. pointer message는 최신 target mailbox에 게시하고 즉시 반환한다.
6. frame-clock owner가 refresh당 최대 한 target만 drain한다. timer 기반 60Hz 제한은 사용하지 않는다.
7. border, caption, client clip, grid origin/extent를 한 root commit으로 갱신한다.
8. idle exact region → capture 시작 시 swept/interaction region arm → final commit 뒤 exact region 순서만 허용한다.
9. resize 중 `SetWindowPos`/`SetWindowRgn` per-frame 호출을 금지한다.
10. 프로토타입은 procedural grid를 사용해 framework/raster 병목을 제거한다.

### O1 PASS 조건

- 사용자가 같은 fast/slow four-edge drag에서 current variant 1보다 명확히 부드럽다고 판정
- visible border와 grid clip의 frame mismatch, reverse, flicker, black/white band 0
- cursor-owned edge lag p95 ≤ 1 display-refresh 이동량, max ≤ 2 refresh 이동량
- opposite visible edge drift ≤ 1 physical px
- one visible root/front, simultaneous visible front 2개인 frame 0
- WndProc p99 ≤ 1ms, max ≤ 4ms
- outside-window click은 idle exact region에서 다른 process로 전달
- capture 종료 뒤 click, cursor, focus가 정상 복구

O1 visible FAIL이면 O2로 진행하지 않는다.

## 6. O2 — Atomic transaction과 terminal lifecycle

### 상태 모델

```text
PointerTargetObserved(targetGeneration, ownedRect)
  → VisualTargetPrepared
  → BackingReady
  → RootCommitSubmitted
  → RootCommitVisible
  → Superseded | Failed

mouse-up target
  → FinalBackingReady
  → FinalRootCommitVisible
  → IdleRegionApplied
  → Done
```

### 불변식

- `OwnedWindowRect` generation은 immutable하다.
- running 1 + latest pending 1을 넘지 않는다.
- old target을 새 generation으로 relabel하지 않는다.
- prepared hidden slot은 newer target에 의해 supersede될 수 있다.
- root에 attach된 front만 visible이며 attach/root switch/offset/clip은 한 commit이다.
- `SetWindowRgn`, GPU wait, present/commit completion을 pointer WndProc에서 기다리지 않는다.
- final target은 exactly-once terminal을 가지며 mouse-up 후 버려지지 않는다.
- shutdown/device loss는 pending/visible transaction을 terminal 처리한다.

### O2 PASS 조건

- target generation gap, duplicate terminal, stale visible commit 0
- root switch와 geometry/clip mismatch 0
- hidden preparation abandonment은 허용하되 visible stale front 0
- mouse-up final visible ≤ 100ms, final region 적용 ≤ 1 additional refresh
- capture 중 close, minimize, device loss에서 hang/resource leak 0

## 7. O3 — Product metrics/input/semantics integration

### geometry authority 변경

기존 `actual child HWND client == framework metrics` 계약을 OwnedEnvelope 경로에서는 사용하지 않는다.

- `OwnedWindowRectGeneration`의 client physical extent, DPI, display ID가 framework metrics authority다.
- envelope HWND/client는 input/composition container이며 render size authority가 아니다.
- framework logical origin은 owned client `(0,0)`이다.
- screen 좌표 변환은 `envelope origin + owned window offset + client inset` 하나로 통일한다.

### 작업

1. `FlutterWindowsViewMetricsCoordinator`와 scheduler에 owned-geometry source를 별도 adapter로 추가한다.
2. current standard-shell metrics source와 type/trace를 섞지 않는다.
3. input hit test, pointer packet, IME caret, popup anchor, clipboard/focus 좌표를 owned transform으로 통일한다.
4. UIA root와 semantics bounds를 owned visible client로 clip/offset한다.
5. Skia backing은 owned client + bounded reserve만 사용한다.
6. exact scene generation/extent가 owned target과 일치할 때만 exact terminal을 승인한다.
7. content가 늦으면 border geometry를 rollback하지 않는다. safe fill과 transient scene은 exact와 별도 상태로 유지한다.

### O3 PASS 조건

- metrics/input/semantics가 같은 owned generation과 transform을 사용
- stale/wrong-size exact present, causal gap, receipt mismatch 0
- pointer/capture/cursor/focus/popup 자동 regression PASS
- Korean IME caret/candidate 위치 physical PASS 전까지 `notVerified`
- envelope 전체 크기의 render backing allocation 0
- renderer 50ms fault에서 visible border stall 2 refresh 이상 또는 mouse-up jump 0

## 8. O4 — Windows behavior compatibility gate

custom non-client를 product default로 고려하려면 다음을 모두 실제 Windows behavior로 검증한다.

- move drag와 double-click maximize/restore
- caption minimize/maximize/close buttons와 hover/pressed state
- `Alt+Space` system menu와 keyboard commands
- taskbar activate/minimize/restore, Alt+Tab
- Win+Arrow Snap, Snap Layouts hover, unsnap restore bounds
- minimum/maximum track size
- maximize work-area/insets, fullscreen enter/exit
- primary/secondary, negative virtual origin, mixed DPI monitor 이동
- touch/pen/keyboard resize
- Narrator, Accessibility Insights, caption control UIA
- high contrast, reduced motion, RTL caption layout

### O4 hard gate

- 필수 Windows 기능 하나라도 current product보다 퇴행하면 default cutover를 금지한다.
- Snap Layouts처럼 app-owned popup에서 OS 계약을 충족할 수 없는 기능은 숨기지 말고 product decision으로 올린다.
- 자동 hit-test/command 성공은 실제 shell UI와 accessibility visible PASS를 대신하지 않는다.

## 9. O5 — Four-edge/corner visible gate

O4를 통과한 동일한 candidate binary 하나만 사용한다.

### matrix

- Left, Top, Right, Bottom, four corners
- expand, shrink, immediate reverse
- `600px/150ms`, `600px/300ms`, slow/fine drag
- 각 조건 physical mouse 3회 이상
- initial logical size 420×300, 640×360, 1000×600
- 가능한 DPI 100%, 125%, 150%, 200%
- 가능한 refresh 60, 120, 144, 165Hz

### O5 PASS 조건

- 사용자가 border/chrome/grid/content continuity를 edge별로 직접 PASS
- border/grid clip reverse, flicker, blank, black/white band 0
- grid X/Y non-uniform scale ≤ 2%
- uncovered gap p95/max ≤ 1 physical px
- cursor-owned edge lag p95 ≤ 1 refresh 이동량, max ≤ 2 refresh 이동량
- final lag ≤ 1 physical px
- final exact ≤ 100ms
- raw frame/encoder/trace drop 0

한 edge의 자동 PASS나 과거 Arm N left/top 수용을 다른 edge/corner로 확대하지 않는다.

## 10. O6 — Performance와 lifecycle matrix

visible geometry가 먼저 PASS한 뒤에만 최적화한다.

### timing

- pointer target publication → frame-clock drain
- drain → backing ready
- Skia paint/flush/submit
- GPU fence/copy
- root commit submit → visible
- mouse-up → final exact → idle region

### 최적화 허용 순서

1. unnecessary full clear/copy 제거
2. bounded capacity growth를 interactive hot path 밖으로 이동
3. exact scene과 newest useful transient scene 우선순위 정리
4. DwmFlush pacing과 post-final ordering 분리

thread priority, arbitrary frame cap, debounce, timeout 증가는 원인 timing이 증명된 경우에만 별도 변경으로 허용한다.

### lifecycle

- minimize/restore, maximize/restore, Snap/unsnap
- occlusion/unocclusion, sleep/resume
- display disconnect/reconnect, RDP attach/detach
- context/device loss와 recovery
- resize 중 close/shutdown

## 11. FG — Windows 제품 acceptance

O5/O6를 통과한 동일한 candidate binary 하나만 FG에 사용한다.

### focused automated regression

- owned-envelope structural/region/one-visible-root gate
- scheduler/causal/terminal validator
- F7 input/IME/UIA automated regression
- Windows Release build
- clean publish and restricted-PATH launch
- generated template WindowsAppSdk launch

### physical/visible acceptance

- four-edge/corner fast/slow resize
- Korean IME composition/candidate/caret
- pointer/capture/cursor/focus/popup
- Narrator와 Accessibility Insights
- mixed-DPI monitor 이동
- minimize/maximize/Snap/fullscreen
- sleep/resume와 가능한 RDP

### deployment/rollback

- Windows App Runtime absent/update
- native DLL missing, wrong architecture, corrupt publish
- DispatcherQueue shutdown fault, context/device recovery failure
- current FlutterEmbedder standard-shell rollback live launch
- ArmNLegacy rollback live launch
- MAUI rollback live launch

release cutover와 cleanup은 automated, physical/visible, deployment, rollback이 모두 PASS하고 release 범위와 충돌하는 `notVerified`가 없을 때만 별도 승인한다.

## 12. Evidence 규격

각 run은 다음 provenance를 분리한다.

- source revision, dirty state, binary/package fingerprint
- architecture/presenter/geometry-source identity
- GPU adapter와 software fallback
- monitor rect, work area, DPI, refresh, resolution
- physical mouse 또는 synthetic input identity
- run ID, initial/final owned rect, edge/corner, motion, speed
- raw cursor/owned geometry/root commit timeline
- visible front ID, target generation, clip/offset/content extent
- region mode: idle exact / capture swept / final exact
- framework/raster/GPU/DComp timing과 terminal ledger
- raw capture, slow-motion/external video, derived summary
- 사용자 edge별 visible 판정

build/process/contract 성공을 visible/compositor/physical PASS로 승격하지 않는다. WGC change-driven frame rate를 monitor refresh cadence로 해석하지 않는다.

## 13. 금지 조건

- 기존 R2 safe-fill/stale-scene 개선을 새 ownership gate보다 먼저 구현
- standard moving HWND border와 app content를 timer/flush로 억지 동기화
- pointer WndProc의 framework/GPU/present/commit wait
- resize 중 per-frame `SetWindowPos` 또는 `SetWindowRgn`
- native geometry나 app geometry의 arbitrary 60Hz cap
- final-only debounce, mouse-up geometry replay
- full-frame X/Y stretch 또는 edge pixel 반복을 exact content로 계상
- dual simultaneously visible fronts
- hidden prepared scene을 exact present로 relabel
- monitor/virtual-desktop 크기의 permanent render backing
- edge별 좌표 보정으로 ownership mismatch 은폐
- synthetic 자동 PASS를 physical mouse 또는 사용자 visible PASS로 확대
- O4 이전 current default/rollback source 삭제

## 14. 정확한 재개 지점

다음 작업은 **P0와 O1 계획 범위**로 제한한다.

1. 현재 R1의 사용자 visible 결과를 edge별 manual evidence record로 추가한다.
2. bare standard Win32 solid-child physical mouse control을 준비한다.
3. existing variant 1과 같은 monitor/rect에서 physical fast/slow four-edge를 비교한다.
4. product framework와 분리된 `OwnedEnvelopeV2` prototype을 구현한다.
5. fixed envelope, app-owned border/grid, capture 동안 region 1회 arm, one-root commit을 검증한다.
6. 사용자에게 same-condition side-by-side visible 비교를 요청한다.
7. O1이 명확히 PASS할 때만 O2 transaction lifecycle을 구현한다.

현재 정확한 상태는 **current standard-shell architecture visible FAIL / OwnedEnvelopeV2 notStarted**다. 이번 문서 개정은 계획만 변경했으며 새 구조의 구현·build·runtime 검증은 수행하지 않았다.

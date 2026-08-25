# Doroti Windows App SDK 기반 Flutter-style embedder 전환 계획

## 0. 문서 목적과 최종 결정

- 전면 개편일: 2026-08-24
- 구현 우선 방식 전환일: 2026-08-25
- F8~F10 제품 연결 완료일: 2026-08-25
- 검증 스크립트 정리일: 2026-08-25
- 대상: Doroti Windows 제품 호스트, interactive resize, GPU surface, 입력·IME·접근성·lifecycle·배포
- Flutter 기준 source: repository의 `reference/flutter-master` commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- 목표 제품 호스트: `Doroti.Host.WindowsAppSdk`
- 목표 Windows target: `Doroti.Target.Windows.WindowsAppSdk.win-x64`
- 목표 창 구조: **표준 Win32 top-level HWND + client 전체를 채우는 child render HWND**
- 목표 그래픽 기준선: **ANGLE/EGL window surface + Skia GPU**, exact client physical size
- Windows App SDK 기준선: **exact `2.4.0`, self-contained unpackaged, raw HWND + AppWindow + DispatcherQueue**
- 문서 성격: **구현 우선, 후속 통합 검증·안정화 계획**. 계획의 변경이나 구현 완료는 build/runtime/visible acceptance를 자동으로 의미하지 않는다.

`Doroti/eng`의 검증 스크립트는 정리되어 현재 실행 진입점으로 제공되지 않는다. 아래 gate의 script 기반 PASS와 fingerprint는 삭제 전 실행한 historical evidence이며 현재 checkout에서 재실행 가능한 명령을 뜻하지 않는다.

기존 `Doroti.Host.WindowsAppSdk`의 `fixed work-area envelope + custom non-client + Arm N dual-front` 구현을 **Windows App SDK 기반 Flutter Windows embedder 방식**으로 전환한다. Windows App SDK 제품/target/runner 경계는 유지하되, 그 안의 window tree와 resize/surface lifecycle을 Flutter 방식으로 교체한다.

여기서 “Flutter 방식”은 이름이나 API를 흉내 내는 것이 아니라 다음 ownership과 lifecycle을 채택한다는 뜻이다.

1. Windows가 표준 top-level 창의 non-client, move, resize, Snap, system menu, maximize/minimize를 소유한다.
2. 하나의 child HWND가 top-level client rect를 그대로 채우며 rendering, pointer, keyboard, text, accessibility의 native view 경계가 된다.
3. child `WM_SIZE`의 physical pixel 크기와 현재 DPI/display ID를 framework metrics의 authority로 사용한다.
4. resize target을 framework에 전달한 뒤 raster thread가 같은 크기의 frame과 window surface를 준비한다.
5. Flutter source의 별도 UI/raster 구조에서는 platform thread가 제한 시간 동안 present를 기다린다. 현재 Doroti 제품은 framework callback이 platform STA에 co-located이므로 scene dispatch 뒤 즉시 `WM_SIZE`를 반환하고, exact swap/present terminal과 `DwmFlush`는 raster thread에서 비동기로 완료한다.
6. 첫 frame 전에는 top-level 창을 표시하지 않는다.
7. monitor work-area는 initial placement, restore, popup constraint에만 사용한다. swap-chain/EGL capacity나 평상시 HWND envelope 크기로 사용하지 않는다.
8. Windows App SDK `AppWindow`와 `DispatcherQueue`는 raw HWND lifecycle/deployment 기반을 제공하지만 child render size나 present timing의 authority가 되지 않는다.

새 제품 기준선에서는 다음을 사용하지 않는다.

- monitor work-area 크기의 고정 top-level envelope
- app-owned window region으로 실제 창 모양을 제한하는 방식
- custom caption/border를 resize transaction에 포함하는 방식
- monitor-sized backing 또는 swap-chain capacity
- dual hidden/visible composition front
- `ContentIsland`/SiteBridge를 전체 client render-size/presentation root로 사용하는 방식
- resize hot path의 `SetSourceSize`, provisional full-frame stretch, edge별 translate/clip

기존 Arm N adapter와 MAUI Windows host는 새 경로가 최종 acceptance를 통과할 때까지 rollback/diagnostic으로 보존한다. 같은 `Doroti.Host.WindowsAppSdk` package 안에서도 Flutter-style adapter와 Arm N adapter의 surface/resize state를 섞지 않고 startup host 선택으로만 전환한다.

## 1. 현재 상태와 완료 경계

| 단계 | 상태 | 현재 의미 |
|---|---|---|
| F0-A Flutter source protocol audit | **PASS** | pinned source에서 parent/child HWND, `WM_SIZE`, metrics, raster-thread surface resize, presented ACK, first-frame show 계약을 확인했다. 문서·source audit PASS이며 제품 구현 PASS는 아니다. |
| F0-V source pin/anchor validator | **PASS** | validator/contract는 `fd405070`에서 tracked로 채택됐고, clean current worktree의 20분 제한 source-only run이 PASS(`10 files/12 anchors/4 mappings`)했다. FCR-0와 CI fail-fast wiring도 추가했다. |
| F1 Windows App SDK/ANGLE bootstrap preflight | **PASS** | exact Windows App SDK 2.4 self-contained bootstrap, same-STA platform/100-cycle raster preflight를 확인했다. |
| F2 표준 top-level + child view | **PASS** | raw top-level 하나와 child view 하나의 same-STA 100-cycle structural gate를 확인했다. |
| F3 metrics/display contract | **PASS** | child-client physical authority, immutable metrics/frame generation, 4-DPI matrix를 확인했다. |
| F4 raster-thread EGL surface lifecycle | **PASS** | child HWND EGL surface의 dedicated-raster 1,000 recreate/1,001 swap과 context-loss recovery를 확인했다. |
| F5 bounded resize handshake | **fixture PASS / 제품 wait 제거** | Flutter의 100ms poll contract 자체는 fixture에서 PASS했다. 그러나 현재 Doroti 제품은 framework callback이 platform STA에 함께 있어 이 wait가 `WM_SIZE` 반환을 막는 visible regression을 만들었으므로, 제품 WndProc는 scene dispatch 뒤 즉시 반환하고 exact present terminal과 post-return raster `DwmFlush`를 비동기로 유지한다. |
| F6 scheduler/vsync/present integration | **기본 계약 PASS / 고속 회귀 재개** | single pending slot, latest-metrics pre-dispatch replacement, active-resize ordinary continuation, native DWM timing, causal present를 확인했다. 좌·상 저속 미세 떨림 보강 뒤에도 사용자가 네 방향 고속 drag에서 창 위치·크기와 content가 포인터를 따라오지 못하는 현상을 확인했으므로 기존 F6 PASS를 고속 visible/cadence 완료로 확대하지 않는다. |
| F6-R high-speed interactive resize | **계획 확정 / 자동 capture·log notVerified** | 재현 기준은 한 edge를 `600 physical px / 150ms`로 이동하는 약 4,000px/s drag다. drag 전·중·후의 전체화면 frame과 cursor/native/framework/raster/present 로그를 같은 시간축으로 수집해 최초 지연 경계를 찾고, 그 owner를 수정한 뒤 같은 입력으로 반복 검증한다. |
| F7 input/IME/accessibility | **PASS** | actual child HWND automated input/IMM32/UIA gate, 수동-gate fail-safe, keyboard control-character regressions가 PASS했다. 2026-08-25 사용자가 pointer/capture/cursor, focus/popup, Korean IME/clipboard, Narrator, Accessibility Insights, resize/DPI bounds 전 항목을 직접 확인해 PASS로 수용했다. |
| F8 DPI/display/lifecycle/recovery | **구현 완료 / 자동 fixture·제품 smoke PASS / 물리 검증 deferred** | lifecycle manager를 FlutterEmbedder 제품 host에 연결했고 shutdown 시 pending frame/resize를 terminal 처리한다. 물리 mixed-DPI/monitor/sleep/RDP 및 visible recovery는 `notVerified`다. |
| F9 target/runner/package integration | **구현 완료 / build·publish·launch PASS** | WindowsAppSdk target/runner/package, native artifact provenance, `FlutterEmbedder`/`ArmNLegacy`/MAUI rollback 선택을 연결했다. MAUI는 별도 entrypoint build만 확인했고 live launch는 `notVerified`다. |
| F10 default adapter cutover | **구현 완료 / 자동 product smoke PASS** | CLI, Demo, target, template의 Windows 기본을 WindowsAppSdk/FlutterEmbedder로 전환하고 rollback을 보존했다. release 확정과 기존 host 삭제는 FG 뒤다. |
| FG Windows 제품 acceptance | **자동 범위 partial PASS / 고속 all-edge resize 선행 차단** | 2026-08-25 사용자가 네 방향의 빠른 drag에서 창 위치·크기와 content 추종이 끊기는 현상을 확인했다. F6-R의 `600px/150ms` 전체화면 capture+causal log gate가 PASS하기 전에는 FG visible resize matrix를 시작하거나 기존 결과를 최종 수용으로 해석하지 않는다. |

기존 증거는 다음처럼 재분류한다.

- Arm N의 고속 좌측·상단 manual PASS는 **기존 구조의 scoped regression PASS**로 보존한다.
- 기존 M1 observer FAIL과 G2 `notVerified`는 과거 구조의 판정으로 보존한다.
- 새 Flutter-style 경로는 F1부터 독립적으로 구현·판정한다.
- 기존 observer는 diagnostic으로 재사용할 수 있지만, 불안정한 observer 한 종류를 F1~F8 구현 시작의 hard stop으로 두지 않는다.
- FG visible PASS에는 qualified output evidence와 사용자의 실제 mouse drag 확인이 모두 필요하다.
- F6-R 수정 반복은 사용자에게 매번 재현을 요청하지 않고 자동 고속 drag, 전체화면 frame sequence, causal log로 진행한다. 사용자의 직접 확인은 자동 evidence가 모두 PASS한 최종 binary에서 한 번만 수행한다.
- Flutter runtime capture/A-B instrumentation은 범위 밖이다. pinned Flutter source를 protocol reference로만 사용한다.

### 1.1 구현 우선 실행 원칙

**F8 구현 마무리 → F9 제품 통합 → F10 기본 경로 wiring → 통합 build/smoke** 순서는 완료했다. 현재 exact 실행 순서는 **F6-R 고속 resize 안정화 → focused build/contract 회귀 → FG 물리·visible acceptance**다.

- 단계별 `PASS 조건`은 다음 구현 단계의 시작을 막는 hard gate가 아니라 **후속 검증 백로그**다.
- 선행 단계의 API·ownership·artifact가 다음 단계 구현에 필요할 만큼 존재하면 다음 구현을 시작한다.
- 자동·수동·물리 검증은 구현 흐름을 매번 끊지 않고, F8~F10 end-to-end 경로가 연결된 뒤 묶어서 실행한다.
- 구현 중 발견한 compile error, 명백한 crash, 데이터 손상, ABI 불일치, 무한 대기, rollback 불능은 즉시 고친다. 그 밖의 동작 문제는 재현 조건과 영향 범위를 백로그에 기록하고 구현을 계속한 뒤 안정화 단계에서 수정한다.
- 미실행 검증은 `deferred` 또는 `notVerified`로 남긴다. 넘긴 검증을 `PASS`로 간주하거나 기존 fixture PASS를 제품 visible PASS로 승격하지 않는다.
- F10 wiring은 rollback adapter를 유지한 채 구현할 수 있지만, release 확정·rollback 삭제·구형 host cleanup은 FG와 사용자 제품 수용 뒤에만 한다.

## 2. pinned Flutter Windows source에서 채택하는 계약

### 2.1 native window tree

Flutter Windows host는 표준 top-level HWND를 만들고, 별도의 `WS_CHILD | WS_VISIBLE` view HWND를 parent에 붙여 client rect 전체로 `MoveWindow`한다.

참조 지점:

- `reference/flutter-master/engine/src/flutter/shell/platform/windows/host_window.cc`
  - `SetChildContent`: `SetParent` 후 parent client rect로 child를 배치
  - `HostWindow::InitializeFlutterView`: view 생성, 표준 top-level 생성, child 연결
  - top-level `WM_SIZE`: 현재 client rect로 child resize/reposition
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_window.cc`
  - `FlutterWindow::InitializeChild`: `WS_CHILD | WS_VISIBLE` view HWND 생성
  - child `WM_SIZE`: physical width/height를 view delegate에 전달

Doroti도 같은 두 HWND 경계를 사용한다.

```text
standard top-level HWND
└─ Doroti child view HWND
   ├─ native input / focus / IME / UIA boundary
   └─ exact-size ANGLE/EGL window surface
```

top-level 창에는 일반 Windows style과 `DefWindowProc` 동작을 유지한다. child view는 client `(0,0,width,height)`를 채운다. 제품 기본 경로에서 custom non-client나 fixed envelope를 만들지 않는다.

### 2.2 resize handshake

Flutter의 OpenGL Windows path는 다음 순서를 사용한다.

```text
child WM_SIZE
→ ResizeStarted(target physical width/height)
→ engine window metrics 전송
→ framework가 target size frame 생성
→ raster thread가 exact-size window surface 재생성
→ buffer swap/present
→ FrameGenerated → Done
→ platform thread unblock
→ raster thread DwmFlush ordering point
```

핵심 규칙:

- platform thread가 `WM_SIZE`를 받는다.
- framework metrics는 physical `width`, `height`, `pixel_ratio`, `display_id`, constraints를 함께 가진다.
- raster thread의 frame 크기가 pending resize target과 다르면 surface를 변경하거나 present하지 않는다.
- surface resize는 raster thread에서만 수행한다.
- 현재 Flutter source는 크기가 바뀌면 기존 EGL window surface를 destroy하고 exact 크기로 다시 만든다.
- present 완료가 resize transaction의 완료 신호다.
- platform thread는 무제한 대기가 아니라 현재 source 기준 100ms 동안 engine task runner를 polling한다.
- arbitrary Win32 nested message pump를 열지 않는다.
- 완료 신호 뒤 raster thread가 `DwmFlush`를 호출해 이전 크기 surface stretch glitch를 줄인다.

Doroti는 이 상태 전이와 exact-present terminal을 기준선으로 삼고 모든 target에 monotonic `resizeGeneration`을 추가한다. 다만 현재 제품은 framework/UI callback이 별도 Flutter UI thread가 아니라 platform STA에서 실행된다. 이 구조에서 present까지 기다리면 WndProc와 연속 `WM_SIZE` 자체가 직렬로 막히므로, 2026-08-25 visible regression 수정 이후 제품 경로는 framework scene dispatch까지만 동기 수행하고 즉시 반환한다. 아직 시작하지 않은 pending callback은 새 causal ticket의 latest metrics request로 교체하며, raster는 exact generation/extent만 recreate/swap하고 platform 반환 뒤 `DwmFlush`한다. Flutter의 100ms poll은 별도-thread fixture/source reference로 보존하며 현재 제품 hot path에는 적용하지 않는다.

### 2.3 first frame, DPI, monitor

Flutter host는 view별 first-frame callback 뒤 top-level을 표시한다. Doroti도 `ShowWindow` 전에 첫 exact metrics frame의 surface swap을 요구한다.

Flutter host의 monitor/work-area 사용도 구분해 따른다.

- DPI 변경: `WM_DPICHANGED` suggested rect 적용
- normal window restore/placement: 현재 monitor `rcWork`에 맞춤
- fullscreen: 현재 monitor `rcMonitor` 사용
- render surface: child client physical pixels 사용

따라서 work-area는 window placement 정보이지 rendering capacity가 아니다.

## 3. 목표 아키텍처

### 3.1 프로젝트와 dependency 경계

기존 Windows App SDK 제품 경계를 유지하고 host 내부 adapter를 분리한다.

- `Doroti/src/Doroti.Host.WindowsAppSdk`
- `Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64`
- `DorotiDemoApp/windowsappsdk`

`Doroti.Host.WindowsAppSdk`의 새 Flutter-style adapter는 다음을 소유한다.

- exact Windows App SDK `2.4.0` bootstrap/runtime selection
- Windows App SDK `DispatcherQueueController`
- raw HWND의 `WindowId`와 `AppWindow` association
- Win32 top-level/child HWND와 WndProc
- platform task runner와 thread-affine lifecycle
- ANGLE/EGL display/context/window surface
- Skia GPU surface 연결과 swap/present ACK
- Windows input/text/UIA/display integration

새 adapter는 MAUI와 XAML `Window`에 의존하지 않는다. Windows App SDK `2.4.0`은 host/target/runner에 exact pin하고, `AppWindow`와 `DispatcherQueue`를 raw HWND에 연결한다. Windows App SDK package/runtime provenance를 build와 launch evidence에 함께 남긴다.

ANGLE/EGL/native DLL은 exact version과 hash를 고정하고 publish 결과에 명시적으로 포함한다. 시스템 PATH나 개발 PC에 우연히 설치된 DLL로 fallback하지 않는다.

`ContentIsland`/SiteBridge의 경계는 다음처럼 제한한다.

- primary Doroti render/input view는 Flutter처럼 raw `WS_CHILD` HWND다.
- full-client `DesktopAttachedSiteBridge`는 새 adapter에서 사용하지 않는다. 기존 bridge는 Arm N rollback에만 남긴다.
- `ContentIsland`가 필요한 native/plugin content는 별도의 bounded `DesktopChildSiteBridge` descendant로만 추가한다.
- auxiliary island는 등록된 purpose/rect가 있을 때만 존재하며, primary child metrics, EGL surface size, frame scheduling, present ACK의 authority가 될 수 없다.
- island를 추가하는 순간 focus, keyboard/IME, pointer, UIA parent-child boundary를 F7에서 독립 검증한다.

### 3.2 size와 coordinate authority

authority는 다음처럼 한 번만 나눈다.

| 정보 | 유일한 authority |
|---|---|
| outer window geometry/non-client | top-level HWND와 Windows |
| client physical width/height | child HWND `GetClientRect`/`WM_SIZE` |
| device pixel ratio | child HWND의 per-monitor-v2 DPI |
| display identity/refresh | child HWND가 속한 `HMONITOR`/display manager |
| logical constraints/layout | Doroti framework |
| EGL surface size/thread binding | raster thread |
| presented resize completion | exact-size frame의 successful swap/present |
| initial/restore placement | current monitor `rcWork` |
| fullscreen bounds | current monitor `rcMonitor` |
| Windows App SDK object lifecycle | platform thread `DispatcherQueue` |
| raw HWND wrapper/deployment identity | `WindowId`/`AppWindow`와 selected Windows App Runtime |

logical size를 다시 physical size의 authority로 쓰지 않는다. `physical = logical × DPR` 계산은 framework metrics 해석에만 사용하며, 최종 surface 크기는 실제 child client pixels와 일치해야 한다.

### 3.3 thread ownership

- platform thread
  - top-level/child WndProc
  - Windows App SDK `DispatcherQueue`와 `AppWindow`
  - resize state 시작과 metrics 게시
  - fixture handshake의 engine task polling; co-located 제품 resize hot path에서는 present poll 없음
  - focus, lifecycle, display events
- framework/UI owner (현재 제품은 platform STA에 co-located)
  - metrics 적용, layout, scene 생성
  - resize generation을 보존한 frame request
- raster thread
  - EGL context와 window surface의 유일한 owner
  - exact-size Skia target 생성
  - raster, flush, swap/present
  - presented/failed terminal 보고

WndProc가 직접 Skia raster, GPU fence wait, EGL surface destroy, swap/present 완료 대기를 수행하지 않는다. 현재 co-located framework callback은 metrics 적용/layout/scene 제출까지만 수행하며 raster terminal을 기다리지 않고 반환한다. 별도 framework/UI thread를 도입하기 전에는 Flutter의 bounded present poll을 제품 WndProc에 다시 넣지 않는다. 일반 Win32 message나 Windows App SDK `DispatcherQueue` event loop를 중첩 실행하지 않으며, 바깥 message loop 종료 뒤 `AppWindow`/island를 먼저 닫고 `DispatcherQueueController.ShutdownQueue()`로 thread-affine rundown을 완료한다.

### 3.4 render surface lifecycle

Flutter-equivalent 기준선은 ANGLE/EGL을 사용한다.

1. child HWND 생성 후 raster thread에서 EGL display/config/context를 준비한다.
2. child physical pixels로 EGL window surface를 만든다.
3. resize target frame이 exact 크기로 생성되면 raster thread에서 이전 window surface를 destroy한다.
4. 같은 child HWND와 target physical size로 새 window surface를 만든다.
5. context/current 상태와 `eglSwapInterval`을 복원한다.
6. exact frame을 raster한 뒤 `eglSwapBuffers` 성공을 presented terminal로 보고한다.
7. device/context loss는 current resize transaction을 `Failed`로 닫고 전체 EGL/Skia context generation을 재생성한다.

resource cache, decoded image, shader 자산은 window surface와 분리한다. surface 재생성 때문에 framework scene이나 장기 GPU resource identity가 임의로 초기화되지 않게 한다.

Direct D3D12 presenter는 새 기준선에 포함하지 않는다. 향후 필요하면 FG PASS 뒤 같은 host/view/resize contract 아래 별도 backend로 제안하며, Flutter-equivalent 기준선을 대체하려면 독립 acceptance를 다시 통과해야 한다.

### 3.5 resize state machine

Doroti 상태는 Flutter 계약을 다음처럼 명시화한다.

```text
Idle
  → ResizeStarted(generation, widthPx, heightPx, dpr, displayId)
  → FrameGenerated(same generation, exact extent)
  → SurfaceReady(same generation, exact extent)
  → Presented
  → Done

ResizeStarted → TimedOut
ResizeStarted/FrameGenerated/SurfaceReady → Failed
older generation → Superseded
zero-sized/minimized target → Suspended
```

규칙:

- 모든 started generation은 정확히 하나의 terminal을 가진다.
- frame 크기나 generation이 target과 다르면 surface/present 단계에 들어가지 않는다.
- 이미 framework work가 시작된 ordinary animation frame은 resize generation으로 relabel되지 않는다. 아직 dispatch되지 않은 단일 continuation은 기존 ticket을 폐기하고 새 causal ID의 latest exact resize ticket으로 교체할 수 있다.
- resize 중 새 `WM_SIZE`가 이전 transaction 완료 뒤 도착하면 새 generation으로 시작한다.
- timeout은 성공으로 간주하지 않는다. native resize 진행을 반환하되 최신 실제 client size를 다시 관측하고 redraw를 요청한다.
- 별도-thread handshake fixture의 timeout 기준은 Flutter source와 같은 100ms다. co-located 제품 WndProc에는 이 timeout wait 자체를 두지 않으며, 향후 별도 UI thread를 도입하더라도 수치를 늘리기 전에 framework/raster/surface 구간별 원인을 증명해야 한다.

### 3.6 input, text, accessibility

child HWND를 Flutter view와 같은 native interaction root로 사용한다.

- pointer/mouse/touch/pen/wheel: child client 좌표로 변환
- capture: child가 down부터 up/cancel까지 소유
- keyboard/focus: child HWND가 입력 focus owner
- cursor: content 영역은 Doroti cursor, top-level non-client는 `DefWindowProc`
- IME: child HWND에 연결된 TSF 또는 IMM32 경로 하나만 owner로 선택
- caret/candidate bounds: committed child screen coordinates 사용
- accessibility: child HWND root provider 아래 Doroti semantics tree 연결
- top-level caption/system menu/Snap UIA: Windows 기본 provider 사용

full-client transparent island, hidden XAML input control, custom caption UIA를 새 기본 구조에 도입하지 않는다. 특정 Windows App SDK native/plugin content가 필요하면 bounded `DesktopChildSiteBridge` descendant로 추가하고 z-order/focus/keyboard/IME/semantics 계약을 별도 검증한다.

## 4. source/contract 불변식과 금지 조건

새 Flutter-style adapter에는 다음 contract를 정적·동적 검사한다.

- product top-level HWND: 정확히 1개
- Doroti child view HWND: view당 정확히 1개
- Windows App SDK `DispatcherQueue`: platform thread당 정확히 1개
- raw top-level `WindowId`와 `AppWindow`: 정확히 1:1
- selected Windows App Runtime: build의 exact `2.4.0` package contract와 일치
- child rect: 항상 top-level client rect와 일치
- render surface extent: committed child physical pixels와 일치
- platform metrics: width/height/DPR/display ID/constraints를 한 snapshot으로 게시
- EGL window surface create/destroy/recreate: raster thread에서만 수행
- first visible show: first exact frame swap 이후
- resize transaction: exactly-once terminal
- stale generation present: 0
- minimized `0×0`: surface recreate 금지, suspended 처리
- timeout 중 arbitrary nested Win32 message dispatch 금지
- work-area/monitor-size backing allocation 금지
- custom non-client/fixed envelope/window region 금지
- dual visible front 또는 hidden-front swap protocol 금지
- resize hot path의 `ResizeBuffers`, `SetSourceSize`, full-frame stretch 금지
- `AppWindow`/ContentIsland/XAML size event를 primary render-size authority로 사용 금지
- full-client `DesktopAttachedSiteBridge`를 primary render/input view와 중첩 금지
- auxiliary `DesktopChildSiteBridge`가 primary present timing을 변경하는 것 금지
- floating/pre-release ANGLE, SkiaSharp, Windows native dependency 금지

## 5. 구현 우선 작업 계획

F0~F7의 확보된 PASS evidence는 그대로 보존했다. F8~F10은 단계별 물리 검증 완료를 기다리지 않고 interface와 artifact 의존성 순서로 구현·연결했으며, 각 단계의 미실행 `PASS 조건`은 FG 안정화 체크리스트로 유지한다.

### F0 — Flutter source protocol lock

상태: **F0-A PASS — manual source audit / F0-V PASS — tracked contract, clean-checkout source-only run, FCR-0/CI fail-fast wiring**

작업:

F0-A 완료 내용:

1. `reference/flutter-master`가 `56b8e1a851a594b1a154f8ea93270807dab22b9a`인지 확인한다.
2. 아래 symbol을 직접 대조해 이 문서의 protocol을 확정한다.
   - `HostWindow::InitializeFlutterView`
   - `SetChildContent`
   - top-level/child `WM_SIZE`
   - `FlutterWindowsView::OnWindowSizeChanged`
   - `OnFrameGenerated`/`OnEmptyFrameGenerated`
   - `ResizeRenderSurface`
   - `OnFramePresented`

F0-V 현재 evidence:

1. Flutter Windows host protocol 계약과 당시 source-only validator는 `fd405070`에서 tracked로 채택됐다.
2. root와 `reference/flutter-master`의 tracked worktree가 clean인 상태에서, 20분 timeout을 둔 현재 checkout 실행이 PASS했다.
3. Flutter commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`, source fingerprint `b79424311e4d2675283c3a676343a63fcc82a20b0711c14b0eebbde47ed37ecf`, files/anchors/mappings `10/12/4`를 확인했다.
4. 당시 FCR-0 entry point와 `.github/workflows/flutter-windows-host-protocol.yml`은 source-only validator를 실행하도록 연결했다. workflow는 contract에서 exact Flutter revision을 읽고, fresh checkout에서 20분 job timeout을 사용했다.
5. scope는 source-only reference이며 Doroti build/runtime/visible acceptance가 아니다.

F0-V 유지 작업:

1. 이후 source 변경마다 FCR-0 및 `flutter-windows-host-protocol` workflow가 protocol diff 없는 자동 승격을 fail-fast로 거부하게 유지한다.
2. 다음 CI run에서는 clean checkout의 command/fingerprint가 현재 source-only run과 일치하는지 evidence로 남긴다. 이는 F0 source-only PASS를 제품/runtime/visible acceptance로 승격하지 않는다.

PASS 조건:

- pin/hash와 symbol anchor validator PASS
- 이 문서의 window/resize/surface 순서가 pinned source와 일치함
- tracked F0 wiring이 source drift를 fail-fast로 검증함
- Flutter runtime 계측 없이 source-only reference 경계가 명확함

### F1 — Windows App SDK/ANGLE bootstrap preflight

상태: **PASS — static contract v2, self-contained fixture/product publish, and live 100-cycle platform/raster preflight**

선행 조건: F0-V PASS

현재 evidence:

1. `FlutterWindowsAppSdkBootstrap`은 legacy Arm N/ContentIsland 경로와 분리되어 STA, PMv2 DPI, COM/OLE/WinRT, 하나의 `DispatcherQueueController`, raw HWND `WindowId`/`AppWindow` association을 같은 platform thread에서 소유한다.
2. `FlutterWindowsAngleEglContext`는 `Avalonia.Angle.Windows.Natives` `2.1.27548.20260419`의 `av_libglesv2.dll`과 `SkiaSharp.NativeAssets.Win32` `4.151.1`의 `libSkiaSharp.dll`을 app-directory absolute path/hash로 검증·load하고, pbuffer-only ANGLE/EGL/Skia smoke와 checked teardown을 수행한다.
3. 당시 F1 static/live gate가 source fingerprint `60eedfc4add90840104a038b0a5b6169a9d23139cbb819da776e7aae9adac5f7`에 결속된 evidence, fresh restore/publish, fixture와 product runner의 self-contained unpackaged x64 publish, DLL hash/path, package contract `Microsoft.WindowsAppSDK` `2.4.0`을 검증했다.
4. live run은 same-STA thread에서 raw HWND/`WindowId`/`AppWindow`/DispatcherQueue setup-shutdown 100회와 MTA raster thread에서 ANGLE/EGL/Skia pbuffer create/destroy 100회를 PASS했다. teardown failure, MAUI/XAML loaded startup assembly, PATH fallback은 모두 0/false였다.
5. observed hardware run은 `ANGLE (AMD Radeon 780M Graphics, Direct3D11)`이고 `softwareFallback=false`였다. 이는 F1 ABI/deployment evidence일 뿐 F2 이후 window-surface resize나 visible product acceptance가 아니다.

작업:

1. 기존 `Doroti.Host.WindowsAppSdk` 안에 Arm N과 분리된 Flutter-style adapter 경계를 만든다.
2. `Microsoft.WindowsAppSDK` exact `2.4.0`, self-contained unpackaged runtime selection을 유지·검증한다.
3. STA platform thread에서 `DispatcherQueueController.CreateOnCurrentThread()`를 호출하고 raw HWND의 `WindowId`/`AppWindow`를 같은 queue에 연결한다.
4. per-monitor-v2 DPI awareness를 window와 Windows App SDK object 생성 전에 설정한다.
5. COM/OLE, Windows App SDK DispatcherQueue, platform task runner의 생성·종료 순서를 고정한다.
6. ANGLE EGL/GLES native artifact version, architecture, hash, publish copy를 고정한다.
7. raster thread에서 display/config/context를 만들고 offscreen smoke를 수행한다.
8. Skia `GRContext`를 ANGLE GLES context와 연결한다.
9. self-contained unpackaged x64 publish에서 selected Windows App Runtime과 native DLL을 PATH fallback 없이 load한다.

PASS 조건:

- clean restore/build/publish PASS
- Windows App Runtime selected version = exact `2.4.0` package contract
- raw HWND `WindowId`/`AppWindow`/DispatcherQueue association과 shutdown 100회 PASS
- EGL display/context + Skia GPU context create/destroy 100회 crash/leak 0
- selected adapter/backend/native DLL provenance가 로그에 존재
- MAUI/XAML `Window` transitive startup 경로 0
- WARP/software fallback 여부가 명시되고 hardware run과 구분됨

중단 조건:

- exact native dependency를 재현 가능하게 배포할 수 없음
- Windows App Runtime selection 또는 DispatcherQueue/AppWindow rundown이 재현되지 않음
- SkiaSharp와 ANGLE context ABI가 안정적으로 연결되지 않음
- context teardown이 thread-affine하게 완료되지 않음

### F2 — 표준 top-level HWND + child view HWND

상태: **PASS — static contract and restricted-PATH self-contained same-STA 100-cycle structural gate**

선행 조건: F1 PASS

현재 evidence:

1. `FlutterWindowsHostWindow`은 legacy Arm N/ContentIsland와 분리되어 hidden `WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN` top-level 하나와 `WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS` child view 하나만 소유한다. raw HWND `WindowId`/`AppWindow`/DispatcherQueue association은 F1 bootstrap으로 1:1 유지한다.
2. top-level `WM_SIZE`는 `GetClientRect` 뒤 child를 `MoveWindow`하고, `WM_GETMINMAXINFO`는 현재 top-level DPI에서 physical client constraint를 `AdjustWindowRectExForDpi`로 outer track size로 변환한다. custom non-client, `WS_POPUP`, full-client island, Arm N presenter는 새 경로에서 사용하지 않는다.
3. source fingerprint `5cd998e62126450da2043f290dc466730ac1b0968d0cda6b073a516a667f44ca`에 결속된 F2 static/live contract는 self-contained unpackaged x64 fixture publish, selected Windows App SDK `2.4.0`/ANGLE/Skia artifact hash, 제한 PATH launch를 재검증했다.
4. live gate는 dedicated same-STA thread에서 warmup 뒤 startup/shutdown 100회(실패 0)를 PASS했다: top-level 1, child 1, association 1, resize 뒤 client-child mismatch 0/layout 6, exact min/max `338×287`~`978×767`, first callback 전 hidden/후 정확히 한 번 show, ordered teardown, GDI `2→2`, USER `5→5`, MAUI/XAML startup assembly 0이다.
5. 이 gate의 `NotifyFirstFrameSwapped`는 first-show state ordering만 검증하는 synthetic completion이다. 실제 child HWND EGL window-surface create/swap, blank/white-frame 0, compositor-visible acceptance는 F4/FG까지 **notVerified**로 유지한다.

작업:

1. 일반 overlapped top-level HWND를 만들고 standard non-client를 유지한다.
2. top-level raw HWND에서 `WindowId`와 `AppWindow`를 얻고 platform `DispatcherQueue`에 associate한다.
3. `WS_CHILD | WS_VISIBLE` Doroti view HWND를 만든다.
4. child를 top-level에 붙이고 현재 client rect 전체로 배치한다.
5. top-level `WM_SIZE`에서 `GetClientRect` 후 child를 `MoveWindow`한다.
6. framework constraints를 `WM_GETMINMAXINFO` physical track size로 변환한다.
7. first-frame callback 전 top-level을 숨기고 exact first swap 뒤 표시한다.
8. close/destroy 순서를 `auxiliary islands → view surface → child → engine → AppWindow/top-level → DispatcherQueue`로 검증한다.

PASS 조건:

- top-level 1개, child view 1개
- raw HWND, `WindowId`, `AppWindow`, DispatcherQueue association 1:1
- standard caption resize/move/system menu/Snap/maximize/minimize 기본 동작
- child rect와 client rect mismatch 0
- first show blank/white frame 0
- startup/shutdown 100회 crash/leak 0

### F3 — physical metrics와 display contract

상태: **PASS — child-client physical authority, immutable metrics/frame generation, and self-contained 4-DPI 100-cycle matrix**

선행 조건: F2 PASS

현재 evidence:

1. `WindowsViewMetrics`/`FlutterWindowsViewMetricsCoordinator`는 child HWND `GetClientRect`만 physical width/height authority로 읽고, 같은 immutable snapshot에 view ID, `resizeGeneration`, DPR, monitor display ID, physical min/max constraints와 suspended state를 보관한다. logical→physical rounding은 `WindowsViewMetrics.LogicalToPhysical` 한 지점으로만 제한한다.
2. F2 host는 child `WM_SIZE` 뒤 `ChildClientRectChanged`를, `WM_DPICHANGED`/`WM_DISPLAYCHANGE` 뒤 `ChildDpiOrDisplayChanged`를 발행한다. coordinator는 매 callback에서 child DPI와 monitor identity를 다시 읽고, identical observation은 generation/surface recreate 없이 유지하며 `0×0`는 surface request 없는 suspended publication으로 닫는다.
3. framework mapping은 `ViewMetrics`, `DorotiViewEpoch`, `DorotiResizeEpoch`, `DorotiFrameDescriptor`를 동일 snapshot에서 만들고 current view/generation/physical extent와 정확히 일치하지 않는 stale 또는 mismatched frame을 admission 전에 거부한다.
4. source fingerprint `ca200b4ba7fa260d9ac366637b06be60a08275782d423b786e63839244ab4f3c`의 restricted-PATH self-contained live gate는 same-STA lifecycle 100회(실패 0), 100/125/150/200% 각 100회(총 400 PASS)를 기록했다. 각 row에서 actual child client = metrics = frame physical pixels 및 generation equality이고 stale/mismatch/repeated-size/zero-size recreate admission은 모두 0, suspension/restore는 각 100회다.
5. F3는 actual child DPI/display observation과 deterministic contract matrix를 증명하지만 EGL child-window surface create/recreate/swap, present, compositor/visible acceptance는 F4/FG까지 **notVerified**다.

작업:

1. child `WM_SIZE`를 physical width/height의 authority로 사용한다.
2. 같은 snapshot에 DPR, display ID, min/max constraints, view ID를 포함한다.
3. `WM_DPICHANGED`와 monitor 전환 시 child 기준 DPI/display를 다시 읽는다.
4. framework에 immutable `WindowsViewMetrics`와 `resizeGeneration`을 전달한다.
5. metrics 적용 뒤 생성된 scene/frame이 generation과 exact physical extent를 보존하게 한다.
6. minimize `0×0`, restore, repeated identical size를 별도 상태로 처리한다.

PASS 조건:

- actual client pixels = metrics pixels = frame pixels
- logical size × DPR 반올림 규칙이 한 지점에만 존재
- repeated identical size에서 불필요한 surface recreation 0
- stale/mismatched metrics-frame admission 0
- 100/125/150/200% deterministic contract matrix PASS

### F4 — raster-thread EGL window surface lifecycle

상태: **PASS — dedicated raster-thread child-HWND EGL surface, 1,000 recreate / 1,001 real swap, and context-loss recovery**

선행 조건: F3 PASS

현재 evidence:

1. `FlutterWindowsAngleEglSharedContext`는 F1이 absolute path/hash로 load한 ANGLE/Skia artifact 위에서 raster-thread-only EGL display/context/parking pbuffer/Skia cache를 소유한다. `FlutterWindowsAngleEglWindowSurface`만 child HWND `EGL_CreateWindowSurface`와 default-framebuffer `GRBackendRenderTarget`/`SKSurface` lifetime을 소유하므로 normal resize는 shared context/cache를 폐기하지 않는다.
2. `UpdateForMetrics`는 F3 `WindowsViewMetrics`의 drawable physical extent와 실제 child `GetClientRect`, `eglQuerySurface` extent가 모두 정확히 일치할 때만 window surface를 만들거나 교체한다. `0×0` publication은 surface create 없이 destroy/suspend로 처리하고, `eglMakeCurrent`/`eglSwapInterval(1)` 상태를 recreate마다 복원한다.
3. raster thread는 Skia clear/flush/submit 뒤 실제 `eglSwapBuffers` 성공을 terminal result로 돌려주며, first successful swap 후에만 platform thread가 F2 show gate를 연다. fixed-size ANGLE, `SetSourceSize`, `ResizeBuffers`, dual front, Arm N/ContentIsland 경로는 contract로 금지했다.
4. source fingerprint `637349ef9c27e01b3f105d5deeb0c086d2ecfacbbac4d1c548f73ff5325a395e`의 restricted-PATH self-contained live gate는 dedicated MTA raster thread에서 child window surface recreate 1,000회, create/destroy `1002/1002`, recreate `1001`, 실제 swap attempt/success `1001/1001`, exact mismatch/zero-size create/terminal failure/thread-affinity violation 모두 0을 기록했다.
5. deterministic context/device-loss injection은 1회 detect/recover 뒤 valid real swap을 PASS했고 per-window EGL/Skia leak, GDI/USER growth는 0/bounded였다. observed ANGLE AMD hardware renderer는 기록했지만 visible/hardware-visible PASS claim은 모두 false이며 F5/FG까지 **notVerified**다.

작업:

1. child HWND용 exact-size EGL window surface를 raster thread에서 만든다.
2. 크기 변경 시 기존 surface를 destroy하고 target 크기로 재생성한다.
3. EGL current/context/swap interval 상태를 복원한다.
4. Skia render target을 새 surface framebuffer와 연결한다.
5. raster → Skia flush/submit → `eglSwapBuffers`의 성공/실패를 terminal로 기록한다.
6. shared context/resource cache와 window surface lifetime을 분리한다.
7. context/device loss injection과 재생성 경로를 만든다.

PASS 조건:

- create/recreate/swap이 raster thread 외에서 호출된 횟수 0
- exact surface extent mismatch 0
- resize 1,000회에서 EGL/Skia resource leak 0
- context loss injection 뒤 valid frame으로 복구
- software/WARP run은 hardware visible PASS로 승격하지 않음

### F5 — Flutter-style bounded resize handshake

상태: **fixture PASS / 현재 제품 hot path의 synchronous present wait는 visible regression으로 제거**

선행 조건: F4 PASS

현재 evidence:

1. `FlutterWindowsResizeHandshake`은 `ResizeStarted → FrameGenerated → SurfaceReady → Presented → Done`의 immutable generation/extent ledger를 소유한다. platform thread는 최대 100ms 동안 engine task runner만 polling하며 arbitrary nested Win32 dispatch를 열지 않는다. timeout/failed/superseded/suspended는 각각 exactly-once terminal로 닫히고 timeout은 실제 child rect 재관측 뒤 latest redraw를 요청한다.
2. `FlutterWindowsResizeRasterPresenter`는 F4 raster owner에서만 exact frame을 surface recreate/swap에 넘긴다. successful swap이 `Done`을 게시한 뒤 platform poll이 unblock되고, 같은 raster thread의 `DwmFlush`가 ordering point로 실행된다.
3. source fingerprint `5bc89c5739147494fc80bbf7a053c410f32cbe4ffdfe54672d4f079330865715`에 결속된 restricted-PATH self-contained live gate는 여덟 edge/corner 각각 3회, 총 24 exact present를 PASS했다. normal timeout, exact mismatch, terminal missing/duplicate, nested dispatch, Done 전 `DwmFlush`는 모두 0이고 post-unblock `DwmFlush`는 24회였다.
4. deterministic timeout fault는 child rect 재관측/latest redraw를 기록하고 UI deadlock/infinite wait 없이 `TimedOut` terminal 1회로 닫혔다. `TimedOut`/`Failed`/`Superseded`/`Suspended` matrix도 각각 정확히 1회 terminal을 확인했고 EGL/Skia leak 0, GDI/USER bounded를 기록했다. AMD hardware ANGLE renderer는 관측했지만 visible/hardware-visible PASS claim은 false이며 F6~FG는 **notVerified**다.
5. 제품 연결 검토에서 Doroti framework callback이 platform STA에 co-located인데도 fixture의 present wait를 그대로 적용한 것이 발견됐다. child `WM_SIZE`가 framework build/layout/scene 뒤 EGL recreate/swap까지 기다려 다음 native resize 전달을 늦췄다. 제품은 synchronous present wait를 0회로 만들고 exact terminal/DwmFlush를 raster completion에 남겼다. 별도 UI thread 없는 현재 구조에서 fixture와 제품의 wait 정책은 의도적으로 다르다.

작업:

1. `ResizeStarted → FrameGenerated → SurfaceReady → Presented → Done` 상태를 구현한다.
2. 별도-thread handshake fixture는 platform thread가 metrics를 보낸 뒤 최대 100ms 동안 engine task runner만 polling한다. co-located 제품 hot path는 scene dispatch 뒤 즉시 반환한다.
3. raster thread는 exact generation/extent frame에서만 surface를 재생성한다.
4. successful swap/present 후 `Done`을 게시해 platform thread를 깨운다.
5. unblock 후 raster thread에서 `DwmFlush` ordering point를 실행한다.
6. timeout/failed/superseded/suspended를 exactly once terminal로 닫는다.
7. timeout 뒤 실제 child rect를 재관측하고 latest redraw를 요청한다.

PASS 조건:

- exact generation/extent가 아닌 resize present 0
- transaction terminal 누락/중복 0
- normal drag의 resize timeout 0
- fault injection timeout에서 UI deadlock/무한 wait 0
- platform thread가 arbitrary nested Win32 message를 dispatch한 횟수 0
- left/right/top/bottom/corner에서 동일 protocol 사용

중단 조건:

- timeout을 늘려야만 정상 drag가 진행됨
- WndProc가 GPU fence나 raster lock을 직접 기다림
- 방향별 special-case anchor/clip/front가 다시 필요해짐

### F6 — scheduler, vsync, ordinary frame 통합

상태: **PASS — native DWM timing, dedicated raster causal chain, and deterministic 60/120/144/165Hz scheduler matrix**

선행 조건: F5 PASS

현재 evidence:

1. `FlutterWindowsFrameScheduler`은 view별 single pending slot을 소유한다. pending/in-flight resize는 ordinary frame을 evict/reject하고, stale epoch/extent는 raster admission과 present 전에 거부한다. hidden/minimized/suspended 동안 scheduling을 멈추고 restore 때 현재 F3 metrics로 framework frame을 다시 요청한다.
2. `FlutterWindowsDwmVsyncSource`는 bound child HWND를 provenance로 보존하되 Windows 8.1+ 규칙에 맞춰 `DwmGetCompositionTimingInfo(NULL, ...)` desktop-composition timing을 사용한다. native `DWM_TIMING_INFO`의 pack/size와 HRESULT를 검증하고 F4의 raster-thread `eglSwapInterval(1)` policy를 변경하지 않는다.
3. `FlutterWindowsScheduledRaster`는 platform callback에서 dedicated MTA raster owner로만 post한다. callback/raster/F4 swap/Skia receipt에는 causal ID와 monotonic timestamp가 연결되어, real swap과 receipt가 모두 exact metrics를 유지할 때만 presented로 센다.
4. source fingerprint `c54e8e07973726206345198bf771636b35f230a98c5758c5ab25d710f5eca600`의 restricted-PATH self-contained live gate는 native DWM timing 26회, fallback 0, NULL timing call/HRESULT 0을 기록했다. 두 independent view에서 callback=raster=swap=presented 26회, queue max 1, starvation/stale/causal gap/EGL·Skia leak 0을 PASS했다.
5. deterministic 60/120/144/165Hz matrix는 각 row의 target cadence, ordinary resume, bounded queue, stale/wrong-size present 0을 PASS했다. 이는 scheduler timing contract이며 physical scan-out cadence, blank/white frame absence, compositor-visible acceptance는 FG까지 **notVerified**다.
6. 2026-08-25 회귀 수정은 아직 시작하지 않은 callback만 latest metrics의 새 ticket으로 교체하고, active resize 중 framework가 요청한 ordinary continuation 한 개를 보존한다. 이 규칙이 없으면 framework의 `_hasScheduledFrame`은 true인데 host queue에는 callback이 없는 상태가 되어 resize frame이 끊겼다. deterministic regression과 기존 F6 validator는 warning/error 0으로 PASS했다.
7. 누적 현재 폭을 계속 줄여 최소 폭에 고정되던 기존 rapid smoke도 initial base 폭 기준 왕복 패턴으로 수정했다. corrected 8초 제품 run은 exit 0, `resizeCallbacks=518`, `resizeMerged=13`, `presented=358`, `resizeDone=352`, `resizeDwmFlush=352`, `queueMax=1`, `stalePresent=0`, `resizePlatformWaits=0`을 기록했다. platform metrics/scene dispatch는 총 2,334,876µs/518회, 최대 28,541µs였다. 이는 runtime contract 증거이며 visible continuity PASS는 아니다.
8. 좌·상 미세 떨림은 DPR 산식이 아니라 top-level 원점과 크기가 함께 바뀌는 leading-edge geometry가 exact backing paint보다 먼저 admission되던 순서 문제로 확인했다. `WM_WINDOWPOSCHANGING` proposal은 좌·상 interactive 경로에서만 immutable metrics/frame을 선준비하고, visible present가 아니라 비가시 backing 준비만 최대 8ms 기다린다. 고정-capacity backing의 clear/copy도 가상 데스크톱 전체가 아닌 exact child client rect로 제한했다. 수정 후 좌측+상단 실제 border-drag 자동 입력은 `leadingEdgePrepared=477`, `leadingEdgeAdmitted=477`, `leadingEdgeAdmissionBeforePreparation=10`, `presented=717`, `stalePresent=0`, `failed=0`, exit 0을 기록했다. F6/top-level/EGL focused validator와 Windows Release build도 PASS했지만 직접 visible 판정은 아니다.
9. 사용자는 그 뒤 네 edge 모두에서 빠른 drag가 창 위치·크기를 따라오지 못하는 것처럼 보인다고 보고했다. 사람이 테두리를 잡을 시간을 둔 actual mouse-input probe의 최종 stress 기준은 `600 physical px / 150ms`다. 측정된 이동 시간은 좌/우/상/하 `160.4/150.1/150.1/150.1ms`였고, `stalePresent=0`, `failed=0`, platform callback 최대 `267µs`였지만 `submitted/scenePresented/superseded`는 각각 `17/11/6`, `17/6/11`, `30/15/15`, `22/11/11`이었다. 이 값에는 ordinary frame이 섞여 있어 resize 표시율로 직접 환산하지 않지만, native callback block보다 최신 크기에 밀려 폐기되는 causal work가 큰 병목 후보임을 보여 준다. 자동 counter는 사용자의 visible FAIL을 뒤집지 않는다.

작업:

1. display refresh와 ANGLE swap interval을 기준으로 Windows vsync source를 구성한다.
2. resize frame과 ordinary animation frame의 우선순위/병합 규칙을 고정한다.
3. pending resize generation이 있을 때 wrong-size ordinary frame을 present하지 않는다.
4. frame callback, raster, swap, presented timing을 causal ID로 연결한다.
5. hidden/minimized 상태에서는 scheduling을 멈추고 restore 때 latest metrics로 재개한다.
6. multi-window 확장을 고려해 view별 state/surface/callback을 분리한다.

PASS 조건:

- animation/resize starvation 0
- frame queue 무한 증가 0
- stale/wrong-size present 0
- 60/120/144/165Hz 사용 가능 환경에서 cadence 목표 충족
- resize 종료 후 ordinary animation이 유한 시간 안에 정상 cadence로 복귀

### F6-R — high-speed interactive resize 추종 안정화

상태: **계획 확정 / 구현 notStarted / 자동 전체화면 capture·log notVerified**

선행 조건: F6의 exact generation, single pending slot, stale/wrong-size present 0 계약을 보존한다.

문제 정의:

1. 최종 primary stress는 한 edge를 `600 physical px / 150ms`에 이동하는 약 4,000px/s actual mouse drag다. 자동 입력은 창 표시 후 warmup, edge hover, mouse-down hold를 거쳐 실제 resize hit-test가 성립한 뒤에만 이동을 시작한다.
2. 좌·상은 `WM_WINDOWPOSCHANGING` proposal에서 선준비하지만 우·하는 actual child `WM_SIZE` 뒤에 framework/raster work를 시작한다. 300ms와 150ms run 모두 우·하에서 useful exact scene보다 superseded work가 크게 늘었다.
3. 현재 platform resize callback 최대치는 267µs이고 stale/wrong-size present와 failure는 0이다. 따라서 좌표/DPR 산식을 먼저 바꾸지 않는다. native moving edge 자체의 추종과 child content의 exact-present cadence를 별도 timestamp로 분리해 어느 경계에서 늦는지 확정한다.
4. 기존 좌·상 저속 보강과 client-only GPU copy는 보존하되, 그 자동 counter를 고속 all-edge visible PASS로 승격하지 않는다.
5. 사용자의 눈으로 매 수정본을 반복 계측하지 않는다. 전체화면 capture에서 보이는 cursor·window border·client pixels와 내부 causal log를 동기화해 최초 불일치 frame과 generation을 찾는 것을 구현 반복의 판정 기준으로 삼는다.

구현 작업:

1. stress probe가 고속 drag 시작 250ms 전부터 mouse-up 500ms 뒤까지 앱 client crop이 아닌 **대상 monitor 전체 physical frame**을 native refresh cadence로 수집하게 한다. 다중 monitor에서는 앱이 있는 monitor 전체를 캡처하고 monitor rect·virtual-screen origin을 provenance에 남긴다.
2. capture는 drag 중 PNG encoding이나 disk write를 하지 않고 bounded in-memory/GPU ring에 원본 frame과 QPC timestamp를 보관한 뒤 mouse-up 후 artifact로 기록한다. capture backend, resolution, requested/observed cadence, dropped/duplicated frame 수를 남긴다.
3. raw 전체화면 frame을 보존한다. capture backend가 hardware cursor를 포함하지 않으면 동일 QPC의 `GetCursorPos`를 별도 기록하고 분석용 사본에만 cursor marker를 합성한다. 원본 capture를 marker가 들어간 파생 이미지로 대체하지 않는다.
4. 같은 monotonic timeline에 입력 target/actual cursor, `WM_WINDOWPOSCHANGING`, top-level `GetWindowRect`, child `GetClientRect`, metrics 적용, framework callback, raster 시작/준비, exact admission, swap/present, `WM_EXITSIZEMOVE`와 resize/ordinary 구분을 JSONL로 기록한다.
5. 각 run은 raw full-screen frames, timestamp index, causal JSONL, summary, first-bad-frame 전후 contact sheet를 한 artifact bundle로 남긴다. frame별 cursor↔moving-edge 거리, border↔child extent, pixels↔presented generation을 계산해 최초 지연 구간과 worst generation을 표시한다.
6. capture 자체가 원인이 되지 않도록 동일 입력의 log-only run과 capture+log run을 짝지어 actual drag duration, native message latency, present cadence를 비교한다. capture 때문에 input duration이 10% 넘게 변하거나 frame drop이 생기면 그 run은 원인 판정에 쓰지 않는다.
7. 최초 불일치 경계에 따라 owner를 고친다. cursor보다 top-level edge가 먼저 늦으면 platform hot path의 polling/wait/message work를 줄이고, native/child geometry는 맞지만 pixels만 늦으면 framework mailbox·scheduler·raster admission/present를 고치며, exact swap 뒤 desktop frame만 늦으면 DirectComposition commit/DWM ordering을 고친다.
8. pixels 경계가 원인일 때의 우선 후보는 좌·상 전용 proposal path의 all-edge/corner 일반화와 아직 시작하지 않은 framework/raster work의 latest-only 병합이다. 이는 capture+log가 해당 경계를 지목한 뒤 적용하며, proposal은 matching child `WM_SIZE` 전 visible authority가 될 수 없다.
9. matching `WM_SIZE`는 같은 immutable proposal generation을 admit한다. proposal과 actual child extent가 다르면 proposal을 cancel하고 actual metrics로 새 generation 하나만 요청한다. queue depth 1과 exact generation/extent를 유지한다.
10. `WM_EXITSIZEMOVE`에서 unadmitted proposal을 정리하고 actual child rect의 final exact frame을 반드시 요청한다. mouse-up, close, minimize, DPI/display transition이 proposal wait나 GPU fence 뒤에 막히지 않게 한다.
11. 한 번의 가설 수정마다 동일 seed·방향·시작 rect의 capture+log bundle을 다시 생성해 first-bad-frame, p95/max gap, superseded work가 실제로 개선됐는지 비교한다. 개선이 없으면 변경을 완료로 간주하지 않고 다음 causal 경계를 조사한다.

금지 조건:

- matching child `WM_SIZE` 전 provisional frame의 visible present
- resize generation/extent가 actual child와 다른 stale present
- `ResizeBuffers`, `SetSourceSize`, full-frame stretch, dual visible front 재도입
- WndProc의 GPU fence, swap/present, composition commit 완료 대기
- 600px/150ms 입력을 debounce해 마지막 크기만 늦게 표시하는 방식
- 좌/우/상/하마다 별도 좌표 산식 또는 anchor 보정 추가

검증 순서:

1. baseline: 네 edge 각각 `600px/150ms` expand를 log-only와 full-screen capture+log로 한 쌍씩 실행해 observer overhead와 최초 지연 경계를 확정한다. 측정 actual duration이 180ms를 넘으면 invalid run으로 재실행한다.
2. 수정 반복: first-bad-frame 직전·직후 전체화면 pixels와 동일 timestamp의 causal events를 비교해 owner를 하나씩 수정하고, 같은 seed/start rect/input trace로 before/after bundle을 생성한다.
3. primary stress: 원인 수정 뒤 네 edge와 네 corner 각각 expand, shrink, immediate reverse를 `600px/150ms`로 3회 실행한다. 모든 valid run에 raw frames, contact sheet, causal log와 summary가 있어야 한다.
4. cadence comparison: 같은 경로를 `600px/300ms`와 slow/fine drag로 반복해 고속 수정이 저속 좌·상 미세 떨림을 되돌리지 않았는지 full-screen frame sequence로 확인한다.
5. size/DPI/refresh: 420×300, 640×360, 1000×600, work-area 근접 크기와 가능한 100/125/150/200%, 60/120/144/165Hz에서 실행한다.
6. focused regression: F2 top-level, F4 surface, F6 scheduler validator와 Windows Release build를 20분 제한으로 실행한다.
7. F6-R 수정 과정에서는 사용자에게 매 run 확인을 요청하지 않는다. 자동 capture+log가 모두 PASS한 final binary만 FG의 1회 사용자 제품 수용 대상으로 넘긴다.

PASS 조건:

- actual moving edge가 cursor 진행 방향을 거슬러 되돌아가거나 멈춰 누적 지연되지 않음
- mouse-up 뒤 최종 edge/client extent가 1 refresh interval 안에 target의 1 physical px 이내로 수렴
- full-screen frame sequence에서 moving border와 client pixels가 3 capture frame 이상 정지했다가 건너뛰거나 역방향으로 보이는 구간 0
- active drag 중 exact resize-present 간격 p95가 2 refresh interval 이하이고 max가 3 refresh interval 이하
- `queueMax <= 1`, stale/wrong-size present 0, failed 0, terminal 누락/중복 0
- platform hot path의 GPU/present wait 0; message dispatch p99 1ms 이하, max 4ms 이하
- final current metrics, framework metrics, surface metrics, child client extent가 같은 generation/extent로 종료
- 네 edge/corner의 expand/shrink/reverse 3회에서 visible freeze, pointer-border 역방향 jitter, white/black band, stale edge, raster flicker 0
- capture requested/observed cadence 일치, dropped capture frame 0, log-only 대비 capture run의 actual drag duration·native latency 변화 10% 이하
- raw full-screen frames와 causal log가 같은 first-bad-frame/root owner를 지목하고, 수정 후 해당 불일치가 모든 primary stress run에서 0
- 위 자동 capture+log 조건이 모두 PASS할 때 F6-R을 닫고, 사용자 확인은 FG final binary의 1회 제품 수용으로만 수행

### F7 — input, focus, IME, clipboard, accessibility

상태: **PASS — automated raw-child-HWND contract + user physical/manual acceptance**

선행 조건: F6 PASS

현재 automated evidence:

1. `FlutterWindowsHostWindow`의 typed child-message hook 뒤에 child-only router, `FlutterWindowsInputHost`, single `FlutterWindowsKeyboardManager`, IMM32 text owner, child-only `FlutterWindowsUiaBridge`를 연결했다. top-level WndProc/non-client cursor/accessibility는 intercept하지 않는다.
2. keyboard manager는 modifier/dead key/UTF-16 surrogate pair를 한 stream으로 직렬화하고 system key/char는 DefWindowProc으로 넘긴다. 기존 자동 gate가 놓친 `Ctrl+A/C/V/X` C0 `WM_CHAR`, handled Enter 뒤 `\r`, Backspace 제어문자 literal commit을 억제하고 각각 독립 counter/regression으로 고정했다. IMM32는 child client/screen pixel caret 및 candidate rect를 F3 metrics에서 계산하며 `ImmNotifyIME` exact export를 사용한다.
3. UIA bridge는 `WM_GETOBJECT`의 child root만 반환하고 immutable semantics snapshot의 Invoke/Value/Scroll action을 `IFlutterWindowsEngineTaskRunner`로 enqueue한다. child당 root provider는 하나이며 stale fragment는 disconnect한다. semantics scroll position/extents도 framework contract에서 보존한다.
4. 당시 F7 static/self-contained restricted-PATH gate가 source fingerprint `274297c68673341c5489effe6f53faee990f872a1075b92b4c8322963e11c742`에서 PASS했다. fresh fixture build는 warning/error 0이었다.
5. latest live evidence `Doroti/artifacts/windowsappsdk-flutter-input/f7-live-20260825-024636-99c63e210a864335bc62ad9551384477/f7-live-evidence.json`는 actual child HWND/typed WndProc/UIA `WM_GETOBJECT`, outside-up/cancel/capture, child focus/cursor, IMM32 candidate/caret, UIA root/fragments/actions, engine action `Invoke/Value/Scroll`, GDI/USER boundedness을 재확인했다. 새 keyboard regressions는 shortcut/action/non-text control suppression을 각각 `1/1/1`, editing-state 오염 0으로 PASS했다.
6. 당시 visible manual gate는 실제 observer가 여섯 항목을 본 뒤 F1~F6으로 개별 확인하고 F8로 명시 완료한 경우에만 current source fingerprint에 결속된 manual PASS를 기록했으며, Esc/close/timeout/incomplete는 `notVerified`로 실패했다. selection/copy/cut/paste용 최소 framework-side editor와 right-click popup도 포함했다.
7. 2026-08-25 incomplete manual run `Doroti/artifacts/windowsappsdk-flutter-input-manual/f7-manual-20260825-024653-641938da97ec4e359f2dea569bfa7f99/f7-manual-evidence.json`은 F-key/F8 completion을 기록하지 못했다. 사용자는 이후 같은 기능을 직접 확인하고 “전부 통과”로 수용했으며, 별도 user-attestation `Doroti/artifacts/windowsappsdk-flutter-input-manual/f7-user-attestation-20260825-024902.json`에 여섯 physical 항목 PASS와 source fingerprint를 기록했다. incomplete fixture telemetry 자체는 수정하지 않고 두 provenance를 분리 보존한다.
8. fixture는 auxiliary island를 만들지 않았으며 primary metrics/surface/present cadence를 변경하지 않았다. auxiliary island와 visible FG는 계속 `notVerified`이고 F7 PASS 범위에 포함하지 않는다.

작업:

1. child HWND WndProc에서 mouse/pointer/touch/pen/wheel을 Doroti event로 변환한다.
2. add/hover/down/move/up/remove/cancel과 capture lifecycle을 보존한다.
3. key down/up, modifier, dead key, surrogate pair를 단일 keyboard manager로 직렬화한다.
4. TSF 또는 IMM32 중 하나를 child text input owner로 확정한다.
5. 한국어 composition/candidate/caret/selection/clipboard를 Doroti text model에 연결한다.
6. child HWND UIA root provider 아래 semantics tree와 Invoke/Value/Scroll pattern을 구현한다.
7. standard top-level non-client focus/cursor/accessibility는 Windows 기본 동작을 보존한다.
8. 실제 Windows App SDK content가 필요한 최소 fixture만 bounded `DesktopChildSiteBridge`로 만들고 primary child와 영역을 겹치지 않는다.
9. auxiliary island가 있을 때 pointer, keyboard/IME, focus, UIA parent-child owner를 명시하고 왕복을 검증한다.

PASS 조건:

- content와 resize border cursor ownership 충돌 0
- capture 중 child 밖 이동 후 up/cancel 정상
- Alt+Tab, minimize/restore, popup 왕복 뒤 focus 정상
- 한국어 두벌식 조합/후보창/caret/selection/clipboard 정상
- Narrator/Accessibility Insights에서 중복 root나 끊긴 tree 0
- resize/DPI 변경 뒤 input/UIA bounds가 화면 pixels와 일치
- auxiliary island가 primary metrics/surface/present cadence를 변경한 횟수 0

### F8 — DPI, monitor, fullscreen, lifecycle, recovery

상태: **구현 완료 / 자동 fixture·제품 smoke PASS / 물리 검증 deferred**

구현 의존성: F7 child HWND/input owner가 존재함. F7의 추가 검증 실행 여부는 F8 구현을 막지 않는다.

현재 구현:

1. `FlutterWindowsHostWindow`가 top-level message를 typed event로 전달하고 `WM_DPICHANGED` suggested rect를 실제 top-level HWND에 적용한다.
2. `FlutterWindowsLifecycleManager`가 DPI/display/fullscreen/restore, minimize, power/session suspend-resume, shutdown terminalization과 graphics recovery callback을 소유한다.
3. `FlutterWindowsHostAdapter`가 lifecycle manager, latest-metrics redraw, graphics recovery, non-blocking exact resize terminal을 실제 제품 composition root에서 사용한다.
4. shutdown terminal callback은 scheduler를 suspend하고 outstanding product resize wait를 실패 terminal로 해제한다.
5. deterministic lifecycle fixture는 source fingerprint `9b6bd3f4bac3ac93a74d4acffca5b8b363d1024d86aab3b9014d0a162566e913`에서 PASS했다. latest evidence는 `Doroti/artifacts/windowsappsdk-flutter-lifecycle/f8-live-20260825-033310-bd794539ccbd4814a4ec1d975c5fd51e/f8-live-evidence.json`이다.
6. 기존 제품 smoke의 `resizeDone=3`, `resizeTimedOut=0`, `resizeSuperseded=7`, `resizeDwmFlush=3`, `stalePresent=0` 기록은 process/lifecycle smoke로만 보존한다. 누적 현재 폭을 줄여 minimum에 고정되던 패턴이어서 rapid continuity evidence로는 폐기했고, corrected 왕복 smoke 결과는 F6 현재 evidence 7항을 사용한다.
7. mixed-DPI cross-monitor, 실제 monitor disconnect/rearrange, sleep/resume, RDP attach/detach, visible black/white stuck recovery는 실행을 미루고 후속 검증 백로그로 보존한다.

구현 결과:

1. `WM_DPICHANGED` suggested top-level rect를 적용하고 child metrics를 새 epoch로 게시한다.
2. current `HMONITOR`, display ID, refresh, DPI 변경을 display manager에 반영한다.
3. initial/restore placement만 current monitor `rcWork`에 맞춘다.
4. fullscreen은 `rcMonitor`와 standard style 전환으로 구현하고 원래 rect/DPI/monitor/work-area를 보존한다.
5. monitor disconnect/rearrange 뒤 restore rect를 새 `rcWork`에 맞추는 recovery를 연결했다.
6. sleep/resume, RDP attach/detach, display change, device/context loss 신호를 recovery path에 연결했다. 실제 물리 전환 검증은 후속 백로그로 둔다.
7. exact self-contained Windows App SDK bootstrap의 runtime fail-fast와 DispatcherQueue/AppWindow teardown을 제품 경로에서 사용한다. runtime absence/update fault injection은 FG deployment 백로그다.
8. shutdown 중 pending resize/frame/surface callback을 terminal 처리한다. FlutterEmbedder primary 경로는 island를 만들지 않는다.

남은 물리 검증 항목(현재 구현을 막지 않음):

- mixed-DPI monitor 이동 중 logical/physical/display identity 불일치 0
- maximize/Snap/fullscreen/restore 뒤 stale surface/input 0
- monitor disconnect 뒤 offscreen window 0
- sleep/RDP/display change 뒤 valid frame 복구
- device/context loss 뒤 black/white stuck surface 없이 복구
- shutdown callback/UAF/crash 0

### F9 — Windows target/runner/package integration

상태: **구현 완료 / build·publish·launch PASS / 물리·negative deployment 일부 notVerified**

구현 의존성: F8 lifecycle API와 Flutter-style host artifact가 build 가능한 상태. F8 물리 검증 완료는 선행 조건이 아니다.

현재 구현:

1. `Doroti.Target.Windows.WindowsAppSdk.win-x64` manifest/description/buildTransitive metadata에 top-level+child HWND, ANGLE/EGL/Skia, FlutterEmbedder/Arm N capability와 MAUI separate-host rollback을 기록했다.
2. `DorotiDemoApp/windowsappsdk` runner가 startup 환경 선택으로 `FlutterEmbedder`와 `ArmNLegacy`를 같은 제품 binary에서 구성한다.
3. 당시 F9 static/live product gate가 static wiring, self-contained win-x64 publish, restricted-PATH launch, native PE architecture/hash, 기본/resize/Arm N smoke를 검사했다.
4. product publish가 `av_libglesv2.dll`과 `libSkiaSharp.dll`의 x64 provenance/hash를 기록하며 PATH fallback 없이 launch했다.
5. `doroti.ps1 -WindowsAdapter FlutterEmbedder|ArmNLegacy|MauiRollback`으로 명시적 선택할 수 있다. MAUI는 workspace의 별도 `DorotiHostKind=Maui` runner를 선택해 Release build가 PASS했다.
6. WindowsAppSdk FlutterEmbedder 및 Arm N launch는 PASS했고 MAUI live launch, native DLL missing/wrong-arch fault injection은 `notVerified`다.

남은 검증 항목:

- 실제 clean checkout x64 전체 restore/build/test/publish/launch PASS
- required native DLL 누락/잘못된 architecture를 startup에서 명확히 fail-fast
- non-Windows target 회귀 0
- MAUI rollback live launch PASS

## 6. F10 구현과 FG 통합 안정화

### F10 — WindowsAppSdk 기본 adapter 전환

상태: **구현 완료 / 자동 product smoke PASS / FG 물리 검증 deferred**

구현 의존성: F9 target/runner/package graph가 연결되고 `ArmNLegacy`/MAUI rollback 선택이 동작 가능한 구조. FG 완료 전에도 기본 선택 wiring을 구현할 수 있다.

현재 구현:

1. `Doroti.Host.WindowsAppSdk`의 production default를 `FlutterEmbedder`로 전환했다.
2. `DorotiDemoApp/windows`, app template, `doroti.ps1 build -Platform windows`, target discovery/descriptor의 기본 graph가 WindowsAppSdk/FlutterEmbedder를 선택한다.
3. Demo 문서와 Release validation 기본 graph를 새 adapter에 맞췄다.
4. `ArmNLegacy` in-process rollback과 MAUI 별도 Windows runner를 보존했다.
5. 기존 host 삭제는 수행하지 않았다. FG 전체 PASS, 사용자 제품 수용, 별도 cleanup 승인 뒤에만 수행한다.
6. latest 통합 evidence는 `Doroti/artifacts/windowsappsdk-flutter-product/product-20260825-033318-0d11c8dda40849a38ca7a75d0da05b6b/product-live-evidence.json`이며 기본 FlutterEmbedder, rapid resize, Arm N smoke가 모두 PASS했다.

남은 검증 항목(FG와 함께 실행):

- template에서 생성한 새 app의 Windows launch PASS
- explicit MAUI rollback host live launch PASS
- non-Windows target 회귀 0
- release artifact provenance가 새 기본 host를 정확히 표시

### FG — Flutter-style Windows 제품 acceptance

상태: **자동 제품 범위 partial PASS / 물리·visible 전체 notVerified**

실행 시점: F8~F10 구현과 제품 binary wiring이 끝난 뒤 통합 안정화 단계에서 실행한다. 개별 미실행 항목은 앞 단계 구현을 막지 않지만 release 확정과 rollback 삭제는 막는다.

현재 자동 범위는 기존 self-contained publish/restricted-PATH/Arm N rollback과 수정 후 local Release build/corrected rapid resize/stale-wrong-size present 0까지 PASS했다. 그러나 사용자가 실제 네 방향 고속 drag의 추종 실패를 확인했으므로 F6-R은 열린 상태다. F6-R의 `600px/150ms` full-screen capture+causal log matrix가 PASS한 뒤에만 아래 FG matrix로 진행한다. 수정 반복 중 사용자 확인은 요구하지 않으며, 자동 evidence가 끝난 final binary만 한 번 사용자 제품 수용으로 넘긴다. 자동 수치는 물리 monitor/lifecycle, 한국어 IME, Narrator/Accessibility Insights 또는 사용자 제품 수용을 대신하지 않는다.

테스트 matrix:

- client size: 420×300, 640×360, 1000×600, monitor work-area 근접 크기
- DPI: 100%, 125%, 150%, 200%
- refresh: 사용 가능한 60, 120, 144, 165Hz
- drag: fine/slow, `600px/300ms`, primary stress `600px/150ms`, expand/shrink, immediate reverse, 네 edge와 네 corner
- display: same-monitor, cross-monitor, mixed-DPI, monitor disconnect/rearrange
- lifecycle: minimize/restore, maximize, Snap, fullscreen, sleep/resume, RDP
- input: mouse, touch/pen 가능 환경, wheel, keyboard, focus, capture
- text: 한국어 IME 조합/후보창/caret/selection/clipboard
- accessibility: Narrator와 Accessibility Insights
- recovery: EGL context/device loss와 failed surface recreation
- deployment: clean publish, native DLL missing/wrong-arch, rollback host
- Windows App SDK: selected runtime provenance, runtime absence/update, DispatcherQueue/AppWindow/island shutdown

PASS 조건:

- correctness, visible continuity, cadence, input, text, accessibility, lifecycle, recovery, deployment가 각각 PASS
- F6-R primary stress full-screen capture+causal log PASS
- 방향별 실제 mouse drag 10초 이상, 3회 연속 valid run
- 검은/흰 영역, 색 띠, stale edge, full-frame stretch, raster flicker 0
- pointer-border 역방향 jitter 또는 장시간 freeze 0
- stale/wrong-size generation present 0
- resize transaction timeout/failed/terminal 누락 0
- surface/context failure가 없거나 fault injection에서 검증된 recovery
- qualified output evidence와 사용자 실제 화면 확인 모두 PASS

한 축이라도 미실행이면 해당 축과 FG 전체는 `notVerified`다. build, contract counter, `eglSwapBuffers` 성공, `DwmFlush` 반환만으로 visible PASS를 선언하지 않는다.

## 7. 자동화와 evidence 규격

각 run은 최소한 다음 provenance를 남긴다.

- git commit/dirty status
- Flutter source pin/hash
- build configuration, RID, target/host kind
- Windows version/build
- ANGLE/EGL/GLES/SkiaSharp version과 native DLL hash/path
- adapter, backend, driver, LUID
- monitor/display ID, refresh, DPI, work-area와 monitor rect
- top-level/child HWND, client physical pixels
- run ID, timestamp, input trace, drag direction/speed
- full-screen capture backend, target monitor/virtual-screen rect, physical resolution, requested/observed FPS, dropped/duplicated frames
- log-only/capture+log paired-run ID와 capture observer overhead
- hardware/WARP/software 구분

resize event JSONL:

- `cursorSample`
- `screenFrameCaptured`
- `windowPosProposed`
- `topLevelRectObserved`
- `childRectObserved`
- `windowSizeObserved`
- `resizeStarted`
- `metricsDelivered`
- `sceneBuilt`
- `frameGenerated`
- `rasterStarted`/`rasterPrepared`
- `provisionalAdmitted`/`provisionalCancelled`
- `surfaceDestroyStarted`/`surfaceDestroyed`
- `surfaceCreated`/`surfaceReady`
- `swapStarted`/`swapCompleted`
- `resizeDone`
- `timedOut`/`superseded`/`failed`/`suspended`
- `dwmOrderingPoint`
- `contextLost`/`contextRecovered`

모든 event는 적용 가능한 `viewId`, `resizeGeneration`, target extent, actual top-level/child extent, cursor position, thread ID, monotonic timestamp를 포함한다. captured frame index도 같은 monotonic timestamp로 causal event에 join할 수 있어야 한다. summary는 count뿐 아니라 p50/p95/p99/max, first-bad-frame, worst generation, cursor-edge lag, exact-present gap, capture drop/observer overhead와 timeout/failed/terminal imbalance를 기록한다.

app counter/log, raw full-screen capture, derived contact sheet, 사용자의 최종 visible 판정은 서로 다른 원본 evidence로 보존한다. 한 종류의 성공을 다른 종류의 PASS로 승격하지 않으며, F6-R 반복 수정은 앞의 두 원본으로 진행하고 사용자 판정은 final binary에서 한 번만 수행한다.

## 8. 후속 문제 대응·rollback 규칙

아래 항목은 다음 구현 단계로 넘어가지 못하게 하는 일반 hard gate가 아니다. 구현 중 치명 오류는 즉시 고치고, 그 밖의 실패는 재현 조건·영향 범위·관련 generation/evidence를 검증 백로그에 남긴 뒤 end-to-end 경로 구현을 계속한다. 안정화 단계에서는 아래 원칙으로 원인을 고치며 단순 timeout 증가나 우회 렌더링으로 숨기지 않는다.

- F1에서 Windows App SDK runtime/AppWindow/DispatcherQueue 또는 ANGLE/Skia ABI·배포가 성립하지 않으면 D3D12/Arm N으로 조용히 대체하지 않는다. 원인과 대안 결정을 기록하고 중단한다.
- F2에서 standard window 기능이 깨지면 custom non-client를 추가하지 않고 top-level/child ownership을 수정한다.
- F4에서 surface recreation이 느리면 monitor-sized capacity, `SetSourceSize`, dual-front를 새 adapter에 넣지 않는다. context/resource lifetime과 surface creation 비용을 먼저 분리한다.
- F5 timeout이 반복되면 100ms를 늘리지 않고 metrics→layout→raster→surface→swap의 worst generation을 고친다.
- F5에서 visible artifact가 남으면 timer/debounce/background/edge별 좌표 보정을 추가하지 않고 handshake와 actual child/surface extent를 다시 검증한다.
- F6 cadence가 부족하면 resize correctness와 cadence를 분리해 원인을 찾되 wrong-size frame을 허용하지 않는다.
- F7 IME/UIA 때문에 full-client XAML/ContentIsland가 필요해 보이면 F7을 FAIL로 기록하고 bounded `DesktopChildSiteBridge` 최소 spike로 원인을 분리한다.
- F8 monitor 이동 시 fixed primary work-area로 clamp하지 않는다. 현재 child가 속한 monitor/DPI/display를 다시 관측한다.
- FG 전체 PASS, 사용자 제품 수용, 별도 cleanup 승인 전에는 Arm N/MAUI rollback을 삭제하지 않는다.
- rollback은 startup adapter/host 선택으로 수행하고, 한 process/view 안에서 Arm N D3D12 state와 Flutter-style EGL state를 혼합하지 않는다.

## 9. 예상 repository 변경 범위

주로 개편할 기존 영역:

- `Doroti/src/Doroti.Host.WindowsAppSdk/`
- `Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64/`
- `DorotiDemoApp/windowsappsdk/`
- WindowsAppSdk/Flutter-style/ANGLE contract 및 live validation
- runner/template/package graph의 WindowsAppSdk adapter selection

연결할 기존 영역:

- `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`
- `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- engine/view metrics, pointer, keyboard, text, semantics contract
- `DorotiDemoApp/windows`

보존할 rollback/diagnostic 영역 또는 구현:

- `ArmNDualFrontPresenter`와 기존 Arm N adapter
- 기존 full-client `WindowsAppSdkIslandBridge`는 Arm N rollback 전용
- `Doroti/src/Doroti.Host.Maui`
- `Doroti/src/Doroti.Target.Windows.Maui.win-x64`
- `Doroti/validation/windows-top-level-presentation`

새 Flutter-style path는 FG 전에도 F10 기본 선택 wiring까지 구현할 수 있다. 다만 FG 전체 PASS 전에는 기존 영역을 삭제·release 확정하거나 과거 evidence를 새 경로의 성공으로 재해석하지 않는다.

## 10. 정확한 다음 작업

1. F8~F10 구현과 제품 연결은 완료됐다. platform synchronous present wait, pending-frame continuation, rapid smoke 패턴과 좌·상 leading-edge 선준비/client-only GPU copy 수정은 기존 evidence로 보존하되 고속 all-edge 완료 증거로 해석하지 않는다.
2. 다음 exact resume point는 F6-R full-screen capture+causal log harness다. `600px/150ms` drag의 250ms 전부터 mouse-up 500ms 뒤까지 대상 monitor 전체 frame을 수집하고 cursor, top-level/child geometry, framework metrics, raster preparation, exact admission, swap/present를 같은 QPC timeline으로 기록한다.
3. 먼저 네 edge의 log-only/capture+log paired baseline을 만들어 capture observer overhead를 배제한다. 각 run의 raw frame sequence와 first-bad-frame contact sheet를 로그에 join해 native geometry, child geometry, framework/raster, composition/DWM 중 최초 지연 owner를 확정한다.
4. 확인된 owner만 수정한다. pixels 경계가 원인이면 all-edge/corner proposal 선준비와 latest-only framework/raster mailbox를 우선 검토하되, matching `WM_SIZE` 전 visible present 금지, actual child exactness, queue depth 1, platform GPU/present wait 0은 유지한다.
5. 동일 seed/start rect/input trace로 before/after capture bundle을 비교하고 개선되지 않은 가설은 완료 처리하지 않는다. 원인이 없어질 때까지 capture+log → first-bad-frame 분석 → scoped 수정 → 동일 run 재검증을 반복하며 사용자에게 중간 확인을 요청하지 않는다.
6. 네 edge/corner의 expand/shrink/reverse를 `600px/150ms`로 3회, `600px/300ms`와 slow/fine drag로 회귀 실행한다. F2/F4/F6 validator와 Windows Release build도 20분 제한으로 재실행한다.
7. F6-R 자동 capture+log gate가 모두 PASS한 final binary만 사용자가 같은 `dotnet run --project ./DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj` 명령으로 한 번 확인한다. 이후 FG의 4개 client size, 가능한 DPI/refresh, mixed-DPI/lifecycle/input/recovery matrix로 이동한다.
8. clean generated-template launch, MAUI rollback live launch, Windows App Runtime absence/update, native DLL missing/wrong-arch와 DispatcherQueue shutdown fault를 별도 deployment evidence로 남긴다.
9. 한 축이라도 미실행이면 FG 전체는 `notVerified`로 유지한다. FG 전체 PASS와 사용자 제품 수용 뒤에만 release 확정과 기존 host cleanup을 수행한다.

현재 상태를 다음처럼 오해하면 안 된다.

- Flutter-style host의 F1~F10 제품 경로가 구현됐다: **맞음**. 자동 build/publish/launch와 기존 rapid-resize/rollback smoke도 PASS했다. 다만 F6-R 고속 all-edge와 FG 물리·visible 전체 acceptance는 아직 `notVerified`다.
- 기존 Arm N을 조금 수정하는 계획이다: **아님** — 같은 WindowsAppSdk host package 안의 별도 child/EGL adapter다.
- monitor work-area를 render envelope/capacity로 계속 사용한다: **아님**
- work-area를 전혀 사용하지 않는다: **아님** — initial/restore/popup placement에만 사용한다.
- Windows App SDK 2.4가 제품 기반이다: **맞음** — exact pin, AppWindow, DispatcherQueue, self-contained runtime을 사용한다.
- Windows App SDK가 primary render size/present를 소유한다: **아님** — Flutter식 child HWND와 raster thread가 소유한다.
- full-client ContentIsland가 필수다: **아님** — bounded auxiliary content에만 허용한다.
- pinned Flutter runtime을 같이 빌드·계측한다: **아님** — source-only protocol reference다.
- 기존 Arm N/MAUI rollback을 지금 삭제한다: **아님**
- 이번 문서 개편이나 검증 연기가 runtime resize 문제 해결을 증명한다: **아님**

## 11. 기준 source

Flutter source pin:

- `reference/flutter-master`: `56b8e1a851a594b1a154f8ea93270807dab22b9a`

핵심 reference:

- `reference/flutter-master/engine/src/flutter/shell/platform/windows/host_window.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_window.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.h`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/egl/window_surface.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/egl/manager.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/task_runner.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/text_input_manager.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/accessibility_bridge_windows.cc`
- `reference/flutter-master/engine/src/flutter/shell/platform/windows/display_manager.cc`

Windows App SDK 공식 reference:

- DispatcherQueue와 AppWindow lifecycle: <https://learn.microsoft.com/en-us/windows/apps/develop/dispatcherqueue>
- `DesktopAttachedSiteBridge`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.desktopattachedsitebridge>
- `DesktopChildSiteBridge`: <https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.desktopchildsitebridge>
- `ContentIsland`: <https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island>

이 문서의 단계와 상태가 새 Windows 제품 경로의 active 기준이다. 구현 상태(`notStarted/inProgress/implemented`)와 검증 상태(`deferred/notVerified/PASS/FAIL`)를 분리해 갱신하고, 새 evidence나 후속 문제에는 exact resume point를 함께 기록한다.

# Doroti Windows App SDK 기반 Flutter-style embedder 전환 계획

## 0. 문서 목적과 최종 결정

- 전면 개편일: 2026-08-24
- 대상: Doroti Windows 제품 호스트, interactive resize, GPU surface, 입력·IME·접근성·lifecycle·배포
- Flutter 기준 source: repository의 `reference/flutter-master` commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- 목표 제품 호스트: `Doroti.Host.WindowsAppSdk`
- 목표 Windows target: `Doroti.Target.Windows.WindowsAppSdk.win-x64`
- 목표 창 구조: **표준 Win32 top-level HWND + client 전체를 채우는 child render HWND**
- 목표 그래픽 기준선: **ANGLE/EGL window surface + Skia GPU**, exact client physical size
- Windows App SDK 기준선: **exact `2.4.0`, self-contained unpackaged, raw HWND + AppWindow + DispatcherQueue**
- 문서 성격: ordered 구현 계획. 이번 개편은 계획만 바꾸며 구현·build·runtime acceptance를 의미하지 않는다.

기존 `Doroti.Host.WindowsAppSdk`의 `fixed work-area envelope + custom non-client + Arm N dual-front` 구현을 **Windows App SDK 기반 Flutter Windows embedder 방식**으로 전환한다. Windows App SDK 제품/target/runner 경계는 유지하되, 그 안의 window tree와 resize/surface lifecycle을 Flutter 방식으로 교체한다.

여기서 “Flutter 방식”은 이름이나 API를 흉내 내는 것이 아니라 다음 ownership과 lifecycle을 채택한다는 뜻이다.

1. Windows가 표준 top-level 창의 non-client, move, resize, Snap, system menu, maximize/minimize를 소유한다.
2. 하나의 child HWND가 top-level client rect를 그대로 채우며 rendering, pointer, keyboard, text, accessibility의 native view 경계가 된다.
3. child `WM_SIZE`의 physical pixel 크기와 현재 DPI/display ID를 framework metrics의 authority로 사용한다.
4. resize target을 framework에 전달한 뒤 raster thread가 같은 크기의 frame과 window surface를 준비한다.
5. 그 frame의 buffer swap/present가 끝날 때까지 platform thread가 제한 시간 동안 engine task만 처리하며 resize handshake를 수행한다.
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
| F0-V source pin/anchor validator | diagnosticOnly | 현재 untracked validator/contract가 source-only PASS(`10 files/12 anchors/4 mappings`)했다. repository 채택과 clean-checkout 재실행 전에는 정식 PASS가 아니다. |
| F1 Windows App SDK/ANGLE bootstrap preflight | notStarted | 기존 host에서 exact 2.4 AppWindow/DispatcherQueue와 ANGLE native dependency 경계를 아직 결합하지 않았다. |
| F2 표준 top-level + child view | notStarted | Flutter식 window tree를 아직 구현하지 않았다. |
| F3 metrics/display contract | notStarted | child client physical pixels 기반 metrics를 아직 연결하지 않았다. |
| F4 raster-thread EGL surface lifecycle | notStarted | exact-size EGL window surface create/destroy/recreate/swap을 아직 구현하지 않았다. |
| F5 bounded resize handshake | notStarted | `ResizeStarted → FrameGenerated → Done` 동기화를 아직 구현하지 않았다. |
| F6 scheduler/vsync/present integration | notStarted | ordinary frame과 resize frame의 통합 cadence를 아직 구현하지 않았다. |
| F7 input/IME/accessibility | notStarted | child HWND 중심의 전체 native contract를 아직 구현하지 않았다. |
| F8 DPI/display/lifecycle/recovery | notStarted | mixed-DPI, monitor, fullscreen, device/context loss를 아직 구현하지 않았다. |
| F9 target/runner/package integration | notStarted | 기존 WindowsAppSdk target/runner/package에 Flutter-style adapter와 native artifact graph를 아직 통합하지 않았다. |
| FG Windows 제품 acceptance | notVerified | 새 제품 binary와 visible/input/text/UIA/lifecycle/deployment matrix가 존재하지 않는다. |
| F10 default adapter cutover | notStarted | FG PASS 뒤 기존 WindowsAppSdk host의 기본 adapter를 Flutter-style로 전환하는 단계다. |

기존 증거는 다음처럼 재분류한다.

- Arm N의 고속 좌측·상단 manual PASS는 **기존 구조의 scoped regression PASS**로 보존한다.
- 기존 M1 observer FAIL과 G2 `notVerified`는 과거 구조의 판정으로 보존한다.
- 새 Flutter-style 경로는 F1부터 독립적으로 구현·판정한다.
- 기존 observer는 diagnostic으로 재사용할 수 있지만, 불안정한 observer 한 종류를 F1~F8 구현 시작의 hard stop으로 두지 않는다.
- FG visible PASS에는 qualified output evidence와 사용자의 실제 mouse drag 확인이 모두 필요하다.
- Flutter runtime capture/A-B instrumentation은 범위 밖이다. pinned Flutter source를 protocol reference로만 사용한다.

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

Doroti는 이 상태 전이를 그대로 기준선으로 삼되, 모든 target에 monotonic `resizeGeneration`을 추가해 timeout이나 재진입 시 stale frame을 거부한다.

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
  - engine task polling
  - focus, lifecycle, display events
- framework/UI thread
  - metrics 적용, layout, scene 생성
  - resize generation을 보존한 frame request
- raster thread
  - EGL context와 window surface의 유일한 owner
  - exact-size Skia target 생성
  - raster, flush, swap/present
  - presented/failed terminal 보고

WndProc가 직접 Skia draw, GPU fence wait, surface destroy를 수행하지 않는다. platform thread의 bounded resize wait 중에는 Flutter처럼 engine task runner만 polling하고, 일반 Win32 message나 Windows App SDK `DispatcherQueue` event loop를 중첩 실행하지 않는다. resize 완료에 DispatcherQueue work가 필요해지는 설계는 ownership 위반으로 처리한다. 바깥 message loop 종료 뒤 `AppWindow`/island를 먼저 닫고 `DispatcherQueueController.ShutdownQueue()`로 thread-affine rundown을 완료한다.

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
- ordinary animation frame이 resize generation으로 relabel되지 않는다.
- resize 중 새 `WM_SIZE`가 이전 transaction 완료 뒤 도착하면 새 generation으로 시작한다.
- timeout은 성공으로 간주하지 않는다. native resize 진행을 반환하되 최신 실제 client size를 다시 관측하고 redraw를 요청한다.
- 초기 timeout 기준은 Flutter source와 같은 100ms다. 수치를 늘리기 전에 framework/raster/surface 구간별 원인을 증명해야 한다.

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

## 5. ordered 구현 계획

각 단계는 앞 단계 PASS 뒤에만 시작한다. 실패하면 해당 단계에서 중단하고 원인과 exact resume point를 이 문서에 기록한다.

### F0 — Flutter source protocol lock

상태: **F0-A PASS — manual source audit / F0-V diagnosticOnly — current untracked worktree PASS**

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

1. `Doroti/eng/validate-flutter-windows-host-protocol.ps1`과 `Doroti/validation/contracts/flutter-windows-host-protocol.json`이 현재 untracked 상태로 존재한다.
2. 20분 timeout을 둔 현재 worktree 실행이 PASS했다.
3. Flutter commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`, source fingerprint `b79424311e4d2675283c3a676343a63fcc82a20b0711c14b0eebbde47ed37ecf`, files/anchors/mappings `10/12/4`를 확인했다.
4. scope는 source-only reference이며 Doroti build/runtime/visible acceptance가 아니다.

F0-V 남은 작업:

1. 기존 untracked validator/contract를 검토해 scoped implementation 변경으로 채택한다.
2. clean checkout에서 같은 command와 fingerprint를 재실행한다.
3. source가 바뀌면 protocol diff 없이 자동 승격하지 않는 fail-fast contract를 CI에 연결한다.

PASS 조건:

- pin/hash와 symbol anchor validator PASS
- 이 문서의 window/resize/surface 순서가 pinned source와 일치함
- Flutter runtime 계측 없이 source-only reference 경계가 명확함

### F1 — Windows App SDK/ANGLE bootstrap preflight

상태: notStarted

선행 조건: F0-V PASS

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

상태: notStarted

선행 조건: F1 PASS

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

상태: notStarted

선행 조건: F2 PASS

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

상태: notStarted

선행 조건: F3 PASS

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

상태: notStarted

선행 조건: F4 PASS

작업:

1. `ResizeStarted → FrameGenerated → SurfaceReady → Presented → Done` 상태를 구현한다.
2. platform thread가 metrics를 보낸 뒤 최대 100ms 동안 engine task runner만 polling한다.
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

상태: notStarted

선행 조건: F5 PASS

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

### F7 — input, focus, IME, clipboard, accessibility

상태: notStarted

선행 조건: F6 PASS

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

상태: notStarted

선행 조건: F7 PASS

작업:

1. `WM_DPICHANGED` suggested top-level rect를 적용하고 child metrics를 새 epoch로 게시한다.
2. current `HMONITOR`, display ID, refresh, DPI 변경을 display manager에 반영한다.
3. initial/restore placement만 current monitor `rcWork`에 맞춘다.
4. fullscreen은 `rcMonitor`와 standard style 전환으로 구현하고 원래 rect/DPI/monitor/work-area를 보존한다.
5. monitor disconnect/rearrange 뒤 restore rect를 새 `rcWork`에 맞춘다.
6. sleep/resume, RDP attach/detach, display change, device/context loss를 검증한다.
7. Windows App SDK runtime update/absence와 DispatcherQueue shutdown을 lifecycle matrix에 포함한다.
8. shutdown 중 pending resize/frame/surface/island callback을 모두 terminal 처리한다.

PASS 조건:

- mixed-DPI monitor 이동 중 logical/physical/display identity 불일치 0
- maximize/Snap/fullscreen/restore 뒤 stale surface/input 0
- monitor disconnect 뒤 offscreen window 0
- sleep/RDP/display change 뒤 valid frame 복구
- device/context loss 뒤 black/white stuck surface 없이 복구
- shutdown callback/UAF/crash 0

### F9 — Windows target/runner/package integration

상태: notStarted

선행 조건: F8 PASS

작업:

1. 기존 `Doroti.Target.Windows.WindowsAppSdk.win-x64` manifest/description에 Flutter-style adapter capability를 추가한다.
2. 기존 `DorotiDemoApp/windowsappsdk`에 `FlutterEmbedder`와 `ArmNLegacy` startup 선택을 추가하고 qualification 기본은 명시적 flag로만 선택한다.
3. clean CI restore/build/test/publish/launch를 추가한다.
4. Windows App SDK package/runtime와 ANGLE/EGL/GLES/Skia native artifact provenance/architecture check를 package에 포함한다.
5. runtime flag로 `FlutterEmbedder`, `ArmNLegacy`, MAUI rollback을 선택 가능하게 한다.
6. 이 단계에서는 기존 WindowsAppSdk host의 production default adapter와 app template 기본값을 바꾸지 않는다.

PASS 조건:

- clean checkout x64 restore/build/test/publish/launch PASS
- selected Windows App Runtime과 PATH 외 native DLL이 정확히 고정된 self-contained unpackaged launch PASS
- required native DLL 누락/잘못된 architecture를 startup에서 명확히 fail-fast
- non-Windows target 회귀 0
- `ArmNLegacy`와 MAUI rollback launch 절차 PASS

## 6. FG — Flutter-style Windows 제품 acceptance

상태: notVerified

선행 조건: F9 PASS

테스트 matrix:

- client size: 420×300, 640×360, 1000×600, monitor work-area 근접 크기
- DPI: 100%, 125%, 150%, 200%
- refresh: 사용 가능한 60, 120, 144, 165Hz
- drag: slow/fast, expand/shrink, immediate reverse, 네 edge와 네 corner
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
- 방향별 실제 mouse drag 10초 이상, 3회 연속 valid run
- 검은/흰 영역, 색 띠, stale edge, full-frame stretch, raster flicker 0
- pointer-border 역방향 jitter 또는 장시간 freeze 0
- stale/wrong-size generation present 0
- resize transaction timeout/failed/terminal 누락 0
- surface/context failure가 없거나 fault injection에서 검증된 recovery
- qualified output evidence와 사용자 실제 화면 확인 모두 PASS

한 축이라도 미실행이면 해당 축과 FG 전체는 `notVerified`다. build, contract counter, `eglSwapBuffers` 성공, `DwmFlush` 반환만으로 visible PASS를 선언하지 않는다.

### F10 — WindowsAppSdk 기본 adapter 전환

상태: notStarted

선행 조건: FG 전체 PASS와 사용자 제품 수용

작업:

1. `Doroti.Host.WindowsAppSdk`의 production default를 `FlutterEmbedder` adapter로 전환한다.
2. `DorotiDemoApp/windows`, app template, `doroti.ps1 build --platform windows`, target discovery, runner SDK의 기본 graph가 WindowsAppSdk/FlutterEmbedder를 선택하게 한다.
3. release/package 문서와 CI 기본 job을 새 adapter에 맞춘다.
4. `ArmNLegacy`와 MAUI Windows rollback launch를 최소 한 release 동안 보존한다.
5. 기존 host 삭제는 별도 cleanup 승인과 rollback 사용 현황 확인 뒤 수행한다.

PASS 조건:

- clean checkout 기본 Windows build/publish/launch가 WindowsAppSdk/FlutterEmbedder를 선택
- template에서 생성한 새 app의 Windows launch PASS
- explicit rollback host build/launch PASS
- non-Windows target 회귀 0
- release artifact provenance가 새 기본 host를 정확히 표시

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
- hardware/WARP/software 구분

resize event JSONL:

- `windowSizeObserved`
- `resizeStarted`
- `metricsDelivered`
- `sceneBuilt`
- `frameGenerated`
- `surfaceDestroyStarted`/`surfaceDestroyed`
- `surfaceCreated`/`surfaceReady`
- `swapStarted`/`swapCompleted`
- `resizeDone`
- `timedOut`/`superseded`/`failed`/`suspended`
- `dwmOrderingPoint`
- `contextLost`/`contextRecovered`

모든 event는 `viewId`, `resizeGeneration`, target extent, actual child extent, thread ID, monotonic timestamp를 포함한다. summary는 count뿐 아니라 p50/p95/p99/max, worst generation, timeout/failed/terminal imbalance를 기록한다.

app counter, observer/camera, 사용자의 visible 판정은 서로 다른 원본 evidence로 보존한다. 한 종류의 성공을 다른 종류의 PASS로 승격하지 않는다.

## 8. 실패 시 중단·rollback 규칙

- F1에서 Windows App SDK runtime/AppWindow/DispatcherQueue 또는 ANGLE/Skia ABI·배포가 성립하지 않으면 D3D12/Arm N으로 조용히 대체하지 않는다. 원인과 대안 결정을 기록하고 중단한다.
- F2에서 standard window 기능이 깨지면 custom non-client를 추가하지 않고 top-level/child ownership을 수정한다.
- F4에서 surface recreation이 느리면 monitor-sized capacity, `SetSourceSize`, dual-front를 새 adapter에 넣지 않는다. context/resource lifetime과 surface creation 비용을 먼저 분리한다.
- F5 timeout이 반복되면 100ms를 늘리지 않고 metrics→layout→raster→surface→swap의 worst generation을 고친다.
- F5에서 visible artifact가 남으면 timer/debounce/background/edge별 좌표 보정을 추가하지 않고 handshake와 actual child/surface extent를 다시 검증한다.
- F6 cadence가 부족하면 resize correctness와 cadence를 분리해 원인을 찾되 wrong-size frame을 허용하지 않는다.
- F7 IME/UIA 때문에 full-client XAML/ContentIsland가 필요해 보이면 F7을 FAIL로 기록하고 bounded `DesktopChildSiteBridge` 최소 spike로 원인을 분리한다.
- F8 monitor 이동 시 fixed primary work-area로 clamp하지 않는다. 현재 child가 속한 monitor/DPI/display를 다시 관측한다.
- F10과 별도 cleanup 승인 전에는 Arm N/MAUI rollback을 삭제하지 않는다.
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

새 Flutter-style path가 FG 전체 PASS를 얻기 전에는 기존 영역을 삭제하거나 과거 evidence를 새 경로의 성공으로 재해석하지 않는다.

## 10. 정확한 다음 작업

1. F0-V의 기존 untracked source validator/contract를 검토·채택하고 clean checkout에서 재실행해 정식 PASS로 승격한다.
2. F1 기존 `Doroti.Host.WindowsAppSdk`에 분리된 Flutter-style adapter와 exact 2.4/AppWindow/DispatcherQueue/ANGLE bootstrap을 구현한다.
3. F2 WindowsAppSdk raw standard top-level + child HWND와 first-frame show gate를 구현한다.
4. F3 physical metrics/display snapshot을 engine에 연결한다.
5. F4 child HWND exact-size EGL window surface lifecycle을 raster thread에 구현한다.
6. F5 100ms bounded resize handshake와 exactly-once terminal ledger를 구현한다.
7. F2~F5 최소 live gate에서 실제 네 edge/네 corner visible 동작을 확인한다.
8. 그 gate가 PASS한 뒤에만 F6 scheduler, F7 input/text/UIA, F8 lifecycle, F9 integration으로 진행한다.
9. FG 전체 PASS와 사용자 제품 수용 뒤에만 F10 WindowsAppSdk 기본 adapter 전환을 수행한다.

현재 상태를 다음처럼 오해하면 안 된다.

- Flutter-style host가 이미 구현됐다: **아님**
- 기존 Arm N을 조금 수정하는 계획이다: **아님** — 같은 WindowsAppSdk host package 안의 별도 child/EGL adapter다.
- monitor work-area를 render envelope/capacity로 계속 사용한다: **아님**
- work-area를 전혀 사용하지 않는다: **아님** — initial/restore/popup placement에만 사용한다.
- Windows App SDK 2.4가 제품 기반이다: **맞음** — exact pin, AppWindow, DispatcherQueue, self-contained runtime을 사용한다.
- Windows App SDK가 primary render size/present를 소유한다: **아님** — Flutter식 child HWND와 raster thread가 소유한다.
- full-client ContentIsland가 필수다: **아님** — bounded auxiliary content에만 허용한다.
- pinned Flutter runtime을 같이 빌드·계측한다: **아님** — source-only protocol reference다.
- 기존 Arm N/MAUI rollback을 지금 삭제한다: **아님**
- 이번 문서 개편이 runtime resize 문제 해결을 증명한다: **아님**

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

이 문서의 단계와 상태가 새 Windows 제품 경로의 active 기준이다. 구현 중 새 evidence가 생기면 해당 단계의 `PASS/FAIL/notVerified`와 exact resume point를 함께 갱신한다.

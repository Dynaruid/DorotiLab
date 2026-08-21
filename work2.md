# Windows / Web resize pipeline 전면 재설계 계획

작성일: 2026-08-21

현재 상태: 계획 작성 완료 / 구현 `notStarted` / 실기기·브라우저 검증 `notVerified`

## 1. 해결할 현상

1. Windows에서 창 테두리를 드래그하면 창 크기가 포인터를 즉시 따라가지 못하고, 렌더링 속도에 맞춰 계단식으로 뒤쫓는다.
2. Web에서 resize 중 이전 크기와 현재 크기의 프레임이 번갈아 표시되어 캔버스가 출렁인다.

이번 작업은 현 구조에 debounce나 대기 시간을 더 붙이는 보수가 아니다. 크기 소유권, 프레임 생성, GPU surface 변경, present 완료를 하나의 세대 기반 파이프라인으로 다시 나눈다.

`work.md`는 이 작업의 입력이나 진행 기준으로 사용하지 않는다. 구현 중에도 이 문서만 순서와 완료 조건으로 사용한다.

## 2. 현재 코드에서 확인한 근본 문제

### Windows

- `WindowsResizeContinuityGuard`는 `WM_SIZE`/`WM_DPICHANGED` 처리 안에서 다음 작업을 한 번에 수행한다.
  - `DefSubclassProc`와 WinUI layout 반영
  - framework metrics 전달과 frame build
  - `SKGLView.InvalidateSurface()`
  - Skia raster와 ANGLE `eglSwapBuffers`
  - `DwmFlush`
- 이 방식은 크기와 프레임을 정확히 맞추지만, UI/window thread가 build+raster+present를 모두 기다린다. 따라서 창 자체가 렌더링 완료 속도로만 움직인다.
- 현재 evidence에서 exact-size mismatch나 generation 역행은 없지만, final swap 자체보다 `InvalidateSurface` 전체와 `DwmFlush`가 더 큰 비용이다. swap interval 0/1/default 변경도 증상을 결정적으로 바꾸지 못했다. 즉, 문제의 중심은 swap interval이 아니라 UI thread에 직렬화된 transaction이다.
- `MauiHostAdapter.BeginPaint()`가 paint surface 크기를 다시 framework metrics로 올린다. native target과 실제 surface가 잠시 다를 때 paint가 metrics의 역방향 source가 되어 resize feedback을 만들 수 있다.
- `DorotiWindowsSwapChainPanel`은 stock `SKSwapChainPanel`의 마지막 swap 직전만 관찰한다. private leading resize swap, EGL surface 생성/교체, context thread ownership은 통제하지 못한다. 이 경계로는 Flutter의 exact-size surface 계약을 완성할 수 없다.

### Web

- 현재 크기 신호가 최소 세 경로에서 온다.
  - Doroti root `ResizeObserver`
  - `window.resize` 후 별도 rAF 측정
  - SkiaSharp `SizeWatcher`가 canvas `clientWidth/clientHeight`를 읽어 호출하는 경로
- `resizeHost()`는 root inline style을 쓰면서 즉시 metrics/epoch도 갱신하고, 이후 `ResizeObserver`가 같은 변경을 다시 보고한다.
- `installCanvasResizeContinuity()`는 SkiaSharp의 private `SKHtmlCanvas.requestAnimationFrame`과 `renderFrameCallback`을 monkey patch한다. backing-store 쓰기를 rAF 안으로 옮기지만, Doroti target generation과 SkiaSharp pending width/height가 같은 transaction으로 묶이지 않는다.
- SkiaSharp 원본은 rAF가 이미 pending인지 확인하기 전에 `canvas.width/height`를 쓴다. 이 쓰기는 WebGL backing store를 지운다. 현재 patch는 이 동작 시점만 이동했을 뿐, resize authority와 frame queue를 Doroti가 소유하지 않는다.
- target을 관찰한 시점에 `surfaceGeneration`을 올리고 실제 canvas backing store 교체 시점과 분리한다. 따라서 target, framework scene, backing store, paint가 서로 다른 세대를 가리킬 수 있다.
- `BrowserHostAdapter.ScheduleFrame()`은 callback id별 dictionary를 사용한다. 브라우저 rAF가 하나만 pending이어야 한다는 정책과, framework callback 적재 정책이 하나의 bounded queue로 표현돼 있지 않다.

## 3. 로컬 Flutter 소스에서 가져올 계약

기준 checkout: `reference/flutter-master`, commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`.

| Flutter 코드 | 가져올 원칙 | Doroti에 그대로 복사하지 않을 부분 |
|---|---|---|
| `engine/src/flutter/shell/platform/windows/flutter_windows_view.cc` | resize target을 먼저 기록하고 metrics를 보낸다. 생성된 frame의 pixel size가 target과 정확히 같을 때만 surface를 바꾼다. swap 뒤 `OnFramePresented`가 resize를 끝낸다. | Flutter의 100 ms platform-thread wait를 현재 단일-thread Doroti 구조에 그대로 넣지 않는다. |
| `engine/src/flutter/shell/platform/windows/compositor_opengl.cc` | `OnFrameGenerated(width,height)` 승인 → surface current → blit → `SwapBuffers` → `OnFramePresented` 순서를 지킨다. | stock `SKSwapChainPanel`의 opaque/private lifecycle 위에서 이 순서를 흉내 내지 않는다. |
| `engine/src/flutter/shell/platform/windows/egl/surface.cc` | EGL surface 생성/파괴/swap은 surface owner가 맡는다. | UI thread와 raster thread가 같은 EGL context를 번갈아 current로 만들지 않는다. |
| `engine/src/flutter/lib/web_ui/lib/src/engine/view_embedder/dimensions_provider/custom_element_dimensions_provider.dart` | host element가 browser resize의 유일한 크기 관찰 대상이고 DPR 변경은 별도 signal로 다룬다. | canvas와 window를 추가 authority로 관찰하지 않는다. |
| `engine/src/flutter/lib/web_ui/lib/src/engine/frame_service.dart` | pending rAF는 하나뿐이다. 이미 예약됐으면 중복 예약하지 않는다. | timer/debounce로 resize 순서를 가리지 않는다. |
| `engine/src/flutter/lib/web_ui/lib/src/engine/renderer.dart`와 `compositing/rasterizer.dart` | render queue는 `current + latest next` 두 칸뿐이다. current가 끝나기 전 새 요청이 오면 next를 최신 요청으로 교체한다. frame 시작 때 physical size를 한 번 캡처하고 그 크기로 surface와 raster를 끝까지 처리한다. | 오래된 요청을 FIFO로 모두 재생하지 않는다. |
| `engine/src/flutter/lib/web_ui/lib/src/engine/compositing/render_canvas.dart` | display canvas의 pixel size는 실제로 표시할 bitmap과 정확히 같을 때만 바꾼다. 같은 크기면 backing store를 다시 만들지 않는다. | observer callback에서 선제적으로 `canvas.width/height`를 바꾸지 않는다. |

참고할 SkiaSharp package source identity는 `4.151.1`, commit `279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764`이다. Web의 `SKHtmlCanvas.js`와 `SizeWatcher.js`는 동작을 이해하기 위한 기준으로만 사용하고, private runtime monkey patch는 최종 구조에서 제거한다.

## 4. 새 공통 계약

### 4.1 서로 다른 세대의 의미를 분리한다

- `ResizeTargetGeneration`: OS/DOM이 요청한 논리 크기, DPR, 물리 크기의 단조 증가 세대.
- `FrameworkFrameGeneration`: 해당 target metrics로 build된 immutable scene의 세대.
- `SurfaceGeneration`: EGL surface 또는 Web canvas backing store가 실제로 교체된 세대.
- `PresentSequence`: GPU submit/present가 성공한 순번.

target 관찰만으로 `SurfaceGeneration`을 증가시키지 않는다. surface를 실제 생성·resize한 owner만 증가시킨다.

### 4.2 frame descriptor

모든 resize 관련 frame은 다음 immutable descriptor를 가진다.

```text
FrameDescriptor {
  viewId
  resizeTargetGeneration
  metricsGeneration
  logicalWidth, logicalHeight
  physicalWidth, physicalHeight
  devicePixelRatio
  sceneSequence
}
```

surface presenter는 descriptor의 물리 크기가 현재 승인된 target과 정확히 일치할 때만 새 surface/backing store로 commit할 수 있다.

### 4.3 상태 기계

```text
Idle
  -> TargetPending
  -> FrameBuilding
  -> ExactFrameReady
  -> SurfaceCommit
  -> Presenting
  -> Presented

어느 단계에서든 더 새 target 도착
  -> 현재 작업은 완료 가능한 지점까지만 진행하거나 superseded 처리
  -> latest target 하나만 보존
```

### 4.4 반드시 지킬 불변식

1. 플랫폼마다 크기 authority는 하나다.
2. target generation은 역행하지 않는다.
3. paint/surface 크기는 framework metrics를 역방향으로 갱신하지 않는다. 일치 여부를 검증하는 데만 쓴다.
4. 이미 새 target B를 승인한 뒤 오래된 target A의 frame을 present하지 않는다.
5. surface/backing store 크기는 exact frame을 같은 transaction에서 paint할 수 있을 때만 바꾼다.
6. render queue는 `current + latest next`를 넘지 않는다. 중간 frame은 `superseded` terminal을 받는다.
7. `presented`는 draw 함수 반환이 아니라 플랫폼 present 경계를 통과했을 때만 기록한다.
8. minimize/0-size는 surface 생성 대상이 아니다. 마지막 정상 frame을 보존하고 복원 target에서 새 generation을 시작한다.
9. context loss/recreation 뒤에는 마지막 정상 scene 또는 최신 exact scene을 새 surface에 다시 raster한다.
10. 계측 실패는 구현 중단 조건이 아니다. 제품 동작 검증을 다른 관찰 수단으로 바꾸고 다음 실험을 계속한다.

## 5. 구현 단계

### RZ-1. 공통 resize 모델을 먼저 고정

대상:

- `Doroti/src/Doroti.Ui/ResizeLifecycle.cs`
- `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- 신규 `Doroti/validation/resize-contract/`

작업:

- `DorotiResizeEpoch`을 target descriptor로 명확히 하고 surface generation과 분리한다.
- `SkiaSceneRenderer.SceneFrame`에 frame descriptor를 저장한다.
- pending scene을 FIFO가 아닌 `current + latest next` 정책으로 명시한다.
- `Paint` 호출자가 원하는 target descriptor를 전달하고, renderer가 exact/superseded/replay 여부를 반환하도록 contract를 바꾼다.
- resize 중 오래된 scene은 surface commit 전에 거절한다. 일반 animation 중 최신 scene 교체 정책은 유지한다.
- 마지막 presented scene 보존과 context-replay는 유지하되, resize target과 다른 scene을 exact frame으로 보고하지 않는다.
- deterministic validation app에서 다음 순열을 모두 실행한다.
  - A target → A frame → A present
  - A target → A frame → B target → A reject → B present
  - A target → B target → C target → C만 build/present
  - 동일 size 중복 signal
  - DPR만 변경
  - 0-size/minimize → restore
  - surface/context recreation

완료 조건:

- 모든 생성 frame이 정확히 하나의 `presented`, `superseded`, `dropped`, `failed` terminal을 가진다.
- 오래된 generation present와 queue depth 2 초과가 validation에서 0이다.

### WIN-1. UI thread의 synchronous render transaction 제거

대상:

- `WindowsResizeContinuityGuard.cs`
- `MauiHostAdapter.cs`
- `MauiSkiaSurface.cs`

작업:

- `WM_SIZE`/`WM_DPICHANGED` callback은 최신 target 기록, metrics dispatch 예약, raster worker wake-up만 수행하게 줄인다.
- callback 안의 `UpdateLayout`, synchronous `DispatchPendingFrame`, `InvalidateSurface`, `DwmFlush`를 제거한다.
- `MauiHostAdapter.BeginPaint()`가 paint size로 `_logicalSize`, `_density`, metrics generation을 다시 쓰는 경로를 제거한다. paint size는 target/surface consistency 검사로만 사용한다.
- `CompositionTarget.Rendering`은 framework begin-frame clock만 제공한다. EGL swap backpressure나 resize 완료를 UI compositor callback이 소유하지 않게 한다.
- 현재 ETW/in-process trace는 관찰 장치로 남기되 pipeline 제어에 사용하지 않는다.

완료 조건:

- `WM_SIZE` handler가 framework build, raster, EGL, `DwmFlush` 어느 것도 직접 호출하지 않는다.
- resize 중 UI/window message pump가 계속 pointer와 paint 메시지를 처리한다.

### WIN-2. Windows surface를 Doroti가 끝까지 소유

stock `SKGLView`/`SKSwapChainPanel` subclassing을 최종 Windows renderer에서 제거한다. Flutter Windows와 같은 thread/surface ownership을 얻기 위해 다음 순서로 구현한다.

#### 1차 경로: child HWND + Doroti-owned ANGLE/EGL

- `IMauiSkiaSurface`를 구현하는 `DorotiWindowsRenderSurface`를 만든다.
- WinUI top-level HWND 아래에 렌더 전용 child HWND를 만들고 XAML layout 결과는 `SetWindowPos`로 child bounds에만 반영한다.
- 전용 raster thread가 EGL display/config/context/window surface와 `GRDirectContext`를 생성·사용·파괴한다.
- UI thread는 EGL object를 current로 만들거나 swap하지 않는다.
- target mailbox는 latest 하나만 보존한다. raster thread는 exact frame descriptor가 준비되면 그 target 크기로 surface를 생성/교체하고 raster한 뒤 swap한다.
- resize 진행 중 새 target이 오면 아직 surface commit 전인 오래된 frame은 버린다. 이미 raster 중인 frame은 swap 직전에 generation을 다시 검사한다.
- `eglSwapBuffers` 성공 후 resize-present ACK를 기록한다. resize frame에 한해서 `DwmFlush`가 필요하면 raster thread에서 수행하고 UI thread와 분리한다.
- pointer, wheel, key, focus, cursor, IME, semantics bridge는 기존 `IMauiSkiaSurface` 계약을 통해 새 HWND host에 다시 연결한다.

#### 2차 경로: child HWND 통합이 제품 요구를 만족하지 못할 때

- airspace, accessibility 또는 XAML composition 제약이 실제 제품 동작을 막으면 stock panel로 돌아가지 않는다.
- Doroti-owned `SwapChainPanel + DXGI swap chain` presenter를 구현하고 Skia의 Direct3D backend 가능성을 검증한다.
- 이 경로도 surface/context/present 전체를 Doroti가 소유하고 raster thread 한 곳에서만 다룬다는 불변식은 같다.
- 과거 owned-ANGLE spike의 `eglCreateWindowSurface` 실패는 실패 evidence로 보존하되, 새 HWND native-window 경로와 동일 문제로 간주하지 않는다.

완료 조건:

- `DorotiWindowsSkiaViewHandler`, stock panel private swap 관찰, `WindowsEglInterop`의 borrowed-current-context 방식이 제품 경로에서 사라진다.
- target/frame/surface/present generation을 한 presenter에서 일관되게 판단한다.
- context 생성부터 파괴까지 EGL/Skia GPU object thread ownership 위반이 없다.

### WIN-3. Flutter식 exact-size 승인과 latest-only pacing 적용

작업:

- OS target 수신과 framework build를 비동기로 연결한다.
- framework scene이 아직 준비되지 않았을 때 창 크기 변경 자체는 막지 않는다. 이전 surface는 compositor가 일시적으로 보존할 수 있지만, 오래된 frame을 새 frame처럼 다시 submit하지 않는다.
- exact scene이 준비되는 즉시 `surface resize → raster → swap → ACK`를 한 raster-thread transaction으로 수행한다.
- interactive resize 중 중간 target은 latest mailbox에서 합친다. `WM_EXITSIZEMOVE`에서는 마지막 target을 반드시 drain한다.
- Flutter의 100 ms timeout은 recovery 상한 참고값으로만 사용한다. 매 `WM_SIZE`마다 UI thread를 100 ms block하는 정책은 도입하지 않는다.
- swap interval 0/1/default는 진단용 환경 변수로만 유지하고 제품 기본 동작을 그 값에 의존시키지 않는다.

완료 조건:

- 창 테두리 이동이 render transaction 때문에 계단식으로 정지하지 않는다.
- stale present, target/surface size mismatch, generation regression이 0이다.
- drag 종료 후 최종 target frame이 누락되지 않는다.

### WEB-1. Web 크기 authority를 하나로 통합

대상:

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- `BrowserHostContracts.cs`
- `DorotiSurface.razor`
- `wwwroot/doroti.web.css`

작업:

- root element `ResizeObserver`를 유일한 논리 크기 authority로 둔다.
- `window.resize` 크기 측정 경로를 제거한다. DPR 변경은 Flutter처럼 별도 DPR watcher를 두고, 같은 root sampling 함수로 합류시킨다.
- observer callback은 바로 metrics나 canvas를 바꾸지 않는다. latest candidate만 저장하고 pending rAF 하나에서 root rect와 DPR을 한 번 읽는다.
- `resizeHost()`는 CSS constraint만 요청하고 직접 epoch/metrics/surface generation을 갱신하지 않는다. observer가 확인한 실제 layout 결과만 publish한다.
- 동일한 logical size/DPR/rounded physical size는 generation을 올리지 않는다.
- target observation과 실제 backing-store commit을 분리한다.

완료 조건:

- size publisher가 root observer + DPR resample coordinator 한 곳뿐이다.
- canvas 관찰, `window.resize` 재측정, inline-style 즉시 metrics publish가 없다.

### WEB-2. SkiaSharp monkey patch 대신 Doroti-owned canvas presenter 구현

작업:

- `SKGLView`가 묶어 둔 `SizeWatcher`와 `SKHtmlCanvas` lifecycle을 제품 경로에서 제거한다.
- SkiaSharp 4.151.1의 public WebAssembly/GRContext API를 이용하는 최소 `DorotiWebGlSurface` Razor/JS host를 만든다.
- 필요한 upstream 동작을 옮길 경우 원본 commit과 라이선스 표기를 코드에 남기고, Doroti가 다음을 직접 소유한다.
  - WebGL context 생성/복구/파괴
  - pending rAF 하나
  - canvas backing-store 크기
  - managed paint callback
  - present/submitted terminal 기록
- JS API를 `requestPresent(hostId, frameDescriptor)` 형태로 만들고 queue는 `current + latest next` 두 칸으로 제한한다.
- rAF callback 시작 시 descriptor가 latest target인지 다시 확인한다. stale이면 canvas를 건드리지 않고 `superseded` 처리한다.
- exact frame일 때만 같은 callback 안에서 `canvas.width/height` 변경 → WebGL current/context 확인 → managed Skia paint를 연속 수행한다.
- canvas CSS 크기는 root를 따르게 하고, pixel backing size는 presenter만 쓴다.
- 실제 browser scan-out ACK는 얻을 수 없으므로 terminal 이름을 `submitted`로 유지한다. rAF callback 반환을 `presented`라고 부르지 않는다.

완료 조건:

- `installCanvasResizeContinuity()`/`uninstallCanvasResizeContinuity()`와 `SKHtmlCanvas` method 교체가 삭제된다.
- `canvas.width/height` writer가 Doroti presenter 한 곳뿐이다.
- target B가 관찰된 뒤 A 크기로 backing store가 돌아가는 일이 없다.

### WEB-3. framework frame queue도 latest-only로 통합

작업:

- `BrowserHostAdapter.ScheduleFrame()`의 callback dictionary를 bounded current/latest 구조로 바꾼다.
- browser rAF 예약은 항상 최대 하나다.
- metrics target generation을 framework frame descriptor까지 전달한다.
- 현재 raster가 끝나기 전에 여러 scene이 오면 current는 끝내고 next는 최신 scene으로 교체한다. 교체된 scene/callback은 terminal을 남기고 완료시켜 awaiter가 매달리지 않게 한다.
- context loss 중 새 frame을 쌓지 않는다. 최신 target과 최신 scene 하나만 보존해 restore 뒤 exact-size replay한다.

완료 조건:

- rAF pending count ≤ 1, render queue depth ≤ 2다.
- 모든 resize frame의 target, backing store, surface, raster size가 일치한다.

### RZ-2. 기존 실험 코드 정리와 계약 문서화

새 backend가 live gate를 통과한 뒤에만 정리한다.

- synchronous resize transaction과 borrowed EGL context를 위한 코드를 제거한다.
- 더 이상 사용하지 않는 handler registration, target manifest native view type, 환경 변수, validation anchor를 새 backend에 맞춘다.
- 기존 resize evidence는 baseline으로 보존한다. 새 결과와 섞어 최신 PASS처럼 재사용하지 않는다.
- `validate-resize-continuity.ps1`은 문자열 존재 검사 중심에서 state-machine validation과 source fingerprint 검사 중심으로 바꾼다.
- Windows/Web 이외 플랫폼은 공통 contract compile 및 기존 smoke를 돌리되 이번 작업의 live PASS로 승격하지 않는다.

## 6. 실험 및 검증 계획

모든 build/test 명령은 저장소 지침에 따라 20분 timeout으로 실행한다.

### 6.1 공통 자동 검증

- resize state model permutation validation
- queue depth와 terminal accounting validation
- stale generation reject validation
- context loss/recreation validation
- Release build:
  - `Doroti.Host.Maui` Windows target
  - `Doroti.Host.Web`
  - `DorotiDemoApp/windows`
  - `DorotiDemoApp/web`
- TypeScript typecheck와 Web publish
- `git diff --check`

자동 검증은 native window와 browser의 가시적 동작을 대신하지 않는다.

### 6.2 Windows live 시나리오

각 시나리오는 작은 창과 최대화에 가까운 창에서 수행한다.

1. 우하단 corner를 10초 동안 일정 속도로 확대/축소한다.
2. 좌우/상하 edge를 빠르게 왕복한다.
3. maximize ↔ restore를 20회 반복한다.
4. 100%, 125%, 150% DPI와 monitor 간 이동을 확인한다.
5. resize 중 scroll/hover/click을 함께 입력한다.
6. minimize 후 restore, GPU context recreation, 창 종료를 확인한다.

필수 판정:

- 화면 녹화에서 창 edge가 장시간 멈췄다가 따라오는 staircase가 없다.
- `WM_SIZE` callback은 enqueue/dispatch만 하고 장시간 block하지 않는다.
- stale present = 0, size mismatch = 0, generation regression = 0.
- 최종 resize target은 100 ms 안에 drain되거나 명시적 `failed` 후 즉시 recovery frame을 만든다.
- 입력, IME, semantics, focus, close가 새 surface host에서도 동작한다.

### 6.3 Web live 시나리오

Chrome에서 다음을 자동 viewport resize와 실제 drag 둘 다로 수행한다.

1. viewport width/height를 서로 다른 패턴으로 빠르게 왕복한다.
2. A→B→C 크기를 한 rAF보다 빠르게 연속 요청한다.
3. DPR/브라우저 zoom을 바꾼다.
4. DevTools throttling 상태에서 resize한다.
5. `WEBGL_lose_context`로 context loss/restore를 유도한다.
6. resize 중 scroll, pointer, keyboard input을 함께 수행한다.

필수 판정:

- backing-store generation이 단조 증가하고 B 승인 후 A 크기로 돌아가지 않는다.
- canvas backing size와 raster size가 매 submit에서 일치한다.
- rAF pending count ≤ 1, render queue depth ≤ 2다.
- resize 중 blank flash와 이전/현재 크기 교대가 화면 녹화에서 보이지 않는다.
- 최종 root rect, framework metrics, canvas CSS size, physical backing size가 일치한다.

## 7. 계측 실패 시 진행 규칙

계측은 원인 파악과 회귀 분석을 돕는 수단이지 구현 허가 gate가 아니다.

### Windows fallback 순서

1. ETW/WPA correlation
2. in-process `DorotiResizeTrace`
3. `QueryPerformanceCounter` 기반 bounded log
4. `GetWindowRect`/cursor sampling + 화면 녹화
5. 사용자 직접 관찰

### Web fallback 순서

1. CDP/Performance trace
2. in-page bounded resize trace export
3. console generation log + canvas/root snapshot
4. 화면 녹화
5. 사용자 직접 관찰

한 계측 방식이 실패하면 해당 항목만 `notVerified`로 남기고 다음 방식과 구현 단계로 진행한다. 단, compile 실패, crash, stale present, 실제 flicker처럼 제품 동작이 실패한 경우에는 우회해 PASS 처리하지 않고 코드를 고친다.

backend spike가 실패한 경우도 전체 작업을 중단하지 않는다. 실패 원인을 짧게 기록하고 WIN-2에 정한 다음 surface ownership 경로로 이동한다.

## 8. 완료 판정과 보고 형식

최종 완료는 다음 네 묶음이 모두 충족될 때만 선언한다.

- `COMMON-CONTRACT`: 자동 state/queue 계약 `PASS`
- `WINDOWS-LIVE`: 실제 interactive resize와 입력/IME/종료 `PASS`
- `WEB-LIVE`: 실제 Chrome resize/context restore `PASS`
- `CROSS-SMOKE`: Android/iOS/macOS/Linux 관련 기존 경로 build/smoke 결과를 `PASS`, `failed`, `notVerified` 중 정확히 기록

각 gate 결과에는 다음을 남긴다.

```text
status: PASS | failed | notVerified
sourceFingerprint:
commandOrProcedure:
observableResult:
evidence:
limitations:
```

build 성공, process 생존, rAF 실행만으로 resize 현상 해결을 선언하지 않는다. Windows 창과 Web canvas의 가시적 resize가 실제로 안정된 것을 최종 증거로 삼는다.

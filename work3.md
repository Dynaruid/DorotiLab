# Doroti WinRT-first Flutter-style Windows backend 작업 계획

## 0. 문서 목적과 범위

- 계획 기준일: 2026-08-25
- 대상: `Doroti.Host.WindowsAppSdk`와 `Doroti.Target.Windows.WindowsAppSdk.win-x64`의 새 Windows App SDK backend. 기존 `Doroti.Target.Windows.Maui.win-x64`는 독립된 정식 Windows backend로 유지한다.
- 새 backend 작업명: `WinRtComposition`
- 기술 방향: **WinRT-first / XAML-free / Flutter-style thread·surface lifecycle**
- 현재 Windows App SDK 기준: repository에 고정된 exact `2.4.0`, self-contained unpackaged
- test timeout: 각 build/test command 최대 20분
- 문서 상태: **새 작업계획 작성과 이전 Windows App SDK backend 정리 완료**. 현재 두 backend의 Release build만 확인했으며 새 WinRT backend 구현, 제품 runtime, visible PASS는 아직 주장하지 않는다.

이 계획은 이전 Windows backend 계획을 대체한다. 이전 backend의 제품 source, 전용 validator, contract, trace script, 작업 문서와 전용 history summary는 사용자 결정에 따라 제거했으며, 그 결과를 새 backend의 evidence나 baseline으로 이월하지 않는다.

새 방향은 다음을 뜻한다.

- Doroti framework, scheduler, raster, input, semantics 계약은 Flutter Windows embedding 구조를 참고한다.
- Windows shell, content hosting, input, focus, coordinate conversion, accessibility의 primary API는 Windows App SDK/WinRT를 사용한다.
- render content는 WinUI XAML control이 아니라 `ContentIsland`에 연결한 composition surface다.
- 현재 package/API에서 WinRT만으로 top-level content attachment가 완결되지 않으므로 HWND creation/message pump와 DirectX COM interop은 작은 shim으로 격리한다.
- Windows는 `WindowsAppSdk`와 `Maui` 두 backend를 영구 지원한다. 현재 공개 기본값은 실행 가능한 `Maui`다. Windows App SDK target은 내부 구현 capability로 `WinRtComposition`만 광고하며, 구현 전에는 의도적으로 fail-fast 한다.

## 1. 검토 결론

### 1.1 현재 repository 상태

현재 repository 상태는 다음과 같다.

1. 이전 Windows App SDK 제품 backend와 그 전용 validation 경로는 제거됐다.
2. target manifest, transitive target, Demo runner, template, CLI는 `WinRtComposition`만 Windows App SDK adapter로 선언한다.
3. `DorotiWindowsAppSdkRunner`는 새 backend가 구현될 때까지 명시적인 `NotSupportedException`으로 종료한다.
4. generic Windows App SDK 2.4 API preflight와 독립 native resize capture 도구는 새 spike에 재사용할 수 있으므로 유지한다.
5. MAUI는 rollback adapter가 아니라 독립된 정식 Windows backend이며 CLI 기본값이다. Windows App SDK 개발 경로는 `-WindowsBackend WindowsAppSdk`로 명시한다.

이전 backend의 자동·수동·visible 결과는 새 backend의 PASS나 active baseline으로 사용하지 않는다. W0부터 새 provenance와 evidence ledger를 만든다.

### 1.2 WinRT 방향의 타당성

**결론: WinRT-first backend는 타당하지만, pure WinRT backend라고 부르면 안 된다.**

이유는 다음과 같다.

- `AppWindow`는 WinRT API이지만 내부 모델은 top-level HWND다.
- `AppWindow` 자체에는 UI framework content를 attach하는 일반 API가 없다.
- 현재 exact package projection을 확인하면 `ContentIsland.Create`, `CreateForSystemVisual`, `DesktopAttachedSiteBridge`, `ContentSiteView`, island input/focus/automation API는 존재한다.
- 같은 projection에는 experimental 문서에 보이는 `ContentAppWindowBridge`가 없으므로 이를 전제로 구현하면 안 된다.
- 따라서 최소 top-level HWND를 만든 뒤 `WindowId → AppWindow`를 연결하고, `DesktopAttachedSiteBridge`로 `ContentIsland`를 attach하는 것이 현재 checkout에서 가능한 WinRT-first 경계다.
- Direct3D/Skia pixels를 composition tree에 넣는 과정에는 `ICompositorInterop`/DXGI/Direct3D COM interop가 필요하다.

WinRT 전환으로 기대할 수 있는 실제 이점은 다음과 같다.

- render용 child HWND 제거
- `ContentIsland` 하나를 layout/input/focus/accessibility boundary로 통일
- raw child WndProc input translation과 WinRT island state의 중복 ownership 제거
- composition visual tree에 surface, clip, background, diagnostic overlay를 명시적으로 구성
- `ContentSiteView.ClientSize`, `ActualSize`, `RasterizationScale`, coordinate converter를 같은 metrics generation으로 묶기

반대로 WinRT 전환만으로 보장되지 않는 것은 다음과 같다.

- standard native border와 app content가 같은 scan-out frame에서 atomic하게 바뀌는 것
- `AppWindow.Changed` 또는 composition commit ACK가 실제 화면 표시 시점인 것
- Korean IME와 candidate/caret가 CoreText만으로 desktop island에서 완전하게 동작하는 것
- pointer가 island 밖으로 나간 뒤의 drag capture가 별도 interop 없이 유지되는 것
- Narrator/Accessibility Insights가 기존 child-root UIA 구현과 동일하게 동작하는 것

따라서 independent WinRT island prototype의 physical/visible gate를 product integration보다 먼저 둔다.

### 1.3 채택·재작성·제외 결정

| 항목 | 결정 | 이유 |
|---|---|---|
| `Doroti.Host.WindowsAppSdk` target/package boundary | 유지 | runner, template, self-contained publish 계약을 새 backend에 재사용한다. |
| C#/.NET host | primary 유지 | Doroti framework와 기존 host capability를 그대로 연결할 수 있다. |
| 작은 C++/WinRT 또는 native interop DLL | 조건부 허용 | projected C#에서 swap-chain composition interop를 안전하게 표현할 수 없을 때만 사용한다. |
| standard top-level window/AppWindow | primary | Snap, system menu, taskbar, maximize, native accessibility를 먼저 보존한다. |
| child render HWND | 새 backend에서 제외 | `ContentIsland`가 render/input/accessibility view boundary를 맡는다. |
| WinUI XAML, `SwapChainPanel`, MAUI | 제외 | Doroti scene 위에 두 번째 UI framework/layout owner를 만들지 않는다. |
| ANGLE/EGL | 새 primary에서 제외 | window-surface recreate 대신 composition swap chain/surface lifecycle을 사용한다. |
| D3D12/Skia raster | 유지 | 현재 renderer/backend와 resource ownership을 재사용한다. |
| raw WndProc client input | 기본 제외 | island input이 primary다. 확인된 API 공백만 interop shim으로 보완한다. |
| `Owned Geometry Envelope v2` | fallback decision으로 보류 | WinRT standard-shell prototype이 같은 visible FAIL을 보일 때만 별도 승인한다. |

## 2. 목표 architecture

### 2.1 전체 구조

```text
Doroti runner / target package
└─ WinRtComposition backend
   ├─ platform STA
   │  ├─ minimal TopLevelHwndShim
   │  ├─ WindowId + AppWindow
   │  ├─ Microsoft.UI.Dispatching.DispatcherQueue
   │  ├─ DesktopAttachedSiteBridge
   │  ├─ ContentIsland
   │  ├─ InputPointerSource / InputKeyboardSource
   │  ├─ InputFocusController
   │  └─ text input + automation adapters
   │
   ├─ framework MTA
   │  └─ Doroti view/layout/scene build
   │
   ├─ raster MTA
   │  ├─ D3D12 device/queue
   │  ├─ Skia GRContext
   │  ├─ composition swap chain/backing
   │  └─ exact frame generation/terminal ledger
   │
   ├─ composition dispatcher thread
   │  └─ Windows.UI.Composition root
   │     ├─ opaque/background visual
   │     ├─ one visible content surface visual
   │     └─ validation-only grid/edge overlay
   │
   └─ frame-clock MTA
      └─ one cadence owner; callbacks only, no raster or window waits
```

`Windows.UI.Composition` system visual path는 현재 repository의 connection preflight가 이미 가진 구조를 먼저 사용한다. `Microsoft.UI.Composition` direct root는 별도 A/B feature probe에서 thread-affinity, swap-chain interop, resize cadence가 더 낫다는 증거가 있을 때만 채택한다. 두 composition API tree를 한 candidate에서 섞지 않는다.

### 2.2 새 코드 경계

구현 단계에서는 다음 책임을 분리한다. 이름은 계획상의 working name이며 public API로 확정한 것이 아니다.

- `WinRtCompositionHostAdapter`
  - Doroti capability 구현과 전체 lifecycle 조정
- `WinRtPlatformRuntime`
  - apartment, DispatcherQueue, WinRT 초기화/종료
- `WinRtAppWindowShell`
  - 최소 HWND, `WindowId`, `AppWindow`, standard presenter, show/close
- `WinRtContentIslandView`
  - `DesktopAttachedSiteBridge`, `ContentIsland`, site/island state
- `WinRtViewMetricsCoordinator`
  - physical/logical size, scale, display/transform generation
- `WinRtCompositionPresenter`
  - visual tree, surface brush, clip/offset, one-visible-front transaction
- `WinRtRasterSurface`
  - D3D12/Skia resource와 composition swap chain/backing
- `WinRtInputRouter`
  - pointer, wheel, keyboard, focus, cursor
- `WinRtTextInputAdapter`
  - CoreText feasibility path와 명시적 IMM32/TSF fallback
- `WinRtAutomationBridge`
  - `AutomationProviderRequested`와 Doroti semantics/UIA provider 연결
- `WinRtLifecycleManager`
  - visibility, minimize, occlusion, suspend, display/device loss, ordered shutdown
- `WinRtInterop`
  - 허용된 HWND/message-loop/DWM/DirectX COM 호출만 포함

`WinRtInterop` 형식이나 raw handle을 `Doroti.Ui`, `Doroti.Hosting`, renderer public API로 노출하지 않는다.

### 2.3 thread ownership

| owner | 소유 객체 | 금지 사항 |
|---|---|---|
| platform STA | HWND shim, AppWindow, site bridge, ContentIsland, input/focus/text/automation | framework layout, GPU wait, present/commit wait |
| framework MTA | Doroti view, metrics callback, scene build | HWND/WinRT thread-affine object 접근 |
| raster MTA | D3D12, Skia context, backing/swap chain, present terminal | AppWindow/ContentIsland 직접 변경 |
| composition dispatcher | compositor와 visual tree, surface attach, clip/offset transaction | scene build, input dispatch |
| frame-clock MTA | cadence wait와 timestamp publication | arbitrary 60Hz cap, raster, synchronous platform call |

thread-affine WinRT object 접근은 owning DispatcherQueue에 게시한다. `RPC_E_WRONG_THREAD`를 catch해서 다른 thread로 retry하는 방식은 금지하고 contract failure로 기록한다.

## 3. 핵심 계약

### 3.1 window와 content hosting

- top-level은 standard overlapped window이고 Windows가 caption, border, move, resize, system menu, Snap을 소유한다.
- HWND shim은 AppWindow가 content를 직접 attach할 수 없는 현재 API 공백을 메우는 container다.
- top-level client 전체에 `DesktopAttachedSiteBridge` 하나와 `ContentIsland` 하나만 연결한다.
- 새 backend에는 render/input용 `WS_CHILD` HWND를 만들지 않는다.
- top-level은 첫 exact frame이 composition root에 연결되기 전까지 숨긴다.
- first-frame failure, island disconnect, surface failure가 나면 빈 창을 보이지 말고 launch를 terminal failure로 끝낸다.
- custom title bar/non-client는 standard-shell baseline과 Snap/Layout/accessibility gate를 통과한 뒤 별도 기능으로만 검토한다.

### 3.2 metrics와 coordinate authority

한 `WinRtViewMetricsGeneration`은 다음 값을 immutable하게 묶는다.

- `ContentSiteView.ClientSize`: parent HWND client의 physical pixel extent
- `ContentIsland.ActualSize`: island logical extent
- `ContentIsland.RasterizationScale`
- local-to-client와 local-to-screen transform
- display ID, monitor/work-area identity
- visibility/enabled state

authority 규칙은 다음과 같다.

- render physical extent는 `ClientSize`가 authority다.
- framework logical extent는 `ActualSize`가 authority다.
- `round(ActualSize × RasterizationScale)`와 `ClientSize`는 축별 1px 이내여야 한다.
- 불일치가 1px을 넘으면 값을 조용히 보정하지 않고 generation을 `inconsistent`로 terminal 처리한 뒤 다음 site state를 기다린다.
- `AppWindow.Size`, outer HWND rect, monitor work area는 placement/lifecycle evidence이며 render extent authority가 아니다.
- pointer, popup anchor, IME caret, semantics bounds는 같은 generation의 coordinate converter를 사용한다.
- scale/display/transform/size 중 하나가 바뀌면 generation을 증가시키고 old exact frame을 새 extent로 relabel하지 않는다.

### 3.3 raster와 composition presentation

- D3D12/Skia resource는 raster thread만 생성·사용·폐기한다.
- product surface는 HWND EGL surface가 아니라 `CreateSwapChainForComposition` 또는 동등한 Direct3D-backed composition surface다.
- `ICompositorInterop.CreateCompositionSurfaceForSwapChain` 같은 native interop는 bounded bridge 안에서만 사용한다.
- visible root에는 content surface visual이 하나만 연결된다.
- buffer capacity와 visible client extent를 분리한다.
- capacity는 initial client + bounded reserve로 시작하고 필요할 때만 증가한다. monitor/virtual-desktop 크기의 permanent backing을 금지한다.
- `CompositionSurfaceBrush.Stretch=None` 또는 동등한 1:1 mapping을 사용한다.
- full-frame X/Y stretch, edge pixel repetition, `SetSourceSize` 왜곡을 exact content로 인정하지 않는다.
- resize 중 exact scene이 늦으면 last valid content는 1:1로 유지하고 새 영역은 명시적 background/transient 상태로 남긴다.
- background/transient update는 `exactPresented`가 아니다.

한 visible generation의 terminal 순서는 다음과 같다.

```text
SiteStateObserved
  → MetricsPublished
  → FrameworkSceneBuilt
  → RasterSubmitted
  → SwapChainPresented
  → CompositionTransactionSubmitted
  → CompositionCommitAcknowledged
  → Superseded | Failed | ExactTerminal
```

- running 1 + latest pending 1을 넘지 않는다.
- old target을 newer generation으로 relabel하지 않는다.
- composition attach/clip/offset/visible extent는 한 compositor transaction으로 제출한다.
- composition commit ACK는 compositor acceptance이며 scan-out evidence가 아니다.
- `DwmFlush`는 cadence/ordering 보조 수단일 뿐 visible PASS 증거가 아니다.
- mouse-up final target은 exactly-once terminal을 가지고 버려지지 않는다.

### 3.4 Flutter-style scheduler와 first frame

- platform/site state callback은 immutable metrics를 latest mailbox에 게시하고 즉시 반환한다.
- framework callback과 raster는 platform callback을 block하지 않는다.
- frame-clock owner는 refresh당 최대 한 useful target을 drain한다.
- timer 기반 arbitrary 60Hz throttle, resize debounce, mouse-up geometry replay를 금지한다.
- first frame은 current metrics generation과 exact physical extent가 일치해야 한다.
- first exact present와 root attach를 확인한 뒤 platform thread에서 AppWindow를 보인다.
- resize, ordinary invalidation, semantics-only update는 type과 terminal ledger를 분리한다.

### 3.5 input, focus, cursor

primary owner는 `ContentIsland`다.

- site bridge의 `ProcessesPointerInput`과 `ProcessesKeyboardInput`을 true로 설정한다.
- `InputPointerSource.GetForIsland`의 entered/exited/pressed/moved/released/wheel/capture-lost를 Doroti packet으로 변환한다.
- intermediate points, pointer ID, device type, timestamp, contact/buttons, modifiers를 보존한다.
- mouse, touch, pen을 같은 pointer lifecycle에서 구분한다.
- hover pointer는 `add → hover/move → remove`, contact는 `add → down → move → up/cancel → remove`를 유지한다.
- `InputKeyboardSource`의 ordinary/system key, character, dead-key, surrogate ordering을 검증한다.
- `InputFocusController`가 focus request, got/lost focus, keyboard cue를 소유한다.
- cursor는 `InputPointerSource.Cursor`가 primary다.
- island 밖 drag에서 release/cancel이 오지 않으면 `SetCapture` 또는 TSF/Win32 routing을 작은 shim으로 추가하되 dual event emission을 금지한다.
- raw WndProc와 WinRT source가 동시에 같은 pointer/key event를 Doroti에 전달하는 mode는 허용하지 않는다.

### 3.6 text input과 Korean IME

CoreText는 목표 방향이지만 현재 desktop island에서 먼저 feasibility를 증명한다.

1. `CoreTextServicesManager`/`CoreTextEditContext`가 unpackaged ContentIsland platform thread에서 생성 가능한지 확인한다.
2. text request/update, selection, composition start/update/complete, layout request, focus enter/leave를 Doroti editing state와 연결한다.
3. caret/selection rect는 current metrics generation의 local-to-screen transform으로 전달한다.
4. Korean two-beolsik 조합, candidate window, caret, selection replacement를 실제 OS IME로 검증한다.
5. CoreText가 CoreWindow/view 제약으로 실패하거나 candidate 위치를 보장하지 못하면 기존 IMM32 implementation 또는 bounded TSF adapter를 `WinRtTextInputFallback`으로 유지한다.

WinRT keyboard event 자동 PASS만으로 IME PASS를 선언하지 않는다. 문서/API상 IME가 keyboard event를 소비할 수 있으므로 text service path의 physical result가 hard gate다.

### 3.7 accessibility와 semantics

- `ContentIsland.AutomationProviderRequested`에서 Doroti fragment-root provider를 반환한다.
- `ContentIsland.GetAutomationHostProvider()`와 parent/sibling provider 관계를 보존한다.
- semantics bounds, hit test, focus, action dispatch는 current metrics generation과 같은 transform을 사용한다.
- provider 요청 중 framework/raster를 synchronous wait하지 않는다. 최신 immutable semantics snapshot을 사용한다.
- UIA tree automation은 구조 evidence이며 Narrator/Accessibility Insights physical PASS를 대신하지 않는다.

### 3.8 lifecycle과 teardown

다음을 서로 다른 state로 기록한다.

- AppWindow visible/hidden/minimized/maximized/fullscreen
- ContentIsland connected/disconnected, enabled/visible
- occluded/unoccluded
- display/scale/transform change
- D3D device/context loss
- close/shutdown during resize or pending frame

정상 teardown 순서는 다음과 같다.

```text
stop new input/text/automation requests
→ suspend scheduler and terminalize pending generations
→ stop framework callbacks
→ drain/stop raster and release GPU resources
→ detach/dispose composition visuals and surface
→ dispose input/focus/text/automation adapters
→ disconnect/dispose ContentIsland and SiteBridge
→ destroy AppWindow/HWND shim
→ shutdown composition/platform DispatcherQueues
→ uninitialize WinRT/OLE/COM
```

device loss는 AppWindow/ContentIsland를 먼저 파괴하지 않는다. raster resources와 composition surface를 새 generation으로 재생성하고 exact recovery frame 뒤 정상 표시 상태로 복귀한다.

## 4. 실행 순서와 hard gate

```text
W0 API/projection and baseline lock
→ W1 independent WinRT island visual spike
→ D1 standard-shell ownership decision
→ W2 backend boundary and selector
→ W3 AppWindow/ContentIsland/metrics core
→ W4 composition raster and scheduler
→ W5 input/text/accessibility
→ W6 resize visible gate
→ W7 Windows behavior and lifecycle
→ W8 product/package integration
→ FG final acceptance
→ release cutover decision
```

- W0/W1 전에는 fail-fast 경계 밖의 product backend 구현을 시작하지 않는다.
- W1은 Doroti framework 없이 API와 visible behavior를 판별한다.
- D1 결정 전에는 standard-shell candidate를 product backend로 확대하지 않는다.
- W5 physical input/IME/accessibility 전에는 공개 Windows 기본값을 MAUI에서 Windows App SDK로 바꾸지 않는다.
- W6 visible PASS 전에는 대규모 performance matrix나 cleanup을 하지 않는다.
- MAUI backend를 Windows App SDK acceptance나 cleanup 조건에 종속시키지 않는다.

### 반복 횟수 정책

- interactive physical/synthetic scenario는 조건별 기본 2회다.
- 첫 실행에서 명확한 visible FAIL, crash, hang, contract violation이 나오면 즉시 FAIL로 기록한다.
- 두 결과가 충돌하거나 비결정 failure를 재현할 때만 해당 조건에 한해 추가 실행한다.
- 수백 회 factorial run을 기본 acceptance로 사용하지 않는다.
- structural/unit test의 내부 case 수와 lifecycle soak 횟수는 interactive scenario 제한과 별개다.

## 5. W0 — API/projection과 baseline lock

### 작업

1. exact `Microsoft.WindowsAppSDK 2.4.0`과 resolved projection/runtime package, assembly SHA-256를 기록한다.
2. central `1.8.260508005`와 host `VersionOverride=2.4.0`의 package graph를 inventory하되 이 단계에서 upgrade/downgrade하지 않는다.
3. current projection의 required API를 source/reflection contract로 고정한다.
   - `ContentIsland.CreateForSystemVisual`
   - `DesktopAttachedSiteBridge.CreateFromWindowId`
   - `ContentSiteView.ClientSize/ActualSize/RasterizationScale`
   - `InputPointerSource.GetForIsland`
   - `InputKeyboardSource.GetForIsland`
   - `InputFocusController.GetForIsland`
   - `ContentIsland.AutomationProviderRequested`
4. 현재 projection에 `ContentAppWindowBridge`가 없음을 explicit capability로 기록한다.
5. official Windows App SDK Islands sample의 thread, bridge, input, DPI, accessibility 구조를 current package API와 대조한다.
6. 독립 native grid/capture tool, bare standard native control, fail-fast runner의 fingerprint를 baseline manifest로 묶는다.

### W0 PASS 조건

- required projected API와 native interop entrypoint가 compile-time/runtime probe에서 모두 확인됨
- package/projection/runtime identity가 하나의 manifest에 고정됨
- pure WinRT가 아닌 `attached HWND shim + ContentIsland` 경계가 contract에 명시됨
- missing API, experimental API, OS-version 조건이 silent fallback 없이 기록됨
- 이전 adapter dependency/reference 0, 새 backend implementation 변경 0

required API가 없거나 self-contained deployment에서 activation되지 않으면 W1로 진행하지 않는다.

## 6. W1 — Independent WinRT island visual spike

### 범위

새 validation target에서만 구현한다. Doroti framework, product renderer, current scheduler를 연결하지 않는다.

### 구현

1. standard top-level HWND shim과 AppWindow를 만든다.
2. `DesktopAttachedSiteBridge`와 one `ContentIsland`를 client 전체에 연결한다.
3. dedicated system compositor와 one root visual을 만든다.
4. D3D-backed composition swap chain/surface에 asymmetric procedural grid, right/bottom marker, solid background를 그린다.
5. `ContentSiteView` state로 logical/physical metrics generation을 만든다.
6. input/keyboard/focus source를 island primary로 연결하고 raw WndProc client input은 끈다.
7. first exact frame 뒤에만 window를 보인다.
8. resize 동안 latest target만 render하고 root clip/extent와 surface를 한 transaction으로 갱신한다.
9. bare standard native control과 같은 monitor/rect에서 side-by-side physical test를 한다.

### W1 contract PASS 조건

- child render HWND 0
- connected ContentIsland 1, site bridge 1, visible content surface 1
- metrics generation reversal/duplicate terminal/stale exact present 0
- first show before exact frame 0
- pointer/keyboard/focus duplicate owner 0
- resize 중 black/white/transparent band, flicker, non-uniform grid scale 0
- close during resize, island disconnect, surface recreate에서 hang 0

### W1 physical/visible 비교

- same monitor, DPI, refresh, initial rect
- Left/Top/Right/Bottom fast `600px/150ms`, medium `600px/300ms`, slow/fine drag
- expand, shrink, immediate reverse 각 2회
- border cadence, cursor-edge 추종, content/grid continuity, opposite edge를 분리 판정
- raw capture와 가능하면 240fps 이상 external video의 provenance를 분리

## 7. D1 — standard-shell ownership 결정

W1 결과에 따라 architecture를 다음처럼 결정한다.

### A. standard-shell WinRT candidate 계속

다음을 모두 만족하면 W2로 진행한다.

- 동일 조건의 bare standard native control과 비교해 content continuity가 동등 이상이라고 사용자가 판정
- border와 content mismatch가 W6 목표 threshold 안에 들어갈 가능성이 보임
- Snap/system menu/taskbar/native caption을 그대로 보존
- island metrics/input이 raw child HWND보다 단순한 one-owner 구조를 실제로 형성

### B. platform floor 수용 여부 결정

bare native control과 WinRT spike가 같은 border cadence를 보이고 content만 정상이라면, standard Windows shell cadence를 제품 한계로 수용할지 사용자/product decision을 받는다. 수용하지 않으면 W2로 자동 진행하지 않는다.

### C. WinRT-owned envelope 별도 spike

standard shell에서도 동일한 visible FAIL이 재현되고 platform floor 수용을 거부할 때만 다음 후보를 별도 승인한다.

- fixed envelope HWND/AppWindow
- `ContentIsland` root 안에서 app-owned chrome/client geometry
- island input/focus/accessibility primary
- one visible composition surface
- idle exact input region과 capture region lifecycle

이 fallback은 이전 구현을 복원하지 않고 새 WinRT ownership으로 설계한다. AppWindow가 보는 full envelope와 사용자가 보는 owned rect가 달라지는 Snap, taskbar preview, maximize, UIA bounds 문제를 먼저 해결해야 한다. 별도 architecture 승인 없이는 default 후보가 아니다.

## 8. W2 — backend boundary와 selector

### 작업

1. 현재 fail-fast `DorotiWindowsAppSdkRunner`를 실제 `WinRtComposition` backend boundary로 교체한다.
2. `DOROTI_WINDOWS_ADAPTER=WinRtComposition`만 Windows App SDK runner가 허용한다.
3. target descriptor의 유일한 Windows App SDK capability를 `WinRtComposition`으로 유지한다.
4. WinRT backend 코드는 새 namespace/folder에 두고 제거한 backend source를 복원하거나 복사하지 않는다.
5. host-neutral capability 경계 밖으로 `AppWindow`, `ContentIsland`, raw handle을 노출하지 않는다.
6. architecture test에 다음을 추가한다.
   - XAML/MAUI/`SwapChainPanel` dependency 금지
   - child render HWND creation 금지
   - ANGLE/EGL dependency 금지
   - raw interop callsite allowlist

### W2 PASS 조건

- Windows App SDK default/capability가 `WinRtComposition` 하나로 일치
- `-WindowsBackend WindowsAppSdk|Maui` 선택과 Windows App SDK 내부 capability가 분리됨
- WinRT-only validation consumer가 host를 load할 수 있음
- new code의 Win32/COM callsite가 allowlist 밖에 0
- 제거된 backend evidence/validator를 새 backend PASS로 오표기한 항목 0

## 9. W3 — AppWindow/ContentIsland/metrics core

### 작업

1. platform STA bootstrap과 one DispatcherQueue contract를 구현한다.
2. HWND shim → WindowId → AppWindow → SiteBridge → ContentIsland 순서와 역순 teardown을 고정한다.
3. island/site state callback을 immutable metrics mailbox로 연결한다.
4. `ClientSize`, `ActualSize`, scale, transform, display identity consistency validator를 만든다.
5. framework logical origin을 island `(0,0)`으로 통일한다.
6. screen conversion을 ContentCoordinateConverter 한 경로로 통일한다.
7. size/scale/display/visibility가 바뀔 때 generation과 terminal ledger를 기록한다.

### W3 PASS 조건

- 100%, 125%, 150%, 200%에서 logical/physical round-trip 오차 ≤ 1px
- negative virtual origin과 secondary monitor coordinate round-trip 오차 ≤ 1px
- AppWindow outer size가 render metrics로 유입되는 경우 0
- stale/duplicate/reversed metrics generation 0
- callback의 framework/raster wait 0
- connect/disconnect/reconnect 100회 leak/hang 0

## 10. W4 — composition raster와 scheduler

### 작업

1. D3D12/Skia composition swap chain/backing을 raster thread에 구현한다.
2. swap chain을 composition surface/brush/visual에 연결하는 bounded interop를 구현한다.
3. capacity growth, exact extent, clip, content offset을 분리한다.
4. scheduler에 running 1 + latest pending 1, exactly-once terminal을 적용한다.
5. first-frame hidden/show handshake를 구현한다.
6. resize 중 transient/background와 exact scene을 별도 terminal로 기록한다.
7. device loss/context loss/surface recreate를 새 surface generation으로 처리한다.

### W4 PASS 조건

- one visible content surface/front
- simultaneous visible front 2개인 frame 0
- wrong-size/stale exact present 0
- non-uniform stretch 0
- first exact → show exactly once
- 100-cycle create/show/resize/close와 20-cycle device recovery hang/leak 0
- mouse-up final exact ≤ 100ms
- renderer 50ms fault가 platform size callback이나 native border를 block한 횟수 0

## 11. W5 — input, text, accessibility

### automated 작업

- mouse/touch/pen lifecycle, wheel, intermediate points, modifiers
- island 밖 drag와 capture lost/cancel
- cursor 변경과 focus request/got/lost
- ordinary/system/dead key, character, surrogate pair
- clipboard와 popup anchor
- CoreText creation/notification feasibility
- automation provider request, fragment navigation, bounds, hit test, action dispatch

### physical hard gate

- real mouse border resize 후 client re-entry/click/cursor recovery
- touch/pen 가능한 범위
- Korean two-beolsik composition/candidate/caret/selection
- Alt+Tab/minimize/restore/popup focus
- Narrator focus/read/action
- Accessibility Insights tree/action/bounds

### W5 PASS 조건

- raw/WinRT duplicate pointer/key packet 0
- press/release/cancel terminal 누락 0
- focus와 text client state mismatch 0
- IME candidate/caret 위치 오차 ≤ 2 physical px
- semantics bounds와 visual bounds 오차 ≤ 1 physical px
- automation callback의 framework/raster synchronous wait 0

자동 gate가 통과해도 physical Korean IME/Narrator/Accessibility Insights가 실행되지 않으면 해당 항목은 `notVerified`이며 W6 product candidate로 승격하지 않는다.

## 12. W6 — resize visible gate

W5를 통과한 동일 candidate binary 하나만 사용한다.

### matrix

- Left, Top, Right, Bottom, four corners
- expand, shrink, immediate reverse
- `600px/150ms`, `600px/300ms`, slow/fine drag
- initial logical size 420×300, 640×360, 1000×600
- 가능한 DPI 100%, 125%, 150%, 200%
- 가능한 refresh 60, 120, 144, 165Hz
- 각 physical 조건 기본 2회

### W6 PASS 조건

- 사용자가 border/chrome/grid/content continuity를 edge별 직접 PASS
- reverse, flicker, blank, black/white/transparent band 0
- grid X/Y non-uniform scale ≤ 2%
- uncovered gap p95/max ≤ 1 physical px
- cursor-owned edge lag p95 ≤ 1 refresh 이동량, max ≤ 2 refresh 이동량
- opposite edge drift ≤ 1 physical px
- final lag ≤ 1 physical px
- mouse-up final exact ≤ 100ms
- raw frame/encoder/trace drop 0

AppWindow/site callback, frame terminal, composition commit ACK가 PASS해도 사용자 visible FAIL이면 W6는 FAIL이다. 한 edge의 PASS를 다른 edge/corner로 확대하지 않는다.

## 13. W7 — Windows behavior와 lifecycle

### Windows shell

- move drag와 double-click maximize/restore
- native caption buttons와 hover/pressed state
- `Alt+Space` system menu와 keyboard commands
- taskbar activate/minimize/restore, Alt+Tab
- Win+Arrow Snap, Snap Layouts hover, unsnap restore bounds
- minimum/maximum track size
- fullscreen/compact overlay가 제품에서 지원될 경우 enter/exit
- high contrast, reduced motion, RTL title/layout

### display/lifecycle

- primary/secondary, negative origin, mixed DPI monitor 이동
- minimize/restore, occlusion/unocclusion
- sleep/resume
- display disconnect/reconnect
- RDP attach/detach
- resize 중 close/shutdown
- island disconnect/reconnect
- device/context loss와 recovery

### W7 hard gate

- current product보다 Windows shell 기능이 퇴행한 항목 0
- teardown order violation, wrong-thread access, orphan DispatcherQueue 0
- restore 뒤 stale frame/input/semantics generation 0
- 가능한 physical matrix에서 crash/hang/resource leak 0

필수 Windows 기능 하나라도 퇴행하면 default cutover를 금지한다.

## 14. W8 — product와 package integration

### 작업

1. `DorotiWindowsAppSdkRunner`의 fail-fast 경계를 실제 backend launch로 교체하고 generated template에 Windows App SDK와 MAUI runner를 모두 제공한다.
2. Release build, clean publish, restricted-PATH launch를 실행한다.
3. self-contained unpackaged deployment와 Windows App Runtime absent/update 조건을 검증한다.
4. native interop DLL이 생기면 RID/architecture/hash/load failure를 negative-test한다.
5. product demo의 scroll, animation, popup, text input, semantics를 sustained smoke로 실행한다.
6. MAUI backend의 build, launch, package evidence를 Windows App SDK artifact provenance와 분리해 확인한다.
7. W0~W7 전체가 PASS한 뒤에만 공개 Windows 기본값을 MAUI에서 Windows App SDK로 바꾸도록 별도 승인한다.

### W8 PASS 조건

- 두 backend의 package-only consumer와 generated template launch PASS
- clean publish가 local build output에 의존하지 않음
- wrong architecture/missing/corrupt native dependency가 diagnostic failure로 종료됨
- MAUI backend가 독립적으로 build/launch 가능
- fail-fast 교체 전/후 binary와 target manifest fingerprint가 기록됨

## 15. FG — 최종 제품 acceptance

W6~W8을 통과한 동일 candidate binary 하나만 사용한다.

### automated evidence

- API/projection and interop allowlist validator
- ContentIsland/metrics/input/automation structural validator
- scheduler/causal/terminal/device-recovery validator
- Release build, clean publish, package-only consumer, generated template
- lifecycle soak와 resource distribution

### physical/visible evidence

- four-edge/corner fast/medium/slow resize
- sustained scroll/animation 중 resize
- Korean IME composition/candidate/caret
- pointer/capture/cursor/focus/popup
- Narrator와 Accessibility Insights
- mixed-DPI monitor 이동
- minimize/maximize/Snap/fullscreen
- sleep/resume와 가능한 RDP

### FG 판정

- automated, physical, visible, deployment, separate-host evidence를 분리한다.
- build/process/contract 성공을 visible/compositor/physical PASS로 승격하지 않는다.
- composition commit ACK, DwmFlush return, WGC frame cadence를 scan-out proof로 해석하지 않는다.
- release 범위와 충돌하는 `notVerified`가 하나라도 남으면 default/release cutover를 승인하지 않는다.
- 제거한 backend를 release fallback으로 다시 도입하지 않는다.

## 16. 성능 budget

성능 최적화는 W6 visible geometry가 PASS한 뒤 수행한다.

- site/input callback p99 ≤ 1ms, max ≤ 4ms
- metrics publication → framework drain p95 ≤ 1 refresh
- raster submit → swap-chain present timing을 별도 기록
- composition transaction submit → ACK를 별도 기록
- ordinary animation frame p95 ≤ one refresh budget
- interactive resize queue depth: running 1 + pending 1
- final exact ≤ 100ms
- sustained 10분 run에서 GPU/managed/native memory, handles, threads가 bounded

최적화 허용 순서는 다음과 같다.

1. unnecessary full clear/copy 제거
2. capacity growth를 interactive hot path 밖으로 이동
3. exact와 transient 우선순위 정리
4. resource reuse와 dirty region 최적화
5. frame-clock/commit ordering 조정

thread priority, arbitrary frame cap, debounce, timeout 증가는 timing 원인이 증명된 경우에만 별도 변경으로 허용한다.

## 17. Evidence 규격

각 run은 다음 provenance를 기록한다.

- source revision, dirty state, binary/package fingerprint
- Windows App SDK package/projection/runtime identity와 assembly hash
- backend/shell/compositor API identity
- HWND shim count, AppWindow ID, SiteBridge/ContentIsland ID
- input/text/accessibility owner와 fallback mode
- GPU adapter, D3D feature level, Skia backend, software fallback
- monitor rect/work area, DPI/scale, refresh, resolution
- physical mouse/touch/pen 또는 synthetic input identity
- metrics generation: client/actual/scale/transform/display
- frame generation: scene/raster/swap/present/commit/terminal
- visible surface/front ID, capacity, clip, extent, exact/transient state
- raw cursor/window/island/composition timeline
- capture/video와 derived analysis
- 사용자 edge별 visible 판정

기존 backend evidence와 새 WinRT evidence를 같은 run ID나 PASS ledger에 합치지 않는다.

## 18. 금지 조건

- 현재 API에서 `pure WinRT`, `HWND-free`라고 주장
- 존재하지 않는 `ContentAppWindowBridge`를 전제로 설계
- AppWindow를 render content attachment API로 사용
- WinUI XAML, MAUI, `SwapChainPanel`을 새 backend 내부에 도입
- child render/input HWND 생성
- raw WndProc와 WinRT input의 dual emission
- `AppWindow.Changed` outer geometry만 framework metrics로 사용
- pointer/site callback에서 framework/GPU/present/commit wait
- composition ACK나 DwmFlush를 visible/scan-out PASS로 계상
- full-frame stretch, `SetSourceSize` 왜곡, edge replication을 exact로 계상
- dual simultaneously visible fronts
- old frame을 새 metrics generation으로 relabel
- mouse-up geometry replay 또는 final-only debounce
- monitor/virtual-desktop 크기의 permanent render backing
- physical visible FAIL을 자동 counter로 덮기
- W7 전 custom title bar/default cutover
- FG 전 fail-fast runner를 미검증 backend launch로 교체
- W0 승인 전 package upgrade 또는 새 backend source implementation 수행

## 19. 검토 근거

- [Windows App SDK windowing overview](https://learn.microsoft.com/en-us/windows/apps/develop/ui/windowing-overview)
- [Manage app windows and HWND interop](https://learn.microsoft.com/en-us/windows/apps/develop/ui/manage-app-windows)
- [Windows App SDK 1.7 ContentIsland changes](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/release-notes/windows-app-sdk-1-7)
- [ContentIsland overview](https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island)
- [ContentIsland API](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.contentisland)
- [ContentSite API](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.content.contentsite)
- [InputPointerSource API](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputpointersource)
- [InputKeyboardSource API](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputkeyboardsource)
- [InputFocusController API](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputfocuscontroller)
- [Composition native DirectX interop](https://learn.microsoft.com/en-us/windows/apps/develop/composition/composition-native-interop)
- [Custom text input and CoreText](https://learn.microsoft.com/en-us/windows/apps/develop/input/custom-text-input)
- [Windows App SDK Islands samples](https://github.com/microsoft/WindowsAppSDK-Samples/tree/main/Samples/Islands)

official documentation은 architecture feasibility 근거이며 current exact package의 실제 API/projection과 runtime behavior를 대신하지 않는다. W0에서 local projection과 다시 대조한다.

## 20. 정확한 재개 지점

다음 작업은 **W0와 W1 independent validation 범위**로 제한한다.

1. exact 2.4 package/projection/runtime identity와 required API manifest를 만든다.
2. `ContentAppWindowBridge` 부재와 허용할 HWND/COM interop allowlist를 contract로 고정한다.
3. product source와 분리된 WinRT ContentIsland procedural-grid target을 만든다.
4. AppWindow + DesktopAttachedSiteBridge + ContentIsland + one composition surface를 연결한다.
5. SiteView metrics와 island input/focus의 one-owner contract를 검증한다.
6. bare native control과 same-condition physical four-edge 비교를 수행한다.
7. 사용자 visible 판정으로 D1 standard-shell ownership을 결정한다.
8. D1이 A로 승인될 때만 W2 product backend boundary를 구현한다.

현재 정확한 상태는 **이전 Windows App SDK backend 제거 완료 / WindowsAppSdk·Maui Release build PASS / WinRtComposition notStarted / Windows App SDK runner intentional fail-fast 확인 / W0 notStarted**다. MAUI 제품 runtime과 새 Windows App SDK의 physical/visible 검증은 수행하지 않았다.

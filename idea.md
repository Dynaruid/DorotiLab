# Windows interactive resize 다음 구조 제안

## 0. 결론

- 작성일: 2026-08-23
- 검토 checkout: `753a8df4`
- 근거 범위: 현재 `work.md`, Windows host 구현, 고정 Flutter source, 로컬 Avalonia source, Microsoft 공식 API 문서
- 검증 상태: **문서/API/source 검토만 완료**. 아래 후보의 build, runtime, WGC acceptance는 모두 `notVerified`다.
- provenance 주의: `work.md` 머리말의 checkout은 `21320c77`이지만 현재 checkout은 `753a8df4`다. 실행 수치와 run ID는 `work.md`의 기록을 인용했고 재실행하지 않았으며, 코드 위치와 package graph는 현재 checkout에서 다시 확인했다.

다음 Windows 후보는 `SwapChainPanel`을 다시 조정하거나 raw child HWND의 resize 순서를 더 미세하게 바꾸는 방식이 아니어야 한다. 우선순위는 다음과 같다.

1. **MAUI/WinUI를 유지한 `Microsoft.UI.Composition` surface spike**를 수행한다.
   - raw render child HWND와 `SwapChainPanel`을 가시 경로에서 제거한다.
   - WinUI visual tree 안의 `SpriteVisual` 하나가 보이는 surface를 소유한다.
   - 보이는 front를 직접 resize하지 않고, 보이지 않는 exact staging `CompositionDrawingSurface`에 그린 뒤 surface/size/offset/clip을 한 compositor transaction으로 교체한다.
2. 위 경로가 동일한 G1 WGC gate를 통과하지 못하면 **순수 Win32 top-level host**로 ownership을 옮긴다.
   - MAUI/WinUI top-level compositor를 Windows 제품의 visible path에서 제거한다.
   - 같은 composition presenter를 `ICompositorDesktopInterop.CreateDesktopWindowTarget(HWND, ...)`에 연결한다.
3. 두 후보 모두 실패하기 전에는 ANGLE raw HWND, `WM_SIZING` pre-present ordering, timeout/debounce, `SetSourceSize` capacity buffer를 다시 시도하지 않는다.

이 순서의 핵심은 renderer를 먼저 바꾸는 것이 아니라 **보이는 front의 수명과 창 geometry를 같은 compositor가 소유하게 만드는 것**이다. 현재 W0는 D3D12 raster/copy correctness는 통과했지만, WinUI parent geometry와 raw child HWND present 사이에는 원자적 commit API가 없어서 strict visual gate를 통과하지 못했다.

Web B0~B2는 이 문서에서 새로 승격하지 않는다. `work.md`의 ordered gate대로 Windows G1이 해결될 때까지 `notStarted`/`notVerified`를 유지한다.

## 1. 현재 W0에서 확인된 것

### 1.1 유지할 것

- `DorotiResizeEpoch`, build token, exact-size admission, `FrameTransaction`, exactly-once terminal ledger는 원인 진단과 stale frame 차단에 유효하다.
- exact D3D12 offscreen backing store와 GPU-only copy 자체는 runtime correctness를 통과했다.
- raster 시간만으로 현재 23~32Hz app-present와 수백 ms의 visible gap을 설명할 수 없다.
- input 수명주기와 startup visibility 회귀 수정은 보존해야 한다.

### 1.2 폐기할 ownership

현재 구현은 다음 두 visible domain을 동시에 사용한다.

```text
WinUI/MAUI top-level HWND + XAML compositor
  └─ raw STATIC child HWND
       └─ CreateSwapChainForHwnd + D3D12 Present
```

코드상 raw child는 `DorotiWindowsDxgiSurface.cs:819-831`에서 생성되고, parent `WM_SIZE`가 `ResizeChildToParent()`를 호출하며(`:956-962`, `:1173-1190`), child에는 별도 flip-model HWND swap chain이 연결된다(`:1825-1847`). exact frame은 `ResizeBuffers → CopyResource → Present(0) → DwmFlush`로 제출된다(`:1873-1940`).

이 구조에서 다음 네 사건은 하나의 API transaction이 아니다.

1. DWM이 top-level border/client geometry를 변경한다.
2. WinUI가 자신의 XAML/composition tree를 새 client 크기로 처리한다.
3. app이 raw child HWND를 이동/확장한다.
4. child의 DXGI swap chain이 exact buffer를 present한다.

`DwmFlush`는 호출한 프로세스가 큐에 넣은 DirectX 변경을 기다리는 API이지, WinUI parent layout과 child `SetWindowPos`, child swap-chain present를 하나의 원자적 commit으로 묶는 API가 아니다. 실제 WGC가 `Present` 성공 뒤에도 border-before-frame gap을 본 것은 이 경계와 일치한다.

### 1.3 Flutter에서 그대로 복사하면 안 되는 부분

Flutter reference는 여전히 resize handshake의 좋은 protocol reference다.

- `flutter_windows_view.cc:208-259`: `WM_SIZE` target을 게시하고 최대 100ms 동안 matching frame/present를 기다린다.
- `flutter_windows_view.cc:747-775`: matching frame present 뒤 platform thread를 깨우고 raster thread에서 `DwmFlush`한다.
- `flutter_window.cc:481-513`: renderer HWND를 자체 Win32 window class로 생성한다.
- runner `win32_window.cpp:245-253`: 고전 Win32 top-level HWND에 render child를 직접 reparent/resize한다.

하지만 Flutter parent는 MAUI/WinUI XAML compositor가 아니다. 따라서 Flutter의 `100ms wait + EGL fixed-size surface + DwmFlush`만 현재 WinUI parent에 이식해도 같은 ownership이 되지 않는다. 실제 W0 ordering/ANGLE 결과가 이 차이를 확인했다.

## 2. API 검토에서 얻은 설계 제약

| API | 공식 보장 | 이 작업에서의 결론 |
| --- | --- | --- |
| `Microsoft.UI.Content.ContentIsland` / `ChildSiteLink` | WinUI 3 안에 Composition visual, Win2D, Direct3D surface 같은 non-XAML content를 넣고 input/output/layout/accessibility 경계를 가질 수 있다. `ChildSiteLink`는 Windows App SDK 1.7+에 있다. | 현재 resolved Windows App SDK `1.8.260508005`에서 MAUI 유지형 spike가 가능하다. 실제 C# projection/build는 `notVerified`. |
| `CompositionDrawingSurface` native interop | `BeginDraw`로 받은 texture의 update는 `EndDraw` 전에는 composition에 노출되지 않고, visual tree commit 전에는 화면에 나타나지 않는다. | visible front를 파괴적으로 resize하지 않는 exact staging/atomic switch에 가장 직접적으로 맞는다. |
| `Compositor.RequestCommitAsync` | 비동기 commit cycle을 요청한다. | completion은 compositor commit 경계이지 실제 scan-out 증명이 아니다. `VisibleSurfaceCommitted`까지만 의미를 부여한다. |
| `ID3D11On12Device2.UnwrapUnderlyingResource` / `ReturnUnderlyingResource` | D3D11 resource를 같은 queue에서 D3D12 resource로 사용하고 fence/value로 D3D11On12에 반환할 수 있다. 최소 OS는 Windows 10 2004, build 19041이다. | 프로젝트의 Windows 최소 target과 맞는다. 현재 D3D12/Skia offscreen 결과를 composition surface texture로 GPU-only copy하는 bridge 후보이다. |
| `CreateSwapChainForComposition` | composition/XAML용 flip-model swap chain을 만들며 `Scaling=Stretch`가 필수다. | exact 크기일 때만 surface를 교체하면 쓸 수 있지만, size mismatch 중 no-stretch 정책을 API가 대신 보장하지 않는다. drawing-surface interop 실패 시에만 제한된 fallback으로 둔다. |
| `CreateSwapChainForHwnd` / DXGI flip model | HWND 하나에는 flip-model swap chain 하나만 사용하고, 그 HWND를 다른 presentation API와 혼용하지 말아야 한다. | raw child를 유지한 채 다른 chain/overlay를 더 붙이지 않는다. pure Win32 fallback에서는 top-level HWND 자체가 유일한 target이어야 한다. |
| `SetSourceSize` | swap-chain의 `[0,0,width,height]` source region을 선택해 `ResizeBuffers` 없이 effective resize를 제공한다. | 임의 anchor나 exact frame/geometry atomicity는 제공하지 않는다. 기존 visible overflow 실패도 있으므로 제품 해법으로 되살리지 않는다. |
| `Present(0)` | flip queue의 오래된 frame은 더 최신 frame에 의해 discard될 수 있다. | 성공 반환이나 app trace를 실제 화면 표시 ACK로 부르지 않는다. |
| `DwmFlush` | 현재 호출 프로세스가 큐에 둔 DirectX 변경이 그려질 때까지 기다린다. | commit 뒤 bounded confirmation에는 쓸 수 있지만 size ownership을 고치지 못한다. hot path cadence를 만드는 timer로 쓰지 않는다. |
| `IDCompositionDevice.WaitForCommitCompletion` | 이전 `Commit`을 composition engine이 처리할 때까지 기다린다. | DirectComposition fallback의 processed-commit 신호일 뿐 scan-out acceptance가 아니다. |

또한 로컬 Avalonia source는 다음 구조를 사용한다.

- native Win32 HWND에 `CreateDesktopWindowTarget`을 연결한다: `reference/Avalonia-main/src/Windows/Avalonia.Win32/WinRT/Composition/WinUiCompositedWindow.cs:37-52`
- `CompositionSurfaceBrush`의 surface를 교체한다: 같은 파일 `:71-81`
- `CompositionDrawingSurface`를 만들고 `BeginDraw/EndDraw`로 texture를 갱신한다: `WinUiCompositedWindowSurface.cs:118-137`, `:149-215`, `:237-248`
- DirectComposition fallback은 `CreateTargetForHwnd → SetContent → Commit` 구조를 사용한다: `DComposition/DirectCompositedWindow.cs:27-51`

Avalonia의 구현을 그대로 복사할 이유는 없지만, **XAML control의 swap-chain attachment가 아니라 native HWND/Composition visual/surface를 명시적으로 소유하는 구조가 실제 desktop framework에서 사용되고 있음**은 참고할 가치가 있다.

## 3. 제안 A — MAUI를 유지하는 Composition surface presenter

### 3.1 visible graph

```text
WinUI top-level / XAML root
  └─ DorotiWindowsDxgiHost (layout + current IME/semantics boundary)
       └─ placement ContainerVisual
            └─ SpriteVisual (visible owner는 이것 하나)
                 └─ CompositionSurfaceBrush
                      └─ exact front CompositionDrawingSurface
```

첫 visual spike는 `ElementCompositionPreview.SetElementChildVisual`로 충분한지 먼저 본다. full product에서 independent input/output/accessibility boundary가 필요하면 동일 visual을 `XamlRoot.ContentIsland + ChildSiteLink + child ContentIsland`로 옮긴다. 둘을 동시에 도입하지 않는다.

### 3.2 surface graph

```text
current epoch exact scene
  -> existing D3D12/Skia offscreen backing store
  -> staging CompositionDrawingSurface.BeginDraw(full rect)
  -> returned ID3D11Texture2D
  -> D3D11On12 UnwrapUnderlyingResource(ID3D12Resource)
  -> same D3D12 queue CopyResource / CopyTextureRegion
  -> signal fence
  -> ReturnUnderlyingResource(fence, value)
  -> EndDraw
  -> brush.Surface + visual.Size/Offset/Clip atomic property update
  -> RequestCommitAsync
  -> front/staging role swap
```

필수 조건은 다음과 같다.

- visible `front` surface에는 `Resize`/`BeginDraw`를 하지 않는다.
- staging surface만 exact target으로 만들거나 resize한다.
- `BeginDraw`가 돌려준 offset을 무시하지 않는다. copy destination 좌표에 반드시 반영한다.
- D3D12 copy가 `COMMON` 상태로 끝나고 fence가 제출된 뒤 `ReturnUnderlyingResource`한다.
- `ReturnUnderlyingResource → EndDraw` 전에 staging을 brush에 연결하지 않는다.
- epoch가 latest가 아니면 staging을 visible로 교체하지 않고 `Superseded` terminal로 끝낸다.
- brush surface, visual physical size, offset, clip, scale, transaction ID를 같은 compositor batch에서 바꾼다.
- `RequestCommitAsync` completion은 `VisibleSurfaceCommitted`; WGC/PresentMon 같은 외부 관측만 strict visual/cadence evidence다.

### 3.3 provisional 표시

exact staging이 준비되기 전에는 front를 늘리지 않는다. `WM_SIZING`의 edge identity를 이용해 이전 exact front를 **움직이지 않는 edge에 평행이동으로 고정**한다.

| drag edge | old front provisional anchor |
| --- | --- |
| left | right 고정, `offsetX = newWidth - oldWidth` |
| right | left 고정, `offsetX = 0` |
| top | bottom 고정, `offsetY = newHeight - oldHeight` |
| bottom | top 고정, `offsetY = 0` |
| corner | X/Y 규칙 조합 |

- shrink는 새 client rect로 crop한다.
- expansion으로 새로 드러난 영역은 root background만 보인다.
- old front를 새 root 전체로 stretch하지 않는다.
- exact commit에서 surface/size/offset을 함께 바꾸고 offset을 0으로 되돌린다.

이 정책은 실패했던 raw child pre-present ordering과 다르다. HWND geometry를 먼저/나중에 바꾸는 것이 아니라, **같은 compositor tree 안에서 old front의 위치와 clip을 보존하고 exact surface와 함께 교체**한다.

### 3.4 scheduling 변경

MAUI 유지형 경로에서는 synchronous `WM_SIZE` 100ms wait를 제거하는 쪽이 맞다.

- `WM_SIZE`/DPI handler는 immutable epoch와 sizing edge만 게시하고 즉시 반환한다.
- framework/raster queue는 기존 `current + latest` bound를 유지한다.
- exact surface가 준비된 시점에 latest gate를 통과한 한 transaction만 compositor commit한다.
- WinUI UI dispatcher가 막힌 상태에서 XAML layout/compositor callback이 진행될 것이라는 전제를 두지 않는다.
- `DwmFlush`는 우선 제거한 상태와 commit 뒤 resize-only 상태를 A/B하되, 둘 다 동일 strict WGC gate로만 판단한다.

Flutter식 synchronous backpressure는 pure Win32 fallback에서 별도 compositor/task-runner가 실제로 진행 가능할 때만 다시 검토한다.

### 3.5 예상 변경 경계

- `DorotiWindowsDxgiHost`
  - `SwapChainPanel Presenter`를 제거하고 composition placement/root visual 수명만 소유한다.
- `DorotiWindowsDxgiSurface`
  - raw render child HWND를 presentation/input owner로 만들지 않는다.
  - top-level HWND subclass는 size/DPI/sizing-edge 관찰용으로만 유지한다.
  - XAML host의 pointer/key/focus 경로를 다시 기본값으로 사용한다.
- 새 `WindowsCompositionSurfacePresenter`
  - compositor, front/staging surface, D3D11On12 bridge, fence, exact visible commit을 소유한다.
- `WindowsD3D12BackingStore`
  - 현재 exact Skia raster target을 재사용한다.
- `DorotiMauiSurface`
  - hidden `Entry/Editor`와 semantics overlay는 A 단계에서 유지한다.
- package/interops
  - `Vortice.Direct3D11` 또는 좁은 native interop bridge가 추가로 필요할 수 있다. 실제 package/API projection은 P0에서 먼저 확인한다.

## 4. 제안 B — pure Win32 top-level host fallback

제안 A가 G1을 통과하지 못하면 WinUI top-level compositor가 여전히 독립 size owner라는 뜻으로 보고, Windows runner를 MAUI에서 분리한다.

```text
DorotiWin32Application
  -> WS_OVERLAPPEDWINDOW top-level HWND
  -> one WndProc owns size/DPI/input/lifecycle
  -> ICompositorDesktopInterop.CreateDesktopWindowTarget(topLevelHwnd)
  -> same WindowsCompositionSurfacePresenter
  -> Doroti common framework/runtime
```

이 경로의 중요한 차이는 render child HWND가 없는 것이다. system non-client chrome과 Doroti client content는 top-level HWND 하나를 기준으로 하고, 그 client에 desktop composition target 하나만 연결한다.

제품화할 때는 다음 경계를 만든다.

- 새 `Doroti.Host.Win32`
  - window class, message loop, `WM_SIZE/WM_SIZING/WM_DPICHANGED`, pointer/touch/pen, focus, clipboard, shutdown 소유
- 새 `Doroti.Target.Windows.Win32.win-x64`
  - Windows runner target manifest와 package identity 소유
- common host 추출
  - 현재 `MauiFrameworkHost`의 Doroti view/session/render 연결을 UI toolkit 독립 service로 이동
- IME/accessibility
  - hidden MAUI `Entry/Editor`에 의존하지 않고 TSF/IMM32 bridge와 UIA fragment root를 명시적으로 구현
- template/validator
  - Windows target identity를 `Maui/WinUI-Xaml`에서 실제 `Win32/Composition` backend로 바꿈

제안 B는 단순한 presenter 교체가 아니라 Windows shell migration이므로, 제안 A보다 먼저 제품 코드에 넣지 않는다. 다만 Microsoft의 최신 composition native interop 공식 sample 자체가 `WS_OVERLAPPEDWINDOW → CreateDesktopWindowTarget → CompositionDrawingSurface` 구조를 사용하므로 API 방향은 유효하다.

## 5. 실행 순서와 hard gate

### P0 — API feasibility, 제품 연결 금지

별도 작은 executable 또는 opt-in symbol 아래에서 다음만 증명한다.

- resolved Windows App SDK 1.8에서 C# 또는 좁은 native bridge로 compositor/surface API 생성 성공
- `CompositionDrawingSurface.BeginDraw`가 `ID3D11Texture2D`와 valid offset을 반환
- D3D11On12 unwrap → D3D12 GPU copy → fence → return → `EndDraw`가 반복 성공
- visible front를 resize하지 않고 staging surface 교체 가능
- CPU readback/GDI/bitmap encode 0
- device removal 시 checked-out resource와 open draw transaction이 남지 않음

실패하면 P1로 진행하지 않는다. 이 단계 성공은 화면 continuity PASS가 아니다.

### P1 — MAUI composition visual strict WGC

현재 G1과 같은 200% DPI/165Hz left drag로 420/640/1000 logical matrix를 실행한다.

- current D3D12 raw-child baseline과 Flutter 고정 baseline을 같은 장비에서 비교한다.
- blank, title/caption, AppBar height, circle aspect, right edge, final gap, capture error를 모두 유지한다.
- border-content phase와 지속 시간, target 수, exact commit 수, compositor commit latency를 기록한다.
- `RequestCommitAsync` completion과 WGC-visible 변화 시간을 분리한다.
- `work.md`의 기존 G1 요구값을 그대로 적용한다.

P1 실패 시 timer/timeout/sample 수를 조정하지 않고 제안 A를 철회하고 P2로 간다.

### P2 — pure Win32 ownership A/B

같은 renderer/scene에서 두 control arm만 비교한다.

1. top-level HWND 자체에 existing `CreateSwapChainForHwnd` presenter를 연결한 최소 control
2. top-level HWND에 `CreateDesktopWindowTarget + CompositionDrawingSurface` presenter를 연결한 후보

둘 중 strict G1을 통과한 더 단순한 경로만 제품 후보로 남긴다. 둘 다 실패하면 host ownership 가설만으로 해결되지 않은 것이므로 W1/W2 migration을 시작하지 않는다.

### P3 — 제품 통합

P1 또는 P2가 통과한 뒤에만 진행한다.

- input/IME/semantics/accessibility/lifecycle을 선택된 visible owner에 맞춘다.
- 7방향, DPI/monitor, minimize/restore, device loss를 실행한다.
- 기존 raw child, inactive ANGLE spike, `SwapChainPanel` dual exact path를 삭제한다.
- Windows G2가 끝나기 전에는 Web milestone을 시작하지 않는다.

## 6. 중단 조건

- compositor update가 UI dispatcher를 기다리며 synchronous `WM_SIZE`와 deadlock/reentrancy를 만든다.
- visible front를 exact staging 준비 전에 resize해야만 API가 동작한다.
- D3D11On12 resource를 `EndDraw` 전에 안전하게 반환할 수 없거나 GPU fence ownership이 불명확하다.
- composition commit trace는 성공하지만 WGC에서 exact surface가 보이지 않는다.
- small-window right gap 또는 2-refresh 초과 phase가 기존 raw child baseline보다 안정적으로 개선되지 않는다.
- input/IME/accessibility를 XAML과 ContentIsland 양쪽이 동시에 소유해야만 동작한다.

위 조건 중 하나가 발생하면 해당 후보를 실패로 기록하고 다음 ownership 후보로 이동한다. `DwmFlush`, swap interval, frame-latency wait, 100ms timeout을 반복 조정해 실패를 숨기지 않는다.

## 7. 하지 않을 것

- raw `STATIC`/`CS_OWNDC` child HWND에 다른 ANGLE DLL을 다시 붙이는 실험
- `eglSwapBuffers` 성공을 visible ownership 성공으로 간주
- current visible surface의 선행 `ResizeBuffers`/`Resize`
- `SwapChainPanel.SetSwapChain`을 매 exact frame마다 교체
- capacity buffer + `SetSourceSize`를 제품 경로로 복귀
- previous frame의 full-client stretch, non-uniform scale, CSS/XAML mask
- CPU `readPixels`, GDI copy, PNG round-trip
- `RequestCommitAsync`, `Present`, `DwmFlush`, `WaitForCommitCompletion`을 scan-out ACK라고 기록
- P1 이전 W1 제품 승격, P3 이전 구 경로 삭제

## 8. 최종 추천

가장 작은 다음 행동은 **제안 A의 P0/P1 spike**다. 이유는 다음과 같다.

- 현재 Windows App SDK 1.8에 필요한 Content/Composition API가 이미 들어 있다.
- 현재 D3D12/Skia renderer와 exact backing store를 버리지 않는다.
- 실패한 raw child HWND visible ownership만 제거한다.
- `BeginDraw/EndDraw + compositor commit`은 exact staging이 준비되기 전 old front를 유지한다는 API contract를 제공한다.
- 실패해도 같은 presenter를 pure Win32 top-level host로 옮길 수 있어 실험 코드가 버려지지 않는다.

따라서 `work.md`의 다음 milestone은 기존 W0 재시도가 아니라 아래처럼 새로 정의하는 것이 적절하다.

```text
C0: CompositionDrawingSurface + D3D11On12 feasibility
C1: MAUI/WinUI composition visual strict WGC
  PASS -> C2: product input/lifecycle integration
  FAIL -> N0: pure Win32 top-level ownership A/B
N0 PASS -> N1: Windows runner shell migration
앞 단계 FAIL -> 이후 milestone notStarted
```

## 9. 공식 문서와 reference

### Microsoft 공식 문서

- [ContentIsland](https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island)
- [Composition native interoperation with DirectX and Direct2D](https://learn.microsoft.com/en-us/windows/apps/develop/composition/composition-native-interop)
- [CompositionGraphicsDevice.CreateDrawingSurface2](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositiongraphicsdevice.createdrawingsurface2?view=windows-app-sdk-1.8)
- [Compositor.RequestCommitAsync](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositor.requestcommitasync?view=windows-app-sdk-1.8)
- [ICompositionDrawingSurfaceInterop.BeginDraw](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/win32/microsoft.ui.composition.interop/nf-microsoft-ui-composition-interop-icompositiondrawingsurfaceinterop-begindraw)
- [ID3D11On12Device2.UnwrapUnderlyingResource](https://learn.microsoft.com/en-us/windows/win32/api/d3d11on12/nf-d3d11on12-id3d11on12device2-unwrapunderlyingresource)
- [ID3D11On12Device2.ReturnUnderlyingResource](https://learn.microsoft.com/en-us/windows/win32/api/d3d11on12/nf-d3d11on12-id3d11on12device2-returnunderlyingresource)
- [CreateSwapChainForComposition](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition)
- [DXGI flip model](https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-flip-model)
- [IDXGISwapChain2.SetSourceSize](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setsourcesize)
- [IDXGISwapChain.Present](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-present)
- [DwmFlush](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)
- [IDCompositionDevice.CreateTargetForHwnd](https://learn.microsoft.com/en-us/windows/win32/api/dcomp/nf-dcomp-idcompositiondevice-createtargetforhwnd)
- [IDCompositionDevice.WaitForCommitCompletion](https://learn.microsoft.com/en-us/windows/win32/api/dcomp/nf-dcomp-idcompositiondevice-waitforcommitcompletion)

### 로컬 source reference

- Flutter resize protocol: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc`
- Flutter native view HWND: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_window.cc`
- Flutter runner parent/child ownership: `reference/flutter-master/dev/integration_tests/windows_startup_test/windows/runner/win32_window.cpp`
- Avalonia Windows.UI.Composition target/surface: `reference/Avalonia-main/src/Windows/Avalonia.Win32/WinRT/Composition/`
- Avalonia DirectComposition fallback: `reference/Avalonia-main/src/Windows/Avalonia.Win32/DComposition/`

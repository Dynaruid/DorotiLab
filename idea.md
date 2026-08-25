# Flutter식 Windows resize를 App SDK + SkiaSharp로 이식하는 방안

작성일: 2026-08-25
상태: 설계 조사. 아직 구현 및 물리 화면 검증을 통과한 결론이 아니다.

## 결론

구성할 수 있다. 다만 **Flutter의 resize 동작을 최대한 그대로 복제하려면 렌더 출력의 주체는 ContentIsland가 아니라 앱이 소유한 전용 child HWND여야 한다.** 권장 조합은 다음과 같다.

- top-level: 표준 `WS_OVERLAPPEDWINDOW` + Windows App SDK `AppWindow`
- render target: 앱이 직접 만든 `WS_CHILD | WS_VISIBLE` child HWND 1개
- renderer: SkiaSharp Direct3D(D3D12) + exact-size offscreen render target
- presenter: `CreateSwapChainForHwnd`, exact-size back buffers, `Present`, resize 때 `ResizeBuffers`
- synchronization: Flutter와 같은 `WM_SIZE -> metrics -> exact frame -> exact surface -> Present -> DwmFlush` handshake
- 입력·IME·접근성: 렌더 surface 수명과 분리하여 Win32, WinRT, Windows App SDK 또는 필요한 WinUI 3 island로 연결
- 선택적 C++: child WndProc와 Flutter식 message-only task pump만 작은 native bridge로 격리

이 구성은 Flutter의 **child HWND topology**, **100 ms bounded wait**, **틀린 크기 frame 거부**, **raster/presentation thread의 exact surface 교체**, **present 후 `DwmFlush`**를 거의 일대일로 옮길 수 있다. EGL을 그대로 쓸 필요는 없다. Flutter의 `EGL_FIXED_SIZE_ANGLE`이 제공하는 의미를 D3D12에서는 “오래된 back-buffer 참조를 모두 해제하고, exact 크기로 `ResizeBuffers`한 뒤, exact frame만 present한다”로 번역하면 된다.

반면 ContentIsland + composition swap chain도 구현 가능하지만, 이것은 동일한 resize 상태 기계를 복제하는 **WinRT Composition 변형**이지 Flutter의 출력 topology를 복제하는 것은 아니다. `CreateSwapChainForComposition`은 API 계약상 `DXGI_SCALING_STRETCH`가 필요하고, visual-tree commit이라는 추가 단계도 생긴다. 따라서 창 resize 문제를 Flutter와 같은 조건에서 비교하려는 첫 실험으로는 부적합하다.

가장 중요한 한계도 그대로 남는다. 이 handshake는 오래된 client surface가 새 child 크기에 늘어나 보이는 현상을 줄이는 장치이지, top-level USER32/DWM non-client geometry와 child swap chain scan-out을 하나의 원자적 transaction으로 만드는 장치는 아니다. 따라서 이 설계가 left/top resize jitter까지 해결했다고 간주하려면 같은 PC의 실제 화면에서 별도 검증해야 한다.

### 확인한 버전 범위

현재 저장소의 관련 spike는 Windows App SDK `2.4.0`을 고정하고 있고, 중앙 패키지는 SkiaSharp/`SkiaSharp.Direct3D.Vortice` `4.151.1`, Vortice `3.8.3`을 사용한다.

- [`Doroti.Validation.WinRtContentIslandSpike.csproj:16`](Doroti/validation/winrt-content-island-spike/Doroti.Validation.WinRtContentIslandSpike.csproj#L16)
- [`Doroti.Validation.WindowsAppSdk24Preflight.csproj:16`](Doroti/validation/windowsappsdk-24-preflight/Doroti.Validation.WindowsAppSdk24Preflight.csproj#L16)
- [`Directory.Packages.props:8`](Doroti/Directory.Packages.props#L8)

아래 판단은 이 버전 조합과 로컬 Flutter checkout에 한정한다. 로컬 source/API 존재와 기존 spike는 구성 가능성의 근거지만, 새 권장 조합의 runtime/visible 성공 증거는 아니다.

## Flutter에서 실제로 복제해야 할 것

검토 기준은 로컬에 고정된 Flutter checkout `56b8e1a851a594b1a154f8ea93270807dab22b9a`이다.

| 단계 | Flutter 구현 | 이식해야 할 의미 |
|---|---|---|
| window topology | runner의 top-level은 `WS_OVERLAPPEDWINDOW`, engine view는 `WS_CHILD | WS_VISIBLE` | top-level과 render child를 분리한다. |
| resize 시작 | [`flutter_window.cc:599`](reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_window.cc#L599)의 child `WM_SIZE` | child client size의 **physical pixel**을 resize target으로 삼는다. |
| metrics와 대기 | [`flutter_windows_view.cc:208`](reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc#L208) | target을 기록하고 metrics를 보낸 뒤 platform thread가 최대 100 ms 기다린다. |
| 제한된 message pump | [`task_runner_window.cc:164`](reference/flutter-master/engine/src/flutter/shell/platform/windows/task_runner_window.cc#L164) | 일반 window queue 전체가 아니라 message-only task HWND의 메시지만 처리하여 `WM_SIZE` 재진입을 막는다. |
| exact-frame admission | [`flutter_windows_view.cc:165`](reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc#L165) | pending target과 width/height가 정확히 일치하는 frame만 resize frame으로 인정한다. |
| exact surface 교체 | [`flutter_windows_view.cc:822`](reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc#L822) | raster thread에서 기존 surface를 폐기하고 target 크기의 surface를 만든다. |
| 자동 추종 금지 | [`egl/manager.cc:283`](reference/flutter-master/engine/src/flutter/shell/platform/windows/egl/manager.cc#L283) | `EGL_FIXED_SIZE_ANGLE`처럼 presentation surface 크기를 앱이 명시적으로 소유한다. |
| present 완료 | [`flutter_windows_view.cc:747`](reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc#L747) | exact frame을 present한 뒤 platform wait를 깨우고 raster thread에서 `DwmFlush`한다. |

Flutter는 large backing을 계속 유지하면서 clip만 바꾸지 않는다. resize가 시작되면 새 metrics에 맞는 frame을 기다리고, 그 frame이 도착했을 때 EGL window surface 자체를 새 exact 크기로 파괴·재생성한다.

그대로 복사하지 말아야 할 세부도 있다.

- Flutter의 admission key는 사실상 width/height와 resize state다. Doroti에서는 동일 크기가 반복되는 경우와 stale completion을 구분하기 위해 `generation`을 추가하는 편이 안전하다.
- 100 ms timeout은 실패를 숨기는 수단이 아니다. WndProc를 무한히 막지 않기 위한 상한이다. timeout 뒤 늦게 완료된 frame도 generation을 확인하여 terminal 상태로 정산해야 한다.
- `DwmFlush`는 호출 앱이 queue한 DirectX 변경을 DWM이 처리할 때까지 기다리는 API다. 실제 모니터 scan-out이나 외곽 프레임과 child content의 원자성을 증명하지 않는다.

## 선택지 비교

| 항목 | A. 전용 child HWND + HWND swap chain | B. ContentIsland + composition swap chain |
|---|---|---|
| Flutter topology 유사성 | 높음 | 중간 |
| presentation surface | child HWND에 직접 연결 | Composition surface/visual에 연결 |
| DXGI 생성 | `CreateSwapChainForHwnd` | `CreateSwapChainForComposition` |
| scaling 계약 | `DXGI_SCALING_NONE` 사용 가능 | API상 `DXGI_SCALING_STRETCH` 필수 |
| 1:1 pixel 보장 방법 | exact buffer와 child client size | exact buffer + `CompositionSurfaceBrush.Stretch=None` + exact visual size |
| resize | release/fence 후 `ResizeBuffers` | release/fence 후 `ResizeBuffers`; surface/visual 변경 시 commit도 관리 |
| 동기화 완료점 | successful `Present` | `Present`와 composition commit을 분리해서 추적 |
| 입력/접근성 | child/top-level HWND 또는 별도 island | ContentIsland 기능을 직접 이용 가능 |
| 첫 구현 권장도 | **권장** | 비교군으로 유지 |

### A를 권장하는 이유

이 실험의 독립 변수는 resize/presentation 전략이어야 한다. ContentIsland를 주 presentation 경로로 쓰면 Flutter에는 없는 site bridge, composition surface binding, visual commit, dispatcher affinity가 동시에 들어온다. 결과가 좋아도 나빠도 원인을 Flutter handshake에서 분리하기 어려워진다.

전용 child HWND를 사용하면 현재 로컬 코드에서 이미 확인된 다음 재료를 그대로 조합할 수 있다.

- [`DorotiWindowsDxgiSurface.cs:2458`](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs#L2458): `CreateSwapChainForHwnd`
- [`DorotiWindowsDxgiSurface.cs:2523`](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs#L2523): exact-size `ResizeBuffers`
- [`DorotiWindowsDxgiSurface.cs:2611`](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs#L2611): SkiaSharp `GRContext.CreateDirect3D`
- [`DorotiWindowsDxgiSurface.cs:2580`](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs#L2580): present 뒤 `DwmFlush`
- [`windows-composition-surface/Program.cs:173`](Doroti/validation/windows-composition-surface/Program.cs#L173): 독립 spike의 SkiaSharp D3D12 context 생성

이것들은 구현 가능성을 보여 주는 로컬 증거일 뿐, 지금 요청에서 해당 제품 코드를 재사용하거나 이미 화면 문제가 해결되었다는 뜻은 아니다.

### B를 사용할 수 있는 범위

ContentIsland는 non-XAML Composition/Direct3D content와 입력·레이아웃·접근성 경계를 호스팅할 수 있다. 로컬 spike도 다음을 이미 수행한다.

- [`winrt-content-island-spike/Program.cs:234`](Doroti/validation/winrt-content-island-spike/Program.cs#L234): brush `Stretch=None`
- [`winrt-content-island-spike/Program.cs:243`](Doroti/validation/winrt-content-island-spike/Program.cs#L243): `ContentIsland.CreateForSystemVisual`
- [`winrt-content-island-spike/Program.cs:1039`](Doroti/validation/winrt-content-island-spike/Program.cs#L1039): exact-size `ResizeBuffers`
- [`winrt-content-island-spike/Program.cs:1082`](Doroti/validation/winrt-content-island-spike/Program.cs#L1082): `CreateSwapChainForComposition`

따라서 B에서도 exact metrics/frame admission은 구현할 수 있다. 다만 다음 차이를 명시해야 한다.

1. `CreateSwapChainForComposition`은 flip model과 `DXGI_SCALING_STRETCH`를 요구한다. `Scaling=None`으로 바꾸는 경로가 아니다.
2. 실제 stretch를 막는 것은 scaling enum이 아니라 buffer, brush, visual의 크기를 항상 같은 physical pixel target으로 맞추는 계약이다.
3. Composition drawing surface의 `EndDraw`나 swap-chain `Present`만으로 visual tree가 화면에 반영되었다고 볼 수 없다. visual 변경에는 commit 경계가 추가된다.
4. `RequestCommitAsync` 성공은 compositor가 변경을 받아들였다는 자동화 증거이지, 사용자가 본 scan-out 증거가 아니다.

## 권장 구조

```text
UI / platform thread
  standard top-level HWND + AppWindow
      |
      +-- app-owned WS_CHILD render HWND
      |       |
      |       +-- WM_SIZE(physical px)
      |       +-- optional native ResizeBridge
      |
      +-- input / IME / accessibility integration
              Win32, WinRT, App SDK, or bounded WinUI 3 island

framework thread (MTA)
  metrics(generation, width, height, scale)
      -> layout/build exact scene

raster/presentation thread
  exact frame admission
      -> SkiaSharp D3D12 exact backing resource
      -> GPU fence / release old back-buffer wrappers
      -> exact ResizeBuffers
      -> copy/transition into current HWND back buffer
      -> Present
      -> complete generation
      -> wake task HWND
      -> DwmFlush
```

### 소유권 규칙

- platform thread만 top-level/child HWND와 resize state의 시작을 소유한다.
- framework thread는 metrics를 소비하고 frame을 만들 뿐 HWND나 DXGI surface를 바꾸지 않는다.
- raster/presentation thread만 Skia `GRContext`, D3D12 command queue, swap chain, back buffers와 GPU fence를 소유한다.
- 동일 COM/GPU 자원의 수명을 C++과 C# 양쪽이 나누어 소유하지 않는다.
- App SDK dispatcher가 필요한 island를 사용하더라도 platform thread의 100 ms wait 중 그 island의 임의 callback을 재진입시키지 않는다.

마지막 규칙 때문에 Flutter처럼 별도 framework/raster thread가 필요하다. layout/build가 `WM_SIZE`를 처리하는 동일 thread에 있으면 platform wait와 frame 생성이 서로를 기다리는 deadlock이 된다.

## resize protocol

권장 상태는 최소한 다음과 같다.

```text
Idle
  -> WaitingForExactFrame(generation, target)
  -> SurfacePreparing(generation, target)
  -> Presenting(generation, target)
  -> Presented(generation, target)

별도 관측 플래그:
  platformWaitTimedOut = true | false
```

timeout을 terminal frame failure와 같은 상태로 만들지 않는 이유는 100 ms가 지난 뒤 raster thread가 정상 present를 끝낼 수 있기 때문이다. 다만 새 generation이 시작되면 이전 generation의 frame은 surface 변경 권한을 잃는다.

### 순서

1. child `WM_SIZE`에서 `GetClientRect`로 physical width/height를 다시 읽는다.
2. 0 크기, 기존 surface와 같은 크기, 최소화 상태는 metrics만 정산하고 blocking resize를 시작하지 않는다.
3. `generation++`, target width/height, DPI scale을 원자적으로 게시한다.
4. framework thread에 metrics를 전달한다.
5. platform thread는 최대 100 ms 동안 message-only task HWND만 poll한다.
6. framework/raster 경로가 만든 frame은 `(generation, width, height)`가 현재 target과 모두 같을 때만 admission된다.
7. Skia가 exact-size backing resource에 그린 뒤 `Flush/Submit`하고 필요한 GPU fence를 기다린다.
8. 이전 back-buffer를 가리키는 `SKSurface`, `GRBackendRenderTarget`, RTV와 기타 direct/indirect reference를 모두 해제한다.
9. HWND swap chain을 exact target으로 `ResizeBuffers`하고 모든 back-buffer wrapper를 다시 얻는다.
10. exact backing을 current back buffer로 copy/transition한 뒤 `Present`한다.
11. successful present만 해당 generation을 `Presented`로 만들고 task HWND에 wake message를 보낸다.
12. raster/presentation thread가 `DwmFlush`한다.
13. 첫 exact present 전까지 top-level을 숨기는 정책을 쓴다면 이 시점에만 show한다.

여기서 backing resource를 큰 크기로 유지하고 clip만 바꾸면 Flutter 복제가 아니다. 성능상 backing allocation을 pool로 재사용할 수는 있지만, **admission되는 frame의 logical/physical viewport와 presentation buffer는 exact target이어야 한다.**

### stale, timeout, 실패 처리

- 현재 generation보다 오래된 frame은 draw 결과가 있어도 present하지 않는다.
- 같은 width/height가 반복되어도 generation이 다르면 별개의 요청으로 정산한다.
- 100 ms가 지나면 WndProc는 반환한다. 대기 초과를 성공으로 기록하지 않는다.
- timeout 후 exact frame이 도착하면 현재 generation인 경우에만 계속 present할 수 있다.
- `ResizeBuffers` 실패 시 기존 valid front를 파괴한 것으로 위장하지 말고 오류를 기록한다. GPU/device-loss 복구 경로를 별도로 둔다.
- `ResizeBuffers` 전에 오래된 buffer reference가 하나라도 남아 있으면 실패할 수 있으므로 wrapper dispose 순서를 테스트 계약으로 고정한다.
- resize 중 새 target이 오면 current + latest만 유지하여 unbounded frame queue를 만들지 않는다.

## 선택적 C++ bridge

C++는 필수는 아니다. Vortice + SkiaSharp로 swap chain과 GPU 자원을 C#에서 소유하는 것이 이미 가능하다. 우선 managed presenter로 구현하고, 다음 두 부분에서만 native code가 실제 이점을 줄 때 bridge를 넣는 것이 좋다.

- child WndProc의 수명과 callback 안정성
- Flutter와 같은 message-only HWND + filtered `GetMessage(hwnd, ...)` pump

GPU presentation까지 C++로 옮기면 SkiaSharp/Vortice와 native DXGI가 같은 COM object의 수명을 나누어 갖기 쉽다. 그렇게 할 경우에는 bridge가 D3D12 device, queue, swap chain, fence, back buffers를 전부 소유해야 하며 중간 소유권은 허용하지 않는 편이 안전하다.

### 최소 bridge 경계 예시

아래는 구현안의 모양이며 아직 확정 ABI가 아니다.

```cpp
struct DorotiResizeMetrics {
  uint64_t generation;
  uint32_t width_px;
  uint32_t height_px;
  double scale;
};

using DorotiMetricsCallback = void(__stdcall*)(
    const DorotiResizeMetrics* metrics,
    void* context);

extern "C" {
__declspec(dllexport) void* DorotiCreateResizeBridge(
    HWND parent,
    DorotiMetricsCallback callback,
    void* context);

__declspec(dllexport) HWND DorotiGetRenderHwnd(void* bridge);

// message-only task HWND만 pump한다. 일반 HWND message를 재진입시키지 않는다.
__declspec(dllexport) bool DorotiPollResizeTask(
    void* bridge,
    uint64_t generation,
    uint32_t timeout_ms);

// C# raster/presentation owner가 successful Present 뒤 호출한다.
__declspec(dllexport) bool DorotiCompleteExactPresent(
    void* bridge,
    uint64_t generation,
    uint32_t width_px,
    uint32_t height_px);

__declspec(dllexport) void DorotiDestroyResizeBridge(void* bridge);
}
```

이 경계에서는 C++가 HWND와 wait/wake state만 소유하고, C#의 `FlutterHwndExactPresenter`가 D3D12/SkiaSharp를 전부 소유한다. callback은 `WM_SIZE` 안에서 framework 작업을 직접 실행하지 않고 immutable metrics를 queue에 넣기만 해야 한다.

## 입력·IME·접근성 연결 원칙

입력·IME·접근성에 App SDK나 WinUI 3를 사용하는 것은 이 설계와 충돌하지 않는다. 충돌하는 것은 그 계층이 resize target과 render surface의 최종 소유자가 되는 경우다.

- pointer/keyboard/focus는 top-level 또는 render child HWND에서 받아 framework input queue로 전달할 수 있다.
- IME는 IMM32/TSF, WinRT input API, 또는 제한된 WinUI 3 island 중 검증된 경로를 선택할 수 있다.
- UIA provider는 top-level `WM_GETOBJECT`와 연결하거나 App SDK의 island accessibility 경계를 이용할 수 있다.
- WinUI 3 controls가 실제 화면에 필요하면 별도 sibling region/island로 두고 z-order, focus, DPI, clipping을 독립 검증한다.
- 투명 island를 render child 위에 전면 배치하여 모든 input을 대신 받는 구성은 hit testing, occlusion, focus가 새 변수가 되므로 최초 Flutter A/B 실험에는 넣지 않는다.

즉 **render/present ownership만 Flutter식 child HWND 경로에 고정**하고, 입력 계층은 교체 가능한 adapter로 남긴다.

## 구현·검증 순서

### FRC-0 — source contract

- 위 Flutter checkout/line mapping을 validator에 고정한다.
- state transition, 100 ms bound, exact admission, stale rejection을 단위 테스트한다.
- 산출물은 source/자동화 증거이며 visible proof가 아니다.

### FRC-1 — 최소 HWND presenter

- 표준 top-level + app-owned render child만 만든다.
- App SDK input/WinUI 3를 아직 붙이지 않는다.
- SkiaSharp D3D12 exact backing + `CreateSwapChainForHwnd` + exact `ResizeBuffers` + `Present`를 구현한다.
- 첫 exact present 전 show 금지와 device-loss cleanup을 검증한다.

### FRC-2 — Flutter handshake

- message-only task HWND, filtered poll, 100 ms timeout을 넣는다.
- resize 중 동일 크기 반복, A→B→A, frame/present 사이 새 resize, timeout 뒤 late completion을 테스트한다.
- 모든 generation이 `presented`, `superseded`, `failed` 중 하나로 정산되는지 확인한다.

### FRC-3 — input 계층 graft

- App SDK/WinRT/WinUI 3 입력·IME·UIA 연결을 한 기능씩 추가한다.
- 각 단계에서 render HWND, exact target, presenter thread ownership이 바뀌지 않았음을 계약으로 검사한다.
- ContentIsland를 사용한다면 renderer가 아니라 integration boundary로만 시작한다.

### FRC-4 — 물리 visible A/B hard gate

같은 PC, 모니터, DPI, refresh rate에서 다음 세 실행물을 비교한다.

1. pinned Flutter Windows sample
2. `FlutterHwndExact` spike
3. bare native HWND/DXGI control

각 실행물에서 left/right/top/bottom/corner를 slow/medium/fast/reverse로 반복한다. WGC/Desktop Duplication capture와 수동 육안 판정을 분리해 기록하고, 다음을 각각 세어야 한다.

- stale stretch
- white/transparent band
- child가 outer frame을 따라가지 못한 frame
- left/top origin-moving jitter
- timeout과 superseded generation
- present 실패/device loss

자동 capture가 깨끗해도 물리 모니터에서 보이는 현상을 통과한 것으로 기록하지 않는다. 반대로 수동 PASS도 테스트한 edge/speed/DPI 범위를 넘어 일반화하지 않는다.

**FRC-4가 통과하기 전에는 이 아이디어를 제품 host의 resize 해결책으로 승격하지 않는다.** 먼저 독립 spike로 기존 ContentIsland 경로와 나란히 비교해야 한다.

## 예상 결과와 판단 기준

이 설계로 직접 줄일 수 있는 대상은 다음과 같다.

- 새 child 크기에 이전 크기의 swap-chain image가 stretch되는 시간
- frame viewport와 presentation buffer 크기가 어긋나는 frame
- resize 도중 stale frame이 surface를 되돌리는 경쟁
- `ResizeBuffers`와 Skia back-buffer wrapper 수명 충돌

이 설계만으로 보장할 수 없는 대상은 다음과 같다.

- USER32가 interactive sizing 중 top-level 외곽을 갱신하는 cadence
- DWM의 실제 scan-out 시점
- top-level non-client frame과 child swap chain의 원자적 commit
- WinUI 3/ContentIsland를 추가했을 때의 focus, IME, UIA 품질

따라서 최종 판단은 “구성이 가능한가?”에는 **예**, “Flutter처럼 하면 현재 창 resize 문제가 해결되는가?”에는 **독립 spike와 물리 FRC-4 전에는 미검증**이다.

## 공식 자료

- [Windows App SDK ContentIsland overview](https://learn.microsoft.com/en-us/windows/apps/develop/composition/content-island)
- [Windows App SDK Islands samples](https://github.com/microsoft/WindowsAppSDK-Samples/tree/main/Samples/Islands)
- [Composition native interoperation](https://learn.microsoft.com/en-us/windows/apps/develop/composition/composition-native-interop)
- [`IDXGIFactory2::CreateSwapChainForComposition`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition)
- [`IDXGISwapChain::ResizeBuffers`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizebuffers)
- [`ICompositorInterop::CreateCompositionSurfaceForSwapChain`](https://learn.microsoft.com/en-us/windows/win32/api/windows.ui.composition.interop/nf-windows-ui-composition-interop-icompositorinterop-createcompositionsurfaceforswapchain)
- [`DwmFlush`](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)
- [SkiaSharp `GRContext.CreateDirect3D` API](https://github.com/mono/SkiaSharp-API-docs/blob/main/SkiaSharpAPI/SkiaSharp/GRContext.xml)

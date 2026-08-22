# Windows / Web interactive resize 문제 원인 분석

- 분석일: 2026-08-22
- 범위: 제공된 Windows/Chrome 녹화와 캡처, 현재 `Doroti.Host.Maui`/`Doroti.Host.Web` resize·scene·present 로직
- 결론: **Windows와 Web의 현상은 같은 버그가 아니다.** Windows는 최신 surface target과 그 surface에 그리는 framework layout metrics가 같은 세대인지 보증하지 못하는 문제가 핵심이고, 그 위에 exact-size buffer 재생성 비용이 추종 지연을 더한다. Web은 exact frame이 준비될 때까지 이전 GPU frame을 새 크기로 의도적으로 확대하는 정책이 찌그러짐의 직접 원인이다. 녹화에 남은 Web의 짧은 blank는 `preserveDrawingBuffer=false`인 default framebuffer를 연속해서 다시 보장하지 못하는 별도 continuity 문제다.

## 1. 검토한 증거

### Windows

- `화면 녹화 중 2026-08-22 082720.mp4`
  - 1576×1006, 30 fps, 344 frames, 약 11.47초
- `videoframe_3392.png`
- `videoframe_3518.png`
- `videoframe_3961.png`
- `videoframe_4013.png`

### Chrome

- `화면 녹화 중 2026-08-22 083221.mp4`
  - 1880×1374, 30 fps, 141 frames, 약 4.70초
- `videoframe2_656.png`

### 코드와 이전 계측

- Windows target/surface: [`DorotiWindowsDxgiSurface.cs`](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs)
- Windows framework metrics/frame dispatch: [`MauiHostAdapter.cs`](Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs)
- 공통 scene descriptor와 exact gate: [`ResizeLifecycle.cs`](Doroti/src/Doroti.Ui/ResizeLifecycle.cs), [`SkiaSceneRenderer.cs`](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs)
- Web DOM size/presenter: [`doroti.web.ts`](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)
- Web canvas/managed Skia bridge: [`DorotiWebGlSurface.razor`](Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor)
- 이전 자동 계측과 acceptance 경계: [`windows-web-resize-pipeline-summary.md`](history/26-08-21/windows-web-resize-pipeline-summary.md)

이번 분석에서는 build/test를 다시 실행하지 않았다. 녹화의 화면 상태와 현재 source를 대조한 원인 분석이며, 과거 자동 trace 수치는 현재 영상과 시간 동기화된 새 계측값이 아니라 병목 위치를 설명하는 보조 자료로만 사용한다.

## 2. 화면 증거가 보여 주는 것

### 2.1 Windows는 단순히 “이전 전체 frame”만 보이는 상태가 아니다

`videoframe_3518.png`에서 현재 검은 client/surface는 대략 x=530..1544까지 존재하지만 AppBar의 보라색 영역은 x=531..1444에서 끝난다. 최신 surface 오른쪽 약 100 px가 검은 배경으로 남는다. `videoframe_4013.png`도 AppBar가 x=742..1456에서 끝나고 오른쪽 약 88 px가 남는다.

반면 `videoframe_3392.png`와 `videoframe_3961.png`에서는 AppBar가 당시 client 오른쪽 끝까지 도달한다. 즉 resize 도중 다음 두 상태가 교대로 나타난다.

1. surface 크기와 framework layout 폭이 일치한 frame
2. surface는 최신 크기인데 내부 scene은 이전 폭으로 layout된 frame

만약 이전 back buffer 전체를 compositor가 새 client 크기로 단순 확대만 했다면 AppBar도 buffer의 오른쪽 끝까지 같이 확대되어야 한다. 캡처처럼 검은 surface는 끝까지 있는데 AppBar만 먼저 끝나는 모습은 **새 back buffer에 더 좁은 layout scene을 그린 경우**와 일치한다.

### 2.2 왼쪽 경계 drag에서 더 크게 보이는 이유

왼쪽 경계를 움직일 때 오른쪽 창 경계는 화면상 고정된 기준점으로 남는다. 최신 surface 폭보다 좁은 scene이 왼쪽에 맞춰 그려지면 부족한 폭이 고정된 오른쪽 경계 앞에 검은 띠로 그대로 드러난다. 반대 방향의 mismatch, 즉 scene이 surface보다 넓으면 오른쪽이 잘리므로 빈 띠보다 덜 눈에 띈다.

오른쪽 경계를 움직일 때는 mismatch가 움직이는 경계 쪽에 집중되지만, 왼쪽 경계를 움직이면 창의 screen-space 원점도 함께 이동하고 고정된 오른쪽 끝과 내부 reflow가 동시에 비교된다. 따라서 별도의 left 전용 코드가 없어도 같은 1~3 frame 지연과 폭 불일치가 더 크게 체감된다.

### 2.3 Chrome은 고정 높이 UI까지 함께 확대·축소된다

Chrome 녹화에서 정상 exact frame의 AppBar 높이는 약 111 px로 안정된다. 그러나 resize 중에는 다음처럼 변한다.

- 약 0.63초: 137 px
- 약 0.70초: 143 px
- 약 0.77초: 146 px
- 약 1.03초: 92 px

고정 논리 높이여야 하는 AppBar와 글자, control까지 창의 가로·세로 변화에 맞춰 동시에 커졌다 작아진다. 이는 새로운 크기로 다시 layout한 Skia 결과가 아니라 **이전 bitmap/frame 전체를 새 사각형에 비등방 확대**한 형태다. `videoframe2_656.png`의 두꺼워진 글자와 control도 이 구간에 해당한다.

또한 Chrome 녹화에는 배경만 보이는 구간이 두 번 있다.

- 약 2.00초 부근: 최소 2 frames, 약 0.07초
- 약 3.30초 부근: 약 4 frames, 약 0.13초

따라서 현재 Web 경로는 “찌그러지지만 항상 이전 frame을 유지한다”는 상태에도 도달하지 못했다.

## 3. Windows 근본 원인

### 3.1 surface target과 framework metrics의 출처가 다르다

Windows native target은 `SwapChainPanel`에서 만든다.

- `PublishTarget()`은 `panel.ActualWidth`, `panel.ActualHeight`, `panel.CompositionScaleX`를 읽는다.
- 이 값으로 `DorotiResizeEpoch`와 physical back-buffer 크기를 만든다.
- `Maui.SizeChanged`와 `SwapChainPanel.SizeChanged`가 모두 `PublishTarget()`을 호출한다.

반면 framework layout metrics는 `MauiHostAdapter.HandleSizeChanged()`에서 `_surface.Width`/`_surface.Height`, 즉 MAUI `View.Width`/`Height`를 읽어 만든다. 이 메서드는 `CaptureSnapshot()`에서 최신 panel target도 얻지만, 그 snapshot의 `LogicalWidth`/`LogicalHeight`는 사용하지 않고 density만 사용한다.

따라서 한 layout pass 안에서도 다음 두 값이 서로 다른 시점의 값일 수 있다.

```text
native surface target = SwapChainPanel.ActualWidth/ActualHeight
framework layout size = MAUI View.Width/Height
```

두 `SizeChanged` source의 발생 순서와 XAML/MAUI layout 반영 순서는 하나의 원자적 transaction으로 묶여 있지 않다. 캡처의 오른쪽 검은 띠는 이 두 크기가 일시적으로 갈라진 결과와 일치한다.

### 3.2 scene이 실제로 사용한 metrics 세대가 descriptor에 묶이지 않는다

`SkiaSceneRenderer.Submit()`은 scene이 framework에서 만들어진 뒤 그 시점의 `_host.ResizeTarget`을 다시 읽는다. 그리고 descriptor의 logical/physical size를 **scene을 만들 때 사용한 metrics**가 아니라 이 최신 target에서 복사한다.

즉 다음 race가 가능하다.

```text
metrics A로 framework layout/build 시작
        ↓
panel target은 B로 진행
        ↓
A 크기의 drawing commands가 Submit됨
        ↓
Submit은 현재 target B를 descriptor에 기록
        ↓
B 크기 surface에 A layout commands가 exact frame으로 승인됨
```

`DorotiFrameDescriptor`에는 `MetricsGeneration` 필드가 있지만 `IsExactFor()`는 이를 검사하지 않는다. `LogicalWidth`/`LogicalHeight`도 비교하지 않는다. 현재 exact 판정은 아래 네 값만 본다.

- `ResizeTargetGeneration`
- `PhysicalWidth`
- `PhysicalHeight`
- `DevicePixelRatio`

그 결과 현재 gate는 **surface descriptor가 target과 같은지**는 보증하지만, **scene layout이 그 target의 metrics로 만들어졌는지**는 보증하지 않는다. 자동 trace에서 `exact-size mismatch == 0`이어도 캡처와 같은 내부 layout underfill을 통과시킬 수 있는 이유다.

### 3.3 exact buffer 재생성 파이프라인도 본질적으로 한두 frame 늦다

target을 받은 뒤 visible frame까지 현재 경로는 다음 작업을 요구한다. 구현은 `ResizeBuffers` 준비와 framework build 일부를 겹치지만, 두 결과가 모두 맞아야 raster/present로 진행할 수 있다.

```text
SizeChanged
  ├→ panel target → ResizeBuffers 준비
  └→ MetricsChanged → framework frame/layout/build
                 ↓ 두 결과의 exact 승인
  → Skia raster
  → GPU flush/submit
  → DXGI Present
```

`RasterMain()`은 최신 target이 바뀌면 paint 전, flush 전, present 전 단계에서 이전 작업을 `superseded` 처리한다. 이것은 stale frame 제출을 막는 데 필요하지만, resize 입력이 pipeline보다 빠르면 완성 직전 frame도 버리고 다음 exact frame을 기다리게 한다. 그동안 border는 OS가 즉시 바꾸지만 content는 마지막으로 commit된 frame 또는 잘못 승인된 layout을 유지한다.

이전 200% DPI 자동 trace는 boundary→target 전달 p95가 98 µs였던 반면 target→ACK가 p50 약 16.6 ms, p95 약 23.1 ms였고, surface 준비만 p50 약 8.1 ms였다. 따라서 주된 시간은 `SizeChanged` 감지 누락이 아니라 **target 이후 exact surface/layout/raster/commit**에 있다. 165 Hz에서는 16.6 ms도 약 2.7 refresh intervals이고 23.1 ms는 약 3.8 intervals이므로 빠른 drag에서 경계가 먼저 가는 것이 보인다.

또한 swap chain은 `Scaling.Stretch`로 생성된다. Microsoft 문서상 `DXGI_SCALING_STRETCH`는 back buffer와 presentation target 크기가 다를 때 buffer를 target에 맞춰 확대한다. 따라서 exact frame 사이에 이미 present된 buffer가 합성되는 방식도 일시적인 확대를 만들 수 있다. 다만 제공된 Windows 캡처의 **검은 오른쪽 띠**는 단순 compositor stretch보다 3.1~3.2의 scene/metrics 불일치를 더 직접적으로 가리킨다.

## 4. Web 근본 원인

### 4.1 CSS 크기는 즉시 바뀌지만 target sampling은 rAF까지 미룬다

`.doroti-root > canvas`는 CSS `width: 100%; height: 100%`다. 브라우저 창 크기가 바뀌면 canvas의 화면상 CSS box는 즉시 새 viewport를 채운다.

그러나 `ResizeObserver` callback의 `scheduleHostSample()`은 실제 `getBoundingClientRect()` 측정과 resize epoch 갱신을 `requestAnimationFrame()`까지 미룬다. 이 사이에는 다음 상태가 존재한다.

```text
CSS display size = 새 창 크기
canvas drawing buffer = 이전 physical 크기
front FBO = 이전 exact frame 크기
```

HTML/WebGL의 display size와 drawing-buffer size는 서로 다른 값이므로, 이 구간의 화면은 브라우저가 이전 buffer를 새 CSS box에 맞춰 표시한 결과가 된다.

### 4.2 presenter가 이전 front를 새 크기로 명시적으로 stretch한다

`runPresenter()`는 새 physical size를 받으면 다음 순서로 동작한다.

1. `canvas.width/height`를 새 크기로 설정한다.
2. exact managed raster를 기다리기 전에 이전 `front` FBO를 default framebuffer로 blit한다.
3. 이때 source rect는 이전 `front.width/height`, destination rect는 새 `physicalWidth/height`다.
4. filter는 `gl.NEAREST`다.
5. 이후에야 staging FBO에 managed Skia exact frame을 그리고, 최신 generation이면 front로 commit한다.

즉 이전 frame의 가로세로 비율과 새 viewport 비율이 다르면 전체 UI가 비등방으로 늘어나며, `NEAREST` 때문에 글자와 곡선은 더 거칠고 픽셀화되어 보인다. 이것이 Chrome 녹화에서 AppBar 높이까지 92~146 px로 변하는 직접 원인이다. **Skia가 새 scene을 찌그러뜨려 그리는 것이 아니라, Web presenter가 마지막 정상 Skia frame을 새 크기로 찌그러뜨려 임시 표시한다.**

이 동작은 우발적인 부작용만은 아니다. 삭제 전 작업계획과 현재 history에는 resize 중 `stretched front`를 허용하고 blank만 금지한 정책이 기록되어 있다. 따라서 기존 smoke가 PASS한 것은 현재 사용자 요구인 “resize 중에도 기하가 찌그러지지 않음”을 검증한 결과가 아니다.

### 4.3 exact frame까지 rAF 경계가 중첩된다

Web의 새 exact frame은 최소 다음 예약 경계를 거친다.

```text
ResizeObserver
  → sampleHost rAF: 새 DOM 크기/epoch 관찰
  → managed MetricsChanged
  → framework requestFrame rAF: layout/build/scene submit
  → presenter rAF: backing reset/staging raster/front commit
```

여기에 JS↔WASM/Blazor async callback이 들어간다. 중간에 더 최신 target이 오면 current 또는 latest request가 `superseded`되고 다음 presenter rAF를 기다린다. 따라서 retained front의 찌그러진 상태가 한 frame이 아니라 여러 frames 지속될 수 있다.

### 4.4 blank가 남는 이유

WebGL context는 `preserveDrawingBuffer: 0`으로 생성된다. WebGL 사양상 drawing buffer는 크기가 바뀔 때 clear되며, `preserveDrawingBuffer=false`이면 page compositor에 제시된 뒤에도 기본값으로 clear될 수 있다.

현재 코드는 app-owned `front` FBO 자체는 유지하지만 default framebuffer에는 다음 경우에만 다시 blit한다.

- `ResizeObserver` signal에서 `retained-refresh`
- backing-store reset 직후 `retained-restore`
- exact staging의 `front-commit`

즉 CSS/compositor resize와 default-buffer discard가 위 callback보다 먼저 화면에 반영되거나, 연속 resize 중 signal/rAF/async commit 사이가 벌어지면 root background만 보일 수 있다. 녹화의 약 0.07초/0.13초 blank가 이 continuity hole이 실제 browser presentation에서 아직 남아 있음을 보여 준다.

사양상 FBO를 retained source로 쓰는 선택 자체는 적절하다. 문제는 **retained FBO의 존재**가 아니라, 그것을 default framebuffer에 보이는 크기·시점·필터가 현재 요구와 맞지 않는다는 것이다.

## 5. 기존 검증이 문제를 통과시킨 이유

### Windows

- `IsExactFor()`가 metrics generation과 logical layout 크기를 검사하지 않는다.
- 자동 gate의 `exact-size mismatch == 0`은 back-buffer descriptor 일치만 증명한다.
- window bounds→target 추종은 빠르지만 target→scene layout의 동일 세대성은 측정하지 않는다.
- 자동 drag와 green build는 AppBar가 실제 client 끝까지 채워지는지 증명하지 않는다.

### Web

- 이전 40-sample smoke의 핵심 기준은 blank/stale/GL error/queue depth였다.
- 당시 설계가 `stretched front`를 명시적으로 허용했으므로 고정 높이 UI의 scale 변화는 실패 조건이 아니었다.
- 자동 native bounds 변경과 40장의 sample PNG는 실제 pointer border drag의 모든 30/60/165 Hz 중간 frame을 보존하지 않는다.
- history에도 실제 pointer window-border drag와 최종 사용자 visible acceptance가 `notVerified`로 남아 있다.

따라서 기존 PASS와 이번 녹화는 모순되지 않는다. 기존 gate가 다루지 않은 visible invariant를 사용자가 이번에 확인한 것이다.

## 6. 최종 원인 판정

| 우선순위 | 플랫폼 | 판정 | 근거 |
| --- | --- | --- | --- |
| 1 | Windows | **scene metrics와 surface target의 세대 결합 누락** | 최신 surface 안에서 AppBar만 88~100 px 짧음. descriptor에 `MetricsGeneration`은 있으나 exact gate에서 미검사 |
| 2 | Windows | **exact `ResizeBuffers`/layout/raster/present latency와 supersede** | target 전달은 빠르지만 과거 target→ACK p50 16.6 ms/p95 23.1 ms. OS border는 이 pipeline을 기다리지 않음 |
| 3 | Web | **이전 front를 새 크기로 `NEAREST` stretch하는 명시적 정책** | 코드의 source/destination 크기 다른 `blitFramebuffer`; 녹화의 AppBar 높이 92~146 px 변화 |
| 4 | Web | **여러 rAF + async interop 동안 default framebuffer continuity 불완전** | 녹화에서 약 0.07초와 0.13초 blank, `preserveDrawingBuffer=false`, signal 시점에만 retained re-blit |

따라서 단순 debounce, `Present(1)`, swap interval 변경, ResizeObserver 추가만으로는 해결되지 않는다. Windows는 **scene이 실제로 build된 metrics epoch와 surface target epoch를 하나의 승인 단위로 묶는 것**이 먼저이고, Web은 **exact frame 전까지 무엇을 보여 줄지에 대한 정책에서 비등방 stretch를 제거하면서도 default framebuffer continuity를 보장하는 것**이 먼저다.

## 7. 공식 사양/문서 근거

- Microsoft Learn, [`DXGI_SCALING`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling): `DXGI_SCALING_STRETCH`는 back buffer를 presentation target 크기에 맞춰 scale한다.
- Microsoft Learn, [`IDXGISwapChain::ResizeBuffers`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizebuffers): 창 resize에 맞춰 back buffer 크기를 바꾸며, 호출 전 모든 buffer reference를 해제해야 한다.
- Microsoft Learn, [`SwapChainPanel`](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainpanel): composition scale 변화 시 crisp rendering을 위해 scale을 다시 반영하고 render해야 한다.
- Microsoft Learn, [`IDXGISwapChain::Present`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-present): flip-model `SyncInterval=0`은 queue 동작을 바꾸지만 layout/build/ResizeBuffers 비용 자체를 제거하지 않는다.
- WHATWG, [`canvas` element](https://html.spec.whatwg.org/multipage/canvas.html): `width`/`height`는 canvas output bitmap 크기를 제어하며 CSS 표시 크기와 별개다.
- Khronos, [WebGL drawing buffer specification](https://registry.khronos.org/webgl/specs/latest/1.0/#2.2): drawing-buffer 크기는 canvas `width`/`height`에 의해 정해지고 resize 또는 `preserveDrawingBuffer=false` presentation 뒤 clear될 수 있다.
- Khronos, [`WebGL2RenderingContext.blitFramebuffer`](https://registry.khronos.org/webgl/specs/latest/2.0/#3.7.4): 서로 다른 source/destination rectangle 사이의 framebuffer 전송과 filter 동작을 정의한다.

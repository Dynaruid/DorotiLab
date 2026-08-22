# Windows / Web interactive resize 근본 해결 연구

- 작성일: 2026-08-22
- 입력: [`problem.md`](problem.md), 현재 Doroti resize/scene/raster/present 코드, 이전 계측·실패 기록
- 제약: SkiaSharp fork, reflection, private member monkey patch를 사용하지 않는다.
- 상태: 구현 전 설계 검토. 이 문서를 작성하면서 build/live test는 실행하지 않았다.

## 1. 결론

확인된 epoch 오인과 Web provisional 왜곡은 SkiaSharp를 포크하지 않고 고칠 수 있다. 고쳐야 할 경계는 Skia 내부가 아니라 Doroti가 이미 소유한 세 곳이다. Windows에서 exact present 사이에 남을 수 있는 compositor stretch까지 완전히 제거할 수 있는지는 별도 public-host spike로 검증해야 한다.

1. **framework frame이 실제로 사용한 metrics/target epoch를 scene에 보존한다.** `Submit()` 시점의 최신 target으로 이전 scene을 다시 이름 붙이지 않는다.
2. **Windows는 API가 요구하는 `DXGI_SCALING_STRETCH`를 유지하되, app이 `Present()`하는 모든 buffer를 현재 target과 exact-size로 만든다.** 이 수정은 잘못된 A layout을 B 크기 buffer에 그려 승인하는 직접 원인을 없앤다. panel resize와 다음 exact present 사이에 compositor가 보존된 이전 buffer를 stretch하는 잔여 현상은 따로 계측한다.
3. **Web은 ResizeObserver와 retained FBO를 하나의 표시 transaction으로 묶는다.** 이전 front를 새 viewport 전체로 stretch하지 않고, 겹치는 논리 영역만 1:1 crop하여 보여 준 뒤 최신 exact staging frame만 commit한다.
4. Windows의 `IDXGISwapChain2.SetSourceSize` capacity 경로는 바로 채택하지 않는다. 과거 지연은 줄었지만 사용자가 내부 Skia 배율 이상을 확인한 실패 분기이므로, 공통 정확성 수정 뒤 별도 실험 gate에서만 재검토한다.

가장 중요한 사실은 다음과 같다.

> OS border는 framework layout보다 먼저 움직일 수 있다. 아직 계산되지 않은 responsive layout을 어떤 present API도 미리 만들어 줄 수는 없다. Web은 그 사이 **이전 frame의 1:1 crop + background**를 명시적으로 만들 수 있다. 반면 현재 Windows `SwapChainPanel` 경로는 `CreateSwapChainForComposition`이 `DXGI_SCALING_STRETCH`를 요구하므로, app의 mismatched present는 금지할 수 있어도 다음 exact present 전 compositor 동작까지 1:1 clip으로 강제할 수는 없다. 먼저 이 구간을 최소화·계측하고, 잔여 stretch가 허용 불가능하면 SkiaSharp가 아니라 Windows presentation host 경계를 다시 설계해야 한다.

따라서 목표는 “모든 중간 frame이 이미 최종 reflow를 가진다”가 아니다. 다음 두 보장을 분리한다.

- 정확성: stale layout을 최신 exact frame으로 승인하거나 비등방 확대하지 않는다.
- 성능: 최신 exact frame을 가능한 적은 refresh opportunity 안에 commit한다.

## 2. 포크가 필요 없는 이유

현재 프로젝트는 이미 필요한 공개 경계를 가지고 있다.

현재 고정 패키지는 SkiaSharp 4.151.1과 Vortice.DXGI 3.8.3이며, 설치된 public API metadata에서도 `GRContext.CreateDirect3D`, `GRContext.ResetContext`, `GRBackendRenderTarget`, `SKSurface.Create`, `IDXGISwapChain2.SetSourceSize`를 확인할 수 있다.

| 역할 | 현재/권장 공개 API | 소유자 |
| --- | --- | --- |
| Windows GPU context | `GRContext.CreateDirect3D` | SkiaSharp public API |
| D3D12 back buffer wrapping | `GRBackendRenderTarget`, `SKSurface.Create` | SkiaSharp public API |
| Windows composition surface | `SwapChainPanel`, `CreateSwapChainForComposition` | WinUI/DXGI public API |
| Windows 현재 swap-chain scaling | `DXGI_SCALING_STRETCH` | `CreateSwapChainForComposition`이 요구하는 DXGI public API 계약 |
| Windows source viewport 실험 | `IDXGISwapChain2.SetSourceSize`, `SetMatrixTransform` | DXGI public API. 단독으로 1:1 표시를 보장하지 않음 |
| Web exact raster target | WebGL2 FBO + `GRBackendRenderTarget`, `SKSurface.Create` | WebGL2/SkiaSharp public API |
| Web 크기 관찰 | `ResizeObserverEntry.contentBoxSize`, `devicePixelContentBoxSize` | Web platform API |
| Web retained 표시 | `clear`, `blitFramebuffer`, `requestAnimationFrame` | WebGL2/browser API |

실제로 고쳐야 하는 것은 scene의 세대 identity, host의 size authority, retained frame 표시 정책이다. 이것들은 모두 Doroti source 안에 있다.

Web의 `SkiaSharpGL`, Emscripten `GL.framebuffers` handle 등록은 Web 표준 API는 아니다. 그러나 이는 현재 public SkiaSharp GL surface에 app-owned FBO를 연결하는 버전 고정 adapter이며, 이번 geometry bug의 원인은 아니다. 이번 범위에서는 작은 adapter와 ABI contract test로 격리한다. 장기적으로는 이 의존성을 없애는 public hook을 upstream에 제안하거나 native GLES sidecar를 검토할 수 있지만, geometry 수정 자체의 선행 조건은 아니다.

## 3. 먼저 고정할 공통 불변식

다음 불변식이 Windows와 Web 모두의 선행 조건이다.

### 3.1 하나의 viewport epoch

host가 framework에 metrics를 알릴 때 target과 metrics를 분리해서 읽지 않는다. 아래와 같은 immutable 묶음을 한 번 만든다.

```text
DorotiViewEpoch
  ViewId
  ResizeTargetGeneration
  MetricsGeneration
  LogicalWidth / LogicalHeight
  PhysicalWidth / PhysicalHeight
  DeviceScaleX / DeviceScaleY
  Timestamp
```

일반적인 window에서는 `DeviceScaleX == DeviceScaleY`이고 기존 scalar `devicePixelRatio`로 노출할 수 있다. 두 값이 다르면 framework의 scalar DPR 계약으로는 정확히 표현할 수 없으므로 조용히 X 값만 양축에 쓰지 말고 `nonUniformDeviceScale`로 reject/diagnose해야 한다.

scale은 `PhysicalWidth / LogicalWidth`처럼 반올림된 dimension에서 역산하지 않는다. Windows의 선언된 `CompositionScaleX/Y` 또는 Web의 관찰 당시 DPR과 physical dimensions를 각각 원본 필드로 보존하고, scale은 선언값끼리, dimension은 dimension끼리 비교한다.

Windows에서는 `SwapChainPanel` target payload 하나로 이 epoch를 만들고, Web에서는 한 `ResizeObserverEntry`로 만든다. `ViewMetrics`, resize target, physical size를 각자 다른 시점에 다시 읽어 조립하지 않는다.

### 3.2 frame 시작에서 build token 캡처

`PlatformDispatcher.DispatchFrame()` 시작 시 현재 `DorotiViewEpoch`을 한 번 캡처해 그 begin/draw transaction 전체에 적용한다.

```text
DorotiSceneBuildToken
  ViewEpoch
  FrameworkFrameNumber
  RootPhysicalWidth / RootPhysicalHeight
```

`RootPhysicalWidth/Height`는 이미 `RenderView.compositeFrame()`이 `render(scene, size)`로 전달하고 있지만, 현재 [`DorotiView.render(scene, Size size)`](Doroti/src/Doroti.Ui/PlatformDispatcher.cs)는 `size`를 버린다. 이 값을 token에 넣으면 “metrics는 B라고 기록했지만 실제 root는 A 크기로 compositing된 scene”도 검출할 수 있다.

구현은 frame scope + immutable `DorotiSceneSubmission(Scene, BuildToken)` 형태가 적절하다. public `Scene`을 mutable하게 바꾸는 것보다 submission에 token을 붙이면 scene command 수명과 frame identity를 분리할 수 있다. frame scope 밖에서 만든 token 없는 scene은 resize-aware host에서 exact로 승인하지 않는다.

### 3.3 submit-time relabel 금지

현재 [`SkiaSceneRenderer.Submit()`](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs)은 scene이 완성된 뒤 `_host.ResizeTarget`을 다시 읽어 descriptor를 만든다. 이 동작을 제거한다.

renderer는 submission의 build token에 `SceneSequence`만 추가해야 한다.

```text
잘못된 흐름
  layout A → target B → Submit에서 B를 읽음 → A commands에 B descriptor

권장 흐름
  frame 시작에 A token → layout A → target B → A token 그대로 Submit
  → latest B gate가 A를 superseded 처리
```

queue 역시 단순 도착 순서로 더 오래된 A가 pending B를 교체하지 못하게 한다. admission 순서는 `(ResizeTargetGeneration, MetricsGeneration, SceneSequence)`로 비교하고, stale submission은 terminal만 기록한다.

### 3.4 exact match는 모든 관련 값을 검사

현재 [`DorotiFrameDescriptor.IsExactFor()`](Doroti/src/Doroti.Ui/ResizeLifecycle.cs)는 target generation, physical size, DPR만 검사한다. 새 matcher는 최소 다음을 모두 확인하고 bool 대신 mismatch code를 반환해야 한다.

- scene target generation == current target generation
- scene metrics generation == 그 target에 bind된 latest metrics generation
- scene logical size == current epoch logical size
- scene physical size == current epoch physical size
- scene root physical size == current target physical size
- surface physical size와 scale == current target

target generation은 주 identity이고 수치 필드는 corruption을 잡는 중복 assertion이다. logical double은 서로 다른 시점에 재계산하지 말고 canonical epoch 값을 복사해 비교한다.

`request serial == latest serial`은 이 geometry matcher에 넣지 않는다. 현재 serial은 resize뿐 아니라 animation/input 등 모든 `InvalidateSurface()`에서 증가하므로, 별도의 scheduler latest-work gate에서만 검사한다. 즉 viewport가 틀린 frame과 크기는 정확하지만 더 최신 render request가 생긴 frame을 서로 다른 terminal/mismatch code로 구분한다.

### 3.5 pre-present latestness와 남는 race

Windows는 현재 final target/serial check lock을 푼 뒤 `Present()`를 호출한다. 그 사이 새 target이 publish될 수 있지만, broad `_gate`를 `Present()`와 현재 그 내부의 `ReleaseBuffer()`까지 잡는 것을 기본 해법으로 삼지 않는다. UI target publication을 막아 이미 도착한 B를 A 뒤로 인위적으로 재정렬할 수 있고, OS border와 compositor를 원자화하지도 못하기 때문이다.

기준선은 다음과 같다.

- `Present()` 바로 직전에 atomic latest target generation과 scheduler serial을 다시 검사한다.
- trace에 `prePresentTargetGeneration`, `presentedGeneration`, `postPresentObservedGeneration`을 각각 남긴다. pre-present에 이미 알려진 새 target을 무시한 경우는 `newerTargetKnownAtPrePresent`, check 뒤 Present 도중 target이 진행한 경우는 `targetAdvancedDuringPresent`로 나눈다.
- geometry mismatch와 scheduler supersede를 구분한다.
- `Present()`와 buffer release를 분리해 critical path를 측정 가능하게 만든다.

`targetAdvancedDuringPresent`가 실제로 관찰되고 strict Doroti-order 선형화가 필요할 때만, target publication과 “final check + DXGI Present 호출만” 공유하는 별도의 좁은 commit arbitration을 feature flag로 실험한다. UI latency, deadlock, reentrancy와 left-drag 화면 gate를 통과해야 하며, callback·trace formatting·resource release는 arbitration 밖에 둔다. 어느 방식도 실제 화면 판정을 대신하지 않는다.

## 4. Windows 권장 구조

### 4.1 WIN-1: single size authority

Windows logical size authority는 `SwapChainPanel` 하나로 둔다.

- `SwapChainPanel.SizeChanged`의 `NewSize` 또는 같은 layout pass의 `ActualWidth/ActualHeight`를 사용한다.
- `CompositionScaleX`와 `CompositionScaleY`를 모두 캡처한다.
- Windows metrics 입력에서 MAUI `_surface.Width/_surface.Height` 재읽기를 제거한다.
- parameterless `SizeChanged` 대신 `TargetChanged(DorotiResizeEpoch payload)`를 보낸다.
- `MauiHostAdapter`는 payload의 logical/physical/scale만 적용하고 immutable view epoch을 저장한다.

현재는 panel event가 target B를 만든 직후 adapter가 아직 A인 MAUI width를 읽을 수 있다. 이후 MAUI event가 와도 `PublishTarget()`이 같은 target generation이라 조기 반환하면 correction notification까지 사라진다. payload 방식은 이 feedback/race 자체를 없앤다.

`Maui.SizeChanged`는 Windows에서 size authority로 사용하지 않는다. 필요하면 “panel 값을 다시 publish하라”는 보조 signal로만 사용할 수 있고, 그 경우에도 metrics는 panel payload로만 만든다.

### 4.2 WIN-2: exact-size correctness baseline

공통 epoch 수정 직후에는 물리 target과 back buffer를 1:1로 유지하는 현재 exact-size 구조를 기준선으로 둔다. `CreateSwapChainForComposition` 공식 계약상 `Scaling`은 `DXGI_SCALING_STRETCH`여야 하므로 이를 `None`으로 바꾸지 않는다. 대신 app이 제출하는 순간에는 buffer/source와 현재 panel physical target이 정확히 같아 실제 stretch가 필요 없는 상태만 허용한다.

1. swap chain 생성은 API가 요구하는 `Scaling.Stretch`와 flip-sequential을 유지한다.
2. `SetMatrixTransform`에는 원본으로 보존한 `1 / CompositionScaleX`, `1 / CompositionScaleY`를 각각 적용하고, 변경은 다음 `Present()`부터 유효하다는 전제로 token과 함께 검증한다.
3. `ResizeBuffers` 전 현재 surface를 flush하고 Skia command를 submit한다. 단순 경로는 `Submit(true)`로 완료를 기다린다. explicit fence 경로라면 먼저 `Context.Submit(false)`로 Skia work를 같은 D3D12 queue에 제출한 뒤 그 queue에 signal하고 fence를 wait한다. 그 다음 `SKSurface` → `GRBackendRenderTarget`/resource info → `ID3D12Resource`와 모든 direct/indirect reference를 해제하고 resize한다.
4. Microsoft의 XAML/DirectX interop 지침대로 `ResizeBuffers` 뒤 같은 swap chain을 `ISwapChainPanelNative.SetSwapChain`으로 다시 연결하는 경로를 검증한다.
5. exact build token이 아니면 raster, replay, flush, present 어느 단계에서도 승인하지 않는다.
6. scheduler serial은 별도 latest-work gate로 검사하고, `Present(0)` 직전 target generation을 마지막으로 다시 읽는다.
7. `Present()` 전후 generation을 기록하고, 실제 race가 확인될 때만 앞 절의 좁은 commit arbitration을 별도 실험한다.

이 기준선은 캡처에서 보인 “A layout이 B 크기 buffer 내부에서 오른쪽을 못 채우거나 찌그러진 채 exact로 승인되는 문제”를 직접 막는다. 다만 panel이 이미 B로 변했는데 compositor가 아직 A의 마지막 buffer를 보존하는 짧은 구간에는 필수 `Stretch` 때문에 transient compositor scaling이 남을 수 있다. 이는 app의 mismatched present와 다른 현상으로 따로 측정해야 하며, 현재 API에서 `Scaling.None`/background strip으로 없앨 수 있다고 주장해서는 안 된다.

target-only wake에서 `ResizeBuffers`를 미리 수행하는 현재 overlap은 과거 latency를 줄인 근거가 있으므로 첫 구현에서 바로 제거하지 않는다. 다만 GPU별로 last visible content를 잃거나 full-background frame을 만든다면 다음 중 하나로 분기한다.

- exact scene이 준비될 때까지 destructive `ResizeBuffers`를 미룬다.
- 아래 capacity back buffer 실험을 통과시켜 per-target `ResizeBuffers`를 제거한다.

`Present(0)`, swap interval, `DwmFlush`는 이 정확성 문제의 해법이 아니다. 이들은 epoch mismatch나 이전 bitmap의 geometry를 고치지 않는다.

### 4.3 WIN-3: `SetSourceSize` capacity 경로는 실험 gate

이전 실험에서 `SetSourceSize`는 target→ACK p50을 약 12.811 ms로 낮췄지만 사용자가 내부 Skia 배율 이상을 확인해 철회되었다. 따라서 “Microsoft가 제공한 API이므로 동작할 것”이라고 가정해서는 안 된다.

다만 Microsoft는 `SetSourceSize`를 `ResizeBuffers` 없이 source region을 바꾸는 저비용 effective resize API로 정의한다. 공통 correctness와 exact-size `Stretch` baseline이 통과한 뒤 다음처럼 **buffer capacity와 현재 유효 viewport를 분리한 새 설계**로만 재실험할 가치가 있다. 이 API는 source rectangle을 선택할 뿐이며, 그 자체로 1:1 표시를 보장하지 않는다. `SetSourceSize`와 `SetMatrixTransform` 모두 다음 `Present()`에 맞춰 하나의 target token으로 적용하고 실제 결과를 visual gate로 판정해야 한다.

```text
Swap-chain resource capacity: Cw × Ch, 비교적 오래 유지
Presented source viewport:    [0, 0, Tw, Th]
Scene build/raster target:    exact Tw × Th
Composition matrix:           inverse ScaleX / ScaleY
```

필수 조건은 다음과 같다.

- `GRBackendRenderTarget` width/height는 실제 D3D12 resource capacity `Cw/Ch`로 고정한다.
- renderer의 `surface capacity`와 `scene viewport`를 별도 인자로 만든다. 현재처럼 scene physical size와 wrapped surface size가 같아야 한다는 gate를 그대로 재사용하지 않는다.
- 매 back buffer를 전체 app background로 clear하고, `[0,0,Tw,Th]`만 clip하여 scene을 1:1로 그린다. capacity에 맞추는 canvas scale은 금지한다.
- exact scene raster가 끝난 뒤 `SetSourceSize(Tw, Th)`와 matrix를 함께 적용하고 present한다. source, matrix, 필수 `Stretch`, panel 조합의 최종 mapping을 계산만으로 승인하지 않고 capture로 검증한다.
- target이 capacity를 넘을 때만 fence 후 grow-only `ResizeBuffers`를 수행한다. shrink trim은 interactive resize 밖에서만 한다.
- 같은 buffer 안의 아직 그리지 않은 영역이 expansion 때 노출되지 않도록 전체 clear/guard 영역을 검증한다.
- capacity surface와 source viewport, scene viewport 세 크기를 trace에 각각 남긴다.

대략 3840×2160 RGBA buffer 두 장만 약 63 MiB이므로 capacity 정책에는 명시적 GPU memory budget과 최대 texture size가 필요하다.

이 실험의 stop condition은 엄격해야 한다.

- AppBar 높이 또는 글자/원형 종횡비가 1 frame이라도 달라짐
- 100/125/150/200% 중 하나에서 source와 panel 끝점이 맞지 않음
- left drag에서 고정 오른쪽 기준점이 흔들림
- `GRBackendRenderTarget` dimension과 resource dimension 불일치
- device removed/reset 또는 unbounded resource 증가

하나라도 발생하면 capacity 경로는 실패로 남기고 exact-size baseline을 유지한다. 과거 이상 원인을 현재 자료만으로 capacity/wrapper 불일치라고 단정하지 않는다.

### 4.4 WIN-4: 잔여 compositor stretch가 남을 때의 구조적 분기

epoch/authority 수정과 exact-frame latency 개선 뒤에도 panel resize와 다음 present 사이의 stretch가 사용자 기준을 넘는다면, 현재 `SwapChainPanel + CreateSwapChainForComposition` 조합 안에서 옵션 하나로 해결할 수 있다고 가정하지 않는다. `CreateSwapChainForComposition`이 `DXGI_SCALING_STRETCH`를 요구하기 때문이다.

그때는 **지원되는 no-scale/clip semantics를 실제로 증명할 수 있는 별도의 공개 Windows presentation host**를 architecture spike로 조사한다. 후보를 정할 때 WinUI embedding, input/semantics/accessibility, DPI, window lifetime과 no-scale 동작을 작은 executable에서 먼저 검증한다. 이는 Doroti의 Windows host를 넓게 바꾸는 일일 수 있지만 SkiaSharp fork는 아니다. 지원 여부를 확인하지 않은 채 현재 composition swap chain에 `Scaling.None`만 넣는 것은 후보가 아니다.

### 4.5 Windows에서 우선하지 않을 대안

| 대안 | 판정 |
| --- | --- |
| frame-latency waitable swap chain | 과거 개선 없음/악화. epoch 수정 뒤 별도 계측 없이는 재도입하지 않음 |
| `Context.Submit(false)`만 변경 | 과거 `DXGI_ERROR_INVALID_CALL`. D3D12 fence/resource lifetime 설계 없이 금지 |
| `Present(1)` 또는 `DwmFlush` | build/target mismatch를 해결하지 않음 |
| debounce/timer | 최신 target 도달을 늦추며 correctness transaction을 만들지 못함 |
| `AppWindow.ClientSize`/`WM_SIZE`를 새 authority로 추가 | panel layout과 다시 두 size authority를 만듦. 계측에만 사용 |
| 현재 `CreateSwapChainForComposition`에 `Scaling.None` 지정 | API의 명시적 `Scaling.Stretch` 요구와 충돌하므로 제품 경로에서 금지 |
| SkiaSharp Views handler 복귀 | private scheduler/size authority를 다시 가져오므로 현재 Doroti-owned pipeline보다 제어가 약함 |

## 5. Web 권장 구조

### 5.1 WEB-1: ResizeObserver callback에서 target과 provisional frame을 함께 commit

현재 [`scheduleHostSample()`](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)은 `ResizeObserver` 안에서 다시 rAF를 예약한다. browser rendering 절차에서 rAF callback 뒤에 ResizeObserver delivery가 오므로, observer 안의 새 rAF는 다음 rendering opportunity로 넘어간다.

`getBoundingClientRect()`를 다음 rAF에서 다시 읽지 말고 `ResizeObserverEntry`를 직접 사용한다.

- logical: `contentBoxSize` 또는 `contentRect` fallback
- physical: `devicePixelContentBoxSize` 우선
- fallback: canonical rounding rule로 `logical × devicePixelRatio`

가능한 browser에서는 `observe(root, { box: "device-pixel-content-box" })`를 사용해 CSS logical size가 같고 device-pixel size만 바뀌는 경우도 notification 대상으로 만든다. 미지원 browser는 `content-box` observer와 기존 DPR `matchMedia` watcher를 유지하되, 둘 다 같은 epoch 생성 함수로 들어가야 한다.

observer callback은 다음 synchronous section을 끝낸 뒤 managed metrics를 emit해야 한다.

```text
ResizeObserverEntry로 target B 생성
  → latest epoch를 B로 저장
  → canvas backing을 B physical size로 변경
  → canvas CSS를 B logical size로 명시
  → app background clear
  → retained front와 B가 겹치는 논리 영역만 blit
  → 위 작업이 끝난 뒤 metrics B emit
```

`canvas.width/height` 변경과 첫 clear/blit 사이에는 `await`, Promise continuation, rAF, timer가 있어서는 안 된다. backing reset은 drawing buffer를 clear하므로 같은 JS stack에서 opaque background 또는 retained pixels를 다시 만들어야 한다. 요청 크기뿐 아니라 실제 `gl.drawingBufferWidth/Height`도 검사한다. 둘이 다르면 metrics B를 emit하거나 exact presenter를 진행하지 않고 해당 epoch를 `failed/unsupported-size`로 끝낸다. 실제 크기를 새 canonical target으로 채택할 수 있는지는 별도 capability 검증을 거치며, 그 전에는 opaque safe background를 유지한다.

### 5.2 CSS size authority 제거

현재 `.doroti-root > canvas { width: 100%; height: 100%; }`는 root가 바뀌는 즉시 이전 bitmap을 새 box에 맞춰 확대하는 독립 size authority다.

- `100%`를 제거하고 JS epoch가 inline `style.width/height`를 명시한다.
- observer callback 전까지 canvas는 마지막 명시 크기를 유지하므로 root expansion은 background로 보이고 shrink는 `.doroti-root { overflow:hidden }`로 clip된다.
- callback 안에서는 새 backing과 provisional pixels를 만든 뒤 같은 turn에 CSS size도 새 epoch로 바꾼다.

이렇게 하면 main thread가 잠시 늦어도 old bitmap 자체가 자동 stretch되지 않고, callback이 실행된 frame에도 backing reset과 redraw 사이 blank가 paint되지 않는다.

pointer와 semantics 좌표 authority는 최신 root logical box로 유지해야 한다. canvas가 잠시 이전 명시 크기인 구간까지 완전히 커버하려면 pointer listener를 root/input overlay에 두고 root-local 좌표를 사용한다. visual canvas의 provisional 크기와 hit-test 영역을 같은 것으로 가정하지 않는다.

### 5.3 retained front는 1:1 crop + background

`front`에는 physical width/height뿐 아니라 그 frame의 logical width/height와 DPR을 보존한다. 새 target B의 provisional 표시 규칙은 다음과 같다.

- 같은 DPR: source/destination의 겹치는 physical rectangle을 1:1 blit
- DPR 변경: `newDpr / oldDpr`의 **동일한 X/Y 비율**만 임시 허용
- expansion: 오른쪽/아래 새 영역은 app background
- shrink: top-left 기준 crop
- X/Y 독립 scale: 항상 금지

현재 surface는 `GRSurfaceOrigin.BottomLeft`이므로 top-left 논리 crop을 WebGL rectangle로 바꿀 때 Y origin을 명시적으로 변환하고 test해야 한다. source 전체를 destination 전체에 넣는 현재 `blitToDefault(..., gl.NEAREST)`는 제거한다.

DPR-only transition에서 균일 resample이 필요하면 `LINEAR`를 사용할 수 있지만, 이는 exact frame 전 짧은 provisional 정책일 뿐이다. 같은 DPR에서는 filter 선택과 무관하게 source/destination pixel 수가 같아야 한다.

### 5.4 WebGL/Skia state ownership

JS의 provisional clear/blit과 managed Skia raster는 같은 WebGL context를 공유하므로 “직전에 누가 어떤 GL state를 남겼는가”에 의존하면 안 된다. canvas backing size 변경은 app-owned FBO 자체를 파괴하지 않고 default drawing buffer를 재할당·clear하지만, viewport를 포함한 GL state를 자동 reset하지 않는다. 또한 JS 작업은 Skia가 cache한 GL state를 무효화할 수 있다.

- 모든 JS clear/blit entry는 `READ_FRAMEBUFFER`/`DRAW_FRAMEBUFFER`, read/draw buffer, viewport, clear color와 필요한 color/depth/stencil mask를 명시한다.
- 특히 `SCISSOR_TEST`를 끄고, source/destination rectangle과 color attachment를 매번 다시 설정한다. 이전 Skia clip/scissor가 남아 있다고 가정하지 않는다.
- JS가 GL state를 만진 뒤 다음 managed Skia raster 직전에 public `GRContext.ResetContext(GRGlBackendState.All)`로 외부 state 변경을 알린다.
- JS 작업 뒤 임의 state를 “복원”하는 대신, JS와 Skia 각각이 자기 entry state를 완전히 선언하는 ownership boundary로 만든다.
- 각 transition에서 `gl.getError()`와 FBO completeness를 debug/validator build에 기록한다.

`ResetContext`를 frame마다 무조건 부르는 것이 아니라 JS interop이 state를 변경했다는 dirty bit가 있을 때 호출하면 public API 경계를 유지하면서 비용을 제한할 수 있다.

### 5.5 exact staging commit

app-owned front/staging FBO 구조는 유지한다. Khronos가 권장하는 retained 방식이며 `preserveDrawingBuffer:true`로 바꿀 필요가 없다.

```text
scene B submission
  → staging FBO B에 public SkiaSharp surface로 raster
  → await 뒤 latest epoch 재검사
  → B가 아니면 staging 폐기/superseded
  → B이면 default framebuffer에 exact-size blit
  → front/staging swap
  → terminal submitted 기록
```

ResizeObserver가 이미 backing과 provisional content를 B로 만든 뒤이므로 presenter는 raster 시작 전에 `canvas.width/height`를 다시 쓰지 않는다. exact commit의 source/destination rectangle도 동일 크기여야 한다.

`preserveDrawingBuffer:false`는 유지한다. resize pending 동안 browser가 새 compositing draw를 필요로 하면 front FBO에서 동일 크기 re-blit한다. default framebuffer를 source of truth로 보지 않고 front FBO를 source of truth로 유지한다.

WebGL context loss 때 FBO는 보존될 수 없으므로 이는 resize continuity와 다른 허용 경계다. opaque root background를 보인 뒤 context restore와 latest exact scene으로 front/staging을 재생성한다.

### 5.6 WEB-2: 불필요한 rAF 직렬화 제거

정확성 수정 뒤 다음 성능 개선을 한다.

- `ResizeObserver → sample rAF` 제거
- managed framework frame rAF는 browser vsync entry로 유지
- framework frame 안에서 도착한 `requestPresent()`를 presenter 전용 다음 rAF까지 무조건 미루지 않고 same-turn drain 또는 한 microtask에서 시작
- one-in-flight + one-latest coalescing과 exactly-once terminal은 유지

가능하면 하나의 `BrowserFramePump`가 latest target, managed callback, ready presenter work를 조정한다. 다만 async .NET raster가 browser paint 전에 끝난다는 보장은 없으므로 rAF 완료를 display ACK로 기록하지 않는다. `submitted`와 실제 browser compositor/화면 관측은 구분한다.

### 5.7 Web에서 우선하지 않을 대안

| 대안 | 판정 |
| --- | --- |
| `LINEAR` full stretch | 픽셀화만 줄고 고정 높이 UI가 변하는 문제는 그대로 |
| `object-fit: contain` | 비등방 왜곡은 막지만 전체 UI scale과 blur가 변함. 비상 fallback 수준 |
| `preserveDrawingBuffer:true` | resize clear를 없애지 못하고 일부 GPU 성능 비용. 진단 비교 외 비권장 |
| default framebuffer에 직접 Skia raster | staging의 atomic/latest gate와 retained continuity를 잃음 |
| 두 visible canvas/ImageBitmap swap | context/bitmap lifetime과 GPU copy가 늘어남 |
| OffscreenCanvas/Worker | 전체 managed Skia/Emscripten context를 worker로 옮겨야 의미가 있음. geometry 해결의 선행 조건 아님 |
| private `SKHtmlCanvasInterop` patch/fork | 필요 없음 |

## 6. 파일별 구현 위치

권장 변경 순서는 다음과 같다.

### 공통 계약

1. [`ResizeLifecycle.cs`](Doroti/src/Doroti.Ui/ResizeLifecycle.cs)
   - `DorotiViewEpoch`, `DorotiSceneBuildToken`, 상세 `DorotiFrameMatchResult` 추가
   - logical/metrics/root/surface까지 검사하는 matcher 추가
2. [`ViewContracts.cs`](Doroti/src/Doroti.Ui/ViewContracts.cs), [`PlatformDispatcher.cs`](Doroti/src/Doroti.Ui/PlatformDispatcher.cs)
   - host epoch capture와 frame build scope 추가
   - `render(scene, size)`의 실제 root physical size 사용
   - warm-up frame도 명시적 epoch scope 적용
3. [`GraphicsAndSemanticsContracts.cs`](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs)
   - immutable `DorotiSceneSubmission` 또는 scene build token 보존
4. [`SkiaRendererContracts.cs`](Doroti/src/Doroti.Skia.Rendering/SkiaRendererContracts.cs), [`SkiaSceneRenderer.cs`](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs)
   - `MetricsGeneration`/`ResizeTarget` 분리 재읽기를 하나의 epoch로 교체
   - submit-time relabel 제거, stale admission ordering, 공통 matcher 적용

생성된 framework의 `RenderView.compositeFrame()` 흐름은 바꿀 필요가 없다. 이미 전달하는 `size`를 Doroti UI 경계에서 버리지 않으면 된다.

### Windows

5. [`MauiSkiaSurface.cs`](Doroti/src/Doroti.Host.Maui/MauiSkiaSurface.cs), [`MauiHostAdapter.cs`](Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs)
   - parameterless size signal을 target payload로 변경
   - Windows에서 MAUI View size 재읽기 제거
6. [`MauiSkiaCapabilities.cs`](Doroti/src/Doroti.Host.Maui/MauiSkiaCapabilities.cs), [`DorotiWindowsDxgiSurface.cs`](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs)
   - atomic view epoch, API-required `Scaling.Stretch`, exact-size buffer, X/Y matrix, resize 뒤 panel rebind
   - pre/post-present generation과 detailed mismatch trace; commit arbitration은 별도 feature flag
   - resize 전 Skia submit/GPU completion/reference release 순서 명시
   - capacity 실험 시 surface capacity와 scene viewport 분리

### Web

7. [`BrowserHostContracts.cs`](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs), [`BrowserSkiaCapabilities.cs`](Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs)
   - ResizeObserver epoch과 metrics generation의 1:1 mapping
8. [`doroti.web.ts`](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)
   - observer entry 직접 사용, synchronous provisional commit, crop blit, latest exact commit
   - JS clear/blit의 framebuffer/scissor/viewport/mask state를 완전히 선언하고 GL dirty bit 노출
   - 불필요한 sample/presenter rAF 제거 또는 통합
9. [`doroti.web.css`](Doroti/src/Doroti.Host.Web/wwwroot/doroti.web.css)
   - canvas `width/height:100%` 제거, JS-owned explicit size
10. [`DorotiWebGlSurface.razor`](Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor)
    - existing public `GRBackendRenderTarget`/`SKSurface` 경계 유지
    - JS가 GL state를 변경한 뒤 raster 전 `GRContext.ResetContext(GRGlBackendState.All)` 호출
    - build token까지 exact render 결과에 연결

Qt/Android/iOS/macOS host는 새 공통 type에 맞춘 compile adaptation이 필요하지만 Windows의 panel policy나 Web의 provisional policy를 공유하도록 강제하지 않는다.

## 7. 구현 단계와 실패 관문

순서를 바꾸지 않는 것이 중요하다.

### C0 — deterministic epoch contract

- A metrics build → target B → A submit이 B로 relabel되지 않고 `superseded`
- 같은 physical size여도 metrics generation이 다르면 reject
- logical size 또는 root physical size만 다르게 주입해도 reject
- stale A submission이 pending B를 교체하지 않음
- 모든 scene terminal exactly once, queue depth ≤ 2
- scheduler serial만 최신이 아니면 geometry mismatch가 아닌 scheduler-superseded로 종료
- barrier race에서 B publish가 final pre-present check보다 먼저면 A present 0회
- final check 뒤 B가 도착하는 race는 `targetAdvancedDuringPresent`로 빠짐없이 집계하고 geometry mismatch와 구분

C0가 실패하면 native/Web 최적화로 진행하지 않는다.

### WIN-1 — authority + exact-present baseline

- panel target B + MAUI width A 상황에서도 adapter epoch은 B
- duplicate signal 뒤 correction 유실 0
- swap-chain 생성이 필수 `Scaling.Stretch` 계약을 지키고, app이 present한 buffer/scene/target은 항상 exact size
- 선언된 CompositionScaleX/Y와 X/Y inverse matrix 적용 시점을 같은 token으로 확인
- flush/submit→GPU completion→reference release→`ResizeBuffers` 순서 확인
- resize 후 `SetSwapChain` 재연결과 resource lifetime 확인
- left/right/top/bottom/four corners 각각 실제 drag
- panel 변화와 다음 exact present 사이 compositor stretch frame/지속 시간을 별도 계측

app-presented geometry가 깨지면 `SetSourceSize`로 넘어가지 않는다. app present는 모두 exact인데 잔여 compositor stretch가 사용자 기준을 넘으면 WIN-4 host architecture spike로 분기한다.

### WEB-1 — provisional geometry + exact commit

- observer callback 안에서 backing reset→clear/crop 사이 async gap 0
- 같은 DPR provisional `scaleX == scaleY == 1`
- DPR 변경 때만 `scaleX == scaleY == newDpr/oldDpr`
- exact commit source/destination 동일 크기
- JS entry마다 scissor/framebuffer/viewport/mask state가 명시되고, JS→Skia 전 dirty bit에 따라 `ResetContext(All)` 수행
- blank full-background-only frame 0, GL error/FBO incomplete 0
- context loss/restore terminal exactly once

WEB-1이 통과한 뒤에만 rAF 수를 줄인다.

### WIN-2 — optional capacity/SetSourceSize spike

- baseline과 별도 feature flag/실험 executable로 수행
- 과거 내부 배율 이상을 재현하는 fixed-height/fixed-circle visual oracle 포함
- 모든 DPI와 left drag를 통과할 때만 제품 경로 후보
- 실패하면 exact-size baseline으로 즉시 복귀하고 실패 evidence 보존

### PERF — latency 최적화

- correctness gate를 그대로 둔 상태에서 target→metrics, metrics→build, build→raster, raster→present를 분리 측정
- Web은 observer→provisional, observer→exact commit을 분리 측정
- `Present`/rAF completion을 실제 display ACK로 부르지 않음

## 8. 실제 화면 검증 기준

기존 `exact-size mismatch == 0`과 blank 검사만으로는 부족하다. 다음 기하 oracle을 녹화의 모든 frame에 적용한다.

| 항목 | 실패 기준 |
| --- | --- |
| AppBar 높이 | DPR로 정규화한 logical 높이가 안정 frame 대비 1 px 초과 변동 |
| 원형 control | 관측된 raster bounding box의 width/height 차이가 `max(1 physical px, ceil(DPR))` 초과 |
| 글자/아이콘 | X/Y scale ratio 불일치 |
| exact frame 우측 끝 | scene AppBar 끝과 current client 끝이 1 physical px 초과 불일치 |
| Web provisional frame | stretch 금지. expansion background strip은 generation lag 동안만 허용 |
| Windows retained interval | app-presented mismatched frame은 0. exact present 전 compositor stretch의 frame 수와 최대 종횡비 오차를 별도 기록 |
| drag 종료 | latest exact frame이 목표 refresh 수 안에 도달하지 못함 |
| blank | startup/context loss 외 full-background-only frame 발생 |

필수 matrix는 다음과 같다.

- Windows: left, right, top, bottom, 네 corner를 각 10초
- Chrome: 동일 border drag와 maximize/restore, 빠른 확대/축소
- DPI: 100/125/150/200%, 서로 다른 DPI monitor 이동
- refresh: 60/120/144/165 Hz에서 가능한 장비
- stress: Web 4× CPU slowdown, Windows 빠른 방향 전환
- lifecycle: minimize/restore, context/device loss, 종료 cleanup

Windows live validator는 `Left`, `Right`, `Top`, `Bottom`과 네 corner를 선택할 수 있고, 이번 사용자 증상은 `HTLEFT` 자동 drag로 직접 검증한다. Web의 40-sample smoke는 fixed-height와 raster-tolerant aspect oracle을 함께 사용한다.

correctness gate에서는 다음 counter가 모두 0이어야 한다.

```text
presentedSceneMetricsMismatch
presentedSceneTargetMismatch
presentedRootSizeMismatch
presentedSurfaceMismatch
newerTargetKnownAtPrePresent
nonUniformProvisionalScale
unterminatedFrames
queueDepthOverTwo
```

`targetAdvancedDuringPresent`와 presented ACK의 generation lag는 UI-thread final target gate 이후 correctness counter로 0을 요구한다. 값이 생기면 UI dispatcher 안에서 새 `SizeChanged`가 먼저 처리된 뒤 오래된 frame이 커밋된 것이므로 실패다.

Web expansion 중 background strip 자체는 correctness 실패가 아니다. 그것은 “아직 exact reflow가 없음”을 왜곡 없이 표시한 상태다. 다만 그 지속 시간은 별도 latency gate로 제한하고, drag 종료 뒤 안정 exact frame에는 남아 있으면 안 된다. Windows에서는 현재 composition API가 retained buffer를 stretch할 수 있으므로 같은 strip 정책을 보장한다고 기록하지 않는다. capture에서 잔여 stretch가 허용 기준을 넘으면 WIN-4로 분기한다.

## 9. 최종 권장 우선순위

1. 공통 build token과 submit-time relabel 제거
2. Windows panel payload authority + full exact matcher + scheduler/latest pre-present gate
3. Web ResizeObserver synchronous 1:1 provisional commit + exact staging commit
4. Windows 필수 `Scaling.Stretch` + exact-size buffer, X·Y matrix, resource lifetime/rebind 검증과 잔여 compositor stretch 계측
5. Web rAF 직렬화 축소
6. 그 뒤에도 Windows latency가 부족할 때만 corrected capacity + `SetSourceSize` spike
7. exact app frame 사이에도 허용 불가능한 Windows stretch가 남을 때만 public presentation host architecture spike

이 순서라면 SkiaSharp fork 없이 현재 두 현상의 직접 원인인 epoch 오인, 이중 size authority, full-frame provisional stretch를 제거할 수 있다. 다만 Windows의 마지막 exact buffer를 panel이 잠시 stretch하는 현상은 현재 composition API 계약의 별도 한계이므로, 실측 결과에 따라 Doroti-owned Windows presentation host를 공개 API로 재구성할 수 있다. Web의 Emscripten handle adapter도 버전 고정 contract test로 격리하되, 장기적으로 public upstream hook을 제안할 여지는 남긴다. 어느 경우도 지금 SkiaSharp fork를 첫 해법으로 선택할 이유는 없다.

## 10. 공식 문서

### Windows / SkiaSharp

- Microsoft Learn, [`SwapChainPanel`](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swapchainpanel)
- Microsoft Learn, [`SizeChangedEventArgs`](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.sizechangedeventargs)
- Microsoft Learn, [`DXGI_SCALING`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling)
- Microsoft Learn, [`IDXGIFactory2::CreateSwapChainForComposition`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition)
- Microsoft Learn, [`IDXGISwapChain::ResizeBuffers`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizebuffers)
- Microsoft Learn, [`IDXGISwapChain2::SetSourceSize`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setsourcesize)
- Microsoft Learn, [`IDXGISwapChain2::SetMatrixTransform`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setmatrixtransform)
- Microsoft Learn, [`DirectX and XAML interop`](https://learn.microsoft.com/en-us/windows/uwp/gaming/directx-and-xaml-interop)
- Microsoft Learn, [`IDXGISwapChain::Present`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-present)
- Microsoft Learn, [`GRBackendRenderTarget`](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.grbackendrendertarget)
- Microsoft Learn, [`SKSurface.Create`](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.sksurface.create)
- Microsoft Learn, [`GRContext.ResetContext`](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.grcontext.resetcontext)

### Web

- WHATWG, [`Update the rendering`](https://html.spec.whatwg.org/multipage/webappapis.html#update-the-rendering)
- W3C/CSSWG, [`Resize Observer`](https://drafts.csswg.org/resize-observer/)
- WHATWG, [`canvas element sizing`](https://html.spec.whatwg.org/multipage/canvas.html#the-canvas-element)
- Khronos, [`WebGL drawing buffer`](https://registry.khronos.org/webgl/specs/latest/1.0/#2.2)
- Khronos, [`WebGL2 blitFramebuffer`](https://registry.khronos.org/webgl/specs/latest/2.0/#3.7.4)

## 11. 2026-08-22 구현 및 재검증 결과

추가 재현에서 문서 단계의 가설보다 더 직접적인 세 가지 결함을 확인해 수정했다.

- Web 시작 blank: `DorotiResizeEpoch`에 생성자가 둘이 된 뒤 브라우저 snapshot JSON의 역직렬화 생성자가 모호해졌다. scalar DPR 생성자를 명시적인 JSON 생성자로 지정했다.
- Web 비균일 epoch: DPR 전환 중 `devicePixelContentBoxSize`가 새 `window.devicePixelRatio`보다 한 observer tick 늦을 수 있었다. logical size, DPR, physical size가 같은 epoch일 때만 device-pixel box를 채택하고 아니면 `logical * DPR`로 일관된 backing size를 만든다.
- Windows resize lag: raster thread의 final generation 검사 뒤 UI dispatcher에 들어가기 전 새 `SizeChanged`가 처리될 수 있었다. panel size 적용과 `Present()` 바로 직전 UI thread에서 generation을 다시 확인하고, 오래된 frame은 terminal superseded로 끝낸다.

현재 자동 검증 결과는 다음과 같다.

- 공통 contract: `PASS`, stale present 0, UI commit 직전 race 방지 permutation 포함, Web/Windows host build 경고·오류 0.
- Chrome 40-sample: `PASS`, blank 0, stale front commit 0, AppBar geometry failure 0, 관측 원형 geometry failure 0. 증거: `web-resize-chrome-20260822-102424.summary.json`.
- Windows 200% DPI `HTLEFT` 10초: `PASS`, 591 target, exact size mismatch 0, lagged ACK 0, target-advanced-during-present 0. UI commit 직전 새 target을 발견한 12 frame은 표시하지 않고 superseded 처리했다. 증거: `rsz0b-default-left-20260822-102343.summary.json`.

이 검증은 runtime/geometry 계약을 증명하지만 최종 사용자 가시성 전체를 대신하지 않는다. Windows GDI 캡처는 DXGI composition swap chain을 담지 못해 실제 scan-out 녹화는 `notVerified`이고, 다른 edge·DPI·120–165 Hz·실기기 브라우저 border drag matrix도 아직 실행하지 않았다.

## 12. 2026-08-22 사용자 재검증 이후 구조 수정

11절의 세대 gate만으로는 사용자가 보는 문제가 개선되지 않았다. 이어서 구현한 capacity back buffer + `SetSourceSize` 실험은 자동 trace상 빠르더라도 Windows 화면에서 UI가 viewport보다 커져 넘치는 것이 사용자의 직접 관찰로 확인되었다. 이 실험은 실패로 판정하고 제품 코드에서 철회했다. `SetSourceSize`가 저비용 effective resize API라는 사실은 현재 SwapChainPanel/Skia 좌표 조합의 가시적 정확성을 보장하지 않는다.

Windows는 다음 이중 exact swap-chain 방식으로 교체했다.

1. panel에 붙은 active swap chain은 다음 exact frame이 완성될 때까지 resize하지 않는다.
2. 분리된 staging swap chain만 새 physical target과 정확히 같은 크기로 `ResizeBuffers`하고 Skia raster를 수행한다.
3. UI-thread final generation gate를 통과하면 staging chain을 먼저 `Present(0)`하고 `ISwapChainPanelNative.SetSwapChain`으로 panel binding을 교체한다.
4. 이전 active chain은 다음 staging chain으로 재사용한다. capacity/source rectangle과 scene viewport를 섞지 않는다.

검증기도 짧은 green trace가 가시 문제를 가리는 일을 줄이도록 바꿨다.

- Windows 기본값은 60초 연속 drag + 15초 관찰이며 `-KeepWindowOpen`으로 사용자가 닫을 때까지 유지할 수 있다.
- 반복 횟수가 아니라 실제 wall-clock으로 종료하고, left/right/top/bottom drag는 작업영역 바깥으로 확장하지 않고 안쪽 축소→원위치로 움직인다. 작업영역 이탈 sample은 correctness failure다.
- Web baseline은 `Emulation.setDeviceMetricsOverride`만 쓰지 않고 실제 Chrome window bounds를 변경한다. 기본 300 samples, 16ms 요청 간격, 종료 전 10초 관찰을 사용한다.
- Web provisional presentation은 target 크기로 default framebuffer를 즉시 만들고 이전 exact front의 교집합만 1:1 crop blit한다. CSS/display 크기가 달라져도 이전 bitmap을 확대하지 않는다.

현재 자동 결과는 다음과 같다.

- Windows 200% DPI `HTLEFT` 10초 + 15초 관찰: dual exact staging backend, 384 targets, 작업영역 이탈 0, exact size mismatch 0, lagged ACK 0, target-advanced-during-present 0. 증거: `rsz0b-default-left-20260822-104758.summary.json`.
- Chrome 실제 window-bounds 120 samples + 5초 관찰: blank 0, stale front commit 0, AppBar geometry failure 0, provisional scale failure 0. 증거: `web-resize-chrome-20260822-104847.summary.json`.

Windows 내부 UI overflow가 사라졌는지는 자동 trace로 확정하지 않는다. capacity 실험을 실패로 만든 것처럼 사용자의 직접 관찰 또는 DXGI를 포함하는 Windows Graphics Capture가 최종 gate다. dual exact staging 실행은 자동 correctness만 통과한 상태이며 실제 화면 acceptance는 `notVerified`다.

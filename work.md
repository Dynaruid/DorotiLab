# Windows/Web resize continuity 재설계 계획

작성일: 2026-08-21

## 0. 2026-08-21 실행 결과

- `RSZ-0`: **failed (중단 게이트 적용)**
  - 공통 `ResizeEpoch`/bounded trace 스키마와 Windows/Web 계측 코드는 구현했다.
  - Release 정적 빌드는 Windows host와 Web host/TypeScript 모두 PASS다.
  - Windows 자동 보조 resize 227회에서 target/build/raster/aggregate swap boundary/`DwmFlush`/ACK를
    generation 1~227로 연결했고 generation 역행과 synchronous miss는 0회였다.
  - 당시 구현은 공개 `SKGLView`의 `InvalidateSurface()` 전체 시간만 측정해 `eglSwapBuffers` 전/후 대기를
    분리하지 못했고, EGL swap interval도 요청하지 않았다. 따라서 아래 RSZ-0 중단 조건에 따라
    최적화 단계로 진행하지 않았다.
  - 자동 resize 중 작은 창에서 `RenderFlex` overflow assertion이 발생했으므로 이 실행 자체도 live PASS가 아니다.
- 후속 API/고정 소스 재검토에서 Windows `PaintSurface`가 current EGL context 안에서 호출되고,
  public `SKSwapChainPanel`/`SKGLViewHandler` 확장점과 패키지의 `libEGL.dll` export를 이용하면
  SkiaSharp 포크 없이 swap interval 요청과 마지막 swap 구간 계측이 가능함을 확인했다.
  이 결과는 아직 비교 실행하지 않았으므로 기존 `RSZ-0 failed`를 PASS로 바꾸지 않고 `RSZ-0B`로 보강한다.
- `RSZ-0B`, `WIN-RSZ-1` ~ `WIN-RSZ-3`, `WEB-RSZ-1` ~ `WEB-RSZ-3`, `CROSS-RSZ`: **notVerified**
- 실제 10초 마우스 드래그/화면 녹화, browser-live, 120/144 Hz, 다른 플랫폼 실기기 항목: **notVerified**
- 증거: `Doroti/validation/evidence/resize/rsz0-gate-2026-08-21.json`
- 재실행: `pwsh -NoProfile -File ./Doroti/eng/validate-resize-continuity.ps1 -Shard Contract`

## 1. 목표와 현재 판정

사용자가 실제 창/브라우저 크기를 연속으로 조절할 때 다음 두 문제를 해결한다.

- Windows: 깜빡임은 없어졌지만 내부 렌더링이 창 경계를 30 fps 미만으로 뒤쫓는 듯한 버벅임이 남아 있다.
- Web: 조절 직전 크기와 현재 크기가 번갈아 적용되는 듯 캔버스 크기가 출렁인다.

이번 문서는 현재 코드, SkiaSharp 4.151.1(package source commit
`279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764`)의 플랫폼 View 동작, 로컬 Flutter 기준 revision
`56b8e1a851a594b1a154f8ea93270807dab22b9a`의 Windows/Web resize 경로를 비교해 만든 구현 계획이다.
RSZ-0A의 정적/자동 계측만 실행됐고 실패 판정은 유지한다. 이번 API 재검토로 추가한 RSZ-0B 이후 단계와
실기기/실브라우저 성능 게이트는 모두 `notVerified`이다.

결론부터 말하면 **SkiaSharp 코어와 Views를 포크하지 않고 진행한다.** 현재 사용 중인
`SkiaSharp.Views.Maui.Controls`와 `SkiaSharp.Views.Blazor`가 Doroti에 필요한 resize generation과
프레임 mailbox를 직접 제공하지는 않지만, Windows에는 공개 View/handler 확장점과 표준 EGL ABI가 있고
Web은 공개 SkiaSharp 코어 API 위에 Doroti 전용 canvas component를 둘 수 있다.

- Windows 1순위: 기존 `SKGLView` 수명주기 안에서 작은 EGL interop으로 swap interval 가설을 검증한다.
- Windows 2순위: public `SKGLViewHandler`와 `SKSwapChainPanel`을 상속해 flush 이후부터 마지막 swap 반환까지를 계측한다.
- Windows 3순위: private resize 선행 swap과 UI thread 소유권이 병목이면 Doroti-owned ANGLE surface로 이동한다.
- Web: private `SKHtmlCanvas` monkey patch를 제거하고 Doroti가 size authority/rAF/WebGL surface를 소유한다.
- 공개 확장점이 더 필요하면 장기 포크 대신 upstream API 제안/PR과 정식 패키지 갱신으로 해결한다.

## 2. 소스 검토 결과

### 2.1 Windows

현재 경로는 `WM_SIZE` 한 번마다 UI thread에서 거의 모든 일을 직렬로 수행한다.

1. `WindowsResizeContinuityGuard`가 `WM_SIZE`/`WM_DPICHANGED`를 받는다.
2. `UpdateLayout` 후 새 metrics로 framework frame을 즉시 만든다.
3. `SKGLView.InvalidateSurface()`로 paint와 ANGLE swap을 같은 호출 흐름에서 진행한다.
4. 그 직후 UI thread에서 `DwmFlush()`까지 기다린다.

이 구조는 크기 일치는 보장하기 쉬워도 build/raster/swap/compositor wait가 매 `WM_SIZE` 처리를 막는다.
현재 10 ms invalidation 간격도 native swap과 DWM 대기가 직렬이면 60 Hz를 보장하지 못한다.

SkiaSharp 4.151.1의 고정 소스에서 demand rendering(`HasRenderLoop=false`) 중 resize가 발생하면 실제 순서는
다음과 같다.

1. `glesContext.MakeCurrent()`
2. `pendingSizeChange`이면 그리기 전에 `eglSwapBuffers()`
3. `SKSwapChainPanel.OnRenderFrame()`에서 `PaintSurface` callback
4. callback 반환 후 `canvas.Flush()`와 `GRContext.Flush()`
5. 그리기 뒤 `eglSwapBuffers()`

따라서 resize frame의 `InvalidateSurface()` 구간에는 최대 두 번의 swap 대기가 포함될 수 있다.
이는 실제 30 fps 원인으로 아직 입증되지 않았지만 기존의 EGL/DWM 이중 대기 가설보다 먼저 분리해서
측정해야 하는 고우선순위 가설이다.

관리 `SKGLView` API에는 swap interval property나 post-swap event가 없지만 포크가 필요하다는 뜻은 아니다.
`AngleSwapChainPanel.OnRenderFrame`은 protected virtual이고 `SKSwapChainPanel`과 `SKGLViewHandler`는 public이다.
또한 `PaintSurface`는 EGL context가 current인 동안 실행되므로 Doroti 전용 P/Invoke에서
`eglGetCurrentDisplay`, `eglGetCurrentSurface`, `eglSwapInterval`, `eglGetError`를 호출할 수 있다.
현재 패키지의 `libEGL.dll`에 이 네 export가 존재함도 확인했다. EGL에는 적용된 interval을 일반적으로
다시 읽는 API가 없으므로 trace에는 `requested interval`, 호출 성공 여부, EGL error를 기록하고 이를
"실제 interval 조회값"이라고 부르지 않는다.

Flutter Windows는 다음처럼 책임을 분리한다.

- `OnWindowSizeChanged`: 새 목표 크기를 기록하고 metrics를 전달한다.
- `OnFrameGenerated(width, height)`: raster thread에서 목표와 정확히 같은 크기의 프레임만 받아 surface를 바꾼다.
- `OnFramePresented`: 해당 프레임의 swap 뒤에 resize 완료를 알린다.
- DWM composition이 동기화를 제공할 때는 EGL vsync를 끄고, 정확한 크기 프레임이 swap된 뒤 raster thread에서
  `DwmFlush`를 사용한다.
- platform thread는 resize 동안 최대 100 ms 단위로 응답을 유지하지만 build/raster/context를 직접 소유하지 않는다.

즉 Windows의 근본 수정은 `WM_SIZE` callback 수를 더 늘리는 것이 아니라 **target size, generated frame,
swap/present ACK를 같은 generation으로 묶고 UI thread에서 GPU/compositor 대기를 분리하는 것**이다.

### 2.2 Web

현재 Web에는 동일한 canvas 크기를 바꾸거나 관찰하는 주체가 겹쳐 있다.

- CSS의 `.doroti-root > canvas { width: 100%; height: 100% }`
- SkiaSharp Blazor `SKGLView` 내부 `SizeWatcher`의 canvas `ResizeObserver`
- `doroti.web.ts`의 canvas `ResizeObserver`와 별도 `window.resize` listener
- `installCanvasResizeContinuity`가 가로챈 `SKHtmlCanvas.requestAnimationFrame(width, height)`의 backing-store 변경
- `resizeHost`가 쓰는 canvas inline logical width/height

특히 backing `canvas.width/height` 변경은 canvas의 관찰 크기, SkiaSharp managed `canvasSize`, Doroti의
`surfaceGeneration`과 서로 다른 시점에 반영된다. 그 사이 각 observer가 다시 snapshot과 frame을 만들 수 있어
이전 크기 A와 최신 크기 B가 교대로 보이는 현상과 구조적으로 일치한다. callback이나 debounce를 하나 더
붙이는 방식은 이 feedback loop를 없애지 못한다.

Flutter Web은 다음 원칙을 쓴다.

- custom host element 하나만 `ResizeObserver`로 관찰하고 DPR 변화는 별도 입력으로 합친다.
- browser `requestAnimationFrame`은 한 개만 pending 상태로 둔다.
- render queue는 `current + latest next`만 보존하고 중간 요청은 대체한다.
- 한 프레임의 physical size를 snapshot한 뒤, 같은 크기로 visible canvas/surface를 갱신하고 그 프레임을 raster한다.

Blazor `SKGLView`는 public이지만 `SizeWatcherInterop`, `SKHtmlCanvasInterop`, WebGL context/FBO,
`OnRenderFrame`, canvas `ElementReference`는 private/internal이다. 상속만으로 size authority를 교체하는
안정적인 공개 경로는 현재 4.151.1과 확인한 upstream `main` 모두에 없다.

따라서 Web은 private `SKHtmlCanvas` monkey patch를 더 확장하지 않고 **Doroti가 host 기준 크기 epoch와 WebGL
surface를 단일 소유하는 컴포넌트**로 교체한다. managed renderer는 public SkiaSharp 코어 타입
(`GRGlInterface`, `GRContext`, `GRBackendRenderTarget`, `SKSurface`)만 사용한다. 초기 spike에서 패키지의
정적 Web 자산을 bootstrap 용도로 재사용할 수는 있지만 이를 공개 API로 간주하지 않고, 접근 경로와 패키지
revision을 contract test로 고정한다.

## 3. 고정할 공통 계약

모든 플랫폼 변경은 아래 계약을 먼저 코드와 trace schema로 고정한 뒤 진행한다.

```text
ResizeEpoch = {
  generation,
  logicalWidth,
  logicalHeight,
  physicalWidth,
  physicalHeight,
  devicePixelRatio,
  timestamp
}

Target(epoch) -> Build(epoch) -> Raster(epoch) -> Swap/Submit(epoch) -> Ack(epoch)
```

- `generation`은 resize/DPI/context 복구 때만 단조 증가한다. 단순 snapshot 조회는 증가시키지 않는다.
- GPU context/surface를 사용하는 thread를 플랫폼별로 하나로 고정한다.
- mailbox는 `in-flight 1 + latest pending 1`만 허용한다. 새 요청은 pending 중간 프레임을 대체한다.
- raster 시작 시 사용한 epoch와 완료 시 최신 target epoch가 다르면 그 결과를 화면에 present하지 않는다.
- 모든 frame은 `presented/submitted`, `superseded`, `dropped`, `failed` 중 정확히 한 terminal ACK를 가진다.
- Windows의 swap 완료와 Web의 browser 제출 완료를 같은 의미의 `presented`로 부르지 않는다.
  Web에는 실제 display scan-out callback이 없으므로 `submitted`와 다음-rAF presentation proxy를 구분한다.
- 새 backing surface가 생겼지만 새 scene이 아직 없으면 마지막 성공 scene을 최신 epoch로 다시 raster한다.
  이전 크기의 back buffer를 늘여 붙이는 방식은 허용하지 않는다.

## 4. 구현 순서와 중단 게이트

### RSZ-0A. 기존 재현과 계측 기준

상태: **failed**. 아래 완료 항목은 정적/자동 보조 증거이며 live resize PASS가 아니다.

- [x] 공통 `ResizeEpoch`와 bounded trace schema를 추가한다.
- [x] Windows 자동 resize 227회에서 target/build/raster/aggregate swap boundary/`DwmFlush`/ACK를 연결한다.
- [x] Web trace에 size event 출처, epoch, CSS/backing/surface size, rAF id를 기록한다.
- [ ] `eglSwapBuffers` 자체의 전/후 대기를 분리한다.
- [ ] 단순 Demo 화면에서 10초 동안 모서리/변을 마우스로 연속 드래그한 baseline trace와 화면 녹화를 남긴다.

### RSZ-0B. 포크 없는 EGL/API 계측 보강

- [ ] Windows 전용 `WindowsEglInterop`에 `eglGetCurrentDisplay`, `eglGetCurrentSurface(EGL_DRAW)`,
  `eglSwapInterval`, `eglGetError`의 최소 P/Invoke를 둔다. `eglSwapBuffers`는 직접 호출하지 않는다.
- [ ] `PaintSurface` 또는 custom panel의 `OnRenderFrame`에서 current display/surface가 유효함을 확인한다.
- [ ] 현재 DWM composition 상태, 요청 interval(0/1), API 성공/오류, context/surface generation을 기록한다.
- [ ] `DorotiWindowsSwapChainPanel : SKSwapChainPanel`에서 `base.OnRenderFrame()` 반환 직후 `pre-swap`을 기록하고,
  동기 `InvalidateSurface()` 반환 직후 `post-swap`을 기록해 마지막 swap 구간을 분리한다.
- [ ] 위 반환 경계가 유효하도록 `DrawInBackground=false`, `HasRenderLoop=false`를 contract로 고정한다.
- [ ] resize frame의 선행 swap 존재는 소스 계약과 ETW/PresentMon 보조 증거로 별도 표기한다. public callback이
  없는 선행 swap을 마지막 frame의 swap으로 오인하지 않는다.
- [ ] 기존 interval 정책과 0/1 요청 빌드의 10초 실제 resize trace를 같은 환경에서 비교한다.
- [ ] 작은 창의 `RenderFlex` overflow를 제거하거나 재현 범위에서 분리하기 전에는 live PASS로 승격하지 않는다.

중단 조건:

- 각 프레임에서 target/build/raster/swap 크기를 연결할 수 없거나 실제 swap 대기 시간을 분리하지 못하면
  최적화 작업을 시작하지 않고 trace 계약부터 보강한다.
- build 성공, PaintSurface callback, `Flush` 호출만으로 화면 표시 성능을 PASS 처리하지 않는다.
- `eglSwapInterval` 성공 반환은 실제 사용자-visible 표시나 성능 개선의 증거가 아니다.

### WIN-RSZ-1. EGL/DWM 이중 대기 가설 검증과 최소 수정

- [ ] 1A: 기존 `SKGLView.PaintSurface`의 current EGL context에서 swap interval만 요청하는 최소 비교 빌드를 만든다.
- [ ] context/device loss 또는 surface generation 변경 시 정책을 다시 적용하고, 같은 generation에서 불필요하게
  매 draw마다 재호출하지 않는다.
- [ ] 1B: `DorotiWindowsSkiaViewHandler : SKGLViewHandler`가
  `DorotiWindowsSwapChainPanel : SKSwapChainPanel`을 생성하게 하고 MAUI handler로 등록한다.
- [ ] custom panel은 `base.OnRenderFrame()`을 그대로 호출해 기존 Skia surface/flush 동작을 유지하고,
  그 반환 이후만 마지막 `eglSwapBuffers` 직전 경계로 계측한다.
- [ ] `IgnorePixelScaling`, touch, `CanvasSize`, `GRContext`, context destroy/recreate가 기존 handler와 동일하게
  연결되는지 contract test로 고정한다. private `MauiSKSwapChainPanel` cast에 의존하는 mapper는 Doroti handler가 소유한다.
- [ ] DWM composition이 켜져 있으면 Flutter와 같이 `eglSwapInterval(display, 0)`, 꺼져 있으면 1을 사용한다.
- [ ] `DwmFlush`는 동기 `InvalidateSurface()`가 반환한 뒤 one-in-flight worker 한 곳에서만 수행하고 UI thread
  paint/build 경로에서는 제거한다. worker ACK가 다른 generation의 제출과 섞이지 않게 mailbox 규칙을 적용한다.
- [ ] before/after trace로 `eglSwapBuffers`와 `DwmFlush` 각각의 대기 분포를 비교한다.
- [ ] resize 외 일반 animation/scroll의 pacing과 tearing 회귀를 함께 확인한다.

분기 조건:

- 1A만으로 성능 게이트를 통과하고 마지막 swap ACK가 충분히 보장되면 최소 변경을 유지하고 `WIN-RSZ-3`으로 간다.
- 1A의 aggregate 경계가 부족하지만 1B의 public handler/panel 계측이 성능 게이트를 통과하면 1B를 유지한다.
- interval을 0으로 바꿔도 UI thread 직렬화가 병목이거나 post-swap ACK를 보장할 수 없으면 임시 timer를 추가하지
  않고 `WIN-RSZ-2`의 Doroti-owned surface로 진행한다.
- private `pendingSizeChange`에 의한 선행 swap을 제거해야만 게이트를 통과할 수 있다면 reflection이나 runtime patch를
  쓰지 않고 `WIN-RSZ-2`로 진행한다.

### WIN-RSZ-2. Doroti-owned Windows ANGLE surface와 raster thread

- [ ] WinUI/MAUI에는 입력과 layout을 받는 얇은 host만 남기고, ANGLE EGL context/surface와 Skia `GRContext`,
  render target, `SKSurface`는 전용 raster thread가 생성/사용/폐기한다.
- [ ] `WM_SIZE`는 최신 `ResizeEpoch`와 framework metrics만 게시하고 paint/swap/`DwmFlush`를 직접 실행하지 않는다.
- [ ] immutable scene snapshot을 mailbox에 넣고 raster thread가 `current + latest` 규칙으로 처리한다.
- [ ] raster thread는 target과 scene epoch를 확인하고 exact-size surface를 만든 직후 같은 epoch scene을 그려 swap한다.
- [ ] swap 뒤 ACK를 보내고 필요할 때만 raster thread에서 `DwmFlush`한다. stale generation은 swap 전에 폐기한다.
- [ ] Flutter의 resize handshake처럼 move/resize message loop의 응답성을 유지하되, 100 ms 이내 bounded wait/pump는
  정확한 크기 frame ACK를 기다리는 용도로만 사용한다. timeout 시 이전 크기 buffer를 번갈아 present하지 않는다.
- [ ] minimize/0x0, restore, maximize, DPI monitor 이동, device/context loss, 종료 중 in-flight frame을 명시적으로 처리한다.
- [ ] 현재 `WindowsResizeContinuityGuard`의 synchronous `InvalidateSurface + DwmFlush` 경로는 새 surface가 안정된 뒤 제거한다.

SkiaSharp 포크 정책:

- 현재 계획에서는 SkiaSharp 코어와 Views의 source/binary fork를 만들지 않는다.
- 먼저 public `SKGLViewHandler`/`SKSwapChainPanel`, 표준 EGL ABI, public SkiaSharp 코어 API로 구현한다.
- `AngleSwapChainPanel`의 private lifecycle을 재사용할 수 없으면 이를 복사하거나 reflection으로 여는 대신
  Doroti 내부 `WindowsAngleSurface`/native interop adapter가 수명주기를 소유한다.
- 공통 라이브러리에 필요한 확장점은 upstream issue/PR로 제안하고 정식 패키지에 들어온 뒤 사용한다.
- 위 세 경로로도 불가능한 경우에는 구현을 중단하고 별도 설계 승인으로 되돌린다. 포크 승인은 이 문서의
  자동 분기가 아니며 사용자의 명시적 범위 변경 없이는 진행하지 않는다.

### WIN-RSZ-3. Windows 통합과 정리

- [ ] 기존 `MauiHostAdapter`의 10 ms spacing과 synchronous resize 예외 경로를 공통 mailbox scheduler로 합친다.
- [ ] `SKSwapChainPanel` paint 내부의 기존 `canvas.Flush/GRContext.Flush`와 Doroti의 flush가 중복되지 않게 한다.
- [ ] frame trace가 target-to-ACK 지연, stale/superseded 수, surface generation, replay 여부를 보고하게 한다.
- [ ] 창을 닫았을 때 `doroti.ps1 run`의 child process/raster thread/trace writer가 모두 종료되는 회귀도 함께 검증한다.

### WEB-RSZ-1. 크기 authority 단일화

- [ ] canvas 자신이 아니라 `.doroti-root` host element 하나만 `ResizeObserver`로 관찰한다.
- [ ] host content box와 DPR을 한 번에 `ResizeEpoch`로 만들고 logical/physical rounding 규칙을 고정한다.
- [ ] `window.resize`는 직접 generation을 올리거나 size를 쓰지 않는다. DPR 감지 보조 신호로만 사용한다.
- [ ] `resizeHost`가 canvas inline width/height를 쓰는 경로와 canvas observer를 제거한다.
- [ ] snapshot 조회와 show/hide는 resize가 아니므로 `surfaceGeneration`을 올리지 않는다.
- [ ] canvas CSS 크기는 `width/height: 100%` 한 곳에서만 정하고 JS는 backing width/height만 소유한다.

중단 조건:

- 한 resize 동안 size event 출처가 둘 이상 backing store를 쓰거나 generation이 A-B-A로 되돌아가면
  renderer 작업으로 넘어가지 않고 authority 제거를 먼저 끝낸다.

### WEB-RSZ-2. Doroti-owned WebGL canvas surface

- [ ] Blazor `<SKGLView>`와 private `SKHtmlCanvas.requestAnimationFrame` monkey patch를 Doroti 전용 canvas component로 교체한다.
- [ ] `SKGLView` 상속으로 private `SizeWatcherInterop`/`OnRenderFrame`에 접근하거나 reflection으로 여는 방식을 사용하지 않는다.
- [ ] JS가 WebGL context/FBO와 browser rAF를 소유하고, managed 쪽은 SkiaSharp 코어의 `GRContext`, render target,
  `SKSurface`를 명시적으로 생성/재생성하는 얇은 adapter를 둔다.
- [ ] 초기 spike가 패키지의 `_content/SkiaSharp.Views.Blazor/SKHtmlCanvas.js` bootstrap을 재사용하면 이를
  internal implementation dependency로 명시하고 4.151.1 revision/export contract test를 둔다. instance monkey patch는 하지 않는다.
- [ ] 장기 구조는 Doroti의 최소 JS bridge 또는 upstream 공개 scheduler/size hook으로 교체 가능하게 경계를 분리한다.
- [ ] pending browser rAF는 하나만 허용한다. 반복 `ScheduleFrame`은 최신 epoch/scene만 교체한다.
- [ ] rAF 시작 시 latest epoch를 snapshot하고, backing `canvas.width/height`와 render target을 그 크기로 맞춘 직후
  동일 epoch scene을 raster/submit한다.
- [ ] raster/submit 도중 더 최신 epoch가 오면 완료 결과를 latest ACK로 오인하지 않고 다음 rAF를 예약한다.
- [ ] WebGL context lost/restored 때 generation을 올리고 GPU resource를 새 context에 다시 만든 뒤 마지막 scene을 replay한다.
- [ ] `BrowserSkiaCapabilities.CompletePaint`는 user-visible present 의미로 사용하지 않고 `rasterComplete/submitted`로 분리한다.

Web 포크 정책:

- SkiaSharp Blazor assembly나 정적 자산을 수정한 패키지를 만들지 않는다.
- package bootstrap 재사용 spike와 Doroti-owned JS bridge를 별도 빌드로 비교하고, 전자는 package update 시
  contract가 깨지면 명확히 실패하도록 한다.
- upstream에는 external size authority, single-rAF scheduler, before/after-submit hook을 독립 API로 제안한다.

### WEB-RSZ-3. Web render queue와 표시 증거

- [ ] `BrowserHostAdapter.ScheduleFrame`의 callback별 rAF dictionary를 `one pending rAF + current/latest mailbox`로 바꾼다.
- [ ] trace에 epoch별 CSS/backing/surface/raster size와 `submitted/superseded/failed` terminal 상태를 기록한다.
- [ ] next-rAF timestamp와 screenshot/video의 경계 위치를 보조 presentation proxy로 수집하되 실제 scan-out ACK라고 부르지 않는다.
- [ ] Chrome/Edge에서 먼저 direct onscreen WebGL 경로를 안정화한다.
- [ ] 이후에만 OffscreenCanvas/`transferToImageBitmap`/`bitmaprenderer` 경로를 capability gate 뒤의 선택 최적화로 검토한다.
  Firefox/Safari는 기능 및 성능 근거 없이 offscreen 경로를 강제하지 않는다.

### CROSS-RSZ. 다른 플랫폼 회귀 검토

- [ ] 공통 `ResizeEpoch`/mailbox 계약을 Qt, Android, iOS, macOS/Mac Catalyst surface adapter에 대입해
  size authority, context owner thread, stale-frame 폐기, replay 계약의 누락을 표로 남긴다.
- [ ] 이미 별도 native present/swap 계약이 있는 플랫폼은 Windows/Web 구현을 억지로 공유하지 않고 scheduler/trace schema만 공유한다.
- [ ] Qt의 기존 resize continuity 수정이 generation/ACK 계약을 지키는지 재검증한다.
- [ ] Android/iOS/macOS 실기기 또는 native-live resize/rotation 검증을 실행하지 못한 항목은 `notVerified`로 남긴다.

## 5. 성능 및 동작 완료 게이트

아래 기준은 Release, hardware acceleration 상태에서 DorotiDemoApp의 단순 화면으로 측정한다. 개발자 도구를 연 상태,
software renderer, 원격 데스크톱 등 결과에 영향을 주는 환경은 별도로 기록한다.

### Windows

- [ ] `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows`
- [ ] 60 Hz 모니터에서 10초 연속 실제 드래그 중 exact-size ACK 기준 p95 frame interval <= 20 ms,
  p99 <= 34 ms, 50 ms 초과 연속 stall 0회.
- [ ] target과 실제 그려진 content edge 차이가 정상 steady-state frame에서 1 physical pixel 이하이다.
- [ ] 이전/현재 크기 back buffer 교대, 검은 프레임, 사라짐/깜빡임이 0회이다.
- [ ] 변/모서리, 빠른 왕복, maximize/restore, minimize/restore, DPI가 다른 모니터 이동을 통과한다.
- [ ] 앱 창을 닫은 뒤 runner와 child process가 5초 안에 종료된다.
- [ ] 120/144 Hz 환경은 가능한 경우 별도 측정하고, 미실행 시 `notVerified`로 남긴다.

### Web

- [ ] `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web`
- [ ] Chrome/Edge hardware WebGL에서 10초 연속 실제 viewport 드래그 중 rAF 제출 간격 p95 <= 20 ms,
  p99 <= 34 ms, 50 ms 초과 연속 stall 0회.
- [ ] 각 submitted frame의 backing/surface/raster physical size가 해당 epoch와 정확히 일치한다.
- [ ] trace에서 generation이 단조 증가하고 A-B-A 크기 교대 및 stale frame 표시가 0회이다.
- [ ] 80/100/125/150/200% browser zoom, DPR 변경, DevTools responsive resize를 통과한다.
- [ ] Chrome, Edge, Firefox에서 동작 검증하고 Safari는 실행 가능한 macOS 환경이 없으면 `notVerified`로 남긴다.

### 공통 회귀

- [ ] 일반 animation/scroll 입력이 resize 수정 전보다 느려지지 않고 frame mailbox terminal ACK 누락이 0회이다.
- [ ] resize 중 input event, semantics/accessibility, pointer 좌표가 최신 logical size와 일치한다.
- [ ] context loss/recreate와 앱 종료 후 GPU/native resource 및 callback이 남지 않는다.
- [ ] 모든 자동 테스트는 저장소 지침에 따라 20분 timeout을 적용한다.
- [ ] build/test PASS와 화면 resize PASS를 분리해 보고하며, 사람이 확인하지 못한 native-live/browser-live 항목은
  `notVerified`로 유지한다.

## 6. 예상 변경 범위

우선 검토/수정 대상:

- `Doroti/src/Doroti.Host.Maui/WindowsResizeContinuityGuard.cs`
- `Doroti/src/Doroti.Host.Maui/MauiSkiaSurface.cs`
- `Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs`
- 신규 Windows EGL interop와 `SKGLViewHandler`/`SKSwapChainPanel` subclass 파일
- 신규 Windows ANGLE surface/raster-thread/mailbox 파일
- `Doroti/src/Doroti.Host.Web/DorotiSurface.razor`
- `Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs`
- `Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs`
- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- 신규 Web canvas surface component와 resize trace/fixture
- Windows/Web product validator와 resize evidence artifact

변경하지 않을 범위:

- 증상 은폐 목적의 DemoApp 전용 debounce/timer
- SkiaSharp 코어/Views source 또는 binary fork와 private member reflection/monkey patch
- SkiaSharp 코어 drawing API, text/image/runtime-effect 구현
- 화면 표시 증거 없이 validator 숫자만 맞추는 변경
- 다른 플랫폼을 확인하지 않고 동일 native surface 구현으로 강제 통합하는 변경

## 7. 참조 소스

현재 checkout에 고정된 Flutter revision을 기준으로 아래 경로를 계속 비교한다.

- Windows resize handshake: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc`
- Windows swap interval: `reference/flutter-master/engine/src/flutter/shell/platform/windows/egl/window_surface.cc`
- Windows compositor ACK: `reference/flutter-master/engine/src/flutter/shell/platform/windows/compositor_opengl.cc`
- Web single rAF: `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/frame_service.dart`
- Web host size authority: `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/view_embedder/dimensions_provider/custom_element_dimensions_provider.dart`
- Web current/latest queue: `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/rasterizer.dart`
- Web canvas/surface sizing: `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/render_canvas.dart`

SkiaSharp 4.151.1 고정 revision과 공식 API 문서:

- package source revision: `279f93f4ffa7f9fe4e9c0bc298bedc3c9e439764`
- Windows resize/swap 순서: `source/SkiaSharp.Views/SkiaSharp.Views.WinUI/AngleSwapChainPanel.cs`
- Windows Skia flush 순서: `source/SkiaSharp.Views/SkiaSharp.Views.WinUI/SKSwapChainPanel.cs`
- MAUI handler 확장점: `source/SkiaSharp.Views.Maui/SkiaSharp.Views.Maui.Core/Handlers/SKGLView/SKGLViewHandler.Windows.cs`
- Blazor managed View: `source/SkiaSharp.Views/SkiaSharp.Views.Blazor/SKGLView.razor.cs`
- Blazor JS/WebGL bootstrap: `source/SkiaSharp.Views/SkiaSharp.Views.Blazor/wwwroot/SKHtmlCanvas.ts`
- `SKSwapChainPanel` API: <https://learn.microsoft.com/dotnet/api/skiasharp.views.windows.skswapchainpanel?view=skiasharp-views>
- `AngleSwapChainPanel.OnRenderFrame` API: <https://learn.microsoft.com/dotnet/api/skiasharp.views.windows.angleswapchainpanel.onrenderframe?view=skiasharp-views>
- MAUI custom handler: <https://learn.microsoft.com/dotnet/maui/user-interface/handlers/create>
- Blazor `SKGLView` API: <https://learn.microsoft.com/dotnet/api/skiasharp.views.blazor.skglview?view=skiasharp-views>
- Khronos EGL registry: <https://registry.khronos.org/EGL/>
- Win32 `DwmFlush`: <https://learn.microsoft.com/windows/win32/api/dwmapi/nf-dwmapi-dwmflush>

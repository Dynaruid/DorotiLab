# Windows / Web interactive resize 근본 구조 개편 요약

- 기록 폴더 기준일: 2026-08-22
- 요약·보관일: 2026-08-23
- 원본: 삭제한 루트 `problem.md`, `research.md`, `work.md`의 진단·연구·실행 결과를 통합한 역사 기록
- 원본 계획 기준 checkout: `21320c77`
- 보관 시점 HEAD: `753a8df4`
- Flutter source-only 비교 기준: `reference/flutter-master` commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- 최종 상태: **M0/G0와 Windows W0 D3D12 runtime correctness는 PASS, D3D12 ordering 후보와 ANGLE visible ownership 및 G1 visual/cadence는 FAIL, W1 이후와 Web B0~B2는 중단**

이 문서는 2026-08-22에 시작한 원인 분석과 2026-08-23까지 이어진 구현·검증을 압축한 기록이다. 작성 과정에서 build나 live 검증을 다시 실행하지 않았으며, 아래 수치는 삭제 전 세 원본에 기록된 evidence를 보존한 것이다.

## 1. 문서 관계와 최종 해석

- `problem.md`는 제공된 Windows/Chrome 영상과 당시 코드를 대조해 두 플랫폼의 현상이 같은 버그가 아니라고 판정했다.
- `research.md`는 실패한 과거 `SwapChainPanel`/Web multi-queue 경로, Flutter source protocol, 공통 epoch 불변식과 새 host 후보를 비교했다.
- `work.md`는 공통 `FrameTransaction`, Windows W0 native child HWND presenter, 후속 Windows/Web milestone과 ordered gate를 정의하고 실제 실행 결과를 기록했다.

초기 분석의 Windows 핵심은 scene metrics와 surface target의 세대 불일치였고, Web 핵심은 이전 GPU front를 새 viewport 전체에 비등방 stretch하는 정책이었다. 공통 epoch 계약을 구현해 correctness는 개선했지만, Windows의 최종 가시 문제는 **child HWND/border와 matching visible buffer의 전환 위상차**로 남았다. 따라서 green counter와 Release build는 제품 수준의 visible/cadence 완료를 뜻하지 않는다.

## 2. 최초 화면 증거와 근본 원인

### Windows

- 영상에서는 client surface가 오른쪽 끝까지 존재하는데 AppBar만 약 88~100 px 먼저 끝나는 frame이 관측됐다. 이전 buffer 전체가 단순 확대된 모습보다 최신 surface에 이전 폭 layout을 그린 상태와 일치했다.
- native target은 `SwapChainPanel.ActualWidth/ActualHeight`에서 만들었지만 framework metrics는 MAUI `View.Width/Height`를 다시 읽었다. 두 signal과 layout 반영 시점은 하나의 transaction이 아니었다.
- `SkiaSceneRenderer.Submit()`은 scene build가 끝난 뒤 최신 target을 다시 읽어 descriptor를 붙였다. `IsExactFor()`도 `MetricsGeneration`, logical size, root physical size를 검사하지 않아 A metrics로 만든 scene을 B target의 exact frame처럼 승인할 수 있었다.
- target 뒤에는 framework layout/build, exact buffer 준비, Skia raster, GPU submit, DXGI present가 필요했다. stale present를 막는 supersede가 correctness에는 필요하지만 target churn에서는 cadence 손실을 만들었다.
- 왼쪽 border drag는 화면상 오른쪽 끝이 고정되므로 content underfill과 border/content phase 차이가 회색·검은 띠로 더 크게 드러났다.

### Web

- canvas CSS `width/height: 100%`는 viewport와 즉시 변하지만 target sampling은 `ResizeObserver` 뒤 별도 rAF까지 지연됐다. 이 사이 old drawing buffer가 새 CSS box에 맞춰 표시됐다.
- presenter는 새 `canvas.width/height`를 설정한 뒤 이전 front 전체를 새 physical rectangle 전체에 `NEAREST` blit했다. 고정 높이 AppBar가 영상에서 약 92~146 px로 변한 직접 원인이다.
- exact frame까지 `ResizeObserver → sample rAF → framework rAF → presenter rAF`와 JS/.NET async 경계가 중첩돼 찌그러진 provisional frame이 여러 frame 지속될 수 있었다.
- `preserveDrawingBuffer=false`인 default framebuffer는 resize/presentation 뒤 clear될 수 있는데 retained FBO 재-blit 시점이 모든 browser paint를 덮지 못했다. 영상에는 약 0.07초와 0.13초의 background-only 구간이 남았다.

## 3. 고정한 공통 구조 계약

플랫폼별 `FrameTransaction` owner가 아래 상태를 단방향으로 소유한다.

```text
ObservedTarget
  -> MetricsDelivered
  -> SceneBuiltForSameEpoch
  -> ExactBackingStoreReady
  -> VisibleSurfaceCommitted
  -> Presented | Superseded | Failed
```

- host는 logical/physical size, X/Y scale, target generation, metrics generation을 immutable epoch 하나로 만든다.
- framework frame은 build 시작 시 epoch/build token을 캡처하고 submit까지 유지한다.
- submit 또는 present 직전에 scene을 최신 target으로 다시 이름 붙이지 않는다.
- target/metrics generation, logical/physical/root/surface size와 scale이 모두 맞아야 exact다.
- 모든 scene/transaction은 exactly once terminal을 가진다.
- 일반 queue는 `current + latest` 두 칸을 넘지 않는다.
- matching exact backing store 전에는 visible surface를 파괴적으로 resize하지 않는다.
- `Present`, rAF 종료, GPU submit은 scan-out ACK 또는 visible acceptance로 부르지 않는다.
- minimize 0×0, device/context loss, shutdown도 명시적 terminal로 끝낸다.

## 4. 목표 플랫폼 구조

### Windows

제품 목표는 native client HWND의 size/DPI를 단일 authority로 삼고, 같은 epoch의 framework build와 exact offscreen D3D12/Skia raster를 완료한 뒤 단일 HWND flip-model swap chain으로 1:1 GPU copy/present하는 bounded native resize transaction이다.

W0는 다음 경로를 실제 구현했다.

```text
native WM_SIZE/DPI epoch
  -> exact framework scene
  -> exact offscreen D3D12 backing
  -> ResizeBuffers + GPU-only CopyResource
  -> Present(0)
  -> resize-only DwmFlush
  -> transaction completion
```

`CreateSwapChainForHwnd`, `Scaling.None`, flip sequential, buffer count 2, maximum frame latency 1을 사용했다. CPU readback과 dual visible swap chain은 사용하지 않았다.

### Web

목표는 `ResizeObserver`가 latest epoch만 게시하고 하나의 rAF가 managed metrics/build/raster와 exact visible commit을 끝내는 `BrowserFramePump`다.

- primary: `OffscreenCanvas`에서 exact raster 후 `ImageBitmapRenderingContext`로 atomic visible transfer.
- fallback: 같은 rAF 안에서 exact staging FBO를 visible WebGL canvas로 1:1 blit.
- exact frame 전에는 이전 canvas의 자연 크기를 유지하고 expansion은 root background, shrink는 crop으로 표시한다.
- 같은 DPR에서 이전 전체 frame의 stretch는 금지한다.
- pointer/semantics 좌표 authority는 최신 root epoch로 유지한다.
- CPU readback, `preserveDrawingBuffer=true`, CSS mask/transform, full-frame provisional stretch는 금지한다.

이 Web B0~B2 구조는 Windows G1 ordered gate 실패로 구현하지 않았다.

## 5. 구현 및 gate 결과

| 범위 | 상태 | 보존할 판정 |
| --- | --- | --- |
| M0 공통 `FrameTransaction` | **PASS** | `Observed → Metrics → Scene → Backing → Visible → Terminal`, 요청 epoch frame dispatch, scene/backing/visible 분리 구현 |
| G0 contract/build | **PASS** | contract v4 22/22 terminal, unterminated 0, queue depth 2, stale present 0, illegal transition 0, backing mismatch 0. Windows/Web/Linux-Qt/Android/macOS target Release 경고·오류 0 |
| macOS demo packaging | **failed, 별도 환경 경계** | Windows에 `sips`가 없어 packaging 실패. macOS host/target compile PASS와 구분 |
| W0 D3D12 runtime correctness | **PASS** | 단일 child HWND/swap chain, exact offscreen backing, fence, GPU copy, `Present(0)`, bounded wait가 실제 실행됨 |
| G1 Windows visual | **FAIL** | blank와 원형 aspect failure는 0이지만 drag 중 content-edge gap이 남음 |
| G1 Windows cadence | **FAIL** | 165 Hz에서 app present 23.0~32.1 Hz, 모든 inter-present interval이 2 refresh 초과 |
| D3D12 pre-present ordering 후보 | **FAIL, 철회** | 420/640 logical WGC에서 gap 또는 cadence 악화 |
| ANGLE/EGL fallback | **FAIL, active 경로에서 제거** | swap/trace는 성공했지만 WinUI/DWM visible content가 어둡고 투명함 |
| Windows W1 제품 승격 | `notStarted` | W0 spike의 demo 연결을 제품 cutover로 보지 않음 |
| Windows W2 input/IME/semantics/lifecycle | `notVerified` | mouse 일부 회귀는 복구했지만 전체 acceptance 미실행 |
| Windows W3 구 경로 삭제 | `notStarted` | G1 실패로 중단 |
| Web B0/B1/B2 | `notStarted` | ordered G1 gate 실패로 미실행 |
| G2 Windows 제품 acceptance | `notVerified` | 7방향, DPI/monitor, lifecycle, full input/accessibility 미실행 |
| G3/G4 Web smoke·실사용 acceptance | `notVerified` | 새 single-rAF 구조 기준 검증 미실행 |

G0의 macOS 항목은 Windows에서 가능한 target compile까지만 PASS다. `sips`가 필요한 bundle/package와 macOS native 실행 증거가 아니다.

## 6. Windows W0 핵심 evidence

작은 창 correctness run `win-rsz-default-left-20260823-111737-d0ab1ce8`은 target 290, exact present 282, target/surface mismatch 0, ACK generation lag 0, present race 0, framework exception 0이었다. target→ACK p50/p95는 17.155/22.397 ms였다.

같은 200% DPI, 165 Hz, 10초 `HTLEFT` WGC에서 크기별 결과는 다음과 같았다.

| 시작 logical/physical 크기 | evidence | 최대 우측 gap | 창 폭 대비 | 최대 지속 | app present |
| --- | --- | ---: | ---: | ---: | ---: |
| 420×300 / 840×600 | `win-rsz-default-left-20260823-111807-ff16239c` | 37 px | 4.4% | 296.976 ms | 23.6 Hz |
| 640×360 / 1280×720 | `win-rsz-default-left-20260823-112035-f9415586` | 25 px | 2.0% | 381.816 ms | 32.1 Hz |
| 1000×600 / 2000×1200 | `win-rsz-default-left-20260823-112113-a2e5af7c` | 28 px | 1.4% | 721.217 ms | 23.0 Hz |

세 run 모두 blank, circle aspect failure, 종료 잔여 gap, exact mismatch, ACK generation lag, present race, framework exception이 0이지만 visual/cadence gate는 FAIL이다. 큰 창이 더 빠른 것이 아니라 비슷한 절대 gap이 작은 창 폭에서 더 큰 비율을 차지해 더 떨려 보였다.

DPI를 보정한 Flutter baseline `flutter-rsz-default-20260823-112152-4dd6a255`는 같은 1280×720/165 Hz left drag에서 최대 gap 8 px/96.966 ms, blank/AppBar/circle/title failure 0이었다. Flutter도 strict cadence 기준은 FAIL했지만 Doroti의 25 px/381.816 ms보다 border-content phase가 명확히 작았다.

surface prepare p50은 1.281~2.361 ms, `DwmFlush` p50은 7.152~7.559 ms였다. 이 값과 크기별 cadence는 Skia draw 하나가 주 병목이라는 가설을 지지하지 않는다.

## 7. 실패한 후속 후보와 rollback

### D3D12 `WM_SIZING` pre-present ordering

`parent WM_SIZING target 선게시 → exact present/DwmFlush → child HWND 확장`을 실행했지만 다음과 같이 악화됐다.

- 420 logical `win-rsz-default-left-20260823-190914-db2d1754`: 최대 gap 39 px, app present 21.0 Hz.
- 640 logical `win-rsz-default-left-20260823-191035-cc5ee44f`: 최대 gap 35 px/557.574 ms, app present 27.7 Hz.

hard failure branch에 따라 1000 logical 재실행 없이 후보를 철회했다. bounded `WM_SIZING` wait만으로 border와 DWM visible commit을 원자화할 수 없고 native target 전달률까지 낮출 수 있다고 판정했다.

### ANGLE/EGL fallback

- SkiaSharp WinUI `libEGL.dll`은 raw child HWND `eglCreateWindowSurface`에서 access violation을 일으켰다.
- desktop ANGLE runtime은 `EGL_FIXED_SIZE_ANGLE`, GLES/Skia GPU backing, 1:1 draw와 `eglSwapBuffers`까지 성공했다.
- 그러나 `STATIC`/`CS_OWNDC` child, direct framebuffer, offscreen draw, raw red clear 어느 것도 WGC의 visible surface에 나타나지 않았다.
- 최종 `win-rsz-default-left-20260823-193141-f4a528b3`은 target/present 300/270, target→ACK p50/p95 12.432/19.065 ms, framework exception 0이었지만 422 capture frame에서 title/circle 관측이 모두 0이었다.

ANGLE package와 active backend identity는 제거했다. 재현용 presenter source만 `DOROTI_WINDOWS_ANGLE_SPIKE` opt-in symbol 아래 비활성으로 남겼다. rollback run `win-rsz-default-left-20260823-193739-c1a12e10`에서 D3D12 backend와 실제 scene 복귀를 확인했지만 gap 39 px/351.503 ms, app present 21.2 Hz로 G1은 계속 FAIL이다.

과거 capacity buffer + `SetSourceSize` 실험도 trace latency는 줄었지만 사용자가 내부 UI 확대/overflow를 직접 확인해 제품 경로에서 철회했다. `SwapChainPanel` dual exact staging은 correctness 역사일 뿐 현재 권장 구조가 아니다.

## 8. W0 과정에서 함께 수정한 결함

- Windows exact-scene wake를 UI dispatcher가 아닌 native serial/`AutoResetEvent` coalescer로 전달했다.
- D3D12 copy fence는 제출 값마다 독립 event로 기다리고 CPU confirmed value를 단조 증가시키며 reset 시 이전 제출 상태를 폐기한다.
- frame-latency token은 실제 `Present` 직전에만 소비한다.
- exact scene miss는 `WM_SIZE` transaction을 조기 완료하지 않으며 100 ms 뒤 retire된 generation은 늦게 present하지 않는다.
- runtime-effect input pool은 shrink 뒤 큰 surface subset을 재사용하지 않고 exact size만 재사용한다.
- monitor를 실제 소유한 DXGI adapter를 선택했고 해당 장비에서는 AMD Radeon 780M이 선택됐다.

일반 `doroti.ps1 run`에서 cold first scene이 100 ms보다 늦으면 generation 1을 retire해 present 0으로 고착되는 startup blank도 수정했다. 첫 visible frame 전 initial `WM_SIZE`는 비동기로 진행하고 첫 성공 present 이후 bounded wait를 적용한다. `cli-run-exact-command` WGC의 146/146 frame에서 title/circle이 보였고 blank/final gap은 0이었다. 이는 startup visibility만 PASS이며 장시간 G1 판정을 바꾸지 않는다.

native child HWND 도입 후 생긴 다음 회귀도 복구했다.

- title/caption 가림: child를 `AppWindow.TitleBar.Height` 아래에 배치하고 실제 `WM_SIZE`를 WinUI에 전달했다.
- validator 입력 잔류: 예외 경로에서도 `LEFTUP`, `WM_CANCELMODE`, timer/thread-priority cleanup을 수행한다.
- mouse 단절: render child가 Win32 mouse/capture/leave/cancel을 직접 `MauiSurfacePointerData`로 변환하며 non-client cursor는 `DefWindowProc`에 맡긴다.

6회 click, resize 뒤 click, drag-out release 뒤 재진입 click은 실제 OS mouse 입력으로 통과했다. 그러나 keyboard/IME, touch/pen, accessibility, 다른 DPI/monitor와 W2 전체는 계속 `notVerified`다.

## 9. Web의 보존할 과거 결과와 현재 경계

2026-08-22에는 Web JSON epoch 역직렬화 모호성, DPR 전환의 비균일 epoch와 이전 front의 1:1 crop/background provisional 정책을 수정했다. 당시 Chrome 40-sample과 실제 window-bounds 자동 smoke는 blank/stale/AppBar/provisional-scale counter 0으로 correctness PASS를 기록했다.

그러나 이는 과거 retained multi-queue presenter의 자동 geometry 결과다. 실제 pointer border drag, browser compositor cadence, Firefox, visible acceptance를 증명하지 않았고 이후 계획한 single-rAF/OffscreenCanvas 구조 B0~B2도 실행하지 않았다. 따라서 과거 smoke PASS를 현재 Web 완료나 새 구조의 G3/G4 PASS로 승격하지 않는다. 활성 smoke 표본 수는 40이며 과거 600회 기준은 사용하지 않는다.

## 10. 검증 원칙과 남은 acceptance

- correctness, cadence, visual을 별도 판정한다.
- Windows는 fixed-height AppBar, 원형 control, content right edge, blank를 WGC 모든 frame에 적용하고 Flutter를 같은 장비·DPI·refresh·edge·input 조건에서 비교한다.
- Web은 자동 screenshot smoke와 실제 browser compositor/border drag acceptance를 구분한다.
- 실제 input sample, native target, completed transaction과 present 수를 함께 기록한다.
- build/process/contract/CDP 성공은 visible UI, compositor, scan-out을 증명하지 않는다.

아직 남은 범위:

- Windows 7방향 drag, 100/125/150/200% DPI와 monitor 이동, 60/120/144/165 Hz matrix.
- minimize/restore, maximize/restore, 빠른 방향 전환, occlusion, device loss/recovery.
- pointer 외 keyboard/Korean IME/touch/pen/focus/semantics/accessibility 전체.
- W1 제품 host 승격, W2 ownership 이관, W3 구 presentation 경로 삭제.
- Web B0 single-rAF, B1 atomic transfer/fallback, B2 구 multi-queue 경로 삭제.
- Chrome/Edge/Firefox 실제 border drag, zoom/DPR, CPU slowdown, context loss/restore.

## 11. 최종 중단점과 재개 방향

최종 판정은 다음과 같다.

```text
M0/G0                                      PASS
Windows W0 D3D12 runtime correctness      PASS
D3D12 pre-present ordering candidate      FAIL
ANGLE visible ownership                   FAIL
G1 Windows visual/cadence                 FAIL
W1~W3, Web B0~B2, G2~G4                  notStarted / notVerified
```

두 W0 fallback이 모두 실패했으므로 기존 `WM_SIZING` ordering을 반복하거나 timeout/debounce를 미세 조정하는 것이 재개점이 아니다. 다음 단계에는 **WinUI composition과 raw HWND GPU surface의 visible ownership을 성립시키는 별도 native host 설계 또는 다른 presenter ownership 결정**이 먼저 필요하다. 그 구조 후보가 정해지기 전에는 W1 제품 승격이나 Web milestone으로 진행하지 않는다.

## 12. 주요 재현 명령

```powershell
pwsh -NoProfile -File Doroti/eng/validate-resize-continuity.ps1

pwsh -NoProfile -File Doroti/eng/validate-resize-continuity-live.ps1 `
  -SwapInterval default `
  -DurationSeconds 10 `
  -PostDragObservationSeconds 2 `
  -Edge Left `
  -InitialLogicalWidth 420 `
  -InitialLogicalHeight 300 `
  -RetainRawTrace `
  -WindowsGraphicsCapture `
  -PngStride 10 `
  -AllowCadenceFailure `
  -AllowVisualFailure

pwsh -NoProfile -File Doroti/eng/validate-flutter-windows-resize-baseline.ps1 `
  -Renderer Default `
  -DurationSeconds 10 `
  -PngStride 10 `
  -SkipBuild `
  -CollectAcceptanceFailures `
  -Edges Left

pwsh -NoProfile -File Doroti/eng/doroti.ps1 run `
  -App DorotiDemoApp `
  -Platform windows
```

모든 test command timeout은 저장소 지침에 따라 최대 20분이다.

> 문서 성격: 삭제한 루트 `problem.md`, `research.md`, `work.md`의 2026-08-22~23 Windows/Web resize 진단·실행·실패 경계 요약. 새로운 active plan이나 제품 완료 선언이 아니다.

# Windows / Web interactive resize 근본 구조 개편 작업계획

## 0. 문서 상태와 작업 경계

- 작성일: 2026-08-23
- 현재 checkout 기준: `21320c77`
- Flutter 비교 기준: `reference/flutter-master`의 고정 commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- 상태: **W0 fallback까지 실행 후 G1 중단, Windows chrome/mouse-input 회귀 복구** (2026-08-23). M0/G0와 D3D12 offscreen-copy runtime correctness는 PASS했지만, pre-present expansion ordering과 ANGLE/EGL fallback이 모두 strict visual gate를 넘지 못했다. 이후 발견된 타이틀바 가림, 검증 종료 후 resize 입력 잔류, raw child HWND의 mouse pointer 단절은 복구했지만 G1 visual/cadence는 계속 FAIL이다. Ordered hard gate에 따라 W1 이후와 Web milestone은 진행하지 않았다.
- 입력 자료: 현재 `problem.md`, `research.md`, Windows/Web host source, 고정 Flutter Windows/Web engine source, Microsoft/WHATWG/CSSWG 공식 API 문서.
- 범위: Windows와 Web의 viewport metrics → framework frame → GPU backing store → visible present 소유권을 다시 설계한다.
- 공통 epoch/exact-match/terminal ledger는 보존하되, 그것을 가시 문제의 해결 자체로 간주하지 않는다.
- macOS/iOS/Android/Linux의 presenter 구조는 이번에 재설계하지 않는다. 공통 계약 변경으로 인한 build/contract regression만 막는다.
- 모든 test command의 timeout은 최대 20분으로 둔다.

이번 작업의 완료 조건은 counter 추가나 trace green이 아니다. 사용자가 보는 resize에서 고정 높이 UI와 원형 control이 찌그러지지 않고, Windows border와 exact content가 Flutter 기준에 가까운 하나의 transaction으로 움직여야 한다.

### 0.1 2026-08-23 실행 결과

- `M0`: **PASS**. `FrameTransaction`의 `Observed → Metrics → Scene → Backing → Visible → Terminal` 단방향 상태, 요청 시점 epoch frame dispatch, renderer의 scene/backing/visible 분리, Windows/Web deterministic fixture를 구현했다.
- `G0`: **PASS**. resize contract v4는 22/22 terminal, unterminated 0, queue depth 2, stale present 0, illegal transition 0, mismatched backing-store commit 0이다. Windows/Web/Linux-Qt/Android/macOS target Release compile은 경고 0/오류 0이다. macOS demo packaging 전체는 Windows에 `sips`가 없어 별도로 실패했지만 macOS host/target compile은 PASS했다.
- `W0 runtime correctness`: **PASS**. child HWND 한 개, flip-model HWND swap chain 한 개, exact offscreen D3D12 backing store, GPU-only `CopyResource`, `Present(0)`, resize-only `DwmFlush`, 100ms bounded wait가 실제 실행됐다. 작은 420×300 logical run `win-rsz-default-left-20260823-111737-d0ab1ce8`은 target 290, exact present 282, target/surface mismatch 0, ACK generation lag 0, present race 0, framework exception 0이며 target→ACK p50/p95는 17.155/22.397ms다.
- 작은 창 stress에서 가려져 있던 runtime 결함도 함께 수정했다. Windows exact-scene wake는 UI dispatcher를 거치지 않고 native serial/event coalescer로 전달한다. D3D12 copy fence는 제출 값마다 독립 event로 기다리고 CPU confirmed value를 단조 증가시키며 reset 때 제출 상태를 폐기한다. frame-latency token은 실제 `Present` 직전에만 소비한다. exact scene이 아직 없는 raster 시도는 WM_SIZE transaction을 조기 완료하지 않고, 100ms 뒤 retire된 generation은 늦게 present하지 않는다. runtime-effect input pool은 shrink 뒤 큰 surface의 subset snapshot을 재사용하지 않고 exact size만 재사용한다. monitor-owning DXGI adapter를 선택하며 이 장비에서는 AMD Radeon 780M이 선택됐다.
- `G1 visual`: **FAIL**. 크기별 200% DPI `HTLEFT` WGC 결과는 다음과 같다. blank와 circle aspect failure, 종료 잔여 gap은 모든 run에서 0이지만 drag 중 우측 content gap은 남는다.

  | logical / physical 시작 크기 | evidence | max right gap | 창 폭 대비 | max gap duration | app present |
  | --- | --- | ---: | ---: | ---: | ---: |
  | 420×300 / 840×600 | `win-rsz-default-left-20260823-111807-ff16239c` | 37px | 4.4% | 296.976ms | 23.6Hz |
  | 640×360 / 1280×720 | `win-rsz-default-left-20260823-112035-f9415586` | 25px | 2.0% | 381.816ms | 32.1Hz |
  | 1000×600 / 2000×1200 | `win-rsz-default-left-20260823-112113-a2e5af7c` | 28px | 1.4% | 721.217ms | 23.0Hz |

- 사용자의 “큰 창은 거의 안 떨리고 작은 창은 떨린다”는 관찰은 처리량 차이보다 **고정 픽셀 위상차의 상대 크기**로 설명된다. 큰 창은 cadence와 gap 지속 시간이 오히려 나쁘지만 28px가 전체 폭의 1.4%라 덜 보인다. 작은 창에서는 37px가 4.4%이고, WGC frame에서 child HWND의 새 영역이 exact swap-chain buffer보다 먼저 드러나 회색 띠로 직접 관측된다.
- `G1 cadence`: **FAIL**. 세 크기 모두 165Hz에서 app present가 23.0–32.1Hz이고 모든 inter-present interval이 2 refresh를 넘었다. surface prepare p50은 1.281–2.361ms, `DwmFlush` p50은 7.152–7.559ms이므로 Skia draw 하나로 환원할 수 없다.
- DPI 보정을 적용한 Flutter 고정 commit baseline `flutter-rsz-default-20260823-112152-4dd6a255`는 같은 1280×720/165Hz left drag에서 우측 gap 최대 8px/96.966ms, blank/AppBar/circle/title failure 0이었다. cadence strict gate 자체는 FAIL했지만 Doroti의 25px/381.816ms보다 border-content phase가 명확히 작다.
- 후속 W0 후보였던 pre-present expansion ordering과 ANGLE/EGL `EGL_FIXED_SIZE_ANGLE` fallback은 0.3절과 같이 모두 실행했으나 FAIL했다. 실패한 presenter를 기본 제품 경로에 남기지 않았고 demo는 이전 D3D12 W0 경로로 복귀했다.
- 중단 경계: W1 제품 승격, W2, W3, B0, B1, B2, G2, G3, G4는 모두 `notVerified`/`notStarted`다. 현재 demo에 연결된 native/offscreen 경로는 W0 검증용 구현이며 완료된 제품 cutover로 간주하지 않는다.
- 재현 명령: `pwsh -NoProfile -File Doroti/eng/validate-resize-continuity.ps1`, 작은 창은 `pwsh -NoProfile -File Doroti/eng/validate-resize-continuity-live.ps1 -SwapInterval default -DurationSeconds 10 -PostDragObservationSeconds 2 -Edge Left -InitialLogicalWidth 420 -InitialLogicalHeight 300 -RetainRawTrace -WindowsGraphicsCapture -PngStride 10 -AllowCadenceFailure -AllowVisualFailure`, Flutter 비교는 `pwsh -NoProfile -File Doroti/eng/validate-flutter-windows-resize-baseline.ps1 -Renderer Default -DurationSeconds 10 -PngStride 10 -SkipBuild -CollectAcceptanceFailures -Edges Left`다.

### 0.2 2026-08-23 외출 전 종료 인수인계

- 종료 판정: 이번 실행은 `W0 runtime correctness PASS / G1 visual·cadence FAIL`에서 의도적으로 멈췄다. 추가 실험, W1 제품 승격, Web 착수는 하지 않았다.
- 마지막 회귀 검증: `pwsh -NoProfile -File Doroti/eng/validate-resize-continuity.ps1`가 contract v4 22/22 terminal과 Windows/Web Release build 경고 0/오류 0으로 PASS했다. 종료 문서 반영 후 `git diff --check`도 PASS했다.
- 유효한 비교 증거: 작은 창 correctness는 `win-rsz-default-left-20260823-111737-d0ab1ce8`, 크기별 WGC visual은 `win-rsz-default-left-20260823-111807-ff16239c`, `win-rsz-default-left-20260823-112035-f9415586`, `win-rsz-default-left-20260823-112113-a2e5af7c`, Flutter 기준은 `flutter-rsz-default-20260823-112152-4dd6a255`다. 도중 포커스를 잃은 두 실행은 acceptance evidence에서 제외했다.
- 저장 상태: 구현·validator·문서 변경은 현재 작업 트리에 **미커밋** 상태로 보존했다. commit/push는 하지 않았고 관련 파일을 되돌리거나 정리하지 않았다. WGC capture 도구와 Flutter baseline validator도 함께 남겨 두었다.
- 프로세스 상태: 종료 시 `DorotiDemoApp.Windows`, `Doroti.WindowsResizeCapture`, `flutter_tester` 잔여 프로세스는 없다.
- 정확한 재개점: 기존 측정을 반복하는 것보다 먼저 다음 W0 후보인 `parent WM_SIZING suggested expansion target 선게시 → 기존 작은 child에서 exact buffer를 crop 상태로 present → DwmFlush 뒤 child HWND 확장` 순서를 격리된 spike로 구현한다. 이후 위 420/640/1000 logical 크기 matrix를 같은 WGC 조건으로 다시 비교한다.
- 실패 분기: 위 순서가 안전한 단일 HWND/D3D12 ownership으로 성립하지 않거나 WGC gap을 악화하면 더 미세한 timeout/debounce 조정으로 가지 않고 ANGLE/EGL `EGL_FIXED_SIZE_ANGLE` presenter 후보로 전환한다.
- 계속 미검증: 7방향 drag, 다른 DPI/monitor, minimize/restore, device loss, input/IME/accessibility, W1~W3 제품화, Web B0~B2와 G2~G4는 모두 `notVerified`/`notStarted`다. 현재 native/offscreen 경로는 W0 spike이며 제품 완료 상태가 아니다.

### 0.3 2026-08-23 W0 재개 실행과 최종 중단

- `parent WM_SIZING suggested target 선게시 → exact present/DwmFlush 대기 → child HWND 확장` 후보를 구현하고 동일 200% DPI `HTLEFT` WGC로 확인했다. 420 logical run `win-rsz-default-left-20260823-190914-db2d1754`는 최대 gap이 기존 37px에서 39px로 늘고 app present가 23.6Hz에서 21.0Hz로 낮아졌다. 640 logical run `win-rsz-default-left-20260823-191035-cc5ee44f`는 최대 gap이 25px/381.816ms에서 35px/557.574ms로, app present는 32.1Hz에서 27.7Hz로 악화됐다. 따라서 명시된 hard failure branch를 적용해 1000 logical 재실행 없이 D3D12 ordering 후보를 철회했다.
- ANGLE fallback의 첫 public runtime 후보였던 SkiaSharp WinUI `libEGL.dll`은 raw child HWND `eglCreateWindowSurface`에서 access violation으로 사용할 수 없었다. 이어 desktop ANGLE runtime으로 `EGL_FIXED_SIZE_ANGLE`, GLES context, Skia GPU offscreen backing, 1:1 draw, `eglSwapBuffers`를 실행했다. swap/transaction trace와 AMD D3D11 renderer 식별은 성공했지만 WGC에서 Doroti title과 circle이 0 frame 관측되어 visible surface는 계속 어둡고 투명했다.
- `STATIC` child, GPU offscreen draw, raw GL red clear, 마지막으로 `CS_OWNDC` custom child HWND까지 차례로 확인했다. 최종 `win-rsz-default-left-20260823-193141-f4a528b3`은 target/present 300/270, target→ACK p50/p95 12.432/19.065ms, framework exception 0이지만 422 WGC frame에서 title/circle 관측이 모두 0이고 raw red clear도 보이지 않았다. 이는 app trace의 EGL swap 성공이 WinUI/DWM의 visible ownership 성공을 뜻하지 않음을 보여 준다.
- 최종 판정은 `W0 D3D12 runtime correctness PASS / D3D12 ordering candidate FAIL / ANGLE visible ownership FAIL / G1 visual·cadence FAIL`이다. 실패한 ANGLE 코드는 제품 경로와 패키지 의존성에서 제거했고, 재현 참고용 presenter source만 `DOROTI_WINDOWS_ANGLE_SPIKE` opt-in 전처리 심볼 아래 비활성 상태로 남겼다. 기본 demo/validator identity는 기존 `WindowsHwndD3D12Presenter`다.
- rollback 확인 `win-rsz-default-left-20260823-193739-c1a12e10`은 backend가 다시 `Win32/child-HWND/offscreen-copy/DXGI-D3D12-Skia`이고 408 WGC frame에서 title 406, circle 39, blank 0, framework exception 0이었다. 따라서 ANGLE의 어두운 surface가 active 경로에 남지 않았다. 다만 최대 우측 gap 39px/351.503ms, app present 21.2Hz로 G1 visual/cadence는 예상대로 계속 FAIL이다.
- 일반 CLI `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows`에서는 validator와 달리 시작 직후 추가 `SetWindowPos`가 없어 화면이 계속 비는 startup 결함이 있었다. Cold first scene이 100ms보다 늦게 준비되면 유일한 generation을 timeout-retired했고 새 `WM_SIZE`가 없어 present 0으로 고착됐다. 첫 성공 present 전에는 initial `WM_SIZE`를 비동기로 진행하고, coordinator는 성공 completion과 실제 timeout invalidation을 분리하도록 수정했다. 성공한 generation은 같은 크기의 regular frame에 계속 사용할 수 있다.
- 환경변수 없이 위 사용자 명령을 그대로 실행한 `cli-run-exact-command` WGC는 첫 입력 전 저장 frame부터 Material Gallery가 보였고, 146/146 frame에서 title과 circle이 관측됐으며 blank/circle aspect failure/capture error/drop/final gap은 모두 0이었다. 이는 startup visibility 회귀의 수정 증거이며 3초 resize run이므로 기존 장시간 G1 visual/cadence FAIL 판정을 바꾸지 않는다.
- ordered gate에 따라 W1, W2, W3, B0, B1, B2, G2, G3, G4는 실행하지 않았으며 모두 `notStarted`/`notVerified`다. 이 문서의 “전체 작업” 실행은 이 hard gate에서 종료한다. 다음 구조 선택에는 WinUI composition과 raw HWND EGL surface를 결합하는 별도 native host 설계 또는 다른 presenter ownership 결정이 필요하며, 현재 범위에서 timeout/debounce 조정으로 우회하지 않는다.

### 0.4 2026-08-23 Windows chrome/input 회귀 수정

- 사용자 재확인에서 native child HWND가 WinUI client-coordinate title bar까지 `(0, 0)`에서 덮어 제목과 caption button이 보이지 않았고, 부모 subclass가 대화형 resize 중 실제 `WM_SIZE`를 `return 0`으로 삼킨 뒤 `WM_EXITSIZEMOVE`에서 다른 메시지인 가짜 `WM_SIZE`를 `DefSubclassProc`에 넘기고 있었다. child를 `AppWindow.TitleBar.Height` 아래에 배치하고 `Window.ExtendsContentIntoTitleBar=false`를 명시했으며, MAUI 최상위 `Window.Title`을 application title로 설정했다. 부모의 모든 실제 `WM_SIZE`는 WinUI에 그대로 전달한다.
- WGC frame에서 `Doroti Material Demo` title과 maximize/close caption button, 그 아래 분리된 Doroti content가 실제로 관측됐다. 200% DPI에서 top-level client의 title bar는 32 physical px이고 render `STATIC` child는 그 아래에서 시작한다. GDI `CopyFromScreen`은 별도 composition caption control을 누락했으므로 titlebar acceptance 근거로 사용하지 않았다.
- `Doroti.WindowsResizeCapture`는 `SendInput LEFTDOWN` 뒤 `GetWindowRect`/capture 예외가 나도 RAII cleanup이 항상 `LEFTUP + WM_CANCELMODE`, thread priority 복구, `timeEndPeriod(1)`을 수행한다. PowerShell live validator의 `finally`도 `LEFTUP + WM_CANCELMODE` 뒤 창을 닫는다. 실제 10초 run 종료 후 `LeftButtonDown=False`, current cursor=`IDC_ARROW`, 잔여 Demo/capture process 0을 확인했다.
- 첫 회귀 run `win-rsz-default-left-20260823-201303-affacfa1`은 1280×720/200% DPI/165Hz에서 blank/AppBar/circle/title failure 0, final gap 0, capture error/drop 0, framework exception 0이고 최대 우측 gap 13px/521.207ms, app present 31.4Hz였다. 그러나 최종 소스 지문 run `win-rsz-default-left-20260823-201749-69e4186f`은 chrome 복구와 final gap 0, capture error/drop 0, framework exception 0을 유지했어도 transient gap이 94px/2,381.790ms, app present 23.1Hz였다. 따라서 13px 결과를 안정적 개선으로 승격하지 않으며 원래 G1은 계속 FAIL이다.
- resize transaction에서 frame-latency wait를 생략하고 `DwmFlush`만 남기는 후보도 같은 조건으로 실행했으나 `win-rsz-default-left-20260823-201554-439ef492`에서 20px/3,036.379ms/21.3Hz로 악화되어 즉시 철회했다. 최종 경로는 기존 frame-latency wait + resize-only `DwmFlush`를 유지한다.
- 이 수정은 W1 제품 승격이나 G1 PASS가 아니다. W1~W3, Web B0~B2, G2~G4 및 7방향/DPI/monitor/input/IME/accessibility 전체 acceptance는 기존대로 `notStarted`/`notVerified`다.

### 0.5 2026-08-23 Windows mouse interaction 회귀 수정

- 화면 전체를 덮는 raw render child HWND가 mouse hit-test의 실제 대상인데 기존 XAML `_host.Pointer*` handler만 Doroti pointer packet을 만들고 있었다. child를 `HTTRANSPARENT`로 두어도 별도 WinUI content island까지 안정적으로 재전달되지 않으므로 화면은 보이지만 내부 button이 되었다가 안 되는 상태가 발생했다. render child를 `HTCLIENT` input owner로 두고 `WM_MOUSEMOVE`, 좌/우/중 button down/up, wheel, capture/cancel/leave를 `MauiSurfacePointerData`로 직접 연결했다. keyboard/IME focus 경로는 기존 XAML panel에 남겼다.
- mouse protocol도 Win32 message를 단순히 모두 `move`로 바꾸지 않는다. 첫 진입은 `add`, button 없는 이동은 `hover`, 첫 button 접촉은 `down`, capture 중 이동은 `move`, 마지막 button 해제는 `up`, 정상 이탈은 `remove`, capture loss는 `cancel`로 보낸다. drag capture 중 child 밖으로 나간 `WM_MOUSELEAVE`는 active pointer를 제거하지 않는다.
- cursor owner는 top-level `WM_SETCURSOR`에서 hit-test가 `HTCLIENT`일 때만 Doroti client cursor를 적용하고, `HTLEFT` 등 non-client edge/corner는 `DefWindowProc`에 맡긴다. 200% DPI에서 left border와 interior를 세 번 왕복한 결과가 매번 `HTLEFT + IDC_SIZEWE → HTCLIENT + IDC_ARROW`였고 마지막 `LeftButtonDown=False`를 확인했다.
- 새 Release 실행본을 1280×720 physical로 두고 실제 OS mouse 입력을 사용했다. 창 밖으로 이동했다가 button으로 다시 들어오는 수명주기를 매회 포함한 6회 연속 click에서 `nativePointerEvents 0 → 43`, 화면 상태 `Pressed 0 → Pressed 6`을 확인했다. 이어 left border를 40px 실제 drag-resize한 뒤 같은 button을 다시 click해 `nativePointerEvents 53 → 58`, `Pressed 7`을 확인했다. 최종 소스에서는 button을 누른 채 child 밖으로 drag-release한 뒤 다시 들어와 click하는 capture 경로도 `nativePointerEvents=18`, `Pressed 1`, process responding으로 통과했다. 증거 화면은 `.doroti/tmp/native-input-sm-after-six-clicks.png`, `.doroti/tmp/native-input-after-resize-click.png`, `.doroti/tmp/native-input-final-drag-out-click.png`이고 Release build는 경고 0/오류 0이다.
- 이 확인은 Windows mouse hover/click/capture와 resize 뒤 click에 한정한다. keyboard/IME, touch/pen, accessibility, 다른 DPI/monitor와 W2 전체 acceptance는 계속 `notVerified`이며 G1 visual/cadence FAIL 판정도 바꾸지 않는다.

## 1. 최종 구조 결정

### 1.1 근본 원인 판정

현재 공통 문제는 Skia draw 속도가 아니라 **viewport를 정하는 주체, frame을 만드는 주체, 보이는 surface를 바꾸는 주체가 서로 다른 queue와 시점에 있다는 것**이다.

| 플랫폼 | 현재 구조의 핵심 단절 | 폐기할 전제 |
| --- | --- | --- |
| Windows | XAML/MAUI size signal, framework build, detached composition swap-chain 준비, UI dispatcher의 `Present + SetSwapChain`이 각각 독립 진행 | exact-size counter가 0이면 가시 resize도 해결됐다는 전제 |
| Web | `ResizeObserver`, framework rAF, Blazor `InvokeAsync`, presenter microtask, managed raster, default framebuffer blit가 여러 비동기 경계를 통과 | retained FBO만 있으면 browser가 매 paint에 올바른 frame을 표시한다는 전제 |

고칠 구조는 플랫폼마다 하나의 `FrameTransaction` owner를 두고 아래 상태 전이를 소유하게 하는 것이다.

```text
ObservedTarget
  -> MetricsDelivered
  -> SceneBuiltForSameEpoch
  -> ExactBackingStoreReady
  -> VisibleSurfaceCommitted
  -> Presented | Superseded | Failed
```

`DorotiResizeEpoch`과 build token은 transaction의 identity다. stopwatch, debounce, generation 재라벨은 transaction을 진행시키는 제어 입력이 될 수 없다.

### 1.2 Windows 결정

Windows 제품 경로는 다음 구조로 교체한다.

```text
native client HWND의 WM_SIZE / DPI
  -> WindowsResizeTransaction 시작
  -> 동일 epoch metrics + framework build
  -> exact-size offscreen D3D12/Skia backing store에 raster
  -> 단 하나의 HWND flip-model swap chain을 exact size로 준비
  -> offscreen frame을 1:1 GPU copy/blit
  -> Present -> resize 때만 DwmFlush -> transaction 완료
  -> WM_SIZE 처리 복귀
```

핵심은 `native child HWND`만 도입하는 것이 아니다. **scene raster를 visible swap chain resize보다 먼저 끝내고, native resize handler가 matching present까지 제한된 시간 동안 backpressure를 거는 것**이 핵심이다. Flutter Windows의 `kResizeStarted → kFrameGenerated → kDone`, raster-thread surface recreation, `SwapBuffers`, `DwmFlush`, platform task polling을 같은 책임 구조로 옮긴다.

초기 renderer는 ANGLE/EGL로 바꾸지 않고 현재 D3D12/SkiaSharp를 유지한다. 기존 자료에서 Skia raster가 주 병목이라는 근거가 없고, renderer 교체 없이도 host transaction을 바로 검증할 수 있기 때문이다. D3D12 public interop으로 offscreen backing store와 단일 HWND surface의 안전한 1:1 GPU transfer를 만들 수 없을 때만 ANGLE/EGL fixed-size surface를 후속 fallback으로 사용한다.

### 1.3 Web 결정

Web 제품 경로는 다음 구조로 교체한다.

```text
ResizeObserver가 latest epoch만 게시
  -> 하나의 requestAnimationFrame
     -> managed metrics/beginFrame/drawFrame
     -> exact offscreen backing store raster
     -> visible canvas intrinsic/CSS size를 exact frame에 맞춤
     -> ImageBitmap transfer 또는 exact 1:1 WebGL blit
     -> transaction terminal
```

`DorotiWebGlSurface.razor → IJSObjectReference → JS queueMicrotask → DotNetObjectReference.RenderFrame` 왕복을 hot path에서 제거한다. 가능한 브라우저에서는 Flutter Web과 같은 `OffscreenCanvas + ImageBitmapRenderingContext`를 제품 경로로 사용한다. 이 결합을 현재 SkiaSharp WASM context에 안전하게 연결할 수 없는 브라우저에서는 같은 **단일 rAF transaction** 안에서 staging FBO를 visible WebGL canvas에 1:1 blit하는 fallback을 사용한다. fallback도 이전의 다중 queue 구조로 돌아가면 안 된다.

exact frame 전 provisional 정책은 다음 하나로 고정한다.

- visible canvas의 intrinsic/CSS 크기는 마지막 exact frame 크기에 묶어 둔다.
- root가 커지면 남는 영역은 배경으로 보이고, root가 작아지면 `overflow: hidden`으로 이전 frame을 crop한다.
- 같은 DPR에서 이전 frame을 새 root 전체로 stretch하지 않는다.
- pointer/semantics 좌표 authority는 canvas가 아니라 최신 root epoch다.
- 다음 rAF의 exact commit에서 canvas 크기와 bitmap을 함께 바꾼다.

## 2. 변경하지 않을 핵심 불변식

- scene은 build 시작 시 캡처한 `DorotiResizeEpoch`/metrics generation을 끝까지 유지한다.
- `Submit()` 또는 present 직전에 최신 target으로 descriptor를 다시 쓰지 않는다.
- logical size, physical size, scale X/Y, metrics generation, target generation, backing-store size가 모두 맞아야 exact다.
- 모든 scene/transaction은 exactly once terminal을 갖는다.
- 일반 frame queue는 `current + latest`를 넘지 않는다.
- Windows interactive resize transaction 안에서는 latest-only supersede로 target을 계속 버리지 않고 native handler backpressure로 한 target을 끝낸다. handler가 복귀한 뒤 다음 `WM_SIZE`가 새 transaction이 된다.
- visible surface는 matching exact backing store가 준비되기 전에는 파괴적으로 resize하지 않는다.
- `Present`, rAF callback 종료, GPU submit을 실제 scan-out ACK라고 부르지 않는다.
- device/context loss, minimize의 0×0 target, shutdown 중 transaction은 반드시 terminal 처리한다.

## 3. M0 — 구조 계약을 코드로 고정

상태: **PASS (2026-08-23)**

### 작업

- `Doroti/src/Doroti.Ui/ResizeLifecycle.cs`
  - 기존 epoch와 terminal ledger 위에 `FrameTransaction` 상태 전이를 추가한다.
  - 최소 payload는 epoch/build token, scene descriptor, backing-store identity/size, visible target identity, terminal reason이다.
  - 상태 전이는 단방향으로 만들고 platform code가 임의로 건너뛸 수 없게 한다.
- `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`
  - `DispatchFrame`이 호출 순간의 host 값을 다시 읽지 않고 요청에 포함된 epoch로 build scope를 연다.
  - host가 exact resize frame을 요청하고 완료를 기다릴 수 있는 명시적 API를 추가한다.
- `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
  - scene submission과 backing-store raster completion을 분리한다.
  - visible surface를 받지 않아도 exact offscreen target에 retained scene을 raster할 수 있는 entry를 만든다.
  - scene completion은 visible commit 뒤에만 `submitted`가 되고, backing-store raster만 끝난 상태는 별도 상태로 유지한다.
- `Doroti/validation/resize-contract/Program.cs`
  - `Observed → Metrics → Scene → Backing → Visible → Terminal` 순서, exactly-once terminal, size/epoch mismatch 거절을 deterministic permutation으로 검증한다.
  - Windows backpressure transaction과 Web single-rAF transaction을 별도 fixture로 만든다.

### 중단 조건

- 기존 stale-frame/exact-match contract를 유지하지 못하면 Windows/Web host 작업으로 진행하지 않는다.
- common contract 때문에 Qt/macOS/Android host가 새로운 presentation 방식을 구현해야 하는 구조가 되면 API 경계를 다시 나눈다. 이번 변경은 Windows/Web opt-in capability로 시작한다.

### 완료 gate

- resize contract PASS.
- affected .NET projects Release build 경고 0/오류 0.
- scene을 최신 target으로 submit-time relabel하는 코드 경로 0.

## 4. W0 — 단일 HWND + offscreen backing store 구조 spike

상태: **FAIL — runtime correctness PASS, D3D12 ordering/ANGLE fallback 및 G1 visual/cadence FAIL (2026-08-23)**

이 단계는 계측용 mock이 아니라 이후 제품 host가 그대로 재사용할 최소 구현이다. 먼저 고정 높이 bar, 원, 우측 edge가 있는 작은 native executable에서 presenter transaction을 완성한다.

### 작업

- 새 native host/presenter 책임을 다음 단위로 분리한다.
  - `WindowsNativeView`: child HWND 생성/파괴, focus와 native message 수신.
  - `WindowsPlatformTaskRunner`: message-only HWND와 task queue, 특정 task-runner window만 처리하는 `PollOnce`.
  - `WindowsResizeCoordinator`: `WM_SIZE`/DPI epoch와 `Started/FrameReady/Presented/Done/TimedOut` 상태 소유.
  - `WindowsD3D12BackingStore`: exact-size offscreen GPU render target와 Skia surface 소유.
  - `WindowsHwndPresenter`: 단 하나의 HWND flip-model swap chain, back-buffer reference/fence/present 소유.
- `CreateSwapChainForHwnd`를 사용하고 `Scaling.None`, flip model, buffer count 2를 사용한다.
- HWND 하나에는 flip-model swap chain 하나만 둔다. dual swap-chain attach/swap은 사용하지 않는다.
- swap chain 생성 때 `FrameLatencyWaitableObject`를 활성화하고 maximum frame latency 1로 둔다. regular frame은 waitable handle이 열린 뒤 raster/present를 시작한다.
- scene 전체는 offscreen backing store에 먼저 raster한다.
- matching backing store가 준비된 뒤에만 다음 순서로 visible commit한다.
  1. 기존 back-buffer를 참조하는 Skia/resource/view를 해제한다.
  2. 같은 D3D12 queue의 fence로 old back-buffer GPU 사용 완료를 확인한다.
  3. `ResizeBuffers`를 exact client physical size로 수행한다.
  4. 새 current back buffer를 wrap한다.
  5. offscreen frame을 같은 크기로 GPU-only 1:1 copy/blit한다.
  6. resource를 present state로 전환하고 `Present(0)`한다.
  7. resize transaction에서만 `DwmFlush` 후 completion을 signal한다.
- CPU `readPixels`, bitmap encode/decode, GDI copy는 금지한다.
- context-wide `Submit(true)`를 resize마다 무조건 호출하는 대신, 실제 old back-buffer lifetime을 보호하는 queue fence를 사용한다. SkiaSharp D3D12 resource-state interop이 이를 안전하게 표현하지 못하면 W0를 실패로 판정한다.

### 완료 gate

- 좌/우/top/bottom/corner size 변경에서 모든 visible frame이 exact backing store에서 온다.
- single HWND에 두 번째 flip swap chain이 생기지 않는다.
- WGC에서 고정 높이/원형/우측 edge가 stretch, overflow, blank 없이 유지된다.
- `ResizeBuffers → 1:1 transfer → Present → DwmFlush` transaction이 deadlock 없이 반복된다.
- old back-buffer reference leak, `DXGI_ERROR_INVALID_CALL`, device removed 0.

### 실패 분기

- D3D12/SkiaSharp public interop 때문에 exact offscreen GPU transfer 또는 buffer fence를 안전하게 만들 수 없으면 제품 통합을 중단한다.
- 이 경우 동일 `WindowsResizeCoordinator`/task-runner 계약은 유지하고 presenter만 Flutter와 같은 ANGLE/EGL `EGL_FIXED_SIZE_ANGLE` surface + framebuffer blit + `eglSwapBuffers`로 교체해 W0를 다시 수행한다.
- `SwapChainPanel`, `SetSourceSize`, capacity buffer, 두 composition swap chain으로 되돌아가지 않는다.

## 5. W1 — native resize transaction을 제품 host에 연결

상태: `notStarted` — G1 실패로 제품 승격 중단. W0 검증을 위해 demo에 임시 연결된 코드는 acceptance 완료로 보지 않는다.

### 작업

- `Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs`
  - `Grid + SwapChainPanel + WindowsD3D12Presenter` 제품 경로를 W0의 native host로 교체한다.
  - `_view.SizeChanged`, host `SizeChanged`, `CompositionScaleChanged`를 각각 authority로 사용하는 구조를 제거한다.
  - `_renderSwapChain/_presentedSwapChain`, UI-thread `SetSwapChain`, `InvokeOnUiThread`, panel inverse matrix를 제거한다.
- MAUI top-level HWND 아래에 render child HWND를 만들고 실제 native client rect와 `GetDpiForWindow`를 size/DPI authority로 사용한다.
- 현재 full-window Doroti surface는 top-level client resize에서 child HWND를 직접 배치한다. 향후 embedded view를 위해 XAML bounds가 필요해도, XAML은 위치 입력만 제공하고 최종 child client rect를 게시하는 native bridge는 하나만 둔다.
- top-level/child window message 연결은 수명 해제가 명확한 subclass/token 방식으로 구현한다. unload/reload에서 stale callback이 남지 않게 한다.
- child `WM_SIZE` 처리:
  1. zero/minimized size는 surface suspend terminal로 끝낸다.
  2. physical client size와 DPI로 immutable epoch를 만든다.
  3. `WindowsResizeCoordinator.Begin(epoch)`을 호출한다.
  4. metrics와 exact frame request를 `WindowsPlatformTaskRunner`에 게시한다.
  5. 최대 100ms 동안 task-runner message-only HWND만 `PollOnce`하며 matching `Presented/Failed`를 기다린다.
  6. arbitrary WinUI message를 중첩 처리하거나 nested `WM_SIZE`를 재진입시키지 않는다.
  7. timeout이면 stale frame을 present하지 않고 handler를 복귀한 뒤 latest target을 새 transaction으로 재요청한다.
- `Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs`
  - Windows `DispatchWindowsResizeFrame`, `CompositionTarget.Rendering`, surface invalidation의 중복 frame ownership을 `WindowsResizeCoordinator` 한 곳으로 옮긴다.
  - animation frame과 resize-required frame을 구분하되 둘 다 동일 frame transaction API를 사용한다.
- `Doroti/src/Doroti.Host.Maui/DorotiMauiSurface.cs`
  - native surface의 backing-store-ready와 visible-presented completion을 framework scene completion에 연결한다.
  - evidence writer는 transaction hot path 밖에 유지한다.

### 완료 gate

- `WM_SIZE` target 하나당 matching exact frame present 또는 명시적 timeout/failure terminal 하나.
- resize transaction 중 UI dispatcher `Present/SetSwapChain` 호출 0.
- 정상 drag에서 superseded frame 폭증으로 cadence를 잃는 구조 0. 새 target은 이전 native transaction 복귀 뒤 처리한다.
- 100ms timeout 뒤 입력/창 lifecycle이 복구되고 deadlock이 없다.

## 6. W2 — input, IME, semantics, lifecycle 소유권 이관

상태: `notVerified` — G1 중단 조건으로 미실행

native child HWND로 presentation만 옮기고 XAML panel에 input/focus가 남으면 새 이중 소유권이 생긴다. 이 단계까지 완료해야 Windows host cutover로 인정한다.

### 작업

- pointer/wheel/key/focus를 child HWND 기준 좌표와 DPI로 전달하는 `WindowsNativeInputBridge`로 이동한다.
- pointer capture, leave/cancel, high-resolution wheel, modifier state를 포함한다.
- text input은 기존 on-demand hidden `Entry/Editor`를 유지할 수 있지만 focus authority와 caret screen rect 변환은 child HWND에 둔다.
- accessibility는 기존 semantics tree를 보존하고 native view의 screen bounds/focus와 일치시킨다. XAML overlay가 HWND airspace 때문에 실제 bounds/focus를 잃으면 native UIA provider로 옮긴다.
- `WM_DPICHANGED`, monitor 이동, minimize/restore, occlusion, device removed/reset, unload/reload, shutdown을 transaction terminal과 연결한다.
- device recovery는 old transaction을 `failed`, context/surface generation을 증가시키고 latest epoch exact frame을 다시 요청한다.

### 완료 gate

- mouse/touch/pen, keyboard, Korean IME, clipboard, focus traversal, semantics action이 기존 기능보다 퇴행하지 않는다.
- DPI monitor 이동 뒤 pointer/semantics/render 좌표가 같은 epoch다.
- minimize/restore와 device recovery 뒤 blank/stale surface가 남지 않는다.

## 7. W3 — Windows 구 경로 삭제와 제품 전환

상태: `notStarted` — G1 중단 조건으로 미실행

### 작업

- W0~W2 동안만 제한된 A/B switch를 허용한다.
- strict visual/cadence gate를 통과하면 native transaction presenter를 기본값으로 바꾸고 다음 코드를 삭제한다.
  - `SwapChainPanel` dual exact staging presenter
  - `_renderSwapChain/_presentedSwapChain` 역할 교환
  - UI-thread `Present + SetSwapChain`
  - panel width/height/inverse composition matrix commit
  - Windows resize 전용 `CompositionTarget.Rendering` ownership
- target identity, diagnostics schema, template/manifest 설명을 실제 native HWND backend로 갱신한다.
- 삭제 후 repository search로 구 backend identity와 unreachable compatibility branch가 0인지 확인한다.

## 8. B0 — Web single-rAF frame pump

상태: `notStarted` — ordered G1 gate 실패로 미실행

### 작업

- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
  - `ResizeObserver`는 exact device-pixel epoch를 게시하고 rAF 하나만 예약한다.
  - `sampleHost`용 추가 rAF, presenter `queueMicrotask`, separate current/latest drain을 제거한다.
  - 한 rAF callback 안에서 managed frame dispatch, exact raster, visible commit, terminal 기록을 순서대로 끝내는 `BrowserFramePump`를 만든다.
- `Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs`
  - 현재 synchronous `[JSExport] DispatchAnimationFrame` 경계를 확장해 frame request/epoch/raster 결과를 한 호출 흐름으로 연결한다.
  - callback ID 하나를 덮어쓰는 방식 대신 transaction ID와 epoch를 명시한다.
- `Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor`
  - hot path의 `IJSObjectReference`, `DotNetObjectReference`, async `RenderFrame`, `CompleteFrame` 왕복을 제거한다.
  - Razor component는 canvas/IME/semantics markup과 수명 연결만 담당한다.
- `Doroti/src/Doroti.Host.Web/DorotiSurface.razor`
  - `InvokeAsync(RequestPresentAsync)` invalidation queue를 제거하고 `BrowserFramePump`를 직접 연결한다.
- managed frame 안에서 비동기 asset 작업이 시작되면 현재 rAF를 기다리게 하지 않고 transaction을 `notReady`로 끝낸 뒤 다음 rAF를 예약한다. 완성되지 않은 scene을 재라벨해 commit하지 않는다.

### provisional 표시 정책

- observer callback에서는 visible canvas `width/height`를 먼저 reset하지 않는다.
- visible canvas는 마지막 exact bitmap의 intrinsic/CSS 크기를 유지한다.
- root background와 clipping으로 expansion/shrink를 표현한다.
- exact transaction이 끝날 때만 새 intrinsic/CSS size와 새 pixels를 함께 commit한다.
- DPR 변화 중 이전 exact frame을 잠시 보여 줄 수 있지만 X/Y 비율이 다른 stretch는 허용하지 않는다.

### 완료 gate

- 한 target의 exact commit까지 browser scheduling boundary는 rAF 하나.
- rAF 내부 JS→managed frame dispatch 1회, managed→present commit 1회 이하.
- Blazor renderer queue와 `queueMicrotask`가 presentation hot path에 없음.
- target이 rAF 전에 관찰되고 build+raster가 frame budget 안에 끝나면 같은 rAF에서 exact visible commit.

## 9. B1 — OffscreenCanvas backing store와 atomic visible transfer

상태: `notStarted` — ordered G1 gate 실패로 미실행

### 작업

- browser feature probe 뒤 `OffscreenCanvas`에 단 하나의 WebGL2/Skia context를 만든다.
- exact target size의 offscreen surface에 raster하고 `transferToImageBitmap()`으로 frame을 만든다.
- visible canvas는 `bitmaprenderer` context를 사용한다.
- exact frame을 commit하기 직전에 visible canvas intrinsic/CSS size를 frame size/DPR과 맞춘 뒤 `transferFromImageBitmap()`으로 표시한다.
- ImageBitmap은 transfer 후 소비된 것으로 취급하고 닫기/수명 contract를 명시한다.
- context loss 시 current transaction과 bitmap lease를 terminal 처리하고 offscreen context/surface를 재생성한 뒤 latest exact frame을 요청한다.
- SkiaSharp Emscripten 내부 객체 전체를 `globalThis`에 노출하지 않는다. `DorotiSkiaInterop.js`는 context 생성/current/FBO registration에 필요한 좁은 Doroti-owned bridge만 제공한다.

### GPU fallback

OffscreenCanvas 또는 bitmap transfer와 현재 SkiaSharp context 결합이 불가능한 환경에서는 다음 fallback을 사용한다.

- visible WebGL2 context 하나와 exact-size `front + staging` FBO를 유지한다.
- 같은 rAF 안에서 staging raster를 끝낸 뒤에만 visible canvas size를 바꾸고 default framebuffer로 exact 1:1 blit한다.
- source/destination pixel rect는 항상 같다.
- observer와 exact frame 사이에는 old canvas natural size + root crop/background 정책을 사용한다.
- CPU readback, `preserveDrawingBuffer=true`, `toDataURL`, CSS transform/mask, old front full-rect stretch는 금지한다.

### 중단 조건

- primary와 fallback 모두 같은 rAF 안에서 exact visible commit을 만들 수 없으면 B2/최적화로 진행하지 않는다.
- context restore 뒤 private Emscripten handle table이 일관되지 않으면 bridge ABI를 먼저 고친다.

## 10. B2 — Web 구 presentation 경로 삭제

상태: `notStarted` — ordered G1 gate 실패로 미실행

### 작업

- 다음 구 경로를 제거한다.
  - observer callback의 default framebuffer reset/retained refresh
  - separate presenter `current + latest + queueMicrotask` drain
  - Razor async `RenderFrame/CompleteFrame`
  - visible default framebuffer를 장기 retained front처럼 취급하는 코드
  - full-frame provisional stretch와 이를 허용하던 diagnostics 문구
- `BrowserFrameDiagnostics`와 trace phase를 single-rAF transaction 기준으로 단순화한다.
- resize/animation/context recovery가 같은 `BrowserFramePump`를 사용하게 한다.
- pointer listener와 semantics overlay는 root-local 최신 epoch 좌표를 유지한다.

## 11. 검증 순서와 실패 관문

상태: **G0 PASS / G1 FAIL / G2~G4 notVerified (2026-08-23)**

검증은 구조 변경 뒤 수행하며, green counter를 만들기 위해 구조를 다시 복잡하게 만들지 않는다. 앞 gate가 실패하면 다음 milestone으로 진행하지 않는다.

### G0 — deterministic contract/build

- `dotnet run --project Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release`
- `dotnet build DorotiDemoApp/windows/DorotiDemoApp.Windows.csproj -c Release`
- `dotnet build DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release`
- 공통 계약 영향을 받는 Qt/macOS/Android compile gate.
- 요구값: stale present 0, relabel 0, unterminated transaction 0, exact mismatch 0, illegal state transition 0.

### G1 — Windows W0 strict gate

- WGC가 실제 child HWND/DXGI content를 포함하는지 먼저 확인한다.
- left edge 10초를 고해상도 native input driver로 실행한다.
- fixed-height bar, 원, 우측 edge oracle을 모든 capture frame에 적용한다.
- 현재 host와 같은 장비/driver/refresh에서 Flutter reference app을 함께 실행한다.
- 요구값:
  - app-presented geometry mismatch/overflow/blank 0.
  - border-content phase p95가 Flutter보다 한 refresh interval 이상 나쁘지 않음.
  - 2 refresh intervals를 넘는 phase gap 0(명시적 timeout/device recovery 제외).
  - actual input sample, delivered `WM_SIZE`, completed transaction, present 수를 함께 기록.
- 실패하면 W1 제품 통합을 중단하고 W0 presenter fallback만 수행한다.

### G2 — Windows 제품 acceptance

- left/right/top/bottom/네 corner 각 10초.
- 100/125/150/200% DPI 및 monitor 이동.
- 가능한 장비에서 60/120/144/165Hz.
- minimize/restore, maximize/restore, 빠른 방향 전환, occlusion, device loss/recovery.
- pointer/keyboard/Korean IME/focus/semantics/accessibility.
- correctness, cadence, visual을 별도 PASS/FAIL로 기록한다. 셋 중 하나라도 실패하면 Windows 완료로 표시하지 않는다.

### G3 — Web 40-sample smoke

- 활성 smoke 기준은 40 samples다.
- Chrome과 Edge에서 실제 browser window bounds 변경을 사용한다.
- fixed-height AppBar, 원형 control, right edge, blank, non-uniform scale, stale commit, GL error, transaction terminal을 확인한다.
- observer→rAF→visible commit scheduling boundary가 하나인지 trace로 확인한다.
- 자동 screenshot PASS를 browser compositor/실제 border-drag acceptance로 확대 해석하지 않는다.

### G4 — Web 실제 interaction/lifecycle

- Chrome/Edge/Firefox 실제 pointer border drag.
- maximize/restore, 빠른 확대/축소, DPR/zoom, 4× CPU slowdown.
- OffscreenCanvas primary와 WebGL fallback을 각각 검증한다.
- context loss/restore 뒤 latest exact frame과 input/semantics 좌표 복구.
- 같은 DPR에서 non-uniform stretch 0, full-background-only frame 0, old canvas crop/background 지속은 최대 한 rAF.

## 12. 완료 후 문서화

상태: **완료 (2026-08-23 evidence boundary 반영)**

- `research.md`를 현재 구현과 실제 evidence 기준으로 다시 쓴다.
- 과거 dual exact `SwapChainPanel`과 multi-queue Web presenter는 실패한 역사로만 남기고 현재 권장 구조처럼 서술하지 않는다.
- 각 gate를 `PASS`, `FAIL`, `notVerified`로 명시한다.
- build/contract PASS를 visible/cadence PASS로 바꾸어 쓰지 않는다.
- evidence에는 source commit, Flutter commit, OS/browser/GPU, DPI/refresh, input rate, raw trace, WGC/video를 함께 기록한다.
- 결과 파일이 checkout에 없거나 재생산되지 않으면 재현 가능한 PASS로 기록하지 않는다.

## 13. 명시적 비목표

- SkiaSharp fork 또는 Skia draw micro-optimization을 첫 해법으로 삼지 않는다.
- `SetSourceSize`, capacity back buffer, `Scaling.Stretch`, CSS scale/mask를 제품 resize 정책으로 되살리지 않는다.
- `Present(1)`, `DwmFlush`, swap interval 하나만 바꿔 해결됐다고 판단하지 않는다.
- debounce/timer로 target 수를 숨기지 않는다.
- HWND 하나에 여러 flip-model swap chain을 붙이지 않는다.
- Web CPU readback이나 `preserveDrawingBuffer=true`로 continuity를 만들지 않는다.
- validator, trace field, sample 수 증가만을 milestone 완료로 인정하지 않는다.

## 14. 코드/API 근거

### 현재 코드

- Windows current host: `Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs`
- Windows frame dispatch: `Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs`
- common frame/epoch: `Doroti/src/Doroti.Ui/PlatformDispatcher.cs`, `Doroti/src/Doroti.Ui/ResizeLifecycle.cs`
- renderer: `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- Web host/presenter: `Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs`, `Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor`, `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`
- Web Skia bridge: `Doroti/src/Doroti.Target.Web.browser-wasm/build/DorotiSkiaInterop.js`

### Flutter 고정 reference

- Windows bounded resize handshake: `reference/flutter-master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc`
- Windows task polling: `reference/flutter-master/engine/src/flutter/shell/platform/windows/task_runner_window.cc`
- Windows exact framebuffer blit/swap: `reference/flutter-master/engine/src/flutter/shell/platform/windows/compositor_opengl.cc`
- Windows fixed-size surface: `reference/flutter-master/engine/src/flutter/shell/platform/windows/egl/manager.cc`
- Web single rAF owner: `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/frame_service.dart`
- Web offscreen raster/visible transfer: `reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/compositing/offscreen_canvas_rasterizer.dart`, `render_canvas.dart`

### 공식 API 문서

- Microsoft Learn, [`CreateSwapChainForHwnd`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforhwnd)
- Microsoft Learn, [`DXGI flip model`](https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-flip-model)
- Microsoft Learn, [`ResizeBuffers`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-resizebuffers)
- Microsoft Learn, [`GetFrameLatencyWaitableObject`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-getframelatencywaitableobject)
- Microsoft Learn, [`SetMaximumFrameLatency`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_3/nf-dxgi1_3-idxgiswapchain2-setmaximumframelatency)
- Microsoft Learn, [`Present`](https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgiswapchain-present)
- Microsoft Learn, [`DwmFlush`](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)
- WHATWG, [`canvas`, `OffscreenCanvas`, `ImageBitmapRenderingContext`](https://html.spec.whatwg.org/dev/canvas.html)
- WHATWG, [`requestAnimationFrame` / update the rendering](https://html.spec.whatwg.org/multipage/imagebitmap-and-animations.html)
- CSSWG, [`Resize Observer`](https://drafts.csswg.org/resize-observer/)

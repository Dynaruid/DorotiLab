# Doroti Windows App SDK + C++ Flutter-style child HWND backend 작업 계획

## 0. 문서 목적과 현재 경계

- 계획 기준일: 2026-08-25
- 설계 기준: [`idea.md`](idea.md)의 **A. 전용 child HWND + HWND swap chain** 구성
- 구현 방향: Windows native core는 C++20 `.cpp`로 구현하고, Doroti framework와 기존 `SkiaSceneRenderer`는 managed .NET에 유지한다.
- 대상 제품 경계: `Doroti.Host.WindowsAppSdk`와 `Doroti.Target.Windows.WindowsAppSdk.win-x64`
- Windows App SDK 기준: repository에서 이 host에 고정한 exact `2.4.0`, self-contained unpackaged, 우선 `win-x64`
- Flutter source 기준: local checkout `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- test timeout: 각 build/test/validation command 최대 20분
- 문서 상태: **C0-C4 PASS / C5-D3D12 FAIL(이력 보존) / C5-A managed ANGLE PASS / C6 automated PASS·physical notVerified / high-speed resize PARTIAL(엄격 capture FAIL) / C7-C11 notVerified**

이 문서는 기존 `WinRtComposition`/ContentIsland presentation 계획을 대체하는 새 작업계획이다. 현재 worktree의 WinRT/ContentIsland spike와 validator 수정은 이 계획의 구현 또는 PASS 증거로 간주하지 않으며, 후속 작업에서도 사용자 소유 변경으로 보존한다.

이 계획에서 `.cpp 구현`은 다음처럼 해석한다.

- top-level HWND, AppWindow 연결, child render HWND, message-only task HWND, WndProc와 message loop는 C++가 소유한다.
- resize state machine과 100 ms bounded wait는 C++가 소유한다.
- 선택된 GPU presenter의 device/context, exact render target, window surface, submit/present는 managed presenter가 단독 소유한다. 현재 product ANGLE/EGL-D3D11 분기는 fixed-size EGL default framebuffer에 직접 raster하고, 기존 D3D12 offscreen/copy presenter는 진단·비교 경로로 보존한다.
- C#은 process bootstrap, Doroti application/framework, scene build, `SkiaSceneRenderer`, managed presenter, input/semantics adapter와 versioned C ABI 호출을 소유한다.
- C++는 top-level/child/task HWND와 AppWindow, WndProc, message loop, metrics/input ingress만 소유하며 GPU COM pointer를 생성하거나 ABI로 전달하지 않는다.
- Doroti framework 전체나 공용 UI API를 C++로 재작성하는 것은 범위 밖이다.

## 1. 검토 결론

### 1.1 A 구성 채택

채택할 기본 topology는 다음과 같다.

- top-level: standard `WS_OVERLAPPEDWINDOW` HWND + Windows App SDK `AppWindow`
- render target: 앱이 직접 만든 `WS_CHILD | WS_VISIBLE` child HWND 1개
- renderer input: current generation과 크기가 정확히 일치하는 Doroti scene
- raster target: managed presenter가 만든 exact-size ANGLE/GLES offscreen surface
- presenter: child HWND에 연결한 `EGL_FIXED_SIZE_ANGLE` window surface와 hardware D3D11 ANGLE display
- resize handshake: child `WM_SIZE → metrics → exact scene → exact backing → fixed-size surface recreation → copy → eglSwapBuffers → wake → DwmFlush`
- input/IME/UIA: render topology를 바꾸지 않는 별도 adapter

ContentIsland와 composition swap chain은 A 경로의 renderer나 size authority로 사용하지 않는다. 필요성이 나중에 입증되면 입력·접근성 또는 popup 같은 bounded integration boundary로만 별도 검토한다.

### 1.2 C3 실패 뒤 승인된 ownership 변경

native-owned device/queue/resource를 managed Vortice/SkiaSharp에 frame lease로 전달하는 최초 C3는 D3D12 debug error ID 1315를 8건 발생시켜 실패했다. 사용자는 2026-08-25에 hard-stop 대안 1을 승인했다.

채택한 해법은 managed presenter 단독 GPU ownership이다.

1. C++ platform STA가 top-level/child/task HWND와 resize generation을 소유한다.
2. C++는 immutable metrics와 child HWND identity만 managed 쪽에 전달한다.
3. managed presenter가 adapter/device/queue/fence, `GRContext`, exact backing, HWND swap chain과 모든 back-buffer wrapper를 생성·폐기한다.
4. `SkiaSceneRenderer.Paint`, `Flush/Submit`, fence, copy, `ResizeBuffers`, `Present`, `DwmFlush`는 같은 managed presentation owner에서 순서화한다.
5. successful exact `Present` 뒤에만 C ABI로 terminal/completion을 task HWND에 돌려준다.
6. shutdown은 managed GPU idle/release ACK 뒤 C++가 child/task/top-level HWND를 파괴한다.

따라서 GPU COM pointer나 Vortice wrapper는 C ABI를 통과하지 않는다. 최초 frame-lease spike는 실패 진단으로만 보존하고 제품 경로에 연결하지 않는다.

### 1.3 채택·제외 결정

| 항목                                                | 결정                    | 이유                                                                                                                                                                                  |
| --------------------------------------------------- | ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| existing `Doroti.Host.WindowsAppSdk`/target package | 유지                    | runner와 publish boundary를 재사용한다.                                                                                                                                               |
| adapter working name                                | `HwndExactCpp`          | 더 이상 composition backend가 아니므로 `WinRtComposition` 이름을 재사용하지 않는다.                                                                                                   |
| C++20 native DLL                                    | 채택                    | HWND, AppWindow, task pump, resize protocol의 단일 owner다.                                                                                                                           |
| C++/WinRT                                           | AppWindow 연결에만 사용 | XAML/ContentIsland renderer를 들이지 않는다.                                                                                                                                          |
| managed `SkiaSceneRenderer`                         | 유지                    | Doroti scene semantics와 기존 renderer를 별도 포팅하지 않는다.                                                                                                                        |
| managed-owned D3D12 presenter                       | 진단 경로로 유지        | GPU object가 ABI를 넘지 않는 ownership은 옳지만 실제 Demo scene에서 D3D12 ID 1422가 재현되어 product 기본 경로로 승격하지 않는다.                                                     |
| native-owned D3D12 frame lease                      | 제외                    | 최초 C3에서 D3D12 debug error ID 1315가 8건 발생했다. diagnostic history로만 보존한다.                                                                                                |
| `CreateSwapChainForHwnd` + `DXGI_SCALING_NONE`      | D3D12 비교 경로에 유지  | A topology의 기존 D3D12 진단을 보존한다.                                                                                                                                              |
| exact offscreen backing                             | 채택                    | resize 전에 exact scene을 준비하고 back buffer 수명과 분리한다.                                                                                                                       |
| ContentIsland/composition swap chain                | primary에서 제외        | Flutter child HWND topology와 독립 변수를 흐린다.                                                                                                                                     |
| WinUI XAML, `SwapChainPanel`, MAUI code reuse       | 제외                    | 별도 UI/layout/presentation owner를 만들지 않는다. MAUI backend는 독립 유지한다.                                                                                                      |
| managed ANGLE/EGL-D3D11                             | 채택                    | 2026-08-26 사용자 승인으로 기존 제외 결정을 대체했다. `eglGetPlatformDisplayEXT`에 D3D11 hardware를 명시하며 software renderer를 fail-closed한다. SkiaSharp source는 수정하지 않는다. |
| large permanent backing, stretch, clip-only resize  | 제외                    | Flutter exact-surface protocol을 복제하지 못한다.                                                                                                                                     |
| Flutter runtime build/instrumentation               | 제외                    | pinned source contract만 사용한다. 과거 중단된 비교 실행 경로를 재개하지 않는다.                                                                                                      |

## 2. 목표 architecture

```text
generated .NET WinMain / DorotiWindowsAppSdkRunner
  └─ doroti_windows_appsdk_host_v1 C ABI
      |
      +-- native platform STA (.cpp)
      |     +-- standard top-level HWND
      |     +-- WindowId + AppWindow
      |     +-- one WS_CHILD render HWND
      |     +-- one HWND_MESSAGE task HWND
      |     +-- WndProc, DPI/lifecycle, input ingress
      |     +-- current resize generation + 100 ms filtered wait
      |
      +-- managed framework thread
      |     +-- immutable metrics consumption
      |     +-- layout/build
      |     +-- exact Doroti scene publication
      |
      +-- managed raster/presentation thread
            +-- ANGLE EGL display/context (hardware D3D11) + Skia GRContext
            +-- exact offscreen surface
            +-- EGL_FIXED_SIZE_ANGLE child-HWND surface
            +-- SkiaSceneRenderer.Paint -> Flush/Submit
            +-- copy -> eglSwapBuffers -> DwmFlush
            +-- C ABI terminal -> native task-HWND completion wake
```

### 2.1 thread ownership

| owner                              | 소유 객체                                                                                                       | 금지 사항                                                                           |
| ---------------------------------- | --------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| native platform STA                | top-level/child/task HWND, AppWindow, WndProc, resize wait state, input ingress                                 | framework layout, Skia raster, GPU fence wait, arbitrary top-level message re-entry |
| managed framework thread           | Doroti view/layout/build, immutable scene와 metrics mailbox                                                     | HWND/AppWindow/GPU presenter 접근                                                   |
| managed raster/presentation thread | ANGLE/EGL-D3D11 object, exact backing, fixed-size window surface, `GRContext`, copy, present, device-loss state | AppWindow/child geometry 직접 변경, framework scene mutation                        |

managed presentation thread는 C++ platform STA를 재진입하지 않는다. resize/close 요청은 task HWND packet으로 전달하고 bounded completion만 기다린다.

### 2.2 native 코드 경계

계획상의 native project는 `Doroti/src/Doroti.Host.WindowsAppSdk.Native/`에 둔다.

```text
Doroti.Host.WindowsAppSdk.Native.vcxproj
include/
  doroti_windows_host_v1.h
src/
  exports.cpp
  app_window_host.cpp
  render_child_window.cpp
  task_window.cpp
  resize_coordinator.cpp
  input_router.cpp
  text_input.cpp
  uia_provider.cpp
  lifecycle.cpp
```

초기 C0-C4에서는 필요한 파일만 만든다. input/text/UIA 파일을 빈 placeholder로 미리 추가하지 않는다.

### 2.3 managed 코드 경계

`Doroti.Host.WindowsAppSdk`에는 다음 working component를 둔다.

- `DorotiWindowsAppSdkRunner`: descriptor 검증, application/session 생성, native run 진입
- `WindowsNativeV1`: versioned C ABI struct/delegate와 `LibraryImport`
- `WindowsManagedState`: callback lifetime, fatal error, terminal ledger, ordered shutdown
- `WindowsHostAdapter`: Doroti view/frame/input/platform capability
- `WindowsManagedHwndPresenterBase`: managed GPU owner 공통 contract
- `WindowsManagedAngleEglPresenter`: ANGLE/EGL-D3D11 exact backing과 fixed-size HWND surface의 product GPU owner
- `WindowsManagedHwndPresenter`: 기존 D3D12/Skia 비교·진단 presenter
- `WindowsInputMapper`, `WindowsTextInputAdapter`, `WindowsSemanticsBridge`: native packet과 Doroti contract 변환

public Doroti API에 raw HWND, COM pointer, Vortice type 또는 native ABI struct를 노출하지 않는다.

## 3. versioned C ABI 계약

### 3.1 기본 규칙

- ABI 이름은 `doroti-windows-host/v1`로 고정한다.
- 모든 struct 첫 필드는 `abi_version`, `struct_size`다.
- 문자열은 pointer + byte length UTF-8이며 null 종료에 의존하지 않는다.
- bool은 ABI에서 `uint32_t`로 표현한다.
- enum underlying type과 packing/alignment를 header와 managed layout test에서 고정한다.
- exception은 DLL 경계를 넘지 않는다. native는 status/error callback, managed는 callback guard로 terminal failure를 기록한다.
- callback context는 opaque pointer이며 shutdown ACK 전까지 유효하다.
- callback ABI와 host API function pointer는 명시적으로 `__cdecl` 하나만 사용한다.
- version/size/required feature bit가 다르면 창을 만들기 전에 fail-fast 한다.

### 3.2 최소 contract

초기 header는 다음 책임을 표현한다. 아래 이름은 작업용이며 C1에서 layout fixture와 함께 확정한다.

- `doroti_windows_run_v1(configuration, callbacks)`
- `doroti_windows_metrics_v1`
  - generation, width/height physical px, scale, logical size, display identity, timestamp
- `doroti_windows_frame_request_v1`
  - generation, exact extent, causal frame ID, lease ID
- `doroti_windows_frame_terminal_v1`
  - `presented | superseded | failed`, error category, timestamps
- callbacks
  - host ready, metrics, render exact frame, frame terminal, input, text, semantics action, lifecycle, log
- host API
  - request frame/close, set cursor/capture, clipboard, text client/caret, semantics update, diagnostics snapshot

child HWND는 managed presenter의 생성/종료 동안만 유효한 opaque identity다. GPU COM pointer와 Vortice/SkiaSharp object는 ABI에 포함하지 않는다.

## 4. resize와 presentation protocol

### 4.1 immutable identity

모든 resize target과 frame은 다음 identity를 유지한다.

```text
(view_id, generation, width_px, height_px, scale, causal_frame_id)
```

크기가 같아도 generation이 다르면 다른 요청이다. build 또는 submit 시점에 오래된 scene을 새 generation으로 relabel하지 않는다.

### 4.2 상태와 terminal

```text
Idle
  -> MetricsPublished
  -> WaitingForExactScene
  -> RasterLeaseActive
  -> ExactBackingReady
  -> Presenting
  -> Presented

terminal: Presented | Superseded | Failed
observation flag: PlatformWaitTimedOut = true | false
```

100 ms timeout은 terminal failure가 아니다. timeout 뒤에도 같은 current generation의 exact frame은 present할 수 있다. 새 generation이 시작되면 이전 generation은 surface 변경 권한을 잃고 정확히 한 번 `Superseded`로 정산한다.

### 4.3 순서

1. child `WM_SIZE`에서 전달된 `lParam`만 믿지 않고 `GetClientRect`로 physical extent를 다시 읽는다.
2. 0×0, minimize, 동일 current extent는 별도 lifecycle/no-op 규칙으로 정산하고 blocking resize를 시작하지 않는다.
3. platform thread가 generation을 증가시키고 immutable metrics를 managed framework mailbox에 게시한다.
4. platform thread는 최대 100 ms 동안 task HWND completion과 deadline만 기다린다.
5. 일반 top-level/child message를 이 wait 안에서 dispatch하지 않아 `WM_SIZE`와 input callback 재진입을 막는다.
6. framework/raster 경로는 current generation과 width/height가 모두 일치하는 scene만 admission한다.
7. managed presenter가 exact backing을 준비하고 managed SkiaSharp가 exact viewport에 paint한 뒤 `Flush/Submit`한다.
8. managed presenter가 같은 queue의 fence completion을 확인한다.
9. 이전 swap-chain back-buffer wrapper를 모두 해제한 뒤 필요하면 exact extent로 `ResizeBuffers`한다.
10. backing을 current back buffer로 transition/copy하고 다시 `PRESENT` state로 전환한다.
11. current generation 재검사 후 `Present`한다. successful `Present`만 `Presented`가 된다.
12. `DwmFlush`를 호출하고 duration/result를 진단으로 기록한다.
13. C ABI를 통해 task HWND에 exact completion을 post하여 platform wait를 깨운다.
14. 첫 exact present 전 창을 숨기는 정책은 이 시점에만 show를 허용한다.

### 4.4 queue와 stale 처리

- running 1 + latest pending 1만 유지한다.
- 새 target이 오면 아직 raster를 시작하지 않은 older pending은 즉시 supersede한다.
- raster 중 stale이 된 backing은 swap-chain resize/copy/present를 수행하지 않는다.
- `ResizeBuffers` 직전, copy 직전, `Present` 직전에 generation을 다시 검사한다.
- 모든 accepted generation과 scene은 exactly-once terminal을 가진다.
- resize timeout, frame miss, stale rejection, GPU wait, resize, copy, present, DWM flush를 별도 counter/timestamp로 남긴다.

### 4.5 GPU/resource ownership

- managed presenter만 device/queue/fence/backing/swap-chain buffer wrapper를 소유한다.
- C++는 GPU object를 만들거나 GPU pointer를 보관하지 않는다.
- capacity backing, clip-only resize, `SetSourceSize`, full-frame stretch를 사용하지 않는다.
- offscreen resource와 swap-chain buffer는 target과 정확히 같은 extent다.
- device/context generation이 바뀌면 managed `GRContext`와 모든 GPU wrapper를 폐기한 뒤 새 presenter generation으로 다시 만든다.
- `ResizeBuffers` 실패 시 old front가 valid하다는 사실과 new generation 실패를 구분해 기록한다.
- device removed/reset, adapter change, suspend/resume는 별도 device generation을 만들며 silent software fallback하지 않는다.

## 5. window, DPI, lifecycle 계약

- top-level HWND는 standard caption/border/system menu/Snap behavior를 Windows에 맡긴다.
- child render HWND만 client content를 소유하며 initial position은 `(0,0)`, extent는 parent client의 physical px다.
- child client size가 render authority다. top-level outer rect와 work area는 placement evidence일 뿐 render size가 아니다.
- `WM_DPICHANGED`는 suggested top-level rect 적용과 새 child metrics publication을 분리해 기록한다.
- minimize 시 0-size swap-chain resize를 하지 않고 frame production을 정지한다.
- restore는 새 generation과 exact frame 후 visible terminal을 가져야 한다.
- close는 새 callback admission 중지 → pending terminal 정산 → managed GPU idle/renderer/context release ACK → child/task/top-level destroy 순서다.
- AppWindow event와 WndProc가 같은 lifecycle event를 중복 소유하지 않는다. platform STA가 하나의 normalized event로 병합한다.
- custom title bar, borderless window, owned envelope, two-front composition은 이 계획에서 제외한다.

## 6. input, IME, accessibility 원칙

입력 계층은 C4 resize/presentation hard gate를 통과한 뒤 한 기능씩 추가한다.

### 6.1 pointer, keyboard, focus, cursor

- child HWND가 render-client pointer/keyboard/focus packet의 sole owner다.
- pointer lifecycle은 `add → hover → down → move → up → remove/cancel`을 보존한다.
- drag 중 child 밖으로 나가도 `SetCapture`/`ReleaseCapture`를 사용하여 move/up을 잃지 않는다.
- `WM_SETCURSOR`는 `HTCLIENT`에만 Doroti cursor를 적용하고 non-client resize cursor는 `DefWindowProc`에 맡긴다.
- key down/up, system key, repeat, dead key, surrogate pair를 별도 packet으로 보존한다.
- WndProc는 packet을 enqueue하고 즉시 반환한다. framework 또는 GPU completion을 기다리지 않는다.

### 6.2 IME와 clipboard

- 첫 경로는 현재 Windows host에서 검증 가능한 IMM32 desktop HWND adapter로 둔다.
- composition/candidate window 위치는 current metrics generation의 caret rect와 DPI transform을 사용한다.
- Korean two-beolsik 조합, candidate, selection, reconversion, focus loss/restore를 물리 검증한다.
- clipboard는 Unicode text부터 시작하며 native buffer lifetime과 managed string lifetime을 ABI에서 분리한다.
- TSF/CoreText/WinUI island는 IMM32 경로의 실제 한계가 증명된 뒤 별도 decision으로만 추가한다.

### 6.3 UIA

- top-level `WM_GETOBJECT`가 UIA root ingress를 소유하고 child render content를 한 fragment tree로 연결한다.
- semantics snapshot은 immutable generation으로 native provider에 전달한다.
- stale semantics bounds를 새 DPI/size generation으로 relabel하지 않는다.
- automation action은 native provider thread에서 framework를 직접 실행하지 않고 managed action queue에 게시한다.

## 7. build, package, provenance

- native project는 MSVC C++20, `/W4`, `/WX`, `/permissive-`, reproducible build option을 사용한다.
- Windows SDK와 Windows App SDK/C++/WinRT input metadata 경로를 build script에서 exact하게 기록한다.
- native DLL 이름과 exported ABI는 versioned name으로 고정하고 ordinal import를 사용하지 않는다.
- `Doroti.Target.Windows.WindowsAppSdk.win-x64`가 native DLL과 필요한 runtime asset을 app directory에 복사/pack한다.
- PATH fallback, user-machine DLL discovery, validation build output 재사용을 금지한다.
- publish 뒤 app directory의 native DLL SHA-256, architecture, imports, export set을 검사한다.
- Debug artifact나 validation-only DLL이 product package에 섞이지 않았는지 검사한다.
- `WinRtComposition` manifest/target/runner 문자열은 C1 contract 승인 뒤 `HwndExactCpp`로 일관되게 교체한다. 그 전에는 기존 runner fail-fast를 유지한다.
- MAUI target/default와 다른 platform package를 변경하지 않는다.

## 8. ordered implementation gates

아래 gate는 순서대로 실행한다. 각 gate의 hard stop을 통과하기 전에는 다음 단계로 진행하지 않는다.

### C0 — source contract와 범위 고정

작업:

- pinned Flutter source의 top-level/child topology, child `WM_SIZE`, 100 ms wait, exact frame admission, surface recreation, present completion, `DwmFlush` mapping을 source validator에 고정한다.
- 이 계획의 A topology와 금지 항목을 machine-readable contract로 만든다.
- current repository의 Windows App SDK 2.4 pin, target fail-fast, package boundary를 재확인한다.
- 기존 ContentIsland/WinRT spike는 diagnostic history로만 분리하고 새 PASS에 합산하지 않는다.

검증:

- source mapping 전부 존재
- contract schema/fixtures PASS
- `git diff --check -- work3.md` 및 링크/경로 확인

종료 기준:

- source/문서 계약만 `PASS`
- 구현, runtime, visible은 `notVerified`

### C1 — native toolchain, ABI, packaging skeleton

작업:

- native `.vcxproj`, public C header, managed layout mirror, build script를 추가한다.
- ABI version/size/alignment/function-pointer calling convention test를 만든다.
- native DLL load/export/version/architecture/hash 검사를 만든다.
- product target의 app-directory packaging을 구성하되 adapter는 계속 fail-fast 한다.

검증:

- clean x64 Release native build
- managed/native layout fixture 양방향 PASS
- clean self-contained publish에 native DLL exactly once 포함
- PATH를 비운 상태에서 app-directory DLL만 load

**Hard stop C1:** ABI 또는 publish provenance가 불안정하면 창/renderer 구현을 시작하지 않는다.

### C2 — standalone C++ A-topology control

작업:

- managed Doroti를 붙이지 않은 native control executable을 만든다.
- standard top-level HWND + AppWindow + one child HWND + task HWND를 만든다.
- C++ D3D12/DXGI로 exact checker/grid backing, exact swap chain, copy, present를 구현한다.
- first exact present 전 show 금지, deterministic teardown, device-loss injection을 넣는다.

자동 검증:

- topology/class/style/parent 관계
- child-client physical extent == backing == swap-chain buffer
- 10-cycle create/show/resize/minimize/restore/close
- 0 stale present, 0 wrong-size present, 0 unaccounted generation
- D3D12 debug layer와 DXGI live-object report clean

**Hard stop C2:** native control만으로 exact resource release와 `ResizeBuffers`가 안정적이지 않으면 managed renderer를 붙이지 않는다.

### C3 — managed-owned SkiaSharp/D3D12 HWND presenter feasibility

작업:

- C++가 GPU object 없이 top-level/child/task HWND와 task pump만 소유한다.
- managed가 Vortice D3D12 device/queue/fence/swap chain과 SkiaSharp `GRContext`/`SKSurface`를 단독 소유한다.
- exact grid를 managed `SkiaSceneRenderer` 또는 최소 Skia callback으로 raster하고 managed presenter가 copy/present한다.
- C ABI에는 HWND identity, metrics, resize/terminal task만 전달하고 GPU COM pointer는 전달하지 않는다.
- normal, stale, exception, timeout, context recreate, shutdown 중 managed wrapper/COM release 순서를 검사한다.

필수 증거:

- `ResizeBuffers` 전 managed back-buffer wrapper reference 0
- managed fence가 managed Skia submit 뒤에 완료
- `ResizeBuffers` 10-cycle 중 `DXGI_ERROR_INVALID_CALL` 0
- context/device generation 변경 시 old wrapper 사용 0
- callback exception이 process crash나 lease leak 없이 `Failed` terminal로 변환
- C++ GPU object 생성 0, ABI GPU pointer 0
- D3D12 operational debug error 0, DXGI live-object leak 0

최초 native-owned frame-lease C3는 D3D12 debug error ID 1315로 실패했고 사용자가 대안 1을 승인했다. SkiaSharp 4.151.1의 D3D12 context 초기화에서 재현되는 ID 1315는 별도 `upstreamInitializationDiagnostic`으로 수량과 시점을 기록하며, 초기화 뒤 resize/copy/present/shutdown 구간의 새 debug error는 0이어야 한다. 이 구분은 오류 필터나 전체 debug clean 주장이 아니다.

**Hard stop C3:** managed 단일 GPU owner에서도 operational debug error, COM/back-buffer 잔존, `ResizeBuffers` invalid call 또는 shutdown leak이 발생하면 임시 reflection, private SkiaSharp API, raw pointer 장기 보관 또는 CPU readback으로 우회하지 않는다.

당시에는 native Skia/libSkiaSharp C ABI와 Doroti scene command serialization을 더 큰 대안으로 남겼다. 이후 2026-08-26 사용자 승인으로 SkiaSharp source를 수정하지 않는 managed ANGLE/EGL-D3D11 대체 분기를 먼저 구현했다.

### C4 — Flutter-style bounded resize handshake

작업:

- C2/C3를 resize coordinator와 연결한다.
- message-only task HWND, filtered wait, 100 ms deadline, current+latest queue를 구현한다.
- successful exact `Present`만 completion wake를 발생시키도록 한다.

자동 matrix:

- same-size repeated generation
- A→B→A
- grow/shrink
- left/right/top/bottom/corner synthetic sizing
- frame build 전 supersede
- raster 중 supersede
- lease return과 copy 사이 supersede
- timeout 뒤 late current completion
- timeout 뒤 stale completion
- minimize/restore, DPI change, close-during-wait

합격 기준:

- exact admission mismatch 0
- stale `ResizeBuffers`/copy/present 0
- recursive top-level/child WndProc dispatch 0
- wait ≤ 100 ms + scheduler tolerance
- every generation exactly one `Presented | Superseded | Failed`

**Hard stop C4:** contract가 PASS해도 visible resize 개선을 주장하지 않는다. C5 product integration만 허용한다.

### C5 — Doroti framework와 product host 연결

작업:

- runner가 native `run_v1`을 호출하고 application/session/view를 시작하도록 한다.
- metrics → framework build → exact scene → managed render callback 경로를 연결한다.
- existing `SkiaSceneRenderer`의 immutable build token, resize epoch, terminal receipt를 native generation과 매핑한다.
- ordinary invalidation, resize frame, semantics-only update의 terminal ledger를 분리한다.
- first exact product frame 뒤에만 AppWindow를 show한다.

검증:

- Demo publish/launch/first frame/clean close
- current+latest bounded queue
- retained scene replay와 new scene을 구분
- missing build token, wrong extent, old generation rejection
- native/managed terminal count 일치

2026-08-25의 **C5-D3D12** 실행에서는 최소 synthetic scene은 통과했지만 self-contained `DorotiDemoApp.WindowsAppSdk`의 실제 scene이 managed Skia submit 중 D3D12 operational error ID 1422를 6건 발생시켰다. 이 실패와 fail-closed runner 동작은 지우거나 PASS로 재해석하지 않는다.

2026-08-26 사용자가 SkiaSharp source 수정 대신 **C5-A managed ANGLE/EGL-D3D11** 대체 분기를 승인했다. 이 분기는 C++의 HWND/task pump ownership과 GPU pointer 0 ABI를 그대로 유지하고, managed가 ANGLE display/context, Skia `GRContext`, exact backing, fixed-size HWND surface와 `eglSwapBuffers`를 소유한다. ANGLE이 product 기본값이며 `DOROTI_WINDOWS_PRESENTER=D3D12`로만 실패 이력의 D3D12 비교 경로를 명시 선택한다. 어느 방향으로도 runtime 자동 fallback하지 않는다.

C5-A managed-owner resize probe는 context generation 2, fixed-size surface resize 10, invalid call 0, present/submit/copy 10/10/10, initialization/operational EGL/GLES error 0으로 PASS했다. Product validator도 AMD Radeon 780M의 `Direct3D11` ANGLE renderer를 확인했고 present/submit/copy 4/4/4, terminal 4/4, duplicate/unterminated 0으로 PASS했다. 실제 `DorotiDemoApp.WindowsAppSdk`는 10회 제한 반복에서 매회 exit 0, present/submit/copy 6/6/6, operational GPU error 0이었다. Demo의 기존 `RenderFlex._reportOverflow` assertion은 10회 모두 stderr에 기록되었으므로 presenter PASS와 분리하고 UI/layout 문제로 남긴다.

2026-08-26의 C5-A/C6 후속에서는 native task HWND가 managed layout/raster/`eglSwapBuffers`/`DwmFlush`를 동기 실행하여 `WM_SIZE`와 input ingress를 막던 구조를 제거했다. C++ platform/message thread는 child HWND geometry와 input packet을 즉시 처리하고, raster worker는 실행 중 1개와 최신 pending 1개만 받아 metrics/frame을 직렬 처리한다. managed input callback도 ingress에서 복사·enqueue한 뒤 같은 raster worker의 frame 시작에서 dispatch한다. ANGLE은 exact fixed-size EGL default framebuffer에 직접 raster하고 swap interval 0을 사용하며, `DwmFlush`는 비교 진단용 명시 opt-in(`DOROTI_WINDOWS_DWM_FLUSH=1`)으로만 남긴다. offscreen snapshot/copy는 product ANGLE 경로에서 제거했다.

자동 C6/product validator는 platform/raster OS thread가 서로 다르고 input ingress는 platform thread, Doroti input dispatch와 draw는 raster worker임을 확인했다. 동기 resize 요청 20건은 exact generation 2건으로 coalesce되어 2건 모두 presented됐고 duplicate/unterminated/failed 0, ANGLE present/submit/copy 2/2/0, operational EGL/GLES error 0이었다. 이 결과는 automated thread/queue/input contract만 PASS시키며 실제 마우스 capture, resize cursor, Alt+Tab/focus는 대체하지 않는다.

고속 우측 확장 WGC gate(600 px/600 ms, 165 Hz)는 변경 전 `gapFrame=29`, 최대 연속 gap `551.524 ms`, Demo present 8에서 worker/latest-only/direct-raster/swap-0/no-default-`DwmFlush` 적용 후 최종 기본 경로 `gapFrame=7`, 최대 연속 gap `60.604 ms`, capture frame 44로 개선됐다. blank frame 0, final gap 0, capture/encoder error 0이지만 순간 최대 gap 32 px가 남으므로 엄격 visible resize gate는 **PARTIAL/FAIL**이며 C5-A visible PASS로 승격하지 않는다. 근거는 `.doroti/evidence/c5a-c6-f6r-right.json`과 `.doroti/evidence/c5a-c6-f6r-right-default-final-2.json`이다.

**C5-A 경계:** product/runtime과 C6 automated contract는 PASS다. 사용자가 보고한 message-pump blocking 원인은 수정했지만 strict high-speed expansion capture에는 잔여 gap이 있으므로 resize 완료나 visible PASS를 주장하지 않는다. C6 physical/manual과 C7-C11도 `notVerified`다.

### C6 — pointer, keyboard, focus, cursor, clipboard

작업:

- child/top-level WndProc packet을 versioned ABI로 전달한다.
- pointer lifecycle, capture, re-entry, cursor, keyboard/dead-key/surrogate, focus, clipboard를 순차 연결한다.

검증:

- automated packet/coordinate/capture contract
- 실제 border drag 후 client re-entry, click, cursor recovery 수동 확인
- Alt+Tab, minimize/restore, popup/focus transition 수동 확인

자동 input PASS는 실제 마우스/cursor/focus visible PASS로 대체하지 않는다.

### C7 — IME와 accessibility hard gate

작업:

- IMM32 composition/candidate/caret/selection 경로를 연결한다.
- UIA root/fragment/provider와 semantics/action dispatch를 연결한다.

검증:

- Korean two-beolsik 실제 입력, 후보창, caret, selection, focus restore
- Narrator 탐색/읽기/action
- Accessibility Insights의 tree/bounds/pattern/error 확인
- resize/DPI 후 IME/UIA bounds가 current generation과 일치

### C8 — lifecycle, DPI, monitor, device recovery

검증 matrix:

- 10-cycle launch/close와 resize/minimize/restore
- 100/125/150/175/200% DPI
- mixed-DPI multi-monitor 이동과 resize
- maximize/restore, Snap, system menu, keyboard sizing
- display change, adapter/device removed/reset injection, context recreation
- shutdown during metrics wait, raster lease, fence wait, present

합격 기준:

- hang/crash/leak 0
- stale device/context use 0
- terminal 누락/중복 0
- first-frame/restore blank exposure 0

### C9 — clean publish와 product provenance

작업:

- target manifest를 `HwndExactCpp`, `Win32.ChildHwnd`, managed ANGLE/EGL-D3D11/Skia backend identity로 갱신한다.
- runner fail-fast를 실제 backend launch로 교체한다.
- clean checkout restore/build/test/publish/install-like launch를 수행한다.
- native runtime dependency, SHA-256, adapter/device/backend identity를 evidence에 기록한다.

검증:

- solution/product Release build
- relevant unit/contract/integration test
- self-contained unpackaged app-directory launch
- native DLL missing/wrong architecture/wrong version에 명시적 fail-fast
- validation-only artifact와 PATH fallback 0

### C10 — visible/cadence A/B hard gate

같은 PC, monitor, DPI, refresh rate, window size에서 다음 실행물을 비교한다.

1. bare native C++ HWND/DXGI control
2. managed SkiaSharp frame-lease spike
3. Doroti `HwndExactCpp` product host

pinned Flutter checkout은 source protocol 기준으로만 사용하며 runtime build/capture 비교를 재개하지 않는다.

각 실행물에서 left/right/top/bottom과 네 corner를 slow/medium/fast/reverse로 반복한다. WGC/Desktop Duplication 자동 capture와 사용자 물리 화면 판정을 별도 evidence로 남긴다.

관측 항목:

- white/transparent band
- stale stretch 또는 edge repetition
- child가 outer client를 따라가지 못한 frame
- left/top origin-moving jitter
- content phase와 present cadence
- timeout, superseded generation, resize/present/device failure

**Hard stop C10:** 자동 capture와 cadence가 PASS해도 물리 화면 PASS가 없으면 제품 resize 해결로 승격하지 않는다. 물리 PASS도 테스트한 edge/speed/DPI/monitor 범위에만 한정한다.

### C11 — default cutover와 회귀

C0-C10이 모두 통과한 뒤에만 수행한다.

- Windows App SDK target의 default adapter를 `HwndExactCpp`로 확정한다.
- MAUI backend는 독립 선택지로 유지한다.
- template/CLI/package/docs/diagnostics backend identity를 일치시킨다.
- 전체 platform regression을 수행하되 다른 platform 문제를 이 backend PASS로 덮지 않는다.

## 9. evidence ledger 규칙

각 gate는 다음 증거를 분리한다.

| evidence          | 의미                                          | 의미하지 않는 것                         |
| ----------------- | --------------------------------------------- | ---------------------------------------- |
| sourceContract    | pinned source와 계획 mapping 확인             | binary/runtime 동작                      |
| build             | compile/link/publish 성공                     | window launch, GPU context, visible 화면 |
| automatedContract | state/ABI/terminal/exactness 확인             | DWM scan-out, 물리 화면, 실제 IME/UIA    |
| runtimeDiagnostic | process, topology, device, callback 실행 확인 | 사용자-visible 품질                      |
| capture           | 특정 capture 경로의 pixel/cadence 관측        | 실제 monitor scan-out과 완전 동일        |
| physicalManual    | 지정한 환경/동작의 사용자-visible 판정        | 다른 edge/DPI/monitor의 일반화           |

각 gate 결과는 `PASS | FAIL | BLOCKED | notVerified` 중 하나로 기록한다. 실행하지 않은 항목을 성공으로 추론하지 않는다.

### 9.1 2026-08-25 실행 ledger

사용자 요청에 따라 100회 이상으로 계획된 반복 검증은 약 10회 수준으로 축소했다. 시간 기반 `100 ms` 계약과 `100/125/150/175/200%` DPI matrix는 반복 횟수가 아니므로 유지한다.

| gate             | 결과        | 이번 실행의 근거                                                                                                                                                                                                                                                                                                                                     | 별도 경계                                                                                                                                                                                                        |
| ---------------- | ----------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| C0               | PASS        | pinned Flutter commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`, 5 files, 12 anchors, 3 mappings, source fingerprint `0113e56a1a0c895f1e832868f6344b2e4973d0c4c86b2151d308371804979ffa`                                                                                                                                                             | sourceContract만 PASS. 구현/runtime/visible은 `notVerified`                                                                                                                                                      |
| C1               | PASS        | x64 Release native DLL, managed/native ABI v1 layout(`Host=96`, `Callbacks=88`, pointer packet 128, key packet 72), ABI GPU pointer 0, empty `PATH` app-directory load, 3 exports, self-contained publish의 product native DLL exactly once와 SHA-256 `1a28279633d9d4b44aeb19766049c9db68c89299270ff886f322403362378239`, bootstrap DLL exactly once | ABI/build/publish provenance만 PASS. product render/visible은 별도 gate                                                                                                                                          |
| C2               | PASS        | 10-cycle + 1 warmup, topology/AppWindow/minimize/restore 각 10, accepted/terminal 30/30, wrong-size/stale/unaccounted 0, `ResizeBuffers` 20, D3D12 error/corruption 0, device-loss 1/1, GDI 9→9, USER 5→5                                                                                                                                            | standalone automated/runtimeDiagnostic만 PASS. capture/physicalManual은 `notVerified`                                                                                                                            |
| C3-native-lease  | FAIL        | public Vortice/SkiaSharp D3D12 path에서 context acquire/release 2/2, render 13, terminal `Presented/Superseded/Failed = 10/1/2`, fence-after-submit 10, `ResizeBuffers` 10, invalid call 0, per-frame reference leak 0까지 일치했으나 D3D12 debug error ID 1315가 8건 발생                                                                           | `GetGPUDescriptorHandleForHeapStart`가 shader-visible이 아닌 descriptor heap에 호출됨. 오류 필터, reflection/private API, CPU fallback으로 숨기지 않았으며 DXGI report 호출 성공만으로 leak PASS를 주장하지 않음 |
| C3-managed-owner | PASS        | C++ top/child/task HWND 각 1, C++ GPU object와 ABI GPU pointer 0, managed device generation 2, `ResizeBuffers`/submit-fence/copy-fence/present 각 10, invalid call 0, terminal `Presented/Superseded/Failed = 10/1/2`, duplicate 0, operational D3D12 error 0, GDI/USER stable                                                                       | Skia 초기화 ID 1315 error 8건과 operational warning ID 820 10건은 숨기지 않고 별도 기록. automated ownership/runtimeDiagnostic만 PASS; visible/physical은 `notVerified`                                          |
| C3-A ANGLE owner | PASS        | C++ top/child/task HWND 각 1, ABI GPU pointer 0, ANGLE context generation 2, fixed-size surface resize/present/submit/copy 각 10, invalid call 0, EGL/GLES initialization·operational error 0, terminal `Presented/Superseded/Failed = 10/1/2`, duplicate 0                                                                                          | hardware `ANGLE (... Direct3D11 ...)` 확인. automated ownership/resize/runtimeDiagnostic만 PASS; visible/physical은 `notVerified`                                                                                |
| C4               | PASS        | native filtered wait success/timeout 1/1, task completion dispatch 1, top/child recursive dispatch 0, max native wait 109 ms; managed current+latest queue max 2, accepted/terminal 34/34, mismatch/duplicate/unterminated 0, stale present prevented 3, timeout 2                                                                                   | automated coordinator contract만 PASS. visible resize/cadence/physical은 `notVerified`                                                                                                                           |
| C5-D3D12         | FAIL        | synthetic product fixture는 application/session/view attach/detach/shutdown 1/1/1, new scene/replay 1/3, managed present/fence/copy 4/4/4, ABI GPU pointer 0으로 PASS했으나 실제 self-contained Demo scene에서 operational D3D12 error ID 1422가 6건 발생하고 보강된 runner가 exit code 1로 fail-closed                                              | 실패 이력을 보존한다. debug filter, CPU fallback 또는 SkiaSharp source patch로 숨기지 않음                                                                                                                       |
| C5-A ANGLE       | PASS        | 기존 product/Demo 반복 PASS 이력에 더해 direct fixed-size EGL default framebuffer product validator에서 resize request 20→accepted/presented 2/2, present/submit/copy 2/2/0, duplicate/unterminated/failed 0, EGL/GLES error 0                                                                                                                       | automated/runtime presenter PASS. WGC 고속 우측 확장은 baseline gap 29 frames/551.524 ms→7 frames/60.604 ms로 개선됐지만 최대 32 px가 남아 strict visible gate는 `FAIL`; physical은 `notVerified`              |
| C6               | PASS        | pointer lifecycle/capture cancel, key down/up, focus, client-only cursor ownership, Unicode clipboard round-trip에 더해 platform/raster thread 분리, input ingress=platform, framework input dispatch=raster, resize current+latest coalescing을 automated product validator에서 확인                                                                 | automated contract만 PASS. 실제 border drag/capture/re-entry/resize cursor/Alt+Tab/focus는 `notVerified`                                                                                                      |
| C7-C11           | notVerified | 이번 요청에서 실행하지 않음                                                                                                                                                                                                                                                                                                                          | IME/UIA/lifecycle matrix/capture/physical acceptance는 후속 gate                                                                                                                                                 |

현재 전체 상태는 `C5_ANGLE_PRODUCT_PASS_C6_AUTOMATED_PASS_RESIZE_CAPTURE_PARTIAL`이다. D3D12 product branch의 실패는 보존되며 ANGLE 분기가 이를 자동 fallback으로 숨기지 않는다. C6 physical/manual과 C7-C11은 별도 실행 근거 없이 PASS로 올리지 않는다.

## 10. 금지 사항

- ContentIsland/composition surface를 resize 문제 해결용 primary renderer로 되돌리지 않는다.
- child HWND와 composition visual을 동시에 visible front owner로 두지 않는다.
- C++은 swap chain/back buffer/fence를 소유하지 않으며 managed presenter만 GPU object를 소유한다.
- private SkiaSharp reflection, undocumented handle wrapping, unbounded raw pointer lifetime을 사용하지 않는다.
- `ResizeBuffers` 실패를 retry loop, sleep, extra flush로 숨기지 않는다.
- timer throttle, resize debounce, mouse-up geometry replay를 primary fix로 사용하지 않는다.
- old scene을 current generation으로 relabel하지 않는다.
- full-frame stretch, capacity crop, `SetSourceSize`, edge pixel repetition을 exact frame으로 인정하지 않는다.
- automated PASS를 physical/visible, Korean IME, Narrator acceptance로 표현하지 않는다.
- C10 전에는 MAUI default 제거, broad product cutover, 기존 diagnostic work 삭제를 하지 않는다.

## 11. 정확한 시작점과 완료 정의

구현은 **C0 source/contract → C1 versioned ABI/toolchain → C2 standalone native control → C3/C4 managed ownership·coordinator → C5-A ANGLE product → C6 automated thread/input contract**까지 진행됐다. 다음 정확한 재개점은 고속 expansion capture의 잔여 최대 32 px를 없애는 visible-front 전략 결정이다. 현재 금지된 full-frame stretch/edge repetition/dual visible owner를 몰래 사용하지 않는다. 그 automated gate가 통과한 뒤 C6 실제 capture/re-entry/cursor/Alt+Tab/focus 수동 gate로 진행한다.

이 계획의 완료는 다음을 모두 만족한 상태다.

1. A topology가 native C++ owner로 구현되고 source/ABI/resource ownership contract를 통과한다.
2. managed Doroti scene이 exact native backing에 raster되고 every generation이 exactly-once terminal을 가진다.
3. input, Korean IME, UIA, lifecycle, DPI, monitor, device recovery gate가 각각 근거와 함께 통과한다.
4. clean self-contained publish가 app-directory native provenance를 보장한다.
5. C10의 자동 capture/cadence와 사용자 물리 화면 판정이 모두 통과한다.
6. 테스트하지 않은 환경은 명시적으로 `notVerified`로 남긴다.

그 전까지 이 문서가 주장하는 결론은 **A 구성과 C++ native ownership의 구현 경로를 채택했다**는 것뿐이며, resize 문제 해결이나 제품 준비 완료를 주장하지 않는다.

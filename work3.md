# Doroti Windows App SDK + C++ Flutter-style child HWND backend 작업 계획

## 0. 문서 목적과 현재 경계

- 계획 기준일: 2026-08-25, 실행 갱신: 2026-08-26
- 설계 기준: [`idea.md`](idea.md)의 **A. 전용 child HWND + HWND swap chain** 구성
- 구현 방향: Windows native core는 C++20 `.cpp`로 구현하고, Doroti framework와 기존 `SkiaSceneRenderer`는 managed .NET에 유지한다.
- 대상 제품 경계: `Doroti.Host.WindowsAppSdk`와 `Doroti.Target.Windows.WindowsAppSdk.win-x64`
- Windows App SDK 기준: repository에서 이 host에 고정한 exact `2.4.0`, self-contained unpackaged, 우선 `win-x64`
- Flutter source 기준: local checkout `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- test timeout: 각 build/test/validation command 최대 20분
- 문서 상태: **C0-C4 PASS / C5-D3D12 FAIL(이력 보존) / C5-A managed ANGLE/D3D11 current automated PASS / C5-Vulkan automated·physical 이력 보존 / C6 automated PASS·physical notVerified / C7 automated partial PASS·physical notVerified / C8 automated partial PASS·나머지 matrix notVerified / C9 current ANGLE publish PASS / 이전 ANGLE C10 user acceptance는 이력 보존·현재 build physical 재확인 notVerified / C11 WindowsAppSdk default 유지·global regression FAIL**

이 문서는 기존 `WinRtComposition`/ContentIsland presentation 계획을 대체하는 새 작업계획이다. 현재 worktree의 WinRT/ContentIsland spike와 validator 수정은 이 계획의 구현 또는 PASS 증거로 간주하지 않으며, 후속 작업에서도 사용자 소유 변경으로 보존한다.

이 계획에서 `.cpp 구현`은 다음처럼 해석한다.

- top-level HWND, AppWindow 연결, child render HWND, message-only task HWND, WndProc와 message loop는 C++가 소유한다.
- resize state machine과 running 1 + latest pending 1 queue는 C++가 소유한다. 현재 product child `WM_SIZE`는 Flutter 계약과 같이 exact frame terminal을 최대 100 ms만 기다리며, ANGLE presenter는 최초 surface와 resize surface의 첫 swap 직후 `DwmFlush`를 완료한 다음 terminal을 반환한다.
- 선택된 GPU presenter의 device/context, exact render target, window surface, submit/present는 managed presenter가 단독 소유한다. 현재 product 기본 분기는 ANGLE/EGL-D3D11이며 `EGL_FIXED_SIZE_ANGLE` child surface에 Skia가 직접 raster하고 `eglSwapBuffers`한다. D3D12는 명시적 실패 진단 경로로만 남고 자동 fallback하지 않는다. Vulkan presenter와 Silk.NET Vulkan 의존성은 제거했다.
- C#은 process bootstrap, Doroti application/framework, scene build, `SkiaSceneRenderer`, managed presenter, input/semantics adapter와 versioned C ABI 호출을 소유한다.
- C++는 top-level/child/task HWND와 AppWindow, WndProc, message loop, metrics/input ingress만 소유하며 GPU COM pointer를 생성하거나 ABI로 전달하지 않는다.
- Doroti framework 전체나 공용 UI API를 C++로 재작성하는 것은 범위 밖이다.

## 1. 검토 결론

### 1.1 A 구성 채택

채택할 기본 topology는 다음과 같다.

- top-level: standard `WS_OVERLAPPEDWINDOW` HWND + Windows App SDK `AppWindow`
- render target: 앱이 직접 만든 `WS_CHILD | WS_VISIBLE` child HWND 1개
- renderer input: current generation과 크기가 정확히 일치하는 Doroti scene
- raster target: managed presenter가 만든 current client extent와 정확히 같은 ANGLE default framebuffer/Skia GL surface
- presenter: child HWND에 연결한 hardware D3D11 ANGLE/EGL window surface
- initial/resize handshake: `exact child metrics → EGL fixed-size surface creation/recreation → Skia direct raster → eglSwapBuffers → DwmFlush → terminal wake → first show`
- input/IME/UIA: render topology를 바꾸지 않는 별도 adapter

ContentIsland와 composition swap chain은 A 경로의 renderer나 size authority로 사용하지 않는다. 필요성이 나중에 입증되면 입력·접근성 또는 popup 같은 bounded integration boundary로만 별도 검토한다.

### 1.2 C3 실패 뒤 승인된 ownership 변경

native-owned device/queue/resource를 managed Vortice/SkiaSharp에 frame lease로 전달하는 최초 C3는 D3D12 debug error ID 1315를 8건 발생시켜 실패했다. 사용자는 2026-08-25에 hard-stop 대안 1을 승인했다.

채택한 해법은 managed presenter 단독 GPU ownership이다.

1. C++ platform STA가 top-level/child/task HWND와 resize generation을 소유한다.
2. C++는 immutable metrics와 child HWND identity만 managed 쪽에 전달한다.
3. managed presenter가 ANGLE display/context, exact EGL window surface, `GRContext`와 Skia framebuffer wrapper를 생성·폐기한다.
4. `SkiaSceneRenderer.Paint`, `Flush/Submit`, `eglSwapBuffers`, 최초 surface/resize-present `DwmFlush`는 managed ANGLE presenter가 순서화한다.
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
| exact ANGLE default framebuffer                    | 채택                    | child client와 같은 크기의 `EGL_FIXED_SIZE_ANGLE` surface에 직접 raster해 별도 stretch/copy front를 두지 않는다.                                                                      |
| ContentIsland/composition swap chain                | primary에서 제외        | Flutter child HWND topology와 독립 변수를 흐린다.                                                                                                                                     |
| WinUI XAML, `SwapChainPanel`, MAUI code reuse       | 제외                    | 별도 UI/layout/presentation owner를 만들지 않는다. MAUI backend는 독립 유지한다.                                                                                                      |
| managed ANGLE/EGL-D3D11                             | product 기본으로 채택   | x64 `Avalonia.Angle.Windows.Natives` runtime과 좁은 EGL/GLES P/Invoke를 사용한다. hardware D3D11이 아니면 fail-closed한다.                                                            |
| managed direct Vulkan                               | 구현 제거               | 과거 automated/physical 실패·통과 근거는 아래 historical 절에만 보존하고 presenter 소스, 선택지, runtime-effect backend, Silk.NET 의존성은 제거한다.                               |
| bounded exact child + fixed-size EGL surface        | 채택                    | child와 EGL surface를 current client extent로 맞추고 resize transaction만 최대 100 ms 기다린다. full-frame stretch와 capacity clip은 제외한다.                                      |
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
      |     +-- current resize generation + latest-only dispatch + bounded wait
      |
      +-- managed framework thread
      |     +-- immutable metrics consumption
      |     +-- layout/build
      |     +-- exact Doroti scene publication
      |
      +-- managed raster/presentation thread
            +-- ANGLE/EGL hardware D3D11 context + Skia GRContext
            +-- EGL_FIXED_SIZE_ANGLE child-HWND window surface
            +-- SkiaSceneRenderer.Paint -> Flush/Submit
            +-- eglSwapBuffers -> first-surface/resize DwmFlush -> terminal wake
            +-- C ABI terminal -> native task-HWND completion wake
```

### 2.1 thread ownership

| owner                              | 소유 객체                                                                                                       | 금지 사항                                                                           |
| ---------------------------------- | --------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| native platform STA                | top-level/child/task HWND, AppWindow, WndProc, resize wait state, input ingress                                 | framework layout, Skia raster, GPU fence wait, arbitrary top-level message re-entry |
| managed framework thread           | Doroti view/layout/build, immutable scene와 metrics mailbox                                                     | HWND/AppWindow/GPU presenter 접근                                                   |
| managed raster/presentation thread | ANGLE display/context, exact EGL window surface, `GRContext`, submit/swap, device-loss state                       | AppWindow/child geometry 직접 변경, framework scene mutation                        |

managed presentation thread는 C++ platform STA를 재진입하지 않는다. resize/close 요청은 task HWND packet으로 전달하며, product `WM_SIZE`/geometry 처리는 GPU completion과 독립적으로 진행된다.

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
- `WindowsManagedAngleEglPresenter`: current product의 ANGLE/EGL-D3D11 exact child-surface GPU owner
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
  -> ExactIntermediateReady
  -> ExactBackingReady
  -> Presenting
  -> Presented

terminal: Presented | Superseded | Failed
historical/probe observation flag: PlatformWaitTimedOut = true | false
```

100 ms timeout은 Flutter source mapping과 C4 filtered-wait probe에서 terminal failure가 아니다. 현재 product child `WM_SIZE`도 이 bounded wait를 실행하지만 timeout 뒤 geometry/message pump를 계속 진행한다. 새 generation이 시작되면 이전 generation은 surface 변경 권한을 잃고 정확히 한 번 `Superseded`로 정산한다.

### 4.3 순서

1. top `WM_SIZE`의 current physical client extent로 visible child HWND를 `(0,0,width,height)`에 정확히 맞춘다. child `WM_SIZE`가 metrics와 bounded render transaction의 authority다.
2. 0×0, minimize, 동일 current extent는 별도 lifecycle/no-op 규칙으로 정산하고 blocking resize를 시작하지 않는다.
3. platform thread가 generation을 증가시키고 immutable metrics를 latest render work에 담는다.
4. platform thread는 latest render work를 게시한 뒤 exact terminal을 최대 100 ms 기다린다. timeout은 기록하되 geometry를 rollback하거나 무기한 기다리지 않는다.
5. raster worker가 해당 work의 metrics를 먼저 전달하고, 같은 worker에서 `MetricsChanged`·layout·input dispatch·paint를 순서대로 수행한다.
6. framework/raster 경로는 요청 generation과 width/height가 모두 일치하는 scene만 admission한다. 이미 시작한 generation 뒤에 더 최신 metrics가 도착해도 그 scene을 새 generation으로 relabel하지 않는다.
7. managed presenter는 크기가 바뀌면 old Skia wrapper와 EGL surface를 해제하고 같은 child HWND에 정확한 크기의 `EGL_FIXED_SIZE_ANGLE` surface를 만든다.
8. managed SkiaSharp가 ANGLE default framebuffer에 해당 generation scene을 직접 paint하고 `Flush/Submit`한다. 별도 offscreen snapshot/copy나 scale/stretch는 사용하지 않는다.
9. successful `eglSwapBuffers` 뒤에도 current generation을 재검사한다. 여전히 latest인 exact present만 resize terminal `Presented`가 되고 older work는 `Superseded`다.
10. 새 EGL surface의 첫 successful swap이면 managed presenter가 `DwmFlush`를 완료한 뒤 render callback에서 terminal을 반환한다. 최초 surface도 포함하므로 흰 child background를 first show로 오인하지 않는다.
11. native render worker는 terminal state를 기록하고 platform wait를 깨운 뒤 task HWND에 exact completion을 post한다.
12. 첫 exact content present와 first-surface `DwmFlush`가 모두 끝난 completion에서만 top-level show를 허용한다.

### 4.4 queue와 stale 처리

- running 1 + latest pending 1만 유지한다.
- 새 target이 오면 아직 raster를 시작하지 않은 older pending은 즉시 supersede한다.
- raster를 시작한 generation은 exact-size로 paint/submit할 수 있지만 swap 직전 latest 검사를 통과하지 못하면 present하지 않고 `Superseded`로 정산한다.
- 아직 raster를 시작하지 않은 pending만 latest 1개로 coalesce한다. 완료된 intermediate를 current generation으로 relabel하지 않는다.
- 모든 accepted generation과 scene은 exactly-once terminal을 가진다.
- resize timeout, frame miss, stale rejection, GPU wait, resize, copy, present, DWM flush를 별도 counter/timestamp로 남긴다.

### 4.5 GPU/resource ownership

- managed presenter만 device/queue/fence/backing/swap-chain buffer wrapper를 소유한다.
- C++는 GPU object를 만들거나 GPU pointer를 보관하지 않는다.
- child HWND와 `EGL_FIXED_SIZE_ANGLE` surface를 current target extent와 정확히 맞춘다.
- offscreen snapshot/copy, full-frame provisional stretch, capacity backing을 사용하지 않는다.
- `SetSourceSize`와 edge-pixel 반복은 사용하지 않는다.
- device/context generation이 바뀌면 managed `GRContext`와 모든 GPU wrapper를 폐기한 뒤 새 presenter generation으로 다시 만든다.
- `ResizeBuffers` 실패 시 old front가 valid하다는 사실과 new generation 실패를 구분해 기록한다.
- device removed/reset, adapter change, suspend/resume는 별도 device generation을 만들며 silent software fallback하지 않는다.

## 5. window, DPI, lifecycle 계약

- top-level HWND는 standard caption/border/system menu/Snap behavior를 Windows에 맡긴다.
- child render HWND만 client content를 소유하며 position은 `(0,0)`, extent는 top client와 정확히 같다.
- pointer/IMM32/UIA bounds는 exact child client와 current DPI를 authority로 사용한다.
- top client size, exact child size와 native generation이 render authority다. top-level outer rect는 target metrics가 아니다.
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

### C4 — Flutter-style bounded wait reference와 product async resize coordinator

작업:

- C2/C3를 resize coordinator와 연결한다.
- message-only task HWND, filtered-wait 독립 probe, 100 ms deadline contract와 current+latest queue를 구현한다.
- successful exact `Present`만 completion wake를 발생시키도록 한다.

2026-08-26 Vulkan live-resize 후속에서 product `WM_SIZE`의 100 ms wait는 제거했다. 독립 C4 probe와 Flutter source contract는 그대로 유지하며, product는 platform metrics mailbox와 raster latest-only queue를 비동기로 연결한다.

같은 날 후속 ANGLE 복원 요청에서 위 Vulkan 비동기 product wiring을 폐기하고 `efb4c371`의 bounded resize transaction을 다시 채택했다. current product는 child `WM_SIZE`에서 exact terminal을 최대 100 ms 기다리고, ANGLE fixed-size surface recreation/direct raster/swap/`DwmFlush`가 끝난 뒤 terminal을 반환한다.

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

2026-08-26 ANGLE 후속에서 app background brush로 노출 영역을 칠하는 완화책은 사용자가 문제를 숨길 수 있다고 판정하여 제품 코드에서 제거했다. 당시 ANGLE 제품 경로는 Flutter와 같이 child `WM_SIZE` 안에서 raster worker의 exact-size surface recreation/present를 최대 100 ms만 기다리고, resize present 뒤 `DwmFlush`를 수행했다. product validator는 accepted/presented resize generation `18/18`, failed/unterminated/duplicate `0/0/0`, ANGLE present/submit `18/18`, operational error 0으로 PASS했다. 사용자는 실제 Demo에서 상·하·좌·우 border drag를 확인하여 실시간 추종성과 떨림·래스터 왜곡이 모두 훌륭한 수준이라고 판정했다. 따라서 이 범위의 **physical resize는 PASS**다. 다만 600 px/600 ms reverse 자동 입력은 요청한 edge excursion과 시작 rect 복귀를 완료하지 못했으므로 strict synthetic high-speed gate의 **FAIL**은 별도로 보존한다. 사용하지 않은 `AsyncTransient` 실험 분기는 제품 코드에서 제거했다. 근거는 `.doroti/evidence/hwnd-exact-cpp-c5-angle-product-bounded-resize.json`과 사용자 물리 판정이다.

2026-08-26 현재 요청으로 direct Vulkan 구현을 완전히 제거하고 ANGLE/EGL-D3D11을 product 기본값으로 확정했다. Git의 최초 ANGLE 구현 `e58e7910`과 Vulkan 전환 직전 구현 `efb4c371`을 대조해, 전환 직전의 exact child + child `WM_SIZE` 100 ms bounded wait + `EGL_FIXED_SIZE_ANGLE` + default framebuffer direct raster + resize swap 직후 managed `DwmFlush` 순서를 현재 base API에 이식했다. `WindowsManagedVulkanPresenter`, runner의 `Vulkan` 선택지, `Silk.NET.Vulkan*`, Vulkan runtime-effect backend와 validator 분기는 삭제했다. 명시적 비교 선택지는 `DOROTI_WINDOWS_PRESENTER=D3D12`만 남고 자동 fallback은 없다.

같은 build를 사용자가 `dotnet run --project ./DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Release`로 직접 실행했을 때, 흰 창이 먼저 보이고 resize를 시작해야 content가 나타나는 **first-frame physical FAIL**을 보고했다. 진단상 framework scene은 이미 submitted/presented였으므로 frame build 유실이 아니라 최초 EGL surface만 resize surface와 달리 swap 뒤 `DwmFlush`하지 않는 compositor ordering 차이였다. 새 surface 생성마다 첫 swap 뒤 `DwmFlush`하도록 수정해 terminal/first-show 이전에 DWM 반영을 완료한다. 수정 후 resize 입력 없이 750 ms 시점의 Release Demo capture에 `Doroti Material` content가 나타났고 product validator도 resize `18/18/0`, present/submit/copy `18/18/0`, GPU error 0으로 PASS했다. 근거는 `.doroti/evidence/winappsdk-initial-frame-first-surface-flush.png`와 `.doroti/evidence/hwnd-exact-cpp-c5-angle-first-frame-flush.json`이다. 이 capture는 automated visible evidence이며 사용자의 수정 후 physical 재확인은 아직 `notVerified`다.

검토 중 `Silk.NET.OpenGLES.ANGLE.Native 2025.9.12`의 `win-x64`와 `win-x86` DLL이 같은 SHA-256이고 모두 PE machine `0x014c`인 것을 실제 package에서 확인해 x64 product runtime으로 채택하지 않았다. `Silk.NET.OpenGLES`도 ANGLE presenter의 소수 진단 호출만 대체하므로 제거했다. 현재 x64 ANGLE runtime은 이전 물리 PASS 이력이 있는 `Avalonia.Angle.Windows.Natives 2.1.27548.20260419`의 `av_libglesv2.dll`이며 EGL/GLES boundary는 좁은 P/Invoke와 Skia `eglGetProcAddress` resolver로 유지한다.

복원된 native DLL로 다시 실행한 current product validator는 AMD Radeon 780M의 hardware `Direct3D11` ANGLE renderer, device generation `2`, resize accepted/presented/superseded `18/18/0`, failed/unterminated/duplicate `0/0/0`, present/submit/copy `18/18/0`, initialization/operational EGL/GLES error `0`, ABI GPU pointer `0`으로 **PASS**했다. C4도 filtered wait success/timeout `1/1`, max wait `109 ms`, accepted/terminal `34/34`, duplicate/mismatch/unterminated `0/0/0`으로 PASS했다. 근거는 `.doroti/evidence/hwnd-exact-cpp-c5-angle-pre-vulkan-resize.json`과 `.doroti/evidence/hwnd-exact-cpp-c4-angle-pre-vulkan-resize.json`이다.

환경변수 없이 실행한 실제 `DorotiDemoApp.WindowsAppSdk`도 2회 모두 기본 `ANGLE/EGL-D3D11`과 AMD Radeon 780M hardware D3D11을 선택해 exit `0`, present/submit/copy `1/1/0`, operational error `0`, visible-after-exact-present `true`로 종료했다. 두 run의 기존 `RenderFlex._reportOverflow` assertion은 presenter와 분리된 UI/layout 진단으로 보존한다.

**C5-A 경계:** current build/runtime/automated contract는 PASS다. 과거 같은 ANGLE runtime/direct-raster 계열의 상·하·좌·우 physical PASS와 C10 사용자 acceptance는 이력으로 보존하지만, presenter/base/native ordering이 다시 변경된 현재 build의 physical/capture 판정으로 자동 이전하지 않는다.

### C5-Vulkan — 제거된 historical direct Vulkan cutover

2026-08-26 당시 product 기본 presenter를 `ANGLE/EGL-D3D11`에서 direct Vulkan으로 교체했다. 이 절의 결과는 삭제 전 진단·비교 이력이며 현재 repository에는 실행 가능한 Vulkan presenter, 선택지, runtime-effect backend 또는 Silk.NET Vulkan 의존성이 없다. 당시 최종 경로는 `Silk.NET.Vulkan 2.23.0`과 `Silk.NET.Vulkan.Extensions.KHR 2.23.0`을 바인딩으로만 사용하며, loader는 restricted search policy 아래 `%SystemRoot%\System32\vulkan-1.dll`을 절대 경로로 열었다.

managed presenter가 Vulkan 1.1 instance, hardware physical device, graphics+present queue, `VK_KHR_win32_surface`, swapchain, capacity device-local backing image, command buffer와 fence를 단독 소유한다. Skia는 `GRContext.CreateVulkan`으로 backing의 generation별 exact viewport에 raster하고, explicit image barrier/copy 뒤 `vkQueuePresentKHR`를 호출한다. acquire, Skia submit과 copy는 5초 bounded fence로 확인하며 모든 `VkResult`를 call site에서 검사한다. WSI가 out-of-date가 되면 성공한 present로 기록하지 않고 해당 terminal을 `Superseded`로 정산한다.

managed owner probe를 독립 process로 2회 실행했고 두 번 모두 NVIDIA GeForce RTX 4060 Laptop GPU에서 device generation `2`, resize/present/submit/copy `10/10/10/10`, invalid call `0`, initialization/operational Vulkan error `0`, duplicate terminal `0`으로 PASS했다. NVIDIA WSI는 최초 초기화 때 process-lifetime USER object 1개를 유지했으며 같은 run의 device recreation에서 추가 증가하지 않았다. validator는 Vulkan에만 USER delta `0..1`을 기록·허용하고 D3D12 비교 경로는 기존 delta `0`을 계속 강제한다.

product validator도 최신 코드로 2회 PASS했다. run 1은 accepted/presented/superseded resize `2/2/0`, present/submit/copy `2/2/2`; run 2는 `9/6/3`, `6/7/7`이었고 두 run 모두 failed/unterminated/duplicate terminal `0/0/0`, operational Vulkan error `0`, visible-after-exact-present `true`, device generation `2`였다. superseded 3건은 WSI actual extent와 일치하지 않는 오래된 generation 또는 out-of-date present를 성공으로 위장하지 않고 폐기한 결과다. 근거는 `.doroti/evidence/hwnd-exact-cpp-c3-vulkan-owner-run{1,2}.json`과 `.doroti/evidence/hwnd-exact-cpp-c5-vulkan-product-run{1,2}.json`이다.

2026-08-26 live-resize 후속에서 Vulkan product `child WM_SIZE`의 exact-present wait를 제거했다. native는 child geometry/metrics를 즉시 게시하고 반환하며, managed는 latest native generation을 atomically 확인한 뒤 raster worker에서만 `MetricsChanged`와 exact scene을 적용한다. 아직 worker가 집지 않은 다른 generation의 pending work, stale WSI frame, shutdown pending은 각각 exactly-once `Superseded`로 정산하고, 동일 generation의 일반 frame 재요청은 resize terminal로 중복 정산하지 않는다. 최종 코드 product validator 2회는 모두 resize `accepted/presented/superseded=19/1/18`, `failed/unterminated/duplicate=0/0/0`, device generation `2`, present/submit/copy `4/4/4`, operational Vulkan error `0`, visible-after-exact-present `true`로 PASS했다. C4 coordinator도 accepted/terminal `34/34`, queue max `2`, mismatch/duplicate/unterminated `0`, native filtered-wait probe max `110 ms`로 회귀 PASS했다. 근거는 `.doroti/evidence/hwnd-exact-cpp-c5-vulkan-live-resize-run{1,2}.json`과 `.doroti/evidence/hwnd-exact-cpp-c4-resize-coordinator.json`이다.

2026-08-26 실제 Demo 후속에서 위 exact-size WSI 재생성/latest-only 조합은 600 px/600 ms 우측 확장 동안 새 exact frame이 거의 모두 supersede되어 old front 밖에 검은/미갱신 영역을 약 594~624 ms 노출했다. ANGLE 당시에는 EGL/DXGI compositor가 old front를 live geometry에 합성해 이 starvation을 가렸지만 direct Vulkan 경로에는 같은 동작이 없었다. product `WM_SIZE`에 100 ms wait를 되돌린 실험은 gap을 없애는 대신 window/cursor edge를 최대 59 px 늦춰 폐기했다.

첫 수정은 child를 현재 monitor work-area 이상의 고정 capacity surface로 유지하고 top parent가 clip한 뒤 last exact backing을 최신 client rect로 provisional stretch했다. 자동 캡처에서는 검은 영역을 줄였지만 사용자가 실제 Demo에서 resize 중 전체 화면이 늘어나는 것을 확인했으므로 이 방식은 **physical FAIL**로 판정하고 product 코드에서 제거했다. 빠른 terminal/metrics 교차로 superseded generation의 metrics가 먼저 제거되던 race 수정만 유지한다.

대체 수정은 이미 raster를 시작한 generation을 최신 metrics 도착만으로 버리지 않는다. 각 generation을 자기 width/height로 layout/paint하고 capacity backing의 나머지는 app background로 clear한 뒤, 완성된 intermediate scene을 원래 크기 그대로 present한다. 더 최신 generation이 있으면 scene은 `submitted`로 보존하되 resize terminal은 정확히 한 번 `Superseded`로 남긴다. pending은 계속 latest 1개로 coalesce하므로 backlog는 늘리지 않는다.

최종 Release product validator 2회는 모두 resize `accepted/presented/superseded=19/1/18`, `failed/unterminated/duplicate=0/0/0`, device generation `2`, present/submit/copy `13/13/13`, operational Vulkan error `0`, visible-after-exact-present `true`로 PASS했다. 이전 provisional 버전의 present `2`와 달리 실제 exact intermediate가 drag 중 계속 제출됨을 확인했다. 600 px/약600 ms 하단 capture 2회는 각각 37 frames, blank/gap/error `0/0/0`, circle aspect failure `1`이었다. 우측 2회는 각각 36 frames, blank/error/final gap `0/0/0`, circle aspect failure `0`이고 직접 확인한 중간 PNG에서 text/grid/circle이 원래 비율로 다시 layout되었다. 다만 비동기 window sample을 사용하는 strict gap oracle은 최대 30~31 px, 575.769 ms와 title scale failure 33을 계속 기록하므로 C10 automated는 **FAIL**, 사용자 재확인 전 physical은 `notVerified`다. 근거는 `.doroti/evidence/hwnd-exact-cpp-c5-vulkan-exact-intermediate-{1,2}.json`, `.doroti/evidence/winappsdk-vulkan-resize-20260826/exact-intermediate-bottom-expand-{1,2}.json`, `exact-intermediate-right-expand-{1,2}.json`이다.

사용자는 이 exact-intermediate 버전의 실제 Demo에서 **우측·하단은 매우 완벽**하다고 판정했으므로 그 두 edge의 확인한 물리 범위는 PASS로 기록한다. 반면 좌측·상단은 계속 떨림이 보여 physical FAIL이었다. 원인은 exact scene을 present할 때만 반대편으로 정렬한 탓에, 두 present 사이 top-level 원점 이동을 기존 Vulkan front가 먼저 따라간 뒤 다음 present에서 되돌아오는 sawtooth motion이었다. managed copy 위치만 바꾸는 첫 opposite-edge 실험은 비율 왜곡은 없었지만 이 떨림을 제거하지 못해 폐기했다.

후속 수정은 `WM_SIZING`의 edge 방향을 native authority로 삼아 좌측 drag에서는 capacity child의 **우측 screen edge**, 상단 drag에서는 **하단 screen edge**를 매 `WM_SIZE`에 동기 고정한다. managed Vulkan copy도 child offset을 읽어 exact scene을 capacity의 같은 반대쪽 끝에 1:1 배치한다. 600 px/600 ms capture를 edge별 2회 실행한 결과 blank/capture error/final gap은 전부 0이고 circle aspect failure는 좌측 `0/0`, 상단 `1/0`, 우측 `0/0`, 하단 `1/1`이었다. strict asynchronous gap oracle은 좌측 최대 24 px·약 297~400 ms, 상단 최대 10~12 px·약 55~127 ms, 우측 최대 31~32 px, 하단 0을 기록하므로 C10 automated FAIL은 유지한다. 직접 확인한 좌/상 중간 PNG에는 stretch가 없었다. 별도 진단 좌측 run은 accepted generation 71, framework-rendered scene 50, Vulkan present/submit/copy `53/53/53`, failed/unterminated/duplicate와 operational error 0이었다. 최신 product validator 2회도 각각 present/submit/copy `6/6/6`, resize `19/1/18`, failed/unterminated/duplicate 0으로 PASS했다. 근거는 `.doroti/evidence/winappsdk-vulkan-resize-20260826/stationary-child-{left,top,right,bottom}-expand-{1,2}.json`과 `.doroti/evidence/hwnd-exact-cpp-c5-vulkan-stationary-child-{1,2}.json`이다. 이 새 방식의 좌측·상단 물리 판정은 사용자 재확인 전 `notVerified`다.

당시 환경변수로 presenter를 지정하지 않은 실제 `DorotiDemoApp.WindowsAppSdk` smoke도 기본 `Vulkan`/RTX 4060을 선택해 present/submit/copy `1/1/1`, operational error `0`, visible-after-exact-present `true`로 종료했다. 이 default 선택은 현재 ANGLE cutback으로 대체됐다. 기존 Demo layout의 `RenderFlex._reportOverflow` assertion은 stderr에 그대로 기록되었으며 Vulkan presenter PASS로 덮지 않고 별도 UI/layout 문제로 보존한다.

self-contained Release publish와 empty `PATH` launch, missing native/wrong architecture/wrong version fail-fast도 `.doroti/evidence/hwnd-exact-cpp-c9-vulkan-publish.json`에서 PASS했다. app directory의 native host/bootstrap과 system32 Vulkan loader 경로·SHA-256을 별도로 기록한다.

**경계:** 위 결과는 build/runtime/automated exact-terminal gate다. 이전 ANGLE 경로에서 사용자가 승인한 상·하·좌·우 physical resize와 C10 acceptance는 역사적 근거로 보존하지만 Vulkan backend의 visible/physical acceptance로 이전하지 않는다. Vulkan Demo의 실제 border drag, strict capture/cadence, IME/UIA와 전체 C8 matrix는 `notVerified`다.

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

2026-08-26 자동 범위에서는 versioned ABI에 IMM32 composition/result state와 text action을 연결하고, UIA root/fragment provider의 `Invoke`/`Value`, current child HWND screen origin+scale bounds, Doroti semantics action dispatch를 검증했다. `한` composing range `0..1` 뒤 `한글` final state와 UIA tap action이 도착했으며 `.doroti/evidence/hwnd-exact-cpp-c7-ime-uia.json`에 기록했다. 이 결과는 **automated partial PASS**다. 실제 두벌식 키보드/후보창/caret/selection/focus restore, Narrator, Accessibility Insights는 `notVerified`다.

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

2026-08-26 자동 범위에서는 minimize→inactive/hidden/paused, restore→resumed, display change, detach lifecycle과 ANGLE context/device generation 재생성을 연결했다. 단일 run과 10회 반복 모두 device generation `2`, resize accepted/presented `18/18`, terminal 누락/중복과 operational GPU error `0`으로 PASS했고 `.doroti/evidence/hwnd-exact-cpp-c8-lifecycle-device.json` 및 `.doroti/evidence/hwnd-exact-cpp-c8-cycle-{1..10}.json`에 기록했다. 이는 **automated partial PASS**일 뿐이며 DPI 100/125/150/175/200%, mixed-monitor, maximize/Snap/system menu/keyboard sizing, 실제 device removal, 각 wait 지점 shutdown, visible first-frame/restore는 `notVerified`다.

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

2026-08-26 Vulkan 제거 뒤 ANGLE default로 self-contained Release publish와 app-directory loader를 다시 검증했다. native host/bootstrap/`av_libglesv2.dll`의 x64 PE와 SHA-256을 기록했고 empty `PATH` launch는 성공했다. missing host, missing ANGLE runtime, wrong architecture, wrong ABI/version 네 경로는 모두 exit `1`로 명시적으로 fail-fast했다. restore/package graph에는 Silk.NET이 없으며 `.doroti/evidence/hwnd-exact-cpp-c9-angle-no-vulkan.json`의 C9는 **PASS**다. installer/MSIX는 범위 밖이며, 이전 전체 `Doroti.Product.slnx` Release build의 Windows-host macOS `sips` FAIL은 재실행 근거 없이 해소된 것으로 보지 않는다.

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

**2026-08-26 acceptance 결정(ANGLE 이력):** 사용자는 당시 ANGLE 실제 화면 판정을 근거로 자동 진단 FAIL을 삭제하거나 PASS로 재분류하지 않는 조건에서 C10 hard stop을 명시적으로 해제했다. 이 예외는 당시 ANGLE build의 최종 acceptance 근거로 보존한다. 현재 cutback build는 같은 ANGLE runtime/direct-raster 계열이지만 base/native ordering이 다시 바뀌었으므로 새 물리 확인 전에는 이 acceptance를 자동 이전하지 않는다.

2026-08-26 현재 제품 빌드의 대표 조건(`Right`, reverse, 600 px/600 ms)을 재실행했다. 첫 시도는 사용자 동시 창 조작 가능성이 보고되어 자동 판정에서 제외했다. 사용자 조작 없이 다시 수행한 clean rerun도 capture runner가 요청 edge excursion과 시작 rect 복귀를 충족하지 못해 exit `1`로 중단했다. 이 결과는 제품 pixel 결함이 아니라 resize-driver input qualification **FAIL**이며 `.doroti/evidence/c10-product-right-reverse-current-failure.json`에 기록했다. 직전 pixel/cadence FAIL(`uncoveredEdgeGapFrames=7`, 최대 gap `32 px`, 최대 연속 `60.604 ms`)은 `.doroti/evidence/c5a-c6-f6r-right-default-final-2.json`에 별도로 보존한다. 사용자는 실제 창을 육안으로 확인한 뒤 이 진단 실패들을 보존하는 조건으로 C10을 통과시키는 최종 acceptance 결정을 내렸다. 따라서 C10은 사용자 결정으로 **PASS**이며 `.doroti/evidence/c10-user-acceptance.json`에 결정과 남은 진단을 분리 기록한다.

### C11 — default cutover와 회귀

C0-C10이 모두 통과한 뒤에만 수행한다.

2026-08-26 C10 사용자 acceptance 뒤 Windows App SDK target/buildTransitive/template/Demo/CLI의 default와 capability를 `HwndExactCpp`로 일치시켰다. 이어 Windows CLI의 기본 backend도 `WindowsAppSdk`로 전환하고 `-WindowsBackend Maui`를 독립 명시 선택지로 유지했다. target/template NuGet package 안의 identity와 native host exactly once, adapter 환경변수 없는 Demo launch exit `0`, 기본 CLI WindowsAppSdk run exit `0`, 명시적 MAUI backend build를 확인했다. 이 Windows cutover 범위는 **PASS**다. 전체 `Doroti.Product.slnx` Release build는 Windows App SDK project까지 성공한 뒤 macOS project가 Windows에 없는 `sips`를 호출해 MSB3073/9009 한 건으로 **FAIL**했다. `.doroti/evidence/c11-default-cutover.json`에 두 결과를 분리했으며 C11 전체는 global regression 실패 때문에 **FAIL**이다.

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
| C1               | PASS        | x64 Release native DLL, managed/native ABI v1 layout(`Host=144`, `Callbacks=120`, pointer packet 128, key packet 72), ABI GPU pointer 0, empty `PATH` app-directory load, 3 exports, self-contained publish의 product native DLL exactly once와 SHA-256 `9ccd39aa768164735de2c306e9eee1026f1b8eaf3f2ca574466784f16627eaae`, bootstrap DLL exactly once | ABI/build/publish provenance만 PASS. product render/visible은 별도 gate                                                                                                                                          |
| C2               | PASS        | 10-cycle + 1 warmup, topology/AppWindow/minimize/restore 각 10, accepted/terminal 30/30, wrong-size/stale/unaccounted 0, `ResizeBuffers` 20, D3D12 error/corruption 0, device-loss 1/1, GDI 9→9, USER 5→5                                                                                                                                            | standalone automated/runtimeDiagnostic만 PASS. capture/physicalManual은 `notVerified`                                                                                                                            |
| C3-native-lease  | FAIL        | public Vortice/SkiaSharp D3D12 path에서 context acquire/release 2/2, render 13, terminal `Presented/Superseded/Failed = 10/1/2`, fence-after-submit 10, `ResizeBuffers` 10, invalid call 0, per-frame reference leak 0까지 일치했으나 D3D12 debug error ID 1315가 8건 발생                                                                           | `GetGPUDescriptorHandleForHeapStart`가 shader-visible이 아닌 descriptor heap에 호출됨. 오류 필터, reflection/private API, CPU fallback으로 숨기지 않았으며 DXGI report 호출 성공만으로 leak PASS를 주장하지 않음 |
| C3-managed-owner | PASS        | C++ top/child/task HWND 각 1, C++ GPU object와 ABI GPU pointer 0, managed device generation 2, `ResizeBuffers`/submit-fence/copy-fence/present 각 10, invalid call 0, terminal `Presented/Superseded/Failed = 10/1/2`, duplicate 0, operational D3D12 error 0, GDI/USER stable                                                                       | Skia 초기화 ID 1315 error 8건과 operational warning ID 820 10건은 숨기지 않고 별도 기록. automated ownership/runtimeDiagnostic만 PASS; visible/physical은 `notVerified`                                          |
| C3-A ANGLE owner | PASS        | C++ top/child/task HWND 각 1, ABI GPU pointer 0, ANGLE context generation 2, fixed-size surface resize/present/submit/copy 각 10, invalid call 0, EGL/GLES initialization·operational error 0, terminal `Presented/Superseded/Failed = 10/1/2`, duplicate 0                                                                                          | hardware `ANGLE (... Direct3D11 ...)` 확인. automated ownership/resize/runtimeDiagnostic만 PASS; visible/physical은 `notVerified`                                                                                |
| C3-Vulkan owner | historical PASS / removed | 삭제 전 독립 실행 2회의 근거만 보존한다. current source/package/runtime에는 Vulkan presenter가 없다. | current executable gate가 아니며 재실행할 수 없음 |
| C4               | PASS        | native filtered wait success/timeout 1/1, task completion dispatch 1, top/child recursive dispatch 0, max native wait 109 ms; managed current+latest queue max 2, accepted/terminal 34/34, mismatch/duplicate/unterminated 0, stale present prevented 3, timeout 2. current product child `WM_SIZE`는 100 ms bounded wait를 사용하고 resize `eglSwapBuffers` 뒤 managed `DwmFlush`가 완료되어야 terminal을 반환 | automated coordinator contract와 current wiring만 PASS. visible resize/cadence/physical은 `notVerified` |
| C5-D3D12         | FAIL        | synthetic product fixture는 application/session/view attach/detach/shutdown 1/1/1, new scene/replay 1/3, managed present/fence/copy 4/4/4, ABI GPU pointer 0으로 PASS했으나 실제 self-contained Demo scene에서 operational D3D12 error ID 1422가 6건 발생하고 보강된 runner가 exit code 1로 fail-closed                                              | 실패 이력을 보존한다. debug filter, CPU fallback 또는 SkiaSharp source patch로 숨기지 않음                                                                                                                       |
| C5-A ANGLE       | PASS        | hardware `ANGLE (... Direct3D11 ...)`, device generation 2, resize accepted/presented/superseded `18/18/0`, present/submit/copy `18/18/0`, failed/unterminated/duplicate·EGL/GLES error 0, ABI GPU pointer 0. first-surface/resize first swap은 `DwmFlush` 뒤 terminal을 반환하며 resize 없는 Release capture에 initial content가 보임 | automated/runtime/initial capture만 PASS. 수정 전 사용자의 first-frame physical FAIL은 보존하고 수정 후 physical은 재확인 전 `notVerified`; strict synthetic resize FAIL도 유지 |
| C5-Vulkan        | historical PASS / removed | 삭제 전 capacity/exact-intermediate/stationary-edge 실험과 validator 결과는 이력으로만 보존한다. current source/package/runtime에는 Vulkan 경로가 없다. | current gate가 아니며 과거 edge별 물리 PASS/FAIL과 strict async oracle FAIL만 진단 이력으로 유지 |
| C6               | PASS        | pointer lifecycle/capture cancel, key down/up, focus, client-only cursor ownership, Unicode clipboard round-trip에 더해 platform/raster thread 분리, input ingress=platform, framework input dispatch=raster, resize current+latest coalescing을 automated product validator에서 확인                                                                 | automated contract만 PASS. 실제 border drag/capture/re-entry/resize cursor/Alt+Tab/focus는 `notVerified`                                                                                                      |
| C7               | notVerified | IMM32 `한` composing→`한글` final, text action, UIA root/fragment `Invoke`/`Value`, current HWND+scale bounds, semantics tap dispatch의 automated partial PASS                                                                                                                                                                                      | 실제 두벌식/후보창/caret/selection/focus restore, Narrator, Accessibility Insights는 `notVerified`                                                                                                              |
| C8               | notVerified | minimize/restore/display-change/detach lifecycle, ANGLE context recreation, 10회 반복에서 device generation `2`, resize accepted/presented `18/18`, terminal 누락/중복·GPU error `0`의 automated partial PASS                                                                                                                                 | DPI/mixed-monitor/window-management/device-removal/wait-point shutdown/visible blank matrix는 `notVerified`                                                                                                     |
| C9               | PASS        | Vulkan/Silk.NET 제거 후 current ANGLE self-contained Release publish, app-directory native host/bootstrap/`av_libglesv2.dll` SHA-256, empty `PATH` success, missing host·missing ANGLE runtime·wrong architecture·wrong version exit `1` fail-fast | installer/MSIX는 범위 밖. 전체 Product solution의 기존 macOS `sips` FAIL은 재실행하지 않음 |
| C10              | notVerified | 과거 ANGLE strict synthetic FAIL과 사용자 physical PASS/C10 acceptance, Vulkan edge별 PASS/FAIL을 모두 역사적 근거로 보존 | current ANGLE cutback build의 left/right/top/bottom/corner slow/medium/fast/reverse를 조건별 2회 capture와 사용자 실제 drag로 다시 판정하기 전에는 현재 C10 PASS로 올리지 않음 |
| C11              | FAIL        | Windows 기본 backend=`WindowsAppSdk`, App SDK adapter/capability=`HwndExactCpp`, target manifest graphics backend=`managed-ANGLE-EGL-D3D11-Skia`. current target/host Release pack은 target→host→`Avalonia.Angle.Windows.Natives` 의존성 사슬을 보존하고, native host exactly once, 무환경변수 Demo와 기본 CLI run exit `0`, 명시적 MAUI build도 PASS | 전체 Product solution은 Windows에서 macOS `sips` 부재(MSB3073/9009)로 FAIL. Windows backend PASS로 숨기지 않음 |

현재 전체 상태는 `C5_ANGLE_CURRENT_AUTOMATED_PASS_C7_AUTOMATED_PARTIAL_PASS_C8_AUTOMATED_PARTIAL_PASS_C9_ANGLE_NO_VULKAN_PASS_C10_ANGLE_CURRENT_NOT_VERIFIED_C11_WINDOWS_CUTOVER_PASS_GLOBAL_REGRESSION_FAIL`이다. D3D12 실패와 삭제 전 Vulkan 실패·통과 이력, 과거 ANGLE physical/C10 acceptance, strict synthetic capture 실패를 모두 보존한다. current presenter 선택은 ANGLE 기본 또는 명시적 D3D12뿐이며 자동 fallback은 없다. C6 input/focus physical/manual, C7 physical IME/UIA, C8 미실행 matrix와 current ANGLE visible/physical은 별도 실행 근거 없이 PASS로 올리지 않는다.

## 10. 금지 사항

- ContentIsland/composition surface를 resize 문제 해결용 primary renderer로 되돌리지 않는다.
- child HWND와 composition visual을 동시에 visible front owner로 두지 않는다.
- C++은 swap chain/back buffer/fence를 소유하지 않으며 managed presenter만 GPU object를 소유한다.
- private SkiaSharp reflection, undocumented handle wrapping, unbounded raw pointer lifetime을 사용하지 않는다.
- `ResizeBuffers` 실패를 retry loop, sleep, extra flush로 숨기지 않는다.
- timer throttle, resize debounce, mouse-up geometry replay를 primary fix로 사용하지 않는다.
- product `WM_SIZE`의 exact terminal wait는 100 ms를 넘기지 않는다. 최초 surface와 resize surface의 첫 swap 직후 managed presenter가 `DwmFlush`하고 그 뒤 terminal/first-show를 진행한다.
- old scene을 current generation으로 relabel하지 않는다.
- provisional full-frame stretch, capacity child/backing, `SetSourceSize`, edge pixel repetition을 사용하지 않는다.
- automated PASS를 physical/visible, Korean IME, Narrator acceptance로 표현하지 않는다.
- C10 전에는 MAUI default 제거, broad product cutover, 기존 diagnostic work 삭제를 하지 않는다.

## 11. 정확한 시작점과 완료 정의

구현은 기존 **C0-C11 WindowsAppSdk/HwndExactCpp** 경계 위에서 product 기본 presenter를 **C5-A ANGLE/EGL-D3D11**로 되돌리고 current C5/C9 automated gate를 통과한 상태다. 가장 가까운 재개점은 실제 Demo의 left/right/top/bottom/corner를 slow/medium/fast/reverse 조건별 2회씩 capture하고 사용자 실제 drag를 함께 판정해 current C10 범위를 결정하는 것이다. 그 다음 macOS 도구가 있는 host에서 전체 Product regression을 재실행해 C11의 global FAIL을 재평가한다. C6 실제 capture/re-entry/resize cursor/Alt+Tab/focus, C7 실제 IME/Narrator/Accessibility Insights와 C8 DPI/monitor/window-management/device/shutdown matrix도 별도 수동/물리 재개점으로 남는다.

이 계획의 완료는 다음을 모두 만족한 상태다.

1. A topology가 native C++ owner로 구현되고 source/ABI/resource ownership contract를 통과한다.
2. managed Doroti scene이 exact child/EGL surface에 raster되고 every generation이 exactly-once terminal을 가진다.
3. input, Korean IME, UIA, lifecycle, DPI, monitor, device recovery gate가 각각 근거와 함께 통과한다.
4. clean self-contained publish가 app-directory native provenance를 보장한다.
5. C10의 자동 capture/cadence와 사용자 물리 화면 판정이 모두 통과하거나, 사용자가 남은 자동 실패를 보존한 채 최종 acceptance 예외를 명시적으로 승인한다.
6. 테스트하지 않은 환경은 명시적으로 `notVerified`로 남긴다.

그 전까지 이 문서가 주장하는 결론은 **A 구성과 C++ native ownership의 구현 경로를 채택했다**는 것뿐이며, resize 문제 해결이나 제품 준비 완료를 주장하지 않는다.

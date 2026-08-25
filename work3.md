# Doroti Windows App SDK + C++ Flutter-style child HWND backend 작업 계획

## 0. 문서 목적과 현재 경계

- 계획 기준일: 2026-08-25
- 설계 기준: [`idea.md`](idea.md)의 **A. 전용 child HWND + HWND swap chain** 구성
- 구현 방향: Windows native core는 C++20 `.cpp`로 구현하고, Doroti framework와 기존 `SkiaSceneRenderer`는 managed .NET에 유지한다.
- 대상 제품 경계: `Doroti.Host.WindowsAppSdk`와 `Doroti.Target.Windows.WindowsAppSdk.win-x64`
- Windows App SDK 기준: repository에서 이 host에 고정한 exact `2.4.0`, self-contained unpackaged, 우선 `win-x64`
- Flutter source 기준: local checkout `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- test timeout: 각 build/test/validation command 최대 20분
- 문서 상태: **planOnly / implementationNotStarted / runtimeNotVerified / visibleNotVerified**

이 문서는 기존 `WinRtComposition`/ContentIsland presentation 계획을 대체하는 새 작업계획이다. 현재 worktree의 WinRT/ContentIsland spike와 validator 수정은 이 계획의 구현 또는 PASS 증거로 간주하지 않으며, 후속 작업에서도 사용자 소유 변경으로 보존한다.

이 계획에서 `.cpp 구현`은 다음처럼 해석한다.

- top-level HWND, AppWindow 연결, child render HWND, message-only task HWND, WndProc와 message loop는 C++가 소유한다.
- resize state machine, 100 ms bounded wait, D3D12 device/queue/fence, exact offscreen resource, HWND swap chain, `ResizeBuffers`, copy, `Present`, `DwmFlush`는 C++가 소유한다.
- C#은 process bootstrap, Doroti application/framework, scene build, `SkiaSceneRenderer`, input/semantics adapter와 versioned C ABI 호출만 소유한다.
- C#은 HWND, swap chain, back buffer 또는 fence의 최종 소유자가 되지 않는다.
- managed SkiaSharp가 native-owned D3D12 resource를 사용하는 구간은 명시적인 **frame lease**로 제한한다. 이 lease 경로의 안전성이 C3 gate에서 증명되지 않으면 제품 통합으로 진행하지 않는다.
- Doroti framework 전체나 공용 UI API를 C++로 재작성하는 것은 범위 밖이다.

## 1. 검토 결론

### 1.1 A 구성 채택

채택할 기본 topology는 다음과 같다.

- top-level: standard `WS_OVERLAPPEDWINDOW` HWND + Windows App SDK `AppWindow`
- render target: 앱이 직접 만든 `WS_CHILD | WS_VISIBLE` child HWND 1개
- renderer input: current generation과 크기가 정확히 일치하는 Doroti scene
- raster target: C++가 만든 exact-size D3D12 offscreen texture
- presenter: child HWND에 연결한 `CreateSwapChainForHwnd`, exact-size back buffers, `DXGI_SCALING_NONE`
- resize handshake: child `WM_SIZE → metrics → exact scene → exact backing → ResizeBuffers → copy → Present → wake → DwmFlush`
- input/IME/UIA: render topology를 바꾸지 않는 별도 adapter

ContentIsland와 composition swap chain은 A 경로의 renderer나 size authority로 사용하지 않는다. 필요성이 나중에 입증되면 입력·접근성 또는 popup 같은 bounded integration boundary로만 별도 검토한다.

### 1.2 C++ 전환의 핵심 위험

A 구성 자체보다 먼저 해결해야 할 위험은 managed renderer와 native GPU owner 사이의 ABI다.

현재 `SkiaSceneRenderer.Paint`는 managed `SKSurface`를 받는다. 반면 C++ presenter가 D3D12 object를 소유하면 managed 쪽은 native device, queue, exact texture를 임의 수명으로 보관해서는 안 된다. 따라서 단순 raw pointer 전달이나 C++/C# 양쪽의 독립 COM wrapper 보관은 허용하지 않는다.

채택할 해법은 한 프레임 동안만 유효한 native frame lease다.

1. C++ raster/presentation thread가 exact backing resource를 준비한다.
2. C++가 device/queue/resource에 대한 명시적 `AddRef`가 포함된 lease descriptor를 managed render callback에 전달한다.
3. callback 안에서만 Vortice wrapper, `GRBackendRenderTarget`, `SKSurface`를 구성하거나 재사용한다.
4. managed raster가 `Flush/Submit`을 마친 뒤 모든 per-frame wrapper를 해제하고 lease를 반환한다.
5. 같은 D3D12 queue에 C++ fence를 signal하여 Skia submit 완료를 순서화한다.
6. C++만 backing-to-back-buffer copy, swap-chain resize와 present를 수행한다.

장기 `GRContext`가 device/queue reference를 유지해야 한다면 host lifetime lease와 frame resource lease를 분리한다. host shutdown은 managed context 해제 ACK를 받은 뒤에만 native device/queue를 파괴한다. 이 순서를 C3에서 자동 검증하지 못하면 이 설계를 채택하지 않는다.

### 1.3 채택·제외 결정

| 항목 | 결정 | 이유 |
|---|---|---|
| existing `Doroti.Host.WindowsAppSdk`/target package | 유지 | runner와 publish boundary를 재사용한다. |
| adapter working name | `HwndExactCpp` | 더 이상 composition backend가 아니므로 `WinRtComposition` 이름을 재사용하지 않는다. |
| C++20 native DLL | 채택 | HWND, task pump, resize protocol, D3D12/DXGI lifecycle의 단일 owner다. |
| C++/WinRT | AppWindow 연결에만 사용 | XAML/ContentIsland renderer를 들이지 않는다. |
| managed `SkiaSceneRenderer` | 유지 | Doroti scene semantics와 기존 renderer를 별도 포팅하지 않는다. |
| native-owned D3D12 frame lease | C3 hard gate 후 채택 | C++ ownership과 managed SkiaSharp raster를 연결한다. |
| `CreateSwapChainForHwnd` + `DXGI_SCALING_NONE` | 채택 | A topology와 exact physical pixel presentation을 유지한다. |
| exact offscreen backing | 채택 | resize 전에 exact scene을 준비하고 back buffer 수명과 분리한다. |
| ContentIsland/composition swap chain | primary에서 제외 | Flutter child HWND topology와 독립 변수를 흐린다. |
| WinUI XAML, `SwapChainPanel`, MAUI code reuse | 제외 | 별도 UI/layout/presentation owner를 만들지 않는다. MAUI backend는 독립 유지한다. |
| ANGLE/EGL | 이 계획에서 제외 | 이번 실험은 D3D12 translation을 검증한다. 실패 시 자동 fallback하지 않는다. |
| large permanent backing, stretch, clip-only resize | 제외 | Flutter exact-surface protocol을 복제하지 못한다. |
| Flutter runtime build/instrumentation | 제외 | pinned source contract만 사용한다. 과거 중단된 비교 실행 경로를 재개하지 않는다. |

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
      +-- native raster/presentation thread (.cpp)
            +-- D3D12 device/queue/fence
            +-- exact offscreen resource
            +-- HWND flip-model swap chain/back buffers
            +-- frame lease callback into managed SkiaSharp
            +-- fence -> ResizeBuffers -> copy -> Present
            +-- task-HWND completion wake -> DwmFlush
```

### 2.1 thread ownership

| owner | 소유 객체 | 금지 사항 |
|---|---|---|
| native platform STA | top-level/child/task HWND, AppWindow, WndProc, resize wait state, input ingress | framework layout, Skia raster, GPU fence wait, arbitrary top-level message re-entry |
| managed framework thread | Doroti view/layout/build, immutable scene와 metrics mailbox | HWND/AppWindow/D3D12 접근 |
| native raster/presentation thread | D3D12/DXGI object, exact backing, swap chain, fence, copy, present, device-loss state | AppWindow/child geometry 직접 변경, managed scene mutation |
| managed render callback | leased resource를 감싼 SkiaSharp surface와 `SkiaSceneRenderer.Paint` | callback 종료 뒤 per-frame resource 보관, swap-chain/back-buffer 접근 |

native raster thread가 managed render callback을 호출하는 동안 같은 thread에서 재진입하지 않는다. managed callback은 UI/platform callback을 기다리거나 동기적으로 window API를 호출하지 않는다.

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
  d3d12_presenter.cpp
  frame_lease.cpp
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
- `WindowsSkiaFrameLease`: native host/context lease와 per-frame resource wrapper
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
- `doroti_windows_d3d12_lease_v1`
  - adapter identity, device/queue/resource pointers, format/state, sample count, host/context generation
- `doroti_windows_frame_terminal_v1`
  - `presented | superseded | failed`, error category, timestamps
- callbacks
  - host ready, metrics, render exact frame, frame terminal, input, text, semantics action, lifecycle, log
- host API
  - request frame/close, set cursor/capture, clipboard, text client/caret, semantics update, diagnostics snapshot

raw pointer가 포함된 lease는 callback scope 밖에서 사용할 수 없고, ABI에 지정된 retain/release 함수 없이 복사·보관할 수 없다.

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
7. native raster thread가 exact backing을 준비하고 managed render callback에 bounded frame lease를 전달한다.
8. managed SkiaSharp가 exact viewport에 paint하고 `Flush/Submit`한 뒤 per-frame wrapper를 해제한다.
9. C++가 같은 queue에 fence를 signal하고 completion을 확인한다.
10. 이전 swap-chain back-buffer reference를 모두 해제한 뒤 필요하면 exact extent로 `ResizeBuffers`한다.
11. backing을 current back buffer로 transition/copy하고 다시 `PRESENT` state로 전환한다.
12. current generation 재검사 후 `Present`한다. successful `Present`만 `Presented`가 된다.
13. task HWND에 exact completion을 post하여 platform wait를 깨운다.
14. raster/presentation thread에서 `DwmFlush`를 호출하고 duration/result를 진단으로 기록한다.
15. 첫 exact present 전 창을 숨기는 정책은 이 시점에만 show를 허용한다.

### 4.4 queue와 stale 처리

- running 1 + latest pending 1만 유지한다.
- 새 target이 오면 아직 raster를 시작하지 않은 older pending은 즉시 supersede한다.
- raster 중 stale이 된 backing은 swap-chain resize/copy/present를 수행하지 않는다.
- `ResizeBuffers` 직전, copy 직전, `Present` 직전에 generation을 다시 검사한다.
- 모든 accepted generation과 scene은 exactly-once terminal을 가진다.
- resize timeout, frame miss, stale rejection, GPU wait, resize, copy, present, DWM flush를 별도 counter/timestamp로 남긴다.

### 4.5 GPU/resource ownership

- swap-chain buffer를 managed callback에 전달하지 않는다.
- managed raster는 native-owned exact offscreen resource만 lease한다.
- capacity backing, clip-only resize, `SetSourceSize`, full-frame stretch를 사용하지 않는다.
- offscreen resource와 swap-chain buffer는 target과 정확히 같은 extent다.
- host/context generation이 바뀌면 managed `GRContext`와 모든 wrapper를 폐기한 뒤 새 lease로 다시 만든다.
- `ResizeBuffers` 실패 시 old front가 valid하다는 사실과 new generation 실패를 구분해 기록한다.
- device removed/reset, adapter change, suspend/resume는 별도 device generation을 만들며 silent software fallback하지 않는다.

## 5. window, DPI, lifecycle 계약

- top-level HWND는 standard caption/border/system menu/Snap behavior를 Windows에 맡긴다.
- child render HWND만 client content를 소유하며 initial position은 `(0,0)`, extent는 parent client의 physical px다.
- child client size가 render authority다. top-level outer rect와 work area는 placement evidence일 뿐 render size가 아니다.
- `WM_DPICHANGED`는 suggested top-level rect 적용과 새 child metrics publication을 분리해 기록한다.
- minimize 시 0-size swap-chain resize를 하지 않고 frame production을 정지한다.
- restore는 새 generation과 exact frame 후 visible terminal을 가져야 한다.
- close는 새 callback admission 중지 → pending terminal 정산 → managed renderer/context release ACK → native GPU idle/release → child/task/top-level destroy 순서다.
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
- 100-cycle create/show/resize/minimize/restore/close
- 0 stale present, 0 wrong-size present, 0 unaccounted generation
- D3D12 debug layer와 DXGI live-object report clean

**Hard stop C2:** native control만으로 exact resource release와 `ResizeBuffers`가 안정적이지 않으면 managed renderer를 붙이지 않는다.

### C3 — managed SkiaSharp ↔ native D3D12 frame lease feasibility

작업:

- native-owned device/queue/exact offscreen resource를 callback-scoped lease로 전달한다.
- managed Vortice wrapper와 SkiaSharp D3D12 `GRContext`/`SKSurface`를 구성한다.
- exact grid를 managed `SkiaSceneRenderer` 또는 최소 Skia callback으로 raster하고 native가 copy/present한다.
- host lifetime lease와 per-frame resource lease를 분리한다.
- normal, stale, exception, timeout, context recreate, shutdown 중 wrapper/COM release 순서를 검사한다.

필수 증거:

- managed callback 종료 뒤 per-frame back/resource reference 0
- native fence가 managed Skia submit 뒤에 완료
- `ResizeBuffers` 1,000-cycle 중 `DXGI_ERROR_INVALID_CALL` 0
- context/device generation 변경 시 old wrapper 사용 0
- callback exception이 process crash나 lease leak 없이 `Failed` terminal로 변환
- D3D12 debug layer error 0, DXGI live-object leak 0

**Hard stop C3:** 위 lease 모델이 공개 API와 안정적인 lifetime rule로 증명되지 않으면 임시 reflection, private SkiaSharp API, raw pointer 장기 보관 또는 CPU readback으로 우회하지 않는다. 이 경우 작업을 멈추고 다음 중 하나를 별도 승인받는다.

1. GPU presenter ownership을 managed 쪽으로 되돌리고 C++는 HWND/task pump만 소유
2. native Skia/libSkiaSharp C ABI와 Doroti scene command serialization을 포함하는 더 큰 native-renderer 계획

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

**Hard stop C7:** physical Korean IME와 accessibility를 자동 smoke만으로 PASS 처리하지 않는다.

### C8 — lifecycle, DPI, monitor, device recovery

검증 matrix:

- 100-cycle launch/close와 resize/minimize/restore
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

- target manifest를 `HwndExactCpp`, `Win32.ChildHwnd`, exact D3D12/Skia backend identity로 갱신한다.
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

| evidence | 의미 | 의미하지 않는 것 |
|---|---|---|
| sourceContract | pinned source와 계획 mapping 확인 | binary/runtime 동작 |
| build | compile/link/publish 성공 | window launch, GPU context, visible 화면 |
| automatedContract | state/ABI/terminal/exactness 확인 | DWM scan-out, 물리 화면, 실제 IME/UIA |
| runtimeDiagnostic | process, topology, device, callback 실행 확인 | 사용자-visible 품질 |
| capture | 특정 capture 경로의 pixel/cadence 관측 | 실제 monitor scan-out과 완전 동일 |
| physicalManual | 지정한 환경/동작의 사용자-visible 판정 | 다른 edge/DPI/monitor의 일반화 |

각 gate 결과는 `PASS | FAIL | BLOCKED | notVerified` 중 하나로 기록한다. 실행하지 않은 항목을 성공으로 추론하지 않는다.

## 10. 금지 사항

- ContentIsland/composition surface를 resize 문제 해결용 primary renderer로 되돌리지 않는다.
- child HWND와 composition visual을 동시에 visible front owner로 두지 않는다.
- C++과 C#이 swap chain/back buffer/fence를 독립적으로 소유하지 않는다.
- private SkiaSharp reflection, undocumented handle wrapping, unbounded raw pointer lifetime을 사용하지 않는다.
- `ResizeBuffers` 실패를 retry loop, sleep, extra flush로 숨기지 않는다.
- timer throttle, resize debounce, mouse-up geometry replay를 primary fix로 사용하지 않는다.
- old scene을 current generation으로 relabel하지 않는다.
- full-frame stretch, capacity crop, `SetSourceSize`, edge pixel repetition을 exact frame으로 인정하지 않는다.
- automated PASS를 physical/visible, Korean IME, Narrator acceptance로 표현하지 않는다.
- C10 전에는 MAUI default 제거, broad product cutover, 기존 diagnostic work 삭제를 하지 않는다.

## 11. 정확한 시작점과 완료 정의

첫 구현 작업은 product source가 아니라 **C0 source/contract → C1 versioned ABI/toolchain → C2 standalone native control** 순서다. 현재 `WinRtComposition` runner의 fail-fast는 C5 product integration 전까지 유지한다.

이 계획의 완료는 다음을 모두 만족한 상태다.

1. A topology가 native C++ owner로 구현되고 source/ABI/resource ownership contract를 통과한다.
2. managed Doroti scene이 exact native backing에 raster되고 every generation이 exactly-once terminal을 가진다.
3. input, Korean IME, UIA, lifecycle, DPI, monitor, device recovery gate가 각각 근거와 함께 통과한다.
4. clean self-contained publish가 app-directory native provenance를 보장한다.
5. C10의 자동 capture/cadence와 사용자 물리 화면 판정이 모두 통과한다.
6. 테스트하지 않은 환경은 명시적으로 `notVerified`로 남긴다.

그 전까지 이 문서가 주장하는 결론은 **A 구성과 C++ native ownership의 구현 경로를 채택했다**는 것뿐이며, resize 문제 해결이나 제품 준비 완료를 주장하지 않는다.

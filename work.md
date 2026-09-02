# Doroti Windows App SDK Silk.NET Vulkan 작업계획

- 작성일: 2026-09-02
- 상태: **plan-only / implementation notRun / qualification notRun / physical acceptance notVerified**
- 대상: `Doroti.Host.WindowsAppSdk`의 opaque `HwndExactCpp` 경로에 Silk.NET 기반 direct Vulkan presenter 추가
- 현재 기본값: managed ANGLE/EGL-D3D11
- 새 선택값: `DOROTI_WINDOWS_PRESENTER=Vulkan`
- 핵심 결정: Vulkan은 명시적 opt-in 경로로만 추가한다. ANGLE 기본값, C++ HWND/input/lifecycle 소유권, `experimentalAcrylic` Composition 경로는 바꾸지 않는다.

## 0. 검토 결론

추가는 가능하지만 2026-08-26에 제거한 구현을 그대로 복원하지 않는다. 당시 direct Vulkan은 build, controlled product probe, self-contained publish까지 통과했지만 실제 방향 전환 border resize에서 `vkQueueSubmit ... ErrorDeviceLost`가 반복돼 제품 경로에서 제거됐다. 자동 PASS를 현재 Vulkan 품질로 재사용하지 않는다.

현재 코드와 제거된 presenter를 대조한 결과, 새 경로가 먼저 해결해야 할 핵심은 바인딩 선택이 아니라 WSI 상태와 수명주기다.

| 검토 항목 | 결론 |
|---|---|
| Silk.NET 패키지 | `Silk.NET.Vulkan`과 `Silk.NET.Vulkan.Extensions.KHR` 2.23.0을 같은 버전으로 중앙 고정한다. 2.23.0은 현재 확인한 stable이며 Vulkan 1.4.336 registry 기반 binding을 제공한다. |
| native runtime | Silk.NET은 binding이다. 제품에 임의의 `vulkan-1.dll`이나 ICD를 복사하지 않고 `%SystemRoot%\System32\vulkan-1.dll`을 명시적으로 연다. loader/driver/필수 extension이 없으면 Vulkan 요청을 fail-fast한다. |
| 과거 acquired-image 결함 | 제거된 구현은 acquire와 copy 뒤 `shouldPresent`가 false가 되면 `vkQueuePresentKHR`도 release도 하지 않고 반환할 수 있었다. 이 경로가 과거 device loss의 단독 원인이었다고 단정하지 않지만, 새 구현에서는 허용하지 않는다. |
| 과거 swapchain retirement | 매 resize마다 `vkDeviceWaitIdle` 뒤 old swapchain을 즉시 파괴했다. queue idle만으로 presentation engine의 참조 종료를 가정하지 않고 present fence로 retirement를 증명한다. |
| 기본 present mode | 첫 제품 후보는 `FIFO`로 고정한다. `MAILBOX`/`IMMEDIATE`는 correctness와 resize gate를 통과한 뒤 별도 진단 비교만 허용한다. |
| Acrylic 관계 | Vulkan은 opaque child-HWND swapchain만 소유한다. `experimentalAcrylic`의 ContentIsland/Presentation/ANGLE 경로와 결합하거나 fallback으로 숨기지 않는다. |

기존 Acrylic 상태는 [2026-09-02 체크포인트](history/26-09-02/windows-appsdk-experimental-acrylic-checkpoint.md)에 보존했다. 그 경로의 600ms resize FAIL, 전체 matrix `notRun`, 실물 `notVerified` 판정은 그대로다.

## 1. 범위와 비범위

### 포함

- `HwndExactCpp`의 기존 visible child HWND에 `VK_KHR_win32_surface`/`VK_KHR_swapchain`을 연결한다.
- managed .NET이 Silk.NET Vulkan instance/device/queue/swapchain, Skia Vulkan context, image/fence/semaphore, present를 소유한다.
- 현재 `WindowsManagedHwndPresenterBase`, `WindowsManagedResizeCoordinator`, `SkiaSceneRenderer`와 terminal ledger를 재사용하되 Vulkan의 acquire commit 경계를 표현하도록 presenter 계약을 보강한다.
- loader, API version, instance/device extension, feature, queue, format, present mode, adapter, package/publish를 capability gate로 만든다.
- 과거 실패를 재현하는 resize stress, device-loss, lifecycle, input, visible, physical acceptance를 별도 evidence로 기록한다.
- Windows App SDK target manifest, self-contained publish, README/ADR은 Vulkan qualification 결과에 맞춰 갱신한다.

### 제외

- ANGLE/EGL-D3D11 기본값 교체
- `experimentalAcrylic` 또는 Composition Swapchain을 Vulkan으로 변경
- `Silk.NET.Windowing`, GLFW, SDL 등 두 번째 창/event loop 도입
- C++로 GPU 객체나 raw pointer를 넘기는 ABI 확장
- CPU readback/upload, GDI/bitmap staging, full-frame stretch
- SkiaSharp private reflection, SkiaSharp fork/patch, 자동 ANGLE fallback
- Vulkan SDK 또는 validation layer를 제품 runtime 필수 의존성으로 배포
- Vulkan 선택과 동시에 public framework API를 추가하는 작업

## 2. 선택과 실패 계약

| 요청 | effective presenter | 계약 |
|---|---|---|
| unset 또는 `AngleD3D11` | ANGLE/EGL-D3D11 | 현재 제품 기본값과 검증 경로를 유지한다. |
| `Vulkan` | Silk.NET direct Vulkan | capability가 하나라도 부족하면 창 표시 전에 원인과 함께 fail-fast한다. ANGLE로 조용히 바꾸지 않는다. |
| `D3D12` | 별도 diagnostic artifact | 기존 동작을 유지한다. |
| `experimentalAcrylic` backdrop | ANGLE-D3D11/Composition | 기존 별도 topology를 유지한다. |
| `experimentalAcrylic` + explicit `Vulkan` | 없음 | 상충 요청으로 pre-window fail-fast한다. 어느 쪽도 조용히 무시하지 않는다. |

Vulkan device loss는 같은 창에서 ANGLE로 전환하지 않는다. raster worker에서 Vulkan device/surface/swapchain을 최대 1회 재생성하고, 재발하거나 복구 gate를 넘으면 accepted generation을 `failed`로 끝내고 명시적으로 종료한다. 자동 fallback은 renderer identity와 evidence를 흐리므로 금지한다.

## 3. 소유권과 불변조건

### 기존 소유권 유지

- native C++: top-level HWND, visible child HWND, message-only task HWND, WndProc, resize/input/lifecycle ingress
- managed common path: current+latest frame scheduling, scene, Skia renderer, input-sequence/resize-generation 판정, terminal ledger
- managed Vulkan presenter: loader, instance, Win32 surface, physical/logical device, queue, Skia Vulkan context, backing image, swapchain images, synchronization, present/retirement
- ABI GPU pointer count: 0

### 공통 frame 불변조건

- accepted generation은 `presented`, `superseded`, `failed` 중 정확히 terminal 하나로 끝난다.
- running 1 + latest pending 1을 유지하며 stale generation을 visible하게 present하지 않는다.
- HWND client physical extent가 surface/swapchain extent의 authority다. idle과 resize settle의 visible content는 exact physical size다.
- WndProc/platform thread는 Vulkan fence, semaphore, acquire, queue idle, swapchain recreation, `DwmFlush`를 기다리지 않는다.
- resize, scene, input generation을 합치거나 old frame을 latest라고 재명명하지 않는다.
- first frame과 새 surface의 첫 exact present ordering은 별도 진단한다. `DwmFlush`는 ordering gate일 뿐 scan-out 증명이 아니다.

### Vulkan acquire 상태 머신

각 acquired image는 다음 상태만 따른다.

`available → acquired → copySubmitted → presented`

또는 stale/close/recreate가 끼면 다음과 같이 끝낸다.

`available → acquired → copySubmitted? → sync signals drained → GPU fence complete → released`

규칙:

1. acquire 전 latest/input predicate가 false면 image를 acquire하지 않고 generation을 `superseded`로 끝낸다.
2. acquire 성공 뒤에는 일반 return/exception 경로가 없다. 반드시 present하거나 `vkReleaseSwapchainImagesKHR`로 release한다.
3. acquire-ready 또는 render-finished binary semaphore를 present가 소비하지 않는 분기에서는 no-op queue submit과 fence로 signal을 소비한 뒤 slot을 재사용한다.
4. copy 제출 뒤 stale가 되면 submit/drain fence 완료 후에만 image를 release한다.
5. release 기능을 위해 `VK_KHR_swapchain_maintenance1` feature를 hard requirement로 둔다. extension/function/feature가 없으면 제품 Vulkan 후보는 capability FAIL이다.
6. acquired image count, outstanding image index, unconsumed semaphore signal, terminal, release/present 결과를 ledger에 남긴다. recreate/dispose 뒤 outstanding과 unconsumed signal은 모두 0이어야 한다.

## 4. capability와 package gate

### NuGet/package

- `Doroti/Directory.Packages.props`에 아래 두 package version을 동일한 2.23.0으로 고정한다.
  - `Silk.NET.Vulkan`
  - `Silk.NET.Vulkan.Extensions.KHR`
- `Doroti.Host.WindowsAppSdk.csproj`에 두 package reference를 추가한다.
- restore graph에서 `Silk.NET.Core`의 단일 resolved version, 취약성 경고, duplicate native asset, RID asset을 기록한다.
- `av_libglesv2.dll`은 ANGLE 기본 경로를 위해 그대로 포함한다. Vulkan을 위해 app-local loader/ICD를 추가하지 않는다.

### loader/instance

- `%SystemRoot%\System32\vulkan-1.dll`의 resolved absolute path와 PE architecture를 검사하고 restricted native context로 연다.
- `vkEnumerateInstanceVersion`과 selected physical device `apiVersion`을 각각 기록한다. 초기 구현 하한은 Vulkan 1.1이다.
- 필수 instance extension:
  - `VK_KHR_surface`
  - `VK_KHR_win32_surface`
  - `VK_KHR_get_surface_capabilities2`
  - `VK_KHR_surface_maintenance1`
- 개발/qualification에서만 `VK_EXT_debug_utils`와 `VK_LAYER_KHRONOS_validation`을 요청한다. 없는 validation layer를 제품 실패로 보지는 않지만 validation run 자체는 `notRun`으로 남긴다.

### physical/logical device

- graphics와 Win32 surface present를 모두 지원하는 queue family를 선택한다. 분리 queue는 첫 후보에서 지원하지 않는다.
- software ICD/CPU device는 거부한다. device name, vendor/device ID, driver version, API version, device type, LUID validity/LUID를 manifest에 남긴다.
- 다중 GPU에서는 명시적 device override 또는 기존 Windows adapter policy와 매칭되는 device를 선택하고, 단순 enumeration 첫 항목 선택은 금지한다.
- 필수 device extension/feature:
  - `VK_KHR_swapchain`
  - `VK_KHR_swapchain_maintenance1`
  - `VkPhysicalDeviceSwapchainMaintenance1FeaturesKHR.swapchainMaintenance1 == VK_TRUE`
- surface가 `VK_IMAGE_USAGE_TRANSFER_DST_BIT`을 지원하지 않거나 exact client extent를 만들 수 없으면 capability FAIL이다.

### format/present policy

- 첫 후보는 `B8G8R8A8_UNORM + VK_COLOR_SPACE_SRGB_NONLINEAR_KHR`를 우선하고 `R8G8B8A8_UNORM`을 검증된 대안으로만 허용한다.
- opaque HWND이므로 supported composite alpha 중 `OPAQUE`를 요구한다.
- present mode는 `FIFO`, pre-transform은 supported current transform, image count는 `minImageCount + 1`을 기본으로 하되 `maxImageCount`로 제한한다.
- 실제 선택값과 fallback 선택 과정을 diagnostics에 남긴다. format/present mode를 조용히 바꾸지 않는다.

## 5. Skia/Vulkan 렌더 구조

1. Silk.NET proc resolver로 `GRVkBackendContext`를 구성한다. instance, physical device, device, graphics queue/index와 negotiated API/extension 정보를 같은 owner가 보유한다.
2. Skia는 device-local offscreen color image를 `GRBackendRenderTarget`/`SKSurface`로 감싼다. swapchain image를 장기 Skia surface로 직접 보유하지 않는다.
3. 첫 correctness 구현은 raster worker 단일 thread와 동일 graphics/present queue를 사용한다.
4. scene paint 뒤 `GRContext.Flush`/`Submit(syncCpu: true)`로 Skia 접근 완료를 증명한 후 Vulkan copy command를 제출한다. 성능 최적화 전에는 암묵적 queue ordering에 의존하지 않는다.
5. backing image는 `COLOR_ATTACHMENT_OPTIMAL ↔ TRANSFER_SRC_OPTIMAL`, acquired image는 `UNDEFINED/PRESENT_SRC_KHR ↔ TRANSFER_DST_OPTIMAL → PRESENT_SRC_KHR` 전이를 명시한다.
6. swapchain image별 사용 이력과 layout을 추적한다. 재사용 image를 매번 무조건 `UNDEFINED`로 가정하지 않는다.
7. copy submit은 acquire-ready semaphore를 기다리고 render-finished semaphore를 signal하며, `vkQueuePresentKHR`는 그 semaphore를 기다린다. present하지 않는 release 분기는 두 binary signal을 명시적으로 drain한다.
8. present wait semaphore는 frame ordinal이 아니라 acquired swapchain image index에 연결해 재사용한다. acquire/submit/drain fence와 present fence는 역할을 분리하고, 각 sync slot은 모든 payload 소비가 증명된 뒤에만 재사용한다.
9. Skia context, Skia surface/backend target, command buffers, synchronization, swapchain/surface, device, instance 순서의 해제 계약을 테스트로 고정한다.

초기 동기식 Skia submit은 성능 최적화 대상이지만 V0~V4 correctness gate 전에는 비동기로 바꾸지 않는다. 이후 성능이 기준선을 넘지 못할 때만 Skia flush semaphore interop을 별도 단계로 설계한다.

## 6. swapchain 생성, resize, retirement

- `WM_SIZE`는 metrics/latest render만 publish한다. swapchain 작업은 raster worker가 수행한다.
- 0×0/minimized target은 swapchain을 만들거나 acquire하지 않고 lifecycle terminal로 끝낸다.
- create/recreate 전에 surface capabilities/formats/present modes를 다시 조회한다.
- 새 `VkSwapchainCreateInfoKHR.oldSwapchain`에 직전 swapchain을 전달하되 즉시 파괴하지 않는다.
- 각 `vkQueuePresentKHR`에 `VkSwapchainPresentFenceInfoKHR` fence를 연결한다. retired swapchain을 참조한 과거 presentation fence가 모두 signal된 뒤에만 그 swapchain과 관련 wait semaphore를 해제한다.
- 연속 recreate는 retired chain을 누적할 수 있으므로 active 1 + retired 최대 2를 hard bound로 둔다. bound를 넘으면 raster worker에서 bounded wait하고 timeout 시 FAIL한다.
- `VK_ERROR_OUT_OF_DATE_KHR`:
  - acquire 전이면 현재 target을 `superseded`하고 최신 extent에서 recreate한다.
  - present 뒤이면 해당 image는 presented/released 계약에 맞게 끝내고 최신 extent를 다시 예약한다.
- `VK_SUBOPTIMAL_KHR`은 이미 acquired된 image를 끝까지 present한 뒤 recreate reason으로 기록한다.
- `VK_ERROR_SURFACE_LOST_KHR`은 Win32 surface를 1회 재생성한다. `VK_ERROR_DEVICE_LOST`는 device recovery 계약으로 이동한다.
- `vkDeviceWaitIdle`은 dispose/device-recovery의 bounded 최종 장벽으로만 사용한다. interactive resize마다 호출하지 않는다.
- `WM_EXITSIZEMOVE`는 최신 physical extent의 final exact generation을 요청한다. 100ms bounded wait는 기존 native 계약을 넘지 않으며 WndProc에서 Vulkan primitive를 직접 기다리지 않는다.

## 7. 구현 단계와 hard gate

각 단계가 FAIL이면 다음 제품 통합 단계로 진행하지 않는다. evidence 없이 체크박스를 완료로 바꾸지 않는다.

### V0. 기준선과 실패 재현 계약

- [ ] 시작 `git status --short`, HEAD, OS/build, Windows App SDK, .NET SDK, GPU/driver/LUID, WDDM, monitor refresh/DPI를 manifest에 기록
- [ ] ANGLE 기본 Release build, managed presenter product probe, first-frame capture, resize/counter baseline 실행
- [ ] 이전 Vulkan source를 `aa3f532^`에서 읽기 전용 reference로 고정하고 제품 코드에 직접 cherry-pick하지 않음
- [ ] 과거 `ErrorDeviceLost`, acquired-without-terminal, immediate old-swapchain destruction을 regression scenario로 문서화
- [ ] Acrylic 체크포인트와 working tree의 사용자 변경 보존 확인

Gate: 현재 ANGLE 기준선이 별도 PASS여야 한다. 기존 checker가 stale reference 때문에 막히면 `blocked`를 PASS로 바꾸지 않고 checker 갱신을 독립 항목으로 둔다.

### V1. Silk.NET capability spike

예상 파일:

- `Doroti/validation/windows-vulkan-capability/`
- `Doroti/eng/validate-windows-vulkan-capability.ps1`
- `Doroti/validation/contracts/windows-vulkan-v0.json`

검사:

- [ ] package 2.23.0 pair와 System32 loader provenance
- [ ] API/instance extension/device extension/feature/queue/adapter/format/present-mode 출력
- [ ] `KhrSwapchainMaintenance1.ReleaseSwapchainImages` 실제 function load와 한 acquired image release
- [ ] validation layer 사용 가능 run에서 error/warning 0
- [ ] unsupported/missing loader, software ICD, missing extension/feature의 명시적 negative fail-fast

Gate: mandatory capability가 하나라도 없으면 이 machine/driver에서 Vulkan 제품 통합은 `blocked-capability`이며 V2로 진행하지 않는다.

### V2. WSI 수명주기 spike

제품 renderer 없이 실제 hidden/visible child HWND와 단색 marker로 상태 머신을 검증한다.

- [ ] acquire→copy→present 1,000회
- [ ] acquire 직후, copy submit 직후, present 직전 각각 forced stale 1,000회
- [ ] 각 forced stale에서 acquire/render-finished signal drain, image release, reacquire 성공과 outstanding/unconsumed signal 0
- [ ] same-image semaphore reuse validation
- [ ] 500회 varying-size recreate와 active 1 + retired ≤2
- [ ] close/minimize/restore/recreate 중 leaked handle, duplicate/missing terminal 0
- [ ] `OUT_OF_DATE`, `SUBOPTIMAL`, `SURFACE_LOST`, injected device reset 분기별 terminal one-to-one

Gate: validation error, acquired leak, reuse-before-fence, unbounded retired swapchain, `DEVICE_LOST`가 하나라도 있으면 FAIL이다.

### V3. optional product presenter 통합

예상 변경 파일:

- `Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs`
- `Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedHwndPresenterBase.cs`
- `Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs`
- `Doroti/src/Doroti.Skia.Rendering/DorotiSkiaRuntimeEffects.cs`
- `Doroti/Directory.Packages.props`
- `Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj`
- `Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64/`

- [ ] `Vulkan` selector 추가, unset default ANGLE 유지, unknown/conflicting value fail-fast
- [ ] target/backend/provenance에 requested/effective presenter와 Vulkan details 기록
- [ ] current renderer/resize/input terminal 계약에 acquire commit/release 결과 연결
- [ ] Vulkan reset 전 `SkiaSceneRenderer.InvalidateGpuContextResources()` ordering 고정
- [ ] `experimentalAcrylic`과 code path/resource가 공유되지 않음을 contract 검사

Gate: explicit Vulkan run만 Vulkan을 만들고, unset/Angle/Acrylic before/after 결과가 동일해야 한다.

### V4. frame correctness와 device lifecycle

- [ ] initial content가 resize 없이 750ms capture에서 보임
- [ ] 10 exact-size resize와 10 present/submit/copy, invalid call/validation error 0
- [ ] frame marker가 generation/extent/input sequence와 일치하고 stale visible present 0
- [ ] transparent/opaque scene의 color channel과 premultiplied 결과 확인
- [ ] shader/runtime effect가 Vulkan backend에서 명시적으로 지원되거나 fail-fast함
- [ ] device reset 10회, app start/close 10회, minimize/restore 10회
- [ ] first frame, first post-resize frame, first post-reset frame terminal 누락 0

Gate: build/counter만으로 PASS하지 않는다. visible marker와 terminal ledger가 함께 맞아야 한다.

### V5. live-resize qualification

동일 session에서 `ANGLE → Vulkan → ANGLE` 순으로 실행하고 environment manifest를 공유한다.

- [ ] left/right/top/bottom과 네 모서리
- [ ] slow/medium/fast, expand/shrink/reverse, 방향 전환 반복
- [ ] 과거 실패 재현용 600px/600ms와 각 edge 10초 이상 stress
- [ ] 같은 case 3회 연속, `vkQueueSubmit`/present/acquire/device error 0
- [ ] 100/125/150/200% DPI, 60/120/144/165Hz 가능한 조합
- [ ] mixed-DPI monitor crossing, Snap, maximize/restore, Alt+Tab, occlusion
- [ ] queue depth ≤2, outstanding acquired ≤1, retired swapchain ≤2, final exact settle
- [ ] ANGLE 같은-run 대비 target→present/visible p95와 max, cadence, timeout 비교

자동 hard gate:

- wrong-size, stale-visible, blank/black band, duplicate/missing terminal, acquired leak, validation error, `DEVICE_LOST` = 0
- final settle generation exact, first post-exit present 누락 = 0
- target→present p95는 같은-run ANGLE + 1 refresh interval 이하, max는 +2 refresh interval 이하
- ANGLE 기준선보다 100ms timeout 수가 늘지 않음

WGC/counter는 physical scan-out이나 사람의 체감을 증명하지 않는다. strict synthetic oracle가 현재 ANGLE에서도 실패한다면 두 결과를 각각 보존하고 Vulkan 성공으로 재분류하지 않는다.

### V6. input, IME, accessibility, packaging

- [ ] hover/click/wheel/drag, resize cursor, focus, keyboard, clipboard
- [ ] 한국어 두벌식 조합/후보창/caret와 selection
- [ ] Narrator 또는 Accessibility Insights의 UIA tree/actions
- [ ] self-contained win-x64 publish와 empty-PATH launch
- [ ] Vulkan managed assemblies 존재, app-local `vulkan-1.dll`/ICD 부재, System32 loader provenance
- [ ] missing loader/driver/extension negative run의 actionable fail-fast
- [ ] package consumer/template/default ANGLE launch 회귀 0
- [ ] Windows target-scoped Release 0 warnings/errors와 가능한 global regression

실행하지 못한 물리/GPU/monitor/IME/UIA/global 항목은 `notVerified`로 남긴다.

### V7. 문서화와 승격 결정

- [ ] ADR-025 후속 ADR에 optional Vulkan owner, synchronization, retirement, failure/fallback 계약 기록
- [ ] README/README.ko.md에 selector, prerequisites, diagnostics, unsupported behavior 기록
- [ ] target manifest에 `defaultRenderer=ANGLE`과 `optionalRenderers=[Vulkan]`을 분리
- [ ] automated evidence와 physical acceptance를 별도 표로 기록
- [ ] Vulkan을 experimental optional로 둘지 supported optional로 승격할지 명시적 결정

기본값을 Vulkan으로 바꾸는 일은 이 계획의 완료 조건이 아니다. 별도 사용자 결정과 전 GPU/driver matrix가 필요하다.

## 8. diagnostics/evidence 계약

Vulkan diagnostics에는 최소 다음을 포함한다.

- requested/effective presenter, Silk.NET package version, loader path/hash/version
- Vulkan instance/device API version, extensions/features, validation enabled 여부
- device name/type/vendor/device/driver/LUID, queue family
- surface format/color space/composite alpha/present mode/image count/extent
- accepted/presented/released/superseded/failed generation 수
- acquire/submit/copy/present/release 결과별 count와 마지막 `VkResult`
- outstanding acquired count/index, max outstanding, acquire/render-finished signal drain 수, submit/present fence 상태
- swapchain generation, recreate reason, active/retired count, retirement latency
- validation message severity/type/VUID, device-loss 시 마지막 256 events ring
- first-frame, resize target, present, visible capture QPC와 terminal attribution

evidence는 `.doroti/evidence/windows-vulkan-<timestamp>-<id>/` 아래 immutable manifest/report/log로 남긴다. build, automated runtime, automated visible, physical visual/input, IME, accessibility, package, global regression은 서로 다른 status field를 사용한다.

## 9. 중단과 rollback

다음 중 하나면 제품 통합 또는 승격을 중단한다.

- 필수 maintenance extension/feature 또는 same graphics+present queue 부재
- validation VUID, acquired image leak, unsafe semaphore reuse, retired swapchain bound 초과
- 방향 전환 resize에서 `VK_ERROR_DEVICE_LOST` 재현
- stale frame, wrong-size frame, black/blank band가 ANGLE 기준선보다 악화
- device reset/close 중 hang 또는 terminal 누락
- Vulkan 때문에 unset ANGLE, D3D12 diagnostic, `experimentalAcrylic`, input/IME/UIA 계약이 회귀

중단 시 제거 가능한 단위는 Vulkan presenter, package references, selector/validator extension이다. ANGLE 기본 경로와 Acrylic 구현을 되돌리거나 숨은 fallback을 넣지 않는다. 실패 source와 evidence는 history에 보존한다.

## 10. 완료 정의

다음이 모두 참일 때 이 계획을 완료로 표시한다.

1. unset은 계속 ANGLE이며 `DOROTI_WINDOWS_PRESENTER=Vulkan`만 direct Vulkan을 선택한다.
2. Silk.NET 2.23.0 package pair와 System32 loader/driver provenance가 재현 가능하다.
3. acquired image가 present/release 없이 남는 경로가 없고, present semaphore/swapchain retirement가 fence로 증명된다.
4. Skia paint→Vulkan copy→present의 queue/layout/lifetime이 validation error 없이 동작한다.
5. 과거 방향 전환 border-resize `ErrorDeviceLost`를 포함한 V5가 3회 연속 통과한다.
6. exact-size/latest-generation/terminal one-to-one과 first-frame/device-reset/lifecycle gate가 통과한다.
7. ANGLE, D3D12 diagnostic, Acrylic, input, IME, UIA, package 경로의 회귀가 없다.
8. 자동 결과와 실제 scan-out/사람의 border drag 결과가 별도로 기록된다.
9. 실행하지 않은 GPU/driver/DPI/refresh/IME/accessibility/global 항목은 `notVerified`로 남는다.
10. ADR/README/target manifest가 실제 effective behavior와 일치한다.

## 11. 검토 자료

### 저장소 내부

- [현재 Windows App SDK ANGLE 결정](Doroti/docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md)
- [2026-08-26 Windows App SDK 요약과 제거된 Vulkan 결과](history/26-08-26/windows-appsdk-hwnd-exact-summary.md)
- 제거된 presenter reference: `git show aa3f532^:Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs`

### upstream

- [Silk.NET.Vulkan 2.23.0](https://www.nuget.org/packages/Silk.NET.Vulkan/2.23.0)
- [Silk.NET.Vulkan.Extensions.KHR 2.23.0](https://www.nuget.org/packages/Silk.NET.Vulkan.Extensions.KHR/2.23.0)
- [Khronos Vulkan loader application interface](https://github.com/KhronosGroup/Vulkan-Loader/blob/main/docs/LoaderApplicationInterface.md)
- [Khronos swapchain recreation sample](https://docs.vulkan.org/samples/latest/samples/api/swapchain_recreation/README.html)
- [Khronos swapchain semaphore reuse guidance](https://docs.vulkan.org/guide/latest/swapchain_semaphore_reuse.html)
- [Vulkan specification](https://registry.khronos.org/vulkan/specs/latest-ratified/pdf/vkspec.pdf)
- [SkiaSharp `GRVkBackendContext` API](https://github.com/mono/SkiaSharp-API-docs/blob/main/SkiaSharpAPI/SkiaSharp/GRVkBackendContext.xml)

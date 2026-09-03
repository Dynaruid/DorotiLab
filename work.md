# Doroti Windows App SDK Silk.NET Vulkan 작업계획

- 작성일: 2026-09-02
- 상태: **Vulkan offscreen + retained-child DirectComposition presentation implemented / user-observed Left·TopLeft PASS / strict repeat 11/12 matrices PARTIAL / Presentation-manager loss recovery notVerified**
- 대상: `Doroti.Host.WindowsAppSdk`의 opaque `HwndExactCpp` opt-in Vulkan 경로
- 현재 기본값: managed ANGLE/EGL-D3D11
- 새 선택값: `DOROTI_WINDOWS_PRESENTER=Vulkan`
- 핵심 결정: Vulkan은 명시적 opt-in 경로로만 유지하고, visible Win32 WSI 대신 same-LUID external memory와 Windows Presentation API를 거쳐 retained child HWND의 native DirectComposition target이 표시를 소유한다. USER32의 proposed RECT는 수정하지 않고 pre-geometry CompositionFrame gate만 적용한다. ANGLE 기본값과 별도 `experimentalAcrylic` 경로는 바꾸지 않는다. 현재 source와 acceptance의 권위 있는 체크포인트는 18절이며 0~17절은 결정 과정과 이전 evidence다.

## 0. 검토 결론

추가는 가능하지만 2026-08-26에 제거한 구현을 그대로 복원하지 않는다. 당시 direct Vulkan은 build, controlled product probe, self-contained publish까지 통과했지만 실제 방향 전환 border resize에서 `vkQueueSubmit ... ErrorDeviceLost`가 반복돼 제품 경로에서 제거됐다. 자동 PASS를 현재 Vulkan 품질로 재사용하지 않는다.

현재 코드와 제거된 presenter를 대조한 결과, 새 경로가 먼저 해결해야 할 핵심은 바인딩 선택이 아니라 WSI 상태와 수명주기다. 2026-09-02 AMD live-resize 후속 계측에서는 Skia paint나 queue retirement가 아니라 매 exact extent의 `vkCreateSwapchainKHR`가 약 19~25 ms를 차지하는 주 병목으로 확인됐다. 2026-09-03에는 top의 exact logical viewport와 grow-only child/swapchain capacity를 분리해 ordinary resize에서 이 재생성 비용을 제거했다. 이후 실물 확인에서 검정/흰 노출과 특히 좌·상단 raster 떨림이 계속 보고되었다. Proposed `WM_SIZING` frame과 pre-geometry flush도 실물에서 실패했으므로 제거하고, Flutter Windows처럼 실제 `WM_SIZE` transaction을 exact present까지 최대 100ms 유지하는 구조로 전환했다. Parent 배경색으로 노출을 가리는 처리는 사용하지 않는다.

| 검토 항목 | 결론 |
|---|---|
| Silk.NET 패키지 | `Silk.NET.Vulkan`과 `Silk.NET.Vulkan.Extensions.KHR` 2.23.0을 같은 버전으로 중앙 고정한다. 2.23.0은 현재 확인한 stable이며 Vulkan 1.4.336 registry 기반 binding을 제공한다. |
| native runtime | Silk.NET은 binding이다. 제품에 임의의 `vulkan-1.dll`이나 ICD를 복사하지 않고 `%SystemRoot%\System32\vulkan-1.dll`을 명시적으로 연다. loader/driver/필수 extension이 없으면 Vulkan 요청을 fail-fast한다. |
| 과거 acquired-image 결함 | 제거된 구현은 acquire와 copy 뒤 `shouldPresent`가 false가 되면 `vkQueuePresentKHR`도 release도 하지 않고 반환할 수 있었다. 이 경로가 과거 device loss의 단독 원인이었다고 단정하지 않지만, 새 구현에서는 허용하지 않는다. |
| swapchain retirement | acquire 전까지 stale 여부를 확정하고 acquire 성공을 presentation commit으로 삼는다. 현재 recreate는 raster worker의 `vkQueueWaitIdle` 뒤 old swapchain을 파괴한다. 이 방식은 실기기/validation에서 동작하지만, Khronos가 설명하듯 unextended Vulkan의 WaitIdle은 presentation-engine resource retirement를 명세상 보장하지 않는다. AMD가 `VK_EXT_swapchain_maintenance1`/present fence를 제공하지 않아 이 항목은 `notVerified`다. |
| 기본 present mode | 첫 제품 후보는 `FIFO`로 고정한다. `MAILBOX`/`IMMEDIATE`는 correctness와 resize gate를 통과한 뒤 별도 진단 비교만 허용한다. |
| Acrylic 관계 | Vulkan은 opaque offscreen raster와 retained-child DirectComposition target을 소유한다. `experimentalAcrylic`의 ContentIsland/Presentation/ANGLE 경로와 결합하거나 fallback으로 숨기지 않는다. |

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
- WndProc/platform thread는 Vulkan API를 직접 호출하지 않는다. Retained Vulkan의 실제 `WM_SIZE`는 authoritative viewport를 발행한 뒤 같은 generation이 raster worker에서 present될 때까지 최대 100ms 기다리고, 성공한 interactive transaction은 반환 전에 `DwmFlush`를 한 번 완료한다. 이는 고정 30/60 fps timer가 아니라 render-driven USER32 backpressure다.
- resize, scene, input generation을 합치거나 old frame을 latest라고 재명명하지 않는다.
- first frame과 새 surface의 첫 exact present ordering은 별도 진단한다. `DwmFlush`는 ordering gate일 뿐 scan-out 증명이 아니다.

### Vulkan acquire 상태 머신

stale generation은 acquire 전에 끝내며, acquired image는 다음 상태만 따른다.

`available → acquired → copySubmitted → presented`

규칙:

1. acquire 전 latest/input predicate가 false면 image를 acquire하지 않고 generation을 `superseded`로 끝낸다.
2. acquire 성공 자체가 presentation commit이다. 그 뒤 최신 generation이 바뀌더라도 해당 image는 반드시 copy하고 `vkQueuePresentKHR`까지 진행하며, post-acquire stale-return/release 분기를 두지 않는다.
3. acquire-ready semaphore는 copy submit이, image-index별 render-finished semaphore는 present가 항상 소비한다. 같은 image를 다시 acquire한 시점에는 이전 present wait가 끝났으므로 해당 semaphore를 안전하게 재사용할 수 있다.
4. recreate는 acquired transaction 밖에서만 시작한다. 현재 구현은 raster worker의 `vkQueueWaitIdle` 뒤 old swapchain과 관련 semaphore를 해제하지만, unextended present-resource retirement의 명세 공백은 별도 `notVerified`다.
5. `VK_KHR_swapchain_maintenance1`, `vkReleaseSwapchainImagesKHR`, present fence는 요구하지 않는다.
6. acquired/presented count, outstanding image index, terminal, acquire/submit/present 결과를 ledger에 남긴다. recreate/dispose 뒤 outstanding은 0이어야 한다.

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
- 개발/qualification에서만 `VK_EXT_debug_utils`와 `VK_LAYER_KHRONOS_validation`을 요청한다. 없는 validation layer를 제품 실패로 보지는 않지만 validation run 자체는 `notRun`으로 남긴다.

### physical/logical device

- graphics와 Win32 surface present를 모두 지원하는 queue family를 선택한다. 분리 queue는 첫 후보에서 지원하지 않는다.
- software ICD/CPU device는 거부한다. device name, vendor/device ID, driver version, API version, device type, LUID validity/LUID를 manifest에 남긴다.
- 다중 GPU에서는 명시적 device override 또는 기존 Windows adapter policy와 매칭되는 device를 선택하고, 단순 enumeration 첫 항목 선택은 금지한다.
- 필수 device extension은 `VK_KHR_swapchain`이다. swapchain-maintenance extension/feature는 진단 정보로만 기록하며 selection 조건으로 사용하지 않는다.
- surface가 `VK_IMAGE_USAGE_TRANSFER_DST_BIT`을 지원하지 않거나 exact client extent를 만들 수 없으면 capability FAIL이다.

### format/present policy

- 첫 후보는 `B8G8R8A8_UNORM + VK_COLOR_SPACE_SRGB_NONLINEAR_KHR`를 우선하고 `R8G8B8A8_UNORM`을 검증된 대안으로만 허용한다.
- opaque HWND이므로 supported composite alpha 중 `OPAQUE`를 요구한다.
- present mode는 `FIFO`, pre-transform은 supported current transform으로 고정한다. presenter가 acquired/copy slot 하나만 운용하므로 image count는 `minImageCount`를 요청하고 driver가 돌려준 실제 개수로 per-image synchronization 배열을 만든다.
- 실제 선택값과 fallback 선택 과정을 diagnostics에 남긴다. format/present mode를 조용히 바꾸지 않는다.

## 5. Skia/Vulkan 렌더 구조

1. Silk.NET proc resolver로 `GRVkBackendContext`를 구성한다. instance, physical device, device, graphics queue/index와 negotiated API/extension 정보를 같은 owner가 보유한다.
2. Skia는 device-local offscreen color image를 `GRBackendRenderTarget`/`SKSurface`로 감싼다. swapchain image를 장기 Skia surface로 직접 보유하지 않는다.
3. 첫 correctness 구현은 raster worker 단일 thread와 동일 graphics/present queue를 사용한다.
4. scene paint 뒤 `GRContext.Flush`/`Submit(syncCpu: false)`로 Skia 작업을 같은 Vulkan queue에 제출한다. 뒤따르는 copy의 명시적 image barrier와 동일-queue ordering으로 접근 순서를 보장하며 raster CPU를 Skia GPU 완료까지 세우지 않는다.
5. backing image는 `COLOR_ATTACHMENT_OPTIMAL ↔ TRANSFER_SRC_OPTIMAL`, acquired image는 `UNDEFINED/PRESENT_SRC_KHR ↔ TRANSFER_DST_OPTIMAL → PRESENT_SRC_KHR` 전이를 명시한다.
6. swapchain image별 사용 이력과 layout을 추적한다. 재사용 image를 매번 무조건 `UNDEFINED`로 가정하지 않는다.
7. copy submit은 acquire-ready semaphore를 기다리고 image-index별 render-finished semaphore를 signal하며, `vkQueuePresentKHR`는 그 semaphore를 항상 기다린다.
8. present wait semaphore는 frame ordinal이 아니라 acquired swapchain image index에 연결한다. 같은 image의 재acquire를 이전 present wait 소비의 증거로 삼는다. Recreate의 old-generation sync slot 해제는 별도 unextended-retirement 한계를 가진다.
9. copy fence는 제출 직후 기다리지 않고 다음 command-buffer/acquire-semaphore 재사용 직전에 최대 한 건만 기다린다. swapchain recreate의 queue-idle도 이 pending copy를 닫는다.
10. Skia context, Skia surface/backend target, command buffers, synchronization, swapchain/surface, device, instance 순서의 해제 계약을 테스트로 고정한다. 정상 해제는 Skia resource release 뒤 context abandon을, device loss는 먼저 abandon한 뒤 wrapper 해제를 사용한다.

비동기 Skia submit과 next-use copy-fence wait는 V0~V4 correctness/lifecycle 재검증 뒤에만 유지한다. 외부 validation-layer 강제 실행에서도 VUID/SYNC-HAZARD 메시지가 없어야 한다.

## 6. swapchain 생성, resize, retirement

- Retained Vulkan은 `WM_SIZING`에서 metrics나 frame을 미리 발행하지 않는다. 실제 `WM_SIZE` client extent가 유일한 authority이며, 해당 generation의 exact present까지 platform thread가 최대 100ms 기다린다. Vulkan API와 swapchain 작업 자체는 raster worker가 수행한다.
- 0×0/minimized target은 swapchain을 만들거나 acquire하지 않고 lifecycle terminal로 끝낸다.
- create/recreate 전에 surface capabilities/formats/present modes를 다시 조회한다.
- 새 `VkSwapchainCreateInfoKHR.oldSwapchain`에 직전 swapchain을 전달하되 즉시 파괴하지 않는다.
- recreate 직전에 raster worker가 `vkQueueWaitIdle`을 호출해 이전 command submission을 끝낸다. 현재는 성공 뒤 old swapchain과 관련 wait semaphore를 해제해 retired chain을 누적하지 않지만, 이는 presentation engine 완료를 명세상 증명하지 못하는 maintenance-free 실용 경로다.
- queue-idle 횟수와 latency를 기록한다. WndProc/platform thread는 이 Vulkan primitive를 직접 호출하지 않지만 bounded exact terminal wait를 통해 그 비용을 간접적으로 관찰할 수 있으므로 interactive resize 성능을 별도 live gate로 판정한다.
- `VK_ERROR_OUT_OF_DATE_KHR`:
  - acquire 전이면 현재 target을 `superseded`하고 최신 extent에서 recreate한다.
  - present 결과이면 해당 acquired image의 presentation terminal을 기록하고 최신 extent를 다시 예약한다.
- `VK_SUBOPTIMAL_KHR`은 이미 acquired된 image를 끝까지 present한 뒤 recreate reason으로 기록한다.
- `VK_ERROR_SURFACE_LOST_KHR`은 Win32 surface를 1회 재생성한다. `VK_ERROR_DEVICE_LOST`는 device recovery 계약으로 이동한다.
- `vkDeviceWaitIdle`은 dispose/device-recovery의 최종 장벽으로만 사용하고, swapchain recreate에는 더 좁은 `vkQueueWaitIdle`을 사용한다.
- `WM_EXITSIZEMOVE`는 최신 physical extent의 final exact generation을 요청한다. 100ms bounded wait는 기존 native 계약을 넘지 않으며 WndProc에서 Vulkan primitive를 직접 기다리지 않는다.

## 7. 구현 단계와 hard gate

각 단계가 FAIL이면 다음 제품 통합 단계로 진행하지 않는다. evidence 없이 체크박스를 완료로 바꾸지 않는다.

### V0. 기준선과 실패 재현 계약

- [x] 시작 `git status --short`, HEAD, OS/build, Windows App SDK, .NET SDK, GPU/driver/LUID, WDDM, monitor refresh/DPI를 manifest에 기록
- [x] ANGLE 기본 Release build, managed presenter product probe, first-frame capture, resize/counter baseline 실행
- [x] 이전 Vulkan source를 `aa3f532^`에서 읽기 전용 reference로 고정하고 제품 코드에 직접 cherry-pick하지 않음
- [x] 과거 `ErrorDeviceLost`, acquired-without-terminal, immediate old-swapchain destruction을 regression scenario로 문서화
- [x] Acrylic 체크포인트와 시작 시 clean working tree 보존 확인

Gate: 현재 ANGLE 기준선이 별도 PASS여야 한다. 기존 checker가 stale reference 때문에 막히면 `blocked`를 PASS로 바꾸지 않고 checker 갱신을 독립 항목으로 둔다.

### V1. Silk.NET capability spike

예상 파일:

- `Doroti/validation/windows-vulkan-capability/`
- `Doroti/eng/validate-windows-vulkan-capability.ps1`
- `Doroti/validation/contracts/windows-vulkan-v0.json`

검사:

- [x] package 2.23.0 pair와 System32 loader provenance
- [x] API/instance extension/device extension/feature/queue/adapter/format/present-mode 출력
- [x] acquire 전 latest 판정과 acquire 성공 뒤 unconditional copy/present 정책을 capability contract로 고정
- [x] validation layer 사용 가능 run에서 error/warning 0
- [x] unsupported/missing loader, software ICD, missing extension/feature의 명시적 negative fail-fast

Gate: mandatory capability가 하나라도 없으면 이 machine/driver에서 Vulkan 제품 통합은 `blocked-capability`이며 V2로 진행하지 않는다.

### V2. WSI 수명주기 spike

제품 renderer 없이 실제 visible child HWND와 단색 marker로 상태 머신을 검증한다. 2026-09-02 사용자 피드백에 따라 기본 qualification은 필요한 분기만 짧게 실행한다. 반복 부하 검사는 `--wsi-stress`, 원래의 1,000/500회 검사는 `--wsi-soak` 명시 실행으로 분리하며 기본 gate나 후속 작업에서 자동 실행하지 않는다.

- [x] acquire→copy→present 3회 qualification (`--wsi-stress`: 25회, `--wsi-soak`: 1,000회 선택)
- [x] acquire 전 forced stale 3회는 acquire 없이 supersede하고, acquire 직후/copy 직후/present 직전 forced stale 각 3회는 committed image를 끝까지 present (`--wsi-stress`: 각 25회, `--wsi-soak`: 각 1,000회 선택)
- [x] 모든 post-acquire forced stale에서 release/drain 없이 present, reacquire 성공과 outstanding/unconsumed signal 0
- [x] same-image semaphore reuse validation
- [x] 3회 varying-size recreate와 active 1 + retired ≤2 (`--wsi-stress`: 10회, `--wsi-soak`: 500회 선택)
- [x] close/minimize/restore/recreate 2회 qualification에서 duplicate/missing terminal 0
- [x] 실제 `OUT_OF_DATE`, `SUBOPTIMAL`, `SURFACE_LOST`, `DEVICE_LOST` 결과 주입과 분기별 terminal one-to-one; surface/device loss는 각 1회 복구 뒤 outstanding 0

Gate: validation error, acquired leak, post-acquire early return, unsafe semaphore reuse, unbounded retired swapchain, `DEVICE_LOST`가 하나라도 있으면 FAIL이다.

### V3. optional product presenter 통합

예상 변경 파일:

- `Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs`
- `Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedHwndPresenterBase.cs`
- `Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs`
- `Doroti/src/Doroti.Skia.Rendering/DorotiSkiaRuntimeEffects.cs`
- `Doroti/Directory.Packages.props`
- `Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj`
- `Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64/`

- [x] `Vulkan` selector 추가, unset default ANGLE 유지, unknown/conflicting value fail-fast
- [x] target/backend/provenance에 requested/effective presenter와 Vulkan details 기록
- [x] current renderer/resize/input terminal 계약에 acquire-as-presentation-commit 결과 연결
- [x] Vulkan reset 전 `SkiaSceneRenderer.InvalidateGpuContextResources()` ordering 고정
- [x] `experimentalAcrylic`과 code path/resource가 공유되지 않음을 selector contract로 고정

Gate: explicit Vulkan run만 Vulkan을 만들고, unset/Angle/Acrylic before/after 결과가 동일해야 한다.

### V4. frame correctness와 device lifecycle

- [x] initial content가 resize 없이 750ms 이후 WGC capture에서 보임
- [x] 10 exact-size resize와 present/submit/copy, invalid call/validation error 0
- [x] visible frame marker 단조 증가, final client/swapchain extent exact, stale visible marker 0; input sequence terminal attribution 확인
- [x] opaque background/rect와 반투명 overlap의 BGRA 및 source-over 결과 확인
- [x] shader/runtime effect가 Vulkan backend에서 명시적으로 지원되거나 fail-fast함
- [x] device reset 10회, app start/close 10회, minimize/restore 10회
- [x] 자동 product run에서 first/post-resize/post-reset terminal 누락 0 (물리 scan-out은 별도 `notVerified`)

Gate: build/counter만으로 PASS하지 않는다. visible marker와 terminal ledger가 함께 맞아야 한다.

### V5. live-resize qualification

동일 session에서 `ANGLE → Vulkan → ANGLE` 순으로 실행하고 environment manifest를 공유한다.

- [x] left/right/top/bottom과 네 모서리의 600px/600ms reverse motion
- [ ] slow/medium/fast × expand/shrink/reverse 전체 Cartesian (`-FullCurrentDpiMatrix` 구현, 장시간 실행은 `notRun`)
- [x] 과거 실패 재현용 600px/600ms; 각 edge 10초 이상 stress는 별도 `notRun`
- [x] 같은 8-edge reverse case 3회 연속, `vkQueueSubmit`/present/acquire/device error 0
- [ ] 100/125/150/200% DPI, 60/120/144/165Hz 가능한 조합
- [ ] mixed-DPI monitor crossing, Snap, maximize/restore, Alt+Tab, occlusion
- [x] 실행한 24 case에서 outstanding acquired ≤1, retired swapchain ≤2, final exact settle
- [x] ANGLE 같은-run 대비 target→present p95와 max, presentation cadence/gap, timeout 비교를 validator hard gate로 구현

자동 hard gate:

- wrong-size, stale-visible, blank/black band, duplicate/missing terminal, acquired leak, validation error, `DEVICE_LOST` = 0
- final settle generation exact, first post-exit present 누락 = 0
- target→present p95는 같은-run ANGLE + 1 refresh interval 이하, max는 +2 refresh interval 이하
- ANGLE 기준선보다 100ms timeout 수가 늘지 않음

WGC/counter는 physical scan-out이나 사람의 체감을 증명하지 않는다. strict synthetic oracle가 현재 ANGLE에서도 실패한다면 두 결과를 각각 보존하고 Vulkan 성공으로 재분류하지 않는다.

이전 24-case 자동 결과는 `ANGLE → Vulkan(8 edges × 3 reverse runs) → ANGLE`에서 `PASS-automated-partial`이었지만, 당시 validator는 actual-motion receipt cadence와 target→present hard gate를 계산하지 않았다. 따라서 그 결과를 현재 smoothness PASS로 재사용하지 않는다.

Strict gate를 처음 적용한 `.doroti/evidence/windows-vulkan-smooth-resize-pass11-fifo-strict/manifest.json`의 AMD Radeon 780M left-edge 600 px/600 ms reverse probe는 accepted/presented 27/27, presentation starvation guard, exact settle, resource/error gate를 통과했다. 그러나 target→present p95 25.59 ms가 같은-run ANGLE 기준+1 refresh 허용치 19.62 ms를 넘고 max 27.26 ms도 허용치 26.65 ms를 0.60 ms 초과해 전체 결과는 정직하게 `FAIL`이었다. 이는 아래 최종 pass16 수치와 구분되는 중간 evidence이며, NVIDIA 전체 24-case evidence를 AMD 전체 matrix로 확장한 결과도 아니다.

### V6. input, IME, accessibility, packaging

- [x] 자동 product contract의 hover/click/wheel/drag, resize cursor, focus, keyboard, clipboard
- [ ] 한국어 두벌식 조합/후보창/caret와 selection
- [ ] Narrator 또는 Accessibility Insights의 UIA tree/actions
- [x] self-contained win-x64 publish와 empty-PATH launch
- [x] Vulkan managed assemblies 존재, app-local `vulkan-1.dll`/ICD 부재, System32 loader provenance
- [x] missing loader/driver/extension negative run의 actionable fail-fast
- [x] package consumer/default ANGLE launch 회귀 0
- [x] Windows target-scoped Release 0 warnings/errors (global regression은 `notRun`)

실행하지 못한 물리/GPU/monitor/IME/UIA/global 항목은 `notVerified`로 남긴다.

### 현재 evidence

- proposed-size admission 종합 qualification: `.doroti/evidence/windows-vulkan-wmsizing-prepare-capability-rerun2/manifest.json` — `PASS`; AMD capability/real WSI/product, 5종 actual result injection, reset/minimize-restore/resize/start-close, loader/package negative, source→binary correspondence 통과
- proposed-size admission final source-locked 8-edge live reverse 3회: `.doroti/evidence/windows-vulkan-wmsizing-prepare-final/manifest.json` — `PASS-automated-partial`; 24/24, motion accepted/presented 1,140/1,140, supersede/timeout/marker regression 0, 최소 77.75 Hz, target→present p95 최대 14.54 ms/max 16.65 ms, source/repository/binary endpoint stable
- retained oversized child final source-locked 24-case: `.doroti/evidence/windows-vulkan-retained-surface-final/manifest.json` — `PASS-automated-partial`; 24/24, WGC blank/marker regression/Vulkan error 0, motion 최소 119.15 presentations/s, target→present p95 최대 4.92 ms/max 8.13 ms, source/repository/binary endpoint stable, source→binary correspondence PASS
- retained oversized child 종합 qualification: `.doroti/evidence/windows-vulkan-retained-surface-capability/manifest.json` — `PASS`; AMD capability/WSI/product, actual result injection, reset/minimize-restore/resize/start-close, loader/package negative, source→binary correspondence 통과
- retained oversized child 8-edge live reverse 3회: `.doroti/evidence/windows-vulkan-retained-surface-eight-edges-3x/manifest.json` — `PASS-automated-partial`; 24/24, blank/Vulkan error 0, motion 최소 119.15 presentations/s, target→present p95 최대 4.95 ms, max 최대 9.00 ms, surface recreate process당 1회
- retained oversized child final live probe: `.doroti/evidence/windows-vulkan-retained-surface-live-probe/manifest.json` — `PASS-automated-partial`; Vulkan target→present p95 3.39 ms, presentation 136.17 Hz, surface recreate 1/reuse 85
- retained oversized child product: `.doroti/evidence/windows-vulkan-retained-surface-product.json` — `PASS`; viewport 950x670/surface 1280x752, accepted/presented 10/10, recreate 1/reuse 9, out-of-date/device/surface loss 0
- 종합 qualification: `.doroti/evidence/windows-vulkan-final-qualification7/manifest.json` — `PASS`
- AMD maintenance-free 종합 qualification: `.doroti/evidence/windows-vulkan-amd-acquire-commit-final/manifest.json` — `PASS`; capability/WSI/product, result injection, reset/lifecycle/resize/start-close, ANGLE/package negative gate 통과
- 8-edge live reverse 3회: `.doroti/evidence/windows-vulkan-live-eight-edges-3x-fixed/manifest.json` — `PASS-automated-partial`
- final live probe: `.doroti/evidence/windows-vulkan-live-final-probe/manifest.json` — `PASS-automated-partial`
- diagnostics live probe: `.doroti/evidence/windows-vulkan-live-diagnostics-probe2/manifest.json` — `PASS-automated-partial`; recreate reason/retirement latency/QPC/256-event ring populated
- WGC scene pixel: `.doroti/evidence/windows-vulkan-visible-v3/capture.json` — capture 1, blank/error 0; opaque/alpha-over sample 일치
- self-contained publish: `.doroti/evidence/windows-vulkan-c9-publish-final.json` — `PASS`
- Windows target Release build — 0 warnings/errors
- AMD acquire-as-commit WSI: `.doroti/evidence/windows-vulkan-amd-acquire-commit-postcleanup-wsi.json` — `PASS`; maintenance extension 없이 accepted 24, presented 17, pre-commit superseded 7, outstanding 0
- AMD product/Demo: `.doroti/evidence/windows-vulkan-amd-acquire-commit-postcleanup-product.json`, `.doroti/evidence/windows-vulkan-amd-acquire-commit-demo-v2.json` — `PASS`; effective Vulkan, visible exact present, acquired=presented, device/surface loss 0
- AMD live probe: `.doroti/evidence/windows-vulkan-amd-acquire-commit-postcleanup-live-probe/manifest.json` — `PASS-automated-partial`
- smooth-resize 시작 baseline: `.doroti/evidence/windows-vulkan-smooth-resize-baseline/manifest.json` — 구 validator 기준 `PASS-automated-partial`; Vulkan accepted/presented 20/20, WGC captured/decoded 72/10, capture callback gap p95 24.24 ms
- smooth-resize 최종 FIFO strict probe: `.doroti/evidence/windows-vulkan-smooth-resize-pass16-fifo-final/manifest.json` — `FAIL`; starvation guard `PASS`, target→present ANGLE delta `FAIL`, accepted/presented 27/27, captured/decoded 78/15
- 후속 automated correctness/lifecycle 종합: `.doroti/evidence/windows-vulkan-smooth-resize-capability-final3/manifest.json` — `PASS`; capability/WSI/product/result injection/reset/minimize-restore/resize/start-close. Unextended present-resource retirement의 명세 증명은 이 PASS 범위 밖이다.
- 외부 validation layer 최초 probe: `.doroti/evidence/windows-vulkan-smooth-resize-pass12-validation-layer/manifest.json` — `SYNC-HAZARD-WRITE-AFTER-READ`을 검출한 `FAIL`; acquire wait의 transfer stage와 destination layout-transition barrier source stage가 연결되지 않은 문제를 발견한 evidence로 보존한다.
- barrier 수정 후 외부 validation layer probe: `.doroti/evidence/windows-vulkan-smooth-resize-pass17-validation-final/manifest.json` — performance strict gate는 `FAIL`, starvation guard/resource gate는 `PASS`, manifest `validationMessageCount=0`. 제품 내 debug messenger 활성화를 증명하는 결과는 아니다.
- final source-locked strict probe: `.doroti/evidence/windows-vulkan-smooth-resize-pass24-final/manifest.json` — strict performance `FAIL`; 양끝 HEAD/source/repository/binary fingerprint와 fresh/single-writer 실행을 적용한다.
- final source-locked correctness/lifecycle 재검증: `.doroti/evidence/windows-vulkan-smooth-resize-capability-final10/manifest.json` — `PASS`; capability/WSI/product/5-result injection/reset/minimize-restore/resize/start-close, render-time와 wait-idle-first synthetic device-loss abandon order, 양끝 source/product EXE+IL/probe EXE+IL/managed-host/native SHA-256 기록.
- final source-locked loader-positive 외부 validation probe: `.doroti/evidence/windows-vulkan-smooth-resize-pass25-validation-final/manifest.json` — strict performance `FAIL`; loader의 `VK_LAYER_KHRONOS_validation` 활성화 양성 증거와 validation warning/error 0, resource gate, observer까지 포함한 양끝 binary SHA-256 기록. 제품 내 debug messenger는 계속 비활성이다.

### V7. 문서화와 승격 결정

- [x] ADR-025 후속 ADR에 optional Vulkan owner, synchronization, retirement, failure/fallback 계약 기록
- [x] README/README.ko.md에 selector, prerequisites, diagnostics, unsupported behavior 기록
- [x] target manifest에 `defaultRenderer=AngleD3D11`과 `optionalRenderers=[Vulkan]`을 분리
- [x] automated evidence와 physical acceptance를 별도 표로 기록
- [x] Vulkan은 **experimental optional** 유지로 결정; 물리/matrix gate 전 승격 금지

기본값을 Vulkan으로 바꾸는 일은 이 계획의 완료 조건이 아니다. 별도 사용자 결정과 전 GPU/driver matrix가 필요하다.

## 8. diagnostics/evidence 계약

Vulkan diagnostics에는 최소 다음을 포함한다.

- requested/effective presenter, Silk.NET package version, loader path/hash/version
- Vulkan instance/device API version, extensions/features, validation enabled 여부
- device name/type/vendor/device/driver/LUID, queue family
- surface format/color space/composite alpha/present mode/image count/extent
- accepted/presented/superseded/failed generation 수
- acquire/submit/copy/present 결과별 count와 마지막 `VkResult`
- outstanding acquired count/index, max outstanding, acquire-as-commit/retirement mode
- swapchain generation, recreate reason, active/retired count, queue-idle 횟수와 retirement latency
- swapchain-create/recreate/backing-wrap latency, grow-only backing capacity/allocation/reuse, deferred copy/fence-wait count와 pending bound
- validation message severity/type/VUID, device-loss 시 마지막 256 events ring
- first-frame, resize target, present, visible capture QPC와 terminal attribution

evidence는 `.doroti/evidence/windows-vulkan-<timestamp>-<id>/` 아래 immutable manifest/report/log로 남긴다. build, automated runtime, automated visible, physical visual/input, IME, accessibility, package, global regression은 서로 다른 status field를 사용한다.

## 9. 중단과 rollback

다음 중 하나면 제품 통합 또는 승격을 중단한다.

- 필수 surface/swapchain extension, Vulkan 1.1, hardware device 또는 same graphics+present queue 부재
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
3. stale generation은 acquire 전에 끝나고 acquired image는 항상 copy/present되며 same-image 재acquire로 steady-state semaphore 수명이 닫힌다. Recreate/shutdown의 present-resource retirement도 present fence/wait 또는 동등한 명세 근거로 닫혀야 한다.
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
- [Flutter Windows resize handshake and post-present DwmFlush](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/windows/flutter_windows_view.cc)
- [Flutter Windows child-window sizing](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/windows/host_window.cc)
- [Win32 `WM_SIZING` proposed rectangle contract](https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-sizing)
- [Win32 `DwmFlush` compositor synchronization](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmflush)
- [Win32 child-window clipping](https://learn.microsoft.com/en-us/windows/win32/winmsg/window-features)
- [Vulkan `vkQueuePresentKHR` queueing contract](https://docs.vulkan.org/refpages/latest/refpages/source/vkQueuePresentKHR.html)
- [Vulkan present modes](https://docs.vulkan.org/refpages/latest/refpages/source/VkPresentModeKHR.html)
- [Skia surface API](https://github.com/google/skia/blob/main/include/core/SkSurface.h)
- [SkiaSharp `GRVkBackendContext` API](https://github.com/mono/SkiaSharp-API-docs/blob/main/SkiaSharpAPI/SkiaSharp/GRVkBackendContext.xml)

## 12. 2026-09-02 AMD live-resize smoothness 후속

### 확인한 원인

- 구 validator는 WGC callback cadence만 보고 실제 resize target의 accepted/presented cadence와 target→terminal latency를 보지 않아, 사람이 느낀 버벅임을 `PASS-automated-partial` 안에 숨길 수 있었다.
- 완전 비동기 latest-only 실험은 600 ms 실제 motion 구간에서 새 generation이 swapchain 생성보다 빨리 도착해 Vulkan present가 0까지 굶는 반례를 만들었다. 이를 제품 경로로 채택하지 않았다.
- 최종 FIFO run에서 resize 중 `vkQueueWaitIdle`은 대체로 한 자리 µs, retained backing wrap은 약 0.1 ms였지만 `vkCreateSwapchainKHR`는 대체로 19~25 ms였다. direct Win32 WSI exact-swapchain 재생성의 driver 비용이 남은 cadence/latency 하한이다.

### 적용한 shared-path 수정

- Vulkan만 renderer-owned window surface resource invalidation을 건너뛰고, exact-size Skia wrapper 아래의 device-local backing storage를 grow-only로 재사용한다. 독립 width/height high-water 조합이 512 MiB를 넘거나 후보 image/memory allocation이 OOM이면 submit 전에 후보를 버리고 exact extent로 한 번 재시도하며, exact allocation도 상한·device dimension·실제 allocation 한계를 넘으면 fail-fast한다.
- Skia submit은 CPU 비동기로 바꾸고 copy fence를 다음 one-slot 재사용 시점까지 지연한다. pending copy는 최대 1이며 acquire 성공 뒤 copy/present terminal 계약은 그대로다. Acquire semaphore의 transfer wait와 acquired-image layout transition을 같은 transfer source stage에 연결해 synchronization-validation hazard도 제거한다.
- swapchain은 single acquired slot에 맞춰 surface `minImageCount`를 요청하고, 실제 반환 개수만 신뢰한다. per-image present semaphore는 swapchain generation 사이에 재사용하지 않는다.
- device-lost 복구는 먼저 Skia context를 `AbandonContext(false)`로 abandon하고, 그 뒤 renderer cache/wrapper를 폐기한 다음 native Vulkan handle을 teardown한다. 모든 renderer GPU-resource release 앞에서 `vkDeviceWaitIdle`을 preflight하므로 그 호출이 처음 loss를 보고하는 reset/dispose/surface-recovery 경로도 같은 순서를 따른다. Non-device-loss idle 오류는 성공 복구로 삼지 않고 원본 예외를 보존한다. Context abandon까지 확인되지 않으면 View/capabilities/session/application을 포함한 GPU 소유 graph 전체를 process-lifetime static root로 격리하고 native child handle을 파괴하지 않으며, 비정상 상태가 64회 누적되면 unsafe finalization 대신 fail-fast한다. Synthetic `DEVICE_LOST`와 `DEVICE_LOST_ON_WAIT_IDLE` qualification은 정상 device-loss 호출 순서를 event ledger로 확인하되, 실제 하드웨어 device removal과 non-device idle 오류의 실기기 유발은 별도 `notVerified`다.
- interactive `WM_SIZE`에서는 per-frame `DwmFlush`를 제거했다. 기존 100 ms bounded exact terminal handshake는 유지하고, `WM_EXITSIZEMOVE`/비-interactive final settle만 native raster worker에서 one-shot `DwmFlush`한다. Enter 시 stale request를 지우고 call 직전 interaction/shutdown을 재확인하며, HRESULT 실패는 debugger counter에 남기되 이후 모든 frame에서 무제한 재시도하지 않는다. 이미 시작된 `DwmFlush`와 새 `WM_ENTERSIZEMOVE` 사이의 매우 좁은 TOCTOU는 호출 자체를 취소할 Win32 API가 없어 남는 경계다. 이 동작은 required-feature bit로 협상돼 구 native DLL과 섞이면 조용히 무시하지 않고 fail-fast한다.
- validator는 실제 outer-rect motion 구간의 receipt terminal QPC로 presentation rate/gap, accepted→next-present, target→present p95/max를 산출하고 같은-run `ANGLE → Vulkan → ANGLE` 기준을 hard gate로 판정한다. 시작/종료 HEAD, 전체 tracked diff, untracked file, 핵심 source file과 product EXE/IL·probe EXE/IL·managed/native host·observer binary SHA-256을 양끝에서 비교한다. Build를 생략하면 source→binary correspondence를 `notVerified-skip-build`로 낮춘다. Fresh output-directory lock과 두 validator 공통 repo-global lock으로 stale/mixed artifact를 차단한다. 외부 validation run은 loader가 `VK_LAYER_KHRONOS_validation` 삽입을 양성 보고해야 하고, ANGLE 기준이 없는 motion/duration은 strict cadence PASS로 올리지 않는다.

### 결과와 남은 경계

| 항목 | 시작 baseline | 최종 FIFO strict probe | 판정 |
|---|---:|---:|---|
| Vulkan accepted/presented resize generations | 20/20 | pass20 27/27; hardened pass22 29/29 | one-to-one PASS |
| WGC captured/decoded markers | 72/10 | pass20 80/12; hardened pass22 79/13 | transport sampling은 cadence oracle가 아님 |
| 실제 motion presentation rate | 구 validator 미계측 | pass20 34.53 Hz ≥ 32.80; pass22 30.22 Hz < 30.86 | run-variable, reproducibility FAIL |
| target→present p95 | 구 validator 미계측 | pass20 25.93 > 20.31 ms; pass22 39.51 > 20.09 ms | 반복 FAIL |
| target→present max | 구 validator 미계측 | pass20 29.92 > 26.45 ms; pass22 39.51 > 33.49 ms | 반복 FAIL |

양끝 HEAD/source/repository/binary fingerprint가 일치한 자동 correctness/lifecycle qualification은 `PASS`다. 최초 synchronization-validation probe가 acquire/layout barrier hazard를 검출했고, stage 수정 뒤 loader-positive 재실행은 `VK_LAYER_KHRONOS_validation` 활성화와 validation warning/error 0을 확인했다. 그러나 SkiaSharp가 Skia의 최종 Vulkan image layout을 조회/갱신하는 API를 노출하지 않아 현재 validation-product workload 밖의 layout 계약은 별도 `notVerified`이고, unextended present-resource retirement도 위 명세 한계가 남는다. Strict same-run latency gate는 여전히 `FAIL`, 사람의 실제 border drag/scan-out도 `notVerified`이므로 이 작업계획은 완료/아카이브하지 않는다. 현재 direct WSI와 exact-per-size 계약을 유지한 채 남은 driver swapchain-create 하한을 제거할 근거는 확인하지 못했다. ANGLE 수준 이하의 지연과 명세상 증명 가능한 retirement를 함께 요구하면 별도 범위에서 Vulkan offscreen + DXGI/Composition presentation transport 또는 maintenance-capable GPU 경로를 설계·검증해야 한다.

## 13. 2026-09-03 retained oversized child surface 후속

### root cause와 설계 변경

- 이전 Vulkan 경로는 top-level client가 바뀔 때마다 visible child HWND를 exact size로 먼저 바꿨고, child `WM_SIZE`가 새 logical metrics를 발행한 뒤 `vkCreateSwapchainKHR`를 포함한 exact-extent surface recreation이 끝나기를 기다렸다. AMD Radeon 780M에서 보통 19~25 ms, 간헐적으로 96 ms인 driver 비용 동안 compositor가 새 parent 영역에 아직 맞지 않는 child/swapchain을 보게 되어 검은 노출 또는 이전 raster stretch가 발생할 수 있었다.
- Vulkan required-feature ABI를 추가해 visible child HWND를 현재 monitor work area 크기로 시작하는 grow-only capacity에 유지한다. Top HWND는 계속 exact logical viewport를 소유하고 `WS_CLIPCHILDREN`으로 oversized child를 client에 자른다. Win32의 child clipping 계약상 child가 parent 밖으로 그려져도 화면에 노출되지 않는다.
- parent `WM_SIZE`는 capacity 안에서 child `SetWindowPos`를 호출하지 않고 top client 기반 logical metrics만 발행한다. 따라서 ordinary live resize는 Win32 surface/swapchain extent를 바꾸지 않으며, capacity를 넘을 때만 25%/256 px 단위로 child와 surface를 한 번 grow/recreate한다. ANGLE/Acrylic 경로는 이 opt-in feature를 요청하지 않아 기존 동작을 유지한다.
- Skia wrapper와 Vulkan copy는 retained surface capacity 전체를 사용한다. 매 frame capacity 전체를 app background로 clear하고 logical viewport로 clip한 뒤 scene을 그리므로 새로 드러나는 영역은 검정/undefined pixel이나 늘어난 이전 raster가 아니라 현재 app frame이다.
- logical viewport와 physical surface extent를 diagnostics에서 분리하고 `SurfaceWidth/SurfaceHeight`, `SurfaceRecreates`, `RetainedSurfaceReuses`를 추가했다. Validator도 Vulkan에는 `surface >= viewport`와 reuse/recreate를 요구하고 ANGLE의 exact `ResizeBuffers` 계약은 그대로 유지한다.

이 구조는 Flutter Windows의 resize target→raster-thread terminal handshake를 보존하면서, Flutter가 `DwmFlush`로 완화한다고 명시한 “이전 surface가 현재 view에 stretch되는” 구간 자체를 ordinary Vulkan resize에서 제거한다. 또한 Vulkan swapchain-recreation guidance의 old/new swapchain lifetime 복잡성을 per-frame 경로에서 피한다. 별도 composited transport나 intentional raster scaling은 도입하지 않았다.

### 자동 결과

| 검증 | 결과 | 핵심 수치 |
|---|---|---|
| Release native/managed/product build | `PASS` | target-scoped 0 warnings/errors; native DLL SHA-256 `010d0bfe98f8e2a22e15502187cc254d54344c51c6734e35574b3e37adfd3004` |
| product 10 resize | `PASS` | accepted/presented 10/10, viewport 950x670, surface 1280x752, recreate 1/reuse 9, Vulkan 오류 0 |
| same-run live probe | `PASS-automated-partial` | ANGLE before/after p95 13.44/13.49 ms, Vulkan p95 3.39 ms/max 3.88 ms, presentation 136.17 Hz, recreate 1/reuse 85 |
| 8 edges/corners × 3 reverse | `PASS-automated-partial` | 24/24, blank frame/오류 0, 총 motion present 2,017, 최소 119.15 Hz, p95 최대 4.95 ms/max 9.00 ms, recreate 각 process 1/reuse 합계 2,140 |
| capability/lifecycle | `PASS` | AMD capability+real WSI, product, actual five-result injection, resize/reset/minimize-restore/start-close 10회, loader/package negative, source→binary correspondence |

### 남은 경계

- 초기 retained 1280x752 swapchain 생성은 창을 보이기 전에 약 100~150 ms가 들었다. 4K/다중 monitor의 더 큰 capacity가 갖는 VRAM/startup 비용, monitor/DPI 이동 중 capacity growth, Snap/maximize/restore의 실제 시각 결과는 아직 `notVerified`다.
- WGC pixel/cadence와 terminal ledger는 자동 evidence일 뿐 물리 scan-out과 사람이 느끼는 border drag를 증명하지 않는다. 실제 창 재확인과 GPU/driver/DPI/refresh full matrix 전에는 Vulkan을 기본값으로 승격하지 않는다.
- `VK_EXT_swapchain_maintenance1`이 없는 장치의 unextended present-resource retirement와 SkiaSharp가 노출하지 않는 arbitrary-workload Vulkan image-layout 계약은 이전과 같이 별도 specification-level `notVerified`다.
- 따라서 이전 exact-per-extent strict `FAIL`은 역사 evidence로 유지하되, 이번 shared-path 구조에 대한 자동 live-resize 결과는 `PASS-automated-partial`로 별도 기록한다. 작업계획은 physical/matrix/retirement 완료 조건이 남아 있으므로 archive하지 않는다.

## 14. 2026-09-03 실물 resize 보고 후 proposed-size admission 후속

### 관찰과 원인 판정

- retained surface 24-case 자동 PASS 뒤 사용자가 실물 창에서 검정/흰 영역과 raster가 창 크기에 뒤늦게 맞춰지는 느낌, 특히 좌·상단 resize의 내부 떨림을 보고했다. 따라서 이전 자동 결과는 물리 acceptance가 아니며 이 실물 증상은 pre-fix `FAIL-observed`로 기록한다.
- retained child가 swapchain 재생성을 없애도 top-level HWND의 위치/extent는 USER32가 먼저 바꾼다. 좌·상단 drag는 창 origin과 client extent를 동시에 바꾸므로, 직전 logical viewport frame이 이동한 새 origin에 잠시 놓였다가 다음 frame에서 새 layout으로 바뀌는 geometry/presentation phase mismatch가 특히 잘 보인다.
- `vkQueuePresentKHR` 성공은 presentation request가 queue에 들어갔다는 뜻이지 DWM scan-out 완료가 아니다. FIFO를 유지한 채 native resize를 30/60 fps로 고정하면 old frame이 남는 시간을 오히려 늘릴 수 있으므로 고정 limiter는 채택하지 않았다.

### 구현

- Retained Vulkan에만 `WM_SIZING` proposed outer rect를 현재 non-client delta로 client extent로 바꾸고, top geometry가 바뀌기 전에 해당 logical viewport generation을 queue한 뒤 interactive 16 ms bound 안에서 exact terminal을 준비한다.
- 이어지는 실제 `WM_SIZE`가 같은 extent면 준비된 generation을 그대로 승인한다. 다르거나 prepare가 실패/timeout이면 actual client extent를 새 generation으로 republish해 기존 exact 계약으로 복구한다. Prepare state는 enter/exit sizing과 admission 뒤 명시적으로 지운다.
- 준비된 frame이 실제 geometry로 승인된 뒤에만 platform thread가 `DwmFlush`를 호출한다. 이것은 고정 FPS cap이 아니라 현재 DWM cadence에 다음 USER32 geometry step을 맞추는 admission barrier다. ANGLE과 Acrylic은 retained feature를 요청하지 않으므로 `WM_SIZING` 동작이 바뀌지 않는다.
- 자동 lifecycle smoke는 첫 show/device-reset과 겹친 minimize 명령이 실제 `SIZE_MINIMIZED`에 도달하지 않았는데 phase를 넘길 수 있었다. `IsIconic`과 native minimized state가 일치한 뒤에만 minimize/restore phase를 진행하도록 고쳤고, 실패 보고서에도 실제 lifecycle 배열을 남긴다.

### 현재 자동 evidence

| 검증 | 결과 | 핵심 수치 |
|---|---|---|
| Release native + managed product build | `PASS` | warning/error 0 |
| AMD capability/lifecycle aggregate | `V2-PASS-qualification` | capability/real WSI/product, 5종 result injection, reset/minimize/resize/start-close 반복 PASS |
| 8 edges/corners × 3 reverse, 600 px/600 ms | `PASS-automated-partial` | 24/24, motion accepted/presented 1,140/1,140, superseded/timeout/marker regression 0, 최소 77.75 Hz, target→present p95 최대 14.54 ms/max 16.65 ms, process당 surface create 1 |

새 수치는 DWM admission pacing 때문에 이전 119.15 Hz 이상의 unconstrained 제출률보다 낮지만, 임의 30/60 fps 제한이 아니며 같은-run ANGLE 허용치와 strict latency gate를 모두 통과했다. 자동 WGC/terminal 결과만으로 검정/흰 영역이나 좌·상단 체감 떨림 제거를 확정하지 않는다. 동일 명령의 사람 border drag, 물리 scan-out, 전체 GPU/DPI/refresh/monitor matrix는 post-fix `notVerified`이며 사용자의 재확인이 필요하다.

## 15. 2026-09-03 두 번째 실물 보고와 검증 축소

### 판정과 변경

- Proposed-size admission 적용 뒤에도 사용자가 “약간 나아졌지만 흰색/검정 영역은 남는다”고 재확인했다. 따라서 14절 구현의 실물 결과는 `FAIL-observed`이며, 당시 24-case WGC PASS를 시각 acceptance로 해석하지 않는다.
- 원인은 준비된 frame의 `DwmFlush`가 matching `WM_SIZE` 안에서 실행되어 top-level geometry가 이미 노출된 다음이었다. 성공한 prepare의 compositor 동기화를 `WM_SIZING` 반환 전으로 옮겨, USER32 geometry가 frame보다 앞서 보이는 순서를 제거했다.
- Parent 영역을 view의 light/dark background RGB로 채우는 fallback은 실제 top/child 표시 간격을 제거하지 않고 증상을 가리므로 채택하지 않는다. 관련 native ABI 필드와 `WM_ERASEBKGND`/`WM_PAINT` painting은 제거했다.
- 이는 frame rate를 30/60 Hz로 제한하는 변경이 아니다. 현재 display/DWM 경계를 proposed geometry 앞에 두는 ordering 변경이다.

### 축소한 검증 정책과 결과

- 기본 validator는 `Left`와 `TopLeft`를 각 1회만 실행한다. `-Probe`는 가장 민감한 `TopLeft` 1회만 실행하고 ANGLE before/after 비교도 생략한다.
- 이전 8 edges/corners × 3회 검증은 `-FullEightEdgeMatrix`, slow/medium/fast 전체 조합은 `-FullCurrentDpiMatrix`를 명시할 때만 실행한다.
- Pre-geometry ordering과 parent masking이 함께 있던 revision의 `.doroti/evidence/windows-vulkan-pregeometry-flush-focused-final/manifest.json`은 `PASS-automated-partial`이었지만, masking 제거 뒤의 현재 source와 같지 않으므로 현재 시각 evidence로 재사용하지 않는다. Masking 제거 후에는 요청대로 반복 capture 없이 native Release와 managed product build만 다시 통과시켰고, 현재 live resize는 `notRun`이다.
- Focused probe는 crash, Vulkan/terminal/resource, final exact geometry 회귀만 좁게 확인한다. WGC가 포착하지 못하는 DWM shell transient와 실제 사람 체감은 그대로 `notVerified`이며, 다음 판단은 DemoApp의 좌측 및 좌상단 직접 drag 한 번을 권위 결과로 사용한다.

## 16. 2026-09-03 Flutter actual-WM_SIZE blocking 전환

### Flutter 대조와 설계

- 현재 Flutter Windows embedder는 실제 surface resize가 필요한 `OnWindowSizeChanged`에서 target size를 기록하고 window metrics를 보낸 뒤, platform task runner를 polling하면서 raster가 해당 크기의 frame을 present할 때까지 platform thread를 최대 100 ms 막는다. Raster thread는 이 시간에도 계속 실행하며 exact target이 아닌 frame은 resize 완료로 승인하지 않는다. Present 뒤에는 platform task를 깨우고 `DwmFlush`로 이전 크기 surface의 stretch를 완화한다.
- Doroti의 실패한 `WM_SIZING` proposed-layout/16 ms prepare와 prepared-generation 상태를 모두 제거했다. Retained Vulkan의 실제 top-client `WM_SIZE`만 logical extent의 authority가 되고, 그 generation을 queue한 뒤 raster worker의 exact present terminal까지 기존 100 ms bound로 기다린다. HWND thread는 이 동안 Vulkan API나 scene rendering을 직접 수행하지 않는다.
- Flutter와 달리 AMD에서 매 step 약 19~25 ms가 들었던 swapchain 재생성을 되살리지 않도록 grow-only retained child/surface는 유지한다. Exact present 성공 시 Doroti는 해당 `WM_SIZE` transaction을 한 번의 `DwmFlush`까지 유지한다. 따라서 fixed native-window FPS limiter가 아니라 render completion이 USER32 resize 진행에 backpressure를 건다.
- Parent의 light/dark 색을 칠하는 fallback이나 다른 masking은 추가하지 않았다. 드러나는 흰색/검정 영역이 있다면 자동 결과와 별개로 실제 결함으로 계속 보이게 둔다.

### 축소 검증 범위

- 현재 변경에는 native Release build와 managed product build를 먼저 실행하고, 가장 민감한 `TopLeft` focused probe를 한 번만 실행한다. 8방향 반복 matrix와 ANGLE 전후 비교는 실행하지 않는다.
- 자동 probe는 exact generation 승인, timeout, marker/resource 회귀만 판정한다. 실제 border drag의 흰색/검정 노출과 좌·상단 체감 떨림은 사용자의 직접 확인 전까지 `notVerified`다.

### 현재 결과

- Native Release와 managed product build는 경고/오류 0으로 `PASS`했다. Native DLL SHA-256은 `bf4ee55eea25d328906014e97997a8e47a1cc451d98a3e289ca972126d268516`이다.
- 사용자 입력이 겹쳐 pointer resize 자체가 성립하지 않은 첫 시도는 구현 판정에서 제외했다. 간섭 없이 다시 실행한 `.doroti/evidence/windows-vulkan-flutter-wmsize-focused-clean/manifest.json`의 유일한 `TopLeft` case는 `PASS-automated-partial`이다.
- Clean focused run은 captured 79, outer-rect change 33, motion target accepted 31, presented terminal 32, superseded 0, platform wait timeout 0, marker regression 0이었다. Presentation rate는 54.09 Hz, target→present p95/max는 22.71/26.47 ms였고 Vulkan acquire/present는 44/44, outstanding 0, surface recreate 1/retained reuse 37이었다.
- Probe는 `-SkipBuild`였으므로 manifest 자체의 source→binary 표기는 `notVerified-skip-build`다. 다만 그 직전에 동일 source로 별도 native/managed Release build를 성공시켰고, clean run 도중 source/repository/binary endpoint hash는 모두 안정적이었다. 이것을 물리 시각 증거로 승격하지 않는다.

## 17. 2026-09-03 Vulkan offscreen + Composition presentation 전환

### 최종 원인과 선택

- Retained visible child/WSI는 ordinary resize의 swapchain recreation을 제거했지만, USER32 top-level geometry와 별도 visible Vulkan child buffer라는 두 표시 소유자를 유지했다. `WM_SIZE` blocking과 `DwmFlush`는 이 둘을 하나의 atomic commit으로 만들 수 없었고, 사용자 실물 Left/TopLeft drag에서 흰색/검정 transient와 raster 떨림이 계속 관찰됐다.
- Vulkan은 app raster만 offscreen으로 수행하고, 최종 표시를 Windows Presentation API와 하나의 `ContentIsland`에 맡기는 구조를 선택했다. 공개 API상 top-level USER32 geometry와 Presentation buffer 자체가 완전히 같은 transaction은 아니므로 post-fix 물리 결과는 여전히 별도 acceptance다.
- Parent-color masking, fixed 30/60 FPS limiter, CPU readback/upload, full-frame stretch는 도입하지 않았다. ANGLE 기본 경로와 experimental Acrylic 경로도 그대로 유지한다.

### shared runtime 구현

- Native ABI feature `COMPOSITION_PRESENTATION = 1 << 3`과 Vulkan Composition create/destroy/replace/availability/present/crop/retire exports를 추가했다. ABI GPU pointer count는 0을 유지한다.
- Composition 요청 시 top-level HWND가 input과 exact client metrics의 authority가 되고 bootstrap child는 숨긴다. `WM_SIZE`는 `ContentIsland` viewport와 latest raster generation을 비동기로 갱신하며 visible child WSI를 생성하지 않는다.
- Vulkan physical device의 정확한 LUID로 D3D11 device를 만들고 `B8G8R8A8Unorm` shared NT texture 3개를 Presentation manager에 등록한다. Vulkan은 각 texture를 `D3D11TextureBit`, dedicated external memory, `TransferDstBit` 용도로 import한다.
- Skia/Vulkan retained backing에서 선택된 Presentation texture로 copy한 뒤 Vulkan fence를 CPU에서 확인하고 cropped Present를 commit한다. 각 texture의 availability event가 재사용과 retirement의 authority이며 active/retired WSI swapchain은 항상 0이다.
- teardown에서는 null surface binding 대신 native-only 1×1 successor texture를 먼저 present한다. 그 뒤 imported 3-slot availability를 기다려 Vulkan image/memory를 파괴하므로 현재 표시 중인 마지막 buffer도 수명주기가 닫힌다.
- 최초 viewport는 retained capacity 중앙에 배치한다. 성공한 present/crop의 실제 `sourceX/sourceY/width/height`를 저장하고, Left/TopLeft에서는 그 사각형의 right/bottom 고정 edge를 기준으로 다음 crop과 exact frame 위치를 계산한다. 반복 crop과 방향 전환도 같은 표시 사각형에서 이어진다.
- Platform crop, raster present, retirement 전환은 같은 viewport gate를 사용한다. retirement 상태 게시 뒤에는 새 crop/present를 허용하지 않으며, negative Present/Crop HRESULT는 manager를 poison한다. `WaitForMultipleObjects`의 `WAIT_FAILED`도 명시적으로 실패시킨다.
- 느린 Vulkan 초기화 전에 실행되던 input smoke를 첫 exact present 뒤로 옮겼다. Composition focus authority는 top-level `WM_ACTIVATE`이며, product validator는 lifecycle restore 뒤의 최종 상태와 무관하게 실제 synthetic gain→loss 순서를 검사한다.

### 자동 검증

| 검증 | 결과 | 핵심 증거 |
|---|---|---|
| Native Release + managed/product Release | `PASS` | warning/error 0 |
| Windows App SDK native ABI | `PASS` | AMD64, feature bit 8, Composition exports 7개, buffer count 3 |
| AMD aggregate capability/lifecycle | `Vulkan-Composition-PASS-automated-partial` | exact-LUID external memory, validation warning/error 0, product, device-loss 2종, reset 10, minimize/restore 10, resize 10, start/close 10 PASS; source→binary `PASS-built-after-source-fingerprint` |
| Focused TopLeft live probe | `PASS-automated-partial` | 600 px/600 ms reverse, outer changes 74, motion accepted 73, motion presented 72, superseded/timeout/marker regression 0, 121.69 Hz, target→present p95/max 9.93/10.45 ms, final exact PASS |

- Aggregate evidence: `.doroti/evidence/windows-vulkan-composition-capability-final4/manifest.json`
- Focused live evidence: `.doroti/evidence/windows-vulkan-composition-live-probe-final/manifest.json`
- Live probe의 Vulkan snapshot은 present/copy-fence wait 86/86, buffer 3, active WSI 0, backing allocation/reuse 1/85, outstanding 0이었다.

### 판정과 남은 경계

- 구현과 AMD 자동 검증은 `PASS-automated-partial`이다. 기존 direct-WSI two-visible-owner 원인은 현재 제품 경로에서 제거됐다.
- WGC와 Presentation receipt는 물리 scan-out, DWM shell transient, 사람의 체감 smoothness를 증명하지 않는다. 사용자의 post-fix Left/TopLeft 실물 확인 전에는 `problem.md`를 물리 PASS로 닫거나 이 계획을 archive하지 않는다.
- NVIDIA/Intel, refresh/DPI/mixed-monitor/Snap/maximize/occlusion matrix와 물리 IME/accessibility는 `notVerified`다.
- `PRESENTATION_ERROR_LOST`는 현재 context poison/fail-fast이며 Presentation manager/import graph 자동 재생성은 후속 P1이다. Native C export 예외 경계와 반환 snapshot hardening은 후속 P2다.

## 18. 2026-09-03 retained-child pre-geometry 최종 acceptance

### host geometry 제한 실험의 결론

- `WM_SIZING` proposed RECT를 마지막 승인 geometry로 되돌리고 posted message 또는 timer가 준비된 크기를 `SetWindowPos`하는 host limiter를 시험했다. USER32가 보관한 이전 proposal이 나중에 적용되며 cursor와 창 geometry가 어긋났고, intermittent gap이 오히려 커졌다.
- 이 limiter와 cursor-anchor/pending-rectangle 상태는 최종 코드에서 제거했다. USER32가 top-level geometry를 계속 단독 소유하고 Doroti는 proposed RECT를 수정하지 않는다. 고정 30/60 FPS window limiter도 사용하지 않는다.

### 현재 shared runtime

- Vulkan은 retained offscreen backing만 rasterize하고, same-LUID D3D11 shared texture 3개를 Windows Presentation API에 공급한다. Native DirectComposition target은 숨겨진 top-level 대상이 아니라 monitor work-area 이상으로 유지되는 visible child HWND에 붙는다.
- Top HWND는 `WS_CLIPCHILDREN`, Composition 요청 시 `WS_EX_NOREDIRECTIONBITMAP`을 사용한다. Retained child의 full-capacity Presentation surface에는 마지막 app-owned frame guard가 유지되어 parent clip이 old/new geometry 어느 쪽을 먼저 보더라도 미소유 tail을 노출하지 않는다.
- `WM_SIZING`은 proposed outer RECT에서 client size만 계산하고 matching generation을 발행한다. Raster worker가 Vulkan copy/Present를 끝낸 뒤, native Presentation API가 같은 present id와 content tag의 `CompositionFrame` 통계를 관찰할 때까지 기다리고서 handler가 반환한다.
- 이어지는 `WM_SIZE`가 prepared extent와 일치하면 중복 render 없이 승인한다. 준비가 timeout/실패했거나 실제 client size가 다르면 actual geometry를 새 generation으로 republish하고 기존 exact terminal 경로로 복구한다. Retained child capacity는 monitor 이동과 growth 때만 확대한다.

### 반복 자동 evidence

- `.doroti/evidence/windows-vulkan-fixed-child-pregeometry-probe1`부터 `probe12`까지 focused Left/TopLeft reverse 600 ms matrix를 12회 실행했다. `probe1`은 현재 source에서 build했고 `probe2`~`probe12`는 같은 binary를 반복 관찰했다.
- matrix 11/12, case 22/24가 통과했다. 유일한 strict 실패인 `probe7`에서 Left는 한 capture frame에 최대 8 px, TopLeft는 한 capture frame에 최대 60 px의 validation-background right gap을 검출했다.
- 같은 두 실패 case도 transport/resource/final-exact 계약은 통과했다. CompositionFrame wait/observed는 Left 30/30, TopLeft 28/28이고 timeout은 모두 0이었다. 다른 11개 matrix에서는 두 case 모두 통과했다.
- 따라서 자동 판정은 의도적으로 `PARTIAL`로 유지한다. 단일 반복 실패를 삭제하거나 12/12 PASS로 재분류하지 않는다.

### 사용자 acceptance

- 사용자가 위 headed 반복 test가 실제 화면에 표시되는 것을 직접 확인하고 현재 결과면 통과로 보아도 된다고 승인했다. 이 문제의 사람 체감 판정은 2026-09-03 `PASS-observed`다.
- 이 승인은 AMD Radeon 780M 선택, 96 DPI, 165 Hz의 현재 화면과 Left/TopLeft test에 한정한다. NVIDIA/Intel, 다른 DPI/refresh, mixed-monitor/Snap/maximize/occlusion과 물리 IME/accessibility는 `notVerified`다.
- 검은 띠 이슈는 현재 scope에서 해결로 닫는다. strict oracle의 1회성 두 frame은 알려진 잔여 evidence로 유지하고, 다른 환경에서 사람이 다시 증상을 관찰할 때 별도 matrix 결함으로 재개한다. Presentation-manager loss recovery와 C ABI hardening은 이 acceptance와 별개의 후속 작업이다.

### 최종 source-built 회귀 확인

- `.doroti/evidence/windows-vulkan-fixed-child-pregeometry-final2/manifest.json`은 `PASS-automated-partial`이다. Source→binary correspondence, source/repository/binary endpoint 안정성과 retained-child visible-owner gate가 모두 PASS했다.
- Left는 gap 0, outer change 26, accepted/presented 25/26, 45.22 Hz, target→present p95 25.24 ms, CompositionFrame wait/observed 30/30, timeout 0이었다.
- TopLeft는 gap 0, outer change 26, accepted/presented 25/26, 45.88 Hz, target→present p95 26.53 ms, CompositionFrame wait/observed 31/31, timeout 0이었다. 두 case 모두 final exact와 resource gate를 통과했다.
- `.doroti/evidence/windows-vulkan-fixed-child-pregeometry-capability-final2/manifest.json`은 aggregate `PASS`, gate `Vulkan-Composition-PASS-automated-partial`이다. Product, exact resize 10, device reset 10, minimize/restore 10, start/close 10/10이 통과했고 Composition buffer 3, active WSI 0, source/repository/binary endpoint 안정성을 확인했다.

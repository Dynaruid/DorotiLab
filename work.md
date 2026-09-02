# Doroti Windows App SDK Silk.NET Vulkan 작업계획

- 작성일: 2026-09-02
- 상태: **retained-surface implementation partial / automated lifecycle PASS / 24-case strict live-resize PASS-automated-partial / unextended present-retirement notVerified / physical acceptance notVerified**
- 대상: `Doroti.Host.WindowsAppSdk`의 opaque `HwndExactCpp` 경로에 Silk.NET 기반 direct Vulkan presenter 추가
- 현재 기본값: managed ANGLE/EGL-D3D11
- 새 선택값: `DOROTI_WINDOWS_PRESENTER=Vulkan`
- 핵심 결정: Vulkan은 명시적 opt-in 경로로만 추가한다. ANGLE 기본값, C++ HWND/input/lifecycle 소유권, `experimentalAcrylic` Composition 경로는 바꾸지 않는다.

## 0. 검토 결론

추가는 가능하지만 2026-08-26에 제거한 구현을 그대로 복원하지 않는다. 당시 direct Vulkan은 build, controlled product probe, self-contained publish까지 통과했지만 실제 방향 전환 border resize에서 `vkQueueSubmit ... ErrorDeviceLost`가 반복돼 제품 경로에서 제거됐다. 자동 PASS를 현재 Vulkan 품질로 재사용하지 않는다.

현재 코드와 제거된 presenter를 대조한 결과, 새 경로가 먼저 해결해야 할 핵심은 바인딩 선택이 아니라 WSI 상태와 수명주기다. 2026-09-02 AMD live-resize 후속 계측에서는 Skia paint나 queue retirement가 아니라 매 exact extent의 `vkCreateSwapchainKHR`가 약 19~25 ms를 차지하는 주 병목으로 확인됐다. 2026-09-03에는 top의 exact logical viewport와 grow-only child/swapchain capacity를 분리해 ordinary resize에서 이 재생성 비용과 compositor stretch 구간을 제거했다.

| 검토 항목 | 결론 |
|---|---|
| Silk.NET 패키지 | `Silk.NET.Vulkan`과 `Silk.NET.Vulkan.Extensions.KHR` 2.23.0을 같은 버전으로 중앙 고정한다. 2.23.0은 현재 확인한 stable이며 Vulkan 1.4.336 registry 기반 binding을 제공한다. |
| native runtime | Silk.NET은 binding이다. 제품에 임의의 `vulkan-1.dll`이나 ICD를 복사하지 않고 `%SystemRoot%\System32\vulkan-1.dll`을 명시적으로 연다. loader/driver/필수 extension이 없으면 Vulkan 요청을 fail-fast한다. |
| 과거 acquired-image 결함 | 제거된 구현은 acquire와 copy 뒤 `shouldPresent`가 false가 되면 `vkQueuePresentKHR`도 release도 하지 않고 반환할 수 있었다. 이 경로가 과거 device loss의 단독 원인이었다고 단정하지 않지만, 새 구현에서는 허용하지 않는다. |
| swapchain retirement | acquire 전까지 stale 여부를 확정하고 acquire 성공을 presentation commit으로 삼는다. 현재 recreate는 raster worker의 `vkQueueWaitIdle` 뒤 old swapchain을 파괴한다. 이 방식은 실기기/validation에서 동작하지만, Khronos가 설명하듯 unextended Vulkan의 WaitIdle은 presentation-engine resource retirement를 명세상 보장하지 않는다. AMD가 `VK_EXT_swapchain_maintenance1`/present fence를 제공하지 않아 이 항목은 `notVerified`다. |
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
- WndProc/platform thread는 Vulkan API나 `DwmFlush`를 직접 호출하지 않는다. 다만 opaque child `WM_SIZE`는 Flutter와 같은 bounded exact handshake로 raster terminal을 최대 100 ms 기다리므로, 그 terminal 앞의 swapchain recreate/present 비용을 간접적으로 기다릴 수 있다.
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

- `WM_SIZE`는 metrics/latest render를 publish하고 opaque child 경로에서는 그 generation의 terminal을 최대 100 ms 기다린다. Vulkan API와 swapchain 작업 자체는 raster worker가 수행한다.
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
- [Win32 child-window clipping](https://learn.microsoft.com/en-us/windows/win32/winmsg/window-features)
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

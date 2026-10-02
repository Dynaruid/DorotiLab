# 네이티브 공통 C 경로 작업계획

작성일: **2026-10-02** · 대상: iOS / Mac Catalyst / macOS / Android / Windows / Linux

상태: **구현 및 실행 가능한 자동 검증 완료, 전체 인수 PARTIAL.** Apple(iOS/Mac Catalyst/macOS) 검증은 사용자 요청으로 SKIPPED다. 공통 C 연결과 CPU/GPU·Windows·Android·Qt 자동 검증은 수행했다. 실제 표시 성능·물리 입력·10분 사용, Windows MAUI native 입력 adapter와 OpenGL Wayland compatibility는 미완료다. [실행 결과와 플랫폼 JSON](works/results/2026-10-02-native-frame-pipeline.md)을 기준으로 체크하며, 과거 iOS 수치를 새 payload에 소급하지 않는다.

Linux Qt 구성 후속 검토: **SDK·샘플·템플릿 Quick/C 기본값, 옵션별 빌드 격리, 누락된 네이티브 산출물 거절 및 package-only Release 검증 PASS.** [후속 결과](works/results/2026-10-02-linux-qt-configuration-review.md)와 §14를 참조한다. 물리 GPU·표시·입력 인수와 OpenGL Wayland 제한은 유지한다.

후속 작업: [work6 — A/B 제거 및 C 경로 단일화](work6.md)의 **구현·실행 가능한 자동 검증 완료**, 사용자 추가 요청의 동적 texture 예산도 반영했다. [후속 결과](works/results/2026-10-02-native-frame-c-only.md)를 참조한다. 아래 계획의 A/B·legacy 옵션 유지와 같은 바이너리 A/C 비교 요구는 C 단일 정책·변경 전/후 C 회귀 기준으로 대체했다. native/replay/resize의 C 내부 직렬 처리와 기존 실행 증거·미완료 인수는 유지한다.

2026-10-03 Apple 후속 검토: 현재 요청에 따라 이전에 생략한 Apple 검증을 별도로 수행하고 Catalyst 컴파일 조건, UIKit/AppKit 재시도 및 AppKit deferred resize를 보강했다. [새 후보의 검토·실행 결과](works/results/2026-10-03-apple-frame-configuration-review.md)를 참조한다. 위의 Apple SKIPPED는 10월 2일 후보의 이력이며 새 후보에 대한 판정은 후속 결과로 구분한다.

2026-10-03 AppKit 별도 요청: window별 Metal retirement·재연결, background 진단, 분리 시 키/focus, backing factor와 숨김/복귀를 추가 보강했다. [AppKit 별도 결과](works/results/2026-10-03-appkit-configuration-review.md)는 Graphite/Ganesh native 자동 검사와 합성 fixture의 한계를 구분한다.

**목표:** 모든 네이티브 제품 호스트에 C의 프레임 생성·제출 원칙을 공통 기본 구조로 적용한다. 앞 프레임의 GPU 작업이 다음 framework 장면 준비를 막지 않게 하고, 새 shader-only 장면은 최대 2개 GPU 프레임과 플랫폼의 비동기 표시 경로를 사용한다. native view 합성·resize·rotation·replay는 각 플랫폼에 필요한 직렬 처리와 표시 동기화를 유지한다.

작업 순서: **플랫폼별 경로 확인 → 공통 정책 추출 → Apple 연결 → Android 연결 → Windows 연결 → Linux 연결 → 통합 검증·기본 적용**.

## 1. 기준과 범위

사용자는 아이폰에서 수정본의 정상 동작을 확인하고 iOS C 경로를 기본으로 채택했다. 이어서 같은 구조를 모든 네이티브 플랫폼으로 확장하는 방향을 요청했다.

기준 자료:

- [work4 — iOS 구현·검증 이력](work4.md)
- [iOS 프레임 연결 결과](works/results/2026-10-02-ios-frame-loop.md)
- [iOS C 기본 적용 identity·복귀 결과](works/results/2026-10-02-ios-default-C-summary.json)
- [Fast adaptive 기본값·픽셀·성능 결과](works/results/2026-10-02-fast-adaptive-default-summary.json)

유지할 범위:

- 공개 `ImageFilter.variableBlur`, `ImageFilterConfig.CreateVariableBlur`, Sample2의 기본값은 **fastGaussian + adaptiveResolution=true + resolutionScale=0.25**다.
- 기존 Gaussian/Adaptive/Fast/Kawase 커널, sigma 의미, DPR, 캡처 범위와 화질을 유지한다. 프레임 연결 변경과 셰이더 최적화의 성능 자료를 섞지 않는다.
- Sample2와 Testbed를 검증 대상으로 사용하고 제품 호스트·샘플·새 프로젝트 템플릿까지 정책을 일치시킨다.
- 각 플랫폼의 기존 제품 backend와 GPU device/queue 선택을 유지한다. Metal, Vulkan, Vulkan→D3D12 출력의 표시 구현을 각 어댑터에 둔다.
- Web은 이번 범위에서 제외한다. 기존 Ganesh/OpenGL·소프트웨어 대체 경로는 호환성 검사를 수행하고 지원 능력을 명시한다. 두 GPU 프레임의 수명을 보장하지 못하는 경로를 C 통과로 보고하지 않는다.
- iOS의 기존 기본 채택은 유지한다. 과거 반복 성능과 새 공통화 payload의 결과는 각각 보존한다.

## 2. 현재 코드와 플랫폼별 차이

아래는 계획 작성 시 소스에서 확인한 사실이다. backend의 프레임 슬롯 개수만으로 C의 framework 준비·표시·수명 계약을 충족했다고 판정하지 않는다.

| 대상 | 현재 확인한 구조 | 필요한 작업 |
| --- | --- | --- |
| iOS / UIKit / Graphite-Metal | framework 준비가 GPU admission 앞에 있고 shader-only 최대 2, C 기본값. native·회전·replay 별도 처리 | 현재 동작을 기준으로 공통 정책을 추출하고 iOS 회귀 확인 |
| Mac Catalyst / UIKit / Graphite-Metal | UIKit handler를 공유하지만 준비·admission 연결의 핵심 부분은 `IOS && !MACCATALYST` 조건으로 제한됨 | Catalyst의 frame pulse·활성화·resize에 공통 준비와 admission 연결 |
| macOS / AppKit / Metal | native pending은 1, 그 외 `_inFlight` 상한은 3. GPU 상한과 drawable 검사 뒤에 paint 진입 | 준비를 앞 단계로 분리하고 fresh shader-only 최대 2, replay/native 직렬 및 실제 합성에 맞춘 표시 선택 |
| Android / SurfaceView / Graphite-Vulkan | Vulkan window에 최대 2개 슬롯이 있으나 슬롯·swapchain 획득 뒤 paint에서 framework callback 실행 | Choreographer pulse에서 먼저 준비하고 슬롯이 가득 차도 최신 장면 유지. native 합성 경계 연결 |
| Windows / Windows App SDK | managed `BeginFrame`이 presenter 작업 앞에 있으나 native 요청·출력 제한과 Vulkan/D3D12 자원 수명은 별도 구현 | native scheduler까지 준비 기회가 보장되는지 확인하고 producer·copy·composition 수명과 공통 admission 연결 |
| Windows / MAUI | 별도 DXGI surface·resize·native composition 경로 사용 | 준비·raster 분리 연결을 별도로 적용하고 resize/ACK 계약 보존 |
| Linux / Qt Quick / Graphite-Vulkan | runner의 `BeginFrame`은 render 앞에 있음. 기본 Quick 경로는 이전 Qt sampling retirement의 queue wait와 copy fence wait를 사용 | Qt 소비 완료를 포함한 frame별 수명·비동기 publication을 구현하고 정상 프레임의 CPU 대기 분리 |
| Linux / Qt Vulkan window 대체 경로 | `GraphiteVulkanWindow`를 통한 별도 제출 경로 | Quick과 구분해 공통 admission·retirement·resize 계약 연결 |

주요 근거:

- [현재 공통 정책](Doroti/src/Doroti.Skia.Rendering/NativeFramePipeline.cs) (기존 iOS 정책·옵션 래퍼는 work6에서 제거), [MAUI 준비 연결](Doroti/src/Doroti.Host.Maui/MauiFrameworkHost.cs)
- [AppKit draw](Doroti/src/Doroti.Host.Maui/DorotiMacOSMetalView.cs), [Android draw](Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs)
- [Vulkan window 제출](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.cs), [Vulkan frame 슬롯](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanWindow.Frames.cs)
- [Windows runner](Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs), [Windows Vulkan presenter](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs), [MAUI DXGI surface](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs)
- [Qt runner](Doroti/src/Doroti.Host.Qt/DorotiQtRunner.cs), [Qt surface](Doroti/src/Doroti.Host.Qt/QtSkiaSurface.cs), [Qt Quick GPU 수명](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanQuick.cs)

## 3. 공통 설계 계약

### 3.1 준비와 GPU admission 분리

각 호스트의 유효한 frame pulse에서 다음 순서를 사용한다.

1. 해당 호스트의 소유 스레드에서 입력과 lifecycle/viewport 상태를 반영한다.
2. immutable pixel size·DPR·view/resize/context/surface generation과 frame timestamp를 게시한다.
3. framework callback을 최대 한 번 실행하여 최신 장면을 준비한다.
4. 준비 결과의 실제 장면·native 상태와 현재 GPU 작업 수로 admission을 판단한다.
5. 사용할 슬롯과 출력 타깃을 확보한 뒤 새 장면을 기록·제출한다.
6. 플랫폼별 completion을 처리하고 대기 중인 장면 또는 다음 pulse를 깨운다.

GPU 슬롯이나 drawable/swapchain image가 부족해도 일반 framework 준비 기회를 유지한다. 실행 중 만들어진 재요청은 다음 pulse로 남긴다. GPU 완료로 인한 raster 재시도는 이미 준비한 장면을 사용하며 같은 pulse의 callback을 중복 실행하지 않는다.

소유 스레드는 각 호스트의 기존 계약을 따른다. Windows raster owner, Qt GUI/sync/render thread, UIKit/AppKit의 스레드 역할을 일괄 UI 스레드로 바꾸지 않는다. resize 전환의 준비·적용 순서는 기존 exact epoch 계약과 함께 별도로 처리한다.

### 3.2 큐와 admission

- framework 요청은 합쳐 처리하고 renderer의 최신 pending scene은 **하나**만 유지한다.
- viewport/context/generation이 정확히 일치하는 **새 shader-only 장면**에 최대 **2개 GPU 프레임**을 허용한다. Off와 일반 UI의 새 장면도 대상이다.
- replay, native/shield가 포함된 장면, pending native 작업, resize/rotation은 직렬 admission을 사용한다.
- shader 작업이 2개 남은 상태에서 native 장면이 준비되면 장면을 보존한다. 기존 작업을 비동기로 drain한 뒤 native 작업을 제출한다.
- C→직렬 전환은 새 슬롯 사용을 막는 정책 전환이다. 진행 중인 프레임의 자원을 즉시 버리지 않는다.
- 장면 조회와 실제 raster 사이에 native/viewport 상태가 달라지면 renderer의 원자적 검사를 통해 다시 거절한다.
- cap은 **논리 GPU 프레임**에 대한 상한이다. swapchain buffer 수, 표시 중인 front texture, Vulkan→D3D12 공유 source 슬롯 수와 구분한다. 필요한 표시·소비 자원을 무조건 두 개로 줄이지 않는다.

### 3.3 표시와 자원 수명

공통 정책은 비동기 표시를 요청할 수 있는지 결정하고, 실제 API 호출은 플랫폼 어댑터가 수행한다. iOS의 `Transaction` 의미는 공통 계약에서 `표시 동기화 필요`로 표현하고 각 플랫폼의 합성 방식에 매핑한다.

장면·recording·surface·snapshot·drawable·native lease와 공유 texture는 해당 자원을 사용하는 **마지막 GPU 소비자**가 완료될 때까지 유지한다. Vulkan producer 완료만으로 D3D12 copy 또는 Qt sampling 중인 자원을 재사용하지 않는다.

GPU 완료, OS의 표시 통보, native composition 적용 완료는 별도 상태다. 항상 같은 순서로 도착한다고 가정하지 않는다. 제출 성공·scheduling 완료·present 호출 성공을 GPU 완료로 처리하지 않으며, 표시되지 않은 프레임의 callback을 무기한 기다리는 semaphore도 만들지 않는다.

terminal은 한 번만 처리한다. 역순 completion에서도 오래된 장면이 최신 replay source를 덮지 않게 한다. 제출 실패·device loss·종료에서는 실제 완료 또는 확인된 context loss에 따라 회수하고, 완료를 확인하지 못한 자원을 timeout만으로 재사용하지 않는다.

### 3.4 공통 코드 경계와 비교 옵션

- UIKit에 의존하지 않는 admission·기본 옵션·frame lifetime 규칙을 공유한다. 현재 `IosFrameAdmissionPolicy`와 `IosFrameLoopOptions`를 출발점으로 사용한다.
- renderer 장면 적격성·슬롯 상한은 `Doroti.Skia.Rendering`의 공통 계약에, callback/pulse와 host 연결은 hosting 경계에 둔다. 추출 위치는 실제 프로젝트 의존성을 확인해 순환 참조 없이 정한다.
- display link, Choreographer, Windows scheduler, Qt render loop와 GPU API 객체는 플랫폼 코드에 둔다.
- 공통 C 기본값과 명시 serial 복귀 옵션을 제공한다. 기존 iOS 환경변수·collector 동작은 호환성을 유지한다. Android 실행 인자 등 플랫폼의 실제 설정 전달 방식도 연결한다.
- A는 해당 플랫폼의 기존 직렬 기준 경로, C는 준비 분리+최대 2+비동기 표시 경로다. B는 표시 대기만 분리할 수 있는 backend에서 진단용으로 사용한다. 다른 플랫폼의 A/B를 iOS transaction과 동일한 API라고 가정하지 않는다.

## 4. P0 — 실행 경로와 기준 확보

- [x] 제품 target과 실제 기본 backend를 플랫폼별로 확정한다. Windows App SDK/MAUI, macOS/Mac Catalyst, Linux Quick/Vulkan window를 각각 구분한다.
- [x] framework callback 앞뒤, native scheduler, 슬롯 검사, drawable/acquire, GPU submit, copy/consumer 완료와 표시 API의 순서를 기록한다.
- [x] 정상 프레임의 CPU 대기를 찾아 목적을 분류한다. 타깃 확보·producer 완료·consumer retirement·resize·종료 대기를 구분하고, 각 대기를 제거할 대체 신호를 먼저 정한다.
- [ ] 각 플랫폼의 현재 사용 runtime·SDK/TFM·OS·GPU·backend·실제 Hz·DPR·pixel extent와 source/payload hash를 기록한다. 사용자 지시에 따라 기기 상태는 양호하게 가정한다.
- [ ] 기본 Fast adaptive와 Off, 일반 애니메이션·스크롤, native 입력·WebView·resize의 기준 동작을 확보한다.
- [ ] frame별 진단을 opt-in으로 연결한다. scene/frame/input ID와 generation, 거절 이유, pending 최대값, 완료·표시 시점을 보존한다. hot path에서 전체 이력 복사나 console/JSON 출력을 하지 않는다.

**완료 조건:** 플랫폼별 준비를 막는 지점과 정상 프레임의 대기 원인, 필요한 생산자·소비자 완료 신호가 표로 설명된다.

## 5. P1 — 공통 정책 추출과 iOS 기준 보존

- [x] 공통 admission 결과에 fresh-only 여부, GPU 상한, 표시 동기화 필요, 거절 이유를 표현한다.
- [x] 기본 C와 serial/async 비교 옵션의 우선순위를 공통 규칙으로 정하고 플랫폼별 설정 전달을 연결한다.
- [x] callback 준비와 renderer 조회·슬롯 검사 순서를 분리한다. immutable 준비 descriptor를 모든 어댑터가 전달할 수 있게 한다.
- [x] terminal·generation·역순 완료·폐기 규칙 중 공유할 부분을 추출한다. 플랫폼 native 객체의 수명은 어댑터가 소유한다.
- [x] 기존 `ShaderFramePipelineRegression`을 실제 공통 정책으로 연결한다. GPU full에서도 callback 실행, callback에서 native 삽입, raster-only wake, 두 슬롯 상한, resize/context 변경, 실패·역순 완료를 검증한다.
- [ ] iOS C 기본값·명시 A/B·Fast API 기본값을 유지하고 공통 CPU/Metal 검사와 기존 lifecycle 도구를 통과한다.

**완료 조건:** iOS가 추출한 공통 정책을 실제 사용하며 기존 동작을 유지한다. 다른 플랫폼에서도 같은 규칙을 사용할 수 있고 UIKit 종속성이 없다.

## 6. P2 — macOS와 Mac Catalyst 연결

- [x] AppKit surface에 준비/query/pending 연결을 제공하고 `_inFlight`·drawable 검사 앞에서 정상 frame 준비를 수행한다.
- [x] macOS의 shader-only 상한 3을 공통 상한 2로 맞춘다. native 작업과 replay·layout 전환은 직렬로 유지한다.
- [x] configured coordinator의 존재와 실제 native 합성을 구분하여 shader-only 비동기 표시를 선택한다.
- [x] Mac Catalyst의 준비/admission을 iOS 전용 조건 밖의 공통 연결로 옮긴다. iOS 회전 처리와 Catalyst resize 처리는 각 구현을 유지한다.
- [ ] active/inactive, 창 detach/reattach, minimize/restore, 화면 이동·DPR 변경, live resize의 시작·정지·재시작을 확인한다.
- [ ] native Editor/WebView 생성·삭제·재생성, selection/IME, resize와 새 shader 장면의 경계를 검증한다.

**완료 조건:** 두 Apple desktop target 모두 미설정 C를 사용하며, pending 상한·표시·native/resize·지속 복귀가 각각 확인된다.

## 7. P3 — Android 연결

- [x] Choreographer timestamp와 유효한 window metrics를 사용하여 framework를 먼저 준비한다. window bootstrap/resize와 일반 슬롯 획득을 분리한다.
- [x] `GraphiteVulkanWindow.Render`의 슬롯/acquire 실패가 callback을 막지 않게 한다. render 전에 준비한 장면을 paint에서 다시 만들지 않는다.
- [x] 현재 2-slot 구조에 공통 fresh-only admission을 연결하고 native/shield/replay 전환 시 실제 GPU 완료까지 drain한다.
- [x] swapchain acquire는 정상 C에서 비차단 재시도를 사용하며 GPU full/이미지 부족 시 busy loop 없이 다음 pulse·완료 신호로 깨운다.
- [x] Vulkan fence 완료, renderer terminal, SurfaceView/native view 합성을 구분한다. present 요청 성공만으로 GPU 자원 회수를 통보하지 않는다.
- [ ] Activity pause/resume·재생성, surface destroy/recreate, 회전·IME inset 변경을 확인한다. 이전 view의 GPU retirement hold를 새 view가 우회하지 못하게 한다.
- [ ] 실제 제품 runtime으로 Sample2와 Testbed를 빌드·설치하고 손가락 스크롤·입력·WebView를 확인한다.

**완료 조건:** GPU 두 슬롯이 찬 상태에서도 framework 준비가 이어지고 native/회전 전환과 surface 재생성 후 렌더링이 지속된다.

## 8. P4 — Windows 연결

- [x] Windows App SDK native scheduler부터 managed `BeginFrame`까지 확인한다. presenter backpressure로 render callback 자체가 막히면 준비 pulse를 별도 연결한다.
- [x] Windows MAUI에도 같은 준비/query 연결을 제공한다. 두 호스트의 resize generation·ACK·정확한 출력 크기 계약을 각각 유지한다.
- [x] Vulkan recording/producer와 D3D12 copy/output 슬롯의 ownership·fence를 frame별로 추적한다. producer 완료와 shared source 재사용 가능 시점을 분리한다.
- [x] GPU-only 신호/대기를 사용하는 비동기 경로에 공통 2-frame admission을 연결한다. 정상 프레임의 CPU fence 대기는 자원별 완료 조회·deferred retry로 대체한다.
- [x] native composition의 BeginDraw/EndDraw·commit·front adoption과 frame terminal을 보존한다. native 작업과 resize 중에는 필요한 동기화를 사용한다.
- [ ] 연속 resize, move, minimize/restore, 여러 모니터의 DPI 전환, Editor/IME/WebView·native 삽입/삭제, 종료·device reset을 확인한다.
- [ ] Vulkan direct 출력과 D3D12/composition 출력이 지원되는 구성을 구분해 검증하고 다른 출력 방식의 수명 결과를 대신 사용하지 않는다.

**완료 조건:** 두 Windows 호스트에서 준비가 presenter 대기에 종속되지 않고, producer와 copy/표시 자원 수명이 보장되며 native·resize 계약을 통과한다.

## 9. P5 — Linux Qt 연결

- [x] **기본 Qt Quick 경로**를 우선 적용한다. Vulkan window 대체 경로를 바꾸는 것만으로 Linux 완료를 판정하지 않는다.
- [x] `GraphiteVulkanQuick.Begin/Complete`의 queue idle·copy fence wait를 frame별 producer/Qt consumer 완료 신호로 대체할 설계를 만든다.
- [x] Qt native bridge에서 texture bank의 publication·sampling·retirement를 연결한다. front bank와 pending frame이 동시에 사용하는 자원을 식별하고 두 pending frame의 자원을 격리한다.
- [x] Qt가 아직 sampling 중인 texture를 덮거나 해제하지 않는다. 실패·거절된 commit은 이전 front pixels와 geometry를 유지한다.
- [x] Qt GUI/sync/render thread 사이의 Vulkan queue 접근·신호 순서를 보존한다. 단순히 `WaitQueue`/`WaitForFences`를 삭제하지 않는다.
- [x] 공통 callback 준비와 fresh-only admission을 Quick 및 Vulkan window 경로에 연결한다. native WebView/shield·resize는 직렬 경계를 유지한다.
- [x] bridge ABI가 바뀌면 version/struct size 검사를 갱신하고 Sample2·Testbed·템플릿의 native 소스를 함께 맞춘다.
- [x] SDK·새 템플릿·CMake 기본값을 Quick/Graphite로 맞추고, Quick/WebEngine/Graphite/Desktop 옵션 오류와 누락된 native build/publish 산출물을 거절한다. 옵션별 캐시와 `publish --no-build` 경계를 회귀검사한다.
- [x] Linux package-only 새 템플릿의 무설정 C, Release publish·설치/업데이트/제거, Wayland 20회 resize와 Qt consumer 종료 회수를 검증한다. 확대 배율 실측 DPR 2.25 및 xcb 입력/resize smoke를 별도 기록한다.
- [x] Linux Qt Debug 핫리로드를 CLI·VS Code에 연결한다. 실제 metadata update의 PID/State/입력값/스크롤 유지, 컴파일 오류 복구, 명시 Restart와 Stop을 검증한다. 상세 범위는 §15와 [핫리로드 결과](works/results/2026-10-02-linux-qt-hot-reload.md)를 따른다.
- [ ] 기본 지원 Wayland 환경의 resize·DPR·minimize/restore·WebView/IME와 지원하는 대체 환경을 확인한다. shutdown 뒤 미회수 GPU/Qt lease가 남지 않는지 검사한다.

**완료 조건:** Qt consumer 완료를 증명하는 비동기 수명이 실제 연결되고, 기본 Quick 경로에서 C 동작·화질·native 합성·복귀를 통과한다.

## 10. P6 — 정확성·성능·사용 검증

### 10.1 정확성과 수명

- [x] 공통 CPU 계약 검사에서 callback 순서·coalescing·GPU full·native 전환·replay·generation·역순/중복 완료·실패를 통과한다.
- [ ] 플랫폼의 실제 GPU에서 A/C 정지 픽셀을 비교한다. Off와 Full/Adaptive/Fast/Fixed/Kawase, sigma 0/1/2/4/8/20/32, 변환·경계·native 합성을 포함한다.
- [x] 같은 cache/recorder를 사용하는 두 미완료 recording을 유지해 장면별 snapshot·중간 surface가 서로 오염되지 않는지 확인한다.
- [ ] 생성→재생성→종료, 제출 실패·device loss, native 삽입/삭제, resize/DPR 변경에서 terminal 누락·중복·미완료 자원 재사용이 없음을 확인한다.
- [ ] iOS부터 발견했던 복귀 직후 몇 프레임만 증가하는 문제를 검사한다. 복귀 3회마다 초기 snapshot과 최소 5초 후 snapshot을 비교해 지속 렌더링을 확인한다.

픽셀 오차는 해당 플랫폼의 기존 GPU 허용 기준을 사용하고 A/C 사이에 동일하게 적용한다. 다른 backend 사이의 차이를 pipeline 정확성의 실패나 통과 근거로 대신 사용하지 않는다.

### 10.2 성능과 실제 overlap

플랫폼·runtime·backend별로 같은 계측 바이너리의 A/C를 순서를 교차해 각각 3회 측정한다. 기본 비교 묶음은 **Off, Fast σ20, Fast σ32, Adaptive σ20, Adaptive σ32**의 5개 조건으로, 플랫폼 실행 구성당 **30회**다. B는 표시 대기의 원인을 분리할 필요가 있을 때 추가한다.

- [ ] 실제 표시 FPS와 interval p50/p95/p99, 긴 interval 비율을 측정한다. render/present 호출 횟수로 화면 FPS를 만들지 않는다.
- [ ] callback·장면 생성·raster·submit의 CPU 시간과 GPU 실행/완료·owner callback·표시 시간을 분리한다.
- [ ] 다음 raster가 실제 앞 GPU 실행과 겹치는지 확인한다. 가능한 backend는 hardware start/end와 CPU 시계의 대응·정밀도를 기록한다. 정밀 GPU 시간이 없으면 nonblocking admission·pending 증거와 overlap 미계측 상태를 구분하고 완료 조회 지연만으로 overlap을 확정하지 않는다.
- [ ] pending 최대 2, 준비된 최신 장면 최대 1, 거절·fallback 이유와 GPU/표시 대기열을 집계한다.
- [ ] 물리 또는 검증된 native 입력의 input→scene→실제 표시 지연을 A/C로 비교한다. 합성 스크롤·callback→display 지연과 별도 보고한다.
- [ ] Full/Fixed/Kawase도 σ20/32 smoke와 화질 검사를 수행한다. 기본 묶음 또는 smoke에서 실패·새 병목이 나오면 해당 모드의 반복 비교를 확대한다.

성능 판단:

- 60Hz 환경의 표시 목표는 평균 **58 FPS 이상**, p95 **17.5ms 이하**, p99 **34.2ms 이하**, **25ms 초과 interval 2% 이하**다. 90/120Hz 등은 실제 설정 Hz를 `H`, 주기를 `T`로 두고 평균 `0.967H` 이상, p95 `1.05T`, p99 `2.05T`, `1.5T` 초과 비율 2% 기준으로 보고한다.
- 이미 표시 주기에 가까운 플랫폼은 FPS 10% 증가를 요구하지 않는다. Off·일반 UI·기본 Fast의 반복 회귀 여부와 실제 준비 분리·bounded admission을 확인한다.
- 기존 직렬 병목으로 목표에 미달했던 조건은 A/C 개선율을 보고한다. 반복 평균 10% 개선은 호스트 개선 지표이며 모든 블러 모드의 표시 목표 통과를 뜻하지 않는다.
- 표시 주기를 넘는 GPU 작업은 이 구조만으로 해결됐다고 주장하지 않는다. 목표에 미달한 모드와 CPU/GPU/표시 병목을 구체적으로 남긴다.
- 실제 입력 p95 악화가 반복 측정의 변동 범위를 넘으면 원인을 해결한다. FPS 증가만으로 입력 지연 검사를 통과시키지 않는다.

### 10.3 일반 사용

- [ ] benchmark와 frame별 진단을 끄고 첫 진입·페이지 재진입·강제 종료 후 재실행의 C 기본 선택을 확인한다.
- [ ] 느린/빠른 스크롤·방향 전환, sigma 슬라이더, 모드 변경·Off 전환을 확인한다.
- [ ] native 버튼·텍스트 입력·IME·selection·WebView, 회전/resize/DPR 전환을 확인한다.
- [ ] 플랫폼별 최종 앱을 10분 사용하고 Off와 비교한다. 멈춤·깜빡임·입력 지연·복귀와 시작/종료 자원 상태를 기록한다.
- [ ] 도구로 실행 가능한 검사와 실제 손가락/IME 관찰을 구분한다. 짧은 정상 동작 회신을 정량 입력이나 10분 프로토콜 통과로 확대하지 않는다.

## 11. P7 — 기본 적용과 결과 정리

- [x] 각 제품 호스트가 같은 공통 C 규칙을 실제 사용하도록 기본값을 적용한다. capability 부족·device recovery·native/resize fallback 이유를 진단에 남긴다.
- [x] 환경 설정 없이 실행한 정책을 확인하고 A 복귀 옵션, 지원하는 B 비교 옵션을 검증한다. 비Apple 실제 실행, Apple 검증은 사용자 요청 SKIPPED.
- [ ] Sample2/Testbed, host package, CLI·새 프로젝트 템플릿의 설정과 설명을 일치시킨다. 개발 runtime의 결과를 배포 runtime 검증으로 대신 사용하지 않는다.
- [ ] 공통 CPU/GPU 및 변경한 플랫폼의 기존 build·native bridge·resize·입력 검사를 통과한다. 다른 OS의 실행 환경이 없으면 빌드·소스 검사와 실제 실행 미검증 상태를 구분한다.
- [x] 결과를 `works/results/<실행일>-native-frame-pipeline.md`와 플랫폼별 JSON으로 기록한다. 원본은 `temp/testing/native-frame-pipeline/<platform>/<run>/`에 보존한다.
- [x] 실제 payload/source hash, runtime/SDK, 표시 구성, 정책·fallback, 성공·실패 실행, 재시도 이유, 수동 결과를 보존한다. 실패 실행을 제거해 통과 평균을 만들지 않는다.
- [x] `git diff --check`를 통과하고 `work4.md`의 과거 수치·미검증 항목은 보존한다. 새 결과를 기존 payload의 결과로 소급하지 않는다.

**전체 완료 조건:** 모든 범위 내 네이티브 제품 호스트에 공통 준비·admission·완료 계약이 연결되고 C가 기본으로 선택된다. 플랫폼별 실제 GPU·native 합성·lifecycle·기본 Fast 성능과 일반 사용을 검증한 최종 앱 및 결과 자료를 제공한다. 대체 경로의 제한과 목표에 미달한 옵션은 별도로 명시한다.

## 12. 단계별 산출물

| 단계 | 산출물 | 다음 단계 진입 근거 |
| --- | --- | --- |
| P0 | 플랫폼/host/backend/owner/wait/completion 표와 기준 identity | 제거할 대기와 대체 신호를 설명할 수 있음 |
| P1 | 공통 정책·descriptor·CPU 회귀, iOS 기준 유지 | 실제 공통 코드와 iOS 기존 계약 검사 통과 |
| P2 | macOS·Mac Catalyst 후보와 실행 증거 | 새 장면 max 2, native/resize·복귀 통과 |
| P3 | Android 후보와 실행 증거 | full queue 준비·Vulkan 수명·surface 복귀 통과 |
| P4 | Windows App SDK·MAUI 후보와 실행 증거 | scheduler 준비·producer/consumer retirement·resize 통과 |
| P5 | Linux Quick 및 지원 대체 경로 후보와 실행 증거 | Qt sampling 수명·비동기 publication·native/복귀 통과 |
| P6 | 플랫폼별 픽셀·30회 핵심 비교·입력·10분 사용 자료 | 정확성·bounded queue·회귀 기준과 미달 범위가 확인됨 |
| P7 | 기본 C 앱/패키지·템플릿·문서와 최종 집계 | 무설정 실행·rollback·제품 runtime 검증 완료 |

이 계획의 우선 해결 대상은 **host 연결과 자원 수명**이다. iOS의 flag 이름을 다른 OS에 추가하는 작업이나 GPU 상한만 2로 바꾸는 작업으로 전체 완료를 판정하지 않는다.

## 13. 실행 상태 — 2026-10-02

| 단계 | 최종 상태 |
| --- | --- |
| P0 | host/backend/owner/wait/retirement 표와 identity 기록. 일부 host의 전체 frame별 timing·input/display 연결은 미완료 |
| P1 | 공통 계약·CPU 회귀 PASS, iOS 옵션 호환 유지. Apple Metal/lifecycle SKIPPED |
| P2 | AppKit/Catalyst 연결 구현. Apple build/runtime/사용 검증 SKIPPED |
| P3 | x64·arm64 Release APK build, emulator A/B/C·native 재생성·3회 지속 복귀 PASS. physical Android GPU/입력 미검증 |
| P4 | 두 Windows 호스트 C/A/B, Release shader smoke, AppSDK native·resize·reset/recovery PASS. MAUI native 입력 adapter·physical DPI/IME 미완료 |
| P5 | 기본 Quick ABI 6 및 Qt consumer drain PASS. Vulkan-window와 OpenGL xcb 별도 smoke. software Vulkan, OpenGL Wayland 실패 보존 |
| P6 | CPU 및 실제 Radeon Vulkan 31조건 A/C 픽셀·두 recording PASS. 실제 표시 계측 권한 부족; 정량 30회·물리 입력·10분 사용 미완료 |
| P7 | 기본 C·옵션·샘플·템플릿·문서·플랫폼 JSON 반영. 제품 runtime 제한과 실패 원본 보존. 전체 인수 PARTIAL |

Apple 검증 관련 미체크 항목은 **SKIPPED(사용자 요청)**이며 재검증을 시도하지 않았다.
그 밖의 미체크 항목은 부분 증거만 있거나 실제 환경/사람의 관찰이 필요한 인수다.

## 14. Linux Qt 구성 후속 검토 — 2026-10-02

이번 요청의 범위는 Linux Qt 구성 검토·보완이다. 다른 OS의 인수나 기존 전체 완료 조건을 새로 통과 처리하지 않는다. 기존 플랫폼 JSON은 이전 payload의 증거로 보존하며 새 결과를 [구성 검토 보고서](works/results/2026-10-02-linux-qt-configuration-review.md)와 [JSON](works/results/2026-10-02-linux-qt-configuration-review.json)에 추가했다.

확인한 미흡 사항과 수정:

- 샘플은 Quick을 사용하지만 SDK·템플릿·직접 CMake 빌드는 Widgets 기본값이었다. 모두 Quick/Graphite 기본값으로 맞췄다. WebEngine은 SDK·템플릿에서 선택 사항, 샘플에서 기본 활성화다.
- 옵션을 바꿔도 같은 native build 디렉터리를 사용했다. Configuration과 Quick/Graphite/WebEngine/GStreamer 조합별로 분리하여 다른 옵션의 `--no-build` 게시가 캐시를 대신 사용하지 못하게 했다.
- 네이티브 라이브러리가 없으면 복사 target을 건너뛰었다. `DOROTIQT006`으로 실패하게 하고 잘못된 boolean·지원하지 않는 조합도 먼저 거절한다. 명시적인 prebuilt 디렉터리는 동일 옵션·ABI의 산출물을 제공해야 한다.
- OpenGL 비교를 위한 Graphite 옵션을 MSBuild에서 CMake에 전달하지 않았다. `DorotiQtGraphite=false`를 연결하고 Quick/WebEngine 비활성화 및 `DOROTI_LINUX_GRAPHITE=0` 실행 조건을 문서화했다.
- 문서의 매 프레임 queue idle·copy fence wait 설명과 Qt 최소 버전을 현재 구현에 맞췄다. 기본 C는 producer/copy fence 및 `afterFrameEnd` 이후 Qt consumer fence로 회수하며, native/resize와 종료 시 동기화는 유지한다. ABI 6/208바이트는 변경하지 않았다.

검증은 Ubuntu 26.04, .NET SDK 10.0.400/runtime 10.0.11, Qt 6.10.2, Wayland/KDE와 llvmpipe에서 수행했다. Testbed Debug·Sample2 Release build, CPU 회귀, Vulkan 31조건 A/C 픽셀·두 recording, Wayland 전체 smoke·20회 resize, 확대 배율(DPR 2.25), xcb 입력/resize, Sample2 default/A/B/C, OpenGL/xcb serial, 격리 NuGet-only 템플릿 publish·설치 검사를 통과했다. Sample2 무설정 C에서 pending 최대 2를 관찰했고 종료 consumer 제출/완료가 일치했다.

물리 GPU·실제 표시 FPS/overlap·30회 성능 비교·물리 IME/Orca·동적 DPR/화면 이동·최소화/복원·10분 사용·clean OS 배포는 별도 인수다. 확대 배율 실행을 동적 DPR 전환으로, xcb smoke를 기존 Qt WSI 경쟁 조건 해결로 간주하지 않는다. OpenGL Wayland 문제도 수정 완료로 표시하지 않는다.

## 15. Linux Qt 핫리로드 — 2026-10-02

추가 요청에 따라 Linux Qt를 `describe`/`dev` 및 VS Code 개발 대상 목록에 연결했다. `dev -Platform linux`는 `DorotiQtDevelopment=true`, 최적화하지 않은 Debug·portable 심볼·startup hook을 사용한다. Release/AOT/trim/single-file 등의 개발 실행은 거절한다. 기존 Quick/Graphite·공통 C 프레임 정책은 유지한다.

현재 환경의 inotify 인스턴스 제한(128)으로 watcher 시작 실패를 재현하여, Linux `dev`는 `DOTNET_USE_POLLING_FILE_WATCHER`가 없을 때 `1`을 설정한다. 명시한 환경 설정은 보존한다. 지원하지 않는 수정은 기존 앱을 유지하며 사용자가 Restart를 선택할 때 새 프로세스와 상태를 만든다.

CLI와 격리 프로필에 설치한 VSIX에서 실제 C# method-body 변경, 같은 PID/State·카운터·한글 텍스트·스크롤 유지, 컴파일 오류 복구, rude edit, 명시 Restart 및 Stop을 통과했다. 검사에는 별도 생성한 소스 템플릿과 repository ProjectReference를 사용했다. NuGet-only 핫리로드, 여러 창·대체 renderer, native C++/QML/assets 변경, 물리 입력이나 실제 표시 성능 인수로 확대하지 않는다.

검증한 로컬 확장 `doroti-local.doroti@0.1.0`을 현재 VS Code에 설치했고 `.vscode/settings.json`에 이 워크스페이스의 CLI와 .NET 경로를 구성했다. **Doroti: Select Project → Select Target(`linux`) → Run**으로 시작하고 C# 저장 또는 **Hot Reload**로 반영한다.

```sh
pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiSampleApp2 -Platform linux
```

[상세 실행 결과](works/results/2026-10-02-linux-qt-hot-reload.md) · [개발 실행 계약](Doroti/docs/development-hot-reload.md).

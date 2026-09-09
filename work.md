# 네이티브 Graphite Vulkan·Metal 전환 작업계획

작성일: 2026-09-09. 상태: **전체 전환 PARTIAL — Windows native bridge 검증에 이어 공통 Metal session·macOS AppKit Graphite 후보 구현, 실제 Material 출력·pkg 생성·분리 경로 실행 검증. Linux ABI 2 Vulkan bridge·108프레임 진단과 SVGA3D GPU 기준선 재검증 추가. 5개 OS 기본값 전환 미완료**.

## 0. 후속 구현 실행 현황 (2026-09-09)

### 0.1 앞선 Windows 실행 기록

앞선 `work.md의 전체작업` 요청으로 구현을 시작했다. 아래 최초 계획의 단계와 수용 기준을 유지한다. [상세 실행 보고서](Doroti/docs/validation/native-graphite-2026-09-09.md)와 [버전 관리되는 결과·hash·첫 실패](history/26-09-09/native-graphite-execution.json)에 근거를 남겼다.

- **NG0 PARTIAL:** baseline commit/diff, OS/GPU/driver, target manifest, 고정 NuGet의 native asset inventory/hash, 실제 probe 로딩 경로, budget·in-flight 한도를 기록했다. 현재 Vulkan/D3D11 capability는 RTX 4060 Laptop·Radeon 780M 모두 PASS다. 실제 앱 장면/성능/메모리/물리 기준선과 최종 지원 정책은 미완료다.
- **NG1 PARTIAL:** 기존 검증 프로젝트에 Graphite probe를 구현했다. context/recorder, color/text/gradient/offscreen draw, raster upload, 비동기 readback과 정상 종료를 실행했다. 고정 Skia 안에 texture state 조회/갱신·wait/signal semaphore·GPU 작업 polling C ABI를 통합하고 win-x64 DLL을 재빌드했다. 두 GPU에서 각각 36프레임의 외부 texture → Vulkan copy → 동일 wrapper 재사용이 PASS, synchronization validation 경고/오류 0이다. 원본/재빌드의 작은 offscreen 장면 hash도 일치한다. 텍스트/gradient 시각 승인, 출력 자원 반환, device loss/미완료 작업 종료, Metal, feature/extension 전달 및 RID 패키징은 아직 미검증/미구현이다.
- **NG2–NG6 미완료:** NG1 전체 gate가 열려 있어 제품 session/host 교체와 기본값 승격은 수행하지 않았다. Apple 실행 환경 및 연결된 Android 장치도 확보되지 않았다. Windows MAUI·Catalyst 유지 범위를 축소하거나 이전 제품 경로를 삭제하지 않았다.
- **NG7 PARTIAL:** README 양 언어판·ADR의 현재 기본값, `work3.md`, Linux target의 실제 v2 ABI/GL backend 표기를 정리했다. 제품 승격·dependency 제거·clean publish는 미완료다.
- FCR-7 Material/widget 계약과 Linux Qt ABI/keyboard/clipboard 계약 PASS. 각 검증에 외부 20분 timeout을 적용했다. Web browser·실기기·physical scan-out·성능 개선은 `notVerified`다.

실행/복구 명령은 [native bridge README](Doroti/native/graphite/README.md)에 있다. 시험 DLL은 별도 probe 프로세스에만 명시적으로 로드하며 NuGet cache나 제품 산출물을 교체하지 않는다. 아래 `[ ]`는 부분 증거만으로 체크하지 않은 **전체 완료 조건**이다.

### 0.2 이번 Apple 환경 후속 구현

당시 Apple 후속 실행 환경은 **Apple M1·macOS 26.6.2·Xcode 26.6·.NET 10.0.400**이었다. 이전 Windows 실행의 Apple 환경 미확보 상태를 그대로 적용하지 않았다. [Apple 상세 보고서](Doroti/docs/validation/native-graphite-apple-2026-09-09.md), [공통 session 계약](Doroti/docs/architecture/native-graphite-session.md), [버전 관리되는 실행 결과](history/26-09-09/native-graphite-apple-execution.json)에 추가 근거를 기록한다.

- **NG1 PARTIAL:** 고정 macOS native asset의 실제 Graphite/Metal context 생성, 외부 texture 36프레임 재사용·async readback·색상/이미지/offscreen/runtime shader 픽셀 검증 PASS. 창 probe에서 Graphite 명령 버퍼 44개 완료, resize 20회·최소화/복원·숨김/복귀·진행 중 종료 후 자원 반환 PASS. Metal API Validation 활성 로그에 오류/경고 없음. device loss·전체 플랫폼 ABI/배포 gate는 남아 있다.
- **NG2 PARTIAL:** `SkiaGraphiteSession`에 단일 owner/thread·generation, context/recorder 예산, view별 image provider/cache, bounded frames, recorder surface·async readback, GPU 완료 후 반환을 구현했다. native Metal RuntimeEffects ID와 AppKit renderer cache 해제를 연결했다. Vulkan session·전체 image export/cache·Web browser 검증은 미완료다.
- **NG4 PARTIAL:** `DOROTI_MACOS_GRAPHITE=1`로 실제 macOS 제품 host의 Graphite 후보를 명시적으로 선택할 수 있다. MTKView·입력·transaction presentation 구조를 보존했다. Material 샘플에서 Ganesh/Graphite 모두 새 장면 2회·재표시 2회·명령 버퍼 5개 완료, 오류와 CPU readback/copy 0을 확인했다. iOS/Catalyst handler 전환·실기기·제품 전체 lifecycle은 미완료다.
- **NG7 PARTIAL:** canonical `build/run/publish` 실행 및 macOS `.pkg` 생성 PASS. 패키지를 별도 디렉터리에 풀어 Graphite 앱을 실행해 first-content/replay를 확인했다. 시스템 설치·clean publish·기본값 승격을 완료로 간주하지 않는다. README 양 언어판과 새 보고서를 동기화했다.
- Runtime shader·FCR-7 전체 계약·Web host 빌드 PASS. Windows/Linux/Android 제품 전환, 실제 WebGPU/WebGL 브라우저 회귀, 성능·메모리 비교와 사람의 물리 화면/입력 승인은 미완료다. Android 연결 장치가 없고 등록된 iPhone 12는 unavailable 상태다.

기본값은 계속 기존 경로다. macOS 후보를 사용하지 않으려면 `DOROTI_MACOS_GRAPHITE` 옵션을 제거한다. 아래 checkbox는 각 항목 전체 완료 조건이므로 일부 Apple 증거만으로 체크하지 않는다.

### 0.3 이번 Linux 환경 후속 구현

현재 작업 환경은 **Ubuntu 26.04·VMware SVGA II/vmwgfx·Qt 6.10.2·.NET 10.0.400**이다. [Linux 상세 보고서](Doroti/docs/validation/native-graphite-linux-2026-09-09.md)와 [실행 결과·hash·첫 실패](history/26-09-09/native-graphite-linux-execution.json)에 근거를 추가한다.

- **GPU 가속 기록 재확인:** 사용자 지적에 따라 ADR-022/023과 8월 20일 기록을 대조했다. 현재도 `SVGA3D; ... LLVM`, `vmwgfx`, DRI3, direct rendering이 확인된다. 이는 llvmpipe가 아니며 **기존 OpenGL GPU 가속은 유효**하다. 실제 Wayland/XWayland Material 출력·각 20회 resize·정상 종료 PASS, software fallback·실패 프레임·전체 화면 CPU copy 0이다. 반면 현재 Vulkan이 노출하는 장치는 CPU llvmpipe 하나다. 두 API의 지원 증거를 혼동하지 않는다.
- **NG0 PARTIAL:** Linux OS/GL/Vulkan·실제 loader/managed/native hash·target manifest·예산·frame 한도를 기록했다. 시스템 설치 없이 별도 디렉터리에 Clang/Ninja/Khronos validation layer를 준비했다. 물리 Vulkan·성능/메모리 기준선은 미검증이다.
- **NG1 PARTIAL:** 같은 pinned Skia에 ABI 2 enabled Features/Features2 pNext·instance/device extension 전달과 VMA 연결을 추가하고 `libSkiaSharp.so`를 빌드했다. 실제 활성화한 robustBufferAccess·16-bit storage·maintenance1을 전달한 상태에서 3개 instance/device/context generation, 총 108개 외부 texture frame의 상태 조회/갱신·semaphore·copy fence·반환이 PASS다. validation 경고/오류 0, stock/rebuilt의 세 offscreen hash 일치. 모두 명시적 CPU Vulkan 진단이며 물리 GPU·제품 출력 증거가 아니다.
- **NG1 오류 경로 보완:** 제출 직후 예외에서 wrapper가 GPU drain보다 먼저 해제되던 probe 수명 문제를 수정했다. 강제 제출 후 실패는 예상한 원문 오류와 exit 1을 보존하며 validation 0으로 종료했다. 모의 `VK_ERROR_DEVICE_LOST` 전달·IsDeviceLost·종료 PASS, 실제 장치 손실/복구는 미검증이다.
- **NG2/NG7 검증·문서:** Linux native asset 참조가 누락된 Runtime shader/FCR-7 실행 프로젝트를 보완해 표준 명령 PASS. Qt ABI/keyboard/clipboard PASS. canonical Linux build/run/publish 및 별도 디렉터리 publish 실행 PASS. Windows managed probe cross-build PASS이며 Windows 실행 PASS가 아니다. README 양 언어판·work3의 현재/과거 Web 기본값 설명을 동기화했다.
- **Web:** canonical build PASS. Chrome은 SVGA3D 기반 WebGL GPU 장치를 사용하며 실제 direct presenter와 Material 화면/테마/포인터/스크롤/상태 유지 6개 browser 계약 PASS. WebGPU requestAdapter는 null이고, 미지원 오류 표시·자동 WebGL fallback 금지 1개 계약 PASS. 전체 WebGPU 출력·물리 입력/접근성은 미검증이다.

초기 누락 native library, workload 설치 중 canonical artifact 등록 거부, 동시 rebuild 충돌, 너무 이른 device-loss 주입과 브라우저 검증 도구 누락 기록을 보존한다. 이후 성공으로 첫 실패를 덮지 않는다. **공통 제품 Vulkan session·각 플랫폼 adapter/WSI·RID 제품 패키징·기본값 승격은 아직 미완료**이며, 이번 증거를 기존 제품 경로 삭제 근거로 사용하지 않는다.

## 1. 목표와 검토 결론

사용자가 지정한 다음 구성을 네이티브 제품의 최종 기본 렌더링 경로로 채택한다.

| 플랫폼 | 목표 렌더러 | 화면 출력 경계 |
| --- | --- | --- |
| Windows | Skia Graphite → Vulkan | 기존 Windows App SDK HWND·Windows Presentation·DirectComposition 유지 |
| Linux | Skia Graphite → Vulkan | Qt 창·입력 유지, Vulkan surface/swapchain 출력으로 전환 |
| Android | Skia Graphite → Vulkan | MAUI 호스트 유지, Android Surface/ANativeWindow 기반 출력 추가 |
| macOS | Skia Graphite → Metal | AppKit MTKView/CAMetalLayer 출력 유지 |
| iOS | Skia Graphite → Metal | MAUI/UIKit 호스트 유지, Graphite를 소유하는 Metal view/handler로 전환 |

**구성은 기술적으로 타당하며, 현재 고정 버전에 Graphite Vulkan·Metal 생성 API도 있다. 다만 바로 API 이름만 교체할 수 있는 작업은 아니다.** Graphite 기록·제출 수명과 외부 texture 상태/동기화 연결을 먼저 입증해야 한다. 특히 Vulkan은 공개 바인딩의 interop 공백이 선행 차단 요소다. 성능 향상은 아직 측정하지 않았으므로 보장하지 않는다.

Windows의 `Graphite → Vulkan`은 **Skia의 GPU 렌더링 API**를 뜻한다. 현재 제품의 D3D11 공유 texture 및 Windows Presentation/DirectComposition을 통한 최종 합성은 재사용한다. D3D 계열 출력 연동까지 제거하고 Win32 Vulkan WSI로 바꾸는 것은 이번 계획의 목표가 아니다.

최초 검토·작성 요청의 산출물은 이 계획이었으며 당시 제품 코드·패키지·기본값을 변경하지 않았다. 후속 전체 구현 요청의 진행은 0절에 기록한다. [work3 보관 기록](history/26-09-08/work3-summary.md)과 `history/`의 과거 실행·실패 기록은 보존하며 구현은 아래 단계의 범위를 따른다.

## 2. 현재 코드 기준선

아래 표는 최초 2026-09-09 checkout의 기준선이다. 이후 추가된 macOS 명시적 Graphite 후보는 0.2절에 기록한다. 문서나 과거 PASS를 현재 실행 결과로 간주하지 않는다.

| 대상 | 현재 확인한 구조 | 변경량·주의점 |
| --- | --- | --- |
| 공통 패키지 | `Doroti/Directory.Packages.props`: SkiaSharp 계열 `4.154.0-preview.1.26454.9` | 일괄 최신화보다 이 버전의 실제 native asset/API 적합성부터 확인 |
| Windows App SDK | `DorotiWindowsAppSdkRunner.ResolveRequestedPresenter()` 기본값 `Vulkan`; `WindowsManagedVulkanPresenter`는 `GRContext.CreateVulkan` | Vulkan 장치·출력은 존재하지만 Skia 엔진은 Ganesh |
| Windows MAUI | `DorotiWindowsDxgiSurface`, `WindowsCompositionSurfacePresenter`의 GL/Direct3D Ganesh 경로 | 독립 실행 대상이므로 최종 Windows 구성에 맞춘 별도 연결 필요 |
| Linux Qt | `QtSkiaSurface`의 `GRContext.CreateGl`; native template의 `QOpenGLWindow`·FBO·C ABI v2 | GL 전용 surface 계약을 Vulkan 계약으로 교체해야 함 |
| Android MAUI | `MauiSkglSurface`의 `SKGLView`, `GRContext` 검사 | Vulkan용 native view/handler와 surface 수명 관리 신설 필요 |
| macOS AppKit | `DorotiMacOSMetalView`의 `GRContext.CreateMetal`, MTKView·drawable·command queue 직접 소유 | 창·drawable 흐름을 보존하면서 Skia 기록/제출 부분 교체 |
| iOS | `DorotiIosMetalViewHandler`: `SKGLView` 추상화 아래 실제 `SKMetalView`·Ganesh Metal | 클래스 이름만 보고 OpenGL로 분류하지 않음. Ganesh 소유 view 교체 필요 |
| Mac Catalyst | `DorotiMacCatalystSkglViewHandler`의 `SKMetalView` | 공유 Apple handler 변경에 따른 유지보수 대상. 같은 Graphite Metal 계약 적용 |
| Web | `DorotiWebWorkerSurface.Graphite.cs`에 Graphite/Dawn 구현; loader의 기본값은 `worker-direct-webgpu` | 구조 참고 및 공통 변경 회귀 대상. 별도 네이티브 Vulkan/Metal 전환 대상 아님 |
| 공유 렌더러 | `SkiaSceneRenderer`, `SkiaGpuSurfaces`, RuntimeEffects | recorder 등록, GPU offscreen surface, image provider 재사용 가능. native backend 식별·캐시 수명 확장 필요 |

관련 파일은 모두 `Doroti/src/` 아래 해당 host 프로젝트에 있다. Linux native 원본은 `Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/`이며 생성된 앱 복사본만 수정해서는 안 된다.

확인된 문서 불일치:

- `README.md` 앞부분·ADR-027에는 Windows ANGLE 기본값 설명이 남아 있으나 현재 runner는 Vulkan을 기본 선택한다.
- `README.md`와 `work3.md`의 WebGL 기본값/일부 모드 설명은 현재 WebGPU 기본 선택 코드와 다르다.
- Linux target manifest는 `nativeAbi=v1`을 선언하지만 실제 managed/native 코드는 v2다.
- ADR-027의 과거 SkiaSharp 4.152 표기는 현재 중앙 패키지 4.154와 다르다.

위 항목은 NG7에서 현재 동작 설명을 정리한다. 당시 실패·측정치와 미검증 상태는 삭제하거나 PASS로 다시 표시하지 않는다.

## 3. 외부 소스 검토와 선행 위험

### 3.1 고정 패키지의 Graphite API

로컬 NuGet `skiasharp.nuspec`의 repository commit은 `143a933a753dbfeca1909524b2c06c546c5c3e20`이다. 아래 바인딩 검토는 움직이는 main이 아니라 이 commit을 기준으로 했다.

- `SKGraphiteContext.CreateVulkan(SKGraphiteVkBackendContext)` 및 `CreateMetal(SKGraphiteMtlBackendContext)`가 있다. `IsBackendAvailable`로 빌드된 backend를 조사할 수 있고, `CreateRecorder → Snap → InsertRecording → Submit`이 제출 경로다. API 존재와 장치에서의 성공은 별개다. [고정 버전 Context 소스](https://github.com/mono/SkiaSharp/blob/143a933a753dbfeca1909524b2c06c546c5c3e20/binding/SkiaSharp/Gpu/Graphite/SKGraphiteContext.cs)
- Vulkan context는 raw instance/device/queue 및 procedure callback을 받는다. 기존 Ganesh용 `GRSilkNetBackendContext`를 Graphite context로 그대로 넘길 수 없다. callback·extension·feature·allocator 계약을 native shim까지 확인한다. [고정 버전 Vulkan context](https://github.com/mono/SkiaSharp/blob/143a933a753dbfeca1909524b2c06c546c5c3e20/binding/SkiaSharp/Gpu/Graphite/SKGraphiteVkBackendContext.cs)
- `SKGraphiteBackendTexture.CreateVulkan/CreateMetal`로 외부 texture를 감쌀 수 있다. 하지만 확인한 클래스에는 Vulkan image layout/queue-family 변경 후 상태 조회·갱신 메서드가 없다. [고정 버전 BackendTexture](https://github.com/mono/SkiaSharp/blob/143a933a753dbfeca1909524b2c06c546c5c3e20/binding/SkiaSharp/Gpu/Graphite/SKGraphiteBackendTexture.cs)
- 확인한 `SKGraphiteInsertRecordingInfo`에는 recording·target·translation·clip만, `SKGraphiteSubmitInfo`에는 Sync·MarkBoundary·FrameID만 노출된다. 외부 wait/signal semaphore와 최종 texture state 전달을 이 구조체만으로 해결했다고 가정하면 안 된다. [고정 버전 생성 바인딩](https://github.com/mono/SkiaSharp/blob/143a933a753dbfeca1909524b2c06c546c5c3e20/binding/SkiaSharp/SkiaApi.generated.cs)

**NG1은 필수 중단 지점이다.** 공개 API로 안전한 native present 연동을 입증하지 못하면 해당 Skia revision의 C shim/바인딩 확장과 재현 가능한 native asset 빌드를 먼저 수행한다. 외부 texture layout을 임의 상수로 추정하거나 프레임마다 wrapper를 다시 만들어 문제를 숨긴 상태로 제품 전환하지 않는다. native bridge가 필요하면 동일 Skia build 안에 통합하고, 서로 다른 Skia 인스턴스 사이에 C++ 포인터를 전달하지 않는다.

### 3.2 플랫폼 제약

- **Windows:** 현재 Vulkan 1.1·하드웨어 adapter·LUID 일치·D3D11 dedicated external-memory import 조건을 보존한다. 이 조건이 Graphite의 필수 feature/extension까지 보장하지는 않는다. 현재 제품 출력은 Vulkan WSI가 아닌 Windows Presentation이므로 WSI의 acquire/present 상태기계를 그대로 이식하지 않는다.
- **Linux:** Qt는 `QVulkanWindow`에서 device/queue/swapchain을 관리하지만 고급 용도에는 직접 `QWindow`를 구현할 수 있다고 설명한다. Graphite도 제출을 소유하므로 두 주체가 같은 queue를 독립 제출하는 설계를 피한다. **우선안은 Qt QWindow+QVulkanInstance와 Doroti 소유 Vulkan 출력**이며, QVulkanWindow는 공개 동기화 계약만으로 interop이 증명될 때만 대안으로 선택한다. 현재 CMake의 Qt 최소 6.5와 최신 문서 API의 도입 버전 차이도 조사한다. [Qt QVulkanWindow](https://doc.qt.io/qt-6/qvulkanwindow.html)
- **Android:** 현재 MAUI host 최소 API는 21이다. Vulkan은 API 24 이상 및 Vulkan 지원 장치가 필요하므로 최종 Vulkan 전용 제품은 최소 API 24 이상으로 올려야 한다. Graphite가 더 높은 Vulkan feature/API를 요구하면 그 조건까지 반영한다. API 레벨만으로 GPU 지원을 보장하지 않는다. [Android Vulkan 요구사항](https://developer.android.com/ndk/guides/graphics/getting-started)
- **Apple:** 고정 버전 upstream Metal 테스트는 GPU family 선검사와 simulator의 일부 shader 실패를 별도로 다룬다. Metal 지원 OS라는 이유만으로 Graphite 성공을 단정하지 않는다. 실제 연결된 Skia revision의 Metal Caps와 GPU family를 대조하고 native abort 전에 capability를 판정한다. simulator PASS는 실기기 PASS가 아니다. [고정 버전 Metal renderer](https://github.com/mono/SkiaSharp/blob/143a933a753dbfeca1909524b2c06c546c5c3e20/tests/Tests/SkiaSharp/Visual/Renderers/GraphiteMetalRenderer.cs)
- **Vulkan WSI:** 제출 fence 완료와 presentation의 semaphore 소비·자원 반환은 별개다. Linux/Android의 present semaphore 재사용·swapchain 파기에는 별도 근거가 필요하다. [Khronos semaphore 재사용 지침](https://docs.vulkan.org/guide/latest/swapchain_semaphore_reuse.html)

## 4. 설계 기준

### 4.1 공유 부분과 플랫폼 소유권

공통 흐름은 `플랫폼 frame 획득 → recorder용 SKSurface → 기존 SkiaSceneRenderer → Snap → InsertRecording → Submit → 플랫폼 present → 자원 반환`이다. 실제 method 이름과 타입은 NG2에서 확정하며 이 문장의 추상 흐름을 기존 API로 오인하지 않는다.

- view/session마다 GPU device/context generation, recorder, image provider/cache, in-flight 자원을 명시적으로 소유한다. 초기 구현은 recorder 단일 owner/thread를 사용한다.
- 공통 코드는 Graphite session·기록 제출·오류·캐시 정책을 담당한다. native window, swapchain/drawable, queue 외부 동기화, present 및 release는 host가 담당한다.
- Web의 static JS worker 객체를 native 전역 singleton으로 복사하지 않는다. 다중 창·다중 view 간 캐시/texture 섞임을 막는다.
- `SkiaGpuSurfaces.Register/CreateCompatible`를 이용하되 owner와 generation에 맞게 해제한다. `SKCanvas.Context != null`만으로 GPU를 판정하는 코드를 제거/보완한다.
- `DorotiSkiaRuntimeEffects`의 backend allowlist·진단 ID, image filter pool, picture/raster cache, texture-backed image 수명을 native Graphite와 연결한다. Ganesh용 예외 정책은 효과별 검증 후 조정한다.
- raster image upload에는 recorder의 image provider를 사용한다. GPU readback은 지원되는 비동기 요청/완료 경로로 검증하며 `SKImage.ReadPixels` 같은 기존 동기 경로를 무조건 유지하지 않는다.
- context/recorder budget을 명시한다. `default(SKGraphiteContextOptions)`의 0-byte cache 의미에 유의하고 동일 메모리 예산으로 기준선을 비교한다.

### 4.2 프레임·수명 계약

- Framework의 rebuild/layout 대상·순서, lifecycle, semantics, focus/IME, retained scene 의미를 보존한다. 병렬 layout/다중 recorder 최적화는 포함하지 않는다.
- exact viewport, DPR, resize generation, input/scene sequence, replay/superseded 판정을 유지한다. 과거 generation의 비동기 완료는 새 view의 present로 기록하지 않는다.
- `Submit` 성공은 GPU 완료나 화면 표시 완료가 아니다. recorded/submitted/GPU-complete/present-requested/platform-terminal을 구별하고 physical scan-out은 별도 증거로 남긴다.
- frame slot, texture, drawable, callback을 GPU와 플랫폼 소비가 끝날 때까지 보존한다. in-flight 큐는 bounded이며 NG0에서 기준선과 한도를 기록한다.
- Vulkan barrier는 실제 image layout/access/stage/queue-family를 기준으로 한다. 같은 queue의 실행 순서만으로 메모리 가시성과 layout transition이 해결됐다고 판단하지 않는다.
- Metal은 Graphite submit과 present command buffer가 같은 queue에서 올바르게 순서화되도록 한다. drawable texture의 usage/framebufferOnly와 샘플링·효과 요구를 확인한다.
- 정상 종료는 신규 frame 차단 → 제출/플랫폼 자원 반환 확인 → surface/cache/recorder/context → platform device/window의 의존 순서로 정리한다. device loss는 정상 drain과 분리하고 무한 wait를 허용하지 않는다.
- 호환되지 않는 장치에는 명확한 backend/capability 오류를 출력한다. 네이티브 Ganesh/OpenGL로 자동 fallback하지 않는다.

### 4.3 범위와 전환 정책

- 기본 Windows App SDK뿐 아니라 명시적 Windows MAUI runner도 최종 Graphite/Vulkan 대상이다. MAUI runner 자체를 제거하는 것으로 대체하지 않는다.
- Mac Catalyst는 기존 지원을 유지하며 Apple 공유 코드 변경에 맞춰 Graphite/Metal로 전환한다.
- Web의 `worker-direct-webgpu` 기본값과 명시적 `worker-direct-webgl`은 유지한다. 네이티브 Ganesh 정리 중 WebGL이 의존하는 공통 기능을 삭제하지 않는다.
- 전환 중 기존 native Ganesh는 명시적인 비교/복구 경로로만 유지한다. 해당 플랫폼의 NG7 승격 이후 제품 선택지에서 제거하고 과거 증거는 history에 보존한다. 한 플랫폼 PASS로 다른 플랫폼 경로를 삭제하지 않는다.
- 실패 시 마지막 검증된 버전/명시적 이전 경로로 복구한다. 롤백 실행과 자동 fallback은 구분한다.

## 5. 실행 단계

상태 표기: `TODO` 미착수, `PASS` 해당 증거 범위 통과, `PARTIAL` 구현/일부 검증 완료, `BLOCKED` 선행 기술 조건 미충족, `notVerified` 실행 환경·측정 증거 없음. 현재 단계 상태는 0절 및 실행 보고서를 따르며, 아래 checkbox는 각 항목의 전체 완료 여부다.

### NG0 — 재현 가능한 기준선과 지원 표 확정

- [ ] 현재 commit/diff, 앱·host·Skia managed/native 파일 hash, OS/GPU/driver, target RID, 실행 옵션을 기록한다.
- [ ] Windows App SDK/MAUI, Linux x64, Android arm64/x64, macOS arm64, iOS device/simulator, Mac Catalyst의 현재 target/배포 산출물을 조사한다.
- [ ] 실제 선택되는 native asset과 Graphite backend 포함 여부, loader 경로, Skia source revision·build flags를 표로 만든다. Vulkan loader 존재와 Graphite 포함 여부를 구분한다.
- [ ] Android 최소 SDK 및 Apple GPU capability 정책을 확정하고 지원에서 제외되는 환경을 명시한다. 물리 장치 확보가 안 된 행은 `notVerified`로 남긴다.
- [ ] 동일 Material/Cupertino 장면·입력/resize trace, 이미지/문자/blur/shader 장면으로 플랫폼별 기존 경로의 기능·성능·메모리 기준선을 저장한다.

완료 조건: 각 target의 current/desired backend와 구현·빌드·실행·물리 증거 칸이 분리된 기준선이 있고, 미측정 행을 PASS로 표시하지 않는다.

### NG1 — Graphite native interop 입증 및 필요한 바인딩 보완

- [ ] 고정 버전 `IsBackendAvailable`, context/recorder 생성, raster image upload, 색상/텍스트/gradient/offscreen draw, Snap/Insert/Submit, async readback, 정상 해제를 최소 probe로 검증한다.
- [ ] Vulkan의 enabled features/extensions, image format/usage, sample count, allocator, procedure callback 보존을 Skia native shim과 대조한다.
- [ ] **외부 texture render → 외부 copy/present → 다음 frame 재사용**을 Vulkan validation layer로 검증한다. layout/queue-family 조회·갱신, 최종 상태 지정, semaphore/완료 통지가 부족하면 필요한 API와 소유권을 목록화한다.
- [ ] 공개 바인딩만으로 불충분하면 같은 pinned Skia revision에 C ABI·managed binding을 추가하고 필요한 RID 자산을 재빌드/패키징한다. managed/native ABI 일치, symbol export, native 의존성, license/provenance, 재생성 절차를 기록한다.
- [ ] Metal 외부 drawable texture wrapping, Graphite submit → present 순서 및 completion 후 해제를 입증한다. capability 선검사·nil drawable·실패한 Snap/Submit도 처리한다.
- [ ] resize/context 재생성/미완료 작업 중 종료를 반복하여 callback 잔류·use-after-free·leak가 없는지 확인한다.

완료 조건: **offscreen readback뿐 아니라 외부 출력 자원과의 왕복 수명·동기화가 검증**된다. Vulkan interop 실패면 NG3/NG5/NG6 제품 전환은 BLOCKED다. 단순 `Sync=true`/매 프레임 device-idle 우회는 실험 기준선으로만 기록하고 제품 완료로 승격하지 않는다.

### NG2 — 공유 Graphite session·렌더러 연결

- [ ] `Doroti.Skia.Rendering`/`Doroti.Skia.RuntimeEffects`에 필요한 최소 session 계약을 설계하고 플랫폼별 device/present 소유권을 문서화한다.
- [ ] recorder 기반 surface, image provider/cache, runtime effects, offscreen capture, picture cache, 비동기 readback을 연결한다.
- [ ] native Graphite backend ID와 generation-aware cache invalidation, device-loss·종료·다중 view 분리를 구현한다.
- [ ] 기존 paint completion/terminal ledger와 실제 native present 완료를 연결하고 submit 성공을 present 성공으로 바꾸지 않는다.
- [ ] 공통 변경에 대해 기존 WebGPU/WebGL과 기존 네이티브 기준선 회귀를 확인한다.

완료 조건: 동일 scene을 재사용하며 GPU resource 소유권·캐시·효과·완료 진단이 backend별로 정확하다. 임시 CPU raster/전체 화면 readback-copy로 통과시키지 않는다.

### NG3 — Windows Graphite/Vulkan

- [ ] `WindowsManagedVulkanPresenter(.Prepared).cs`의 Ganesh context/target을 Graphite context/recorder/backend texture로 전환한다.
- [ ] 현재 hardware GPU 선택, LUID 일치, Vulkan image → dedicated shared D3D11 texture copy → Windows Presentation slot 반환 계약을 유지한다. 필요시 NG1 bridge를 이 구간에 연결한다.
- [ ] retained backing·exact child HWND·resize generation·Acrylic·DPI·mixed-monitor 정책을 유지하고 stale 작업의 제출/표시를 차단한다.
- [ ] `DorotiWindowsAppSdkRunner`, native `vulkan_composition.cpp`, ABI·capability 진단에서 Ganesh 가정을 수정한다. C#↔Windows host ABI에 불필요한 GPU 포인터 노출을 추가하지 않는다.
- [ ] Windows MAUI용 surface adapter를 연결한다. 재사용할 Vulkan core와 WinUI/MAUI 창·dispatcher/present 경계를 분리하여 App SDK 전용 객체를 MAUI에 강제로 주입하지 않는다.
- [ ] opaque/Acrylic, resize 8방향, minimize/restore, GPU reset/loss, start/close, Korean IME/UIA, GPU preference 및 self-contained 실행을 비교한다.

완료 조건: App SDK와 MAUI의 기본 제품 경로에서 `Graphite/Vulkan`이 실제 진단·출력으로 확인되고 기존 Windows 합성 동작이 유지된다. GPU 모델별/물리 resize PASS는 해당 장치에서 확인한 범위로 한정한다.

### NG4 — macOS·iOS 및 Mac Catalyst Graphite/Metal

- [ ] `DorotiMacOSMetalView`의 MTKView/device/queue/drawable/transaction presentation을 보존하며 Graphite session을 연결한다.
- [ ] iOS `SKMetalView`의 Ganesh 자동 소유를 대체하는 Doroti Metal view/handler를 구현하고 `MauiSkiaPaintContext`의 context identity를 Graphite와 연결한다.
- [ ] `DorotiMacCatalystSkglViewHandler`의 live-resize·pointer 정책도 새 Metal view에 이식한다.
- [ ] UIKit/AppKit main-thread 의무, drawable 보존, command completion, background/foreground, orientation/safe area, nil drawable, detach/reattach를 처리한다.
- [ ] 한국어 IME, selection/focus, VoiceOver, pointer/touch, blur/gradient/texture shader, screenshot/readback을 검증한다.

완료 조건: native macOS, iOS 실기기 및 Catalyst에서 각각 Graphite/Metal 출력·수명 검증이 있다. simulator만 실행했으면 iOS 전체 PASS로 기록하지 않는다.

### NG5 — Linux Qt Graphite/Vulkan

- [ ] Qt QWindow 기반 Vulkan presenter를 구현한다. device/queue/swapchain과 Graphite submit의 owner를 하나의 계약으로 정한다.
- [ ] `QtSkiaSurface`, `QtNativeV2`, native template의 GL FBO/proc-address descriptor를 Vulkan에 맞게 교체하고 호환성이 깨지는 ABI는 새 버전으로 관리한다.
- [ ] CMake/target manifest/템플릿 및 앱 생성·재생성 경로에 Vulkan loader·헤더·QPA 요구사항을 반영한다.
- [ ] Wayland configure/ack·fractional scaling·surface 수명과 X11/XWayland의 창 surface 연결을 각각 처리한다. 기존 clipboard, IME, semantics, decoration/backdrop 코드를 보존한다.
- [ ] swapchain acquire/out-of-date/suboptimal/surface-lost/device-lost, graphics/present queue 분리, resize와 present semaphore 반환을 검증한다.
- [ ] Wayland·실제 X11·XWayland를 구분하고 물리 GPU에서 first-content, resize, input/IME/Orca 및 재시작을 확인한다.

완료 조건: 현재 Linux Qt 제품 대상이 GL FBO 없이 Graphite/Vulkan로 출력된다. VMware/소프트웨어 renderer 결과는 물리 Linux 하드웨어 승인으로 재사용하지 않는다.

### NG6 — Android Graphite/Vulkan

- [ ] `IMauiSkiaSurface`를 구현하는 Android Vulkan surface와 MAUI handler를 추가하고 `MauiSkglSurface` 선택을 교체한다.
- [ ] `SurfaceView`/`ANativeWindow` 우선안으로 생성·크기 변경·파괴 이벤트, native window retain/release 및 Vulkan surface generation을 연결한다. Activity를 새 프레임워크로 갈아엎지 않는다.
- [ ] Android native interop 프로젝트/템플릿·패키지에 필요한 Vulkan surface bridge를 추가하고 실제 loader/ABI/배포 자산을 확인한다.
- [ ] 최소 SDK·Vulkan capability·manifest·runner doctor를 함께 수정한다. 미지원 장치는 native crash나 검은 화면 대신 명시적 오류로 처리한다.
- [ ] rotation·IME inset·safe area·foreground/background·잠금/복귀·surface 재생성·memory pressure 중 in-flight 이미지와 swapchain을 안전하게 반환한다.
- [ ] Choreographer/기존 frame request와 제출을 연결한다. SurfaceView의 z-order·투명도·native overlay/touch 전달이 기존 기능을 깨뜨리지 않는지 확인한다.
- [ ] arm64 실기기에서 가능한 Adreno/Mali 범위를 나누고 x64 emulator는 별도 행으로 검증한다. touch/한국어 IME/TalkBack, texture upload·blur·shader, 긴 스크롤과 열/전력 상태를 측정한다.

완료 조건: 실기기에서 Graphite/Vulkan 출력·lifecycle·입력/접근성 회귀가 확인되고 지원 기기 조건이 배포 설정과 일치한다.

### NG7 — 플랫폼별 승격·이전 경로 정리·문서/배포 동기화

- [ ] 아래 검증 표를 해당 플랫폼별로 채우고 실패·미측정 항목을 그대로 남긴다. 기능/수명 필수 gate 실패 시 그 플랫폼 기본값 전환은 보류한다.
- [ ] 채택할 target의 기본값·명시 선택 옵션·capability/backend 진단과 target manifest를 동시에 전환한다.
- [ ] 승격한 플랫폼의 native Ganesh/GL 제품 선택지와 불필요한 dependency를 정리한다. `SkiaSharp.Vulkan.Silk.NET` 등은 실제 잔존 참조와 WebGL·진단 산출물을 확인한 뒤 제거한다.
- [ ] `.props/.csproj`, native build, package/reuse hash, 템플릿, 생성 앱, SDK/run/doctor 경로를 맞추고 clean publish 및 설치 환경 실행으로 확인한다.
- [ ] README 양 언어판, 관련 ADR의 현재 결정, Linux ABI/graphicsBackend manifest, `work3.md`의 현재 상태 설명을 실제 코드와 맞춘다. 과거 실행 기록은 유지한다.
- [ ] 각 플랫폼에 구현 commit·artifact hash·실행 환경·raw 결과·잔여 `notVerified`·롤백 방법을 남긴다.

완료 조건: 요청한 5개 OS의 활성 제품 경로가 목표 구성을 충족하고, Windows MAUI·Mac Catalyst 유지 범위와 실기기 미검증 범위가 명시된다. 일부 플랫폼만 끝났다면 전체 완료 대신 PARTIAL이다. 성능 향상 주장은 별도 측정 PASS가 있을 때만 한다.

## 6. 검증 및 수용 기준

저장소 `.github/copilot-instructions.md`에 따라 **각 테스트 실행은 외부 20분 timeout**을 적용한다. 실패 원문·첫 실패를 남기고 환경 오류와 제품 오류를 구분한다. 구현과 무관한 일회성 검증 스크립트를 대량 추가하지 않으며 기존 contract/validation 프로젝트를 우선 확장한다. 오래된 문서의 실행 명령은 실제 파일 존재부터 확인한다.

| 검증 축 | 필요한 증거 | 현재 상태 |
| --- | --- | --- |
| 패키지/API | RID별 native backend 포함·ABI·loader·실제 context 생성 | Windows stock/bridge·Linux stock/ABI 2 diagnostic context PASS; 전체 RID 제품 패키징 notVerified |
| GPU interop | Vulkan validation/Metal validation, texture 상태·제출 순서·자원 반환 | Windows 두 GPU external-copy PASS; M1 Metal texture/창 probe 완료·반환 PASS; Linux llvmpipe 3개 context/108 external frame·모의 loss PASS; Windows platform present·실제 device loss 미검증 |
| 공통 기능 | FCR-7 Material, retained rendering, image pipeline, RuntimeEffects, paragraph/text·clip·alpha·blur | FCR-7·Runtime shader 계약 PASS; macOS Graphite Material first-content/replay PASS; 전체 효과·image export 미검증 |
| 프레임 계약 | generation·DPR·resize·replay/superseded·중복 terminal 없음·bounded queue | notVerified |
| native lifecycle | 장치/표면 재생성, 시작/종료, background, 최소화/복귀, 다중 view | Linux probe context 재생성·모의 loss PASS; 제품 전체 lifecycle notVerified |
| 플랫폼 기능 | Windows Acrylic/UIA, Linux Wayland/X11/Orca, Apple VoiceOver, Android TalkBack·IME | notVerified |
| 배포 | canonical `Doroti/eng/doroti.ps1` 경로의 build/publish/run, 템플릿 재생성, 서명/실기기 설치 | macOS build/run/publish·pkg 및 Linux canonical build/run/publish·분리 경로 실행 PASS; clean 설치·전체 RID 미검증 |
| Web 회귀 | 기존 WebGPU 기본/명시 WebGL, 공통 surface/effects/cache 변경 영향 | Web canonical build·Chrome/SVGA3D WebGL 6개·WebGPU unavailable/no-fallback 1개 계약 PASS; WebGPU 실제 출력 미검증 |
| 성능·메모리 | 같은 target/work·장면·DPR·GPU/전력 조건의 기존 Ganesh 대비 측정 | notVerified |
| 물리 확인 | 사람이 수행한 스크롤·창 resize·DPI 경계·터치/IME, 실제 화면 관찰 | notVerified |

성능 측정안은 NG0에서 실행 전에 고정한다. 초기 기준은 동일 장치에서 warm-up 60초 후 180초 측정, 기준/후보 교차 3쌍이며 각 실행은 20분 timeout 안에 둔다. cold startup/첫 shader compile과 warm steady-state를 분리한다. 측정 항목은 first-content, CPU record/submit, GPU frame time, input-to-platform-terminal p50/p95/p99, long frame, allocations/GC, process·GPU memory peak, Android 열/전력 상태다.

승격의 초기 회귀 기준은 주요 지연 지표·메모리 peak가 같은 조건의 기준선보다 10% 이상 악화하지 않는 것이다. baseline 변동 폭이 10%보다 크면 결과를 inconclusive로 남기고 측정 조건부터 정리한다. 이 수치는 기존 제품 합격선이나 성능 향상 보장이 아닌 **이번 계획의 제안 gate**다. 사용자 관찰상 기존보다 나쁘면 자동 측정 PASS만으로 승격하지 않는다. 정량 검증 없이 아키텍처만 우선 채택하는 별도 결정이 생기면 그 결정과 `performance=notVerified`를 함께 남긴다.

## 7. 의존 순서와 최종 완료 정의

실행 순서: **NG0 → NG1 → NG2 → NG3 → NG4 → NG5 → NG6 → NG7**. 플랫폼 구현의 선행 조건은 공통 NG1/NG2이며, NG7 승격은 플랫폼별로 적용할 수 있다. NG4는 Mac/Xcode 및 Apple 장치, NG5는 Linux 실행 환경, NG6는 Android Vulkan 장치가 필요하다. 장치 미확보가 문서/공통 코드 진행까지 막지는 않지만 실행 gate를 대체하지도 않는다.

최종적으로 다음을 모두 만족해야 전체 전환을 완료로 기록한다.

- [ ] Windows·Linux·Android의 활성 제품 기본 렌더러가 Graphite/Vulkan이다.
- [ ] macOS·iOS의 활성 제품 기본 렌더러가 Graphite/Metal이다.
- [ ] Windows MAUI와 Mac Catalyst도 정해진 유지 범위를 충족한다.
- [ ] 필요한 native interop API와 패키지 빌드가 재현 가능하며 device/surface 수명이 검증된다.
- [ ] 기능·프레임·배포 필수 gate가 통과하고, 성능·물리·장치 범위의 PASS/FAIL/notVerified가 구분돼 있다.
- [ ] Web 기존 경로 회귀가 없고 Framework 작업 대상·동작 의미가 유지된다.
- [ ] 이전 제품 경로 정리와 README/ADR/manifest/템플릿 동기화가 끝났으며 과거 실패 증거가 보존된다.

현재 완료 증거는 **기존 Windows probe/bridge 검증, 공통 Metal session과 macOS AppKit 명시적 Graphite 후보, M1 texture·창 probe·Material 제품 출력·pkg 생성/분리 경로 실행, Runtime shader·FCR-7 계약과 Web 빌드**다. Linux ABI 2 진단 bridge·3개 context/108 external frame·모의 loss와 SVGA3D Qt GPU/분리 publish 재검증도 추가됐다. NG1 전체 수명·제품 Vulkan ABI/배포 gate, 나머지 제품 host 전환·기본값 승격·이전 경로 정리는 미완료다. iOS/Android 실기기·WebGPU 실제 browser 출력·성능·물리 확인은 `notVerified`이며, 전체 전환 완료로 기록하지 않는다.

# 네이티브 Graphite 전환·Android 스크롤 개선 요약

보관일: 2026-09-10. 원본: 루트 `work.md`(작성일 2026-09-09, 후속 기록 0.1~0.8 포함).
원본 SHA-256: `FF5AC1C8B78DFC91C41B961E902C2E3EA23781C593D29072D4CAAA40CBAC69F0`.

사용자 요청에 따라 실행 결과·결정·잔여 작업을 이 문서로 요약하고 루트 `work.md`를 삭제했다. 보관 작업에서 제품 코드나 기기 상태를 변경하거나 검증을 다시 실행하지 않았다.

## 1. 최종 결정과 상태

**Graphite 기본 구성과 요청된 Windows/Android 구현·검증은 수행했지만, 최초 NG0~NG7의 전 플랫폼 수용 조건 전체는 PARTIAL이다.** 원본 앞부분과 마지막 표의 “후보”, “기본값 전환 미완료”, “Android 제품 미검증”은 이전 단계의 상태다. 기본 구성에는 후속 0.6절, Android 실기기·성능에는 0.7~0.8절을 우선 적용한다.

| 대상 | 채택한 기본 구성 | 검증 범위·잔여 한계 |
| --- | --- | --- |
| Windows App SDK | Graphite/Vulkan → 기존 Windows Presentation·Composition | NVIDIA/AMD 제품 출력·Acrylic·reset/모의 loss·준비 프레임·실제 앱·publish 검사. 전체 물리 화면/DPI/IME/UIA 검증은 별도 |
| Windows MAUI | Graphite/Vulkan → 같은 LUID의 D3D12 공유 backing → CompositionDrawingSurface | Gallery/resize와 두 GPU의 공유 texture 픽셀 검증. 전체 기능/기기 범위는 미완료 |
| Android MAUI | Graphite/Vulkan → SurfaceView/ANativeWindow·swapchain | API 24 이상, 하드웨어 Vulkan 1.1. Galaxy S25 Release/AOT 설치·스크롤·복귀·키보드 확인. 다른 기기·TalkBack 전체·실제 loss 등 미검증 |
| Linux Qt | Graphite/Vulkan → QWindow·QVulkanInstance·관리되는 WSI | adapter·native build/pack 구성. 후속 제품 실행은 `skippedByUser`; 이전 Linux probe/GL 실행 증거와 구분 |
| macOS AppKit | Graphite/Metal → MTKView/CAMetalLayer | 이전 M1 제품 후보 first-content/replay·패키지 실행 기록 보존. 후속 제품 검증은 `skippedByUser` |
| iOS / Mac Catalyst | Graphite/Metal → MAUI 아래 UIKit MTKView handler | 기존 host·입력 경계 유지. 후속 Apple 빌드/실기기 검증은 `skippedByUser` |
| Web | `worker-direct-webgpu` 기본, `worker-direct-webgl` 명시 선택 | 네이티브 전환과 구분. 기록된 WebGPU 6개·WebGL 6개 browser 계약 PASS |

고정 SkiaSharp는 `4.154.0-preview.1.26454.9`, 검토한 repository revision은 `143a933a753dbfeca1909524b2c06c546c5c3e20`이다. 동일 Skia build 안의 native bridge와 managed binding을 사용한다. 서로 다른 Skia 인스턴스 사이에 포인터를 전달하지 않는다. 명시적 비교용 `=0` 경로는 남아 있으며 자동 Ganesh/GL/software fallback은 하지 않는다.

### 사용자 결정의 범위

- **NVIDIA receipt 지연 허용:** matching present-id/tag/display-instance 통계가 기존 최대 249.355ms, Graphite 최대 252.540ms에 도착했다. 사용자 승인으로 NVIDIA(vendor `0x10de`) 대기 한도는 1,000ms, 다른 GPU는 50ms다. 응답 누락·GPU 오류는 계속 실패이며 기존 50ms 실패와 `PreparedReceiptsOver50Milliseconds` 기록을 유지한다.
- **Graphite 기본값 채택:** “Graphite를 기본으로”, 이어 Android·Linux·iOS·Windows MAUI 구성 요청에 따라 adapter와 기본값을 연결했다. 이는 정량 성능·물리 화면·전 기기 검증 완료와 별개다.
- **검증 생략:** Linux·Apple 후속 제품 실행의 `skippedByUser`는 유지한다. 9월 9일 USB 단절 뒤 생략한 최종 Android 재설치 기록은 과거 상태이며, 9월 10일의 새 요청으로 최종 APK 설치·실행·성능 측정을 수행했다.

## 2. 전환 구현·검증 이력

| 단계별 실행 | 보존할 핵심 결과 |
| --- | --- |
| 최초 Windows NG0/NG1 | RTX 4060 Laptop·Radeon 780M Vulkan/D3D11 capability, Graphite context/recorder·draw/upload/readback. GPU별 외부 texture 36프레임 재사용, synchronization validation 0. 당시에는 제품 기본값을 승격하지 않음 |
| Apple 후속 | M1/macOS 26.6.2/Xcode 26.6/.NET 10.0.400. Metal 외부 texture 36프레임, 픽셀·async readback, 44개 command buffer 완료, resize 20회·숨김/복귀/종료. 명시적 제품 후보와 별도 경로 `.pkg` 실행 PASS |
| Linux 후속 | Ubuntu 26.04/VMware SVGA II/Qt 6.10.2. 기존 SVGA3D OpenGL GPU 가속은 유효. Vulkan은 CPU llvmpipe 진단만 가능했으며 ABI 2·3개 context·108프레임·validation 0·stock/rebuilt hash 일치. 이를 물리 Vulkan 제품 승인으로 사용하지 않음 |
| Windows·Galaxy 후속 | 공통 Vulkan session, ABI 3 외부 device-loss 전달, win-x64/android-arm64 자산. 두 GPU × 기존/후보 × opaque/Acrylic/모의 loss 12개 제품 host 실행 PASS. 실제 Material 앱 4개 실행 및 Web browser 12개 계약 PASS |
| Galaxy native probe | SM-S931N/API 36/Adreno 830. 3개 context·36개 persistent texture 프레임의 픽셀/GPU 완료/모의 외부 loss/해제. 당시 작은 native probe였고 앱·WSI 검증과 구분 |
| 기본 구성 후속 | Windows 두 GPU 기본 실행, Windows MAUI Gallery/resize, D3D12 texture 72프레임 픽셀, runtime shader/FCR-7/Qt 관리 계약, Windows publish/별도 실행·패키지 내용 PASS. Android arm64/x64 AAR 및 Release/AOT 패키징 연결 |

공통 `SkiaGraphiteSession`은 단일 owner/thread·generation, recorder와 image provider/cache, 예산·bounded frames, offscreen/runtime shader, 비동기 capture/readback, GPU 완료 후 state 갱신과 자원 반환을 담당한다. window·surface·swapchain/drawable·present·플랫폼 동기화는 host가 소유한다. native worker 종료는 join과 GPU drain 뒤에만 소유권을 넘긴다.

## 3. Android 스크롤 개선

### 1차: 중복 갱신 예약과 queue-idle 제거

Choreographer에서 이미 받은 프레임을 `PostOnAnimation`으로 다시 미루던 경로를 현재 vsync에서 그리도록 바꿨다. Vulkan의 매 프레임 `QueueWaitIdle`은 이미지별 presentation semaphore 재사용으로 대체했다. Galaxy 기존 설치본 비교 중앙값은 **24.995→16.664ms**, p95는 **49.974→33.327ms**였다. 최대 간격은 개선되지 않았고 사용자도 낮은 FPS가 남았다고 보고했다.

### 2차: GPU 완료 대기와 움직이는 shader cache 개선

- **주 병목:** copy fence 완료까지 UI 스레드에서 평균 약 7.749ms 대기. 독립 backing/target/command/fence/acquire semaphore를 갖는 **최대 2프레임** 파이프라인, timeout 0 acquire, GPU 측 semaphore 대기와 비차단 fence polling으로 변경했다. 마지막 프레임은 별도 idle polling으로 반환하며 재그리기를 유발하지 않는다. 실제 GPU 완료 전 슬롯을 재사용하지 않고 기존 5초 완료 timeout을 유지한다.
- **캐시 낭비:** 이동으로 무효화된 shader output을 매 프레임 새로 만들었다. transform/크기/content generation이 3프레임 안정될 때 재승격한다. 그동안 full child texture와 shader 좌표를 유지해 직접 그리며 후보 수는 64개로 제한한다.
- **접근성 조사:** native measure/layout은 스크롤 중 각각 6회, 총 약 18.552ms였다. 초기 생성 약 61ms spike는 있었으나 지속적인 낮은 FPS의 주 병목 근거는 아니었다. 접근성 기능·서비스·발행/액션 경계를 유지했다.
- **범위:** 비차단 파이프라인은 Android에 적용한다. idle 완료 callback이 없는 Qt의 동기 반환 계약과 Windows D3D12 import 방식은 유지한다.

최종 비교는 동일 Galaxy·동일 Gallery에서 1,500ms 왕복 swipe를 수행한 기록이다. SurfaceFlinger의 128프레임 ring 손실을 막기 위해 약 200ms마다 연속 수집하고 초기 두 swipe를 제외했다. 기준 APK도 직전 코드에 동일 계측을 추가한 빌드다.

| 지표 | 기준 | 최종 |
| --- | ---: | ---: |
| 표시 간격 표본 | 384 | 899 |
| 중앙값 | 16.664ms | **8.331ms** |
| p95 | 33.327ms | **8.333ms** |
| 최대 | 124.963ms | 91.625ms |
| 12.5ms 초과 간격 | 350/384 | 21/899 |
| 전체 Render CPU 평균 | 18.668ms | 6.055ms |
| shader용 GPU 표면 생성 | 323회 | **23회** |
| 접근성 active control / 적용 / 속성 쓰기 | 26 / 7 / 365 | **26 / 7 / 365** |

최종 Android Release/AOT build·서명 APK 설치·기기 `base.apk` hash 일치, **12개 회귀 검사**(FCR-7, FCR-5, runtime shader, MAUI semantics, GPU compositing 7종, D3D12 interop) PASS가 기록됐다. 새 moving 검사는 소수점 이동·클리핑·반투명·색 변환·안정 후 캐시 재사용을 20프레임 전 픽셀 비교했다. Windows 두 GPU D3D12 72프레임 검사는 Android WSI validation-layer 검사와 구분한다.

최종 실기기 smoke는 중첩 스크롤·shader/blur·키보드 열기/닫기와 실제 Surface 파괴·재생성 **6회**(스크롤 중 홈 이동 4회 포함)를 확인했다. generation별 제출/반환은 **340/340, 1/1, 111/111, 87/87, 84/84, 83/83**, 잔류 frame 0. frame receipt 961, replay 36, failed/dropped/superseded/software fallback 0, 마지막 입력/표시 sequence 535/535였다. 앱 receipt는 물리 scan-out이나 vsync miss 수가 아니다.

## 4. 남은 단계와 수용 조건

최초 의존 순서는 **NG0 → NG1 → NG2 → NG3 → NG4 → NG5 → NG6 → NG7**이다. 다음은 원본의 미체크 전체 조건을 최신 후속 결과와 합쳐 남긴 것으로, 새 구현 지시가 아니다.

| 단계 | 구현/검증 성과 이후에도 남은 범위 |
| --- | --- |
| NG0 기준선 | 전체 target/RID의 실제 loader/hash·지원 정책·동일 장면 성능/메모리 기준선 완성 |
| NG1 interop | 전체 RID ABI·배포, 물리 Vulkan/Metal의 외부 자원 왕복·실제 loss/실패·미완료 종료·다중 view 검증 |
| NG2 공통 session | 전 플랫폼 image export·capture·cache·AOT/trim, 전체 효과와 공통 변경의 Web/native 회귀 범위 |
| NG3 Windows | 전체 resize 방향·DPR·mixed-monitor·한국어 IME/UIA·물리 출력·실제 loss 및 성능/메모리 검증 |
| NG4 Apple | iOS 실기기/Catalyst와 최종 AppKit의 전체 lifecycle·orientation/safe area·nil drawable·IME/VoiceOver·배포 검증 |
| NG5 Linux | 물리 Vulkan GPU의 Wayland/X11/XWayland·fractional scaling·WSI 오류/queue·IME/Orca·제품 배포 실행 |
| NG6 Android | Adreno 외 Mali 등 기기 범위, x64 emulator 별도 증거, rotation/잠금/memory pressure, 한글 IME/TalkBack 전체, validation layer·실제 loss·장시간 열/전력 |
| NG7 승격/정리 | 기본값 채택과 별개로 전 target clean publish/설치·템플릿/manifest/ADR 일치·이전 제품 경로 및 불필요한 dependency 최종 정리 |

Framework rebuild/layout·입력·focus/IME·semantics·retained scene 의미를 보존한다. recorded/submitted/GPU-complete/present-requested/platform-terminal/physical scan-out을 구분하고 generation/DPR/viewport·중복 terminal·bounded queue 계약을 유지한다. native 종료는 신규 frame 차단 → GPU/플랫폼 소비 확인 → cache/surface/recorder/context → device/window 순서다. 오류 은폐용 자동 fallback과 명시적 비교/복구는 구분한다.

원래 전 플랫폼 성능 수용안은 warm-up 60초 후 180초 측정, 기준/후보 교차 3쌍, 지연·메모리 10% 회귀 확인이었다. baseline 변동이 10%보다 크면 inconclusive로 둔다. 이번 짧은 Galaxy 스크롤 비교를 그 전체 gate 완료로 간주하지 않는다. 초기 GPU shader 준비·접근성 생성과 간헐적 긴 프레임, 장시간 열/전력·메모리, 모든 화면/기기의 일정한 120fps 및 실제 device loss는 남아 있다. 테스트에는 저장소 규칙의 외부 **20분 timeout**을 적용한다.

## 5. 실패·정정 보존

- NVIDIA의 과거 50ms receipt 실패는 승인된 1,000ms 정책으로 덮어쓰지 않는다. 일반 output mode 1의 `DwmFlush`와 prepared mode 2의 matching 표시 통계를 구분한다.
- 초기 native library 누락·restore/workload/artifact 등록·동시 rebuild·native include `interface` 매크로·Windows Store `python3` alias·managed 접근/종료 thread·Web static import 링크 오류와 수정 후 성공은 기존 9월 9일 결과에 남아 있다.
- 9월 9일 Android 최종 배포의 USB 단절/장치 없음과 당시 사용자 검증 생략은 9월 10일 성공과 별도다.
- Android 1차 측정의 이전 SurfaceFlinger buffer 포함, `/proc/uptime`과 timestamp clock 불일치, stale evidence cache를 최종 결과에서 제외했다. 2차는 128프레임 ring 손실을 막는 연속 수집을 사용했다.
- 최초 pipeline build `CS8602`는 수정 후 재빌드했다. smoke 두 번의 “6회 파괴 기대, 2회 관측” 실패는 처음 의심한 intent 문제만으로 설명되지 않았다. native log에서 빠른 복귀가 SurfaceDestroyed를 막았음을 확인하고 실제 파괴·반환 이벤트를 기다린 검사로 정정했다.

## 6. 근거 문서·실행 안내

- [최초 Windows 보고서](../../Doroti/docs/validation/native-graphite-2026-09-09.md), [실행 JSON](../26-09-09/native-graphite-execution.json)
- [Apple 보고서](../../Doroti/docs/validation/native-graphite-apple-2026-09-09.md), [실행 JSON](../26-09-09/native-graphite-apple-execution.json)
- [Linux 보고서](../../Doroti/docs/validation/native-graphite-linux-2026-09-09.md), [실행 JSON](../26-09-09/native-graphite-linux-execution.json)
- [Windows·Galaxy 보고서](../../Doroti/docs/validation/native-graphite-windows-android-2026-09-09.md), [실행 JSON](../26-09-09/native-graphite-windows-android-execution.json)
- [NVIDIA 지연 승인 JSON](../26-09-09/native-graphite-nvidia-latency-acceptance.json)
- [기본 구성 보고서](../../Doroti/docs/validation/native-graphite-defaults-2026-09-09.md), [실행 JSON](../26-09-09/native-graphite-default-execution.json)
- [Android 1차 개선](../../Doroti/docs/validation/android-scroll-2026-09-10.md), [Android 최종 개선](../../Doroti/docs/validation/android-scroll-followup-2026-09-10.md)
- [공통 session 계약](../../Doroti/docs/architecture/native-graphite-session.md), [native build·복구 안내](../../Doroti/native/graphite/README.md)

**보관 시점의 자료 상태:** 위 문서와 9월 9일 실행 JSON의 존재를 확인했다. 반면 기존 보고서가 가리키는 `history/26-09-10/android-scroll/` 및 `android-scroll-followup/` 원시 자료 폴더는 현재 작업 디렉터리에 없다. Android 수치는 남아 있는 `work.md`와 검증 보고서의 기존 결과를 요약한 것이며, 이번 보관 작업에서 원시 로그를 재검증하거나 없는 파일을 재생성하지 않았다. 두 보고서의 측정 스크립트 경로도 사용 전 존재 여부를 확인해야 한다.

재실행 시 canonical 진입점은 `Doroti/eng/doroti.ps1`이며 예를 들어 Android는 다음과 같다. 이는 보관을 위해 이번에 실행한 명령이 아니다.

```powershell
pwsh -File Doroti/eng/doroti.ps1 build -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform android -Rid android-arm64
pwsh -File Doroti/eng/doroti.ps1 run -App DorotiTestbedApp/DorotiTestbedApp.csproj -Platform android -Rid android-arm64 -Device R3CY30KZA4B -NoBuild
```

비교/복구는 각 host의 명시 옵션과 마지막 검증된 산출물을 사용한다. 현재는 Graphite가 기본이므로 과거 후보 단계의 “활성화 환경 변수를 제거하면 이전 renderer로 복귀”라는 설명을 현재 복구 방법으로 그대로 적용하지 않는다. 상세 조건은 위 기본 구성·native README를 따른다.

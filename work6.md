# 네이티브 C 경로 단일화 후속 작업계획

작성일: **2026-10-02** · 선행 작업: [work5 — 네이티브 공통 C 경로](work5.md)

상태: **C 단일화·동적 texture 예산 구현 및 실행 가능한 자동 검증 완료, 전체 인수 PARTIAL.** 제품의 A/B 실행 옵션·분기는 제거했다. [실행 결과와 source/payload JSON](works/results/2026-10-02-native-frame-c-only.md)을 기준으로 확인한다. Apple 실행 SKIPPED와 실제 표시·물리 입력·10분 사용의 미검증 범위, 선행 [기존 결과](works/results/2026-10-02-native-frame-pipeline.md)는 유지한다.

**목표:** 모든 범위 내 네이티브 호스트에서 A·B 실행 모드와 이를 선택하는 분기를 제거하고, framework 준비 → 실제 장면 조회 → GPU admission → 제출 → 마지막 소비자 완료의 C 구조를 유일한 프레임 정책으로 사용한다. C의 기본 적용은 이미 완료됐으므로, 이번 작업은 선택 옵션·분기·도구·문서를 정리하고 변경 전 C와의 회귀를 확인하는 후속 작업이다.

작업 순서: **기존 C 기준 확보 → 공통 정책 단일화 → 플랫폼 호스트 정리 → 검사·수집 도구 전환 → 문서·패키지 반영 → 회귀 및 결과 기록**.

## 1. 가능 여부와 유지할 계약

**가능하다.** [공통 정책](Doroti/src/Doroti.Skia.Rendering/NativeFramePipeline.cs)의 기본값은 이미 C이며, UIKit/Catalyst·AppKit·Android·Windows App SDK·Windows MAUI·Linux Qt가 이 정책을 사용한다. 착수 시 남아 있던 A/B의 `Pipeline`·`Asynchronous` 조합, 설정 해석과 호스트 조건문은 이번 구현에서 제거했다.

유지할 C 계약:

- 일반 frame pulse에서 유효한 immutable viewport/context 정보를 게시하고 GPU slot/drawable/acquire 앞에서 framework를 준비한다. resize/rotation 전환은 기존 정확한 generation 계약에 맞춘 별도 순서를 유지한다.
- GPU full에서도 일반 framework 준비 기회를 유지하고, framework 요청 및 최신 pending scene은 각각 하나로 합친다. GPU 재시도는 이미 준비된 장면을 사용한다.
- fresh shader-only 장면은 지원 backend에서 논리 GPU 프레임 **최대 2개**와 비동기 표시를 사용한다. shader 모드뿐 아니라 Off·일반 UI도 같은 정책을 따른다.
- native/shield·pending native·replay·resize/rotation은 진행 중인 작업을 drain하고 직렬 admission 및 필요한 표시 동기화를 사용한다. 이는 **C 내부 상태 전환**으로 유지한다.
- recording·snapshot·drawable·native lease·공유 texture는 마지막 GPU 소비자의 완료 또는 확인된 context loss까지 보존한다. terminal·제출 성공·present receipt·timeout은 자원 재사용 허가가 아니다.
- 기존 backend/device/queue와 Windows resize ACK·Qt consumer retirement·Android surface retirement hold를 유지한다. 논리 프레임 상한 2를 front texture·DXGI back buffer·Qt bank 수에 그대로 적용하지 않는다.
- 공개 VariableBlur와 Sample2의 **fastGaussian + adaptiveResolution=true + resolutionScale=0.25**, 커널·sigma·DPR·캡처 범위·화질은 유지한다.

범위는 work5와 같은 네이티브 호스트다. Web은 제외한다. Ganesh/OpenGL·embedded composition 등 두 프레임 수명을 지원하지 않는 기존 backend는 C 정책의 capability 제한에 따라 1개 admission을 유지한다. 이 backend의 폐기는 별도 범위이며, 실제 두 프레임 capability를 통과한 것으로 보고하지 않는다.

## 2. 착수 시 제거·정리 대상

| 영역 | 현재 코드와 사용처 | 후속 변경 |
| --- | --- | --- |
| 공통 정책 | [NativeFramePipeline.cs](Doroti/src/Doroti.Skia.Rendering/NativeFramePipeline.cs)의 public `NativeFrameLoopOptions`, `Resolve`, `FromSettings`, `Decide`·`PrepareAndDecide`의 mode bool | A/B를 만들 수 있는 옵션·인자를 제거하고 장면/슬롯/capability/전환 상태로 admission 결정 |
| iOS 호환 래퍼 | `IosFrameLoopOptions.cs`, `IosFrameAdmissionPolicy.cs` (이번 작업에서 제거), 테스트 프로젝트의 링크 | 현재 제품 참조를 확인한 뒤 불필요한 래퍼와 테스트 링크 제거 |
| UIKit / Catalyst | [DorotiUIKitGraphiteViewHandler.cs](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs)의 옵션 필드·준비·표시·diagnostics 분기 | C 준비 연결을 상시 사용하고 `SynchronizePresentation` 등 실제 admission 결과로 표시 결정 |
| AppKit | [DorotiMacOSMetalView.cs](Doroti/src/Doroti.Host.Maui/DorotiMacOSMetalView.cs)의 매 draw 옵션 조회 | mode 조회 제거, Graphite capability와 layout/native 상태로 결정 |
| Android | [DorotiAndroidVulkanViewHandler.cs](Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs)의 Intent/env 옵션과 준비 분기 | C 준비 연결 고정, 설정 검증과 surface/lifecycle 계약 보존 |
| Windows App SDK / MAUI | [DorotiWindowsAppSdkRunner.cs](Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs), [DorotiWindowsDxgiSurface.cs](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs), 각 presenter | 옵션 기반 admission 제거, producer/D3D12 consumer와 native/resize 경계 유지 |
| Linux Qt | [DorotiQtRunner.cs](Doroti/src/Doroti.Host.Qt/DorotiQtRunner.cs), Quick/Vulkan-window 및 app-owned native shim | mode 조회 제거, Quick 준비 token·publication·sampling retirement 유지 |
| 검사·수집 | [CPU 회귀](Doroti/tests/Doroti.Tests/ShaderFramePipelineRegression.cs), [GPU 회귀](Doroti/tests/Doroti.Tests/NativeFrameGpuRegression.cs), [공통 collector](Doroti/tests/native_frame_pipeline_collect.py), [iOS collector](Doroti/tests/variable_blur_device.py), [iOS lifecycle](Doroti/tests/ios_frame_loop_lifecycle_device.py) | A/B 실행 행렬 제거, C 기준 회귀·격리 검사로 전환; 삭제 파일을 source hash 대상으로 계속 읽지 않게 수정 |
| 제품 설명 | [공통 문서](Doroti/docs/native-frame-pipeline.md), [검사 README](Doroti/tests/README.md), [Sample2 README](samples/DorotiSampleApp2/README.md), [템플릿 README](Doroti/templates/Doroti.Templates/content/doroti-app/desktop/README.md) | 유일한 C 정책, 내부 직렬 전환, capability 제한 및 제거된 설정 안내 |

`A/B` 문자열 전체 삭제를 목표로 삼지 않는다. 블러 구현 비교·창 lifetime 정책처럼 다른 의미의 A/B, 과거 측정 자료, 마이그레이션 설명은 각각의 용도대로 보존한다.

## 3. 설정·API 전환 규칙

제품에 A/B 실행 가능성을 남기지 않으면서 오래된 실행 설정을 식별한다.

| 입력 | 전환 후 동작 |
| --- | --- |
| `DOROTI_NATIVE_FRAME_MODE` 미설정 또는 빈 값 | C 실행 |
| `DOROTI_NATIVE_FRAME_MODE=C` | C 실행. 기존 C 실행 명령의 호환 확인용으로만 허용 |
| `DOROTI_NATIVE_FRAME_MODE=A`, `B` 또는 그 밖의 값 | 시작 시 명확한 구성 오류. 제거된 A/B와 설정 수정 방법을 표시 |
| `DOROTI_VARIABLE_BLUR_SERIAL_FRAMES`, `DOROTI_VARIABLE_BLUR_PIPELINE`, `DOROTI_NATIVE_PRESENTATION`, `DOROTI_IOS_SHADER_PRESENTATION`의 비어 있지 않은 값 | 제거된 설정 오류. C를 지정했던 legacy 값도 삭제하도록 안내 |

- 공통의 작은 설정 검사만 남긴다. 설정을 `Pipeline`·`Asynchronous` bool로 변환하거나 A/B 구현을 호출하는 compatibility layer는 남기지 않는다.
- 환경 설정과 Android Intent extra를 모두 검사하며, 공통 `C` 지정과 충돌하는 legacy 설정도 숨기지 않는다. Android는 현행 Intent 우선 규칙과 실제 전달 방법을 반영해 검사한다.
- 설정 오류는 호스트 초기화 단계에서 확인하고 GPU 자원 생성 전에 전달한다. hot path의 매 프레임 환경변수 조회·반복 경고를 제거한다.
- public `NativeFrameLoopOptions` 생성자와 admission 시그니처 변경은 소스/API 호환 변경이다. 참조 소비자·샘플·패키지를 함께 갱신하고 저장소의 버전 정책에 맞춰 breaking change를 기록한다. A/B bool API를 유지한 채 내부에서 무시하는 구현은 사용하지 않는다.
- GPU API의 저수준 동기화 인자는 실제 resize/native/종료 수명에 필요할 수 있다. mode 선택만 하던 인자와 실제 동기화 인자를 구분해 후자 및 필요한 호출을 보존한다.

## 4. F0 — 기존 C 기준 확보와 참조 조사

- [x] 작업 시작 시 `rg`로 제품·샘플·템플릿·테스트·CLI·VS Code 실행 설정의 A/B selector, legacy 키, bool 옵션과 public API 참조를 다시 조사한다.
- [x] 변경 전 C 후보의 source/payload hash, runtime/SDK·backend·GPU·viewport/DPR·표시 구성을 기록한다. 현재 디스크에 없는 과거 raw 산출물을 남아 있다고 보고하지 않는다.
- [x] Windows 두 호스트의 변경 전 Release C payload를 보존하고 Off/Fast σ20·32/Adaptive σ20·32를 확보했다. native/lifecycle은 기존 증거와 새 후보 검사로 구분했다. 다른 플랫폼의 유효한 정량 before/after 성능 기준은 미확보다. 오래된 Linux binary는 consumer 계약이 달라 baseline으로 사용하지 않았다.
- [x] `work5`의 현재 PARTIAL·Apple SKIPPED·Windows MAUI native 입력 adapter 미완료·OpenGL Wayland 실패를 후속 결과의 시작 상태로 기록한다.

**완료 조건:** 삭제할 실행 참조와 보존할 C 내부 동기화/compatibility 경계, 변경 전 C 비교 후보를 식별했다.

## 5. F1 — 공통 C 정책 단일화

- [x] 공통 admission API에서 `pipeline`·`asynchronous` mode 인자를 제거한다. fresh 여부·GPU 상한·표시 동기화는 scene, pending, native, resize 및 backend capability로 계산한다.
- [x] 준비 API는 정상 pulse에서 C 순서를 사용한다. `prepareFramework=false`의 raster-only 재시도와 resize/rotation의 별도 준비 계약은 유지한다.
- [x] `NativeFrameLoopOptions` 및 불필요한 iOS 래퍼를 제거하고 §3의 초기 구성 검사로 대체한다. 테스트 `.csproj`의 직접 Compile 링크도 갱신한다.
- [x] 진단의 정책 표기는 항상 C로 하고, `GpuLimit`·capability·직렬 사유·실제 비동기 표시 선택을 별도 필드로 보고한다. 두 슬롯을 지원하지 않는 backend도 mode A/B로 다시 이름 붙이지 않는다.
- [x] terminal·generation·역순 완료·마지막 GPU 소비자 회수 규칙은 기존 공통 계약으로 유지한다.

**완료 조건:** 제품의 공통 API로 A/B 정책을 구성할 수 없으며, C의 정상/직렬 admission과 초기 설정 거절을 CPU 회귀로 검증한다.

## 6. F2 — 모든 네이티브 호스트 정리

- [x] UIKit/Catalyst의 mode 필드·A/B diagnostics·callback 준비 분기를 제거한다. native/rotation/resize transaction과 지속 복귀 연결은 유지한다.
- [x] AppKit의 옵션 조회 및 A/B 분기를 제거한다. `_inFlight` 상한·drawable 앞 준비·Ganesh capability 제한·native/layout 직렬 전환을 확인한다.
- [x] Android의 Intent/env mode 선택과 옵션 cache를 제거한다. Choreographer 준비, 비차단 acquire, completion retry, pause/resume·surface destroy/recreate hold를 확인한다.
- [x] Windows App SDK·MAUI에서 옵션과 이전 serial/async 비교용 분기를 제거한다. 두 private bank와 producer→D3D12 final consumer 수명, embedded composition 제한, exact resize/ACK 및 recovery를 유지한다.
- [x] Qt Quick·Vulkan-window·OpenGL에서 공통 C admission을 사용한다. Quick의 준비 token 재사용과 Qt sampling 완료, OpenGL의 실제 single-frame 제한 및 종료 drain을 보존한다.
- [x] native shim을 변경했다면 Sample2·Testbed·템플릿 복제본과 실제 ABI를 함께 갱신한다. managed 모드 삭제만으로 ABI 변경을 만들지 않는다.

**완료 조건:** 모든 범위 내 호스트에서 mode 옵션 없이 C 정책을 사용하며, A/B 실행 분기가 남지 않았다. 실제 장면·capability에 필요한 직렬 처리는 유지된다.

## 7. F3 — 검사와 수집 도구의 C 전환

- [x] `ShaderFramePipelineRegression`에서 A/B·legacy 해석 성공 행렬을 제거한다. 무설정/C 실행, 제거된 설정 거절, full queue 준비·coalescing·native 삽입·raster wake·상한 2·resize/context·terminal 검사를 유지/갱신한다.
- [x] `NativeFrameGpuRegression`의 픽셀 기준을 **C로 한 장면씩 제출하고 완료를 확인한 기준**과 **C로 두 미완료 recording을 유지한 결과**의 비교로 바꾼다. 두 경우 모두 제품 C 제출 방식을 사용하고 fixture의 drain 위치만 달리한다.
- [x] 기존 Off/Full/Adaptive/Fast/Fixed/Kawase·sigma 31조건과 red/green snapshot 격리·third frame 거절·역순 완료·consumer 전량 회수를 유지한다. 픽셀 허용 기준은 기존 backend 기준을 따른다.
- [x] 공통 및 iOS collector에서 `--policies A/B/C`, `--serial-frames`, mode를 바꾸는 presentation 인자와 legacy 환경변수 주입을 제거한다. 무설정/C 설정 확인과 제거된 설정의 negative probe를 독립 검사로 제공한다.
- [x] iOS lifecycle 도구를 무설정 C로 실행하도록 바꾸고 복귀 3회·각 5초 후 지속 frame 증가 검사를 유지한다.
- [x] 삭제·변경한 파일을 source/payload hash manifest와 테스트 로그·JSON schema에 반영한다. 과거 A/B/C 결과를 읽는 기능이 필요하면 read-only 호환으로 유지한다.
- [x] 성능 collector는 후보별 C 수집으로 바꾼다. 같은 환경의 **변경 전 C / 변경 후 C × 5조건 × 각각 3회 = 총 30회**를 기본 비교로 사용하고 실행 순서를 교차한다. 기준 후보가 없으면 after-only 결과로 기록한다.

변경 전후 비교는 서로 다른 payload의 회귀 검사다. 과거 같은 바이너리 A/C 성능 자료와 구분한다. 표시 계측 권한이 없으면 queue/admission 자료만 보고하고 표시 FPS·hardware overlap·입력 지연은 `notMeasured`로 유지한다. 단순 smoke만 수행한 결과에 30회 성능 PASS를 부여하지 않는다.

**완료 조건:** 현재 도구로 A/B를 실행할 수 없으며 C의 정확성·수명·회귀를 확인할 검사가 유지된다.

## 8. F4 — 샘플·템플릿·문서·패키지 반영

- [x] 공통 문서·테스트 README·Sample2 README·템플릿 README에서 활성 A/B 선택과 복귀 명령을 제거하고 C 정책·내부 직렬 전환·capability 제한을 설명한다.
- [x] CLI·VS Code·샘플 실행 설정 및 새 프로젝트 산출물에 legacy selector가 들어가지 않는지 확인한다. 새 프로젝트의 무설정 실행은 C다.
- [x] public API 변경과 설정 삭제 안내, 패키지 후보 identity를 migration/release 자료에 기록한다. 제거 전 버전으로의 패키지 복귀는 배포 절차로 다루며 제품 A/B 런타임 옵션으로 유지하지 않는다.
- [x] `work4.md`, `work5.md`의 과거 실행 이력과 `works/results`의 A/B/C JSON·픽셀·실패 증거를 보존한다. 기존 문서의 당시 옵션 유지 요구는 이 후속 구현 이후의 제품 요구로 사용하지 않는다.

**완료 조건:** 소스·샘플·새 템플릿·배포 패키지 설명이 C 단일 정책과 일치하고 설정/API 변경을 확인할 수 있다.

## 9. F5 — 회귀 검증과 결과 기록

모든 테스트 실행은 [.github 지침](.github/copilot-instructions.md)에 따라 **1,200초(20분) timeout**을 사용한다. [기존 검사 도구](Doroti/tests/README.md)와 [timeout wrapper](Doroti/eng/run-with-timeout.py)를 활용하며, 반복은 목적에 필요한 수로 제한한다.

| 검증 | 필요한 증거 | 판정 한계 |
| --- | --- | --- |
| 소스/API | 활성 A/B resolver·host 분기·collector 실행 옵션 부재, 구성 negative probe, 참조 빌드 | 과거 결과와 삭제 안내의 A/B 문자열은 허용 |
| CPU / 실제 GPU | 공통 계약, full queue 준비, 31조건 C 픽셀·2 recording 격리·third 거절·consumer drain | fixture로 플랫폼 표시·물리 입력을 대신하지 않음 |
| Windows App SDK / MAUI | 각 Sample2/Testbed의 관련 build·C smoke·native/resize·reset/recovery·종료 | MAUI 입력 adapter 미완료와 물리 DPI/IME는 별도 상태 |
| Android | 실제 runtime을 기록한 APK build와 가능한 환경의 설치·C/설정 거절·native·rotation·3회 지속 복귀·surface 재생성 | emulator 결과와 arm64 build를 physical Android 인수로 확대하지 않음 |
| Linux Qt | Quick build·무설정 C·native/input/20회 resize·consumer drain, 지원 대체 backend 별도 smoke·새 템플릿 실행 | software Vulkan·OpenGL Wayland 기존 실패·물리 IME/표시는 구분 |
| Apple | 모든 관련 소스 참조 갱신, 수행 가능한 정적 검사와 검사 도구 정합성 확인 | 기존 사용자 요청의 build/Metal/device **SKIPPED** 상태 유지. 과거 iOS 수치를 새 payload PASS로 소급하지 않음 |
| 성능 / 사용 | 확보 가능한 변경 전/후 C 비교, 실제 표시·input 지연·10분 사용의 별도 증거 | 기존 work5의 미측정·물리 인수 상태를 자동 통과시키지 않음 |

- [x] 관련 공통 검사와 변경한 호스트의 수행 가능한 검증을 통과한다. 설정 오류가 발생해도 GPU 자원과 callback이 누수되지 않는지 확인한다.
- [x] `git diff --check`, 현재 문서 링크와 native 복제본/ABI 검사, 패키지·템플릿 검사를 수행한다.
- [x] 결과를 `works/results/<실행일>-native-frame-c-only.md`와 플랫폼별 JSON에 기록한다. 이전/새 source 및 payload hash, 구성 거절 결과, 실제 capability·직렬 사유와 실패도 보존한다.
- [x] 보존할 최종 증거는 추적되는 결과/migration 경로에 남긴다. raw는 `temp/testing/native-frame-c-only/<platform>/<run>/`에 두되 삭제 후 파일 존재를 주장하지 않는다.
- [x] work5의 미완료 인수와 이번 작업의 새 실패·SKIPPED·notMeasured·notVerified를 구분하고 최종 상태를 갱신한다.

**구현 완료 조건:** 제품·샘플·템플릿·현재 수집 도구에 A/B 실행 선택과 구현 분기가 없고, C의 준비·bounded admission·내부 직렬 전환·마지막 소비자 수명이 유지된다. 제거된 설정을 명확히 거절하며, 수행 가능한 회귀 검사가 통과하고 API/설정 변경 및 결과를 제공한다.

**전체 인수 조건:** 플랫폼별 실제 GPU·native 합성·lifecycle·표시 성능·물리 입력·사용 인수가 각각 충족돼야 한다. C 단일화 구현 완료만으로 work5의 기존 전체 인수 PARTIAL을 해제하지 않는다.

## 10. F6 — 사용자 추가 요청: 동적 texture 예산

고정 128MiB guard를 사용하던 Qt Quick R/P allocator에 공통 [DynamicTextureBudget](Doroti/src/Doroti.Skia.Rendering/DynamicTextureBudget.cs)을 적용한다. 다른 backend/device/queue와 블러 화질은 유지한다.

- [x] 물리 화면 크기·DPR·raster segment 수·동시 프레임·published/retiring 이미지로 발생한 실제 Vulkan allocation requirement를 수요로 사용한다. 더 높은 고정 MiB 값으로 교체하지 않는다.
- [x] 지원하는 physical device의 `VK_EXT_memory_budget`으로 heap별 process budget/usage를 조회한다. 지원하지 않거나 유효한 보고가 없으면 heap/host capacity 추정으로 구분한다.
- [x] 필요한 수요에 여유를 더해 allowance를 늘리고, 실제 회수 후에 줄인다. heap/host capacity의 10%, driver가 보고한 남은 공간의 20%를 여유로 유지한다. 이것은 allocation 성공 보장이 아니다.
- [x] compatible device-local memory type 중 실제 headroom이 있는 heap을 선택한다. 모든 후보가 부족하면 필요한 bytes·owned bytes·limit·source를 표시하고 거절한다.
- [x] 이미 GPU/Qt가 사용하는 할당은 budget 축소나 압력으로 해제하지 않는다. producer 및 마지막 소비자 완료/확인된 context loss 규칙을 유지한다.
- [x] CPU에서 growth·pressure·실제 회수 뒤 shrink·recovery·overflow를 검증한다. 실제 GPU에서 128MiB 초과 두 미완료 bank, resize 축소 및 종료 전량 회수를 확인한다.
- [x] Qt 진단에 current/peak allowance, heap별 owned/limit/source/rejected request를 추가하고 정상 종료 후 상태를 보존한다.
- [x] 올바른 Desktop 구성의 Linux native/input/resize 검증 및 큰 extent의 동작을 확인한다. 이전 고정 예산 실패와 잘못된 검사용 빌드 구성은 raw에 보존한다.
- [x] 새 코드로 최종 후보를 다시 만들고 앞선 C 단일화 후보의 검증을 소급하지 않는다. [동적 예산 계약](Doroti/docs/dynamic-texture-budget.md)을 갱신한다.


## 11. 최종 실행 상태

- F0–F4: C 기본 payload 보존, 옵션/API·host 분기·검사·문서·migration 정리 완료. Windows 두 host의 교차 C smoke와 새 Windows/Linux package-only template 검증 PASS.
- F5: 공통 CPU 및 Radeon Vulkan 31조건, Windows native/lifetime, Android x64 APK·C/native/복귀, Linux Quick 및 지원 대체 backend 자동 검증 PASS. 실패와 빌드/도구 재시도는 결과에 보존했다.
- F6: dynamic demand/driver headroom·live allocation floor·retirement shrink 구현, CPU 및 실제 GPU 128MiB 초과 2-bank 검사, 올바른 Linux Desktop native/input/20 resize PASS.
- Windows 최종 후보는 `0.3.0-beta.rc.20261002.conly.dynamic`다. Linux는 별도의 새 isolated Qt package feed와 portable install을 검증했다. Apple·물리 GPU/입력/표시·30회 정량 성능·10분 사용·서명/clean OS는 전체 인수와 구분한다.

체크된 성능 collector 항목은 before/after C 수집 기능의 구현과 queue smoke를 뜻한다. 실제 표시 계측 없는 실행을 정량 성능 PASS로 처리하지 않았다. 기존 work5의 전체 인수 PARTIAL은 유지한다.

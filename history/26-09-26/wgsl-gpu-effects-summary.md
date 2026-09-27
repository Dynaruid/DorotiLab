# WGSL 기반 위젯 GPU 효과 작업 요약

- 작업 기록: 2026-09-26~27. 보관일: 2026-09-28. 사용자 지정 경로인 `history/26-09-26`에 보관한다.
- 대상: 루트 `work.md`의 WGSL GPU 효과 설계, G0~G9 필수 단계와 G10 후속 범위, 구현·검증 현황.
- 최종 기록 상태: **PARTIAL — Windows fragment·backdrop 경로 구현·실행 확인, G0~G9 전체 미완료**. G0~G4 Windows milestone도 전체 완료로 판정하지 않는다.
- 아래 PASS·notVerified·notMeasured는 기존 작업의 기록이다. 이번 보관에서는 제품 빌드·화면·장치 검증을 재실행하지 않았다. 원문의 제안 계약과 완료 조건 전체가 구현된 API를 뜻하지 않는다.

구현·재현 명령과 상세 증거는 [GPU 효과 검증 기록](../../Doroti/validation/gpu-effects/README.md), 컴파일러 사용법과 현재 ABI는 [WGSL 도구](../../tools/Doroti.Wgsl/README.md)를 참조한다. 09-26 전체 구현 요청으로 계획 전용 제한이 해제되었고, WindowsAppSdk·MAUI Windows는 실제 표시까지, 다른 플랫폼은 우선 코드 구현까지 진행한 범위다. 비Windows 실행은 `notVerified`로 남길 수 있지만 실행기 미구현을 완료로 간주하지 않는다.

## 목표와 범위

위젯 서브트리를 GPU 텍스처에 렌더링하고 WGSL에서 생성한 플랫폼 셰이더로 처리한 뒤 기존 Skia 장면에 합성한다. 레이아웃·기본 래스터화·최종 합성·호스트 presentation은 기존 경로를 유지한다.

- 자식 입력의 `GpuEffect`부터 시작하여 `GpuBackdropEffect`, compute, 다중 패스, storage, 이전 프레임 history로 확장한다.
- `.wgsl`과 `.effect.json`을 빌드 입력으로 받아 검증·reflection·C# 파라미터 생성·플랫폼 variant 패키징을 자동화한다.
- 필수 대상은 Windows/Android/Linux Vulkan, Apple Metal, WebGPU/Dawn 및 **WebGL2/Ganesh**다. WebGL2는 후속 선택 항목이 아니다.
- WebGL2는 fragment 다중 패스·ping-pong·texture history·배경 효과까지 필수이며 compute/storage 실행은 명시적 미지원이다. 필요한 fragment 대안은 효과 작성자가 제공한다.
- 제품 효과 경로에 GPU→CPU→GPU 픽셀 왕복을 넣지 않는다. GPU 내부 복사 비용은 별도 측정하며 이 조건만으로 zero-copy라고 부르지 않는다.
- Skia 렌더러 전체 교체, 새 창·swapchain, 일반 앱에 원시 GPU 핸들 공개, 모든 네이티브 PlatformView 자동 캡처는 제외한다. D3D12/HLSL/DXIL, 사용자 mesh/vertex, HDR 확장은 G10 선택 범위다.

## 보존할 설계와 실행 계약

| 구성 | 책임 및 현재 경계 |
| --- | --- |
| `GpuEffectProgram` / `GpuEffectParameters` | 불변 프로그램과 직렬화된 파라미터 스냅샷. 생성 ABI와 실제 byte 수 검사 |
| 위젯 → render object/layer → scene payload | child 또는 backdrop 입력, bounds, enabled, 파라미터를 실제 실행기에 연결 |
| 호스트 내부 backend / texture lease | 기존 device·queue·context에 귀속한 할당·pass·동기화·retirement. 정식 capability/범용 lease 계약은 잔여 |
| `GpuEffectController` | 명시적 갱신 또는 명시적으로 시작한 ticker. 정지·dispose 후 연속 repaint 종료, 공개 시간 계약은 Flutter `Duration` 유지 |
| App SDK / Runner SDK | 앱 공통 검증·타입 생성과 runner별 variant 선택·배포 분리. 현재 모든 선언 variant를 생성 C#에 포함하는 초기 통합 |

기본 실행 순서는 **child 캡처 → Skia 입력 segment 제출 → 같은 장치의 외부 fragment pass → 출력 상태 전환 → Skia 합성 → 최종 host fence 후 회수**다. 한 논리 프레임 안에 여러 recording/submission segment를 유지하고 중간 segment를 별도 presented frame으로 세지 않는다. `SKSurface.Snapshot()`에서 네이티브 핸들을 꺼낸다고 가정하지 않고 호스트 소유 이미지를 Skia에 wrap한다.

- 같은 queue에서도 memory barrier·image layout 전환을 생략하지 않는다. scheduled state와 GPU 완료 상태를 구별하고, 정상 프레임마다 device/queue idle wait를 넣지 않는다.
- 입력과 writable 출력은 분리한다. 중첩은 child 의존성부터 실행하며 정식 DAG는 순환을 진단하고 temporal history만 별도 edge로 허용하도록 설계한다.
- WebGPU는 기존 Worker의 Dawn device/queue, WebGL2는 기존 Ganesh와 동일 GL context·texture handle table을 사용한다. 별도 GPU device/canvas나 renderer 자동 전환을 만들지 않는다.
- WebGL2에서는 Skia 입력 flush 뒤 FBO pass를 실행하고 실제 GL binding/state를 복구한 뒤 Ganesh cache를 무효화한다. `ResetContext`만으로 실제 GL 상태 복구를 대체하지 않는다. context 복구 시 canvas endpoint 재바인딩 조건도 보존한다.
- capture는 transform·DPR·paint bounds를 반영한다. outsets/halo, clip·opacity·mask 전체 조합은 잔여다. 기본적으로 시각 효과가 layout·hit testing·semantics를 바꾸지 않는다.
- `enabled=false`는 추가 효과 pass 없이 child를 그린다. 미지원 host/profile은 명시적으로 실패하며 CPU/SkSL 또는 다른 renderer로 조용히 대체하지 않는다.
- 자원은 마지막 GPU 사용까지 유지한다. resize/DPR/device generation, 미제출 취소와 중간 제출 후 취소, dispose/device loss의 회수 경로를 구별한다. 취소된 프레임의 history를 다음 입력으로 승격하지 않는다.
- 입력 캐시는 child revision·bounds·transform/DPR·외부 texture revision·format·device generation, 출력 캐시는 program/parameter/history revision까지 포함하도록 설계했다. parameter-only 입력 재사용과 완전한 pool/cache는 미완료다.
- 선형 RGB·premultiplied alpha 처리와 backend별 layout adapter가 목표다. 현재 RGBA8 처리 전체가 선형 RGB 계약을 구현했다고 보지 않는다. native-view 입력은 캡처 capability를 별도로 확인해야 한다.

## WGSL ABI와 빌드 경로

현재 도구는 **Rust 1.95.0 / Naga 30.0.1 / Cargo.lock**을 고정한다. `validate`, `reflect`, `compile`, `generate`로 WGSL·entry·binding 검사, reflection, 언어 variant 및 C# serializer를 제공한다. 앱은 Rust runtime이나 별도 wgpu device를 로드하지 않는다.

| 기본 binding | 의미 |
| --- | --- |
| group 0 / binding 0 | 읽기 전용 `texture_2d<f32>` 입력 |
| group 0 / binding 1 | 입력 sampler |
| group 0 / binding 2 | 32바이트 엔진 frame uniform: input/output pixel size, logical size, time/deltaTime |
| group 1 / binding 0 | 생성된 사용자 parameter uniform |

기본 fragment는 엔진 fullscreen triangle과 UV 입력·색상 출력을 사용한다. 원문의 group 0 / binding 3 compute storage 출력은 확장 설계이며 현재 compute 구현 완료를 뜻하지 않는다. scalar/vector/matrix/array의 offset·padding은 Naga reflection에 따라 직렬화하고 backend mapping을 따로 검증한다. WGSL layout과 GLSL std140 등이 항상 같다고 가정하지 않는다. 좌상단 원점·UV/Y 방향 규칙은 backend 경계에서 일관되게 처리한다.

| 대상 | 산출물과 남는 처리 |
| --- | --- |
| Vulkan | SPIR-V 생성 후 실행 시 shader module/pipeline 생성·캐시 |
| Metal | MSL 생성. 현재 첫 사용 시 소스 컴파일하며 Apple SDK별 `.metallib` 빌드·패키징은 잔여 |
| WebGPU | WGSL 제공 후 브라우저에서 module/pipeline 컴파일 |
| WebGL2 | GLSL ES 3.00 및 engine vertex/binding metadata 제공 후 실제 GL compile/link |
| D3D12 | HLSL→DXIL 및 실행기는 G10 선택 범위 |

`DorotiGpuEffect` item을 App SDK가 수집하여 `CoreCompile` 전에 검증·C# 생성을 수행하는 초기 경로가 있다. `DorotiWgslTool`로 준비된 compiler를 지정하며 앱 빌드가 Cargo 설치나 네트워크 다운로드를 자동 실행하지 않는다. compiler 부재·빈 성공 출력은 오류로 처리한다.

정식 배포는 플랫폼 중립 앱의 공통 ABI와 backend/RID별 runner resource catalog를 분리해야 한다. compiler RID 배포, NuGet 소비, publish, source/variant ABI hash 로드 검사, 원자적 산출물 공개는 남아 있다. 증분 키에는 소스·definition·ABI·compiler·profile·옵션을 포함하고 삭제/rename 및 동시 runner 빌드에서 stale/중복 산출물을 막아야 한다. Web은 같은 `browser-wasm`에서도 WebGPU와 WebGL2 variant를 구별하며 RID만으로 renderer를 결정하지 않는다.

## G0~G10 단계별 보관 상태

| 단계 | 확보한 결과 | 남은 핵심 작업 |
| --- | --- | --- |
| G0 외부 pass | Windows 실제 GPU와 WindowsAppSdk/MAUI에서 Skia→Vulkan fragment→Skia 합성 PASS. Metal/Dawn/WebGL2 연결 코드 추가 | 비Windows 제품 host의 동일 device/context, frame 완료·회수·최종 표시 실행 증거 |
| G1 계약·ABI | 불변 program/parameters, fragment binding, scalar/vector/matrix/array serializer와 ABI 오류 검사 | 정식 capability/texture lease/graph schema, linear RGB/outsets, backend별 layout 검증 완성 |
| G2 도구·SDK | 고정 compiler와 SPIR-V/MSL/WGSL/GLSL 생성. App SDK 생성·증분·삭제/rename PASS | compiler RID 배포, runner catalog, NuGet/publish, 로드 시 ABI hash 검증 |
| G3 위젯·장면 | `GpuEffect`→layer→scene→실행기, enabled, transformed paint bounds/DPR 캡처 | parameter-only 입력 재사용, 외부 texture revision, outsets·중첩·retained 수명 및 필요한 transport 계약 |
| G4 Windows | 두 host의 공통 Vulkan 실행·표시·3가지 resize·toggle PASS. pipeline 생성 1/재사용 7 | DPR 전환·스크롤·애니메이션·device loss와 G1/G2 잔여 조건 |
| G5 고급 패스 | 중첩 fragment 두 pass identity 픽셀·취소 회수 PASS | 정식 DAG, compute/storage/history, WebGL2 다중 pass·history |
| G6 비Windows native | Android/Linux 공통 실행기 등록, Apple Metal fragment 실행기·host 등록. iOS reference API 컴파일 PASS | `.metallib` 패키징, AppKit/Catalyst/iOS/Android/Linux 실제 실행·재생성·수명 검증 `notVerified` |
| G7 Web | WebGPU/Dawn·WebGL2 Worker fragment 실행기와 fence retirement, Web host 빌드 PASS. Edge 독립 GL probe PASS | 제품 Worker 최종 표시·context/device loss `notVerified`, 다중 pass/history/compute, 브라우저별 검증 |
| G8 backdrop·API | `GpuBackdropEffect`, 두 Windows host의 backdrop 표시/resize/toggle PASS. controller·시간 snapshot 계약 8건 PASS | 현재 레이어·선행 draw 의미의 복합 중첩, native input capability, 비동기 loader/준비·취소·실패 계약 |
| G9 비용·배포 | 메모리 budget·완료 후 회수, compiler/SDK/Windows 증거·문서 | pool/cache·warmup/hot reload, 배포, 종료/device loss, 성능 `notMeasured` |
| G10 선택 범위 | 기본 G0~G9 완료 조건 밖 | D3D12 실행기, 범용 buffer producer, 사용자 mesh/vertex, HDR·추가 storage format |

G7-A WebGPU와 G7-B WebGL2는 모두 필수다. G0~G9의 기능·수명·필수 플랫폼 검증이 충족되어야 전체 완료이며, Windows 수직 경로 성공이나 언어 번역 성공으로 대체하지 않는다.

## 보존한 검증 결과와 한계

| 기존 검증 | 결과 및 해석 범위 |
| --- | --- |
| Rust compiler | 5개 계약 PASS: profile/determinism, 복합 layout, binding/entry 거절, compute/profile 거절, frame ABI |
| WGSL SPIR-V native probe | 8 GPU draw, 중첩 2-pass identity와 반투명 입력 exact pixel PASS. pipeline 생성 1/재사용 7, 취소 3회, live bytes 0 복귀, peak 65,632 bytes |
| 사용자/엔진 uniform | multiplier 0.5, 32바이트 frame ABI, time 0.5/delta 0.25, 논리 32×24/물리 64×48 픽셀 PASS. tolerance 1/255 |
| WindowsAppSdk·MAUI Windows | Release 빌드 경고/오류 0. 실제 화면 RGB→BGR, host별 3가지 synthetic resize, toggle off, 후속 텍스트 표시 PASS |
| Windows backdrop | 두 host의 선행 레이어 배경 색상 변환·3가지 resize·toggle off PASS |
| Controller | 불변 byte 복사, idle, 명시적/idempotent start, 시간 snapshot, stop/manual update/dispose 등 8개 PASS |
| Edge 독립 WebGL2 | Windows Edge/ANGLE AMD Radeon 780M D3D11, 3,072픽셀, tolerance 1/255, 복합 UBO·GL 상태 복구·retirement PASS. 제품 Ganesh Worker 실행 증거는 아님 |
| Web·Metal 컴파일 | Web C#/TypeScript 빌드 PASS. iOS 26.5.10315 reference assembly에 대한 Apple API 컴파일 PASS. native MSL/Metal 실행 `notVerified` |
| SDK 독립 소비자 | clean/no-op/edit/rename/removal, stale API 거절, 128바이트 serializer golden, 빈 compiler 성공 거절 등 8개 PASS |
| 기존 회귀 | SkSL backdrop/image filter 독립 native 픽셀 159개 및 native texture 계약 25개 PASS |
| 미검증·미측정 | 물리 drag·혼합 monitor DPR·animation cadence·접근성, 비Windows 제품 실행 `notVerified`. CPU/GPU 시간·first-use·SkSL 비교·장시간 메모리 `notMeasured` |

효과 실행기의 CPU 픽셀 왕복·device/queue-idle 부재는 소스 감사 결과다. 일반 이미지 업로드·uniform 쓰기와 진단 readback은 별도이며, 하드웨어 profiler 비용 측정으로 확대하지 않는다. probe의 peak/live allocation은 앱 전체 메모리나 장시간 안정성 수치가 아니다. Windows 화면 검사는 실제 screen pixel을 확인했지만 synthetic 입력/resize를 물리 사용자 조작 검증으로 바꾸어 표기하지 않는다.

## 후속 작업과 자료 보존

후속 구현은 정식 ABI/capability·배포 계약, cache/outsets·복합 합성, compute/storage/DAG/history, 플랫폼별 실제 실행·복구, 비용 측정을 단계별로 이어간다. 플랫폼 실행 gate에는 macOS AppKit·Mac Catalyst, iOS device/simulator 분리, Android pause/resume·surface 재생성, Linux resize/teardown, WebGPU 및 Chrome 계열·Firefox·Safari/WebKit WebGL2를 포함한다.

검증은 독립 CPU 수식 또는 native 기준 픽셀, 실제 외부 pass 및 표시, GPU 완료 뒤 자원 회수를 함께 확인한다. identity는 exact, 부동소수 효과는 사전 정의 tolerance를 사용한다. 동일 오류가 있는 compiler 출력을 양쪽 기준으로 삼지 않는다. timing 미지원 환경에서 CPU 시간을 GPU 시간으로 대신 표기하지 않는다.

- 빌드·테스트는 `python Doroti/validation/run-with-timeout.py <command>`의 1,200초 process-tree 제한을 사용하고 공유 `obj` 빌드는 직렬화한다. 반복 frame/resize 검증은 보통 30회 이내로 제한한다.
- 주요 구현 위치: `tools/Doroti.Wgsl/`, App/Runner SDK, Hosting/Ui/Framework, Skia.Rendering/Vulkan, Apple host, Web Worker. 독립 계약·compiler fixture와 상세 기록은 `Doroti/validation/gpu-effects/`에 있다.
- Testbed 효과 Evidence/Fixture 및 `verify-windows.py`는 2026-09-27 제거되었다. 관련 sample mode·실행 명령은 역사적 기록이며 현재 그대로 재현 가능하다고 보장하지 않는다.
- 원시 로그·캡처는 삭제 가능한 `Doroti/artifacts/gpu-effects/`에 기록되었던 자료다. 보관 문서는 해당 파일의 현재 존재를 보장하지 않으며 추적되는 검증 README가 결과와 제약을 보존한다.

원문의 참고 기준은 Naga 변환·검증, WGSL 메모리 layout, WebGPU shader module, WebGL2/GLSL ES 3.00, Apple Metal library, 선택 범위의 DXC다. 구체적인 지원 여부는 고정 toolchain·생성 metadata·플랫폼 실행 증거로 구별한다.

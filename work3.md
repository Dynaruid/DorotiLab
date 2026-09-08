# SkiaSharp 4.154 전환·WebGPU 추가 작업계획

작성·정리: 2026-09-08.

**현재 범위:** 사용자가 병렬 레이아웃을 실행한 뒤 이득이 작다고 판단하여
해당 기능의 제거를 요청했다. 순수 treemap 계산 엔진, 별도 실험 화면, Material
샘플의 병렬 패널, 선택 옵션과 전용 검증을 제거한다. T0b/T1–T7 병렬 레이아웃
계획과 J0 조합 검증은 종료하며 후속 작업 목록에서 제외한다.

기존 측정·최초 실패·실행 결과는 [통합 실행 보고서](history/26-09-08/work3-execution.md),
[병렬 실행 기록](history/26-09-08/work3-parallel-execution.json),
[결합 실행 기록](history/26-09-08/work3-execution.md)에 보존한다.
과거 후보의 실행 URL과 PASS는 당시 산출물의 결과이며 현재 제품 기능을 뜻하지 않는다.

## 1. 현재 상태와 남은 작업

- U0–U3: SkiaSharp `4.154.0-preview.1.26454.9` 전환과 가용 플랫폼 회귀 완료.
  OS별 물리 입력·실기기·정량 성능의 미검증 범위는 기존 보고서를 따른다.
- G0–G4: WebGPU 추가 미완료. 첫 제품 출력 뒤 정상 종료·장치 손실 gate가
  실패해 후보를 보존하고 제품 실행은 WebGL S2로 원복했다.
- 병렬 레이아웃: 사용자 판단에 따라 제거. 기존 계산 비중 0.3% 미만의 측정과
  이후 실험의 결과를 보존하며 성능 개선으로 판정하지 않는다.

후속 순서는 현재 S2 보존 → WebGPU 수명 문제 해결 → 기능 회귀 → 성능·채택
판정이다. 패키지 전환과 WebGPU의 효과를 구분하며 WebGPU 기본값 전환은 포함하지 않는다.

## 2. 유지하는 runtime·소유권 기준

사용자가 체감상 더 낫다고 선택한 S1은 메인에서 단일 threaded .NET runtime을
초기화하고 같은 runtime의 JSWebWorker에서 framework/layout/Skia direct 렌더링을
수행하는 구조다. S2는 이 구조에 SkiaSharp 4.154 전환을 적용한 WebGL 기준선이다.
`WasmEnableThreads=true`, `runtimeLocation="main"`, `worker-direct-webgl` 기본값,
전용 MessagePort, owner SynchronizationContext와 `closed`→`disposed` 종료 계약을
유지한다. DOM/input/IME/semantics는 메인, UI 트리 변경과 layout/raster는 렌더
Worker owner가 담당한다. 별도 병렬 레이아웃 계산 스레드는 사용하지 않는다.
[ADR-003](Doroti/docs/adr/ADR-003-web-main-runtime-render-worker.md)을 따른다.

기존 trimmed Release의 first content·shared heap·resize·스크롤·선택 상태·
테마/열 복귀·종료·잘못된 protocol 거부 결과는
[메인 runtime 전환 보고서](history/26-09-08/wasm-main-runtime-render-worker.md)에 있다.
최초 Worker-root 부팅 FAIL은 [최초 기록](history/26-09-08/wasm-threads-bootstrap.md)에
보존한다. 이 결과를 병렬 기능 제거 후 새 실행의 검증으로 재사용하지 않는다.

S1 이전 HAMT + indexed와 미채택 C2/C3/C4 실험은
[보관된 work.md](history/26-09-08/work.original.md) 9.12–9.13절을 따른다.
AOT·가상화 확대는 이 작업 범위에 포함하지 않는다.

## 3. 회귀·채택 기준

기능 수용은 geometry·State·anchor·hit-test·semantics와 owner/종료 계약을 보존해야
한다. 오래된 화면, owner blocking, 처리되지 않은 오류와 상태 소실이 남으면
승격을 보류한다. 성능은 같은 workload 3쌍에서 전체 지연·cold 진입·메모리와
비대상 회귀를 측정한다. GPU 일부만 개선되면 `componentOnly`, 개선이 확인되지
않으면 `performanceUnproven`이다. 작은 표본의 p95를 안정적 통계로 주장하지 않는다.

비대상 p95 10% 초과 악화, live/transient memory 10% 초과 증가, 유효 trace 누락,
대상 작업 불일치는 승격 보류 조건이다. 기존 30% 구조 개선과
16.7/33.3/50/100ms 목표는 별도로 유지한다. 자동 검사·사용자 체감·물리 기기
수용을 구분하며 미검증 범위는 `notVerified`로 남긴다.

## 4. SkiaSharp 4.154.0 Preview 1 전환 계획

### 4.1 확인된 기반과 미검증 범위

전환 버전은 정확히 **`4.154.0-preview.1.26454.9`**로 고정한다. 2026-09-08
검토에서 릴리스 태그 소스와 실제 `SkiaSharp`·`SkiaSharp.NativeAssets.WebAssembly`
NuGet을 확인했다. 앞선 “Graphite 바인딩을 직접 추가해야 한다”는 판단은 이 프리뷰에
적용되지 않는다. 열린 이슈의 상태로 태그의 실제 구현을 부정하지 않는다.

| 확인 항목 | 확인 결과 | 아직 증명하지 않은 것 |
| --- | --- | --- |
| 관리 API | 배포 DLL에 `SKGraphiteContext`, `SKGraphiteRecorder`, `SKGraphiteDawnBackendContext`, `CreateDawn`, `IsBackendAvailable` 존재 | Doroti에서의 호출·trim·ABI 호환성 |
| WASM 배포물 | `3.1.56/mt` 및 `mt,simd/libSkiaSharp.a`, `emdawnwebgpu` JS/C++ 포함. `mt,simd`에서 Dawn 구현과 Graphite 진입 심볼 확인 | 현재 SDK 최종 링크와 Worker 안의 실제 장치 생성 |
| 패키지 targets | net9 이상 Dawn 연결·WebGPU export 및 threads/SIMD별 archive 선택 설정 존재 | Doroti 자체 interop/link flags와 함께 평가된 최종 결과 |
| 공식 브라우저 테스트 소스 | Graphite/Dawn 도형 렌더링·비동기 readback 구현 존재 | 해당 upstream 테스트의 실행 PASS, Doroti visible canvas·shared-runtime Worker 수용 |

소스/패키지 기반으로는 **공식 패키지를 이용한 구현이 첫 경로**다. NuGet cache,
생성된 dotnet JS 또는 SkiaSharp 소스를 수정하는 방식을 선행하지 않는다. 호환성
문제가 실제로 확인되면 최소 재현과 원인을 기록하고, 별도 fork가 필요한지는 그때
판단한다. 이 표는 최초 계획 당시의 확인 범위다. 후속 실행 상태는 아래 U/G
표와 통합 실행 보고서를 따르며 성능·물리 기기의 미검증 항목은 유지한다.

### 4.2 전환 범위와 단계

| 단계 | 실행 내용 | 종료 조건 | 상태 |
| --- | --- | --- | --- |
| U0 목록·기준선 고정 | current HEAD/dirty, S1 manifest/원본 산출물 보존; 모든 직접/전이 Skia·HarfBuzz 참조, SDK 기본값, manifest, lock, 배포 native asset 목록 작성 | 영향받는 host/TFM/RID와 정확한 버전 대응표·산출물 hash 고정; 각 대상 NuGet 가용성 확인 | PASS: 19개 NuGet·native/hash 목록, S1 보존 |
| U1 패키지·소스 정합성 | 중앙 버전/SDK 기본값 갱신, 정상 restore로 lock 재생성, 필요한 공통 API 호환성 수정 | 구버전 managed/native 혼합 0, dependency downgrade 0, 실제 native/관리 버전 진단 일치 | PASS: 4.154 전환·격리 restore, Android CS1705 수정 |
| U2 기존 WebGL 검증 | net10.0 threaded trimmed Release publish, 실제 선택 archive·링크·boot assets 검사, S1과 동일 입력 회귀 | WebGL first content/shared heap/owner/resize/scroll/상태/종료 PASS, runtime 오류 0; 다운로드·부팅·메모리 변화 기록 | PASS: S2 고정 및 최종 WebGL 7-case 회귀; 정량 성능은 미입증 |
| U3 타 host·패키징 판정 | 영향 host의 restore/build 및 실행 가능한 first-content·이미지/텍스트 검사, 생성 앱 restore/build/pack 경로 확인 | 플랫폼별 결과와 한계 기록; 필수 사용 host의 FAIL 해결 전 전체 전환 완료 금지; Web gate 통과 시 범위를 명시해 S2 고정 | 가용 build/restore·생성/pack/CLI PASS; 플랫폼별 물리 notVerified 기록 |

구체적인 갱신 대상:

- `Doroti/Directory.Packages.props`의 SkiaSharp 계열 10개 항목: 기본 패키지,
  NativeAssets.Win32/WebAssembly/Linux, HarfBuzz, Views.Blazor/Views/Views.Maui.Controls,
  Direct3D.Vortice, Vulkan.Silk.NET. 각 패키지의 목표 버전 존재를 U0에서 확인한다.
- `Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.props`의 `SkiaSharpVersion` 기본값,
  `Doroti/src/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json`,
  `Doroti/src/Doroti.Host.Maui/MauiFrameworkHost.cs`의 진단 버전 및 현재 버전을
  표현하는 release metadata/validation 기대값. 하드코딩 진단은 실제 assembly/native
  값과 맞는지 검증한다. 과거 history·원본 FAIL·과거 버전 설명은 치환하지 않는다.
- Testbed 및 제품/템플릿이 사용하는 `packages*.lock.json`과 전이 native assets.
  HarfBuzzSharp는 SkiaSharp와 버전 체계가 다르므로 `4.154...`로 일괄 치환하지 않고
  목표 nuspec의 요구 버전과 실제 resolution을 따른다. Android/iOS/macOS 등의
  전이 native 패키지도 기록한다. locked restore 정책을 꺼서 불일치를 숨기지 않는다.
- Web publish·정적 asset 무효화·서비스 워커/브라우저 캐시 정책을 확인해 구버전
  WebCIL/DLL과 신버전 native WASM이 섞이지 않게 한다. 기존 `DorotiSkiaInterop.js`
  연결 플래그를 보존하고 Dawn JS/C++ 및 `_wgpuCreateInstance`/texture release export가
  누락·중복되지 않는지 evaluated properties/items와 최종 link response로 확인한다.

U3 대상은 Web, Windows App SDK, MAUI Windows/Android/iOS/macOS, Linux/Qt, CLI 등
**U0에서 실제 Skia 의존성이 확인된 host**다. 각 host의 기존 presenter를 유지하며
Graphite 전환을 일괄 적용하지 않는다. 실행 환경이 없는 플랫폼은 `notVerified`로
남기고 restore/build 성공, 실제 first content, IME/접근성/물리 기기 수용을 구분한다.
Web S2 고정은 다른 플랫폼의 미검증 항목을 PASS로 바꾸지 않는다.

U 단계의 목적은 버전 전환과 기존 기능 유지다. 성능 향상은 필수 전제가 아니며
S1↔S2에서 부팅/프레임/메모리 회귀를 측정한다. 3절의 10% 초과 악화 보류 기준을
함께 사용하되 작은 표본을 안정적 p95로 주장하지 않는다. 필수 기능 실패 또는
설명되지 않은 회귀가 남으면 제품 버전·lock·native asset·관련 호환성 변경을
일관되게 S1으로 원복하고 실패 패치와 결과를 보존한다. WebGPU 실패만으로 통과한
패키지 전환까지 원복하지 않는다.

## 5. `worker-direct-webgpu` 경로 추가 계획

### 5.1 소유권·선택·출력 계약

S2와 같은 단일 main runtime, shared heap, JS-affine 렌더 Worker를 사용한다.
DOM/input/IME/semantics는 메인, framework/layout 및 Graphite 장치·context·recorder·
texture는 렌더 Worker owner에 둔다. GPUDevice/Queue/Texture JS 객체나 네이티브
핸들을 계산 스레드로 전달하지 않는다. shared WASM memory가 JS 객체의 공유를
뜻하지 않으며, emdawnwebgpu의 장치 등록과 네이티브 호출이 같은 owner에서 동작하는지
G0에서 증명한다. Graphite의 병렬 recording 기능은 이번 초기 구현 범위에서 제외한다.

- 설정/URL에 `worker-direct-webgpu`를 명시적으로 추가한다. 기존 WebGL 기본값과
  `auto`의 현재 선택 의미를 유지한다. requested/selected renderer, 실제 backend,
  패키지/native 버전, owner/thread, device/context generation을 진단에 남긴다.
- secure context, `navigator.gpu`, adapter/device 확보, 필요한 features/limits와
  `SKGraphiteContext.IsBackendAvailable(...)`로 Dawn backend 가용성을
  검사한다. 정확한 enum/API 표기는 G0에서 컴파일로 확정한다. 지원 안 됨·장치 생성
  실패는 구체적 오류로 종료하며 WebGL/CanvasKit/Canvas2D로 자동 우회하지 않는다.
  사용자가 WebGL을 선택해 새 세션을 시작하는 것은 별도 명시적 선택이다.
- OffscreenCanvas의 `webgpu` context를 configure하고 현재 출력 texture를 받아
  Graphite backend texture와 surface로 감싼다. texture 획득·제출은 브라우저의
  프레임 수명에 맞추고 이전 프레임의 current texture를 영구 재사용하지 않는다.
  색 형식·alpha·색 공간·surface origin을 WebGL의 bottom-left/RGBA 가정과 분리한다.
- 기존 CSS logical size/물리 backing size, DPR, ResizeEpoch, 최신 generation만
  표시하는 계약을 지킨다. WebGL framebuffer capacity/GL clear 방식을 그대로
  복사하지 않고 WebGPU에서 exact visible 영역·resize·빈 여백 처리를 검증한다.
  공식 테스트의 CPU readback 경로를 정상 화면 출력 경로로 사용하지 않는다.

### 5.2 구현 단계와 선행 gate

| 단계 | 실행 내용 | 종료 조건 | 상태 |
| --- | --- | --- | --- |
| G0 Worker 최소 실증 | S2의 threaded trimmed 환경에서 native Dawn 가용성, 장치/instance/queue 등록, surface, 도형·텍스트·이미지 출력, resize·정상 종료 | 실제 렌더 Worker에서 first pixels와 shared-runtime/owner 확인, async 제출·resource 정리, 오류 0 | PASS: 같은 shared runtime owner, 도형·텍스트·이미지·resize·정상 종료 smoke |
| G1 host 연결 | loader/types/strict protocol/diagnostics와 모드 선택; WebGPU surface와 프레임 제출 구현 | 명시 모드의 visible canvas 출력, 최신 generation만 반영, unsupported·device lost·종료 경로 정의 | 첫 제품 출력/resize PASS; G3 FAIL에 따라 제품 원복 |
| G2 공통 렌더러 대응 | Graphite 이미지 업로드, picture 캐시, runtime effects/image filters, snapshot/readback 지원 | 텍스트·이미지·클립·블렌드·그림자·shader/effect 기능과 리소스 소유권 보존 | 후보 구현 보존·원복; 전체 effects/readback corpus 미완료 |
| G3 기능·수명 회귀 | 아래 corpus를 S2/G에서 비교, fractional DPR·입력·장치 손실·취소·shutdown 검사 | 누락/오래된 화면/상태 소실/owner 위반/미처리 GPU 오류 0; 화면 차이 원인별 판정 | FAIL: 정상 종료 mapAsync Aborted, device loss disposed 미확인 |
| G4 성능·채택 기록 | 같은 버전/직렬 layout/입력의 S2↔G, cold/resize/scroll·GPU/CPU 비용 구분 | 실험 모드 기능 수용과 성능 판정 분리; 기본 WebGL 유지, 미검증 기기 명시 | 미채택·performanceUnproven; 필수 G3 실패로 3쌍 성능 미실행 |

G0가 실패하면 장치 등록·JS owner·ABI·native link의 최소 원인을 먼저 분리한다.
main 전용 또는 독립 runtime에서만 되는 smoke를 현재 topology PASS로 바꾸지 않는다.
threads=false, runtime 복제, 메인으로 raster 이동으로 우회하지 않는다. 원인이
해결되지 않으면 G1 이후 승격을 보류하고 검증된 S2를 유지한다.

### 5.3 제출·이미지·캐시·종료

프레임 제출은 `Recorder.Snap()` → `Context.InsertRecording()` 결과 검사 →
`Context.Submit()` 순서다. 기존 `GRContext.Flush()` 호출의 단순 이름 교체로
처리하지 않는다. browser Dawn의 `Submit(Sync=true)` 금지를 지키며 owner event
loop를 막는 Wait/Result/동기 GPU 완료 대기를 넣지 않는다. 제출 완료와 물리 표시
완료는 별도 진단이며 GPU queue 완료 신호를 scan-out/FPS 증거로 쓰지 않는다.

- recorder의 image provider와 texture image 변환/캐시를 연결해 raster SKImage·
  glyph atlas·이미지가 누락되지 않게 한다. context/recorder generation과 font/image
  수명에 맞춰 캐시를 무효화하며 매 프레임 중복 업로드를 계측한다.
- `Doroti.Skia.Rendering/SkiaSceneRenderer.cs`의 `canvas.Context` 검사와
  `SKSurface.Create(context, ...)`에 의존하는 picture GPU 캐시에 Graphite 경로를
  추가한다. 기존 fractional phase·raster extent·승격 예산·사용 순서 eviction을
  보존한다. 캐시가 조용히 꺼진 상태를 기능·성능 동등으로 보고하지 않는다.
- `Doroti.Skia.RuntimeEffects`, 이미지 필터와 Web Skia capabilities의 GPU 가정,
  `Picture.toImage`/`Image.toByteData` 등 snapshot/readback을 함께 점검한다.
  public 동기 계약이 필요한 기능은 구현 경계를 먼저 확정하고 임의 비동기 변경이나
  빈 이미지 반환으로 통과시키지 않는다. 미지원 기능은 기능 gate 미완료로 남긴다.
- frame/texture/recording 소유권과 release callback을 명시하고 in-flight 작업을
  정리한 뒤 recorder/context를 해제한다. upstream 테스트가 context를 process
  수명으로 유지하는 것을 Doroti의 정상 종료 구현으로 복사하지 않는다.
  device loss 시 신규 프레임을 중단하고 pending frame을 정확히 한 번 종료한다.
  손실 장치에서 무기한 완료 대기하지 않으며 오류·취소와 안전한 해제 순서를 검증한다.
  초기 정책은 오류 보고 후 해당 역할 종료이고 자동 재시작/다른 renderer 우회는 없다.
- 전용 MessagePort의 `closed`→`disposed`와 DOM endpoint 수명을 지킨다.
  protocol 필드 추가 시 producer/consumer/version 검증을 함께 변경하고 ADR-002/003에
  Graphite 소유권·제출·종료를 후속 구현과 함께 기록한다.

### 5.4 검증과 채택

기능 corpus는 기존 first content, resize, scroll/progress, 선택 상태, 테마 변경과
2열→1열→2열 복귀, focus/한글 IME/semantics, 이미지·텍스트·클립·블렌드·shader,
context/device loss, 입력 중 종료, 잘못된 protocol 거부를 포함한다. 자동 검사와
실제 입력·물리 기기 관찰을 각각 기록한다. 지원 브라우저/OS/GPU 조합별로 결과를
남기며 Chromium 한 환경의 결과를 다른 브라우저 지원 완료로 확대하지 않는다.

DPR 1/1.25/1.5/2의 직접/캐시 화면과 hover/전환 중간 프레임을 확인한다. backend별
AA 차이는 동일성 오차와 구분해 근거·허용 범위를 먼저 기록하고, 글자/이미지 누락,
geometry/색/alpha/상태 회귀를 tolerance 확대로 숨기지 않는다.

성능 비교는 S2↔G의 같은 workload 3쌍을 기본으로 build/layout, CPU draw/record,
submit, 가능한 GPU 시간, input-to-exact-frame 지연, cold shader/pipeline 준비,
다운로드 크기와 WASM/managed/GPU/process 메모리를 구분한다. 측정 불가능한 GPU
시간은 `notMeasured`다. 3절의 회귀 보류 기준을 적용하고 기존 절대 성능 gate는
유지한다. 기능 PASS여도 개선 미확인은 `performanceUnproven`, GPU 부분만 개선은
`componentOnly`로 남긴다. 기능이 통과한 명시적 실험 모드 추가와 기본 renderer
변경은 별개이며, 이 계획의 완료가 WebGPU 기본값 승격을 뜻하지 않는다.

G의 필수 기능 gate 실패 시 실험 산출물·최초 실패를 보존하고 제품 실행은 S2로
돌린다. G 전용 변경과 공통 렌더러 변경을 구분해 원복하고 통과한 U 변경을 유지한다.

## 6. 실행 예산·완료 판정

후속 실행은 U/G 구분을 가진 ledger로 센다. 과거 최초 부팅 2회와 runtime 전환
20회, 통합·병렬·결합 후보 실행 기록은 그대로 보존한다. 한 실행 묶음은 기본
10회, 독립 host/기능 검증 확장 이유를 기록한 경우 최대 20회다. 모든
test/build/benchmark는 외부 timeout 20분, retry 0이다. warm-up·실패·중단·setup
재시도도 포함하고 GPU/browser와 build는 순차 실행한다.

각 결과에는 단계, HEAD/dirty, package/native/runtime 버전, renderer, owner,
source/asset hash, command·timeout·elapsed, 최초 실패, 기능/성능/물리 수용 상태를
남긴다. 한 묶음에 완료하지 못한 플랫폼과 gate는 범위를 명시하고 다음 작업으로
남긴다. 실행 예산 때문에 검증을 생략한 항목을 PASS로 바꾸지 않는다.

- [x] SkiaSharp 목표 태그·배포 NuGet의 Graphite/Dawn/WASM 지원 기반 검토
- [x] U0–U3: 패키지 전환 및 가용 플랫폼별 회귀/범위 판정; 물리 notVerified 별도
- [ ] G0–G4: WebGPU 수명 FAIL 해결, 기능·성능 검증 및 채택 판정
- [x] 사용자 요청에 따른 병렬 레이아웃 코드·옵션·전용 검증·계획 제거

### 6.1 병렬 레이아웃 제거 검증 (2026-09-08)

- PASS: `dotnet build DorotiTestbedApp/DorotiTestbedApp.csproj -c Release --no-restore --nologo`,
  1분 47초, 경고·오류 0. 공용 Rendering과 Material/Testbed를 함께 컴파일했다.
- PASS: Web host TypeScript 소스 검사(`--noEmit --skipLibCheck`),
  남은 Playwright TypeScript 검사(`--noEmit`). 각 실행 timeout은 20분이다.
- 최초 Web host 전체 타입 검사 FAIL: 설치된 `@webgpu/types`와 TypeScript
  `lib.dom.d.ts`의 WebGPU 선언이 중복된다. 라이브러리 선언 검사를 제외한
  후속 소스 PASS와 구분하며 의존성·검사 설정은 변경하지 않았다.
- PASS: 이력 밖의 삭제 기능 코드 참조 없음, 문서 상대 링크와 `git diff --check`.
- notVerified: 제거 후 Web native publish·실제 브라우저 화면·물리 입력 회귀.
  기존 후보의 실행·성능 결과로 이번 검증을 대체하지 않는다.

## 7. 버전 고정 근거

아래는 2026-09-08 확인한 자료다. 릴리스 요약에 m154 업데이트만 적혀 있어도
실제 태그에 포함된 API와 배포물을 함께 판단한다. 소스의 테스트 존재를 실행
결과로 간주하지 않는다.

- R8: [4.154.0 Preview 1 릴리스](https://github.com/mono/SkiaSharp/releases/tag/v4.154.0-preview.1.26454.9), [릴리스 설명](https://mono.github.io/SkiaSharp/docs/releases/4.154.0.html).
- R9: 고정 태그 [SKGraphiteContext](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/binding/SkiaSharp/Gpu/Graphite/SKGraphiteContext.cs), [Dawn backend context](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/binding/SkiaSharp/Gpu/Graphite/SKGraphiteDawnBackendContext.cs).
- R10: 고정 태그 [WASM native build](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/native/wasm/build.cake), [NuGet 연결 targets](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/binding/SkiaSharp.NativeAssets.WebAssembly/buildTransitive/SkiaSharp.targets).
- R11: 고정 태그 [브라우저 Graphite/Dawn renderer 테스트](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/tests/Tests/SkiaSharp/Visual/Renderers/GraphiteDawnRenderer.cs), [release 수명 테스트](https://github.com/mono/SkiaSharp/blob/v4.154.0-preview.1.26454.9/tests/Tests/SkiaSharp/SKGraphiteReleaseDawnTests.cs).
- R12: 실제 검토한 [SkiaSharp NuGet](https://www.nuget.org/packages/SkiaSharp/4.154.0-preview.1.26454.9), [WebAssembly native NuGet](https://www.nuget.org/packages/SkiaSharp.NativeAssets.WebAssembly/4.154.0-preview.1.26454.9). U0에서 전체 패키지 가용성과 다운로드 hash를 다시 고정한다.

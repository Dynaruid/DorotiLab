# WGSL 기반 위젯 GPU 효과 작업계획

작성일: 2026-09-26

상태: **계획 작성 완료 / 제품 구현 미착수**. 이번 작업은 현재 소스와 공식 문서 검토 및 이 문서 작성으로 한정한다. 아래 API·파일명·빌드 항목은 별도 표시가 없으면 제안이다. 코드 빌드, 셰이더 컴파일, GPU 실행 및 성능 측정은 이번 계획 작성에서 수행하지 않았다.

## 1. 목표와 완료 범위

위젯 서브트리를 GPU 텍스처에 렌더링하고, WGSL에서 생성한 플랫폼 셰이더로 처리한 결과를 Doroti 장면에 합성한다. 위젯 레이아웃·기본 래스터화·최종 합성은 기존 Skia를 사용하고, 효과 패스는 Vulkan·Metal·WebGPU에서 직접 실행한다.

- 자식 위젯 이미지에 fragment 효과를 적용하는 `GpuEffect`를 먼저 제공한다.
- 같은 실행 계약을 compute, 다중 패스, storage buffer, 이전 프레임 입력, 배경 효과로 확장한다.
- `.wgsl` 에셋과 효과 기술 파일을 빌드 입력으로 받고, 검증·리플렉션·C# 파라미터 생성·플랫폼 산출물 패키징을 자동화한다.
- 제품 실행 중 CPU 픽셀 readback/upload 왕복을 사용하지 않는다. GPU 내부 복사 여부와 바이트 수는 별도로 계측한다. 이 조건만으로 zero-copy라고 부르지 않는다.
- 기본 대상은 현재 Graphite 기반 Windows/Vulkan, Android/Vulkan, Linux/Vulkan, Apple/Metal, Web/Dawn-WebGPU와 **Ganesh 기반 Web/WebGL2**이다. WebGL2도 이번 작업의 필수 구현·검증·배포 대상이다.
- 첫 수직 구현은 Windows/Vulkan이다. Windows만 통과한 상태를 전체 플랫폼 완료로 보고하지 않는다.

현재 작업에서 제외하는 항목:

- Skia 위젯 렌더러 전체 교체, 새 창·스왑체인·프레젠테이션 계층 도입.
- 임의 네이티브 포인터나 GPU 큐를 일반 위젯 코드에 공개하는 API.
- 플랫폼 네이티브 WebView/PlatformView의 모든 내용을 GPU 입력으로 자동 캡처하는 기능.
- D3D12 효과 실행기와 범용 외부 엔진 SDK는 후속 선택 범위다. 번역 산출물 존재를 실행 지원으로 간주하지 않는다.
- WebGL2의 compute shader·storage buffer/texture 실행은 지원 범위에 넣지 않는다. WebGL2는 fragment 기반 다중 렌더 패스·텍스처 history·배경 효과까지 필수 지원하며, 고급 효과의 기능 차이는 명시적인 프로파일로 관리한다.
- ray tracing, mesh shader 등 WGSL 공통 계약 밖의 기능을 자동으로 지원한다는 약속.

## 2. 현재 소스 검토 결과

| 근거 | 현재 제공하는 것 | 이번 작업에서 필요한 것 |
| --- | --- | --- |
| [SkiaSceneRenderer.Filters.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.Filters.cs) | SkSL 이미지·백드롭 셰이더 조합용 GPU 중간 표면과 레이어 처리 | WGSL 전용 장면 명령과 외부 효과 실행 경로. 기존 SkSL 분기에 WGSL 문자열을 전달하는 방식은 사용하지 않는다. |
| [SkiaGpuSurfaces.cs](Doroti/src/Doroti.Skia.RuntimeEffects/SkiaGpuSurfaces.cs) | 같은 Ganesh context 또는 Graphite recorder에서 호환 표면 생성 | 네이티브 핸들·usage·상태 전환을 호스트가 제어하는 효과용 텍스처 할당 |
| [SkiaGraphiteSession.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.cs) | recorder, frame, 제출, GPU 완료 후 수명 관리 | 한 논리 프레임 안에서 Skia 입력 기록·외부 패스·Skia 합성을 순서대로 제출하는 계약 |
| [SkiaGraphiteSession.Vulkan.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.Vulkan.cs) | 호스트 소유 Vulkan 이미지의 target wrapping, 상태 조회·갱신 | 효과 입력/출력 target, 중간 제출과 GPU 상태 전환의 안전한 확장 |
| [SkiaGraphiteVulkanInterop.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteVulkanInterop.cs) | 실제 제출된 이미지 상태 관찰, 큐·리소스 소유권 검사 | 외부 명령 실행 전후 상태를 Graphite와 일치시키는 인터롭 검증 |
| [SkiaGraphiteSession.NativeTextures.cs](Doroti/src/Doroti.Skia.Rendering/SkiaGraphiteSession.NativeTextures.cs) | 외부 이미지 import 및 GPU 완료까지 lease 유지 | 효과용 양방향 이미지 전달. 현재 import API가 Skia 내부 표면 export를 보장하지는 않는다. |
| [NativeTextureContracts.cs](Doroti/src/Doroti.Ui/NativeTextureContracts.cs) | producer-complete 불변 프레임과 참조 수명 계약 | 같은 프레임에서 생성·쓰기·읽기가 이어지는 효과 자원은 별도 계약으로 정의 |
| [DorotiWebWorkerSurface.Graphite.cs](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.Graphite.cs), [doroti.webgpu.ts](Doroti/src/Doroti.Host.Web/Web/doroti.webgpu.ts) | Worker의 JS GPUDevice/Queue/Texture를 Dawn에 연결 | 동일 device/queue를 이용한 사용자 패스와 효과용 texture usage 확장 |
| [DorotiWebWorkerSurface.cs](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs), [doroti.web.texture-worker.ts](Doroti/src/Doroti.Host.Web/Web/doroti.web.texture-worker.ts), [doroti.raster.worker.ts](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts) | Worker의 동일 WebGL2 context, Ganesh framebuffer wrapping, GL texture handle table, GL state dirty 처리와 자원 fence | 효과용 FBO/texture, GLSL program, Skia→외부 GL pass→Skia 전환과 정확한 GL state 복구 |
| [App SDK](Doroti/src/Doroti.App.Sdk/Sdk/Sdk.targets) | 플랫폼 중립 `net10.0` 앱, `.sksl` EmbeddedResource, `GetDorotiApplicationContract` | 공통 WGSL 검증·리플렉션·생성 코드 및 효과 입력 계약 |
| [Runner SDK](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets), [애플리케이션 리소스 로딩](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs) | 플랫폼 runner, 앱 manifest, 리소스 길이·SHA-256 검사 | runner별 shader variant manifest와 실제 바이너리 제공자 |

검토 시 루트 `work.md`는 내용 없는 미추적 파일이었다. IDE에 표시된 `tools/Doroti.Wgsl/src/lib.rs`와 해당 디렉터리는 현재 디스크에서 발견되지 않았다. 구현 시작 시 다시 확인하고, 기존 도구가 생겼다면 내용과 소유권을 검토해 확장한다. 이 계획은 그 도구가 이미 작동한다고 가정하지 않는다.

중요한 제약:

1. 현재 `Frame.SubmitCore`는 `Snap → InsertRecording → Submit` 후 활성 recording frame을 종료한다. 이를 입력 캡처 중간에 그대로 호출하면 한 프레임의 수명·제출 계약을 깨뜨릴 수 있다.
2. 현재 Vulkan interop `Insert`는 비어 있지 않은 외부 wait/signal 배열을 거부한다. 기존 메서드 인자만 채워 넣어서 외부 GPU 패스를 연결할 수 있다고 가정하지 않는다.
3. `GetState()`의 scheduled state와 GPU 완료 상태는 구별해야 한다. `SetStateAfterGpuCompletion`의 조건을 우회해서 아직 실행되지 않은 barrier를 완료된 것으로 기록하지 않는다.
4. `SKSurface.Snapshot()`은 네이티브 실행기에 넘길 수 있는 공개 Vulkan/Metal 핸들을 자동 제공하지 않는다. **호스트가 효과용 이미지를 먼저 할당하고 Skia에 wrap하는 방식**을 우선 검증한다.

## 3. 결정할 아키텍처

### 3.1 공통 실행 계약

`GpuEffect`의 자식 장면을 효과 입력 이미지에 렌더링한 뒤, 패스 그래프를 실행하고 최종 이미지를 원래 장면에 합성한다. 위젯마다 상시 GPU 버퍼를 생성하지 않고 효과가 필요한 서브트리만 분리한다.

- `GpuEffectProgram`: 에셋 ID, entry point, ABI 버전, 필요한 기능, 패스 정의를 가진 불변 프로그램.
- `GpuEffectParameters`: 생성된 타입이 직렬화한 불변 파라미터 스냅샷. UI 스레드의 변경 가능한 객체를 렌더 스레드가 직접 참조하지 않는다.
- `GpuEffectLayer` / `SceneGpuEffectPayload`: child 범위, capture rect, output outsets, 프로그램·파라미터 revision을 보존한다.
- `IGpuEffectBackend`: capability 조회, 텍스처 lease, pipeline 준비, pass 기록, 제출 순서 연결, retirement를 담당하는 호스트 내부 계약.
- `GpuTextureLease`: device/owner/context generation, 크기, format, usage, 색 공간, alpha, 유효 영역과 마지막 GPU 사용 정보를 가진 내부 타입. 일반 앱에는 원시 핸들을 노출하지 않는다.
- `GpuEffectController`: 시간·history·외부 입력 revision에 따른 repaint 요청. 지속 애니메이션은 명시적으로 시작하며 정지 상태에서는 idle wakeup을 만들지 않는다.

제안 사용 형태:

```xml
<ItemGroup>
  <DorotiGpuEffect Include="assets/effects/liquid.wgsl"
                   Definition="assets/effects/liquid.effect.json" />
</ItemGroup>
```

```csharp
// Effects.Liquid / LiquidParameters는 빌드에서 생성하는 제안 API다.
new GpuEffect(
    effect: Effects.Liquid,
    parameters: new LiquidParameters { Time = seconds, Strength = 0.6f },
    child: panel);
```

기존 `FragmentProgram`/`FragmentShader`는 SkSL 계약을 유지한다. WGSL 프로그램을 기존 SkSL 객체로 위장하지 않는다. 공통 render layer/lease 처리는 실제 중복이 확인된 부분만 공유한다.

### 3.2 첫 WGSL ABI

초기 프로파일은 2D 단일 sample 텍스처와 엔진 제공 fullscreen triangle vertex shader를 사용한다. 사용자 WGSL의 `fs_main` fragment entry를 호출하며, vertex/fragment 위치·UV 규약은 빌드 manifest에 버전으로 기록한다.

| 바인딩 | 의미 | 소유자 |
| --- | --- | --- |
| group 0 / binding 0 | `texture_2d<f32>` 자식 입력 | 엔진, 읽기 전용 |
| group 0 / binding 1 | 입력 sampler | 엔진, address/filter 정책을 효과 정의에 명시 |
| group 0 / binding 2 | 프레임 uniform | 엔진 |
| group 1 / binding 0 | 사용자 파라미터 uniform | 생성된 C# 직렬화기 |
| group 0 / binding 3 | compute 프로파일의 출력 storage texture | 엔진, 초기에는 write-only `rgba8unorm` |

프레임 uniform 제안: `inputSize: vec2<u32>`, `outputSize: vec2<u32>`, `logicalSize: vec2<f32>`, `time: f32`, `deltaTime: f32`의 32바이트 구조. 실제 offset/size는 Naga 검증 결과와 golden 테스트로 확정한다. uniform/벡터/행렬/배열 정렬을 C# 메모리 레이아웃에서 추측하지 않는다.

- 추가 샘플러·storage buffer·history는 이후 capability와 manifest에 바인딩을 명시한다. 임의 배열을 모든 GPU에서 지원한다고 가정하지 않는다.
- WGSL 기준 직렬화 ABI와 backend별 실제 binding/layout mapping을 함께 검증한다. 번역 과정에서 생기는 register 재배치나 padding 변환은 생성 metadata와 backend adapter가 처리한다. 모든 출력 언어에 같은 메모리 배치를 무조건 복사하지 않는다.
- effect definition은 entry point, 패스 read/write, output size/scale, sampling, outsets, required features, history 정책을 선언한다. WGSL 텍스트 안의 주석을 별도 설정 언어로 파싱하지 않는다.
- 텍스처 좌표는 좌상단 원점, UV와 픽셀 좌표 변환 규칙을 고정한다. Vulkan·Metal·Web의 Y 방향 차이는 백엔드/생성 vertex 코드에서 한 번만 처리한다.
- 기본 처리 색 공간은 선형 RGB, premultiplied alpha로 설계하고 Skia 입력/출력 변환 경계를 명시한다. `rgba8unorm`과 sRGB texture view를 같은 것으로 취급하지 않는다. HDR/`rgba16float`는 별도 capability로 확장한다.
- 픽셀 변형은 기본적으로 layout, hit testing, semantic bounds를 바꾸지 않는다. 시각 outsets와 상호작용 영역은 분리해서 문서화한다.

### 3.3 그래프와 GPU 실행 순서

한 논리 프레임에 대해 다음 순서를 보장한다.

1. 렌더 owner가 child와 파라미터를 스냅샷하고 필요한 자원을 예약한다.
2. 호스트 소유 입력 texture를 Graphite target으로 wrap하여 child를 렌더링한다.
3. 입력을 생성하는 Skia 기록을 제출하고, 뒤의 효과 패스가 그 쓰기를 관찰하도록 의존성과 이미지 상태를 연결한다.
4. 외부 fragment/compute 패스가 입력을 읽고 별도 출력에 기록한다.
5. 출력의 상태를 Skia 샘플링에 맞추고 후속 Graphite 기록으로 합성한다.
6. 기존 host presentation 경로로 제출한다. 전체 자원은 마지막 GPU 사용이 완료된 후 반환한다.

같은 queue를 사용해도 필요한 memory barrier·layout transition은 생략하지 않는다. 첫 제품 경로는 동일 device와 호스트 관리 queue를 사용하고, 별도 wgpu device나 adapter를 새로 생성하지 않는다. 프레임 도중 `QueueWaitIdle`/device-wide wait를 정상 경로로 사용하지 않는다.

실제 Graphite 외부 패스 경계가 가능한지 G0에서 확인한다. 필요한 경우 **하나의 frame transaction 안에 여러 recording/submission segment를 유지하는 API**를 추가한다. 중간 segment마다 presented/frame generation을 증가시키지 않는다. 지원되지 않는 네이티브 binding이 필요하면 명시적인 최소 확장과 플랫폼 패키지 배포 작업을 별도 항목으로 기록한다. reflection으로 private handle을 꺼내는 우회는 사용하지 않는다.

WebGL2는 Graphite recording 대신 같은 Worker의 Ganesh `GRContext`와 현재 `WebGL2RenderingContext`를 사용한다. Skia 입력 쓰기를 flush한 뒤 동일 context에서 FBO 기반 효과 draw를 실행하고, Skia에 제어를 돌려주기 전에 실제 GL binding 상태와 Skia의 state cache를 일치시킨다. 효과마다 별도 canvas/context를 만들거나 WebGPU로 전환하지 않는다. `ResetContext(GRGlBackendState.All)`는 Skia cache invalidation이며 실제 FBO/VAO/viewport 등의 상태 복구까지 자동 수행한다고 가정하지 않는다.

중첩 효과는 자식 의존성부터 처리하고 입력/출력은 ping-pong 자원으로 분리한다. 같은 subresource를 동시에 sampled input과 writable output으로 사용하지 않는다. dependency cycle은 빌드 또는 graph 구성 단계에서 진단한다. 이전 프레임 참조만 명시적인 temporal edge로 허용한다.

### 3.4 캡처·수명·캐시

- capture rect는 child paint bounds, effect outsets, 변환, DPR, ancestor clip을 고려한다. 출력 clip을 먼저 적용해 blur/왜곡 입력 halo를 잘라내지 않는다.
- 공개되지 않은 Skia saveLayer 내부 이미지를 사용하지 않는다. opacity·color filter·mask 내부의 현재 레이어를 정확히 입력으로 삼도록 장면 경계를 처리한다.
- `enabled=false`는 추가 텍스처·pass 없이 child를 그대로 그린다. 지원하지 않는 백엔드에서 임의로 비활성화하지 않는다.
- 입력 재사용 키: child paint revision, capture bounds, transform/DPR, texture-source revision, format, device generation. 출력 키에는 프로그램 hash와 파라미터 revision, history revision도 포함한다.
- uniform만 변하면 입력 child 재래스터화 없이 효과 출력만 갱신할 수 있어야 한다. 동영상·외부 텍스처 업데이트는 widget rebuild 없이 입력 revision을 갱신한다.
- 크기/DPR/context generation 변경 시 이전 자원을 새 프레임에 사용하지 않는다. in-flight 자원은 폐기 예약하고 GPU 완료 후 retire한다.
- history는 effect instance/device generation에 귀속한다. resize·재시작 시 clear하며, 기록·제출이 취소된 프레임의 history를 유효한 다음 입력으로 승격하지 않는다.
- texture pool은 host의 in-flight 제한과 별도의 effect memory budget 안에서 동작한다. `width × height × bytesPerPixel × liveTargets`와 중간 버퍼를 함께 계산한다. 예산 초과 시 명시적 오류 또는 앱이 선택한 품질 정책을 적용한다.
- retained scene, dispose, 미제출 recording 폐기, 제출 후 오류, device loss, hot reload마다 release 경로를 구분한다. 해제 스레드와 queue 완료 증거를 기록한다.

### 3.5 필수 backend 프로파일과 기능 차이

| 기능 | Vulkan·Metal·WebGPU | WebGL2 |
| --- | --- | --- |
| 자식 위젯 fragment 효과·uniform·sampled texture | 필수 | 필수 |
| 여러 fragment 패스·ping-pong texture | 필수 | 필수, FBO draw로 실행 |
| 이전 프레임 texture history | 필수 | 필수, context generation별 관리 |
| 배경 입력·중첩 clip/opacity/mask | G8 필수 | G8 필수 |
| compute·storage buffer·storage texture | G5 필수, 실제 device limits 확인 | 명시적 미지원 |
| float render target·HDR·추가 texture format | capability에 따라 확장 | 확장/format별 검사, 기본 RGBA8 경로와 구분 |

`webgl2-fragment`는 GLSL ES **3.00**과 WebGL2의 기능·한도에 맞춘 필수 프로파일이다. Naga의 일반 GLSL 출력 성공만으로 WebGL2 호환성을 판정하지 않는다. ES 3.10 이상의 기능, compute entry, SSBO, image load/store 등은 해당 프로파일 검증 단계에서 거부한다.

- WGSL의 texture/sampler 분리를 GLSL의 combined sampler와 texture unit에 매핑하고, uniform block/binding은 리플렉션 metadata에 맞춰 UBO 또는 필요한 backend serializer로 연결한다.
- `MAX_TEXTURE_SIZE`, texture unit 수, uniform block 크기·alignment, draw buffer 수, renderable/filterable format을 실제 context에서 확인한다. 필수 RGBA8 경로가 float color buffer 확장에 의존하지 않게 한다.
- 공통 fragment 에셋은 같은 WGSL 원본에서 각 backend variant를 만든다. compute 효과가 WebGL2를 지원하려면 작성자가 명시한 fragment pass graph 대안을 제공해야 한다. compiler가 임의로 compute를 fragment나 CPU 처리로 바꾸지 않는다.
- 에셋의 `RequiredBackends`와 선택적 capability를 definition에 명시한다. WebGL2 필수 에셋에 대응 variant가 없으면 빌드 오류이며, WebGPU 전용으로 선언한 에셋은 그 제한을 manifest에 보존하고 WebGL2에서 실행 요청 시 capability 오류를 반환한다.
- variant 선택은 이미 선택된 renderer 안에서 이루어진다. 효과 때문에 renderer 선택 정책을 바꾸지 않는다.

## 4. WGSL 컴파일과 패키징

### 4.1 도구 구성

`tools/Doroti.Wgsl/`에 Rust/Naga 기반 도구를 제안한다. 초기 실행기는 C#과 기존 호스트 네이티브 경계에 두고, 전체 wgpu runtime을 제품에 추가하지 않는다.

- `validate`: WGSL 파싱·검증, entry point/stage, 선언된 binding·feature 검사.
- `reflect`: uniform offset/size/stride, resource binding, workgroup 크기, 필요한 기능을 안정된 JSON schema로 출력.
- `compile`: 선택된 target profile에 대해 SPIR-V/MSL/WGSL 등의 산출물 생성.
- `generate`: 공통 reflection으로 C# effect 식별자와 명시적 byte serializer 생성. 런타임 reflection/동적 코드 생성을 요구하지 않는다.
- Naga·Rust toolchain·Cargo.lock·schema 버전을 고정한다. 배포된 SDK는 빌드 호스트 RID에 맞는 도구를 사용하고 사용자 앱 빌드마다 Cargo 네트워크 설치를 실행하지 않는다.
- 진단은 원본 `.wgsl`/definition의 파일·행·열과 안정된 오류 코드로 MSBuild에 전달한다.

### 4.2 플랫폼별 산출물

| 실행 백엔드 | 빌드 처리 | 실행 시 남는 처리 | 범위 |
| --- | --- | --- | --- |
| Vulkan | WGSL → SPIR-V, target environment 검증 | shader module/pipeline 생성·캐시 | 기본 |
| Metal | WGSL → MSL → Apple SDK의 `.metallib` | function/pipeline 생성·캐시 | 기본, SDK·OS·device/simulator 타깃 구분 |
| WebGPU | WGSL 검증·필요한 정규화·manifest 패키징 | 브라우저 shader module/pipeline 컴파일 | 기본 |
| D3D12 | WGSL → HLSL → DXC/DXIL | D3D12 pipeline | 후속 선택 범위 |
| WebGL2 | WGSL → GLSL ES 3.00 fragment + 엔진 vertex shader + binding metadata | 같은 Worker GL context에서 shader compile/program link | **기본 필수**, 다중 fragment 패스·history·배경 효과 포함, compute 제외 |

MSL 문자열 생성과 `.metallib` 생성은 별개이다. Apple 최종 빌드는 해당 SDK/toolchain이 있는 환경에서 수행한다. WebGPU는 WGSL 소스를 받으므로 모든 GPU 기계어를 빌드 시 확정한다고 설명하지 않는다. Naga는 그 자체로 Metal AIR나 DXIL 최종 생성기가 아니다.

WebGL2도 GLSL 소스를 패키징하며 브라우저가 compile/link를 수행한다. 빌드에서 문법·프로파일·binding을 검사하고 실제 브라우저의 compile/link 결과와 원본 WGSL 위치 진단을 별도로 수집한다. 런타임의 program 준비 비용은 pipeline warmup과 캐시 검증에 포함한다.

### 4.3 App SDK와 Runner SDK 분리

1. App SDK는 `DorotiGpuEffect` 입력 수집, WGSL 공통 검증, reflection 및 C# 타입 생성을 `CoreCompile` 전에 수행한다. 앱은 계속 `net10.0` 플랫폼 중립이다.
2. `GetDorotiApplicationContract`에 effect source/definition, 논리 asset ID, hash, 공통 ABI manifest를 추가한다. 앱에 Metal/Vulkan host 참조를 넣지 않는다.
3. Runner SDK가 실제 renderer backend·RID·SDK에 맞는 variant를 생성/선택한다. RID만으로 renderer를 결정하지 않는다. Windows Vulkan을 프레젠테이션이 DXGI라는 이유로 HLSL 대상으로 취급하지 않는다.
4. 현재 리소스 provider는 앱 assembly manifest resource를 읽는다. runner variant가 그곳에 이미 포함되어 있다고 가정하지 않고, 별도 shader resource catalog/provider를 application boundary에 연결한다. stable asset ID와 variant hash를 묶어 기존 무결성 검사를 확장한다.
5. NuGet으로 배포하는 위젯 라이브러리도 WGSL/definition/reflection을 전달할 수 있어야 한다. ProjectReference와 package 소비 모두 테스트한다.
6. 생성물은 공통 앱 `obj`와 runner의 backend/RID별 `obj`로 분리한다. 같은 앱의 Windows/Web/iOS 빌드가 산출물을 덮어쓰지 않게 한다.
7. 증분 키는 소스·definition·생성 ABI·compiler 버전·target profile·플랫폼 compiler 옵션/버전이다. 변경 없는 빌드는 재컴파일하지 않고 삭제·이름 변경된 에셋은 publish에 남기지 않는다.
8. 산출물 manifest에는 schema/tool version, entry point/stage, bindings, uniform layout, required features, profile, source/binary hashes, 패스 의존성을 포함한다. 컴파일된 C# ABI와 variant manifest 불일치는 로드 시 거부한다.
9. Web runner는 선택 가능한 WebGPU/WebGL2 프로파일의 효과 variant를 함께 publish한다. 같은 `browser-wasm` RID의 두 backend 산출물을 분리하고, 의도적으로 하나만 포함하는 publish 옵션은 선택 가능한 renderer/capability와 함께 검사한다. browser cache와 삭제된 GLSL program의 재사용 키에도 backend/profile hash를 포함한다.

## 5. 구현 단계와 통과 조건

모든 단계는 현재 **NOT_STARTED**다. 아래 `[ ]`는 구현 작업이며 이번 소스 검토로 완료 처리하지 않는다.

### G0 — 외부 GPU 패스 연결 가능성 검증

- [ ] 현재 SkiaSharp/Graphite binding, Vulkan 상태 추적, Metal/Dawn 호스트 소유권을 확인하고 필요한 API 확장을 목록화한다.
- [ ] Windows/Vulkan에서 호스트 소유 입력/출력 이미지와 고정 fragment shader로 `Skia draw → native pass → Skia composite` 최소 경로를 만든다. WGSL 전체 빌드 시스템보다 먼저 검증한다.
- [ ] 실제 장치/queue 동일성, usage flags, MSAA resolve 필요 여부, 기록 순서, native pass 실행, 입력/출력 상태, 완료 후 release를 증명한다.
- [ ] CPU readback 0회, 프레임 도중 device/queue idle wait 0회, 최종 표시 순서와 픽셀을 확인한다.
- [ ] Metal/Dawn도 같은 연결을 할 수 있는지 소스와 최소 probe로 구분해 기록한다. 아직 실행하지 못한 플랫폼은 `notVerified`다.
- [ ] WebGL2 Worker에서 호스트 할당 texture/FBO → Ganesh 입력 draw → 외부 GL fragment pass → Ganesh 합성의 최소 probe를 수행한다. 실제 제품 GL context 동일성, state 복구, feedback loop 부재와 context-loss 시 수명을 확인한다.

통과 조건: Windows 실제 GPU와 제품 host에서 외부 효과 패스가 실행된 증거가 있고, 한 frame transaction의 수명 규칙이 유지된다. 필요한 binding이 없다면 정확한 추가 API·빌드/패키지 작업을 계획에 반영한 뒤 해당 backend 구현을 계속한다. 컴파일러만 만들어 G0를 통과했다고 하지 않는다.

### G1 — 효과 프로파일과 자원 ABI 확정

- [ ] `GpuEffectProgram`, parameter snapshot, capability, internal texture lease, 패스 definition schema를 설계·구현한다.
- [ ] fragment 기본 프로파일, binding map, 픽셀/UV 좌표, 색 공간·alpha, outsets, 오류 계약을 고정한다.
- [ ] `webgl2-fragment` 프로파일의 GLSL ES 3.00 제한, combined sampler/UBO mapping, capability 오류 및 명시적 대안 pass graph 계약을 확정한다.
- [ ] WGSL 구조체 layout과 C# serializer의 golden을 만든다. `vec3`, matrix, array padding, `u32/i32/f32`를 포함한다.
- [ ] 사용자 입력/엔진 예약 binding 충돌, 없는 entry point, 읽기·쓰기 alias, 잘못된 resource format을 진단한다.

통과 조건: ABI를 단순 scalar 예제에만 맞추지 않고 구조체·배열 및 잘못된 선언까지 검증한다. unsupported capability는 명시적으로 보고한다.

### G2 — WGSL 도구와 빌드 에셋 경로

- [ ] `tools/Doroti.Wgsl/` 도구와 도구 배포 패키지를 추가하고 compiler/schema를 고정한다.
- [ ] App SDK 공통 검증·reflection·C# 생성 및 application contract 전달을 구현한다.
- [ ] 우선 Vulkan variant와 runner resource catalog를 연결하고 Metal/WebGPU/WebGL2 variant 생성을 같은 schema로 확장한다. WebGL2의 엔진 vertex shader와 사용자 fragment shader link 인터페이스도 검증한다.
- [ ] clean/no-op/수정/삭제/이름변경 빌드, 동시 다른 runner 빌드, NuGet 소비, publish 무결성을 검증한다.
- [ ] source/variant ABI hash 불일치 및 compiler 부재 진단을 추가한다. 결과 없는 compiler 성공 코드를 성공으로 인정하지 않는다.

통과 조건: 새 앱이 `.wgsl` 에셋 선언만으로 타입 생성과 선택 backend 패키징까지 수행하고, 수동 파일 복사 없이 manifest에서 읽힌다.

### G3 — 위젯·렌더 레이어·장면 통합

- [ ] Widgets에 `GpuEffect`, Rendering에 proxy render object/layer, Ui에 불변 scene payload를 추가한다.
- [ ] enabled, parameter update, child repaint, external texture revision, retained layer 및 dispose에 맞는 invalidation을 연결한다.
- [ ] capture bounds/halo/DPR/transform/clip/opacity/mask와 합성 위치를 유지한다. hit testing과 semantics 계약을 유지한다.
- [ ] 미지원 host에서 분명한 capability 오류를 반환한다. SkSL 필터 또는 CPU 효과로 조용히 바꾸지 않는다.
- [ ] 실제 wire transport를 사용하는 경로가 있다면 opcode/schema를 명시적으로 확장하고 encode/decode golden을 추가한다. 비활성 display-list 경로를 제품 실행 증거로 사용하지 않는다.

통과 조건: 위젯에서 생성한 scene이 실제 실행기에 도달하고, 낮은 수준의 수동 SceneBuilder 예제에만 기능이 존재하지 않는다.

### G4 — Windows/Vulkan 제품 경로 완성

- [ ] G0의 검증된 frame segment 계약을 `SkiaGraphiteSession`과 host/Vulkan 계층에 통합한다.
- [ ] input/output allocator, descriptor bindings, pipeline cache, native render pass, barrier, output wrapping, retirement를 구현한다.
- [ ] WindowsAppSdk 기본 host와 MAUI host의 공통 Vulkan 경로를 연결한다. 기존 DXGI/HWND presentation 경로는 유지한다.
- [ ] offscreen pixel 비교와 실제 testbed 표시를 모두 확인한다. resize·DPR·스크롤·애니메이션 중 stale texture/blank frame을 확인한다.
- [ ] 기존 [backdrop-filters](Doroti/validation/backdrop-filters/README.md), [texture 경로](Doroti/validation/textures/README.md), 플랫폼 raster/resize 경로 회귀를 수행한다.

통과 조건: WGSL 에셋부터 제품 위젯 표시까지 단일 fragment 효과가 동작한다. **G0~G4를 첫 Windows milestone**로 삼으며, 이때 전체 목표 상태는 `PARTIAL`이다.

### G5 — Compute·다중 패스·상태 자원

- [ ] compute entry/workgroup/dispatch와 write-only storage texture를 추가한다. 실제 장치 limit/feature를 검사한다.
- [ ] DAG 스케줄링, 임시 texture aliasing 조건, ping-pong, fragment↔compute 사이 barrier를 구현한다.
- [ ] storage buffer의 크기·접근 권한·수명, history의 resize/device-loss/reset/취소 정책을 구현한다.
- [ ] WebGL2에는 같은 그래프 계약의 fragment→fragment·ping-pong·history 실행을 제공한다. compute/storage 요구는 명시적으로 거부하고, 작성자가 제공한 WebGL2 fragment 대안은 별도 variant로 검증한다.
- [ ] 두 단계 blur/bloom과 시간 누적 효과로 순서와 history 오류를 검출한다. GPU→CPU→GPU 중계 없이 실행되는지 확인한다.

통과 조건: 두 개 이상의 실제 GPU 패스가 의도한 자원을 읽고 쓰며, 폐기된 프레임·재생성된 device의 history가 섞이지 않는다.

### G6 — Metal 및 다른 Vulkan 호스트

- [ ] Apple SDK별 `.metallib` 생성, native resource packaging, 동일 MTLDevice/queue 기반 input/output texture와 encoder 연결을 구현한다.
- [ ] Graphite Metal 기록과 사용자 encoder 사이 순서·완료·lease를 연결한다. iOS device와 simulator 산출물을 구분한다.
- [ ] Android/Linux의 기존 Vulkan 장치·queue·surface lifecycle에 공통 실행기를 연결한다.
- [ ] Android pause/resume·surface 재생성, Apple background/foreground, Linux resize·teardown을 확인한다.

통과 조건: macOS AppKit·Mac Catalyst, iOS 실기기, Android 실기기, Linux Qt GPU host에서 에셋 패키징→표시→자원 회수 증거를 남긴다. iOS simulator는 device와 별도 행으로 관리한다. 크로스 빌드나 emulator만으로 실기기 항목을 PASS 처리하지 않는다.

### G7 — WebGPU·WebGL2 Worker 실행기

#### G7-A — WebGPU

- [ ] WGSL/manifest를 runner publish에 포함하고 렌더 Worker에서 pipeline을 준비한다.
- [ ] `doroti.webgpu.ts`의 기존 GPUDevice/queue를 사용한다. managed 영역은 안전한 ID·메타데이터만 전달한다.
- [ ] effect texture를 필요한 render attachment / sampled / storage / copy usage로 생성하고 Dawn과 동일 자원을 공유한다.
- [ ] Graphite 입력 제출·JS 효과 command buffer·Graphite 합성의 queue 순서와 texture handle 수명을 증명한다.
- [ ] device loss, Worker 종료, 탭 background, canvas resize와 삭제 에셋의 stale cache를 검증한다.
- [ ] 현재 renderer 선택 정책을 유지하고, 효과의 필수 capability가 없으면 진단한다. WebGL2/WebGPU 간 자동 전환을 추가하지 않는다.

통과 조건: 실제 브라우저의 제품 Worker 경로에서 fragment/compute 예제를 표시하고, JS mock·소스 검사와 실행 증거를 구별한다. 이 조건만으로 G7 전체를 완료 처리하지 않는다.

#### G7-B — WebGL2 (이번 작업의 필수 범위)

- [ ] WGSL에서 생성한 GLSL ES 3.00 vertex/fragment 에셋, reflection, program cache를 Web runner와 Worker loader에 연결한다. 실제 WebGL2에서 compile/link하고 오류를 원본 에셋과 연계한다.
- [ ] 기존 Worker의 현재 GL context 및 texture handle table을 사용해 입력/출력 texture와 FBO를 할당한다. framebuffer completeness, texture format, sampler 상태와 Ganesh wrapping을 검사한다.
- [ ] Skia 입력 flush → GL 효과 draw → Skia 합성 전환을 구현한다. program, read/draw framebuffer, VAO, active texture·sampler·UBO binding, viewport/scissor, blend/depth/stencil/color mask 등 변경한 상태를 관리하고 Ganesh state cache를 무효화한다.
- [ ] 입력 texture와 출력 attachment를 분리하고, sampling/attachment feedback loop 없이 fragment 다중 패스와 texture history를 실행한다. compute 전용 에셋의 명시적 거부와 작성된 대안 variant 선택도 확인한다.
- [ ] 기존 GL fence/비동기 완료 추적과 texture lease retirement를 연결한다. `gl.finish()`나 동기식 fence 대기를 매 프레임 사용하지 않으며, `readPixels`·CPU staging 왕복은 제품 경로에 넣지 않는다.
- [ ] resize·DPR·context loss·Worker 재시작 시 program/FBO/texture/UBO/VAO/history를 generation별로 재생성한다. 현재 Worker의 context 복구에는 canvas endpoint 재바인딩이 필요하다는 조건을 보존한다.
- [ ] 동일 위젯 장면을 WebGL2와 WebGPU에서 표시하고 identity·색상·왜곡·2패스 blur·history를 비교한다. 효과 이후 그리는 텍스트/clip/texture가 오염되지 않는지 확인한다.
- [ ] Chrome 계열·Firefox·Safari/WebKit의 실제 WebGL2 실행 증거를 기록하고, 모바일 브라우저의 메모리·context-loss도 확인한다. 확보하지 못한 브라우저/장치는 별도 `notVerified`로 남긴다.

통과 조건: 제품 Worker/Ganesh 경로에서 fragment, 다중 패스, history, context 복구와 최종 표시 검증이 통과해야 한다. WebGL2의 compute 미지원은 명시한 기능 경계이며, fragment 실행 미구현이나 브라우저 실행 미검증을 대체하지 않는다. **G7-A와 G7-B 모두 필수**다.

### G8 — 배경 효과와 제품 API 마무리

- [ ] `GpuBackdropEffect`를 추가해 child 입력과 backdrop 입력을 명확히 구분한다.
- [ ] 현재 레이어의 선행 그리기만 입력에 포함하고, 자기 출력이나 뒤의 형제 위젯이 입력에 섞이지 않게 한다.
- [ ] 중첩 opacity/mask/filter, 겹치는 배경 영역, halo, blend mode 및 유지된 레이어를 검증한다.
- [ ] WebGL2에서도 현재 레이어의 backdrop 입력을 texture/FBO로 확보해 처리한다. 최종 canvas 전체를 복사해 중첩 레이어 의미를 잃거나 `readPixels`로 우회하지 않는다.
- [ ] 네이티브 PlatformView가 입력에 포함되는 경우 host capability를 확인하고, 캡처 불가능한 surface는 명시적으로 거부한다.
- [ ] loader/controller의 비동기 준비, 취소, enabled, 지원 여부 조회, asset/pipeline 실패 표시 계약을 정리한다.

통과 조건: child와 backdrop의 의미 차이가 API·문서·픽셀 테스트에 일치하며, GPU 합성 장면 밖의 native content까지 지원한다고 오인시키지 않는다.

### G9 — 비용·수명·배포 검증과 문서

- [ ] 텍스처 pool/예산, 캐시 invalidation, pipeline warmup·hot reload를 완성한다.
- [ ] 같은 입력·해상도에서 native effect 없음 / 기존 SkSL 효과 / 새 WGSL 효과를 비교한다. 효과가 다르면 성능 우열 비교로 보고하지 않는다.
- [ ] 각 플랫폼에서 first-use 비용, steady CPU/GPU frame cost, input capture 횟수, GPU copy/readback bytes, peak live texture/buffer bytes, retire backlog를 기록한다.
- [ ] 앱 종료·장치 재생성 후 live lease가 기준치로 복귀하는지 확인한다. async GPU 자원은 단순 관리 객체 Dispose 횟수로 판정하지 않는다.
- [ ] SDK/package/template/testbed/README에 실제 사용법·지원 프로파일·제약과 플랫폼별 증거를 반영한다.

통과 조건: G0~G9의 필수 플랫폼 기능·수명 검증이 통과해야 전체 완료다. **WebGL2 G7-B와 WebGL2 배경 효과·브라우저 검증도 포함**한다. 성능 수치가 없으면 `notMeasured`, 특정 장치/백엔드 실행이 없으면 `notVerified`를 유지한다. 구조만으로 기존 SkSL보다 빠르다고 주장하지 않는다.

### G10 — 후속 선택 범위

- [ ] D3D12/HLSL/DXIL 실행기의 필요성을 별도 평가한다. WebGL2는 이 후속 범위가 아니라 G7-B 필수 구현 대상이다.
- [ ] 범용 GPU buffer producer, 사용자 mesh/vertex pass, HDR 및 추가 storage format을 capability 단위로 확장한다.

이 단계는 기본 G0~G9 완료 조건에 포함하지 않는다. 앱이 선택하지 않은 backend로의 자동 전환은 추가하지 않는다.

## 6. 변경 예정 위치

| 위치 | 책임 |
| --- | --- |
| `tools/Doroti.Wgsl/` (신규 예정) | Naga 도구, CLI, reflection schema, golden, toolchain lock |
| `Doroti/src/Doroti.App.Sdk/Sdk/` | 에셋 item 수집, 공통 검증·생성 코드, application contract |
| `Doroti/src/Doroti.Runner.Sdk/Sdk/` | backend variant 생성/선택·publish·증분 캐시 |
| `Doroti/src/Doroti.Hosting/` | effect resource catalog, capability 등록, runner/app manifest 연결 |
| `Doroti/src/Doroti.Ui/` | 플랫폼 중립 program/parameters/scene payload 계약 |
| `Doroti/src/Doroti.Framework.Widgets/`, `Doroti.Framework.Rendering/` | 위젯, repaint, layer, bounds·semantics |
| `Doroti/src/Doroti.Skia.Rendering/` | 장면 실행 계획, capture/composite, Graphite frame segments |
| `Doroti/src/Doroti.Skia.Vulkan/`, 관련 Windows/Maui/Qt host | 같은 device/queue의 native pass와 자원 상태·수명 |
| Apple host 및 기존 native binding 프로젝트 | Metal shader library·texture·encoder 연결 |
| `Doroti/src/Doroti.Host.Web/` 및 `Web/` | Worker/Dawn/JS WebGPU와 Ganesh/WebGL2 효과 실행·GL 상태·자원 수명 연동 |
| `Doroti/validation/gpu-effects/` (신규 예정) | 계약·빌드·픽셀·수명·성능 fixture 및 결과 요약 |
| `DorotiTestbedApp/src/`, `assets/effects/` (후자는 신규 예정) | 실제 위젯 예제·효과 WGSL·definition |
| `Doroti/templates/Doroti.Templates/content/doroti-app/` | 신규 소비자 재현 예제 |

공통 실행기 인터페이스를 별도 assembly로 분리할지는 G1에서 의존성 방향을 보고 확정한다. Ui/Framework 프로젝트가 Vulkan·Metal·Rust 도구에 직접 의존하지 않게 한다.

## 7. 검증 설계와 증거 기준

| 축 | 필수 사례 | 판정 |
| --- | --- | --- |
| 컴파일·ABI | 구문 오류, binding 충돌, uniform padding, entry/stage 누락, feature 초과 | 원본 위치 진단과 byte layout 일치 |
| 빌드·배포 | clean/incremental/삭제/rename, 여러 runner, NuGet 소비, publish | stale/missing/중복 asset 0, variant 선택·hash 일치 |
| 입력 정확성 | identity, 채널 변경, 좌표 grid/방향 반전, 반투명 child | 기준 픽셀과 비교. identity/정수 연산은 exact, 부동소수 효과는 사전 정의 tolerance |
| 레이아웃·합성 | DPR, 회전/scale, rounded/path clip, halo, opacity/mask, z-order | 오프스크린 픽셀 + 실제 표시 캡처 |
| 고급 패스 | fragment→compute→fragment, ping-pong, history, storage buffer | 자원별 pass 추적·픽셀·동기화 validation |
| WebGL2 | GLSL ES 3.00 compile/link, sampler/UBO mapping, FBO 다중 패스·history, GL 상태 복구 | 실제 Worker/Ganesh 실행·표시, GL 오류/feedback loop 0, compute 요구 명시적 거부 |
| 수명 | parameter-only 변화, 외부 texture update, resize, cancel, dispose, device loss | stale generation 0, GPU 사용 중 조기 해제 0 |
| 플랫폼 | WindowsAppSdk/MAUI, Android, Linux, Apple device/simulator, Web Worker의 WebGPU·WebGL2 | 각 host/device/backend/browser의 실행 근거를 별도로 기록 |
| 비용 | cold/warm pipeline, 작은 패널/전체 화면/복수 효과, idle | CPU readback 0, copy/메모리/GPU 시간 측정 및 예산 준수 |

- 비교용 기준은 독립 CPU 수식 또는 직접 native GPU 패스 등으로 구성한다. 같은 잘못된 컴파일 결과를 양쪽에 사용해서 통과시키지 않는다.
- Vulkan validation, Metal GPU validation, WebGPU error scope를 활용하고 pipeline 생성뿐 아니라 실제 pass 실행을 관찰한다.
- WebGL2에서는 실제 compile/link 로그, framebuffer completeness, GL error와 context-loss 이벤트를 검사한다. 확장 지원 여부·GPU/브라우저 버전을 기록하고, GPU timing 확장이 없으면 CPU 시간을 GPU 시간으로 대신 표기하지 않는다.
- 처음 효과 장착 시 shader 준비 중 상태는 API로 정의한다. 준비되지 않은 효과를 성공으로 처리하거나 임의 투명 결과로 숨기지 않는다.
- GPU profiler/capture 또는 API 계측으로 외부 pass 실행을 확인한다. 파일에 `.spv`가 있다는 사실이나 draw counter만으로 검증을 대체하지 않는다.
- bounded 시나리오마다 보통 30회 이내의 frame/resize 반복으로 확인한다. 무의미한 수백 회 반복을 기본 검증으로 두지 않는다.
- 모든 빌드·테스트 프로세스는 `python Doroti/validation/run-with-timeout.py <command>`의 1,200초 process-tree 제한으로 실행하고 공유 `obj` 빌드는 직렬화한다.
- 정확한 새 검증 프로젝트명·명령은 해당 단계 구현 시 확정하여 기록한다. 존재하지 않는 프로젝트를 실행한 것으로 보고하지 않는다.
- 원시 캡처·로그는 삭제 가능한 `Doroti/artifacts/gpu-effects/`에 저장한다. 장치·버전·명령·결과·허용 오차·제약은 추적되는 `Doroti/validation/gpu-effects/` 요약에 남긴다.

현재 증거 상태: 소스 검토 **완료**, WGSL compiler **NOT_STARTED**, native effect executor **NOT_STARTED**, 플랫폼 GPU/표시 검증 **notVerified**, 성능·메모리 **notMeasured**. 기존 SkSL 필터 검증 결과를 새 WGSL 실행기의 결과로 재사용하지 않는다.

## 8. 공식 참고 자료

- [Naga 입력·출력 및 검증 도구](https://github.com/gfx-rs/wgpu/blob/trunk/naga/README.md): WGSL 입력과 SPIR-V/MSL/HLSL/GLSL 출력, 각 플랫폼의 추가 검증 도구를 구분한다.
- [WGSL 메모리 정렬·크기](https://www.w3.org/TR/WGSL/#alignment-and-size): host-shareable 데이터의 생성 serializer와 ABI golden 기준.
- [WebGPU shader module 생성](https://www.w3.org/TR/webgpu/#dom-gpudevice-createshadermodule): Web runner는 WGSL을 전달하고 브라우저가 module/pipeline을 생성한다.
- [WebGL 2.0 명세](https://registry.khronos.org/webgl/specs/latest/2.0/): GLSL ES 3.00, framebuffer·texture·uniform 및 context 제약의 기준. 일반 OpenGL/GLSL 지원과 구별한다.
- [Apple Metal libraries](https://developer.apple.com/documentation/metal/metal-libraries): Metal 소스 변환 이후 라이브러리 생성·로딩 단계를 분리한다.
- [DirectX Shader Compiler](https://github.com/microsoft/DirectXShaderCompiler): 선택 범위인 HLSL→DXIL 도구. 현재 Windows/Vulkan 효과 구현의 선행 의존성은 아니다.

공식 문서와 현재 체크아웃을 바탕으로 작성했다. 구체적인 compiler/SDK 버전과 지원 feature 집합은 G0/G1에서 고정하고 manifest 및 검증 결과에 함께 기록한다.

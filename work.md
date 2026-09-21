# Web 네이티브 소스 Texture 작업계획

작성일: 2026-09-22  
상태: **계획 작성 완료 / 아래 구현·검증은 미착수**  
범위: Doroti Web의 Canvas·Video·카메라 프레임을 기존 `Texture` 위젯으로 합성한다.

이번 요청의 산출물은 이 작업계획이다. 제품 코드 변경과 실행 검증은 수행하지 않는다. 기존 Android·Windows·Apple·Linux 텍스처 작업과 작업 트리의 변경사항은 유지한다.

## 1. 목표와 완료 기준

- `HTMLCanvasElement`, `HTMLVideoElement`를 등록하면 Doroti의 `textureId`로 표시할 수 있다.
- `VideoFrame`, `ImageBitmap`을 직접 공급하는 입력도 제공한다. 렌더 Worker가 소유한 `OffscreenCanvas`도 별도 입력으로 지원한다.
- 카메라는 우선 `getUserMedia()` → `HTMLVideoElement` 경로로 연결한다. WebCodecs 디코더 출력은 `VideoFrame` 입력을 재사용한다.
- 기존 `Texture`의 크기 지정, 필터링, 변환, clip, opacity, freeze, retained scene 동작을 유지한다.
- 프레임 도착은 raster 갱신을 요청한다. 새 프레임마다 위젯 트리를 다시 빌드하지 않는다.
- 앱이 픽셀을 `getImageData`, `readPixels`, `VideoFrame.copyTo`, PNG/Base64 또는 C# `byte[]`로 변환해 전달하는 경로를 만들지 않는다. 테스트용 화면 캡처는 별도로 구분한다.
- 기본 `worker-direct-webgpu`와 명시적으로 선택한 `worker-direct-webgl`에서 각각 검증한다. WebGPU 실패 시 자동으로 WebGL/CPU 경로로 바꾸지 않는다.
- 최신 프레임 대기, 소유권 이전, GPU 사용 완료, source 교체, unregister, view 종료를 하나의 수명 계약으로 관리한다.

**완료 판정:** P0–P6의 필수 게이트가 통과하고 재현 명령·증거·미검증 항목이 기록돼야 한다. JS 객체 생성, TypeScript 빌드, GPU 제출 횟수만으로 화면 표시 성공을 판정하지 않는다.

## 2. 현재 구현에서 확인한 연결 지점

| 파일 | 현재 상태와 후속 역할 |
| --- | --- |
| `Doroti/src/Doroti.Ui/TextureContracts.cs` | view 소유 `TextureRegistry`, CPU `TextureEntry`, native 등록 API가 있다. Web DOM 타입은 이 공통 계층에 넣지 않는다. |
| `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.Textures.cs` | texture ID 해석, external source 등록, freeze 전달, raster invalidation, texture revision을 제공한다. |
| `Doroti/src/Doroti.Skia.Rendering/SkiaExternalTexture.cs` | Web importer가 연결할 `ISkiaExternalTextureSource`와 등록 수명 계약이 있다. |
| `Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs` | 기존 texture capability를 노출하지만 Canvas/Video 입력 어댑터는 없다. |
| `Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.Graphite.cs` | Worker 소유 Dawn context/recorder, `CreateDawn`, recording submit 및 texture handle 반환 경로가 있다. |
| `Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs` | WebGL의 실제 `GRContext`와 framebuffer를 소유한다. |
| `Doroti/src/Doroti.Host.Web/Web/doroti.webgpu.ts` | Doroti 소유 `GPUDevice`/queue, `importJsTexture`, queue completion, native release 경로가 있다. |
| `Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts` | 프레임 admission, 렌더링, Worker 메시지 처리, context loss, shutdown을 연결한다. |
| `Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts` | 메시지 버전과 허용 kind를 검증한다. texture 메시지와 등록 세대 검증을 추가할 위치다. |
| `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts`, `doroti.web.worker-host.ts`, `doroti.web.managed-worker.ts` | main/Worker/managed 경계를 조사하여 실제 transferable 전달 통로를 확정한다. |
| `Doroti/src/Doroti.Target.Web.browser-wasm/build/DorotiSkiaInterop.js` | 브라우저 GPU 객체와 WASM native handle의 매핑·해제 계약을 확인한다. |

중요한 차이: 현재 Web Graphite 경로는 `SkiaGraphiteSession`의 native frame 수명 관리에 직접 올라가 있지 않다. 앞서 만든 `CreateNativeTexture()`를 노출하는 것만으로 Web GPU importer가 동작한다고 간주하지 않는다. Web의 실제 recorder/queue completion에 external source 수명을 연결한다. 광범위한 Web renderer 교체는 이번 범위에 넣지 않는다.

## 3. 입력 API와 소유권 설계

다음은 구현할 API의 **설계안**이며 기존 API가 아니다. 최종 명칭은 P0에서 확정한다.

```ts
const videoEntry = await textures.registerVideo(video);
const canvasEntry = await textures.registerCanvas(canvas);
canvasEntry.markFrameAvailable();

const producer = await textures.createFrameProducer();
producer.pushFrame(videoFrame); // 성공 시 소유권을 넘기는 계약

await videoEntry.dispose();
```

- registry는 owning view에 바인딩한다. 등록 응답의 `textureId`만 C# 위젯에 전달한다.
- DOM 요소, `VideoFrame`, `ImageBitmap`, GPU 객체는 JS/Worker 계층에 둔다. control 메타데이터와 transferable payload를 분리한다.
- C# `long` ID와 JS의 safe integer 범위를 혼용하지 않도록 검증한다. 기존 ID 발급 체계와 호환되는 wire 표현을 P0에서 확정한다.
- `registerVideo`: `requestVideoFrameCallback()`으로 새 프레임을 알리고 `VideoFrame`을 만들어 전달한다. 준비 전 영상, 일시정지, seek, ended, source 교체를 처리한다.
- `registerCanvas`: `markFrameAvailable()`을 명시적으로 호출한다. Canvas에는 범용 변경 감지 이벤트가 있다고 가정하지 않는다. 진행 중인 비동기 snapshot은 1개로 제한하고 추가 요청은 dirty 상태로 합친다.
- 기존 HTML Canvas의 그리기 컨텍스트를 뺏거나 `transferControlToOffscreen()`을 강제로 호출하지 않는다. 기본 어댑터는 `createImageBitmap(canvas)` snapshot을 사용한다.
- Worker 소유 `OffscreenCanvas`는 같은 owner에서 직접 copy할 수 있다. 다른 Worker에서 전달하는 경우 소유권 이전과 프레임 snapshot을 구분한다. `transferToImageBitmap()`은 backing image를 교체하므로 보존 동작을 기대하는 Canvas 어댑터의 기본값으로 사용하지 않는다.
- `pushFrame` 성공 이후 caller는 해당 객체를 사용하거나 닫지 않는다. 게시 실패, 세대 불일치, 등록 폐기 시 어느 쪽이 `close()`하는지 반환값/예외 계약과 테스트로 고정한다.
- 외부에서 빌린 video/stream은 기본적으로 일시정지하거나 track을 종료하지 않는다. Doroti 카메라 helper가 직접 만든 stream만 종료 시 `track.stop()`한다.

## 4. GPU 합성 방식

```text
main: HTMLVideoElement ── 새 프레임 ── VideoFrame ─┐
main: HTMLCanvasElement ── 명시적 갱신 ─ ImageBitmap ├─ transfer + credit ─┐
producer: WebCodecs ───────────────── VideoFrame ─┘                     │
                                                                      ▼
render Worker: 최신 pending 1개 → owning GPU device/context의 텍스처
                                         ↓
                                  SKImage → Texture 합성
                                         ↓
                                GPU 완료 → 자원 반환
```

### WebGPU: 1차 필수 경로

1. 렌더 Worker의 기존 `GPUDevice`에서 RGBA8 또는 BGRA8 텍스처를 생성한다. 포맷, Skia color type, sRGB, premultiplied alpha를 일치시킨다.
2. `copyExternalImageToTexture()`로 `VideoFrame`/`ImageBitmap`/owner-local `OffscreenCanvas`를 복사한다. source별 flip·crop·색 변환 정책을 명시한다.
3. pinned WebGPU 구현과 Graphite가 요구하는 `COPY_DST`, `TEXTURE_BINDING`, `RENDER_ATTACHMENT` 등의 usage를 확인하고 필요한 플래그만 설정한다.
4. 기존 `importJsTexture()` → `SKGraphiteBackendTexture.CreateDawn()` 경계를 확장해 sampling용 `SKImage`로 연결한다. native handle 참조 해제와 `GPUTexture.destroy()`는 별개의 수명으로 관리한다.
5. source copy와 Graphite sampling을 같은 device/queue 순서에 넣는다. freeze된 current texture와 이미 제출된 texture를 덮어쓰지 않도록 bounded pool 또는 명시적 last-use 규칙을 적용한다.
6. queue completion에 맞춰 retired image/texture를 반환한다. 매 프레임 `await onSubmittedWorkDone()`으로 생산·렌더링 전체를 직렬화하지 않는다.

`GPUExternalTexture`는 일반 `GPUTexture`와 타입·수명이 다르므로 현재 Dawn wrapper에 바로 넣지 않는다. `importExternalTexture(VideoFrame)` → 전용 WGSL sampling → 일반 RGBA texture 방식은 **후속 최적화 후보**로 둔다. 복사 횟수와 성능을 측정해 이점이 확인될 때 도입한다. 브라우저 내부 GPU 복사·업로드 여부나 하드웨어 디코딩은 API 선택만으로 보장하지 않는다.

### WebGL: 명시적 선택 경로

1. 기존 Worker `WebGL2RenderingContext`에서 `texImage2D`/`texSubImage2D`의 image source 경로를 사용한다.
2. Emscripten GL object table에 texture를 등록하고 실제 GL name과 Skia backend texture wrapper를 연결한다. 임의 숫자를 GL/native handle로 해석하지 않는다.
3. binding, active texture, unpack alpha/flip/color conversion 등의 상태를 관리하고 Skia의 context state cache를 적절히 무효화한다.
4. 다른 Canvas/context의 `WebGLTexture` 직접 공유는 지원한다고 가정하지 않는다.
5. source 소비 완료와 GPU texture의 마지막 sampling 완료를 구분한다. context loss에서는 정상 fence completion을 기다리며 무한정 정지하지 않는다.

## 5. 큐·프레임·종료 계약

- **상류부터 제한:** Worker 안의 pending만 1개로 해도 `postMessage` 큐가 쌓일 수 있다. 전송 중 frame credit/ACK와 main의 최신 후보 1개를 함께 관리한다. credit이 없을 때 video snapshot 생성을 생략하거나 기존 후보를 닫고 교체한다.
- **등록 세대:** view ID, registration generation, source generation, frame sequence를 검증한다. unregister/recreate 후 늦게 도착한 프레임과 snapshot Promise 결과는 표시하지 않고 반환한다.
- **프레임 소유권:** caller 소유 → transfer됨 → pending → copy/import 중 → consumer 소유 → retired/closed의 정상 경로와 rejected/dropped 경로를 정의한다. 디코더 pool 자원을 GC에 맡기지 않는다.
- **source와 destination 분리:** 입력 `VideoFrame`/`ImageBitmap`을 닫아도 되는 시점과 Skia가 사용하는 GPU destination을 파괴해도 되는 시점을 구분한다. source copy API의 수명 보장을 확인하고 불필요하게 디코더 프레임을 장기 보관하지 않는다.
- **freeze:** 마지막으로 표시한 GPU destination을 보존한다. 다음 입력은 무제한 쌓지 않는다. 처음부터 freeze한 경우 첫 유효 프레임을 얻는 기존 계약을 유지한다.
- **수정 알림:** frame arrival은 texture revision과 raster wakeup을 갱신한다. retained scene/PlatformView raster cache가 이전 source 이미지를 영구 재사용하지 않도록 확인한다.
- **크기 변경:** 새 storage generation을 만들고 이전 GPU allocation은 마지막 사용 완료 후 반환한다. 0 크기, device limit 초과, 과도한 메모리 요구는 명시적으로 거부한다.
- **예산:** source별 및 view 전체의 frame 수·GPU byte 한도를 설정한다. host의 in-flight 제한과 함께 계산하고, 예산 부족 시 새 입력을 drop/reject한다. 완료 미확인 texture를 재사용해 예산을 맞추지 않는다.
- **종료:** callback 취소 → 신규 전송 중지 → pending/늦은 프레임 close → GPU 사용 완료 또는 context/device loss 처리 → Skia/native handle 반환 → GPU 자원 해제 순서를 따른다.
- **복구:** 현재 host의 context/device loss 정책을 따른다. stale device handle을 새 context에서 재사용하지 않는다. host 재바인딩이 필요한 경우 그 상태를 노출하고 자동 renderer 전환은 하지 않는다.
- **입력 제약:** origin-clean/CORS, 영상 readiness, 지원되지 않는 source 타입, 자동재생 정책, 카메라 권한 거부를 구별해 보고한다. 외부 미디어 정책을 우회하지 않는다.

## 6. 단계별 작업과 게이트

### P0 — 타입·native bridge·owner 경계 확정

- [ ] 실제 main/managed Worker/render Worker 통로와 transfer list 전달 가능 지점을 추적한다. JSON control channel에 frame 객체를 넣지 않는다.
- [ ] pinned SkiaSharp/WASM의 Dawn image wrapper, GL texture 등록, release callback 및 GPU 완료 hook을 확인한다.
- [ ] browser 전용 capability/entry API, ID wire 형식, 오류 모델, 메모리 예산을 확정한다.
- [ ] `SkiaGraphiteSession` 재사용 여부를 판단하되 기존 Web 세션과 이중 recorder/device를 만들지 않는다.
- [ ] 정적 RGBA source 하나를 실제 Skia GPU 장면 안에 합성하는 작은 검증을 WebGPU·WebGL 각각 수행한다.

게이트 G0: 실제 제품 GPU 컨텍스트에서 source → native wrapper → SKImage sampling이 가능하고 참조 반환 경로를 설명할 수 있어야 한다. binding 제한을 발견하면 필요한 bridge 변경과 근거를 기록하고 해결한다. CPU 변환으로 성공을 대체하지 않는다.

### P1 — JS 등록 API와 bounded frame 전달

- [ ] 제안 파일 `Web/doroti.web.textures.ts`에 source 등록, frame producer, source 교체, dispose를 구현한다.
- [ ] 프로토콜 kind/버전 또는 capability 협상을 추가하고 main/Worker 양쪽을 함께 갱신한다.
- [ ] credit/ACK, 최신 후보, 비동기 Canvas snapshot coalescing, late-frame cleanup을 구현한다.
- [ ] 등록된 ID와 공통 `TextureRegistry` external source의 매핑을 연결한다.
- [ ] 타입 선언 및 호스트 정적 자산 패키징을 갱신한다.

게이트 G1: 느린 consumer, 게시 실패, source 교체, snapshot 완료 전 dispose에서도 상류/하류 큐가 제한되고 모든 전달된 frame이 정확히 반환된다.

### P2 — WebGPU/Dawn importer와 합성 수명

- [ ] 제안 `BrowserTextureEntry.cs` 및 `DorotiWebWorkerSurface.Textures.cs`에서 browser external source를 연결한다.
- [ ] `doroti.webgpu.ts`의 owning device에 source copy, texture pool, native import/release를 추가한다.
- [ ] 현재 Graphite submit/queue completion에 자원 retirement를 연결한다. canceled recording과 실패한 submit도 처리한다.
- [ ] texture-only invalidation, freeze, resize, retained scene, clip/transform/opacity를 연결한다.

게이트 G2: WebGPU Canvas·Video가 화면에서 갱신되며, freeze/resume/재생성과 GPU 완료 후 자원 반환이 pixel·진단 양쪽에서 확인된다. WebGPU validation 오류 0건.

### P3 — WebGL importer

- [ ] 동일 source/protocol을 WebGL texture upload와 Skia Ganesh wrapper로 연결한다.
- [ ] GL 상태 복원/무효화, texture name 소유권, sampling 완료 및 context loss 정리를 구현한다.
- [ ] backend별 capability와 실패 이유를 명시한다. 지원하지 않는 direct frame 입력은 명확히 거부한다.

게이트 G3: 명시적인 `worker-direct-webgl` 실행에서 Canvas·Video·카메라 기본 경로와 합성 동작이 통과한다. GL 오류 및 살아 있는 wrapper/texture 잔여가 없다.

### P4 — source 어댑터와 샘플

- [ ] Canvas 패턴: 방향을 구분할 모서리 색, 이동 도형, 반투명 영역, 크기 변경, 수동 갱신을 넣는다.
- [ ] Video: 저장소의 로컬 테스트 영상으로 재생/정지/seek/ended/source 교체를 확인한다.
- [ ] Camera: 사용자가 시작하는 액션에서만 접근을 요청한다. 생성한 stream의 소유권과 종료를 구현한다.
- [ ] 직접 `VideoFrame`/`ImageBitmap` 공급과 owner-local `OffscreenCanvas` 예제를 제공한다. WebCodecs 디코딩 예제는 codec capability를 확인한다.
- [ ] 기존 Texture 샘플에 Web source 선택과 Freeze/Resume, Recreate, Stop을 추가한다. 기술 진단값은 테스트/개발 화면에 둔다.

게이트 G4: Canvas·영상·카메라가 모두 같은 `Texture` 위젯 합성 경로를 사용한다. 일반 샘플 시작만으로 카메라 권한 요청이나 source 재생을 시작하지 않는다.

### P5 — 자동 검증과 Windows 브라우저 실행

- [ ] 소유권/프로토콜 계약 테스트와 실제 렌더링 테스트를 구분해 추가한다.
- [ ] Windows의 설치된 Chrome 또는 Edge에서 제품 WebGPU 및 명시적 WebGL을 실행한다. 브라우저 버전, GPU, 렌더러, origin isolation, 실행 자산 해시를 기록한다.
- [ ] 아래 매트릭스를 source/backend에 맞춰 실행한다. 기능이 없는 브라우저는 지원 실패/미검증으로 기록하고 테스트 통과로 바꾸지 않는다.
- [ ] 기존 CPU Texture 및 native ownership 계약 테스트를 실행해 공통 계층 회귀를 확인한다.
- [ ] native 앱·Android 에뮬레이터·Apple/Linux 실행은 이 Web 작업의 필수 실행 범위에 포함하지 않는다. 해당 코드를 변경해야 한다면 영향과 필요한 별도 검증을 먼저 계획에 반영한다.

게이트 G5: 필수 기본 경로는 실제 픽셀 검사까지 통과하고, 실패·종료 경로에서 pending/current/in-flight 및 frame close 계정이 일치한다. 물리 카메라 검증이 불가능하면 사유와 함께 해당 항목을 `notVerified`로 남기고 전체 카메라 완료를 선언하지 않는다.

### P6 — 문서·패키징·최종 보고

- [ ] `Doroti/docs/textures.md`와 validation 문서에 API, 소유권, backend 지원, 오류, CORS, 카메라 시작/종료 사용법을 추가한다.
- [ ] published testbed 및 패키지에 JS module/type declarations/native export가 누락되지 않았는지 확인한다.
- [ ] README 지원 상태와 진단 schema를 갱신한다.
- [ ] CSharpier, TypeScript/managed build, whitespace 검사 및 관련 게이트 결과를 기록한다.

게이트 G6: 구현 파일, 재현 명령, 실제 실행 증거, 미검증 범위를 연결한 보고서가 있어야 한다. 브라우저 GPU 입력 구현을 하드웨어 디코딩·zero-copy·물리 표시 지연 개선의 증거로 확대하지 않는다.

## 7. 검증 매트릭스와 진단

| 영역 | 필수 검사 |
| --- | --- |
| 픽셀/합성 | 모서리 색·방향, premultiplied alpha, clip, transform, opacity, 필터링, source 크기 변경, retained scene |
| 프레임 갱신 | widget rebuild 없이 갱신, 최초 blank, 최초 freeze, freeze 중 생산, resume에서 최신 프레임 선택 |
| source | Canvas 수동 dirty, snapshot 중복 요청, video pause/seek/ended/source 교체, direct frame 입력, 카메라 시작/종료 |
| backpressure | consumer 지연, 입력 burst, 전송 실패, 제한된 in-flight/pending, drop된 프레임 close, 예산 초과 |
| 소유권 | caller frame 사용 금지 시점, 소유권 이전 실패, 빌린 video/stream 보존, own camera track 종료 |
| 수명 | unregister, 빠른 recreate, 늦은 snapshot/ACK, view dispose, 숨김/복귀, context/device loss, late completion |
| 오류 | CORS/tainted source, 준비 전 video, 0 크기, 초과 크기, 폐기된 frame, 외부 view ID, 세대 불일치, 권한 거부 |
| 제품 통합 | publish된 앱에서 module 로딩, existing PlatformView와 texture 중첩, 키보드/포인터·기존 샘플 회귀 |

진단은 source별 `received / transferred / accepted / dropped / rejected / copied / drawn / closed`, main/Worker pending, live source 객체, current/in-flight GPU textures, 현재/최대 GPU bytes, context/source generation을 구분한다. main의 transferred 객체는 더 이상 main 소유가 아니므로 main과 Worker의 close 카운터를 단순 합산해 이중 해제로 오판하지 않는다.

앱이 수행한 CPU pixel copy/readback 호출 수를 계측하되 브라우저 내부 전송량으로 표기하지 않는다. 입력 fps, 표시 갱신, drop 수, queue depth는 각각 보고한다. 일반 스크린샷만으로 디코딩 가속이나 GPU completion을 증명하지 않는다.

## 8. 실행 명령과 증거 정책

모든 테스트는 `.github/copilot-instructions.md`에 따라 **20분 timeout**을 적용한다. 브라우저 테스트에도 동일한 process-tree deadline을 적용한다. 수백 회 반복하지 않고 보통 30회 이내의 명시적인 시나리오로 검증한다.

저장소 루트 기준 기존 명령:

```powershell
python Doroti/validation/run-with-timeout.py dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/Textures.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/NativeContracts/NativeContracts.csproj -c Release
git diff --check
```

Web 프로젝트 빌드가 기존 MSBuild TypeScript 경로를 실행하도록 한다. 별도의 임의 npm build 체계를 추가하지 않는다. 브라우저 자동화 runner와 fixture는 P5에서 실제 저장소 도구에 맞춰 추가하고, 아직 존재하지 않는 명령을 검증 완료 명령으로 기록하지 않는다. 로컬 서버는 제품에서 요구하는 COOP/COEP와 secure-context 조건을 유지한다.

증거는 `Doroti/artifacts/textures/web/<실행시각>/`에 저장하는 것을 기준으로 한다. report에는 source/backend, 브라우저/GPU, 자산 해시, pixel 결과, 오류 로그, frame 소유권·retirement 카운터를 넣는다. 실카메라 영상/캡처는 로컬 검증에만 사용하며 테스트 자산이나 저장소에 포함하지 않는다.

## 9. 이번 범위 밖의 후속 작업

- `GPUExternalTexture`를 직접 샘플링하는 최적화와 복사 경로 성능 비교.
- `MediaStreamTrackProcessor` 전용 카메라 입력: 브라우저별 노출 위치·호환성을 확인한 뒤 추가.
- HDR, wide gamut, protected/DRM 영상, 범용 YUV-plane 직접 import.
- 다른 GPU device/context에서 생성한 `GPUTexture`/`WebGLTexture`의 직접 공유.
- 임의 DOM/iframe의 texture화, cross-origin 콘텐츠 정책 우회.
- 독립적인 미디어 플레이어/오디오 동기화 시스템, 장시간 전력·메모리·물리 표시 지연 검증.
- 자동 backend fallback 및 현 Web runtime/renderer의 전면 교체.

## 10. 설계 참고

- [WebGPU 외부 이미지 복사](https://gpuweb.github.io/gpuweb/explainer/) — 일반 GPUTexture로 가져오는 1차 경로.
- [WebGPU ExternalTexture](https://gpuweb.github.io/gpuweb/#external-texture) — 일반 GPUTexture와 다른 binding·수명 및 복사 보장 한계.
- [WebCodecs](https://www.w3.org/TR/webcodecs/) — VideoFrame transfer와 미디어 자원 반환.
- [Video frame callback](https://wicg.github.io/video-rvfc/) — 영상 프레임 단위 알림.
- [WebGL 2 규격](https://registry.khronos.org/webgl/specs/latest/2.0/) — WebGL source upload·context 계약.

완료 표기 규칙: 미실행은 `notVerified`, 환경/드라이버 조건 미충족은 사유를 기재한다. 부분 성공을 전체 플랫폼 지원 완료로 표시하지 않는다.

# Web 차세대 구조 결정 및 자동 고성능/호환 모드 작업 계획

작성일: 2026-11-12

## 1. 목표와 결론

- 동일한 앱 URL과 배포본에서, 브라우저가 실제 cross-origin isolation 및 공유 메모리 조건을 만족하면 threaded 고성능 경로를 사용한다.
- COOP/COEP가 없거나 적용되지 않아 조건을 만족하지 못하면, 성능 저하를 허용하고 단일 스레드 호환 경로로 시작한다.
- **.NET 10 SDK와 .NET 11 SDK로 현재 `net10.0` Web 타깃을 빌드하는 경우를 모두 필수 검증한다.** SDK 11 사용과 대상 TFM/runtime의 `net11.0` 전환은 별개의 변경으로 다룬다.
- **차세대 제품의 지원 목표에 SDK 11 + `net11.0` + Mono 11의 양 모드도 포함한다.** SDK 11 + net10 검증만으로 .NET 11 runtime 지원을 선언하지 않는다. Worker 진입점, Emscripten/native ABI, Mono/CoreCLR 차이는 4.5절과 P-1에서 먼저 판정한다. CoreCLR WASM은 초기 제품 지원에서 제외한다.
- 헤더 부재는 예상된 환경 차이로 처리한다. 배포 누락, 잘못된 빌드 조합, 앱 예외까지 정상적인 호환 전환으로 숨기지는 않는다.
- 이번 작업은 **소스 검토 및 구현 계획 작성**이다. 런타임 코드 변경, 두 모드 빌드 및 브라우저 실측은 아직 수행하지 않았다.

**결정: 공통 렌더 세션 + threaded/compatible 이중 빌드 + 작은 Mono 10/11 연결 모듈을 채택한다.** 앱·렌더러·서비스 프로토콜은 공유하고, .NET 초기화/Worker 연결의 버전 차이만 분리한다. 런타임을 로드하기 전에 한 빌드를 선택하며 실행 중 runtime을 갈아 끼우지 않는다.

이는 아키텍처 결정이지 실행 검증 완료 선언이 아니다. 기존 threaded 경로를 안정 기준선으로 보존하고, 각 신규 프로필은 native 링크와 실제 첫 frame/입력/종료 검증을 통과한 뒤에만 배포한다.

### 1.1 확정한 차세대 구조

```text
Browser main: DOM / input / IME / semantics / browser services
  |
  +-- SessionHost: profile selection, lifecycle, bounded transport
        |
        +-- compatible
        |     standalone Render Worker [one Mono runtime + app + Skia + GPU]
        |
        +-- threaded
              Mono bootstrap adapter [runtime created on browser main]
                +-- JS-affine Render Worker [app + Skia + GPU]
                +-- runtime-owned deputy / GC / ThreadPool workers
                    (all share ONE managed runtime / shared heap)
```

`SessionHost`와 연결 모듈은 논리적 책임 이름이다. 새 범용 프레임워크나 플러그인 검색 시스템을 만들지 않고, 기존 [host](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts), [loader](packages/platforms/web/Doroti.Host.Web/Web/doroti.loader.ts), [managed worker](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts), [raster worker](packages/platforms/web/Doroti.Host.Web/Web/doroti.raster.worker.ts)의 경계를 정리한다.

확정 사항:

1. **앱/layout/paint와 GPU의 소유자는 렌더 Worker로 고정한다.** 메인 DOM shell에 앱 비즈니스 로직이나 렌더링을 옮기지 않는다. threaded의 main runtime은 부트스트랩과 런타임 관리에 사용한다.
2. **한 페이지·한 세션에 managed runtime 한 개만 둔다.** threaded의 여러 pthread를 여러 runtime으로 세지 않는다. 호환 모드는 독립 Worker 하나의 non-shared runtime이다. JS 전용 DOM/texture 보조 작업은 이 제한과 구분한다.
3. **앱의 실행 계약은 하나, runtime 연결 방식은 둘이다.** 앱 시작/입력/resize/frame/서비스/종료는 공통 경로로 처리한다. main-owned shared runtime과 standalone runtime의 차이는 연결 계층에서만 다룬다.
4. **실행 프로필과 renderer를 분리한다.** isolation은 shared-memory/threads 선택에 사용한다. GPU는 renderer capability와 검증된 native 계약으로 선택한다. 초기 출시에서는 threaded의 기존 Android WebGPU 우선 정책과 compatible의 WebGL2를 유지한다.
5. **Mono 10과 Mono 11은 명시적으로 검증한 연결 계약을 둔다.** SDK 번호로 추측하지 않고 실제 runtime/자산 metadata를 사용한다. CoreCLR/NativeAOT를 일반 fallback 후보로 넣지 않는다.
6. **성능 손실은 병렬 처리 제약에서 허용하되 결과 품질을 몰래 낮추지 않는다.** 같은 widget/layout/Skia 경로, 동일한 입력 순서와 접근성 의미를 유지한다. 고성능 효과는 실측하며 스레드가 모든 연산을 자동 병렬화한다고 주장하지 않는다.

### 1.2 채택하지 않는 대안

| 대안 | 결정과 이유 |
| --- | --- |
| 모든 .NET bootstrap/app/GPU를 하나의 독립 Worker에 통일 | 초기 제품 구조로 채택하지 않는다. `dotnetSidecar`는 upstream 내부 표식이며, threaded Mono는 deputy managed thread와 JS owner를 구분한다. Worker에서 `dotnet.create()`만 호출해도 managed paint와 GPU가 같은 owner가 된다는 보장이 없다. |
| sidecar에서 동기 호출 제한을 풀어 현재 paint를 그대로 실행 | 채택하지 않는다. `ThrowWhenBlockingWait` 등의 설정은 cross-thread GPU 소유권을 해결하지 않는다. 동기 JSExport가 다른 managed thread로 전달되면 JS/native GL 컨텍스트가 어긋날 수 있다. |
| main과 Worker에 .NET runtime을 각각 생성 | 기각한다. 앱 상태, native 객체, 메모리/GC 비용이 이중화되고 현재 shared-runtime 의미가 깨진다. |
| layout과 raster를 별도 managed runtime/Worker로 분할 | 이번 범위에서 기각한다. scene 직렬화·리소스 핸들·프레임 동기화·메모리 전송 비용을 새로 도입할 필요가 없다. 실제 병목이 입증될 때 별도 검토한다. |
| CoreCLR WASM을 곧바로 고성능 기본값으로 채택 | 기각한다. 현재 확인된 실험 상태와 다른 native/AOT/interop 계약을 검증 없이 제품에 도입하지 않는다. |
| 헤더를 만드는 Service Worker를 필수 배포 구성으로 사용 | 기각한다. 진정한 비격리 실행을 제공하지 못하고 최초 방문/호스팅/CSP 제약을 늘린다. 일반 HTTPS 정적 호스팅에서 compatible이 직접 시작해야 한다. |

따라서 차세대 구조의 목표는 '모든 것이 같은 스레드에서 부팅되는 것'이 아니라 **앱과 렌더 세션 계약은 같고, runtime 내부 구조 변화는 연결 계층에서 제한적으로 처리되는 것**이다.

## 2. 현재 구조에서 확인한 사실

| 영역 | 현재 동작 및 근거 | 필요한 작업 |
| --- | --- | --- |
| 기본 빌드 | [Web target props](packages/platforms/web/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props)는 `WasmEnableThreads=true`를 기본값으로 둔다. [Testbed 프로젝트](samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj)와 다른 샘플/템플릿에도 명시되어 있다. | JS 설정만 바꾸지 않고 non-threaded 산출물을 별도로 생성한다. |
| SDK/TFM 고정 | [루트 global.json](global.json)과 [제품 global.json](Doroti/global.json)은 SDK `10.0.400`, `latestPatch`, `allowPrerelease=false`다. [Web 계약 검사](packages/platforms/web/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets)는 TFM `net10.0`만 허용하고 [provider props](packages/platforms/build/Sdk.props)는 Web runtime `10.0.11`을 기본값으로 둔다. | SDK 11 설치만으로 SDK 11 빌드가 되지 않는다. 검증 job별 SDK 선택과 실제 TFM/runtime/pack 해석을 확인한다. |
| 기본 부트스트랩 | [loader](packages/platforms/web/Doroti.Host.Web/Web/doroti.loader.ts)는 `runtimeLocation` 미지정 시 `main`을 선택한다. [Sample2](samples/DorotiSampleApp2/web/src/doroti_bootstrap.ts), [템플릿](Doroti/templates/Doroti.Templates/content/doroti-app/web/src/doroti_bootstrap.ts)은 `main`을 고정한다. | 환경/배포 프로필 선택을 먼저 수행하고 샘플의 고정을 제거한다. |
| 고성능 런타임 | [managed worker](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts)는 `crossOriginIsolated`, `SharedArrayBuffer`, 실제 runtime threads 및 shared heap을 요구한다. [managed render thread](packages/platforms/web/Doroti.Host.Web/BrowserManagedRenderThread.cs)는 .NET 10의 JS-affine Worker 진입점을 사용한다. | 기존 검사를 유지한다. 격리 검사 삭제로 우회하지 않는다. |
| 기존 호환 경로 | [host](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts)는 `runtimeLocation="worker"`를 받으며, 빌드 프로필의 `threads=false`를 요구한다. [raster worker](packages/platforms/web/Doroti.Host.Web/Web/doroti.raster.worker.ts)는 Worker 안에서 독립 .NET 런타임을 시작할 수 있다. | 새 아키텍처 대신 이 경로를 우선 검증하고 자동 선택에 연결한다. |
| Worker 생성 | [worker host](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.worker-host.ts)는 비격리 환경에서 일반 module Worker를 만든다. 격리 환경에서는 COEP 상속을 위한 blob Worker 경로도 있다. | 비격리 상태에서도 Worker 자체는 사용 가능하다. CSP/실제 브라우저 지원은 별도 검증한다. |
| owner/async | [BrowserOwnerSynchronizationContext](packages/platforms/web/Doroti.Host.Web/BrowserOwnerSynchronizationContext.cs)가 standalone Worker의 JS event-loop context를 제공한다. [runner](packages/platforms/web/Doroti.Target.Web.browser-wasm/DorotiWebWorkerRunner.cs)가 이를 설치한다. | continuation, 타이머, 해제 경로가 owner를 유지하는지 검증한다. |
| 렌더러 선택 | [renderer policy](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.policy.ts)는 Android에서 WebGPU 우선, 나머지에서 WebGL2 우선이다. WebGPU 자동 선택의 사전 조건이 안 맞으면 WebGL2로 전환한다. | 런타임 선택과 렌더러 선택을 분리한다. WebGL2 선택만으로 threads 의존성이 사라지는 것은 아니다. |
| WebGPU 제한 | [WebGPU 초기화](packages/platforms/web/Doroti.Host.Web/Web/doroti.webgpu.ts)와 raster worker는 현재 격리된 main-owned shared runtime을 요구한다. | 최초 호환 모드는 WebGL2로 한정한다. 이는 현재 Doroti 구현 제약이지 WebGPU 표준 전체의 COOP/COEP 요구사항이라는 뜻은 아니다. |
| 빌드 프로필 | [provider SDK targets](packages/platforms/build/Sdk.targets)의 `DorotiWriteWebRuntimeProfile`이 `schemaVersion: 1, threads: boolean`인 단일 프로필을 생성한다. | dual 배포 목록, 선택된 런타임 URL, 버전 일관성 계약이 필요하다. |
| 산출물 경로 | host는 document base/import map에서 단일 `_framework/dotnet.js`를 찾는다. raster worker는 모듈 위치의 `../../`를 앱 리소스 기준으로 사용한다. | runtime asset base와 application base를 구분해야 한다. 폴더만 옮기면 폰트/플러그인/라우팅이 깨질 수 있다. |
| preload/fingerprint | [Testbed HTML](samples/DorotiTestbedApp/web/wwwroot/index.html)에 SDK가 채우는 framework preload/import map placeholder가 있다. [publish 검사](packages/platforms/web/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets)는 단일 root framework를 전제로 한다. | 선택 전 양쪽 runtime 다운로드를 피하고 dual 경로를 publish 검사에 반영한다. |
| 개발/배포 | [devserver](packages/platforms/web/devserver/Program.cs)는 `--apply-cop-headers`가 있으면 헤더를 강제한다. [Nginx](tools/doroti-cloudflared/nginx.conf)와 [tunnel](tools/doroti-cloudflared/tunnel.ps1)은 격리 헤더 및 단일 framework/pthread 자산을 전제로 한다. | 기존 고성능 기본값을 유지하면서 비격리 검증 경로와 dual 산출물 검사를 추가한다. |

추가 확인:

- 현재 로컬에 설치된 `SkiaSharp.NativeAssets.WebAssembly 4.154.0`의 targets는 `WasmEnableThreads`에 따라 `mt`/`st` native archive를 선택한다. 실제 `mt`, `mt,simd`, `st`, `st,simd` 디렉터리도 존재한다. 이는 호환 빌드 가능성을 뒷받침하지만 **현재 앱의 링크/AOT/실행 성공을 입증하지는 않는다**.
- [Skia interop](packages/platforms/web/Doroti.Target.Web.browser-wasm/build/DorotiSkiaInterop.js)는 emdawnwebgpu ABI에 의존한다. 호환 모드가 WebGL2라고 해서 첫 단계에서 Dawn 링크 라이브러리나 exports까지 제거하지 않는다.
- [Testbed bootstrap](samples/DorotiTestbedApp/web/src/doroti_bootstrap.ts)은 텍스처 샘플 초기화를 `main`일 때만 호출한다. [WebTextureSources](samples/DorotiTestbedApp/web/WebTextureSources.cs)를 비롯한 런타임 소유권 의존 초기화는 호환 모드에서 기능이 빠지지 않도록 정리해야 한다.
- 현재 추적되는 Web 전용 TS/브라우저 테스트 러너는 확인되지 않았다. 기존 테스트 이름이나 로컬 `bin`/`obj` 잔여물만 보고 실행 가능한 테스트 프로젝트라고 가정하지 않는다.
- 작성 시점 로컬 SDK는 `10.0.403`과 `11.0.100-rc.3.26522.148`이다. SDK 11 RC의 manifest에는 `wasm-tools-net10`, `wasm-experimental-net10`이 있고 net10 WASM/AOT pack은 `10.0.14`, Emscripten은 `3.1.56` 계열로 선언되어 있다. manifest 존재는 workload 설치 완료나 빌드 성공의 증거가 아니며, 이 RC 결과를 다른 SDK 11 릴리스의 검증 결과로 확대하지 않는다.

## 3. 설계 원칙

### 3.1 한 WASM 바이너리의 threads를 실행 중 끌 수는 없다

Emscripten pthread 빌드는 공유 메모리 전제 자체가 다르다. threaded 앱을 먼저 로드한 뒤 실패하면 같은 바이너리를 단일 스레드로 실행하는 방식은 사용할 수 없다.

- `WasmEnableThreads=true/false`로 **런타임, 앱 어셈블리, native Skia를 포함한 각각의 완결된 빌드**를 만든다.
- 두 결과의 `_framework` 파일을 덮어쓰거나 섞지 않는다.
- 한 페이지에서는 선택된 managed runtime 한 개만 생성한다.
- 호환 모드도 Release AOT 사용을 검증한다. non-threaded와 non-AOT는 다른 축이며, 호환성을 이유로 AOT/SIMD를 일괄 비활성화하지 않는다.

### 3.2 모드별 계약

| 항목 | 고성능: `threaded` | 호환: `compatible` |
| --- | --- | --- |
| WASM 빌드 | `WasmEnableThreads=true` | `WasmEnableThreads=false` |
| 런타임 소유자 | 기존 `runtimeLocation="main"` | 기존 `runtimeLocation="worker"` |
| managed UI/Skia 실행 | main-owned 런타임의 JS-affine render Worker | 독립 Worker 내 단일 managed 실행 스레드 |
| 메모리 | shared WASM heap | non-shared WASM heap |
| 렌더러 | 기존 Android WebGPU 우선/기타 WebGL2 우선 정책 유지 | WebGL2 |
| DOM/input/platform 연동 | 기존 Worker 메시지 브리지 | 동일 브리지 재사용 |
| 기대 성능 | 기존 병렬 처리 가능 경로 보존 | 앱/layout/paint/일부 작업이 같은 Worker event loop를 공유하므로 지연 증가 가능 |
| 장애 복구 | 기존 page restart | 기존 Worker session restart; 메모리 상태 보존은 보장하지 않음 |

`main`은 런타임 소유권이며, 화면 렌더링을 DOM 메인 스레드에서 한다는 뜻이 아니다. 호환 모드에서도 DOM 메인 스레드와 Worker는 분리된다.
또한 고성능 모드가 모든 장치/화면에서 반드시 더 빠르다는 보장은 없다. 기존 성능을 유지하는 경로로 정의하고 차이는 실측한다.

### 3.3 공통 렌더 세션과 소유권

- 기존 [Worker endpoint](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts)의 `postMessage`/event/종료 계약을 공통 세션으로 감싼다. standalone은 소유한 Worker, threaded는 runtime-owned Worker와 연결된 MessagePort를 제공한다. 호출자가 transport 종류에 따라 앱 로직을 분기하지 않게 한다.
- [프로토콜 v5와 상태 머신](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.protocol.ts), 요청 mailbox, 입력/resize admission, texture lease를 재사용한다. handshake 또는 메시지 shape가 바뀌면 프로토콜 버전을 함께 올리고 구버전 메시지를 명시적으로 거부한다.
- 세션 식별자/세대는 host가 발급한다. 늦은 이전 세션 응답은 현재 상태에 반영하지 않으며, pending 요청은 원인과 함께 종료하고 전달된 자원은 회수한다. 진단에는 버린 응답의 이유를 남긴다.
- 앱 초기화는 render owner에서 정확히 한 번 수행한다. [Testbed의 main-only 초기화](samples/DorotiTestbedApp/web/src/doroti_bootstrap.ts)처럼 main에서 임의로 assembly export를 호출하는 패턴은 공통 managed 시작 경로로 옮긴다. DOM 전용 작업은 기존 서비스 브리지를 통한다.
- CPU 작업을 offload할 때에는 managed 결과만 owner dispatcher로 되돌린다. `SKSurface`, GL context, Dawn handle, `JSObject` 등 owner-affine 객체를 ThreadPool로 넘기지 않는다. [standalone synchronization context](packages/platforms/web/Doroti.Host.Web/BrowserOwnerSynchronizationContext.cs)를 다중 스레드 context인 것처럼 재사용하지 않는다.
- main↔render 통신은 양 모드 모두 기존 메시지/transferable transport를 사용한다. shared heap은 .NET runtime 내부에 두고 DOM shell이 managed 포인터를 직접 읽거나 별도 SAB ring buffer를 관리하지 않는다. 매 프레임 scene 전체/bitmap을 main으로 복사하는 새 경로도 만들지 않는다.
- 입력 move/resize는 기존 admission 방식으로 최신 값 합치기를 유지하되 pointer down/up/cancel, keyboard/IME, lifecycle 경계 이벤트의 순서는 보존한다. 서비스·texture queue는 유한하고 포화/실패가 드러나야 한다. 호환 모드가 느리다는 이유로 대기열을 무제한으로 늘리지 않는다.

### 3.4 자동 선택 순서

1. 경량 부트스트랩에서 배포 manifest의 schema/host 계약/배포 세대/검증된 runtime 연결 계약을 확인한다. 미지원 runtime flavor는 capability fallback이 아니라 명시적인 구성 오류다.
2. `globalThis.isSecureContext`, `globalThis.crossOriginIsolated === true`, `typeof SharedArrayBuffer` 및 필요한 shared WebAssembly memory 생성 가능 여부를 확인한다.
3. Worker/module Worker, OffscreenCanvas, `transferControlToOffscreen`, 선택 렌더러의 지원 여부를 확인한다. WebGL2 검사는 실제 visible canvas를 소비하지 않는 사전 검사와 Worker 초기화 검사를 구분한다.
4. `auto`에서는 shared-memory 조건을 만족하면 threaded, 그렇지 않으면 compatible을 선택한다.
5. 선택된 런타임을 기준으로 기존 renderer policy를 해석한다. 호환 경로의 자동 렌더러는 WebGL2다.
6. 선택된 runtime의 URL/프로필/app base를 연결 모듈에 전달한다. 그 뒤에만 선택된 .NET을 import/create한다. threaded는 render role/port를 연결하고 compatible은 독립 Worker를 시작한다.
7. 실제 runtime 버전, `wasmEnableThreads`, heap 종류, render Worker의 isolation/owner 및 GPU capability를 확인한다. main의 WebGPU probe 성공만으로 Worker의 지원을 확정하지 않는다.
8. render endpoint가 준비되면 visible canvas를 해당 세션에 한 번 이전하고 앱을 시작한다. 실제 frame commit 후 splash를 해제한다. 초기화 후 다른 runtime/renderer로 몰래 재시작하지 않는다.

헤더 문자열을 별도로 fetch하여 판단하거나 user-agent만으로 고성능 모드를 결정하지 않는다. 헤더가 있어도 Permissions-Policy, iframe 상위 문서, 비보안 컨텍스트 등에 의해 실제 isolation이 꺼질 수 있다. COEP `credentialless`도 실제 isolation 조건을 만족하면 배제하지 않는다.

### 3.5 명시적 설정과 오류 처리

아래 이름은 **구현할 계약으로 결정한 이름이며 아직 구현된 옵션이 아니다**.

- 빌드/배포: `DorotiWebExecutionMode=auto|threaded|compatible`.
  - `auto` publish: 두 빌드를 포함하는 배포본.
  - `threaded`/`compatible` publish: 용량이나 배포 환경이 명확한 경우의 단일 모드 배포본.
- loader 요청: `executionMode: "auto" | "threaded" | "compatible"`.
- 진단 출력: 요청 모드, 선택 모드, 선택 이유, runtime 계약/버전, 실제 isolation/threads/heap, renderer, build ID, 세션 세대, runtime-ready와 first-frame 시각.
- 앱에서 runtime bootstrap 위치를 직접 선택하는 신규 API는 추가하지 않는다. 기존 `runtimeLocation`은 legacy 호환에만 남기고, 신규 코드에서는 실행 모드가 내부 소유권을 결정한다.

정책:

- 기존 `runtimeLocation` 지정은 마이그레이션 중 명시적 legacy 요청으로 보존한다. 새 요청과 충돌하면 오류로 표시하고 임의 덮어쓰기는 하지 않는다.
- 강제 `threaded`인데 격리/공유 메모리 조건이 부족하면 필요한 조건을 안내한다. 강제 요청을 조용히 무시하지 않는다.
- 강제 `compatible`은 격리 환경에서도 compatible을 사용하여 재현/비교가 가능해야 한다.
- `compatible`과 명시적 `worker-direct-webgpu`의 조합은 현재 미지원임을 명확히 알린다. 기존 명시적 renderer override 의미를 유지한다.
- 단일 모드 배포는 manifest에 그 사실을 표시한다. compatible-only + auto는 이유를 기록하고 compatible을 선택한다. threaded-only를 비격리 환경에 배포하면 dual/compatible 재배포가 필요함을 알린다.
- dual manifest가 약속한 파일 누락/404, schema 오류, 세대 불일치, CSP 차단, 앱 예외는 배포/실행 실패다. 임의의 다른 모드를 시도하며 성공처럼 처리하지 않는다.
- 런타임이나 GPU 생성 이후의 오류를 자동 backend/runtime 교체로 복구하는 기능은 이번 범위에서 제외한다. canvas 소유권과 shared runtime 수명을 고려한 기존 복구 정책을 유지한다.
- 모드 선택은 페이지 시작 시 한 번 수행한다. Worker 재시작은 같은 선택을 사용하고 페이지 재시작은 환경을 다시 평가한다. 자동 선택 결과를 강제 URL 옵션으로 굳혀 다음 시작을 방해하지 않도록 요청값과 결과값을 구분한다.

### 3.6 실패·종료·재시작 결정

- 종료는 입력 중지 → 진행 중 frame/service 요청 종료 → GPU/texture/native 해제 → endpoint 종료 순으로 공통화하되, transport별 실제 종료 동작은 분리한다.
- standalone Worker는 정리 완료를 기다린 뒤 종료할 수 있다. threaded renderer의 native pthread를 직접 `terminate()`하지 않고 기존 dispose/role 완료 경로를 사용한다.
- threaded runtime이 이미 생성된 페이지에서는 다른 managed runtime을 새로 만들지 않는다. 복구에는 전체 page reload를 사용한다. compatible은 정리된 새 Worker 세션을 허용하되 기존 제한된 재시도와 실패 표시를 유지한다.
- 타임아웃/컨텍스트 손실에서 확인되지 않은 GPU/native 자원을 먼저 해제해 use-after-free를 만들지 않는다. 정리 실패를 노출하고, 직렬화한 앱 상태만 복원한다. 런타임 내부 메모리의 무중단 이전은 제공하지 않는다.

## 4. 빌드/배포 구성안

### 4.1 논리적 산출물 배치

다음은 구현 목표 배치다. fingerprint 파일명은 SDK가 생성한 실제 결과에서 얻는다.

```text
wwwroot/
  index.html
  doroti_bootstrap.js
  doroti.runtime-profile.json       # v2: build ID + variants + 실제 loader URL
  _content/Doroti.Host.Web/         # 두 빌드에서 동일함을 확인한 공통 host 자산
  assets/, fonts/, locales/, plugins/  # 앱이 실제 선언한 공통 자산
  runtimes/
    threaded/
      _framework/                  # threaded 전체 framework 산출물
      doroti.runtime-profile.json  # 이 variant의 빌드 정보
    compatible/
      _framework/                  # non-threaded 전체 framework 산출물
      doroti.runtime-profile.json
```

- 기존 v1 단일 profile 소비는 유지하고 v2 목록은 명시적으로 해석한다. v1 배포에 존재하지 않는 대체 런타임을 추측하지 않는다.
- manifest v2에는 `hostContract`, `buildId`, `runtimeContract`와 variant별 threads, loader/profile URL, 지원 renderer를 담는다. runtimeContract는 검증된 Mono 10/11 연결 모듈을 선택하는 닫힌 식별자이며 임의 script/plugin URL을 받아 실행하는 확장점이 아니다. SDK/TFM/runtime flavor·버전/workload/toolchain 식별 정보는 provenance로 기록하고 두 variant의 일관성을 검사한다. 이름은 제안 필드명이며 구현 시 정적 타입과 schema 검사를 함께 정의한다.
- manifest의 loader/profile URL은 앱 배포 기준으로 해석하며 초기 지원은 same-origin으로 한정한다. SDK가 생성한 실제 fingerprint/boot metadata를 사용하고 Worker/boot 파일명을 추측하지 않는다. .NET boot manifest 자체를 재구현하거나 패키지 캐시·생성 runtime JS를 패치하지 않는다.
- 공통 자산은 같은 source/configuration의 두 산출물을 비교한 뒤 합친다. 동일 경로의 내용이 다르면 덮어쓰지 말고 빌드 실패 또는 명시적인 variant 분리로 처리한다.
- 먼저 두 개의 완전한 publish 결과를 생성한 뒤 병합한다. 최종 배포에서 선택하지 않은 `_framework`는 다운로드/instantiate하지 않는다.
- 순환 publish를 막는 내부 child-build 구분과 두 자식 빌드 실패 전파가 필요하다. 한쪽만 성공한 결과로 최종 manifest를 발행하지 않는다.
- 초기에는 순차 publish로 충돌 위험을 줄인다. 공통 캐시 공유/병렬 publish 최적화는 올바른 출력 분리 이후 검토한다.

### 4.2 캐시와 출력 격리

- SDK 버전/TFM/runtime flavor/모드별 `ArtifactsPath` 또는 SDK 호환 출력 경로를 사용해 restore/intermediate/native link/AOT/WebCIL/static-web-assets/publish 결과를 분리한다. workload/runtime pack, native toolchain 또는 WebCIL 형식이 달라지면 캐시도 무효화한다. 최종 `PublishDir`만 다르게 지정하는 것으로 끝내지 않는다.
- 프로젝트 참조, generated bootstrap, TypeScript 출력, static-web-assets manifest가 다른 모드 경로를 덮어쓰지 않는지 확인한다.
- [Web build targets](packages/platforms/build/Doroti.Web.targets)는 이미 WebCIL identity에 `WasmEnableThreads`를 포함한다. 이를 유지하되 이것만으로 두 빌드의 모든 native/intermediate 충돌이 해결됐다고 판단하지 않는다.
- `DorotiWebInterpretWidgetTree`, trimming, AOT, SIMD, heap 옵션은 variant에 의도대로 전달하고 manifest/진단에서 확인 가능하게 한다. 기존 Safari 관련 설정을 누락하지 않는다.
- HTML/선택 manifest는 재검증하도록 하고 fingerprint 자산은 장기 캐시할 수 있게 한다. 두 모드를 한 배포 세대로 교체하며 구세대 클라이언트가 필요한 파일을 즉시 잃지 않게 배포한다.

### 4.3 경로와 preload

- 앱 리소스 기준인 `applicationBaseUrl`과 runtime 파일 기준 URL을 분리하여 Worker 초기화 및 재시작에도 전달한다.
- 모드를 고르기 위해 `document.baseURI`나 앱 라우트 자체를 `runtimes/...`로 바꾸지 않는다. asset/font/localization/plugin/API 상대 URL 및 history/navigation 의미를 유지한다.
- `resolveCurrentDotnetModuleUrl`, profile fetch, Worker의 `../../` 기반 리소스 해석을 선택 descriptor에 맞게 정리한다.
- .NET loader의 상대 모듈, boot resource, native worker, compressed asset URL이 선택한 variant 안에서 끝까지 해결되는지 검증한다. DLL/WebCIL/native WASM 일부만 골라 복사하지 않는다.
- SDK가 주입하는 import map/fingerprint 및 preload를 dual 구조에 맞춰 생성한다. 공통 모듈 매핑은 유지하되 threaded runtime preload가 비격리 페이지에서 먼저 시작되지 않게 한다.
- Worker entry는 document import map을 자동으로 따르지 않으므로 기존 fingerprint 처리와 별도로 실제 선택 URL을 검증한다.
- `/`뿐 아니라 `/demo/` 같은 subpath, deep link 새로고침에서도 같은 계약을 검증한다. missing framework/profile을 SPA HTML로 돌려주지 않는다.

### 4.4 .NET 11 SDK 빌드 지원

**SDK 빌드 호환성의 필수 목표는 SDK 11 + `net10.0` + 두 실행 모드다. 이것만으로 .NET 11 WASM 구조를 검증한 것은 아니다.** SDK major만 보고 runtime을 11로 올리거나 renderer/threads를 바꾸지 않는다. 브라우저는 빌드에 사용한 SDK가 아니라 실제 capability와 배포 프로필로 실행 모드를 선택한다.

#### SDK 선택과 재현성

- SDK 10/11 검증은 별도 CI job/격리 checkout에서 정확한 SDK 버전을 선택한다. 루트와 제품 하위의 SDK pin, CLI 작업 디렉터리 및 MSBuild resolver가 서로 다른 SDK를 고르지 않도록 확인한다.
- SDK 11 RC 검증에서는 해당 job에서만 prerelease를 명시적으로 허용한다. 공용 작업 트리의 SDK pin을 무조건 `latestMajor`로 바꾸어 다른 플랫폼 빌드까지 바꾸지 않는다. 이 계획 작성 단계에서는 SDK 설치나 pin 변경을 하지 않는다.
- `dotnet --version`, 평가된 `NETCoreSdkVersion`, TFM, `RuntimeFrameworkVersion`, workload-set/manifest, WASM SDK/runtime/AOT/Emscripten pack의 실제 버전을 기록한다. SDK 11이 설치되어 있다는 확인만으로 SDK 11 검증을 완료하지 않는다.
- child publish와 별도 프로세스로 실행되는 devserver 빌드에도 동일한 SDK 선택을 적용한다. SDK 10의 threaded와 SDK 11의 compatible 산출물을 임의로 합쳐 한 dual 배포본을 만들지 않는다.

#### workload 및 패키지 조합

- SDK 11에서 `net10.0`을 빌드할 때 해당 SDK의 `wasm-tools-net10` 계약과 threaded pack의 `wasm-experimental-net10` 필요 여부를 실제 restore/build 진단으로 확인한다. SDK 10에 설치된 `wasm-tools`나 SDK 11의 current `wasm-tools`만으로 충분하다고 가정하지 않는다.
- 현재 runtime/Blazor 패키지 pin `10.0.11`과 로컬 SDK 11 RC manifest의 net10 pack `10.0.14`가 다르므로, restore와 native/AOT 링크가 해석하는 실제 조합을 P0에서 확인한다. 숫자가 다르다는 이유만으로 즉시 실패라고 단정하거나 pin을 일괄 제거하지 않는다.
- 지원 가능한 조합을 명시적으로 선택하고, pack/runtime 불일치가 확인되면 관련 버전만 함께 조정한다. AOT 비활성화나 compatible 강제 선택으로 SDK 11의 threaded 빌드 실패를 숨기지 않는다.
- Skia의 `mt`/`st` archive, Emscripten/LLVM, emdawnwebgpu JS ABI 및 Doroti interop를 양 모드에서 검증한다. SDK 11 + net10이 반드시 net11 native toolchain을 쓰는 것은 아니므로 SDK major만으로 라이브러리를 분기하지 않는다.

#### SDK/runtime 내부 계약 검증

- [Web targets](packages/platforms/build/Doroti.Web.targets)의 `_PrepareForBrowserWasmBuildNative`, `_WasmAotCompileApp`, `_WasmAssembliesInternal`/`AOT_InternalForceToInterpret`, `_ResolveWasmConfiguration`, WebCIL 무효화 hook이 SDK 11에서 실제 선택된 WASM targets에도 유효한지 binlog와 산출물로 확인한다. build 성공만으로 Safari mixed-AOT 설정이 적용됐다고 판단하지 않는다.
- [native interop 연결](packages/platforms/build/Sdk.targets)의 `_BrowserWasmWriteRspForLinking`/`_BrowserWasmLinkDotNet`, Debug stack workaround도 실제 toolchain 기준으로 확인한다. 차이가 발견된 지점만 작은 호환 분기로 처리하며 SDK 전체 복제/생성 runtime 패치는 하지 않는다.
- SDK 11의 static-web-assets/import map/preload/압축 산출물에 맞춰 loader URL을 발견한다. 기존 publish 검사의 fingerprint 10자리 패턴이나 `dotnet.native.worker*.mjs` 가정이 유지된다고 단정하지 않는다.
- [managed render thread](packages/platforms/web/Doroti.Host.Web/BrowserManagedRenderThread.cs)의 비공개 `JSWebWorker.RunAsyncVoid`, JSExport/JSImport, `getDotnetRuntime` 및 shared-port handshake는 **실제 탑재 runtime** 기준으로 검증한다. SDK만 11로 바꾸었다는 이유로 .NET 10 runtime 진입점을 무조건 교체하지 않는다.
- Debug/Hot Reload/devserver와 Release/AOT를 각각 검증한다. framework pack 누락이나 사라진 hook을 무시하여 일부 기능만 빠진 성공 산출물을 만들지 않는다.

#### `net11.0` 제품 지원 목표와 전환 원칙

- 이번 차세대 설계에서 `net11.0`/Mono 11을 지원 목표로 결정한다. 다만 현재 기본 `net10.0` 계약을 즉시 제거하거나 저장소 전체 패키지를 일괄 11로 올리지는 않는다.
- target descriptor/검사, Web runner/host 경계, runtime/Blazor 패키지, manifest/템플릿을 일관되게 정리하고 SDK 11 + net11 + 두 모드를 별도 자격 검증한다. 기존 검사 한 줄만 완화하는 것으로 완료하지 않는다. 공통 net10 라이브러리는 호환되는 한 그대로 참조하며 Web 지원 때문에 모든 플랫폼 TFM을 바꾸지 않는다.
- 이때 JS-affine Worker API, Mono/runtime backend, native/AOT/Skia ABI 및 loader 구조를 다시 확인한다. .NET 10의 private API가 그대로 있다고 가정하거나 동작하지 않을 때 자동 compatible 전환으로 은폐하지 않는다.
- 기본 TFM 변경은 net11 인수 검증 통과 뒤 별도 배포로 적용한다. 그전에는 net10 기본값을 유지한다. net11이 차단되면 net10 호환 기능만 별도 전달할 수 있지만 이를 차세대 net11 지원 완료라고 부르지 않는다.

### 4.5 .NET 11 WASM 구조 재검토 결과와 설계 차단 항목

2026-11-12 추가 검토. 이전 계획은 SDK 11 + net10과 일반적인 ABI 확인 항목에 집중했으며, 아래 실제 구조 변경을 구체적으로 반영하지 못했다. 근거는 upstream `v10.0.11`/`v11.0.0` 소스, 공식 문서, 설치된 SDK 11 RC targets/manifest다. **앱 실행 재현 결과와는 구분한다.**

| 구분 | 확인된 차이 | Doroti 영향과 조치 |
| --- | --- | --- |
| Mono pthread 진입점 | .NET 10은 `js-module-threads` 자산 URL에서 Worker를 만든다. .NET 11은 loader의 `import.meta.url`에서 얻은 `scriptUrl`, 즉 선택된 `dotnet.js`로 module Worker를 생성한다. 초기 preload와 runtime config 전달 경로도 loader에 있다. | [managed worker](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts)는 `dotnet.native.worker` 접두사와 `.mjs` 확장자로만 Worker를 식별한다. .NET 11에서는 이 필터를 통과하지 않아 `doroti-managed-port` 리스너가 연결되지 않는 문제가 정적 분석상 확인된다. 기존 main-runtime 경로를 그대로 재사용하면 연결 timeout으로 이어질 수 있다. |
| native toolchain | .NET 11 `v11.0.0`의 Emscripten 버전은 `6.0.2`다. SDK 11로 net10을 빌드하는 경로는 로컬 manifest 기준 여전히 `3.1.56`이다. | 로컬 SkiaSharp 4.154.0 targets는 net9 이상에도 `3.1.56` archive를 선택한다. net11에 같은 archive를 넣어도 된다고 가정하지 않는다. C/C++ ABI, EH, linker flags, emdawnwebgpu, JS library/export 호환성을 확인하고 필요하면 지원 패키지 또는 재빌드한 native 자산을 사용한다. 실제 링크 실패까지 확인한 것은 아니다. |
| Mono와 CoreCLR | 설치된 SDK 11 RC의 WebAssembly props는 `UseMonoRuntime=true`가 기본이다. 별도로 .NET 11에는 CoreCLR WASM 경로와 전용 native relink targets가 존재한다. `v11.0.0` CoreCLR WASM 빌드 가이드는 실험적 지원으로 설명한다. | '.NET 11이면 모두 CoreCLR'도, 'net11도 기존 Mono와 동일'도 전제하지 않는다. `TFM + runtime flavor + runtime version`별 지원 계약을 나누고 실제 선택값을 확인한다. CoreCLR을 자동으로 선택하거나 기존 안정 경로로 홍보하지 않는다. |
| CoreCLR native/EH | CoreCLR 전용 targets는 `_CoreCLRWriteLinkRsp`/`_CoreCLRLinkNative`를 사용하며 SIMD/EH와 `WASM_LEGACY_EXCEPTIONS=0`을 강제한다. | 기존 [native hook](packages/platforms/build/Sdk.targets)의 Mono `_BrowserWasm*` hook과 Emscripten 3.1.56 Debug workaround가 그대로 적용되지 않는다. CoreCLR native 연결은 별도 검증하며, COOP/COEP가 없어도 SIMD/EH 등 브라우저 요구가 사라지지 않는다. |
| WebCIL/AOT | 공식 .NET 11 runtime 문서는 CoreCLR WASM에서 WebCIL V1을 기본으로 하고 header/table 계약과 Crossgen2 출력이 바뀐다고 설명한다. Mono/WebCIL reader의 V0/V1 지원도 별도로 언급한다. | Mono AOT의 `RunAOTCompilation`, assembly별 interpreter opt-in, IL stripping/cache 규칙을 CoreCLR/NativeAOT에 그대로 적용하지 않는다. payload와 compiler/runtime 조합을 식별하고 WebCIL 내부 형식을 직접 재작성하지 않는다. |
| Blazor 자산 등록 | SDK 11 RC targets에는 .NET 11 패키지의 `blazor.webassembly.js`를 framework static web asset으로 처리하는 경로가 있고, 구 패키지의 `BlazorWebAssemblyJSPath`와 구분한다. | [provider 검증](packages/platforms/build/Sdk.targets)의 `DOROTIWEB023`은 해당 파일 경로의 존재를 무조건 요구한다. net11 패키지 전환 시 이 검사도 실제 자산 계약 기준으로 변경해야 한다. Doroti가 직접 dotnet loader를 사용하더라도 SDK 자산 파이프라인 검사는 영향을 받는다. |

유지되는 것과 미확인 부분:

- .NET 11 Mono 소스에도 feature-gated `JSWebWorker.RunAsyncVoid`가 있으며, Mono의 공개 타입 선언에는 `getDotnetRuntime`, `runtimeBuildInfo.wasmEnableThreads`, `localHeapViewU8`가 남아 있다. 따라서 API가 모두 제거되었다고 단정하지 않는다. 실제 threaded pack/trim/AOT에서 사용할 수 있는지는 별도 검증한다.
- .NET 11 Mono loader는 non-threaded Worker 환경을 sidecar로 구분한다. 기존 독립 Worker 재사용 가능성은 있지만 owner context/초기화/종료/서비스 브리지까지 동등하다는 증거는 아니다.
- .NET 11의 공식 Blazor Web Worker 템플릿은 별도 Worker 런타임 호출을 위한 참고 자료다. 공유 managed heap의 JS-affine render Worker와 같지 않으며, 이를 단순 도입해 main과 Worker에 런타임 두 개를 만드는 것으로 해결하지 않는다.
- 로컬에 설치된 WASM runtime/native packs는 이번 확인 기준 .NET 10의 `10.0.11`/`10.0.14`뿐이다. .NET 11 SDK RC 및 net11 manifest 존재만으로 **.NET 11 WASM 실행 검증 완료**라고 보고할 수 없다. SDK/workload 추가 설치는 이 문서 작업에서 수행하지 않았다.

설계에 반영할 결정:

1. 자동 실행 모드 선택과 runtime별 host 연결을 분리한다. Mono 10/11 차이는 기존 초기화 모듈 안의 작은 명시적 연결 계약으로 관리하고 범용 runtime 추상화는 만들지 않는다. 공통 renderer/앱 코드에 SDK major 조건을 흩뿌리지 않는다.
2. Mono 10은 실제 threads 자산, Mono 11은 선택된 loader의 URL/fingerprint로 runtime Worker를 식별한다. 현재 `Worker` 생성자 관찰과 private `JSWebWorker.RunAsyncVoid` 의존은 **남아 있는 기술적 제약**으로 인정하고 연결 계층에만 둔다. 생성자 래핑의 중복 설치/실패 시 복원/다른 Worker 통과/추가 pthread 실패 관찰까지 수명을 정의한다. 범용 `dotnet.*` 정규식으로 넓히거나 .NET preload/control 메시지를 소비하지 않는다. origin, session token, protocol, MessagePort 검증은 유지한다.
3. native 연결 가능성은 '빈 앱 빌드'가 아닌 **Skia/interop를 링크한 최소 render probe**로 입증한다. threaded는 managed port 연결/실제 render owner, compatible은 non-shared heap/실제 첫 frame까지 확인한다.
4. CoreCLR WASM은 초기 제품 프로필에서 명시적으로 미지원 처리한다. 별도 도입 시 threaded JS-affine Worker, standalone runtime, JS interop, native/AOT 계약을 검증한다. 해당 flavor에 `WasmEnableThreads` 토글만 적용하면 두 모드가 생긴다고 가정하지 않는다.
5. dual 배포의 두 variant는 같은 검증된 runtime flavor/버전/toolchain으로 만든다. 고성능을 CoreCLR, 호환을 Mono로 몰래 교차 선택하지 않는다. .NET 11에서 문제가 생겨 .NET 10으로 바꾸는 것도 브라우저 호환 fallback이 아니라 별도 배포 정책이다.
6. Mono/Emscripten의 shared/non-shared 두 빌드 필요성은 유지된다. 바뀌는 것은 그 두 산출물의 생성·초기화 계약이다. COOP/COEP 미충족과 runtime 구조 미지원은 서로 다른 사유로 진단한다.

#### 모든 런타임을 독립 Worker 하나로 통일하지 않은 근거

- Mono 10/11 source에는 sidecar 환경 판정이 있지만, `dotnetSidecar` 선언과 내부 분기는 안정된 앱용 hosting API와 같지 않다. 특히 Mono 11의 non-threaded 자동 sidecar 처리를 threaded에도 그대로 일반화하지 않는다.
- Mono 11의 [deputy thread](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/pthreads/deputy-thread.ts)는 managed runtime을 시작하고 main JS context와의 synchronization context를 설치한다. [동기 JSExport 구현](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/managed-exports.ts)은 현재 thread에 managed 실행이 없으면 다른 managed thread로 동기 전달할 수 있다.
- Doroti의 paint는 JS↔managed↔native Skia↔GL/Dawn이 같은 render owner에서 호출되는 경로다. 따라서 bootstrap을 Worker로 옮기거나 동기 호출 제한을 완화하는 것만으로 owner 일치가 입증되지 않는다. 현재 JS-affine render role을 유지하는 쪽을 채택한다.
- 향후 공식 Worker-hosting API로 동일 owner·shared runtime·해제를 보장할 수 있으면 연결 모듈만 교체한다. 그 가능성을 위해 현재부터 미검증 supervisor Worker나 새 RPC 계층을 제품에 넣지 않는다.

## 5. 호환성 범위와 예상 제약

### 보존할 기능

- 첫 화면/첫 frame, 레이아웃, 스크롤/애니메이션, 텍스트 입력/IME, 포커스와 접근성 semantics.
- 폰트 fallback, 이미지 decode, asset/localization, navigation 및 history 복원.
- clipboard/file picker 등 DOM 서비스, JavaScript plugin, 지원하는 platform view/iframe composition.
- WebGL2에서 제공하던 texture/effect 동작. [GPU effect backend](packages/platforms/web/Doroti.Host.Web/DorotiWebWorkerSurface.GpuEffects.cs)는 이미 WebGL2/WebGPU variant를 구분한다. 앱의 shader에도 WebGL2 variant가 있는지 확인하고 누락을 숨기지 않는다.
- resize/DPR, context loss, 종료/재시작, pending request/lease의 정상 정리.

### 이번 범위에서 보장하지 않을 것

- Worker/OffscreenCanvas/WebGL2가 없는 구형 브라우저까지 지원하는 것. Canvas2D/소프트웨어/DOM-main renderer 추가는 별도 과제다.
- 비격리 WebGPU의 초기 제품 지원. renderer와 실행 모드는 구조적으로 분리하지만 compatible WebGPU는 별도 native/owner 검증 이후 확장한다. 기존 격리 검사만 삭제하여 지원으로 간주하지 않는다.
- 임의의 `Thread`, blocking wait, native pthread 의존 플러그인의 투명한 호환. 앱/플러그인이 해당 기능을 요구하면 호환 구현이나 명시적인 미지원 안내가 필요하다.
- 일반 HTTP에서 모든 서비스 API를 제공하는 것. 비격리 HTTPS/localhost를 우선 지원하며 clipboard/camera 등 secure-context/권한 요구는 남는다.
- 모든 iframe/sandbox/CSP 구성에서 실행되는 것. embedding, origin, frame policy와 자원 접근 제한은 각각 충족해야 한다.
- 성능 저하를 감추기 위한 자동 DPR/blur/품질 하향. 우선 동일한 결과를 보존하고 별도 품질 정책은 실측 후 제안한다.

주의할 실행 경로:

- [DartAsync](Doroti/src/Doroti.Runtime/DartAsync.cs)의 scheduler 부재 시 `Task.Run`, [이미지 decode](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs)의 `Task.Run`, [platform view 해제](Doroti/src/Doroti.Hosting/PlatformViewCoordinator.cs)의 비동기 정리를 실제 단일 스레드 런타임에서 확인한다.
- `Task.Run`이 있다고 무조건 미지원으로 분류하거나 모두 제거하지 않는다. non-threaded에서는 병렬 CPU 처리를 보장하지 않으며 owner context/응답성/취소가 핵심 검증 대상이다.
- [Hot Reload bridge](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.hot-reload.ts)는 현재 main runtime 초기화에서 연결된다. 호환 개발 모드의 Hot Reload는 따로 자격 검증한다. 지원되지 않으면 명시적으로 알리고 rebuild/page reload를 제공한다.

## 6. 단계별 작업 계획

### P-1. .NET 11 runtime 구조 적합성 판정

- [ ] SDK 10/11 + net10/Mono 10 및 SDK 11 + net11/Mono 11을 구분한 지원 상태표를 만든다. CoreCLR은 초기 미지원으로 검사한다. 설치·소스 확인·빌드·실행 증거를 따로 표시한다.
- [ ] Mono 10의 별도 pthread 자산과 Mono 11의 loader-entry Worker를 각각 식별하는 계약을 정의하고 managed-port handshake 회귀 테스트를 설계한다. 관련 없는 Worker와 잘못된 token/port는 수락하지 않아야 한다.
- [ ] Emscripten 6.0.2와 Skia/emdawn/interop 조합을 검증할 최소 render probe를 준비한다. native 자산 조정 여부를 판정하고 빌드 성공만으로 렌더링 성공을 대신하지 않는다.
- [ ] net11 Blazor static-web-assets 검사와 Mono/CoreCLR별 native hook/WebCIL/AOT 차이를 분류한다. 지원하지 않는 flavor는 명시적으로 거부한다.
- [ ] 실제 net11 자격 검증이 차단되면 이유·필요한 패키지/도구·남은 검증을 기록한다. net10 결과를 net11 통과로 대체하지 않는다.

완료 조건: Mono 10/11 연결과 native 자산의 지원 가능/차단 판정이 있다. net11은 Skia/interop 양 모드 render probe에서 managed paint의 thread ID, JS context, GPU 소유권 및 반환 경로를 확인한다. 기본 TFM의 일괄 변경은 이 단계의 전제 조건이 아니다.

### P0. 기존 standalone 경로 자격 검증

- [ ] SDK 10/11 각각에 대해 SDK 선택, net10 workload/pack 및 runtime pin의 해석을 확인하고 재현 가능한 toolchain 조합을 기록한다. SDK 11은 필수 검증 축이며 마지막에 붙이는 선택 검사가 아니다.
- [ ] Testbed와 실제 앱 한 개(Sample2)를 모드별 격리 출력에서 `WasmEnableThreads=false`로 빌드한다.
- [ ] 헤더가 실제 없는 서버에서 `runtimeLocation="worker"` + WebGL2로 시작한다. 별도 thread-enabled 산출물을 잘못 읽지 않았는지 runtime/profile/heap으로 확인한다.
- [ ] Debug/non-AOT와 Release/AOT를 분리 검증한다. Skia `st` 선택 및 기존 emdawn interop 링크가 맞는지 확인한다.
- [ ] 첫 frame, 입력/스크롤, 폰트/이미지, timers, 최소 플러그인 호출 및 종료를 확인한다.
- [ ] main-only 텍스처 초기화 등 재사용 경로의 기능 누락과 실제 blocking/thread 의존성을 기록하고 필요한 부분만 수정한다.

완료 조건: SDK 10/11로 빌드한 net10 앱 각각이 비격리 실제 환경에서 non-shared runtime 한 개로 렌더링되고 상호작용한다. threaded 기준 동작도 각각 확인한다. 실패하면 toolchain/native/owner/서비스 원인을 먼저 해결하며 dual 자동 배포로 가리지 않는다.

### P1. 공개 모드와 manifest 계약

- [ ] 결정한 실행 모드 옵션과 우선순위/legacy 동작을 구현 가능한 정적 타입으로 정리하고 빌드 프로필 v2를 정의한다.
- [ ] 요청 모드와 선택 결과, runtime descriptor를 분리한다. 신규 공개 API는 실행 모드만 받고 `runtimeLocation`은 legacy 호환 및 내부 진단으로 한정한다.
- [ ] capability 판정과 실행 선택을 작은 독립 함수로 분리해 조합별 테스트가 가능하도록 한다.
- [ ] runtime/profile/heap 불일치, 미지원 조합, 선택 이유의 오류/진단 계약을 정의한다.
- [ ] build provenance에 SDK/TFM/runtime flavor·버전/workload/toolchain 정보를 구분해 기록하고, 빌드 SDK major를 브라우저 선택 조건으로 사용하지 않는다. 선택된 runtime host 계약의 지원 여부는 별도 검증한다.
- [ ] [loader 타입 선언](packages/platforms/web/Doroti.Host.Web/Web/types/doroti-loader/index.d.ts)과 패키징되는 동일 선언을 함께 갱신한다.
- [ ] 공통 session/endpoint 계약, 세션 세대, 종료 사유와 runtime-ready/first-frame 구분을 기존 프로토콜에 반영한다. 앱 초기화/서비스가 runtime 소유권별로 분기하지 않게 한다.

완료 조건: 선택/검증 규칙이 테스트되고 v1 단일 모드 소비를 깨지 않는다. 아직 없는 새 옵션을 문서상 사용 가능하다고 발표하지 않는다.

### P2. dual publish와 자산 통합

- [ ] [provider SDK targets](packages/platforms/build/Sdk.targets), [Web targets](packages/platforms/build/Doroti.Web.targets), [publish 검증](packages/platforms/web/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets)에 모드별 child publish 및 통합 검사를 추가한다.
- [ ] restore부터 AOT/static-web-assets까지 출력 격리를 검증한다. threaded → compatible → threaded 재빌드에서도 캐시 오염이 없어야 한다.
- [ ] SDK 10 → 11 → 10 전환에서도 native/AOT/WebCIL/TypeScript/static-web-assets 캐시가 섞이지 않는지 확인하고, SDK 11의 hook 실행과 실제 native 입력을 검사한다.
- [ ] net11 프로필에 호환성이 검증된 Emscripten 6.0.2 native 자산과 새 Blazor static-web-assets 경로를 사용하며, 다른 runtime/toolchain의 native/AOT/WebCIL 산출물을 병합하지 않는다.
- [ ] 실제 fingerprint와 압축 파일을 보존하여 variant를 배치하고 공통 자산의 동일성을 검사한다.
- [ ] HTML/import map/preload를 선택형으로 정리하고 두 빌드의 ID/프로필/필수 자산이 완성된 뒤 manifest를 생성한다.
- [ ] source checkout과 NuGet target/provider 소비 모두 동일하게 구성되는지 확인한다. 런타임 profile 생성 위치를 공통 runner SDK에 있다고 오인하지 않는다.

완료 조건: 한 publish root에 일관된 두 산출물이 있으며 각각 독립적으로 로드된다. 한쪽 빌드 실패, 자산 충돌, 누락은 publish 실패다.

### P3. 자동 loader 및 호환 실행 연결

- [ ] 기존 standalone/shared render 진입부를 공통 session 역할로 정리한다. 입력/frame/서비스/GPU loop는 공유하고 runtime 생성/연결/해제만 나눈다.
- [ ] .NET import/create 및 visible canvas 이전보다 앞에서 모드를 결정한다.
- [ ] 선택한 dotnet/profile URL과 app base를 host, raster Worker, 재시작 경로까지 전달한다.
- [ ] Mono 10/11의 서로 다른 pthread 진입점에 맞춰 Worker 감시/managed-port 연결을 수행한다. 로드된 runtime이 미지원이면 isolation fallback과 구분해 실패시킨다.
- [ ] 고성능 renderer policy는 유지하고 compatible은 WebGL2로 연결한다. Android의 WebGPU 선택이 비격리 호환 시작을 막지 않게 한다.
- [ ] standalone runtime의 실제 threads/heap 검사와 진단을 추가한다.
- [ ] 단일 runtime/visible canvas 소유권, splash/error/retry, 취소/해제, Worker 재시작의 선택 일관성을 확인한다.
- [ ] runtime 소유권에 묶인 앱 초기화를 공통 managed 시작 또는 적절한 메시지 경로로 이동한다. DOM 전용 작업은 계속 main bridge로 수행한다.
- [ ] Worker 생성자 관찰은 Mono 연결 모듈 내부에서만 설치하고 기존 Worker 동작을 보존한다. 잘못된 handshake/timeout/추가 pthread 장애/종료 후 늦은 메시지를 검증한다.

완료 조건: 동일 배포본/동일 앱 URL이 서버 헤더 조건에 따라 고성능 또는 호환으로 자동 시작한다. 호환 선택을 오류로 표시하지 않고 실제 실패는 표시한다.

### P4. 개발 도구·샘플·템플릿·문서 정합성

- [ ] 초기 도입은 기존 threaded 배포 동작을 유지하는 opt-in으로 시작한다. 검증 완료 후 신규 템플릿과 제공 샘플은 auto를 기본 사용하고 기존 앱에는 명시적 마이그레이션 절차를 제공한다.
- [ ] Testbed, Sample2, Demo, Carousel 및 템플릿의 threads/main 고정 설정을 새 모드와 일관되게 정리한다. raw `WasmEnableThreads`와 mode가 충돌하면 명확한 진단을 제공한다.
- [ ] devserver 옵션 전달 경로까지 포함해 격리 on/off가 실제 응답에 반영되도록 한다. 강제 헤더가 남아 있는 상태를 비격리 테스트로 인정하지 않는다.
- [ ] `run/watch`의 모드 전환은 필요한 빌드/서버 재시작으로 처리한다. auto publish 지원과 dual Hot Reload를 동일 과제로 과도하게 확장하지 않는다.
- [ ] SDK 10/11 + net10, SDK 11 + net11/Mono의 CI job, 지원 SDK/workload 버전 및 선택 절차를 문서화한다. SDK 11로 net10을 빌드하는 안내와 net11 타깃 마이그레이션 안내를 분리한다.
- [ ] tunnel의 단일 root framework 및 pthread 필수 검사, Nginx runtime 경로/404/MIME 검사를 모드별로 갱신한다. 기존 격리 공유 환경은 유지한다.
- [ ] [Web provider README](packages/platforms/web/README.md), [제품 README](Doroti/README.md), [한국어 README](Doroti/README.ko.md), 샘플 가이드 및 [target manifest](packages/platforms/web/Doroti.Target.Web.browser-wasm/doroti-target-manifest.json)를 갱신한다.

완료 조건: 사용자가 별도 JS 수정이나 배포 폴더 수작업 없이 auto publish를 만들 수 있고, 헤더를 설정할 수 없는 HTTPS 정적 호스팅에서도 compatible이 선택된다.

의존 순서: **P-1 → P0 → P1 → P2/P3 → P4 → 최종 인수 검증**. P-1의 구조 판정을 거치지 않은 채 기존 net10 계약을 공통 계약으로 확정하지 않는다. P2/P3은 P1 계약 확정 후 나눠 진행할 수 있지만 두 결과를 합친 실제 배포 테스트는 반드시 수행한다.

출시 순서와 중단 기준:

1. 기존 단일 threaded 배포를 변경하지 않고 contract/진단을 도입한다.
2. 격리된 출력에서 compatible 단일 프로필의 기능 동등성을 확인한 다음 dual auto를 opt-in으로 배포한다.
3. Mono 11 native/owner 검증과 아래 전체 행렬을 통과하면 차세대 지원으로 승격한다. Skia ABI, shared render owner 또는 cleanup 검증이 차단되면 해당 프로필을 출시하지 않는다.
4. 신규 템플릿의 auto 기본값과 net11 기본 TFM은 각각 인수 후 적용한다. 기존 앱은 명시적으로 전환한다.
5. 회귀 시 이전 **완결된 배포본**으로 되돌린다. 운영 중 한쪽 framework만 덮어쓰거나 실제 오류를 compatible 선택으로 숨기지 않는다.

## 7. 검증 계획과 인수 기준

모든 테스트 실행에는 **외부 프로세스 기준 20분(1,200초) timeout**을 적용한다. test runner의 개별 timeout만 두어 전체 실행이 무한 대기하지 않도록 한다. 일반적인 반복 검증은 시나리오당 3~5회, 추가 확인도 대부분 30회 이내로 제한한다.

### SDK/타깃/실행 모드 행렬

| SDK | TFM/runtime 계열 | 빌드·실행 검증 | 분류 |
| --- | --- | --- | --- |
| 고정한 SDK 10 | `net10.0` / .NET 10 | threaded + compatible 각각 Debug/non-AOT 및 Release/AOT | 기존 기준선, 필수 |
| 고정한 SDK 11 | `net10.0` / .NET 10 | threaded + compatible 각각 Debug/non-AOT 및 Release/AOT | 이번 지원 목표, 필수 |
| SDK 11 | `net11.0` / Mono 11 | loader-entry pthread handshake + Emscripten 6.0.2/Skia link + 양 모드 Debug/non-AOT 및 Release/AOT | 차세대 제품 지원의 필수 목표, 실행 통과 전 지원 표기 금지 |
| SDK 11 | `net11.0` / CoreCLR | 선택 시 명시적인 미지원 진단 | 초기 제품 제외; 별도 도입에서만 runtime/ABI/성능 검증 |

- 세 개의 지원 조합(SDK10/net10, SDK11/net10, SDK11/net11 Mono)마다 auto dual publish를 만들고, 같은 배포본의 COOP/COEP on/off 선택과 실제 첫 frame을 확인한다. 핵심 구성은 toolchain 조합 3종 × 실행 모드 2종 × Debug/Release 2종의 12개다.
- `RunAOTCompilation=false`인 Release 및 Sample2의 mixed-AOT도 영향 경로를 중심으로 추가 검증한다. 모든 브라우저/설정의 무차별 조합 반복은 피하되 각 SDK 산출물의 양 모드 브라우저 smoke는 생략하지 않는다.
- SDK 11은 시험한 정확한 RC/정식 버전과 workload set을 기록한다. RC 성공을 정식/이후 feature band 지원으로 간주하지 않으며, 버전 변경 시 native link/AOT/자동 선택/첫 frame 핵심 검사를 다시 수행한다.
- SDK 11 필수 job이 미실행/실패라면 SDK 10 결과만으로 전체 완료를 선언하지 않는다.
- 12개 구성 검증은 소수 실행으로 수행하고, 전체 브라우저 행렬을 매번 곱해 수백 회 반복하지 않는다. native link/owner/threads 선택과 기능 회귀를 우선 검증하고 플랫폼별 위험 경로를 추가한다.

### 필수 환경/선택 행렬

| 조건 | 기대 결과 |
| --- | --- |
| HTTPS/localhost + 올바른 COOP/COEP + shared-memory 지원 | auto → threaded, shared runtime 한 개 |
| HTTPS + COOP/COEP 둘 다 없음 | auto → compatible, non-shared runtime 한 개 |
| COOP만 있음 / COEP만 있음 | 실제 isolation false이면 compatible |
| 헤더는 있으나 Permissions-Policy로 isolation 차단 | 실제 capability를 따라 compatible |
| 격리 환경에서 강제 compatible | non-threaded Worker 사용 |
| 비격리 환경에서 강제 threaded | 시작 전 설명 가능한 오류 |
| compatible + 명시적 WebGPU | 미지원 조합 오류; 조용한 renderer 변경 없음 |
| Android auto + WebGPU 미지원 | threaded 가능 시 기존 WebGL2 전환, 비격리면 compatible WebGL2 |
| dual manifest의 파일 누락/잘못된 schema/다른 build ID | 명확한 실패; 다른 runtime이나 HTML을 대신 읽지 않음 |
| Worker/CSP/OffscreenCanvas/WebGL2 미지원 | 조건별 오류; 무한 splash/재시도 없음 |
| compatible-only / threaded-only 기존 배포 | 선언된 모드 안에서 동작; 없는 variant 추측 금지 |

### 빌드/배포 검증

- [ ] production TypeScript 빌드 및 두 variant의 .NET native link/AOT 성공. 이 변경에 필요한 검증부터 수행하고 무관한 전체 솔루션 검증은 피한다.
- [ ] 비격리 검증은 실제 응답 헤더가 없는 서버에서 수행한다. `crossOriginIsolated` 값을 JS mock으로 바꾸는 단위 테스트만으로 완료하지 않는다.
- [ ] Network에서 선택되지 않은 runtime의 fetch/preload/worker 생성이 없는지 확인한다. 호환 경로에서 pthread worker가 뜨지 않아야 한다.
- [ ] .NET의 실제 build info 및 heap buffer 종류로 프로필을 검증한다. DOM dataset/파일명 확인만으로 통과시키지 않는다.
- [ ] Mono 11 threaded에서 실제 loader-entry Worker 생성과 managed-port 수락을 관찰한다. 구 `.mjs` 필터로 인한 연결 timeout을 검출하고 runtime preload 메시지를 방해하지 않는지 확인한다.
- [ ] 일반 unrelated Worker의 생성/메시지는 변하지 않으며, 잘못된 token/protocol/port와 이전 세션 응답을 현재 세션으로 수락하지 않는다. endpoint 연결 실패가 120초 후의 원인 불명 timeout만으로 남지 않도록 단계별 원인을 기록한다.
- [ ] `/`, subpath, deep link, 캐시된 구세대 HTML + 새 배포, `.mjs`/`.wasm` MIME 및 압축 응답을 확인한다.
- [ ] 동시 페이지 접근에서 양 모드 파일이 같은 배포 세대에 속하고 서로 덮어쓰지 않는지 확인한다.

### 기능/브라우저 검증

- [ ] Windows Chromium/Firefox, Android Chrome, 실제 iOS/iPadOS Safari에서 isolation 유무에 따른 선택과 첫 frame/입력/스크롤을 확인한다.
- [ ] iOS AOT는 실제 Safari로 확인한다. [기존 Safari 제한](packages/platforms/web/README.md#safari-and-memory-configuration)을 유지하며 Playwright WebKit 통과만으로 실제 장치 안정성을 선언하지 않는다.
- [ ] Sample2의 깊은 widget tree, 반복 blur/scroll, 이미지 decode 중 입력, 화면 복귀/resize, 한글 IME, 접근성, 파일 선택/취소, clipboard, navigation, texture/effect 및 plugin을 검증한다.
- [ ] Worker/context loss 후 pending 요청과 GPU/외부 lease가 정리되고, 재시작이 기존 정책대로 제한되며 실제 실패가 노출되는지 확인한다.
- [ ] 앱 startup이 세션당 한 번 실행되고 두 모드 모두 같은 기능을 등록하는지 확인한다. 입력 burst/resize 중에도 up/cancel/IME commit/종료 경계가 누락되지 않고 큐가 제한을 넘지 않아야 한다.
- [ ] 새 선택 로직의 소수 집중 테스트와 최소 브라우저 통합 fixture를 유지한다. 현재 저장소의 TypeScript/MSBuild 방식을 재사용하고 앱 빌드에 npm/bundler 의존성을 불필요하게 추가하지 않는다.

### 성능 검증

- [ ] 동일 SDK/toolchain/장치/앱/화면/renderer/AOT/캐시 조건으로 기존 threaded와 변경 후 threaded를 비교한다. 다음으로 compatible을 비교하며 WebGPU↔WebGL2 차이와 threads 차이를 구분한다. SDK 10↔11 차이는 별도 비교하여 모드 변경 효과와 섞지 않는다.
- [ ] cold/warm 시작부터 첫 실제 frame까지의 시간, 입력→frame 지연, frame time p50/p95, managed/WASM 메모리, 전송량 및 publish 시간/크기를 기록한다. `started` 이벤트만으로 첫 frame을 대신 측정하지 않는다.
- [ ] 고성능 경로의 잠정 회귀 경보 기준은 주요 지연 지표의 기존 대비 10% 초과 악화로 둔다. 사전 baseline에서 측정 잡음을 확인하고 초과 시 원인을 분석한다. 측정 없이 성능 동등성을 선언하지 않는다.
- [ ] 호환 경로에는 고성능 경로와 같은 FPS를 요구하지 않는다. 기능 완료/무응답 없음은 필수이며, 목표 장치의 허용 frame/input 지연 수치는 P0 실측 후 확정한다. 결정되지 않은 SLA나 성능 감소율을 임의 보장하지 않는다.

검증 산출물은 삭제 가능한 [Doroti/artifacts](Doroti/artifacts)에 둘 수 있다. 보존할 결과/한계 요약은 추적 문서에 남기고, 원시 산출물을 정리한 뒤에는 파일이 남아 있다고 보고하지 않는다.

## 8. 완료 정의

- [ ] SDK10/net10, SDK11/net10, SDK11/net11 Mono의 12개 핵심 구성을 검증하고 정확한 runtime/workload/native 조합을 기록한다. net11이 차단되면 net10 지원 전달과 차세대 전체 완료를 구분한다. 'SDK 11 빌드 성공'과 '.NET 11 WASM 지원'을 같은 표현으로 쓰지 않는다.
- [ ] auto publish 한 번으로 threaded/compatible을 모두 포함하는 정상 배포본을 만든다.
- [ ] 같은 앱 URL에서 실제 isolation/shared-memory 조건 충족 시 기존 고성능 경로, 미충족 시 compatible 경로가 자동 선택된다.
- [ ] 선택되지 않은 runtime은 다운로드/생성하지 않으며 두 모드 모두 managed runtime은 정확히 한 개다.
- [ ] 기존 고성능 기능/성능을 검증하고, 호환 모드의 필수 기능·성능 차이·미지원 범위를 기록한다.
- [ ] 빌드/캐시/리소스 경로 및 샘플 초기화가 모드에 따라 깨지지 않는다.
- [ ] 공통 render-session 경로에서 앱/GPU owner를 보존하고, SDK/runtime 버전 분기가 연결/빌드 경계 밖으로 퍼지지 않는다. 남아 있는 private JSWebWorker/Worker 관찰 의존과 지원 버전을 문서화한다.
- [ ] manifest/실행 불일치와 실제 장애가 드러나고, 안내 없는 품질 하향이나 실패 숨김이 없다.
- [ ] 새 템플릿/샘플과 기존 앱의 마이그레이션 가이드가 구현 상태와 일치한다.

## 9. 외부 근거

- [Emscripten Pthreads support](https://emscripten.org/docs/porting/pthreads.html): pthread/shared-memory 조건 및 thread/non-thread 두 빌드 필요성.
- [MDN: crossOriginIsolated](https://developer.mozilla.org/en-US/docs/Web/API/Window/crossOriginIsolated): 실제 격리 상태, COOP/COEP와 Permissions-Policy 관계.
- [MDN: WebGPU API](https://developer.mozilla.org/en-US/docs/Web/API/WebGPU_API): WebGPU의 secure-context 요구. 현재 Doroti의 threaded/isolated 제한과 구분할 근거.
- [Microsoft Learn: global.json](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json): SDK 선택과 대상 runtime의 구분, resolver 탐색 위치, roll-forward 및 prerelease 설정.
- [Mono 10 Worker 생성](https://github.com/dotnet/runtime/blob/v10.0.11/src/mono/browser/runtime/pthreads/ui-thread.ts), [Mono 11 Worker 생성](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/pthreads/ui-thread.ts), [Mono 11 preload Worker](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/loader/assets.ts), [loader URL 결정](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/loader/polyfills.ts): 별도 threads 자산에서 dotnet loader 진입점으로의 변경.
- [.NET 11 Emscripten 버전](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/emscripten-version.txt): `6.0.2`.
- [.NET 11 JSWebWorker](https://github.com/dotnet/runtime/blob/v11.0.0/src/libraries/System.Runtime.InteropServices.JavaScript/src/System/Runtime/InteropServices/JavaScript/JSWebWorker.cs), [Mono runtime 타입](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/dotnet.d.ts), [Mono sidecar 구분](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/loader/globals.ts): 기존 API 존재와 실제 빌드에서의 사용 가능성을 구분할 근거.
- [.NET 11 runtime 개선](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime#webassembly-improvements), [CoreCLR WASM 빌드 가이드](https://github.com/dotnet/runtime/blob/v11.0.0/docs/workflow/building/coreclr/wasm.md), [CoreCLR native targets](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/build/BrowserWasmApp.CoreCLR.targets): runtime flavor, WebCIL V1 및 별도 native pipeline.
- [Blazor Web Worker](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-with-dotnet-on-web-workers?view=aspnetcore-11.0): 일반 독립 Worker와 Doroti의 공유 런타임 render Worker를 구분한다.
- [Mono 11 deputy thread](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/pthreads/deputy-thread.ts), [동기 JSExport의 thread 전달](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/managed-exports.ts), [sidecar 내부 표식](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/types/sidecar.d.ts): bootstrap Worker 통일과 JS/native render owner 일치는 별개라는 설계 근거.

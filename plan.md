# Doroti cross-platform 부트 개선 작업 계획

- 재작성일: **2026-09-06**
- 분석 기준: HEAD `0499c1d`의 source, runner/target SDK, Testbed/template 설정 및 저장소 실행 기록.
- 실행 갱신: **2026-09-07**, 시작 HEAD `ccf61de` clean tree에서 source 재확인 후 구현·build·publish·실행 검증 수행. **가능한 구현/검증 및 결과 정리 완료, 전체 플랫폼 acceptance 미완료**. Apple/arm64 실기기/물리 IME·접근성·scan-out/전후 성능은 `notVerified`다.
- 실행 결과: [cross-platform-boot-results.md](history/26-09-07/cross-platform-boot-results.md), [원시 로그·JSON·캡처](.doroti/evidence/boot/20260907-implementation/). 아래 1–2절은 변경 전 분석 근거로 보존하며 다음 구현 delta가 현재 source를 설명한다.
- 목표: 설치된 Release 앱의 첫 유효 content 표시와 입력 준비까지 불필요한 직렬 작업을 줄인다. CLI build/deploy 시간은 별도 트랙으로 개선한다.
- 이전 계획: [2026-08-28 원문 보존본](history/26-09-06/cross-platform-first-boot-plan-2026-08-28.md). 당시 구현 및 실패 기록: [2026-08-29 MVP 결과](history/26-08-29/cross-platform-first-boot-implementation.md).

## 1. 이전 계획과 달라진 현재 기준

2026-09-07 구현 delta: Web JS/WASM 검증은 병렬이며 UI `dotnet.js` import는 CanvasKit 초기화와 겹친다. runtime.create/attach readiness는 유지한다. Testbed/template의 font preload와 document 전용 Blazor metadata를 동기화했다. MAUI Entry/Editor와 실제 MaterialSample light/dark theme를 lazy화했다. Windows presenter 선택을 native 검사 앞으로 옮겨 normal Vulkan/D3D12의 ANGLE 검사를 생략하되 audit는 유지한다. CLI는 binary/상속/evaluated external 입력과 toolchain/output을 묶은 state v2를 사용하며 build 성공을 launch 전에 기록한다. Qt target은 공용 Runner SDK에 있다. Android profile은 현재 DEX로 rebind/strict 검증했다. 과거 root의 lazy theme 완료와 현재 sample의 남은 eager 필드를 구분했다.

이전 계획의 미완료 목록을 그대로 재실행하지 않는다. 현재 제품 기본값은 **Windows App SDK + Vulkan**, **Web CanvasKit UI/Raster Worker**다. 기본값 변경 자체는 resize 성능이나 physical scan-out의 합격을 뜻하지 않는다.

| 항목 | 현재 source에서 확인한 사실 | 이번 계획의 처리 |
| --- | --- | --- |
| Windows | presenter 미지정/빈 값은 `Vulkan`. 명시적 `AngleD3D11`, 진단 `D3D12` 유지 | Vulkan opaque와 앱 요청 Acrylic 우선. ANGLE은 명시적 호환 경로 |
| Web | renderer 미지정/auto/미인식 값은 `worker-canvaskit-webgl` | 기본 Worker 경로를 분석 기준으로 사용. `document-webgl` 등은 명시적 회귀 대상 |
| Application boundary | source-generated `JsonSerializerContext` 사용. embedded JSON 파싱은 여전히 실행 | reflection metadata 제거 완료와 JSON parsing 제거를 구분 |
| Theme | Testbed RootApp/light·dark factory가 lazy. Material factory memoization 존재 | 같은 lazy theme 작업을 다시 제안하지 않음 |
| MAUI text | Entry/Editor 관리 객체는 constructor에서 생성, native attach는 on-demand | 남은 관리 객체 생성만 추가 개선 후보 |
| Semantics | MAUI와 Web Worker가 view 준비 시 semantics 활성화 | 접근성 상태 계약 없이 일괄 지연하지 않음 |
| Android | x64 AOT off + trim on, marshal methods off. Baseline Profile 파일/packaging 존재 | trimming 복구와 profile 최초 도입 대신 현재 APK 적합성 검증 |
| Web Release | trimming/symbol 제거 설정 존재. Testbed는 `WasmBuildNative=true` | trimming 도입을 반복하지 않음. native WASM build를 managed AOT로 부르지 않음 |
| Windows provenance | 정상 부트 full DLL hash는 audit-only. D3D12 diagnostics assembly 분리 | 해시 제거 대신 presenter별 필수 DLL 검사 재검토 |
| CLI | NoBuild/NoRestore/LastSuccessful과 source fingerprint 존재 | 새 옵션 대신 입력·출력 검증과 탐색 비용 보완 |

기본값 근거는 [renderer-defaults 기록](history/26-09-05/renderer-defaults.md)과 현재 source다. 과거 기록의 ANGLE default, CanvasKit opt-in, auto=document-webgl은 해당 실행 당시 조건으로만 읽는다. 이전 APK/Web payload 크기와 과거 PASS를 현재 artifact의 크기·성능으로 재사용하지 않는다.

## 2. 현재 부트 구성과 개선 근거

### 2.1 공용 경계

- [workspace](DorotiTestbedApp/doroti-workspace.json)가 플랫폼별 runner 경로를 선택한다. windows는 windowsappsdk runner이며 MAUI는 CLI에서 별도로 선택한다.
- [Program.cs](DorotiTestbedApp/Program.cs)는 `IDorotiApplicationStartup`으로 entrypoint/view를 등록한다. native entry/generated bootstrap은 [Runner SDK](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)가 소유한다.
- [DorotiApplicationBoundary.cs](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs)는 manifest schema/RID와 plugin handler/ABI를 검증한다. resource SHA-256 검증은 `LoadAsync` 시점이다. 모든 resource를 startup에 읽는 구조는 아니다.
- [DorotiHostSession.cs](Doroti/src/Doroti.Hosting/DorotiHostSession.cs)는 deferred Start 뒤 첫 AttachView에서 framework를 한 번 bootstrap한다. GPU surface와 attach의 선후 관계는 host마다 달라 단일 순서를 강제하지 않는다.
- [Testbed App](DorotiTestbedApp/src/App.cs)은 frame callback에서 root를 attach한다. 갤러리 자체 비용과 Framework/host 비용을 구분하고, Demo 화면을 단순화해 공용 개선으로 보고하지 않는다.

### 2.2 Web — 우선 개선 대상

현재 기본 경로의 주요 의존성은 다음과 같다.

```text
HTML / doroti_bootstrap.ts
  -> startDoroti singleton / renderer 선택
  -> CanvasKit manifest fetch
  -> JS fetch + 길이/SHA-256 검증
  -> WASM fetch + 길이/SHA-256 검증
  -> DOM endpoints + UI/Raster Worker 생성 및 초기 메시지
     ├─ UI: classic importScripts -> CanvasKitInit -> text service
     └─ Raster: classic importScripts -> CanvasKitInit -> WebGL2 / gpu-ready
  -> UI CanvasKit ready AND Raster ready
  -> UI Worker의 dotnet.js import / runtime.create
  -> generated StartWorker
  -> NanumGothic fetch / font registration
  -> boundary + session/view + semantics
  -> build/layout -> DisplayList encode -> resource readiness -> Raster submit
```

직접 확인한 사실과 후보:

1. [doroti.loader.ts](Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts): Worker 경로는 `Blazor.start()`를 호출하지 않는다. `context.blazorOptions`는 document 경로에 전달되며 기본 CanvasKit 경로에는 전달되지 않는다. 앱의 loadBootResource callback이 기본 Worker 다운로드를 제어한다고 가정하면 안 된다.
2. [doroti.canvaskit.host.ts](Doroti/src/Doroti.Host.Web/Web/doroti.canvaskit.host.ts): loadCanvasKitManifest가 JS/WASM을 **순차 fetch·검증한 뒤** Worker를 시작한다. 두 파일의 검증은 독립적이다. `cache: no-cache`는 재검증 정책이며 매번 전체 네트워크 다운로드한다는 뜻은 아니다.
3. [classic bootstrap](Doroti/src/Doroti.Host.Web/Web/doroti.canvaskit.bootstrap.ts), [UI role](Doroti/src/Doroti.Host.Web/Web/doroti.ui.worker.ts), [Raster role](Doroti/src/Doroti.Host.Web/Web/doroti.canvaskit.worker.ts): 각 Worker가 CanvasKit instance를 초기화한다. 검증용 bytes를 그대로 소비하는 경로는 아니므로 이후 URL load와의 중복 요청/byte 처리량은 조사 대상이다. 실제 wire 중복량은 HTTP cache를 포함해 확인해야 한다.
4. UI의 maybeStartManagedRuntime는 두 role readiness를 모두 기다린다. 독립적인 runtime 준비를 앞당길 여지가 있으나 bridge 설치·GPU/text capability·root attach는 준비 완료 순서를 보존해야 한다.
5. [DorotiWebWorkerRunner.cs](Doroti/src/Doroti.Target.Web.browser-wasm/DorotiWebWorkerRunner.cs)는 managed 진입 후 fallback font를 fetch한다. [CanvasKitResourceRegistry.cs](Doroti/src/Doroti.Host.Web/CanvasKitResourceRegistry.cs)의 retained resource 수명과 ACK 계약을 유지하면서 요청을 앞당길 수 있다.
6. host settleReady는 rasterReady와 runtimeReady로 loader started를 완료한다. **첫 content 표시 신호가 아니다.** front/scene/resource/terminal을 연결한 별도 부트 판정이 필요하다.
7. [index.html](DorotiTestbedApp/web/wwwroot/index.html)은 Blazor loader preload를 유지한다. 기본 Worker에서 실제로 사용되는 요청인지, framework 자동 preload와 CanvasKit/font 우선순위가 맞는지 확인해야 한다.

UI=managed Framework/layout/text, Raster=visible OffscreenCanvas/WebGL2, main=DOM/input/IME/semantics 경계를 유지한다. UI CanvasKit은 text layout을 담당하므로 단순 중복 instance로 보고 삭제하지 않는다.

### 2.3 Windows

- [DorotiWindowsAppSdkRunner.cs](Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs): native loading 준비 → ABI/Windows App Runtime/COM 준비 → boundary/session → presenter 선택과 native host 실행 → view/framework → 첫 raster/presentation 경로다.
- [WindowsManagedVulkanPresenter.cs](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs)의 backend는 `Vulkan/Composition-Swapchain`이다. opaque도 Composition topology를 사용하며 Acrylic은 backdrop 준비를 더한다. ANGLE/EGL 첫 swap 순서를 기본 부트 설명으로 사용할 수 없다.
- [WindowsNativeV1.cs](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsNativeV1.cs)의 ConfigureAppDirectoryLoading은 presenter 선택 전에 host/bootstrap/`av_libglesv2.dll` 존재와 PE를 검사한다. Vulkan 부트에서도 ANGLE 파일 검사는 실행된다. 이것이 곧 ANGLE GPU 초기화라는 뜻은 아니다.
- [host project](Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj)는 Windows App SDK 2.4.0, ANGLE native package, Silk Vulkan/Skia Vulkan을 함께 참조한다. 복수 backend 배포와 selected backend의 startup 필수 dependency를 구분할 필요가 있다.
- 현재 Acrylic/Composition 사용을 고려하면 “AppWindow를 보관만 하므로 Windows App SDK 제거”라는 옛 판단을 적용할 수 없다. raw Win32 재분류는 이번 범위에서 제외한다.

### 2.4 Android / Apple / Linux

| 제품 | 현재 구성 | 남은 방향 |
| --- | --- | --- |
| Android arm64 | MAUI, Release AOT/trimming 기본 경로, marshal methods off | 정확한 APK/profile 적합성, 첫 text client/semantics 검증 |
| Android x64 | MAUI, AOT off + trim on | 현재 emulator startup gate. arm64 결과로 대체 금지 |
| iOS | MAUI/UIKit entry, physical/simulator RID 분리 | signed Release/Metal first content와 첫 입력. DLR을 위한 interpreter 설정 보존 |
| Mac Catalyst | MAUI/UIKit entry, maccatalyst-arm64 | iOS와 별도 cold launch/Metal/input 결과 |
| native AppKit | net10.0-macos, osx-arm64, AppKit-Main, MAUI 공용층 + 전용 Metal surface | Catalyst와 별도 NSWindow/Metal lifecycle 검증 |
| Linux | Qt native shim + managed host, SelfContained=false, linux-x64 | published executable의 QPA/GL/first frame, Wayland/X11 분리 |

[Runner SDK](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)의 iOS/Catalyst `MtouchInterpreter=-all`은 managed AOT와 런타임 생성 DLR 코드 지원을 함께 고려한 설정이다. “모든 assembly를 interpreter로 실행” 또는 “interpreter 제거 완료”로 단순 해석하지 않는다.

[DorotiMauiSurface.cs](Doroti/src/Doroti.Host.Maui/DorotiMauiSurface.cs)는 hidden input 관리 객체를 즉시 생성하고 [MauiTextInputBridge.cs](Doroti/src/Doroti.Host.Maui/MauiTextInputBridge.cs)가 active client의 native attach를 담당한다. 객체 생성과 native attach를 별도 최적화로 다룬다.

Android [profile README](DorotiTestbedApp/android/profiles/README.md)는 기존 CUJ와 재생성 조건을 기록한다. 파일 존재만으로 현재 DEX에 유효하거나 DEX layout용 Startup Profile까지 적용됐다고 판정하지 않는다.

Linux [runner](DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj)의 BuildDorotiQtNative는 build 시 CMake configure/build를 호출한다. 설치된 executable runtime 부트와 별도인 developer iteration 비용이다.

### 2.5 CLI / SDK / template

[doroti.ps1](Doroti/eng/doroti.ps1)의 fingerprint는 workspace, Doroti/src, Doroti/eng를 재귀 탐색한 뒤 확장자 목록에 맞는 파일 내용을 hash한다.

- 제외 경로 검사는 재귀 열거 후 수행하며 node_modules가 제외 목록에 없다. 설치된 Web dependency tree가 있으면 광범위 탐색/hash 후보가 된다.
- Doroti 루트의 Directory.Build.props/targets, Directory.Packages.props는 위 세 root 밖이다. ttf/png/prof/profm 등 binary resource도 현재 확장자 목록 밖이다. 실제 build input을 기준으로 누락을 검증해야 한다.
- state는 runner/configuration/RID/source fingerprint를 기록하지만 실행 artifact의 파일별 hash나 toolchain identity는 기록하지 않는다. missing/tampered output, SDK/workload 변경에 대한 거부 계약을 보완해야 한다.
- state 저장은 정상 dotnet run 반환 후다. “build 성공 artifact”와 “run 정상 종료”를 같은 상태로 쓰는 것이 의도인지 구분한다.
- Testbed Web에는 native-build/AOT 실험 target이 있지만 template은 동일 설정이 아니다. 공용 최적화는 Runner/Target/Host에 두고 Testbed와 새 template 앱을 각각 검증한다.

## 3. 판정과 증거 계약

### 3.1 분리할 시나리오

| 시나리오 | 시작 기준 | 종료/결과 |
| --- | --- | --- |
| first-install | 새 설치/명시적 데이터 초기화 후 launch | 첫 content/input-ready, 설치·런타임 준비 상태 |
| process cold | process가 없는 설치된 Release 앱 launch | launch-to-first-content, launch-to-input-ready |
| warm/resume | 살아 있는 process의 resume | resume-to-valid-content, focus/input 복원 |
| Web cold navigation | 새 browser profile/cache 조건을 기록한 navigation | navigation-to-first-content/input-ready, transferred/decoded bytes |
| Web repeat navigation | 동일 배포/cache 조건의 재탐색 | HTTP/runtime 상태를 표시한 별도 결과 |
| developer launch | CLI 진입 | fingerprint/restore/build/native/AOT/deploy/launch 분리 |

OS의 TTID/TTFD와 Doroti marker는 별도 필드로 기록한다. Android am start -W 또는 reportFullyDrawn 값을 Doroti first content와 무조건 동일시하지 않는다.

### 3.2 최소 marker와 검증 원칙

- 공용 어휘: launch-start, host-ready, framework-attached, first-scene, first-submit, first-content-evidence, input-ready, startup-failed.
- 기존 frame/resize/resource trace를 재사용하고 빈 부트 구간에만 marker를 추가한다. 새 범용 telemetry 시스템은 만들지 않는다.
- run ID, revision/artifact identity, platform/RID/configuration, renderer/backdrop, view/session/context/surface generation과 scene sequence를 연결한다. Web role 간 clock 기준을 명시하고 서로 다른 원시 performance.now를 직접 빼지 않는다.
- GPU submit/flush, API completion, browser front notification, 캡처에서 확인한 content, physical scan-out을 각각 다른 증거 수준으로 기록한다. presented라는 이름만으로 마지막 수준까지 확인됐다고 보고하지 않는다.
- first content는 clear/splash/Loading text가 아닌 해당 root의 유효 장면이다. input-ready는 pointer/key/첫 text client와 semantics 준비를 검증한다. 실제 한글 IME와 screen reader 결과는 별도로 남긴다.
- 명백한 동기 작업/할당 중복 제거는 profiler baseline 없이 구현한다. 숫자로 개선을 주장하거나 병렬화/AOT/캐시의 우열을 선택할 때만 짧은 대응 측정을 한다.
- 정량 비교는 같은 장치·Release·renderer·cache 조건, profiler off로 실행한다. 기본은 조건당 전후 10회 이내로 원시값과 중앙값을 기록하며 소표본 p95를 안정적인 tail 성능 보장으로 쓰지 않는다. native resize 대규모 matrix는 다시 돌리지 않는다.

## 4. 실행 작업과 완료 기준

P0–P6를 실행했다. 체크는 해당 구현 또는 명시된 검토의 완료이며 모든 하위 physical/performance acceptance의 PASS를 뜻하지 않는다. 장치/환경이 없는 항목과 부분 검증은 unchecked로 남긴다. 조건별 실제 결과와 보류 설계는 연결된 실행 결과 문서를 따른다.

### P0. 현재 부트 계약 고정

- [x] 위 source graph를 기준으로 최소 marker/evidence schema를 기존 validator에 연결한다. 실제 dependency 차이는 이 문서에 갱신한다.
- [x] Windows 기본 Vulkan opaque/Acrylic과 Web 기본 CanvasKit에 first-content/input-ready 판정을 연결한다. loader started의 의미를 소리 없이 변경하지 않는다.
- [x] TextField 없는 첫 화면, 첫 화면 TextField autofocus, system dark, accessibility-active 시나리오를 구분한다. 최소 fixture와 실제 Testbed를 둘 다 사용한다.
- [x] revision/evaluated property/artifact/default·explicit renderer를 결과에 남긴다. 과거 build 폴더 크기나 PASS를 새 baseline으로 복사하지 않는다.

완료 기준: 첫 장면과 startup failure를 식별하고 loader/GPU ready만으로 content PASS가 발생하지 않는다. 정량 baseline 수집은 모든 구조 개선의 선행 조건이 아니다.

### P1. Web 기본 경로의 불필요한 직렬 대기 축소

소유 파일: doroti.canvaskit.host.ts, doroti.ui.worker.ts, doroti.canvaskit.bootstrap.ts, DorotiWebWorkerRunner.cs, 공용 loader/types/contract 및 HTML template.

- [x] **자산 검증 병렬화:** manifest 검증 뒤 독립적인 JS/WASM fetch·length/hash 검증을 병렬 시작한다. 둘 다 성공하기 전 실행 금지, 실패 정리, 동일 origin/path/version/integrity 거부 계약을 보존한다.
- [x] **runtime 준비 분리:** CanvasKit 준비와 겹칠 수 있는 dotnet.js import/runtime resource 준비를 분리한다. runtime.create 선행은 callback/bridge 초기화에 안전한 범위만 허용한다. StartWorker/view attach는 GPU/text 준비 뒤 한 번만 호출한다.
- [x] **폰트 요청 앞당기기:** managed entry 뒤에서 시작하는 font waterfall을 줄인다. base path/CORS/cache가 일치하는 preload 또는 동일 session의 단일 요청 경로를 우선 적용한다. decode/register/resource ACK는 기존 소유층에서 처리하고 fallback font를 빼거나 첫 장면을 잘못된 글꼴로 그리지 않는다.
- [x] **부트 옵션 계약:** document용 blazorOptions와 Worker runtime 설정의 지원 범위를 API/type/docs에 명시한다. 필요한 Worker 옵션은 명시적 계약으로 연결하고 함수/Response를 무리하게 structured clone하지 않는다.
- [x] **preload/payload 재산정:** 기본 CanvasKit과 명시적 document의 실제 request graph를 따로 기록한다. 불필요한 Blazor loader preload와 필요한 dotnet/CanvasKit/font 우선순위를 정리하되 alternate renderer 및 fingerprint/importmap 계약을 유지한다.
- [x] **중복 처리 후속 판단:** main verification → Worker URL load의 요청/byte copy/compile를 확인한다. verified bytes/compiled module 재사용은 pinned upstream 지원과 ownership/integrity 동등성이 확인될 때만 별도 적용한다. runtime hash 삭제나 unverified cache hit로 대체하지 않는다.

완료 기준: TypeScript/loader 계약, 기본 CanvasKit 첫 content·한글 text·plugin·ARIA·resize/context recovery와 명시적 document 회귀 PASS. 404/hash mismatch/WASM init 실패 원인이 남고 Worker/port/canvas lease가 정리된다. 요청·검증·compile 절감과 사용자 부트 시간 개선은 별도 판정한다.

### P2. 공용 first-frame 비용의 남은 부분

- [x] **MAUI 관리 input 객체 lazy화:** Entry/Editor와 구독을 factory로 바꾸는 범위를 검토·구현한다. single-line 첫 client에는 해당 객체만, multiline 전환 시 나머지를 생성한다. focus/handler attach, hide/show, unload/reload, selection, disposal exactly-once를 유지한다.
- [x] **manifest 직접 생성 검토:** generated descriptor에 typed manifest를 제공해 JSON parsing/중간 할당을 없앨 수 있는지 SDK/boundary 계약으로 검토한다. public JSON Load 경로와 schema/RID/duplicate/plugin ABI/resource integrity 검증을 보존한다. 두 manifest를 따로 관리하거나 복잡한 runtime cache가 필요하면 후속으로 남긴다.
- [x] **현재 첫 화면의 도달 경로만 정리:** theme default/type initializer/DLR 중 현재 root의 build/layout/paint에서 도달하는 eager 생성·중복 변환만 고친다. 과거 C1/C2/C3 정적화 완료를 다시 작업으로 세지 않는다.
- [x] **semantics 유지:** 상태와 무관한 전체 지연은 하지 않는다. 첫 publish에 중복 snapshot/serialization이 있으면 내용·순서·actions를 보존하는 범위만 정리한다.

완료 기준: descriptor/boundary negative cases, session exactly-once, FCR scheduler/rendering/semantics/Material, lazy input lifecycle PASS. text 없는 화면의 관리 객체 생성 감소를 확인하고 첫 text 입력으로 비용이 과도하게 이동하지 않았는지 확인한다. 전체 Framework dynamic 제거는 범위 밖이다.

### P3. Windows Vulkan/Composition 부트 정리

- [x] **presenter 선택 선행:** renderer/GPU/backdrop 정책을 먼저 확정하고 selected backend별 native 검사 목록을 분리한다. Vulkan-only 부트의 ANGLE 존재/PE 검사 제거 가능성은 native host의 실제 import graph까지 확인한다.
- [x] **배포/검사 계약 동기화:** 복수 backend 지원을 유지하며 startup 필수 파일과 optional backend 파일을 구분한다. selected backend의 missing/wrong-architecture/ABI는 fail-closed로 처리하고 audit manifest/C9도 함께 갱신한다.
- [x] **Vulkan 초기화 중복 정리:** adapter/device/Skia/Composition/backdrop 준비에서 중복 probe/device가 있는지 확인해 실제 중복만 제거한다. UI/Composition/raster thread ownership과 첫 exact content 공개 순서를 보존한다.
- [x] **opaque/Acrylic 분리:** Acrylic activation은 요청 창에만 수행한다. 불필요한 작업과 first-present에 필수인 fence/commit 대기를 구분하고 첫 buffer 공개 전에 content를 준비한다.
- [ ] **명시적 경로 회귀:** ANGLE opaque/Acrylic, Windows MAUI, 진단 D3D12의 선택/실패 의미를 유지한다. Vulkan 실패를 ANGLE로 자동 fallback하지 않는다.

완료 기준: target Release publish, empty-PATH normal/audit/negative probe, Vulkan opaque/Acrylic 첫 content/input PASS. no app-local Vulkan loader/ICD와 provenance 계약 유지. resize/IME/Narrator/physical scan-out 미수행은 별도 notVerified이며 과거 resize FAIL을 닫지 않는다.

### P4. Android·Apple·Linux 적용과 남은 target gate

- [ ] **Android arm64:** 현재 signed APK 기준 install/process-cold/warm, foreground PID/activity, first content, 첫 text client, crash/ANR를 확인한다. profile strict decode/DEX 적합성과 ART compilation state를 검증하고 변경된 DEX에 맞춰 필요 시 CUJ profile을 재생성한다.
- [x] **Android x64:** AOT off + trim on Release emulator launch로 남은 runtime gate를 수행한다. 오류 artifact/원인을 보존하며 무조건 trimming 전체를 끄는 것을 기본 해법으로 삼지 않는다.
- [x] **Android 선택 실험:** Baseline Profile과 DEX layout용 Startup Profile/managed AOT profile을 구분한다. AOT 비교와 marshal methods 재활성화는 재현 가능한 fault 수정 및 장치 matrix가 확보됐을 때만 별도 진행한다.
- [ ] **Apple 3제품:** iOS physical/simulator, Catalyst, AppKit을 각각 Apple host에서 build/sign/launch한다. P2의 첫 focus, Metal first content, suspend/resume, 한글 IME/VoiceOver를 검증한다. DLR이 남은 상태에서 interpreter를 일괄 제거하지 않는다.
- [ ] **Linux:** framework-dependent published executable로 실제 Wayland/X11 각각 Qt/QPA/GL 초기화, first frame, input/IME/semantics를 검증한다. WSLg/VM은 별도 표시한다.

완료 기준: target별 build/runtime/input/accessibility 범위를 기록한다. Windows의 macOS sips 부재를 전체 solution 필수 gate로 반복하지 않고 Apple packaging은 Apple host에서 수행한다. 과거 해당 FAIL은 유지한다.

### P5. CLI 안전한 재실행과 build graph 개선

- [x] **fingerprint 입력 정확성 우선:** 상속 props/targets/package pin/SDK 선택, native source, binary resource/profile/font를 build dependency 기준으로 포함한다. 실제 입력 변경에도 같은 fingerprint가 되는 negative case를 먼저 막는다.
- [x] **출력 identity 확인:** 성공 artifact 목록/RID/configuration/toolchain을 state에 묶고 missing/변조/stale output을 거부한다. build 성공과 run 종료 기록 분리 여부 및 state schema upgrade/fail-closed 정책을 정한다.
- [x] **탐색 비용 축소:** generated output/dependency directory를 재귀 진입 전에 제외한다. package-lock/pin과 build input은 유지하며 mtime-only cache로 정확성을 낮추지 않는다. unrelated target invalidation은 의존 graph가 확인된 만큼만 줄인다.
- [x] **native/asset incremental:** Qt CMake, CanvasKit restore/prepare, TypeScript, Android/Apple binding target의 inputs/outputs를 검토한다. source/pin/toolchain 변경 시 재실행하고 해당 target build에만 참여하게 정리한다.
- [x] **template 동기화:** 공용 SDK/target에 구현하고 Testbed와 새 template 앱으로 clean build/재실행/changed-input/removed-output을 검증한다. README 영문/한글의 명령과 실제 재사용 단계를 맞춘다.

완료 기준: clean build는 필요한 단계 수행, unchanged run은 유효한 artifact 재사용, source/config/RID/native/resource/toolchain/output 불일치는 명확히 거부. fingerprint 시간을 포함한 developer launch 결과를 앱 runtime TTID와 분리한다.

### P6. 결과 정리와 채택

- [x] implemented, structural PASS, runtime PASS, performance notVerified를 구분한다. 환경 부재는 notVerified, 실제 실행 실패는 FAIL로 남긴다.
- [x] 정량 비교를 했다면 동일 조건 원시 launch 기록과 변화량을 남긴다. package byte/init 호출 수 감소만으로 TTID 개선률을 만들지 않는다.
- [x] 결과는 `history/<실행일>/cross-platform-boot-results.md`, 로그/JSON/capture는 `.doroti/evidence/boot/<run-id>/`에 저장하고 이 문서에서 연결한다.
- [x] 기본값/API/template/문서 일치와 미수행 physical/Apple/Linux/mobile Web/accessibility gate를 다음 작업 목록으로 남긴다.

## 5. 검증 진입점과 현재 상태

기존 project/script/Playwright fixture를 확장해 실행했다. 아래는 2026-09-07의 결과 요약이며 개별 실패·수정·재실행 label은 결과 문서에 보존한다.

| 영역 | 기존 진입점 | 신규 실행 상태 |
| --- | --- | --- |
| Bootstrap/공용 | app-bootstrap descriptor/lazy-input, dynamic-dispatch, FCR3/4/6/7 | PASS; native input acceptance 별도 |
| Web loader | loader contract, Host Web TypeScript | build/typecheck/404/hash/init cleanup/parallel gate PASS |
| Web 실제 앱 | startup, worker/display-list/resize/recovery, input/ARIA | content/한글 값 PASS; JS plugin endpoint PASS, managed roundtrip/physical IME 미검증 |
| Windows | C9 publish/product validator, MAUI publish/live | 최종 C9 PASS; 초기 2 FAIL 보존, MAUI frame/semantics PASS, D3D12 missing diagnostic 거부 확인; 물리 입력 미검증 |
| Android | signed Release 두 RID, strict profile, x64 emulator | build/profile PASS, x64 content/runtime 확인; arm64 runtime notVerified |
| Apple | iOS/Catalyst/AppKit 각 runner 및 Apple host | notVerified: Apple host 없음 |
| Linux | linux-qt-contract, published Qt Wayland/xcb | WSLg explicit D3D12 API smoke PASS; default llvmpipe FAIL, native input 미검증 |
| CLI/template | launch-identity/reuse contract, 새 source-wired template | build/reuse/negative PASS; NuGet release 설치는 미검증 |

- 모든 test/build 및 자식 프로세스는 [.github/copilot-instructions.md](.github/copilot-instructions.md)에 따라 **20분 timeout**을 적용하고, timeout 시 해당 작업이 시작한 process tree를 종료한다.
- 좁은 correctness gate가 통과하면 해당 target Release smoke로 진행한다. 문서 갱신만을 위해 full build/native drag/기기 설치를 수행하지 않는다.
- P1/P3은 기본 renderer 외 명시적 대체 경로도 검증한다. 실제 배포 Brotli/gzip/cache header, mobile browser, 물리 IME/accessibility는 로컬 desktop PASS와 별개다.

## 6. 범위 밖과 보류 기준

- Web full/partial AOT 재시도, CanvasKit 단일 Worker 재설계, Windows raw Win32 전환, Framework 전체 dynamic 제거, 전체 Material assembly 재분할은 필수 작업이 아니다.
- [CanvasKit 재설계 결과](history/26-09-05/web-canvaskit-redesign-v2-results.md)의 full AOT compiler 실패와 isolated partial-AOT browser startup stack overflow는 유지한다. publish 성공만으로 AOT startup 성공을 선언하거나 기존 실패를 재분류하지 않는다.
- [CanvasKit 2부 기록](history/26-09-05/web-canvaskit-redesign-v2-part2-results.md)의 resize/latency 실패와 [Vulkan resize 기록](history/26-09-05/windows-vulkan-acrylic-resize-summary.md)은 유지한다. cache 옵션·resize FPS·presentation topology를 부트 최적화에 섞어 변경하지 않는다.
- theme/manifest/input 중 작은 구조 변경으로 끝나지 않는 항목은 원인/예상 이득/호환 비용을 남기고 후속으로 분리한다. 정량 계측이 없다는 이유만으로 명백한 중복 제거를 중단하지 않는다.
- integrity/ABI/generation/ownership를 약화하거나 첫 입력/semantics/글꼴 품질을 희생하는 변경은 채택하지 않는다. 실패 원인과 원본 evidence를 보존한다.
- locale 축소, OS 최소 버전 상향, backend 제거, public API breaking change가 필요할 때 해당 제품 결정을 별도로 확정한다. 나머지 공용/기본 경로 개선은 그 결정을 기다리지 않고 진행한다.

# Doroti cross-platform 최초 부트 개선 작업 계획

- 작성일: 2026-08-28
- 상태: **계획 수립 완료, 구현·실기 측정은 모두 `notStarted`/`notVerified`**
- 목표: 설치된 Release 앱의 process cold start에서 첫 유효 Doroti content가 실제 compositor에 표시되고 입력 가능한 시점까지의 시간을 줄인다.
- 부목표: `doroti.ps1 run`의 restore/build/native build/AOT/deploy 시간도 계측하고 재사용 경로를 제공하되, 앱 runtime 부트 성능과 별도 지표로 유지한다.

## 1. 판정 범위

이 계획에서 “최초 부트”는 다음 네 구간을 섞지 않는다.

| 구간 | 정의 | 주 지표 |
| --- | --- | --- |
| first-install | 새 설치 또는 앱 데이터 초기화 뒤 첫 실행 | TTID, TTFD, package/runtime 준비 시간 |
| process cold | 설치된 Release 앱이 실행 중이지 않은 상태에서 새 process 시작 | TTID p50/p95 |
| warm/resume | process가 남아 있거나 background에서 resume | resume-to-present p50/p95 |
| developer launch | `doroti.ps1 run` 시작부터 build, deploy, process start까지 | restore/build/native/AOT/deploy/launch 구간별 시간 |

`TTID(Time To Initial Display)`는 splash/background clear가 아니라 첫 유효 Doroti content가 terminal `presented`에 도달한 시점이다. `TTFD(Time To Fully Drawn)`는 첫 content 표시 뒤 기본 pointer/key/text/accessibility 경로가 준비되고 startup main-thread 작업이 끝난 시점이다. Android에서는 TTID와 `reportFullyDrawn`에 대응하고, Web canvas는 DOM의 일반 FCP/LCP만 믿지 않고 첫 성공 WebGL commit marker를 사용한다.

다음 원칙을 유지한다.

- Debug, build 성공, splash 표시, process 생존은 Release TTID/TTFD의 PASS가 아니다.
- profiler가 켜진 trace는 원인 분석용이다. 최종 숫자는 profiler-free Release 반복 실행에서 얻는다.
- first-install, process cold, warm, emulator/simulator, VM, 물리 기기 결과는 서로 대신하지 않는다.
- accessibility를 끄거나 빈 화면을 먼저 present해서 숫자만 줄이지 않는다. 첫 content와 semantics-ready를 별도 marker로 기록한다.
- 기존 `presented`/`replayed`/`superseded`/`failed` terminal, 정확한 surface size, hardware GPU와 fail-closed 계약은 유지한다.
- 공용 원인은 가장 낮은 `Framework`/`Hosting`/renderer 소유층에서 고치고 DemoApp만 우회하지 않는다.

## 2. 현재 source·artifact 분석

아래 수치는 현재 checkout에 남아 있는 대표 Release 산출물의 정적 snapshot이다. clean publish와 실기 launch를 이번 계획 작성에서 새로 실행한 결과가 아니며, 지연의 인과관계도 아직 `notVerified`다.

### 2.1 공용 startup 경로

현재 모든 runner는 대체로 다음 순서를 사용한다.

```text
native/process entry
  -> generated DorotiBootstrap / DorotiApplicationFactory
  -> DorotiApplicationBoundary.Load(manifest JSON)
  -> platform window/view + GPU surface/context
  -> DorotiHostSession.Start(deferFrameworkBootstrap: true)
  -> RegisterView / AttachView
  -> WidgetsFlutterBinding 초기화
  -> root widget + ThemeData 생성
  -> build/layout/semantics/scene/raster
  -> native compositor present terminal
```

확인한 공용 개선 후보는 다음과 같다.

- `DorotiApplicationBoundary.Load`가 매 launch에서 embedded manifest를 `System.Text.Json`으로 역직렬화한다. manifest가 작아 우선 원인으로 단정할 수는 없지만 모든 플랫폼의 첫-frame closure에 들어간다.
- Demo의 `RootApp` 첫 접근은 light/dark `ThemeData`를 모두 만든다. `ThemeData.Create`는 typography와 다수 component theme default를 구성한다.
- `Doroti.Framework.Material`은 현재 약 4.29 MiB assembly/9.33 MiB source이고 정적 객체 초기화 후보가 매우 많다. animated icon과 typography 같은 대형 정적 object graph는 실제 first-frame type-initializer trace로 사용 여부를 가려야 한다.
- Framework에는 `dynamic`/DLR 관련 call site가 175개 파일, 1,747개 검색 일치로 남아 있다. first-frame closure에 들어온 call site는 Windows/Linux JIT, Android x64 JIT/interpreter, Apple AOT+interpreter, Web download/trimming에 동시에 영향을 줄 수 있다.
- MAUI surface는 첫 frame 전에 hidden `Entry`, `Editor`, semantics layer와 render surface를 만든다. Windows만 text proxy의 native attach를 지연하며 Android/iOS/Mac Catalyst/AppKit은 두 input view를 visual tree에 즉시 추가한다.
- MAUI와 Web은 view attach 직후 semantics tree를 활성화한다. 실제 accessibility 사용 중에는 이 동작을 보존해야 하지만, 비활성 상태에서도 first present와 같은 critical path에서 full semantics build/apply가 실행되는지는 측정이 필요하다.
- end-to-end process-entry→first-present startup trace는 없다. 기존 frame/resize/scroll trace는 framework가 시작된 뒤의 세부 증거라 runtime loader, host bootstrap, GPU context와 root/type 초기화를 하나의 시간축으로 비교할 수 없다.

### 2.2 Windows

기본 경로는 `WindowsAppSdk`/`HwndExactCpp` + managed ANGLE/EGL-D3D11이다. MAUI는 명시적 별도 backend다.

확인한 기본 경로의 후보:

- managed entry에서 native host, Windows App Runtime bootstrap, ANGLE DLL의 존재·PE를 검사한 뒤 세 파일의 SHA-256을 매 launch마다 전체 파일 read로 계산한다. 이 provenance hash는 diagnostics가 꺼져 있어도 실행된다.
- 이어서 ABI layout 검사, `RoInitialize`, `MddBootstrapInitialize2`, 세 HWND 생성, `AppWindow::GetFromWindowId`, render worker 시작이 순차 실행된다.
- 현재 native C++에서 `AppWindow`는 연결·보관·해제 외의 제품 동작에 사용되지 않는다. Windows App SDK identity를 유지할지 raw Win32 host로 재분류할지는 성능 A/B 뒤 ADR이 필요한 별도 architecture 결정이다.
- 첫 render callback에서 ANGLE D3D11 display/context, fixed EGL surface, Skia `GRContext`, window/backing surface가 생성되고 첫 swap 뒤 `DwmFlush`한다. 이 ordering은 first-frame 가시성 계약이므로 제거 대상이 아니라 구간별 측정 대상이다.
- 기존 Release app directory는 397개 파일/약 337.5 MiB였고 PDB와 현재 기본값이 아닌 D3D12 diagnostic dependency도 포함한다. directory 크기 자체가 startup 원인이라는 뜻은 아니지만 cold file scan, 배포와 loader working set을 분리해서 측정해야 한다.
- default ANGLE host assembly가 diagnostic D3D12 presenter와 Vortice/D3D12 package를 함께 참조한다. 기본 제품에서 진단 backend를 별도 assembly/package로 분리할 여지가 있다.

명시적 Windows MAUI 경로는 WinUI/MAUI/CommunityToolkit.Markup/SkiaSharp와 공용 hidden text/semantics 초기화 비용을 가진다. 기본 backend의 결과로 대체하지 않고 별도 baseline만 유지한다.

### 2.3 Android

- `android-arm64` Release는 full Mono AOT/trimming 경로다. 현재 APK는 약 25.85 MiB이고 assembly/AOT/native library 유사 entry가 131개다.
- `android-x64` Release는 알려진 Mono AOT startup fault를 피하려고 AOT와 trimming을 모두 끈다. 현재 APK는 약 42.13 MiB이며 raw `libassembly-store.so`가 약 35.71 MiB다.
- Android marshal methods도 startup fault 이력 때문에 전 RID에서 꺼져 있다. 근거 없이 다시 켜지 않고 현재 .NET 10 runtime에서 crash reproduction과 device matrix를 먼저 닫아야 한다.
- 현재 Release APK에는 app-owned Baseline Profile entry가 없다. Java/AndroidX startup profile과 managed AOT profile은 서로 다른 최적화이므로 각각 효과를 측정해야 한다.
- `MainApplication`에서 MAUI builder와 descriptor를 만들고, 첫 Activity view 생성 때 hidden `Entry`/`Editor`, semantics, `SKGLTextureView`/OpenGL ES 경로를 붙인다.
- x64 emulator는 arm64 physical 결과를 대신하지 않는다. 대표 성능 판정은 arm64 물리 기기에서 한다.

### 2.4 iOS, Mac Catalyst, native AppKit macOS

- iOS와 Mac Catalyst는 UIKit/MAUI, native AppKit은 `NSApplication` + experimental MAUI AppKit host를 사용한다. 세 제품을 별도 측정한다.
- Apple target은 managed assembly AOT와 DLR용 interpreter 제약을 함께 가진다. first-frame `dynamic` call site를 줄이지 않은 채 AOT/linker flag만 바꾸면 기능 회귀 또는 효과 없는 binary 증가가 될 수 있다.
- iOS/Mac Catalyst도 공용 hidden text input 두 개와 semantics layer를 첫 view에 즉시 만든다.
- iOS/Mac Catalyst GPU context는 첫 native drawable/paint에서 생성한다. AppKit은 `MTLDevice`, command queue, `MTKView`를 먼저 준비하고 첫 drawable에서 Skia Metal `GRContext`/surface를 만든다.
- 현재 Apple output directory는 각 target 약 130 MiB 수준이지만 최종 signed bundle/install footprint와 cold-launch mapped bytes는 별도 측정해야 한다.
- simulator, Mac Catalyst, AppKit 결과를 iPhone/iPad physical launch나 서로 다른 macOS backend의 결과로 바꾸지 않는다.

### 2.5 Web

- Web runner는 loader의 단일 `Blazor.start()`가 끝난 뒤 managed host를 만들고, 첫 Razor render 이후 JS host/WebGL surface/framework view를 연결한다.
- product contract가 `PublishTrimmed=false`를 강제하고 `WasmBuildNative=true`를 사용한다.
- 현재 Release `_framework`의 원본 initial payload는 약 47.2 MiB, gzip 파일 합계는 약 17.6 MiB다. `.wasm` 222개가 원본 약 41.0 MiB이고 PDB 21개가 약 3.0 MiB이며 전체 ICU data가 약 2.5 MiB다.
- Material 약 4.29 MiB, Widgets 약 2.55 MiB, Cupertino 약 0.96 MiB와 여러 범용 BCL assembly가 initial graph에 들어간다. first screen에 필요 없는 assembly/type data를 trim/lazy-load하지 못하는 것이 현재 가장 강한 정적 병목 후보다.
- `started` loader stage는 Blazor runtime 시작 완료일 뿐 Doroti canvas first commit이 아니다. download, WebAssembly compile, managed main, JS module import, WebGL context, first Doroti present를 나눠야 한다.

### 2.6 Linux/Qt

- Linux runner는 framework-dependent `net10.0`, managed-owned process + Qt C ABI v2이며 ReadyToRun/trimming startup 설정이 없다.
- managed descriptor/session을 만든 뒤 `QApplication`, `QOpenGLWindow`, accessibility factory를 만들고 framework view를 attach한 다음 window를 show한다.
- first paint에서 Qt current FBO를 Skia가 감싸고 `frameSwapped`이 유일한 presented terminal이다.
- Wayland acrylic registry 처리는 show 뒤 timer/event queue로 진행되므로 현재 source상 동기 roundtrip은 없지만, first present/TTFD와 겹치는지는 trace로 확인한다.
- `dotnet run`이 수행하는 CMake native build는 developer launch 시간이며 게시된 executable의 runtime TTID와 분리한다.

### 2.7 CLI/developer launch

`doroti.ps1 run`은 현재 항상 `dotnet run --project ... --configuration Release`를 호출한다. 따라서 첫 invocation에는 restore, C#/TypeScript/native build, Android/Apple AOT, deploy가 앱 launch 앞에 붙을 수 있다. 사용자가 체감한 “부트”가 이 전체 구간이라면 runtime 최적화만으로 해결되지 않는다.

## 3. 공통 측정 계약

### S0. startup event schema 추가

상태: `notStarted`

`Doroti.Ui` 또는 `Doroti.Hosting`에 allocation이 제한된 `DorotiStartupTrace`를 두고 다음 marker를 monotonic clock 하나로 기록한다.

| ID | marker | 의미 |
| --- | --- | --- |
| B00 | launchRequested | harness가 process/browser/activity launch를 요청 |
| B01 | nativeEntry | OS/native entrypoint 진입 |
| B02 | managedEntry | generated `DorotiBootstrap.Main` 또는 application delegate 진입 |
| B03 | descriptorReady | application descriptor/plugin registration 완료 |
| B04 | boundaryReady | manifest/resource/plugin boundary 준비 완료 |
| B05 | nativeViewReady | window/activity/view와 최소 host API 준비 |
| B06 | gpuContextReady | 실제 제품 GPU context 생성 완료 |
| B07 | frameworkReady | `WidgetsFlutterBinding` bootstrap 완료 |
| B08 | rootAttached | root element attach 완료 |
| B09 | firstSceneSubmitted | 첫 유효 scene이 renderer에 제출됨 |
| B10 | firstRasterSubmitted | GPU raster/submit 완료 |
| B11 | firstPresented | compositor 기반 첫 유효 content terminal |
| B12 | semanticsReady | 초기 접근성 tree 적용 완료 |
| B13 | fullyDrawn | 기본 입력/텍스트/semantics와 startup 후속 작업 완료 |

구현 조건:

- process/view/target/RID/configuration/artifact hash와 marker thread ID를 기록한다.
- Windows QPC, Android monotonic/Choreographer, Apple monotonic/signpost, browser `performance.now`, Linux monotonic clock을 managed 기준과 한 번만 상관시킨다.
- 기본 Release에서는 작은 in-memory ring만 유지하고 JSON 직렬화·파일 쓰기는 first present 이후 또는 종료 시 opt-in으로 수행한다.
- trace 자체의 on/off A/B에서 TTID p50 차이가 2% 또는 1 ms 중 큰 값 이하인지 검증한다.
- first content가 없는 background/splash/clear는 `firstPresented`로 기록하지 않는다.

### S1. baseline harness와 기준 동결

상태: `notStarted`

- 같은 commit의 고정 Release publish artifact를 사용한다. 각 반복에서 rebuild하지 않는다.
- first-install 5회, process cold 10회, warm/resume 20회를 기본 sample로 한다. variance가 크면 30회까지 늘리고 p50/p95/max를 남긴다.
- profiler-on 3회는 원인 분석용, profiler-free 반복은 acceptance용으로 분리한다.
- TTID=`B00→B11`, Doroti managed boot=`B02→B09`, GPU init=`B05→B06`, TTFD=`B00→B13`으로 고정한다.
- marker 사이 CPU time, wall time, allocation, JIT/type initializer, file I/O, loaded module/assembly, download bytes를 함께 수집한다.
- 결과는 `.doroti/evidence/startup/<commit>/<platform>/`에 raw JSON/trace로 저장하고, active `work.md`에는 요약과 PASS/FAIL/`notVerified`만 갱신한다.

초기 개선 목표는 플랫폼별 baseline 확정 뒤 다음 상대 gate로 고정한다.

- process-cold TTID p50 30% 이상 단축
- process-cold TTID p95 20% 이상 단축
- TTFD와 warm/resume p95가 5% 넘게 악화되지 않음
- crash/ANR/hang/blank first frame 0, terminal exactly-once 위반 0
- first-present 직후 입력 처리와 accessibility-active semantics가 누락되지 않음

reference device가 정해지기 전에는 임의의 절대 500 ms/1 s 기준으로 PASS를 선언하지 않는다.

## 4. 실행 순서

### S2. 공용 first-frame closure 축소

상태: `notStarted`

S0/S1에서 각 변경의 marker 기여도를 확인하며 다음 순서로 진행한다.

1. **Typed manifest/bootstrap**
   - Runner SDK가 manifest의 resource/plugin descriptor를 generated C# 배열 또는 source-generated JSON context로 만든다.
   - 매 launch reflection 기반 JSON metadata 초기화를 제거하되 resource hash, RID, plugin ABI fail-closed 검증은 유지한다.
   - manifest가 작아 개선이 측정 오차 이하이면 구조 복잡도를 늘리지 않고 `noBenefit`으로 종료한다.

2. **Theme와 대형 type initializer**
   - EventPipe/loader trace로 `B07→B09`에 실제 실행된 type initializer와 allocation 상위를 뽑는다.
   - Demo가 active/inactive light/dark `ThemeData`를 모두 동기 생성하지 않도록 호환 API의 lazy factory를 설계한다.
   - `ThemeData.Create`의 immutable default typography/component theme는 안전한 shared template/copy-on-write 후보로 측정한다.
   - first frame에서 사용하지 않은 animated icon/path table과 Cupertino catalog는 type-local `Lazy<T>` 또는 별도 lazy-load assembly/resource로 이동한다.
   - 전체 9천여 정적 후보를 기계적으로 바꾸지 않고 trace에 들어온 type과 Web payload 기여가 큰 data만 수정한다.

3. **First-frame `dynamic` 제거**
   - `B07→B11`에 실제 bind된 DLR call site 목록과 JIT/AOT fallback 시간을 수집한다.
   - layout/build/paint/semantics의 first-frame closure부터 typed generic/interface 호출로 바꾼다.
   - public 의미와 Flutter reference 동작을 유지하며, 전체 Framework의 일괄 변환은 별도 follow-up으로 둔다.
   - Web trim/AOT warning과 Apple interpreter 필요 범위가 줄었는지 target별로 검증한다.

4. **Lazy platform service**
   - hidden `Entry`/`Editor` native handler는 첫 text client activation 때 만든다. text field가 initial screen에 있으면 TTFD 전에 생성하고 caret/IME를 잃지 않는다.
   - accessibility가 OS에서 활성인 경우 초기 semantics를 지연하지 않는다. 비활성인 경우 first content present 다음 idle/frame으로 옮기는 A/B만 허용한다.
   - clipboard/plugin/native bridge, shader warm-up과 image cache는 first screen이 요구하는 최소 항목만 동기 준비한다.

5. **불필요한 MAUI registration 제거**
   - 제품 source가 직접 쓰지 않는 `CommunityToolkit.Maui.Markup` registration/package를 제거하는 graph A/B를 수행한다.
   - Skia handler나 platform lifecycle에 transitive side effect가 있으면 유지하고 결과를 `noBenefit`으로 기록한다.

S2 공용 gate:

- FCR-0/FCR-3/FCR-4/FCR-6/FCR-7 Release validation PASS
- light/dark first frame, system theme live switch, initial text field/IME, accessibility-active launch 회귀 0
- 모든 platform source/build gate PASS, 실제 실행하지 못한 target은 `notVerified`

### S3. Windows 기본/MAUI 분리 최적화

상태: `notStarted`

#### W1. 기본 HwndExactCpp 빠른 경로

1. native DLL SHA-256을 build/publish manifest에 기록하고 일반 launch에서는 file metadata + PE header + ABI/version만 fail-fast 검증한다. 전체 hash 재계산은 diagnostics/audit 명령에서만 수행한다.
2. `B02→B11`을 `RoInitialize`, Windows App Runtime bootstrap, HWND, `AppWindow`, render worker, ANGLE display/context, Skia context/surface, first scene, swap, `DwmFlush`로 세분한다.
3. 기본 ANGLE assembly/package에서 D3D12 diagnostic presenter와 Vortice/D3D12 dependency를 분리한다. `DOROTI_WINDOWS_PRESENTER=D3D12`는 별도 진단 artifact가 있을 때만 명시적으로 허용하고 silent fallback은 두지 않는다.
4. clean publish에서 PDB, diagnostic symbols, 사용하지 않는 optional runtime asset을 배포 파일에서 제외한다. Windows App SDK가 지원하는 self-contained file set만 사용하고 임의 DLL 삭제는 금지한다.
5. 일반 ReadyToRun on/off를 같은 self-contained artifact 조건에서 A/B한다. JIT 감소가 file I/O/working-set 증가보다 클 때만 채택한다.
6. first exact present 전 show 금지와 첫 swap 후 `DwmFlush`는 유지한다. 이를 제거한 숫자는 acceptance에 사용하지 않는다.

#### W2. Windows App SDK identity 후속 결정

`MddBootstrapInitialize2`/`AppWindow`가 dominant이고 `AppWindow` 기능이 실제로 불필요하다는 A/B 증거가 있을 때만 다음 둘 중 하나를 operator decision/ADR로 선택한다.

- Windows App SDK backend identity를 유지하고 해당 비용을 수용한다.
- raw Win32 + ANGLE host를 새 backend/target package로 분리하고 기존 Windows App SDK 계약과 migration을 문서화한다.

기존 backend 이름만 유지한 채 bootstrap을 몰래 제거하지 않는다. 이 항목은 MVP 후속이다.

#### W3. 명시적 Windows MAUI

- S2 lazy text/semantics/package 정리를 적용한다.
- WinUI bootstrap, MAUI DI/build, DXGI/Skia context와 first present를 별도 marker로 측정한다.
- 기본 HwndExactCpp보다 느리다는 이유로 제거하거나 자동 fallback하지 않는다.

Windows acceptance:

- target-scoped Release publish, empty `PATH` launch와 native provenance audit PASS
- process-cold/warm 각 반복에서 first exact content visible, failed terminal 0
- resize/mixed-DPI/input 기존 gate 유지
- 실제 한글 IME/Narrator는 수행 전까지 `notVerified`

### S4. Android arm64 physical 우선 최적화

상태: `notStarted`

1. 고정 serial의 arm64 물리 기기에서 `am start -W`, Perfetto, managed startup trace를 연결하고 TTID/TTFD를 수집한다. x64 emulator는 개발 편의 baseline만 둔다.
2. arm64 full AOT, default/profiled AOT, Doroti first-screen custom AOT profile을 같은 Release APK에서 A/B한다. AOT library preload/lazy-load, mapped bytes와 first-frame JIT fallback을 함께 본다.
3. app startup Critical User Journey로 Android Baseline Profile과 Startup Profile을 생성·패키징하고 `baseline.prof`, dex layout, device compilation state를 검증한다. ART profile 개선과 managed Mono AOT 개선을 별도 수치로 기록한다.
4. x64는 AOT off를 유지한 채 trimming만 다시 켤 수 있는지 분리 실험한다. 현재 35.71 MiB assembly store와 startup JIT/type load가 실제로 줄고 emulator startup fault가 재발하지 않을 때만 채택한다.
5. `AndroidEnableMarshalMethods`는 기존 startup fault의 최소 재현, fixed runtime 확인과 arm64/x64 matrix가 먼저다. 재활성화 후보가 실패하면 계속 off로 두고 `FAIL` 근거를 남긴다.
6. hidden text handler, semantics, inactive theme를 lazy화하고 첫 화면에 TextField가 있는 별도 cold-start case로 TTFD/IME를 검증한다.
7. splash 종료 시점과 Doroti first content를 같은 color라서 오인하지 않도록 screenshot/pixel + `B11`을 함께 사용한다.

Android acceptance:

- fixed physical arm64 Release first-install/cold/warm 반복 gate PASS
- APK 설치, foreground Activity/PID, screenshot, crash/ANR, `gfxinfo`/Perfetto 확인
- first input, text selection/overlay, TalkBack-active launch와 scroll cadence 회귀 확인
- emulator x64 PASS는 physical arm64 PASS를 대신하지 않음

### S5. Apple 세 제품 독립 최적화

상태: `notStarted`

1. generated entry, UIApplication/NSApplication, MAUI builder, native view, Metal context, root/type init, first command-buffer completion에 signpost/startup marker를 연결한다.
2. iOS physical, iOS simulator, Mac Catalyst, native AppKit을 별도 artifact와 결과표로 측정한다.
3. S2의 first-frame `dynamic` 제거 뒤 AOT/interpreter 범위를 재평가한다. linker/AOT/registrar 설정은 size, launch, native bridge와 reflection 회귀를 같이 통과할 때만 변경한다.
4. hidden `Entry`/`Editor` handler와 inactive theme를 lazy화한다. 첫 화면 TextField/VoiceOver-active case는 TTFD 전에 준비한다.
5. Metal device/queue/context를 더 일찍 prewarm하는 실험은 main-thread blocking이 줄고 TTID가 실제 개선될 때만 채택한다. drawable 준비 전 불필요한 GPU 생성은 금지한다.
6. signed install, dyld/managed assembly mapping, AOT fallback, first drawable 대기를 Instruments/xctrace로 분해한다.

Apple acceptance:

- iPhone/iPad physical iOS, Mac Catalyst, AppKit Release launch 결과를 각각 기록
- 첫 Metal completion과 visible screenshot 일치, failed/stale terminal 0
- Korean IME/VoiceOver/signing/notarization을 실제 수행하지 않으면 `notVerified`

### S6. Web payload/compile 경로 우선 최적화

상태: `notStarted`

1. navigation, loader, boot resource download, `Blazor.start`, managed main, JS module import, WebGL context, framework attach, first WebGL commit에 `performance.mark/measure`와 `DorotiStartupTrace`를 연결한다.
2. generated Framework graph가 trim-safe해지도록 first screen에 필요한 reflection/dynamic root를 명시하고 product Release의 `PublishTrimmed=true` 금지 gate를 단계적으로 제거한다.
3. trim warning 0만으로 PASS하지 않고 Material gallery, plugin, image/font/shader, semantics, resize, text input browser live gate를 통과한다.
4. initial route에서 필요 없는 Cupertino, animated icon/catalog, diagnostics와 optional BCL assembly를 assembly lazy loading 또는 package 분할로 뒤로 보낸다. root Material/Skia/runtime assembly는 억지 lazy-load하지 않는다.
5. Release publish에서 PDB/source map을 symbol artifact로 분리하고 실제 HTTP Brotli/gzip, immutable fingerprint cache header를 검증한다.
6. 지원 locale을 operator가 확정한 뒤 필요한 ICU shard만 initial load하거나 추가 culture data를 lazy-load한다. 한국어 text/IME/locale fallback이 깨지면 full ICU를 유지한다.
7. service worker/repeat cache는 warm navigation만 개선한 것으로 기록하고 fresh-profile cold TTID를 대체하지 않는다.
8. initial compressed payload 50% 감소를 1차 목표로 두되, WebGL/Skia compile과 first scene이 dominant라면 payload 숫자만으로 완료하지 않는다.

Web acceptance:

- clean Release publish + 실제 HTTP compression과 fresh browser profile 10회
- desktop Chrome와 한 개 mid-tier mobile browser/device의 cold/repeat 결과
- first canvas pixel, pointer/key/IME/clipboard/ARIA/resize live PASS
- loader `started`만으로 first-present PASS 선언 금지

### S7. Linux/Qt startup 최적화

상태: `notStarted`

1. published executable 기준으로 hostfxr/CoreCLR, managed main, `dlopen` Qt/Skia/native shim, `QApplication`, QPA plugin, `QOpenGLWindow`, GL/Skia context, first `frameSwapped`을 marker로 나눈다.
2. `perf`, `strace` file-open summary, `LD_DEBUG=statistics`, EventPipe JIT/type initializer를 원인 분석용으로 수집한다.
3. framework-dependent IL, ReadyToRun, self-contained ReadyToRun을 같은 machine/session에서 A/B한다. startup 이득, output size, RSS와 steady-state frame time을 모두 비교한다.
4. S2 lazy text/semantics/theme를 적용하고 QAccessible factory/backdrop event가 TTFD에 미치는 영향을 확인한다.
5. native shim은 published artifact에서 재빌드하지 않는다. CMake incremental 개선은 developer launch 항목에서 별도로 처리한다.

Linux acceptance:

- 실제 Wayland와 실제 X11 session을 독립 측정하고 VM/WSLg는 보조 결과로만 유지
- first `frameSwapped`, screenshot, input/IME/accessibility와 terminal invariant 확인
- 물리 Linux/한글 IME/Orca를 실행하지 않으면 `notVerified`

### S8. CLI/developer iteration 개선

상태: `notStarted`

1. `doroti.ps1 run`에 workspace resolve, restore, managed compile, TypeScript/native build, AOT/link, deploy, process launch, B11 대기 구간 timer를 추가한다.
2. `-NoBuild`/`-NoRestore` 또는 명시적 fast-run option을 설계한다. runner/configuration/RID/artifact fingerprint가 맞지 않으면 stale binary를 실행하지 않고 재build 필요를 명확히 실패시킨다.
3. Release 기본값은 acceptance 재현성을 위해 유지한다. 일상 개발에는 명시적 Debug fast path와 “마지막 성공 artifact 재실행” 명령을 문서화한다.
4. Android Gradle AAR, Apple Xcode binding, Linux CMake와 Web TypeScript output은 input hash가 같을 때 재사용되는지 MSBuild binlog로 확인한다.
5. CLI 개선 결과는 `command→process start`와 `process start→B11`을 반드시 따로 출력한다.

CLI acceptance:

- clean first run은 필요한 build/deploy를 생략하지 않음
- no-build run은 동일 artifact를 사용하고 runtime TTID가 일반 run과 같음
- stale RID/config/native binding은 fail-closed
- README.md/README.ko.md와 template 명령 동기화

## 5. MVP와 후속 경계

### MVP

1. S0 startup trace와 S1 baseline harness
2. 실제 trace 상위 공용 병목: inactive theme/type initializer, first-frame DLR, hidden text/semantics
3. Windows production hash 제거와 D3D12 diagnostic 분리
4. Android arm64 physical AOT/profile 비교와 x64 trimming 분리
5. Web trimming 해제에 필요한 최소 graph 정리, production symbol/ICU/payload 축소
6. Apple/Linux marker와 각 platform baseline, 효과가 확인된 S2 공용 변경 적용
7. CLI phase timer와 안전한 no-build 재실행

### 후속

- Windows App SDK를 raw Win32 target으로 대체/병행하는 architecture 변경
- 전체 Framework의 `dynamic` 제거, 전체 assembly 재분할 또는 NativeAOT 전환
- 모든 Material/Cupertino catalog의 binary/resource 재설계
- app-specific deferred route/data/network loading
- store installer, production telemetry sampling, cloud profile/field metric 운영

## 6. 검증 matrix와 상태 기록

| Gate | 필수 결과 | 현재 상태 |
| --- | --- | --- |
| G0 source/schema | startup marker ordering, trace overhead, no pre-present file I/O | `notStarted` |
| G1 common runtime | FCR-0/3/4/6/7, theme/text/semantics first-frame 회귀 없음 | `notStarted` |
| G2 Windows default | HwndExactCpp Release cold/warm, exact visible present, input/resize 유지 | `notVerified` |
| G3 Windows MAUI | 독립 Release cold/warm baseline과 common regression | `notVerified` |
| G4 Android arm64 | physical first-install/cold/warm, ANR/crash/screenshot/TTID/TTFD | `notVerified` |
| G5 Android x64 | emulator dev path, AOT-off + trim 후보와 startup fault | `notVerified` |
| G6 iOS physical | signed Release cold/warm, Metal present, IME/VoiceOver | `notVerified` |
| G7 Mac Catalyst | 독립 Release cold/warm, Metal present | `notVerified` |
| G8 AppKit | 독립 Release cold/warm, Metal completion present | `notVerified` |
| G9 Web | trimmed clean publish, payload, fresh/repeat browser first commit | `notVerified` |
| G10 Linux | actual Wayland/X11 published launch와 frameSwapped | `notVerified` |
| G11 CLI | clean/fast developer launch phase 분리와 stale artifact 거부 | `notStarted` |
| G12 cross-target | package/template/README와 전체 Release regression | `notStarted` |

모든 test/build job에는 repository 지침대로 20분 timeout을 적용한다. 한 플랫폼의 PASS나 사용자의 체감 개선으로 다른 플랫폼의 자동 FAIL/`notVerified`를 PASS로 바꾸지 않는다.

## 7. 우선순위와 중단 기준

구현 우선순위는 다음과 같다.

1. 계측·baseline 없이는 최적화에 착수하지 않는다.
2. 여러 플랫폼에 동시에 나타난 `B07→B09` Framework 병목을 가장 먼저 고친다.
3. 현재 정적 증거가 큰 Web initial payload, Windows launch-time hashing/diagnostic graph, Android x64 untrimmed store를 플랫폼 우선 후보로 둔다.
4. 각 변경은 한 번에 한 변수만 A/B하고 p50/p95, TTFD, size/RSS, correctness를 함께 비교한다.
5. 개선이 측정 오차 이하이거나 다른 gate를 5% 넘게 악화시키면 제거하고 `noBenefit`/`FAIL` 근거를 남긴다.

## 8. 구현 전에 확정할 operator 결정

- 기준 장치/OS: Android physical serial과 refresh rate, iPhone/iPad, macOS 기기, Windows CPU/GPU, Linux Wayland/X11, Web mobile device/network profile
- 제품 절대 목표: 상대 30%/20% gate 뒤 허용할 플랫폼별 TTID/TTFD 상한
- Web locale 범위: 한국어/영어 고정인지 임의 locale의 offline 전환까지 필요한지
- Windows deployment: self-contained Windows App SDK identity를 반드시 유지할지 raw Win32 + ANGLE 신규 target을 허용할지
- public API: `MaterialApp`/`ThemeData` lazy factory를 additive API로 노출할지 내부 shared-default 최적화만 허용할지
- 사용자가 말한 “처음 부트”의 주 증상이 developer first run인지 설치된 앱의 cold TTID인지. 둘 다 측정하되 실제 우선순위는 baseline에서 확정한다.

## 9. 구현 참고 자료

- [.NET ReadyToRun deployment](https://learn.microsoft.com/dotnet/core/deploying/ready-to-run)
- [.NET for Android build properties](https://learn.microsoft.com/dotnet/android/building-apps/build-properties)
- [Android Baseline Profiles](https://developer.android.com/topic/performance/baselineprofiles/overview)
- [Blazor WebAssembly lazy-loaded assemblies](https://learn.microsoft.com/aspnet/core/blazor/webassembly-lazy-load-assemblies)
- [Blazor WebAssembly host/deploy and Release trimming](https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly/)

> 현재 결론: source와 기존 artifact만으로 Web payload, Windows production hashing/diagnostic dependency, Android x64 untrimmed store, 공용 ThemeData/type initializer/DLR/hidden platform view를 강한 후보로 좁혔다. 어느 후보도 live startup trace 전에는 확정 원인이나 완료로 판정하지 않는다.

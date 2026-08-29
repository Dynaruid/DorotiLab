# Doroti cross-platform 최초 부트 개선 작업 계획

- 작성일: 2026-08-28
- 상태: **2026-08-29 MVP 구현 실행 완료**. 구조적으로 안전한 공용/Windows/Android/Web/Linux/CLI 항목은 구현했고, 장치·OS·배포 환경이 필요한 gate와 선택적 tuning/operator 결정은 `notVerified` 또는 후속으로 유지한다. 판정과 증거는 `history/26-08-29/cross-platform-first-boot-implementation.md`에 기록했다.
- 목표: 설치된 Release 앱의 process cold start에서 첫 유효 Doroti content가 실제 compositor에 표시되고 입력 가능한 시점까지의 시간을 줄인다.
- 부목표: `doroti.ps1 run`의 불필요한 restore/build/native build/AOT/deploy를 줄이고 안전한 재사용 경로를 제공하되, 앱 runtime 부트 개선과 별도 작업으로 유지한다.

## 1. 판정 범위

이 계획에서 “최초 부트”는 다음 네 구간을 섞지 않는다.

| 구간 | 정의 | 정량 보고 시 지표 |
| --- | --- | --- |
| first-install | 새 설치 또는 앱 데이터 초기화 뒤 첫 실행 | TTID, TTFD, package/runtime 준비 시간 |
| process cold | 설치된 Release 앱이 실행 중이지 않은 상태에서 새 process 시작 | TTID p50/p95 |
| warm/resume | process가 남아 있거나 background에서 resume | resume-to-present p50/p95 |
| developer launch | `doroti.ps1 run` 시작부터 build, deploy, process start까지 | restore/build/native/AOT/deploy/launch 구간별 시간 |

`TTID(Time To Initial Display)`는 splash/background clear가 아니라 첫 유효 Doroti content가 terminal `presented`에 도달한 시점이다. `TTFD(Time To Fully Drawn)`는 첫 content 표시 뒤 기본 pointer/key/text/accessibility 경로가 준비되고 startup main-thread 작업이 끝난 시점이다. Android에서는 TTID와 `reportFullyDrawn`에 대응하고, Web canvas는 DOM의 일반 FCP/LCP만 믿지 않고 첫 성공 WebGL commit marker를 사용한다.

다음 원칙을 유지한다.

- Debug, build 성공, splash 표시, process 생존은 Release TTID/TTFD의 PASS가 아니다.
- profiler가 켜진 trace는 원인 분석용이다. 정량 성능을 실제로 보고하는 경우에만 profiler-free Release 반복 실행에서 최종 숫자를 얻는다.
- first-install, process cold, warm, emulator/simulator, VM, 물리 기기 결과는 서로 대신하지 않는다.
- accessibility를 끄거나 빈 화면을 먼저 present해서 숫자만 줄이지 않는다. 정량 추적을 할 때는 첫 content와 semantics-ready를 구분한다.
- 기존 `presented`/`replayed`/`superseded`/`failed` terminal, 정확한 surface size, hardware GPU와 fail-closed 계약은 유지한다.
- 공용 원인은 가장 낮은 `Framework`/`Hosting`/renderer 소유층에서 고치고 DemoApp만 우회하지 않는다.
- source와 실행 구조상 동기 작업·할당·DLR/reflection·중복 초기화를 제거하면 분명히 일이 줄어드는 변경은 새 profiler/baseline 없이 구현한다.
- 계측은 설계 선택이 애매하거나 회귀가 의심되거나 정량 성능 수치를 보고해야 할 때만 사용한다. 계측 미수행은 구조 개선의 착수 조건이나 자동 rollback 사유가 아니다.

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
- `Doroti.Framework.Material`은 현재 약 4.29 MiB assembly/9.33 MiB source이고 정적 객체 초기화 후보가 매우 많다. 기본 root가 직접 참조하지 않는 animated icon과 catalog 같은 대형 정적 object graph는 source reachability와 초기화 구조를 기준으로 first-frame 밖으로 옮긴다.
- Framework에는 `dynamic`/DLR 관련 call site가 175개 파일, 1,747개 검색 일치로 남아 있다. first-frame closure에 들어온 call site는 Windows/Linux JIT, Android x64 JIT/interpreter, Apple AOT+interpreter, Web download/trimming에 동시에 영향을 줄 수 있다.
- 2026-08-28 S2 실행에서 first-frame 공용 반복 경로의 C1 render-tree, C2 Navigator/Route, C3 Actions를 direct/interface/non-generic base 계약으로 정적화했다. 선택 owner의 compiler-generated `CallSite<T>`는 0개이며 `Doroti.Framework.Widgets` 전체 field는 기준 1,386개에서 1,128개로 감소했다. 이는 구조 지표이고 정량 TTID/CPU 개선 수치는 `notVerified`다.
- MAUI surface는 첫 frame 전에 hidden `Entry`, `Editor`, semantics layer와 render surface를 만든다. Windows만 text proxy의 native attach를 지연하며 Android/iOS/Mac Catalyst/AppKit은 두 input view를 visual tree에 즉시 추가한다.
- MAUI와 Web은 view attach 직후 semantics tree를 활성화한다. 실제 accessibility 사용 중에는 이 동작을 보존하고, 비활성 상태의 full semantics build/apply는 first present 뒤로 안전하게 지연할 수 있는지 lifecycle과 기능 검증으로 판단한다.
- end-to-end process-entry→first-present startup trace는 없다. 따라서 이 계획은 trace 구축을 기다리지 않고 source에서 확인되는 동기 hashing, parsing, DLR, 미사용 초기화와 package graph부터 직접 줄인다.

### 2.2 Windows

기본 경로는 `WindowsAppSdk`/`HwndExactCpp` + managed ANGLE/EGL-D3D11이다. MAUI는 명시적 별도 backend다.

확인한 기본 경로의 후보:

- managed entry에서 native host, Windows App Runtime bootstrap, ANGLE DLL의 존재·PE를 검사한 뒤 세 파일의 SHA-256을 매 launch마다 전체 파일 read로 계산한다. 이 provenance hash는 diagnostics가 꺼져 있어도 실행된다.
- 이어서 ABI layout 검사, `RoInitialize`, `MddBootstrapInitialize2`, 세 HWND 생성, `AppWindow::GetFromWindowId`, render worker 시작이 순차 실행된다.
- 현재 native C++에서 `AppWindow`는 연결·보관·해제 외의 제품 동작에 사용되지 않는다. Windows App SDK identity를 유지할지 raw Win32 host로 재분류할지는 필요한 제품 기능과 배포 계약을 기준으로 ADR에서 결정한다.
- 첫 render callback에서 ANGLE D3D11 display/context, fixed EGL surface, Skia `GRContext`, window/backing surface가 생성되고 첫 swap 뒤 `DwmFlush`한다. 이 ordering은 first-frame 가시성 계약이므로 제거 대상이 아니다.
- 기존 Release app directory는 397개 파일/약 337.5 MiB였고 PDB와 현재 기본값이 아닌 D3D12 diagnostic dependency도 포함한다. 배포에 필요하지 않은 symbol과 diagnostic dependency는 loader 기여도 측정을 기다리지 않고 제품 artifact에서 분리한다.
- default ANGLE host assembly가 diagnostic D3D12 presenter와 Vortice/D3D12 package를 함께 참조한다. 기본 제품에서 진단 backend를 별도 assembly/package로 분리할 여지가 있다.

명시적 Windows MAUI 경로는 WinUI/MAUI/CommunityToolkit.Markup/SkiaSharp와 공용 hidden text/semantics 초기화 비용을 가진다. 기본 backend와 섞지 않고 독립 제품 경로로 검증한다.

### 2.3 Android

- `android-arm64` Release는 full Mono AOT/trimming 경로다. 현재 APK는 약 25.85 MiB이고 assembly/AOT/native library 유사 entry가 131개다.
- `android-x64` Release는 알려진 Mono AOT startup fault를 피하려고 AOT와 trimming을 모두 끈다. 현재 APK는 약 42.13 MiB이며 raw `libassembly-store.so`가 약 35.71 MiB다.
- Android marshal methods도 startup fault 이력 때문에 전 RID에서 꺼져 있다. 근거 없이 다시 켜지 않고 현재 .NET 10 runtime에서 crash reproduction과 device matrix를 먼저 닫아야 한다.
- 현재 Release APK에는 app-owned Baseline Profile entry가 없다. Java/AndroidX startup profile과 managed AOT profile은 서로 다른 최적화이므로 별도 계약으로 추가·검증한다.
- 2026-08-28 S2 검증 artifact는 `android-arm64` signed APK 27,384,351 bytes였다. `R3CY30KZA4B`에 설치해 first frame, foreground PID/focus, pointer state 변경, scroll, crash/ANR 0건을 확인했지만 이는 Baseline Profile이나 cold/warm 정량 개선 결과가 아니다.
- `MainApplication`에서 MAUI builder와 descriptor를 만들고, 첫 Activity view 생성 때 hidden `Entry`/`Editor`, semantics, `SKGLTextureView`/OpenGL ES 경로를 붙인다.
- x64 emulator는 arm64 physical 결과를 대신하지 않는다. 대표 성능 판정은 arm64 물리 기기에서 한다.

### 2.4 iOS, Mac Catalyst, native AppKit macOS

- iOS와 Mac Catalyst는 UIKit/MAUI, native AppKit은 `NSApplication` + experimental MAUI AppKit host를 사용한다. 세 제품을 별도 검증한다.
- Apple target은 managed assembly AOT와 DLR용 interpreter 제약을 함께 가진다. first-frame `dynamic` call site를 줄이지 않은 채 AOT/linker flag만 바꾸면 기능 회귀 또는 효과 없는 binary 증가가 될 수 있다.
- iOS/Mac Catalyst도 공용 hidden text input 두 개와 semantics layer를 첫 view에 즉시 만든다.
- iOS/Mac Catalyst GPU context는 첫 native drawable/paint에서 생성한다. AppKit은 `MTLDevice`, command queue, `MTKView`를 먼저 준비하고 첫 drawable에서 Skia Metal `GRContext`/surface를 만든다.
- 현재 Apple output directory는 각 target 약 130 MiB 수준이다. signed bundle에 불필요한 symbol/resource를 제외하고, 실제 install footprint는 실행 환경이 있을 때 별도 기록한다.
- simulator, Mac Catalyst, AppKit 결과를 iPhone/iPad physical launch나 서로 다른 macOS backend의 결과로 바꾸지 않는다.

### 2.5 Web

- Web runner는 loader의 단일 `Blazor.start()`가 끝난 뒤 managed host를 만들고, 첫 Razor render 이후 JS host/WebGL surface/framework view를 연결한다.
- product contract가 `PublishTrimmed=false`를 강제하고 `WasmBuildNative=true`를 사용한다.
- 현재 Release `_framework`의 원본 initial payload는 약 47.2 MiB, gzip 파일 합계는 약 17.6 MiB다. `.wasm` 222개가 원본 약 41.0 MiB이고 PDB 21개가 약 3.0 MiB이며 전체 ICU data가 약 2.5 MiB다.
- Material 약 4.29 MiB, Widgets 약 2.55 MiB, Cupertino 약 0.96 MiB와 여러 범용 BCL assembly가 initial graph에 들어간다. first screen에 필요 없는 assembly/type data를 trim/lazy-load하지 못하는 것이 현재 가장 강한 정적 병목 후보다.
- `started` loader stage는 Blazor runtime 시작 완료일 뿐 Doroti canvas first commit이 아니다. download, WebAssembly compile, managed main, JS module import, WebGL context, first Doroti present를 나눠야 한다.
- 2026-08-28 S2 검증 publish의 `wwwroot`는 714개 파일, plain 46,255,091 bytes, Brotli 12,932,420 bytes, gzip 16,976,949 bytes였다. Chrome live에서 실제 canvas first present, text input, pointer action, scroll과 console error/warning 0건을 확인했다. trimming/payload 최적화 자체는 아직 수행하지 않았다.

### 2.6 Linux/Qt

- Linux runner는 framework-dependent `net10.0`, managed-owned process + Qt C ABI v2이며 ReadyToRun/trimming startup 설정이 없다.
- managed descriptor/session을 만든 뒤 `QApplication`, `QOpenGLWindow`, accessibility factory를 만들고 framework view를 attach한 다음 window를 show한다.
- first paint에서 Qt current FBO를 Skia가 감싸고 `frameSwapped`이 유일한 presented terminal이다.
- Wayland acrylic registry 처리는 show 뒤 timer/event queue로 진행되므로 현재 source상 동기 roundtrip이 아니며 우선 최적화 대상에서 제외한다. 실제 회귀가 의심될 때만 trace로 확인한다.
- `dotnet run`이 수행하는 CMake native build는 developer launch 시간이며 게시된 executable의 runtime TTID와 분리한다.

### 2.7 CLI/developer launch

`doroti.ps1 run`은 현재 항상 `dotnet run --project ... --configuration Release`를 호출한다. 따라서 첫 invocation에는 restore, C#/TypeScript/native build, Android/Apple AOT, deploy가 앱 launch 앞에 붙을 수 있다. 사용자가 체감한 “부트”가 이 전체 구간이라면 runtime 최적화만으로 해결되지 않는다.

## 3. 공통 작업 및 최소 검증 계약

### S0. 구조적 개선 판단

상태: `inProgress` — first-frame DLR cluster에 적용 완료, 나머지 구조 후보는 미착수

다음 중 하나가 source, project graph 또는 runtime 계약에서 확인되면 별도 profiler/baseline 없이 구현한다.

- first-frame closure에서 매 실행 반복되는 JSON parsing, hashing, reflection, DLR binding 또는 불필요한 allocation을 제거한다.
- 첫 화면이 사용하지 않는 theme/catalog/resource/service를 lazy화하거나 별도 package로 옮긴다.
- 이미 build/publish에서 알 수 있는 manifest, hash, generated metadata를 runtime에 다시 계산하지 않는다.
- 동일한 의미를 더 적은 assembly/file/module load와 더 짧은 synchronous chain으로 제공한다.
- developer launch에서 input fingerprint가 같은 build/deploy 단계를 안전하게 재사용한다.

구조 개선은 public 의미, fail-closed 검증, 첫 유효 content, input/IME/accessibility, terminal exactly-once를 보존해야 한다. 복잡한 cache나 adapter를 추가해 비용을 다른 곳으로 옮기는 변경은 채택하지 않는다.

### S1. 최소 validation과 선택적 진단

상태: `inProgress` — first-frame DLR focused/FCR/target smoke 적용 완료, 전체 부트 gate는 미완료

- 각 변경은 owning build/test와 관련 FCR validation을 먼저 통과한다.
- 가능한 대표 target에서 Release launch 1회 이상의 first content, crash/hang/blank, input, resize/scroll, text와 accessibility smoke를 확인한다.
- 실행하지 못한 physical target과 정량 TTID/TTFD는 `notVerified`로 남기되 다른 target의 구조 개선을 막지 않는다.
- exact CPU/allocation/TTID 수치가 필요하거나 회귀가 의심될 때만 기존 `B00`~`B13` marker, EventPipe/Perfetto/signpost/browser performance marker 또는 짧은 profiler-free A/B를 사용한다.
- 반복 sample 수, p50/p95와 confidence interval은 정량 benchmark를 실제로 수행하는 경우에만 정한다. 모든 patch의 기본 gate로 요구하지 않는다.

결과는 기능 `PASS`/`FAIL`, 실행하지 않은 항목 `notVerified`, 구조상 예상되는 성능 개선 `expectedImprovement`로 구분한다. `expectedImprovement`는 정량 성능 PASS를 뜻하지 않는다.

## 4. 실행 순서

### S2. 공용 first-frame closure 축소

상태: `completed` (안전한 MVP 범위) — 항목 1/2/3/4의 text handler/5 완료. OS accessibility 활성 상태를 신뢰성 있게 판별할 계약이 없어 semantics 지연은 적용하지 않았다.

S0의 구조 판단과 S1의 최소 validation을 적용해 다음 순서로 진행한다.

1. **Typed manifest/bootstrap**
   - Runner SDK가 manifest의 resource/plugin descriptor를 generated C# 배열 또는 source-generated JSON context로 만든다.
   - 매 launch reflection 기반 JSON metadata 초기화를 제거하되 resource hash, RID, plugin ABI fail-closed 검증은 유지한다.
   - generated 경로가 단순하고 fail-closed 계약을 유지할 수 있으면 manifest 크기와 무관하게 매 launch 역직렬화를 제거한다. 더 복잡한 runtime cache가 필요하면 범위에서 제외한다.

2. **Theme와 대형 type initializer**
   - RootApp과 first screen이 직접 구성하는 theme/type initializer를 source에서 추적한다. 필요하면 EventPipe/loader trace는 확인용으로만 사용한다.
   - Demo가 active/inactive light/dark `ThemeData`를 모두 동기 생성하지 않도록 호환 API의 lazy factory를 설계한다.
   - `ThemeData.Create`의 immutable default typography/component theme는 안전한 shared template/copy-on-write 여부를 참조 의미와 mutation test로 판단한다.
   - first frame에서 사용하지 않은 animated icon/path table과 Cupertino catalog는 type-local `Lazy<T>` 또는 별도 lazy-load assembly/resource로 이동한다.
   - 전체 9천여 정적 후보를 기계적으로 바꾸지 않고 기본 root에서 도달하거나 Web initial payload 기여가 큰 data만 수정한다.

3. **First-frame `dynamic` 제거 — `completed` (2026-08-28)**
   - layout/build/paint/semantics와 initial route의 source reachability 및 반복성을 기준으로 DLR call site를 고른다.
   - binder 제거가 분명한 공용 경로부터 typed generic/interface/direct 호출로 바꾼다.
   - public 의미와 Flutter reference 동작을 유지하며, 전체 Framework의 일괄 변환은 별도 follow-up으로 둔다.
   - Web trim/AOT warning과 Apple interpreter 필요 범위가 줄었는지 target별로 검증한다.
   - 실행 결과: C1 render-tree core, C2 initial Navigator/Route, C3 Actions를 구현하고 신규 dynamic-dispatch focused validation과 FCR-3~7을 PASS했다. Windows/Web/Android 가능한 Release build/live smoke도 PASS했다.
   - 경계: Web trim dependency와 Apple interpreter 범위 감소, Windows dark/Narrator, Android physical 문자 commit/TalkBack, Apple/Linux runtime, profiler-free 반복 TTID/CPU/allocation은 `notVerified`다.

4. **Lazy platform service**
   - hidden `Entry`/`Editor` native handler는 첫 text client activation 때 만든다. text field가 initial screen에 있으면 TTFD 전에 생성하고 caret/IME를 잃지 않는다.
   - accessibility가 OS에서 활성인 경우 초기 semantics를 지연하지 않는다. 비활성인 경우 lifecycle상 안전하면 first content present 다음 idle/frame으로 옮기고 기능 회귀를 검증한다.
   - clipboard/plugin/native bridge, shader warm-up과 image cache는 first screen이 요구하는 최소 항목만 동기 준비한다.

5. **불필요한 MAUI registration 제거**
   - 제품 source와 generated registration graph가 쓰지 않는 `CommunityToolkit.Maui.Markup` registration/package를 제거한다.
   - Skia handler나 platform lifecycle에 transitive side effect가 있으면 유지하고 이유를 기록한다.

S2 공용 gate:

- FCR-0/FCR-3/FCR-4/FCR-6/FCR-7 Release validation PASS
- light/dark first frame, system theme live switch, initial text field/IME, accessibility-active launch 회귀 0
- 수정이 영향을 준 owning project와 가능한 platform source/build PASS, 실행 환경이 없는 target은 `notVerified`

### S3. Windows 기본/MAUI 분리 최적화

상태: `completed` (W1/W3), W2와 ReadyToRun A/B는 계획대로 후속

#### W1. 기본 HwndExactCpp 빠른 경로

1. native DLL SHA-256을 build/publish manifest에 기록하고 일반 launch에서는 file metadata + PE header + ABI/version만 fail-fast 검증한다. 전체 hash 재계산은 diagnostics/audit 명령에서만 수행한다.
2. `RoInitialize`, Windows App Runtime bootstrap, HWND, `AppWindow`, render worker, ANGLE/Skia context와 first present의 기존 순서를 보존한다. 병목이 남을 때만 `B02→B11` 세부 marker를 추가한다.
3. 기본 ANGLE assembly/package에서 D3D12 diagnostic presenter와 Vortice/D3D12 dependency를 분리한다. `DOROTI_WINDOWS_PRESENTER=D3D12`는 별도 진단 artifact가 있을 때만 명시적으로 허용하고 silent fallback은 두지 않는다.
4. clean publish에서 PDB, diagnostic symbols, 사용하지 않는 optional runtime asset을 배포 파일에서 제외한다. Windows App SDK가 지원하는 self-contained file set만 사용하고 임의 DLL 삭제는 금지한다.
5. ReadyToRun on/off는 구조상 우열이 분명하지 않은 tuning이므로 MVP를 막지 않는 선택적 후속으로 둔다. 수행할 때만 짧은 동일-artifact A/B로 결정한다.
6. first exact present 전 show 금지와 첫 swap 후 `DwmFlush`는 유지한다. 이를 제거한 숫자는 acceptance에 사용하지 않는다.

#### W2. Windows App SDK identity 후속 결정

`AppWindow`가 현재 연결·보관·해제 외의 제품 동작에 쓰이지 않는다는 source 사실과 deployment 요구를 기준으로 다음 둘 중 하나를 operator decision/ADR로 선택한다. 성능 A/B는 필수 선행 조건이 아니다.

- Windows App SDK backend identity를 유지하고 해당 비용을 수용한다.
- raw Win32 + ANGLE host를 새 backend/target package로 분리하고 기존 Windows App SDK 계약과 migration을 문서화한다.

기존 backend 이름만 유지한 채 bootstrap을 몰래 제거하지 않는다. 이 항목은 MVP 후속이다.

#### W3. 명시적 Windows MAUI

- S2 lazy text/semantics/package 정리를 적용한다.
- WinUI bootstrap, MAUI DI/build, DXGI/Skia context와 first present는 독립 경로로 유지한다. 추가 marker는 원인 분리가 필요할 때만 넣는다.
- 기본 HwndExactCpp보다 느리다는 이유로 제거하거나 자동 fallback하지 않는다.

Windows acceptance:

- target-scoped Release publish, empty `PATH` launch와 native provenance audit PASS
- 가능한 process-cold와 warm launch smoke에서 first exact content visible, failed terminal 0
- resize/mixed-DPI/input 기존 gate 유지
- 실제 한글 IME/Narrator는 수행 전까지 `notVerified`

### S4. Android arm64 physical 우선 최적화

상태: `partial` — arm64 physical/profile packaging과 x64 trimmed build 완료, x64 emulator launch 및 marshal-method 재활성화 matrix는 `notVerified`

1. 고정 serial의 arm64 물리 기기에서 Release 설치·실행, screenshot, Activity/PID, crash/ANR, input smoke를 먼저 닫는다. `am start -W`, Perfetto와 managed trace는 정량 보고나 회귀 분석이 필요할 때만 사용한다.
2. app startup Critical User Journey로 Android Baseline Profile과 Startup Profile을 생성·패키징하고 `baseline.prof`, dex layout, device compilation state를 검증한다.
3. arm64 full AOT/default/profiled AOT의 우열은 구조상 확정할 수 없으므로 MVP 구조 개선과 분리한 선택적 tuning으로 둔다. 실제로 선택할 때만 같은 Release APK 조건의 짧은 A/B를 수행한다.
4. x64는 AOT off를 유지한 채 trimming을 다시 켜서 assembly store를 줄이되, Release build와 emulator launch에서 기존 startup fault가 재발하면 즉시 기존 설정으로 복귀하고 `FAIL`을 기록한다.
5. `AndroidEnableMarshalMethods`는 기존 startup fault의 최소 재현, fixed runtime 확인과 arm64/x64 matrix가 먼저다. 재활성화 후보가 실패하면 계속 off로 두고 `FAIL` 근거를 남긴다.
6. hidden text handler, semantics, inactive theme를 lazy화하고 첫 화면에 TextField가 있는 별도 cold-start case로 TTFD/IME를 검증한다.
7. splash 종료 시점과 Doroti first content를 같은 color라서 오인하지 않도록 screenshot/pixel + `B11`을 함께 사용한다.

Android acceptance:

- fixed physical arm64 Release first-install/cold/warm 각 가능한 smoke PASS
- APK 설치, foreground Activity/PID, screenshot, crash/ANR 확인. `gfxinfo`/Perfetto는 선택적 진단
- first input, text selection/overlay, TalkBack-active launch와 scroll cadence 회귀 확인
- emulator x64 PASS는 physical arm64 PASS를 대신하지 않음

### S5. Apple 세 제품 독립 최적화

상태: `partial` — S2 공용 변경과 Windows-hosted managed compile 적용, Apple OS의 signed runtime gate는 `notVerified`

1. generated entry, UIApplication/NSApplication, MAUI builder, native view, Metal context, root/type init과 first-command-buffer ordering을 source에서 정리한다. signpost/startup marker는 원인 분리가 필요할 때만 연결한다.
2. iOS physical, iOS simulator, Mac Catalyst, native AppKit을 별도 artifact와 기능 결과표로 기록한다.
3. S2의 first-frame `dynamic` 제거 뒤 AOT/interpreter 범위를 재평가한다. linker/AOT/registrar 설정은 size, launch, native bridge와 reflection 회귀를 같이 통과할 때만 변경한다.
4. hidden `Entry`/`Editor` handler와 inactive theme를 lazy화한다. 첫 화면 TextField/VoiceOver-active case는 TTFD 전에 준비한다.
5. Metal device/queue/context 조기 prewarm은 비용 이동 여부가 불명확하므로 기본 범위에서 제외한다. 이후 실제 대기가 의심될 때만 별도 실험한다.
6. signed install, dyld/managed assembly mapping, AOT fallback 또는 first drawable 대기가 문제로 남을 때만 Instruments/xctrace로 분해한다.

Apple acceptance:

- iPhone/iPad physical iOS, Mac Catalyst, AppKit Release launch 결과를 각각 기록
- 첫 Metal completion과 visible screenshot 일치, failed/stale terminal 0
- Korean IME/VoiceOver/signing/notarization을 실제 수행하지 않으면 `notVerified`

### S6. Web payload/compile 경로 우선 최적화

상태: `partial` — trimming/symbol/payload와 desktop Chrome live 완료, assembly 분할·실제 배포 HTTP header·fresh profile/mobile은 후속 또는 `notVerified`

1. navigation, loader, boot resource graph, `Blazor.start`, managed main, JS module import, WebGL context, framework attach와 first WebGL commit의 synchronous chain을 source에서 정리한다. `performance.mark/measure`는 병목 분리가 필요할 때만 연결한다.
2. generated Framework graph가 trim-safe해지도록 first screen에 필요한 reflection/dynamic root를 명시하고 product Release의 `PublishTrimmed=true` 금지 gate를 단계적으로 제거한다.
3. trim warning 0만으로 PASS하지 않고 Material gallery, plugin, image/font/shader, semantics, resize, text input browser live gate를 통과한다.
4. initial route에서 필요 없는 Cupertino, animated icon/catalog, diagnostics와 optional BCL assembly를 assembly lazy loading 또는 package 분할로 뒤로 보낸다. root Material/Skia/runtime assembly는 억지 lazy-load하지 않는다.
5. Release publish에서 PDB/source map을 symbol artifact로 분리하고 실제 HTTP Brotli/gzip, immutable fingerprint cache header를 검증한다.
6. 지원 locale을 operator가 확정한 뒤 필요한 ICU shard만 initial load하거나 추가 culture data를 lazy-load한다. 한국어 text/IME/locale fallback이 깨지면 full ICU를 유지한다.
7. service worker/repeat cache는 warm navigation만 개선한 것으로 기록하고 fresh-profile cold TTID를 대체하지 않는다.
8. initial compressed payload는 사용하지 않는 assembly/symbol/ICU/resource를 제거한 만큼 줄인다. 임의의 50% 수치를 구현 gate로 두지 않고 first scene 기능을 완료 기준으로 삼는다.

Web acceptance:

- clean Release publish + 실제 HTTP compression과 fresh browser profile launch smoke
- 가능한 desktop Chrome와 한 개 mid-tier mobile browser/device의 cold/repeat 기능 결과
- first canvas pixel, pointer/key/IME/clipboard/ARIA/resize live PASS
- loader `started`만으로 first-present PASS 선언 금지

### S7. Linux/Qt startup 최적화

상태: `partial` — managed Release build와 Qt ABI contract 완료, 실제 Wayland/X11 runtime은 `notVerified`

1. published executable 기준으로 hostfxr/CoreCLR, managed main, `dlopen` Qt/Skia/native shim, `QApplication`, QPA plugin, `QOpenGLWindow`, GL/Skia context와 first `frameSwapped`의 source ordering을 유지한다.
2. `perf`, `strace`, `LD_DEBUG=statistics`, EventPipe는 실제 Linux startup 문제가 남을 때만 원인 분석용으로 수집한다.
3. framework-dependent IL/ReadyToRun/self-contained ReadyToRun 비교는 구조상 우열이 불명확한 선택적 tuning으로 두고 MVP를 막지 않는다.
4. S2 lazy text/semantics/theme를 적용하고 QAccessible factory/backdrop event가 TTFD에 미치는 영향을 확인한다.
5. native shim은 published artifact에서 재빌드하지 않는다. CMake incremental 개선은 developer launch 항목에서 별도로 처리한다.

Linux acceptance:

- 실제 Wayland와 실제 X11 session을 독립 검증하고 VM/WSLg는 보조 결과로만 유지
- first `frameSwapped`, screenshot, input/IME/accessibility와 terminal invariant 확인
- 물리 Linux/한글 IME/Orca를 실행하지 않으면 `notVerified`

### S8. CLI/developer iteration 개선

상태: `completed` — fingerprint 기반 normal/last-successful Windows 실행과 missing/stale state fail-closed 계약 검증 완료

1. `-NoBuild`/`-NoRestore` 또는 명시적 fast-run option을 먼저 설계한다. runner/configuration/RID/artifact fingerprint가 맞지 않으면 stale binary를 실행하지 않고 재build 필요를 명확히 실패시킨다.
2. workspace resolve, restore, managed compile, TypeScript/native build, AOT/link, deploy, process launch timer는 재사용 실패 원인을 구분해야 할 때만 추가한다.
3. Release 기본값은 acceptance 재현성을 위해 유지한다. 일상 개발에는 명시적 Debug fast path와 “마지막 성공 artifact 재실행” 명령을 문서화한다.
4. Android Gradle AAR, Apple Xcode binding, Linux CMake와 Web TypeScript output은 input hash가 같으면 재사용하도록 dependency/fingerprint 계약을 고친다. 필요하면 MSBuild binlog로 원인을 확인한다.
5. CLI는 실행한 artifact의 configuration/RID/fingerprint와 생략·재실행한 단계를 명확히 출력한다. 상세 시간은 선택적이다.

CLI acceptance:

- clean first run은 필요한 build/deploy를 생략하지 않음
- no-build run은 동일 artifact를 사용하고 runtime TTID가 일반 run과 같음
- stale RID/config/native binding은 fail-closed
- README.md/README.ko.md와 template 명령 동기화

## 5. MVP와 후속 경계

### MVP

1. S0 구조 판단과 S1 최소 validation 계약 적용
2. inactive theme/type initializer, first-frame DLR, hidden text/semantics의 명백한 공용 비용 제거
3. Windows production hash 제거와 D3D12 diagnostic 분리
4. Android Baseline/Startup Profile 패키징, arm64 physical smoke와 x64 trimming 복구
5. Web trimming 해제에 필요한 최소 graph 정리, production symbol/ICU/payload 축소
6. Apple/Linux에 S2 공용 구조 개선 적용과 가능한 기능 검증
7. 안전한 no-build/no-restore 재실행과 artifact fingerprint 검증

### 후속

- Windows App SDK를 raw Win32 target으로 대체/병행하는 architecture 변경
- 전체 Framework의 `dynamic` 제거, 전체 assembly 재분할 또는 NativeAOT 전환
- 모든 Material/Cupertino catalog의 binary/resource 재설계
- app-specific deferred route/data/network loading
- store installer, production telemetry sampling, cloud profile/field metric 운영

## 6. 검증 matrix와 상태 기록

| Gate | 필수 결과 | 현재 상태 |
| --- | --- | --- |
| G0 source/architecture | first-frame reachability, generated metadata, no avoidable pre-present parsing/hash/file I/O | `PASS`: source-generated manifest metadata, lazy active theme, launch-time Windows full hash 제거. 정량 개선폭은 `notVerified` |
| G1 common runtime | FCR-0/3/4/6/7, theme/text/semantics first-frame 회귀 없음 | `partial`: FCR-3/4/6/7와 lazy theme/text activation `PASS`; 현재 checkout에 실행 가능한 FCR-0 aggregate가 없어 FCR-0은 `notVerified`, accessibility-active launch도 `notVerified` |
| G2 Windows default | HwndExactCpp Release cold/warm, exact visible present, input/resize 유지 | `PASS`(자동화 범위): C5-A/C9, empty PATH, normal/audit provenance, failed terminal 0. 물리 IME/Narrator와 정량 TTID는 `notVerified` |
| G3 Windows MAUI | 독립 Release cold/warm 기능 smoke와 common regression | `partial`: Release build `PASS`; 독립 live cold/warm는 `notVerified` |
| G4 Android arm64 | physical first-install/cold/warm smoke, ANR/crash/screenshot/input | `PASS`(수행 범위): `R3CY30KZA4B` Release install, profile install-dm, cold/warm, PID/foreground/screenshot/ASCII IME, crash·ANR 0. 한글 commit/TalkBack은 `notVerified` |
| G5 Android x64 | emulator dev path, AOT-off + trim 후보와 startup fault | `partial`: trimmed Release build/profile strict validation `PASS`, APK 22,990,835 bytes; x64 emulator launch는 `notVerified` |
| G6 iOS physical | signed Release cold/warm, Metal present, IME/VoiceOver | `notVerified` |
| G7 Mac Catalyst | 독립 Release cold/warm, Metal present | `notVerified` |
| G8 AppKit | 독립 Release cold/warm, Metal completion present | `notVerified` |
| G9 Web | trimmed clean publish, payload, fresh/repeat browser first commit | `partial`: trimmed publish와 desktop Chrome canvas/Korean text/action/scroll/ARIA `PASS`; fresh profile/mobile/실제 배포 compression·cache header는 `notVerified` |
| G10 Linux | actual Wayland/X11 published launch와 frameSwapped | `partial`: managed Release build와 Qt ABI `PASS`; 실제 Wayland/X11은 `notVerified` |
| G11 CLI | clean/fast developer launch phase 분리와 stale artifact 거부 | `PASS`: 동일 fingerprint normal/`-LastSuccessful` 실행과 missing state fail-closed 확인 |
| G12 cross-target | package/template/README와 전체 Release regression | `FAIL`: 개별 Windows/Android/Web/Linux 및 Apple managed compile은 진행됐으나 전체 solution은 Windows에 없는 macOS `sips`에서 1 error |

모든 test/build job에는 repository 지침대로 20분 timeout을 적용한다. 한 플랫폼의 PASS나 사용자의 체감 개선으로 다른 플랫폼의 자동 FAIL/`notVerified`를 PASS로 바꾸지 않는다.

## 7. 우선순위와 중단 기준

구현 우선순위는 다음과 같다.

1. source와 runtime 계약상 불필요한 동기 작업을 확실히 제거할 수 있으면 계측·baseline 없이 바로 구현한다.
2. 여러 플랫폼이 공유하는 Framework/Hosting의 theme/type initializer, first-frame DLR, hidden text/semantics를 먼저 고친다.
3. 정적 증거가 큰 Web initial payload, Windows launch-time hashing/diagnostic graph, Android x64 untrimmed store를 플랫폼 우선 작업으로 둔다.
4. 각 변경은 좁은 patch로 구현하고 owning test/FCR/Release smoke에서 correctness를 확인한다.
5. 기능 계약을 깨거나 비용을 reflection/cache/adapter로 옮기는 변경은 되돌리고 `FAIL`을 남긴다. 정량 계측이 없거나 개선폭이 작다는 이유만으로 구조적으로 타당한 변경을 제거하지 않는다.

## 8. 구현과 병행할 operator 결정

아래 결정은 해당 항목의 제품 범위를 바꿀 때만 필요하며, 앞선 공용 구조 개선의 착수 조건이 아니다.

- 기준 장치/OS: Android physical serial과 refresh rate, iPhone/iPad, macOS 기기, Windows CPU/GPU, Linux Wayland/X11, Web mobile device/network profile
- Web locale 범위: 한국어/영어 고정인지 임의 locale의 offline 전환까지 필요한지
- Windows deployment: self-contained Windows App SDK identity를 반드시 유지할지 raw Win32 + ANGLE 신규 target을 허용할지
- public API: `MaterialApp`/`ThemeData` lazy factory를 additive API로 노출할지 내부 shared-default 최적화만 허용할지
- 사용자가 말한 “처음 부트”에서 developer first run과 설치된 앱의 cold start 중 어느 체감을 제품 우선순위로 둘지

## 9. 구현 참고 자료

- [.NET ReadyToRun deployment](https://learn.microsoft.com/dotnet/core/deploying/ready-to-run)
- [.NET for Android build properties](https://learn.microsoft.com/dotnet/android/building-apps/build-properties)
- [Android Baseline Profiles](https://developer.android.com/topic/performance/baselineprofiles/overview)
- [Blazor WebAssembly lazy-loaded assemblies](https://learn.microsoft.com/aspnet/core/blazor/webassembly-lazy-load-assemblies)
- [Blazor WebAssembly host/deploy and Release trimming](https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly/)

> 현재 결론: source와 기존 artifact만으로 Web payload, Windows production hashing/diagnostic dependency, Android x64 untrimmed store, 공용 ThemeData/type initializer/DLR/hidden platform view는 런타임 작업을 분명히 줄일 수 있는 구현 대상으로 확정한다. live startup trace는 이 작업들의 선행 조건이 아니며, 정량 수치나 남은 병목을 설명해야 할 때만 사용한다.

## 10. 진행 이력

### 2026-08-29 — 최초 부트 MVP 구현 실행

- 공용 manifest source generation, active-theme lazy factory, MAUI text native handler on-demand, 미사용 CommunityToolkit Markup 제거를 적용했다.
- Windows launch-time full hash를 audit-only로 옮기고 build/publish provenance, D3D12 diagnostics assembly 분리, Release symbol 제거를 구현했다. C5-A와 C9 normal/audit/negative probe가 PASS했다.
- Android arm64 startup CUJ profile을 실제 장치에서 수집·패키징했고 `install-dm/speed-profile`, cold/warm와 text activation을 확인했다. x64는 AOT off+trim on build와 profile strict validation이 PASS했다.
- Web trimming과 Release symbol 제거를 적용해 initial uncompressed 27,049,342 bytes/Brotli 7,447,738 bytes로 줄였고 desktop Chrome live를 PASS했다.
- CLI normal/last-successful fingerprint 재사용을 구현해 동일 artifact 실행과 missing state fail-closed를 확인했다.
- 상세 결과와 `notVerified`/후속 경계는 `history/26-08-29/cross-platform-first-boot-implementation.md`에 남겼다.

### 2026-08-28 — S2 First-frame `dynamic` 제거 완료

- 당시 작업 계획의 W0~W4를 실행해 C1 render-tree core, C2 Navigator/Route, C3 Actions를 `keep`으로 확정했다. 결과는 `history/26-08-29/cross-platform-first-boot-implementation.md`에 보존했다.
- 신규 focused validation에서 render child ordering/해제, 서로 다른 `Route<T>` result, Actions intent/override/listener exactly-once와 선택 owner `CallSite<T> = 0`을 검증했다.
- FCR-3~7, Widgets/Material, Windows default, Web publish, Android arm64 build를 PASS했다.
- 실제 Windows는 system-light first frame, TextField, pointer action, wheel scroll을 확인했고 Web은 canvas/input/action/scroll 및 console error/warning 0건을 확인했다.
- Android physical `R3CY30KZA4B`는 first frame, PID/focus, pointer action, scroll, crash/ANR 0건을 확인했다.
- 구조적 판정은 `expectedImprovement`다. 반복 TTID/TTFD/CPU/allocation A/B와 실행하지 않은 physical accessibility/Apple/Linux gate는 `notVerified`로 유지한다.

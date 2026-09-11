# PlatformView 작업계획

작성일·재검토일: 2026-09-11 · 재검토 HEAD: `5b8996fbe0fde6cff76b7024fe8046505e108d6e` + 기존 working tree 변경 포함

Linux 계획 추가 재검토: 2026-09-11 · HEAD `b2f7555438e6f865fd1b17c2a7f2880fd363da95`. [ref.md](ref.md)를 반영한 [work2 WV-7](work2.md)의 시스템 Qt WebEngine 직접 adapter 계획에 맞춰 PV-9와 인계 조건을 갱신했다. 이번 변경은 문서에 한정하며 기존 AppKit 증거와 다른 플랫폼 상태를 승격하지 않는다.

기준: [idea.md](idea.md) 및 이번 사용자 요구인 **Doroti 위젯과 플랫폼 뷰의 양방향 전체·부분 겹침**. 공통 기반과 Windows/Web 선행 작업, AppKit NativeOverlay 제품 연결이 일부 진행되어 있다. **현재 전체 상태는 `PARTIAL`이며 사용자 요구를 아직 충족하지 못한다.** 기존 PV 단계는 유지하되 C 교차 합성을 필수 목표로 구체화한다. 최초 재검토는 문서 수정만 수행했으며, 이후 사용자 요청에 따른 macOS 제품 코드·검증 변경은 아래 후속 구현 절에 기록한다.

최초 재검토 시 checkout에는 기존 문서가 참조하던 `Doroti/docs/platform-views/contract.md`, `support-matrix.md`, `Doroti/docs/validation/platform-views/2026-09-11/README.md`가 없었다. 후속 구현에서 계약/지원표를 복구했으며 실행 기록은 AppKit README를 사용한다. 계약의 실제 구현은 [Ui 계약](Doroti/src/Doroti.Ui/PlatformViewContracts.cs)과 [composition plan](Doroti/src/Doroti.Hosting/PlatformCompositionPlan.cs), 확인 가능한 범위·증거는 [AppKit 문서](Doroti/docs/platform-views/appkit.md), [AppKit 실행 기록](Doroti/docs/validation/platform-views/2026-09-11/README.appkit.md), [검증 안내](Doroti/validation/platform-views/README.md)를 기준으로 한다. 공통 계약·지원표 문서 복구와 양방향 겹침 기준 반영은 수행했으며, 기기별 성능 예산은 남아 있다.

## macOS 후속 구현 — 2026-09-11

사용자의 맥 작업 요청에 따라 **native AppKit macOS** 경로를 추가 구현했다. 아래 최초 재검토의 Windows/Web 등 다른 플랫폼 상태는 유지하며, AppKit의 단일 surface/B 전용 설명은 이 후속 결과로 갱신한다.

- `AppKitPlatformRasterSurface`로 중간·전경별 투명 CAMetalLayer를 만들고 Graphite/Ganesh의 실제 GPU surface를 연결했다. 첫 raster는 기존 MTKView를 사용한다.
- native clip, raster, shield가 동일 paint order를 사용한다. AppKit hit-test도 이 순서를 따르며, shield는 기존 Metal view의 입력 경로로 연결한다. 슬롯은 제한 내 재사용하고 detach 시 GPU lease 종료 뒤 해제한다.
- 전체 frame 준비 후 같은 Metal queue에 제출하고 Core Animation transaction에서 drawable들을 표시한다. 실패 시 준비 취소/배치 rollback과 제출된 자원의 GPU retirement를 처리한다. 물리적인 display 동기화와 device-loss 스트레스는 미검증이다.
- Testbed에 같은 key/handle을 유지하는 10개 장면(부분/전체 가림, 해제, 역순, 중간 그림, alpha, shield on/off/역순, 이동)과 전체 영역을 보호하는 modal 예제를 추가했다.
- `macos-interleaved` gate는 두 renderer에서 실제 창 캡처의 색 공간을 sRGB로 변환해 픽셀을 비교하고, native identity·편집 내용·hit target·합성 입력의 단일 탭 전달을 검사한다. [최신 실행 결과](Doroti/docs/validation/platform-views/2026-09-11/README.appkit.md)를 기준으로 한다.
- 누락된 [공통 계약](Doroti/docs/platform-views/contract.md)과 [지원표](Doroti/docs/platform-views/support-matrix.md)를 복구했다.

**PV-8은 여전히 PARTIAL이다.** 제한된 C 제품 경로를 구현했지만 한글 IME, VoiceOver, 완전 가림 시 focus/semantics 정책, 실제 wheel/drag/capture, 제품 두 창 close/reopen, resize/DPR/device-loss 스트레스, 0/1/4-view 성능 및 최종 배포는 완료로 계산하지 않는다. 다른 플랫폼과 Mac Catalyst도 승격하지 않는다. `skippedByUser`로 처리한 항목은 없다.

## 재검토 결론

- **설계 방향은 유지한다.** 기존 `raster → native → raster` planner는 양방향 겹침을 위한 기반이다. registry·identity·수명·typed scene·segment 분할을 버리고 다시 만들 필요는 없다.
- **제품 합성 구현은 추가·수정해야 한다.** 최초 재검토 당시 AppKit의 단일 Metal canvas는 한 방향의 겹침만 가능했다. 후속 구현에서는 segment별 투명 Metal surface로 제한된 C 제품 경로를 연결했다. 전체 수용 기준은 여전히 미완료다.
- **B 성공을 목표 달성으로 보지 않는다.** NativeOverlay는 중간 단계다. 요구를 충족하는 경로는 `InterleavedComposition`이며, 전체/부분 가림·반대 순서·동적 순서 변경·입력 정합성을 실제 제품에서 검증해야 한다. `PointerInterceptor`만 추가해도 시각적 합성이 해결되지는 않는다.
- **Windows/Web도 소스상 제품 연결이 미완성이다.** Windows HWND factory와 Web DOM registry는 존재하지만 각각 제품 runner/managed host에 연결되지 않았다. Windows MAUI는 WindowsAppSdk와 다른 presenter를 사용한다. 두 플랫폼 모두 B 제품 통합부터 마무리해야 하며 C에는 실제 다중 raster surface 합성이 추가로 필요하다. 상세 근거는 2.1~2.3에 기록한다.

## 실행 상태 — 2026-09-11

| 단계 | 구현/검증 상태 | 남은 필수 작업 |
|---|---|---|
| PV-0 | `PARTIAL` — typed 계약/evidence schema 및 계약·지원표 문서 복구, 양방향 겹침 기준 반영 | 플랫폼/기기별 성능 수치는 PV-10에서 확정 |
| PV-1 | `PARTIAL` — typed capability/registry/coordinator, manifest 입력 검증, owner별 legacy messenger/focus handler, create 중 dispose, 공통 PlatformView/HtmlElementView facade 구현; 공통 자동 검증 통과 | SDK manifest 생성기와 각 runner의 factory/coordinator/channel 등록, 모든 기존 controller 전략의 제품 연결 |
| PV-2 | `PARTIAL` — typed scene payload, retained planner, effect 거부, balanced raster segment, 실제 Skia CPU 픽셀, commit/retirement 계약, bounded overlay pool 구현·자동 검증; AppKit B/C scene·다중 surface·paint order 연결 | 다른 backend의 제품 다중 surface 연결, NativeOverlay 제한 검사, transaction 실패/retirement/device-loss 종합 검증 |
| PV-3B | `PARTIAL` — HWND factory·독립 stacking/attachment harness 존재, 기존 실행 기록 보존; 제품 등록 호출 없음 | WindowsAppSdk의 HWND 생성 시점/factory 공급·coordinator/channel 연결, Windows MAUI 별도 adapter, DPI·입력/IME/close 제품 검증 |
| PV-3C | `TODO` — 제품 Vulkan DComp는 여전히 topmost 단일 content root; HWND factory는 C 거부 | HWND/composition-native 별도 C 결정, WindowsAppSdk 다중 visual·C ABI·retirement, MAUI Composition/SwapChainPanel 경로별 구현·검증 |
| PV-4B | `PARTIAL` — DOM registry·불변 batch·stale 거부·HtmlElementView wrapper와 독립 DOM harness 존재; 제품 host/worker 연결 없음 | managed factory 공급·등록·codec·수명/응답 protocol, HtmlElementView 입력 정책, 실제 root 입력/focus 분리와 제품 실행 |
| PV-4C | `TODO` — WebGPU/WebGL은 alpha 설정이 있으나 단일 canvas/presenter 경로 | segment별 endpoint/context 자원과 canvas/native/shield 순서, frame 단위 준비/commit·context-loss 및 두 renderer 실행 |
| PV-5 | `PARTIAL` — PointerInterceptor widget/layer/DOM shield와 안팎 입력 단일 전달 검증 | native gesture arena, wheel/drag/capture 종합 검증, 한글 IME 상호 배제, semantics subtree·screen reader 제품 검증 |
| PV-6 | `TODO` | Android View/SurfaceView B/C host 및 emulator/실기기 검증 |
| PV-7 | `TODO` | UIKit iOS/Catalyst B/C host, binding/ABI 및 각 runner·기기 검증 |
| PV-8 | `PARTIAL` — AppKit B 및 제한된 C 다중 Metal surface·paint order·shield 경로, Graphite/Ganesh 제품 검증 추가 | 전체 C1~C6/PV-5, 한글 IME·VoiceOver·제품 두 창/close-reopen·device-loss·성능·배포 검증 |
| PV-9 | `TODO` | WV-7A 시스템 Qt/ABI 범위, WV-7B 공동 host spike, generic 초기화·attachment와 X11/Wayland B/C 구현·검증 |
| PV-10 | `PARTIAL` — Testbed fixture와 자동 수명 시나리오 추가 | 실제 제품 0/1/4-view 성능, 두 창, route/lifecycle, 최종 runner/template/package 배포 회귀 |

AppKit은 제한된 제품 `InterleavedComposition` 경로를 제공하지만, 어느 backend도 전체 C1~C6/PV-5 완료로 광고하지 않는다. 독립 native/DOM harness 성공은 제품 B/C 통과가 아니다. 위 Windows/Web 성공 기록은 기존 작업 기록을 보존한 것이며 이번 재검토에서 재실행하지 않았다. 해당 날짜별 결과 파일이 현재 checkout에 없으므로 재현·증거 복구 없이 검증 완료 범위를 확대하지 않는다. 미실행 항목은 사용자 생략이 아니므로 `skippedByUser`로 표시하지 않는다. 재개 순서는 **PV-0 요구/문서 정리 → PV-2의 제품 다중 surface 연결 → PV-3C/PV-4C와 PV-5 → PV-6~PV-9 C 확장**이다. AppKit PV-8C는 실제 제품 경로의 선행 검증을 수행했으며, 남은 플랫폼별 게이트는 독립적으로 진행한다.

## 1. 목표와 work2 경계

Doroti 위젯 트리 안에 native control을 생성·배치하고, Doroti 그림과 순서대로 합성하며, 입력·포커스·접근성·수명을 연결하는 공통 기반을 만든다. Windows → Web에서 합성 구조를 먼저 검증하고 Android → UIKit → AppKit → Linux Qt로 확장한다.

### 1.1 필수 동작 — 양방향 전체·부분 겹침

`Stack` 등에서 결정된 **실제 scene paint order**를 플랫폼 합성에서도 지킨다. 아래 화살표는 뒤에서 앞으로의 순서다.

| 장면 | 기대 결과 |
|---|---|
| 플랫폼 뷰 → 불투명 Doroti 위젯 | 겹치는 부분만 가려지고, 위젯이 전체 영역을 덮으면 플랫폼 뷰가 완전히 보이지 않음 |
| 플랫폼 뷰 → 반투명 Doroti 위젯 | 겹친 영역에서 live native 콘텐츠와 Doroti 색이 alpha 합성됨 |
| Doroti 위젯 → 플랫폼 뷰 | 플랫폼 뷰가 위젯의 전체 또는 일부를 덮고, 플랫폼 뷰 영역 밖의 위젯은 유지됨 |
| Doroti 배경 → native A → Doroti 중간 → native B → Doroti 전경 | 같은 화면에서 양방향 관계가 동시에 성립하며 중간 위젯은 A 위·B 아래에 표시됨 |
| 같은 인스턴스의 순서·위치·가림 영역 변경 | 다음 commit에서 새 순서가 반영되고, 가림 해제 시 native 상태가 보존되어 다시 보임 |

합성 모드는 지원 전략이며 앞/뒤 순서를 선택하는 스위치가 아니다. 모든 native를 최상단 또는 최하단으로 옮기는 옵션만으로 완료하지 않는다. 전체 가림은 dispose나 snapshot 교체로 흉내 내지 않는다. 완전히 가려진 native의 표시를 생략하는 최적화는 가능하지만 인스턴스·콘텐츠 상태와 복귀 동작을 보존해야 한다.

시각적 가림과 입력 정책은 별도로 검증한다. 전경의 버튼·메뉴·modal barrier가 입력을 막도록 구성되면 뒤의 native로 전달하지 않고, 노출된 native 영역은 정상 동작해야 한다. `IgnorePointer`처럼 통과를 의도한 위젯은 그 정책을 따른다. alpha가 0이라는 이유만으로 입력 통과를 추정하지 않는다.

### 1.2 work2 경계

| 이 문서가 소유하는 작업 | [work2.md](work2.md)가 소유하는 작업 |
|---|---|
| factory, instance registry, host capability, attach/detach/dispose | WebView controller/widget/settings와 backend 등록 |
| scene payload, raster/native 구간, compositor commit/retirement | 탐색·JS·메시지·profile·cookie·local content |
| native 입력, PointerInterceptor, focus/IME 전환, semantics 연결 | 웹 콘텐츠 안의 입력·탐색 이벤트·권한·process 복구 |
| 플랫폼별 native container 및 generic control 검증 | WebView2/Android WebView/WKWebView/iframe/Qt 연결 |

WebView는 선택형 소비자다. 공통 PlatformView가 WebView 패키지를 참조하지 않는다. 단, Windows composition과 Qt stacking의 실현 가능성을 확인하는 spike에는 최소 WebView native 객체를 사용할 수 있다. 이는 work2의 API 구현 완료로 계산하지 않는다.

제품 목표는 **1.1의 양방향 겹침을 지원하는 교차 합성**이다. B는 C 구현에 필요한 중간 성과이며, 제한형 배치만 가능한 backend는 제한을 공개하되 이번 요구는 미충족으로 남긴다. 특정 view 종류에서 C가 불가능하면 원인과 대안을 기록하고 전체 상태를 `PARTIAL`로 유지한다. ExternalTexture, Snapshot, Android HCPP 상당 기능은 후속 연구 범위이며 B/C 실패를 조용히 대체하지 않는다.

## 2. 현재 구현 재검토 — 유지할 기반과 수정할 부분

| 영역과 실제 파일 | 확인 내용 / 필요한 변화 |
|---|---|
| [widgets/platform_view](Doroti/src/Doroti.Framework.Widgets/platform_view.cs), [services/platform_views](Doroti/src/Doroti.Framework.Services/platform_views.cs) | 기존 Android/Darwin widget/controller를 재사용하되 실제 host 수명·callback routing에 연결한다. |
| [rendering/platform_view](Doroti/src/Doroti.Framework.Rendering/platform_view.cs), [layer](Doroti/src/Doroti.Framework.Rendering/layer.cs) | layout·gesture·PlatformViewLayer에서 불변 placement를 추출한다. |
| [공통 widget](Doroti/src/Doroti.Framework.Widgets/PlatformView.cs), [render leaf](Doroti/src/Doroti.Framework.Rendering/PlatformViewSurface.cs), [Ui 계약](Doroti/src/Doroti.Ui/PlatformViewContracts.cs) | typed native layer와 별도 input shield가 있다. `PlatformViewRequest` 기본값은 `NativeOverlay`다. 양방향 겹침 사용 예제는 `InterleavedComposition`을 명시해야 하며, 일반 위젯 API의 기본 정책도 PV-1에서 정리한다. |
| [scene 계약](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs) | `addPlatformView`는 이미 `ScenePlatformViewPayload`를 HostPayload에 넣는다. 미구현이라는 기존 설명을 정정한다. texture 지원 여부와는 별개다. |
| [planner](Doroti/src/Doroti.Hosting/PlatformCompositionPlan.cs) | retained scene을 펼쳐 `R/N/R/N/R`과 shield를 만들고 native capability를 확인한다. 기반은 유지한다. `NativeOverlay`는 native끼리 겹침과 native가 있는 scene의 shield를 거부하지만, 일반 Doroti raster가 native 위를 덮는지 판정하지 않는다. |
| [SkiaSceneRenderer](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) | `DrawPlatformRasterSegment`와 host scene callback이 있다. 일반 `DrawScene`의 native/shield 명령은 host plan이 필요하다는 오류를 낸다. 분할 자체는 구현되었으나 여러 제품 surface에 배치하는 연결이 남았다. |
| [Capabilities](Doroti/src/Doroti.Ui/Capabilities.cs), [application boundary](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs) | view별 capability registry 및 manifest/plugin 구조가 있다. 선택형 `platform.views`와 factory 등록을 여기에 결합한다. |
| [Windows DComp](Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/vulkan_composition.cpp) | `CreateTargetForHwnd(..., TRUE, ...)`, 단일 root SetContent/SetRoot 경로다. 다중 visual과 alpha foreground를 검증해야 한다. |
| [Web registry](Doroti/src/Doroti.Host.Web/Web/doroti.web.platform-views.ts), [protocol](Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts) | DOM registry·native/shield의 order·불변 batch는 있다. 모듈 주석대로 worker protocol 제품 연결은 아직 없고 raster canvas slot도 batch에 없다. DOM z-index만으로 canvas/native 교차 합성이 완성되지 않는다. |
| [Widgets csproj](Doroti/src/Doroti.Framework.Widgets/Doroti.Framework.Widgets.csproj) | `_html_element_view_web.cs`의 compile 제외는 이미 해소됐다. 제품 factory/protocol 연결과 합성 검증은 별도로 남았다. |
| [Android host](Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs) | SurfaceView가 MAUI semantics/IME overlay 아래에 놓인다. native view와 Doroti foreground의 계층을 별도로 설계한다. |
| [UIKit host](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs) | UIView 계층·입력 adapter 및 C 합성을 연결해야 한다. AppKit 구현으로 대체되지 않는다. |
| [AppKit host](Doroti/src/Doroti.Host.Maui/AppKitPlatformViewHost.cs), [factory](Doroti/src/Doroti.Host.Maui/AppKitPlatformViewFactory.cs) | `GetContainer`는 layer-backed container를 MTKView의 sibling으로 붙인다. 첫 raster는 기존 canvas, 이후 raster는 독립 투명 Metal surface에 재생하고 native/shield와 paint order를 공유한다. host factory는 제한된 B/C를 지원하며, standalone factory는 명시적 opt-in 없이는 B 전용이다. 일반 전경의 입력 차단은 PointerInterceptor로 별도 지정한다. |
| [제품 fixture](DorotiTestbedApp/src/PlatformViewFixture.cs), [기존 검증](Doroti/validation/platform-views/Program.cs) | C용 중간/전경 widget과 modal fixture, planner 순서·CPU segment 픽셀 검증은 있다. AppKit 실행 증거는 별도 B fixture다. 전체 가림·반대 순서·같은 instance의 순서 변경과 최종 native 포함 합성/입력 검증을 추가해야 한다. |
| [Qt host](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/src/doroti_qt_host.cpp), [Qt CMake](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/CMakeLists.txt) | Graphite는 QWindow, 비교 경로는 QOpenGLWindow이며 Qt 최소 6.5다. 일반 QWidget 예제를 그대로 적용할 수 없다. |

[ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md)에 따라 제품 프로젝트에서 직접 구현한다. [ADR-022](Doroti/docs/adr/ADR-022-default-native-platform-bridge.md)에 따라 SDK/COM/native pointer는 backend/runner 경계 안에 두고 Apple runner와 binding의 독립성을 유지한다.

### 2.1 Windows 소스 추적 결과

macOS에서도 Windows 제품 소스와 호출부를 확인했다. 아래는 정적 검토 결과이며 Windows 실행 성공을 뜻하지 않는다.

| 경로/근거 | 확인된 구현과 남은 문제 |
|---|---|
| [WindowsAppSdk runner](Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs) `RunCore`/host-ready | `DorotiApplicationBoundary.Load`에 platform-view factory를 공급하지 않는다. host-ready의 capability 등록에 `ConfigurePlatformViews`/channel adapter가 없고 renderer의 `PlatformScenePainter`도 설정하지 않는다. `RenderAndPrepare`/`RenderAndPresent`는 기존 단일 raster 경로다. |
| [공통 boundary](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs) `Load`/`CreatePlatformViewRegistry` | `Configure(capabilities, messages)`는 platform-view 등록을 대신하지 않는다. manifest에 view를 추가해도 공급 factory가 없으면 load 시 거부된다. Windows runner는 boundary를 native HWND 생성 전에 읽으므로, HWND 확보 뒤 사용할 factory/provider와 registry 초기화 순서를 설계해야 한다. |
| [HWND factory](Doroti/src/Doroti.Host.WindowsAppSdk/WindowsHwndPlatformViewFactory.cs) `QuerySupport`/`CreateAsync`/`ApplyAsync` | lower target 선택 인자와 `WS_CLIPCHILDREN`을 전제로 B만 허용한다. factory 자체가 DComp target을 lower로 바꾸지는 않는다. translation/rect clip을 적용하지만 `PaintOrder`를 쓰지 않고 `SetWindowPos`에 `SWP_NOZORDER`에 해당하는 `0x0004`를 준다. 이 factory를 등록하는 것만으로 앞뒤 순서 변경이나 C가 구현되지 않는다. |
| [Vulkan DComp](Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/vulkan_composition.cpp), [C ABI](Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_vulkan_composition_v1.h) | 제품 `AttachCompositionToWindow`는 `CreateTargetForHwnd(..., TRUE, ...)`와 한 root의 `SetContent`를 사용한다. API의 3개 buffer slot은 한 presentation surface의 교대 buffer이며 배경/중간/전경 3개 layer가 아니다. native placement·paint order·다중 surface batch 계약이 없다. premultiplied-alpha API는 있으므로 alpha 기능 부재와 교차 합성 부재를 혼동하지 않는다. |
| [MAUI framework 등록](Doroti/src/Doroti.Host.Maui/MauiFrameworkHost.cs), [Windows surface](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs), [Composition presenter](Doroti/src/Doroti.Host.Maui/WindowsCompositionSurfacePresenter.cs) | platform-view coordinator 연결은 `#if MACOS` 안에 있다. Windows는 Composition 경로의 단일 `SpriteVisual`을 `SetElementChildVisual`로 붙이거나 별도 `SwapChainPanel` 경로를 사용한다. WindowsAppSdk의 DComp C ABI 수정만으로 MAUI의 native container·scene 분할·입력이 연결되지 않는다. |

판정: WindowsAppSdk와 Windows MAUI 모두 **제품 B 미완료, C 미구현**이다. 현재 HWND factory와 수명 계약은 유지하되 runner 등록·UI dispatcher·native parent 소유권을 먼저 연결한다. 이후 실제 presenter별 C를 구현한다. HWND를 composition visual처럼 다루는 가정이나 topmost/lower 플래그 변경만으로 양방향 겹침 완료를 선언하지 않는다.

### 2.2 Web 소스 추적 결과

| 경로/근거 | 확인된 구현과 남은 문제 |
|---|---|
| [BrowserWasmTarget](Doroti/src/Doroti.Target.Web.browser-wasm/BrowserWasmTarget.cs), [BrowserFrameworkHost](Doroti/src/Doroti.Host.Web/BrowserFrameworkHost.cs) | boundary load에는 JavaScript plugin handler만 공급하고 platform-view factory는 공급하지 않는다. `CreateView`에 `platform.views` coordinator/channel 등록이 없다. JS plugin 등록과 native DOM factory 등록은 별개다. |
| [HtmlElementView wrapper](Doroti/src/Doroti.Framework.Widgets/_html_element_view_web.cs), [공개 widget](Doroti/src/Doroti.Framework.Widgets/platform_view.cs) | wrapper는 `InterleavedComposition`을 요청하고 creation params를 `StandardMessageCodec` bytes로 만든다. DOM registry는 `parameters: unknown`을 받으므로 실제 전달·decode 계약이 필요하다. `translucent`는 거부하지만 `transparent`는 request/placement로 전달하지 않는다. `CreateFromTagName`도 여전히 예외이므로 전체 HtmlElementView API 지원으로 계산하지 않는다. |
| [DOM registry](Doroti/src/Doroti.Host.Web/Web/doroti.web.platform-views.ts) | 안정적인 native container·generation·shield·z-index를 구현했다. 그러나 제품 TS에서 registry를 import/생성하는 호출이 없으며, `NativeBatch`에는 views/shields만 있고 raster canvas slot은 없다. |
| [main](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts), [worker](Doroti/src/Doroti.Host.Web/Web/doroti.raster.worker.ts), [protocol](Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts) | 제품 protocol v4 경로에 platform-view create/dispose/focus/placement 처리와 segment prepare/commit 응답이 없다. registry의 batch version 1은 별도 모듈 버전이다. main/worker의 decode·message dispatch와 managed bridge까지 함께 연결해야 한다. |
| [DOM endpoints](Doroti/src/Doroti.Host.Web/Web/doroti.web.dom.ts), [GPU module](Doroti/src/Doroti.Host.Web/Web/doroti.webgpu.ts), [managed surface](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.cs), [Graphite surface](Doroti/src/Doroti.Host.Web/DorotiWebWorkerSurface.Graphite.cs) | canvas 하나를 transfer하고 worker의 단일 presenter/managed surface로 렌더한다. WebGPU `alphaMode: "premultiplied"`, WebGL `alpha: 1`/`premultipliedAlpha: 1`은 이미 있다. canvas만 여러 개 추가하는 대신 segment별 target·context/texture·generation·retirement를 연결해야 한다. device/recorder 공유 가능 여부와 surface별 자원 소유권은 구분한다. |
| [제품 입력](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts) root pointer handler, [registry 입력](Doroti/src/Doroti.Host.Web/Web/doroti.web.platform-views.ts) | 제품 root는 pointer event에 `preventDefault`, pointer capture, `focusActiveEndpoint`, managed dispatch를 수행한다. 현재 native-origin 필터가 없으므로 DOM control을 그 아래에 그대로 붙이면 native 기본 입력·focus와 충돌할 수 있다. registry의 shield는 bubbling을 막지만 노출 native 이벤트와 제품 root의 구분은 별도 통합 작업이다. |

판정: Web 역시 **제품 B 미완료, C 미구현**이다. DOM registry를 실제 host에 연결하고 native 직접 입력과 Doroti 입력을 구분한 뒤, 두 renderer의 다중 canvas 합성을 구현한다. 현재 HtmlElementView는 C를 요청하므로 B-only 연결만으로 기존 wrapper가 동작한다고 가정하지 않는다. B 검증용 명시적 PlatformView fixture와 C용 HtmlElementView fixture를 구분한다.

### 2.3 기존 Windows/Web 검증 코드의 증명 범위

- [Windows stacking probe](Doroti/validation/platform-views/windows-stacking.cpp)는 독립 D3D11/DComp 창에서 topmost와 lower target의 가림을 비교한다. native click은 `BM_CLICK`이며 제품 Vulkan presenter나 다중 raster surface를 실행하지 않는다.
- [Windows attachment harness](Doroti/validation/platform-views/windows/Program.cs)는 WinForms 창에서 factory 생성·focus·100회 수명을 검사한다. `lowerCompositionTarget=true`를 전달하지만 DComp target 자체를 만들지 않는다. 이 검증은 factory 동작 증거이며 제품 lower target 연결이나 C 증거가 아니다.
- [Web DOM harness](Doroti/validation/platform-views/web-dom.mjs)는 registry만 따로 compile/import하고 일반 root의 버튼·iframe·shield를 검사한다. root handler도 카운터에 불과하므로 제품의 capture/focus/managed dispatch 충돌을 검증하지 않는다. 반복 이동은 버튼의 `left`를 바꾸고 iframe 위치는 고정한다. **현재 테스트만으로 iframe 자체 이동·resize·순서 변경 후 문서 상태 보존까지 검증했다고 해석하지 않는다.**
- 이번 추가 검토는 코드와 호출 관계 확인만 수행했다. Windows build/runtime과 Web browser/renderer 검증은 새로 실행하지 않았으며 모두 해당 환경에서 별도 증거가 필요하다. macOS에서도 독립 Web DOM 검증은 가능하지만 그 결과로 Windows 실행이나 제품 WebGPU/WebGL 합성을 대신할 수 없다.

## 3. 선행 계약

### 3.1 책임과 API 초안

| 소유 프로젝트 | 추가/변경할 계약 초안 |
|---|---|
| `Doroti.Ui` | `PlatformViewHandle`, `PlatformViewRequest/Support`, `IPlatformViewHostCapability`, typed `ScenePlatformViewPayload`, immutable placement/clip/transform |
| `Doroti.Hosting` | factory registry, owner별 instance coordinator, `PlatformCompositionPlan`, frame commit/retirement, 기존 platform_views 채널 adapter |
| `Doroti.Framework.Services` | legacy ID → owner/instance generation mapping, create/dispose/focus callback, 미지원 전략 오류 |
| `Doroti.Framework.Rendering` / `Widgets` | 공통 PlatformView facade, 기존 플랫폼 위젯 연결, bounded layout, input shield placement |
| `Doroti.Skia.Rendering` | retained scene 순회, raster segment 기록·렌더, 효과 판정, overlay pool |
| 각 host | UI dispatcher, native attachment/container, compositor transaction, 입력·IME·접근성 adapter |

계약은 PV-0에서 기존 public API와 조립 가능하도록 확정한다. PlatformView를 쓰지 않는 앱의 필수 capability 목록에 무조건 추가하지 않는다. 등록 누락은 해당 기능 사용 시 기존 `DorotiCapabilityException` 체계로 보고한다.

### 3.2 Identity·수명·실패

- handle은 `OwnerViewId + InstanceId + InstanceGeneration`으로 식별한다. [ViewContracts](Doroti/src/Doroti.Ui/ViewContracts.cs)의 frame/epoch 및 presenter surface generation과 분리한다.
- factory 정의 공유와 live instance 소유권을 구분한다. 동시 다중 attach는 거부하고 window 간 reparent는 지원 조회 후 명시적으로 수행한다.
- 상태 전이: `Requested → Creating → Ready → Attached ↔ Hidden/Detached → Disposing → Disposed`; 생성 실패는 `Failed`로 마감하고 부분 자원을 회수한다.
- create 중 dispose/cancel, 중복 dispose, owner close, 늦은 callback, factory 예외에 terminal 결과를 하나만 만든다. UI thread에서 생성 결과를 검사한 뒤 attach한다.
- 크기 0·unbounded layout·비가시 상태의 정책을 정의한다. layout 미충족은 명시적 오류 또는 표시 보류이며 임의 전체 창 크기로 대체하지 않는다.
- remove 시 입력과 새 scene 참조를 즉시 끊는다. 이미 사용 중인 native attachment/raster 자원은 plan 참조와 GPU/compositor retirement가 끝난 뒤 해제한다.
- 오류에는 owner/instance/frame/backend, 요청 capability, 원인을 포함한다. texture·snapshot·renderer 전환을 조용히 수행하지 않는다.

### 3.3 합성·효과·스레드

1. retained subtree를 포함한 scene에서 transform/clip/opacity/paint order를 수집한다.
2. `Doroti 배경 → native A → Doroti 중간 → native B → Doroti 전경` 순서의 plan을 만들고 **각 raster 구간을 native와 교대로 배치 가능한 surface/visual/canvas에 연결**한다. 여러 segment를 한 최하단 canvas로 다시 합치지 않는다. native가 N개이면 최대 N+1 raster 구간을 기본으로 하며 shield 분할까지 포함한 자원 상한을 검증한다. shadow/filter의 확장 bounds도 고려한다.
3. 지원하지 않는 clip/transform/effect를 제출 전에 판정한다. `SaveLayer`, group opacity, backdrop sampling은 canvas 분할만으로 해결되었다고 간주하지 않는다.
4. native 생성/배치/focus는 platform UI thread, Graphite recorder/context는 기존 render owner가 소유한다. frame별 placement는 typed batch로 전달하며 MethodChannel/JSON 반복 호출을 주 경로로 삼지 않는다.
5. 동일 frame/epoch의 raster와 placement를 commit하고 stale plan을 거부한다. 일부 segment 준비 실패 시 부분 화면을 새 완료 frame으로 표시하지 않는 복구 정책을 정의한다.
6. GPU 완료, native 배치 적용, compositor 표시 관측을 별도 기록한다. OS가 표시를 관측할 수 없다면 `Presented`를 추정해서 채우지 않는다.
7. 중간·전경 raster surface는 투명하게 초기화하고 내용이 없는 픽셀에서 아래 native가 보이도록 alpha를 유지한다. 각 segment에 불투명 앱 배경을 다시 칠하지 않는다. 전경 Doroti 그림 자체의 반투명 합성과 native까지 감싸는 group opacity는 별도 기능으로 판정한다.
8. paint order가 바뀌면 native와 raster뿐 아니라 입력 shield 순서도 함께 갱신한다. 순서만 바뀌는 경우 동일 handle/key를 유지한 native를 재생성하지 않는다. 최상단 투명 surface가 창 전체의 native 입력을 가로채지 않도록 hit-test 영역을 구성한다.

`NativeOverlay`의 제한 검사도 보강한다. 현재처럼 input shield와 native끼리의 겹침만 확인하면 `Stack(PlatformView, Container)`의 일반 전경 그림이 오류 없이 native 아래로 내려갈 수 있다. PV-2에서 후속 raster의 native 영역 침범을 판정하거나, 판정할 수 없는 scene은 보수적으로 거부하는 계약을 마련한다. bounds가 없는 picture/effect를 임의로 비겹침으로 간주하지 않는다. B에서는 잘못된 순서로 성공 처리하지 않고 C 필요 오류를 반환한다.

native view가 0개인 scene은 기존 단일 surface 경로를 유지한다. overlay pool에는 개수·메모리·재사용 상한을 둔다. WebView 콘텐츠 내부의 독립 rendering clock까지 원자 동기화한다고 약속하지 않는다.

### 3.4 지원 수준

| 수준 | 필수 동작 | 완료 선언 제한 |
|---|---|---|
| `NativeOverlay` | 이동/resize/DPR/rect clip/hide-show, native끼리 비겹침, 단일 Doroti surface 위의 native | 한 방향의 제한형 배치. Doroti가 native를 덮는 요구는 미충족이며 일반 Stack 호환을 선언하지 않음 |
| `InterleavedComposition` | 양방향 전체·부분 가림, raster/native/raster 순서, alpha foreground, 중간 그림, 순서 변경, 정합한 입력 | 1.1과 PV 플랫폼 C·PV-5를 통과한 view 종류에만 공개 |
| `ExternalTexture` | producer별 import/fence/release | PV-X 연구 후 별도 지원 |
| `Snapshot` | 정지 이미지와 live 입력 중단 정책 | 명시적 사용자가 선택하는 후속 모드 |

support key는 backend·OS/runtime·view 종류·요청 효과다. rect/rounded/path clip, affine/perspective, opacity, capture inclusion, gesture mediation, accessibility, synchronized placement를 각각 보고한다. rounded clip이나 opacity는 backend 검증 전 기본 지원에 넣지 않는다.

## 4. 작업 단계

현재 단계 상태는 위 실행 상태 표를 따른다. 아래는 재검토한 작업 범위와 완료 기준이다. B는 기본 배치, C는 이번 요구에 필수인 교차 합성 게이트다. **모든 플랫폼 C에는 5절의 C1~C6를 공통 적용한다.** C 실패 시 B 결과를 보존하되 해당 backend의 요구 충족 상태는 `PARTIAL`로 남긴다.

### PV-0 — 범위·지원표·관측 기준 고정

선행: 없음.

- 위 소스와 Flutter external-view embedder/Avalonia attachment의 대응표를 작성하고 legacy controller별 지원·거부 정책을 정한다.
- WindowsAppSdk와 Windows MAUI, Android, iOS, Mac Catalyst, AppKit macOS, Web 두 renderer, Linux X11/Wayland를 별도 target 행으로 만든다.
- 공통 계약·지원표는 `Doroti/artifacts/platform-views/`에 기록하고, `Doroti/validation/platform-views/` 검증 도구의 결과 schema를 정의한다. `Doroti/docs/platform-views/`의 별도 문서 생성·복구는 요구하지 않는다.
- 공통 계약·지원표에서 B와 양방향 겹침 C를 구분한다. `idea.md`의 비겹침만 필요한지에 대한 미결정 질문은 이번 요구로 해소되었다. work2의 겹침 인계 기준도 이번 C 기준을 따른다.
- 테스트 기기/OS/runtime/RID와 0/1/4 native view 기준 측정 시나리오, 성능 예산, overlay 상한을 기록한다. 수치 미결정 항목에는 결정 담당 단계와 종료 조건을 둔다.

완료 기준: 필수 기능·제한 기능·후속 연구 및 모든 target의 검증 책임이 명시되고, public API·오류·수명·evidence schema가 리뷰 가능하다. 구현 성공을 뜻하지 않는다.

### PV-1 — Capability·registry·수명·기존 API 연결

선행: PV-0.

- Ui/Hosting의 typed 계약과 owner별 registry를 구현하고 manifest factory 등록을 연결한다.
- Services의 `flutter/platform_views`/`platform_views_2` 요청을 같은 coordinator로 연결한다. 실제 미지원 Android 전략은 명시적으로 거부한다.
- host→framework `viewFocused` handler의 설치·해제와 두 owner callback routing을 구현한다.
- Widgets/Rendering의 create-ready-size-dispose 순서를 연결한다. Web element factory도 동일 수명 의미를 갖게 한다.
- `PlatformViewRequest`의 현재 B 기본값과 일반 `Stack` 사용 기대를 정리한다. C용 API/예제는 `InterleavedComposition`을 명시하고 미지원 시 오류를 반환한다. 기본값 변경 여부는 기존 B 호출의 호환성을 검토해 결정하되, 묵시적 B fallback으로 요구를 축소하지 않는다.

완료 기준: fake host 기반의 create 실패/취소, 늦은 성공, 두 owner ID 충돌, 중복 dispose, detach/reattach, owner close 검증 통과. factory 미등록·미지원 backend가 예측 가능한 오류를 반환한다. native 표시 증거는 다음 단계에서 확보한다.

### PV-2 — Scene payload·composition planner·raster segment

선행: PV-1.

- SceneBuilder/Layer에 typed payload를 추가하고 retained scene의 owner/generation 검증을 연결한다.
- native 앞/사이/뒤의 raster segment와 input shield를 같은 plan으로 만든다. Skia renderer가 native 명령을 일반 draw로 실행하지 않게 한다.
- render/UI 경계에서 commit token과 retirement 참조를 유지한다. resize/device loss/plan 취소 시 pool과 native instance의 수명을 분리한다.
- effect rejection과 raster capture의 native-content 누락 metadata를 구현한다.
- 이미 구현된 planner/segment replay를 유지하고 각 host의 실제 frame 제출 경로에 다중 surface를 연결한다. `IPlatformCompositionPresenter`/`PlatformCompositionSession` 계약 또는 동등한 host transaction에서 준비·commit·rollback·retirement를 보장한다. 같은 canvas로 전 구간을 재생하는 AppKit B 경로를 C 경로로 재사용하지 않는다.
- 3.3의 NativeOverlay 전경 겹침 검사, 투명 초기화, paint order 변경 및 완전 가림 후 상태 보존을 구현한다.

완료 기준: 0/1/2 native 위치와 retained/clip/transform 조합, C1~C6의 plan·raster 단위 검증, stale epoch·실패 rollback·자원 retirement 검증 통과. B에서 일반 전경 겹침을 조용히 허용하지 않는다. 0-view 제품 scene 회귀가 없다. mock planner/독립 segment 픽셀 통과를 실제 native 포함 합성으로 기록하지 않는다.

### PV-3 — Windows 합성 선행 검증 및 host 구현

선행: PV-2. work2 WV-2의 최소 WebView2 객체 spike와 함께 실행 가능.

- **PV-3B:** native button/편집기용 HWND attachment와 WebView2 composition attachment를 분리한다. 현재 topmost DComp 아래에서 generic child HWND가 실제 보이는지 먼저 확인한다. UI dispatcher, bounds/DPI, focus, close cleanup을 구현한다.
- WindowsAppSdk는 `RunCore`의 boundary load와 host-ready의 HWND 확보 순서에 맞춰 factory 공급을 구성하고, owner별 coordinator·legacy channel·renderer scene callback·close cleanup을 연결한다. manifest만 추가하면 factory 누락으로 실패하는 현재 경로를 함께 수정한다. lower DComp target/parent style의 실제 구성을 검증하고 factory의 bool 인자로 이를 대신하지 않는다.
- **PV-3C:** 단일 content visual을 container root의 background child로 옮기고 native visual과 foreground raster visual을 순서대로 배치한다. 기존 Vulkan/Presentation 경로에서 alpha, resize, present/retirement를 유지한다.
- 일반 HWND factory는 현재 B 전용이다. DComp root를 다중 visual로 바꾸는 것만으로 HWND가 그 사이에 들어간다고 가정하지 않는다. HWND control과 composition-native view 각각의 C 구현 경로를 결정·검증하며, WebView2 composition 성공을 generic HWND C 성공으로 대체하지 않는다.
- C ABI에 composition part/surface identity·paint order·frame token을 연결하고 segment마다 필요한 buffer와 retirement를 관리한다. 기존 3개 교대 buffer를 3개 layer로 전용하지 않는다. DComp device 소유권, segment surface 생성·반납 책임을 문서화한다. COM 소유자는 unmanaged 호출 종료까지 유지한다.
- Windows MAUI는 자체 factory/dispatcher/coordinator/channel 등록과 `WindowsCompositionSurfacePresenter`의 container visual·native attachment 구조를 별도로 연결한다. Composition 경로와 `SwapChainPanel` 경로, WindowsAppSdk의 선택 presenter별로 지원표를 나누고 C 미지원 경로는 명시적으로 거부한다. 한 runner/presenter 성공을 다른 경로의 결과로 전용하지 않는다.

완료 기준: 실제 창에서 B의 표시/입력 및 C의 메뉴/반투명 modal/두 native 사이 그림, DPI 1/1.25/1.5/2, resize/창 닫기 중 callback을 확인한다. 일반 HWND와 composition view의 지원표를 별도로 채운다. C가 실패하면 원인과 수정할 compositor 결정을 남기고 다음 플랫폼에서 같은 설계가 검증된 것으로 전제하지 않는다.

### PV-4 — Web DOM·worker 합성

선행: PV-2 공통 계약. Windows의 구조 검토를 참고하되 Windows 실행 환경이나 C 통과를 Web B/C 코드 작업의 착수 조건으로 삼지 않는다. DOM/canvas 구조는 자체 근거로 검증한다.

- **PV-4B:** 기존 DOM registry를 main의 owner lifecycle에 실제 생성/해제하고 `BrowserWasmTarget`의 factory 공급, `BrowserFrameworkHost`의 coordinator/channel·scene 연결을 구현한다. compile 제외는 이미 해소되어 있으므로 남은 작업은 실행 경로 연결이다.
- managed factory ↔ JS registry의 create/ready/error/cancel/dispose/focus와 `StandardMessageCodec` creation params 전달/해석을 구현한다. frame placement는 typed 불변 batch를 유지한다. close/worker 재생성 중 늦은 응답은 owner/instance generation으로 거부한다.
- `HtmlElementView`의 `transparent` 입력 정책을 실제 DOM hit-test로 전달하거나 미지원으로 거부한다. `translucent` 거부는 유지하며 `CreateFromTagName` 지원 여부도 명시한다. B 전용 fixture에는 `NativeOverlay`를 명시하고 C를 요청하는 기존 wrapper를 묵시적으로 B로 바꾸지 않는다.
- **PV-4C:** raster segment별 canvas/DOM slot을 도입하고 OffscreenCanvas transfer, 재사용, context loss, resize/DPR, atomic 적용 가능 범위를 구현한다.
- DOM batch의 native/shield order를 raster canvas slot까지 확장해 한 paint-order 공간에 배치한다. native를 항상 canvas 위에 올리는 DOM 구조로 C를 처리하지 않는다. canvas 투명 영역의 입력 통과와 필요한 shield 영역을 별도로 적용한다.
- protocol v4 변경 시 loader/main/worker/직렬화 소비자를 함께 갱신한다. out-of-order·이전 protocol·이전 epoch 메시지를 거부한다.
- 단일 canvas/presenter/static managed surface 상태를 segment 자원 모델로 확장한다. segment endpoint transfer/재사용, 현재 texture 획득, WebGL context 선택, 투명 clear, frame 전체 준비 결과와 commit/retirement를 연결한다. 기존 WebGPU/WebGL alpha 설정을 유지하되 alpha 설정만으로 C가 완료되었다고 보지 않는다.
- canvas backing size와 CSS 논리 크기를 분리하고 DOM을 매 frame 재생성하지 않는다. iframe 이동/resize가 reload를 유발하는지도 측정한다.
- 실제 iframe의 위치·크기·z-order를 변경한 뒤 document 상태·입력값·load 횟수를 검증하도록 DOM harness를 보완한다. 이후 제품 main/worker를 실행하는 fixture에서 C1~C6와 context loss 복구를 각각 검증한다.

완료 기준: `worker-direct-webgpu`와 명시적 `worker-direct-webgl` 각각 B/C를 브라우저에서 검증한다. 두 iframe 사이 Doroti 그림, focus, resize, stale packet, element 보존, context loss를 기록한다. renderer 자동 전환으로 실패를 가리지 않는다.

### PV-5 — PointerInterceptor·입력·IME·접근성 공통 통합

선행: PV-1/PV-2, Windows/Web의 해당 B/C. 이후 플랫폼마다 재적용하는 공통 게이트다.

- native 직접 수신과 Doroti 중재형 입력을 구분한다. gesture arena 승패·buffer·cancel 및 부모 scroll 경쟁을 backend별로 정의한다.
- PointerInterceptor의 `intercepting`, `debug`, clip/transform/paint order를 input shield로 내린다. 시각적 전경 surface와 입력 보호를 독립적으로 구현한다.
- Doroti 전경의 보호 영역, 그 밖의 노출 native 영역, 뒤로 이동한 shield 위의 native 영역에 각각 이벤트를 보내 순서 변경 후 수신자를 검증한다. modal은 dialog 본문뿐 아니라 barrier 영역의 native 입력도 차단하고, 완전 가림 시 focus/semantics 정책과 닫은 뒤 복귀를 확인한다.
- Web shield 입력을 기존 ingress에 한 번만 전달한다. 영역 밖 iframe 입력, wheel/drag/capture, 메뉴 종료 시 listener 제거와 입력 복귀를 보장한다.
- Web 제품 root의 pointer/wheel/key·focus 처리에서 native 직접 수신 대상과 Doroti/shield 대상을 구분한다. 노출 DOM control 이벤트에 Doroti용 `preventDefault`·capture·hidden-editor refocus가 적용되지 않게 하고, shield에서 들어온 입력은 올바른 좌표로 한 번만 dispatch한다. 독립 DOM harness의 카운터 통과와 제품 입력 통합 통과를 분리한다.
- native focus ↔ Doroti focus, Tab/Shift+Tab, activation, modal 종료 복귀, native IME와 Doroti hidden editor의 상호 배제를 연결한다.
- native semantics subtree와 framework `platformViewId`를 연결하고 읽기 순서·가림·키보드 탐색·중복 노출을 검증한다.

완료 기준: 보호 안/밖 클릭 카운터가 각각 정확히 한 번 반응하고 intercepting on/off 및 scroll/clip 후 영역이 맞는다. 한글 IME 조합·selection·focus 왕복, 해당 OS screen reader를 실제 제품에서 확인한다. cross-origin iframe에는 동일 gesture 중재가 가능하다고 광고하지 않는다.

공식 [PointerInterceptor Web 소스](https://github.com/flutter/packages/blob/main/packages/pointer_interceptor/pointer_interceptor_web/lib/pointer_interceptor_web.dart)의 보이지 않는 DOM shield와 mousedown focus 보존을 참고한다. 이 기능이 foreground 그림을 생성하는 것은 아니다.

### PV-6 — Android host

선행: PV-2/PV-5 공통 계약.

- **PV-6B:** SurfaceView와 native View용 container, UI-thread attachment, layout/clip/hide/focus를 구현한다.
- **PV-6C:** 별도 Doroti foreground surface와 ordering을 연결한다. SurfaceView z-order와 실제 alpha 동작을 확인하고 frame/placement 오차를 계측한다.
- gesture/IME/semantics, rotation/insets, Activity background/foreground, Surface 재생성, owner 종료를 연결한다. GPU surface 재생성으로 native instance를 무조건 재생성하지 않는다.

완료 기준: x64 emulator와 arm64 실기기 결과를 분리해 native 입력·부모 scroll·빠른 이동·전경 modal·재진입을 검증한다. 내부 SurfaceView를 가진 control의 제한을 명시한다. Android 최소 OS 변경과 HCPP/texture 도입은 PV-X 결정 전 수행하지 않는다.

### PV-7 — iOS / Mac Catalyst UIKit host

선행: PV-2/PV-5 공통 계약.

- **PV-7B:** UIView attachment, native hit-test/gesture, responder/focus/IME와 semantics 연결.
- **PV-7C:** Metal background/native/foreground hierarchy 및 shield 순서를 구현하고 clip/inset/회전/activation 수명을 연결한다.
- UIKit 공통 코드를 공유하되 iOS와 Mac Catalyst runner·binding·ABI·입력 장치 검증은 분리한다.

완료 기준: iOS simulator/실기기와 Mac Catalyst 실행을 별도 행으로 기록한다. touch/selection/한글 IME/VoiceOver와 detach/dispose 반복을 확인한다. NativeAOT 지원 주장은 신규 callback/binding 포함 실제 publish/ILC/native link/install/run 증거가 있어야 한다.

### PV-8 — native AppKit macOS host

선행: PV-2/PV-5 공통 계약.

2026-09-11 macOS 구현: [AppKit 구성·지원 범위](Doroti/docs/platform-views/appkit.md),
[실행 증거](Doroti/docs/validation/platform-views/2026-09-11/README.appkit.md).
NSButton/NSTextField의 B 및 제한된 C를 제품 host에서 제공한다. manifest 선택 등록,
owner별 coordinator/channel, flipped 좌표·rect clip·focus/field editor와 native semantics
placeholder 중복 제거를 유지한다. C는 첫 raster MTKView와 중간·전경별 투명 Metal
surface를 native/shield와 같은 순서로 배치하고 transaction 표시 및 GPU lease 해제를
연결한다. `macos-interleaved`에서 실제 제품의 10개 장면과 입력/픽셀을 검증한다.
한글 IME·VoiceOver·두 창·close/reopen·device-loss·성능·배포 등 미검증 게이트는
[AppKit 문서](Doroti/docs/platform-views/appkit.md)에 남긴다. 다른 플랫폼이나
Mac Catalyst의 상태는 이 작업으로 승격하지 않는다.

- **PV-8B:** NSView attachment, flipped 좌표/scale, responder chain, mouse/wheel/key/IME, accessibility 연결.
- **PV-8C:** 모든 NSView를 한 overlay container에 올리는 B 경로와 별도로, `Metal 배경 → NSView A → 투명 Metal 중간 → NSView B → 투명 Metal 전경`을 실제 sibling/합성 계층으로 구성한다. 현재 renderer별 단일 drawable 제출을 다중 surface 준비·commit·retirement로 확장한다. alpha/clip, shield·hit-test, 순서 변경, live resize, 창 activation을 함께 구현한다.

완료 기준: native AppKit runner에서 B/C와 PV-5를 실행하고 두 창·Tab/Shift+Tab·VoiceOver·close/reopen을 확인한다. Mac Catalyst 성공을 AppKit 성공으로 대체하지 않는다.

### PV-9 — Linux Qt native hosting

선행: 공통 제품 연결은 PV-1/PV-2, 입력 완료는 PV-5. **PV-9A의 최소 native 실험은 WV-7A/B와 함께 조기 진행**하며 공통 입력 전체 완료를 기다리지 않는다. work2는 시스템 Qt WebEngine 직접 adapter를 소유하고, 이 문서는 generic host/합성/입력을 소유한다. QWebView/Qt 6.11 채택 여부는 더 이상 선행 미결정 항목이 아니다.

- **PV-9A — host 구조 결정:** WV-7B의 최소 QWebEngineView와 현재 Graphite Vulkan QWindow를 사용해 QWidget shell/attachment를 실험한다. `raster → live native A → raster → live native B → raster`의 실제 겹침·alpha·hit-test를 X11/Wayland에서 조사한다. Widgets 제약이 확인되면 WebEngine Quick을 사용하는 Qt host 구조의 비용을 함께 비교하되 하나의 제품 경로로 결정한다. 결과를 view 종류·renderer·QPA별 B/C 결정 기록으로 남긴다.
- **PV-9B — generic attachment·초기화:** 선택한 QWindow/QWidget/Quick 계층에서 parent/geometry/DPR/rect clip/focus/hide/owner 종료를 연결한다. 기존 QApplication/event loop를 공유하고 plugin opt-in 준비를 application 생성 전에 실행할 generic hook을 제공한다. WebEngine scheme 등록/engine-specific 초기화는 WV-7C shim이 수행한다. UI-thread dispatch, 두 owner와 늦은 callback 거부, callback 종료 전 자원·module 해제 방지를 공통 계약에 맞춘다.
- **PV-9C — 제품 교차 합성:** PV-9A에서 정한 host 계층에 raster segment별 background/intermediate/foreground surface, native attachment, shield를 같은 paint order로 연결한다. frame/epoch별 준비·commit·실패 rollback·GPU retirement, resize/DPR/clip 및 동적 순서 변경을 구현한다. C1~C6와 PV-5는 실제 제품 native 포함 화면/입력으로 검증한다. 별도 popup 창이나 snapshot/hide 대체로 일반 C를 선언하지 않는다.
- QWidget `createWindowContainer()`의 embedded window는 widget 위의 opaque box로 쌓이고 여러 겹친 container의 순서는 정의되지 않는다. 단순 reparent/raise/lower 또는 비겹침 B 성공을 C 설계 근거로 삼지 않는다. [Qt container 공식 제약](https://doc.qt.io/qt-6/qwidget.html#createWindowContainer). C가 불가능한 조합은 구체적 원인·제한형 B와 미충족 요구를 남긴다.
- **의존성 경계:** generic `libdoroti_qt_host.so`에 WebEngine/WebChannel/Quick 필수 링크를 추가하지 않는다. Quick host가 필요하면 선택 구성으로 분리한다. 현재 generic Qt 하한 6.5와 WebView 선택 앱의 계획 API 하한 6.8을 구분하고, 같은 process의 host/shim은 WV-7A에서 검증한 시스템 Qt 조합을 사용한다. 시스템 WebEngine 패키징·진단은 WV-7E가 소유한다.
- **실제 변경 접점:** [managed Qt host](Doroti/src/Doroti.Host.Qt/DorotiQtRunner.cs), [QtNativeV2](Doroti/src/Doroti.Host.Qt/QtNativeV2.cs), [Testbed native host](DorotiTestbedApp/linux/native/src/doroti_qt_host.cpp), [template native host](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/src/doroti_qt_host.cpp), 각 CMake/header와 [runner targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets)를 동기화한다. 현재 v2 이름/export의 실제 ABI 값은 3이므로 version/struct size/feature negotiation 및 validator를 함께 갱신한다.

완료 기준: generic control과 WV-7F의 실제 WebEngine 각각 X11/Wayland B/C·입력·한글 IME·Orca·live resize·창 종료 증거가 있다. native X11/XWayland/WSLg/VM/물리 Linux를 구분하고 C 미해결 또는 미실행이면 Linux 목표는 `PARTIAL`이다. WV-7B와 PV-9A는 공동 spike여서 순환 선행 조건이 아니며, PV-9B 후 WebView 기능 개발을 진행할 수 있다.

### PV-10 — 제품 통합·성능·배포 회귀

선행: PV-1~PV-9의 구현/제한 판정 완료. 각 target의 미실행 게이트는 계속 `notVerified`다.

- 신규 `DorotiTestbedApp/src/PlatformViewFixture.cs`에 native button/편집기, 2개 중첩 native, 메뉴/modal, 두 owner 창, lifecycle 스트레스를 구성한다. WebView 시나리오는 work2 fixture가 소유한다.
- 기존 C fixture에 C1~C6를 선택·반복할 수 있는 제어와 앞/뒤 입력 카운터를 추가한다. 같은 key/handle의 순서 변경, 전체 가림 후 native 편집 내용 보존을 검증하고 B fixture 실행과 결과를 분리한다.
- 0/1/4 native view에서 동일 기기·장면·길이로 frame p50/p95/p99, 입력 지연, 배치 오차, overlay 수, GPU/native memory를 비교한다.
- route push/pop 및 create/dispose 100회 후 live instance/listener/attachment가 기준으로 복귀하는지 확인한다. OS process cache와 실제 instance 누수를 구분한다.
- 선택 capability 없는 앱, 기존 single-surface scene, renderer device loss, final runner/template/package를 회귀 검증한다.

완료 기준: 지원표의 광고된 모든 필수 기능에 제품 실행 증거가 있고 성능 예산을 충족한다. 실패·환경 미확보·제한형 지원을 숨기지 않으며, 일부 target만 통과하면 전체 플랫폼 완료는 `PARTIAL`이다.

### PV-X — 선택 후속 연구

필수 범위 밖: Android SurfaceControl transaction/HCPP 원리 적용, producer별 external texture import/fence, snapshot 전환, overlay pooling 고급 최적화, perspective/backdrop 효과. 측정 가능한 요구와 별도 계약이 생긴 뒤 착수하며 기존 지원 수준을 조용히 변경하지 않는다.

## 5. 공통 검증과 증거

| 검증 영역 | 최소 시나리오 | 합격 판단 |
|---|---|---|
| Identity/수명 | 2 owner, create 중 dispose, late callback, surface 재생성 | stale 접근/중복 terminal 없음, native 수명 보존, 종료 후 소유 자원 회수 |
| 합성 | 아래 C1~C6 및 scroll/resize/DPR/clip | 실제 native 포함 화면의 순서/alpha/배치/입력 일치, 미지원 효과의 명시적 거부 |
| 입력 | tap/drag/wheel/capture/부모 scroll, shield on/off | 이중 전달 없음, 보호 영역 외 native 정상 동작 |
| Focus/IME/접근성 | 한글 조합/selection/Tab, 가려진 view, screen reader | focus 주체 하나, native subtree 중복 없음, 가림 정책 일치 |
| 성능 | 0/1/4 view, 동일 workload, 반복 수명 | PV-0에서 정한 기기별 예산 충족 및 측정값/변동 기록 |
| Capture | raster readback와 실제 창 캡처 비교 | native content 포함 여부를 결과에 정확히 명시 |

각 플랫폼 C의 필수 수용 사례:

| ID | 장면/조작 | 합격 판단 |
|---|---|---|
| C1 | 불투명 Doroti widget으로 native 일부 → 전체 → 가림 해제 | 겹친 영역만 정확히 가림, 전체 가림에서 native 픽셀 노출 없음, 해제 후 동일 instance와 편집 상태 복귀 |
| C2 | 같은 영역의 Doroti widget을 먼저 그리고 native를 뒤에 배치; 부분/전체 겹침 | native가 widget 위에 표시되며 노출 native 영역의 입력이 native에 한 번 전달됨 |
| C3 | native A → Doroti 중간 → native B → Doroti 전경을 실제 겹치는 좌표에 배치 | 중간 그림은 A 위·B 아래, 전경 그림은 A/B 위에 표시됨 |
| C4 | live native 위 반투명 색/메뉴/modal 및 비어 있는 foreground 영역 | 아래 콘텐츠와 alpha 합성, 빈 foreground에서 native가 그대로 보임; native를 감싸는 미지원 group effect는 별도 오류 |
| C5 | 동일 key/handle을 유지하며 앞뒤 순서 변경, 이동/scroll/resize/DPR/clip 변경 | 새 frame에 시각·shield·입력 좌표가 함께 갱신됨, native 재생성·상태 손실·stale 배치 없음 |
| C6 | 전경 버튼/보호 영역·native 노출 영역·modal barrier 클릭, shield on/off와 순서 변경 | 의도한 hit-test 대상에 한 번 전달, 보호 뒤 native 오작동 없음, 영역 밖 입력 정상, modal 종료 후 focus/입력 복귀 |

plan 순서와 CPU segment 픽셀 검증은 공통 자동 검증으로 유지한다. **C 합격 증거는 실제 제품 compositor의 native 포함 화면 캡처/영상과 입력 기록**이어야 한다. 현재 AppKit B 스크린샷이나 `R/N/R/N/R` 단위 검증만으로 C1~C6를 통과 처리하지 않는다. rounded/path clip·perspective·native group opacity 등의 미지원 효과까지 일괄 지원한다고 해석하지 않는다.

결과는 `Doroti/artifacts/platform-views/<date>/<target>/`에 commit, dirty 변경 식별, OS/RID/기기/runtime/renderer, 명령·exit code·timeout, capability 요청/결과, frame/epoch trace, screenshot/video, 실패 원인과 재개 지점을 남긴다. AppKit 구성·지원 범위와 미검증 게이트를 포함한 후속 문서·실행 기록도 `Doroti/artifacts/platform-views/` 안에 작성하며, `Doroti/docs/platform-views/`와 `Doroti/docs/validation/`에 별도 문서를 생성·복구할 필요는 없다. 위의 기존 문서 링크는 과거 구현·실행 기록 참조다. `sourceReviewed`, `build`, `automated`, `productLive`, `physical`, `nativeAot`를 독립 필드로 기록한다. `skippedByUser`는 실제 사용자 생략 요청이 있을 때만 사용한다.

모든 테스트는 [.github 지침](.github/copilot-instructions.md)에 따라 외부 **1200초 timeout**으로 실행한다. 저장소 루트에서의 기본 build 검증 예시는 다음과 같다. 해당 단계의 focused validator와 target build/run도 같은 wrapper를 사용한다. 새 validator의 정확한 명령은 그 validator를 추가하는 단계에서 고정한다.

```powershell
python Doroti/validation/run-with-timeout.py pwsh -NoProfile -File Doroti/eng/doroti.ps1 build
```

최초 문서 재검토는 제품 build/runtime 테스트를 실행하지 않았고, 후속 macOS 구현에서는 build/runtime gate를 실행했다. 실제 명령·결과는 [검증 README](Doroti/validation/platform-views/README.md)와 현재 존재하는 날짜별 evidence에서 구분한다.

## 6. work2 인계 및 완료 규칙

| work2 단계 | 필요한 PlatformView 인계 |
|---|---|
| WV-0 / WV-1 | PV-0 계약 초안 / PV-1·PV-2 공통 기반 |
| WV-2 Windows | PV-3B로 기본 표시, PV-3C·PV-5로 겹침 제품 승인 |
| WV-3 Web | PV-4B로 iframe 표시, PV-4C·PV-5로 겹침 제품 승인 |
| WV-4 Android | PV-6B, 이후 PV-6C·PV-5 |
| WV-5 UIKit | PV-7B, 이후 PV-7C·PV-5, iOS/Catalyst 별도 |
| WV-6 AppKit | PV-8B, 이후 PV-8C·PV-5 |
| WV-7A/B Qt 조사·host spike | WV-7A 버전/ABI 범위와 PV-9A/WV-7B 공동 실험; 표시·합성 구조 및 제한 결정 |
| WV-7C/D Qt adapter·기능 | PV-9B의 generic pre-application hook/attachment/수명; WebEngine API·scheme/profile은 work2 소유 |
| WV-7E/F Qt 배포·제품 승인 | generic host 의존성 분리 회귀, PV-9C·PV-5의 실제 WebEngine C1~C6/입력 증거 |

기본 B 통과 후 work2의 탐색·JS 작업을 진행할 수 있다. **이번 요구의 완료는 양방향 전체·부분 겹침 C1~C6와 PV-5를 통과한 조합에 한정한다.** C 미지원 target/view 종류는 제한형 B로 공개할 수 있지만 요구를 충족한 것으로 계산하지 않는다. 플랫폼 공통 지원 선언에는 모든 대상의 실제 증거가 필요하며 문서 체크박스만으로 완료하지 않는다. Linux 추가 재검토에서는 work2 WV-7A~F와 이 문서의 PV-9·인계표를 함께 수정했다.

주요 로컬 레퍼런스: [Flutter PlatformViewLayer](reference/flutter-master/engine/src/flutter/flow/layers/platform_view_layer.cc), [external embedder 계약](reference/flutter-master/engine/src/flutter/flow/embedded_views.h), [Avalonia attachment](reference/Avalonia-main/src/Avalonia.Controls/Platform/INativeControlHostImpl.cs), [Web embedder](reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/platform_views/embedder.dart). Flutter 파일의 이식 provenance와 참조 폴더 전체 버전은 구분한다.

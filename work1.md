# PlatformView 작업계획

작성일: 2026-09-11 · 검토 HEAD: `cf83abd603aa06b942051cabdeb5132f4dd774c7`

기준: [idea.md](idea.md). 2026-09-11의 전체 구현 요청에 따라 공통 기반과 Windows/Web 선행 구현·검증을 진행했다. **현재 전체 상태는 `PARTIAL`이며 전체 작업 완료가 아니다.** 아래 원래 단계·완료 기준은 유지한다. 구현된 계약은 [contract.md](Doroti/docs/platform-views/contract.md), 제품 지원 여부는 [support-matrix.md](Doroti/docs/platform-views/support-matrix.md), 실행 증거는 [2026-09-11 결과](Doroti/docs/validation/platform-views/2026-09-11/README.md)를 따른다.

## 실행 상태 — 2026-09-11

| 단계 | 구현/검증 상태 | 남은 필수 작업 |
|---|---|---|
| PV-0 | `DONE` — 계약, 지원표, evidence schema, 예산/검증 책임 기록 | 플랫폼/기기별 수치 확정은 지정된 PV-10에서 수행 |
| PV-1 | `PARTIAL` — typed capability/registry/coordinator, manifest 입력 검증, owner별 legacy messenger/focus handler, create 중 dispose, 공통 PlatformView/HtmlElementView facade 구현; 공통 자동 검증 통과 | SDK manifest 생성기와 각 runner의 factory/coordinator/channel 등록, 모든 기존 controller 전략의 제품 연결 |
| PV-2 | `PARTIAL` — typed scene payload, retained planner, effect 거부, balanced raster segment, 실제 Skia CPU 픽셀, commit/retirement 계약, bounded overlay pool 구현·자동 검증 | 제품 renderer의 frame 제출 경로와 실제 GPU/compositor transaction·retirement·device loss 연결 |
| PV-3B | `PARTIAL` — 실제 HWND/DComp stacking 선행 실험, 제품 HWND factory를 독립 UI harness에서 생성/배치/포커스/100회 수명 검증 | WindowsAppSdk와 MAUI runner 각각 연결, DPI 전체 행·실제 입력/IME/close callback 제품 검증 |
| PV-3C | `TODO` — topmost HWND 가림 재현 및 lower target + WS_CLIPCHILDREN 결정 기록 | composition-native visual, background/native/foreground surface, C ABI/Presentation retirement 구현 및 실제 창 검증 |
| PV-4B | `PARTIAL` — main DOM registry, immutable batch, stale packet 거부, HtmlElementView compile 제외 해소; Chromium DPR 1/1.25/1.5/2와 iframe 보존/수명 검증 | managed factory ↔ main/worker protocol 실제 연결과 두 제품 renderer 실행 |
| PV-4C | `TODO` | OffscreenCanvas segment transfer/재사용/commit/context-loss와 실제 두 renderer의 교차 합성 |
| PV-5 | `PARTIAL` — PointerInterceptor widget/layer/DOM shield와 안팎 입력 단일 전달 검증 | native gesture arena, wheel/drag/capture 종합 검증, 한글 IME 상호 배제, semantics subtree·screen reader 제품 검증 |
| PV-6 | `TODO` | Android View/SurfaceView B/C host 및 emulator/실기기 검증 |
| PV-7 | `TODO` | UIKit iOS/Catalyst B/C host, binding/ABI 및 각 runner·기기 검증 |
| PV-8 | `TODO` | native AppKit B/C host 및 macOS 입력/VoiceOver 검증 |
| PV-9 | `TODO` | WV-7A Qt 결정, QWindow X11/Wayland B/C 구현·검증 |
| PV-10 | `PARTIAL` — Testbed fixture와 자동 수명 시나리오 추가 | 실제 제품 0/1/4-view 성능, 두 창, route/lifecycle, 최종 runner/template/package 배포 회귀 |

현재 어느 backend도 제품 `InterleavedComposition` 완료로 광고하지 않는다. 독립 native/DOM harness 성공은 제품 B/C 통과가 아니다. 미실행 항목은 사용자 생략이 아니므로 `skippedByUser`로 표시하지 않는다. 남은 구현의 첫 재개 지점은 **PV-2의 실제 frame/compositor 연결 → PV-3C/PV-4C → PV-5 → PV-6~PV-9**다.

## 1. 목표와 work2 경계

Doroti 위젯 트리 안에 native control을 생성·배치하고, Doroti 그림과 순서대로 합성하며, 입력·포커스·접근성·수명을 연결하는 공통 기반을 만든다. Windows → Web에서 합성 구조를 먼저 검증하고 Android → UIKit → AppKit → Linux Qt로 확장한다.

| 이 문서가 소유하는 작업 | [work2.md](work2.md)가 소유하는 작업 |
|---|---|
| factory, instance registry, host capability, attach/detach/dispose | WebView controller/widget/settings와 backend 등록 |
| scene payload, raster/native 구간, compositor commit/retirement | 탐색·JS·메시지·profile·cookie·local content |
| native 입력, PointerInterceptor, focus/IME 전환, semantics 연결 | 웹 콘텐츠 안의 입력·탐색 이벤트·권한·process 복구 |
| 플랫폼별 native container 및 generic control 검증 | WebView2/Android WebView/WKWebView/iframe/Qt 연결 |

WebView는 선택형 소비자다. 공통 PlatformView가 WebView 패키지를 참조하지 않는다. 단, Windows composition과 Qt stacking의 실현 가능성을 확인하는 spike에는 최소 WebView native 객체를 사용할 수 있다. 이는 work2의 API 구현 완료로 계산하지 않는다.

초기 제품 목표는 **기본 배치 + 지원 backend에서의 교차 합성**이다. 제한형 배치만 통과한 backend는 그 수준만 공개한다. ExternalTexture, Snapshot, Android HCPP 상당 기능은 후속 연구 범위이며 필수 단계 완료에 포함하지 않는다.

## 2. 현재 소스에서 확인한 출발점

| 영역과 실제 파일 | 확인 내용 / 필요한 변화 |
|---|---|
| [widgets/platform_view](Doroti/src/Doroti.Framework.Widgets/platform_view.cs), [services/platform_views](Doroti/src/Doroti.Framework.Services/platform_views.cs) | 기존 Android/Darwin widget/controller를 재사용하되 실제 host 수명·callback routing에 연결한다. |
| [rendering/platform_view](Doroti/src/Doroti.Framework.Rendering/platform_view.cs), [layer](Doroti/src/Doroti.Framework.Rendering/layer.cs) | layout·gesture·PlatformViewLayer에서 불변 placement를 추출한다. |
| [scene 계약](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs) | `addPlatformView`/`addTexture`에 typed HostPayload가 없다. PlatformView payload부터 구현한다. texture 지원과 혼동하지 않는다. |
| [SkiaSceneRenderer](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) | DrawScene에 PlatformView 처리 분기가 없고 미지원 명령 예외로 끝난다. 합성 planner와 raster segment 실행이 필요하다. |
| [Capabilities](Doroti/src/Doroti.Ui/Capabilities.cs), [application boundary](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs) | view별 capability registry 및 manifest/plugin 구조가 있다. 선택형 `platform.views`와 factory 등록을 여기에 결합한다. |
| [Windows DComp](Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/vulkan_composition.cpp) | `CreateTargetForHwnd(..., TRUE, ...)`, 단일 root SetContent/SetRoot 경로다. 다중 visual과 alpha foreground를 검증해야 한다. |
| [Web DOM](Doroti/src/Doroti.Host.Web/Web/doroti.web.dom.ts), [protocol](Doroti/src/Doroti.Host.Web/Web/doroti.web.protocol.ts) | canvas/textarea/semantics 구성, protocol v4. DOM registry와 main/worker 합성 계획 교환이 없다. |
| [Widgets csproj](Doroti/src/Doroti.Framework.Widgets/Doroti.Framework.Widgets.csproj) | `_html_element_view_web.cs`가 compile 제외다. 단순 포함으로 지원을 선언하지 않는다. |
| [Android host](Doroti/src/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs) | SurfaceView가 MAUI semantics/IME overlay 아래에 놓인다. native view와 Doroti foreground의 계층을 별도로 설계한다. |
| [UIKit host](Doroti/src/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs), [AppKit host](Doroti/src/Doroti.Host.Maui/DorotiMacOSMetalView.cs) | UIView와 NSView 계층·입력 adapter를 각각 연결한다. |
| [Qt host](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/src/doroti_qt_host.cpp), [Qt CMake](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/CMakeLists.txt) | Graphite는 QWindow, 비교 경로는 QOpenGLWindow이며 Qt 최소 6.5다. 일반 QWidget 예제를 그대로 적용할 수 없다. |

[ADR-019](Doroti/docs/adr/ADR-019-product-framework-source-ownership.md)에 따라 제품 프로젝트에서 직접 구현한다. [ADR-022](Doroti/docs/adr/ADR-022-default-native-platform-bridge.md)에 따라 SDK/COM/native pointer는 backend/runner 경계 안에 두고 Apple runner와 binding의 독립성을 유지한다.

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
2. `Doroti 배경 → native A → Doroti 중간 → native B → Doroti 전경` 순서의 plan을 만든다. shadow/filter의 확장 bounds도 고려한다.
3. 지원하지 않는 clip/transform/effect를 제출 전에 판정한다. `SaveLayer`, group opacity, backdrop sampling은 canvas 분할만으로 해결되었다고 간주하지 않는다.
4. native 생성/배치/focus는 platform UI thread, Graphite recorder/context는 기존 render owner가 소유한다. frame별 placement는 typed batch로 전달하며 MethodChannel/JSON 반복 호출을 주 경로로 삼지 않는다.
5. 동일 frame/epoch의 raster와 placement를 commit하고 stale plan을 거부한다. 일부 segment 준비 실패 시 부분 화면을 새 완료 frame으로 표시하지 않는 복구 정책을 정의한다.
6. GPU 완료, native 배치 적용, compositor 표시 관측을 별도 기록한다. OS가 표시를 관측할 수 없다면 `Presented`를 추정해서 채우지 않는다.

native view가 0개인 scene은 기존 단일 surface 경로를 유지한다. overlay pool에는 개수·메모리·재사용 상한을 둔다. WebView 콘텐츠 내부의 독립 rendering clock까지 원자 동기화한다고 약속하지 않는다.

### 3.4 지원 수준

| 수준 | 필수 동작 | 완료 선언 제한 |
|---|---|---|
| `NativeOverlay` | 이동/resize/DPR/rect clip/hide-show, 비겹침 native 영역 | Doroti 전경 UI와 일반 Stack 호환을 선언하지 않음 |
| `InterleavedComposition` | raster/native/raster 순서, 투명 foreground, 중간 그림, 정합한 입력 | PV 플랫폼 C 게이트와 PV-5를 통과한 view 종류에만 공개 |
| `ExternalTexture` | producer별 import/fence/release | PV-X 연구 후 별도 지원 |
| `Snapshot` | 정지 이미지와 live 입력 중단 정책 | 명시적 사용자가 선택하는 후속 모드 |

support key는 backend·OS/runtime·view 종류·요청 효과다. rect/rounded/path clip, affine/perspective, opacity, capture inclusion, gesture mediation, accessibility, synchronized placement를 각각 보고한다. rounded clip이나 opacity는 backend 검증 전 기본 지원에 넣지 않는다.

## 4. 작업 단계

현재 단계 상태는 위 실행 상태 표를 따른다. 아래는 원래 작업 범위와 완료 기준이다. B는 기본 배치, C는 교차 합성 게이트다. C 실패 시 B 결과를 보존하되 해당 backend의 교차 합성은 `PARTIAL`로 남긴다.

### PV-0 — 범위·지원표·관측 기준 고정

선행: 없음.

- 위 소스와 Flutter external-view embedder/Avalonia attachment의 대응표를 작성하고 legacy controller별 지원·거부 정책을 정한다.
- WindowsAppSdk와 Windows MAUI, Android, iOS, Mac Catalyst, AppKit macOS, Web 두 renderer, Linux X11/Wayland를 별도 target 행으로 만든다.
- 신규 `Doroti/docs/platform-views/contract.md`, `support-matrix.md`, `Doroti/validation/platform-views/`의 결과 schema를 정의한다.
- 테스트 기기/OS/runtime/RID와 0/1/4 native view 기준 측정 시나리오, 성능 예산, overlay 상한을 기록한다. 수치 미결정 항목에는 결정 담당 단계와 종료 조건을 둔다.

완료 기준: 필수 기능·제한 기능·후속 연구 및 모든 target의 검증 책임이 명시되고, public API·오류·수명·evidence schema가 리뷰 가능하다. 구현 성공을 뜻하지 않는다.

### PV-1 — Capability·registry·수명·기존 API 연결

선행: PV-0.

- Ui/Hosting의 typed 계약과 owner별 registry를 구현하고 manifest factory 등록을 연결한다.
- Services의 `flutter/platform_views`/`platform_views_2` 요청을 같은 coordinator로 연결한다. 실제 미지원 Android 전략은 명시적으로 거부한다.
- host→framework `viewFocused` handler의 설치·해제와 두 owner callback routing을 구현한다.
- Widgets/Rendering의 create-ready-size-dispose 순서를 연결한다. Web element factory도 동일 수명 의미를 갖게 한다.

완료 기준: fake host 기반의 create 실패/취소, 늦은 성공, 두 owner ID 충돌, 중복 dispose, detach/reattach, owner close 검증 통과. factory 미등록·미지원 backend가 예측 가능한 오류를 반환한다. native 표시 증거는 다음 단계에서 확보한다.

### PV-2 — Scene payload·composition planner·raster segment

선행: PV-1.

- SceneBuilder/Layer에 typed payload를 추가하고 retained scene의 owner/generation 검증을 연결한다.
- native 앞/사이/뒤의 raster segment와 input shield를 같은 plan으로 만든다. Skia renderer가 native 명령을 일반 draw로 실행하지 않게 한다.
- render/UI 경계에서 commit token과 retirement 참조를 유지한다. resize/device loss/plan 취소 시 pool과 native instance의 수명을 분리한다.
- effect rejection과 raster capture의 native-content 누락 metadata를 구현한다.

완료 기준: 0/1/2 native 위치와 retained/clip/transform 조합의 순서 검증, stale epoch·실패 rollback·자원 retirement 검증 통과. 0-view 제품 scene 회귀가 없다. mock planner 통과를 실제 합성으로 기록하지 않는다.

### PV-3 — Windows 합성 선행 검증 및 host 구현

선행: PV-2. work2 WV-2의 최소 WebView2 객체 spike와 함께 실행 가능.

- **PV-3B:** native button/편집기용 HWND attachment와 WebView2 composition attachment를 분리한다. 현재 topmost DComp 아래에서 generic child HWND가 실제 보이는지 먼저 확인한다. UI dispatcher, bounds/DPI, focus, close cleanup을 구현한다.
- **PV-3C:** 단일 content visual을 container root의 background child로 옮기고 native visual과 foreground raster visual을 순서대로 배치한다. 기존 Vulkan/Presentation 경로에서 alpha, resize, present/retirement를 유지한다.
- C ABI 변경, DComp device 소유권, segment surface 생성·반납 책임을 문서화한다. COM 소유자는 unmanaged 호출 종료까지 유지한다.
- WindowsAppSdk와 Windows MAUI adapter를 각각 연결한다. 한 runner 성공을 다른 runner의 결과로 전용하지 않는다.

완료 기준: 실제 창에서 B의 표시/입력 및 C의 메뉴/반투명 modal/두 native 사이 그림, DPI 1/1.25/1.5/2, resize/창 닫기 중 callback을 확인한다. 일반 HWND와 composition view의 지원표를 별도로 채운다. C가 실패하면 원인과 수정할 compositor 결정을 남기고 다음 플랫폼에서 같은 설계가 검증된 것으로 전제하지 않는다.

### PV-4 — Web DOM·worker 합성

선행: PV-2, PV-3의 구조 결정 기록. Windows C 통과 자체는 Web B 착수 조건이 아니다.

- **PV-4B:** main DOM이 stable element/container registry를 소유하고 worker에는 ID·불변 배치만 보낸다. `_html_element_view_web.cs`의 제품 API 연결 방식을 정하고 필요한 compile 제외를 해소한다.
- **PV-4C:** raster segment별 canvas/DOM slot을 도입하고 OffscreenCanvas transfer, 재사용, context loss, resize/DPR, atomic 적용 가능 범위를 구현한다.
- protocol v4 변경 시 loader/main/worker/직렬화 소비자를 함께 갱신한다. out-of-order·이전 protocol·이전 epoch 메시지를 거부한다.
- canvas backing size와 CSS 논리 크기를 분리하고 DOM을 매 frame 재생성하지 않는다. iframe 이동/resize가 reload를 유발하는지도 측정한다.

완료 기준: `worker-direct-webgpu`와 명시적 `worker-direct-webgl` 각각 B/C를 브라우저에서 검증한다. 두 iframe 사이 Doroti 그림, focus, resize, stale packet, element 보존, context loss를 기록한다. renderer 자동 전환으로 실패를 가리지 않는다.

### PV-5 — PointerInterceptor·입력·IME·접근성 공통 통합

선행: PV-1/PV-2, Windows/Web의 해당 B/C. 이후 플랫폼마다 재적용하는 공통 게이트다.

- native 직접 수신과 Doroti 중재형 입력을 구분한다. gesture arena 승패·buffer·cancel 및 부모 scroll 경쟁을 backend별로 정의한다.
- PointerInterceptor의 `intercepting`, `debug`, clip/transform/paint order를 input shield로 내린다. 시각적 전경 surface와 입력 보호를 독립적으로 구현한다.
- Web shield 입력을 기존 ingress에 한 번만 전달한다. 영역 밖 iframe 입력, wheel/drag/capture, 메뉴 종료 시 listener 제거와 입력 복귀를 보장한다.
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

- **PV-8B:** NSView attachment, flipped 좌표/scale, responder chain, mouse/wheel/key/IME, accessibility 연결.
- **PV-8C:** Doroti Metal surface와 NSView 사이 foreground ordering/alpha/clip, live resize, 창 activation을 구현한다.

완료 기준: native AppKit runner에서 B/C와 PV-5를 실행하고 두 창·Tab/Shift+Tab·VoiceOver·close/reopen을 확인한다. Mac Catalyst 성공을 AppKit 성공으로 대체하지 않는다.

### PV-9 — Linux Qt native hosting

선행: PV-2/PV-5 및 work2 WV-7A의 Qt API/버전 결정. generic QWindow B 실험은 WV-7A와 함께 진행 가능.

- **PV-9B:** 현 QWindow 기반 Graphite host에 native attachment를 결합한다. parent/geometry/DPR/focus/hide/종료를 X11과 Wayland에서 각각 구현한다.
- **PV-9C:** native window stacking과 Doroti foreground surface의 교차 합성 가능성을 검증하고 필요한 Qt container 구조를 결정한다. 지원 불가 조합은 구체적인 원인과 제한형 capability를 남긴다.
- QWidget shell/`createWindowContainer()`를 선택하면 host 구조 변경 비용과 opaque native window 제약을 평가한다. Linux generic hosting에는 WebEngine 필수 의존성을 넣지 않는다.

완료 기준: X11/Wayland별 B/C·입력·IME·접근성·live resize·창 종료 증거가 있다. 비겹침 패널 표시를 interleaved 성공으로 승격하지 않는다. [Qt container 공식 제약](https://doc.qt.io/qt-6/qwidget.html#createWindowContainer)을 decision record에 반영한다.

### PV-10 — 제품 통합·성능·배포 회귀

선행: PV-1~PV-9의 구현/제한 판정 완료. 각 target의 미실행 게이트는 계속 `notVerified`다.

- 신규 `DorotiTestbedApp/src/PlatformViewFixture.cs`에 native button/편집기, 2개 중첩 native, 메뉴/modal, 두 owner 창, lifecycle 스트레스를 구성한다. WebView 시나리오는 work2 fixture가 소유한다.
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
| 합성 | 2 native 사이 그림, alpha modal, scroll/resize/DPR/clip | 순서/배치/입력 일치, 미지원 효과의 명시적 거부 |
| 입력 | tap/drag/wheel/capture/부모 scroll, shield on/off | 이중 전달 없음, 보호 영역 외 native 정상 동작 |
| Focus/IME/접근성 | 한글 조합/selection/Tab, 가려진 view, screen reader | focus 주체 하나, native subtree 중복 없음, 가림 정책 일치 |
| 성능 | 0/1/4 view, 동일 workload, 반복 수명 | PV-0에서 정한 기기별 예산 충족 및 측정값/변동 기록 |
| Capture | raster readback와 실제 창 캡처 비교 | native content 포함 여부를 결과에 정확히 명시 |

결과는 신규 `Doroti/docs/validation/platform-views/<date>/<target>/`에 commit, dirty 변경 식별, OS/RID/기기/runtime/renderer, 명령·exit code·timeout, capability 요청/결과, frame/epoch trace, screenshot/video, 실패 원인과 재개 지점을 남긴다. `sourceReviewed`, `build`, `automated`, `productLive`, `physical`, `nativeAot`를 독립 필드로 기록한다. `skippedByUser`는 실제 사용자 생략 요청이 있을 때만 사용한다.

모든 테스트는 [.github 지침](.github/copilot-instructions.md)에 따라 외부 **1200초 timeout**으로 실행한다. 저장소 루트에서의 기본 build 검증 예시는 다음과 같다. 해당 단계의 focused validator와 target build/run도 같은 wrapper를 사용한다. 새 validator의 정확한 명령은 그 validator를 추가하는 단계에서 고정한다.

```powershell
python Doroti/validation/run-with-timeout.py pwsh -NoProfile -File Doroti/eng/doroti.ps1 build
```

계획 작성 당시에는 제품 build/runtime 테스트를 실행하지 않았다. 이번 구현 실행의 실제 명령·결과는 [검증 README](Doroti/validation/platform-views/README.md)와 날짜별 evidence에서 구분한다.

## 6. work2 인계 및 완료 규칙

| work2 단계 | 필요한 PlatformView 인계 |
|---|---|
| WV-0 / WV-1 | PV-0 계약 초안 / PV-1·PV-2 공통 기반 |
| WV-2 Windows | PV-3B로 기본 표시, PV-3C·PV-5로 겹침 제품 승인 |
| WV-3 Web | PV-4B로 iframe 표시, PV-4C·PV-5로 겹침 제품 승인 |
| WV-4 Android | PV-6B, 이후 PV-6C·PV-5 |
| WV-5 UIKit | PV-7B, 이후 PV-7C·PV-5, iOS/Catalyst 별도 |
| WV-6 AppKit | PV-8B, 이후 PV-8C·PV-5 |
| WV-7 Qt | WV-7A 결정과 PV-9 공동 수행, 이후 PV-9B/C·PV-5 |

기본 B 통과 후 work2의 탐색·JS 작업을 진행할 수 있다. 반투명 modal/겹치는 메뉴의 제품 지원은 C와 입력 게이트가 통과한 조합만 허용한다. 플랫폼 공통 지원 선언에는 모든 대상의 실제 증거가 필요하며 문서 체크박스만으로 완료하지 않는다.

주요 로컬 레퍼런스: [Flutter PlatformViewLayer](reference/flutter-master/engine/src/flutter/flow/layers/platform_view_layer.cc), [external embedder 계약](reference/flutter-master/engine/src/flutter/flow/embedded_views.h), [Avalonia attachment](reference/Avalonia-main/src/Avalonia.Controls/Platform/INativeControlHostImpl.cs), [Web embedder](reference/flutter-master/engine/src/flutter/lib/web_ui/lib/src/engine/platform_views/embedder.dart). Flutter 파일의 이식 provenance와 참조 폴더 전체 버전은 구분한다.

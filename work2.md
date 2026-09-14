# WebView 작업계획 — 재구성 PlatformView 소비자

2026-09-14 · 기준 HEAD `227a0b4c7c9534aff2cf2c9edb5b038c9d2656cc`.

**WebView는 [PlatformView 아키텍처](idea.md)의 선택형 소비자로 구현한다.** 공통 hosting·합성·입력은 [work1.md](work1.md)가 소유하고, 이 문서는 탐색·JS·document·profile·리소스·플랫폼 SDK adapter를 소유한다. 이번 변경은 문서 재구성이며 제품 WebView adapter를 구현하지 않았다. 단계는 `TODO`, 제품 기능 검증은 `notVerified`다.

이전 WV-0~WV-9/WV-X 상세 요구는 [원본](history/26-09-14/platformview-rearchitecture/work2.original.md)에 보존했다. 새 계획은 그 기능 범위를 유지하고, Linux **Qt Quick 제품 합성 + 시스템 Qt WebEngine**에 맞춰 표시 접점·선행 조건을 수정한다. 예전 Widgets 우선·Qt callback ABI 3·부재 `ref.md` 전제를 현재 기준으로 사용하지 않는다.

**현재 요구:** WebView는 플랫폼별로 더 적합한 구성을 선택하며 HCPP를 강제하지 않는다. [work1 R6](work1.md)에서 실제 합성·입력/접근성·효과 호환성·성능/메모리·안정성을 비교한다. R-E의 `PlatformEffect`는 공통 효과 의미와 최대한 유사한 비주얼을 제공한다. WebView 위 실제 backdrop와 선명한 Doroti 전경이라는 제품 요구는 유지한다.

**Windows controller 확정:** WindowsAppSdk와 Windows MAUI는 `Microsoft.Web.WebView2.Core.CoreWebView2CompositionController`를 사용한다. 이는 우선 후보가 아닌 필수 선택이다. windowed controller·기본 MAUI WebView handler를 대체 backend로 자동 선택하지 않는다. 기능 adapter를 공유하되 두 runner의 host 결합·실행 증거는 별도로 남긴다.

## 1. 기능과 소유권

| work1 소유 | work2 소유 |
|---|---|
| owner registry, factory, native attachment, generation, dispose completion | controller/widget/settings, SDK environment/profile 수명 |
| scene/mutator, raster/native 순서, transport, frame commit/retirement | navigation/document state, JS·message·request ID |
| input policy/shield, focus·IME 양보, native semantics anchor | 웹 콘텐츠 내부 정책·permission/popup/file/download/fullscreen |
| HWND/native visual/UIView/NSView/Quick item/DOM 부착 계약 | 해당 타입을 제공하는 WebView SDK adapter |
| PlatformEffect 위젯·native/CSS 효과·sample 의존성·frame 수명 | 실제 WebView source의 sample 가능 여부·content invalidation·결합 fixture |

WebView가 별도 native ID registry·compositor·gesture shield를 만들지 않는다. PlatformView도 URL·cookie·JS 객체를 알지 않는다. host의 attachment 종류에 맞지 않는 SDK object는 명시적으로 거부한다. WebViewController, native instance, attachment, navigation/document, profile/environment는 각각 독립 수명이다.

### 1.0 플랫폼별 전략 선택과 효과 수용 조건

기본 정책은 `PlatformPreferred`다. native hierarchy/visual·live texture·GPU compositor·bounded readback·DOM/canvas 중 제품 요구를 충족하는 후보를 선택한다. HCPP 대응은 Android 후보 중 하나이며 Flutter의 API 34+/Impeller 조건을 Doroti WebView 전체의 요구조건으로 올리지 않는다. [Flutter HCPP 참고](https://docs.flutter.dev/platform-integration/android/platform-views)

- WebView identity·문서 상태·실시간 갱신·입력/IME/접근성을 보존하고 raster/native/effect를 scene 순서로 합성한다. live texture도 이 계약이 검증되면 채택 가능하다. 정지 snapshot 교체는 정상 표시 전략이 아니다.
- GPU import/copy를 우선 검토하되 bounded readback/upload를 금지하지 않는다. 실제 영역·빈도·지연·메모리를 비교하고 사전 예산으로 결정한다. 방식 이름이나 readback 0만으로 성공을 판정하지 않는다.
- 선택은 renderer/OS/runtime/provider/control subtree·scene 요구에 따라 협상하고 actual strategy/선택 이유를 기록한다. focus/IME/capture/navigation을 잃는 runtime 재생성은 하지 않는다. 생성 시 선택을 우선하고 runtime handoff는 검증된 조합만 허용한다.
- 효과를 요청한 scene은 실제 backdrop source·동적 갱신·공통 시각 유사성까지 검증한다. PlatformEffect의 자동 material/parameter 보정은 정상 구현이다. source를 못 읽는 tint-only 결과를 정상 blur 근사로 반환하지 않는다. 효과 없는 WebView 지원과 별도로 조회한다.
- 최종 fixture는 `배경 → WebView A → 중간 raster → WebView B → PlatformEffect → 선명한 Doroti child` 및 한 view 위 부분 effect를 포함한다. 공통 strength/tint/테마를 바꿔 플랫폼 간 유사성을 비교한다.
- strategy 지원과 실제 frame atomicity/scanout·성능 승인은 별도다. browser commit 관측을 Android surface transaction ACK와 같은 값으로 보고하지 않는다.

### 1.1 필수 및 선택 범위

| 기능 | 계획상 수용 조건 |
|---|---|
| 공통 필수 | async create/ready/dispose, LoadUri, feature query, typed 오류, attach/detach 상태 보존 |
| 합성·효과 필수 | 선택한 플랫폼 전략으로 실제 WebView source·공통 PlatformEffect 비주얼·입력/전경 shield·동적 content freshness 충족 |
| 기본 탐색 | LoadHtml/base URI, reload/stop/back/forward, URI/title/loading/progress를 가능한 SDK에 연결. 관측 불가 상태는 Unknown |
| 탐색 정책 | redirect/main-frame·subresource 오류, allow/cancel/external, popup. sync policy와 deferral 가능 범위 구분 |
| 앱 콘텐츠 | `LoadAppContentAsync(resourceKey)`, 기존 manifest resolver, 상대 CSS/JS/image/fetch, MIME·origin·Range/media seek |
| JS/메시지 | 명시적 enable, typed 평가 결과, user script/handler, origin/frame·document 제한, timeout/cancel |
| 세션 | persistent/ephemeral profile, 공유/격리, cookies/cache/storage별 명시적 삭제·완료 관측 |
| 실사용 | file chooser·download·permission·fullscreen·process failure/복구. 미지원 feature는 거부/제한 공개 |
| 후속 WV-X | headless, 고급 keep-alive pool, 인증 challenge·proxy·고급 interception·devtools·추가 inappwebview 호환 |

조건부 기능은 WV-0에서 backend별 필수/선택/미지원으로 확정한다. 필수 요구를 제외할 때는 범위 변경과 이유를 기록한다. 기본 detach/reattach는 필수지만 화면 없이 실행하는 headless와 같지 않다. 전체 inappwebview API 동등성은 이번 목표가 아니다.

## 2. 참조와 현재 상태

주 기능 레퍼런스는 [flutter_inappwebview widget](reference/flutter_inappwebview-master/flutter_inappwebview/lib/src/in_app_webview/in_app_webview.dart), [controller interface](reference/flutter_inappwebview-master/flutter_inappwebview_platform_interface/lib/src/in_app_webview/platform_inappwebview_controller.dart), [Windows adapter](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/in_app_webview/in_app_webview.cpp), [Web element](reference/flutter_inappwebview-master/flutter_inappwebview_web/lib/web/in_app_web_view_web_element.dart)다. 로컬 pubspec의 `6.2.0-beta.3`는 참조 스냅샷 버전이며 최신 release나 Doroti 검증 버전이 아니다.

현재 generic native controls의 B/C와 Qt WebEngine 독립 probe에 관한 기록이 있지만 `Doroti/src`의 WebView 제품 구현을 확인하지 못했다. generic control 생성·iframe load·Qt probe를 WebView API/합성/JS/profile 제품 완료로 계산하지 않는다. 과거 증거 부재는 work1 상태표를 따른다.

| backend | native 기능 adapter / 표시 접점 | 공통 PlatformView 의존성 |
|---|---|---|
| WindowsAppSdk | CoreWebView2CompositionController + 호환 raster/effect visual | controller는 고정. GPU/기타 raster 전송은 제품 결과로 선택. 현재 layered BUTTON/EDIT C와 호환을 가정하지 않음 |
| Windows MAUI | 동일 CoreWebView2CompositionController 기능 adapter, runner 결합 별도 | MAUI presenter와 RootVisualTarget/input 연결 검증 |
| Android | `android.webkit.WebView` + 플랫폼별 선택 전략 | hierarchy/live texture/bounded readback/HCPP 대응 후보 비교. 실제 subtree/OS/provider의 입력·효과·성능 검증 |
| iOS / Mac Catalyst | WKWebView / UIView | UIKit R5 및 runner별 R4/R7 |
| AppKit | WKWebView / NSView | AppKit R5 및 responder/IME/semantics |
| Web | iframe / main DOM / CSS backdrop effect | R5 multi-canvas와 R-E CSS effect 제품 연결, 협력 가능 origin 범위의 입력/JS |
| Linux | 시스템 Qt WebEngine Quick / 같은 QQuickWindow의 item | Quick attachment·graphics backend 공존 spike, X11/Wayland별 R5 |

Qt Widgets/QWebEngineView는 기존 B probe·제한 경로로 남긴다. Quick Controls C가 WebEngine Quick C를 보장하지 않는다. Qt WebView wrapper 대신 WebEngine 직접 adapter와 C ABI를 사용한다. Qt WebEngine은 Widgets/Quick 접점을 구분하므로 현재 Quick host에 맞는 접점을 검증한다. [Qt WebEngine overview](https://doc.qt.io/qt-6/qtwebengine-overview.html), [Qt WebView 문서](https://doc.qt.io/qt-6/qtwebview-index.html)

## 3. 공통 API와 수명 계약

### 3.1 패키지 구조

- 제안 `Doroti.WebView.Core`: controller, settings, navigation/document/profile DTO·feature query. framework Widget 의존성을 피한다.
- 제안 `Doroti.WebView`: `WebViewWidget`과 Core·기존 typed PlatformView 연결. 앱이 쓰는 진입 패키지다.
- 제안 `.Windows/.Android/.UIKit/.AppKit/.Web/.Linux.Qt`: SDK adapter·manifest factory·native asset 등록. 실제 TFM/project 수는 WV-0에서 확정한다.
- Linux의 자체 `libdoroti_webview_qt.so`는 opt-in shim이며 시스템 Qt/WebEngine에 동적으로 연결한다. generic host에 WebEngine dependency를 강제로 추가하지 않는다.
- 기존 [application boundary](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs), [runner targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets), Testbed/template 등록 흐름을 사용한다. public 계약에 native pointer/SDK 객체를 노출하지 않는다.

### 3.2 Controller·명령·이벤트

`WebViewController`는 widget rebuild와 독립이고 같은 owner의 PlatformView controller/handle을 소유한다. `CreateAsync` 완료는 native ready이며 page loaded·첫 content frame이 아니다. 초기 명령은 준비 전 NotReady로 거부하는 단순 정책을 기본안으로 하고, 필요한 대기 기능은 explicit `Ready` 이후 사용한다. backend 작업 queue는 상한을 두며 navigation·JS·dispose 순서를 정의한다.

명령/응답은 `(owner, instanceGeneration, navigationId, documentGeneration, requestId)`로 식별한다. 새 document에서 이전 JS pending을 취소하고 late 결과를 폐기한다. SPA same-document navigation과 새 document navigation을 구분한다. widget detach 때 살아 있는 native를 재생성하지 않는다.

SDK의 main-frame navigation started/committed/completed/error와 first content frame은 별도 이벤트다. 관측 못 하는 값은 Unknown/unsupported이며 임의 progress나 synthetic Presented를 만들지 않는다. 외부 URL·popup은 앱의 명시적 policy를 거치고 UI thread를 동기 대기시키지 않는다. sync 결정 API에는 사전 정책, deferral API에는 timeout/cancel을 사용한다.

dispose는 새 명령 admission 차단 → pending 취소·handler 해제 → attachment/input 차단 → 공통 retirement → native SDK 해제 순서의 의존성을 가진다. profile/environment는 마지막 view와 별도 소유자가 해제할 때 닫는다. close/failed process 후 callback도 generation 검사를 거친다.

### 3.3 JS·origin·profile·앱 콘텐츠

| 계약 | 필수 조건 |
|---|---|
| message envelope | version·request/document 식별·이름·typed payload, 크기/pending/timeout 상한 |
| 출처 검증 | SDK가 제공한 origin/frame/source를 사용. payload의 자칭 origin을 신뢰하지 않음 |
| bridge | 허용 origin/world/frame만 노출, native 객체/reflection 공개 금지, navigation/dispose 때 listener 회수 |
| JS 결과 | primitive/배열/map/null·undefined·평가 실패·직렬화 불가 구분, Promise 의미는 SDK별로 명시 |
| profile | shared/isolated/ephemeral 지원 범위를 실제 SDK에 맞춤. Android cookie 일부 분리로 완전한 profile 격리 주장 금지 |
| data clear | cookie/cache/localStorage/IndexedDB 등 삭제 범위·완료를 별도로 확인. view dispose와 혼동 금지 |
| content | manifest의 resource key→정규화 경로, origin·MIME·relative fetch, 취소/response 수명, Range/media |
| 기본 정책 | TLS 오류 무시·전체 file 접근·권한 자동 승인 없음. popup/download/외부 protocol은 명시적 앱 정책 |

앱 콘텐츠 접점은 Windows virtual-host mapping/resource interception, Android WebViewAssetLoader, Apple WKURLSchemeHandler/제한 file base, Linux UrlSchemeHandler, Web same-origin URL이다. 이들은 구현 후보이며 API별 `since`·fetch/보안 컨텍스트·Range 지원을 WV-0와 각 adapter 단계에서 공식 SDK 문서 및 실제 페이지로 검증한다. loopback HTTP 서버는 기본 의존성으로 추가하지 않는다.

Web cross-origin iframe에는 임의 JS/history/cookie·navigation interception·native와 같은 pointer 중재를 약속하지 않는다. same-origin 또는 협력 페이지 bridge만 별도 feature로 제공하며 COOP/COEP/CSP/sandbox·frame source와 source window를 검증한다. iframe load를 HTTP 성공이나 첫 표시로 간주하지 않는다. [HtmlElementView의 DOM/iframe 제약](https://api.flutter.dev/flutter/widgets/HtmlElementView-class.html)

## 4. 실행 gate

아래 backend 구현의 R6/R-E/WV-H 선행은 **attachment·effect 계약과 조기 probe 준비**를 뜻한다. 공통 host와 WebView adapter를 함께 구현한 뒤 R6 합성, R-E 효과 결합, WV-H 제품 검증을 마감한다. work1 R7과 WV-9는 동일 제품 증거를 공유하는 공동 승인으로 수행하며 서로의 최종 완료를 기다리는 순환 의존성을 만들지 않는다.

### WV-H — 플랫폼별 전략 + 공통 PlatformEffect 조기 결합 gate

선행: work1 R1/R3 계약. 최소 native SDK probe는 WV-0와 함께 시작하며 최종 제품 승인은 해당 R6/R-E와 WV-2~WV-7 구현 뒤에 수행한다. 새 상태 `TODO`/`notVerified`.

WV-H ID는 기존 인계를 위해 유지하며 이제 HCPP 전용 gate를 뜻하지 않는다. 후보별 기능/입력·시각 fidelity를 먼저 확인한 뒤 성능·메모리·안정성을 비교한다.

| backend | 먼저 확인할 결합 | 실패 시 처리 |
|---|---|---|
| Android | hierarchy·live texture·bounded readback·HCPP 후보로 동일 WebView+effect 장면 비교 | source sample/입력/예산 불충족 후보 제외. 충족한 다른 전략으로 선택 가능 |
| Windows | CoreWebView2CompositionController RootVisualTarget + raster/effect visual의 호환 tree·전송 방식 | controller 선택 유지. API 객체 혼용 또는 host backdrop만 성공하면 결합 미완료 |
| UIKit/AppKit | WKWebView + 공통 effect intent의 public material/blur/tint 매핑 + 선명한 전경 | 강도/색감 유사성과 source 검증. native alpha/mask/겹침 제약은 다른 lowering 또는 명시적 제한, private CAFilter 사용 안 함 |
| Linux Quick | 같은 QQuickWindow의 WebEngine item·GPU raster를 live source로 sample, effect/child 제외 | GPU backend 불일치·sample 누락·recursive feedback이면 미완료. QWidget B로 대체하지 않음 |
| Web | canvas/iframe/effect DOM/foreground 순서, CSS backdrop root, pointer pass-through | CSS.supports만 성공하거나 iframe pixels가 실제로 흐려지지 않으면 미승인. cross-origin JS를 요구하지 않음 |

최소 결과는 handle/topology/actual transport·선택 이유·source coverage/freshness·입력/clip·공통 reference 대비 blur 강도/색감·전송 비용·실패 이유다. 선명한 child와 native blur를 같은 capture에서 확인하고 framework repaint 없이 변하는 WebView animation도 검사한다. 보호된 media/별도 surface는 별도 행으로 기록한다. generic native 표시 probe만으로 gate를 생략하지 않는다.

### WV-0 — 기능 대응·attachment·배포 범위 고정

선행: work1 R0/R1 계약 초안. 구현 상태 `TODO`.

- backend별 기능표에 SDK API/버전·thread·callback·지원/조건/미지원·검증 fixture를 연결한다. wrapper 이름만으로 기능 동등성을 정하지 않는다.
- 공개 API/프로젝트·TFM·NativeAOT 직렬화 전략, OS/RID/runtime/provider 최소 범위를 고정한다. 새 계획 작성 중에는 SDK 지원 버전을 임의 승격하지 않는다.
- Windows composition visual과 Linux Quick item의 최소 실제 WebView attach probe를 조기에 수행한다. 결과로 work1 attachment 계약을 보정한다.
- WV-H에서 각 backend의 후보 전략과 effect source·공통 비주얼을 함께 검증한다. Android는 같은 scene으로 입력/효과/비용을 비교하고 HCPP 채택 필요성을 판단한다.
- local HTML fixture에 navigation/redirect/history, JS 왕복, editable/IME, media, popup/permission, profile data를 준비한다. 0/1/4 view·두 owner·route lifecycle 기준을 연결한다.

완료: 요청 기능별 소유권·API 접점·인계/실패 조건이 있으며 generic control만으로 WebView support를 등록하지 않는다.

### WV-1 — 공통 controller/widget/profile

선행: WV-0 + work1 R1. 실제 제품 attachment 통합은 해당 R3/R5에 의존한다.

- Core/widget·feature query·typed error·create/ready/dispose·bounded queue·native handle facade를 구현한다.
- owner/instance/document/request generation, handler/JS cancellation, keep-alive lease, profile/environment 소유권을 연결한다.
- navigation policy·origin/frame·content resolver·JS 직렬화 공통 계약을 만든다. 플랫폼 지원 차이는 capability로 노출한다.

완료: 두 owner, create/close race, stale navigation/JS callback, timeout·직렬화 실패, profile scope 테스트. fake SDK 결과는 제품 WebView 검증과 분리한다.

### WV-2 — Windows CoreWebView2CompositionController

선행: WV-1 + WV-H 조기 probe 및 work1 Windows R6/R-E attachment/commit 계약. Windows MAUI는 별도 gate다.

- STA/UI dispatcher에서 CoreWebView2Environment와 profile options를 준비하고 `CreateCoreWebView2CompositionControllerAsync(parentHwnd[, options])`로 생성한다. owner별 controller를 보존하고 runtime 부재·생성 실패·생성 중 close/late completion을 구분한다.
- `RootVisualTarget`를 host 소유 visual에 연결한다. 위치/transform/clip/z-order와 `Bounds`·DPI/rasterization scale의 의미를 맞춘다. parent HWND는 owner 수명과 입력 접점이며 windowed WebView의 표시 경로로 사용하지 않는다. resize/순서 변경만으로 controller를 재생성하지 않는다.
- `SendMouseInput`으로 mouse/wheel/leave를, `SendPointerInput`으로 touch/pen을 전달한다. 좌표·capture·pointer sequence와 CursorChanged/cursor 갱신을 공통 input bridge에서 관리한다. 전경 shield는 WebView 전달 전에 적용하며 keyboard/focus/IME·Tab 경로는 controller/host의 실제 계약으로 별도 연결한다.
- 종료는 새 입력/명령 차단·event 해제·RootVisualTarget 연결 해제와 frame retirement를 조율한 뒤 SDK controller Close 및 참조 해제로 마감한다. 생성 중 owner close 뒤 도착한 controller도 같은 UI dispatcher에서 닫는다. SDK Close를 GPU/presentation retirement 완료 신호로 취급하지 않는다.
- navigation·JS/messages·app content·data clear·popup/file/download/permission/process failure를 feature표에 맞춰 구현한다.
- 실제 WebView A/Doroti 중간/WebView B/전경 modal로 C1~C6, resize/DPI·한글 IME·접근성을 검사한다.
- 호환 tree의 backdrop effect와 선명한 전경을 검증한다. raster 전송·effect sample·frame retirement는 공통 host가 담당하며 WebView adapter 안에 별도 compositor를 만들지 않는다.

완료: 실제 페이지 기능·합성·공통 효과 비주얼·dispose race·clean 배포·NativeAOT를 검증한다. CoreWebView2CompositionController 선택은 유지하며 raster/effect 후보 변경은 actual strategy와 비교 결과에 기록한다. 어떤 호환 구성도 요구를 충족하지 못하면 `PARTIAL`이다. 기존 BUTTON/EDIT 결과와 분리한다.

추가 확인: 실제 생성 경로가 CoreWebView2CompositionController인지, RootVisualTarget 연결·단일 입력 전달·resize 시 같은 controller 유지·생성 중 close 회수가 되는지 검사한다. managed SDK/COM interop를 포함한 NativeAOT publish/run도 검증하며 controller 선택 자체를 다른 backend로 바꾸어 AOT gate를 통과시키지 않는다.

공식 계약: [CreateCoreWebView2CompositionControllerAsync](https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environment.createcorewebview2compositioncontrollerasync?view=webview2-dotnet-1.0.3856.49), [CoreWebView2CompositionController visual/input API](https://learn.microsoft.com/en-us/microsoft-edge/webview2/reference/winrt/microsoft_web_webview2_core/corewebview2compositioncontroller?view=webview2-winrt-1.0.3856.49). 문서의 SDK 버전은 참조본이며 Doroti 패키지 버전 pin은 실제 의존성/AOT 검증 단계에서 확정한다.

### WV-3 — Web iframe

선행: WV-1 + work1 R5 Web 제품 연결, R6 browser compositor 계약 및 R-E CSS effect.

- main DOM factory와 stable iframe을 typed handle에 연결하고 navigation 시 src/document revision을 관리한다. 위치/순서 변경 때 iframe reparent/recreate로 state를 잃지 않는다.
- cross-origin·same-origin·협력 bridge를 기능표로 나눈다. unknown progress/history·JS unsupported를 정직하게 반환한다.
- worker-direct WebGPU/WebGL, multi-canvas·shield·DOM focus·2 owner·DPR·context loss·stale packet을 실제 제품에서 확인한다.
- main DOM effect element를 iframe 뒤·Doroti child 앞에 배치한다. source iframe은 이동/가림 때 재생성하지 않는다. backdrop root·ancestor opacity·rounded clip·cross-origin iframe/영상과 browser별 실제 sample을 확인한다. 효과는 기본 pointer-events none이며 필요한 전경 입력은 기존 shield로 보호한다.

완료: load·identity·선택 bridge·origin 거부·modal·dispose 기능이 제품에서 동작한다. 독립 web-dom harness와 iframe load만으로 gate를 닫지 않는다.

### WV-4 — Android WebView

선행: WV-1 + work1 R4/R5/R6 Android 및 R-E, WV-H의 전략/효과 조기 비교.

- 실제 WebView factory·UI lifecycle·provider feature query·설정·앱 콘텐츠·navigation/JS를 구현한다. cookie/profile 격리 제약을 공개한다.
- native-origin 부모 scroll와 WebView 내부 scroll/selection·pinch, IME·autofill·accessibility, media/SurfaceView 자식의 representation 제약을 검증한다.
- rotate/insets/background/resume·renderer process 종료·owner close를 재현한다. API/ABI/provider별 feature와 실제 device 결과를 남긴다.
- WebView 생성 시 PlatformPreferred 정책으로 후보를 협상한다. 선택한 hierarchy/texture/transfer 경로를 제품에 연결해 blur source·공통 비주얼·입력·성능을 검증한다. SurfaceControl/HCPP는 유리할 때만 채택한다. 일반 View.Draw 또는 API31 cross-window blur 성공을 inline backdrop 지원으로 계산하지 않는다.

완료: 실제 WebView에서 C1~C6·기능/오류/취소·배포가 통과한다. generic EditText·기존 spinner baseline을 WebView 성능으로 전용하지 않는다.

### WV-5 — UIKit iOS / Mac Catalyst

선행: WV-1 + work1 UIKit R4/R5/R6/R-E.

- WKWebView configuration/data store/script message handler·navigation delegate·scheme handler를 생성 전에 설정한다. UIView attachment·async callback·handler retain cycle·dispose를 연결한다.
- native gesture/scroll/selection·키보드 inset·한글 IME·focus·VoiceOver·mixed foreground를 검증한다. group effects는 지원표대로 처리한다.
- UIVisualEffectView 등의 public material/blur·tint를 공통 strength/비주얼에 매핑하고 선명한 child를 유지한다. alpha/mask·Reduce Transparency·입력 통과·source animation을 검증하며 private filter로 numeric sigma를 맞추지 않는다.

완료: simulator/device와 iOS/Catalyst runner를 별도 승인한다. 앱 배포·AOT는 신규 binding/bridge가 포함된 final runner에서 검증한다. 과거 renderer-only Apple 생략 기록을 이번 WV 작업의 사용자 생략으로 확대하지 않는다.

### WV-6 — AppKit macOS

선행: WV-1 + work1 AppKit R4/R5/R6/R-E.

- NSView/WKWebView responder·좌표·backing scale·focus/IME·VoiceOver와 Metal/native 합성을 연결한다. UIKit 코드를 이름만 바꿔 이식하지 않는다.
- navigation/JS/profile/assets와 두 WebView·modal·resize·close를 실제 AppKit 제품에서 검증한다.
- NSVisualEffectView withinWindow를 WKWebView 위에 배치한다. 기존 BehindWindow backdrop와 수명/설정을 분리하고 Apple이 금지하는 effect view끼리 겹침은 명시적으로 거부한다. material theme·focus·screen reader·선명한 Doroti 전경을 확인한다.

완료: 실제 창 픽셀·입력·data store·late callback·배포 증거가 있다. 현재 부재한 과거 AppKit artifact를 근거로 미실행 기능을 승인하지 않는다.

### WV-7 — Linux 시스템 Qt WebEngine Quick

현재 기준은 [Testbed Linux 설정](DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj)의 Quick 선택, [QtPlatformViewHost](Doroti/src/Doroti.Host.Qt/QtPlatformViewHost.cs), [Quick native host](DorotiTestbedApp/linux/native/src/doroti_qt_quick.cpp)다. [QtNativeV2](Doroti/src/Doroti.Host.Qt/QtNativeV2.cs)는 이름과 달리 callback ABI **4 / 192 bytes**, preparation offset 184이며 [Quick 기능](Doroti/src/Doroti.Host.Qt/QtQuickNative.cs)은 선택 bit 17이다. 새 WebView ABI는 이들을 덮어쓰지 않고 version/size/features를 협상한다.

공급 정책은 유지한다. 배포판의 Qt/WebEngine runtime을 사용하고 Chromium·Qt 엔진을 앱/NuGet에 복사하거나 직접 다운로드·빌드하지 않는다. 자체 shim과 managed adapter만 Doroti가 공급한다. WebEngine 미사용 앱에는 해당 의존성을 추가하지 않는다. 시스템 패키지의 helper/resources/locales/QML module 비용은 별도로 기록한다. [Qt 배포 구성](https://doc.qt.io/qt-6/qtwebengine-deploying.html)

| 단계 | 작업 | 완료/중단 조건 |
|---|---|---|
| WV-7A 패키지/API | linux-x64부터 시스템 Core/Quick/Qml/WebEngineQuick/WebChannel·QML module·toolchain/QPA closure 조사. 이전 6.8 API 하한은 후보이며 Quick 공개 API 요구와 함께 재확정 | 최소 patch·배포판 build·현재 보안 업데이트·required API표. 예전 패키지 버전을 최신 지원표로 복사하지 않음 |
| WV-7B 실제 Quick spike | 현재 Quick GPU 구성을 우선해 공개 WebEngineView + effect + 선명한 child·공통 비주얼 비교. graphics backend·Chromium GPU 공존 검사 | XWayland/native Wayland C1~C6/E1~E3·비용 통과. 실패 시 시스템 Qt 정책 안의 다른 공개 구성을 비교하고 선택 이유/지원 범위를 갱신. private texture extraction 금지 |
| WV-7C C ABI·startup | existing pre-application hook에 scheme/WebEngineQuick 초기화 순서 연결, opt-in shim load·Quick item attachment·focus/event·owner close. 공개 C ABI만 managed에 노출 | 같은 QApplication/event loop/Qt build, version/size/features 거부, 두 owner·late callback·모듈 종료 수명 검사 |
| WV-7D 기능 | 공개 Quick WebEngineView/WebEngineProfile·script/navigation/message API를 기능표에 매핑. 필요한 QWebChannel/scheme 기능은 공개 API로만 연결 | Quick에 없는 QWebEnginePage/Widgets API를 있다고 가정하지 않음. JS/origin/profile/content 성공·오류·취소 재현 |
| WV-7E 배포 | runner targets·Testbed/template·CMake에 shim build/copy/publish, distro dependency·helper/data/QML manifest 연결 | 개발 Qt 경로 없는 clean install/run, runtime/버전/helper/QML 누락별 typed 오류, 미사용 앱 무의존성 |
| WV-7F 제품 승인 | work1 Quick R4/R5/R6/R-E/R7과 함께 native/GPU 합성·효과·입력·IME·Orca·DPR/resize·두 owner·0/1/4-view·process recovery | QPA·VM/물리 GPU·OS/runtime별 결과 분리. generic Quick Controls/Widgets probe로 WebEngine 승인 금지 |

WV-7B는 WV-0부터 최소 조사/probe를 시작한다. WV-7C는 WV-1과 공통 attachment 계약 확정 후 구현한다. QWebEngineView/Widgets B 경로는 비교·제한 지원으로만 남기며 C 실패를 B 성공으로 종료하지 않는다.

Qt Quick rendering은 현재 Qt 소유 Vulkan instance/device/queue, Graphite R→P copy, basic render loop·retirement를 유지한다. WebEngine 내부 texture 소유권은 Chromium/Qt에 맡긴다. 공개 API가 제공하지 않는 native pointer를 추출하지 않는다. [Qt Quick 소유권·thread 구조](https://doc.qt.io/qt-6/qtquick-visualcanvas-scenegraph.html)

startup은 profile/scheme 설정 시점과 application 생성 전 요구를 SDK 버전별로 확인한다. shim의 live QObject·callback·process hook이 남으면 unload하지 않는다. native/managed 예외가 ABI 밖으로 전파되지 않도록 오류로 변환한다. custom scheme response device·cancel·Range/media 기능은 별도 검사한다.

QWebChannel message 안의 origin은 신뢰 근거가 아니다. native에서 검증할 수 있는 world/frame 범위가 부족하면 trusted app content/main-frame bridge로 제한한다. profile 저장 정책은 page/item 생성 전에 결정하고 cookie/cache 삭제와 전체 storage 삭제를 구분한다.

일반 사용자와 기본 Chromium sandbox 조건에서 clean 실행한다. no-sandbox/root 진단 결과는 제품 PASS가 아니다. 배포판·Qt build·QPA plugin·sandbox/GPU 실패를 각각 기록한다. 시스템 Qt 보안 업데이트 후의 재실행 조건도 문서화한다. [Qt WebEngine 플랫폼 조건](https://doc.qt.io/qt-6/qtwebengine-platform-notes.html)

### WV-8 — 정책·복구·상태 보존 마감

선행: 해당 WV-2~WV-7 기능 구현. 각 backend 개발 중에도 해당 검증을 수행한다.

- redirect/popup/외부 protocol, file chooser cancel, download 수명, permission 허용/거부, fullscreen 복귀를 같은 fixture로 비교한다.
- trusted/untrusted origin/frame, oversized·late message, TLS 오류, 리소스 경로 이탈을 거부한다.
- profile 공유/격리/ephemeral 종료·명시적 데이터 삭제, keep-alive 재attach·dispose race를 검증한다.
- process failure의 terminal·재생성 정책을 명확히 한다. 새 엔진이 복구하지 못한 history/form/media 상태를 복구 성공으로 표시하지 않는다.

완료: 광고한 기능의 성공/실패/취소·복구 경로가 있고 UI deadlock, stale JS 실행, listener/native instance 누수가 없다.

### WV-9 — 제품 Lab·성능·배포 승인

선행: 해당 WV-8, WV-H 최종 제품 검증 및 work1 R4/R5/R6/R-E/R7의 WebView 종류에 대한 게이트.

- `WebViewFixture`와 실제 sample에 navigation/local content/feature query·미지원 안내·IME·modal·여러 view·profile 선택을 연결한다.
- 공통 PlatformEffect strength/tint·부분 clip·선명한 child·입력·동적 source를 fixture에 추가한다. 같은 reference content의 blur/색감 유사성을 비교하고 effect 수·sample pixels·GPU pass·copy/readback bytes·메모리를 예산으로 평가한다. 정상 시각 근사와 degraded/접근성 대체 상태를 구분한다.
- 0/1/4 view의 frame p50/p95/p99, JS 왕복·입력 지연, native/process/GPU memory·listener/overlay 수를 측정한다. 생성 완료·GPU submit을 첫 content 표시 시간으로 대체하지 않는다.
- 같은 OS/runtime/device/content/소스로 변경 전후를 비교한다. iframe 내부 관측 불가 항목은 미산출로 남긴다.
- template부터 final runner publish/install/run과 NativeAOT ILC/native link/bridge 실행을 검증한다. 미사용 앱 dependency/크기 회귀도 확인한다.
- 지원표·API 예제·runtime 준비/typed 오류·iframe 제한·profile 삭제·리소스·배포·재현 명령을 작성한다.

완료: 광고할 backend/feature에 증거가 있다. 환경·합성·실기기·배포 미완료가 남으면 전체 상태는 `PARTIAL`이다.

### WV-X — 선택 확장

headless, 고급 keep-alive/pooling, 인증/proxy/interception, devtools, inappwebview 추가 API는 별도 요구에 따른다. 확장점을 두되 no-op 성공 stub을 공개하지 않는다. 기본 C/입력/JS/profile/배포 미완료를 WV-X로 옮겨 범위를 축소하지 않는다.

플랫폼별 적합한 WebView 구성과 공통 PlatformEffect 결합은 필수 목표다. HCPP 구현 자체는 필요성이 입증될 때 선택하는 연구/전략이며 필수가 아니다.

## 5. 검증·기록 및 인계

검증 source는 `Doroti/validation/webview/`, 산출물은 `Doroti/artifacts/webview/<date>/<target>/<run>/`에 둔다. 신규 API 가이드는 구현과 함께 작성한다. `[sourceReviewed, build, automated, productLive, physical, nativeAot]`와 OS/RID/SDK/runtime/provider/QPA/renderer·source hash·명령/exit/실패를 분리한다. 없는 과거 파일은 unavailable로 기록한다.

모든 build/test/run child는 [.github 지침](.github/copilot-instructions.md)의 20분 timeout과 [기존 wrapper](Doroti/validation/run-with-timeout.py)를 사용한다. 이번 문서 변경에서는 실행하지 않았다. 기존 동일 source/환경의 통과 증거는 재사용하고 변경/실패가 필요한 검증만 추가한다. `skippedByUser`는 실제 사용자 생략 지시가 있을 때만 사용한다.

| 미결정 항목 | 종료 단계 | 필요한 근거 |
|---|---|---|
| Core/Widget assembly·TFM·기능 최소값 | WV-0/1 | SDK 대응·AOT boundary·실제 caller |
| Windows visual family·GPU interop·AOT | WV-0 probe/WV-2 + work1 R5/R6 | CoreWebView2CompositionController 선택은 확정. RootVisualTarget/input/GPU raster/effect·final publish 실제 결합 검증 |
| 플랫폼별 전략·공통 효과 결합 | WV-H + work1 R6/R-E | 실제 source·동적 갱신·입력·비주얼 유사성·전송/메모리/지연·공개 API와 선택 이유 |
| Android WebView profile/gesture 범위 | WV-0/WV-4 + work1 R4 | OS/provider feature와 실제 scroll/IME |
| Web 협력 bridge·origin 범위 | WV-0/WV-3 | browser policy·메시지 source 검증 |
| Linux Quick 공개 API·Vulkan 공존·최소 Qt build | WV-7A/B | 시스템 패키지·실제 C 장면·private API 미사용 |
| 성능/clean deployment/NativeAOT 승인 | WV-9 + work1 R7 | 해당 신규 WebView 포함 final 제품 증거 |

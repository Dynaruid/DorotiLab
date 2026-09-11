# 플랫폼별 WebView 작업계획

작성일: 2026-09-11 · 검토 HEAD: `cf83abd603aa06b942051cabdeb5132f4dd774c7`

기준: [idea.md](idea.md), 공통 호스팅 선행 계획: [work1.md](work1.md). 이 문서는 **계획 작성 결과**다. 아래 단계는 모두 `TODO`, WebView 제품 실행·성능·NativeAOT 검증은 `notVerified`다. API/패키지/신규 산출물 경로는 구현 단계에서 확정할 제안이다.

## 1. 목표와 구현 범위

공통 `WebViewController + WebViewWidget`를 제공하고 Windows WebView2, Android WebView, iOS/Mac Catalyst WKWebView, native AppKit macOS WKWebView, Web iframe, Linux Qt backend로 연결한다. WebView2·Qt WebEngine 등의 의존성은 해당 backend를 선택한 앱에만 포함한다.

주 레퍼런스는 [로컬 flutter_inappwebview](reference/flutter_inappwebview-master/README.md)다. 공통 기능·생성 옵션·controller·이벤트·keep-alive/profile 의미를 참고하되 API 전체를 일괄 포팅하지 않는다. 로컬 pubspec의 `6.2.0-beta.3`는 참조 스냅샷 표기이며 최신 배포 버전이나 Doroti 검증 버전이 아니다. Linux는 idea.md의 **Qt WebView 계열** 방향을 유지한다.

| work1이 제공하는 기반 | 이 문서가 구현하는 기능 |
|---|---|
| 생성/attachment/placement/수명과 generic capability | WebView factory, controller/widget, typed options |
| compositor·foreground surface·PointerInterceptor | WebView 위 앱 메뉴/modal을 사용하는 통합 시나리오 |
| native 입력/focus/IME/semantics 경계 | 웹 문서 입력·selection·탐색·JS·process 이벤트 |
| frame/epoch/instance generation | navigation ID/request ID/profile ID와 비동기 취소 |

work1 전체 완료를 기다려야 모든 기능 작업을 시작할 수 있는 구조로 만들지 않는다. backend의 기본 배치 B가 준비되면 기능을 개발하고, 겹침 제품 지원은 C와 입력 검증 이후 승인한다. WebView 전용 코드에서 공통 compositor를 중복 구현하지 않는다.

## 2. 기능 범위와 지원 의미

### 2.1 필수 공통 기반과 선택 기능

| 분류 | 계획 범위 | 완료 기준 |
|---|---|---|
| 필수 공통 기반 | async create/ready/dispose, widget attach, LoadUri, capability query, typed 오류 | 모든 backend가 실제 지원/제약을 보고하며 늦은 응답·dispose 경쟁을 처리 |
| 기본 탐색 | LoadHtml/base URI, reload/stop/back/forward, URI/title/loading/progress | 플랫폼별 가능 기능을 구현하고 unavailable/unknown 상태를 명시 |
| 탐색 정책 | allow/cancel/external, redirect, main-frame/subresource error, popup | callback timing/취소 가능 범위를 실제 SDK와 대응시킴 |
| 앱 콘텐츠 | `LoadAppContentAsync(resourceKey)`, 상대 리소스/fetch/media | 기존 resource manifest와 연결하고 origin·경로·MIME 정책 검증 |
| JS/메시지 | 명시적 JS enable, async 평가, typed 결과, user script, handler 등록/해제 | native backend 우선 구현, Web은 동작 가능한 origin/협력 페이지 범위만 제공 |
| 세션 | persistent/ephemeral profile, cookies/storage/cache, 명시적 data clear | native backend별 지원 범위·삭제 범위·수명 분리 검증 |
| 실사용 확장 | file chooser, download, permission, media/fullscreen, process recovery | 지원 feature는 구현·검증, 미지원 feature는 typed 오류/정책으로 처리 |
| 후속 확장 | headless, 고급 keep-alive pool, devtools UI, 인증·고급 request interception | WV-X로 분리. API 전체 동등성은 이번 필수 범위가 아님 |

기본 attach/detach 상태 보존은 필수 수명 계약이다. widget과 독립적인 명시적 keep-alive token/profile 소유권은 WV-1에서 정하고 지원 backend에서 구현한다. 화면 없이 독립 실행하는 headless는 별도 후속 기능이다.

### 2.2 플랫폼별 목표표

아래는 **구현 목표/확정할 제한**이며 현재 지원표가 아니다. `조건부` 기능은 WV-0 대응표에서 필수·선택·미지원으로 확정하고, 필수 기능을 제외하면 사유와 범위 변경을 기록한다.

| 대상 | 엔진/API | 기본 탐색 | JS/message | profile/assets | 겹침 의존성 |
|---|---|---|---|---|---|
| WindowsAppSdk | WebView2 composition controller | 목표 | 목표 | 목표 | PV-3C/PV-5 |
| Windows MAUI | 같은 WebView2 기능 adapter, 별도 host 결합 | 목표 | 목표 | 목표 | 해당 runner PV-3C/PV-5 |
| Android | `android.webkit.WebView` | 목표 | 목표 | OS/provider별 조건부 격리, asset loader | PV-6C/PV-5 |
| iOS | WKWebView / UIKit | 목표 | 목표 | data store와 scheme 정책 검증 | PV-7C/PV-5 |
| Mac Catalyst | WKWebView / UIKit, 별도 runner | 목표 | 목표 | iOS 결과와 별도 검증 | PV-7C/PV-5 |
| AppKit macOS | WKWebView / NSView | 목표 | 목표 | 목표, 실제 API별 확인 | PV-8C/PV-5 |
| Web | iframe / main DOM | LoadUri 중심, history/정밀 상태 조건부 | same-origin/협력 페이지만 조건부 | 브라우저 저장소 정책, same-origin asset | PV-4C/PV-5 |
| Linux | Qt WebView 계열, WV-7A에서 API 확정 | 목표 | 공개 API 충족 여부에 따라 Qt WebEngine 직접 adapter 판단 | 동일 결정에서 확정 | PV-9C/PV-5, X11/Wayland 분리 |

Web의 cross-origin iframe은 임의 JS/history/cookie/탐색 차단을 native WebView와 동등하게 제공할 수 없다. `UnsupportedFeature`, `Unknown` 상태와 협력 bridge 지원을 구분한다. progress를 관측할 수 없으면 임의 비율을 만들지 않고, iframe load event를 HTTP 성공 또는 첫 content 표시로 단정하지 않는다.

## 3. 공통 구조와 계약

### 3.1 패키지와 실제 변경 위치

| 위치 | 책임 / 예정 산출물 |
|---|---|
| 신규 `Doroti/src/Doroti.WebView/` | controller/widget, navigation/state/settings, typed message·profile·resource 계약 |
| 신규 `Doroti.WebView.Windows`, `.Android`, `.UIKit`, `.AppKit`, `.Web`, `.Linux.Qt` | 플랫폼 factory·SDK adapter·옵션·feature query. 최종 프로젝트/TFM 분리는 WV-0에서 확정 |
| [DorotiApplicationBoundary](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs) | 기존 manifest/plugin/resource 등록 접점 재사용 |
| [native bridge](Doroti/src/Doroti.Hosting/DorotiNativePlatformBridge.cs)와 runner/native/binding | SDK 객체를 backend 안에 유지하고 callback을 UI thread로 marshaling |
| [templates](Doroti/templates/Doroti.Templates/content/doroti-app) 및 각 runner | opt-in package/native asset/build item 등록, final publish 배포 |
| 신규 `DorotiTestbedApp/src/WebViewFixture.cs` | 동일 local content로 플랫폼별 기능·입력·겹침·복구 검증 |
| 신규 `Doroti/validation/webview/`, `Doroti/docs/webview/` | 기능 대응표, focused validator, 배포/오류 문서 |

공통 `net10.0` 앱은 platform SDK/binding/native pointer를 노출받지 않는다. UIKit 코드 공유가 iOS/Catalyst final runner·ABI를 합치는 근거가 되지 않는다. NativeAOT를 위해 동적 reflection 객체 노출 대신 명시적 DTO/직렬화 계약을 쓴다.

### 3.2 Controller·명령·이벤트

- controller는 widget rebuild와 독립적이며 한 시점에 native attachment 하나를 소유한다. `CreateAsync` 또는 readiness task를 선택해 생성 실패·취소·dispose 결과를 노출한다.
- 준비 전 명령은 제한된 queue에서 순서를 보장하거나 명시적 NotReady로 거부한다. WV-1에서 정책·queue 상한을 고정한다. UI thread 동기 wait는 사용하지 않는다.
- `owner/instance generation + navigationId + requestId`로 응답을 검증한다. document 교체 시 이전 document JS pending request를 취소한다. SPA navigation과 새 document navigation은 구분한다.
- native created, navigation started/committed/completed, first content frame은 별도 개념이다. backend가 관측 못 하는 이벤트는 합성해서 성공 처리하지 않는다.
- navigation 오류는 main-frame/subresource와 취소/네트워크/정책/엔진 종료를 구분한다. URL/title 등의 이벤트 순서와 redirect 관계를 대응표로 남긴다.
- sync 결정만 가능한 API에는 사전 policy를 사용한다. deferral 가능 API는 timeout/cancel을 두고 UI thread를 막지 않는다.
- 외부 열기와 popup은 앱의 명시적 policy로 처리한다. WebView package가 무조건 URL launcher를 실행하거나 새 창을 띄우지 않는다.

### 3.3 JS bridge·세션·로컬 콘텐츠

JS message envelope에는 version, request ID, navigation/document 식별자, 이름, 직렬화 가능한 payload를 둔다. SDK가 제공하는 origin/frame/source를 검증하고 부족한 metadata 때문에 안전한 범위를 보장할 수 없으면 bridge 활성 범위를 제한한다. trusted app content와 임의 원격 페이지를 같은 권한으로 취급하지 않는다.

handler는 허용된 origin/frame만 대상으로 등록하고 navigation/dispose 시 오래된 handler/listener를 정리한다. 최대 message 크기·timeout·pending 수와 binary/직렬화 불가 결과 오류를 정의한다. native 객체/reflection을 웹에 공개하지 않으며 TLS 오류 무시·전체 파일 접근·권한 자동 승인을 기본값으로 넣지 않는다.

profile/environment는 view보다 긴 수명을 가질 수 있다. view dispose, profile dispose, cookie/storage/cache 삭제를 분리한다. Android 등에서 독립 profile 생성에 제약이 있으면 cookie 저장소 일부 분리만으로 완전한 격리를 선언하지 않는다.

| Backend | 앱 콘텐츠 구현 접점 | 확인할 사항 |
|---|---|---|
| Windows | virtual-host mapping 또는 resource interception | origin, path traversal, MIME, fetch, Range/seek, runtime 지원 |
| Android | AndroidX WebViewAssetLoader | asset/resource mapping, HTTPS origin, file 접근 제한, fetch/media |
| Apple | WKURLSchemeHandler 또는 제한된 file/base URL | custom scheme에서 필요한 fetch/media/보안 컨텍스트 동작, read 범위 |
| Linux Qt | 선택 API에 따른 public scheme handler 또는 지원 경로 | QWebView의 Qt Resource 제약, rich bridge/profile 요구 충족 |
| Web | 앱 서버의 same-origin URL | base URI, CSP/sandbox, origin, 상대 경로 |

loopback HTTP 서버는 기본 의존성으로 두지 않는다. 플랫폼 기능으로 충족하지 못하는 요구가 확인되면 별도 결정을 남긴다.

## 4. 단계별 실행 계획

모든 단계는 `TODO`다. 공통 계약 → Windows/Web 선행 제품 경로 → Android/Apple/Qt → 통합 검증 순서로 실행한다. Qt API 조사는 초기부터 진행할 수 있다.

### WV-0 — 기능 대응표·플랫폼 결정·검증 fixture 설계

선행: work1 PV-0 계약 초안. PV-1 구현과 독립적으로 조사 가능.

- 로컬 inappwebview widget/controller/settings/creation params/keep-alive/environment/native backend를 조사해 `Doroti/docs/webview/feature-map.md`를 작성한다.
- 각 기능에 reference symbol, Doroti API, native API, OS/runtime 하한, callback 의미, 지원 수준, 담당 WV 단계, 검증 fixture를 연결한다.
- 패키지/TFM/RID/native ABI, JS enable 기본값, trusted origin, profile 기본 수명, navigation 정책 timeout을 확정한다.
- 동일 테스트 콘텐츠를 설계한다: relative CSS/JS/image/fetch, history/redirect, main-frame/subresource 실패, input/selection, delayed message, popup/file input, media seek, cookie/storage.
- same-origin, 다른 origin의 협력 페이지, 비협력 페이지, embedding 거부 페이지를 독립 fixture로 구성한다. 외부 사이트 상태에만 의존하지 않도록 재현 서버를 계획한다.

완료 기준: 모든 목표 기능의 범위와 제약이 대응표에 있고, 최소 runtime과 성능 예산을 실제 착수 환경에서 결정할 작업이 배정되어 있다. Linux API 결정은 WV-7A가 소유한다.

### WV-1 — 공통 package·controller·widget·profile 계약

선행: WV-0 및 PV-1/PV-2 공통 기반. UI 표시 전 fake backend 검증 가능.

- 공통 typed API와 backend factory 선택을 구현하고 PlatformView handle에 widget을 연결한다.
- readiness/명령 queue/취소/navigation ID/event subscription 및 idempotent dispose를 구현한다.
- feature query, 미지원 기능 오류, 플랫폼 options, navigation policy, local resource resolver를 구현한다.
- profile/environment 소유권과 keep-alive token을 구현한다. keep-alive 재attach와 동시 attach 거부를 검증한다.

완료 기준: fake backend에서 생성 중 dispose, 준비 전 명령, navigation 후 late JS, profile 종료 중 callback, 두 controller 격리, handler 중복 등록/해제가 통과한다. 신규 core만 참조하는 앱에 WebView2/Qt native runtime 의존성이 들어가지 않는다.

### WV-2 — Windows WebView2

선행: WV-1, PV-3B. 최소 composition 객체 spike는 PV-3와 공동 진행 가능.

- **WV-2A 생성/표시:** environment·composition controller를 message pump가 있는 STA UI thread에서 생성한다. runtime 부재/지원 버전 미달/비동기 초기화 실패를 구분한다. wrapper와 C++ COM adapter 중 현재 ABI/AOT와 맞는 방식을 spike로 결정한다.
- **WV-2B 기능:** navigation/history/errors/policy, JS/user scripts/message, profile/storage/cookie, app content mapping을 구현한다. COM event token을 보관해 dispose 시 해제한다.
- **WV-2C 입력/확장:** composition controller의 mouse/pointer, cursor/capture/focus/IME를 PV-3/PV-5와 연결한다. popup/file chooser/download/permission/fullscreen/process failure는 지원표에 따라 구현한다.
- **WV-2D 배포:** WindowsAppSdk와 Windows MAUI runner를 각각 검증한다. 선택한 runtime 배포 방식, 초기화 진단, native asset 및 NativeAOT 실제 publish를 확인한다.

완료 기준: local fixture 탐색·JS·profile 격리/삭제, 초기화 중 창 닫기, process 종료/재생성, 한글 입력·selection·Tab이 실제 창에서 동작한다. 겹침 승인은 PV-3C/PV-5까지 통과해야 한다. WebView2 composition 성공은 generic HWND 전체 지원의 증거가 아니다.

WebView2의 STA/message loop와 비동기 호출 원칙은 [Microsoft threading 문서](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/threading-model)를 따른다. 로컬 inappwebview의 Flutter texture bridge를 Doroti의 직접 DComp 출력으로 그대로 옮기지 않는다.

### WV-3 — Web iframe / EmbeddedWebContent

선행: WV-1, PV-4B. 겹침 완료에는 PV-4C/PV-5 필요.

- **WV-3A 수명/로드:** stable iframe/container와 controller를 연결하고 rebuild/resize/offstage/keep-alive에서 불필요한 reload를 막는다. LoadUri와 same-origin 앱 콘텐츠를 구현한다.
- **WV-3B 제한 계약:** same-origin·협력 cross-origin·비협력 cross-origin별 지원표를 반환한다. 임의 history/cookie/JS 제어와 navigation interception을 지원한다고 가정하지 않는다.
- **WV-3C 메시지:** exact targetOrigin, event.origin/source, document handshake/request ID로 협력 bridge를 검증한다. navigation 후 이전 페이지 응답과 다른 iframe의 위조 메시지를 거부한다.
- **WV-3D embedding/입력:** CSP frame-ancestors/X-Frame-Options, iframe sandbox/allow와 호스트 COOP/COEP 조합을 확인한다. `iframe → shield → Doroti foreground`의 시각/입력 순서와 메뉴 종료 후 복귀를 검증한다.
- **WV-3E renderer 회귀:** WebGPU/WebGL을 명시적으로 선택해 각각 검증하고 main/worker protocol·context loss·iframe 수명 정합성을 확인한다.

완료 기준: 지원 범위 안의 load/message가 동작하고 범위 밖 API가 명시적 오류를 반환한다. embedding 거부를 항상 세밀하게 관측할 수 있다고 약속하지 않는다. 관측 가능한 실패 또는 앱 timeout으로 상태를 종료하고 외부 열기는 앱이 선택한다. iframe load만으로 실제 content 표시를 성공 처리하지 않는다.

검토 시 보완한 점: cross-origin isolation은 SharedArrayBuffer 등과 관련된 조건이다. 이를 WebGPU API 자체의 일률적 요구로 표현하지 않고 **Doroti 현재 worker/runtime의 요구와 iframe 응답 헤더 호환성**을 확인한다. [MDN crossOriginIsolated](https://developer.mozilla.org/en-US/docs/Web/API/Window/crossOriginIsolated), [postMessage](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage).

### WV-4 — Android WebView

선행: WV-1, PV-6B. 겹침 완료에는 PV-6C/PV-5 필요.

- **WV-4A 생성/기본 탐색:** Activity/UI-thread 소유권, WebViewClient/WebChromeClient, lifecycle 연결과 create/close 경쟁을 구현한다.
- **WV-4B 앱 기능:** navigation callback 차이, evaluateJavascript/message adapter, WebViewAssetLoader, cookie/storage/cache, user script 가능 시점과 backend options를 매핑한다. OS API와 WebView provider/version을 별도로 기록한다.
- **WV-4C 실사용:** file chooser/permission/fullscreen video/process 종료 복구를 구현한다. callback 종료·취소를 보장하고 확대/selection/키보드 inset을 확인한다.
- **WV-4D 수명/배포:** Activity 재생성, background/foreground, native Surface 재생성과 WebView 수명을 분리한다. AAR/binding/runner 및 실제 설치 패키지를 검증한다.

완료 기준: emulator x64와 arm64 기기에서 로컬 HTML/fetch/media seek, JS/message, 부모 scroll 경쟁, 한글 IME/selection, background 복귀와 route 100회 후 instance 정리를 확인한다. process cache 때문에 메모리가 즉시 0이 되지 않는 것과 Activity/WebView 참조 누수를 구분한다. texture 기반 scroll 성능을 측정 없이 더 빠르다고 채택하지 않는다.

### WV-5 — iOS / Mac Catalyst WKWebView

선행: WV-1, PV-7B. 겹침 완료에는 PV-7C/PV-5 필요.

- **WV-5A UIKit backend:** WKWebView configuration·navigation/UI delegate·script message handler·website data store를 연결한다. delegate/handler의 참조 수명과 unsubscribe를 관리한다.
- **WV-5B 기능:** navigation decision, user script 주입 시점, JS 결과/오류, app content scheme/read 범위, persistent/ephemeral session을 구현한다.
- **WV-5C 입력/복구:** touch/scroll/selection menu/키보드 inset, permission/file/media capability, web content process 종료/재생성 정책을 검증한다.
- **WV-5D 제품/NativeAOT:** iOS와 Catalyst 각각 binding·RID·native link·sign/install/run을 확인한다. 새 delegate/message bridge를 실제 publish 산출물로 실행한다.

완료 기준: iOS simulator와 실제 iPhone/iPad 해당 대상, Mac Catalyst 실행을 별도 기록한다. local content/JS/profile, 한글 조합/selection, 전경 modal/shield, VoiceOver, 재진입/종료를 확인한다. 기존 host probe나 Mono full-trim을 신규 WebView NativeAOT 성공으로 기록하지 않는다.

### WV-6 — native AppKit macOS WKWebView

선행: WV-1, PV-8B. 겹침 완료에는 PV-8C/PV-5 필요.

- UIKit과 공유 가능한 WebKit 메시지·탐색 의미만 공통화하고 NSView/responder/focus/좌표 adapter는 별도로 구현한다.
- macOS file chooser/download/popup/permission과 profile/local content capability를 구현한다.
- 두 owner 창, 키보드 탐색, pointer/wheel, window activation, close 중 callback을 검증한다.
- AppKit runner의 framework/binding/native asset와 실제 publish 실행을 확인한다.

완료 기준: AppKit 실제 앱에서 기능 fixture와 한글 IME/selection/VoiceOver·겹침·live resize가 통과한다. Catalyst 결과를 복사하지 않는다. 지원 RID별 빌드·실행·AOT 결과를 분리한다.

### WV-7 — Linux Qt WebView 계열

선행: A는 WV-0과 함께 선행 조사 가능. B 이후 WV-1 및 PV-9B 필요. 겹침 완료에는 PV-9C/PV-5 필요.

#### WV-7A: 공개 API·Qt 버전·host 결합 결정

- 설치 Qt/toolchain, 지원 배포판·architecture, X11/Wayland 환경을 조사하고 현 Qt 최소 6.5와의 차이를 기록한다.
- **우선 spike는 Qt 6.11+ QWebView/QWindow 결합**으로 한다. QWebView는 Qt 6.11에서 도입되었고 공개 API·Qt Resource 로드 제약을 확인할 수 있다. JS handler/profile 요구를 충족하는지는 별도 기능 대응표로 판정한다. [QWebView 공식 API](https://doc.qt.io/qt-6/qwebview.html).
- 필요한 기능이 QWebView 공개 API에 없으면 Qt 내부 대안인 QWebEngineView/Page 직접 adapter 또는 QML/Quick API의 비용을 비교해 **한 가지 제품 경로**를 결정한다. private backend 접근이나 runtime 자동 fallback은 사용하지 않는다.
- 결정 문서에 선택 API, Qt 최소/검증 버전, Qt 모듈, 지원 기능/제한, host 변경, package 배포, B/C 지원 수준을 적는다. Qt 계열 선택은 유지하며 WPE/CEF/Wry로 대체하지 않는다.

완료 기준: 기능 요구와 host stacking을 함께 만족하는 구현안이 확정되거나, 불가능한 기능과 제한형 제품 범위를 명시한다. 충족하지 못한 필수 요구는 `PARTIAL`로 남긴다. 이 결정 전 Qt 최소 버전을 임의로 올리지 않는다.

#### WV-7B: 초기화·native adapter·기능

- Qt WebView 모듈 경로를 선택하면 `QtWebView::initialize()`를 현 QApplication/context 생성 전에 연결한다. 직접 WebEngine 경로를 선택하면 해당 공개 API의 초기화 계약을 별도로 적용한다.
- 선택한 public API로 탐색·load/state·JS/message·profile·local content를 구현하고 미지원 기능은 query/오류로 노출한다.
- Qt signal/callback disconnect, UI thread, page/profile/view 종료 순서, engine process 종료를 처리한다.

Linux의 Qt WebView는 Qt WebEngine에 의존한다. wrapper 선택도 WebEngine 배포를 제거하지 않는다. 초기화 순서와 모듈 관계는 [Qt WebView 공식 문서](https://doc.qt.io/qt-6/qtwebview-index.html)를 기준으로 한다.

#### WV-7C: 합성·배포·제품 실행

- PV-9에서 선택한 QWindow/QWidget/Quick 구조에 연결하고 focus/clip/overlay/input shield를 확인한다. `createWindowContainer()` 경로의 opaque stacking 제약을 일반 겹침 지원으로 오해하지 않는다.
- Qt WebEngine process, resources/locales/plugins, scheme 설정, 배포 디렉터리와 native library 검색 경로를 포함해 clean 환경 실행을 검증한다.
- X11/Wayland 각각 local content/fetch/media seek, JS/profile 지원 범위, 한글 IME/selection, 접근성, resize/종료를 시험한다.

완료 기준: 선택 API/최소 버전과 실제 배포 파일이 기록되고 X11/Wayland 결과가 분리되어 있다. 개발 머신 빌드 성공만으로 clean 배포와 compositor 지원을 통과 처리하지 않는다. Qt 종속성이 WebView 미사용 Linux 앱에 강제로 추가되지 않는다.

### WV-8 — 공통 실사용·보안·복구·상태 보존 마감

선행: WV-2~WV-7의 구현 또는 명시적 제한 결정. 플랫폼별 구현 중에도 해당 검증을 수행한다.

- navigation/redirect/popup/외부 protocol 정책, file chooser 취소, download 수명, permission 거부/허용, fullscreen 진입/복귀를 같은 fixture로 비교한다.
- trusted/untrusted origin, 잘못된 frame/source, oversized/late message, TLS 오류, 앱 리소스 경로 이탈의 거부 동작을 확인한다.
- profile 공유/격리/ephemeral 종료/명시적 data clear, keep-alive 재attach, dispose 후 late callback을 검증한다.
- process 종료 후 기존 controller의 terminal 상태와 재생성 정책을 정리한다. 엔진이 보존하지 못한 history/form/media 상태를 복구된 것처럼 보고하지 않는다.

완료 기준: 지원표상 구현 feature마다 성공·실패·취소 경로가 있고 미지원 feature는 무응답/no-op으로 끝나지 않는다. UI thread deadlock, stale JS 실행, handler/listener/native instance 누수가 없다.

### WV-9 — 제품 Lab·패키지·문서·최종 승인

선행: WV-8 및 해당 플랫폼 work1 B/C·PV-5 게이트.

- WebViewFixture에 탐색 도구, local content, feature query/미지원 UI, 한글 입력/selection, 메뉴/modal, 여러 WebView, session 선택, 반복 수명 시나리오를 연결한다.
- 0/1/4 view에서 frame p50/p95/p99, native/JS 왕복 지연, 첫 content 표시 관측 가능 여부, native/GPU memory, process·listener·overlay 수를 기록한다.
- 같은 OS/runtime/기기/콘텐츠와 고정된 반복 수로 baseline 대비 측정한다. native 생성 완료나 GPU submit을 표시 시간/사용자 입력 지연으로 대신하지 않는다.
- 생성 template부터 최종 runner publish/install/run까지 선택 package가 포함되는지 확인하고 WebView 미사용 앱의 dependency/크기 회귀도 확인한다.
- API 사용 예제, 지원표, runtime 설치/초기화 오류, iframe 제한, profile/data 삭제, local content, 배포 및 재현 명령을 문서화한다.

완료 기준: 지원한다고 광고할 플랫폼/기능에 실제 증거가 있고 필수 게이트가 통과한다. target 일부의 환경 부재/합성 미해결/실기기 미검증이 남으면 전체 완료는 `PARTIAL`; 해당 기능은 `notVerified` 또는 명시적 unsupported로 유지한다.

### WV-X — 후속 확장

headless WebView, 고급 keep-alive pool, devtools UI, 인증 challenge·프록시·세밀한 request interception, 고급 browser window 관리, inappwebview 추가 API 호환은 별도 요구/성능 측정에 따라 계획한다. 공통 계약에 확장점을 두되 미구현 API를 성공하는 stub으로 공개하지 않는다.

## 5. 검증 행렬과 증거 규칙

| 게이트 | 필수 사례 | 증거 |
|---|---|---|
| Core | create/dispose 경쟁, command 순서, stale navigation, 2 owner | focused validator 결과, callback/ID trace |
| Navigation | URI/HTML/history/redirect/취소/main-frame 오류 | 재현 페이지, native 이벤트 순서, 실제 화면 |
| App content | 상대 CSS/JS/image/fetch, media Range/seek, 경로 거부 | 요청/응답 trace와 제품 표시/입력 |
| JS/message | 허용/거부 origin/frame, timeout, navigation 후 late 결과 | typed 결과·거부 원인·pending 수 복귀 |
| Session | 공유/격리, ephemeral 종료, cache/cookie/storage 삭제 | 지정 profile 데이터 확인, 다른 profile 보존 |
| 입력/겹침 | 한글 IME/selection, 부모 scroll, 메뉴/modal/shield | 실제 기기 입력, 전경/배경 클릭 카운터, video |
| Lifecycle | route 100회, create 중 close, background/foreground, process 종료 | crash/instance/listener/메모리 추이, 재진입 화면 |
| Deploy/AOT | final publish/install/run, 새 binding/JS bridge | TFM/RID/toolchain, ILC/native link, 산출물 hash, 기능 실행 |

target 행은 WindowsAppSdk, Windows MAUI, Android emulator x64/실기기 arm64, iOS simulator/device, Mac Catalyst, AppKit macOS, WebGPU/WebGL의 브라우저별 결과, Linux X11/Wayland다. 실제 지원 RID·OS·기기·browser/provider/runtime 버전은 WV-0에서 고정한다. 물리 기기 결과를 다른 OS/runtime 조합으로 확대하지 않는다.

`Doroti/artifacts/webview/<date>/<target>/`에 commit/dirty 식별, 환경, 명령·exit code·1200초 timeout 여부, feature-map 행, trace/capture/video, 실패·잔여 작업·재개 명령을 기록한다. 검증 보고서도 같은 경로에 작성하며 `Doroti/docs/validation/`에 별도 문서를 생성할 필요는 없다. 단계별 `sourceReviewed/build/automated/productLive/physical/nativeAot`를 독립적으로 남긴다. 사용자 요청 없이 `skippedByUser`로 처리하지 않는다.

모든 테스트는 [.github 지침](.github/copilot-instructions.md)의 **20분 외부 timeout**을 적용한다. 기존 [run-with-timeout.py](Doroti/validation/run-with-timeout.py)를 사용하고 플랫폼별 실제 명령은 각 adapter/validator를 구현할 때 고정한다. 계획 작성만 수행하는 이번 변경에서는 제품 build/runtime 테스트를 실행하지 않는다.

## 6. 착수 순서와 미결정 항목의 종료 지점

1. PV-0와 WV-0에서 공통 계약/기능표를 고정하고 WV-7A의 Qt 버전/API 조사를 시작한다.
2. PV-1/PV-2 및 WV-1을 구현한다. Windows PV-3/WV-2로 실제 합성·WebView 경로를 먼저 확인한다.
3. Web PV-4/PV-5/WV-3으로 DOM/worker/iframe·입력 보호를 확인한다. 두 선행 플랫폼의 결과로 공통 계약을 보정한다.
4. Android PV-6/WV-4, UIKit PV-7/WV-5, AppKit PV-8/WV-6, Qt PV-9/WV-7B/C를 진행한다.
5. PV-10과 WV-8/WV-9에서 제품·성능·배포를 마감한다. 선택 연구 PV-X/WV-X는 분리한다.

| 미결정 항목 | 결정 단계 | 결정 완료에 필요한 자료 |
|---|---|---|
| Windows wrapper/C ABI, runtime 배포 | WV-2A/D + PV-3 | 실제 DComp 결합, STA callback, AOT/배포 spike |
| 초기 profile 범위·명령 queue·policy timeout | WV-0/1 | 기능 대응표와 native API 제약, 오류/취소 fixture |
| Web same-origin/협력 bridge 범위 | WV-0/3 | origin별 동작표, CSP/COOP/COEP·sandbox 재현 |
| Qt QWebView/직접 WebEngine/QML 선택·최소 버전 | WV-7A | 필수 기능 API 대응, QWindow stacking, 설치·배포 환경 |
| backend별 interleaved 지원·효과 | 해당 PV-C + PV-5 | 실제 foreground/입력·alpha/clip·frame 정합성 |
| 기기별 성능 예산 | PV-0/WV-0, 최종 WV-9 | 기존 baseline과 동일 workload 측정, 허용 회귀 수치 |

주요 로컬 레퍼런스: [공통 widget](reference/flutter_inappwebview-master/flutter_inappwebview/lib/src/in_app_webview/in_app_webview.dart), [controller interface](reference/flutter_inappwebview-master/flutter_inappwebview_platform_interface/lib/src/in_app_webview/platform_inappwebview_controller.dart), [Windows 구현](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/in_app_webview/in_app_webview.cpp), [Web element 수명](reference/flutter_inappwebview-master/flutter_inappwebview_web/lib/web/in_app_web_view_web_element.dart). reference의 구현 존재는 Doroti 기능 구현·제품 검증 완료를 뜻하지 않는다.

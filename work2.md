# 플랫폼별 WebView 작업계획

작성일: 2026-09-11 · 검토 HEAD: `cf83abd603aa06b942051cabdeb5132f4dd774c7`

Linux 계획 재검토: 2026-09-11 · HEAD: `b2f7555438e6f865fd1b17c2a7f2880fd363da95`. [ref.md](ref.md), 현재 Qt host/runner와 공식 Qt·배포판 문서를 대조했다. 이번 변경은 Linux 계획 및 work1 인계 정리이며 구현·패키지 설치·제품 실행은 수행하지 않았다.

기준: [idea.md](idea.md), 공통 호스팅 선행 계획: [work1.md](work1.md). 이 문서는 **계획 작성 결과**다. 아래 단계는 모두 `TODO`, WebView 제품 실행·성능·NativeAOT 검증은 `notVerified`다. API/패키지/신규 산출물 경로는 구현 단계에서 확정할 제안이다.

## 1. 목표와 구현 범위

공통 `WebViewController + WebViewWidget`를 제공하고 Windows WebView2, Android WebView, iOS/Mac Catalyst WKWebView, native AppKit macOS WKWebView, Web iframe, Linux Qt backend로 연결한다. WebView2·Qt WebEngine 등의 의존성은 해당 backend를 선택한 앱에만 부여한다. Linux Qt WebEngine은 시스템 패키지로 공급하고 Doroti 배포물에는 managed adapter와 자체 native shim을 포함한다.

주 레퍼런스는 [로컬 flutter_inappwebview](reference/flutter_inappwebview-master/README.md)다. 공통 기능·생성 옵션·controller·이벤트·keep-alive/profile 의미를 참고하되 API 전체를 일괄 포팅하지 않는다. 로컬 pubspec의 `6.2.0-beta.3`는 참조 스냅샷 표기이며 최신 배포 버전이나 Doroti 검증 버전이 아니다. Linux는 idea.md의 **Qt 계열** 선택을 유지하면서 **Qt WebEngine 공개 C++ API 직접 adapter**로 구체화한다. idea.md의 QWebView 우선 실험 및 API 미결정 항목은 이번 WV-7 계획으로 대체하며, WPE/CEF/Wry/WebKitGTK 병행이나 자동 fallback은 도입하지 않는다.

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
| Linux | 시스템 Qt WebEngine 공개 API + C ABI shim | 목표 | QWebEnginePage/Script + 제한된 QWebChannel bridge 목표 | QWebEngineProfile/UrlSchemeHandler 목표 | WV-7B + PV-9C/PV-5, X11/Wayland 분리 |

Web의 cross-origin iframe은 임의 JS/history/cookie/탐색 차단을 native WebView와 동등하게 제공할 수 없다. `UnsupportedFeature`, `Unknown` 상태와 협력 bridge 지원을 구분한다. progress를 관측할 수 없으면 임의 비율을 만들지 않고, iframe load event를 HTTP 성공 또는 첫 content 표시로 단정하지 않는다.

## 3. 공통 구조와 계약

### 3.1 패키지와 실제 변경 위치

| 위치 | 책임 / 예정 산출물 |
|---|---|
| 신규 `Doroti/src/Doroti.WebView/` | controller/widget, navigation/state/settings, typed message·profile·resource 계약 |
| 신규 `Doroti.WebView.Windows`, `.Android`, `.UIKit`, `.AppKit`, `.Web`, `.Linux.Qt` | 플랫폼 factory·SDK adapter·옵션·feature query. 최종 프로젝트/TFM 분리는 WV-0에서 확정 |
| 신규 `Doroti/src/Doroti.WebView.Linux.Qt/native/` | WebEngine 공개 API를 감싼 선택형 `libdoroti_webview_qt.so` 제안. 시스템 Qt에 동적 링크하며 C ABI만 managed에 노출 |
| [Qt runner build](Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets), [Testbed native](DorotiTestbedApp/linux/native/CMakeLists.txt), [template native](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native/CMakeLists.txt) | opt-in shim build/copy/publish와 generic pre-application hook 연결. Testbed·생성 template 모두 반영 |
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
| Linux Qt | QWebEngineUrlScheme + QWebEngineUrlSchemeHandler | application 생성 전 scheme 등록, profile별 handler, origin/경로/MIME/fetch/Range·seek 검증 |
| Web | 앱 서버의 same-origin URL | base URI, CSP/sandbox, origin, 상대 경로 |

loopback HTTP 서버는 기본 의존성으로 두지 않는다. 플랫폼 기능으로 충족하지 못하는 요구가 확인되면 별도 결정을 남긴다.

## 4. 단계별 실행 계획

모든 단계는 `TODO`다. 공통 계약 → Windows/Web 선행 제품 경로 → Android/Apple/Qt → 통합 검증 순서로 실행한다. Qt 시스템 패키지·ABI 조사와 최소 host 합성 spike는 초기부터 진행할 수 있다.

### WV-0 — 기능 대응표·플랫폼 결정·검증 fixture 설계

선행: work1 PV-0 계약 초안. PV-1 구현과 독립적으로 조사 가능.

- 로컬 inappwebview widget/controller/settings/creation params/keep-alive/environment/native backend를 조사해 `Doroti/docs/webview/feature-map.md`를 작성한다.
- 각 기능에 reference symbol, Doroti API, native API, OS/runtime 하한, callback 의미, 지원 수준, 담당 WV 단계, 검증 fixture를 연결한다.
- 패키지/TFM/RID/native ABI, JS enable 기본값, trusted origin, profile 기본 수명, navigation 정책 timeout을 확정한다.
- 동일 테스트 콘텐츠를 설계한다: relative CSS/JS/image/fetch, history/redirect, main-frame/subresource 실패, input/selection, delayed message, popup/file input, media seek, cookie/storage.
- same-origin, 다른 origin의 협력 페이지, 비협력 페이지, embedding 거부 페이지를 독립 fixture로 구성한다. 외부 사이트 상태에만 의존하지 않도록 재현 서버를 계획한다.

완료 기준: 모든 목표 기능의 범위와 제약이 대응표에 있고, 최소 runtime과 성능 예산을 실제 착수 환경에서 결정할 작업이 배정되어 있다. Linux는 WebEngine 직접 API를 기준으로 WV-7A에서 버전·패키지·ABI를, WV-7B/PV-9에서 host 구조를 확정한다.

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

### WV-7 — Linux 시스템 Qt WebEngine

#### 재검토 결정과 현재 기반

`ref.md`의 시스템 패키지 활용·WebEngine 직접 연결·C ABI shim 제안을 채택한다. Linux Qt WebView도 WebEngine에 의존하므로 wrapper가 엔진 설치를 없애지는 않는다. Qt WebView의 QML overlap 제한 역시 일반 합성 지원을 보장하지 않는다. 따라서 QWebView/Qt 6.11을 필수 출발점으로 두지 않는다. [Qt WebView 공식 문서](https://doc.qt.io/qt-6/qtwebview-index.html).

- **공급 정책:** 배포판 저장소의 Qt WebEngine을 사용한다. 앱/NuGet에 Chromium·Qt WebEngine runtime을 복사하거나 엔진을 직접 빌드·다운로드하는 경로는 이번 범위에 넣지 않는다. 시스템 설치에 드는 디스크·메모리 비용은 별도로 측정한다.
- **단일 backend:** `Doroti.WebView.Linux.Qt → 자체 C ABI shim → 시스템 Qt WebEngine`으로 연결한다. ref.md 앞부분의 WPE 우선·WebKitGTK fallback 제안은 기존 Qt 선택과 맞지 않아 채택하지 않는다. 로컬 inappwebview Linux 구현도 WPE이므로 기능 의미만 참고한다.
- **API 기준:** QWebEnginePage/Profile/Script/UrlSchemeHandler와 QWebChannel을 사용한다. 표시 접점은 QWebEngineView/Widgets부터 검증하되, 현재 Graphite QWindow와의 합성이 가능하다는 뜻은 아니다. WebEngine Quick은 WV-7B에서 Widgets 경로의 구체적 제약이 확인될 때만 비교하고 제품 경로 하나를 고정한다.
- **버전 기준:** WebView 선택 앱의 Qt/WebEngine API 하한은 **6.8을 계획 기준**으로 두고 WV-7A에서 실제 최소 patch·배포판 build를 확정한다. 신규 API는 `since`/compile guard와 capability로 구분한다. WebView 미사용 generic Qt host의 현재 CMake 하한 6.5를 일괄 올리지 않는다.

아래는 2026-09-11 공식 패키지 페이지 확인값이며 지원 인증표가 아니다. ref.md의 전체 배포판 최신 버전표나 고정 설치 용량을 요구조건으로 옮기지 않는다. 설치 단계에서 저장소·architecture·보안 업데이트 상태를 다시 기록한다.

| 시스템 패키지 후보 | 확인된 WebEngine 패키지 | 계획상 용도 |
|---|---|---|
| Debian 13 stable | `qt6-webengine-dev 6.8.2+dfsg-4` | 6.8 API 하한 검증 후보. [공식 패키지](https://packages.debian.org/trixie/qt6-webengine-dev) |
| Ubuntu 26.04 LTS | `qt6-webengine-dev 6.10.2+dfsg-1`, Universe | 최신 LTS 통합 후보. [공식 패키지](https://packages.ubuntu.com/resolute/qt6-webengine-dev) |
| Arch Linux x86_64 | `qt6-webengine 6.11.2-1` | rolling update 호환성 후보. [공식 패키지](https://archlinux.org/packages/extra/x86_64/qt6-webengine/) |

현재 [Testbed runner](DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj)는 `linux-x64`다. [native host](DorotiTestbedApp/linux/native/src/doroti_qt_host.cpp)는 QApplication 하나를 만들며 Graphite에서는 QWindow, 비교 경로에서는 QOpenGLWindow를 사용한다. [CMake](DorotiTestbedApp/linux/native/CMakeLists.txt)는 Qt 6.5 Core/Gui/Widgets/OpenGL/OpenGLWidgets를 참조하며 WebEngine 연결은 없다. [QtNativeV2](Doroti/src/Doroti.Host.Qt/QtNativeV2.cs)의 이름/export는 v2이지만 실제 `AbiVersion`은 **3**이다. [runner targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets)는 현재 `libdoroti_qt_host.so`만 복사하므로 새 shim의 build/publish 연결이 필요하다.

선행 순서: **WV-7A → WV-7B(PV-9 공동 spike) → WV-7C → WV-7D → WV-7E → WV-7F**. A와 B의 최소 native 실험은 WV-0부터 시작할 수 있다. C의 controller 연결은 WV-1 및 PV-9B에 의존한다. 기능 개발은 B 표시 후 진행하되 전체 겹침 완료에는 PV-9C/PV-5가 필요하다. 아래 구현·제품 검증은 모두 `TODO`/`notVerified`다.

#### WV-7A: 시스템 패키지·API 하한·ABI 범위 확정

- 우선 `linux-x64`의 하한/최신 조합을 고르고 distro/release/repository/Qt Core·WebEngine build/compiler/glibc/libstdc++/QPA/GPU를 기록한다. arm64는 별도 runner·native build·실행 증거를 확보하기 전 지원표에 추가하지 않는다. Fedora/backports 등 추가 조합도 패키지 공급과 제품 지원을 구분한다.
- 기능표의 API별 도입 버전과 실제 제공 모듈을 확인한다. Widgets 기준 직접 의존 후보는 `Qt6::WebEngineCore`, `Qt6::WebEngineWidgets`, `Qt6::WebChannel`이다. Qt WebView/QML/Quick은 직접 의존으로 강제하지 않되 배포판 패키지가 끌어오는 전이 의존성은 실제 설치 비용에 포함한다.
- host와 shim은 같은 배포판 Qt 계열/toolchain으로 빌드한다. C ABI는 managed 경계를 안정화할 뿐 Qt C++ ABI나 glibc 차이를 제거하지 않는다. 최신 Qt에서 빌드한 바이너리를 구버전에 로드할 수 있다고 가정하지 않고, 배포판별 빌드 또는 검증된 하한 빌드의 호환 범위를 고정한다. 서로 다른 Qt 배포본을 한 process에 혼합하지 않는다.
- `Doroti/docs/webview/linux-qt.md`에 engine 선택, 최소/검증 버전, 필수/선택 기능, 개발·runtime 패키지 구분, build/RID 행렬을 작성한다. 버전 숫자만으로 Chromium 보안 수정 수준을 판정하지 않고 저장소 업데이트 정보도 기록한다.

완료 기준: 재현 가능한 시스템 패키지/toolchain 명세와 기능 대응표가 있으며, 6.8 기준 충족 여부·제외 조합·지원 범위 변경이 명시되어 있다. 조사만으로 해당 배포판 제품 지원을 승인하지 않는다.

#### WV-7B: 표시·교차 합성 구조 조기 검증 — PV-9 공동 게이트

- 실제 Graphite Vulkan QWindow와 QWebEngineView를 한 owner 안에 연결하는 최소 native spike를 만든다. generic host의 attachment/container/foreground/shield는 PV-9가 소유하고, WV-7B는 최소 live HTML 객체와 기능 probe를 공급한다. 별도 QApplication/event loop를 만들지 않는다.
- QWidget shell/`createWindowContainer()`로 비겹침 B를 검증하더라도, embedded window가 불투명하게 위에 쌓이고 겹친 여러 container의 순서가 정의되지 않는 제약을 C 설계에 그대로 적용한다. [Qt container 제약](https://doc.qt.io/qt-6/qwidget.html#createWindowContainer).
- **두 live WebView 사이와 위에 Doroti 중간·전경을 겹치는 spike**를 X11/Wayland에서 각각 실행한다. alpha, 부분/전체 가림, 역순, shield, resize와 GPU backend 공존을 확인한다. hidden/스크린샷 대체 또는 단순 raise/lower 성공으로 C를 판정하지 않는다.
- Widgets가 C를 충족하지 못하면 WebEngine Quick의 공개 API와 Qt scene graph 결합 비용을 비교한다. Quick 도입 시 pre-application 초기화·graphics API·모듈·presenter 변경 범위를 기록한다. private Chromium/Qt texture extraction이나 renderer의 조용한 전환은 채택하지 않는다.
- 결정은 `표시 API + host 계층 + renderer + QPA + B/C 제약` 단위로 남긴다. C 미해결이어도 B 기능 작업은 진행 가능하나 Linux 전체 목표는 `PARTIAL`이다. host 재설계가 필요하면 PV-9 작업에 반영하고 WebView adapter 안에 compositor를 중복 구현하지 않는다.

완료 기준: PV-9B 인계 가능한 attachment 구조와 C의 실행 가능한 설계 또는 구체적 실패 근거가 있다. 이 spike 통과와 제품 C1~C6/PV-5 최종 승인은 분리한다.

#### WV-7C: 선택형 C ABI shim·초기화·수명

- WebEngine에 동적 링크한 별도 `libdoroti_webview_qt.so`와 managed adapter를 만든다. C#은 Qt의 C++ symbol을 직접 P/Invoke하지 않고 version/struct size/feature bits, opaque handle, UTF-8 pointer+length, typed status를 가진 자체 C ABI만 호출한다. buffer allocator/free와 callback context 소유자를 명시한다.
- generic `libdoroti_qt_host.so`에 WebEngine의 필수 `DT_NEEDED`를 넣지 않는다. 앱 manifest opt-in에 따라 전용 shim을 로드하고 generic host hook을 통해 결합한다. ABI 변경은 native header/managed layout/Testbed/template/contract validator를 함께 갱신한다.
- **첫 widget 생성 시점보다 앞선 process 준비 단계를 둔다.** opt-in shim 준비와 앱 scheme 등록을 현 `doroti_qt_run_v2`의 QApplication 생성 전에 실행한다. instance/page/profile 생성은 QApplication 이후 Qt GUI thread에서 수행한다. Qt WebView 모듈을 쓰지 않으므로 `QtWebView::initialize()`를 기본 호출로 복사하지 않는다. Quick 선택 시 해당 초기화 순서를 별도 적용한다. [scheme 초기화 계약](https://doc.qt.io/qt-6/qwebengineurlscheme.html).
- create/ready/dispose, attach/detach, signal disconnect, UI-thread dispatch와 비동기 request 취소를 연결한다. page 파괴 중 발생하는 늦은 JS callback도 generation으로 거부한다. page/view 종료 뒤 profile을 정리하고 event loop 종료 전에 필요한 deferred deletion을 처리한다.
- live QObject/callback가 남은 동안 shim을 unload하지 않는다. 등록된 process hook을 포함한 module 수명을 정하고 기본적으로 process 동안 유지한다. native 예외를 ABI 밖으로 전파하지 않으며 managed callback 예외도 경계 안에서 오류로 변환한다.
- runtime 부재, 버전/ABI 불일치, helper/resources 누락, 초기화 실패를 구분한다. 선택 feature가 없으면 미로딩 상태를 유지하고, opt-in 준비 실패는 정의한 typed 오류로 반환한다. 앱 전체 종료/기능 unavailable 정책을 WV-1 계약에 맞춘다.

완료 기준: 미사용 앱은 WebEngine 없이 실행되고 선택 앱은 생성·종료 및 두 owner/late callback 검증을 통과한다. 새 shim이 build와 final publish에 실제 포함되며, v2 이름만 보고 ABI 2로 연결하지 않는다.

#### WV-7D: 탐색·JS bridge·profile·앱 콘텐츠

| 기능 | 공개 API 접점 | 구현·수용 조건 |
|---|---|---|
| 탐색/state/policy | QWebEnginePage/View, history, loadingChanged, acceptNavigationRequest | redirect·취소·실패의 의미 매핑, sync 정책에서 UI wait 금지, loadFinished와 첫 표시 구분 |
| JS/user script | runJavaScript, QWebEngineScript | world/document generation·typed 직렬화·timeout·취소, 지원 못 하는 결과와 Promise 처리 범위를 명시 |
| 양방향 메시지 | QWebChannel + 제한된 QObject facade | 명시적 등록 API만 노출, 크기/요청 수 제한, document 교체 시 해제 |
| 세션 | QWebEngineProfile, cookieStore, cache API | persistent/off-the-record·공유/격리·보관 경로, 페이지보다 긴 profile 수명, 데이터별 삭제 및 완료 관측 |
| 앱 콘텐츠 | QWebEngineUrlScheme/Handler/RequestJob | 기존 manifest resolver, 정규화 경로·MIME·origin·상대 fetch, response device 수명과 Range/media seek |

API 대응 근거: [QWebEnginePage](https://doc.qt.io/qt-6/qwebenginepage.html), [QWebChannel](https://doc.qt.io/qt-6/qwebchannel.html), [QWebEngineProfile](https://doc.qt.io/qt-6/qwebengineprofile.html).

- QWebChannel은 JS와 QObject를 연결하는 transport다. 메시지 안의 자칭 origin/frame을 신뢰 근거로 쓰지 않는다. native에서 검증 가능한 문서/프레임과 격리 world·주입 범위를 조사하고, 보장이 부족하면 bridge를 trusted app content/main frame으로 제한한다. 임의 원격 페이지에 일반 native 객체를 공개하지 않는다.
- profile 저장 경로/정책은 page 생성 전에 설정한다. cookie/cache 삭제를 전체 localStorage/IndexedDB 삭제로 보고하지 않고 데이터 종류별 API·완료 callback·재실행 결과를 기록한다. 미지원 삭제 범위는 capability로 노출한다.
- 앱 scheme의 보안/fetch 관련 flag는 필요한 범위만 설정한다. handler 설치는 profile별로 수행하고 close/cancel 중 QIODevice와 pending 요청을 정리한다. custom scheme이 HTTP의 모든 응답/Range 의미를 제공한다고 가정하지 않으며 media fixture 실패는 제한으로 남긴다.
- popup/file chooser/download/permission/fullscreen 및 render process 종료는 WV-0 기능표에 따라 구현하고 WV-8의 허용·거부·취소·복구 시나리오에 연결한다. Qt 버전에 따라 없는 API를 성공 stub으로 채우지 않는다.

완료 기준: 실제 페이지에서 기본 탐색·JS 왕복·origin 거부·profile 격리/삭제·상대 asset/fetch/media와 dispose 경쟁을 검증한다. page/widget의 상태 보존과 Chromium process 재생성 후 잃은 상태를 구분한다.

#### WV-7E: 시스템 의존성 패키징·clean 배포

- Doroti가 배포하는 파일(managed adapter, 자체 shim, host/앱 산출물)과 시스템이 공급하는 파일(Qt libraries, WebEngineProcess, resources/locales, QPA plugins, 필요한 codec)을 manifest에서 분리한다. 시스템 파일을 앱 output에 복사해 no-bundle 정책을 무효화하지 않는다. [Qt WebEngine 배포 구성](https://doc.qt.io/qt-6/qtwebengine-deploying.html).
- `.deb`는 빌드용 `qt6-webengine-dev`와 실행용 library/helper/data 패키지를 분리하고 distro의 dependency 도구와 설치 파일 목록으로 Depends를 검증한다. Core만 나열하거나 Quick library를 무조건 요구하지 않는다. Arch 및 추가 배포 형식도 해당 저장소의 실제 package closure로 작성한다.
- 시스템 prefix/QLibraryInfo와 패키지 설치 위치에 따라 helper/resources/plugins를 찾는다. 개발 PC의 절대 경로·혼합 Qt plugin·임의 LD_LIBRARY_PATH에 의존하지 않으며, resolved path와 패키지 소유자·버전을 진단에 남긴다. missing component에 필요한 설치 패키지를 안내하되 앱 시작 중 자동 설치하지 않는다.
- 일반 사용자·기본 Chromium sandbox 상태에서 clean 환경을 실행한다. `--no-sandbox`나 root 실행으로 실패를 우회한 결과를 제품 PASS로 기록하지 않는다. QPA·GPU·sandbox 실패를 각각 진단한다. [Qt 플랫폼 조건](https://doc.qt.io/qt-6/qtwebengine-platform-notes.html).
- WebEngine 없는 미사용 앱, 정상 설치된 선택 앱, runtime/버전/helper 누락 negative fixture를 분리한다. 최종 앱 크기·추가 시스템 패키지 설치량·process memory를 따로 측정하고, 보안 업데이트 후 재실행과 라이선스/notice 배포 항목을 기록한다. AppImage/Flatpak 등 별도 runtime 모델은 이번 시스템 패키지 지원과 구분해 후속으로 둔다.

완료 기준: 개발 Qt SDK 경로 없이 final publish/install/run이 재현되고 dependency manifest에 helper/data까지 포함된다. 사용하지 않는 Linux 앱에 WebEngine·WebChannel·Quick 의존성이 추가되지 않는다. NativeAOT 지원은 신규 shim/bridge가 포함된 실제 ILC/native link/publish/run으로 별도 입증한다.

#### WV-7F: X11/Wayland 제품·입력·합성·성능 승인

- `배포판/build × RID × QPA(xcb/wayland) × compositor × GPU/driver × Qt/WebEngine 버전`으로 결과를 나눈다. XWayland를 native X11과, WSLg/VM을 물리 Linux와 구분한다. 최초 지원 RID는 linux-x64이며 미실행 후보는 `notVerified`다.
- Testbed 제품에서 기본 기능/JS/profile/asset, 두 WebView, keep-alive 재attach, 생성 중 close, process failure, live resize/DPR/scroll을 실행한다. generic control 성공과 실제 WebEngine view 성공을 분리한다.
- PV-9C/PV-5와 함께 C1~C6, 한글 IME 조합/selection·Tab 왕복, wheel/drag/capture·modal shield, 접근성/Orca를 확인한다. native 포함 창 캡처·입력 trace로 검증하며 raster readback만으로 native 화면을 판정하지 않는다.
- WV-9 기준 0/1/4 view의 frame/JS 왕복/입력 지연·process/GPU memory를 같은 환경에서 비교한다. 제품 Graphite와 WebEngine GPU의 공존 여부, software 경로 사용 여부를 기록한다. renderer 전환으로 성공시킨 진단 결과는 원래 조합의 PASS가 아니다.

완료 기준: 광고할 각 조합의 기능·배포·C1~C6/PV-5 증거가 있고 잔여 제한이 지원표와 일치한다. B만 가능하거나 물리 입력/접근성/합성이 미검증이면 Linux 목표는 `PARTIAL`이며 다른 backend 성공으로 승격하지 않는다.

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

1. PV-0와 WV-0에서 공통 계약/기능표를 고정하고 WV-7A의 시스템 Qt 버전/패키지/ABI 조사를 시작한다. WV-7B/PV-9A에서 최소 WebEngine 표시·합성 구조를 조기에 검증한다.
2. PV-1/PV-2 및 WV-1을 구현한다. Windows PV-3/WV-2로 실제 합성·WebView 경로를 먼저 확인한다.
3. Web PV-4/PV-5/WV-3으로 DOM/worker/iframe·입력 보호를 확인한다. 두 선행 플랫폼의 결과로 공통 계약을 보정한다.
4. Android PV-6/WV-4, UIKit PV-7/WV-5, AppKit PV-8/WV-6, Qt PV-9/WV-7C~F를 진행한다. Linux는 WV-7B/PV-9B 인계 후 기능을 개발하고 PV-9C/PV-5 제품 승인과 시스템 패키지 배포 검증을 완료한다.
5. PV-10과 WV-8/WV-9에서 제품·성능·배포를 마감한다. 선택 연구 PV-X/WV-X는 분리한다.

| 미결정 항목 | 결정 단계 | 결정 완료에 필요한 자료 |
|---|---|---|
| Windows wrapper/C ABI, runtime 배포 | WV-2A/D + PV-3 | 실제 DComp 결합, STA callback, AOT/배포 spike |
| 초기 profile 범위·명령 queue·policy timeout | WV-0/1 | 기능 대응표와 native API 제약, 오류/취소 fixture |
| Web same-origin/협력 bridge 범위 | WV-0/3 | origin별 동작표, CSP/COOP/COEP·sandbox 재현 |
| Linux Qt/WebEngine 최소 patch·시스템 패키지·ABI 지원 범위 | WV-7A/E | 6.8 API 기준 검증, distro별 build/runtime closure, clean publish/install/run |
| Linux WebEngine 표시 API·host 계층 | WV-7B + PV-9A/B/C | QWebEngineView 우선 spike, 필요 시 Quick 비교, Graphite 공존·실제 겹침·입력 증거 |
| backend별 interleaved 지원·효과 | 해당 PV-C + PV-5 | 실제 foreground/입력·alpha/clip·frame 정합성 |
| 기기별 성능 예산 | PV-0/WV-0, 최종 WV-9 | 기존 baseline과 동일 workload 측정, 허용 회귀 수치 |

주요 로컬 레퍼런스: [공통 widget](reference/flutter_inappwebview-master/flutter_inappwebview/lib/src/in_app_webview/in_app_webview.dart), [controller interface](reference/flutter_inappwebview-master/flutter_inappwebview_platform_interface/lib/src/in_app_webview/platform_inappwebview_controller.dart), [Windows 구현](reference/flutter_inappwebview-master/flutter_inappwebview_windows/windows/in_app_webview/in_app_webview.cpp), [Web element 수명](reference/flutter_inappwebview-master/flutter_inappwebview_web/lib/web/in_app_web_view_web_element.dart). reference의 구현 존재는 Doroti 기능 구현·제품 검증 완료를 뜻하지 않는다.


## work0 renderer contract handoff (2026-09-12)

공식 SkiaSharp NativeAssets `4.154.0-preview.1.26454.9`가 기본이다. Vulkan은 실제 1.2 profile, 공개 session 생성, 성공한 queue submit 순서의 observer 상태를 사용한다. 각 raster segment의 일반 R과 플랫폼 소유 P를 구분하고 GPU copy 뒤 R을 관찰된 L/ownership으로 복원한다. host fence 완료와 플랫폼 front/present retirement는 별도 경계다. segment identity·paint order·alpha·frame admission 계약을 유지해야 한다.

구 `SkiaGraphiteVulkanOptions`/private ABI 생성 경로는 제거했다. 새 직접 소비자는 `CreateOfficialVulkan`의 typed observed-state/retirement 계약을 사용한다. 정상 desktop 제품은 manifest 없이 표준 공식 DLL을 검증하고, 프로세스 안에서 native 자산을 교체하지 않는다. timeout을 device loss로 바꾸거나 미완료 generation을 재사용하지 않는다. Apple/Linux는 코드 구성만 완료했으며 검증은 `skippedByUser`다. 전체 성능 수용은 미완료다. [전환 및 검증 범위](Doroti/docs/validation/official-graphite-cutover-2026-09-12.md)를 따른다.
